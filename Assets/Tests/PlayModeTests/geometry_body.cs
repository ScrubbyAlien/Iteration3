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
        
        yield return new WaitForSeconds(interval);
        float expectedDistance1 = TestHelpers.CalculateDistance(0, -1, interval);
        float y1 = body.transform.position.y;
        yield return new WaitForSeconds(interval);
        float expectedDistance2 = TestHelpers.CalculateDistance(0, -1, interval * 2);
        float y2 = body.transform.position.y;
        
        Assert.That(y1, Is.EqualTo(expectedDistance1).Within(0.02f));
        Assert.That(y2, Is.EqualTo(expectedDistance2).Within(0.02f));
    }
    
    [UnityTest]
    public IEnumerator body_is_obeying_gravity() {
        body.SetParameters(-1);
        
        float y0 = body.transform.position.y;
        yield return TestHelpers.WaitForNumberOfFrames(10);
        float y1 = body.transform.position.y;
        
        Assert.That(y1, Is.LessThan(y0));
    }

    [UnityTest]
    public IEnumerator body_moves_right() {
        body.SetParameters(-1, newX: 3);
        
        float x0 = body.transform.position.x;
        yield return TestHelpers.WaitForNumberOfFrames(10);
        float x1 = body.transform.position.x;
        
        Assert.That(x1, Is.GreaterThan(x0));
    }

    [UnityTest]
    public IEnumerator body_collides_with_floor() {
        Vector3 startPos = TestHelpers.GetObjectPositionByName("CollideWithFloorPosition");
        body.SetParameters(-1, startPos);
        
        float y0 = body.transform.position.y;
        yield return TestHelpers.WaitForNumberOfFrames(10);
        float y1 = body.transform.position.y; 
        
        Assert.That(y1, Is.EqualTo(y0).Within(0.01f));
    }
   
}