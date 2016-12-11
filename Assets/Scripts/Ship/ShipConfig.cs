using UnityEngine;

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
        public const float CameraLerpSpeed = 3f;
    }
}