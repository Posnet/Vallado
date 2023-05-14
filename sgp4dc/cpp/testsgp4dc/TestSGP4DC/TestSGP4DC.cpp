/* ---------------------------------------------------------------------
*
*                              testSGP4DC.cpp
*
*  this program tests the sgp4 differential correction. 
*
*                          companion code for
*             fundamentals of astrodynamics and applications
*                                  2013
*                            by david vallado
*
*     (w) 719-573-2600, email dvallado@agi.com, davallado@gmail.com
*     *****************************************************************
*  current :
*             3 nov 14  david vallado
*                        conversion to msvs c++
*  changes :
*             8 jul 08  david vallado
*                        original version for 2008 paper
*
*   cout << "argc= " << _argc << endl;
*
*   for (int i = 0; i < _argc; ++i)
*      cout << _argv[i] << endl;
*   typeans    = *_argv[1];
*   typeodrun  = *_argv[2];
*   statetype  = *_argv[3];
*   percentchg = atof(_argv[4]);    // 0.001
*   deltaamtchg= atof(_argv[5]);    // 0.0000001
*   rmsepsilon = atof(_argv[6]);    // 0.001
*   strcpy( fname, _argv[7]);
*   lastob     = atof(_argv[8]);    // 200
*   statesize  = atof(_argv[9]);    // 6 or 7
*
*       ----------------------------------------------------------------      */


//#include "stdafx.h"

#include <math.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <fstream>
#include <iostream>
#include <sstream>
#include <algorithm> 
#include <cctype>
#include <locale>

#include "D:/Codes/LIBRARY/CPP/Libraries/MathTimeLib/MathTimeLib/MathTimeLib.h"  // edirection, globals, re, mu, jpldedata, equintype
#include "D:/Codes/LIBRARY/CPP/Libraries/AstroLib/AstroLib/AstroLib.h"  // eopt, jpldedata, equintype
#include "D:/Codes/LIBRARY/CPP/Libraries/eopspw/eopspw/eopspw.h"  // eopdata, spwdata, eopsize, spwsize
//#include "D:/Codes/LIBRARY/CPP/Libraries/astPert/astPert/astPert.h"
#include "D:/Codes/LIBRARY/CPP/Libraries/SGP4/SGP4/SGP4.h"  // gravconsttype, elsetrec, SGP4Version
#include "D:/Codes/LIBRARY/CPP/Libraries/SGP4DC/SGP4DC/SGP4DC.h"  // obsrec, senrec, WGS72MU, SGP4DCVersion


// local routines in the main file
void processrazel
(
	char batchmode, char proptype, int derivType, double cdam, double cram,
	iau80data iau80rec,
	const std::vector<eopdata> eoparr,
	double jdeopstart, double jdeopstartf,
	const std::vector<spwdata> spwarr,
	double jdspwstart, double jdspwstartf,
	const std::vector<jpldedata> jpldearr,
	char jplopt,
	double jdjpldestart,
	double 	jdepoch, double jdepochf, char interp,
	int firstob, int obsskip, int obsread,
	std::vector<obsrec> &obsarr,
	std::vector<double> &xnom
);

void processxyz
(
	char batchmode, char proptype, int derivType, double cdam, double cram,
	iau80data iau80rec,
	const std::vector<eopdata> eoparr,
	double jdeopstart, double jdeopstartf,
	const std::vector<spwdata> spwarr,
	double jdspwstart, double jdspwstartf,
	const std::vector<jpldedata> jpldearr,
	char jplopt,
	double jdjpldestart,
	double 	jdepoch, double jdepochf, char interp,
	int firstob, int obsskip, int obsread,
	std::vector<obsrec> &obsarr,
	std::vector<double> &xnom
);

void processtle
(
	char batchmode, char proptype, int derivType, double cdam, double cram,
	iau80data iau80rec,
	const std::vector<eopdata> eoparr,
	double jdeopstart, double jdeopstartf,
	const std::vector<spwdata> spwarr,
	double jdspwstart, double jdspwstartf,
	const std::vector<jpldedata> jpldearr,
	char jplopt,
	double jdjpldestart,
	double 	jdepoch, double jdepochf, char interp,
	int firstob, int obsskip, int obsread,
	std::vector<obsrec> &obsarr,
	std::vector<double> &xnom
);

void readobs
(
	char filename[85],
	iau80data iau80rec,
	const std::vector<eopdata> eoparr,
	double jdeopstart, double jdeopstartf,
	char interp,
	char proptype,
	double &jdepoch, double &jdepochf,
	std::vector<obsrec> &obsarr,
	int firstob, int obsskip, int obsread,
	std::vector<double> &xnom
);


/*       ----------------------------------------------------------------     */

//int _tmain(int argc, _TCHAR* argv[])
//int main(array<System::String ^> ^args)
int main()
{
	// command line arguments
	char batchmode, opsmode, typeans, typeodrun, proptype;
	int derivType, firstob, obsskip, obsread;
	double cdam, cram, jdutc, jdutcf, jdepoch, jdepochf;
	double rad = 180.0 / pi;
	char fname[166];
	char interp = 's';  // 'l' linear,  's' spline for jplde
	// derivType = 1 = 2 - body
	// derivType = 2 = J2
	// derivType = 3 = J3
	// derivType = 4 = J4
	// derivType = 5 = Jx
	// derivType = 6 = drag
	// derivType = 7 = 3 - body
	// derivType = 8 = srp
	// derivType = 10 = all
	// derivtype = 11 - srp semianalytical
	// derivtype = 12 - 3body semianalytical
	// derivtype = 13 - zonal semianalytical
	// interp - interpolation for sun / moon vectors 'l', 's'
	// opt - method of calc sun / moon        'a', 'j'
	derivType = 1; 
	
	std::vector<obsrec> obsarr;
	std::vector<double> xnom = std::vector< double >(7);
	double rosc[3], vosc[3], dat, dut1;	// ICRF osculating (in STK scenario HPOP objects)

	printf(" =============================   test SGP4DC =============================\n");
	
	// -------------------- initialize all the EOP data once at the beginning -------------------
	// initialize FK5 nutation values
	std::string Nut80Loc = "D:/Codes/LIBRARY/DataLib/";
	std::string Nut00Loc = "D:/Codes/LIBRARY/DataLib/";  // all other files are here
	iau80data iau80arr;
	EopSpw::iau80in(Nut80Loc, iau80arr);

	// initialize EOP file
	double jdeopstart, jdeopstartf;
	std::vector<eopdata> eoparr;
	EopSpw::readeop(eoparr, "../EOP-All-v1.1_2020-07-01.txt", jdeopstart, jdeopstartf);
	                            
	// initialize SPW file
	double jdspwstart, jdspwstartf;
	std::vector<spwdata> spwarr;
	EopSpw::readspw(spwarr, "../SpaceWeather-All-v1.2_2020-07-01.txt", jdspwstart, jdspwstartf);

	// initialize the JPL DE ephemerides
	char jplopt = 'j';  // jpl ephemerides for sun and moon, 'a' analytical
	double jdjpldestart, jplpldestartf;
	std::vector<jpldedata> jpldearr;
	jpldearr.resize(jpldesize);
	AstroLib::initjplde(jpldearr, "../sunmooneph_430t.txt", jdjpldestart, jplpldestartf);

	interp = 's';  // 'l' linear,  's' spline for jplde
	cram = 0.02;    // mass = 100 kg, area = 2 m^2
	cdam = 0.044;   // cr = 1.0, cd = 2.2


	// ------ setup run
	batchmode = 'n';   // set this to 'n' for debugging mode in the compiler
	typeans = 'b';     // bksub or svd b, s

	// ephemeris to tle
	//typeodrun = 't';   
	// tle to ephemeris to tle (sgp4)
	//typeodrun = 'x';
	// range az el obs to vector (book example)
    typeodrun = 'r';

	proptype = 'n';    // semianalytic 's,' numerical 'n', twobody 't'
	jplopt = 'j';      // use interp of jpl ephemerides for Sun/Moon
	derivType = 1;     // 1-2body, 2-j2, 7-3body, 8-srp, 11 srp sa, 12 3bd sa, 13 zon sa

	// sgp4 options
	//opsmode = 'a' best understanding of how afspc code works
	//opsmode = 'i' improved sgp4 resulting in smoother behavior, will NOT match tests
	opsmode = 'a'; //afspc mode, or improved mode


	if (batchmode == 'y')
	{
		// read inputs from command line
		//         func(); // func can get the program arguments from the command line and
		// send them into global variables.
	}
	else
	{
		typeans = 'b';
		//printf("input typeans bksub or svd b, s \n\n");
		//typeans = getchar();
		//fflush(stdin);
		//printf("input type of run razel, xyz, tle r, x, t, \n\n");
		//typeodrun = getchar();
		//fflush(stdin);
		printf("input proptype s-sgp4, a-semianalytical, n-numerical, t-2body, \n\n");
			
		proptype = getchar();
		fflush(stdin);
		if (proptype == 'n')
		{
			printf("input derivtype 1-2body, 2-j2, 7-3body, 8-srp, 11 srp sa, 12 3bd sa, 13 zon sa \n\n");
			scanf_s("%i", &derivType);
		}

		firstob = 1;        // first obs to read
		//printf("input first ob to read (1, ...) \n");
		//scanf_s("%i", &firstob);
		firstob = 1;

		obsskip = 1;        // number of steps to skip between reads
		printf("input obs to skip in between (1, ...) \n");
		scanf_s("%i", &obsskip);

		obsread = 1;        // total obs to read at the obsskip
		printf("input total obs to read (100, .. 5000) \n");
		scanf_s("%i", &obsread);
	}

	// ----- output setup
	printf("\n%s\n", SGP4Version);
	printf("%s\n", SGP4DCVersion);
	printf("opsmode    %c  a afspc, i improved \n", opsmode);
	printf("typeans    %c  b bksbd, s svd \n", typeans);
	printf("typeodrun  %c  r razel, x xyz, t tle \n", typeodrun);
	printf("proptype   %c  s-sgp4, a-semianalytical, n-numerical, t-2body \n", proptype);
	printf("derivtype  %i  1-2body, 2-j2, 7-3body, 8-srp, 11 srp sa, 12 3bd sa, 13 zon sa \n", derivType);
	printf("firstob to read %i \n", firstob);
	printf("obsskip to read %i \n", obsskip);
	printf("obsread to skip  %i \n", obsread);


	switch (typeodrun)
	{
	// ---------------------- obs range az and el ---------------------
	case 'r': 
		// book example
		strcpy_s(fname, "../geos6a.inp");
		xnom[0] = 5975.29040000;  // eci
		xnom[1] = 2568.64000000;
		xnom[2] = 3120.58450000;
		xnom[3] = 3.98384600;
		xnom[4] = -2.07115900;
		xnom[5] = -5.91709500;

		//ttt = (jdepoch + jdepochf - 2451545.0) / 36525.0;
		//AstroLib::teme_eci(rnomTEME, vnomTEME, ateme, MathTimeLib::eFrom, recin, vecin, aeci, iau80rec, opt, ttt, 0.0, 0.0);

		MathTimeLib::jday(1995, 1, 29, 2, 38, 37.0000000, jdutc, jdutcf);
		jdepoch = jdutc;
		jdepochf = jdutcf;
		//latgd = 21.572056 / rad;   // AAS paper values
		//lon = -158.266578 / rad;
		//alt = 0.3002;  // km
    	dat = 29;  // assume the EOP values stay the same throughout the interval
		dut1 = 0.3261068;  // s
		
		processrazel(batchmode, proptype, derivType, cdam, cram, iau80arr, eoparr, jdeopstart, jdeopstartf, spwarr, jdspwstart, jdspwstartf,
				jpldearr, jplopt, jdjpldestart, jdepoch, jdepochf, interp, firstob, obsskip, obsread, obsarr, xnom);
		break;
	// ---------------------------- ephemeris -------------------------
	case 'x': 
//		strcpy_s(fname, "../GEOSat3BodyHPOP11.e");
//		strcpy_s(fname, "../GEOSatSRPHPOP11.e");

		// ------ setup satellite and obs, semi analytical/numerical test case
		// GEO xyz example
		MathTimeLib::jday(2017, 5, 11, 3, 51, 42.7657, jdutc, jdutcf);
		jdepoch = jdutc;
		jdepochf = jdutcf;

		xnom[0] = -25922.11316919;
		xnom[1] = -33253.50739001;
		xnom[2] = -24.98335897;
		xnom[3] = 2.425002428;
		xnom[4] = -1.890408793;
		xnom[5] = 0.001567493;
			   
		// GPS ephemeris xyz example
		strcpy_s(fname, "../gpsd6n22.e");  
		MathTimeLib::jday(2006, 10, 15, 23, 59, 47.0000000, jdutc, jdutcf);
		jdepoch = jdutc;
		jdepochf = jdutcf;
	
		// earth fixed:
		xnom[0] = -11623.97465000;
		xnom[1] = 23865.86794700;
		xnom[2] = -48.20044600;
		xnom[3] = -0.265442426;
		xnom[4] = -0.157132463;
		xnom[5] = -3.160974175;

		dat = 33;
		dut1 = 0.1266898;  // note that c++ will interpolate this, so values will differ

		if (proptype == 's')
		{
			// set these later when read in obs
			//jdepoch = satrec.jdepoch;
			//jdepochf = satrec.jdepochf;
		}
		else
		{
			jdepoch = jdutc;
			jdepochf = jdutcf;
		}

		readobs(fname, iau80arr, eoparr, jdeopstart, jdeopstartf, interp, proptype, jdepoch, jdepochf, obsarr, firstob, obsskip, obsread, xnom );
		processxyz(batchmode, proptype, derivType, cdam, cram, iau80arr, eoparr, jdeopstart, jdeopstartf, spwarr, jdspwstart, jdspwstartf, 
			jpldearr, jplopt, jdjpldestart, jdepoch, jdepochf, interp, firstob, obsskip, obsread, obsarr, xnom);
		break;

	// ---------------------------- tle, form ephemeris, then try to refind original ----------------------------
	case 't': processtle(batchmode, proptype, derivType, cdam, cram, iau80arr, eoparr, jdeopstart, jdeopstartf, spwarr, jdspwstart, jdspwstartf,
		jpldearr, jplopt, jdjpldestart, jdepoch, jdepochf, interp, firstob, obsskip, obsread, obsarr, xnom);
		break;
	}
}  // main



/*       ----------------------------------------------------------------     */
// read obs from an ephemeris file

void readobs
(
	char filename[85],
	iau80data iau80rec,
	const std::vector<eopdata> eoparr,
	double jdeopstart, double jdeopstartf,
	char interp,
	char proptype,
	double &jdepoch, double &jdepochf,
	std::vector<obsrec> &obsarr,
	int firstob, int obsskip, int obsread,
	std::vector<double> &xnom
)
{
	FILE *infile;
	int j, ii, kk;
	int year, mon, day, hr, min, timezone, dat;
	char longstr1[175], longstr[175], coords[30];
	char strr[4], monstr[5], strr1, strr2, strr3;
	char tmp[20], tmpc, tmpc1, units[10];
	double dtsec, dtseczero, conv, sec, mfme;
	double dut1, lod, xp, yp, ddpsi, ddeps, iaudx, dy, icrsx, y, s,
		ut1, tut1, jdut1, jdut1f, utc, tai, tt, ttt, jdtt, jdttf, tcg, tdb, ttdb, jdtdb, jdtdbf, tcb,
		p, a, ecc, incl, raan, argp, nu, m, eccanom, arglat, truelon, lonper;
	double rpos[3], vpos[3], apos[3],  reci[3], veci[3], aeci[3],
		dtmin, periodmin;
	elsetrec satrec;
	elsetrec satrecx;
	std::vector< std::vector<double> > trans = std::vector< std::vector<double> >(3, std::vector<double>(3));
	int sttrlen = 185;

	obsrec currobsrec;
	obsarr.resize(5000);


	conv = 0.001;
	timezone = 0;


	// --------------------------------------------------------------------------------------
	int err = fopen_s(&infile, filename, "r");

	if (infile == NULL)
	{
		printf("Failed to open %s ", filename);
	}
	else
	{
		conv = 1.0;  // set for km in case sscan doesn't work
		conv = 0.001;
		strcpy_s(units, "Meters"); // default

		// ---------------------- search the header for information -----------------------------
		do
		{
			fgets(longstr1, sttrlen, infile);
			//removeWS(longstr1);
			strr1 = char(longstr1[0]);
			strr3 = char(longstr1[4]);
			if (strr1 == 'D' || strr3 == 'D')   // search for units
			{
				std::string ExampleString(longstr1);
				std::istringstream yearDayMonthHourStringStream(ExampleString);
				yearDayMonthHourStringStream >> tmp >> units;

				if (strcmp(units, "Kilometers") == 0)
					conv = 1.0;
				else
					conv = 0.001;
			}
			if (strr1 == 'S' || strr3 == 'S')   // search for epoch time position
			{
#ifdef _MSC_VER
				sscanf_s(longstr1, "%s %i %s %i %3i %c %2i %c %lf ",
					&tmp, sizeof(tmp), &currobsrec.day, &monstr, sizeof(3), &currobsrec.year, &currobsrec.hr, &tmpc, 1,
					&currobsrec.min, &tmpc1, 1, &currobsrec.sec);
#else
				scanf(longstr1, "%14s %i %4s %i %3i %1c %2i %1c %lf ",
					&tmp, &currobsrec.day, &monstr, &currobsrec.year, &currobsrec.hr, &tmpc,
					&currobsrec.min, &tmpc1, &currobsrec.sec);
#endif
				currobsrec.mon = MathTimeLib::getmon(monstr);
				MathTimeLib::jday(currobsrec.year, currobsrec.mon, currobsrec.day,
					currobsrec.hr, currobsrec.min, currobsrec.sec, satrec.jdsatepoch, satrec.jdsatepochF);
				currobsrec.jd = satrec.jdsatepoch;
				currobsrec.jdf = satrec.jdsatepochF;
				jdepoch = satrec.jdsatepoch;
				jdepochf = satrec.jdsatepochF;
			}
			if (strr1 == 'C' || strr3 == 'C')    // search for coordinate system
			{
				strr2 = char(longstr1[5]);
				if ((char(longstr1[2]) == 'o') || (strr2 == 'o'))
				{
					//std::string coord;
#ifdef _MSC_VER
					sscanf_s(longstr1, "%s %s ", &tmp, sizeof(tmp), &coords, sizeof(coords));
#else
					scanf(longstr1, "%s %s ", &tmp, &coords);
#endif
					// dav move the eop initialization
					//              findatmosparam( jde, mfme, interp, fluxtype, f81type, inputtype, spwarr, jdspwstart,
					//                f107, f107bar, ap, avgap, aparr, kp, sumkp, kparr );
					//                              sethelp(iauhelp,'n');
					timezone = 0;
				}
			}

		} while ((strr1 != 'E' && strr3 != 'E') && (feof(infile) == 0));

		fgets(longstr1, sttrlen, infile);  // blank line

	}


	// ---------------------- search the file for observations -----------------------------
    // ---- read more than the points for the fit span to permit prediction analysis if possible
	ii = 0;
	while ((feof(infile) == 0) && (ii < obsread))   // read more in for comparison later
	{
		fgets(longstr, sttrlen, infile);
		strncpy_s(strr, &longstr[0], 1);
		strr[1] = '\0';
		if (strcmp(strr, "E") != 0)
		{
			sscanf_s(longstr, "%lf %lf %lf %lf %lf %lf %lf ",
				&dtsec, &currobsrec.x, &currobsrec.y, &currobsrec.z, &currobsrec.xdot, &currobsrec.ydot, &currobsrec.zdot);

			rpos[0] = currobsrec.x*conv;   // get inputs in km
			rpos[1] = currobsrec.y*conv;
			rpos[2] = currobsrec.z*conv;
			vpos[0] = currobsrec.xdot*conv;
			vpos[1] = currobsrec.ydot*conv;
			vpos[2] = currobsrec.zdot*conv;

			if (ii == 0) // fix the jd epoch in case it starts later than 0.0 sec
			{
				dtseczero = dtsec;  // assumes epoch point is the first in the file
				MathTimeLib::invjday(currobsrec.jd, currobsrec.jdf + dtsec / 86400.0, year, mon, day, hr, min, sec);
				MathTimeLib::jday(year, mon, day, hr, min, sec, jdepoch, jdepochf);
			}

			MathTimeLib::invjday(jdepoch, jdepochf + (dtsec - dtseczero) / 86400.0, year, mon, day, hr, min, sec);
			currobsrec.year = year;
			currobsrec.mon = mon;
			currobsrec.day = day;
			currobsrec.hr = hr;
			currobsrec.min = min;
			currobsrec.sec = sec;
			MathTimeLib::jday(year, mon, day, 0, 0, 0.0, currobsrec.jd, currobsrec.jdf);
			mfme = hr * 60 + min + sec / 60.0;

			// convert coordinate system as necessary
			// obs need to be in eci
			eOpt opt = e80;
			if ((strcmp(coords, "J2000") == 0) || (strcmp(coords, "MeanOfEpoch") == 0))
			{
				EopSpw::findeopparam(currobsrec.jd, currobsrec.jdf, 's', eoparr, jdeopstart, dut1, dat, lod, xp, yp, ddpsi, ddeps,
					iaudx, dy, icrsx, y, s);
				MathTimeLib::convtime(year, mon, day, hr, min, sec, timezone, dut1, dat,
					ut1, tut1, jdut1, jdut1f, utc, tai, tt, ttt, jdtt, jdttf, tcg, tdb, ttdb, jdtdb, jdtdbf, tcb);
				//coordFK5::itrf_gcrf(ritrf, vitrf, aitrf, eFrom, rpos, vpos, apos, iau80rec, ttt, jdut1, lod, xp, yp, 2, ddpsi, ddeps, deltapsi, deltaeps, trans);
				//coordFK5::teme_ecef(rteme, vteme, ateme, eTo, ritrf, vitrf, aitrf, ttt, jdut1, lod, xp, yp, eqeterms);
				AstroLib::mod_gcrf(rpos, vpos, apos, MathTimeLib::eTo, reci, veci, aeci, ttt);
			}
			else
				if (strcmp(coords, "Fixed") == 0)
				{
					EopSpw::findeopparam(currobsrec.jd, currobsrec.jdf, 's', eoparr, jdeopstart, dut1, dat, lod, xp, yp, ddpsi, ddeps,
						iaudx, dy, icrsx, y, s);
					MathTimeLib::convtime(year, mon, day, hr, min, sec, timezone, dut1, dat,
						ut1, tut1, jdut1, jdut1f, utc, tai, tt, ttt, jdtt, jdttf, tcg, tdb, ttdb, jdtdb, jdtdbf, tcb);
					AstroLib::itrf_gcrf(rpos, vpos, apos, MathTimeLib::eTo, reci, veci, aeci, iau80rec, opt, ttt, jdut1, lod, xp, yp, 2, ddpsi, ddeps, trans);
					//coordFK5::teme_ecef( rteme, vteme, ateme, eTo,rpos, vpos, apos, ttt, jdut1, lod, xp, yp, eqeterms);
				}
				else
				{
					for (j = 0; j < 3; j++)
					{
						reci[j] = rpos[j];
						veci[j] = vpos[j];
					}
				}

			// now reset the obs vector to be eci, (teme?)
			currobsrec.x = reci[0];
			currobsrec.y = reci[1];
			currobsrec.z = reci[2];
			currobsrec.xdot = veci[0];
			currobsrec.ydot = veci[1];
			currobsrec.zdot = veci[2];

			// setup the initial satrec TLE estimate if sgp4 used 
			// this will be the nominal 
			if (ii == 0 && proptype == 's')
			{
				AstroLib::rv2coe(reci, veci, p, a, ecc, incl, raan, argp, nu, m, eccanom, arglat, truelon, lonper);
				satrec.no_kozai = 60.0 * sqrt(3.986008e5 / (a * a * a));  // rad per min, sgp4 wgs-72 mu value

				strncpy_s(satrec.satnum, "55555", 5 * sizeof(char));  // set some number here
				strncpy_s(satrec.intldesg, "testsat", 11);
				satrec.classification = 'U';

				// set the time span based on the period size,
				// but have to calculate on second iteration when dtsec is known
				// so calc here before satrec.a changes on the 1st iteration
				periodmin = (2.0 * pi * sqrt(pow(a, 3) / 398600.8)) / 60.0;  // minutes
				satrec.a = a / 6378.135;  // ER

				satrec.ecco = ecc;
				satrec.inclo = incl;
				satrec.nodeo = raan;
				satrec.argpo = argp;
				satrec.mo = m;
				if (year >= 2000)
					satrec.epochyr = year - 2000;
				else
					satrec.epochyr = year - 1900;
				MathTimeLib::finddays(year, mon, day, hr, min, sec, satrec.epochdays);
				satrecx = satrec;    // store the answer for later
			}  // if ii = 0
			else
			{
				if (ii == 0)
				{
					for (j = 0; j < 3; j++)
					{
						xnom[j] = reci[j];
						xnom[j + 3] = veci[j];
					}
				}
			}

			if (ii == 1)
			{
				// lastob = int(lastob / (dtsec / 60.0) * .5 * int( periodmin/ lastob )); // reset this based on the ephemeris file spacing
				// lastob = 200; // force this/. 901 is at 43 sec, 906 is at 15 min, ice is at 30 sec, gps is at 15 min
				//printf(" %3i with spacing of %lf min is %lf min \n", lastob, (dtsec - dtseczero) / 60.0, lastob*(dtsec - dtseczero) / 60.0);
			}

			dtmin = (dtsec - dtseczero) / 60.0;  // in min
			currobsrec.dtmin = dtmin;
			currobsrec.sennum = 1;
			currobsrec.satnum = 55555;
			currobsrec.obstype = 4;  // xyz obs
			obsarr[ii] = currobsrec;

			ii = ii + 1;   // increase the obs by one even if obsstep is different

			if (fmod(ii, 500) == 0)
				printf("over 500 obs \n");
		}   // if strr

		// reread any additional steps if needed
		if (obsskip > 1)
		{
			for (kk = 1; kk < obsskip; kk++)
			{
				if (feof(infile) == 0)
					fgets(longstr, sttrlen, infile);
			}
		}

	} // while ii through producing the observation vectors

	fclose(infile);

}  // readobs





/*       ----------------------------------------------------------------     */
void processrazel
(
	char batchmode, char proptype, int derivType, double cdam, double cram,
	iau80data iau80rec,
	const std::vector<eopdata> eoparr,
	double jdeopstart, double jdeopstartf,
	const std::vector<spwdata> spwarr,
	double jdspwstart, double jdspwstartf,
	const std::vector<jpldedata> jpldearr,
	char jplopt,
	double jdjpldestart,
	double 	jdepoch, double jdepochf, char interp,
	int firstob, int obsskip, int obsread,
	std::vector<obsrec> &obsarr,
	std::vector<double> &xnom
)
{
	FILE *infile, *outfile, *outfile1;
	std::vector<obsrec> obsrecarr;
	elsetrec satrec;
	long search_satno = 0;
	char longstr1[140], longstr2[140];
	char strr[2];
	char coords[10];
	char typerun;
	gravconsttype whichconst;
	char  statetype, typeans, opsmode;
	int statesize;
	char fname[160];
	double percentchg, deltaamtchg, rmsepsilon;

	int day, mon, year, hr, min, loops, i;
	double conv, sec;
	obsrecarr.resize(5000);

	double  dut1, ut1, tut1, jdut1, jdut1f, utc, tai, tt, ttt, jdtt, jdttf, tcg, tdb,
		ttdb, jdtdb, jdtdbf, tcb, mfme, lod, xp, yp, ddpsi, ddeps, iaudx, dy, icrsx, y, s;
	char keepit;
	int timezone, dat, ktr;
	//     std::vector< std::vector<double> > trans(3,3);
	std::vector< std::vector<double> > trans = std::vector< std::vector<double> >(3, std::vector<double>(3));

	double rngest, zrng, zaz, zel, zrtasc, zdecl,
		theta, theta1, copa, rsum[3], vsum[3];
	char error[15];
	int ii, j, skipped, eqeterms;
	double rpos[3], vpos[3], rnomTEME[3], vnomTEME[3], dr[3], dv[3], r1[3], r2[3], r3[3], v1[3], v2[3], v3[3],
		rteme[3], vteme[3], ateme[3], aitrf[3], dtmin, jd, jdf,
		r1teme[3], v1teme[3], r2teme[3], v2teme[3], r3teme[3], v3teme[3];
	obsrec currobsrec;
	senrec currsenrec;
	double rad = 180.0 / pi;
	double dayconv = 1.0 / 86400.0;

	eqeterms = 2;

	statesize = 6;
	if (batchmode == 'n') // needs a default value here in batch mode
		statesize = 7;
	// set these variables up with intial dimensions
	// these must be set here
	// function matrices can be resized
	std::vector< std::vector<double> > x = std::vector< std::vector<double> >(statesize, std::vector<double>(1));
	std::vector< std::vector<double> > dx = std::vector< std::vector<double> >(statesize, std::vector<double>(1));
	std::vector< std::vector<double> > atwai = std::vector< std::vector<double> >(statesize, std::vector<double>(statesize));
	std::vector< std::vector<double> > atwa = std::vector< std::vector<double> >(statesize, std::vector<double>(statesize));
	std::vector< std::vector<double> > atwb = std::vector< std::vector<double> >(statesize, std::vector<double>(1));

	whichconst = wgs72; // type of run for sgp4
	typerun = 'c';   // c catalog (just reads tle), v verification, m manual
	opsmode = 'a';
	typeans = 's';
	interp = 'l'; // set eop interpolation to linear
	// printf("input interp n l s \n\n");

	//     sethelp(iauhelp,'n');
	timezone = 0;

	if (batchmode == 'n')
	{
		statetype = 't';   // v vectors, t tle vars, e equinoctial
		printf("input type of elements tle or equin \n\n");
		statetype = getchar();
		fflush(stdin);
		strcpy_s(fname, "../geos6a.inp");
		printf("input obs filename: \n");
		//scanf_s("%s", &fname, sizeof(fname));

		percentchg = 0.001;  // percent to change components for finite differencing
		printf("input percentchg for finite differencing (.001) \n");
		//scanf_s("%lf", &percentchg);

		deltaamtchg = 0.0000001;  // delta amt to change components for finite differencing
		printf("input deltaamtchg to chk in finite differencing (.0000001) \n");
		//scanf_s("%lf", &deltaamtchg);

		rmsepsilon = 0.0002;  // percent to change components for finite differencing
		printf("input rmsepsilon to quit (.02) \n");
		//scanf_s("%lf", &rmsepsilon);

		statesize = 7;        // solve for bstar or not
		printf("input statesize (6 or 7) \n");
		//scanf_s("%i", &statesize);
	}

	printf("typerun     %c c catalog (just reads tle), v verification, m manual \n", interp);
	printf("interp type %c eop interpolation, l linear, s spline \n", interp);
	printf("state type  %c v vectors, t tle vars, e equinoctial \n", statetype);
	printf("fname       %s \n", fname);
	printf("percentchg  %lf \n", percentchg);
	printf("deltaamtchg %11.8lf \n", deltaamtchg);
	printf("rmsepsilon  %lf \n", rmsepsilon);
	printf("state size  %i \n", statesize);

	int err = fopen_s(&infile, fname, "r");
	err = fopen_s(&outfile, "testdc.out", "w");
	err = fopen_s(&outfile1, "testdc1.out", "w");

	fprintf(outfile, "typeans %c \n", typeans);

	if (infile == NULL)
	{
		printf("Failed to open %s ", fname);  // , strerror(errno)
	}
	else
	{
		conv = 0.001;
		// read header lines
		do
		{
			fgets(longstr1, 91, infile);
			strncpy_s(strr, &longstr1[0], 1);
			strr[1] = '\0';
		} while ((strcmp(strr, "#") == 0) && (feof(infile) == 0));

		// ----------------------- read obs from the input file -------------------------
		strcpy_s(coords, "Fixed");  // assume earth fixed observations unless specified otherwise
		ii = 0;
		// limit to obsread?/?????????
		while (feof(infile) == 0)
		{
			if (ii != 0)
				fgets(longstr2, 120, infile);
			else
				strcpy_s(longstr2, longstr1);  // data for the 1st point
			strncpy_s(strr, &longstr2[0], 1);
			strr[1] = '\0';

			if (strcmp(strr, "#") != 0)
			{
				sscanf_s(longstr2, "%d", &currobsrec.obstype);
				switch (currobsrec.obstype)
				{
				case 0:
				{
					sscanf_s(longstr2, "%d %d %d %d %d %d %d %d %lf %lf ",
						&currobsrec.obstype, &currobsrec.satnum, &currobsrec.sennum,
						&currobsrec.year, &currobsrec.mon, &currobsrec.day,
						&currobsrec.hr, &currobsrec.min, &currobsrec.sec,
						&currobsrec.rng);
				}
				break;
				case 1:
				{
					sscanf_s(longstr2, "%d %d %d %d %d %d %d %d %lf %lf %lf ",
						&currobsrec.obstype, &currobsrec.satnum, &currobsrec.sennum,
						&currobsrec.year, &currobsrec.mon, &currobsrec.day,
						&currobsrec.hr, &currobsrec.min, &currobsrec.sec,
						&currobsrec.az, &currobsrec.el);
					currobsrec.az = currobsrec.az / rad;
					currobsrec.el = currobsrec.el / rad;
				}
				break;
				case 2:
				{
					sscanf_s(longstr2, "%d %d %d %d %d %d %d %d %lf %lf %lf %lf ",
						&currobsrec.obstype, &currobsrec.satnum, &currobsrec.sennum,
						&currobsrec.year, &currobsrec.mon, &currobsrec.day,
						&currobsrec.hr, &currobsrec.min, &currobsrec.sec,
						&currobsrec.rng, &currobsrec.az, &currobsrec.el);
					currobsrec.az = currobsrec.az / rad;
					currobsrec.el = currobsrec.el / rad;
				}
				break;
				case 3:
				{
					sscanf_s(longstr2, "%d %d %d %d %d %d %d %d %lf %lf %lf ",
						&currobsrec.obstype, &currobsrec.satnum, &currobsrec.sennum,
						&currobsrec.year, &currobsrec.mon, &currobsrec.day,
						&currobsrec.hr, &currobsrec.min, &currobsrec.sec,
						&currobsrec.trtasc, &currobsrec.tdecl);
					currobsrec.trtasc = currobsrec.trtasc / rad;
					currobsrec.tdecl = currobsrec.tdecl / rad;
				}
				break;
				} // case obstype

				year = currobsrec.year;
				mon = currobsrec.mon;
				day = currobsrec.day;
				hr = currobsrec.hr;
				min = currobsrec.min;
				sec = currobsrec.sec;
				mfme = hr * 60 + min + sec / 60.0;
				// not 0.0 for utc
				MathTimeLib::jday(year, mon, day, hr, min, sec, currobsrec.jd, currobsrec.jdf);  
				if (ii == 1) // need the second one since it uses gibbs and thus the middle vector
				{
					MathTimeLib::jday(year, mon, day, hr, min, sec, satrec.jdsatepoch, satrec.jdsatepochF);
					obsrecarr[0].dtmin = ((obsrecarr[0].jd +
						(obsrecarr[0].hr * 60 + obsrecarr[0].min + obsrecarr[0].sec / 60.0) / 1440.0)
						- satrec.jdsatepoch) * 1440.0;
				}
				// calculate the sensor each time because the obs might be from a different site
				SGP4DC::getsensorparams(currobsrec.sennum, currsenrec);
				AstroLib::site(currsenrec.senlat, currsenrec.senlon, currsenrec.senalt,
					currobsrec.rsecef, currobsrec.vsecef);

				dtmin = ((currobsrec.jd + currobsrec.jdf) - (jdepoch + jdepochf)) * 1440.0;  // satrec.jdepoch
				if (ii == 1)
					currobsrec.dtmin = 0.0;  // second state is the epoch
				else
					currobsrec.dtmin = dtmin;
				obsrecarr[ii] = currobsrec;
				ii = ii + 1;
			}   // if strr
		} // while ii through producing the observation vectors


		printf("over 30 obs %3i \n", ii-1);

		// setup nominal vector -----------------------------------------
		satrec.bstar = 0.0001;  // set a deafult

		// need to get some kind of range estimate for these data types...
		if ((currobsrec.obstype == 1) || (currobsrec.obstype == 3))
		{
			printf("input range estimate in er (km?) \n");
			scanf_s("%lf", &rngest);
		}

		zrng = 0.0;
		zaz = 0.0;
		zel = 0.0;
		zrtasc = 0.0;
		zdecl = 0.0;
		rsum[0] = 0.0;
		rsum[1] = 0.0;
		rsum[2] = 0.0;
		vsum[0] = 0.0;
		vsum[1] = 0.0;
		vsum[2] = 0.0;
		skipped = 0;
		keepit = 'y';

		// try averaging the first 10 observations to get a value
		for (ktr = 1; ktr < 10; ktr++)     // 10
		{
			switch (currobsrec.obstype)
			{
			case 1:
			{
				AstroLib::rv_radec(r1, v1, MathTimeLib::eFrom,
					rngest, obsrecarr[ktr].az, obsrecarr[ktr].el, zrng, zaz, zel);
			}
			break;
			case 2:  // very simplistic iod here just gets 3 position vectors in ecef,
				// obs are in topocentric (sez) coordinates (ecef) range az el obs
			{
				AstroLib::rv_razel(r1, v1, currsenrec.senlat, currsenrec.senlon, currsenrec.senalt, MathTimeLib::eFrom,
					obsrecarr[ktr].rng, obsrecarr[ktr].az, obsrecarr[ktr].el, zrng, zaz, zel);
				AstroLib::rv_razel(r2, v2, currsenrec.senlat, currsenrec.senlon, currsenrec.senalt, MathTimeLib::eFrom,
					obsrecarr[ktr + 1].rng, obsrecarr[ktr + 1].az, obsrecarr[ktr + 1].el, zrng, zaz, zel);
				AstroLib::rv_razel(r3, v3, currsenrec.senlat, currsenrec.senlon, currsenrec.senalt, MathTimeLib::eFrom,
					obsrecarr[ktr + 2].rng, obsrecarr[ktr + 2].az, obsrecarr[ktr + 2].el, zrng, zaz, zel);
			}
			break;
			case 3:
			{
				// but the rs vector should be in eci here - see code...
				AstroLib::rv_tradec(r1, v1, obsrecarr[ktr].rsecef, MathTimeLib::eFrom,
					rngest, obsrecarr[ktr].trtasc, obsrecarr[ktr].tdecl, zrng, zrtasc, zdecl);
			}
			break;
			} // case obstype

			// convert to eci  perform iod (in earth inertial, use teme as approx eci)
			jd = obsrecarr[ktr].jd;
			jdf = (obsrecarr[ktr].hr * 60.0 + obsrecarr[ktr].min + obsrecarr[ktr].sec / 60.0) * dayconv;
			EopSpw::findeopparam(jd, jdf, 's', eoparr, jdeopstart, dut1, dat, lod, xp, yp, ddpsi, ddeps,
				iaudx, dy, icrsx, y, s);
			MathTimeLib::convtime(obsrecarr[ktr].year, obsrecarr[ktr].mon, obsrecarr[ktr].day, obsrecarr[ktr].hr, obsrecarr[ktr].min, obsrecarr[ktr].sec,
				timezone, dut1, dat, ut1, tut1, jdut1, jdut1f, utc, tai, tt, ttt, jdtt, jdttf, tcg, tdb, ttdb, jdtdb, jdtdbf, tcb);
			// itrf to teme
			//AstroLib::teme_ecef(r1teme, v1teme, ateme, MathTimeLib::eFrom, r1, v1, aitrf, ttt, xp, yp, jdut1, lod, eqeterms);
			// these will actually be regular eci
			AstroLib::itrf_gcrf(r1, v1, aitrf, MathTimeLib::eTo, r1teme, v1teme, ateme,
				iau80rec, e80, ttt, jdut1 + jdut1f, lod, xp, yp, eqeterms, ddpsi, ddeps, trans);

			jd = obsrecarr[ktr + 1].jd;
			jdf = (obsrecarr[ktr + 1].hr * 60.0 + obsrecarr[ktr + 1].min + obsrecarr[ktr + 1].sec / 60.0) * dayconv;
			EopSpw::findeopparam(jd, jdf, 's', eoparr, jdeopstart, dut1, dat, lod, xp, yp, ddpsi, ddeps,
				iaudx, dy, icrsx, y, s);
			MathTimeLib::convtime(obsrecarr[ktr + 1].year, obsrecarr[ktr + 1].mon, obsrecarr[ktr + 1].day, obsrecarr[ktr + 1].hr, obsrecarr[ktr + 1].min, obsrecarr[ktr + 1].sec,
				timezone, dut1, dat, ut1, tut1, jdut1, jdut1f, utc, tai, tt, ttt, jdtt, jdttf, tcg, tdb, ttdb, jdtdb, jdtdbf, tcb);
			// itrf to teme
			//AstroLib::teme_ecef(r2teme, v2teme, ateme, MathTimeLib::eFrom, r2, v2, aitrf, ttt, xp, yp, jdut1, lod, eqeterms);
			AstroLib::itrf_gcrf(r2, v2, aitrf, MathTimeLib::eTo, r2teme, v2teme, ateme,
				iau80rec, e80, ttt, jdut1 + jdut1f, lod, xp, yp, eqeterms, ddpsi, ddeps, trans);

			jd = obsrecarr[ktr + 2].jd;
			jdf = (obsrecarr[ktr + 2].hr * 60.0 + obsrecarr[ktr + 2].min + obsrecarr[ktr + 2].sec / 60.0) * dayconv;
			EopSpw::findeopparam(jd, jdf, 's', eoparr, jdeopstart, dut1, dat, lod, xp, yp, ddpsi, ddeps,
				iaudx, dy, icrsx, y, s);
			MathTimeLib::convtime(obsrecarr[ktr + 2].year, obsrecarr[ktr + 2].mon, obsrecarr[ktr + 2].day, obsrecarr[ktr + 2].hr, obsrecarr[ktr + 2].min, obsrecarr[ktr + 2].sec,
				timezone, dut1, dat, ut1, tut1, jdut1, jdut1f, utc, tai, tt, ttt, jdtt, jdttf, tcg, tdb, ttdb, jdtdb, jdtdbf, tcb);
			// itrf to teme
			//AstroLib::teme_ecef(r3teme, v3teme, ateme, MathTimeLib::eFrom, r3, v3, aitrf, ttt, xp, yp, jdut1, lod, eqeterms);
			AstroLib::itrf_gcrf(r3, v3, aitrf, MathTimeLib::eTo, r3teme, v3teme, ateme,
				iau80rec, e80, ttt, jdut1 + jdut1f, lod, xp, yp, eqeterms, ddpsi, ddeps, trans);

			// these are really eci here for now
			AstroLib::gibbs(r1teme, r2teme, r3teme, v2teme, theta, theta1, copa, error);
			fprintf(outfile1, " gibbs  %11.5lf  %11.5lf  %11.5lf %11.7lf %11.7lf %11.7lf \n", r2teme[0], r2teme[1], r2teme[2], v2teme[0], v2teme[1], v2teme[2]);
			// most of these angles are just 0.72 deg apart
			// the times in herrgibbs need to be in increasing order...
			AstroLib::herrgibbs(r1teme, r2teme, r3teme, obsrecarr[ktr - 1].jd + obsrecarr[ktr - 1].jdf,
				obsrecarr[ktr].jd + obsrecarr[ktr].jdf, obsrecarr[ktr + 1].jd + obsrecarr[ktr + 1].jdf,
				v2teme, theta, theta1, copa, error);   // this is earth fixed
			fprintf(outfile1, "hgibbs  %11.5lf  %11.5lf  %11.5lf %11.7lf %11.7lf %11.7lf ", r2[0], r2[1], r2[2], v2[0], v2[1], v2[2]);
			fprintf(outfile1, "hgibbs  %11.5lf  %11.5lf  %11.5lf %11.7lf %11.7lf %11.7lf \n", obsrecarr[ktr].dtmin - obsrecarr[ktr].dtmin,
				obsrecarr[ktr + 1].dtmin - obsrecarr[ktr].dtmin,
				obsrecarr[ktr + 2].dtmin - obsrecarr[ktr].dtmin,
				theta*rad, theta1*rad, copa*rad);

			// now sum up the vector in teme to get an initial guess
			// use two-body for simplicity and since short time span for iod
			// move vectors back to middle vector time which is one ahead
			AstroLib::kepler(r2teme, v2teme, obsrecarr[ktr + 1].dtmin*60.0, r1, v1);  // these will be all in eci/teme

			//                  if (ktr > 0) // don't ask for the first one
			//                    {
			//                      printf("%11.7lf %11.7lf %11.7lf  keepit? y, n \n",r1[0], r1[1], r1[2] );
			//                      keepit = getchar();
			//                      fflush(stdin);
			//                    }
			//                  if (keepit == 'y')
			{
				rsum[0] = rsum[0] + r1[0];
				rsum[1] = rsum[1] + r1[1];
				rsum[2] = rsum[2] + r1[2];
				vsum[0] = vsum[0] + v1[0];
				vsum[1] = vsum[1] + v1[1];
				vsum[2] = vsum[2] + v1[2];
			}
			//                    else
			//                      skipped = skipped + 1;

		}  // for ktr to sum up vectors
		fprintf(outfile1, "res eci  %11.5lf  %11.5lf  %11.5lf %11.7lf %11.7lf %11.7lf %3i %3i \n", rsum[0], rsum[1], rsum[2], vsum[0], vsum[1], vsum[2], ktr, skipped);

		//              if (ktr > 1)
		//                  ktr = ktr - skipped;

		rteme[0] = rsum[0] / ktr;
		rteme[1] = rsum[1] / ktr;
		rteme[2] = rsum[2] / ktr;
		vteme[0] = vsum[0] / ktr;
		vteme[1] = vsum[1] / ktr;
		vteme[2] = vsum[2] / ktr;

        // set it outright for testing =================================================
		rteme[0] = 5975.2904;  
		rteme[1] = 2568.6400;
		rteme[2] = 3120.5845;
		vteme[0] = 3.983846;
		vteme[1] = -2.071159;
		vteme[2] = -5.917095;

		for (i = 0; i < 6; i++)
		{
			if (i < 3)
				xnom[i] = rteme[i];
			else
				xnom[i] = vteme[i - 3];
		}

		if (statesize > 6)
			xnom[6] = 0.0001; // set initial guess as it changes in state2satrec
		else
			satrec.bstar = 0.0001;
		SGP4DC::state2satrec(xnom, jdepoch, jdepochf, 'v', statesize, MathTimeLib::eTo, satrec); // force this method to assign values
		if (obsrecarr[1].year >= 2000)
			satrec.epochyr = obsrecarr[1].year - 2000;
		else
			satrec.epochyr = obsrecarr[1].year - 1900;
		MathTimeLib::finddays(obsrecarr[1].year, obsrecarr[1].mon, obsrecarr[1].day, obsrecarr[1].hr,
			obsrecarr[1].min, obsrecarr[1].sec, satrec.epochdays);
		//               satrec.jdsatepoch = satrec.jdsatepoch;  // set earlier

		SGP4Funcs::sgp4init(whichconst, opsmode, satrec.satnum, satrec.jdsatepoch - 2433281.5, satrec.bstar,
			satrec.ndot, satrec.nddot, satrec.ecco, satrec.argpo, satrec.inclo, satrec.mo, satrec.no_kozai,
			satrec.nodeo, satrec);


		printf("rteme  %11.5lf  %11.5lf  %11.5lf %11.7lf %11.7lf %11.7lf \n", rteme[0], rteme[1], rteme[2], vteme[0], vteme[1], vteme[2]);
		printf("coe  %11.5lf  %11.5lf  %11.5lf %11.7lf %11.7lf %11.7lf %11.7lf \n", satrec.a*6378.135, satrec.ecco, satrec.inclo*rad,
			satrec.nodeo*rad, satrec.argpo*rad, satrec.mo*rad, satrec.no_kozai);
		fprintf(outfile1, "rteme  %11.5lf  %11.5lf  %11.5lf %11.7lf %11.7lf %11.7lf \n", rteme[0], rteme[1], rteme[2], vteme[0], vteme[1], vteme[2]);
		fprintf(outfile1, "coe  %11.5lf  %11.5lf  %11.5lf %11.7lf %11.7lf %11.7lf %11.7lf \n", satrec.a*6378.135, satrec.ecco, satrec.inclo*rad,
			satrec.nodeo*rad, satrec.argpo*rad, satrec.mo*rad, satrec.no_kozai);

		// nominal vector in teme
		//              satrec.a = 7203.20025180/6378.135;  //er
		//              satrec.ecco = 0.0015278327;
		//              satrec.inclo = 114.954226 / rad;
		//              satrec.nodeo = 190.308229 / rad;
		//              satrec.argpo = 345.4910919 / rad;
		//              satrec.mo = 165.996981 / rad;
		// answer??
		satrec.a = 7214.3395180 / 6378.135;  //er
		satrec.ecco = 0.0006009327;
		satrec.inclo = 114.962396 / rad;
		satrec.nodeo = 190.297730 / rad;
		satrec.argpo = 123.8855619 / rad;
		satrec.mo = 123.828381 / rad;
		satrec.no_kozai = (1.0 / 13.446839) * sqrt(1.0 / (satrec.a * satrec.a * satrec.a));  // rad / min

		SGP4Funcs::sgp4init(whichconst, opsmode, satrec.satnum, satrec.jdsatepoch - 2433281.5, satrec.bstar,
			satrec.ndot, satrec.nddot, satrec.ecco, satrec.argpo, satrec.inclo, satrec.mo, satrec.no_kozai,
			satrec.nodeo, satrec);
		// call the propagator to get the initial state vector value
		SGP4Funcs::sgp4(satrec, 0.0, rnomTEME, vnomTEME);

		SGP4DC::state2satrec(xnom, jdepoch, jdepochf, statetype, statesize, MathTimeLib::eFrom, satrec);
		//rnomTEME[0] = 6157.4121;
		//rnomTEME[1] = 2455.16193;
		//rnomTEME[2] = 2823.89107;
		//vnomTEME[0] = 3.667402;
		//vnomTEME[1] = -2.204994;
		//vnomTEME[2] = -6.0647111;
		printf("running new satellite \n");

		// --------- now determine the orbit for the satellite ----------
		SGP4DC::leastsquares(percentchg, deltaamtchg, rmsepsilon, whichconst, interp, jdeopstart, satrec, typeans,
			statetype, proptype, firstob, obsread, statesize, obsrecarr, loops,
			xnom, jdepoch, jdepochf, derivType, cdam, cram, jplopt, jpldearr, jdjpldestart, iau80rec, eoparr, x, dx, atwai, atwa, atwb, outfile, outfile1);

		SGP4Funcs::sgp4(satrec, 0.0, rnomTEME, vnomTEME);
		rpos[0] = obsrecarr[0].x;
		rpos[1] = obsrecarr[0].y;
		rpos[2] = obsrecarr[0].z;
		vpos[0] = obsrecarr[0].xdot;
		vpos[1] = obsrecarr[0].ydot;
		vpos[2] = obsrecarr[0].zdot;
		for (j = 0; j < 3; j++)
		{
			dr[j] = rnomTEME[j] - rpos[j];
			dv[j] = vnomTEME[j] - vpos[j];
		}
		fprintf(outfile, " difference %11lf  %11lf  %11lf %11lf \n", dr[0], dr[1], dr[2], MathTimeLib::mag(dr));
		fprintf(outfile1, " difference %11lf  %11lf  %11lf %11lf \n", dr[0], dr[1], dr[2], MathTimeLib::mag(dr));
		printf(" difference %11lf  %11lf  %11lf %11lf \n", dr[0], dr[1], dr[2], MathTimeLib::mag(dr));

		fclose(infile);
		fclose(outfile);
		fclose(outfile1);
	}
}   // processrazel


// trim from start (in place)
static inline void ltrim(std::string &s) 
{
	s.erase(s.begin(), std::find_if(s.begin(), s.end(), [](int ch) 
	{
		return !std::isspace(ch);
	}));
}

void removeWS(char *c)
{
	char *s = c;
	while (s++, *(c++))
	{
		if (isspace(*s))++s;
		*s = *c;
	}
}

void whitespace(char str[])
{
	int i;
	int j;
	for (i = 0; str[i] != 0; i++)
	{
		if (isspace(str[i]))
		{
			for (j = i; str[j] != 0; j++)
			{
				str[j] = str[j + 1];
			}
		}
	}
}


/*  --------------------------------------------------------------------------    
// this procedure reads a .e file and performs LS OD on it
// there are options to read a span of data, at given timesteps
// the process use numerical partial derivatives for the A matrix
// the nominal vector is formed at the timeof the first measurement
//  -------------------------------------------------------------------------- */

void processxyz
(
	char batchmode, char proptype, int derivType, double cdam, double cram,
	iau80data iau80rec,
	const std::vector<eopdata> eoparr,
	double jdeopstart, double jdeopstartf,
	const std::vector<spwdata> spwarr,
	double jdspwstart, double jdspwstartf,
	const std::vector<jpldedata> jpldearr,
	char jplopt,
	double jdjpldestart,
	double 	jdepoch, double jdepochf, char interp,
	int firstob, int obsskip, int obsread,
	std::vector<obsrec> &obsarr,
	std::vector<double> &xnom
)
{
	FILE *outfile, *outfile1;
	elsetrec satrec;
	gravconsttype whichconst;
	long search_satno = 0;
	char typerun;
	int loops, eqeterms, j;
	double ttt;
	std::vector< std::vector<double> > trans = std::vector< std::vector<double> >(3, std::vector<double>(3));
	char statetype, typeans, opsmode;
	int statesize;
	double percentchg, deltaamtchg, rmsepsilon;
	double magcov;
	double rpos[3], vpos[3], rnomTEME[3], vnomTEME[3], dr[3], dv[3], magdr1, magdr, reci[3], veci[3], aeci[3],
		ateme[3], recin[3], vecin[3];

	whichconst = wgs72;
	typerun = 'c';  // c catalog, v verification, m manual
	opsmode = 'a';
	typeans = 'b';  // mat inv or svd
	eqeterms = 2;
	interp = 's';

	if (batchmode == 'n')
	{
		statetype = 'v';   // v vectors, t tle vars, e equinoctial
		//printf("input type of elements tle or equin \n\n");
		//statetype = getchar();
		//fflush(stdin);
		percentchg = 0.001;  // percent to change components for finite differencing
		//printf("input percentchg for finite differencing (.001) \n");
		//scanf_s("%lf", &percentchg, 5 * sizeof(char));
		deltaamtchg = 0.0000001;  // delta amt to change components for finite differencing
		//printf("input deltaamtchg to chk in finite differencing (.0000001) \n");
		//scanf_s("%lf", &deltaamtchg, 10 * sizeof(char));
		rmsepsilon = 0.0002;  // percent to change components for finite differencing
		//printf("input rmsepsilon to quit (.02) \n");
		//scanf_s("%lf", &rmsepsilon, 5 * sizeof(char));
		statesize = 6;        // solve for bstar or not
		//printf("input statesize (6 or 7) \n");
		//scanf_s("%i", &statesize, sizeof(char));
	}

	// set these variables up with initial dimensions
	// these must be set here
	// function matrices can be resized
	std::vector< std::vector<double> > x = std::vector< std::vector<double> >(statesize, std::vector<double>(1));
	std::vector< std::vector<double> > dx = std::vector< std::vector<double> >(statesize, std::vector<double>(1));
	std::vector< std::vector<double> > atwai = std::vector< std::vector<double> >(statesize, std::vector<double>(statesize));
	std::vector< std::vector<double> > atwa = std::vector< std::vector<double> >(statesize, std::vector<double>(statesize));
	std::vector< std::vector<double> > atwb = std::vector< std::vector<double> >(statesize, std::vector<double>(1));

	printf("state type  %c \n", statetype);
	printf("percentchg  %lf pct to change components for finite diff \n", percentchg);
	printf("deltaamtchg %11.8lf static change amount for each case \n", deltaamtchg);
	printf("rmsepsilon  %lf \n", rmsepsilon);
	printf("state size  %i \n", statesize);


	// open files for reading and writing results
	int err = fopen_s(&outfile, "testdc.out", "w");
	err = fopen_s(&outfile1, "testdc1.out", "w");

	//	fprintf(outfile, "typeans %c \n", typeans);
	fprintf(outfile, "state type  %c \n", statetype);
	fprintf(outfile, "percentchg  %lf \n", percentchg);
	fprintf(outfile, "deltaamtchg %11.8lf \n", deltaamtchg);
	fprintf(outfile, "rmsepsilon  %lf \n", rmsepsilon);
	fprintf(outfile, "state size  %i \n", statesize);


	// setup nominal vector -----------------------------------------
	eOpt opt = e80;
	if (proptype == 's')
	{
		if (proptype == 's')
		{
			recin[0] = obsarr[0].x;
			recin[1] = obsarr[0].y;
			recin[2] = obsarr[0].z;
			vecin[0] = obsarr[0].xdot;
			vecin[1] = obsarr[0].ydot;
			vecin[2] = obsarr[0].zdot;

			// convert eci to teme for sgp4 tle creation!
			ttt = (jdepoch + jdepochf - 2451545.0) / 36525.0;
			AstroLib::teme_eci(rnomTEME, vnomTEME, ateme, MathTimeLib::eFrom, recin, vecin, aeci, iau80rec, opt, ttt, 0.0, 0.0);

			xnom[0] = rnomTEME[0];
			xnom[1] = rnomTEME[1];
			xnom[2] = rnomTEME[2];
			xnom[3] = vnomTEME[0];
			xnom[4] = vnomTEME[1];
			xnom[5] = vnomTEME[2];
			SGP4DC::state2satrec(xnom, jdepoch, jdepochf, statetype, statesize, MathTimeLib::eTo, satrec);
		}
		satrec.bstar = 0.0001;  // set a deafult for opsmode
		SGP4Funcs::sgp4init(whichconst, 'a', satrec.satnum, satrec.jdsatepoch - 2433281.5, satrec.bstar,
			satrec.ndot, satrec.nddot, satrec.ecco, satrec.argpo, satrec.inclo, satrec.mo, satrec.no_kozai,
			satrec.nodeo, satrec);
		// call the propagator to get the initial state vector value
		SGP4Funcs::sgp4(satrec, 0.0, rnomTEME, vnomTEME);
		ttt = (satrec.jdsatepoch + satrec.jdsatepochF - 2451545.0) / 36525.0;
		AstroLib::teme_eci(rnomTEME, vnomTEME, ateme, MathTimeLib::eTo, recin, vecin, aeci, iau80rec, opt, ttt, 0.0, 0.0);
		for (j = 0; j < 3; j++)
		{
			xnom[j] = recin[j];
			xnom[j + 3] = vecin[j];
		}
	}
	else
	{
		for (j = 0; j < 3; j++)
		{
			recin[j] = xnom[j];
			vecin[j] = xnom[j + 3];
		}
	}

	printf(" init nom vector %11lf  %11lf  %11lf  %11lf  %11lf  %11lf \n", recin[0], recin[1], recin[2], vecin[0], vecin[1], vecin[2]);

	// print initial difference
	reci[0] = obsarr[0].x;
	reci[1] = obsarr[0].y;
	reci[2] = obsarr[0].z;
	veci[0] = obsarr[0].xdot;
	veci[1] = obsarr[0].ydot;
	veci[2] = obsarr[0].zdot;
	for (j = 0; j < 3; j++)
	{
		dr[j] = recin[j] - reci[j];
		dv[j] = vecin[j] - veci[j];
	}
	fprintf(outfile, " init diff %11lf  %11lf  %11lf %11lf \nNote the this assumes the .e file starts at satellite epoch\n", dr[0], dr[1], dr[2], MathTimeLib::mag(dr));
	magdr1 = 1000.0 * MathTimeLib::mag(dr); // initial difference in m
	printf(" init diff %11lf  %11lf  %11lf %11lf \nNote the this assumes the .e file starts at satellite epoch\n", dr[0], dr[1], dr[2], MathTimeLib::mag(dr));
	magdr1 = 1000.0 * MathTimeLib::mag(dr); // initial difference in m

	printf("running new satellite \n");

	// --------- now determine the orbit for the satellite ----------
	// note xnom is changed in here - it should come out in x though?....
	SGP4DC::leastsquares(percentchg, deltaamtchg, rmsepsilon, whichconst, interp, jdeopstart, satrec, typeans,
		statetype, proptype, firstob, obsread, statesize, obsarr, loops,
		xnom, jdepoch, jdepochf, derivType, cdam, cram, jplopt, jpldearr, jdjpldestart, iau80rec, eoparr, x, dx, atwai, atwa, atwb, outfile, outfile1);

	if (proptype == 's')
	{
		SGP4Funcs::sgp4(satrec, 0.0, rnomTEME, vnomTEME);
		AstroLib::teme_eci(rnomTEME, vnomTEME, ateme, MathTimeLib::eTo, recin, vecin, aeci, iau80rec, opt, ttt, 0.0, 0.0);
	}
	else
	{
		for (j = 0; j < 3; j++)
		{
			recin[j] = xnom[j];
			vecin[j] = xnom[j + 3];
		}
	}

	rpos[0] = obsarr[0].x;
	rpos[1] = obsarr[0].y;
	rpos[2] = obsarr[0].z;
	vpos[0] = obsarr[0].xdot;
	vpos[1] = obsarr[0].ydot;
	vpos[2] = obsarr[0].zdot;
	for (j = 0; j < 3; j++)
	{
		dr[j] = recin[j] - rpos[j];
		dv[j] = vecin[j] - vpos[j];
	}
	magdr = MathTimeLib::mag(dr) * 1000.0;
	//               magcov =  sqrt( xnom[0] / satrec.ecco * atwai[1][0] +
	//                              -xnom[1] / (satrec.ecco * satrec.ecco) * atwai[4][0] +
	//                               xnom[1] / (satrec.ecco * satrec.ecco) * atwai[5][0]);

	magcov = sqrt(atwai[2][2]) * 6378135.0;  // in m

	fprintf(outfile, " difference %11lf  %11lf  %11lf %11lf %10.2lf \n", dr[0], dr[1], dr[2], MathTimeLib::mag(dr), magcov);
	fprintf(outfile1, " difference %11lf  %11lf  %11lf km %12lf m %10.2lf \n\n", dr[0], dr[1], dr[2], 1000.0 * MathTimeLib::mag(dr), magcov);
	if (proptype == 's')
		printf("%4i %8.1lf %8.4lf m a %10.5lf e %10.7lf i %8.3lf %11.6lf m\n", loops, magdr1, magdr, satrec.a, satrec.ecco, satrec.inclo*57.2957795,
			magcov);

	// ------------- now take the converged TLE and propagate (and ----------------
	// ---------- convert coordinate system) over ephemeris and compare -----------
	//for (j = 0; j < obsread; j++)
	//{
	//	if (proptype == 's')
	//		SGP4Funcs::sgp4(satrec, obsrecarr[j].dtmin, rteme, vteme);
	//	dr[0] = 1000.0 * (obsrecarr[j].x - reci[0]);
	//	dr[1] = 1000.0 * (obsrecarr[j].y - reci[1]);
	//	dr[2] = 1000.0 * (obsrecarr[j].z - reci[2]);
	//	dv[0] = 1000.0 * (obsrecarr[j].xdot - veci[0]);
	//	dv[1] = 1000.0 * (obsrecarr[j].ydot - veci[1]);
	//	dv[2] = 1000.0 * (obsrecarr[j].zdot - veci[2]);
	//	fprintf(outfile, "diff %11.3lf  %11.4lf %11.4lf %11.4lf  %11.3lf m \n", obsrecarr[j].dtmin, dr[0], dr[1], dr[2], MathTimeLib::mag(dr));
	//}
	fprintf(outfile, " init diff %11lf  %11lf  %11lf %11lf \n", dr[0], dr[1], dr[2], MathTimeLib::mag(dr));
	magdr1 = 1000.0 * MathTimeLib::mag(dr); // initial difference in m

	if (proptype == 's')
		SGP4DC::printtle(satrec);

	fclose(outfile);
	fclose(outfile1);

}  // processxyz


/*       ---------------------------------------------------------------      */
void processtle
(
	char batchmode, char proptype, int derivType, double cdam, double cram,
	iau80data iau80rec,
	const std::vector<eopdata> eoparr,
	double jdeopstart, double jdeopstartf,
	const std::vector<spwdata> spwarr,
	double jdspwstart, double jdspwstartf,
	const std::vector<jpldedata> jpldearr,
	char jplopt,
	double jdjpldestart,
	double 	jdepoch, double jdepochf, char interp,
	int firstob, int obsskip, int obsread,
	std::vector<obsrec> &obsarr,
	std::vector<double> &xnom
)
{
	FILE *infile, *outfile, *outfile1, *outfile2;
	std::vector<obsrec> obsrecarr;
	elsetrec satrec;
	long search_satno = 0;
	char longstr1[130], longstr2[130];
	char str[2];
	char typerun;
	gravconsttype whichconst;
	double startmfe, stopmfe, deltamin, magdr;
	int ll1m, l1m, l10m, l50m, l1km, l10km;
	double totalnum, periodmin, avgiter, magcov;
	int i, ii, j, loops;
	double rnomTEME[3], vnomTEME[3], rpos[3], vpos[3], dr[3], dv[3], dtmin, jd, jdf, magdr1;
	obsrec currobsrec;
	double rad, apalt, pralt;
	char  statetype, typeans, opsmode;
	int statesize, lastob;
	char fname[160];
	double percentchg, deltaamtchg, rmsepsilon;
	obsrecarr.resize(5000);

	interp = 'l'; // set eop interpolation to linear

	if (batchmode == 'n') // needs a default value here in batch mode
		statesize = 7;
	// set these variables up with intial dimensions
	// these must be set here
	// function matrices can be resized
	std::vector< std::vector<double> > x = std::vector< std::vector<double> >(statesize, std::vector<double>(1));
	std::vector< std::vector<double> > dx = std::vector< std::vector<double> >(statesize, std::vector<double>(1));
	std::vector< std::vector<double> > atwai = std::vector< std::vector<double> >(statesize, std::vector<double>(statesize));
	std::vector< std::vector<double> > atwa = std::vector< std::vector<double> >(statesize, std::vector<double>(statesize));
	std::vector< std::vector<double> > atwb = std::vector< std::vector<double> >(statesize, std::vector<double>(1));

	rad = 180.0 / pi;
	whichconst = wgs72;
	typerun = 'c';  // c catalog, v verification, m manual
	if (batchmode == 'n')
	{
		statetype = 't';   // v vectors, t tle vars, e equinoctial
		printf("input type of elements tle or equin \n\n");

		statetype = getchar();
		fflush(stdin);

		printf("input TLE filename: \n");
		scanf_s("%s", fname, 85 * sizeof(char));

		percentchg = 0.001;  // percent to change components for finite differencing
		//printf("input percentchg for finite differencing (.001) \n");
		//scanf_s("%lf", &percentchg, 5 * sizeof(char));

		deltaamtchg = 0.0000001;  // delta amt to change components for finite differencing
		//printf("input deltaamtchg to chk in finite differencing (.0000001) \n");
		//scanf_s("%lf", &deltaamtchg, 10 * sizeof(char));

		rmsepsilon = 0.0002;  // percent to change components for finite differencing
		//printf("input rmsepsilon to quit (.02) \n");
		//scanf_s("%lf", &rmsepsilon, 5 * sizeof(char));

		lastob = 72;        // how many obs per rev to consider
		printf("input lastob to quit (72) \n");  // this will be 72 points per rev, but calculate later for 2 periods
		scanf_s("%i", &lastob);

		statesize = 6;        // solve for bstar or not
		//printf("input statesize (6 for posVel or 7 for posVel and B*) \n");
		//scanf_s("%i", &statesize, 2 * sizeof(char));
	}
	printf("fname %s \n", fname);

	firstob = 0;
	opsmode = 'a';
	typeans = 's';

	fprintf(outfile, "typeans %c \n", typeans);
	fprintf(outfile, "state type %c \n", statetype);
	fprintf(outfile, "percentchg %lf \n", percentchg);
	fprintf(outfile, "deltaamtchg %11.8lf \n", deltaamtchg);
	fprintf(outfile, "rmsepsilon %lf \n", rmsepsilon);
	fprintf(outfile, "state size %i \n", statesize);
	fprintf(outfile, "pts per period %i \n", lastob);

	fprintf(outfile2, "typeans %c \n", typeans);
	fprintf(outfile2, "state type %c \n", statetype);
	fprintf(outfile2, "percentchg %lf \n", percentchg);
	fprintf(outfile2, "deltaamtchg %11.8lf \n", deltaamtchg);
	fprintf(outfile2, "rmsepsilon %lf \n", rmsepsilon);
	fprintf(outfile2, "state size %i \n", statesize);
	fprintf(outfile2, "pts per period %i \n", lastob);
	fprintf(outfile2, " iter   norad      inti diff (m)  final diff    a           e          i     ap alt   pr alt\n");

	int err = fopen_s(&infile, fname, "r");
	err = fopen_s(&outfile, "testdc.out", "w");
	err = fopen_s(&outfile1, "testdc1.out", "w");
	err = fopen_s(&outfile2, "testdc2.out", "w");
	if (infile == NULL)
	{
		printf("Failed to open %s ", fname);
	}
	else
	{
		// initialize EOP file
		std::vector<eopdata> eoparr;
		EopSpw::readeop(eoparr, "../EOP-All-v1.1_2018-01-04.txt", jdeopstart, jdeopstartf);

		// --- setup counters for bins of entire catalog
		ll1m = 0;
		l1m = 0;
		l10m = 0;
		l50m = 0;
		l1km = 0;
		l10km = 0;
		avgiter = 0;

		while (feof(infile) == 0)
		{
			do
			{
				fgets(longstr1, 130, infile);
				strncpy_s(str, &longstr1[0], 1);
				str[1] = '\0';
			} while ((strcmp(str, "#") == 0) && (feof(infile) == 0));

			// ---- setup the sgp4 vectors from the input file ----
			if (feof(infile) == 0)
			{
				fgets(longstr2, 130, infile);
				// convert the char string to sgp4 elements
				// includes initialization of sgp4
				SGP4Funcs::twoline2rv(longstr1, longstr2, typerun, 'd', opsmode, whichconst,
					startmfe, stopmfe, deltamin, satrec);
				fprintf(outfile, "%ld xx \n", satrec.satnum);
				printf(" %5ld ", satrec.satnum);
			} // if going through input file

			// set the time span based on the period size
			//satrec.a = pow(satrec.no * satrec.tumin, (-2.0 / 3.0));
			periodmin = 2.0 * pi * sqrt(pow(satrec.radiusearthkm * satrec.a, 3) / 398600.8) / 60.0;  // in  min
			dtmin = int((periodmin) / lastob);

			// ---- loop to create ephemeris points (observations) ----------
			// ----------------- and setup obsrecarr ------------------------
			for (ii = 0; ii <= lastob; ii++)
			{
				currobsrec.dtmin = dtmin;
				SGP4Funcs::sgp4(satrec, dtmin * ii, rpos, vpos);
				jd = satrec.jdsatepoch;
				jdf = (dtmin * ii) / 1440.0;
				MathTimeLib::invjday(jd, jdf, currobsrec.year, currobsrec.mon, currobsrec.day,
					currobsrec.hr, currobsrec.min, currobsrec.sec);
				MathTimeLib::jday(currobsrec.year, currobsrec.mon, currobsrec.day, 0, 0, 0.0, currobsrec.jd, currobsrec.jdf);
				currobsrec.sennum = 1;
				currobsrec.init = satrec.init;
				currobsrec.method = satrec.method;
				currobsrec.obstype = 4;  // xyz obs
				currobsrec.x = rpos[0];
				currobsrec.y = rpos[1];
				currobsrec.z = rpos[2];
				currobsrec.xdot = vpos[0];
				currobsrec.ydot = vpos[1];
				currobsrec.zdot = vpos[2];
				currobsrec.bstar = satrec.bstar;
				obsrecarr[ii] = currobsrec;
			} // for ii through producing the observation vectors

			// setup nominal vector -----------------------------------------
			/*
			// create nominal to be a little off
			satrec.no = satrec.no*0.999;
			satrec.ecco = satrec.ecco*0.999;        //
			satrec.inclo = satrec.inclo + 0.01/rad;
			satrec.nodeo = satrec.nodeo*1.0002;     //
			satrec.argpo = satrec.argpo*1.0001;     //
			satrec.mo = satrec.mo;                  //
			satrec.bstar = 0.0001; // initial guess only 0.001
			*/
			// -------- or use vector from first call from TLE...
			SGP4Funcs::sgp4(satrec, 0.0, rnomTEME, vnomTEME);
			for (i = 0; i < 6; i++)
			{
				if (i < 3)
					xnom[i] = rnomTEME[i];
				else
					xnom[i] = vnomTEME[i - 3];
			}
			if (statesize > 6)
				xnom[6] = 0.0001; // set initial guess as it changes in state2satrec
			else
				satrec.bstar = 0.0001;
			SGP4DC::state2satrec(xnom, jdepoch, jdepochf, 'v', statesize, MathTimeLib::eTo, satrec); // force this method to assign values

			SGP4Funcs::sgp4init(whichconst, opsmode, satrec.satnum, satrec.jdsatepoch - 2433281.5, satrec.bstar,
				satrec.ndot, satrec.nddot, satrec.ecco, satrec.argpo, satrec.inclo, satrec.mo, satrec.no_kozai,
				satrec.nodeo, satrec);
			// call the propagator to get the initial state vector value
			SGP4Funcs::sgp4(satrec, 0.0, rnomTEME, vnomTEME);

			// print initial difference -----------------------
			rpos[0] = obsrecarr[0].x;
			rpos[1] = obsrecarr[0].y;
			rpos[2] = obsrecarr[0].z;
			vpos[0] = obsrecarr[0].xdot;
			vpos[1] = obsrecarr[0].ydot;
			vpos[2] = obsrecarr[0].zdot;
			for (j = 0; j < 3; j++)
			{
				dr[j] = rnomTEME[j] - rpos[j];
				dv[j] = vnomTEME[j] - vpos[j];
			}
			fprintf(outfile, " init diff %11lf  %11lf  %11lf %11lf \n", dr[0], dr[1], dr[2], MathTimeLib::mag(dr));
			magdr1 = 1000.0 * MathTimeLib::mag(dr); // initial difference in m
			//               printf( " init diff %11lf  %11lf  %11lf %11lf \n",dr[0], dr[1], dr[2], mag(dr) );

			//               if (statetype == 'v')
			//                 {
			//                   // set some rnomTEME vnomTEME value...

			//                   rv2coe ( rnomTEME, vnomTEME, p, a, ecc, incl, raan, argp, nu, m, eccanom, arglat, truelon, lonper);
			//                   satrec.no = 60.0 * sqrt(3.986008e5/(a * a * a));  // rad per min, sgp4 wgs-72 mu value
			//                   satrec.a = a/6378.135;  // er
			//                   satrec.ecco = ecc;
			//                   satrec.inclo = incl;
			//                   satrec.nodeo = raan;
			//                   satrec.argpo = argp;
			//                   satrec.mo = m;
			//                 }

			SGP4DC::state2satrec(xnom, jdepoch, jdepochf, statetype, statesize, MathTimeLib::eFrom, satrec);

			//               printf( "running new satellite \n" );

			// --------- now determine the orbit for the satellite ----------
			SGP4DC::leastsquares(percentchg, deltaamtchg, rmsepsilon, whichconst, interp, jdeopstart, satrec, typeans,
				statetype, proptype, firstob, lastob, statesize, obsrecarr, loops,
				xnom, jdepoch, jdepochf, derivType, cdam, cram, jplopt, jpldearr, jdjpldestart, iau80rec, eoparr, x, dx, atwai, atwa, atwb, outfile, outfile1);
			SGP4Funcs::sgp4(satrec, 0.0, rnomTEME, vnomTEME);  // the final answer from LS
			rpos[0] = obsrecarr[0].x;
			rpos[1] = obsrecarr[0].y;
			rpos[2] = obsrecarr[0].z;
			vpos[0] = obsrecarr[0].xdot;
			vpos[1] = obsrecarr[0].ydot;
			vpos[2] = obsrecarr[0].zdot;
			for (j = 0; j < 3; j++)
			{
				dr[j] = rnomTEME[j] - rpos[j];
				dv[j] = vnomTEME[j] - vpos[j];
			}
			magdr = MathTimeLib::mag(dr) * 1000.0;
			fprintf(outfile, " difference %11lf  %11lf  %11lf %11lf \n", dr[0], dr[1], dr[2], MathTimeLib::mag(dr));
			fprintf(outfile1, " %8d  difference %11lf  %11lf  %11lf km %12lf m \n\n", satrec.satnum, dr[0], dr[1], dr[2], 1000.0 * MathTimeLib::mag(dr));
			apalt = 6378.135 * satrec.a * (1.0 + satrec.ecco) - 6378.135;
			pralt = 6378.135 * satrec.a * (1.0 - satrec.ecco) - 6378.135;
			//               magcov =  1000.0 * sqrt(atwai[0][0] + atwai[1][1] + atwai[2][2]);
			magcov = sqrt(atwai[2][2]) * 6378135.0;  // in m

			if (1000.0 * MathTimeLib::mag(dr) < 0.0001)
				fprintf(outfile2, " %4i %8d %12.0lf %14.6lf %10.2lf %12lf %8.4lf %7.0lf %7.0lf %10.2lf\n",
				loops, satrec.satnum, magdr1, 0.000001, satrec.a * 6378, satrec.ecco, satrec.inclo*rad, apalt, pralt, magcov);
			else
				fprintf(outfile2, " %4i %8d %12.0lf %14.6lf %10.2lf %12lf %8.4lf %7.0lf %7.0lf %10.2lf\n",
				loops, satrec.satnum, magdr1, 1000.0 * MathTimeLib::mag(dr), satrec.a * 6378, satrec.ecco, satrec.inclo*rad, apalt, pralt, magcov);

			printf("%4i %8.1lf %8.4lf m a %10.5lf e %9.6lf i %7.3lf %9.4lf m\n", loops, magdr1, 1000.0 * MathTimeLib::mag(dr), satrec.a, satrec.ecco, satrec.inclo*rad,
				magcov);


			if (magdr < 1.0)
				ll1m = ll1m + 1;
			else
			{
				if (magdr < 10.0)
					l1m = l1m + 1;
				else
				{
					if (magdr < 100.0)
						l10m = l10m + 1;
					else
					{
						if (magdr < 1000.0)
							l50m = l50m + 1;
						else
						{
							if (magdr < 10000.0)
								l1km = l1km + 1;
							else
								l10km = l10km + 1;
						}
					}
				}
			}
			avgiter = avgiter + loops;

		}  // while not eof

		totalnum = ll1m + l1m + l10m + l50m + l1km + l10km;
		printf("%5i %8.2lf      < 1m  avgiter = %8.2lf \n", ll1m, 100.0 * (ll1m / totalnum), avgiter / totalnum);
		printf("%5i %8.2lf   1m < 10m \n", l1m, 100.0 * ((ll1m + l1m) / totalnum));
		printf("%5i %8.2lf  10m < 100m \n", l10m, 100.0 * ((ll1m + l1m + l10m) / totalnum));
		printf("%5i %8.2lf 100m < 1km \n", l50m, 100.0 * ((ll1m + l1m + l10m + l50m) / totalnum));
		printf("%5i %8.2lf  1km < 10km \n", l1km, 100.0 * ((ll1m + l1m + l10m + l50m + l1km) / totalnum));
		printf("%5i %8.2lf 10km > \n", l10km, 100.0 * ((ll1m + l1m + l10m + l50m + l1km + l10km) / totalnum));

		fprintf(outfile, "%5i %8.2lf      < 1m  avgiter = %8.2lf \n", ll1m, 100.0 * (ll1m / totalnum), avgiter / totalnum);
		fprintf(outfile, "%5i %8.2lf      < 1m \n", ll1m, 100.0 * (ll1m / totalnum));
		fprintf(outfile, "%5i %8.2lf   1m < 10m \n", l1m, 100.0 * ((ll1m + l1m) / totalnum));
		fprintf(outfile, "%5i %8.2lf  10m < 100m \n", l10m, 100.0 * ((ll1m + l1m + l10m) / totalnum));
		fprintf(outfile, "%5i %8.2lf 100m < 1km \n", l50m, 100.0 * ((ll1m + l1m + l10m + l50m) / totalnum));
		fprintf(outfile, "%5i %8.2lf  1km < 10km \n", l1km, 100.0 * ((ll1m + l1m + l10m + l50m + l1km) / totalnum));
		fprintf(outfile, "%5i %8.2lf 10km > \n", l10km, 100.0 * ((ll1m + l1m + l10m + l50m + l1km + l10km) / totalnum));

		fclose(infile);
		fclose(outfile);
		fclose(outfile1);
	}


	// Read an STK epehemris
	// by Alek Lidtke
	//void LoadEphemerisTable(std::string fullFileName)  // ExternalSpaceObject::
	//{
	//	/* Read a STK v 10 ephemeris file that contains the ephemeris of this object. The epochs in Julian Days of all the ephemeris points will
	//	be stored in EphemerisEpochs attribute, the Cartesian positions in km in EphemerisPositions, and the Cartesian velocities
	//	in km/sec in EphemerisVelocities. The state vectors are defined in the True Equator Mean Equinox reference frame.
	//	@param - name of a file that contains Julian day epoch, Cartesian positions and velocities in meteres and metres per second in TEME reference frame.
	//	*/
	//	EphemerisEpochs = std::vector<double>(); // Initialise the vectors that will hold the ephemeris points.
	//	EphemerisPositions = std::vector< std::vector<double> >();
	//	EphemerisVelocities = std::vector< std::vector<double> >();

	//	/* Parse the ephemeris file. */
	//	std::ifstream TLEfileStream(fullFileName, std::ifstream::in);

	//	std::string EphemerisLine; // Currently read ephemeris point.

	//	int noEphemPoints; // Counter of the total number of ephemeris points read.
	//	double JDAYepoch;
	//	int EphemerisPointsRead = 0; // Counter.
	//	double temp; // Temporary number read from the file.

	//	while (std::getline(TLEfileStream, EphemerisLine))
	//	{
	//		std::istringstream iss(EphemerisLine);

	//		/* Get information about the ephemeris file being parsed from the header and parse the ephemeris points. */
	//		if (EphemerisLine.find("NumberOfEphemerisPoints") == 0){
	//			std::string noEphemPointsString = EphemerisLine.substr(EphemerisLine.find(" ") + 1); // Don't want the white space before the number of points.
	//			std::basic_istringstream<char> noEphemPointsSS(noEphemPointsString); // Convert from std::string to an int.
	//			noEphemPointsSS >> noEphemPoints;
	//			EphemerisEpochs.resize(noEphemPoints); // Allocate memory for the ephemeris points.
	//			EphemerisPositions.resize(noEphemPoints);
	//			EphemerisVelocities.resize(noEphemPoints);
	//			for (std::vector<int>::size_type i = 0; i<EphemerisEpochs.size(); i++){
	//				EphemerisPositions.at(i).resize(3); // Resize every entry to be able to hold epoch, 3 Cartesian poisiton and 3 Cartesian velocity compontents (Julian Days, metres, and metres per second, respectively).
	//				EphemerisVelocities.at(i).resize(3);
	//			}
	//		}
	//		else if (EphemerisLine.find("# Time of first point:") == 0){
	//			std::string JDAYepochString = EphemerisLine.substr(EphemerisLine.find("UTCG =") + 6, 14); // Only get this part of the string that contains the epoch of the first point.
	//			std::basic_istringstream<char> JDAYepochSS(JDAYepochString);
	//			JDAYepochSS >> JDAYepoch; // Find the epoch of the first point - will get offset times from this epoch in seconds for every ephemeris point.
	//		}
	//		else if (EphemerisLine.size() != 0){ // Don't try to call .at(0) for empty strings.
	//			if (isdigit(EphemerisLine.at(0))){ // Only actual ephemeris points will start with digits.
	//				std::basic_istringstream<char> ephemPointSS(EphemerisLine); // Parse this ephemeris point - always 7 numbers in scientific notation but don't preassume anyting.
	//				for (int counter = 0; counter<7; counter++){ // Read the epoch, position, and velocity for this point.
	//					if (counter == 0){
	//						ephemPointSS >> temp; // Change the epoch to Julian Days from the time offset in seconds from the first point.
	//						EphemerisEpochs.at(EphemerisPointsRead) = JDAYepoch + temp*SECOND_IN_JDAYS;
	//					}
	//					else if ((counter>0) && (counter<4)){ // Now parsing position components.
	//						ephemPointSS >> temp;
	//						EphemerisPositions.at(EphemerisPointsRead).at(counter - 1) = temp / 1000.0; // Change to km.
	//					}
	//					else if ((counter>3) && (counter<7)){ // Now parsing velocity compontents.
	//						ephemPointSS >> temp;
	//						EphemerisVelocities.at(EphemerisPointsRead).at(counter - 4) = temp / 1000.0;
	//					}
	//				}
	//				EphemerisPointsRead += 1; // Make sure this ephemeris point won't be overwritten.
	//			}
	//		}
	//	};

	//	int LastLineInfo = EphemerisLine.find("END Ephemeris");
	//	if ((LastLineInfo == 0) || (EphemerisLine.empty()))
	//	{ // Reached the last line of the file.
	//		std::cout << "Parsed entre ephemeris file " << fullFileName << " and read " << EphemerisEpochs.size() << " ephemeris points." << std::endl;
	//	}
	//	else if ((LastLineInfo != 0) && (!EphemerisLine.empty()))
	//	{ // Haven't reached the last line of the file. Last line may be empty.
	//		std::cerr << "Haven't reached the end of the ephemeris file while reading the ephemeris from " << fullFileName << "." << std::endl;
	//	}
	//	else if (noEphemPoints != (int)EphemerisEpochs.size())
	//	{
	//		std::cerr << "Haven't read as many ephemeris points as quoted in the file header for " << fullFileName << "." << std::endl;
	//	};
	//}  // LoadEphemerisTable


}  // processtle




