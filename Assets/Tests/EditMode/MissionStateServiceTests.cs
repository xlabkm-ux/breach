using Breach.Mission;
using NUnit.Framework;
using UnityEngine;

public sealed class MissionStateServiceTests
{
    [Test]
    public void Transition_NotStarted_ToInfiltration_IsAllowed()
    {
        var go = new GameObject("MissionStateServiceTests");
        var service = go.AddComponent<MissionStateService>();

        var ok = service.TryTransition(MissionState.Infiltration, out _);

        Assert.That(ok, Is.True);
        Assert.That(service.CurrentState, Is.EqualTo(MissionState.Infiltration));
        Object.DestroyImmediate(go);
    }

    [Test]
    public void Transition_NotStarted_ToExtraction_IsDenied()
    {
        var go = new GameObject("MissionStateServiceTests");
        var service = go.AddComponent<MissionStateService>();

        var ok = service.TryTransition(MissionState.Extraction, out _);

        Assert.That(ok, Is.False);
        Assert.That(service.CurrentState, Is.EqualTo(MissionState.NotStarted));
        Object.DestroyImmediate(go);
    }
}
