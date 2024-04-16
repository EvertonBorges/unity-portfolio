using UnityEngine;
using AYellowpaper.SerializedCollections;

[CreateAssetMenu(fileName = "SO_Translation", menuName = "Data/Translations", order = 0)]
public class SO_Translations : ScriptableObject
{
    
    public SerializedDictionary<string, SO_Translation> translations;

}
