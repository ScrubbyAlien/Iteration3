using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "NewRocketPattern", menuName = "Control Patterns/Rocket")]
public class RocketPattern : ControlPattern
{
    [SerializeField]
    private RocketParameters parameters;

    private float speed;
    
    public override ModeManager.Modes mode => ModeManager.Modes.Rocket;
    public override ControlPattern Create() {
        RocketPattern pattern = ScriptableObject.CreateInstance<RocketPattern>();
        pattern.parameters = parameters;
        return pattern;
    }
    protected override void OnActivated(IGeometryBody body, float speed) {
        this.speed = speed;
        body.SetXVelocity(speed);
        body.SetYVelocity(0f);
        body.SetGravity(parameters.thrust.down);
        body.SetRotation(0f);
        body.SetAngularVelocity(0f);
        body.OnChangeVelocity += TurnRocket;
        body.OnTouchGround += OnBump;
    }
    protected override void OnDeactivated(IGeometryBody body, float speed) {
        this.speed = 0f; 
        body.OnChangeVelocity -= TurnRocket;
        body.OnTouchGround -= OnBump;
    }
    public override void ActionPerformed(InputAction.CallbackContext context, IGeometryBody body) {
        body.SetGravity(parameters.thrust.up);
    }
    public override void ActionCanceled( InputAction.CallbackContext context, IGeometryBody body) {
        body.SetGravity(parameters.thrust.down);
    }

    private void OnBump(IGeometryBody body) {
        TurnRocket(body, Vector2.right);
    }

    private void TurnRocket(IGeometryBody body, Vector2 newVelocity) {
        float clampedY = Mathf.Clamp(newVelocity.y, -parameters.max, parameters.max);
        body.SetYVelocity(clampedY, false); // don't invoke events to avoid infinite loop
        Vector2 clampedVelocity = new Vector2(newVelocity.x, clampedY);
        float angle = Vector2.SignedAngle(Vector2.right, clampedVelocity.normalized);
        body.SetRotation(angle);
    }

    public void SetParameters(RocketParameters param) {
        parameters = param;
    }
    
    public override void SelectedGizmos(IGeometryBody body, float speed) {
        int segments = 20;
        float time = 0.5f;

        Vector2 p = body.position;
        Vector2[] verticesUp = new Vector2[segments];
        Vector2[] verticesDown = new Vector2[segments];
        for (int i = 0; i < segments; i++) {
            float fraction = i / (segments - 1f);
            float x = fraction * time * speed;
            float yUp = NewtonianMechanics.CalculateDistance(0, parameters.thrust.up, fraction * time);
            float yDown = NewtonianMechanics.CalculateDistance(0, parameters.thrust.down, fraction * time);
            verticesUp[i] = p + new Vector2(x, yUp);
            verticesDown[i] = p + new Vector2(x, yDown);
        }

        for (int i = 0; i < segments - 1; i++) {
            Gizmos.DrawLine(verticesUp[i], verticesUp[i + 1]);
            Gizmos.DrawLine(verticesDown[i], verticesDown[i + 1]);
        }
    }
}

[Serializable]
public struct RocketParameters
{
    [SerializeField]
    private float upwardThurst;
    [SerializeField]
    private float downwardThrust;
    [SerializeField]
    private float maximumYVelocity;

    public (float up, float down) thrust => (upwardThurst, downwardThrust);
    public float max => maximumYVelocity;
    
    public RocketParameters(float up, float down, float max) {
        upwardThurst = up;
        downwardThrust = down;
        maximumYVelocity = max;
    }
}
