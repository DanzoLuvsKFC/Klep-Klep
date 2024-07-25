using UnityEngine;

public class BinLid : InteractableInterface
{
    [SerializeField] private bool _iHighLight;

    private Transform lid;
    [SerializeField, Range(0, 10)] private float rotationSpeed = 3f;
    [SerializeField] private float currentAngle = 0;

    [SerializeField, Range (-500, 500)] private float minRot =-190;
    [SerializeField, Range(-500, 500)] private float maxRot =  0;

    //Freefall 
    [SerializeField, Range(0, 50)] private float freefallSpeed = 5f;
    private bool isInteracting = false;

    private void Start()
    {
        lid = transform.parent;
    }

    protected override void Interact()
    {
        isInteracting = true;
        var mouseX = -Input.GetAxis("Mouse Y");
        var mouseDelta = (mouseX * rotationSpeed) + currentAngle;

        var newAngle = Mathf.Clamp(mouseDelta, minRot, maxRot);
        var clampedAngle = newAngle - currentAngle;
        lid.RotateAround(lid.position, Vector3.right, clampedAngle);
        currentAngle = newAngle;
    }

    protected override void IHighlight()
    {

    }

    private void Update()
    {
        if (!isInteracting && currentAngle!= 0)
        {
            LidFreefall();
        }

        if (Input.GetMouseButtonUp(0))
        {
            isInteracting = false;
        }
    }

    private void LidFreefall()
    {
        if (currentAngle >= -90)
        {
            currentAngle = Mathf.Lerp(currentAngle, 0, Mathf.SmoothStep(0, 1, Time.deltaTime * freefallSpeed));
            lid.localRotation = Quaternion.Euler(new Vector3(currentAngle, lid.localRotation.eulerAngles.y, lid.localRotation.eulerAngles.z));
        }

        if (Mathf.Abs(currentAngle) < 1)
        {
            currentAngle = 0;
        }
    }
}
