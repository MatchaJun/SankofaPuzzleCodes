using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Raycast : MonoBehaviour
{
    public float gazeTimeRequired = 2f; 
    public float raycastDistance = 10f; //Distância máxima do raycast
    public Material glowing;
    public Material glowingFail;
    public Image image;

    private float gazeTimer = 0f;
    private float gazeTimerPuzzle = 0f;
    private GameObject currentTarget = null;
    public PuzzleManager currentPuzzleManager = null;
    public List<PuzzlePoint> currentPuzzlePoints = new List<PuzzlePoint>();
    public bool isOnPuzzle = false;
    private int currentIndex = -1;
    private LineRenderer lineRenderer;

    public SoundEventManager soundEvent;
    private Coroutine resetCoroutine = null;

    public LayerMask puzzleLayer;

    private void Start()
    {
        //lineRenderer.positionCount = 0;
    }

    private void LateUpdate()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (currentPuzzleManager != null)
        {
            currentPuzzleManager.currentPuzzlePointsList = currentPuzzlePoints;
            currentPuzzleManager.ray = ray;
            currentPuzzleManager.player = gameObject;
        }

        image.fillAmount = gazeTimer;
        // print(currentPuzzlePoints);
    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);

        // Visualiza o raycast para debug
        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, UnityEngine.Color.red);

        if (Physics.Raycast(ray, out hit, raycastDistance, puzzleLayer))
        {
            if (!isOnPuzzle)
            {
                if (hit.transform.CompareTag("InteractivePoint"))
                {
                    if (currentTarget != hit.transform.gameObject)
                    {
                        currentTarget = hit.transform.gameObject;
                        gazeTimer = 0f;
                    }

                    gazeTimer += Time.deltaTime;

                    // Verifica se o jogador olhou para o objeto pelo tempo necessário
                    if (gazeTimer >= gazeTimeRequired)
                    {
                        // Inicia a interação com o ponto
                        InteractWithPoint(hit.transform.gameObject);
                    }
                }
                else
                {
                    PuzzleManager puzzleManager = hit.collider.GetComponent<PuzzleManager>();
                    if (puzzleManager != null && !puzzleManager.hasInteract)
                    {
                        currentPuzzleManager = puzzleManager;                      

                        lineRenderer = currentPuzzleManager.GetComponent<LineRenderer>();
                        lineRenderer.positionCount = 2;
                        gazeTimer += Time.deltaTime;

                        if ((gazeTimer >= gazeTimeRequired || Input.GetMouseButtonDown(0)) && !currentPuzzleManager.hasInteract)
                        {
                            soundEvent.PlaySound(1, currentPuzzleManager.gameObject);
                            isOnPuzzle = true;
                            currentPuzzleManager.isOnPuzzle = true;

                            Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                            GameObject particleInstance = Instantiate(currentPuzzleManager.particlePrefab, currentPuzzleManager.transform.position, Quaternion.identity);
                            particleInstance.transform.rotation = Quaternion.Euler(-90, 0, 0);

                            currentPuzzleManager.hasInteract = true;
                            for(int i = 0; i < currentPuzzleManager.puzzlePoints.Count; i++)
                            {
                                Transform childTransform = currentPuzzleManager.puzzlePoints[i].transform.Find("PointEffect");

                                if (childTransform != null)
                                {
                                    PuzzlePointEffects puzzlePointeffect = childTransform.GetComponent<PuzzlePointEffects>();

                                    if (puzzlePointeffect != null)
                                    {
                                        puzzlePointeffect.isActive = true;
                                    }
                                }
                            }
                            for (int i = 0; i < 3; i++)
                            {
                                print(currentPuzzleManager.puzzlePoints[i]);
                            }
                        }
                    }
                }
            }
            else
            {                

                if (currentIndex != currentPuzzleManager.puzzlePoints.Count - 1)
                {
                    PuzzlePoint pz = hit.transform.gameObject.GetComponent<PuzzlePoint>();

                    if (hit.transform.CompareTag("PuzzlePoint") && !pz.IsActivated)
                    {
                        currentTarget = hit.transform.gameObject;

                        gazeTimerPuzzle += Time.deltaTime;

                        if (gazeTimerPuzzle >= 0.01f)
                        {
                            InteractWithPuzzlePoint(currentTarget);
                            currentIndex++;
                            if (currentIndex != currentPuzzleManager.puzzlePoints.Count - 1) {
                                if(!compareLists(currentPuzzleManager.puzzlePoints, currentPuzzlePoints))
                                {
                                    currentPuzzleManager.finish = true;
                                    Reset();
                                } 
                            }                         
                        }
                    }
                }
                else
                {
                    currentPuzzleManager.finish = true;

                    if(compareLists(currentPuzzleManager.puzzlePoints, currentPuzzlePoints))
                    {
                        Complete();
                    }
                    else
                    {                    
                        Reset();
                    }
                }
            }
        }
        else
        {
            currentTarget = null;
            gazeTimer = 0f;
            gazeTimerPuzzle = 0f;
        }
    }

    

    void InteractWithPoint(GameObject point)
    {
        if (currentPuzzleManager != null)
        {        
            currentPuzzleManager.InteractWithPoint(point);           
        }
        else
        {
            Debug.Log($"Interagiu com: {point.name}");
        }
    }

    void InteractWithPuzzlePoint(GameObject point)
    {

        PuzzlePoint px = point.GetComponent<PuzzlePoint>();
        soundEvent.PlaySound(1, point);
        currentPuzzlePoints.Add(px);

        Transform childTransform = point.transform.Find("PointEffect");

        if (childTransform != null)
        {
            PuzzlePointEffects puzzlePointeffect = childTransform.GetComponent<PuzzlePointEffects>();

            if (puzzlePointeffect != null)
            {
                puzzlePointeffect.isActive = false;
            }
        }
        px.ActivatePoint();
        print(currentPuzzlePoints);

        if (currentIndex != currentPuzzleManager.puzzlePoints.Count -1) { 
            lineRenderer.positionCount += 1; 
        }
    }

    static bool compareLists<T>(List<T> lista1, List<T> lista2)
    {
        if (lista1.Count != lista2.Count)
        {
            for (int i = 0; i < lista2.Count; i++)
            {
                if (!lista2[i].Equals(lista1[i]))
                {
                    break;
                }

                if (i == lista2.Count - 1)
                {
                    return true;
                }
            }

            for (int i = 0; i < lista2.Count; i++)
            {
                List<T> invertedLista1 = new List<T>(lista1);
                invertedLista1.Reverse();

                if (!lista2[i].Equals(invertedLista1[i]))
                {
                    break;
                }

                if (i == lista2.Count - 1)
                {
                    return true;
                }
            }

            return false;
        }

        for (int i = 0; i < lista1.Count; i++)
        {
            if (!lista1[i].Equals(lista2[i]))
            {
                break; 
            }
            if (i == lista1.Count - 1) 
            {
                return true; 
            }
        }

        List<T> invertedLista2 = new List<T>(lista2); 
        invertedLista2.Reverse(); 

        for (int i = 0; i < lista1.Count; i++)
        {
            if (!lista1[i].Equals(invertedLista2[i]))
            {
                return false; 
            }
        }

        return true;
    }

    private bool isRunning = false;

    private void Complete()
    {
        PuzzleList puzzleList = FindObjectOfType<PuzzleList>();
        if (puzzleList != null)
        {
            puzzleList.SpawnNextPuzzle();
        }
        soundEvent.PlaySound(3, currentPuzzleManager.gameObject);
        currentPuzzleManager.Finish();
        currentPuzzleManager = null;
        isOnPuzzle = false;
        currentIndex = -1;
        currentPuzzlePoints.Clear();
    }

    private void Reset()
    {
        resetCoroutine = StartCoroutine(ResetCoroutine());
    }

    IEnumerator ResetCoroutine()
    {
        if (!isRunning)
        {
            isOnPuzzle = false;
            isRunning = true;
            soundEvent.PlaySound(2, currentPuzzleManager.gameObject);
            lineRenderer.material = glowingFail;
            glowingFail.SetFloat("_CutoffHeight", 3);
            StartCoroutine(AnimateCutoff(glowingFail, 3));

            yield return new WaitForSeconds(3f);

            for (int i = 0; i < currentPuzzlePoints.Count; i++)
            {
                currentPuzzlePoints[i].ResetPoint();
            }

            for (int i = 0; i < currentPuzzlePoints.Count; i++)
            {
                Transform childTransform = currentPuzzlePoints[i].transform.Find("PointEffect");

                if (childTransform != null)
                {
                    PuzzlePointEffects puzzlePointeffect = childTransform.GetComponent<PuzzlePointEffects>();

                    if (puzzlePointeffect != null)
                    {
                        puzzlePointeffect.isActive = false;
                    }
                }
            }

            currentPuzzleManager.finish = false;
            currentIndex = -1;
            currentPuzzlePoints.Clear();
            
            currentPuzzleManager.isOnPuzzle = false;
            currentPuzzleManager.hasInteract = false;
            gazeTimer = 0f;
            currentPuzzleManager = null;

            lineRenderer.material = glowing;
            lineRenderer.positionCount = 0;
            lineRenderer.positionCount = 2;
            isRunning = false;

        }
    }

    IEnumerator AnimateCutoff(Material glowingFail, float duration)
    {
        float startTime = Time.time;
        float startCutoff = currentPuzzleManager.gameObject.transform.position.y + 5;
        float targetCutoff = currentPuzzleManager.gameObject.transform.position.y - 2; 

        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            float newCutoff = Mathf.Lerp(startCutoff, targetCutoff, t);
            glowingFail.SetFloat("_CutoffHeight", newCutoff);
            yield return null;
        }

        glowingFail.SetFloat("_CutoffHeight", targetCutoff);
    }
}