using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [SerializeField] private bool moveLeft = true;
    [SerializeField] private float speed = 1;
    [SerializeField] private float maxForce = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            if (collision.relativeVelocity.magnitude > maxForce)
            {
                Debug.Log(collision.relativeVelocity.magnitude);
                return;
            }
            if (moveLeft)
                collision.transform.GetComponent<Rigidbody2D>().AddForce(Vector2.right * speed);
            else
                collision.transform.GetComponent<Rigidbody2D>().AddForce(-Vector2.right * speed);
        }
    }
}
