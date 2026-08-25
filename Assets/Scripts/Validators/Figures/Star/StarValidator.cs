using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StarValidator", menuName = "Validators/Star")]
public class StarValidator : ShapeValidator
{
    public override float CheckShape(List<Vector2> drawnPoints)
    {
        if (drawnPoints == null || drawnPoints.Count < 20) return 0f;

        Vector2 centroid = Vector2.zero;
        foreach (Vector2 p in drawnPoints) centroid += p;
        centroid /= drawnPoints.Count;

        float maxDist = 0f;
        foreach (Vector2 p in drawnPoints) maxDist = Mathf.Max(maxDist, Vector2.Distance(centroid, p));

        if (maxDist < 0.1f) return 0f;

        float startEndDist = Vector2.Distance(drawnPoints[0], drawnPoints[drawnPoints.Count - 1]);
        float closureScore = Mathf.Clamp01(1f - (startEndDist / (maxDist * 1.0f)));

        List<int> peakIndices = new List<int>();
        float lastDist = Vector2.Distance(centroid, drawnPoints[0]);
        bool rising = false;

        for (int i = 1; i < drawnPoints.Count; i++)
        {
            float dist = Vector2.Distance(centroid, drawnPoints[i]);
            if (rising && dist < lastDist)
            {
                peakIndices.Add(i - 1);
                rising = false;
            }
            else if (!rising && dist > lastDist)
            {
                rising = true;
            }
            lastDist = dist;
        }

        if (rising) peakIndices.Add(drawnPoints.Count - 1);

        int numPeaks = peakIndices.Count;
        float peakCountScore = Mathf.Clamp01(1f - Mathf.Abs(numPeaks - 5) / 3f);

        float shapeScore = peakCountScore;

        if (numPeaks >= 4 && numPeaks <= 6)
        {
            float angleVarTotal = 0f;
            float targetAngle = 72f;
            for (int i = 0; i < numPeaks; i++)
            {
                Vector2 dir1 = drawnPoints[peakIndices[i]] - centroid;
                Vector2 dir2 = drawnPoints[peakIndices[(i + 1) % numPeaks]] - centroid;
                float angle = Vector2.Angle(dir1, dir2);
                angleVarTotal += Mathf.Abs(angle - targetAngle);
            }
            float avgAngleVar = angleVarTotal / numPeaks;
            float angleScore = Mathf.Clamp01(1f - (avgAngleVar / 25f));
            shapeScore = (peakCountScore * 0.6f) + (angleScore * 0.4f);
        }

        float finalScore = (shapeScore * 0.7f) + (closureScore * 0.3f);
        return Mathf.Clamp(finalScore * 100f, 0f, 100f);
    }
}