using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEngine;

public class BinarySerializeOpt
{
    /// <summary>
    /// 类序列化成xml
    /// </summary>
    /// <param name="path"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool Xmlserialize(string path, System.Object obj)
    {
        try
        {
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
                {
                    //XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                    //namespaces.Add(string.Empty, string.Empty);
                    XmlSerializer xs = new XmlSerializer(obj.GetType());
                    xs.Serialize(sw, obj);
                }
            }
            return true;
;        }
        catch (Exception e)
        {
            Debug.LogError("此类无法转换成xml " + obj.GetType() + "," + e);
        }
        return false;
    }

    /// <summary>
    /// 编辑器使读取xml
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public static T XmlDeserialize<T>(string path) where T : class
    {
        T t = default(T);
        try
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));
                t = (T)xs.Deserialize(fs);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("此xml无法转成二进制: " + path + "," + e);
        }
        return t;
    }

    /// <summary>
    /// Xml的反序列化
    /// </summary>
    /// <param name="path"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static System.Object XmlDeserialize(string path, Type type)
    {
        System.Object obj = null;
        try
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                XmlSerializer xs = new XmlSerializer(type);
                obj = xs.Deserialize(fs);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("此xml无法转成二进制: " + path + "," + e);
        }
        return obj;
    }


    /// <summary>
    /// 运行时使读取xml
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public static T XmlDeserializeRun<T>(string path) where T : class
    {
        T t = default(T);
        TextAsset textAsset = ResourceManager.Instance.LoadResource<TextAsset>(path);

        if (textAsset == null)
        {
            UnityEngine.Debug.LogError("cant load TextAsset: " + path);
            return null;
        }

        try
        {
            using (MemoryStream stream = new MemoryStream(textAsset.bytes))
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));
                t = (T)xs.Deserialize(stream);
            }
            ResourceManager.Instance.ReleaseResouce(path, true);
        }
        catch (Exception e)
        {
            Debug.LogError("load TextAsset exception: " + path + "," + e);
        }
        return t;
    }

    /// <summary>
    /// 类转换成二进制
    /// </summary>
    /// <param name="path"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool BinarySerilize(string path, System.Object obj)
    {
        try
        {
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, obj);
            }
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("此类无法转换成二进制 " + obj.GetType() + "," + e);
        }
        return false;
    }

    /// <summary>
    /// 读取二进制
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public static T BinaryDeserilize<T>(string path) where T : class
    {
        T t = default(T);
        TextAsset textAsset = ResourceManager.Instance.LoadResource<TextAsset>(path);

        if (textAsset == null)
        {
            UnityEngine.Debug.LogError("cant load TextAsset: " + path);
            return null;
        }

        try
        {
            using (MemoryStream stream = new MemoryStream(textAsset.bytes))
            {
                BinaryFormatter bf = new BinaryFormatter();
                t = (T)bf.Deserialize(stream);
            }
            ResourceManager.Instance.ReleaseResouce(path, true);
        }
        catch (Exception e)
        {
            Debug.LogError("load TextAsset exception: " + path + "," + e);
        }
        return t;
    }
}
