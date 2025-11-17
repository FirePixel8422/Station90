using UnityEngine;


public class LightBulbController : MonoBehaviour
{
    [Header("EmmisionColor of the lightbulb model")]
    [ColorUsage(true, true)]
    [SerializeField] private Color lightColor = new Color(1, 0, 0, 1);

    [Header("Flicker Settings")]
    [SerializeField] private MinMaxFloat flickerIntensityMinMax;
    [SerializeField] private MinMaxFloat flickerDelayMinMax;
    [SerializeField] private MinMaxFloat flickerTimeMinMax;


    private Light lightSource;
    private Renderer lightBulbRenderer;
    private float baseIntensity;

    private static readonly int EmissionID = Shader.PropertyToID("_EmissionColor");
    private MaterialPropertyBlock mpb;

    private float nextStateUpdateGlobalTime;
    private bool isFlickering;


    private void Awake()
    {
        // Get all lights under this gameObject and store the realtime light source and base intensity
        if (transform.TryGetComponentsInChildren(out Light[] lights, true))
        {
            for (int i = 0; i < lights.Length; i++)
            {
                if (lights[i].lightmapBakeType != LightmapBakeType.Realtime) continue;

                lightSource = lights[i];
                baseIntensity = lightSource.intensity;
            }
        }

        lightBulbRenderer = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();

        UpdateEmission(lightBulbRenderer, lightColor);
    }
    private void UpdateEmission(Renderer renderer, Color newColor)
    {
        renderer.GetPropertyBlock(mpb);
        mpb.SetColor(EmissionID, newColor);
        renderer.SetPropertyBlock(mpb);
    }

    private void OnEnable() => UpdateScheduler.RegisterUpdate(OnUpdate);
    private void OnDisable() => UpdateScheduler.UnRegisterUpdate(OnUpdate);

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
        // If lamp was flickering, let it flicker
        else
        {
            lightSource.intensity = EzRandom.Range(flickerIntensityMinMax);

            isFlickering = true;
            nextStateUpdateGlobalTime = Time.time + EzRandom.Range(flickerTimeMinMax);
        }
    }
}
