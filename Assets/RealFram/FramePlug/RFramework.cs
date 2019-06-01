using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RFramework : MonoSingleton<RFramework>
{
    protected override void Awake()
    {
        base.Awake();
        GameObject.DontDestroyOnLoad(gameObject);
        AssetBundleManager.Instance.LoadAssetBundleConfig();
        ResourceManager.Instance.Init(this);
        ObjectManager.Instance.Init(transform.Find("RecyclePoolTrs"), transform.Find("SceneTrs"));
    }

    // Use this for initialization
    void Start ()
    {
        
    }


    //加载配置表
    void LoadConfiger()
    {
        //加载配置表示例
        //ConfigerManager.Instance.LoadData<MonsterData>(CFG.TABLE_MONSTER);
        //ConfigerManager.Instance.LoadData<BuffData>(CFG.TABLE_BUFF);
    }
	
	// Update is called once per frame
	void Update ()
    {

	}

    private void OnApplicationQuit()
    {
#if UNITY_EDITOR
        ResourceManager.Instance.ClearCache();
        Resources.UnloadUnusedAssets();
        Debug.Log("清空编辑器缓存");
#endif
    }
}
