using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    [SerializeField] private VoidEventChannelSO _onMusicBeatEvent;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _onMusicBeatEvent.OnEventRaised += BeatScaling;
    }

    private void OnDisable()
    {
        _onMusicBeatEvent.OnEventRaised -= BeatScaling;
    }

    public void BeatScaling()
    {
        _animator.SetTrigger("MusicBeat");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerCubes"))
        {
            GameManager.Instance.LostGame();
        }
    }
}
