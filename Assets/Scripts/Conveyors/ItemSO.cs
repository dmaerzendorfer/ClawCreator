using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public Sprite sprite;
    public GameObject prefab;
    public EquipmentType equipmentType;
}

public enum EquipmentType {
    Headwear,
    Eyes,
    Nose,
    Mouth,
    Clothing,
    Ears
}
