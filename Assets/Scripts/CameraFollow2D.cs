using UnityEngine;
[RequireComponent(typeof(Camera))]
public class CameraFollow2D : MonoBehaviour
{
[Header("Target")]
[SerializeField] private Transform target;
[SerializeField] private Vector2 offset = Vector2.zero; // shift the framing, e.g. (0, 1) to see more above the player
[Header("Axes")]
[SerializeField] private bool followX = true;
[SerializeField] private bool followY = true;
[SerializeField] private float fixedY = 0f; // used when followY is false
[Header("Smoothing")]
[SerializeField] private float smoothTime = 0.2f; // lower = snappier. SmoothDamp is frame-rate independent.
[Header("Dead Zone")]
[SerializeField] private Vector2 deadZone = Vector2.zero;
[Header("Look Ahead")]
[SerializeField] private float lookAheadDistance = 0f;
[SerializeField] private float lookAheadSmooth = 0.5f;
[Header("Bounds")]
[SerializeField] private BoxCollider2D levelBounds; // optional, clamps the camera to the level
private Camera cam;
private Vector3 velocity; // internal state for SmoothDamp
private Vector3 lastTargetPos;
private Vector2 currentLookAhead;
private void Awake()
{
cam = GetComponent<Camera>();
if (target != null) lastTargetPos = target.position;
}
// LateUpdate runs after every Update, so the target has already moved this frame.
private void LateUpdate()
{
if (target == null) return;
Vector3 desired = transform.position;
// 1. Look-ahead based on the target's movement direction.
Vector3 targetDelta = target.position - lastTargetPos;
lastTargetPos = target.position;
Vector2 lookGoal = Vector2.zero;
if (lookAheadDistance > 0f && targetDelta.sqrMagnitude > 0.0001f)
lookGoal = ((Vector2)targetDelta).normalized * lookAheadDistance;
currentLookAhead = Vector2.Lerp(currentLookAhead, lookGoal, Time.deltaTime /
Mathf.Max(lookAheadSmooth, 0.001f));
Vector2 focus = (Vector2)target.position + offset + currentLookAhead;
// 2. Dead zone: only move when the focus point leaves the box.
if (followX)
{
float dx = focus.x - transform.position.x;
if (Mathf.Abs(dx) > deadZone.x)
desired.x = focus.x - Mathf.Sign(dx) * deadZone.x;
}
if (followY)
{
float dy = focus.y - transform.position.y;
if (Mathf.Abs(dy) > deadZone.y)
desired.y = focus.y - Mathf.Sign(dy) * deadZone.y;
}
else
{
desired.y = fixedY;
}
// 3. Clamp to level bounds so we never show outside the level.
if (levelBounds != null)
desired = ClampToBounds(desired);
// 4. Keep the camera in front of the scene.
desired.z = -10f;
// 5. Smooth toward the desired position.
transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
}
private Vector3 ClampToBounds(Vector3 pos)
{
float halfHeight = cam.orthographicSize;
float halfWidth = halfHeight * cam.aspect;
Bounds b = levelBounds.bounds;
float minX = b.min.x + halfWidth;
float maxX = b.max.x - halfWidth;
float minY = b.min.y + halfHeight;
float maxY = b.max.y - halfHeight;
pos.x = minX > maxX ? b.center.x : Mathf.Clamp(pos.x, minX, maxX);
pos.y = minY > maxY ? b.center.y : Mathf.Clamp(pos.y, minY, maxY);
return pos;
}
// Draws the dead zone in the Scene view.
private void OnDrawGizmosSelected()
{
Gizmos.color = Color.yellow;
Gizmos.DrawWireCube(transform.position, new Vector3(deadZone.x * 2f, deadZone.y * 2f, 0f));
}
}