using UnityEngine;

namespace SpaceLeague.Ship
{
    public abstract class Ship : MonoBehaviour
    {
        [SerializeField] protected Transform ship;
        [SerializeField] protected Transform shipRender;
        [SerializeField] protected Transform shipCamera;
        [SerializeField] protected float movementSpeed;

        [HideInInspector] public Vector2 AimDirection;

        protected float rotationAngleStepPercentage = 0.6f;

        public Vector3 GlobalDirection
        {
            get 
            {
                AimDirection = Vector3.ClampMagnitude(AimDirection, ShipConfig.MaxAimDirectionMagnitude);
                return ship.position + Vector3.MoveTowards(
                    ship.forward * ShipConfig.ShipFieldOfView, 
                    ship.TransformDirection(AimDirection), 
                    ShipConfig.ShipFieldOfView * rotationAngleStepPercentage);
            }
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
            int sign = -1 * (int)Mathf.Sign(AimDirection.x);
            float currentAngle = shipRender.localEulerAngles.z;
            if (currentAngle > 180f) currentAngle -= 360f;
            float rotationPercentage = Mathf.Abs(AimDirection.x) / ShipConfig.MaxAimDirectionMagnitude;
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

            shipCamera.position = Vector3.Lerp(
                shipCamera.position,
                ship.position - ship.forward * ShipConfig.CameraDistance + ship.TransformDirection(ShipConfig.CameraPositionOffset),
                Time.deltaTime * ShipConfig.CameraLerpSpeed);

            shipCamera.LookAt(GlobalDirection, ship.up);
        }

        protected virtual void DisplayLogs()
        {
            Debug.DrawLine(ship.position, ship.position + ship.TransformDirection(AimDirection), Color.red);
            Debug.DrawLine(ship.position, GlobalDirection);
        }

        public abstract void Damaged(Transform attackingShip, float damage);
    }
}