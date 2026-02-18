using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameObject BallInScene;

    Transform CamTransform;
    Camera CamCamera;

    [SerializeField] float camSmoothSpeed = 0.125f;
    [SerializeField] float maxZoomIn = 3f;
    [SerializeField] float maxZoomOut = 10f;

    void Start()
    {
        CamTransform = this.transform;
        CamCamera = Camera.main;

    }

    void Update()
    {
        if (BallInScene == null) BallInScene = FindBallInScene();

        if (BallInScene != null)
        {
            FollowBall();
            ZoomIn();
        }
        else
        {
            ResetCamPos();
            ZoomOut();
        }
    }

    GameObject FindBallInScene()
    {
        BallInScene = GameObject.FindWithTag("CannonBall");
        return BallInScene;
    }

    void FollowBall()
    {
        Vector3 ballPosition = new Vector3(BallInScene.transform.position.x, BallInScene.transform.position.y, CamTransform.position.z);
        float ballSpeed = BallInScene.GetComponent<Rigidbody>().linearVelocity.magnitude;
        CamTransform.position = Vector3.Lerp(CamTransform.position, ballPosition, camSmoothSpeed * (ballSpeed * 0.5f) * Time.deltaTime);
    }

    void ResetCamPos()
    {
        Vector3 defaultPosition = new Vector3(0, 0, CamTransform.position.z);
        CamTransform.position = Vector3.Lerp(CamTransform.position, defaultPosition, camSmoothSpeed * Time.deltaTime);
    }

    void ZoomIn()
    {
        if (CamCamera.orthographicSize <= maxZoomIn) return;
        float actualZoom = Mathf.Lerp(CamCamera.orthographicSize, maxZoomIn, camSmoothSpeed * Time.deltaTime);
        CamCamera.orthographicSize = actualZoom;
    }

    void ZoomOut()
    {
        if (CamCamera.orthographicSize >= maxZoomOut) return;
        float actualZoom = Mathf.Lerp(CamCamera.orthographicSize, maxZoomOut, camSmoothSpeed * Time.deltaTime);
        CamCamera.orthographicSize = actualZoom;
    }


}
