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
        StartCoroutine(UpdateDrillerBotsCountAndApplyPathfinding());
    }

    IEnumerator UpdateDrillerBotsCountAndApplyPathfinding()
    {
        while (true)
        {
            DrillerPathfinding[] drillerPathfindings = CountDrillerBots();
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
            DrillerPathfinding[] drillerPathFindings = CountDrillerBots();
            terrainDeformer.drillerPathfindings = drillerPathFindings;
            generateRandomMinerals.drillerPathfindings = drillerPathFindings;
            yield return new WaitForSeconds(1f);
        }
    }

    public DrillerPathfinding[] CountDrillerBots()
    {
        return FindObjectsOfType<DrillerPathfinding>();
    }

    public void ApplyPathfindToAllDrillerPathfindings()
    {
        foreach (DrillerPathfinding drillerPathfinding in CountDrillerBots()) 
        { 
            StartCoroutine(drillerPathfinding.PickRandomPosOrClosestMineral());
        }
    }
}
