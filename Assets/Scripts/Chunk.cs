using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Chunk : MonoBehaviour
{
    public static readonly Vector3Int size = new Vector3Int(8, 8, 8);

    public void SetUp(Generator _generator, int i, int j, int k)
    {
        generator = _generator;
        position = new Vector3Int(i, j, k);
    }



    Voxel[,,] datas = new Voxel[size.x, size.y, size.z];


    public Vector3Int position;
    Generator generator;
    Mesh mesh;
    

    public void GenerateMesh()
    {
        Mesh mesh = new Mesh();

        for (int k = 0; k < size.z; ++k)
            for (int j = 0; j < size.y; ++j)
                for (int i = 0; i < size.x; ++i)
                {
                    datas[i, j, k].GetMesh(this, i, j, k, mesh);

                }
        
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    public Voxel GetData(Vector3Int position, int adjIndex)
    {
        return GetData(position.x, position.y, position.z, adjIndex);
    }

    public Voxel GetData(int x, int y, int z, int adjIndex)
    {
        Vector3Int chunkPos = position;
        if (x < 0) chunkPos.x--;
        else if(x >= size.x) chunkPos.x++;
        if (y < 0) chunkPos.y--;
        else if (y >= size.y) chunkPos.y++;
        if (z < 0) chunkPos.z--;
        else if (z >= size.z) chunkPos.z++;
        
        if (chunkPos == position)
            return datas[x, y, z];
        
        return generator.GetData(chunkPos, x, y, z, adjIndex);
    }

    public Voxel this[int x, int y, int z]
    {
        get { return datas[x, y, z]; }
        set { datas[x, y, z] = value; }
    }

}


public class Voxel
{
    public static readonly Vector3Int[] adjacents =
    {
        new Vector3Int(1,0,0),  // right
        new Vector3Int(0,1,0),  // up
        new Vector3Int(0,0,1),  // front
        new Vector3Int(0,0,-1), // back
        new Vector3Int(0,-1,0), // down
        new Vector3Int(-1,0,0)  // left
    };

    static readonly Vector3 rightUpBack = new Vector3(.5f, .5f, .5f);
    static readonly Vector3 rightUpFront = new Vector3(.5f, .5f, -.5f);
    static readonly Vector3 rightDownBack = new Vector3(.5f, -.5f, .5f);
    static readonly Vector3 rightDownFront = new Vector3(.5f, -.5f, -.5f);
    static readonly Vector3 leftUpBack = new Vector3(-.5f, .5f, .5f);
    static readonly Vector3 leftUpFront = new Vector3(-.5f, .5f, -.5f);
    static readonly Vector3 leftDownBack = new Vector3(-.5f, -.5f, .5f);
    static readonly Vector3 leftDownFront = new Vector3(-.5f, -.5f, -.5f);

    public static readonly Dictionary<int, Vector3[]> faceVertices = new Dictionary<int, Vector3[]>
    {
        {0, new Vector3[]{ rightUpFront, rightUpBack, rightDownFront, rightDownBack} },     // right face
        {1, new Vector3[]{ leftUpBack, rightUpBack, leftUpFront, rightUpFront} },           // up face
        {2, new Vector3[]{ leftDownBack, rightDownBack, leftUpBack, rightUpBack} },         // front face
        {3, new Vector3[]{ leftUpFront, rightUpFront, leftDownFront, rightDownFront} },     // back face
        {4, new Vector3[]{ leftDownFront, rightDownFront, leftDownBack, rightDownBack} },   // down face
        {5, new Vector3[]{ leftDownFront, leftDownBack, leftUpFront, leftUpBack} }          // left face
    };

    public float value;

    public Voxel(float _v)
    {
        value = _v;
    }

    public void GetMesh(Chunk chunk, int x, int y, int z, Mesh chunkMesh)
    {
        if (value < .3f)
            return;

        List<Vector3> vertices = new List<Vector3>(chunkMesh.vertices);
        List<int> triangles = new List<int>(chunkMesh.triangles);
        List<Vector2> uvs = new List<Vector2>(chunkMesh.uv);
        Voxel data;

        for (int i = 0; i < adjacents.Length; i++)
        {
            data = chunk.GetData(adjacents[i] + new Vector3Int(x,y,z), i);
            if (data == null || data.value < .3f)
            {
                foreach (Vector3 v in faceVertices[i])
                    vertices.Add(v + new Vector3(x, y, z));

                triangles.AddRange(new int[] { vertices.Count - 4, vertices.Count - 3, vertices.Count - 2 });
                triangles.AddRange(new int[] { vertices.Count - 2, vertices.Count - 3, vertices.Count - 1 });
                uvs.AddRange(new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) });
            }
        }
        
        chunkMesh.vertices = vertices.ToArray();
        chunkMesh.triangles = triangles.ToArray();
        chunkMesh.uv = uvs.ToArray();
    }
}
