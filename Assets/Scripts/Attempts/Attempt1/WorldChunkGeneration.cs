using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldChunkGeneration : MonoBehaviour
{
    public Transform chunkPrefab;
    public int chunkSize;
    public float frequency;
    public float amplitude;
    public int seed;
    public float minHeight;
    public float maxHeight;
    public int renderDistance;
    Vector3 playerPos;
    public List<Vector3> generatedChunks = new List<Vector3>();
    Vector3 currentChunkPos;

    void GenerateChunk(Vector3 chunkPos)
    {
        Transform newChunk = Instantiate(chunkPrefab, chunkPos, Quaternion.identity, gameObject.transform);
        newChunk.name = "Chunk " + chunkPos.ToString();
        OldChunk currentChunk = newChunk.GetComponent<OldChunk>();
        float[,,] blockList = new float[chunkSize, chunkSize, chunkSize];
        for (int i = (int)chunkPos.x; i < (int)(chunkPos.x) + chunkSize; i++)
        {
            for (int j = (int)chunkPos.y; j < (int)(chunkPos.y) + chunkSize; j++)
            {
                for (int k = (int)chunkPos.z; k < (int)(chunkPos.z) + chunkSize; k++)
                {
                    blockList[i - (int)chunkPos.x, j - (int)chunkPos.y, k - (int)chunkPos.z] = perlin3D(i, j, k, 8, 2, frequency, seed);
                }
            }
        }
        blockList = LimitHeight(blockList);
        currentChunk.SetFields(chunkPos, chunkSize, blockList);
        currentChunk.MakeMesh();
        generatedChunks.Add(chunkPos);
    }

    float[,,] LimitHeight(float[,,] blockList)
    {
        float min = blockList[0, 0, 0];
        float max = blockList[0, 0, 0];

        for (int i = 0; i < chunkSize; i++)
        {
            for (int j = 0; j < chunkSize; j++)
            {
                for (int k = 0; k < chunkSize; k++)
                {
                    if (blockList[i, j, k] > max)
                    {
                        max = blockList[i, j, k];
                    }
                    if (blockList[i, j, k] < min)
                    {
                        min = blockList[i, j, k];
                    }
                }
            }
        }

        for (int i = 0; i < chunkSize; i++)
        {
            for (int j = 0; j < chunkSize; j++)
            {
                for (int k = 0; k < chunkSize; k++)
                {
                    blockList[i, j, k] = Mathf.InverseLerp(min, max, blockList[i, j, k]);
                }
            }
        }
        return blockList;
    }
    void GenerateSurfaceChunk(Vector3 chunkPos)
    {
        Transform newChunk = Instantiate(chunkPrefab, chunkPos, Quaternion.identity, gameObject.transform);
        newChunk.name = "Chunk " + chunkPos.ToString();
        OldChunk currentChunk = newChunk.GetComponent<OldChunk>();
        float[,,] blockList = new float[chunkSize, chunkSize, chunkSize];
        for (int i = (int)chunkPos.x; i < (int)(chunkPos.x) + chunkSize; i++)
        {
            for (int j = (int)chunkPos.y; j < (int)(chunkPos.y) + chunkSize; j++)
            {
                for (int k = (int)chunkPos.z; k < (int)(chunkPos.z) + chunkSize; k++)
                {
                    float height = 0;
                    for (int l = 1; l <= 32; l *= 2)
                    {
                        height += Mathf.PerlinNoise(seed + ((float)i / (frequency / l)), seed + ((float)k / (frequency / l))) * (amplitude / l);
                    }
                    if (Math.Ceiling(height) == j)
                    {
                        blockList[i - (int)chunkPos.x, j - (int)chunkPos.y, k - (int)chunkPos.z] = 1;
                    }
                    else
                    {
                        blockList[i - (int)chunkPos.x, j - (int)chunkPos.y, k - (int)chunkPos.z] = 0;
                    }
                }
            }
        }
        currentChunk.SetFields(chunkPos, chunkSize, blockList);
        currentChunk.MakeMesh();
        generatedChunks.Add(chunkPos);
    }

    public static float perlin3D(float x, float y, float z, int octaves, float lacunarity, float frequency, int seed)
    {
        float noiseFrequency = (float) (1 / frequency);
        FastNoise noise = new FastNoise();
        noise.SetSeed(seed);
        noise.SetFrequency(noiseFrequency);
        noise.SetNoiseType(FastNoise.NoiseType.PerlinFractal);
        noise.SetInterp(FastNoise.Interp.Quintic);
        noise.SetFractalOctaves(octaves);
        noise.SetFractalLacunarity(lacunarity);
        noise.SetFractalType(FastNoise.FractalType.FBM);
        return noise.GetNoise(x, y, z);
    }
    void GenerateSerfaceOnPlayer()
    {
        playerPos = gameObject.transform.Find("Player").transform.position;
        playerPos.y = 0;


        // +x
        if (playerPos.x > currentChunkPos.x)
        {
            if (!generatedChunks.Contains(currentChunkPos + Vector3.right * chunkSize))
            {
                for (int i = (int)currentChunkPos.x - renderDistance; i < renderDistance + currentChunkPos.x; i += chunkSize)
                {
                    for (int j = (int)currentChunkPos.z - renderDistance; j < renderDistance + (int)currentChunkPos.z; j += chunkSize)
                    {
                        if (!generatedChunks.Contains(new Vector3(i, 0, j)))
                        {
                            GenerateSurfaceChunk(new Vector3(i, 0, j));
                        }
                    }
                }
            }
            currentChunkPos = currentChunkPos + Vector3.right * chunkSize;
        }
        // -x
        if (playerPos.x < currentChunkPos.x)
        {
            if (!generatedChunks.Contains(currentChunkPos + Vector3.left * chunkSize))
            {
                for (int i = (int)currentChunkPos.x - renderDistance; i < renderDistance + currentChunkPos.x; i += chunkSize)
                {
                    for (int j = (int)currentChunkPos.z - renderDistance; j < renderDistance + (int)currentChunkPos.z; j += chunkSize)
                    {
                        if (!generatedChunks.Contains(new Vector3(i, 0, j)))
                        {
                            GenerateSurfaceChunk(new Vector3(i, 0, j));
                        }
                    }
                }
            }
            currentChunkPos = currentChunkPos + Vector3.left * chunkSize;
        }
        // +z
        if (playerPos.z > currentChunkPos.z)
        {
            if (!generatedChunks.Contains(currentChunkPos + Vector3.forward * chunkSize))
            {
                for (int i = (int)currentChunkPos.x - renderDistance; i < renderDistance + currentChunkPos.x; i += chunkSize)
                {
                    for (int j = (int)currentChunkPos.z - renderDistance; j < renderDistance + (int)currentChunkPos.z; j += chunkSize)
                    {
                        if (!generatedChunks.Contains(new Vector3(i, 0, j)))
                        {
                            GenerateSurfaceChunk(new Vector3(i, 0, j));
                        }
                    }
                }
            }
            currentChunkPos = currentChunkPos + Vector3.forward * chunkSize;
        }
        // -z
        if (playerPos.z < currentChunkPos.z)
        {
            if (!generatedChunks.Contains(currentChunkPos + Vector3.back * chunkSize))
            {
                for (int i = (int)currentChunkPos.x - renderDistance; i < renderDistance + currentChunkPos.x; i += chunkSize)
                {
                    for (int j = (int)currentChunkPos.z - renderDistance; j < renderDistance + (int)currentChunkPos.z; j += chunkSize)
                    {
                        if (!generatedChunks.Contains(new Vector3(i, 0, j)))
                        {
                            GenerateSurfaceChunk(new Vector3(i, 0, j));
                        }
                    }
                }
            }
            currentChunkPos = currentChunkPos + Vector3.back * chunkSize;
        }
    }

    void DeleteFarAwayChunks()
    {
        foreach (Transform child in gameObject.transform)
        {
            if (child.name != "Player")
            {
                if (Vector3.Distance(child.position, playerPos) > renderDistance)
                {
                    generatedChunks.Remove(child.GetComponent<OldChunk>().chunkPos);
                    Destroy(child.gameObject);
                }
            }
        }
    }

    IEnumerator WaitedUpdate()
    {
        while (true)
        {
            GenerateSerfaceOnPlayer();
            DeleteFarAwayChunks();
            yield return new WaitForSeconds(0.5f);
        }
    }

    void Start()
    {
        //GenerateSurfaceChunk(Vector3.zero);
        //currentChunkPos = Vector3.zero;
        //StartCoroutine("WaitedUpdate");
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                for (int k = 0; k < 5; k++)
                {
                    GenerateChunk(new Vector3(i, j, k) * chunkSize);
                }
            }
        }
        //GenerateChunk(new Vector3(0, 0, 0) * chunkSize);
    }

    void FixedUpdate()
    {

    }
}
