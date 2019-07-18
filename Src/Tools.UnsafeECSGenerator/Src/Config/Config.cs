using System;
using System.Collections.Generic;
using System.Linq;
using Lockstep.Tools.MacroExpansion;

namespace Lockstep.Tools.MacroExpansion {
    public class Config {

        
        public const string TF_REF                = "#REF";
        public const string TF_ENUM               = "#ENUM";
        public const string TF_ASSET              = "#ASSET";
        public const string TF_COLLISION          = "#COLLISION";
        public const string TF_ENTITY             = "#ENTITY"; //func syntactic sugar
        public const string TF_COMPONENT          = "#COMPONENT";
        public const string TF_FIELDS             = "#FIELDS";
        public const string TF_SIGNAL             = "#SIGNAL";
        public const string TF_EVENT              = "#EVENT";
        public const string TF_BITSET             = "#BITSET";        
        public const string TF_GET_BUILDIN_COMPS  = "#BUILDIN_COMPS";

        
        public const string BT_REF         = "IRef";
        public const string BT_ENUM        = "IEnum";
        public const string BT_COLLISION   = "ICollision";
        public const string BT_ASSET       = "IAsset";
        public const string BT_ENTITY      = "IEntity";
        public const string BT_COMPONENT   = "IComponent";
        public const string BT_FIELDS      = "IFields";
        public const string BT_SIGNAL      = "ISignal";
        public const string BT_EVENT       = "IEvent";
        public const string BT_BITSET      = "IBitset";
          
          
        //func param  
        public const string PF_SIZE               = "#SIZE";
        public const string PF_ALL_SIZE           = "#ALL_SIZE";
        public const string PF_SIZE_OF_BITSET     = "#SIZE_OF_BITSET";
        
        
        // enumerator func 
        public const string EF_GET_TEST_RANGE     = "GetEntityArray";
        public const string EF_GET_BUILDIN_COMPS  = "GetBuildInComponents";
        
        public static string MacroExpend(string token){
            if(Facade.BuildInMacroExpendDefine.TryGetValue(token,out var valInner))
            {
                return valInner;
            }
            if(MacroDefine.TryGetValue(token,out var val))
            {
                return val;
            }
            return token;           

        }
        public static Dictionary<string, string> MacroDefine = new Dictionary<string, string>() {};

        public static Dictionary<Type, string> Type2Str = new Dictionary<Type, string>() {
            {typeof(byte), "byte"},
            {typeof(sbyte), "sbyte"},
            {typeof(short), "short"},
            {typeof(ushort), "ushort"},
            {typeof(int), "int"},
            {typeof(uint), "uint"},
            {typeof(long), "long"},
            {typeof(ulong), "ulong"},
            {typeof(bool), "bool"},
            {typeof(string), "string"},
            {typeof(float), "LFloat"},
            {typeof(Lockstep.ECS.ECDefine.Vector2), "LVector2"},
            {typeof(Lockstep.ECS.ECDefine.Vector3), "LVector3"},
            {typeof(Lockstep.ECS.ECDefine.Quaternion), "LQuaternion"},
        };

    }
}