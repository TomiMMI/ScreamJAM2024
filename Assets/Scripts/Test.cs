using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private Vector3 start1;
    [SerializeField] private Vector3 end1;
    [SerializeField] private Vector3 start2;
    [SerializeField] private Vector3 end2;
    public Vector3 IntersectionPointTwoLines(Vector3 line1Start, Vector3 line1End, Vector3 line2Start, Vector3 line2End)
    {
        (float x1, float y1) = GetXYPosition(line1Start);
        (float x2, float y2) = GetXYPosition(line1End);
        (float x3, float y3) = GetXYPosition(line2Start);
        (float x4, float y4) = GetXYPosition(line2End);

        float topX = (x1 * y2 - x2 * y1) * (x3 - x4) - (x3 * y4 - x4 * y3) * (x1 - x2);
        float topY = (x1 * y2 - x2 * y1) * (y3 - y4) - (x3 * y4 - x4 * y3) * (y1 - y2);
        float bottom = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
        float pX = topX / bottom;
        float pY = topY / bottom;

        Vector3 pVector = new Vector3(pX, 0f, pY);
        bool isInBoundsLine1 = IsIntersectionInBounds(line1Start, line1End, pVector);
        bool isInBoundsLine2 = IsIntersectionInBounds(line2Start, line2End, pVector);

        if (!isInBoundsLine1 || !isInBoundsLine2)
        {
            return default;
        }

        return pVector;
    }
    public (float, float) GetXYPosition(Vector3 vector)
    {
        return (vector.x, vector.z);
    }
    public bool IsIntersectionInBounds(Vector3 lineStart, Vector3 lineEnd, Vector3 intersection)
    {
        float distAC = Vector3.Distance(lineStart, intersection);
        float distBC = Vector3.Distance(lineEnd, intersection);
        float distAB = Vector3.Distance(lineStart, lineEnd);
        if (Mathf.Abs(distAC + distBC - distAB) > 0.001f)
        {
            return false;
        }

        return true;
    }
    private void Start()
    {
        Debug.Log(IntersectionPointTwoLines(start1, end1, start2, end2));
    }
}
