using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "RhombusValidator", menuName = "Validators/Rhombus")]
public class RhombusValidator : ShapeValidator
{
    public override float CheckShape(List<Vector2> drawnPoints)
    {
        if (drawnPoints == null || drawnPoints.Count < 10) return 0f;

        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;

        Vector2 top = drawnPoints[0];
        Vector2 bottom = drawnPoints[0];
        Vector2 left = drawnPoints[0];
        Vector2 right = drawnPoints[0];

        foreach (Vector2 p in drawnPoints)
        {
            if (p.x < minX) { minX = p.x; left = p; }
            if (p.x > maxX) { maxX = p.x; right = p; }
            if (p.y < minY) { minY = p.y; bottom = p; }
            if (p.y > maxY) { maxY = p.y; top = p; }
        }

        float width = maxX - minX;
        float height = maxY - minY;
        float maxDim = Mathf.Max(width, height);

        if (maxDim <= 0.01f) return 0f;

        Vector2 center = new Vector2((minX + maxX) * 0.5f, (minY + maxY) * 0.5f);
        float topOffsetX = Mathf.Abs(top.x - center.x) / maxDim;
        float bottomOffsetX = Mathf.Abs(bottom.x - center.x) / maxDim;
        float leftOffsetY = Mathf.Abs(left.y - center.y) / maxDim;
        float rightOffsetY = Mathf.Abs(right.y - center.y) / maxDim;

        float avgOffsetError = (topOffsetX + bottomOffsetX + leftOffsetY + rightOffsetY) / 4f;
        float vertexPositionScore = Mathf.Clamp01(1f - (avgOffsetError / 0.2f));

        float totalEdgeError = 0f;
        foreach (Vector2 p in drawnPoints)
        {
            float distTR = DistanceToSegment(p, top, right);
            float distRB = DistanceToSegment(p, right, bottom);
            float distBL = DistanceToSegment(p, bottom, left);
            float distLT = DistanceToSegment(p, left, top);

            float closestEdgeDist = Mathf.Min(Mathf.Min(distTR, distRB), Mathf.Min(distBL, distLT));
            totalEdgeError += closestEdgeDist;
        }

        float avgEdgeError = totalEdgeError / drawnPoints.Count;
        float normalizedEdgeError = avgEdgeError / maxDim;
        float edgeScore = Mathf.Clamp01(1f - (normalizedEdgeError / 0.12f));

        float endDistance = Vector2.Distance(drawnPoints[0], drawnPoints[drawnPoints.Count - 1]);
        float closureScore = Mathf.Clamp01(1f - (endDistance / (maxDim * 0.25f)));

        float shapeScore = (vertexPositionScore * 0.35f) + (edgeScore * 0.65f);
        float finalScore = (shapeScore * 0.8f) + (closureScore * 0.2f);

        return Mathf.Clamp(finalScore * 100f, 0f, 100f);
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