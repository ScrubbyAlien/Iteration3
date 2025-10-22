using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class BouncePad : MonoBehaviour, ITestable
{
    public bool enabledForTesting => enabled;
    private new BoxCollider2D collider;

    [SerializeField, Tooltip("Speed value used to calculate gizmos, does not represent actual player speed")]
    private float displaySpeed;
    
    [SerializeField]
    private JumpingParameters parameters;
    public JumpingParameters jumpingParameters => parameters;
    private JumpingParameters.JumpValues values;
    
    //todo extract rotation logic into seperate class, or maybe event somehow?
    
    private void Awake() {
        collider = GetComponent<BoxCollider2D>();
    }

    private void Update() {
        UpdateWithDelta(Time.deltaTime);
    }

    private void OnEnable() {
        Vector3 p = transform.position;
        transform.position = new Vector3(Mathf.Round(p.x), Mathf.Round(p.y), p.z);
    }

    public void UpdateWithDelta(float delta) {
        if (PlayerInCollider(out GeometryBody body)) {
            float diff = transform.position.x - body.transform.position.x;
            JumpingParameters diffAdjustedParameters = new JumpingParameters() {
                height = parameters.height,
                timeUp = parameters.timeUp + 0.5f * diff / body.linearVelocity.x
            };
            body.Jump(diffAdjustedParameters);
            enabled = false;
        }
    }

    private void BouncePlayer(GeometryBody body) {
        body.Jump(parameters);
        enabled = false;
    }

    private bool PlayerInCollider(out GeometryBody body) {
        ContactFilter2D filter = new ContactFilter2D() {
            useLayerMask = true, 
            layerMask = LayerMask.GetMask("Player")
        };
        Collider2D[] playerCollider = new Collider2D[1];
        Vector3 position = transform.position + (Vector3)collider.offset;
        if (Physics2D.OverlapBox(position, collider.size, 0, filter, playerCollider) > 0) {
            body = playerCollider[0].GetComponent<GeometryBody>();
            return true;
        }
        body = null;
        return false;
    }

    private void OnDrawGizmosSelected() {
        collider = GetComponent<BoxCollider2D>();
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