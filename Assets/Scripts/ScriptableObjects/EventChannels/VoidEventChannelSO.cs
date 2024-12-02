using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// This class is used for Events that have no arguments (Example: Exit game event)
/// </summary>
[CreateAssetMenu(menuName = "SO/Events/Void Event Channel")]
public class VoidEventChannelSO : ScriptableObject
{
    #region Events

    public UnityAction OnEventRaised;

    #endregion

    #region Public Methods

    public void RaiseEvent()
    {
        if (OnEventRaised != null)
            OnEventRaised.Invoke();
    }

    #endregion
}

