using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class GeometryBody : MonoBehaviour, IGeometryBody
{
    public event Action<IGeometryBody, Vector2> OnChangeVelocity;
    public event Action<Collider2D> OnCollide;
    public event Action<IGeometryBody> OnTouchGround;
    public event Action<IGeometryBody> OnTouchCeiling;
    public event Action<IGeometryBody> OnDrop;
    public event Action<IGeometryBody, JumpingParameters> OnJump;
    
    [SerializeField, Range(0, 0.3f)]
    private float maxDeltaTime;
    private float deltaTime => Mathf.Min(Time.deltaTime, maxDeltaTime);
    [SerializeField]
    private float initialGravity;
    private float gravity;
    public float g => gravity;
    private bool gravitySet;
    
    private new BoxCollider2D collider;
    private SpriteRenderer sprite;
    
    public Vector2 linearVelocity { get; private set; }
    public Vector2 position => transform.position;
    public float angularVelocity { get; private set; }
    public float rotation {
        get {
            if (sprite) return sprite.transform.rotation.eulerAngles.z;
            else return 0;
        }
    }

    [SerializeField]
    private LayerMask floorLayers, hazardLayers;
    [SerializeField, Range(-1, 0)]
    private float yGroundCheckOffset;
    [SerializeField, Range(0, 1)]
    private float collisionCheckOffset;
    [SerializeField]
    private float collisionAngle;
    
    [SerializeField]
    private GeometryBodyEvents events;
    [Serializable]
    private struct GeometryBodyEvents
    {
        public UnityEvent<Collider2D> OnCollide;
    }

    private bool checkGround => linearVelocity.y <= 0;
    private bool checkCeiling => linearVelocity.y >= 0;

    private Vector2 lastPosition;
    private Vector2 nextPosition;
    
    private void Awake() {
        collider = GetComponent<BoxCollider2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        lastPosition = transform.position;
        nextPosition = transform.position;
        if (!gravitySet) gravity = initialGravity;
    }
    
    private void Update() {
        if (Physics2D.simulationMode != SimulationMode2D.Script) return;
        Physics2D.Simulate(deltaTime);

        UpdateWithDelta(deltaTime);
    }

    public bool enabledForTesting => enabled;
    public void UpdateWithDelta(float delta) {
        SetYVelocity(linearVelocity.y + gravity * delta); // Apply gravity
        SetRotation(sprite.transform.rotation.eulerAngles.z + angularVelocity * delta); // Apply angularVelocity
        
        UpdatePosition(delta);

        CheckCollisions();
    }
    
    private void UpdatePosition(float delta) {
        Vector3 groundCheckPosition = transform.position + new Vector3(0, yGroundCheckOffset, 0);
        Vector3 ceilingCheckPosition = transform.position - new Vector3(0, yGroundCheckOffset, 0);
        Vector2 size = collider.bounds.size;
        float nextY = transform.position.y + linearVelocity.y * delta;
        float nextX = transform.position.x + linearVelocity.x * delta;
        if (checkGround && OverlapBox(groundCheckPosition, size, floorLayers, out List<Collider2D> groundResults))  
        {
            Vector2 highestPoint = groundResults
                                   .Select(c => c.ClosestPoint(transform.position)) // closest points
                                   .Aggregate(((v1, v2) => v1.y > v2.y ? v1 : v2)); // find highest closest point
            nextY =  highestPoint.y + collider.bounds.extents.y;
            SetYVelocity(0);
            OnTouchGround?.Invoke(this);
        }
        // check if touhces ceiling, hurt top of head
        else if (checkCeiling && OverlapBox(ceilingCheckPosition, size, floorLayers, out List<Collider2D> ceilingResults)) {
            Vector2 lowestPoint = ceilingResults
                                  .Select(c => c.ClosestPoint(transform.position)) // closest points
                                  .Aggregate(((v1, v2) => v1.y < v2.y ? v1 : v2)); // find lowest closest point
            nextY =  lowestPoint.y - collider.bounds.extents.y;
            SetYVelocity(0);
            OnTouchCeiling?.Invoke(this);
        }
        else OnDrop?.Invoke(this);
        SetPosition(new Vector3(nextX, nextY));
    }

    private void CheckCollisions() {
        Vector2 hazardCheckSize = new Vector2(collisionCheckOffset, collisionCheckOffset);
        Vector3 sideWallCheckPos = transform.position + Vector3.right * collisionCheckOffset;

        // Check hazards
        if (OverlapBox(transform.position, hazardCheckSize, hazardLayers, out List<Collider2D> resultsForward)) {
            resultsForward.ForEach(result => OnCollide?.Invoke(result));
        }

        // Check if colliding with wall from side
        if (OverlapBox(sideWallCheckPos, collider.size, floorLayers, out List<Collider2D> results)) {
            results.ForEach(result => {
                Vector3 closest = result.ClosestPoint(transform.position);
                Vector2 toClosest = ((Vector2)(closest - transform.position)).normalized;
                if (Vector2.Angle(Vector2.right, toClosest) < collisionAngle) { // check if the collision happened to the right of the player
                    OnCollide?.Invoke(result);
                }
                // else if (Vector2.SignedAngle(Vector2.right, toClosest) > collisionAngle) {
                //     OnTouchGround?.Invoke(this);
                // }
                // else if ()
            });
        }

    }

    public void Jump(JumpingParameters parameters) {
        JumpingParameters.JumpValues values = parameters.CalculateJumpValues();
        SetRotation(Nearest90Deg());
        SetYVelocity(values.velocity);
        SetGravity(values.gravity);
        OnJump?.Invoke(this, parameters);
    }
    
    public float Nearest90Deg() {
        float rotMod90 = rotation % 90f;
        float nearest = rotMod90 < 45f ? rotation - rotMod90 : rotation - rotMod90 + 90f;
        return nearest;
    }
    
    public void SetGravity(float newGravity) {
        gravitySet = true;
        gravity = newGravity;
    }
    public void ResetGravity() => gravity = initialGravity;

    public void SetXVelocity(float xVelocity, bool invokeEvent = true) {
        SetVelocity(new Vector2(xVelocity, linearVelocity.y), invokeEvent);
    }
    public void SetYVelocity(float yVelocity, bool invokeEvent = true) {
        SetVelocity(new Vector2(linearVelocity.x, yVelocity), invokeEvent);
    }
    public void SetVelocity(Vector2 newVelocity, bool invokeEvent = true) {
        linearVelocity = newVelocity;
        if (invokeEvent) OnChangeVelocity?.Invoke(this, linearVelocity);
    }
    public void SetAngularVelocity(float newAngularVelocity) {
        angularVelocity = newAngularVelocity;
    }

    public void SetPosition(Vector3 position) {
        transform.position = position;
    }
    public void SetXPosition(float x) {
        SetPosition(new Vector3(x, transform.position.y));
    }
    public void SetYPosition(float y) {
        SetPosition(new Vector3(transform.position.x, y));
    }
    public void SetRotation(float newRotation) {
        float canonRotation = newRotation % 360;
        if (!sprite) sprite = GetComponentInChildren<SpriteRenderer>();
        sprite.transform.rotation = Quaternion.Euler(new Vector3(0, 0, canonRotation));
    }
    
    
    private static bool OverlapBox(Vector3 position, Vector2 size, LayerMask layers, out List<Collider2D> results) {
        ContactFilter2D filter = new ContactFilter2D() { layerMask = layers, useLayerMask = true };
        results = new();
        return Physics2D.OverlapBox(position, size, 0, filter, results) > 0;
    }
}