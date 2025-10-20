using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class ControlPattern : ScriptableObject
{
    public abstract ControlPattern Create();
    protected abstract void OnActivated(GeometryBody body, float speed);
    protected abstract void OnDeactivated(GeometryBody body, float speed);
    public abstract void ActionPerformed(InputAction.CallbackContext context, GeometryBody body);
    public abstract void ActionCanceled(InputAction.CallbackContext context, GeometryBody body);

    protected MonoBehaviour mono;
    public void ActivateControl(MonoBehaviour monoBehaviour, GeometryBody body, float speed) {
        mono = monoBehaviour;
        OnActivated(body, speed);
    }
    public void DeactivateControl(MonoBehaviour monoBehaviour, GeometryBody body, float speed) {
        mono = null;
        OnDeactivated(body, speed);
    }
    
    protected Coroutine StartCoroutine(IEnumerator routine) {
        if (!mono)  {
            Debug.LogWarning("Attempting to start coroutine while mono behaviour reference is null");
            return null;
        }
        return mono.StartCoroutine(routine);
    }
    protected void StopCoroutine(Coroutine coroutine) {
        if (!mono)  {
            Debug.LogWarning("Attempting to stop coroutine while mono behaviour reference is null");
            return;
        }
        mono.StopCoroutine(coroutine);
    }

    public virtual void SelectedGizmos(GeometryBody body) { }
}
