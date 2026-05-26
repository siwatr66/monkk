using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Stats")]
    public EnemyType enemyType;
    [SerializeField] private int maxHealth = 30;
    private int currentHealth;
    private bool isDead;

    [Header("Visual Drop")]
    public Sprite enemySprite;

    void Start()
    {
        currentHealth = maxHealth;
        isDead = false;
    }

    public void TakeDamage(int damage, EnemyType attackerType)
    {
        if (isDead) return;

        float damageMultiplier = GetElementMultiplier(attackerType, enemyType);
        int finalDamage = Mathf.RoundToInt(damage * damageMultiplier);

        currentHealth -= finalDamage;

        if (damageMultiplier >= 2f)
            Debug.Log($"[CRITICAL WEAKNESS] Damage x{damageMultiplier} -> {gameObject.name} takes {finalDamage}");
        else if (damageMultiplier > 1f)
            Debug.Log($"[MINOR WEAKNESS] Damage x{damageMultiplier} -> {gameObject.name} takes {finalDamage}");
        else
            Debug.Log($"[NORMAL] {gameObject.name} takes {finalDamage}");

        Debug.Log($"{gameObject.name} HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private float GetElementMultiplier(EnemyType attacker, EnemyType defender)
    {
        if (attacker == EnemyType.Normal || attacker == EnemyType.None) return 1.0f;

        if (defender == EnemyType.Fire)
        {
            if (attacker == EnemyType.Water) return 2.5f;
            if (attacker == EnemyType.Earth) return 1.5f;
            return 1.0f;
        }

        if (defender == EnemyType.Water)
        {
            if (attacker == EnemyType.Earth) return 2.5f;
            if (attacker == EnemyType.Wind) return 1.5f;
            return 1.0f;
        }

        if (defender == EnemyType.Earth)
        {
            if (attacker == EnemyType.Wind) return 2.5f;
            if (attacker == EnemyType.Fire) return 1.5f;
            return 1.0f;
        }

        if (defender == EnemyType.Wind)
        {
            if (attacker == EnemyType.Fire) return 2.5f;
            if (attacker == EnemyType.Water) return 1.5f;
            return 1.0f;
        }

        return 1.0f;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{gameObject.name} defeated.");

        try
        {
            if (ShapeshiftManager.Instance != null && enemySprite != null)
            {
                ShapeshiftManager.Instance.UnlockForm(enemyType, enemySprite);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[Shapeshift Error] {e.Message}");
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.HandleEnemyDefeated();
        }
        else if (EnemySpawnerManager.Instance != null)
        {
            EnemySpawnerManager.Instance.NextEnemy();
        }

        Destroy(gameObject);
    }
}
