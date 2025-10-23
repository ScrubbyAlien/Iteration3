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
    
    [SerializeField]
    private ModeManager modeManager;
    public ControlPattern activeControlPattern { get; private set; }

    private void OnValidate() {
        ChangeMode(modeManager.mode);
    }

    private void Awake() {
        body = GetComponent<GeometryBody>();
        InputSystem.actions.FindAction("Player/Action").performed += Performed;
        InputSystem.actions.FindAction("Player/Action").canceled += Canceled;
    }
    
    public void Performed(InputAction.CallbackContext context) => activeControlPattern.ActionPerformed(context, body);
    public void Canceled(InputAction.CallbackContext context) => activeControlPattern.ActionCanceled(context, body);

    public void ChangeMode(ModeManager.Modes mode) {
        if (body == null) body = GetComponent<GeometryBody>();
        modeManager.mode = mode;
        if (activeControlPattern != null) activeControlPattern.DeactivateControl(this, body, speed);
        activeControlPattern = modeManager.GetPattern();
        activeControlPattern?.ActivateControl(this, body, speed);
    }

    public void SetSprite(Sprite sprite) {
        if (!spriteRenderer) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
    }
    
    private void OnDrawGizmosSelected() {
        if (body == null) body = GetComponent<GeometryBody>();
        modeManager.GeneratePatternsDictionary();
        modeManager.GetPatternGizmo().Invoke(body, speed);
    }
}