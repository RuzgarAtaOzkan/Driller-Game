using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform targetDriller;
    [SerializeField] Vector3 offset = new Vector3(0f, 25f, 0f);

    private void Start()
    {
        targetDriller = GameObject.Find("Driller").transform;
        Camera.main.transform.position = targetDriller.position + offset;
    }

    void Update() 
    { 
        FollowCenterPosition(); 
    }

    private void FollowCenterPosition()
    {
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetDriller.position + offset, Time.deltaTime);
    }
}
