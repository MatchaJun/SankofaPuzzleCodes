using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundToPlayList : MonoBehaviour
{
    public bool anySoundIsPlaying = false;

    [Header("Characters")]
    [SerializeField] private Animator griotSprite;
    [SerializeField] private bool griot;
    [SerializeField] private Animator sankofaSprite;
    [SerializeField] private bool sankofa;
    private GameObject[] soundEventManagers;
    private SoundEventManager soundEventManager;
    private FMOD.Studio.EventInstance[] soundEventList;
    private int i;
    private float timer;
    private float seconds;
    private float delay;
    private void Awake()
    {
        soundEventManagers = GameObject.FindGameObjectsWithTag("AudioManager"); 
        soundEventManager = soundEventManagers[0].GetComponent<SoundEventManager>();
        StartCoroutine(CountDown());
    }

    IEnumerator CountDown()
    {
        while (true)
        {
            delay = delay - .1f;
            if (delay < 0)
            {
                delay = 0;
            }
            yield return new WaitForSeconds(.1f);
        }
    }
    private void Update()
    {

    }
    public void SoundIsPlaying(float index)
    {
        StartCoroutine(PlaySoundCoroutine(index));
    }
    public float Delay()
    {
        return delay;
    }
    public void AddToList(float index)
    {
        delay = delay + index;
    }
    IEnumerator PlaySoundCoroutine(float index)
    {
        anySoundIsPlaying = true;
        yield return new WaitForSeconds(index);
        anySoundIsPlaying = false;
    }
}
