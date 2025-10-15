using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class cooldown
{
    [Test]
    public void cooldown_zero_never_starts() {
        Cooldown cd = new Cooldown();
        cd.Start();
        Assert.That(cd.on, Is.False);
    }

    [Test]
    public void non_zero_cooldown_starts() {
        Cooldown cd = new Cooldown(1f);
        cd.Start();
        Assert.That(cd.on, Is.True);
    }
}