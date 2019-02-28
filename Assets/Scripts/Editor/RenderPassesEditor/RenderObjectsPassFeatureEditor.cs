using System.Linq;
using UnityEngine;
using UnityEditorInternal;
using UnityEngine.Rendering.LWRP;

namespace UnityEditor.Rendering.LWRP 
{
	[CustomPropertyDrawer(typeof(RenderObjectsPassFeature.RenderObjectsSettings), true)]
    public class RenderObjectsPassFeatureEditor : PropertyDrawer
    {
	    internal class Styles
	    {
		    public static float defaultLineSpace = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
		    public static GUIContent callback = new GUIContent("Event", "Chose the Callback position for this render pass object.");

		    //Headers
		    public static GUIContent filtersHeader = new GUIContent("Filters", "Filters.");
		    public static GUIContent renderHeader = new GUIContent("Render Options", "Things.");
		    
		    //Filters
		    public static GUIContent renderQueueFilter = new GUIContent("Queue", "Filter the render queue range you want to render.");
		    public static GUIContent layerMask = new GUIContent("Layer Mask", "Chose the Callback position for this render pass object.");
		    public static GUIContent shaderPassFilter = new GUIContent("Shader Passes", "Chose the Callback position for this render pass object.");
		    
		    //Render Options
		    public static GUIContent overrideMaterial = new GUIContent("Material", "Chose an override material, every renderer will be rendered with this material.");
		    public static GUIContent overrideMaterialPass = new GUIContent("Pass Index", "The pass index for the override material to use.");
		    
		    //Depth Settings
		    public static GUIContent overrideDepth = new GUIContent("Override Depth", "Override depth rendering.");
		    public static GUIContent writeDepth = new GUIContent("Write Depth", "Chose to write depth to the screen.");
		    public static GUIContent depthState = new GUIContent("Depth Test", "Choose a new test setting for the depth.");
		    
		    //Stencil Settings
		    public static GUIContent overrideStencil = new GUIContent("Override Stencil", "Override stencil rendering.");
		    public static GUIContent stencilIndex = new GUIContent("Value", "The stencil index to write to.");
		    public static GUIContent stencilFunction = new GUIContent("Compare Function", "Choose the comparison function against the stencil value on screen.");
		    public static GUIContent stencilPass = new GUIContent("Pass", "What happens to the stencil value when passing.");
		    public static GUIContent stencilFail = new GUIContent("Fail", "What happens the the stencil value when failing.");
		    public static GUIContent stencilZFail = new GUIContent("Z Fail", "What happens the the stencil value when failing Z testing.");
		    
		    //Camera Settings
		    public static GUIContent overrideCamera = new GUIContent("Override Camera", "Override camera projections.");
		    public static GUIContent cameraFOV = new GUIContent("Field Of View", "Field Of View to render this pass in.");
		    public static GUIContent positionOffset = new GUIContent("Position Offset", "This Vector acts as a relative offset for the camera.");
		    public static GUIContent restoreCamera = new GUIContent("Restore", "Restore to the original camera projection before this pass.");

	    }

	    //Stencil rendering
	    private const int stencilBits = 3;
	    private const int minStencilValue = 0;
	    private const int maxStencilValue = (1 << stencilBits) - 1;

	    //Headers and layout
	    private HeaderBool m_FiltersFoldout;
	    private int m_FilterLines = 3;
	    private HeaderBool m_RenderFoldout;
	    private int m_RenderLines = 3;
	    private int m_DepthLines = 3;
	    private int m_StencilLines = 5;
	    private int m_CameraLines = 4;
	    
	    private bool firstTime = true;
	    
	    // Serialized Properties
	    private SerializedProperty m_Callback;
	    //Filter props
	    private SerializedProperty m_FilterSettings;
	    private SerializedProperty m_RenderQueue;
	    private SerializedProperty m_LayerMask;
	    private SerializedProperty m_ShaderPasses;
	    //Render props
	    private SerializedProperty m_OverrideMaterial;
	    private SerializedProperty m_OverrideMaterialPass;
	    //Depth props
	    private SerializedProperty m_OverrideDepth;
	    private SerializedProperty m_WriteDepth;
	    private SerializedProperty m_DepthState;
	    //Stencil props
	    private SerializedProperty m_OverrideStencil;
	    private SerializedProperty m_StencilIndex;
	    private SerializedProperty m_StencilFunction;
	    private SerializedProperty m_StencilPass;
	    private SerializedProperty m_StencilFail;
	    private SerializedProperty m_StencilZFail;
	    //Caemra props
	    private SerializedProperty m_CameraSettings;
	    private SerializedProperty m_OverrideCamera;
	    private SerializedProperty m_FOV;
	    private SerializedProperty m_CameraOffset;
	    private SerializedProperty m_RestoreCamera;

	    private ReorderableList m_ShaderPassesList;

	    private void Init(SerializedProperty property)
	    {
		    //Header bools
		    var key = $"{this.ToString().Split('.').Last()}.{property.serializedObject.targetObject.name}";
		    m_FiltersFoldout = new HeaderBool($"{key}.FiltersFoldout", true);
		    m_RenderFoldout = new HeaderBool($"{key}.RenderFoldout");

		    m_Callback = property.FindPropertyRelative("Event");
		    //Filter props
		    m_FilterSettings = property.FindPropertyRelative("filterSettings");
		    m_RenderQueue = m_FilterSettings.FindPropertyRelative("RenderQueueType");
		    m_LayerMask = m_FilterSettings.FindPropertyRelative("LayerMask");
		    m_ShaderPasses = m_FilterSettings.FindPropertyRelative("PassNames");
			//Render options
		    m_OverrideMaterial = property.FindPropertyRelative("overrideMaterial");
		    m_OverrideMaterialPass = property.FindPropertyRelative("overrideMaterialPassIndex");
		    //Depth props
		    m_OverrideDepth = property.FindPropertyRelative("overrideDepthState");
		    m_WriteDepth = property.FindPropertyRelative("enableWrite");
		    m_DepthState = property.FindPropertyRelative("depthCompareFunction");
		    //Stencil
		    m_OverrideStencil = property.FindPropertyRelative("overrideStencilState");
		    m_StencilIndex = property.FindPropertyRelative("stencilReference");
		    m_StencilFunction = property.FindPropertyRelative("stencilCompareFunction");
		    m_StencilPass = property.FindPropertyRelative("passOperation");
		    m_StencilFail = property.FindPropertyRelative("failOperation");
		    m_StencilZFail = property.FindPropertyRelative("zFailOperation");
		    //Camera
		    m_CameraSettings = property.FindPropertyRelative("cameraSettings");
		    m_OverrideCamera = m_CameraSettings.FindPropertyRelative("overrideCamera");
		    m_FOV = m_CameraSettings.FindPropertyRelative("cameraFieldOfView");
		    m_CameraOffset = m_CameraSettings.FindPropertyRelative("offset");
		    m_RestoreCamera = m_CameraSettings.FindPropertyRelative("restoreCamera");
		    
		    m_ShaderPassesList = new ReorderableList(null, m_ShaderPasses, true, true, true, true);

		    m_ShaderPassesList.drawElementCallback =
		    (Rect rect, int index, bool isActive, bool isFocused) =>
		    {
			    var element = m_ShaderPassesList.serializedProperty.GetArrayElementAtIndex(index);
			    var propRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
			    var labelWidth = EditorGUIUtility.labelWidth;
			    EditorGUIUtility.labelWidth = 50;
			    element.stringValue = EditorGUI.TextField(propRect, "Name", element.stringValue);
			    EditorGUIUtility.labelWidth = labelWidth;
		    };
		    
		    m_ShaderPassesList.drawHeaderCallback = (Rect testHeaderRect) => {
			    EditorGUI.LabelField(testHeaderRect, Styles.shaderPassFilter);
		    };
	    }

	    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
	    {
			rect.height = EditorGUIUtility.singleLineHeight;
			EditorGUI.BeginProperty(rect, label, property);
			if(firstTime)
			    Init(property);

			//Forward Callbacks
			EditorGUI.PropertyField(rect, m_Callback, Styles.callback);
			rect.y += Styles.defaultLineSpace;

			DoFilters(ref rect);

			m_RenderFoldout.value = EditorGUI.Foldout(rect, m_RenderFoldout.value, Styles.renderHeader, true);
			SaveHeaderBool(m_RenderFoldout);
			rect.y += Styles.defaultLineSpace;
			if (m_RenderFoldout.value)
			{
				EditorGUI.indentLevel++;
				//Override material
				EditorGUI.PropertyField(rect, m_OverrideMaterial, Styles.overrideMaterial);
				rect.y += Styles.defaultLineSpace;
				//Override material pass index
				EditorGUI.BeginChangeCheck();
				EditorGUI.PropertyField(rect, m_OverrideMaterialPass, Styles.overrideMaterialPass);
				if (EditorGUI.EndChangeCheck())
					m_OverrideMaterialPass.intValue = Mathf.Max(0, m_OverrideMaterialPass.intValue);
				rect.y += Styles.defaultLineSpace;
				//Override depth
				DoDepthOverride(ref rect);
				rect.y += Styles.defaultLineSpace;
				//Override stencil
				DoStencilOverride(ref rect);
				rect.y += Styles.defaultLineSpace;
				//Override camera
				DoCameraOverride(ref rect);
				rect.y += Styles.defaultLineSpace;

				EditorGUI.indentLevel--;
			}
			
			EditorGUI.EndProperty();
		    firstTime = false;
	    }

	    void DoFilters(ref Rect rect)
	    {
	        m_FiltersFoldout.value = EditorGUI.Foldout(rect, m_FiltersFoldout.value, Styles.filtersHeader, true);
	        SaveHeaderBool(m_FiltersFoldout);
		    rect.y += Styles.defaultLineSpace;
		    if (m_FiltersFoldout.value)
		    {
			    EditorGUI.indentLevel++;
			    //Render queue filter
			    EditorGUI.PropertyField(rect, m_RenderQueue, Styles.renderQueueFilter);
			    rect.y += Styles.defaultLineSpace;
			    //Layer mask
			    EditorGUI.PropertyField(rect, m_LayerMask, Styles.layerMask);
			    rect.y += Styles.defaultLineSpace;
			    //Shader pass list
			    EditorGUI.indentLevel--;
			    m_ShaderPassesList.DoList(rect);
			    rect.y += m_ShaderPassesList.GetHeight();
		    }
	    }

	    void DoDepthOverride(ref Rect rect)
	    {
		    EditorGUI.PropertyField(rect, m_OverrideDepth, Styles.overrideDepth);
		    if (m_OverrideDepth.boolValue)
		    {
			    rect.y += Styles.defaultLineSpace;
			    EditorGUI.indentLevel++;
			    //Write depth
			    EditorGUI.PropertyField(rect, m_WriteDepth, Styles.writeDepth);
			    rect.y += Styles.defaultLineSpace;
			    //Depth testing options
			    EditorGUI.PropertyField(rect, m_DepthState, Styles.depthState);
			    EditorGUI.indentLevel--;
		    }
	    }

	    void DoStencilOverride(ref Rect rect)
	    {
		    EditorGUI.PropertyField(rect, m_OverrideStencil, Styles.overrideStencil);
		    if (m_OverrideStencil.boolValue)
		    {
			    EditorGUI.indentLevel++;
			    rect.y += Styles.defaultLineSpace;
			    //Stencil value
			    EditorGUI.BeginChangeCheck();
			    var stencilVal = m_StencilIndex.intValue;
			    stencilVal = EditorGUI.IntSlider(rect, Styles.stencilIndex, stencilVal, minStencilValue, maxStencilValue);
			    if (EditorGUI.EndChangeCheck())
				    m_StencilIndex.intValue = stencilVal;
			    rect.y += Styles.defaultLineSpace;
			    //Stencil compare options
			    EditorGUI.PropertyField(rect, m_StencilFunction, Styles.stencilFunction);
			    rect.y += Styles.defaultLineSpace;
			    //Stencil compare options
			    EditorGUI.indentLevel++;
			    var stencilOpLabelRect = new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, rect.height);
			    EditorGUI.LabelField(stencilOpLabelRect, "Operations");
			    var indentLevel = EditorGUI.indentLevel;
			    EditorGUI.indentLevel = 0;
			    var labelWidth = EditorGUIUtility.labelWidth;
			    EditorGUIUtility.labelWidth = 50f;

			    var stencilOpPassRect = new Rect(rect.x + stencilOpLabelRect.width, rect.y, (rect.width - stencilOpLabelRect.width) * 0.5f, rect.height);
			    EditorGUI.PropertyField(stencilOpPassRect, m_StencilPass, Styles.stencilPass);
					
			    var stencilOpFailRect = new Rect(stencilOpPassRect.x + stencilOpPassRect.width, rect.y, stencilOpPassRect.width, rect.height);
			    EditorGUI.PropertyField(stencilOpFailRect, m_StencilFail, Styles.stencilFail);
					
			    EditorGUI.indentLevel = indentLevel - 1;
			    EditorGUIUtility.labelWidth = labelWidth;
			    rect.y += Styles.defaultLineSpace;
			    //Stencil compare options
			    EditorGUI.PropertyField(rect, m_StencilFunction, Styles.stencilZFail);
			    EditorGUI.indentLevel--;
		    }
	    }
	    
	    void DoCameraOverride(ref Rect rect)
	    {
		    EditorGUI.PropertyField(rect, m_OverrideCamera, Styles.overrideCamera);
		    if (m_OverrideCamera.boolValue)
		    {
			    rect.y += Styles.defaultLineSpace;
			    EditorGUI.indentLevel++;
			    //FOV
			    EditorGUI.Slider(rect, m_FOV, 4f, 179f, Styles.cameraFOV);
			    rect.y += Styles.defaultLineSpace;
			    //Offset vector
			    var offset = m_CameraOffset.vector4Value;
			    EditorGUI.BeginChangeCheck();
			    var newOffset = EditorGUI.Vector3Field(rect, Styles.positionOffset, new Vector3(offset.x, offset.y, offset.z));
			    if(EditorGUI.EndChangeCheck())
					m_CameraOffset.vector4Value = new Vector4(newOffset.x, newOffset.y, newOffset.z, 1f);
			    rect.y += Styles.defaultLineSpace;
			    //Restore prev camera projections
			    EditorGUI.PropertyField(rect, m_RestoreCamera, Styles.restoreCamera);
			    rect.y += Styles.defaultLineSpace;
			    
			    EditorGUI.indentLevel--;
		    }
	    }

	    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	    {
		    float height = Styles.defaultLineSpace;
		    if (!firstTime)
		    {
		        height += Styles.defaultLineSpace * (m_FiltersFoldout.value ? m_FilterLines : 1);
		        height += m_FiltersFoldout.value ? m_ShaderPassesList.GetHeight() : 0;

		        height += Styles.defaultLineSpace * (m_RenderFoldout.value ? m_RenderLines : 1);
			    if (m_RenderFoldout.value)
			    {
				    height += Styles.defaultLineSpace * (m_OverrideDepth.boolValue ? m_DepthLines : 1);
				    height += Styles.defaultLineSpace * (m_OverrideStencil.boolValue ? m_StencilLines : 1);
				    height += Styles.defaultLineSpace * (m_OverrideCamera.boolValue ? m_CameraLines : 1);
			    }
		    }

		    return height;
	    }
	    
	    private void SaveHeaderBool(HeaderBool boolObj)
	    {
		    EditorPrefs.SetBool(boolObj.key, boolObj.value);
	    }

	    class HeaderBool
	    {
		    public string key;
		    public bool value;

		    public HeaderBool(string _key, bool _default = false)
		    {
			    key = _key;
			    if (EditorPrefs.HasKey(key))
				    value = EditorPrefs.GetBool(key);
			    else
					value = _default;
			    EditorPrefs.SetBool(key, value);
		    }
	    }
    }
}
