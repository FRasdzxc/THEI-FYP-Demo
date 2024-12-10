using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float dragSpeed = 2;
    public float scrollSpeed = 5;
    public float minZoom = 5f;
    public float maxZoom = 20f;

    private Vector3 dragOrigin;

    void Update()
    {
        HandleDragging();
        HandleZooming();
    }

    void HandleDragging()
    {
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(1)) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0);

        transform.Translate(move, Space.World);
    }

    void HandleZooming()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            float zoomAmount = Camera.main.orthographicSize - scroll * scrollSpeed;
            Camera.main.orthographicSize = Mathf.Clamp(zoomAmount, minZoom, maxZoom);
        }
    }
}
