namespace TwelveG.EnvironmentController
{
  using UnityEngine;

  public class RandomFlickerLight : MonoBehaviour
  {
    public float minIntensity = 0.5f;
    public float maxIntensity = 2f;
    public float flickerSpeed = 0.05f;

    private Light _light;

    private void OnEnable()
    {
      _light = GetComponent<Light>();
    }

    void Update()
    {
      float noise = Mathf.PerlinNoise(Time.time * 10f, 0f);
      _light.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
    }
  }
}