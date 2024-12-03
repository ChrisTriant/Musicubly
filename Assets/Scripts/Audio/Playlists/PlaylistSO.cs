using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Playlist")]
public class PlaylistSO : ScriptableObject
{
    [field: SerializeField] public List<AudioClip> Songs;
}
