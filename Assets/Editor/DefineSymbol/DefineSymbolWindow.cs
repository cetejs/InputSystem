using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace MiniFramework
{
    internal sealed class DefineSymbolWindow : EditorWindow
    {
        private const string DefineSymbolPath = "Editor/DefineSymbol/DefineSymbol.txt";

        private BuildTargetGroup targetGroup = BuildTargetGroup.Standalone;
        private readonly List<DefineSymbol> defineSymbols = new List<DefineSymbol>();

        [MenuItem("Tool/DefineSymbolWindow")]
        private static void CreatWindow()
        {
            DefineSymbolWindow window = GetWindow<DefineSymbolWindow>();
            window.titleContent.text = "DefineSymbolWindow";
        }

        private void OnEnable()
        {
            SwitchPlatform();
            CollectDefineSymbol();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                Rect optionsRect = GUILayoutUtility.GetRect(0, 20, GUILayout.ExpandWidth(false));
                if (GUILayout.Button("Options", EditorStyles.toolbarDropDown, GUILayout.ExpandWidth(false)))
                {
                    GenericMenu options = new GenericMenu();
                    options.AddItem(new GUIContent("Show in Explorer"), false, OnShowInExplorerClick);
                    options.AddItem(new GUIContent("Clear All"), false, OnClearAllClick);
                    options.DropDown(optionsRect);
                }
            }
            EditorGUILayout.EndHorizontal();
            var lastTargetGroup = targetGroup;
            targetGroup = (BuildTargetGroup)EditorGUILayout.EnumPopup("targetGroup", targetGroup);
            for (int i = 0; i < defineSymbols.Count; i++)
            {
                DefineSymbol defineSymbol = defineSymbols[i];
                defineSymbol.isActive = EditorGUILayout.Toggle(defineSymbol.content, defineSymbol.isActive);
                defineSymbols[i] = defineSymbol;
            }

            if (GUILayout.Button("Refresh") || lastTargetGroup != targetGroup)
            {
                OnRefreshClick();
            }

            if (GUILayout.Button("Save"))
            {
                OnSaveClick();
            }
        }

        private void SwitchPlatform()
        {
            targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        }

        private void CollectDefineSymbol()
        {
            defineSymbols.Clear();
            var activeDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            var fullPath = Path.Combine(Application.dataPath, DefineSymbolPath);
            if (File.Exists(fullPath))
            {
                var lines = File.ReadAllText(fullPath).Split(new[]
                {
                    "\r\n",
                    "\n"
                }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    DefineSymbol defineSymbol = new DefineSymbol
                    {
                        content = line,
                        isActive = activeDefineSymbols.Contains(line)
                    };
                    defineSymbols.Add(defineSymbol);
                }
            }
            else
            {
                Debug.LogError($"Path '{fullPath}' is not exist");
                Close();
            }
        }

        private void SaveDefineSymbol()
        {
            var sb = new StringBuilder();
            foreach (DefineSymbol defineSymbol in defineSymbols)
            {
                if (defineSymbol.isActive)
                {
                    sb.AppendFormat("{0};", defineSymbol.content);
                }
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, sb.ToString());
        }

        private void ClearDefineSymbol()
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, "");
        }

        private void OnShowInExplorerClick()
        {
            var fullPath = Path.Combine(Application.dataPath, DefineSymbolPath);
            EditorUtility.RevealInFinder(fullPath);
        }

        private void OnClearAllClick()
        {
            ClearDefineSymbol();
        }

        private void OnRefreshClick()
        {
            CollectDefineSymbol();
        }

        private void OnSaveClick()
        {
            SaveDefineSymbol();
        }

        private struct DefineSymbol
        {
            public string content;
            public bool isActive;
        }
    }
}