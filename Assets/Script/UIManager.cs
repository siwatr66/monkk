// UIManager.cs (เวอร์ชันสมบูรณ์แบบเต็มไฟล์ แก้ไขบั๊กการดึงคลังรูปภาพสล็อตโมบายเรียบร้อย)
using UnityEngine;
using UnityEngine.UI; // จำเป็นต้องเปิดใช้ Namespace นี้เพื่อควบคุมรูปภาพบนหน้าจอ Canvas

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Skill UI Settings")]
    [SerializeField] private Image[] skillButtonImages; // อาร์เรย์เก็บรูปภาพปุ่มช่องสล็อตบนหน้าจอ
    [SerializeField] private Sprite lockedSprite;        // รูปภาพพิกเซลอาร์ตตอนที่ช่องสล็อตนั้นยังล็อกอยู่

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

    private void Start()
    {
        UpdateSkillUI();
    }

    // ⭐ ฟังก์ชันอัปเดตหน้าจอ: ปรับแต่งสีสันและรูปภาพไอคอนให้ตรงตามการโหลดประวัติเซฟในเครื่องมือถือ
    public void UpdateSkillUI()
    {
        if (ShapeshiftManager.Instance == null || skillButtonImages == null) return;

        // ดึงคลังข้อมูล HashSet ตัวเลขที่เปิดล็อกแล้วออกมาตรวจเช็คค่า
        var unlockedList = ShapeshiftManager.Instance.GetUnlockedForms();

        // วิ่งลูปตรวจเช็คปุ่มสล็อตทั้ง 4 ช่องบนหน้าจอมือถือ (1=น้ำ, 2=ไฟ, 3=ลม, 4=ดิน)
        for (int i = 1; i <= 4; i++)
        {
            // ตรวจสอบความปลอดภัยป้องกันการชี้ตำแหน่งอินเดกซ์อาร์เรย์เกินขนาดพิกัด
            if (i - 1 >= skillButtonImages.Length || skillButtonImages[i - 1] == null) continue;

            // 🟢 [กรณีที่ 1]: ถ้าผู้เล่นเคยตบมอนสเตอร์ตายและเปิดล็อกช่องธาตุนี้สำเร็จแล้ว
            if (unlockedList.Contains(i))
            {
                // แสดงผลสีสว่างเต็มร้อยเปอร์เซ็นต์เพื่อให้ผู้เล่นรู้ว่าใช้นิ้วมือกดทัชเปลี่ยนร่างได้
                skillButtonImages[i - 1].color = Color.white;
            }
            // 🔴 [กรณีที่ 2]: ช่องสล็อตนั้นยังไม่ถูกเปิดล็อก (ยังไม่ได้ตบบอสประจำธาตุ)
            else
            {
                if (lockedSprite != null)
                {
                    // เอารูปกุญแจล็อกหรือรูปพื้นหลังว่างๆ มาแปะแทน
                    skillButtonImages[i - 1].sprite = lockedSprite;
                    skillButtonImages[i - 1].color = Color.white;
                }
                else
                {
                    // ถ้าโอมส์ไม่ได้ใส่รูปทับไว้ ให้ทำเป็นปุ่มสีดำโปร่งแสง 50% เพื่อให้ดูเป็นช่องสถานะล็อกออโต้
                    skillButtonImages[i - 1].color = new Color(0f, 0f, 0f, 0.5f);
                }
            }
        }

        Debug.Log("🖥️ [UI Manager] อัปเดตสถานะความสว่างปุ่มสล็อตสลับร่างบนจอมือถือเรียบร้อย!");
    }
}