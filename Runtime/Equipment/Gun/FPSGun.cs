using System;
using FishNet.Object;
using UnityEngine;
using UnityEngine.VFX;
using Wolfey.Audio;

namespace WolfeyFPS
{
    [RequireComponent(typeof(IGun), typeof(FPSEquip))]
    public class FPSGun : NetworkBehaviour
    {
        [Header("References")]
        public GunInfo gunInfo;
        
        [Header("Sound FX")] 
        [SerializeField] PlayRandomClip shotFailed;
        [SerializeField] PlayRandomClip shotSuccess;
        [SerializeField] PlayRandomClip reloadFailed;
        [SerializeField] PlayRandomClip reloadSuccess;

        [Header("Visual FX")] 
        [SerializeField] VisualEffect muzzleFlash;

        public FPSEquip Equip { get; private set; }
        
        public IGun Gun { get; set; }
        
        bool _attackHeld;
        bool _reloading;
        bool _waitingForRpm;

        int _roundsInMag;
        int _roundsCarried;
        float _lastFireTime;
        float _fireDelay;

        const float AttackLenience = 0.1f;

        bool CanShoot => _roundsInMag > 0 && _attackHeld;
        bool CanReload => _roundsInMag < gunInfo.magCapacity && _roundsCarried > 0 && !_reloading;

        bool Equipped => Equip.Equipped;
        FPSWeaponHandler WeaponHandler => Equip.WeaponHandler;
        Animator Hands => WeaponHandler.Hands;
        CameraRecoil CamRecoil => WeaponHandler.recoil;
        bool UnderAttackActual => Time.time > _lastFireTime + _fireDelay;
        bool UnderAttackLenience => Time.time > _lastFireTime + _fireDelay - AttackLenience;

        void Awake()
        {
            Equip = GetComponent<FPSEquip>();

            _fireDelay = 60f / gunInfo.roundsPerMinute;
            Equip.OnPlusAttack += PlusAttack;
            Equip.OnMinusAttack += MinusAttack;
            Equip.OnReload += Reload;
            Equip.OnReloadFinished += OnReloadFinished;
            Equip.OnUnEquip += UnEquip;

            _roundsInMag = gunInfo.magCapacity;
            _roundsCarried = gunInfo.carryCapacity;
        }

        void OnReloadFinished()
        {
            int maxToFill = Math.Min(_roundsCarried, gunInfo.magCapacity);
            int diff = maxToFill - _roundsInMag;
            _roundsInMag = maxToFill;
            _roundsCarried -= diff;
            _reloading = false;
        }

        void ReloadFailed()
        {
            //finish this stub
            reloadFailed.Play();
            
        }

        void ShotFailed()
        {
            //finish this stub
            shotFailed.Play();
        }

        void PerformReload()
        {
            reloadSuccess.Play();
            Hands.SetTrigger(FPSWeaponHandler.Reload);
        }

        [ObserversRpc(ExcludeOwner = true)]
        void ReloadObserver()
        {
            PerformReload();
        }
        
        [ServerRpc]
        void ReloadServer()
        {
            ReloadObserver();
        }
        
        void Reload()
        {
            if (!CanReload)
            {
                ReloadFailed();
                return;
            }

            _reloading = true;
            
            ReloadServer();
            PerformReload();
        }

        void PlusAttack()
        {
            if(!UnderAttackLenience || !Hands.GetBool(FPSWeaponHandler.SwitchReady)) return;
            
            if (_roundsInMag < 1)
            {
                ShotFailed();
                return;
            }
            
            _attackHeld = true;
        }

        void MinusAttack()
        {
            if (gunInfo.fullAuto)
            {
                _attackHeld = false;
            }
        }

        void UnEquip()
        {
            _reloading = false;
            _attackHeld = false;
        }
        
        void PlayShotSounds()
        {
            shotSuccess.Play();
            muzzleFlash.Play();
        }

        [ObserversRpc(ExcludeOwner = true)]
        void ShootObserver()
        {
            PlayShotSounds();
            Hands.SetTrigger(FPSWeaponHandler.Attack);
        }

        [ServerRpc]
        void ShootServer()
        {
            ShootObserver();
        }
        
        void Shoot()
        {
            _attackHeld = gunInfo.fullAuto;
            
            PlayShotSounds();
            ShootServer();
            
            Gun.Shoot(CamRecoil.Shoot(gunInfo));
            Hands.SetTrigger(FPSWeaponHandler.Attack);

            _roundsInMag--;
            _waitingForRpm = true;
            _lastFireTime = Time.time;
        }

        void Update()
        {
            if(!UnderAttackActual) return;

            if (_waitingForRpm)
            {
                _waitingForRpm = false;
            }
            
            if(!CanShoot) return;
            
            Shoot();
        }
    }
}
