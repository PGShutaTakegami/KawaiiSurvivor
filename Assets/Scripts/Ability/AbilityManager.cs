using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// アビリティ1つ分の設定データ
/// </summary>
[System.Serializable]
public class AbilityData
{
    [Header("基本")]
    /// <summary>
    /// アビリティ名
    /// </summary>
    public string name;

    /// <summary>
    /// アビリティPrefab
    /// </summary>
    public GameObject abilityPrefab;

    /// <summary>
    /// 解放済みかどうか
    /// </summary>
    public bool isUnlocked;

    [Header("レベル")]
    /// <summary>
    /// レベルを使用するかどうか
    /// </summary>
    public bool useLevel = true;

    /// <summary>
    /// 現在のレベル
    /// </summary>
    public int level = 1;

    [Header("速度")]
    /// <summary>
    /// 速度を使用するかどうか
    /// </summary>
    public bool useSpeed = false;

    /// <summary>
    /// アビリティの速度
    /// </summary>
    public float speed = 5f;

    [Header("寿命")]
    /// <summary>
    /// 寿命を使用するかどうか
    /// </summary>
    public bool useLifeTime = true;

    /// <summary>
    /// アビリティの寿命
    /// </summary>
    public float lifeTime = 3f;

    [Header("ダメージ")]
    /// <summary>
    /// ダメージを使用するかどうか
    /// </summary>
    public bool useDamage = true;

    /// <summary>
    /// 基礎ダメージ
    /// </summary>
    public float damage = 100f;

    [Header("貫通")]
    /// <summary>
    /// 貫通を使用するかどうか
    /// </summary>
    public bool usePenetration = false;

    /// <summary>
    /// 貫通回数
    /// </summary>
    public int penetration = 0;

    [Header("攻撃頻度")]
    /// <summary>
    /// 攻撃頻度を使用するかどうか
    /// </summary>
    public bool useAttackSpeed = true;

    /// <summary>
    /// 攻撃間隔
    /// </summary>
    public float attackSpeed = 1.2f;

    [Header("生成位置")]
    /// <summary>
    /// 生成位置のオフセットを使用するかどうか
    /// </summary>
    public bool useSpawnOffset = false;

    /// <summary>
    /// 生成位置のオフセット
    /// </summary>
    public Vector3 spawnOffset = Vector3.zero;

    [Header("生成後にPlayerの子にする")]
    /// <summary>
    /// 生成後にPlayerの子にするかどうか
    /// </summary>
    public bool parentToPlayer = false;

    [Header("Pool")]
    /// <summary>
    /// 初期生成数
    /// </summary>
    public int spawnPool = 10;

    /// <summary>
    /// 最大保持数
    /// </summary>
    public int maxSpawnPool = 30;

    /// <summary>
    /// ObjectPool
    /// </summary>
    [HideInInspector] public ObjectPool<GameObject> pool;

    /// <summary>
    /// 攻撃タイマー
    /// </summary>
    [HideInInspector] public float timer;

    /// <summary>
    /// 常駐系アビリティの現在オブジェクト
    /// </summary>
    [HideInInspector] public GameObject activeObject;

    /// <summary>
    /// 適用済みレベル
    /// </summary>
    [HideInInspector] public int appliedLevel = -1;
}

/// <summary>
/// アビリティ実行時に渡すデータ
/// </summary>
public struct AbilityRuntimeData
{
    /// <summary>
    /// AbilityManager
    /// </summary>
    public AbilityManager manager;

    /// <summary>
    /// アビリティ名
    /// </summary>
    public string abilityName;

    /// <summary>
    /// プレイヤー
    /// </summary>
    public PlayerController player;

    /// <summary>
    /// レベル
    /// </summary>
    public int level;

    /// <summary>
    /// 速度
    /// </summary>
    public float speed;

    /// <summary>
    /// 寿命
    /// </summary>
    public float lifeTime;

    /// <summary>
    /// ダメージ
    /// </summary>
    public float damage;

    /// <summary>
    /// 貫通回数
    /// </summary>
    public int penetration;
}

/// <summary>
/// アビリティ初期化用インターフェース
/// </summary>
public interface IAbilitySetup
{
    /// <summary>
    /// アビリティの設定を反映
    /// </summary>
    /// <param name="data"></param>
    void SetupAbility(AbilityRuntimeData data);
}

/// <summary>
/// アビリティの解放・レベルアップ・生成・Pool管理
/// </summary>
public class AbilityManager : MonoBehaviour
{
    [Header("プレイヤー")]
    /// <summary>
    /// プレイヤー参照
    /// </summary>
    public PlayerController player;

    [Header("アビリティ一覧")]
    /// <summary>
    /// 登録するアビリティ一覧
    /// </summary>
    public AbilityData[] abilities;

    /// <summary>
    /// 開始時の処理
    /// </summary>
    private void Start()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerController>();
        }

        InitializePools();
    }

    /// <summary>
    /// アビリティの使用タイミングを管理
    /// </summary>
    private void Update()
    {
        foreach (AbilityData ability in abilities)
        {
            if (!ability.isUnlocked) continue;

            if (!ability.useAttackSpeed)
            {
                int currentLevel = ability.useLevel ? Mathf.Max(1, ability.level) : 1;

                if (ability.activeObject != null && ability.appliedLevel != currentLevel)
                {
                    ApplyRuntimeData(ability, ability.activeObject);
                    ability.appliedLevel = currentLevel;
                }

                continue;
            }

            ability.timer -= Time.deltaTime;

            if (ability.timer <= 0f)
            {
                UseAbility(ability);
                ability.timer = GetAttackInterval(ability);
            }
        }
    }

    /// <summary>
    /// 各アビリティのObjectPoolを初期化
    /// </summary>
    private void InitializePools()
    {
        foreach (AbilityData ability in abilities)
        {
            if (ability.abilityPrefab == null) continue;

            AbilityData currentAbility = ability;

            currentAbility.pool = new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    GameObject obj = Instantiate(currentAbility.abilityPrefab);
                    obj.SetActive(false);
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
                defaultCapacity: Mathf.Max(0, currentAbility.spawnPool),
                maxSize: Mathf.Max(1, currentAbility.maxSpawnPool)
            );

            for (int i = 0; i < currentAbility.spawnPool; i++)
            {
                GameObject obj = currentAbility.pool.Get();
                currentAbility.pool.Release(obj);
            }

            currentAbility.timer = GetAttackInterval(currentAbility);

            if (currentAbility.isUnlocked && !currentAbility.useAttackSpeed)
            {
                UseAbility(currentAbility);
            }
        }
    }

    /// <summary>
    /// アビリティを生成して実行
    /// </summary>
    /// <param name="ability"></param>
    private void UseAbility(AbilityData ability)
    {
        if (player == null || ability.pool == null) return;

        Vector3 offset = ability.useSpawnOffset ? ability.spawnOffset : Vector3.zero;
        GameObject obj = ability.pool.Get();

        if (ability.parentToPlayer)
        {
            obj.transform.SetParent(player.transform);
            obj.transform.localPosition = offset;
            obj.transform.localRotation = Quaternion.identity;
        }
        else
        {
            obj.transform.SetParent(null);
            obj.transform.position = player.transform.position + offset;
            obj.transform.rotation = Quaternion.identity;
        }

        ability.activeObject = obj;
        ApplyRuntimeData(ability, obj);
    }

    /// <summary>
    /// アビリティに現在の設定値を渡す
    /// </summary>
    /// <param name="ability"></param>
    /// <param name="obj"></param>
    private void ApplyRuntimeData(AbilityData ability, GameObject obj)
    {
        if (obj == null) return;

        AbilityRuntimeData data = new AbilityRuntimeData
        {
            manager = this,
            abilityName = ability.name,
            player = player,
            level = ability.useLevel ? Mathf.Max(1, ability.level) : 1,
            speed = ability.useSpeed ? ability.speed : 0f,
            lifeTime = ability.useLifeTime ? ability.lifeTime : 0f,
            damage = ability.useDamage ? ability.damage : 0f,
            penetration = ability.usePenetration ? ability.penetration : 0
        };

        IAbilitySetup[] setups = obj.GetComponents<IAbilitySetup>();
        foreach (IAbilitySetup setup in setups)
        {
            setup.SetupAbility(data);
        }

        ability.appliedLevel = data.level;
    }

    /// <summary>
    /// アビリティをPoolに戻す
    /// </summary>
    /// <param name="abilityName"></param>
    /// <param name="abilityObject"></param>
    public void ReleaseAbility(string abilityName, GameObject abilityObject)
    {
        if (abilityObject == null) return;

        AbilityData ability = GetAbilityData(abilityName);

        if (ability == null || ability.pool == null)
        {
            Destroy(abilityObject);
            return;
        }

        if (ability.activeObject == abilityObject)
        {
            ability.activeObject = null;
        }

        abilityObject.transform.SetParent(null);
        ability.pool.Release(abilityObject);
    }

    /// <summary>
    /// Prefab指定で解放またはレベルアップ
    /// </summary>
    /// <param name="abilityPrefab"></param>
    public void UnlockOrLevelUpAbilityByPrefab(GameObject abilityPrefab)
    {
        AbilityData ability = GetAbilityDataByPrefab(abilityPrefab);
        if (ability == null) return;

        if (!ability.isUnlocked)
        {
            UnlockAbilityData(ability);
        }
        else
        {
            LevelUpAbilityData(ability);
        }
    }

    /// <summary>
    /// Prefab指定で解放
    /// </summary>
    /// <param name="abilityPrefab"></param>
    public void UnlockAbilityByPrefab(GameObject abilityPrefab)
    {
        AbilityData ability = GetAbilityDataByPrefab(abilityPrefab);
        UnlockAbilityData(ability);
    }

    /// <summary>
    /// Prefab指定でレベルアップ
    /// </summary>
    /// <param name="abilityPrefab"></param>
    public void LevelUpAbilityByPrefab(GameObject abilityPrefab)
    {
        AbilityData ability = GetAbilityDataByPrefab(abilityPrefab);
        LevelUpAbilityData(ability);
    }

    /// <summary>
    /// 名前指定で解放
    /// </summary>
    /// <param name="abilityName"></param>
    public void UnlockAbility(string abilityName)
    {
        AbilityData ability = GetAbilityData(abilityName);
        UnlockAbilityData(ability);
    }

    /// <summary>
    /// 名前指定でレベルアップ
    /// </summary>
    /// <param name="abilityName"></param>
    public void LevelUpAbility(string abilityName)
    {
        AbilityData ability = GetAbilityData(abilityName);
        LevelUpAbilityData(ability);
    }

    /// <summary>
    /// アビリティを解放
    /// </summary>
    /// <param name="ability"></param>
    private void UnlockAbilityData(AbilityData ability)
    {
        if (ability == null) return;

        ability.isUnlocked = true;
        ability.timer = 0f;

        if (!ability.useAttackSpeed && ability.activeObject == null)
        {
            UseAbility(ability);
        }
    }

    /// <summary>
    /// アビリティをレベルアップ
    /// </summary>
    /// <param name="ability"></param>
    private void LevelUpAbilityData(AbilityData ability)
    {
        if (ability == null) return;

        ability.level++;
        ability.timer = 0f;

        if (!ability.useAttackSpeed && ability.activeObject != null)
        {
            ApplyRuntimeData(ability, ability.activeObject);
        }
    }

    /// <summary>
    /// 攻撃間隔を取得
    /// </summary>
    /// <param name="ability"></param>
    /// <returns></returns>
    private float GetAttackInterval(AbilityData ability)
    {
        if (!ability.useAttackSpeed) return 999999f;

        return Mathf.Max(0.05f, ability.attackSpeed);
    }

    /// <summary>
    /// 名前からアビリティデータを取得
    /// </summary>
    /// <param name="abilityName"></param>
    /// <returns></returns>
    private AbilityData GetAbilityData(string abilityName)
    {
        foreach (AbilityData ability in abilities)
        {
            if (ability.name == abilityName)
            {
                return ability;
            }
        }

        return null;
    }

    /// <summary>
    /// Prefabからアビリティデータを取得
    /// </summary>
    /// <param name="abilityPrefab"></param>
    /// <returns></returns>
    private AbilityData GetAbilityDataByPrefab(GameObject abilityPrefab)
    {
        if (abilityPrefab == null)
        {
            return null;
        }

        foreach (AbilityData ability in abilities)
        {
            if (ability.abilityPrefab == abilityPrefab)
            {
                return ability;
            }
        }

        return null;
    }
}