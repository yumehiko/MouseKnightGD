using Godot;

namespace photon.App;

public partial class LoadableSession : Resource
{
    [Export] private PackedScene _sessionPack;
    [Export] private AudioStream _initMusic;
    
    public PackedScene SessionPack => _sessionPack;
    public AudioStream InitMusic => _initMusic;
    
    public T Load<T>(MusicPlayer musicPlayer) where T : Node
    {
        if(_initMusic != null) musicPlayer.SetMusic(_initMusic);
        return _sessionPack.Instantiate<T>();
    }
}