using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 敵の移動・HP・攻撃・経験値ドロップ・Pool返却を管理するスクリプト
/// </summary>
public class EnemyController : MonoBehaviour
{
    [Header("ステータス")]
    /// <summary>
    /// 敵の最大HP
    /// </summary>
    public float maxHp = 10f;

    /// <summary>
    /// プレイヤーへ与えるダメージ
    /// </summary>
    public float attackPower = 1f;

    /// <summary>
    /// 移動速度
    /// </summary>
    public float speed = 2f;

    [Header("経験値")]
    [Range(0, 100)]
    [Tooltip("経験値オーブを落とす確率（0〜100%）")]
    /// <summary>
    /// 経験値オーブを落とす確率
    /// </summary>
    public int xpPercent = 70;

    [Tooltip("生成する経験値オーブのプレハブ")]
    /// <summary>
    /// 生成する経験値オーブPrefab
    /// </summary>
    public GameObject xpOrbPrefab;

    [Tooltip("オーブが出現する位置のオフセット（Y軸を少し上げる用）")]
    /// <summary>
    /// 経験値オーブの出現位置オフセット
    /// </summary>
    public Vector3 xpDropOffset = new Vector3(0f, 0.5f, 0f);

    [Header("ダメージ表示")]
    /// <summary>
    /// ダメージ表示の出現位置オフセット
    /// </summary>
    public Vector3 damageNumberOffset = new Vector3(0f, 1f, 0f);

    [Tooltip("プレイヤーに攻撃する間隔（秒）")]
    /// <summary>
    /// プレイヤーへ攻撃する間隔
    /// </summary>
    public float attackSpeed = 1.0f;

    /// <summary>
    /// 現在HP
    /// </summary>
    private float hp;

    /// <summary>
    /// 次に攻撃できる時間
    /// </summary>
    private float nextAttackTime = 1.0f;

    /// <summary>
    /// プレイヤーに接触中かどうか
    /// </summary>
    private bool isTouchingPlayer = false;

    /// <summary>
    /// 接触中のPlayerController
    /// </summary>
    private PlayerController playerController;

    /// <summary>
    /// プレイヤーのTransform
    /// </summary>
    private Transform playerTransform;

    /// <summary>
    /// Animator参照
    /// </summary>
    private Animator animator;

    /// <summary>
    /// ダメージ表示Manager参照
    /// </summary>
    private DamageNumberManager damageNumberManager;

    /// <summary>
    /// Enemy用ObjectPool
    /// </summary>
    private ObjectPool<GameObject> pool;

    /// <summary>
    /// Poolへ返却済みかどうか
    /// </summary>
    private bool isReleased;

    /// <summary>
    /// DamageトリガーのHash
    /// </summary>
    private static readonly int DamageTriggerHash = Animator.StringToHash("Damage");

    /// <summary>
    /// 開始時の参照取得
    /// </summary>
    private void Start()
    {
        animator = GetComponent<Animator>();

        damageNumberManager = FindFirstObjectByType<DamageNumberManager>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    /// <summary>
    /// 有効化時の初期化
    /// </summary>
    private void OnEnable()
    {
        hp = maxHp;
        isReleased = false;
        isTouchingPlayer = false;
        playerController = null;
        nextAttackTime = Time.time + attackSpeed;
    }

    /// <summary>
    /// Pool参照を設定
    /// </summary>
    /// <param name="enemyPool"></param>
    public void SetPool(ObjectPool<GameObject> enemyPool)
    {
        pool = enemyPool;
    }

    /// <summary>
    /// プレイヤー追尾と攻撃タイミングの管理
    /// </summary>
    private void Update()
    {
        if (playerTransform != null && !isTouchingPlayer)
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }

        if (isTouchingPlayer && Time.time >= nextAttackTime)
        {
            AttackPlayer();
        }
    }

    /// <summary>
    /// ダメージを受ける処理
    /// </summary>
    /// <param name="amount"></param>
    public void TakeDamage(float amount)
    {
        hp -= amount;

        if (damageNumberManager != null)
        {
            damageNumberManager.ShowDamage(amount, transform.position + damageNumberOffset);
        }

        if (animator != null)
        {
            animator.ResetTrigger(DamageTriggerHash);
            animator.SetTrigger(DamageTriggerHash);
        }

        if (hp <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// プレイヤーへダメージを与える
    /// </summary>
    private void AttackPlayer()
    {
        if (playerController == null)
        {
            return;
        }

        playerController.TakeDamage(attackPower);
        nextAttackTime = Time.time + attackSpeed;
    }

    /// <summary>
    /// 敵が倒された時の処理
    /// </summary>
    private void Die()
    {
        if (isReleased) return;

        DropXpOrb();

        isReleased = true;

        if (pool != null)
        {
            pool.Release(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 経験値オーブを確率で生成
    /// </summary>
    private void DropXpOrb()
    {
        if (xpOrbPrefab == null)
        {
            return;
        }

        int randomValue = Random.Range(0, 100);
        if (randomValue < xpPercent)
        {
            Vector3 spawnPosition = transform.position + xpDropOffset;
            Instantiate(xpOrbPrefab, spawnPosition, Quaternion.identity);
        }
    }

    /// <summary>
    /// Playerに触れた時の処理
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerController = collision.GetComponent<PlayerController>();

            if (playerController == null)
            {
                playerController = collision.GetComponentInParent<PlayerController>();
            }

            if (playerController != null)
            {
                isTouchingPlayer = true;

                if (Time.time >= nextAttackTime)
                {
                    AttackPlayer();
                }
            }
        }
    }

    /// <summary>
    /// Playerから離れた時の処理
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isTouchingPlayer = false;
            playerController = null;
        }
    }
}