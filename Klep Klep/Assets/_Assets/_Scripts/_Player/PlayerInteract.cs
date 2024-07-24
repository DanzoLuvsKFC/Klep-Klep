using UnityEngine;

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

    //Throw
    [SerializeField, Range(0, 100f)] private float throwForce;
    #endregion

    //Light Control system (Home Automation)
    private bool isOn;
    private AudioSource clapSFx;
    private Light _light;

    void Start()
    {
        raySource = Camera.main.transform;

        isHeld = false;
    }

    void Update()
    {
        ManageBooleans();
        FindInteractableObjects();
        DoorsEtcetera();
        PickRotateThrow();
    }

    private void ManageBooleans()
    {
        canLook = Input.GetKey(KeyCode.R) ? false : true;

        if (Input.GetMouseButtonUp(0))
        {
            isHeld = false;
            canInteract = false;
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
        }
        else if (!isHeld && heldObject != null)
        {
            heldObject.parent = null;
            heldObjectRb.useGravity = true;
            heldObjectRb.angularDrag = 0.05f;
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
