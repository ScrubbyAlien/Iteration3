using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;

[TestFixture]
public class portal
{
    [Test]
    public void set_sprite_gets_called() {
        // not a super useful test, but I want to practice with nsubstitute
        string path = "TestControlPatterns/TestStandardJumpPattern";
        Sprite sprite = Resources.LoadAll<Sprite>("TestSprites/tileset")[1];
        StandardJumpPattern pattern = Resources.Load<StandardJumpPattern>(path);
        pattern.SetJumpingParameters(new JumpingParameters(1, 0.5f));
        IPlayerController controller = Substitute.For<IPlayerController>();
        IGeometryBody body = Substitute.For<IGeometryBody>();
        
        pattern.ActivateControl(controller, body, 10f);
        
        controller.Received().SetSprite(sprite);
    }
}