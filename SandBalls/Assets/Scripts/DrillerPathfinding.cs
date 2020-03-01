using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DrillerPathfinding : MonoBehaviour
{
    NavMeshAgent agent;
    GameObject mineralsParent;
    Terrain terr; // warning, there has to be game object called terrain in hierarchy

    float waitTime; //wait time before finding a new destination target, by dividing target distance with speed of agent
    
    void Start()
    {
        terr = GameObject.Find("Terrain").GetComponent<Terrain>(); // todo might serialize it later
        agent = GetComponent<NavMeshAgent>();
        mineralsParent = GameObject.Find("MineralsParent");
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

    public IEnumerator FindClosestMineral()
    {
        int randomTarget;
        while (true)
        {
            randomTarget = UnityEngine.Random.Range(0, 2);
            if (randomTarget < 0.8f)
            {
                Vector3 randomPos = new Vector3(UnityEngine.Random.Range(terr.terrainData.size.x - (terr.terrainData.size.x - 10f), terr.terrainData.size.x - 10f), transform.position.y, UnityEngine.Random.Range(terr.terrainData.size.z - (terr.terrainData.size.z - 10f), terr.terrainData.size.z - 10f));
                agent.SetDestination(randomPos);
                waitTime = Vector3.Distance(transform.position, randomPos) / agent.speed;
                if (waitTime > 13f) { waitTime = 1f; }
            }
            else if (randomTarget >= 0.8f)
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
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Mineral(Clone)")
        {
            Destroy(collision.gameObject);
        }
    }
}
