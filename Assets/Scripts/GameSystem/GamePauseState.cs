using UnityEngine;

/// <summary>
/// レベルアップ中のポーズ状態を管理するクラス
/// </summary>
public static class GamePauseState
{
    /// <summary>
    /// レベルアップによるポーズ中かどうか
    /// </summary>
    public static bool IsLevelUpPaused { get; private set; }

    /// <summary>
    /// レベルアップ時に時間を停止
    /// </summary>
    public static void PauseForLevelUp()
    {
        IsLevelUpPaused = true;
        Time.timeScale = 0f;
    }

    /// <summary>
    /// レベルアップ画面終了時に時間を再開
    /// </summary>
    public static void ResumeFromLevelUp()
    {
        IsLevelUpPaused = false;
        Time.timeScale = 1f;
    }
}