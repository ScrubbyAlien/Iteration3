using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(GeometryBody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed;
    private GeometryBody body;
    
    [SerializeField]
    private JumpingParameters jumpingParameters;
    public JumpingParameters jumpingParams => jumpingParameters;

    private JumpingParameters.JumpValues values;

    [SerializeField]
    private float bufferTime;
    private Coroutine buffer;
    
    private bool onGround;
    public void ForceOnGround() => onGround = true;
    
    private void Awake() {
        body = GetComponent<GeometryBody>();
        InputSystem.actions.FindAction("Player/Jump").performed += Jump;
    }

    private void Start() {
        values = jumpingParameters.CalculateJumpValues();
        body.SetGravity(values.gravityDown);
        body.SetXVelocity(speed);
        body.OnTouchGround += () => onGround = true;
    }

    public void Jump(InputAction.CallbackContext context) => Jump();
    public void Jump() {
        if (!onGround) {
            if (buffer != null) StopCoroutine(buffer);
            buffer = StartCoroutine(BufferJumpInput(bufferTime));
            return;
        }
        body.IgnoreCollisionsFor1Frame();
        onGround = false;
        body.SetYVelocity(values.velocity);
        body.SetGravity(values.gravityUp);
        StartCoroutine(SetDownGravity(jumpingParameters.timeUp));
    }

    private IEnumerator BufferJumpInput(float buffer) {
        float bufferEnd = Time.time + buffer;
        while (Time.time < bufferEnd) {
            if (onGround) {
                Jump();
                break;
            }
            yield return null;
        }
    }
    
    private IEnumerator SetDownGravity(float delay) {
        yield return new WaitForSeconds(delay);
        body.SetGravity(values.gravityDown);
    }
    
    public void SetJumpingParameters(JumpingParameters newParameters) {
        jumpingParameters = newParameters;
        values = jumpingParameters.CalculateJumpValues();
    }
    
    private void OnDrawGizmosSelected() {
        values = jumpingParameters.CalculateJumpValues();
        int segments = 20;
        float totalTime = jumpingParameters.timeUp + jumpingParameters.timeDown;
        float up = jumpingParameters.timeUp;
        float down = jumpingParameters.timeDown;
        
        Vector2 p = transform.position;
        Vector2[] vertices = new Vector2[segments];
        for (int i = 0; i < segments; i++) {
            float fraction = i / (segments - 1f);
            float x = fraction * totalTime * speed;
            float y = TestHelpers.CalculateDistance(values.velocity, values.gravityUp, fraction * totalTime);
            if (x > up * speed) {
                y = jumpingParameters.height + TestHelpers.CalculateDistance(0, values.gravityDown, fraction * totalTime - up);
            }
            vertices[i] = p + new Vector2(x, y);
        }

        for (int i = 0; i < segments - 1; i++) {
            
            Gizmos.DrawLine(vertices[i], vertices[i + 1]);
        }
    }
}