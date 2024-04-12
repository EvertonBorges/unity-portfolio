using UnityEngine;
using TMPro;

public class UI_Settings_Dropdown : MonoBehaviour
{
    
    [SerializeField] private InspectorEvent _event;
    [SerializeField] private TMP_Dropdown _dropdown;

    public void Setup(int value)
    {
        _dropdown.onValueChanged.RemoveAllListeners();
        _dropdown.onValueChanged.AddListener(DPD_ChangeValue);

        _dropdown.SetValueWithoutNotify(value);
    }

    private void DPD_ChangeValue(int index)
    {
        _event.Notify(index);
    }

}
