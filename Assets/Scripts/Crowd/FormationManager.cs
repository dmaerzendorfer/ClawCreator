using System.Collections.Generic;
using System.Linq;
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
        [Tooltip("Characters will walk to one of these points before despawning. Should be offscreen.")]
        private List<GameObject> despawnPoints;

        [SerializeField]
        private int maxCrowdSize = 2;


        [SerializeField]
        private CrowdFormationSettings formationSettings;

        private List<CharacterMovementBehaviour> _avatars = new List<CharacterMovementBehaviour>();
        private CharacterMovementBehaviour _centerCharacterMovement;

        private List<Vector3> _crowdSlots;

        private void Start()
        {
            //just calc for display purposes
            _crowdSlots = CrowdFormation.GenerateSlots(
                transform.position,
                transform.forward,
                100,
                formationSettings
            );
        }

        private void OnDrawGizmos()
        {
            if (_crowdSlots == null) return;

            Gizmos.color = Color.cyan;
            foreach (var slot in _crowdSlots)
            {
                Gizmos.DrawSphere(slot, 0.1f);
            }
        }

        [Button]
        public GameObject SpawnNextAvatar()
        {
            if (_centerCharacterMovement != null)
            {
                //insert current center avatar at front
                _avatars.Insert(0, _centerCharacterMovement);
                //newbiew avatars get better crowd slots
                _centerCharacterMovement.StartNewbieTimer();
                //make avatars move into formation
                UpdateFormation();
            }

            //spawn new avatar
            var newAvatar = Instantiate(characterMovementPrefab, spawnPoint.transform.position,
                spawnPoint.transform.rotation);
            _centerCharacterMovement = newAvatar;
            _centerCharacterMovement.transform.parent = transform;
            _centerCharacterMovement = newAvatar;
            return _centerCharacterMovement.gameObject;
        }


        void UpdateFormation()
        {
            //let the characters pop and wait a few sec before they move

            //check if we have too many avatars, if so, send the ones in the back to despawn
            if (_avatars.Count > maxCrowdSize)
            {
                int excess = _avatars.Count - maxCrowdSize;
                for (int i = 0; i < excess; i++)
                {
                    var toBeDeleted = _avatars[_avatars.Count - 1];
                    _avatars.RemoveAt(_avatars.Count - 1);

                    //todo: maybe let them emote with a wave or smth before moving out?
                    var despawnPoint = despawnPoints
                        .OrderBy(x => (x.transform.position - toBeDeleted.transform.position).sqrMagnitude).First();
                    toBeDeleted.deleteAfterReachingTarget = true;
                    toBeDeleted.SetTarget(despawnPoint.transform.position);
                }
            }

            _crowdSlots = CrowdFormation.GenerateSlots(
                transform.position,
                transform.forward,
                _avatars.Count,
                formationSettings
            );

            CrowdFormation.AssignSlotsSmart(_avatars, _crowdSlots, formationSettings, transform.position);
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

        public int GetAvatarCount()
        {
            return _avatars.Count;
        }
    }
}