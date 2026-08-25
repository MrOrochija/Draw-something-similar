using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CrescentValidator", menuName = "Validators/Crescent")]
public class CrescentValidator : ShapeValidator
{
    public override float CheckShape(List<Vector2> drawnPoints)
    {
        if (drawnPoints == null || drawnPoints.Count < 12) return 0f;

        Vector2 tipA = drawnPoints[0];
        Vector2 tipB = drawnPoints[0];
        float maxTipDistSqr = -1f;

        for (int i = 0; i < drawnPoints.Count; i++)
        {
            for (int j = i + 1; j < drawnPoints.Count; j++)
            {
                float sqrDist = Vector2.SqrMagnitude(drawnPoints[i] - drawnPoints[j]);
                if (sqrDist > maxTipDistSqr)
                {
                    maxTipDistSqr = sqrDist;
                    tipA = drawnPoints[i];
                    tipB = drawnPoints[j];
                }
            }
        }

        float tipDistance = Mathf.Sqrt(maxTipDistSqr);
        if (tipDistance <= 0.01f) return 0f;

        Vector2 ab = tipB - tipA;
        int positiveCount = 0;
        int negativeCount = 0;
        float maxPositiveDist = 0f;
        float maxNegativeDist = 0f;

        foreach (Vector2 p in drawnPoints)
        {
            float signedDist = (ab.x * (p.y - tipA.y) - ab.y * (p.x - tipA.x)) / tipDistance;

            if (signedDist > 0f)
            {
                positiveCount++;
                if (signedDist > maxPositiveDist) maxPositiveDist = signedDist;
            }
            else
            {
                negativeCount++;
                if (-signedDist > maxNegativeDist) maxNegativeDist = -signedDist;
            }
        }

        int dominantCount = Mathf.Max(positiveCount, negativeCount);
        float sideRatio = (float)dominantCount / drawnPoints.Count;
        float oneSideScore = Mathf.Clamp01((sideRatio - 0.7f) / 0.25f);

        float maxArcHeight = Mathf.Max(maxPositiveDist, maxNegativeDist);
        float arcRatio = maxArcHeight / tipDistance;
        float arcScore = Mathf.Clamp01(1f - (Mathf.Abs(arcRatio - 0.4f) / 0.3f));

        float minArcHeight = Mathf.Min(maxPositiveDist, maxNegativeDist);
        float innerDistRatio = minArcHeight / tipDistance;
        float hollowScore = Mathf.Clamp01(1f - (innerDistRatio / 0.15f));

        float endDistance = Vector2.Distance(drawnPoints[0], drawnPoints[drawnPoints.Count - 1]);
        float closureScore = Mathf.Clamp01(1f - (endDistance / (tipDistance * 0.4f)));

        float shapeScore = (oneSideScore * 0.45f) + (arcScore * 0.3f) + (hollowScore * 0.25f);
        float finalScore = (shapeScore * 0.8f) + (closureScore * 0.2f);

        return Mathf.Clamp(finalScore * 100f, 0f, 100f);
    }
}