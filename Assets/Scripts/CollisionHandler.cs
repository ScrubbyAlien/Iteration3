using System;
using UnityEngine;

[RequireComponent(typeof(GeometryBody))]
public class CollisionHandler : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem explosion;
    
    public event Action OnDie;
    private IGeometryBody body;


    private void Awake() {
        body = GetComponent<GeometryBody>();
        body.OnCollide += OnCollide;
    }

    private void OnCollide(Collider2D other) {
        gameObject.SetActive(false);
        Instantiate(explosion, transform.position, Quaternion.identity);
        OnDie?.Invoke();
    }
}