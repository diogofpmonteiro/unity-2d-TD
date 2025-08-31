using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public LevelData[] allLevels;
    public LevelData CurrentLevel { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        int currentIndex = SceneManager.GetActiveScene().buildIndex - 1; // -1 to account for MainMenu scene
        CurrentLevel = allLevels[currentIndex]; // default to scene index
    }

    public void LoadLevel(LevelData levelData)
    {
        CurrentLevel = levelData;
        SceneManager.LoadScene(levelData.levelName);
    }
}
