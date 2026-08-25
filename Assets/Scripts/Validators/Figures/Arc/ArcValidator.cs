using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ArcValidator", menuName = "Validators/Arc")]
public class ArcValidator : ShapeValidator
{
    public override float CheckShape(List<Vector2> drawnPoints)
    {
        if (drawnPoints == null || drawnPoints.Count < 6) return 0f;

        Vector2 start = drawnPoints[0];
        Vector2 end = drawnPoints[drawnPoints.Count - 1];
        float chordLength = Vector2.Distance(start, end);

        if (chordLength < 0.05f) return 0f;

        Vector2 chordVector = end - start;
        int positiveCount = 0;
        int negativeCount = 0;
        float maxDeviation = 0f;

        for (int i = 1; i < drawnPoints.Count - 1; i++)
        {
            Vector2 p = drawnPoints[i];
            float signedDist = (chordVector.x * (p.y - start.y) - chordVector.y * (p.x - start.x)) / chordLength;

            if (signedDist > 0f) positiveCount++;
            else negativeCount++;

            if (Mathf.Abs(signedDist) > maxDeviation)
                maxDeviation = Mathf.Abs(signedDist);
        }

        int totalInnerPoints = drawnPoints.Count - 2;
        int dominantSideCount = Mathf.Max(positiveCount, negativeCount);
        float oneSideScore = (float)dominantSideCount / totalInnerPoints;

        if (oneSideScore < 0.8f) return 0f;

        float curvatureRatio = maxDeviation / chordLength;
        float curveScore = Mathf.Clamp01(1f - (Mathf.Abs(curvatureRatio - 0.25f) / 0.2f));

        float totalPath = 0f;
        for (int i = 1; i < drawnPoints.Count; i++)
            totalPath += Vector2.Distance(drawnPoints[i - 1], drawnPoints[i]);

        float pathRatio = totalPath / chordLength;
        float pathScore = Mathf.Clamp01(1f - (Mathf.Abs(pathRatio - 1.25f) / 0.4f));

        float finalScore = (oneSideScore * 0.4f) + (curveScore * 0.4f) + (pathScore * 0.2f);

        return Mathf.Clamp(finalScore * 100f, 0f, 100f);
    }
}