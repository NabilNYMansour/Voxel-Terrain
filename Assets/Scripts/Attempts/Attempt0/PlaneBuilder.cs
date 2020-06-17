using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneBuilder : MonoBehaviour
{
    public Transform dot;
    public float width;
    public float length;
    public float iteration;
    public Gradient gradient;

    void GenerateDots(float[] heightMap)
    {
        int count = 0;
        for (float i = 0; i < width; i += iteration)
        {
            for (float j = 0; j < length; j += iteration)
            {
                GameObject newObject = Instantiate(dot.gameObject, new Vector3(i, heightMap[count], j), Quaternion.identity, gameObject.transform);
                newObject.name = i.ToString() + ", " + j.ToString();
                count++;
            }
        }
    }

    static Vector3[] GenerateVertices(float width, float[] heightMap, float length, float iteration)
    {
        Vector3[] toReturn = new Vector3[(int)(width * length / iteration)];
        int count = 0;
        for (float i = 0; i < width; i += iteration)
        {
            for (float j = 0; j < length; j += iteration)
            {
                toReturn[count] = new Vector3(i, heightMap[count], j);
                count++;
            }
        }
        return toReturn;
    }

    static float[] zeroHeightMapMaker(float width, float length, float iteration)
    {
        float[] map = new float[(int)(width * length / iteration)];
        return map;
    }

    static float[] PerlinHeightMapMaker(float width, float length, float iteration, float spikeness)
    {
        float[] map = new float[(int)(width * length / iteration)];
        int count = 0;
        for (float i = 0; i < width; i += iteration)
        {
            for (float j = 0; j < length; j += iteration)
            {
                map[count] = Mathf.PerlinNoise(i / 10f, j / 10f) * spikeness;
                count++;
            }
        }
        return map;
    }

    float[] AddMorePerlin(float[] map, float spikeness, float amount)
    {
        int count = 0;
        for (float i = 0; i < width; i += iteration)
        {
            for (float j = 0; j < length; j += iteration)
            {
                map[count] += Mathf.PerlinNoise(i / amount, j / amount) * spikeness;
                count++;
            }
        }
        return map;
    }

    void MeshMaker(Vector3[] map)
    {
        MeshFilter mf = gameObject.AddComponent<MeshFilter>();
        MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();

        mesh.vertices = map;

        List<int> triangles = new List<int>();
        for (int k = 0; k < mesh.vertices.Length; k++)
        {
            if (k < length * width - width)
            {
                if (((int)((k + iteration) / width) - ((k + iteration) / width)) != 0)
                {
                    triangles.Add(k + 1);
                    triangles.Add(k + (int)width);
                    triangles.Add(k);

                    triangles.Add(k + 1);
                    triangles.Add(k + (int)width + 1);
                    triangles.Add(k + (int)width);
                }
            }
        }
        int[] tri = triangles.ToArray();
        mesh.triangles = tri;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        mf.mesh = mesh;

        List<Vector2> uvs = new List<Vector2>();
        for (int i = 0; i < map.Length; i++)
        {
            uvs.Add(new Vector2(map[i].x / width, map[i].z / width));
        }

        float min = map[0].y;
        float max = map[0].y;
        for (int i = 0; i < map.Length; i++)
        {
            if (min > map[i].y)
            {
                min = map[i].y;
            }
            if (max < map[i].y)
            {
                max = map[i].y;
            }
        }

        mesh.uv = uvs.ToArray();

        int x = (int)map[map.Length - 1].x;
        int y = (int)map[map.Length - 1].z;
        Texture2D texture = new Texture2D(x, y);
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                float currentHeight = FindHeight(map, i, j);
                float height = Mathf.InverseLerp(min, max, currentHeight);
                texture.SetPixel(i, j, gradient.Evaluate(height));
            }
        }
        texture.Apply();
        mr.material.mainTexture = texture;

        MeshCollider collider = gameObject.AddComponent<MeshCollider>();
        collider.sharedMesh = mesh;
    }

    float FindHeight(Vector3[] map, int xVal, int zVal)
    {
        foreach (Vector3 vec in map)
        {
            if ((int)vec.x == xVal && (int)vec.z == zVal)
            {
                return vec.y;
            }
        }
        return -1;
    }

    void CenterObject()
    {
        gameObject.transform.position = new Vector3(((-width) + 1) / 2, 0, ((-length) + 1) / 2);
    }

    void Start()
    {
        float[] zeroHeightMap = zeroHeightMapMaker(width, length, iteration);
        float[] perlinMap = PerlinHeightMapMaker(width, length, iteration, 10);
        perlinMap = AddMorePerlin(perlinMap, 3, 5f);
        //perlinMap = AddMorePerlin(perlinMap, 1, 5f);
        //perlinMap = AddMorePerlin(perlinMap, 1f, 3f);
        Vector3[] map = GenerateVertices(width, perlinMap, length, iteration);
        //GenerateDots(perlinMap);
        CenterObject();
        MeshMaker(map);
    }
}
