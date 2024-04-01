using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class MeshGenerator : MonoBehaviour
{
    public class MeshNode {
        int lenght;
        int sectionLenght;

        MeshGenerator meshG;

        MeshNode diferentSectionNode;


        Mesh newMesh;

        public MeshNode(int _lenght, MeshGenerator _meshG)
        {
            lenght = _lenght;
            sectionLenght = 1;
            meshG = _meshG;
        }

        public Mesh goDiferentSameSection(Mesh oldMesh) {
            //primer generem la nova mesh en referencia al vell

            if (oldMesh == null) newMesh = meshG.myGenerarMeshNode(sectionLenght);
            else newMesh = meshG.myUpdateMeshNode(oldMesh, sectionLenght);

            if (lenght - (sectionLenght) <= 0) return newMesh;//hem arribat al final

            diferentSectionNode = new MeshNode(lenght - 1, meshG);

            return diferentSectionNode.goDiferentSameSection(newMesh);
        }



    }

    public class Spawner
    {
        GameObject parent, center, growCenter, left, right;
        public Spawner(GameObject _parent)
        {
            parent = _parent;
            center = new GameObject("center-spawner");
            center.transform.parent = _parent.transform;
            center.transform.position = _parent.transform.localPosition;

            left = new GameObject("left-spawner");
            left.transform.parent = center.transform;
            left.transform.localPosition = (Vector3.left * 0.5f);

            right = new GameObject("right-spawner");
            right.transform.parent = center.transform;
            right.transform.localPosition = (Vector3.right * 0.5f);

            growCenter = new GameObject("growCenter-spawner");
            growCenter.transform.parent = center.transform;
            growCenter.transform.localPosition = (Vector3.up);
        }

        public void addPosition(Vector3 newPosition, float newDegree)
        {
            center.transform.localPosition = newPosition;
            center.transform.rotation = Quaternion.AngleAxis(newDegree, Vector3.forward);

        }
        
        public void rotate(float addDegree)
        {
            center.transform.rotation = Quaternion.AngleAxis(center.transform.rotation.eulerAngles.z + addDegree, Vector3.forward);
        }

        public Mesh grow(int lenghtY, Mesh mesh)
        {
            var debuger = "";
            int yExtra = 0;
            //estem generant de 0 o ja tenim mesh generada?
            if (mesh.vertexCount != 0)
            {
                growCenter.transform.parent = parent.transform;

                center.transform.localPosition = growCenter.transform.localPosition;

                growCenter.transform.parent = center.transform;
                growCenter.transform.localPosition = Vector3.up;
                yExtra = -1;
            }

            Vector3[] newVertices = new Vector3[2 * (lenghtY + yExtra + 1)];


            for (int y = 0, i = 0; y <= (lenghtY + yExtra); y++)
            {
                //es quade en ordre ascendent cap a Y, per tant,
                //priumer guardem el V3 dels V 0 i 2, despres els 1 i 3, etc

                left.transform.parent = parent.transform;
                right.transform.parent = parent.transform;

                newVertices[i++] = left.transform.localPosition;
                debuger += newVertices[i-1] + ", ";
                newVertices[i++] = right.transform.localPosition;
                debuger += newVertices[i-1] + ", ";


                left.transform.parent = center.transform;
                right.transform.parent = center.transform;
                if (y + 1 <= (lenghtY + yExtra))
                {
                    growCenter.transform.parent = parent.transform;

                    center.transform.localPosition = growCenter.transform.localPosition;

                    growCenter.transform.parent = center.transform;
                    growCenter.transform.localPosition = Vector3.up;
                }
            }
            Debug.Log(debuger);
            debuger = "";


            int vIndex = (mesh.triangles.Length / 6) * 2;
            int[] newTriangles = new int[lenghtY * 6];

            for (int y = 0, mesT = 0; y < lenghtY; y++)
            {
                newTriangles[mesT + 0] = vIndex + 0;
                newTriangles[mesT + 1] = vIndex + 2;
                newTriangles[mesT + 2] = vIndex + 1;

                newTriangles[mesT + 3] = vIndex + 1;
                newTriangles[mesT + 4] = vIndex + 2;
                newTriangles[mesT + 5] = vIndex + 3;

                mesT += 6;
                vIndex += 2;
            }

            mesh.vertices = mesh.vertices.Concat(newVertices).ToArray();
            mesh.triangles = mesh.triangles.Concat(newTriangles).ToArray();

            mesh.RecalculateNormals();
            return mesh;

        }
    }

    [SerializeField] bool generate, rotate;

    [SerializeField] int WorldY = 1, degreeAdd = 0;
    Mesh mesh;
    GameObject spanwer;

    Spawner GOspwaner;

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
        //spanwer = new GameObject("spanwer");
        //spanwer.transform.parent = transform;
        //spanwer.transform.position = transform.localPosition;
        //myGenerarMesh(WorldY);

        GOspwaner = new Spawner(transform.gameObject);
    }

    public Mesh myGenerarMeshNode(int numbY)
    {
        int[] myTriangles = new int[numbY * 6];
        Vector3[] myVertexs = new Vector3[(2) * (numbY + 1)];

        int i = 0;
        for (int y = 0; y <= numbY; y++)
        {
            //es quade en ordre ascendent cap a Y, per tant,
            //priumer guardem el V3 dels V 0 i 2, despres els 1 i 3, etc
            myVertexs[i++] = new Vector3(spanwer.transform.localPosition.x - 0.5f, spanwer.transform.localPosition.y + y, 0);
            myVertexs[i++] = new Vector3(spanwer.transform.localPosition.x + 0.5f, spanwer.transform.localPosition.y + y, 0);
            spanwer.transform.position += Vector3.up * y;
        }

        int mesT = 0;
        int vIndex = 0;

        for (int y = 0; y < numbY; y++)
        {
            //estem ficant l'index d'on es trove el vector que volem
            myTriangles[mesT + 0] = vIndex + 0;
            myTriangles[mesT + 1] = vIndex + 2;
            myTriangles[mesT + 2] = vIndex + 1;

            myTriangles[mesT + 3] = vIndex + 1;
            myTriangles[mesT + 4] = vIndex + 2;
            myTriangles[mesT + 5] = vIndex + 3;

            mesT += 6;
            vIndex += 2;
        }

        Mesh nodeMesh = new Mesh();
        nodeMesh.vertices = myVertexs;
        nodeMesh.triangles = myTriangles;

        nodeMesh.RecalculateNormals();
        return nodeMesh;
    }

    public Mesh growMesh(Mesh oldMesh, int growY)//¡? tinc son
    {
        int[] myTriangles = new int[growY * 6];
        Vector3[] myVertexs = new Vector3[(2) * (growY + 1)];

        int i = 0;
        for (int y = 0; y <= growY; y++)
        {
            //es quade en ordre ascendent cap a Y, per tant,
            //priumer guardem el V3 dels V 0 i 2, despres els 1 i 3, etc
            myVertexs[i++] = new Vector3(spanwer.transform.localPosition.x - 0.5f, spanwer.transform.localPosition.y + y, 0);
            myVertexs[i++] = new Vector3(spanwer.transform.localPosition.x + 0.5f, spanwer.transform.localPosition.y + y, 0);
            spanwer.transform.position += Vector3.up * y;
        }

        int mesT = 0;
        int vIndex = (oldMesh.triangles.Length / 6) * 2;

        for (int y = 0; y < growY; y++)
        {
            //estem ficant l'index d'on es trove el vector que volem
            myTriangles[mesT + 0] = vIndex + 0;
            myTriangles[mesT + 1] = vIndex + 2;
            myTriangles[mesT + 2] = vIndex + 1;

            myTriangles[mesT + 3] = vIndex + 1;
            myTriangles[mesT + 4] = vIndex + 2;
            myTriangles[mesT + 5] = vIndex + 3;

            mesT += 6;
            vIndex += 2;
        }

        Mesh nodeMesh = new Mesh();
        nodeMesh.vertices = myVertexs;
        nodeMesh.triangles = myTriangles;

        nodeMesh.RecalculateNormals();
        return nodeMesh;
    }

    public Mesh myUpdateMeshNode(Mesh oldMesh, int numbY)
    {
        int[] newTriangles = new int[numbY * 6];
        Vector3[] newVertexs = new Vector3[2 * numbY];

        float baseY = oldMesh.vertices[oldMesh.vertices.Length - 1].y;
        int i = 0;

        float deviationDegrees = 50;


        for (float y = 1; y <= numbY; y++)
        {
            var left = new GameObject("left");
            left.transform.parent = spanwer.transform;
            left.transform.localPosition =  (Vector3.left * 0.5f);

            var center = new GameObject("center");
            center.transform.parent = spanwer.transform;
            center.transform.localPosition = (Vector3.up);

            var right = new GameObject("right");
            right.transform.parent = spanwer.transform;
            right.transform.localPosition = (Vector3.right * 0.5f);

            spanwer.transform.rotation = Quaternion.AngleAxis(deviationDegrees, Vector3.forward);
            center.transform.parent = transform;
            spanwer.transform.localPosition = center.transform.localPosition;

            left.transform.parent = transform;
            right.transform.parent = transform;

            newVertexs[i++] = left.transform.localPosition;
            newVertexs[i++] = right.transform.localPosition;


        }

        int mesT = 0;
        int vIndex = (oldMesh.triangles.Length / 6) * 2;

        for (int y = 0; y < numbY; y++)
        {
            newTriangles[mesT + 0] = vIndex + 0;
            newTriangles[mesT + 1] = vIndex + 2;
            newTriangles[mesT + 2] = vIndex + 1;

            newTriangles[mesT + 3] = vIndex + 1;
            newTriangles[mesT + 4] = vIndex + 2;
            newTriangles[mesT + 5] = vIndex + 3;
            mesT += 6;
            vIndex += 2;
        }

        oldMesh.vertices = oldMesh.vertices.Concat(newVertexs).ToArray();
        oldMesh.triangles = oldMesh.triangles.Concat(newTriangles).ToArray();
        
        oldMesh.RecalculateNormals();
        return oldMesh;
    }
    /*
    public void myGenerarMesh(int numbY)
    {
        int[] myTriangles = new int[ numbY * 6];
        Vector3[] myVertexs = new Vector3[(2) * (numbY + 1)];

        int i = 0;
        for (int y = 0; y <= numbY; y++)
        {
            //es quade en ordre ascendent cap a Y, per tant,
            //priumer guardem el V3 dels V 0 i 2, despres els 1 i 3, etc
            myVertexs[i++] = new Vector3(-0.5f, y, 0);
            myVertexs[i++] = new Vector3(0.5f, y, 0);
        }

        int mesT = 0;
        int vIndex = 0;

        for (int y = 0; y < numbY; y++)
        {
            //estem ficant l'index d'on es trove el vector que volem
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

    public void myUpdateMesh(Vector3[] actualV, int[] actualT, int numbY)
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
    */
    void Update()
    {
        try
        {
            if (generate)
            {
                generate = !generate;
                //myUpdateMesh(mesh.vertices, mesh.triangles, WorldY);

                //spanwer.transform.position = transform.localPosition;
                //MeshNode node = new MeshNode(WorldY, this);
                //Mesh newMesh = node.goDiferentSameSection(null);

                //mesh.Clear();
                //mesh.vertices = newMesh.vertices;
                //mesh.triangles = newMesh.triangles;

                mesh = GOspwaner.grow(WorldY, mesh);
            }
            if (rotate)
            {
                rotate = !rotate;
                GOspwaner.rotate(degreeAdd);

                //mesh = GOspwaner.grow(WorldY, mesh);
            }
        }
        catch(Exception e)
        {
            Debug.LogError("Something goes wrong: " + e.Message + "\n" + e.StackTrace);
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
