using UnityEngine;
using System.Collections.Generic;

namespace SpaceLeague.Pooling
{
	// will contain oll pooled objects
    [CreateAssetMenu(fileName = "GameObjectPool", menuName = "Space League/GameObjectPool")]
	public class PoolProvider : ScriptableObject
	{
		private static PoolProvider instance;
		public static PoolProvider Instance
		{
			get
			{
				if (instance == null)
				{
                    instance = Resources.Load<PoolProvider>("GameObjectPool");
					instance.Init();
				}

				return instance;
			}
		}

		[SerializeField]
		private Pool[] configuredPools;

		private void Init()
		{
			foreach (Pool p in configuredPools)
				p.Init();
		}

		public GameObject RequestGameObject(PooledObject requestedObject)
		{
			foreach (Pool p in configuredPools)
				if (p.PooledObject.Equals(requestedObject))
					return p.GetPooledObject();

			// it will never enter here, the pool will create new gameObjects if needed
			return null;
		}
	}

	// Pool of gameobjects
	[System.Serializable]
	public class Pool
	{
		// Just to keep the editor organised with names
		public string PoolName;

		[SerializeField]
		private PooledObject Object;
		public PooledObject PooledObject
		{
			get { return Object; }
		}

		[SerializeField]
		private GameObject prefab;
		[SerializeField]
		private int size;

		private Transform poolParent;
		private List<GameObject> pool;

		public void Init()
		{
			if (isInitialized()) return;

			if (poolParent == null) poolParent = new GameObject(prefab.name + "_pool").transform;

			pool = new List<GameObject>();

			for (int i = 0; i < size; i++)
				AddClone();
		}

		public bool isInitialized()
		{
			return poolParent != null && pool.Count > 0;
		}

		public GameObject GetPooledObject()
		{
            for(int i =0; i < pool.Count; i++)
			{
                if (pool[i] == null) pool[i] = CreateClone();
                if (!pool[i].activeSelf)
				{
                    pool[i].SetActive(true);
                    return pool[i];
				}
			}

			GameObject clone = AddClone();
			clone.SetActive(true);
			return clone;
		}

		private GameObject AddClone()
		{
            GameObject clone = CreateClone();
			pool.Add(clone);
			return clone;
		}

        private GameObject CreateClone()
        {
            GameObject clone = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
            clone.transform.SetParent(poolParent);
            clone.SetActive(false);
            return clone;
        }
	} 
}