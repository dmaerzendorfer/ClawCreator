using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.InputSystem;

public class Claw : MonoBehaviour
{

    [SerializeField] private GameObject leftHinge;
    [SerializeField] private GameObject rightHinge;
    [SerializeField] private float maxAngle;
    [SerializeField] private float minAngle;
    [SerializeField] private float speed = 1;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private float grabHeight;
    [SerializeField] private Transform grabCenter;
    
    private Vector2 _movementVector;
    // private bool _open;
    private float _lastActivation;
    private bool _inAnimation = false;
    private List<CapsuleScript> _capsules = new List<CapsuleScript>();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_inAnimation) return;
        Vector2 plannedMovement = _movementVector * (Time.deltaTime * speed);
        if (transform.position.x + plannedMovement.x <= -4) return;
        if (transform.position.x + plannedMovement.x >= 14) return;
        transform.Translate(plannedMovement, Space.World);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _movementVector = context.ReadValue<Vector2>();
        _movementVector.y = 0;
    }

    public void OnActivate(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        if (_inAnimation) return;

        // if (_lastActivation >= Time.time)
        // {
        //     return;
        // }

        // _lastActivation = Time.time + secondsBetweenGrabs;
        // _open = !_open;
        // if (_open)
        // {
        Tween.Rotation(leftHinge.transform, Quaternion.Euler(0, 0, maxAngle), duration: 1);
        Tween.Rotation(rightHinge.transform, Quaternion.Euler(0, 180, maxAngle), duration: 1);

        Vector3 grabPosition = new Vector3(transform.position.x, grabHeight, transform.position.z);
        Vector3 returnPosition = new Vector3(transform.position.x, startPosition.y, transform.position.z);

        _inAnimation = true;
        
        // Sequence ballSequence = new Sequence();
        
        Sequence grabSequence = Sequence.Create()
            // Move down and open claw
            .Group(Tween.Position(transform, grabPosition, duration: 1))
            .Group(Tween.Rotation(leftHinge.transform, Quaternion.Euler(0, 0, maxAngle), duration: 1))
            .Group(Tween.Rotation(rightHinge.transform, Quaternion.Euler(0, 180, maxAngle), duration: 1))
            // grab stuff
            .Chain(Tween.Rotation(leftHinge.transform, Quaternion.Euler(0, 0, minAngle), duration: 1))
            .Group(Tween.Rotation(rightHinge.transform, Quaternion.Euler(0, 180, minAngle), duration: 1))
            // move back up
            .Chain(Tween.Position(transform, returnPosition, duration: 1))
            // move to start position
            .Chain(Tween.Position(transform, startPosition, duration: 1).OnComplete(() => { 
                DetectBalls();
                CreateBallSequence();
            }))
            // move to bg by scaling
            .Chain(Tween.Scale(transform, new Vector3(0.7f, 0.7f, 0.7f), duration: 1))
            // .Group(ballSequence)
            // open claw
            .Chain(Tween.Rotation(leftHinge.transform, Quaternion.Euler(0, 0, maxAngle), duration: 1))
            .Group(Tween.Rotation(rightHinge.transform, Quaternion.Euler(0, 180, maxAngle), duration: 1).OnComplete(() =>
            {
                // foreach (CapsuleScript capsule in _capsules)
                // {
                //     Destroy(capsule.gameObject);
                // }
                _capsules.Clear();
            }))
            // close claw
            .Chain(Tween.Rotation(leftHinge.transform, Quaternion.Euler(0, 0, minAngle), duration: 1))
            .Group(Tween.Rotation(rightHinge.transform, Quaternion.Euler(0, 180, minAngle), duration: 1))
            // move to foreground
            .Chain(Tween.Scale(transform, new Vector3(1f, 1f, 1f), duration: 1))
            // stop animation lock
            .OnComplete(() => {
                _inAnimation = false;
            });
    }
    //     }
    //     else
    //     {
    //         Tween.Rotation(leftHinge.transform, Quaternion.Euler(0, 0, minAngle), duration: 1);
    //         Tween.Rotation(rightHinge.transform, Quaternion.Euler(0, 180, minAngle), duration: 1);
    //     }
    // }

    private void DetectBalls()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(new Vector2(grabCenter.position.x, grabCenter.position.y), new Vector2(2, 2), 0, Vector2.down, 0);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject.CompareTag("Ball"))
            {
                _capsules.Add(hit.collider.gameObject.GetComponent<CapsuleScript>());
                Debug.Log("Found Ball " + _capsules.Count + " at " + grabCenter.position);
            }
        }
    }

    private Sequence CreateBallSequence()
    {
        Sequence s = Sequence.Create();
        Debug.Log("Creating Ball Sequence for balls: " + _capsules.Count);
        foreach (CapsuleScript capsule in _capsules)
        {
            Debug.Log("Grouping for scale: " + capsule);
            s.Group(Tween.Scale(capsule.transform, new Vector3(0.7f, 0.7f, 0.7f), duration: 1));
        }
        return s;
    }
}
