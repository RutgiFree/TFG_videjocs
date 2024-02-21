using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class VegetableProxy : MonoBehaviour
{
    private class SpawnerCheckPoint
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    private class vNode
    {
        public GameObject vSegment;
        public int segmentID;
        public string myName;
        public Stack<SpawnerCheckPoint> spawnerCheckPoints;

        public vNode sameSegmentChild;
        public vNode newSegmentChild;

        public vNode parent;
    }

    //patro representant? -> patro proxy o patro decorator?
    Vegetable myVegetable;
    [SerializeField] string vName;
    [SerializeField] public Rules.states vState { get; private set; }

    [SerializeField] string activeDNA;
    bool canGrow;
    [SerializeField] float lineLengh;
    [SerializeField] float angle;

    [SerializeField] GameObject branch;



    Stack<vNode> parentsStack;
    GameObject spawner;
    GameObject parent;
    vNode parentNode;
    vNode currentNode;
    int currentPlusLengh;
    int currentSegmentID;

    void Awake()
    {
        activeDNA = ((char)Rules.DNAnucleotides.NONE).ToString();
        canGrow = false;
        lineLengh = 1f;
        currentPlusLengh = 0;
        angle = 25f;


        currentNode = null;
        parentsStack = new Stack<vNode>();
    }

    void Start()
    {
        spawner = new GameObject("spawner");
        spawner.transform.position = transform.position;
        spawner.transform.parent = transform;

        parent = new GameObject("parent");
        parent.transform.position = transform.position;
        parent.transform.parent = transform;

        parentNode = new vNode
        {
            vSegment = parent,
            spawnerCheckPoints = new Stack<SpawnerCheckPoint>(),
            segmentID = 0,
        };
        parentsStack.Push(parentNode);
    }

    void Update()
    {
        if (canGrow)
        {
            canGrow = !canGrow;

            spawner.transform.position = parentNode.vSegment.transform.position;
            spawner.transform.rotation = parentNode.vSegment.transform.rotation;

            //recetegem tot, mirem si el pare te fills del seguent segment (mai te del seu propi)
            currentNode = parentNode.newSegmentChild;

            if(currentNode == null)//no tenim cap hortalisa creada, anem a iniciar-la
            {
                GameObject segment = Instantiate(branch);
                segment.transform.position = parentNode.vSegment.transform.position;
                segment.transform.rotation = parentNode.vSegment.transform.rotation;
                segment.transform.parent = parentsStack.Peek().vSegment.transform;

                currentNode = new vNode
                {
                    vSegment = segment,
                    segmentID = parentNode.segmentID+1,
                    spawnerCheckPoints = new Stack<SpawnerCheckPoint>(),
                    parent = parentNode,
                };
                segment.name = currentNode.segmentID+".1";
                parentNode.newSegmentChild = currentNode;
                parentsStack.Push(currentNode);
            }

            currentSegmentID = currentNode.segmentID;
            currentPlusLengh = 0;
            growVegetable(activeDNA);
        }
    }

    void growVegetable(string _activeDNA)
    {
        string cadena = "";
        foreach (char c in _activeDNA)
        {
            switch ((Rules.DNAnucleotides)c)
            {
                case Rules.DNAnucleotides.GROW: //toca creixe
                    
                    //ara toca ferse més gran
                    GameObject segment = currentNode.vSegment;

                    if (segment == null)//no esta instanciat?
                    {
                        segment = Instantiate(branch);
                        segment.transform.position = spawner.transform.position;//el fikem on el spawner
                        segment.transform.rotation = spawner.transform.rotation;//l'orientem com el spawner
                        segment.transform.parent = parentsStack.Peek().vSegment.transform;//li pasem el seu pare
                        segment.name = currentNode.myName;//li posem el seu nom
                        currentNode.vSegment = segment;//li asinem el segment instanciat
                    }
                    parentsStack.Push(currentNode);

                    spawner.transform.position = segment.transform.position;

                    Vector3 inicialPosition = spawner.transform.position;
                    spawner.transform.Translate(Vector3.up * (lineLengh + currentPlusLengh));

                    segment.GetComponent<LineRenderer>().SetPosition(0, inicialPosition);//posicio inicial
                    segment.GetComponent<LineRenderer>().SetPosition(1, spawner.transform.position);//posicio final

                    //si tornem aqui vol dir que sera més llarg
                    currentPlusLengh++;
                    cadena = cadena + "G";
                    break;

                case Rules.DNAnucleotides.POSITIVE_ROTATION: //toca girar direccio horaria (+)
                    spawner.transform.Rotate(Vector3.back * angle);
                    break;

                case Rules.DNAnucleotides.NEGATIVE_ROTATION: //toca girar direccio anti-horaria (-)
                    spawner.transform.Rotate(Vector3.forward * angle);
                    break;

                case Rules.DNAnucleotides.START_BRANCH: //toca iniciar branca
                    
                    //fem un checkpoit de posicio i angle del spawner
                    currentNode.spawnerCheckPoints.Push(new SpawnerCheckPoint
                    {
                        position = spawner.transform.position,
                        rotation = spawner.transform.rotation,
                    });

                    //canviem el current
                    //te fills de nova secció?
                    var auxCurrent = currentNode.newSegmentChild;
                    if (auxCurrent == null)//no tenim fills de diferent secció, per tant, ho preparem tot per la futura creació
                    {
                        auxCurrent = new vNode
                        {
                            segmentID = currentNode.segmentID + 1,
                            spawnerCheckPoints = new Stack<SpawnerCheckPoint>(),
                            parent = currentNode,
                        };
                        auxCurrent.myName = auxCurrent.segmentID + ".1";
                        currentNode.newSegmentChild = auxCurrent;
                    }
                    currentNode = auxCurrent;

                    if (currentNode.vSegment != null)//trenim fills visibles
                    {
                        currentNode.vSegment.transform.position = spawner.transform.position;
                        currentNode.vSegment.transform.rotation = spawner.transform.rotation;
                    }

                    //pasem al seguent segment
                    currentSegmentID = currentNode.segmentID;

                    //reiniciemt la llargada del segment
                    currentPlusLengh = 0;

                    cadena = cadena + "[" + currentSegmentID;
                    break;

                case Rules.DNAnucleotides.END_BRANCH: //toca tancar branca
                    //tenim que treure els pares de seccions posteriors, ja que volem els pares de seccions anteriors
                    while (parentsStack.Peek().segmentID >= currentSegmentID) parentsStack.Pop();

                    //Estem tancant secció, per tant asegurenmnos que estem a una seccio anterior al "currentSegmentID" (més petita que l'actual)
                    auxCurrent = currentNode;
                    while (auxCurrent.segmentID >= currentSegmentID)//mentrers estigui a una secció més gran o igual a l'actual, seguim buscant
                    {
                        auxCurrent = auxCurrent.parent;
                        if (auxCurrent == null) throw new Exception("ALGUNA COSA NO HA ANAT BE, PAREM MAQUINES -> vegetableProxy");
                    }
                    currentNode = auxCurrent;

                    //ja estem a la anterior secció
                    currentSegmentID = currentNode.segmentID;

                    //agafem al checkpoint del spawner
                    var auxCheckpoint = currentNode.spawnerCheckPoints.Pop();
                    spawner.transform.position = auxCheckpoint.position;
                    spawner.transform.rotation = auxCheckpoint.rotation;

                    //tenim fill de mateixa secció? 
                    auxCurrent = currentNode.sameSegmentChild;
                    if (auxCurrent == null)//no tenim fills de mateixa secció, caldra crear-lo
                    {

                        auxCurrent = new vNode
                        {
                            segmentID = currentNode.segmentID,
                            spawnerCheckPoints = new Stack<SpawnerCheckPoint>(),
                            parent = currentNode,
                        };
                        auxCurrent.myName = auxCurrent.segmentID + ".2";
                        currentNode.sameSegmentChild = auxCurrent;
                    }
                    currentNode = auxCurrent;
                    
                    if(currentNode.vSegment != null)//trenim fills visibles
                    {
                        currentNode.vSegment.transform.position = auxCheckpoint.position;
                        currentNode.vSegment.transform.rotation = auxCheckpoint.rotation;
                    }


                    //reiniciemt la llarga del segment
                    currentPlusLengh = 0;

                    cadena = cadena + "]" + currentSegmentID;
                    break;
                default://no fem res
                    break;
            }
        }
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
        canGrow = true;
        return activeDNA;
    }

    public Rules.states nextState()//pasem a la seguent fase
    {
        if (myVegetable == null) throw new System.NotImplementedException();
        vState = myVegetable.nextState();
        return vState;
    }



}
