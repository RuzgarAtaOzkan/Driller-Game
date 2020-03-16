using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillerController : MonoBehaviour
{
    Rigidbody rb;
    Terrain terr;
    float speed = 6f;

    private void Start()
    {
        terr = GameObject.Find("Terrain").GetComponent<Terrain>();
        rb = GetComponent<Rigidbody>();
        SpawnOnRandomPosOnTerrain(20f);
    }

    void Update()
    {
        MoveDriller(speed);
        //ControlVelocity(0.0f, 0.0f, 0.0f);
        ControlHeight(2.2f);
    }

    public void SpawnOnRandomPosOnTerrain(float offset)
    {
        float terrainXPos = terr.transform.position.x;
        float terrainZPos = terr.transform.position.z;
        float terrainXSize = terr.terrainData.size.x;
        float terrainZSize = terr.terrainData.size.z;
        float randomXPosForMineral = Random.Range(terrainXPos + offset, terrainXPos + terrainXSize - offset);
        float randomZPosForMineral = Random.Range(terrainZPos + offset, terrainZPos + terrainZSize - offset);
        Vector3 randomTerrainPos = new Vector3(randomXPosForMineral, transform.position.y, randomZPosForMineral);
        transform.position = randomTerrainPos;
    }

    public Vector3 ControlVelocity(float xVelocity, float yVelocity, float zVelocity)
    {
        Vector3 velocity = new Vector3(xVelocity, yVelocity, zVelocity);
        rb.velocity = velocity;
        rb.angularVelocity = velocity;
        return velocity;
    }

    public float ControlHeight(float yPosition)
    {
        float xPos = transform.position.x;
        float yPos = yPosition;
        float zPos = transform.position.z;
        Vector3 position = new Vector3(xPos, yPos, zPos);
        transform.position = position;
        return yPosition;
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
