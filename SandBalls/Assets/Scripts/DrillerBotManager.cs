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
        StartCoroutine(KeepTrackOfDrillerBotsInHierarchy());
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
}
