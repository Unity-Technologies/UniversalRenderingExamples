using UnityEditor;

namespace UnityEngine.Rendering.Universal
{
    [CustomPropertyDrawer(typeof(DrawFullscreenFeature.Settings))]
    public class DrawFullScreenFeatureDrawer : PropertyDrawer
    {
        static class Styles
        {
            public static readonly GUIContent materialLabel = EditorGUIUtility.TrTextContent("Material");
            public static readonly GUIContent materialPassLabel = EditorGUIUtility.TrTextContent("Material Pass");
            public static readonly GUIContent sourceTypeLabel = EditorGUIUtility.TrTextContent("Source Type");
            public static readonly GUIContent destinationTypeLabel = EditorGUIUtility.TrTextContent("Destination Type");
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var renderPassEventProperty = property.FindPropertyRelative("renderPassEvent");
            var materialProperty = property.FindPropertyRelative("blitMaterial");
            var materialPassProperty = property.FindPropertyRelative("blitMaterialPassIndex");
            var sourceTypeProperty = property.FindPropertyRelative("sourceType");
            var destinationTypeProperty = property.FindPropertyRelative("destinationType");
            var sourceTextureIdProperty = property.FindPropertyRelative("sourceTextureId");
            var destinationTextureIdProperty = property.FindPropertyRelative("destinationTextureId");

            EditorGUI.BeginProperty(position, label, property);
            EditorGUILayout.PropertyField(renderPassEventProperty);

            EditorGUI.BeginChangeCheck();
            Material material = EditorGUILayout.ObjectField(Styles.materialLabel, materialProperty.objectReferenceValue, typeof(Material), allowSceneObjects: false) as Material;
            if (EditorGUI.EndChangeCheck())
                materialProperty.objectReferenceValue = material;

            DisplayPassPopup(material, materialPassProperty);

            EditorGUI.BeginChangeCheck();
            var selectedSourceType = (BufferType)EditorGUILayout.EnumPopup(Styles.sourceTypeLabel, (BufferType)sourceTypeProperty.enumValueIndex);
            if (EditorGUI.EndChangeCheck())
                sourceTypeProperty.enumValueIndex = (int)selectedSourceType;

            if (selectedSourceType != BufferType.CameraColor)
                EditorGUILayout.PropertyField(sourceTextureIdProperty);

            EditorGUI.BeginChangeCheck();
            var selectedDestinationType = (BufferType)EditorGUILayout.EnumPopup(Styles.destinationTypeLabel, (BufferType)destinationTypeProperty.enumValueIndex);
            if (EditorGUI.EndChangeCheck())
                destinationTypeProperty.enumValueIndex = (int)selectedDestinationType;

            if (selectedDestinationType != BufferType.CameraColor)
                EditorGUILayout.PropertyField(destinationTextureIdProperty);

            EditorGUI.EndProperty();
        }

        void DisplayPassPopup(Material material, SerializedProperty materialPassProperty)
        {
            if (material != null)
            {
                int passCount = material.passCount;
                if (passCount == 0)
                    return;

                string[] labels = new string[passCount];
                int[] options = new int[passCount];
                for (int i = 0; i < passCount; ++i)
                {
                    string passName = material.GetPassName(i);
                    if (passName.Length == 0)
                        passName = "Unnamed Pass";

                    labels[i] = string.Format("{0}: {1}", i, passName);
                    options[i] = i;
                }

                EditorGUI.BeginChangeCheck();
                int option = EditorGUILayout.IntPopup("Material Pass", materialPassProperty.intValue, labels, options);
                if (EditorGUI.EndChangeCheck())
                    materialPassProperty.intValue = option;
            }
        }
    }
}
