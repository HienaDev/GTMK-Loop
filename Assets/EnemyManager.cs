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
            float spawnRadius = 3f; // Radius of the spawn circle on XZ plane

            while (true)
            {
                if (prefabsToSpawn.Length > 0)
                {
                    // Pick a random prefab
                    int randomIndex = UnityEngine.Random.Range(0, prefabsToSpawn.Length);

                    // Generate a random point in a circle on the XZ plane
                    Vector2 randomXZ = UnityEngine.Random.insideUnitCircle * spawnRadius;
                    Vector3 spawnOffset = new Vector3(randomXZ.x, 0f, randomXZ.y);
                    Vector3 randomizedSpawnPos = spawnPosition.position + spawnOffset;

                    // Instantiate at randomized position
                    GameObject temp = Instantiate(prefabsToSpawn[randomIndex], randomizedSpawnPos, Quaternion.identity);

                    // Handle enemy reference
                    Enemy tempEnemy = temp.GetComponent<Enemy>();
                    print(tempEnemy);
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
