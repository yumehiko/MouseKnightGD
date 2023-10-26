using System.Threading;
using Fractural.Tasks;
using Godot;
using GTweens;
using GTweensGodot.Contexts;
using GTweens.Easings;
using GTweensGodot.Extensions;

namespace MouseKnightGD.App;

public partial class MusicPlayer : AudioStreamPlayer2D
{
    public void SetMusic(AudioStream music, float volume = 0.0f)
    {
        Stop();
        VolumeDb = volume;
        Stream = music;
        Play();
    }
    
    public async GDTask Fadein(float fadeTime, CancellationToken ct)
    {
        await this.TweenVolumeDb(0.0f, fadeTime)
            .SetEasing(Easing.OutQuad)
            .PlayAsync(ct);
    }
    
    public async GDTask Fadeout(float fadeTime, CancellationToken ct)
    {
        await this.TweenVolumeDb(-80.0f, fadeTime)
            .SetEasing(Easing.OutQuad)
            .PlayAsync(ct);
        Stop();
    }
}