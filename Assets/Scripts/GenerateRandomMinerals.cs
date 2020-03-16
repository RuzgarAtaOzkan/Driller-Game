using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateRandomMinerals : MonoBehaviour
{
    int mineralAmount = 60;
    Terrain terr;
    GameObject mineralsParent;

    [SerializeField] GameObject mineral;
    [SerializeField] Material[] materials;
    
    void Start()
    {
        terr = GameObject.Find("Terrain").GetComponent<Terrain>();
        mineralsParent = GameObject.Find("MineralsParent");
        GenerateMinerals(mineralAmount, 10f);
        ProcessCoroutines();
    }

    public void GenerateMinerals(int amountOfMineralToSpawn, float offset)
    {
        for (int i = 0; i < amountOfMineralToSpawn; i++)
        {
            float terrainXPos = terr.transform.position.x;
            float terrainZPos = terr.transform.position.z;

            float terrainXSize = terr.terrainData.size.x;
            float terrainZSize = terr.terrainData.size.z;

            float randomXPosForMineral = Random.Range(terrainXPos + offset, terrainXPos + terrainXSize - offset);
            float randomZPosForMineral = Random.Range(terrainZPos + offset, terrainZPos + terrainZSize - offset);

            Vector3 randomTerrainPos = new Vector3(randomXPosForMineral, 2.6f, randomZPosForMineral);

            GameObject instantiatedMineral = Instantiate(mineral, randomTerrainPos, Quaternion.identity);

            PickRandomMineralColor(instantiatedMineral);

            instantiatedMineral.transform.SetParent(mineralsParent.transform);
        }
    }

    private void PickRandomMineralColor(GameObject instantiatedMineral)
    {
        int randomNumber = Random.Range(0, materials.Length);
        Material[] materialsToPlace = new Material[instantiatedMineral.GetComponent<MeshRenderer>().materials.Length];
        Material randomMaterial = materials[randomNumber];
        for (int j = 0; j < materialsToPlace.Length; j++) 
        {
            materialsToPlace[j] = randomMaterial; 
        }
        instantiatedMineral.GetComponent<MeshRenderer>().materials = materialsToPlace;
    }

    private IEnumerator CheckMineralAmount(int mineralEdgeAmount, float updateTime)
    {
        while (true)
        {
            GameObject[] mineralAmountInHierarchy = GameObject.FindGameObjectsWithTag("Mineral");
            if (mineralAmountInHierarchy.Length <= mineralEdgeAmount)
            {
                GenerateMinerals(mineralAmount, 10f);
            }
            yield return new WaitForSeconds(updateTime);
        }
    }

    private void ProcessCoroutines()
    {
        StartCoroutine(CheckMineralAmount(6, 2f)); // if its below 6 generate random minerals
    }
}
