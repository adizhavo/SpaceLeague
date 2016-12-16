using UnityEngine;

namespace SpeaceLeague
{
    public class Spawner : MonoBehaviour {

    	[SerializeField] private GameObject PlayerShipPrefab;
        [SerializeField] private GameObject EnemyShipPrefab;
        [SerializeField] private int AmountOfEnemies;
        [SerializeField] private int SpawnRadius;

        private void Awake()
        {
            GameObject player = Instantiate(PlayerShipPrefab) as GameObject;
            player.transform.position = Vector3.zero;

            for(int i = 0; i < AmountOfEnemies; i ++)
            {
                GameObject enemy = Instantiate(EnemyShipPrefab) as GameObject;
                enemy.transform.position = Random.insideUnitSphere * SpawnRadius;
            }
        }
    }
}