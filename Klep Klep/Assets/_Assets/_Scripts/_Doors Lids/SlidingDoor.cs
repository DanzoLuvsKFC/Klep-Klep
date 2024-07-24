using UnityEngine;

public class SlidingDoor : InteractableInterface
{
    [SerializeField] private Transform slideHandle;
    [SerializeField] private float moveSpeed;
    private float currentPosition = 0.0087f;

    [SerializeField, Range(0, 009f)] private float minMove, maxMove;
    [SerializeField, Range(0, 20)] private float lerpTime;

    protected override void Interact()
    {
        var mouseX = CheckPlayerSide.pos;
        currentPosition = (mouseX * moveSpeed) + currentPosition;

        currentPosition = Mathf.Clamp(currentPosition, minMove, maxMove);
        
    }

    private void Update()
    {
        slideHandle.localPosition = new Vector3(Mathf.Lerp(slideHandle.localPosition.x, currentPosition, Time.deltaTime * lerpTime), slideHandle.localPosition.y, slideHandle.localPosition.z);
    }
}
