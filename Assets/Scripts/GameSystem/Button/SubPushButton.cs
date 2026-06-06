using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// サブ選択ボタンのHP回復・コイン加算・UI非表示を管理するスクリプト
/// </summary>
public class SubPushButton : MonoBehaviour
{
    /// <summary>
    /// ボタンの処理タイプ
    /// </summary>
    public enum ButtonType
    {
        Heal,
        Coin
    }

    [Header("ボタンの機能")]
    /// <summary>
    /// このボタンで実行する処理
    /// </summary>
    public ButtonType buttonType;

    [Header("参照")]
    /// <summary>
    /// 回復対象のPlayerController
    /// </summary>
    public PlayerController playerController;

    /// <summary>
    /// コイン加算対象のGameController
    /// </summary>
    public GameController gameController;

    [Header("HP回復")]
    /// <summary>
    /// 回復量
    /// </summary>
    public float healAmount = 10f;

    [Header("コイン加算")]
    /// <summary>
    /// 加算するコイン量
    /// </summary>
    public int addCoinAmount = 10;

    [Header("押した後に閉じるUI")]
    /// <summary>
    /// ボタン押下後に非表示にするUI
    /// </summary>
    public GameObject closeTarget;

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
    }

    /// <summary>
    /// 有効化時にクリックイベントを登録
    /// </summary>
    private void OnEnable()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }

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
    /// ボタンが押された時の処理
    /// </summary>
    private void OnButtonClicked()
    {
        if (buttonType == ButtonType.Heal)
        {
            HealPlayer();
        }
        else if (buttonType == ButtonType.Coin)
        {
            AddCoin();
        }

        if (closeTarget != null)
        {
            closeTarget.SetActive(false);
        }
    }

    /// <summary>
    /// プレイヤーを回復
    /// </summary>
    private void HealPlayer()
    {
        if (playerController == null)
        {
            return;
        }

        playerController.Heal(healAmount);
    }

    /// <summary>
    /// コインを加算
    /// </summary>
    private void AddCoin()
    {
        if (gameController == null)
        {
            return;
        }

        gameController.AddCoin(addCoinAmount);
    }
}