using UnityEngine;
using SpaceLeague.Pooling;

namespace SpaceLeague.Ship
{
    public enum ShipFlyMode
    {
        Normal,
        DogFight
    }

    public abstract class AbstractShip : MonoBehaviour
    {
        [SerializeField] protected Transform ship;
        [SerializeField] protected Transform shipRender;
        [SerializeField] protected Transform shipCamera;
        [SerializeField] protected float movementSpeed;
        [SerializeField] public float maxDogFightFiller;
        [SerializeField] protected float dogFightPointsPerHit;
        [SerializeField] protected float maxHealth;

        [SerializeField] private GameObject damageSmoke;

        protected float currentHealth;

        [HideInInspector] public Vector2 localMoveDirection;
        [HideInInspector] public Vector2 localAimDirection;

        [HideInInspector] public bool IsReadyForDogFight
        {
            get { return currentDogFightFiller + Mathf.Epsilon >= maxDogFightFiller && IsPositionedForDogFight && ShipToDogFight != null; }
        }

        protected float rotationAngleStepPercentage = 0.6f;
        [HideInInspector] public float currentDogFightFiller = 0;

         public ShipFlyMode currentFlyMode;
        [HideInInspector] public bool IsPositionedForDogFight = false;
        [HideInInspector] public Transform ShipToDogFight;

        private float currentCameraDistance;
        private Vector3 currentCameraOffset;
        private float dogFightTimeElapse = 0f;

        public Vector3 ShootingDirection
        {
            get
            {
                if (currentFlyMode.Equals(ShipFlyMode.DogFight)) 
                {
                    localAimDirection = Vector3.ClampMagnitude(localAimDirection, ShipConfig.MaxAimDirectionMagnitude);
                    return ship.position + Vector3.MoveTowards(ship.forward * ShipConfig.ShipFieldOfView, 
                        ship.TransformDirection(localAimDirection), 
                        ShipConfig.ShipFieldOfView * rotationAngleStepPercentage);
                }
                else return GlobalDirection;
            }
        }

        public Vector3 GlobalDirection
        {
            get 
            {
                localMoveDirection = Vector2.ClampMagnitude(localMoveDirection, ShipConfig.MaxMoveDirectionMagnitude);
                return ship.position + Vector3.MoveTowards(
                    ship.forward * ShipConfig.ShipFieldOfView, 
                    ship.TransformDirection(localMoveDirection), 
                    ShipConfig.ShipFieldOfView * rotationAngleStepPercentage);
            }
        }

        private void Awake()
        {
            currentFlyMode = ShipFlyMode.Normal;
            currentCameraOffset = ShipConfig.CameraPositionOffset;
            currentCameraDistance = ShipConfig.CameraDistance;

            currentHealth = maxHealth;
        }

        private void OnEnable()
        {
            ShipUtils.RegisterShip(this);
        }

        private void OnDisable()
        {
            ShipUtils.UnRegisterShip(this);
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
            int sign = -1 * (int)Mathf.Sign(localMoveDirection.x);
            float currentAngle = shipRender.localEulerAngles.z;
            if (currentAngle > 180f) currentAngle -= 360f;
            float rotationPercentage = Mathf.Abs(localMoveDirection.x) / ShipConfig.MaxMoveDirectionMagnitude;
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


            Vector3 positionOffset = currentFlyMode.Equals(ShipFlyMode.Normal) ? Vector3.zero : ship.InverseTransformDirection(Vector3.ProjectOnPlane(ShipToDogFight.position - ship.position, ship.forward));
            Vector3 targetOffset = currentFlyMode.Equals(ShipFlyMode.Normal) ? ShipConfig.CameraPositionOffset : ShipConfig.DogFightCameraPositionOffset;
            targetOffset.x *= Mathf.Sign(positionOffset.x);
            targetOffset.y *= Mathf.Sign(positionOffset.y);


            currentCameraDistance = Mathf.Lerp(currentCameraDistance, 
                currentFlyMode.Equals(ShipFlyMode.Normal) ? ShipConfig.CameraDistance : ShipConfig.DogFightCameraDistance,
                Time.deltaTime * 3f);
            currentCameraOffset = Vector3.Lerp(currentCameraOffset, 
                targetOffset,
                Time.deltaTime * 3f);

            shipCamera.position = Vector3.Lerp(
                shipCamera.position,
                ship.position - ship.forward * currentCameraDistance + ship.TransformDirection(currentCameraOffset) ,
                Time.deltaTime * ShipConfig.CameraLerpSpeed);

            shipCamera.LookAt(GlobalDirection, ship.up);
        }

        protected virtual void DisplayLogs()
        {
            Debug.DrawLine(ship.position, ship.position + ship.TransformDirection(localMoveDirection), Color.red);
            Debug.DrawLine(ship.position, GlobalDirection);
        }

        public void EnterDogFight()
        {
            currentFlyMode = ShipFlyMode.DogFight;
            dogFightTimeElapse = 0f;
        }

        public void ExitDogFightMode()
        {
            currentFlyMode = ShipFlyMode.Normal;
        }

        protected virtual void Update()
        {
            if (currentFlyMode.Equals(ShipFlyMode.DogFight) && ShipToDogFight != null)
            {
                dogFightTimeElapse += Time.deltaTime;
                currentDogFightFiller = maxDogFightFiller - dogFightTimeElapse;
                if (currentDogFightFiller < 0 || !ShipToDogFight.gameObject.activeSelf)
                {
                    currentDogFightFiller = 0f;
                    ExitDogFightMode();
                }

                CalculateAimDirection(ShipToDogFight.position + (-1 * ShipToDogFight.forward *  ShipConfig.DogFightDistance), 5f);
            }
            else
            {
                ShipToDogFight = ShipUtils.LookForShipToDogFight(this);
                IsPositionedForDogFight = ShipToDogFight != null;
            }
        }

        protected void CalculateAimDirection(Vector3 targetTr, float reactionSpeed)
        {
            Vector3 projection = Vector3.ProjectOnPlane(targetTr - ship.position, ship.forward);
            Vector3 localProjection = ship.InverseTransformPoint(ship.position + projection);

            float angle = Vector3.Angle(targetTr - ship.position, GlobalDirection - ship.position);
            if (angle < 2f) localMoveDirection = Vector3.Lerp(localMoveDirection, Vector3.zero, Time.deltaTime * ShipConfig.DirectionResetSpeed);
            else localMoveDirection = Vector3.Lerp(localMoveDirection, localProjection, Time.deltaTime * reactionSpeed);
        }

        public void AddDogFightPoints()
        {
            if (currentFlyMode.Equals(ShipFlyMode.DogFight)) return;

            currentDogFightFiller += dogFightPointsPerHit;
            if (currentDogFightFiller > maxDogFightFiller) currentDogFightFiller = maxDogFightFiller;
        }

        public virtual void Damage(Transform attackingShip, float damage)
        {
            currentHealth -= damage;
            if (currentHealth < 0f) Destroy();
            else if (currentHealth < maxHealth * .3f) damageSmoke.SetActive(true);
        }

        protected virtual void Destroy()
        {
            damageSmoke.SetActive(false);
            gameObject.SetActive(false);

            PoolProvider.Instance.RequestGameObject(PooledObject.Explosion).transform.position = ship.position;
            PoolProvider.Instance.RequestGameObject(PooledObject.DirtZone).transform.position = ship.position;

            LeanTween.delayedCall(5f, ()=>{ gameObject.SetActive(true); });

            Awake();
        }
    }
}