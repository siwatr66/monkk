// PlayerHealth.cs (เวอร์ชันเสร็จสมบูรณ์ 100% - ส่งข้อมูลเชื่อมต่อกับ GameManager คลีนๆ)
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    private bool isDead = false;

    void Start()
    {
        ResetHealth();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"🛡 ️[PLAYER HP] พระโดนตอดเลือด! HP เหลือ: {currentHealth} / {maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("💀 [PLAYER STATUS] พระสิ้นชีพ! ดีดส่งสัญญาณไปหา GameManager หลังบ้าน...");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDied();
        }
    }

    public void HealFull()
    {
        currentHealth = maxHealth;
        isDead = false;
        Debug.Log($"💖 [PLAYER HP] รีเลือดพระกลับมาเต็มหลอด! HP: {currentHealth}/{maxHealth}");
    }

    public void RespawnSetup()
    {
        ResetHealth();
    }

    private void ResetHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
    }
}