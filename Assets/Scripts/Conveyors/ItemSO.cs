using System.Collections.Generic;
using EditorAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
    [AssetPreview(64f,64f)]
    public Sprite sprite;
    [AssetPreview(64f,64f)]
    public Sprite happySprite;
    [AssetPreview(64f,64f)]
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