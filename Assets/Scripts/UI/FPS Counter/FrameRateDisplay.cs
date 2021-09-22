using UnityEngine;
using TMPro;

public class FrameRateDisplay : MonoBehaviour
{
    public enum DisplayMode { FPS, MS }

    [SerializeField]
    private TextMeshProUGUI display;

    [SerializeField]
    DisplayMode displayMode = DisplayMode.FPS;

    [SerializeField, Range(0.1f, 2.0f)]
    float updateTimedown = 1.0f;

    private int frames;
    private float duration, minDuration = float.MaxValue, maxDuration = 0f;

    private void Update()
    {
        float frameDuration = Time.unscaledDeltaTime;
        frames += 1;
        duration += frameDuration;
        if (frameDuration > maxDuration)
            maxDuration = frameDuration;
        if (frameDuration < minDuration)
            minDuration = frameDuration;

        if (duration >= updateTimedown)
        {
            if(displayMode == DisplayMode.FPS)
                display.SetText("FPS\n{0:1}\n{1:1}\n{2:1}", 
                    1f / minDuration,
                    1f / frameDuration,
                    1f / maxDuration);
            else
                display.SetText("FPS\n{0:1}\n{1:1}\n{2:1}",
                   1000f * minDuration,
                   1000f * frameDuration,
                   1000f * maxDuration);

            frames = 0;
            duration = 0;
            minDuration = float.MaxValue;
            maxDuration = 0f;
        }
    }
}