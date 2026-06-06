using UnityEngine;

/// <summary>
/// 弾の移動・ダメージ・貫通・Pool返却を管理するスクリプト
/// </summary>
public class Bullet : MonoBehaviour, IAbilitySetup
{
    /// <summary>
    /// アビリティ名
    /// </summary>
    private string abilityName;

    /// <summary>
    /// AbilityManager参照
    /// </summary>
    private AbilityManager abilityManager;

    /// <summary>
    /// 弾の移動速度
    /// </summary>
    private float speed;

    /// <summary>
    /// 弾の寿命
    /// </summary>
    private float lifeTime;

    /// <summary>
    /// 敵に与えるダメージ
    /// </summary>
    private float damage;

    /// <summary>
    /// 残り貫通回数
    /// </summary>
    private int penetration;

    /// <summary>
    /// 弾の進行方向
    /// </summary>
    private Vector2 travelDirection = Vector2.right;

    /// <summary>
    /// 寿命タイマー
    /// </summary>
    private float lifeTimer;

    /// <summary>
    /// Poolへ返却済みかどうか
    /// </summary>
    private bool isReleased;

    /// <summary>
    /// アビリティ生成時の設定を反映
    /// </summary>
    /// <param name="data"></param>
    public void SetupAbility(AbilityRuntimeData data)
    {
        abilityManager = data.manager;
        abilityName = data.abilityName;

        speed = data.speed;
        lifeTime = data.lifeTime;
        damage = data.damage * data.level;
        penetration = data.penetration;

        lifeTimer = lifeTime;
        isReleased = false;

        if (data.player != null)
        {
            Vector2 direction = data.player.GetLastMoveDirection();

            if (direction != Vector2.zero)
            {
                travelDirection = direction.normalized;
            }
        }

        // 進行方向に合わせて弾を回転
        float angle = Mathf.Atan2(travelDirection.y, travelDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    /// <summary>
    /// 弾の移動と寿命管理
    /// </summary>
    private void Update()
    {
        if (isReleased) return;

        transform.Translate(travelDirection * speed * Time.deltaTime, Space.World);

        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
        {
            ReturnToPool();
        }
    }

    /// <summary>
    /// 敵に当たった時の処理
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReleased) return;
        if (!collision.CompareTag("Enemy")) return;

        EnemyController enemy = collision.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        // 貫通回数が残っていれば減らし、なければPoolへ戻す
        if (penetration > 0)
        {
            penetration--;
        }
        else
        {
            ReturnToPool();
        }
    }

    /// <summary>
    /// 弾をPoolへ戻す
    /// </summary>
    private void ReturnToPool()
    {
        if (isReleased) return;

        isReleased = true;

        if (abilityManager != null)
        {
            abilityManager.ReleaseAbility(abilityName, gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}