using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance { get; private set; }
    GameManager GMref;
    SoundManager SMref;

    [Header("Menu Screens")]
    GameObject[] ScreensGame;
    [SerializeField] GameObject PauseMenu;
    [SerializeField] GameObject GameOverScreen;
    [SerializeField] GameObject WinScreen;

    [Header("Sound sliders")]
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    [Header("Others")]
    [SerializeField] Text ammoText;

    void Awake()
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
    }

    void Start()
    {
        GMref = GameManager.Instance;
        SMref = SoundManager.Instance;

        ScreensGame = new GameObject[] { PauseMenu, GameOverScreen, WinScreen };

        masterSlider.value = SMref.GetSavedVolume("Master");
        musicSlider.value = SMref.GetSavedVolume("Music");
        sfxSlider.value = SMref.GetSavedVolume("SFX");

        masterSlider.onValueChanged.AddListener(delegate { SMref.SetMaster(masterSlider.value); });
        musicSlider.onValueChanged.AddListener(delegate { SMref.SetMusic(musicSlider.value); });
        sfxSlider.onValueChanged.AddListener(delegate { SMref.SetSFX(sfxSlider.value); });

        UpdateAmmoText();
    }

    public void UpdateAmmoText()
    {
        ammoText.text = "Ammo: " + GMref.CannonAmmo;
    }

    public void GetAllScreensOff()
    {
        foreach (GameObject screen in ScreensGame)
        {
            screen.SetActive(false);
        }
    }

    public bool IsPauseMenuActive()
    {
        return PauseMenu.activeSelf;
    }

    public void TogglePauseMenu(bool isPaused)
    {
        PauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }


    public bool IsGameOverScreenActive()
    {
        return GameOverScreen.activeSelf;
    }

    public void ToggleGameOverScreen(bool isGameOver)
    {
        GameOverScreen.SetActive(isGameOver);
        Time.timeScale = isGameOver ? 0 : 1;
    }


    public bool IsWinScreenActive()
    {
        return WinScreen.activeSelf;
    }

    public void ToggleWinScreen(bool isWinScreen)
    {
        WinScreen.SetActive(isWinScreen);
        Time.timeScale = isWinScreen ? 0 : 1;
    }

}
