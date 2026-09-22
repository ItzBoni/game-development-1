using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed = 8f;

    public Transform groundCheck;
    public float groundRadius = 0.15f;
    public LayerMask groundLayer;
    public float airControl = 0.5f;
    public float jumpSpeed = 14f;

    Rigidbody2D rb;
    Vector2 moveInput;
    bool grounded;
    
    //Awake is called even before Start() is called
    void Awake() { rb = GetComponent<Rigidbody2D>();}

    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
    //     rb = GetComponent<Rigidbody2D>();
    // }

    void OnMove(InputValue value) {
        moveInput = value.Get<Vector2>();
    }

    void FixedUpdate()
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        float targetX = moveInput.x*moveSpeed;

        if(grounded){
            rb.linearVelocity = new Vector2(targetX, rb.linearVelocity.y);
        } else {
            rb.linearVelocity = new Vector2(
                Mathf.Lerp(rb.linearVelocity.x, targetX, airControl),
                rb.linearVelocity.y
            );
        }
    }

    void OnJump(){
        Debug.Log("OnJump executed");
        if(grounded)
            Debug.Log("Jump and is grounded");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x,jumpSpeed);
    }
}