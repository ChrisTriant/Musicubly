using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaylistPlayer : MonoBehaviour
{
    #region Events

    [SerializeField] private AudioClipEventChannelSO _onClipChanged;

    #endregion

    #region Fields

    [SerializeField] private PlaylistSO _playlist;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private bool _playOnStart = true;

    private Coroutine _playCoroutine;
    private int _trackIndex = - 1;
    private float _clipEndTime;

    #endregion

    #region LifeCycle

    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        if (_playOnStart)
            Play();
    }

    #endregion

    #region Public Methods

    public void Play()
    {
        _playCoroutine = StartCoroutine(PlayNextClip());
    }

    public void Pause()
    {
        if(_audioSource.isPlaying)
            StopCoroutine(_playCoroutine);
    }

    public void Next()
    {
        _trackIndex = (_trackIndex + 1) % _playlist.Songs.Count;
        SetupTrackClip(_trackIndex);
    }

    public void Previous()
    {
        if (--_trackIndex < 0)
            _trackIndex = 0;

        SetupTrackClip(_trackIndex);
    }

    #endregion

    #region Private Methods

    public void Initialize()
    {
        if(_playlist.Songs.Count == 0)
        {
            Debug.LogWarning("Empty playlist.");
            return;
        }

        _trackIndex = -1;
    }

    private IEnumerator PlayNextClip()
    {
        while (true)
        {
            // Wait until the current clip finishes playing
            while (Time.time < _clipEndTime)
            {
                yield return null;
            }

            Next();
        }
    }

    private void SetupTrackClip(int index, bool autoplay = true)
    {
        if(index < 0 || index >= _playlist.Songs.Count)
        {
            Debug.LogWarning("Index outside of playlist size.");
            return;
        }
        _audioSource.clip = _playlist.Songs[index];
        _onClipChanged.RaiseEvent(_playlist.Songs[index]);

        // Calculate when the clip will finish
        _clipEndTime = Time.time + _audioSource.clip.length;

        if (autoplay)
            PlayTrack();
    }

    private void PlayTrack()
    {
        Debug.Log("Playing track: " + _playlist.Songs[_trackIndex].name);
        _audioSource.Play();
    }

    #endregion
}
