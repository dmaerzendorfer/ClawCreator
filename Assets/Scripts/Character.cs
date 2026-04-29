using System;
using Audio;
using EditorAttributes;
using PrimeTween;
using UnityEngine;
using Random = UnityEngine.Random;
using Void = EditorAttributes.Void;

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

    [FoldoutGroup("Emotion Settings", nameof(timeBetweenBlinks), nameof(blinkDuration),
        nameof(timeBetweenHappyEmote),
        nameof(happyEmoteDuration),
        nameof(happyParticles))]
    [SerializeField] private Void groupHolder;

    [SerializeField, HideProperty, MinMaxSlider(0, 10)]
    private Vector2 timeBetweenBlinks = new Vector2(3, 5);

    [SerializeField, HideProperty] private float blinkDuration = .1f;

    [SerializeField, HideProperty, MinMaxSlider(0, 10)]
    private Vector2 timeBetweenHappyEmote = new Vector2(5, 6);

    [SerializeField, HideProperty] private float happyEmoteDuration = 2f;
    [SerializeField, HideProperty] private ParticleSystem happyParticles;


    private Rigidbody _rigidbody;
    private GameManager _gameManager;
    private AudioManager _audioManager;
    private Tween _popTween;

    //emotion related fields
    private ItemSO _currentEyesItem;
    private ItemSO _currentMouthItem;

    private Tween _happyEmoteTween;
    private Sequence _happyEmoteSequence;
    private Sequence _blinkSequence;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _gameManager = GameManager.GetInstance();
        _audioManager = AudioManager.Instance;
        avoidance.enableAvoidance = false;
        
        happyParticles.Stop(true);
        StartBlinkingSequence();
        StartHappySequence();
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

    public void TriggerHappyEmote(float? duration = null)
    {
        if (_happyEmoteTween.isAlive) return; //if already in a happy emote, don't trigger another one

        if (duration == null) duration = happyEmoteDuration;
        _blinkSequence.Complete(); //no blinking while happy
        _happyEmoteSequence.Complete(); //also no random happiness during already being happy

        happyParticles.Play(true);

        if (_currentEyesItem != null)
            features.eyesPlane.material.SetTexture(BaseTexture, _currentEyesItem.happySprite.texture);
        if (_currentMouthItem != null)
            features.mouthPlane.material.SetTexture(BaseTexture, _currentMouthItem.happySprite.texture);

        _happyEmoteTween = Tween.Delay(duration.Value).OnComplete(() =>
        {
            //revert to normal face
            if (_currentEyesItem != null)
                features.eyesPlane.material.SetTexture(BaseTexture, _currentEyesItem.sprite.texture);
            if (_currentMouthItem != null)
                features.mouthPlane.material.SetTexture(BaseTexture, _currentMouthItem.sprite.texture);
            //resume blinking and random happiness
            StartBlinkingSequence();
            StartHappySequence();
        });
    }

    public void StartHappySequence()
    {
        _happyEmoteSequence = Sequence.Create(-1)
            .ChainDelay(Random.Range(timeBetweenHappyEmote.x, timeBetweenHappyEmote.y))
            .ChainCallback(() => { TriggerHappyEmote(); });
    }

    public void StartBlinkingSequence()
    {
        _blinkSequence = Sequence.Create(-1).ChainCallback(() =>
            {
                if (_currentEyesItem != null)
                    features.eyesPlane.material.SetTexture(BaseTexture, _currentEyesItem.sprite.texture);
            }) //set eyes to normal
            .Chain(Tween.Delay(Random.Range(timeBetweenBlinks.x, timeBetweenBlinks.y)).OnComplete(() =>
                    {
                        if (_currentEyesItem != null)
                            features.eyesPlane.material.SetTexture(BaseTexture, _currentEyesItem.happySprite.texture);
                    }) //wait random time, then sest to blink
                    .Chain(Tween.Delay(blinkDuration)).OnComplete(() =>
                    {
                        if (_currentEyesItem != null)
                            features.eyesPlane.material.SetTexture(BaseTexture, _currentEyesItem.sprite.texture);
                    }) //change back to normal sprite after blink duration
            );
    }

    public void MoveTo(Vector3 worldPos, Action onComplete)
    {
        transform.LookAt(worldPos);
        var oldMass = _rigidbody.mass;
        _rigidbody.mass *= 100f;
        Tween.RigidbodyMovePosition(_rigidbody, worldPos, walkDuration, Ease.InOutSine)
            .OnComplete(() =>
            {
                _rigidbody.mass = oldMass;
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
        TriggerHappyEmote(1.5f);

        switch (item.equipmentType)
        {
            case EquipmentType.Eyes:
                features.eyesPlane.material.SetTexture(BaseTexture, item.sprite.texture);
                _currentEyesItem = item;
                break;
            case EquipmentType.Mouth:
                features.mouthPlane.material.SetTexture(BaseTexture, item.sprite.texture);
                _currentMouthItem = item;
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
}