using UnityEditor;
using UnityEngine;

public static class CameraDiagnostic
{
    [MenuItem("TacticalBreach/Diagnose Scene Camera")]
    public static void Diagnose()
    {
        var cam = Camera.main;
        if (cam == null) { Debug.LogError("Main Camera not found!"); return; }

        Debug.Log($"[Camera] Position: {cam.transform.position}, Rotation: {cam.transform.rotation.eulerAngles}");
        Debug.Log($"[Camera] Near Clip: {cam.nearClipPlane}, Far Clip: {cam.farClipPlane}");
        Debug.Log($"[Camera] Orthographic: {cam.orthographic}, Size: {cam.orthographicSize}");
        
        var floor = GameObject.Find("World_Base");
        if (floor != null) Debug.Log($"[Floor] Position: {floor.transform.position}");
        
        var ops = Object.FindObjectsByType<TacticalBreach.Squad.OperativeMember>(FindObjectsInactive.Include);
        foreach(var op in ops) Debug.Log($"[Operative] {op.name} Position: {op.transform.position}");
    }
}
