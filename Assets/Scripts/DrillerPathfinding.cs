using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DrillerPathfinding : MonoBehaviour
{
    float waitTime; //wait time before finding a new destination target, by dividing target distance with speed of agent
    [HideInInspector] public bool isCoroutineStarted = false; // check if coroutine started to avoid overwrite the enumerator
    NavMeshAgent agent;

    DrillerController drillerController;
    TerrainDeformer terrainDeformer;
    GameObject mineralsParent;
    Terrain terr; // warning, there has to be game object called Terrain in hierarchy
    
    void Start()
    {
        terrainDeformer = FindObjectOfType<TerrainDeformer>();
        drillerController = FindObjectOfType<DrillerController>(); // todo will remove experimental
        terr = GameObject.Find("Terrain").GetComponent<Terrain>(); // todo might serialize it later
        agent = GetComponent<NavMeshAgent>();
        mineralsParent = GameObject.Find("MineralsParent"); // warning, there has to be a gameobject calles MineralsParent
        SpawnOnRandomPosOnTerrain(20f); 
    }

    private void Update()
    {
        //drillerController.ControlVelocity(0f, 0f, 0f);
        drillerController.ControlHeight(2.2f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Mineral")
        {
            //EatMineral(collision);
            Destroy(collision.gameObject);
        }
    }

    private void SpawnOnRandomPosOnTerrain(float offset)
    {
        float terrainXPos = terr.transform.position.x;
        float terrainZPos = terr.transform.position.z;
        float terrainXSize = terr.terrainData.size.x;
        float terrainZSize = terr.terrainData.size.z;
        float randomXPosForMineral = UnityEngine.Random.Range(terrainXPos + offset, terrainXPos + terrainXSize - offset);
        float randomZPosForMineral = UnityEngine.Random.Range(terrainZPos + offset, terrainZPos + terrainZSize - offset);
        Vector3 randomTerrainPos = new Vector3(randomXPosForMineral, transform.position.y, randomZPosForMineral);
        transform.position = randomTerrainPos;
    }

    public IEnumerator PickRandomPosOrClosestMineral() // Main pathfinding algorithm of the driller bot, call this function when need it
    {
        isCoroutineStarted = true;
        while (true)
        {
            int randomTarget = UnityEngine.Random.Range(0, 2);
            if (randomTarget < 1f) { PickRandomPosOnTerrain(20f); }
            else if (randomTarget >= 1f) { PickClosestMineralOnTerrain(); }
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void PickRandomPosOnTerrain(float offset)
    {
        if (this != null)
        {
            float terrainXPos = terr.transform.position.x;
            float terrainZPos = terr.transform.position.z;
            float terrainXSize = terr.terrainData.size.x;
            float terrainZSize = terr.terrainData.size.z;
            float randomXPosOnTerrain = UnityEngine.Random.Range(terrainXPos + offset, terrainXPos + (terrainXSize - offset));
            float randomZPosOnTerrain = UnityEngine.Random.Range(terrainZPos + offset, terrainZPos + (terrainZSize - offset));
            Vector3 randomPos = new Vector3(randomXPosOnTerrain, transform.position.y, randomZPosOnTerrain);
            agent.SetDestination(randomPos);
            Debug.DrawLine(transform.position, randomPos, Color.red, waitTime);
            waitTime = Vector3.Distance(transform.position, randomPos) / agent.speed;
            if (waitTime > 10f) { waitTime = 1f; }
        }
    }

    private void PickClosestMineralOnTerrain()
    {
        Transform[] minerals = mineralsParent.GetComponentsInChildren<Transform>();
        List<int> values = new List<int>();
        int minValue = int.MaxValue;
        foreach (Transform mineral in minerals)
        {
            float distanceToMineral = Vector3.Distance(transform.position, mineral.position);
            int convertedDistance = Mathf.RoundToInt(distanceToMineral);
            values.Add(convertedDistance);
            int finalMinValue = FindMinValue(values);
            if (finalMinValue < minValue)
            {
                minValue = finalMinValue;
                waitTime = Vector3.Distance(transform.position, mineral.position) / agent.speed;
                agent.SetDestination(mineral.position);
                Debug.DrawLine(transform.position, mineral.position, Color.red, waitTime);
            }
        }
    }

    private int FindMinValue(List<int> list)
    {
        if (list.Count == 0) { throw new InvalidOperationException("Empty list"); }
        int minValue = int.MaxValue;
        foreach (int item in list)
        {
            int value = item;
            if (value < minValue) { minValue = value; }
        }
        return minValue;
    }

    private void EatMineral(Collision collision)
    {
        if (collision.gameObject.tag == "Mineral")
        {
            Vector3 drillerScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
            Vector3 mineralScale = new Vector3(collision.transform.localScale.x, collision.transform.localScale.y, collision.transform.localScale.z);
            transform.localScale = new Vector3(drillerScale.x + (mineralScale.x / 2), drillerScale.y + (mineralScale.y / 2), drillerScale.z + (mineralScale.z / 2));
            terrainDeformer.inds = transform.localScale.x * 4f;
        }
    }

}
