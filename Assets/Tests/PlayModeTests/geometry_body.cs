using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[TestFixture]
public class geometry_body : integration_test_fixture
{
    protected override string sceneName => "GeometryBodyTestScene";
    private GeometryBody body = null;

    [UnitySetUp]
    protected override IEnumerator Setup() {
        yield return base.Setup();
        body = Object.FindFirstObjectByType<GeometryBody>();
        yield return null;
    }
    
    // todo write test to make sure gravity is exponential
    [UnityTest]
    public IEnumerator gravity_is_exponential() {
        body.SetParameters(-1);
        float interval = 0.25f;
        
        SimulateUpdatesInDuration(body, interval, 0.01f);
        float expectedDistance1 = TestHelpers.CalculateDistance(0, -1, interval);
        float y1 = body.transform.position.y;
        SimulateUpdatesInDuration(body, interval, 0.01f);
        float expectedDistance2 = TestHelpers.CalculateDistance(0, -1, interval * 2);
        float y2 = body.transform.position.y;
        
        Assert.That(y1, Is.EqualTo(expectedDistance1).Within(0.005f));
        Assert.That(y2, Is.EqualTo(expectedDistance2).Within(0.005f));
        yield return null;
    }
    
    [UnityTest]
    public IEnumerator body_is_obeying_gravity() {
        body.SetParameters(-1);
        
        float y0 = body.transform.position.y;
        SimulateUpdatesInDuration(body, 0.1f, 0.01f);
        float y1 = body.transform.position.y;
        
        Assert.That(y1, Is.LessThan(y0));

        yield return null;
    }

    [UnityTest]
    public IEnumerator body_moves_right() {
        body.SetParameters(-1, xVelocity: 3);
        
        float x0 = body.transform.position.x;
        SimulateUpdatesInDuration(body, 0.1f, 0.01f);
        float x1 = body.transform.position.x;
        
        Assert.That(x1, Is.GreaterThan(x0));
        yield return null;
    }

    [UnityTest]
    public IEnumerator body_collides_with_floor() {
        Vector3 startPos = GetObjectPositionByName("CollideWithFloorPosition");
        body.SetParameters(-1, startPos, 0, -1);
        
        SimulateUpdatesInDuration(body, 0.1f, 0.01f);
        float y0 = body.transform.position.y;
        SimulateUpdatesInDuration(body, 0.1f, 0.01f);
        float y1 = body.transform.position.y; 
        
        Assert.That(y1, Is.EqualTo(y0));
        yield return null;
    }

    [UnityTest]
    public IEnumerator body_collides_with_ceiling() {
        Vector3 startPos = GetObjectPositionByName("CollideWithCeilingPosition");
        body.SetParameters(position: startPos, yVelocity: 10);
        
        SimulateUpdatesInDuration(body, 0.1f, 0.01f);
        float y0 = body.transform.position.y;
        SimulateUpdatesInDuration(body, 0.1f, 0.01f);
        float y1 = body.transform.position.y; 
        
        Assert.That(y1, Is.EqualTo(y0));
        yield return null;
    }
    
    [UnityTest]
    public IEnumerator body_rotates_in_time() {
        float angularVelocity = 100f;
        float duration = 1.3f;

        body.SetParameters(angularVelocity: angularVelocity);
        SimulateUpdatesInDuration(body, duration, 0.01f);
        
        Assert.That(body.rotation, Is.EqualTo(duration * angularVelocity).Within(0.005f));
        yield return null;
    }
   
}