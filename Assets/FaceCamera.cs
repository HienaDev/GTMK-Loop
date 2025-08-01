using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCamera != null)
        {
            // Get direction from this object to the camera
            Vector3 direction = mainCamera.transform.position - transform.position;

            // Calculate rotation that looks at the camera
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            // Extract only the X rotation from that rotation
            Vector3 euler = lookRotation.eulerAngles;

            // Keep only the X rotation, keep current Y and Z rotation
            transform.rotation = Quaternion.Euler(euler.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }
    }
}
