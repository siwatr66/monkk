public static class SceneFlowState
{
    public const string MainMenuSceneName = "MainMenu";
    public const string GameplaySceneName = "SampleScene";
    public const string GameOverSceneName = "GameOver";
    public const string VictorySceneName = "Victory";
    public const int DefaultEnemiesToWin = 4;

    public static int LastDefeatedEnemies { get; private set; }
    public static int LastEnemiesToWin { get; private set; } = DefaultEnemiesToWin;

    public static void BeginRun(int enemiesToWin)
    {
        LastDefeatedEnemies = 0;
        LastEnemiesToWin = enemiesToWin;
    }

    public static void RecordRun(int defeatedEnemies, int enemiesToWin)
    {
        LastDefeatedEnemies = defeatedEnemies;
        LastEnemiesToWin = enemiesToWin;
    }

    public static void ResetRunInfo()
    {
        LastDefeatedEnemies = 0;
        LastEnemiesToWin = DefaultEnemiesToWin;
    }
}
