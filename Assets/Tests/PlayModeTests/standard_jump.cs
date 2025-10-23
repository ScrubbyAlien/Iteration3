using System.Collections;
using System.Runtime.Serialization;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

[TestFixture]
public class standard_jump : integration_test_fixture
{
    protected override string sceneName => "GeometryBodyTestScene";
    private StandardJumpPattern pattern;
    private IGeometryBody body;

    protected override IEnumerator Setup() {
        yield return base.Setup();
        body = Object.FindFirstObjectByType<GeometryBody>();
        pattern = Object.FindFirstObjectByType<PlayerController>().activeControlPattern as StandardJumpPattern;
        yield return null;
    }

    [UnityTest]
    public IEnumerator jumping_reaches_height_in_time() {
        body.SetParameters(); // set default parameters, all zeroes
        JumpingParameters parameters = new JumpingParameters(0.5f, 2f);
        pattern.SetJumpingParameters(parameters);

        pattern.ForceOnGround();
        pattern.Jump(body);
        SimulateUpdatesInDuration(body, parameters.timeUp, 0.01f);
        
        Assert.That(body.position.y, Is.EqualTo(parameters.height).Within(0.005f));
        yield return null;
    }

    [UnityTest]
    public IEnumerator jumping_comes_down_in_time() {
        body.SetParameters(); // set default parameters, all zeroes
        JumpingParameters parameters = new JumpingParameters(0.5f, 2f);
        pattern.SetJumpingParameters(parameters);
        
        pattern.ForceOnGround();
        pattern.Jump(body);
        SimulateUpdatesInDuration(body, parameters.timeUp, 0.01f);
        SimulateUpdatesInDuration(body, parameters.timeUp, 0.01f);
        
        Assert.That(body.position.y, Is.EqualTo(0f).Within(0.005f));
        yield return null;
    }

    [UnityTest]
    public IEnumerator cannot_jump_in_the_air() {
        body.SetParameters();
        JumpingParameters parameters = new JumpingParameters(0.5f, 2f);
        pattern.SetJumpingParameters(parameters);
        
        pattern.ForceOnGround();
        pattern.Jump(body);
        SimulateUpdatesInDuration(body, parameters.timeUp, 0.01f);
        pattern.Jump(body);
        SimulateUpdatesInDuration(body, parameters.timeUp, 0.01f);
        
        Assert.That(body.position.y, Is.EqualTo(0f).Within(0.005f));
        yield return null;
    }
    
    [UnityTest]
    public IEnumerator falls_down_after_bumping_ceiling() {
        Vector3 startPos = GetObjectPositionByName("JumpIntoCeilingPosition");
        body.SetParameters(position: startPos);
        pattern.SetJumpingParameters(new JumpingParameters(2f, 2f));
        
        pattern.ForceOnGround();
        pattern.Jump(body);
        SimulateUpdatesWhile(body, 0.01f, t => t.As<GeometryBody>().linearVelocity.y >= 0);
        float bumpY = body.position.y;
        SimulateUpdatesInDuration(body, 1f, 0.01f);
        float fallY = body.position.y;
        
        Assert.That(fallY, Is.LessThan(bumpY));
        yield return null;
    }
}