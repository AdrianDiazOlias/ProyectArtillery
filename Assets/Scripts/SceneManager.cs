using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{

    public static SceneManager Instance { get; private set; }

    GameManager GameMref;
    ScreenManager ScreenMref;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        GameMref = GameManager.Instance;
        ScreenMref = ScreenManager.Instance;
    }

    public void LoadScene(int buildIndex)
    {
        Debug.Log("Loading scene " + buildIndex);
        UnityEngine.SceneManagement.SceneManager.LoadScene(buildIndex);
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        GameMref.FindCoresInScene();
        ScreenMref.GetAllScreensOff();
        GameMref.ResetCannon();
        GameMref.ResumeGame();
    }

    public void ReloadCurrentScene()
    {
        var active = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        UnityEngine.SceneManagement.SceneManager.LoadScene(active.buildIndex);
    }

    public void LoadNextScene()
    {
        int currentIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        int count = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings - 1;
        int nextIndex = (currentIndex + 1) % count;
        LoadScene(nextIndex);
    }
}

