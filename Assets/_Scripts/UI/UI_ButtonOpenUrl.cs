using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UI_ButtonOpenUrl : MonoBehaviour
{

    [SerializeField] private Button _button;
    [SerializeField] private string _link;

    private void Awake()
    {
        _button ??= GetComponent<Button>();

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(OpenLink);
    }

    private void OpenLink()
    {
        Application.OpenURL(_link);
    }

}
