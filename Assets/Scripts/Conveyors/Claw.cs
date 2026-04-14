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
    [SerializeField] private float secondsBetweenGrabs = 10;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private float grabHeight;
    
    private Vector2 _movementVector;
    // private bool _open;
    private float _lastActivation;
    private bool _inAnimation = false;
    
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
        
        Sequence.Create()
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
            .Chain(Tween.Position(transform, startPosition, duration: 1))
            // move to bg by scaling
            .Chain(Tween.Scale(transform, new Vector3(0.7f, 0.7f, 0.7f), duration: 1))
            // open claw
            .Chain(Tween.Rotation(leftHinge.transform, Quaternion.Euler(0, 0, maxAngle), duration: 1))
            .Group(Tween.Rotation(rightHinge.transform, Quaternion.Euler(0, 180, maxAngle), duration: 1))
            // close claw
            .Chain(Tween.Rotation(leftHinge.transform, Quaternion.Euler(0, 0, minAngle), duration: 1))
            .Group(Tween.Rotation(rightHinge.transform, Quaternion.Euler(0, 180, minAngle), duration: 1))
            // move to forground
            .Chain(Tween.Scale(transform, new Vector3(1f, 1f, 1f), duration: 1))
            // stop animation lock
            .OnComplete(() => {_inAnimation = false;});
    }
    //     }
    //     else
    //     {
    //         Tween.Rotation(leftHinge.transform, Quaternion.Euler(0, 0, minAngle), duration: 1);
    //         Tween.Rotation(rightHinge.transform, Quaternion.Euler(0, 180, minAngle), duration: 1);
    //     }
    // }
}
