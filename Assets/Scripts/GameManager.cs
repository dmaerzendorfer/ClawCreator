using Audio;
using EditorAttributes;
using PrimeTween;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _gameManager;

    public AudioManager audioManager;
    public Transform dropInPoint;
    public Character characterPrefab;
    public CrowdManager crowdManager;
    public Character currentCharacter;

    [Header("ClawSettings")]
    public Claw claw;

    public int grabAmounts = 3;


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

        currentCharacter = Instantiate(characterPrefab, dropInPoint.position, dropInPoint.rotation);

        claw.SetClawText(grabAmounts.ToString());
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
        //make character move to empty slot
        currentCharacter.MoveTo(crowdManager.GetFirstRowPosition(), () =>
        {
            //drop in a new character
            currentCharacter = Instantiate(characterPrefab, dropInPoint.position, dropInPoint.rotation);
            //reset grab amount
            _grabsLeft = grabAmounts;
            claw.SetClawText(_grabsLeft.ToString());
            claw.canGrab = true;
            audioManager.PlaySound("Yay");
            //todo add visual feedback eG pop of claw or smth
        });
    }
}