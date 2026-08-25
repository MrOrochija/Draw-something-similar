using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "HeartValidator", menuName = "Validators/Heart")]
public class HeartValidator : ShapeValidator
{
    public override float CheckShape(List<Vector2> drawnPoints)
    {
        if (drawnPoints == null || drawnPoints.Count < 15) return 0f;

        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;

        foreach (Vector2 p in drawnPoints)
        {
            if (p.x < minX) minX = p.x;
            if (p.x > maxX) maxX = p.x;
            if (p.y < minY) minY = p.y;
            if (p.y > maxY) maxY = p.y;
        }

        float width = maxX - minX;
        float height = maxY - minY;
        if (width <= 0.01f || height <= 0.01f) return 0f;

        float endDistance = Vector2.Distance(drawnPoints[0], drawnPoints[drawnPoints.Count - 1]);
        float maxDim = Mathf.Max(width, height);
        float closureScore = Mathf.Clamp01(1f - (endDistance / (maxDim * 0.35f)));

        float topDipY = float.MaxValue;
        float leftPeakY = float.MinValue;
        float rightPeakY = float.MinValue;
        float bottomTipY = float.MaxValue;
        float bottomTipX = 0f;

        foreach (Vector2 p in drawnPoints)
        {
            float normX = (p.x - minX) / width;
            float normY = (p.y - minY) / height;

            if (normX >= 0.1f && normX <= 0.45f)
                leftPeakY = Mathf.Max(leftPeakY, normY);

            if (normX >= 0.55f && normX <= 0.9f)
                rightPeakY = Mathf.Max(rightPeakY, normY);

            if (normX >= 0.35f && normX <= 0.65f && normY > 0.3f)
                topDipY = Mathf.Min(topDipY, normY);

            if (normY < bottomTipY)
            {
                bottomTipY = normY;
                bottomTipX = normX;
            }
        }

        if (topDipY == float.MaxValue || leftPeakY == float.MinValue || rightPeakY == float.MinValue)
            return 0f;

        float minPeak = Mathf.Min(leftPeakY, rightPeakY);
        float dipDepth = minPeak - topDipY; 
        float dipScore = Mathf.Clamp01(dipDepth / 0.12f);

        float bottomCenterScore = Mathf.Clamp01(1f - (Mathf.Abs(bottomTipX - 0.5f) / 0.25f));

        float symmetryScore = Mathf.Clamp01(1f - (Mathf.Abs(leftPeakY - rightPeakY) / 0.25f));

        float shapeScore = (dipScore * 0.4f) + (bottomCenterScore * 0.35f) + (symmetryScore * 0.25f);
        float finalScore = (shapeScore * 0.75f) + (closureScore * 0.25f);

        return Mathf.Clamp(finalScore * 100f, 0f, 100f);
    }
}