using SimplexNoise;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public static Generator singleton;

    [SerializeField] Vector3Int worldSize = new Vector3Int(8,8,8);
    [SerializeField] float noiseScale = .2f;

    [SerializeField] GameObject chunkModel;

    public Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();

    void Start()
    {
        StartCoroutine(Generate());
    }

    IEnumerator Generate()
    {
        Chunk chunk;
        for (int k = 0; k < worldSize.z; ++k)
        {
            for (int j = 0; j < worldSize.y; ++j)
            {
                for (int i = 0; i < worldSize.x; ++i)
                {
                    chunk = Instantiate(chunkModel, new Vector3(i * Chunk.size.x, j * Chunk.size.y, k * Chunk.size.z), Quaternion.identity, transform).GetComponent<Chunk>();
                    chunk.SetUp(this, i, j, k);
                    chunks.Add(new Vector3Int(i, j, k), chunk);
                    FillChunk(i, j, k);
                }
            }
        }

        for (int k = 0; k < worldSize.z; ++k)
        {
            for (int j = 0; j < worldSize.y; ++j)
            {
                for (int i = 0; i < worldSize.x; ++i)
                {
                    chunks[new Vector3Int(i, j, k)].GenerateMesh();
                    yield return new WaitForEndOfFrame();
                }
            }
        }
    }

    public Voxel GetData(Vector3Int chunkPos, int x, int y, int z, int adjPos)
    {
        if (!chunks.ContainsKey(chunkPos))
            return null;

        switch (adjPos)
        {
            case 0:
                return chunks[chunkPos][0, y, z];
            case 1:
                return chunks[chunkPos][x, 0, z];
            case 2:
                return chunks[chunkPos][x, y, 0];
            case 3:
                return chunks[chunkPos][x, y, Chunk.size.z - 1];
            case 4:
                return chunks[chunkPos][x, Chunk.size.y - 1, z];
            case 5:
                return chunks[chunkPos][Chunk.size.x - 1, y, z];
            default:
                return null;
        }
    }
  

    
    void FillChunk(int x, int y, int z)
    {
        float value;
        for (int k = 0; k < Chunk.size.x; ++k)
        {
            for (int j = 0; j < Chunk.size.y; ++j)
            {
                for (int i = 0; i < Chunk.size.z; ++i)
                {
                    value = Noise.Generate((x * Chunk.size.x + i) * noiseScale, (y * Chunk.size.y + j) * noiseScale, (z * Chunk.size.z + k) * noiseScale);
                    chunks[new Vector3Int(x, y, z)][i, j, k] = new Voxel(0.5f-value);
                }
            }
        }
    }
}