using Cinemachine;
using StarterAssets;
using System.Collections;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.VFX;
using Debug = UnityEngine.Debug;

public class PuzzleAction : MonoBehaviour
{
    [Header("FinalPuzzle")]
    public GameObject playerCapsule;
    public GameObject goTo;
    public GameObject goToBack;
    public VisualEffect wind;
    public float lerpSpeed = 0.2f;
    private StarterAssetsInputs starterAssetsInputs;
    private FirstPersonController firstPersonController;

    [Header("Door")]
    public GameObject door;

    [Header("LookAtObject")]
    public CinemachineVirtualCamera cam;
    private float lookAtDuration = 1f;
    private float zoomInFOV = 50f;
    private float zoomDuration = .3f;
    private float startFOV = 70f;
    private GameObject[] gjList;
    private Camera main;

    public float duration;

    private void Awake()
    {
        gjList = GameObject.FindGameObjectsWithTag("Bridge");
        if (gjList != null)
        {
            foreach (GameObject go in gjList)
            {
                Transform childFresnel = go.transform.Find("PonteFresnel");
                foreach (Transform child in childFresnel)
                {
                    Renderer renderer = child.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        Material dissolveBridgeMaterial = renderer.material;
                        dissolveBridgeMaterial.SetFloat("_Dissolve", 1f);
                    }
                }
            }
        }

        main = Camera.main;
        if (playerCapsule != null)
        {
            firstPersonController = playerCapsule.GetComponent<FirstPersonController>();
            starterAssetsInputs = playerCapsule.GetComponent<StarterAssetsInputs>();
        }
    }

    public void OpenDoor()
    {
        StartCoroutine(DoorCoroutine(2f));
    }

    public void OpenFinalDoor()
    {
        StartCoroutine(MyCoroutine());
    }

    public void SpawnBridge()
    {
        StartCoroutine(BridgeCoroutine());
    }

    public void SpawnBridgePuzzle()
    {
        StartCoroutine(BridgeCoroutinePuzzle());
    }

    public GameObject player;

    IEnumerator MyCoroutine()
    {
        door.GetComponent<Animator>().SetTrigger("Open");
        yield return new WaitForSeconds(2);
        player.transform.position = new Vector3(-4.55196619f, 2.5f, 14.21f);
    }

    public void SpawnPuzzle()
    {
        gameObject.SetActive(true);
    }

    public void GoToFinalPuzzle()
    {
        StartCoroutine(FinalPuzzle());
    }

    IEnumerator FinalPuzzle()
    {
        wind.Play();
        yield return new WaitForSeconds(3f);

        firstPersonController._controller.enabled = false;
        firstPersonController.isWindActive = true;

        while (Vector3.Distance(playerCapsule.transform.position, goTo.transform.position) > 3.2f)
        {
            playerCapsule.transform.position = Vector3.Lerp(playerCapsule.transform.position, goTo.transform.position, lerpSpeed * Time.deltaTime);
            yield return null;
        }

        wind.Stop();
    }

    public void GoToPortal()
    {
        StartCoroutine(Portal());
    }


    IEnumerator Portal()
    {
        wind.Play();
        yield return new WaitForSeconds(3f);

        firstPersonController._controller.enabled = false;
        firstPersonController.isWindActive = true;

        while (Vector3.Distance(playerCapsule.transform.position, goToBack.transform.position) > .2f)
        {
            playerCapsule.transform.position = Vector3.Lerp(playerCapsule.transform.position, goToBack.transform.position, lerpSpeed * Time.deltaTime);
            print(Vector3.Distance(playerCapsule.transform.position, goToBack.transform.position));
            yield return null;
        }

        firstPersonController._controller.enabled = true;
        firstPersonController.isWindActive = false;

        wind.Stop();
    }

    private IEnumerator ApplyDissolve(Material material, float start, float end)
    {
        float startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            float newCutoff = Mathf.Lerp(start, end, t);
            material.SetFloat("_Dissolve", newCutoff);
            yield return null;
        }

        material.SetFloat("_Dissolve", end);
    }

    IEnumerator BridgeCoroutine()
    {
        Transform ponteTransform = transform.Find("Ponte");

        if (ponteTransform != null)
        {
            foreach (Transform child in ponteTransform)
            {
                Renderer renderer = child.GetComponent<Renderer>();
                MeshCollider meshCollider = child.GetComponent<MeshCollider>();
                meshCollider.enabled = true;
                if (renderer != null)
                {
                    Material dissolveMaterial = renderer.material;
                    StartCoroutine(ApplyDissolve(dissolveMaterial, 1, 0));
                }
            }
        }
        else
        {
            Debug.LogWarning("Ponte não encontrada");
        }

        yield return null;
    }

    IEnumerator BridgeCoroutinePuzzle()
    {
        yield return new WaitForSeconds(.5f);
        Transform ponteTransform = transform.Find("PonteFresnel");

        if (ponteTransform != null)
        {
            foreach (Transform child in ponteTransform)
            {
                Renderer renderer = child.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Material dissolveMaterial = renderer.material;
                    StartCoroutine(ApplyDissolve(dissolveMaterial, 1, 0));
                }
            }
        }
        else
        {
            Debug.LogWarning("Ponte não encontrada");
        }

        yield return null;
    }

    IEnumerator DoorCoroutine(float delay)
    {
        print("Abriu");
        yield return new WaitForSeconds(delay);

        Transform childTransform = transform.Find("Gate_Ren");
        if (childTransform == null)
        {
            yield break;
        }

        Renderer childRenderer = childTransform.GetComponent<Renderer>();
        if (childRenderer == null)
        {
            Debug.LogError("Renderer component not found");
            yield break;
        }

        Material[] materials = childRenderer.materials;

        BoxCollider meshCollider = gameObject.GetComponent<BoxCollider>();
        meshCollider.enabled = false;

        float startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            float newCutoff = Mathf.Lerp(0, 1, t);

            foreach (Material material in materials)
            {
                material.SetFloat("_Dissolve", newCutoff);
            }

            yield return null;
        }

        foreach (Material material in materials)
        {
            material.SetFloat("_Dissolve", 1);
        }
    }

    public void LookAtObject()
    {
        StartCoroutine(LookAtObjectCoroutine());
    }

    public void LookAtDown(float delay)
    {
        StartCoroutine(LookDown(delay));
    }

    private IEnumerator LookAtObjectCoroutine()
    {
        starterAssetsInputs.cursorLocked = false;
        starterAssetsInputs.cursorInputForLook = false;
        firstPersonController._controller.enabled = false;
        firstPersonController.SetCinemachineTargetPitch(0f);
        firstPersonController.SetRotationVelocity(0f);
        firstPersonController.UpdateCameraRotation();

        while (starterAssetsInputs.move.magnitude > 0.1f)
        {
            yield return null;
        }
        Vector3 initialPosition = firstPersonController.CinemachineCameraTarget.transform.position;
        Quaternion initialRotation = firstPersonController.CinemachineCameraTarget.transform.rotation;

        Quaternion targetRotation = Quaternion.LookRotation(gameObject.transform.position - firstPersonController.CinemachineCameraTarget.transform.position);

        float time = 0.0f;
        while (time < lookAtDuration)
        {
            firstPersonController.CinemachineCameraTarget.transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, time / lookAtDuration);
            time += Time.deltaTime;

            firstPersonController.UpdateCameraRotation();

            yield return null;
        }
        firstPersonController.CinemachineCameraTarget.transform.rotation = targetRotation;

        yield return new WaitForSeconds(0.3f);

        //Zoom in
        time = 0;
        while (time < zoomDuration)
        {
            cam.m_Lens.FieldOfView = Mathf.Lerp(startFOV, zoomInFOV, time / zoomDuration);
            time += Time.deltaTime;
            yield return null;
        }
        cam.m_Lens.FieldOfView = zoomInFOV;

        yield return new WaitForSeconds(1.5f);

        playerCapsule.transform.rotation = firstPersonController.CinemachineCameraTarget.transform.rotation;

        //Ajuste 
        Vector3 playerEulerAngles = playerCapsule.transform.eulerAngles;
        playerEulerAngles.z = 0f;
        playerEulerAngles.x = 0f;
        playerCapsule.transform.eulerAngles = playerEulerAngles;

        GameObject cameraRoot = playerCapsule.transform.Find("PlayerCameraRoot").gameObject;
        Vector3 cameraRootEulerAngles = cameraRoot.transform.eulerAngles;

        cameraRootEulerAngles.z = 0f;
        cameraRootEulerAngles.y = playerEulerAngles.y;
        //Zoom out

        Vector3 directionToLookAt = (gameObject.transform.position - playerCapsule.transform.position).normalized;

        Quaternion desiredRotation = Quaternion.LookRotation(directionToLookAt, Vector3.up);

        time = 0;
        while (time < zoomDuration)
        {
            cameraRootEulerAngles.x = Mathf.Lerp(cameraRootEulerAngles.x, 0f, time / zoomDuration); ;
            cam.m_Lens.FieldOfView = Mathf.Lerp(zoomInFOV, startFOV, time / zoomDuration);
            cameraRoot.transform.rotation = Quaternion.Slerp(initialRotation, desiredRotation, time / zoomDuration);
            time += Time.deltaTime;
            firstPersonController.UpdateCameraRotation();
            yield return null;
        }

        cam.m_Lens.FieldOfView = startFOV;

        SynchronizeRotationValues();

        firstPersonController.UpdateCameraRotation();

        starterAssetsInputs.look = Vector2.zero;
        starterAssetsInputs.cursorLocked = true;
        starterAssetsInputs.cursorInputForLook = true;
        firstPersonController._controller.enabled = true;
    }

    private IEnumerator LookDown(float delay)
    {
        yield return new WaitForSeconds(delay);
        // Desativa o controle da câmera
        starterAssetsInputs.cursorLocked = false;
        starterAssetsInputs.cursorInputForLook = false;

        // Guarda a posição inicial da câmera
        Quaternion initialRotation = firstPersonController.CinemachineCameraTarget.transform.rotation;

        // Calcula a rotação alvo para olhar para baixo (90 graus para baixo)
        Quaternion targetRotation = Quaternion.Euler(90f, initialRotation.eulerAngles.y, initialRotation.eulerAngles.z);

        float time = 0.0f;
        while (time < 2f)
        {
            // Interpola a rotação da câmera para olhar para baixo
            firstPersonController.CinemachineCameraTarget.transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, time / 2f);
            time += Time.deltaTime;

            firstPersonController.UpdateCameraRotation();

            yield return null;
        }
        // Define a rotação final da câmera para olhar para baixo
        firstPersonController.CinemachineCameraTarget.transform.rotation = targetRotation;

        // Aguarda um curto período de tempo (opcional)
        yield return new WaitForSeconds(0.3f);

        // Sincroniza a rotação do jogador com a rotação da câmera
        playerCapsule.transform.rotation = firstPersonController.CinemachineCameraTarget.transform.rotation;

        // Ajusta os ângulos de Euler do jogador para garantir que os valores de rotação em Z e X sejam 0
        Vector3 playerEulerAngles = playerCapsule.transform.eulerAngles;
        playerEulerAngles.z = 0f;
        playerEulerAngles.x = 0f;
        playerCapsule.transform.eulerAngles = playerEulerAngles;

        GameObject cameraRoot = playerCapsule.transform.Find("PlayerCameraRoot").gameObject;

        Vector3 cameraRootEulerAngles = cameraRoot.transform.eulerAngles;
        cameraRootEulerAngles.z = 0f;
        cameraRootEulerAngles.y = playerEulerAngles.y;

        // Reativa o controle da câmera
        starterAssetsInputs.look = Vector2.zero;
        starterAssetsInputs.cursorLocked = true;
        starterAssetsInputs.cursorInputForLook = true;

        // Atualiza a rotação da câmera
        firstPersonController.UpdateCameraRotation();
        SynchronizeRotationValues();
    }

    // Método para sincronizar os valores de rotação
    private void SynchronizeRotationValues()
    {
        Vector3 currentRotation = firstPersonController.CinemachineCameraTarget.transform.localEulerAngles;

        float pitch = currentRotation.x > 180 ? currentRotation.x - 360 : currentRotation.x;
        float yaw = currentRotation.y > 180 ? currentRotation.y - 360 : currentRotation.y;

        firstPersonController.SetCinemachineTargetPitch(pitch);
        firstPersonController.SetRotationVelocity(yaw);
    }
}