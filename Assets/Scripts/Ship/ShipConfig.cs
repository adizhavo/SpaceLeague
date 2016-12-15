using UnityEngine;
using System.Collections.Generic;

namespace SpaceLeague.Ship
{
    public static class ShipConfig
    {
        public const float MaxAimDirectionMagnitude = 1f;
        public const float DirectionResetSpeed = 3f;
        public const float ShipFieldOfView = 100f;

        public const float RenderMaxRotationAngle = 50f;
        public const float RenderRotationSpeed = 3f;

        public static readonly Vector3 CameraPositionOffset = new Vector3(0f, 3.5f, 0f);
        public const float CameraDistance = 9f;

        public static readonly Vector3 DogFightCameraPositionOffset = new Vector3(1f, 2f, 0f);
        public const float DogFightCameraDistance = 1f;

        public const float CameraLerpSpeed = 3f;

        public const float DogFightDistance = 4f;
    }

    public static class ShipUtils
    {
        private static List<AbstractShip> registeredShips = registeredShips = new List<AbstractShip>();

        public static void RegisterShip(AbstractShip s)
        {
            if (!registeredShips.Contains(s)) registeredShips.Add(s);
        }

        public static void UnRegisterShip(AbstractShip s)
        {
            if (registeredShips.Contains(s)) registeredShips.Remove(s);
        }

        public static Transform LookForShipToDogFight(AbstractShip requester)
        {
            foreach(AbstractShip s in registeredShips)
                if (s.Equals(requester)) continue;
                else if(IsTargetOnSight(s.transform, requester.transform, requester.GlobalDirection)) return s.transform;

            return null;
        }

        public static bool IsTargetOnSight(Transform target, Transform observer, Vector3 observerDirection)
        {
            if (target == null) return false;

            Vector3 targetProjection = Vector3.ProjectOnPlane(target.position - observer.position, observer.forward);
            Vector2 targetLocalProjection = observer.InverseTransformPoint(targetProjection);
            Vector3 globalDirectionProjection = Vector3.ProjectOnPlane(observerDirection - observer.position, observer.forward);
            Vector2 globalDirectionLocalProjection = observer.InverseTransformPoint(globalDirectionProjection);

            Rect aimRect = new Rect(
                globalDirectionLocalProjection.x - 3f,
                globalDirectionLocalProjection.y - 3f, 
                6f, 
                6f);

            bool isInsideRect = aimRect.Contains(targetLocalProjection);

            Vector3 targetVerticalProjection = Vector3.ProjectOnPlane(target.position - observer.position, observer.up);
            Vector3 targetVerticalLocalProjection = observer.InverseTransformPoint(observer.position + targetVerticalProjection);
            int sign = (int)Mathf.Sign(targetVerticalLocalProjection.z);
            return sign > 0 && isInsideRect;
        }

        public static Transform ChooseTargetToFightForAI(AbstractShip ship)
        {
            AbstractShip closesShip = null;
            float closesDistanceSoFar = Mathf.Infinity;
            foreach(AbstractShip s in registeredShips)
            {
                if (s.Equals(ship)) continue;
                if (closesShip == null) closesShip = s;
                else
                {
                    float distance = Vector3.Distance(ship.transform.position, s.transform.position);
                    if (closesDistanceSoFar > distance)
                    {
                        closesDistanceSoFar = distance;
                        closesShip = s;
                    }
                }
            }

            return closesShip.transform;
        }
    }
}