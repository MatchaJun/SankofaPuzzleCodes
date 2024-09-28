using FMODUnity;
using System.Collections;
using UnityEngine;
using FMOD.Studio;
using System;
using UnityEngine.Rendering;
using TMPro;
using Unity.VisualScripting;

[System.Serializable]
public struct SubtitleText
{
    public float time;
    public string text;
}


public class SoundEventManager : MonoBehaviour
{
    [Header("AudioScaler")]
    [SerializeField] private AudioScaler audioScaler;

    [Header("Events")]
    public EventReference[] fmodEvents;
    private GameObject[] soundManager;
    [HideInInspector]
    public SoundToPlayList soundToPlayList;
    [Header("Trigger")]
    public bool isTrigger = false;

    [Header("Subtitle")]

    public SubtitleText[] subtitleText;
    public GameObject subtitleGO;
    [HideInInspector]
    public PauseMenu pauseMenu;
    public TMP_Text subtitle;
    private void Awake()
    {
        /*subtitleGO = GameObject.FindWithTag("Subtitle");
        if(subtitleGO != null)
        {
            Transform subtitleChild = subtitleGO.transform.Find("SubtitleText");
            subtitle = subtitleChild.GetComponent<TextMeshPro>();

        }*/
        soundManager = GameObject.FindGameObjectsWithTag("AudioManager");
        pauseMenu = FindObjectOfType<PauseMenu>();
        soundToPlayList = soundManager[0].GetComponent<SoundToPlayList>();
    }

    public void PlaySound(int index, GameObject go = null, bool firstTime = true)
    {
        if (!isTrigger)
        {
            if (index >= 0 && index < fmodEvents.Length)
            {
                FMOD.Studio.EventInstance soundEvent = FMODUnity.RuntimeManager.CreateInstance(fmodEvents[index]);

                if (go != null)
                {
                    FMODUnity.RuntimeManager.AttachInstanceToGameObject(soundEvent, go.GetComponent<Transform>(), go.GetComponent<Rigidbody>());
                }
                else
                {
                    FMODUnity.RuntimeManager.AttachInstanceToGameObject(soundEvent, gameObject.transform);
                }

                soundEvent.setVolume(pauseMenu.volume);
                soundEvent.start();

                Debug.Log("Tocou " + index);
            }
            else
            {
                Debug.LogError("Índice de evento de som fora do intervalo!");
            }
        }
        else
        {
            StartCoroutine(PlaySoundCoroutine(index, go, firstTime));
        }
    }

    IEnumerator PlaySoundCoroutine(int index, GameObject go = null, bool firstTime = true)
    {
        if (index >= 0 && index < fmodEvents.Length)
        {
            FMOD.Studio.EventInstance soundEvent = FMODUnity.RuntimeManager.CreateInstance(fmodEvents[index]);

            FMODUnity.RuntimeManager.AttachInstanceToGameObject(soundEvent, gameObject.transform);
            float delay = soundToPlayList.Delay();
            soundToPlayList.AddToList(GetEventDuration(index));
            yield return new WaitForSeconds(delay);
            if (pauseMenu.subtitleToggle)
            {
                if (firstTime)
                {
                    StartSubtitles();
                }
            }
            soundEvent.setVolume(pauseMenu.volume);
            soundEvent.start();

            float scaleDuration = GetEventDuration(index);
            audioScaler.StartScalingWithAudio(soundEvent, scaleDuration);
            Debug.Log("Tocou " + index);
        }
        else
        {
            Debug.LogError("Índice de evento de som fora do intervalo!");
        }
    }

    void StartSubtitles()
    {
        StartCoroutine(SubtitleCoroutine());
    }

    IEnumerator SubtitleCoroutine()
    {
        subtitleGO.SetActive(true);
        foreach (var voiceLine in subtitleText)
        {
            subtitle.text = voiceLine.text;

            yield return new WaitForSecondsRealtime(voiceLine.time);
        }

        subtitleGO.SetActive(false);
    }

    public float GetEventDuration(int index)
    {
        if (index >= 0 && index < fmodEvents.Length)
        {
            FMOD.Studio.EventDescription eventDescription;
            FMODUnity.RuntimeManager.StudioSystem.getEventByID(fmodEvents[index].Guid, out eventDescription);

            int length;
            eventDescription.getLength(out length);
            float duration = length / 1000f; 
            return duration;
        }
        else
        {
            Debug.LogError("Índice de evento de som fora do intervalo!");
            return -1f; 
        }
    }
}