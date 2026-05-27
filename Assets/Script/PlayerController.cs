using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    private Rigidbody2D rb;
    private float moveInputX = 0f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private LayerMask whatIsGround;
    private bool isGrounded;

    [Header("Combat Settings")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private int attackDamage = 10;

    [Header("Shapeshifter Visuals")]
    [SerializeField] private SpriteRenderer characterSpriteRenderer;
    [SerializeField] private Sprite normalFormSprite;
    [SerializeField] private Sprite waterFormSprite;
    [SerializeField] private Sprite fireFormSprite;
    [SerializeField] private Sprite windFormSprite;
    [SerializeField] private Sprite earthFormSprite;

    private Animator anim;
    private bool isFacingRight = true;

    // 🟢 [แก้ไขจุดที่ 1]: สลับชื่อแฮชส่งค่าให้ตรงกับพารามิเตอร์ "Run" และเพิ่ม "isGrounded" ในอนิเมเตอร์
    private static readonly int RunHash = Animator.StringToHash("Run");
    private static readonly int GroundedHash = Animator.StringToHash("isGrounded");
    private static readonly int AttackHash = Animator.StringToHash("Attack");

    private InputSystem_Actions inputActions;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Player.Move.performed += ctx => moveInputX = ctx.ReadValue<Vector2>().x;
        inputActions.Player.Move.canceled += ctx => moveInputX = 0f;
        inputActions.Player.Jump.performed += ctx => JumpLogic();
        inputActions.Player.Attack.performed += ctx => AttackLogic();
    }

    void OnEnable() => inputActions.Player.Enable();
    void OnDisable() => inputActions.Player.Disable();

    void Start()
    {
        if (TryGetComponent<Rigidbody2D>(out var rigidbody2D)) rb = rigidbody2D;
        if (TryGetComponent<Animator>(out var animator)) anim = animator;
        if (characterSpriteRenderer == null) characterSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(moveInputX * moveSpeed, rb.linearVelocity.y);
        }

        if (moveInputX > 0 && !isFacingRight) Flip();
        else if (moveInputX < 0 && isFacingRight) Flip();

        // 🟢 [แก้ไขระบบเช็ค Layer พื้น]: ตรวจหาเลเยอร์หญ้า/ด่านสุสานผ่านพิกัดกล่องเซนเซอร์ปลายเท้า
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        }

        // 🟢 [แก้ไขจุดที่ 2]: บังคับส่งสัญญาณชีพไปบงการหน้าต่าง Animator ให้เปลี่ยนท่า
        if (anim != null && anim.enabled)
        {
            anim.SetFloat(RunHash, Mathf.Abs(moveInputX)); // ส่งค่าความเร็วเดินไปที่สวิตช์ Run
            anim.SetBool(GroundedHash, isGrounded);       // ส่งค่าแท้/เท็จของการแตะพื้นเลเยอร์ไปที่ isGrounded
        }

        // คอยเช็คอัปเดตรูปร่างให้ตรงเสมอตลอดเวลา
        ForceApplyVisualChange();
    }

    // ⭐ [ฟังก์ชันเปลี่ยนรูปภาพพระแปลงร่าง]: เปิดเป็น public เพื่อให้ระบบ UI เรียกใช้งานได้
    public void ForceApplyVisualChange()
    {
        if (ShapeshiftManager.Instance == null || characterSpriteRenderer == null) return;

        int currentForm = ShapeshiftManager.Instance.CurrentForm;
        Sprite selectedSprite = null;

        switch (currentForm)
        {
            case 0: selectedSprite = normalFormSprite; break;
            case 1: selectedSprite = waterFormSprite; break;
            case 2: selectedSprite = fireFormSprite; break;
            case 3: selectedSprite = windFormSprite; break;
            case 4: selectedSprite = earthFormSprite; break;
        }

        if (selectedSprite != null && characterSpriteRenderer.sprite != selectedSprite)
        {
            if (anim != null) anim.enabled = (currentForm == 0);
            characterSpriteRenderer.sprite = selectedSprite;
            Debug.Log($"🎭 [เปลี่ยนร่างสำเร็จ] ตัวพระสลับภาพกราฟิกไปใช้ร่างหมายเลข {currentForm} บนหน้าจอแล้ว!");
        }
    }

    private void JumpLogic()
    {
        if (isGrounded && rb != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void AttackLogic()
    {
        if (anim != null && anim.enabled) anim.SetTrigger(AttackHash);
        if (attackPoint == null) return;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.TryGetComponent<EnemyHealth>(out var enemyHealth))
            {
                int currentFormIndex = ShapeshiftManager.Instance != null ? ShapeshiftManager.Instance.CurrentForm : 0;
                enemyHealth.TakeDamage(attackDamage, (EnemyType)currentFormIndex);
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