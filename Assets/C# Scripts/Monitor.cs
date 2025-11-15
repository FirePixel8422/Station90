using System.Collections;
using UnityEngine;


public class Monitor : MonoBehaviour
{
    [SerializeField] private Transform staticScreenCamHolder;

    [SerializeField] private MinMaxInt monitorFPSMinMax = new MinMaxInt(10, 25);
    [SerializeField] private MinMaxFloat camSwapDelayMinMax = new MinMaxFloat(0.5f, 1f);

    private Camera monitorCamera;
    private CameraController[] gameCameras;

    private int selectedCameraIndex;
    private bool isSwappingCamera;

    private float nextCamUpdateGlobalTime;



    private void OnEnable() => UpdateScheduler.RegisterUpdate(OnUpdate);
    private void OnDisable() => UpdateScheduler.UnRegisterUpdate(OnUpdate);

    private void Start()
    {
        monitorCamera = GetComponentInChildren<Camera>();
        monitorCamera.enabled = false;

        gameCameras = this.FindObjectsOfType<CameraController>();

        gameCameras[selectedCameraIndex].SetupCamera(monitorCamera);
    }

    private void OnUpdate()
    {
        if (Time.time > nextCamUpdateGlobalTime)
        {
            nextCamUpdateGlobalTime = Time.time + (1f / EzRandom.Range(monitorFPSMinMax));
            monitorCamera.Render();
        }

        if (isSwappingCamera) return;

        if (Input.GetKeyDown(KeyCode.M))
        {
            isSwappingCamera = true;
            StartCoroutine(ChangeCamera());
        }
    }

    private IEnumerator ChangeCamera()
    {
        selectedCameraIndex += 1;

        if (selectedCameraIndex == gameCameras.Length)
        {
            selectedCameraIndex = 0;
        }

        monitorCamera.transform.SetParent(staticScreenCamHolder, false, false);

        yield return new WaitForSeconds(EzRandom.Range(camSwapDelayMinMax));

        gameCameras[selectedCameraIndex].SetupCamera(monitorCamera);

        isSwappingCamera = false;
    }
}
