using System;
using System.Collections.Generic;
using ExcelDataReader;
using System.Reflection;
using System.IO;
using UnityEngine;
using System.Data;


/// <summary>
/// 测试用例
/// </summary>
public class Skill {
    #region Read From Excel
    public int ID = 1;
    public string Name = "技能名";
    public string Description = "技能效果";
    /// <summary>
    /// 代表强度,实际情况另说
    /// </summary>
    public int Power = 100;
    /// <summary>
    /// 蓝耗
    /// </summary>
    public int MPCost = 10;
    /// <summary>
    /// 默认cd
    /// </summary>
    public float CD = -1f;
    /// <summary>
    /// 默认施法时间,负数表示为瞬发技能
    /// </summary>
    public float CastInterval = 1.5f;
    /// <summary>
    /// 技能被释放时执行的脚本
    /// </summary>
    public string CastScript = "";
    #endregion
     
    //请忽视它
    public List<DiceSide> needSides;

    //打印函数
    public void print(){
        var str = Utils.FormatString("{0}:[\nDescrp:{1},\nPow:{2},\nMp:{3},\t]",Name,Description,Power,MPCost);
        Log.Debug(str);
    }
}

//TODO: 完成加载器功能, 并尽可能完备的添加好各种异常处理,格式错误,读取为空等等等等.
//改进LoadSkill函数,使这个类可以自动加载Excel中所有的表. 并将所有的表的数据进行保存.
//LoadSkill()只是举例,请保留额外添加的测试用例.
//整体流程: Excel -> ExcelDataReader -> DataSet -> DataTable -> List<T>

/// <summary>
/// Excel加载器
/// </summary>
public class ExcelReader : Manager<ExcelReader> {
    public string path;
    public Dictionary<string, DataTable> Data = new Dictionary<string, DataTable>();

    public override void Start(EventHelper helper) {
        helper.OnGameLoadEvent += LoadData;
    }

    public void LoadData() {
        path = Application.dataPath + Utils.GetPlatformPath();
        DataSet set;
        try{
            var stream = File.Open(path, FileMode.Open, FileAccess.Read);
            var reader = ExcelReaderFactory.CreateReader(stream);
            //将其转化为DataSet
            set = reader.AsDataSet();
            foreach (DataTable table in set.Tables) {
            //将DataTable分类保存,其中Excel分页的命名就是TableName
            //DataColunm和DataRow分别对应Excel的行列,具体请查阅.net中的DataTable数据类型
                if(table.Rows.Count<2){
                    Log.Warning("{0}表内容不完整（空表或只有表头），已跳过",table.TableName);
                    continue;
                }
                Data.Add(table.TableName, table);
            }
            Log.Debug("本次加载已完成,共读取{0}张表,共加载成功{1}张表",set.Tables.Count,Data.Count);
        }
        catch(SystemException e){
            Type errorType = e.GetType();
            if(errorType.Equals(typeof(FileNotFoundException))){
                Log.Error("无法找到加载文件:{0}\n",path);
                return ;
            }
            else{
                Log.Error("未知加载错误(请根据错误类型补充)\n");
                return;
            }
        }
        
        LoadTable<Skill>();
    }

    public List<T> LoadTable<T>() where T : class,new(){
        DataTable table = Data[typeof(T).Name];
        int rowCount = table.Rows.Count;
        //使用首行创建TableParser,将这张表和一个数据类型绑定
        DataRow firstRow = table.Rows[0];
        TableParser<T> classParser = TableParser<T>.Create(firstRow);

        List<T> result = new List<T>();
        //读取其他行的信息到数据中
        for(int i = 1; i < rowCount; i++) {
            //
            if(classParser.Parse(table.Rows[i]) == null){
                Log.Warning("第{0}行解析失败",i);
                continue;
            }
            //添加转换后的实例到列表并输出显示
            result.Add(classParser.Parse(table.Rows[i]));
            PrintResult<T>(result[i-1]);
        }
        return result;
    }

    public void PrintResult<T>(T res){
        FieldInfo[] infos = res.GetType().GetFields();
        var str = Utils.FormatString("ID:{0},Name:{1}",infos[0].GetValue(res),infos[1].GetValue(res));
        Log.Debug(str);
    }

    /// <summary>
    /// 单个表解析器
    /// </summary>
    public class TableParser<T> where T : class,new() {
        /// <summary>
        /// 将Excel中自己定义的类型转化为标准C#类型
        /// </summary>
        public static Dictionary<string, Type> TypeDic = new Dictionary<string, Type>()
        {
            {"int", typeof(int)},
            {"string", typeof(string)},
            {"float", typeof(float)},
        };

        public Dictionary<string, Type> headerData = new Dictionary<string, Type>();

        public int Count = 0;

        private TableParser() { }
        /// <summary>
        /// 通过首行来创建TableParser对象,创建失败返回null
        /// </summary>
        public static TableParser<T> Create(DataRow header) {
            TableParser<T> parser = new TableParser<T>();
            foreach (object obj in header.ItemArray) { // 表中
                string str = (string)obj;
                string[] ss = str.Split('\n');
                if(ss.Length < 3) {
                    Log.Error("解析表头错误{0}", str);
                    return null;
                }
                //按照从左到右的顺序，将表中每列所代表的变量及其类型添加到解析器
                parser.headerData.Add(ss[0], TypeDic[ss[1]]); // 本身存的就是typeof结果
            }
            return parser;
        }

        /// <summary>
        /// dataRow -> obj
        /// </summary>
        public T Parse(DataRow row) {
            T obj = new T();
            //如果赋值未成功，则直接返回空
            if(!SetValue(obj, row)){
                return null;
            }
            return obj; //返回值被加入结果列表
        }

        //将data中的数据赋值给对象 可参考Global中的反射
        private bool SetValue(T obj,DataRow data){
            FieldInfo[] vars = obj.GetType().GetFields();
            for(int i=0;i<data.ItemArray.Length;i++){
                //如果当前行有任何一格信息为空，则提示表格信息不完善，并直接跳出赋值
                if(data.ItemArray[i].Equals(DBNull.Value)){
                    Log.Warning("当前行信息不完善已跳过，请检查存储{0}类型表中{1}列信息是否都完善填写",typeof(T),vars[i].Name);
                    return false;
                }
                //如果信息完善则进行对应的类型转换
                try{
                    var value = Convert.ChangeType(data.ItemArray[i],headerData[vars[i].Name]);
                    vars[i].SetValue(obj,value);
                }catch(SystemException e){
                    Log.Warning("信息填写有误，请检查存储{0}类型表中{1}列信息的类型是否正确",typeof(T),vars[i].Name);
                    return false;
                }
            }
            return true;
        }
    }
}