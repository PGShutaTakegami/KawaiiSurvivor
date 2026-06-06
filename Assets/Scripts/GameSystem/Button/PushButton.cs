using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// レベルアップ時のアビリティ選択ボタンを管理するスクリプト
/// </summary>
public class PushButton : MonoBehaviour
{
    /// <summary>
    /// ボタンで解放またはレベルアップするアビリティ設定
    /// </summary>
    [System.Serializable]
    public class AbilityUpgradeOption
    {
        [Tooltip("AbilityManagerに登録しているAbility Prefab")]
        /// <summary>
        /// 対象のアビリティPrefab
        /// </summary>
        public GameObject abilityPrefab;

        [Tooltip("チェックを入れると、このボタンを押した時に解放またはレベルアップします")]
        /// <summary>
        /// このボタンで実行対象にするかどうか
        /// </summary>
        public bool isTarget;
    }

    [Header("AbilityManager")]
    /// <summary>
    /// AbilityManager参照
    /// </summary>
    public AbilityManager abilityManager;

    [Header("LevelUpFrame")]
    /// <summary>
    /// 閉じる対象のレベルアップUI
    /// </summary>
    public GameObject levelUpFrame;

    [Header("対象のアビリティPrefab")]
    /// <summary>
    /// このボタンで処理するアビリティ候補
    /// </summary>
    public List<AbilityUpgradeOption> targetAbilities;

    /// <summary>
    /// Buttonコンポーネント
    /// </summary>
    private Button button;

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        button = GetComponent<Button>();
        FindReferencesIfNeeded();
    }

    /// <summary>
    /// 有効化時にクリックイベントを登録
    /// </summary>
    private void OnEnable()
    {
        FindReferencesIfNeeded();

        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClicked);
            button.onClick.AddListener(OnButtonClicked);
        }
    }

    /// <summary>
    /// 無効化時にクリックイベントを解除
    /// </summary>
    private void OnDisable()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClicked);
        }
    }

    /// <summary>
    /// レベルアップUI参照を設定
    /// </summary>
    /// <param name="targetFrame"></param>
    public void SetLevelUpFrame(GameObject targetFrame)
    {
        levelUpFrame = targetFrame;
    }

    /// <summary>
    /// AbilityManager参照を設定
    /// </summary>
    /// <param name="manager"></param>
    public void SetAbilityManager(AbilityManager manager)
    {
        abilityManager = manager;
    }

    /// <summary>
    /// AbilityManagerとレベルアップUI参照をまとめて設定
    /// </summary>
    /// <param name="manager"></param>
    /// <param name="targetFrame"></param>
    public void SetReferences(AbilityManager manager, GameObject targetFrame)
    {
        abilityManager = manager;
        levelUpFrame = targetFrame;
    }

    /// <summary>
    /// ボタンが押された時の処理
    /// </summary>
    public void OnButtonClicked()
    {
        FindReferencesIfNeeded();

        if (abilityManager == null)
        {
            return;
        }

        foreach (AbilityUpgradeOption option in targetAbilities)
        {
            if (option == null) continue;
            if (!option.isTarget) continue;
            if (option.abilityPrefab == null) continue;

            abilityManager.UnlockOrLevelUpAbilityByPrefab(option.abilityPrefab);
        }

        if (levelUpFrame != null)
        {
            levelUpFrame.SetActive(false);
        }
    }

    /// <summary>
    /// 必要な参照を取得
    /// </summary>
    private void FindReferencesIfNeeded()
    {
        if (abilityManager == null)
        {
            abilityManager = FindFirstObjectByType<AbilityManager>();
        }
    }
}