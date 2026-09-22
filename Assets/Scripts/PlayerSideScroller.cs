using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerSidescroller : MonoBehaviour
{
[Header("Movement")]
[SerializeField] private float speed = 8f; // horizontal speed in units per second
[SerializeField] private float jumpForce = 14f; // initial upward velocity when jumping
[Header("Ground Check")]
[SerializeField] private Transform groundCheck; // empty object at the player's feet
[SerializeField] private float groundRadius = 0.15f;
[SerializeField] private LayerMask groundLayer;
[Header("Feel")]
[SerializeField] private float coyoteTime = 0.1f; // grace period to jump after leaving a ledge
[SerializeField] private float jumpBufferTime = 0.1f; // grace period to press jump before landing
private Rigidbody2D rb;
private float horizontalInput;
private float coyoteCounter;
private float jumpBufferCounter;
private bool jumpRequested; // set in Update, consumed in FixedUpdate
private bool jumpCutRequested;
// Other scripts (like the camera) can read this to know which way we face.
public int FacingDirection { get; private set; } = 1;
private void Awake()
{
rb = GetComponent<Rigidbody2D>();
// Enforce the setting in code so it can't be forgotten in the Inspector.
rb.interpolation = RigidbodyInterpolation2D.Interpolate;
}
private void Update()
{
// Read input every frame. Physics is applied in FixedUpdate.
horizontalInput = InputHelper.Horizontal();
if (horizontalInput != 0)
FacingDirection = horizontalInput > 0 ? 1 : -1;
// Coyote time: reset the counter while grounded, count down in the air.
if (IsGrounded())
coyoteCounter = coyoteTime;
else
coyoteCounter -= Time.deltaTime;
// Jump buffer: remember a jump press for a short window.
if (InputHelper.JumpPressed())
jumpBufferCounter = jumpBufferTime;
else
jumpBufferCounter -= Time.deltaTime;
// Jump when both windows are open. We only flag it here and apply it in FixedUpdate.
if (jumpBufferCounter > 0f && coyoteCounter > 0f)
{
jumpRequested = true;
jumpBufferCounter = 0f;
coyoteCounter = 0f;
}
// Variable jump height: release early to cut the jump short.
if (InputHelper.JumpReleased())
jumpCutRequested = true;
}
private void FixedUpdate()
{
// Keep the vertical velocity from physics, only override horizontal.
rb.linearVelocityX = horizontalInput * speed;
if (jumpRequested)
{
rb.linearVelocityY = jumpForce;
jumpRequested = false;
}
if (jumpCutRequested)
{
if (rb.linearVelocityY > 0f)
rb.linearVelocityY *= 0.5f;
jumpCutRequested = false;
}
}
private bool IsGrounded()
{
return Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
}
// Draws the ground check circle in the Scene view for easier tuning.
private void OnDrawGizmosSelected()
{
if (groundCheck == null) return;
Gizmos.color = Color.green;
Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
}
}