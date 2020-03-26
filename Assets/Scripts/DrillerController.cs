using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillerController : MonoBehaviour
{
    TerrainDeformer terrainDeformer;
    CameraFollow cameraFollow;

    List<GameObject> drillerHeads = new List<GameObject>();
    Rigidbody rb;
    Terrain terr;

    float speed = 4f;

    private void Start()
    {
        cameraFollow = FindObjectOfType<CameraFollow>();
        terrainDeformer = FindObjectOfType<TerrainDeformer>();
        rb = GetComponent<Rigidbody>();
        terr = GameObject.Find("Terrain").GetComponent<Terrain>();
        SpawnOnRandomPosOnTerrain(20f);
        StartCoroutine(UpdateAllDrillerHeads(2f));
        cameraFollow.Start();
    }

    void Update()
    {
        MoveDriller(speed);
        ControlHeight(1.08f);
        ControlVelocity(0f, 0f, 0f);
        RotateAllDrillerHeads(9f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Mineral" && collision.gameObject.name != "Driller Bot")
        {
            EatMineral(collision, 25f);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.name == "Driller Bot")
        {
            Debug.Log("driller collided");
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
        Vector3 position = new Vector3(x, transform.position.y, z);
        rb.MovePosition(rb.position + position * speed * Time.deltaTime);
        if (position.magnitude > 0)
        {
            Quaternion rotations = Quaternion.LookRotation(position, Vector3.up);
            transform.rotation = rotations;
        }
    }

    private Vector3 ShakeDriller(float xShakeMagnitude, float zShakeMagnitude)
    {
        float defaultXValue = xShakeMagnitude - xShakeMagnitude;
        float defaultZValue = zShakeMagnitude - zShakeMagnitude;
        float randomXValue = UnityEngine.Random.Range(-xShakeMagnitude, xShakeMagnitude);
        float randomZValue = UnityEngine.Random.Range(-zShakeMagnitude, zShakeMagnitude);
        float lerpedXValue = Mathf.Lerp(defaultXValue, randomXValue, Time.deltaTime);
        float lerpedZValue = Mathf.Lerp(defaultZValue, randomZValue, Time.deltaTime);
        Vector3 shakePos = new Vector3(lerpedXValue, transform.position.y, lerpedZValue);
        return shakePos;
    }

    public Vector3 EatMineral(Collision collision, float mineralDivideValue) // second parameter determines which should we divide the driller scale width
    {
        if (collision.gameObject.tag == "Mineral" && collision.gameObject.name != "Driller Bot")
        {
            Vector3 drillerScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
            Vector3 mineralScale = new Vector3(collision.transform.localScale.x, collision.transform.localScale.y, collision.transform.localScale.z);
            Vector3 newScaleToApply = new Vector3(drillerScale.x + (mineralScale.x / mineralDivideValue), transform.localScale.y, drillerScale.z + (mineralScale.z / mineralDivideValue));
            terrainDeformer.inds += 1.5f;
            cameraFollow.IncreaseCamFov();
            transform.localScale = newScaleToApply;
        }
        return transform.localScale;
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

            if (gameObjects[i] == null && gameObjects[i].name != gameObjectName) // clear drillerHeads gameObjects list 
            { 
                drillerHeads.Remove(gameObjects[i]); 
            }
        }
        Debug.Log(drillerHeads);
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
