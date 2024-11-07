using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class RoomPlacement : MonoBehaviour
{
    [Serializable]
    public struct Rooms
    {
        public GameObject roomPrefab;
        public int roomCount;
    }

    [SerializeField]
    private Rooms[] Room;
    [SerializeField] private Delauney delauney;

    private List<GameObject> placedList;

    public int Seed;

    private bool m_Started;

    [SerializeField] private GameObject FirstRoom;

    // Start is called before the first frame update
    void Start()
    {
        m_Started = true;
        if (Seed != 0)
        {
            UnityEngine.Random.InitState(Seed);
        } else
        {
            UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);
        }

        placedList = new List<GameObject>();
        int nom = 0;
        foreach (Rooms x in Room)
        {
            for (int i = 0; i < x.roomCount; i++)
            {
                int angle;
                if (UnityEngine.Random.Range(0, 2) == 1){
                    angle = 90;
                }
                else
                {
                    angle = 0;
                }
                var temps = Instantiate(x.roomPrefab, new Vector3(UnityEngine.Random.Range(-5, 5), 0, UnityEngine.Random.Range(-5, 5)), Quaternion.Euler(0, angle, 0));
                //String abc = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                //temps.transform.name = "" + abc[nom];
                //nom++;
                placedList.Add(temps);
            }
        }
        Push(placedList);
    }

    void Push(List<GameObject> x)
    {
        Debug.Log("Ping");
        List<Vector3> vectorList = new List<Vector3>();
        foreach(GameObject y in x)
        {
            y.GetComponent<Collider>().enabled = false;
            Collider[] colliderlist = Physics.OverlapBox(y.transform.position, y.transform.localScale / 1.5f, Quaternion.identity);
            Vector3 Total = Vector3.zero;
            foreach (Collider c in colliderlist)
            {
                Total += y.transform.position + new Vector3(UnityEngine.Random.Range(0f, 1f), 0, UnityEngine.Random.Range(0f, 1f)) - c.transform.position;
            }
            vectorList.Add(Total.normalized);
            y.GetComponent<Collider>().enabled = true;
        }
        for (int i = 0; i < x.Count; i++)
        {
            x[i].transform.position += vectorList[i];
        }
        Check(x);
    }

    void Check(List<GameObject> x)
    {
        Debug.Log("Pong");
        List<GameObject> NewList = new List<GameObject>();
        List<GameObject> FakeNewList = new List<GameObject>();
        foreach (GameObject y in x)
        {
            y.GetComponent<Collider>().enabled = false;
            FakeNewList.Add(y);
            if (Physics.OverlapBox(y.transform.position, y.transform.localScale / 1.5f, Quaternion.identity).Length > 0)
            {
                NewList.Add(y);
            }
            y.GetComponent<Collider>().enabled = true;
        }
        if (NewList.Count != 0)
        {
            StartCoroutine(WaitCoroutine(FakeNewList));
        }
        else
        {
            Debug.Log("SA MERE, ça marche !");
            placedList.Add(FirstRoom);
            delauney.StartDelauney(placedList);
            return;
        }
    }
    IEnumerator WaitCoroutine(List<GameObject> x)
    {
        Debug.Log("Donald Trump");
        yield return new WaitForSeconds(0.001f);
        Push(x);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
        if (m_Started)
            //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
            Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
