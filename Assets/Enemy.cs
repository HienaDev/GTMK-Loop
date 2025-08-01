using System;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyType enemyType;
    public Transform target;     // The object to move toward
    public float speed = 3f;     // Movement speed (units per second)

    [SerializeField] private GameObject sprite;
    [SerializeField] private GameObject particles;
    [SerializeField] private GameObject deathParticles;
    [SerializeField] private Collider col;

     public ParticleSystem[] hitParticles; // Array of hit particle systems

    [SerializeField] private float timeToDisappear = 5f; // Time before the enemy disappears
    void Update()
    {
        if (target != null)
        {
            // Move toward the target at constant speed
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                speed * Time.deltaTime
            );
        }
    }

    public void BlowUp(bool death, Color color)
    {
        sprite.SetActive(false);
        col.enabled = false;

        target = null;

        if(death)
        {
            deathParticles.SetActive(true);
        }
        else
        {
            particles.SetActive(true);
        }

        foreach (var hitParticle in hitParticles)
        {
            if (hitParticle != null)
            {
                hitParticle.startColor = color;
            }
        }

        StartCoroutine(DisappearAfterTime());
    }

    private IEnumerator DisappearAfterTime()
    {
        yield return new WaitForSeconds(timeToDisappear);
        Destroy(gameObject);
    }
}
