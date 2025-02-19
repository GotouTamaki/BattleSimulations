using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Item
{
    [SerializeField] int _id;
    [SerializeField] string _name;
    [SerializeField] EffectTiming _effectTiming;
    [SerializeField] EffectSubject _effectSubject;

    public int GetId => _id;

    public string GetName => _name;

    public EffectTiming GetEffectTiming => _effectTiming;

    public EffectSubject GetEffectSubject => _effectSubject;

    public Item() { }

    public Item(int id, string name, EffectTiming timing, EffectSubject effectSubject)
    {
        _id = id;
        _name = name;
        _effectTiming = timing;
        _effectSubject = effectSubject;
    }

    public void ActivateItemEffect(List<Character> team, EffectTiming timing, EffectSubject effectSubject)
    {
        if (_effectTiming == timing && _effectSubject == effectSubject)
        {
#if UNITY_EDITOR
            Debug.Log($"<color=red>アイテム {GetName} の効果発動！</color>");
#endif

            ApplyItemEffect(team);
        }
    }

    public void ApplyItemEffect(List<Character> team)
    {
        Character fastest = team.OrderByDescending(c => c.GetSPD).FirstOrDefault();

        if (fastest != null)
        {
#if UNITY_EDITOR
            Debug.Log($"<color=white>{fastest.GetName} のコイン威力が+1された！</color>");
#endif
        }
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
