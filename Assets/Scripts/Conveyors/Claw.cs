using System;
using System.Collections.Generic;
using Audio;
using PrimeTween;
using TMPro;
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
    [SerializeField] public TextMeshProUGUI countDisplay;
    [SerializeField] public float timeBetweenAnimation = 0.7f;
    [SerializeField] public float timeExtension = 2f;
    [SerializeField] public float easingStrength = 0.5f;

    public bool canGrab = true;

    private Vector2 _movementVector;
    private GameManager _gm;
    private AudioManager _am;

    // private bool _open;
    private float _lastActivation;
    private bool _inAnimation = false;
    private List<CapsuleScript> _capsules = new List<CapsuleScript>();
    private Sequence _grabSequence;
    private bool _movingDown = false;


    private void Start()
    {
        _gm = GameManager.GetInstance();
        _am = AudioManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (_movementVector.x != 0 && !_inAnimation)
            _am.PlaySound("LeftRight");
        else
            _am.StopSound("LeftRight");


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

    public void SetClawText(string s)
    {
        countDisplay.SetText(s);
    }

    public void OnActivate(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        if (_movingDown)
        {
            _grabSequence.Stop();
            _inAnimation = true;
            Sequence.Create()
                .Group(Tween.Position(transform, new Vector3(transform.position.x, startPosition.y, transform.position.z), duration: timeBetweenAnimation))
                .Group( Tween.Rotation(leftHinge.transform, Quaternion.Euler(0, 0, minAngle), duration: timeBetweenAnimation)
                .Group( Tween.Rotation(rightHinge.transform, Quaternion.Euler(0, 180, minAngle), duration: timeBetweenAnimation))
                    .OnComplete(() =>
                    {
                        _movingDown = false;
                        _inAnimation = false;
                        _gm.OnGrabComplete();
                        _am.PlaySound("Click");
                    })
                );
        }
        if (!canGrab) return;

        if (_inAnimation) return;

        // if (_lastActivation >= Time.time)
        // {
        //     return;
        // }

        // _lastActivation = Time.time + secondsBetweenGrabs;
        // _open = !_open;
        // if (_open)
        // {
        Tween.Rotation(leftHinge.transform, Quaternion.Euler(0, 0, maxAngle), duration: timeBetweenAnimation);
        Tween.Rotation(rightHinge.transform, Quaternion.Euler(0, 180, maxAngle), duration: timeBetweenAnimation);

        Vector3 grabPosition = new Vector3(transform.position.x, grabHeight, transform.position.z);
        Vector3 returnPosition = new Vector3(transform.position.x, startPosition.y, transform.position.z);

        _inAnimation = true;

        // Sequence ballSequence = new Sequence();

        _movingDown = true;
        _grabSequence = Sequence.Create()
            // Move down and open claw
            .Group(Tween.Position(transform, grabPosition, duration: timeBetweenAnimation))
            .Group(Tween.Rotation(leftHinge.transform, Quaternion.Euler(0, 0, maxAngle), duration: timeBetweenAnimation))
            .Group(Tween.Rotation(rightHinge.transform, Quaternion.Euler(0, 180, maxAngle), duration: timeBetweenAnimation).OnComplete(() => { _movingDown = false; })
            )
            // grab stuff
            .Chain(Tween.Rotation(leftHinge.transform, Quaternion.Euler(0, 0, minAngle), duration: timeBetweenAnimation))
            .Group(Tween.Rotation(rightHinge.transform, Quaternion.Euler(0, 180, minAngle), duration: timeBetweenAnimation))
            // move back up
            .Chain(Tween.Position(transform, returnPosition, duration: timeBetweenAnimation))
            // move to start position
            .Chain(Tween.Position(transform, startPosition, duration: timeBetweenAnimation +
                                                                      (Mathf.Abs(transform.position.x - 5) / 9) * timeExtension).OnComplete(() =>
            {
                DetectBalls();
                CreateBallSequence();
            }))
            // move to bg by scaling
            .Chain(Tween.Scale(transform, new Vector3(0.7f, 0.7f, 0.7f), duration: timeBetweenAnimation)
                // .Group(ballSequence)
                // open claw
                .Chain(Tween.Rotation(leftHinge.transform, Quaternion.Euler(0, 0, maxAngle), duration: timeBetweenAnimation))
                .Group(Tween.Rotation(rightHinge.transform, Quaternion.Euler(0, 180, maxAngle), duration: timeBetweenAnimation)
                    .OnComplete(() =>
                    {
                        // foreach (CapsuleScript capsule in _capsules)
                        // {
                        //     Destroy(capsule.gameObject);
                        // }
                        _capsules.Clear();
                    }))
                // close claw
                .Chain(Tween.Rotation(leftHinge.transform, Quaternion.Euler(0, 0, minAngle), duration: timeBetweenAnimation))
                .Group(Tween.Rotation(rightHinge.transform, Quaternion.Euler(0, 180, minAngle), duration: timeBetweenAnimation))
                // move to foreground
                .Chain(Tween.Scale(transform, new Vector3(1f, 1f, 1f), duration: timeBetweenAnimation))
                // stop animation lock
                .OnComplete(() =>
                {
                    _inAnimation = false;
                    _gm.OnGrabComplete();
                    _am.PlaySound("Click");
                }));
    }
    //     }
    //     else
    //     {
    //         Tween.Rotation(leftHinge.transform, Quaternion.Euler(0, 0, minAngle), duration: timeBetweenAnimation);
    //         Tween.Rotation(rightHinge.transform, Quaternion.Euler(0, 180, minAngle), duration: timeBetweenAnimation);
    //     }
    // }

    private void DetectBalls()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(new Vector2(grabCenter.position.x, grabCenter.position.y),
            new Vector2(2, 2), 0, Vector2.down, 0);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject.CompareTag("Ball"))
            {
                _capsules.Add(hit.collider.gameObject.GetComponent<CapsuleScript>());
            }
        }
    }

    private Sequence CreateBallSequence()
    {
        Sequence s = Sequence.Create();
        foreach (CapsuleScript capsule in _capsules)
        {
            s.Group(Tween.Scale(capsule.transform, new Vector3(0.7f, 0.7f, 0.7f), duration: timeBetweenAnimation));
        }

        return s;
    }
}