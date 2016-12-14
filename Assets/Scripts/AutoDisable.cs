using UnityEngine;
using System.Collections;

namespace SpaceLeague.Utility
{
    public class AutoDisable : MonoBehaviour 
    {
        [SerializeField] private float delayTime;

        private WaitForSeconds waitTime;

        private void Awake()
        {
            waitTime = new WaitForSeconds(delayTime);
        }

        private void OnEnable()
        {
            StopAllCoroutines();
            StartCoroutine(Disable());
        }

        private IEnumerator Disable()
        {
            yield return waitTime;
            gameObject.SetActive(false);
        }
    }
}