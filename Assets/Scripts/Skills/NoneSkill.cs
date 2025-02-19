using UnityEngine;

public class NoneSkill : SkillBase, ISkill
{
    public NoneSkill() : base() { }

    public NoneSkill(int id, string name, int coinCount, int basePower, int coinPower, EffectTiming effectTiming)
    : base(0, "none", 0, 0, 0, EffectTiming.None) { }

    public override void UseSkill(Character user, Character target, int coinPower)
    {
#if UNITY_EDITOR
        Debug.Log("none");
#endif
    }

    public override void ApplySkillEffect(Character user, Character target, int coinPower = 0)
    {
#if UNITY_EDITOR
        Debug.Log("none");
#endif
    }
}
