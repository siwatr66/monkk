// GameManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Level Settings")]
    [SerializeField] private string nextSceneName; // ปรับเป็น SerializeField ตามมาตรฐาน Unity 6 เพื่อความปลอดภัยของข้อมูล

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayerDied()
    {
        Debug.Log("Player Died. Restarting Level...");
        // Unity 6 แนะนำให้เคลียร์ Garbage Collection เล็กน้อยก่อนโหลดซีนใหม่ในโมบายเพื่อลดการกระตุก
        System.GC.Collect();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LevelCleared()
    {
        Debug.Log("Level Cleared!");
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.SaveGameData();
        }

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.Log("No next scene defined. You win the game!");
        }
    }
}