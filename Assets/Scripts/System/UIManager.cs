using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text _allySkillText;
    [SerializeField] private Text _allySkillCoinPowerText;
    [SerializeField] private Text _enemySkillText;
    [SerializeField] private Text _enemySkillCoinPowerText;
    [SerializeField] private Text _resultText;
    [SerializeField] private GameObject _restartButton;

    public Text GetAllySkillText => _allySkillText;

    public Text GetAllySkillCoinPowerText => _allySkillCoinPowerText;

    public Text GetEnemySkillText => _enemySkillText;

    public Text GetEnemySkillCoinPowerText => _enemySkillCoinPowerText;

    public Text GetResultText => _resultText;

    public GameObject GetRestartButton => _restartButton;

    public void SetAllySkillText(string text)
    {
        _allySkillText.text = text;
    }

    public void SetAllySkillCoinPowerText(string text)
    {
        _allySkillCoinPowerText.text = text;
    }

    public void SetEnemySkillText(string text)
    {
        _enemySkillText.text = text;
    }

    public void SetEnemySkillCoinPowerText(string text)
    {
        _enemySkillCoinPowerText.text = text;
    }

    public void SetResultText(string text)
    {
        _resultText.text = text;
    }

    public void SetEnableRestartButton(bool flag)
    {
        _restartButton.SetActive(flag);
    }
}
