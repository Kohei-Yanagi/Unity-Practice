using UnityEngine;
using UnityEngine.InputSystem;

public class Desktop_Move : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Transform cameraTransform;

    void Update()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
        {
            input.y += 1;
        }

        if (Keyboard.current.sKey.isPressed)
        {
            input.y -= 1;
        }

        if (Keyboard.current.dKey.isPressed)
        {
            input.x += 1;
        }

        if (Keyboard.current.aKey.isPressed)
        {
            input.x -= 1;
        }

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 move = forward * input.y + right * input.x;

        if (move.sqrMagnitude > 1f)
        {
            move.Normalize();
        }

        transform.position += move * moveSpeed * Time.deltaTime;
    }
}