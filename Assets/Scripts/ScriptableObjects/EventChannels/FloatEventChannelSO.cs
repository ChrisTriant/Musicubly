using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "SO/Events/Float Event Channel")]
public class FloatEventChannelSO : ScriptableObject
{
    #region Events

    public UnityAction<float> OnEventRaised;

    #endregion

    #region Public Methods

    public void RaiseEvent(float value)
    {
        if (OnEventRaised != null)
            OnEventRaised.Invoke(value);
    }

    #endregion
}
