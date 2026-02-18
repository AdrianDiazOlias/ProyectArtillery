using UnityEngine;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance { get; private set; }
    GameManager GMref;
    SoundManager SMref;

    [Header("Sound sliders")]
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

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

        masterSlider.value = SMref.GetSavedVolume("Master");
        musicSlider.value = SMref.GetSavedVolume("Music");
        sfxSlider.value = SMref.GetSavedVolume("SFX");

        masterSlider.onValueChanged.AddListener(delegate { SMref.SetMaster(masterSlider.value); });
        musicSlider.onValueChanged.AddListener(delegate { SMref.SetMusic(musicSlider.value); });
        sfxSlider.onValueChanged.AddListener(delegate { SMref.SetSFX(sfxSlider.value); });
    }


    void Update()
    {

    }
}
