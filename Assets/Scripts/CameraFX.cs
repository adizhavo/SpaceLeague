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
        [SerializeField] private HitFX hitEffect;

        private void Awake()
        {
            instance = this;
        }

        public void AddDirt()
        {
            dirtEffect.Apply();
        }

        public void ShowHit()
        {
            hitEffect.Apply();
        }
    }

    [System.Serializable]
    public class DirtFX
    {
        public RectTransform[] DirtPiece;

        public void Apply()
        {
            int amount = Random.Range(1, DirtPiece.Length);

            for (int i = 0; i < amount; i++)
            {
                RectTransform dirt = GetPiece();

                if (dirt == null)
                    return;

                LeanTween.alpha(dirt, 1f, 0.1f).setDelay(Random.Range(0f, 1f)).setOnComplete(
                    () =>
                    {
                        LeanTween.alpha(dirt, 0f, Random.Range(0.7f, 3f)).setOnComplete(
                            () =>
                            {
                                dirt.gameObject.SetActive(false);
                            }
                        );
                    }
                );
            }
        }

        private RectTransform GetPiece()
        {
            foreach (RectTransform g in DirtPiece)
                if (!g.gameObject.activeSelf)
                {
                    g.gameObject.SetActive(true);
                    return g;
                }

            return null;
        }
    }

    [System.Serializable]
    public class HitFX
    {
        public RectTransform[] HitPiece;

        public void Apply()
        {
            RectTransform hit = GetPiece();

            if (hit == null)
                return;

            LeanTween.alpha(hit, 1f, 0.1f).setDelay(Random.Range(0f, 1f)).setOnComplete(
                () =>
                {
                    LeanTween.alpha(hit, 0f, Random.Range(0.7f, 3f)).setOnComplete(
                        () =>
                        {
                            hit.gameObject.SetActive(false);
                        }
                    );
                }
            );
        }

        private RectTransform GetPiece()
        {
            foreach (RectTransform g in HitPiece)
                if (!g.gameObject.activeSelf)
                {
                    g.gameObject.SetActive(true);
                    return g;
                }

            return null;
        }
    }
}