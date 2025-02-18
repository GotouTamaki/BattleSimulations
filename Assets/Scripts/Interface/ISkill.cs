public interface ISkill
{
    public string GetName { get; }

    public int GetCoinCount { get; }

    public int GetBasePower { get; }

    public int GetCoinPower { get; }

    public EffectTiming GetEffectTiming { get; }

    public int TossCoins();

    public void UseSkill(Character user, Character target, int coinPower);

    public void ActivateSkillEffect(Character user, Character target, EffectTiming timing);

    public void ApplySkillEffect(Character user, Character target);
}
