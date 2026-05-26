// GameManager.cs
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // เมื่อพระชนะศัตรูแต่ละตัวได้ -> เลือดพระเต็ม + เรียกตัวถัดไป
    public void LevelCleared()
    {
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.SaveGameData();
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && player.TryGetComponent<PlayerHealth>(out var playerHealth))
        {
            playerHealth.HealFull();
        }

        if (EnemySpawnerManager.Instance != null)
        {
            EnemySpawnerManager.Instance.NextEnemy();
        }
    }

    // เมื่อพระตาย -> ย้ายตัวพระกลับจุดเกิด และรีเซ็ตมอนสเตอร์กลับไปตัวแรกสุด
    public void PlayerDied()
    {
        if (EnemySpawnerManager.Instance != null)
        {
            EnemySpawnerManager.Instance.ResetSpawner();
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject spawnObj = GameObject.Find("RespawnPoint");

        if (player != null && spawnObj != null)
        {
            player.transform.position = spawnObj.transform.position;

            if (player.TryGetComponent<PlayerHealth>(out var playerHealth))
            {
                playerHealth.RespawnSetup();
            }
        }
    }
}