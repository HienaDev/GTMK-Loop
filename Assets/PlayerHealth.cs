using System.Diagnostics;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    private int currentHealth;

    [SerializeField] private ParticleSystem ghosts;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            TakeDamage(1);
            other.GetComponent<Enemy>().BlowUp(false, Color.white);
        }
    }




    private void TakeDamage(int amount)
    {
        currentHealth -= amount;
        print("Player took damage! Current health: " + currentHealth);
        ghosts.Play();
        CameraShaker.Instance.Shake(0.7f, 1f);
        if (currentHealth <= 0)
        {

            Die();
        }
    }

    private void Die()
    {
        print("Player died!");
        // Add death behavior here (disable movement, trigger animation, etc.)
        gameObject.SetActive(false);
    }
}
