using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LineValidator", menuName = "Validators/Line")]
public class LineValidator : ShapeValidator
{
    public override float CheckShape(List<Vector2> drawnPoints)
    {
        if (drawnPoints == null || drawnPoints.Count < 5) return 0f;

        Vector2 start = drawnPoints[0];
        Vector2 end = drawnPoints[drawnPoints.Count - 1];
        float directDistance = Vector2.Distance(start, end);

        if (directDistance < 0.05f) return 0f;

        float totalPathLength = 0f;
        float totalDeviation = 0f;

        for (int i = 0; i < drawnPoints.Count; i++)
        {
            if (i > 0)
                totalPathLength += Vector2.Distance(drawnPoints[i - 1], drawnPoints[i]);

            totalDeviation += DistanceToSegment(drawnPoints[i], start, end);
        }

        float straightnessScore = Mathf.Clamp01(directDistance / totalPathLength);

        float avgDeviation = totalDeviation / drawnPoints.Count;
        float normalizedDeviation = avgDeviation / directDistance;
        float deviationScore = Mathf.Clamp01(1f - (normalizedDeviation / 0.1f));

        float finalScore = (straightnessScore * 0.5f) + (deviationScore * 0.5f);
        return Mathf.Clamp(finalScore * 100f, 0f, 100f);
    }

    private float DistanceToSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        Vector2 ap = p - a;
        if (ab.sqrMagnitude == 0f) return ap.magnitude;
        
        float t = Mathf.Clamp01(Vector2.Dot(ap, ab) / ab.sqrMagnitude);
        return Vector2.Distance(p, a + t * ab);
    }
}