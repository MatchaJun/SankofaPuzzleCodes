using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ProgressCircle : MonoBehaviour
{
    public int segments = 100;
    public float radius = 1f;
    private LineRenderer line;

    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();
        SetupCircle();
    }

    void SetupCircle()
    {
        line.positionCount = segments + 1;
        line.useWorldSpace = false;
        CreatePoints(0); 
    }

    public void CreatePoints(float progress)
    {
        float x;
        float y;
        float z = 0f;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            line.SetPosition(i, new Vector3(x, y, z));

            angle += (360f / segments) * progress;
        }
    }


    public void UpdateProgress(float progress)
    {
        CreatePoints(progress);
    }
}