using UnityEngine;

[CreateAssetMenu(fileName = "SO_Sound", menuName = "Data/Sounds", order = 0)]
public class SO_Sound : ScriptableObject
{
    
    public SO_SoundType type = SO_SoundType.SFX;
    public AudioClip[] clip = null;
    public float volume = 1f;
    public float pitch = 1f;
    public bool loop = false;
    public bool spatialBlend = false;

}
