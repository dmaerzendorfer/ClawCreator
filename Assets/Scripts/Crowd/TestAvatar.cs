using PrimeTween;
using UnityEngine;

namespace Crowd
{
    public class TestAvatar : MonoBehaviour
    {
        public Vector3 target;

        public float newbieTime = 5f;
        
        public bool IsNew => isNew;
        
        private bool isNew = false;
        public float timeInFrontRow = 0f;
        public bool isInFrontRow = false;
        
        public void SetTarget(Vector3 pos)
        {
            target = pos;
            // NavMeshAgent or simple movement
            Tween.PositionAtSpeed(transform, target, 3f, Easing.Standard(Ease.InOutSine));
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
                timeInFrontRow = Mathf.Max(0f, timeInFrontRow - Time.deltaTime * 0.5f);
            }
        }
    }
}