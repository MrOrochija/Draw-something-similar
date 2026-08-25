using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CheckmarkValidator", menuName = "Validators/Checkmark")]
public class CheckmarkValidator : ShapeValidator
{
    public override float CheckShape(List<Vector2> drawnPoints)
    {
        if (drawnPoints == null || drawnPoints.Count < 6) return 0f;

        int lowestIndex = 0;
        float minY = float.MaxValue;

        for (int i = 0; i < drawnPoints.Count; i++)
        {
            if (drawnPoints[i].y < minY)
            {
                minY = drawnPoints[i].y;
                lowestIndex = i;
            }
        }

        if (lowestIndex <= 0 || lowestIndex >= drawnPoints.Count - 1) return 0f;

        Vector2 start = drawnPoints[0];
        Vector2 vertex = drawnPoints[lowestIndex];
        Vector2 end = drawnPoints[drawnPoints.Count - 1];

        float xOrderScore = (start.x < vertex.x && vertex.x < end.x) ? 1f : 0.1f;

        float leftLegLen = Vector2.Distance(start, vertex);
        float rightLegLen = Vector2.Distance(vertex, end);

        if (leftLegLen <= 0.001f || rightLegLen <= 0.001f) return 0f;

        float heightDiff = end.y - start.y;
        float heightScore = Mathf.Clamp01((heightDiff / leftLegLen) + 0.5f);

        Vector2 dirLeft = (start - vertex).normalized;
        Vector2 dirRight = (end - vertex).normalized;
        float angle = Vector2.Angle(dirLeft, dirRight);
        float angleScore = Mathf.Clamp01(1f - (Mathf.Abs(angle - 60f) / 35f));

        float leftError = CalculateSegmentError(drawnPoints, 0, lowestIndex, start, vertex);
        float rightError = CalculateSegmentError(drawnPoints, lowestIndex, drawnPoints.Count - 1, vertex, end);
        float avgLegLen = (leftLegLen + rightLegLen) / 2f;
        float straightnessScore = Mathf.Clamp01(1f - ((leftError + rightError) / (avgLegLen * 0.15f)));

        float finalScore = (xOrderScore * 0.25f) + 
                           (angleScore * 0.25f) + 
                           (heightScore * 0.25f) + 
                           (straightnessScore * 0.25f);

        return Mathf.Clamp(finalScore * 100f, 0f, 100f);
    }

    private float CalculateSegmentError(List<Vector2> points, int startIndex, int endIndex, Vector2 a, Vector2 b)
    {
        float totalDist = 0f;
        int count = 0;
        for (int i = startIndex; i <= endIndex; i++)
        {
            totalDist += DistanceToSegment(points[i], a, b);
            count++;
        }
        return count > 0 ? totalDist / count : 0f;
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