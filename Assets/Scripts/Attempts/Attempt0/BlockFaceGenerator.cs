using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class BlockFaceGenerator : MonoBehaviour
{
    float blockSize = 0.5f;
    public Material mat;
    public void SetMesh(bool[] faces, float heightDifference)
    // 0 is top, 1 is bottom, 2 is forward, 3 is backwards, 4 is right, 5 is left.
    {
        Mesh mesh = new Mesh();
        Vector3 v1 = new Vector3(transform.position.x - blockSize, transform.position.y + heightDifference, transform.position.z - blockSize);
        Vector3 v2 = new Vector3(transform.position.x + blockSize, transform.position.y + heightDifference, transform.position.z - blockSize);
        Vector3 v3 = new Vector3(transform.position.x - blockSize, transform.position.y + heightDifference, transform.position.z + blockSize);
        Vector3 v4 = new Vector3(transform.position.x + blockSize, transform.position.y + heightDifference, transform.position.z + blockSize);

        Vector3 v5 = new Vector3(transform.position.x - blockSize, transform.position.y - heightDifference, transform.position.z - blockSize);
        Vector3 v6 = new Vector3(transform.position.x + blockSize, transform.position.y - heightDifference, transform.position.z - blockSize);
        Vector3 v7 = new Vector3(transform.position.x - blockSize, transform.position.y - heightDifference, transform.position.z + blockSize);
        Vector3 v8 = new Vector3(transform.position.x + blockSize, transform.position.y - heightDifference, transform.position.z + blockSize);

        mesh.vertices = new Vector3[] { v1, v2, v3, v4, v5, v6, v7, v8 };

        List<int> triangles = new List<int>();
        if (faces[0])
        {
            triangles.AddRange(new int[] { 0, 2, 1, 2, 3, 1 });
        }

        if (faces[1])
        {
            triangles.AddRange(new int[] { 4, 5, 6, 6, 5, 7 });
        }

        if (faces[2])
        {
            triangles.AddRange(new int[] { 7, 2, 6, 3, 2, 7 });
        }

        if (faces[3])
        {
            triangles.AddRange(new int[] { 4, 0, 5, 5, 0, 1 });
        }

        if (faces[4])
        {
            triangles.AddRange(new int[] { 1, 7, 5, 1, 3, 7 });
        }

        if (faces[5])
        {
            triangles.AddRange(new int[] { 4, 6, 0, 6, 2, 0 });
        }
        mesh.triangles = triangles.ToArray();

        List<Vector2> uvs = new List<Vector2>();
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            uvs.Add(mesh.vertices[i] / mesh.vertices.Length);
        }

        mesh.uv = uvs.ToArray();

        //List<Vector3> normals = new List<Vector3>();
        //for (int i = 0; i < mesh.vertices.Length; i++)
        //{
        //    normals.Add()
        //}

        //mesh.normals = normals.ToArray();

        //mesh.RecalculateBounds();
        mesh.Optimize();
        mesh.RecalculateNormals();

        MeshFilter mf = gameObject.GetComponent<MeshFilter>();
        mf.transform.position = Vector3.zero;
        gameObject.transform.position = Vector3.zero;
        mf.mesh = mesh;

        MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
        mr.material = mat;

        //     Mesh mesh = new Mesh();

        //     Vector3 vertLeftTopFront = new Vector3(-1, 1, 1);
        //     Vector3 vertRightTopFront = new Vector3(1, 1, 1);
        //     Vector3 vertRightTopBack = new Vector3(1, 1, -1);
        //     Vector3 vertLeftTopBack = new Vector3(-1, 1, -1);

        //     //Vertices//
        //     Vector3[] vertices = new Vector3[]
        //         {
        ////front face//
        //vertLeftTopFront,//left top front, 0
        //vertRightTopFront,//right top front, 1
        //new Vector3(-1,-1,1),//left bottom front, 2
        //new Vector3(1,-1,1),//right bottom front, 3

        ////back face//
        //vertRightTopBack,//right top back, 4
        //vertLeftTopBack,//left top back, 5
        //new Vector3(1,-1,-1),//right bottom back, 6
        //new Vector3(-1,-1,-1),//left bottom back, 7

        ////left face//
        //vertLeftTopBack,//left top back, 8
        //vertLeftTopFront,//left top front, 9
        //new Vector3(-1,-1,-1),//left bottom back, 10
        //new Vector3(-1,-1,1),//left bottom front, 11

        ////right face//
        //vertRightTopFront,//right top front, 12
        //vertRightTopBack,//right top back, 13
        //new Vector3(1,-1,1),//right bottom front, 14
        //new Vector3(1,-1,-1),//right bottom back, 15

        ////top face//
        //vertLeftTopBack,//left top back, 16
        //vertRightTopBack,//right top back, 17
        //vertLeftTopFront,//left top front, 18
        //vertRightTopFront,//right top front, 19

        ////bottom face//
        //new Vector3(-1,-1,1),//left bottom front, 20
        //new Vector3(1,-1,1),//right bottom front, 21
        //new Vector3(-1,-1,-1),//left bottom back, 22
        //new Vector3(1,-1,-1)//right bottom back, 23

        //         };

        //     //Triangles// 3 points, clockwise determines which side is visible
        //     int[] triangles = new int[]
        //     {
        ////front face//
        //0,2,3,//first triangle
        //3,1,0,//second triangle

        ////back face//
        //4,6,7,//first triangle
        //7,5,4,//second triangle

        ////left face//
        //8,10,11,//first triangle
        //11,9,8,//second triangle

        ////right face//
        //12,14,15,//first triangle
        //15,13,12,//second triangle

        ////top face//
        //16,18,19,//first triangle
        //19,17,16,//second triangle

        ////bottom face//
        //20,22,23,//first triangle
        //23,21,20//second triangle
        //     };

        //     //UVs//
        //     Vector2[] uvs = new Vector2[]
        //     {
        ////front face// 0,0 is bottom left, 1,1 is top right//
        //new Vector2(0,1),
        //         new Vector2(0,0),
        //         new Vector2(1,1),
        //         new Vector2(1,0),

        //         new Vector2(0,1),
        //         new Vector2(0,0),
        //         new Vector2(1,1),
        //         new Vector2(1,0),

        //         new Vector2(0,1),
        //         new Vector2(0,0),
        //         new Vector2(1,1),
        //         new Vector2(1,0),

        //         new Vector2(0,1),
        //         new Vector2(0,0),
        //         new Vector2(1,1),
        //         new Vector2(1,0),

        //         new Vector2(0,1),
        //         new Vector2(0,0),
        //         new Vector2(1,1),
        //         new Vector2(1,0),

        //         new Vector2(0,1),
        //         new Vector2(0,0),
        //         new Vector2(1,1),
        //         new Vector2(1,0)
        //     };

        //     mesh.Clear();
        //     mesh.vertices = vertices;
        //     mesh.triangles = triangles;
        //     mesh.uv = uvs;
        //     mesh.Optimize();
        //     mesh.RecalculateNormals();

        //     MeshFilter mf = gameObject.GetComponent<MeshFilter>();
        //     //mf.transform.position = Vector3.zero;
        //     //gameObject.transform.position = Vector3.zero;
        //     mf.mesh = mesh;

        //     MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
        //     mr.material = mat;
    }

    // Start is called before the first frame update
    void Start()
    {
        //SetMesh(new bool[] { true, true, true, true, true, true }, 0.5f);
    }
}
