using UnityEngine;

// ポーズ状態でもUIアニメーションを再生するためのスクリプト
public class UnscaledUIAnimation : MonoBehaviour
{
    /// <summary>
    /// アタッチされたオブジェクトのAnimation
    /// </summary>
    private Animation[] animations;

    /// <summary>
    /// アニメーションの取得
    /// </summary>
    private void Awake()
    {
        animations = GetComponentsInChildren<Animation>(true);
    }

    /// <summary>
    /// アニメーションが再生されないとき再生
    /// </summary>
    private void OnEnable()
    {
        animations = GetComponentsInChildren<Animation>(true);

        foreach (Animation animation in animations)
        {
            if (animation == null) continue;

            foreach (AnimationState state in animation)
            {
                state.speed = 0f;
            }

            if (!animation.isPlaying)
            {
                animation.Play();
            }
        }
    }

    /// <summary>
    /// 毎フレームの処理
    /// </summary>
    private void Update()
    {
        if (animations == null)
        {
            return;
        }

        foreach (Animation animation in animations)
        {
            if (animation == null) continue;

            foreach (AnimationState state in animation)
            {
                if (!animation.IsPlaying(state.name)) continue;

                state.time += Time.unscaledDeltaTime;

                if (state.time >= state.length)
                {
                    if (state.wrapMode == WrapMode.Loop)
                    {
                        state.time %= state.length;
                    }
                    else
                    {
                        state.time = state.length;
                    }
                }

                animation.Sample();
            }
        }
    }
}