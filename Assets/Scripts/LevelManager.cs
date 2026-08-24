using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [System.Serializable]
    public class Figure
    {
        public string figureName;
        public ShapeValidator validator;
        public bool active = true;
    }

    public DrawingCanvas canvas;
    public Figure[] allFigures; 

    private Figure currentTargetFigure; 
    private int currentTargetIndex = -1;

    public void PickRandomFigure()
    {
        if (allFigures.Length == 0) return;

        List<int> activeIndices = new List<int>();
        for (int i = 0; i < allFigures.Length; i++)
        {
            if (allFigures[i].active)
            {
                activeIndices.Add(i);
            }
        }

        if (activeIndices.Count == 0)
        {
            currentTargetFigure = null;
            currentTargetIndex = -1;
            return;
        }

        int randomActiveIndex = Random.Range(0, activeIndices.Count);
        
        currentTargetIndex = activeIndices[randomActiveIndex];
        currentTargetFigure = allFigures[currentTargetIndex];

        canvas.ClearCanvas(canvas.eraserColor);
        canvas.currentStroke.Clear();
    }

    public string GetCurrentFigureName()
    {
        if (currentTargetFigure != null) return currentTargetFigure.figureName;
        return "Unknown";
    }

    public float CheckDrawingPercent()
    {
        if (currentTargetFigure == null || currentTargetFigure.validator == null) return 0f;

        float matchPercentage = currentTargetFigure.validator.CheckShape(canvas.currentStroke);
        
        if (currentTargetIndex != -1)
        {
            allFigures[currentTargetIndex].active = false;
        }

        return matchPercentage;
    }
}