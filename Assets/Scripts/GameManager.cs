using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _gameManager;
    
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
    }

    public static GameManager GetInstance()
    {
        return _gameManager;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
