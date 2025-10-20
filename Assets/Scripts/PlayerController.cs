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

    [SerializeField, ExposeFields]
    private ControlPattern controlPatternAsset;
    public ControlPattern activeControlPattern { get; private set; }
    
    private void Awake() {
        body = GetComponent<GeometryBody>();
        InputSystem.actions.FindAction("Player/Action").performed += Performed;
        InputSystem.actions.FindAction("Player/Action").canceled += Canceled;
        
        activeControlPattern = controlPatternAsset.Create();
        activeControlPattern.ActivateControl(this, body, speed);
    }
    
    public void Performed(InputAction.CallbackContext context) => activeControlPattern.ActionPerformed(context, body);
    public void Canceled(InputAction.CallbackContext context) => activeControlPattern.ActionCanceled(context, body);

    public void ChangeControlPattern(ControlPattern newPattern) {
        // activeControlPattern.DeactivateControl(body);
    }
    
    private void OnDrawGizmosSelected() {
        if (!body) body = GetComponent<GeometryBody>();
        controlPatternAsset?.SelectedGizmos(body);
    }
}