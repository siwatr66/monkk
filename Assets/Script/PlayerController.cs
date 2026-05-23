// PlayerController.cs
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    private Rigidbody2D rb;
    private float mobileMoveInput = 0f;
    private float combinedMoveInput = 0f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private LayerMask whatIsGround;
    private bool isGrounded;

    [Header("Combat")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private int attackDamage = 10;

    private Animator anim;
    private bool isFacingRight = true;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int AttackHash = Animator.StringToHash("Attack");

    void Start()
    {
        if (TryGetComponent<Rigidbody2D>(out var rigidbody2D)) rb = rigidbody2D;
        if (TryGetComponent<Animator>(out var animator)) anim = animator;
    }

    void Update()
    {
        float keyboardMoveInput = 0f;

        // ตรวจสอบการกดปุ่มบนคีย์บอร์ด (A/D หรือ ลูกศรซ้ายขวา)
        if (InputSystemKeyboardCheck(KeyCode.D) || InputSystemKeyboardCheck(KeyCode.RightArrow)) keyboardMoveInput = 1f;
        else if (InputSystemKeyboardCheck(KeyCode.A) || InputSystemKeyboardCheck(KeyCode.LeftArrow)) keyboardMoveInput = -1f;

        if (Mathf.Abs(keyboardMoveInput) > 0.1f)
        {
            combinedMoveInput = keyboardMoveInput;
        }
        else
        {
            combinedMoveInput = mobileMoveInput;
        }

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(combinedMoveInput * moveSpeed, rb.linearVelocity.y);
        }

        if (combinedMoveInput > 0 && !isFacingRight) Flip();
        else if (combinedMoveInput < 0 && isFacingRight) Flip();

        if (anim != null) anim.SetFloat(SpeedHash, Mathf.Abs(combinedMoveInput));

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        // ตรวจสอบปุ่ม Spacebar (กระโดด) สำหรับคีย์บอร์ดคอมพิวเตอร์
        if (InputSystemKeyboardCheckDown(KeyCode.Space) && isGrounded)
        {
            JumpLogic();
        }

        // --- แก้ไขระบบตรวจจับคลิกเมาส์ซ้าย ผ่าน Input System (New) ---
        if (InputSystemMouseClickCheck())
        {
            AttackLogic();
            Debug.Log("Attack triggered by Left Mouse Click.");
        }
    }

    // ฟังก์ชันช่วยตรวจสอบการกดปุ่มบนคีย์บอร์ด
    private bool InputSystemKeyboardCheck(KeyCode key)
    {
#if ENABLE_INPUT_SYSTEM
        var currentKeyboard = UnityEngine.InputSystem.Keyboard.current;
        if (currentKeyboard == null) return false;

        if (key == KeyCode.D || key == KeyCode.RightArrow) return currentKeyboard.dKey.isPressed || currentKeyboard.rightArrowKey.isPressed;
        if (key == KeyCode.A || key == KeyCode.LeftArrow) return currentKeyboard.aKey.isPressed || currentKeyboard.leftArrowKey.isPressed;
#endif
        return false;
    }

    private bool InputSystemKeyboardCheckDown(KeyCode key)
    {
#if ENABLE_INPUT_SYSTEM
        var currentKeyboard = UnityEngine.InputSystem.Keyboard.current;
        if (currentKeyboard == null) return false;

        if (key == KeyCode.Space) return currentKeyboard.spaceKey.wasPressedThisFrame;
#endif
        return false;
    }

    // ฟังก์ชันใหม่: ตรวจสอบการคลิกเมาส์ซ้าย (Mouse0) แบบปลอดภัยไม่ให้เด้ง Error ใน Unity 6
    private bool InputSystemMouseClickCheck()
    {
#if ENABLE_INPUT_SYSTEM
        var currentMouse = UnityEngine.InputSystem.Mouse.current;
        if (currentMouse == null) return false;

        // เช็คว่ามีการ "กดคลิกเมาส์ซ้ายลงไปในเฟรมนี้" หรือไม่ (ป้องกันแอนิเมชันเล่นซ้อนกันวนลูป)
        return currentMouse.leftButton.wasPressedThisFrame;
#endif
        return false;
    }

    private void JumpLogic()
    {
        if (rb != null) rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private void AttackLogic()
    {
        //if (anim != null) anim.SetTrigger(AttackHash);
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (ShapeshiftManager.Instance != null)
            {
                EnemyType currentForm = ShapeshiftManager.Instance.CurrentForm;
                if (enemy.TryGetComponent<EnemyHealth>(out var enemyHealth))
                {
                    enemyHealth.TakeDamage(attackDamage, currentForm);
                    Debug.Log($"Attacked {enemy.name} for {attackDamage} damage with form {currentForm}.");
                }
            }
        }
    }

    public void Move(float direction) => mobileMoveInput = direction;
    public void MobileJump() { if (isGrounded) JumpLogic(); }
    public void MobileAttack() => AttackLogic();

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}