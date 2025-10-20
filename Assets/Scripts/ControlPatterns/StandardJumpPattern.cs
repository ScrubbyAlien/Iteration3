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
    }

    protected override void OnDeactivated(GeometryBody body, float speed) {
        this.speed = 0;
        body.OnTouchGround -= OnTouchGround;
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
        body.IgnoreCollisionsFor1Frame();
        onGround = false;
        float angularVelocity = -180f / (jumpingParameters.timeUp * 2);
        body.SetAngularVelocity(angularVelocity);
        body.SetYVelocity(values.velocity);
        body.SetGravity(values.gravity);
    }

    private void OnTouchGround(GeometryBody body) {
        onGround = true;
        if (landing != null) StopCoroutine(landing);
        landing = StartCoroutine(Land(landingSmoothingTime, body));
    }
    
    private IEnumerator BufferJumpInput(float buffer, GeometryBody body) {
        float bufferEnd = Time.time + buffer;
        while (Time.time < bufferEnd) {
            if (onGround) {
                Jump(body);
                break;
            }
            yield return null;
        }
    }

    private IEnumerator Land(float smoothingTime, GeometryBody body) {
        if (Mathf.Abs(body.transform.rotation.eulerAngles.z) < 0.01f) {
            yield break;
        }
        body.SetAngularVelocity(0f);
        float rot = body.transform.rotation.eulerAngles.z;
        float rotMod90 = rot % 90f;
        float next90DegRotation = rot - rotMod90 + 90f;
        float startTime = Time.time;
        float stopTime = startTime + smoothingTime;
        while (Time.time < stopTime) {
            float t = Mathf.Sqrt((Time.time - startTime) / smoothingTime);
            body.SetRotation(Mathf.Lerp(rot, next90DegRotation, t));
            yield return null;
        }
        body.SetRotation(next90DegRotation);
    }
    
    public void SetJumpingParameters(JumpingParameters newParameters) {
        jumpingParameters = newParameters;
        values = jumpingParameters.CalculateJumpValues();
    }

    public override void SelectedGizmos(GeometryBody body) {
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
