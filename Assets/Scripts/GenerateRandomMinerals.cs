﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateRandomMinerals : MonoBehaviour
{
    int mineralAmount = 60;
    Terrain terr;
    GameObject mineralsParent;
    DrillerBotManager drillerBotManager;

    public DrillerPathfinding[] drillerPathfindings;

    [SerializeField] GameObject mineral;
    [SerializeField] Material[] materials;
    
    void Start()
    {
        drillerBotManager = FindObjectOfType<DrillerBotManager>();
        terr = GameObject.Find("Terrain").GetComponent<Terrain>();
        mineralsParent = GameObject.Find("MineralsParent");
        GenerateMinerals(mineralAmount);
        ProcessCoroutines();
    }

    public void GenerateMinerals(int amountOfMineralToSpawn)
    {
        for (int i = 0; i < amountOfMineralToSpawn; i++)
        {
            float terrainXPos = terr.transform.position.x;
            float terrainZPos = terr.transform.position.z;
            float terrainXSize = terr.terrainData.size.x;
            float terrainZSize = terr.terrainData.size.z;
            float randomXPosForMineral = Random.Range(terrainXPos + 10f, terrainXPos + terrainXSize - 10f);
            float randomZPosForMineral = Random.Range(terrainZPos + 10f, terrainZPos + terrainZSize - 10f);
            Vector3 randomTerrainPos = new Vector3(randomXPosForMineral, 2.6f, randomZPosForMineral);
            GameObject instantiatedMineral = Instantiate(mineral, randomTerrainPos, Quaternion.identity);
            PickRandomMineralColor(instantiatedMineral);
            instantiatedMineral.transform.SetParent(mineralsParent.transform);
        }
    }

    private void PickRandomMineralColor(GameObject instantiatedMineral)
    {
        Material[] materialsToPlace = new Material[instantiatedMineral.GetComponent<MeshRenderer>().materials.Length];
        int ramdonNumber = Random.Range(0, materials.Length);
        Material randomMaterial = materials[ramdonNumber];
        for (int j = 0; j < materialsToPlace.Length; j++) { materialsToPlace[j] = randomMaterial; }
        instantiatedMineral.GetComponent<MeshRenderer>().materials = materialsToPlace;
    }

    private IEnumerator CheckMineralAmount(int mineralEdgeAmount)
    {
        while (true)
        {
            GameObject[] mineralAmountInHierarchy = GameObject.FindGameObjectsWithTag("Mineral");
            if (mineralAmountInHierarchy.Length <= mineralEdgeAmount)
            {
                GenerateMinerals(mineralAmount);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private void ProcessCoroutines()
    {
        StartCoroutine(CheckMineralAmount(6)); // if its below 6 generate random minerals
        //StartCoroutine(drillerBotManager.UpdateDrillerBotsCountAndApplyPathfinding()); // we are already updating pathfinding in drillerBotManager
    }
}