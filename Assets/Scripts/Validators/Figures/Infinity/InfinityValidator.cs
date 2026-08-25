using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "InfinityValidator", menuName = "Validators/Infinity")]
public class InfinityValidator : ShapeValidator
{
    public override float CheckShape(List<Vector2> drawnPoints)
    {
        if (drawnPoints == null || drawnPoints.Count < 20) return 0f;

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

        Vector2 center = new Vector2((minX + maxX) * 0.5f, (minY + maxY) * 0.5f);

        float aspectRatio = width / height;
        float aspectScore = Mathf.Clamp01(1f - (Mathf.Abs(aspectRatio - 2.0f) / 1.2f));

        float minCenterDistMid = float.MaxValue;
        int startIndex = drawnPoints.Count / 5;
        int endIndex = (drawnPoints.Count * 4) / 5;

        for (int i = startIndex; i < endIndex; i++)
        {
            float dist = Vector2.Distance(drawnPoints[i], center);
            if (dist < minCenterDistMid)
            {
                minCenterDistMid = dist;
            }
        }

        float centerCrossScore = Mathf.Clamp01(1f - (minCenterDistMid / (width * 0.20f)));

        int leftCount = 0;
        int rightCount = 0;

        foreach (Vector2 p in drawnPoints)
        {
            if (p.x < center.x) leftCount++;
            else rightCount++;
        }

        float leftRatio = (float)leftCount / drawnPoints.Count;
        float balanceScore = Mathf.Clamp01(1f - (Mathf.Abs(leftRatio - 0.5f) / 0.25f));

        float endDistance = Vector2.Distance(drawnPoints[0], drawnPoints[drawnPoints.Count - 1]);
        float closureScore = Mathf.Clamp01(1f - (endDistance / (width * 0.35f)));

        float shapeScore = (aspectScore * 0.25f) + (centerCrossScore * 0.45f) + (balanceScore * 0.3f);
        float finalScore = (shapeScore * 0.8f) + (closureScore * 0.2f);

        return Mathf.Clamp(finalScore * 100f, 0f, 100f);
    }
}