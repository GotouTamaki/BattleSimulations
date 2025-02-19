using UnityEngine;

[CreateAssetMenu(fileName = "CharacterTeamData", menuName = "Scriptable Objects/CharacterTeamData")]
public class CharacterTeamData : ScriptableObject
{
    [SerializeField] Character[] _characters;
}
