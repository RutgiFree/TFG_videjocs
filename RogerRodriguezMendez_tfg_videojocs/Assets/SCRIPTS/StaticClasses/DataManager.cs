using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
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
                        new DNAinfo("D1[+G[---Z][+++Z]XL][+++K][----K][++++++K][-K]", 5),
                        new DNAinfo("D1[G[---Z][+++Z]XL][+++++K][-----K][++K][---K]", 10)
                    }
                },
                {
                    Rules.DNAnucleotides.GROW,
                    new DNAinfo[]
                    {
                        new DNAinfo("G", 6),
                        new DNAinfo("2C1C1", 10)
                    }
                },
                {
                    Rules.DNAnucleotides.AUX_1,
                    new DNAinfo[]
                    {
                        new DNAinfo("Z", 5),
                        new DNAinfo("2[+++++Q][--Q]WL", 6),
                        new DNAinfo("2[++Q][----Q]WL", 8),
                        new DNAinfo("2[++++Q][----Q]WL", 10),
                    }
                },
                {
                    Rules.DNAnucleotides.AUX_2,
                    new DNAinfo[]
                    {
                        new DNAinfo("X", 5),
                        new DNAinfo("G[----Z][++Z]Y", 6),
                        new DNAinfo("G[--Z][++++Z]Y", 8),
                        new DNAinfo("G[---Z][+++Z]Y", 10),
                    }
                },
                {
                    Rules.DNAnucleotides.AUX_3,
                    new DNAinfo[]
                    {
                        new DNAinfo("W", 5),
                        new DNAinfo("1[+++++Q][--Q]", 6),
                        new DNAinfo("1[++Q][----Q]", 8),
                        new DNAinfo("1[++++Q][----Q]", 10),
                    }
                },
                {
                    Rules.DNAnucleotides.AUX_4,
                    new DNAinfo[]
                    {
                        new DNAinfo("Y", 5),
                        new DNAinfo("G[----Z][++Z]G", 6),
                        new DNAinfo("G[--Z][++++Z]G", 8),
                        new DNAinfo("G[---Z][+++Z]G", 10),
                    }
                },
                {
                    Rules.DNAnucleotides.AUX_5,
                    new DNAinfo[]
                    {
                        new DNAinfo("Q", 5),
                        new DNAinfo("L", 10),
                    }
                },
                {
                    Rules.DNAnucleotides.AUX_6,
                    new DNAinfo[]
                    {
                        new DNAinfo("K", 5),
                        new DNAinfo("G[---Z][+++Z]XL", 10),
                    }
                },
            };


            Vegetable veg =
                new Vegetable(
                        "Carrot",
                        new Rules.states[] { Rules.states.GERMINATION, Rules.states.GROWING, Rules.states.DYING },
                        0, myDNA, 0.125f, 0.65f);
            auxData.Add(veg.name.ToUpper(), veg);

            //tomato
            myDNA = new Dictionary<Rules.DNAnucleotides, DNAinfo[]>
            {
                {
                    Rules.DNAnucleotides.NONE,
                    new DNAinfo[]
                    {
                        new DNAinfo("+GZ-GZX[CC2L[------W][++++++W]]", 10)
                    }
                },
                {
                    Rules.DNAnucleotides.GROW,
                    new DNAinfo[]
                    {
                        new DNAinfo("G", 3),
                        new DNAinfo("2", 6),
                        new DNAinfo("3", 10)
                    }
                },
                {
                    Rules.DNAnucleotides.AUX_1,
                    new DNAinfo[]
                    {
                        new DNAinfo("[+++++YC1L]2[-----YC1L]", 3),
                        new DNAinfo("[-----YC1L]2[+++++P]", 5),
                        new DNAinfo("[-----P]2[+++++YC1L]", 6),
                        new DNAinfo("[+++++YC1L]2[++++++P]", 8),
                        new DNAinfo("[+++++P]2[-----YC1L]", 10)
                    }
                },
                {
                    Rules.DNAnucleotides.AUX_2,
                    new DNAinfo[]
                    {
                        new DNAinfo("X", 3),
                        new DNAinfo("+GZ-K", 5),
                        new DNAinfo("+GZ-GZ+K", 10)
                    }
                },
                {
                    Rules.DNAnucleotides.AUX_3,
                    new DNAinfo[]
                    {
                        new DNAinfo("W", 5),
                        new DNAinfo("L", 10)
                    }
                },
                {
                    Rules.DNAnucleotides.AUX_4,
                    new DNAinfo[]
                    {
                        new DNAinfo("Y", 1),
                        new DNAinfo("2[------W][++++++W]Q", 5),
                        new DNAinfo("3[------W][++++++W]CCC2[++++++++W][------W]Q", 10)
                    }
                },
                {
                    Rules.DNAnucleotides.AUX_5,
                    new DNAinfo[]
                    {
                        new DNAinfo("Q", 1),
                        new DNAinfo("CCC1[++++++L][------L]", 5),
                        new DNAinfo("CCC2[++++++++L][------L]CCC1[++++++L][------L]", 10)
                    }
                },
                {
                    Rules.DNAnucleotides.AUX_6,
                    new DNAinfo[]
                    {
                        new DNAinfo("K", 3),
                        new DNAinfo("GZ", 5),
                        new DNAinfo("GZ+GZ", 8),
                        new DNAinfo("GZ-GZ", 10),
                    }
                },
                {
                    Rules.DNAnucleotides.AUX_7,
                    new DNAinfo[]
                    {
                        new DNAinfo("P", 3),
                        new DNAinfo("2[++++++T][------T]RCC1T", 5),
                        new DNAinfo("3[++++++T][------T]RCC1T", 10),
                    }
                },
                {
                    Rules.DNAnucleotides.AUX_8,
                    new DNAinfo[]
                    {
                        new DNAinfo("R", 3),
                        new DNAinfo("CCC1[++++++++T][------T]", 5),
                        new DNAinfo("CCC2[++++++++T][------T]CCC1[++++++++T][------T]", 10),
                    }
                },
                {
                    Rules.DNAnucleotides.AUX_9,
                    new DNAinfo[]
                    {
                        new DNAinfo("T", 5),
                        new DNAinfo("F", 10),
                    }
                },
                {
                    Rules.DNAnucleotides.FLOWER,
                    new DNAinfo[]
                    {
                        new DNAinfo("F", 5),
                        new DNAinfo("D", 10),
                    }
                }
            };
            veg =
                new Vegetable(
                        "Tomato",
                        new Rules.states[] { Rules.states.GERMINATION, Rules.states.GROWING, Rules.states.FLOWERING, Rules.states.RIPENING, Rules.states.DYING },
                        0, myDNA, 0.15f,1f);
            auxData.Add(veg.name.ToUpper(), veg);

            //pumpkin
            myDNA = new Dictionary<Rules.DNAnucleotides, DNAinfo[]>
            {
                {//G+[[N]-N]-F[-GN]+N //G[+N][-N]
                    Rules.DNAnucleotides.NONE,
                    new DNAinfo[]
                    {
                        new DNAinfo("G[++GZ[--------L]G][--GX[++++++++L]G]", 10)
                    }
                },
                {
                    Rules.DNAnucleotides.GROW_1,
                    new DNAinfo[]
                    {
                        new DNAinfo("1", 5),
                        new DNAinfo("2", 10)
                    }
                },
                {
                    Rules.DNAnucleotides.GROW_2,
                    new DNAinfo[]
                    {
                        new DNAinfo("2", 5),
                        new DNAinfo("3", 10)
                    }
                },
                {
                    Rules.DNAnucleotides.AUX_1,
                    new DNAinfo[]
                    {
                        new DNAinfo("Z", 5),
                        new DNAinfo("++G++G++G++G[--------L]W", 7),
                        new DNAinfo("++G[--------L]++G++G++G[--------L]W", 10)
                    }
                },
                {
                    Rules.DNAnucleotides.AUX_2,
                    new DNAinfo[]
                    {
                        new DNAinfo("X", 5),
                        new DNAinfo("--G--G--G--G[++++++++L]Y", 7),
                        new DNAinfo("--G[++++++++L]--G--G--G[++++++++L]Y", 10)
                    }
                },
                {
                    Rules.DNAnucleotides.AUX_3,
                    new DNAinfo[]
                    {
                        new DNAinfo("W", 5),
                        new DNAinfo("-1[Q]+1[Q]T", 10)
                    }
                },
                {
                    Rules.DNAnucleotides.AUX_4,
                    new DNAinfo[]
                    {
                        new DNAinfo("Y", 5),
                        new DNAinfo("+1[K]-1[K]V", 10)
                    }
                },
                {
                    Rules.DNAnucleotides.AUX_5,
                    new DNAinfo[]
                    {
                        new DNAinfo("Q", 1),
                        new DNAinfo("P", 2),
                        new DNAinfo("--------L", 10)
                    }
                },
                {
                    Rules.DNAnucleotides.AUX_6,
                    new DNAinfo[]
                    {
                        new DNAinfo("K", 1),
                        new DNAinfo("R", 2),
                        new DNAinfo("++++++++L", 10)
                    }
                },
                {
                    Rules.DNAnucleotides.AUX_7,
                    new DNAinfo[]
                    {
                        new DNAinfo("P", 6),
                        new DNAinfo("++++++F", 10),
                    }
                },
                {
                    Rules.DNAnucleotides.AUX_8,
                    new DNAinfo[]
                    {
                        new DNAinfo("R", 1),
                        new DNAinfo("------F", 10)
                    }
                },
                {
                    Rules.DNAnucleotides.AUX_9,
                    new DNAinfo[]
                    {
                        new DNAinfo("T", 1),
                        new DNAinfo("-1[Q]+1[Q]-1[P]+1", 5),
                        new DNAinfo("-1[Q]+1[P]-1[Q]+1[Q]-1", 7),
                        new DNAinfo("-1[Q]+1[Q]-1[Q]+1[P]-1[Q]+1", 10)
                    }
                },
                {
                    Rules.DNAnucleotides.AUX_10,
                    new DNAinfo[]
                    {
                        new DNAinfo("V", 1),
                        new DNAinfo("+1[K]-1[K]+1[R]-1", 5),
                        new DNAinfo("+1[K]-1[R]+1[K]-1[K]+1", 7),
                        new DNAinfo("+1[K]-1[K]+1[K]-1[R]+1[K]-1", 10)
                    }
                },
                {
                    Rules.DNAnucleotides.FLOWER,
                    new DNAinfo[]
                    {
                        new DNAinfo("F", 5),
                        new DNAinfo("D", 10)
                    }
                },
            };
            veg =
                new Vegetable(
                        "Pumpkin",
                        new Rules.states[] { Rules.states.GERMINATION, Rules.states.GROWING, Rules.states.FLOWERING, Rules.states.RIPENING, Rules.states.DYING },
                        0, myDNA, 0.25f, 1.5f);
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

                Debug.Log("LOADED: " + loadedVegetables.Count);

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
