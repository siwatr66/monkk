// SaveSystem.cs
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadGameData();
    }

    public void SaveGameData()
    {
        if (ShapeshiftManager.Instance == null) return;

        int count = ShapeshiftManager.Instance.unlockedForms.Count;
        PlayerPrefs.SetInt("UnlockedCount", count);

        foreach (var pair in ShapeshiftManager.Instance.unlockedForms)
        {
            PlayerPrefs.SetInt("Slot_" + pair.Key + "_Type", (int)pair.Value.type);
        }
        PlayerPrefs.Save();
        Debug.Log("Game Saved!");
    }

    public void LoadGameData()
    {
        int count = PlayerPrefs.GetInt("UnlockedCount", 0);
        if (count == 0 || ShapeshiftManager.Instance == null) return;

        for (int i = 1; i <= 5; i++)
        {
            if (PlayerPrefs.HasKey("Slot_" + i + "_Type"))
            {
                int typeId = PlayerPrefs.GetInt("Slot_" + i + "_Type");
                EnemyType savedType = (EnemyType)typeId;

                // Unity 6 ยังคงรองรับ Resources.Load แต่แนะนำให้โฟลเดอร์ชื่ออยู่ใน Assets/Resources/Enemies/
                Sprite loadedSprite = Resources.Load<Sprite>("Enemies/" + savedType.ToString());

                if (!ShapeshiftManager.Instance.unlockedForms.ContainsKey(i))
                {
                    ShapeshiftManager.Instance.unlockedForms.Add(i, new ShapeshiftManager.FormSkill 
                    { 
                        type = savedType, 
                        sprite = loadedSprite 
                    });
                }
            }
        }
        
        if (UIManager.Instance != null) UIManager.Instance.UpdateSkillUI();
        Debug.Log("Game Loaded!");
    }

    [ContextMenu("Clear Save")]
    public void ClearSave()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Save Cleared!");
    }
}