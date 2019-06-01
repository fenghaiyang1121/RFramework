using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

[System.Serializable]
public class BuffData :ExcelBase
{
#if UNITY_EDITOR
    public override void Construction()
    {
        AllBuffList = new List<BuffBase>();
        for (int i = 0; i < 10; i++)
        {
            BuffBase buff = new BuffBase();
            buff.Id = i + 1;
            buff.Name = "全BUFF" + i;
            buff.OutLook = "Assets/GameData/..." + i;
            buff.Time = Random.Range(0.5F, 10);
            buff.BuffType = (BuffEnum)Random.Range(0, 4);
            buff.AllString = new List<string>();
            buff.AllString.Add("ceshi" + i);
            buff.AllString.Add("ceshiq" + i);
            buff.AllBuffList = new List<BuffTest>();
            int count = Random.Range(0, 4);
            for (int j = 0; j < count; j++)
            {
                BuffTest test = new BuffTest();
                test.Id = j + Random.Range(0, 5);
                test.Name = "name" + j;
                buff.AllBuffList.Add(test);
            }
            AllBuffList.Add(buff);
        }
        MonsterBuffList = new List<BuffBase>();
        for (int i = 0; i < 5; i++)
        {
            BuffBase buff = new BuffBase();
            buff.Id = i + 1;
            buff.Name = "全BUFF" + i;
            buff.OutLook = "Assets/GameData/..." + i;
            buff.Time = Random.Range(0.5F, 10);
            buff.BuffType = (BuffEnum)Random.Range(0, 4);
            buff.AllString = new List<string>();
            buff.AllString.Add("ceshi" + i);
            buff.AllBuffList = new List<BuffTest>();
            int count = Random.Range(0, 4);
            for (int j = 0; j < count; j++)
            {
                BuffTest test = new BuffTest();
                test.Id = j + Random.Range(0, 5);
                test.Name = "name" + j;
                buff.AllBuffList.Add(test);
            }
            MonsterBuffList.Add(buff);
        }
    }
#endif

    public override void Init()
    {
        AllBuffDic.Clear();
        for (int i = 0; i < AllBuffList.Count; i++)
        {
            AllBuffDic.Add(AllBuffList[i].Id, AllBuffList[i]);
        }
    }

    /// <summary>
    /// 根据ID查找buff
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public BuffBase FinBuffById(int id)
    {
        return AllBuffDic[id];
    }

    [XmlIgnore]
    public Dictionary<int, BuffBase> AllBuffDic = new Dictionary<int, BuffBase>();

    [XmlElement("AllBuffList")]
    public List<BuffBase> AllBuffList { get; set; }

    [XmlElement("MonsterBuffList")]
    public List<BuffBase> MonsterBuffList { get; set; }
}

public enum BuffEnum
{
    None = 0,
    Ranshao = 1,
    Bingdong = 2,
    Du = 3,
}

[System.Serializable]
public class BuffBase
{
    [XmlAttribute("Id")]
    public int Id { get; set; }

    [XmlAttribute("Name")]
    public string Name { get; set; }

    [XmlAttribute("OutLook")]
    public string OutLook { get; set; }

    [XmlAttribute("Time")]
    public float Time { get; set; }
    
    [XmlAttribute("BuffType")]
    public BuffEnum BuffType { get; set; }

    [XmlElement("AllString")]
    public List<string> AllString { get; set; }

    [XmlElement("AllBuffList")]
    public List<BuffTest> AllBuffList { get; set; }
}

[System.Serializable]
public class BuffTest
{
    [XmlAttribute("Id")]
    public int Id { get; set; }

    [XmlAttribute("Name")]
    public string Name { get; set; }
}
