using EditorAttributes;
using PrimeTween;
using UnityEngine;
using Random = UnityEngine.Random;
using Void = EditorAttributes.Void;

namespace Crowd
{
    public class CharacterEmotions : MonoBehaviour
    {
        private static readonly int BaseTexture = Shader.PropertyToID("_BaseTexture");

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

        public Character character;

        //emotion related fields
        [HideInInspector]
        public ItemSO currentEyesItem;

        [HideInInspector]
        public ItemSO currentMouthItem;

        private Tween _happyEmoteTween;
        private Sequence _happyEmoteSequence;
        private Sequence _blinkSequence;


        private void Start()
        {
            happyParticles.Stop(true);
            StartBlinkingSequence();
            Random.InitState((int)System.DateTime.Now.Ticks);
        }

        public void TriggerHappyEmote(float? duration = null, bool once = false)
        {
            if (_happyEmoteTween.isAlive) _happyEmoteTween.Complete(); 
            
            if (duration == null) duration = happyEmoteDuration;
            _blinkSequence.Stop(); //no blinking while happy
            _happyEmoteSequence.Complete(); //also no random happiness during already being happy

            happyParticles.Play(true);

            if (currentEyesItem != null)
                character.features.eyesPlane.material.SetTexture(BaseTexture, currentEyesItem.happySprite.texture);
            if (currentMouthItem != null)
                character.features.mouthPlane.material.SetTexture(BaseTexture, currentMouthItem.happySprite.texture);

            _happyEmoteTween = Tween.Delay(duration.Value).OnComplete(() =>
            {
                //revert to normal face
                if (currentEyesItem != null)
                    character.features.eyesPlane.material.SetTexture(BaseTexture, currentEyesItem.sprite.texture);
                if (currentMouthItem != null)
                    character.features.mouthPlane.material.SetTexture(BaseTexture, currentMouthItem.sprite.texture);
                //resume blinking and random happiness
                StartBlinkingSequence();
                if (!once)
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
                    if (currentEyesItem != null)
                        character.features.eyesPlane.material.SetTexture(BaseTexture, currentEyesItem.sprite.texture);
                }) //set eyes to normal
                .Chain(Tween.Delay(Random.Range(timeBetweenBlinks.x, timeBetweenBlinks.y)).OnComplete(() =>
                        {
                            if (currentEyesItem != null)
                                character.features.eyesPlane.material.SetTexture(BaseTexture,
                                    currentEyesItem.happySprite.texture);
                        }) //wait random time, then sest to blink
                        .Chain(Tween.Delay(blinkDuration)).OnComplete(() =>
                        {
                            if (currentEyesItem != null)
                                character.features.eyesPlane.material.SetTexture(BaseTexture,
                                    currentEyesItem.sprite.texture);
                        }) //change back to normal sprite after blink duration
                );
        }
    }
}