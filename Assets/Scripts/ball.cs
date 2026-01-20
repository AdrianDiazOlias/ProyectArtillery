using UnityEngine;

public class ball : MonoBehaviour
{
    GameManager GMref;

    [SerializeField] float speedThreshold;
    [SerializeField] float delayBeforeDestroy = 2f;
    private float slowTimer = 0f;
    void Start()
    {
        GMref = GameManager.Instance;
    }

    void Update()
    {
        float speed = CheckBallSpeed().magnitude;
        if (speed < speedThreshold)
        {
            slowTimer += Time.deltaTime;
            if (slowTimer >= delayBeforeDestroy)
            {
                Debug.Log("Ball destroyed due to be too slow!");
                GMref.isShootingEnabled = true;
                Destroy(gameObject);
            }
        }
        else
        {
            slowTimer = 0f;
        }
    }

    Vector3 CheckBallSpeed()
    {
        return gameObject.GetComponent<Rigidbody>().linearVelocity;
    }
}
