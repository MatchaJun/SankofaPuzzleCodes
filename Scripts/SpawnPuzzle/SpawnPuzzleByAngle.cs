using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPuzzleByAngle : MonoBehaviour
{
    [Header("Puzzle")]
    public GameObject puzzleToSpawn;
    public GameObject puzzleDrawing;
    public string puzzleDrawingTrigger;
    [Header("Position")]
    public Transform targetPosition;
    public float positionTolerance = 1.0f;
    public float rotationTolerance = 5.0f;

    private Camera playerCamera;
    private SoundEventManager soundEvent;
    private bool isSoundPlaying = false;

    void Start()
    {
        playerCamera = Camera.main;
        soundEvent = GetComponent<SoundEventManager>();
    }

    void Update()
    {
        if (CorrectPosition() && CorrectAngle())
        {
            OnPlayerInCorrectPositionAndAngle();
        }
    }

    bool CorrectPosition()
    {
        float distance = Vector3.Distance(playerCamera.transform.position, targetPosition.position);
        return distance <= positionTolerance;
    }

    bool CorrectAngle()
    {
        Vector3 directionToPuzzleDrawing = puzzleDrawing.transform.position - playerCamera.transform.position;
        float angle = Vector3.Angle(playerCamera.transform.forward, directionToPuzzleDrawing);
        return angle <= rotationTolerance;
    }

    void OnPlayerInCorrectPositionAndAngle()
    {
        puzzleToSpawn.SetActive(true);

        Transform child = puzzleToSpawn.transform.Find("SpawnPuzzleVFX");
        child.gameObject.SetActive(true);

        RaycastHit hit;
        if (Physics.Raycast(child.position, Vector3.down, out hit, Mathf.Infinity))
        {
            Vector3 groundPosition = hit.point;
            child.transform.position = new Vector3(child.transform.position.x, groundPosition.y + 0.25f, child.transform.position.z);
        }
        else
        {
            Debug.LogWarning("Ground not found beneath the VFX.");
        }

        if (puzzleDrawing != null && !string.IsNullOrEmpty(puzzleDrawingTrigger))
        {
            puzzleDrawing.GetComponent<Animator>().SetTrigger(puzzleDrawingTrigger);
        }

        if (soundEvent != null && !isSoundPlaying)
        {
            soundEvent.PlaySound(0, gameObject);
            isSoundPlaying = true;
        }

        Destroy(gameObject, 3);
    }
}