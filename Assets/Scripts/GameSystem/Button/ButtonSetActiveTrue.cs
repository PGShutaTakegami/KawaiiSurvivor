using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSetActiveTrue : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("ActiveTrueにするターゲット")]
    /// <summary>
    /// trueにするターゲットオブジェクト
    /// </summary>
    public GameObject target;

    /// <summary>
    /// 設定されたObjectがアクティブなときTimeScaleは1に戻らない
    /// </summary>
    public GameObject[] noTime;

    /// <summary>
    /// 初期スケール
    /// </summary>
    private Vector3 defaultScale;
    /// <summary>
    /// 有効なとき
    /// </summary>
    private void Awake()
    {
        defaultScale = transform.localScale;
    }
    /// <summary>
    /// ボタンが押されたとき
    /// </summary>
    public void PushButton()
    {
        target.SetActive(true);

        if (CanResumeTime())
        {
            Time.timeScale = 1f;
        }
    }
    /// <summary>
    /// ポーズ状態を再開できるかどうか
    /// </summary>
    /// <returns></returns>
    private bool CanResumeTime()
    {
        foreach (GameObject obj in noTime)
        {
            if (obj != null && obj.activeSelf)
            {
                return false;
            }
        }

        return true;
    }
    /// <summary>
    /// マウスを乗せたときの処理
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = defaultScale + new Vector3(0.2f, 0.2f, 0f);
    }
    /// <summary>
    /// マウスを離したときの処理
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = defaultScale;
    }
}