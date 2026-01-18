using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Cannon Settings")]
    [SerializeField] GameObject CannonBallPrefab;
    [SerializeField] int CannonAmmo;

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
    }


    void Start()
    {
        FindCoresInScene();
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
}
