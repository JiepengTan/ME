using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using LitJson;
using Lockstep.ECS.ECDefine;
using Lockstep.UnsafeECS.Game;
using Lockstep.Tools.MacroExpansion;
using Lockstep.UnsafeECS;
using static Lockstep.Tools.MacroExpansion.Config;
using static Lockstep.Tools.MacroExpansion.Define;
using IEntity = Lockstep.ECS.ECDefine.IEntity;

namespace Lockstep.Tools.MacroExpansion {
    [ParserDefiner(ExecuteOrder = 0)]
    public class EvalFunctions {
        static StringBuilder _builder = new StringBuilder();
        public static object curForItem => Facade.curForItem;
        public static TypeInfo curType => Facade.curType;
        public static FieldInfo curField => Facade.curField;
        public static Func<Type, string> curFuncGetTypeName => Facade.curFuncGetTypeName;
        public static int TotalEntitySize = 0;

        public static void DoAfterLoadDll(){
            TotalEntitySize = DllInfos.GetTypes<Lockstep.ECS.ECDefine.IEntity>()
                .Select(t => DllInfos.GetAttribute<EntityCountAttribute>(t).count).Sum();
            var txt = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,Facade.ConfigInfo.MacroDefinePath));
            var macroDefine = JsonMapper.ToObject<Dictionary<string, string>>(txt);
            Config.MacroDefine = macroDefine;
            Facade.MacroExpend = Config.MacroExpend;
            Facade.FuncGetTypeName = new Func<Type, string>[] {GetTypeName, GetTypeNameEntityPointer};
        }
        public static string EvalValue(string token){
            return token;
        }

        [EnumerableFunc(EF_GET_BUILDIN_COMPS)]
        public static IEnumerable GetBuildInComponents(string param){
            return UnsafeECS.Define.AllBuildInComponentTypes.ToTypeGenInfo();
        }


        [EnumerableFunc(EF_GET_TEST_RANGE)]
        public static IEnumerable GetEntityArray(string param){
            var clsName = Facade.ForceGetEnvToken(Define.PC_CLS_NAME);
            var type = DllInfos.GetType(clsName);
            var size = DllInfos.GetAttribute<EntityCountAttribute>(type).count;
            var ary = new int[size];
            for (int i = 0; i < size; i++) {
                ary[i] = i;
            }

            return ary;
        }


        [ExecuteFunc(PF_SIZE_OF_BITSET)]
        public static string GetSizeOfBitSet(){
            return curType.Name.Replace("Bitset", "");
        }

        [ExecuteFunc(PF_SIZE)]
        public static string GetEntitySize(){
            return DllInfos.GetAttribute<EntityCountAttribute>(curType.RawType).count.ToString();
        }

        [ExecuteFunc(PF_ALL_SIZE)]
        public static string GetTotalEntitySize(){
            return TotalEntitySize.ToString();
        }

        [ExecuteFunc(PC_CLS_TYPE_NAME)]
        public static string GetClassTypeName(){
            return curType.TypeName;
        }

        [ExecuteFunc(PC_CLS_NAME)]
        public static string GetClassName(){
            string val = "";
            try {
                val = curType.RawType == null ? curType.Name : GetTypeName(curType.RawType);
            }
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }

            return val;
            //return curType.RawType == null ? curType.Name : GetTypeName(curType.RawType);
        }

        [ExecuteFunc(PC_FOR_ITEM)]
        public static string GetForItem(){
            string val = "";
            try {
                val = curForItem.ToString();
            }
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }

            return val;
        }

        [ExecuteFunc(PA_FIELD_TYPE)]
        public static string GetFieldTypeName(){
            return curField.RawType == null ? curType.Name : GetTypeName(curField.RawType);
        }

        [ExecuteFunc(PA_FIELD_NAME)]
        public static string GetFieldName(){
            return curField.Name;
        }


        [PrepareFunction(BT_ENUM)]
        public static TypeInfo[] PrepareTypeInfosEnum(Type[] rawTypes){
            return DllInfos.GetTypes(BT_ENUM, (bt, t) => t.IsEnum);
        }

        [PrepareFunction(BT_REF)]
        public static TypeInfo[] PrepareTypeInfosRef(Type[] rawTypes){
            return DllInfos.GetTypes(BT_REF, (bt, t) => !t.IsGenericType && DllInfos.IsTargetType(bt, t));
        }

        [PrepareFunction(BT_COLLISION)]
        public static TypeInfo[] PrepareTypeInfosCollision(Type[] rawTypes){
            List<TypeInfo> typeInfos = new List<TypeInfo>();
            typeInfos.AddRange(rawTypes.Where(DllInfos.IsTargetType<ISignal>).ToArray().ToTypeGenInfo());
            HashSet<Type> AllTypes = new HashSet<Type>();
            List<MethodInfo> Funcs = new List<MethodInfo>();
            var collisionDefine = rawTypes.Where(DllInfos.IsTargetType<ICollisionDefine>).ToArray();
            if (collisionDefine.Length == 0) return typeInfos.ToArray();
            var colDefine = collisionDefine[0];
            var methods = colDefine.GetMethods().Where(
                methodInfo => methodInfo.GetCustomAttributes(typeof(SignalAttribute), false).ToArray()
                                  .Length > 0);
            Funcs.AddRange(methods);
            foreach (var methodInfo in Funcs) {
                var pars = methodInfo.GetParameters();
                foreach (var param in pars) {
                    AllTypes.Add(param.ParameterType);
                }
            }


            AllTypes.Add(typeof(Entity));
            var formats = new string[] {"On{0}Created", "On{0}Destroy"};
            foreach (var type in AllTypes) {
                foreach (var format in formats) {
                    typeInfos.Add(new TypeInfo() {
                        Name = string.Format(format, type.Name),
                        FieldInfos = new List<FieldInfo>() {
                            new FieldInfo() {
                                Name = "entity",
                                RawType = type
                            }
                        }
                    });
                }
            }

            var prefixes = new string[] {"OnCollisionDynamic", "OnTriggerDynamic"};
            foreach (var func in Funcs) {
                if (func.GetParameters().Length > 1) {
                    var name = string.Concat(func.GetParameters().Select(t => t.ParameterType.Name));
                    foreach (var prefix in prefixes) {
                        var info = func.ToTypeGenInfo();
                        info.Name = prefix + name;
                        typeInfos.Add(info);
                    }
                }
            }

            return typeInfos.ToArray();
        }


        public static string GetTypeName(Type type){
            return GetTypeNameWithPostFix(type, "");
        }

        public static string GetTypeNameEntityPointer(Type type){
            var postfix = "";
            if (DllInfos.IsTargetType<IEntity>(type) || type.Name == "Entity") {
                postfix = "*";
            }

            return GetTypeNameWithPostFix(type, postfix);
        }

        private static string GetTypeNameWithPostFix(Type type, string postfix){
            if (Type2Str.TryGetValue(type, out string val)) {
                return val + postfix;
            }

            if (type.IsGenericType) {
                return $"{type.Name.Replace("`1", "")}{type.GenericTypeArguments[0].Name}" + postfix;
            }
            else {
                return type.Name + postfix;
            }
        }

        public static int GetEntityCount(Type entityType){
            return DllInfos.GetAttribute<EntityCountAttribute>(entityType).count;
        }
    }
}