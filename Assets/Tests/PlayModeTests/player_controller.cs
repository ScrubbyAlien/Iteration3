using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

[TestFixture]
public class player_controller : integration_test_fixture
{
    protected override string sceneName => "GeometryBodyTestScene";
    private PlayerController controller;

    protected override IEnumerator Setup() {
        yield return base.Setup();
        controller = Object.FindFirstObjectByType<PlayerController>();
        yield return null;
    }

    [UnityTest]
    public IEnumerator jumping_reaches_height_in_time() {
        GeometryBody body = controller.GetComponent<GeometryBody>();
        body.SetParameters(); // set default parameters, all zeroes
        JumpingParameters parameters = new JumpingParameters(6f, 2f, 1f);
        controller.SetJumpingParameters(parameters);

        controller.Jump(default(InputAction.CallbackContext));
        yield return new WaitForSeconds(parameters.timeUp);
        
        Assert.That(controller.transform.position.y, Is.EqualTo(parameters.height).Within(0.1f));
    }

    [UnityTest]
    public IEnumerator jumping_comes_down_in_time() {
        GeometryBody body = controller.GetComponent<GeometryBody>();
        body.SetParameters(); // set default parameters, all zeroes
        JumpingParameters parameters = new JumpingParameters(6f, 2f, 1f);
        controller.SetJumpingParameters(parameters);
        
        controller.Jump(default(InputAction.CallbackContext));
        yield return new WaitForSeconds(parameters.timeUp + parameters.timeDown);
        
        Assert.That(controller.transform.position.y, Is.EqualTo(0f).Within(0.1f));
    }

    [UnityTest]
    public IEnumerator cannot_jump_in_the_air() {
        
        yield return null;
    }
}