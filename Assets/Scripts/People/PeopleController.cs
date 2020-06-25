using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleController : MonoBehaviour
{
    //---------------------------Instance----------------------------
    public static PeopleController _instance;
    public static PeopleController Instance
    {
        get
        {
            if (null == _instance)
            {
                _instance = FindObjectOfType(typeof(PeopleController)) as PeopleController;
            }
            return _instance;
        }
    }
    //---------------------------People1 & People2----------------------------
    public GameObject people1, people2;

    void Update()
    {
        //---------------------------Set People1 & People2 Active----------------------------
        if (ParseJoints.Instance.isValid)
        {
            people1.SetActive(true);
            people2.SetActive(ParseJoints.Instance.isTwo & GameManager.Instance.type == GameType.Double);
        }
        else
        {
            people1.SetActive(false);
            people2.SetActive(false);
        }
    }
}
