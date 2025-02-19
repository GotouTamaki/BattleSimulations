using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text _allySkillText;
    [SerializeField] private Text _enemySkillText;
    [SerializeField] private Text _resultText;

    public Text GetAllySkillText => _allySkillText;

    public Text GetEnemySkillText => _enemySkillText;

    public Text GetResultText => _resultText;

    public void SetAllySkillText(string text)
    {
        _allySkillText.text = text;
    }

    public void SetEnemySkillText(string text)
    {
        _enemySkillText.text = text;
    }

    public void SetResultText(string text)
    {
        _resultText.text = text;
    }
}
