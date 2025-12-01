using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerFlashLight : MonoBehaviour
{
    [Header("Max distance before the flashlights intensity becomes value 0 of the curve")]
    [SerializeField] private float maxLightDistance = 15;

    [Header("Intensity based on percentage of distance from maxLightDistance")]
    [SerializeField] private NativeSampledAnimationCurve intensityCurve = NativeSampledAnimationCurve.Default;

    [Header("Intensity to assign the flashlight when no surfaces are hit with the ray check")]
    [SerializeField] private float intensityOnVoid = 1;

    [Header("How many rings to cast rays in, and how many rays per ring")]
    [SerializeField] private int[] checkRingRayCounts;

    [Header("Flicker Settings")]
    [SerializeField] private MinMaxFloat flickerIntensityMultiplierMinMax;
    [SerializeField] private MinMaxFloat flickerDelayMinMax;
    [SerializeField] private MinMaxFloat flickerTimeMinMax;

    [Header("Max flashlight tilt angle")]
    [SerializeField] private float maxFlashlightTiltAngle = 25f;

    [Header("Power Down Settings")]
    [SerializeField] private float powerDownDuration = 2f;

    private Light flashLight;
    private Camera cam;

    private bool isEnabled = true;
    private float fullPowerDownGlobalTime;
    private float powerDownStartIntensity;

    private bool isFlickering;
    private float nextStateUpdateGlobalTime;
    private float cIntensityMultiplier;



    public void OnFlashLightToggle(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            isEnabled = !isEnabled;

            // Turned On
            if (isEnabled)
            {
                powerDownStartIntensity = flashLight.intensity;
                fullPowerDownGlobalTime = Time.time + powerDownDuration;
            }
        }
    }

    private void Awake()
    {
        flashLight = GetComponentInChildren<Light>(true);
        cam = GetComponentInChildren<Camera>(true);
    }


    private void OnEnable() => UpdateScheduler.RegisterUpdate(OnUpdate);
    private void OnDisable() => UpdateScheduler.UnRegisterUpdate(OnUpdate);

    private void OnUpdate()
    {
        if (isEnabled)
        {
            if (TryGetAvgLightDistance(out float distance))
            {
                flashLight.intensity = GetIntensity(distance) * cIntensityMultiplier;
            }
            else
            {
                flashLight.intensity = intensityOnVoid * cIntensityMultiplier;
            }

            // Wait until next state update delay has passed before executing
            if (Time.time < nextStateUpdateGlobalTime) return;

            // Toggle flickering state
            isFlickering = !isFlickering;
            
            // If Lamp starts flickering now
            if (isFlickering)
            {
                cIntensityMultiplier = EzRandom.Range(flickerIntensityMultiplierMinMax);
                flashLight.intensity *= cIntensityMultiplier;

                nextStateUpdateGlobalTime = Time.time + EzRandom.Range(flickerTimeMinMax);
            }
            // If lamp is no longer flickering, let it flicker
            else
            {
                cIntensityMultiplier = 1;

                nextStateUpdateGlobalTime = Time.time + EzRandom.Range(flickerDelayMinMax);
            }
        }
        else
        {
            float powerPercentage01 = math.clamp(fullPowerDownGlobalTime - Time.time, 0, int.MaxValue) / powerDownDuration;
            flashLight.intensity = powerDownStartIntensity * powerPercentage01;
        }
    }

    private float GetIntensity(float distance)
    {
        return intensityCurve.Evaluate(distance / maxLightDistance);
    }


    public bool TryGetAvgLightDistance(out float avgDistance)
    {
        Vector3 flashLightPos = flashLight.transform.position;

        Vector3 center = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
        Ray centerRay = cam.ScreenPointToRay(center);

        Vector3 origin = centerRay.origin;
        Vector3 forward = centerRay.direction;

        float coneRad = flashLight.spotAngle * Mathf.Deg2Rad;

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
                    totalDistance += Vector3.Distance(flashLightPos, hit.point);
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
                    totalDistance += Vector3.Distance(flashLightPos, hit.point);
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

        Quaternion lookRotation = GetLookRotation(flashLight.transform.position, origin + forward * avgDistance);
        float angleDiff = Quaternion.Angle(flashLight.transform.rotation, lookRotation);

        lookRotation = Quaternion.RotateTowards(lookRotation, flashLight.transform.rotation, Mathf.MoveTowards(angleDiff, 0, maxFlashlightTiltAngle));
        flashLight.transform.rotation = lookRotation;

        flashLight.transform.localEulerAngles = new Vector3(flashLight.transform.localEulerAngles.x, flashLight.transform.localEulerAngles.y, 0f);
        return true;
    }


    public static Quaternion GetLookRotation(Vector3 from, Vector3 targetPos)
    {
        Vector3 dir = targetPos - from;
        return Quaternion.LookRotation(dir, Vector3.up);
    }
}
