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
        SerializedProperty eventDrivenTextSO;

        SerializedProperty uITextType;
        
        SerializedProperty uIOptionTextSO;
        SerializedProperty textListSO;

        private void OnEnable()
        {
            isTMPSingleText = serializedObject.FindProperty("isTMPSingleText");
            isTMPDropDownList = serializedObject.FindProperty("isTMPDropDownList");
            textDependsOnEvents = serializedObject.FindProperty("textDependsOnEvents");
            eventDrivenTextSO = serializedObject.FindProperty("eventDrivenTextSO");

            uITextType = serializedObject.FindProperty("uITextType");
            
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
            EditorGUILayout.PropertyField(uITextType);

            EditorGUILayout.Space(10);

            if (isTMPSingleText.boolValue)
            {
                EditorGUILayout.PropertyField(uIOptionTextSO);
            }

            if (isTMPDropDownList.boolValue)
            {
                EditorGUILayout.PropertyField(textListSO);
            }

            if( textDependsOnEvents.boolValue)
            {
                EditorGUILayout.PropertyField(eventDrivenTextSO);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}