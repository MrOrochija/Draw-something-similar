using UnityEngine;

[CreateAssetMenu(fileName = "NewFigure", menuName = "Figure/Figure Data")]
public class Figure : ScriptableObject
{
    public string figureName;
    public ShapeValidator validator;
}