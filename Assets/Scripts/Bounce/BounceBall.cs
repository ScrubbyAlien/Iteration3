using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CircleCollider2D))]
public class BounceBall : MonoBehaviour, ITestable
{
    public bool enabledForTesting => enabled;

    private new CircleCollider2D collider;
    
    [SerializeField]
    private JumpingParameters parameters;
    public JumpingParameters jumpingParameters => parameters;
    private JumpingParameters.JumpValues values;

    [SerializeField, Tooltip("Speed value used to calculate gizmos, does not represent actual player speed")]
    private float displaySpeed;

    private bool triggerOnAction;
    
    private void Awake() {
        values = parameters.CalculateJumpValues();
        collider = GetComponent<CircleCollider2D>();
        InputSystem.actions.FindAction("Player/Action").performed += OnAction;
    }

    private void Update() => UpdateWithDelta(Time.deltaTime);
    public void UpdateWithDelta(float delta) {
        if (triggerOnAction) OnAction();
        triggerOnAction = false;
    }

    private void OnAction(InputAction.CallbackContext context) => OnAction();
    public void OnAction() {
        if (!OverlapsPlayer(out GeometryBody playerBody)) return;
        playerBody.Jump(parameters);
        enabled = false;
    }

    public void TriggerOnActionNextFrame() {
        triggerOnAction = true;
    }

    private bool OverlapsPlayer(out GeometryBody playerBody) {
        List<Collider2D> playerCollider = new();
        ContactFilter2D filter = new ContactFilter2D() {
            useLayerMask = true, 
            layerMask = LayerMask.GetMask("Player") 
        };
        
        if (Physics2D.OverlapCircle(transform.position, collider.radius, filter, playerCollider) > 0) {
            playerBody = playerCollider[0].GetComponent<GeometryBody>();
            return true;
        }
        playerBody = null;
        return false;
    }
    
    private void OnDrawGizmosSelected() {
        collider = GetComponent<CircleCollider2D>();
        values = jumpingParameters.CalculateJumpValues();
        int segments = 20;
        float totalTime = jumpingParameters.timeUp * 2;
        
        Vector2 p = transform.position + (Vector3)collider.offset;
        Vector2[] vertices = new Vector2[segments];
        for (int i = 0; i < segments; i++) {
            float fraction = i / (segments - 1f);
            float x = fraction * totalTime * displaySpeed;
            float y = TestHelpers.CalculateDistance(values.velocity, values.gravity, fraction * totalTime);
            vertices[i] = p + new Vector2(x, y);
        }

        for (int i = 0; i < segments - 1; i++) {
            Gizmos.DrawLine(vertices[i], vertices[i + 1]);
        }
    }
}