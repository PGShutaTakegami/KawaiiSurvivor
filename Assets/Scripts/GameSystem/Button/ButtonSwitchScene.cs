using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

/// <summary>
/// ボタン押下で指定シーンへ切り替え、マウスホバー時にスケールを変更するスクリプト
/// </summary>
public class ButtonSwitchScene : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// 切り替え先のシーン名
    /// </summary>
    public string targetScene;

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
    /// ボタンが押された時に指定シーンへ切り替え
    /// </summary>
    public void PushButton()
    {
        SceneManager.LoadScene(targetScene);
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