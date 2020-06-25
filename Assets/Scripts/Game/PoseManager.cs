using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoseManager : MonoBehaviour
{
    //---------------------------KeyPoints----------------------------
    List<Vector3> keyPoints;
    //---------------------------People----------------------------
    public GameObject people;
    //---------------------------Error----------------------------
    public float error = 0.05f; // 姿势误差，设成多少合适呢？
    float timer = 0.0f;  

    void Update()
    {
        //---------------------------Set KeyPoints----------------------------
        keyPoints = people.GetComponent<People>().points;
    }

    /* 
     * 检查是否做了准备动作（T Pose)并保持一段时间
     * 如果做了符合的动作，timer++
     * 否则timer清零
     * 返回当前计时或错误提示
     * -1: 手臂伸平
     * -2: 身体对准墙
     */
    public float Prepare(List<Collider> colliders)
    {
        if (keyPoints.Count < 10)
        {
            return -2;
        }
        // 首先直接检测手臂是否伸平
        if (keyPoints[2].y - keyPoints[3].y > error
            || keyPoints[3].y - keyPoints[4].y > error
            || keyPoints[5].y - keyPoints[6].y > error
            || keyPoints[6].y - keyPoints[7].y > error)
        {
            timer = 0;
            return -1;
        }

        // 手臂伸平后判断整体位置
        //---------------------------Skip Small Keypoints (index > 14)----------------------------
        for (int i = 0; i < 15; i++)
        {
            Vector3 point = keyPoints[i];

            bool wall = false;
            bool hole = false;
            for (int j = 0; j < colliders.Count; j++)
            {
                Collider collider = colliders[j];
                bool hit = collider.bounds.Contains(point);
                // 在墙范围内
                if (collider.tag == "Wall" && collider.bounds.Contains(point))
                {
                    wall = true;
                }
                // 在洞的范围内
                if (collider.tag == "Hole" && collider.bounds.Contains(point))
                {
                    hole = true;
                    break;
                }
                // 位置在洞的范围内,提前结束这一层循环
                if (wall && hole)
                {
                    break;
                }
            }
            // 有一个点在外面就结束判断
            if (!hole)
            {
                timer = 0;
                return -2;
            }
        }
        timer += Time.deltaTime;
        return timer;
    }

    // 清空准备时的计时
    public float ClearTimer()
    {
        timer = 0;
        return timer;
    }

    /* 
     * 判断关键点与墙的位置
     * 关键点同时与“wall”和“hole”相交说明通过
     * 只和“wall”相交说明撞墙
     * 都不相交可能是位置太偏或者判断太早（需要其他函数来判断）
     * 
     * 返回一个分数
     */
    public float Play(List<Collider> colliders)
    {
        float score = 0;
        for (int i = 0; i < 15; i++)
        {
            Vector3 point = keyPoints[i];
            bool wall = false;
            bool hole = false;
            for (int j = 0; j < colliders.Count; j++)
            {
                Collider collider = colliders[j];
                if (collider.tag == "Wall" && collider.bounds.Contains(point))
                {
                    wall = true;
                }
                if (collider.tag == "Hole" && collider.bounds.Contains(point))
                {
                    hole = true;
                }
            }
            if (wall && hole)
            {
                score += 1;
            }
            else
            {
                //keyBalls[i].GetComponent<MeshRenderer>().material = badMat;
            }
            //else if(wall && !hole)
            //{
            //    Debug.Log("hit the wall at " + point);
            //}
            //else
            //{
            //    Debug.Log("error "+i+" "+point);
            //}
        }
        return score;
    }
}

// 25个关键点索引对应的身体部位
public enum KeypointIdx_25
{
    Nose,      // 0
    Neck,      // 1
    RShoulder, // 2
    RElbow,    // 3
    RWrist,    // 4
    LShoulder, // 5
    LElbow,    // 6
    LWrist,    // 7
    MidHip,    // 8
    RHip,      // 9
    RKnee,     // 10
    RAnkle,    // 11
    LHip,      // 12
    LKnee,     // 13
    LAnkle,    // 14
    REye,      // 15
    LEye,      // 16
    REar,      // 17
    LEar,      // 18
    LBigToe,   // 19
    LSmallToe, // 20
    LHeel,     // 21
    RBigToe,   // 22
    RSmallToe, // 23
    RHeel      // 24
}