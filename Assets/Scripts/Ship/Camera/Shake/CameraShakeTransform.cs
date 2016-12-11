using UnityEngine;

namespace SpaceLeague.CameraShake
{
    public class CameraShakeTransform : MonoBehaviour
    {    
        public float LerpSpeed;

        private void Update () 
        {
            CameraShakeProvider.Instance.FrameFeed();
            Vector2 shakePoint = CameraShakeProvider.Instance.GetShakePoint();
            Vector3 localShakePoint = new Vector3(shakePoint.x, shakePoint.y, transform.localPosition.z);
            transform.localPosition = Vector3.Lerp(transform.localPosition, localShakePoint, LerpSpeed * Time.deltaTime);
        }
    }
}