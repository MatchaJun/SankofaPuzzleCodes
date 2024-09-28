using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmitterVariables : MonoBehaviour
{
    private StudioEventEmitter emitter;
    private EventInstance eventInstance;

    void Start()
    {
        emitter = GetComponent<StudioEventEmitter>();
        if (emitter != null)
        {
            eventInstance = emitter.EventInstance;
        }
    }

    public void SetMusicParameter(string parameterName, float value)
    {
        if (eventInstance.isValid())
        {
            eventInstance.setParameterByName(parameterName, value);
        }
        else
        {
            Debug.LogError("Variavel Invalida");
        }
    }
}
