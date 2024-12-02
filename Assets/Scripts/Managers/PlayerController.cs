using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    #region Fields

    [SerializeField] List<GameObject> _cubes;

    #endregion

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            transform.Rotate(new(0, 0, 90), Space.World);
        }
    }

    public void EnableCubesByIndices(int[] indices)
    {    
        foreach(var  cube in _cubes)
        {
            cube.SetActive(false);
        }

        foreach(int idx in indices)
        {
            _cubes[idx].SetActive(true);
        }

        SetRandomRotation();
    }

    #region Private Methods

    private void SetRandomRotation()
    {
        int times = Random.Range(1, 4);

        transform.Rotate(new(0, 0, times * 90), Space.World);
    }

    #endregion
}
