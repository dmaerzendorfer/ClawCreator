using System;
using Audio;
using EditorAttributes;
using PrimeTween;
using UnityEngine;

public class PolaroidAnimations : MonoBehaviour
{
    public TweenSettings<Vector3> flyInPositionSettings;
    public TweenSettings<float> fadeInSettings;
    public TweenSettings<Vector3> flyOutPositionSettings;
    public TweenSettings<Vector3> flyOutRotationSettings;

    [MinMaxSlider(-360, 360)]
    public Vector2 flyOutEndRotationRangeZ;

    public SpriteRenderer fadeSprite;

    private Sequence _flyInSequence;
    private Sequence _flyOutSequence;
    private AudioManager _am;

    private void Start()
    {
        _am = AudioManager.Instance;
    }


    public void SlideIn()
    {
        if (_flyInSequence.isAlive)
        {
            _flyInSequence.Complete();
        }

        fadeSprite.color = new Color(0, 0, 0, 1);
        transform.rotation = Quaternion.Euler(flyOutRotationSettings.startValue);
        transform.position = flyInPositionSettings.startValue;
        _flyInSequence = Sequence.Create()
            .Group(Tween.Position(transform, flyInPositionSettings))
            .Group(Tween.Custom(fadeInSettings, t =>
            {
                var c = fadeSprite.color;
                c.a = t;
                fadeSprite.color = c;
            }));
    }

    public void PlaySound()
    {
        _am.PlaySound("Polaroid");
    }

    public void FallOut(Action onComplete = null)
    {
        PlaySound();
        
        if (_flyInSequence.isAlive)
        {
            _flyInSequence.Complete();
        }

        flyOutRotationSettings.endValue = new Vector3(0, 0,
            UnityEngine.Random.Range(flyOutEndRotationRangeZ.x, flyOutEndRotationRangeZ.y));

        _flyInSequence = Sequence.Create()
            .Group(Tween.Position(transform, flyOutPositionSettings))
            .Group(Tween.Rotation(transform, flyOutRotationSettings))
            .OnComplete(onComplete);
    }
}