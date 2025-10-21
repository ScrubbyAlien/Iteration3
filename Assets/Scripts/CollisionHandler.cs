using System;
using UnityEngine;

[RequireComponent(typeof(GeometryBody))]
public class CollisionHandler : MonoBehaviour
{
    public event Action OnDie;
    private GeometryBody body;


    private void Awake() {
        body = GetComponent<GeometryBody>();
        body.OnCollide += OnCollide;
        OnDie += () => Debug.Log("Die");
        
    }

    private void OnCollide(Collider2D other) {
        // todo explode or smth
        gameObject.SetActive(false);
        OnDie?.Invoke();
    }
}