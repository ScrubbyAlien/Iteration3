using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class GeometryBody : MonoBehaviour, ITestable
{
    public event Action<Collider2D> OnCollide;
    public event Action<GeometryBody> OnTouchGround;
    public event Action<GeometryBody> OnDrop;
    public event Action<GeometryBody, JumpingParameters> OnJump;
    
    [SerializeField, Range(0, 0.3f)]
    private float maxDeltaTime;
    private float deltaTime => Mathf.Min(Time.deltaTime, maxDeltaTime);
    [SerializeField]
    private float gravityConstant;
    private float baseG;
    public float g => gravityConstant;
    private new BoxCollider2D collider;
    private SpriteRenderer sprite;
    private Vector2 velocity;
    private float angularVelocity;
    public float rotation => sprite.transform.rotation.eulerAngles.z;
    
    [SerializeField]
    private LayerMask floorLayers, hazardLayers;
    [SerializeField, Range(-1, 0)]
    private float yGroundCheckOffset;
    [SerializeField, Range(0, 1)]
    private float collisionCheckOffset;
    
    [SerializeField]
    private GeometryBodyEvents events;
    [Serializable]
    private struct GeometryBodyEvents
    {
        public UnityEvent<Collider2D> OnCollide;
    }

    public bool falling => velocity.y <= 0;
    private bool checkGround;

    private Vector2 lastPosition;
    private Vector2 nextPosition;
    
    private void Awake() {
        collider = GetComponent<BoxCollider2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        baseG = gravityConstant;
        lastPosition = transform.position;
        nextPosition = transform.position;
    }
    
    private void Update() {
        if (Physics2D.simulationMode != SimulationMode2D.Script) return;
        Physics2D.Simulate(deltaTime);

        UpdateWithDelta(deltaTime);
    }

    public bool enabledForTesting => enabled;
    public void UpdateWithDelta(float delta) {
        SetYVelocity(velocity.y + g * delta); // Apply gravity
        SetRotation(sprite.transform.rotation.eulerAngles.z + angularVelocity * delta); // Apply angularVelocity
        
        UpdatePosition(delta);

        CheckCollisions();

        if (velocity.y <= 0) checkGround = true;
        else checkGround = false;
    }
    

    private void UpdatePosition(float delta) {
        Vector3 groundCheckPosition = transform.position + new Vector3(0, yGroundCheckOffset, 0);
        Vector2 size = collider.bounds.size;
        float nextY = transform.position.y + velocity.y * delta;
        float nextX = transform.position.x + velocity.x * delta;
        if (checkGround && OverlapBox(groundCheckPosition, size, floorLayers, out List<Collider2D> results))  
        {
            Vector2 highestPoint = results.Select(c => c.ClosestPoint(transform.position)) // closest points
                                          .Aggregate(((v1, v2) => v1.y > v2.y ? v1 : v2)); // find highest closest point
            nextY =  highestPoint.y + collider.bounds.extents.y;
            velocity.y = 0;
            OnTouchGround?.Invoke(this);
        }
        // todo check if touches ceiling
        else OnDrop?.Invoke(this);
        SetPosition(new Vector3(nextX, nextY));
    }
    
    private void CheckCollisions() {
        Vector3 forwardCheckPos = transform.position + Vector3.right * collisionCheckOffset;
        Vector3 upwardCheckPos = transform.position + Vector3.up * collisionCheckOffset;
        Vector3 downwardCheckPos = transform.position + Vector3.down * collisionCheckOffset;
        
        LayerMask hazardAndFloor = hazardLayers | floorLayers; // merge hazard and floor layers with bitwise or
        Vector2 checkRightSize = collider.size + new Vector2(0, yGroundCheckOffset * 2); // account for ground check
        
        // make sure colliding with floor object from the side (not from above) triggers OnCollide
        if (OverlapBox(forwardCheckPos, checkRightSize, hazardAndFloor, out List<Collider2D> resultsForward)) {
            resultsForward.ForEach(result => OnCollide?.Invoke(result)); 
        }
        if (OverlapBox(upwardCheckPos, collider.size, hazardLayers, out List<Collider2D> resultsUp)) {
            resultsUp.ForEach(result => OnCollide?.Invoke(result));
        }
        if (OverlapBox(downwardCheckPos, collider.size, hazardLayers, out List<Collider2D> resultsDown)) {
            resultsDown.ForEach(result => OnCollide?.Invoke(result));
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
    
    public void SetGravity(float newGravityConstant) {
        gravityConstant = newGravityConstant;
    }
    public void ResetGravity() => gravityConstant = baseG;

    public void SetXVelocity(float xVelocity) {
        velocity = new Vector2(xVelocity, velocity.y);
    }
    public void SetYVelocity(float yVelocity) {
        if (yVelocity > 0) checkGround = false;
        velocity = new Vector2(velocity.x, yVelocity);
    }
    public void SetVelocity(Vector2 newVelocity) {
        velocity = newVelocity;
    }
    public void SetAngularVelocity(float newAngularVelocity) {
        angularVelocity = newAngularVelocity;
    }

    public void SetPosition(Vector3 position) {
        transform.position = position;
    }
    public void SetRotation(float newRotation) {
        float canonRotation = newRotation % 360;
        sprite.transform.rotation = Quaternion.Euler(new Vector3(0, 0, canonRotation));
    }
    
    public void SetParameters(
        float g = 0, 
        Vector3 position = default(Vector3), 
        float rotation = 0,
        float xVelocity = 0, 
        float yVelocity = 0,
        float angularVelocity = 0) 
    {
        SetGravity(g);
        SetPosition(position);
        SetRotation(rotation);
        SetXVelocity(xVelocity);
        SetYVelocity(yVelocity);
        SetAngularVelocity(angularVelocity);
    }
    
    
    private static bool OverlapBox(Vector3 position, Vector2 size, LayerMask layers, out List<Collider2D> results) {
        ContactFilter2D filter = new ContactFilter2D() { layerMask = layers, useLayerMask = true };
        results = new();
        return Physics2D.OverlapBox(position, size, 0, filter, results) > 0;
    }
}