using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ParseData : MonoBehaviour
{
    //---------------------------Instance----------------------------
    public static ParseData _instance;
    public static ParseData Instance
    {
        get
        {
            if (null == _instance)
            {
                _instance = FindObjectOfType(typeof(ParseData)) as ParseData;
            }
            return _instance;
        }
    }
    //---------------------------Data From OpenPose & Keypoints From Data----------------------------
    string data = "";
    public List<Vector3> keypoints = new List<Vector3>();
    //---------------------------Magnitude of Enlargement----------------------------
    public float b = 20f;
    //---------------------------Test With File----------------------------
    public int num = 136;
    int i = 0;

    void FixedUpdate()
    {
        //---------------------------Just Return When not Prepare or Playing----------------------------
        if (!(GameManager.Instance.state == GameState.Prepare || GameManager.Instance.state == GameState.Playing))
            return;
        //---------------------------Refresh KeyPoints----------------------------
        keypoints = new List<Vector3>();
        data = "";
        //---------------------------Get From OpenPose----------------------------
        if (!GameManager.Instance.test)
        {
            data = PhoneCamera.Instance.result;
            if (data.Length > 10)
            {
                parse();
            }
        }
        else
        {
            //---------------------------Test With File----------------------------      
            string path = Application.dataPath + "/PoseData/DemoPose/MyTest" + i.ToString() + ".txt";
            readFile(path);
            parse();
            i++;
            if (i > num)
            {
                i = 0;
            }
        }             
    }

    public void parse()
    {
        //---------------------------Split Data----------------------------
        data = data.Replace("[", "").Replace("]", "").Replace(Environment.NewLine, "").Replace("\n", "").Replace("\r", "");
        string[] dataList = data.Split(' ');
        //---------------------------Set Keypoints----------------------------
        Vector3 pos;
        int j = 0;
        float x = 0, y = 0, z;              
        for (int i = 0; i < dataList.Length; i++)
        {
            if(dataList[i] != "")
            {
                if(j == 0)
                {
                    x = float.Parse(dataList[i]) * b * -1;
                    j++;
                }
                else if(j == 1)
                {
                    y = float.Parse(dataList[i]) * b * -1;
                    j++;
                }
                else
                {
                    z = float.Parse(dataList[i]);
                    pos = new Vector3(x, y, z);
                    keypoints.Add(pos);
                    j = 0;
                }          
            }
        }
    }

    public void readFile(string path)
    {
        //---------------------------Read File for Test----------------------------
        StreamReader fi;
        fi = new StreamReader(path);
        data = fi.ReadToEnd();
    }
}
