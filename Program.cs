using de4dot.blocks;
using de4dot.blocks.cflow;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IntptrPoint
{
    class Program
    {
        public static string Asmpath;
        public static ModuleDefMD module;
        public static Assembly asm;

        private static bool PointMutations = true;



        static void Main(string[] args)
        {
            Console.Title = "IntPtr Fixer by OFF_LINE";
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n");
            string lol = @" ______              __      _______     __                ________  __    __ 
/      |            /  |    /       \   /  |              /        |/  |  /  |
$$$$$$/  _______   _$$ |_   $$$$$$$  | _$$ |_     ______  $$$$$$$$/ $$ |  $$ |
  $$ |  /       \ / $$   |  $$ |__$$ |/ $$   |   /      \ $$ |__    $$  \/$$/ 
  $$ |  $$$$$$$  |$$$$$$/   $$    $$/ $$$$$$/   /$$$$$$  |$$    |    $$  $$<  
  $$ |  $$ |  $$ |  $$ | __ $$$$$$$/    $$ | __ $$ |  $$/ $$$$$/      $$$$  \ 
 _$$ |_ $$ |  $$ |  $$ |/  |$$ |        $$ |/  |$$ |      $$ |       $$ /$$  |
/ $$   |$$ |  $$ |  $$  $$/ $$ |        $$  $$/ $$ |      $$ |      $$ |  $$ |
$$$$$$/ $$/   $$/    $$$$/  $$/          $$$$/  $$/       $$/       $$/   $$/ 
                                                                              
                                                                              
                                                                              ";
            Console.WriteLine(lol);
            string diretorio = args[0];
            try
            {
                Program.module = ModuleDefMD.Load(diretorio, (ModuleCreationOptions) null);
                Program.asm = Assembly.LoadFrom(diretorio);
                Program.Asmpath = diretorio;
            }

            catch (Exception)
            {
                Console.WriteLine("Not .NET Assembly...");
            }
            string text = Path.GetDirectoryName(diretorio);
            bool flag = !text.EndsWith("\\");
            bool flag2 = flag;
            if (flag2)
            {
                text += "\\";
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Successfully loaded assembly!");
            Console.ForegroundColor = ConsoleColor.White;

        
            try
            {
                Console.WriteLine("Removing INTPTR mutations!");
                IntptrPoint.IntPtrRemover.Clean();
                Console.ForegroundColor = ConsoleColor.White;

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[!] Failed to remove mutations. ORIGINAL EXCEPTION: " + ex.ToString());

            }
            string filename = string.Format("{0}{1}_RemovedPoint{2}", text, Path.GetFileNameWithoutExtension(diretorio), Path.GetExtension(diretorio));
            ModuleWriterOptions writerOptions = new ModuleWriterOptions(Program.module);
            writerOptions.MetaDataOptions.Flags |= MetaDataFlags.PreserveAll;
            writerOptions.Logger = DummyLogger.NoThrowInstance;
            NativeModuleWriterOptions NativewriterOptions = new NativeModuleWriterOptions(Program.module);
            NativewriterOptions.MetaDataOptions.Flags |= MetaDataFlags.PreserveAll;
            NativewriterOptions.Logger = DummyLogger.NoThrowInstance;
            bool isILOnly = Program.module.IsILOnly;
            if (isILOnly)
            {
                Program.module.Write(filename, writerOptions);
            }
            else
            {
                Program.module.NativeWrite(filename, NativewriterOptions);
            }
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Successfully removed " + IntPtrRemover.amount + " IntPtr mutations");
            Console.ReadLine();
        }
    }
}
