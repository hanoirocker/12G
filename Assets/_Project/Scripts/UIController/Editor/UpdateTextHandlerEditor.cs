using UnityEditor;
using TwelveG.UIController;

namespace TwelveG.EditorScripts
{
    [CustomEditor(typeof(UpdateTextHandler))]
    public class UpdateTextHandlerEditor : Editor
    {
        SerializedProperty isTMPSingleText;
        SerializedProperty isTMPDropDownList;
        SerializedProperty textDependsOnEvents;
        
        SerializedProperty uIOptionTextSO;
        SerializedProperty textListSO;

        private void OnEnable()
        {
            isTMPSingleText = serializedObject.FindProperty("isTMPSingleText");
            isTMPDropDownList = serializedObject.FindProperty("isTMPDropDownList");
            textDependsOnEvents = serializedObject.FindProperty("textDependsOnEvents");
            
            uIOptionTextSO = serializedObject.FindProperty("uIOptionTextSO");
            textListSO = serializedObject.FindProperty("textListSO");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(isTMPSingleText);
            EditorGUILayout.PropertyField(isTMPDropDownList);
            EditorGUILayout.PropertyField(textDependsOnEvents);

            EditorGUILayout.Space(10);

            if (isTMPSingleText.boolValue)
            {
                EditorGUILayout.LabelField("Singular Text SO's", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(uIOptionTextSO);
            }

            if (isTMPDropDownList.boolValue)
            {
                EditorGUILayout.LabelField("List Text SO's", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(textListSO);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}