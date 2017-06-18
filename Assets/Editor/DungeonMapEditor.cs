using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

[CustomEditor(typeof(DungeonMap))]
class DungeonMapEditor : Editor
{
    DungeonMap script;

    public override void OnInspectorGUI()
    {
        script = DungeonMap.instance;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("tilesPerUnit"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("seed"));
        if (!script.generating)
        {
            if (GUILayout.Button("Execute Pipelines"))
                script.ExecutePipelines();
        }
        else
        {
            EditorGUILayout.LabelField("Generating...");
        }
        OnPipelinesGUI();
        serializedObject.ApplyModifiedProperties();
    }

    #region pipelines
    void OnPipelinesGUI()
    {
        for (int i = 0; i < script.pipelines.Count; ++i)
        {
            var p = script.pipelines[i];
            string name = p.GetType().Name;
            string propName = name.Replace(name[0], (char)(name[0] + 32));
            var prop = serializedObject.FindProperty(propName);
            EditorGUILayout.PropertyField(prop, true);
            if (prop.isExpanded)
            {
                ++EditorGUI.indentLevel;
                var method = GetType().GetMethod(
                    string.Format("On{0}GUI", name),
                    BindingFlags.Instance | BindingFlags.NonPublic);
                if (null != method)
                    method.Invoke(this, new object[] { p });
                --EditorGUI.indentLevel;
                if (GUILayout.Button("Execute"))
                    script.StartCoroutine(p.Execute(script));
            }
        }
    }

    void OnGenerateCellsGUI(GenerateCells p)
    {
        if (GUILayout.Button("Clear"))
            p.Clear(script);
    }

    string[] algoNames;
    Type[] algoTypes = new Type[] { typeof(LockAndMove), typeof(PushAway) };

    void OnSeparateCellsGUI(SeparateCells p)
    {
        if (null == algoNames)
            algoNames = Array.ConvertAll(algoTypes, v => v.Name);

        int index = Array.IndexOf(algoTypes, p.cellSeparator.GetType());
        index = EditorGUILayout.Popup("Algorithm", index, algoNames);
        if (index >= 0 && index < algoTypes.Length)
        {
            if (p.cellSeparator.GetType() != algoTypes[index])
                p.cellSeparator = (ICellSeparator)Activator.CreateInstance(algoTypes[index]);
        }

        if (p.cellSeparator is PushAway)
        {
            PushAway sep = (PushAway)p.cellSeparator;
            sep.xzDist = EditorGUILayout.FloatField("XZDist", sep.xzDist);
        }
        if (GUILayout.Button("OnePass"))
            p.cellSeparator.SeparateCellsOnePass(script.cells);
    }

    void OnMainCellsGUI(MainCells p)
    {
        if (GUILayout.Button("Clear"))
            p.Clear(script);
    }
    #endregion pipelines
}
