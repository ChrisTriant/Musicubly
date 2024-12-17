using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Fields

    [Header("Panels")]
    [SerializeField] private GameObject StartUI;
    [SerializeField] private GameObject PauseUI;
    [SerializeField] private GameObject InGameUI;
    [SerializeField] private GameObject LoseScreenUI;

    [SerializeField] private List<TMP_Text> _scoreTexts;

    [SerializeField] private Button _startButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _resumeButton;

    #endregion

    #region LifeCycle

    private void Start()
    {
        BindEvents();
        StartUI.SetActive(true);
        InGameUI.SetActive(false);
        PauseUI.SetActive(false);
        LoseScreenUI.SetActive(false);
    }

    private void OnDestroy()
    {
        UnbindEvents();
    }

    #endregion

    #region Private Methods

    private void BindEvents()
    {
        BindGameMangerEvents();
        BindButtonEvents();
    }

    private void UnbindEvents()
    {
        UnbindGameManagerEvents();
        UnbindButtonEvents();
    }

    private void BindGameMangerEvents()
    {
        GameManager.OnScoreChanged += HandleScoreChange;
        GameManager.OnGameStart += HandleGameStart;
        GameManager.OnGamePause += HandleGamePause;
        GameManager.OnGameResumed += HandleGameResumed;
        GameManager.OnDefeat += HandleDefeat;
    }

    private void UnbindGameManagerEvents()
    {
        GameManager.OnScoreChanged -= HandleScoreChange;
        GameManager.OnGameStart -= HandleGameStart;
        GameManager.OnGamePause -= HandleGamePause;
        GameManager.OnGameResumed -= HandleGameResumed;
        GameManager.OnDefeat -= HandleDefeat;
    }

    private void BindButtonEvents()
    {
        _startButton.onClick.AddListener(GameManager.Instance.StartGame);
        _restartButton.onClick.AddListener(GameManager.Instance.RestartGame);
        _pauseButton.onClick.AddListener(GameManager.Instance.PauseGame);
        _resumeButton.onClick.AddListener(GameManager.Instance.ResumeGame);
    }

    private void UnbindButtonEvents()
    {
        _startButton.onClick.RemoveListener(GameManager.Instance.StartGame);
        _restartButton.onClick.RemoveListener(GameManager.Instance.RestartGame);
        _pauseButton.onClick.RemoveListener(GameManager.Instance.PauseGame);
        _resumeButton.onClick.RemoveListener(GameManager.Instance.ResumeGame);
    }


    private void HandleScoreChange(int score)
    {
        foreach(var scoreText in _scoreTexts)
        {
            scoreText.text = $"{score}";
        }
    }

    private void HandleGameStart()
    {
        InGameUI.SetActive(true);
        StartUI.SetActive(false);
        PauseUI.SetActive(false);
        LoseScreenUI.SetActive(false);
    }

    private void HandleGamePause()
    {
        PauseUI.SetActive(true);
        InGameUI.SetActive(false);
    }

    private void HandleGameResumed()
    {
        PauseUI.SetActive(false);
        InGameUI.SetActive(true);
    }

    private void HandleDefeat()
    {
        PauseUI.SetActive(false);
        InGameUI.SetActive(false);
        StartUI.SetActive(false);
        LoseScreenUI.SetActive(true);
    }

    #endregion
}
