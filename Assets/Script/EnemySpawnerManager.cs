// EnemySpawnerManager.cs (เวอร์ชันสมบูรณ์ 100% บังคับสตาร์ทเสกศัตรูตัวแรกทันที)
using UnityEngine;

public class EnemySpawnerManager : MonoBehaviour
{
    public static EnemySpawnerManager Instance { get; private set; }

    [Header("Spawner Settings")]
    [SerializeField] private GameObject[] enemyPrefabs; // อาร์เรย์เก็บแผ่น Prefab มอนสเตอร์
    [SerializeField] private Transform spawnPoint;      // จุดพิกจัดเสกมอนสเตอร์ในฉาก

    private int currentPrefabIndex = 0;
    private GameObject currentSpawnedEnemy;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 🟢 [ระบบเซฟโซนกันเหนียว]: สั่งเปิดสวิตช์เสกมอนตัวแรกทันทีเมื่อเข้าฉากเกม
        BeginRun();
    }

    // ฟังก์ชันสั่งเริ่มต้นวงจรเสกมอนสเตอร์ด่านหลัก
    public void BeginRun()
    {
        currentPrefabIndex = 0;

        // ถ้าในสนามยังไม่มีมอนสเตอร์ยืนอยู่เลย ให้สั่งเสกทันที
        if (currentSpawnedEnemy == null)
        {
            SpawnNextEnemy();
        }
    }

    // ฟังก์ชันหลักในการสั่งผลิตตัวมอนสเตอร์เด้งลงสนาม
    public bool SpawnNextEnemy()
    {
        // 1. เช็คความปลอดภัยก่อนว่ามีไฟล์ Prefab ใส่ไว้ในช่องหรือเปล่า
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogError("🛑 [Spawner Error] ลืมลากไฟล์มอนสเตอร์ใส่ในช่อง Enemy Prefabs ของเครื่องเสกศัตรู!");
            return false;
        }

        // 2. เช็คจุดพิกัดวาง ถ้าไม่มีให้ใช้ตำแหน่งของเครื่องเสกเองแทน
        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position;

        // ถ้าดัชนีเกินจำนวนคิวมอนสเตอร์ ให้ลูปวนกลับมาตัวแรกใหม่ (เล่นวนไปเรื่อยๆ)
        if (currentPrefabIndex >= enemyPrefabs.Length)
        {
            currentPrefabIndex = 0;
        }

        GameObject selectedPrefab = enemyPrefabs[currentPrefabIndex];

        if (selectedPrefab != null)
        {
            // 💥 สั่งคลอดวัตถุมอนสเตอร์ลงสนามฟิสิกส์ 2D
            currentSpawnedEnemy = Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
            Debug.Log($"👾 [Spawner Success] เสกมอนสเตอร์สำเร็จ! ตัวที่ปล่อย: {currentSpawnedEnemy.name} | ลำดับคิวค้าง: {currentPrefabIndex}");

            currentPrefabIndex++; // ขยับคิวเตรียมพร้อมสำหรับตัวถัดไป
            return true;
        }

        return false;
    }

    // ฟังก์ชันเรียกคิวมอนตัวถัดไป (สำหรับกรณีเชื่อมสายสัญญาณระบบเก่า)
    public void NextEnemy()
    {
        SpawnNextEnemy();
    }
}