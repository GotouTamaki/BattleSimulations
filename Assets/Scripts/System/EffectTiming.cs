public enum EffectTiming
{
    TurnStart,     // ターン開始時（赤）
    BeforeMatch,   // マッチ前（橙）
    MatchWin,      // マッチ勝利時（黄）
    MatchLose,     // マッチ敗北時（緑）
    OnHit,         // 攻撃側の攻撃的中時（紫）
    OnDefend,      // 被弾側の防御時（青）
    MatchEnd,       // マッチ後（シアン）
    None
}
