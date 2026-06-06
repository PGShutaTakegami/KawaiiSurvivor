using TMPro;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// ゲーム全体のタイマー・コイン・敵生成・ゲームクリアを管理するスクリプト
/// </summary>
public class GameController : MonoBehaviour
{
    /// <summary>
    /// 敵1種類分の生成設定
    /// </summary>
    [System.Serializable]
    public class EnemyData
    {
        [Tooltip("敵の名前")]
        /// <summary>
        /// 敵の名前
        /// </summary>
        public string name;

        [Tooltip("敵のPrefab")]
        /// <summary>
        /// 敵Prefab
        /// </summary>
        public GameObject enemyPrefab;

        [Tooltip("敵の出現重み（高いほど出やすい）")]
        /// <summary>
        /// 敵の出現重み
        /// </summary>
        public float spawnRate = 10f;

        [Tooltip("初期に用意しておく敵の数")]
        /// <summary>
        /// 初期生成数
        /// </summary>
        public int spawnPool = 10;

        [Tooltip("Poolに保持できる最大数。超えた分はDestroyされる")]
        /// <summary>
        /// Poolの最大保持数
        /// </summary>
        public int maxSpawnPool = 30;

        /// <summary>
        /// 敵用ObjectPool
        /// </summary>
        [HideInInspector] public ObjectPool<GameObject> pool;
    }

    [Header("敵の設定")]
    /// <summary>
    /// 生成する敵の一覧
    /// </summary>
    public EnemyData[] enemies;

    [Header("スポーン頻度（秒）")]
    /// <summary>
    /// 敵の生成間隔
    /// </summary>
    public float spawnInterval = 1.5f;

    [Header("プレイヤーから離れすぎ防止の最大半径")]
    /// <summary>
    /// 敵の最大生成半径
    /// </summary>
    public float maxSpawnRadius = 15f;

    [Header("スコア")]
    /// <summary>
    /// 現在スコア
    /// </summary>
    public int score;

    [Header("コイン")]
    /// <summary>
    /// 現在のコイン数
    /// </summary>
    public int coin;

    /// <summary>
    /// コイン表示用Text
    /// </summary>
    public TMP_Text coinText;

    [Header("タイマー")]
    /// <summary>
    /// ゲーム時間
    /// </summary>
    public float gameDuration = 300f;

    /// <summary>
    /// 現在の残り時間
    /// </summary>
    public float gameTimer;

    /// <summary>
    /// タイマー表示用Text
    /// </summary>
    public TMP_Text timerText;

    /// <summary>
    /// ゲームクリア時に表示するUI
    /// </summary>
    public GameObject gameCompleteUI;

    /// <summary>
    /// PlayerAreaのTransform
    /// </summary>
    private Transform areaTransform;

    /// <summary>
    /// PlayerAreaのCircleCollider2D
    /// </summary>
    private CircleCollider2D safeZoneCollider;

    /// <summary>
    /// ゲーム終了済みかどうか
    /// </summary>
    private bool isGameFinished = false;

    /// <summary>
    /// 開始時の初期化処理
    /// </summary>
    private void Start()
    {
        gameTimer = gameDuration;

        UpdateCoinText();
        UpdateTimerText();

        GameObject areaObj = GameObject.FindGameObjectWithTag("PlayerArea");

        if (areaObj != null)
        {
            areaTransform = areaObj.transform;
            safeZoneCollider = areaObj.GetComponent<CircleCollider2D>();
        }

        InitializePools();

        InvokeRepeating(nameof(SpawnEnemyOutside), spawnInterval, spawnInterval);
    }

    /// <summary>
    /// タイマー更新とゲームクリア判定
    /// </summary>
    private void Update()
    {
        if (isGameFinished)
        {
            return;
        }

        gameTimer -= Time.deltaTime;
        if (gameTimer <= 0f)
        {
            gameTimer = 0f;
            UpdateTimerText();
            GameClear();
            return;
        }

        UpdateTimerText();
    }

    /// <summary>
    /// コインを加算
    /// </summary>
    /// <param name="amount"></param>
    public void AddCoin(int amount)
    {
        coin += amount;
        UpdateCoinText();
    }

    /// <summary>
    /// コイン表示を更新
    /// </summary>
    private void UpdateCoinText()
    {
        if (coinText != null)
        {
            coinText.text = coin.ToString();
        }
    }

    /// <summary>
    /// タイマー表示を更新
    /// </summary>
    private void UpdateTimerText()
    {
        if (timerText == null)
        {
            return;
        }

        int minutes = Mathf.FloorToInt(gameTimer / 60f);
        int seconds = Mathf.FloorToInt(gameTimer % 60f);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    /// <summary>
    /// ゲームクリア時の処理
    /// </summary>
    private void GameClear()
    {
        if (isGameFinished)
        {
            return;
        }

        isGameFinished = true;

        if (gameCompleteUI != null)
        {
            gameCompleteUI.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    /// <summary>
    /// 敵のObjectPoolを初期化
    /// </summary>
    private void InitializePools()
    {
        foreach (EnemyData enemy in enemies)
        {
            if (enemy.enemyPrefab == null) continue;

            EnemyData currentEnemy = enemy;

            currentEnemy.pool = new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    GameObject obj = Instantiate(currentEnemy.enemyPrefab);
                    obj.SetActive(false);

                    EnemyController enemyController = obj.GetComponent<EnemyController>();
                    if (enemyController != null)
                    {
                        enemyController.SetPool(currentEnemy.pool);
                    }

                    return obj;
                },
                actionOnGet: obj =>
                {
                    obj.SetActive(true);
                },
                actionOnRelease: obj =>
                {
                    obj.SetActive(false);
                },
                actionOnDestroy: obj =>
                {
                    Destroy(obj);
                },
                collectionCheck: false,
                defaultCapacity: Mathf.Max(0, currentEnemy.spawnPool),
                maxSize: Mathf.Max(1, currentEnemy.maxSpawnPool)
            );

            for (int i = 0; i < currentEnemy.spawnPool; i++)
            {
                GameObject obj = currentEnemy.pool.Get();
                currentEnemy.pool.Release(obj);
            }
        }
    }

    /// <summary>
    /// PlayerAreaの外側に敵を生成
    /// </summary>
    private void SpawnEnemyOutside()
    {
        if (isGameFinished)
        {
            return;
        }

        if (areaTransform == null || safeZoneCollider == null || enemies == null || enemies.Length == 0) return;

        EnemyData selectedEnemy = GetRandomEnemyData();
        if (selectedEnemy == null || selectedEnemy.pool == null) return;

        Vector3 centerPos = areaTransform.position;
        float minSpawnRadius = safeZoneCollider.radius * areaTransform.lossyScale.x;

        Vector3 spawnPosition = GetRandomPositionInDonut(centerPos, minSpawnRadius, maxSpawnRadius);

        GameObject enemy = selectedEnemy.pool.Get();
        enemy.transform.position = spawnPosition;
        enemy.transform.rotation = Quaternion.identity;
    }

    /// <summary>
    /// 円形範囲の外側かつ最大半径以内のランダム座標を取得
    /// </summary>
    /// <param name="center"></param>
    /// <param name="minRadius"></param>
    /// <param name="maxRadius"></param>
    /// <returns></returns>
    private Vector3 GetRandomPositionInDonut(Vector3 center, float minRadius, float maxRadius)
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        float distance = Random.Range(minRadius, maxRadius);

        return center + new Vector3(direction.x * distance, direction.y * distance, 0f);
    }

    /// <summary>
    /// 出現重みに応じて敵データを取得
    /// </summary>
    /// <returns></returns>
    private EnemyData GetRandomEnemyData()
    {
        float totalRate = 0f;

        foreach (EnemyData enemy in enemies)
        {
            if (enemy.enemyPrefab != null && enemy.spawnRate > 0f)
            {
                totalRate += enemy.spawnRate;
            }
        }

        if (totalRate <= 0f) return null;

        float randomValue = Random.Range(0f, totalRate);
        float currentSum = 0f;

        foreach (EnemyData enemy in enemies)
        {
            if (enemy.enemyPrefab == null || enemy.spawnRate <= 0f) continue;

            currentSum += enemy.spawnRate;
            if (randomValue <= currentSum)
            {
                return enemy;
            }
        }

        return null;
    }
}