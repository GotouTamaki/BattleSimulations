using UnityEngine;

[System.Serializable]
public abstract class SkillBase : ISkill
{
    [SerializeField] protected int _id = 1;
    [SerializeField] protected string _name;
    [SerializeField] protected int _coinCount = 1;
    [SerializeField] protected int _basePower;
    [SerializeField] protected int _coinPower;
    [SerializeField] protected EffectTiming _effectTiming;

    public SkillBase() { }

    public SkillBase(int id, string name, int coinCount, int basePower, int coinPower, EffectTiming effectTiming)
    {
        _id = id;
        _name = name;
        _coinCount = coinCount;
        _basePower = basePower;
        _coinPower = coinPower;
        _effectTiming = effectTiming;
    }

    int ISkill.GetId => _id;

    string ISkill.GetName => _name;

    int ISkill.GetCoinCount => _coinCount;

    int ISkill.GetBasePower => _basePower;

    int ISkill.GetCoinPower => _coinPower;

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

    public abstract void UseSkill(Character user, Character target, int totalCoinPower = 0);

    public virtual void ActivateSkillEffect(Character user, Character target, EffectTiming timing, int totalCoinPower = 0)
    {
        if (_effectTiming == timing)
        {
#if UNITY_EDITOR
            Debug.Log(GetEffectTimingColor(timing) + $"{user.GetName} ÇÃÉXÉLÉã {_name} ÇÃå¯â î≠ìÆÅI" + "</color>");
#endif

            ApplySkillEffect(user, target, totalCoinPower);
        }
    }

    public abstract void ApplySkillEffect(Character user, Character target, int totalCoinPower = 0);

#if UNITY_EDITOR
    protected string GetEffectTimingColor(EffectTiming timing)
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
