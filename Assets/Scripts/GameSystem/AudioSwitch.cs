using UnityEngine;

/// <summary>
/// GameObjectの状態に応じてBGMを切り替えるスクリプト
/// </summary>
public class AudioSwitch : MonoBehaviour
{
    /// <summary>
    /// ゲームクリアUI
    /// </summary>
    public GameObject gameComplete;

    /// <summary>
    /// ゲームオーバーUI
    /// </summary>
    public GameObject gameOver;

    /// <summary>
    /// 通常時のBGM
    /// </summary>
    public AudioClip defaultBGM;

    /// <summary>
    /// ゲームクリア時のBGM
    /// </summary>
    public AudioClip gameCompleteBGM;

    /// <summary>
    /// ゲームオーバー時のBGM
    /// </summary>
    public AudioClip gameOverBGM;

    [Header("音量")]
    /// <summary>
    /// BGM音量
    /// </summary>
    public float volume = 1f;

    /// <summary>
    /// AudioSource
    /// </summary>
    private AudioSource audioSource;

    /// <summary>
    /// 現在のBGM状態
    /// </summary>
    private AudioState currentState;

    /// <summary>
    /// BGM状態
    /// </summary>
    private enum AudioState
    {
        Default,
        GameComplete,
        GameOver
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    /// <summary>
    /// 開始時に通常BGMを再生
    /// </summary>
    private void Start()
    {
        currentState = AudioState.Default;
        PlayBGM(defaultBGM);
    }

    /// <summary>
    /// UIの表示状態に応じてBGMを切り替え
    /// </summary>
    private void Update()
    {
        audioSource.volume = volume;

        if (gameOver != null && gameOver.activeSelf)
        {
            ChangeState(AudioState.GameOver, gameOverBGM);
            return;
        }

        if (gameComplete != null && gameComplete.activeSelf)
        {
            ChangeState(AudioState.GameComplete, gameCompleteBGM);
            return;
        }

        ChangeState(AudioState.Default, defaultBGM);
    }

    /// <summary>
    /// BGM状態を変更
    /// </summary>
    /// <param name="nextState"></param>
    /// <param name="clip"></param>
    private void ChangeState(AudioState nextState, AudioClip clip)
    {
        if (currentState == nextState)
        {
            return;
        }

        currentState = nextState;
        PlayBGM(clip);
    }

    /// <summary>
    /// 指定したBGMをLoop再生
    /// </summary>
    /// <param name="clip"></param>
    private void PlayBGM(AudioClip clip)
    {
        if (clip == null)
        {
            audioSource.Stop();
            audioSource.clip = null;
            return;
        }

        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.volume = volume;
        audioSource.Play();
    }
}