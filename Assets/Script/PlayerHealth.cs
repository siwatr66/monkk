// PlayerHealth.cs (เวอร์ชันสมบูรณ์ 100% - รองรับระบบส่งสัญญาณฟื้นชีพด่านเดิม)
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
        Debug.Log($"盒 [PLAYER HP] พระโดนโจมตี! เลือดเหลือ: {currentHealth} / {maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("💀 [PLAYER STATUS] พระสิ้นชีพ! ส่งสายสัญญาณให้คำสั่ง GameManager จัดการชุบชีวิต...");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDied();
        }
    }

    public void HealFull()
    {
        currentHealth = maxHealth;
        isDead = false;
        Debug.Log($"💖 [PLAYER HP] เติมเลือดเต็มหลอดจากการฆ่ามอนสเตอร์! HP: {currentHealth}/{maxHealth}");
    }

    public void RespawnSetup()
    {
        ResetHealth();
    }

    private void ResetHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
        Debug.Log($"🛡️ [PLAYER HP] ฟื้นชีพเสร็จสิ้น! รีเซ็ตเลือดพระกลับมาเต็มหลอด: {currentHealth}/{maxHealth}");
    }
}