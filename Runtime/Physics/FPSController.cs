using System;
using UnityEngine;
using UnityEngine.InputSystem;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;

namespace WolfeyFPS
{
    [RequireComponent(typeof(Rigidbody))]
    public class FPSController : NetworkBehaviour
    {
        [Header("Settings")] 
        public float friction = 60f;
        public float slidingFriction = 20f;
        public float accel = 90f;
        public float slidingAccel = 25f;
        public float maxGroundSpeed = 10f;
        public float maxAirSpeed = 15f;
        public float rotSensitivity = 0.05f;
        public float jumpPower = 15f;
        public float interpolationTicks = 1f;
        public float maxSpeedMult = 1f;
        public float fricGain = 2f;
        public float hitFricMult = 1f;
        public float slideHeight;
        public float slideGrav;
        public float slideTransitionSpd = 10f;
        public float slidePushForce;
        public float ladderClimbAccel;
        public float ladderMaxSlideSpeed;
        public float ladderSideAccel;
        
        [Header("Inputs")] 
        [SerializeField] InputAction movementInput;
        [SerializeField] InputAction rotationInput;
        [SerializeField] InputAction jumpInput;
        [SerializeField] InputAction slideInput;
        
        [Header("References")] 
        [SerializeField] GroundCheck gCheck;
        [SerializeField] GroundCheck headCheck;
        [SerializeField] Transform cam;
        [SerializeField] Transform head;
        [SerializeField] Animator bodyAnimator;
        [SerializeField] Transform tpsRot;
        [SerializeField] CapsuleCollider bean;
        [SerializeField] Transform weaponBodyRoot;
        
        float _defaultBeanHeight;
        float _defaultHeadOffset;
        float _defaultWeaponBodyRootOffset;

        public bool LadderClimbing { get; set; }
        
        bool _sliding;
        bool _paused;
        public Rigidbody Rb { get; private set; }
        Transform _transform;
        TickData _ownerTickData;
        Quaternion Rot => Quaternion.Euler(_ownerTickData.serverRot.x, _ownerTickData.serverRot.y, 0f);
        Quaternion HorizontalRot => Quaternion.Euler(0f, _ownerTickData.serverRot.y, 0f);
        Quaternion ObserversRot => Quaternion.Euler(tickData.serverRot.x, tickData.serverRot.y, 0f);
        Quaternion HorizontalObserverRot => Quaternion.Euler(0f, tickData.serverRot.y, 0f);

        float Friction => bodyAnimator.GetBool(SlideID) ? slidingFriction : friction;
        float Accel => LadderClimbing ? ladderSideAccel : bodyAnimator.GetBool(SlideID) ? slidingAccel : accel;
        
        const float MoveInputThresh = 0.1f;
        const float MinVerticalRot = -90f;
        const float MaxVerticalRot = 90f;
        const float RotInterp = 180f;
        float _slideLerp;
        
        static readonly int xinputID = Animator.StringToHash("xinput");
        static readonly int zinputID = Animator.StringToHash("zinput");
        static readonly int GroundedID = Animator.StringToHash("Grounded");
        static readonly int SlideID = Animator.StringToHash("Slide");
        static readonly int HeadCheckID = Animator.StringToHash("HeadCheck");

        [HideInInspector]
        [SyncVar(Channel = Channel.Unreliable, ReadPermissions = ReadPermission.ExcludeOwner, SendRate = 0f)]
        public TickData tickData;
        
        [Serializable]
        public struct TickData
        {
            public Vector3 serverPos;
            public Vector2 serverRot;
            public Vector2 inputVectors;
        }
        
        void Awake()
        {
            Rb = GetComponent<Rigidbody>();
            _transform = transform;
            var height = bean.height;
            _defaultBeanHeight = height;
            _defaultHeadOffset = height - head.localPosition.y;
            _defaultWeaponBodyRootOffset = height - weaponBodyRoot.localPosition.y;
            jumpInput.performed += JumpInputOnperformed;
            headCheck.GroundedSwitch += HeadCheckOnGroundedSwitch;
        }

        void HeadCheckOnGroundedSwitch()
        {
            bodyAnimator.SetBool(HeadCheckID, headCheck.Grounded);
        }

        void OnDestroy()
        {
            jumpInput.performed -= JumpInputOnperformed;
        }
        

        [ServerRpc]
        void PerformSlideServer(bool pressed)
        {
            PerformSlideObserver(pressed);
        }

        [ObserversRpc(ExcludeOwner = true, BufferLast = true)]
        void PerformSlideObserver(bool pressed)
        {
            PerformSlide(pressed);
        }

        void PerformSlide(bool pressed)
        {
            bodyAnimator.SetBool(SlideID, pressed);
            if (!pressed)
            {
                SetUnSlide();
            }
        }

        void JumpInputOnperformed(InputAction.CallbackContext obj)
        {
            if(!gCheck.Grounded) return;
            Rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            
            Rb.position = transform.parent.position;
            
            if (!IsOwner)
            {
                //Sketchy phyiscs
                //_rb.useGravity = false;
                Rb.isKinematic = true;
                return;
            }
            
            movementInput.Enable();
            rotationInput.Enable();
            jumpInput.Enable();
            slideInput.Enable();
            Cursor.lockState = CursorLockMode.Locked;
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            if(!IsOwner) return;
            movementInput.Disable();
            rotationInput.Disable();
            jumpInput.Disable();
            slideInput.Disable();
            
            Cursor.lockState = CursorLockMode.None;
        }

        public void DisableMovement()
        {
            Rb.isKinematic = true;
            
            movementInput.Disable();
            jumpInput.Disable();
            slideInput.Disable();
            _ownerTickData.inputVectors = Vector2.zero;
        }

        public void EnableMovement()
        {
            movementInput.Enable();
            jumpInput.Enable();
            slideInput.Enable();
        }

        public void SpawnSwitched()
        {
            if(!IsOwner) return;
            Rb.isKinematic = false;
        }

        void ApplyFriction()
        {
            if(!gCheck.Grounded || LadderClimbing) return;
            Vector3 vel = Rb.velocity;
            float speed = vel.magnitude;
            float newSpeed = Mathf.Max(0f, speed - Friction * hitFricMult * Time.fixedDeltaTime);
            hitFricMult = Mathf.Min(1f, hitFricMult + fricGain * Time.fixedDeltaTime);
            Rb.velocity = vel.normalized * newSpeed;
        }
        
        void AddMoveForce()
        {
            _ownerTickData.inputVectors = movementInput.ReadValue<Vector2>();

            if (_ownerTickData.inputVectors.sqrMagnitude < MoveInputThresh)
            {
                return;
            }

            Vector3 wishDir = _transform.TransformDirection(new Vector3(_ownerTickData.inputVectors.x, 0, _ownerTickData.inputVectors.y));
            
            Vector3 currentVelocity = Rb.velocity;

            float maxSpeed = gCheck.Grounded ? maxGroundSpeed : maxAirSpeed;
            
            if (LadderClimbing)
            {
                //perhaps reduce left / right movement
                currentVelocity.y = _ownerTickData.inputVectors.y * ladderClimbAccel;
                maxSpeed = ladderMaxSlideSpeed;
            }
            
            maxSpeed *= maxSpeedMult;
            float currentSpeed = Vector3.Dot(currentVelocity, wishDir);
            float addSpeed = Mathf.Clamp(maxSpeed - currentSpeed, 0f, Accel * Time.fixedDeltaTime);
            
            Rb.velocity = currentVelocity + addSpeed * wishDir;
        }

        [ServerRpc]
        void SetSyncVars(TickData newtickData, Channel channel = Channel.Unreliable)
        {
            tickData = newtickData;
        }

        void ReadRotInput()
        {
            Vector2 rotDelta = rotationInput.ReadValue<Vector2>();
            rotDelta *= rotSensitivity;

            _ownerTickData.serverRot.x = Mathf.Clamp(
                _ownerTickData.serverRot.x - rotDelta.y,
                MinVerticalRot,
                MaxVerticalRot
            );
            
            _ownerTickData.serverRot.y += rotDelta.x;
        }

        public void SetRot(Vector2 rot)
        {
            _ownerTickData.serverRot = rot;
        }

        void UpdateCamera()
        {
            cam.SetPositionAndRotation(head.position, Rot);
        }

        void RotateRb()
        {
            Rb.MoveRotation(HorizontalRot);
        }

        void CheckSlide()
        {
            bool pressed = slideInput.IsPressed();
            if(pressed == bodyAnimator.GetBool(SlideID)) return;
            PerformSlideServer(pressed);
            PerformSlide(pressed);
        }
        
        public void SetSlide()
        {
            _slideLerp = 0f;
            _sliding = true;
            
            if(!IsOwner) return;
            var velocity = Rb.velocity;
            velocity += velocity.normalized * slidePushForce;
            Rb.velocity = velocity;
        }

        public void SetUnSlide()
        {
            _slideLerp = 0f;
            _sliding = false;
        }
        
        Vector3 TargetBeanCenter => _sliding 
            ? new Vector3(0f, slideHeight * 0.5f, 0f) 
            : new Vector3(0f, _defaultBeanHeight * 0.5f, 0f);
        
        float TargetBeanHeight => _sliding ? slideHeight : _defaultBeanHeight;

        Vector3 TargetHeadLocalPos => _sliding
            ? new Vector3(0f, slideHeight - _defaultHeadOffset, 0f)
            : new Vector3(0f, _defaultBeanHeight - _defaultHeadOffset, 0f);
        
        Vector3 TargetWeaponBodyLocalPos => _sliding 
            ? new Vector3(0f, slideHeight - _defaultWeaponBodyRootOffset, 0f)
            : new Vector3(0f, _defaultBeanHeight - _defaultWeaponBodyRootOffset, 0f);
        
        void ApplySlide()
        {
            bean.height = Mathf.Lerp(bean.height, TargetBeanHeight, _slideLerp);
            bean.center = Vector3.Lerp(bean.center, TargetBeanCenter, _slideLerp);
            
            if(!IsOwner || !_sliding) return;
            Rb.velocity += Vector3.down * (slideGrav * Time.fixedDeltaTime);
        }

        void ApplySlideVisual()
        {
            _slideLerp = Mathf.Min(1f, _slideLerp + Time.deltaTime * slideTransitionSpd);
            
            head.localPosition = Vector3.Lerp(head.localPosition, TargetHeadLocalPos, _slideLerp);
            weaponBodyRoot.localPosition =
                Vector3.Lerp(weaponBodyRoot.localPosition, TargetWeaponBodyLocalPos, _slideLerp);
        }
/*
        void OnCollisionEnter(Collision collision)
        {
            //Perhaps try here to resolve a slide floor hit
            
        }
*/
        [Client]
        void FixedUpdate()
        {
            if (!IsOwner)
            {
                ApplySlide();
                return;
            }

            if (!Rb.isKinematic)
            {
                CheckSlide();
                ApplyFriction();
                if (!_paused)
                {
                    AddMoveForce();
                    ApplySlide();
                }
            }

            if (!_paused)
            {
                RotateRb();
            }

            tickData.serverRot = _ownerTickData.serverRot;
            tickData.inputVectors = _ownerTickData.inputVectors;
            tickData.serverPos = _transform.position;
            SetSyncVars(tickData);
        }
        
        void ObserverUpdate()
        {
            if(!IsSpawned) return;
            
            bodyAnimator.SetFloat(xinputID, tickData.inputVectors.x);
            bodyAnimator.SetFloat(zinputID, tickData.inputVectors.y);
            bodyAnimator.SetBool(GroundedID, gCheck.Grounded);

            Vector3 interpolatedPos = Vector3.MoveTowards(
                _transform.position,
                tickData.serverPos,
                maxGroundSpeed * Time.deltaTime
            );
            
            interpolatedPos = Vector3.Lerp(
                interpolatedPos,
                tickData.serverPos,
                interpolationTicks * Time.deltaTime);
            
            float rotInterp = RotInterp * Time.deltaTime;
            tpsRot.rotation = Quaternion.Slerp(tpsRot.rotation, ObserversRot, rotInterp);
            Quaternion interpolatedHorizontalRot = Quaternion.Slerp(_transform.rotation, HorizontalObserverRot, rotInterp);
            
            _transform.SetPositionAndRotation(interpolatedPos, interpolatedHorizontalRot);
        }

        void OwnerUpdate()
        {
            if(!IsSpawned) return;
            bodyAnimator.SetFloat(xinputID, tickData.inputVectors.x);
            bodyAnimator.SetFloat(zinputID, tickData.inputVectors.y);
            bodyAnimator.SetBool(GroundedID, gCheck.Grounded);
        }


        [Client]
        void Update()
        {
            if (IsOwner)
            {
                OwnerUpdate();
            }
            
            ApplySlideVisual();
            
            if (!IsOwner)
            {
                ObserverUpdate();
                return;
            }

            if (!_paused)
            {
                ReadRotInput();
            }
        }
        
        [Client]
        void LateUpdate()
        {
            if (!IsOwner)
            {
                return;
            }
            
            UpdateCamera();
        }

        public void Pause()
        {
            _paused = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public void UnPause()
        {
            _paused = false;
            if(IsOffline) return;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
