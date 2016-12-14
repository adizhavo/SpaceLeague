using UnityEngine;
using SpaceLeague.Ship;
using SpaceLeague.CameraShake;

namespace SpaceLeague.Ship.Player
{
    public class PlayerShip : Ship 
    {    
        [SerializeField] private RectTransform aimUI;
        [HideInInspector] public float aimSensibility = 1f;

        public Transform targetShip;

        private void Start()
        {
            #if !UNITY_EDITOR
            AimSensibility /= 80f;
            #endif

            Init(movementSpeed, 0.7f, shipCamera);
        }

        public void Init(float aimSensibility, float movementSpeed, float rotationAngleStepPercentage, Transform shipCamera = null)
        {
            this.aimSensibility = aimSensibility > 0f ? aimSensibility : 0f;
            base.Init(movementSpeed, rotationAngleStepPercentage, shipCamera);
        }

        protected override void Update()
        {
            base.Update();

            PositionAimUI();

            #if PLAYER_SHIP_LOG
            DisplayLogs();
            #endif
        }

        private void FixedUpdate () 
        {
            UpdateShipDirection();
            MoveShipForward();
            UpdateCameraPosition();
        }

        private void PositionAimUI()
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(GlobalDirection);
            aimUI.position = screenPos;
        }

        public override void Damaged(Transform attackingShip, float damage)
        {
            #if UNITY_EDITOR
            Debug.Log("Player damaged by " + attackingShip.name);
            #endif
            CameraShakeProvider.Instance.StartShake(ShakeType.Hit);
        }
    }
}
