using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParseJoints : MonoBehaviour
{
    //---------------------------Instance----------------------------
    public static ParseJoints _instance;
    public static ParseJoints Instance
    {
        get
        {
            if (null == _instance)
            {
                _instance = FindObjectOfType(typeof(ParseJoints)) as ParseJoints;
            }
            return _instance;
        }
    }
    //---------------------------Data Has been Parsed by ParseData----------------------------
    public List<Vector3> parsedData = new List<Vector3>();
    //---------------------------Set Keypoints & Joints of people1 & people2----------------------------
    public List<Vector3> p1 = new List<Vector3>();
    public List<Vector3> p2 = new List<Vector3>();
    public List<List<Vector3>> joints1 = new List<List<Vector3>>();
    public List<List<Vector3>> joints2 = new List<List<Vector3>>();
    //---------------------------Index of Each Joints----------------------------
    int[,] keys = new int[,] { { 0, 1 }, { 1, 2 }, { 2, 3 }, { 3, 4 }, { 1, 5 }, { 5, 6 },
                    { 6, 7 }, { 1, 8 }, { 8, 9 }, { 9, 10 }, { 10, 11 }, { 8, 12 },
                    { 12, 13 }, { 13, 14 }, { 11, 22 }, { 14, 19 } };
    //---------------------------Whether There is Two People & OpenPose is Working----------------------------
    public bool isTwo = false;
    public bool isValid = true;
    //---------------------------Center In Fixed Position----------------------------
    public Vector3 one = new Vector3(0, 4.0f, 0);
    public Vector3 two = new Vector3(-5, 4.0f, 0);
    Vector3 center;
    int index = 1;
    public bool setCenter = false;
    float shiftX = 3.0f;

    void Start()
    {
        //---------------------------Initiate Keypoints & Joints of people1 & people2 to Zero----------------------------
        List<Vector3> zero = new List<Vector3>();
        zero.Add(Vector3.zero);
        zero.Add(Vector3.zero);
        for(int i = 0; i < 16; i++)
        {
            joints1.Add(zero);
            joints2.Add(zero);
        }
        for (int i = 0; i < 25; i++)
        {
            p1.Add(Vector3.zero);
            p2.Add(Vector3.zero);
        }
    }

    void Update()
    {
        //---------------------------Get Data Has been Parsed by ParseData----------------------------
        parsedData = ParseData.Instance.keypoints;
        //---------------------------Set Whether There is Two People & OpenPose is Working----------------------------
        setTwo();
        //---------------------------Set Keypoints & Joints of people1 & people2----------------------------
        parseJoints();
    }

    void setTwo()
    {
        //---------------------------Keys Less Than 25 Means OpenPose can't See People----------------------------
        if (parsedData.Count < 25)
        {
            isTwo = false;
            isValid = false;
            return;
        }
        //---------------------------Keys == 25 Means OpenPose See One People----------------------------
        else if (parsedData.Count == 25)
        {
            isTwo = false;
        }
        //---------------------------Keys == 50 Means OpenPose See Two People----------------------------
        else
        {
            isTwo = true;
        }       
        isValid = true;
    }

    void chooseCenterKey(int i0, int i1)
    {
        //---------------------------Choose Center Keys----------------------------
        if (parsedData[1].z != 0)
        {
            index = i0 + 1;
        }
        else if (parsedData[8].z != 0)
        {
            index = i0 + 8;
        }
        else if (parsedData[0].z != 0)
        {
            index = i0;
        }
        else
        {
            for (int i = i0; i < i1; i++)
            {
                if (parsedData[i].z != 0)
                {
                    index = i;
                }
            }
        }
    }

    void parseJoints()
    {
        //---------------------------Just Return When It is Invalid----------------------------
        if (!isValid)
        {
            return;
        }
        //---------------------------Choose the Center & Set Shifting----------------------------
        if(GameManager.Instance.type == GameType.Single)
        {
            center = one;
        }
        else
        {
            center = two;
        }
        chooseCenterKey(0, 25);
        float shiftingX, shiftingY;
        shiftingY = center.y - parsedData[index].y;
        if (setCenter)
        {
            shiftingX = center.x - parsedData[index].x;
        }
        else
        {
            shiftingX = shiftX;
        }
        //---------------------------Set Keypoints & Joints of people1 & people2----------------------------
        List<Vector3> pos;
        Vector3 k1, k2, impossibleK = new Vector3(30, 30, 0), shifting = new Vector3(shiftingX, shiftingY, 0);
        if (!isTwo)
        {
            //---------------------------Keypoints----------------------------
            for (int i = 0; i < 25; i++)
            {
                //---------------------------(0, 0, 0) Means OpenPose can't See this Keypoint, Just Set it to an Impossible Point----------------------------
                if (parsedData[i] == Vector3.zero)
                {
                    p1[i] = impossibleK;
                }
                else
                {
                    p1[i] = parsedData[i] + shifting;
                }
            }
            //---------------------------Joints----------------------------
            for (int i = 0; i < keys.Length / 2; i++)
            {
                pos = new List<Vector3>();
                k1 = p1[keys[i, 0]];
                pos.Add(k1);
                k2 = p1[keys[i, 1]];
                pos.Add(k2);              
                joints1[i] = pos;
            }
        }
        else if(isTwo & GameManager.Instance.type == GameType.Double)
        {
            //---------------------------Keypoints: People1----------------------------
            for (int i = 0; i < 25; i++)
            {
                //---------------------------(0, 0, 0) Means OpenPose can't See this Keypoint, Just Set it to an Impossible Point----------------------------
                if (parsedData[i] == Vector3.zero)
                {
                    p1[i] = impossibleK;
                }
                else
                {
                    p1[i] = parsedData[i] + shifting;
                }
            }
            //---------------------------Keypoints: People2----------------------------
            chooseCenterKey(25, 50);
            //---------------------------Whether Set Center----------------------------
            shiftingY = center.y - parsedData[index].y;
            if (setCenter)
            {
                shiftingX = -center.x - parsedData[index].x;
            }
            else
            {
                shiftingX = shiftX;
            }           
            shifting = new Vector3(shiftingX, shiftingY, 0);
            //---------------------------Whether Set Center----------------------------
            for (int i = 25; i < 50; i++)
            {
                //---------------------------(0, 0, 0) Means OpenPose can't See this Keypoint, Just Set it to an Impossible Point----------------------------
                if (parsedData[i] == Vector3.zero)
                {
                    p2[i - 25] = impossibleK;
                }
                else
                {
                    p2[i - 25] = parsedData[i] + shifting;
                }
            }
            //---------------------------Joints: People1----------------------------
            for (int i = 0; i < keys.Length / 2; i++)
            {
                pos = new List<Vector3>();
                k1 = p1[keys[i, 0]];
                pos.Add(k1);
                k2 = p1[keys[i, 1]];
                pos.Add(k2);
                joints1[i] = pos;
            }
            //---------------------------Joints: People2----------------------------
            for (int i = 0; i < keys.Length / 2; i++)
            {
                pos = new List<Vector3>();
                k1 = p2[keys[i, 0]];
                pos.Add(k1);
                k2 = p2[keys[i, 1]];
                pos.Add(k2);
                joints2[i] = pos;
            }
        }   
    }
}

//---------------------------Just For Inference: KeyPoints----------------------------
enum HumanKeyPoints
{
    Nose,
    Neck,
    RShoulder,
    RElbow,
    RWrist,
    LShoulder,
    LElbow,
    LWrist,
    MidHip,
    RHip,
    RKnee,
    RAnkle,
    LHip,
    LKnee,
    LAnkle,
    REye,
    LEye,
    REar,
    LEar,
    LBigToe,
    LSmallToe,
    LHeel,
    RBigToe,
    RSmallToe,
    RHeel
};

//---------------------------Just For Inference: Joints----------------------------
enum HumanJoints
{
    NoseNeck01,
    NeckRShoulder12,
    RShoulderRElbow23,
    RElbowRWrist34,
    NeckLShoulder15,
    LShoulderLElbow56,
    LElbowLWrist67,
    NeckMidHip18,
    MidHipRHip89,
    RHipRKnee910,
    RKneeRAnkle1011,
    MidHipLHip812,
    LHipLKnee1213,
    LKneeLAnkle1314,
    //REye,
    //LEye,
    //REar,
    //LEar,
    LAnkleLBigToe1122,
    //LSmallToe,
    //LHeel,
    RAnkleRBigToe1419
    //RSmallToe,
    //RHeel
};
