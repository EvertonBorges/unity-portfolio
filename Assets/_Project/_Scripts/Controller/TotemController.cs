using System.IO;
using UnityEngine;
using UnityEngine.Video;
using TMPro;

[RequireComponent(typeof(VideoPlayer))]
public class TotemController : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private TextMeshProUGUI _txtTitle;
    [SerializeField] private TextMeshProUGUI _txtDescription;

    [Header("Totem Infos")]
    [SerializeField] private string _title;
    [SerializeField] private string _description;

    [Header("Video Parameters")]
    [SerializeField] private string _videoName;

    [Header("Events")]
    [SerializeField] private InspectorEvent _activeEvent;
    [SerializeField] private InspectorEvent _desactiveEvent;

    void Start()
    {
        if (_videoPlayer == null) _videoPlayer = GetComponent<VideoPlayer>();

        LoadVideo();
        LoadTotemInfos();
        
        HandleEvents();
        DesactiveEvent();

        Manager_Events.UI.OnChangeLanguage += OnChangeLanguage;
    }

    private void LoadVideo()
    {
        var path = Path.Combine(Application.dataPath, "Resources", "Videos", _videoName);
        _videoPlayer.url = path;
    }

    private void LoadTotemInfos()
    {
        _txtTitle.SetText(_title);
        _txtDescription.SetText(Manager_Translation.Instance.ConvertText(_description));
    }

    private void OnChangeLanguage()
    {
        _txtDescription.SetText(Manager_Translation.Instance.ConvertText(_description));
    }

    private void HandleEvents()
    {
        Manager_Events.Add(_activeEvent, ActiveEvent);
        Manager_Events.Add(_desactiveEvent, DesactiveEvent);
    }

    public void PlayVideo()
    {
        _videoPlayer.Play();
    }

    public void StopVideo()
    {
        _videoPlayer.Stop();
        _videoPlayer.targetTexture.Release();
    }

    private void ActiveEvent()
    {
        _videoPlayer.Prepare();
    }

    private void DesactiveEvent()
    {
        StopVideo();
    }

    void OnDestroy()
    {
        Manager_Events.Remove(_activeEvent, ActiveEvent);
        Manager_Events.Remove(_desactiveEvent, DesactiveEvent);

        Manager_Events.UI.OnChangeLanguage -= OnChangeLanguage;
    }

}
