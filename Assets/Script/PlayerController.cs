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

    [Header("Shapeshifter Color Visuals (🟢 เปลี่ยนมาตั้งค่าสีแทนรูปสไปรท์)")]
    [SerializeField] private SpriteRenderer characterSpriteRenderer;
    [SerializeField] private Color normalColor = Color.white;  // สีเดิมของนักเวท
    [SerializeField] private Color waterColor = Color.blue;    // ร่างน้ำ (เช่น สีน้ำเงิน/ฟ้า)
    [SerializeField] private Color fireColor = Color.red;      // ร่างไฟ (เช่น สีแดง)
    [SerializeField] private Color windColor = Color.cyan;     // ร่างลม (เช่น สีฟ้าสว่าง/เขียวมินต์)
    [SerializeField] private Color earthColor = new Color(0.5f, 0.35f, 0.1f); // ร่างดิน (สีน้ำตาล)

    private Animator anim;
    private bool isFacingRight = true;

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

        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        }

        if (anim != null && anim.enabled)
        {
            anim.SetFloat(RunHash, Mathf.Abs(moveInputX));
            anim.SetBool(GroundedHash, isGrounded);
        }

        // คอยเช็คอัปเดตสีสันให้ตรงเสมอตลอดเวลา
        ForceApplyVisualChange();
    }

    // ⭐ [ฟังก์ชันย้อมสีพระแปลงร่าง]: เปิดเป็น public ให้ระบบ UI สลับสีตามจานสีที่เราเลือกได้ทันที
    public void ForceApplyVisualChange()
    {
        if (ShapeshiftManager.Instance == null || characterSpriteRenderer == null) return;

        int currentForm = ShapeshiftManager.Instance.CurrentForm;
        Color selectedColor = normalColor;

        // 🟢 [ลอจิกสลับสี]: เปลี่ยนจากเช็ครูปมาเป็นจิ้มเลือกโทนสีตามหมายเลขร่างแปลง
        switch (currentForm)
        {
            case 0: selectedColor = normalColor; break;
            case 1: selectedColor = waterColor; break;
            case 2: selectedColor = fireColor; break;
            case 3: selectedColor = windColor; break;
            case 4: selectedColor = earthColor; break;
        }

        // ถ้าย้อมสีปัจจุบันไม่ตรงกับสีร่างนั้น ให้สั่งเปลี่ยนสีทันที (และไม่ต้องปิดการทำงานของ Animator แล้ว!)
        if (characterSpriteRenderer.color != selectedColor)
        {
            characterSpriteRenderer.color = selectedColor;
            Debug.Log($"🎭 [ย้อมสีร่างแปลงสำเร็จ] สลับไปใช้โทนสีของร่างหมายเลข {currentForm} แล้ว!");
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