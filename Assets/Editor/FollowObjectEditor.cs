using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FollowObject))]
public class FollowObjectEditor : Editor
{
    private FollowObject fo;

    private SerializedProperty follow;
    private SerializedProperty behaviour;
    private SerializedProperty dampening;
    private SerializedProperty parallax;
    private SerializedProperty x, y;
    private SerializedProperty minX, maxX;
    private SerializedProperty minY, maxY;

    private void OnEnable() {
        fo = target as FollowObject;
        follow = serializedObject.FindProperty("follow");
        behaviour = serializedObject.FindProperty("behaviour");
        dampening = serializedObject.FindProperty("dampening");
        parallax = serializedObject.FindProperty("parallax");
        x = serializedObject.FindProperty("x");
        y = serializedObject.FindProperty("y");
        minX = serializedObject.FindProperty("minX");
        minY = serializedObject.FindProperty("minY");
        maxX = serializedObject.FindProperty("maxX");
        maxY = serializedObject.FindProperty("maxY");
    }

    /// <inheritdoc />
    public override void OnInspectorGUI() {
        EditorGUILayout.PropertyField(follow);
        EditorGUILayout.PropertyField(behaviour);
        if (behaviour.enumValueIndex == 1) EditorGUILayout.PropertyField(dampening);
        if (behaviour.enumValueIndex == 2) EditorGUILayout.PropertyField(parallax);

        EditorGUIUtility.labelWidth = 15;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(x);
        EditorGUILayout.PropertyField(y);
        EditorGUILayout.EndHorizontal();

        EditorGUIUtility.labelWidth = 40;

        if (x.boolValue) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(minX);
            EditorGUILayout.PropertyField(maxX);
            EditorGUILayout.EndHorizontal();
        }
        if (y.boolValue) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(minY);
            EditorGUILayout.PropertyField(maxY);
            EditorGUILayout.EndHorizontal();
        }

        serializedObject.ApplyModifiedProperties();
    }
}