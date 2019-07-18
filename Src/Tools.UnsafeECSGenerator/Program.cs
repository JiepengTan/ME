//#define DEBUG_SHOW_PASER_INFO//输出debug 信息

using System;
using System.Collections.Generic;
using System.IO;
using Lockstep.Util;

namespace Lockstep.Tools.MacroExpansion {
    internal class Program {
        public static void Main(string[] args){
            ParseCodes(args);
        } 
        
        private static void ParseCodes(string[] args){
            if (args == null || args.Length == 0) {
                args = new[] {"../Config/ECSGenerator/UnsafeECS.json"};
            }

            if (args.Length > 0) {
                foreach (var path in args) {
                    var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + path);
                    Console.WriteLine("ConfigPath: " + configPath);
                    Facade.LoadConfig(configPath);
                    Facade.ParseAll();
                }
            }
            else {
                Console.WriteLine("Need config path");
            }
        }
    }
}