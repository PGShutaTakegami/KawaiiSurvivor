using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// レベルアップUIの表示・ポーズ・アビリティボタン生成を管理するスクリプト
/// </summary>
public class LevelController : MonoBehaviour
{
    [Header("プレイヤーへの参照")]
    /// <summary>
    /// プレイヤー参照
    /// </summary>
    public PlayerController playerController;

    [Header("AbilityManager")]
    /// <summary>
    /// AbilityManager参照
    /// </summary>
    public AbilityManager abilityManager;

    [Header("表示するUIフレーム")]
    /// <summary>
    /// レベルアップ時に表示するUI
    /// </summary>
    public GameObject levelFrame;

    [Header("ボタン配置位置")]
    /// <summary>
    /// 生成するボタンの配置位置
    /// </summary>
    public Vector3[] localOffset = new Vector3[3];

    [Header("Clone元のAbilityボタンPrefab")]
    /// <summary>
    /// 生成元のアビリティボタンPrefab
    /// </summary>
    public GameObject[] abilityButton;

    /// <summary>
    /// 前フレームのレベルアップUI表示状態
    /// </summary>
    private bool wasLevelFrameActive = false;

    /// <summary>
    /// 生成済みボタン一覧
    /// </summary>
    private readonly List<GameObject> spawnedButtons = new List<GameObject>();

    /// <summary>
    /// 開始時の参照取得と初期化
    /// </summary>
    private void Start()
    {
        if (playerController == null)
        {
            playerController = FindFirstObjectByType<PlayerController>();
        }

        if (abilityManager == null)
        {
            abilityManager = FindFirstObjectByType<AbilityManager>();
        }

        if (levelFrame != null)
        {
            levelFrame.SetActive(false);
            wasLevelFrameActive = false;
        }

        GamePauseState.ResumeFromLevelUp();
    }

    /// <summary>
    /// レベルアップUIの表示状態を監視
    /// </summary>
    private void Update()
    {
        if (levelFrame == null) return;

        bool isActiveNow = levelFrame.activeSelf;

        if (isActiveNow && !wasLevelFrameActive)
        {
            GamePauseState.PauseForLevelUp();
            SetUIAnimatorsUnscaledTime(levelFrame);
            SpawnAbilityButtons();
        }

        if (!isActiveNow && wasLevelFrameActive)
        {
            GamePauseState.ResumeFromLevelUp();
            ClearSpawnedButtons();
        }

        wasLevelFrameActive = isActiveNow;
    }

    /// <summary>
    /// 有効化時にレベルアップイベントへ登録
    /// </summary>
    private void OnEnable()
    {
        if (playerController == null)
        {
            playerController = FindFirstObjectByType<PlayerController>();
        }

        if (abilityManager == null)
        {
            abilityManager = FindFirstObjectByType<AbilityManager>();
        }

        if (playerController != null)
        {
            playerController.OnLevelUp -= ShowLevelUpUI;
            playerController.OnLevelUp += ShowLevelUpUI;
        }
    }

    /// <summary>
    /// 無効化時にレベルアップイベントから解除
    /// </summary>
    private void OnDisable()
    {
        if (playerController != null)
        {
            playerController.OnLevelUp -= ShowLevelUpUI;
        }

        GamePauseState.ResumeFromLevelUp();
    }

    /// <summary>
    /// レベルアップUIを表示
    /// </summary>
    private void ShowLevelUpUI()
    {
        if (levelFrame != null)
        {
            levelFrame.SetActive(true);
        }
    }

    /// <summary>
    /// アビリティボタンをランダム配置で生成
    /// </summary>
    private void SpawnAbilityButtons()
    {
        ClearSpawnedButtons();

        if (levelFrame == null || abilityButton == null || abilityButton.Length == 0) return;
        if (localOffset == null || localOffset.Length == 0) return;

        int spawnCount = Mathf.Min(abilityButton.Length, localOffset.Length);
        int[] buttonIndexes = CreateShuffledIndexes(abilityButton.Length);
        int[] offsetIndexes = CreateShuffledIndexes(localOffset.Length);

        for (int i = 0; i < spawnCount; i++)
        {
            GameObject buttonPrefab = abilityButton[buttonIndexes[i]];
            if (buttonPrefab == null) continue;

            GameObject buttonClone = Instantiate(buttonPrefab, levelFrame.transform);
            buttonClone.SetActive(true);

            PushButton pushButton = buttonClone.GetComponent<PushButton>();
            if (pushButton != null)
            {
                pushButton.SetReferences(abilityManager, levelFrame);
            }

            RectTransform rectTransform = buttonClone.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.localPosition = localOffset[offsetIndexes[i]];
                rectTransform.localRotation = Quaternion.identity;
                rectTransform.localScale = Vector3.one;
            }
            else
            {
                buttonClone.transform.localPosition = localOffset[offsetIndexes[i]];
                buttonClone.transform.localRotation = Quaternion.identity;
                buttonClone.transform.localScale = Vector3.one;
            }

            SetUIAnimatorsUnscaledTime(buttonClone);
            spawnedButtons.Add(buttonClone);
        }
    }

    /// <summary>
    /// 生成済みボタンを削除
    /// </summary>
    private void ClearSpawnedButtons()
    {
        for (int i = spawnedButtons.Count - 1; i >= 0; i--)
        {
            if (spawnedButtons[i] != null)
            {
                Destroy(spawnedButtons[i]);
            }
        }

        spawnedButtons.Clear();
    }

    /// <summary>
    /// UI内のAnimatorを時間停止中でも動く設定にする
    /// </summary>
    /// <param name="root"></param>
    private void SetUIAnimatorsUnscaledTime(GameObject root)
    {
        Animator[] animators = root.GetComponentsInChildren<Animator>(true);

        foreach (Animator animator in animators)
        {
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
    }

    /// <summary>
    /// 指定数のインデックスをシャッフルして作成
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    private int[] CreateShuffledIndexes(int count)
    {
        int[] indexes = new int[count];

        for (int i = 0; i < count; i++)
        {
            indexes[i] = i;
        }

        for (int i = 0; i < indexes.Length; i++)
        {
            int randomIndex = Random.Range(i, indexes.Length);

            int temp = indexes[i];
            indexes[i] = indexes[randomIndex];
            indexes[randomIndex] = temp;
        }

        return indexes;
    }

    /// <summary>
    /// レベルアップUIを非表示
    /// </summary>
    public void HideLevelUpUI()
    {
        if (levelFrame != null)
        {
            levelFrame.SetActive(false);
        }

        GamePauseState.ResumeFromLevelUp();
        ClearSpawnedButtons();
        wasLevelFrameActive = false;
    }
}