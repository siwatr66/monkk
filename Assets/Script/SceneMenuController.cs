using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

public class SceneMenuController : MonoBehaviour
{
    [SerializeField] private Text titleText;
    [SerializeField] private Text messageText;
    [SerializeField] private Button primaryButton;
    [SerializeField] private Text primaryButtonLabel;

    public void BindUi(Text title, Text message, Button button, Text buttonLabel)
    {
        titleText = title;
        messageText = message;
        primaryButton = button;
        primaryButtonLabel = buttonLabel;
    }

    void Start()
    {
        Time.timeScale = 1f;
        EnsureInputModule();
        ConfigureSceneUi();
    }

    private void ConfigureSceneUi()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        switch (sceneName)
        {
            case SceneFlowState.MainMenuSceneName:
                SetContent("MAIN MENU", "Defeat 4 monsters to win.", "Start", LoadGameplayScene);
                break;

            case SceneFlowState.GameOverSceneName:
                SetContent(
                    "GAME OVER",
                    $"You defeated {SceneFlowState.LastDefeatedEnemies}/{SceneFlowState.LastEnemiesToWin} monsters.",
                    "Restart",
                    LoadGameplayScene);
                break;

            case SceneFlowState.VictorySceneName:
                SetContent(
                    "VICTORY",
                    $"You defeated {SceneFlowState.LastDefeatedEnemies}/{SceneFlowState.LastEnemiesToWin} monsters.",
                    "Restart",
                    LoadGameplayScene);
                break;

            default:
                SetContent("MONKK", "Return to the main menu.", "Main Menu", LoadMainMenuScene);
                break;
        }
    }

    public void LoadGameplayScene()
    {
        GameSceneManager.LoadGameplay();
    }

    public void LoadMainMenuScene()
    {
        GameSceneManager.LoadMainMenu();
    }

    private void SetContent(string title, string message, string buttonText, UnityEngine.Events.UnityAction clickAction)
    {
        if (titleText != null) titleText.text = title;
        if (messageText != null) messageText.text = message;
        if (primaryButtonLabel != null) primaryButtonLabel.text = buttonText;

        if (primaryButton != null)
        {
            primaryButton.onClick.RemoveAllListeners();
            if (clickAction != null)
            {
                primaryButton.onClick.AddListener(clickAction);
            }
        }
    }

    private void EnsureInputModule()
    {
        if (EventSystem.current == null) return;

#if ENABLE_INPUT_SYSTEM
        if (EventSystem.current.GetComponent<InputSystemUIInputModule>() == null)
        {
            var module = EventSystem.current.gameObject.AddComponent<InputSystemUIInputModule>();
            module.AssignDefaultActions();
        }
#else
        if (EventSystem.current.GetComponent<StandaloneInputModule>() == null)
        {
            EventSystem.current.gameObject.AddComponent<StandaloneInputModule>();
        }
#endif
    }
}
