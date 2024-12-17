using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Events

    public static event Action<int> OnScoreChanged;
    public static event Action OnDefeat;
    public static event Action OnGameStart;
    public static event Action OnGamePause;
    public static event Action OnGameResumed;

    #endregion

    #region Fields

    [SerializeField] private ObstacleManager _obstacleManager;
    [SerializeField] private PlayerController _playerController;

    [SerializeField] private float _obstacleSpawningInterval = 2;
    [SerializeField] private float _minObstacleSpeed = 5;
    [SerializeField] private float _obstacleSpeed = 5;
    [SerializeField] private float _maxObstacleSpeed = 10;
    [SerializeField] private float _obstacleSpeedAcceleration = 0.5f;

    private int _score;

    private List<int[]> _nextCubeIndices;

    private Coroutine _gameCoroutine;

    #endregion

    #region Properties

    public static GameManager Instance { get; private set; }
    public int Score 
    {
        get => _score;
        private set
        {
            _score = value;
            OnScoreChanged?.Invoke(_score);
        }
    }

    public bool IsPaused { get; private set; } = true;


    #endregion

    #region Public Methods

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void StartGame()
    {
        Score = 0;
        OnGameStart?.Invoke();

        IsPaused = false;
        _nextCubeIndices = new();

        _obstacleSpeed = _minObstacleSpeed;
        _gameCoroutine = StartCoroutine(SpawnObstacleWaves());
    }

    public void PauseGame()
    {
        IsPaused = true;
        _obstacleManager.FreezeObstacles();
        OnGamePause?.Invoke();
    }

    public void ResumeGame()
    {
        IsPaused = false;
        _obstacleManager.SetObstacleSpeed(_obstacleSpeed);
        OnGameResumed?.Invoke();
    }

    public void LostGame()
    {
        StopAllObstacles();
        OnDefeat?.Invoke();
    }

    public void RestartGame()
    {
        _obstacleManager.ResetObstacles();
        
        StopCoroutine(_gameCoroutine);
        StartGame();
    }

    #endregion

    #region Private Methods

    private IEnumerator SpawnObstacleWaves()
    {
        yield return new WaitForSeconds(0.4f);
        var obstacleController = _obstacleManager.SpawnNewObstacle();
        obstacleController.SetObstacleMoveSpeed(_obstacleSpeed);
        var disabledIndices = obstacleController.DisableRandomCubes(OnObstaclePassed);

        //The indices list is empty so we just update the player.
        _nextCubeIndices.Add(disabledIndices);
        _playerController.EnableCubesByIndices(disabledIndices);

        yield return new WaitForSeconds(_obstacleSpawningInterval);

        while (true)
        {
            if (!IsPaused)
            {                
                obstacleController = _obstacleManager.SpawnNewObstacle();
                obstacleController.SetObstacleMoveSpeed(_obstacleSpeed);
                disabledIndices = obstacleController.DisableRandomCubes(OnObstaclePassed);

                //If the indices list has stored values, keep on adding. If not update the player with the new indices.

                if (_nextCubeIndices.Count != 0)
                {
                    _nextCubeIndices.Add(disabledIndices);
                }
                else
                {
                    _nextCubeIndices.Add(disabledIndices);
                    _playerController.EnableCubesByIndices(_nextCubeIndices[0]);
                }                 

                yield return new WaitForSeconds(_obstacleSpawningInterval);
            }
            else
            {
                yield return null;
            }
        }
    }

    private void OnObstaclePassed()
    {
        Score++;
        ReceiveNewIndices();

        if (Score % 10 == 0)
            IncreaseObstacleSpeed();
    }

    private void ReceiveNewIndices()
    {
        if (_nextCubeIndices.Count > 1)
        {
            _playerController.EnableCubesByIndices(_nextCubeIndices[1]);
            _nextCubeIndices.RemoveAt(0);
        }
        else if (_nextCubeIndices.Count == 1)
        {
            _nextCubeIndices.RemoveAt(0);
        }
    }

    private void IncreaseObstacleSpeed()
    {
        if(_obstacleSpeed < _maxObstacleSpeed)
        _obstacleSpeed += _obstacleSpeedAcceleration;
    }

    private void StopAllObstacles()
    {
        StopCoroutine(_gameCoroutine);
        _obstacleManager.FreezeObstacles();
    }

    #endregion
}
