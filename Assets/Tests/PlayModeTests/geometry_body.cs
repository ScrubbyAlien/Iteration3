using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[TestFixture]
public class geometry_body
{
    private string sceneName = "GeometryBodyTestScene";
    private Scene scene = new Scene();
    private GeometryBody body = null;

    [UnitySetUp]
    private IEnumerator Setup() {
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        scene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(scene);
        body = Object.FindFirstObjectByType<GeometryBody>();
        yield return null;
    }

    [UnityTearDown]
    private IEnumerator TearDown() {
        yield return SceneManager.UnloadSceneAsync(scene);
        yield return null;
    }
    
    [UnityTest]
    public IEnumerator body_is_obeying_gravity() {
        body.SetPosition(Vector3.zero);
        body.SetGravity(-1);
        float y0 = body.transform.position.y;
        yield return TestHelpers.WaitForNumberOfFrames(10);
        float y1 = body.transform.position.y;
        Assert.That(y1, Is.LessThan(y0));
    }

    [UnityTest]
    public IEnumerator body_moves_right() {
        body.SetPosition(Vector3.zero);
        body.SetSpeed(3);
        float x0 = body.transform.position.x;
        yield return TestHelpers.WaitForNumberOfFrames(10);
        float x1 = body.transform.position.x;
        Assert.That(x1, Is.GreaterThan(x0));
    }

    [UnityTest]
    public IEnumerator body_collides_with_floor() {
        body.SetGravity(-1);
        body.SetSpeed(0);
        Vector3 startPos = Object.FindObjectsByType<Transform>(FindObjectsSortMode.None)
                                 .First(g => g.gameObject.name == "CollideWithFloorPosition")
                                 .transform.position;
        body.SetPosition(startPos);
        float y0 = body.transform.position.y;
        yield return TestHelpers.WaitForNumberOfFrames(10);
        float y1 = body.transform.position.y; 
        Assert.That(y1, Is.EqualTo(y0).Within(0.01f));
    }
}