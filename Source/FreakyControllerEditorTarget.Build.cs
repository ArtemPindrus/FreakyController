﻿using Flax.Build;

public class FreakyControllerEditorTarget : GameProjectEditorTarget
{
    /// <inheritdoc />
    public override void Init()
    {
        base.Init();

        // Reference the modules for editor
        Modules.Add("FreakyController");
        Modules.Add("FreakyControllerEditor");
    }
}
