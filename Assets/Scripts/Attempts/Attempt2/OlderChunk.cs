using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class OlderChunk : MonoBehaviour
{
    public Material mat;
    Vector3Int chunkPos;
    int chunkSize;
    int chunkHeight;
    FastNoise noiseGenerator;
    MeshFilter mf;
    MeshRenderer mr;
    Dictionary<(int, int, int), bool> map = new Dictionary<(int, int, int), bool>();

    public void SetChunk(Vector3Int chunkPos, int chunkWidth, int chunkHeight, FastNoise noiseGenerator)
    {
        this.chunkPos = chunkPos;
        this.chunkSize = chunkWidth;
        this.chunkHeight = chunkHeight;
        this.noiseGenerator = noiseGenerator;
    }

    void BuildAboveFace(int i, int j, int k, List<Vector3> verticies, List<int> triangles, float blockSize, float heightDifference)
    {
        Vector3 v1 = new Vector3(i - blockSize - chunkPos.x, j + heightDifference - chunkPos.y, k - blockSize - chunkPos.z);
        verticies.Add(v1);
        Vector3 v2 = new Vector3(i + blockSize - chunkPos.x, j + heightDifference - chunkPos.y, k - blockSize - chunkPos.z);
        verticies.Add(v2);
        Vector3 v3 = new Vector3(i - blockSize - chunkPos.x, j + heightDifference - chunkPos.y, k + blockSize - chunkPos.z);
        verticies.Add(v3);
        Vector3 v4 = new Vector3(i + blockSize - chunkPos.x, j + heightDifference - chunkPos.y, k + blockSize - chunkPos.z);
        verticies.Add(v4);
        triangles.AddRange(new int[] { verticies.LastIndexOf(v2), verticies.LastIndexOf(v1), verticies.LastIndexOf(v3), verticies.LastIndexOf(v4), verticies.LastIndexOf(v2), verticies.LastIndexOf(v3) });
    }
    void BuildBelowFace(int i, int j, int k, List<Vector3> verticies, List<int> triangles, float blockSize, float heightDifference)
    {
        Vector3 v5 = new Vector3(i - blockSize - chunkPos.x, j - heightDifference - chunkPos.y, k - blockSize - chunkPos.z);
        verticies.Add(v5);
        Vector3 v6 = new Vector3(i + blockSize - chunkPos.x, j - heightDifference - chunkPos.y, k - blockSize - chunkPos.z);
        verticies.Add(v6);
        Vector3 v7 = new Vector3(i - blockSize - chunkPos.x, j - heightDifference - chunkPos.y, k + blockSize - chunkPos.z);
        verticies.Add(v7);
        Vector3 v8 = new Vector3(i + blockSize - chunkPos.x, j - heightDifference - chunkPos.y, k + blockSize - chunkPos.z);
        verticies.Add(v8);
        triangles.AddRange(new int[] { verticies.LastIndexOf(v5), verticies.LastIndexOf(v6), verticies.LastIndexOf(v7), verticies.LastIndexOf(v7), verticies.LastIndexOf(v6), verticies.LastIndexOf(v8) });
    }
    void BuildForwardFace(int i, int j, int k, List<Vector3> verticies, List<int> triangles, float blockSize, float heightDifference)
    {
        Vector3 v3 = new Vector3(i - blockSize - chunkPos.x, j + heightDifference - chunkPos.y, k + blockSize - chunkPos.z);
        verticies.Add(v3);
        Vector3 v4 = new Vector3(i + blockSize - chunkPos.x, j + heightDifference - chunkPos.y, k + blockSize - chunkPos.z);
        verticies.Add(v4);
        Vector3 v7 = new Vector3(i - blockSize - chunkPos.x, j - heightDifference - chunkPos.y, k + blockSize - chunkPos.z);
        verticies.Add(v7);
        Vector3 v8 = new Vector3(i + blockSize - chunkPos.x, j - heightDifference - chunkPos.y, k + blockSize - chunkPos.z);
        verticies.Add(v8);
        triangles.AddRange(new int[] { verticies.LastIndexOf(v8), verticies.LastIndexOf(v3), verticies.LastIndexOf(v7), verticies.LastIndexOf(v4), verticies.LastIndexOf(v3), verticies.LastIndexOf(v8) });
    }
    void BuildBackwardFace(int i, int j, int k, List<Vector3> verticies, List<int> triangles, float blockSize, float heightDifference)
    {
        Vector3 v1 = new Vector3(i - blockSize - chunkPos.x, j + heightDifference - chunkPos.y, k - blockSize - chunkPos.z);
        verticies.Add(v1);
        Vector3 v2 = new Vector3(i + blockSize - chunkPos.x, j + heightDifference - chunkPos.y, k - blockSize - chunkPos.z);
        verticies.Add(v2);
        Vector3 v5 = new Vector3(i - blockSize - chunkPos.x, j - heightDifference - chunkPos.y, k - blockSize - chunkPos.z);
        verticies.Add(v5);
        Vector3 v6 = new Vector3(i + blockSize - chunkPos.x, j - heightDifference - chunkPos.y, k - blockSize - chunkPos.z);
        verticies.Add(v6);
        triangles.AddRange(new int[] { verticies.LastIndexOf(v5), verticies.LastIndexOf(v1), verticies.LastIndexOf(v6), verticies.LastIndexOf(v6), verticies.LastIndexOf(v1), verticies.LastIndexOf(v2) });
    }
    void BuildRightFace(int i, int j, int k, List<Vector3> verticies, List<int> triangles, float blockSize, float heightDifference)
    {
        Vector3 v2 = new Vector3(i + blockSize - chunkPos.x, j + heightDifference - chunkPos.y, k - blockSize - chunkPos.z);
        verticies.Add(v2);
        Vector3 v4 = new Vector3(i + blockSize - chunkPos.x, j + heightDifference - chunkPos.y, k + blockSize - chunkPos.z);
        verticies.Add(v4);
        Vector3 v6 = new Vector3(i + blockSize - chunkPos.x, j - heightDifference - chunkPos.y, k - blockSize - chunkPos.z);
        verticies.Add(v6);
        Vector3 v8 = new Vector3(i + blockSize - chunkPos.x, j - heightDifference - chunkPos.y, k + blockSize - chunkPos.z);
        verticies.Add(v8);
        triangles.AddRange(new int[] { verticies.LastIndexOf(v2), verticies.LastIndexOf(v8), verticies.LastIndexOf(v6), verticies.LastIndexOf(v2), verticies.LastIndexOf(v4), verticies.LastIndexOf(v8) });
    }
    void BuildLeftFace(int i, int j, int k, List<Vector3> verticies, List<int> triangles, float blockSize, float heightDifference)
    {
        Vector3 v1 = new Vector3(i - blockSize - chunkPos.x, j + heightDifference - chunkPos.y, k - blockSize - chunkPos.z);
        verticies.Add(v1);
        Vector3 v3 = new Vector3(i - blockSize - chunkPos.x, j + heightDifference - chunkPos.y, k + blockSize - chunkPos.z);
        verticies.Add(v3);
        Vector3 v5 = new Vector3(i - blockSize - chunkPos.x, j - heightDifference - chunkPos.y, k - blockSize - chunkPos.z);
        verticies.Add(v5);
        Vector3 v7 = new Vector3(i - blockSize - chunkPos.x, j - heightDifference - chunkPos.y, k + blockSize - chunkPos.z);
        verticies.Add(v7);
        triangles.AddRange(new int[] { verticies.LastIndexOf(v5), verticies.LastIndexOf(v7), verticies.LastIndexOf(v1), verticies.LastIndexOf(v7), verticies.LastIndexOf(v3), verticies.LastIndexOf(v1) });

    }

    public void SetNoiseMap()
    {
        for (int i = chunkPos.x - 1; i < chunkPos.x + chunkSize + 1; i++)
        {
            for (int j = chunkPos.y - 1; j < chunkPos.y + chunkHeight + 1; j++)
            {
                for (int k = chunkPos.z - 1; k < chunkPos.z + chunkSize + 1; k++)
                {
                    if (Mathf.InverseLerp(chunkPos.y - 1, chunkPos.y + chunkHeight + 1, noiseGenerator.GetNoise(i, k)) == j)
                    {
                        map.Add((i - chunkPos.x - 1, j - chunkPos.y - 1, k - chunkPos.z - 1), true);
                    }
                    else
                    {
                        map.Add((i - chunkPos.x - 1, j - chunkPos.y - 1, k - chunkPos.z - 1), false);
                    }
                    //print((i - chunkPos.x - 1, j - chunkPos.y - 1, k - chunkPos.z - 1));
                }
            }
        }
    }

    public void BuildChunk()
    {
        mf = gameObject.GetComponent<MeshFilter>();
        mr = gameObject.GetComponent<MeshRenderer>();

        Mesh mesh = new Mesh();

        float blockSize = 0.5f;
        float heightDifference = 0.5f;

        List<Vector3> verticies = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int i = chunkPos.x; i < chunkPos.x + chunkSize; i++)
        {
            for (int j = chunkPos.y; j < chunkPos.y + chunkHeight; j++)
            {
                for (int k = chunkPos.z; k < chunkPos.z + chunkSize; k++)
                {
                    print(i + ", " + j + ", " + k);
                    if (map[(i - chunkPos.x, j - chunkPos.y, k - chunkPos.z - 1)] && !map[(i - chunkPos.x, j - chunkPos.y + 1, k - chunkPos.z - 1)])
                    {
                        BuildAboveFace(i, j, k, verticies, triangles, blockSize, heightDifference);
                    }
                }
            }
        }



        mesh.vertices = verticies.ToArray();
        mesh.triangles = triangles.ToArray();
        //mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();
        mf.mesh = mesh;
        mr.material = mat;
    }

    void FixedUpdate()
    {
        // 0,0,0
        Debug.DrawLine(chunkPos, chunkPos + (Vector3.forward * chunkSize));
        Debug.DrawLine(chunkPos, chunkPos + (Vector3.up * chunkSize));
        Debug.DrawLine(chunkPos, chunkPos + (Vector3.right * chunkSize));

        // 1,1,1
        Debug.DrawLine(chunkPos + (Vector3.one * chunkSize), chunkPos + (new Vector3(0, 1, 1) * chunkSize));
        Debug.DrawLine(chunkPos + (Vector3.one * chunkSize), chunkPos + (new Vector3(1, 1, 0) * chunkSize));
        Debug.DrawLine(chunkPos + (Vector3.one * chunkSize), chunkPos + (new Vector3(1, 0, 1) * chunkSize));

        // Rest
        Debug.DrawLine(chunkPos + (new Vector3(0, 1, 0) * chunkSize), chunkPos + (new Vector3(0, 1, 1) * chunkSize));
        Debug.DrawLine(chunkPos + (new Vector3(0, 1, 1) * chunkSize), chunkPos + (new Vector3(0, 0, 1) * chunkSize));
        Debug.DrawLine(chunkPos + (new Vector3(0, 0, 1) * chunkSize), chunkPos + (new Vector3(1, 0, 1) * chunkSize));
        Debug.DrawLine(chunkPos + (new Vector3(1, 0, 1) * chunkSize), chunkPos + (new Vector3(1, 0, 0) * chunkSize));
        Debug.DrawLine(chunkPos + (new Vector3(1, 0, 0) * chunkSize), chunkPos + (new Vector3(1, 1, 0) * chunkSize));
        Debug.DrawLine(chunkPos + (new Vector3(1, 1, 0) * chunkSize), chunkPos + (new Vector3(0, 1, 0) * chunkSize));
    }
}
