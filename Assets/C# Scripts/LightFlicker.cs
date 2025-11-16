using UnityEngine;


public class LightFlicker : MonoBehaviour
{
    [SerializeField] private MinMaxFloat flickerIntensityMinMax;
    [SerializeField] private MinMaxFloat flickerDelayMinMax;
    [SerializeField] private MinMaxFloat flickerTimeMinMax;


    private Light lightSource;
    private float baseIntensity;

    private float nextStateUpdateGlobalTime;
    private bool isFlickering;


    private void OnEnable() => UpdateScheduler.RegisterUpdate(OnUpdate);
    private void OnDisable() => UpdateScheduler.UnRegisterUpdate(OnUpdate);

    private void Start()
    {
        lightSource = GetComponent<Light>();
        baseIntensity = lightSource.intensity;
    }

    private void OnUpdate()
    {
        // Wait until next state update delay has passed before executing
        if (Time.time < nextStateUpdateGlobalTime) return;

        // If lamp was flickering, stabilize it and set 
        if (isFlickering)
        {
            lightSource.intensity = baseIntensity;

            isFlickering = false;
            nextStateUpdateGlobalTime = Time.time + EzRandom.Range(flickerDelayMinMax);
        }
        else
        {
            lightSource.intensity = EzRandom.Range(flickerIntensityMinMax);

            isFlickering = true;
            nextStateUpdateGlobalTime = Time.time + EzRandom.Range(flickerTimeMinMax);
        }
    }
}
