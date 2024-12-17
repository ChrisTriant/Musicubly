using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    #region Fields

    [SerializeField] private Vector3 _spawnPoint;
    [SerializeField] private GameObject _obstaclePrefab;
    [SerializeField] private int _poolSize;

    private List<ObstacleController> _totalObstacleControllers = new();
    private Queue<ObstacleController> _availableObstacleControllers;

    #endregion

    #region LifeCycle

    private void Awake()
    {
        InitializePool();
    }

    #endregion

    #region Public Methods

    public ObstacleController SpawnNewObstacle()
    {
        var obstacleController = _availableObstacleControllers.Dequeue();
        obstacleController.ResetCubes();
        obstacleController.transform.SetPositionAndRotation(_spawnPoint, Quaternion.identity);
        obstacleController.gameObject.SetActive(true);
        return obstacleController;
    }

    public void ResetObstacles()
    {
        _availableObstacleControllers.Clear();

        for(int i = 0; i < _totalObstacleControllers.Count; i++)
        {
            var controller = _totalObstacleControllers[i];
            controller.ResetCubes();
            controller.gameObject.SetActive(false);
            _availableObstacleControllers.Enqueue(controller);
        }
    }

    public void FreezeObstacles()
    {
        foreach (var controller in _totalObstacleControllers)
        {
            controller.SetObstacleMoveSpeed(0);
        }
    }

    public void SetObstacleSpeed(float speed)
    {
        foreach (var controller in _totalObstacleControllers)
        {
            controller.SetObstacleMoveSpeed(speed);
        }
    }

    #endregion

    #region Private Methods

    private void InitializePool()
    {
        _availableObstacleControllers = new();
        for (int i = 0; i < _poolSize; i++)
        {
            var obstacle = Instantiate(_obstaclePrefab, _spawnPoint, Quaternion.identity);
            obstacle.transform.parent = transform;
            obstacle.SetActive(false);
            var obstacleController = obstacle.GetComponent<ObstacleController>();
            obstacleController.OnHitEnd += OnObstacleHitEnd;
            _totalObstacleControllers.Add(obstacleController);
            _availableObstacleControllers.Enqueue(obstacleController);
        }
    }

    private void OnObstacleHitEnd(ObstacleController obstacleController)
    {
        obstacleController.gameObject.SetActive(false);
        _availableObstacleControllers.Enqueue(obstacleController);
    }

    #endregion
}
