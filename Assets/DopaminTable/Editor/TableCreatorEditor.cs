#if UNITY_EDITOR
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class TableCreatorEditor : EditorWindow
{
    [MenuItem("Tables/Create Table")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(TableCreatorEditor), true, "테이블 생성/수정");
    }


    string _TableName;
    int _Version;
    string _TableContent;

    private void OnGUI()
    {        
        GUILayout.MinWidth(800);

        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();

        GUIStyle labelStyle = EditorStyles.boldLabel;
        labelStyle.alignment = TextAnchor.MiddleLeft;
        labelStyle.fontSize = 14;

        GUILayout.Label("테이블명", labelStyle, new GUILayoutOption[] { GUILayout.Width(80) });

        _TableName = EditorGUILayout.TextField(_TableName, new GUILayoutOption[] { GUILayout.Width(800), GUILayout.Height(20) });

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("버전", labelStyle, new GUILayoutOption[] { GUILayout.Width(80) });

        _Version = EditorGUILayout.IntField(_Version, new GUILayoutOption[] { GUILayout.Width(800), GUILayout.Height(20) });

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.Label("데이터", EditorStyles.boldLabel);
      
        GUILayout.Space(10);

        _TableContent = EditorGUILayout.TextArea(_TableContent, new GUILayoutOption[]
        {
            GUILayout.MinHeight(50),
            GUILayout.MaxHeight(300),
        });

        GUILayout.Space(20);

        EditorGUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.ExpandWidth(true) });

        if(GUILayout.Button("생성", new GUILayoutOption[] { GUILayout.Width(250), GUILayout.Height(60), GUILayout.ExpandWidth(true) }))
        {
            _AssetReload = false;

            CreateTable();
        }

        if (GUILayout.Button("수정", new GUILayoutOption[] { GUILayout.Width(250), GUILayout.Height(60), GUILayout.ExpandWidth(true) }))
        {
            _AssetReload = false;

            CreateJsonFile();
        }

        EditorGUILayout.EndHorizontal();
        
        GUILayout.Space(20);

    }

    string[] _Rows;
    string[] _Keys;
    bool _AssetReload = false;


    void OnEnable()
    {
        AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
    }

    private void OnDisable()
    {
        AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
    }

    private void OnAfterAssemblyReload()
    {
        if(_AssetReload)
        {
            CreateJsonFile();
            RecreateTablesClass();

            _AssetReload = false;
        }
    }

    private void CreateTable()
    {
        CreateRowDatas();

        CreateTableClass();

        //클래스 생성 후 _AssetReload가 true가 되며 AssetDataBase.Refresh() 완료 후에 OnAfterAssemblyReload()로 넘어간다.
    }

    private void CreateRowDatas()
    {
        _TableContent = _TableContent.Replace("\r", "");

        //테이블 string값 한줄씩 분리하기
        _Rows = _TableContent.Split("\n");

        //키값 나누기
        _Keys = _Rows[1].Split("\t");
    }

    private void CreateTableClass()
    {
        //타입 값 가져오기
        string[] types = _Rows[0].Split("\t");



        bool isContainGenericCollection = false;

        foreach(string type in types)
        {
            if(type.Length > 6)
            {
                if(type.Substring(0, 5) == "List<")
                {
                    isContainGenericCollection = true;
                    break;
                }
            }
        }


        StringBuilder classSb = new StringBuilder();

        classSb.AppendLine("using System;");
        classSb.AppendLine("using UnityEngine;"); //일일이 검출하기 귀찮으므로 그냥 넣기

        if (isContainGenericCollection)
        {
            classSb.AppendLine("using System.Collections.Generic;");
        }
        classSb.AppendLine("");


        classSb.AppendLine("namespace DopaminTable");
        classSb.AppendLine("{");
        
        //Data struct 생성 시작
        classSb.AppendLine("    [Serializable]");
        classSb.AppendLine($"    public struct Table_{_TableName}Data");
        classSb.AppendLine("    {");

        int keyCount = _Keys.Length;
        for(int i = 0; i < keyCount; ++i)
        {
            classSb.AppendLine($"        public {types[i]} {_Keys[i]} {{  get; set; }}");
        }

        //Data struct 생성 끝
        classSb.AppendLine("    }");

        classSb.AppendLine("");

        //Table class 생성 시작
        classSb.AppendLine($"    public class Table_{_TableName} : Table<Table_{_TableName}Data>");
        classSb.AppendLine("    {");
        classSb.AppendLine($"        public ushort Version {{ get; private set; }} = {_Version};");
        classSb.AppendLine(""); 
        classSb.AppendLine($"        public Table_{_TableName}Data GetData(uint id)");
        classSb.AppendLine("        {");
        classSb.AppendLine("             return Data.ContainsKey(id) ? Data[id] : default;");
        classSb.AppendLine("        }");
        classSb.AppendLine("");
        classSb.AppendLine($"        public static uint CreateCode(ushort middle, ushort last)");
        classSb.AppendLine("        {");
        classSb.AppendLine("            if (middle > 999) middle = 999;");
        classSb.AppendLine("            if (last > 999) last = 999;");
        classSb.AppendLine("");
        classSb.AppendLine($"            return (uint)({int.Parse(_TableName.Substring(_TableName.Length - 3) + "000000")} + (middle * 1000) + last);");
        classSb.AppendLine("        }");
        classSb.AppendLine("    }");
        
        //namespace end
        classSb.AppendLine("}");


        //using(var save = File.WriteAllText($"{Application.dataPath}/DopaminTable/Scripts/Table/Table_{_TableName}.json", classSb.ToString(), Encoding.UTF8))
        using (var saveFile = File.CreateText($"{Application.dataPath}/DopaminTable/Scripts/Table/Table_{_TableName}.cs"))
        {
            saveFile.Write(classSb.ToString());
        }

        _AssetReload = true;
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

        EditorUtility.DisplayDialog("Class!", "저장 성공!", "확인");
    }

    private void CreateJsonFile()
    {
        if (_AssetReload == false)
        {
            CreateRowDatas();
        }

        Type tableClassType = Type.GetType($"DopaminTable.Table_{_TableName},DopaminTable");
        if (tableClassType == null)
        {
            EditorUtility.DisplayDialog("클래스 파일 없음", "생성 버튼을 먼저 눌러주세요.", "확인");
            return;
        }

        Type tableDataType = Type.GetType($"DopaminTable.Table_{_TableName}Data,DopaminTable");
        if (tableDataType == null)
        {
            EditorUtility.DisplayDialog("데이터 클래스 파일 없음", "생성 버튼을 먼저 눌러주세요.", "확인");
            return;
        }


        List<object> obj = new List<object>();

        string[] types = _Rows[0].Split("\t");
        
        int rowLength = _Rows.Length;
        int typeCount = types.Length;

        for(int a = 2; a < rowLength; ++a)
        {
            var dataClass = Activator.CreateInstance(tableDataType);

            string[] values = _Rows[a].Split("\t");

            for (int j = 0; j < typeCount; ++j)
            {
                var property = tableDataType.GetProperty(_Keys[j]);

                if (property != null)
                {
                    //List 형식인가?
                    if (property.PropertyType.IsGenericType)
                    {
                        string[] genericValues;

                        string type = types[j];
                        Type listGenericType = property.PropertyType.GetGenericArguments()[0]; //List의 T를 받아옴.

                        if (listGenericType == typeof(Vector3Int) || listGenericType == typeof(Vector3) || listGenericType == typeof(Vector2Int) || listGenericType == typeof(Vector2))
                        {
                            //(0,0,0),(0,0,0) 형식을 ), 기준으로 분리. 인자값 가져오기.
                            genericValues = values[j].Split("),");
                            genericValues[^1] = genericValues[^1].Substring(0, genericValues[^1].Length - 1);
                        }
                        else
                        {
                            genericValues = values[j].Split(",");
                        }

                        Array v = Array.CreateInstance(listGenericType, genericValues.Length);
                        for (int i = 0; i < genericValues.Length; ++i)
                        {
                            v.SetValue(Convert.ChangeType(genericValues[i], listGenericType), i);
                        }

                        Type listType = typeof(List<>).MakeGenericType(listGenericType);

                        object list = Activator.CreateInstance(listType, new object[] { v });
                        try
                        {
                            property.SetValue(dataClass, Convert.ChangeType(list, property.PropertyType));
                        }
                        catch (Exception)
                        {
                            EditorUtility.DisplayDialog("오류 발생", $"키값 {property.Name}의 {a}줄 값 : {values[j]}", "확인");
                            throw new Exception($"오류 발생. 키값 {property.Name}의 {a}번째 줄 값 : {values[j]}");
                        }
                    }
                    else
                    {
                        try
                        {
                            property.SetValue(dataClass, Convert.ChangeType(values[j], property.PropertyType));
                        }
                        catch(Exception)
                        {
                            EditorUtility.DisplayDialog("오류 발생", $"키값 {property.Name}의 {a}줄 값 : {values[j]}", "확인");
                            throw new Exception($"오류 발생. 키값 {property.Name}의 {a}번째 줄 값 : {values[j]}");
                        }
                    }
                }
            }

            obj.Add(dataClass);
        }
        string json = JsonConvert.SerializeObject(obj);

        File.WriteAllText($"{Application.dataPath}/DopaminTable/Datas/Table_{_TableName}.json", json, Encoding.UTF8);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("JSON!", "저장 성공!", "확인");
    }


    string _TablesDesc = 
        "using System;" +
        "\r\n" +
        "\r\nnamespace DopaminTable" +
        "\r\n{" +
        "\r\n    public class Tables" +
        "\r\n    {" +
        "\r\n        static Tables _Instance;" +
        "\r\n" +
        "\r\n        int _TotalTableCount;" +
        "\r\n        int _LoadTableCount;" +
        "\r\n" +
        "\r\n        Action<int, int> _ProgressCallback;" +
        "\r\n" +
        "\r\n" +
        "\r\n        //TABLE LIST - START" +

        "@@REPLACE@@" + //Properties

        "\r\n        //TABLE LIST - END" +
        "\r\n" +
        "\r\n        public static void Create()" +
        "\r\n        {" +
        "\r\n            _Instance ??= new Tables();" +

        "\r\r@@REPLACE2@@" + //Create

        "\r\n        }" +
        "\r\n" +
        "\r\n" +
        "\r\n" +
        "\r\n        public static void Load(Action<int, int> progressCallback)" +
        "\r\n        {" +
        "\r\n            _Instance._ProgressCallback = progressCallback;" +
        "\r\n            _Instance._LoadTableCount = 0;" +
        "\r\n" +

        "\r\n@@REPLACE3@@" + //Load

        "\r\n        }" +
        "\r\n" +
        "\r\n        private void CompleteLoad()" +
        "\r\n        {" +
        "\r\n            ++_LoadTableCount;" +
        "\r\n" +
        "\r\n            _ProgressCallback?.Invoke(_LoadTableCount, _TotalTableCount);" +
        "\r\n        }" +
        "\r\n    }" +
        "\r\n}";

    private void RecreateTablesClass()
    {
        StringBuilder tableNameSb = new StringBuilder();
        StringBuilder tableCreateSb = new StringBuilder();
        StringBuilder tableLoadSb = new StringBuilder();

        int fileCount = 0;

        string[] fileNames = Directory.GetFiles($"{Application.dataPath}/DopaminTable/Scripts/Table");
        foreach ( string fileName in fileNames )
        {
            if (fileName.Substring(fileName.Length - 2) == "cs")
            {
                ++fileCount;

                string tableName = fileName.Split("/")[^1].Split('.')[0];
                string propertyName = tableName.Split("Table_")[1];

                tableNameSb.Append($"\r\r        {tableName} _{propertyName};\r\n        public static {tableName} {propertyName} => _Instance._{propertyName};");

                tableCreateSb.Append($"\n            _Instance._{propertyName} = new {tableName}();");

                tableLoadSb.Append($"\n            _Instance._{propertyName}.Load(_Instance.CompleteLoad);");
            }
        }

        //Debug.Log(_TablesDesc.Replace("@@REPLACE@@", tableNameSb.ToString()).Replace("@@REPLACE2@@", tableCreateSb.ToString()).Replace("@@REPLACE3@@", tableLoadSb.ToString()));

        using (var saveFile = File.CreateText($"{Application.dataPath}/DopaminTable/Scripts/Tables.cs"))
        {
            saveFile.Write(_TablesDesc.Replace("int _TotalTableCount;", $"int _TotalTableCount = {fileCount};").Replace("@@REPLACE@@", tableNameSb.ToString()).Replace("@@REPLACE2@@", tableCreateSb.ToString()).Replace("@@REPLACE3@@", tableLoadSb.ToString()));
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif