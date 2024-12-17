using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    #region Fields

    [SerializeField] List<GameObject> _cubes;

    #endregion

    #region LifeCycle

    private void Update()
    {
        if (GameManager.Instance.IsPaused) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            transform.Rotate(new(0, 0, 90), Space.World);
        }
    }

    #endregion

    #region Public Methods

    public void EnableCubesByIndices(int[] indices)
    {    
        foreach(var cube in _cubes)
        {
            cube.SetActive(false);
        }

        foreach(int idx in indices)
        {
            _cubes[idx].SetActive(true);
        }

        SetRandomRotation();
    }

    #endregion

    #region Private Methods

    private void SetRandomRotation()
    {
        int times = Random.Range(1, 4);

        transform.Rotate(new(0, 0, times * 90), Space.World);
    }

    #endregion
}
