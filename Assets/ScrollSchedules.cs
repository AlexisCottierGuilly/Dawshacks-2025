using UnityEngine;

public class ScrollSchedules : MonoBehaviour
{
    // scroll = move this object

    float scrollSpeed = 0.5f; // Speed of scrolling

    void Update()
    {
        // Get the mouse scroll wheel input
        float scrollInput = Input.GetAxis("Mouse ScrollWheel") * -500f;

        // Calculate the new position based on the scroll input and speed
        Vector3 newPosition = transform.position + new Vector3(0, scrollInput * scrollSpeed, 0);

        // Set the new position of the object
        transform.position = newPosition;
    }
}
