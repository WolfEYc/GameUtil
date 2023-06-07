using System;
using FishNet.Connection;
using FishNet.Object;
using SpearMonkey;
using UnityEngine;
using UnityEngine.InputSystem;
using Wolfey.Events;


namespace WolfeyFPS
{
    public class FPSWeaponHandler : NetworkBehaviour
    {
        [Header("References")] 
        public PlayerSpawnerHandler spawnHandler;
        
        public Animator fpsHands;
        public Animator tpsHands;
        
        public SkinnedMeshRenderer tpsRenderer, fpsRenderer;
        public Transform camTransform;
        public CameraRecoil recoil;
        public LayerMask damageInflictLayer;
        public FPSController controller;
        public Rigidbody rb;

        public Transform bodyWeaponRoot, fpsWeaponRoot;
        public EventObject equipChanged;
        public EventObject equipIdxChanged;
        public KillDist killDist;

        [Header("Inputs")]
        [SerializeField] InputAction plusAttack;
        [SerializeField] InputAction minusAttack;
        [SerializeField] InputAction reload;
        [SerializeField] InputAction[] equipsInputs;
        [SerializeField] InputActionListener interactionListener;
        
        FPSEquip[] _equips;

        RuntimeAnimatorController _defaultController;
        
        public Animator Hands => Owner.IsLocalClient ? fpsHands : tpsHands;
        public Transform WeaponHoldTransform => Owner.IsLocalClient ? fpsWeaponRoot : bodyWeaponRoot;
        
        static readonly int Equip = Animator.StringToHash("Equip");
        public static readonly int SwitchReady = Animator.StringToHash("Switch");
        public static readonly int Attack = Animator.StringToHash("Attack");
        public static readonly int Reload = Animator.StringToHash("Reload");
        public static readonly int MinusAttack = Animator.StringToHash("MinusAttack");
        public static readonly int Held = Animator.StringToHash("MouseHeld");

        int _currentEquipIdx;
        
        bool _bound;

        public bool HasEquipment()
        {
            for (int i = 0; i < MaxEquips; i++)
            {
                if (_equips[i] == null) continue;
                return true;
            }

            return false;
        }

        const int MaxEquips = 3;
        public FPSEquip CurrentEquip => _equips[_currentEquipIdx];

        Action<InputAction.CallbackContext>[] _equipActions;
        
        Action<InputAction.CallbackContext> GetEquipAction(int idx)
        {
            return _ => EquipAtIdx(idx);
        }
        
        void Awake()
        {
            _equips = new FPSEquip[MaxEquips];
            _defaultController = Hands.runtimeAnimatorController;
            _equipActions = new Action<InputAction.CallbackContext>[MaxEquips];

            for (int i = 0; i < MaxEquips; i++)
            {
                _equipActions[i] = GetEquipAction(i);
            }
        }

        void OnEnable()
        {
            ReBind();
        }

        void OnDisable()
        {
            UnBind();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!IsOwner)
            {
                tpsHands.SetBool(Held, true);
            }
        }

        public void ReBind()
        {
            if(_bound) return;
            _bound = true;
            plusAttack.performed += PlusAttackOnperformed;
            minusAttack.performed += MinusAttackOnperformed;
            reload.performed += ReloadOnperformed;

            for (int i = 0; i < equipsInputs.Length; i++)
            {
                equipsInputs[i].performed += _equipActions[i];
            }
        }
        
        public void UnBind()
        {
            plusAttack.performed -= PlusAttackOnperformed;
            minusAttack.performed -= MinusAttackOnperformed;
            reload.performed -= ReloadOnperformed;

            for (int i = 0; i < equipsInputs.Length; i++)
            {
                equipsInputs[i].performed -= _equipActions[i];
            }

            _bound = false;
        }

        public void DitchEquipment()
        {
            for (int i = 0; i < _equips.Length; i++)
            {
                DitchEquip(i);
            }
        }

        public void DitchEquip(int equipIdx)
        {
            if(_equips[equipIdx] == null) return;

            _equips[equipIdx].transform.parent = null;
            
            if (IsServer)
            {
                Despawn(_equips[equipIdx].NetworkObject, DespawnType.Destroy);
            }

            LoseEquip(equipIdx);
        }

        public void LoseEquip(int equipIdx)
        {
            _equips[equipIdx] = null;
            equipChanged.Invoke(new EquipEventData(equipIdx, null, Owner));
            if(equipIdx != _currentEquipIdx) return;
            
            if (Owner.IsLocalClient)
            {
                EquipAtIdx(_currentEquipIdx);
            } else
            {
                PerformEquip(_currentEquipIdx);
            }
        }

        /// <summary>
        /// ONLY CALL THIS FROM FPSEquip
        /// </summary>
        /// <param name="equip"></param>
        public void PickupObserver(FPSEquip equip)
        {
            _equips[equip.EquipIdx] = equip;
            equipChanged.Invoke(new EquipEventData(equip.EquipIdx, equip, Owner));
            
            if (Owner.IsLocalClient)
            {
                EquipAtIdx(equip.EquipIdx);
            } else if (equip.EquipIdx == _currentEquipIdx)
            {
                PerformEquip(equip.EquipIdx);
            }
        }
        
        public void Enable()
        {
            if (!Owner.IsLocalClient)
            {
                recoil.gameObject.SetActive(false);
                tpsRenderer.enabled = true;
                fpsRenderer.enabled = false;
                return;
            }

            tpsRenderer.enabled = false;
            fpsRenderer.enabled = true;
        }
        
        public void EnableInputs()
        {
            if(!IsOwner) return;
            plusAttack.Enable();
            minusAttack.Enable();
            reload.Enable();
            interactionListener.enabled = true;

            foreach (InputAction equipAction in equipsInputs)
            {
                equipAction.Enable();
            }
        }
        
        public void DisableInputs()
        {
            if(!IsOwner) return;
            
            plusAttack.Disable();
            minusAttack.Disable();
            reload.Disable();
            interactionListener.enabled = false;
            
            foreach (InputAction equipAction in equipsInputs)
            {
                equipAction.Disable();
            }
        }

        public void Disable()
        {
            DitchEquipment();
            
            if (!Owner.IsLocalClient)
            {
                tpsRenderer.enabled = false;
                return;
            }
            DisableInputs();
            
            //EquipAtIdx(0);

            fpsRenderer.enabled = false;
        }
        
        void ReloadOnperformed(InputAction.CallbackContext obj)
        {
            _equips[_currentEquipIdx]?.Reload();
        }

        void PlusAttackOnperformed(InputAction.CallbackContext obj)
        {
           _equips[_currentEquipIdx]?.PlusAttack();
        }
        
        void MinusAttackOnperformed(InputAction.CallbackContext obj)
        {
            _equips[_currentEquipIdx]?.MinusAttack();
        }

        public void UpdateMouseHeld()
        {
            Hands.SetBool(Held, plusAttack.IsPressed());
        }
        
        public void SetSwitchReady()
        {
            /*
            Hands.ResetTrigger(Attack);
            Hands.ResetTrigger(MinusAttack);
            */
            _equips[_currentEquipIdx]?.SwitchReady();
            Hands.SetTrigger(SwitchReady);
        }

        public void SetReloadFinished()
        {
            _equips[_currentEquipIdx]?.ReloadFinished();
        }

        public void SetMinusAttack()
        {
            Hands.SetTrigger(MinusAttack);
        }

        public void PerformAction()
        {
            _equips[_currentEquipIdx]?.PerformAction();
        }

        public void ReEquip()
        {
            EquipAtIdx(_currentEquipIdx);
        }
        
        void EquipAtIdx(int idx)
        {
            EquipAtIdxServer(idx);
            PerformEquip(idx);
        }
        
        [ServerRpc]
        void EquipAtIdxServer(int idx)
        {
            PerformEquipObserver(idx);
        }

        [ObserversRpc(BufferLast = true, ExcludeOwner = true)]
        void PerformEquipObserver(int idx)
        {
            PerformEquip(idx);
        }

        void PerformEquip(int idx)
        {
            _currentEquipIdx = idx;
            equipIdxChanged.Invoke(new EquipIdxData(_currentEquipIdx, Owner));
            
            foreach (FPSEquip equip in _equips)
            {
                if(equip == null) continue;
                equip.UnEquip();
            }

            if (_equips[_currentEquipIdx] != null)
            {
                _equips[_currentEquipIdx].Equip();
                Debug.Log("EQUIPPING!", gameObject);
            }
            else
            {
                Hands.runtimeAnimatorController = _defaultController;
            }

            Hands.ResetTrigger(SwitchReady);
            Hands.ResetTrigger(Attack);
            Hands.ResetTrigger(MinusAttack);
            Hands.SetTrigger(Equip);
        }

        public class EquipIdxData : NetworkEventData
        {
            public int idx;
            
            public EquipIdxData(int idx, NetworkConnection owner) : base(owner)
            {
                this.idx = idx;
            }
        }
    }
}
