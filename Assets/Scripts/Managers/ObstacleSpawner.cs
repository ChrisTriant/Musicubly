using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    #region Fields

    [SerializeField] private Vector3 _spawnPoint;
    [SerializeField] private GameObject _obstaclePrefab; 

    #endregion

    #region LifeCycle


    #endregion

    #region Public Methods
    
    public GameObject SpawnNewObstacle()
    {
        return Instantiate(_obstaclePrefab, _spawnPoint, Quaternion.identity);
    }

    #endregion
}
