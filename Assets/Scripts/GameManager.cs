using EditorAttributes;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _gameManager;

    public Transform dropInPoint;
    public Character characterPrefab;
    public CrowdManager crowdManager;

    public Character currentCharacter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_gameManager == null)
        {
            _gameManager = this;
        }
        else
        {
            Destroy(gameObject);
        }

        currentCharacter = Instantiate(characterPrefab, dropInPoint.position, dropInPoint.rotation);
    }

    public static GameManager GetInstance()
    {
        return _gameManager;
    }

    [Button]
    public void NextCharacter()
    {
        //make character move to empty slot
        currentCharacter.MoveTo(crowdManager.GetFirstRowPosition(), () =>
        {
            //drop in a new character
            currentCharacter = Instantiate(characterPrefab, dropInPoint.position, dropInPoint.rotation);
        });
    }
}