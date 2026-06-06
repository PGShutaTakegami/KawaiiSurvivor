using TMPro;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// ダメージ数値の表示・移動・Pool返却を管理するスクリプト
/// </summary>
public class DamageNumber : MonoBehaviour
{
    [Header("寿命")]
    /// <summary>
    /// 表示される時間
    /// </summary>
    public float lifeTime = 1f;

    [Header("UI移動")]
    /// <summary>
    /// ダメージ数値の移動速度
    /// </summary>
    public Vector2 moveVelocity = new Vector2(0f, 80f);

    /// <summary>
    /// DamageNumber用ObjectPool
    /// </summary>
    private ObjectPool<GameObject> pool;

    /// <summary>
    /// ダメージ数値を表示するText
    /// </summary>
    private TMP_Text damageText;

    /// <summary>
    /// UI座標用RectTransform
    /// </summary>
    private RectTransform rectTransform;

    /// <summary>
    /// 寿命タイマー
    /// </summary>
    private float timer;

    /// <summary>
    /// Poolへ返却済みかどうか
    /// </summary>
    private bool isReleased;

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        damageText = GetComponentInChildren<TMP_Text>(true);
        rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// Pool参照を設定
    /// </summary>
    /// <param name="damageNumberPool"></param>
    public void SetPool(ObjectPool<GameObject> damageNumberPool)
    {
        pool = damageNumberPool;
    }

    /// <summary>
    /// 表示するダメージ値を設定
    /// </summary>
    /// <param name="damage"></param>
    public void Setup(float damage)
    {
        if (damageText == null)
        {
            damageText = GetComponentInChildren<TMP_Text>(true);
        }

        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        if (damageText != null)
        {
            damageText.text = Mathf.RoundToInt(damage).ToString();
            damageText.enabled = true;
            damageText.gameObject.SetActive(true);
        }

        timer = lifeTime;
        isReleased = false;
    }

    /// <summary>
    /// 有効化時の初期化
    /// </summary>
    private void OnEnable()
    {
        timer = lifeTime;
        isReleased = false;
    }

    /// <summary>
    /// ダメージ数値の移動と寿命管理
    /// </summary>
    private void Update()
    {
        if (isReleased) return;

        if (rectTransform != null)
        {
            rectTransform.anchoredPosition += moveVelocity * Time.deltaTime;
        }

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            ReturnToPool();
        }
    }

    /// <summary>
    /// DamageNumberをPoolへ戻す
    /// </summary>
    public void ReturnToPool()
    {
        if (isReleased) return;

        isReleased = true;

        if (pool != null)
        {
            pool.Release(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}