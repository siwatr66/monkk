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

    [Header("Combat Settings")]
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
        // 1. ระบบรับค่าเคลื่อนที่จากคีย์บอร์ด
        float keyboardMoveInput = 0f;
        if (InputSystemKeyboardCheck(KeyCode.D) || InputSystemKeyboardCheck(KeyCode.RightArrow)) keyboardMoveInput = 1f;
        else if (InputSystemKeyboardCheck(KeyCode.A) || InputSystemKeyboardCheck(KeyCode.LeftArrow)) keyboardMoveInput = -1f;

        combinedMoveInput = Mathf.Abs(keyboardMoveInput) > 0.1f ? keyboardMoveInput : mobileMoveInput;

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(combinedMoveInput * moveSpeed, rb.linearVelocity.y);
        }

        if (combinedMoveInput > 0 && !isFacingRight) Flip();
        else if (combinedMoveInput < 0 && isFacingRight) Flip();

        if (anim != null) anim.SetFloat(SpeedHash, Mathf.Abs(combinedMoveInput));

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        // 2. ระบบสั่งกระโดดและโจมตี
        if (InputSystemKeyboardCheckDown(KeyCode.Space) && isGrounded) JumpLogic();
        if (InputSystemMouseClickCheck()) AttackLogic();

        // ⭐ 3. [ระบบดักจับปุ่มแปลงร่าง 1-4 สำหรับ Unity 6]
        HandleShapeshiftInput();
    }

    // ⭐ อัลกอริทึมเช็คปุ่มกดเลข 1-4 เพื่อสั่งสลับร่างแบบยิงตรงเข้า Manager
    // เปลี่ยนมาใช้ระบบส่งตัวเลขเจาะจง เพื่อให้ตรงกับ ShapeshiftManager ดั้งเดิมของคุณ
    private void HandleShapeshiftInput()
    {
        if (ShapeshiftManager.Instance == null) return;

        if (InputSystemKeyboardCheckDown(KeyCode.Alpha1))
        {
            // ส่งเลข 1 แทน (หรือถ้าในระบบคุณนับ Water เป็นเลขอื่น เช่น 0 หรือ 2 สามารถเปลี่ยนเลขในวงเล็บได้เลยครับ)
            ShapeshiftManager.Instance.TransformToForm(1);
            Debug.Log("[Player] กดปุ่มเลข 1 สลับร่างธาตุน้ำ!");
        }
        else if (InputSystemKeyboardCheckDown(KeyCode.Alpha2))
        {
            ShapeshiftManager.Instance.TransformToForm(2); // ส่งเลข 2 แทนธาตุไฟ
            Debug.Log("[Player] กดปุ่มเลข 2 สลับร่างธาตุไฟ!");
        }
        else if (InputSystemKeyboardCheckDown(KeyCode.Alpha3))
        {
            ShapeshiftManager.Instance.TransformToForm(3); // ส่งเลข 3 แทนธาตุลม
            Debug.Log("[Player] กดปุ่มเลข 3 สลับร่างธาตุลม!");
        }
        else if (InputSystemKeyboardCheckDown(KeyCode.Alpha4))
        {
            ShapeshiftManager.Instance.TransformToForm(4); // ส่งเลข 4 แทนธาตุดิน
            Debug.Log("[Player] กดปุ่มเลข 4 สลับร่างธาตุดิน!");
        }
        else if (InputSystemKeyboardCheckDown(KeyCode.Alpha0))
        {
            ShapeshiftManager.Instance.TransformToForm(0); // ส่งเลข 0 กลับร่างปกติ
            Debug.Log("[Player] กดปุ่มเลข 0 กลับสู่ร่างพระปกติ");
        }
    }

    private void JumpLogic()
    {
        if (rb != null) rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private void AttackLogic()
    {
        if (anim != null) anim.SetTrigger(AttackHash);

        if (attackPoint == null) return;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth == null) enemyHealth = enemy.GetComponentInChildren<EnemyHealth>();

            if (enemyHealth != null)
            {
                // ดึงข้อมูลธาตุปัจจุบันที่พระกำลังแปลงร่างอยู่ไปคำนวณดาเมจคูณไขว้
                EnemyType currentForm = EnemyType.Normal;
                if (ShapeshiftManager.Instance != null)
                {
                    currentForm = ShapeshiftManager.Instance.CurrentForm;
                }

                enemyHealth.TakeDamage(attackDamage, currentForm);
            }
        }
    }

    // ฟังก์ชันตรวจจับการกดปุ่มออโต้ รองรับทั้ง Input System เก่าและใหม่
    private bool InputSystemKeyboardCheck(KeyCode key)
    {
#if ENABLE_INPUT_SYSTEM
        var currentKeyboard = UnityEngine.InputSystem.Keyboard.current;
        if (currentKeyboard == null) return false;
        if (key == KeyCode.D || key == KeyCode.RightArrow) return currentKeyboard.dKey.isPressed || currentKeyboard.rightArrowKey.isPressed;
        if (key == KeyCode.A || key == KeyCode.LeftArrow) return currentKeyboard.aKey.isPressed || currentKeyboard.leftArrowKey.isPressed;
#endif
        return Input.GetKey(key);
    }

    private bool InputSystemKeyboardCheckDown(KeyCode key)
    {
#if ENABLE_INPUT_SYSTEM
        var currentKeyboard = UnityEngine.InputSystem.Keyboard.current;
        if (currentKeyboard == null) return false;
        if (key == KeyCode.Space) return currentKeyboard.spaceKey.wasPressedThisFrame;
        if (key == KeyCode.Alpha1) return currentKeyboard.digit1Key.wasPressedThisFrame;
        if (key == KeyCode.Alpha2) return currentKeyboard.digit2Key.wasPressedThisFrame;
        if (key == KeyCode.Alpha3) return currentKeyboard.digit3Key.wasPressedThisFrame;
        if (key == KeyCode.Alpha4) return currentKeyboard.digit4Key.wasPressedThisFrame;
        if (key == KeyCode.Alpha0) return currentKeyboard.digit0Key.wasPressedThisFrame;
#endif
        return Input.GetKeyDown(key);
    }

    private bool InputSystemMouseClickCheck()
    {
#if ENABLE_INPUT_SYSTEM
        var currentMouse = UnityEngine.InputSystem.Mouse.current;
        if (currentMouse == null) return false;
        return currentMouse.leftButton.wasPressedThisFrame;
#endif
        return Input.GetMouseButtonDown(0);
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