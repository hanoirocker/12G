using UnityEngine;

namespace TwelveG.PostProcessing
{
  public class GlitchEffect : MonoBehaviour
  {
    public Material material;
    [Header("Parameteres")]
    [Space]
    public float effectAmount;
    public float noiseAmount;
    public float glitchStrength;
    public float scanLinesStrength;

    private void Update()
    {
      material.SetFloat("_EffectAmount", effectAmount);
      material.SetFloat("_NoiseAmount", noiseAmount);
      material.SetFloat("_GlitchStrength", glitchStrength);
      material.SetFloat("_ScanLinesStrength", scanLinesStrength);
    }
  }
}
