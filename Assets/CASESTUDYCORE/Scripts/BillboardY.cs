using UnityEngine;

public class BillboardY : MonoBehaviour
{
    public bool invertForward = false;
    Camera cam;

    void LateUpdate()
    {
        if (!cam) cam = Camera.main;
        if (!cam) return;

        Vector3 lookPos = cam.transform.position;
        lookPos.y = transform.position.y;
        transform.LookAt(lookPos);

        if (invertForward) transform.Rotate(0f, 180f, 0f);
    }
}
