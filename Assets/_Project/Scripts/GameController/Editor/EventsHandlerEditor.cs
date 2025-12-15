using TwelveG.GameController;
using UnityEditor;

namespace TwelveG.EditorScripts
{
    [CustomEditor(typeof(EventsHandler))]
    public class EventsHandlerEditor : Editor
    {
        // Propiedades Serializadas
        SerializedProperty introEvents;
        SerializedProperty afternoonEvents;
        SerializedProperty eveningEvents;
        SerializedProperty nightEvents;

        SerializedProperty freeRoam;
        SerializedProperty loadSpecificEvent;
        SerializedProperty eventIndexToLoad;

        SerializedProperty weatherEvent;

        SerializedProperty enableFlashlight;
        SerializedProperty enableWalkieTalkie;

        private void OnEnable()
        {
            // Vinculamos las propiedades por nombre
            introEvents = serializedObject.FindProperty("introEvents");
            afternoonEvents = serializedObject.FindProperty("afternoonEvents");
            eveningEvents = serializedObject.FindProperty("eveningEvents");
            nightEvents = serializedObject.FindProperty("nightEvents");

            freeRoam = serializedObject.FindProperty("freeRoam");
            loadSpecificEvent = serializedObject.FindProperty("loadSpecificEvent");
            eventIndexToLoad = serializedObject.FindProperty("eventIndexToLoad");

            weatherEvent = serializedObject.FindProperty("weatherEvent");

            enableFlashlight = serializedObject.FindProperty("enableFlashlight");
            enableWalkieTalkie = serializedObject.FindProperty("enableWalkieTalkie");
        }

        public override void OnInspectorGUI()
        {
            // Actualizamos la representación de los datos
            serializedObject.Update();

            // --- 1. EVENTS REFERENCES (Siempre visible) ---
            EditorGUILayout.PropertyField(introEvents);
            EditorGUILayout.PropertyField(afternoonEvents);
            EditorGUILayout.PropertyField(eveningEvents);
            EditorGUILayout.PropertyField(nightEvents);

            EditorGUILayout.Space(10);

            // Solo mostramos Free Roam si Load Specific Event NO está activado
            if (!loadSpecificEvent.boolValue)
            {
                EditorGUILayout.PropertyField(freeRoam);
            }

            // Solo mostramos Load Specific Event si Free Roam NO está activado
            if (!freeRoam.boolValue)
            {
                EditorGUILayout.PropertyField(loadSpecificEvent);
            }

            // Si Load Specific Event está activado, mostramos el índice
            if (loadSpecificEvent.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(eventIndexToLoad);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(10);

            EditorGUILayout.PropertyField(weatherEvent);

            EditorGUILayout.Space(10);

            EditorGUILayout.PropertyField(enableFlashlight);
            EditorGUILayout.PropertyField(enableWalkieTalkie);

            // Guardamos cualquier cambio realizado en el inspector
            serializedObject.ApplyModifiedProperties();
        }
    }
}