#define _USE_MATH_DEFINES  
#include <cmath>
#define _USE_MATH_DEFINES 
#include <math.h>
#include <iostream>
#include <windows.h>
#pragma comment( lib, "gdiplus.lib" )
#include <gdiplus.h>
#include "ImageProc.h"
using namespace Gdiplus;
namespace KorneaDSP
{
	int GetEncoderClsid(const WCHAR* format, CLSID* pClsid)
	{
		UINT  num = 0;          // number of image encoders
		UINT  size = 0;         // size of the image encoder array in bytes

		ImageCodecInfo* pImageCodecInfo = NULL;

		GetImageEncodersSize(&num, &size);
		if (size == 0)
			return -1;  // Failure

		pImageCodecInfo = (ImageCodecInfo*)(malloc(size));
		if (pImageCodecInfo == NULL)
			return -1;  // Failure

		GetImageEncoders(num, size, pImageCodecInfo);

		for (UINT j = 0; j < num; ++j)
		{
			if (wcscmp(pImageCodecInfo[j].MimeType, format) == 0)
			{
				*pClsid = pImageCodecInfo[j].Clsid;
				free(pImageCodecInfo);
				return j;  // Success
			}
		}

		free(pImageCodecInfo);
		return -1;  // Failure
	}

	void FilterBW(Bitmap * src, Bitmap * dst)
	{
		int            w, h;
		w = src->GetWidth();
		h = src->GetHeight();

		for (int y = 0; y < h; y++) {
			for (int x = 0; x < w; x++) {
				Color c;
				src->GetPixel(x, y, &c);
				
				//BYTE nc =( c.GetBlue + c.GetRed + c.GetGreen) / 3;
				//Color * dc = new Color(nc, nc, nc);
				dst->SetPixel(x, y,  c);
			}
		}
	}

	//Bitmap OpenImage (WCHAR  * inpath)
	//{
	

	//	Bitmap*    pBitmapIn = new Bitmap(inpath);

	/*	Bitmap*    pBitmapOut = new Bitmap(pBitmapIn->GetWidth(), pBitmapIn->GetHeight(), PixelFormat32bppRGB);

		Filter(pBitmapIn, pBitmapOut);

		CLSID   encoderClsid;
		GetEncoderClsid(L"image/jpg", &encoderClsid);
		pBitmapOut->Save(L"output.jpg", &encoderClsid, 0);

		GdiplusShutdown(gdiplusToken);*/
//		return  * pBitmapIn;
	//}

	//----------------------------------------------------------------------------
}

