using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHint : MonoBehaviour
{
    public GameObject hint;

    private void Start()
    {
        hint.SetActive(false);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            hint.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            hint.SetActive(false);
        } 
    }
}
