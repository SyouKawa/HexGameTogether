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
/// ��������
/// </summary>
public class Skill {
    #region Read From Excel
    public int ID = 1;
    public string Name = "������";
    public string Discription = "����Ч��";
    /// <summary>
    /// ����ǿ��,ʵ�������˵
    /// </summary>
    public int Power = 100;
    /// <summary>
    /// ����
    /// </summary>
    public int MPCost = 10;
    /// <summary>
    /// Ĭ��cd
    /// </summary>
    public float CD = -1f;
    /// <summary>
    /// Ĭ��ʩ��ʱ��,������ʾΪ˲������
    /// </summary>
    public float CastInterval = 1.5f;
    /// <summary>
    /// ���ܱ��ͷ�ʱִ�еĽű�
    /// </summary>
    public string CastScript = "";
    #endregion
     
    //�������
    public List<DiceSide> needSides;
}


//TODO: ��ɼ���������, ���������걸����Ӻø����쳣����,��ʽ����,��ȡΪ�յȵȵȵ�.
//�Ľ�LoadSkill����,ʹ���������Զ�����Excel�����еı�. �������еı�����ݽ��б���.
//LoadSkill()ֻ�Ǿ���,�뱣��������ӵĲ�������.
//��������: Excel -> ExcelDataReader -> DataSet -> DataTable -> List<T>

/// <summary>
/// Excel������
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
            //��ȡExcel
            using(var reader = ExcelReaderFactory.CreateReader(stream)) {
                //����ת��ΪDataSet
                set = reader.AsDataSet();
            }
        }

        foreach (DataTable table in set.Tables) {
            //��DataTable���ౣ��,����Excel��ҳ����������TableName
            //DataColunm��DataRow�ֱ��ӦExcel������,���������.net�е�DataTable��������
            Data.Add(table.TableName, table);
        }

        Log.Debug("���μ��������,������{0}�ű�", set.Tables.Count);
    }

    public List<Skill> LoadSkill() {
        DataTable table = Data["Skill"];
        int colCount = table.Columns.Count;
        DataRow firstRow = table.Rows[0];
        //ʹ�����д���TableParser,�����ű��һ���������Ͱ�
        TableParser<Skill> skillParser = TableParser<Skill>.Create(firstRow);

        List<Skill> result = new List<Skill>();
        //��ȡ�����е���Ϣ��������
        for(int i = 1; i < colCount; i++) {
            result.Add(skillParser.Parse(table.Rows[i]));
        }
        return result;
    }

    /// <summary>
    /// �����������
    /// </summary>
    public class TableParser<T> where T : class, new() {
        /// <summary>
        /// ��Excel���Լ����������ת��Ϊ��׼C#����
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
        /// ͨ������������TableParser����,����ʧ�ܷ���null
        /// </summary>
        public static TableParser<T> Create(DataRow header) {
            TableParser<T> parser = new TableParser<T>();
            foreach (object obj in header.ItemArray) {
                string str = (string)obj;
                string[] ss = str.Split('#');
                if(ss.Length < 2) {
                    Log.Error("������ͷ����{0}", str);
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

        //��data�е����ݸ�ֵ������ �ɲο�Global�еķ���
        private void SetValue(T obj,DataRow data){

        }
    }
}