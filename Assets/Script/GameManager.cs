using UnityEngine;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Victory Settings")]
    [SerializeField] private int enemiesToWin = 4;

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

    public void LevelCleared()
    {
        HandleEnemyDefeated();
    }

    public void HandleEnemyDefeated()
    {
        if (!IsPlaying) return;

        defeatedEnemiesCount++;
        SceneFlowState.RecordRun(defeatedEnemiesCount, enemiesToWin);

        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.SaveGameData();
        }

        if (defeatedEnemiesCount >= enemiesToWin)
        {
            LoadVictoryScene();
            return;
        }

        HealPlayerAfterKill();

        bool spawnedNextEnemy = EnemySpawnerManager.Instance != null && EnemySpawnerManager.Instance.SpawnNextEnemy();
        if (!spawnedNextEnemy)
        {
            LoadVictoryScene();
        }
    }

    public void PlayerDied()
    {
        if (!IsPlaying) return;

        currentState = GameState.GameOver;
        SetPlayerControlEnabled(false);
        SceneFlowState.RecordRun(defeatedEnemiesCount, enemiesToWin);
        GameSceneManager.LoadGameOver();
    }

    private void BeginGameplay()
    {
        currentState = GameState.Playing;
        defeatedEnemiesCount = 0;
        Time.timeScale = 1f;
        SceneFlowState.BeginRun(enemiesToWin);

        ResetPlayerToSpawn();

        if (EnemySpawnerManager.Instance != null)
        {
            EnemySpawnerManager.Instance.BeginRun();
        }
    }

    private void LoadVictoryScene()
    {
        if (currentState == GameState.Victory) return;

        currentState = GameState.Victory;
        SetPlayerControlEnabled(false);
        SceneFlowState.RecordRun(defeatedEnemiesCount, enemiesToWin);
        GameSceneManager.LoadVictory();
    }

    private void HealPlayerAfterKill()
    {
        GameObject player = FindPlayer();
        if (player != null && player.TryGetComponent<PlayerHealth>(out var playerHealth))
        {
            playerHealth.HealFull();
        }
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

        if (player.TryGetComponent<PlayerHealth>(out var playerHealth))
        {
            playerHealth.RespawnSetup();
        }

        SetPlayerControlEnabled(true);
    }

    private void SetPlayerControlEnabled(bool enableControl)
    {
        GameObject player = FindPlayer();
        if (player == null) return;

        if (player.TryGetComponent<PlayerController>(out var playerController))
        {
            playerController.enabled = enableControl;
        }

        if (!enableControl && player.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private GameObject FindPlayer()
    {
        return GameObject.FindGameObjectWithTag("Player");
    }
}
