using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "SO/Events/Bool Event Channel")]
public class BoolEventChannelSO : ScriptableObject
{
    #region Events

    public UnityAction<bool> OnEventRaised;

    #endregion

    #region Public Methods

    public void RaiseEvent(bool value)
    {
        if (OnEventRaised != null)
            OnEventRaised.Invoke(value);
    }

    #endregion
}
