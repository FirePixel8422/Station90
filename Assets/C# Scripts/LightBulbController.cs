using Unity.Mathematics;
using UnityEngine;


public class LightBulbController : UpdateMonoBehaviour
{
    [Header("EmmisionColor of the lightbulb model")]
    [ColorUsage(true, true)]
    [SerializeField] private Color lightColor = new Color(1, 0, 0, 1);

    [Header("Flicker Settings")]
    [SerializeField] private MinMaxFloat flickerDelayMinMax;
    [SerializeField] private MinMaxFloat flickerTimeMinMax;

    [Header("Intensity multiplier for the lightbulb materials emission")]
    [SerializeField] private float matEmmissionMultiplier = 10;


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
                if (lights[i].bakingOutput.lightmapBakeType != LightmapBakeType.Realtime) continue;

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

    protected override void OnUpdate()
    {
        // Wait until next state update delay has passed before toggling flicker state
        if (Time.time < nextStateUpdateGlobalTime) return;

        isFlickering = !isFlickering;

        if (isFlickering == false)
        {
            lightSource.intensity = baseIntensity;
            nextStateUpdateGlobalTime = Time.time + EzRandom.Range(flickerDelayMinMax);
        }
        else
        {
            lightSource.intensity = 0;
            nextStateUpdateGlobalTime = Time.time + EzRandom.Range(flickerTimeMinMax);
        }

        UpdateEmission(lightBulbRenderer, lightColor * lightSource.intensity * matEmmissionMultiplier);
    }
}
