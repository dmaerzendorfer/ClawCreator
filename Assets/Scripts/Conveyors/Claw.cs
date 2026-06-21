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

    [Header("Pop Feedback")]
    [SerializeField] public TweenSettings<Vector3> popFeedbackSettings;

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
    private Tween _popTween;


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
        if (!_inAnimation)
        _am.StopSound("LeftRight");


        if (_inAnimation) return;
        Vector2 plannedMovement = _movementVector * (Time.deltaTime * speed);
        if (transform.position.x + plannedMovement.x <= -4) return;
        if (transform.position.x + plannedMovement.x >= 14) return;

        // if (_movementVector.x != 0) _am.PlaySound("LeftRight");
        transform.Translate(plannedMovement, Space.World);
    }

    public void DoScaleFeedback(float delay = 0f, Action OnComplete = null)
    {
        if (_popTween.isAlive) _popTween.Complete();
        Tween.Delay(delay).OnComplete(() =>
        {
            _am.PlaySound("Pop2");
            _popTween = Tween.Scale(transform, popFeedbackSettings)
                .OnComplete(OnComplete);
        });
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _movementVector = context.ReadValue<Vector2>();
        _movementVector.y = 0;
    }

    public void SetClawText(string s)
    {
        countDisplay.SetText(s);
        DoScaleFeedback();
    }

    public void OnActivate(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        if (_movingDown)
        {
            _am.StopSound("Down");
            _grabSequence.Stop();
            _inAnimation = true;
            _movingDown = false;
            Sequence.Create()
                .Chain(Tween.Rotation(leftHinge.transform, Quaternion.Euler(0, 0, minAngle),
                    duration: timeBetweenAnimation))
                .Group(Tween.Rotation(rightHinge.transform, Quaternion.Euler(0, 180, minAngle),
                    duration: timeBetweenAnimation))
                .ChainCallback(() => _am.PlaySound("Click"))
                //go up
                .ChainCallback(() => _am.PlaySound("Up"))
                .Chain(Tween.Position(transform,
                    new Vector3(transform.position.x, startPosition.y, transform.position.z),
                    duration: timeBetweenAnimation + 0.5f))
                .ChainCallback(() => _am.StopSound("Up"))

                //go center
                .ChainCallback(() => _am.PlaySound("LeftRight"))
                .Chain(Tween.Position(transform, startPosition, duration: timeBetweenAnimation +
                                                                          (Mathf.Abs(transform.position.x - 5) / 9) *
                                                                          timeExtension).OnComplete(() =>
                {
                    DetectBalls();
                    CreateBallSequence();
                }))
                // move to bg by scaling
                .Chain(Tween.Scale(transform, new Vector3(0.7f, 0.7f, 0.7f), duration: timeBetweenAnimation))
                // .Group(ballSequence)
                // open claw
                .ChainCallback(() => _am.StopSound("LeftRight"))
                .Chain(Tween.Rotation(leftHinge.transform, Quaternion.Euler(0, 0, maxAngle),
                    duration: timeBetweenAnimation))
                .Group(Tween.Rotation(rightHinge.transform, Quaternion.Euler(0, 180, maxAngle),
                        duration: timeBetweenAnimation)
                    .OnComplete(() =>
                    {
                        // foreach (CapsuleScript capsule in _capsules)
                        // {
                        //     Destroy(capsule.gameObject);
                        // }
                        _capsules.Clear();
                        _am.StopSound("LeftRight");
                    }))
                // close claw
                .Chain(Tween.Rotation(leftHinge.transform, Quaternion.Euler(0, 0, minAngle),
                    duration: timeBetweenAnimation))
                .Group(Tween.Rotation(rightHinge.transform, Quaternion.Euler(0, 180, minAngle),
                    duration: timeBetweenAnimation))

                // move to foreground
                .ChainCallback(() => _am.PlaySound("LeftRight"))
                .Chain(Tween.Scale(transform, new Vector3(1f, 1f, 1f), duration: timeBetweenAnimation)
                    .OnComplete(() =>
                    {
                        _inAnimation = false;
                        _gm.OnGrabComplete();
                        _am.StopSound("LeftRight");
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
            // Move down and open claw at same time
            .ChainCallback(() => _am.PlaySound("Down"))
            .ChainCallback(() => _am.PlaySound("Click"))
            .Group(Tween.Position(transform, grabPosition, duration: timeBetweenAnimation))
            .Group(Tween.Rotation(leftHinge.transform, Quaternion.Euler(0, 0, maxAngle),
                duration: timeBetweenAnimation))
            .Group(Tween
                .Rotation(rightHinge.transform, Quaternion.Euler(0, 180, maxAngle), duration: timeBetweenAnimation)
                .OnComplete(() => { _movingDown = false; })
            )
            .ChainCallback(() => _am.StopSound("Down"))
            // grab stuff
            .Chain(Tween.Rotation(leftHinge.transform, Quaternion.Euler(0, 0, minAngle),
                duration: timeBetweenAnimation))
            .Group(Tween.Rotation(rightHinge.transform, Quaternion.Euler(0, 180, minAngle),
                duration: timeBetweenAnimation))
            // move back up
            .ChainCallback(() => _am.PlaySound("Up"))
            .Chain(Tween.Position(transform, returnPosition, duration: timeBetweenAnimation + 0.5f))
            .ChainCallback(() => _am.StopSound("Up"))
            // move to start position
            .ChainCallback(() =>
            {
                _am.PlaySound("LeftRight");
                Debug.Log("left right sound for movement");
            })
            .Chain(Tween.Position(transform, startPosition, duration: timeBetweenAnimation +
                                                                      (Mathf.Abs(transform.position.x - 5) / 9) *
                                                                      timeExtension).OnComplete(() =>
            {
                DetectBalls();
                CreateBallSequence();
            }))
            .ChainCallback(() => _am.PlaySound("LeftRight"))
            // move to bg by scaling
            .Chain(Tween.Scale(transform, new Vector3(0.7f, 0.7f, 0.7f), duration: timeBetweenAnimation))
            // .Group(ballSequence)
            // open claw
            .ChainCallback(() => _am.StopSound("LeftRight"))
            .Chain(Tween.Rotation(leftHinge.transform, Quaternion.Euler(0, 0, maxAngle),
                duration: timeBetweenAnimation))
            .Group(Tween
                .Rotation(rightHinge.transform, Quaternion.Euler(0, 180, maxAngle), duration: timeBetweenAnimation)
                .OnComplete(() =>
                {
                    // foreach (CapsuleScript capsule in _capsules)
                    // {
                    //     Destroy(capsule.gameObject);
                    // }
                    _capsules.Clear();
                }))
            // close claw
            .Chain(Tween.Rotation(leftHinge.transform, Quaternion.Euler(0, 0, minAngle),
                duration: timeBetweenAnimation))
            .Group(Tween.Rotation(rightHinge.transform, Quaternion.Euler(0, 180, minAngle),
                duration: timeBetweenAnimation))
            // move to foreground
            .ChainCallback(() => _am.PlaySound("LeftRight"))
            .Chain(Tween.Scale(transform, new Vector3(1f, 1f, 1f), duration: timeBetweenAnimation))
            // stop animation lock
            .OnComplete(() =>
            {
                _am.StopSound("LeftRight");
                _inAnimation = false;
                _gm.OnGrabComplete();
            });
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