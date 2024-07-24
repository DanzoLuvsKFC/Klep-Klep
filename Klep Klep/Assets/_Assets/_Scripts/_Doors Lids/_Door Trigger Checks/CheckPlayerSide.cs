using UnityEngine;

public class CheckPlayerSide : MonoBehaviour
{
    [SerializeField] private Vector3 scale;

    public static float pos;
    public static float rot;

    public space state;
    public enum space
    {
        outside,
        inside,
    }

    void Update()
    {
        PlayerSide();
    }

    private void PlayerSide ()
    {
        Collider[] col = Physics.OverlapBox(transform.position, scale);

        foreach (var player in col)
        {
            if (player.tag == "Player" && state == space.outside)
            {
                pos = Input.GetAxis("Mouse X");
                rot = Input.GetAxis("Mouse Y");
            }

            if (player.tag == "Player" && state == space.inside)
            {
                pos = -Input.GetAxis("Mouse X");
                rot = -Input.GetAxis("Mouse Y");
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, scale);
    }
}
