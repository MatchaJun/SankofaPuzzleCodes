using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleManagerEffect : MonoBehaviour
{
    private Material material;
    public bool isActive = false;
    public float maxVisibilityDistance = 1f; 
    Camera camera;
    void Start()
    {
        camera = Camera.main;
        material = GetComponent<Renderer>().material;
        SetTransparency(0); 
    }

    void Update()
    {
        // Cria um raio da câmera para o objeto
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        RaycastHit hit;

        Raycast raycast = camera.GetComponent<Raycast>();
        isActive = raycast.isOnPuzzle;

        if (Physics.Raycast(ray, out hit) && !isActive)
        {
            if (hit.collider.gameObject == gameObject) 
            {
                float distanceToCenter = (hit.point - transform.position).magnitude;
                float visibility = Mathf.Clamp01(1 - (distanceToCenter / maxVisibilityDistance));
                SetTransparency(visibility);
            }
        }
        else
        {

            SetTransparency(0);
        }
    }

    void SetTransparency(float alpha)
    {
        Color color = material.color;
        color.a = alpha;
        material.color = color;
    }
}
