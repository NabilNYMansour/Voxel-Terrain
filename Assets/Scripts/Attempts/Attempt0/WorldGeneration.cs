using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    public int chunkSize;
    public int seed;
    public float stretchness;
    public float amplitude;
    public float shapness;
    public int octaves;
    public float vertexDistance;
    public Gradient gradient;
    float minVal;
    float maxVal;
    float spikeness = 10;


    Vector3[] GenerateChunck(Vector3[] nodesLocations)
    {
        Vector3[] map = new Vector3[chunkSize * chunkSize];
        int count = 0;
        for (int i = (int)nodesLocations[0].x; i < nodesLocations[3].x; i++)
        {
            for (int j = (int)nodesLocations[0].z; j < nodesLocations[3].z; j++)
            {
                float perlin = Mathf.PerlinNoise(seed + (i / stretchness), seed + (j / stretchness)) * spikeness * amplitude;
                if (perlin > maxVal)
                {
                    maxVal = perlin;
                }
                if (perlin < minVal)
                {
                    minVal = perlin;
                }
                map[count] = new Vector3(i, perlin, j);
                count++;
            }
        }
        return map;
    }

    Texture2D GenerateOctaveChunkTexture(Vector3[] map, int octaves, float divisionsCoeficient)
    {
        float originalStretchness = stretchness;
        float originalSpikeness = spikeness;

        List<Vector3[]> maps = new List<Vector3[]>();
        float divisions = 1;
        for (int i = 0; i < octaves; i++)
        {
            maps.Add(GenerateChunck(map));
            divisions *= divisionsCoeficient;
            stretchness = originalStretchness / divisions;
            spikeness = originalSpikeness / divisions;
        }

        Vector3[] finalMap = new Vector3[chunkSize * chunkSize];
        int count = 0;
        for (int i = 0; i < chunkSize; i++)
        {
            for (int j = 0; j < chunkSize; j++)
            {
                float height = 0;
                foreach (Vector3[] m in maps)
                {
                    height += m[count].y;
                }
                finalMap[count] = new Vector3(i, height, j);
                count++;
            }
        }
        Texture2D texture = new Texture2D(chunkSize, chunkSize);
        count = 0;
        for (int i = 0; i < chunkSize; i++)
        {
            for (int j = 0; j < chunkSize; j++)
            {
                float currentHeight = finalMap[count].y;
                float height = Mathf.InverseLerp(minVal + minVal, maxVal + maxVal, currentHeight);
                for (int k = 0; k < shapness; k++)
                {
                    height *= height;
                }
                height = Mathf.InverseLerp(0, 1, height);
                texture.SetPixel(i, j, new Color(height, height, height));
                count++;
            }
        }
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
                vertices.Add(new Vector3(i * vertexDistance, texture.GetPixel(i, j).b, j * vertexDistance));
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
                texture.SetPixel(i, j, gradient.Evaluate(texture.GetPixel(i,j).b));
            }
        }
        texture.Apply();
        mr.material.mainTexture = texture;
        CenterObject();
    }

    void SaveTexture(Texture2D texture, string pathWay)
    {
        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(pathWay, bytes);
    }

    void CenterObject()
    {
        gameObject.transform.position = new Vector3(((-chunkSize) + 1) / 2, 0, ((-chunkSize) + 1) / 2);
    }

    void Start()
    {
        Vector3[] map = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 0, chunkSize), new Vector3(chunkSize, 0, 0), new Vector3(chunkSize, 0, chunkSize) };
        //SaveTexture(GenerateOctaveChunkTexture(map, octaves, 2.5f), "Assets/Prefabs/Texture.jpg");
        MakeMesh(GenerateOctaveChunkTexture(map, octaves, 3f));
    }
}
