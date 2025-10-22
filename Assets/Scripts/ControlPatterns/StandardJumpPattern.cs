using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "NewStandardJumpPattern", menuName = "Control Patterns/Standard Jump")]
public class StandardJumpPattern : ControlPattern
{
    private float speed;
    
    [SerializeField]
    private JumpingParameters jumpingParameters;
    public JumpingParameters jumpingParams => jumpingParameters;

    private JumpingParameters.JumpValues values;

    [SerializeField]
    private float bufferTime;
    private Coroutine buffer;

    [SerializeField]
    private float landingSmoothingTime;
    private Coroutine landing;

    private bool onGround;
    public void ForceOnGround() => onGround = true;

    public override ControlPattern Create() {
        StandardJumpPattern pattern = ScriptableObject.CreateInstance<StandardJumpPattern>();
        pattern.jumpingParameters = jumpingParameters;
        pattern.bufferTime = bufferTime;
        pattern.landingSmoothingTime = landingSmoothingTime;
        return pattern;
    }
    
    protected override void OnActivated(GeometryBody body, float speed) {
        values = jumpingParameters.CalculateJumpValues();
        body.SetGravity(values.gravity);
        body.SetXVelocity(speed);
        this.speed = speed;
        body.OnTouchGround += OnTouchGround;
        body.OnDrop += OnDrop;
        body.OnJump += RotateOnJump;
    }

    protected override void OnDeactivated(GeometryBody body, float speed) {
        this.speed = 0;
        body.OnTouchGround -= OnTouchGround;
        body.OnDrop -= OnDrop;
    }

    public override void ActionPerformed(InputAction.CallbackContext context, GeometryBody body) {
        Jump(body);
    }
    public override void ActionCanceled(InputAction.CallbackContext context, GeometryBody body) {
        
    }
    
    public void Jump(GeometryBody body) {
        if (!onGround) {
            if (buffer != null) StopCoroutine(buffer);
            buffer = StartCoroutine(BufferJumpInput(bufferTime, body));
            return;
        }
        onGround = false;
        body.Jump(jumpingParameters);
    }

    private void RotateOnJump(GeometryBody body, JumpingParameters parameters) {
        body.SetAngularVelocity(FallingAngularVelocity(parameters));
    }
    
    private float FallingAngularVelocity(JumpingParameters parameters) {
        return -180f / (parameters.timeUp * 2);
    }

    private void OnTouchGround(GeometryBody body) {
        if (!onGround) {
            if (landing != null) StopCoroutine(landing);
            landing = StartCoroutine(Land(landingSmoothingTime, body));
        }
        onGround = true;
    }

    private void OnDrop(GeometryBody body) {
        if (onGround) {
            body.SetAngularVelocity(FallingAngularVelocity(jumpingParameters));
        }
        onGround = false;
    }
    
    private IEnumerator BufferJumpInput(float buffer, GeometryBody body) {
        float bufferEnd = Time.time + buffer;
        while (Time.time < bufferEnd) {
            if (onGround) {
                if (landing != null) StopCoroutine(landing, true);
                Jump(body);
                break;
            }
            yield return null;
        }
    }

    private IEnumerator Land(float smoothingTime, GeometryBody body) {
        body.SetAngularVelocity(0f);
        float originalRotation = body.rotation;
        float nearest90Deg = body.Nearest90Deg();
        float rotDiff = originalRotation - nearest90Deg;
        float startTime = Time.time;
        float stopTime = startTime + (smoothingTime * rotDiff / 90f);
        while (Time.time < stopTime) {
            float t = (Time.time - startTime) / (smoothingTime * rotDiff / 90);
            body.SetRotation(Mathf.Lerp(originalRotation, nearest90Deg, t));
            yield return null;
        }
        body.SetRotation(nearest90Deg);
    }

    private float Next90Deg(GeometryBody body) {
        float rotMod90 = body.rotation % 90f;
        float next90DegRotation = body.rotation - rotMod90;
        return next90DegRotation;
    }

    public void SetJumpingParameters(JumpingParameters newParameters) {
        jumpingParameters = newParameters;
        values = jumpingParameters.CalculateJumpValues();
    }

    public override void SelectedGizmos(GeometryBody body, float speed) {
        values = jumpingParameters.CalculateJumpValues();
        int segments = 20;
        float totalTime = jumpingParameters.timeUp * 2;
        
        Vector2 p = body.transform.position;
        Vector2[] vertices = new Vector2[segments];
        for (int i = 0; i < segments; i++) {
            float fraction = i / (segments - 1f);
            float x = fraction * totalTime * speed;
            float y = TestHelpers.CalculateDistance(values.velocity, values.gravity, fraction * totalTime);
            vertices[i] = p + new Vector2(x, y);
        }

        for (int i = 0; i < segments - 1; i++) {
            Gizmos.DrawLine(vertices[i], vertices[i + 1]);
        }
    }
}
