#ifndef _sgp4dc_
#define _sgp4dc_
/* -----------------------------------------------------------------------------
*
*                               sgp4dc.h
*
*    this file contains the differential corrections routines using the sgp4
*    analytical propagation code. a detailed discussion of the theory and history
*    may be found in the 2008 aiaa paper by vallado and crawford.
*
*                            companion code for
*               fundamentals of astrodynamics and applications
*                                    2007
*                              by david vallado
*
*       (w) 719-573-2600, email dvallado@agi.com
*
*    current :
*               6 aug 08  david vallado
*                           add operationmode for afspc (a) or improved (i)
*    changes :
*              18 jun 08  david vallado
*                           original version
* --------------------------------------------------------------------------- */

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <math.h>
#include <vector.h>
#include <assert.h>

#include "sgp4unit.h"
#include "astmath.h"
#include "asttime.h"
#include "ast2body.h"
#include "astiod.h"                                                       `
#include "coordfk5.h"
#include "eopspw.h"

// note that although a version is given here, a baseline version will be output
// when the obs case difficulties are resolved.
#define SGP4DCVersion      "SGP4DC Version 2008-09-04"

#define pi 3.14159265358979323846
#define twopi 2.0*3.14159265358979323846
//#define statesize (7) // 7 if solve for bstar, else 6


//   elsetrec satrecold;

// Individual obs types are as follows:
//                     0 Rng
//                     1 Az, El
//                     2 Rng, Az, El
//                     3 TRtAsc, TDecl
//                     4 x y z xdot ydot zdot bstar

typedef struct obsrec
   {
     int    sennum;
     long   satnum;
     int    year, mon, day, hr, min;
     double jd, sec, dtmin, lst;
     int    error;
     char   init, method;
     double rsecef[3], vsecef[3];
     int    obstype;
     double x, y, z, xdot, ydot, zdot, bstar,
            rng, az, el, drng, daz, del,
            rtasc, decl, trtasc, tdecl;
   } obsrec;

typedef struct senrec
   {
     int sennum;
     char senname[30];
     double senlat, senlon, senalt,
            rngmin, rngmax, azmin, azmax, elmin, elmax,
            biasrng,  biasaz,  biasel,  biasdrng, biasdaz, biasdel,
            biastrtasc, biastdecl,
            noisex, noisey, noisez, noisexdot, noiseydot, noisezdot, noisebstar,
            noiserng, noiseaz, noiseel, noisedrng, noisedaz, noisedel,
            noisetrtasc, noisetdecl;
    } senrec;

void dsvbksb
     (
       std::vector< std::vector<double> > u,
       std::vector< std::vector<double> > w,
       std::vector< std::vector<double> > v,
       int m, int n,
       std::vector< std::vector<double> > b,
       std::vector< std::vector<double> > &dx 
     );    


void printtle( elsetrec );

int dsvdcmp
   (
     std::vector< std::vector<double> > &a, int m, int n,
     std::vector< std::vector<double> > &w,
     std::vector< std::vector<double> > &v
   );
//      std::vector< std::vector<double> > &u,

void getsensorparams
   (
     int sennum,
     senrec& currsenrec
   );

void state2satrec
   (
     std::vector<double> &xnom,
     std::vector< std::vector<double> > &scalef,
     char statetype,   int statesize,
     edirection direct,
     elsetrec& satrec
   );

void findatwaatwb
   (
     int firstob, int lastob, int statesize,
     double percentchg, double deltaamtchg, gravconsttype whichconst,
     elsetrec satrec,
     obsrec obsrecarr[10000],
     char statetype,
     std::vector< std::vector<double> > &scalef,
     std::vector<double> &xnom,
     double& drng2,double& daz2,double& del2,double& ddrng2,double& ddaz2,
     double& ddel2,double& dtrtasc2, double& dtdecl2,
     double& dx2, double& dy2, double& dz2, double& dxdot2,
     double& dydot2, double& dzdot2,
     std::vector< std::vector<double> > &atwa,
     std::vector< std::vector<double> > &atwb,
     std::vector< std::vector<double> > &atw,
     std::vector< std::vector<double> > &b
   );

void finitediff
   (
     gravconsttype whichconst,
     int pertelem, double percentchg,  double deltaamtchg, char statetype, int statesize,
     elsetrec& satrec, std::vector<double> xnom,
     std::vector< std::vector<double> > &scalef,
     double &deltaamt
   );

void leastsquares
   (
     double percentchg, double deltaamtchg, double epsilon, gravconsttype whichconst,
     elsetrec &satrec, elsetrec satrecx,
     char typeans, char statetype,
     int firstob, int lastob, int statesize,
     obsrec obsrecarr[10000], int &loops,
     std::vector< std::vector<double> > &scalef,
     std::vector<double> &xnom,
     std::vector< std::vector<double> > &x,
     std::vector< std::vector<double> > &dx,
     std::vector< std::vector<double> > &atwai,
     std::vector< std::vector<double> > &atwa,
     std::vector< std::vector<double> > &atwb,
     FILE *outfile, FILE *outfile1
   );


#endif




