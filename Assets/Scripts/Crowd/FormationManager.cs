using System;
using System.Collections.Generic;
using EditorAttributes;
using UnityEngine;

namespace Crowd
{
    public class FormationManager : MonoBehaviour
    {
        [SerializeField]
        private TestAvatar avatarPrefab;

        [SerializeField]
        private GameObject spawnPoint;

        [SerializeField]
        private CrowdFormationSettings formationSettings;

        private List<TestAvatar> avatars = new List<TestAvatar>();
        private TestAvatar centerAvatar;

        private List<Vector3> crowdSlots;

        private void Start()
        {
            var newAvatar = Instantiate(avatarPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
            centerAvatar = newAvatar;
            centerAvatar.transform.parent = transform;

            //just calc for display purposes
            crowdSlots = CrowdFormation.GenerateSlots(
                centerAvatar.transform.position,
                centerAvatar.transform.forward,
                100,
                formationSettings
            );
        }

        private void OnDrawGizmos()
        {
            if (crowdSlots == null) return;
            
            Gizmos.color = Color.cyan;
            foreach (var slot in crowdSlots)
            {
                Gizmos.DrawSphere(slot, 0.1f);
            }
        }


        [Button]
        public void SpawnNextAvatar()
        {
            //insert current center avatar at front
            avatars.Insert(0, centerAvatar);
            //newbiew avatars get better crowd slots
            centerAvatar.StartNewbieTimer();
            //make avatars move into formation
            UpdateFormation();

            //spawn new avatar
            var newAvatar = Instantiate(avatarPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
            centerAvatar.transform.parent = transform;
            centerAvatar = newAvatar;
        }


        void UpdateFormation()
        {
            crowdSlots = CrowdFormation.GenerateSlots(
                transform.position,
                transform.forward,
                avatars.Count,
                formationSettings
            );

            CrowdFormation.AssignSlotsSmart(avatars, crowdSlots,formationSettings,transform.position);
        }

        public static void AssignSlotsClosest(
            List<TestAvatar> avatars,
            List<Vector3> slots)
        {
            // Track available slots
            List<Vector3> availableSlots = new List<Vector3>(slots);

            foreach (var mini in avatars)
            {
                float bestDist = float.MaxValue;
                int bestIndex = -1;

                for (int i = 0; i < availableSlots.Count; i++)
                {
                    float dist = (mini.transform.position - availableSlots[i]).sqrMagnitude;

                    if (dist < bestDist)
                    {
                        bestDist = dist;
                        bestIndex = i;
                    }
                }

                if (bestIndex != -1)
                {
                    mini.SetTarget(availableSlots[bestIndex]);
                    availableSlots.RemoveAt(bestIndex);
                }
            }
        }
    }
}