using UnityEngine;

[System.Serializable]
public class Buff
{
    public string Name;
    public int Power;
    public int MaxCount;
    public EffectTiming EffectTiming;
    public bool IsDebuff;
    public int TurnsRemaining;

    public Buff(string name, int power, int maxCount, EffectTiming timing, bool isDebuff)
    {
        Name = name;
        Power = power;
        MaxCount = maxCount;
        EffectTiming = timing;
        IsDebuff = isDebuff;
        TurnsRemaining = maxCount;
    }

    public void ApplyEffect(Character target)
    {
        if (EffectTiming == EffectTiming.TurnStart)
        {
            if (IsDebuff)
                target.AddATK(-Power);
            else
                target.AddATK(Power);

#if UNITY_EDITOR
            Debug.Log($"<color=red>{target.GetName} に {Name} が適用！</color>");
#endif
        }
    }

    public bool ShouldRemove()
    {
        TurnsRemaining--;
        return TurnsRemaining <= 0;
    }
}
