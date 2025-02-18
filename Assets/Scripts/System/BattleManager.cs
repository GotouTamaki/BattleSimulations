using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BattleManager : MonoBehaviour
{

    public List<Character> AllyTeam = new List<Character>();
    public List<Character> EnemyTeam = new List<Character>();
    private int turnCount = 1;

    void Start()
    {
        GenerateCharacters();
#if UNITY_EDITOR
        DisplayCharacterList();
#endif
        StartCoroutine(BattleRoutine());
    }

    void GenerateCharacters()
    {
        for (int i = 0; i < 5; i++)
        {
            Character character = CreateRandomCharacter("味方" + (i + 1));
            character.SetIsAlly(true);
            AllyTeam.Add(character);
        }

        for (int i = 0; i < 5; i++)
        {
            Character character = CreateRandomCharacter("敵" + (i + 1));
            character.SetIsAlly(false);
            EnemyTeam.Add(character);
        }
    }

    Character CreateRandomCharacter(string name)
    {
        int hp = Random.Range(100, 151);
        int atk = Random.Range(10, 51);
        int def = Random.Range(1, 21);
        int spd = Random.Range(10, 51);

        List<ISkill> skills = new List<ISkill>
        {
            new AttackSkill("パワースラッシュ", 1, 5, 10, EffectTiming.MatchWin),
            new AttackSkill("フレイムストライク", 2, 7, 3, EffectTiming.MatchLose),
            //(Random.value > 0.5f)
            //    ? new DefendSkill("防御", 1, 10, 4, EffectTiming.TurnStart)
            //    : new EvasionSkill("回避", 1, 4, 10, EffectTiming.OnDefend)
            new DefendSkill("防御", 1, 4, 5, EffectTiming.TurnStart)
        };

        return new Character(name, hp, atk, def, spd, skills);
    }

#if UNITY_EDITOR
    void DisplayCharacterList()
    {
        Debug.Log("<color=white>=== キャラクター & スキル一覧 ===</color>");

        Debug.Log("<color=blue>=== 味方チーム ===</color>");
        foreach (var character in AllyTeam)
        {
            Debug.Log(character.GetCharacterInfo());
        }

        Debug.Log("<color=red>=== 敵チーム ===</color>");
        foreach (var character in EnemyTeam)
        {
            Debug.Log(character.GetCharacterInfo());
        }
    }
#endif

    IEnumerator BattleRoutine()
    {
        while (AllyTeam.Any(c => c.IsAlive) && EnemyTeam.Any(c => c.IsAlive))
        {
#if UNITY_EDITOR
            Debug.Log($"<color=white>=== ターン {turnCount} ===</color>");
#endif

            // ターン開始前
            foreach (var character in AllyTeam.Concat(EnemyTeam))
            {
                character.SavePreviousStatus();

                if (!character.IsAlive) continue;

                character.SelectSkill(UnityEngine.Random.Range(0, character.GetSkills.Count));

                // 生きている敵対チームのキャラクターを狙う
                List<Character> potentialTargets = character.GetIsAlly ? EnemyTeam : AllyTeam;
                potentialTargets = potentialTargets.Where(_ => _.IsAlive).ToList();
                character.SelectTarget(potentialTargets, UnityEngine.Random.Range(0, potentialTargets.Count));

#if UNITY_EDITOR
                Debug.Log($"<color=white>{character.GetName} は {character.GetSelectedSkill.GetName} を {character.GetTarget.GetName} に使用予定</color>");
#endif
            }

            // ターン開始時のスキル発動（赤）
            foreach (var character in AllyTeam.Concat(EnemyTeam))
            {
                if (!character.IsAlive) continue;

                character.GetSelectedSkill.ActivateSkillEffect(character, character, EffectTiming.TurnStart);
            }

            // 速度順でターン進行
            List<Character> allCharacters = AllyTeam.Concat(EnemyTeam).OrderByDescending(c => c.GetSPD).ToList();

            foreach (var character in allCharacters)
            {
                if (!character.IsAlive || character.GetTarget == null || !character.GetTarget.IsAlive) continue;
                if (character.GetSelectedSkill.GetEffectTiming == EffectTiming.None) continue;

                Character attacker = character;
                Character defender = attacker.GetTarget;

                if (defender.GetTarget == attacker && defender.GetSelectedSkill.GetEffectTiming != EffectTiming.None)
                {
                    // 双方がターゲットにし合っている -> マッチ発生
                    int attackerPower = attacker.GetSelectedSkill.TossCoins();
                    int defenderPower = defender.GetSelectedSkill.TossCoins();

#if UNITY_EDITOR
                    Debug.Log($"<color=red>{attacker.GetName} : {attacker.GetSelectedSkill.GetName} vs {defender.GetName} : {defender.GetSelectedSkill.GetName}"
                            + $"\n 出目 {attackerPower} vs {defenderPower}</color>");
#endif

                    if (attackerPower >= defenderPower)
                    {
#if UNITY_EDITOR
                        Debug.Log($"<color=yellow>{attacker.GetName} がマッチに勝利！</color>");
#endif
                        defender.GetSelectedSkill.ActivateSkillEffect(attacker, defender, EffectTiming.OnDefend);
                        int coinPower = attacker.GetSelectedSkill.TossCoins();
                        attacker.GetSelectedSkill.UseSkill(attacker, defender, coinPower);
                        attacker.GetSelectedSkill.ActivateSkillEffect(attacker, defender, EffectTiming.MatchWin);
                        defender.GetSelectedSkill.ActivateSkillEffect(attacker, defender, EffectTiming.MatchLose);
                        defender.SelectSkill(new NoneSkill());
                    }
                    else
                    {
#if UNITY_EDITOR
                        Debug.Log($"<color=yellow>{defender.GetName} がマッチに勝利！</color>");
#endif
                        attacker.GetSelectedSkill.ActivateSkillEffect(attacker, defender, EffectTiming.OnDefend);
                        int coinPower = defender.GetSelectedSkill.TossCoins();
                        defender.GetSelectedSkill.UseSkill(defender, attacker, coinPower);
                        defender.GetSelectedSkill.ActivateSkillEffect(defender, attacker, EffectTiming.MatchWin);
                        attacker.GetSelectedSkill.ActivateSkillEffect(attacker, defender, EffectTiming.MatchLose);
                        attacker.SelectSkill(new NoneSkill());
                    }
                }
                else
                {
                    // 一方的に攻撃
                    int coinPower = attacker.GetSelectedSkill.TossCoins();
                    attacker.GetSelectedSkill.UseSkill(attacker, defender, coinPower);
                }

                yield return new WaitForSeconds(1f);
            }

#if UNITY_EDITOR
            // ターン終了時にステータスの差分を表示
            Debug.Log($"<color=cyan>=== ターン {turnCount} 終了時のステータス ===</color>");
            foreach (var character in AllyTeam.Concat(EnemyTeam))
            {
                character.DisplayStatusDifference();
            }
#endif

            turnCount++;
            yield return new WaitForSeconds(2f);
        }

#if UNITY_EDITOR
        string result = AllyTeam.Any(c => c.IsAlive) ? "味方の勝利！" : "敵の勝利！";
        Debug.Log($"<color=yellow>{result}</color>");
#endif
    }
}
