#ifndef _SGP4h_
#define _SGP4h_
/*     ----------------------------------------------------------------
*
*                                 SGP4.h
*
*    this file contains the sgp4 procedures for analytical propagation
*    of a satellite. the code was originally released in the 1980 and 1986
*    spacetrack papers. a detailed discussion of the theory and history
*    may be found in the 2006 aiaa paper by vallado, crawford, hujsak,
*    and kelso.
*
*    current :
*               7 dec 15  david vallado
*                           fix jd, jdfrac
*    changes :
*               3 nov 14  david vallado
*                           update to msvs2013 c++
*              30 Dec 11  david vallado
*                           consolidate updated code version
*              30 Aug 10  david vallado
*                           delete unused variables in initl
*                           replace pow inetger 2, 3 with multiplies for speed
*               3 Nov 08  david vallado
*                           put returns in for error codes
*              29 sep 08  david vallado
*                           fix atime for faster operation in dspace
*                           add operationmode for afspc (a) or improved (i)
*                           performance mode
*              20 apr 07  david vallado
*                           misc fixes for constants
*              11 aug 06  david vallado
*                           chg lyddane choice back to strn3, constants, misc doc
*              15 dec 05  david vallado
*                           misc fixes
*              26 jul 05  david vallado
*                           fixes for paper
*                           note that each fix is preceded by a
*                           comment with "sgp4fix" and an explanation of
*                           what was changed
*              10 aug 04  david vallado
*                           2nd printing baseline working
*              14 may 01  david vallado
*                           2nd edition baseline
*                     80  norad
*                           original baseline
*       ----------------------------------------------------------------      */

#pragma once

#include <math.h>
#include <stdio.h>
#include <string.h>
#include <iostream>

#define SGP4Version "SGP4 Version 2016-03-09"

// -------------------------- structure declarations ----------------------------
typedef enum
{
  wgs72old,
  wgs72,
  wgs84
} gravconsttype;

/*
typedef struct elsetrec
{
  long int  satnum;
  int       epochyr, epochtynumrev;
  int       error;
  char      operationmode;
  char      init, method;

  // Near Earth
  int    isimp;
  double aycof  , con41  , cc1    , cc4      , cc5    , d2      , d3   , d4    ,
         delmo  , eta    , argpdot, omgcof   , sinmao , t       , t2cof, t3cof ,
         t4cof  , t5cof  , x1mth2 , x7thm1   , mdot   , nodedot, xlcof , xmcof ,
         nodecf;

  // Deep Space 
  int    irez;
  double d2201  , d2211  , d3210  , d3222    , d4410  , d4422   , d5220 , d5232 ,
         d5421  , d5433  , dedt   , del1     , del2   , del3    , didt  , dmdt  ,
         dnodt  , domdt  , e3     , ee2      , peo    , pgho    , pho   , pinco ,
         plo    , se2    , se3    , sgh2     , sgh3   , sgh4    , sh2   , sh3   ,
         si2    , si3    , sl2    , sl3      , sl4    , gsto    , xfact , xgh2  ,
         xgh3   , xgh4   , xh2    , xh3      , xi2    , xi3     , xl2   , xl3   ,
         xl4    , xlamo  , zmol   , zmos     , atime  , xli     , xni;

  double a, altp, alta, epochdays, jdsatepoch, jdsatepochF, nddot, ndot,
	     bstar, rcse, inclo, nodeo, ecco, argpo, mo, no_kozai;
  // sgp4fix add new variables from tle
  char  classification, intldesg[11];
  int   ephtype;
  long  elnum    , revnum;
  // sgp4fix add unkozai'd variable
  double no_unkozai;
  // sgp4fix add singly averaged variables
  double am     , em     , im     , Om       , om     , mm      , nm;
  // sgp4fix add constant parameters to eliminate mutliple calls during execution
  double tumin, mu, radiusearthkm, xke, j2, j3, j4, j3oj2;

  //       Additional elements to capture relevant TLE and object information:       
  long dia_mm; // RSO dia in mm
  double period_sec; // Period in seconds
  unsigned char active; // "Active S/C" flag (0=n, 1=y) 
  unsigned char not_orbital; // "Orbiting S/C" flag (0=n, 1=y)  
  double rcs_m2; // "RCS (m^2)" storage  

} elsetrec;
*/
typedef char char_11[11];

#define SATREC_FIELDS                      \
  SATREC(long int, satnum, "%li")          \
  SATREC(int, epochyr, "%i")               \
  SATREC(int, epochtynumrev, "%i")         \
  SATREC(int, error, "%i")                 \
  SATREC(char, operationmode, "%c")        \
  SATREC(char, init, "%c")                 \
  SATREC(char, method, "%c")               \
  SATREC(int, isimp, "%i")                 \
  SATREC(double, aycof, "%lf")             \
  SATREC(double, con41, "%lf")             \
  SATREC(double, cc1, "%lf")               \
  SATREC(double, cc4, "%lf")               \
  SATREC(double, cc5, "%lf")               \
  SATREC(double, d2, "%lf")                \
  SATREC(double, d3, "%lf")                \
  SATREC(double, d4, "%lf")                \
  SATREC(double, delmo, "%lf")             \
  SATREC(double, eta, "%lf")               \
  SATREC(double, argpdot, "%lf")           \
  SATREC(double, omgcof, "%lf")            \
  SATREC(double, sinmao, "%lf")            \
  SATREC(double, t, "%lf")                 \
  SATREC(double, t2cof, "%lf")             \
  SATREC(double, t3cof, "%lf")             \
  SATREC(double, t4cof, "%lf")             \
  SATREC(double, t5cof, "%lf")             \
  SATREC(double, x1mth2, "%lf")            \
  SATREC(double, x7thm1, "%lf")            \
  SATREC(double, mdot, "%lf")              \
  SATREC(double, nodedot, "%lf")           \
  SATREC(double, xlcof, "%lf")             \
  SATREC(double, xmcof, "%lf")             \
  SATREC(double, nodecf, "%lf")            \
  SATREC(int, irez, "%i")                  \
  SATREC(double, d2201, "%lf")             \
  SATREC(double, d2211, "%lf")             \
  SATREC(double, d3210, "%lf")             \
  SATREC(double, d3222, "%lf")             \
  SATREC(double, d4410, "%lf")             \
  SATREC(double, d4422, "%lf")             \
  SATREC(double, d5220, "%lf")             \
  SATREC(double, d5232, "%lf")             \
  SATREC(double, d5421, "%lf")             \
  SATREC(double, d5433, "%lf")             \
  SATREC(double, dedt, "%lf")              \
  SATREC(double, del1, "%lf")              \
  SATREC(double, del2, "%lf")              \
  SATREC(double, del3, "%lf")              \
  SATREC(double, didt, "%lf")              \
  SATREC(double, dmdt, "%lf")              \
  SATREC(double, dnodt, "%lf")             \
  SATREC(double, domdt, "%lf")             \
  SATREC(double, e3, "%lf")                \
  SATREC(double, ee2, "%lf")               \
  SATREC(double, peo, "%lf")               \
  SATREC(double, pgho, "%lf")              \
  SATREC(double, pho, "%lf")               \
  SATREC(double, pinco, "%lf")             \
  SATREC(double, plo, "%lf")               \
  SATREC(double, se2, "%lf")               \
  SATREC(double, se3, "%lf")               \
  SATREC(double, sgh2, "%lf")              \
  SATREC(double, sgh3, "%lf")              \
  SATREC(double, sgh4, "%lf")              \
  SATREC(double, sh2, "%lf")               \
  SATREC(double, sh3, "%lf")               \
  SATREC(double, si2, "%lf")               \
  SATREC(double, si3, "%lf")               \
  SATREC(double, sl2, "%lf")               \
  SATREC(double, sl3, "%lf")               \
  SATREC(double, sl4, "%lf")               \
  SATREC(double, gsto, "%lf")              \
  SATREC(double, xfact, "%lf")             \
  SATREC(double, xgh2, "%lf")              \
  SATREC(double, xgh3, "%lf")              \
  SATREC(double, xgh4, "%lf")              \
  SATREC(double, xh2, "%lf")               \
  SATREC(double, xh3, "%lf")               \
  SATREC(double, xi2, "%lf")               \
  SATREC(double, xi3, "%lf")               \
  SATREC(double, xl2, "%lf")               \
  SATREC(double, xl3, "%lf")               \
  SATREC(double, xl4, "%lf")               \
  SATREC(double, xlamo, "%lf")             \
  SATREC(double, zmol, "%lf")              \
  SATREC(double, zmos, "%lf")              \
  SATREC(double, atime, "%lf")             \
  SATREC(double, xli, "%lf")               \
  SATREC(double, xni, "%lf")               \
  SATREC(double, a, "%lf")                 \
  SATREC(double, altp, "%lf")              \
  SATREC(double, alta, "%lf")              \
  SATREC(double, epochdays, "%lf")         \
  SATREC(double, jdsatepoch, "%lf")        \
  SATREC(double, jdsatepochF, "%lf")       \
  SATREC(double, nddot, "%lf")             \
  SATREC(double, ndot, "%lf")              \
  SATREC(double, bstar, "%lf")             \
  SATREC(double, rcse, "%lf")              \
  SATREC(double, inclo, "%lf")             \
  SATREC(double, nodeo, "%lf")             \
  SATREC(double, ecco, "%lf")              \
  SATREC(double, argpo, "%lf")             \
  SATREC(double, mo, "%lf")                \
  SATREC(double, no_kozai, "%lf")          \
  SATREC(char, classification, "%c")       \
  SATREC(char_11, intldesg, "%10s")        \
  SATREC(int, ephtype, "%i")               \
  SATREC(long, elnum, "%li")               \
  SATREC(long, revnum, "%li")              \
  SATREC(double, no_unkozai, "%lf")        \
  SATREC(double, am, "%lf")                \
  SATREC(double, em, "%lf")                \
  SATREC(double, im, "%lf")                \
  SATREC(double, Om, "%lf")                \
  SATREC(double, om, "%lf")                \
  SATREC(double, mm, "%lf")                \
  SATREC(double, nm, "%lf")                \
  SATREC(double, tumin, "%lf")             \
  SATREC(double, mu, "%lf")                \
  SATREC(double, radiusearthkm, "%lf")     \
  SATREC(double, xke, "%lf")               \
  SATREC(double, j2, "%lf")                \
  SATREC(double, j3, "%lf")                \
  SATREC(double, j4, "%lf")                \
  SATREC(double, j3oj2, "%lf")             \
  SATREC(long, dia_mm, "%li")              \
  SATREC(double, period_sec, "%lf")        \
  SATREC(unsigned char, active, "%u")      \
  SATREC(unsigned char, not_orbital, "%u") \
  SATREC(double, rcs_m2, "%lf")

typedef struct elsetrec
{
#define SATREC(type, name, format) type name;
  SATREC_FIELDS
#undef SATREC
} elsetrec;

namespace SGP4Funcs
{

  //	public class SGP4Class
  //	{

  bool sgp4init(
      gravconsttype whichconst, char opsmode, const int satn, const double epoch,
      const double xbstar, const double xndot, const double xnddot, const double xecco, const double xargpo,
      const double xinclo, const double xmo, const double xno,
      const double xnodeo, elsetrec &satrec);

  bool sgp4(
      // no longer need gravconsttype whichconst, all data contained in satrec
      elsetrec &satrec, double tsince,
      double r[3], double v[3]);

  bool sgp4_pos(
      // no longer need gravconsttype whichconst, all data contained in satrec
      elsetrec &satrec, double tsince,
      double r[3]);

  void getgravconst(
      gravconsttype whichconst,
      double &tumin,
      double &mu,
      double &radiusearthkm,
      double &xke,
      double &j2,
      double &j3,
      double &j4,
      double &j3oj2);

  // older sgp4io methods
  void twoline2rv(
      char longstr1[130], char longstr2[130],
      char typerun, char typeinput, char opsmode,
      gravconsttype whichconst,
      double &startmfe, double &stopmfe, double &deltamin,
      elsetrec &satrec);
  void omm2rv(
      char *OBJECT_ID,
      char *EPOCH,
      double MEAN_MOTION,
      double ECCENTRICITY,
      double INCLINATION,
      double RA_OF_ASC_NODE,
      double ARG_OF_PERICENTER,
      double MEAN_ANOMALY,
      double GM,
      signed char EPHEMERIS_TYPE,
      char *CLASSIFICATION_TYPE,
      uint32_t NORAD_CAT_ID,
      uint32_t ELEMENT_SET_NO,
      double REV_AT_EPOCH,
      double BSTAR,
      double MEAN_MOTION_DOT,
      double MEAN_MOTION_DDOT,
      char typerun, char typeinput, char opsmode,
      gravconsttype whichconst,
      double &startmfe, double &stopmfe, double &deltamin,
      elsetrec &satrec);

  // older sgp4ext methods
  double gstime(
      double jdut1);

  double sgn(
      double x);

  double mag(
      double x[3]);

  void cross(
      double vec1[3], double vec2[3], double outvec[3]);

  double dot(
      double x[3], double y[3]);

  double angle(
      double vec1[3],
      double vec2[3]);

  void newtonnu(
      double ecc, double nu,
      double &e0, double &m);

  double asinh(
      double xval);

  void rv2coe(
      double r[3], double v[3], const double mu,
      double &p, double &a, double &ecc, double &incl, double &omega, double &argp,
      double &nu, double &m, double &arglat, double &truelon, double &lonper);

  void jday(
      int year, int mon, int day, int hr, int minute, double sec,
      double &jd, double &jdFrac);

  void days2mdhms(
      int year, double days,
      int &mon, int &day, int &hr, int &minute, double &sec);

  void invjday(
      double jd, double jdFrac,
      int &year, int &mon, int &day,
      int &hr, int &minute, double &sec);

} // namespace SGP4Funcs

#endif
