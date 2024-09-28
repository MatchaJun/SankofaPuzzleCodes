using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHolder : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] float maxGrabDistance = 10f, throwForce = 20f, moveForce = 50f, maxVelocity = 5f;
    [SerializeField] Transform objectHolder;

    Rigidbody grabbedRB;

    void Update()
    {

        /*
        if (Input.GetMouseButtonDown(0) && grabbedRB)
        {
            grabbedRB.AddForce(cam.transform.forward * throwForce, ForceMode.VelocityChange);
            grabbedRB = null;
        }
        */

        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));

            if (Physics.Raycast(ray, out hit, maxGrabDistance))
            {
                Object obj = hit.collider.GetComponent<Object>();

                if (obj != null && obj.canBeHolded) 
                {
                    if (grabbedRB != null)
                    {
                        if (obj.colliding)
                        {
                            obj.Finish();
                            obj.canBeHolded = false;
                        }
                        grabbedRB = null;
                    }
                    else
                    {
                        grabbedRB = hit.collider.GetComponent<Rigidbody>();
                    }
                }
            }      
        }
    }

    void FixedUpdate()
    {
        if (grabbedRB)
        {
            Vector3 directionToTarget = objectHolder.position - grabbedRB.position;
            float distance = directionToTarget.magnitude;

            float stopDistance = 0.4f;

            if (distance > stopDistance)
            {
                Vector3 velocityChange = directionToTarget.normalized * moveForce * Time.fixedDeltaTime;
                grabbedRB.velocity = Vector3.ClampMagnitude(grabbedRB.velocity + velocityChange, maxVelocity);

                if (grabbedRB.velocity.magnitude > maxVelocity)
                {
                    grabbedRB.velocity = grabbedRB.velocity.normalized * maxVelocity;
                }
            }
            else
            {
                grabbedRB.velocity = Vector3.zero;
                grabbedRB.angularVelocity = Vector3.zero;
            }
        }
    }
}