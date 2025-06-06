using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
    public static CameraController main;

    //GENERAL
    [SerializeField] private GameObject buildBlocker, destroyBlocker, chopBlocker;

    //DRAG
    private Vector3 originPos, difference, targetPosition;
    private float camWidth, camHeight;
    [SerializeField] private Tilemap tm;
    private Camera terrainCam;
    
    //ZOOMS
    private float zoomTarget;
    private float multiplier = 2f, minZoom = 1f, maxZoom = 6.7f, smoothTime = .1f;
    private float velocity = 0f;

    //EXPANDING & CONTRACTING
    private Rect ogRect = new(0.51f, 0.15f, 0.47f, 0.65f);
    private Rect expRect = new(0.16f, 0.1f, 0.7f, 0.8f);
    private Rect zeroRect = Rect.zero;

    //REFERENCOAS
    [SerializeField] GameObject capitalist;

    private bool isDragging;

    private void Awake()
    {
        main = this;
        terrainCam = GetComponent<Camera>();
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

    public IEnumerator ExpandViewport(string purchaseType, float delay)
    {
        yield return new WaitForSeconds(delay);

        capitalist.SetActive(false);

        terrainCam.rect = expRect;

        if (purchaseType == "Build")
        {
            buildBlocker.SetActive(true);
        }
        else if (purchaseType == "Destroy")
        {
            destroyBlocker.SetActive(true);
        }
        else if (purchaseType == "Chop")
        {
            chopBlocker.SetActive(true);
        }
        
    }

    public void CompressViewport()
    {
        capitalist.SetActive(true);
        terrainCam.rect = ogRect;
        buildBlocker.SetActive(false);
        destroyBlocker.SetActive(false);
        chopBlocker.SetActive(false);
    }

    public void ZeroViewport()
    {
        terrainCam.gameObject.SetActive(false);
        terrainCam.rect = zeroRect;
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

    //Mathf.Clamp(targetPosition.y, tm.localBounds.min.y + camHeight, tm.localBounds.max.y - camHeight)
    private Vector3 GetMousePosition => terrainCam.ScreenToWorldPoint((Vector3)Mouse.current.position.ReadValue());
}
