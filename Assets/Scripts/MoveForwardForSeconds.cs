using UnityEngine;

public class MoveForwardForSeconds : MonoBehaviour
{
    public float speed = 15f;         // Movement speed in units/second
    public float duration = 3f;      // Time in seconds to move forward

    private float timer = 0f;
    private bool isMoving = true;

    void Update()
    {
        if (isMoving)
        {
            // Move forward relative to the object's orientation
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            // Count elapsed time
            timer += Time.deltaTime;

            if (timer >= duration)
            {
                isMoving = false; // Stop moving
            }
        }
    }
}
