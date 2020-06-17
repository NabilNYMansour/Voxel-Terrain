using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class OldChunk : MonoBehaviour
{
    public Transform Block;
    public Material mat;
    public Gradient gradient;
    int chunkSize;
    public Vector3 chunkPos;
    public float[,,] blockTypeList;
    float minHeight;
    float maxHeight;
    MeshFilter mf;
    MeshRenderer mr;

    public void SetFields(Vector3 chunkPos, int chunkSize, float[,,] blockTypeList)
    {
        this.chunkPos = chunkPos;
        this.chunkSize = chunkSize;
        this.blockTypeList = blockTypeList;
    }

    public void MakeMesh()
    {
        mf = gameObject.GetComponent<MeshFilter>();
        mr = gameObject.GetComponent<MeshRenderer>();

        Mesh mesh = new Mesh();

        float blockSize = 0.5f;
        float heightDifference = 0.5f;

        List<Vector3> verticies = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int i = 0; i < chunkSize; i++)
        {
            for (int j = 0; j < chunkSize; j++)
            {
                for (int k = 0; k < chunkSize; k++)
                {
                    //Checking above:
                    if ((j < chunkSize - 1 && blockTypeList[i, j, k] > 0.5f && blockTypeList[i, j + 1, k] <= 0.5f) || (j == chunkSize - 1 && blockTypeList[i, j, k] > 0.5f && WorldChunkGeneration.perlin3D(i + chunkPos.x, j + chunkPos.y + 1, k + chunkPos.z, 8, 2, gameObject.transform.parent.GetComponent<WorldChunkGeneration>().frequency, gameObject.transform.parent.GetComponent<WorldChunkGeneration>().seed) <= 0.5))
                    {
                        Vector3 v1 = new Vector3(i - blockSize, j + heightDifference, k - blockSize);
                        verticies.Add(v1);
                        Vector3 v2 = new Vector3(i + blockSize, j + heightDifference, k - blockSize);
                        verticies.Add(v2);
                        Vector3 v3 = new Vector3(i - blockSize, j + heightDifference, k + blockSize);
                        verticies.Add(v3);
                        Vector3 v4 = new Vector3(i + blockSize, j + heightDifference, k + blockSize);
                        verticies.Add(v4);
                        triangles.AddRange(new int[] { verticies.LastIndexOf(v2), verticies.LastIndexOf(v1), verticies.LastIndexOf(v3), verticies.LastIndexOf(v4), verticies.LastIndexOf(v2), verticies.LastIndexOf(v3) });
                    }
                    // Checking below:
                    if ((j >= 1 && blockTypeList[i, j, k] > 0.5f && blockTypeList[i, j - 1, k] <= 0.5f) || (j == 0 && blockTypeList[i, j, k] > 0.5f && WorldChunkGeneration.perlin3D(i + chunkPos.x, j + chunkPos.y - 1, k + chunkPos.z, 8, 2, gameObject.transform.parent.GetComponent<WorldChunkGeneration>().frequency, gameObject.transform.parent.GetComponent<WorldChunkGeneration>().seed) <= 0.5))
                    {
                        Vector3 v5 = new Vector3(i - blockSize, j - heightDifference, k - blockSize);
                        verticies.Add(v5);
                        Vector3 v6 = new Vector3(i + blockSize, j - heightDifference, k - blockSize);
                        verticies.Add(v6);
                        Vector3 v7 = new Vector3(i - blockSize, j - heightDifference, k + blockSize);
                        verticies.Add(v7);
                        Vector3 v8 = new Vector3(i + blockSize, j - heightDifference, k + blockSize);
                        verticies.Add(v8);
                        triangles.AddRange(new int[] { verticies.LastIndexOf(v5), verticies.LastIndexOf(v6), verticies.LastIndexOf(v7), verticies.LastIndexOf(v7), verticies.LastIndexOf(v6), verticies.LastIndexOf(v8) });
                    }
                    // Checking forwards:
                    if ((k < chunkSize - 1 && blockTypeList[i, j, k] > 0.5f && blockTypeList[i, j, k + 1] <= 0.5f) || k == chunkSize - 1 && blockTypeList[i, j, k] > 0.5f && WorldChunkGeneration.perlin3D(i + chunkPos.x, j + chunkPos.y, k + chunkPos.z + 1, 8, 2, gameObject.transform.parent.GetComponent<WorldChunkGeneration>().frequency, gameObject.transform.parent.GetComponent<WorldChunkGeneration>().seed) <= 0.5)
                    {
                        Vector3 v3 = new Vector3(i - blockSize, j + heightDifference, k + blockSize);
                        verticies.Add(v3);
                        Vector3 v4 = new Vector3(i + blockSize, j + heightDifference, k + blockSize);
                        verticies.Add(v4);
                        Vector3 v7 = new Vector3(i - blockSize, j - heightDifference, k + blockSize);
                        verticies.Add(v7);
                        Vector3 v8 = new Vector3(i + blockSize, j - heightDifference, k + blockSize);
                        verticies.Add(v8);
                        triangles.AddRange(new int[] { verticies.LastIndexOf(v8), verticies.LastIndexOf(v3), verticies.LastIndexOf(v7), verticies.LastIndexOf(v4), verticies.LastIndexOf(v3), verticies.LastIndexOf(v8) });
                    }
                    // Checking backwards:
                    if ((k >= 1 && blockTypeList[i, j, k] > 0.5f && blockTypeList[i, j, k - 1] <= 0.5f) || k == 0 && blockTypeList[i, j, k] > 0.5f && WorldChunkGeneration.perlin3D(i + chunkPos.x, j + chunkPos.y, k + chunkPos.z - 1, 8, 2, gameObject.transform.parent.GetComponent<WorldChunkGeneration>().frequency, gameObject.transform.parent.GetComponent<WorldChunkGeneration>().seed) <= 0.5)
                    {
                        Vector3 v1 = new Vector3(i - blockSize, j + heightDifference, k - blockSize);
                        verticies.Add(v1);
                        Vector3 v2 = new Vector3(i + blockSize, j + heightDifference, k - blockSize);
                        verticies.Add(v2);
                        Vector3 v5 = new Vector3(i - blockSize, j - heightDifference, k - blockSize);
                        verticies.Add(v5);
                        Vector3 v6 = new Vector3(i + blockSize, j - heightDifference, k - blockSize);
                        verticies.Add(v6);
                        triangles.AddRange(new int[] { verticies.LastIndexOf(v5), verticies.LastIndexOf(v1), verticies.LastIndexOf(v6), verticies.LastIndexOf(v6), verticies.LastIndexOf(v1), verticies.LastIndexOf(v2) });
                    }
                    // Checking right:
                    if ((i < chunkSize - 1 && blockTypeList[i, j, k] > 0.5f && blockTypeList[i + 1, j, k] <= 0.5f) || i == chunkSize - 1 && blockTypeList[i, j, k] > 0.5f && WorldChunkGeneration.perlin3D(i + chunkPos.x + 1, j + chunkPos.y, k + chunkPos.z, 8, 2, gameObject.transform.parent.GetComponent<WorldChunkGeneration>().frequency, gameObject.transform.parent.GetComponent<WorldChunkGeneration>().seed) <= 0.5)
                    {
                        Vector3 v2 = new Vector3(i + blockSize, j + heightDifference, k - blockSize);
                        verticies.Add(v2);
                        Vector3 v4 = new Vector3(i + blockSize, j + heightDifference, k + blockSize);
                        verticies.Add(v4);
                        Vector3 v6 = new Vector3(i + blockSize, j - heightDifference, k - blockSize);
                        verticies.Add(v6);
                        Vector3 v8 = new Vector3(i + blockSize, j - heightDifference, k + blockSize);
                        verticies.Add(v8);
                        triangles.AddRange(new int[] { verticies.LastIndexOf(v2), verticies.LastIndexOf(v8), verticies.LastIndexOf(v6), verticies.LastIndexOf(v2), verticies.LastIndexOf(v4), verticies.LastIndexOf(v8) });
                    }
                    // Checking left:
                    if ((i >= 1 && blockTypeList[i, j, k] > 0.5f && blockTypeList[i - 1, j, k] <= 0.5f) || i == 0 && blockTypeList[i, j, k] > 0.5f && WorldChunkGeneration.perlin3D(i + chunkPos.x - 1, j + chunkPos.y, k + chunkPos.z, 8, 2, gameObject.transform.parent.GetComponent<WorldChunkGeneration>().frequency, gameObject.transform.parent.GetComponent<WorldChunkGeneration>().seed) <= 0.5)
                    {
                        Vector3 v1 = new Vector3(i - blockSize, j + heightDifference, k - blockSize);
                        verticies.Add(v1);
                        Vector3 v3 = new Vector3(i - blockSize, j + heightDifference, k + blockSize);
                        verticies.Add(v3);
                        Vector3 v5 = new Vector3(i - blockSize, j - heightDifference, k - blockSize);
                        verticies.Add(v5);
                        Vector3 v7 = new Vector3(i - blockSize, j - heightDifference, k + blockSize);
                        verticies.Add(v7);
                        triangles.AddRange(new int[] { verticies.LastIndexOf(v5), verticies.LastIndexOf(v7), verticies.LastIndexOf(v1), verticies.LastIndexOf(v7), verticies.LastIndexOf(v3), verticies.LastIndexOf(v1) });
                    }
                }
            }
        }

        List<Vector2> uvs = new List<Vector2>();
        for (int i = 0; i < verticies.Count; i++)
        {

        }

        mesh.vertices = verticies.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

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
