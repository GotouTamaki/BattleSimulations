using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Item
{
    public string Name;
    public EffectTiming EffectTiming;

    public Item(string name, EffectTiming timing)
    {
        Name = name;
        EffectTiming = timing;
    }

    public void ActivateEffect(Character user, List<Character> team, EffectTiming timing)
    {
        if (EffectTiming == timing)
        {
#if UNITY_EDITOR
            Debug.Log($"<color=red>{user.GetName} のアイテム {Name} の効果発動！</color>");
#endif
            if (Name == "扇風機")
            {
                Character fastest = team.OrderByDescending(c => c.GetSPD).FirstOrDefault();
                if (fastest != null)
                {
#if UNITY_EDITOR
                    Debug.Log($"<color=white>{fastest.GetName} のコイン威力が+1された！</color>");
#endif
                }
            }
        }
    }
}
