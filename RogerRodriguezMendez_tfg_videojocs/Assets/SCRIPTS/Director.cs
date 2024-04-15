using System;
using System.Linq;
using UnityEngine;

 [RequireComponent(typeof(VegetableProxyMesh))]
public class Director : MonoBehaviour
{//vegetable sistem? com a nom
    [SerializeField] VegetableProxyMesh vProxy;
    [SerializeField] string vName;
    [SerializeField] int vIteretionsDone;
    [SerializeField] string vDNA;
    [SerializeField] Rules.states vState;


    void Start()
    {
        vProxy = GetComponent<VegetableProxyMesh>();
    }

    public bool setVegetable(string _vName)
    {
        try
        {
            vProxy.setVegetable(DataManager.getVegetable(_vName));
            vState = vProxy.vState;
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }

    public int pasTime()
    {
        try
        {
            vDNA = vProxy.pasTime();
            return ++vIteretionsDone;
        }
        catch (Exception e)
        {
            Debug.LogError("Something goes wrong: " + e.Message + "\n" + e.StackTrace);
            return -1;
        }
    }

    public void changeState()
    {
        try
        {
            vState = vProxy.nextState();
            if (vState == Rules.states.DEATH) Destroy(this.gameObject);
        }
        catch (Exception e)
        {
            Debug.LogError("Something goes wrong: " + e.Message + "\n" + e.StackTrace);
        }
    }
}
