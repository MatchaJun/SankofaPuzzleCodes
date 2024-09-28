using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    public float rotationSpeed = 50f;
    public bool finalPuzzle = false;
    void Update()
    {
        if (!finalPuzzle)
        {
            Camera camera = Camera.main;

            Vector3 directionToCamera = transform.position - camera.transform.position;
            directionToCamera.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);
            Quaternion zRotation = Quaternion.Euler(0, 0, transform.eulerAngles.z);
            transform.rotation = targetRotation * zRotation;

            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime, Space.Self);
        }
        else
        {
            Camera camera = Camera.main;
            gameObject.transform.LookAt(camera.transform.position);
        }
    }
}
