using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour
{
    public float speed;
    public List<Collider> colliders;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //move();
        //if (transform.position.z <= 0.5f)
        //    Debug.Log(transform.position.z);
    }

    /*
     * 墙移动
     * 返回当前z轴的值
     */
    public float move()
    {
        Vector3 pos = transform.position;
        transform.Translate(Vector3.back * speed * Time.deltaTime);
        return transform.position.z;
    }
}
