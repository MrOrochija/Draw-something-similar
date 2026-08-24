using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CircleValidator", menuName = "Validators/Circle")]
public class CircleValidator : ShapeValidator
{
    public override float CheckShape(List<Vector2> drawnPoints)
    {
        if (drawnPoints == null || drawnPoints.Count < 10) return 0f;

        Vector2 center = Vector2.zero;
        foreach (Vector2 p in drawnPoints)
        {
            center += p;
        }
        center /= drawnPoints.Count;

        float totalRadius = 0f;
        foreach (Vector2 p in drawnPoints)
        {
            totalRadius += Vector2.Distance(p, center);
        }
        float avgRadius = totalRadius / drawnPoints.Count;

        if (avgRadius <= 0.001f) return 0f;

        float totalRadiusError = 0f;
        foreach (Vector2 p in drawnPoints)
        {
            totalRadiusError += Mathf.Abs(Vector2.Distance(p, center) - avgRadius);
        }
        float avgRadiusError = totalRadiusError / drawnPoints.Count;
        float radiusVariance = avgRadiusError / avgRadius;
        
        float radiusScore = Mathf.Clamp01(1f - (radiusVariance / 0.4f));

        float endDistance = Vector2.Distance(drawnPoints[0], drawnPoints[drawnPoints.Count - 1]);
        float closureScore = Mathf.Clamp01(1f - (endDistance / (avgRadius * 1.5f)));

        float angularCoverageScore = CalculateAngularCoverage(drawnPoints, center);

        float finalScore = (radiusScore * 0.4f) + (closureScore * 0.3f) + (angularCoverageScore * 0.3f);

        return Mathf.Clamp(finalScore * 100f, 0f, 100f);
    }

    private float CalculateAngularCoverage(List<Vector2> points, Vector2 center)
    {
        bool[] sectors = new bool[8];

        foreach (Vector2 p in points)
        {
            Vector2 dir = p - center;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f;

            int sectorIndex = Mathf.FloorToInt(angle / 45f) % 8;
            sectors[sectorIndex] = true;
        }

        int filledSectors = 0;
        foreach (bool filled in sectors)
        {
            if (filled) filledSectors++;
        }

        return Mathf.Clamp01(filledSectors / 7f);
    }
}