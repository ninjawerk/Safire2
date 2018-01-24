using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Kornea.Audio.DSP
{
    public struct EQSTATE
    {
 
        public double lf;        
        public double f1p0;      
        public double f1p1;
        public double f1p2;
        public double f1p3;

        public double hf;        
        public double f2p0;      
        public double f2p1;
        public double f2p2;
        public double f2p3;

        public double sdm1;     
        public double sdm2;      
        public double sdm3;      

        public double lg;        
        public double mg;       
        public double hg;       

    } ;
    class k32Imports
    {
        [DllImport(@"Korneadspx32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void init_3band_state([MarshalAs(UnmanagedType.Struct)] ref EQSTATE es, int lowfreq, int highfreq, int mixfreq);

        [DllImport(@"Korneadspx32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double do_3band([MarshalAs(UnmanagedType.Struct)] ref EQSTATE es, double sample);

    }
}
