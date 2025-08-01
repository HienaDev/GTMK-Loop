using System;
using UnityEngine;
using System.Collections;
using System.Diagnostics;

namespace PDollarGestureRecognizer
{
    public class EnemyManager : MonoBehaviour
    {
        public GameObject[] prefabsToSpawn;      // Assign multiple prefabs in Inspector
        public Transform spawnPosition;          // Where to center the spawn circle
        [SerializeField] private Demo demo;      // Reference to the Demo script

        [SerializeField] private float timeToSpawn = 5f; // Time interval to spawn enemies (not used in this version)

        [Header("Spawn Settings")]
        public float spawnRadius = 5f;           // Radius of the circle (shown in Scene view)

        [SerializeField] private Transform player;

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
                    // Pick a random prefab
                    int randomIndex = UnityEngine.Random.Range(0, prefabsToSpawn.Length);

                    // Get a random point on the outer edge of the circle (unit circle normalized)
                    Vector2 randomDir = UnityEngine.Random.insideUnitCircle.normalized;
                    Vector3 spawnOffset = new Vector3(randomDir.x, 0f, randomDir.y) * spawnRadius;
                    Vector3 randomizedSpawnPos = spawnPosition.position + spawnOffset;

                    // Instantiate at randomized position
                    GameObject temp = Instantiate(prefabsToSpawn[randomIndex], randomizedSpawnPos, Quaternion.identity);

                    // Handle enemy reference
                    Enemy tempEnemy = temp.GetComponent<Enemy>();
                    print(tempEnemy);
                    demo.AddEnemy(tempEnemy);
                    tempEnemy.target = player;

                    timeToSpawn -= 0.025f;

                    if(timeToSpawn < 1f)
                    {
                        timeToSpawn = 1f; // Prevent too fast spawning
                    }

                    // Flip sprite if spawned to the right of spawnPosition
                    if (randomizedSpawnPos.x < spawnPosition.position.x)
                    {
                        // Assuming the SpriteRenderer is on the first child
                        SpriteRenderer sr = temp.GetComponentInChildren<SpriteRenderer>();
                        if (sr != null)
                        {
                            sr.flipX = true;
                        }
                    }


                }
                else
                {
                    print("No prefabs assigned to spawn.");
                }

                yield return new WaitForSeconds(timeToSpawn);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (spawnPosition != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(spawnPosition.position, spawnRadius);
            }
        }
    }
}
