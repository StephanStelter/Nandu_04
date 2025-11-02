using Unity.VisualScripting;
using UnityEngine;
using static MyEventHandler;

public class CameraMovement : MonoBehaviour
{
    public static CameraMovement Instance { get; set; }

    public float moveSpeed = 1000f;
    public float rotateSpeed = 100f;
    public float minZoom = -1500f;
    public float maxZoom = 600f;
    public float zoomSpeed = 1000f;

    public float currentZoom;

    private Vector3 targetFollowOffset;
    public Vector3 targetOffest;
    public Vector3 rotation;
    public bool startFocus = false;
    private Vector3 newTargetFollowOffset;
    public float height;
    public float lowHeight;
    public int heightChange;
    private Quaternion saveLastQuaternion;

    public MyEventHandler eventHandler;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Initialize follow offset with current camera position
        targetFollowOffset = transform.position;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
        CameraFokus();
    }

    private void CameraFokus()
    {
        // Standard Kameraausrichtung
        if (!startFocus) { transform.position = Vector3.Lerp(transform.position, targetFollowOffset, Time.deltaTime * zoomSpeed); }

        currentZoom = transform.position.y;

        // angeklickte Karte fokusieren
        if (startFocus)
        {
            // Karte gerade anzeigen
            transform.rotation = Quaternion.Euler(0, 90, 0);

            // Karte fokusieren
            transform.position = Vector3.Lerp(transform.position, newTargetFollowOffset, Time.deltaTime * zoomSpeed);
        }
    }

    // Camera movement (left-right, up-down)
    private void HandleMovement()
    {
        Vector3 inputMoveDir = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            inputMoveDir.z = +1f;
        }

        if (Input.GetKey(KeyCode.S))
        {
            inputMoveDir.z = -1f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            inputMoveDir.x = -1f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            inputMoveDir.x = +1f;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            inputMoveDir.y = -1f;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            inputMoveDir.y = +1f;
        }

        Vector3 moveVector = transform.forward * inputMoveDir.z + transform.right * inputMoveDir.x + transform.up * inputMoveDir.y;

        // Update the target position instead of the actual position
        targetFollowOffset += moveVector * moveSpeed * Time.deltaTime;
    }

    // Camera rotation
    private void HandleRotation()
    {
        // Mausrotation
        if (Input.GetMouseButton(1) && eventHandler.gameMode == GameMode.Default)
        {
            float mouseX = Input.GetAxis("Mouse X") * rotateSpeed * 3 * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * rotateSpeed * 3 * Time.deltaTime;

            rotation.y += mouseX;
            rotation.x -= mouseY;
        }

        // Tastengesteuerte Rotation (Q/E → Yaw)
        if (Input.GetKey(KeyCode.E))
        {
            rotation.y += rotateSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            rotation.y -= rotateSpeed * Time.deltaTime;
        }

        // Vertikale Rotation begrenzen
        rotation.x = Mathf.Clamp(rotation.x, -89f, 89f);

        // Anwendung der Rotation (Z = 0 bleibt stabil)
        transform.rotation = Quaternion.Euler(rotation.x, rotation.y, 0f);
    }

    // Camera zoom
    private void HandleZoom()
    {
        float zoomAmount = 100f;

        if (Input.mouseScrollDelta.y > 0)
        {
            targetFollowOffset.y -= zoomAmount;
        }

        if (Input.mouseScrollDelta.y < 0)
        {
            targetFollowOffset.y += zoomAmount;
        }

        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, minZoom, maxZoom);
    }

    // Camera fokus
    public void FokusPosition(GameObject focusGameobject)
    {
        // Rotation speichern
        saveLastQuaternion = transform.rotation;

        newTargetFollowOffset = focusGameobject.transform.position + targetOffest;
        if (focusGameobject.transform.position.y > heightChange)
        {
            newTargetFollowOffset = new Vector3(newTargetFollowOffset.x, newTargetFollowOffset.y + height, newTargetFollowOffset.z);
        }
        else
        {
            newTargetFollowOffset = new Vector3(newTargetFollowOffset.x, newTargetFollowOffset.y + lowHeight, newTargetFollowOffset.z);
        }
        startFocus = true;
    }

    public void SetLastRotation()
    {
        transform.rotation = saveLastQuaternion;
    }
}