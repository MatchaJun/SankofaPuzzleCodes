using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    public GameObject volume;

    private void Start()
    {
        StartCoroutine(Volume());
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            SceneManager.LoadScene("Recording2");
        }
    }

    IEnumerator Volume()
    {
        yield return new WaitForSeconds(0f);
        volume.SetActive(false);
        yield return new WaitForSeconds(0f);
        volume.SetActive(true);
    }
}