//                // testdc
//
// test sgp4 differential correction
//

#include <stdio.h>
#include <math.h>
#include <string.h>
#include <stdlib.h>
#include <fstream>
#include <vector>
#include <iostream>
using namespace std;

#ifdef _WIN32
#include <io.h>
#endif

#include "astmath.h"
#include "asttime.h"
#include "astiod.h"
#include "eopspw.h"
#include "coordfk5.h"

// aiaa 2006 paper version of sgp4
//#include "sgp4ext.h"
#include "sgp4io.h"
#include "sgp4unit.h"

#include "sgp4dc.h"


void processrazel();
void processxyz();
void processtle();



FILE *infile, *obsfile;
FILE *outfile, *outfile1, *outfile2;

   eOpt opt;
   char optteme, iauhelp, nutopt;
   iau80data iau80rec;

   // command line arguments
   char opsmode, typeans, typeodrun, statetype, fname[25];
   double percentchg, deltaamtchg, rmsepsilon;
   int lastob, statesize;
   char batchmode;


/*       ----------------------------------------------------------------     */

// this is specific to Borland c++ and it reads parameters from the command line
void func()
{
   cout << "argc= " << _argc << endl;

   for (int i = 0; i < _argc; ++i)
      cout << _argv[i] << endl;
   typeans    = *_argv[1];
   typeodrun  = *_argv[2];
   statetype  = *_argv[3];
   percentchg = atof(_argv[4]);    // 0.001
   deltaamtchg= atof(_argv[5]);    // 0.0000001
   rmsepsilon = atof(_argv[6]);    // 0.001
   strcpy( fname, _argv[7]);
   lastob     = atof(_argv[8]);    // 200
   statesize  = atof(_argv[9]);    // 6 or 7
}


int main(int argc, char ** argv)
   {
     batchmode = 'y';   // set this to 'n' for debugging mode inthe compiler
     if (batchmode == 'y')
       {
         // read inputs from command line
         func(); // func can get the program arguments from the command line and
                 // send them into global variables.
       }

     printf("%s\n",SGP4Version );
     printf("%s\n",SGP4DCVersion );

     //operationmode = 'a' best understanding of how afspc code works
     //operationmode = 'i' improved sgp4 resulting in smoother behavior
     opsmode = 'a'; //afspc mode, or improved mode
     printf( "opsmode %c \n",opsmode );

     if (batchmode == 'n')
       {
         printf("input typeans bksub or svd b, s \n\n");
         typeans = getchar();
         fflush(stdin);
         printf("input type of run razel, xyz, tle \n\n");
         typeodrun = getchar();
         fflush(stdin);
       }

     printf( "typeans %c \n",typeans );
     printf( "typeodrun %c \n",typeodrun );

     switch (typeodrun)
       {
         case 'r' : processrazel();
         break;
         case 'x' : processxyz();
         break;
         case 't' : processtle();
         break;
       }
   }  // main


/*       ----------------------------------------------------------------     */
void processrazel()
  {
     obsrec obsrecarr[10000];
     elsetrec satrec;
     elsetrec satrecx;
     long search_satno = 0;
     char longstr1[140], longstr2[140];
     char strr[2], monstr[3], strr1;
     char tmp[20], tmpc, units[10], coords[10];
     char typeinput, typerun;
     gravconsttype whichconst;
     int day, mon, year, hr, min, loops, i;
     double startmfe, stopmfe, conv, sec, deltamin, dtsec,p, a, ecc, incl, omega,
            argp, nu, m, arglat, truelon, lonper;

     eopdata eoparr[eopsize];
     double jdeopstart, dut1, ut1, tut1, jdut1, utc, tai, tt, ttt, jdtt, tcg, tdb,
            ttdb, jdtdb, tcb, jde, mfme, lod, xp, yp, ddpsi, ddeps, gst,
            iaudx, dy, icrsx, y, s, deltapsi, deltaeps;
     char interp, keepit;
     int timezone, dat, ktr;
     std::vector< std::vector<double> > trans(3,3);

     double jdepoch, dt1, dt2, dt3, rngest, zrng, zaz, zel, zrtasc, zdecl,
            theta, theta1, copa, rsum[3], vsum[3];
     char error[12];
     int firstob, ii, j, skipped;
     double rpos[3], vpos[3], apos[3], rnom[3], vnom[3], dr[3], dv[3],
            r1[3], r2[3], r3[3], v1[3], v2[3], v3[3], reci[3], veci[3],
            rteme[3], vteme[3], ateme[3], ritrf[3], vitrf[3], aitrf[3], dtmin, jd,
            r1teme[3], v1teme[3],r2teme[3], v2teme[3],r3teme[3], v3teme[3];
     obsrec currobsrec;
     senrec currsenrec;
     double rad = 180.0 / pi;

     if (batchmode == 'n') // needs a default value here in batch mode
         statesize = 7;
     // set these variables up with intial dimensions
     // these must be set here
     // function matrices can be resized
     std::vector<double> xnom(statesize);
     std::vector< std::vector<double> > x(statesize,1);
     std::vector< std::vector<double> > scalef(statesize,1);
     std::vector< std::vector<double> > dx(statesize,1);
     std::vector< std::vector<double> > atwai(statesize,statesize);
     std::vector< std::vector<double> > atwa(statesize,statesize);
     std::vector< std::vector<double> > atwb(statesize,1);

     whichconst = wgs72; // type of run for sgp4
     typerun    = 'c';   // c catalog (just reads tle), v verification, m manual

     if (batchmode == 'n')
       {
         statetype  = 't';   // v vectors, t tle vars, e equinoctial
         printf("input type of elements tle or equin \n\n");
         statetype = getchar();
         fflush(stdin);
//         char *fname = "geos5.inp";
         printf("input obs filename: \n");
         scanf( "%s",fname );
       }
     printf( "state type %c \n",statetype );

     printf( "fname %s \n",fname );

     infile = fopen(fname, "rb");
     outfile = fopen("testdc.out", "w");
     outfile1 = fopen("testdc1.out", "w");

     interp = 'l'; // set eop interpolation to linear
//         printf("input interp n l s \n\n");
     printf( "interp type %c \n",interp );

     sethelp(iauhelp,'n');
     timezone = 0;

     firstob    = 0;
     if (batchmode == 'n')
       {
         percentchg = 0.001;  // percent to change components for finite differencing
         printf("input percentchg for finite differencing (.001) \n");
         scanf( "%lf",&percentchg );
         deltaamtchg = 0.0000001;  // delta amt to change components for finite differencing
         printf("input deltaamtchg to chk in finite differencing (.0000001) \n");
         scanf( "%lf",&deltaamtchg );
         rmsepsilon = 0.0002;  // percent to change components for finite differencing
         printf("input rmsepsilon to quit (.02) \n");
         scanf( "%lf",&rmsepsilon );
         lastob = 10;        // how many obs to fit
         printf("input lastob to fit (10) \n");
         scanf( "%i",&lastob );
         statesize = 7;        // solve for bstar or not
         printf("input statesize (6 or 7) \n");
         scanf( "%i",&statesize );
       }
     printf( "percentchg %lf \n",percentchg );
     printf( "deltaamtchg %lf \n",deltaamtchg );
     printf( "rmsepsilon %lf \n",rmsepsilon );

     fprintf( outfile, "typeans %c \n",typeans );

     // initialize eop data for coordinate system changes
     iau80in( iau80rec );
     initeop (eoparr, jdeopstart);

     if(infile == NULL)
       {
       	  printf("Failed to open %s (%s)", fname, strerror(errno));
       }
       else
       {
          conv = 0.001;
          // read header lines
          do
            {
              fgets( longstr1,91,infile);
              strncpy(strr, &longstr1[0], 1);
              strr[1] = '\0';
            } while ((strcmp(strr, "#")==0) && (feof(infile) == 0));

          // ---- read obs from the input file ----
          strcpy( coords,"Fixed");  // assume earth fixed observations unless specified otherwise
          ii = 0;
             while (feof(infile) == 0)
               {
                 if (ii != 0)
                     fgets( longstr2,120,infile);
                   else
                     strcpy( longstr2, longstr1);  // data for the 1st point
                 strncpy(strr, &longstr2[0], 1);
                 strr[1] = '\0';

                 if (strcmp(strr, "#") != 0)
                   {
                     sscanf( longstr2,"%d",&currobsrec.obstype);
                      switch (currobsrec.obstype)
                        {
                          case 0 :
                            {
                              sscanf(longstr2,"%d %d %d %d %d %d %d %d %lf ",
                                     &currobsrec.obstype, &currobsrec.satnum, &currobsrec.sennum,
                                     &currobsrec.year, &currobsrec.mon, &currobsrec.day,
                                     &currobsrec.hr, &currobsrec.min, &currobsrec.sec,
                                     &currobsrec.rng );
                            }
                          break;
                          case 1 :
                            {
                              sscanf(longstr2,"%d %d %d %d %d %d %d %d %lf %lf ",
                                     &currobsrec.obstype, &currobsrec.satnum, &currobsrec.sennum,
                                     &currobsrec.year, &currobsrec.mon, &currobsrec.day,
                                     &currobsrec.hr, &currobsrec.min, &currobsrec.sec,
                                     &currobsrec.az, &currobsrec.el );
                              currobsrec.az = currobsrec.az / rad;
                              currobsrec.el = currobsrec.el / rad;
                            }
                          break;
                          case 2 :
                            {
                              sscanf(longstr2,"%d %d %d %d %d %d %d %d %lf %lf %lf %lf ",
                                     &currobsrec.obstype, &currobsrec.satnum, &currobsrec.sennum,
                                     &currobsrec.year, &currobsrec.mon, &currobsrec.day,
                                     &currobsrec.hr, &currobsrec.min, &currobsrec.sec,
                                     &currobsrec.rng, &currobsrec.az, &currobsrec.el );
                              currobsrec.az = currobsrec.az / rad;
                              currobsrec.el = currobsrec.el / rad;
                            }
                          break;
                          case 3 :
                            {
                              sscanf(longstr2,"%d %d %d %d %d %d %d %d %lf %lf ",
                                     &currobsrec.obstype, &currobsrec.satnum, &currobsrec.sennum,
                                     &currobsrec.year, &currobsrec.mon, &currobsrec.day,
                                     &currobsrec.hr, &currobsrec.min, &currobsrec.sec,
                                     &currobsrec.trtasc, &currobsrec.tdecl );
                              currobsrec.trtasc = currobsrec.trtasc / rad;
                              currobsrec.tdecl = currobsrec.tdecl / rad;
                            }
                          break;
                        } // case obstype

                      year = currobsrec.year;
                      mon  = currobsrec.mon;
                      day  = currobsrec.day;
                      hr   = currobsrec.hr;
                      min  = currobsrec.min;
                      sec  = currobsrec.sec;
                      mfme = hr * 60 + min + sec/60.0;
                      jday( year, mon, day, 0.0, 0.0, 0.0,  currobsrec.jd );
                      if (ii == 1) // need the second one since it uses gibbs and thus the middle vector
                        {
                          jday( year, mon, day, hr, min, sec,  satrec.jdsatepoch );
                          obsrecarr[0].dtmin = ((obsrecarr[0].jd +
                                                (obsrecarr[0].hr * 60 + obsrecarr[0].min + obsrecarr[0].sec / 60.0)/1440.0)
                                                - satrec.jdsatepoch) * 1440.0;
                        }
                      // calculate the sensor each time because the obs might be from a different site
                      getsensorparams( currobsrec.sennum, currsenrec );
                      site( currsenrec.senlat, currsenrec.senlon, currsenrec.senalt,
                            currobsrec.rsecef, currobsrec.vsecef );

                      dtmin = ((currobsrec.jd + mfme/1440.0) - satrec.jdsatepoch) * 1440.0;
                      currobsrec.dtmin = dtmin;
                      obsrecarr[ii] = currobsrec;
                      ii = ii + 1;
                      if (fmod(ii,30) == 0 )
                          printf("over 30 obs \n");
                    }   // if strr
                } // while ii through producing the observation vectors

              lastob = ii-1;   // use the number of observations read from the file

lastob = 25;              
              // setup nominal vector -----------------------------------------
              satrec.bstar = 0.0001;  // set a deafult

              // need to get some kind of range estimate for these data types...
              if ( (currobsrec.obstype == 1) || (currobsrec.obstype == 3) )
                {
                   printf("input range estimate in er (km?) \n");
                   scanf( "%lf",&rngest );
                }

              zrng   = 0.0;
              zaz    = 0.0;
              zel    = 0.0;
              zrtasc = 0.0;
              zdecl  = 0.0;
              rsum[0] = 0.0;
              rsum[1] = 0.0;
              rsum[2] = 0.0;
              vsum[0] = 0.0;
              vsum[1] = 0.0;
              vsum[2] = 0.0;
              skipped  = 0;
              keepit = 'y';

              // try averaging the first 10 observations to get a value
              for (ktr = 0; ktr < 10; ktr++)     // 10
                {
                  switch (currobsrec.obstype)
                    {
                      case 1 :
                        {
                          rv_radec( r1, v1, eFrom,
                                    rngest, obsrecarr[ktr].az, obsrecarr[ktr].el, zrng, zaz, zel);
                        }
                        break;
                      case 2 :  // very simplistic iod here just gets 3 position vectors in ecef,
                                // obs are in topocentric (sez) coordinates
                        {
                          rv_razel( r1, v1, obsrecarr[ktr].rsecef,  currsenrec.senlat,  currsenrec.senlon, eFrom,
                                    obsrecarr[ktr].rng, obsrecarr[ktr].az, obsrecarr[ktr].el, zrng, zaz, zel );
                          rv_razel( r2, v2, obsrecarr[ktr+1].rsecef,  currsenrec.senlat,  currsenrec.senlon, eFrom,
                                    obsrecarr[ktr+1].rng, obsrecarr[ktr+1].az, obsrecarr[ktr+1].el, zrng, zaz, zel );
                          rv_razel( r3, v3, obsrecarr[ktr+2].rsecef,  currsenrec.senlat,  currsenrec.senlon, eFrom,
                                    obsrecarr[ktr+2].rng, obsrecarr[ktr+2].az, obsrecarr[ktr+2].el, zrng, zaz, zel );
                        }
                        break;
                      case 3 :
                        {
                          // but the rs vector should be in eci here - see code...
                          rv_tradec( r1, v1, obsrecarr[ktr].rsecef, eFrom,
                                     rngest, obsrecarr[ktr].trtasc, obsrecarr[ktr].tdecl, zrng, zrtasc, zdecl);
                        }
                        break;
                    } // case obstype

                  // perform iod (in earth inertial, use teme as approx eci)
                  jd = obsrecarr[ktr].jd;
                  mfme = obsrecarr[ktr].hr * 60.0 + obsrecarr[ktr].min + obsrecarr[ktr].sec / 60.0;
                  findeopparam ( jd, mfme, interp, eoparr, jdeopstart, dut1, dat, lod, xp, yp, ddpsi, ddeps,
                                 iaudx, dy, icrsx, y, s, deltapsi, deltaeps );
                  convtime ( obsrecarr[ktr].year, obsrecarr[ktr].mon, obsrecarr[ktr].day, obsrecarr[ktr].hr, obsrecarr[ktr].min, obsrecarr[ktr].sec,
                             timezone, dut1, dat, ut1, tut1, jdut1, utc, tai, tt, ttt, jdtt, tcg, tdb, ttdb, jdtdb, tcb );
                  // itrf to teme
                  iau76fk5_itrf_teme( r1, v1, aitrf, eTo, r1teme, v1teme, ateme, ttt, xp, yp, jdut1, lod, trans );

                  jd = obsrecarr[ktr+1].jd;
                  mfme = obsrecarr[ktr+1].hr * 60.0 + obsrecarr[ktr+1].min + obsrecarr[ktr+1].sec / 60.0;
                  findeopparam ( jd, mfme, interp, eoparr, jdeopstart, dut1, dat, lod, xp, yp, ddpsi, ddeps,
                                 iaudx, dy, icrsx, y, s, deltapsi, deltaeps );
                  convtime ( obsrecarr[ktr+1].year, obsrecarr[ktr+1].mon, obsrecarr[ktr+1].day, obsrecarr[ktr+1].hr, obsrecarr[ktr+1].min, obsrecarr[ktr+1].sec,
                             timezone, dut1, dat, ut1, tut1, jdut1, utc, tai, tt, ttt, jdtt, tcg, tdb, ttdb, jdtdb, tcb );
                  // itrf to teme
                  iau76fk5_itrf_teme( r2, v2, aitrf, eTo, r2teme, v2teme, ateme, ttt, xp, yp, jdut1, lod, trans );

                  jd = obsrecarr[ktr+2].jd;
                  mfme = obsrecarr[ktr+2].hr * 60.0 + obsrecarr[ktr+2].min + obsrecarr[ktr+2].sec / 60.0;
                  findeopparam ( jd, mfme, interp, eoparr, jdeopstart, dut1, dat, lod, xp, yp, ddpsi, ddeps,
                                 iaudx, dy, icrsx, y, s, deltapsi, deltaeps );
                  convtime ( obsrecarr[ktr+2].year, obsrecarr[ktr+2].mon, obsrecarr[ktr+2].day, obsrecarr[ktr+2].hr, obsrecarr[ktr+2].min, obsrecarr[ktr+2].sec,
                             timezone, dut1, dat, ut1, tut1, jdut1, utc, tai, tt, ttt, jdtt, tcg, tdb, ttdb, jdtdb, tcb );
                  // itrf to teme
                  iau76fk5_itrf_teme( r3, v3, aitrf, eTo, r3teme, v3teme, ateme, ttt, xp, yp, jdut1, lod, trans );


                  gibbs( r1teme, r2teme, r3teme, v2teme, theta, theta1, copa, error );
fprintf(outfile1, " gibbs  %11.5lf  %11.5lf  %11.5lf %11.7lf %11.7lf %11.7lf \n",r2teme[0], r2teme[1], r2teme[2], v2teme[0], v2teme[1], v2teme[2] );
// most of these angles are just 0.72 deg apart
// the times in herrgibbs need to be in increasing order...
                  herrgibbs( r1teme, r2teme, r3teme, 0.0,
                             obsrecarr[ktr+1].dtmin/1440 - obsrecarr[ktr].dtmin/1440,
                             obsrecarr[ktr+2].dtmin/1440 - obsrecarr[ktr].dtmin/1440,
                             v2teme, theta, theta1, copa, error );   // this is earth fixed
fprintf(outfile1, "hgibbs  %11.5lf  %11.5lf  %11.5lf %11.7lf %11.7lf %11.7lf ",r2[0], r2[1], r2[2], v2[0], v2[1], v2[2] );
fprintf(outfile1, "hgibbs  %11.5lf  %11.5lf  %11.5lf %11.7lf %11.7lf %11.7lf \n",obsrecarr[ktr].dtmin- obsrecarr[ktr].dtmin,
                           obsrecarr[ktr+1].dtmin- obsrecarr[ktr].dtmin,
                           obsrecarr[ktr+2].dtmin- obsrecarr[ktr].dtmin,
                           theta*rad, theta1*rad, copa*rad );

                  // now sum up the vector in teme to get an initial guess
                  // use two-body for simplicity and since short time span for iod
                  // move vectors back to middle vector time which is one ahead
                  kepler ( r2teme, v2teme, -obsrecarr[ktr+1].dtmin*60.0, r1, v1 );  // these will be all in teme

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
fprintf(outfile1, "res  %11.5lf  %11.5lf  %11.5lf %11.7lf %11.7lf %11.7lf %3i %3i \n",rsum[0], rsum[1], rsum[2], vsum[0], vsum[1], vsum[2],ktr,skipped );

//              if (ktr > 1)
//                  ktr = ktr - skipped;

               rteme[0] = rsum[0] / ktr;
               rteme[1] = rsum[1] / ktr;
               rteme[2] = rsum[2] / ktr;
               vteme[0] = vsum[0] / ktr;
               vteme[1] = vsum[1] / ktr;
               vteme[2] = vsum[2] / ktr;

               for (i= 0; i < 6; i++)
                 {
                   if (i < 3)
                       xnom[i] = rteme[i];
                     else
                       xnom[i] = vteme[i-3];
                 }
               if (statesize > 6)
                   xnom[6] = 0.0001; // set initial guess as it changes in state2satrec
                 else
                   satrec.bstar = 0.0001;  
               state2satrec( xnom, scalef, 'v', statesize, eTo, satrec ); // force this method to assign values
               if (obsrecarr[1].year >= 2000)
                   satrec.epochyr = obsrecarr[1].year - 2000;
                 else
                   satrec.epochyr = obsrecarr[1].year - 1900;
               finddays(obsrecarr[1].year, obsrecarr[1].mon, obsrecarr[1].day, obsrecarr[1].hr,
                        obsrecarr[1].min, obsrecarr[1].sec,  satrec.epochdays );
//               satrec.jdsatepoch = satrec.jdsatepoch;  // set earlier

               sgp4init( whichconst, opsmode, satrec.satnum, satrec.jdsatepoch-2433281.5, satrec.bstar,
                         satrec.ecco, satrec.argpo, satrec.inclo, satrec.mo, satrec.no,
                         satrec.nodeo, satrec);

fprintf(outfile1, "rteme  %11.5lf  %11.5lf  %11.5lf %11.7lf %11.7lf %11.7lf \n",rteme[0], rteme[1], rteme[2], vteme[0], vteme[1], vteme[2] );
fprintf(outfile1, "coe  %11.5lf  %11.5lf  %11.5lf %11.7lf %11.7lf %11.7lf %11.7lf \n",satrec.a*6378.135, satrec.ecco, satrec.inclo*rad,
                                 satrec.nodeo*rad, satrec.argpo*rad, satrec.mo*rad,satrec.no );

// nominal vector in teme
//              satrec.a = 7203.20025180/6378.135;  //er
//              satrec.ecco = 0.0015278327;
//              satrec.inclo = 114.954226 / rad;
//              satrec.nodeo = 190.308229 / rad;
//              satrec.argpo = 345.4910919 / rad;
//              satrec.mo = 165.996981 / rad;
// answer??
              satrecx.a = 7214.3395180 / 6378.135;  //er
              satrecx.ecco = 0.0006009327;
              satrecx.inclo = 114.962396 / rad;
              satrecx.nodeo = 190.297730 / rad;
              satrecx.argpo = 123.8855619 / rad;
              satrecx.mo = 123.828381 / rad;
              satrecx.no  = (1.0 / 13.446839) * sqrt( 1.0 / (satrecx.a * satrecx.a * satrecx.a) );  // rad / min

              sgp4init( whichconst, opsmode, satrec.satnum, satrec.jdsatepoch-2433281.5, satrec.bstar,
                         satrec.ecco, satrec.argpo, satrec.inclo, satrec.mo, satrec.no,
                         satrec.nodeo, satrec);
              // call the propagator to get the initial state vector value
              sgp4 (whichconst, satrec,  0.0, rnom,  vnom);

              state2satrec( xnom, scalef, statetype, statesize, eFrom, satrec );
//rnom[0] = 6157.4121;
//rnom[1] = 2455.16193;
//rnom[2] = 2823.89107;
//vnom[0] = 3.667402;
//vnom[1] = -2.204994;
//vnom[2] = -6.0647111;
              printf( "running new satellite \n" );

              // --------- now determine the orbit for the satellite ----------
              leastsquares( percentchg, deltaamtchg, rmsepsilon, whichconst, satrec, satrecx, typeans,
                            statetype, firstob, lastob, statesize, obsrecarr, loops,
                            scalef, xnom, x, dx, atwai, atwa, atwb, outfile, outfile1 );
              sgp4 (whichconst, satrec,  0.0, rnom,  vnom);
              rpos[0] = obsrecarr[0].x;
              rpos[1] = obsrecarr[0].y;
              rpos[2] = obsrecarr[0].z;
              vpos[0] = obsrecarr[0].xdot;
              vpos[1] = obsrecarr[0].ydot;
              vpos[2] = obsrecarr[0].zdot;
              for (j = 0; j < 3; j++)
                {
                  dr[j] = rnom[j] - rpos[j];
                  dv[j] = vnom[j] - vpos[j];
                }
              fprintf(outfile, " difference %11lf  %11lf  %11lf %11lf \n",dr[0], dr[1], dr[2], mag(dr) );
              fprintf(outfile1, " difference %11lf  %11lf  %11lf %11lf \n",dr[0], dr[1], dr[2], mag(dr) );
              printf( " difference %11lf  %11lf  %11lf %11lf \n",dr[0], dr[1], dr[2], mag(dr) );

          fclose(infile);
          fclose(outfile);
          fclose(outfile1);
        }
   }   // processrazel

/*       ---------------------------------------------------------------      */
// this function reads a .e file and processes it into a TLE
// there are options to read a span of data, at given timesteps
void processxyz()
  {
     obsrec obsrecarr[10000];
     elsetrec satrec;
     elsetrec satrecx;
     long search_satno = 0;
     char longstr1[160], longstr2[160];
     char strr[2], monstr[3], strr1, strr2;
     char infilename[30], tmp[20], tmpc, units[10], coords[10];
     char typeinput, typerun;
     gravconsttype whichconst;
     int day, mon, year, hr, min, loops, obsstep, obsread;
     double startmfe, stopmfe, conv, sec, deltamin, dtsec,p, a, ecc, incl, omega,
            argp, nu, m, arglat, truelon, lonper;

     eopdata eoparr[eopsize];
     double jdeopstart, dut1, ut1, tut1, jdut1, utc, tai, tt, ttt, jdtt, tcg, tdb,
            ttdb, jdtdb, tcb, jde, mfme, lod, xp, yp, ddpsi, ddeps,
            iaudx, dy, icrsx, y, s, deltapsi, deltaeps;
     char interp;
     int timezone, dat ;
     std::vector< std::vector<double> > trans(3,3);

     double jdepoch, periodmin, dtseczero, magcov;
     int firstob, ii, j, kk;
     double rpos[3], vpos[3], apos[3], rnom[3], vnom[3], dr[3], dv[3], magdr1, magdr,
            rteme[3], vteme[3], ateme[3], ritrf[3], vitrf[3], aitrf[3], dtmin, jd;
     obsrec currobsrec;

     if (batchmode == 'n') // needs a default value here in batch mode
         statesize = 7;
     // set these variables up with intial dimensions
     // these must be set here
     // function matrices can be resized
     std::vector<double> xnom(statesize);
     std::vector< std::vector<double> > x(statesize,1);
     std::vector< std::vector<double> > scalef(statesize,1);
     std::vector< std::vector<double> > dx(statesize,1);
     std::vector< std::vector<double> > atwai(statesize,statesize);
     std::vector< std::vector<double> > atwa(statesize,statesize);
     std::vector< std::vector<double> > atwb(statesize,1);

     whichconst = wgs72;
     typerun    = 'c';  // c catalog, v verification, m manual

     if (batchmode == 'n')
       {
         statetype  = 't';   // v vectors, t tle vars, e equinoctial
         printf("input type of elements tle or equin \n\n");
         statetype = getchar();
         fflush(stdin);
         printf("input obs filename: \n");
         scanf( "%s",fname );
         percentchg = 0.001;  // percent to change components for finite differencing
         printf("input percentchg for finite differencing (.001) \n");
         scanf( "%lf",&percentchg );
         deltaamtchg = 0.0000001;  // delta amt to change components for finite differencing
         printf("input deltaamtchg to chk in finite differencing (.0000001) \n");
         scanf( "%lf",&deltaamtchg );
         rmsepsilon = 0.0002;  // percent to change components for finite differencing
         printf("input rmsepsilon to quit (.02) \n");
         scanf( "%lf",&rmsepsilon );
         lastob = 72;        // how many obs per rev to consider
         printf("input lastob to fit (72) \n");  // this will be 72 points to fit
         scanf( "%i",&lastob );
         statesize = 72;        // solve for bstar or not
         printf("input statesize (6 or 7) \n");
         scanf( "%i",&statesize );
       }

     obsstep = 1;        // number of steps to skip between reads
     printf("input obsstep (1, ...) \n");
     scanf( "%i",&obsstep );

     obsread = 1;        // total obs to read at the obsstep
     printf("input total obs to read (1000, ...) \n");
     scanf( "%i",&obsread );

     printf( "state type %c \n",statetype );
     printf( "fname %s \n",fname );

     infile = fopen(fname, "r");
     outfile = fopen("testdc.out", "w");
     outfile1 = fopen("testdc1.out", "w");

     printf( "percentchg %lf \n",percentchg );
     printf( "deltaamtchg %lf \n",deltaamtchg );
     printf( "rmsepsilon %lf \n",rmsepsilon );

     firstob    = 0;

     fprintf( outfile, "typeans %c \n",typeans );
     fprintf( outfile, "state type %c \n",statetype );
     fprintf( outfile, "percentchg %lf \n",percentchg );
     fprintf( outfile, "deltaamtchg %11.8lf \n",deltaamtchg );
     fprintf( outfile, "rmsepsilon %lf \n",rmsepsilon );
     fprintf( outfile, "state size %i \n",statesize );
     fprintf( outfile, "obs to fit %i \n",lastob );
     fprintf( outfile, "obs to read %i \n",obsread );
     fprintf( outfile, "obs to skip %i \n",obsstep );

     if(infile == NULL)
       {
       	  printf("Failed to open %s (%s)", infilename, strerror(errno));
       }
       else
       {

                              // initialize coordinate system
                              iau80in( iau80rec );
                              initeop (eoparr, jdeopstart);
//                              printf("input interp n l s \n");
//                              scanf( "%1c",&interp );
                              interp = 'l';
                              printf( "interp type %c \n",interp );


          conv = 0.001;
          strcpy( units,"Meters"); // default
              do // search the header for information
                {
                  fgets( longstr1,161,infile);
                  strr1 = char(longstr1[0]);
                  switch (strr1)
                     {
                     case 'D' :   // search for units
                       {
                          sscanf(longstr1,"%12s %s ", &tmp, &units );
                          if (strcmp(units, "Meters")==0)
                              conv = 0.001;
                            else
                              conv = 1.0;
                       }
                     break;
                     case 'S' :   // search for epoch time postion
                       {
                         sscanf(longstr1,"%14s %i %4s %i %3i %1c %2i %1c %lf ",
                             &tmp, &currobsrec.day, &monstr, &currobsrec.year, &currobsrec.hr, &tmpc,
                             &currobsrec.min, &tmpc, &currobsrec.sec );
                         currobsrec.mon = getmon(monstr);
                         jday( currobsrec.year, currobsrec.mon, currobsrec.day,
                               currobsrec.hr, currobsrec.min, currobsrec.sec, satrec.jdsatepoch);
                       }
                     break;
                     case 'C' :    // search for coordinate system
                       {
                          strr2 = char(longstr1[16]);
                          if ((char(longstr1[2]) == 'o') && (strr2 != 'E') )
                            {
                              sscanf(longstr1,"%16s %s ", &tmp, &coords );
// dav move the eop initialization
       //       initspw (spwarr, jdspwstart);
       //              findatmosparam( jde, mfme, interp, fluxtype, f81type, inputtype, spwarr, jdspwstart,
       //                f107, f107bar, ap, avgap, aparr, kp, sumkp, kparr );
                              sethelp(iauhelp,'n');
                              timezone = 0;
                            }
                       }
                     break;

                     }  // switch
                } while ( (strr1 != 'E') && (feof(infile) == 0) );

              fgets( longstr1,161,infile);  // blank line

              // ---- read the observations (vectors) from the input file ----
              // ---- read more than the points for the fit span to permit prediction analysis if possible
              ii = 0;
              while ( (feof(infile) == 0) && (ii < obsread) )   // read more in for comparison later
                {
                   fgets( longstr2,161,infile);
                   strncpy(strr, &longstr2[0], 1);
                   strr[1] = '\0';
                   if (strcmp(strr, "E") != 0)
                     {
                       sscanf(longstr2,"%lf %lf %lf %lf %lf %lf %lf ",
                              &dtsec, &currobsrec.x, &currobsrec.y, &currobsrec.z,
                              &currobsrec.xdot, &currobsrec.ydot, &currobsrec.zdot );

                           rpos[0] = currobsrec.x*conv;
                           rpos[1] = currobsrec.y*conv;
                           rpos[2] = currobsrec.z*conv;
                           vpos[0] = currobsrec.xdot*conv;
                           vpos[1] = currobsrec.ydot*conv;
                           vpos[2] = currobsrec.zdot*conv;

                           if (ii == 0) // fix the jd epoch in case it starts later than 0.0 sec
                             {
                               dtseczero = dtsec;
                               invjday( satrec.jdsatepoch + dtsec/86400.0, year,mon,day,hr,min,sec );
                               jday( year,mon,day,hr,min,sec, satrec.jdsatepoch );
                             }

                           invjday( satrec.jdsatepoch + (dtsec-dtseczero)/86400.0, year,mon,day,hr,min,sec );
                           currobsrec.year = year;
                           currobsrec.mon = mon;
                           currobsrec.day = day;
                           currobsrec.hr = hr;
                           currobsrec.min = min;
                           currobsrec.sec = sec;
                           jday( year, mon, day, 0, 0, 0.0, currobsrec.jd );
                           mfme = hr * 60 + min + sec/60.0;

                           // convert coordinate system as necessary
                           if ( (strcmp(coords,"J2000") == 0) || (strcmp(coords,"MeanOfEpoch") == 0) )
                             {
                               findeopparam ( currobsrec.jd, mfme, interp, eoparr, jdeopstart, dut1, dat, lod, xp, yp, ddpsi, ddeps,
                                              iaudx, dy, icrsx, y, s, deltapsi, deltaeps );
                               convtime ( year, mon, day, hr, min, sec, timezone, dut1, dat,
                                          ut1, tut1, jdut1, utc, tai, tt, ttt, jdtt, tcg, tdb, ttdb, jdtdb, tcb );
                               iau76fk5_itrf_j2k( ritrf, vitrf, aitrf,eFrom, rpos,vpos,apos,iau80rec, ttt,jdut1,lod,xp,yp,2, trans);
                               iau76fk5_itrf_teme( ritrf, vitrf, aitrf, eTo, rteme, vteme, ateme, ttt, xp, yp, jdut1, lod, trans );
                             }
                             else
                               if (strcmp(coords,"Fixed") == 0)
                                 {
                                   findeopparam ( currobsrec.jd, mfme, interp, eoparr, jdeopstart, dut1, dat, lod, xp, yp, ddpsi, ddeps,
                                                  iaudx, dy, icrsx, y, s, deltapsi, deltaeps );
                                   convtime ( year, mon, day, hr, min, sec, timezone, dut1, dat,
                                              ut1, tut1, jdut1, utc, tai, tt, ttt, jdtt, tcg, tdb, ttdb, jdtdb, tcb );
                                   iau76fk5_itrf_teme( rpos,vpos,apos, eTo, rteme, vteme, ateme, ttt, xp, yp, jdut1, lod, trans );
                                 }
                                 else
                                 {
                                 for(j = 0; j < 3; j++)
                                   {
                                     rteme[j] = rpos[j];
                                     vteme[j] = vpos[j];
                                   }
                                 }

                       // now reset the obs vector to be teme!!
                       currobsrec.x = rteme[0];
                       currobsrec.y = rteme[1];
                       currobsrec.z = rteme[2];
                       currobsrec.xdot = vteme[0];
                       currobsrec.ydot = vteme[1];
                       currobsrec.zdot = vteme[2];

                       if (ii == 0)
                         {
                           rv2coe ( rteme, vteme, p, a, ecc, incl, omega, argp, nu, m, arglat, truelon, lonper);
                           satrec.no = 60.0 * sqrt(3.986008e5/(a * a * a));  // rad per min, sgp4 wgs-72 mu value

                           // set the time span based on the period size,
                           // but have to calculate on second iteration when dtsec is known
                           // so calc here before satrec.a changes on the 1st iteration
                           periodmin = (2.0 * pi * sqrt(pow(a,3) / 398600.8) ) / 60.0;  // minutes
                           satrec.a = a/6378.135;  // ER
                           satrec.ecco = ecc;
                           satrec.inclo = incl;
                           satrec.nodeo = omega;
                           satrec.argpo = argp;
                           satrec.mo = m;
                           if (year >= 2000)
                               satrec.epochyr = year - 2000;
                             else
                               satrec.epochyr = year - 1900;
                           finddays(year, mon, day, hr, min, sec, satrec.epochdays );
                           satrecx = satrec;    // store the answer for later
                         }  // if ii = 0
                       if (ii == 1 )
                         {
//                           lastob = int(lastob / (dtsec / 60.0) * .5 * int( periodmin/ lastob )); // reset this based on the ephemeris file spacing
//lastob = 200; // force this/. 901 is at 43 sec, 906 is at 15 min, ice is at 30 sec, gps is at 15 min
                           printf(" %3i with spacing of %lf min is %lf min \n", lastob, (dtsec-dtseczero)/60.0,lastob*(dtsec-dtseczero)/60.0 );
                         }

                       dtmin = (dtsec-dtseczero) / 60.0;
                       currobsrec.dtmin = dtmin;
                       currobsrec.sennum = 1;
                       currobsrec.obstype = 4;  // xyz obs
                       obsrecarr[ii] = currobsrec;
                       ii = ii + 1;   // increase the obs by one even if obsstep is different
                       if (fmod(ii,500) == 0)
                           printf("over 500 obs \n");
                     }   // if strr
                   // reread any additional steps if needed
                   if (obsstep > 1)
                     {
                       for (kk = 1; kk < obsstep; kk++)
                           if (feof(infile) == 0)
                              fgets( longstr2,161,infile);
                     }

                 } // while ii through producing the observation vectors

 //              lastob = ii;

               // setup nominal vector -----------------------------------------
               satrec.bstar = 0.0001;  // set a deafult
               sgp4init( whichconst, opsmode, satrec.satnum, satrec.jdsatepoch-2433281.5, satrec.bstar,
                         satrec.ecco, satrec.argpo, satrec.inclo, satrec.mo, satrec.no,
                         satrec.nodeo, satrec);
               // call the propagator to get the initial state vector value
               sgp4 (whichconst, satrec,  0.0, rnom,  vnom);

               // print initial difference
               rteme[0] = obsrecarr[0].x;
               rteme[1] = obsrecarr[0].y;
               rteme[2] = obsrecarr[0].z;
               vteme[0] = obsrecarr[0].xdot;
               vteme[1] = obsrecarr[0].ydot;
               vteme[2] = obsrecarr[0].zdot;
               for (j = 0; j < 3; j++)
                 {
                   dr[j] = rnom[j] - rteme[j];
                   dv[j] = vnom[j] - vteme[j];
                 }
               fprintf(outfile," init diff %11lf  %11lf  %11lf %11lf \n",dr[0], dr[1], dr[2], mag(dr) );
               magdr1 = 1000.0 * mag(dr); // initial difference in m

               state2satrec( xnom, scalef, statetype, statesize, eFrom, satrec );

               printf( "running new satellite \n" );

               // --------- now determine the orbit for the satellite ----------
               leastsquares( percentchg, deltaamtchg, rmsepsilon, whichconst, satrec, satrecx, typeans,
                             statetype, firstob, lastob, statesize, obsrecarr, loops,
                             scalef, xnom, x, dx, atwai, atwa, atwb, outfile, outfile1 );
               sgp4 (whichconst, satrec,  0.0, rnom,  vnom);
               rpos[0] = obsrecarr[0].x;
               rpos[1] = obsrecarr[0].y;
               rpos[2] = obsrecarr[0].z;
               vpos[0] = obsrecarr[0].xdot;
               vpos[1] = obsrecarr[0].ydot;
               vpos[2] = obsrecarr[0].zdot;
               for (j = 0; j < 3; j++)
                 {
                   dr[j] = rnom[j] - rpos[j];
                   dv[j] = vnom[j] - vpos[j];
                 }
               magdr = mag(dr) * 1000.0;
//               magcov =  sqrt( xnom[0] / satrec.ecco * atwai[1][0] +
//                              -xnom[1] / (satrec.ecco * satrec.ecco) * atwai[4][0] +
//                               xnom[1] / (satrec.ecco * satrec.ecco) * atwai[5][0]);

               magcov = sqrt( atwai[2][2] ) * 6378135.0;  // in m

               fprintf(outfile," difference %11lf  %11lf  %11lf %11lf %10.2lf \n",dr[0], dr[1], dr[2], mag(dr), magcov );
               fprintf(outfile1," difference %11lf  %11lf  %11lf km %12lf m %10.2lf \n\n", dr[0], dr[1], dr[2], 1000.0 * mag(dr), magcov );
               printf( "%4i %8.1lf %8.4lf m a %10.5lf e %10.7lf i %8.3lf %11.6lf m\n",loops, magdr1, magdr, satrecx.a, satrecx.ecco, satrecx.inclo*57.2957795,
                       magcov );

               // ------------- now take the converged TLE and propagate (and ----------------
               // ---------- convert coordinate system) over ephemeris and compare -----------
               for (j = 0; j < obsread; j++)
                 {
                   sgp4 (whichconst, satrec,  obsrecarr[j].dtmin, rteme,  vteme);
                   dr[0] = 1000.0 * (obsrecarr[j].x - rteme[0]);
                   dr[1] = 1000.0 * (obsrecarr[j].y - rteme[1]);
                   dr[2] = 1000.0 * (obsrecarr[j].z - rteme[2]);
                   dv[0] = 1000.0 * (obsrecarr[j].xdot - vteme[0]);
                   dv[1] = 1000.0 * (obsrecarr[j].ydot - vteme[1]);
                   dv[2] = 1000.0 * (obsrecarr[j].zdot - vteme[2]);
                   fprintf(outfile,"diff %11.3lf  %11.4lf %11.4lf %11.4lf  %11.3lf m \n",obsrecarr[j].dtmin, dr[0], dr[1], dr[2], mag(dr) );
                 }
               fprintf(outfile," init diff %11lf  %11lf  %11lf %11lf \n",dr[0], dr[1], dr[2], mag(dr) );
               magdr1 = 1000.0 * mag(dr); // initial difference in m

          fclose(infile);
          fclose(outfile);
          fclose(outfile1);
        }
   }  // processxyz


/*       ---------------------------------------------------------------      */
void processtle()
   {
     obsrec obsrecarr[10000];
     elsetrec satrec;
     elsetrec satrecx;
     long search_satno = 0;
     char longstr1[130], longstr2[130];
     char str[2];
     char typeinput, typerun;
     gravconsttype whichconst;
     double startmfe, stopmfe, deltamin, magdr;
     double p, a, ecc, incl, omega, argp, nu, m, arglat, truelon, lonper;
     int ll1m, l1m, l10m, l50m, l1km, l10km;

     eopdata eoparr[eopsize];
     double jdeopstart, jdepoch, totalnum, periodmin, avgiter, magcov;
     int firstob, i, ii, j, loops, loopktr, idx;
     double rnom[3], vnom[3], rpos[3], vpos[3], dr[3], dv[3], dtmin, jd, magdr1;
     obsrec currobsrec;
     double rad, apalt, pralt;

     if (batchmode == 'n') // needs a default value here in batch mode
         statesize = 7;
     // set these variables up with intial dimensions
     // these must be set here
     // function matrices can be resized
     std::vector<double> xnom(statesize);
     std::vector< std::vector<double> > x(statesize,1);
     std::vector< std::vector<double> > scalef(statesize,1);
     std::vector< std::vector<double> > dx(statesize,1);
     std::vector< std::vector<double> > atwai(statesize,statesize);
     std::vector< std::vector<double> > atwa(statesize,statesize);
     std::vector< std::vector<double> > atwb(statesize,1);

     rad = 180.0 / pi;
     whichconst = wgs72;
     typerun    = 'c';  // c catalog, v verification, m manual
     if (batchmode == 'n')
       {
         statetype  = 't';   // v vectors, t tle vars, e equinoctial
         printf("input type of elements tle or equin \n\n");
         statetype = getchar();
         fflush(stdin);
         printf("input TLE filename: \n");
         scanf( "%s",fname );
         percentchg = 0.001;  // percent to change components for finite differencing
         printf("input percentchg for finite differencing (.001) \n");
         scanf( "%lf",&percentchg );
         deltaamtchg = 0.0000001;  // delta amt to change components for finite differencing
         printf("input deltaamtchg to chk in finite differencing (.0000001) \n");
         scanf( "%lf",&deltaamtchg );
         rmsepsilon = 0.0002;  // percent to change components for finite differencing
         printf("input rmsepsilon to quit (.02) \n");
         scanf( "%lf",&rmsepsilon );
         lastob = 72;        // how many obs per rev to consider
         printf("input lastob to quit (72) \n");  // this will be 72 points per rev, but calculate later for 2 periods
         scanf( "%i",&lastob );
         statesize = 7;        // solve for bstar or not
         printf("input statesize (6 or 7) \n");
         scanf( "%i",&statesize );
       }
     printf( "fname %s \n",fname );

     infile = fopen(fname, "rb");
     outfile = fopen("testdc.out", "w");
     outfile1 = fopen("testdc1.out", "w");
     outfile2 = fopen("testdc2.out", "w");

     firstob    = 0;

     fprintf( outfile, "typeans %c \n",typeans );
     fprintf( outfile, "state type %c \n",statetype );
     fprintf( outfile, "percentchg %lf \n",percentchg );
     fprintf( outfile, "deltaamtchg %11.8lf \n",deltaamtchg );
     fprintf( outfile, "rmsepsilon %lf \n",rmsepsilon );
     fprintf( outfile, "state size %i \n",statesize );
     fprintf( outfile, "pts per period %i \n",lastob );
     fprintf( outfile2, "typeans %c \n",typeans );
     fprintf( outfile2, "state type %c \n",statetype );
     fprintf( outfile2, "percentchg %lf \n",percentchg );
     fprintf( outfile2, "deltaamtchg %11.8lf \n",deltaamtchg );
     fprintf( outfile2, "rmsepsilon %lf \n",rmsepsilon );
     fprintf( outfile2, "state size %i \n",statesize );
     fprintf( outfile2, "pts per period %i \n",lastob );
     fprintf( outfile2," iter   norad      inti diff (m)  final diff    a           e          i     ap alt   pr alt\n");

     if(infile == NULL)
     	{
       	  printf("Failed to open %s (%s)", fname, strerror(errno));
	}
	else
	{
                              // initialize coordinate system
                              iau80in( iau80rec );
                              initeop (eoparr, jdeopstart);


          // --- setup counters for bins of entire catalog
          ll1m  = 0;
          l1m   = 0;
          l10m  = 0;
          l50m  = 0;
          l1km  = 0;
          l10km = 0;
          avgiter = 0;

          while (feof(infile) == 0)
            {
              do
                {
                  fgets( longstr1,130,infile);
                  strncpy(str, &longstr1[0], 1);
                  str[1] = '\0';
                } while ((strcmp(str, "#")==0)&&(feof(infile) == 0));

              // ---- setup the sgp4 vectors from the input file ----
              if (feof(infile) == 0)
                {
                  fgets( longstr2,130,infile);
                  // convert the char string to sgp4 elements
                  // includes initialization of sgp4
                  twoline2rv( longstr1, longstr2, typerun, typeinput, opsmode, whichconst,
                              startmfe, stopmfe, deltamin, satrec );
                  fprintf(outfile, "%ld xx \n", satrec.satnum);
                  printf(" %5ld ", satrec.satnum);
                } // if going through input file

               // set the time span based on the period size
               periodmin = 2.0 * pi * sqrt(pow(satrec.a,3) / 398600.8);
               dtmin = int( (periodmin )/ lastob );

               // ---- loop to create ephemeris points (observations) ----------
               // ----------------- and setup obsrecarr ------------------------
               for(ii=0; ii <= lastob; ii++)
                 {
                   currobsrec.dtmin = dtmin;
                   sgp4 (whichconst, satrec,  dtmin, rpos, vpos);
                   jd = satrec.jdsatepoch + dtmin/1440.0;
                   invjday( jd, currobsrec.year, currobsrec.mon, currobsrec.day,
                            currobsrec.hr, currobsrec.min, currobsrec.sec);
                   jday( currobsrec.year, currobsrec.mon, currobsrec.day, 0, 0, 0.0, currobsrec.jd );
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
               satrecx = satrec;    // store the answer for later
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
               sgp4 (whichconst, satrecx,  0.0, rnom,  vnom);
               for (i= 0; i < 6; i++)
                 {
                   if (i < 3)
                       xnom[i] = rnom[i];
                     else
                       xnom[i] = vnom[i-3];
                 }
               if (statesize > 6)
                   xnom[6] = 0.0001; // set initial guess as it changes in state2satrec
                 else
                   satrec.bstar = 0.0001;  
               state2satrec( xnom, scalef, 'v', statesize, eTo, satrec ); // force this method to assign values

               satrec.jdsatepoch = satrecx.jdsatepoch;  // be sure to set the epoch!
               sgp4init( whichconst, opsmode, satrec.satnum, satrec.jdsatepoch-2433281.5, satrec.bstar,
                         satrec.ecco, satrec.argpo, satrec.inclo, satrec.mo, satrec.no,
                         satrec.nodeo, satrec);
               // call the propagator to get the initial state vector value
               sgp4 (whichconst, satrec,  0.0, rnom,  vnom);

               // print initial difference -----------------------
               rpos[0] = obsrecarr[0].x;
               rpos[1] = obsrecarr[0].y;
               rpos[2] = obsrecarr[0].z;
               vpos[0] = obsrecarr[0].xdot;
               vpos[1] = obsrecarr[0].ydot;
               vpos[2] = obsrecarr[0].zdot;
               for (j = 0; j < 3; j++)
                 {
                   dr[j] = rnom[j] - rpos[j];
                   dv[j] = vnom[j] - vpos[j];
                 }
               fprintf(outfile," init diff %11lf  %11lf  %11lf %11lf \n",dr[0], dr[1], dr[2], mag(dr) );
               magdr1 = 1000.0 * mag(dr); // initial difference in m
//               printf( " init diff %11lf  %11lf  %11lf %11lf \n",dr[0], dr[1], dr[2], mag(dr) );

//               if (statetype == 'v')
//                 {
//                   // set some rnom vnom value...

//                   rv2coe ( rnom, vnom, p, a, ecc, incl, omega, argp, nu, m, arglat, truelon, lonper);
//                   satrec.no = 60.0 * sqrt(3.986008e5/(a * a * a));  // rad per min, sgp4 wgs-72 mu value
//                   satrec.a = a/6378.135;  // er
//                   satrec.ecco = ecc;
//                   satrec.inclo = incl;
//                   satrec.nodeo = omega;
//                   satrec.argpo = argp;
//                   satrec.mo = m;
//                 }

               state2satrec( xnom, scalef, statetype, statesize, eFrom, satrec );

//               printf( "running new satellite \n" );

               // --------- now determine the orbit for the satellite ----------
               leastsquares( percentchg, deltaamtchg, rmsepsilon, whichconst, satrec, satrecx, typeans,
                             statetype, firstob, lastob, statesize, obsrecarr, loops,
                             scalef, xnom, x, dx, atwai, atwa, atwb, outfile, outfile1 );
               sgp4 (whichconst, satrec,  0.0, rnom,  vnom);  // the final answer from LS
               rpos[0] = obsrecarr[0].x;
               rpos[1] = obsrecarr[0].y;
               rpos[2] = obsrecarr[0].z;
               vpos[0] = obsrecarr[0].xdot;
               vpos[1] = obsrecarr[0].ydot;
               vpos[2] = obsrecarr[0].zdot;
               for (j = 0; j < 3; j++)
                 {
                   dr[j] = rnom[j] - rpos[j];
                   dv[j] = vnom[j] - vpos[j];
                 }
               magdr = mag(dr) * 1000.0;
               fprintf(outfile," difference %11lf  %11lf  %11lf %11lf \n",dr[0], dr[1], dr[2], mag(dr) );
               fprintf(outfile1," %8d  difference %11lf  %11lf  %11lf km %12lf m \n\n",satrec.satnum, dr[0], dr[1], dr[2], 1000.0 * mag(dr) );
               apalt = 6378.135 * satrecx.a * (1.0 + satrecx.ecco) - 6378.135;
               pralt = 6378.135 * satrecx.a * (1.0 - satrecx.ecco) - 6378.135;
//               magcov =  1000.0 * sqrt(atwai[0][0] + atwai[1][1] + atwai[2][2]);
               magcov = sqrt( atwai[2][2] ) * 6378135.0;  // in m

               if (1000.0 * mag(dr) < 0.0001)
                   fprintf(outfile2," %4i %8d %12.0lf %14.6lf %10.2lf %12lf %8.4lf %7.0lf %7.0lf %10.2lf\n",
                           loops, satrec.satnum, magdr1, 0.000001, satrecx.a*6378, satrecx.ecco, satrecx.inclo*rad, apalt, pralt, magcov );
                 else
                   fprintf(outfile2," %4i %8d %12.0lf %14.6lf %10.2lf %12lf %8.4lf %7.0lf %7.0lf %10.2lf\n",
                           loops, satrec.satnum, magdr1, 1000.0 * mag(dr), satrecx.a*6378, satrecx.ecco, satrecx.inclo*rad, apalt, pralt, magcov );

               printf( "%4i %8.1lf %8.4lf m a %10.5lf e %9.6lf i %7.3lf %9.4lf m\n",loops, magdr1, 1000.0 * mag(dr), satrecx.a, satrecx.ecco, satrecx.inclo*rad,
                       magcov );


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

          totalnum =  ll1m + l1m + l10m + l50m + l1km + l10km;
          printf( "%5i %8.2lf      < 1m  avgiter = %8.2lf \n",ll1m, 100.0 * (ll1m / totalnum), avgiter / totalnum );
          printf( "%5i %8.2lf   1m < 10m \n",l1m, 100.0 * ((ll1m + l1m) / totalnum) );
          printf( "%5i %8.2lf  10m < 100m \n",l10m, 100.0 * ((ll1m + l1m + l10m) / totalnum) );
          printf( "%5i %8.2lf 100m < 1km \n",l50m, 100.0 * ((ll1m + l1m + l10m + l50m)/ totalnum) );
          printf( "%5i %8.2lf  1km < 10km \n",l1km, 100.0 * ((ll1m + l1m + l10m + l50m + l1km)/ totalnum) );
          printf( "%5i %8.2lf 10km > \n",l10km, 100.0 * ((ll1m + l1m + l10m + l50m + l1km + l10km)/ totalnum) );

          fprintf( outfile,"%5i %8.2lf      < 1m  avgiter = %8.2lf \n",ll1m, 100.0 * (ll1m / totalnum), avgiter / totalnum );
          fprintf( outfile,"%5i %8.2lf      < 1m \n",ll1m, 100.0 * (ll1m / totalnum) );
          fprintf( outfile,"%5i %8.2lf   1m < 10m \n",l1m, 100.0 * ((ll1m + l1m) / totalnum) );
          fprintf( outfile,"%5i %8.2lf  10m < 100m \n",l10m, 100.0 * ((ll1m + l1m + l10m) / totalnum) );
          fprintf( outfile,"%5i %8.2lf 100m < 1km \n",l50m, 100.0 * ((ll1m + l1m + l10m + l50m)/ totalnum) );
          fprintf( outfile,"%5i %8.2lf  1km < 10km \n",l1km, 100.0 * ((ll1m + l1m + l10m + l50m + l1km)/ totalnum) );
          fprintf( outfile,"%5i %8.2lf 10km > \n",l10km, 100.0 * ((ll1m + l1m + l10m + l50m + l1km + l10km)/ totalnum) );

          fclose(infile);
          fclose(outfile);
          fclose(outfile1);
        }

   }  // processtle


