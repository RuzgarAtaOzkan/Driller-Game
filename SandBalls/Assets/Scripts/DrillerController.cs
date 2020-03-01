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
        transform.position = new Vector3(Random.Range(10f, terr.terrainData.size.x - 10f), 1.5f, Random.Range(10f, terr.terrainData.size.z - 10f));
        transform.rotation = Quaternion.Euler(90f, transform.rotation.y, transform.rotation.z);
    }

    void Update()
    {
        MoveDriller();
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
        if (collision.gameObject.name == "Mineral(Clone)")
        {
            Destroy(collision.gameObject);
        }
    }
}
