#ifndef _astPert_h_
#define _astPert_h_
/* --------------------------------------------------------------------
*
*                                astpert.h
*
*    this file contains miscallaneous propagation and perturbation functions.
*
*    current :
*               4 may 09  david vallado
*                           misc updates
*    changes :
*               4 may 09  david vallado
*                           original baseline
---------------------------------------------------------------------- */

#include <stdio.h>
#include <math.h>
#include <string.h>
#include <stdlib.h>
#include <vector>
#include <ctime> // timing 

#include "D:/Codes/LIBRARY/CPP/Libraries/MathTimeLib/MathTimeLib/MathTimeLib.h"  // pi, twopi, edirection
#include "D:/Codes/LIBRARY/CPP/Libraries/AstroLib/AstroLib/AstroLib.h"  // re, mu, jpldedata, equintype
#include "D:/Codes/LIBRARY/CPP/Libraries/eopspw/eopspw/eopspw.h"  // eopdata, spwdata, eopsize, spwsize
#include "D:/Codes/LIBRARY/CPP/Libraries/SGP4/SGP4/SGP4.h"  // gravconsttype, elsetrec, SGP4Version
#include "D:/Codes/LIBRARY/CPP/Libraries/SGP4DC/SGP4DC/SGP4DC.h"  // obsrec, senrec, WGS72MU, SGP4DCVersion

#pragma once


/*    *****************************************************************
*     type definitions
*     *****************************************************************     */

// structure for gravity field data
typedef struct gravityModelData
{
	double c[2200][2200]; double s[2200][2200];
	double cNor[2200][2200]; double sNor[2200][2200];
	double cSig[2200][2200]; double sSig[2200][2200];
	int numrecsobs; 
	double mu, re;
} gravityModelData;

const int gravsize = 2202; // 2202



namespace astPert
{
	// in mathtimelib:
//#define mu          (3.986004415e5)  /* kg3/s2 */
//#define	re          (6378.1363)  // km
#define j2          (0.0010826267)
#define j3          (-0.0000025327)
#define j4          (-0.0000016196)
#define magr3s      (149597870.7)  // km
#define muSun       (1.32712428e11)  // kg3/s2  1.327122e11 STK values, no difference
#define muMoon      (4902.799)       // kg3/s2  4902.90280030555
#define pSR         (4.56e-9)        // e-6 N/m2 usual, kg km/m2 make sure in km!!
#define NewtonToKgF (1.0/9.796432222)


// control output in routines
#define show        ('n')
#define testr       ('n')  // 'y' if test case comparison, 'n' for hpop compare


	void itermeanoscZonal
		(
		double jdutc,
		double jdutcF,
		double dat, double dut1,
		double rosc[3], double vosc[3],
		double rmean[3], double vmean[3]
		);

	void itermeanosc3body
		(
		double jdutc,
		double jdutcF,
		double dat, double dut1,
		double rosc[3], double vosc[3],
		char interp,
		const std::vector<jpldedata> &jpldearr,
		double jdjpldestart,
		double rmean[3], double vmean[3]
		);

	void itermeanoscSRP
		(
		double jdutc,
		double jdutcF,
		double dat, double dut1,
		double rosc[3], double vosc[3],
		double cram, char interp,
		const std::vector<jpldedata> &jpldearr,
		double jdjpldestart,
		double rmean[3], double vmean[3]
		);
		
    void deriv
		(
		std::vector< std::vector<double> > Xeci,
		std::vector< std::vector<double> > &Xdoteci,
		double& rmag
		);

	void rk4
		(
		double jdutc,
		double jdutcF,
		double dtsec,
		std::vector< std::vector<double> > Xeci,
		double& rmag,
		int derivType,
		double cdam,
		double cram,
		char interp, char opt,
		const std::vector<jpldedata> &jpldearr,
		double jdjpldestart,
		const iau80data &iau80rec,
		double jdeopstart,
		const std::vector<eopdata> &eoparr,
		std::vector< std::vector<double> > &Xfeci
		);

	void rkF45
		(
		double jdutc,
		double jdutcF,
		double dtsec,
		std::vector< std::vector<double> > Xeci,
		double& rmag,
		int derivType,
    	double cdam,
		double cram,
		char interp, char opt,
		const std::vector<jpldedata> &jpldearr,
		double jdjpldestart,
		const iau80data &iau80rec,
		double jdeopstart,
		const std::vector<eopdata> &eoparr,
		std::vector< std::vector<double> > &Xfeci
		);

	void testPropOrbit
		(
		double jd, double jdF,
		double cdam, double cram,
		int derivType, char interp, char opt,
		double rosc[3], double vosc[3],
		double rmean[3], double vmean[3],
		double days
		);

	void pderiv
		(
		double jdutc, double jdutcF,
		std::vector< std::vector<double> > Xeci,
		double& rmag,
		double rsun[3], double rmoon[3],
		int derivtype,
		double cdam, double cram,
		std::vector< std::vector<double> > trans,
		std::vector< std::vector<double> > pm,
		double omegaearth[3],
		std::vector< std::vector<double> > &Xdoteci
		);

	void AccTwoBody
		(
		double reci[3], double accel[3], double& rmag
		);

	void AccNonSph
		(
		double recef[3], double accel[3], double& rmag
		);

	void Acc3Body
		(
		double jdutc, double jdutcF, double reci[3], double& rmag, double rsuneci[3], double rmooneci[3],
		double accel[3]
		);

	void AccDrag
		(
		double recef[3], double vecef[3], double& rmag,
		double omegaearth, double cdam,
		double accel[3]
		);

	void AccSRP
		(
		double jdutc, double jdutcF, double reci[3], double rsuneci[3],
		double cram, double accel[3]
		);

	void zonalPeriodic
		(
		double beta, double beta2, double p, double n, double a, double ecc, double incl, double argp, double nu, double magr,
		double sini, double cosi, double m,
		double& dndtp, double& dedtp, double& didtp, double& draandtp, double& dargpdtp, double& dmdtp, double& dmdarpdtp
		);
		
	void zonalperts
		(
		double jdutc, double jdutcF, double rmean[3], double vmean[3],
		double dtsec,
		double tut1, double rosc[3], double vosc[3], double rmean1[3], double vmean1[3]
		);

	void dragperts
		(
		double jdtdb, double jdtdbF, double rmean[3], double vmean[3],
		double dtsec,
		double tut1, double cdam, double rosc[3], double vosc[3], double rmean1[3], double vmean1[3]
		);

	void thirdbodyPeriodic
		(
		double beta1, double n1, double ecc1, double m1, double eccanom1, double mrat3, double mrat4, double e11, double e21, double e31,
		double sini1, double cosi1, double sinargp1, double cosargp1,
		double& dndtp, double& dedtp, double& didtp, double& draandtp, double& dargpdtp, double& dmdtp, double& x3, double& x4
		);

	void thirdbodyperts
		(
		double jdtdb, double jdtdbF, double rmean[3], double vmean[3], 
		char interp, 
		const std::vector<jpldedata> &jpldearr,
		double jdjpldestart,
		double dtsec,
		double tut1, double rosc[3], double vosc[3], double rmean1[3], double vmean1[3]
		);

	void srpPeriodic
		(
		double fsrp, double beta1, double n1, double a1, double ecc1, double eccanom1,
		double d11, double d21, double d31, double d41, double sini1,
		double& dndtp, double& dedtp, double& didtp, double& draandtp, double& x7, double& x8
		);

	void srpperts
		(
		double jdtdb, double jdtdbF, double rmean[3], double vmean[3],
		char interp, char opt, char fullsun,
		double dtsec,
		const std::vector<jpldedata> &jpldearr,
		double jdjpldestart,
		double tut1, double cram, double rosc[3], double vosc[3], double rmean1[3], double vmean1[3]
		);

}  // namespace


#endif
