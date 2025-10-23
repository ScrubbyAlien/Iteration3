using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GeometryBody))]
public class GeometryBodyEditor : Editor
{
    private bool showInfo = true;
    private GeometryBody body;
    
    public void OnEnable() {
        body = target as GeometryBody;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        showInfo = EditorGUILayout.Foldout(showInfo, "Info");

        if (showInfo) {
            EditorGUI.indentLevel++;
            GUI.enabled = false;

            EditorGUILayout.FloatField("Current Gravity", body.g);
            EditorGUILayout.Vector2Field("Position", body.position);
            EditorGUILayout.Vector2Field("Linear Velocity", body.linearVelocity);
            EditorGUILayout.FloatField("Rotation", body.rotation);
            EditorGUILayout.FloatField("Angular Velocity", body.angularVelocity);
            
            GUI.enabled = true;
            EditorGUI.indentLevel--;
        }
    }
}

