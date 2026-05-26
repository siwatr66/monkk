using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    private static GameSceneManager instance;
    private static bool initialSceneResolved;
    private static bool initialSceneHookInstalled;

    public static GameSceneManager Instance
    {
        get
        {
            EnsureInstance();
            return instance;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        EnsureInstance();
        InstallInitialSceneGuard();
    }

    private static void InstallInitialSceneGuard()
    {
        if (initialSceneHookInstalled) return;

        SceneManager.sceneLoaded += HandleInitialSceneLoaded;
        initialSceneHookInstalled = true;
        initialSceneResolved = false;
    }

    private static void HandleInitialSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (initialSceneResolved) return;

        initialSceneResolved = true;
        SceneManager.sceneLoaded -= HandleInitialSceneLoaded;
        initialSceneHookInstalled = false;

        if (scene.name != SceneFlowState.MainMenuSceneName)
        {
            LoadMainMenu();
        }
    }

    private static void EnsureInstance()
    {
        if (instance != null) return;

        instance = FindAnyObjectByType<GameSceneManager>();
        if (instance != null)
        {
            DontDestroyOnLoad(instance.gameObject);
            return;
        }

        GameObject managerObject = new GameObject("GameSceneManager");
        instance = managerObject.AddComponent<GameSceneManager>();
        DontDestroyOnLoad(managerObject);
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }

        if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public static void LoadMainMenu()
    {
        SceneFlowState.ResetRunInfo();
        Instance.LoadSceneByName(SceneFlowState.MainMenuSceneName);
    }

    public static void LoadGameplay()
    {
        SceneFlowState.BeginRun(SceneFlowState.DefaultEnemiesToWin);
        Instance.LoadSceneByName(SceneFlowState.GameplaySceneName);
    }

    public static void LoadGameOver()
    {
        Instance.LoadSceneByName(SceneFlowState.GameOverSceneName);
    }

    public static void LoadVictory()
    {
        Instance.LoadSceneByName(SceneFlowState.VictorySceneName);
    }

    private void LoadSceneByName(string sceneName)
    {
        if (!IsSceneInBuildSettings(sceneName))
        {
            Debug.LogError($"[GameSceneManager] Scene '{sceneName}' is not in Build Settings.");
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    private bool IsSceneInBuildSettings(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string buildSceneName = Path.GetFileNameWithoutExtension(path);
            if (buildSceneName == sceneName)
            {
                return true;
            }
        }

        return false;
    }
}
