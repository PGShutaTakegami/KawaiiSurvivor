using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// ボタン押下で指定オブジェクトを非表示にし、マウスホバー時にスケールを変更するスクリプト
/// </summary>
public class ButtonSetActiveFalse : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// 非表示にする対象オブジェクト
    /// </summary>
    public GameObject target;

    /// <summary>
    /// 初期スケール
    /// </summary>
    private Vector3 defaultScale;

    /// <summary>
    /// 初期スケールを保存
    /// </summary>
    private void Awake()
    {
        defaultScale = transform.localScale;
    }

    /// <summary>
    /// ボタンが押された時の処理
    /// </summary>
    public void PushButton()
    {
        target.SetActive(false);
    }

    /// <summary>
    /// マウスを乗せた時の処理
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = defaultScale + new Vector3(0.2f, 0.2f, 0f);
    }

    /// <summary>
    /// マウスを離した時の処理
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = defaultScale;
    }
}