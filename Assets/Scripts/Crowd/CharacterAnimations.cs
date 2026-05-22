using System.Collections;
using EditorAttributes;
using UnityEditor.Animations;
using UnityEngine;

namespace Crowd
{
    public class CharacterAnimations : MonoBehaviour
    {
     
        [SerializeField]
        private Animator animator;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        
        private bool _isIdle = false;
        private bool _isWalking = false;
        [SerializeField, HideProperty, MinMaxSlider(2, 60)]
        private Vector2 timeBetweenIdleAnimations = new Vector2(5, 30);
        private Coroutine _idleRoutine;

        public void LoopIdle()
        {
            //TODO loop idle animations until interrupted, interrupt walking
            _isIdle = true;
            _isWalking = false;
            animator.SetBool("walking", false);
            _idleRoutine = StartCoroutine(Idle());
        }

        public void LoopWalk()
        {
            //TODO walk from podest to crowd or move within crowd, interrupt idling
            _isWalking = true;
            _isIdle = false;
            if (_idleRoutine != null)
            {
                StopCoroutine(_idleRoutine);
            }
            animator.SetBool("walking", true);
        }

        public void HitGround()
        {
            animator.SetBool("onground", true);
            LoopIdle();
        }

        private IEnumerator Idle()
        {
            yield return new WaitForSeconds(Random.Range(timeBetweenIdleAnimations.x, timeBetweenIdleAnimations.y));
            switch (Random.Range(0, 10))
            {
                case 0:
                    animator.SetTrigger("interrupt1");
                    break;
                case 1:
                    animator.SetTrigger("interrupt2");
                    break;
                case 2:
                    animator.SetTrigger("interrupt3");
                    break;
                case 3:
                    animator.SetTrigger("interrupt4");
                    break;
                case 4:
                    animator.SetTrigger("interrupt5");
                    break;
                case 5:
                    animator.SetTrigger("interrupt6");
                    break;
                case 6:
                    animator.SetTrigger("interrupt7");
                    break;
                case 7:
                    animator.SetTrigger("interrupt8");
                    break;
                case 8:
                    animator.SetTrigger("interrupt9");
                    break;
                case 9:
                    animator.SetTrigger("interrupt10");
                    break;
                
            }

            if (_isIdle)
            {
                StartCoroutine(Idle());
            }
        }

    }
}