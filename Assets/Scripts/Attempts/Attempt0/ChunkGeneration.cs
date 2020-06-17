//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ChunkGeneration : MonoBehaviour
//{
//    public Transform chunkPrefab;
//    public int chunkSize;
//    public float frequency;
//    public float amplitude;
//    public int seed;
//    public float minHeight;
//    public float maxHeight;
//    Vector3 playerPos;
//    Vector3 currentChunckPos;
//    List<Vector3> loadedChunks = new List<Vector3>();

//    void GenerateChunk(Vector3 chunkPos)
//    {
//        if (!loadedChunks.Contains(chunkPos))
//        {
//            // Making a new chunk:
//            Transform newChunk = Instantiate(chunkPrefab, chunkPos, Quaternion.identity, gameObject.transform.parent);
//            Chunk currentChunk = newChunk.GetComponent<Chunk>();
//            List<Vector3> blockList = new List<Vector3>();
//            //for (int i = (int)chunkPos.x - chunkSize / 2; i < (int)chunkPos.x + chunkSize / 2; i++)
//            for (int i = (int)chunkPos.x; i < (int)chunkPos.x + chunkSize; i++)
//            //for (int i = 0; i < chunkSize; i++)
//            {
//                //for (int j = (int)chunkPos.z - chunkSize / 2; j < (int)chunkPos.z + chunkSize / 2; j++)
//                for (int j = (int)chunkPos.z; j < (int)chunkPos.z + chunkSize; j++)
//                //for (int j = 0; j < chunkSize; j++)
//                {
//                    float height = 0;
//                    for (int k = 1; k <= 32; k *= 2)
//                    {
//                        height += Mathf.PerlinNoise(seed + ((float)i / (frequency / k)), seed + ((float)j / (frequency / k))) * (amplitude / k);
//                    }
//                    // Currently, well only have 2d terrain:
//                    blockList.Add(new Vector3(i, height, j));
//                }
//            }
//            currentChunk.SetFields(chunkPos, chunkSize, blockList);
//            currentChunk.DebugMakeMesh();
//            loadedChunks.Add(chunkPos);
//        }
//    }

//    void Start()
//    {
//        GenerateChunk(Vector3.zero);
//        GenerateChunk(Vector3.forward * chunkSize / 2);
//    }

//    void Update()
//    {
//        //playerPos = gameObject.transform.Find("Player").transform.position;
//        //playerPos.y = 0;
//        //if (Vector3.Distance(playerPos, currentChunckPos) > chunkSize / 2)
//        //{
//        //    print(playerPos + "," + currentChunckPos);
//        //    currentChunckPos = playerPos;
//        //    currentChunckPos.y = 0;
//        //    GenerateChunk(currentChunckPos);
//        //}
//    }
//}
