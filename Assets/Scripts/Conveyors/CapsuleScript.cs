using UnityEngine;

public class CapsuleScript : MonoBehaviour
{
    private Vector3 _startPos;

    [SerializeField] private Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= -10)
        {
            transform.position = _startPos;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
        }
    }
    
}
