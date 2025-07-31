using UnityEngine;

public class MoveForwardForSeconds : MonoBehaviour
{
    public float speed = 15f;
    public float duration = 3f;

    private float timer = 0f;
    private bool isMoving = true;

    public Vector3 moveDirection = Vector3.forward; // Set externally


    void Update()
    {
        if (isMoving)
        {
            transform.Translate(moveDirection.normalized * speed * Time.deltaTime, Space.World);
            timer += Time.deltaTime;
            if (timer >= duration)
            {
                isMoving = false;
            }
        }
    }
}
