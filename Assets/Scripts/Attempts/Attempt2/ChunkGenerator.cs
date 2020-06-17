using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    public Transform chunkPrefab;
    public int chunkSize;
    public int chunkWidth;
    public float frequency;
    //public float amplitude;
    public int seed;
    FastNoise noise;
    Vector3 playerPos;
    Vector3Int currentPos;

    void SetUpNoiseGenerator(int octaves, float lacunarity)
    {
        float noiseFrequency = (float)(1 / frequency);
        noise = new FastNoise();
        noise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        noise.SetInterp(FastNoise.Interp.Quintic);
        noise.SetFractalType(FastNoise.FractalType.FBM);
        noise.SetSeed(seed);
        noise.SetFrequency(noiseFrequency);
        noise.SetFractalOctaves(octaves);
        noise.SetFractalLacunarity(lacunarity);
    }

    void GenerateChunk(Vector3Int newChunkPos)
    {
        Transform newChunk = Instantiate(chunkPrefab, newChunkPos, Quaternion.identity, gameObject.transform);
        OlderChunk currentChunk = newChunk.GetComponent<OlderChunk>();
        currentChunk.SetChunk(newChunkPos, chunkSize, chunkWidth, noise);
        currentChunk.SetNoiseMap();
        currentChunk.BuildChunk();
    }

    //void GenerateAroundPlayer()
    //{
    //    if 
    //}

    IEnumerator GenerationTest()
    {
        int numberOfChunks = 8;
        for (int i = -numberOfChunks / 2; i < numberOfChunks / 2; i++)
        {
            for (int j = -numberOfChunks / 2; j < numberOfChunks / 2; j++)
            {
                GenerateChunk(new Vector3Int(i, 0, j) * chunkSize);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        SetUpNoiseGenerator(8, 2);

        //StartCoroutine("GenerationTest");
        GenerateChunk(new Vector3Int(1, 0, 1) * chunkSize);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
