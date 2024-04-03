using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class MeshGenerator : MonoBehaviour
{
    public class MeshNode {
        public class SectionsController
        {
            public string rules;
            public Mesh mesh;
            public int sectionIndex;
            public bool sectionEnded;
        }

        float sectionDegree;
        Vector3 startPoint;
        Mesh sectionMesh;
        SectionsController sectionsController;

        Spawner spawner;

        MeshNode diferentSectionNode;
        MeshNode sameSectionNode;


        public MeshNode(Vector3 _startPoint, Spawner _spawner, int sectionIndex)
        {
            sectionDegree = 0;
            spawner = _spawner;
            startPoint = _startPoint;

            sectionsController = new SectionsController();
            sectionsController.mesh = new Mesh();
            sectionsController.sectionIndex = sectionIndex;
            sectionsController.sectionEnded = false;

            sectionMesh = new Mesh();

        }

        public SectionsController startGeneration(string rules)
        {
            sectionsController.rules = rules;
            Mesh generalMesh = null;
            
            var myUnSeenRules = rules;

            foreach (char c in myUnSeenRules)
            {
                if (!spawner.translateStandartRules(c, sectionMesh))
                {
                    if ((Rules.DNAnucleotides)c == Rules.DNAnucleotides.START_BRANCH)
                    {
                        string difSectionRules = myUnSeenRules.Split('[', 2)[1];

                        startPoint = spawner.getPositionCenter();
                        sectionDegree = spawner.getRotation();

                        var auxiliar = goDifSection(difSectionRules);
                        generalMesh = spawner.UnifyMeshes(sectionMesh, auxiliar.mesh);

                        spawner.addPositionAndDegree(startPoint, sectionDegree);
                        string sameSectionRules = auxiliar.rules;

                        //despres de la diferent hi ha mes coses?
                        if (sameSectionRules.Length == 0)
                        {
                            //no hi ha res mes a fer, marxem
                            sectionsController.mesh = generalMesh;
                            sectionsController.rules = "";
                            sectionsController.sectionEnded = true;
                            Debug.Log("all ended!: " + sectionsController.sectionIndex);
                            return sectionsController;
                        }

                        auxiliar = goSameSection(sameSectionRules);
                        generalMesh = spawner.UnifyMeshes(generalMesh, auxiliar.mesh);

                        if (auxiliar.sectionEnded && auxiliar.sectionIndex == sectionsController.sectionIndex)
                        {
                            //en la meva seccio he trobat un final, per tant, retornem el que tenim
                            sectionsController.mesh = generalMesh;
                            sectionsController.rules = auxiliar.rules;
                            sectionsController.sectionEnded = true;
                            Debug.Log("Section ended!: " + sectionsController.sectionIndex);
                            return sectionsController;
                        }

                        myUnSeenRules = auxiliar.rules;
                        break;

                    }
                    if ((Rules.DNAnucleotides)c == Rules.DNAnucleotides.END_BRANCH)
                    {
                        sectionsController.mesh = sectionMesh;
                        sectionsController.rules = myUnSeenRules.Split(']', 2)[1];
                        sectionsController.sectionEnded = true;
                        Debug.Log("Section ended?: " + sectionsController.sectionIndex);
                        return sectionsController;
                    }
                }
            }
            sectionsController.mesh = generalMesh ?? sectionMesh;
            sectionsController.rules = "";
            spawner.addPositionAndDegree(startPoint, sectionDegree);
            return sectionsController;
        }

        SectionsController goDifSection(string rules)
        {
            Debug.Log("GO DIF [:" + rules);
            if (diferentSectionNode == null) diferentSectionNode = new MeshNode(spawner.getPositionCenter(), spawner, sectionsController.sectionIndex+1);
            return diferentSectionNode.startGeneration(rules);
        }
        SectionsController goSameSection(string rules)
        {
            Debug.Log("GO SAME ]:" + rules);
            if (sameSectionNode == null) sameSectionNode = new MeshNode(spawner.getPositionCenter(), spawner, sectionsController.sectionIndex);
            return sameSectionNode.startGeneration(rules);
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

        public Vector3 getPositionCenter() { return center.transform.localPosition; }
        public float getRotation() { return center.transform.rotation.eulerAngles.z; }

        public void addPositionAndDegree(Vector3 newPosition, float newDegree)
        {
            center.transform.localPosition = newPosition;
            center.transform.rotation = Quaternion.AngleAxis(newDegree, Vector3.forward);

        }
        
        public void rotate(float addDegree)
        {
            center.transform.rotation = Quaternion.AngleAxis(center.transform.rotation.eulerAngles.z + addDegree, Vector3.forward);
        }

        public Mesh UnifyMeshes(Mesh mesh1, Mesh mesh2)
        {
            // Create a new mesh
            Mesh unifiedMesh = new Mesh();

            // Combine vertices
            List<Vector3> vertices = new List<Vector3>(mesh1.vertices);
            vertices.AddRange(mesh2.vertices);

            // Combine triangles
            List<int> triangles = new List<int>(mesh1.triangles);
            // Adjust triangle indices for mesh2
            int vertexOffset = mesh1.vertices.Length;
            for (int i = 0; i < mesh2.triangles.Length; i++)
            {
                triangles.Add(mesh2.triangles[i] + vertexOffset);
            }

            // Assign combined vertices and triangles to the new mesh
            unifiedMesh.vertices = vertices.ToArray();
            unifiedMesh.triangles = triangles.ToArray();

            // Recalculate normals
            unifiedMesh.RecalculateNormals();

            return unifiedMesh;
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

        public bool translateStandartRules(char value, Mesh mesh)
        {
            switch ((Rules.DNAnucleotides)value)
            {
                case Rules.DNAnucleotides.GROW:
                    //creix la mesh acutal
                    grow(1, mesh);
                    break;
                case Rules.DNAnucleotides.POSITIVE_ROTATION:
                    //rotem en +
                    rotate(25);
                    break;
                case Rules.DNAnucleotides.NEGATIVE_ROTATION:
                    //rotem en -
                    rotate(-25);
                    break;
                case Rules.DNAnucleotides.NONE:
                    //no fem res
                    break;
                default: return false;
            }
            return true;
        }
    }

    [SerializeField] bool generate;

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
    MeshNode meshNode;
    string rules;
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        GOspwaner = new Spawner(transform.gameObject);
        GOspwaner.addPositionAndDegree(Vector3.zero, transform.localEulerAngles.z);

        rules = "G+[[N]-N]-G[-GN]+N";
        meshNode = new MeshNode(GOspwaner.getPositionCenter(), GOspwaner, 0);
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

    public Mesh growMesh(Mesh oldMesh, int growY)
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

    void Update()
    {
        try
        {
            if (generate)
            {
                generate = !generate;

                meshNode = new MeshNode(GOspwaner.getPositionCenter(), GOspwaner, 0);

                GetComponent<MeshFilter>().mesh = meshNode.startGeneration(rules).mesh;
                GOspwaner.addPositionAndDegree(Vector3.zero, transform.localEulerAngles.z);
                rules = rules.Replace("G", "GG");
                rules = rules.Replace("N", "G+[[N]-N]-G[-GN]+N");

            }
        }
        catch(Exception e)
        {
            Debug.LogError("Something goes wrong: " + e.Message + "\n" + e.StackTrace);
        }
    }
}
