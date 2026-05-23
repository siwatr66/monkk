// EnemyAI.cs
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform leftPoint;  
    [SerializeField] private Transform rightPoint; 
    private bool movingLeft = true;

    [Header("Attack Settings")]
    [SerializeField] private int damageToPlayer = 10;
    [SerializeField] private float attackCooldown = 1.5f; 
    private float lastAttackTime;

    private Rigidbody2D rb;
    private bool isFacingRight = false;

    void Start()
    {
        if (TryGetComponent<Rigidbody2D>(out var rigidbody2D))
        {
            rb = rigidbody2D;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // ล็อคการหมุนผ่านโค้ดเซฟตี้สำหรับ Unity 6
        }
    }

    void Update()
    {
        if (leftPoint == null || rightPoint == null || rb == null) return;

        if (movingLeft)
        {
            rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
            if (transform.position.x <= leftPoint.position.x)
            {
                movingLeft = false;
                Flip();
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
            if (transform.position.x >= rightPoint.position.x)
            {
                movingLeft = true;
                Flip();
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                if (collision.gameObject.TryGetComponent<PlayerHealth>(out var playerHealth))
                {
                    playerHealth.TakeDamage(damageToPlayer);
                    lastAttackTime = Time.time;
                }
            }
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
}