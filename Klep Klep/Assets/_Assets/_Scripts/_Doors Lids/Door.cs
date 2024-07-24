using UnityEngine;

public class Door : InteractableInterface
{
    [SerializeField] private Transform _door;

    [SerializeField, Range (0, 50)] private float rotationSpeed;
    [SerializeField, Range(-100, 100)] private float minRot, maxRot;
    [SerializeField] private float currentAngle;

    [SerializeField, Range(0, 10)] private float lerpTime;

    protected override void Interact ()
    {
        if (_door == null) return;
        var mouseY = CheckPlayerSide.rot;
        currentAngle = (mouseY * rotationSpeed) + currentAngle;

        currentAngle = Mathf.Clamp (currentAngle, minRot, maxRot);
    }

    private void Update ()
    {
        float targetAngle = Mathf.Lerp(_door.localEulerAngles.y, currentAngle, Time.deltaTime * lerpTime);
        _door.localRotation = Quaternion.Euler(0, targetAngle, 0);
    }
}
