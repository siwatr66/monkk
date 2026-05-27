using UnityEngine;
using UnityEngine.SceneManagement; // สั่งเปิดระบบควบคุมข้ามซีน

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Win Settings")]
    [SerializeField] private int enemiesToWin = 4; // ⚔️ ชกมอนสเตอร์ครบ 4 ธาตุชนะเกมทันที
    [SerializeField] private string victorySceneName = "Victory"; // ชื่อซีนชัยชนะของโอมส์
    private int currentKills = 0;

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

    void Start()
    {
        currentKills = 0;
    }

    // 💀 ระบบเมื่อผู้เล่นเลือดหมดหลอด (ตาย)
    public void PlayerDied()
    {
        Debug.Log("💀 [GAMEMANAGER] พระตาย! รีเซ็ตแต้มคิล และส่งกลับจุดเกิด...");
        ResetLevelOnDeath();
    }

    private void ResetLevelOnDeath()
    {
        currentKills = 0; // พระตาย แต้มเริ่มนับหนึ่งใหม่หมด

        PlayerHealth player = Object.FindAnyObjectByType<PlayerHealth>();
        if (player != null)
        {
            // วาร์ปพระกลับไปที่จุด Spawn Point
            GameObject spawnPoint = GameObject.Find("Spawn Point");
            if (spawnPoint != null)
            {
                player.transform.position = spawnPoint.transform.position;
            }
            // รีเซ็ตเลือดพระกลับมาเต็มหลอด
            player.RespawnSetup();
        }

        // 🧹 สั่งทำลายมอนสเตอร์ตัวเก่าตกค้างในด่านทิ้งให้หมดฉากทันที เพื่อป้องกันการแยกร่าง
        EnemyHealth[] activeEnemies = Object.FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);
        foreach (EnemyHealth enemy in activeEnemies)
        {
            Destroy(enemy.gameObject);
        }

        // สั่งเครื่องเสกให้รีสตาร์ทจ่ายมอนสเตอร์ตัวที่ 1 ลงสนามใหม่แบบคลีนๆ
        if (EnemySpawnerManager.Instance != null)
        {
            EnemySpawnerManager.Instance.ResetSpawner();
        }
    }

    // ⚔️ ระบบเมื่อพระชกมอนสเตอร์ตาย (พระชนะ)
    public void HandleEnemyDefeated()
    {
        currentKills++;
        Debug.Log($"🥊 [GAMEMANAGER] พระฆ่ามอนสเตอร์สำเร็จ! แต้มปัจจุบัน: {currentKills} / {enemiesToWin}");

        // 🏆 ถ้ารวบรวมพลังแปลงร่างชกครบ 4 ตัว ชนะด่านข้ามซีนทันที!
        if (currentKills >= enemiesToWin)
        {
            Debug.Log($"🏆 [YOU WIN] ชนะหลูปเกมแล้ว! กำลังโหลดสลับไปที่ซีน: {victorySceneName}");
            SceneManager.LoadScene(victorySceneName);
        }
        else
        {
            // ถ้ายังตีไม่ครบ ให้สปอนเนอร์ส่งตัวธาตุถัดไปในตลับลงมาให้ตีต่อ
            if (EnemySpawnerManager.Instance != null)
            {
                EnemySpawnerManager.Instance.NextEnemy();
            }
        }
    }
}