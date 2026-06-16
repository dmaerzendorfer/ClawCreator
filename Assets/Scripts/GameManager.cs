using Audio;
using Crowd;
using EditorAttributes;
using PrimeTween;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    private static GameManager _gameManager;

    public AudioManager audioManager;
    public Character currentCharacter;
    public FormationManager formationManager;

    public UnityEvent characterDone;

    [Header("ClawSettings")]
    public Claw claw;

    public int grabAmounts = 3;

    [SerializeField]
    private QrGenerator qrGen;

    private int _grabsLeft = 3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (_gameManager == null)
        {
            _gameManager = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        claw.SetClawText(grabAmounts.ToString());

        currentCharacter = formationManager.SpawnNextAvatar().GetComponent<Character>();
    }

    public static GameManager GetInstance()
    {
        return _gameManager;
    }

    public void OnGrabComplete()
    {
        _grabsLeft--;
        _grabsLeft = Mathf.Clamp(_grabsLeft, 0, grabAmounts);
        claw.SetClawText(_grabsLeft.ToString());

        if (_grabsLeft <= 0)
        {
            claw.canGrab = false;
            //wait a sec, then do next character
            Sequence.Create().ChainDelay(1).OnComplete(() => { NextCharacter(); });
        }
    }

    [Button]
    public void NextCharacter()
    {
        //first emote
        currentCharacter.emotions.TriggerHappyEmote(withParticles:true);
        audioManager.PlaySound("Tada");
        currentCharacter.animations.TriggerWaveBothHands();
        Tween.Delay(.5f, characterDone.Invoke);
        
        //then wait a bit
        Tween.Delay(2, () =>
        {
            //then let characters move
            //spawn next character
            qrGen.updateQRCode(currentCharacter.hairId, currentCharacter.noseId, currentCharacter.mouthId,
                currentCharacter.eyeId, currentCharacter.outfitId, currentCharacter.color);
            currentCharacter = formationManager.SpawnNextAvatar().GetComponent<Character>();
            //reset grab amount
            _grabsLeft = grabAmounts;
            claw.SetClawText(_grabsLeft.ToString());
            claw.canGrab = true;
        });
    }
}