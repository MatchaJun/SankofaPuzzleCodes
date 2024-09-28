using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Griot : MonoBehaviour
{
    public float amplitude = 0.5f; 
    public float frequency = 1f; 

    private Vector3 startPos; 

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float newY = startPos.y + amplitude * Mathf.Sin(Time.time * frequency * 2 * Mathf.PI);

        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}
