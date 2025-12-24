using UnityEditor;
using TwelveG.GameController;
using System.Collections.Generic;

namespace TwelveG.EditorScripts
{
    [CustomEditor(typeof(EventsHandler))]
    public class EventsHandlerEditor : Editor
    {
        SerializedProperty introEvents;
        SerializedProperty afternoonEvents;
        SerializedProperty eveningEvents;
        SerializedProperty nightEvents;

        SerializedProperty freeRoam;
        SerializedProperty loadSpecificEvent;
        SerializedProperty eventEnumToLoad;
        SerializedProperty loadNextSceneOnEventsFinished;

        SerializedProperty headacheVFXIntensity;
        SerializedProperty electricFeelVFXIntensity;
        SerializedProperty weatherEvent;
        SerializedProperty enableFlashlight;
        SerializedProperty enableWalkieTalkie;

        private void OnEnable()
        {
            introEvents = serializedObject.FindProperty("introEvents");
            afternoonEvents = serializedObject.FindProperty("afternoonEvents");
            eveningEvents = serializedObject.FindProperty("eveningEvents");
            nightEvents = serializedObject.FindProperty("nightEvents");

            freeRoam = serializedObject.FindProperty("freeRoam");
            loadSpecificEvent = serializedObject.FindProperty("loadSpecificEvent");
            eventEnumToLoad = serializedObject.FindProperty("eventEnumToLoad");
            loadNextSceneOnEventsFinished = serializedObject.FindProperty("loadNextSceneOnEventsFinished");

            headacheVFXIntensity = serializedObject.FindProperty("headacheVFXIntensity");
            electricFeelVFXIntensity = serializedObject.FindProperty("electricFeelVFXIntensity");
            weatherEvent = serializedObject.FindProperty("weatherEvent");
            enableFlashlight = serializedObject.FindProperty("enableFlashlight");
            enableWalkieTalkie = serializedObject.FindProperty("enableWalkieTalkie");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(introEvents);
            EditorGUILayout.PropertyField(afternoonEvents);
            EditorGUILayout.PropertyField(eveningEvents);
            EditorGUILayout.PropertyField(nightEvents);
            EditorGUILayout.Space(10);


            if (!loadSpecificEvent.boolValue)
            {
                EditorGUILayout.PropertyField(freeRoam);
            }

            if (!freeRoam.boolValue)
            {
                EditorGUILayout.PropertyField(loadNextSceneOnEventsFinished);
                EditorGUILayout.PropertyField(loadSpecificEvent);
            }

            if (loadSpecificEvent.boolValue)
            {
                EditorGUI.indentLevel++;
                DrawFilteredEnumPopup();
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(10);

            EditorGUILayout.PropertyField(headacheVFXIntensity);
            EditorGUILayout.PropertyField(electricFeelVFXIntensity);
            EditorGUILayout.Space(10);

            EditorGUILayout.PropertyField(weatherEvent);
            EditorGUILayout.Space(10);

            EditorGUILayout.PropertyField(enableFlashlight);
            EditorGUILayout.PropertyField(enableWalkieTalkie);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawFilteredEnumPopup()
        {
            SceneEnum currentSceneName = SceneUtils.RetrieveCurrentSceneEnum();
            
            List<string> displayOptions = new List<string>();
            List<int> optionValues = new List<int>();
            if (currentSceneName == SceneEnum.Afternoon) // Pon el nombre real de tu escena
            {
                AddEnumOption(EventsEnum.Afternoon_Narrative_Intro, displayOptions, optionValues);
                AddEnumOption(EventsEnum.TVTime, displayOptions, optionValues);
            }
            else if (currentSceneName == SceneEnum.Evening) // Pon el nombre real de tu escena
            {
                // Agrega aquí los de la noche (Evening)
                AddEnumOption(EventsEnum.Evening_Narrative_Intro, displayOptions, optionValues);
                AddEnumOption(EventsEnum.WakeUp, displayOptions, optionValues);
                AddEnumOption(EventsEnum.Birds, displayOptions, optionValues);
                AddEnumOption(EventsEnum.PizzaTime, displayOptions, optionValues);
                AddEnumOption(EventsEnum.LostSignal1, displayOptions, optionValues);
                AddEnumOption(EventsEnum.LostSignal2, displayOptions, optionValues);
                AddEnumOption(EventsEnum.FernandezSuicide, displayOptions, optionValues);
                AddEnumOption(EventsEnum.WalkieTalkieQuest, displayOptions, optionValues);
                AddEnumOption(EventsEnum.FirstContact, displayOptions, optionValues);
                AddEnumOption(EventsEnum.Noises, displayOptions, optionValues);
                AddEnumOption(EventsEnum.Headaches, displayOptions, optionValues);
            }
            else if (currentSceneName == SceneEnum.Night) // Pon el nombre real de tu escena
            {
                // TODO: Agregar los de la noche
            }
            
            // Fallback: Si la lista está vacía (ej: estamos en cualquier Menu), mostramos todo por seguridad
            if (displayOptions.Count == 0)
            {
                EditorGUILayout.PropertyField(eventEnumToLoad);
                return;
            }

            int currentEnumInt = eventEnumToLoad.enumValueIndex;
            
            int selectedValue = EditorGUILayout.IntPopup("Event To Load", currentEnumInt, displayOptions.ToArray(), optionValues.ToArray());

            eventEnumToLoad.enumValueIndex = selectedValue;
        }

        private void AddEnumOption(EventsEnum ev, List<string> labels, List<int> values)
        {
            labels.Add(ev.ToString());
            values.Add((int)ev);
        }
    }
}