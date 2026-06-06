using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class ActiveSwitchScene : MonoBehaviour
{
    [Header("切り替え先シーン名")]
    public string targetScene;

    private int activeFrame;
    /// <summary>
    /// Objectが有効なとき
    /// </summary>
    private void OnEnable()
    {
        activeFrame = Time.frameCount;
    }
    /// <summary>
    /// 毎フレームの処理
    /// </summary>
    private void Update()
    {
        if (Time.frameCount <= activeFrame + 1)
        {
            return;
        }

        if (IsAnyInputDown())
        {
            SwitchScene();
        }
    }
    /// <summary>
    /// マウスのボタンorキーボードのキーorゲームパッドのボタンが押された時
    /// </summary>
    /// <returns></returns>
    private bool IsAnyInputDown()
    {
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            return true;
        }

        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                return true;
            }

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                return true;
            }

            if (Mouse.current.middleButton.wasPressedThisFrame)
            {
                return true;
            }
        }

        if (Gamepad.current != null)
        {
            if (Gamepad.current.buttonSouth.wasPressedThisFrame)
            {
                return true;
            }

            if (Gamepad.current.startButton.wasPressedThisFrame)
            {
                return true;
            }
        }

        return false;
    }
    /// <summary>
    /// シーン切り替え処理
    /// </summary>
    private void SwitchScene()
    {
        if (string.IsNullOrEmpty(targetScene))
        {
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(targetScene);
    }
}