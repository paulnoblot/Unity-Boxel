using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMesh : MonoBehaviour
{
    enum FACE
    {
        right = 0,
        up = 1,
        front = 2,
        back = 3,
        down = 4,
        left = 5
    }

    [SerializeField]
    FACE face = FACE.right;
    
    void Start()
    {
        Mesh mesh = new Mesh();

        mesh.vertices = Voxel.faceVertices[(int)face];
        mesh.triangles = new int[] { 0, 1, 2, 2, 1, 3 };

        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
    }

}
