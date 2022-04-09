using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boidTest : MonoBehaviour
{

    // ���������Ǿ�̬�ģ�ֻ��Ҫ��ʼ��һ�Σ����е�Ԫ����
    private static Vector3[] m_ObstanceRayDirection = null;

    private void Awake()
    {

    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    void setObstanceRayDirection()
    {
        List<Vector3> dirs = new List<Vector3>();
        // ����ʹ��120���㣻ʵ����60������ΪҪȥ����������ߡ�
        for (int i = 1; i < 120; ++i)
        {
            float t = i / 119f; // 120 - 1
            float inc = Mathf.Acos(1f - 2f * t);
            float z = Mathf.Cos(inc);
            if (z > 0) // ֻ���ĳ�ǰ�ķ��򣬺��Գ���ķ���
            {
                float az = 2f * Mathf.PI * 1.618f * i; // �ƽ�ָ���+1
                float x = Mathf.Sin(inc) * Mathf.Cos(az);
                float y = Mathf.Sin(inc) * Mathf.Sin(az);
                dirs.Add(new Vector3(x, y, z).normalized);
            }
        }
        m_ObstanceRayDirection = dirs.ToArray();
    }


    // Update is called once per frame
    void Update()
    {
        setObstanceRayDirection();
        CheckObstances();

    }

    private Vector3 CheckObstances()
    {
        Vector3 bestDir = transform.forward;
        float maxDis = 0;
        foreach (var dir in m_ObstanceRayDirection)
        {
            Vector3 tdir = transform.TransformDirection(dir);
            Debug.DrawRay(transform.position, tdir, Color.red, 1f);
            var ray = new Ray(transform.position, transform.TransformDirection(dir));
            var result = Physics.RaycastAll(ray, 10f);
            foreach (RaycastHit raycastHit in result)
            {
                float dis = raycastHit.distance;
                if (dis > maxDis)
                {
                    bestDir = tdir;
                    maxDis = dis;
                }
                return tdir;
            }   
        }
        return bestDir;
    }

}
