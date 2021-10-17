using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts;

public class StepsGraph : MonoBehaviour
{
    [SerializeField] FBProfileManager profileManager;
    [SerializeField] LineChart line;
    void OnEnable()
    {
        profileManager.onStepsUpdate.AddListener(UpdateGraph);
    }

    // Update is called once per frame
    void UpdateGraph()
    {
        if(!(line is null))
        {
            line.RemoveData();
            line.AddSerie(SerieType.Line);
            foreach(var pair in profileManager.getSteps)
            {
                line.AddXAxisData(pair.Key);
                line.AddData(0, pair.Value);
            }
        }
    }
}
