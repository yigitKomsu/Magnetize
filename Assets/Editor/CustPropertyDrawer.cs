using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ArrayLayout))]
public class CustPropertyDrawer : PropertyDrawer {


	public override void OnGUI(Rect position,SerializedProperty property,GUIContent label){
		EditorGUI.PrefixLabel(position,label);
		Rect newposition = position;
		newposition.y += 18f;
		SerializedProperty data = property.FindPropertyRelative("row");

		for(int j=0;j<5;j++){
			SerializedProperty row = data.GetArrayElementAtIndex(j).FindPropertyRelative("column");
			newposition.height = 18f;
            if (row.arraySize != 5)
                row.arraySize = 5;
            newposition.width = position.width/7;
			for(int i=0;i<5;i++){
				EditorGUI.PropertyField(newposition,row.GetArrayElementAtIndex(i),GUIContent.none);
				newposition.x += newposition.width;
			}

			newposition.x = position.x;
			newposition.y += 18f;
		}
	}

	public override float GetPropertyHeight(SerializedProperty property,GUIContent label){
		return 18f * 8;
	}
}
