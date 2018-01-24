#include <stdexcept>
using namespace std;

namespace KorneaDSP
{
 

#ifndef __EQ3BAND__
#define __EQ3BAND__

	typedef struct
	{
		// Filter #1 (Low band)

		double  lf;       // Frequency
		double  f1p0;     // Poles ...
		double  f1p1;
		double  f1p2;
		double  f1p3;

		// Filter #2 (High band)

		double  hf;       // Frequency
		double  f2p0;     // Poles ...
		double  f2p1;
		double  f2p2;
		double  f2p3;

		// Sample history buffer

		double  sdm1;     // Sample data minus 1
		double  sdm2;     //                   2
		double  sdm3;     //                   3

		// Gain Controls

		double  lg;       // low  gain
		double  mg;       // mid  gain
		double  hg;       // high gain

	} EQSTATE;

	extern "C" { __declspec(dllexport) void   init_3band_state(EQSTATE* es, int lowfreq, int highfreq, int mixfreq); }
	extern "C" { __declspec(dllexport)  double do_3band(EQSTATE* es, double sample); }


#endif 

}