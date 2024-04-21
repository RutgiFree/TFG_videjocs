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
        public float angle;

        public MeshInfo(): this(new Vector3[0], new int[0]) { }
        public MeshInfo(Vector3[] _vertices, int[] _triangles)
        {
            vertices = _vertices;
            triangles = _triangles;
            angle = 0;
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

        MeshNode diferentSectionNode;//podem tenir N diferents
        MeshNode newStartNode;//podem tenir N same

        Queue<MeshNode> resetNodes;

        public MeshNode(Vector3 _startPoint, Spawner _spawner, int sectionIndex)
        {
            sectionDegree = 0;
            spawner = _spawner;
            startPoint = _startPoint;

            sectionsController = new SectionsController(new MeshInfo(), sectionIndex, false);
            resetNodes = new Queue<MeshNode>();
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
                        startPoint = spawner.getPositionCenter();
                        sectionDegree = spawner.getDegree();

                        string resetNodeRules = myUnSeenRules;

                        do
                        {
                            resetNodeRules = resetNodeRules.Split('[', 2)[1];


                            var newResetNode = goResetNode(resetNodeRules);

                            resetNodeRules = newResetNode.getRules();

                            if(resetNodeRules.Length == 0)
                            {
                                var allMeshInfo1 = getAllResetsMeshInfo();

                                sectionsController.setMeshInfo(spawner.UnifyMultyMeshes(sectionsController.getMeshInfo(), allMeshInfo1));
                                sectionsController.EndSection("");
                                return sectionsController;
                            }

                            spawner.setPositionAndDegree(startPoint, sectionDegree);

                        } while ((Rules.DNAnucleotides)resetNodeRules[0] == Rules.DNAnucleotides.START_BRANCH);

                        var allMeshInfo2 = getAllResetsMeshInfo();

                        generalMeshInfo = spawner.UnifyMultyMeshes(sectionsController.getMeshInfo(), allMeshInfo2);

                        var startNode = goNewStartNode(resetNodeRules);
                        generalMeshInfo = spawner.UnifyMeshes(generalMeshInfo, startNode.getMeshInfo());

                        //tenim 3 meshes a unir. que fem?
                        //la mesh dif i same tenen informacio? si no es aixi no unim res
                        //la mesh dif te informacio i la mesh same no? o a la inversa? unim la base amb la que te informacio

                        //les dues tenen informacio? unim les 3 meshes, pero quina es dreta? i quina es esquerra?

                        /*
                        if (!auxiliar1.getMeshInfo().isEmpty() && !auxiliar2.getMeshInfo().isEmpty())
                        {
                            Debug.Log("2 meshes!");
                            if (auxiliar1.getMeshInfo().angle > 0 && auxiliar2.getMeshInfo().angle <= 0)
                            {
                                Debug.Log("1r!: " + (auxiliar1.getMeshInfo().angle) + ", " + (auxiliar2.getMeshInfo().angle));
                                generalMeshInfo = spawner.UnifyTreeMeshes(sectionsController.getMeshInfo(), auxiliar1.getMeshInfo(), auxiliar2.getMeshInfo());
                            }
                            else if (auxiliar2.getMeshInfo().angle > 0 && auxiliar1.getMeshInfo().angle <= 0)
                            {
                                Debug.Log("2n!: " + (auxiliar1.getMeshInfo().angle) + ", " + (auxiliar2.getMeshInfo().angle));
                                generalMeshInfo = spawner.UnifyTreeMeshes(sectionsController.getMeshInfo(), auxiliar2.getMeshInfo(), auxiliar1.getMeshInfo());
                            }
                            else if (auxiliar1.getMeshInfo().angle >= 0 && auxiliar2.getMeshInfo().angle >= 0 || auxiliar1.getMeshInfo().angle <= 0 && auxiliar2.getMeshInfo().angle <= 0)
                            {
                                var dif = Math.Abs(auxiliar1.getMeshInfo().angle) - Math.Abs(auxiliar2.getMeshInfo().angle);
                                if (dif > 0)
                                {
                                    Debug.Log("3r!: " + (auxiliar1.getMeshInfo().angle) + ", " + (auxiliar2.getMeshInfo().angle));
                                    generalMeshInfo = spawner.UnifyTreeMeshes(sectionsController.getMeshInfo(), auxiliar1.getMeshInfo(), auxiliar2.getMeshInfo());
                                }
                                else if (dif < 0)
                                {
                                    Debug.Log("4rt!: " + (auxiliar1.getMeshInfo().angle) + ", " + (auxiliar2.getMeshInfo().angle));
                                    generalMeshInfo = spawner.UnifyTreeMeshes(sectionsController.getMeshInfo(), auxiliar2.getMeshInfo(), auxiliar1.getMeshInfo());
                                }
                                else
                                {
                                    Debug.Log("5e!: "+(auxiliar1.getMeshInfo().angle) +", "+(auxiliar2.getMeshInfo().angle));
                                    generalMeshInfo = spawner.UnifyMeshes(spawner.UnifyMeshes(sectionsController.getMeshInfo(), auxiliar1.getMeshInfo()), auxiliar2.getMeshInfo());
                                }
                            }
                        }
                        else if (!auxiliar1.getMeshInfo().isEmpty())
                        {
                            Debug.Log("DIF mesh!: "+auxiliar1.getRules());
                            generalMeshInfo = spawner.UnifyTwoMeshes(sectionsController.getMeshInfo(), auxiliar1.getMeshInfo());
                        }
                        else if(!auxiliar2.getMeshInfo().isEmpty())
                        {
                            Debug.Log("SAME mesh!" + auxiliar2.getRules());
                            generalMeshInfo = spawner.UnifyTwoMeshes(sectionsController.getMeshInfo(), auxiliar2.getMeshInfo());
                        }
                        */

                        if (startNode.getEnded() && startNode.getIndex() == sectionsController.getIndex())
                        {
                            //en la meva seccio he trobat un final, per tant, retornem el que tenim
                            sectionsController.setMeshInfo(generalMeshInfo);
                            sectionsController.EndSection(startNode.getRules());
                            return sectionsController;
                        }

                        myUnSeenRules = startNode.getRules();
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

        MeshInfo[] getAllResetsMeshInfo()
        {
            List<MeshInfo> retorn = new List<MeshInfo>();
            foreach(MeshNode mn in resetNodes)
            {
                retorn.Add(mn.sectionsController.getMeshInfo());
            }
            return retorn.ToArray();
        }

        SectionsController goResetNode(string rules)
        {
            resetNodes.Enqueue(new MeshNode(spawner.getPositionCenter(), spawner, sectionsController.getIndex() + 1));
            return resetNodes.Last().startGeneration(rules);
        }
        SectionsController goNewStartNode(string rules)
        {
            newStartNode = new MeshNode(spawner.getPositionCenter(), spawner, sectionsController.getIndex());
            return newStartNode.startGeneration(rules);
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
        public float getDegree()
        {
            return center.transform.rotation.eulerAngles.z;
        }

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


        public MeshInfo UnifyMeshes(MeshInfo baseM, MeshInfo mesh)
        {
            // Combine vertices (using AddRange makes the proces more eficient)
            List<Vector3> vertices = new List<Vector3>(baseM.getVertices());
            int vertexOffset = vertices.Count;
            vertices.AddRange(mesh.getVertices());

            // Combine triangles
            List<int> trianglesM1 = new List<int>(baseM.getTriangles());
            List<int> trianglesM2 = new List<int>(mesh.getTriangles());

            // Adjust triangle indices for mesh2
            foreach (int t in trianglesM2)
                trianglesM1.Add(t + vertexOffset);


            // Assign combined vertices and triangles to the new mesh
            return new MeshInfo(vertices.ToArray(), trianglesM1.ToArray());
        }

        public MeshInfo UnifyMultyMeshes(MeshInfo baseM, MeshInfo[] meshes)
        {
            List<Vector3> vertices = new List<Vector3>(baseM.getVertices());
            List<int> triangles = new List<int>(baseM.getTriangles());
            
            int vertexOffset = vertices.Count();
            List<int> trianglesM2 = new List<int>();

            foreach(MeshInfo mi in meshes)
            {
            // Combine vertices (using AddRange makes the proces more eficient)
                vertices.AddRange(mi.getVertices());
                trianglesM2.AddRange(mi.getTriangles());

                foreach (int t in trianglesM2)
                    triangles.Add(t + vertexOffset);
                

                vertexOffset = vertices.Count();
                trianglesM2.Clear();
            }
            return new MeshInfo(vertices.ToArray(), triangles.ToArray());
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
        public MeshInfo UnifyTreeMeshes(MeshInfo bMesh, MeshInfo lMesh, MeshInfo rMesh)
        {
            if (bMesh.getVertices().Length < 2 || lMesh.getVertices().Length < 2 || rMesh.getVertices().Length < 2) return UnifyMeshes(UnifyMeshes(bMesh, lMesh), rMesh);
            StringBuilder sb = new StringBuilder();

            List<Vector3> verticesMb = new List<Vector3>(bMesh.getVertices());
            List<Vector3> verticesMl = new List<Vector3>(lMesh.getVertices());
            List<Vector3> verticesMr = new List<Vector3>(rMesh.getVertices());
            List<Vector3> unifiedVertices = new List<Vector3>(bMesh.getVertices());

            List<int> trianglesMb = new List<int>(bMesh.getTriangles());
            List<int> trianglesMl = new List<int>(lMesh.getTriangles());
            List<int> trianglesMr = new List<int>(rMesh.getTriangles());
            List<int> unifiedTriangles = new List<int>(bMesh.getTriangles());

            int vertexOffset = verticesMb.Count();

            //primer unium la lMesh a la bMesh, per tant, eliminem els 2 primers vertecs
            //canviant-los per els 2 ultims vertes de la bMesh

            //eliminem els e primers vertexs
            verticesMl.RemoveAt(0);
            verticesMl.RemoveAt(0);

            Debug.Log("1r UNIFICACIO!!");
            Debug.Log("ABANS Ml vertex -> " + trianglesMl[0] + ", " + trianglesMl[1] + ", " + trianglesMl[2] + ", " + trianglesMl[3]);

            sb.Append("TRIANGLES Mb ABANS: ");
            trianglesMb.ForEach(t => sb.Append(t+", "));
            Debug.Log(sb.ToString());
            sb.Clear();

            //mofiquem el primer triangle del lMesh
            trianglesMl[0] = verticesMb.Count() - 2; //agafem l'index del penultum vertex de bMesh
            unifiedTriangles.Add(trianglesMl[0]);

            trianglesMl[1] = trianglesMl[1] + vertexOffset - 2; //manteim l'index d'aquest vertex pero en referencia a la base. (-2) => puix hem eliminat 2 vertex
            unifiedTriangles.Add(trianglesMl[1]);

            trianglesMl[2] = verticesMb.Count() - 1; //agafem l'index de l'ultim vertex de bMesh
            unifiedTriangles.Add(trianglesMl[2]);

            //modifiquem l'inici del segon triangle
            trianglesMl[3] = trianglesMl[2]; //agafem l'index de l'ultim vertex de bMesh
            unifiedTriangles.Add(trianglesMl[3]);

            Debug.Log("DESPRES Ml vertex -> " + trianglesMl[0] + ", " + trianglesMl[1] + ", " + trianglesMl[2] + ", " + trianglesMl[3]);


            //modifiquem els index de la restra de triangles, en referencia a la base
            for (int i = 4; i < trianglesMl.Count(); i++)
                unifiedTriangles.Add(trianglesMl[i] + vertexOffset - 2); //manteim l'index del vertex pero en referencia a la base. (-2) => puix hem eliminat 2 vertex

            sb.Append("TRIANGLES UNIFICATS DESPRES: ");
            unifiedTriangles.ForEach(t => sb.Append(t + ", "));
            Debug.Log(sb.ToString());
            sb.Clear();

            unifiedVertices.AddRange(verticesMl);

            vertexOffset = unifiedVertices.Count();

            //ara unifiquem la rMesh al conjunt. Aixi doncs, eliminarem el primer triangle i
            //modificarem els 2 primer vertes del segon triangle en referencia a la bMesh i lMesh.

            //eliminem els primers vertexs
            verticesMr.RemoveAt(0);
            verticesMr.RemoveAt(0);
            verticesMr.RemoveAt(0);


            Debug.Log("2n UNIFICACIO!!");
            Debug.Log(" -> " + trianglesMr[0] + ", " + trianglesMr[1] + ", " + trianglesMr[2]);
            //eliminem el primer triangle
            trianglesMr.RemoveAt(0);
            trianglesMr.RemoveAt(0);
            trianglesMr.RemoveAt(0);

            //mofiquem el segon triangle respecte a la bMesh, pel primer vertex,
            // i respecte a la lMesh, pel segon vertex.

            Debug.Log("ABANS Mr vertex -> " + trianglesMr[0] + ", " + trianglesMr[1] + ", " + trianglesMr[2] + ", " + trianglesMr[3]);

            sb.Append("TRIANGLES Mb ABANS: ");
            trianglesMb.ForEach(t => sb.Append(t + ", "));
            Debug.Log(sb.ToString());
            sb.Clear();

            sb.Append("TRIANGLES Ml ABANS: ");
            trianglesMl.ForEach(t => sb.Append(t + ", "));
            Debug.Log(sb.ToString());
            sb.Clear();

            trianglesMr[0] = verticesMb.Count() - 1; //agafem l'index de l'ultim vertex de bMesh
            unifiedTriangles.Add(trianglesMr[0]);
            

            trianglesMr[1] = trianglesMl[5] + verticesMb.Count() - 2; //agafem l'index de l'ultim vertex de lMesh en el 2n triangle. (-2) => puix hem eliminat 2 vertex en Ml
            unifiedTriangles.Add(trianglesMr[1]);

            trianglesMr[2] = trianglesMr[2] + vertexOffset - 3; //manteim l'index d'aquest vertex pero en referencia a la base. (-1) => puix hem eliminat 2 vertex
            unifiedTriangles.Add(trianglesMr[2]);

            //modifiquem el meu propi per unirlo al del Ml
            trianglesMr[3] = trianglesMr[1];
            unifiedTriangles.Add(trianglesMr[3]);

            Debug.Log("DESPRES Mr vertex -> " + trianglesMr[0] + ", " + trianglesMr[1] + ", " + trianglesMr[2] + ", " + trianglesMr[3]);


            //modifiquem els index de la restra de triangles, en referencia a la base
            for (int i = 4; i < trianglesMr.Count(); i++)
                unifiedTriangles.Add(trianglesMr[i] + vertexOffset - 3); //manteim l'index del vertex pero en referencia a la base. (-3) => puix hem eliminat 2 vertex

            sb.Append("TRIANGLES UNIFICATS DESPRES: ");
            unifiedTriangles.ForEach(t => sb.Append(t + ", "));
            Debug.Log(sb.ToString());
            sb.Clear();

            unifiedVertices.AddRange(verticesMr);

            MeshInfo unifiedMesh = new MeshInfo(unifiedVertices.ToArray(), unifiedTriangles.ToArray());

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
                    meshInfo.angle += 25;
                    break;
                case Rules.DNAnucleotides.NEGATIVE_ROTATION:
                    //rotem en -
                    setRotation(-25);
                    meshInfo.angle -= 25;
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
