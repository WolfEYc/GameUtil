using UnityEngine;

namespace WolfeyFPS
{
    [CreateAssetMenu(menuName = "WolfeyFPS/Gun Info", fileName = "New Gun Info", order = 0)]
    public class GunInfo : ScriptableObject
    {
        [Header("References")]
        public FPSGun prefab;

        [Header("Stats")] 
        public int price;
        public int killAward;

        public bool fullAuto;
        public float roundsPerMinute;
        
        public int magCapacity;
        public int carryCapacity;
        
        public float dmgPerShot;
        public float dmgFalloffRange;
        public float dmgFalloffMult;
        public float headShotMultiplier;

        public float maxRange;
        public int tracerEvery;
        public float moveSpeed;
        public float taggingPower;

        public float movementInaccuracy;
        public AnimationCurve xRecoil, yRecoil;
    }
}
