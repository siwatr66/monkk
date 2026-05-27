// EnemyHealth.cs (เวอร์ชันเสร็จสมบูรณ์ 100% - ระบบ Matrix ธาตุไขว้ + ระบบแผ่ออร่าตอดเลือดพระ มอนสเตอร์ไม่ตายแทน)
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    // ใส่เลขธาตุใน Inspector สำหรับปุ่มงอก: 1 = น้ำ, 2 = ไฟ, 3 = ลม, 4 = ดิน
    [Header("Mobile Shapeshift ID")]
    [SerializeField] private int enemyElementNumber = 1;

    [Header("Visual References")]
    [SerializeField] private EnemyType enemyType;
    [SerializeField] private Sprite enemySprite;

    [Header("Aura Attack Settings")]
    [SerializeField] private int counterDamage = 15;      // ดาเมจที่มอนสเตอร์จะตอดเลือดพระ
    [SerializeField] private float attackCooldown = 1.0f;  // ตอดเลือดพระทุกๆ 1 วินาทีเมื่อยืนแช่ใกล้กัน
    private float nextAttackTime = 0f;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        Debug.Log($"👾 [ENEMY SPAWN] {gameObject.name} (ธาตุ: {enemyType}) ลงสนามแล้ว! HP เริ่มต้น: {currentHealth}/{maxHealth}");
    }

    public void TakeDamage(int damage, EnemyType attackerElement)
    {
        if (isDead) return;

        // คำนวณตัวคูณดาเมจตามระบบ Matrix ชนะทางหลัก-รอง-แพ้ทาง
        float multiplier = GetMatrixDamageMultiplier(attackerElement, enemyType);
        int finalDamage = Mathf.RoundToInt(damage * multiplier);

        currentHealth -= finalDamage;

        Debug.Log($"⚔️ [COMBAT MATRIX] พระร่าง {attackerElement} 🥊 ชก มอนสเตอร์ {enemyType} -> พลังทวีคูณ: {multiplier}x | ดาเมจสุทธิ: {finalDamage} | [ENEMY HP] เหลือ: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            isDead = true;
            Die();
        }
    }

    // 🟢 [ซ่อมลอจิกเด็ดขาด]: มอนสเตอร์ทำดาเมจใส่พระฝั่งเดียว ห้ามมีคำสั่ง Die() ของมอนสเตอร์ในบล็อกนี้!
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isDead) return;

        // เช็คชัวร์ว่าวัตถุที่วิ่งมาชนหรือแช่อยู่รอบตัวมี Tag ตรงกับคำว่า Player (ตัวพระ)
        if (collision.CompareTag("Player"))
        {
            if (Time.time >= nextAttackTime)
            {
                if (collision.TryGetComponent<PlayerHealth>(out var playerHealth))
                {
                    Debug.Log($"💥 [AURA TOUCH] {gameObject.name} ปล่อยออร่าตอดพระ! หัก HP พระ {counterDamage} หน่วย");

                    // สั่งหักเลือดพระเท่านั้น มอนสเตอร์ห้ามลดเลือดตัวเอง
                    playerHealth.TakeDamage(counterDamage);

                    nextAttackTime = Time.time + attackCooldown;
                }
            }
        }
    }

    // สมองกลคำนวณตารางธาตุไขว้ Matrix ชนะทางหลัก/รอง/แพ้ทาง
    private float GetMatrixDamageMultiplier(EnemyType attacker, EnemyType defender)
    {
        if (attacker == EnemyType.Normal || defender == EnemyType.Normal) return 1.0f;
        if (attacker == defender) return 0.8f;

        switch (attacker)
        {
            case EnemyType.Water:
                if (defender == EnemyType.Fire) return 2.0f;
                if (defender == EnemyType.Wind) return 1.2f;
                if (defender == EnemyType.Earth) return 0.5f;
                break;
            case EnemyType.Fire:
                if (defender == EnemyType.Wind) return 2.0f;
                if (defender == EnemyType.Earth) return 1.2f;
                if (defender == EnemyType.Water) return 0.5f;
                break;
            case EnemyType.Wind:
                if (defender == EnemyType.Earth) return 2.0f;
                if (defender == EnemyType.Water) return 1.2f;
                if (defender == EnemyType.Fire) return 0.5f;
                break;
            case EnemyType.Earth:
                if (defender == EnemyType.Water) return 2.0f;
                if (defender == EnemyType.Fire) return 1.2f;
                if (defender == EnemyType.Wind) return 0.5f;
                break;
        }
        return 1.0f;
    }

    void Die()
    {
        Debug.Log($"💀 [ENEMY STATUS] {gameObject.name} HP เหลือ 0 ดับคาหมัดพระเรียบร้อย!");

        try
        {
            if (ShapeshiftManager.Instance != null)
            {
                ShapeshiftManager.Instance.UnlockForm(enemyElementNumber);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[Shapeshift Error] {e.Message}");
        }

        // ส่งสัญญาณบอก GameManager ว่ามอนสเตอร์ตายจริง ๆ ค่อยเสกตัวใหม่ลงด่าน
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