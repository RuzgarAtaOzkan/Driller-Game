using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    TerrainDeformer terrainDeformer;
    Transform targetDriller;
    public Vector3 offset = new Vector3(0f, 25f, 0f);
    
    public void Start()
    {
        terrainDeformer = FindObjectOfType<TerrainDeformer>();
        targetDriller = GameObject.Find("Driller").transform;
        Camera.main.transform.position = targetDriller.position + offset;
        offset.y = terrainDeformer.inds - (terrainDeformer.inds / 6f);
    }

    void Update() 
    { 
        FollowCenterPosition(); 
    }

    private void FollowCenterPosition()
    {
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetDriller.position + offset, Time.deltaTime);
    }

    public float IncreaseCamFov()
    {
        offset.y = terrainDeformer.inds - (terrainDeformer.inds / 3f);
        return offset.y;
    }
}

