using System;
using UnityEngine;

public class AnimationSpeed : MonoBehaviour
{
    [SerializeField]
    private float speed;
    
    [SerializeField]
    private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator.speed = speed;
    }

    private void OnEnable()
    {
        animator.speed = speed;
    }
}
