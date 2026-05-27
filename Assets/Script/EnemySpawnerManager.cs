using UnityEngine;

public class EnemySpawnerManager : MonoBehaviour
{
    public static EnemySpawnerManager Instance { get; private set; }

    [Header("Spawner Settings")]
    [SerializeField] private GameObject[] enemyPrefabs; // ตลับใส่ไฟล์มอนสเตอร์ทั้ง 4 ธาตุ
    [SerializeField] private Transform spawnPoint;      // พิกัดจุดเสกในด่าน

    private int currentPrefabIndex = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 🟢 ดีเลย์นิดนึงตอนเริ่มเกม เพื่อให้มอนสเตอร์ตัวแรกโผล่มาอย่างปลอดภัย ไม่โดนลบฟรี
        Invoke("FirstSpawn", 0.05f);
    }

    private void FirstSpawn()
    {
        ResetSpawner();
    }

    // สั่งล้างควอนตัมคิว กลับมานับหนึ่งใหม่ตอนเปิดเกม หรือตอนที่พระตายด่านรีเซ็ต
    public void ResetSpawner()
    {
        currentPrefabIndex = 0;
        SpawnNextEnemy();
    }

    // สั่งเลื่อนคิวส่งมอนสเตอร์ธาตุถัดไปลงฉาก (ทำงานเมื่อพระตบตัวเก่าชนะ)
    public void NextEnemy()
    {
        SpawnNextEnemy();
    }

    private void SpawnNextEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogError("🛑 [Spawner Error] ลืมลากไฟล์มอนสเตอร์ใส่ในช่อง Enemy Prefabs ฝั่งขวาครับ!");
            return;
        }

        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position;

        // วนลูปกลับมาตัวแรกถ้าคิวเกินจำนวนที่มี
        if (currentPrefabIndex >= enemyPrefabs.Length)
        {
            currentPrefabIndex = 0;
        }

        GameObject selectedPrefab = enemyPrefabs[currentPrefabIndex];

        if (selectedPrefab != null)
        {
            // เสกวัตถุมอนสเตอร์ตัวจริงลงสู่ฉาก
            GameObject newEnemy = Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
            Debug.Log($"👾 [Spawner] เสกสำเร็จ: {newEnemy.name} | ลำดับคิวธาตุ: {currentPrefabIndex}");

            currentPrefabIndex++; // บันทึกคิวขยับรอตัวธาตุต่อไป
        }
    }
}