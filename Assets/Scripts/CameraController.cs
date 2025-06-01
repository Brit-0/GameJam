using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using static Unity.Collections.AllocatorManager;

public class CameraController : MonoBehaviour
{
    public static CameraController main;

    //GENERAL
    [SerializeField] private GameObject buildBlocker, destroyBlocker;

    //DRAG
    private Vector3 originPos, difference, targetPosition;
    private float camWidth, camHeight;
    [SerializeField] private Tilemap tm;
    [SerializeField] private Camera terrainCam;
    
    //ZOOMS
    private float zoomTarget;
    private float multiplier = 2f, minZoom = 1f, maxZoom = 6.85f, smoothTime = .1f;
    private float velocity = 0f;

    //EXPANDING & CONTRACTING
    private Rect ogRect = new(0.587f, 0.03f, 0.4f, 0.5f);
    private Rect expRect = new(0.16f, 0.1f, 0.7f, 0.8f);

    private bool isDragging;

    private void Awake()
    {
        main = this;
    }

    private void Start()
    {
        zoomTarget = terrainCam.orthographicSize;
        tm.CompressBounds();
    }

    public void OnDrag(InputAction.CallbackContext ctx)
    {
        if (ctx.started) originPos = GetMousePosition;
        isDragging = ctx.started || ctx.performed;
    }

    public void ExpandViewport(string purchaseType)
    {
        terrainCam.rect = expRect;

        if (purchaseType == "Build")
        {
            buildBlocker.SetActive(true);
        }
        else if (purchaseType == "Destroy")
        {
            destroyBlocker.SetActive(true);
        }
        
    }

    public void CompressViewport()
    {
        terrainCam.rect = ogRect;
        buildBlocker.SetActive(false);
        destroyBlocker.SetActive(false);
    }

    private void LateUpdate()
    {
        if (!terrainCam.pixelRect.Contains(Input.mousePosition)) return;

        zoomTarget -= Input.GetAxisRaw("Mouse ScrollWheel") * multiplier;
        zoomTarget = Mathf.Clamp(zoomTarget, minZoom, maxZoom);
        terrainCam.orthographicSize = Mathf.SmoothDamp(terrainCam.orthographicSize, zoomTarget, ref velocity, smoothTime);

        if (isDragging)
        {
            difference = GetMousePosition - transform.position;

            targetPosition = originPos - difference;
        }

        targetPosition = GetCameraBounds();
        transform.position = targetPosition;
    }

    private Vector3 GetCameraBounds()
    {
        camHeight = terrainCam.orthographicSize;
        camWidth = camHeight * terrainCam.aspect;

        return new Vector3(Mathf.Clamp(targetPosition.x, tm.localBounds.min.x + camWidth, tm.localBounds.max.x - camWidth),
                           Mathf.Clamp(targetPosition.y, tm.localBounds.min.y + camHeight, tm.localBounds.max.y - camHeight),
                           transform.position.z);
    }

    private Vector3 GetMousePosition => terrainCam.ScreenToWorldPoint((Vector3)Mouse.current.position.ReadValue());
}
