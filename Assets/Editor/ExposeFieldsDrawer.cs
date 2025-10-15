using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ExposeFieldsAttribute))]
public class ExposeFieldsDrawer : PropertyDrawer
{
    // https://discussions.unity.com/t/editor-tool-better-scriptableobject-inspector-editing/671671

    // Cached scriptable object editor
    private Editor editor = null;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        // Draw label
        EditorGUI.PropertyField(position, property, label, true);

        // Draw foldout arrow
        if (property.objectReferenceValue != null) {
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, GUIContent.none);
        }

        // Draw foldout properties
        if (property.isExpanded) {
            // Make child fields be indented
            EditorGUI.indentLevel++;

            // Draw object properties
            if (!editor) {
                Editor.CreateCachedEditor(property.objectReferenceValue, typeof(MonoBehaviourEditor), ref editor);
            }
            editor.OnInspectorGUI();

            // Set indent back to what it was
            EditorGUI.indentLevel--;
        }
    }
}