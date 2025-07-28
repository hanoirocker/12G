namespace TwelveG.GameController
{
  using TMPro;
  using UnityEngine;
  using UnityEngine.UI;

  public class SliderHandler : MonoBehaviour
  {
    [Header("References")]
    [SerializeField] private TextMeshProUGUI volumeTextValue = null;

    private Slider volumeSlider = null;

    private void Awake()
    {
      volumeSlider = GetComponent<Slider>();
    }

    private void Start()
    {
      // Llamar a función del SaveSystem que retorne valor
      // de parámetro de audio previamente guardado. Sino, el valor es 0.5 (default value)
    }

    public void SetSliderVolume(float volume)
    {
      // Llamar a AudioManager para cada tipo de slider
      volumeTextValue.text = volume.ToString("0.0");
    }
  }
}