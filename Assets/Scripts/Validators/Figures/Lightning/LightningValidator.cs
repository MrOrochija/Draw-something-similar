using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LightningValidator", menuName = "Validators/Lightning")]
public class LightningValidator : ShapeValidator
{
    public override float CheckShape(List<Vector2> drawnPoints)
    {
        if (drawnPoints == null || drawnPoints.Count < 8) return 0f;

        List<int> cornerIndices = new List<int>();
        cornerIndices.Add(0);

        for (int i = 1; i < drawnPoints.Count - 1; i++)
        {
            Vector2 dir1 = (drawnPoints[i] - drawnPoints[i - 1]).normalized;
            Vector2 dir2 = (drawnPoints[i + 1] - drawnPoints[i]).normalized;
            float angle = Vector2.Angle(dir1, dir2);

            if (angle > 50f)
            {
                if (i - cornerIndices[cornerIndices.Count - 1] > 2)
                    cornerIndices.Add(i);
            }
        }
        cornerIndices.Add(drawnPoints.Count - 1);

        int numSegments = cornerIndices.Count - 1;

        float segmentCountScore = Mathf.Clamp01(1f - Mathf.Abs(numSegments - 3.5f) / 1.5f);

        if (numSegments < 2 || numSegments > 5) return 0f;

        float totalTurnAngle = 0f;
        for (int i = 1; i < cornerIndices.Count - 1; i++)
        {
            Vector2 dir1 = (drawnPoints[cornerIndices[i]] - drawnPoints[cornerIndices[i-1]]).normalized;
            Vector2 dir2 = (drawnPoints[cornerIndices[i+1]] - drawnPoints[cornerIndices[i]]).normalized;
            totalTurnAngle += Vector2.Angle(dir1, dir2);
        }
        float avgTurnAngle = numSegments > 1 ? totalTurnAngle / (numSegments - 1) : 0f;
        float angleScore = Mathf.Clamp01((avgTurnAngle - 50f) / 50f);

        float maxLength = 0f;
        float totalLength = 0f;
        List<float> segmentLengths = new List<float>();
        for (int i = 1; i < cornerIndices.Count; i++)
        {
            float len = Vector2.Distance(drawnPoints[cornerIndices[i]], drawnPoints[cornerIndices[i-1]]);
            segmentLengths.Add(len);
            maxLength = Mathf.Max(maxLength, len);
            totalLength += len;
        }

        float lenVarTotal = 0f;
        float avgLen = totalLength / numSegments;
        foreach (float len in segmentLengths) lenVarTotal += Mathf.Abs(len - avgLen);
        float lenVar = lenVarTotal / totalLength;
        float lengthScore = Mathf.Clamp01(1f - (lenVar / 0.8f));

        float finalScore = (segmentCountScore * 0.3f) + (angleScore * 0.4f) + (lengthScore * 0.3f);
        return Mathf.Clamp(finalScore * 100f, 0f, 100f);
    }
}