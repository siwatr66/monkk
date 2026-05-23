// EnemyHealth.cs
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Stats")]
    public EnemyType enemyType; 
    [SerializeField] private int maxHealth = 30;
    private int currentHealth;

    [Header("Visual Drop")]
    public Sprite enemySprite; 

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage, EnemyType attackerType)
    {
        int finalDamage = damage;

        if (CheckWeakness(attackerType, enemyType))
        {
            finalDamage *= 2; 
            Debug.Log("Critical Element! Damage doubled!");
        }

        currentHealth -= finalDamage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private bool CheckWeakness(EnemyType attacker, EnemyType defender)
    {
        if (attacker == EnemyType.Fire && defender == EnemyType.Wind) return true;
        if (attacker == EnemyType.Wind && defender == EnemyType.Earth) return true;
        if (attacker == EnemyType.Earth && defender == EnemyType.Water) return true;
        if (attacker == EnemyType.Water && defender == EnemyType.Fire) return true;
        return false;
    }

    void Die()
    {
        if (ShapeshiftManager.Instance != null)
        {
            ShapeshiftManager.Instance.UnlockForm(enemyType, enemySprite);
        }
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LevelCleared();
        }
        
        Destroy(gameObject);
    }
}