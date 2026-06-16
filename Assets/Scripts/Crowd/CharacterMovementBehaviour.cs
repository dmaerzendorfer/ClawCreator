using EditorAttributes;
using PrimeTween;
using UnityEngine;

namespace Crowd
{
    public class CharacterMovementBehaviour : MonoBehaviour
    {
        public Character character;
        public Vector3 target;
        public float avgSpeed = 3f;
        public AudioSource walkSound;
        
        [MinMaxSlider(0f, 2f)]
        public Vector2 delayBeforeMove = new Vector2(0f, 1f);

        public float newbieTime = 5f;
        public bool deleteAfterReachingTarget = false;
        public float timeInFrontRow = 0f;
        public bool isInFrontRow = false;

        public bool IsNew => isNew;
        private bool isNew = false;

        private Tween walkingTween;

        public void SetTarget(Vector3 pos, bool delayWalk = true)
        {
            walkingTween.Stop();
            character.LookForward();
            var delay = delayWalk ? Random.Range(delayBeforeMove.x, delayBeforeMove.y) : 0f;
                
            
            Tween.Delay(delay, () =>
            {
                target = pos;
                walkSound.Play();
                walkingTween = Tween.PositionAtSpeed(transform, target, avgSpeed, Easing.Standard(Ease.InOutSine))
                    .OnUpdate(this, (character, tween) => { character.transform.LookAt(target); }).OnComplete(() =>
                    {
                        if (deleteAfterReachingTarget)
                        {
                            Tween.StopAll(gameObject);
                            Destroy(gameObject);
                        }
                        walkSound.Stop();
                        character.RotateTowardsCamera();
                        character.emotions.StartHappySequence();
                        character.animations.LoopIdle();
                    });
                character.animations.LoopWalk();
            });
        }

        public void StartNewbieTimer()
        {
            isNew = true;
            Tween.Delay(newbieTime).OnComplete(() => isNew = false);
        }

        public void Update()
        {
            if (isInFrontRow)
            {
                timeInFrontRow += Time.deltaTime;
            }
        }
    }
}