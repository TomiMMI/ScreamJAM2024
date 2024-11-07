using UnityEngine;

[System.Serializable]
public class LinearEquation
{
    public float _A;
    public float _B;
    public float _C;

    public LinearEquation() { }

    //Ax+By=C
    public LinearEquation(Vector2 pointA, Vector2 pointB)
    {
        float deltaX = pointB.x - pointA.x;
        float deltaY = pointB.y - pointA.y;
        _A = deltaY; //y2-y1
        _B = -deltaX; //x1-x2
        _C = _A * pointA.x + _B * pointA.y;
    }

    public LinearEquation PerpendicularLineAt(Vector3 point)
    {
        LinearEquation newLine = new LinearEquation();

        newLine._A = -_B;
        newLine._B = _A;
        newLine._C = newLine._A * point.x + newLine._B * point.y;

        return newLine;
    }
}