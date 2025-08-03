using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class Enemy : MonoBehaviour
{
    public EnemyType enemyType;
    public Transform target;     // The object to move toward
    public float speed = 3f;     // Movement speed (units per second)

    [SerializeField] private GameObject sprite;
    [SerializeField] private GameObject particles;
    [SerializeField] private GameObject deathParticles;
    [SerializeField] private Collider col;

    [SerializeField] private AudioClip[] blowUpSounds; // Pool of explosion sounds
    [SerializeField] private AudioMixerGroup audioMixerGroup;
    
    private AudioSource audioSource;
    private System.Random rnd;

    public ParticleSystem[] hitParticles; // Array of hit particle systems

    [SerializeField] private float timeToDisappear = 5f; // Time before the enemy disappears

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        
        if (audioMixerGroup != null)
            audioSource.outputAudioMixerGroup = audioMixerGroup;

        rnd = new System.Random();
    }
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

        if (death)
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
        // Play a random blow-up sound
        PlayRandomBlowUpSound();

        StartCoroutine(DisappearAfterTime());
    }

    private IEnumerator DisappearAfterTime()
    {
        yield return new WaitForSeconds(timeToDisappear);
        Destroy(gameObject);
    }
    private void PlayRandomBlowUpSound()
    {
        if (blowUpSounds == null || blowUpSounds.Length == 0 || audioSource == null)
            return;

        int index = rnd.Next(0, blowUpSounds.Length);
        audioSource.clip = blowUpSounds[index];
        audioSource.Play();
    }
}
