using UnityEngine;
using UnityEngine.UI;
using SpaceLeague.Ship;
using SpaceLeague.Ship.Player;
using SpaceLeague.Ship.Weapon;

namespace SpaceLeague.Ship.Player.Inputs
{
    public class PlayerInputs : MonoBehaviour 
    {
        [SerializeField] private PlayerShip playerShip;
        [SerializeField] private ShipMainCannon mainWeapon;

        public Transform target;

    	private void Update () 
        {
            UpdateControls();
    	}

        private void UpdateControls()
        {
            #if UNITY_EDITOR
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            if (playerShip.currentFightMode.Equals(Ship.ShipFightMode.Normal)) playerShip.localMoveDirection =  new Vector3(horizontal, vertical, 0f) * playerShip.aimSensibility;        
            else { }

            if (Input.GetKey(KeyCode.Space)) mainWeapon.OpenFire();
//            if (Input.GetKey(KeyCode.LeftControl) && playerShip.currentFightMode.Equals(Ship.ShipFightMode.Normal)) playerShip.EnterDogFight(target);
            #else
            MapTouches(); 
            if (fireTouchIndex != -1) mainWeapon.OpenFire();
            if (touchPadIndex != -1) playerShip.LocalMoveDirection += Input.GetTouch(touchPadIndex).deltaPosition * playerShip.AimSensibility;
            else playerShip.LocalMoveDirection = Vector2.Lerp(playerShip.LocalMoveDirection, Vector2.zero, Time.deltaTime * ShipConfig.DirectionResetSpeed);
            #endif
        }

        #if !UNITY_EDITOR
        private int touchPadIndex = -1;
        private int fireTouchIndex = -1;

        private void MapTouches()
        {
            if (Input.touchCount > 0)
            {
                touchPadIndex = Input.GetTouch(0).position.x > Screen.width / 2 ? 0 : -1;
                fireTouchIndex = Input.GetTouch(0).position.x < Screen.width / 2 ? 0 : -1;

                if (Input.touchCount > 1)
                {
                    if (fireTouchIndex != -1) touchPadIndex = 1;
                    if (touchPadIndex != -1) fireTouchIndex = 1;
                }
            }
            else
            {
                touchPadIndex = -1;
                fireTouchIndex = -1;
            }
        }
        #endif
    }
}