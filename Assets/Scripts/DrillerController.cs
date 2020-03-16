using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillerController : MonoBehaviour
{
    float speed = 6f;
    List<GameObject> drillerHeads = new List<GameObject>();
    Rigidbody rb;
    Terrain terr;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        terr = GameObject.Find("Terrain").GetComponent<Terrain>();
        SpawnOnRandomPosOnTerrain(20f);
        StartCoroutine(UpdateAllDrillerHeads(2f));
    }

    void Update()
    {
        MoveDriller(speed);
        //ControlVelocity(0.0f, 0.0f, 0.0f);
        ControlHeight(2.2f);
        RotateAllDrillerHeads(8f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Mineral")
        {
            Destroy(collision.gameObject);
        }
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
        Vector3 shakePos = new Vector3(xMagnitude, transform.position.y, zMagnitude);
        return shakePos;
    }

    private IEnumerator UpdateAllDrillerHeads(float updateTime)
    {
        while (true)
        {
            FindAllDrillerHeads("Cylinder.001");
            yield return new WaitForSeconds(updateTime);
        }
    }

    private List<GameObject> FindAllDrillerHeads(string gameObjectName)
    {
        GameObject[] gameObjects = FindObjectsOfType<GameObject>();
        for (int i = 0; i < gameObjects.Length; i++)
        {
            if (gameObjects[i].name == gameObjectName && !drillerHeads.Contains(gameObjects[i])) 
            { 
                drillerHeads.Add(gameObjects[i]); 
            }

            if (gameObjects[i] == null) 
            { 
                drillerHeads.Remove(gameObjects[i]); 
            }
        }
        return drillerHeads;
    }

    private void RotateAllDrillerHeads(float rotateSpeed)
    {
        try
        {
            foreach (GameObject drillerHead in drillerHeads)
            {
                drillerHead.transform.Rotate(Vector3.up * rotateSpeed);
            }
        }
        catch
        {
            foreach (GameObject drillerHead in drillerHeads)
            {
                Debug.LogWarning("drillerHead: " + drillerHead + " is missing");
            }
        }
    }
}
