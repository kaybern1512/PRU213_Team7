using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    Rigidbody2D rb;
    Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // đọc input từ bàn phím
        moveInput.x = Keyboard.current.aKey.isPressed ? -1 :
                      Keyboard.current.dKey.isPressed ? 1 : 0;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }
}
