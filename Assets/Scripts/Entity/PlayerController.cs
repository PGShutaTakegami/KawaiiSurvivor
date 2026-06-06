using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour
{
    public event Action OnLevelUp;

    [Header("ステータス")]
    /// <summary>
    /// プレイヤーの最大HP
    /// </summary>
    [SerializeField] private float maxHp = 1000f;

    /// <summary>
    /// HP現在値
    /// </summary>
    public float hp;

    /// <summary>
    /// 移動速度
    /// </summary>
    public float moveSpeed = 5f;

    /// <summary>
    /// プレイヤーレベル
    /// </summary>
    public int level = 0;

    [Header("HPバー")]
    /// <summary>
    /// HPバーのオブジェクト
    /// </summary>
    public GameObject hpBar;

    [Header("経験値")]
    /// <summary>
    /// 次のレベルまでの経験値
    /// </summary>
    public int[] xpToNextLevel;

    [Header("XPバー")]
    /// <summary>
    /// XPバーのオブジェクト
    /// </summary>
    public GameObject xpBar;

    [Header("現在の経験値")]
    /// <summary>
    /// 現在の経験値
    /// </summary>
    public int currentXp = 0;

    /// <summary>
    /// ゲームオーバーUI
    /// </summary>
    public GameObject gameOverUI;

    [Header("ダメージ音")]
    /// <summary>
    /// ダメージを受けた時に再生する音
    /// </summary>
    public AudioClip damageAudio;

    /// <summary>
    /// ダメージ音の音量
    /// </summary>
    public float damageVolume = 1f;

    /// <summary>
    /// Rigidbody2D
    /// </summary>
    private Rigidbody2D rb;

    /// <summary>
    /// InputActionから読み取った移動入力
    /// </summary>
    private Vector2 moveInput;

    /// <summary>
    /// 移動用のInputAction
    /// </summary>
    private InputAction moveAction;

    /// <summary>
    /// 最後に向いた移動方向(IceBulletに使用)
    /// </summary>
    private Vector2 lastMoveDirection = Vector2.right;

    /// <summary>
    /// HPバーのスケール設定
    /// </summary>
    private Vector3 hpBarInitialScale;

    /// <summary>
    /// XPバーのスケール設定
    /// </summary>
    private Vector3 xpBarInitialScale;

    /// <summary>
    /// ゲームオーバー状態かどうか
    /// </summary>
    private bool isGameOver = false;

    /// <summary>
    /// 移動処理&HP・XPバーの更新
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        moveAction = new InputAction("Move", InputActionType.Value, "<Gameplay>/Move");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (hpBar != null)
        {
            hpBarInitialScale = hpBar.transform.localScale;
        }

        if (xpBar != null)
        {
            xpBarInitialScale = xpBar.transform.localScale;

            Vector3 scale = xpBarInitialScale;
            scale.x = 0f;
            xpBar.transform.localScale = scale;
        }
    }

    /// <summary>
    /// スタート時の処理
    /// </summary>
    private void Start()
    {
        Time.timeScale = 1f;

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }

        hp = maxHp;
        UpdateHpBar();
        UpdateXpBar();
    }

    /// <summary>
    /// プレイヤーの有効時
    /// </summary>
    private void OnEnable()
    {
        moveAction.Enable();
    }

    /// <summary>
    /// プレイヤーの無効時
    /// </summary>
    private void OnDisable()
    {
        moveAction.Disable();
    }

    /// <summary>
    /// 毎フレームの処理
    /// </summary>
    private void Update()
    {
        if (isGameOver)
        {
            return;
        }

        moveInput = moveAction.ReadValue<Vector2>();

        if (moveInput != Vector2.zero)
        {
            lastMoveDirection = moveInput;
        }

        UpdateHpBar();
        UpdateXpBar();
    }

    /// <summary>
    /// 移動速度の適用
    /// </summary>
    private void FixedUpdate()
    {
        if (isGameOver)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = moveInput * moveSpeed;
    }

    /// <summary>
    /// 最後に移動した方向を取得
    /// </summary>
    /// <returns></returns>
    public Vector2 GetLastMoveDirection()
    {
        return lastMoveDirection;
    }

    /// <summary>
    /// ダメージを受けたときの処理
    /// </summary>
    /// <param name="amount"></param>
    public void TakeDamage(float amount)
    {
        if (isGameOver) return;

        hp -= amount;
        hp = Mathf.Clamp(hp, 0f, maxHp);

        PlayDamageAudio();
        UpdateHpBar();

        if (hp <= 0f)
        {
            GameOver();
        }
    }

    /// <summary>
    /// ダメージ音を再生
    /// </summary>
    private void PlayDamageAudio()
    {
        if (damageAudio == null)
        {
            return;
        }

        Vector3 playPosition = transform.position;

        if (Camera.main != null)
        {
            playPosition = Camera.main.transform.position;
        }

        AudioSource.PlayClipAtPoint(damageAudio, playPosition, damageVolume);
    }

    /// <summary>
    /// HP回復処理
    /// </summary>
    /// <param name="amount"></param>
    public void Heal(float amount)
    {
        if (isGameOver) return;

        hp += amount;

        if (hp > maxHp)
        {
            hp = maxHp;
        }

        UpdateHpBar();
    }

    /// <summary>
    /// HPバーの更新処理
    /// </summary>
    private void UpdateHpBar()
    {
        if (hpBar == null || maxHp <= 0f)
        {
            return;
        }

        float hpPercent = Mathf.Clamp01(hp / maxHp);

        Vector3 scale = hpBarInitialScale;
        scale.x = hpPercent;
        hpBar.transform.localScale = scale;
    }

    /// <summary>
    /// XPバーの更新処理
    /// </summary>
    private void UpdateXpBar()
    {
        if (xpBar == null)
        {
            return;
        }

        if (xpToNextLevel == null || level >= xpToNextLevel.Length)
        {
            Vector3 maxScale = xpBarInitialScale;
            maxScale.x = 1f;
            xpBar.transform.localScale = maxScale;
            return;
        }

        int requiredXp = xpToNextLevel[level];

        if (requiredXp <= 0)
        {
            return;
        }

        float xpPercent = Mathf.Clamp01((float)currentXp / requiredXp);

        Vector3 scale = xpBarInitialScale;
        scale.x = xpPercent;
        xpBar.transform.localScale = scale;
    }

    /// <summary>
    /// ゲームオーバーの時の処理
    /// </summary>
    private void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        moveInput = Vector2.zero;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    /// <summary>
    /// 経験値取得処理
    /// </summary>
    /// <param name="amount"></param>
    public void AddXp(int amount)
    {
        currentXp += amount;

        CheckLevelUp();
        UpdateXpBar();
    }

    /// <summary>
    /// レベルアップ時の処理
    /// </summary>
    private void CheckLevelUp()
    {
        if (xpToNextLevel == null || level >= xpToNextLevel.Length)
        {
            return;
        }

        int requiredXp = xpToNextLevel[level];
        bool leveledUp = false;

        while (currentXp >= requiredXp)
        {
            currentXp -= requiredXp;
            level++;
            leveledUp = true;

            if (level < xpToNextLevel.Length)
            {
                requiredXp = xpToNextLevel[level];
            }
            else
            {
                break;
            }
        }

        if (leveledUp)
        {
            UpdateXpBar();
            OnLevelUp?.Invoke();
        }
    }
}