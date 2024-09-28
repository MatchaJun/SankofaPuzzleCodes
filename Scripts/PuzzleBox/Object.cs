using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Object : MonoBehaviour
{
    [HideInInspector]
    public bool canBeHolded = true;
    public UnityEvent onObjectCorrect;
    [HideInInspector]
    public bool colliding;

    public void Finish()
    {
        onObjectCorrect.Invoke();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Objective")
        {
            colliding = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Objective")
        {
            colliding = false;
        }
    }
}
