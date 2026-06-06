using UnityEngine;

/// <summary>
/// Swordアビリティの表示・当たり判定・ダメージ・Pool返却を管理するスクリプト
/// </summary>
public class Sword : MonoBehaviour, IAbilitySetup
{
    [Header("表示するオブジェクト")]
    /// <summary>
    /// 表示用オブジェクト
    /// </summary>
    public GameObject visualObject;

    [Header("レベルアップごとの追加ダメージ")]
    /// <summary>
    /// 1レベルごとに追加されるダメージ量
    /// </summary>
    public float damageUpPerLevel = 300f;

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
    /// 攻撃中かどうか
    /// </summary>
    private bool isAttacking;

    /// <summary>
    /// Poolへ返却済みかどうか
    /// </summary>
    private bool isReleased;

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

        // Swordは1レベルごとにダメージだけを加算
        damage = data.damage + damageUpPerLevel * (level - 1);

        isReleased = false;
        isAttacking = true;

        if (visualObject != null)
        {
            visualObject.SetActive(true);
        }

        if (hitbox != null)
        {
            hitbox.enabled = true;
        }

        CancelInvoke(nameof(ReturnToPool));

        if (lifeTime > 0f)
        {
            Invoke(nameof(ReturnToPool), lifeTime);
        }
    }

    /// <summary>
    /// 敵に当たった時の処理
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isAttacking) return;
        if (!collision.CompareTag("Enemy")) return;

        EnemyController enemy = collision.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
    }

    /// <summary>
    /// SwordをPoolへ戻す
    /// </summary>
    private void ReturnToPool()
    {
        if (isReleased) return;

        isReleased = true;
        isAttacking = false;

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
        CancelInvoke(nameof(ReturnToPool));
        isAttacking = false;
    }
}