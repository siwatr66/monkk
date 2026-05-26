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
        // คำนวณตัวคูณจากระบบ Matrix ไขว้ธาตุ
        float damageMultiplier = GetElementMultiplier(attackerType, enemyType);

        // คำนวณดาเมจสุทธิสุทธิ
        int finalDamage = Mathf.RoundToInt(damage * damageMultiplier);

        currentHealth -= finalDamage;

        // ยิง Log แจ้งระดับความแรงบน Console ให้เห็นชัดๆ
        if (damageMultiplier >= 2f)
            Debug.Log($"💥 [CRITICAL WEAKNESS] ชนะทางรุนแรงที่สุด! ดาเมจคูณ {damageMultiplier}x -> {gameObject.name} โดนไป {finalDamage} หน่วย!");
        else if (damageMultiplier > 1f)
            Debug.Log($"✨ [MINOR WEAKNESS] ชนะทางรองลงมา! ดาเมจคูณ {damageMultiplier}x -> {gameObject.name} โดนไป {finalDamage} หน่วย!");
        else
            Debug.Log($"⚔️ [NORMAL] โจมตีธาตุทั่วไป ดาเมจ {finalDamage} หน่วย");

        Debug.Log($"{gameObject.name} เลือดปัจจุบันเหลือ: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // ⭐ อัลกอริทึม Matrix ไขว้ธาตุขั้นสูง (Cross-Element Calculation)
    private float GetElementMultiplier(EnemyType attacker, EnemyType defender)
    {
        // ถ้าร่างมนุษย์ปกติ (Normal) ไม่คิดตัวคูณ
        if (attacker == EnemyType.Normal || attacker == EnemyType.None) return 1.0f;

        // --- MATRIX ระบบธาตุไฟ (FIRE) ---
        if (defender == EnemyType.Fire)
        {
            if (attacker == EnemyType.Water) return 2.5f; // แพ้น้ำที่สุด
            if (attacker == EnemyType.Earth) return 1.5f; // แพ้ดินรองลงมา
            return 1.0f;
        }

        // --- MATRIX ระบบธาตุน้ำ (WATER) ---
        if (defender == EnemyType.Water)
        {
            if (attacker == EnemyType.Earth) return 2.5f; // แพ้ดินที่สุด
            if (attacker == EnemyType.Wind) return 1.5f;  // แพ้ลมรองลงมา
            return 1.0f;
        }

        // --- MATRIX ระบบธาตุดิน (EARTH) ---
        if (defender == EnemyType.Earth)
        {
            if (attacker == EnemyType.Wind) return 2.5f;  // แพ้ลมที่สุด
            if (attacker == EnemyType.Fire) return 1.5f;  // แพ้ไฟรองลงมา
            return 1.0f;
        }

        // --- MATRIX ระบบธาตุลม (WIND) ---
        if (defender == EnemyType.Wind)
        {
            if (attacker == EnemyType.Fire) return 2.5f;  // แพ้ไฟที่สุด
            if (attacker == EnemyType.Water) return 1.5f; // แพ้น้ำรองลงมา
            return 1.0f;
        }

        return 1.0f;
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} เลือดเหลือ 0! เริ่มกระบวนการตาย...");

        try
        {
            if (ShapeshiftManager.Instance != null && enemySprite != null)
            {
                ShapeshiftManager.Instance.UnlockForm(enemyType, enemySprite);
                Debug.Log($"[Shapeshift] ปลดล็อกร่าง {enemyType} เข้าคลังสำเร็จ!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[Shapeshift Error] ติดขัด: {e.Message} แต่จะข้ามไปเสกมอนสเตอร์ตัวใหม่ให้เกมไม่ค้าง");
        }

        if (EnemySpawnerManager.Instance != null)
        {
            Debug.Log("[Spawner Link] ส่งสัญญาณเรียกศัตรูตัวถัดไปลงสนาม...");
            EnemySpawnerManager.Instance.NextEnemy();
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && player.TryGetComponent<PlayerHealth>(out var playerHealth))
        {
            playerHealth.HealFull();
        }

        Destroy(gameObject);
    }
}