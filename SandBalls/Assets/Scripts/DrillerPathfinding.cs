﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DrillerPathfinding : MonoBehaviour
{
    NavMeshAgent agent;
    GameObject mineralsParent; //warning, there has to be a gameobject calles MineralsParent
    Terrain terr; // warning, there has to be game object called Terrain in hierarchy

    TerrainDeformer terrainDeformer;

    float waitTime; //wait time before finding a new destination target, by dividing target distance with speed of agent
    
    void Start()
    {
        terrainDeformer = FindObjectOfType<TerrainDeformer>(); // todo will remove experimental
        terr = GameObject.Find("Terrain").GetComponent<Terrain>(); // todo might serialize it later
        agent = GetComponent<NavMeshAgent>();
        mineralsParent = GameObject.Find("MineralsParent");
    }

    public IEnumerator PickRandomPosOrClosestMineral()
    {
        while (true)
        {
            int randomTarget = UnityEngine.Random.Range(0, 2);
            if (randomTarget < 1f) { PickRandomPosOnTerrain(); }
            else if (randomTarget >= 1f) { PickClosestMineralOnTerrain(); }
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void PickRandomPosOnTerrain()
    {
        if (this != null)
        {
            float randomXPosOnTerrain = UnityEngine.Random.Range(terr.transform.position.x + 10f, terr.transform.position.x + (terr.terrainData.size.x - 10f));
            float randomZPosOnTerrain = UnityEngine.Random.Range(terr.transform.position.z + 10f, terr.transform.position.z + (terr.terrainData.size.z - 10f));
            Vector3 randomPos = new Vector3(randomXPosOnTerrain, transform.position.y, randomZPosOnTerrain);
            agent.SetDestination(randomPos);
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
            }
        }
    }

    public int FindMinValue(List<int> list)
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
        Vector3 drillerScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        Vector3 mineralScale = new Vector3(collision.transform.localScale.x, collision.transform.localScale.y, collision.transform.localScale.z);
        transform.localScale = new Vector3(drillerScale.x + (mineralScale.x / 2), drillerScale.y + (mineralScale.y / 2), drillerScale.z + (mineralScale.z / 2));
        terrainDeformer.inds = transform.localScale.x * 4f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Mineral")
        {
            //EatMineral(collision);
            Destroy(collision.gameObject);
        }
    }
}