using UnityEngine;

/// <summary>
/// 経験値アイテムの吸い込み範囲をレベルに応じて拡大するスクリプト
/// </summary>
public class ItemArea : MonoBehaviour, IAbilitySetup
{
    /// <summary>
    /// 初期スケール
    /// </summary>
    private Vector3 initialScale;

    /// <summary>
    /// 初期化済みかどうか
    /// </summary>
    private bool isInitialized;

    /// <summary>
    /// 開始前の初期化
    /// </summary>
    private void Awake()
    {
        EnsureInitialization();
    }

    /// <summary>
    /// アビリティ生成時の設定を反映
    /// </summary>
    /// <param name="data"></param>
    public void SetupAbility(AbilityRuntimeData data)
    {
        SetLevel(data.level);
    }

    /// <summary>
    /// レベルに応じて吸い込み範囲を変更
    /// </summary>
    /// <param name="level"></param>
    public void SetLevel(int level)
    {
        EnsureInitialization();

        if (level < 1)
        {
            level = 1;
        }

        // 1レベル上がるごとにスケールを0.5加算
        float addedScale = (level - 1) * 0.5f;

        transform.localScale = new Vector3(
            initialScale.x + addedScale,
            initialScale.y + addedScale,
            initialScale.z + addedScale
        );
    }

    /// <summary>
    /// 初期スケールを保存
    /// </summary>
    private void EnsureInitialization()
    {
        if (isInitialized) return;

        initialScale = transform.localScale;
        isInitialized = true;
    }
}