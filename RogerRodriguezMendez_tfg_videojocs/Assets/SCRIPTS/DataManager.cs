using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[Serializable]
public class SerialisableArrayContainer<T>
{
    public T[] items;
}

[InitializeOnLoad]
public class DataManager
{
    const string dataPath= "MyGameData";
    public static vegetableObject[] vegetables { get; private set; }

    static DataManager()
    {

        vegetableObject[] auxData = LoadVegetableObject();
        if (auxData == null)
        {
            vegetableObject carrot = new vegetableObject("Carrot", new Rules.states[] { Rules.states.GERMINATION, Rules.states.GROWING, Rules.states.DYING }, 0);
            auxData = new vegetableObject[] { carrot };
            SaveVegetableObject(auxData);
        }
        vegetables = auxData;

    }

    public static void SaveVegetableObject(vegetableObject[] vegetable)
    {
        string combinePath = Path.Combine(dataPath, "VegetableObjects");
        try
        {
            //if the directory is not created it will be created.
            Directory.CreateDirectory(Path.GetDirectoryName(combinePath));

            //the data
            SerialisableArrayContainer<vegetableObject> container = new SerialisableArrayContainer<vegetableObject>();
            container.items = vegetable;
            string jsonData = JsonUtility.ToJson(container, true);

            //all is ready to be saved, now it's time to create the channels:
            using (FileStream strem = new FileStream(combinePath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(strem))
                {
                    writer.Write(jsonData);
                }
            }

            for (int i = 0; i < vegetable.Length; i++)
            {
                Debug.Log("SAVED: "+vegetable[i].debug());
            }
        }
        catch( Exception e)
        {
            Debug.LogError("ERROR ON: SaveVegetableObject(vegetableObject[] vegetable)->" + combinePath + "\n" + e);
        }
       
    }

    public static vegetableObject[] LoadVegetableObject()
    {
        string combinePath = Path.Combine(dataPath, "VegetableObjects");

        if (!File.Exists(combinePath)) return null; //no information to load
        vegetableObject[] loadedVegetables = null; //inicilise variable

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
            SerialisableArrayContainer<vegetableObject> container = JsonUtility.FromJson<SerialisableArrayContainer<vegetableObject>>(rawData);
            loadedVegetables = container.items;
            for (int i = 0; i < loadedVegetables.Length; i++)
            {
                Debug.Log("LOADED: "+loadedVegetables[i].debug());
            }

        }
        catch (Exception e)
        {
            Debug.LogError("ERROR ON: LoadVegetableObject() ->"+ combinePath+"\n" + e);
        }

        return loadedVegetables;
    }
}
