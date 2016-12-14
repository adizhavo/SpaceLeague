using UnityEngine;
using SpaceLeague.Pooling;

namespace SpaceLeague.Ship.Weapon
{
    public class Bullet : MonoBehaviour 
    {
        [SerializeField] private float damage;
        [SerializeField] private float movementSpeed;
        [SerializeField] private float lifeTime;

        private float timeCounter = 0f;

        private Vector3? previousPos;
        private RaycastHit hitObject;

        [HideInInspector] public Transform fireSource;

    	private void Update() 
        {
            transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime, Space.Self);

            timeCounter += Time.deltaTime;
            if (timeCounter > lifeTime)
            {
                gameObject.SetActive(false);
                return;
            }

            CheckCollisions();
    	}

        private void OnDisable()
        {
            timeCounter = 0f;
            previousPos = null;
        }

        private void CheckCollisions()
        {
            if (!previousPos.HasValue)
            {
                previousPos = transform.position;
                return;
            }

            if (Physics.Linecast(previousPos.Value, transform.position, out hitObject))
            {
                HitTarget(hitObject.transform.root);
            }

            previousPos = transform.position;
        }

        private void HitTarget(Transform tr)
        {
            if (fireSource != null && tr.Equals(fireSource)) return;

            Ship s = tr.GetComponent<Ship>();
            if (s != null)
            {
                s.Damaged(fireSource, damage);
                fireSource.GetComponent<Ship>().AddDogFightPoints();
            }

            PoolProvider.Instance.RequestGameObject(PooledObject.Sparks).transform.position = transform.position;

            gameObject.SetActive(false);
        }
    }
}