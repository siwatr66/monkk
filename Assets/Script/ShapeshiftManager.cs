// ShapeshiftManager.cs
using UnityEngine;
using System.Collections.Generic;

public class ShapeshiftManager : MonoBehaviour
{
    public static ShapeshiftManager Instance { get; private set; }

    public Dictionary<int, FormSkill> unlockedForms = new Dictionary<int, FormSkill>();
    public EnemyType CurrentForm { get; private set; } = EnemyType.None;
    
    private SpriteRenderer playerSpriteRenderer;
    private Sprite originalSprite;

    [System.Serializable]
    public class FormSkill
    {
        public EnemyType type;
        public Sprite sprite;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Unity 6 แนะนำให้ใช้ TryGetComponent เพื่อความเร็วและปลอดภัย ป้องกัน NullReference
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && player.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
        {
            playerSpriteRenderer = spriteRenderer;
            originalSprite = playerSpriteRenderer.sprite;
        }
    }

    void Update()
    {
        // ตัวทดสอบบน Unity Editor คีย์บอร์ดคอมพิวเตอร์
        if (Input.GetKeyDown(KeyCode.Alpha1)) TransformToForm(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) TransformToForm(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) TransformToForm(3);
        if (Input.GetKeyDown(KeyCode.Alpha4)) TransformToForm(4);
        if (Input.GetKeyDown(KeyCode.Alpha5)) TransformToForm(5);
    }

    public void TransformToForm(int slot)
    {
        if (unlockedForms.TryGetValue(slot, out var form))
        {
            CurrentForm = form.type;
            if (playerSpriteRenderer != null)
            {
                playerSpriteRenderer.sprite = form.sprite;
            }
            Debug.Log($"Shapeshifted into: {CurrentForm}");
        }
        else
        {
            Debug.Log($"Slot {slot} is empty!");
        }
    }

    public void RevertToOriginal()
    {
        CurrentForm = EnemyType.None;
        if (playerSpriteRenderer != null)
        {
            playerSpriteRenderer.sprite = originalSprite;
        }
        Debug.Log("Reverted to Monk form");
    }

    public void UnlockForm(EnemyType type, Sprite sprite)
    {
        foreach (var pair in unlockedForms)
        {
            if (pair.Value.type == type) return;
        }

        for (int i = 1; i <= 5; i++)
        {
            if (!unlockedForms.ContainsKey(i))
            {
                FormSkill newForm = new FormSkill { type = type, sprite = sprite };
                unlockedForms.Add(i, newForm);
                Debug.Log($"Unlocked {type} on Slot {i}!");
                
                if (UIManager.Instance != null) UIManager.Instance.UpdateSkillUI();
                break;
            }
        }
    }
}