using UnityEngine;

[CreateAssetMenu(fileName = "SO_Sound", menuName = "Data/Sounds", order = 1)]
public class SO_Sound : ScriptableObject
{
    
    public SO_SoundType type = SO_SoundType.SFX;
    public AudioClip[] clip = null;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0f, 10f)] public float pitch = 1f;
    public bool loop = false;
    public bool spatialBlend = false;

    public void Play()
    {
        Manager_Events.Sound.OnPlay.Notify(this);
    }

}
