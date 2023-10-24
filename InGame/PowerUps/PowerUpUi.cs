using System.Linq;
using System.Threading.Tasks;
using Fractural.Tasks;
using Godot;

namespace MouseKnightGD.InGame.PowerUps;
public partial class PowerUpUi : Control
{
	[Export] private PowerUp[] _powerUps;
	[Export] private PowerUpUiButton[] _powerUpUiButtons;

	public override void _Ready()
	{
		base._Ready();
		Call().Forget();
	}

	public async GDTask Call()
	{
		GD.Print("PowerUpUi.Call");
		foreach (var button in _powerUpUiButtons)
		{
			var powerUp = GetPowerUp();
			button.Register(powerUp);
		}
		GD.Print("PowerUpUi.Call await");
		await GDTask.WhenAny(_powerUpUiButtons.Select(x => x.PowerUpTcs.Task));
		GD.Print("PowerUpUi.Call end");
	}
	
	private PowerUp GetPowerUp()
	{
		// TODO: 取得できるパワーアップを選び、ランダムに取得する。
		var randomId = GD.RandRange(0, _powerUps.Length - 1);
		GD.Print($"PowerUpUi.GetPowerUp: {randomId}");
		var powerUp = _powerUps[randomId];
		return powerUp;
	}
}
