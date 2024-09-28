using UnityEngine;
using FMOD;
using FMOD.Studio;
using System.Collections;
using System;

public class AudioScaler : MonoBehaviour
{
    private float scaleMultiplier = 1.0f; 
    private float audioDuration = 0.0f; 
    private EventInstance currentEventInstance; 
    private Vector3 initialScale; 
    private float lerpSpeed = 8.0f; 

    void Start()
    {
        initialScale = transform.localScale;
    }

    public void StartScalingWithAudio(EventInstance eventInstance, float duration)
    {
        currentEventInstance = eventInstance;

        audioDuration = duration;

        StartCoroutine(UpdateScaleDuringPlayback());
    }

    private IEnumerator UpdateScaleDuringPlayback()
    {
        float elapsedTime = 0.0f; 

        while (elapsedTime < audioDuration)
        {
            float eventVolume = GetEventVolume(currentEventInstance);

            float targetScaleY = initialScale.y * (1.0f + eventVolume * 0.2f); 

            float newScaleY = Mathf.Lerp(transform.localScale.y, targetScaleY, Time.deltaTime * lerpSpeed);
            transform.localScale = new Vector3(initialScale.x, newScaleY, initialScale.z);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.localScale = initialScale;
    }

    private float GetEventVolume(EventInstance eventInstance)
    {
        ChannelGroup group;
        eventInstance.getChannelGroup(out group);

        DSP dsp;
        group.getDSP(0, out dsp);
        dsp.setMeteringEnabled(true, true);

        DSP_METERING_INFO meteringInfo;
        dsp.getMeteringInfo(IntPtr.Zero, out meteringInfo);

        float averageVolume = (meteringInfo.peaklevel[0] + meteringInfo.peaklevel[1]) / 2.0f;
        return averageVolume;
    }
}