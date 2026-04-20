using UnityEngine;

public class BallContainer : MonoBehaviour
{
    
    [SerializeField] private ItemSO[] items;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            CapsuleScript capsule = transform.GetChild(i).GetComponent<CapsuleScript>();
            capsule.SetItem(items[i % items.Length]);
        } 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
