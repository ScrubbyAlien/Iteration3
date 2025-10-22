using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

[TestFixture]
public class bouncing_tests : integration_test_fixture
{
    protected override string sceneName => "BouncingTestScene";
    private GeometryBody body;

    protected override IEnumerator Setup() {
        yield return base.Setup();
        body = Object.FindFirstObjectByType<GeometryBody>();
    }

    [UnityTest]
    public IEnumerator bounce_pad_bounces_to_height_in_time() {
        Vector3 startPos = GetObjectPositionByName("BouncePadTestPosition");
        body.SetParameters(position: startPos, g: -10, xVelocity: 10);

        BouncePad bouncePad = Object.FindFirstObjectByType<BouncePad>();
        ITestable[] testables = new ITestable[] { bouncePad, body };
        Predicate<ITestable[]> notBounced = t => t[0].As<BouncePad>().enabledForTesting;
        SimulateUpdatesWhile(testables, 0.01f, notBounced, true);
        SimulateUpdatesInDuration(testables, bouncePad.jumpingParameters.timeUp, 0.01f);
        float endY = body.transform.position.y;
        
        Assert.That(endY - startPos.y, Is.EqualTo(bouncePad.jumpingParameters.height).Within(0.05f));
        yield return null;
    }

    
}