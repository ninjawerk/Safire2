using System.Runtime.InteropServices;

namespace Kornea.Windows {

    public static partial class HookManager {
       
        [StructLayout(LayoutKind.Sequential)]
        private struct Point {
            
            public int X;
             
            public int Y;
        }
      
        [StructLayout(LayoutKind.Sequential)]
        private struct MouseLLHookStruct {
             public Point Point;
             public int MouseData;
             public int Flags;
             public int Time;
             public int ExtraInfo;
        }

       
        [StructLayout(LayoutKind.Sequential)]
        private struct KeyboardHookStruct
        {
 
            public int VirtualKeyCode;
        
            public int ScanCode;
           
            public int Flags;
           
            public int Time;
          
            public int ExtraInfo;
        }
    }
}
