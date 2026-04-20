using System;
using Audio;
using EditorAttributes;
using PrimeTween;
using UnityEngine;

[Serializable]
public class CharacterFeatures
{
    public MeshRenderer eyesPlane;
    public MeshRenderer mouthPlane;
    public MeshRenderer headwear;
    public MeshFilter headwearMesh;
    public MeshRenderer nose;
    public MeshFilter noseMesh;
    public MeshRenderer clothing;
    public MeshFilter clothingMesh;
}

[RequireComponent(typeof(Rigidbody))]
public class Character : MonoBehaviour
{
    private static readonly int BaseTexture = Shader.PropertyToID("_BaseTexture");

    public bool shouldLookAtTarget = false;
    public Transform head;
    public Transform lookAtTarget;

    public CharacterAvoidance avoidance;

    public float walkDuration = 1f;

    public TweenSettings<Vector3> popFeedbackSettings;

    [Header("BodyParts")]
    public CharacterFeatures features;

    public ItemSO test_item;


    private Rigidbody _rigidbody;
    private GameManager _gameManager;
    private AudioManager _audioManager;
    private Tween _popTween;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _gameManager = GameManager.GetInstance();
        _audioManager = AudioManager.Instance;
        avoidance.enableAvoidance = false;
    }

    public void MoveTo(Vector3 worldPos, Action onComplete)
    {
        transform.LookAt(worldPos);
        Tween.RigidbodyMovePosition(_rigidbody, worldPos, walkDuration, Ease.InOutSine)
            .OnComplete(() =>
            {
                // Rotate the Rigidbody so the character faces the GameManager but only on the Y axis (yaw)
                Vector3 direction = _gameManager.transform.position - transform.position;
                direction.y = 0f; // remove vertical component so we only rotate around Y

                if (direction.sqrMagnitude > 0.0001f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    // Zero out pitch and roll to ensure rotation only on Y
                    targetRotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);

                    transform.rotation = targetRotation;
                }

                shouldLookAtTarget = true;
                onComplete?.Invoke();
                //after 2sec of reaching the target this character starts avoiding
                Tween.Custom(0f, 1f, 2f, f => { }).OnComplete(() => { avoidance.enableAvoidance = true; });
            });
    }

    [Button]
    public void TestItem()
    {
        ApplyItem(test_item);
    }

    public void ApplyItem(ItemSO item)
    {
        if (_popTween.isAlive) _popTween.Complete();
        _popTween = Tween.Scale(transform, popFeedbackSettings);
        _audioManager.PlaySound("Pop");

        switch (item.equipmentType)
        {
            case EquipmentType.Eyes:
                features.eyesPlane.material.SetTexture(BaseTexture, item.sprite.texture);
                break;
            case EquipmentType.Mouth:
                features.mouthPlane.material.SetTexture(BaseTexture, item.sprite.texture);
                break;
            case EquipmentType.Headwear:
                features.headwear.materials = item.materials.ToArray();
                features.headwearMesh.mesh = item.mesh;
                break;
            case EquipmentType.Nose:
                features.nose.materials = item.materials.ToArray();
                features.noseMesh.mesh = item.mesh;
                break;
            case EquipmentType.Clothing:
                features.clothing.materials = item.materials.ToArray();
                features.clothingMesh.mesh = item.mesh;
                break;
        }
    }

    private void Update()
    {
        if (shouldLookAtTarget)
            head.transform.LookAt(_gameManager.currentCharacter.head.transform);
    }

    void OnCollisionEnter(Collision collision)
    {
        var mask = LayerMask.GetMask("Ground");
        if ((mask & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer &&
            collision.relativeVelocity.magnitude > 2f)
        {
            _audioManager.PlaySound("Thump");
        }
    }
}