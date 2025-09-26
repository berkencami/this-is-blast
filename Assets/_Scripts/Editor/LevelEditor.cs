#if UNITY_EDITOR
using ThisIsBlast.LevelSystem;
using ThisIsBlast.Utility;
using ThisIsBlast.Enums;
using UnityEngine;
using UnityEditor;

namespace ThisIsBlast.LevelEditor
{
    [CustomEditor(typeof(LevelData))]
    public class LevelEditor : Editor
    {
        private BlockType _selectedType = BlockType.Red;

        private SerializedProperty blockColumns;
        private SerializedProperty shooters;
        private SerializedProperty shooterColumns;

        private static Vector2 _blocksScroll;
        private static int _cellSize = 24;
        private static int _targetLength = 10;

        private enum ToolMode
        {
            Paint,
            Erase
        }

        private static ToolMode _tool = ToolMode.Paint;

        private static int _tabIndex = 0; // 0: Blocks, 1: Shooters

        private void OnEnable()
        {
            blockColumns = serializedObject.FindProperty("_blockColumns");
            shooters = serializedObject.FindProperty("_shooters");
            shooterColumns = serializedObject.FindProperty("_shooterColumns");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space(10);
            _tabIndex = GUILayout.Toolbar(_tabIndex, new[] { "Blocks", "Shooters" });

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Palette");
            EditorGUILayout.Space(8);
            EditorGUILayout.BeginHorizontal();
            foreach (BlockType bt in System.Enum.GetValues(typeof(BlockType)))
            {
                if (bt == BlockType.None) continue;
                var originalColor = GUI.backgroundColor;
                GUI.backgroundColor = ColorMap.Get(bt);
                if (GUILayout.Button("", GUILayout.Width(28), GUILayout.Height(28)))
                {
                    _selectedType = bt;
                }

                GUI.backgroundColor = originalColor;
            }

            EditorGUILayout.LabelField($"Selected: {_selectedType}", GUILayout.Width(120));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10);

            switch (_tabIndex)
            {
                case 0:
                    DrawBlocksEditor();
                    break;
                case 1:
                    DrawShootersGrid();
                    break;
            }


            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save Level", GUILayout.Width(100), GUILayout.Height(35)))
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }

            if (GUILayout.Button("Reset Data", GUILayout.Width(100), GUILayout.Height(35)))
            {
                if (EditorUtility.DisplayDialog("Reset Level Data",
                        "This will clear all data and reinitialize arrays. Continue?", "Yes", "No"))
                {
                    ResetLevelData();
                }
            }

            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        private void EnsureBlockColumns()
        {
            if (blockColumns.arraySize != 10)
            {
                blockColumns.arraySize = 10;
            }

            for (var i = 0; i < 10; i++)
            {
                var col = blockColumns.GetArrayElementAtIndex(i);
                var items = col.FindPropertyRelative("items");
                if (items == null) continue;
                if (items.arraySize < 0) items.arraySize = 0;
            }
        }

        private void DrawBlocksEditor()
        {
            EnsureBlockColumns();

            EditorGUILayout.LabelField("Columns: 10 (fixed)");
            EditorGUILayout.Space(5);

            // Removed Add Count UI

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Tool", GUILayout.Width(30));
            _tool = (ToolMode)GUILayout.Toolbar((int)_tool, new[] { "Paint", "Erase" }, GUILayout.Width(150));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Clear All", GUILayout.Width(100)))
            {
                if (EditorUtility.DisplayDialog("Clear All Columns", "Delete all blocks in all 10 columns?", "Yes",
                        "No"))
                {
                    for (var c = 0; c < 10; c++)
                    {
                        var items = blockColumns.GetArrayElementAtIndex(c).FindPropertyRelative("items");
                        items.arraySize = 0;
                    }

                    EditorUtility.SetDirty(target);
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(6);
            EditorGUILayout.BeginHorizontal();
            _targetLength = EditorGUILayout.IntSlider("Column Length", _targetLength, 0, 50);
            if (GUILayout.Button("Apply", GUILayout.Width(60)))
            {
                for (var c = 0; c < 10; c++)
                {
                    var items = blockColumns.GetArrayElementAtIndex(c).FindPropertyRelative("items");
                    var cur = items.arraySize;
                    if (cur < _targetLength)
                    {
                        var fillType = _selectedType;
                        while (items.arraySize < _targetLength)
                        {
                            items.arraySize++;
                            items.GetArrayElementAtIndex(items.arraySize - 1).intValue = (int)fillType;
                        }
                    }
                    else if (cur > _targetLength)
                    {
                        items.arraySize = _targetLength;
                    }
                }

                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.EndHorizontal();

            // Selection tools removed

            EditorGUILayout.Space(5);

            // Compute max length
            var maxLen = 0;
            var lenCache = new int[10];
            for (var c = 0; c < 10; c++)
            {
                var items = blockColumns.GetArrayElementAtIndex(c).FindPropertyRelative("items");
                var len = items.arraySize;
                lenCache[c] = len;
                if (len > maxLen) maxLen = len;
            }

            // Header: lengths
            EditorGUILayout.BeginHorizontal();
            for (var c = 0; c < 10; c++)
            {
                EditorGUILayout.BeginVertical(GUILayout.Width(_cellSize + 14));
                EditorGUILayout.LabelField($"{lenCache[c]}", GUILayout.Width(_cellSize + 8));
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndHorizontal();

            // Grid scroll (draw all rows; keep viewport height constant so scrollbar works reliably)
            var viewportHeight = Mathf.Clamp(_cellSize * 16, 120, 600);
            _blocksScroll = EditorGUILayout.BeginScrollView(_blocksScroll, GUILayout.Height(viewportHeight));

            for (var r = 0; r < maxLen; r++)
            {
                EditorGUILayout.BeginHorizontal();
                for (var c = 0; c < 10; c++)
                {
                    var items = blockColumns.GetArrayElementAtIndex(c).FindPropertyRelative("items");
                    var has = r < items.arraySize;
                    var bt = has ? (BlockType)items.GetArrayElementAtIndex(r).intValue : BlockType.None;
                    var color = has ? ColorMap.Get(bt) : new Color(0.25f, 0.25f, 0.25f, 1f);
                    var orig = GUI.backgroundColor;
                    GUI.backgroundColor = color;
                    if (GUILayout.Button("", GUILayout.Width(_cellSize), GUILayout.Height(_cellSize)))
                    {
                        if (_tool == ToolMode.Paint)
                        {
                            var selType = _selectedType;
                            if (has)
                            {
                                var prop = items.GetArrayElementAtIndex(r);
                                prop.intValue = (int)selType;
                                Debug.Log($"Paint: Set column {c}, row {r} to {selType} (direct int: {(int)selType})");
                            }
                            else
                            {
                                // Extend array to include the clicked position
                                while (items.arraySize <= r)
                                {
                                    items.arraySize++;
                                    var newProp = items.GetArrayElementAtIndex(items.arraySize - 1);
                                    newProp.intValue = (int)BlockType.None;
                                }

                                var targetProp = items.GetArrayElementAtIndex(r);
                                targetProp.intValue = (int)selType;
                                Debug.Log(
                                    $"Paint (new): Set column {c}, row {r} to {selType} (direct int: {(int)selType})");
                            }

                            EditorUtility.SetDirty(target);
                            serializedObject.ApplyModifiedProperties();
                        }
                        else if (_tool == ToolMode.Erase)
                        {
                            if (has)
                            {
                                // Remove at specific position and shift remaining items
                                for (var k = r; k < items.arraySize - 1; k++)
                                {
                                    items.GetArrayElementAtIndex(k).intValue =
                                        items.GetArrayElementAtIndex(k + 1).intValue;
                                }

                                items.arraySize--;
                                EditorUtility.SetDirty(target);
                            }
                        }
                    }

                    GUI.backgroundColor = orig;
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawShootersGrid()
        {
            EditorGUILayout.LabelField("Shooters Configuration");
            EditorGUILayout.Space(6);

            // Shooter columns slider
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Columns", GUILayout.Width(60));
            var newCols = EditorGUILayout.IntSlider(shooterColumns.intValue, 1, 5);
            if (newCols != shooterColumns.intValue)
            {
                shooterColumns.intValue = newCols;

                var newSize = newCols;

                if (shooters.arraySize > newSize)
                {
                    shooters.arraySize = newSize;
                }
                else if (shooters.arraySize < newSize)
                {
                    while (shooters.arraySize < newSize)
                    {
                        shooters.arraySize++;
                        var elem = shooters.GetArrayElementAtIndex(shooters.arraySize - 1);
                        elem.FindPropertyRelative("color").intValue = (int)BlockType.None;
                        elem.FindPropertyRelative("count").intValue = 0;
                    }
                }

                EditorUtility.SetDirty(target);
                serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(4);

            var cols = shooterColumns.intValue;

            // Calculate rows from current shooter count
            var currentRows = Mathf.Max(1, Mathf.CeilToInt(shooters.arraySize / (float)cols));

            // Row management buttons
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+ Row", GUILayout.Width(70)))
            {
                for (int i = 0; i < cols; i++)
                {
                    shooters.arraySize++;
                    var elem = shooters.GetArrayElementAtIndex(shooters.arraySize - 1);
                    elem.FindPropertyRelative("color").intValue = (int)BlockType.None;
                    elem.FindPropertyRelative("count").intValue = 0;
                }

                EditorUtility.SetDirty(target);
            }

            if (GUILayout.Button("- Row", GUILayout.Width(70)))
            {
                if (currentRows > 1 && shooters.arraySize >= cols)
                {
                    shooters.arraySize = Mathf.Max(0, shooters.arraySize - cols);
                    EditorUtility.SetDirty(target);
                }
            }

            if (GUILayout.Button("Clear All", GUILayout.Width(80)))
            {
                if (EditorUtility.DisplayDialog("Clear Shooters", "Remove all shooters?", "Yes", "No"))
                {
                    shooters.arraySize = 0;
                    EditorUtility.SetDirty(target);
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField($"Rows: {currentRows}", EditorStyles.miniLabel);

            // Ensure we have enough elements for the grid
            var targetSize = currentRows * cols;
            while (shooters.arraySize < targetSize)
            {
                shooters.arraySize++;
                var elem = shooters.GetArrayElementAtIndex(shooters.arraySize - 1);
                elem.FindPropertyRelative("color").intValue = (int)BlockType.None;
                elem.FindPropertyRelative("count").intValue = 0;
            }

            EditorGUILayout.Space(10);

            // Draw shooter grid
            for (var r = 0; r < currentRows; r++)
            {
                EditorGUILayout.BeginHorizontal();
                for (var c = 0; c < cols; c++)
                {
                    var index = r * cols + c;
                    if (index >= shooters.arraySize) break;

                    var cell = shooters.GetArrayElementAtIndex(index);
                    var colorProp = cell.FindPropertyRelative("color");
                    var countProp = cell.FindPropertyRelative("count");

                    var bt = (BlockType)colorProp.intValue;
                    var color = bt == BlockType.None ? Color.gray : ColorMap.Get(bt);

                    var original = GUI.backgroundColor;
                    GUI.backgroundColor = color;
                    if (GUILayout.Button("", GUILayout.Width(35), GUILayout.Height(35)))
                    {
                        colorProp.intValue = (int)_selectedType;
                        if (colorProp.intValue == (int)BlockType.None)
                            countProp.intValue = 0;
                        else if (countProp.intValue < 1)
                            countProp.intValue = 1;

                        Debug.Log(
                            $"Shooter {index}: Set color to {_selectedType} (direct int: {(int)_selectedType}), count: {countProp.intValue}");
                        EditorUtility.SetDirty(target);
                        serializedObject.ApplyModifiedProperties();
                    }

                    GUI.backgroundColor = original;

                    EditorGUILayout.BeginVertical(GUILayout.Width(50));
                    EditorGUI.BeginDisabledGroup(colorProp.intValue == (int)BlockType.None);
                    var newCount = EditorGUILayout.IntField(countProp.intValue, GUILayout.Width(45));
                    if (newCount != countProp.intValue)
                    {
                        countProp.intValue = Mathf.Max(1, newCount);
                        EditorUtility.SetDirty(target);
                        serializedObject.ApplyModifiedProperties();
                    }

                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.EndHorizontal();
            }
        }


        private void ResetLevelData()
        {
            blockColumns.arraySize = 10;
            for (var i = 0; i < 10; i++)
            {
                var col = blockColumns.GetArrayElementAtIndex(i);
                var items = col.FindPropertyRelative("items");
                items.arraySize = 0;
            }

            // Reset shooters
            shooters.arraySize = 0;

            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();

            Debug.Log("Level data reset successfully!");
        }
    }
}
#endif