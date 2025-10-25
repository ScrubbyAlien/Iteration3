using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

[TestFixture]
public class rocket : integration_test_fixture
{
    private const string PatternPath = "TestControlPatterns/TestRocketPattern";
    
    protected override string sceneName => "RocketPatternTestScene";
    private IGeometryBody body;
    private RocketPattern pattern;
    
    protected override IEnumerator Setup() {
        yield return base.Setup();
        body = Object.FindFirstObjectByType<GeometryBody>();
        pattern = Object.FindFirstObjectByType<PlayerController>().activeControlPattern as RocketPattern;
        yield return null;
    }

    [UnityTest]
    public IEnumerator max_y_velocity_is_not_exceeded() {
        body.SetParameters();
        RocketParameters parameters = new RocketParameters(50f, -50f, 10f);
        pattern.SetParameters(parameters);
        
        pattern.ActionPerformed(default(InputAction.CallbackContext), body);
        SimulateUpdatesInDuration(body, 3f, 0.01f);
        pattern.ActionCanceled(default(InputAction.CallbackContext), body);
        
        Assert.That(body.linearVelocity.y, Is.InRange(-parameters.max, parameters.max));
        yield return null;
    }

    [UnityTest]
    public IEnumerator correct_y_is_reached_during_rise() {
        body.SetParameters();
        RocketParameters parameters = new RocketParameters(50f, -50f, 1000f);
        pattern.SetParameters(parameters);
        float duration = 2f;
        
        pattern.ActionPerformed(default(InputAction.CallbackContext), body);
        SimulateUpdatesInDuration(body, duration, 0.01f);
        pattern.ActionCanceled(default(InputAction.CallbackContext), body);
        float expected = NewtonianMechanics.CalculateDistance(0f, parameters.thrust.up, duration) + 0.25f * duration;
        
        Assert.That(body.position.y, Is.EqualTo(expected));
        yield return null;
    }
}