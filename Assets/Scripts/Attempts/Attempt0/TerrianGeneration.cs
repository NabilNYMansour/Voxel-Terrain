using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrianGeneration : MonoBehaviour
{
    public int chunkSize;
    public float stretchness;
    public float spikeness;
    public int seed;
    public int octaves;
    public float divisionCoef;
    public float vertexDistance;
    public Gradient gradient;
    // For Texture
    float minVal;
    float maxVal;

    Dictionary<(int, int), float> GeneratePerlinMap(int octaves, float divisionCoef)
    {
        float originalStretchness = stretchness;
        Dictionary<(int, int), float> map = new Dictionary<(int, int), float>();
        for (int k = 1; k <= octaves; k++)
        {
            for (int i = 0; i < chunkSize; i++)
            {
                for (int j = 0; j < chunkSize; j++)
                {
                    float newVal = Mathf.PerlinNoise(seed + (i / stretchness), seed + (j / stretchness));
                    if (k == 1)
                    {
                        map.Add((i, j), newVal);
                    }
                    else
                    {
                        map[(i, j)] += newVal / (2 * k);
                    }
                }
            }
            stretchness = originalStretchness / (divisionCoef * k);
        }
        return map;
    }

    Texture2D MapToTexture(Dictionary<(int, int), float> map)
    {
        // Finding min and max values:
        for (int i = 0; i < chunkSize; i++)
        {
            for (int j = 0; j < chunkSize; j++)
            {
                float val = 0;
                map.TryGetValue((i, j), out val);
                if (val > maxVal)
                {
                    maxVal = val;
                }
                if (val < minVal)
                {
                    minVal = val;
                }
            }
        }
        Texture2D texture = new Texture2D(chunkSize, chunkSize);
        for (int i = 0; i < chunkSize; i++)
        {
            for (int j = 0; j < chunkSize; j++)
            {
                float height = 0;
                map.TryGetValue((i, j), out height);
                height = Mathf.InverseLerp(minVal, maxVal, height);
                texture.SetPixel(i, j, new Color(height, height, height));
            }
        }
        texture.Apply();
        return texture;
    }

    void MakeMesh(Texture2D texture)
    {
        MeshFilter mf = gameObject.AddComponent<MeshFilter>();
        MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        for (int i = 0; i < texture.width; i++)
        {
            for (int j = 0; j < texture.width; j++)
            {
                float h = texture.GetPixel(i, j).b;
                vertices.Add(new Vector3(i * vertexDistance, h * spikeness, j * vertexDistance));
            }
        }

        mesh.vertices = vertices.ToArray();

        List<int> triangles = new List<int>();
        for (int k = 0; k < mesh.vertices.Length; k++)
        {
            if (k < (chunkSize * chunkSize) - chunkSize)
            {
                if (((int)((k + 1) / (float)chunkSize) - ((k + 1) / (float)chunkSize)) != 0)
                {
                    triangles.Add(k);
                    triangles.Add(k + chunkSize + 1);
                    triangles.Add(k + chunkSize);

                    triangles.Add(k);
                    triangles.Add(k + 1);
                    triangles.Add(k + chunkSize + 1);
                }
            }
        }
        int[] tri = triangles.ToArray();
        mesh.triangles = tri;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        mf.mesh = mesh;

        List<Vector2> uvs = new List<Vector2>();
        for (int i = 0; i < chunkSize; i++)
        {
            for (int j = 0; j < chunkSize; j++)
            {
                uvs.Add(new Vector2(i / (float)chunkSize, j / (float)chunkSize));
            }
        }
        mesh.uv = uvs.ToArray();

        for (int i = 0; i < chunkSize; i++)
        {
            for (int j = 0; j < chunkSize; j++)
            {
                texture.SetPixel(i, j, gradient.Evaluate(texture.GetPixel(i, j).b));
            }
        }
        texture.Apply();
        mr.material.mainTexture = texture;
    }

    void SaveTexture(Texture2D texture, string pathWay)
    {
        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(pathWay, bytes);
    }

    void Start()
    {
        Texture2D map = MapToTexture(GeneratePerlinMap(octaves, divisionCoef));
        SaveTexture(map, "Assets/Prefabs/Texture.jpg");
        MakeMesh(map);
    }
}
