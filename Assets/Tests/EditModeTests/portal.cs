using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;

[TestFixture]
public class portal
{
    // todo create scriptable object with portal info and mode swappning functions
    // todo create portal prefab with reference to scriptable object
    // todo test scriptable object swapping functions with help of nsubstitute
    
    
    [Test]
    public void set_new_parameters_gets_called() {
        // not a super useful test, but I want to practice with nsubstitute
        string path = "TestControlPatterns/TestStandardJumpPattern";
        Sprite sprite = Resources.LoadAll<Sprite>("TestSprites/tileset")[1];
        
        StandardJumpPattern pattern = Resources.Load<StandardJumpPattern>(path).Create() as StandardJumpPattern;
        pattern.SetJumpingParameters(new JumpingParameters(1, 0.5f));
        IPlayerController controller = Substitute.For<IPlayerController>();
        IGeometryBody body = Substitute.For<IGeometryBody>();
        
        pattern.ActivateControl(controller, body, 10f);
        
        controller.Received().SetNewParameters(sprite, Vector2.one);
    }


    private const string PortalTestPrefab = "TestPrefabs/TestPortal";
    
    [Test]
    public void portal_swaps_mode_to_rocket() {
        Portal portal = Resources.Load<Portal>(PortalTestPrefab);
        IPlayerController controller = Substitute.For<IPlayerController>();
        
        portal.OnGoingThroughPortal(controller, portal.portalToMode);
        
        controller.Received().ChangeMode(portal.portalToMode);
    }
}