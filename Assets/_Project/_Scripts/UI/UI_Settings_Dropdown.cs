using UnityEngine;

public class UI_Settings_Dropdown : MonoBehaviour
{
    
    [SerializeField] private InspectorEvent _event;

    public void DPD_ChangeValue(int index)
    {
        _event.Notify(index);
    }

}
