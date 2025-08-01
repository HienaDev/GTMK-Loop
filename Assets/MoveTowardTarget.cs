using UnityEngine;

public class MoveTowardTarget : MonoBehaviour
{
    public Transform target;     // The object to move toward
    public float speed = 3f;     // Movement speed (units per second)

    void Update()
    {
        if (target != null)
        {
            // Move toward the target at constant speed
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                speed * Time.deltaTime
            );
        }
    }
}
