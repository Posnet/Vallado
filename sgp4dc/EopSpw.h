#ifndef _EopSpw_h_
#define _EopSpw_h_
/* ---------------------------------------------------------------------
*
*                              EopSpw.h
*
*  this file contains routines to read the eop and space weather data
*  from the cssi files on celestrak. it also includes routines to
*  interpolate and convert between the values.
*
*    current :
*              30 sep 15  david vallado
*                           fix jd, jdfrac
*    changes :
*               3 nov 14  david vallado
*                           update to msvs2013 c++
*              14 dec 05  david vallado
*                           misc fixes
*              21 oct 05  david vallado
*                           original version
*       ----------------------------------------------------------------      */

#pragma once

#include <math.h>
#if defined(_WIN32) || defined(WIN32)
#include <io.h>
#endif
#include <stdio.h>
#include <string.h>
#include <stdlib.h>

#include "D:/Codes/LIBRARY/CPP/Libraries/MathTimeLib/MathTimeLib/MathTimeLib.h"  // pi, infinite, undefined


/*    *****************************************************************
*     type definitions
*     *****************************************************************     */

// note the deltapsi/deltaeps are for iau76/fk5 from eop
// x/y/s/a and deltapsi/deltaeps are for computational speed in nutation calcs

typedef struct eopdata
  {
       double xp,   yp,  dut1, lod,  ddpsi,    ddeps,    dx,   dy;
       int    year, mon, day,  mjd,  dat;
       double x,    y,   s,    deltapsi, deltaeps;
  } eopdata;

typedef struct spwdata
  {
       double adjf10,  adjctrf81, adjlstf81, obsf10,   obsctrf81, obslstf81, cp;
       int    year,    mon,       day,       mjd,      bsrn,     nd,        avgap,     c9,
              isn,     q,         aparr[8],  kparr[8], sumkp;
  } spwdata;

const int eopsize = 25000; // 25000 if from 62
const int spwsize = 25000; // 25000 if from 62

namespace EopSpw 
{
	void readspw
		(
		std::vector<spwdata> &spwarr,
		std::string infilename,
		double& jdspwstart, double& jdspwstartFrac
		);

	void readeop
		(
		std::vector<eopdata> &eoparr,
		std::string infilename,
		double& jdeopstart, double& jdeopstartFrac
		);

	void findeopparam
		(
		double  jd, double jdF, char interp, 
		const std::vector<eopdata> eoparr,
		double jdeopstart,
		double& dut1, int& dat,
		double& lod, double& xp, double& yp,
		double& ddpsi, double& ddeps, double& dx, double& dy,
		double& x, double& y, double& s
		);

	void findspwparam
		(
		double jd, double jdF, char interp, char fluxtype, char f81type, char inputtype,
		const std::vector<spwdata> spwarr,
		double jdspwstart,
		double& f107, double& f107bar,
		double& ap, double& avgap, double aparr[8],
		double& kp, double& sumkp, double kparr[8]
		);

	double kp2ap
		(
		double kpin
		);

	double ap2kp
		(
		double apin
		);

};  // namespace

#endif
