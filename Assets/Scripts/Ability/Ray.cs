using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rayアビリティの表示・当たり判定・連続ヒット・Pool返却を管理するスクリプト
/// </summary>
public class Ray : MonoBehaviour, IAbilitySetup
{
    [Header("表示するオブジェクト")]
    /// <summary>
    /// 表示用オブジェクト
    /// </summary>
    public GameObject visualObject;

    [Header("同じ敵に連続ヒットする間隔")]
    /// <summary>
    /// 同じ敵に再ヒットできるまでの時間
    /// </summary>
    public float hitInterval = 0.25f;

    [Header("ランダム回転")]
    /// <summary>
    /// 生成時にZ回転をランダムにするかどうか
    /// </summary>
    public bool useRandomZRotation = true;

    /// <summary>
    /// ランダム回転の最小値
    /// </summary>
    public float minZRotation = 0f;

    /// <summary>
    /// ランダム回転の最大値
    /// </summary>
    public float maxZRotation = 360f;

    /// <summary>
    /// アビリティ名
    /// </summary>
    private string abilityName;

    /// <summary>
    /// AbilityManager参照
    /// </summary>
    private AbilityManager abilityManager;

    /// <summary>
    /// 現在のレベル
    /// </summary>
    private int level;

    /// <summary>
    /// アビリティの寿命
    /// </summary>
    private float lifeTime;

    /// <summary>
    /// 敵に与えるダメージ
    /// </summary>
    private float damage;

    /// <summary>
    /// 当たり判定
    /// </summary>
    private Collider2D hitbox;

    /// <summary>
    /// 寿命タイマー
    /// </summary>
    private float timer;

    /// <summary>
    /// 攻撃中かどうか
    /// </summary>
    private bool isAttacking;

    /// <summary>
    /// Poolへ返却済みかどうか
    /// </summary>
    private bool isReleased;

    /// <summary>
    /// 敵ごとの次回ヒット可能時間
    /// </summary>
    private readonly Dictionary<EnemyController, float> nextHitTimes = new Dictionary<EnemyController, float>();

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        hitbox = GetComponent<Collider2D>();

        if (visualObject != null)
        {
            visualObject.SetActive(false);
        }

        if (hitbox != null)
        {
            hitbox.enabled = false;
        }
    }

    /// <summary>
    /// アビリティ生成時の設定を反映
    /// </summary>
    /// <param name="data"></param>
    public void SetupAbility(AbilityRuntimeData data)
    {
        abilityManager = data.manager;
        abilityName = data.abilityName;

        level = data.level;
        lifeTime = data.lifeTime;
        damage = data.damage * level;

        timer = lifeTime;
        isReleased = false;
        isAttacking = true;
        nextHitTimes.Clear();

        if (useRandomZRotation)
        {
            float randomZ = Random.Range(minZRotation, maxZRotation);
            transform.rotation = Quaternion.Euler(0f, 0f, randomZ);
        }

        if (visualObject != null)
        {
            visualObject.SetActive(true);
        }

        if (hitbox != null)
        {
            hitbox.enabled = true;
        }
    }

    /// <summary>
    /// 寿命を管理
    /// </summary>
    private void Update()
    {
        if (isReleased) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            ReturnToPool();
        }
    }

    /// <summary>
    /// 敵に触れている間のダメージ処理
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isAttacking) return;
        if (!collision.CompareTag("Enemy")) return;

        EnemyController enemy = collision.GetComponent<EnemyController>();
        if (enemy == null) return;

        if (nextHitTimes.TryGetValue(enemy, out float nextHitTime))
        {
            if (Time.time < nextHitTime)
            {
                return;
            }
        }

        // 同じ敵にはhitIntervalごとにダメージを与える
        enemy.TakeDamage(damage);
        nextHitTimes[enemy] = Time.time + hitInterval;
    }

    /// <summary>
    /// RayをPoolへ戻す
    /// </summary>
    private void ReturnToPool()
    {
        if (isReleased) return;

        isReleased = true;
        isAttacking = false;
        nextHitTimes.Clear();

        if (visualObject != null)
        {
            visualObject.SetActive(false);
        }

        if (hitbox != null)
        {
            hitbox.enabled = false;
        }

        if (abilityManager != null)
        {
            abilityManager.ReleaseAbility(abilityName, gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 無効化時の処理
    /// </summary>
    private void OnDisable()
    {
        isAttacking = false;
        nextHitTimes.Clear();

        if (hitbox != null)
        {
            hitbox.enabled = false;
        }

        if (visualObject != null)
        {
            visualObject.SetActive(false);
        }
    }
}