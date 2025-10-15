using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class GeometryBody : MonoBehaviour
{
    public float gravityConstant;
    public float g => gravityConstant;
    public float speed;
    private new BoxCollider2D collider;
    private Vector2 velocity;
    
    
    private void Awake() {
        collider = GetComponent<BoxCollider2D>();
        SetSpeed(speed);
    }
    
    private void Update() {
        if (Physics2D.simulationMode != SimulationMode2D.Script) return;
        Physics2D.Simulate(Time.deltaTime);

        UpdateVelocity();
        
        List<Collider2D> others = new();
        if (collider.Overlap(others) > 0) others.ForEach(other => OnOverlap(other));
        
        transform.position += (Vector3)velocity;
    }

    private void UpdateVelocity() {
        velocity = new Vector2(
            speed * Time.deltaTime, 
            velocity.y + g * Time.deltaTime
        );
    }

    private void OnOverlap(Collider2D other) {
        ColliderDistance2D distance = collider.Distance(other);
        Vector3 offset = distance.normal * distance.distance;
        SetPosition(transform.position + offset);
        SetYVelocity(0);
    }

    public void SetGravity(float newGravityConstant) {
        gravityConstant = newGravityConstant;
    }

    public void SetSpeed(float speed) {
        this.speed = speed;
    }

    public void SetPosition(Vector3 position) {
        transform.position = position;
    }

    public void SetYVelocity(float yVelocity) {
        velocity = new Vector2(velocity.x, yVelocity);
    }
}