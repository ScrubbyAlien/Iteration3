using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
public class Portal : MonoBehaviour, IPortal
{
    [SerializeField]
    private ParticleSystem transitionEffect;
    [SerializeField]
    private ModeManager.Modes mode;
    [SerializeField]
    private LayerMask playerLayers;

    private new CapsuleCollider2D collider;
    

    public ModeManager.Modes portalToMode => mode;
    public bool enabledForTesting => enabled;

    private void Awake() {
        collider = GetComponent<CapsuleCollider2D>();
    }
    private void Update() => UpdateWithDelta(Time.deltaTime);
    public void UpdateWithDelta(float delta) {
        if (OverlapPortal(out IPlayerController playerController)) {
            Instantiate(transitionEffect, transform.position + Vector3.back*2, Quaternion.identity);
            OnGoingThroughPortal(playerController, mode);
            enabled = false;
        }
    }
    
    public void OnGoingThroughPortal(IPlayerController controller, ModeManager.Modes mode) {
        controller.ChangeMode(mode);
    }

    private bool OverlapPortal(out IPlayerController playerCollider) {
        List<Collider2D> results = new();
        if (Physics2D.OverlapCapsule(transform.position, collider.size, collider.direction, 
                                     0, PlayerFilter(), results) > 0
        ) {
            playerCollider = results[0].GetComponent<IPlayerController>();
            return true;
        }
        playerCollider = null;
        return false;
    }
    
    private ContactFilter2D PlayerFilter() => new ContactFilter2D() { useLayerMask = true, layerMask = playerLayers };
}