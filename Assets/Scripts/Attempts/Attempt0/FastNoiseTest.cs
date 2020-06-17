using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FastNoise;

public class FastNoiseTest : MonoBehaviour
{
    public int mapSize;
    public int seed;
    public float frequency;
    public int octaves;
    public float lacunarity;
    float[,] map;

    void FastNoiseToMap()
    {
        FastNoise noiseGenerator = new FastNoise();
        noiseGenerator.SetSeed(seed);
        noiseGenerator.SetFrequency(frequency);
        noiseGenerator.SetNoiseType(FastNoise.NoiseType.PerlinFractal);
        noiseGenerator.SetInterp(FastNoise.Interp.Quintic);
        noiseGenerator.SetFractalOctaves(octaves);
        noiseGenerator.SetFractalLacunarity(lacunarity);
        noiseGenerator.SetFractalType(FastNoise.FractalType.FBM);

        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                map[i, j] = noiseGenerator.GetNoise(i, j);
            }
        }
    }

    void MakeTexture()
    {
        float min = map[0, 0];
        float max = map[0, 0];

        foreach (float value in map)
        {
            if (value > max)
            {
                max = value;
            }
            if (value < min)
            {
                min = value;
            }
        }

        Texture2D texture = new Texture2D(mapSize, mapSize);
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                float height = Mathf.InverseLerp(min, max, map[i, j]);
                texture.SetPixel(i, j, new Color(height, height, height));
            }
        }
        texture.Apply();
        SaveTexture(texture, "Assets/Prefabs/Texture.jpg");
    }

    void SaveTexture(Texture2D texture, string pathWay)
    {
        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(pathWay, bytes);
    }

    // Start is called before the first frame update
    void Start()
    {
        map = new float[mapSize, mapSize];
        FastNoiseToMap();
        MakeTexture();
        print(Time.deltaTime);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
