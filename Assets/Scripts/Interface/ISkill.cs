public interface ISkill
{
    public int GetId { get; }

    public string GetName { get; }

    public int GetCoinCount { get; }

    public int GetBasePower { get; }

    public int GetCoinPower { get; }

    public EffectTiming GetEffectTiming { get; }

    public int TossCoins();

    public void UseSkill(Character user, Character target, int coinPower = 0);

    public void ActivateSkillEffect(Character user, Character target, EffectTiming timing, int totalCoinPower = 0);

    public void ApplySkillEffect(Character user, Character target, int totalCoinPower);
}
