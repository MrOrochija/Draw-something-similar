using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SquareValidator", menuName = "Validators/Square")]
public class SquareValidator : ShapeValidator
{
    public override float CheckShape(List<Vector2> drawnPoints)
    {
        if (drawnPoints.Count < 10) return 0f;

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
        float maxDim = Mathf.Max(width, height);

        if (maxDim == 0) return 0f;

        float ratioScore = Mathf.Min(width, height) / maxDim;

        Vector2 topLeft = new Vector2(minX, maxY);
        Vector2 topRight = new Vector2(maxX, maxY);
        Vector2 bottomLeft = new Vector2(minX, minY);
        Vector2 bottomRight = new Vector2(maxX, minY);

        float minDistTL = float.MaxValue;
        float minDistTR = float.MaxValue;
        float minDistBL = float.MaxValue;
        float minDistBR = float.MaxValue;

        float totalEdgeError = 0f;

        foreach (Vector2 p in drawnPoints)
        {
            minDistTL = Mathf.Min(minDistTL, Vector2.Distance(p, topLeft));
            minDistTR = Mathf.Min(minDistTR, Vector2.Distance(p, topRight));
            minDistBL = Mathf.Min(minDistBL, Vector2.Distance(p, bottomLeft));
            minDistBR = Mathf.Min(minDistBR, Vector2.Distance(p, bottomRight));

            float distLeft = p.x - minX;
            float distRight = maxX - p.x;
            float distBottom = p.y - minY;
            float distTop = maxY - p.y;

            float closestEdgeDist = Mathf.Min(
                Mathf.Min(distLeft, distRight), 
                Mathf.Min(distBottom, distTop)
            );
            
            totalEdgeError += closestEdgeDist;
        }

        float avgCornerError = (minDistTL + minDistTR + minDistBL + minDistBR) / 4f;
        float normalizedCornerError = avgCornerError / maxDim;
        float cornerScore = Mathf.Clamp01(1f - (normalizedCornerError / 0.15f)); 

        float avgEdgeError = totalEdgeError / drawnPoints.Count;
        float normalizedEdgeError = avgEdgeError / maxDim;
        float edgeScore = Mathf.Clamp01(1f - (normalizedEdgeError / 0.1f));

        float finalScore = (ratioScore * 0.2f) + (cornerScore * 0.4f) + (edgeScore * 0.4f);

        return Mathf.Clamp(finalScore * 100f, 0f, 100f);
    }
}