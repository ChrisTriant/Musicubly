using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Fields

    [SerializeField] private ObstacleSpawner _spawner;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private GameObject StartUI;
    [SerializeField] private GameObject PauseUI;
    [SerializeField] private GameObject InGameUI;
    [SerializeField] private GameObject LoseScreenUI;
    [SerializeField] private TMP_Text _scoreTextField;

    [SerializeField] private float _obstacleSpawningInterval = 2;
    [SerializeField] private float _minObstacleSpeed = 5;
    [SerializeField] private float _obstacleSpeed = 5;
    [SerializeField] private float _maxObstacleSpeed = 10;
    [SerializeField] private float _obstacleSpeedAcceleration = 0.5f;
    [SerializeField] private bool _isPaused = true;

    private List<int[]> _nextCubeIndices;

    private Coroutine _gameCoroutine;

    #endregion

    #region Properties

    public static GameManager Instance { get; private set; }
    public int Score { get; private set; } = 0;
    public int Level { get; private set; } = 1;

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

    private void Start()
    {
        StartUI.SetActive(true);
        InGameUI.SetActive(false);
        PauseUI.SetActive(false);
        LoseScreenUI.SetActive(false);
        _obstacleSpeed = _minObstacleSpeed;
    }

    public void StartGame()
    {
        Score = 0;
        _scoreTextField.text = Score.ToString();
        Level = 1;
        _isPaused = false;
        StartUI.SetActive(false);
        InGameUI.SetActive(true);
        PauseUI.SetActive(false);
        _nextCubeIndices = new();

        _gameCoroutine = StartCoroutine(SpawnObstacleWaves());
    }

    public void PauseGame()
    {
        _isPaused = true;
        PauseUI.SetActive(true);
        InGameUI.SetActive(false);
    }

    public void ResumeGame()
    {
        _isPaused = false;
        PauseUI.SetActive(false);
        InGameUI.SetActive(true);

        var obstacles = FindObjectsOfType<ObstacleController>();
        foreach (var obstacle in obstacles)
        {
            obstacle.SetObstacleMoveSpeed(_obstacleSpeed);
        }
    }

    public void LostGame()
    {
        StopAllObstacles();
        LoseScreenUI.SetActive(true);
    }

    public void Restart()
    {        
        LoseScreenUI.SetActive(false);
        _spawner.ResetObstacles();
        
        StopCoroutine(_gameCoroutine);
        StartGame();
    }

    #endregion

    #region Private Methods

    private IEnumerator SpawnObstacleWaves()
    {
        yield return new WaitForSeconds(0.4f);
        var obstacleController = _spawner.SpawnNewObstacle();
        obstacleController.SetObstacleMoveSpeed(_obstacleSpeed);
        var disabledIndices = obstacleController.DisableRandomCubes(OnObstaclePassed);

        //The indices list is empty so we just update the player.
        _nextCubeIndices.Add(disabledIndices);
        _playerController.EnableCubesByIndices(disabledIndices);

        yield return new WaitForSeconds(_obstacleSpawningInterval);

        while (true)
        {
            if (!_isPaused)
            {                
                obstacleController = _spawner.SpawnNewObstacle();
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
        _scoreTextField.text = Score.ToString();
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
        var obstacles = FindObjectsOfType<ObstacleController>();
        foreach (var obstacle in obstacles)
        {
            obstacle.SetObstacleMoveSpeed(0);
        }
    }

    #endregion
}
