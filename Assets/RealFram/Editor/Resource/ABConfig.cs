using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ABConfig", menuName = "CreatABConfig", order = 0)]
public class ABConfig : ScriptableObject
{
    //单个文件所在文件夹路径，会遍历这个文件夹下面所有Prefab,所有的Prefab的名字不能重复，必须保证名字的唯一性
    public List<string> m_AllPrefabPath = new List<string>();
    public List<FileDirABName> m_AllFileDirAB = new List<FileDirABName>();

    [System.Serializable]
    public struct FileDirABName
    {
        public string ABName;
        public string Path;
    }
}
