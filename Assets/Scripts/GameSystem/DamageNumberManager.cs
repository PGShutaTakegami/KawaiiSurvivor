using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// ダメージ数値の生成・表示位置変換・Pool管理を行うスクリプト
/// </summary>
public class DamageNumberManager : MonoBehaviour
{
    [Header("Damage Number Prefab")]
    /// <summary>
    /// ダメージ数値Prefab
    /// </summary>
    public GameObject damageNumberPrefab;

    [Header("表示先Canvas")]
    /// <summary>
    /// ダメージ数値を表示するCanvas
    /// </summary>
    public Canvas damageNumberCanvas;

    [Header("World Camera")]
    /// <summary>
    /// ワールド座標をスクリーン座標に変換するCamera
    /// </summary>
    public Camera worldCamera;

    [Header("Pool")]
    /// <summary>
    /// 初期生成数
    /// </summary>
    public int spawnPool = 20;

    /// <summary>
    /// Poolの最大保持数
    /// </summary>
    public int maxSpawnPool = 100;

    /// <summary>
    /// DamageNumber用ObjectPool
    /// </summary>
    private ObjectPool<GameObject> pool;

    /// <summary>
    /// CanvasのRectTransform
    /// </summary>
    private RectTransform canvasRectTransform;

    /// <summary>
    /// 開始時の参照取得とPool初期化
    /// </summary>
    private void Start()
    {
        if (damageNumberCanvas == null)
        {
            damageNumberCanvas = GetComponentInParent<Canvas>();
        }

        if (worldCamera == null)
        {
            worldCamera = Camera.main;
        }

        if (damageNumberCanvas != null)
        {
            canvasRectTransform = damageNumberCanvas.GetComponent<RectTransform>();
        }

        InitializePool();
    }

    /// <summary>
    /// DamageNumberのObjectPoolを初期化
    /// </summary>
    private void InitializePool()
    {
        if (damageNumberPrefab == null)
        {
            return;
        }

        if (damageNumberCanvas == null || canvasRectTransform == null)
        {
            return;
        }

        pool = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                GameObject obj = Instantiate(damageNumberPrefab);
                obj.transform.SetParent(damageNumberCanvas.transform, false);
                obj.SetActive(false);

                DamageNumber damageNumber = obj.GetComponent<DamageNumber>();
                if (damageNumber != null)
                {
                    damageNumber.SetPool(pool);
                }

                return obj;
            },
            actionOnGet: obj =>
            {
                obj.transform.SetParent(damageNumberCanvas.transform, false);
                obj.SetActive(true);
            },
            actionOnRelease: obj =>
            {
                obj.transform.SetParent(damageNumberCanvas.transform, false);
                obj.SetActive(false);
            },
            actionOnDestroy: obj =>
            {
                Destroy(obj);
            },
            collectionCheck: false,
            defaultCapacity: Mathf.Max(0, spawnPool),
            maxSize: Mathf.Max(1, maxSpawnPool)
        );

        for (int i = 0; i < spawnPool; i++)
        {
            GameObject obj = pool.Get();
            pool.Release(obj);
        }
    }

    /// <summary>
    /// 指定したワールド座標にダメージ数値を表示
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="worldPosition"></param>
    public void ShowDamage(float damage, Vector3 worldPosition)
    {
        if (pool == null || damageNumberCanvas == null || canvasRectTransform == null)
        {
            return;
        }

        GameObject obj = pool.Get();
        obj.transform.SetParent(damageNumberCanvas.transform, false);

        RectTransform damageRectTransform = obj.GetComponent<RectTransform>();
        if (damageRectTransform == null)
        {
            pool.Release(obj);
            return;
        }

        Camera canvasCamera = damageNumberCanvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : damageNumberCanvas.worldCamera;

        // ワールド座標をCanvas内のローカル座標に変換
        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(worldCamera, worldPosition);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            screenPosition,
            canvasCamera,
            out Vector2 localPosition
        );

        damageRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        damageRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        damageRectTransform.pivot = new Vector2(0.5f, 0.5f);
        damageRectTransform.anchoredPosition = localPosition;
        damageRectTransform.localScale = Vector3.one;
        damageRectTransform.localRotation = Quaternion.identity;

        DamageNumber damageNumber = obj.GetComponent<DamageNumber>();
        if (damageNumber != null)
        {
            damageNumber.Setup(damage);
        }
    }
}