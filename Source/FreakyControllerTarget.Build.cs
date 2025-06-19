using Flax.Build;

public class FreakyControllerTarget : GameProjectTarget
{
    /// <inheritdoc />
    public override void Init()
    {
        base.Init();

        // Reference the modules for game
        Modules.Add("FreakyController");
    }
}
