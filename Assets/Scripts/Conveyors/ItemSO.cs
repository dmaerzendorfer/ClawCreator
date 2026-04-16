using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public Sprite sprite;
    public Mesh mesh;
    public List<Material> materials;
    public EquipmentType equipmentType;
}

public enum EquipmentType
{
    Headwear,
    Eyes,
    Nose,
    Mouth,
    Clothing,
    Ears
}