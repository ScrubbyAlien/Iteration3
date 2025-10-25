using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class jumping_parameters
{
    [Test]
    public void correct_jump_values() { 
        // test cases picked with help from desmos https://www.desmos.com/calculator/z2gfbfa7w6
        
        JumpingParameters parameters = new();
        parameters.height = 2f;
        parameters.timeUp = 1f;
        JumpingParameters.JumpValues values = parameters.CalculateJumpValues();
        
        Assert.That(values.velocity, Is.EqualTo(4f));
        Assert.That(values.gravity, Is.EqualTo(-4f));
    }

    [Test]
    public void dont_calculate_if_time_is_zero() {
        JumpingParameters.logErrors = false;
        JumpingParameters parameters = new();
        parameters.height = 1f;
        parameters.timeUp = 0;
        Assert.That(parameters.CalculateJumpValues, Throws.Nothing);
        JumpingParameters.logErrors = true;
    }
}