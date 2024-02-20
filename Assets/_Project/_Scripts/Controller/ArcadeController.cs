using UnityEngine;

public class ArcadeController : MonoBehaviour
{

    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Color _color;
    [Range(-10f, 10f)] [SerializeField] private float _intensity = 0f;

    private Material[] m_materials;

    void Awake()
    {
        m_materials = _meshRenderer.materials;

        var color = _color * Mathf.Pow(2f, _intensity);

        m_materials[1].SetColor("_EmissionColor", color);
        m_materials[3].SetColor("_EmissionColor", color);
    }

}
