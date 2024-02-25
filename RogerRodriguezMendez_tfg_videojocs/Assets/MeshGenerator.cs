using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{

    [SerializeField] bool generate;

    [SerializeField] int WorldY = 1;
    Mesh mesh;

    /*Un quadrat esta format per 2 triangles
     *    O---O
     *    |\ 1|
     *    | \ |
     *    |2 \|
     *    O---O
     */
    /*Un triangle esta format per 3 vertex (sentit horari per les normals)
     *    1      1---3
     *    |\      \  |
     *    | \      \ |
     *    |  \      \|
     *    0---2      2
     *Per tant un quadrat te una estructura de 6 vertexs:
     *    0,1,2   1,3,2
     */

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        myGenerarMesh(WorldY);

    }

    void myGenerarMesh(int numbY)
    {
        int[] myTriangles = new int[ numbY * 6];
        Vector3[] myVertexs = new Vector3[(2) * (numbY + 1)];

        int i = 0;
        for (int y = 0; y <= numbY; y++)
        {
                myVertexs[i++] = new Vector3(-0.5f, y, 0);
                myVertexs[i++] = new Vector3(0.5f, y, 0);
        }

        int mesT = 0;
        int vIndex = 0;

        for (int y = 0; y < numbY; y++)
        {

            myTriangles[mesT + 0] = vIndex + 0;
            myTriangles[mesT + 1] = vIndex + 2;
            myTriangles[mesT + 2] = vIndex + 1;

            myTriangles[mesT + 3] = vIndex + 1;
            myTriangles[mesT + 4] = vIndex + 2;
            myTriangles[mesT + 5] = vIndex + 3;

            mesT += 6;
            vIndex+=2;
        }

        mesh.Clear();
        mesh.vertices = myVertexs;
        mesh.triangles = myTriangles;

        mesh.RecalculateNormals();
    }

    void myUpdateMesh(Vector3[] actualV, int[] actualT, int numbY)
    {

        int[] newTriangles = new int[ numbY * 6];
        Vector3[] newVertexs = new Vector3[2 * numbY];

        float baseY = actualV[actualV.Length - 1].y;
        int i = 0;
        for (float y = 1; y <= numbY ; y++)
        {
            newVertexs[i++] = new Vector3(-0.5f, y + baseY, 0);
            newVertexs[i++] = new Vector3(0.5f, y + baseY, 0);
        }

        int mesT = 0;
        int vIndex = (actualT.Length / 6) * 2;

        for (int y = 0; y < numbY; y++)
        {
            newTriangles[mesT + 0] = vIndex + 0;
            newTriangles[mesT + 1] = vIndex + 2;
            newTriangles[mesT + 2] = vIndex + 1;

            newTriangles[mesT + 3] = vIndex + 1;
            newTriangles[mesT + 4] = vIndex + 2;
            newTriangles[mesT + 5] = vIndex + 3;
            mesT += 6; 
            vIndex +=2;
        }

        mesh.vertices = mesh.vertices.Concat(newVertexs).ToArray();
        mesh.triangles = mesh.triangles.Concat(newTriangles).ToArray();

        mesh.RecalculateNormals();
    }
    void Update()
    {
        if (generate)
        {
            generate = !generate;
            myUpdateMesh(mesh.vertices, mesh.triangles, WorldY);
        }
    }


    /*
    void generarMesh()
    {
        triangles = new int[WorldX * WorldY * 6];
        vertexs = new Vector3[(WorldX + 1) * (WorldY + 1)];

        int i = 0;
        for (int y = 0; y <= WorldY; y++)
        {
            for (int x = 0; x <= WorldX; x++)
            {
                vertexs[i] = new Vector3(x, y, 0);
                i++;
            }
        }

        int mesT = 0;
        int mesV = 0;

        for (int y = 0; y < WorldY; y++)
        {
            for (int x = 0; x < WorldX; x++)
            {
                triangles[mesT + 0] = mesV + 0;
                triangles[mesT + 1] = mesV + WorldX + 1;
                triangles[mesT + 2] = mesV + 1;

                triangles[mesT + 3] = mesV + 1;
                triangles[mesT + 4] = mesV + WorldX + 1;
                triangles[mesT + 5] = mesV + WorldX + 2;
                
                mesV ++; //es per augmentar la base sobre on es fica el vertex
                mesT += 6; //cada quadrat te 6 vertex, que equival a 6 linie

            }
            mesV++; 
        }
    }

    void actualitzarMesh()
    {
        mesh.Clear();
        mesh.vertices = vertexs;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }
    */
}
