using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerFlashlight : MonoBehaviour
{
    [Header("Max distance before the flashlights intensity becomes value 0 of the curve")]
    [SerializeField] private float maxLightDistance = 15;

    [Header("Intensity based on percentage of distance from maxLightDistance")]
    [SerializeField] private NativeSampledAnimationCurve intensityCurve = NativeSampledAnimationCurve.Default;

    [Header("Intensity to assign the flashlight when no surfaces are hit with the ray check")]
    [SerializeField] private float intensityOnVoid = 1;

    [Header("How many rings to cast rays in, and how many rays per ring")]
    [SerializeField] private int[] checkRingRayCounts;

    [Header("Max flashlight tilt angle")]
    [SerializeField] private float maxFlashlightTiltAngle = 25f;

    [Header("Flashlight toggle delay and intensity update speed")]
    [SerializeField] private float flashlightLerpSpeed = 2f;
    [SerializeField] private float flashlightToggleDelay = 0.2f; 

    [Header("Flashlight SFX")]
    [SerializeField] private AudioSource flashlightAudioSource;
    [SerializeField] private MinMaxFloat randomPitchMinMax = new MinMaxFloat(0.9f, 1.1f);
    [SerializeField] private AudioClip flashlightToggleOnClip;
    [SerializeField] private AudioClip flashlightToggleOffClip;

    private Light flashlight;
    private Camera cam;

    private bool isEnabled = true;
    private float cIntensity;



    public void OnFlashlightToggle(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)  
        {
            Invoke(nameof(ToggleFlashlightAfterDelay), flashlightToggleDelay);

            flashlightAudioSource.PlayOneShotClipWithPitch(isEnabled ? flashlightToggleOnClip : flashlightToggleOffClip, EzRandom.Range(randomPitchMinMax));
        }
    }
    private void ToggleFlashlightAfterDelay()
    {
        isEnabled = !isEnabled;
        if (isEnabled == false)
        {
            cIntensity = 0;
        }
    }
    private void Awake()
    {
        flashlight = GetComponentInChildren<Light>(true);
        cam = GetComponentInChildren<Camera>(true);
        intensityCurve.Bake();
    }

    private void OnEnable() => UpdateScheduler.RegisterUpdate(OnUpdate);
    private void OnDisable() => UpdateScheduler.UnRegisterUpdate(OnUpdate);

    private void OnUpdate()
    {
        if (isEnabled)
        {
            UpdateFlashlightIntensity();
        }

        flashlight.intensity = math.lerp(flashlight.intensity, cIntensity, flashlightLerpSpeed * Time.deltaTime);
    }
    private void UpdateFlashlightIntensity()
    {
        bool doesFlashlightSeeWall = TryGetAvgLightDistance(out float distance);

        cIntensity = doesFlashlightSeeWall ?
            intensityCurve.Evaluate(distance / maxLightDistance) :
            intensityOnVoid;
    }


    private bool TryGetAvgLightDistance(out float avgDistance)
    {
        Vector3 flashlightPos = flashlight.transform.position;

        Vector3 center = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
        Ray centerRay = cam.ScreenPointToRay(center);

        Vector3 origin = centerRay.origin;
        Vector3 forward = centerRay.direction;

        float coneRad = flashlight.spotAngle * Mathf.Deg2Rad;

        float totalDistance = 0;
        int hitCount = 0;

        // Compute an orthonormal basis around forward
        Vector3 right = Vector3.Cross(forward, Vector3.up);
        if (right.sqrMagnitude < 0.0001f)
        {
            right = Vector3.Cross(forward, Vector3.right);
        }
        right.Normalize();

        Vector3 up = Vector3.Cross(right, forward).normalized;

        // Process rings
        for (int ringIndex = 0; ringIndex < checkRingRayCounts.Length; ringIndex++)
        {
            int raysInRing = checkRingRayCounts[ringIndex];

            // radius 0 = center ray, radius 1 = edge of cone
            float ringT = checkRingRayCounts.Length == 1
                ? 0f
                : (float)ringIndex / (checkRingRayCounts.Length - 1);

            float ringAngle = ringT * coneRad;

            if (raysInRing == 1)
            {
                // Single ray in ring > forward with this angle
                Vector3 dirSingle = Quaternion.AngleAxis(ringAngle * Mathf.Rad2Deg, right) * forward;
                dirSingle.Normalize();

                if (Physics.Raycast(origin, dirSingle, out RaycastHit hit, maxLightDistance))
                {
                    totalDistance += Vector3.Distance(flashlightPos, hit.point);
                    hitCount += 1;
                }
                continue;
            }

            float step = 360f / raysInRing;

            for (int r = 0; r < raysInRing; r++)
            {
                float angleAround = r * step;

                // Direction on ring
                Vector3 axis = (right * Mathf.Cos(angleAround * Mathf.Deg2Rad)) + (up * Mathf.Sin(angleAround * Mathf.Deg2Rad)).normalized;

                Quaternion rot = Quaternion.AngleAxis(ringAngle * Mathf.Rad2Deg, axis);
                Vector3 dir = rot * forward;
                dir.Normalize();

                if (Physics.Raycast(origin, dir, out RaycastHit hit, maxLightDistance))
                {
                    totalDistance += Vector3.Distance(flashlightPos, hit.point);
                    hitCount++;
                }
            }
        }

        if (hitCount == 0)
        {
            avgDistance = 0;
            return false;
        }

        avgDistance = totalDistance / hitCount;

        Quaternion lookRotation = GetLookRotation(flashlight.transform.position, origin + forward * avgDistance);
        float angleDiff = Quaternion.Angle(flashlight.transform.rotation, lookRotation);

        lookRotation = Quaternion.RotateTowards(lookRotation, flashlight.transform.rotation, Mathf.MoveTowards(angleDiff, 0, maxFlashlightTiltAngle));
        flashlight.transform.rotation = lookRotation;

        flashlight.transform.localEulerAngles = new Vector3(flashlight.transform.localEulerAngles.x, flashlight.transform.localEulerAngles.y, 0f);
        return true;
    }
    private Quaternion GetLookRotation(Vector3 from, Vector3 targetPos)
    {
        Vector3 dir = targetPos - from;
        return Quaternion.LookRotation(dir, Vector3.up);
    }


    private void OnDestroy()
    {
        intensityCurve.Dispose();
    }
}
