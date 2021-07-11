#ifndef _AstroLib_h_
#define _AstroLib_h_
/* --------------------------------------------------------------------
*
*                                AstroLib.h
*
*    this file contains miscallaneous two-body motion functions. Several areas are covered:
*    coordinate transformations, 2body routines, jpl ephemeris conversions, covariance operations
*
*    current :
*              30 sep 15  david vallado
*                           fix jd, jdfrac
*    changes :
*               3 nov 14  david vallado
*                           update to msvs2013 c++
*               4 may 09  david vallado
*                           misc updates
*              23 feb 07  david vallado
*                           3rd edition baseline
*               6 dec 05  david vallado
*                           add ijk2ll
*              20 jul 05  david vallado
*                           2nd printing baseline
*              14 may 01  david vallado
*                           2nd edition baseline
*              23 nov 87  david vallado
*                           original baseline
  ----------------------------------------------------------------------      */

#include <math.h>
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <vector>
#include <stdint.h>

  // be sure to update to your specific paths!!
  // " " tells the compiler to look in this directory first, usually the parent directory
  // you can leave generic as MathTimeLib.h, but you have to set the include directory in the property pages
#include "D:/Codes/LIBRARY/CPP/Libraries/MathTimeLib/MathTimeLib/MathTimeLib.h"  // globals, edirection


#pragma once

typedef enum
{
	e80,
	e96,
	e00a,
	e00b,
	e00cio
} eOpt;


typedef struct iau80data
{
	int    iar80[107][6];
	double rar80[107][5];
} iau80data;

typedef struct iau00data
{
	double axs0[1600][2];  // reals
	int a0xi[1600][14];  // integers
	double ays0[1275][2];  // reals
	int a0yi[1275][14];  // integers
	double ass0[66][2];  // reals
	int a0si[66][14];  // integers

	double agst[35][2];  // reals
	int agsti[35][14];  // integers

	double apn[1358][2];  // reals
	int apni[1358][14];  // integers
	double ape[1056][2];  // reals
	int apei[1056][14];  // integers

	double appn[678][8];  // reals
	int appni[678][5];  // integers
	double apln[687][5];  // reals
	int aplni[687][14];  // integers
} iau00data;


const int jpldesize = 60000; // 60000 if from 1957-2100

typedef struct jpldedata
{
	double rsun[3], rmoon[3];
	int    year, mon, day;
	double rsmag, rmmag, mjd;
} jpldedata;



// setup variations on equinoctial elements
typedef enum
{
	ea_m, en_m, ep_m, ea_nu, en_nu, ep_nu
} equintype;



namespace AstroLib
{
	void sethelp
	(
		char& iauhelp, char iauopt
	);


	// -----------------------------------------------------------------------------------------
	//                                       Coordinate system functions
	// -----------------------------------------------------------------------------------------

	void ddpsiddeps_dxdy
	(
		double ttt, double& ddpsi, double& ddeps,
		MathTimeLib::edirection direct,
		double& dx, double& dy
	);

	void iau80in
	(
		std::string EopLoc,
		iau80data& iau80rec
	);

	void iau00in
	(
		std::string EopLoc,
		iau00data& iau00arr
	);

	void fundarg
	(
		double ttt, eOpt opt,
		double& l, double& l1, double& f, double& d, double& omega,
		double& lonmer, double& lonven, double& lonear, double& lonmar,
		double& lonjup, double& lonsat, double& lonurn, double& lonnep,
		double& precrate
	);

	void iau00xys
	(
		double ttt, double ddx, double ddy, eOpt opt,
		const iau00data &iau00arr,
		double& x, double& y, double& s,
		std::vector< std::vector<double> > &nut
	);

	void iau00pn
	(
		double ttt, double ddx, double ddy, eOpt opt,
		const iau00data &iau00arr,
		double x, double y, double s,
		std::vector< std::vector<double> > &nut
	);

	void gstime00
	(
		double jdut1, double deltapsi, double ttt, const iau00data &iau00arr, eOpt opt, double& gst,
		std::vector< std::vector<double> > &st
	);

	double  gstime
	(
		double jdut1
	);

	void    lstime
	(
		double lon, double jdut1, double& lst, double& gst
	);

	void precess
	(
		double ttt, eOpt opt,
		double& psia, double& wa, double& epsa, double& chia,
		std::vector< std::vector<double> > &prec
	);

	void nutation
	(
		double ttt, double ddpsi, double ddeps,
		const iau80data &iau80arr, eOpt opt,
		double& deltapsi, double& deltaeps, double& trueeps, double& meaneps, double& omega,
		std::vector< std::vector<double> > &nut
	);

	void nutation00a
	(
		double ttt, double ddpsi, double ddeps,
		const iau00data &iau00arr, eOpt opt,
		std::vector< std::vector<double> > &tm
	);
		
	void sidereal
	(
		double jdut1, double deltapsi, double meaneps, double omega,
		double lod, int eqeterms, eOpt opt,
		std::vector< std::vector<double> > &st,
		std::vector< std::vector<double> > &stdot
	);

	void polarm
	(
		double xp, double yp, double ttt, eOpt opt, std::vector< std::vector<double> > &pm
	);

	void framebias
	(
		char opt,
		double& term1, double& term2, double& term3, std::vector< std::vector<double> > &fb
	);

	void itrf_gcrf
	(
		double ritrf[3], double vitrf[3], double aitrf[3],
		MathTimeLib::edirection direct,
		double rgcrf[3], double vgcrf[3], double agcrf[3],
		const iau80data &iau80rec, eOpt opt,
		double ttt, double jdut1, double lod, double xp,
		double yp, int eqeterms, double ddpsi, double ddeps,
		std::vector< std::vector<double> > &trans
	);


	void eci_ecef00
	(
		double reci[3], double veci[3],
		MathTimeLib::edirection direct,
		double recef[3], double vecef[3],
		const iau00data &iau00arr, eOpt opt,
		double ttt, double jdut1, double lod, double xp, double yp, int eqeterms, double ddx, double ddy
	);

	void itrf_j2k
	(
		double ritrf[3], double vitrf[3], double aitrf[3],
		MathTimeLib::edirection direct,
		double rj2k[3], double vj2k[3], double aj2k[3],
		const iau80data &iau80rec, eOpt opt,
		double ttt, double jdut1, double lod,
		double xp, double yp, int eqeterms,
		std::vector< std::vector<double> > &trans
	);

	void itrf_mod
	(
		double ritrf[3], double vitrf[3], double aitrf[3],
		MathTimeLib::edirection direct,
		double rmod[3], double vmod[3], double amod[3],
		const iau80data &iau80rec, eOpt opt,
		double ttt, double jdut1, double lod, double xp,
		double yp, int eqeterms, double ddpsi, double ddeps,
		std::vector< std::vector<double> > &trans
	);

	void itrf_tod
	(
		double ritrf[3], double vitrf[3], double aitrf[3],
		MathTimeLib::edirection direct,
		double rtod[3], double vtod[3], double atod[3],
		const iau80data &iau80rec, eOpt opt,
		double ttt, double jdut1, double lod, double xp,
		double yp, int eqeterms, double ddpsi, double ddeps,
		std::vector< std::vector<double> > &trans
	);

	void itrf_pef
	(
		double ritrf[3], double vitrf[3], double aitrf[3],
		MathTimeLib::edirection direct,
		double rpef[3], double vpef[3], double apef[3],
		double ttt, double xp, double yp,
		std::vector< std::vector<double> > &trans
	);

	void pef_gcrf
	(
		double rpef[3], double vpef[3], double apef[3],
		MathTimeLib::edirection direct,
		double rgcrf[3], double vgcrf[3], double agcrf[3],
		const iau80data &iau80rec, eOpt opt,
		double ttt, double jdut1, double lod, int eqeterms,
		double ddpsi, double ddeps
	);

	void tod_gcrf
	(
		double rtod[3], double vtod[3], double atod[3],
		MathTimeLib::edirection direct,
		double rgcrf[3], double vgcrf[3], double agcrf[3],
		const iau80data &iau80rec, eOpt opt,
		double ttt, double ddpsi, double ddeps
	);

	void mod_gcrf
	(
		double rmod[3], double vmod[3], double amod[3],
		MathTimeLib::edirection direct,
		double rgcrf[3], double vgcrf[3], double agcrf[3],
		double ttt
	);

	void teme_eci
	(
		double rteme[3], double vteme[3], double ateme[3],
		MathTimeLib::edirection direct,
		double rgcrf[3], double vgcrf[3], double agcrf[3],
		const iau80data &iau80rec, eOpt opt,
		double ttt, double ddpsi, double ddeps
	);

	void teme_ecef
	(
		double rteme[3], double vteme[3], double ateme[3],
		MathTimeLib::edirection direct,
		double recef[3], double vecef[3], double aecef[3],
		double ttt, double jdut1, double lod, double xp, double yp, int eqeterms
	);



	// -----------------------------------------------------------------------------------------
	//                                       2-body functions
	// -----------------------------------------------------------------------------------------

	void rv2coe
	(
		double r[3], double v[3],
		double& p, double& a, double& ecc, double& incl, double& raan, double& argp,
		double& nu, double& m, double& eccanom, double& arglat, double& truelon, double& lonper
	);

	void coe2rv
	(
		double p, double ecc, double incl, double raan, double argp, double nu,
		double arglat, double truelon, double lonper,
		double r[3], double v[3]
	);

	void rv2eq
	(
		double r[3], double v[3],
		double& a, double& n, double& af, double& ag, double& chi, double& psi, double& meanlonM, double& meanlonNu, int& fr
	);

	void eq2rv
	(
		double a, double af, double ag, double chi, double psi, double meanlon, int fr,
		double r[3], double v[3]
	);

	void eq2coe
	(
		double x1in, double af, double ag, double chi, double psi, double x2in, int fr, equintype typeorbit,
		double& p, double& ecc, double& incl, double& raan, double& argp, double& nu, double& m, double& eccanom, double& arglat, double& truelon, double& lonper
	);

	void findc2c3
	(
		double znew,
		double& c2new, double& c3new
	);

	void findfandg
	(
		double r1[3], double v1[3], double r2[3], double v2[3], double dtsec,
		double x, double c2, double c3, double z,
		int opt,
		double& f, double& g, double& fdot, double& gdot
	);

	void checkhitearth
	(
		double altPad, double r1[3], double v1t[3], double r2[3], double v2t[3], int nrev, char& hitearth
	);

	void kepler
	(
		double ro[3], double vo[3], double dtseco, double r[3], double v[3]
	);

	void pkepler
	(
		double r1[3], double v1[3], double& dtsec, double& ndot, double& nddot, double r2[3], double v2[3]
	);

	void rv2rsw
	(
		double r[3], double v[3],
		double rrsw[3], double vrsw[3], std::vector< std::vector<double> > &transmat
	);

	void rv2pqw
	(
		double r[3], double v[3],
		double rpqw[3], double vpqw[3]
	);

	void rv2ntw
	(
		double r[3], double v[3],
		double rntw[3], double vntw[3], std::vector< std::vector<double> > &transmat
	);

	void newtone
	(
		double ecc, double e0, double& m, double& nu
	);
		
	void newtonm
	(
		double ecc, double m, double& e0, double& nu
	);

	void newtonnu
	(
		double ecc, double nu,
		double& e0, double& m
	);

	void gc_gd
	(
		double&    latgc,
		MathTimeLib::edirection direct,
		double&    latgd
	);

	void ecef2ll
	(
		double recef[3],
		double& latgc, double& latgd, double& lon, double& hellp
	);


	void initjplde
	(
		std::vector<jpldedata> &jpldearr,
		char infilename[140],
		double& jdjpldestart, double& jdjpldestartFrac
	);

	void findjpldeparam
	(
		double  jdtdb, double jdtdbF, char interp,
		const std::vector<jpldedata> &jpldearr,
		double jdjpldestart,
		double rsun[3], double& rsmag,
		double rmoon[3], double& rmmag
	);

	void sun
	(
		double jdtdb, double jdtdbF,
		double rsun[3], double& rtasc, double& decl
	);

	void sunmoonjpl
	(
		double jdtdb, double jdtdbF,
		char interp,
		const std::vector<jpldedata> &jpldearr,
		double jdjpldestart,
		double rsun[3], double& rtascs, double& decls,
		double rmoon[3], double& rtascm, double& declm
	);

	void moon
	(
		double jdtdb, double jdtdbF,
		double rmoon[3], double& rtasc, double& decl
	);

	// IOD type routines
	void site
	(
		double latgd, double lon, double alt,
		double rsecef[3], double vsecef[3]
	);

	/* ------------------------ angles-only techniques -------------------------- */
	void anglesgauss
	(
		double tdecl1, double tdecl2, double tdecl3,
		double trtasc1, double trtasc2, double trtasc3,
		double jd1, double jd2, double jd3,
		double rs1[3], double rs2[3], double rs3[3], double r2[3], double v2[3]
	);

	//void angleslaplace
	//    (
	//      double, double, double, double, double, double, double, double, double,
	//      double[], double[], double[], double[], double[]
	//    );

	/* ------------------------- conversion techniques -------------------------- */
	void radec_azel
	(
		double& rtasc, double& decl, double& lst, double& latgd,
		MathTimeLib::edirection direct,
		double& az, double& el
	);

	void radec_elatlon
	(
		double& rtasc, double& decl,
		MathTimeLib::edirection direct,
		double& ecllat, double& ecllon
	);

	void rv_elatlon
	(
		double rijk[3], double vijk[3],
		MathTimeLib::edirection direct,
		double& rr, double& ecllat, double& ecllon,
		double& drr, double& decllat, double& decllon
	);

	void rv_radec
	(
		double rijk[3], double vijk[3], 
		MathTimeLib::edirection direct,
		double& rr, double& rtasc, double& decl, double& drr, double& drtasc, double& ddecl
	);

	void rv_razel
	(
		double recef[3], double vecef[3], double rsecef[3], double latgd, double lon,
		MathTimeLib::edirection direct,
		double& rho, double& az, double& el, double& drho, double& daz, double& del
	);

	void rv_tradec
	(
		double rijk[3], double vijk[3], double rsijk[3],
		MathTimeLib::edirection direct,
		double& rho, double& trtasc, double& tdecl,
		double& drho, double& dtrtasc, double& dtdecl
	);

	void rvsez_razel
	(
		double rhosez[3], double drhosez[3],
		MathTimeLib::edirection direct,
		double& rho, double& az, double& el, double& drho, double& daz, double& del
	);

	void gibbs
	(
		double r1[3], double r2[3], double r3[3],
		double v2[3], double& theta, double& theta1, double& copa, char error[12]
	);

	void herrgibbs
	(
		double r1[3], double r2[3], double r3[3], double jd1, double jd2, double jd3,
		double v2[3], double& theta, double& theta1, double& copa, char error[12]
	);

	void lambertumins
	(
		double r1[3], double r2[3], int nrev, char df,
		double& kbi, double& tof
	);

	void lambertminT
	(
		double r1[3], double r2[3], char dm, char df, int nrev,
		double& tmin, double& tminp, double& tminenergy
	);

	void lambhodograph
	(
		double r1[3], double v1[3], double r2[3], double p, double ecc, double dnu, double dtsec,
		double v1t[3], double v2t[3]
	);

	static double kbattin(double v);

	static double seebattin(double v);

	void lambertuniv
	(
		double r1[3], double r2[3], char dm, char df, int nrev, double dtsec, double tbi[5][5], double altpad,
		double v1t[3], double v2t[3], char& hitearth, int& error, FILE *outfile
	);

	void lambertbattin
	(
		double r1[3], double r2[3], double v1[3], char dm, char df, int nrev, double dtsec, double tbi[5][5], double altpad,
		double v1t[3], double v2t[3], char& hitearth, int& error, FILE *outfile
	);

	void target
	(
		double rint[3], double vint[3], double rtgt[3], double vtgt[3],
		char dm, char df, char kind, double dtsec,
		double v1t[3], double v2t[3], double dv1[3], double dv2[3], int error
	);


	// -----------------------------------------------------------------------------------------
	//                                       covariance functions
	// -----------------------------------------------------------------------------------------



	// -----------------------------------------------------------------------------------------
	//                                       perturbed functions
	// -----------------------------------------------------------------------------------------



};  // namespace

#endif