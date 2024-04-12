using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Settings_Slider : MonoBehaviour
{

    [SerializeField] private InspectorEvent _event;
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _txtValue;
    [SerializeField] private float _minValue = 0f;
    [SerializeField] private float _maxValue = 100f;
    [SerializeField] private float _converter = 0f;
    public float Converter => _converter;

    private float m_value = 0f;

    public void Setup(float value)
    {
        _slider.onValueChanged.RemoveAllListeners();
        _slider.onValueChanged.AddListener(SLD_ChangeValue);

        _slider.minValue = _minValue;
        _slider.maxValue = _maxValue;

        m_value = value - _converter;

        _slider.SetValueWithoutNotify(m_value);
        _txtValue.SetText(m_value.ToString());

        //TODO load settings using event info
    }

    private void SLD_ChangeValue(float value)
    {
        _event.Notify(value + _converter);
        _txtValue.SetText(value.ToString());

        m_value = value;
    }

    public void BTN_Plus(float value)
    {
        _slider.value = m_value + value;
    }

}
