// EnemyHealth.cs (เวอร์ชันเสร็จสมบูรณ์ 100% - รวมระบบ Matrix ธาตุไขว้ + ระบบวิ่งชนหักเลือดพระอัตโนมัติ)
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    // ⭐ ใส่เลขธาตุใน Inspector สำหรับปุ่มงอก: 1 = น้ำ, 2 = ไฟ, 3 = ลม, 4 = ดิน
    [Header("Mobile Shapeshift ID")]
    [SerializeField] private int enemyElementNumber = 1;

    [Header("Visual References")]
    [SerializeField] private EnemyType enemyType; // รหัส Enum ชนิดธาตุ (Normal, Water, Fire, Wind, Earth)
    [SerializeField] private Sprite enemySprite;

    [Header("Counter Attack Settings")]
    [SerializeField] private int counterDamage = 15; // ดาเมจที่จะหักเลือดพระเมื่อวิ่งชนกัน

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

        // 🚨 [CONSOLE HP CHECK]: พิมพ์โชว์ดาเมจและ HP มอนสเตอร์ขึ้น Console ชัดๆ ทุกครั้งที่โดนชก
        Debug.Log($"⚔️ [COMBAT MATRIX] พระร่าง {attackerElement} 🥊 ชก มอนสเตอร์ {enemyType} -> พลังทวีคูณ: {multiplier}x | ดาเมจสุทธิ: {finalDamage} | [ENEMY HP] เหลือ: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 💥 [ระบบแก้บั๊กพระไม่ตายวินาทีวิ่งชนกันกลางฉาก]
    // ฟังก์ชันฟิสิกส์ 2D ของ Unity จะทำงานออโต้ทันทีเมื่อตัวมันวิ่งไปเตะโดนกล่องฟิสิกส์ของพระ
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        // เช็คตัวตรวจจับว่าวัตถุที่วิ่งมาชน มี Tag ตรงกับคำว่า Player (ตัวพระ) หรือไม่
        if (collision.CompareTag("Player"))
        {
            if (collision.TryGetComponent<PlayerHealth>(out var playerHealth))
            {
                Debug.Log($"💥 [COLLISION TOUCH] {gameObject.name} วิ่งชนเข้าตัวพระ! สั่งหัก HP พระสวนกลับทันที {counterDamage} หน่วย");
                playerHealth.TakeDamage(counterDamage); // ยิงดาเมจลดเลือดพระทันที
            }
        }
    }

    // 🧬 [สมองกลคำนวณตารางธาตุไขว้ Matrix ชนะทางหลัก/รอง/แพ้ทาง]
    private float GetMatrixDamageMultiplier(EnemyType attacker, EnemyType defender)
    {
        if (attacker == EnemyType.Normal || defender == EnemyType.Normal) return 1.0f;
        if (attacker == defender) return 0.8f;

        switch (attacker)
        {
            case EnemyType.Water: // 💧 ร่างน้ำ
                if (defender == EnemyType.Fire) return 2.0f; // ชนะทางหลัก (น้ำดับไฟ)
                if (defender == EnemyType.Wind) return 1.2f; // ชนะทางรอง
                if (defender == EnemyType.Earth) return 0.5f; // แพ้ทางหลัก
                break;
            case EnemyType.Fire:  // 🔥 ร่างไฟ
                if (defender == EnemyType.Wind) return 2.0f; // ชนะทางหลัก (ไฟโหมลม)
                if (defender == EnemyType.Earth) return 1.2f; // ชนะทางรอง
                if (defender == EnemyType.Water) return 0.5f; // แพ้ทางหลัก
                break;
            case EnemyType.Wind:  // 🌪️ ร่างลม
                if (defender == EnemyType.Earth) return 2.0f; // ชนะทางหลัก (ลมพัดดิน)
                if (defender == EnemyType.Water) return 1.2f; // ชนะทางรอง
                if (defender == EnemyType.Fire) return 0.5f; // แพ้ทางหลัก
                break;
            case EnemyType.Earth: // 🪨 ร่างดิน
                if (defender == EnemyType.Water) return 2.0f; // ชนะทางหลัก (ดินทับน้ำ)
                if (defender == EnemyType.Fire) return 1.2f; // ชนะทางรอง
                if (defender == EnemyType.Wind) return 0.5f; // แพ้ทางหลัก
                break;
        }
        return 1.0f;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"💀 [ENEMY STATUS] {gameObject.name} HP เหลือ 0 ดับคาหมัดพระเรียบร้อย!");

        try
        {
            if (ShapeshiftManager.Instance != null)
            {
                // ส่งตัวเลข ID ข้ามฝั่งไปสั่งให้ปุ่มพิกเซลอาร์ตงอกบนจอ Canvas
                ShapeshiftManager.Instance.UnlockForm(enemyElementNumber);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[Shapeshift Error] {e.Message}");
        }

        // ระบบคุมด่านดึงมอนสเตอร์ตัวถัดไปสปอนลงสนาม
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