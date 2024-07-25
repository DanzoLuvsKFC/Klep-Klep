using UnityEngine;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviour
{
    //Raycasts
    [SerializeField] private LayerMask interactableMask;
    [SerializeField, Range(0, 10f)] private float interactRange;
    private Transform raySource;

    private bool canInteract;
    private InteractableInterface interactableInterface;

    #region Pick/ Throw/ Rotate
    public static bool canLook;

    //pick/ drop
    private Transform heldObject;
    private Rigidbody heldObjectRb;
    [SerializeField] private Transform holdPos;
    private bool isHeld;
    [SerializeField, Range(0, 10)] private float pickObjectLerpTime;
    [SerializeField, Range(0, 100f)] private float rotationSpeed;

    //Zoom  (FieldOfView)
    [SerializeField, Range(30f, 45)] private float minFieldOfView;
    [SerializeField, Range(45, 60f)] private float maxFieldOfView;
    private Camera cam;

    //Throw
    [SerializeField, Range(0, 100f)] private float throwForce;
    #endregion

    //Light Control system (Home Automation)
    private bool isOn;
    private AudioSource clapSFx;
    private Light _light;

    //highlight
    public static bool _isHighLighted;
    
    private GameObject _highLightObject;
    private GameObject _previousHighLightedObject;

    //HandGestures (Part of UI)
    [SerializeField] private Image _handImage;
    [SerializeField] private Sprite _openHand;
    [SerializeField] private Sprite _closedHand;
    private CanvasGroup _canvasGroup;

    void Start()
    {
        raySource = Camera.main.transform;
        _canvasGroup  = _handImage.GetComponent<CanvasGroup>();
        cam = Camera.main;
        isHeld = false;
    }

    void Update()
    {
        ManageBooleans();
        FindInteractableObjects();
        DoorsEtcetera();
        PickRotateThrow();
        HighlightGameObjects();
    }

    private void ManageBooleans()
    {
        canLook = Input.GetKey(KeyCode.R) ? false : true;
        if (Input.GetMouseButtonUp(0))
        {
            isHeld = false;
            canInteract = false;
        }

        if (_handImage == null || _openHand == null || _closedHand == null) return;
        if (!_isHighLighted) { _canvasGroup.alpha = 0; }
        else
        {
            _canvasGroup.alpha = 1;
        }

        if (_isHighLighted)
        {
            _handImage.sprite = _openHand;
        }
       if (_isHighLighted && (canInteract || isHeld))
        {
            _handImage.sprite = _closedHand;
        }
    }

    private void HighlightGameObjects ()
    {
        Ray ray = new Ray(raySource.position, raySource.forward);
        Debug.DrawRay(ray.origin, ray.direction * interactRange, Color.green);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            if (hit.collider.TryGetComponent (out InteractableInterface interactable))
            {
                if (_previousHighLightedObject != null && _previousHighLightedObject != interactable._highlight)
                {
                    _previousHighLightedObject.SetActive(false);
                }

                if (interactable._highlight != null)
                {
                    _highLightObject = interactable._highlight.gameObject;
                    _highLightObject.SetActive(true);
                    _isHighLighted = true;
                    _previousHighLightedObject = _highLightObject;
                }
                else
                {
                    _highLightObject?.SetActive(false);
                    _isHighLighted = false;
                }
            }
            else
            {
                _highLightObject?.SetActive(false);
                _isHighLighted = false;
            }
        }
    }

    private void FindInteractableObjects()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = new Ray(raySource.position, raySource.forward);
            Debug.DrawRay(ray.origin, ray.direction * interactRange, Color.green);

            if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableMask))
            {
                if (hit.collider.TryGetComponent(out InteractableInterface interactable))
                {
                    interactableInterface = interactable;
                    canInteract = true;
                }

                if (hit.collider.tag == "Pickables")
                {
                    isHeld = true;
                    heldObject = hit.collider.gameObject.transform;
                    heldObjectRb = heldObject.GetComponent<Rigidbody>();
                }
            }
        }
    }

    private void DoorsEtcetera()
    {
        if (canInteract)
        {
            interactableInterface.BaseInteract();
        }
    }

    private void PickRotateThrow()
    {
        if (isHeld && heldObject != null)
        {
            heldObject.position = Vector3.Lerp(heldObject.position, holdPos.position, Time.deltaTime * pickObjectLerpTime);
            heldObjectRb.velocity = Vector3.zero;
            heldObject.SetParent(holdPos);

            heldObjectRb.useGravity = false;
            heldObjectRb.angularDrag = 1.5f;

            if (Input.GetKey(KeyCode.R))
            {
                var mouseX = Input.GetAxis("Mouse X");
                var mouseY = Input.GetAxis("Mouse Y");

                var mouseDeltaX = mouseX * rotationSpeed;
                var mouseDeltaY = mouseY * rotationSpeed;

                heldObject.RotateAround(heldObject.position, Vector3.up, mouseDeltaX);
                heldObject.RotateAround(heldObject.position, Vector3.forward, mouseDeltaY);
            }

            //zoom
            if (Input.mouseScrollDelta != Vector2.zero)
            {
                Vector2 scrollDelta = Input.mouseScrollDelta;
                var scrollAmount = scrollDelta.y;

                cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minFieldOfView, maxFieldOfView);
                cam.fieldOfView -= scrollAmount;
            }
        }
        else if (!isHeld && heldObject != null)
        {
            heldObject.parent = null;
            heldObjectRb.useGravity = true;
            heldObjectRb.angularDrag = 0.05f;
            cam.fieldOfView = maxFieldOfView;
        }

        if (heldObject != null)
        {
            if (Input.GetMouseButtonDown(1))
            {
                isHeld = false;
                heldObjectRb.AddForce(raySource.forward * throwForce, ForceMode.Impulse);
            }
        }
    }
}
