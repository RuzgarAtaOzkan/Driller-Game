using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillerBotManager : MonoBehaviour
{
    TerrainDeformer terrainDeformer;
    GenerateRandomMinerals generateRandomMinerals;

    private void Start()
    {
        terrainDeformer = FindObjectOfType<TerrainDeformer>();
        generateRandomMinerals = FindObjectOfType<GenerateRandomMinerals>();
        ProcessCoroutines();
    }

    private void ProcessCoroutines()
    {
        StartCoroutine(KeepTrackOfDrillerBotsInHierarchy());
        //StartCoroutine(UpdateDrillerBotsCountAndApplyPathfinding()); // this coroutine has to be started after minerals generated
    }

    public IEnumerator UpdateDrillerBotsCountAndApplyPathfinding()
    {
        while (true)
        {
            DrillerPathfinding[] drillerPathfindings = FindAllDrillerpathfindingsInScene();
            foreach (DrillerPathfinding drillerPathfinding in drillerPathfindings)
            {
                if (!drillerPathfinding.isCoroutineStarted)
                {
                    StartCoroutine(drillerPathfinding.PickRandomPosOrClosestMineral());
                }
            }
            yield return new WaitForSeconds(2f);
        }
    }

    public IEnumerator KeepTrackOfDrillerBotsInHierarchy()
    {
        while (true)
        {
            DrillerPathfinding[] drillerPathFindings = FindAllDrillerpathfindingsInScene();
            terrainDeformer.drillerPathfindings = drillerPathFindings;
            generateRandomMinerals.drillerPathfindings = drillerPathFindings;
            yield return new WaitForSeconds(1f);
        }
    }

    public DrillerPathfinding[] FindAllDrillerpathfindingsInScene()
    {
        DrillerPathfinding[] drillerPathfindings = FindObjectsOfType<DrillerPathfinding>();
        return drillerPathfindings;
    }

    public void ApplyPathfindToAllDrillerPathfindings()
    {
        foreach (DrillerPathfinding drillerPathfinding in FindAllDrillerpathfindingsInScene()) 
        {
            if (!drillerPathfinding.isCoroutineStarted)
            {
                StartCoroutine(drillerPathfinding.PickRandomPosOrClosestMineral());
            }
        }
    }
}
