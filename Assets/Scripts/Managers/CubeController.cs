using UnityEngine;

public class CubeController : MonoBehaviour
{
    #region Fields

    [SerializeField] private VoidEventChannelSO _onMusicBeatEvent;
    private Animator _animator;

    #endregion

    #region LifeCycle

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerCubes"))
        {
            GameManager.Instance.LostGame();
        }
    }

    #endregion

    #region Private Methods

    private void BeatScaling()
    {
        _animator.SetTrigger("MusicBeat");
    }

    #endregion
}
