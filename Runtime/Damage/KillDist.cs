using FishNet.Object;
using UnityEngine;
using Wolfey.Events;
using WolfeyFPS;

namespace SpearMonkey
{
    [RequireComponent(typeof(Rigidbody))]
    public class KillDist : NetworkBehaviour, IDamageable
    {
        [SerializeField] PlayerSpawnerHandler spawnerHandler;
        [SerializeField] float yHeight;
        [SerializeField] EventObject deathEvent, selfDeathEvent, recieveNotif;
        [SerializeField] float killLenience;
        [SerializeField] AudioClip elimOther;
        
        Rigidbody _rb;
        NetworkObject _lastDamager;
        Sprite _lastDamageTool;
        
        float _lastDamagedTime;

        bool WithinKillTime => _lastDamagedTime + killLenience > Time.time;

        public DeathData GetDeathData()
        {
            NetworkObject killer = null;
            Sprite killTool = null;
            
            if (WithinKillTime)
            {
                killer = _lastDamager;
                killTool = _lastDamageTool;
            }
            
            DeathData deathdata = new DeathData(NetworkObject, transform)
            {
                Killer = killer,
                killTool = killTool
            };
            
            return deathdata;
        }

        void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        void DeathEvent()
        {
            DeathData deathdata = GetDeathData();
            deathEvent.Invoke(deathdata);
            
            if (!ReferenceEquals(deathdata.Killer, null) && deathdata.Killer.Owner == LocalConnection)
            {
                NetworkedSteamUser steamUser = deathdata.DeadObject.GetComponent<NetworkedSteamUser>();
                recieveNotif.Invoke(new ReceiveNotifData(deathdata.Owner,
                    $"Eliminated {steamUser.SteamUser.Nickname}", Color.red));
                
                
                //UIAudio.PlayClip(elimOther);
            }
            
            if (IsOwner)
            {
                selfDeathEvent.Invoke(deathdata);

                if (!ReferenceEquals(deathdata.Killer, null))
                {
                    NetworkedSteamUser steamUser = deathdata.Killer.GetComponent<NetworkedSteamUser>();
                    recieveNotif.Invoke(new ReceiveNotifData(deathdata.Owner,
                        $"{steamUser.SteamUser.Nickname} Eliminated You"));
                }
            }
        }

        [ObserversRpc(ExcludeOwner = true, ExcludeServer = true)]
        void OnDeath()
        {
            DeathEvent();
        }

        [ServerRpc(RunLocally = true)]
        public void NotifyServerOfDeath()
        {
            DeathEvent();
            if (IsServer)
            {
                OnDeath();
            }
        }
        
        void FixedUpdate()
        {
            if (!IsOwner || spawnerHandler.Dead || _rb.position.y > yHeight) return;
            
            NotifyServerOfDeath();
        }

        public void Damage(float amt, Vector3 pt, Vector3 source, NetworkObject damager, Sprite damageTool)
        {
            //Debug.Log($"DAMAGED BY", damager.gameObject);
            _lastDamager = damager;
            _lastDamagedTime = Time.time;
            _lastDamageTool = damageTool;
        }
    }
}
