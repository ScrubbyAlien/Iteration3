using UnityEngine;
using UnityEngine.InputSystem;

public interface IPlayerController
{
    public PlayerController controller { get; }
    public ControlPattern activeControlPattern { get; }

    public void Performed(InputAction.CallbackContext context);
    public void Canceled(InputAction.CallbackContext context);

    public void ChangeMode(ModeManager.Modes mode);
    public void SetNewParameters(Sprite sprite, Vector2 colliderSize);
}