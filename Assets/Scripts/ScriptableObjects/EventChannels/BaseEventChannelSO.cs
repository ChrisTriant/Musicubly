using UnityEngine;
using UnityEngine.Events;

public class BaseEventChannelSO<T> : ScriptableObject
{
    #region Events

    public UnityAction<T> OnEventRaised;

    #endregion

    #region Public Methods

    public void RaiseEvent(T value)
    {
        if (OnEventRaised != null)
            OnEventRaised.Invoke(value);
    }

    #endregion
}
