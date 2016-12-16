using UnityEngine;
using UnityEngine.UI;

namespace SpaceLeague
{
    public class CameraFX : MonoBehaviour 
    {
        public static CameraFX instance
        {
            private set;
            get;
        }

        [SerializeField] private DirtFX dirtEffect;

        private void Awake()
        {
            instance = this;
        }

        public void AddDirt()
        {
            dirtEffect.Apply();
        }
    }

    [System.Serializable]
    public class DirtFX
    {
        public RectTransform[] DirtPiece;

        public void Apply()
        {
            RectTransform dirt = GetPiece();

            if (dirt == null) return;

            LeanTween.alpha(dirt, 1f, 0.1f).setDelay(Random.Range(0f, 0.5f)).setOnComplete(
                ()=>
                {
                    LeanTween.alpha(dirt, 0f, 1f).setOnComplete(
                        ()=>
                        {
                            dirt.gameObject.SetActive(false);
                        }
                    );
                }
            );
        }

        private RectTransform GetPiece()
        {
            foreach(RectTransform g in DirtPiece)
                if (!g.gameObject.activeSelf)
                {
                    g.gameObject.SetActive(true);
                    return g;
                }

            return null;
        }
    }
}