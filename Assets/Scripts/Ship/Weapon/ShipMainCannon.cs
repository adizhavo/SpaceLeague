using UnityEngine;
using SpaceLeague.Ship;
using SpaceLeague.Pooling;

namespace SpaceLeague.Ship.Weapon
{
    public class ShipMainCannon : MonoBehaviour
    {
        [SerializeField] protected Ship ship;
        [SerializeField] protected float fireRate;
        [SerializeField] [Range(0f, 10f)] protected float accuracy;
        [SerializeField] protected Transform weaponPivot;

        protected float timeCounter;

        private void Start()
        {
            timeCounter = fireRate;
        }

        public void OpenFire()
        {
            timeCounter += Time.deltaTime;
            if (timeCounter > fireRate) Fire();
        }

        protected virtual void Fire()
        {
            Transform bullet = PoolProvider.Instance.RequestGameObject(PooledObject.Bullet).transform;
            bullet.position = weaponPivot.position;
            Vector3 offset = Random.onUnitSphere * (10f - accuracy);
            bullet.LookAt(ship.GlobalDirection + offset, weaponPivot.up);
            bullet.GetComponent<Bullet>().fireSource = ship.transform;
            timeCounter = 0f;
        }
    }
}