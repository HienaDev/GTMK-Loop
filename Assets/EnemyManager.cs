using System.Diagnostics;
using System;
using UnityEngine;
using System.Collections;

namespace PDollarGestureRecognizer
{
    public class EnemyManager : MonoBehaviour
    {
        public GameObject[] prefabsToSpawn;  // Assign multiple prefabs in Inspector
        public Transform spawnPosition;      // Optional spawn position

        [SerializeField] private Demo demo; // Reference to the Demo script

        private void Start()
        {
            StartCoroutine(SpawnRandomPrefabEveryFiveSeconds());
        }

        private IEnumerator SpawnRandomPrefabEveryFiveSeconds()
        {
            while (true)
            {
                if (prefabsToSpawn.Length > 0)
                {
                    int randomIndex = UnityEngine.Random.Range(0, prefabsToSpawn.Length);
                    GameObject temp = Instantiate(prefabsToSpawn[randomIndex], spawnPosition.position, Quaternion.identity);
                    Enemy tempEnemy = temp.GetComponent<Enemy>();
                    demo.AddEnemy(tempEnemy);
                }
                else
                {
                    print("No prefabs assigned to spawn.");
                }

                yield return new WaitForSeconds(5f);
            }
        }
    }
}
