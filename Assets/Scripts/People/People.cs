using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class People : MonoBehaviour
{
    //---------------------------Keys----------------------------
    public List<GameObject> keys;
    public List<Vector3> points = new List<Vector3>();

    private void Start()
    {
        for (int i = 0; i < 15; i++)
        {
            points.Add(Vector3.zero);
        }
    }
    private void Update()
    {
        for (int i = 0; i < keys.Count; i++)
        {
            points[i] = keys[i].transform.position;
        }
    }
}
