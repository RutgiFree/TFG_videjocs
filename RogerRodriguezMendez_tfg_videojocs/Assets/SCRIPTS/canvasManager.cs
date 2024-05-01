using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class canvasManager : MonoBehaviour
{
    [SerializeField] GameObject vDirector;
    [SerializeField] GameObject spawnPoint;
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Dropdown dropdown;
    [SerializeField] Button pasTimeButton;
    [SerializeField] Button resetButton;
    [SerializeField] CameraManager myCameraScript;

    GameObject director;
    Director directorScript;

    void Awake()
    {
        dropdown.onValueChanged.AddListener(delegate {
            resetDirector();
            pasTimeButton.transform.gameObject.SetActive(true);
            resetButton.transform.gameObject.SetActive(true);
        });

        pasTimeButton.onClick.AddListener(delegate {
            title.text = "Iteracions: "+ directorScript.pasTime();
        });
        pasTimeButton.transform.gameObject.SetActive(false);

        resetButton.onClick.AddListener(delegate {
            resetDirector();
        });
        resetButton.transform.gameObject.SetActive(false);
    }
    void Start()
    {
        List<TMP_Dropdown.OptionData> optionDataList = new List<TMP_Dropdown.OptionData>();
        foreach (string vegetableName in DataManager.vegetablesNames)  optionDataList.Add(new TMP_Dropdown.OptionData(vegetableName)); 
        dropdown.options = optionDataList;

        title.text = "Benvinguts! :)";
    }


    private void resetDirector()
    {
        if (director)
            director.GetComponent<Director>().deleteAll();

        director = Instantiate(vDirector, spawnPoint.transform.position, spawnPoint.transform.rotation);
        directorScript = director.GetComponent<Director>();
        directorScript.setVegetable(DataManager.vegetablesNames[dropdown.value]);
        title.text = "Iteracions: " + directorScript.pasTime();
        myCameraScript.resetSize();
    }


}
