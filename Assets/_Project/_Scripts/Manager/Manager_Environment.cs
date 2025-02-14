using AYellowpaper.SerializedCollections;
using UnityEngine;

public class Manager_Environment : MonoBehaviour
{

    public enum SpaceEnum
    {
        EXTERNAL, MUSEUM
    }

    public SerializedDictionary<SpaceEnum, GameObject[]> _objects;

    private void EnableExternalArea()
    {
        EnableObjects(SpaceEnum.EXTERNAL, true);
        DisableMuseum();
    }

    private void EnableMuseum()
    {
        EnableObjects(SpaceEnum.MUSEUM, true);
        DisableExternalArea();
    }

    private void DisableExternalArea()
    {
        EnableObjects(SpaceEnum.EXTERNAL, false);
    }

    private void DisableMuseum()
    {
        EnableObjects(SpaceEnum.MUSEUM, false);
    }

    private void EnableObjects(SpaceEnum spaceEnum, bool enable)
    {
        foreach (var obj in _objects)
        {
            if (obj.Key != spaceEnum)
                continue;

            foreach (var gameObj in obj.Value)
                gameObj.SetActive(enable);
        }
    }

    private void OnEnable()
    {
        Manager_Events.GameManager.Area.EnableExternalArea += EnableExternalArea;
        Manager_Events.GameManager.Area.EnableMuseum += EnableMuseum;
    }

    private void OnDisable()
    {
        Manager_Events.GameManager.Area.EnableExternalArea -= EnableExternalArea;
        Manager_Events.GameManager.Area.EnableMuseum -= EnableMuseum;
    }

}
