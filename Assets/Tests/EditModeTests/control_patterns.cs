using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;

[TestFixture]
public class control_patterns
{
    private const string TestStandardJumpAsset = "TestControlPatterns/TestStandardJumpPattern";
    private const string TestRocketAsset = "TestControlPatterns/TestRocketPattern";

    [Test]
    public void standard_jump_sets_correct_body_parameters() {
        StandardJumpPattern patternAsset = Resources.Load<StandardJumpPattern>(TestStandardJumpAsset);
        StandardJumpPattern pattern = patternAsset.Create() as StandardJumpPattern;
        JumpingParameters parameters = new JumpingParameters(3f, 3f);
        pattern.SetJumpingParameters(parameters);
        IGeometryBody body = Substitute.For<IGeometryBody>();
        IPlayerController controller = Substitute.For<IPlayerController>();
        float speed = 10f;

        pattern.ActivateControl(controller, body, speed);

        body.Received().SetGravity(parameters.CalculateJumpValues().gravity);
        body.Received().SetXVelocity(speed);
    }

    [Test]
    public void rocket_sets_correct_body_parameters() {
        RocketPattern patternAsset = Resources.Load<RocketPattern>(TestRocketAsset);
        RocketPattern pattern = patternAsset.Create() as RocketPattern;
        RocketParameters parameters = new RocketParameters(3f, -3f, 7f);
        pattern.SetParameters(parameters);
        IGeometryBody body = Substitute.For<IGeometryBody>();
        IPlayerController controller = Substitute.For<IPlayerController>();
        float speed = 10f;

        pattern.ActivateControl(controller, body, speed);

        body.Received().SetGravity(parameters.thrust.down);
        body.Received().SetXVelocity(speed);
        body.Received().SetRotation(0f);
        body.Received().SetAngularVelocity(0f);
        body.Received().SetYVelocity(0f);
    }
}