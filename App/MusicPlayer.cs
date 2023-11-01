using System.Threading;
using Fractural.Tasks;
using Godot;
using Godot.Extensions;

namespace photon.App;

public partial class MusicPlayer : AudioStreamPlayer2D
{
    private Tween _volumeTween;
    
    public void SetMusic(AudioStream music, float volume = 0.0f)
    {
        Stop();
        VolumeDb = volume;
        Stream = music;
        Play();
    }
    
    public async GDTask Fadein(float fadeTime, CancellationToken ct)
    {
        _volumeTween?.Kill();
        _volumeTween = CreateTween();
        _volumeTween.TweenProperty(this, "volume_db", 0.0f, fadeTime)
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.Out);
        await _volumeTween.PlayAsync(ct);
    }
    
    public async GDTask Fadeout(float fadeTime, CancellationToken ct)
    {
        _volumeTween?.Kill();
        _volumeTween = CreateTween();
        _volumeTween.TweenProperty(this, "volume_db", -80.0f, fadeTime)
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.Out);
        await _volumeTween.PlayAsync(ct);
        Stop();
    }
}