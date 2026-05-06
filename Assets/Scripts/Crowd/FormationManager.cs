using System.Collections.Generic;
using EditorAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Crowd
{
    public class FormationManager : MonoBehaviour
    {
        [FormerlySerializedAs("avatarPrefab")]
        [SerializeField]
        private CharacterMovementBehaviour characterMovementPrefab;

        [SerializeField]
        private GameObject spawnPoint;

        [SerializeField]
        private CrowdFormationSettings formationSettings;

        private List<CharacterMovementBehaviour> avatars = new List<CharacterMovementBehaviour>();
        private CharacterMovementBehaviour centerCharacterMovement;

        private List<Vector3> crowdSlots;

        private void Start()
        {
            //just calc for display purposes
            crowdSlots = CrowdFormation.GenerateSlots(
                transform.position,
                transform.forward,
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
        public GameObject SpawnNextAvatar()
        {
            if (centerCharacterMovement != null)
            {
                //insert current center avatar at front
                avatars.Insert(0, centerCharacterMovement);
                //newbiew avatars get better crowd slots
                centerCharacterMovement.StartNewbieTimer();
                //make avatars move into formation
                UpdateFormation();
            }

            //spawn new avatar
            var newAvatar = Instantiate(characterMovementPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
            centerCharacterMovement = newAvatar;
            centerCharacterMovement.transform.parent = transform;
            centerCharacterMovement = newAvatar;
            return centerCharacterMovement.gameObject;
        }


        void UpdateFormation()
        {
            crowdSlots = CrowdFormation.GenerateSlots(
                transform.position,
                transform.forward,
                avatars.Count,
                formationSettings
            );

            CrowdFormation.AssignSlotsSmart(avatars, crowdSlots, formationSettings, transform.position);
        }

        /// <summary>
        /// deprecated, use CrowdFormation AssignSlotsSmart instead.
        /// </summary>
        /// <param name="avatars"></param>
        /// <param name="slots"></param>
        public static void AssignSlotsClosest(
            List<CharacterMovementBehaviour> avatars,
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