using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodHand : MonoBehaviour
{
    [SerializeField]
    Camera cam;
    [SerializeField]
    Generator world;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Create();
        if (Input.GetMouseButtonDown(1))
            Destroy();
    }

    void Create()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            Vector3Int chunkPos = Vector3Int.RoundToInt(hit.point + hit.normal / 2f);
            chunkPos.x /= Chunk.size.x;
            chunkPos.y /= Chunk.size.y;
            chunkPos.z /= Chunk.size.z;

            Vector3Int voxelPos = Vector3Int.RoundToInt(hit.point + hit.normal / 2f);
            voxelPos.x -= chunkPos.x * Chunk.size.x;
            voxelPos.y -= chunkPos.y * Chunk.size.y;
            voxelPos.z -= chunkPos.z * Chunk.size.z;

            if (world.chunks.ContainsKey(chunkPos))
            {
                world.chunks[chunkPos][voxelPos.x, voxelPos.y, voxelPos.z].value = 1f;
                world.chunks[chunkPos].GenerateMesh();
            }
        }
    }

    void Destroy()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            Vector3Int chunkPos = Vector3Int.RoundToInt(hit.point - hit.normal / 2f);
            chunkPos.x /= Chunk.size.x;
            chunkPos.y /= Chunk.size.y;
            chunkPos.z /= Chunk.size.z;

            Vector3Int voxelPos = Vector3Int.RoundToInt(hit.point - hit.normal / 2f);
            voxelPos.x -= chunkPos.x *Chunk.size.x;
            voxelPos.y -= chunkPos.y *Chunk.size.y;
            voxelPos.z -= chunkPos.z * Chunk.size.z;

            world.chunks[chunkPos][voxelPos.x, voxelPos.y, voxelPos.z].value = 0f;
            world.chunks[chunkPos].GenerateMesh();
        }
    }
}
