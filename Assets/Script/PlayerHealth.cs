using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    private bool isDead;

    private SpriteRenderer spriteRenderer;
    private PlayerController playerController;

    void Awake()
    {
        currentHealth = maxHealth;
        isDead = false;

        TryGetComponent(out spriteRenderer);
        TryGetComponent(out playerController);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"Player HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void HealFull()
    {
        currentHealth = maxHealth;
        isDead = false;
        Debug.Log($"Player healed: {currentHealth}/{maxHealth}");
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Player HP reaches 0.");

        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (playerController != null) playerController.enabled = false;

        if (TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDied();
        }
    }

    public void RespawnSetup()
    {
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (playerController != null) playerController.enabled = true;

        if (TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = Vector2.zero;
        }

        HealFull();
    }
}
