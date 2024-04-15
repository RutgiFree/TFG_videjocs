using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class VegetableProxyMesh : MonoBehaviour
{
    public class MeshNode
    {
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

                        spawner.setPositionAndDegree(startPoint, sectionDegree);
                        string sameSectionRules = auxiliar.rules;

                        //despres de la diferent hi ha mes coses?
                        if (sameSectionRules.Length == 0)
                        {
                            //no hi ha res mes a fer, marxem
                            sectionsController.mesh = generalMesh;
                            sectionsController.rules = "";
                            sectionsController.sectionEnded = true;
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
                        return sectionsController;
                    }
                }
            }
            sectionsController.mesh = generalMesh ?? sectionMesh;
            sectionsController.rules = "";
            spawner.setPositionAndDegree(startPoint, sectionDegree);
            return sectionsController;
        }

        SectionsController goDifSection(string rules)
        {
            if (diferentSectionNode == null) diferentSectionNode = new MeshNode(spawner.getPositionCenter(), spawner, sectionsController.sectionIndex + 1);
            return diferentSectionNode.startGeneration(rules);
        }
        SectionsController goSameSection(string rules)
        {
            if (sameSectionNode == null) sameSectionNode = new MeshNode(spawner.getPositionCenter(), spawner, sectionsController.sectionIndex);
            return sameSectionNode.startGeneration(rules);
        }
    }

    public class Spawner
    {
        GameObject parent, center, growCenter, left, right;
        MeshFilter meshFilterRecipient;
        public Spawner(GameObject _parent)
        {
            parent = _parent;
            center = new GameObject("center-spawner");
            center.transform.parent = _parent.transform;
            center.transform.position = _parent.transform.localPosition;
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

        public void setRotation(float addDegree)
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

        public Mesh addMeshToPosition(Mesh mesh, Vector3 bottomPosMesh, float angleMesh, Vector3 finalPos, float finalAngle)
        {
            //checkpoit del spawner
            Vector3 spanwerPos = center.transform.localPosition;
            float spawnerAngle = center.transform.localEulerAngles.z;

            center.transform.localPosition = bottomPosMesh;
            center.transform.rotation = Quaternion.AngleAxis(angleMesh, Vector3.forward);

            meshFilterRecipient.mesh = mesh;

            center.transform.localPosition = finalPos;
            center.transform.rotation = Quaternion.AngleAxis(finalAngle, Vector3.forward);

            //recetegem el spawner on era i com estave
            //meshFilterRecipient.mesh = new Mesh();
            //addPositionAndDegree(spanwerPos, spawnerAngle);
            return null;
        }

        public Mesh grow(int lenghtY, Mesh mesh)
        {
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
        GetComponent<MeshFilter>().mesh = meshNode.startGeneration(activeDNA).mesh;
        GOspwaner.setPositionAndDegree(Vector3.zero, transform.localEulerAngles.z);//recetegem la posicio del spawner
        return activeDNA;
    }

    public Rules.states nextState()//pasem a la seguent fase
    {
        if (myVegetable == null) throw new System.NotImplementedException();
        vState = myVegetable.nextState();
        return vState;
    }



}
