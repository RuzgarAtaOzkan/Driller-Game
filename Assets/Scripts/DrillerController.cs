using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillerController : MonoBehaviour
{
    Rigidbody rb;
    Terrain terr;
    float speed = 3f;

    private void Start()
    {
        terr = GameObject.Find("Terrain").GetComponent<Terrain>();
        rb = GetComponent<Rigidbody>();
        PickRandomPosOnTerrain();
    }

    void Update()
    {
        MoveDriller(speed);
        ControlVelocity(0.0f, 0.0f, 0.0f);
    }

    private void PickRandomPosOnTerrain()
    {
        const float heightToPlace = 2.2f;

        float terrainXPos = terr.transform.position.x;
        float terrainZPos = terr.transform.position.z;

        float terrainXSize = terr.terrainData.size.x;
        float terrainZSize = terr.terrainData.size.z;

        float randomXPosForMineral = Random.Range(terrainXPos + 10f, terrainXPos + terrainXSize - 10f);
        float randomZPosForMineral = Random.Range(terrainZPos + 10f, terrainZPos + terrainZSize - 10f);

        Vector3 randomTerrainPos = new Vector3(randomXPosForMineral, heightToPlace, randomZPosForMineral);

        transform.position = randomTerrainPos;
    }

    public void ControlVelocity(float xVelocity, float yVelocity, float zVelocity)
    {
        Vector3 velocity = new Vector3(xVelocity, yVelocity, zVelocity);
        rb.velocity = velocity;
        rb.angularVelocity = velocity;
    }

    private void MoveDriller(float speed)
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 position = new Vector3(x, 0f, z);
        rb.MovePosition(rb.position + position * speed * Time.deltaTime);
        if (position.magnitude > 0)
        {
            Quaternion rotations = Quaternion.LookRotation(position, Vector3.up);
            transform.rotation = rotations;
        }
    }

    private Vector3 ShakeDriller(float xShakeMagnitude, float zShakeMagnitude)
    {
        float xMagnitude = Random.Range(-xShakeMagnitude, xShakeMagnitude);
        float zMagnitude = Random.Range(-zShakeMagnitude, zShakeMagnitude);
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
