using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TriangleValidator", menuName = "Validators/Triangle")]
public class TriangleValidator : ShapeValidator
{
    public override float CheckShape(List<Vector2> drawnPoints)
    {
        if (drawnPoints == null || drawnPoints.Count < 10) return 0f;

        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;
        Vector2 centroid = Vector2.zero;

        foreach (Vector2 p in drawnPoints)
        {
            if (p.x < minX) minX = p.x;
            if (p.x > maxX) maxX = p.x;
            if (p.y < minY) minY = p.y;
            if (p.y > maxY) maxY = p.y;
            centroid += p;
        }
        centroid /= drawnPoints.Count;

        float diagonal = Vector2.Distance(new Vector2(minX, minY), new Vector2(maxX, maxY));
        if (diagonal <= 0.001f) return 0f;

        Vector2 v1 = GetFurthestPoint(drawnPoints, centroid);
        Vector2 v2 = GetFurthestPoint(drawnPoints, v1);
        Vector2 v3 = GetFurthestPointFromLine(drawnPoints, v1, v2);

        float triangleHeight = DistanceToSegment(v3, v1, v2);
        float baseLength = Vector2.Distance(v1, v2);
        
        if (triangleHeight < baseLength * 0.15f) return 0f;

        float totalError = 0f;
        foreach (Vector2 p in drawnPoints)
        {
            float distToSide1 = DistanceToSegment(p, v1, v2);
            float distToSide2 = DistanceToSegment(p, v2, v3);
            float distToSide3 = DistanceToSegment(p, v3, v1);

            float minError = Mathf.Min(distToSide1, Mathf.Min(distToSide2, distToSide3));
            totalError += minError;
        }

        float avgError = totalError / drawnPoints.Count;
        float normalizedError = avgError / diagonal;

        float shapeScore = Mathf.Clamp01(1f - (normalizedError / 0.1f));

        float endDistance = Vector2.Distance(drawnPoints[0], drawnPoints[drawnPoints.Count - 1]);
        float closureScore = Mathf.Clamp01(1f - (endDistance / (diagonal * 0.2f)));

        float finalScore = (shapeScore * 0.7f) + (closureScore * 0.3f);

        return Mathf.Clamp(finalScore * 100f, 0f, 100f);
    }

    private Vector2 GetFurthestPoint(List<Vector2> points, Vector2 fromPoint)
    {
        Vector2 furthest = points[0];
        float maxDist = -1f;

        foreach (Vector2 p in points)
        {
            float dist = Vector2.SqrMagnitude(p - fromPoint);
            if (dist > maxDist)
            {
                maxDist = dist;
                furthest = p;
            }
        }
        return furthest;
    }

    private Vector2 GetFurthestPointFromLine(List<Vector2> points, Vector2 lineStart, Vector2 lineEnd)
    {
        Vector2 furthest = points[0];
        float maxDist = -1f;

        foreach (Vector2 p in points)
        {
            float dist = DistanceToSegment(p, lineStart, lineEnd);
            if (dist > maxDist)
            {
                maxDist = dist;
                furthest = p;
            }
        }
        return furthest;
    }

    private float DistanceToSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        Vector2 ap = p - a;

        if (ab.sqrMagnitude == 0f) return ap.magnitude;

        float t = Mathf.Clamp01(Vector2.Dot(ap, ab) / ab.sqrMagnitude);
        Vector2 projection = a + t * ab;

        return Vector2.Distance(p, projection);
    }
}