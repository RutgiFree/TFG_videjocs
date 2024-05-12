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
        int angle;

        public MeshInfo(int _angle) : this(new Vector3[0], new int[0], _angle) { }
        public MeshInfo(Vector3[] _vertices, int[] _triangles, int _angle)
        {
            vertices = _vertices;
            triangles = _triangles;
            angle = _angle;
        }

        public void addAngle(int _angle) { angle += _angle; }
        public void setVertices(Vector3[] _vertices) { vertices = _vertices; }
        public void setTriangles(int[] _triangles) { triangles = _triangles; }

        public Vector3[] getVertices() { return vertices; }
        public int[] getTriangles() { return triangles; }
        public int getAngle() { return angle; }

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

    public class FruitsFlower
    {
        bool isFruit;
        GameObject gObj;

        public FruitsFlower(GameObject _gObj, bool _isFruit)
        {
            gObj = _gObj;
            isFruit = _isFruit;
        }

        public bool getIsFruit() { return isFruit; }
        public GameObject getGameObject() { return gObj; }

        public void deleteObject() {
            Destroy(gObj);
        }
    }
    public class Leaf
    {
        GameObject gObj;

        public Leaf(GameObject _gObj)
        {
            gObj = _gObj;
        }

        public GameObject getGameObject() { return gObj; }

        public void deleteObject()
        {
            Destroy(gObj);
        }
    }

    public class MeshNode
    {

        float sectionDegree;
        Vector3 startPoint;
        SectionsController sectionsController;

        Spawner spawner;

        MeshNode newStartNode;
        Queue<MeshNode> resetNodes;

        Queue<FruitsFlower> fruits;
        Queue<FruitsFlower> flowers;
        Queue<Leaf> leafs;

        public MeshNode(Vector3 _startPoint, Spawner _spawner, int sectionIndex, int _angle)
        {
            sectionDegree = 0;
            spawner = _spawner;
            startPoint = _startPoint;

            sectionsController = new SectionsController(new MeshInfo(_angle), sectionIndex, false);
            resetNodes = new Queue<MeshNode>();
            fruits = new Queue<FruitsFlower>();
            flowers = new Queue<FruitsFlower>();
            leafs = new Queue<Leaf>();
        }

        public void deleteAllGObj()
        {
            foreach (FruitsFlower f in fruits)
                f.deleteObject();

            foreach (FruitsFlower f in flowers) 
                f.deleteObject();

            foreach (Leaf l in leafs)
                l.deleteObject();

            foreach (MeshNode rn in resetNodes)
                rn.deleteAllGObj();

            if (newStartNode != null)
                newStartNode.deleteAllGObj();
        }

        public SectionsController startGeneration(string rules)
        {
            sectionsController.setRules(rules);
            MeshInfo generalMeshInfo = null; 

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
                            spawner.setPositionAndDegree(startPoint, sectionDegree);

                            resetNodeRules = newResetNode.getRules();

                            if(resetNodeRules.Length == 0)
                            {
                                var allMeshInfo1 = getAllResetsMeshInfo();
                                sectionsController.setMeshInfo(spawner.UnifyMultyMeshes(sectionsController.getMeshInfo(), allMeshInfo1));
                                sectionsController.EndSection("");
                                return sectionsController;
                            }


                        } while ((Rules.DNAnucleotides)resetNodeRules[0] == Rules.DNAnucleotides.START_BRANCH);

                        var allMeshInfo2 = new List<MeshInfo>(getAllResetsMeshInfo());
                        var startNode = goNewStartNode(resetNodeRules);

                        allMeshInfo2.Add(startNode.getMeshInfo());

                        generalMeshInfo = spawner.UnifyMultyMeshes(sectionsController.getMeshInfo(), allMeshInfo2.ToArray());

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
                    if ((Rules.DNAnucleotides)c == Rules.DNAnucleotides.FRUIT)
                    {
                        GameObject fruit = spawner.createFruit();
                        if (fruit)
                            fruits.Enqueue(new FruitsFlower(fruit, true));
                    }
                    if ((Rules.DNAnucleotides)c == Rules.DNAnucleotides.LEAF)
                    {
                        GameObject leaf = spawner.createLeaf();
                        if (leaf)
                            leafs.Enqueue(new Leaf(leaf));
                    }
                }
            }

            //sectionsController.setRules("");
            if (generalMeshInfo == null || generalMeshInfo.isEmpty()) sectionsController.setMeshInfo(sectionsController.getMeshInfo());
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
            resetNodes.Enqueue(new MeshNode(spawner.getPositionCenter(), spawner, sectionsController.getIndex() + 1, sectionsController.getMeshInfo().getAngle()));
            return resetNodes.Last().startGeneration(rules);
        }
        SectionsController goNewStartNode(string rules)
        {
            newStartNode = new MeshNode(spawner.getPositionCenter(), spawner, sectionsController.getIndex(), sectionsController.getMeshInfo().getAngle());
            return newStartNode.startGeneration(rules);
        }
    }

    public class Spawner
    {
        GameObject parent, center, growCenter, left, right;

        GameObject fruitVariant, leafVariant;

        float initialDegree, heigth, width;
        public Spawner(GameObject _parent, float squerWidth, float squereHeight)
        {
            parent = _parent;
            center = new GameObject("center-spawner");
            center.transform.parent = _parent.transform;
            center.transform.position = _parent.transform.localPosition;
            initialDegree = _parent.transform.eulerAngles.z;

            left = new GameObject("left-spawner");
            left.transform.parent = center.transform;
            left.transform.localPosition = (Vector3.left * squerWidth);

            right = new GameObject("right-spawner");
            right.transform.parent = center.transform;
            right.transform.localPosition = (Vector3.right * squerWidth);

            growCenter = new GameObject("growCenter-spawner");
            growCenter.transform.parent = center.transform;
            growCenter.transform.localPosition = (Vector3.up * squereHeight);
            heigth = squereHeight;
            width = squerWidth;
        }

        public void setFruitsVariants(GameObject _fruitVariant) { fruitVariant = _fruitVariant; }

        public void setLeafsVariants(GameObject _leafVariant) { leafVariant = _leafVariant; }

        public GameObject createLeaf()
        {
            if (leafVariant == null) return null;
            var fruit = createGO(leafVariant);
            fruit.name = "LEAF";
            fruit.transform.rotation = center.transform.rotation;
            fruit.transform.localScale *= heigth;
            return fruit;
        }
        public GameObject createFruit()
        {
            if (leafVariant == null) return null;
            var fruit = createGO(fruitVariant);
            fruit.name = "FRUIT";
            fruit.transform.localScale *= heigth;
            return fruit;
        }

        public GameObject createGO(GameObject gObj)
        {
            gObj = Instantiate(gObj);
            gObj.transform.parent = parent.transform;
            gObj.transform.localPosition = center.transform.localPosition;
            return gObj;
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
            left.transform.localPosition = (Vector3.left * width);
            right.transform.localPosition = (Vector3.right * width);
            growCenter.transform.localPosition = (Vector3.up * heigth);
        }

        public void setRotation(float addDegree)
        {
            center.transform.rotation = Quaternion.AngleAxis(center.transform.rotation.eulerAngles.z + addDegree, Vector3.forward);
        }

        public MeshInfo UnifyMultyMeshes(MeshInfo baseM, MeshInfo[] meshes)
        {
            //eliminem les meshes sense informacio, i si encara hi han coses seguim, sino retornem la base
            var aux = meshes.Where(mesh => !mesh.isEmpty()).ToArray();
            if (aux.Length == 0) return baseM;
            else meshes = aux;

            //ordenem de mes gran a mes petit respecte l'angle (o el que es el amteix, d'esquerra a dreta)
            Array.Sort(meshes, (a, b) => b.getAngle().CompareTo(a.getAngle()));

            //agafo i preparo la base
            List<Vector3> vertices = new List<Vector3>(baseM.getVertices());
            List<int> triangles = new List<int>(baseM.getTriangles());

            //pero, la base es empty?
            if (vertices.Count() == 0)
            {
                //tenim que crea una base sobre la qual es treballe, on aquesta base seran 2 vertex
                vertices.Add(left.transform.localPosition);
                vertices.Add(right.transform.localPosition);
            }

            int vertexOffset = vertices.Count();
                int vertexBaseOffset = vertices.Count();
            List<int> trianglesM2 = new List<int>();
            List<Vector3> verticesM2 = new List<Vector3>();

            //ara els tenim ordenar per el seu grau, aixi que, anirem de esquerra a dreta.
            //també savem el numero / llarga dels que s'uniran
            //hem de mirar si els afegits a la base son >= 2, sino es el cas, es fa una unio simple

            //es una unió Complexa?
            if (meshes.Length >= 2)
            {
                //savem que el primer de la llista es l'element de l'esquerra i que l'ultim es el de la dreta.
                //anirem de dreta a esquerra, pero avans volem dividr la llista en 2, a fi de fer la part esquerra i la part dreta
                //per tant, es necari saver quin es punt del mig:

                List<Vector3> oldMidPointV = new List<Vector3>();
                List<int> oldMidPointT = new List<int>();
                List<int> totalMidPoint = new List<int>();

                //quin es el centre?
                int midIndex;

                if (meshes.Length % 2 == 0) midIndex = (meshes.Length / 2) - 1;
                else midIndex = ((meshes.Length-1) / 2);

                //si tenim length 2 => el de la meitat es 1, pero caldrà tranformarlo en index, per tant, 1-1 = 0
                //si tenim length 3 => el de la meitat es 2, pero caldrà tranformarlo en index, per tant, 2-1 = 1
                //si tenim length 4 => el de la meitat es 3, pero caldrà tranformarlo en index, per tant, 3-1 = 2
                //...


                int index = 0;
                foreach (MeshInfo mesh in meshes)
                {
                    //preparem les variables
                    trianglesM2.Clear();
                    verticesM2.Clear();
                    trianglesM2.AddRange(mesh.getTriangles());
                    verticesM2.AddRange(mesh.getVertices());

                    //eliminem els nostres primers 2 vertex que subtituirem pels 2 ultims de la base
                    verticesM2.RemoveAt(0);
                    verticesM2.RemoveAt(0);

                    //elimino els 3 primer triangles
                    trianglesM2.RemoveAt(0);
                    trianglesM2.RemoveAt(0);
                    trianglesM2.RemoveAt(0);

                    //si ens troivem mes enlla del centre la base offset pase a ser -1
                    if (index <= midIndex)
                    {
                        trianglesM2[0] = vertexBaseOffset - 2; //agafem el penultim vertex de la base en la que ens volem unir (esquerra)
                    }
                    else if(index > midIndex)
                    {
                        trianglesM2[0] = vertexBaseOffset - 1; //agafem l'enultim vertex de la base en la que ens volem unir (dreta)
                        if(totalMidPoint.Count() ==0)
                            totalMidPoint.Add(oldMidPointT[0]); //guardem el punt considerat el centre total de la unio, l'anterior vell punt mig
                    }

                    triangles.Add(trianglesM2[0]);

                    //no tenim vell punt entremig?
                    if (oldMidPointV.Count() == 0)
                    {
                        //aixi doncs estem a la esquerra del tot
                        trianglesM2[1] = trianglesM2[1] + vertexOffset - 2; //manteim el nostre pero en referencia a la base (offset) => el -2 es pk hem eliminat 2 vertexs
                    }
                    else
                    {   //agafem el vell punt intermig
                        verticesM2[trianglesM2[1] - 2] = oldMidPointV[0];
                        trianglesM2[1] = oldMidPointT[0];
                    }

                    triangles.Add(trianglesM2[1]);


                    //tenim algu a la nostras dreta?
                    if (index + 1 < meshes.Length)
                    {
                        //agafem el entremig entre jo i el de la dreta
                        verticesM2[trianglesM2[2] - 2] = Vector3.Lerp(verticesM2[trianglesM2[2] - 2], meshes[index + 1].getVertices()[2], 0.5f);
                    }

                    oldMidPointV.Clear();
                    oldMidPointV.Add(verticesM2[trianglesM2[2] - 2]);//guardem el futur bell punt intermig

                    oldMidPointT.Clear();
                    oldMidPointT.Add(trianglesM2[2] + vertexOffset - 2);//guardem l'index d'on es trove el futur bell punt intermig

                    trianglesM2[2] = trianglesM2[2] + vertexOffset - 2; //manteim el nostre pero en referencia a la base (offset) => el -2 es pk hem eliminat 2 vertexs
                    triangles.Add(trianglesM2[2]);

                    vertices.AddRange(verticesM2);
                    for (int i = 3; i < trianglesM2.Count; i++)
                        triangles.Add(trianglesM2[i] + vertexOffset - 2);

                    vertexOffset = vertices.Count();

                    index++;
                }

                //ara cal omplir aquest triangle "buit" que hi ha entre els elements esquerres i els drets
                //necesitem quin es el punt considerant mig (el vertex)
                //despres encesitem els 2 vertex de la base

                //aixi doncs, i seguin l'ordre en el que es fiquen els triangles, farem:
                triangles.Add(vertexBaseOffset - 2);
                triangles.Add(totalMidPoint[0]);
                triangles.Add(vertexBaseOffset - 1);
            }
            
            else if(meshes.Length  == 1) //es una unió simple
            {
                //preparem les variables
                trianglesM2.AddRange(meshes[0].getTriangles());
                verticesM2.AddRange(meshes[0].getVertices());

                //eliminem els nostres primers 2 vertex que subtituirem pels 2 ultims de la base
                verticesM2.RemoveAt(0);
                verticesM2.RemoveAt(0);

                trianglesM2[0] = vertexOffset - 2; //agafem el penultim vertex de la base en la que ens volem unir
                triangles.Add(trianglesM2[0]);

                trianglesM2[1] = trianglesM2[1] + vertexOffset - 2; //manteim el nostre pero en referencia a la base (offset) => el -2 es pk hem eliminat 2 vertexs
                triangles.Add(trianglesM2[1]);

                trianglesM2[2] = vertexOffset - 1;//agafem l'ultim vertex de la base en la que ens volem unir
                triangles.Add(trianglesM2[2]);

                trianglesM2[3] = trianglesM2[2];//agafem l'ultim vertex de la base en la que ens volem unir
                triangles.Add(trianglesM2[3]);


                vertices.AddRange(verticesM2);
                for (int i = 4; i < trianglesM2.Count; i++)
                    triangles.Add(trianglesM2[i] + vertexOffset -2);
                
            }

            
            return new MeshInfo(vertices.ToArray(), triangles.ToArray(), baseM.getAngle());
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
                growCenter.transform.localPosition = Vector3.up * heigth;
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
                    growCenter.transform.localPosition = Vector3.up * heigth;
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
                case Rules.DNAnucleotides.GROW_1:
                    //creix la mesh acutal Y = 1
                    grow(1, meshInfo);
                    break;
                case Rules.DNAnucleotides.GROW_2:
                    //creix la mesh acutal Y = 2
                    grow(2, meshInfo);
                    break;
                case Rules.DNAnucleotides.GROW_3:
                    //creix la mesh acutal Y = 3
                    grow(3, meshInfo);
                    break;
                case Rules.DNAnucleotides.GROW_4:
                    //creix la mesh acutal Y = 4
                    grow(4, meshInfo);
                    break;
                case Rules.DNAnucleotides.POSITIVE_ROTATION:
                    //rotem en +
                    setRotation(10);
                    meshInfo.addAngle(10);
                    break;
                case Rules.DNAnucleotides.NEGATIVE_ROTATION:
                    //rotem en -
                    setRotation(-10);
                    meshInfo.addAngle(-10);
                    break;
                case Rules.DNAnucleotides.CONTINUO_ROTATION:
                    //rotem en la mateixa direcció
                    if (meshInfo.getAngle() > 0) setRotation(5);
                    else setRotation(-5);
                    break;
                case Rules.DNAnucleotides.NONE | Rules.DNAnucleotides.AUX_1 | 
                Rules.DNAnucleotides.AUX_2 | Rules.DNAnucleotides.AUX_3 | 
                Rules.DNAnucleotides.AUX_4 | Rules.DNAnucleotides.AUX_5 | 
                Rules.DNAnucleotides.AUX_6:
                    //no fem res, es l'axioma -> NONE
                    //no fem res, son auxiliars -> AUX_...
                    break;
                default: return false;
            }
            return true;
        }
    }


    [SerializeField] string vName;
    [SerializeField] public Rules.states vState { get; private set; }
    [SerializeField] string activeDNA;

    [SerializeField]
    UnityGObjMap fruitsMap;

    [SerializeField]
    UnityGObjMap leafsMap;

    [SerializeField]
    UnityMaterialMap branchesMap;

    [SerializeField]
    float width = 0.25f, heigth = 1f;

    Dictionary<string, GameObject[]> fruitsVariants, leafsVariants;
    Dictionary<string, Material[]> bracnhresVariants;

    Material branchMaterial;

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
        GOspwaner = new Spawner(transform.gameObject, width, heigth);
        GOspwaner.setPositionAndDegree(Vector3.zero, transform.localEulerAngles.z);

        activeDNA = ((char)Rules.DNAnucleotides.NONE).ToString();
        vegetableMesh = new Mesh();

        fruitsVariants = fruitsMap.toDictionaryGObj();
        leafsVariants = leafsMap.toDictionaryGObj();
        bracnhresVariants = branchesMap.toDictionaryMaterial();
    }

    public void deleteAll()
    {
        meshNode.deleteAllGObj();
        Destroy(gameObject);
    }


    public void setVegetable(Vegetable vegetable)//li diem quina hortalissa es
    {
        if (vegetable == null) throw new NotImplementedException();
        myVegetable = vegetable;
        vName = myVegetable.name;
        vState = myVegetable.myState;

        setVariants(GOspwaner);

    }

    public void setVariants(Spawner _spawner)
    {
        GameObject[] auxGameObj;
        Material[] auxMaterial;

        if (fruitsVariants.TryGetValue(vName.ToLower(), out auxGameObj))
            _spawner.setFruitsVariants(auxGameObj[Random.Range(0, auxGameObj.Length)]);

        if (leafsVariants.TryGetValue(vName.ToLower(), out auxGameObj))
            _spawner.setLeafsVariants(auxGameObj[Random.Range(0, auxGameObj.Length)]);

        if (bracnhresVariants.TryGetValue(vName.ToLower(), out auxMaterial))
            branchMaterial = (auxMaterial[Random.Range(0, auxMaterial.Length)]);
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


        if(meshNode!!= null)
            meshNode.deleteAllGObj();
        

        meshNode = new MeshNode(GOspwaner.getPositionCenter(), GOspwaner, 0, 0);//iniciem la generacio de 0 sempre
        MeshInfo meshInfo = meshNode.startGeneration(activeDNA).getMeshInfo();

        vegetableMesh.Clear();
        vegetableMesh.vertices = meshInfo.getVertices();
        vegetableMesh.triangles = meshInfo.getTriangles();
        vegetableMesh.RecalculateNormals();
        if (branchMaterial)
        {
            //si tenim un m,aterial a afegor cal calcular les UVs per dirli a la mesh quins colors agafara
            Vector2[] uvs = new Vector2[vegetableMesh.vertices.Length];
            float y = 0;
            for (int i = 0; i < uvs.Length; i++)
            {
                //Savem que les coordenades en les UVs van de (0,0) a (1,1), on el centre absolut es (0'5, 0'5),
                //per tant, el centre inferior es (0'5,0), i el nostre centre absolñut en coordenades de vertex es (0,0).
                //Savent aixo cal fer una transformacio per que no hi agi cionflictes en les coordenades, i per tant poder aplicar una gradiant de colors.

                //aixi doncs, agafam una cordenada de vertes, per exemple (-0'5,0) que equivaldria a  un verteex inferior a la dreta,
                //i la dividim per l'alçada, que es pot obtenir mitjant "bounds.size.y", fent que per alçada tota la mesh capigui dintre del material.
                //Per el que fa l'amplada, es fa un calcul mes llarg, on agafem de la cordenada la seva X, i li sumem l'amplada, que es pot obtenir mitjant "bounds.size.x"

        
                uvs[i] = new Vector2(0, y);
                i++;

                if(i < uvs.Length)
                    uvs[i] = new Vector2(1, y);
                
                y++;
                if (y > 1) y = 0;
            }

            vegetableMesh.uv = uvs;

            GetComponent<MeshRenderer>().material = branchMaterial;
        }

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
[Serializable]
public class UnityGObjMap
{
    [SerializeField]
    UnityGObjMapElements[] elementsGObjects;

    public Dictionary<string, GameObject[]> toDictionaryGObj()
    {

        Dictionary<string, GameObject[]> dictionary = new Dictionary<string, GameObject[]>();

        foreach(UnityGObjMapElements element in elementsGObjects)
            dictionary.Add(element.key, element.value);

        return dictionary;
    }
}

[Serializable]
public class UnityMaterialMap
{
    [SerializeField]
    UnityMaterialMapElements[] materialElements;

    public Dictionary<string, Material[]> toDictionaryMaterial()
    {

        Dictionary<string, Material[]> dictionary = new Dictionary<string, Material[]>();

        foreach (UnityMaterialMapElements element in materialElements)
            dictionary.Add(element.key, element.value);

        return dictionary;
    }
}

[Serializable]
public class UnityGObjMapElements
{
    [SerializeField]
    public string key;
    [SerializeField]
    public GameObject[] value;
}

[Serializable]
public class UnityMaterialMapElements
{
    [SerializeField]
    public string key;
    [SerializeField]
    public Material[] value;
}