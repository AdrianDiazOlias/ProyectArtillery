using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    ScreenManager ScreenMref;
    SceneManager SceneMref;
    GameControls gameControls;

    InputAction TogglePauseAction;

    [Header("Cannon Settings")]
    [SerializeField] GameObject CannonBallPrefab;
    [SerializeField] public int CannonAmmo { get; private set; } = 3;
    [SerializeField] public bool isShootingEnabled = true;

    [Header("Core Settings")]
    [SerializeField] GameObject[] coreCount;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        gameControls = new GameControls();
    }
    private void OnEnable()
    {
        TogglePauseAction = gameControls.ScreenControls.TogglePause;
        TogglePauseAction.performed += ctx => ScreenMref.TogglePauseMenu(ScreenMref.IsPauseMenuActive() ? false : true);
        TogglePauseAction.Enable();
    }

    void Start()
    {
        ScreenMref = ScreenManager.Instance;
        SceneMref = SceneManager.Instance;

        SceneMref.LoadScene(0);
        Invoke(nameof(FindCoresInScene), 0.1f);
    }

    public GameObject SpawnBall(Vector3 spawnPosition)
    {
        if (CannonBallPrefab != null)
        {
            GameObject cannonBall = Instantiate(CannonBallPrefab, spawnPosition, Quaternion.identity);
            return cannonBall;
        }
        else
        {
            Debug.LogError("CannonBallPrefab is not assigned in GameManager!");
            return null;
        }
    }

    public void UseAmmo()
    {
        if (CannonAmmo > 0)
        {
            CannonAmmo--;
            ScreenMref.UpdateAmmoText();
            Debug.Log("Ammo used. Remaining ammo: " + CannonAmmo);
        }
        else
        {
            Debug.Log("No ammo left!");
        }
    }

    public int CheckAmmo()
    {
        return CannonAmmo;
    }

    public void FindCoresInScene()
    {
        coreCount = GameObject.FindGameObjectsWithTag("Core");
        Debug.Log("Cores in scene: " + coreCount.Length);
        if (coreCount.Length == 0)
        {
            ScreenMref.ToggleWinScreen(true);
            Debug.Log("All cores destroyed! You win!");
        }
    }

    public void DestroyCore(GameObject core)
    {
        for (int i = 0; i < coreCount.Length; i++)
        {
            if (coreCount[i] == core)
            {
                coreCount[i] = null;
                Destroy(core);
                Debug.Log("Core destroyed!");
                Invoke(nameof(FindCoresInScene), 0.1f);
                break;
            }
        }
    }

    public void ResetCannon()
    {
        CannonAmmo = 3;
        ScreenMref.UpdateAmmoText();
        isShootingEnabled = true;
    }

    public void ResumeGame()
    {
        ScreenMref.GetAllScreensOff();
        ScreenMref.TogglePauseMenu(false);
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        ScreenMref.GetAllScreensOff();
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        Invoke(nameof(FindCoresInScene), 0.1f);
        ResetCannon();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GameOver()
    {
        ScreenMref.GetAllScreensOff();
        ScreenMref.ToggleGameOverScreen(true);
    }
}

