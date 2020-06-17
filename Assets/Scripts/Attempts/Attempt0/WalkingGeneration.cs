using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingGeneration : MonoBehaviour
{
    public Transform dot;
    public int chunkSize;
    public float frequency;
    public float amplitude;
    public int seed;
    public float disableDistance;
    public Material red;
    Vector3 playerPos;
    Camera playerEyes;
    List<Vector2> map = new List<Vector2>();

    void GenerateTerrian(Vector3 center)
    {
        for (int i = (int)center.x - chunkSize / 2; i < (int)center.x + chunkSize / 2; i++)
        {
            for (int j = (int)center.z - chunkSize / 2; j < (int)center.z + chunkSize / 2; j++)
            {
                if (!map.Contains(new Vector3(i, j)))
                {
                    // !!!!!Make foggy noise:
                    float height = Mathf.PerlinNoise(seed + (i / frequency), seed + (j / frequency)) * amplitude;

                    Instantiate(dot, new Vector3(i, height, j), Quaternion.identity, gameObject.transform);
                    map.Add(new Vector2(i, j));
                }
            }
        }
    }

    void DisableRendering()
    {
        foreach (Transform Block in gameObject.transform)
        {
            if (Block.tag != "Player")
            {
                if (Vector3.Distance(playerPos, Block.position) > disableDistance)
                {
                    Block.gameObject.SetActive(false);
                }
                else
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    if (!Block.gameObject.active)
#pragma warning restore CS0618 // Type or member is obsolete
                    {
                        Block.gameObject.SetActive(true);
                    }
                }
                MeshRenderer br = Block.GetComponent<MeshRenderer>();
                br.enabled = false;
            }
        }
    }

    void EnableRendering()
    {
        DisableRendering();
        for (float i = -1.5f; i <= 1.5f; i += 0.02f)
        {
            for (float j = -1.5f; j <= 1.5f; j += 0.02f)
            {
                Vector3 vector = playerEyes.transform.rotation * new Vector3(i, j, 1);
                Ray ray = new Ray(playerPos, vector);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    hit.collider.gameObject.GetComponent<MeshRenderer>().enabled = true;
                }
                Debug.DrawLine(playerPos, ray.GetPoint(hit.distance), Color.red);
            }
        }
    }

    void Start()
    {
        playerPos = gameObject.transform.Find("Player").transform.position;
        playerEyes = gameObject.transform.Find("Player").transform.Find("Eyes").GetComponent<Camera>();
        playerPos = new Vector3((int)playerPos.x, (int)playerPos.y, (int)playerPos.z);
        chunkSize *= 2;
        GenerateTerrian(playerPos);
        chunkSize /= 2;
    }

    void Update()
    {
        playerPos = gameObject.transform.Find("Player").transform.position;
        GenerateTerrian(playerPos);
        EnableRendering();
    }
}
