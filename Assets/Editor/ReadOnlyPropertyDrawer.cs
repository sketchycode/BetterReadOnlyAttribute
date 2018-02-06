using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;

[CustomPropertyDrawer (typeof (ReadOnlyAttribute))]
public class ReadOnlyPropertyDrawer : PropertyDrawer {
	const int spacer = 8;
    const int textHeight = 16;

	Color fontColor = new Color(.25f, .25f, .25f);
    ReadOnlyAttribute readOnlyAttribute { get { return ((ReadOnlyAttribute)attribute); } }

	public override float GetPropertyHeight (SerializedProperty prop, GUIContent label) {
		var allReadOnlyFields = GetAllReadOnlyAttributeFields(prop);
		return AmIFirstReadOnly(prop, allReadOnlyFields) ? 
			(prop.isExpanded ? (allReadOnlyFields.Length + 1) * textHeight : textHeight) + spacer :
			0;
    }

	public override void OnGUI (Rect position, SerializedProperty prop, GUIContent label) {
		position.y += spacer;
		var allReadOnlyFields = GetAllReadOnlyAttributeFields(prop);
		if(AmIFirstReadOnly(prop, allReadOnlyFields)) {
			int currentIndent = EditorGUI.indentLevel;

			EditorGUI.indentLevel = 0;
			var style = new GUIStyle(EditorStyles.foldout);
			SetStyleTextColor(style);

			prop.isExpanded = EditorGUI.Foldout(position, prop.isExpanded, readOnlyAttribute.grouping, style);
			
			if(prop.isExpanded) {
				position.y += textHeight;
				DrawAllReadOnlyProperties(position, allReadOnlyFields, prop.serializedObject.targetObject);
			}

			EditorGUI.indentLevel = currentIndent;
		}
	}

	void DrawAllReadOnlyProperties(Rect position, FieldInfo[] readOnlyFields, Object targetObject) {
		int indent = EditorGUI.indentLevel;
		GUIStyle style = new GUIStyle(EditorStyles.label);
		EditorStyles.label.normal.textColor = fontColor;
		SetStyleTextColor(style);

		EditorGUI.indentLevel = indent + 1;
		EditorGUI.BeginDisabledGroup(true);
		for(int i=0; i<readOnlyFields.Length; i++) {
			RenderAppropriateEditorElement(readOnlyFields[i], targetObject, position, style);
			position.y += textHeight;
		}
		EditorGUI.EndDisabledGroup();

		EditorGUI.indentLevel = indent;
		EditorStyles.label.normal.textColor = Color.black;
	}

	private bool AmIFirstReadOnly(SerializedProperty prop, FieldInfo[] allReadOnlyFields) {
		string propName = prop.name;
		return allReadOnlyFields.Length > 0 && allReadOnlyFields[0].Name == propName;
	}

	private FieldInfo[] GetAllReadOnlyAttributeFields(SerializedProperty prop) {
		var otherFields = prop.serializedObject.targetObject.GetType().GetFields();
		return otherFields.Where(fi =>
			fi.GetCustomAttributes(true).Any(attr =>
				attr is ReadOnlyAttribute && ((ReadOnlyAttribute)attr).grouping == this.readOnlyAttribute.grouping))
				.ToArray();
	}

	private void RenderAppropriateEditorElement(FieldInfo fieldInfo, object targetObject, Rect position, GUIStyle style) {
		var value = fieldInfo.GetValue(targetObject);
		var label1 = new GUIContent() { text = fieldInfo.Name };
		var label2 = new GUIContent() { text = fieldInfo.GetValue(targetObject).ToString() };
		
		if(value is int || value is float || value is string) {
			EditorGUI.LabelField(position, label1, label2, style);
		}
		else if(value is bool) {
			EditorGUI.Toggle(position, label1, (bool)value);
		}
		else if(value is Vector2) {
			EditorGUI.Vector2Field(position, label1, (Vector2)value);
		}
		else if(value is Vector3) {
			EditorGUI.Vector3Field(position, label1, (Vector3)value);
		}
	}

	private void SetStyleTextColor(GUIStyle style) {
		style.onNormal.textColor = fontColor;
		style.normal.textColor = fontColor;
		style.onActive.textColor = fontColor;
		style.active.textColor = fontColor;
		style.onFocused.textColor = fontColor;
		style.focused.textColor = fontColor;
	}
}