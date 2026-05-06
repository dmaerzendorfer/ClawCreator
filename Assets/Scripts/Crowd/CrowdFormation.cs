using System.Collections.Generic;
using UnityEngine;

namespace Crowd
{
    [System.Serializable]
    public class CrowdFormationSettings
    {
        public int frontRowCapacity = 5;
        public float rowSpacing = 1.5f;
        public float baseRadius = 2.5f;
        public float angleRange = 120f;

        [Header("Randomness")]
        public float positionJitter = 0.15f;

        public float radiusJitter = 0.2f;
        public float angleJitter = 5f;

        [Header("Row Variation")]
        public float rowJitterMultiplier = 0.1f;
        // how much extra chaos per row
    }

    public static class CrowdFormation
    {
        public static List<Vector3> GenerateSlots(
            Vector3 center,
            Vector3 forward,
            int agentCount,
            CrowdFormationSettings settings)
        {
            List<Vector3> slots = new List<Vector3>(agentCount);

            int remaining = agentCount;
            int row = 0;

            while (remaining > 0)
            {
                int rowCapacity = settings.frontRowCapacity + row * 2;
                int countInRow = Mathf.Min(remaining, rowCapacity);

                float baseRadius = settings.baseRadius + row * settings.rowSpacing;
                float angleRange = settings.angleRange + row * 10f;

                // 👇 rowFactor grows per row
                float rowFactor = 1f + row * settings.rowJitterMultiplier;

                for (int i = 0; i < countInRow; i++)
                {
                    // Deterministic randomness per slot
                    // Random.InitState(row * 1000 + i);

                    float t = countInRow == 1 ? 0.5f : (float)i / (countInRow - 1);

                    // Zig-zag
                    // if (row % 2 == 1)
                    //     t = 1f - t;

                    float angle = Mathf.Lerp(-angleRange * 0.5f, angleRange * 0.5f, t);

                    // 👇 apply rowFactor to jitter
                    angle += Random.Range(-settings.angleJitter, settings.angleJitter) * rowFactor;

                    Vector3 dir = Quaternion.AngleAxis(angle, Vector3.up) * forward;

                    float radius = baseRadius + Random.Range(-settings.radiusJitter, settings.radiusJitter) * rowFactor;

                    Vector3 pos = center + dir.normalized * radius;

                    pos += new Vector3(
                        Random.Range(-settings.positionJitter, settings.positionJitter) * rowFactor,
                        0f,
                        Random.Range(-settings.positionJitter, settings.positionJitter) * rowFactor
                    );

                    slots.Add(pos);
                }

                remaining -= countInRow;
                row++;
            }

            return slots;
        }

        public static void AssignSlotsSmart(
            List<CharacterMovementBehaviour> avatars,
            List<Vector3> slots,
            CrowdFormationSettings settings,
            Vector3 center)
        {
            List<Vector3> availableSlots = new List<Vector3>(slots);
            bool first = true;

            foreach (var mini in avatars)
            {
                if (first)
                {
                    //first character always gets a front row slot
                    int index = Random.Range(0, Mathf.Min(settings.frontRowCapacity, availableSlots.Count));
                    mini.isInFrontRow = true;
                    mini.SetTarget(availableSlots[index]);
                    availableSlots.RemoveAt(index);
                    first = false;
                    continue;
                }

                //others fight for good slots
                float bestScore = float.MaxValue;
                int bestIndex = -1;

                for (int i = 0; i < availableSlots.Count; i++)
                {
                    Vector3 slot = availableSlots[i];

                    float dist = (mini.transform.position - slot).sqrMagnitude;
                    float distToCenter = (slot - center).sqrMagnitude;

                    bool isFrontRowSlot = i <= settings.frontRowCapacity - 1;

                    //search for the lowest score
                    float score = dist;

                    // 🟢 New minis want front row
                    if (mini.IsNew && isFrontRowSlot)
                    {
                        score *= 0.5f;
                    }

                    // 🔴 Penalize staying too long in front row
                    if (isFrontRowSlot)
                    {
                        score += mini.timeInFrontRow * 2f;
                    }

                    // slight outward bias for older minis
                    if (!mini.IsNew)
                    {
                        score += distToCenter * 0.3f;
                    }

                    if (score < bestScore)
                    {
                        bestScore = score;
                        bestIndex = i;
                    }
                }

                if (bestIndex != -1)
                {
                    //check if this is a front row slot
                    bool isFrontRowSlot = slots.GetRange(0, Mathf.Min(settings.frontRowCapacity - 1, slots.Count - 1))
                        .Contains(availableSlots[bestIndex]);
                    mini.isInFrontRow = isFrontRowSlot;
                    mini.SetTarget(availableSlots[bestIndex]);
                    availableSlots.RemoveAt(bestIndex);
                }
            }
        }
    }
}