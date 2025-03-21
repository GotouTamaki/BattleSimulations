using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;
using System;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private CharacterTeamData _allyTeamData;
    [SerializeField] private CharacterTeamData _enemyTeamData;
    [SerializeField] private Transform[] _allyTeamPositions;
    [SerializeField] private Transform[] _enemyTeamPositions;
    [SerializeField] private List<Character> _allyTeamList = new List<Character>();
    [SerializeField] private List<Character> _enemyTeamList = new List<Character>();
    private Dictionary<int, GameObject> _allyTeamObjects = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> _enemyTeamObjects = new Dictionary<int, GameObject>();
    //[SerializeField] private int _allyGenerateCount = 5;
    //[SerializeField] private int _enemyGenerateCount = 5;

    private AnimationCommands _animationCommands;
    private FadeController _fadeController;
    private UIManager _uiManager;
    private CameraController _cameraController;
    private int turnCount = 1;

    public Transform[] GetAllyTeamPositions => _allyTeamPositions;

    public Transform[] GetEnemyTeamPositions => _enemyTeamPositions;

    public List<Character> AllyTeamList => _allyTeamList;

    public List<Character> EnemyTeamList => _enemyTeamList;

    public Dictionary<int, GameObject> GetAllyTeamObjects => _allyTeamObjects;

    public Dictionary<int, GameObject> GetEnemyTeamObjects => _enemyTeamObjects;

    private void OnEnable()
    {
        _animationCommands = FindFirstObjectByType<AnimationCommands>();
        _fadeController = FindFirstObjectByType<FadeController>();
        _uiManager = FindFirstObjectByType<UIManager>();
        _cameraController = FindFirstObjectByType<CameraController>();

        _uiManager.SetResultText(string.Empty);
        _uiManager.SetEnableRestartButton(false);

        GenerateCharacters();
#if UNITY_EDITOR
        DisplayCharacterList();
#endif
        // CancellationTokenの取得
        //var ct = this.GetCancellationTokenOnDestroy();
        BattleRoutine().Forget();
    }

    private void GenerateCharacters()
    {
        for (int i = 0; i < _allyTeamData.Characters.Length; i++)
        {
            //Character character = CreateRandomCharacter(_allyTeamData.Characters[i].GetId, _allyTeamData.Characters[i].GetName);
            Character character = CreateCharacter(_allyTeamData.Characters[i].GetId, _allyTeamData);
            character.SetIsAlly(true);
            _allyTeamList.Add(character);

            GameObject characterObject = Instantiate(_allyTeamData.Characters[i].GetPrefab, _allyTeamPositions[i].position, _allyTeamPositions[i].rotation);
            characterObject.transform.SetParent(_allyTeamPositions[i], false);
            characterObject.GetComponentInChildren<SpriteRenderer>().flipX = false;
            _allyTeamObjects.Add(character.GetId, characterObject);
        }

        for (int i = 0; i < _enemyTeamData.Characters.Length; i++)
        {
            //Character character = CreateRandomCharacter(_enemyTeamData.Characters[i].GetId, _enemyTeamData.Characters[i].GetName);
            Character character = CreateCharacter(_enemyTeamData.Characters[i].GetId, _enemyTeamData);
            _enemyTeamList.Add(character);

            GameObject characterObject = Instantiate(_enemyTeamData.Characters[i].GetPrefab, _enemyTeamPositions[i].position, _enemyTeamPositions[i].rotation);
            characterObject.transform.SetParent(_enemyTeamPositions[i], false);
            characterObject.GetComponentInChildren<SpriteRenderer>().flipX = true;
            _enemyTeamObjects.Add(character.GetId, characterObject);
        }
    }

    private Character CreateCharacter(int index, CharacterTeamData characterTeamData)
    {
        int id = characterTeamData.Characters[index].GetId;
        string name = characterTeamData.Characters[index].GetName;
        int hp = characterTeamData.Characters[index].GetHP;
        int atk = characterTeamData.Characters[index].GetATK;
        int def = characterTeamData.Characters[index].GetDEF;
        int spd = characterTeamData.Characters[index].GetSPD;

        List<ISkill> skills = characterTeamData.Characters[index].GetSkills;
        string[] animationName = characterTeamData.Characters[index].GetAnimationName;

        return new Character(id, name, hp, atk, def, spd, skills, animationName);
    }

    //private Character CreateRandomCharacter(int id, string name)
    //{
    //    int hp = Random.Range(100, 151);
    //    int atk = Random.Range(10, 51);
    //    int def = Random.Range(1, 21);
    //    int spd = Random.Range(10, 51);

    //    List<ISkill> skills = new List<ISkill>
    //    {
    //        new AttackSkill(1,  "パワースラッシュ", 1, 5, 10, EffectTiming.MatchWin),
    //        new AttackSkill(2,  "フレイムストライク", 2, 7, 3, EffectTiming.MatchLose),
    //        //(Random.value > 0.5f)
    //        //    ? new DefendSkill("防御", 1, 10, 4, EffectTiming.TurnStart)
    //        //    : new EvasionSkill("回避", 1, 4, 10, EffectTiming.OnDefend)
    //        new DefendSkill(3,  "防御", 1, 4, 5, EffectTiming.TurnStart)
    //    };

    //    string[] animationName = new string[]
    //    {
    //        "Attack1",
    //        "Attack2",
    //        "Defend1"
    //    };

    //    return new Character(id, name, hp, atk, def, spd, skills , animationName);
    //}

#if UNITY_EDITOR
    private void DisplayCharacterList()
    {
        Debug.Log("<color=white>=== キャラクター & スキル一覧 ===</color>");

        Debug.Log("<color=blue>=== 味方チーム ===</color>");
        foreach (var character in _allyTeamList)
        {
            Debug.Log(character.GetCharacterInfo());
        }

        Debug.Log("<color=red>=== 敵チーム ===</color>");
        foreach (var character in _enemyTeamList)
        {
            Debug.Log(character.GetCharacterInfo());
        }
    }
#endif

    private async UniTaskVoid BattleRoutine()
    {
        while (_allyTeamList.Any(c => c.IsAlive) && _enemyTeamList.Any(c => c.IsAlive))
        {
#if UNITY_EDITOR
            Debug.Log($"<color=white>=== ターン {turnCount} ===</color>");
#endif

            // 座標の初期化
            for (int i = 0; _allyTeamPositions.Length > i; i++)
            {
                _allyTeamObjects[i].transform.position = _allyTeamPositions[i].transform.position;
            }

            for (int i = 0; _enemyTeamPositions.Length > i; i++)
            {
                _enemyTeamObjects[i].transform.position = _enemyTeamPositions[i].transform.position;
            }

            _uiManager.SetAllySkillText(string.Empty);
            _uiManager.SetAllySkillCoinPowerText(string.Empty);
            _uiManager.SetEnemySkillText(string.Empty);
            _uiManager.SetEnemySkillCoinPowerText(string.Empty);

            _fadeController.FadeIn(_fadeController.GetFadeTime).Forget();
            await UniTask.Delay(TimeSpan.FromSeconds(_fadeController.GetFadeTime));

            // ターン開始前
            foreach (var character in _allyTeamList.Concat(_enemyTeamList))
            {
                character.SavePreviousStatus();

                if (!character.IsAlive) continue;

                character.SelectSkill(UnityEngine.Random.Range(0, character.GetSkills.Count));

                // 生きている敵対チームのキャラクターを狙う
                List<Character> potentialTargets = character.GetIsAlly ? _enemyTeamList : _allyTeamList;
                potentialTargets = potentialTargets.Where(_ => _.IsAlive).ToList();
                character.SelectTarget(potentialTargets, UnityEngine.Random.Range(0, potentialTargets.Count));

#if UNITY_EDITOR
                Debug.Log($"<color=white>{character.GetName} は {character.GetSelectedSkill.GetName} を {character.GetTarget.GetName} に使用予定</color>");
#endif
            }

            // ターン開始時のスキル発動（赤）
            foreach (var character in _allyTeamList.Concat(_enemyTeamList))
            {
                if (!character.IsAlive) continue;

                character.GetSelectedSkill.ActivateSkillEffect(character, character, EffectTiming.TurnStart);
            }

            // 速度順でターン進行
            List<Character> allCharacters = _allyTeamList.Concat(_enemyTeamList).OrderByDescending(c => c.GetSPD).ToList();

            foreach (var character in allCharacters)
            {
                if (!character.IsAlive || character.GetTarget == null || !character.GetTarget.IsAlive) continue;
                if (character.GetSelectedSkill == null) character.SelectSkill(new NoneSkill());
                if (character.GetSelectedSkill.GetEffectTiming == EffectTiming.None) continue;

                Character attacker = character;
                Character defender = attacker.GetTarget;

                GameObject attackerObject = null;
                GameObject defenderObject = null;

                if (attacker.GetIsAlly)
                {
                    attackerObject = _allyTeamObjects[attacker.GetId];
                    _animationCommands.SetTargetColor(_allyTeamObjects, attackerObject);
                    _uiManager.SetAllySkillText(attacker.GetSelectedSkill.GetName);
                }
                else
                {
                    attackerObject = _enemyTeamObjects[attacker.GetId];
                    _animationCommands.SetTargetColor(_enemyTeamObjects, attackerObject);
                    _uiManager.SetEnemySkillText(attacker.GetSelectedSkill.GetName);
                }

                if (defender.GetIsAlly)
                {
                    defenderObject = _allyTeamObjects[defender.GetId];
                    _animationCommands.SetTargetColor(_allyTeamObjects, defenderObject);
                    _uiManager.SetAllySkillText(defender.GetSelectedSkill.GetName);
                }
                else
                {
                    defenderObject = _enemyTeamObjects[defender.GetId];
                    _animationCommands.SetTargetColor(_enemyTeamObjects, defenderObject);
                    _uiManager.SetEnemySkillText(defender.GetSelectedSkill.GetName);
                }

                // ターゲットに近づく
                _animationCommands.FasterCharacterMove(attackerObject.transform, defenderObject.transform).Forget();
                _animationCommands.SlowerCharacterMove(defenderObject.transform, attackerObject.transform).Forget();

                _cameraController.SetTargetGroup(attackerObject.transform, defenderObject.transform);

                if (defender.GetTarget == attacker && defender.GetSelectedSkill.GetEffectTiming != EffectTiming.None)
                {
                    // 双方がターゲットにし合っている -> マッチ発生
                    int attackerPower = attacker.GetSelectedSkill.TossCoins();

                    if (attacker.GetIsAlly)
                    {
                        _uiManager.SetAllySkillCoinPowerText($"{attackerPower}");
                    }
                    else
                    {
                        _uiManager.SetEnemySkillCoinPowerText($"{attackerPower}");
                    }

                    int defenderPower = defender.GetSelectedSkill.TossCoins();

                    if (defender.GetIsAlly)
                    {
                        _uiManager.SetAllySkillCoinPowerText($"{defenderPower}");
                    }
                    else
                    {
                        _uiManager.SetEnemySkillCoinPowerText($"{defenderPower}");
                    }

#if UNITY_EDITOR
                    Debug.Log($"<color=red>{attacker.GetName} : {attacker.GetSelectedSkill.GetName} vs {defender.GetName} : {defender.GetSelectedSkill.GetName}"
                            + $"\n 出目 {attackerPower} vs {defenderPower}</color>");
#endif

                    if (attackerPower >= defenderPower)
                    {
#if UNITY_EDITOR
                        Debug.Log($"<color=yellow>{attacker.GetName} がマッチに勝利！</color>");
#endif
                        if (attacker.GetIsAlly)
                        {
                            _uiManager.SetAllySkillCoinPowerText($"<color=red>{attackerPower}</color>");
                        }
                        else
                        {
                            _uiManager.SetEnemySkillCoinPowerText($"<color=red>{attackerPower}</color>");
                        }

                        defender.GetSelectedSkill.ActivateSkillEffect(attacker, defender, EffectTiming.OnDefend, defenderPower);

                        attackerPower = attacker.GetSelectedSkill.TossCoins();
                        attacker.GetSelectedSkill.ActivateSkillEffect(attacker, defender, EffectTiming.MatchWin, attackerPower);
                        defender.GetSelectedSkill.ActivateSkillEffect(attacker, defender, EffectTiming.MatchLose, defenderPower);
                        attacker.GetSelectedSkill.UseSkill(attacker, defender, attackerPower);

                        defender.SelectSkill(new NoneSkill());
                    }
                    else
                    {
#if UNITY_EDITOR
                        Debug.Log($"<color=yellow>{defender.GetName} がマッチに勝利！</color>");
#endif

                        if (defender.GetIsAlly)
                        {
                            _uiManager.SetAllySkillCoinPowerText($"<color=red>{defenderPower}</color>");
                        }
                        else
                        {
                            _uiManager.SetEnemySkillCoinPowerText($"<color=red>{defenderPower}</color>");
                        }

                        attacker.GetSelectedSkill.ActivateSkillEffect(attacker, defender, EffectTiming.OnDefend, attackerPower);

                        defenderPower = defender.GetSelectedSkill.TossCoins();
                        defender.GetSelectedSkill.ActivateSkillEffect(defender, attacker, EffectTiming.MatchWin, defenderPower);
                        attacker.GetSelectedSkill.ActivateSkillEffect(attacker, defender, EffectTiming.MatchLose, attackerPower);
                        defender.GetSelectedSkill.UseSkill(defender, attacker, defenderPower);

                        attacker.SelectSkill(new NoneSkill());
                    }
                }
                else
                {
                    // 一方的に攻撃
                    int attackerPower = attacker.GetSelectedSkill.TossCoins();

                    if (attacker.GetIsAlly)
                    {
                        _uiManager.SetAllySkillCoinPowerText($"<color=red>{attackerPower}</color>");
                        _uiManager.SetEnemySkillText(string.Empty);
                        _uiManager.SetEnemySkillCoinPowerText(string.Empty);
                    }
                    else
                    {
                        _uiManager.SetEnemySkillCoinPowerText($"<color=red>{attackerPower}</color>");
                        _uiManager.SetAllySkillText(string.Empty);
                        _uiManager.SetAllySkillCoinPowerText(string.Empty);
                    }

                    attacker.GetSelectedSkill.UseSkill(attacker, defender, attackerPower);
                }

                // 死亡判定
                if (!attacker.IsAlive)
                {
                    _animationCommands.DeadEffects(attackerObject).Forget();
                }

                if (!defender.IsAlive)
                {
                    _animationCommands.DeadEffects(defenderObject).Forget();
                }

                _cameraController.SetTargetGroup(_allyTeamObjects[1].transform, _enemyTeamObjects[2].transform);
                await UniTask.Delay(TimeSpan.FromSeconds(1f));
            }

#if UNITY_EDITOR
            // ターン終了時にステータスの差分を表示
            Debug.Log($"<color=cyan>=== ターン {turnCount} 終了時のステータス ===</color>");
            foreach (var character in _allyTeamList.Concat(_enemyTeamList))
            {
                character.DisplayStatusDifference();
            }
#endif

            await UniTask.Delay(TimeSpan.FromSeconds(2f));
            _animationCommands.ResetTargetColor(_allyTeamObjects);
            _animationCommands.ResetTargetColor(_enemyTeamObjects);

            turnCount++;
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            _fadeController.FadeOut(_fadeController.GetFadeTime).Forget();
            await UniTask.Delay(TimeSpan.FromSeconds(_fadeController.GetFadeTime));
        }

        string result = string.Empty;

#if UNITY_EDITOR
        result = _allyTeamList.Any(c => c.IsAlive) ? "味方の勝利！" : "敵の勝利！";
        Debug.Log($"<color=yellow>{result}</color>");
#endif

        result = _allyTeamList.Any(c => c.IsAlive) ? $"<color=yellow>Victory!!</color>" : "<color=red>Fail...</color>";
        _uiManager.SetResultText(result);
        await UniTask.Delay(TimeSpan.FromSeconds(2f));
        _uiManager.SetResultText(string.Empty);
        _uiManager.SetEnableRestartButton(true);
    }
}
