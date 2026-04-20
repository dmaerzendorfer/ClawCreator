using System.Collections.Generic;
using EditorAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public Sprite sprite;
    [AssetPreview]
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