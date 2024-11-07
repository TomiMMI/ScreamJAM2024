using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;
using static UnityEngine.Rendering.DebugUI;

public class Delauney : MonoBehaviour
{



    private List<Vector3[]> toTest;

    [SerializeField]private GameObject DebugBalls;
    [SerializeField]private GameObject Sphere;
    [SerializeField]private LayerMask sphereLayer;

    
    private Dictionary<string, Vector3[]> TriangleList;

    List<GameObject> pointList;

    [SerializeField] private Material Red;


    private float DistanceCircum = 0f;

    Vector3[] pointListGizmo;

    public bool Done = false;

    private float Dist;
    private float longest;
    // Start is called before the first frame update
    void Start()
    {
        toTest = new List<Vector3[]>();
        TriangleList = new Dictionary<string, Vector3[]>();
        pointList = new List<GameObject>();
    }
    public void StartDelauney(List<GameObject> roomList)
    {
        foreach(GameObject room in roomList)
        {
            GameObject Sfire = Instantiate(Sphere, room.transform.position, Quaternion.identity);
            pointList.Add(Sfire);
        }

        GameObject furthestpoint = null;
        longest = 0f;
        foreach (GameObject p in pointList)
        {
            if (Vector3.Distance(new Vector3(20f, 0, 20f), p.transform.position) > longest)
            {
                longest = Vector3.Distance(new Vector3(20f, 0, 20f), p.transform.position);
                //Debug.Log(longest);
                furthestpoint = p;
            }
        }
        furthestpoint.GetComponent<Renderer>().material = Red;

        Dist = 2 * (Vector3.Distance(new Vector3(20f, 0f, 20f), furthestpoint.transform.position) + 2f);

        TriangleList.Add("super", new Vector3[] {
            new Vector3(20 + Dist * Mathf.Sin(0 * Mathf.Deg2Rad), 0, 20 + Dist * Mathf.Cos(0 * Mathf.Deg2Rad)),
            new Vector3(20 + Dist * Mathf.Sin(120 * Mathf.Deg2Rad), 0, 20 + Dist * Mathf.Cos(120 * Mathf.Deg2Rad)),
            new Vector3(20 + Dist * Mathf.Sin(240 * Mathf.Deg2Rad), 0, 20 + Dist * Mathf.Cos(240 * Mathf.Deg2Rad))
        });

        var suce = 0;
        foreach (var tri in TriangleList["super"])
        {
            var temp = GameObject.Instantiate(DebugBalls, tri, Quaternion.identity);
            temp.transform.name = "" + suce;
            suce++;
        }

        List<Vector2> toCircum = new List<Vector2>();
        foreach (var tri in TriangleList["super"])
        {
            toCircum.Add(new Vector2(tri.x, tri.z));
        }


        Dictionary<string, Vector3[]> cuttedTri = new Dictionary<string, Vector3[]>();
        cuttedTri = CutTri(new KeyValuePair<string, Vector3[]>("super", TriangleList["super"]), furthestpoint);
        foreach (KeyValuePair<string, Vector3[]> Tri in TriangleList)
        {
            if (Tri.Key != "super")
            {
                TriangleList.Remove(Tri.Key);
            }
        }
        foreach (KeyValuePair<string, Vector3[]> inOutTri in cuttedTri)
        {
            TriangleList.Add(inOutTri.Key, inOutTri.Value);
        }

        pointList.Remove(furthestpoint);

        foreach (GameObject p in pointList)
        {
            //Debug.Log(p.transform.name + " / " + p.transform.position);
            List<KeyValuePair<string, Vector3[]>> badTri = new List<KeyValuePair<string, Vector3[]>>();
            //Debug.Log(TriangleList.Count);
            foreach (KeyValuePair<string, Vector3[]> Tri in TriangleList)
            {
                //Debug.Log(TriangleList.Count);
                if (Tri.Key == "super") continue;
                //Debug.Log(Tri.Key);
                Vector2 Circum = GetCircumcenter(ToVect2(Tri.Value[0]), ToVect2(Tri.Value[1]), ToVect2(Tri.Value[2]));
                //Debug.Log(Tri.Key + " " + Tri.Value[0] + " " + Tri.Value[1] + " " + Tri.Value[2] + " " + p.transform.position);

                //Debug.Log(p.transform.name + Circum + DistanceCircum);
                if (IsInTriCircum(ToVect3(Circum), p))
                {
                    if (p.transform.name == "D")
                    {
                        //Debug.Log(ShowTriangles(TriangleList));
                        //Debug.Log(p.transform.position);
                        //Debug.Log("Circumcircle of point " + Tri.Key + " : " + Circum + " . " + DistanceCircum);
                        //Debug.Log(ShowTriangle(Tri.Value));
                    }

                    badTri.Add(Tri);
                }
            }
            //Debug.Log("Badtri : " + p.gameObject.transform.position + " = " + badTri.Count + ShowTriangles(badTri));
            if (badTri.Count == 0)
            {
                //Debug.Log("-----");
            }
            //if (badTri.Count == 1)
            //{
            //    //Debug.Log("In of CutTri : " + ShowTriangle(badTri[0]) + " /" + p.transform.position);
            //    Dictionary<string, Vector3[]> outTri = CutTri(badTri[0], p);
            //    List<string> keyList = new List<string>();
            //    foreach (KeyValuePair<string, Vector3[]> element in TriangleList)
            //    {
            //        if (element.Value == badTri[0].Value)
            //        {
            //            keyList.Add(element.Key);
            //        }
            //    }
            //    foreach(string key in keyList)
            //    {
            //        TriangleList.Remove(key);
            //    }
            //    foreach (KeyValuePair<string, Vector3[]> inOutTri in outTri)
            //    {
            //        TriangleList.Add(inOutTri.Key, inOutTri.Value);
            //    }
            //}
            else if (badTri.Count >= 1)
            {
                //Debug.Log("coucou");
                List<Vector3[]> BadEdges = FindBadEdges(badTri);
                List<KeyValuePair<string, Vector3[]>> keyList = new List<KeyValuePair<string, Vector3[]>>();
                if (p.transform.name == "E")
                {
                    //Debug.Log(ShowTriangles(BadEdges));
                }

                foreach (KeyValuePair<string, Vector3[]> triangle in badTri)
                {
                    if (CountBadEdges(triangle, BadEdges) == 3)
                    {
                        keyList.Add(triangle);
                    }
                }
                foreach (var key in keyList)
                {
                    badTri.Remove(key);
                }
                foreach (Vector3[] gefveufeuf in BadEdges)
                {
                    //Debug.Log(ShowTriangle(gefveufeuf));
                }
                foreach (KeyValuePair<string, Vector3[]> triangle in badTri)
                {
                    //Debug.Log(ShowTriangle(triangle.Value));

                    Dictionary<string, Vector3[]> outTri = CutTri(triangle, p);
                    List<KeyValuePair<string, Vector3[]>> trianglesARetirer = new List<KeyValuePair<string, Vector3[]>> { triangle };
                    foreach (KeyValuePair<string, Vector3[]> nouveauTriangle in outTri)
                    {
                        if (CountBadEdges(nouveauTriangle, BadEdges) >= 1)
                        {
                            //Debug.Log("Coucou toi ! Comment il va bien mon reuf, Est-ce que c'est bon pour vous ?");
                            trianglesARetirer.Add(nouveauTriangle);
                        }
                    }
                    foreach (KeyValuePair<string, Vector3[]> nouveau in trianglesARetirer)
                    {
                        outTri.Remove(nouveau.Key);
                    }

                    foreach (KeyValuePair<string, Vector3[]> triRajoute in outTri)
                    {
                        if (p.transform.name == "H")
                        {
                            toTest.Add(triRajoute.Value);
                        }
                        TriangleList.Add(triRajoute.Key, triRajoute.Value);
                    }
                    TriangleList.Remove(triangle.Key);
                }
                foreach (KeyValuePair<string, Vector3[]> l in TriangleList)
                {


                }

                /*
                foreach (KeyValuePair<string, Vector3[]> Tri in TriangleList)
                {
                    //Debug.Log(Tri.Value.Length + " " + BadEdges.Count);
                    if (Tri.Value.Contains(BadEdges[0]) && Tri.Value.Contains(BadEdges[1]) && Tri.Key != "super")
                    {
                        keyList.Add(Tri.Key);
                    }
                }
                foreach(string key in keyList)
                {
                    Vector3 PntManquant = new Vector3(100,100,100);
                    foreach(Vector3 point in TriangleList[key])
                    {
                        if(point != BadEdges[0] && point != BadEdges[1])
                        {
                            PntManquant = point;
                        }
                    }
                    TriangleList.Add("AC-" + key, new Vector3[]{
                        BadEdges[0],
                        PntManquant,
                        p.transform.position
                    });
                    TriangleList.Add("BC-" + key, new Vector3[]{
                        BadEdges[1],
                        PntManquant,
                        p.transform.position
                    });
                    TriangleList.Remove(key);
                }
                */
            }
        }
        RemoveSuper(TriangleList);
    }

    private Vector2 ToVect2(Vector3 D3)
    {
        return new Vector2(D3.x, D3.z);
    }

    private Vector3 ToVect3(Vector2 D2)
    {
        return new Vector3(D2.x, 0, D2.y);
    }

    private List<Vector3[]> FindBadEdges(List<KeyValuePair<string, Vector3[]>> badTriangles)
    {
        foreach(KeyValuePair<string, Vector3[]> entree in badTriangles){
            //Debug.Log("badTriangles d'entrée = " + ShowTriangle(entree.Value));
        }
        List<Vector3[]> edgePoints = new List<Vector3[]>();
        for (int i = 0; i <= badTriangles.Count - 2; i++)
        {
            for (int j = i + 1; j <= badTriangles.Count - 1 ; j++)
            {
                Vector3[] line = {};
                foreach (Vector3 point in badTriangles[i].Value)
                {
                    //Debug.Log("Here =" + badTriangles[j].Value.Contains(point));
                    if (badTriangles[j].Value.Contains(point))
                    {
                        line = line.Concat(new Vector3[] { point }).ToArray();
                            //Debug.Log("Bad edge = " + p);
                    }
                }
                if(line.Length == 2)
                {
                    edgePoints.Add(line);
                }
            }
        }
        foreach (Vector3[] gefveufeuf in edgePoints)
        {
            Debug.Log("Edgepoint :" + ShowTriangle(gefveufeuf));
        }
        return edgePoints;
    }

    private void RemoveSuper(Dictionary<string, Vector3[]> TriangleList)
    {
        Vector3[] Super = TriangleList["super"];
        TriangleList.Remove("super");
        List<string> SuperList = new List<string>();
        foreach (KeyValuePair<string,Vector3[]> triangles in TriangleList)
        {
            if (triangles.Value.Contains(Super[0]) || triangles.Value.Contains(Super[1]) || triangles.Value.Contains(Super[2])){
                SuperList.Add(triangles.Key);   
            }
        }
        foreach(string key in SuperList)
        {
            TriangleList.Remove(key);
        }
    }

    private int CountBadEdges(KeyValuePair<string, Vector3[]> triangle, List<Vector3[]> badEdges)
    {
        int numberOfBadEdges = 0;
        Debug.Log("COUNT BAD EDGES : Triangle - " + ShowTriangle(triangle.Value));
        foreach(var truc in badEdges)
        {
            Debug.Log("Bad Edge : " + ShowTriangle(truc));
        }

        List<List<Vector3>> checklist = new List<List<Vector3>>() { new List<Vector3> { triangle.Value[0], triangle.Value[1] }, new List<Vector3> { triangle.Value[1], triangle.Value[2] }, new List<Vector3> { triangle.Value[2], triangle.Value[0] } };
        foreach(List<Vector3> element in checklist)
        {
            foreach (var badEdge in badEdges)
            {
                if (element.Contains(badEdge[0]) && element.Contains(badEdge[1]))
                {
                    numberOfBadEdges++;
                }
            }
        }
        Debug.Log("Found Bad EDGES number =" + numberOfBadEdges);
        return numberOfBadEdges;
    }

    private Dictionary<string, Vector3[]> CutTri(KeyValuePair<string, Vector3[]> Tri, GameObject point)
    {
        Dictionary<string, Vector3[]> OutTri = new Dictionary<string, Vector3[]>();
        OutTri.Add(Tri.Key + "01" + point.transform.name, new Vector3[]{
            Tri.Value[0],
            Tri.Value[1],
            point.transform.position
        });
        OutTri.Add(Tri.Key + "02" + point.transform.name, new Vector3[]{
            Tri.Value[0],
            Tri.Value[2],
            point.transform.position
        });
        OutTri.Add(Tri.Key + "12" + point.transform.name, new Vector3[]{
            Tri.Value[1],
            Tri.Value[2],
            point.transform.position
        });

        //Debug.Log("OutTri = " + OutTri.Count + " / " + OutTri);
        return OutTri;
    }

        //ZeroKelvinTutorial function to find Circumcenter : https://www.reddit.com/r/Unity3D/comments/wppjjd/how_to_calculate_the_circumcenter_of_a_triangle/s
        public Vector2 GetCircumcenter(Vector2 pointA, Vector2 pointB, Vector2 pointC)
    {
        LinearEquation lineAB = new LinearEquation(pointA, pointB);
        LinearEquation lineBC = new LinearEquation(pointB, pointC);

        Vector2 midPointAB = Vector2.Lerp(pointA, pointB, .5f);
        Vector2 midPointBC = Vector2.Lerp(pointB, pointC, .5f);

        LinearEquation perpendicularAB = lineAB.PerpendicularLineAt(midPointAB);
        LinearEquation perpendicularBC = lineBC.PerpendicularLineAt(midPointBC);

        Vector2 circumcircle = GetCrossingPoint(perpendicularAB, perpendicularBC);

        DistanceCircum = Vector2.Distance(circumcircle, pointA);

        //Debug.Log("CIRCLE VALUES " + circumcircle + " " + Delauney.DistanceCircum);;

        return circumcircle;
    }

    static Vector2 GetCrossingPoint(LinearEquation line1, LinearEquation line2)
    {
        float A1 = line1._A;
        float A2 = line2._A;
        float B1 = line1._B;
        float B2 = line2._B;
        float C1 = line1._C;
        float C2 = line2._C;

        //Cramer's rule
        float Determinant = A1 * B2 - A2 * B1;
        float DeterminantX = C1 * B2 - C2 * B1;
        float DeterminantY = A1 * C2 - A2 * C1;

        float x = DeterminantX / Determinant;
        float y = DeterminantY / Determinant;

        return new Vector2(x, y);
    }

    private bool IsInTriCircum(Vector3 Center, GameObject Current)
    {
        Debug.Log("Done");
        Collider[] touched = Physics.OverlapSphere(Center, DistanceCircum, sphereLayer); 
        if (touched.Length == 0)
        {
            Debug.Log("Rien Chef");
            return false;
        }
        foreach (Collider collider in touched) {
            if (collider.gameObject == Current)
            {
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {

        if (Done == true)
        {
            Gizmos.color = UnityEngine.Color.yellow;
            Gizmos.DrawWireSphere(new Vector3(20, 0, 20), Dist/2);
            //Gizmos.color = UnityEngine.Color.magenta;

            Gizmos.color = UnityEngine.Color.cyan;

            foreach (KeyValuePair<string, Vector3[]> Tri in TriangleList)
            {
                Gizmos.DrawLine(Tri.Value[0], Tri.Value[1]);
                Gizmos.DrawLine(Tri.Value[1], Tri.Value[2]);
                Gizmos.DrawLine(Tri.Value[2], Tri.Value[0]);
            }
            Gizmos.color = UnityEngine.Color.green;
           if(toTest.Count != 0)
            {
                foreach (Vector3[] Tri in toTest)
                {
                    Gizmos.DrawLine(Tri[0], Tri[1]);
                    Gizmos.DrawLine(Tri[1], Tri[2]);
                    Gizmos.DrawLine(Tri[2], Tri[0]);
                }
            }
        }
    }
    private string ShowTriangles(List<Vector3[]> triangles)
    {
        string message = "";
        
        foreach(Vector3[] values in triangles)
        {
            message += "<<";
            foreach(Vector3 value in values)
            {
                message += value + ";";
            }
            message += ">>";
        }
        return message;
    }
    private string ShowTriangle(Vector3[] triangles)
    {
        string message = "";

        foreach (Vector3 values in triangles)
        {
            message += values + ";";
        }
        return message;
    }
    private string ShowTriangles(List<KeyValuePair<string, Vector3[]>> triangles)
    {
        string message = "";

        foreach (KeyValuePair<string, Vector3[]> values in triangles)
        {
            message += "<<";
            foreach (Vector3 value in values.Value)
            {
                message += value + ";";
            }
            message += ">>";
        }
        return message;
    }
    private string ShowTriangles(Dictionary<string, Vector3[]> triangles)
    {
        string message = "";

        foreach (KeyValuePair<string, Vector3[]> values in triangles)
        {

            message += "<<" + values.Key + " = ";
            foreach (Vector3 value in values.Value)
            {
                message += value + ";";
            }
            message += ">>";
        }
        return message;
    }
}
