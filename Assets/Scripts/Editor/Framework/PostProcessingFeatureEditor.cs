using System;
using System.Collections.Generic;
using System.Reflection;
using Common.Editor.EditorExtensions;
using PostProcessingSandbox.Runtime.Framework;
using UnityEditor;
using UnityEngine;

namespace PostProcessingSandbox.Editor.Framework
{
    [CustomEditor(typeof(PostProcessingFeature))]
    public class PostProcessingFeatureEditor : UnityEditor.Editor
    {
        private static string[] _invalidTypePrefixes =
        {
            "System",
            "Unity",
            "unity",
            "Mono.",
            "JetBrains.",
            "Bee",
            "mscorlib",
            "netstandard",
            "nunit",
            "log4net",
            "PPv2URPConverters"
        };

        private static Type _volumeType = typeof(PostProcessingVolume);
        
        private bool _typesFoldout = true;

        private List<string> _availableTypes = new List<string>();
        private string[] _availableTypesNames;
        
        private void Awake()
        {
            this._availableTypes = new List<string>();
            List<string> names = new List<string>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                string assemblyName = assembly.GetName().Name;
                bool invalid = false;
                foreach (string invalidTypePrefix in PostProcessingFeatureEditor._invalidTypePrefixes)
                {
                    if (assemblyName.StartsWith(invalidTypePrefix))
                    {
                        invalid = true;
                        break;
                    }
                }

                if (invalid)
                {
                    continue;
                }
                
                foreach (Type exportedType in assembly.GetTypes())
                {
                    if (!PostProcessingFeatureEditor._volumeType.IsAssignableFrom(exportedType))
                    {
                        continue;
                    }

                    if (
                        assembly == Assembly.GetAssembly(PostProcessingFeatureEditor._volumeType) &&
                        exportedType.Name.StartsWith("PostProcessingVolume")
                    ) {
                        continue;
                    }
                    
                    this._availableTypes.Add(exportedType.AssemblyQualifiedName);
                    names.Add($"{assemblyName}.{exportedType.Name}");
                }
            }
            
            this._availableTypesNames = names.ToArray();
        }

        public override void OnInspectorGUI()
        {
            PostProcessingFeature editTarget = (PostProcessingFeature) this.target;

            this._typesFoldout = EditorListDisplay.Display(
                this._typesFoldout,
                "Post Processing Effects",
                editTarget.Effects,
                i =>
                {
                    string type = editTarget.Effects[i];

                    int popupIndex = -1;
                    if (this._availableTypes.Contains(type))
                    {
                        popupIndex = this._availableTypes.IndexOf(type);
                    }

                    popupIndex = EditorGUILayout.Popup(popupIndex, this._availableTypesNames);

                    if (popupIndex == -1)
                    {
                        return;
                    }
                    
                    editTarget.Effects[i] = this._availableTypes[popupIndex];
                },
                () => editTarget.Effects.Add(null),
                type => editTarget.Effects.Remove(type),
                this.Repaint
            );
            
            if (!this.serializedObject.hasModifiedProperties) return;

            this.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(this.target);
        }
    }
}