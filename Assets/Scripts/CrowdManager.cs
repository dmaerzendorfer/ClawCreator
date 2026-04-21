using System.Collections.Generic;
using EditorAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class CrowdManager : MonoBehaviour
{
    // creates the desire postions of the first row of the crowd
    //holds references to all the crowd characters

    public float baseRadius = 5f;
    public float arcAngle = 140f; // degrees (less than 180 looks nicer)
    public int slots = 8;

    [Header("Shape")]
    public float width = 1.2f; // ellipse stretch (X)

    public float depth = 0.8f; // ellipse squash (Z)

    [Header("Noise")]
    public float positionNoise = 0.1f;

    [ReadOnly]
    public List<Transform> firstRowPositions = new List<Transform>();

    private int _currentSlotIndex = 0;


    private void OnValidate()
    {
        GenerateSlots();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        foreach (var slot in firstRowPositions)
        {
            Gizmos.DrawSphere(slot.position, 0.1f);
        }
    }

    public Vector3 GetFirstRowPosition()
    {
        var x = firstRowPositions[_currentSlotIndex];
        _currentSlotIndex+=2;
        _currentSlotIndex %= firstRowPositions.Count;

        return x.position + new Vector3(Random.Range(-positionNoise, positionNoise), 0,
            Random.Range(-positionNoise, positionNoise));
        ;
    }


    private void GenerateSlots()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var thing = transform.GetChild(i).gameObject;

            if (Application.isPlaying) Destroy(thing);
            else
            {
                UnityEditor.EditorApplication.delayCall += () => { DestroyImmediate(thing); };
            }
        }

        firstRowPositions.Clear();

        Vector3 center = transform.position;

        for (int i = 0; i < slots; i++)
        {
            float angle = -arcAngle / 2 + i * (arcAngle / (slots - 1));
            float rad = Mathf.Deg2Rad * angle;
            float x = Mathf.Sin(rad) * baseRadius * width;
            float z = Mathf.Cos(rad) * baseRadius * depth;

            Vector3 position = center + new Vector3(x, 0, z);

            GameObject slotObj = new GameObject($"Slot_{i}");
            slotObj.transform.SetParent(transform);
            slotObj.transform.position = position;
            slotObj.transform.rotation = Quaternion.LookRotation(center - position);
            slotObj.transform.parent = transform;

            firstRowPositions.Add(slotObj.transform);
        }
    }
}