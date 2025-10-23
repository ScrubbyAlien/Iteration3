using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;
using UnityEngine.U2D;

[TestFixture]
public class mode_manager
{
    [Test]
    public void empty_constructor_default_mode_is_standard() {
        ModeManager manager = new();
        Assert.That(manager.mode, Is.EqualTo(ModeManager.Modes.Standard));
    }

    [Test]
    public void non_empty_constructers_sets_mode_correctly() {
        ModeManager manager = new() { mode = ModeManager.Modes.Ball };
        Assert.That(manager.mode, Is.EqualTo(ModeManager.Modes.Ball));
    }

    [Test]
    public void change_mode_to_rocket() {
        ModeManager manager = new();
        manager.mode = ModeManager.Modes.Rocket;
        Assert.That(manager.mode, Is.EqualTo(ModeManager.Modes.Rocket));
    }

    [Test]
    public void gets_correct_pattern_type() {
        ModeManager manager = new();
        manager.GeneratePatternsDictionary("TestControlPatterns");
        var standardPattern = manager.GetPattern();
        Assert.That(standardPattern, Is.InstanceOf<StandardJumpPattern>());
    }

    [Test]
    public void gets_correct_pattern_type_by_mode() {
        ModeManager manager = new();
        manager.GeneratePatternsDictionary("TestControlPatterns");
        var rocketPattern = manager.GetPattern(ModeManager.Modes.Rocket);
        Assert.That(rocketPattern, Is.InstanceOf<RocketPattern>());
    }

    
}