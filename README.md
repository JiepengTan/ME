##  ME (MacroExpansion)

##### 1.特点
1. 主要功能是宏展开，提供for,if,function,三种基本语言的特性，用于弥补c#等不支持宏拓展的编程语言的缺点。
2. 初衷是用于[LockstepPlatform][1]中的[UnsafeECS][2]框架的设计，加快基于代码生成的框架的迭代速度。

原理：
读取配置制定路径下的dll, 反射所有类型，然后根据模版文件，使用这些反射得到的类型信息，替换模版中的token,模版中可以通过for if 以及函数调用的形式进行求值替换

视频解说地址:

3. part01: [https://www.bilibili.com/video/av59694947][5]


<iframe src="//player.bilibili.com/player.html?aid=59694947&cid=103980168&page=1" scrolling="no" border="0" frameborder="no" framespacing="0" allowfullscreen="true"> </iframe>


4. part02: [https://www.bilibili.com/video/av59718130][6]

<iframe src="//player.bilibili.com/player.html?aid=59718130&cid=104000103&page=1" scrolling="no" border="0" frameborder="no" framespacing="0" allowfullscreen="true"> </iframe>
##### 2.语法&语言特性

```cs  

 1. 以 #_ME_FOR 作为for循环的开头标志
 2. 以 #_ME_ENDFOR 作为for循环的结束标志
 3. 以 分号 ; 作为关键字语句的划分
 4. 函数调用 以 call XxxFunc(0,10,1);
```

1. 默认设置:

```cs   

#FOR_ITEM     //call 关键字调用时候如果返回的不是IEnumerable 中的元素不是FieldInfo or TypeInfo 时候，代表的元素
#CLS_NAME     //FieldInfo.Name  or TypeInfo.Name
#FIELD_TYPE   //FieldInfo.TypeName
#FIELD_NAME   //FieldInfo.Name

```

2. 内置函数

```cs   

#RANGE  宏展后 call GetRange  返回一个数组枚举。 使用#FOR_ITEM  可以获取其中的值。
        //GetRange
    eg: #RANGE(0,10,2) 表示返回 [0,2,4,6,8] 
        #RANGE(2,5) 返回 [2，3，4]
        #RANGE(4) 返回 [0,1,2,3]

#GET_FIELDS 返回当前#CLS_NAME 表示的类型 所有的(public) Field 成员
    等同代码:
            //GetFields
            var clsName = Facade.ForceGetEnvToken("#CLS_NAME");
            var type = DllInfos.GetType(clsName);
            return type.ToTypeGenInfo().GetFields();

#GET_FIELDS_OF(TestName) 返回当前#CLS_NAME 表示的类型 的名字为 "TestName" 的Field的类型的所有Fileds
    等同代码:
            //GetFieldsOfField
            var clsName = Facade.ForceGetEnvToken("#CLS_NAME");
            var type = DllInfos.GetType(clsName);
            var field = type.GetField("TestName");
            return field.FieldType.ToTypeGenInfo().GetFields();

HasField(#CLS_NAME,#BUILIN_NAME) 返回当前#CLS_NAME 表示的类型 是否含有 #BUILIN_NAME 所表示的值的Field 成员
    等同代码:
            var paras = param.Split(',');
            var clsName1 = Facade.ForceGetEnvToken(paras[0].Trim());
            var clsName2 = Facade.ForceGetEnvToken(paras[1].Trim());
            var type = DllInfos.GetType(clsName1);
            return type.GetField(clsName2) != null;

#CALL_PARAMS_LIST 返回一个类型的左右Field 成员拼接而成的函数调用参数
        eg:
            定义类型
            public class OnCastProjectile : ISignal {
                public EntityRef projectileSource;
                public Vector2 forward;
                public Vector2 right;
                public float projectileAngle;
            }
            
           #CALL_PARAMS_LIST = “EntityRef projectileSource, LVector2 forward, LVector2 right, LFloat projectileAngle”

#CALL_PARAMS_LIST 返回一个类型的左右Field 成员拼接而成的函数调用参数
        eg:
            定义类型
            public class OnCastProjectile : ISignal {
                public EntityRef projectileSource;
                public Vector2 forward;
                public Vector2 right;
                public float projectileAngle;
            }
            
           #CALL_PARAMS_LIST = “,projectileSource ,forward ,right ,projectileAngle“ 
           //!! 注意最前面会含有 “,”
```


##### 3.关键字说明
-     call    //函数调用 用于for 循环 item 枚举
-     If      //条件判定 用于过滤不需要的数据
-     typeof  //需要制定名字的所有子类(不含自身) 用于for 循环 item 枚举
-     tag     //用于标记需要特殊处理的循环,可以更加这个tag 来配置不同的处理函数
-     rename  //重命名相应的变量，避免命名冲突
-     use     //明确制定下面block中需要进行替换的额外的宏,

##### 4.例子说明

 具体项目配置请直接下载项目查看[ME][4]
 配置目录在 xxx/ME-0.2.0/Config/ECSGenerator/TemplateInput

1. typeof 使用

```cs     

    public unsafe partial struct __default {
        //IEntity 
#_ME_FOR #ENTITY
        public static #CLS_NAME #CLS_NAME;
#_ME_ENDFOR  
        //IAsset 
#_ME_FOR typeof IAsset
        public static #CLS_NAME #CLS_NAME;
#_ME_ENDFOR 
        //Bitset  
#_ME_FOR typeof(IBitset) 
        public static #CLS_NAME #CLS_NAME;
#_ME_ENDFOR 
    }
```

```cs  

    public unsafe partial struct __default {
        //IEntity 
        public static Goblin Goblin;
        public static Enemy Enemy;
        public static Projectile Projectile;
  
        //IAsset 
        public static State State;
        public static Action Action;
        public static Decision Decision;
        public static ProjectileSpec ProjectileSpec;
        public static SpawnerData SpawnerData;
        public static CharacterSpec CharacterSpec;

        //Bitset  
        public static Bitset128 Bitset128;
        public static Bitset256 Bitset256;
        public static Bitset512 Bitset512;
        public static Bitset1024 Bitset1024;
        public static Bitset2048 Bitset2048;
        public static Bitset4096 Bitset4096;
 
        //...还有其他代码
    }    
```

2. use , call #RANGE 

```cs
    [StructLayout(LayoutKind.Sequential, Pack = Define.PackSize)]
    public unsafe partial struct __entities {
#_ME_FOR #RANGE(0,10,1)
        public const Int32 Size#FOR_ITEM = #FOR_ITEM;
#_ME_ENDFOR 

#_ME_FOR #ENTITY;use(#SIZE)
        public const Int32 #CLS_NAMESize = #SIZE;
    #_ME_FOR call GetEntityArray;rename(#TESTCLS_NAME = #CLS_NAME)
        public const #TESTCLS_NAME #CLS_NAME#FOR_ITEM;
    #_ME_ENDFOR 
#_ME_ENDFOR 
    }
```
输出结果
```cs
    [StructLayout(LayoutKind.Sequential, Pack = Define.PackSize)]
    public unsafe partial struct __entities {
        public const Int32 Size0 = 0;
        public const Int32 Size1 = 1;
        public const Int32 Size2 = 2;
        public const Int32 Size3 = 3;
        public const Int32 Size4 = 4;
        public const Int32 Size5 = 5;
        public const Int32 Size6 = 6;
        public const Int32 Size7 = 7;
        public const Int32 Size8 = 8;
        public const Int32 Size9 = 9;
 

        public const Int32 GoblinSize = 4;
        public const Goblin Goblin0;
        public const Goblin Goblin1;
        public const Goblin Goblin2;
        public const Goblin Goblin3;
        //...还有其他代码
    }
```
3. rename, if  使用范例

```cs  
#_ME_FOR #BUILDIN_COMPS; rename(#BUILIN_NAME = #CLS_NAME)
        public unsafe Buffer<#BUILIN_NAMEFilter> GetAll#BUILIN_NAME()
        {
            Buffer<#BUILIN_NAMEFilter> buffer = Buffer<#BUILIN_NAMEFilter>.Alloc(#ALL_SIZE);
#_ME_FOR #ENTITY; if(HasField(#CLS_NAME,#BUILIN_NAME))
            #CLS_NAME* #CLS_NAMEPtr = this._entities->Get#CLS_NAME(0);
            var idx#CLS_NAME = #SIZE;
            while (idx#CLS_NAME >= 0)
            {
                if (#CLS_NAMEPtr->_entity._active)
                {
                  buffer.Items[buffer.Count].Entity = &#CLS_NAMEPtr->_entity;
                  buffer.Items[buffer.Count].#BUILIN_NAME = &#CLS_NAMEPtr->#BUILIN_NAME;
                  ++buffer.Count;
                }
                --idx#CLS_NAME;
                ++#CLS_NAMEPtr;
            }
#_ME_ENDFOR 
            return buffer;
        }
#_ME_ENDFOR 
    }
```

```cs  
                           
        public unsafe Buffer<Transform2DFilter> GetAllTransform2D()
        {
             //...还有其他代码
             public unsafe Buffer<Transform2DVerticalFilter> GetAllTransform2DVertical()
             {
                 Buffer<Transform2DVerticalFilter> buffer = Buffer<Transform2DVerticalFilter>.Alloc(#ALL_SIZE);
      
                 return buffer;
             }
             public unsafe Buffer<NavMeshAgentFilter> GetAllNavMeshAgent()
             {
                 Buffer<NavMeshAgentFilter> buffer = Buffer<NavMeshAgentFilter>.Alloc(#ALL_SIZE);
                 Enemy* EnemyPtr = this._entities->GetEnemy(0);
                 var idxEnemy = 30;
                 while (idxEnemy >= 0)
                 {
                     if (EnemyPtr->_entity._active)
                     {
                       buffer.Items[buffer.Count].Entity = &EnemyPtr->_entity;
                       buffer.Items[buffer.Count].NavMeshAgent = &EnemyPtr->NavMeshAgent;
                       ++buffer.Count;
                     }
                     --idxEnemy;
                     ++EnemyPtr;
                 }
      
                 return buffer;
             }
             //...还有其他代码
        }
```

3. tag , #DECLARE_PARAMS_LIST

```cs
    public unsafe partial class FrameSignals  {
#_ME_FOR #COLLISION; use(#DECLARE_PARAMS_LIST,#CALL_PARAMS_LIST); tag(1)
        public void #CLS_NAME(#DECLARE_PARAMS_LIST) {
            var array = _f._ISignal#CLS_NAMESystems;
            var systems = &(_f._globals->Systems);
            for (Int32 i = 0; i < array.Length; ++i) {
                var s = array[i];
                if (BitSet256.IsSet(systems, s.RuntimeIndex)) {
                  s.#CLS_NAME(_f#CALL_PARAMS_LIST);
                }
            }
        }
#_ME_ENDFOR    
    }
```

```cs
    
    public unsafe partial class FrameSignals  {
        public void OnDamage(DamageStructure dmg) {
            var array = _f._ISignalOnDamageSystems;
            var systems = &(_f._globals->Systems);
            for (Int32 i = 0; i < array.Length; ++i) {
                var s = array[i];
                if (BitSet256.IsSet(systems, s.RuntimeIndex)) {
                  s.OnDamage(_f,dmg);
                }
            }
        }
        public void OnBossDeath() {
            var array = _f._ISignalOnBossDeathSystems;
            var systems = &(_f._globals->Systems);
            for (Int32 i = 0; i < array.Length; ++i) {
                var s = array[i];
                if (BitSet256.IsSet(systems, s.RuntimeIndex)) {
                  s.OnBossDeath(_f);
                }
            }
        }
        public void OnCastProjectile(EntityRef projectileSource, LVector2 forward, LVector2 right, LFloat projectileAngle, EProjectileType projectileType, ProjectileSpec projectileSpec) {
            var array = _f._ISignalOnCastProjectileSystems;
            var systems = &(_f._globals->Systems);
            for (Int32 i = 0; i < array.Length; ++i) {
                var s = array[i];
                if (BitSet256.IsSet(systems, s.RuntimeIndex)) {
                  s.OnCastProjectile(_f,projectileSource ,forward ,right ,projectileAngle ,projectileType ,projectileSpec);
                }
            }
        }
        //...还有其他代码
    }
```


 [1]: https://github.com/JiepengTan/LockstepPlatform
 [2]: https://github.com/JiepengTan/Lockstep.UnsafeECS
 [3]: https://github.com/JiepengTan/LockstepECL
 [4]: https://github.com/JiepengTan/ME/releases/tag/v0.2.0
 [5]: https://www.bilibili.com/video/av59694947
 [6]: https://www.bilibili.com/video/av59718130
