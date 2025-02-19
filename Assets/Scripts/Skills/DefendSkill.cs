using UnityEngine;

[System.Serializable]
public class DefendSkill : SkillBase, ISkill
{
    public DefendSkill() : base() { }

    public DefendSkill(int id, string name, int coinCount, int basePower, int coinPower, EffectTiming effectTiming)
        : base(id, name, coinCount, basePower, coinPower, effectTiming) { }

    public override void UseSkill(Character user, Character target, int totalCoinPower = 0)
    {
#if UNITY_EDITOR
        Debug.Log($"<color=blue>{user.GetName} の DEF が {totalCoinPower} 上昇！</color>");
#endif

        user.AddDEF(totalCoinPower);
        ActivateSkillEffect(user, target, EffectTiming.OnDefend, totalCoinPower); // 防御時（青）
    }

    public override void ApplySkillEffect(Character user, Character target, int totalCoinPower)
    {
#if UNITY_EDITOR
        Debug.Log($"<color=yellow>{user.GetName} の ATK が上昇！</color>");
#endif
        user.AddATK(totalCoinPower);
    }
}
