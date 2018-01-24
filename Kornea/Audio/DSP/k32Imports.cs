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
		//[DllImport(@"Korneadspx32.dll", CallingConvention = CallingConvention.Cdecl)]
		//public static extern void init_3band_state([MarshalAs(UnmanagedType.Struct)] ref EQSTATE es, int lowfreq, int highfreq, int mixfreq);

		//[DllImport(@"Korneadspx32.dll", CallingConvention = CallingConvention.Cdecl)]
		//public static extern double do_3band([MarshalAs(UnmanagedType.Struct)] ref EQSTATE es, double sample);
		static double vsa = (1.0 / 4294967295.0);   //(Denormal Fix)

		public static void init_3band_state(ref EQSTATE  es, int lowfreq, int highfreq, int mixfreq)
		{
			//// Clear state 
			//memset(es, 0, sizeof(EQSTATE));

			es.lg = 1.0;
			es.mg = 1.0;
			es.hg = 1.0;

			// filter cutoff frequencies

			es.lf = 2 * Math.Sin(Math.PI * ((double)lowfreq / (double)mixfreq));
			es.hf = 2 * Math.Sin(Math.PI * ((double)highfreq / (double)mixfreq));
		}

		public static double do_3band(ref EQSTATE  es, double sample)
		{
			double l, m, h;

			//  (lowpass)

			es.f1p0 += (es.lf * (sample - es.f1p0)) + vsa;
			es.f1p1 += (es.lf * (es.f1p0 - es.f1p1));
			es.f1p2 += (es.lf * (es.f1p1 - es.f1p2));
			es.f1p3 += (es.lf * (es.f1p2 - es.f1p3));

			l = es.f1p3;

			// (highpass)

			es.f2p0 += (es.hf * (sample - es.f2p0)) + vsa;
			es.f2p1 += (es.hf * (es.f2p0 - es.f2p1));
			es.f2p2 += (es.hf * (es.f2p1 - es.f2p2));
			es.f2p3 += (es.hf * (es.f2p2 - es.f2p3));

			h = es.sdm3 - es.f2p3;

			//   midrange (signal - (low + high))

			m = es.sdm3 - (h + l);

			// Scale, Combine and store

			l *= es.lg;
			m *= es.mg;
			h *= es.hg;

			// Shuffle history buffer 

			es.sdm3 = es.sdm2;
			es.sdm2 = es.sdm1;
			es.sdm1 = sample;

			// Return result

			return (l + h + m);
		}
    }
}
