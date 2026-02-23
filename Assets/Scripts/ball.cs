using System.Collections;
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
        Vector3 ballPos = transform.position;
        float speed = CheckBallSpeed().magnitude;
        if (speed < speedThreshold || ballPos.y < -10f)
        {
            slowTimer += Time.deltaTime;
            if (slowTimer >= delayBeforeDestroy)
            {
                Debug.Log("Ball destroyed due to be too slow!");
                if (GMref.CheckAmmo() == 0)
                {
                    GMref.GameOver();
                }
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

    void OnCollisionEnter(Collision collision)
    {

    }
}
