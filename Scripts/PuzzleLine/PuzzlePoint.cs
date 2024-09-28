using UnityEngine;
using UnityEngine.UIElements;

public class PuzzlePoint : MonoBehaviour
{
    public GameObject particlePrefab;
    public bool IsActivated { get; private set; }

    public void ActivatePoint()
    {
        IsActivated = true;

        Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        GameObject particleInstance = Instantiate(particlePrefab, transform.position, Quaternion.identity);
        particleInstance.transform.rotation = Quaternion.Euler(-90, 0, 0);

        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    public void ResetPoint()
    {
        IsActivated = false;
        gameObject.GetComponent<MeshRenderer>().enabled = true;
    }
}
