using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererEffect : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float minWidth = 0.05f; // A largura mínima da linha
    public float maxWidth = 0.9f;  // A largura máxima da linha
    public float frequency = 2f;   // Frequência da "onda"
    private float waveCounter = 0f;

    void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
    }

    void Update()
    {
        waveCounter += Time.deltaTime;
        float wave = Mathf.Sin(waveCounter * frequency) * 0.5f + 0.5f; // Resulta em um valor entre 0 e 1
        float width = Mathf.Lerp(minWidth, maxWidth, wave); // Interpola entre minWidth e maxWidth

        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
    }
}
