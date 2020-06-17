using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public Transform chunkPrefab;
    public int chunkSize;
    public int chunkHeight;
    public float frequency;
    public float amplitude;
    public int seed;
    public int octaves;
    public float lacunarity;
    public int chunksToRender;
    public int renderDistance;
    public Material mat;

    FastNoise noise = new FastNoise();

    List<Vector3Int> generatedChunks = new List<Vector3Int>();
    Vector3Int playerPos;
    Vector3Int currentChunkPos;

    void SetNoise()
    {
        noise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        noise.SetInterp(FastNoise.Interp.Quintic);
        noise.SetFractalType(FastNoise.FractalType.FBM);
        noise.SetSeed(seed);
        noise.SetFrequency(1f / frequency);
        noise.SetFractalOctaves(octaves);
        noise.SetFractalLacunarity(lacunarity);
    }

    IEnumerator GenerateAroundPlayer()
    {
        int x = 0;
        int y = 0;

        Vector3Int newChunkPos = currentChunkPos + new Vector3Int(x, 0, y) * chunkSize;
        if (!generatedChunks.Contains(newChunkPos))
        {
            Transform newChunk = Instantiate(chunkPrefab, newChunkPos, Quaternion.identity, gameObject.transform);
            newChunk.name = newChunkPos.ToString();
            newChunk.GetComponent<Chunk>().Establish(chunkSize, chunkHeight, amplitude, mat, noise, newChunkPos, frequency, octaves);
            generatedChunks.Add(newChunkPos);
            yield return new WaitForSeconds(0.1f);
        }

        float time = 0.05f;

        // Utilizing spiral algorithm:
        float angle;
        for (int j = 1; j < chunksToRender; j++)
        {
            if (j > 5)
            {
                time = 0.3f;
            }

            for (int i = 0 - j + 1; i < (8 * j); i++)
            {
                angle = (i * ((float)Math.PI / 4 / j));
                if (Math.Cos(angle) * j > 0 && Math.Abs(Math.Cos(angle) * j) * 10 > 1)
                {
                    x = (int)Math.Ceiling(Math.Cos(angle) * j);
                }
                else if (Math.Cos(angle) * j < 0 && Math.Abs(Math.Cos(angle) * j) * 10 > 1)
                {
                    x = (int)Math.Floor(Math.Cos(angle) * j);
                }
                else
                {
                    x = 0;
                }
                if (Math.Sin(angle) * j > 0 && Math.Abs(Math.Sin(angle) * j) * 10 > 1)
                {
                    y = (int)Math.Ceiling(Math.Sin(angle) * j);
                }
                else if (Math.Sin(angle) * j < 0 && Math.Abs(Math.Sin(angle) * j) * 10 > 1)
                {
                    y = (int)Math.Floor(Math.Sin(angle) * j);
                }
                else
                {
                    y = 0;
                }

                newChunkPos = currentChunkPos + new Vector3Int(x, 0, y) * chunkSize;
                if (!generatedChunks.Contains(newChunkPos))
                {
                    Transform newChunk = Instantiate(chunkPrefab, newChunkPos, Quaternion.identity, gameObject.transform);
                    newChunk.name = newChunkPos.ToString();
                    newChunk.GetComponent<Chunk>().Establish(chunkSize, chunkHeight, amplitude, mat, noise, newChunkPos, frequency, octaves);
                    generatedChunks.Add(newChunkPos);
                    yield return new WaitForSeconds(time);
                }
            }
        }
    }

    void DegenerateAroundPlayer()
    {
        foreach (Transform child in gameObject.transform)
        {
            if (child.name != "Player" && child.name != "Highlight(Clone)")
            {
                if (Vector2Int.Distance(new Vector2Int((int)child.position.x, (int)child.position.z), new Vector2Int(playerPos.x, playerPos.z)) > renderDistance)
                {
                    Destroy(child.gameObject);
                    generatedChunks.Remove(new Vector3Int((int)child.position.x, (int)child.position.y, (int)child.position.z));
                }
            }
        }
    }

    void Rerender()
    {
        StopAllCoroutines();
        DegenerateAroundPlayer();
        StartCoroutine("GenerateAroundPlayer");
    }

    void Start()
    {
        System.Random random = new System.Random();
        seed = random.Next();
        SetNoise();
        currentChunkPos = Vector3Int.zero * chunkSize;
        StartCoroutine("GenerateAroundPlayer");
    }

    void FixedUpdate()
    {
        Vector3 player = gameObject.transform.Find("Player").transform.position;
        playerPos = new Vector3Int((int)player.x, 0, (int)player.z);
        // Checking distance between current chunk and player:
        if (Vector2Int.Distance(new Vector2Int(currentChunkPos.x, currentChunkPos.z), new Vector2Int(playerPos.x, playerPos.z)) > chunkSize * chunksToRender / 6)
        {
            Vector3 direction = (player - currentChunkPos).normalized;
            currentChunkPos += DesiredDirection(direction) * chunkSize;
            Rerender();
        }
    }

    Vector3Int DesiredDirection(Vector3 direction) // Assuming normalized
    {
        float[] distances = new float[]
        {
            // Forward: (index 0)
            Vector3.Distance(direction,Vector3.forward),
            // Backward:
            Vector3.Distance(direction,Vector3.back),
            // Right:
            Vector3.Distance(direction,Vector3.right),
            // Left:
            Vector3.Distance(direction,Vector3.left)
        };
        float min = distances[0];
        foreach (float val in distances)
        {
            if (val < min)
            {
                min = val;
            }
        }
        switch (Array.IndexOf(distances, min))
        {
            case 0:
                return new Vector3Int(0, 0, 1);
            case 1:
                return new Vector3Int(0, 0, -1);
            case 2:
                return new Vector3Int(1, 0, 0);
            case 3:
                return new Vector3Int(-1, 0, 0);
        }
        return Vector3Int.zero;
    }
}
