using UnityEngine;

[System.Serializable]
public class AttackSkill : SkillBase, ISkill
{
    public AttackSkill() : base() { }

    public AttackSkill(int id, string name, int coinCount, int basePower, int coinPower, EffectTiming effectTiming)
        : base(id, name, coinCount, basePower, coinPower, effectTiming) { }

    public override void UseSkill(Character user, Character target, int totalCoinPower = 0)
    {
        int totalDamage = Mathf.Max(1, user.GetATK + totalCoinPower - target.GetDEF);
        if (totalDamage > 0)
        {
#if UNITY_EDITOR
            Debug.Log($"<color=purple>{user.GetName} の攻撃が {target.GetName} に {totalDamage} ダメージ！</color>");
#endif

            ActivateSkillEffect(user, target, EffectTiming.OnHit, totalCoinPower); // 攻撃的中時（紫）
            target.AddDamage(totalDamage);
        }
    }

    public override void ApplySkillEffect(Character user, Character target, int totalCoinPower = 0)
    {
        // 例：ATKを一時的に上昇
#if UNITY_EDITOR
        Debug.Log($"<color=yellow>{user.GetName} の ATK が上昇！</color>");
#endif
        user.AddATK(totalCoinPower);
    }
}
