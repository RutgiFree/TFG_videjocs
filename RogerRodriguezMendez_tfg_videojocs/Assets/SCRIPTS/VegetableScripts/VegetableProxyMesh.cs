using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

public class VegetableProxyMesh : MonoBehaviour
{
    public class MeshInfo
    {
        Vector3[] vertices;
        int[] triangles;

        public MeshInfo(): this(new Vector3[0], new int[0]) { }
        public MeshInfo(Vector3[] _vertices, int[] _triangles)
        {
            vertices = _vertices;
            triangles = _triangles;
        }

        public void setVertices(Vector3[] _vertices) { vertices = _vertices; }
        public void setTriangles(int[] _triangles) { triangles = _triangles; }

        public Vector3[] getVertices() { return vertices; }
        public int[] getTriangles() { return triangles; }

        public bool isEmpty() { return vertices.Length == 0 && triangles.Length == 0;  }

    }
    public class SectionsController
    {
        MeshInfo meshInfo;
        string rules;
        int sectionIndex;
        bool sectionEnded;

        public SectionsController(MeshInfo _meshInfo, int _sectionIndex, bool _sectionEnded)
        {
            meshInfo = _meshInfo;
            rules = "";
            sectionIndex = _sectionIndex;
            sectionEnded = _sectionEnded;
        }

        public void EndSection(string _rules)
        {
            sectionEnded = true;
            rules = _rules;
        }
        public void setRules(string _rules) { rules = _rules; }
        public void setMeshInfo(MeshInfo _meshInfo) { meshInfo = _meshInfo; }

        public MeshInfo getMeshInfo() { return meshInfo; }
        public string getRules() { return rules; }
        public int getIndex() { return sectionIndex; }
        public bool getEnded() { return sectionEnded; }
    }

    public class MeshNode
    {

        float sectionDegree;
        Vector3 startPoint;
        SectionsController sectionsController;

        Spawner spawner;

        MeshNode diferentSectionNode;
        MeshNode sameSectionNode;


        public MeshNode(Vector3 _startPoint, Spawner _spawner, int sectionIndex)
        {
            sectionDegree = 0;
            spawner = _spawner;
            startPoint = _startPoint;

            sectionsController = new SectionsController(new MeshInfo(), sectionIndex, false);
        }

        public SectionsController startGeneration(string rules)
        {
            sectionsController.setRules(rules);
            MeshInfo generalMeshInfo = new MeshInfo();

            var myUnSeenRules = rules;

            foreach (char c in myUnSeenRules)
            {
                if (!spawner.translateStandartRules(c, sectionsController.getMeshInfo()))
                {
                    if ((Rules.DNAnucleotides)c == Rules.DNAnucleotides.START_BRANCH)
                    {
                        string difSectionRules = myUnSeenRules.Split('[', 2)[1];

                        startPoint = spawner.getPositionCenter();
                        sectionDegree = spawner.getRotation();

                        var auxiliar = goDifSection(difSectionRules);
                        generalMeshInfo = spawner.UnifyTwoMeshes(sectionsController.getMeshInfo(), auxiliar.getMeshInfo());

                        spawner.setPositionAndDegree(startPoint, sectionDegree);
                        string sameSectionRules = auxiliar.getRules();

                        //despres de la diferent hi ha mes coses?
                        if (sameSectionRules.Length == 0)
                        {
                            //no hi ha res mes a fer, marxem
                            sectionsController.setMeshInfo(generalMeshInfo);
                            sectionsController.EndSection("");
                            return sectionsController;
                        }

                        auxiliar = goSameSection(sameSectionRules);
                        generalMeshInfo = spawner.UnifyMeshes(generalMeshInfo, auxiliar.getMeshInfo());

                        if (auxiliar.getEnded() && auxiliar.getIndex() == sectionsController.getIndex())
                        {
                            //en la meva seccio he trobat un final, per tant, retornem el que tenim
                            sectionsController.setMeshInfo(generalMeshInfo);
                            sectionsController.EndSection(auxiliar.getRules());
                            return sectionsController;
                        }

                        myUnSeenRules = auxiliar.getRules();
                        break;

                    }
                    if ((Rules.DNAnucleotides)c == Rules.DNAnucleotides.END_BRANCH)
                    {
                        sectionsController.setMeshInfo(sectionsController.getMeshInfo());
                        sectionsController.EndSection(myUnSeenRules.Split(']', 2)[1]);
                        return sectionsController;
                    }
                }
            }
            //sectionsController.setRules("");
            if (generalMeshInfo.isEmpty()) sectionsController.setMeshInfo(sectionsController.getMeshInfo());
            else sectionsController.setMeshInfo(generalMeshInfo);
            sectionsController.EndSection("");
            spawner.setPositionAndDegree(startPoint, sectionDegree);
            return sectionsController;
        }

        SectionsController goDifSection(string rules)
        {
            if (diferentSectionNode == null) diferentSectionNode = new MeshNode(spawner.getPositionCenter(), spawner, sectionsController.getIndex() + 1);
            return diferentSectionNode.startGeneration(rules);
        }
        SectionsController goSameSection(string rules)
        {
            if (sameSectionNode == null) sameSectionNode = new MeshNode(spawner.getPositionCenter(), spawner, sectionsController.getIndex());
            return sameSectionNode.startGeneration(rules);
        }
    }

    public class Spawner
    {
        GameObject parent, center, growCenter, left, right;
        MeshFilter meshFilterRecipient;
        float initialDegree;
        public Spawner(GameObject _parent)
        {
            parent = _parent;
            center = new GameObject("center-spawner");
            center.transform.parent = _parent.transform;
            center.transform.position = _parent.transform.localPosition;
            initialDegree = _parent.transform.eulerAngles.z;
            meshFilterRecipient = center.AddComponent<MeshFilter>();

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

        public void setPositionAndDegree(Vector3 newPosition, float newDegree)
        {
            center.transform.localPosition = newPosition;
            center.transform.rotation = Quaternion.AngleAxis(newDegree, Vector3.forward);
        }
        public void resetAllPositions()
        {
            center.transform.localPosition = Vector3.zero;
            center.transform.rotation = Quaternion.AngleAxis(initialDegree, Vector3.forward);
            left.transform.localPosition = (Vector3.left * 0.5f);
            right.transform.localPosition = (Vector3.right * 0.5f);
            growCenter.transform.localPosition = (Vector3.up);
        }

        public void setRotation(float addDegree)
        {
            center.transform.rotation = Quaternion.AngleAxis(center.transform.rotation.eulerAngles.z + addDegree, Vector3.forward);
        }


        public MeshInfo UnifyMeshes(MeshInfo mesh1, MeshInfo mesh2)
        {
            // Combine vertices (using AddRange makes the proces more eficient)
            List<Vector3> vertices = new List<Vector3>(mesh1.getVertices());
            int vertexOffset = vertices.Count;
            vertices.AddRange(mesh2.getVertices());

            // Combine triangles
            List<int> trianglesM1 = new List<int>(mesh1.getTriangles());
            List<int> trianglesM2 = new List<int>(mesh2.getTriangles());

            // Adjust triangle indices for mesh2
            for (int i = 0; i < trianglesM2.Count; i++)
            {
                trianglesM1.Add(trianglesM2[i] + vertexOffset);
            }

            // Assign combined vertices and triangles to the new mesh
            MeshInfo unifiedMesh = new MeshInfo(vertices.ToArray(),trianglesM1.ToArray());            

            // Recalculate normals
            //unifiedMesh.RecalculateNormals();-------------------------------------------------------------------------------------------------------------------------------------------------

            return unifiedMesh;
        }

        public MeshInfo UnifyTwoMeshes(MeshInfo mesh1, MeshInfo mesh2)
        {
            List<int> trianglesM1 = new List<int>(mesh1.getTriangles());
            List<int> trianglesM2 = new List<int>(mesh2.getTriangles());

            List<Vector3> vertices = new List<Vector3>(mesh1.getVertices());
            int vertexOffset = vertices.Count();

            List<Vector3> verticesM2 = new List<Vector3>(mesh2.getVertices());
            bool doMerge = verticesM2.Count() >= 2;

            if (doMerge)
            {
                verticesM2.RemoveAt(0);
                verticesM2.RemoveAt(0);

                trianglesM2[0] = vertices.Count() - 2; 
                trianglesM1.Add(trianglesM2[0]);

                trianglesM2[1] = trianglesM2[1] + vertexOffset - 2;
                trianglesM1.Add(trianglesM2[1]);

                trianglesM2[2] = vertices.Count() - 1; 
                trianglesM1.Add(trianglesM2[2]);
            }

            vertices.AddRange(verticesM2);

            for (int i = doMerge? 3 : 0; i < trianglesM2.Count; i++)
            {
                if (doMerge)
                {
                    trianglesM1.Add(trianglesM2[i] + vertexOffset - 2);
                }
                else trianglesM1.Add(trianglesM2[i] + vertexOffset);
            }

            MeshInfo unifiedMesh = new MeshInfo(vertices.ToArray(), trianglesM1.ToArray());

            return unifiedMesh;
        }

        public MeshInfo grow(int lenghtY, MeshInfo meshInfo)
        {
            int yExtra = 0;
            List<Vector3> vertices = new List<Vector3>(meshInfo.getVertices());
            List<int> triangles = new List<int>(meshInfo.getTriangles());

            //estem generant de 0 o ja tenim mesh generada?
            if (!meshInfo.isEmpty())
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

                newVertices[i++] = right.transform.localPosition;


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

            int vIndex = (triangles.Count / 6) * 2;
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

            vertices.AddRange(newVertices);
            triangles.AddRange(newTriangles);

            meshInfo.setVertices(vertices.ToArray());
            meshInfo.setTriangles(triangles.ToArray());

            return meshInfo;
        }

        public bool translateStandartRules(char value, MeshInfo meshInfo)
        {
            switch ((Rules.DNAnucleotides)value)
            {
                case Rules.DNAnucleotides.GROW:
                    //creix la mesh acutal
                    grow(1, meshInfo);
                    break;
                case Rules.DNAnucleotides.POSITIVE_ROTATION:
                    //rotem en +
                    setRotation(25);
                    break;
                case Rules.DNAnucleotides.NEGATIVE_ROTATION:
                    //rotem en -
                    setRotation(-25);
                    break;
                case Rules.DNAnucleotides.NONE:
                    //no fem res
                    break;
                default: return false;
            }
            return true;
        }
    }


    [SerializeField] string vName;
    [SerializeField] public Rules.states vState { get; private set; }
    [SerializeField] string activeDNA;

    Spawner GOspwaner;
    MeshNode meshNode;

    Vegetable myVegetable;
    Mesh vegetableMesh;

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

    void Awake()
    {
        GOspwaner = new Spawner(transform.gameObject);
        GOspwaner.setPositionAndDegree(Vector3.zero, transform.localEulerAngles.z);

        activeDNA = ((char)Rules.DNAnucleotides.NONE).ToString();
        vegetableMesh = new Mesh();
    }


    public void setVegetable(Vegetable vegetable)//li diem quina hortalissa es
    {
        if (vegetable == null) throw new System.NotImplementedException();
        myVegetable = vegetable;
        vName = myVegetable.name;
        vState = myVegetable.myState;
    }

    public void getFruit()
    {
        if (myVegetable == null) throw new System.NotImplementedException();
        myVegetable.getFruit();
    }
    public string pasTime()
    {
        if (myVegetable == null) throw new System.NotImplementedException();
        activeDNA = myVegetable.pasTime(activeDNA);

        meshNode = new MeshNode(GOspwaner.getPositionCenter(), GOspwaner, 0);//iniciem la generacio de 0 sempre
        MeshInfo meshInfo = meshNode.startGeneration(activeDNA).getMeshInfo();

        vegetableMesh.Clear();
        vegetableMesh.vertices = meshInfo.getVertices();
        vegetableMesh.triangles = meshInfo.getTriangles();
        vegetableMesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = vegetableMesh;
        GOspwaner.resetAllPositions();//recetegem la posicio del spawner
        return activeDNA;
    }

    public Rules.states nextState()//pasem a la seguent fase
    {
        if (myVegetable == null) throw new System.NotImplementedException();
        vState = myVegetable.nextState();
        return vState;
    }



}
