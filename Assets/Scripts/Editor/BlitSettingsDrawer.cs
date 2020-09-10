using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[CustomPropertyDrawer(typeof(Blit.BlitSettings))]
public class BlitSettingsDrawer : PropertyDrawer
{
    static class Styles
    {
        public static readonly GUIContent sourceTypeLabel = EditorGUIUtility.TrTextContent("Source Type");
        public static readonly GUIContent destinationTypeLabel = EditorGUIUtility.TrTextContent("Destination Type");
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUILayout.PropertyField(property.FindPropertyRelative("renderPassEvent"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("blitMaterial"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("blitMaterialPassIndex"));

        var sourceTypeProperty = property.FindPropertyRelative("sourceType");
        var destinationTypeProperty = property.FindPropertyRelative("destinationType");

        EditorGUI.BeginChangeCheck();
        var selectedSourceType = (BufferType)EditorGUILayout.EnumPopup(Styles.sourceTypeLabel, (BufferType)sourceTypeProperty.enumValueIndex);
        if (EditorGUI.EndChangeCheck())
            sourceTypeProperty.enumValueIndex = (int)selectedSourceType;

        if (selectedSourceType != BufferType.CameraColor)
            EditorGUILayout.PropertyField(property.FindPropertyRelative("sourceTextureId"));

        EditorGUI.BeginChangeCheck();
        var selectedDestinationType = (BufferType)EditorGUILayout.EnumPopup(Styles.destinationTypeLabel, (BufferType)destinationTypeProperty.enumValueIndex);
        if (EditorGUI.EndChangeCheck())
            destinationTypeProperty.enumValueIndex = (int)selectedDestinationType;

        if (selectedDestinationType != BufferType.CameraColor)
            EditorGUILayout.PropertyField(property.FindPropertyRelative("destinationTextureId"));

        EditorGUI.EndProperty();
    }
}
