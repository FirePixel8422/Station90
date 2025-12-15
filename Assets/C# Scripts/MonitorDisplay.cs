using System.Collections;
using Unity.Mathematics;
using UnityEngine;


public class MonitorDisplay : UpdateMonoBehaviour
{
    [SerializeField] private Transform staticScreenCamHolder;

    [SerializeField] private MinMaxInt monitorFPSMinMax = new MinMaxInt(10, 25);
    [SerializeField] private MinMaxFloat camSwapDelayMinMax = new MinMaxFloat(0.5f, 1f);

    private Camera monitorCamera;
    private CameraController[] gameCameras;

    private int selectedCameraIndex;
    private bool isSwappingCamera;

    private float nextCamUpdateGlobalTime;


    private void Start()
    {
        monitorCamera = GetComponentInChildren<Camera>();
        monitorCamera.enabled = false;

        gameCameras = this.FindObjectsOfType<CameraController>();

        gameCameras[selectedCameraIndex].SetupCamera(monitorCamera);
    }

    protected override void OnUpdate()
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
        selectedCameraIndex.IncrememtSmart(gameCameras.Length);

        monitorCamera.transform.SetParent(staticScreenCamHolder, false, false);

        yield return new WaitForSeconds(EzRandom.Range(camSwapDelayMinMax));

        gameCameras[selectedCameraIndex].SetupCamera(monitorCamera);

        isSwappingCamera = false;
    }
}
