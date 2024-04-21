using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;

[InitializeOnLoad]
public class DataManager
{
    const string dataPath= "MyGameData";
    private static Dictionary<string, Vegetable> vegetablesMemory;
    public static string[] vegetablesNames { get; private set; }

    static DataManager()
    {

        Dictionary<string, Vegetable> auxData = LoadVegetableObject();
        if (auxData == null)
        {
            auxData = new Dictionary<string, Vegetable>();

            //carrot
            Dictionary<Rules.DNAnucleotides, DNAinfo[]> myDNA = new Dictionary<Rules.DNAnucleotides, DNAinfo[]>
            {
                { 
                    Rules.DNAnucleotides.NONE, 
                    new DNAinfo[] 
                    { 
                        new DNAinfo("G[+N]G", 5), 
                        new DNAinfo("G[-N]G", 10) 
                    }
                },
                {
                    Rules.DNAnucleotides.GROW,
                    new DNAinfo[]
                    {
                        new DNAinfo("G", 5),
                        new DNAinfo("GG", 10)
                    }
                }
            };


            Vegetable veg =
                new Vegetable(
                        "Carrot",
                        new Rules.states[] { Rules.states.GERMINATION, Rules.states.GROWING, Rules.states.DYING },
                        0, myDNA);
            auxData.Add(veg.name.ToUpper(), veg);

            //eaggplant
            myDNA = new Dictionary<Rules.DNAnucleotides, DNAinfo[]>
            {
                {//G+[[N]-N]-F[-GN]+N //G[+N][-N]
                    Rules.DNAnucleotides.NONE,
                    new DNAinfo[]
                    {
                        new DNAinfo("G+[[N]-N]-G[-GN]+N", 10)
                    }
                },
                {
                    Rules.DNAnucleotides.GROW,
                    new DNAinfo[]
                    {
                        new DNAinfo("GG", 10)
                    }
                }
            };
            veg =
                new Vegetable(
                        "Eaggplant",
                        new Rules.states[] { Rules.states.GERMINATION, Rules.states.GROWING, Rules.states.FLOWERING, Rules.states.RIPENING, Rules.states.DYING },
                        0, myDNA);
            auxData.Add(veg.name.ToUpper(), veg);

            //cacatua
            myDNA = new Dictionary<Rules.DNAnucleotides, DNAinfo[]>
            {
                {//G+[[N]-N]-F[-GN]+N //G[+N][-N]
                    Rules.DNAnucleotides.NONE,
                    new DNAinfo[]
                    {
                        new DNAinfo("G[++G][--G]G", 10)
                    }
                },
                {
                    Rules.DNAnucleotides.GROW,
                    new DNAinfo[]
                    {
                        new DNAinfo("GG", 10)
                    }
                }
            };
            veg =
                new Vegetable(
                        "Cacatua",
                        new Rules.states[] { Rules.states.GERMINATION, Rules.states.GROWING, Rules.states.FLOWERING, Rules.states.RIPENING, Rules.states.DYING },
                        0, myDNA);
            auxData.Add(veg.name.ToUpper(), veg);


            SaveVegetableObject(auxData);
        }
        vegetablesMemory = auxData;
        vegetablesNames = auxData.Keys.ToArray();

    }

    public static Vegetable getVegetable(string name)
    {
        if (!vegetablesNames.Contains(name.ToUpper())) throw new Exception("Vegetable not present");
        return vegetablesMemory[name.ToUpper()];
    }

    public static void SaveVegetableObject(Dictionary<string, Vegetable> vegetable)
    {
        string combinePath = Path.Combine(dataPath, "VegetableObjectsMap");
        try
        {
            if (File.Exists(combinePath)) Debug.Log("AREADY EXISTING PATH... REWRITING IT");

            Debug.Log("SAVING VEGETABLES... ");

            //if the directory is not created it will be created.
            Directory.CreateDirectory(Path.GetDirectoryName(combinePath));

            //the data
            string jsonData = JsonConvert.SerializeObject(vegetable, Formatting.Indented);

            //all is ready to be saved, now it's time to create the channels:
            using (FileStream strem = new FileStream(combinePath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(strem))
                {
                    writer.Write(jsonData);
                }
            }

            Debug.Log("SAVED: "+vegetable.Count);
        }
        catch( Exception e)
        {
            Debug.LogError("ERROR ON: SaveVegetableObject(vegetableObject[] vegetable)->" + combinePath + "\n" + e.StackTrace);
        }
       
    }

    public static Dictionary<string, Vegetable> LoadVegetableObject()
    {
        string combinePath = Path.Combine(dataPath, "VegetableObjectsMap");

        if (!File.Exists(combinePath)) return null; //no information to load
        try
        {
            Debug.Log("LOADING VEGETABLES... ");

            if (File.Exists(combinePath))
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

                // Deserialize the JSON string into a Dictionary
                var loadedVegetables = JsonConvert.DeserializeObject<Dictionary<string, Vegetable>>(rawData);

                var debug = "";
                foreach(Vegetable v in loadedVegetables.Values)
                {
                    debug = debug + v.debug() + "\n";
                }
                Debug.Log("LOADED: " + loadedVegetables.Count+ "\n"+ debug);

                return loadedVegetables;
            }
            else
            {
                Debug.Log("No saved vegetable data found.");
                return null;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("ERROR ON: LoadVegetableObject() ->"+ combinePath + "\n" + e.StackTrace);
        }

        return null;
    }
}
