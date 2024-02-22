using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Dialog", menuName = "Data/Dialog", order = 0)]
public class SO_Dialogs : ScriptableObject
{

    public string title;
    public SO_Dialog[] dialogs;

}