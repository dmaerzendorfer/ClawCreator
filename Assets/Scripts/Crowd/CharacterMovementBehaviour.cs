using PrimeTween;
using UnityEngine;

namespace Crowd
{
    public class CharacterMovementBehaviour : MonoBehaviour
    {
        public Character character;
        public Vector3 target;
        public float avgSpeed = 3f;

        public float newbieTime = 5f;

        public bool IsNew => isNew;

        private bool isNew = false;
        public float timeInFrontRow = 0f;
        public bool isInFrontRow = false;

        public void SetTarget(Vector3 pos)
        {
            character.LookForward();
            target = pos;
            var originalRot = transform.rotation;
            Tween.PositionAtSpeed(transform, target, avgSpeed, Easing.Standard(Ease.InOutSine))
                .OnUpdate(this, (character, tween) => { character.transform.LookAt(target); }).OnComplete(() =>
                {
                    character.shouldLookAtTarget = true;
                    character.transform.rotation = originalRot;
                    character.emotions.StartHappySequence();
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
            else
            {
                // decay so they can come back later
                //timeInFrontRow = Mathf.Max(0f, timeInFrontRow - Time.deltaTime * 0.5f);
            }
        }
    }
}