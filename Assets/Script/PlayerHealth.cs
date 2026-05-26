// PlayerHealth.cs
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    private SpriteRenderer spriteRenderer;
    private MonoBehaviour playerController; // ใช้ MonoBehaviour ทั่วไปเพื่อความยืดหยุ่นในการหา Component

    void Start()
    {
        currentHealth = maxHealth;

        // แคชคอมโพเนนต์ไว้ล่วงหน้าเพื่อความเร็วในการประมวลผลตามมาตรฐาน Unity 6
        TryGetComponent<SpriteRenderer>(out spriteRenderer);

        // พยายามดึงสคริปต์ควบคุมของพระ (ปรับชื่อคลาสให้ตรงกับที่คุณใช้จริง เช่น PlayerController)
        if (TryGetComponent(System.Type.GetType("PlayerController"), out var controller))
        {
            playerController = (MonoBehaviour)controller;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Player HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // ฟังก์ชันฟื้นฟูพลังชีวิตให้กลับมาเต็มร้อย
    public void HealFull()
    {
        currentHealth = maxHealth;
        Debug.Log($"พระฟื้นฟูพลังชีวิตเต็ม: {currentHealth}/{maxHealth}");
    }

    // อัลกอริทึมการตายแบบซ่อนตัว (Disable Mechanics) เพื่อป้องกันปัญหา Null Reference
    void Die()
    {
        Debug.Log("Player HP reaches 0! Hiding Player...");

        // 1. ซ่อนภาพสไปรท์ของพระไม่ให้มองเห็นในฉาก
        if (spriteRenderer != null) spriteRenderer.enabled = false;

        // 2. ปิดสคริปต์ควบคุมเพื่อไม่ให้ผู้เล่นกดเดินหรือกดโจมตีได้ตอนตาย
        if (playerController != null) playerController.enabled = false;

        // 3. ส่งสัญญาณบอก GameManager ให้รีเซ็ตมอนสเตอร์และย้ายพิกัดเกิดใหม่
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDied();
        }
    }

    // ฟังก์ชันเปิดการมองเห็นและตัวควบคุมกลับมาทำงาน (เรียกใช้ตอนเกิดใหม่)
    public void RespawnSetup()
    {
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (playerController != null) playerController.enabled = true;
        HealFull();
    }
}