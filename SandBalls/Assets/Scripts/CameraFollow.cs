using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform targetDriller;
    [SerializeField] Vector3 offset = new Vector3(0f, 25f, 0f);

    void Update() { FollowCenterPosition(); }

    private void FollowCenterPosition()
    {
        transform.position = targetDriller.position + offset;
    }
}
