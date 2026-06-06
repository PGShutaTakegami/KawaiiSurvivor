using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// ボタン押下でゲームを終了し、マウスホバー時にスケールを変更するスクリプト
/// </summary>
public class FinishGame : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
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
    /// ボタンが押された時にゲームを終了
    /// </summary>
    public void PushButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
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