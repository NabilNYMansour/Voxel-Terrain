using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManipulateTerrain : MonoBehaviour
{
    public Transform Highlight;
    Transform currentHighlight;

    void GetCurrentPosChunkScript()
    {
        Camera eyes = gameObject.transform.Find("Eyes").GetComponent<Camera>();
        Ray ray = new Ray(eyes.transform.position, eyes.transform.rotation * Vector3.forward);
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
        RaycastHit hit;
#pragma warning disable CS0618 // Type or member is obsolete
        if (currentHighlight.gameObject.active)
#pragma warning restore CS0618 // Type or member is obsolete
        {
#pragma warning disable CS0618 // Type or member is obsolete
            currentHighlight.gameObject.active = false;
#pragma warning restore CS0618 // Type or member is obsolete
        }
        if (Physics.Raycast(ray, out hit, 3f))
        {
            Vector3Int hitPoint = Vector3Int.zero;

            if (hit.normal == Vector3.up)
            {
                hitPoint = new Vector3Int(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.y - 0.5f), Mathf.RoundToInt(hit.point.z));
            }
            else if (hit.normal == Vector3.down)
            {
                hitPoint = new Vector3Int(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.y + 0.5f), Mathf.RoundToInt(hit.point.z));
            }
            else if (hit.normal == Vector3.forward)
            {
                hitPoint = new Vector3Int(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.y), Mathf.RoundToInt(hit.point.z - 0.5f));
            }
            else if (hit.normal == Vector3.back)
            {
                hitPoint = new Vector3Int(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.y), Mathf.RoundToInt(hit.point.z + 0.5f));
            }
            else if (hit.normal == Vector3.right)
            {
                hitPoint = new Vector3Int(Mathf.RoundToInt(hit.point.x - 0.5f), Mathf.RoundToInt(hit.point.y), Mathf.RoundToInt(hit.point.z));
            }
            else if (hit.normal == Vector3.left)
            {
                hitPoint = new Vector3Int(Mathf.RoundToInt(hit.point.x + 0.5f), Mathf.RoundToInt(hit.point.y), Mathf.RoundToInt(hit.point.z));
            }

            // Outline:
            currentHighlight.transform.position = hitPoint;
#pragma warning disable CS0618 // Type or member is obsolete
            if (!currentHighlight.gameObject.active)
#pragma warning restore CS0618 // Type or member is obsolete
            {
#pragma warning disable CS0618 // Type or member is obsolete
                currentHighlight.gameObject.active = true;
#pragma warning restore CS0618 // Type or member is obsolete
            }

            if (Input.GetMouseButtonDown(1))
            {
                Vector3Int chunkPos = hit.collider.gameObject.GetComponent<Chunk>().chunkPos;
                hit.collider.gameObject.GetComponent<Chunk>().removeVoxel(hitPoint - chunkPos);
            }

            else if (Input.GetMouseButtonDown(0))
            {
                if (hit.normal == Vector3.up)
                {
                    hitPoint = new Vector3Int(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.y + 0.5f), Mathf.RoundToInt(hit.point.z));
                }
                else if (hit.normal == Vector3.down)
                {
                    hitPoint = new Vector3Int(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.y - 0.5f), Mathf.RoundToInt(hit.point.z));
                }
                else if (hit.normal == Vector3.forward)
                {
                    hitPoint = new Vector3Int(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.y), Mathf.RoundToInt(hit.point.z + 0.5f));
                }
                else if (hit.normal == Vector3.back)
                {
                    hitPoint = new Vector3Int(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.y), Mathf.RoundToInt(hit.point.z - 0.5f));
                }
                else if (hit.normal == Vector3.right)
                {
                    hitPoint = new Vector3Int(Mathf.RoundToInt(hit.point.x + 0.5f), Mathf.RoundToInt(hit.point.y), Mathf.RoundToInt(hit.point.z));
                }
                else if (hit.normal == Vector3.left)
                {
                    hitPoint = new Vector3Int(Mathf.RoundToInt(hit.point.x - 0.5f), Mathf.RoundToInt(hit.point.y), Mathf.RoundToInt(hit.point.z));
                }

                Vector3Int chunkPos = hit.collider.gameObject.GetComponent<Chunk>().chunkPos;
                hit.collider.gameObject.GetComponent<Chunk>().addVoxel(hitPoint - chunkPos);
            }
        }
    }

    void Start()
    {
        currentHighlight = Instantiate(Highlight, Vector3.zero, Quaternion.identity, gameObject.transform.parent.transform);
#pragma warning disable CS0618 // Type or member is obsolete
        currentHighlight.gameObject.active = false;
#pragma warning restore CS0618 // Type or member is obsolete
    }

    // Update is called once per frame
    void Update()
    {
        GetCurrentPosChunkScript();
    }
}
