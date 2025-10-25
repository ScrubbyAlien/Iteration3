using System;
using UnityEngine;

public interface IGeometryBody : ITestable
{
    public event Action<IGeometryBody, Vector2> OnChangeVelocity;
    public event Action<Collider2D> OnCollide;
    public event Action<IGeometryBody> OnTouchGround;
    public event Action<IGeometryBody> OnTouchCeiling;
    public event Action<IGeometryBody> OnDrop;
    public event Action<IGeometryBody, JumpingParameters> OnJump;

    public float g { get; }
    public Vector2 linearVelocity { get; }
    public Vector2 position { get; }
    public float angularVelocity { get; }
    public float rotation { get; }
    
    public void Jump(JumpingParameters parameters);
    public float Nearest90Deg();
    public void SetGravity(float newGravity);
    public void ResetGravity();

    public void SetXVelocity(float xVelocity, bool invokeEvent = true);
    public void SetYVelocity(float yVelocity, bool invokeEvent = true);
    public void SetVelocity(Vector2 newVelocity, bool invokeEvent = true);
    public void SetAngularVelocity(float newAngularVelocity);

    public void SetPosition(Vector3 position);
    public void SetXPosition(float x);
    public void SetYPosition(float y);
    public void SetRotation(float newRotation);
    
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



}