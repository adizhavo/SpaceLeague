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

    	private void Update () 
        {
            UpdateControls();
    	}

        private void UpdateControls()
        {
            #if UNITY_EDITOR
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            if (playerShip.currentFlyMode.Equals(ShipFlyMode.Normal)) playerShip.localMoveDirection = new Vector3(horizontal, vertical, 0f) * playerShip.moveSensibility;        
            else playerShip.localAimDirection = new Vector3(horizontal, vertical, 0f) * playerShip.aimSensibility * ShipConfig.MaxAimDirectionMagnitude;

            if (Input.GetKey(KeyCode.Space)) mainWeapon.OpenFire();
            if (Input.GetKey(KeyCode.Tab) && playerShip.IsReadyForDogFight) playerShip.EnterDogFight();

            #else
            MapTouches(); 
            if (fireTouchIndex != -1) mainWeapon.OpenFire();
            if (touchPadIndex != -1 && playerShip.currentFlyMode.Equals(ShipFlyMode.Normal)) 
                playerShip.localMoveDirection += Input.GetTouch(touchPadIndex).deltaPosition * playerShip.moveSensibility * Time.deltaTime;
            else if (touchPadIndex != -1) 
                playerShip.localAimDirection += Input.GetTouch(touchPadIndex).deltaPosition * playerShip.aimSensibility * Time.deltaTime;
            else 
            {
                playerShip.localMoveDirection = Vector2.Lerp(playerShip.localMoveDirection, Vector2.zero, Time.deltaTime * ShipConfig.DirectionResetSpeed);
                playerShip.localAimDirection = Vector2.Lerp(playerShip.localAimDirection, Vector2.zero, Time.deltaTime * ShipConfig.DirectionResetSpeed);
            }
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