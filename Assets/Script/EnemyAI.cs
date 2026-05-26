// EnemyAI.cs
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 1.5f; // ความเร็วในการเดินตามพระ (ปรับได้)

    private Transform playerTransform; // พิกัดของพระ
    private Rigidbody2D rb;
    private bool isFacingRight = true; // ตัวแปรเช็คทิศทางปัจจุบันของศัตรู

    void Start()
    {
        if (TryGetComponent<Rigidbody2D>(out var rigidbody2D))
        {
            rb = rigidbody2D;
        }

        // ⭐ ค้นหาพิกัดของพระในด่านตั้งแต่เริ่มเกม
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    // ฟังก์ชันรับพิกัดหมุดเดิน (ไม่ได้ใช้แล้วในระบบตามล่านี้ แต่ขอเก็บไว้ไม่ให้ Spawner Error)
    public void SetupPatrolPoints(Transform left, Transform right) { }

    void FixedUpdate()
    {
        // ถ้าไม่เจอพระในฉาก หรือ Spawner ยังเสกศัตรูไม่เสร็จ ให้ยืนรอเฉยๆ
        if (playerTransform == null) return;

        // --- ระบบ AI ตามล่า (Follow Player Logic) ---
        FollowPlayerLogic();
    }

    private void FollowPlayerLogic()
    {
        // 1. คำนวณหาทิศทางที่ต้องเดินไปหาพระ (แกน X เท่านั้น)
        Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
        float moveDirection = directionToPlayer.x;

        // 2. สั่งเดินตามแกน X ที่คำนวณได้
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(moveDirection * moveSpeed, rb.linearVelocity.y);
        }

        // 3. ⭐ อัลกอริทึมหันหน้าตามพระอยู่ตลอดเวลา
        if (moveDirection > 0 && !isFacingRight)
        {
            // ถ้าต้องเดินขวา แต่หน้าหันซ้าย -> หันกลับไปขวา
            Flip();
        }
        else if (moveDirection < 0 && isFacingRight)
        {
            // ถ้าต้องเดินซ้าย แต่หน้าหันขวา -> หันกลับไปซ้าย
            Flip();
        }
    }

    // อัลกอริทึมกลับด้านสไปรท์มอนสเตอร์ 180 องศาแนวราบ
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1; // คูณลบหนึ่งที่แกน X เพื่อกลับด้านภาพสไปรท์
        transform.localScale = localScale;
    }
}