﻿using UnityEngine;
using MalbersAnimations.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations.Scriptables
{
    [CreateAssetMenu(menuName = "Malbers Animations/Collections/Runtime GameObject Set", order = 1000, fileName = "New Runtime Gameobject Collection")]
    public class RuntimeGameObjects : RuntimeCollection<GameObject>
    {
        public GameObjectEvent OnItemAdded = new GameObjectEvent();
        public GameObjectEvent OnItemRemoved = new GameObjectEvent();



        /// <summary>Return the Closest game object from an origin</summary>

        public GameObject Item_GetClosest(GameObject origin)
        {
            GameObject closest = null;

            float minDistance = float.MaxValue;

            foreach (var item in items)
            {
                var Distance = Vector3.Distance(item.transform.position, origin.transform.position);

                if (Distance < minDistance)
                {
                    closest = item;
                    minDistance = Distance;
                }
            }
            return closest;
        }


        public override void Item_Add(GameObject newItem)
        {
            items?.RemoveAll(x => x == null); //REmove all Assets that are Empty

            if (!items.Contains(newItem))
            {
                items.Add(newItem);
                OnItemAdded.Invoke(newItem);
            }
        }

        public override void Item_Remove(GameObject newItem)
        {
            if (items.Contains(newItem))
            {
                items.Remove(newItem);
                OnItemRemoved.Invoke(newItem);
            }

            if (items == null || items.Count == 0)
                Clear();
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(RuntimeGameObjects))]
    public class RuntimeGameObjectsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (Application.isPlaying)
            {
                var M = (RuntimeGameObjects)target;
                MalbersEditor.DrawHeader(M.name + " - List");
               
                EditorGUI.BeginDisabledGroup(true);
                for (int i = 0; i < M.Items.Count; i++)
                {
                    EditorGUILayout.ObjectField("Item " + i, M.Items[i], typeof(GameObject), false);
                }
                EditorGUI.EndDisabledGroup();
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("Description"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnSetEmpty"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnItemAdded"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnItemRemoved"));

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}

