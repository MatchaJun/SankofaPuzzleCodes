using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Animator fade;
    public FirstPersonController playerCapsule;
    private void OnTriggerEnter(Collider other)
    {
        fade.SetTrigger("Final");
        playerCapsule._controller.enabled = false;
    }

}
