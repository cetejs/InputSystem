using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace MiniFramework
{
    internal sealed class SymbolWindow : EditorWindow
    {
        private const string DefineSymbolPath = "Editor/DefineSymbol/Symbols.txt";

        private BuildTargetGroup targetGroup = BuildTargetGroup.Standalone;
        private readonly List<Symbol> symbols = new List<Symbol>();

        [MenuItem("Tool/SymbolWindow")]
        private static void CreatWindow()
        {
            var window = GetWindow<SymbolWindow>();
            window.titleContent.text = "SymbolWindow";
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
                var optionsRect = GUILayoutUtility.GetRect(0, 20, GUILayout.ExpandWidth(false));
                if (GUILayout.Button("Options", EditorStyles.toolbarDropDown, GUILayout.ExpandWidth(false)))
                {
                    var options = new GenericMenu();
                    options.AddItem(new GUIContent("Show in Explorer"), false, OnShowInExplorerClicked);
                    options.AddItem(new GUIContent("Clear All"), false, OnClearAllClicked);
                    options.DropDown(optionsRect);
                }
            }
            EditorGUILayout.EndHorizontal();
            var lastTargetGroup = targetGroup;
            targetGroup = (BuildTargetGroup) EditorGUILayout.EnumPopup("targetGroup", targetGroup);
            foreach (var symbol in symbols)
            {
                symbol.enabled = EditorGUILayout.Toggle(symbol.content, symbol.enabled);
            }

            if (GUILayout.Button("Refresh") || lastTargetGroup != targetGroup)
            {
                OnRefreshClicked();
            }

            if (GUILayout.Button("Save"))
            {
                OnSaveClicked();
            }
        }

        private void SwitchPlatform()
        {
            targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        }

        private void CollectDefineSymbol()
        {
            symbols.Clear();
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
                    var symbol = new Symbol
                    {
                        content = line,
                        enabled = activeDefineSymbols.Contains(line)
                    };

                    symbols.Add(symbol);
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
            foreach (var symbol in symbols)
            {
                if (symbol.enabled)
                {
                    sb.AppendFormat("{0};", symbol.content);
                }
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, sb.ToString());
        }

        private void ClearDefineSymbol()
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, "");
        }

        private void OnShowInExplorerClicked()
        {
            var fullPath = Path.Combine(Application.dataPath, DefineSymbolPath);
            EditorUtility.RevealInFinder(fullPath);
        }

        private void OnClearAllClicked()
        {
            ClearDefineSymbol();
        }

        private void OnRefreshClicked()
        {
            CollectDefineSymbol();
        }

        private void OnSaveClicked()
        {
            SaveDefineSymbol();
        }

        private class Symbol
        {
            public string content;
            public bool enabled;
        }
    }
}