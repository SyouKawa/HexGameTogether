using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelDataReader;
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
    public string Discription = "技能效果";
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
        path = Application.dataPath + "\\Resources\\ExcelData\\GameData.xlsx";
        DataSet set;

        using(var stream = File.Open(path, FileMode.Open, FileAccess.Read)) {
            //读取Excel
            using(var reader = ExcelReaderFactory.CreateReader(stream)) {
                //将其转化为DataSet
                set = reader.AsDataSet();
            }
        }

        foreach (DataTable table in set.Tables) {
            //将DataTable分类保存,其中Excel分页的命名就是TableName
            //DataColunm和DataRow分别对应Excel的行列,具体请查阅.net中的DataTable数据类型
            Data.Add(table.TableName, table);
        }

        Log.Debug("本次加载已完成,共加载{0}张表", set.Tables.Count);
    }

    public List<Skill> LoadSkill() {
        DataTable table = Data["Skill"];
        int colCount = table.Columns.Count;
        DataRow firstRow = table.Rows[0];
        //使用首行创建TableParser,将这张表和一个数据类型绑定
        TableParser<Skill> skillParser = TableParser<Skill>.Create(firstRow);

        List<Skill> result = new List<Skill>();
        //读取其他行的信息到数据中
        for(int i = 1; i < colCount; i++) {
            result.Add(skillParser.Parse(table.Rows[i]));
        }
        return result;
    }

    /// <summary>
    /// 单个表解析器
    /// </summary>
    public class TableParser<T> where T : class, new() {
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
            foreach (object obj in header.ItemArray) {
                string str = (string)obj;
                string[] ss = str.Split('#');
                if(ss.Length < 2) {
                    Log.Error("解析表头错误{0}", str);
                    return null;
                }
                parser.headerData.Add(ss[0], TypeDic[ss[1]]);
            }
            return parser;
        }

        /// <summary>
        /// dataRow -> obj
        /// </summary>
        public T Parse(DataRow row) {
            T obj = new T();
            SetValue(obj, row);
            return obj;
        }

        //将data中的数据赋值给对象 可参考Global中的反射
        private void SetValue(T obj,DataRow data){

        }
    }
}