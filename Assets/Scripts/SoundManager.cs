using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private AudioMixer masterMixer;

    [Header("Exposed Parameter Names")]
    [SerializeField] private string masterParam = "Master";
    [SerializeField] private string musicParam = "Music";
    [SerializeField] private string sfxParam = "SFX";

    private const float defaultVolume = 0.80f; // linear 0..1

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
        float master = PlayerPrefs.GetFloat("Master", defaultVolume);
        float music = PlayerPrefs.GetFloat("Music", defaultVolume);
        float sfx = PlayerPrefs.GetFloat("SFX", defaultVolume);

        SetMaster(master, save: false);
        SetMusic(music, save: false);
        SetSFX(sfx, save: false);
    }

    public void SetMaster(float linear, bool save = true)
    {
        SetMixerVolume(masterParam, linear);
        if (save) PlayerPrefs.SetFloat("Master", Mathf.Clamp01(linear));
    }

    public void SetMusic(float linear, bool save = true)
    {
        SetMixerVolume(musicParam, linear);
        if (save) PlayerPrefs.SetFloat("Music", Mathf.Clamp01(linear));
    }

    public void SetSFX(float linear, bool save = true)
    {
        SetMixerVolume(sfxParam, linear);
        if (save) PlayerPrefs.SetFloat("SFX", Mathf.Clamp01(linear));
    }

    public float GetMaster()
    {
        return GetMixerVolumeLinear(masterParam);
    }

    public float GetMusic()
    {
        return GetMixerVolumeLinear(musicParam);
    }

    public float GetSFX()
    {
        return GetMixerVolumeLinear(sfxParam);
    }

    public float GetSavedVolume(string savedParam)
    {
        return PlayerPrefs.GetFloat(savedParam, defaultVolume);
    }

    public void ResetToDefaults()
    {
        SetMaster(defaultVolume);
        SetMusic(defaultVolume);
        SetSFX(defaultVolume);
    }

    private void SetMixerVolume(string exposedName, float linear)
    {
        if (masterMixer == null) return;
        float dB = LinearToDecibel(Mathf.Clamp01(linear));
        masterMixer.SetFloat(exposedName, dB);
    }

    private float GetMixerVolumeLinear(string exposedName)
    {
        if (masterMixer == null) return defaultVolume;
        if (masterMixer.GetFloat(exposedName, out float dB))
        {
            return DecibelToLinear(dB);
        }
        return defaultVolume;
    }

    private static float LinearToDecibel(float linear)
    {
        if (linear <= 0.0001f) return -80f;
        return 20f * Mathf.Log10(linear);
    }

    private static float DecibelToLinear(float dB)
    {
        return Mathf.Pow(10f, dB / 20f);
    }
}
