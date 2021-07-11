#ifndef _MathTimeLib_h_
#define _MathTimeLib_h_
/* --------------------------------------------------------------------
*
*                                MathTimeLib.h
*
*    this file contains miscallaneous math functions. vectors use the usual 0-1-2
*    indexing scheme.
*
*    current :
*              30 sep 15  david vallado
*                           fix jd, jdfrac
*    changes :
*               3 nov 14  david vallado
*                           update to msvs2013 c++
*              11 dec 07  david vallado
*                           fixes to matrix operations
*              10 aug 06  david vallado
*                           use fmod
*              20 jul 05  david vallado
*                           2nd printing baseline
*              14 may 01  david vallado
*                           2nd edition baseline
*              23 nov 87  david vallado
*                           original baseline
* ----------------------------------------------------------------------      */

#include <math.h>
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <vector>

#pragma once


namespace MathTimeLib
{
	//global interfaces
#define pi 3.14159265358979323846
#define twopi 2.0 * pi
#define infinite  999999.9
#define undefined 999999.1
#define mum 3.986004415e14        // m^3/s^2 stk uses .4415
#define mu 398600.4415            // km^3/s^2 stk uses .4415
#define re 6378.1363              // km  stk uses .1363
#define velkmps 7.9053657160394282
#define earthrot 7.29211514670698e-05  // older rad/s
#define spdLit 2.99792458e8       // speed of light m/s
//#define  earthrot 7.292115e-05  // rad/s


// global object definitions
	typedef enum
	{
		eTo, eFrom
	} edirection;


	/*
	*  because c++ has no multi-dimensioned, dynamic allocated matrix, I built one.
	*  the matrix type uses the standard vector to allow dynamic re-sizing during runtime.
	*  the first time a variable is defined, you define it as follows
	*       matrix(4,3);
	*  afterwards, you can just pass the variable.
	*       matrix mat3;
	*  anytime you create a new matrix inside a procedure or function, you must re-size
	*  the matrix. use the following statements to re-size!!
	*       mat3.resize(rows);  // set the # of rows
	*       for (std::vector< std::vector<double> >::iterator it=mat3.begin(); it != mat3.end();++it)
	*           it->resize(cols);  // set the # of cols
	*  the type definition has no size so you can use any size you like ... and then change it :-)
	*/
	// typedef matrix (std::vector< std::vector<double> >);

	double  sgn
	(
		double x
	);

	double round
	(
		double x
	);

	double  acosh
	(
		double xval
	);

	double  asinh
	(
		double xval
	);

	double  cot
	(
		double xval
	);

	double  dot
	(
		double x[3], double y[3]
	);

	double  mag
	(
		double x[3]
	);

	void  cross
	(
		double vec1[3], double vec2[3], double outvec[3]
	);

	void  norm
	(
		double vec[3],
		double outvec[3]
	);

	double angle
	(
		double vec1[3],
		double vec2[3]
	);

	void  rot1
	(
		double vec[3],
		double xval,
		double outvec[3]
	);

	void  rot2
	(
		double vec[3],
		double xval,
		double outvec[3]
	);

	void  rot3
	(
		double vec[3],
		double xval,
		double outvec[3]
	);

	void  rot1mat
	(
		double xval,
		std::vector< std::vector<double> >& outmat
	);

	void  rot2mat
	(
		double xval,
		std::vector< std::vector<double> >& outmat
	);

	void  rot3mat
	(
		double xval,
		std::vector< std::vector<double> >& outmat
	);

	void  addvec
	(
		double a1, double vec1[3],
		double a2, double vec2[3],
		double vec3[3]
	);

	void  addvec3
	(
		double a1, double vec1[3],
		double a2, double vec2[3],
		double a3, double vec3[3],
		double vec4[3]
	);

	void  writevec
	(
		char title[10],
		double r[3], double v[3], double a[3]
	);

	void  matinverse
	(
		std::vector< std::vector<double> > mat,
		int  order,
		std::vector< std::vector<double> >& matinv
	);

	void  matvecmult
	(
		std::vector< std::vector<double> > mat,
		double vec[3],
		double vecout[3]
	);

	void  matmult
	(
		std::vector< std::vector<double> > mat1,
		std::vector< std::vector<double> > mat2,
		std::vector< std::vector<double> >& mat3,
		int mat1r, int mat1c, int mat2c
	);

	void vecouter
	(
		double vec1[3], double vec2[3],
		std::vector< std::vector<double> >& mat
	);

	void matadd
	(
		std::vector< std::vector<double> > mat1,
		std::vector< std::vector<double> > mat2,
		int mat1r, int mat1c,
		std::vector< std::vector<double> >& mat3
	);

	void matsub
	(
		std::vector< std::vector<double> > mat1,
		std::vector< std::vector<double> > mat2,
		int mat1r, int mat1c,
		std::vector< std::vector<double> >& mat3
	);

	void  mattrans
	(
		std::vector< std::vector<double> > mat1,
		std::vector< std::vector<double> >& mat2,
		int mat1r, int mat1c
	);

	void ludecomp
	(
		std::vector< std::vector<double> >& lu,
		std::vector< int >& indexx,
		//          double lu[3][3],
		//          double indexx[3],
		int order
	);


	void lubksub
	(
		std::vector< std::vector<double> > lu,
		std::vector< int > indexx,
		//          double lu[3][3],
		//          double indexx[3],
		int order,
		std::vector< double >& b
		//          double b[3]
	);


	double determinant
	(
		std::vector< std::vector<double> > mat1,
		int  order
	);

	void cholesky
	(
		std::vector< std::vector<double> > a,
		int n,
		std::vector< std::vector<double> >& ret
	);

	void  writemat
	(
		char matname[30],
		std::vector< std::vector<double> > mat,
		int row, int col
	);

	void  writeexpmat
	(
		char matname[30],
		std::vector< std::vector<double> > mat,
		int row, int col
	);

	void  cubicspl
	(
		double p1, double p2, double p3, double p4,
		double& acu0, double& acu1, double& acu2, double& acu3
	);

	void  quadratic
	(
		double a, double b, double c, char opt,
		double& r1r, double& r1i, double& r2r, double& r2i
	);

	void  cubic
	(
		double a3, double b2, double c1, double d0, char opt,
		double& r1r, double& r1i, double& r2r, double& r2i, double& r3r, double& r3i
	);

	double cubicinterp
	(
		double p1a, double p1b, double p1c, double p1d, double p2a, double p2b,
		double p2c, double p2d, double valuein
	);

	double factorial
	(
		long n
	);


	// -----------------------------------------------------------------------------------------
	//                                       time functions
	// -----------------------------------------------------------------------------------------

	int getmon
	(
		char instr[3]
	);

	int getday
	(
		char instr[3]
	);

	int dayofweek
	(
		double jd
	);

	void jday
	(
		int year, int mon, int day, int hr, int minute, double sec,
		double& jd, double& jdFrac
	);

	void    jdayall
	(
		int year, int mon, int day, int hr, int minute, double sec,
		char whichtype, double& jd
	);

	void    days2mdhms
	(
		int year, double days,
		int& mon, int& day, int& hr, int& minute, double& sec
	);

	void    invjday
	(
		double jd, double jdFrac,
		int& year, int& mon, int& day,
		int& hr, int& minute, double& sec
	);

	void    finddays
	(
		int year, int month, int day, int hr, int minute,
		double sec, double& days
	);

	void    hms_sec
	(
		int& hr, int& min, double& sec, edirection direct, double& utsec
	);

	void    hms_ut
	(
		int& hr, int& min, double& sec, edirection direct, double& ut
	);

	void    hms_rad
	(
		int& hr, int& min, double& sec, edirection direct, double& hms
	);

	void    dms_rad
	(
		int& deg, int& min, double& sec, edirection direct, double& dms
	);

	double jd2sse
	(
		double jd
	);

	void    convtime
	(
		int year, int mon, int day, int hr, int min, double sec, int timezone,
		double dut1, int dat,
		double& ut1, double& tut1, double& jdut1, double& jdut1Frac, double& utc, double& tai,
		double& tt, double& ttt, double& jdtt, double& jdttFrac, double& tcg, double& tdb,
		double& ttdb, double& jdtdb, double& jdtdbFrac, double& tcb
	);

};  // namespace MathTimeLib

#endif
