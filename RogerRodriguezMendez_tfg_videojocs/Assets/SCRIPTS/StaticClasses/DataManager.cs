using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Serializable]
public class SerialisableMapContainer<TK, TV>
{
    public TK[] keys;
    public TV[] values;
}

[InitializeOnLoad]
public class DataManager
{
    const string dataPath= "MyGameData";
    private static Dictionary<string, VegetableObjectWorker> vegetablesMemory;
    public static string[] vegetablesNames { get; private set; }



    static DataManager()
    {

        Dictionary<string, VegetableObjectWorker> auxData = LoadVegetableObject();
        if (auxData == null)
        {
            auxData = new Dictionary<string, VegetableObjectWorker>();

            //carrot
            VegetableObjectWorker veg = 
                new VegetableObjectWorker(
                        "Carrot", 
                        new Rules.states[] { Rules.states.GERMINATION, Rules.states.GROWING, Rules.states.DYING }, 
                        0);
            auxData.Add(veg.name.ToUpper(), veg);

            //eaggplant
            veg = 
                new VegetableObjectWorker(
                        "Eaggplant", 
                        new Rules.states[] { Rules.states.GERMINATION, Rules.states.GROWING, Rules.states.FLOWERING, Rules.states.RIPENING, Rules.states.DYING }, 
                        0);
            auxData.Add(veg.name.ToUpper(), veg);


            SaveVegetableObject(auxData);
        }
        vegetablesMemory = auxData;
        vegetablesNames = auxData.Keys.ToArray();

    }

    public static VegetableObjectWorker getVegetable(string name)
    {
        if (!vegetablesNames.Contains(name.ToUpper())) throw new Exception("Vegetable not present");
        return vegetablesMemory[name.ToUpper()];
    }

    public static void SaveVegetableObject(Dictionary<string, VegetableObjectWorker> vegetable)
    {
        string combinePath = Path.Combine(dataPath, "VegetableObjectsMap");
        try
        {
            //if the directory is not created it will be created.
            Directory.CreateDirectory(Path.GetDirectoryName(combinePath));

            //the data
            SerialisableMapContainer<string, VegetableObjectWorker> container = new SerialisableMapContainer<string, VegetableObjectWorker>();
            container.keys = vegetable.Keys.ToArray(); 
            container.values = vegetable.Values.ToArray();

            string jsonData = JsonUtility.ToJson(container, true);

            //all is ready to be saved, now it's time to create the channels:
            using (FileStream strem = new FileStream(combinePath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(strem))
                {
                    writer.Write(jsonData);
                }
            }

            for (int i = 0; i < container.values.Length; i++)
            {
                Debug.Log("SAVED: "+ container.values[i].debug());
            }
        }
        catch( Exception e)
        {
            Debug.LogError("ERROR ON: SaveVegetableObject(vegetableObject[] vegetable)->" + combinePath + "\n" + e);
        }
       
    }

    public static Dictionary<string, VegetableObjectWorker> LoadVegetableObject()
    {
        string combinePath = Path.Combine(dataPath, "VegetableObjectsMap");

        if (!File.Exists(combinePath)) return null; //no information to load
        Dictionary<string, VegetableObjectWorker> loadedVegetables = null; //inicilise variable

        try
        {
            string rawData = "";

            //all is ready to be loaded, now it's time to create the channels:
            using (FileStream strem = new FileStream(combinePath, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(strem))
                {
                    rawData = reader.ReadToEnd();
                }
            }

            //the data
            SerialisableMapContainer<string, VegetableObjectWorker> container = JsonUtility.FromJson<SerialisableMapContainer<string, VegetableObjectWorker>>(rawData);
            string[] keys = container.keys;
            VegetableObjectWorker[] values = container.values;


            Dictionary<string, VegetableObjectWorker> aux = new Dictionary<string, VegetableObjectWorker>();

            for (int i=0; i <keys.Length; i++)
            {
                aux.Add(keys[i], values[i]);
                Debug.Log("LOADING: "+keys[i]+" ->" + values[i].debug());
            }

            loadedVegetables = aux;
            Debug.Log("LOADED ALL: "+loadedVegetables.Count);

        }
        catch (Exception e)
        {
            Debug.LogError("ERROR ON: LoadVegetableObject() ->"+ combinePath+"\n" + e);
        }

        return loadedVegetables;
    }
}
