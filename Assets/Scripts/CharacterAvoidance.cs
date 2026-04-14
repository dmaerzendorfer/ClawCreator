using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class CharacterAvoidance : MonoBehaviour
{
    private Rigidbody _rigidbody;

    // Avoidance / distancing settings
    [Header("Avoidance")]
    public bool enableAvoidance = true;

    public bool showDebugRays = true;

    public float rayRadius = 5f;

    public int rayCount = 16;

    // Minimum allowed horizontal distance to other characters. If another character is closer than
    // this, this character will move away to maintain at least `minDistance`.
    public float minDistance = 2f;
    public float repulsionSpeed = 2f; // units per second to move towards safe position
    public float rayHeightOffset = 0.5f; // start rays slightly above ground (y up)
    public LayerMask avoidanceLayerMask = ~0; // which layers to consider (default: everything)


    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // Physics-driven avoidance - shoot rays and try to maintain distance in physics step
        if (enableAvoidance && _rigidbody != null)
        {
            MaintainDistanceFromHits();
        }
    }

    /// <summary>
    /// Casts rays in a horizontal circle around this character (Y up), finds other Characters hit,
    /// computes a target position that keeps this character at `desiredDistance` from each hit character,
    /// averages those targets and moves the rigidbody toward the average using MovePosition.
    /// </summary>
    private void MaintainDistanceFromHits()
    {
        Vector3 origin = transform.position + Vector3.up * rayHeightOffset;

        int hitsFound = 0;
        Vector3 avgDesiredPos = Vector3.zero;

        int rays = Mathf.Max(3, rayCount);
        for (int i = 0; i < rays; i++)
        {
            float angle = (360f / rays) * i;
            Vector3 dir = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;

            // Debug draw for visibility in editor/game view
            if (showDebugRays)
                Debug.DrawRay(origin, dir * rayRadius, Color.yellow);

            if (Physics.Raycast(origin, dir, out RaycastHit hit, rayRadius, avoidanceLayerMask,
                    QueryTriggerInteraction.Ignore))
            {
                // Prefer to find the Character component on the hit collider or its parents
                Character other = hit.collider.GetComponentInParent<Character>();
                // Ignore hits on ourselves: compare GameObjects because `this` is CharacterAvoidance
                if (other != null && other.gameObject != gameObject)
                {
                    // Compute horizontal vector from other to this character
                    Vector3 toThis = transform.position - other.transform.position;
                    toThis.y = 0f;

                    float dist = toThis.magnitude;
                    if (dist < 0.0001f)
                    {
                        // fallback: use the ray direction if positions are nearly identical
                        toThis = new Vector3(dir.x, 0f, dir.z);
                        if (toThis.sqrMagnitude < 0.0001f) toThis = Vector3.forward;
                        dist = toThis.magnitude;
                    }

                    // Only consider this hit if we're too close: distance less than minDistance
                    if (dist < minDistance)
                    {
                        Vector3 targetPos = other.transform.position + toThis.normalized * minDistance;
                        // Keep our current Y so we don't snap vertically
                        targetPos.y = transform.position.y;

                        avgDesiredPos += targetPos;
                        hitsFound++;

                        // Draw hit ray in red for hits that cause repulsion
                        if (showDebugRays)
                            Debug.DrawLine(origin, hit.point, Color.red);
                    }
                }
            }
        }

        if (hitsFound > 0)
        {
            avgDesiredPos /= hitsFound;

            // Move towards the averaged desired position at configured speed in physics step
            Vector3 newPos =
                Vector3.MoveTowards(transform.position, avgDesiredPos, repulsionSpeed * Time.fixedDeltaTime);

            // Use MovePosition for physics-safe movement
            _rigidbody.MovePosition(newPos);
            // transform.position = newPos; // keep transform in sync
        }
    }
}