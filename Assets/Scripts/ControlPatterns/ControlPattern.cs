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

    public virtual void SelectedGizmos(GeometryBody body, float speed) { }
}
