using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityEngine.VFX;
using FMODUnity;
using FMOD.Studio;
using static UnityEngine.Rendering.DebugUI;

public class PuzzleManager : MonoBehaviour
{
    [Header("Puzzle")]
    public UnityEvent onPuzzleCompleted;
    public List<PuzzlePoint> puzzlePoints;

    [HideInInspector]
    public bool hasInteract = false;
    [HideInInspector]
    public bool isOnPuzzle;
    [HideInInspector]
    public bool finish = false;

    [Header("Line")]
    public LineRenderer lineRenderer;
    public Material lineMaterial;
    [HideInInspector]
    public List<PuzzlePoint> currentPuzzlePointsList;

    [Header("Vars")]

    public GameObject player;
    public Ray ray;
    private bool hasFinised = false;
    private int currentIndex = -1;

    private List<PuzzlePoint> selfList = new List<PuzzlePoint>();

    
    public SoundEventManager soundEvent;

    public GameObject particlePrefab;

    public VisualEffect circle;

    //public SoundEmitterVariables musicController;
    private SoundEventManager soundEventManager;
    private GameObject emitterGameObject;
    private StudioEventEmitter emitter;
    private EventInstance eventInstance;

    public LayerMask puzzleLayer;
    private string puzzleLayerName = "Puzzle";
    void Start()
    {
        //PuzzleLayer
        puzzleLayer = LayerMask.NameToLayer(puzzleLayerName);

        PuzzleManager[] puzzleManagers = FindObjectsOfType<PuzzleManager>();

        PuzzlePoint[] puzzlePoints = FindObjectsOfType<PuzzlePoint>();

        foreach (PuzzleManager manager in puzzleManagers)
        {
            manager.gameObject.layer = puzzleLayer;
        }

        foreach (PuzzlePoint point in puzzlePoints)
        {
            point.gameObject.layer = puzzleLayer;
        }

        //PuzzleSetup
        soundEventManager = gameObject.GetComponent<SoundEventManager>();
        lineRenderer = GetComponent<LineRenderer>();
        ResetPuzzle();
        circle.Stop();

        //SoundEmitter
        emitterGameObject = GameObject.FindGameObjectWithTag("AudioManager");
        if (emitterGameObject != null)
        {
            emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
        }
        if (emitter != null)
        {
            eventInstance = emitter.EventInstance;
            print(emitter.gameObject);
        }

    }

    private void LateUpdate()
    {
        if (isOnPuzzle)
        {
            drawLine(ray);
        }
    }

    void Update()
    {
        if (currentIndex >= puzzlePoints.Count) return;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, 100f, puzzleLayer))
        {
            var hitPoint = hit.collider.GetComponent<PuzzlePoint>();
            if (hitPoint != null && !hitPoint.IsActivated && puzzlePoints.IndexOf(hitPoint) == currentIndex + 1)
            {
                hitPoint.ActivatePoint();
                currentIndex++;
                DrawLineTo(hitPoint.transform.position);

                if (currentIndex == puzzlePoints.Count - 1)
                {
                    Debug.Log("Puzzle Completo!");
                }
            }
        }
    }

    void drawLine(Ray ray)
    {
        if (!hasFinised)
        {
            lineRenderer.SetPosition(0, gameObject.transform.position);

            for (int i = 1; i < lineRenderer.positionCount - 1; i++)
            {
                int indexInList = i - 1;

                if (indexInList < currentPuzzlePointsList.Count)
                {
                    GameObject puzzlePointObject = currentPuzzlePointsList[indexInList].gameObject;
                    lineRenderer.SetPosition(i, puzzlePointObject.transform.position);
                }
            }

            int lastIndex = lineRenderer.positionCount - 1;
            if (!finish)
            {
                Vector3 endPoint = player.transform.position + ray.direction * 6f;
                lineRenderer.SetPosition(lastIndex, endPoint);
            }
            else
            {
                if (currentPuzzlePointsList.Count > 0 && lastIndex - 1 < currentPuzzlePointsList.Count)
                {
                    GameObject lastPointObject = currentPuzzlePointsList[lastIndex - 1].gameObject;
                    lineRenderer.SetPosition(lastIndex, lastPointObject.transform.position);
                }
                else
                {
                    lineRenderer.SetPosition(lastIndex, gameObject.transform.position);
                }
            }
        }
    }

    public void Finish()
    {
        selfList.Clear();

        foreach (var point in currentPuzzlePointsList)
        {
            selfList.Add(point);
        }

        if (selfList.Count > 0)
        {
            selfList.Add(selfList[0]);
        }

        StartCoroutine(AnimateBrightnessAndMove());
    }

    private void DrawLineTo(Vector3 position)
    {
        lineRenderer.positionCount = currentIndex + 2;
        lineRenderer.SetPosition(currentIndex + 1, position);
    }

    public void ResetPuzzle()
    {
        foreach (var point in puzzlePoints)
        {
            point.ResetPoint();
        }
        currentIndex = -1;
        lineRenderer.positionCount = 0;
    }

    public void InteractWithPoint(GameObject point)
    {
        var puzzlePoint = point.GetComponent<PuzzlePoint>();
        if (puzzlePoint != null && !puzzlePoint.IsActivated && puzzlePoints.IndexOf(puzzlePoint) == currentIndex + 1)
        {
            puzzlePoint.ActivatePoint();
            currentIndex++;
            DrawLineTo(puzzlePoint.transform.position);

            if (currentIndex == puzzlePoints.Count - 1)
            {
                Debug.Log("Puzzle Completo!");
            }
        }
    }

    public float duration = 4f;
    public float maxBrightness = 5f;

    public float musicFadeDuration = 10f;
    private bool MusicFadePlaying = false;
    IEnumerator AnimateBrightnessAndMove()
    {
        float startTime = Time.time;
        Color initialColor = lineMaterial.GetColor("_EmissionColor"); // Cor inicial de emissão

        /*
        if (MusicFadePlaying)
        {
            StopCoroutine(MusicFade(0));
            StartCoroutine(MusicFade(0));
        }
        else
        {
            StopCoroutine(MusicFade(1));
            StartCoroutine(MusicFade(1));
        }
        */

        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            float brightness;

            if (t < 0.5f)
            {
                brightness = Mathf.Lerp(2f, maxBrightness, t / 0.5f);
            }
            else
            {
                brightness = Mathf.Lerp(maxBrightness, 2f, (t - 0.5f) / 0.5f);
            }

            Color newColor = initialColor * brightness;
            lineMaterial.SetColor("_EmissionColor", newColor);

            yield return null;
        }

        lineMaterial.SetColor("_EmissionColor", initialColor);

        onPuzzleCompleted.Invoke();

        if(soundEventManager.GetComponent<AudioSource>() != null) 
        {
            soundEventManager.PlaySound(0, gameObject);
        }

        hasFinised = true;


        // Movimento
        Vector3 initialPosition = transform.position;
        float moveDuration = 1f; 

        float moveUpTime = 0.3f * moveDuration;
        float elapsedTime = 0f;
        if (circle != null)
        {
            circle.transform.SetParent(null);

            RaycastHit hit;
            if (Physics.Raycast(circle.transform.position, Vector3.down, out hit, Mathf.Infinity))
            {
                Vector3 groundPosition = hit.point;
                circle.transform.position = new Vector3(circle.transform.position.x, groundPosition.y + 0.25f, circle.transform.position.z);
            }
            else
            {
                Debug.LogWarning("Ground not found beneath the VFX.");
            }
        }

        while (elapsedTime < moveUpTime)
        {
            Vector3 newPosition = Vector3.Lerp(initialPosition, initialPosition + Vector3.up * 0.5f, elapsedTime / moveUpTime);
            Vector3 offset = newPosition - transform.position;
            transform.position = newPosition;

            UpdateLinePositions(offset);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if(circle != null)
        {
            circle.Play();

        }
        float moveDownTime = 2.5f * moveDuration;
        elapsedTime = 0f;
        Vector3 peakPosition = transform.position;
        while (elapsedTime < moveDownTime)
        {
            Vector3 newPosition = Vector3.Lerp(peakPosition, peakPosition + Vector3.down * 20f, elapsedTime / moveDownTime);
            Vector3 offset = newPosition - transform.position;
            transform.position = newPosition;

            UpdateLinePositions(offset);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(musicFadeDuration + 3f);
        Destroy(gameObject);
    }

    IEnumerator MusicFade(float inicialVolume)
    {
        MusicFadePlaying = true;
        eventInstance.setParameterByName("MasterFade", inicialVolume);

        if (inicialVolume > 0)
        {
            float startTime = Time.time;
            float volume;
            while (Time.time - startTime < 3)
            {
                float t = (Time.time - startTime) / 2;
                volume = Mathf.Lerp(1, 0, t / 3f);


                eventInstance.setParameterByName("MasterFade", volume);

                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(3);
        }


        yield return new WaitForSeconds(musicFadeDuration);

        float startTime2 = Time.time;
        float volume2;
        while (Time.time - startTime2 < 3)
        {
            float t = (Time.time - startTime2) / 2;
            volume2 = Mathf.Lerp(0, 1, t / 3f);


            eventInstance.setParameterByName("MasterFade", volume2);

            yield return null;
        }

        eventInstance.setParameterByName("MasterFade", 1);
        MusicFadePlaying = false;
    }

    private void UpdateLinePositions(Vector3 offset)
    {
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            lineRenderer.SetPosition(i, lineRenderer.GetPosition(i) + offset);
        }
    }
}