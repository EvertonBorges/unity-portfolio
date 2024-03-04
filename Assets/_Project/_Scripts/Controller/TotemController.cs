using System.IO;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class TotemController : MonoBehaviour
{

    [Header("Video Parameters")]
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private string _videoName;

    [Header("Events")]
    [SerializeField] private InspectorEvent _activeEvent;
    [SerializeField] private InspectorEvent _desactiveEvent;

    void Awake()
    {
        _videoPlayer ??= GetComponent<VideoPlayer>();

        var path = Path.Combine(Application.dataPath, "Resources", "Videos", _videoName);
        _videoPlayer.url = path;

        Manager_Events.Add(_activeEvent, ActiveEvent);
        Manager_Events.Add(_desactiveEvent, DesactiveEvent);

        DesactiveEvent();
    }

    private void ActiveEvent()
    {
        _videoPlayer.Play();
    }

    private void DesactiveEvent()
    {
        _videoPlayer.Stop();
    }

    void OnDestroy()
    {
        Manager_Events.Remove(_activeEvent, ActiveEvent);
        Manager_Events.Remove(_desactiveEvent, DesactiveEvent);
    }

}
