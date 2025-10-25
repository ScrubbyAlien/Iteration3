using UnityEngine;

public interface IPortal : ITestable
{
    public ModeManager.Modes portalToMode { get; }
    public void OnGoingThroughPortal(IPlayerController controller, ModeManager.Modes mode);
}