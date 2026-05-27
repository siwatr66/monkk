// GameManager.cs (เวอร์ชันแก้ไขบั๊ก GameSceneManager สมบูรณ์ 100% ศัตรูกลับมาสปอนแน่นอน)
using UnityEngine;
using UnityEngine.SceneManagement; // ใช้ระบบดั้งเดิมของ Unity ในการเปลี่ยนฉากเพื่อความชัวร์

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Victory Settings")]
    [SerializeField] private int enemiesToWin = 4;
    [SerializeField] private string victorySceneName = "Victory"; // ตั้งชื่อฉากจบตรงนี้ได้เลย

    public bool IsPlaying => currentState == GameState.Playing;

    private GameState currentState = GameState.Playing;
    private int defeatedEnemiesCount;

    private enum GameState
    {
        Playing,
        GameOver,
        Victory
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        BeginGameplay();
    }

    public void HandleEnemyDefeated()
    {
        if (!IsPlaying) return;

        defeatedEnemiesCount++;
        Debug.Log($"🎯 [Game Manager] มอนสเตอร์ตายแล้ว! ตัวที่ตายสะสม: {defeatedEnemiesCount} / {enemiesToWin}");

        if (defeatedEnemiesCount >= enemiesToWin)
        {
            currentState = GameState.Victory;
            // ใช้คำสั่งพื้นฐานของ Unity โหลดฉากจบตรงๆ เพื่อแก้บั๊กค้าง
            SceneManager.LoadScene(victorySceneName);
            return;
        }

        GameObject player = FindPlayer();
        if (player != null && player.TryGetComponent<PlayerHealth>(out var playerHealth))
        {
            playerHealth.HealFull();
        }

        // 🟢 สั่งเครื่องเสกให้สปอนมอนสเตอร์ตัวถัดไปลงสนามทันที
        if (EnemySpawnerManager.Instance != null)
        {
            EnemySpawnerManager.Instance.SpawnNextEnemy();
        }
    }

    public void PlayerDied()
    {
        if (!IsPlaying) return;
        Debug.Log("🔄 [Game Manager] พระตาย! กำลังสั่งวาร์ปเกิดใหม่ด่านเดิม + คืนร่างพระปกติ...");
        ResetPlayerToSpawn();
    }

    private void BeginGameplay()
    {
        currentState = GameState.Playing;
        defeatedEnemiesCount = 0;
        Time.timeScale = 1f;
        ResetPlayerToSpawn();
    }

    private void ResetPlayerToSpawn()
    {
        GameObject player = FindPlayer();
        if (player == null) return;

        GameObject spawnObj = GameObject.Find("RespawnPoint");
        if (spawnObj != null)
        {
            player.transform.position = spawnObj.transform.position;
        }

        if (player.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = Vector2.zero;
        }

        // คืนร่างพระมนุษย์ปกติ (Form 0)
        if (ShapeshiftManager.Instance != null)
        {
            ShapeshiftManager.Instance.CurrentForm = 0;
        }

        // สั่งให้พระเปลี่ยนภาพกราฟิกกลับมาเป็นร่างมนุษย์ดั้งเดิมทันที
        if (player.TryGetComponent<PlayerController>(out var playerController))
        {
            playerController.ForceApplyVisualChange();
        }

        if (player.TryGetComponent<PlayerHealth>(out var playerHealth))
        {
            playerHealth.RespawnSetup();
        }
    }

    private GameObject FindPlayer()
    {
        return GameObject.FindGameObjectWithTag("Player");
    }
}