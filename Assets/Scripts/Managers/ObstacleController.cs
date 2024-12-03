using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObstacleController : MonoBehaviour
{
    #region Events

    public event Action<ObstacleController> OnHitEnd = delegate { };

    #endregion

    #region Fields

    [SerializeField] List<GameObject> _cubes;
    private float _moveSpeed = 0;
    private UnityAction _onObstaclePassCallback;

    #endregion

    #region LifeCycle

    private void Update()
    {
        //The obstacles will be moving towards the world origin where Musicuber is placed.
        float newZPos = transform.position.z - _moveSpeed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, transform.position.y, newZPos);
    }

    #endregion

    #region Public Methods

    public void SetObstacleMoveSpeed(float speed)
    {
        _moveSpeed = speed;
    }

    public int[] DisableRandomCubes(UnityAction _onObstaclPassed)
    {
        _onObstaclePassCallback = _onObstaclPassed;

        int cubeNumber = UnityEngine.Random.Range(1, 8);
        List<int> cubeIndices = new List<int>();
        for(int i = 0; i < cubeNumber; i++)
        {
            int cubeIdx = UnityEngine.Random.Range(0, 9);
            if (cubeIndices.Contains(cubeIdx))
            {
                --i;
                continue;
            }
            _cubes[cubeIdx].SetActive(false);
            cubeIndices.Add(cubeIdx);
        }
        return cubeIndices.ToArray();
    }

    public void ResetCubes()
    {
        for (int i = 0; i < _cubes.Count; i++)
        {
            _cubes[i].SetActive(true);
        }
    }

    #endregion

    #region Private Methods

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EndOfObstacles"))
        {
            OnHitEnd.Invoke(this); 
            //Destroy(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _onObstaclePassCallback?.Invoke();
        }
    }

    #endregion
}
