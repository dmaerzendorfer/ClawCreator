using System;
using System.Collections;
using PrimeTween;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Character : MonoBehaviour
{
    private Rigidbody _rigidbody;
    public bool shouldLookAtTarget = false;
    public Transform head;
    public Transform lookAtTarget;

    public float walkDuration = 1f;
    // public float moveSpeed = 10f;
    // public float maxDeltaDistance = 0.1f;

    private Coroutine _moveCoroutine;

    private GameManager _gameManager;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _gameManager = GameManager.GetInstance();
    }

    public void MoveTo(Vector3 worldPos, Action onComplete)
    {
        Tween.RigidbodyMovePosition(_rigidbody, worldPos,walkDuration).OnUpdate(target: this,(t,tween) =>
        {
            head.transform.LookAt(worldPos);
        }).OnComplete(
            ()=>
            {
                shouldLookAtTarget = true;
                onComplete();
            });
        // if (_moveCoroutine == null)
        //     _moveCoroutine = StartCoroutine(MoveToTarget(target, onComplete));
    }

    // private IEnumerator MoveToTarget(Transform target, Action onComplete)
    // {
    //     while (Vector3.Distance(transform.position, target.position) > maxDeltaDistance)
    //     {
    //         transform.LookAt(target);
    //         Vector3 newPosition = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
    //         _rigidbody.MovePosition(newPosition);
    //         yield return null;
    //     }
    //
    //     shouldLookAtTarget = true;
    //     onComplete();
    // }

    private void Update()
    {
        if (shouldLookAtTarget)
            head.transform.LookAt(_gameManager.currentCharacter.head.transform);
    }
}