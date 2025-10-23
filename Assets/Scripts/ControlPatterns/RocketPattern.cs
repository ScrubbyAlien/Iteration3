using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "NewRocketPattern", menuName = "Control Patterns/Rocket")]
public class RocketPattern : ControlPattern
{
    public override ModeManager.Modes mode => ModeManager.Modes.Rocket;
    public override ControlPattern Create() {
        return ScriptableObject.CreateInstance<RocketPattern>();
    }
    protected override void OnActivated(IGeometryBody body, float speed) {
        // throw new System.NotImplementedException();
    }
    protected override void OnDeactivated(IGeometryBody body, float speed) {
        // throw new System.NotImplementedException();
    }
    public override void ActionPerformed(InputAction.CallbackContext context, IGeometryBody body) {
        throw new System.NotImplementedException();
    }
    public override void ActionCanceled( InputAction.CallbackContext context, IGeometryBody body) {
        throw new System.NotImplementedException();
    }
}
