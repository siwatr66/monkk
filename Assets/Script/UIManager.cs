// UIManager.cs
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Skill Buttons UI (Slot 1-5)")]
    [SerializeField] private Image[] skillButtonImages; 
    [SerializeField] private Sprite lockedSprite;       

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
        UpdateSkillUI();
    }

    public void UpdateSkillUI()
    {
        if (ShapeshiftManager.Instance == null) return;

        for (int i = 1; i <= 5; i++)
        {
            // ป้องกัน Array Out Of Bound Index ขอบเขตปุ่มเบื้องต้น
            if (i - 1 >= skillButtonImages.Length || skillButtonImages[i - 1] == null) continue;

            if (ShapeshiftManager.Instance.unlockedForms.ContainsKey(i))
            {
                skillButtonImages[i - 1].sprite = ShapeshiftManager.Instance.unlockedForms[i].sprite;
                skillButtonImages[i - 1].color = Color.white; 
            }
            else
            {
                if (lockedSprite != null)
                {
                    skillButtonImages[i - 1].sprite = lockedSprite;
                }
                else
                {
                    skillButtonImages[i - 1].color = new Color(0f, 0f, 0f, 0.5f); 
                }
            }
        }
    }
}