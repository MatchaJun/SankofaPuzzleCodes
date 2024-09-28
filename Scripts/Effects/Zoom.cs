using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoom : MonoBehaviour
{
    public bool canZoom;

    public CinemachineVirtualCamera cam;
    public float zoomedFov = 20; 
    public float defaultFov = 40;
    public float zoomDuration = 2; 

    private Coroutine currentZoomCoroutine = null;
    private float targetFov;

    void Start()
    {
        targetFov = defaultFov; 
    }

    void Update()
    {
        if (Input.GetMouseButton(1) && targetFov != zoomedFov) 
        {
            targetFov = zoomedFov;
            StartZoomCoroutine(targetFov, zoomDuration);
        }
        else if (!Input.GetMouseButton(1) && targetFov != defaultFov) 
        {
            targetFov = defaultFov;
            StartZoomCoroutine(targetFov, zoomDuration / 2);
        }
    }

    void StartZoomCoroutine(float targetFOV, float duration)
    {
        if (currentZoomCoroutine != null)
        {
            StopCoroutine(currentZoomCoroutine);
        }

        currentZoomCoroutine = StartCoroutine(ChangeFOV(targetFOV, duration));
    }

    IEnumerator ChangeFOV(float targetFOV, float duration)
    {
        float startFOV = cam.m_Lens.FieldOfView;
        float time = 0;

        while (time < duration)
        {
            cam.m_Lens.FieldOfView = Mathf.Lerp(startFOV, targetFOV, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        cam.m_Lens.FieldOfView = targetFOV; 
    }
}
