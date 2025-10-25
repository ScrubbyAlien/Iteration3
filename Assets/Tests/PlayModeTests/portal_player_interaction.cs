using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class portal_player_interaction : integration_test_fixture
{
    protected override string sceneName => "PortalTestScene";
    private IGeometryBody body;
    private IPortal portal;
    
    protected override IEnumerator Setup() {
        yield return base.Setup();
        body = Object.FindFirstObjectByType<GeometryBody>();
        portal = Object.FindFirstObjectByType<Portal>();
        yield return null;
    }

    [UnityTest]
    public IEnumerator portal_change_player_to_rocket_pattern() {
        Vector3 startPos = GetObjectPositionByName("PortalTestStartPosition");
        body.SetParameters(position: startPos, xVelocity: 5);
        ITestable[] testables = { body, portal };

        // make sure the starting mode is standard jump
        IPlayerController controller = Object.FindFirstObjectByType<PlayerController>();
        Assert.That(controller.activeControlPattern, Is.InstanceOf<StandardJumpPattern>());
        
        SimulateUpdatesInDuration(testables, 1f, 0.01f, true);
        
        Assert.That(controller.activeControlPattern, Is.InstanceOf<RocketPattern>());
        yield return null;
    }
}