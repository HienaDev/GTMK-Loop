using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    private int currentHealth;

    [SerializeField] private ParticleSystem ghosts;

    [SerializeField] private Sprite[] healthIcons;
    [SerializeField]  private Image healthUI;

    [SerializeField] private GameObject deathScreen;

    private Animator playerAnimator;


    private void Start()
    {
        playerAnimator = GetComponentInChildren<Animator>();
        currentHealth = maxHealth;
        UpdateHealthIcon();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            TakeDamage(1);
            other.GetComponent<Enemy>().BlowUp(false, Color.white);
        }
    }

    private void UpdateHealthIcon()
    {
        int healthIndex = maxHealth - currentHealth;
        if (healthIndex < 0 || healthIndex >= healthIcons.Length)
        {
            return;
        }
        healthUI.sprite = healthIcons[ maxHealth - currentHealth];
    }


    private void TakeDamage(int amount)
    {
        playerAnimator.SetTrigger("Damage");
        currentHealth -= amount;
        print("Player took damage! Current health: " + currentHealth);
        ghosts.Play();
        CameraShaker.Instance.Shake(0.7f, 1f);
        UpdateHealthIcon();
        if (currentHealth <= 0)
        {

            Die();
        }
    }

    private void Die()
    {
        print("Player died!");
        deathScreen.SetActive(true);
        // Add death behavior here (disable movement, trigger animation, etc.)
        gameObject.SetActive(false);
    }
}
