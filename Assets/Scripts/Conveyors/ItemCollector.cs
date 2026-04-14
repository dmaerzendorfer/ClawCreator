using System;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    
    private List<ItemSO> _recentItems = new List<ItemSO>();
    [SerializeField] private GameObject character;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball") && other.transform.localScale.x < 1)
        {
            Debug.Log("Collided with dropped ball trigger!");
            Destroy(other.gameObject);
            _recentItems.Add(other.gameObject.GetComponent<CapsuleScript>().GetItem());
            // TODO - you can also just do some other logic here with the other.gameObject.GetComponent<CapsuleScript>().GetItem() line.
            Sequence.Create().ChainDelay(1).OnComplete(() =>
            {
                _recentItems.Clear();
            });
        }
    }
}
