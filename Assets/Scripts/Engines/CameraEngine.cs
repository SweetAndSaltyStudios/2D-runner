using UnityEngine;

public class CameraEngine : Singelton<CameraEngine>
{
    private Transform target;

    //private Vector2 startPosition = new Vector2(, 0);
    private Camera mainCamera;

    private float othographicSize;

    public float WorldRightEdgePosition
    {
        get
        {
            return transform.position.x + othographicSize;
        }
    }

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void Start()
    {
        //transform.position = startPosition;
        othographicSize = mainCamera.orthographicSize;
    }

    public void ChangeTarget(Transform newTarget) 
    {
        target = newTarget;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        transform.position = new Vector3( target.position.x + 4, 0, 0);
    }
}
