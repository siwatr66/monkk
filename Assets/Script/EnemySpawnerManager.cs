using UnityEngine;

public class EnemySpawnerManager : MonoBehaviour
{
    public static EnemySpawnerManager Instance { get; private set; }

    [Header("Enemy Queue Settings")]
    [SerializeField] private GameObject[] enemyPrefabs;
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

    public void BeginRun()
    {
        currentEnemyIndex = 0;
        ClearActiveEnemies();
        SpawnCurrentEnemy();
    }

    public void ResetToMenuState()
    {
        currentEnemyIndex = 0;
        ClearActiveEnemies();
    }

    public bool SpawnCurrentEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogError("[Spawner] Enemy queue is empty.");
            return false;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("[Spawner] Spawn point is missing.");
            return false;
        }

        if (currentEnemyIndex >= enemyPrefabs.Length)
        {
            Debug.Log("[Spawner] No enemy left in the queue.");
            return false;
        }

        if (enemyPrefabs[currentEnemyIndex] == null)
        {
            Debug.LogError($"[Spawner] Enemy prefab at index {currentEnemyIndex} is missing.");
            return false;
        }

        GameObject newEnemy = Instantiate(enemyPrefabs[currentEnemyIndex], spawnPoint.position, Quaternion.identity);
        newEnemy.tag = "Enemy";
        newEnemy.name = "Enemy_Form_" + enemyPrefabs[currentEnemyIndex].name;
        Debug.Log($"[Spawner] Spawned {newEnemy.name} at queue index {currentEnemyIndex}");

        if (newEnemy.TryGetComponent<EnemyAI>(out var enemyAI))
        {
            enemyAI.SetupPatrolPoints(leftPoint, rightPoint);
        }

        return true;
    }

    public bool NextEnemy()
    {
        currentEnemyIndex++;
        Debug.Log($"[Spawner] Moving to queue index {currentEnemyIndex}");
        return SpawnCurrentEnemy();
    }

    public bool SpawnNextEnemy()
    {
        return NextEnemy();
    }

    public void ResetSpawner()
    {
        BeginRun();
    }

    public void ClearActiveEnemies()
    {
        GameObject[] activeEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in activeEnemies)
        {
            enemy.tag = "Untagged";
            Destroy(enemy);
        }
    }
}
