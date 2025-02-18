using UnityEngine;

public class NoneSkill : ISkill
{
    private string _name;
    private int _coinCount;
    private int _basePower;
    private int _coinPower;
    private EffectTiming _effectTiming = EffectTiming.None;

    public string GetName => _name;

    public int GetCoinCount => _coinCount;

    public int GetBasePower => _basePower;

    public int GetCoinPower => _coinPower;

    public EffectTiming GetEffectTiming => _effectTiming;

    public int TossCoins()
    {
#if UNITY_EDITOR
        Debug.Log("none");
#endif
        return 0;
    }

        public void UseSkill(Character user, Character target, int coinPower)
    {
#if UNITY_EDITOR
        Debug.Log("none");
#endif
    }

    public void ActivateSkillEffect(Character user, Character target, EffectTiming timing)
    {
#if UNITY_EDITOR
        Debug.Log("none");
#endif
    }

    public void ApplySkillEffect(Character user, Character target)
    {
#if UNITY_EDITOR
        Debug.Log("none");
#endif
    }
}
