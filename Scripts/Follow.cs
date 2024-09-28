using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public GameObject objectToFollow;
    public bool Y = true;


    void Update()
    {
        if (objectToFollow != null)
        {
            if (!Y)
            {
                gameObject.transform.position = new Vector3(objectToFollow.transform.position.x, gameObject.transform.position.y, objectToFollow.transform.position.z);
            }
            else
            {
                gameObject.transform.position = objectToFollow.transform.position;
            }
        }
        
    }
}
