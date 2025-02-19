using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;
using System;
using Random = UnityEngine.Random;
using System.Threading;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private List<Character> _allyTeam = new List<Character>();
    [SerializeField] private List<Character> _enemyTeam = new List<Character>();
    [SerializeField] private int _allyGenerateCount = 5;
    [SerializeField] private int _enemyGenerateCount = 5;

    private int turnCount = 1;

    private void Start()
    {
        GenerateCharacters();
#if UNITY_EDITOR
        DisplayCharacterList();
#endif
        // CancellationTokenの取得
        var ct = this.GetCancellationTokenOnDestroy();
        BattleRoutine(ct).Forget();
    }

    private void GenerateCharacters()
    {
        for (int i = 0; i < _allyGenerateCount; i++)
        {
            Character character = CreateRandomCharacter(i + 1, "味方" + (i + 1));
            character.SetIsAlly(true);
            _allyTeam.Add(character);
        }

        for (int i = 0; i < _enemyGenerateCount; i++)
        {
            Character character = CreateRandomCharacter(i + 1 + _allyGenerateCount, "敵" + (i + 1));
            character.SetIsAlly(false);
            _enemyTeam.Add(character);
        }
    }

    private Character CreateRandomCharacter(int id, string name)
    {
        int hp = Random.Range(100, 151);
        int atk = Random.Range(10, 51);
        int def = Random.Range(1, 21);
        int spd = Random.Range(10, 51);

        List<ISkill> skills = new List<ISkill>
        {
            new AttackSkill(1,  "パワースラッシュ", 1, 5, 10, EffectTiming.MatchWin),
            new AttackSkill(2,  "フレイムストライク", 2, 7, 3, EffectTiming.MatchLose),
            //(Random.value > 0.5f)
            //    ? new DefendSkill("防御", 1, 10, 4, EffectTiming.TurnStart)
            //    : new EvasionSkill("回避", 1, 4, 10, EffectTiming.OnDefend)
            new DefendSkill(3,  "防御", 1, 4, 5, EffectTiming.TurnStart)
        };

        return new Character(id, name, hp, atk, def, spd, skills);
    }

#if UNITY_EDITOR
    private void DisplayCharacterList()
    {
        Debug.Log("<color=white>=== キャラクター & スキル一覧 ===</color>");

        Debug.Log("<color=blue>=== 味方チーム ===</color>");
        foreach (var character in _allyTeam)
        {
            Debug.Log(character.GetCharacterInfo());
        }

        Debug.Log("<color=red>=== 敵チーム ===</color>");
        foreach (var character in _enemyTeam)
        {
            Debug.Log(character.GetCharacterInfo());
        }
    }
#endif

    private async UniTaskVoid BattleRoutine(CancellationToken ct)
    {
        while (_allyTeam.Any(c => c.IsAlive) && _enemyTeam.Any(c => c.IsAlive))
        {
#if UNITY_EDITOR
            Debug.Log($"<color=white>=== ターン {turnCount} ===</color>");
#endif

            // ターン開始前
            foreach (var character in _allyTeam.Concat(_enemyTeam))
            {
                character.SavePreviousStatus();

                if (!character.IsAlive) continue;

                character.SelectSkill(UnityEngine.Random.Range(0, character.GetSkills.Count));

                // 生きている敵対チームのキャラクターを狙う
                List<Character> potentialTargets = character.GetIsAlly ? _enemyTeam : _allyTeam;
                potentialTargets = potentialTargets.Where(_ => _.IsAlive).ToList();
                character.SelectTarget(potentialTargets, UnityEngine.Random.Range(0, potentialTargets.Count));

#if UNITY_EDITOR
                Debug.Log($"<color=white>{character.GetName} は {character.GetSelectedSkill.GetName} を {character.GetTarget.GetName} に使用予定</color>");
#endif
            }

            // ターン開始時のスキル発動（赤）
            foreach (var character in _allyTeam.Concat(_enemyTeam))
            {
                if (!character.IsAlive) continue;

                character.GetSelectedSkill.ActivateSkillEffect(character, character, EffectTiming.TurnStart);
            }

            // 速度順でターン進行
            List<Character> allCharacters = _allyTeam.Concat(_enemyTeam).OrderByDescending(c => c.GetSPD).ToList();

            foreach (var character in allCharacters)
            {
                if (!character.IsAlive || character.GetTarget == null || !character.GetTarget.IsAlive) continue;
                if (character.GetSelectedSkill == null) character.SelectSkill(new NoneSkill());
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
                        defender.GetSelectedSkill.ActivateSkillEffect(attacker, defender, EffectTiming.OnDefend, defenderPower);

                        int coinPower = attacker.GetSelectedSkill.TossCoins();
                        attacker.GetSelectedSkill.ActivateSkillEffect(attacker, defender, EffectTiming.MatchWin, attackerPower);
                        defender.GetSelectedSkill.ActivateSkillEffect(attacker, defender, EffectTiming.MatchLose, defenderPower);
                        attacker.GetSelectedSkill.UseSkill(attacker, defender, coinPower);

                        defender.SelectSkill(new NoneSkill());
                    }
                    else
                    {
#if UNITY_EDITOR
                        Debug.Log($"<color=yellow>{defender.GetName} がマッチに勝利！</color>");
#endif
                        attacker.GetSelectedSkill.ActivateSkillEffect(attacker, defender, EffectTiming.OnDefend, attackerPower);

                        int coinPower = defender.GetSelectedSkill.TossCoins();
                        defender.GetSelectedSkill.ActivateSkillEffect(defender, attacker, EffectTiming.MatchWin, coinPower);
                        attacker.GetSelectedSkill.ActivateSkillEffect(attacker, defender, EffectTiming.MatchLose, coinPower);
                        defender.GetSelectedSkill.UseSkill(defender, attacker, coinPower);

                        attacker.SelectSkill(new NoneSkill());
                    }
                }
                else
                {
                    // 一方的に攻撃
                    int coinPower = attacker.GetSelectedSkill.TossCoins();
                    attacker.GetSelectedSkill.UseSkill(attacker, defender, coinPower);
                }

                await UniTask.Delay(TimeSpan.FromSeconds(1f));
            }

#if UNITY_EDITOR
            // ターン終了時にステータスの差分を表示
            Debug.Log($"<color=cyan>=== ターン {turnCount} 終了時のステータス ===</color>");
            foreach (var character in _allyTeam.Concat(_enemyTeam))
            {
                character.DisplayStatusDifference();
            }
#endif

            turnCount++;
            await UniTask.Delay(TimeSpan.FromSeconds(2f));
        }

#if UNITY_EDITOR
        string result = _allyTeam.Any(c => c.IsAlive) ? "味方の勝利！" : "敵の勝利！";
        Debug.Log($"<color=yellow>{result}</color>");
#endif
    }
}
