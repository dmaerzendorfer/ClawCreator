using System;
using System.Collections.Generic;
using System.Linq;
using Audio;
using EditorAttributes;
using PrimeTween;
using UnityEngine;

namespace Crowd
{
    [Serializable]
    public class CharacterFeatures
    {
        public SkinnedMeshRenderer eyesPlane;
        public SkinnedMeshRenderer mouthPlane;
        public SkinnedMeshRenderer headwear;
        public MeshFilter headwearMesh;
        public SkinnedMeshRenderer nose;
        public MeshFilter noseMesh;
        public SkinnedMeshRenderer clothing;
        public MeshFilter clothingMesh;
        public Material skinPlaceholder;
        public List<SkinnedMeshRenderer> skinRenderers;
        public List<Material> possibleSkinMaterials;
    }

    [RequireComponent(typeof(Rigidbody))]
    public class Character : MonoBehaviour
    {
        private static readonly int BaseTexture = Shader.PropertyToID("_BaseTexture");

        public Transform head;

        public TweenSettings<Vector3> popFeedbackSettings;

        [Header("BodyParts")]
        public CharacterFeatures features;

        [Header("Emotions")]
        public CharacterEmotions emotions;

        public CharacterMovementBehaviour movementBehaviour;

        public ItemSO test_item;
        public CharacterAnimations animations;


        private GameManager _gameManager;
        private AudioManager _audioManager;
        private Tween _popTween;


        private Material _skinMaterial;

        public int hairId;
        public int eyeId;
        public int mouthId;
        public int outfitId;
        public int noseId;
        public int color;

        private void Start()
        {
            _gameManager = GameManager.GetInstance();
            _audioManager = AudioManager.Instance;

            //choose a random skin color
            _skinMaterial = features.possibleSkinMaterials.OrderBy((x) => Guid.NewGuid()).First();
            Debug.Log("Trying to parse: " + "0x" + ColorUtility.ToHtmlStringRGB(_skinMaterial.color));
            color = int.Parse(ColorUtility.ToHtmlStringRGB(_skinMaterial.color),
                System.Globalization.NumberStyles.HexNumber);
            features.skinRenderers.ForEach(x => x.sharedMaterial = _skinMaterial);

            _gameManager.characterDone.AddListener(() =>
            {
                //only do this if we are in the front row
                if (!movementBehaviour.isInFrontRow) return;
                if (_gameManager.currentCharacter == this) return;
                // emotions.TriggerHappyEmote(withParticles: false);
                animations.TriggerWaveSingleHand();
            });
        }


        void OnCollisionEnter(Collision collision)
        {
            var mask = LayerMask.GetMask("Ground");
            if ((mask & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer &&
                collision.relativeVelocity.magnitude > 2f)
            {
                _audioManager.PlaySound("Thump");
                animations.HitGround();
            }
        }

        public void LookForward()
        {
            head.transform.localRotation = Quaternion.identity;
        }

        public void RotateTowardsCamera()
        {
            Vector3 relativePos = Camera.main.transform.position - transform.position;

            // 1. Get the camera's position
            Vector3 targetPosition = Camera.main.transform.position;

            // 2. Force the target's Y position to match this object's Y position
            targetPosition.y = transform.position.y;

            // 3. Look at the modified target position
            transform.LookAt(targetPosition);
            return;

            // the second argument, upwards, defaults to Vector3.up
            Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            transform.rotation = Quaternion.Euler(0, rotation.y, 0);

            // transform.LookAt(Camera.main.transform);
        }

        [Button]
        public void TestItem()
        {
            ApplyItem(test_item);
        }

        public void ApplyItem(ItemSO item)
        {
            switch (item.equipmentType)
            {
                case EquipmentType.Eyes:
                    features.eyesPlane.materials[0].SetTexture(BaseTexture, item.sprite.texture);
                    emotions.currentEyesItem = item;
                    eyeId = int.Parse(item.name.Split("_")[item.name.Split("_").Length - 1]);
                    break;
                case EquipmentType.Mouth:
                    features.mouthPlane.materials[0].SetTexture(BaseTexture, item.sprite.texture);
                    emotions.currentMouthItem = item;
                    mouthId = int.Parse(item.name.Split("_")[item.name.Split("_").Length - 1]);
                    break;
                case EquipmentType.Headwear:
                    features.headwear.sharedMaterials = item.materials.ToArray();
                    CheckAndReplaceSkinPlaceholder(features.headwear);
                    // features.headwearMesh.mesh = item.mesh;
                    features.headwear.sharedMesh = item.mesh;
                    hairId = int.Parse(item.name.Split("_")[item.name.Split("_").Length - 1]);
                    break;
                case EquipmentType.Nose:
                    features.nose.sharedMaterials = item.materials.ToArray();
                    CheckAndReplaceSkinPlaceholder(features.nose);
                    // features.noseMesh.mesh = item.mesh;
                    features.nose.sharedMesh = item.mesh;
                    noseId = int.Parse(item.name.Split("_")[item.name.Split("_").Length - 1]);
                    break;
                case EquipmentType.Outfit:
                    features.clothing.sharedMaterials = item.materials.ToArray();
                    CheckAndReplaceSkinPlaceholder(features.clothing);
                    // features.clothingMesh.mesh = item.mesh;
                    features.clothing.sharedMesh = item.mesh;
                    outfitId = int.Parse(item.name.Split("_")[item.name.Split("_").Length - 1]);
                    break;
            }

            DoScaleFeedback();
            _audioManager.PlaySound("Pop");
            emotions.TriggerHappyEmote(1.5f, true);
        }

        public void DoScaleFeedback(float delay = 0f, Action OnComplete = null)
        {
            if (_popTween.isAlive) _popTween.Complete();
            Tween.Delay(delay).OnComplete(() =>
            {
                _popTween = Tween.Scale(transform, popFeedbackSettings)
                    .OnComplete(OnComplete);
            });
        }

        private void CheckAndReplaceSkinPlaceholder(SkinnedMeshRenderer renderer)
        {
            Material[] mats = renderer.sharedMaterials;
            for (int i = 0; i < mats.Length; i++)
            {
                if (mats[i].name == features.skinPlaceholder.name)
                    mats[i] = _skinMaterial;
            }

            renderer.materials = mats;
        }
    }
}