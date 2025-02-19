using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using System.Text;
#endif

[System.Serializable]
public class Character
{
    [SerializeField] private int _id;
    [SerializeField] private string _name;
    [SerializeField] private int _hp;
    [SerializeField] private int _atk;
    [SerializeField] private int _def;
    [SerializeField] private int _spd;
    [SerializeField] private bool _isAlly;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private string[] _animationName;
    [SerializeField, SerializeReference, SubclassSelector] private List<ISkill> _skills = new List<ISkill>();

    // 事前に設定するスキル & ターゲット
    private ISkill _selectedSkill;
    private Character _target;

    private GameObject _sceneGameObject;
    private Transform _sceneTransform;

    public Queue<Buff> Buffs = new Queue<Buff>();

    public int GetId => _id;

    public string GetName => _name;

    public int GetHP => _hp;

    public int GetATK => _atk;

    public int GetDEF => _def;

    public int GetSPD => _spd;

    public bool GetIsAlly => _isAlly;

    public GameObject GetPrefab => _prefab;

    public GameObject GetSceneGameObject => _sceneGameObject;

    public string[] GetAnimationName => _animationName;

    public Transform GetTransform => _sceneTransform;

    public List<ISkill> GetSkills => _skills;

    public ISkill GetSelectedSkill => _selectedSkill;

    public Character GetTarget => _target;

    // 変更前のステータス
    public int PrevHP { get; private set; }

    public int PrevATK { get; private set; }

    public int PrevDEF { get; private set; }

    public int PrevSPD { get; private set; }

    // 死亡判定
    public bool IsAlive => GetHP > 0;

    public Character() { }

    public Character(int id, string name, int hp, int atk, int def, int spd, List<ISkill> skills, string[] animationName)
    {
        _id = id;
        _name = name;
        _hp = hp;
        _atk = atk;
        _def = def;
        _spd = spd;
        _skills = skills;
        _animationName = animationName;

        SavePreviousStatus(); // 初期状態を記録
    }

    public void SetId(int id)
    {
        _id = id;
    }

    public void AddDamage(int damage)
    {
        _hp -= damage;
        _hp = Mathf.Max(0, _hp);
    }

    public void AddATK(int atk)
    {
        _atk += atk;
    }

    public void AddDEF(int def)
    {
        _def += def;
    }

    public void AddSPD(int spd)
    {
        _spd += spd;
    }

    public void SetIsAlly(bool isAlly)
    {
        _isAlly = isAlly;
    }

    public void SetAnimationName(int index, string animationName)
    {
        _animationName[index] = animationName;
    }

    public void SelectSkill(int skillNum)
    {
        _selectedSkill = GetSkills[skillNum];
    }

    public void SelectSkill(ISkill skill)
    {
        _selectedSkill = skill;
    }

    public void SelectTarget(List<Character> potentialTargets, int targetNum)
    {
        _target = potentialTargets[targetNum];
    }

    public void ApplyBuffs()
    {
        foreach (var buff in Buffs)
        {
            buff.ApplyEffect(this);
        }
    }

    public void RemoveExpiredBuffs()
    {
        Buffs = new Queue<Buff>(Buffs.Where(buff => !buff.ShouldRemove()));
    }

    public void SavePreviousStatus()
    {
        PrevHP = GetHP;
        PrevATK = GetATK;
        PrevDEF = GetDEF;
        PrevSPD = GetSPD;
    }

#if UNITY_EDITOR
    public void DisplayStatusDifference()
    {
        string statusLog = $"{GetName} のステータス: ";
        statusLog += $"ID: {GetId}";
        statusLog += CompareStatus("HP", PrevHP, GetHP);
        statusLog += CompareStatus("ATK", PrevATK, GetATK);
        statusLog += CompareStatus("DEF", PrevDEF, GetDEF);
        statusLog += CompareStatus("SPD", PrevSPD, GetSPD);

        Debug.Log(statusLog);
    }

    private string CompareStatus(string statName, int prevValue, int currentValue)
    {
        if (prevValue != currentValue)
        {
            return $" {statName}: <color=red>{prevValue} → {currentValue}</color>";
        }
        return $" {statName}: {currentValue}";
    }
#endif

#if UNITY_EDITOR
    public string GetCharacterInfo()
    {
        StringBuilder info = new StringBuilder();

        info.AppendLine($"{GetName} (ID: {GetId}, HP: {GetHP}, ATK: {GetATK}, DEF: {GetDEF}, SPD: {GetSPD})");
        foreach (var skill in GetSkills)
        {
            info.AppendLine($"  - {skill.GetName} (ID: {skill.GetId}, コイン数: {skill.GetCoinCount}, 基礎威力: {skill.GetBasePower}, コイン威力: {skill.GetCoinPower})");
        }

        return info.ToString();
    }
#endif
}
