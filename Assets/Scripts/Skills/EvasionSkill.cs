using UnityEngine;

[System.Serializable]
public class EvasionSkill : SkillBase, ISkill
{
    public EvasionSkill() : base() { }

    public EvasionSkill(int id, string name, int coinCount, int basePower, int coinPower, EffectTiming effectTiming)
        : base(id, name, coinCount, basePower, coinPower, effectTiming) { }

    public override void UseSkill(Character user, Character target, int totalCoinPower)
    {
        if (totalCoinPower > target.GetATK)
        {
#if UNITY_EDITOR
            Debug.Log($"<color=blue>{user.GetName} は攻撃を回避した！</color>");
#endif
            ActivateSkillEffect(user, target, EffectTiming.OnDefend, totalCoinPower); // 回避成功時（青）
        }
#if UNITY_EDITOR
        else
        {
            Debug.Log($"<color=green>{user.GetName} は回避に失敗した！</color>");
        }
#endif
    }

    public override void ApplySkillEffect(Character user, Character target, int totalCoinPower)
    {
#if UNITY_EDITOR
        Debug.Log($"<color=yellow>{target.GetName} の ATK が減少！</color>");
#endif

        target.AddATK(totalCoinPower);
    }
}
