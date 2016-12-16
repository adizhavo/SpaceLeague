using UnityEngine;
using SpaceLeague.Ship;
using SpaceLeague.CameraShake;

namespace SpaceLeague.Ship.Player
{
    public class PlayerShip : AbstractShip 
    {    
        [SerializeField] private RectTransform aimUI;
        [HideInInspector] public float moveSensibility;
        [HideInInspector] public float aimSensibility;

        public Transform targetShip;

        private void Start()
        {
            #if !UNITY_EDITOR
            moveSensibility = 0.5f;
            aimSensibility = 10f;
            #else
            moveSensibility = 1f;
            aimSensibility = 1f;
            #endif

            aimUI = GameObject.FindGameObjectWithTag("PlayerHUD").GetComponent<RectTransform>();
            shipCamera = GameObject.FindGameObjectWithTag("MainCameraContainer").transform;
            Init(movementSpeed, 0.75f, shipCamera);
        }

        public void Init(float aimSensibility, float movementSpeed, float rotationAngleStepPercentage, Transform shipCamera = null)
        {
            this.moveSensibility = aimSensibility > 0f ? aimSensibility : 0f;
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
            Vector3 screenPos = Camera.main.WorldToScreenPoint(ShootingDirection);
            aimUI.position = screenPos;
        }

        public override void Damage(Transform attackingShip, float damage)
        {
            base.Damage(attackingShip, damage);

            #if UNITY_EDITOR
            Debug.Log("Player damaged by " + attackingShip.name);
            #endif
            CameraShakeProvider.Instance.StartShake(ShakeType.Hit);
        }

        private void OnTriggerStay(Collider other) 
        {
            CameraFX.instance.AddDirt();
            other.gameObject.SetActive(false);
        }
    }
}
