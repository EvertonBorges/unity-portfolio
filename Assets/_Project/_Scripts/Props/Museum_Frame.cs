using UnityEngine;
using TMPro;
using System.Text;

public class Museum_Frame : MonoBehaviour
{

    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private TextMeshProUGUI _text;

    [Header("Frame Infos")]
    [SerializeField] private Texture2D _frame;
    [SerializeField] private string _studio;
    [SerializeField] private string _title;
    [SerializeField] private string _description;

    private readonly StringBuilder text = new();

    void Awake()
    {
        UpdateFrame();
    }

    private void UpdateFrame()
    {
        _meshRenderer.material.mainTexture = _frame;

        text.Clear();
        if (!_studio.IsEmpty()) text.Append($"<b>Studio:</b> <size=0.02>{_studio}</size>\n");
        if (!_title.IsEmpty()) text.Append($"<b>Game:</b> <size=0.02>{_title}</size>\n");
        if (!_description.IsEmpty()) text.Append($"<b>Description:</b> <size=0.02>{_description}</size>");

        _text.SetText(text);
    }

}
