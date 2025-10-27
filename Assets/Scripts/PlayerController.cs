using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(GeometryBody))]
public class PlayerController : MonoBehaviour, IPlayerController
{
    public PlayerController controller => this;
    
    [SerializeField]
    private float speed;
    private IGeometryBody body;
    private SpriteRenderer spriteRenderer;
    private new BoxCollider2D collider;
    
    [SerializeField]
    private ModeManager modeManager;
    public ControlPattern activeControlPattern { get; private set; }

    [SerializeField]
    private bool displayJumpX;
    private List<Vector3> positions;
    
    private void OnValidate() {
        ChangeMode(modeManager.mode);
    }

    private void Awake() {
        body = GetComponent<GeometryBody>();
        InputSystem.actions.FindAction("Player/Action").performed += Performed;
        InputSystem.actions.FindAction("Player/Action").canceled += Canceled;
        ChangeMode(modeManager.mode);
        positions = new();
    }

    private void OnDisable() {
        InputSystem.actions.FindAction("Player/Action").performed -= Performed;
        InputSystem.actions.FindAction("Player/Action").canceled -= Canceled;
    }
    
    public void Performed(InputAction.CallbackContext context) {
        positions.Add(transform.position);
        activeControlPattern.ActionPerformed(context, body);
    }
    public void Canceled(InputAction.CallbackContext context) => activeControlPattern.ActionCanceled(context, body);

    public void ChangeMode(ModeManager.Modes mode) {
        if (body == null) body = GetComponent<GeometryBody>();
        modeManager.mode = mode;
        if (activeControlPattern != null) activeControlPattern.DeactivateControl(this, body, speed);
        activeControlPattern = modeManager.GetPattern();
        activeControlPattern?.ActivateControl(this, body, speed);
    }

    public void SetNewParameters(Sprite sprite, Vector2 colliderSize) {
        if (!spriteRenderer) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (!collider) collider = GetComponent<BoxCollider2D>();
        spriteRenderer.sprite = sprite;
        collider.size = colliderSize;
    }

    private void OnDrawGizmosSelected() {
        if (body == null) body = GetComponent<GeometryBody>();
        modeManager.GeneratePatternsDictionary();
        modeManager.GetPatternGizmo().Invoke(body, speed);
    }

    private void OnDrawGizmos() {
        if (positions == null || !displayJumpX) return;
        Gizmos.color = Color.green;
        foreach (Vector3 position in positions) {
            Gizmos.DrawLine(new Vector3(position.x, 10f, 0), new Vector3(position.x, -10f));
        }
    }
}