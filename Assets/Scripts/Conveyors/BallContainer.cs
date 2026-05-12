using System.Collections.Generic;
using UnityEngine;

public class BallContainer : MonoBehaviour
{
    
    [SerializeField] private ItemSO[] items;
    private List<ItemSO> _eyes = new();
    private List<ItemSO> _hairs = new();
    private List<ItemSO> _noses = new();
    private List<ItemSO> _mouths = new();
    private List<ItemSO> _outfits = new();
    private int _eyeIndex = 0;
    private int _hairIndex = 0;
    private int _noseIndex = 0;
    private int _mouthIndex = 0;
    private int _outfitIndex = 0;
    
    // Sta<ItemSO>lled once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (ItemSO item in items)
        {
            if (item.name.StartsWith("Eyes"))
            {
                _eyes.Add(item);
            } else if (item.name.StartsWith("Headwear"))
            {
                _hairs.Add(item);
            }  else if (item.name.StartsWith("Nose")) 
            {
                _noses.Add(item);
            } else if (item.name.StartsWith("Mouth")) 
            {
                _mouths.Add(item);
            }  else if (item.name.StartsWith("Outfit")) 
            {
                _outfits.Add(item);
            }
        }
        Debug.Log("Found " + transform.childCount + " Balls!");
        for (int i = 0; i < transform.childCount; i++)
        {
            CapsuleScript capsule = transform.GetChild(i).GetComponent<CapsuleScript>();
            if (i % 5 == 0)
            {
                capsule.SetItem(_eyes[_eyeIndex]);
                _eyeIndex = (_eyeIndex + 1) % _eyes.Count;
            }
            else if (i % 5 == 1)
            {
                capsule.SetItem(_hairs[_hairIndex]);
                _hairIndex  = (_hairIndex + 1) % _hairs.Count;
            }
            else if (i % 5 == 2) 
            {
                capsule.SetItem(_noses[_noseIndex]);
                _noseIndex  = (_noseIndex + 1) % _noses.Count;
            }
            else if (i % 5 == 3) 
            {
                capsule.SetItem(_mouths[_mouthIndex]);
                _mouthIndex  = (_mouthIndex + 1) % _mouths.Count;
            }
            else if (i % 5 == 4) 
            {
                capsule.SetItem(_outfits[_outfitIndex]);
                _outfitIndex  = (_outfitIndex + 1) % _outfits.Count;
            }
        } 
    }

    public ItemSO GetReplacementItem(ItemSO previousItem)
    {
        if (previousItem.name.StartsWith("Eyes"))
        {
            _eyeIndex = (_eyeIndex + 1) % _eyes.Count;
            return _eyes[_eyeIndex];
        } 
        if (previousItem.name.StartsWith("Headwear"))
        {
            _hairIndex = (_hairIndex + 1) % _hairs.Count;
            return _hairs[_hairIndex];
        }  
        if (previousItem.name.StartsWith("Nose")) 
        {
            _noseIndex = (_noseIndex + 1) % _noses.Count;
            return _noses[_noseIndex];
        } 
        if (previousItem.name.StartsWith("Mouth")) 
        {
            _mouthIndex = (_mouthIndex + 1) % _mouths.Count;
            return _mouths[_mouthIndex];
        }  
        if (previousItem.name.StartsWith("Outfit")) 
        {
            _outfitIndex = (_outfitIndex + 1) % _outfits.Count;
            return _outfits[_outfitIndex];
        }
        return previousItem; // at worst return the same item again
    }
    
}
