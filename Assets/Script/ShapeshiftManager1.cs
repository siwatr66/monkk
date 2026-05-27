// ShapeshiftManager.cs (เวอร์ชันเสร็จสมบูรณ์ 100% ไร้บั๊กแดง)
using UnityEngine;
using UnityEngine.UI;

public class ShapeshiftManager : MonoBehaviour
{
    public static ShapeshiftManager Instance { get; private set; }

    [Header("UI Slot Container")]
    [SerializeField] private Transform slotContainer;

    [Header("Dynamic Prefab Assets")]
    [SerializeField] private GameObject prefabWater;
    [SerializeField] private GameObject prefabFire;
    [SerializeField] private GameObject prefabWind;
    [SerializeField] private GameObject prefabEarth;

    [Header("Current Status")]
    public int CurrentForm = 0;

    private System.Collections.Generic.HashSet<int> unlockedForms = new System.Collections.Generic.HashSet<int>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UnlockForm(int deadEnemyElementNumber)
    {
        if (unlockedForms.Contains(deadEnemyElementNumber)) return;

        GameObject prefabToSpawn = null;

        switch (deadEnemyElementNumber)
        {
            case 1: prefabToSpawn = prefabWater; break;
            case 2: prefabToSpawn = prefabFire; break;
            case 3: prefabToSpawn = prefabWind; break;
            case 4: prefabToSpawn = prefabEarth; break;
        }

        if (prefabToSpawn != null && slotContainer != null)
        {
            GameObject newSlotObj = Instantiate(prefabToSpawn, slotContainer);

            Button slotButton = newSlotObj.GetComponent<Button>();
            if (slotButton == null) slotButton = newSlotObj.AddComponent<Button>();

            int elementIndex = deadEnemyElementNumber;
            slotButton.onClick.AddListener(() => {
                TransformToForm(elementIndex);
            });

            unlockedForms.Add(deadEnemyElementNumber);
            Debug.Log($"🔮 [UI Spawner] เสกปุ่มหมายเลข {deadEnemyElementNumber} ขึ้นจอสำเร็จ!");
        }
    }

    public void TransformToForm(int formIndex)
    {
        CurrentForm = formIndex;
        Debug.Log($"🧘‍♂️ [Shapeshift System] สับสวิตช์ระบบไปที่ร่างหมายเลข: {CurrentForm}");

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && player.TryGetComponent<PlayerController>(out var playerController))
        {
            // เรียกใช้ฟังก์ชันตัวล่าสุดของพระเพื่อหักดิบเปลี่ยนรูปภาพ
            playerController.ForceApplyVisualChange();
        }
    }

    public System.Collections.Generic.HashSet<int> GetUnlockedForms()
    {
        return unlockedForms;
    }
}