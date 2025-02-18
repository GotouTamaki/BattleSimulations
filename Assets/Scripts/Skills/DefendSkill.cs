using UnityEngine;

[System.Serializable]
public class DefendSkill : ISkill
{
    [SerializeField] private string _name;
    [SerializeField] private int _coinCount;
    [SerializeField] private int _basePower;
    [SerializeField] private int _coinPower;
    [SerializeField] private EffectTiming _effectTiming;

    public DefendSkill(string name, int coinCount, int basePower, int coinPower, EffectTiming effectTiming)
    {
        _name = name;
        _coinCount = coinCount;
        _basePower = basePower;
        _coinPower = coinPower;
        _effectTiming = effectTiming;
    }

    string ISkill.GetName => _name;

    int ISkill.GetCoinCount => _basePower;

    int ISkill.GetBasePower => _coinPower;

    int ISkill.GetCoinPower => _basePower;

    EffectTiming ISkill.GetEffectTiming => _effectTiming;

    public int TossCoins()
    {
        int totalCoinPower = _basePower;

        for (int i = 0; i < _coinCount; i++)
        {
            if (Random.value > 0.5f) totalCoinPower += _coinPower;
        }

        return totalCoinPower;
    }

    public void UseSkill(Character user, Character target, int totalCoinPower)
    {
#if UNITY_EDITOR
        Debug.Log($"<color=blue>{user.GetName} の DEF が {totalCoinPower} 上昇！</color>");
#endif

        user.AddDEF(totalCoinPower);
        ActivateSkillEffect(user, target, EffectTiming.OnDefend); // 防御時（青）
    }

    public void ActivateSkillEffect(Character user, Character target, EffectTiming timing)
    {
        if (_effectTiming == timing)
        {
#if UNITY_EDITOR
            Debug.Log(GetColor(timing) + $"{user.GetName} のスキル {_name} の効果発動！" + "</color>");
#endif

            ApplySkillEffect(user, target);
        }
    }

    public void ApplySkillEffect(Character user, Character target)
    {
#if UNITY_EDITOR
        Debug.Log($"<color=yellow>{user.GetName} の ATK が上昇！</color>");
#endif
        user.AddATK(1);
    }

#if UNITY_EDITOR
    private string GetColor(EffectTiming timing)
    {
        return timing switch
        {
            EffectTiming.TurnStart => "<color=red>",
            EffectTiming.BeforeMatch => "<color=orange>",
            EffectTiming.MatchWin => "<color=yellow>",
            EffectTiming.MatchLose => "<color=green>",
            EffectTiming.OnHit => "<color=purple>",
            EffectTiming.OnDefend => "<color=blue>",
            EffectTiming.MatchEnd => "<color=cyan>",
            _ => "<color=white>"
        };
    }
#endif
}
