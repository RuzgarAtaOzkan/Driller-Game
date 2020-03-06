using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillerController : MonoBehaviour
{
    Rigidbody rb;
    Terrain terr;

    private void Start()
    {
        terr = GameObject.Find("Terrain").GetComponent<Terrain>();
        rb = GetComponent<Rigidbody>();
        PickRandomPosOnTerrain();
    }

    void Update()
    {
        MoveDriller();
    }

    private void PickRandomPosOnTerrain()
    {
        float terrainXPos = terr.transform.position.x;
        float terrainZPos = terr.transform.position.z;
        float terrainXSize = terr.terrainData.size.x;
        float terrainZSize = terr.terrainData.size.z;
        float randomXPosForMineral = Random.Range(terrainXPos + 10f, terrainXPos + terrainXSize - 10f);
        float randomZPosForMineral = Random.Range(terrainZPos + 10f, terrainZPos + terrainZSize - 10f);
        Vector3 randomTerrainPos = new Vector3(randomXPosForMineral, 2.6f, randomZPosForMineral);
        transform.position = randomTerrainPos;
        transform.rotation = Quaternion.Euler(90f, transform.rotation.y, transform.rotation.z);
    }

    private void MoveDriller()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 position = new Vector3(x, 0f, z);
        rb.MovePosition(rb.position + position * 2f * Time.deltaTime);

        if (position.magnitude > 0)
        {
            Quaternion rotations = Quaternion.LookRotation(position, Vector3.up);
            Quaternion xRotation = Quaternion.Euler(90f, transform.rotation.y, transform.rotation.z);
            transform.rotation = rotations * xRotation;
        }
    }

    private Vector3 ShakeDriller()
    {
        float xMagnitude = Random.Range(-1f, 1f);
        float zMagnitude = Random.Range(-1f, 1f);
        Vector3 shakePos = new Vector3(xMagnitude, 0f, zMagnitude);
        return shakePos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Mineral")
        {
            Destroy(collision.gameObject);
        }
    }
}
