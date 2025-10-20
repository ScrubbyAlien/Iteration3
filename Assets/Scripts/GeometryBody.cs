using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class GeometryBody : MonoBehaviour, ITestable
{
    public event Action<Collider2D> OnCollide;
    public event Action OnTouchGround;

    [SerializeField, Range(0, 0.3f)]
    private float maxDeltaTime;
    private float deltaTime => Mathf.Min(Time.deltaTime, maxDeltaTime);
    [SerializeField]
    private float gravityConstant;
    private float baseG;
    public float g => gravityConstant;
    private new BoxCollider2D collider;
    private Vector2 velocity;

    [SerializeField]
    private LayerMask floorLayers;
    [SerializeField, Range(-1, 0)]
    private float yGroundCheckOffset;
    
    [SerializeField]
    private GeometryBodyEvents events;
    [Serializable]
    private struct GeometryBodyEvents
    {
        public UnityEvent<Collider2D> OnCollide;
    }

    private bool checkGround;

    private Vector2 lastPosition;
    private Vector2 nextPosition;
    
    private void Awake() {
        collider = GetComponent<BoxCollider2D>();
        baseG = gravityConstant;
        lastPosition = transform.position;
        nextPosition = transform.position;
    }
    
    private void Update() {
        if (Physics2D.simulationMode != SimulationMode2D.Script) return;
        Physics2D.Simulate(deltaTime);

        UpdateWithDelta(deltaTime);
    }
    
    /// <inheritdoc />
    public void UpdateWithDelta(float delta) {
        // Apply gravity
        SetYVelocity(velocity.y + g * delta);
        UpdatePosition(delta);
        
        List<Collider2D> others = new();
        if (collider.Overlap(others) > 0) others.ForEach(other => {
            events.OnCollide?.Invoke(other);
            OnCollide?.Invoke(other);
        });
        
        if (velocity.y < 0) checkGround = false;
    }

    private void UpdatePosition(float delta) {
        Vector3 groundCheckPosition = transform.position + new Vector3(0, yGroundCheckOffset, 0);
        ContactFilter2D filter = new ContactFilter2D() { layerMask = floorLayers};
        List<Collider2D> results = new();
        float nextY = transform.position.y + velocity.y * delta;
        float nextX = transform.position.x + velocity.x * delta;
        if (!checkGround && Physics2D.OverlapBox(groundCheckPosition, collider.bounds.size, 0, filter, results) > 0)  
        {
            Vector2 highestPoint = results.Select(c => c.ClosestPoint(transform.position)) // closest points
                                          .Aggregate(((v1, v2) => v1.y > v2.y ? v1 : v2)); // find highest closest point
            nextY =  highestPoint.y + collider.bounds.extents.y;
            velocity.y = 0;
            OnTouchGround?.Invoke();
        }
        SetPosition(new Vector3(nextX, nextY));
    }

    public void IgnoreCollisionsFor1Frame() {
        checkGround = true;
    }
    
    public void SetGravity(float newGravityConstant) {
        gravityConstant = newGravityConstant;
    }
    public void ResetGravity() => gravityConstant = baseG;
    
    public void SetXVelocity(float xVelocity) {
        velocity = new Vector2(xVelocity, velocity.y);
    }

    public void SetYVelocity(float yVelocity) {
        velocity = new Vector2(velocity.x, yVelocity);
    }

    public void SetVelocity(Vector2 newVelocity) {
        velocity = newVelocity;
    }

    public void SetPosition(Vector3 position) {
        transform.position = position;
    }
    
    public void SetParameters(float newG = 0, Vector3 position = default(Vector3), float newX = 0, float newY = 0) {
        SetGravity(newG);
        SetPosition(position);
        SetXVelocity(newX);
        SetYVelocity(newY);
    }
}