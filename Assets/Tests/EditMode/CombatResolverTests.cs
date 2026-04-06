using Breach.Combat;
using NUnit.Framework;
using UnityEngine;

public sealed class CombatResolverTests
{
    [Test]
    public void FriendlyFireDisabled_BlocksSameTeamDamage()
    {
        var resolverGo = new GameObject("Resolver");
        var attackerGo = new GameObject("Attacker");
        var targetGo = new GameObject("Target");

        var resolver = resolverGo.AddComponent<CombatResolver>();
        var attacker = attackerGo.AddComponent<HealthComponent>();
        var target = targetGo.AddComponent<HealthComponent>();

        SetTeam(attacker, TeamId.Squad);
        SetTeam(target, TeamId.Squad);
        resolver.SetFriendlyFireEnabled(false);

        var applied = resolver.TryResolveHit(attacker, target, 10);

        Assert.That(applied, Is.False);
        Assert.That(target.CurrentHealth, Is.EqualTo(100));

        Object.DestroyImmediate(resolverGo);
        Object.DestroyImmediate(attackerGo);
        Object.DestroyImmediate(targetGo);
    }

    [Test]
    public void FriendlyFireEnabled_AllowsSameTeamDamage()
    {
        var resolverGo = new GameObject("Resolver");
        var attackerGo = new GameObject("Attacker");
        var targetGo = new GameObject("Target");

        var resolver = resolverGo.AddComponent<CombatResolver>();
        var attacker = attackerGo.AddComponent<HealthComponent>();
        var target = targetGo.AddComponent<HealthComponent>();

        SetTeam(attacker, TeamId.Squad);
        SetTeam(target, TeamId.Squad);
        resolver.SetFriendlyFireEnabled(true);

        var applied = resolver.TryResolveHit(attacker, target, 10);

        Assert.That(applied, Is.True);
        Assert.That(target.CurrentHealth, Is.EqualTo(90));

        Object.DestroyImmediate(resolverGo);
        Object.DestroyImmediate(attackerGo);
        Object.DestroyImmediate(targetGo);
    }

    private static void SetTeam(HealthComponent component, TeamId team)
    {
        var field = typeof(HealthComponent).GetField("team", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(component, team);
    }
}
