using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

public static class SceneMenuGenerator
{
    [MenuItem("Tools/Generate Menu Scenes")]
    public static void GenerateMenuScenes()
    {
        EnsureFolder("Assets/Editor");
        EnsureFolder("Assets/Scenes");

        CreateMenuScene("Assets/Scenes/MainMenu.unity", "MAIN MENU", "Defeat 4 monsters to win.", "Start");
        CreateMenuScene("Assets/Scenes/GameOver.unity", "GAME OVER", "You defeated 0/4 monsters.", "Restart");
        CreateMenuScene("Assets/Scenes/Victory.unity", "VICTORY", "You defeated 4/4 monsters.", "Restart");

        EnsureGameplayManager();
        UpdateBuildSettings();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void CreateMenuScene(string scenePath, string titleValue, string messageValue, string buttonLabelValue)
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var font = LoadRuntimeFont();

        CreateCamera();

        var eventSystemObject = new GameObject("EventSystem", typeof(EventSystem));
#if ENABLE_INPUT_SYSTEM
        eventSystemObject.AddComponent<InputSystemUIInputModule>();
#else
        eventSystemObject.AddComponent<StandaloneInputModule>();
#endif

        var controllerObject = new GameObject("SceneMenuController");
        var controller = controllerObject.AddComponent<SceneMenuController>();

        var canvasObject = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.pixelPerfect = false;

        var scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        var background = CreateUiImage("Background", canvasObject.transform, new Color(0.05f, 0.07f, 0.12f, 1f));
        StretchToParent(background.rectTransform);

        var title = CreateUiText("Title", canvasObject.transform, font, 68, FontStyle.Bold, Color.white);
        SetupRect(title.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 140f), new Vector2(900f, 100f));
        title.text = titleValue;

        var message = CreateUiText("Message", canvasObject.transform, font, 28, FontStyle.Normal, new Color(0.87f, 0.9f, 0.95f, 1f));
        SetupRect(message.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 35f), new Vector2(960f, 120f));
        message.text = messageValue;

        var buttonObject = new GameObject("PrimaryButton", typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(canvasObject.transform, false);

        var buttonRect = buttonObject.GetComponent<RectTransform>();
        SetupRect(buttonRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -130f), new Vector2(320f, 84f));

        var buttonImage = buttonObject.GetComponent<Image>();
        buttonImage.color = new Color(0.96f, 0.75f, 0.18f, 1f);

        var button = buttonObject.GetComponent<Button>();
        var colors = button.colors;
        colors.normalColor = buttonImage.color;
        colors.highlightedColor = buttonImage.color * 1.08f;
        colors.selectedColor = colors.highlightedColor;
        colors.pressedColor = buttonImage.color * 0.92f;
        colors.disabledColor = new Color(0.4f, 0.4f, 0.4f, 0.7f);
        button.colors = colors;

        var buttonLabel = CreateUiText("Label", buttonObject.transform, font, 28, FontStyle.Bold, new Color(0.08f, 0.08f, 0.08f, 1f));
        StretchToParent(buttonLabel.rectTransform);
        buttonLabel.text = buttonLabelValue;

        controller.BindUi(title, message, button, buttonLabel);

        EditorSceneManager.SaveScene(scene, scenePath);
    }

    private static void EnsureGameplayManager()
    {
        const string sampleScenePath = "Assets/Scenes/SampleScene.unity";
        var scene = EditorSceneManager.OpenScene(sampleScenePath, OpenSceneMode.Single);
        var spawnerObject = GameObject.Find("EnemySpawnerManager");
        if (spawnerObject != null && spawnerObject.GetComponent<GameManager>() == null)
        {
            spawnerObject.AddComponent<GameManager>();
            EditorSceneManager.SaveScene(scene);
        }
    }

    private static void UpdateBuildSettings()
    {
        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/SampleScene.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/GameOver.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/Victory.unity", true)
        };
    }

    private static void CreateCamera()
    {
        var cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        cameraObject.tag = "MainCamera";

        var camera = cameraObject.GetComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.05f, 0.07f, 0.12f, 1f);
        camera.orthographic = true;
        camera.orthographicSize = 5f;

        cameraObject.transform.position = new Vector3(0f, 0f, -10f);
    }

    private static Font LoadRuntimeFont()
    {
        var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        return font != null ? font : Resources.GetBuiltinResource<Font>("Arial.ttf");
    }

    private static Image CreateUiImage(string objectName, Transform parent, Color color)
    {
        var imageObject = new GameObject(objectName, typeof(RectTransform), typeof(Image));
        imageObject.transform.SetParent(parent, false);

        var image = imageObject.GetComponent<Image>();
        image.color = color;
        return image;
    }

    private static Text CreateUiText(string objectName, Transform parent, Font font, int fontSize, FontStyle fontStyle, Color color)
    {
        var textObject = new GameObject(objectName, typeof(RectTransform), typeof(Text));
        textObject.transform.SetParent(parent, false);

        var text = textObject.GetComponent<Text>();
        text.font = font;
        text.fontSize = fontSize;
        text.fontStyle = fontStyle;
        text.color = color;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        return text;
    }

    private static void StretchToParent(RectTransform rectTransform)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.anchoredPosition = Vector2.zero;
    }

    private static void SetupRect(
        RectTransform rectTransform,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 pivot,
        Vector2 anchoredPosition,
        Vector2 sizeDelta)
    {
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.pivot = pivot;
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = sizeDelta;
        rectTransform.localScale = Vector3.one;
    }

    private static void EnsureFolder(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            string parent = Path.GetDirectoryName(path)?.Replace("\\", "/");
            string name = Path.GetFileName(path);
            if (!string.IsNullOrEmpty(parent) && !string.IsNullOrEmpty(name))
            {
                AssetDatabase.CreateFolder(parent, name);
            }
        }
    }
}
