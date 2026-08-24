using UnityEngine;
using System.Collections.Generic;

public abstract class ShapeValidator : ScriptableObject
{
    public abstract float CheckShape(List<Vector2> drawnPoints);
}