using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ArrowValidator", menuName = "Validators/Arrow")]
public class ArrowValidator : ShapeValidator
{
    public override float CheckShape(List<Vector2> drawnPoints)
    {
        if (drawnPoints == null || drawnPoints.Count < 12) return 0f;

        Vector2 pointA = drawnPoints[0];
        Vector2 pointB = drawnPoints[0];
        float maxDistSqr = -1f;

        for (int i = 0; i < drawnPoints.Count; i++)
        {
            for (int j = i + 1; j < drawnPoints.Count; j++)
            {
                float sqrDist = Vector2.SqrMagnitude(drawnPoints[i] - drawnPoints[j]);
                if (sqrDist > maxDistSqr)
                {
                    maxDistSqr = sqrDist;
                    pointA = drawnPoints[i];
                    pointB = drawnPoints[j];
                }
            }
        }

        float mainAxisLength = Mathf.Sqrt(maxDistSqr);
        if (mainAxisLength <= 0.05f) return 0f;

        Vector2 mainAxis = pointB - pointA;

        float tailWidthSum = 0f;
        int tailCount = 0;
        float headWidthSum = 0f;
        int headCount = 0;
        float maxHeadWidth = 0f;

        foreach (Vector2 p in drawnPoints)
        {
            Vector2 ap = p - pointA;
            float t = Vector2.Dot(ap, mainAxis) / maxDistSqr;
            float distToAxis = DistanceToSegment(p, pointA, pointB);

            if (t < 0.4f)
            {
                tailWidthSum += distToAxis;
                tailCount++;
            }
            else if (t > 0.6f)
            {
                headWidthSum += distToAxis;
                headCount++;
                if (distToAxis > maxHeadWidth) maxHeadWidth = distToAxis;
            }
        }

        if (tailCount == 0 || headCount == 0) return 0f;

        float avgTailWidth = tailWidthSum / tailCount;
        float avgHeadWidth = headWidthSum / headCount;

        if (avgTailWidth > avgHeadWidth)
        {
            float tempAvg = avgTailWidth;
            avgTailWidth = avgHeadWidth;
            avgHeadWidth = tempAvg;
        }

        float normalizedTailWidth = avgTailWidth / mainAxisLength;
        float shaftScore = Mathf.Clamp01(1f - (normalizedTailWidth / 0.08f));

        float headRatio = maxHeadWidth / mainAxisLength;
        float headScore = Mathf.Clamp01(1f - (Mathf.Abs(headRatio - 0.25f) / 0.18f));

        float asymmetryRatio = avgHeadWidth / (avgTailWidth + 0.001f);
        float asymmetryScore = Mathf.Clamp01((asymmetryRatio - 1.5f) / 2.0f);

        float finalScore = (shaftScore * 0.4f) + (headScore * 0.35f) + (asymmetryScore * 0.25f);

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