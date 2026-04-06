using Breach.Mission;
using NUnit.Framework;
using UnityEngine;

public sealed class ObjectiveServiceTests
{
    [Test]
    public void SuccessCandidate_RequiresAllObjectiveFlags()
    {
        var go = new GameObject("ObjectiveServiceTests");
        var objective = go.AddComponent<ObjectiveService>();

        objective.MarkInfiltrationComplete();
        objective.MarkHostageFreed();
        objective.MarkHostageExtracted();

        Assert.That(objective.IsMissionSuccessCandidate, Is.True);
        Object.DestroyImmediate(go);
    }

    [Test]
    public void SuccessCandidate_FalseIfHostageNotFreed()
    {
        var go = new GameObject("ObjectiveServiceTests");
        var objective = go.AddComponent<ObjectiveService>();

        objective.MarkInfiltrationComplete();
        objective.MarkHostageExtracted();

        Assert.That(objective.IsMissionSuccessCandidate, Is.False);
        Object.DestroyImmediate(go);
    }
}
