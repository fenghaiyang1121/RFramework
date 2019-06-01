using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.IO;
using System.Xml;
using OfficeOpenXml;
using System.ComponentModel;

public class DataEditor
{
    public static string XmlPath = RealConfig.GetRealFram().m_XmlPath;
    public static string BinaryPath = RealConfig.GetRealFram().m_BinaryPath;
    public static string ScriptsPath = RealConfig.GetRealFram().m_ScriptsPath;
    public static string ExcelPath = Application.dataPath + "/../Data/Excel/";
    public static string RegPath = Application.dataPath + "/../Data/Reg/";

    [MenuItem("Assets/类转xml")]
    public static void AssetsClassToXml()
    {
        UnityEngine.Object[] objs = Selection.objects;
        for (int i = 0; i < objs.Length; i++)
        {
            EditorUtility.DisplayProgressBar("文件下的类转成xml", "正在扫描" + objs[i].name + "... ...", 1.0f / objs.Length * i);
            ClassToXml(objs[i].name);
        }
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("Assets/Xml转Binary")]
    public static void AssetsXmlToBinary()
    {
        UnityEngine.Object[] objs = Selection.objects;
        for (int i = 0; i < objs.Length; i++)
        {
            EditorUtility.DisplayProgressBar("文件下的xml转成二进制", "正在扫描" + objs[i].name + "... ...", 1.0f / objs.Length * i);
            XmlToBinary(objs[i].name);
        }
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("Assets/Xml转Excel")]
    public static void AssetsXmlToExcel()
    {
        UnityEngine.Object[] objs = Selection.objects;
        for (int i = 0; i < objs.Length; i++)
        {
            EditorUtility.DisplayProgressBar("文件下的xml转成Excel", "正在扫描" + objs[i].name + "... ...", 1.0f / objs.Length * i);
            XmlToExcel(objs[i].name);
        }
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("Tools/Xml/Xml转成二进制")]
    public static void AllXmlToBinary()
    {
        string path = Application.dataPath.Replace("Assets", "") + XmlPath;
        string[] filesPath = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
        for (int i = 0; i < filesPath.Length; i++)
        {
            EditorUtility.DisplayProgressBar("查找文件夹下面的Xml", "正在扫描" + filesPath[i] + "... ...", 1.0f / filesPath.Length * i);
            if (filesPath[i].EndsWith(".xml"))
            {
                string tempPath = filesPath[i].Substring(filesPath[i].LastIndexOf("/") + 1);
                tempPath = tempPath.Replace(".xml", "");
                XmlToBinary(tempPath);
            }
        }
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("Tools/Xml/Excel转Xml")]
    public static void AllExcelToXml()
    {
        string[] filePaths = Directory.GetFiles(RegPath, "*", SearchOption.AllDirectories);
        for (int i = 0; i < filePaths.Length; i++)
        {
            if (!filePaths[i].EndsWith(".xml"))
                continue;
            EditorUtility.DisplayProgressBar("查找文件夹下的类","正在扫描路径" + filePaths[i] + "... ...", 1.0f / filePaths.Length * i);
            string path = filePaths[i].Substring(filePaths[i].LastIndexOf("/") + 1);
            ExcelToXml(path.Replace(".xml", ""));
        }

        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("Tools/测试/测试读取xml")]
    public static void TestReadXml()
    {
        string xmlPath = Application.dataPath + "/../Data/Reg/MonsterData.xml";
        XmlReader reader = null;
        try
        {
            XmlDocument xml = new XmlDocument();
            reader = XmlReader.Create(xmlPath);
            xml.Load(reader);
            XmlNode xn = xml.SelectSingleNode("data");
            XmlElement xe = (XmlElement)xn;
            string className = xe.GetAttribute("name");
            string xmlName = xe.GetAttribute("to");
            string excelName = xe.GetAttribute("from");
            reader.Close();
            Debug.LogError(className + "  " + xmlName + "  " + excelName);
            foreach (XmlNode node in xe.ChildNodes)
            {
                XmlElement tempXe = (XmlElement)node;
                string name = tempXe.GetAttribute("name");
                string type = tempXe.GetAttribute("type");
                Debug.LogError(name + "  " + type);
                XmlNode listNode = tempXe.FirstChild;
                XmlElement listElement = (XmlElement)listNode;
                string listName = listElement.GetAttribute("name");
                string sheetName = listElement.GetAttribute("sheetname");
                string mainKey = listElement.GetAttribute("mainKey");
                Debug.LogError("list: " + listName + "  " + sheetName + "  " + mainKey);
                foreach (XmlNode nd in listElement.ChildNodes)
                {
                    XmlElement txe = (XmlElement)nd;
                    Debug.LogError(txe.GetAttribute("name") + "  " + txe.GetAttribute("col") + "  " + txe.GetAttribute("type"));
                }
            }
        }
        catch (Exception e)
        {
            if (reader != null)
            {
                reader.Close();
            }
            Debug.LogError(e);
        }
    }

    [MenuItem("Tools/测试/测试写入Excel")]
    public static void TestWriteExcel()
    {
        string xlsxPath = Application.dataPath + "/../Data/Excel/G怪物.xlsx";
        FileInfo xlsxFile = new FileInfo(xlsxPath);
        if (xlsxFile.Exists)
        {
            xlsxFile.Delete();
            xlsxFile = new FileInfo(xlsxPath);
        }
        using (ExcelPackage package = new ExcelPackage(xlsxFile))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("怪物配置");
            //worksheet.DefaultColWidth = 10;//sheet页面默认行宽度
            //worksheet.DefaultRowHeight = 30;//sheet页面默认列高度
            //worksheet.Cells.Style.WrapText = true;//设置所有单元格的自动换行
            //worksheet.InsertColumn();//插入行，从某一行开始插入多少行
            //worksheet.InsertRow();//插入列，从某一列开始插入多少列
            //worksheet.DeleteColumn();//删除行，从某一行开始删除多少行
            //worksheet.DeleteRow();//删除列，从某一列开始删除多少列
            //worksheet.Column(1).Width = 10;//设定第几行宽度
            //worksheet.Row(1).Height = 30;//设定第几列高度
            //worksheet.Column(1).Hidden = true;//设定第几行隐藏
            //worksheet.Row(1).Hidden = true;//设定第几列隐藏
            //worksheet.Column(1).Style.Locked = true;//设定第几行锁定
            //worksheet.Row(1).Style.Locked = true;//设定第几列锁定
            //worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;//设定所有单元格对齐方式

            worksheet.Cells.AutoFitColumns();
            ExcelRange range = worksheet.Cells[1, 1];
            range.Value = " 测试sadddddddddddddd\ndddddddddddddddddddasda";
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.None;
            //range.Style.Fill.BackgroundColor.SetColor();//设置单元格内背景颜色
            //range.Style.Font.Color.SetColor();//设置单元格内字体颜色
            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;//对齐方式 
            range.AutoFitColumns();
            range.Style.WrapText = true;

            ExcelRange range1 = worksheet.Cells[1, 2];
            range1.Value = " Siki学院Ocean老师!!!!!!";


            package.Save();
        }
    }

    [MenuItem("Tools/测试/测试已有类进行反射")]
    public static void TestReflection1()
    {
        TestInfo testInfo = new TestInfo()
        {
            Id = 2,
            Name = "测试反射",
            IsA = false,
            AllStrList = new List<string>(),
            AllTestInfoList = new List<TestInfoTwo>(),
        };

        testInfo.AllStrList.Add("测试1111");
        testInfo.AllStrList.Add("测试2222");
        testInfo.AllStrList.Add("测试3333");

        for (int i = 0; i < 3; i++)
        {
            TestInfoTwo test = new TestInfoTwo();
            test.Id = i + 1;
            test.Name = i + "name";
            testInfo.AllTestInfoList.Add(test);
        }

        GetMemberValue(testInfo, "Name");
        //object list = GetMemberValue(testInfo, "AllStrList");
        //int listCount = System.Convert.ToInt32(list.GetType().InvokeMember("get_Count", BindingFlags.Default | BindingFlags.InvokeMethod, null, list, new object[] { }));

        //for(int i = 0; i < listCount; i++)
        //{
        //    object item = list.GetType().InvokeMember("get_Item", BindingFlags.Default | BindingFlags.InvokeMethod, null, list, new object[] { i });
        //    Debug.LogError(item);
        //}

        object list = GetMemberValue(testInfo, "AllTestInfoList");
        int listCount = System.Convert.ToInt32(list.GetType().InvokeMember("get_Count", BindingFlags.Default | BindingFlags.InvokeMethod, null, list, new object[] { }));
        for (int i = 0; i < listCount; i++)
        {
            object item = list.GetType().InvokeMember("get_Item", BindingFlags.Default | BindingFlags.InvokeMethod, null, list, new object[] { i });

            object id = GetMemberValue(item, "Id");
            object name = GetMemberValue(item, "Name");
            Debug.LogError(id + " " + name);
        }
    }

    [MenuItem("Tools/测试/测试已有数据进行反射")]
    public static void TestReflection2()
    {
        object obj = CreateClass("TestInfo");
        PropertyInfo info = obj.GetType().GetProperty("Id");
        SetValue(info, obj, "21", "int");
        //info.SetValue(obj, System.Convert.ToInt32("20"));
        PropertyInfo nameInfo = obj.GetType().GetProperty("Name");
        SetValue(nameInfo, obj, "aqweddad", "string");
        //nameInfo.SetValue(obj, "huhiuhiuhi");
        PropertyInfo isInfo = obj.GetType().GetProperty("IsA");
        SetValue(isInfo, obj, "true", "bool");
        //isInfo.SetValue(obj, System.Convert.ToBoolean("false"));
        PropertyInfo heighInfo = obj.GetType().GetProperty("Heigh");
        SetValue(heighInfo, obj, "51.4", "float");
        //heighInfo.SetValue(obj, System.Convert.ToSingle("22.5"));
        PropertyInfo enumInfo = obj.GetType().GetProperty("TestType");
        SetValue(enumInfo, obj, "VAR1", "enum");
        //object infoValue = TypeDescriptor.GetConverter(enumInfo.PropertyType).ConvertFromInvariantString("VAR1");
        //enumInfo.SetValue(obj, infoValue);

        Type type = typeof(string);
        object list = CreateList(type);
        for (int i = 0; i < 3; i++)
        {
            object addItem = "测试填数据" + i;
            list.GetType().InvokeMember("Add", BindingFlags.Default | BindingFlags.InvokeMethod, null, list, new object[] { addItem });//调用list的add方法添加数据
        }

        obj.GetType().GetProperty("AllStrList").SetValue(obj, list);

        object twoList = CreateList(typeof(TestInfoTwo));
        for (int i = 0; i < 3; i++)
        {
            object addItem = CreateClass("TestInfoTwo");
            PropertyInfo itemIdInfo = addItem.GetType().GetProperty("Id");
            SetValue(itemIdInfo, addItem, "152" + i, "int");
            PropertyInfo itemNameInfo = addItem.GetType().GetProperty("Name");
            SetValue(itemNameInfo, addItem, "测试类" + i, "string");
            twoList.GetType().InvokeMember("Add", BindingFlags.Default | BindingFlags.InvokeMethod, null, twoList, new object[] { addItem });
        }
        obj.GetType().GetProperty("AllTestInfoList").SetValue(obj, twoList);

        TestInfo testInfo = (obj as TestInfo);
        //foreach (string str in testInfo.AllStrList)
        //{
        //    Debug.LogError(str);
        //}

        foreach (TestInfoTwo test in testInfo.AllTestInfoList)
        {
            Debug.LogError(test.Id + " " + test.Name);
        }
    }

    private static void ExcelToXml(string name)
    {
        string className = "";
        string xmlName = "";
        string excelName = "";
        //第一步，读取Reg文件，确定类的结构
        Dictionary<string, SheetClass> allSheetClassDic = ReadReg(name, ref excelName, ref xmlName, ref className);

        //第二步，读取excel里面的数据
        string excelPath = ExcelPath + excelName;
        Dictionary<string, SheetData> sheetDataDic = new Dictionary<string, SheetData>();
        try
        {
            using (FileStream stream = new FileStream(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (ExcelPackage package = new ExcelPackage(stream))
                {
                    ExcelWorksheets worksheetArray = package.Workbook.Worksheets;
                    for (int i = 0; i < worksheetArray.Count; i++)
                    {
                        SheetData sheetData = new SheetData();
                        ExcelWorksheet worksheet = worksheetArray[i + 1];
                        SheetClass sheetClass = allSheetClassDic[worksheet.Name];
                        int colCount = worksheet.Dimension.End.Column;
                        int rowCount = worksheet.Dimension.End.Row;

                        for (int n = 0; n < sheetClass.VarList.Count; n++)
                        {
                            sheetData.AllName.Add(sheetClass.VarList[n].Name);
                            sheetData.AllType.Add(sheetClass.VarList[n].Type);
                        }

                        for (int m = 1; m < rowCount; m++)
                        {
                            RowData rowData = new RowData();
                            int n = 0;
                            if (string.IsNullOrEmpty(sheetClass.SplitStr) && sheetClass.ParentVar != null
                                && !string.IsNullOrEmpty(sheetClass.ParentVar.Foregin))
                            {
                                rowData.ParnetVlue = worksheet.Cells[m + 1, 1].Value.ToString().Trim();
                                n = 1;
                            }
                            for (; n < colCount; n++)
                            {
                                ExcelRange range = worksheet.Cells[m + 1, n + 1];
                                string value = "";
                                if (range.Value != null)
                                {
                                    value = range.Value.ToString().Trim();
                                }
                                string colValue = worksheet.Cells[1, n + 1].Value.ToString().Trim();
                                rowData.RowDataDic.Add(GetNameFormCol(sheetClass.VarList, colValue), value);
                            }

                            sheetData.AllData.Add(rowData);
                        }
                        sheetDataDic.Add(worksheet.Name, sheetData);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }

        //根据类的结构，创建类，并且给每个变量赋值（从excel里读出来的值）
        object objClass = CreateClass(className);

        List<string> outKeyList = new List<string>();
        foreach (string str in allSheetClassDic.Keys)
        {
            SheetClass sheetClass = allSheetClassDic[str];
            if (sheetClass.Depth == 1)
            {
                outKeyList.Add(str);
            }
        }

        for (int i = 0; i < outKeyList.Count; i++)
        {
            ReadDataToClass(objClass, allSheetClassDic[outKeyList[i]], sheetDataDic[outKeyList[i]], allSheetClassDic, sheetDataDic, null);
        }

        BinarySerializeOpt.Xmlserialize(XmlPath + xmlName, objClass);
        //BinarySerializeOpt.BinarySerilize(BinaryPath + className + ".bytes", objClass);
        Debug.Log(excelName + "表导入unity完成！");
        AssetDatabase.Refresh();
    }

    private static void ReadDataToClass(object objClass, SheetClass sheetClass, SheetData sheetData, Dictionary<string, SheetClass> allSheetClassDic, Dictionary<string, SheetData> sheetDataDic, object keyValue)
    {
        object item = CreateClass(sheetClass.Name);//只是为了得到变量类型
        object list = CreateList(item.GetType());
        for (int i = 0; i < sheetData.AllData.Count; i++)
        {
            if (keyValue != null &&!string.IsNullOrEmpty(sheetData.AllData[i].ParnetVlue))
            {
                if (sheetData.AllData[i].ParnetVlue != keyValue.ToString())
                    continue;
            }
            object addItem = CreateClass(sheetClass.Name);
            for (int j = 0; j < sheetClass.VarList.Count; j++)
            {
                VarClass varClass = sheetClass.VarList[j];
                if (varClass.Type == "list" && string.IsNullOrEmpty(varClass.SplitStr))
                {
                    ReadDataToClass(addItem, allSheetClassDic[varClass.ListSheetName], sheetDataDic[varClass.ListSheetName], allSheetClassDic, sheetDataDic, GetMemberValue(addItem, sheetClass.MainKey));
                }
                else if (varClass.Type == "list")
                {
                    string value = sheetData.AllData[i].RowDataDic[sheetData.AllName[j]];
                    SetSplitClass(addItem, allSheetClassDic[varClass.ListSheetName], value);
                }
                else if (varClass.Type == "listStr" || varClass.Type == "listFloat" || varClass.Type == "listInt" || varClass.Type == "listBool")
                {
                    string value = sheetData.AllData[i].RowDataDic[sheetData.AllName[j]];
                    SetSplitBaseClass(addItem, varClass, value);
                }
                else
                {
                    string value = sheetData.AllData[i].RowDataDic[sheetData.AllName[j]];
                    if (string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(varClass.DeafultValue))
                    {
                        value = varClass.DeafultValue;
                    }
                    if (string.IsNullOrEmpty(value))
                    {
                        Debug.LogError("表格中有空数据，或者Reg文件未配置defaultValue！" + sheetData.AllName[j]);
                        continue;
                    }
                    SetValue(addItem.GetType().GetProperty(sheetData.AllName[j]), addItem, value, sheetData.AllType[j]);
                }
            }
            list.GetType().InvokeMember("Add", BindingFlags.Default | BindingFlags.InvokeMethod, null, list, new object[] { addItem });
        }
        objClass.GetType().GetProperty(sheetClass.ParentVar.Name).SetValue(objClass, list);
    }

    /// <summary>
    /// 自定义类List赋值
    /// </summary>
    /// <param name="objClass"></param>
    /// <param name="sheetClass"></param>
    /// <param name="value"></param>
    private static void SetSplitClass(object objClass, SheetClass sheetClass, string value)
    {
        object item = CreateClass(sheetClass.Name);
        object list = CreateList(item.GetType());
        if (string.IsNullOrEmpty(value))
        {
            Debug.Log("excel里面自定义list的列里有空值！" + sheetClass.Name);
            return;
        }
        else
        {
            string splitStr = sheetClass.ParentVar.SplitStr.Replace("\\n", "\n").Replace("\\r", "\r");
            string[] rowArray = value.Split(new string[] { splitStr }, StringSplitOptions.None);
            for (int i = 0; i < rowArray.Length; i++)
            {
                object addItem = CreateClass(sheetClass.Name);
                string[] valueList = rowArray[i].Trim().Split(new string[] { sheetClass.SplitStr }, StringSplitOptions.None);
                for (int j = 0; j < valueList.Length; j++)
                {
                    SetValue(addItem.GetType().GetProperty(sheetClass.VarList[j].Name), addItem, valueList[j].Trim(), sheetClass.VarList[j].Type);
                }
                list.GetType().InvokeMember("Add", BindingFlags.Default | BindingFlags.InvokeMethod, null, list, new object[] { addItem });
            }

        }
        objClass.GetType().GetProperty(sheetClass.ParentVar.Name).SetValue(objClass, list);
    }

    /// <summary>
    /// 基础List赋值
    /// </summary>
    /// <param name="objClass"></param>
    /// <param name="varClass"></param>
    /// <param name="value"></param>
    private static void SetSplitBaseClass(object objClass, VarClass varClass, string value)
    {
        Type type = null;
        if (varClass.Type == "listStr")
        {
            type = typeof(string);
        }
        else if (varClass.Type == "listFloat")
        {
            type = typeof(float);
        }
        else if (varClass.Type == "listInt")
        {
            type = typeof(int);
        }
        else if (varClass.Type == "listBool")
        {
            type = typeof(bool);
        }
        object list = CreateList(type);
        string[] rowArray = value.Split(new string[] { varClass.SplitStr }, StringSplitOptions.None);
        for (int i = 0; i < rowArray.Length; i++)
        {
            object addItem = rowArray[i].Trim();
            try
            {
                list.GetType().InvokeMember("Add", BindingFlags.Default | BindingFlags.InvokeMethod, null, list, new object[] { addItem });
            }
            catch
            {
                Debug.Log(varClass.ListSheetName + "  里 " + varClass.Name + "  列表添加失败！具体数值是：" + addItem);
            }
        }
        objClass.GetType().GetProperty(varClass.Name).SetValue(objClass, list);
    }

    /// <summary>
    /// 根据列名获取变量名
    /// </summary>
    /// <param name="varlist"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    private static string GetNameFormCol(List<VarClass> varlist, string col)
    {
        foreach (VarClass varClass in varlist)
        {
            if (varClass.Col == col)
                return varClass.Name;
        }
        return null;
    }

    private static void XmlToExcel(string name)
    {
        string className = "";
        string xmlName = "";
        string excelName = "";
        Dictionary<string, SheetClass> allSheetClassDic = ReadReg(name, ref excelName, ref xmlName, ref className);

        object data = GetObjFormXml(className);
        List<SheetClass> outSheetList = new List<SheetClass>();
        foreach (SheetClass sheetClass in allSheetClassDic.Values)
        {
            if (sheetClass.Depth == 1)
            {
                outSheetList.Add(sheetClass);
            }
        }

        Dictionary<string, SheetData> sheetDataDic = new Dictionary<string, SheetData>();
        for (int i = 0; i < outSheetList.Count; i++)
        {
            ReadData(data, outSheetList[i], allSheetClassDic, sheetDataDic, "");
        }

        string xlsxPath = ExcelPath + excelName;
        if (FileIsUsed(xlsxPath))
        {
            Debug.LogError("文件被占用，无法修改");
            return;
        }

        try
        {
            FileInfo xlsxFile = new FileInfo(xlsxPath);
            if (xlsxFile.Exists)
            {
                xlsxFile.Delete();
                xlsxFile = new FileInfo(xlsxPath);
            }
            using (ExcelPackage package = new ExcelPackage(xlsxFile))
            {
                foreach (string str in sheetDataDic.Keys)
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(str);
                    SheetData sheetData = sheetDataDic[str];
                    for (int i = 0; i < sheetData.AllName.Count; i++)
                    {
                        ExcelRange range = worksheet.Cells[1, i + 1];
                        range.Value = sheetData.AllName[i];
                        range.AutoFitColumns();
                    }

                    for (int i = 0; i < sheetData.AllData.Count; i++)
                    {
                        RowData rowData = sheetData.AllData[i];
                        for (int j = 0; j < sheetData.AllData[i].RowDataDic.Count; j++)
                        {
                            ExcelRange range = worksheet.Cells[i + 2, j + 1];
                            string vaule = rowData.RowDataDic[sheetData.AllName[j]];
                            range.Value = vaule;
                            range.AutoFitColumns();
                            if (vaule.Contains("\n") || vaule.Contains("\r\n"))
                            {
                                range.Style.WrapText = true;
                            }
                        }
                    }

                    worksheet.Cells.AutoFitColumns();
                }
                package.Save();
            }
        }
        catch(Exception e)
        {
            Debug.LogError(e);
            return;
        }
        Debug.Log("生成"+xlsxPath+"成功！！！");
    }

    private static Dictionary<string, SheetClass> ReadReg(string name, ref string excelName, ref string xmlName, ref string className)
    {
        string regPath = RegPath + name + ".xml";
        if (!File.Exists(regPath))
        {
            Debug.LogError("此数据不存在配置变化xml：" + name);
        }
        XmlDocument xml = new XmlDocument();
        XmlReader reader = XmlReader.Create(regPath);
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.IgnoreComments = true;//忽略xml里面的注释
        xml.Load(reader);
        XmlNode xn = xml.SelectSingleNode("data");
        XmlElement xe = (XmlElement)xn;
        className = xe.GetAttribute("name");
        xmlName = xe.GetAttribute("to");
        excelName = xe.GetAttribute("from");
        //储存所有变量的表
        Dictionary<string, SheetClass> allSheetClassDic = new Dictionary<string, SheetClass>();
        ReadXmlNode(xe, allSheetClassDic, 0);
        reader.Close();
        return allSheetClassDic;
    }

    /// <summary>
    /// 反序列化xml到类
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private static object GetObjFormXml(string name)
    {
        Type type = null;
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type tempType = asm.GetType(name);
            if (tempType != null)
            {
                type = tempType;
                break;
            }
        }
        if (type != null)
        {
            string xmlPath = XmlPath + name + ".xml";
            return BinarySerializeOpt.XmlDeserialize(xmlPath, type);
        }

        return null;
    }

    /// <summary>
    /// 递归读取类里面的数据
    /// </summary>
    /// <param name="data"></param>
    /// <param name="sheetClass"></param>
    /// <param name="allSheetClassDic"></param>
    /// <param name="sheetDataDic"></param>
    private static void ReadData(object data, SheetClass sheetClass, Dictionary<string, SheetClass> allSheetClassDic, Dictionary<string, SheetData> sheetDataDic, string mainKey)
    {
        List<VarClass> varList = sheetClass.VarList;
        VarClass varClass = sheetClass.ParentVar;
        object dataList = GetMemberValue(data, varClass.Name);

        int listCount = System.Convert.ToInt32(dataList.GetType().InvokeMember("get_Count", BindingFlags.Default | BindingFlags.InvokeMethod, null, dataList, new object[] { }));

        SheetData sheetData = new SheetData();

        if (!string.IsNullOrEmpty(varClass.Foregin))
        {
            sheetData.AllName.Add(varClass.Foregin);
            sheetData.AllType.Add(varClass.Type);
        }

        for (int i = 0; i < varList.Count; i++)
        {
            if (!string.IsNullOrEmpty(varList[i].Col))
            {
                sheetData.AllName.Add(varList[i].Col);
                sheetData.AllType.Add(varList[i].Type);
            }
        }

        string tempKey = mainKey;
        for (int i = 0; i < listCount; i++)
        {
            object item = dataList.GetType().InvokeMember("get_Item", BindingFlags.Default | BindingFlags.InvokeMethod, null, dataList, new object[] { i });

            RowData rowData = new RowData();
            if (!string.IsNullOrEmpty(varClass.Foregin) && !string.IsNullOrEmpty(tempKey))
            {
                rowData.RowDataDic.Add(varClass.Foregin, tempKey);
            }

            if (!string.IsNullOrEmpty(sheetClass.MainKey))
            {
                mainKey = GetMemberValue(item, sheetClass.MainKey).ToString();
            }

            for (int j = 0; j < varList.Count; j++)
            {
                if (varList[j].Type == "list" && string.IsNullOrEmpty(varList[j].SplitStr))
                {
                    SheetClass tempSheetClass = allSheetClassDic[varList[j].ListSheetName];
                    ReadData(item, tempSheetClass, allSheetClassDic, sheetDataDic, mainKey);
                }
                else if (varList[j].Type == "list")
                {
                    SheetClass tempSheetClass = allSheetClassDic[varList[j].ListSheetName];
                    string value = GetSplitStrList(item, varList[j], tempSheetClass);
                    rowData.RowDataDic.Add(varList[j].Col, value);
                }
                else if (varList[j].Type == "listStr" || varList[j].Type == "listFloat" || varList[j].Type == "listInt" || varList[j].Type == "listBool")
                {
                    string value = GetSpliteBaseList(item, varList[j]);
                    rowData.RowDataDic.Add(varList[j].Col, value);
                }
                else
                {
                    object value = GetMemberValue(item, varList[j].Name);
                    if (varList != null)
                    {
                        rowData.RowDataDic.Add(varList[j].Col, value.ToString());
                    }
                    else
                    {
                        Debug.LogError(varList[j].Name + "反射出来为空！");
                    }
                }
            }

            string key = varClass.ListSheetName;
            if (sheetDataDic.ContainsKey(key))
            {
                sheetDataDic[key].AllData.Add(rowData);
            }
            else
            {
                sheetData.AllData.Add(rowData);
                sheetDataDic.Add(key, sheetData);
            }
        }
    }

    /// <summary>
    /// 获取本身是一个类的列表，但是数据比较少；（没办法确定父级结构的）
    /// </summary>
    /// <returns></returns>
    private static string GetSplitStrList(object data, VarClass varClass, SheetClass sheetClass)
    {
        string split = varClass.SplitStr;
        string classSplit = sheetClass.SplitStr;
        string str = "";
        if (string.IsNullOrEmpty(split) || string.IsNullOrEmpty(classSplit))
        {
            Debug.LogError("类的列类分隔符或变量分隔符为空！！！");
            return str;
        }
        object dataList = GetMemberValue(data, varClass.Name);
        int listCount = System.Convert.ToInt32(dataList.GetType().InvokeMember("get_Count", BindingFlags.Default | BindingFlags.InvokeMethod, null, dataList, new object[] { }));
        for (int i = 0; i < listCount; i++)
        {
            object item = dataList.GetType().InvokeMember("get_Item", BindingFlags.Default | BindingFlags.InvokeMethod, null, dataList, new object[] { i });
            for (int j = 0; j < sheetClass.VarList.Count; j++)
            {
                object value = GetMemberValue(item, sheetClass.VarList[j].Name);
                str += value.ToString();
                if (j != sheetClass.VarList.Count - 1)
                {
                    str += classSplit.Replace("\\n", "\n").Replace("\\r", "\r");
                }
            }

            if (i != listCount - 1)
            {
                str += split.Replace("\\n", "\n").Replace("\\r", "\r");
            }
        }
        return str;
    }

    /// <summary>
    /// 获取基础List里面的所有值
    /// </summary>
    /// <returns></returns>
    private static string GetSpliteBaseList(object data, VarClass varClass)
    {
        string str = "";
        if (string.IsNullOrEmpty(varClass.SplitStr))
        {
            Debug.LogError("基础List的分隔符为空！");
            return str;
        }
        object dataList = GetMemberValue(data, varClass.Name);
        int listCount = System.Convert.ToInt32(dataList.GetType().InvokeMember("get_Count", BindingFlags.Default | BindingFlags.InvokeMethod, null, dataList, new object[] { }));

        for (int i = 0; i < listCount; i++)
        {
            object item = dataList.GetType().InvokeMember("get_Item", BindingFlags.Default | BindingFlags.InvokeMethod, null, dataList, new object[] { i });
            str += item.ToString();
            if (i != listCount - 1)
            {
                str += varClass.SplitStr.Replace("\\n", "\n").Replace("\\r", "\r");
            }
        }
        return str;
    }

    /// <summary>
    /// 递归读取配置
    /// </summary>
    /// <param name="xe"></param>
    private static void ReadXmlNode(XmlElement xmlElement, Dictionary<string, SheetClass> allSheetClassDic, int depth)
    {
        depth++;
        foreach (XmlNode node in xmlElement.ChildNodes)
        {
            XmlElement xe = (XmlElement)node;
            if (xe.GetAttribute("type") == "list")
            {
                XmlElement listEle = (XmlElement)node.FirstChild;

                VarClass parentVar = new VarClass()
                {
                    Name = xe.GetAttribute("name"),
                    Type = xe.GetAttribute("type"),
                    Col  = xe.GetAttribute("col"),
                    DeafultValue = xe.GetAttribute("defaultValue"),
                    Foregin = xe.GetAttribute("foregin"),
                    SplitStr = xe.GetAttribute("split"),
                };
                if (parentVar.Type == "list")
                {
                    parentVar.ListName = ((XmlElement)xe.FirstChild).GetAttribute("name");
                    parentVar.ListSheetName = ((XmlElement)xe.FirstChild).GetAttribute("sheetname");
                }

                SheetClass sheetClass = new SheetClass()
                {
                    Name = listEle.GetAttribute("name"),
                    SheetName = listEle.GetAttribute("sheetname"),
                    SplitStr = listEle.GetAttribute("split"),
                    MainKey = listEle.GetAttribute("mainKey"),
                    ParentVar = parentVar,
                    Depth = depth,
                };

                if (!string.IsNullOrEmpty(sheetClass.SheetName))
                {
                    if (!allSheetClassDic.ContainsKey(sheetClass.SheetName))
                    {
                        //获取该类下面所有变量
                        foreach (XmlNode insideNode in listEle.ChildNodes)
                        {
                            XmlElement insideXe = (XmlElement)insideNode;

                            VarClass varClass = new VarClass()
                            {
                                Name = insideXe.GetAttribute("name"),
                                Type = insideXe.GetAttribute("type"),
                                Col = insideXe.GetAttribute("col"),
                                DeafultValue = insideXe.GetAttribute("defaultValue"),
                                Foregin = insideXe.GetAttribute("foregin"),
                                SplitStr = insideXe.GetAttribute("split"),
                            };
                            if (varClass.Type == "list")
                            {
                                varClass.ListName = ((XmlElement)insideXe.FirstChild).GetAttribute("name");
                                varClass.ListSheetName = ((XmlElement)insideXe.FirstChild).GetAttribute("sheetname");
                            }

                            sheetClass.VarList.Add(varClass);
                        }
                        allSheetClassDic.Add(sheetClass.SheetName, sheetClass);
                    }
                }

                ReadXmlNode(listEle, allSheetClassDic, depth);
            }
        }
    }


    /// <summary>
    /// 判断文件是否被占用
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static bool FileIsUsed(string path)
    {
        bool result = false;

        if (!File.Exists(path))
        {
            result = false;
        }
        else
        {
            FileStream fileStream = null;
            try
            {
                fileStream = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);

                result = false;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                result = true;
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 反射new一個list
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static object CreateList(Type type)
    {
        Type listType = typeof(List<>);
        Type specType = listType.MakeGenericType(new System.Type[] { type });//确定list<>里面T的类型
        return Activator.CreateInstance(specType, new object[] { });//new出来这个list
    }

    /// <summary>
    /// 反射变量赋值
    /// </summary>
    /// <param name="info"></param>
    /// <param name="var"></param>
    /// <param name="value"></param>
    /// <param name="type"></param>
    private static void SetValue(PropertyInfo info, object var, string value, string type)
    {
        object val = (object)value;
        if (type == "int")
        {
            val = System.Convert.ToInt32(val);
        }
        else if (type == "bool")
        {
            val = System.Convert.ToBoolean(val);
        }
        else if (type == "float")
        {
            val = System.Convert.ToSingle(val);
        }
        else if (type == "enum")
        {
            val = TypeDescriptor.GetConverter(info.PropertyType).ConvertFromInvariantString(val.ToString());
        }
        info.SetValue(var, val);
    }

    /// <summary>
    /// 反射类里面的变量的具体数值
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="memeberName"></param>
    /// <param name="bindingFlags"></param>
    /// <returns></returns>
    private static object GetMemberValue(object obj, string memeberName, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
    {
        Type type = obj.GetType();
        MemberInfo[] members = type.GetMember(memeberName, bindingFlags);
        //while (members == null || members.Length == 0)
        //{
        //    type = type.BaseType;
        //    if (type == null)
        //        return;

        //    members = type.GetMember("Name",  BindingFlags.Public | BindingFlags.Default);
        //}

        switch (members[0].MemberType)
        {
            case MemberTypes.Field:
                return type.GetField(memeberName, bindingFlags).GetValue(obj);
            case MemberTypes.Property:
                return type.GetProperty(memeberName, bindingFlags).GetValue(obj);
            default:
                return null;
        }
    }

    /// <summary>
    /// 反射创建类的实例
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private static object CreateClass(string name)
    {
        object obj = null;
        Type type = null;
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type tempType = asm.GetType(name);
            if (tempType != null)
            {
                type = tempType;
                break;
            }
        }
        if (type != null)
        {
            obj = Activator.CreateInstance(type);
        }
        return obj;
    }

    /// <summary>
    /// xml转二进制
    /// </summary>
    /// <param name="name"></param>
    private static void XmlToBinary(string name)
    {
        if (string.IsNullOrEmpty(name))
            return;

        try
        {
            Type type = null;
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type tempType = asm.GetType(name);
                if (tempType != null)
                {
                    type = tempType;
                    break;
                }
            }
            if (type != null)
            {
                string xmlPath = XmlPath + name + ".xml";
                string binaryPath = BinaryPath + name + ".bytes";
                object obj = BinarySerializeOpt.XmlDeserialize(xmlPath, type);
                BinarySerializeOpt.BinarySerilize(binaryPath, obj);
                Debug.Log(name + "xml转二进制成功，二进制路径为:" + binaryPath);
            }
        }
        catch
        {
            Debug.LogError(name + "xml转二进制失败！");
        }
    }

    /// <summary>
    /// 实际的类转XML
    /// </summary>
    /// <param name="name"></param>
    private static void ClassToXml(string name)
    {
        if (string.IsNullOrEmpty(name))
            return;

        try
        {
            Type type = null;
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type tempType = asm.GetType(name);
                if (tempType != null)
                {
                    type = tempType;
                    break;
                }
            }
            if (type != null)
            {
                var temp = Activator.CreateInstance(type);
                if (temp is ExcelBase)
                {
                    (temp as ExcelBase).Construction();
                }
                string xmlPath = XmlPath + name + ".xml";
                BinarySerializeOpt.Xmlserialize(xmlPath, temp);
                Debug.Log(name + "类转xml成功，xml路径为:" + xmlPath);
            }
        }
        catch
        {
            Debug.LogError(name + "类转xml失败！");
        }
    }
}

public class SheetClass
{
    //所属父级Var变量
    public VarClass ParentVar { get; set; }
    //深度
    public int Depth { get; set; }
    //类名
    public string Name { get; set; }
    //类对应的sheet名
    public string SheetName { get; set; }
    //主键
    public string MainKey { get; set; }
    //分隔符
    public string SplitStr { get; set; }
    //所包含的变量
    public List<VarClass> VarList = new List<VarClass>();
}

public class VarClass
{
    //原类里面变量的名称
    public string Name { get; set; }
    //变量类型
    public string Type { get; set; }
    //变量对应的Excel里的列
    public string Col { get; set; }
    //变量的默认值
    public string DeafultValue { get; set; }
    //变量是list的话，外联部分列
    public string Foregin { get; set; }
    //分隔符
    public string SplitStr { get; set; }
    //如果自己是List，对应的list类名
    public string ListName { get; set; }
    //如果自己是list,对应的sheet名
    public string ListSheetName {get;set;}
}

public class SheetData
{
    public List<string> AllName = new List<string>();
    public List<string> AllType = new List<string>();
    public List<RowData> AllData = new List<RowData>();
}

public class RowData
{
    public string ParnetVlue = "";
    public Dictionary<string, string> RowDataDic = new Dictionary<string, string>();
}


public enum TestEnum
{
    None = 0,
    VAR1 = 1,
    TEST2 = 2,
}

public class TestInfo
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsA { get; set; }

    public float Heigh { get; set; }

    public TestEnum TestType { get; set; }

    public List<string> AllStrList { get; set; }
    public List<TestInfoTwo> AllTestInfoList { get; set; }
}

public class TestInfoTwo
{
    public int Id { get; set; }
    public string Name { get; set; }
}
