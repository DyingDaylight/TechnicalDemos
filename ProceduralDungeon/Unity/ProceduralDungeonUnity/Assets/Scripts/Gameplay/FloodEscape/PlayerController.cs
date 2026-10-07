using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    
    private Rigidbody2D body;
    private Vector2 moveInput;
    private InputAction moveAction;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        moveAction = InputSystem.actions.FindAction("Player/Move", true);
    }

    private void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
    }
    
    private void FixedUpdate()
    {
        body.linearVelocity = moveInput * moveSpeed;
    }
}
