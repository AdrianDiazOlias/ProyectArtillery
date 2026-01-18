using UnityEngine;

public class Core : MonoBehaviour
{
    GameManager GMref;
    void Start()
    {
        GMref = GameManager.Instance;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("CannonBall"))
        {
            Debug.Log("Core hit by CannonBall!");
            GMref.DestroyCore(gameObject);
        }
    }

}
