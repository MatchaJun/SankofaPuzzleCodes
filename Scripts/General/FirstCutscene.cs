using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstCutscene : MonoBehaviour
{
    public Image bgSankofa;
    public Image bgGriot;
    public Animator tutorial;

    private Color bgColor;
    private Color whiteBgColor;
    void Start()
    {
        bgColor = bgSankofa.color;
        StartCoroutine(ChangeColor());
    }

    private IEnumerator ChangeColor()
    {
        whiteBgColor = Color.white;
        whiteBgColor.a = 0.3f;
        bgSankofa.color = whiteBgColor;
        bgGriot.color = whiteBgColor;
        yield return new WaitForSeconds(55f);
        bgSankofa.color = bgColor;
        bgGriot.color = bgColor;
        tutorial.SetTrigger("startTutorial");
    }
}
