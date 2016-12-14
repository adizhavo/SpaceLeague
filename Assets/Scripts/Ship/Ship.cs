using UnityEngine;

namespace SpaceLeague.Ship
{
    public abstract class Ship : MonoBehaviour
    {
        [SerializeField] protected Transform ship;
        [SerializeField] protected Transform shipRender;
        [SerializeField] protected Transform shipCamera;
        [SerializeField] protected float movementSpeed;
        [SerializeField] public float maxDogFightFiller;
        [SerializeField] protected float dogFightPointsPerHit;

        [HideInInspector] public Vector2 LocalMoveDirection;
        [HideInInspector] public bool readyForDogFight
        {
            get { return currentDogFightFiller + Mathf.Epsilon >= maxDogFightFiller; }
        }

        protected float rotationAngleStepPercentage = 0.6f;
        [HideInInspector] public float currentDogFightFiller = 0;

        public enum ShipFightMode
        {
            Normal,
            DogFight
        }

        public ShipFightMode currentFightMode;
        protected Transform chaisedShip;

        private float currentCameraDistance;
        private Vector3 currentCameraOffset;
        private float dogFightTimeElapse = 0f;

        public Vector3 GlobalDirection
        {
            get 
            {
                LocalMoveDirection = Vector3.ClampMagnitude(LocalMoveDirection, ShipConfig.MaxAimDirectionMagnitude);
                return ship.position + Vector3.MoveTowards(
                    ship.forward * ShipConfig.ShipFieldOfView, 
                    ship.TransformDirection(LocalMoveDirection), 
                    ShipConfig.ShipFieldOfView * rotationAngleStepPercentage);
            }
        }

        private void Awake()
        {
            currentFightMode = ShipFightMode.Normal;
            currentCameraOffset = ShipConfig.CameraPositionOffset;
            currentCameraDistance = ShipConfig.CameraDistance;
        }

        public void Init(float movementSpeed, float rotationAngleStepPercentage, Transform shipCamera = null)
        {
            this.movementSpeed = movementSpeed > 0f ? movementSpeed : 0f;
            this.rotationAngleStepPercentage = Mathf.Clamp01(rotationAngleStepPercentage);
            this.shipCamera = shipCamera;
        }

        protected void UpdateShipDirection()
        {
            ship.LookAt(GlobalDirection, ship.up);
            UpdateRenderDirection();
        }

        private void UpdateRenderDirection()
        {
            int sign = -1 * (int)Mathf.Sign(LocalMoveDirection.x);
            float currentAngle = shipRender.localEulerAngles.z;
            if (currentAngle > 180f) currentAngle -= 360f;
            float rotationPercentage = Mathf.Abs(LocalMoveDirection.x) / ShipConfig.MaxAimDirectionMagnitude;
            float lerpedAngle = Mathf.Lerp(0f, ShipConfig.RenderMaxRotationAngle * sign, rotationPercentage);
            shipRender.localEulerAngles = new Vector3(0f, 0f, lerpedAngle);
        }

        protected void MoveShipForward()
        {
            ship.Translate(Vector3.forward * movementSpeed * Time.deltaTime, Space.Self);
        }

        protected void UpdateCameraPosition()
        {
            if (shipCamera == null) return;

            currentCameraDistance = Mathf.Lerp(currentCameraDistance, 
                currentFightMode.Equals(ShipFightMode.Normal) ? ShipConfig.CameraDistance : ShipConfig.DogFightCameraDistance,
                Time.deltaTime * 3f);
            currentCameraOffset = Vector3.Lerp(currentCameraOffset, 
                currentFightMode.Equals(ShipFightMode.Normal) ? ShipConfig.CameraPositionOffset : ShipConfig.DogFightCameraPositionOffset,
                Time.deltaTime * 3f);

            shipCamera.position = Vector3.Lerp(
                shipCamera.position,
                ship.position - ship.forward * currentCameraDistance + ship.TransformDirection(currentCameraOffset),
                Time.deltaTime * ShipConfig.CameraLerpSpeed);

            shipCamera.LookAt(GlobalDirection, ship.up);
        }

        protected virtual void DisplayLogs()
        {
            Debug.DrawLine(ship.position, ship.position + ship.TransformDirection(LocalMoveDirection), Color.red);
            Debug.DrawLine(ship.position, GlobalDirection);
        }

        public void EnterDogFight(Transform chaisedShip)
        {
            this.chaisedShip = chaisedShip;
            currentFightMode = ShipFightMode.DogFight;
            dogFightTimeElapse = 0f;

        }

        public void ExitDogFightMode()
        {
            chaisedShip = null;
            currentFightMode = ShipFightMode.Normal;
        }

        protected virtual void Update()
        {
            if (currentFightMode.Equals(ShipFightMode.DogFight))
            {
                CalculateAimDirection(chaisedShip.position + (-1 * chaisedShip.forward *  ShipConfig.DogFightDistance) , 2f);

                dogFightTimeElapse += Time.deltaTime;
                if (dogFightTimeElapse > ShipConfig.DogFightTime)
                {
                    ExitDogFightMode();
                }
            }
        }

        protected void CalculateAimDirection(Vector3 targetTr, float reactionSpeed)
        {
            Vector3 projection = Vector3.ProjectOnPlane(targetTr - ship.position, ship.forward);
            Vector3 localProjection = ship.InverseTransformPoint(ship.position + projection);

            float angle = Vector3.Angle(targetTr - ship.position, GlobalDirection - ship.position);
            if (angle < 2f) LocalMoveDirection = Vector3.Lerp(LocalMoveDirection, Vector3.zero, Time.deltaTime * ShipConfig.DirectionResetSpeed);
            else LocalMoveDirection = Vector3.Lerp(LocalMoveDirection, localProjection, Time.deltaTime * reactionSpeed);
        }

        public void AddDogFightPoints()
        {
            currentDogFightFiller += dogFightPointsPerHit;
            if (currentDogFightFiller > maxDogFightFiller) currentDogFightFiller = maxDogFightFiller;
        }

        public abstract void Damaged(Transform attackingShip, float damage);
    }
}