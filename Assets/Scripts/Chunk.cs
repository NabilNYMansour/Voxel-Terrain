using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Chunk : MonoBehaviour
{
    public Vector3Int chunkPos;
    int chunkSize;
    int chunkHeight;
    float amplitude;
    float originalFrequency;
    int originalOctaves;
    Material mat;
    FastNoise noise;
    public Dictionary<Vector3Int, (float, float)> voxels = new Dictionary<Vector3Int, (float, float)>();
    public Dictionary<Vector3Int, (float, float)> voxelsTotal = new Dictionary<Vector3Int, (float, float)>();

    void SetParameters(int chunkSize, int chunkHeight, float amplitude, Material mat, FastNoise noise, Vector3Int chunkPos, float originalFrequency, int originalOctaves)
    {
        this.chunkSize = chunkSize;
        this.chunkHeight = chunkHeight;
        this.amplitude = amplitude;
        this.mat = mat;
        this.noise = noise;
        this.chunkPos = chunkPos;
        this.originalFrequency = originalFrequency;
        this.originalOctaves = originalOctaves;
    }

    void BuildAboveFace(int i, int j, int k, List<Vector3> verticies, List<int> triangles, List<Vector2> uvs, float blockSize, float heightDifference, float maxUV, float minUV)
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
        uvs.AddRange(new Vector2[] { new Vector2(maxUV, maxUV), new Vector2(minUV, maxUV), new Vector2(maxUV, minUV), new Vector2(minUV, minUV) });
    }
    void BuildBelowFace(int i, int j, int k, List<Vector3> verticies, List<int> triangles, List<Vector2> uvs, float blockSize, float heightDifference, float maxUV, float minUV)
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
        uvs.AddRange(new Vector2[] { new Vector2(maxUV, minUV), new Vector2(minUV, minUV), new Vector2(maxUV, maxUV), new Vector2(minUV, maxUV) });
    }
    void BuildForwardFace(int i, int j, int k, List<Vector3> verticies, List<int> triangles, List<Vector2> uvs, float blockSize, float heightDifference, float maxUV, float minUV)
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
        uvs.AddRange(new Vector2[] { new Vector2(maxUV, maxUV), new Vector2(minUV, maxUV), new Vector2(maxUV, minUV), new Vector2(minUV, minUV) });
    }
    void BuildBackwardFace(int i, int j, int k, List<Vector3> verticies, List<int> triangles, List<Vector2> uvs, float blockSize, float heightDifference, float maxUV, float minUV)
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
        uvs.AddRange(new Vector2[] { new Vector2(maxUV, minUV), new Vector2(minUV, minUV), new Vector2(maxUV, maxUV), new Vector2(minUV, maxUV) });
    }
    void BuildRightFace(int i, int j, int k, List<Vector3> verticies, List<int> triangles, List<Vector2> uvs, float blockSize, float heightDifference, float maxUV, float minUV)
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
        uvs.AddRange(new Vector2[] { new Vector2(minUV, maxUV), new Vector2(maxUV, maxUV), new Vector2(minUV, minUV), new Vector2(maxUV, minUV) });
    }
    void BuildLeftFace(int i, int j, int k, List<Vector3> verticies, List<int> triangles, List<Vector2> uvs, float blockSize, float heightDifference, float maxUV, float minUV)
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
        uvs.AddRange(new Vector2[] { new Vector2(maxUV, maxUV), new Vector2(minUV, maxUV), new Vector2(maxUV, minUV), new Vector2(minUV, minUV) });
    }

    void GenerateVoxel()
    {
        MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
        MeshCollider collider = gameObject.GetComponent<MeshCollider>();
        MeshFilter mf = gameObject.GetComponent<MeshFilter>();

        List<Vector3> verticies = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        int minSurfaceHeight = chunkHeight / 3;
        float minUndergroundVal = -0.6f;

        float UVminVal = 0;
        float UVmaxVal = 0;

        System.Random random = new System.Random(noise.GetSeed());

        for (int i = 0; i < chunkSize; i++)
        {
            for (int j = 0; j < chunkSize; j++)
            {
                noise.SetFrequency(1f / originalFrequency);
                noise.SetFractalOctaves(originalOctaves);
                noise.SetFractalLacunarity(2);
                noise.SetFractalGain(0.5f);

                int height = (int)Math.Ceiling((noise.GetNoise(i + chunkPos.x, minSurfaceHeight, j + chunkPos.z) * amplitude)) + chunkHeight / 2;
                int forwardHeight = (int)Math.Ceiling((noise.GetNoise(i + chunkPos.x, minSurfaceHeight, j + chunkPos.z + 1) * amplitude)) + chunkHeight / 2;
                int backwardHeight = (int)Math.Ceiling((noise.GetNoise(i + chunkPos.x, minSurfaceHeight, j + chunkPos.z - 1) * amplitude)) + chunkHeight / 2;
                int rightHeight = (int)Math.Ceiling((noise.GetNoise(i + chunkPos.x + 1, minSurfaceHeight, j + chunkPos.z) * amplitude)) + chunkHeight / 2;
                int leftHeight = (int)Math.Ceiling((noise.GetNoise(i + chunkPos.x - 1, minSurfaceHeight, j + chunkPos.z) * amplitude)) + chunkHeight / 2;

                noise.SetFrequency(2f / originalFrequency);
                noise.SetFractalOctaves(2);
                noise.SetFractalLacunarity(5f);
                noise.SetFractalGain(1f);

                // If certain structure placement:
                if (height >= forwardHeight && height >= backwardHeight && height >= rightHeight && height >= leftHeight && random.Next(0, 10) > 8 && !voxels.ContainsKey(new Vector3Int(i, height + 1, j + 1)) && !voxels.ContainsKey(new Vector3Int(i, height + 1, j - 1)) && !voxels.ContainsKey(new Vector3Int(i + 1, height + 1, j)) && !voxels.ContainsKey(new Vector3Int(i - 1, height + 1, j)) && noise.GetNoise(i + chunkPos.x, height, j + chunkPos.z) > minUndergroundVal && i > 0 && i < chunkSize - 1 && j > 0 && j < chunkSize - 1)
                {
                    // Tree
                    UVminVal = 0.8f;
                    UVmaxVal = 1f;
                    int randomStructureHeight = random.Next(0, 6) - random.Next(1, 2);
                    for (int l = 1; l < 4 + randomStructureHeight; l++)
                    {
                        // Making a tall base:
                        voxels.Add(new Vector3Int(i, l + height, j), (UVminVal, UVmaxVal));
                        if (l == 3 + randomStructureHeight)
                        {
                            BuildAboveFace(i, l + height, j, verticies, triangles, uvs, 0.5f, 0.5f, UVmaxVal, UVminVal);
                        }
                        if (l < 3 + randomStructureHeight)
                        {
                            BuildForwardFace(i, l + height, j, verticies, triangles, uvs, 0.5f, 0.5f, UVmaxVal, UVminVal);
                            BuildBackwardFace(i, l + height, j, verticies, triangles, uvs, 0.5f, 0.5f, UVmaxVal, UVminVal);
                            BuildRightFace(i, l + height, j, verticies, triangles, uvs, 0.5f, 0.5f, UVmaxVal, UVminVal);
                            BuildLeftFace(i, l + height, j, verticies, triangles, uvs, 0.5f, 0.5f, UVmaxVal, UVminVal);
                        }
                    }

                    // Making an extension at the top

                    // Leaves
                    UVminVal = 0.6f;
                    UVmaxVal = 0.8f;
                    // Right
                    if (!voxels.ContainsKey(new Vector3Int(i + 1, 3 + randomStructureHeight + height, j)))
                    {
                        voxels.Add(new Vector3Int(i + 1, 3 + randomStructureHeight + height, j), (UVminVal, UVmaxVal));

                        BuildAboveFace(i + 1, 3 + randomStructureHeight + height, j, verticies, triangles, uvs, 0.5f, 0.5f, UVmaxVal, UVminVal);
                        BuildBelowFace(i + 1, 3 + randomStructureHeight + height, j, verticies, triangles, uvs, 0.5f, 0.5f, UVmaxVal, UVminVal);
                        BuildRightFace(i + 1, 3 + randomStructureHeight + height, j, verticies, triangles, uvs, 0.5f, 0.5f, UVmaxVal, UVminVal);
                        BuildForwardFace(i + 1, 3 + randomStructureHeight + height, j, verticies, triangles, uvs, 0.5f, 0.5f, UVmaxVal, UVminVal);
                        BuildBackwardFace(i + 1, 3 + randomStructureHeight + height, j, verticies, triangles, uvs, 0.5f, 0.5f, UVminVal, UVmaxVal);
                    }
                    // Left
                    if (!voxels.ContainsKey(new Vector3Int(i - 1, 3 + randomStructureHeight + height, j)))
                    {
                        voxels.Add(new Vector3Int(i - 1, 3 + randomStructureHeight + height, j), (UVminVal, UVmaxVal));

                        BuildAboveFace(i - 1, 3 + randomStructureHeight + height, j, verticies, triangles, uvs, 0.5f, 0.5f, UVmaxVal, UVminVal);
                        BuildBelowFace(i - 1, 3 + randomStructureHeight + height, j, verticies, triangles, uvs, 0.5f, 0.5f, UVmaxVal, UVminVal);
                        BuildLeftFace(i - 1, 3 + randomStructureHeight + height, j, verticies, triangles, uvs, 0.5f, 0.5f, UVmaxVal, UVminVal);
                        BuildForwardFace(i - 1, 3 + randomStructureHeight + height, j, verticies, triangles, uvs, 0.5f, 0.5f, UVmaxVal, UVminVal);
                        BuildBackwardFace(i - 1, 3 + randomStructureHeight + height, j, verticies, triangles, uvs, 0.5f, 0.5f, UVminVal, UVmaxVal);
                    }
                    // Forwards
                    if (!voxels.ContainsKey(new Vector3Int(i, 3 + randomStructureHeight + height, j + 1)))
                    {
                        voxels.Add(new Vector3Int(i, 3 + randomStructureHeight + height, j + 1), (UVminVal, UVmaxVal));

                        BuildAboveFace(i, 3 + randomStructureHeight + height, j + 1, verticies, triangles, uvs, 0.5f, 0.5f, UVmaxVal, UVminVal);
                        BuildBelowFace(i, 3 + randomStructureHeight + height, j + 1, verticies, triangles, uvs, 0.5f, 0.5f, UVmaxVal, UVminVal);
                        BuildForwardFace(i, 3 + randomStructureHeight + height, j + 1, verticies, triangles, uvs, 0.5f, 0.5f, UVmaxVal, UVminVal);
                        BuildRightFace(i, 3 + randomStructureHeight + height, j + 1, verticies, triangles, uvs, 0.5f, 0.5f, UVmaxVal, UVminVal);
                        BuildLeftFace(i, 3 + randomStructureHeight + height, j + 1, verticies, triangles, uvs, 0.5f, 0.5f, UVmaxVal, UVminVal);
                    }
                    // Backwards
                    if (!voxels.ContainsKey(new Vector3Int(i, 3 + randomStructureHeight + height, j - 1)))
                    {
                        voxels.Add(new Vector3Int(i, 3 + randomStructureHeight + height, j - 1), (UVminVal, UVmaxVal));

                        BuildAboveFace(i, 3 + randomStructureHeight + height, j - 1, verticies, triangles, uvs, 0.5f, 0.5f, UVmaxVal, UVminVal);
                        BuildBelowFace(i, 3 + randomStructureHeight + height, j - 1, verticies, triangles, uvs, 0.5f, 0.5f, UVmaxVal, UVminVal);
                        BuildBackwardFace(i, 3 + randomStructureHeight + height, j - 1, verticies, triangles, uvs, 0.5f, 0.5f, UVminVal, UVmaxVal);
                        BuildLeftFace(i, 3 + randomStructureHeight + height, j - 1, verticies, triangles, uvs, 0.5f, 0.5f, UVmaxVal, UVminVal);
                        BuildRightFace(i, 3 + randomStructureHeight + height, j - 1, verticies, triangles, uvs, 0.5f, 0.5f, UVmaxVal, UVminVal);
                    }
                }
                else if (noise.GetNoise(i + chunkPos.x, height, j + chunkPos.z) > minUndergroundVal)
                {
                    // Grass
                    UVminVal = 0;
                    UVmaxVal = 0.19f;
                    voxels.Add(new Vector3Int(i, height, j), (UVminVal, UVmaxVal));
                    BuildAboveFace(i, height, j, verticies, triangles, uvs, 0.5f, 0.5f, UVmaxVal, UVminVal);
                }
                for (int k = height; k > 0; k--)
                {
                    float noiseVal = noise.GetNoise(i + chunkPos.x, k, j + chunkPos.z);
                    if (noiseVal > minUndergroundVal)
                    {
                        if (k == height)
                        {
                            // Grass
                            UVminVal = 0;
                            UVmaxVal = 0.19f;
                        }
                        else if (k > height - random.Next(2, 6))
                        {
                            // Dirt
                            UVminVal = 0.43f;
                            UVmaxVal = 0.58f;
                        }
                        else
                        {
                            // Stone
                            UVminVal = 0.23f;
                            UVmaxVal = 0.38f;
                        }
                        voxelsTotal.Add(new Vector3Int(i, k, j), (UVminVal, UVmaxVal));

                        if (noise.GetNoise(i + chunkPos.x, k + 1, j + chunkPos.z) < minUndergroundVal && k != height)
                        {
                            // Stone
                            UVminVal = 0.23f;
                            UVmaxVal = 0.38f;
                            if (!voxels.ContainsKey(new Vector3Int(i, k, j)))
                            {
                                voxels.Add(new Vector3Int(i, k, j), (UVminVal, UVmaxVal));
                            }
                            BuildAboveFace(i, k, j, verticies, triangles, uvs, 0.5f, 0.5f, UVmaxVal, UVminVal);
                        }

                        if (noise.GetNoise(i + chunkPos.x, k - 1, j + chunkPos.z) < minUndergroundVal)
                        {
                            if (!voxels.ContainsKey(new Vector3Int(i, k, j)))
                            {
                                voxels.Add(new Vector3Int(i, k, j), (UVminVal, UVmaxVal));
                            }
                            BuildBelowFace(i, k, j, verticies, triangles, uvs, 0.5f, 0.5f, UVmaxVal, UVminVal);
                        }
                        if (noise.GetNoise(i + chunkPos.x, k, j + chunkPos.z + 1) < minUndergroundVal || k > forwardHeight)
                        {
                            if (!voxels.ContainsKey(new Vector3Int(i, k, j)))
                            {
                                voxels.Add(new Vector3Int(i, k, j), (UVminVal, UVmaxVal));
                            }
                            BuildForwardFace(i, k, j, verticies, triangles, uvs, 0.5f, 0.5f, UVmaxVal, UVminVal);
                        }
                        if (noise.GetNoise(i + chunkPos.x, k, j + chunkPos.z - 1) < minUndergroundVal || k > backwardHeight)
                        {
                            if (!voxels.ContainsKey(new Vector3Int(i, k, j)))
                            {
                                voxels.Add(new Vector3Int(i, k, j), (UVminVal, UVmaxVal));
                            }
                            BuildBackwardFace(i, k, j, verticies, triangles, uvs, 0.5f, 0.5f, UVminVal, UVmaxVal);
                        }
                        if (noise.GetNoise(i + chunkPos.x + 1, k, j + chunkPos.z) < minUndergroundVal || k > rightHeight)
                        {
                            if (!voxels.ContainsKey(new Vector3Int(i, k, j)))
                            {
                                voxels.Add(new Vector3Int(i, k, j), (UVminVal, UVmaxVal));
                            }
                            BuildRightFace(i, k, j, verticies, triangles, uvs, 0.5f, 0.5f, UVmaxVal, UVminVal);
                        }
                        if (noise.GetNoise(i + chunkPos.x - 1, k, j + chunkPos.z) < minUndergroundVal || k > leftHeight)
                        {
                            if (!voxels.ContainsKey(new Vector3Int(i, k, j)))
                            {
                                voxels.Add(new Vector3Int(i, k, j), (UVminVal, UVmaxVal));
                            }
                            BuildLeftFace(i, k, j, verticies, triangles, uvs, 0.5f, 0.5f, UVmaxVal, UVminVal);
                        }
                    }
                }
            }
        }

        mr.material = mat;

        mf.mesh.vertices = verticies.ToArray();
        mf.mesh.triangles = triangles.ToArray();
        mf.mesh.uv = uvs.ToArray();

        mf.mesh.RecalculateBounds();
        mf.mesh.RecalculateNormals();
        mf.mesh.Optimize();

        collider.sharedMesh = mf.mesh;
    }

    public void removeVoxel(Vector3Int voxelToRemvove)
    {
        voxels.Remove(voxelToRemvove);
        voxelsTotal.Remove(voxelToRemvove);
        if (voxelsTotal.ContainsKey(voxelToRemvove + Vector3Int.down) && !voxels.ContainsKey(voxelToRemvove + Vector3Int.down))
        {
            voxels.Add(voxelToRemvove + Vector3Int.down, (voxelsTotal[voxelToRemvove + Vector3Int.down].Item1, voxelsTotal[voxelToRemvove + Vector3Int.down].Item2));
        }

        if (voxelsTotal.ContainsKey(voxelToRemvove + Vector3Int.up) && !voxels.ContainsKey(voxelToRemvove + Vector3Int.up))
        {
            voxels.Add(voxelToRemvove + Vector3Int.up, (voxelsTotal[voxelToRemvove + Vector3Int.up].Item1, voxelsTotal[voxelToRemvove + Vector3Int.up].Item2));
        }

        if (voxelsTotal.ContainsKey(voxelToRemvove + Vector3Int.right) && !voxels.ContainsKey(voxelToRemvove + Vector3Int.right))
        {
            voxels.Add(voxelToRemvove + Vector3Int.right, (voxelsTotal[voxelToRemvove + Vector3Int.right].Item1, voxelsTotal[voxelToRemvove + Vector3Int.right].Item2));
        }

        if (voxelsTotal.ContainsKey(voxelToRemvove + Vector3Int.left) && !voxels.ContainsKey(voxelToRemvove + Vector3Int.left))
        {
            voxels.Add(voxelToRemvove + Vector3Int.left, (voxelsTotal[voxelToRemvove + Vector3Int.left].Item1, voxelsTotal[voxelToRemvove + Vector3Int.left].Item2));
        }

        if (voxelsTotal.ContainsKey(voxelToRemvove + new Vector3Int(0, 0, 1)) && !voxels.ContainsKey(voxelToRemvove + new Vector3Int(0, 0, 1)))
        {
            voxels.Add(voxelToRemvove + new Vector3Int(0, 0, 1), (voxelsTotal[voxelToRemvove + new Vector3Int(0, 0, 1)].Item1, voxelsTotal[voxelToRemvove + new Vector3Int(0, 0, 1)].Item2));
        }

        if (voxelsTotal.ContainsKey(voxelToRemvove + new Vector3Int(0, 0, -1)) && !voxels.ContainsKey(voxelToRemvove + new Vector3Int(0, 0, -1)))
        {
            voxels.Add(voxelToRemvove + new Vector3Int(0, 0, -1), (voxelsTotal[voxelToRemvove + new Vector3Int(0, 0, -1)].Item1, voxelsTotal[voxelToRemvove + new Vector3Int(0, 0, -1)].Item2));
        }

        Rerender();

        Chunk nextChunk;
        // Rerendering adjacent chunks:
        if (voxelToRemvove.x == chunkSize - 1)
        {
            nextChunk = gameObject.transform.parent.Find((chunkPos + Vector3Int.right * chunkSize).ToString()).GetComponent<Chunk>();
            if (nextChunk.voxelsTotal.ContainsKey(new Vector3Int(0, voxelToRemvove.y, voxelToRemvove.z)) && !nextChunk.voxels.ContainsKey(new Vector3Int(0, voxelToRemvove.y, voxelToRemvove.z)))
            {
                nextChunk.voxels.Add(new Vector3Int(0, voxelToRemvove.y, voxelToRemvove.z), nextChunk.voxelsTotal[new Vector3Int(0, voxelToRemvove.y, voxelToRemvove.z)]);
            }
            nextChunk.Rerender();
        }
        else if (voxelToRemvove.x == 0)
        {
            nextChunk = gameObject.transform.parent.Find((chunkPos + Vector3Int.left * chunkSize).ToString()).GetComponent<Chunk>();
            if (nextChunk.voxelsTotal.ContainsKey(new Vector3Int(chunkSize - 1, voxelToRemvove.y, voxelToRemvove.z)) && !nextChunk.voxels.ContainsKey(new Vector3Int(chunkSize - 1, voxelToRemvove.y, voxelToRemvove.z)))
            {
                nextChunk.voxels.Add(new Vector3Int(chunkSize - 1, voxelToRemvove.y, voxelToRemvove.z), nextChunk.voxelsTotal[new Vector3Int(chunkSize - 1, voxelToRemvove.y, voxelToRemvove.z)]);
            }
            nextChunk.Rerender();
        }
        if (voxelToRemvove.z == chunkSize - 1)
        {
            nextChunk = gameObject.transform.parent.Find((chunkPos + new Vector3Int(0, 0, 1) * chunkSize).ToString()).GetComponent<Chunk>();
            if (nextChunk.voxelsTotal.ContainsKey(new Vector3Int(voxelToRemvove.x, voxelToRemvove.y, 0)) && !nextChunk.voxels.ContainsKey(new Vector3Int(voxelToRemvove.x, voxelToRemvove.y, 0)))
            {
                nextChunk.voxels.Add(new Vector3Int(voxelToRemvove.x, voxelToRemvove.y, 0), nextChunk.voxelsTotal[new Vector3Int(voxelToRemvove.x, voxelToRemvove.y, 0)]);
            }
            nextChunk.Rerender();
        }
        else if (voxelToRemvove.z == 0)
        {
            nextChunk = gameObject.transform.parent.Find((chunkPos + new Vector3Int(0, 0, -1) * chunkSize).ToString()).GetComponent<Chunk>();
            if (nextChunk.voxelsTotal.ContainsKey(new Vector3Int(voxelToRemvove.x, voxelToRemvove.y, chunkSize - 1)) && !nextChunk.voxels.ContainsKey(new Vector3Int(voxelToRemvove.x, voxelToRemvove.y, chunkSize - 1)))
            {
                nextChunk.voxels.Add(new Vector3Int(voxelToRemvove.x, voxelToRemvove.y, chunkSize - 1), nextChunk.voxelsTotal[new Vector3Int(voxelToRemvove.x, voxelToRemvove.y, chunkSize - 1)]);
            }
            nextChunk.Rerender();
        }
    }

    public void addVoxel(Vector3Int voxelToAdd)
    {
        // Dirt
        if (!voxels.ContainsKey(voxelToAdd))
        {
            voxels.Add(voxelToAdd, (0.43f, 0.58f));
        }
        if (!voxelsTotal.ContainsKey(voxelToAdd))
        {
            voxelsTotal.Add(voxelToAdd, (0.43f, 0.58f));
        }
        Rerender();

        Chunk nextChunk;
        // Rerendering adjacent chunks:
        if (voxelToAdd.x == chunkSize - 1)
        {
            nextChunk = gameObject.transform.parent.Find((chunkPos + Vector3Int.right * chunkSize).ToString()).GetComponent<Chunk>();
            nextChunk.Rerender();
        }
        else if (voxelToAdd.x == 0)
        {
            nextChunk = gameObject.transform.parent.Find((chunkPos + Vector3Int.left * chunkSize).ToString()).GetComponent<Chunk>();
            nextChunk.Rerender();
        }
        if (voxelToAdd.z == chunkSize - 1)
        {
            nextChunk = gameObject.transform.parent.Find((chunkPos + new Vector3Int(0, 0, 1) * chunkSize).ToString()).GetComponent<Chunk>();
            nextChunk.Rerender();
        }
        else if (voxelToAdd.z == 0)
        {
            nextChunk = gameObject.transform.parent.Find((chunkPos + new Vector3Int(0, 0, -1) * chunkSize).ToString()).GetComponent<Chunk>();
            nextChunk.Rerender();
        }
    }

    void Rerender()
    {
        MeshCollider collider = gameObject.GetComponent<MeshCollider>();
        MeshFilter mf = gameObject.GetComponent<MeshFilter>();

        mf.mesh = new Mesh();
        collider.sharedMesh = new Mesh();

        List<Vector3> verticies = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        Vector3Int[] voxelLocations = voxels.Keys.ToArray();
        for (int i = 0; i < voxelLocations.Length; i++)
        {
            Chunk nextChunk;
            Vector3Int currentVoxel = voxelLocations[i];



            if (!voxelLocations.Contains(currentVoxel + Vector3Int.up) && !voxelsTotal.ContainsKey(currentVoxel + Vector3Int.up))
            {
                BuildAboveFace(currentVoxel.x, currentVoxel.y, currentVoxel.z, verticies, triangles, uvs, 0.5f, 0.5f, voxels[currentVoxel].Item2, voxels[currentVoxel].Item1);
            }
            if (!voxelLocations.Contains(currentVoxel + Vector3Int.down) && !voxelsTotal.ContainsKey(currentVoxel + Vector3Int.down))
            {
                BuildBelowFace(currentVoxel.x, currentVoxel.y, currentVoxel.z, verticies, triangles, uvs, 0.5f, 0.5f, voxels[currentVoxel].Item2, voxels[currentVoxel].Item1);
            }


            if (!voxelLocations.Contains(currentVoxel + new Vector3Int(0, 0, 1)) && !voxelsTotal.ContainsKey(currentVoxel + new Vector3Int(0, 0, 1)))
            {
                if (currentVoxel.z == chunkSize - 1)
                {
                    nextChunk = gameObject.transform.parent.Find((chunkPos + new Vector3Int(0, 0, 1) * chunkSize).ToString()).GetComponent<Chunk>();
                    if (!nextChunk.voxels.ContainsKey(new Vector3Int(currentVoxel.x, currentVoxel.y, 0)) && !nextChunk.voxelsTotal.ContainsKey(new Vector3Int(currentVoxel.x, currentVoxel.y, 0)))
                    {
                        BuildForwardFace(currentVoxel.x, currentVoxel.y, currentVoxel.z, verticies, triangles, uvs, 0.5f, 0.5f, voxels[currentVoxel].Item2, voxels[currentVoxel].Item1);
                    }
                }
                else
                {
                    BuildForwardFace(currentVoxel.x, currentVoxel.y, currentVoxel.z, verticies, triangles, uvs, 0.5f, 0.5f, voxels[currentVoxel].Item2, voxels[currentVoxel].Item1);
                }
            }


            if (!voxelLocations.Contains(currentVoxel + new Vector3Int(0, 0, -1)) && !voxelsTotal.ContainsKey(currentVoxel + new Vector3Int(0, 0, -1)))
            {
                if (currentVoxel.z == 0)
                {
                    nextChunk = gameObject.transform.parent.Find((chunkPos + new Vector3Int(0, 0, -1) * chunkSize).ToString()).GetComponent<Chunk>();
                    if (!nextChunk.voxels.ContainsKey(new Vector3Int(currentVoxel.x, currentVoxel.y, chunkSize - 1)) && !nextChunk.voxelsTotal.ContainsKey(new Vector3Int(currentVoxel.x, currentVoxel.y, chunkSize - 1)))
                    {
                        BuildBackwardFace(currentVoxel.x, currentVoxel.y, currentVoxel.z, verticies, triangles, uvs, 0.5f, 0.5f, voxels[currentVoxel].Item1, voxels[currentVoxel].Item2);
                    }
                }
                else
                {
                    BuildBackwardFace(currentVoxel.x, currentVoxel.y, currentVoxel.z, verticies, triangles, uvs, 0.5f, 0.5f, voxels[currentVoxel].Item1, voxels[currentVoxel].Item2);
                }
            }


            if (!voxelLocations.Contains(currentVoxel + Vector3Int.right) && !voxelsTotal.ContainsKey(currentVoxel + Vector3Int.right))
            {
                if (currentVoxel.x == chunkSize - 1)
                {
                    nextChunk = gameObject.transform.parent.Find((chunkPos + Vector3Int.right * chunkSize).ToString()).GetComponent<Chunk>();
                    if (!nextChunk.voxels.ContainsKey(new Vector3Int(0, currentVoxel.y, currentVoxel.z)) && !nextChunk.voxelsTotal.ContainsKey(new Vector3Int(0, currentVoxel.y, currentVoxel.z)))
                    {
                        BuildRightFace(currentVoxel.x, currentVoxel.y, currentVoxel.z, verticies, triangles, uvs, 0.5f, 0.5f, voxels[currentVoxel].Item2, voxels[currentVoxel].Item1);
                    }
                }
                else
                {
                    BuildRightFace(currentVoxel.x, currentVoxel.y, currentVoxel.z, verticies, triangles, uvs, 0.5f, 0.5f, voxels[currentVoxel].Item2, voxels[currentVoxel].Item1);
                }
            }


            if (!voxelLocations.Contains(currentVoxel + Vector3Int.left) && !voxelsTotal.ContainsKey(currentVoxel + Vector3Int.left))
            {
                if (currentVoxel.x == 0)
                {
                    nextChunk = gameObject.transform.parent.Find((chunkPos + Vector3Int.left * chunkSize).ToString()).GetComponent<Chunk>();
                    if (!nextChunk.voxels.ContainsKey(new Vector3Int(chunkSize - 1, currentVoxel.y, currentVoxel.z)) && !nextChunk.voxelsTotal.ContainsKey(new Vector3Int(chunkSize - 1, currentVoxel.y, currentVoxel.z)))
                    {
                        BuildLeftFace(currentVoxel.x, currentVoxel.y, currentVoxel.z, verticies, triangles, uvs, 0.5f, 0.5f, voxels[currentVoxel].Item2, voxels[currentVoxel].Item1);
                    }
                }
                else
                {
                    BuildLeftFace(currentVoxel.x, currentVoxel.y, currentVoxel.z, verticies, triangles, uvs, 0.5f, 0.5f, voxels[currentVoxel].Item2, voxels[currentVoxel].Item1);
                }
            }
        }

        mf.mesh.vertices = verticies.ToArray();
        mf.mesh.triangles = triangles.ToArray();
        mf.mesh.uv = uvs.ToArray();

        mf.mesh.RecalculateBounds();
        mf.mesh.RecalculateNormals();
        mf.mesh.Optimize();

        collider.sharedMesh = mf.mesh;
    }

    public void Establish(int chunkSize, int chunkHeight, float amplitude, Material mat, FastNoise noise, Vector3Int chunkPos, float originalFrequency, int originalOctaves)
    {
        SetParameters(chunkSize, chunkHeight, amplitude, mat, noise, chunkPos, originalFrequency, originalOctaves);
        GenerateVoxel();
    }

    void GenerationTest()
    {
        MeshFilter mf = gameObject.GetComponent<MeshFilter>();
        MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();

        List<Vector3> verticies = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        for (int i = 0; i < chunkSize; i++)
        {
            for (int j = 0; j < chunkSize; j++)
            {
                float height = noise.GetNoise(i + chunkPos.x, j + chunkPos.z) * amplitude;

                Vector3 newVertex = new Vector3(i, (int)Math.Ceiling(height), j);
                verticies.Add(newVertex);
                uvs.Add(new Vector2((float)i / chunkSize, (float)j / chunkSize));
            }
        }
        List<int> triangles = new List<int>();
        for (int k = 0; k < verticies.Count; k++)
        {
            if (k < chunkSize * chunkSize - chunkSize && ((int)((k + 1) / chunkSize) - ((k + chunkSize) / chunkSize)) != 0)
            {
                triangles.Add(k + 1);
                triangles.Add(k + chunkSize);
                triangles.Add(k);

                triangles.Add(k + 1);
                triangles.Add(k + chunkSize + 1);
                triangles.Add(k + chunkSize);
            }
        }

        mr.material = mat;

        mf.mesh.vertices = verticies.ToArray();
        mf.mesh.triangles = triangles.ToArray();
        mf.mesh.uv = uvs.ToArray();

        mf.mesh.RecalculateBounds();
        mf.mesh.RecalculateNormals();
        mf.mesh.Optimize();
    }
}
