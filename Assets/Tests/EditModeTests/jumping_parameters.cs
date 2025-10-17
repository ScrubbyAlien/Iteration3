using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class jumping_parameters
{
    [Test]
    public void correct_jump_values() { 
        // test cases picked with help from desmos https://www.desmos.com/calculator/xueqpih8h6
        
        JumpingParameters parameters = new();
        parameters.height = 2f;
        parameters.timeUp = 1f;
        parameters.timeDown = 0.5f;
        JumpingParameters.JumpValues values = parameters.CalculateJumpValues();
        
        Assert.That(values.velocity, Is.EqualTo(4f));
        Assert.That(values.gravityUp, Is.EqualTo(-4f));
        Assert.That(values.gravityDown, Is.EqualTo(-16f));
    }

    [Test]
    public void dont_calculate_if_either_times_are_zero() {
        JumpingParameters.logErrors = false;
        JumpingParameters parameters = new();
        parameters.height = 1f;
        parameters.timeUp = 0;
        parameters.timeDown = 1;
        Assert.That(parameters.CalculateJumpValues, Throws.Nothing);

        parameters.timeUp = 1;
        parameters.timeDown = 0;
        Assert.That(parameters.CalculateJumpValues, Throws.Nothing);

        parameters.timeUp = 0;
        Assert.That(parameters.CalculateJumpValues, Throws.Nothing);
        JumpingParameters.logErrors = true;
    }
}