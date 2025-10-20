using System.Collections;
using System.Runtime.Serialization;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

[TestFixture]
public class player_controller : integration_test_fixture
{
    protected override string sceneName => "GeometryBodyTestScene";
    private PlayerController controller;
    private GeometryBody body;

    protected override IEnumerator Setup() {
        yield return base.Setup();
        controller = Object.FindFirstObjectByType<PlayerController>();
        body = controller.GetComponent<GeometryBody>();
        yield return null;
    }

    [UnityTest]
    public IEnumerator jumping_reaches_height_in_time() {
        body.SetParameters(); // set default parameters, all zeroes
        JumpingParameters parameters = new JumpingParameters(0.5f, 2f);
        controller.SetJumpingParameters(parameters);

        controller.ForceOnGround();
        controller.Jump();
        SimulateUpdatesInDuration(body, parameters.timeUp, 0.01f);
        
        Assert.That(controller.transform.position.y, Is.EqualTo(parameters.height).Within(0.005f));
        yield return null;
    }

    [UnityTest]
    public IEnumerator jumping_comes_down_in_time() {
        body.SetParameters(); // set default parameters, all zeroes
        JumpingParameters parameters = new JumpingParameters(0.5f, 2f);
        controller.SetJumpingParameters(parameters);
        
        controller.ForceOnGround();
        controller.Jump();
        SimulateUpdatesInDuration(body, parameters.timeUp, 0.01f);
        SimulateUpdatesInDuration(body, parameters.timeUp, 0.01f);
        
        Assert.That(controller.transform.position.y, Is.EqualTo(0f).Within(0.005f));
        yield return null;
    }

    [UnityTest]
    public IEnumerator cannot_jump_in_the_air() {
        body.SetParameters();
        JumpingParameters parameters = new JumpingParameters(0.5f, 2f);
        controller.SetJumpingParameters(parameters);
        
        controller.ForceOnGround();
        controller.Jump();
        SimulateUpdatesInDuration(body, parameters.timeUp, 0.01f);
        controller.Jump();
        SimulateUpdatesInDuration(body, parameters.timeUp, 0.01f);
        
        Assert.That(controller.transform.position.y, Is.EqualTo(0f).Within(0.005f));
        yield return null;
    }
}