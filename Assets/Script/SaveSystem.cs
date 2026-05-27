// SaveSystem.cs (เวอร์ชันเต็มไฟล์ - แก้ไขทางวิ่งเชื่อมต่อ HashSet สล็อตรงอกโมบายตัวล่าสุดเรียบร้อย)
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
        // โหลดข้อมูลปุ่มที่เคยได้มาคืนสู่หน้าจอทันทีเมื่อเปิดเกม
        LoadGameData();
    }

    // 💾 บันทึกสล็อตโมบายลงเครื่องความจำมือถือ
    public void SaveGameData()
    {
        if (ShapeshiftManager.Instance == null) return;

        // ดึงคลังข้อมูลตัวเลข HashSet ออกมาจากสคริปต์ ShapeshiftManager ข้ามสายสัญญาณมาเช็คค่า
        var activeSlots = ShapeshiftManager.Instance.GetUnlockedForms();

        // เซฟจำนวนปุ่มทัชสกรีนปัจจุบัน
        PlayerPrefs.SetInt("UnlockedCount", activeSlots.Count);

        int index = 0;
        foreach (int elementNumber in activeSlots)
        {
            // บันทึกหมายเลขธาตุเรียงทีละช่อง
            PlayerPrefs.SetInt("SavedSlot_" + index, elementNumber);
            index++;
        }

        PlayerPrefs.Save();
        Debug.Log("💾 [Save System] บันทึกตำแหน่งสล็อตพิกเซลอาร์ตลงเครื่องสำเร็จ!");
    }

    // 📂 ดึงข้อมูลเก่ามาสั่งเสกช่องงอกเรียงแถวกลางจอออโต้
    public void LoadGameData()
    {
        int savedCount = PlayerPrefs.GetInt("UnlockedCount", 0);

        if (savedCount == 0 || ShapeshiftManager.Instance == null)
        {
            Debug.Log("📂 [Save System] ไม่พบเซฟเก่า เริ่มต้นด่านใหม่แบบสล็อตว่าง");
            return;
        }

        // วิ่งดึงค่าตัวเลขเพื่อกระตุ้นให้ปุ่มงอกขึ้นมาจัดแถวพร้อมมือกด
        for (int i = 0; i < savedCount; i++)
        {
            if (PlayerPrefs.HasKey("SavedSlot_" + i))
            {
                int savedElementNumber = PlayerPrefs.GetInt("SavedSlot_" + i);
                ShapeshiftManager.Instance.UnlockForm(savedElementNumber);
            }
        }

        Debug.Log("📂 [Save System] เรียกคืนข้อมูลสล็อตเปลี่ยนร่างบนหน้าจอมือถือเสร็จสิ้น!");
    }

    [ContextMenu("Clear Save")]
    public void ClearSave()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("❌ [Save System] เคลียร์ข้อมูลประวัติเซฟในเครื่องเกลี้ยงหมดจดแล้ว!");
    }
}