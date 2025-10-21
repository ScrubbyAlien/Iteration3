using System.Collections;
using JetBrains.Annotations;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class collision_handler : integration_test_fixture
{
    protected override string sceneName => "CollisionHandlingTestScene";
    private CollisionHandler collisionHandler;
    private GeometryBody body;
    
    protected override IEnumerator Setup() {
        yield return base.Setup();
        collisionHandler = Object.FindFirstObjectByType<CollisionHandler>();
        body = collisionHandler.GetComponent<GeometryBody>();
    }

    [UnityTest]
    public IEnumerator colliding_with_walls_kills_player() {
        Vector3 startPos = GetObjectPositionByName("WallCollisionTestPosition");
        body.SetParameters(position: startPos, g: -20f, xVelocity: 5f);

        bool died = false;
        // make sure body is colliding with wall
        collisionHandler.OnDie += () => died = true;
        SimulateUpdatesInDuration(body, 0.5f, 0.01f);
        
        Assert.That(died, Is.True);
        yield return null;
    }

    [UnityTest]
    public IEnumerator colliding_with_spikes_kills_player() {
        Vector3 startPos = GetObjectPositionByName("SpikeCollisionTestPosition");
        body.SetParameters(position: startPos, g: -20f, xVelocity: 5f);


        bool died = false;
        // make sure body is colliding with wall
        collisionHandler.OnDie += () => died = true;
        SimulateUpdatesInDuration(body, 0.5f, 0.01f);
        
        Assert.That(died, Is.True);
        yield return null;
    }

    [UnityTest]
    public IEnumerator colliding_with_floor_from_above_does_not_kill_player() {
        Vector3 startPos = GetObjectPositionByName("GroundCollisionTestPosition");
        body.SetParameters(position: startPos, g: -20f);

        bool died = false;
        bool touchingGround = false;
        // make sure body is colliding with wall
        body.OnTouchGround += (_) => touchingGround = true;
        collisionHandler.OnDie += () => died = true;
        SimulateUpdatesInDuration(body, 0.5f, 0.01f);
        
        Assert.That(touchingGround, Is.True);
        Assert.That(died, Is.False);
        yield return null;
    }
}