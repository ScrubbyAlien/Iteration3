using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class ControlPattern : ScriptableObject
{
    [SerializeField]
    private Sprite sprite;
    
    public abstract ModeManager.Modes mode { get; }
    public abstract ControlPattern Create();
    protected abstract void OnActivated(IGeometryBody body, float speed);
    protected abstract void OnDeactivated(IGeometryBody body, float speed);
    public abstract void ActionPerformed(InputAction.CallbackContext context, IGeometryBody geometryBody);
    public abstract void ActionCanceled( InputAction.CallbackContext context, IGeometryBody body);

    protected MonoBehaviour mono;
    public void ActivateControl(PlayerController controller, IGeometryBody body, float speed) {
        mono = controller;
        controller.SetSprite(sprite); 
        OnActivated(body, speed);
    }
    public void DeactivateControl(PlayerController controller, IGeometryBody body, float speed) {
        mono = null;
        OnDeactivated(body, speed);
    }
    
    protected Coroutine StartCoroutine(IEnumerator routine) {
        if (!mono || !mono.gameObject.activeInHierarchy)  {
            Debug.LogWarning("Attempting to start coroutine while mono behaviour reference is null or game object is " +
                             "inactive.");
            return null;
        }
        return mono.StartCoroutine(routine);
    }
    protected void StopCoroutine(Coroutine coroutine, bool nullify = false) {
        if (!mono || !mono.gameObject.activeInHierarchy)  {
            Debug.LogWarning("Attempting to stop coroutine while mono behaviour reference is null or game object is " +
                             "inactive.");
            return;
        }
        mono.StopCoroutine(coroutine);
        if (nullify) coroutine = null;
    }

    public virtual void SelectedGizmos( IGeometryBody body, float speed) { }
}
