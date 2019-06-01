# RFrameword
Unity的资源加载框架

使用方式：

1，将RealFram/FramePlug/RFramework 拖到自己游戏的初始场景

2，ab包配置方式：(主要分为两种配置方式)

   打开RealFram/Edtior/Resource/ABConfig（分别为AllPrefabPath与AllFileDirAB）
   AllPrefabPath为prefab文件夹路径，可以设置多个，最终编辑器会去根据文件夹查找里面所有的Prefab去计算依赖打包（注意不要出现同名Prefab，因为每个prefab会单独根据prefab名字打包ab包）
   AllFileDirAB为单个文件夹ab包设置，设置的时候需要设置ab包名与ab包对应文件夹路径（如：data  Asset/RealFram/Data/Binary）
   设置好之后打包就会根据设置自动筛选及自动设定ab包，进行打包，默认打包在Assets同目录根据不同平台所生成的文件夹下面（不会生成在Asset目录里面，打包apk等时，工具会根据平台自动拷贝ab包到StreamingAssets目录，如果热更或者初始包问题可自行更改代码 BuildApp  与   BundleEditor）。
   
3，资源加载代码使用：

  1）同步资源加载：
  
  ResourceManager.Instance.LoadResource<T>(path)  泛型方法，path为资源的Unity工程相对路径，如： Assets/Data/image.png;。此方法加载不需要实例化的资源，如图片，asset,音频等资源文件。
  
  2）异步资源加载：
  
  ResourceManager.Instance.AsyncLoadResource(string path, OnAsyncObjFinish dealfinish, LoadResPriority priority, object param1 = null, object param2 = null, object param3 = null, uint crc = 0, bool bsprite = false) 异步资源加载函数，此方法加载不需要实例化的资源，如图片，asset,音频等资源文件。 path为资源的Unity工程相对路径，如： Assets/Data/image.png;， dealfinish 为加载回调，priority为加载优先级，param1，param2，param3 为参数，可以向回调传递参数。 crc为资源路径计算出来的crc，如果计算了crc那么就会强制根据crc查找，不然会根据路径进行查找资源。bsprite为是否是图片，因为异步资源加载图片有特殊转换。
  void OnAsyncObjFinish(string path, Object obj, object param1 = null, object param2 = null, object param3 = null)
  回调函数写法，都是由上面异步加载传递下来的
  
  3）释放资源：
  
  ResourceManager.Instance.ReleaseResource(Object obj, bool destoryObj = false)  
  同步资源加载的释放方法，obj参数为加载资源的引用，destoryObj为是否完全释放改资源。
  ResourceManager.Instance.ReleaseResource(string path, bool destoryObj = false) 
  同步资源加载的释放方法，path参数为加载资源的路径，destoryObj为是否完全释放改资源。
  
  4）预加载资源：
  
  ResourceManager.Instance.PreloadResource(string path) 
  预加载资源，主要传入路径即可进行预加载。
  
  5）同步实例化gameobject
  
  ObjectManager.Instance.InstantiateObject(string resPath, bool setSceneObject = false, ResourceType resType = ResourceType.NONE, bool bClear = true)  
  同步gameobject加载，resPath为prefab路径，setSceneObject为是否放到Scene节点下面，resType资源类型，bClear为是否跳场景清除。
  
  6）异步实例化gameobject
  
  ObjectManager.Instance.InstantiateObjectAsync(string resPath, OnAsyncObjFinish dealFinish, LoadResPriority priority,
        ResourceType resType = ResourceType.NONE, bool setSceneObject = false,object param1 = null, 
        object param2 = null, object param3 = null, bool bClear = true, bool bCancle = true)
  异步gameobject加载，resPath为prefab路径，dealFinish为加载回调，priority为加载优先级，resType为加载类型，setSceneObject为是否放到Scene节点下面，resType资源类型，param1，param2，param3 为参数，可以向回调传递参数，bClear为是否跳场景清除，bCancle为是否可以取消加载。
  void OnAsyncObjFinish(string path, Object obj, object param1 = null, object param2 = null, object param3 = null)
  回调函数写法，都是由上面异步加载传递下来的
  
  7）预加载gameobject
  
  ObjectManager.Instance.PreloadGameObject(string resPath, int count = 1, bool clear = false)
  预加载gamobject,resPath为Prefab路径，count为预加载个数，clear为跳场景是否清除
  
  8）跳场景时对缓存的清除
  
  ObjectManager.Instance.ClearCache();
  ResourceManager.Instance.ClearCache();
  
  调用这两个函数，在资源加载的时候有参数来确定某些资源或者Prefab跳场景是否清除，如果不清除，将常驻内存，方便快速加载。
  
  9）关于离线数据的使用
  目前离线数据有UI离线数据及特效离线数据，主要用于实例化的gameobject从对象池取出时还原原本结构（当然，如果很复杂的结果还原难以做到，需要使用者自己根据情况还原，基本的还原类型都有，也可以去拓展UIOfflineData与EffectOfflineData或者继承OfflineData去写新的离线数据还原）
  操作方法，选中Prefab右键生成对应的离线数据，此功能还可以拓展缓冲gameobject的任何组件，避免代码中经常出现getcompontent的操作。但是会占部分内存。
  
  10）数据资源加载转换
  此框架包含了数据资源加载及转换， 类<-> xml<->二进制<->excel,都可以互转，可以去查看DataEditor脚本。编辑器工具也有右键转换xml,二进制等功能。xml与excel的转换基于reg文件，在Assets同级目录的Data目录中，有BuffData的xml-excel配置例子。在工程目录中RealFram/DemoData中有BuffData类的例子，可供参看。
  
  
  
   
