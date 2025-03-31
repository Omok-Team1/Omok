#if UNITY_EDITOR
using System; 
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

[CustomPropertyDrawer(typeof(MessagePrimitiveParams))]
public class EventMessageGeneratorDrawer : PropertyDrawer
{
    private List<SerializedProperty> _primitiveDataProp = new();
    private List<string> _primitiveDataName = new();
    private List<int> _values = new();
    
    private string name;
    private bool cache = false;
    private int selectedValue = 0;
    
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // 두 줄 추가하여 높이 확보
        return EditorGUIUtility.singleLineHeight * 3;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (cache is false)
        {
            name = property.displayName;
            
            property.NextVisible(true);

            int count = 0;
            
            while (property.NextVisible(false))
            {
                _primitiveDataName.Add(property.type);
                _primitiveDataProp.Add(property.Copy());
                _values.Add(count++);
            }
            
            cache = true;
        }
        
        Rect contentPosition = EditorGUI.PrefixLabel(position, new GUIContent(name));
        
        if (position.height > 16f)
        {
            position.height = 16f;
            contentPosition = EditorGUI.IndentedRect(position);
            contentPosition.y += 18f;
        }
        
        float half = contentPosition.width / 2;
        Rect labelRect = contentPosition;
        
        EditorGUI.LabelField(labelRect, new GUIContent("Data Types"));
        labelRect.x += half;
        EditorGUI.LabelField(labelRect, new GUIContent("Setting Value"));
        
        EditorGUIUtility.labelWidth = 14f;
        contentPosition.y += 18f;
        contentPosition.width *= 0.5f;
        
        EditorGUI.BeginProperty(contentPosition, GUIContent.none, _primitiveDataProp[selectedValue]);
        {
            EditorGUI.BeginChangeCheck();
            selectedValue = EditorGUI.IntPopup(contentPosition, selectedValue, _primitiveDataName.ToArray(), _values.ToArray());

            contentPosition.x += half;
            EditorGUI.PropertyField(contentPosition, _primitiveDataProp[selectedValue], GUIContent.none);

            if (EditorGUI.EndChangeCheck())
            {
                _primitiveDataProp[selectedValue].serializedObject.ApplyModifiedProperties();
            }
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("🔹 변수 목록 (텍스트)", EditorStyles.boldLabel);
        
            foreach (var idx in _values)
            {
                EditorGUILayout.BeginHorizontal();
                string value = GetPropertyValueAsString(_primitiveDataProp[idx]);
            
                EditorGUILayout.LabelField(_primitiveDataProp[idx].displayName, GUILayout.Width(120));

                EditorGUILayout.LabelField(value, GUILayout.ExpandWidth(true));
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }
        EditorGUI.EndProperty();
    }
    

    
    private string GetPropertyValueAsString(SerializedProperty property)
    {
        switch (property.propertyType)
        {
            case SerializedPropertyType.Integer: return property.intValue.ToString();
            case SerializedPropertyType.Float: return property.floatValue.ToString();
            case SerializedPropertyType.Boolean: return property.boolValue ? "true" : "false";
            case SerializedPropertyType.String: return property.stringValue;
            case SerializedPropertyType.ObjectReference: return property.objectReferenceValue ? property.objectReferenceValue.name : "None";
            default: return "(지원되지 않는 타입)";
        }
    }
}
#endif
