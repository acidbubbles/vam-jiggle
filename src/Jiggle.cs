using System;

public class Jiggle : MVRScript
{
    public override void Init()
    {
        try
        {
            SuperController.LogMessage($"{nameof(Jiggle)} initialized");
        }
        catch (Exception e)
        {
            SuperController.LogError($"{nameof(Jiggle)}.{nameof(Init)}: {e}");
        }
    }

    public void OnEnable()
    {
        try
        {
            SuperController.LogMessage($"{nameof(Jiggle)} enabled");
        }
        catch (Exception e)
        {
            SuperController.LogError($"{nameof(Jiggle)}.{nameof(OnEnable)}: {e}");
        }
    }

    public void OnDisable()
    {
        try
        {
            SuperController.LogMessage($"{nameof(Jiggle)} disabled");
        }
        catch (Exception e)
        {
            SuperController.LogError($"{nameof(Jiggle)}.{nameof(OnDisable)}: {e}");
        }
    }

    public void OnDestroy()
    {
        try
        {
            SuperController.LogMessage($"{nameof(Jiggle)} destroyed");
        }
        catch (Exception e)
        {
            SuperController.LogError($"{nameof(Jiggle)}.{nameof(OnDestroy)}: {e}");
        }
    }
}
