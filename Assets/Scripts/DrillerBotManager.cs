using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillerBotManager : MonoBehaviour
{
    TerrainDeformer terrainDeformer;

    private void Start()
    {
        terrainDeformer = FindObjectOfType<TerrainDeformer>();
        ProcessCoroutines();
    }

    private void Update()
    {
        ApplyPathfindingToAllDrillerPathfindings();
    }

    private void ProcessCoroutines()
    {
        StartCoroutine(UpdateDrillerPathfindingsInOtherScripts(2f));
        StartCoroutine(UpdateDrillerBotsCountAndApplyPathfinding(2f)); // this coroutine has to be started after minerals generated
    }

    public IEnumerator UpdateDrillerBotsCountAndApplyPathfinding(float updateTime)
    {
        while (true)
        {
            DrillerPathfinding[] drillerPathfindings = FindAllDrillerPathfindingsInScene();
            foreach (DrillerPathfinding drillerPathfinding in drillerPathfindings)
            {
                if (!drillerPathfinding.isCoroutineStarted)
                {
                    StartCoroutine(drillerPathfinding.PickRandomPosOrClosestMineral());
                }
            }
            yield return new WaitForSeconds(updateTime);
        }
    }

    public IEnumerator UpdateDrillerPathfindingsInOtherScripts(float updateTime)
    {
        while (true)
        {
            DrillerPathfinding[] drillerPathFindings = FindAllDrillerPathfindingsInScene();
            terrainDeformer.drillerPathfindings = drillerPathFindings;
            yield return new WaitForSeconds(updateTime);
        }
    }

    public DrillerPathfinding[] FindAllDrillerPathfindingsInScene()
    {
        DrillerPathfinding[] drillerPathfindings = FindObjectsOfType<DrillerPathfinding>();
        return drillerPathfindings;
    }

    private void ApplyPathfindingToAllDrillerPathfindings() // warning overwrite the enumerators on drillerPathfindings, not prefered
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            foreach (DrillerPathfinding drillerPathfinding in FindAllDrillerPathfindingsInScene())
            {
                StartCoroutine(drillerPathfinding.PickRandomPosOrClosestMineral());
            }
        }
    }

}
