// EnemySpawnerManager.cs
using UnityEngine;

public class EnemySpawnerManager : MonoBehaviour
{
    public static EnemySpawnerManager Instance { get; private set; }

    [Header("Enemy Queue Settings")]
    [SerializeField] private GameObject[] enemyPrefabs; // ใส่ขนาด 4 ช่อง (ลากศัตรูทั้ง 4 ธาตุมาใส่)
    [SerializeField] private Transform spawnPoint;

    [Header("Patrol Reference for Spawned Enemies")]
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;

    private int currentEnemyIndex = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        SpawnCurrentEnemy();
    }

    public void SpawnCurrentEnemy()
    {
        // เช็คว่าดัชนีคิวไม่เกินจำนวน Prefab ในลิสต์ (0 ถึง 3)
        if (currentEnemyIndex < enemyPrefabs.Length)
        {
            if (enemyPrefabs[currentEnemyIndex] == null)
            {
                Debug.LogError($"[Spawner] ช่องลำดับที่ {currentEnemyIndex} ไม่มี Prefab ศัตรูใส่ไว้ครับ!");
                return;
            }

            // เสกศัตรูลงจุดเกิด
            GameObject newEnemy = Instantiate(enemyPrefabs[currentEnemyIndex], spawnPoint.position, Quaternion.identity);

            // ⭐ [จุดแก้บั๊กสำคัญ] บังคับฉีด Tag คำว่า "Enemy" ใส่ตัวที่เสกใหม่ทันที 
            // ป้องกันปัญหากรณี Prefab ในคลังลืมตั้งค่า Tag จะได้ไม่เกิดอาการตัวถัดไปสปอว์นเงียบ
            newEnemy.tag = "Enemy";

            // ตั้งชื่อให้จำง่ายใน Hierarchy
            newEnemy.name = "Enemy_Form_" + enemyPrefabs[currentEnemyIndex].name;
            Debug.Log($"[Spawner] เสกศัตรูตัวใหม่สำเร็จ: {newEnemy.name} (ลำดับคิวที่: {currentEnemyIndex})");

            // บังคับส่งพิกัดหมุดเดินเข้าสมอง AI
            if (newEnemy.TryGetComponent<EnemyAI>(out var enemyAI))
            {
                enemyAI.SetupPatrolPoints(leftPoint, rightPoint);
            }
        }
        else
        {
            Debug.Log("!! ปราบครบ 4 ธาตุสมบูรณ์ ชนะเกมทั้งหมดในซีนเดียว !!");
        }
    }

    // ฟังก์ชันเลื่อนคิวเพื่อสปอว์นตัวถัดไป
    public void NextEnemy()
    {
        currentEnemyIndex++;
        Debug.Log($"[Spawner] สั่งขยับคิวเลื่อนไปตัวถัดไป -> ดัชนีปัจจุบัน: {currentEnemyIndex}");

        // สั่งเสกตัวใหม่ลงมาลุย
        SpawnCurrentEnemy();
    }

    // ฟังก์ชันล้างสนามเริ่มใหม่ตอนพระตาย
    public void ResetSpawner()
    {
        currentEnemyIndex = 0;
        Debug.Log("[Spawner] ตัวพระตาย! ทำการรีเซ็ตดัชนีคิวกลับไปตัวแรกสุด (Index 0)");

        // กวาดล้างศัตรูที่เหลือรอดทิ้งทั้งหมดในฉาก
        GameObject[] activeEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in activeEnemies)
        {
            Destroy(enemy);
        }

        // เสกตัวแรกสุด (Element 0) ใหม่อีกครั้ง
        SpawnCurrentEnemy();
    }
}