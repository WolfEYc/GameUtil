using UnityEngine;

namespace WolfeyFPS
{
    public class HandsLink : MonoBehaviour
    {
        [SerializeField] FPSWeaponHandler weaponHandler;
        [SerializeField] FPSController controller;

        public void SetReloadDone()
        {
            weaponHandler.SetReloadFinished();
        }

        public void SetSwitchReady()
        {
            weaponHandler.SetSwitchReady();
        }

        public void SetMinusAttack()
        {
            weaponHandler.SetMinusAttack();
        }

        public void ReEquip()
        {
            weaponHandler.ReEquip();
        }

        public void PerformAction()
        {
            weaponHandler.PerformAction();
        }

        public void SlideStarted()
        {
            controller.SetSlide();
        }
    }
}
