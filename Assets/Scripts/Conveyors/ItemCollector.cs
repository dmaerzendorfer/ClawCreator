using System;
using System.Collections.Generic;
using System.Linq;
using PrimeTween;
using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    private List<ItemSO> _recentItems = new List<ItemSO>();

    private GameManager _gm;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gm = GameManager.GetInstance();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball") && other.transform.localScale.x < 1)
        {
            Destroy(other.gameObject);
            var item = other.gameObject.GetComponent<CapsuleScript>().GetItem();
            _recentItems.Add(item);
            _gm.currentCharacter.ApplyItem(item);
            Sequence.Create().ChainDelay(1).OnComplete(() => { _recentItems.Clear(); });
        }
    }
}