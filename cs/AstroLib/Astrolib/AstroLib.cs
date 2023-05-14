﻿/*     -------------------------------------------------------------------------
*
*                                AstroLib.cs
*
* this library contains various astrodynamic routines.
*
*                            companion code for
*               fundamentals of astrodynamics and applications
*                                    2013
*                              by david vallado
*
*               email dvallado@comspoc.com, davallado@gmail.com
*
*    current :
*              17 jun 20  david vallado
*                           misc fixes
*    changes :
*              15 jan 19  david vallado
*                           combine with astiod etc
*              11 jan 18  david vallado
*                           misc cleanup
*              30 sep 15  david vallado
*                           fix jd, jdfrac
*              19 mar 14  david vallado
*                           original baseline
*       ----------------------------------------------------------------      */

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using MathTimeMethods;  // Edirection, globals
using EOPSPWMethods;    // EOPDataClass, SPWDataClass, iau80Class, iau00Class


namespace AstroLibMethods
{
    public class AstroLib
    {
        public string AstroLibVersion = "AstroLib Version 2021-06-03";

        // setup the class so methods can be called
        public MathTimeLib MathTimeLibr = new MathTimeLib();

        public string EOPFileLoc;
        public string SPWFileLoc;
        public EOPSPWLib EOPSPWLibr = new EOPSPWLib();


        // ------------------------ reduction options -------------------------
        public enum EOpt
        {
            e80,     // FK5/IAU76
            e96,     // 1996
            e00a,    // iau2000a approx, not used
            e00b,    // iau2000b approx, not used
            e06cio,  // iau2006/2000 cio full series
            e06cioint,  // iau2006/2000 cio full series interpolated
            e06eq    // iau2006/2000 cio classical
        };

        // ------------------- class for gravity and astro constants -------------------
        // all of these can be overwritten, but are all connected to a gravity model
        public class gravityConst
        {
            public string name;
            public double[,] c = new double[2200, 2200];
            public double[,] s = new double[2200, 2200];
            public double[,] cNor = new double[2200, 2200];
            public double[,] sNor = new double[2200, 2200];
            public double[,] cSig = new double[2200, 2200];
            public double[,] sSig = new double[2200, 2200];
            public double mum = 3.986004415e14; // m^3/s^2 stk uses .4415
            public double mu = 398600.4415;     // km^3/s^2 stk uses .4415
            public double re = 6378.1363;       // km  stk uses .1363
            public double rem = 6378136.3;      // default equatorial radius[m], set with each model
            public int maxsize = 2160;         // default max size gravity model data
            public double velkmps = 7.905366149846074;
            public double earthrot = 7.292115e-05;  // 7.29211514670698e-05 older rad/s        
        };
        public gravityConst gravConst = new gravityConst();


        // ------------------- class for astro constants -------------------
        public class astroConst
        {
            public double speedoflight = 2.99792458e8;  // speed of light m/s
            public double au = 149597870.7;  // km
        };
        public astroConst astroConsts = new astroConst();


        // --------------------- XYS series coefficients --------------------
        public const long xyssize = 60000;
        public Int32 xysdesize;
        public class xysdataClass
        {
            public double x;
            public double y;
            public double s;
            public double jdtt, jdftt, mjd;
        };
        // use a class for all the xys data so it can be processed quickly
        public xysdataClass[] xysarr = new xysdataClass[xyssize];


        // --------------------- jpl planetary ephemerides --------------------
        public const long jplsize = 120000;
        public Int32 jpldesize;
        public class jpldedataClass
        {
            public double[] rsun = new double[3];
            public double[] rmoon = new double[3];
            public Int32 year, mon, day, hr;
            public double rsmag, rmmag, mjd;
        };
        // use a class for all the jpl data so it can be processed
        public jpldedataClass[] jpldearr = new jpldedataClass[jplsize];

        public char printopt = 'n';

        // -----------------------------------------------------------------------------------------
        //                      coordinate transformation functions
        // -----------------------------------------------------------------------------------------

        /* -----------------------------------------------------------------------------
        *
        *                           function gstime00
        *
        *  this function finds the greenwich sidereal time (iau-2006/2000).
        *
        *  author        : david vallado           davallado@gmail.com    1 mar 2001
        *
        *  revisions
        *    vallado     - conversion to c#                              16 Nov 2011
        *    
        *  inputs          description                    range / units
        *    jdut1       - julian date in ut1             days from 4713 bc
        *
        *  outputs       :
        *    gstime      - greenwich sidereal time        0 to 2pi rad
        *
        *  locals        :
        *    temp        - temporary variable for doubles   rad
        *    tut1        - julian centuries from the
        *                  jan 1, 2000 12 h epoch (ut1)
        *
        *  coupling      :
        *    none
        *
        *  references    :
        *    vallado       2013, 188, eq 3-47
        * --------------------------------------------------------------------------- */

        public double[,] gstime00
            (
            double jdut1, double deltapsi, double ttt, EOPSPWLib.iau00Class iau00arr, EOpt opt, double[] fArgs,
            out double gst
            )
        {
            double[,] st = new double[3, 3];
            const double twopi = 2.0 * Math.PI;
            const double deg2rad = Math.PI / 180.0;
            double convrt, ttt2, ttt3, ttt4, ttt5, epsa, tempval, gstsum0, gstsum1;
            double eect2000, ee2000, tut1d, era, gmst2000;
            Int32 i, j;

            // " to rad
            convrt = Math.PI / (180.0 * 3600.0);

            ttt2 = ttt * ttt;
            ttt3 = ttt2 * ttt;
            ttt4 = ttt2 * ttt2;
            ttt5 = ttt3 * ttt2;

            // mean obliquity of the ecliptic
            // see sofa code obl06.f (no iau_ in front)
            epsa = 84381.406 - 46.836769 * ttt - 0.0001831 * ttt2 + 0.00200340 * ttt3 - 0.000000576 * ttt4
                - 0.0000000434 * ttt5; // "
            epsa = (epsa / 3600.0 % 360.0);  // deg
            epsa = epsa * deg2rad; // rad

            //  evaluate the ee complementary terms
            gstsum0 = 0.0;
            // data file is not reversed
            for (i = 32; i >= 0; i--)
            {
                tempval = iau00arr.ag0i[i, 0] * fArgs[0] + iau00arr.ag0i[i, 1] * fArgs[1] + iau00arr.ag0i[i, 2] * fArgs[2]
                    + iau00arr.ag0i[i, 3] * fArgs[3] + iau00arr.ag0i[i, 4] * fArgs[4] + iau00arr.ag0i[i, 5] * fArgs[5]
                    + iau00arr.ag0i[i, 6] * fArgs[6] + iau00arr.ag0i[i, 7] * fArgs[7] + iau00arr.ag0i[i, 8] * fArgs[8]
                    + iau00arr.ag0i[i, 9] * fArgs[9] + iau00arr.ag0i[i, 10] * fArgs[10] + iau00arr.ag0i[i, 11] * fArgs[11]
                    + iau00arr.ag0i[i, 12] * fArgs[12] + iau00arr.ag0i[i, 13] * fArgs[13];
                gstsum0 = gstsum0 + iau00arr.ag0[i, 0] * Math.Sin(tempval) + iau00arr.ag0[i, 1] * Math.Cos(tempval);
            }

            gstsum1 = 0.0;
            // data file is not reversed
            for (j = 0; j >= 0; j--)
            {
                i = 32 + j;
                tempval = iau00arr.ag0i[i, 0] * fArgs[0] + iau00arr.ag0i[i, 1] * fArgs[1] + iau00arr.ag0i[i, 2] * fArgs[2]
                    + iau00arr.ag0i[i, 3] * fArgs[3] + iau00arr.ag0i[i, 4] * fArgs[4] + iau00arr.ag0i[i, 5] * fArgs[5]
                    + iau00arr.ag0i[i, 6] * fArgs[6] + iau00arr.ag0i[i, 7] * fArgs[7] + iau00arr.ag0i[i, 8] * fArgs[8]
                    + iau00arr.ag0i[i, 9] * fArgs[9] + iau00arr.ag0i[i, 10] * fArgs[10] + iau00arr.ag0i[i, 11] * fArgs[11]
                    + iau00arr.ag0i[i, 12] * fArgs[12] + iau00arr.ag0i[i, 13] * fArgs[13];
                gstsum1 = gstsum1 + (iau00arr.ag0[i, 0] * Math.Sin(tempval) + iau00arr.ag0[i, 1] * Math.Cos(tempval)) * ttt;
            }

            // equation of the equinoxes
            eect2000 = gstsum0 + gstsum1 * ttt;  // rad
            ee2000 = deltapsi * Math.Cos(epsa) + eect2000;  // rad

            //  earth rotation angle
            tut1d = jdut1 - 2451545.0;
            era = twopi * (0.7790572732640 + 1.00273781191135448 * tut1d);
            era = (era % (2.0 * Math.PI));

            //  greenwich mean sidereal time, iau 2000.
            gmst2000 = era + (0.014506 + 4612.156534 * ttt + 1.3915817 * ttt2
                   - 0.00000044 * ttt3 + 0.000029956 * ttt4 + 0.0000000368 * ttt5) * convrt; // " to rad

            gst = gmst2000 + ee2000; // rad

            st[0, 0] = Math.Cos(gst);
            st[0, 1] = -Math.Sin(gst);
            st[0, 2] = 0.0;
            st[1, 0] = Math.Sin(gst);
            st[1, 1] = Math.Cos(gst);
            st[1, 2] = 0.0;
            st[2, 0] = 0.0;
            st[2, 1] = 0.0;
            st[2, 2] = 1.0;

            return st;
        }  // gstime00 


        /* -----------------------------------------------------------------------------
          *
          *                           function gstime
          *
          *  this function finds the greenwich sidereal time (iau-82).
          *
          *  author        : david vallado           davallado@gmail.com    1 mar 2001
          *
          *  revisions
          *    vallado     - conversion to c#                              16 Nov 2011
          *    
          *  inputs          description                    range / units
          *    jdut1       - julian date in ut1             days from 4713 bc
          *
          *  outputs       :
          *    gstime      - greenwich sidereal time        0 to 2pi rad
          *
          *  locals        :
          *    temp        - temporary variable for doubles   rad
          *    tut1        - julian centuries from the
          *                  jan 1, 2000 12 h epoch (ut1)
          *
          *  coupling      :
          *    none
          *
          *  references    :
          *    vallado       2013, 188, eq 3-47
          * --------------------------------------------------------------------------- */

        public double gstime
            (
            double jdut1
            )
        {
            const double twopi = 2.0 * Math.PI;
            const double deg2rad = Math.PI / 180.0;
            double temp, tut1;

            tut1 = (jdut1 - 2451545.0) / 36525.0;
            temp = -6.2e-6 * tut1 * tut1 * tut1 + 0.093104 * tut1 * tut1
                + (876600.0 * 3600 + 8640184.812866) * tut1 + 67310.54841;  // sec
            temp = (temp * deg2rad / 240.0 % twopi); //360/86400 = 1/240, to deg, to rad

            // ------------------------ check quadrants ---------------------
            if (temp < 0.0)
                temp += twopi;

            return temp;
        }  // gstime 



        /* -----------------------------------------------------------------------------
	    *                           procedure lstime
	    *
	    *  this procedure finds the local sidereal time at a given location.
	    *
	    *  author        : david vallado           davallado@gmail.com    1 mar 2001
	    *
	    *  inputs          description                    range / units
	    *    lon         - site longitude (west -)        -2pi to 2pi rad
	    *    jdut1       - julian date in ut1             days from 4713 bc
	    *
	    *  outputs       :
	    *    lst         - local sidereal time            0.0 to 2pi rad
	    *    gst         - greenwich sidereal time        0.0 to 2pi rad
	    *
	    *  locals        :
	    *    none.
	    *
	    *  coupling      :
	    *    gstime        finds the greenwich sidereal time
	    *
	    *  references    :
        *    vallado       2013, 188, eq 3-47, Alg 15
	    * --------------------------------------------------------------------------- */

        public void lstime
            (
            double lon, double jdut1, out double lst, out double gst
            )
        {
            const double twopi = 2.0 * Math.PI;

            gst = gstime(jdut1);
            lst = lon + gst;

            /* ------------------------ check quadrants --------------------- */
            lst = (lst % twopi);
            if (lst < 0.0)
                lst = lst + twopi;
        }  // lstime


        /* -----------------------------------------------------------------------------
         *
         *                           function fundarg
         *
         *  this function calculates the delauany variables and planetary values for
         *  several theories.
         *
         *  author        : david vallado           davallado@gmail.com   16 jul 2004
         *
         *  revisions
         *    vallado     - conversion to c++                             23 nov 2005
         *    vallado     - conversion to c#                              16 Nov 2011
         *   
         *  inputs          description                                  range / units
         *    ttt         - julian centuries of tt   
         *    opt         - method option                                e00cio, e00a, e96, e80
         *
         *  outputs       :
         *    fArgs       - fundamental argumaents in an array                rad
         *
         *  locals        :
         *    l           - mean anomaly of the moon                          rad
         *    l1          - mean anomaly of the Sun                           rad
         *    f           - mean longitude of the Moon minus that of asc node rad
         *    d           - mean elongation of the Moon from the Sun          rad
         *    omega       - mean longitude of the ascending node of the Moon  rad
         *    planetary longitudes                                            rad
         *
         *  coupling      :
         *    none        -
         *
         *  references    :
         *    vallado       2013, 210-211, 225
         * --------------------------------------------------------------------------- */

        public void fundarg
            (
            double ttt, EOpt opt, out double[] fArgs
            )
        {
            fArgs = new double[14];
            double l, l1, f, d, omega, lonmer, lonven, lonear, lonmar,
                   lonjup, lonsat, lonurn, lonnep, precrate;
            double deg2rad, oo3600, twopi;
            twopi = 2.0 * Math.PI;
            deg2rad = Math.PI / 180.0;
            // arcsec to deg
            oo3600 = 1.0 / 3600.0;

            l = l1 = f = d = omega = lonmer = lonven = lonear = lonmar = lonjup = lonsat = lonurn
                = lonnep = precrate = 0.0;

            // ---- determine coefficients for various iers nutation theories ----
            // ----  iau-2006/2000 CIO and nutation theory and iau-2000a theory
            if (opt.Equals(EOpt.e06cio) || opt.Equals(EOpt.e06eq) || opt.Equals(EOpt.e00a))
            {
                // ------ form the delaunay fundamental arguments in ", converted to deg
                l = ((((-0.00024470 * ttt + 0.051635) * ttt + 31.8792) * ttt + 1717915923.2178) * ttt
                    + 485868.249036) * oo3600;
                l1 = ((((-0.00001149 * ttt + 0.000136) * ttt - 0.5532) * ttt + 129596581.0481) * ttt
                    + 1287104.793048) * oo3600;
                f = ((((+0.00000417 * ttt - 0.001037) * ttt - 12.7512) * ttt + 1739527262.8478) * ttt
                    + 335779.526232) * oo3600;
                d = ((((-0.00003169 * ttt + 0.006593) * ttt - 6.3706) * ttt + 1602961601.2090) * ttt
                    + 1072260.703692) * oo3600;
                omega = ((((-0.00005939 * ttt + 0.007702) * ttt + 7.4722) * ttt - 6962890.5431) * ttt
                    + 450160.398036) * oo3600;

                // ------ form the planetary arguments in ", converted to deg
                //lonmer = (908103.259872 + 538101628.688982 * ttt) * oo3600;
                //lonven = (655127.283060 + 210664136.433548 * ttt) * oo3600;
                //lonear = (361679.244588 + 129597742.283429 * ttt) * oo3600;
                //lonmar = (1279558.798488 + 68905077.493988 * ttt) * oo3600;
                //lonjup = (123665.467464 + 10925660.377991 * ttt) * oo3600;
                //lonsat = (180278.799480 + 4399609.855732 * ttt) * oo3600;
                //lonurn = (1130598.018396 + 1542481.193933 * ttt) * oo3600;
                //lonnep = (1095655.195728 + 786550.320744 * ttt) * oo3600;
                //precrate = ((1.112022 * ttt + 5028.8200) * ttt) * oo3600;
                // these are close (all in rad) - usually 1e-10, but some are as 
                // high as 1e-06
                // these are from TN-36
                lonmer = ((4.402608842 + 2608.7903141574 * ttt) % twopi);
                lonven = ((3.176146697 + 1021.3285546211 * ttt) % twopi);
                lonear = ((1.753470314 + 628.3075849991 * ttt) % twopi);
                lonmar = ((6.203480913 + 334.0612426700 * ttt) % twopi);
                lonjup = ((0.599546497 + 52.9690962641 * ttt) % twopi);
                lonsat = ((0.874016757 + 21.3299104960 * ttt) % twopi);
                lonurn = ((5.481293872 + 7.4781598567 * ttt) % twopi);
                lonnep = ((5.311886287 + 3.8133035638 * ttt) % twopi);
                precrate = ((0.024381750 + 0.00000538691 * ttt) * ttt);
            }

            // ---- iau-1980 theory
            if (opt.Equals(EOpt.e80))
            {
                // ------ form the delaunay fundamental arguments in deg
                l = ((((0.064) * ttt + 31.310) * ttt + 1717915922.6330) * ttt) * oo3600
                    + 134.96298139;
                l1 = ((((-0.012) * ttt - 0.577) * ttt + 129596581.2240) * ttt) * oo3600
                    + 357.52772333;
                f = ((((0.011) * ttt - 13.257) * ttt + 1739527263.1370) * ttt) * oo3600
                    + 93.27191028;
                d = ((((0.019) * ttt - 6.891) * ttt + 1602961601.3280) * ttt) * oo3600
                    + 297.85036306;
                omega = ((((0.008) * ttt + 7.455) * ttt - 6962890.5390) * ttt) * oo3600
                    + 125.04452222;
                // ------ form the planetary arguments in deg
                // iers tn13 shows no planetary
                // seidelmann shows these equations
                // circ 163 shows no planetary  ???????
                lonmer = 252.3 + 149472.0 * ttt;
                lonven = 179.9 + 58517.8 * ttt;
                lonear = 98.4 + 35999.4 * ttt;
                lonmar = 353.3 + 19140.3 * ttt;
                lonjup = 32.3 + 3034.9 * ttt;
                lonsat = 48.0 + 1222.1 * ttt;
                lonurn = 0.0;
                lonnep = 0.0;
                precrate = 0.0;
            }

            // ---- iau-2000b theory
            if (opt.Equals(EOpt.e00b))
            {
                // ------ form the delaunay fundamental arguments in deg
                l = (1717915923.2178 * ttt + 485868.249036) * oo3600;
                l1 = (129596581.0481 * ttt + 1287104.79305) * oo3600;
                f = (1739527262.8478 * ttt + 335779.526232) * oo3600;
                d = (1602961601.2090 * ttt + 1072260.70369) * oo3600;
                omega = (-6962890.5431 * ttt + 450160.398036) * oo3600;

                // ------ form the planetary arguments in deg
                lonmer = 0.0;
                lonven = 0.0;
                lonear = 0.0;
                lonmar = 0.0;
                lonjup = 0.0;
                lonsat = 0.0;
                lonurn = 0.0;
                lonnep = 0.0;
                precrate = 0.0;
                // instead uses a constant rate
                // dplan = -0.135 * oo3600 * deg2rad;
                // deplan = 0.388 * oo3600 * deg2rad;
            }

            // ---- iau-1996 theory
            if (opt.Equals(EOpt.e96))
            {
                // ------ form the delaunay fundamental arguments in deg
                l = ((((-0.00024470 * ttt + 0.051635) * ttt + 31.8792) * ttt + 1717915923.2178) * ttt) * oo3600 + 134.96340251;
                l1 = ((((-0.00001149 * ttt - 0.000136) * ttt - 0.5532) * ttt + 129596581.0481) * ttt) * oo3600 + 357.52910918;
                f = ((((+0.00000417 * ttt + 0.001037) * ttt - 12.7512) * ttt + 1739527262.8478) * ttt) * oo3600 + 93.27209062;
                d = ((((-0.00003169 * ttt + 0.006593) * ttt - 6.3706) * ttt + 1602961601.2090) * ttt) * oo3600 + 297.85019547;
                omega = ((((-0.00005939 * ttt + 0.007702) * ttt + 7.4722) * ttt - 6962890.2665) * ttt) * oo3600 + 125.04455501;
                // ------ form the planetary arguments in deg
                lonmer = 0.0;
                lonven = 181.979800853 + 58517.8156748 * ttt;
                lonear = 100.466448494 + 35999.3728521 * ttt;
                lonmar = 355.433274605 + 19140.299314 * ttt;
                lonjup = 34.351483900 + 3034.90567464 * ttt;
                lonsat = 50.0774713998 + 1222.11379404 * ttt;
                lonurn = 0.0;
                lonnep = 0.0;
                precrate = (0.0003086 * ttt + 1.39697137214) * ttt;
            }

            // ---- convert units from deg to rad 
            l = (l % 360.0) * deg2rad;
            l1 = (l1 % 360.0) * deg2rad;
            f = (f % 360.0) * deg2rad;
            d = (d % 360.0) * deg2rad;
            omega = (omega % 360.0) * deg2rad;

            // convert all but the cio etc values which are already in rad
            if (!opt.Equals(EOpt.e06cio) && !opt.Equals(EOpt.e06eq) && !opt.Equals(EOpt.e00a))
            {
                lonmer = (lonmer % 360.0) * deg2rad;
                lonven = (lonven % 360.0) * deg2rad;
                lonear = (lonear % 360.0) * deg2rad;
                lonmar = (lonmar % 360.0) * deg2rad;
                lonjup = (lonjup % 360.0) * deg2rad;
                lonsat = (lonsat % 360.0) * deg2rad;
                lonurn = (lonurn % 360.0) * deg2rad;
                lonnep = (lonnep % 360.0) * deg2rad;
                precrate = (precrate % 360.0) * deg2rad;
            }

            fArgs[0] = l;   // delaunay variables
            fArgs[1] = l1;
            fArgs[2] = f;
            fArgs[3] = d;
            fArgs[4] = omega;
            fArgs[5] = lonmer;  // begin planetary longitudes
            fArgs[6] = lonven;
            fArgs[7] = lonear;
            fArgs[8] = lonmar;
            fArgs[9] = lonjup;
            fArgs[10] = lonsat;
            fArgs[11] = lonurn;
            fArgs[12] = lonnep;
            fArgs[13] = precrate;

            if (printopt == 'y')
            {
                Console.WriteLine("fundamental parameters");
                Console.WriteLine("delaunay " + (l / deg2rad).ToString() + " " + (l1 / deg2rad).ToString()
                    + " " + (f / deg2rad).ToString() + " " + (d / deg2rad).ToString()
                    + " " + (omega / deg2rad).ToString());
                Console.WriteLine("plan lon " + (lonmer / deg2rad).ToString() + " " + (lonven / deg2rad).ToString()
                    + " " + (lonear / deg2rad).ToString() + " " + (lonmar / deg2rad).ToString()
                    + " " + (lonjup / deg2rad).ToString() + " " + (lonsat / deg2rad).ToString()
                    + " " + (lonurn / deg2rad).ToString() + " " + (lonnep / deg2rad).ToString()
                    + " " + (precrate / deg2rad).ToString());
            }
        }  //  fundarg 



        /* ----------------------------------------------------------------------------
         *
         *                           function iau06xysSeries
         *
         *  this function calculates the XYS parameters for the iau2006 cio theory.
         *
         *  author        : david vallado           davallado@gmail.com   16 jul 2004
         *
         *  revisions
         *    vallado     - consolidate with iau 2000                     14 feb 2005
         *
         *  inputs description                                         range / units
         *    ttt         - julian centuries of tt
         *
         *  outputs       :
         *    nut         - transformation matrix for ire-gcrf
         *    x           - coordinate of cip                                rad
         *    y           - coordinate of cip                                rad
         *    s           - coordinate                                       rad
         *
         *  locals        :
         *    axs0        - real coefficients for x                          rad
         *    ax0i        - integer coefficients for x
         *    ays0        - real coefficients for y                          rad
         *    ay0i        - integer coefficients for y
         *    ass0        - real coefficients for s                          rad
         *    as0i        - integer coefficients for s
         *    apn0         - real coefficients for nutation                   rad
         *    apn0i        - integer coefficients for nutation
         *    appl        - real coefficients for planetary nutation rad
         *    appli       - integer coefficients for planetary nutation
         *    ttt2,ttt3,  - powers of ttt
         *    l           - delaunay element                                 rad
         *    ll          - delaunay element                                 rad
         *    f           - delaunay element                                 rad
         *    d           - delaunay element                                 rad
         *    omega       - delaunay element                                 rad
         *    deltaeps    - change in obliquity                              rad
         *    many others
         *
         *  coupling      :
         *    iau00in     - initialize the arrays
         *
         *  references    : 
         *    vallado       2013, 212-214
         * ---------------------------------------------------------------------------- */

        public void iau06xysSeries
            (
            double ttt, EOPSPWLib.iau00Class iau00arr, double[] fArgs,
            out double x, out double y, out double s
            )
        {
            double ttt2, ttt3, ttt4, ttt5;
            int i, j;
            double[,] nut1 = new double[3, 3];
            double[,] nut2 = new double[3, 3];
            double sum0, sum1, sum2, sum3, sum4;
            double tempval;

            // " to rad
            double convrt = Math.PI / (180.0 * 3600.0);

            ttt2 = ttt * ttt;
            ttt3 = ttt2 * ttt;
            ttt4 = ttt2 * ttt2;
            ttt5 = ttt3 * ttt2;

            // ---------------- find x
            // the iers code puts the constants in here, however
            // don't sum constants in here because they're larger than the last few terms
            sum0 = 0.0;
            for (i = 1305; i >= 0; i--)
            {
                tempval = iau00arr.ax0i[i, 0] * fArgs[0] + iau00arr.ax0i[i, 1] * fArgs[1] + iau00arr.ax0i[i, 2] * fArgs[2]
                    + iau00arr.ax0i[i, 3] * fArgs[3] + iau00arr.ax0i[i, 4] * fArgs[4] + iau00arr.ax0i[i, 5] * fArgs[5]
                    + iau00arr.ax0i[i, 6] * fArgs[6] + iau00arr.ax0i[i, 7] * fArgs[7] + iau00arr.ax0i[i, 8] * fArgs[8]
                    + iau00arr.ax0i[i, 9] * fArgs[9] + iau00arr.ax0i[i, 10] * fArgs[10] + iau00arr.ax0i[i, 11] * fArgs[11]
                    + iau00arr.ax0i[i, 12] * fArgs[12] + iau00arr.ax0i[i, 13] * fArgs[13];
                sum0 = sum0 + iau00arr.ax0[i, 0] * Math.Sin(tempval) + iau00arr.ax0[i, 1] * Math.Cos(tempval);
            }
            sum1 = 0.0;
            // note that the index changes here to j. this is because the ax0i etc
            // indicies go from 1 to 1600, but there are 5 groups. the i index counts through each
            // calculation, and j takes care of the individual summations. note that
            // this same process is used for y and s.
            for (j = 252; j >= 0; j--)
            {
                i = 1306 + j;
                tempval = iau00arr.ax0i[i, 0] * fArgs[0] + iau00arr.ax0i[i, 1] * fArgs[1] + iau00arr.ax0i[i, 2] * fArgs[2]
                    + iau00arr.ax0i[i, 3] * fArgs[3] + iau00arr.ax0i[i, 4] * fArgs[4] + iau00arr.ax0i[i, 5] * fArgs[5]
                    + iau00arr.ax0i[i, 6] * fArgs[6] + iau00arr.ax0i[i, 7] * fArgs[7] + iau00arr.ax0i[i, 8] * fArgs[8]
                    + iau00arr.ax0i[i, 9] * fArgs[9] + iau00arr.ax0i[i, 10] * fArgs[10] + iau00arr.ax0i[i, 11] * fArgs[11]
                    + iau00arr.ax0i[i, 12] * fArgs[12] + iau00arr.ax0i[i, 13] * fArgs[13];
                sum1 = sum1 + iau00arr.ax0[i, 0] * Math.Sin(tempval) + iau00arr.ax0[i, 1] * Math.Cos(tempval);
            }
            sum2 = 0.0;
            for (j = 35; j >= 0; j--)
            {
                i = 1306 + 253 + j;
                tempval = iau00arr.ax0i[i, 0] * fArgs[0] + iau00arr.ax0i[i, 1] * fArgs[1] + iau00arr.ax0i[i, 2] * fArgs[2]
                    + iau00arr.ax0i[i, 3] * fArgs[3] + iau00arr.ax0i[i, 4] * fArgs[4] + iau00arr.ax0i[i, 5] * fArgs[5]
                    + iau00arr.ax0i[i, 6] * fArgs[6] + iau00arr.ax0i[i, 7] * fArgs[7] + iau00arr.ax0i[i, 8] * fArgs[8]
                    + iau00arr.ax0i[i, 9] * fArgs[9] + iau00arr.ax0i[i, 10] * fArgs[10] + iau00arr.ax0i[i, 11] * fArgs[11]
                    + iau00arr.ax0i[i, 12] * fArgs[12] + iau00arr.ax0i[i, 13] * fArgs[13];
                sum2 = sum2 + iau00arr.ax0[i, 0] * Math.Sin(tempval) + iau00arr.ax0[i, 1] * Math.Cos(tempval);
            }
            sum3 = 0.0;
            for (j = 3; j >= 0; j--)
            {
                i = 1306 + 253 + 36 + j;
                tempval = iau00arr.ax0i[i, 0] * fArgs[0] + iau00arr.ax0i[i, 1] * fArgs[1] + iau00arr.ax0i[i, 2] * fArgs[2]
                    + iau00arr.ax0i[i, 3] * fArgs[3] + iau00arr.ax0i[i, 4] * fArgs[4] + iau00arr.ax0i[i, 5] * fArgs[5]
                    + iau00arr.ax0i[i, 6] * fArgs[6] + iau00arr.ax0i[i, 7] * fArgs[7] + iau00arr.ax0i[i, 8] * fArgs[8]
                    + iau00arr.ax0i[i, 9] * fArgs[9] + iau00arr.ax0i[i, 10] * fArgs[10] + iau00arr.ax0i[i, 11] * fArgs[11]
                    + iau00arr.ax0i[i, 12] * fArgs[12] + iau00arr.ax0i[i, 13] * fArgs[13];
                sum3 = sum3 + iau00arr.ax0[i, 0] * Math.Sin(tempval) + iau00arr.ax0[i, 1] * Math.Cos(tempval);
            }
            sum4 = 0.0;
            for (j = 0; j >= 0; j--)
            {
                i = 1306 + 253 + 36 + 4 + j;
                tempval = iau00arr.ax0i[i, 0] * fArgs[0] + iau00arr.ax0i[i, 1] * fArgs[1] + iau00arr.ax0i[i, 2] * fArgs[2]
                    + iau00arr.ax0i[i, 3] * fArgs[3] + iau00arr.ax0i[i, 4] * fArgs[4] + iau00arr.ax0i[i, 5] * fArgs[5]
                    + iau00arr.ax0i[i, 6] * fArgs[6] + iau00arr.ax0i[i, 7] * fArgs[7] + iau00arr.ax0i[i, 8] * fArgs[8]
                    + iau00arr.ax0i[i, 9] * fArgs[9] + iau00arr.ax0i[i, 10] * fArgs[10] + iau00arr.ax0i[i, 11] * fArgs[11]
                    + iau00arr.ax0i[i, 12] * fArgs[12] + iau00arr.ax0i[i, 13] * fArgs[13];
                sum4 = sum4 + iau00arr.ax0[i, 0] * Math.Sin(tempval) + iau00arr.ax0[i, 1] * Math.Cos(tempval);
            }

            x = -0.016617 + 2004.191898 * ttt - 0.4297829 * ttt2
                - 0.19861834 * ttt3 - 0.000007578 * ttt4 + 0.0000059285 * ttt5; // "
            x = x * convrt + sum0 + sum1 * ttt + sum2 * ttt2 + sum3 * ttt3 + sum4 * ttt4;  // rad

            // ---------------- now find y
            sum0 = 0.0;
            for (i = 961; i >= 0; i--)
            {
                tempval = iau00arr.ay0i[i, 0] * fArgs[0] + iau00arr.ay0i[i, 1] * fArgs[1] + iau00arr.ay0i[i, 2] * fArgs[2]
                    + iau00arr.ay0i[i, 3] * fArgs[3] + iau00arr.ay0i[i, 4] * fArgs[4] + iau00arr.ay0i[i, 5] * fArgs[5]
                    + iau00arr.ay0i[i, 6] * fArgs[6] + iau00arr.ay0i[i, 7] * fArgs[7] + iau00arr.ay0i[i, 8] * fArgs[8]
                    + iau00arr.ay0i[i, 9] * fArgs[9] + iau00arr.ay0i[i, 10] * fArgs[10] + iau00arr.ay0i[i, 11] * fArgs[11]
                    + iau00arr.ay0i[i, 12] * fArgs[12] + iau00arr.ay0i[i, 13] * fArgs[13];
                sum0 = sum0 + iau00arr.ay0[i, 0] * Math.Sin(tempval) + iau00arr.ay0[i, 1] * Math.Cos(tempval);
            }

            sum1 = 0.0;
            for (j = 276; j >= 0; j--)
            {
                i = 962 + j;
                tempval = iau00arr.ay0i[i, 0] * fArgs[0] + iau00arr.ay0i[i, 1] * fArgs[1] + iau00arr.ay0i[i, 2] * fArgs[2]
                    + iau00arr.ay0i[i, 3] * fArgs[3] + iau00arr.ay0i[i, 4] * fArgs[4] + iau00arr.ay0i[i, 5] * fArgs[5]
                    + iau00arr.ay0i[i, 6] * fArgs[6] + iau00arr.ay0i[i, 7] * fArgs[7] + iau00arr.ay0i[i, 8] * fArgs[8]
                    + iau00arr.ay0i[i, 9] * fArgs[9] + iau00arr.ay0i[i, 10] * fArgs[10] + iau00arr.ay0i[i, 11] * fArgs[11]
                    + iau00arr.ay0i[i, 12] * fArgs[12] + iau00arr.ay0i[i, 13] * fArgs[13];
                sum1 = sum1 + iau00arr.ay0[i, 0] * Math.Sin(tempval) + iau00arr.ay0[i, 1] * Math.Cos(tempval);
            }
            sum2 = 0.0;
            for (j = 29; j >= 0; j--)
            {
                i = 962 + 277 + j;
                tempval = iau00arr.ay0i[i, 0] * fArgs[0] + iau00arr.ay0i[i, 1] * fArgs[1] + iau00arr.ay0i[i, 2] * fArgs[2]
                    + iau00arr.ay0i[i, 3] * fArgs[3] + iau00arr.ay0i[i, 4] * fArgs[4] + iau00arr.ay0i[i, 5] * fArgs[5]
                    + iau00arr.ay0i[i, 6] * fArgs[6] + iau00arr.ay0i[i, 7] * fArgs[7] + iau00arr.ay0i[i, 8] * fArgs[8]
                    + iau00arr.ay0i[i, 9] * fArgs[9] + iau00arr.ay0i[i, 10] * fArgs[10] + iau00arr.ay0i[i, 11] * fArgs[11]
                    + iau00arr.ay0i[i, 12] * fArgs[12] + iau00arr.ay0i[i, 13] * fArgs[13];
                sum2 = sum2 + iau00arr.ay0[i, 0] * Math.Sin(tempval) + iau00arr.ay0[i, 1] * Math.Cos(tempval);
            }
            sum3 = 0.0;
            for (j = 4; j >= 0; j--)
            {
                i = 962 + 277 + 30 + j;
                tempval = iau00arr.ay0i[i, 0] * fArgs[0] + iau00arr.ay0i[i, 1] * fArgs[1] + iau00arr.ay0i[i, 2] * fArgs[2]
                    + iau00arr.ay0i[i, 3] * fArgs[3] + iau00arr.ay0i[i, 4] * fArgs[4] + iau00arr.ay0i[i, 5] * fArgs[5]
                    + iau00arr.ay0i[i, 6] * fArgs[6] + iau00arr.ay0i[i, 7] * fArgs[7] + iau00arr.ay0i[i, 8] * fArgs[8]
                    + iau00arr.ay0i[i, 9] * fArgs[9] + iau00arr.ay0i[i, 10] * fArgs[10] + iau00arr.ay0i[i, 11] * fArgs[11]
                    + iau00arr.ay0i[i, 12] * fArgs[12] + iau00arr.ay0i[i, 13] * fArgs[13];
                sum3 = sum3 + iau00arr.ay0[i, 0] * Math.Sin(tempval) + iau00arr.ay0[i, 1] * Math.Cos(tempval);
            }
            sum4 = 0.0;
            for (j = 0; j >= 0; j--)
            {
                i = 962 + 277 + 30 + 5 + j;
                tempval = iau00arr.ay0i[i, 0] * fArgs[0] + iau00arr.ay0i[i, 1] * fArgs[1] + iau00arr.ay0i[i, 2] * fArgs[2]
                    + iau00arr.ay0i[i, 3] * fArgs[3] + iau00arr.ay0i[i, 4] * fArgs[4] + iau00arr.ay0i[i, 5] * fArgs[5]
                    + iau00arr.ay0i[i, 6] * fArgs[6] + iau00arr.ay0i[i, 7] * fArgs[7] + iau00arr.ay0i[i, 8] * fArgs[8]
                    + iau00arr.ay0i[i, 9] * fArgs[9] + iau00arr.ay0i[i, 10] * fArgs[10] + iau00arr.ay0i[i, 11] * fArgs[11]
                    + iau00arr.ay0i[i, 12] * fArgs[12] + iau00arr.ay0i[i, 13] * fArgs[13];
                sum4 = sum4 + iau00arr.ay0[i, 0] * Math.Sin(tempval) + iau00arr.ay0[i, 1] * Math.Cos(tempval);
            }

            y = -0.006951 - 0.025896 * ttt - 22.4072747 * ttt2
                + 0.00190059 * ttt3 + 0.001112526 * ttt4 + 0.0000001358 * ttt5;  // "
            y = y * convrt + sum0 + sum1 * ttt + sum2 * ttt2 + sum3 * ttt3 + sum4 * ttt4;  // rad

            // ---------------- now find s
            sum0 = 0.0;
            for (i = 32; i >= 0; i--)
            {
                tempval = iau00arr.as0i[i, 0] * fArgs[0] + iau00arr.as0i[i, 1] * fArgs[1] + iau00arr.as0i[i, 2] * fArgs[2]
                    + iau00arr.as0i[i, 3] * fArgs[3] + iau00arr.as0i[i, 4] * fArgs[4] + iau00arr.as0i[i, 5] * fArgs[5]
                    + iau00arr.as0i[i, 6] * fArgs[6] + iau00arr.as0i[i, 7] * fArgs[7] + iau00arr.as0i[i, 8] * fArgs[8]
                    + iau00arr.as0i[i, 9] * fArgs[9] + iau00arr.as0i[i, 10] * fArgs[10] + iau00arr.as0i[i, 11] * fArgs[11]
                    + iau00arr.as0i[i, 12] * fArgs[12] + iau00arr.as0i[i, 13] * fArgs[13];
                sum0 = sum0 + iau00arr.as0[i, 0] * Math.Sin(tempval) + iau00arr.as0[i, 1] * Math.Cos(tempval);
            }
            sum1 = 0.0;
            for (j = 2; j >= 0; j--)
            {
                i = 33 + j;
                tempval = iau00arr.as0i[i, 0] * fArgs[0] + iau00arr.as0i[i, 1] * fArgs[1] + iau00arr.as0i[i, 2] * fArgs[2]
                    + iau00arr.as0i[i, 3] * fArgs[3] + iau00arr.as0i[i, 4] * fArgs[4] + iau00arr.as0i[i, 5] * fArgs[5]
                    + iau00arr.as0i[i, 6] * fArgs[6] + iau00arr.as0i[i, 7] * fArgs[7] + iau00arr.as0i[i, 8] * fArgs[8]
                    + iau00arr.as0i[i, 9] * fArgs[9] + iau00arr.as0i[i, 10] * fArgs[10] + iau00arr.as0i[i, 11] * fArgs[11]
                    + iau00arr.as0i[i, 12] * fArgs[12] + iau00arr.as0i[i, 13] * fArgs[13];
                sum1 = sum1 + iau00arr.as0[i, 0] * Math.Sin(tempval) + iau00arr.as0[i, 1] * Math.Cos(tempval);
            }
            sum2 = 0.0;
            for (j = 24; j >= 0; j--)
            {
                i = 33 + 3 + j;
                tempval = iau00arr.as0i[i, 0] * fArgs[0] + iau00arr.as0i[i, 1] * fArgs[1] + iau00arr.as0i[i, 2] * fArgs[2]
                    + iau00arr.as0i[i, 3] * fArgs[3] + iau00arr.as0i[i, 4] * fArgs[4] + iau00arr.as0i[i, 5] * fArgs[5]
                    + iau00arr.as0i[i, 6] * fArgs[6] + iau00arr.as0i[i, 7] * fArgs[7] + iau00arr.as0i[i, 8] * fArgs[8]
                    + iau00arr.as0i[i, 9] * fArgs[9] + iau00arr.as0i[i, 10] * fArgs[10] + iau00arr.as0i[i, 11] * fArgs[11]
                    + iau00arr.as0i[i, 12] * fArgs[12] + iau00arr.as0i[i, 13] * fArgs[13];
                sum2 = sum2 + iau00arr.as0[i, 0] * Math.Sin(tempval) + iau00arr.as0[i, 1] * Math.Cos(tempval);
            }
            sum3 = 0.0;
            for (j = 3; j >= 0; j--)
            {
                i = 33 + 3 + 25 + j;
                tempval = iau00arr.as0i[i, 0] * fArgs[0] + iau00arr.as0i[i, 1] * fArgs[1] + iau00arr.as0i[i, 2] * fArgs[2]
                    + iau00arr.as0i[i, 3] * fArgs[3] + iau00arr.as0i[i, 4] * fArgs[4] + iau00arr.as0i[i, 5] * fArgs[5]
                    + iau00arr.as0i[i, 6] * fArgs[6] + iau00arr.as0i[i, 7] * fArgs[7] + iau00arr.as0i[i, 8] * fArgs[8]
                    + iau00arr.as0i[i, 9] * fArgs[9] + iau00arr.as0i[i, 10] * fArgs[10] + iau00arr.as0i[i, 11] * fArgs[11]
                    + iau00arr.as0i[i, 12] * fArgs[12] + iau00arr.as0i[i, 13] * fArgs[13];
                sum3 = sum3 + iau00arr.as0[i, 0] * Math.Sin(tempval) + iau00arr.as0[i, 1] * Math.Cos(tempval);
            }
            sum4 = 0.0;
            for (j = 0; j >= 0; j--)
            {
                i = 33 + 3 + 25 + 4 + j;
                tempval = iau00arr.as0i[i, 0] * fArgs[0] + iau00arr.as0i[i, 1] * fArgs[1] + iau00arr.as0i[i, 2] * fArgs[2]
                    + iau00arr.as0i[i, 3] * fArgs[3] + iau00arr.as0i[i, 4] * fArgs[4] + iau00arr.as0i[i, 5] * fArgs[5]
                    + iau00arr.as0i[i, 6] * fArgs[6] + iau00arr.as0i[i, 7] * fArgs[7] + iau00arr.as0i[i, 8] * fArgs[8]
                    + iau00arr.as0i[i, 9] * fArgs[9] + iau00arr.as0i[i, 10] * fArgs[10] + iau00arr.as0i[i, 11] * fArgs[11]
                    + iau00arr.as0i[i, 12] * fArgs[12] + iau00arr.as0i[i, 13] * fArgs[13];
                sum4 = sum4 + iau00arr.as0[i, 0] * Math.Sin(tempval) + iau00arr.as0[i, 1] * Math.Cos(tempval);
            }

            s = 0.000094 + 0.00380865 * ttt - 0.00012268 * ttt2
                - 0.07257411 * ttt3 + 0.00002798 * ttt4 + 0.00001562 * ttt5;   // "
            //            + 0.00000171*ttt* Math.Sin(fArgs[4]) + 0.00000357*ttt* Math.Cos(2.0*fArgs[4])  
            //            + 0.00074353*ttt2* Math.Sin(fArgs[4]) + 0.00005691*ttt2* Math.Sin(2.0*(fArgs[2]-fArgs[3]+fArgs[4]))  
            //            + 0.00000984*ttt2* Math.Sin(2.0*(fArgs[2]+fArgs[4])) - 0.00000885*ttt2* Math.Sin(2.0*fArgs[4]);
            s = -x * y * 0.5 + s * convrt + sum0 + sum1 * ttt + sum2 * ttt2 + sum3 * ttt3 + sum4 * ttt4;  // rad
        }  // iau06xysSeries


        /* ----------------------------------------------------------------------------
        *
        *                           function iau06xys
        *
        *  this function calculates the transformation matrix that accounts for the
        *    effects of precession-nutation in the iau2006 cio theory.
        *
        *  author        : david vallado           davallado@gmail.com   16 jul 2004
        *
        *  revisions
        *    vallado     - consolidate with iau 2000                     14 feb 2005
        *
        *  inputs description                                           range / units
        *    ttt         - julian centuries of tt
        *    ddx         - delta x correction to gcrf                       rad
        *    ddy         - delta y correction to gcrf                       rad
        *    interp      - interpolation type (f for full series)           f, n, l, s
        *                  none, linear, spline
        *
        *  outputs       :
        *    nut         - transformation matrix for ire-gcrf
        *    x           - coordinate of cip                                rad
        *    y           - coordinate of cip                                rad
        *    s           - coordinate                                       rad
        *
        *  locals        :
        *    a           - 
        *
        *  coupling      :
        *    iau00in     - initialize the arrays
        *
        *  references    : 
        *    vallado       2013, 212-214
        * ---------------------------------------------------------------------------- */

        public double[,] iau06xys
        (
        double jdtt, double ttt, double ddx, double ddy, char interp,
        EOPSPWLib.iau00Class iau00arr, double[] fArgs, double jdxysstart,
        out double x, out double y, out double s
        )
        {
            double[,] nut1 = new double[3, 3];
            double[,] nut2 = new double[3, 3];
            double a;

            // ---------------- find xys
            if (!interp.Equals("x"))
                iau06xysSeries(ttt, iau00arr, fArgs, out x, out y, out s);
            else
                findxysparam(jdtt, 0.0, interp, xysarr, jdxysstart, out x, out y, out s);

            // add corrections if available
            x = x + ddx;
            y = y + ddy;

            // ---------------- now find a
            // units take on whatever x and y are
            a = 0.5 + 0.125 * (x * x + y * y);

            // ----------------- find nutation matrix ----------------------
            nut1[0, 0] = 1.0 - a * x * x;
            nut1[0, 1] = -a * x * y;
            nut1[0, 2] = x;
            nut1[1, 0] = -a * x * y;
            nut1[1, 1] = 1.0 - a * y * y;
            nut1[1, 2] = y;
            nut1[2, 0] = -x;
            nut1[2, 1] = -y;
            nut1[2, 2] = 1.0 - a * (x * x + y * y);

            nut2[2, 2] = 1.0;
            nut2[0, 0] = Math.Cos(s);
            nut2[1, 1] = Math.Cos(s);
            nut2[0, 1] = Math.Sin(s);
            nut2[1, 0] = -Math.Sin(s);

            return MathTimeLibr.matmult(nut1, nut2, 3, 3, 3);

            //  alternate approach       
            //if (x != 0.0 && y != 0.0)
            //    e = Math.Atan2(y, x);
            //else
            //    e = 0.0;
            //fArgs[3] = Math.Atan(Math.Sqrt((x * x + y * y) / (1.0 - x * x - y * y)));
            //nut1 = rot3mat(-e)*rot2mat(-fArgs[3])*rot3mat(e+s)
        }  // iau06xys


        /* -----------------------------------------------------------------------------
        *
        *                           function createXYS
        *
        *  this function creates the xys data file. the iau-2006/2000 cio series is long
        *  and can consume comutational time. this appraoches precalculates the xys parameters
        *  and stores in a datafile for very fast efficient use. 
        *
        *  author        : david vallado           davallado@gmail.com   22 jan 2018
        *
        *  revisions
        *
        *  inputs          description                           range / units
        *    xysLoc      - location for xys data file  
        *    infilename  - file name
        *
        *  outputs       :
        *    xysarr      - array of xys data records
        *    jdxysstart  - julian date of the start of the xysarr data
        *    jdfxysstart - julian date fraction of the start of the xysarr data
        *
        *  locals        :
        *                -
        *
        *  coupling      :
        *
        *  references    :
        *
        *  -------------------------------------------------------------------------- */


        // create a file of XYS values for interpolation
        public void createXYS
            (
               string outLoc,
               EOPSPWLib.iau00Class iau00arr, double[] fArgs
            )
        {
            Int32 i;
            double dt, jdtt, jdftt, ttt, x, y, s;
            StringBuilder strbuild = new StringBuilder();
            // 000. gives leading 0's
            // ;+00.;-00. gives signs in front
            string fmt = "0.000000000000";

            MathTimeLibr.jday(1956, 12, 31, 0, 0, 0.0, out jdtt, out jdftt);
            dt = 1.0;  // time step 1 day

            // create until 2100
            for (i = 0; i < 142 * 365; i++)
            {
                jdtt = jdtt + dt;
                ttt = (jdtt + jdftt - 2451545.0) / 36525.0;
                iau06xysSeries(ttt, iau00arr, fArgs, out x, out y, out s);

                strbuild.AppendLine(jdtt.ToString("0.000000").PadLeft(8) + " "
                    + jdftt.ToString("0.0000000000").PadLeft(8) + " "
                    + x.ToString(fmt) + " " + y.ToString(fmt) + " " + s.ToString(fmt));
            }

            // write data out
            File.WriteAllText(outLoc + "xysdata.dat", strbuild.ToString());
        }  // create XYS


        /* -----------------------------------------------------------------------------
        *
        *                           function initXYS
        *
        *  this function initializes the jpl planetary ephemeris data. the input
        *  data files are from processing the ascii files into a text file of sun
        *  and moon positions, generated by FOR code and the JPL DE files.
        *
        *  author        : david vallado           davallado@gmail.com   22 jan 2018
        *
        *  revisions
        *
        *  inputs          description                           range / units
        *    xysLoc      - location for xys data file  
        *    infilename  - file name
        *
        *  outputs       :
        *    xysarr      - array of xys data records
        *    jdxysstart  - julian date of the start of the xysarr data
        *    jdfxysstart - julian date fraction of the start of the xysarr data
        *
        *  locals        :
        *                -
        *
        *  coupling      :
        *
        *  references    :
        *
        *  -------------------------------------------------------------------------- */

        public void initXYS
            (
            ref xysdataClass[] xysarr,
            string xysLoc,
            string infilename,
            out double jdxysstart, out double jdfxysstart
            )
        {
            string pattern;
            Int32 i;
            jdxysstart = 0.0;
            jdfxysstart = 0.0;

            // read the whole file at once into lines of an array, indexed from 0
            string[] fileData = File.ReadAllLines(xysLoc + infilename);

            pattern = @"^(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)";
            for (i = 0; i < fileData.Count(); i++)
            {
                // set new record as they are needed
                xysarr[i] = new xysdataClass();

                //line = Regex.Replace(fileData[i], @"\s+", "|");
                //string[] linedata = line.Split('|');
                string[] linedata = Regex.Split(fileData[i], pattern);

                xysarr[i].jdtt = Convert.ToDouble(linedata[1]);
                xysarr[i].jdftt = Convert.ToDouble(linedata[2]);
                xysarr[i].x = Convert.ToDouble(linedata[3]);
                xysarr[i].y = Convert.ToDouble(linedata[4]);
                xysarr[i].s = Convert.ToDouble(linedata[5]);

                xysarr[i].mjd = xysarr[i].jdtt + xysarr[i].jdftt - 2400000.5;

                // ---- find epoch date
                if (i == 0)
                {
                    jdxysstart = xysarr[i].jdtt;
                    jdfxysstart = xysarr[i].jdftt;
                    xysarr[i].mjd = jdxysstart + jdfxysstart;
                }
            }

            xysdesize = i;  // global size of the data read in
        }  // initXYS


        /* -----------------------------------------------------------------------------
        *
        *                           function findxysparam
        *
        *  this routine finds the xys parameters for the iau 2006/2000 transformation. 
        *  several types of interpolation are available. this allows you to use the full cio
        *  series, but maintain very fast performance. 
        *
        *  author        : david vallado               davallado@gmail.com   12 dec 2005
        *
        *  inputs          description                               range / units
        *    jdttt         - epoch julian date                     days from 4713 BC
        *    jdftt         - epoch julian date fraction            day fraction from jdtt
        *    interp        - interpolation                        n-none, l-linear, s-spline
        *    xysarr        - array of xys data records
        *    jdxysstart    - julian date of the start of the xysarr data (set in initxys)
        *
        *  outputs       :
        *    x           - x component of cio                         rad
        *    y           - y component of cio                         rad
        *    s           -                                            rad
        *
        *  locals        :
        *                -
        *
        *  coupling      :
        *    none        -
        *
        *  references    :
        *    vallado       2013,
        * --------------------------------------------------------------------------- */

        public void findxysparam
            (
            double jdtt, double jdftt, char interp,
            xysdataClass[] xysarr,
            double jdxysstart,
            out double x, out double y, out double s
        )
        {
            Int32 recnum;
            Int32 off1, off2;
            double fixf, jdxysstarto, jdb, mfme;
            xysdataClass xysrec, nextxysrec;

            // the ephemerides are centered on jdtt, but it turns out to be 0.5, or 0000 hrs.
            // check if any whole days in jdf
            jdb = Math.Floor(jdtt + jdftt) + 0.5;  // want jd at 0 hr
            mfme = (jdtt + jdftt - jdb) * 1440.0;
            if (mfme < 0.0)
                mfme = 1440.0 + mfme;

            // ---- read data for day of interest
            jdxysstarto = Math.Floor(jdtt + jdftt - jdxysstart);
            recnum = Convert.ToInt32(jdxysstarto);

            // check for out of bound values
            if ((recnum >= 1) && (recnum <= xyssize))
            {
                xysrec = xysarr[recnum];

                // ---- set non-interpolated values
                x = xysrec.x;
                y = xysrec.y;
                s = xysrec.s;

                // ---- find nutation parameters for use in optimizing speed

                // ---- do linear interpolation
                if (interp == 'l')
                {
                    nextxysrec = xysarr[recnum + 1];
                    fixf = mfme / 1440.0;

                    x = xysrec.x + (nextxysrec.x - xysrec.x) * fixf;
                    y = xysrec.y + (nextxysrec.y - xysrec.y) * fixf;
                    s = xysrec.s + (nextxysrec.s - xysrec.s) * fixf;
                }

                // ---- do spline interpolations
                if (interp == 's')
                {
                    off1 = 1;     // every 1 days data 
                    off2 = 2;
                    fixf = mfme / 1440.0; // get back to days for this since each step is in days
                                          // setup so the interval is in between points 2 and 3
                    x = MathTimeLibr.cubicinterp(xysarr[recnum - off1].x, xysarr[recnum].x, xysarr[recnum + off1].x,
                        xysarr[recnum + off2].x,
                        xysarr[recnum - off1].mjd, xysarr[recnum].mjd, xysarr[recnum + off1].mjd, xysarr[recnum + off2].mjd,
                        xysarr[recnum].mjd + fixf);
                    y = MathTimeLibr.cubicinterp(xysarr[recnum - off1].y, xysarr[recnum].y, xysarr[recnum + off1].y,
                        xysarr[recnum + off2].y,
                        xysarr[recnum - off1].mjd, xysarr[recnum].mjd, xysarr[recnum + off1].mjd, xysarr[recnum + off2].mjd,
                        xysarr[recnum].mjd + fixf);
                    s = MathTimeLibr.cubicinterp(xysarr[recnum - off1].s, xysarr[recnum].s, xysarr[recnum + off1].s,
                        xysarr[recnum + off2].s,
                        xysarr[recnum - off1].mjd, xysarr[recnum].mjd, xysarr[recnum + off1].mjd, xysarr[recnum + off2].mjd,
                        xysarr[recnum].mjd + fixf);
                }
            }
            // set default values
            else
            {
                x = 0.0;
                y = 0.0;
                s = 0.0;
            }
        }  // findXYSparam



        /* ----------------------------------------------------------------------------
         *
         *                           function iau00pn
         *
         *  this function calculates the transformation matrix that accounts for the
         *    effects of precession-nutation in the iau2010 theory.
         *
         *  author        : david vallado           davallado@gmail.com   16 jul 2004
         *
         *  revisions
         *    vallado     - consolidate with iau 2000                     14 feb 2005
         *
         *  inputs description                                         range / units
         *    ttt         - julian centuries of tt
         *
         *  outputs       :
         *    nut         - transformation matrix for ire-gcrf
         *    x           - coordinate of cip                                rad
         *    y           - coordinate of cip                                rad
         *    s           - coordinate                                       rad
         *
         *  locals        :
         *    axs0        - real coefficients for x                          rad
         *    ax0i        - integer coefficients for x
         *    ays0        - real coefficients for y                          rad
         *    ay0i        - integer coefficients for y
         *    ass0        - real coefficients for s                          rad
         *    as0i        - integer coefficients for s
         *    apn0        - real coefficients for nutation                   rad
         *    apn0i       - integer coefficients for nutation
         *    appl        - real coefficients for planetary nutation         rad
         *    appli       - integer coefficients for planetary nutation
         *    ttt2,ttt3,  - powers of ttt
         *    fArgs[0]    - delaunay element                                 rad
         *    fArgs[1]    - delaunay element                                 rad
         *    fArgs[2]    - delaunay element                                 rad
         *    fArgs[3]    - delaunay element                                 rad
         *    fArgs[4]    - delaunay element                                 rad
         *    deltaeps    - change in obliquity                              rad
         *    many others
         *
        *  coupling      :
         *    iau00in     - initialize the arrays
         *    fundarg     - find the fundamental arguments
         *
         *  references    : 
         *    vallado       2013, 212-214
         * ---------------------------------------------------------------------------- */

        public double[,] iau00pn
            (
            double ttt, double ddx, double ddy, EOpt opt,
            EOPSPWLib.iau00Class iau00arr, double[] fArgs,
            double x, double y, double s
            )
        {
            double ttt2, ttt3, ttt4, ttt5;
            double[,] nut1 = new double[3, 3];
            double[,] nut2 = new double[3, 3];
            double a;

            ttt2 = ttt * ttt;
            ttt3 = ttt2 * ttt;
            ttt4 = ttt2 * ttt2;
            ttt5 = ttt3 * ttt2;

            // ---------------- find x

            // add corrections if available
            x = x + ddx;
            y = y + ddy;

            // ---------------- now find a
            a = 0.5 + 0.125 * (x * x + y * y); // units take on whatever x and y are

            // ----------------- find nutation matrix ----------------------
            nut1[0, 0] = 1.0 - a * x * x;
            nut1[0, 1] = -a * x * y;
            nut1[0, 2] = x;
            nut1[1, 0] = -a * x * y;
            nut1[1, 1] = 1.0 - a * y * y;
            nut1[1, 2] = y;
            nut1[2, 0] = -x;
            nut1[2, 1] = -y;
            nut1[2, 2] = 1.0 - a * (x * x + y * y);

            nut2[2, 2] = 1.0;
            nut2[0, 0] = Math.Cos(s);
            nut2[1, 1] = Math.Cos(s);
            nut2[0, 1] = Math.Sin(s);
            nut2[1, 0] = -Math.Sin(s);

            return MathTimeLibr.matmult(nut1, nut2, 3, 3, 3);

            //  the matrix apears to be orthogonal now, so the extra processing is not needed.
            //  alt approach       
            //  if (x ~= 0.0) && (y ~= 0.0)
            //      e = atan2(y, x);
            //    else
            //      e = 0.0;
            //  end;
            //  fArgs[3] = atan(sqrt((x^2 + y^2) / (1.0-x^2-y^2)) );
            //  nut1 = rot3mat(-e)*rot2mat(-fArgs[3])*rot3mat(e+s)
        }  // iau00pn




        /* -----------------------------------------------------------------------------
        *
        *                           function precess
        *
        *  this function calculates the transformation matrix that accounts for the effects
        *    of precession. both the 1980 and 2000 theories are handled. note that the
        *    required parameters differ a little.
        *
        *  author        : david vallado           davallado@gmail.com   25 jun 2002
        *
        *  revisions
        *    vallado     - conversion to c++                             21 feb 2005
        *    vallado     - misc updates, nomenclature, etc               23 nov 2005
        *    vallado     - conversion to c#                              16 Nov 2011
        *
        *  inputs          description                                 range / units
        *    ttt         - julian centuries of tt
        *    opt         - method option                           e80, e96, e00a, e06cio, e06eq
        *
        *  outputs       :
        *    psia        - cannonical precession angle                    rad    (00 only)
        *    wa          - cannonical precession angle                    rad    (00 only)
        *    epsa        - cannonical precession angle                    rad    (00 only)
        *    chia        - cannonical precession angle                    rad    (00 only)
        *    prec        - matrix converting from "mod" to gcrf
        *
        *  locals        :
        *    zeta        - precession angle                               rad
        *    z           - precession angle                               rad
        *    theta       - precession angle                               rad
        *    oblo        - obliquity value at j2000 epoch                  "
        *
        *  coupling      :
        *    none        -
        *
        *  references    :
        *    vallado       2013, 226
        * --------------------------------------------------------------------------- */

        public double[,] precess
            (
            double ttt, EOpt opt,
            out double psia, out double wa, out double epsa, out double chia
            )
        {
            double[] outvec = new double[3];
            double[,] prec = new Double[3, 3];
            double convrt, zeta, theta, z, coszeta, sinzeta, costheta, sintheta, cosz, sinz, oblo;
            double[,] p1 = new double[3, 3];
            double[,] p2 = new double[3, 3];
            double[,] p3 = new double[3, 3];
            double[,] p4 = new double[3, 3];
            double[,] tr1 = new double[3, 3];
            double[,] tr2 = new double[3, 3];

            convrt = Math.PI / (180.0 * 3600.0);  // " to rad

            // ------------------- iau-76 precession angles --------------------
            if ((opt.Equals(EOpt.e80)) | (opt.Equals(EOpt.e96)))
            {
                oblo = 84381.448; // "
                psia = ((-0.001147 * ttt - 1.07259) * ttt + 5038.7784) * ttt; // "
                wa = ((-0.007726 * ttt + 0.05127) * ttt) + oblo;
                epsa = ((0.001813 * ttt - 0.00059) * ttt - 46.8150) * ttt + oblo;
                chia = ((-0.001125 * ttt - 2.38064) * ttt + 10.5526) * ttt;

                zeta = ((0.017998 * ttt + 0.30188) * ttt + 2306.2181) * ttt; // "
                theta = ((-0.041833 * ttt - 0.42665) * ttt + 2004.3109) * ttt;
                z = ((0.018203 * ttt + 1.09468) * ttt + 2306.2181) * ttt;
            }
            else
            {
                oblo = 84381.406; // "
                psia = ((((-0.0000000951 * ttt + 0.000132851) * ttt - 0.00114045) * ttt - 1.0790069) * ttt + 5038.481507) * ttt; // "
                wa = ((((0.0000003337 * ttt - 0.000000467) * ttt - 0.00772503) * ttt + 0.0512623) * ttt - 0.025754) * ttt + oblo;
                epsa = ((((-0.0000000434 * ttt - 0.000000576) * ttt + 0.00200340) * ttt - 0.0001831) * ttt - 46.836769) * ttt + oblo;
                chia = ((((-0.0000000560 * ttt + 0.000170663) * ttt - 0.00121197) * ttt - 2.3814292) * ttt + 10.556403) * ttt;

                zeta = ((((-0.0000003173 * ttt - 0.000005971) * ttt + 0.01801828) * ttt + 0.2988499) * ttt + 2306.083227) * ttt + 2.650545; // "
                theta = ((((-0.0000001274 * ttt - 0.000007089) * ttt - 0.04182264) * ttt - 0.4294934) * ttt + 2004.191903) * ttt;
                z = ((((0.0000002904 * ttt - 0.000028596) * ttt + 0.01826837) * ttt + 1.0927348) * ttt + 2306.077181) * ttt - 2.650545;
                // fukishima williams approach 
                //gamb
                //     zeta = ((((0.0000000260 * ttt - 0.000002788) * ttt - 0.00031238) * ttt + 0.4932044) * ttt + 10.556378) * ttt + -0.052928; // "
                // phib
                //     theta = ((((-0.0000000176 * ttt - 0.000000440) * ttt + 0.00053289) * ttt + 0.0511268) * ttt + -46.811016) * ttt + 84381.412819;
                //psib
                //     z = ((((-0.0000000148 * ttt - 0.000026452) * ttt - 0.00018522) * ttt + 1.5584175) * ttt + 5038.481484) * ttt - 0.041775;
            }

            // convert units to rad
            psia = psia * convrt;
            wa = wa * convrt;
            oblo = oblo * convrt;
            epsa = epsa * convrt;
            chia = chia * convrt;

            zeta = zeta * convrt;
            theta = theta * convrt;
            z = z * convrt;

            if (printopt == 'y')
            {
                Console.WriteLine("psia " + psia.ToString() + " wa " + wa.ToString() + " epsa "
                + epsa.ToString() + " chia " + chia.ToString());
                Console.WriteLine("zeta " + zeta.ToString() + " theta " + theta.ToString() + " z "
                    + z.ToString());
                double rad = 180.0 / Math.PI;
                Console.WriteLine("psia " + (psia * rad).ToString() + " wa " + (wa * rad).ToString() + " epsa "
                    + (epsa * rad).ToString() + " chia " + (chia).ToString());
                Console.WriteLine("zeta " + (zeta * rad).ToString() + " theta " + (theta * rad).ToString() + " z "
                    + (z * rad).ToString());
            }
            //          if ((opt.Equals(EOpt.e80)) | (opt.Equals(EOpt.e96)))
            {
                coszeta = Math.Cos(zeta);
                sinzeta = Math.Sin(zeta);
                costheta = Math.Cos(theta);
                sintheta = Math.Sin(theta);
                cosz = Math.Cos(z);
                sinz = Math.Sin(z);

                // ----------------- form matrix  mod to gcrf ------------------
                prec[0, 0] = coszeta * costheta * cosz - sinzeta * sinz;
                prec[0, 1] = coszeta * costheta * sinz + sinzeta * cosz;
                prec[0, 2] = coszeta * sintheta;
                prec[1, 0] = -sinzeta * costheta * cosz - coszeta * sinz;
                prec[1, 1] = -sinzeta * costheta * sinz + coszeta * cosz;
                prec[1, 2] = -sinzeta * sintheta;
                prec[2, 0] = -sintheta * cosz;
                prec[2, 1] = -sintheta * sinz;
                prec[2, 2] = costheta;
            }
            //else
            //{
            //    p1 = MathTimeLibr.rot3mat(-chia);
            //    p2 = MathTimeLibr.rot1mat(wa);
            //    p3 = MathTimeLibr.rot3mat(psia);
            //    p4 = MathTimeLibr.rot1mat(-epsa);
            //    tr1 = MathTimeLibr.matmult(p4, p3, 3, 3, 3);
            //    tr2 = MathTimeLibr.matmult(tr1, p2, 3, 3, 3);
            //    prec = MathTimeLibr.matmult(tr2, p1, 3, 3, 3);
            //    Console.WriteLine( String.Format("prec = {0}  {1}  {2}", prec[0, 0], prec[0, 1], prec[0, 2]));
            //    Console.WriteLine( String.Format("prec = {0}  {1}  {2}", prec[1, 0], prec[1, 1], prec[1, 2]));
            //    Console.WriteLine( String.Format("prec = {0}  {1}  {2}", prec[2, 0], prec[2, 1], prec[2, 2]));

            //    //p1 = MathTimeLibr.rot3mat(zeta);
            //p2 = MathTimeLibr.rot1mat(theta);
            //p3 = MathTimeLibr.rot3mat(-z);
            //p4 = MathTimeLibr.rot1mat(-epsa);
            //tr1 = MathTimeLibr.matmult(p4, p3, 3, 3, 3);
            //tr2 = MathTimeLibr.matmult(tr1, p2, 3, 3, 3);
            //prec = MathTimeLibr.matmult(tr2, p1, 3, 3, 3);
            //Console.WriteLine( String.Format("prec1 = {0}  {1}  {2}", prec[0, 0], prec[0, 1], prec[0, 2]));
            //Console.WriteLine( String.Format("prec1 = {0}  {1}  {2}", prec[1, 0], prec[1, 1], prec[1, 2]));
            //Console.WriteLine( String.Format("prec1 = {0}  {1}  {2}", prec[2, 0], prec[2, 1], prec[2, 2]));
            //    }

            return prec;
        }  //  precess 


        /* -----------------------------------------------------------------------------
        *
        *                           function nutation
        *
        *  this function calculates the transformation matrix that accounts for the
        *    effects of nutation within iau-76/fk5.
        *
        *  author        : david vallado           davallado@gmail.com   27 jun 2002
        *
        *  revisions
        *    vallado     - consolidate with iau 2000                     14 feb 2005
        *    vallado     - conversion to c++                             21 feb 2005
        *    vallado     - conversion to c#                              16 Nov 2011
        *
        *  inputs          description                                 range / units
        *    ttt         - julian centuries of tt
        *    ddpsi       - delta psi correction to gcrf                      rad
        *    ddeps       - delta eps correction to gcrf                      rad
        *    iau80arr    - record containing the iau80 constants rad
        *    opt         - method option                                 e00cio, e00a, e96, e80
        *
        *  outputs       :
        *    deltapsi    - nutation in longitude angle                       rad
        *    trueeps     - true obliquity of the ecliptic                    rad
        *    meaneps     - mean obliquity of the ecliptic                    rad
        *    nut         - transform matrix for tod - mod
        *
        *  locals        :
        *    iar80       - integers for fk5 1980
        *    rar80       - reals for fk5 1980                                rad
         *    fArgs[0]    - delaunay element                                 rad
         *    fArgs[1]    - delaunay element                                 rad
         *    fArgs[2]    - delaunay element                                 rad
         *    fArgs[3]    - delaunay element                                 rad
         *    fArgs[4]    - delaunay element                                 rad
        *    deltaeps    - change in obliquity                               rad
        *
        *  coupling      :
        *    fundarg     - find fundamental arguments
        *    fmod      - modulus division
        *
        *  references    :
        *    vallado       2013, 213, 224
        * --------------------------------------------------------------------------- */

        public double[,] nutation
            (
            double ttt, double ddpsi, double ddeps,
            EOPSPWLib.iau80Class iau80arr, EOpt opt, double[] fArgs,
            out double deltapsi, out double deltaeps, out double trueeps, out double meaneps
            )
        {
            double[,] nut = new double[3, 3];
            double deg2rad, cospsi, sinpsi, coseps, sineps, costrueeps, sintrueeps;
            int i;
            double tempval;

            deg2rad = Math.PI / 180.0;

            // ---- determine coefficients for iau 1980 nutation theory ----
            meaneps = ((0.001813 * ttt - 0.00059) * ttt - 46.8150) * ttt + 84381.448;
            meaneps = (meaneps / 3600.0 % 360.0);
            meaneps = meaneps * deg2rad;

            deltapsi = 0.0;
            deltaeps = 0.0;
            for (i = 105; i >= 0; i--)
            {
                tempval = iau80arr.iar80[i, 0] * fArgs[0] + iau80arr.iar80[i, 1] * fArgs[1] + iau80arr.iar80[i, 2] * fArgs[2]
                    + iau80arr.iar80[i, 3] * fArgs[3] + iau80arr.iar80[i, 4] * fArgs[4];
                deltapsi = deltapsi + (iau80arr.rar80[i, 0] + iau80arr.rar80[i, 1] * ttt) * Math.Sin(tempval);
                deltaeps = deltaeps + (iau80arr.rar80[i, 2] + iau80arr.rar80[i, 3] * ttt) * Math.Cos(tempval);
            }

            // --------------- find nutation parameters --------------------
            deltapsi = (deltapsi + ddpsi) % (2.0 * Math.PI);
            deltaeps = (deltaeps + ddeps) % (2.0 * Math.PI);

            double rad = 180.0 / Math.PI;
            if (printopt == 'y')
                Console.WriteLine(String.Format("meaneps {0}  deltapsi {1}  deltaeps {2} ttt {3} ",
                    meaneps * rad, deltapsi * rad, deltaeps * rad, ttt));

            trueeps = meaneps + deltaeps;

            cospsi = Math.Cos(deltapsi);
            sinpsi = Math.Sin(deltapsi);
            coseps = Math.Cos(meaneps);
            sineps = Math.Sin(meaneps);
            costrueeps = Math.Cos(trueeps);
            sintrueeps = Math.Sin(trueeps);

            nut[0, 0] = cospsi;
            nut[0, 1] = costrueeps * sinpsi;
            nut[0, 2] = sintrueeps * sinpsi;
            nut[1, 0] = -coseps * sinpsi;
            nut[1, 1] = costrueeps * coseps * cospsi + sintrueeps * sineps;
            nut[1, 2] = sintrueeps * coseps * cospsi - sineps * costrueeps;
            nut[2, 0] = -sineps * sinpsi;
            nut[2, 1] = costrueeps * sineps * cospsi - sintrueeps * coseps;
            nut[2, 2] = sintrueeps * sineps * cospsi + costrueeps * coseps;

            // alternate approach
            //astMathr.MathTimeLibr.rot1mat(trueeps, n1);
            //astMathr.rot3mat(deltapsi, n2);
            //astMathr.MathTimeLibr.rot1mat(-meaneps, n3);
            //astMathr.MathTimeLibr.matmult(n2, n1, tr1, 3, 3, 3);
            //astMathr.MathTimeLibr.matmult(n3, tr1, nut, 3, 3, 3);

            return nut;
        }  //  nutation 


        /* ----------------------------------------------------------------------------
        *
        *                        function precnutbias06a
        *
        *  this function calculates the transformation matrix that accounts for the
        *    effects of precession-nutation-bias in the iau2006a equinox theory.
        *
        *  author        : david vallado           davallado@gmail.com   16 jul 2004
        *
        *  revisions
        *    vallado     - consolidate with iau 2000                     14 feb 2005
        *
        *  inputs description                    range / units
        *    ttt         - julian centuries of tt
        *
        *  outputs       :
        *    nut         - transformation matrix for ire-gcrf
        *    deltapsi    - change in longitude rad
         *    fArgs[0]    - delaunay element                                 rad
         *    fArgs[1]    - delaunay element                                 rad
         *    fArgs[2]    - delaunay element                                 rad
         *    fArgs[3]    - delaunay element                                 rad
         *    fArgs[4]    - delaunay element                                 rad
        *    many others for planetary values             rad
        *
        *  locals        :
        *    x           - coordinate rad
        *    y           - coordinate rad
        *    s           - coordinate rad
        *    ax0         - real coefficients for x rad
        *    ax0i        - integer coefficients for x
        *    ay0         - real coefficients for y rad
        *    ay0i        - integer coefficients for y
        *    as0         - real coefficients for s rad
        *    as0i        - integer coefficients for s
        *    apn0         - real coefficients for nutation rad
        *    apn0i        - integer coefficients for nutation
        *    appl        - real coefficients for planetary nutation rad
        *    appli       - integer coefficients for planetary nutation
        *    ttt2,ttt3,  - powers of ttt
        *    deltaeps    - change in obliquity rad
        *
        *  coupling      :
        *    iau00in     - initialize the arrays
        *    fundarg     - find the fundamental arguments
        *    precess     - find the precession quantities
        *
        *  references    :
        *    vallado       2004, 212-214
        * ---------------------------------------------------------------------------- */

        public double[,] precnutbias00a
            (
            double ttt, double ddpsi, double ddeps,
            EOPSPWLib.iau00Class iau00arr, EOpt opt, double[] fArgs
            )
        {
            double[,] pnb = new double[3, 3];
            double[,] prec = new double[3, 3];
            double[,] nut = new double[3, 3];
            double[,] a1 = new double[3, 3];
            double[,] a2 = new double[3, 3];
            double[,] a3 = new double[3, 3];
            double[,] a4 = new double[3, 3];
            double[,] a5 = new double[3, 3];
            double[,] a6 = new double[3, 3];
            double[,] a7 = new double[3, 3];
            double[,] a8 = new double[3, 3];
            double[,] a9 = new double[3, 3];
            double[,] a10 = new double[3, 3];
            double[,] tr1 = new double[3, 3];
            double[,] tr2 = new double[3, 3];
            double psia, wa, epsa, chia, deltapsi, deltaeps, zeta, theta, z;
            int i;
            double tempval, convrt, ttt2, ttt3, ttt4, ttt5, j2d;
            double pnsum, ensum, pplnsum, eplnsum, oblo;

            deltapsi = deltaeps = 0.0;

            // " to rad
            convrt = Math.PI / (180.0 * 3600.0);

            ttt2 = ttt * ttt;
            ttt3 = ttt2 * ttt;
            ttt4 = ttt2 * ttt2;
            ttt5 = ttt3 * ttt2;

            // ---- obtain data coefficients
            // note the prec matrix is not used in this approach, but the angles are
            prec = precess(ttt, opt, out psia, out wa, out epsa, out chia);


            // iau2006 approach 
            // if (opt.Equals(EOpt.e06eq))
            {
                // looks like they still use the iau2000a method and adjust
                pnsum = 0.0;
                // data file is not reversed
                for (i = 1357; i >= 0; i--)
                {
                    tempval = iau00arr.apn0i[i, 0] * fArgs[0] + iau00arr.apn0i[i, 1] * fArgs[1] + iau00arr.apn0i[i, 2] * fArgs[2]
                        + iau00arr.apn0i[i, 3] * fArgs[3] + iau00arr.apn0i[i, 4] * fArgs[4] + iau00arr.apn0i[i, 5] * fArgs[5]
                        + iau00arr.apn0i[i, 6] * fArgs[6] + iau00arr.apn0i[i, 7] * fArgs[7] + iau00arr.apn0i[i, 8] * fArgs[8]
                        + iau00arr.apn0i[i, 9] * fArgs[9] + iau00arr.apn0i[i, 10] * fArgs[10] + iau00arr.apn0i[i, 11] * fArgs[11] + iau00arr.apn0i[i, 12] * fArgs[12] + iau00arr.apn0i[i, 13] * fArgs[13];
                    if (i > 1319)
                        pnsum = pnsum + (iau00arr.apn0[i, 0] * Math.Sin(tempval) + iau00arr.apn0[i, 1] * Math.Cos(tempval)) * ttt;  //note that Math.Sin and Math.Cos are reveresed between n and e
                    else
                        pnsum = pnsum + iau00arr.apn0[i, 0] * Math.Sin(tempval) + iau00arr.apn0[i, 1] * Math.Cos(tempval);
                }

                ensum = 0.0;
                // data file is not reversed
                for (i = 1055; i >= 0; i--)
                {
                    tempval = iau00arr.apl0i[i, 0] * fArgs[0] + iau00arr.apl0i[i, 1] * fArgs[1] + iau00arr.apl0i[i, 2] * fArgs[2]
                        + iau00arr.apl0i[i, 3] * fArgs[3] + iau00arr.apl0i[i, 4] * fArgs[4] + iau00arr.apl0i[i, 5] * fArgs[5]
                        + iau00arr.apl0i[i, 6] * fArgs[6] + iau00arr.apl0i[i, 7] * fArgs[7] + iau00arr.apl0i[i, 8] * fArgs[8]
                        + iau00arr.apl0i[i, 9] * fArgs[9] + iau00arr.apl0i[i, 10] * fArgs[10] + iau00arr.apl0i[i, 11] * fArgs[11]
                        + iau00arr.apl0i[i, 12] * fArgs[12] + iau00arr.apl0i[i, 13] * fArgs[13];
                    if (i > 1036)
                        ensum = ensum + (iau00arr.apl0[i, 0] * Math.Cos(tempval) + iau00arr.apl0[i, 1] * Math.Sin(tempval)) * ttt;
                    else
                        ensum = ensum + iau00arr.apl0[i, 0] * Math.Cos(tempval) + iau00arr.apl0[i, 1] * Math.Sin(tempval);
                }
                //  add planetary and luni-solar components.
                deltapsi = pnsum;  // rad
                deltaeps = ensum;

                // iau2006 corrections to the iau2000a
                j2d = -2.7774e-6 * ttt;  // rad
                deltapsi = deltapsi + deltapsi * (0.4697e-6 + j2d);  // rad
                deltaeps = deltaeps + deltaeps * j2d;
                if (printopt == 'y')
                {
                    Console.WriteLine("dpsi 1320 " + deltapsi.ToString() + " deps " + deltaeps.ToString());
                    double rad = 180.0 / Math.PI;
                    Console.WriteLine("dpsi 1320 " + (deltapsi * rad).ToString() + " deps " + (deltaeps * rad).ToString());
                }
            }

            // the coefficients have changed from tn32, it seems inconsistent. 
            // use cio series or classical approach
            //  if (opt.Equals(EOpt.e00a))
            {
                pnsum = 0.0;
                ensum = 0.0;
                for (i = 677; i >= 0; i--)
                {
                    tempval = iau00arr.aapn0i[i, 0] * fArgs[0] + iau00arr.aapn0i[i, 1] * fArgs[1] + iau00arr.aapn0i[i, 2] * fArgs[2]
                        + iau00arr.aapn0i[i, 3] * fArgs[3] + iau00arr.aapn0i[i, 4] * fArgs[4];
                    // rad
                    tempval = tempval % (2.0 * Math.PI);
                    //            pnsum = pnsum + (apn0[i,1) + apn0[i,2)*ttt) * Math.Sin(tempval) 
                    //                          + (apn0[i,5) + apn0[i,6)*ttt) * Math.Cos(tempval);
                    //            ensum = ensum + (apn0[i,3) + apn0[i,4)*ttt) * Math.Cos(tempval) 
                    //                          + (apn0[i,7) + apn0[i,8)*ttt) * Math.Sin(tempval);
                    // iers doesn't include the last few terms
                    pnsum = pnsum + (iau00arr.aapn0[i, 0] + iau00arr.aapn0[i, 1] * ttt) * Math.Sin(tempval)
                                      + (iau00arr.aapn0[i, 4]) * Math.Cos(tempval);
                    ensum = ensum + (iau00arr.aapn0[i, 2] + iau00arr.aapn0[i, 3] * ttt) * Math.Cos(tempval)
                                      + (iau00arr.aapn0[i, 6]) * Math.Sin(tempval);
                }

                pplnsum = 0.0;
                eplnsum = 0.0;
                // data file is already reversed, if so, do i++
                for (i = 686; i >= 0; i--)
                {
                    tempval = iau00arr.aapl0i[i, 0] * fArgs[0] + iau00arr.aapl0i[i, 1] * fArgs[1] + iau00arr.aapl0i[i, 2] * fArgs[2]
                        + iau00arr.aapl0i[i, 3] * fArgs[3] + iau00arr.aapl0i[i, 4] * fArgs[4] + iau00arr.aapl0i[i, 5] * fArgs[5]
                        + iau00arr.aapl0i[i, 6] * fArgs[6] + iau00arr.aapl0i[i, 7] * fArgs[7] + iau00arr.aapl0i[i, 8] * fArgs[8]
                        + iau00arr.aapl0i[i, 9] * fArgs[9] + iau00arr.aapl0i[i, 10] * fArgs[10] + iau00arr.aapl0i[i, 11] * fArgs[11]
                        + iau00arr.aapl0i[i, 12] * fArgs[12] + iau00arr.aapl0i[i, 13] * fArgs[13];
                    pplnsum = pplnsum + iau00arr.aapl0[i, 0] * Math.Sin(tempval) + iau00arr.aapl0[i, 1] * Math.Cos(tempval);
                    eplnsum = eplnsum + iau00arr.aapl0[i, 2] * Math.Sin(tempval) + iau00arr.aapl0[i, 3] * Math.Cos(tempval);
                }

                // add planetary and luni-solar components.
                deltapsi = pnsum + pplnsum;  // rad
                deltaeps = ensum + eplnsum;
                if (printopt == 'y')
                {
                    Console.WriteLine("dpsi 678 " + deltapsi.ToString() + " deps " + deltaeps.ToString());
                    double rad = 180.0 / Math.PI;
                    Console.WriteLine("dpsi 678 " + (deltapsi * rad).ToString() + " deps " + (deltaeps * rad).ToString());
                }
            }

            // add in j2d corrections
            deltapsi = deltapsi - deltapsi * (0.4697e-6 * 2.7774e-6);
            deltaeps = deltaeps - deltaeps * 2.7774e-6;

            //zeta = ((((-0.0000003173 * ttt - 0.000005971) * ttt + 0.01801828) * ttt + 0.2988499) * ttt + 2306.083227) * ttt + 2.650545; // "
            //theta = ((((-0.0000001274 * ttt - 0.000007089) * ttt - 0.04182264) * ttt - 0.4294934) * ttt + 2004.191903) * ttt;
            //z = ((((0.0000002904 * ttt - 0.000028596) * ttt + 0.01826837) * ttt + 1.0927348) * ttt + 2306.077181) * ttt - 2.650545;
            // prec 2006 fwm approach
            //gamb
            zeta = ((((0.0000000260 * ttt - 0.000002788) * ttt - 0.00031238) * ttt + 0.4932044) * ttt + 10.556378) * ttt + -0.052928; // "
                                                                                                                                      // phib
            theta = ((((-0.0000000176 * ttt - 0.000000440) * ttt + 0.00053289) * ttt + 0.0511268) * ttt + -46.811016) * ttt + 84381.412819;
            //psib
            z = ((((-0.0000000148 * ttt - 0.000026452) * ttt - 0.00018522) * ttt + 1.5584175) * ttt + 5038.481484) * ttt - 0.041775;

            convrt = Math.PI / (180.0 * 3600.0);  // " to rad
            zeta = zeta * convrt;
            theta = theta * convrt;
            z = z * convrt;

            if (printopt == 'y')
                Console.WriteLine("zeta " + zeta.ToString() + " theta " + theta.ToString() + " z "
                    + z.ToString());

            oblo = 84381.406 * convrt; // " to rad or 448 - 406 for iau2006????

            // ----------------- find nutation matrix ----------------------
            // mean to true
            //a1 = MathTimeLibr.rot1mat(epsa + deltaeps);
            //a2 = MathTimeLibr.rot3mat(deltapsi);
            //a3 = MathTimeLibr.rot1mat(-epsa);

            // j2000 to date(precession - nutation - bias)
            // check signs on this, direction, etc.
            a4 = MathTimeLibr.rot3mat(-zeta);  // gamb
            a5 = MathTimeLibr.rot1mat(-theta);  // 
            a6 = MathTimeLibr.rot3mat(-deltapsi - z);  // psib
            a7 = MathTimeLibr.rot1mat(-deltaeps - epsa);

            // icrs to j2000 (bias)
            //a8 = MathTimeLibr.rot1mat(-0.0068192 * convrt);
            //a9 = MathTimeLibr.rot2mat(0.0417750 * Math.Sin(oblo) * convrt);
            ////      a9  = rot2mat(0.0166170*convrt);
            //a10 = MathTimeLibr.rot3mat(0.0146 * convrt);

            tr1 = MathTimeLibr.matmult(a5, a4, 3, 3, 3);
            tr2 = MathTimeLibr.matmult(a6, tr1, 3, 3, 3);
            //pnb = MathTimeLibr.matmult(a7, tr2, 3, 3, 3);
            pnb = MathTimeLibr.matmult(a7, tr2, 3, 3, 3);

            //tr1 = MathTimeLibr.matmult(a2, a1, 3, 3, 3);
            //nut = MathTimeLibr.matmult(a3, tr1, 3, 3, 3);

            //forget prec by a4567
            // prec = precess(ttt, opt, out psia, out wa, out epsa, out chia);

            //pnb = MathTimeLibr.matmult(prec, nut, 3, 3, 3);
            //tr2 = MathTimeLibr.matmult(a8, tr1, 3, 3, 3);
            //tr1 = MathTimeLibr.matmult(a9, tr2, 3, 3, 3);
            //pnb = MathTimeLibr.matmult(a10, tr1, 3, 3, 3);

            //return MathTimeLibr.matmult(a10, tr1, 3, 3, 3);
            return pnb;
        }   // precnutbias00a



        /* -----------------------------------------------------------------------------
        *
        *                           function nutationqmod
        *
        *  this function calculates the transformation matrix that accounts for the
        *    effects of nutation within the qmod paradigm. There are several assumptions. 
        *
        *  author        : david vallado           davallado@gmail.com   27 jun 2002
        *
        *  revisions
        *    vallado     - consolidate with iau 2000                     14 feb 2005
        *    vallado     - conversion to c++                             21 feb 2005
        *    vallado     - conversion to c#                              16 Nov 2011
        *
        *  inputs          description                                range / units
        *    ttt         - julian centuries of tt
        *    iau80arr    - record containing the iau80 constants rad
        *    opt         - method option                               e00a, e00cio, e96, e80
        *
        *  outputs       :
        *    deltapsi    - nutation in longiotude angle                   rad
        *    trueeps     - true obliquity of the ecliptic                 rad
        *    meaneps     - mean obliquity of the ecliptic                 rad
        *    nut         - transform matrix for tod - mod
        *
        *  locals        :
        *    iar80       - integers for fk5 1980
        *    rar80       - reals for fk5 1980                             rad
         *    fArgs[0]    - delaunay element                                 rad
         *    fArgs[1]    - delaunay element                                 rad
         *    fArgs[2]    - delaunay element                                 rad
         *    fArgs[3]    - delaunay element                                 rad
         *    fArgs[4]    - delaunay element                                 rad
        *    deltaeps    - change in obliquity                            rad
        *
        *  coupling      :
        *    fundarg     - find fundamental arguments
        *
        *  references    :
        *    vallado       2013, 213, 224
        * --------------------------------------------------------------------------- */

        public double[,] nutationqmod
            (
            double ttt,
            EOPSPWLib.iau80Class iau80arr, EOpt opt, double[] fArgs,
            out double deltapsi, out double deltaeps, out double meaneps
            )
        {
            double[,] nut = new double[3, 3];
            double[,] n1 = new double[3, 3];
            double[,] n2 = new double[3, 3];
            double deg2rad, sineps;
            int i;
            double tempval;

            deg2rad = Math.PI / 180.0;

            // ---- determine coefficients for iau 1980 nutation theory ----
            //  assumption for mean eps
            meaneps = 84381.448;
            meaneps = ((meaneps / 3600.0) % 360.0);
            meaneps = meaneps * deg2rad;  // rad

            //  assumption that planetary terms are not used later on
            deltapsi = 0.0;
            deltaeps = 0.0;
            //  assumption that only largest 9 terms are used
            for (i = 8; i >= 0; i--)
            {
                tempval = EOPSPWLibr.iau80arr.iar80[i, 0] * fArgs[0] + EOPSPWLibr.iau80arr.iar80[i, 1] * fArgs[1]
                    + EOPSPWLibr.iau80arr.iar80[i, 2] * fArgs[2] + EOPSPWLibr.iau80arr.iar80[i, 3] * fArgs[3]
                    + EOPSPWLibr.iau80arr.iar80[i, 4] * fArgs[4];
                deltapsi = deltapsi + (EOPSPWLibr.iau80arr.rar80[i, 0]
                    + EOPSPWLibr.iau80arr.rar80[i, 1] * ttt) * Math.Sin(tempval);
                deltaeps = deltaeps + (EOPSPWLibr.iau80arr.rar80[i, 2]
                    + EOPSPWLibr.iau80arr.rar80[i, 3] * ttt) * Math.Cos(tempval);
            }

            // --------------- find nutation parameters --------------------
            //  assumtpion that delta delta corrections not used
            deltapsi = deltapsi % (2.0 * Math.PI);
            deltaeps = deltaeps % (2.0 * Math.PI);

            sineps = Math.Sin(meaneps);

            //  approximation, not using the meaneps
            n1[0, 0] = 1.0; // rot1
            n1[1, 1] = Math.Cos(-deltaeps);
            n1[2, 1] = Math.Sin(-deltaeps);
            n1[1, 2] = -Math.Sin(-deltaeps);
            n1[2, 2] = Math.Cos(-deltaeps);
            n2[1, 1] = 1.0;  // rot2
            n2[0, 0] = Math.Cos(deltapsi * sineps);
            n2[2, 0] = -Math.Sin(deltapsi * sineps);
            n2[0, 2] = Math.Sin(deltapsi * sineps);
            n2[2, 2] = Math.Cos(deltapsi * sineps);
            nut = MathTimeLibr.matmult(n2, n1, 3, 3, 3);

            return nut;
        }  //  nutationqmod 


        /* -----------------------------------------------------------------------------
        *
        *                           function sidereal
        *
        *  this function calculates the transformation matrix that accounts for the
        *    effects of sidereal time. Notice that deltaspi should not be moded to a
        *    positive number because it is multiplied rather than used in a
        *    trigonometric argument.
        *
        *  author        : david vallado           davallado@gmail.com   25 jun 2002
        *
        *  revisions
        *    vallado     - fix units on kinematic terms                   5 sep 2002
        *    vallado     - add terms                                     30 sep 2002
        *    vallado     - consolidate with iau 2000                     14 feb 2005
        *    vallado     - conversion to c++                             21 feb 2005
        *    vallado     - conversion to c#                              16 Nov 2011
        *
        *  inputs          description                                 range / units
        *    jdut1       - julian centuries of ut1                           days
        *    deltapsi    - nutation angle                                    rad
        *    meaneps     - mean obliquity of the ecliptic                    rad
        *    lod         - length of day                                     sec
        *    eqeterms    - terms for ast calculation                         0,2
        *
        *  outputs       :
        *    st          - transformation matrix for pef - tod
        *    stdot       - transformation matrix for pef - tod rate
        *
        *  locals        :
        *    gmst         - mean greenwich sidereal time                 0 to 2pi rad
        *    ast         - apparent gmst                                 0 to 2pi rad
        *    hr          - hour                                              hr
        *    min         - minutes                                           min
        *    sec         - seconds                                           sec
        *    temp        - temporary vector
        *    tempval     - temporary variable
        *
        *  coupling      :
        *
        *  references    :
        *    vallado       2013, 212, 223
        * --------------------------------------------------------------------------- */

        public double[,] sidereal
            (
            double jdut1, double deltapsi, double meaneps, double[] fArgs,
            double lod, int eqeterms, EOpt opt
            )
        {
            double gmst, ast, sinast, cosast;
            double era, tut1d;
            //double omegaearth;
            //double[,] stdot = new double[3, 3];
            double[,] st = new double[3, 3];
            ast = era = 0.0;

            // iau76/fk5 approach
            if ((opt.Equals(EOpt.e80)) | (opt.Equals(EOpt.e96)))
            {
                // ------------------------ find gmst --------------------------
                gmst = gstime(jdut1);

                // ------------------------ find mean ast ----------------------
                if ((jdut1 > 2450449.5) && (eqeterms > 0))
                {
                    ast = gmst + deltapsi * Math.Cos(meaneps) + 0.00264 * Math.PI / (3600.0 * 180.0) * Math.Sin(fArgs[4])
                               + 0.000063 * Math.PI / (3600.0 * 180.0) * Math.Sin(2.0 * fArgs[4]);
                }
                else
                    ast = gmst + deltapsi * Math.Cos(meaneps);

                ast = (ast % (2.0 * Math.PI));
            }
            else  // IAU 2006/2000 approach
            {
                // julian centuries of ut1
                tut1d = jdut1 - 2451545.0;

                era = Math.PI * 2.0 * (0.7790572732640 + 1.00273781191135448 * tut1d);
                era = (era % (2.0 * Math.PI));
                if (printopt == 'y')
                    Console.WriteLine("era " + (era * 180.0 / Math.PI).ToString() + " " + era.ToString());
                ast = era;  // set this for the matrix calcs below
            }

            sinast = Math.Sin(ast);
            cosast = Math.Cos(ast);

            st[0, 0] = cosast;
            st[0, 1] = -sinast;
            st[0, 2] = 0.0;
            st[1, 0] = sinast;
            st[1, 1] = cosast;
            st[1, 2] = 0.0;
            st[2, 0] = 0.0;
            st[2, 1] = 0.0;
            st[2, 2] = 1.0;

            // compute sidereal time rate matrix
            //omegaearth = gravConst.earthrot * (1.0 - lod / 86400.0);

            //stdot[0, 0] = -omegaearth * sinast;
            //stdot[0, 1] = -omegaearth * cosast;
            //stdot[0, 2] = 0.0;
            //stdot[1, 0] = omegaearth * cosast;
            //stdot[1, 1] = -omegaearth * sinast;
            //stdot[1, 2] = 0.0;
            //stdot[2, 0] = 0.0;
            //stdot[2, 1] = 0.0;
            //stdot[2, 2] = 0.0;

            return st;
        }  //  sidereal 


        /* -----------------------------------------------------------------------------
        *
        *                           function polarm
        *
        *  this function calculates the transformation matrix that accounts for polar
        *    motion within the iau-76/fk5, iau-2000a, and iau2006/2000 equinox systems. 
        *
        *  author        : david vallado           davallado@gmail.com   25 jun 2002
        *
        *  revisions
        *    vallado     - conversion to c++                             23 nov 2005
        *    vallado     - conversion to c#                              16 Nov 2011
        *
        *  inputs          description                                 range / units
        *    xp          - polar motion coefficient                         rad
        *    yp          - polar motion coefficient                         rad
        *    ttt         - julian centuries of tt (00 theory only)
        *    opt         - method option                           e80, e96, e00a, e06cio, e06eq
        *
        *  outputs       :
        *    pm          - transformation matrix for itrf - pef
        *
        *  locals        :
        *    convrt      - conversion from arcsec to rad
        *    sp          - s prime value (00 theory only)
        *
        *  coupling      :
        *    none.
        *
        *  references    :
        *    vallado       2013, 212, 223
        * --------------------------------------------------------------------------- */

        public double[,] polarm
            (
            double xp, double yp, double ttt, EOpt opt
            )
        {
            double[,] pm = new double[3, 3];
            double cosxp, cosyp, sinxp, sinyp;
            double cossp, sinsp, sp;

            cosxp = Math.Cos(xp);
            sinxp = Math.Sin(xp);
            cosyp = Math.Cos(yp);
            sinyp = Math.Sin(yp);

            // iau-76/fk5 approach
            if ((opt.Equals(EOpt.e80)) | (opt.Equals(EOpt.e96)))
            {
                pm[0, 0] = cosxp;
                pm[0, 1] = 0.0;
                pm[0, 2] = -sinxp;
                pm[1, 0] = sinxp * sinyp;
                pm[1, 1] = cosyp;
                pm[1, 2] = cosxp * sinyp;
                pm[2, 0] = sinxp * cosyp;
                pm[2, 1] = -sinyp;
                pm[2, 2] = cosxp * cosyp;

                // alternate approach
                //astMathr.rot2mat(xp, a1);
                //astMathr.rot1mat(yp, a2);
                //astMathr.MathTimeLibr.matmult(a2, a1, pm, 3, 3, 3);
            }
            else
            // iau-2006/2000 eq or iau2000a approach
            {
                // approximate sp value in rad
                sp = -47.0e-6 * ttt * Math.PI / (180.0 * 3600.0);
                cossp = Math.Cos(sp);
                sinsp = Math.Sin(sp);

                // form the matrix
                pm[0, 0] = cosxp * cossp;
                pm[0, 1] = -cosyp * sinsp + sinyp * sinxp * cossp;
                pm[0, 2] = -sinyp * sinsp - cosyp * sinxp * cossp;
                pm[1, 0] = cosxp * sinsp;
                pm[1, 1] = cosyp * cossp + sinyp * sinxp * sinsp;
                pm[1, 2] = sinyp * cossp - cosyp * sinxp * sinsp;
                pm[2, 0] = sinxp;
                pm[2, 1] = -sinyp * cosxp;
                pm[2, 2] = cosyp * cosxp;

                // alternate approach
                //astMathr.rot1mat(yp, a1);
                //astMathr.rot2mat(xp, a2);
                //astMathr.rot3mat(-sp, a3);
                //astMathr.MathTimeLibr.matmult(a2, a1, tr1, 3, 3, 3);
                //astMathr.MathTimeLibr.matmult(a3, tr1, pm, 3, 3, 3);
            }

            return pm;
        }  //  polarm 


        /* -----------------------------------------------------------------------------
        *
        *                           function framebias
        *
        *  this function calculates the transformation matrix that accounts for frame
        *    bias.
        *
        *  author        : david vallado           davallado@gmail.com   19 sep 05
        *
        *  revisions
        *
        *  inputs          description                                 range / units
        *    opt         - frame bias method option                 'j' j2000, 'f' fk5
        *
        *  outputs       :
        *    term1       - alpha delta o                                rad
        *    term2       - psi deltab Math.Sin(eps deltao)                   rad
        *    term3       - eps delta b                                  rad
        *    fb          - frame bias matrix                            rad
        *
        *  locals        :
        *    convrt      - conversion from arcsec to rad
        *
        *  coupling      :
        *    none.
        *
        *  references    :
        *    vallado       2013, 217
        * --------------------------------------------------------------------------- */

        public double[,] framebias
            (
            char opt,
            out double term1, out double term2, out double term3
                )
        {
            double[,] fb = new double[3, 3];
            double convrt;
            convrt = Math.PI / (3600.0 * 180.0);
            term1 = term2 = term3 = 0.0;

            // j2000 version referred to iau76/80 theory
            if (opt == 'j')
            {
                term1 = -0.0146 * convrt;
                term2 = -0.016617 * convrt;
                term3 = -0.0068192 * convrt;
                fb[0, 0] = 0.99999999999999;
                fb[0, 1] = 0.00000007078279;
                fb[0, 2] = -0.00000008056149;
                fb[1, 0] = -0.00000007078280;
                fb[1, 1] = 1.0;
                fb[1, 2] = -0.00000003306041;
                fb[2, 0] = 0.00000008056149;
                fb[2, 1] = 0.00000003306041;
                fb[2, 2] = 1.0;
            }

            // fk5 version - catalog origin
            if (opt == 'f')
            {
                term1 = -0.0229 * convrt;
                term2 = 0.0091 * convrt;
                term3 = -0.0199 * convrt;
                fb[0, 0] = 0.99999999999999;
                fb[0, 1] = 0.00000011102234;
                fb[0, 2] = 0.00000004411803;
                fb[1, 0] = -0.00000011102233;
                fb[1, 1] = 0.99999999999999;
                fb[1, 2] = -0.00000009647793;
                fb[2, 0] = -0.00000004411804;
                fb[2, 1] = 0.00000009647792;
                fb[2, 2] = 0.99999999999999;
            }

            return fb;
        }  // framebias



        /* ----------------------------------------------------------------------------
        *
        *                           function eci_ecef
        *
        *  this function transforms between the earth fixed (itrf) frame, and
        *    the eci mean equator mean equinox (gcrf).
        *
        *  author        : david vallado           davallado@gmail.com    4 jun 2002
        *
        *  revisions
        *    vallado     - add terms for ast calculation                 30 sep 2002
        *    vallado     - consolidate with iau 2000                     14 feb 2005
        *
        *  inputs          description                                  range / units
        *    recef       - position vector earth fixed                   km
        *    vecef       - velocity vector earth fixed                   km/s
        *    opt         - method option                           e80, e96, e00a, e06cio, e06eq
        *    iau80arr    - iau76/fk5 eop constants
        *    iau00arr    - iau2006 eop constants
        *    ttt         - julian centuries of tt                        centuries
        *    jdut1       - julian date of ut1                            days from 4713 bc
        *    lod         - excess length of day                          sec
        *    xp          - polar motion coefficient                      rad
        *    yp          - polar motion coefficient                      rad
        *    ddpsi       - delta psi correction to gcrf                  rad
        *    ddeps       - delta eps correction to gcrf                  rad
        *
        *  outputs       :
        *    reci        - position vector eci                           km
        *    veci        - velocity vector eci                           km/s
        *
        *  locals        :
        *    eqeterms    - terms for ast calculation                     0,2
        *    deltapsi    - nutation angle                                rad
        *    trueeps     - true obliquity of the ecliptic                rad
        *    meaneps     - mean obliquity of the ecliptic                rad
        *    prec        - matrix for mod - eci 
        *    nut         - matrix for tod - mod 
        *    st          - matrix for pef - tod 
        *    stdot       - matrix for pef - tod rate
        *    pm          - matrix for ecef - pef 
        *
        *  coupling      :
        *   precess      - rotation for precession       
        *   nutation     - rotation for nutation          
        *   sidereal     - rotation for sidereal time     
        *   polarm       - rotation for polar motion      
        *
        *  references    :
        *    vallado       2013, 223-231
        * ---------------------------------------------------------------------------- */

        public void eci_ecef
            (
            ref double[] reci, ref double[] veci,
            Enum direct,
            ref double[] recef, ref double[] vecef,
            EOpt opt,
            EOPSPWLib.iau80Class iau80arr, EOPSPWLib.iau00Class iau00arr,
            double jdtt, double jdftt, double jdut1, double jdxysstart, double lod,
            double xp, double yp, double ddpsi, double ddeps, double ddx, double ddy
            )
        {
            double[] fArgs = new double[14];
            double psia, wa, epsa, chia, x, y, s, gst, ttt;
            double meaneps, deltapsi, deltaeps, trueeps;
            double[] omegaearth = new double[3];
            double[] rpef = new double[3];
            double[] vpef = new double[3];
            double[] rtod = new double[3];
            double[] vtod = new double[3];
            double[] crossr = new double[3];
            double[] tempvec1 = new double[3];
            double[,] a1 = new double[3, 3];
            double[,] a2 = new double[3, 3];
            double[,] a3 = new double[3, 3];
            double[,] tm = new double[3, 3];
            double[,] pn = new double[3, 3];
            double[,] prec = new double[3, 3];
            double[,] nut = new double[3, 3];
            double[,] st = new double[3, 3];
            double[,] pm = new double[3, 3];
            double[,] precp = new double[3, 3];
            double[,] pnp = new double[3, 3];
            double[,] nutp = new double[3, 3];
            double[,] stp = new double[3, 3];
            double[,] pmp = new double[3, 3];
            double[,] temp = new double[3, 3];
            double[,] trans = new double[3, 3];

            int eqeterms = 2;
            char interp = 's';
            deltapsi = 0.0;
            meaneps = 0.0;

            ttt = (jdtt + jdftt - 2451545.0) / 36525.0;

            fundarg(ttt, opt, out fArgs);

            //Console.WriteLine( "ttt " + ttt.ToString() + " jdut1 " + jdut1.ToString());
            // IAU-76/FK5 approach
            if (opt.Equals(EOpt.e80) || opt.Equals(EOpt.e96))
            {
                prec = precess(ttt, opt, out psia, out wa, out epsa, out chia);
                nut = nutation(ttt, ddpsi, ddeps, iau80arr, opt, fArgs,
                    out deltapsi, out deltaeps, out trueeps, out meaneps);
                st = sidereal(jdut1, deltapsi, meaneps, fArgs, lod, eqeterms, opt);
                //Console.WriteLine( String.Format("psia {0}  {1}  {2} {3} ", psia, wa, epsa, chia));
                //Console.WriteLine( String.Format("dpsi {0}  {1}  {2} {3} {4} ", deltapsi, meaneps, trueeps, 
                //    meaneps, fArgs[4]));
                pn = MathTimeLibr.matmult(prec, nut, 3, 3, 3);
            }
            else
            // IAU-2006/2000 CIO series approach
            if (opt.Equals(EOpt.e06cio))
            {
                prec[0, 0] = 1.0;
                prec[1, 1] = 1.0;
                prec[2, 2] = 1.0;
                nut = iau06xys(jdtt + jdftt, ttt, ddx, ddy, 'x', iau00arr, fArgs, jdxysstart, out x, out y, out s);

                st = sidereal(jdut1, 0.0, 0.0, fArgs, lod, 2, opt);
                if (printopt == 'y')
                    Console.WriteLine(String.Format("xys cl {0}  {1}  {2} ", x, y, s));
            }
            else
            // IAU2006nut or IAU-2000a pna approach  EOpt.e00a
            // this is incorrect, but sofa and iers are so confusing, just use the series approach
            {
                prec = new double[3, 3];
                prec[0, 0] = 1.0;
                prec[1, 1] = 1.0;
                prec[2, 2] = 1.0;
                // only needed for s
                // probably just need the abbreivated form
                nut = iau06xys(jdtt + jdftt, ttt, ddx, ddy, interp, iau00arr, fArgs, jdxysstart, out x, out y, out s);
                //double conv = 180.0 / Math.PI * 3600.0;
                //Console.WriteLine( "xys " + (x * conv).ToString() + " " + (y * conv).ToString() + " "
                //   + (s*conv).ToString());

                // x and y come from the pnb matrix
                // sofa does 687 terms and the adds corretion
                // "probably" should do 1320 terms as in TN-36?
                // this is actually pnb coming back
                nut = precnutbias00a(ttt, ddx, ddy, iau00arr, opt, fArgs);
                if (printopt == 'y')
                {
                    Console.WriteLine(String.Format("xys eq {0}  {1}  {2} ", x, y, s));
                    Console.WriteLine(String.Format("pn = {0}  {1}  {2}", pn[0, 0], pn[0, 1], pn[0, 2]));
                    Console.WriteLine(String.Format("pn = {0}  {1}  {2}", pn[1, 0], pn[1, 1], pn[1, 2]));
                    Console.WriteLine(String.Format("pn = {0}  {1}  {2}", pn[2, 0], pn[2, 1], pn[2, 2]));
                }

                x = pn[2, 0];
                y = pn[1, 2]; // transposed from sofa
                if (printopt == 'y')
                    Console.WriteLine(String.Format("x  {0} y  {1} ", x, y));
                // seems like a lot of math where you could find a and then the matrix direct?
                double e, d;
                if (x != 0.0 && y != 0.0)
                    e = Math.Atan2(y, x);
                else
                    e = 0.0;
                d = Math.Atan(Math.Sqrt((x * x + y * y) / (1.0 - x * x - y * y)));
                if (printopt == 'y')
                    Console.WriteLine(String.Format("e {0} d {1} ", e, fArgs[3]));

                a1 = MathTimeLibr.rot3mat(-e);
                a2 = MathTimeLibr.rot2mat(-fArgs[3]);
                a3 = MathTimeLibr.rot3mat(e + s);
                temp = MathTimeLibr.matmult(a2, a3, 3, 3, 3);
                pn = MathTimeLibr.matmult(a1, temp, 3, 3, 3);

                st = gstime00(jdut1, deltapsi, ttt, iau00arr, opt, fArgs, out gst);
                //sofa seems to use this??
                st = sidereal(jdut1, 0.0, 0.0, fArgs, lod, 2, opt);
                if (printopt == 'y')
                    Console.WriteLine(String.Format("gst  {0} ", gst));
            }

            pm = polarm(xp, yp, ttt, opt);

            omegaearth[0] = 0.0;
            omegaearth[1] = 0.0;
            omegaearth[2] = gravConst.earthrot * (1.0 - lod / 86400.0);

            if (direct.Equals(MathTimeLib.Edirection.eto))
            {
                // ---- perform transformations
                pmp = MathTimeLibr.mattrans(pm, 3);
                stp = MathTimeLibr.mattrans(st, 3);
                nutp = MathTimeLibr.mattrans(nut, 3);
                precp = MathTimeLibr.mattrans(prec, 3);
                //pnp = MathTimeLibr.mattrans(pn, 3);

                temp = MathTimeLibr.matmult(stp, nutp, 3, 3, 3);
                //trans = MathTimeLibr.matmult(stp, pnp, 3, 3, 3);
                trans = MathTimeLibr.matmult(temp, precp, 3, 3, 3);
                rpef = MathTimeLibr.matvecmult(trans, reci, 3);
                recef = MathTimeLibr.matvecmult(pmp, rpef, 3);

                tempvec1 = MathTimeLibr.matvecmult(trans, veci, 3);
                MathTimeLibr.cross(omegaearth, rpef, out crossr);
                vpef[0] = tempvec1[0] - crossr[0];
                vpef[1] = tempvec1[1] - crossr[1];
                vpef[2] = tempvec1[2] - crossr[2];
                vecef = MathTimeLibr.matvecmult(pmp, vpef, 3);
            }
            else
            {
                // ---- perform transformations
                rpef = MathTimeLibr.matvecmult(pm, recef, 3);
                temp = MathTimeLibr.matmult(prec, nut, 3, 3, 3);
                //trans = MathTimeLibr.matmult(pn, st, 3, 3, 3);
                trans = MathTimeLibr.matmult(temp, st, 3, 3, 3);
                reci = MathTimeLibr.matvecmult(trans, rpef, 3);

                vpef = MathTimeLibr.matvecmult(pm, vecef, 3);
                MathTimeLibr.cross(omegaearth, rpef, out crossr);
                tempvec1[0] = vpef[0] + crossr[0];
                tempvec1[1] = vpef[1] + crossr[1];
                tempvec1[2] = vpef[2] + crossr[2];
                veci = MathTimeLibr.matvecmult(trans, tempvec1, 3);

                //Console.WriteLine( String.Format("pm = {0}  {1}  {2}", pm[0, 0], pm[0, 1], pm[0, 2]));
                //Console.WriteLine( String.Format("pm = {0}  {1}  {2}", pm[1, 0], pm[1, 1], pm[1, 2]));
                //Console.WriteLine( String.Format("pm = {0}  {1}  {2}", pm[2, 0], pm[2, 1], pm[2, 2]));
                if (printopt == 'y')
                    Console.WriteLine(String.Format("rpefi = {0}  {1}  {2}  {3}  {4}  {5}", rpef[0], rpef[1],
                    rpef[2], vpef[0], vpef[1], vpef[2]));
                //Console.WriteLine( String.Format("st = {0}  {1}  {2}", st[0, 0], st[0, 1], st[0, 2]));
                //Console.WriteLine( String.Format("st = {0}  {1}  {2}", st[1, 0], st[1, 1], st[1, 2]));
                //Console.WriteLine( String.Format("st = {0}  {1}  {2}", st[2, 0], st[2, 1], st[2, 2]));
                //Console.WriteLine( String.Format("pn = {0}  {1}  {2}", pn[0, 0], pn[0, 1], pn[0, 2]));
                //Console.WriteLine( String.Format("pn = {0}  {1}  {2}", pn[1, 0], pn[1, 1], pn[1, 2]));
                //Console.WriteLine( String.Format("pn = {0}  {1}  {2}", pn[2, 0], pn[2, 1], pn[2, 2]));
                rtod = MathTimeLibr.matvecmult(st, rpef, 3);
                vtod = MathTimeLibr.matvecmult(st, tempvec1, 3);
                if (printopt == 'y')
                    Console.WriteLine(String.Format("rtodi = {0}  {1}  {2}  {3}  {4}  {5}", rtod[0], rtod[1],
                    rtod[2], vtod[0], vtod[1], vtod[2]));
                //Console.WriteLine( String.Format("prec = {0}  {1}  {2}", prec[0, 0], prec[0, 1], prec[0, 2]));
                //Console.WriteLine( String.Format("prec = {0}  {1}  {2}", prec[1, 0], prec[1, 1], prec[1, 2]));
                //Console.WriteLine( String.Format("prec = {0}  {1}  {2}", prec[2, 0], prec[2, 1], prec[2, 2]));
            }
        }//  eci_ecef 

        /* ----------------------------------------------------------------------------
         *
         *                           function eci_pef
         *
         *  this function transforms between the eci mean equator mean equinox (gcrf), and
         *    the pseudo earth fixed frame (pef).
         *
         *  author        : david vallado           davallado@gmail.com    4 jun 2002
         *
         *  revisions
         *    vallado     - add terms for ast calculation                 30 sep 2002
         *    vallado     - consolidate with iau 2000                     14 feb 2005
         *
         *  inputs          description                                  range / units
         *    reci        - position vector eci                           km
         *    veci        - velocity vector eci                           km/s
        *    opt         - method option                           e80, e96, e00a, e06cio, e06eq
         *    iau80arr    - iau76/fk5 eop constants
         *    iau00arr    - iau2006 eop constants
         *    ttt         - julian centuries of tt                        centuries
         *    jdut1       - julian date of ut1                            days from 4713 bc
         *    lod         - excess length of day                          sec
         *    ddpsi       - delta psi correction to gcrf                  rad
         *    ddeps       - delta eps correction to gcrf                  rad
         *    ddx         - delta x correction to gcrf                    rad
         *    ddy         - delta y correction to gcrf                    rad
         *
         *  outputs       :
         *    rpef       - position vector pef                            km
         *    vpef       - velocity vector pef                            km/s
         *
         *  locals        :
         *    eqeterms    - terms for ast calculation                     0,2
         *    deltapsi    - nutation angle                                rad
         *    trueeps     - true obliquity of the ecliptic                rad
         *    meaneps     - mean obliquity of the ecliptic                rad
         *    prec        - matrix for mod - eci 
         *    nut         - matrix for tod - mod 
         *    st          - matrix for pef - tod 
         *    stdot       - matrix for pef - tod rate
         *
         *  coupling      :
         *   precess      - rotation for precession       
         *   nutation     - rotation for nutation          
         *   sidereal     - rotation for sidereal time     
         *
         *  references    :
         *    vallado       2013, 223-231
         * ---------------------------------------------------------------------------- */

        public void eci_pef
        (
        ref double[] reci, ref double[] veci,
        Enum direct,
        ref double[] rpef, ref double[] vpef,
        EOpt opt,
        EOPSPWLib.iau80Class iau80arr, EOPSPWLib.iau00Class iau00arr,
        double jdtt, double jdftt, double jdut1, double jdxysstart,
        double lod, double ddpsi, double ddeps, double ddx, double ddy
        )
        {
            double[] fArgs = new double[14];
            double psia, wa, epsa, chia, x, y, s, gst, ttt;
            double meaneps, deltapsi, deltaeps, trueeps;
            double[] omegaearth = new double[3];
            double[] crossr = new double[3];
            double[] tempvec1 = new double[3];
            double[,] tm = new double[3, 3];
            double[,] prec = new double[3, 3];
            double[,] nut = new double[3, 3];
            double[,] st = new double[3, 3];
            double[,] precp = new double[3, 3];
            double[,] nutp = new double[3, 3];
            double[,] stp = new double[3, 3];
            double[,] temp = new double[3, 3];
            double[,] trans = new double[3, 3];

            int eqeterms = 2;

            deltapsi = 0.0;
            meaneps = 0.0;
            char interp = 's';

            ttt = (jdtt + jdftt - 2451545.0) / 36525.0;

            fundarg(ttt, opt, out fArgs);

            // IAU-76/FK5 approach
            if (opt.Equals(EOpt.e80) || opt.Equals(EOpt.e96))
            {
                prec = precess(ttt, opt, out psia, out wa, out epsa, out chia);
                nut = nutation(ttt, ddpsi, ddeps, iau80arr, opt, fArgs,
                    out deltapsi, out deltaeps, out trueeps, out meaneps);
                st = sidereal(jdut1, deltapsi, meaneps, fArgs, lod, eqeterms, opt);
            }
            else
            // IAU-2006/2000 CIO approach
            if (opt.Equals(EOpt.e06cio))
            {
                prec[0, 0] = 1.0;
                prec[1, 1] = 1.0;
                prec[2, 2] = 1.0;
                nut = iau06xys(jdtt + jdftt, ttt, ddx, ddy, interp, iau00arr, fArgs, jdxysstart, out x, out y, out s);
                st = sidereal(jdut1, deltapsi, meaneps, fArgs, lod, 2, opt);
            }
            else
            // iau2006nut or iau-2006/2000 Nut or iau2000a approach
            // these are wrong, use the series approach
            {
                prec[0, 0] = 1.0;
                prec[1, 1] = 1.0;
                prec[2, 2] = 1.0;
                // prec = precess(ttt, opt, out psia, out wa, out epsa, out chia);
                // this is prec*nut
                nut = precnutbias00a(ttt, ddx, ddy, iau00arr, opt, fArgs);
                st = gstime00(jdut1, deltapsi, ttt, iau00arr, opt, fArgs, out gst);
            }

            omegaearth[0] = 0.0;
            omegaearth[1] = 0.0;
            omegaearth[2] = gravConst.earthrot * (1.0 - lod / 86400.0);

            if (direct.Equals(MathTimeLib.Edirection.eto))
            {
                // ---- perform transformations
                stp = MathTimeLibr.mattrans(st, 3);
                nutp = MathTimeLibr.mattrans(nut, 3);
                precp = MathTimeLibr.mattrans(prec, 3);

                temp = MathTimeLibr.matmult(stp, nutp, 3, 3, 3);
                trans = MathTimeLibr.matmult(temp, precp, 3, 3, 3);
                rpef = MathTimeLibr.matvecmult(trans, reci, 3);

                tempvec1 = MathTimeLibr.matvecmult(trans, veci, 3);
                MathTimeLibr.cross(omegaearth, rpef, out crossr);
                vpef[0] = tempvec1[0] - crossr[0];
                vpef[1] = tempvec1[1] - crossr[1];
                vpef[2] = tempvec1[2] - crossr[2];
            }
            else
            {
                // ---- perform transformations
                temp = MathTimeLibr.matmult(prec, nut, 3, 3, 3);
                trans = MathTimeLibr.matmult(temp, st, 3, 3, 3);
                reci = MathTimeLibr.matvecmult(trans, rpef, 3);

                MathTimeLibr.cross(omegaearth, rpef, out crossr);
                tempvec1[0] = vpef[0] + crossr[0];
                tempvec1[1] = vpef[1] + crossr[1];
                tempvec1[2] = vpef[2] + crossr[2];
                veci = MathTimeLibr.matvecmult(trans, tempvec1, 3);
            }
        }  //  eci_pef 


        /* ----------------------------------------------------------------------------
         *
         *                           function eci_tod
         *
         *  this function transforms between the eci mean equator mean equinox (gcrf), and
         *    the true of date frame (tod).
         *
         *  author        : david vallado           davallado@gmail.com    4 jun 2002
         *
         *  revisions
         *    vallado     - add terms for ast calculation                 30 sep 2002
         *    vallado     - consolidate with iau 2000                     14 feb 2005
         *
         *  inputs          description                                  range / units
         *    reci        - position vector eci                           km
         *    veci        - velocity vector eci                           km/s
        *    opt         - method option                           e80, e96, e00a, e06cio, e06eq
         *    iau80arr    - iau76/fk5 eop constants
         *    iau00arr    - iau2006 eop constants
         *    ttt         - julian centuries of tt                        centuries
         *    jdut1       - julian date of ut1                            days from 4713 bc
         *    lod         - excess length of day                          sec
         *    ddpsi       - delta psi correction to gcrf                  rad
         *    ddeps       - delta eps correction to gcrf                  rad
         *    ddx         - delta x correction to gcrf                    rad
         *    ddy         - delta y correction to gcrf                    rad
         *
         *  outputs       :
         *    rtod       - position vector tod                            km
         *    vtod       - velocity vector tod                            km/s
         *
         *  locals        :
         *    deltapsi    - nutation angle                                rad
         *    trueeps     - true obliquity of the ecliptic                rad
         *    meaneps     - mean obliquity of the ecliptic                rad
         *    prec        - matrix for mod - eci 
         *    nut         - matrix for tod - mod 
         *
         *  coupling      :
         *   precess      - rotation for precession       
         *   nutation     - rotation for nutation          
         *
         *  references    :
         *    vallado       2013, 223-231
         * ---------------------------------------------------------------------------- */

        public void eci_tod
            (
            ref double[] reci, ref double[] veci,
            Enum direct,
            ref double[] rtod, ref double[] vtod,
            EOpt opt,
            EOPSPWLib.iau80Class iau80arr, EOPSPWLib.iau00Class iau00arr,
            double jdtt, double jdftt, double jdut1, double jdxysstart,
            double lod, double ddpsi, double ddeps, double ddx, double ddy
            )
        {
            double[] fArgs = new double[14];
            double psia, wa, epsa, chia, x, y, s, ttt;
            double meaneps, deltapsi, deltaeps, trueeps;
            double[,] tm = new double[3, 3];
            double[,] prec = new double[3, 3];
            double[,] nut = new double[3, 3];
            double[,] precp = new double[3, 3];
            double[,] nutp = new double[3, 3];
            double[,] temp = new double[3, 3];
            double[,] trans = new double[3, 3];

            deltapsi = 0.0;
            meaneps = 0.0;
            char interp = 's';

            ttt = (jdtt + jdftt - 2451545.0) / 36525.0;

            fundarg(ttt, opt, out fArgs);

            // IAU-76/FK5 approach
            if (opt.Equals(EOpt.e80) || opt.Equals(EOpt.e96))
            {
                prec = precess(ttt, opt, out psia, out wa, out epsa, out chia);
                nut = nutation(ttt, ddpsi, ddeps, iau80arr, opt, fArgs,
                    out deltapsi, out deltaeps, out trueeps, out meaneps);
            }
            else
            // IAU-2006/2000 CIO approach
            if (opt.Equals(EOpt.e06cio))
            {
                prec[0, 0] = 1.0;
                prec[1, 1] = 1.0;
                prec[2, 2] = 1.0;
                nut = iau06xys(jdtt + jdftt, ttt, ddx, ddy, interp, iau00arr, fArgs, jdxysstart, out x, out y, out s);
            }
            else
            // iau-2006/2000 Nut or iau2000a approach
            // these are wrong, use series approach
            {
                prec[0, 0] = 1.0;
                prec[1, 1] = 1.0;
                prec[2, 2] = 1.0;
                // prec = precess(ttt, opt, out psia, out wa, out epsa, out chia);
                // this is prec*nut
                nut = precnutbias00a(ttt, ddx, ddy, iau00arr, EOpt.e00a, fArgs);
            }

            if (direct.Equals(MathTimeLib.Edirection.eto))
            {
                // ---- perform transformations
                nutp = MathTimeLibr.mattrans(nut, 3);
                precp = MathTimeLibr.mattrans(prec, 3);

                trans = MathTimeLibr.matmult(nutp, precp, 3, 3, 3);
                rtod = MathTimeLibr.matvecmult(trans, reci, 3);
                vtod = MathTimeLibr.matvecmult(trans, veci, 3);
            }
            else
            {
                // ---- perform transformations
                trans = MathTimeLibr.matmult(prec, nut, 3, 3, 3);
                reci = MathTimeLibr.matvecmult(trans, rtod, 3);
                veci = MathTimeLibr.matvecmult(trans, vtod, 3);
            }
        }  //  eci_tod 

        /* ----------------------------------------------------------------------------
         *
         *                           function eci_mod
         *
         *  this function transforms between the eci mean equator mean equinox (gcrf), and
         *    the mean of date frame (tod).
         *
         *  author        : david vallado           davallado@gmail.com    4 jun 2002
         *
         *  revisions
         *    vallado     - add terms for ast calculation                 30 sep 2002
         *    vallado     - consolidate with iau 2000                     14 feb 2005
         *
         *  inputs          description                                  range / units
         *    reci        - position vector eci                           km
         *    veci        - velocity vector eci                           km/s
        *    opt         - method option                           e80, e96, e00a, e06cio, e06eq
         *    iau80arr    - iau76/fk5 eop constants
         *    iau00arr    - iau2006 eop constants
         *    ttt         - julian centuries of tt                        centuries
         *    jdut1       - julian date of ut1                            days from 4713 bc
         *
         *  outputs       :
         *    rmod       - position vector mod                            km
         *    vmod       - velocity vector mod                            km/s
         *
         *  locals        :
         *    prec        - matrix for mod - eci 
         *
         *  coupling      :
         *   precess      - rotation for precession       
         *
         *  references    :
         *    vallado       2013, 223-231
         * ---------------------------------------------------------------------------- */

        public void eci_mod
            (
            ref double[] reci, ref double[] veci,
            Enum direct,
            ref double[] rmod, ref double[] vmod,
            EOpt opt,
            EOPSPWLib.iau80Class iau80arr, EOPSPWLib.iau00Class iau00arr,
            double ttt
            )
        {
            double psia, wa, epsa, chia;
            double[,] prec = new double[3, 3];
            double[,] precp = new double[3, 3];

            // IAU-76/FK5 approach
            if (opt.Equals(EOpt.e80) || opt.Equals(EOpt.e96))
            {
                prec = precess(ttt, opt, out psia, out wa, out epsa, out chia);
            }
            else
            // IAU-2006/2000 CIO approach
            if (opt.Equals(EOpt.e06cio))
            {
                prec[0, 0] = 1.0;
                prec[1, 1] = 1.0;
                prec[2, 2] = 1.0;
            }
            else
            // iau-2006/2000 Nut or iau2000a approach
            {
                prec[0, 0] = 1.0;
                prec[1, 1] = 1.0;
                prec[2, 2] = 1.0;
                // prec = precess(ttt, opt, out psia, out wa, out epsa, out chia);
                // this is prec*nut
            }

            if (direct.Equals(MathTimeLib.Edirection.eto))
            {
                // ---- perform transformations
                precp = MathTimeLibr.mattrans(prec, 3);

                rmod = MathTimeLibr.matvecmult(precp, reci, 3);
                vmod = MathTimeLibr.matvecmult(precp, veci, 3);
            }
            else
            {
                // ---- perform transformations
                reci = MathTimeLibr.matvecmult(prec, rmod, 3);
                veci = MathTimeLibr.matvecmult(prec, vmod, 3);
            }
        }  //  eci_mod 



        /* ----------------------------------------------------------------------------
        *
        *                           function ecef_mod
        *
        *  this function transforms a vector from earth fixed (itrf) frame to
        *  mean of date (mod). 
        *
        *  author        : david vallado           davallado@gmail.com    4 jun 2002
        *
        *  revisions
        *    vallado     - add terms for ast calculation                 30 sep 2002
        *    vallado     - consolidate with iau 2000                     14 feb 2005
        *
        *  inputs          description                                   range / units
        *    recef       - position vector earth fixed                   km
        *    vecef       - velocity vector earth fixed                   km/s
        *    direct      - direction of transfer                         eto, efrom
        *    opt         - method option                           e80, e96, e00a, e06cio, e06eq
        *    ttt         - julian centuries of tt                        centuries
        *    jdut1       - julian date of ut1                            days from 4713 bc
        *    lod         - excess length of day                          sec
        *    xp          - polar motion coefficient                      rad
        *    yp          - polar motion coefficient                      rad
        *    ddpsi       - delta psi correction to gcrf                  rad
        *    ddeps       - delta eps correction to gcrf                  rad
        *    ddx         - delta x correction to gcrf                    rad
        *    ddy         - delta y correction to gcrf                    rad
        *
        *  outputs       :
        *    rmod        - position vector mod                           km
        *    vmod        - velocity vector mod                           km/s
        *
        *  locals        :
        *    deltapsi    - nutation angle                                rad
        *    trueeps     - true obliquity of the ecliptic                rad
        *    meaneps     - mean obliquity of the ecliptic                rad
        *    nut         - matrix for tod - mod 
        *    st          - matrix for pef - tod 
        *    stdot       - matrix for pef - tod rate
        *    pm          - matrix for ecef - pef 
        *
        *  coupling      :
        *   nutation     - rotation for nutation          
        *   sidereal     - rotation for sidereal time     
        *   polarm       - rotation for polar motion      
        *
        *  references    :
        *    vallado       2013, 228-236
        * ---------------------------------------------------------------------------- */

        public void ecef_mod
            (
            ref double[] recef, ref double[] vecef,
            Enum direct,
            ref double[] rmod, ref double[] vmod,
            EOpt opt,
            EOPSPWLib.iau80Class iau80arr, EOPSPWLib.iau00Class iau00arr,
            double jdtt, double jdftt, double jdut1, double jdxysstart,
            double lod, double xp, double yp, double ddpsi, double ddeps, double ddx, double ddy
            )
        {
            double[] fArgs = new double[14];
            double x, y, s, gst, ttt;
            double meaneps, deltapsi, deltaeps, trueeps;
            double[] omegaearth = new double[3];
            double[] rpef = new double[3];
            double[] vpef = new double[3];
            double[] crossr = new double[3];
            double[] tempvec1 = new double[3];
            double[,] tm = new double[3, 3];
            double[,] nut = new double[3, 3];
            double[,] st = new double[3, 3];
            double[,] pm = new double[3, 3];
            double[,] nutp = new double[3, 3];
            double[,] stp = new double[3, 3];
            double[,] pmp = new double[3, 3];
            double[,] temp = new double[3, 3];
            double[,] trans = new double[3, 3];

            deltapsi = 0.0;
            meaneps = 0.0;
            char interp = 's';

            ttt = (jdtt + jdftt - 2451545.0) / 36525.0;

            fundarg(ttt, opt, out fArgs);

            // IAU-76/FK5 approach
            if (opt.Equals(EOpt.e80) || opt.Equals(EOpt.e96))
            {
                nut = nutation(ttt, ddpsi, ddeps, iau80arr, opt, fArgs,
                    out deltapsi, out deltaeps, out trueeps, out meaneps);
                st = sidereal(jdut1, deltapsi, meaneps, fArgs, lod, 2, opt);
            }
            else
            // IAU-2006/2000 CIO approach
            if (opt.Equals(EOpt.e06cio))
            {
                nut = iau06xys(jdtt + jdftt, ttt, ddx, ddy, interp, iau00arr, fArgs, jdxysstart, out x, out y, out s);
                st = sidereal(jdut1, deltapsi, meaneps, fArgs, lod, 2, opt);
            }
            else
            // iau-2006/2000 Nut or iau2000a approach
            {
                // this is prec*nut
                nut = precnutbias00a(ttt, ddx, ddy, iau00arr, opt, fArgs);
                st = gstime00(jdut1, deltapsi, ttt, iau00arr, opt, fArgs, out gst);
            }

            pm = polarm(xp, yp, ttt, opt);

            omegaearth[0] = 0.0;
            omegaearth[1] = 0.0;
            omegaearth[2] = gravConst.earthrot * (1.0 - lod / 86400.0);

            if (direct.Equals(MathTimeLib.Edirection.efrom))
            {
                // ---- perform transformations
                pmp = MathTimeLibr.mattrans(pm, 3);
                stp = MathTimeLibr.mattrans(st, 3);
                nutp = MathTimeLibr.mattrans(nut, 3);

                trans = MathTimeLibr.matmult(stp, nutp, 3, 3, 3);
                rpef = MathTimeLibr.matvecmult(trans, rmod, 3);
                recef = MathTimeLibr.matvecmult(pmp, rpef, 3);

                tempvec1 = MathTimeLibr.matvecmult(trans, vmod, 3);
                MathTimeLibr.cross(omegaearth, rpef, out crossr);
                vpef[0] = tempvec1[0] - crossr[0];
                vpef[1] = tempvec1[1] - crossr[1];
                vpef[2] = tempvec1[2] - crossr[2];
                vecef = MathTimeLibr.matvecmult(pmp, vpef, 3);
            }
            else
            {
                // ---- perform transformations
                rpef = MathTimeLibr.matvecmult(pm, recef, 3);
                temp = MathTimeLibr.matmult(nut, st, 3, 3, 3);
                rmod = MathTimeLibr.matvecmult(temp, rpef, 3);

                vpef = MathTimeLibr.matvecmult(pm, vecef, 3);
                MathTimeLibr.cross(omegaearth, rpef, out crossr);
                tempvec1[0] = vpef[0] + crossr[0];
                tempvec1[1] = vpef[1] + crossr[1];
                tempvec1[2] = vpef[2] + crossr[2];
                vmod = MathTimeLibr.matvecmult(temp, tempvec1, 3);
            }
        }  //  ecef_mod


        /* ----------------------------------------------------------------------------
        *
        *                           function ecef_tod
        *
        *  this function transforms a vector from earth fixed (itrf) frame to
        *  true of date (tod). 
        *
        *  author        : david vallado           davallado@gmail.com    4 jun 2002
        *
        *  revisions
        *    vallado     - add terms for ast calculation                 30 sep 2002
        *    vallado     - consolidate with iau 2000                     14 feb 2005
        *
        *  inputs          description                                   range / units
        *    recef       - position vector earth fixed                   km
        *    vecef       - velocity vector earth fixed                   km/s
        *    direct      - direction of transfer                         eto, efrom
        *    opt         - method option                           e80, e96, e00a, e06cio, e06eq
        *    ttt         - julian centuries of tt                        centuries
        *    jdut1       - julian date of ut1                            days from 4713 bc
        *    lod         - excess length of day                          sec
        *    xp          - polar motion coefficient                      rad
        *    yp          - polar motion coefficient                      rad
        *    ddpsi       - delta psi correction to gcrf                  rad
        *    ddeps       - delta eps correction to gcrf                  rad
        *    ddx         - delta x correction to gcrf                    rad
        *    ddy         - delta y correction to gcrf                    rad
        *
        *  outputs       :
        *    rtod        - position vector eci                           km
        *    vtod        - velocity vector eci                           km/s
        *
        *  locals        :
        *    deltapsi    - nutation angle                                rad
        *    trueeps     - true obliquity of the ecliptic                rad
        *    meaneps     - mean obliquity of the ecliptic                rad
        *    nut         - matrix for tod - mod 
        *    st          - matrix for pef - tod 
        *    stdot       - matrix for pef - tod rate
        *    pm          - matrix for ecef - pef 
        *
        *  coupling      :
        *   nutation     - rotation for nutation          
        *   sidereal     - rotation for sidereal time     
        *   polarm       - rotation for polar motion      
        *
        *  references    :
        *    vallado       2013, 228-236
        * ---------------------------------------------------------------------------- */

        public void ecef_tod
            (
            ref double[] recef, ref double[] vecef,
            Enum direct,
            ref double[] rtod, ref double[] vtod,
            EOpt opt,
            EOPSPWLib.iau80Class iau80arr, EOPSPWLib.iau00Class iau00arr,
            double ttt, double jdut1,
            double lod, double xp, double yp, double ddpsi, double ddeps, double ddx, double ddy
            )
        {
            double[] fArgs = new double[14];
            double gst;
            double deltaeps, trueeps, meaneps, deltapsi;
            double[] omegaearth = new double[3];
            double[] rpef = new double[3];
            double[] vpef = new double[3];
            double[] crossr = new double[3];
            double[] tempvec1 = new double[3];
            double[,] tm = new double[3, 3];
            double[,] nut = new double[3, 3];
            double[,] st = new double[3, 3];
            double[,] pm = new double[3, 3];
            double[,] stp = new double[3, 3];
            double[,] pmp = new double[3, 3];

            deltapsi = 0.0;
            meaneps = 0.0;

            fundarg(ttt, opt, out fArgs);

            // IAU-76/FK5 approach
            if (opt.Equals(EOpt.e80) || opt.Equals(EOpt.e96))
            {
                nut = nutation(ttt, ddpsi, ddeps, iau80arr, opt, fArgs,
                    out deltapsi, out deltaeps, out trueeps, out meaneps);
                st = sidereal(jdut1, deltapsi, meaneps, fArgs, lod, 2, opt);
            }
            else
            // IAU-2006/2000 CIO approach
            if (opt.Equals(EOpt.e06cio))
            {
                st = sidereal(jdut1, 0.0, 0.0, fArgs, lod, 2, opt);
            }
            else
            // iau-2006/2000 Nut or iau2000apna approach
            {
                st = gstime00(jdut1, deltapsi, ttt, iau00arr, opt, fArgs, out gst);
            }

            pm = polarm(xp, yp, ttt, opt);

            omegaearth[0] = 0.0;
            omegaearth[1] = 0.0;
            omegaearth[2] = gravConst.earthrot * (1.0 - lod / 86400.0);

            if (direct.Equals(MathTimeLib.Edirection.efrom))
            {
                // ---- perform transformations
                pmp = MathTimeLibr.mattrans(pm, 3);
                stp = MathTimeLibr.mattrans(st, 3);

                rpef = MathTimeLibr.matvecmult(stp, rtod, 3);
                recef = MathTimeLibr.matvecmult(pmp, rpef, 3);

                tempvec1 = MathTimeLibr.matvecmult(stp, vtod, 3);
                MathTimeLibr.cross(omegaearth, rpef, out crossr);
                vpef[0] = tempvec1[0] - crossr[0];
                vpef[1] = tempvec1[1] - crossr[1];
                vpef[2] = tempvec1[2] - crossr[2];
                vecef = MathTimeLibr.matvecmult(pmp, vpef, 3);
            }
            else
            {
                // ---- perform transformations
                rpef = MathTimeLibr.matvecmult(pm, recef, 3);
                rtod = MathTimeLibr.matvecmult(st, rpef, 3);

                vpef = MathTimeLibr.matvecmult(pm, vecef, 3);
                MathTimeLibr.cross(omegaearth, rpef, out crossr);
                tempvec1[0] = vpef[0] + crossr[0];
                tempvec1[1] = vpef[1] + crossr[1];
                tempvec1[2] = vpef[2] + crossr[2];
                vtod = MathTimeLibr.matvecmult(st, tempvec1, 3);
            }
        }  //  ecef_tod


        /* ----------------------------------------------------------------------------
        *
        *                           function ecef_pef
        *
        *  this function transforms a vector from earth fixed (itrf) frame to
        *  pseudo earth fixed (pef). 
        *
        *  author        : david vallado           davallado@gmail.com    4 jun 2002
        *
        *  revisions
        *    vallado     - add terms for ast calculation                 30 sep 2002
        *    vallado     - consolidate with iau 2000                     14 feb 2005
        *
        *  inputs          description                                   range / units
        *    recef       - position vector earth fixed                   km
        *    vecef       - velocity vector earth fixed                   km/s
        *    direct      - direction of transfer                         eto, efrom
        *    opt         - method option                           e80, e96, e00a, e06cio, e06eq
        *    ttt         - julian centuries of tt                        centuries
        *    lod         - excess length of day                          sec
        *    xp          - polar motion coefficient                      rad
        *    yp          - polar motion coefficient                      rad
        *
        *  outputs       :
        *    rpef        - position vector pef                           km
        *    vpef        - velocity vector pef                           km/s
        *
        *  locals        :
        *    deltapsi    - nutation angle                                rad
        *    trueeps     - true obliquity of the ecliptic                rad
        *    meaneps     - mean obliquity of the ecliptic                rad
        *    nut         - matrix for tod - mod 
        *    st          - matrix for pef - tod 
        *    stdot       - matrix for pef - tod rate
        *    pm          - matrix for ecef - pef 
        *
        *  coupling      :
        *   polarm       - rotation for polar motion      
        *
        *  references    :
        *    vallado       2013, 228-236
        * ---------------------------------------------------------------------------- */

        public void ecef_pef
            (
            ref double[] recef, ref double[] vecef,
            Enum direct,
            ref double[] rpef, ref double[] vpef,
            EOpt opt,
            double ttt, double xp, double yp
            )
        {
            double[,] tm = new double[3, 3];
            double[,] pm = new double[3, 3];
            double[,] pmp = new double[3, 3];

            pm = polarm(xp, yp, ttt, opt);

            if (direct.Equals(MathTimeLib.Edirection.efrom))
            {
                // ---- perform transformations
                pmp = MathTimeLibr.mattrans(pm, 3);

                recef = MathTimeLibr.matvecmult(pmp, rpef, 3);
                vecef = MathTimeLibr.matvecmult(pmp, vpef, 3);
            }
            else
            {
                // ---- perform transformations
                rpef = MathTimeLibr.matvecmult(pm, recef, 3);
                vpef = MathTimeLibr.matvecmult(pm, vecef, 3);
            }
        }  //  ecef_pef


        /* ----------------------------------------------------------------------------
        *
        *                           function teme_ecef
        *
        *  this function transforms a vector between the earth fixed(ITRF) frame and the
        *  true equator mean equniox frame(teme).the results take into account
        *    the effects of sidereal time, and polar motion.
        *
        *  author        : david vallado                  719 - 573 - 2600   30 oct 2017
        *
        *  revisions
        *
        *  inputs          description                                   range / units
        *    rteme        - position vector teme                           km
        *    vteme        - velocity vector teme                           km / s
        *    ateme        - acceleration vector teme                       km / s2
        *    direct       - direction of transfer                          eFrom, 'TOO '
        *    ttt          - julian centuries of tt                         centuries
        *    jdut1        - julian date of ut1                             days from 4713 bc
        *    lod          - excess length of day                           sec
        *    xp           - polar motion coefficient                       arc sec
        *    yp           - polar motion coefficient                       arc sec
        *    eqeterms     - use extra two terms(kinematic) after 1997      0, 2
        *    opt         - method option                           e80, e96, e00a, e06cio, e06eq
        *
        *  outputs       :
        *    recef        - position vector earth fixed                    km
        *    vecef        - velocity vector earth fixed                    km / s
        *    aecef        - acceleration vector earth fixed                km / s2
        *
        *  locals :
        *    st           - matrix for pef - tod
        *    pm           - matrix for ecef - pef
        *
        *  coupling :
        *   gstime        - greenwich mean sidereal time                   rad
        *   polarm        - rotation for polar motion                      pef - ecef
        *
        *  references :
        *    vallado       2013, 231 - 233
        * ----------------------------------------------------------------------------*/

        public void teme_ecef
            (
             ref double[] rteme, ref double[] vteme, Enum direct, double ttt, double jdut1, double lod,
             double xp, double yp, Int32 eqeterms, EOpt opt,
             ref double[] recef, ref double[] vecef
            )
        {
            double deg2rad, gmstg, thetasa, raan;
            double[] omegaearth = new double[3];
            double[,] st = new double[3, 3];
            double[,] stdot = new double[3, 3];
            double[,] pm = new double[3, 3];
            double[] rpef = new double[3];
            double[] vpef = new double[3];
            double[] crossr = new double[3];
            double[,] stp = new double[3, 3];
            double[,] pmp = new double[3, 3];
            double gmst, conv;

            deg2rad = Math.PI / 180.0;
            conv = Math.PI / (3600.0 * 180.0);

            // find fArgs[4] from nutation theory
            raan = 125.04452222 + (-6962890.5390 * ttt + 7.455 * ttt * ttt + 0.008 * ttt * ttt * ttt) / 3600.0;
            raan = (raan % 360.0) * deg2rad;

            // ------------------------find gmst--------------------------
            gmst = gstime(jdut1);

            // teme does not include the geometric terms here
            // after 1997, kinematic terms apply
            if ((jdut1 > 2450449.5) && (eqeterms > 0))
            {
                gmstg = gmst
                    + 0.00264 * conv * Math.Sin(raan)
                    + 0.000063 * conv * Math.Sin(2.0 * raan);
            }
            else
                gmstg = gmst;
            gmstg = gmstg % (2.0 * Math.PI);

            thetasa = gravConst.earthrot * (1.0 - lod / 86400.0);
            omegaearth[0] = 0.0;
            omegaearth[1] = 0.0;
            omegaearth[2] = thetasa;

            st[0, 0] = Math.Cos(gmstg);
            st[0, 1] = -Math.Sin(gmstg);
            st[0, 2] = 0.0;
            st[1, 0] = Math.Sin(gmstg);
            st[1, 1] = Math.Cos(gmstg);
            st[1, 2] = 0.0;
            st[2, 0] = 0.0;
            st[2, 1] = 0.0;
            st[2, 2] = 1.0;

            pm = polarm(xp, yp, ttt, opt);

            if (direct.Equals(MathTimeLib.Edirection.eto))
            {
                stp = MathTimeLibr.mattrans(st, 3);
                pmp = MathTimeLibr.mattrans(pm, 3);

                rpef = MathTimeLibr.matvecmult(stp, rteme, 3);
                recef = MathTimeLibr.matvecmult(pmp, rpef, 3);

                vpef = MathTimeLibr.matvecmult(stp, vteme, 3);
                MathTimeLibr.cross(omegaearth, rpef, out crossr);
                vpef[0] = vpef[0] - crossr[0];
                vpef[1] = vpef[1] - crossr[1];
                vpef[2] = vpef[2] - crossr[2];
                vecef = MathTimeLibr.matvecmult(pmp, vpef, 3);

                // for accel
                //addvec(1.0, tempvec1, -1.0, omgxr, vpef);
                //MathTimeLibr.cross(omegaearth, vpef, omgxv);
                //MathTimeLibr.cross(omegaearth, omgxr, omgxomgxr);
                //MathTimeLibr.matvecmult(stp, ateme, tempvec1);
                //addvec(1.0, tempvec1, -1.0, omgxomgxr, tempvec);
                //addvec(1.0, tempvec, -2.0, omgxv, apef);
                //MathTimeLibr.matvecmult(pmp, apef, aecef);
                //fprintf(1, 'st gmst %11.8f ast %11.8f ome  %11.8f \n', gmst * 180 / pi, ast * 180 / pi, omegaearth * 180 / pi);
            }
            else
            {
                rpef = MathTimeLibr.matvecmult(pm, recef, 3);
                rteme = MathTimeLibr.matvecmult(st, rpef, 3);

                vpef = MathTimeLibr.matvecmult(pm, vecef, 3);
                MathTimeLibr.cross(omegaearth, rpef, out crossr);
                vpef[0] = vpef[0] + crossr[0];
                vpef[1] = vpef[1] + crossr[1];
                vpef[2] = vpef[2] + crossr[2];
                vteme = MathTimeLibr.matvecmult(st, vpef, 3);

                //MathTimeLibr.matvecmult(pm, aecef, apef);
                //MathTimeLibr.cross(omegaearth, omgxr, omgxomgxr);
                //MathTimeLibr.cross(omegaearth, vpef, omgxv);
                //addvec(1.0, apef, 1.0, omgxomgxr, tempvec);
                //addvec(1.0, tempvec, 2.0, omgxv, tempvec1);
                //MathTimeLibr.matvecmult(st, tempvec1, ateme);
            }
        }  // teme_ecef


        /* ----------------------------------------------------------------------------
        *
        *                           function teme_eci
        *
        *  this function transforms a vector from the true equator mean equinox system,
        *  (teme) to the mean equator mean equinox (j2000) system.
        *
        *  author        : david vallado                  719 - 573 - 2600   30 oct 2017
        *
        *  inputs        description                                   range / units
        *    rteme       - position vector of date
        *                  true equator, mean equinox                     km
        *    vteme       - velocity vector of date
        *                  true equator, mean equinox                     km / s
        *    ateme       - acceleration vector of date
        *                  true equator, mean equinox                     km / s2
        *    ttt         - julian centuries of tt                         centuries
        *    ddpsi       - delta psi correction to gcrf                   rad
        *    ddeps       - delta eps correction to gcrf                   rad
        *    opt         - method option                           e80, e96, e00a, e06cio, e06eq
        *
        *  outputs       :
        *    reci        - position vector eci                            km
        *    veci        - velocity vector eci                            km / s
        *    aeci        - acceleration vector eci                        km / s2
        *
        *  locals :
        *    prec        - matrix for eci - mod
        *    nutteme     - matrix for mod - teme - an approximation for nutation
        *    eqeg        - rotation for equation of equinoxes(geometric terms only)
        *    tm          - combined matrix for teme2eci
        *
        *  coupling      :
        *   precess      - rotation for precession                        eci - mod
        *   nutation     - rotation for nutation                          eci - tod
        *
        *  references :
        *    vallado       2013, 231 - 233
        * ----------------------------------------------------------------------------*/

        public void teme_eci
            (
            ref double[] rteme, ref double[] vteme, EOPSPWLib.iau80Class iau80arr, Enum direct,
            double ttt, double ddpsi, double ddeps, EOpt opt,
            ref double[] rgcrf, ref double[] vgcrf
            )
        {
            double[] fArgs = new double[14];
            double[,] prec = new double[3, 3];
            double[,] nut = new double[3, 3];
            double[,] precp = new double[3, 3];
            double[,] nutp = new double[3, 3];
            double[,] eqe = new double[3, 3];
            double[,] eqep = new double[3, 3];
            double[,] temp = new double[3, 3];
            double[,] tempmat = new double[3, 3];
            double psia, wa, epsa, chia, deltapsi, deltaeps, trueeps, meaneps, eqeg;

            fundarg(ttt, opt, out fArgs);

            prec = precess(ttt, EOpt.e80, out psia, out wa, out epsa, out chia);
            nut = nutation(ttt, ddpsi, ddeps, iau80arr, opt, fArgs, out deltapsi, out deltaeps, out trueeps, out meaneps);

            // ------------------------find eqeg----------------------
            // rotate teme through just geometric terms
            eqeg = deltapsi * Math.Cos(meaneps);
            eqeg = eqeg % (2.0 * Math.PI);

            eqe[0, 0] = Math.Cos(eqeg);
            eqe[0, 1] = Math.Sin(eqeg);
            eqe[0, 2] = 0.0;
            eqe[1, 0] = -Math.Sin(eqeg);
            eqe[1, 1] = Math.Cos(eqeg);
            eqe[1, 2] = 0.0;
            eqe[2, 0] = 0.0;
            eqe[2, 1] = 0.0;
            eqe[2, 2] = 1.0;

            if (direct.Equals(MathTimeLib.Edirection.eto))
            {
                eqep = MathTimeLibr.mattrans(eqe, 3);

                temp = MathTimeLibr.matmult(nut, eqep, 3, 3, 3);
                tempmat = MathTimeLibr.matmult(prec, temp, 3, 3, 3);

                rgcrf = MathTimeLibr.matvecmult(tempmat, rteme, 3);
                vgcrf = MathTimeLibr.matvecmult(tempmat, vteme, 3);
            }
            else
            {
                nutp = MathTimeLibr.mattrans(nut, 3);
                precp = MathTimeLibr.mattrans(prec, 3);

                temp = MathTimeLibr.matmult(nutp, precp, 3, 3, 3);
                tempmat = MathTimeLibr.matmult(eqe, temp, 3, 3, 3);

                rteme = MathTimeLibr.matvecmult(tempmat, rgcrf, 3);
                vteme = MathTimeLibr.matvecmult(tempmat, vgcrf, 3);
            }
        }  //  teme_eci


        /* ----------------------------------------------------------------------------
        *
        *                           function qmod2ecef
        *
        *  this function trsnforms a vector from the mean equator mean equniox frame
        *    (qmod), to an earth fixed (ITRF) frame.  the results take into account
        *    the effects of precession, nutation, sidereal time, and polar motion.
        *
        *  author        : david vallado           davallado@gmail.com   27 jun 2002
        *
        *  inputs          description                                   range / units
        *    rqmod       - position vector qmod                          km
        *    vqmod       - velocity vector qmod                          km/s
        *    ttt         - julian centuries of tt                        centuries
        *    jdutc       - julian date of utc                            days from 4713 bc
        *
        *  outputs       :
        *    recef       - position vector earth fixed                   km
        *    vecef       - velocity vector earth fixed                   km/s
        *
        *  locals        :
        *    deltapsi    - nutation angle                                rad
        *    meaneps     - mean obliquity of the ecliptic                rad
        *    nut         - matrix for tod - mod 
        *    st          - matrix for pef - tod 
        *
        *  coupling      :
        *   nutationqmod - rotation for nutation (qmod)                   qmod - tod
        *
        *  references    :
        *    vallado       2007, 239-248
        * ---------------------------------------------------------------------------- */

        public void qmod2ecef
            (
            double[] rqmod, double[] vqmod, double ttt, double jdutc,
            EOPSPWLib.iau80Class iau80arr, EOpt opt,
            out double[] recef, out double[] vecef
            )
        {
            double[] fArgs = new double[14];
            double gmst, ed, deg2rad;
            double meaneps, deltapsi, deltaeps;
            double[,] nut = new double[3, 3];
            double[,] st = new double[3, 3];
            double[,] nutp = new double[3, 3];
            double[,] stp = new double[3, 3];
            double[,] tm = new double[3, 3];
            double[] crossr = new double[3];
            double[] omegaearth = new double[3];
            omegaearth[2] = gravConst.earthrot;

            fundarg(ttt, opt, out fArgs);

            nut = nutationqmod(ttt, iau80arr, opt, fArgs, out deltapsi, out deltaeps, out meaneps);

            //  assumption that utc = ut1 and therefore a new gmst
            ed = jdutc - 2451544.5;  // elapsed days from 1 jan 2000 0 hr 
            gmst = 99.96779469 + 360.9856473662860 * ed + 0.29079e-12 * ed * ed;  // deg
            deg2rad = Math.PI / 180.0;
            gmst = (gmst * deg2rad) % (2.0 * Math.PI);

            st[0, 0] = Math.Cos(gmst);
            st[0, 1] = -Math.Sin(gmst);
            st[0, 2] = 0.0;
            st[1, 0] = Math.Sin(gmst);
            st[1, 1] = Math.Cos(gmst);
            st[1, 2] = 0.0;
            st[2, 0] = 0.0;
            st[2, 1] = 0.0;
            st[2, 2] = 1.0;

            // ---- perform transformations
            stp = MathTimeLibr.mattrans(st, 3);
            nutp = MathTimeLibr.mattrans(nut, 3);

            tm = MathTimeLibr.matmult(stp, nutp, 3, 3, 3);
            recef = MathTimeLibr.matvecmult(tm, rqmod, 3);
            vecef = MathTimeLibr.matvecmult(tm, vqmod, 3);

            MathTimeLibr.cross(omegaearth, recef, out crossr);
            vecef[0] = vecef[0] - crossr[0];
            vecef[1] = vecef[1] - crossr[1];
            vecef[2] = vecef[2] - crossr[2];
        }  //  qmod2ecef 


        /* ----------------------------------------------------------------------------
        *
        *                           function csm2efg
        *
        *  this function transforms an efg (pef) state vector and ric (eci) vector of
        *  another satellite and finds both states in ecef. note that afspc calls pef efg.
        *  there is about a 5cm and 5cm/s error with the afspc values - still tracking that down. 
        *
        *  author        : david vallado           davallado@gmail.com   18 nov 2011
        *
        *  inputs          description                                   range / units
        *    r1pef       - pos vector pseudo earth fixed    km
        *    v1pef       - vel vector pseude earth fixed    km/s
        *    r2ric       - rel pos vector eci               km
        *    v2ric       - rel vel vector eci               km/s
        *    ttt         - julian centuries of tt           centuries
        *    jdut1       - julian date of ut1               days from 4713 bc
        *    lod         - excess length of day             sec
        *    xp          - polar motion coefficient         rad
        *    yp          - polar motion coefficient         rad
        *    eqeterms    - terms for ast calculation        0,2
        *    ddpsi       - delta psi correction to gcrf     rad
        *    ddeps       - delta eps correction to gcrf     rad
        *    opt         - method option                           e80, e96, e00a, e06cio, e06eq
        *
        *  outputs       :
        *    r1ecef      - position vector earth fixed      km
        *    v1ecef      - velocity vector earth fixed      km/s
        *    r2ecef      - position vector earth fixed      km
        *    v2ecef      - velocity vector earth fixed      km/s
        *
        *  locals        :
        *    reci        - position vector eci              km
        *    veci        - velocity vector eci              km/s
        *    deltapsi    - nutation angle                   rad
        *    trueeps     - true obliquity of the ecliptic   rad
        *    meaneps     - mean obliquity of the ecliptic   rad
        *    prec        - matrix for mod - eci 
        *    nut         - matrix for tod - mod 
        *    st          - matrix for pef - tod 
        *    stdot       - matrix for pef - tod rate
        *    pm          - matrix for ecef - pef 
        *
        *  coupling      :
        *   precess      - rotation for precession       
        *   nutation     - rotation for nutation          
        *   sidereal     - rotation for sidereal time     
        *   polarm       - rotation for polar motion      
        *
        *  references    :
        *    vallado       2007, 228-236
        * ---------------------------------------------------------------------------- */

        public void csm2efg
            (
            double[] r1pef, double[] v1pef, double[] r2ric, double[] v2ric,
            double ttt, double jdut1, double lod, double xp, double yp, int eqeterms, double ddpsi, double ddeps,
            EOpt opt,
            out double[] r1ecef, out double[] v1ecef, out double[] r2ecef, out double[] v2ecef
            )
        {
            double[] fArgs = new double[14];
            double psia, wa, epsa, chia;
            double meaneps, deltapsi, deltaeps, trueeps;
            double[] omegaearth = new double[3];
            double[] crossr = new double[3];
            double[] tempvec1 = new double[3];
            double[] rrsw = new double[3];
            double[] vrsw = new double[3];
            double[] r1eci = new double[3];
            double[] v1eci = new double[3];
            double[] r2eci = new double[3];
            double[] v2eci = new double[3];
            double[] r2pef = new double[3];
            double[] v2pef = new double[3];
            double[] r2rict = new double[3];
            double[] v2rict = new double[3];
            double[,] tm = new double[3, 3];
            double[,] prec = new double[3, 3];
            double[,] nut = new double[3, 3];
            double[,] st = new double[3, 3];
            double[,] pm = new double[3, 3];
            double[,] precp = new double[3, 3];
            double[,] nutp = new double[3, 3];
            double[,] stp = new double[3, 3];
            double[,] pmp = new double[3, 3];
            double[,] temp = new double[3, 3];
            double[,] trans = new double[3, 3];
            double[,] rot2rsw = new double[3, 3];
            double[,] rot2rswt = new double[3, 3];

            omegaearth[0] = 0.0;
            omegaearth[1] = 0.0;
            omegaearth[2] = gravConst.earthrot * (1.0 - lod / 86400.0);

            fundarg(ttt, opt, out fArgs);

            prec = precess(ttt, opt, out psia, out wa, out epsa, out chia);

            nut = nutation(ttt, ddpsi, ddeps, EOPSPWLibr.iau80arr, opt, fArgs, out deltapsi, out deltaeps, out trueeps, out meaneps);

            st = sidereal(jdut1, deltapsi, meaneps, fArgs, lod, eqeterms, opt);

            pm = polarm(xp, yp, ttt, opt);

            // ---- perform transformations moving pef to eci
            temp = MathTimeLibr.matmult(prec, nut, 3, 3, 3);
            trans = MathTimeLibr.matmult(temp, st, 3, 3, 3);
            r1eci = MathTimeLibr.matvecmult(trans, r1pef, 3);

            MathTimeLibr.cross(omegaearth, r1pef, out crossr);
            tempvec1[0] = v1pef[0] + crossr[0];
            tempvec1[1] = v1pef[1] + crossr[1];
            tempvec1[2] = v1pef[2] + crossr[2];
            v1eci = MathTimeLibr.matvecmult(trans, tempvec1, 3);
            if (printopt == 'y')
                 Console.WriteLine(String.Format("r1eci = {0}  {1}  {2}  {3}  {4}  {5}", r1eci[0], r1eci[1], r1eci[2], v1eci[0], v1eci[1], v1eci[2]));

            rot2rsw = rv2rsw(r1eci, v1eci, out rrsw, out vrsw);
            rot2rswt = MathTimeLibr.mattrans(rot2rsw, 3);
            r2rict = MathTimeLibr.matvecmult(rot2rswt, r2ric, 3);
            v2rict = MathTimeLibr.matvecmult(rot2rswt, v2ric, 3);

            r2eci[0] = r1eci[0] + r2rict[0];
            r2eci[1] = r1eci[1] + r2rict[1];
            r2eci[2] = r1eci[2] + r2rict[2];
            v2eci[0] = v1eci[0] + v2rict[0];
            v2eci[1] = v1eci[1] + v2rict[1];
            v2eci[2] = v1eci[2] + v2rict[2];
            if (printopt == 'y')
            {
                Console.WriteLine(String.Format("r2eci = {0}  {1}  {2}  {3}  {4}  {5}", r2eci[0], r2eci[1], r2eci[2], v2eci[0], v2eci[1], v2eci[2]));
                Console.WriteLine(String.Format("r2ric = {0}  {1}  {2}  {3}  {4}  {5}", r2ric[0], r2ric[1], r2ric[2], v2ric[0], v2ric[1], v2ric[2]));
            }

            // Convert 2 back to efg
            pmp = MathTimeLibr.mattrans(pm, 3);
            stp = MathTimeLibr.mattrans(st, 3);
            nutp = MathTimeLibr.mattrans(nut, 3);
            precp = MathTimeLibr.mattrans(prec, 3);

            temp = MathTimeLibr.matmult(stp, nutp, 3, 3, 3);
            trans = MathTimeLibr.matmult(temp, precp, 3, 3, 3);
            r2pef = MathTimeLibr.matvecmult(trans, r2eci, 3);
            r2ecef = MathTimeLibr.matvecmult(pmp, r2pef, 3);

            tempvec1 = MathTimeLibr.matvecmult(trans, v2eci, 3);
            MathTimeLibr.cross(omegaearth, r2pef, out crossr);
            v2pef[0] = tempvec1[0] - crossr[0];
            v2pef[1] = tempvec1[1] - crossr[1];
            v2pef[2] = tempvec1[2] - crossr[2];
            v2ecef = MathTimeLibr.matvecmult(pmp, v2pef, 3);

            if (printopt == 'y')
                Console.WriteLine(String.Format("r2pef = {0}  {1}  {2}  {3}  {4}  {5}", r2pef[0], r2pef[1], r2pef[2], v2pef[0], v2pef[1], v2pef[2]));

            // transform pef (efg) vectors for sat 1 to ecef for standard coordinate processing
            r1ecef = MathTimeLibr.matvecmult(pmp, r1pef, 3);
            v1ecef = MathTimeLibr.matvecmult(pmp, v1pef, 3);
        }  //  csm2efg 



        // -----------------------------------------------------------------------------------------
        //                                       2body functions
        // -----------------------------------------------------------------------------------------


        /* -----------------------------------------------------------------------------
         *
         *                           function rv2coe
         *
         *  this function finds the classical orbital elements given the geocentric
         *    equatorial position and velocity vectors. mu is needed if km and m are
         *    both used with the same routine
         *
         *  author        : david vallado           davallado@gmail.com   21 jun 2002
         *
         *  revisions
         *    vallado     - fix special cases                              5 sep 2002
         *    vallado     - delete extra check in inclination code        16 oct 2002
         *    vallado     - add constant file use                         29 jun 2003
         *    vallado     - add mu                                         2 apr 2007
         *
         *  inputs          description                              range / units
         *    r           - ijk position vector                          km 
         *    v           - ijk velocity vector                          km/s 
         *
         *  outputs       :
         *    p           - semilatus rectum                             km
         *    a           - semimajor axis                               km
         *    ecc         - eccentricity
         *    incl        - inclination                                  0.0  to Math.PI rad
         *    raan       - right ascension of ascending node             0.0  to 2pi rad
         *    argp        - argument of perigee                          0.0  to 2pi rad
         *    nu          - true anomaly                                 0.0  to 2pi rad
         *    m           - mean anomaly                                 0.0  to 2pi rad
         *    arglat      - argument of latitude      (ci)               0.0  to 2pi rad
         *    truelon     - true longitude            (ce)               0.0  to 2pi rad
         *    lonper      - longitude of periapsis    (ee)               0.0  to 2pi rad
         *
         *  locals        :
         *    hbar        - angular momentum h vector                    km2 / s
         *    ebar        - eccentricity     e vector
         *    nbar        - line of nodes    n vector
         *    c1          - v**2 - u/r
         *    rdotv       - r dot v
         *    hk          - hk unit vector
         *    sme         - specfic mechanical energy                    km2 / s2
         *    i           - index
         *    e           - eccentric, parabolic,
         *                  hyperbolic anomaly                           rad
         *    temp        - temporary variable
         *    typeorbit   - type of orbit                                ee, ei, ce, ci
         *
         *  coupling      :
         *    mag         - magnitude of a vector
         *    cross       - cross product of two vectors
         *    angle       - find the angle between two vectors
         *    newtonnu    - find the mean anomaly
         *
         *  references    :
         *    vallado       2007, 126, alg 9, ex 2-5
         * --------------------------------------------------------------------------- */

        public void rv2coe
             (
               double[] r, double[] v,
               out double p, out double a, out double ecc, out double incl, out double raan, out double argp,
               out double nu, out double m, out double arglat, out double truelon, out double lonper
             )
        {
            double undefined, small, magr, magv, magn, sme,
                   rdotv, infinite, temp, c1, hk, twopi, magh, halfpi, e;
            double[] hbar = new double[3];
            double[] ebar = new double[3];
            double[] nbar = new double[3];
            int i;
            string typeorbit;

            twopi = 2.0 * Math.PI;
            halfpi = 0.5 * Math.PI;
            small = 0.00000001;
            undefined = 999999.1;
            infinite = 999999.9;
            m = 0.0;

            // -------------------------  implementation   -----------------
            magr = MathTimeLibr.mag(r);
            magv = MathTimeLibr.mag(v);

            // ------------------  find h n and e vectors   ----------------
            MathTimeLibr.cross(r, v, out hbar);
            magh = MathTimeLibr.mag(hbar);
            if (magh > small)
            {
                nbar[0] = -hbar[1];
                nbar[1] = hbar[0];
                nbar[2] = 0.0;
                magn = MathTimeLibr.mag(nbar);
                c1 = magv * magv - gravConst.mu / magr;
                rdotv = MathTimeLibr.dot(r, v);
                for (i = 0; i <= 2; i++)
                    ebar[i] = (c1 * r[i] - rdotv * v[i]) / gravConst.mu;
                ecc = MathTimeLibr.mag(ebar);

                // ------------  find a e and semi-latus rectum   ----------
                sme = (magv * magv * 0.5) - (gravConst.mu / magr);
                if (Math.Abs(sme) > small)
                    a = -gravConst.mu / (2.0 * sme);
                else
                    a = infinite;
                p = magh * magh / gravConst.mu;

                // -----------------  find inclination   -------------------
                hk = hbar[2] / magh;
                incl = Math.Acos(hk);

                // --------  determine type of orbit for later use  --------
                // ------ elliptical, parabolic, hyperbolic inclined -------
                typeorbit = "ei";
                if (ecc < small)
                {
                    // ----------------  circular equatorial ---------------
                    if ((incl < small) | (Math.Abs(incl - Math.PI) < small))
                        typeorbit = "ce";
                    else
                        // --------------  circular inclined ---------------
                        typeorbit = "ci";
                }
                else
                {
                    // - elliptical, parabolic, hyperbolic equatorial --
                    if ((incl < small) | (Math.Abs(incl - Math.PI) < small))
                        typeorbit = "ee";
                }

                // ----------  find right ascension of ascending node ------------
                if (magn > small)
                {
                    temp = nbar[0] / magn;
                    if (Math.Abs(temp) > 1.0)
                        temp = Math.Sign(temp);
                    raan = Math.Acos(temp);
                    if (nbar[1] < 0.0)
                        raan = twopi - raan;
                }
                else
                    raan = undefined;

                // ---------------- find argument of perigee ---------------
                if (typeorbit.Equals("ei"))
                {
                    argp = MathTimeLibr.angle(nbar, ebar);
                    if (ebar[2] < 0.0)
                        argp = twopi - argp;
                }
                else
                    argp = undefined;

                // ------------  find true anomaly at epoch    -------------
                if (typeorbit[0] == 'e')
                {
                    nu = MathTimeLibr.angle(ebar, r);
                    if (rdotv < 0.0)
                        nu = twopi - nu;
                }
                else
                    nu = undefined;

                // ----  find argument of latitude - circular inclined -----
                if ((typeorbit.Equals("ci")) || (typeorbit.Equals("ei")))
                {
                    arglat = MathTimeLibr.angle(nbar, r);
                    if (r[2] < 0.0)
                        arglat = twopi - arglat;
                    m = arglat;
                }
                else
                    arglat = undefined;

                // -- find longitude of perigee - elliptical equatorial ----
                if ((ecc > small) && (typeorbit.Equals("ee")))
                {
                    temp = ebar[0] / ecc;
                    if (Math.Abs(temp) > 1.0)
                        temp = Math.Sign(temp);
                    lonper = Math.Acos(temp);
                    if (ebar[1] < 0.0)
                        lonper = twopi - lonper;
                    if (incl > halfpi)
                        lonper = twopi - lonper;
                }
                else
                    lonper = undefined;

                // -------- find true longitude - circular equatorial ------
                if ((magr > small) && (typeorbit.Equals("ce")))
                {
                    temp = r[0] / magr;
                    if (Math.Abs(temp) > 1.0)
                        temp = Math.Sign(temp);
                    truelon = Math.Acos(temp);
                    if (r[1] < 0.0)
                        truelon = twopi - truelon;
                    if (incl > halfpi)
                        truelon = twopi - truelon;
                    m = truelon;
                }
                else
                    truelon = undefined;

                // ------------ find mean anomaly for all orbits -----------
                if (typeorbit[0] == 'e')
                    newtonnu(ecc, nu, out e, out m);
            }
            else
            {
                p = undefined;
                a = undefined;
                ecc = undefined;
                incl = undefined;
                raan = undefined;
                argp = undefined;
                nu = undefined;
                m = undefined;
                arglat = undefined;
                truelon = undefined;
                lonper = undefined;
            }
        }  // rv2coe



        /* ------------------------------------------------------------------------------
        *
        *                           function coe2rv
        *
        *  this function finds the position and velocity vectors in geocentric
        *    equatorial (ijk) system given the classical orbit elements. the additional
        *    orbital elements provide calculations for perfectly circular and equatorial orbits. 
        *
        *  author        : david vallado           davallado@gmail.com    1 mar 2001
        *
        *  revisions
        *    vallado     - conversion to c#                              23 Nov 2011
        *
        *  inputs          description                    range / units
        *    p           - semilatus rectum               km
        *    ecc         - eccentricity
        *    incl        - inclination                    0.0 to Math.PI rad
        *    raan       - longitude of ascending node    0.0 to 2pi rad
        *    argp        - argument of perigee            0.0 to 2pi rad
        *    nu          - true anomaly                   0.0 to 2pi rad
        *    arglat      - argument of latitude      (ci) 0.0 to 2pi rad
        *    lamtrue     - true longitude            (ce) 0.0 to 2pi rad
        *    lonper      - longitude of periapsis    (ee) 0.0 to 2pi rad
        *
        *  outputs       :
        *    r           - ijk position vector            km
        *    v           - ijk velocity vector            km / s
        *
        *  locals        :
        *    temp        - temporary real*8 value
        *    rpqw        - pqw position vector            km
        *    vpqw        - pqw velocity vector            km / s
        *    sinnu       - sine of nu
        *    cosnu       - cosine of nu
        *    tempvec     - pqw velocity vector
        *
        *  coupling      :
        *    rot3        - rotation about the 3rd axis
        *    rot1        - rotation about the 1st axis
        *
        *  references    :
        *    vallado       2007, 126, alg 10, ex 2-5
        * --------------------------------------------------------------------------- */

        public void coe2rv
            (
            double p, double ecc, double incl, double raan, double argp, double nu,
            double arglat, double truelon, double lonper,
            out double[] r, out double[] v
            )
        {
            double temp, sin_nu, cos_nu, small;
            double[] rpqw = new double[3];
            double[] vpqw = new double[3];
            double[] tempvec = new double[3];

            small = 0.0000001;


            // --------------------  implementation   ----------------------
            //       determine what type of orbit is involved and set up the
            //       set up angles for the special cases.
            // -------------------------------------------------------------
            if (ecc < small)
            {
                // ----------------  circular equatorial  ------------------
                if ((incl < small) | (Math.Abs(incl - Math.PI) < small))
                {
                    argp = 0.0;
                    raan = 0.0;
                    nu = truelon;
                }
                else
                {
                    // --------------  circular inclined  ------------------
                    argp = 0.0;
                    nu = arglat;
                }
            }
            else
            {
                // ---------------  elliptical equatorial  -----------------
                if ((incl < small) | (Math.Abs(incl - Math.PI) < small))
                {
                    argp = lonper;
                    raan = 0.0;
                }
            }

            // ----------  form pqw position and velocity vectors ----------
            sin_nu = Math.Sin(nu);
            cos_nu = Math.Cos(nu);
            temp = p / (1.0 + ecc * cos_nu);
            rpqw[0] = temp * cos_nu;
            rpqw[1] = temp * sin_nu;
            rpqw[2] = 0.0;
            if (Math.Abs(p) < 0.00000001)
                p = 0.00000001;

            vpqw[0] = -sin_nu * Math.Sqrt(gravConst.mu / p);
            vpqw[1] = (ecc + cos_nu) * Math.Sqrt(gravConst.mu / p);
            vpqw[2] = 0.0;

            // ----------------  perform transformation to ijk  ------------
            tempvec = MathTimeLibr.rot3(rpqw, -argp);
            tempvec = MathTimeLibr.rot1(tempvec, -incl);
            r = MathTimeLibr.rot3(tempvec, -raan);

            tempvec = MathTimeLibr.rot3(vpqw, -argp);
            tempvec = MathTimeLibr.rot1(tempvec, -incl);
            v = MathTimeLibr.rot3(tempvec, -raan);
        }  //  coe2rv  



        // ----------------------------------------------------------------------------
        //
        //                           function rv2eq
        //
        // this function transforms a position and velocity vector into the flight
        //    elements - latgc, lon, fpa, az, position and velocity magnitude.
        //
        //  author        : david vallado           davallado@gmail.com    7 jun 2002
        //
        //  revisions
        //    vallado     - fix special orbit types (ee)                   5 sep 2002
        //    vallado     - add constant file use                         29 jun 2003
        //
        //  inputs          description                    range / units
        //    r           - eci position vector            km
        //    v           - eci velocity vector            km/s
        //
        //  outputs       :
        //    n           - mean motion                    rad
        //    a           - semi major axis                km
        //    af          - component of ecc vector
        //    ag          - component of ecc vector
        //    chi         - component of node vector in eqw
        //    psi         - component of node vector in eqw
        //    meanlon     - mean longitude                 rad
        //    truelon     - true longitude                 rad
        //
        //  locals        :
        //    none        -
        //
        //  coupling      :
        //    none        -
        //
        //  references    :
        //    vallado       2013, 108
        //    chobotov            30
        // ----------------------------------------------------------------------------

        public void rv2eq
            (
              double[] r, double[] v, out double a, out double n, out double af, out double ag,
              out double chi, out double psi, out double meanlonM,
              out double meanlonNu, out Int16 fr
            )
        {
            // -------------------------  implementation   -----------------
            double p, ecc, incl, raan, argp, nu, m, arglat, lonper, truelon;
            double small = 0.0000001;
            double undefined = 999999.1;
            double twopi = 2.0 * Math.PI;

            arglat = undefined;
            lonper = undefined;
            truelon = undefined;

            // -------- convert to classical elements ----------------------
            rv2coe(r, v, out p, out a, out ecc, out incl, out raan, out argp, out nu, out m,
                out arglat, out truelon, out lonper);

            // -------- setup retrograde factor ----------------------------
            fr = 1;
            // use for setting at 90 deg
            //if (Math.Abs(incl - Math.PI * 0.5) < 0.0001)
            // ---------- set this so it for orbits near 180 deg !! ---------
            if (Math.Abs(incl - Math.PI) < 0.0001)
                fr = -1;

            if (ecc < small)
            {
                // ----------------  circular equatorial  ------------------
                if (incl < small || Math.Abs(incl - Math.PI) < small)
                {
                    argp = 0.0;
                    raan = 0.0;
                    //   nu   = truelon;
                }
                else
                {
                    // --------------  circular inclined  ------------------
                    argp = 0.0;
                    //   nu  = arglat;
                }
            }
            else
            {
                // ---------------  elliptical equatorial  -----------------
                if ((incl < small) || (Math.Abs(incl - Math.PI) < small))
                {
                    argp = lonper;
                    raan = 0.0;
                }
            }

            af = ecc * Math.Cos(fr * raan + argp);
            ag = ecc * Math.Sin(fr * raan + argp);

            if (fr > 0)
            {
                chi = Math.Tan(incl * 0.5) * Math.Sin(raan);
                psi = Math.Tan(incl * 0.5) * Math.Cos(raan);
            }
            else
            {
                chi = MathTimeLibr.cot(incl * 0.5) * Math.Sin(raan);
                psi = MathTimeLibr.cot(incl * 0.5) * Math.Cos(raan);
            }

            n = Math.Sqrt(gravConst.mu / (a * a * a));

            meanlonM = fr * raan + argp + m;
            meanlonM = meanlonM % twopi;

            meanlonNu = fr * raan + argp + nu;
            meanlonNu = meanlonNu % twopi;
        }   // rv2eq


        // ------------------------------------------------------------------------------
        //
        //                           function eq2rv
        //
        //  this function finds the classical orbital elements given the equinoctial
        //    elements.
        //
        //  author        : david vallado           davallado@gmail.com    9 jun 2002
        //
        //  revisions
        //    vallado     - fix elliptical equatorial orbits case         19 oct 2002
        //    vallado     - add constant file use                         29 jun 2003
        //
        //  inputs          description                    range / units
        //    a           - semimajor axis                 km
        //    af          - component of ecc vector
        //    ag          - component of ecc vector
        //    chi         - component of node vector in eqw
        //    psi         - component of node vector in eqw
        //    meanlon     - mean longitude                 rad
        //    fr          - retrograde factor, neg if incl > 90 deg 1, -1
        //
        //  outputs       :
        //    r           - position vector                km
        //    v           - velocity vector                km/s
        //
        //  locals        :
        //    n           - mean motion                    rad
        //    temp        - temporary variable
        //    p           - semilatus rectum               km
        //    ecc         - eccentricity
        //    incl        - inclination                    0.0  to pi rad
        //    raan        - longitude of ascending node    0.0  to 2pi rad
        //    argp        - argument of perigee            0.0  to 2pi rad
        //    nu          - true anomaly                   0.0  to 2pi rad
        //    m           - mean anomaly                   0.0  to 2pi rad
        //    arglat      - argument of latitude      (ci) 0.0  to 2pi rad
        //    truelon     - true longitude            (ce) 0.0  to 2pi rad
        //    lonper      - longitude of periapsis    (ee) 0.0  to 2pi rad
        //
        //  coupling      :
        //
        //  references    :
        //    vallado 2013:108
        // ------------------------------------------------------------------------------

        public void eq2rv
            (
            double a, double af, double ag, double chi, double psi, double meanlon, Int16 fr, out double[] r, out double[] v
            )
        {
            // -------------------------  implementation   -----------------
            double p, ecc, incl, raan, argp, nu, m, eccanom, arglat, lonper, truelon;
            double small = 0.0000001;
            double undefined = 999999.1;
            double twopi = 2.0 * Math.PI;
            arglat = 999999.1;
            lonper = 999999.1;
            truelon = 999999.1;

            // ---- if n is input ----
            //a = (gravConst.mu/n^2)^(1.0/3.0);

            ecc = Math.Sqrt(af * af + ag * ag);
            p = a * (1.0 - ecc * ecc);
            incl = Math.PI * ((1.0 - fr) * 0.5) + 2.0 * fr * Math.Atan(Math.Sqrt(chi * chi + psi * psi));
            raan = Math.Atan2(chi, psi);
            argp = Math.Atan2(ag, af) - fr * Math.Atan2(chi, psi);

            if (ecc < small)
            {
                // ----------------  circular equatorial  ------------------
                if (incl < small || Math.Abs(incl - Math.PI) < small)
                {
                    argp = 0.0;
                    raan = 0.0;
                    //                truelon = nu;
                }
                else
                {
                    // --------------  circular inclined  ------------------
                    argp = 0.0;
                    //                arglat = nu;
                }
            }
            else
            {
                // ---------------  elliptical equatorial  -----------------
                if ((incl < small) || (Math.Abs(incl - Math.PI) < small))
                {
                    //                argp = lonper;
                    raan = 0.0;
                }
            }

            m = meanlon - fr * raan - argp;
            m = (m + twopi) % twopi;

            newtonm(ecc, m, out eccanom, out nu);

            // ----------  fix for elliptical equatorial orbits ------------
            if (ecc < small)
            {
                // ----------------  circular equatorial  ------------------
                if ((incl < small) || (Math.Abs(incl - Math.PI) < small))
                {
                    argp = undefined;
                    raan = undefined;
                    truelon = nu;
                }
                else
                {
                    // --------------  circular inclined  ------------------
                    argp = undefined;
                    arglat = nu;
                }
                nu = undefined;
            }
            else
            {
                // ---------------  elliptical equatorial  -----------------
                if ((incl < small) || (Math.Abs(incl - Math.PI) < small))
                {
                    lonper = argp;
                    argp = undefined;
                    raan = undefined;
                }
            }

            // -------- now convert back to position and velocity vectors
            coe2rv(p, ecc, incl, raan, argp, nu, arglat, truelon, lonper, out r, out v);
        } // eq2rv


        /* ----------------------------------------------------------------------------
        *
        *                           function rv2flt
        *
        *  this function transforms a position and velocity vector into the flight
        *    elements - latgc, lon, fpa, az, position and velocity magnitude.
        *
        *  author        : david vallado           davallado@gmail.com   17 jun 2002
        *
        *  revisions
        *    vallado     - add terms for ast calculation                 30 sep 2002
        *    vallado     - chg magr var names                            23 may 2003
        *
        *  inputs description                                             range / units
        *    r           - eci position vector km
        *    v           - eci velocity vector km/s
        *    ttt         - julian centuries of tt                          centuries
        *    jdut1       - julian date of ut1                              days from 4713 bc
        *    lod         - excess length of day                            sec
        *    xp          - polar motion coefficient                        arc sec
        *    yp          - polar motion coefficient                        arc sec
        *    terms       - number of terms for ast calculation             0,2
        *    ddpsi, ddeps - corrections for fk5 to gcrf                    rad
        *
        *  outputs       :
        *    magr        - eci position vector magnitude                   km
        *    magv        - eci velocity vector magnitude                   km/sec
        *    latgc       - geocentric latitude                             rad
        *    lon         - longitude                                       rad
        *    fpa         - sat flight path angle                           rad
        *    az          - sat flight path az                              rad
        *
        *  locals        :
        *    fpav        - sat flight path anglefrom vert                  rad
        *
        *    none        -
        *  references    :
        *    vallado       2013, xx
        * ----------------------------------------------------------------------------*/

        public void rv2flt
            (
            double[] reci, double[] veci, double jdtt, double jdftt,
            double jdut1, double jdxysstart, double lod,
            double xp, double yp, int terms, double ddpsi, double ddeps, double ddx, double ddy,
            EOPSPWLib.iau80Class iau80arr, EOPSPWLib.iau00Class iau00arr,
            out double lon, out double latgc, out double rtasc, out double decl,
            out double fpa, out double az, out double magr, out double magv
            )
        {
            double[] avec = new double[3];
            double[] recef = new double[3];
            double[] vecef = new double[3];
            double[] h = new double[3];
            double[] hcrossr = new double[3];
            double small = 0.00000001;
            double temp, hmag, rdotv, fpav;

            magr = MathTimeLibr.mag(reci);
            magv = MathTimeLibr.mag(veci);

            // -------- convert r to ecef for lat/lon calculation
            eci_ecef(ref reci, ref veci, MathTimeLib.Edirection.eto, ref recef, ref vecef,
                EOpt.e80, EOPSPWLibr.iau80arr, EOPSPWLibr.iau00arr,
                jdtt, jdftt, jdut1, jdxysstart, lod, xp, yp, ddpsi, ddeps, ddx, ddy);

            // ----------------- find longitude value  ----------------- uses ecef
            temp = Math.Sqrt(recef[0] * recef[0] + recef[1] * recef[1]);
            if (temp < small)
                lon = Math.Atan2(vecef[1], vecef[0]);
            else
                lon = Math.Atan2(recef[1], recef[0]);
            //latgc = atan2(recef[2] , sqrt(recef[0]^2 + recef[1]^2) )
            latgc = Math.Asin(recef[2] / magr);

            // ------------- calculate rtasc and decl ------------------ uses eci
            temp = Math.Sqrt(reci[0] * reci[0] + reci[1] * reci[1]);
            if (temp < small)
                rtasc = Math.Atan2(veci[1], veci[0]);
            else
                rtasc = Math.Atan2(reci[1], reci[0]);
            //decl = atan2(reci[2] , sqrt(reci[0]^2 + reci[1]^2) )
            decl = Math.Asin(reci[2] / magr);

            MathTimeLibr.cross(reci, veci, out h);
            hmag = MathTimeLibr.mag(h);
            rdotv = MathTimeLibr.dot(reci, veci);
            fpav = Math.Atan2(hmag, rdotv);
            fpa = Math.PI * 0.5 - fpav;

            MathTimeLibr.cross(h, reci, out hcrossr);

            az = Math.Atan2(reci[0] * hcrossr[1] - reci[1] * hcrossr[0], hcrossr[2] * magr);
        }  // rv2flt


        /*------------------------------------------------------------------------------
        *
        *                           procedure rv_elatlon
        *
        *  this procedure converts ecliptic latitude and longitude with position and
        *    velocity vectors. uses velocity vector to find the solution of Math.Singular
        *    cases.
        *
        *  author        : david vallado           davallado@gmail.com   22 jun 2002
        *
        *  inputs          description                    range / units
        *    rijk        - ijk position vector            er
        *    vijk        - ijk velocity vector            er/tu
        *    direction   - which set of vars to output    from  too
        *
        *  outputs       :
        *    rr          - radius of the sat              er
        *    ecllat      - ecliptic latitude              -Math.PI/2 to Math.PI/2 rad
        *    ecllon      - ecliptic longitude             -Math.PI/2 to Math.PI/2 rad
        *    drr         - radius of the sat rate         er/tu
        *    decllat     - ecliptic latitude rate         -Math.PI/2 to Math.PI/2 rad
        *    eecllon     - ecliptic longitude rate        -Math.PI/2 to Math.PI/2 rad
        *
        *  locals        :
        *    obliquity   - obliquity of the ecliptic      rad
        *    temp        -
        *    temp1       -
        *    re          - position vec in eclitpic frame
        *    ve          - velocity vec in ecliptic frame
        *
        *  coupling      :
        *    mag         - magnitude of a vector
        *    rot1        - rotation about 1st axis
        *    dot         - dot product
        *    arcsin      - arc Math.Sine function
        *    Math.Atan2       - arc tangent function that resolves quadrant ambiguites
        *
        *  references    :
        *    vallado       2013, 268, eq 4-15
        -----------------------------------------------------------------------------*/

        public void rv_elatlon
        (
        ref double[] rijk, ref double[] vijk,
        Enum direct,
        ref double rr, ref double ecllat, ref double ecllon,
        ref double drr, ref double decllat, ref double decllon
        )
        {
            const double small = 0.00000001;
            double[] re = new double[3];
            double[] ve = new double[3];
            double obliquity, temp, temp1;

            obliquity = 0.40909280; // 23.439291/rad
            if (direct.Equals(MathTimeLib.Edirection.efrom))
            {
                re[0] = (rr * Math.Cos(ecllat) * Math.Cos(ecllon));
                re[1] = (rr * Math.Cos(ecllat) * Math.Sin(ecllon));
                re[2] = (rr * Math.Sin(ecllon));
                ve[0] = (drr * Math.Cos(ecllat) * Math.Cos(ecllon) -
                    rr * Math.Sin(ecllat) * Math.Cos(ecllon) * decllat -
                    rr * Math.Cos(ecllat) * Math.Sin(ecllon) * decllon);
                ve[1] = (drr * Math.Cos(ecllat) * Math.Sin(ecllon) -
                    rr * Math.Sin(ecllat) * Math.Cos(ecllon) * decllat +
                    rr * Math.Cos(ecllat) * Math.Cos(ecllon) * decllon);
                ve[2] = (drr * Math.Sin(ecllat) + rr * Math.Cos(ecllat) * decllat);

                rijk = MathTimeLibr.rot1(re, -obliquity);
                vijk = MathTimeLibr.rot1(ve, -obliquity);
            }
            else
            {
                /* -------------- calculate angles and rates ---------------- */
                rr = MathTimeLibr.mag(rijk);
                temp = Math.Sqrt(rijk[0] * rijk[0] + rijk[1] * rijk[1]);
                if (temp < small)
                {
                    temp1 = Math.Sqrt(vijk[0] * vijk[0] + vijk[1] * vijk[1]);
                    if (Math.Abs(temp1) > small)
                        ecllon = Math.Atan2(vijk[1] / temp1, vijk[0] / temp1);
                    else
                        ecllon = 0.0;
                }
                else
                    ecllon = Math.Atan2(rijk[1] / temp, rijk[0] / temp);
                ecllat = Math.Asin(rijk[2] / MathTimeLibr.mag(rijk));

                temp1 = -rijk[1] * rijk[1] - rijk[0] * rijk[0]; // different now
                drr = MathTimeLibr.dot(rijk, vijk) / rr;
                if (Math.Abs(temp1) > small)
                    decllon = (vijk[0] * rijk[1] - vijk[1] * rijk[0]) / temp1;
                else
                    decllon = 0.0;
                if (Math.Abs(temp) > small)
                    decllat = (vijk[2] - drr * Math.Sin(ecllat)) / temp;
                else
                    decllat = 0.0;
            }
        } //  rv_elatlon


        /* ------------------------------------------------------------------------------
        * 
        *                            function rv_radec
        * 
        *   this function converts the right ascension and declination values with
        *     position and velocity vectors of a satellite. uses velocity vector to
        *     find the solution of singular cases.
    	*
    	*  author        : david vallado           davallado@gmail.com   22 jun 2002
    	*
        *   inputs          description                          range / units
    	*     r           - position vector eci                       km, er
    	*     v           - velocity vector eci                       km/s, er/tu
    	*     direct      -  direction to convert                     eFrom  eTo
    	*
        *   outputs       :
        *     rr          - radius of the satellite                      km
        *     rtasc       - right ascension                              rad
        *     decl        - declination                                  rad
        *     drr         - radius of the satellite rate                 km/s
        *     drtasc      - right ascension rate                         rad/s
        *     ddecl       - declination rate                             rad/s
        * 
        *   locals        :
        *     temp        - temporary position vector
        *     temp1       - temporary variable
        * 
        *   coupling      :
        *     none
        * 
        *   references    :
    	*    vallado       2013, 259, alg 25
    	-----------------------------------------------------------------------------*/

        public void rv_radec
            (
            ref double[] r, ref double[] v,
            Enum direct,
            ref double rr, ref double rtasc, ref double decl, ref double drr, ref double drtasc, ref double ddecl
            )
        {
            double small = 0.00000001;
            double temp, temp1;

            // -------------------------  implementation   -------------------------
            if (direct.Equals(MathTimeLib.Edirection.efrom))
            {
                r[0] = (rr * Math.Cos(decl) * Math.Cos(rtasc));
                r[1] = (rr * Math.Cos(decl) * Math.Sin(rtasc));
                r[2] = (rr * Math.Sin(decl));
                v[0] = (drr * Math.Cos(decl) * Math.Cos(rtasc) -
                    rr * Math.Sin(decl) * Math.Cos(rtasc) * ddecl -
                    rr * Math.Cos(decl) * Math.Sin(rtasc) * drtasc);
                v[1] = (drr * Math.Cos(decl) * Math.Sin(rtasc) -
                    rr * Math.Sin(decl) * Math.Sin(rtasc) * ddecl +
                    rr * Math.Cos(decl) * Math.Cos(rtasc) * drtasc);
                v[2] = (drr * Math.Sin(decl) + rr * Math.Cos(decl) * ddecl);
            }
            else
            {
                // ------------- calculate angles and rates ----------------
                rr = MathTimeLibr.mag(r);
                temp = Math.Sqrt(r[0] * r[0] + r[1] * r[1]);
                if (temp < small)
                    rtasc = Math.Atan2(v[1], v[0]);
                else
                    rtasc = Math.Atan2(r[1], r[0]);
                if (rtasc < 0.0)
                    rtasc = rtasc + Math.PI * 2.0;
                decl = Math.Asin(r[2] / rr);

                temp1 = -r[1] * r[1] - r[0] * r[0];  // different now
                drr = MathTimeLibr.dot(r, v) / rr;
                if (Math.Abs(temp1) > small)
                    drtasc = (v[0] * r[1] - v[1] * r[0]) / temp1;
                else
                    drtasc = 0.0;
                if (Math.Abs(temp) > small)
                    ddecl = (v[2] - drr * Math.Sin(decl)) / temp;
                else
                    ddecl = 0.0;
            }

        }  // rv_radec



        /*------------------------------------------------------------------------------
        *
        *                           procedure rv_razel
        *
        *  this procedure converts range, azimuth, and elevation and their rates with
        *    the geocentric equatorial (ecef) position and velocity vectors.  notice the
        *    value of small as it can affect rate term calculations. uses velocity
	    *    vector to find the solution of singular cases.
        *
        *  author        : david vallado           davallado@gmail.com   22 jun 2002
        *
        *  inputs          description                            range / units
        *    recef       - ecef position vector                       km
        *    vecef       - ecef velocity vector                       km/s
	    *    latgd       - geodetic latitude                          -pi/2 to pi/2 rad
	    *    lon         - geodetic longitude                         -2pi to pi rad
        *    direct      -  direction to convert                      eFrom  eTo
        *
        *  outputs       :
        *    rho         - satellite range from site                  km
        *    az          - azimuth                                    0.0 to 2pi rad
	    *    el          - elevation                                  -pi/2 to pi/2 rad
        *    drho        - range rate                                 km/s
        *    daz         - azimuth rate                               rad/s
        *    del         - elevation rate                             rad/s
        *
        *  locals        :
        *    rsecef      - ecef site position vector                  km
        *    rhovecef    - ecef range vector from site                km
        *    drhovecef   - ecef velocity vector from site             km/s
        *    rhosez      - sez range vector from site                 km
        *    drhosez     - sez velocity vector from site              km
        *    tempvec     - temporary vector
        *    temp        - temporary extended value
        *    temp1       - temporary extended value
        *    i           - index
        *
        *  coupling      :
        *    mag         - magnitude of a vector
        *    addvec      - add two vectors
        *    rot3        - rotation about the 3rd axis
        *    rot2        - rotation about the 2nd axis
        *    atan2       - arc tangent function which also resloves quadrants
        *    dot         - dot product of two vectors
        *    rvsez_razel - find r and v from site in topocentric horizon (sez) system
        *    arcsin      - arc sine function
        *    sign        - returns the sign of a variable
        *
        *  references    :
        *    vallado       2013, 265, alg 27
        -----------------------------------------------------------------------------*/

        public void rv_razel
            (
            ref double[] recef, ref double[] vecef, double latgd, double lon, double alt,
            Enum direct,
            ref double rho, ref double az, ref double el, ref double drho, ref double daz, ref double del
            )
        {
            const double halfpi = Math.PI * 0.5;
            const double small = 0.0000001;

            double temp, temp1;
            double[] rsecef = new double[3];
            double[] vsecef = new double[3];
            double[] rhoecef = new double[3];
            double[] drhoecef = new double[3];
            double[] rhosez = new double[3];
            double[] drhosez = new double[3];
            double[] tempvec = new double[3];

            site(latgd, lon, alt, out rsecef, out vsecef);

            if (direct.Equals(MathTimeLib.Edirection.efrom))
            {
                /* ---------  find sez range and velocity vectors ----------- */
                rvsez_razel(ref rhosez, ref drhosez, direct, ref rho, ref az, ref el, ref drho, ref daz, ref del);

                /* ----------  perform sez to ecef transformation ------------ */
                tempvec = MathTimeLibr.rot2(rhosez, latgd - halfpi);
                rhoecef = MathTimeLibr.rot3(tempvec, -lon);
                tempvec = MathTimeLibr.rot2(drhosez, latgd - halfpi);
                drhoecef = MathTimeLibr.rot3(tempvec, -lon);

                /* ---------  find ecef range and velocity vectors -----------*/
                MathTimeLibr.addvec(1.0, rhoecef, 1.0, rsecef, out recef);
                vecef[0] = drhoecef[0];
                vecef[1] = drhoecef[1];
                vecef[2] = drhoecef[2];
            }
            else
            {
                /* ------- find ecef range vector from site to satellite ----- */
                MathTimeLibr.addvec(1.0, recef, -1.0, rsecef, out rhoecef);
                drhoecef[0] = vecef[0];
                drhoecef[1] = vecef[1];
                drhoecef[2] = vecef[2];
                rho = MathTimeLibr.mag(rhoecef);

                /* ------------ convert to sez for calculations ------------- */
                tempvec = MathTimeLibr.rot3(rhoecef, lon);
                rhosez = MathTimeLibr.rot2(tempvec, halfpi - latgd);
                tempvec = MathTimeLibr.rot3(drhoecef, lon);
                drhosez = MathTimeLibr.rot2(tempvec, halfpi - latgd);

                /* ------------ calculate azimuth and elevation ------------- */
                temp = Math.Sqrt(rhosez[0] * rhosez[0] + rhosez[1] * rhosez[1]);
                if (Math.Abs(rhosez[1]) < small)
                    if (temp < small)
                    {
                        temp1 = Math.Sqrt(drhosez[0] * drhosez[0] + drhosez[1] * drhosez[1]);
                        az = Math.Atan2(drhosez[1] / temp1, -drhosez[0] / temp1);
                    }
                    else
                        if (rhosez[0] > 0.0)
                        az = Math.PI;
                    else
                        az = 0.0;
                else
                    az = Math.Atan2(rhosez[1] / temp, -rhosez[0] / temp);
                if (az < 0.0)
                    az = az + Math.PI * 2.0;

                el = Math.Asin(rhosez[2] / rho);

                /* ----- calculate range, azimuth and elevation rates ------- */
                drho = MathTimeLibr.dot(rhosez, drhosez) / rho;
                if (Math.Abs(temp * temp) > small)
                    daz = (drhosez[0] * rhosez[1] - drhosez[1] * rhosez[0]) / (temp * temp);
                else
                    daz = 0.0;

                if (Math.Abs(temp) > 0.00000001)
                    del = (drhosez[2] - drho * Math.Sin(el)) / temp;
                else
                    del = 0.0;
            }
        }  //  rv_razel


        /*------------------------------------------------------------------------------
        *
        *                           procedure rv_tradec
        *
        *  this procedure converts topocentric right-ascension declination with
        *    position and velocity vectors. the velocity vector is used to find the
        *    solution of singular cases.
        *
        *  author        : david vallado           davallado@gmail.com   22 jun 2002
        *
        *  inputs          description                            range / units
        *    reci        - eci position vector                    km
        *    veci        - eci velocity vector                    km/s
        *    rseci       - eci site position vector               km
        *    direct      - direction to convert                   eFrom  eTo
        *
        *  outputs       :
        *    rho         - topo radius of the sat                 km
        *    trtasc      - topo right ascension                   rad
        *    tdecl       - topo declination                       rad
        *    drho        - topo radius of the sat rate            km/s
        *    tdrtasc     - topo right ascension rate              rad/s
        *    tddecl      - topo declination rate                  rad/s
        *
        *  locals        :
        *    rhov        - eci range vector from site             km
        *    drhov       - eci velocity vector from site          km/s
        *    latgc       - geocentric latitude                    rad
        *
        *  coupling      :
        *    mag         - magnitude of a vector
        *    addvec      - add two vectors
        *    dot         - dot product of two vectors
        *
        *  references    :
        *    vallado       2022, 254-257, eq 4-1, 4-2, alg 26
        -----------------------------------------------------------------------------*/

        public void rv_tradec
        (
        ref double[] reci, ref double[] veci, double[] rseci,
        Enum direct,
        ref double rho, ref double trtasc, ref double tdecl,
        ref double drho, ref double dtrtasc, ref double dtdecl
        )
        {
            const double small = 0.00000001;

            double[] earthrate = new double[3];
            double[] rhov = new double[3];
            double[] drhov = new double[3];
            double[] vseci = new double[3];
            double latgc, temp, temp1;

            latgc = Math.Asin(rseci[2] / MathTimeLibr.mag(rseci));

            earthrate[0] = 0.0;
            earthrate[1] = 0.0;
            earthrate[2] = gravConst.earthrot;
            MathTimeLibr.cross(earthrate, rseci, out vseci);

            if (direct.Equals(MathTimeLib.Edirection.efrom))
            {
                // --------  calculate topocentric slant range vectors ------------------ 
                rhov[0] = (rho * Math.Cos(tdecl) * Math.Cos(trtasc));
                rhov[1] = (rho * Math.Cos(tdecl) * Math.Sin(trtasc));
                rhov[2] = (rho * Math.Sin(tdecl));

                drhov[0] = (drho * Math.Cos(tdecl) * Math.Cos(trtasc) -
                    rho * Math.Sin(tdecl) * Math.Cos(trtasc) * dtdecl -
                    rho * Math.Cos(tdecl) * Math.Sin(trtasc) * dtrtasc);
                drhov[1] = (drho * Math.Cos(tdecl) * Math.Sin(trtasc) -
                    rho * Math.Sin(tdecl) * Math.Sin(trtasc) * dtdecl +
                    rho * Math.Cos(tdecl) * Math.Cos(trtasc) * dtrtasc);
                drhov[2] = (drho * Math.Sin(tdecl) + rho * Math.Cos(tdecl) * dtdecl);

                // ------ find eci range vector from geocenter to satellite ------ 
                MathTimeLibr.addvec(1.0, rhov, 1.0, rseci, out reci);
                MathTimeLibr.addvec(1.0, drhov, Math.Cos(latgc), vseci, out veci);
            }
            else
            {
                // ------ find eci range vector from site to satellite ------  
                MathTimeLibr.addvec(1.0, reci, -1.0, rseci, out rhov);
                MathTimeLibr.addvec(1.0, veci, -Math.Cos(latgc), vseci, out drhov);

                // -------- calculate topocentric angle and rate values -----  
                rho = MathTimeLibr.mag(rhov);
                temp = Math.Sqrt(rhov[0] * rhov[0] + rhov[1] * rhov[1]);
                if (temp < small)
                {
                    temp1 = Math.Sqrt(drhov[0] * drhov[0] + drhov[1] * drhov[1]);
                    trtasc = Math.Atan2(drhov[1] / temp1, drhov[0] / temp1);
                }
                else
                    trtasc = Math.Atan2(rhov[1] / temp, rhov[0] / temp);
                if (trtasc < 0.0)
                    trtasc = trtasc + Math.PI * 2.0;

                tdecl = Math.Asin(rhov[2] / MathTimeLibr.mag(rhov));

                temp1 = -rhov[1] * rhov[1] - rhov[0] * rhov[0];
                drho = MathTimeLibr.dot(rhov, drhov) / rho;
                if (Math.Abs(temp1) > small)
                    dtrtasc = (drhov[0] * rhov[1] - drhov[1] * rhov[0]) / temp1;
                else
                    dtrtasc = 0.0;
                if (Math.Abs(temp) > small)
                    dtdecl = (drhov[2] - drho * Math.Sin(tdecl)) / temp;
                else
                    dtdecl = 0.0;
            }
        } //  rv_tradec


        /*------------------------------------------------------------------------------
        *
        *                           procedure rvsez_razel
        *
        *  this procedure converts range, azimuth, and elevation values with slant
        *    range and velocity vectors for a satellite from a radar site in the
        *    topocentric horizon (sez) system.
        *
        *  author        : david vallado           davallado@gmail.com   22 jun 2002
        *
        *  inputs          description                           range / units
        *    rhovec      - sez satellite range vector           km
        *    drhovec     - sez satellite velocity vector        km/s
        *    direct      - direction to convert                 eFrom  eTo
        *
        *  outputs       :
        *    rho         - satellite range from site            km
        *    az          - azimuth                              0.0 to 2pi rad
        *    el          - elevation                            -Math.PI/2 to Math.PI/2 rad
        *    drho        - range rate                           km/s
        *    daz         - azimuth rate                         rad/s
        *    del         - elevation rate                       rad/s
        *
        *  locals        :
        *    sinel       - variable for Math.Sin( el )
        *    cosel       - variable for Math.Cos( el )
        *    sinaz       - variable for Math.Sin( az )
        *    cosaz       - variable for Math.Cos( az )
        *    temp        -
        *    temp1       -
        *
        *  coupling      :
        *    mag         - magnitude of a vector
        *    sign        - returns the sign of a variable
        *    dot         - dot product
        *    arcsin      - arc sine function
        *    atan2       - arc tangent function that resolves quadrant ambiguites
        *
        *  references    :
        *    vallado       2013, 261, eq 4-4, eq 4-5
        -----------------------------------------------------------------------------*/

        public void rvsez_razel
        (
        ref double[] rhosez, ref double[] drhosez,
        Enum direct,
        ref double rho, ref double az, ref double el, ref double drho, ref double daz, ref double del
        )
        {
            const double small = 0.00000001;

            double temp1, temp, sinel, cosel, sinaz, cosaz;

            if (direct.Equals(MathTimeLib.Edirection.efrom))
            {
                sinel = Math.Sin(el);
                cosel = Math.Cos(el);
                sinaz = Math.Sin(az);
                cosaz = Math.Cos(az);

                /* ----------------- form sez range vector ------------------ */
                rhosez[0] = (-rho * cosel * cosaz);
                rhosez[1] = (rho * cosel * sinaz);
                rhosez[2] = (rho * sinel);

                /* --------------- form sez velocity vector ----------------- */
                drhosez[0] = (-drho * cosel * cosaz +
                    rhosez[2] * del * cosaz + rhosez[1] * daz);
                drhosez[1] = (drho * cosel * sinaz -
                    rhosez[2] * del * sinaz - rhosez[0] * daz);
                drhosez[2] = (drho * sinel + rho * del * cosel);
            }
            else
            {
                /* ------------ calculate azimuth and elevation ------------- */
                temp = Math.Sqrt(rhosez[0] * rhosez[0] + rhosez[1] * rhosez[1]);
                if (Math.Abs(rhosez[1]) < small)
                    if (temp < small)
                    {
                        temp1 = Math.Sqrt(drhosez[0] * drhosez[0] + drhosez[1] * drhosez[1]);
                        az = Math.Atan2(drhosez[1] / temp1, drhosez[0] / temp1);
                    }
                    else
                        if (drhosez[0] > 0.0)
                        az = Math.PI;
                    else
                        az = 0.0;
                else
                    az = Math.Atan2(rhosez[1] / temp, rhosez[0] / temp);

                el = Math.Asin(rhosez[2] / MathTimeLibr.mag(rhosez));

                /* ------  calculate range, azimuth and elevation rates ----- */
                drho = MathTimeLibr.dot(rhosez, drhosez) / rho;
                if (Math.Abs(temp * temp) > small)
                    daz = (drhosez[0] * rhosez[1] - drhosez[1] * rhosez[0]) / (temp * temp);
                else
                    daz = 0.0;

                if (Math.Abs(temp) > small)
                    del = (drhosez[2] - drho * Math.Sin(el)) / temp;
                else
                    del = 0.0;
            }
        }   //  rvsez_razel



        // this is an approximate transformation for az_el to right ascenion declination
        public void azel_radec
        (
            double az, double el, double lst, double latgd,
            out double rtasc, out double decl, out double rtasc1
        )
        {
            double slha1, clha1, lha1, slha2, clha2, lha2;

            decl = Math.Asin(Math.Sin(el) * Math.Sin(latgd) + Math.Cos(el) * Math.Cos(latgd) * Math.Cos(az));

            slha1 = -(Math.Sin(az) * Math.Cos(el) * Math.Cos(latgd)) / (Math.Cos(decl) * Math.Cos(latgd));
            clha1 = (Math.Sin(el) - Math.Sin(latgd) * Math.Sin(decl)) / (Math.Cos(decl) * Math.Cos(latgd));

            lha1 = Math.Atan2(slha1, clha1);
            //    fprintf(1,' lha1 %13.7f \n', lha1* rad);
            rtasc = lst - lha1;

            // alt approach
            slha2 = -(Math.Sin(az) * Math.Cos(el)) / (Math.Cos(decl));
            clha2 = (Math.Cos(latgd) * Math.Sin(el) - Math.Sin(latgd) * Math.Cos(el) * Math.Cos(az)) / (Math.Cos(decl));

            lha2 = Math.Atan2(slha2, clha2);
            //    fprintf(1,' lha2 %13.7f \n', lha2* rad);

            rtasc1 = lst - lha2;
        }


        // -----------------------------------------------------------------------------
        // rr is input, can put in GEO 42164, or actual geo range value
        public void radecgeo2azel
            (
            double rtasc, double decl, double rr, double latgd, double lon, double alt,
            double ttt, double jdut1, double lod, double xp, double yp, double ddpsi, double ddeps,
            EOpt opt,
            out double az, out double el
            )
        {
            double[] fArgs = new double[14];
            double[] reci = new double[3];
            double[] rhosez = new double[3];
            double[] drhosez = new double[3];
            double[] rhoecef = new double[3];
            double[] rpef = new double[3];
            double[] recef = new double[3];
            double[] rs = new double[3];
            double[] vs = new double[3];
            double[,] prec = new double[3, 3];
            double[,] nut = new double[3, 3];
            double[,] st = new double[3, 3];
            double[,] stdot = new double[3, 3];
            double[,] pm = new double[3, 3];
            double[,] tempmat = new double[3, 3];
            double[] tempvec = new double[3];
            double[] tempvec1 = new double[3];
            double[,] prect = new double[3, 3];
            double[,] nutt = new double[3, 3];
            double[,] stt = new double[3, 3];
            double[,] pmt = new double[3, 3];
            double temp;
            double psia, wa, ea, xa, deltapsi, deltaeps, trueeps, meaneps, magrhosez;
            az = 0.0;
            el = 0.0;

            // find slant range distance to GEO sat
            // ----------------- get site vector in ecef -------------------
            site(latgd, lon, alt, out rs, out vs);

            // find eci GEO location that is being looked at
            // larger call
            // [r,v] = radec2rv( rr,rtasc,decl,drr,drtasc,ddecl );
            reci[0] = rr * Math.Cos(decl) * Math.Cos(rtasc);
            reci[1] = rr * Math.Cos(decl) * Math.Sin(rtasc);
            reci[2] = rr * Math.Sin(decl);

            // larger call
            // [rho,az,el,drho,daz,del] = rv2razel ( reci,veci, latgd,lon,alt,ttt,jdut1,lod,xp,yp,terms,ddpsi,ddeps );
            double halfpi = Math.PI * 0.5;
            double small = 0.00000001;
            // -------------------- convert eci to ecef --------------------
            fundarg(ttt, opt, out fArgs);

            // a = [0;0;0];
            // [recef,vecef,aecef] = eci2ecef(reci,veci,a,ttt,jdut1,lod,xp,yp,terms,ddpsi,ddeps);
            prec = precess(ttt, opt, out psia, out wa, out ea, out xa);
            nut = nutation(ttt, ddpsi, ddeps, EOPSPWLibr.iau80arr, opt, fArgs, out deltapsi, out deltaeps, out trueeps, out meaneps);
            st = sidereal(jdut1, deltapsi, meaneps, fArgs, lod, 2, opt);  // stdot calc, not returned, use terms = 2 since well past 1997
            pm = polarm(xp, yp, ttt, opt);
            prect = MathTimeLibr.mattrans(prec, 3);
            nutt = MathTimeLibr.mattrans(nut, 3);
            stt = MathTimeLibr.mattrans(st, 3);
            pmt = MathTimeLibr.mattrans(pm, 3);

            tempvec = MathTimeLibr.matvecmult(prect, reci, 3);
            tempvec1 = MathTimeLibr.matvecmult(nutt, tempvec, 3);
            rpef = MathTimeLibr.matvecmult(stt, tempvec1, 3);
            recef = MathTimeLibr.matvecmult(pmt, rpef, 3);

            // ------- find ecef range vector from site to satellite -------
            int i;
            for (i = 0; i < 3; i++)
                rhoecef[i] = recef[i] - rs[i];
            //    drhoecef = vecef;
            //    rho      = MathTimeLibr.mag(rhoecef);
            // ------------- convert to sez for calculations ---------------
            tempvec = MathTimeLibr.rot3(rhoecef, lon);
            rhosez = MathTimeLibr.rot2(tempvec, halfpi - latgd);
            //    [tempvec]= rot3( drhoecef, lon         );
            //    [drhosez]= rot2( tempvec,  halfpi-latgd);

            // ------------- calculate azimuth and elevation ---------------
            temp = Math.Sqrt(rhosez[0] * rhosez[0] + rhosez[1] * rhosez[1]);
            if ((temp < small))           // directly over the north pole
                el = Math.Sign(rhosez[2]) * halfpi;   // +- 90 deg
            else
            {
                magrhosez = MathTimeLibr.mag(rhosez);
                el = Math.Asin(rhosez[2] / magrhosez);
            }

            if (temp < small)
                az = Math.Atan2(drhosez[1], -drhosez[0]);
            else
                az = Math.Atan2(rhosez[1] / temp, -rhosez[0] / temp);
        }  // radecgeo2azel



        /* -----------------------------------------------------------------------------
        *
        *                           function rv2rsw
        *
        *  this function converts position and velocity vectors into radial, along-
        *    track, and MathTimeLibr.cross-track coordinates. note that sometimes the middle vector
        *    is called in-track.
        *
        *  author        : david vallado           davallado@gmail.com    9 jun 2002
        *
        *  revisions
        *    vallado     - conversion to c#                              23 Nov 2011
        *
        *  inputs          description                    range / units
        *    r           - position vector                km
        *    v           - velocity vector                km/s
        *
        *  outputs       :
        *    rrsw        - position vector                km
        *    vrsw        - velocity vector                km/s
        *    transmat    - transformation matrix
        *
        *  locals        :
        *    tempvec     - temporary vector
        *    rvec,svec,wvec - direction Math.Cosines
        *
        *  coupling      :
        *
        *  references    :
        *    vallado       2007, 163
        * --------------------------------------------------------------------------- */

        public double[,] rv2rsw
            (
            double[] r, double[] v,
            out double[] rrsw, out double[] vrsw
            )
        {
            double[] rvec = new double[3];
            double[] svec = new double[3];
            double[] wvec = new double[3];
            double[] tempvec = new double[3];
            double[,] transmat = new double[3, 3];

            // --------------------  Implementation   ----------------------
            // in order to work correctly each of the components must be
            // unit vectors
            // radial component
            rvec = MathTimeLibr.norm(r);

            // nMathTimeLibr.cross-track component
            MathTimeLibr.cross(r, v, out tempvec);
            wvec = MathTimeLibr.norm(tempvec);

            // along-track component
            MathTimeLibr.cross(wvec, rvec, out tempvec);
            svec = MathTimeLibr.norm(tempvec);

            // assemble transformation matrix from to rsw frame (individual
            //  components arranged in row vectors)
            transmat[0, 0] = rvec[0];
            transmat[0, 1] = rvec[1];
            transmat[0, 2] = rvec[2];
            transmat[1, 0] = svec[0];
            transmat[1, 1] = svec[1];
            transmat[1, 2] = svec[2];
            transmat[2, 0] = wvec[0];
            transmat[2, 1] = wvec[1];
            transmat[2, 2] = wvec[2];

            rrsw = MathTimeLibr.matvecmult(transmat, r, 3);
            vrsw = MathTimeLibr.matvecmult(transmat, v, 3);

            return transmat;
            /*
            *   alt approach
            *       rrsw[0] = MathTimeLibr.mag(r)
            *       rrsw[2] = 0.0
            *       rrsw[3] = 0.0
            *       vrsw[0] = dot(r, v)/rrsw(0)
            *       vrsw[1] = sqrt(v(0)**2 + v(1)**2 + v(2)**2 - vrsw(0)**2)
            *       vrsw[2] = 0.0
            */
        }  //  rv2rsw 


        /* -----------------------------------------------------------------------------
         *
         *                           function rv2ntw
         *
         *  this function converts position and velocity vectors into normal, in-
         *    track, and cross-track coordinates. 
         *
         *  author        : david vallado           davallado@gmail.com    9 jun 2002
         *
         *  revisions
         *    vallado     - conversion to c#                              23 Nov 2011
         *
         *  inputs          description                           range / units
         *    r           - position vector                           km
         *    v           - velocity vector                           km/s
         *
         *  outputs       :
         *    rntw        - position vector                           km
         *    vntw        - velocity vector                           km/s
         *    transmat    - transformation matrix
         *
         *  locals        :
         *    tempvec     - temporary vector
         *    nvec,tvec,wvec - direction Cosines
         *
         *  coupling      :
         *
         *  references    :
         *    vallado       2013, 164
         * --------------------------------------------------------------------------- */

        public double[,] rv2ntw
            (
            double[] r, double[] v,
            out double[] rntw, out double[] vntw
            )
        {
            double[] nvec = new double[3];
            double[] tvec = new double[3];
            double[] wvec = new double[3];
            double[] tempvec = new double[3];
            double[,] transmat = new double[3, 3];

            // --------------------  Implementation   ----------------------
            // in order to work correctly each of the components must be
            // unit vectors
            // in velocity component
            tvec = MathTimeLibr.norm(v);

            // cross-track component
            MathTimeLibr.cross(r, v, out tempvec);
            wvec = MathTimeLibr.norm(tempvec);

            // along-radial component
            MathTimeLibr.cross(tvec, wvec, out tempvec);
            nvec = MathTimeLibr.norm(tempvec);

            // assemble transformation matrix from to rsw frame (individual
            //  components arranged in row vectors)
            transmat[0, 0] = nvec[0];
            transmat[0, 1] = nvec[1];
            transmat[0, 2] = nvec[2];
            transmat[1, 0] = tvec[0];
            transmat[1, 1] = tvec[1];
            transmat[1, 2] = tvec[2];
            transmat[2, 0] = wvec[0];
            transmat[2, 1] = wvec[1];
            transmat[2, 2] = wvec[2];

            rntw = MathTimeLibr.matvecmult(transmat, r, 3);
            vntw = MathTimeLibr.matvecmult(transmat, v, 3);

            return transmat;
            /*
            *   alt approach
            *       rrsw[0] = MathTimeLibr.mag(r)
            *       rrsw[2] = 0.0
            *       rrsw[3] = 0.0
            *       vrsw[0] = dot(r, v)/rrsw(0)
            *       vrsw[1] = sqrt(v(0)**2 + v(1)**2 + v(2)**2 - vrsw(0)**2)
            *       vrsw[2] = 0.0
            */
        }  //  rv2ntw 

        /* -----------------------------------------------------------------------------
        *
        *                           function rv2pqw
        *
        *  this function finds the pqw vectors given the geocentric equatorial 
        *  position and velocity vectors.  mu is needed if km and m are
        *    both used with the same routine
        *
        *  author        : david vallado           davallado@gmail.com   21 jun 2002
        *
        *  revisions
        *
        *  inputs          description                    range / units
        *    r           - ijk position vector            km or m
        *    v           - ijk velocity vector            km/s or m/s
        *    mu          - gravitational parameter        km3/s2 or m3/s2
        *
        *  outputs       :
        *    rpqw        - pqw position vector            km
        *    vpqw        - pqw velocity vector            km / s
        *
        *  locals        :
        *    hbar        - angular momentum h vector      km2 / s
        *    ebar        - eccentricity     e vector
        *    nbar        - line of nodes    n vector
        *    c1          - v**2 - u/r
        *    rdotv       - r dot v
        *    hk          - hk unit vector
        *    sme         - specfic mechanical energy      km2 / s2
        *    i           - index
        *    p           - semilatus rectum               km
        *    a           - semimajor axis                 km
        *    ecc         - eccentricity
        *    incl        - inclination                    0.0  to Math.PI rad
        *    nu          - true anomaly                   0.0  to 2pi rad
        *    arglat      - argument of latitude      (ci) 0.0  to 2pi rad
        *    truelon     - true longitude            (ce) 0.0  to 2pi rad
        *    lonper      - longitude of periapsis    (ee) 0.0  to 2pi rad
        *    temp        - temporary variable
        *    typeorbit   - type of orbit                  ee, ei, ce, ci
        *
        *  coupling      :
        *    mag         - magnitude of a vector
        *    MathTimeLibr.cross       - MathTimeLibr.cross product of two vectors
        *    angle       - find the angle between two vectors
        *
        *  references    :
        *    vallado       2007, 126, alg 9, ex 2-5
        * --------------------------------------------------------------------------- */

        public void rv2pqw
             (
               double[] r, double[] v,
               out double[] rpqw, out double[] vpqw
             )
        {
            double undefined, small, magr, magv, magn, rdotv, temp, c1, hk, twopi, magh, halfpi;
            double p, ecc, incl, nu, arglat, truelon;
            double sin_nu, cos_nu;
            double[] tempvec = new double[3];
            double[] hbar = new double[3];
            double[] ebar = new double[3];
            double[] nbar = new double[3];
            int i;
            string typeorbit;
            // define these here since setting individually
            rpqw = new double[3];
            vpqw = new double[3];

            twopi = 2.0 * Math.PI;
            halfpi = 0.5 * Math.PI;
            small = 0.00000001;
            undefined = 999999.1;

            // -------------------------  implementation   -----------------
            magr = MathTimeLibr.mag(r);
            magv = MathTimeLibr.mag(v);

            // ------------------  find h n and e vectors   ----------------
            MathTimeLibr.cross(r, v, out hbar);
            magh = MathTimeLibr.mag(hbar);
            if (magh > small)
            {
                nbar[0] = -hbar[1];
                nbar[1] = hbar[0];
                nbar[2] = 0.0;
                magn = MathTimeLibr.mag(nbar);
                c1 = magv * magv - gravConst.mu / magr;
                rdotv = MathTimeLibr.dot(r, v);
                for (i = 0; i <= 2; i++)
                    ebar[i] = (c1 * r[i] - rdotv * v[i]) / gravConst.mu;
                ecc = MathTimeLibr.mag(ebar);

                // ------------  find a e and semi-latus rectum   ----------
                p = magh * magh / gravConst.mu;

                // -----------------  find inclination   -------------------
                hk = hbar[2] / magh;
                incl = Math.Acos(hk);

                // --------  determine type of orbit for later use  --------
                // ------ elliptical, parabolic, hyperbolic inclined -------
                typeorbit = "ei";
                if (ecc < small)
                {
                    // ----------------  circular equatorial ---------------
                    if ((incl < small) | (Math.Abs(incl - Math.PI) < small))
                        typeorbit = "ce";
                    else
                        // --------------  circular inclined ---------------
                        typeorbit = "ci";
                }
                else
                {
                    // --- elliptical, parabolic, hyperbolic equatorial ----
                    if ((incl < small) | (Math.Abs(incl - Math.PI) < small))
                        typeorbit = "ee";
                }


                // ------------  find true anomaly at epoch    -------------
                if (typeorbit[0] == 'e')
                {
                    nu = MathTimeLibr.angle(ebar, r);
                    if (rdotv < 0.0)
                        nu = twopi - nu;
                }
                else
                    nu = undefined;

                // ----  find argument of latitude - circular inclined -----
                if (typeorbit.Equals("ci"))
                {
                    arglat = MathTimeLibr.angle(nbar, r);
                    if (r[2] < 0.0)
                        arglat = twopi - arglat;
                }
                else
                    arglat = undefined;

                // -------- find true longitude - circular equatorial ------
                if ((magr > small) && (typeorbit.Equals("ce")))
                {
                    temp = r[0] / magr;
                    if (Math.Abs(temp) > 1.0)
                        temp = Math.Sign(temp);
                    truelon = Math.Acos(temp);
                    if (r[1] < 0.0)
                        truelon = twopi - truelon;
                    if (incl > halfpi)
                        truelon = twopi - truelon;
                }
                else
                    truelon = undefined;

                // --------------------  implementation   ----------------------
                //       determine what type of orbit is involved and set up the
                //       set up angles for the special cases.
                // -------------------------------------------------------------
                if (ecc < small)
                {
                    // ----------------  circular equatorial  ------------------
                    if (typeorbit.Equals("ce"))
                        nu = truelon;
                    else
                    {
                        // --------------  circular inclined  ------------------
                        nu = arglat;
                    }
                }
            }
            else
            {
                p = undefined;
                ecc = undefined;
                incl = undefined;
                nu = undefined;
                arglat = undefined;
                truelon = undefined;
            }

            if (nu < undefined)
            {
                // ----------  form pqw position and velocity vectors ----------
                sin_nu = Math.Sin(nu);
                cos_nu = Math.Cos(nu);
                temp = p / (1.0 + ecc * cos_nu);
                rpqw[0] = temp * cos_nu;
                rpqw[1] = temp * sin_nu;
                rpqw[2] = 0.0;
                if (Math.Abs(p) < 0.00000001)
                    p = 0.00000001;

                vpqw[0] = -sin_nu * Math.Sqrt(gravConst.mu / p);
                vpqw[1] = (ecc + cos_nu) * Math.Sqrt(gravConst.mu / p);
                vpqw[2] = 0.0;
            }
            else
            {
                rpqw[0] = undefined;
                rpqw[1] = undefined;
                rpqw[2] = undefined;
                vpqw[0] = undefined;
                vpqw[1] = undefined;
                vpqw[2] = undefined;
            }
        }  // rv2pqw


        /* ----------------------------------------------------------------------------
          *
          *                           procedure newtone
          *
          *  this procedure finds the mean and true anomaly given the eccentric anomaly.  
          *
          *  author        : david vallado           davallado@gmail.com   14 feb 2020
          *
          *  inputs          description                    range / units
          *    ecc         - eccentricity                   0.0 to
          *    eccanom     - eccentric anomaly              0.0 to 2pi rad
          *
          *  outputs       :
          *    m           - mean anomaly                   -2pi to 2pi rad
          *    nu          - true anomaly                   0.0 to 2pi rad
          *
          *  locals        :
          *    e1          - eccentric anomaly, next value  rad
          *    sinv        - sine of nu
          *    cosv        - cosine of nu
          *
          *  coupling      :
          *
          *  references    :
          *    vallado       2013, 73, alg 2, ex 2-1
          * ----------------------------------------------------------------- */

        public void newtone
           (
           double ecc, double eccanom, out double m, out double nu
           )
        {
            double small, sinv, cosv;

            // -------------------------  implementation   -----------------
            small = 0.00000001;

            // ------------------------- circular --------------------------
            if (Math.Abs(ecc) < small)
            {
                m = eccanom;
                nu = eccanom;
            }
            else
            {
                // ----------------------- elliptical ----------------------
                if (ecc < 0.999)
                {
                    m = eccanom - ecc * Math.Sin(eccanom);
                    sinv = (Math.Sqrt(1.0 - ecc * ecc) * Math.Sin(eccanom)) / (1.0 - ecc * Math.Cos(eccanom));
                    cosv = (Math.Cos(eccanom) - ecc) / (1.0 - ecc * Math.Cos(eccanom));
                    nu = Math.Atan2(sinv, cosv);
                }
                else
                {

                    // ---------------------- hyperbolic  ------------------
                    if (ecc > 1.0001)
                    {
                        m = ecc * Math.Sinh(eccanom) - eccanom;
                        sinv = (Math.Sqrt(ecc * ecc - 1.0) * Math.Sinh(eccanom)) / (1.0 - ecc * Math.Cosh(eccanom));
                        cosv = (Math.Cosh(eccanom) - ecc) / (1.0 - ecc * Math.Cosh(eccanom));
                        nu = Math.Atan2(sinv, cosv);
                    }
                    else
                    {

                        // -------------------- parabolic ------------------
                        m = eccanom + (1.0 / 3.0) * eccanom * eccanom * eccanom;
                        nu = 2.0 * Math.Atan(eccanom);
                    }
                }
            }

            if (m < 0.0)
                m = m + 2.0 * Math.PI;
            if (nu < 0.0)
                nu = nu + 2.0 * Math.PI;
        }  // newtone


        /* ----------------------------------------------------------------------------
         *
         *                           procedure newtonmx
         *
         *  this procedure performs the newton rhapson iteration to find the
         *    eccentric anomaly given the mean anomaly.  the true anomaly is also
         *    calculated. This is an optimized version from Oltrogge JAS 2015. 
         *
         *  author        : david vallado           davallado@gmail.com    1 mar 2001
         *
         *  inputs          description                    range / units
         *    ecc         - eccentricity                   0.0 to
         *    m           - mean anomaly                   -2pi to 2pi rad
         *
         *  outputs       :
         *    eccanom          - eccentric anomaly              0.0 to 2pi rad
         *    nu          - true anomaly                   0.0 to 2pi rad
         *
         *  locals        :
         *    e1          - eccentric anomaly, next value  rad
         *    sinv        - sine of nu
         *    cosv        - cosine of nu
         *    ktr         - index
         *    r1r         - cubic roots - 1 to 3
         *    r1i         - iMathTimeLibr.maginary component
         *    r2r         -
         *    r2i         -
         *    r3r         -
         *    r3i         -
         *    s           - variables for parabolic solution
         *    w           - variables for parabolic solution
         *
         *  coupling      :
         *    cubic       - solves a cubic polynomial
         *    power       - raises a base number to an arbitrary power
         *
         *  references    :
         *    vallado       2013, 73, alg 2, ex 2-1
         *    oltrogge      JAS 2015
         * ----------------------------------------------------------------- */

        public void newtonmx
            (
            double ecc, double m, out double eccanom, out double nu
            )
        {
            double numiter, small, halfpi, ktr, sinv, cosv, s, w, e1;
            double so, s1, s2, alp, bet, z2, fp, f1p, f2p, cosE, sinE;

            // -------------------------  implementation   -----------------
            numiter = 50;
            small = 0.00000001;
            halfpi = Math.PI * 0.5;

            // -------------------------- hyperbolic  ----------------------
            if ((ecc - 1.0) > small)
            {
                // -------------------  initial guess -----------------------
                if (ecc < 1.6)
                {
                    if (((m < 0.0) && (m > -Math.PI)) || (m > Math.PI))
                        eccanom = m - ecc;
                    else
                        eccanom = m + ecc;
                }
                else
                {
                    if ((ecc < 3.6) && (Math.Abs(m) > Math.PI))
                        eccanom = m - Math.Sign(m) * ecc;
                    else
                        eccanom = m / (ecc - 1.0);
                }

                ktr = 1;
                e1 = eccanom + ((m - ecc * Math.Sinh(eccanom) + eccanom) / (ecc * Math.Cosh(eccanom) - 1.0));
                while ((Math.Abs(e1 - eccanom) > small) && (ktr <= numiter))
                {
                    eccanom = e1;
                    e1 = eccanom + ((m - ecc * Math.Sinh(eccanom) + eccanom) / (ecc * Math.Cosh(eccanom) - 1.0));
                    ktr = ktr + 1;
                }

                eccanom = e1;

                // ----------------  find true anomaly  --------------------
                sinv = -(Math.Sqrt(ecc * ecc - 1.0) * Math.Sinh(e1)) / (1.0 - ecc * Math.Cosh(e1));
                cosv = (Math.Cosh(e1) - ecc) / (1.0 - ecc * Math.Cosh(e1));
                nu = Math.Atan2(sinv, cosv);
            }
            else
            {
                // --------------------- parabolic -------------------------
                if (Math.Abs(ecc - 1.0) < small)
                {
                    //                c = [ 1.0/3.0  0.0  1.0  -m] 
                    //                [r1r] = roots (c) 
                    //                eccanom= r1r 
                    s = 0.5 * (halfpi - Math.Atan(1.5 * m));
                    w = Math.Atan(Math.Pow(Math.Tan(s), 1.0 / 3.0));
                    eccanom = 2.0 * Math.Tan(halfpi - 2.0 * w);  // really cot()
                    ktr = 1;
                    nu = 2.0 * Math.Atan(eccanom);
                }
                else
                {
                    // -------------------- elliptical ----------------------
                    if (ecc > small)
                    {
                        double temp = 1.0 / (4.0 * ecc + 0.5);
                        alp = (1.0 - ecc) * temp;
                        bet = 0.5 * m * temp;
                        z2 = Math.Pow(bet + Math.Sqrt(alp * alp * alp + bet * bet), 2.0 / 3.0);
                        so = 2.0 * bet / (z2 + alp + alp * alp / (z2));
                        s1 = so * (1.0 - 0.07925 * Math.Pow(so, 5) / (1.0 + ecc));
                        fp = 3.0 * Math.Asin(s1) - ecc * s1 * (3.0 - 4.0 * s1 * s1) - m;
                        f1p = 3.0 / Math.Sqrt(1.0 - s1 * s1) + ecc * (12.0 * s1 * s1 - 3.0);
                        f2p = s1 * (24.0 * ecc + 3.0 / Math.Pow(1.0 - s1 * s1, 1.5)) * s1;

                        int n = 3;
                        s2 = s1 - n * fp / (f1p + Math.Sqrt((n - 1) * (n - 1) * f1p * f1p - n * (n - 1.0) * fp * f2p));
                        cosE = 1.0 - Math.Abs(1.0 - Math.Sqrt(1.0 - s2 * s2) * (1.0 - 4.0 * s2 * s2));
                        // sign(m) determines correct quadrant
                        sinE = Math.Sign(m) * s2 * (3.0 - 4.0 * s2 * s2);

                        // --------  find eccentric anomaly  -----------
                        eccanom = Math.Atan2(sinE, cosE);

                        // -------------  find true anomaly  ---------------
                        sinv = (Math.Sqrt(1.0 - ecc * ecc) * sinE) / (1.0 - ecc * cosE);
                        cosv = (cosE - ecc) / (1.0 - ecc * cosE);
                        nu = Math.Atan2(sinv, cosv);
                    }
                    else
                    {
                        // -------------------- circular -------------------
                        ktr = 0;
                        nu = m;
                        eccanom = m;
                    } // if ecc > small
                }  // if abs()
            }
        } // newtonmx


        /* ----------------------------------------------------------------------------
        *
        *                           procedure newtonm
        *
        *  this procedure performs the newton rhapson iteration to find the
        *    eccentric anomaly given the mean anomaly.  the true anomaly is also
        *    calculated.
        *
        *  author        : david vallado           davallado@gmail.com    1 mar 2001
        *
        *  inputs          description                    range / units
        *    ecc         - eccentricity                   0.0 to
        *    m           - mean anomaly                   -2pi to 2pi rad
        *
        *  outputs       :
        *    eccanom          - eccentric anomaly              0.0 to 2pi rad
        *    nu          - true anomaly                   0.0 to 2pi rad
        *
        *  locals        :
        *    e1          - eccentric anomaly, next value  rad
        *    Math.Sinv        - Math.Sine of nu
        *    Math.Cosv        - Math.Cosine of nu
        *    ktr         - index
        *    r1r         - cubic roots - 1 to 3
        *    r1i         - iMathTimeLibr.maginary component
        *    r2r         -
        *    r2i         -
        *    r3r         -
        *    r3i         -
        *    s           - variables for parabolic solution
        *    w           - variables for parabolic solution
        *
        *  coupling      :
        *    Math.Atan2       - arc tangent function which also resloves quadrants
        *    cubic       - solves a cubic polynomial
        *    power       - raises a base number to an arbitrary power
        *    Math.Sinh        - hyperbolic Math.Sine
        *    Math.Cosh        - hyperbolic Math.Cosine
        *    Math.Sign         - returns the sign of an argument
        *
        *  references    :
        *    vallado       2013, 73, alg 2, ex 2-1
        * ----------------------------------------------------------------- */

        public void newtonm
            (
            double ecc, double m, out double eccanom, out double nu
            )
        {
            double numiter, small, halfpi, ktr, sinv, cosv, s, w, e1;

            // -------------------------  implementation   -----------------
            numiter = 50;
            small = 0.00000001;
            halfpi = Math.PI * 0.5;

            // -------------------------- hyperbolic  ----------------------
            if ((ecc - 1.0) > small)
            {
                // -------------------  initial guess -----------------------
                if (ecc < 1.6)
                {
                    if (((m < 0.0) && (m > -Math.PI)) || (m > Math.PI))
                        eccanom = m - ecc;
                    else
                        eccanom = m + ecc;
                }
                else
                {
                    if ((ecc < 3.6) && (Math.Abs(m) > Math.PI))
                        eccanom = m - Math.Sign(m) * ecc;
                    else
                        eccanom = m / (ecc - 1.0);
                }

                ktr = 1;
                e1 = eccanom + ((m - ecc * Math.Sinh(eccanom) + eccanom) / (ecc * Math.Cosh(eccanom) - 1.0));
                while ((Math.Abs(e1 - eccanom) > small) && (ktr <= numiter))
                {
                    eccanom = e1;
                    e1 = eccanom + ((m - ecc * Math.Sinh(eccanom) + eccanom) / (ecc * Math.Cosh(eccanom) - 1.0));
                    ktr = ktr + 1;
                }
                // ----------------  find true anomaly  --------------------
                sinv = -(Math.Sqrt(ecc * ecc - 1.0) * Math.Sinh(e1)) / (1.0 - ecc * Math.Cosh(e1));
                cosv = (Math.Cosh(e1) - ecc) / (1.0 - ecc * Math.Cosh(e1));
                nu = Math.Atan2(sinv, cosv);
            }
            else
            {
                // --------------------- parabolic -------------------------
                if (Math.Abs(ecc - 1.0) < small)
                {
                    //                c = [ 1.0/3.0  0.0  1.0  -m] 
                    //                [r1r] = roots (c) 
                    //                eccanom= r1r 
                    s = 0.5 * (halfpi - Math.Atan(1.5 * m));
                    w = Math.Atan(Math.Pow(Math.Tan(s), 1.0 / 3.0));
                    eccanom = 2.0 * Math.Tan(halfpi - 2.0 * w);  // really cot()
                    ktr = 1;
                    nu = 2.0 * Math.Atan(eccanom);
                }
                else
                {
                    // -------------------- elliptical ----------------------
                    if (ecc > small)
                    {
                        // -----------  initial guess -------------
                        if (((m < 0.0) && (m > -Math.PI)) || (m > Math.PI))
                            eccanom = m - ecc;
                        else
                            eccanom = m + ecc;
                        ktr = 1;
                        e1 = eccanom + (m - eccanom + ecc * Math.Sin(eccanom)) / (1.0 - ecc * Math.Cos(eccanom));

                        while ((Math.Abs(e1 - eccanom) > small) && (ktr <= numiter))
                        {
                            ktr = ktr + 1;
                            eccanom = e1;
                            e1 = eccanom + (m - eccanom + ecc * Math.Sin(eccanom)) / (1.0 - ecc * Math.Cos(eccanom));
                        }
                        // -------------  find true anomaly  ---------------
                        sinv = (Math.Sqrt(1.0 - ecc * ecc) * Math.Sin(e1)) / (1.0 - ecc * Math.Cos(e1));
                        cosv = (Math.Cos(e1) - ecc) / (1.0 - ecc * Math.Cos(e1));
                        nu = Math.Atan2(sinv, cosv);
                    }
                    else
                    {
                        // -------------------- circular -------------------
                        ktr = 0;
                        nu = m;
                        eccanom = m;
                    } // if ecc > small
                }  // if abs()
            }
            if (m < 0.0)
                m = m + 2.0 * Math.PI;
            if (nu < 0.0)
                nu = nu + 2.0 * Math.PI;
        } // newtonm


        /* -----------------------------------------------------------------------------
        *
        *                           function newtonnu
        *
        *  this function solves keplers equation when the true anomaly is known.
        *    the mean and eccentric, parabolic, or hyperbolic anomaly is also found.
        *    the parabolic limit at 168ø is arbitrary. the hyperbolic anomaly is also
        *    limited. the hyperbolic Math.Sine is used because it's not double valued.
        *
        *  author        : david vallado           davallado@gmail.com   27 may 2002
        *
        *  revisions
        *    vallado     - fix small                                     24 sep 2002
        *
        *  inputs          description                    range / units
        *    ecc         - eccentricity                   0.0  to
        *    nu          - true anomaly                   -2pi to 2pi rad
        *
        *  outputs       :
        *    eccanom          - eccentric anomaly              0.0  to 2pi rad       153.02 ø
        *    m           - mean anomaly                   0.0  to 2pi rad       151.7425 ø
        *
        *  locals        :
        *    e1          - eccentric anomaly, next value  rad
        *    sine        - sine of e
        *    cose        - cosine of e
        *    ktr         - index
        *
        *  coupling      :
        *    arcsinh     - arc hyperbolic Math.Sine
        *    Math.Sinh        - hyperbolic Math.Sine
        *
        *  references    :
        *    vallado       2007, 85, alg 5
        * --------------------------------------------------------------------------- */

        public void newtonnu
            (
            double ecc, double nu, out double eccanom, out double m
            )
        {
            double small, sine, cose;

            // ---------------------  implementation   ---------------------
            eccanom = 999999.9;
            m = 999999.9;
            small = 0.00000001;

            // --------------------------- circular ------------------------
            if (Math.Abs(ecc) < small)
            {
                m = nu;
                eccanom = nu;
            }
            else
            {
                // ---------------------- elliptical -----------------------
                if (ecc < 1.0 - small)
                {
                    sine = (Math.Sqrt(1.0 - ecc * ecc) * Math.Sin(nu)) / (1.0 + ecc * Math.Cos(nu));
                    cose = (ecc + Math.Cos(nu)) / (1.0 + ecc * Math.Cos(nu));
                    eccanom = Math.Atan2(sine, cose);
                    m = eccanom - ecc * Math.Sin(eccanom);
                }
                else
                {
                    // -------------------- hyperbolic  --------------------
                    if (ecc > 1.0 + small)
                    {
                        if ((ecc > 1.0) && (Math.Abs(nu) + 0.00001 < Math.PI - Math.Acos(1.0 / ecc)))
                        {
                            sine = (Math.Sqrt(ecc * ecc - 1.0) * Math.Sin(nu)) / (1.0 + ecc * Math.Cos(nu));
                            eccanom = MathTimeLibr.asinh(sine);
                            m = ecc * Math.Sinh(eccanom) - eccanom;
                        }
                    }
                    else
                    {
                        // ----------------- parabolic ---------------------
                        if (Math.Abs(nu) < 168.0 * Math.PI / 180.0)
                        {
                            eccanom = Math.Tan(nu * 0.5);
                            m = eccanom + (eccanom * eccanom * eccanom) / 3.0;
                        }
                    }
                }
            }
            if (ecc < 1.0)
            {
                m = m - Math.Floor(m / (2.0 * Math.PI)) * (2.0 * Math.PI);
                if (m < 0.0)
                {
                    m = m + 2.0 * Math.PI;
                }
                eccanom = eccanom - Math.Floor(eccanom / (2.0 * Math.PI)) * (2.0 * Math.PI);
            }
            if (m < 0.0)
                m = m + 2.0 * Math.PI;
            if (eccanom < 0.0)
                eccanom = eccanom + 2.0 * Math.PI;
        } // newtonnu


        /* -----------------------------------------------------------------------------
        *
        *                           function lon2nu
        *
        *  this function finds the true anomaly from coes and longitude.
        *
        *  author        : david vallado           davallado@gmail.com    1 mar 2011
        *
        *  revisions
        *    vallado     - conversion to c#                              23 Nov 2011
        *    
        *  inputs          description                    range / units
        *    jdut1       - julian date in ut1             days from 4713 bc
        *    lon         - longitude                      0 to 2pi rad
        *    incl        - inclination                    0 to 2pi rad
        *    raan        - right ascenion of the node     0 to 2pi rad
        *    argp        - argument of perigee            0 to 2pi rad
        *
        *  outputs       :
        *    nu          - true anomaly                   0 to 2pi rad
        *
        *  locals        :
        *    temp        - temporary variable for doubles   rad
        *    tut1        - julian centuries from the
        *                  jan 1, 2000 12 h epoch (ut1)
        *
        *  coupling      :
        *    none
        *
        *  references    :
        *    vallado       2013, 110, eq 2-101
        * --------------------------------------------------------------------------- */

        public double lon2nu
            (
            double jdut1, double lon, double incl, double raan, double argp, out string strtext
            )
        {
            double rad, twopi, ed, gmst, lambdau, temp, arglat;

            rad = 180.0 / Math.PI;
            twopi = 2.0 * Math.PI;

            // eutelsat assumption, use their GMST calculation
            ed = jdut1 - 2451544.5;  // elapsed days from 1 jan 2000 0 hrs
            gmst = 99.96779469 + 360.9856473662860 * ed + 0.29079e-12 * ed * ed;  // deg
            gmst = (gmst / rad) % twopi;

            // ------------------------ check quadrants --------------------
            if (gmst < 0.0)
            {
                gmst = gmst + twopi;
            }

            lambdau = gmst + lon - raan;
            strtext = (gmst * rad).ToString() + " lu " + (lambdau * rad).ToString();

            // make sure lambdau is 0 to 360 deg
            if (lambdau < 0.0)
            {
                lambdau = lambdau + twopi;
            }
            if (lambdau > twopi)
            {
                lambdau = lambdau - twopi;
            }

            arglat = Math.Atan(Math.Tan(lambdau) / Math.Cos(incl));

            strtext = strtext + " " + (lambdau * rad).ToString() + " al " + (arglat * rad).ToString();

            if (arglat < 0.0)
            {
                arglat = arglat + twopi;
            }

            // find arglat - should be in the same quadrant as lambdau  
            if (Math.Abs(lambdau - arglat) > Math.PI * 0.5)
            {
                arglat = arglat + 0.5 * Math.PI * Math.Floor(0.5 + (lambdau - arglat) / (0.5 * Math.PI));
            }

            // find nu     
            temp = arglat - argp;
            strtext = strtext + " " + (arglat * rad).ToString();
            return temp;

            //if (printopt == 'y')
            //    Console.WriteLine( String.Format("gmst  lu  nu  arglat = {0}  {1}  {2}  {3}  ", gmst * rad, lambdau * rad, temp * rad, arglat * rad));

        }  // lon2nu  




        /* -----------------------------------------------------------------------------
        *
        *                           function findc2c3
        *
        *  this function calculates the c2 and c3 functions for use in the universal
        *    variable calculation of z.
        *
        *  author        : david vallado           davallado@gmail.com   27 may 2002
        *
        *  revisions
        *                -
        *
        *  inputs          description                    range / units
        *    znew        - z variable                     rad2
        *
        *  outputs       :
        *    c2new       - c2 function value
        *    c3new       - c3 function value
        *
        *  locals        :
        *    sqrtz       - square root of znew
        *
        *  coupling      :
        *    Math.Sinh        - hyperbolic Math.Sine
        *    Math.Cosh        - hyperbolic Math.Cosine
        *
        *  references    :
        *    vallado       2013, 63, alg 1
        * --------------------------------------------------------------------------- */

        public void findc2c3
            (
            double znew,
            out double c2new, out double c3new
            )
        {
            double small, sqrtz;
            small = 0.00000001;

            // -------------------------  implementation   -----------------
            if (znew > small)
            {
                sqrtz = Math.Sqrt(znew);
                c2new = (1.0 - Math.Cos(sqrtz)) / znew;
                c3new = (sqrtz - Math.Sin(sqrtz)) / (sqrtz * sqrtz * sqrtz);
            }
            else
            {
                if (znew < -small)
                {
                    sqrtz = Math.Sqrt(-znew);
                    c2new = (1.0 - Math.Cosh(sqrtz)) / znew;
                    c3new = (Math.Sinh(sqrtz) - sqrtz) / (sqrtz * sqrtz * sqrtz);
                }
                else
                {
                    c2new = 0.5;
                    c3new = 1.0 / 6.0;
                }
            }
        }  //  findc2c3



        /* -----------------------------------------------------------------------------
        *
        *                           function findfandg
        *
        *  this function calculates the f and g functions for use in various applications. 
        *  several methods are available. the values are in normal (not canonical) units.
        *  note that not all the input parameters are needed for each case. also, the step
        *  size dtsec should be small, perhaps on the order of 60-120 secs!
        *
        *  author        : david vallado           davallado@gmail.com   27 jan 2020
        *
        *  inputs          description                    range / units
        *    r1          - position vector                     km
        *    v1          - velocity vector                     km/s
        *    r2          - position vector                     km
        *    v2          - velocity vector                     km/s
        *    x           - universal variable
        *    c2          - stumpff function
        *    c3          - stumpff function
        *    dtsec       - step size                          sec (SMALL time steps only!!)
        *    opt         - calculation method                 pqw, series, c2c3
        *    
        *  outputs       :
        *    f, g        - f and g functions                 
        *    fdot, gdot  - fdot and gdot functions
        *
        *  locals        :
        *                -
        *  coupling      :
        *
        *  references    :
        *    vallado       2013, 83, 87, 813
        *  findfandg(r1, v1t, r2, v2t, 0.0, 0.0, 0.0, 0.0, 0.0, "pqw", out f, out g, out fdot, out gdot);
        *  findfandg(r1, v1t, r2, v2t, dtsec, 0.0, 0.0, 0.0, 0.0, "series", out f, out g, out fdot, out gdot);
        *  findfandg(r1, v1t, r2, v2t, dtsec, 0.35987, 0.6437, 0.2378, -0.0239, "c2c3", out f, out g, out fdot, out gdot);
        * --------------------------------------------------------------------------- */
        public void findfandg
            (
            double[] r1, double[] v1, double[] r2, double[] v2, double dtsec,
            double x, double c2, double c3, double z,
            string opt,
            out double f, out double g, out double fdot, out double gdot
            )
        {
            double magr1, magv1;
            f = 0.0;
            g = 0.0;
            gdot = 0.0;

            magr1 = MathTimeLibr.mag(r1);
            magv1 = MathTimeLibr.mag(v1);

            // -------------------------  implementation   -----------------
            switch (opt)
            {
                case "pqw":
                    double h;
                    double[] hbar = new double[3];
                    double[] rpqw1 = new double[3];
                    double[] rpqw2 = new double[3];
                    double[] vpqw1 = new double[3];
                    double[] vpqw2 = new double[3];
                    MathTimeLibr.cross(r1, v1, out hbar);
                    h = MathTimeLibr.mag(hbar);
                    // find vectors in PQW frame
                    rv2pqw(r1, v1, out rpqw1, out vpqw1);
                    // find vectors in PQW frame
                    rv2pqw(r2, v2, out rpqw2, out vpqw2);

                    // normal units
                    f = (rpqw2[0] * vpqw1[1] - vpqw2[0] * rpqw1[1]) / h;
                    g = (rpqw1[0] * rpqw2[1] - rpqw2[0] * rpqw1[1]) / h;
                    gdot = (rpqw1[0] * vpqw2[1] - vpqw2[0] * rpqw1[1]) / h;
                    fdot = (vpqw2[0] * vpqw1[1] - vpqw2[1] * vpqw1[0]) / h;
                    break;

                case "series":
                    double u, p, q;
                    u = gravConst.mu / (magr1 * magr1 * magr1);
                    p = MathTimeLibr.dot(r1, v1) / (magr1 * magr1);
                    q = (magv1 * magv1 - magr1 * magr1 * u) / (magr1 * magr1);
                    double p2 = p * p;
                    double p3 = p2 * p;
                    double p4 = p3 * p;
                    double p5 = p4 * p;
                    double p6 = p5 * p;
                    double u2 = u * u;
                    double u3 = u2 * u;
                    double u4 = u3 * u;
                    double u5 = u4 * u;
                    double q2 = q * q;
                    double q3 = q2 * q;
                    double dt2 = dtsec * dtsec;
                    double dt3 = dt2 * dtsec;
                    double dt4 = dt3 * dtsec;
                    double dt5 = dt4 * dtsec;
                    double dt6 = dt5 * dtsec;
                    double dt7 = dt6 * dtsec;
                    double dt8 = dt7 * dtsec;
                    f = 1.0 - 0.5 * u * dt2 + 0.5 * u * p * dt3
                        + u / 24.0 * (-15 * p2 + 3.0 * q + u) * dt4
                        + p * u / 8.0 * (7.0 * p2 - 3 * q - u) * dt5
                        + u / 720.0 * (-945.0 * p4 + 630.0 * p2 * q + 210 * u * p2 - 45 * q2 - 24 * u * q - u2) * dt6
                        + p * u / 80.0 * (165 * p4 - 150 * p2 * q - 50 * u * p2 + 25 * q2 + 14.0 * u * q + u2) * dt7
                        + u / 40320.0 * (-135135.0 * p6 + 155925.0 * p4 * q + 51975.0 * u * p4 - 42525 * p2 * q2
                        - 24570.0 * u * p2 * q - 2205 * u2 * p2 + 1575 * q3 + 1107.0 * u * q2 + 117.0 * u2 * q + u3) * dt8;

                    g = dtsec - 1.0 / 6.0 * u * dt3 + 0.25 * u * p * dt4
                        + u / 120.0 * (-45 * p2 + 9.0 * q + u) * dt5
                        + p * u / 24.0 * (14.0 * p2 - 6 * q - u) * dt6
                        + u / 5040.0 * (-4725 * p4 + 3150 * p2 * q + 630 * u * p2 - 225 * q2 - 54 * u * q - u2) * dt7
                        + p * u / 320.0 * (495 * p4 - 450 * p2 * q - 100 * u * p2 + 75 * q2 + 24.0 * u * q + u2) * dt8;

                    fdot = -u * dtsec + 1.5 * u * p * dt2
                        + u / 6.0 * (-15 * p2 + 3 * q + u) * dt3
                        + 5.0 * p * u / 8.0 * (7.0 * p2 - 3 * q - u) * dt4
                        + u / 120.0 * (-945 * p4 + 630 * p2 * q + 210 * u * p2 - 45 * q2 - 24 * u * q - u2) * dt5
                        + 7.0 * p * u / 80.0 * (165 * p4 - 150 * p2 * q - 50 * u * p2 + 25 * q2 + 14 * u * q + u2) * dt6
                        + u / 5040.0 * (-135135.0 * p6 + 155925.0 * p4 * q + 51975.0 * u * p4 - 42525.0 * p2 * q2 - 24570.0 * u * p2 * q
                        - 2205.0 * u2 * p2 + 1575 * q3 + 1107 * u * q2 + 117 * u2 * q + u3) * dt7;

                    gdot = 1.0 - 0.5 * u * dt2 + u * p * dt3
                        + u / 24 * (-45 * p2 + 9 * q + u) * dt4
                        + p * u / 4 * (14 * p2 - 6 * q - u) * dt5
                        + u / 720 * (-4725.0 * p4 + 3150 * p2 * q + 630 * u * p2 - 225 * q2 - 54 * u * q - u2) * dt6
                        + p * u / 40 * (495 * p4 - 450 * p2 * q - 100 * u * p2 + 75 * q2 + 24 * u * q + u2) * dt7;

                    //f = dt8* u* (-135135 * p6 + 155925 * p4* q + 51975 * p4* u - 42525 * p2 * q2 - 24570 * p2 * q * u - 2205 * p2 * u2 
                    //+ 1575 * q3 + 1107 * q2 * u + 117 * q * u2 + u3) / 40320 + dt7 * p * u * (165 * p4 - 150 * p2 * q - 50 * p2 * u 
                    //+ 25 * q2 + 14 * q * u + u2) / 80 + dt6* u * (-945 * p4 + 630 * p2 * q + 210 * p2 * u - 45 * q2 - 24 * q * u - u2) / 
                    //720 + dt5 * p * u * (7 * p2 - 3 * q - u) / 8 + dt4 * u * (-15 * p2 + 3 * q + u) / 24 + dt3 * p * u / 2 - dt2 * u / 2 + 1;
                    //g = dt8 * p * u * (495 * p4 - 450 * p2 * q - 100 * p2 * u + 75 * q2 + 24 * q * u + u2) / 320 + dt7 * u * (-4725 * p4 
                    //+ 3150 * p2 * q + 630 * p2 * u - 225 * q2 - 54 * q * u - u2) / 5040 + dt6 * p * u * (14 * p2 - 6 * q - u) / 24 + dt5 * u * (-45 * p2 + 9 * q + u) / 120 
                    //+ dt4 * p * u / 4 - dt3 * u / 6 + dtsec;
                    //fdot = dt7 * u * (-135135 * p6 + 155925 * p4 * q + 51975 * p4 * u - 42525 * p2 * q2 - 24570 * p2 * q * u - 2205 * p2 * u2 + 1575 * q3 + 1107 * q2 * u 
                    //+ 117 * q * u2 + u3) / 5040 + 7 * dt6 * p * u * (165 * p4 - 150 * p2 * q - 50 * p2 * u + 25 * q2 + 14 * q * u + u2) / 80 + dt5 * u * (-945 * p4 + 630 * p2 * q 
                    //+ 210 * p2 * u - 45 * q2 - 24 * q * u - u2) / 120 + 5 * dt4 * p * u * (7 * p2 - 3 * q - u) / 8 + dt3 * u * (-15 * p2 + 3 * q + u) / 6 + 3 * dt2 * p * u / 2 - dtsec * u;
                    //gdot = dt7 * p * u * (495 * p4 - 450 * p2 * q - 100 * p2 * u + 75 * q2 + 24 * q * u + u2) / 40 + dt6 * u * (-4725 * p4 + 3150 * p2 * q + 630 * p2 * u - 225 * q2 - 54 * q * u - u2) / 720 
                    //+ dt5 * p * u * (14 * p2 - 6 * q - u) / 4 + dt4 * u * (-45 * p2 + 9 * q + u) / 24 + dt3 * p * u - dt2 * u / 2 + 1;

                    break;
                case "c2c3":
                    double xsqrd = x * x;
                    double magr2 = MathTimeLibr.mag(r2);
                    f = 1.0 - (xsqrd * c2 / magr1);
                    g = dtsec - xsqrd * x * c3 / Math.Sqrt(gravConst.mu);
                    gdot = 1.0 - (xsqrd * c2 / magr2);
                    fdot = (Math.Sqrt(gravConst.mu) * x / (magr1 * magr2)) * (z * c3 - 1.0);
                    break;
                default:
                    f = 0.0;
                    g = 0.0;
                    fdot = 0.0;
                    gdot = 0.0;
                    break;
            }
            // g = g * tusec;  / to canonical if needed, fdot/tusec too
            //fdot = (f * gdot - 1.0) / g;

        }  //  findfandg


        /* -----------------------------------------------------------------------------
        *
        *                           function iterateuniversalX
        *
        *  this function iterates to find the universal variable
         ----------------------------------------------------------------------------- */
        public void iterateuniversalX
        (
            double alpha, double dtsec, double rdotv, double magr,
            double[] r, double[] v,
            out int ktr, out double c2new, out double c3new, out double xnew, out double znew
        )
        {
            int numiter, mulrev;
            double[] h = new double[3];
            double rval, xold, xoldsqrd, p, dtnew, a, period, s, w, temp, magh;
            double small, twopi, halfpi;
            char show;

            show = 'n';
            xnew = 0.0;
            znew = 0.0;
            c2new = 0.0;
            c3new = 0.0;

            small = 0.000000001;
            twopi = 2.0 * Math.PI;
            halfpi = Math.PI * 0.5;

            // -------------------------  implementation   -----------------
            // set constants and intermediate printouts
            numiter = 50;
            a = 1.0 / alpha;

            // ------------   setup initial guess for x  ---------------
            // -----------------  circle and ellipse -------------------
            if (alpha >= small)
            {
                period = twopi * Math.Sqrt(Math.Abs(a * a * a) / gravConst.mu);
                // ------- next if needed for 2body multi-rev ----------
                if (Math.Abs(dtsec) > Math.Abs(period))
                    // including the truncation will produce vertical lines that are parallel
                    // (plotting chi vs time)
                    //                    dtsec = rem( dtseco,period );
                    mulrev = Convert.ToInt16(dtsec / period);
                if (Math.Abs(alpha - 1.0) > small)
                    xold = Math.Sqrt(gravConst.mu) * dtsec * alpha;
                else
                    // - first guess can't be too close. ie a circle, r2=a
                    xold = Math.Sqrt(gravConst.mu) * dtsec * alpha * 0.97;
            }
            else
            {
                // --------------------  parabola  ---------------------
                if (Math.Abs(alpha) < small)
                {
                    MathTimeLibr.cross(r, v, out h);
                    magh = MathTimeLibr.mag(h);
                    p = magh * magh / gravConst.mu;
                    s = 0.5 * (halfpi - Math.Atan(3.0 * Math.Sqrt(gravConst.mu / (p * p * p)) * dtsec));
                    w = Math.Atan(Math.Pow(Math.Tan(s), (1.0 / 3.0)));
                    xold = Math.Sqrt(p) * (2.0 * MathTimeLibr.cot(2.0 * w));
                    alpha = 0.0;
                }
                else
                {
                    // ------------------  hyperbola  ------------------
                    temp = -2.0 * gravConst.mu * dtsec /
                          (a * (rdotv + Math.Sign(dtsec) * Math.Sqrt(-gravConst.mu * a) *
                          (1.0 - magr * alpha)));
                    xold = Math.Sign(dtsec) * Math.Sqrt(-a) * Math.Log(temp);
                }
            } // if alpha

            ktr = 1;
            dtnew = -10.0;
            double tmp = 1.0 / Math.Sqrt(gravConst.mu);
            while ((Math.Abs(dtnew * tmp - dtsec) >= small) && (ktr < numiter))
            {
                xoldsqrd = xold * xold;
                znew = xoldsqrd * alpha;

                // ------------- find c2 and c3 functions --------------
                findc2c3(znew, out c2new, out c3new);

                // ------- use a newton iteration for new values -------
                rval = xoldsqrd * c2new + rdotv * tmp * xold * (1.0 - znew * c3new)
                    + magr * (1.0 - znew * c2new);
                dtnew = xoldsqrd * xold * c3new + rdotv * tmp * xoldsqrd * c2new
                    + magr * xold * (1.0 - znew * c3new);

                // ------------- calculate new value for x -------------
                xnew = xold + (dtsec * Math.Sqrt(gravConst.mu) - dtnew) / rval;

                // ----- check if the univ param goes negative. if so, use bissection
                if (xnew < 0.0)
                    xnew = xold * 0.5;

                if (show == 'y')
                {
                    //  printf("%3i %11.7f %11.7f %11.7f %11.7f %11.7f \n", ktr,xold,znew,rval,xnew,dtnew);
                    //  printf("%3i %11.7f %11.7f %11.7f %11.7f %11.7f \n", ktr,xold/sqrt(re),znew,rval/re,xnew/sqrt(re),dtnew/sqrt(mu));
                }

                ktr = ktr + 1;
                xold = xnew;
            }  // while
        }


        /* ------------------------------------------------------------------------------
        *
        *                           function kepler
        *
        *  this function solves keplers problem for orbit determination and returns a
        *    future geocentric equatorial (ijk) position and velocity vector.  the
        *    solution uses universal variables.
        *
        *  author        : david vallado           davallado@gmail.com   22 jun 2002
        *
        *  revisions
        *    vallado     - fix some mistakes                             13 apr 2004
        *
        *  inputs          description                              range / units
        *    r1          - ijk position vector - initial            km
        *    vo          - ijk velocity vector - initial            km / s
        *    dtsec       - length of time to propagate              s
        *
        *  outputs       :
        *    r           - ijk position vector                      km
        *    v           - ijk velocity vector                      km / s
        *    error       - error flag                               'ok',  
        *
        *  locals        :
        *    f           - f expression
        *    g           - g expression
        *    fdot        - f dot expression
        *    gdot        - g dot expression
        *    xold        - old universal variable x
        *    xoldsqrd    - xold squared
        *    xnew        - new universal variable x
        *    xnewsqrd    - xnew squared
        *    znew        - new value of z
        *    c2new       - c2(psi) function
        *    c3new       - c3(psi) function
        *    dtsec       - change in time                           s
        *    timenew     - new time                                 s
        *    rdotv       - result of r1 dot vo
        *    a           - semi or axis                             km
        *    alpha       - reciprocol  1/a
        *    sme         - specific mech energy                     km2 / s2
        *    period      - time period for satellite                s
        *    s           - variable for parabolic case
        *    w           - variable for parabolic case
        *    h           - angular momentum vector
        *    temp        - temporary real*8 value
        *    i           - index
        *
        *  coupling      :
        *    mag         - magnitude of a vector
        *    findc2c3    - find c2 and c3 functions
        *
        *  references    :
        *    vallado       2004, 95-103, alg 8, ex 2-4
         ------------------------------------------------------------------------------ */

        public void kepler
            (
            double[] r1, double[] vo, double dtsec, out double[] r2, out double[] v
            )
        {
            // -------------------------  implementation   -----------------
            int ktr, i, numiter, mulrev;
            double[] h = new double[3];
            double[] rx = new double[3];
            double[] vx = new double[3];
            double f, g, fdot, gdot, rval, xold, xoldsqrd, xnew,
                  xnewsqrd, znew, p, c2new, c3new, dtnew, rdotv, a,
                  alpha, sme, period, s, w, temp, magro, magvo, magh, magr, magv;
            char show;
            //string errork;

            show = 'n';
            double small, twopi, halfpi;

            for (int ii = 0; ii < 3; ii++)
            {
                rx[ii] = 0.0;
                vx[ii] = 0.0;
            }
            r2 = rx;  // seems to be the only way to get these variables out
            v = vx;
            xnew = 0.0;
            c2new = 0.0;
            c3new = 0.0;

            small = 0.000000001;
            twopi = 2.0 * Math.PI;
            halfpi = Math.PI * 0.5;

            // -------------------------  implementation   -----------------
            // set constants and intermediate printouts
            numiter = 50;

            if (show == 'y')
            {
                //            printf(" r1 %16.8f %16.8f %16.8f ER \n",r1[0]/re,r1[1]/re,r1[2]/re );
                //            printf(" vo %16.8f %16.8f %16.8f ER/TU \n",vo[0]/velkmps, vo[1]/velkmps, vo[2]/velkmps );
            }

            // --------------------  initialize values   -------------------
            ktr = 0;
            xold = 0.0;
            znew = 0.0;
            mulrev = 0;

            if (Math.Abs(dtsec) > small)
            {
                magro = MathTimeLibr.mag(r1);
                magvo = MathTimeLibr.mag(vo);
                rdotv = MathTimeLibr.dot(r1, vo);

                // -------------  find sme, alpha, and a  ------------------
                sme = ((magvo * magvo) * 0.5) - (gravConst.mu / magro);
                alpha = -sme * 2.0 / gravConst.mu;

                if (Math.Abs(sme) > small)
                    a = -gravConst.mu / (2.0 * sme);
                else
                    a = 999999.9;
                if (Math.Abs(alpha) < small)   // parabola
                    alpha = 0.0;

                if (show == 'y')
                {
                    //           printf(" sme %16.8f  a %16.8f alp  %16.8f ER \n",sme/(gravConst.mu/re), a/re, alpha * re );
                    //           printf(" sme %16.8f  a %16.8f alp  %16.8f km \n",sme, a, alpha );
                    //           printf(" ktr      xn        psi           r2          xn+1        dtn \n" );
                }

                // ------------   setup initial guess for x  ---------------
                // -----------------  circle and ellipse -------------------
                if (alpha >= small)
                {
                    period = twopi * Math.Sqrt(Math.Abs(a * a * a) / gravConst.mu);
                    // ------- next not needed for 2body multi-rev ----------
                    //if (Math.Abs(dtsec) > Math.Abs(period))
                        // including the truncation will produce vertical lines that are parallel
                        // (plotting chi vs time)
                        //                    dtseco = rem( dtsec,period );
                    // mulrev = Convert.ToInt16(dtsec / period);
                    if (Math.Abs(alpha - 1.0) > small)
                        xold = Math.Sqrt(gravConst.mu) * dtsec * alpha;
                    else
                        // - first guess can't be too close. ie a circle, r2=a
                        xold = Math.Sqrt(gravConst.mu) * dtsec * alpha * 0.97;
                }
                else
                {
                    // --------------------  parabola  ---------------------
                    if (Math.Abs(alpha) < small)
                    {
                        MathTimeLibr.cross(r1, vo, out h);
                        magh = MathTimeLibr.mag(h);
                        p = magh * magh / gravConst.mu;
                        s = 0.5 * (halfpi - Math.Atan(3.0 * Math.Sqrt(gravConst.mu / (p * p * p)) * dtsec));
                        w = Math.Atan(Math.Pow(Math.Tan(s), (1.0 / 3.0)));
                        xold = Math.Sqrt(p) * (2.0 * MathTimeLibr.cot(2.0 * w));
                        alpha = 0.0;
                    }
                    else
                    {
                        // ------------------  hyperbola  ------------------
                        temp = -2.0 * gravConst.mu * dtsec /
                              (a * (rdotv + Math.Sign(dtsec) * Math.Sqrt(-gravConst.mu * a) *
                              (1.0 - magro * alpha)));
                        xold = Math.Sign(dtsec) * Math.Sqrt(-a) * Math.Log(temp);
                    }
                } // if alpha

                ktr = 1;
                dtnew = -10.0;
                double tmp = 1.0 / Math.Sqrt(gravConst.mu);
                while ((Math.Abs(dtnew * tmp - dtsec) >= small) && (ktr < numiter))
                {
                    xoldsqrd = xold * xold;
                    znew = xoldsqrd * alpha;

                    // ------------- find c2 and c3 functions --------------
                    findc2c3(znew, out c2new, out c3new);

                    // ------- use a newton iteration for new values -------
                    rval = xoldsqrd * c2new + rdotv * tmp * xold * (1.0 - znew * c3new)
                        + magro * (1.0 - znew * c2new);
                    dtnew = xoldsqrd * xold * c3new + rdotv * tmp * xoldsqrd * c2new
                        + magro * xold * (1.0 - znew * c3new);

                    // ------------- calculate new value for x -------------
                    xnew = xold + (dtsec * Math.Sqrt(gravConst.mu) - dtnew) / rval;

                    // ----- check if the univ param goes negative. if so, use bissection
                    // if dtsec is -, then so will xnew be -
                    if (xnew < 0.0 && dtsec > 0.0)
                        xnew = xold * 0.5;

                    if (show == 'y')
                    {
                        //  printf("%3i %11.7f %11.7f %11.7f %11.7f %11.7f \n", ktr,xold,znew,rval,xnew,dtnew);
                        //  printf("%3i %11.7f %11.7f %11.7f %11.7f %11.7f \n", ktr,xold/sqrt(re),znew,rval/re,xnew/sqrt(re),dtnew/sqrt(mu));
                    }

                    ktr = ktr + 1;
                    xold = xnew;
                }  // while

                if (ktr >= numiter)
                {
                    //errork = "knotconv";
                    //           printf("not converged in %2i iterations \n",numiter );
                    for (i = 0; i < 3; i++)
                    {
                        v[i] = 0.0;
                        r2[i] = v[i];
                    }
                }
                else
                {
                    // --- find position and velocity vectors at new time --
                    xnewsqrd = xnew * xnew;
                    f = 1.0 - (xnewsqrd * c2new / magro);
                    g = dtsec - xnewsqrd * xnew * c3new / Math.Sqrt(gravConst.mu);

                    for (i = 0; i < 3; i++)
                        r2[i] = f * r1[i] + g * vo[i];
                    magr = MathTimeLibr.mag(r2);
                    gdot = 1.0 - (xnewsqrd * c2new / magr);
                    fdot = (Math.Sqrt(gravConst.mu) * xnew / (magro * magr)) * (znew * c3new - 1.0);
                    for (i = 0; i < 3; i++)
                        v[i] = fdot * r1[i] + gdot * vo[i];
                    magv = MathTimeLibr.mag(v);
                    temp = f * gdot - fdot * g;
                    //if (Math.Abs(temp - 1.0) > 0.00001)
                    //    errork = "fandg";

                    if (show == 'y')
                    {
                        //           printf("f %16.8f g %16.8f fdot %16.8f gdot %16.8f \n",f, g, fdot, gdot );
                        //           printf("f %16.8f g %16.8f fdot %16.8f gdot %16.8f \n",f, g, fdot, gdot );
                        //           printf("r1 %16.8f %16.8f %16.8f ER \n",r2[0]/re,r2[1]/re,r2[2]/re );
                        //           printf("v1 %16.8f %16.8f %16.8f ER/TU \n",v[0]/velkmps, v[1]/velkmps, v[2]/velkmps );
                    }
                }
            } // if Math.Abs
            else
                // ----------- set vectors to incoming since 0 time --------
                for (i = 0; i < 3; i++)
                {
                    r2[i] = r1[i];
                    v[i] = vo[i];
                }

            // fprintf( fid,"%11.5f  %11.5f %11.5f  %5i %3i ",znew, dtseco/60.0, xold/(rad), ktr, mulrev );
        }  // kepler



        /* ----------------------------------------------------------------------------
          *
          *
          *                           function ecef2ll
          *
          *  these subroutines convert a geocentric equatorial position vector into
          *    latitude and longitude.  geodetic and geocentric latitude are found. the
          *    inputs must be ecef.
          *
          *  author        : david vallado           davallado@gmail.com   27 may 2002
          *
          *  revisions
          *    vallado     - fix jdut1 var name, add clarifying comments   26 aug 2002
          *    vallado     - fix documentation for ecef                    19 jan 2005
          *
          *  inputs          description                    range / units
          *    r           - ecef position vector           km
          *
          *  outputs       :
          *    latgc       - geocentric latitude            -Math.PI to Math.PI rad
          *    latgd       - geodetic latitude              -Math.PI to Math.PI rad
          *    lon         - longitude (west -)             -2pi to 2pi rad
          *    hellp       - height above the ellipsoid     km
          *
          *  locals        :
          *    temp        - diff between geocentric/
          *                  geodetic lat                   rad
          *    Math.Sintemp     - Math.Sine of temp                   rad
          *    olddelta    - previous value of deltalat     rad
          *    rtasc       - right ascension                rad
          *    decl        - declination                    rad
          *    i           - index
          *
          *  coupling      :
          *    mag         - magnitude of a vector
          *    gcgd        - converts between geocentric and geodetic latitude
          *
          *  references    :
          *    vallado       2001, 174-179, alg 12 and alg 13, ex 3-3
          *
          ------------------------------------------------------------------------------ */

        public void ecef2ll
            (
            double[] r, out double latgc, out double latgd, out double lon, out double hellp
            )
        {
            double twopi = 2.0 * Math.PI;
            double small = 0.00000001;         // small value for tolerances
            double eesqrd = 0.006694379990141;     // eccentricity of earth sqrd

            int i;
            double temp, decl, rtasc, olddelta, magr, sintemp, c, s;

            c = 0.0;

            // -------------------------  implementation   -----------------
            magr = MathTimeLibr.mag(r);

            // ----------------- find longitude value  ---------------------
            temp = Math.Sqrt(r[0] * r[0] + r[1] * r[1]);
            if (Math.Abs(temp) < small)
                rtasc = Math.Sign(r[2]) * Math.PI * 0.5;
            else
                rtasc = Math.Atan2(r[1], r[0]);

            lon = rtasc;
            if (Math.Abs(lon) >= Math.PI)   // mod it ?
                if (lon < 0.0)
                    lon = twopi + lon;
                else
                    lon = lon - twopi;

            decl = Math.Asin(r[2] / magr);
            latgd = decl;

            // ------------- iterate to find geodetic latitude -------------
            i = 1;
            olddelta = latgd + 10.0;

            while ((Math.Abs(olddelta - latgd) >= small) && (i < 10))
            {
                olddelta = latgd;
                sintemp = Math.Sin(latgd);
                c = gravConst.re / (Math.Sqrt(1.0 - eesqrd * sintemp * sintemp));
                latgd = Math.Atan((r[2] + c * eesqrd * sintemp) / temp);
                i = i + 1;
            }

            // Calculate height
            if (Math.PI * 0.5 - Math.Abs(latgd) > Math.PI / 180.0)  // 1 deg
                hellp = (temp / Math.Cos(latgd)) - c;
            else
            {
                s = c * (1.0 - eesqrd);
                hellp = r[2] / Math.Sin(latgd) - s;
            }

            //latgc = gd2gc(latgd);          // use for surface of the Earth location
            latgc = Math.Asin(r[2] / magr);  // use for any location (ie satellite positions)
        } //  ecef2ll


        /*---------------------------------------------------------------------------
        *
        *                           function gd2gc
        *
        *  this function converts from geodetic to geocentric latitude for positions
        *    on the surface of the earth.  notice that (1-f) squared = 1-esqrd.
        *
        *  author        : david vallado           davallado@gmail.com   30 may 2002
        *
        *  revisions
        *                -
        *
        *  inputs          description                    range / units
        *    latgd       - geodetic latitude              -Math.PI to Math.PI rad
        *
        *  outputs       :
        *    latgc       - geocentric latitude            -Math.PI to Math.PI rad
        *
        *  locals        :
        *    none.
        *
        *  coupling      :
        *    none.
        *
        *  references    :
        *    vallado       2001, 146, eq 3-11
        *
        * [latgc] = gd2gc ( latgd );
        * ------------------------------------------------------------------------------ */

        public double gd2gc
            (
            double latgd
            )
        {
            double eesqrd = 0.006694379990141;     // eccentricity of earth sqrd

            // -------------------------  implementation   -----------------
            return Math.Atan((1.0 - eesqrd) * Math.Tan(latgd));

        }  //  gd2gc



        /* ------------------------------------------------------------------------------
        //
        //                           function checkhitearth
        //
        //  this function checks to see if the trajectory hits the earth during the
        //    transfer. 
        //
        //  author        : david vallado           davallado@gmail.com   14 aug 2017
        //
        //  inputs          description                    range / units
        //    altPad      - pad for alt above surface       km  
        //    r1          - initial position vector of int  km   
        //    v1t         - initial velocity vector of trns km/s
        //    r2          - final position vector of int    km
        //    v2t         - final velocity vector of trns   km/s
        //    nrev        - number of revolutions           0, 1, 2,  
        //
        //  outputs       :
        //    hitearth    - is earth was impacted           'y' 'n'
        //    hitearthstr - is earth was impacted           "y - radii" "no"
        //
        //  locals        :
        //    sme         - specific mechanical energy
        //    rp          - radius of perigee               km
        //    a           - semimajor axis of transfer      km
        //    ecc         - eccentricity of transfer
        //    p           - semi-paramater of transfer      km
        //    hbar        - angular momentum vector of
        //                  transfer orbit
        //    radiuspad   - radius including user pad       km
        //
        //  coupling      :
        //    dot         - dot product of vectors
        //    mag         - magnitude of a vector
        //    MathTimeLibr.cross       - MathTimeLibr.cross product of vectors
        //
        //  references    :
        //    vallado       2013, 503, alg 60
        // ------------------------------------------------------------------------------*/

        public void checkhitearth
            (
               double altPad, double[] r1, double[] v1t, double[] r2, double[] v2t, Int32 nrev,
               out char hitearth, out string hitearthstr, out double rp, out double a
            )
        {
            double radiuspad = gravConst.re + altPad; // radius of Earth with pad, km
            double magh, magv1, v12, ainv, ecc, ecosea1, esinea1, ecosea2;
            double[] hbar = new double[3];
            rp = 0.0;
            a = 0.0;

            double magr1 = MathTimeLibr.mag(r1);
            double magr2 = MathTimeLibr.mag(r2);

            hitearth = 'n';
            hitearthstr = "no";

            // check whether Lambert transfer trajectory hits the Earth
            if (magr1 < radiuspad || magr2 < radiuspad)
            {
                // hitting earth already at start or stop point
                hitearth = 'y';
                hitearthstr = hitearth + "_radii";
            }
            else
            {
                double rdotv1 = MathTimeLibr.dot(r1, v1t);
                double rdotv2 = MathTimeLibr.dot(r2, v2t);

                // Solve for a 
                magv1 = MathTimeLibr.mag(v1t);
                v12 = magv1 * magv1;
                ainv = 2.0 / magr1 - v12 / gravConst.mu;

                // Find ecos(E) 
                ecosea1 = 1.0 - magr1 * ainv;
                ecosea2 = 1.0 - magr2 * ainv;

                // Determine radius of perigee
                // 4 distinct cases pass thru perigee 
                // nrev > 0 you have to check
                if (nrev > 0)
                {
                    a = 1.0 / ainv;
                    // elliptical orbit
                    if (a > 0.0)
                    {
                        esinea1 = rdotv1 / Math.Sqrt(gravConst.mu * a);
                        ecc = Math.Sqrt(ecosea1 * ecosea1 + esinea1 * esinea1);
                    }
                    // hyperbolic orbit
                    else
                    {
                        esinea1 = rdotv1 / Math.Sqrt(gravConst.mu * Math.Abs(-a));
                        ecc = Math.Sqrt(ecosea1 * ecosea1 - esinea1 * esinea1);
                    }
                    rp = a * (1.0 - ecc);
                    if (rp < radiuspad)
                    {
                        hitearth = 'y';
                        hitearthstr = hitearth + "Sub_Earth_nrrp";
                    }
                }
                // nrev = 0, 3 cases:
                // heading to perigee and ending after perigee
                // both headed away from perigee, but end is closer to perigee
                // both headed toward perigee, but start is closer to perigee
                else
                {
                    // this rp calc section is debug only!!!
                    a = 1.0 / ainv;
                    MathTimeLibr.cross(r1, v1t, out hbar);
                    magh = MathTimeLibr.mag(hbar);
                    double[] nbar = new double[3];
                    double[] ebar = new double[3];
                    nbar[0] = -hbar[1];
                    nbar[1] = hbar[0];
                    nbar[2] = 0.0;
                    double magn = MathTimeLibr.mag(nbar);
                    double c1 = magv1 * magv1 - gravConst.mu / magr1;
                    double rdotv = MathTimeLibr.dot(r1, v1t);
                    for (int i = 0; i <= 2; i++)
                        ebar[i] = (c1 * r1[i] - rdotv * v1t[i]) / gravConst.mu;
                    ecc = MathTimeLibr.mag(ebar);
                    rp = a * (1.0 - ecc);


                    // check only cases that may be a problem
                    if ((rdotv1 < 0.0 && rdotv2 > 0.0) || (rdotv1 > 0.0 && rdotv2 > 0.0 && ecosea1 < ecosea2) ||
                    (rdotv1 < 0.0 && rdotv2 < 0.0 && ecosea1 > ecosea2))
                    {
                        // parabolic orbit
                        if (Math.Abs(ainv) <= 1.0e-10)
                        {
                            MathTimeLibr.cross(r1, v1t, out hbar);
                            magh = MathTimeLibr.mag(hbar); // find h magnitude
                            rp = magh * magh * 0.5 / gravConst.mu;
                            if (rp < radiuspad)
                            {
                                hitearth = 'y';
                                hitearthstr = hitearth + "Sub_Earth_para";
                            }
                        }
                        else
                        {
                            a = 1.0 / ainv;
                            // elliptical orbit
                            if (a > 0.0)
                            {
                                esinea1 = rdotv1 / Math.Sqrt(gravConst.mu * a);
                                ecc = Math.Sqrt(ecosea1 * ecosea1 + esinea1 * esinea1);
                            }
                            // hyperbolic orbit
                            else
                            {
                                esinea1 = rdotv1 / Math.Sqrt(gravConst.mu * Math.Abs(-a));
                                ecc = Math.Sqrt(ecosea1 * ecosea1 - esinea1 * esinea1);
                            }
                            if (ecc < 1.0)
                            {
                                rp = a * (1.0 - ecc);
                                if (rp < radiuspad)
                                {
                                    hitearth = 'y';
                                    hitearthstr = hitearth + "Sub_Earth_ell";
                                }
                            }
                            else
                            {
                                // hyperbolic heading towards the earth
                                if (rdotv1 < 0.0 && rdotv2 > 0.0)
                                {
                                    rp = a * (1.0 - ecc);
                                    if (rp < radiuspad)
                                    {
                                        hitearth = 'y';
                                        hitearthstr = hitearth + "Sub_Earth_hyp";
                                    }
                                }
                            }

                        } // ell and hyp checks
                    } // end nrev = 0 cases
                }  // nrev = 0 cases
            }  // check if starting positions ok
        } // checkhitearth


        /* ------------------------------------------------------------------------------
        //
        //                           function checkhitearthc
        //
        //  this function checks to see if the trajectory hits the earth during the
        //    transfer. Calc in canonical units.
        //
        //  author        : david vallado           davallado@gmail.com   14 aug 2017
        //
        //  inputs          description                    range / units
        //    altPadc     - pad for alt above surface       er  
        //    r1c         - initial position vector of int  er   
        //    v1tc        - initial velocity vector of trns er/tu
        //    r2c         - final position vector of int    er
        //    v2tc        - final velocity vector of trns   er/tu
        //    nrev        - number of revolutions           0, 1, 2,  
        //
        //  outputs       :
        //    hitearth    - is earth was impacted           'y' 'n'
        //    hitearthstr - is earth was impacted           "y - radii" "no"
        //
        //  locals        :
        //    sme         - specific mechanical energy
        //    rp          - radius of perigee               er
        //    a           - semimajor axis of transfer      er
        //    ecc         - eccentricity of transfer
        //    p           - semi-paramater of transfer      er
        //    hbar        - angular momentum vector of
        //                  transfer orbit
        //    radiuspadc  - radius including user pad       er
        //
        //  coupling      :
        //    dot         - dot product of vectors
        //    mag         - magnitude of a vector
        //    MathTimeLibr.cross       - MathTimeLibr.cross product of vectors
        //
        //  references    :
        //    vallado       2013, 503, alg 60
        // ------------------------------------------------------------------------------*/

        public void checkhitearthc
            (
               double altPadc, double[] r1c, double[] v1tc, double[] r2c, double[] v2tc, Int32 nrev,
               out char hitearth, out string hitearthstr, out double rp, out double a
            )
        {
            double radiuspadc = 1.0 + altPadc; // radius of Earth with pad, er
            double magh, magv1c, v1c2, ainv, ecc, ecosea1, esinea1, ecosea2;
            double[] hbar = new double[3];

            rp = 0.0;
            a = 0.0;

            double magr1c = MathTimeLibr.mag(r1c);
            double magr2c = MathTimeLibr.mag(r2c);

            hitearth = 'n';
            hitearthstr = "no";

            // check whether Lambert transfer trajectory hits the Earth
            if (magr1c < radiuspadc || magr2c < radiuspadc)
            {
                // hitting earth already at start or stop point
                hitearth = 'y';
                hitearthstr = hitearth + "_radii";
            }
            else
            {
                // canonical units  
                double rdotv1c = MathTimeLibr.dot(r1c, v1tc);
                double rdotv2c = MathTimeLibr.dot(r2c, v2tc);

                // Solve for a 
                magv1c = MathTimeLibr.mag(v1tc);
                v1c2 = magv1c * magv1c;
                ainv = 2.0 / magr1c - v1c2;

                // Find ecos(E) 
                ecosea1 = 1.0 - magr1c * ainv;
                ecosea2 = 1.0 - magr2c * ainv;

                // Determine radius of perigee
                // 4 distinct cases pass thru perigee 
                // nrev > 0 you have to check
                if (nrev > 0)
                {
                    a = 1.0 / ainv;
                    // elliptical orbit
                    if (a > 0.0)
                    {
                        esinea1 = rdotv1c / Math.Sqrt(a);
                        ecc = Math.Sqrt(ecosea1 * ecosea1 + esinea1 * esinea1);
                    }
                    // hyperbolic orbit
                    else
                    {
                        esinea1 = rdotv1c / Math.Sqrt(Math.Abs(-a));
                        ecc = Math.Sqrt(ecosea1 * ecosea1 - esinea1 * esinea1);
                    }
                    rp = a * (1.0 - ecc);
                    if (rp < radiuspadc)
                    {
                        hitearth = 'y';
                        hitearthstr = hitearth + "Sub_Earth_nrev";
                    }
                }
                // nrev = 0, 3 cases:
                // heading to perigee and ending after perigee
                // both headed away from perigee, but end is closer to perigee
                // both headed toward perigee, but start is closer to perigee
                else
                    if ((rdotv1c < 0.0 && rdotv2c > 0.0) || (rdotv1c > 0.0 && rdotv2c > 0.0 && ecosea1 < ecosea2) ||
                        (rdotv1c < 0.0 && rdotv2c < 0.0 && ecosea1 > ecosea2))
                {
                    // parabolic orbit
                    if (Math.Abs(ainv) <= 1.0e-10)
                    {
                        // this a calc section is debug only!!!
                        a = 1.0 / ainv;
                        MathTimeLibr.cross(r1c, v1tc, out hbar);
                        magh = MathTimeLibr.mag(hbar); // find h magnitude
                        rp = magh * magh * 0.5;
                        if (rp < radiuspadc)
                        {
                            hitearth = 'y';
                            hitearthstr = hitearth + "Sub_Earth_Para";
                        }
                    }
                    else  // hyperbolic or elliptical orbit
                    {
                        a = 1.0 / ainv;
                        if (a > 0.0)
                        {
                            esinea1 = rdotv1c / Math.Sqrt(a);
                            ecc = Math.Sqrt(ecosea1 * ecosea1 + esinea1 * esinea1);
                        }
                        else
                        {
                            esinea1 = rdotv1c / Math.Sqrt(Math.Abs(-a));
                            ecc = Math.Sqrt(ecosea1 * ecosea1 - esinea1 * esinea1);
                        }
                        if (ecc < 1.0)
                        {
                            rp = a * (1.0 - ecc);
                            if (rp < radiuspadc)
                            {
                                hitearth = 'y';
                                hitearthstr = hitearth + "Sub_Earth_ell";
                            }
                        }
                        else
                        {
                            // hyperbolic heading towards the earth
                            if (rdotv1c < 0.0 && rdotv2c > 0.0)
                            {
                                rp = a * (1.0 - ecc);
                                if (rp < radiuspadc)
                                {
                                    hitearth = 'y';
                                    hitearthstr = hitearth + "Sub_Earth_hyp";
                                }
                            }
                        }

                    }  // elliptical or hyperbolic

                } // end of perigee check
                else
                {
                    // extra for debugging - not needed for operations
                    a = 1.0 / ainv;
                    if (a > 0.0)
                    {
                        esinea1 = rdotv1c / Math.Sqrt(a);
                        ecc = Math.Sqrt(ecosea1 * ecosea1 + esinea1 * esinea1);
                    }
                    else
                    {
                        esinea1 = rdotv1c / Math.Sqrt(Math.Abs(-a));
                        ecc = Math.Sqrt(ecosea1 * ecosea1 - esinea1 * esinea1);
                    }
                    rp = a * (1.0 - ecc);
                }

            } // end of "hitting Earth surface?" tests

        } // checkhitearthc



        /* ------------------------------------------------------------------------------
        //                           function lambertumins
        //
        //  find the minimum psi values for the universal variable lambert problem
        //
        //  author        : david vallado           davallado@gmail.com    8 jun 2016
        //
        //  inputs          description                          range / units
        //    r1          - ijk position vector 1                 km
        //    r2          - ijk position vector 2                 km
        //    nrev        - multiple revolutions                  0, 1,  
        //    dm          - direction of motion                  'S', 'L'
        //                  this is the inclination discriminator
        //
        //  outputs       :
        //    kbi         - k values for min tof for each nrev
        //    tof         - min time of flight for each nrev      sec
        //
        //  references    :
        //    Arora and Russell AAS 10-198
        // ------------------------------------------------------------------------------*/

        public void lambertumins
            (
            double[] r1, double[] r2, Int32 nrev, char dm,
            out double kbi, out double tof
            )
        {
            double small, oomu, magr1, magr2, vara, cosdeltanu, sqrtmu, sqrty, dtdpsi;
            double psinew, x, y, q, s1, s2, s3, s4, x3, x5, dtnew;
            double psiold2, psiold3, psiold4;
            double c2, c3, c2dot, c3dot, c2ddot, c3ddot, upper, lower, psiold, dtdpsi2;
            double oox;
            Int32 numiter, loops;
            x = 0.0;
            sqrty = 0.0;
            psinew = 0.0;
            numiter = 20; // arbitrary limit here - doens't seem to break it. 

            small = 0.00000001;

            oomu = 1.0 / Math.Sqrt(gravConst.mu);  // for speed
            sqrtmu = Math.Sqrt(gravConst.mu);

            // ---- find parameters that are constant for the initial geometry
            magr1 = MathTimeLibr.mag(r1);
            magr2 = MathTimeLibr.mag(r2);

            cosdeltanu = MathTimeLibr.dot(r1, r2) / (magr1 * magr2);
            if (dm == 'L')  // dm == 'l'
                vara = -Math.Sqrt(magr1 * magr2 * (1.0 + cosdeltanu));
            else
                vara = Math.Sqrt(magr1 * magr2 * (1.0 + cosdeltanu));

            // ------------ find the minimum time for a nrev transfer --------------
            //   nrev = 0;
            //   for zz = 0: 4
            //       nrev = nrev + 1;
            // ---- get outer bounds for each nrev case
            lower = 4.0 * nrev * nrev * Math.PI * Math.PI;
            upper = 4.0 * (nrev + 1.0) * (nrev + 1.0) * Math.PI * Math.PI;

            // ---- streamline since we know it's near the center
            upper = lower + (upper - lower) * 0.6;
            lower = lower + (upper - lower) * 0.3;

            // ---- initial estimate, just put in center 
            psiold = (upper + lower) * 0.5;
            findc2c3(psiold, out c2, out c3);

            loops = 0;
            dtdpsi = 200.0;
            while ((Math.Abs(dtdpsi) >= 0.1) && (loops < numiter))
            {
                if (Math.Abs(c2) > small)
                    y = magr1 + magr2 - (vara * (1.0 - psiold * c3) / Math.Sqrt(c2));
                else
                    y = magr1 + magr2;
                if (Math.Abs(c2) > small)
                {
                    x = Math.Sqrt(y / c2);
                    oox = 1.0 / x;
                }
                else
                {
                    x = 0.0;
                    oox = 0.0;
                }
                sqrty = Math.Sqrt(y);
                if (Math.Abs(psiold) > 1e-5)
                {
                    c2dot = 0.5 / psiold * (1.0 - psiold * c3 - 2.0 * c2);
                    c3dot = 0.5 / psiold * (c2 - 3.0 * c3);
                    c2ddot = 1.0 / (4.0 * psiold * psiold) * ((8.0 - psiold) * c2 + 5.0 * psiold * c3 - 4.0);
                    c3ddot = 1.0 / (4.0 * psiold * psiold) * ((15.0 - psiold) * c3 - 7.0 * c2 + 1.0);
                }
                else
                {
                    psiold2 = psiold * psiold;
                    psiold3 = psiold2 * psiold;
                    psiold4 = psiold3 * psiold;
                    c2dot = -2.0 / MathTimeLibr.factorial(4) + 2.0 * psiold / MathTimeLibr.factorial(6) - 3.0 * psiold2 / MathTimeLibr.factorial(8)
                        + 4.0 * psiold3 / MathTimeLibr.factorial(10) - 5.0 * psiold4 / MathTimeLibr.factorial(12);
                    c3dot = -1.0 / MathTimeLibr.factorial(5) + 2.0 * psiold / MathTimeLibr.factorial(7) - 3.0 * psiold2 / MathTimeLibr.factorial(9)
                        + 4.0 * psiold3 / MathTimeLibr.factorial(11) - 5.0 * psiold4 / MathTimeLibr.factorial(13);
                    c2ddot = 0.0;
                    c3ddot = 0.0;
                }
                // now solve this for dt = 0.0
                //            dtdpsi = x^3*(c3dot - 3.0*c3*c2dot/(2.0*c2))* oomu + 0.125*vara/sqrt(gravConst.mu) * (3.0*c3*sqrty/c2 + vara/x);
                dtdpsi = x * x * x * (c3dot - 3.0 * c3 * c2dot / (2.0 * c2)) * oomu + 0.125 * vara * (3.0 * c3 * sqrty / c2 + vara * oox) * oomu;

                q = 0.25 * vara * Math.Sqrt(c2) - x * x * c2dot;
                x3 = x * x * x;
                x5 = x3 * x * x;
                s1 = -24.0 * q * x3 * c2 * sqrty * c3dot;
                s2 = 36.0 * q * x3 * sqrty * c3 * c2dot - 16.0 * x5 * sqrty * c3ddot * c2 * c2;
                s3 = 24.0 * x5 * sqrty * (c3dot * c2dot * c2 + c3 * c2ddot * c2 - c3 * c2dot * c2dot) - 6.0 * vara * c3dot * y * c2 * x * x;
                s4 = -0.75 * vara * vara * c3 * Math.Pow(c2, 1.5) * x * x + 6.0 * vara * c3 * y * c2dot * x * x +
                    (vara * vara * c2 * (0.25 * vara * Math.Sqrt(c2) - x * x * c2)) * sqrty * oox; // C(z)??

                dtdpsi2 = -(s1 + s2 + s3 + s4) / (16.0 * sqrtmu * (c2 * c2 * sqrty * x * x));
                // NR update
                psinew = psiold - dtdpsi / dtdpsi2;

                //fprintf(" %3i %12.4f %12.4f %12.4f %12.4f %11.4f %12.4f %12.4f %11.4f %11.4f \n", 
                //    loops, y, dtdpsi, psiold, psinew, psinew - psiold, dtdpsi, dtdpsi2, lower, upper);
                psiold = psinew;
                findc2c3(psiold, out c2, out c3);
                // don't need for the loop iterations
                // dtnew = (x^3*c3 + vara*sqrty) * oomu;
                loops = loops + 1;
            }  // while
            // calculate once at the end
            dtnew = (x * x * x * c3 + vara * sqrty) * oomu;
            tof = dtnew;
            kbi = psinew;
            //       fprintf(1,' nrev %3i  dtnew %12.5f psi %12.5f  lower %10.3f upper %10.3f %10.6f %10.6f \n',nrev, dtnew, psiold, lower, upper, c2, c3);
        } // lambertumins



        /*------------------------------------------------------------------------------
        *
        *                           procedure lambertminT
        *
        *  this procedure solves lambert's problem and finds the miniumum time for 
        *  multi-revolution cases.
        *
        *  author        : david vallado           davallado@gmail.com   22 mar 2018
        *
        *  inputs          description                        range / units
        *    r1          - ijk position vector 1                km
        *    r2          - ijk position vector 2                km
        *    dm          - direction of motion                  'L', 'S'
        *    de          - orbital energy                       'L', 'H'
        *    nrev        - number of revs to complete           0, 1, 2, 3,  
        *
        *  outputs       :
        *    tmin        - minimum time of flight               sec
        *    tminp       - minimum parabolic tof                sec
        *    tminenergy  - minimum energy tof                   sec
        *
        *  locals        :
        *    i           - index
        *    loops       -
        *    cosdeltanu  -
        *    sindeltanu  -
        *    dnu         -
        *    chord       -
        *    s           -
        *
        *  coupling      :
        *    mag         - magnitude of a vector
        *    dot         - dot product
        *
        *  references    :
        *    vallado       2013, 494, Alg 59, ex 7-5
        *    prussing      JAS 2000
        -----------------------------------------------------------------------------*/

        public void lambertminT
        (
            double[] r1, double[] r2, char dm, char de, Int32 nrev,
            out double tmin, out double tminp, out double tminenergy
        )
        {
            double a, s, chord, magr1, magr2, cosdeltanu;
            double[] rcrossr = new double[3];
            double alpha, alp, beta, fa, fadot, xi, eta, del, an;
            int i;

            magr1 = MathTimeLibr.mag(r1);
            magr2 = MathTimeLibr.mag(r2);
            cosdeltanu = MathTimeLibr.dot(r1, r2) / (magr1 * magr2);
            // make sure it's not more than 1.0
            if (Math.Abs(cosdeltanu) > 1.0)
                cosdeltanu = 1.0 * Math.Sign(cosdeltanu);

            // these are the same
            chord = Math.Sqrt(magr1 * magr1 + magr2 * magr2 - 2.0 * magr1 * magr2 * cosdeltanu);
            //chord = MathTimeLibr.mag(r2 - r1);

            s = (magr1 + magr2 + chord) * 0.5;

            xi = 0.0;
            eta = 0.0;

            // could do this just for nrev cases, but you can also find these for any nrev if (nrev > 0)
            // ------------- calc tmin parabolic tof to see if the orbit is possible
            if (dm == 'S')
                tminp = (1.0 / 3.0) * Math.Sqrt(2.0 / gravConst.mu) * (Math.Pow(s, 1.5) - Math.Pow(s - chord, 1.5));
            else
                tminp = (1.0 / 3.0) * Math.Sqrt(2.0 / gravConst.mu) * (Math.Pow(s, 1.5) + Math.Pow(s - chord, 1.5));

            // ------------- this is the min energy ellipse tof
            double amin = 0.5 * s;
            alpha = Math.PI;
            beta = 2.0 * Math.Asin(Math.Sqrt((s - chord) / s));
            if (dm == 'S')
                tminenergy = Math.Pow(amin, 1.5) * ((2.0 * nrev + 1.0) * Math.PI - beta + Math.Sin(beta)) / Math.Sqrt(gravConst.mu);
            else
                tminenergy = Math.Pow(amin, 1.5) * ((2.0 * nrev + 1.0) * Math.PI + beta - Math.Sin(beta)) / Math.Sqrt(gravConst.mu);

            // -------------- calc min tof ellipse (prussing 1992 aas, 2000 jas)
            an = 1.001 * amin;
            fa = 10.0;
            i = 1;
            double rad = 180.0 / Math.PI;
            string[] tempstr = new string[25];
            while (Math.Abs(fa) > 0.00001 && i <= 20)
            {
                a = an;
                alp = 1.0 / a;
                double temp = Math.Sqrt(0.5 * s * alp);
                if (Math.Abs(temp) > 1.0)
                    temp = Math.Sign(temp) * 1.0;
                alpha = 2.0 * Math.Asin(temp);
                // now account for direct or retrograde
                temp = Math.Sqrt(0.5 * (s - chord) * alp);
                if (Math.Abs(temp) > 1.0)
                    temp = Math.Sign(temp) * 1.0;
                if (dm == 'L')  // old de l
                    beta = 2.0 * Math.Asin(temp);
                else
                    beta = -2.0 * Math.Asin(temp);  // fix quadrant
                xi = alpha - beta;
                eta = Math.Sin(alpha) - Math.Sin(beta);
                fa = (6.0 * nrev * Math.PI + 3.0 * xi - eta) * (Math.Sin(xi) + eta) - 8.0 * (1.0 - Math.Cos(xi));

                fadot = ((6.0 * nrev * Math.PI + 3.0 * xi - eta) * (Math.Cos(xi) + Math.Cos(alpha)) +
                         (3.0 - Math.Cos(alpha)) * (Math.Sin(xi) + eta) - 8.0 * Math.Sin(xi)) * (-alp * Math.Tan(0.5 * alpha))
                         + ((6.0 * nrev * Math.PI + 3.0 * xi - eta) * (-Math.Cos(xi) - Math.Cos(beta)) +
                         (-3.0 + Math.Cos(beta)) * (Math.Sin(xi) + eta) + 8.0 * Math.Sin(xi)) * (-alp * Math.Tan(0.5 * beta));
                del = fa / fadot;
                an = a - del;
                tempstr[i] = (alpha * rad).ToString("0.00000000") + " " + (beta * rad).ToString("0.00000000") + " " +
                           (xi).ToString("0.00000000") + " " + eta.ToString("0.00000000") + " " + fadot.ToString("0.00000000") + " " + fa.ToString("0.00000000") + " " +
                           an.ToString("0.00000000");
                i = i + 1;
            }
            // could update beta one last time with alpha too????
            if (dm == 'S')
                tmin = Math.Pow(an, 1.5) * (2.0 * Math.PI * nrev + xi - eta) / Math.Sqrt(gravConst.mu);
            else
                tmin = Math.Pow(an, 1.5) * (2.0 * Math.PI * nrev + xi + eta) / Math.Sqrt(gravConst.mu);
        }  // lambertminT


        /*------------------------------------------------------------------------------
        *
        *                           procedure lambertTmaxrp
        *
        *  this procedure solves lambert's problem and finds the TOF for maximum rp
        *
        *  author        : david vallado           davallado@gmail.com   26 aug 2019
        *
        *  inputs          description                    range / units
        *    r1          - ijk position vector 1          km
        *    r2          - ijk position vector 2          km
        *    dm          - direction of motion                  'L', 'S'
        *    de          - orbital energy                       'L', 'H'
        *    nrev        - number of revs to complete     0, 1, 2, 3,  
        *
        *  outputs       :
        *    tmin        - minimum time of flight         sec
        *    tminp       - minimum parabolic tof          sec
        *    tminenergy  - minimum energy tof             sec
        *
        *  locals        :
        *    i           - index
        *    loops       -
        *    cosdeltanu  -
        *    sindeltanu  -
        *    dnu         -
        *    chord       -
        *    s           -
        *
        *  coupling      :
        *    mag         - magnitude of a vector
        *    dot         - dot product
        *
        *  references    :
        *    thompson       2019
        *    -----------------------------------------------------------------------------*/

        public void lambertTmaxrp
        (
            double[] r1, double[] r2, char dm, Int32 nrev,
            out double tmaxrp, out double[] v1t
        )
        {
            double chord, magr1, magr2, cosdeltanu, sindeltanu;
            double[] rcrossr, nr1, nr2, nunit, v2t, tempvec = new double[3];
            double y1, y2, c, x1, x2, e1, e2, r, sme, k;
            int i;
            v1t = new double[] { 0.0, 0.0, 0.0 };
            v2t = new double[] { 0.0, 0.0, 0.0 };

            magr1 = MathTimeLibr.mag(r1);   // km
            magr2 = MathTimeLibr.mag(r2);
            cosdeltanu = MathTimeLibr.dot(r1, r2) / (magr1 * magr2);
            // make sure it's not more than 1.0
            if (Math.Abs(cosdeltanu) > 1.0)
                cosdeltanu = 1.0 * Math.Sign(cosdeltanu);
            MathTimeLibr.cross(r1, r2, out rcrossr);
            if (dm == 'S')
                sindeltanu = MathTimeLibr.mag(rcrossr) / (magr1 * magr2);
            else
                sindeltanu = -MathTimeLibr.mag(rcrossr) / (magr1 * magr2);

            // these are the same
            chord = Math.Sqrt(magr1 * magr1 + magr2 * magr2 - 2.0 * magr1 * magr2 * cosdeltanu);
            //chord = MathTimeLibr.mag(r2 - r1);

            //s = (magr1 + magr2 + chord) * 0.5;

            y1 = gravConst.mu / magr1;
            y2 = gravConst.mu / magr2;

            // ------------- nearly circular endpoints
            MathTimeLibr.addvec(1.0, r1, -1.0, r2, out tempvec);
            // circular orbit  to within 1 meter
            if (MathTimeLibr.mag(tempvec) < 0.001)
            {
                c = Math.Sqrt(y1);
                x1 = x2 = r = 0.0;
                tmaxrp = (2.0 * nrev * Math.PI + Math.Atan2(sindeltanu, cosdeltanu)) * (gravConst.mu / Math.Pow(c, 3));
            }
            else
            {
                // 
                if (magr1 < magr2)
                {
                    c = Math.Sqrt((y2 - y1 * cosdeltanu) / (1.0 - cosdeltanu));
                    x1 = 0.0;
                    x2 = (y1 - c * c) * sindeltanu;
                }
                else
                {
                    c = Math.Sqrt((y1 - y2 * cosdeltanu) / (1.0 - cosdeltanu));
                    x1 = (-y2 + c * c) * sindeltanu;
                    x2 = 0.0;
                }
                r = (Math.Sqrt(x1 * x1 + Math.Pow(y1 - c * c, 2))) / c;

                // check if acos is larger than 1
                double temp = c * (r * r + y1 - c * c) / (r * y1);
                if (Math.Abs(temp) > 1.0)
                    e1 = Math.Sign(temp) * Math.Acos(1.0);
                else
                    e1 = Math.Acos(temp);
                if (x1 < 0.0)
                    e1 = 2.0 * Math.PI - e1;

                temp = c * (r * r + y2 - c * c) / (r * y2);
                if (Math.Abs(temp) > 1.0)
                    e2 = Math.Sign(temp) * Math.Acos(1.0);
                else
                    e2 = Math.Acos(temp);
                if (x2 < 0.0)
                    e2 = 2.0 * Math.PI - e2;

                if (e2 < e1)
                    e2 = e2 + 2.0 * Math.PI;
                k = (e2 - e1) - Math.Sin(e2 - e1);

                tmaxrp = gravConst.mu * (
                    (2.0 * nrev * Math.PI + k) / Math.Pow(Math.Abs(c * c - r * r), 1.5) +
                    (c * sindeltanu) / (y1 * y2));
            }
            sme = (r * r - c * c) * 0.5;
            // close to 180 deg transfer case
            nunit = MathTimeLibr.norm(rcrossr);
            if (magr2 * sindeltanu > 0.001)  // 1 meter
            {
                for (i = 0; i < 3; i++)
                    nunit[i] = Math.Sign(sindeltanu) * nunit[i];
            }
            MathTimeLibr.cross(nunit, r1, out nr1);
            MathTimeLibr.cross(nunit, r2, out nr2);

            for (i = 0; i < 3; i++)
            {
                v1t[i] = (x1 / c) * r1[i] / magr1 + (y1 / c) * (nr1[i] / magr1);
                v2t[i] = (x1 / c) * r2[i] / magr2 + (y2 / c) * (nr2[i] / magr2);
            }

        }  // lambertTmaxrp


        /*------------------------------------------------------------------------------
        *
        *                           procedure lambertuniv
        *
        *  this procedure solves the lambert problem for orbit determination and returns
        *    the velocity vectors at each of two given position vectors.  the solution
        *    uses universal variables for calculation and a bissection technique for
        *    updating psi.
        *
        *  algorithm     : setting the initial bounds:
        *                  using -8pi and 4pi2 will allow single rev solutions
        *                  using -4pi2 and 8pi2 will allow multi-rev solutions
        *                  the farther apart the initial guess, the more iterations
        *                    because of the iteration
        *                  inner loop is for special cases. must be sure to exit both!
        *
        *  author        : david vallado                 davallado@gmail.com   22 jun 2002
        *
        *  inputs          description                          range / units
        *    r1          - ijk position vector 1                km
        *    r2          - ijk position vector 2                km
        *    v1          - ijk velocity vector 1 if avail       km/s
        *    dm          - direction of motion                  'L', 'S'
        *    de          - orbital energy                       'L', 'H'
        *                  only affects nrev >= 1 upper/lower bounds
        *    dtsec       - time between r1 and r2               sec
        *    dtwait      - time to wait before starting         sec
        *    nrev        - number of revs to complete           0, 1, 2, 3,  
        *    kbi         - psi value for min                     
        *    altpad      - altitude pad for hitearth calc       km
        *    show        - control output don't output for speed      'y', 'n'
        *
        *  outputs       :
        *    v1t         - ijk transfer velocity vector         km/s
        *    v2t         - ijk transfer velocity vector         km/s
        *    hitearth    - flag if hit or not                   'y', 'n'
        *    error       - error flag                           1, 2, 3,   use numbers since c++ is so horrible at strings
        *
        *  locals        :
        *    vara        - variable of the iteration,
        *                  not the semi or axis!
        *    y           - area between position vectors
        *    upper       - upper bound for z
        *    lower       - lower bound for z
        *    cosdeltanu  - cosine of true anomaly change        rad
        *    f           - f expression
        *    g           - g expression
        *    gdot        - g dot expression
        *    xold        - old universal variable x
        *    xoldcubed   - xold cubed
        *    zold        - old value of z
        *    znew        - new value of z
        *    c2new       - c2(z) function
        *    c3new       - c3(z) function
        *    timenew     - new time                             sec
        *    small       - tolerance for roundoff errors
        *    i, j        - index
        *
        *  coupling
        *    mag         - magnitude of a vector
        *    dot         - dot product of two vectors
        *    findc2c3    - find c2 and c3 functions
        *
        *  references    :
        *    vallado       2013, 492, alg 58, ex 7-5
        -----------------------------------------------------------------------------*/

        public void lambertuniv
               (
               double[] r1, double[] r2, double[] v1, char dm, char de, Int32 nrev, double dtwait, double dtsec, double kbi,
               double altpad, char show,
               out double[] v1t, out double[] v2t, out char hitearth, out string errorsum, out string errorout
               )
        {
            const double small = 0.0000001;
            const int numiter = 40;

            int loops, ynegktr;
            double rp, vara, y, upper, lower, cosdeltanu, f, g, gdot, xold, xoldcubed, magr1, magr2,
                psiold, psinew, psilast, c2new, c3new, dtnew, dtold, c2dot, c3dot, dtdpsi, psiold2, a;
            string hitearthstr, errorstr, estr;
            //int error;

            // needed since assignments aren't at root level in procedure
            v1t = new double[] { 0.0, 0.0, 0.0 };
            v2t = new double[] { 0.0, 0.0, 0.0 };

            /* --------------------  initialize values   -------------------- */
            estr = "";  // determine various cases
            hitearth = 'x';
            errorstr = "ok";
            errorsum = "ok";
            errorout = "ok";
            //error = 0;
            psilast = 0.0;
            psinew = 0.0;
            dtold = 0.0;
            loops = 0;
            y = 0.0;
            rp = 0.0;
            a = 0.0;
            magr1 = MathTimeLibr.mag(r1);
            magr2 = MathTimeLibr.mag(r2);

            cosdeltanu = MathTimeLibr.dot(r1, r2) / (magr1 * magr2);
            if (dm == 'L')  //~works de == 'h'   
                vara = -Math.Sqrt(magr1 * magr2 * (1.0 + cosdeltanu));
            else
                vara = Math.Sqrt(magr1 * magr2 * (1.0 + cosdeltanu));

            /* -------- set up initial bounds for the bisection ------------ */
            if (nrev == 0)
            {
                lower = -16.0 * Math.PI * Math.PI; // could be negative infinity for all cases, allow hyperbolic and parabolic solutions
                upper = 4.0 * Math.PI * Math.PI;
            }
            else
            {
                lower = 4.0 * nrev * nrev * Math.PI * Math.PI;
                upper = 4.0 * (nrev + 1.0) * (nrev + 1.0) * Math.PI * Math.PI;
                //                if (((df == 'r') && (dm == 'l')) || ((df == 'd') && (dm == 'l')))
                if (de == 'H')   // high way is always the 1st half
                    upper = kbi;
                else
                    lower = kbi;
            }

            /* ----------------  form initial guesses   --------------------- */
            psinew = 0.0;
            xold = 0.0;
            if (nrev == 0)
            {
                psiold = (Math.Log(dtsec) - 9.61202327) / 0.10918231;
                if (psiold > upper)
                    psiold = upper - Math.PI;
            }
            else
                psiold = lower + (upper - lower) * 0.5;

            findc2c3(psiold, out c2new, out c3new);

            double oosqrtmu = 1.0 / Math.Sqrt(gravConst.mu);

            // find initial dtold from psiold
            if (Math.Abs(c2new) > small)
                y = magr1 + magr2 - (vara * (1.0 - psiold * c3new) / Math.Sqrt(c2new));
            else
                y = magr1 + magr2;
            if (Math.Abs(c2new) > small)
                xold = Math.Sqrt(y / c2new);
            else
                xold = 0.0;
            xoldcubed = xold * xold * xold;
            dtold = (xoldcubed * c3new + vara * Math.Sqrt(y)) * oosqrtmu;

            // -----------  determine if the orbit is possible at all ------------ 
            if (Math.Abs(vara) > 0.2)  // small
            {
                loops = 0;
                ynegktr = 1; // y neg ktr
                dtnew = -10.0;
                while ((Math.Abs(dtnew - dtsec) >= small) && (loops < numiter) && (ynegktr <= 10))
                {
                    if (Math.Abs(c2new) > small)
                        y = magr1 + magr2 - (vara * (1.0 - psiold * c3new) / Math.Sqrt(c2new));
                    else
                        y = magr1 + magr2;

                    // ------- check for negative values of y ------- 
                    if ((vara > 0.0) && (y < 0.0))
                    {
                        ynegktr = 1;
                        while ((y < 0.0) && (ynegktr < 10))
                        {
                            psinew = 0.8 * (1.0 / c3new) * (1.0 - (magr1 + magr2) * Math.Sqrt(c2new) / vara);

                            /* ------ find c2 and c3 functions ------ */
                            findc2c3(psinew, out c2new, out c3new);
                            psiold = psinew;
                            lower = psiold;
                            if (Math.Abs(c2new) > small)
                                y = magr1 + magr2 - (vara * (1.0 - psiold * c3new) / Math.Sqrt(c2new));
                            else
                                y = magr1 + magr2;

                            // show loop iteration if ynegktr
                            if (show == 'y')
                                errorout = errorout + "\r\nyneg" + ynegktr.ToString().PadLeft(3) + "   " + psiold.ToString("0.0000000").PadLeft(12) + "  " +
                                    y.ToString("0.0000000").PadLeft(15) + " " + xold.ToString("0.00000").PadLeft(13) + " " +
                                    dtnew.ToString("0.#######").PadLeft(15) + " " + vara.ToString("0.#######").PadLeft(15) + " " +
                                    upper.ToString("0.#######").PadLeft(15) + " " + lower.ToString("0.#######").PadLeft(15) + " " + ynegktr.ToString();

                            ynegktr++;
                        }
                    }

                    loops = loops + 1;

                    if (ynegktr < 10)
                    {
                        if (Math.Abs(c2new) > small)
                            xold = Math.Sqrt(y / c2new);
                        else
                            xold = 0.0;
                        xoldcubed = xold * xold * xold;
                        dtnew = (xoldcubed * c3new + vara * Math.Sqrt(y)) * oosqrtmu;

                        // try newton rhapson iteration to update psinew
                        if (Math.Abs(psiold) > 1e-5)
                        {
                            c2dot = 0.5 / psiold * (1.0 - psiold * c3new - 2.0 * c2new);
                            c3dot = 0.5 / psiold * (c2new - 3.0 * c3new);
                        }
                        else
                        {
                            psiold2 = psiold * psiold;
                            c2dot = -1.0 / MathTimeLibr.factorial(4) + 2.0 * psiold / MathTimeLibr.factorial(6) - 3.0 * psiold2 / MathTimeLibr.factorial(8) +
                                     4.0 * psiold2 * psiold / MathTimeLibr.factorial(10) - 5.0 * psiold2 * psiold2 / MathTimeLibr.factorial(12);
                            c3dot = -1.0 / MathTimeLibr.factorial(5) + 2.0 * psiold / MathTimeLibr.factorial(7) - 3.0 * psiold2 / MathTimeLibr.factorial(9) +
                                     4.0 * psiold2 * psiold / MathTimeLibr.factorial(11) - 5.0 * psiold2 * psiold2 / MathTimeLibr.factorial(13);
                        }
                        dtdpsi = (xoldcubed * (c3dot - 3.0 * c3new * c2dot / (2.0 * c2new)) + vara / 8.0 * (3.0 * c3new * Math.Sqrt(y) / c2new + vara / xold)) * oosqrtmu;
                        psinew = psiold - (dtnew - dtsec) / dtdpsi;
                        double psitmp = psinew;

                        // check if newton guess for psi is outside bounds(too steep a slope), then use bisection
                        if (psinew > upper || psinew < lower)
                        {
                            // --------readjust upper and lower bounds-------
                            // special check for 0 rev cases 
                            if (de == 'L' || (nrev == 0))
                            {
                                if (dtold < dtsec)
                                    lower = psiold;
                                else
                                    upper = psiold;
                            }
                            else
                            {
                                if (dtold < dtsec)
                                    upper = psiold;
                                else
                                    lower = psiold;
                            }

                            psinew = (upper + lower) * 0.5;
                            psilast = psinew;
                            estr = estr + ynegktr.ToString() + " biss ";
                        }

                        // save info of each iteration
                        if (show == 'y')
                            errorout = errorout + "\r\n" + loops.ToString().PadLeft(3) + "   " + y.ToString("0.0000000").PadLeft(12) + "  " +
                                xold.ToString("0.0000000").PadLeft(15) + " " + dtsec.ToString("0.00000").PadLeft(13) + " " +
                                dtnew.ToString("0.#######").PadLeft(15) + " " + lower.ToString("0.#######").PadLeft(15) + " " +
                                upper.ToString("0.#######").PadLeft(15) + " " + dtdpsi.ToString("0.#######").PadLeft(15) + " " +
                                psitmp.ToString() + " " + psinew.ToString() + " " + psilast.ToString() + " " + estr;

                        // -------------- find c2 and c3 functions ---------- 
                        findc2c3(psinew, out c2new, out c3new);
                        psilast = psiold;  // keep previous iteration
                        psiold = psinew;
                        dtold = dtnew;

                        // ---- make sure the first guess isn't too close --- 
                        if ((Math.Abs(dtnew - dtsec) < small) && (loops == 1))
                            dtnew = dtsec - 1.0;
                    }  // if ynegktr < 10

                    ynegktr = 1;
                }  // while

                if ((loops >= numiter) || (ynegktr >= 10))
                {
                    errorstr = "gnotconverged";

                    if (ynegktr >= 10)
                    {
                        errorstr = "ynegative";
                    }
                }
                else
                {
                    // ---- use f and g series to find velocity vectors ----- 
                    f = 1.0 - y / magr1;
                    gdot = 1.0 - y / magr2;
                    g = 1.0 / (vara * Math.Sqrt(y / gravConst.mu)); // 1 over g
                    //	fdot = Math.Sqrt(y) * (-magr2 - magr1 + y) / (magr2 * magr1 * vara);
                    for (int i = 0; i < 3; i++)
                    {
                        v1t[i] = ((r2[i] - f * r1[i]) * g);
                        v2t[i] = ((gdot * r2[i] - r1[i]) * g);
                    }
                    checkhitearth(altpad, r1, v1t, r2, v2t, nrev, out hitearth, out hitearthstr, out rp, out a);
                }
            }
            else
            {
                errorstr = "impossible180";
                // use battin and hodograph
                string errorsumb, erroroutb;
                lambertbattin(r1, r2, v1, dm, de, nrev, 0.0, dtsec, altpad, show, out v1t, out v2t, out hitearth, out errorsumb, out erroroutb);
            }

            double dnu;
            if (dm == 'S')  // find dnu of transfer
                dnu = Math.Acos(cosdeltanu) * 180.0 / Math.PI;
            else
            {
                dnu = -Math.Acos(cosdeltanu) * 180.0 / Math.PI;
                if (dnu < 0.0)
                    dnu = dnu + 360.0;
            }

            if (show == 'y')
                errorsum = errorstr;
            else
                errorsum = errorstr + " " + nrev.ToString().PadLeft(3) + "   " + dm + "  " + de + "  " + dtwait.ToString("0.0000000").PadLeft(12) + " " +
                dtsec.ToString("0.000000").PadLeft(15) + " " + psinew.ToString("0.000000").PadLeft(15) +
                v1t[0].ToString("0.0000000").PadLeft(15) + v1t[1].ToString("0.0000000").PadLeft(15) + v1t[2].ToString("0.0000000").PadLeft(15) +
                v2t[0].ToString("0.0000000").PadLeft(15) + v2t[1].ToString("0.0000000").PadLeft(15) + v2t[2].ToString("0.0000000").PadLeft(15) +
                " " + loops.ToString().PadLeft(4) + " " + rp.ToString("0.0000000").PadLeft(15) + " " + a.ToString("0.0000000").PadLeft(15) +
                " " + dnu.ToString("0.0000000").PadLeft(15) + " " + hitearth;
        }  //  lambertuniv



        /* -------------------------------------------------------------------------- 
        *                           function lambhodograph
        *
        * this function accomplishes 180 deg transfer(and 360 deg) for lambert problem.
        *
        *  author        : david vallado           davallado@gmail.com  22 may 2017
        *
        *  inputs          description                            range / units
        *    r1    - ijk position vector 1                            km
        *    r2    - ijk position vector 2                            km
        *    v1    - intiial ijk velocity vector 1                    km/s
        *    p     - semiparamater of transfer orbit                  km
        *    ecc   - eccentricity of transfer orbit                   km
        *    dnu   - true anomaly delta for transfer orbit            rad
        *    dtsec - time between r1 and r2                           s
        *    dnu - true anomaly change                                rad
        *
        *  outputs       :
        *    v1t - ijk transfer velocity vector                       km/s
        *    v2t - ijk transfer velocity vector                       km/s
        * 
        *  references :
        *    Thompson JGCD 2013 v34 n6 1925
        *    Thompson AAS GNC 2018
        ----------------------------------------------------------------------------- */

        public void lambhodograph
        (
        double[] r1, double[] r2, double[] v1, double p, double ecc, double dnu, double dtsec, out double[] v1t, out double[] v2t
        )
        {
            double eps, magr1, magr2, a, b, x1, x2, y2a, y2b, ptx, oomagr1, oomagr2, oop;
            double[] nvec = new double[3];
            double[] rcrv = new double[3];
            double[] rcrr = new double[3];
            int i;
            // needed since assignments aren't at root level in procedure
            v1t = new double[] { 0.0, 0.0, 0.0 };
            v2t = new double[] { 0.0, 0.0, 0.0 };
            oop = 1.0 / p;

            magr1 = MathTimeLibr.mag(r1);
            oomagr1 = 1.0 / magr1;
            magr2 = MathTimeLibr.mag(r2);
            oomagr2 = 1.0 / magr2;
            eps = 0.001 / magr2;  // 1e-14

            a = gravConst.mu * (1.0 * oomagr1 - 1.0 * oop);  // not the semi - major axis
            b = Math.Pow(gravConst.mu * ecc * oop, 2) - a * a;
            if (b <= 0.0)
                x1 = 0.0;
            else
                x1 = -Math.Sqrt(b);

            // 180 deg, and multiple 180 deg transfers
            if (Math.Abs(Math.Sin(dnu)) < eps)
            {
                MathTimeLibr.cross(r1, v1, out rcrv);
                for (i = 0; i < 3; i++)
                    nvec[i] = rcrv[i] / MathTimeLibr.mag(rcrv);
                if (ecc < 1.0)
                {
                    ptx = (2.0 * Math.PI) * Math.Sqrt(p * p * p / (gravConst.mu * Math.Pow(1.0 - ecc * ecc, 3)));
                    if (dtsec % ptx > ptx * 0.5)  // mod
                        x1 = -x1;
                }
            }
            else
            {
                // this appears to be the more common option
                y2a = gravConst.mu * oop - x1 * Math.Sin(dnu) + a * Math.Cos(dnu);
                y2b = gravConst.mu * oop + x1 * Math.Sin(dnu) + a * Math.Cos(dnu);
                if (Math.Abs(gravConst.mu * oomagr2 - y2b) < Math.Abs(gravConst.mu * oomagr2 - y2a))
                    x1 = -x1;

                // depending on the cross product, this will be normal or in plane,
                // or could even be a fan
                MathTimeLibr.cross(r1, r2, out rcrr);
                for (i = 0; i < 3; i++)
                    nvec[i] = rcrr[i] / MathTimeLibr.mag(rcrr); // if this is r1, v1, the transfer is coplanar!
                if ((dnu % (2.0 * Math.PI)) > Math.PI)
                {
                    for (i = 0; i < 3; i++)
                        nvec[i] = -nvec[i];
                }
            }

            MathTimeLibr.cross(nvec, r1, out rcrv);
            MathTimeLibr.cross(nvec, r2, out rcrr);
            x2 = x1 * Math.Cos(dnu) + a * Math.Sin(dnu);
            for (i = 0; i < 3; i++)
            {
                v1t[i] = (Math.Sqrt(gravConst.mu * p) * oomagr1) * ((x1 / gravConst.mu) * r1[i] + rcrv[i] * oomagr1);
                v2t[i] = (Math.Sqrt(gravConst.mu * p) * oomagr2) * ((x2 / gravConst.mu) * r2[i] + rcrr[i] * oomagr2);
            }
        }  // lambhodograph



        private static double kbattin
        (
        double v
        )
        {
            double sum1, delold, termold, del, term;
            int i;
            double[] d = new double[21]
               {
                  1.0 / 3.0, 4.0 / 27.0,
                  8.0 / 27.0, 2.0 / 9.0,
                  22.0 / 81.0, 208.0 / 891.0,
                  340.0 / 1287.0, 418.0 / 1755.0,
                  598.0 / 2295.0, 700.0 / 2907.0,
                  928.0 / 3591.0, 1054.0 / 4347.0,
                  1330.0 / 5175.0, 1480.0 / 6075.0,
                  1804.0 / 7047.0, 1978.0 / 8091.0,
                  2350.0 / 9207.0, 2548.0 / 10395.0,
                  2968.0 / 11655.0, 3190.0 / 12987.0,
                  3658.0 / 14391.0
               };

            // ----- process forwards ---- 
            delold = 1.0;
            termold = d[0];
            sum1 = termold;
            i = 1;
            while ((i <= 20) && (Math.Abs(termold) > 0.000000001))
            {
                del = 1.0 / (1.0 + d[i] * v * delold);
                term = termold * (del - 1.0);
                sum1 = sum1 + term;

                i = i + 1;
                delold = del;
                termold = term;
            }
            return (sum1);

            //int ktr = 20;
            //double sum2 = 0.0;
            //double term2 = 1.0 + d[ktr] * v;
            //for (i = 1; i <= ktr - 1; i++)
            //{
            //    sum2 = d[ktr - i] * v / term2;
            //    term2 = 1.0 + sum2;
            //}
            //return (d[0] / term2);
        }  // double kbattin


        /* -------------------------------------------------------------------------- */

        private static double seebattin(double v)
        {
            double eta, sqrtopv;
            double delold, termold, sum1, term, del;
            int i;
            double[] c = new double[21]
            {
              0.2,
              9.0 / 35.0, 16.0 / 63.0,
              25.0 / 99.0, 36.0 / 143.0,
              49.0 / 195.0, 64.0 / 255.0,
              81.0 / 323.0, 100.0 / 399.0,
              121.0 / 483.0, 144.0 / 575.0,
              169.0 / 675.0, 196.0 / 783.0,
              225.0 / 899.0, 256.0 / 1023.0,
              289.0 / 1155.0, 324.0 / 1295.0,
              361.0 / 1443.0, 400.0 / 1599.0,
              441.0 / 1763.0, 484.0 / 1935.0
            };

            sqrtopv = Math.Sqrt(1.0 + v);
            eta = v / Math.Pow(1.0 + sqrtopv, 2);

            // ---- process forwards ---- 
            delold = 1.0;
            termold = c[0];
            sum1 = termold;
            i = 1;
            while ((i <= 20) && (Math.Abs(termold) > 0.000000001))
            {
                del = 1.0 / (1.0 + c[i] * eta * delold);
                term = termold * (del - 1.0);
                sum1 = sum1 + term;

                i = i + 1;
                delold = del;
                termold = term;
            }
            return (1.0 / ((1.0 / (8.0 * (1.0 + sqrtopv))) * (3.0 + sum1 / (1.0 + eta * sum1))));

            // first term is diff, indices are offset too
            //double[] c1 = new double[20]
            //   {
            //    9.0 / 7.0, 16.0 / 63.0,
            //    25.0 / 99.0, 36.0 / 143.0,
            //    49.0 / 195.0, 64.0 / 255.0,
            //    81.0 / 323.0, 100.0 / 399.0,
            //    121.0 / 483.0, 144.0 / 575.0,
            //    169.0 / 675.0, 196.0 / 783.0,
            //    225.0 / 899.0, 256.0 / 1023.0,
            //    289.0 / 1155.0, 324.0 / 1295.0,
            //    361.0 / 1443.0, 400.0 / 1599.0,
            //    441.0 / 1763.0, 484.0 / 1935.0
            //   };

            // int ktr = 19;
            // double sum2  = 0.0;   
            // double term2 = 1.0 + c1[ktr] * eta;
            // for (i = 0; i <= ktr - 1; i++)
            // {
            //     sum2 = c1[ktr - i] * eta / term2;
            //     term2 = 1.0 + sum2;
            // }
            // return (8.0*(1.0 + sqrtopv) / 
            //          (3.0 + 
            //            (1.0 / 
            //              (5.0 + eta + ((9.0/7.0)*eta/term2 ) ) ) ) );

        }  // double seebattin


        /*------------------------------------------------------------------------------
        *
        *                           procedure lamberbattin
        *
        *  this procedure solves lambert's problem using battins method. the method is
        *    developed in battin (1987).
        *
        *  author        : david vallado           davallado@gmail.com   22 jun 2002
        *
        *  inputs          description                            range / units
        *    r1          - ijk position vector 1                      km
        *    r2          - ijk position vector 2                      km
        *    v1          - ijk velocity vector 1 if avail             km/s
        *    dm          - direction of motion                       'L', 'S'
        *    de          - orbital energy                            'L', 'H'
        *                  only affects nrev >= 1 solutions
        *    dtsec       - time between r1 and r2                     sec
        *    dtwait      - time to wait before starting               sec
        *    nrev        - number of revs to complete                 0, 1, 2, 3,  
        *    altpad      - altitude pad for hitearth calc             km
        *    show        - control output don't output for speed      'y', 'n'
        *
        *  outputs       :
        *    v1t         - ijk transfer velocity vector               km/s
        *    v2t         - ijk transfer velocity vector               km/s
        *    hitearth    - flag if hti or not                         'y', 'n'
        *    errorsum    - error flag                                 'ok',  
        *    errorout    - text for iterations / last loop
        *
        *  locals        :
        *    i           - index
        *    loops       -
        *    u           -
        *    b           -
        *    sinv        -
        *    cosv        -
        *    rp          -
        *    x           -
        *    xn          -
        *    y           -
        *    l           -
        *    m           -
        *    cosdeltanu  -
        *    sindeltanu  -
        *    dnu         -
        *    a           -
        *    tan2w       -
        *    ror         -
        *    h1          -
        *    h2          -
        *    tempx       -
        *    eps         -
        *    denom       -
        *    chord       -
        *    k2          -
        *    s           -
        *
        *  coupling      :
        *    mag         - magnitude of a vector
        *
        *  references    :
        *    vallado       2013, 494, Alg 59, ex 7-5
        *    thompson      AAS GNC 2018
        -----------------------------------------------------------------------------*/

        public void lambertbattin
               (
               double[] r1, double[] r2, double[] v1, char dm, char de, Int32 nrev, double dtwait, double dtsec,
               double altpad, char show,
               out double[] v1t, out double[] v2t, out char hitearth, out string errorsum, out string errorout
               )
        {
            const double small = 0.0000000001;
            double[] rcrossr = new double[3];
            int loops;
            double u, b, x, xn, y, L, m, cosdeltanu, sindeltanu, dnu, a,
                ror, h1, h2, tempx, eps, denom, chord, k2, s, f;
            double magr1, magr2, lam, temp, temp1, temp2, A, p, ecc, y1, rp;
            double[] v1dvl = new double[3];
            double[] v2dvl = new double[3];
            double[] v2 = new double[3];
            string errorstr;
            hitearth = 'x';
            errorstr = "ok";
            errorsum = "ok";
            errorout = "ok";

            // needed since assignments aren't at root level in procedure
            v1t = new double[] { 0.0, 0.0, 0.0 };
            v2t = new double[] { 0.0, 0.0, 0.0 };

            y = 0.0;

            magr1 = MathTimeLibr.mag(r1);
            magr2 = MathTimeLibr.mag(r2);

            cosdeltanu = MathTimeLibr.dot(r1, r2) / (magr1 * magr2);
            // make sure it's not more than 1.0
            if (Math.Abs(cosdeltanu) > 1.0)
                cosdeltanu = 1.0 * Math.Sign(cosdeltanu);

            MathTimeLibr.cross(r1, r2, out rcrossr);
            if (dm == 'S')
                sindeltanu = MathTimeLibr.mag(rcrossr) / (magr1 * magr2);
            else
                sindeltanu = -MathTimeLibr.mag(rcrossr) / (magr1 * magr2);
            dnu = Math.Atan2(sindeltanu, cosdeltanu);
            // the angle needs to be positive to work for the long way
            if (dnu < 0.0)
                dnu = 2.0 * Math.PI + dnu;

            // these are the same
            chord = Math.Sqrt(magr1 * magr1 + magr2 * magr2 - 2.0 * magr1 * magr2 * cosdeltanu);
            //chord = MathTimeLibr.mag(r2 - r1);

            s = (magr1 + magr2 + chord) * 0.5;
            ror = magr2 / magr1;
            eps = ror - 1.0;

            lam = 1.0 / s * Math.Sqrt(magr1 * magr2) * Math.Cos(dnu * 0.5);
            L = Math.Pow((1.0 - lam) / (1.0 + lam), 2);
            m = 8.0 * gravConst.mu * dtsec * dtsec / (s * s * s * Math.Pow(1.0 + lam, 6));

            // initial guess
            if (nrev > 0)
                xn = 1.0 + 4.0 * L;
            else
                xn = L;   //l    // 0.0 for par and hyp, l for ell

            // alt approach for high energy(long way, multi-nrev) case
            if ((de.Equals('H')) && (nrev > 0))
            {
                h1 = 0.0;
                h2 = 0.0;
                b = 0.0;
                f = 0.0;
                xn = 1e-20;  // be sure to reset this here!!
                x = 10.0;  // starting value
                loops = 1;
                while ((Math.Abs(xn - x) >= small) && (loops <= 20))
                {
                    x = xn;
                    temp = 1.0 / (2.0 * (L - x * x));
                    temp1 = Math.Sqrt(x);
                    temp2 = (nrev * Math.PI * 0.5 + Math.Atan(temp1)) / temp1;
                    h1 = temp * (L + x) * (1.0 + 2.0 * x + L);
                    h2 = temp * m * temp1 * ((L - x * x) * temp2 - (L + x));

                    b = 0.25 * 27.0 * h2 / (Math.Pow(temp1 * (1.0 + h1), 3));
                    if (b < 0.0) // reset the initial condition
                        f = 2.0 * Math.Cos(1.0 / 3.0 * Math.Acos(Math.Sqrt(b + 1.0)));
                    else
                    {
                        A = Math.Pow(Math.Sqrt(b) + Math.Sqrt(b + 1.0), (1.0 / 3.0));
                        f = A + 1.0 / A;
                    }

                    y = 2.0 / 3.0 * temp1 * (1.0 + h1) * (Math.Sqrt(b + 1.0) / f + 1.0);
                    xn = 0.5 * ((m / (y * y) - (1.0 + L)) - Math.Sqrt(Math.Pow(m / (y * y) - (1.0 + L), 2) - 4.0 * L));
                    // set if NANs occur   
                    if (Double.IsNaN(y))
                    {
                        y = 75.0;
                        xn = 1.0;
                    }

                    // output to show all iterations, and also uncomment errorsum in show == 'n'
                    if (show == 'y')
                        errorout = errorout + "\n       ok " + loops.ToString().PadLeft(4) + nrev.ToString().PadLeft(3) + "   " + dm + "  " + de +
                            (Math.Acos(cosdeltanu) * 180.0 / Math.PI).ToString("0.0000000").PadLeft(15) + "  " + dtwait.ToString("0.0000000").PadLeft(12) + " " +
                             dtsec.ToString("0.######").PadLeft(15) + " yh " + y.ToString("0.#######").PadLeft(11) + " x " + x.ToString("0.#######").PadLeft(11) +
                             " h1 " + h1.ToString("0.#######").PadLeft(11) + " h2 " + h2.ToString("0.#######").PadLeft(11) +
                             " b " + b.ToString("0.#######").PadLeft(11) + " f " + f.ToString("0.#######").PadLeft(11) +
                             " dt " + dtsec.ToString("0.######");

                    loops = loops + 1;
                }  // while

                x = xn;
                a = s * Math.Pow(1.0 + lam, 2) * (1.0 + x) * (L + x) / (8.0 * x);
                p = (2.0 * magr1 * magr2 * (1.0 + x) * Math.Pow(Math.Sin(dnu * 0.5), 2)) / (s * Math.Pow(1.0 + lam, 2) * (L + x));  // thompson
                ecc = Math.Sqrt(1.0 - p / a);
                rp = a * (1.0 - ecc);
                lambhodograph(r1, r2, v1, p, ecc, dnu, dtsec, out v1t, out v2t);

                if (show == 'y')
                    errorsum = errorstr + "high " + nrev.ToString().PadLeft(3) + "   " + dm + "  " + de + "  " + dtwait.ToString("0.0000000").PadLeft(12) + " " +
                        dtsec.ToString("0.#######").PadLeft(15) + " " + x.ToString("0.#######").PadLeft(15) + " " +
                        v1t[0].ToString("0.0000000").PadLeft(15) + v1t[1].ToString("0.0000000").PadLeft(15) + v1t[2].ToString("0.0000000").PadLeft(15) +
                        v2t[0].ToString("0.0000000").PadLeft(15) + v2t[1].ToString("0.0000000").PadLeft(15) + v2t[2].ToString("0.0000000").PadLeft(15) +
                        " " + loops.ToString().PadLeft(4) + " " + rp.ToString("0.0000000").PadLeft(15) + " " + a.ToString("0.0000000").PadLeft(15) +
                        " " + dnu.ToString("0.0000000").PadLeft(15) + " " + hitearth;
            }
            else
            {
                // standard processing, low energy
                k2 = 0.0;
                b = 0.0;
                u = 0.0;
                // note that the r nrev = 0 case is not assumed here
                loops = 1;
                y1 = 0.0;
                x = 10.0;  // starting value
                while ((Math.Abs(xn - x) >= small) && (loops <= 30))
                {
                    if (nrev > 0)
                    {
                        x = xn;
                        temp = 1.0 / ((1.0 + 2.0 * x + L) * (4.0 * x * x));
                        temp1 = (nrev * Math.PI * 0.5 + Math.Atan(Math.Sqrt(x))) / Math.Sqrt(x);
                        h1 = temp * Math.Pow(L + x, 2) * (3.0 * Math.Pow(1.0 + x, 2) * temp1 - (3.0 + 5.0 * x));
                        h2 = temp * m * ((x * x - x * (1.0 + L) - 3.0 * L) * temp1 + (3.0 * L + x));
                    }
                    else
                    {
                        x = xn;
                        tempx = seebattin(x);
                        denom = 1.0 / ((1.0 + 2.0 * x + L) * (4.0 * x + tempx * (3.0 + x)));
                        h1 = Math.Pow(L + x, 2) * (1.0 + 3.0 * x + tempx) * denom;
                        h2 = m * (x - L + tempx) * denom;
                    }

                    // ----------------------- evaluate cubic------------------
                    b = 0.25 * 27.0 * h2 / (Math.Pow(1.0 + h1, 3));

                    u = 0.5 * b / (1.0 + Math.Sqrt(1.0 + b));
                    k2 = kbattin(u);
                    y = ((1.0 + h1) / 3.0) * (2.0 + Math.Sqrt(1.0 + b) / (1.0 + 2.0 * u * k2 * k2));
                    xn = Math.Sqrt(Math.Pow((1.0 - L) * 0.5, 2) + m / (y * y)) - (1.0 + L) * 0.5;

                    y1 = Math.Sqrt(m / ((L + x) * (1.0 + x)));
                    loops = loops + 1;

                    if (Double.IsNaN(y))
                    {
                        y = 75.0;
                        xn = 1.0;
                    }

                    // output to show all iterations, and also uncomment errorsum in show == 'n'
                    if (show == 'y')
                        errorout = errorout + "\n       ok " + loops.ToString().PadLeft(4) + nrev.ToString().PadLeft(3) + "   " + dm + "  " + de +
                            (Math.Acos(cosdeltanu) * 180.0 / Math.PI).ToString("0.0000000").PadLeft(15) + "  " + dtwait.ToString("0.0000000").PadLeft(12) + " " +
                            dtsec.ToString("0.######").PadLeft(15) + " yb " + y.ToString("0.######").PadLeft(11) + " x " + x.ToString("0.######").PadLeft(11) +
                            " k2 " + k2.ToString("0.######").PadLeft(11) + " b " + b.ToString("0.######").PadLeft(11) +
                            " u " + u.ToString("0.######").PadLeft(11) + " y1 " + y1.ToString("0.######").PadLeft(11);
                }  // while

                if (loops < 30)
                {
                    p = (2.0 * magr1 * magr2 * y * y * Math.Pow(1.0 + x, 2) * Math.Pow(Math.Sin(dnu * 0.5), 2)) / (m * s * Math.Pow(1.0 + lam, 2));  // thompson
                    ecc = Math.Sqrt((eps * eps + 4.0 * magr2 / magr1 * Math.Pow(Math.Sin(dnu * 0.5), 2) * Math.Pow((L - x) / (L + x), 2)) / (eps * eps + 4.0 * magr2 / magr1 * Math.Pow(Math.Sin(dnu * 0.5), 2)));
                    a = 1.0;   // fix
                    rp = a * (1.0 - ecc);
                    lambhodograph(r1, r2, v1, p, ecc, dnu, dtsec, out v1t, out v2t);

                    if (show == 'y')
                        errorsum = errorstr + "low " + nrev.ToString().PadLeft(3) + "   " + dm + "  " + de + "  " + dtwait.ToString("0.00000").PadLeft(12) + " " +
                            dtsec.ToString("0.00000").PadLeft(15) + " " + x.ToString("0.000000").PadLeft(15) + " " +
                            v1t[0].ToString("0.0000000").PadLeft(15) + v1t[1].ToString("0.0000000").PadLeft(15) + v1t[2].ToString("0.0000000").PadLeft(15) +
                            v2t[0].ToString("0.0000000").PadLeft(15) + v2t[1].ToString("0.0000000").PadLeft(15) + v2t[2].ToString("0.0000000").PadLeft(15) +
                            " " + loops.ToString().PadLeft(4) + " " + rp.ToString("0.0000000").PadLeft(15) + " " + a.ToString("0.0000000").PadLeft(15) +
                            " " + dnu.ToString("0.0000000").PadLeft(15) + " " + hitearth;
                }  // if loops converged < 30
            }  // if nrev and r

        }  //  lambertbattin


        // ------------------------------------------------------------------------------
        //
        //                           function hillsr
        //
        //  this function calculates various position information for hills equations.
        //
        //  author        : david vallado           davallado@gmail.com    1 mar 2001
        //
        //  inputs description                                 range / units
        //    r           - init rel position of int              m or km
        //    v           - init rel velocity of int              m or km/s
        //    alt         - altitude of tgt satellite               km
        //    dts         - desired time                            s
        //
        //  outputs       :
        //    rinit       - final rel position of int             m or km
        //    vinit       - final rel velocity of int             m or km/s
        //
        //  locals        :
        //    nt          - angular velocity times time             rad
        //    omega       -
        //    sinnt       - sine of nt
        //    cosnt       - cosine of nt
        //    radius      - magnitude of range vector               km
        //
        //  coupling      :
        //
        //  references    :
        //    vallado       2007, 397, alg 47, ex 6-14
        // ------------------------------------------------------------------------------

        public void hillsr
            (double[] r, double[] v, double alt, double dts,
            out double[] rint, out double[] vint
            )
        {
            // --------------------  implementation   ----------------------
            double radius, cosnt, sinnt, nt, omega;
            rint = new double[3];
            vint = new double[3];

            radius = gravConst.re + alt; // in km
            omega = Math.Sqrt(gravConst.mu / (radius * radius * radius)); // rad/s
            nt = omega * dts;
            cosnt = Math.Cos(nt);
            sinnt = Math.Sin(nt);

            // --------------- determine new positions  --------------------
            rint[0] = (v[0] / omega) * sinnt -
                     ((2.0 * v[1] / omega) + 3.0 * r[0]) * cosnt +
                     ((2.0 * v[1] / omega) + 4.0 * r[0]);
            rint[1] = (2.0 * v[0] / omega) * cosnt +
                     (4.0 * v[1] / omega + 6.0 * r[0]) * sinnt +
                     (r[1] - (2.0 * v[0] / omega)) -
                     (3.0 * v[1] + 6.0 * omega * r[0]) * dts;
            rint[2] = r[2] * cosnt + (v[2] / omega) * sinnt;

            // --------------- determine new velocities  -------------------
            vint[0] = v[0] * cosnt + (2.0 * v[1] + 3.0 * omega * r[0]) * sinnt;
            vint[1] = -2.0 * v[0] * sinnt + (4.0 * v[1] + 6.0 * omega * r[0]) * cosnt - (3.0 * v[1] + 6.0 * omega * r[0]);
            vint[2] = -r[2] * omega * sinnt + v[2] * cosnt;
        }  // hillsr



        // ------------------------------------------------------------------------------
        //
        //                           function hillsv
        //
        //  this function calculates initial velocity for hills equations.
        //
        //  author        : david vallado           davallado@gmail.com    1 mar 2001
        //
        //  inputs description                                     range / units
        //    r           - initial position vector of int            m
        //    alt         - altitude of tgt satellite                 km
        //    dts         - desired time                              s
        //
        //  outputs       :
        //    v           - initial velocity vector of int            m / s
        //
        //  locals        :
        //    numkm       -
        //    denom       -
        //    nt          - angular velocity times time               rad
        //    omega       -
        //    sinnt       - sine of nt
        //    cosnt       - cosine of nt
        //    radius      - magnitude of range vector                 km
        //
        //  coupling      :
        //    none.
        //
        //  references    :
        //    vallado       2007, 410, eq 6-60, ex 6-15
        // ------------------------------------------------------------------------------

        public void hillsv
            (double[] r, double alt, double dts, out double[] v)
        {
            double radius, omega, nt, cosnt, sinnt, denom, numkm;
            v = new double[3];

            // --------------------implementation----------------------
            radius = gravConst.re + alt;
            omega = Math.Sqrt(gravConst.mu / (radius * radius * radius));
            nt = omega * dts;
            cosnt = Math.Cos(nt);
            sinnt = Math.Sin(nt);

            // ---------------determine initial velocity ------------------
            numkm = ((6.0 * r[0] * (nt - sinnt) - r[1]) * omega * sinnt
                    - 2.0 * omega * r[0] * (4.0 - 3.0 * cosnt) * (1.0 - cosnt));
            denom = (4.0 * sinnt - 3.0 * nt) * sinnt + 4.0 * (1.0 - cosnt) * (1.0 - cosnt);

            if (Math.Abs(denom) > 0.000001)
                v[1] = numkm / denom;
            else
                v[1] = 0.0;
            if (Math.Abs(sinnt) > 0.000001)
                v[0] = -(omega * r[0] * (4.0 - 3.0 * cosnt) + 2.0 * (1.0 - cosnt) * v[1]) / (sinnt);
            else
                v[0] = 0.0;
            v[2] = -r[2] * omega * MathTimeLibr.cot(nt);
        }  // hillsv


        /*---------------------------------------------------------------------------
        *
        *                           procedure site
        *
        *  this function finds the position and velocity vectors for a site.  the
        *    answer is returned in the geocentric equatorial (ecef) coordinate system.
        *    note that the velocity is zero because the coordinate system is fixed to
        *    the earth.
        *
        *  author        : david vallado           davallado@gmail.com   25 jun 2002
        *
        *  inputs          description                    range / units
        *    latgd       - geodetic latitude              -Math.PI/2 to Math.PI/2 rad
        *    lon         - longitude of site              -2pi to 2pi rad
        *    alt         - altitude                       km
        *
        *  outputs       :
        *    rsecef      - ecef site position vector      km
        *    vsecef      - ecef site velocity vector      km/s
        *
        *  locals        :
        *    Math.Sinlat      - variable containing  Math.Sin(lat)  rad
        *    temp        - temporary real value
        *    rdel        - rdel component of site vector  km
        *    rk          - rk component of site vector    km
        *    cearth      -
        *
        *  coupling      :
        *    none
        *
        *  references    :
        *    vallado       2013, 430, alg 51, ex 7-1
        ----------------------------------------------------------------------------*/

        public void site
            (
            double latgd, double lon, double alt,
            out double[] rsecef, out double[] vsecef
            )
        {
            const double eesqrd = 0.00669437999013;
            double sinlat, cearth, rdel, rk;

            // needed since assignments arn't at root level in procedure
            rsecef = new double[] { 0.0, 0.0, 0.0 };
            vsecef = new double[] { 0.0, 0.0, 0.0 };

            /* ---------------------  initialize values   ------------------- */
            sinlat = Math.Sin(latgd);

            /* -------  find rdel and rk components of site vector  --------- */
            cearth = gravConst.re / Math.Sqrt(1.0 - (eesqrd * sinlat * sinlat));
            rdel = (cearth + alt) * Math.Cos(latgd);
            rk = ((1.0 - eesqrd) * cearth + alt) * sinlat;

            /* ----------------  find site position vector  ----------------- */
            rsecef[0] = rdel * Math.Cos(lon);
            rsecef[1] = rdel * Math.Sin(lon);
            rsecef[2] = rk;

            /* ----------------  find site velocity vector  ----------------- */
            vsecef[0] = 0.0;
            vsecef[1] = 0.0;
            vsecef[2] = 0.0;
        }  // site


        // -------------------------- angles-only techniques ------------------------ 


        /*------------------------------------------------------------------------------
        *
        *                           procedure angleslaplace
        *
        *  this procedure solves the problem of orbit determination using three
        *    optical sightings and the method of laplace. the 8th order root is generally 
        *    the big point of discussion. A Halley iteration permits a quick solution to 
        *    find the correct root, with a starting guess of 20000 km. the general 
        *    formulation yields polynomial coefficients that are very large, and can easily 
        *    become overflow operations. Thus, canonical units are used only until teh root is found, 
        *    then regular units are resumed. 
        *
        *  author        : david vallado           davallado@gmail.com   22 jun 2002
        *
        *  inputs          description                               range / units
        *    tdecl1       - declination #1                               rad
        *    tdecl2       - declination #2                               rad
        *    tdecl3       - declination #3                               rad
        *    trtasc1      - right ascension #1                           rad
        *    trtasc2      - right ascension #2                           rad
        *    trtasc3      - right ascension #3                           rad
        *    jd1, jdf1    - julian date of 1st sighting                  days from 4713 bc
        *    jd2, jdf2    - julian date of 2nd sighting                  days from 4713 bc
        *    jd3, jdf3    - julian date of 3rd sighting                  days from 4713 bc
        *    diffsites    - if sites are different (need better test)    'y', 'n'
        *    rs1          - eci site position vector #1                  km
        *    rs2          - eci site position vector #2                  km
        *    rs3          - eci site position vector #3                  km
        *
        *  outputs        :
        *    r2           -  position vector                             km
        *    v2           -  velocity vector                             km / s
        *    errstr       - output results for debugging
        *
        *  locals         :
        *    l1           - line of sight vector for 1st
        *    l2           - line of sight vector for 2nd
        *    l3           - line of sight vector for 3rd
        *    ldot         - 1st derivative of l2
        *    lddot        - 2nd derivative of l2
        *    rs2dot       - 1st derivative of rs2 - vel
        *    rs2ddot      - 2nd derivative of rs2
        *    t12t13       - (t1-t2) * (t1-t3)
        *    t21t23       - (t2-t1) * (t2-t3)
        *    t31t32       - (t3-t1) * (t3-t2)
        *    i            - index
        *    d            -
        *    d1           -
        *    d2           -
        *    d3           -
        *    d4           -
        *    oldr         - previous iteration on r2
        *    rho          - range from site to satellite at t2
        *    rhodot       -
        *    dmat         -
        *    d1mat        -
        *    d2mat        -
        *    d3mat        -
        *    d4mat        -
        *    earthrate    - angular rotation of the earth
        *    l2dotrs      - vector l2 dotted with rs
        *    temp         - temporary vector
        *    temp1        - temporary vector
        *    small        - tolerance
        *    roots        -
        *
        *  coupling       :
        *    mag          - magnitude of a vector
        *    determinant  - evaluate the determinant of a matrix
        *    cross        - cross product of two vectors
        *    norm         - normlize a matrix
        *    sgnval       - sgn a value to a matrix
        *    factor       - find the roots of a polynomial
        *
        *  references     :
        *    vallado       2013, 435
        -----------------------------------------------------------------------------*/

        public void angleslaplace
        (
                double tdecl1, double tdecl2, double tdecl3,
                double trtasc1, double trtasc2, double trtasc3,
                double jd1, double jdf1, double jd2, double jdf2, double jd3, double jdf3, char diffsites,
                double[] rs1, double[] rs2, double[] rs3,
                out double[] r2, out double[] v2,
                out double bigr2, out string errstr
        )
        {
            double[] poly = new double[10];
            double[,] roots = new double[15, 2];
            double[,] dmat = new double[3, 3];
            double[,] dmat1 = new double[3, 3];
            double[,] dmat2 = new double[3, 3];
            double[,] dmat3 = new double[3, 3];
            double[,] dmat4 = new double[3, 3];
            double[,] dmat1c = new double[3, 3];
            double[,] dmat2c = new double[3, 3];
            double[,] dmat3c = new double[3, 3];
            double[,] dmat4c = new double[3, 3];
            double[] los1 = new double[3];
            double[] los2 = new double[3];
            double[] los3 = new double[3];
            double[] ldot = new double[3];
            double[] lddot = new double[3];
            double[] rs2cdot = new double[3];
            double[] rs2cddot = new double[3];
            double[] earthrate = new double[3];
            double[] earthratec = new double[3];
            double[] temp = new double[3];
            double[] temp1 = new double[3];
            double[] r1 = new double[3];
            double[] r2c = new double[3];
            double[] v2c = new double[3];
            double[] rs1c = new double[3];
            double[] rs2c = new double[3];
            double[] rs3c = new double[3];
            double[] r3 = new double[3];
            double deriv, deriv1, deriv2, bigr2c;
            double[,] lmatii = new double[3, 3];
            double[,] cmat = new double[3, 3];
            double[,] rhomat = new double[3, 3];
            double[,] lmati = new double[3, 3];
            double[,] rsmat = new double[3, 3];
            double[,] lir = new double[3, 3];
            //double p, a, ecc, incl, raan, argp, nu, u, m, l, argper;
            string chk3roots;
            char show;
            show = 'a';  // show all
            show = 'y';  // show just root info
            bigr2 = 0.0;

            r2 = new double[] { 0.0, 0.0, 0.0 };
            v2 = new double[] { 0.0, 0.0, 0.0 };
            errstr = "";

            double d, d1c, d2c, d3c, d4c, rho, s1, s2, s3, s4, s5, s6, rhodot, tau12, tau32, tau13,
                l2dotrs;
            double tau12c, tau32c, tau13c;

            // canonical only through forming and solving the 8th order polynomial
            double tu = Math.Sqrt(gravConst.re * gravConst.re * gravConst.re / gravConst.mu);

            // ----------------------   initialize   ------------------------
            double lod = 0.0;  // leave zero for now
            // switch to canonical
            double omegaearthc = gravConst.earthrot * (1.0 - lod / 86400.0) * tu;  // rad/tu
            //double omegaearth = gravConst.earthrot * (1.0 - lod / 86400.0);  // rad/s
            earthratec[0] = 0.0;
            earthratec[1] = 0.0;
            earthratec[2] = omegaearthc;

            // ---- set middle to 0, deltas to other times ----
            // need to separate into jd and jdf portions for accuracy since it's often only seconds or minutes
            tau12 = (jd1 - jd2) * 86400.0 + (jdf1 - jdf2) * 86400.0; // days to sec
            tau13 = (jd1 - jd3) * 86400.0 + (jdf1 - jdf3) * 86400.0;
            tau32 = (jd3 - jd2) * 86400.0 + (jdf3 - jdf2) * 86400.0;

            // switch to canonical
            tau12c = tau12 / tu;
            tau13c = tau13 / tu;
            tau32c = tau32 / tu;

            if (show == 'a' || show == 'y')
                errstr = errstr + "tau12 " + tau12.ToString() + " tau13 " + tau13.ToString() + " tau32 " + tau32.ToString() + "\n";

            // switch to canonical
            for (int i = 0; i < 3; i++)
            {
                rs1c[i] = rs1[i] / gravConst.re;
                rs2c[i] = rs2[i] / gravConst.re;
                rs3c[i] = rs3[i] / gravConst.re;
            }

            // --------------- find line of sight vectors -------------------
            // these are unitless
            los1[0] = Math.Cos(tdecl1) * Math.Cos(trtasc1);
            los1[1] = Math.Cos(tdecl1) * Math.Sin(trtasc1);
            los1[2] = Math.Sin(tdecl1);
            los2[0] = Math.Cos(tdecl2) * Math.Cos(trtasc2);
            los2[1] = Math.Cos(tdecl2) * Math.Sin(trtasc2);
            los2[2] = Math.Sin(tdecl2);
            los3[0] = Math.Cos(tdecl3) * Math.Cos(trtasc3);
            los3[1] = Math.Cos(tdecl3) * Math.Sin(trtasc3);
            los3[2] = Math.Sin(tdecl3);

            if (show == 'a')
            {
                errstr = errstr + "los1 " + los1[0].ToString("0.000000000000") + " " +
                los1[1].ToString("0.000000000000") + " " + los1[2].ToString("0.000000000000") +
                " " + MathTimeLibr.mag(los1).ToString("0.000000000000") + "\n";
                errstr = errstr + "los2 " + los2[0].ToString("0.000000000000") + " " +
                    los2[1].ToString("0.000000000000") + " " + los2[2].ToString("0.000000000000") +
                    " " + MathTimeLibr.mag(los2).ToString("0.000000000000") + "\n";
                errstr = errstr + "los3 " + los3[0].ToString("0.000000000000") + " " +
                    los3[1].ToString("0.000000000000") + " " + los3[2].ToString("0.0000000000") +
                    " " + MathTimeLibr.mag(los3).ToString("0.000000000000") + "\n";
            }

            // --------------------------------------------------------------
            // using lagrange interpolation formula to derive an expression
            // for l(t), substitute t=t2=0 and differentiate to obtain the
            // derivatives of los.
            // ---------------------------------------------------------------
            s1 = -tau32c / (tau12c * tau13c);
            s2 = (tau12c + tau32c) / (tau12c * tau32c); // be careful here! it's -t1-t3 which can be 0
            s3 = -tau12c / (-tau13c * tau32c);
            s4 = 2.0 / (tau12c * tau13c);
            s5 = 2.0 / (tau12c * tau32c);
            s6 = 2.0 / (-tau13c * tau32c);

            // Escobal says that for Earth orbiting satellites the ldot and lddot need additional terms
            for (int i = 0; i < 3; i++)
            {
                ldot[i] = s1 * los1[i] + s2 * los2[i] + s3 * los3[i];  // rad / s
                lddot[i] = s4 * los1[i] + s5 * los2[i] + s6 * los3[i];  // rad / s^2
            }

            if (show == 'a')
            {
                errstr = errstr + "ldot " + ldot[0].ToString() + " " + ldot[1].ToString() + " " + ldot[2].ToString() + "\n";
                errstr = errstr + "lddot " + lddot[0].ToString() + " " + lddot[1].ToString() + " " + lddot[2].ToString() + "\n";
            }

            // -------------------- find 2nd derivative of rs ---------------
            MathTimeLibr.cross(rs1c, rs2c, out temp);
            MathTimeLibr.cross(rs2c, rs3c, out temp1);

            // need discriminator, since eci they will differ
            if (diffsites == 'n')
            {
                // ------- all sightings from same sites ---------
                MathTimeLibr.cross(earthratec, rs2c, out rs2cdot);
                MathTimeLibr.cross(earthratec, rs2cdot, out rs2cddot);
                if (show == 'a')
                {
                    errstr = errstr + "rs2dot " + rs2cdot[0].ToString() + " " + rs2cdot[1].ToString() + " " + rs2cdot[2].ToString() + "ER/TU \n";
                    errstr = errstr + "rs2ddot " + rs2cddot[0].ToString() + " " + rs2cddot[1].ToString() + " " + rs2cddot[2].ToString() + "\n";
                }
            }
            else
            {
                // ---- each sighting from different sites ----
                for (int i = 0; i < 3; i++)
                {
                    // switch to canonical
                    rs2cdot[i] = s1 * rs1c[i] + s2 * rs2c[i] + s3 * rs3c[i];
                    rs2cddot[i] = s4 * rs1c[i] + s5 * rs2c[i] + s6 * rs3c[i];
                }
                if (show == 'a')
                {
                    errstr = errstr + "rs2dot " + rs2cdot[0].ToString() + " " + rs2cdot[1].ToString() + " " + rs2cdot[2].ToString() + "ER/TU\n";
                    errstr = errstr + "rs2ddot " + rs2cddot[0].ToString() + " " + rs2cddot[1].ToString() + " " + rs2cddot[2].ToString() + "\n";
                }
            }
            for (int i = 0; i < 3; i++)
            {
                dmat[i, 0] = los2[i];  // rad
                dmat[i, 1] = ldot[i];  // rad / TU
                dmat[i, 2] = lddot[i];  // rad / TU^2

                // ----  position determinants ----
                dmat1c[i, 0] = los2[i];
                dmat1c[i, 1] = ldot[i];
                dmat1c[i, 2] = rs2cddot[i];

                dmat2c[i, 0] = los2[i];
                dmat2c[i, 1] = ldot[i];
                dmat2c[i, 2] = rs2c[i];

                // ----  velocity determinants ----
                dmat3c[i, 0] = los2[i];
                dmat3c[i, 1] = rs2cddot[i];
                dmat3c[i, 2] = lddot[i];

                dmat4c[i, 0] = los2[i];
                dmat4c[i, 1] = rs2c[i];
                dmat4c[i, 2] = lddot[i];
            }

            d = 2.0 * MathTimeLibr.determinant(dmat, 3);
            d1c = MathTimeLibr.determinant(dmat1c, 3);
            d2c = MathTimeLibr.determinant(dmat2c, 3);
            d3c = MathTimeLibr.determinant(dmat3c, 3);
            d4c = MathTimeLibr.determinant(dmat4c, 3);
            if (show == 'a')
                errstr = errstr + "d " + d.ToString() + " d1 " + d1c.ToString() + " d2 " + d2c.ToString() + "\n";

            if (Math.Abs(d) > 1.0e-12)
            {
                // ---------------- solve eighth order poly -----------------
                // switch to canonical
                l2dotrs = MathTimeLibr.dot(los2, rs2c);
                //l2dotrs = MathTimeLibr.dot(los2, rs2);
                poly[1] = 1.0; // r2^8th variable!!!!!!!!!!!!!!
                poly[2] = 0.0;
                // switch to canonical
                poly[3] = (l2dotrs * 4.0 * d1c / d - 4.0 * d1c * d1c / (d * d) -
                           MathTimeLibr.mag(rs2c) * MathTimeLibr.mag(rs2c));
                poly[4] = 0.0;
                poly[5] = 0.0;
                // switch to canonical
                poly[6] = l2dotrs * 4.0 * d2c / d - 8.0 * d1c * d2c / (d * d);
                //poly[6] = gravConst.mu * (l2dotrs * 4.0 * d2 / d - 8.0 * d1 * d2 / (d * d));  
                poly[7] = 0.0;
                poly[8] = 0.0;
                // switch to canonical
                poly[9] = -4.0 * d2c * d2c / (d * d);
                //poly[9] = -gravConst.mu * gravConst.mu * 4.0 * d2 * d2 / (d * d); 

                if (poly[3] < 0.0 && poly[6] > 0.0)
                    errstr = errstr + "LAPLACE may have multiple roots 1.0  0.0  " + poly[3] + "  0.0 0.0 " + poly[6] + " 0.0 0.0 " + poly[9] + "\n";

                //Console.WriteLine( "Poly------------------------------");
                //Console.WriteLine( poly[1] + " " + poly[2] + " " + poly[3] + " " + poly[4]);
                //Console.WriteLine( poly[5] + " " + poly[6] + " " + poly[7] + " " + poly[8] + " " + poly[9]);
                // note that there will usually be only 1 real positive root (See Der AAS 19-626)
                // only in cases where poly[3] is negative and poly[6] is positive could there be multiple
                // positive real roots. 
                // the real positive root is the one to select. 
                //MathTimeLibr.factor(poly, 8, out roots);

                // simply iterate to find the correct root
                bigr2 = 100.0;
                // can iterate Curtis p 289 simple derivative of the poly
                // makes sense since the values are so huge in the polynomial. JAS 2015 Halley back subs sometimes better, but not always
                // do as Halley iteration since derivatives are possible. tests at LEO, GPS, GEO,
                // all seem to converge to the proper answer
                int kk = 0;
                // switch to canonical units
                bigr2c = 20000.0 / gravConst.re; // guess ~GPS altitude
                while (Math.Abs(bigr2 - bigr2c) > 8.0e-5 && kk < 15)  // do in er, 0.5 km
                {
                    bigr2 = bigr2c;
                    deriv = Math.Pow(bigr2, 8) + poly[3] * Math.Pow(bigr2, 6)
                        + poly[6] * Math.Pow(bigr2, 3) + poly[9];
                    deriv1 = 8.0 * Math.Pow(bigr2, 7) + 6.0 * poly[3] * Math.Pow(bigr2, 5)
                        + 3.0 * poly[6] * Math.Pow(bigr2, 2);
                    deriv2 = 56.0 * Math.Pow(bigr2, 6) + 30.0 * poly[3] * Math.Pow(bigr2, 4)
                        + 6.0 * poly[6] * bigr2;
                    // Newton iteration
                    //bigr2n = bigr2 - deriv / derivn1;
                    // Halley iteration
                    bigr2c = bigr2 - (2.0 * deriv * deriv1) / (2.0 * deriv1 * deriv1 - deriv * deriv2);
                    if (show == 'a')
                        errstr = errstr + "bigr itera " + bigr2c.ToString() + " " + bigr2.ToString() + "\n";
                    kk = kk + 1;
                }

//                if (show == 'y' || show == 'a')
                {
                    errstr = errstr + "Poly--------- " + poly[1] + " " + poly[3] + " " + poly[6] + " " + poly[9]
                    + " tau " + tau12.ToString() + " " + tau32.ToString() + " sec ";

                    if (poly[3] < 0.0 && poly[6] > 0.0)
                        chk3roots = "3 root poss ";
                    else
                        chk3roots = "1 root poss ";
                    errstr = errstr + chk3roots + " root pick " + (bigr2c * gravConst.re).ToString(); // + "\n";
                }

                //if (bigr2c < 0.0 || bigr2c * gravConst.re > 75000.0)
                //    bigr2c = 40000.0 / gravConst.re;  // simply set this to about GEO, allowing for less than that too. 

                // still in canonical units
                rho = -2.0 * d1c / d - 2.0 * d2c / (bigr2c * bigr2c * bigr2c * d);
                //rho = -2.0 * d1 / d - gravConst.mu * 2.0 * d2 / (bigr2 * bigr2 * bigr2 * d);

                // ---- find the middle position vector ----
                for (int k = 0; k < 3; k++)
                    r2c[k] = rho * los2[k] + rs2c[k];

                if (show == 'a')
                    errstr = errstr + " r2 " + r2c[0].ToString() + " " + r2c[1].ToString() + " "
                          + r2c[2].ToString() + " diffsites " + diffsites + "\n"
                          + " rho " + rho.ToString() + " d1/d " + (d1c / d).ToString() + " d2/d " + (d2c / d).ToString() + "\n";

                // -------- find rhodot MathTimeLibr.magnitude --------
                rhodot = -d3c / d - d4c / (bigr2c * bigr2c * bigr2c * d);
                //rhodot = -d3 / d - gravConst.mu * d4 / (bigr2 * bigr2 * bigr2 * d);

                // -----find middle velocity vector---- -
                for (int i = 0; i < 3; i++)
                    v2c[i] = rhodot * los2[i] + rho * ldot[i] + rs2cdot[i];

                // now convert back to normal units
                for (int i = 0; i < 3; i++)
                {
                    r2[i] = r2c[i] * gravConst.re;
                    v2[i] = v2c[i] * gravConst.re / tu;
                }
                // for passing back
                bigr2 = bigr2c * gravConst.re;
            }
            else
            {  // Escobal cases where d = 0.0
               // he solves for r2 magnitude direct, but not the vector
               // need to derive
               //

                //rho = -2.0 * d1 / d - gravConst.mu * 2.0 * d2 / (bigr2 * bigr2 * bigr2 * d);

                //// ---- find the middle position vector ----
                //for (int k = 0; k < 3; k++)
                //    r2[k] = rho * los2[k] + rs2[k];

                //errstr = errstr + " r2 " + r2[0].ToString() + " " + r2[1].ToString() + " "
                //          + r2[2].ToString() + " diffsites " + diffsites + "\n"
                //          + " rho " + rho.ToString() + " d1/d " + (d1 / d).ToString() + " d2/d " + (d2 / d).ToString() + "\n";

                //// -------- find rhodot MathTimeLibr.magnitude --------
                //rhodot = -d3 / d - gravConst.mu * d4 / (bigr2 * bigr2 * bigr2 * d);

                //// -----find middle velocity vector---- -
                //for (int i = 0; i < 3; i++)
                //    v2[i] = rhodot * los2[i] + rho * ldot[i] + rs2dot[i];

                if (show == 'a')
                    errstr = errstr + "Laplace Determinant value was zero " + d.ToString() + "\n";
            }

        }  // angleslaplace


        // ---------------------------------------------------------------------------------
        // get root from Gauss to use as a seed for double-r and Gooding
        // note bigr2 is output in km, but calcs are in cannonical
        public void getGaussRoot
            (
            double tdecl1, double tdecl2, double tdecl3,
            double trtasc1, double trtasc2, double trtasc3,
            double jd1, double jdf1, double jd2, double jdf2, double jd3, double jdf3,
            double[] rseci1, double[] rseci2, double[] rseci3, out double bigr2
            )
        {
            double[] poly = new double[10];
            double[] los1 = new double[3];
            double[] los2 = new double[3];
            double[] los3 = new double[3];
            double[,] lmat = new double[3, 3];
            double[] cmat = new double[3];
            double[] rhomat = new double[3];
            double[,] lmati = new double[3, 3];
            double[,] rsmat = new double[3, 3];
            double[,] lir = new double[3, 3];
            double tau12, tau32, tau13, a1, a1u, a3, a3u, d, d1, d2, l2dotrs;
            double deriv, derivn1, derivn2, bigr2c;

            double tu = Math.Sqrt(gravConst.re * gravConst.re * gravConst.re / gravConst.mu);

            // ---- set middle to 0, deltas to other times ---- 
            // need to separate into jd and jdf portions for accuracy since it's often only seconds or minutes
            tau12 = (jd1 - jd2) * 86400.0 + (jdf1 - jdf2) * 86400.0 / tu; // days to sec
            tau13 = (jd1 - jd3) * 86400.0 + (jdf1 - jdf3) * 86400.0 / tu;
            tau32 = (jd3 - jd2) * 86400.0 + (jdf3 - jdf2) * 86400.0 / tu;

            // ----------------  find line of sight vectors  ---------------- 
            los1[0] = Math.Cos(tdecl1) * Math.Cos(trtasc1);
            los1[1] = Math.Cos(tdecl1) * Math.Sin(trtasc1);
            los1[2] = Math.Sin(tdecl1);

            los2[0] = Math.Cos(tdecl2) * Math.Cos(trtasc2);
            los2[1] = Math.Cos(tdecl2) * Math.Sin(trtasc2);
            los2[2] = Math.Sin(tdecl2);

            los3[0] = Math.Cos(tdecl3) * Math.Cos(trtasc3);
            los3[1] = Math.Cos(tdecl3) * Math.Sin(trtasc3);
            los3[2] = Math.Sin(tdecl3);

            // -------------- find l matrix and determinant ----------------- 
            // -------- called lmat since it is only used for determ -------
            for (int i = 0; i < 3; i++)
            {
                lmat[i, 0] = los1[i];
                lmat[i, 1] = los2[i];
                lmat[i, 2] = los3[i];
                rsmat[i, 0] = rseci1[i] / gravConst.re;  // er
                rsmat[i, 1] = rseci2[i] / gravConst.re;
                rsmat[i, 2] = rseci3[i] / gravConst.re;
            }

            d = MathTimeLibr.determinant(lmat, 3);

            // ------------ now assign the inverse -------------- 
            lmati[0, 0] = (los2[1] * los3[2] - los2[2] * los3[1]) / d;
            lmati[1, 0] = (-los1[1] * los3[2] + los1[2] * los3[1]) / d;
            lmati[2, 0] = (los1[1] * los2[2] - los1[2] * los2[1]) / d;

            lmati[0, 1] = (-los2[0] * los3[2] + los2[2] * los3[0]) / d;
            lmati[1, 1] = (los1[0] * los3[2] - los1[2] * los3[0]) / d;
            lmati[2, 1] = (-los1[0] * los2[2] + los1[2] * los2[0]) / d;

            lmati[0, 2] = (los2[0] * los3[1] - los2[1] * los3[0]) / d;
            lmati[1, 2] = (-los1[0] * los3[1] + los1[1] * los3[0]) / d;
            lmati[2, 2] = (los1[0] * los2[1] - los1[1] * los2[0]) / d;

            lir = MathTimeLibr.matmult(lmati, rsmat, 3, 3, 3);

            // ------------- find f and g series at 1st and 3rd obs --------- 
            // speed by assuming circ sat vel for udot here ??                
            // some similarities in 1/6t3t1 ...                              
            // ---- keep separated this time ----                             
            a1 = tau32 / (tau32 - tau12);
            a1u = (tau32 * ((tau32 - tau12) * (tau32 - tau12) - tau32 * tau32)) /
                (6.0 * (tau32 - tau12));
            a3 = -tau12 / (tau32 - tau12);
            a3u = -(tau12 * ((tau32 - tau12) * (tau32 - tau12) - tau12 * tau12)) /
                (6.0 * (tau32 - tau12));

            // ---- form initial guess of r2 ---- 
            d1 = lir[1, 0] * a1 - lir[1, 1] + lir[1, 2] * a3;
            d2 = lir[1, 0] * a1u + lir[1, 2] * a3u;

            // -------- solve eighth order poly not same as laplace --------- 
            l2dotrs = MathTimeLibr.dot(los2, rseci2) / gravConst.re;
            poly[1] = 1.0;  // r2^8
            poly[2] = 0.0;
            poly[3] = -(d1 * d1 + 2.0 * d1 * l2dotrs + MathTimeLibr.mag(rseci2) / gravConst.re * MathTimeLibr.mag(rseci2) / gravConst.re);
            poly[4] = 0.0;
            poly[5] = 0.0;
            poly[6] = -2.0 * d2 * (l2dotrs + d1);  // * gravConst.mu
            poly[7] = 0.0;
            poly[8] = 0.0;
            poly[9] = -d2 * d2;  // gravConst.mu * gravConst.mu *

            // simply iterate to find the correct root
            bigr2 = 100.0;
            // can do Newton iteration Curtis p 289 simple derivative of the poly
            // makes sense since the values are so huge in the polynomial
            // do as Halley iteration since derivatives are possible. tests at LEO, GPS, GEO,
            // all seem to converge to the proper answer
            int kk = 0;
            bigr2c = 20000.0 / gravConst.re; // er guess ~GPS altitude
            while (Math.Abs(bigr2 - bigr2c) > 0.5 && kk < 15)
            {
                bigr2 = bigr2c;
                deriv = Math.Pow(bigr2, 8) + poly[3] * Math.Pow(bigr2, 6)
                    + poly[6] * Math.Pow(bigr2, 3) + poly[9];
                derivn1 = 8.0 * Math.Pow(bigr2, 7) + 6.0 * poly[3] * Math.Pow(bigr2, 5)
                    + 3.0 * poly[6] * Math.Pow(bigr2, 2);
                derivn2 = 56.0 * Math.Pow(bigr2, 6) + 30.0 * poly[3] * Math.Pow(bigr2, 4)
                    + 6.0 * poly[6] * bigr2;
                // Newton iteration
                //bigr2n = bigr2 - deriv / derivn1;
                // Halley iteration
                bigr2c = bigr2 - (2.0 * deriv * derivn1) / (2.0 * derivn1 * derivn1 - deriv * derivn2);
                kk = kk + 1;
            }
            bigr2 = bigr2c * gravConst.re;
        }  // getGaussRoot

        /*------------------------------------------------------------------------------
        *
        *                           procedure anglesgauss
        *
        *  this procedure solves the problem of orbit determination using three
        *    optical sightings. the solution procedure uses the gaussian technique.
        *    the 8th order root is generally the big point of discussion. A Halley iteration
        *    permits a quick solution to find the correct root, with a starting guess of 20000 km. 
        *    the general formulation yields polynomial coefficients that are very large, and can easily 
        *    become overflow operations. Thus, canonical units are used only until the root is found, 
        *    then regular units are resumed. 
        *
        *  author        : david vallado           davallado@gmail.com   22 jun 2002
        *
        *  inputs          description                              range / units
        *    tdecl1       - declination #1                               rad
        *    tdecl2       - declination #2                               rad
        *    tdecl3       - declination #3                               rad
        *    trtasc1      - right ascension #1                           rad
        *    trtasc2      - right ascension #2                           rad
        *    trtasc3      - right ascension #3                           rad
        *    jd1, jdf1    - julian date of 1st sighting                  days from 4713 bc
        *    jd2, jdf2    - julian date of 2nd sighting                  days from 4713 bc
        *    jd3, jdf3    - julian date of 3rd sighting                  days from 4713 bc
        *    rs1          - eci site position vector #1                  km
        *    rs2          - eci site position vector #2                  km
        *    rs3          - eci site position vector #3                  km
        *
        *  outputs        :
        *    r2           -  position vector at t2                       km
        *    v2           -  velocity vector at t2                       km / s
        *    errstr       - output results for debugging
        *
        *  locals         :
        *    los1         - line of sight vector for 1st
        *    los2         - line of sight vector for 2nd
        *    los3         - line of sight vector for 3rd
        *    tau          - taylor expansion series about
        *                   tau ( t - to )
        *    tausqr       - tau squared
        *    t21t23       - (t2-t1) * (t2-t3)
        *    t31t32       - (t3-t1) * (t3-t2)
        *    i            - index
        *    d            -
        *    rho          - range from site to sat at t2                 km
        *    rhodot       -
        *    dmat         -
        *    earthrate    - velocity of earth rotation
        *    p            -
        *    q            -
        *    oldr         -
        *    oldv         -
        *    f1           - f coefficient
        *    g1           -
        *    f3           -
        *    g3           -
        *    l2dotrs      -
        *
        *  coupling       :
        *    mag          - magnitude of a vector
        *    determinant  - evaluate the determinant of a matrix
        *    factor       - find roots of a polynomial
        *    matmult      - multiply two matrices together
        *    gibbs        - gibbs method of orbit determination
        *    hgibbs       - herrick gibbs method of orbit determination
        *    angle        - angle between two vectors
        *
        *  references     :
        *    vallado       2013, 442, alg 52, ex 7-2
        -----------------------------------------------------------------------------*/

        public void anglesgauss
        (
            double tdecl1, double tdecl2, double tdecl3,
            double trtasc1, double trtasc2, double trtasc3,
            double jd1, double jdf1, double jd2, double jdf2, double jd3, double jdf3,
            double[] rseci1, double[] rseci2, double[] rseci3, out double[] r2, out double[] v2,
            out string errstr
        )
        {
            const double rad = 180.0 / Math.PI;
            Int32 i, ll;
            string error;  //, chk3roots;
            double[] poly = new double[10];
            double[,] roots = new double[16, 3];
            double[] r1 = new double[3];
            double[] r3 = new double[3];
            double[] rs2c = new double[3];
            double[] los1 = new double[3];
            double[] los2 = new double[3];
            double[] los3 = new double[3];
            double[,] lmat = new double[3, 3];
            double[] cmat = new double[3];
            double[] rhomat = new double[3];
            double[,] lmati = new double[3, 3];
            double[,] rsmat = new double[3, 3];
            double[,] rsmatc = new double[3, 3];
            double[,] lir = new double[3, 3];
            r2 = new double[] { 0.0, 0.0, 0.0 };
            v2 = new double[] { 0.0, 0.0, 0.0 };
            double f1old, g1old, f3old, g3old;
            errstr = "";
            double deriv, deriv1, deriv2, bigr2c;
            double rdot, tau12, tau32, tau13, u, udot, p, f1, g1, f3, g3, a, ecc, incl, raan, argp,
            nu, m, l, argper, bigr2, a1, a1u, a3, a3u, d, c1, c2, c3, l2dotrs, magr2,
            rhonew1, rhonew2, rhonew3, theta, theta1, copa, tausqr;
            double a1c, a1uc, a3c, a3uc, d1c, d2c, tau12c, tau32c, tau13c;

            // canonical only through forming and solving the 8th order polynomial
            double tu = Math.Sqrt(gravConst.re * gravConst.re * gravConst.re / gravConst.mu);

            // ----------------------   initialize   ------------------------ 
            char show;
            show = 'a';  // show all
            show = 'y';  // show just root info

            a = 0.0;
            f1old = 0.0;
            g1old = 0.0;
            f3old = 0.0;
            g3old = 0.0;

            // ---- set middle to 0, deltas to other times ---- 
            // need to separate into jd and jdf portions for accuracy since it's often only seconds or minutes
            tau12 = (jd1 - jd2) * 86400.0 + (jdf1 - jdf2) * 86400.0; // days to sec
            tau13 = (jd1 - jd3) * 86400.0 + (jdf1 - jdf3) * 86400.0;
            tau32 = (jd3 - jd2) * 86400.0 + (jdf3 - jdf2) * 86400.0;

            // switch to canonical????????????????????????????????????????????
            tau12c = tau12 / tu;
            tau13c = tau13 / tu;
            tau32c = tau32 / tu;

            if (show == 'a')
            {
                errstr = errstr + " tau12 " + tau12.ToString() + " " + " tau32 " + tau32.ToString()
                    + " tau13 " + tau13.ToString() + " s \n";
                errstr = errstr + " tau12 " + tau12c.ToString() + " " + " tau32 " + tau32c.ToString()
                    + " tau13 " + tau13c.ToString() + " TU \n";
            }

            // ----------------  find line of sight vectors  ---------------- 
            los1[0] = Math.Cos(tdecl1) * Math.Cos(trtasc1);
            los1[1] = Math.Cos(tdecl1) * Math.Sin(trtasc1);
            los1[2] = Math.Sin(tdecl1);

            los2[0] = Math.Cos(tdecl2) * Math.Cos(trtasc2);
            los2[1] = Math.Cos(tdecl2) * Math.Sin(trtasc2);
            los2[2] = Math.Sin(tdecl2);

            los3[0] = Math.Cos(tdecl3) * Math.Cos(trtasc3);
            los3[1] = Math.Cos(tdecl3) * Math.Sin(trtasc3);
            los3[2] = Math.Sin(tdecl3);

            // -------------- find l matrix and determinant ----------------- 
            // -------- called lmat since it is only used for determ -------
            for (i = 0; i < 3; i++)
            {
                lmat[i, 0] = los1[i];
                lmat[i, 1] = los2[i];
                lmat[i, 2] = los3[i];
                // switch to canonical
                rsmat[i, 0] = rseci1[i];  // km
                rsmat[i, 1] = rseci2[i];
                rsmat[i, 2] = rseci3[i];

                rsmatc[i, 0] = rseci1[i] / gravConst.re;  // er
                rsmatc[i, 1] = rseci2[i] / gravConst.re;
                rsmatc[i, 2] = rseci3[i] / gravConst.re;
                rs2c[i] = rseci2[i] / gravConst.re;  // er for later
            }

            d = MathTimeLibr.determinant(lmat, 3);

            // ------------ now assign the inverse -------------- 
            lmati[0, 0] = (los2[1] * los3[2] - los2[2] * los3[1]) / d;
            lmati[1, 0] = (-los1[1] * los3[2] + los1[2] * los3[1]) / d;
            lmati[2, 0] = (los1[1] * los2[2] - los1[2] * los2[1]) / d;

            lmati[0, 1] = (-los2[0] * los3[2] + los2[2] * los3[0]) / d;
            lmati[1, 1] = (los1[0] * los3[2] - los1[2] * los3[0]) / d;
            lmati[2, 1] = (-los1[0] * los2[2] + los1[2] * los2[0]) / d;

            lmati[0, 2] = (los2[0] * los3[1] - los2[1] * los3[0]) / d;
            lmati[1, 2] = (-los1[0] * los3[1] + los1[1] * los3[0]) / d;
            lmati[2, 2] = (los1[0] * los2[1] - los1[1] * los2[0]) / d;

            //errstr = errstr + " lmati " 
            //       + lmati[0, 0].ToString() + " " + lmati[1, 0].ToString() + " " + lmati[2, 0].ToString() + " \n"
            //       + lmati[0, 1].ToString() + " " + lmati[1, 1].ToString() + " " + lmati[2, 1].ToString() + " \n"
            //       + lmati[0, 2].ToString() + " " + lmati[1, 2].ToString() + " " + lmati[2, 2].ToString() + " \n";
            if (show == 'a')
                errstr = errstr + " rsmatc "
                   + rsmatc[0, 0].ToString() + " " + rsmatc[1, 0].ToString() + " " + rsmatc[2, 0].ToString() + " \n"
                   + rsmatc[0, 1].ToString() + " " + rsmatc[1, 1].ToString() + " " + rsmatc[2, 1].ToString() + " \n"
                   + rsmatc[0, 2].ToString() + " " + rsmatc[1, 2].ToString() + " " + rsmatc[2, 2].ToString() + " \n";

            lir = MathTimeLibr.matmult(lmati, rsmatc, 3, 3, 3);
            if (show == 'a')
                errstr = errstr + " lir "
                   + lir[0, 0].ToString() + " " + lir[1, 0].ToString() + " " + lir[2, 0].ToString() + " \n"
                   + lir[0, 1].ToString() + " " + lir[1, 1].ToString() + " " + lir[2, 1].ToString() + " \n"
                   + lir[0, 2].ToString() + " " + lir[1, 2].ToString() + " " + lir[2, 2].ToString() + " \n";

            // ------------- find f and g series at 1st and 3rd obs --------- 
            // speed by assuming circ sat vel for udot here ??                
            // some similarities in 1/6t3t1 ...                              
            // ---- keep separated this time ----                             
            a1c = tau32c / (tau32c - tau12c);
            a1uc = (tau32c * ((tau32c - tau12c) * (tau32c - tau12c) - tau32c * tau32c)) /
                (6.0 * (tau32c - tau12c));
            a3c = -tau12c / (tau32c - tau12c);
            a3uc = -(tau12c * ((tau32c - tau12c) * (tau32c - tau12c) - tau12c * tau12c)) /
                (6.0 * (tau32c - tau12c));

            // ---- form initial guess of r2 ---- 
            d1c = lir[1, 0] * a1c - lir[1, 1] + lir[1, 2] * a3c;
            d2c = lir[1, 0] * a1uc + lir[1, 2] * a3uc;

            if (show == 'a')
                errstr = errstr + " a1 " + a1c.ToString() + " " + " a1u " + a1uc.ToString() + " "
                    + " a3 " + a3c.ToString() + " " + " a3u " + a3uc.ToString() + " "
                    + " d1 " + d1c.ToString() + " " + " d2 " + d2c.ToString() + " canonical \n";

            // -------- solve eighth order poly not same as laplace --------- 
            // switch to canonical to prevent overflows in the poly
            l2dotrs = MathTimeLibr.dot(los2, rs2c);
            if (show == 'a')
                errstr = errstr + " ldotrs " + l2dotrs.ToString() + "\n";
            poly[1] = 1.0;  // r2^8
            poly[2] = 0.0;
            poly[3] = -(d1c * d1c + 2.0 * d1c * l2dotrs + MathTimeLibr.mag(rs2c) * MathTimeLibr.mag(rs2c));
            poly[4] = 0.0;
            poly[5] = 0.0;
            poly[6] = -2.0 * d2c * (l2dotrs + d1c);
            poly[7] = 0.0;
            poly[8] = 0.0;
            poly[9] = -d2c * d2c;  // no mu^2, r2^0

            errstr = errstr + "Gauss poly " + poly[3] + " " + poly[6] + " " + poly[9] + "\n";
            if (poly[3] < 0.0 && poly[6] > 0.0)
                errstr = errstr + "GAUSS may have multiple roots 1.0  0.0  " + poly[3] + "  0.0 0.0 " + poly[6] + " 0.0 0.0 " + poly[9] + "\n";

            // note that there will usually be only 1 real positive root (See Der AAS 19-626)
            // only in cases where poly[3] is negative and poly[6] is positive could there be multiple
            // positive real roots. 
            //MathTimeLibr.factor(poly, 8, out roots);

            // simply iterate to find the correct root
            bigr2 = 100.0;
            // can do Newton iteration Curtis p 289 simple derivative of the poly
            // makes sense since the values are so huge in the polynomial
            // do as Halley iteration since derivatives are possible. tests at LEO, GPS, GEO,
            // all seem to converge to the proper answer
            int kk = 1;
            double bigr2x; //, bigr2nx;
            bigr2c = 20000.0 / gravConst.re; // er guess ~GPS altitude
           // bigr2nx = bigr2c;
            while (Math.Abs(bigr2 - bigr2c) > 8.0e-5 && kk < 15)  // do in er, 0.5 km
            {
                bigr2 = bigr2c;
            //    bigr2x = bigr2nx;
                deriv = Math.Pow(bigr2, 8) + poly[3] * Math.Pow(bigr2, 6)
                    + poly[6] * Math.Pow(bigr2, 3) + poly[9];
                deriv1 = 8.0 * Math.Pow(bigr2, 7) + 6.0 * poly[3] * Math.Pow(bigr2, 5)
                    + 3.0 * poly[6] * Math.Pow(bigr2, 2);
                deriv2 = 56.0 * Math.Pow(bigr2, 6) + 30.0 * poly[3] * Math.Pow(bigr2, 4)
                    + 6.0 * poly[6] * bigr2;
                // just use Halley
                //bigr2n = bigr2 - deriv * n2 / (deriv1 * (n2 - deriv * deriv2 * 0.5));
                // Halley iteration
                bigr2c = bigr2 - (2.0 * deriv * deriv1) / (2.0 * deriv1 * deriv1 - deriv * deriv2);
            }

            // ---- find correct (positive real) root ----
            //bigr2c = 0.0;
            // ------------ check for case of multiple roots
            //if (poly[3] < 0.0 && poly[6] > 0.0)
            //{
            // der - compare to Laplace roots
            // if laplace has 1 root, pick gauss root closest to laplace
            // if laplace has multiple roots, another insight is needed.
            //}
            if (bigr2c < 0.0 || bigr2c * gravConst.re > 75000.0)
                bigr2c = 40000.0 / gravConst.re;  // simply set this to about GEO, allowing for less than that too. 

            // now back to normal units
            bigr2 = bigr2c * gravConst.re;  // km
        //    bigr2nx = bigr2nx * gravConst.re;

            a1 = tau32 / (tau32 - tau12);
            a1u = (tau32 * ((tau32 - tau12) * (tau32 - tau12) - tau32 * tau32)) /
                (6.0 * (tau32 - tau12));
            a3 = -tau12 / (tau32 - tau12);
            a3u = -(tau12 * ((tau32 - tau12) * (tau32 - tau12) - tau12 * tau12)) /
                (6.0 * (tau32 - tau12));
            lir = MathTimeLibr.matmult(lmati, rsmat, 3, 3, 3);


            if (show == 'y' || show == 'a')
            {
        //        errstr = errstr + "Poly--------- " + poly[1] + " " + poly[3] + " " + poly[6] + " " + poly[9]
        //        + " tau " + tau12.ToString() + " " + tau32.ToString() + " sec ";

                //if (poly[3] < 0.0 && poly[6] > 0.0)
                //    chk3roots = "3 root poss ";
                //else
                //    chk3roots = "1 root poss ";
       //         errstr = errstr + chk3roots + " root pick " + bigr2.ToString(); // + "\n";  // + " " + bigr2nx.ToString()
            }

            // ------------- solve matrix with u2 better known -------------- 
            u = gravConst.mu / (bigr2 * bigr2 * bigr2);

            c1 = a1 + a1u * u;
            c2 = -1.0;
            c3 = a3 + a3u * u;

            cmat[0] = -c1;
            cmat[1] = -c2;
            cmat[2] = -c3;
            rhomat = MathTimeLibr.matvecmult(lir, cmat, 3);

            rhonew1 = rhomat[0] / c1;
            rhonew2 = rhomat[1] / c2;
            rhonew3 = rhomat[2] / c3;

            if (show == 'a')
                errstr = errstr + " rhonew start " + rhonew1.ToString() + " " + rhonew2.ToString() + " " + rhonew3.ToString() + "\n";

            // ---- now form the three position vectors ----- 
            for (i = 0; i < 3; i++)
            {
                r1[i] = rhonew1 * los1[i] + rseci1[i];
                r2[i] = rhonew2 * los2[i] + rseci2[i];
                r3[i] = rhonew3 * los3[i] + rseci3[i];
            }

            if (show == 'a')
                errstr = errstr + " r2 " + r2[0].ToString() + " " + r2[1].ToString() + " "
                    + r2[2].ToString() + "\n";

            // now find the middle velocity vector with gibbs or hgibbs from end of formal Gauss
            gibbs(r1, r2, r3, out v2, out theta, out theta1, out copa, out error);
            if (show == 'a')
            {
                errstr = errstr + "v2g " + v2[0].ToString() + " " + v2[1].ToString() + " "
                    + v2[2].ToString() + "\n";
                errstr = errstr + "gibbs " + error + " theta " + (theta * rad).ToString() + " " + (theta1 * rad).ToString()
                    + " " + (copa * rad).ToString() + "\n";
            }

            if (!error.Equals("ok") && (Math.Abs(theta) < 5.0 / rad || Math.Abs(theta1) < 5.0 / rad))
            {
                // hgibbs to get middle vector ---- 
                herrgibbs(r1, r2, r3, jd1 + jdf1, jd2 + jdf2, jd3 + jdf3, out v2, out theta, out theta1, out copa, out error);
                if (show == 'a')
                {
                    errstr = errstr + "v2h " + v2[0].ToString() + " " + v2[1].ToString() + " "
                        + v2[2].ToString() + "\n";
                    errstr = errstr + "hgibbs " + error + " theta " + (theta * rad).ToString() + " " + (theta1 * rad).ToString()
                        + " " + (copa * rad).ToString(); // + "\n";
                }
            }

            rv2coe(r2, v2, out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out u, out l, out argper);
            if (show == 'a')
                errstr = errstr + "p  " + p.ToString() + " a " + a.ToString() + " e " + ecc.ToString() + "\n";

            // escobal says to stop if closely spaced...gtds does lots of processing
            // perhaps if under 40 deg (theta/theta1) would work?
            // refining can take many forms
            // using Gibbs (HGibbs) is nice because it yields position and velocity
            // seems to work best most often "without" any improvement (note that 1 iteration
            // is performed in the root selection as each possible root is tested) 
            // this is not really a newton iteration 
            // perhaps the corrections need to be limited?????

            // ------------- loop through the refining process --------------
            double rho2 = rhonew2 + 100.0;
            string errstr1 = " loops ";
            ll = 0;
            // disabled now...
            while (Math.Abs(rhonew2 - rho2) > 0.1 && ll <= -1)  //; ll < 3; ll++)  -1
            {
                ll = ll + 1;
                errstr = errstr + "loop " + ll.ToString() + " " + (rhonew2 - rho2).ToString() + "\n";
                errstr1 = errstr1 + " " + (rhonew2 - rho2).ToString("0.##");
                // keep track of the convergence
                rho2 = rhonew2;

                // now find the middle velocity vector with gibbs or hgibbs
                gibbs(r1, r2, r3, out v2, out theta, out theta1, out copa, out error);
                if (!error.Equals("ok"))
                    errstr = errstr + "gibbs " + error + " theta " + (theta * rad).ToString() + " " + (theta1 * rad).ToString()
                        + " " + (copa * rad).ToString() + "\n";

                if (!error.Equals("ok") && (Math.Abs(theta) < 5.0 / rad || Math.Abs(theta1) < 5.0 / rad))
                {
                    // hgibbs to get middle vector ---- 
                    herrgibbs(r1, r2, r3, jd1 + jdf1, jd2 + jdf2, jd3 + jdf3, out v2, out theta, out theta1, out copa, out error);
                    if (!error.Equals("ok"))
                        errstr = errstr + "hgibbs " + error + " theta " + (theta * rad).ToString() + " " + (theta1 * rad).ToString()
                            + " " + (copa * rad).ToString() + "\n";
                }
                if (show == 'y')
                    errstr = errstr + "theta " + (theta * rad).ToString() + " " + (theta1 * rad).ToString()
                    + " " + (copa * rad).ToString() + "\n";

                //test output only
                rv2coe(r2, v2, out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out u, out l, out argper);
                errstr = errstr + "p  " + p.ToString() + " a " + a.ToString() + " e " + ecc.ToString() + "\n";
                magr2 = MathTimeLibr.mag(r2);

                // universal variable approach
                //double alpha;
                //double dtsec, rdotv, magr, c2new, c3new, xnew, znew;
                //int ktr;
                //alpha = 1.0 / a;
                //iterateuniversalX(alpha, tau12, MathTimeLibr.dot(r2, v2), magr2, r2, v2,
                //    out ktr, out c2new, out c3new, out xnew, out znew);

                //iterateuniversalX(alpha, tau32, MathTimeLibr.dot(r2, v2), magr2, r2, v2,
                //    out ktr, out c2new, out c3new, out xnew, out znew);

                //double xnewsqrd = xnew * xnew;
                //f1 = 1.0 - (xnewsqrd * c2new / magr2);
                //g1 = tau12 - xnewsqrd * xnew * c3new / Math.Sqrt(gravConst.mu);
                //f3 = 1.0 - (xnewsqrd * c2new / magr2);
                //g3 = tau32 - xnewsqrd * xnew * c3new / Math.Sqrt(gravConst.mu);

                // ---- now get an improved estimate of the f and g series ---- 
                // this is the intermediate form from Escobal pg 108 using only 1st derivative  
                u = gravConst.mu / (magr2 * magr2 * magr2);
                rdot = MathTimeLibr.dot(r2, v2) / magr2;
                udot = (-3.0 * gravConst.mu * rdot) / (Math.Pow(magr2, 4));

                tausqr = tau12 * tau12;
                f1 = 1.0 - 0.5 * u * tausqr - (1.0 / 6.0) * udot * tausqr * tau12;
                g1 = tau12 - (1.0 / 6.0) * u * tau12 * tausqr -
                                    (1.0 / 12.0) * udot * tausqr * tausqr;
                tausqr = tau32 * tau32;
                f3 = 1.0 - 0.5 * u * tausqr - (1.0 / 6.0) * udot * tausqr * tau32;
                g3 = tau32 - (1.0 / 6.0) * u * tau32 * tausqr -
                                    (1.0 / 12.0) * udot * tausqr * tausqr;
                if (show == 'y')
                    errstr = errstr + "f1 " + f1.ToString() + " g1 " + g1.ToString() + " f3 " + f3.ToString() + " g3 " + g3.ToString() + "\n";
                if (Math.Abs(g1old) > 0.000001)
                {
                    f1 = (f1 + f1old) * 0.5;
                    g1 = (g1 + g1old) * 0.5;
                    f3 = (f3 + f3old) * 0.5;
                    g3 = (g3 + g3old) * 0.5;
                }
                c1 = g3 / (f1 * g3 - f3 * g1);
                c3 = -g1 / (f1 * g3 - f3 * g1);

                f1old = f1;
                g1old = g1;
                f3old = f3;
                g3old = g3;

                errstr = errstr + " f1 " + f1.ToString() + " g1 " + g1.ToString() + " f3 "
                + f3.ToString() + " g3 " + g3.ToString() + " c1 "
                + c1.ToString() + " c3 " + c3.ToString() + "\n";

                // ---- solve for all three ranges via matrix equation ----
                cmat[0] = -c1;
                cmat[1] = -c2;
                cmat[2] = -c3;
                rhomat = MathTimeLibr.matvecmult(lir, cmat, 3);

                // store these temporarily
                double rold1, rold2, rold3;
                rold1 = rhonew1;
                rold2 = rhonew2;
                rold3 = rhonew3;

                rhonew1 = rhomat[0] / c1;
                rhonew2 = rhomat[1] / c2;
                rhonew3 = rhomat[2] / c3;
                if (show == 'y')
                    errstr = errstr + ll.ToString() + " rhoold end " + rhonew1.ToString() + " "
                    + rhonew2.ToString() + " " + rhonew3.ToString() + "\n";

                // get ready for next loop
                // ---- find all three vectors ri ---- 
                for (i = 0; i < 3; i++)
                {
                    r1[i] = rhonew1 * los1[i] + rseci1[i];
                    r2[i] = rhonew2 * los2[i] + rseci2[i];
                    r3[i] = rhonew3 * los3[i] + rseci3[i];
                    if (show == 'y')
                        errstr = errstr + "rmat " + r1[i].ToString() + " " + r2[i].ToString() + " "
                        + r3[i].ToString() + "\n";
                }

                if (show == 'y')
                    errstr = errstr + "end loop " + ll.ToString() + " " + rold1.ToString() + " " + rold2.ToString() + " "
                        + rold3.ToString() + "\n\n";
            }  // end while loop


            // now find the middle velocity vector with gibbs or hgibbs from last time through
            gibbs(r1, r2, r3, out v2, out theta, out theta1, out copa, out error);
            if (show == 'a')
            {
                errstr = errstr + "v2g " + v2[0].ToString() + " " + v2[1].ToString() + " "
                    + v2[2].ToString() + "\n";
                errstr = errstr + "gibbs " + error + " theta " + (theta * rad).ToString() + " " + (theta1 * rad).ToString()
                    + " " + (copa * rad).ToString() + "\n";
            }

            if (!error.Equals("ok") && (Math.Abs(theta) < 5.0 / rad || Math.Abs(theta1) < 5.0 / rad))
            {
                // hgibbs to get middle vector ---- 
                herrgibbs(r1, r2, r3, jd1 + jdf1, jd2 + jdf2, jd3 + jdf3, out v2, out theta, out theta1, out copa, out error);
                if (show == 'a')
                {
                    errstr = errstr + "v2h " + v2[0].ToString() + " " + v2[1].ToString() + " "
                        + v2[2].ToString() + "\n";
                    errstr = errstr + "hgibbs " + error + " theta " + (theta * rad).ToString() + " " + (theta1 * rad).ToString()
                        + " " + (copa * rad).ToString() + "\n";
                }
            }

            rv2coe(r2, v2, out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out u, out l, out argper);
            if (show == 'a')
                errstr = errstr + "end p  " + p.ToString() + " a " + a.ToString() + " e " + ecc.ToString() + errstr1 + "\n";
        }    // anglesgauss


        /*------------------------------------------------------------------------------
        *
        *                           procedure doubler
        *
        *  this rountine accomplishes the iteration evaluation of one step for the double-r 
        *  angles only routine.
        *  
        *  author        : david vallado           davallado@gmail.com   23 dec 2003
        *
        *  inputs          description                                 range / units
        *    cc1          - 
        *    cc2          - 
        *    magrsite1    - 
        *    magrsite2    - 
        *    magr1in      - initial estimate 
        *    magr2in      - initial estimate
        *    los1         - line of sight vector for 1st
        *    los2         - line of sight vector for 2nd
        *    los3         - line of sight vector for 3rd
        *    rsite1       - eci site position vector #1                  km
        *    rsite2       - eci site position vector #2                  km
        *    rsite3       - eci site position vector #3                  km
        *    t1           -                                              sec
        *    t3           -                                              sec
        *    dm           - direction of motion                          'S', 'L' 
        *    n12          - # of days between tracks 1,2                 days
        *    n13          - # of days between tracks 1,3                 days
        *    n23          - # of days between tracks 2,3                 days
        *
        *  outputs        :
        *    r2           - position vector at t2                        km
        *    r3           - position vector at t3                        km
        *    f1           -
        *    f2           -
        *    q1           - quality estimate
        *    magr1        - magnitude of r1 vector                       km
        *    magr2        - magnitude of r2 vector                       km
        *    a            - semi major axis                              km / s
        *    deltae32     - eccentric anomaly difference 3-2            
        *    errstr       - output results for debugging
        *    
        *  locals         :
        *                 -
        *
        *  coupling       :
        *    dot, cross, mag
        *
        *  references     :
        *    vallado       2013
        -----------------------------------------------------------------------------*/

        void doubler
        (
            double cc1, double cc2, double magrsite1, double magrsite2, double magr1in, double magr2in,
            double[] los1, double[] los2, double[] los3,
            double[] rsite1, double[] rsite2, double[] rsite3, double t1, double t3, char dm,
            int n12, int n13, int n23,
            out double[] r2, out double[] r3, out double f1, out double f2, out double q1,
            out double magr2, out double a, out double deltae32, out string errstr
        )
        {
            double rho1, rho2, rho3, e, n, p, eSinE, eCosE, magr3, dv21, dv31, dv32, c1, c3, esinv2, ecosv1, ecosv2, ecosv3,
                sinde32, cosde32, sinde21, cosde21, deltae21, deltam12, sindh32, sindh21,
                deltah32, deltah21, deltam32, sindv32, cosdv32, sindv31, cosdv31, sindv21, cosdv21, twopi, magr1;
            double[] r1 = new double[3];
            double[] w = new double[3];
            double[] temp = new double[3];
            char show = 'y';

            twopi = 2.0 * Math.PI;
            r2 = new double[] { 0.0, 0.0, 0.0 };
            r3 = new double[] { 0.0, 0.0, 0.0 };
            errstr = "";

            // for interplanetary
            // re = 149597870.0;
            // mu = 1.32712428e11;

            // be sure the sqrt isn't negative
            double tempsq = cc1 * cc1 - 4.0 * (magrsite1 * magrsite1 - magr1in * magr1in);
            if (tempsq < 0.0)
                tempsq = 300.0;  // default range, use because hyperbolic likely at shorter times, lower alt
            rho1 = (-cc1 + Math.Sqrt(tempsq)) * 0.5;
            tempsq = cc2 * cc2 - 4.0 * (magrsite2 * magrsite2 - magr2in * magr2in);
            if (tempsq < 0.0)
                tempsq = 300.0;  // a default range, not 0.0
            rho2 = (-cc2 + Math.Sqrt(tempsq)) * 0.5;
            for (int i = 0; i < 3; i++)
            {
                r1[i] = rho1 * los1[i] + rsite1[i];
                r2[i] = rho2 * los2[i] + rsite2[i];
            }
            if (show == 'y')
            {
                errstr = errstr + "\nstart of loop doubler test " + magr1in.ToString() + " " + magr2in.ToString() + "\n";
                errstr = errstr + "r1 " + r1[0].ToString() + " " + r1[1].ToString() + " " + r1[2].ToString() + "\n";
                errstr = errstr + "r2 " + r2[0].ToString() + " " + r2[1].ToString() + " " + r2[2].ToString() + "\n";
            }

            magr1 = MathTimeLibr.mag(r1);
            magr2 = MathTimeLibr.mag(r2);

            // normal vector to plane of 1 and 2
            MathTimeLibr.cross(r1, r2, out temp);
            double tem = 1.0 / (magr1 * magr2);
            for (int i = 0; i < 3; i++)
            {
                if (dm == 'S')  // short way will be direct motion?
                    w[i] = temp[i] * tem;
                else
                    w[i] = -temp[i] * tem;  // retrograde motion
            }

            // change to negative sign, from gtds
            rho3 = -MathTimeLibr.dot(rsite3, w) / MathTimeLibr.dot(los3, w);

            for (int i = 0; i < 3; i++)
                r3[i] = rho3 * los3[i] + rsite3[i];
            magr3 = MathTimeLibr.mag(r3);
          
            if (show == 'y')
            {
                errstr = errstr + "r3 " + r3[0].ToString() + " " + r3[1].ToString() + " " + r3[2].ToString() + "\n";
                errstr = errstr + "after 1st mag " + magr1 + " " + magr2 + " " + magr3 + "\n";
            }

            // note these are from the ctr of earth, not site
            cosdv21 = MathTimeLibr.dot(r2, r1) / (magr2 * magr1);
            MathTimeLibr.cross(r2, r1, out temp);
            if (dm == 'S')
                sindv21 = MathTimeLibr.mag(temp) / (magr2 * magr1);
            else
                sindv21 = -MathTimeLibr.mag(temp) / (magr2 * magr1);

            dv21 = Math.Atan2(sindv21, cosdv21) + twopi * n12;

            cosdv31 = MathTimeLibr.dot(r3, r1) / (magr3 * magr1);

            double[] tx = new double[3];
            MathTimeLibr.cross(r3, r1, out tx);
            sindv31 = MathTimeLibr.mag(tx) / (magr3 * magr1);
            
            // these both appear to be the same, but it "may" need to be 'L' or < 0.0 ...
            //if (dm == 'S')
            if (w[2] >= 0.0)
                sindv31 = Math.Sqrt(1.0 - cosdv31 * cosdv31);
            else
                sindv31 = -Math.Sqrt(1.0 - cosdv31 * cosdv31);
            dv31 = Math.Atan2(sindv31, cosdv31) + twopi * n13;

            cosdv32 = MathTimeLibr.dot(r3, r2) / (magr3 * magr2);
            MathTimeLibr.cross(r3, r2, out temp);
            if (dm == 'S')
                sindv32 = MathTimeLibr.mag(temp) / (magr3 * magr2);
            else
                sindv32 = -MathTimeLibr.mag(temp) / (magr3 * magr2);
            dv32 = Math.Atan2(sindv32, cosdv32) + twopi * n23;

            // note that for short meas arcs, the slant range estimates here are not reliable
            // might be better to use Gauss
            // only c1 is different with the various cases
            //if (dv31 <= Math.Abs(dv32))
            if (dv31 > Math.PI)
            {
                c1 = (magr2 * sindv32) / (magr1 * sindv31);
                c3 = (magr2 * sindv21) / (magr3 * sindv31);
                //p = (c1 * magr1 + c3 * magr3 - magr2) / (c1 + c3 - 1.0);
                p = (sindv21 - sindv31 + sindv32) / (-sindv31/magr2 + sindv21/magr3 + sindv32/magr1);
            }
            else
            {
                c1 = (magr1 * sindv31) / (magr2 * sindv32);
                c3 = (magr1 * sindv21) / (magr3 * sindv32);
                p = (c3 * magr3 - c1 * magr2 + magr1) / (-c1 + c3 + 1.0);
            }

            ecosv1 = p / magr1 - 1.0;
            ecosv2 = p / magr2 - 1.0;
            ecosv3 = p / magr3 - 1.0;

            if (Math.Abs(sindv21) != Math.PI)
                //if (Math.Abs(sindv21) > Math.Abs(sindv32))
                esinv2 = (-cosdv21 * ecosv2 + ecosv1) / sindv21;
            else
                esinv2 = (cosdv32 * ecosv2 - ecosv3) / sindv32;

            e = Math.Sqrt(ecosv2 * ecosv2 + esinv2 * esinv2);
            a = p / (1.0 - e * e);

            if (e * e < 0.99)
            {
                n = Math.Sqrt(gravConst.mu / (a * a * a));
                eSinE = magr2 / p * Math.Sqrt(1.0 - e * e) * esinv2;
                eCosE = magr2 / p * (e * e + ecosv2);

                sinde32 = magr3 / Math.Sqrt(a * p) * sindv32 - magr3 / p * (1.0 - cosdv32) * eSinE;
                cosde32 = 1.0 - magr2 * magr3 / (a * p) * (1 - cosdv32);
                deltae32 = Math.Atan2(sinde32, cosde32) + twopi * n23;

                sinde21 = magr1 / Math.Sqrt(a * p) * sindv21 + magr1 / p * (1.0 - cosdv21) * eSinE;
                cosde21 = 1.0 - magr2 * magr1 / (a * p) * (1.0 - cosdv21);
                deltae21 = Math.Atan2(sinde21, cosde21) + twopi * n12;

                deltam32 = deltae32 + 2.0 * eSinE * Math.Pow(Math.Sin(deltae32 * 0.5), 2)
                    - eCosE * Math.Sin(deltae32);
                deltam12 = -deltae21 + 2.0 * eSinE * Math.Pow(Math.Sin(deltae21 * 0.5), 2)
                    + eCosE * Math.Sin(deltae21);
            }
            else
            {
                errstr = errstr + "hyperbolic, e1 is greater than 0.99 " + e.ToString() + "\n";
                if (a > 0.0)
                {
                    a = -a;  // get right sign for hyperbolic orbit
                    p = -p;
                }
                n = Math.Sqrt(gravConst.mu / -(a * a * a));

                eSinE = magr2 / p * Math.Sqrt(e * e - 1.0) * esinv2;
                eCosE = magr2 / p * (e * e + ecosv2);

                sindh32 = magr3 / Math.Sqrt(-a * p) * sindv32 - magr3 / p * (1.0 - cosdv32) * eSinE;
                sindh21 = magr1 / Math.Sqrt(-a * p) * sindv21 + magr1 / p * (1.0 - cosdv21) * eSinE;

                deltah32 = Math.Log(sindh32 + Math.Sqrt(sindh32 * sindh32 + 1.0));
                deltah21 = Math.Log(sindh21 + Math.Sqrt(sindh21 * sindh21 + 1.0));

                deltam32 = -deltah32 + 2.0 * eSinE * Math.Pow(Math.Sinh(deltah32 * 0.5), 2)
                    + eCosE * Math.Sinh(deltah32);
                deltam12 = deltah21 + 2.0 * eSinE * Math.Pow(Math.Sinh(deltah21 * 0.5), 2)
                    - eCosE * Math.Sinh(deltah21);
                // what if ends on hyperbolic solution.
                // try to pass back deltae32?
                deltae32 = deltah32; // fix
            }

            f1 = t1 - deltam12 / n;
            f2 = t3 - deltam32 / n;

            // accuracy estimate
            q1 = Math.Sqrt(f1 * f1 + f2 * f2);

            double rad = 180.0 / Math.PI;
            if (show == 'y')
            {
                errstr = errstr + "in deg dnu21 " + (dv21 * rad).ToString() + " dnu31 "
                    + (dv31 * rad).ToString() + " dnu32 " + (dv32 * rad).ToString() + "\n";
                errstr = errstr + "dm32 " + deltam32.ToString() + " dm12 " + deltam12.ToString() + " "
                    + c1.ToString() + " " + c3.ToString() + " " + p.ToString() + " "
                    + a.ToString() + " " + e.ToString() + " " + eSinE.ToString() + " " + eCosE.ToString() + "\n";
                errstr = errstr + "c1 " + c1.ToString() + " c3 " + c3.ToString() + " p " + p.ToString() + "\n";
                errstr = errstr + "f1 " + f1.ToString() + " f2 " + f2.ToString() + " q1 " + q1.ToString() + " end of doubler" + "\n";
            }
        }  // doubler


        /*------------------------------------------------------------------------------
        *
        *                           procedure anglesdoubler
        *
        *  this procedure solves the problem of orbit determination using three
        *    optical sightings. the solution procedure uses the double-r technique.
        *    the important thing is the input of the initial guesses of the range which
        *    may be easiest from the solution of the gauss 8th order poly.
        *
        *  author        : david vallado           davallado@gmail.com   22 jun 2002
        *
        *  inputs          description                              range / units
        *    tdecl1       - declination #1                               rad
        *    tdecl2       - declination #2                               rad
        *    tdecl3       - declination #3                               rad
        *    trtasc1      - right ascension #1                           rad
        *    trtasc2      - right ascension #2                           rad
        *    trtasc3      - right ascension #3                           rad
        *    trtasc3      - right ascension #3                           rad
        *    jd1, jdf1    - julian date of 1st sighting                  days from 4713 bc
        *    jd2, jdf2    - julian date of 2nd sighting                  days from 4713 bc
        *    jd3, jdf3    - julian date of 3rd sighting                  days from 4713 bc
        *    rs1          - eci site position vector #1                  km
        *    rs2          - eci site position vector #2                  km
        *    rs3          - eci site position vector #3                  km
        *    magr1in      - initial estimate 
        *    magr2in      - initial estimate
        *
        *  outputs         :
        *    r2           -  position vector at t2                       km
        *    v2           -  velocity vector at t2                       km / s
        *    errstr       - output results for debugging
        *
        *  locals         :
        *    los1         - line of sight vector for 1st
        *    los2         - line of sight vector for 2nd
        *    los3         - line of sight vector for 3rd
        *    tau          - taylor expansion series about
        *                   tau ( t - to )
        *    tausqr       - tau squared
        *    t21t23       - (t2-t1) * (t2-t3)
        *    t31t32       - (t3-t1) * (t3-t2)
        *    rho          - range from site to sat at t2                 km
        *    rhodot       -
        *    earthrate    - velocity of earth rotation
        *    p            -
        *    q            -
        *    oldr         -
        *    oldv         -
        *    f1           - f coefficient
        *    g1           -
        *    f3           -
        *    g3           -
        *
        *  coupling       :
        *    mag          - magnitude of a vector
        *    matmult      - multiply two matrices together
        *    angle        - angle between two vectors
        *
        *  references     :
        *    vallado       2013, 442, alg 52, ex 7-2
        -----------------------------------------------------------------------------*/

        public void anglesdoubler
        (
            double tdecl1, double tdecl2, double tdecl3,
            double trtasc1, double trtasc2, double trtasc3,
            double jd1, double jdf1, double jd2, double jdf2, double jd3, double jdf3,
            double[] rs1, double[] rs2, double[] rs3, double magr1in, double magr2in,
            out double[] r2, out double[] v2, out string errstr, double pctchg
        )
        {
            Int32 i, ktr;
            double f1, f2, q1, magr1, magr2, a, a1, a2, deltae32;
            double tol, tau1, tau2, tau3, magr1old, magr2old, f, g,
                magrsite1, magrsite2, magrsite3, cc1, cc2, f1delr1, f2delr1, f1delr2, f2delr2,
                pf2pr1, pf1pr2, q2, q3;
            double deltar1, deltar2, delta1, delta2, pf1pr1, pf2pr2, delta, magr1o, magr2o;  // pctchg
            int n12, n13, n23;  // number of revs for geo cases

            char dm = 'S';
            double[] poly = new double[16]; ;
            double[,] roots = new double[16, 3];
            double[] r1 = new double[3];
            double[] r3 = new double[3];
            double[] los1 = new double[3];
            double[] los2 = new double[3];
            double[] los3 = new double[3];
            string tmpstr;

            // ----------------------   initialize   ------------------------ 
            tol = 1e-8 * gravConst.re;  // km too small?
            tol = 0.1; // km
            tol = 1.0; // km
            //pctchg = 0.2;  // 0.005
            r2 = new double[] { 0.0, 0.0, 0.0 };
            v2 = new double[] { 0.0, 0.0, 0.0 };
            errstr = "";

            // ---- set middle to 0, deltas to other times ---- 
            // need to separate into jd and jdf portions for accuracy since it's often only seconds or minutes
            tau1 = (jd1 - jd2) * 86400.0 + (jdf1 - jdf2) * 86400.0; // days to sec
            tau2 = (jd3 - jd1) * 86400.0 + (jdf3 - jdf1) * 86400.0;
            tau3 = (jd3 - jd2) * 86400.0 + (jdf3 - jdf2) * 86400.0;

            // adding these in "seems" to help GEO cases where the obs are more than a day apart. 2014 JS An Improved...
            // haven't tested this for LEO orbits yet
            n12 = (int) (tau1 / 86400.0);
            n13 = (int) (tau2 / 86400.0);
            n23 = (int) (tau3 / 86400.0);

            // ----------------  find line of sight vectors  ---------------- 
            los1[0] = Math.Cos(tdecl1) * Math.Cos(trtasc1);
            los1[1] = Math.Cos(tdecl1) * Math.Sin(trtasc1);
            los1[2] = Math.Sin(tdecl1);

            los2[0] = Math.Cos(tdecl2) * Math.Cos(trtasc2);
            los2[1] = Math.Cos(tdecl2) * Math.Sin(trtasc2);
            los2[2] = Math.Sin(tdecl2);

            los3[0] = Math.Cos(tdecl3) * Math.Cos(trtasc3);
            los3[1] = Math.Cos(tdecl3) * Math.Sin(trtasc3);
            los3[2] = Math.Sin(tdecl3);

            // --------- now we're ready to start the actual double r algorithm ---------
            magr1old = 99999.9;
            magr2old = 99999.9;
            magr1o = 0.0;
            magr2o = 0.0;
            deltar1 = 0.0;
            deltar2 = 0.0;
            magrsite1 = MathTimeLibr.mag(rs1);
            magrsite2 = MathTimeLibr.mag(rs2);

            // take away negatives because escobal defines rs opposite
            cc1 = 2.0 * MathTimeLibr.dot(los1, rs1);
            cc2 = 2.0 * MathTimeLibr.dot(los2, rs2);

            ktr = 0;
           // string twobad = "";
           // double olddeltar1 = 100000.0;
            deltar1 = 0.0;
            a = 0.0;
            // main loop to get three values of the double-r for Newton iteration
            // use 10 iterations, but also check for 2 in a row that increase in abs(), if so, stop, that run is no good
            while ((Math.Abs(magr1in - magr1old) > tol || Math.Abs(magr2in - magr2old) > tol) && ktr <= 8) // && twobad != "two")
            {
                ktr = ktr + 1;
                magr1o = magr1in;
                magr2o = magr2in;

                // -------------- calculate f1 and f2 with original estimates
                doubler(cc1, cc2, magrsite1, magrsite2, magr1in, magr2in, los1, los2, los3,
                    rs1, rs2, rs3, tau1, tau3, dm, n12, n13, n23,
                    out r2, out r3, out f1, out f2, out q1, out magr2, out a, out deltae32, out tmpstr);
                errstr = errstr + tmpstr + " loop1 " + ktr.ToString() + " magr1in " + magr1in.ToString() + " magr2in " + magr2in.ToString()
                    + " " + a.ToString() + " " + q1.ToString() + "\n";
                errstr = errstr + " qs " + q1 + "\n";
                errstr = errstr + " magr1o " + magr1o.ToString() + " delr1 " + deltar1.ToString()
                    + " magr1 " + magr1in.ToString() + " " + magr1old.ToString() + "\n";
                errstr = errstr + " magr2o " + magr2o.ToString() + " delr2 " + deltar2.ToString()
                    + " magr2 " + magr2in.ToString() + " " + magr2old.ToString() + "\n";

                // check intermediate status -----------------------------------------------------
                //f = 1.0 - a / magr2 * (1.0 - Math.Cos(deltae32));
                //g = tau3 - Math.Sqrt(a * a * a / gravConst.mu) * (deltae32 - Math.Sin(deltae32));
                //for (i = 0; i < 3; i++)
                //    v2[i] = (r3[i] - f * r2[i]) / g;
                //rv2coe(r2, v2,
                //    out p, out a, out ecc, out incl, out raan, out argp, out nu, out m,
                //    out arglat, out truelon, out lonper);
                //fprintf(1, 'coes %11.4f%11.4f%13.9f%13.7f%11.5f%11.5f%11.5f%11.5f\n', ...
                //       p, a, ecc, incl * rad, omega * rad, argp * rad, nu * rad, m * rad);

                // -------------- re-calculate f1 and f2 with r1 = r1 + delta r1
                deltar1 = pctchg * magr1o;
                magr1in = magr1o + deltar1;
                magr2in = magr2o;
                doubler(cc1, cc2, magrsite1, magrsite2, magr1in, magr2in, los1, los2, los3,
                    rs1, rs2, rs3, tau1, tau3, dm, n12, n13, n23,
                    out r2, out r3, out f1delr1, out f2delr1, out q2, out magr2,
                    out a1, out deltae32, out tmpstr);
                errstr = errstr + tmpstr + "loop2 " + ktr.ToString() + " magr1in " + magr1in.ToString() + " magr2in " + magr2in.ToString()
                    + " " + a1.ToString() + " " + q2.ToString() + "\n";
                errstr = errstr + " qs " + q1 + "\n";
                errstr = errstr + " magr1o " + magr1o.ToString() + " delr1 " + deltar1.ToString()
                    + " magr1 " + magr1in.ToString() + " " + magr1old.ToString() + "\n";
                errstr = errstr + " magr2o " + magr2o.ToString() + " delr2 " + deltar2.ToString()
                    + " magr21 " + magr2in.ToString() + " " + magr2old.ToString() + "\n";

                pf1pr1 = (f1delr1 - f1) / deltar1;
                pf2pr1 = (f2delr1 - f2) / deltar1;

                // ----------------  re-calculate f1 and f2 with r2 = r2 + delta r2
                magr1in = magr1o;
                deltar2 = pctchg * magr2o;
                magr2in = magr2o + deltar2;
                doubler(cc1, cc2, magrsite1, magrsite2, magr1in, magr2in, los1, los2, los3,
                    rs1, rs2, rs3, tau1, tau3, dm, n12, n13, n23,
                    out r2, out r3, out f1delr2, out f2delr2, out q3, out magr2,
                    out a2, out deltae32, out tmpstr);
                errstr = errstr + tmpstr + " loop3 " + ktr.ToString() + " magr1in " + magr1in.ToString() + " magr2in " + magr2in.ToString()
                    + " " + a2.ToString() + " " + q3.ToString() + "\n";
                errstr = errstr + " qs " + q1 + "\n";
                errstr = errstr + " magr1o " + magr1o.ToString() + " delr1 " + deltar1.ToString()
                    + " magr1 " + magr1in.ToString() + " " + magr1old.ToString() + "\n";
                errstr = errstr + " magr2o " + magr2o.ToString() + " delr2 " + deltar2.ToString()
                    + " magr22 " + magr2in.ToString() + " " + magr2old.ToString() + "\n";

                pf1pr2 = (f1delr2 - f1) / deltar2;
                pf2pr2 = (f2delr2 - f2) / deltar2;

                // ------------ now calculate an update
                // get this back to the original, since magr1in already set back
                magr2in = magr2o;

                delta = pf1pr1 * pf2pr2 - pf2pr1 * pf1pr2;
                delta1 = pf2pr2 * f1 - pf1pr2 * f2;
                delta2 = pf1pr1 * f2 - pf2pr1 * f1;

                errstr = errstr + "delta1 " + delta1.ToString() + " delta2 " + delta2.ToString() + " mag " + delta.ToString() + "\n";
                
                if (Math.Abs(delta) < 0.001)
                {
                    deltar1 = -delta1;
                    deltar2 = -delta2;
                    errstr = errstr + "delta was 0 dr1 " + deltar1.ToString() + " dr2 " + deltar2.ToString() + "\n";
                    Console.WriteLine("delta was 0 dr1 " + deltar1.ToString() + " dr2 " + deltar2.ToString());
                }
                else
                {
                    deltar1 = -delta1 / delta;
                    deltar2 = -delta2 / delta;
                }

                // limit size of correction!
                double chkamt = 0.10;
                if (Math.Abs(deltar1 / magr1in) > chkamt)  // chg 0.10 to pctchg here
                {
                    Console.WriteLine(deltar1.ToString() + " deltar1 too large ");
                    deltar1 = magr1in * chkamt * Math.Sign(deltar1);
                }
                if (Math.Abs(deltar2 / magr2in) > chkamt)
                {
                    Console.WriteLine(deltar2.ToString() + " deltar2 too large ");
                    deltar2 = magr2in * chkamt * Math.Sign(deltar2);
                }

                errstr = errstr + "deltar1 " + deltar1.ToString() + " dr2 " + deltar2.ToString() + "\n" + "------------" + "\n";
                Console.WriteLine("deltar1 " + deltar1.ToString() + " deltar2 " + deltar2.ToString() + " " 
                    + a.ToString() + " " + a1.ToString() + " " + a2.ToString());

                magr1old = magr1in;
                magr2old = magr2in;

                magr1in = magr1in + deltar1;
                magr2in = magr2in + deltar2;  

                //fprintf(1,'qs %11.7f  %11.7f  %11.7f \n', q1, q2, q3);
                // fprintf(1,'magr1o %11.7f delr1 %11.7f magr1 %11.7f %11.7f  \n', magr1o, deltar1, magr1in, magr1old);
                // fprintf(1,'magr2o %11.7f delr2 %11.7f magr2 %11.7f %11.7f  \n', magr2o, deltar2, magr2in, magr2old);
                errstr = errstr + "\n";
                // try adaptive pctchg, smaller at end. Seems to do better.
                pctchg = pctchg * 0.5;
                //if (Math.Abs(olddeltar1) < Math.Abs(deltar1))
                //    if (twobad == "")
                //        twobad = "one";
                //    else
                //        twobad = "two";
                //olddeltar1 = deltar1;
            }  // while

            // needed to get the last iteration change into the sollution 
            doubler(cc1, cc2, magrsite1, magrsite2, magr1in, magr2in, los1, los2, los3,
                rs1, rs2, rs3, tau1, tau3, dm, n12, n13, n23,
                out r2, out r3, out f1, out f2, out q1, out magr2,
                out a, out deltae32, out tmpstr);
            errstr = errstr + tmpstr + " loop last " + ktr.ToString() + " magr1in " + magr1in.ToString()
                + " magr2in " + magr2in.ToString()
                 + " a== " + a.ToString() + " q1 " + q1.ToString() + "\n";

            errstr = errstr + " qs " + q1 + "\n";
            errstr = errstr + " magr1o " + magr1o.ToString() + " delr1 " + deltar1.ToString()
                + " magr1 " + magr1in.ToString() + " " + magr1old.ToString() + "\n";
            errstr = errstr + " magr2o " + magr2o.ToString() + " delr2 " + deltar2.ToString()
                + " magr2 " + magr2in.ToString() + " " + magr2old.ToString() + "\n";
            errstr = errstr + "n values " + n12 + " " + n13 + " " + n23 + "\n";

            // check it out here from original results 
            // hyperbolic case
            if (a < 0.0)
            {
                f = 1.0 - a / magr2 * (1.0 - Math.Cosh(deltae32));
                g = tau3 - Math.Sqrt(-a * -a * -a / gravConst.mu) * (deltae32 - Math.Sinh(deltae32));
                for (i = 0; i < 3; i++)
                    v2[i] = (r3[i] - f * r2[i]) / g;
                errstr = errstr + "v2 hyp " + v2[0].ToString() + " " + v2[1].ToString() + " " + v2[2].ToString() + "\n";
            }
            else
            {
                f = 1.0 - a / magr2 * (1.0 - Math.Cos(deltae32));
                g = tau3 - Math.Sqrt(a * a * a / gravConst.mu) * (deltae32 - Math.Sin(deltae32));
                for (i = 0; i < 3; i++)
                    v2[i] = (r3[i] - f * r2[i]) / g;
                errstr = errstr + "v2 ell " + v2[0].ToString() + " " + v2[1].ToString() + " " + v2[2].ToString() + "\n" + ktr.ToString();
            }
        } // anglesdoubler


        /* ------------------------------------------------------------------------------
        *
        *                                  anglesgooding
        *
        *   compute orbit from three observed lines of sight (angles only). Uses the gooding
        *     approach. he lists code in his papers, but no driver routines, and the CMDA 
        *     implementation is rather different.
        *
        *  author        : david vallado           davallado@gmail.com   22 jun 2002
        *
        *  inputs          description                              range / units
        *   ind     =  indicator identifying which of the two
        *                          (when there are two) lambert solutions
        *                          to use.
        *                               = 0       if nhrev = 0 or 1
        *                               = 0 or 1  if nhrev.ge. 2
        *    trtasc1      - right ascension #1                           rad
        *    trtasc2      - right ascension #2                           rad
        *    trtasc3      - right ascension #3                           rad
        *    tdecl1       - declination #1                               rad
        *    tdecl2       - declination #2                               rad
        *    tdecl3       - declination #3                               rad
        *    jd1          - julian date of 1st sighting            days from 4713 bc
        *    jd2          - julian date of 2nd sighting            days from 4713 bc
        *    jd3          - julian date of 3rd sighting            days from 4713 bc
        *    rs           -  site position vector                        km
        *    rng1         -  initial range estimate at time t1
        *    rng3         -  initial range estimate at time t3
        *    rng1         - converged value of range estimate
        *    numhalfrev   - number of half evolutions between observations
        *                   number of half-revs(k) included in the angle p1-*-p3
        *   
        *    outputs      :
        *    r2           - middle observation position vector (eci)      km
        *    v2           - middle observation velocity vector (eci)      km/s
        *   the following is in solution array for each solution see "ix_solution" for layout
        *
        *   itnum   = iteration count
        *   cr      = |*| where * is the calculated version of range vector at time t2.
        *   crit    = convergence test value(solution converges when  crit^2 <  critval)
        *   axrtio  = ratio(minor:major) for an ellipse in the
        *                   f/g plane(& also the rho1/rho3); is a function
        *                   of the partials and indicates the condition of
        *                   the jacobian matrix: 0 = 'singular', 1 = ideal
        *
        *   bearng =  direction of corresponding axes in the
        *                         rho1/rho3 plane.
        *                         [corrected 15.4.04 by halving the angle]
        *   a, e, i, bom, q = computed orbital elements
        *   xs(6)  = computed pos/vel at time t2
        *
        *  references     :
        *    vallado 2021 Chap 7
        *    gooding tr 93004, april 1993. 
        *    cmda 1997
        * ------------------------------------------------------------------------------ */

        public void anglesgooding
        (
            double tdecl1, double tdecl2, double tdecl3,
            double trtasc1, double trtasc2, double trtasc3,
            double jd1, double jdf1, double jd2, double jdf2, double jd3, double jdf3,
            double[] rs1, double[] rs2, double[] rs3,
            Int32 numhalfrev, double rng1, double rng2, double rng3,
            out double[] r2, out double[] v2, out string errstr
        )
        {
            int ind;
            double tau12;
            double tau13;
            int nsol;

            double[] r2eci = new double[3];
            double[] v2eci = new double[3];
            double[] los1 = new double[3];
            double[] los2 = new double[3];
            double[] los3 = new double[3];
            //bool lknst;
            double[] data = new double[21];
            double[,] data2 = new double[3, 7];
            double[] datls2 = new double[3];
            //double ro1st, ro3st, dq;
            double[] xs = new double[6];
            double alpha, a, ecc, rp, incl, raan, argp;
            double p, arglat, truelon, lonper, nu, m;
            int ngm, nmod, nfail, itnum, ikn;
            double crit, axrtio, bearng;

            double rad = 180.0 / Math.PI;
            string tmpstr;
            errstr = "";

            double[] r1 = new double[] { 0.0, 0.0, 0.0 };
            r2 = new double[] { 0.0, 0.0, 0.0 };
            double[] r3 = new double[] { 0.0, 0.0, 0.0 };
            v2 = new double[] { 0.0, 0.0, 0.0 };

            //int maxit, kzero, nmarsd, ncdref, npolar, nswap, nlambt, nautct;
            double pdinc; //, t12mod, t13mod, hn;
            //bool lobdp, lautop, lctrans, lminly, lgmean, lautoq, lfixax, lfaxa2, lmodfy;

            //maxit = 25;                 //Limit to itns(cf code 8001)
            //                            // lobdp = true;          //Observ'ns 'dependent' basic
            //kzero = 0;             //Values for 'k' range from 0
            //lautop = false;       //'Automat operation' not used
            //ncdref = 0;               //Code ref for k(in 'auto')
            //                          //NAUTCT = 0                                                       //(std, now changed to - 10000)
            //lautoq = false;         //No spec Table to be set up
            //nmarsd = 0;            //Data from DATA1, not 3 or 4
            //npolar = 0;          //Starters normal, NOT 'polar'

            pdinc = 1e-3;                     //1.0e-5  Increment for num partials, needed
            //lgmean = true;       //Geometric means are used
            //lctrans = false;        //Not use 'common transversal'
            //t12mod = 0.0;                //No change to interval T12
            //t13mod = 0.0;              //No change to interval T13
            //nswap = 0;             //1st data set NOT swapped
            //lminly = false;      //NOT going for 'min goal'
            //nlambt = 0;               //Use both Lamb sols(if poss)
            //lfixax = false;           //Axes changed after 1st it'n???
            //lfaxa2 = false;           //Axes changed during 1st it'n
            //lmodfy = true;              //Allow modding between it'ns
            //nautct = -10000;               //Std value now, not 0 above
            //hn = 0.5;                 //Halley value(basic)

            /* ----------------  find line of sight vectors  ---------------- */
            los1[0] = Math.Cos(tdecl1) * Math.Cos(trtasc1);
            los1[1] = Math.Cos(tdecl1) * Math.Sin(trtasc1);
            los1[2] = Math.Sin(tdecl1);

            los2[0] = Math.Cos(tdecl2) * Math.Cos(trtasc2);
            los2[1] = Math.Cos(tdecl2) * Math.Sin(trtasc2);
            los2[2] = Math.Sin(tdecl2);

            los3[0] = Math.Cos(tdecl3) * Math.Cos(trtasc3);
            los3[1] = Math.Cos(tdecl3) * Math.Sin(trtasc3);
            los3[2] = Math.Sin(tdecl3);

            //MathTimeLibr.determinant(los1[0] * (los2[1] * los3[2] - los3[1] * los2[2]) +
            //    los2[0] * (los3[1] * los1[2] - los1[1] * los3[2]) + los3[0] *
            //    (los1[1] * los2[2] - los2[1] * los1[2]);
            //MathTimeLibr.matvecmult(rs1eci, los1, ol1, om1, on1);
            //MathTimeLibr.matvecmult(ox2, oy2, oz2, los2, ol2, om2, on2);
            //MathTimeLibr.matvecmult(rs3eci, los3, ol3, om3, on3);

            //ro1st = rng1;
            //ro3st = rng3;

            //if (numhalfrev < 2)
            //    ind = 0;

            ikn = 0;
            nsol = 0;

            //  5 continue
            //dot = ol1 * (om2 * on3 - om3 * on2) + ol2 * (om3 * on1 - om1 * on3) + ol3 *
            //    (om1 * on2 - om2 * on1);
            //dq = MathTimeLibr.mag(ol1, om1, on1) * MathTimeLibr.mag(ol2, om2, on2) * 
            //    MathTimeLibr.mag(ol3, om3, on3);
            ////
            //  if (ind = 1  &  nlambt = 2) go to 61
            // need to separate into jd and jdf portions for accuracy since it's often only seconds or minutes
            tau12 = (jd2 - jd1) * 86400.0 + (jdf2 - jdf1) * 86400.0; // days to sec
            //tau13 = (jd1 - jd3) * 86400.0 + (jdf1 - jdf3) * 86400.0;
            tau13 = (jd3 - jd1) * 86400.0 + (jdf3 - jdf1) * 86400.0;

            if (tau12 > 86400.0)
                numhalfrev = 2 * (int) Math.Floor(tau12 / 86400.0);
            if (tau13 > 86400.0)
                numhalfrev = 2 * (int)Math.Floor(tau13 / 86400.0);

            itnum = 25;
            alpha = gravConst.mu / rng1;  // 12245 km reciprocal of sma
            ind = 0;
            crit = 10.0;
            axrtio = 0.3;
            bearng = 0.3;
            ngm = 0;
            nmod = 0;
            rp = 6800.0;  // 1.05;

            obs3lsx(los1, los2, los3, rs1, rs2, rs3, numhalfrev, pdinc, ind, ikn,
                tau12, tau13, rng1, rng3, alpha, itnum, ngm, nmod, out nfail,
                rng2, crit, axrtio, bearng, out r1, out r2, out r3, out tmpstr);
            errstr = errstr + tmpstr;

            //obs3lsx(numhalfrev, ind, obs, tau12, tau13, rng1, rng3, itnum, ngm, nmod, 
            //    out nfail, cr, crit,
            //    axrtio, bearng, xs);
            //
            double theta, theta1, copa;
            string error;
            gibbs(r1, r2, r3, out v2, out theta, out theta1, out copa, out error);

            if (error.Equals("ok") && (Math.Abs(theta) < 1.0 / rad || Math.Abs(theta1) < 1.0 / rad))
            {
                // hgibbs to get middle vector ---- 
                herrgibbs(r1, r2, r3, jd1 + jdf1, jd2 + jdf2, jd3 + jdf3, out v2, out theta, out theta1, out copa, out error);
            }

            rv2coe(r2, v2,
                out p, out a, out ecc, out incl, out raan, out argp, out nu, out m,
                out arglat, out truelon, out lonper);
            // al, q, ei, bom, om, tauxx
            // alpha (1/a), rp, ecc, bom, argp, tau since perigee
            // pv3els(r2, v2, alpha, rp, incl, raan, argp, tauxx);

            alpha = gravConst.mu / a;

            if (alpha == 0.0)
                a = 0.0;
            else
                a = gravConst.mu / alpha;

            ecc = 1.0 - rp * alpha / gravConst.mu;
            incl = incl * rad;
            raan = raan * rad;
            //
            //		call els3pv(gm, al, q, ei, bom, om, tauxx, xs(1), xs(2), xs(3), xs(4), xs(5), xs(6))
            //
            nsol = nsol + 1;


            //    // ------------------------------------------------------------------------------
            //lknst = false;

            //if (nfail == 0 && ikn < 8)
            //{
            //    ikn = ikn + 1;
            //    ro1kn[ikn] = rng1;
            //    r1knsq[ikn] = Math.Pow(rs1eci[0] + rng1 * los1[0], 2) +
            //        Math.Pow(rs1eci[1] + rng1 * los1[1], 2) +
            //        Math.Pow(rs1eci[2] + rng1 * los1[2], 2);
            //    rng1 = ro1st;
            //    ro3kn[ikn] = rng3;
            //    r3knsq[ikn] = Math.Pow(rs3eci[0] + rng3 * los3[0], 2) +
            //        Math.Pow(rs3eci[1] + rng3 * los3[1], 2) +
            //        Math.Pow(rs3eci[2] + rng3 * los3[2], 2);
            //    rng3 = ro3st;
            //}
            //if (lknst)
            //    if (ikn == 1)
            //    {
            //        rng1 = 2.0 * ro1kn[ikn] - ro1st;
            //        rng3 = 2.0 * ro3kn[ikn] - ro3st;
            //    }
            //    else
            //    {
            //        rng1 = 2.0 * ro1kn[ikn] - ro1kn(ikn - 1);
            //        rng3 = 2.0 * ro3kn[ikn] - ro3kn(ikn - 1);
            //    }
            //  go to 5
        }  // anglesgooding



        /* ------------------------------------------------------------------------------
        *                                obs3lsx
        *                                
        *  compute orbit from three observed lines of sight (angles only). this is a subroutine of the 
        *    Gooding angles-only method.                                                                   
        *                                                                  
        *  author        : david vallado           davallado@gmail.com    6 apr 2021
        *
        *  inputs          description                                          range / units
        *    tau12       - t2 - t1                                                s  
        *    tau13       - t3 - t1                                                s
        *    rs1eci      - site position vector #1 eci                            km
        *    rs2eci      - site position vector #2 eci                            km
        *    rs3eci      - site position vector #3 eci                            km
        *    rho1        - assumed range at time t1                               km
        *    rho3        - assumed range at time t3                               km
        *    numhalfrev  - number of half-revs (k) included in input angle p1-*-p3
        *    ind         - indicator for which of two lambert solutions to use
        *                    = 0       if numHalfRev  (k) = 0 or 1          
        *                    = 0 or 1  if numHalfRev  (k) >= 2            
        *    r1          -  position vector #1                                    km
        *    r2          -  position vector #2                                    km
        *    r3          -  position vector #3                                    km
        *    magr1, magr2, magr3     = computed position magnitude from earth center at times t1, t2, t3          
        *                                                                    
        *  outputs       :
        *    num         - number of solutions found by lambert valamb    (0, unlikely 1, 2)
        *    rho2sez     - slant range vector for t2                              km
        *    
        *  locals        :
        *   cntrol = input flags & controls inputs see original gooding /cntrol/ common     
        *                          see beginning of code for layout                   
        *                                                                     
        *               nhrev   =  number of half-revs(k) included in the 
        *                          angle p1-*-p3                           
        *               t12     =  t2 - t1                                 
        *               t13     =  t3 - t1                                 
        *               r01     =  initial range estimate at time t1       
        *               r03     =  initial range estimate at time t3       
        *   r01 = converged value of range estimate outputs at time t1(if obsls "successful")
        *   r03     = converged value of range estimate at time t13(if obsls "successful")
        *               itnum   = iteration count      
        *               hn = 0.5;    'halley'/'modified newton-raphson' control
        *                           =0.5 for halley,  1.0 for modified newton-raphson
        *               ngm     = counts use of "g" means                  
        *               nmod    = overall modification count               
        *               nfail   = count of number of lamber failures       
        *               cr      = |*| where * is the calculated version of 
        *                         range vector at time t2.
        *               crit    = convergence test value(solution
        *                         converges when  crit^2 <  critval)
        *               axrtio  = ratio(minor:major) for an ellipse in the
        *                   f/g plane(& also the rho1/rho3); is a function 
        *                   of the partials and indicates the condition of 
        *                   the jacobian matrix: 0 = 'singular', 1 = ideal 
        *               bearng =  direction of corresponding axes in the rho1/rho3 plane.
        *                         [corrected 15.4.04 by halving the angle]
        *               xs[6]  = computed pos/vel at time t2   
        *   pdinc      -    starting partial derivative "increment":
        *                    delta-x = pdinc* r1, delta-y = pdinc* r3
        *                crival;  "square" of convergence criteria
        *   numsoltns  - number of solutions from the lambert trials 
        *                 note this will vary as trajectories hit the earth
        *                 
        *  references     :
        *    gooding tr 93004, april 1993. 
        *    cmda 1997
        * ------------------------------------------------------------------------------ */

        public void obs3lsx
        (
            double[] los1, double[] los2, double[] los3,
            double[] rs1eci, double[] rs2eci, double[] rs3eci,
            int numhalfrev, double pdinc, int ind, int ikn,
            double tau12, double tau13, double rng1, double rng3, double alpha,
            int itnum, int ngm, int nmod, out int nfail,
            double rng2, double crit, double axrtio, double bearng, out double[] r1, out double[] r2, out double[] r3, out string errstr
        )
        {
            double magr1, magr2, magr3;
            double[] r = new double[3];
            double[] rho2sez = new double[3];
            double[] pvec = new double[3];
            double[] qvec = new double[3];
            double[] qvecest = new double[3];
            double f, g, crtold, critsq, d1, d2, d3, d4, d3d1;
            double dillc, den, dr, fc, fcold, fgxy, fgxy2, fsum, pdincm;
            double r10, r30, rng1old, rng3old, magqvecest;
            double magpvec, dro1, d2ro1, dro1sq;
            double dro3, d2ro3, dro3sq, fm1, fp1, fd1, fdd1;
            double fm3, fp3, fd3, fdd3, gm1, gp1, gd1, gdd1;
            double gm3, gp3, gd3, gdd3, f13, fd13;
            double g13, gd13, rofac, del, fgdd11, fgdd13, fgdd33;
            double delh, ddel, fd1h, fd3h, gd1h, gd3h, d1nr, d3nr, sqar;
            double ffggd1, ffggd3, fg11dd, fg13dd, fg33dd;
            double x13, y13, z13;
            double[] w = new double[9];
            double[] w1 = new double[9];
            double[] w3 = new double[9];
            double[] uw = new double[9];
            double[] ro1sq = new double[9];
            double[] ro3sq = new double[9];
            double[] ro1kno = new double[9];
            double[] ro3kno = new double[9];
            double[] ro1qu = new double[9];
            double[] ro3qu = new double[9];
            double[] ro13sq = new double[9];
            double[] ro13qu = new double[9];

            // min range for the observation (in km). good for ground and space based observations
            double minrng = 300.0;
            // max number of iterations per solution
            int maxit = 20;
            double rad = 180.0 / Math.PI;
            double small = 0.000000001;
            string tmpstr;
            errstr = "";


            // bool lobdep = true;  // true if range starting estimates are observer-dependent
            bool lgmean = false; // true if to use geometric("g") means
            bool lminly = false;  // true if "minimization" run, i.e.solve for minimum
                                  // (derivative = 0) instead of function = 0 
            //bool lfixax = true;  // true if keep axes(use g = / 0 !) after iteration 1
            //bool lfxax2 = true;  // true if keep axes(use g = / 0 !) even in iter. 1 
            bool lmodfy = false;  // true for inter-iteration modding(?)
            //int npolar;  // special polar starters
            double hn = 0.5;      // 'halley'/'modified newton-raphson' control
                                  // =0.5 for halley,  1.0 for modified newton-raphson
            double crival;  //"square" of convergence criteria

            //bool lfxax3;
            bool lmincv;
            bool lspec;
            int i, npdf, nflmod, nmods, numsoltns;
            int[] nldf = new int[6];
            d1 = 0.0;
            d3 = 0.0;
            rng1old = minrng;
            rng3old = minrng;
            r1 = new double[] { 0.0, 0.0, 0.0 };
            r2 = new double[] { 0.0, 0.0, 0.0 };
            r3 = new double[] { 0.0, 0.0, 0.0 };
            double[] ro1kn = new double[9] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            double[] ro3kn = new double[9] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            double[] r1knsq = new double[9] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            double[] r3knsq = new double[9] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };

            axrtio = 0.0;
            fcold = 0.0;        // zero old 'f' ['f' & 'g' (= 0) are target fns computed for point 'p2)]
            nfail = 0;          // not fail(yet!)
            d1nr = 0.0;
            d3nr = 0.0;
            fc = 0.0;
            crtold = 0.0;
            ddel = 0.0;
            magr2 = rng2;  // don't set to 0.0

            pdincm = pdinc;    // 1

            if (lminly)
                crtold = 0.0;
            lmincv = false;
            crival = 100.0;

            // convergence deemed for 'alternative goal' of minimization run, if/when.true.
            nmod = 0;  // 1
            ngm = 0;

            if (ikn == 0 && Math.Abs(rng1 - rng3) < small)  // lobdep &&
            {
                calcps(los1, los3, rs1eci, rs2eci, rs3eci, numhalfrev, tau12, tau13, rng1, rng3, ind,
                    out numsoltns, out magr1, out magr3, out r1, out r3, out rho2sez, out tmpstr);
                errstr = errstr + tmpstr;

                if (numsoltns > 0)
                {
                    rng2 = MathTimeLibr.dot(los2, rho2sez);
                    // rnage2 estimate cannot be < 0.0 km!
                    if (rng2 < 0.0)
                    {
                        // control the part deriv est of the next step (1e-5*rsites)
                        dr = pdinc * (magr1 + magr3);
                        calcps(los1, los3, rs1eci, rs2eci, rs3eci, numhalfrev, tau12, tau13,
                            rng1 + dr, rng3 + dr, ind, out numsoltns, out magr1, out magr3, out r1, out r3, out rho2sez, out tmpstr);
                        errstr = errstr + tmpstr;
                        rng1 = rng1 + dr * rng2 / (rng2 - los2[0] * rho2sez[0] - los2[1] * rho2sez[1] -
                            los2[2] * rho2sez[2]);
                        if (rng1 < 0.0)
                            rng1 = minrng;
                        rng3 = rng1;
                        //if (lpr)
                        //    write(2, 4000) rng1
                        //4000 format('prelim starter(s) replaced by ', g23.15)
                    }
                }
                else
                {
                    // do something???
                }
            }

            // loop till finished or max iterations
            itnum = 1;
            critsq = crival + 1.0;
            while (itnum <= maxit && nfail <= 3 && critsq > crival)  // do 8
            {
                npdf = 0;
                nmods = 0;  // GOTO 2
                // nmod for overall mods count; nmodfy is count for current 
                // iteration, limited to 2

                // to switch to halley if mod nr initially
                if (itnum == 21)
                    hn = 0.5;

                nflmod = 0;

                // GOTO 3 computed slant range vector at t2
                calcps(los1, los3, rs1eci, rs2eci, rs3eci, numhalfrev, tau12, tau13, rng1, rng3, ind,
                    out numsoltns, out magr1, out magr3, out r1, out r3, out rho2sez, out tmpstr);
                errstr = errstr + tmpstr;
                r10 = magr1;
                r30 = magr3;

                // need to check Lambert output




                if (numsoltns == 0)
                {
                    // tricks to resume with diff start
                    // d1 and d3 will be set after one iteration
                    if (itnum > 1 && nflmod < 3)
                    {
                        nflmod = nflmod + 1;
                        d3 = d3 / 3.0;
                        d1 = d1 / 3.0;
                        rng1 = rng1old + d3;
                        rng3 = rng3old + d1;
                        //if (lpr)
                        //    write(2, 5000) nflmod, rng1, rng3
                        //5000 format('lambert fail', i2, ' so cut est by 2/3 to', 2e18.10)
                        //goto 3;
                        calcps(los1, los3, rs1eci, rs2eci, rs3eci, numhalfrev, tau12, tau13, rng1, rng3, ind,
                            out numsoltns, out magr1, out magr3, out r1, out r3, out rho2sez, out tmpstr);
                        errstr = errstr + tmpstr;
                        r10 = magr1;
                        r30 = magr3;
                        //hmmh just let it go back???????
                    }
                    else
                    {
                        nfail = nfail + 1;

                        switch (nfail)
                        {
                            // try to avoid lambert-fail, by closest-point starters
                            case 1:
                                rng1 = -MathTimeLibr.dot(los1, rs1eci);
                                rng3 = -MathTimeLibr.dot(los3, rs3eci);
                                if (rng1 < 0.0)  // lobdep && 
                                    rng1 = minrng;
                                if (rng3 < 0.0) //lobdep &&
                                    rng3 = minrng;
                                //if (lpr) write(2, 5016) itnum, rng1, rng3:
                                //5016 format( 'revised starters after lambert fail (iteration',
                                //    &       i3, ')'/ ' (closest pts or zeros)', e21.13, ' & ', e21.13)
                                //goto 1;
                                break;
                            // try to avoid lambert-fail, by common-perpendicular starters
                            case 2:
                                x13 = rs3eci[0] - rs1eci[0];
                                y13 = rs3eci[1] - rs1eci[1];
                                z13 = rs3eci[2] - rs1eci[2];
                                d1 = x13 * los1[0] + y13 * los1[1] + z13 * los1[2];
                                d3 = x13 * los3[0] + y13 * los3[1] + z13 * los3[2];
                                d2 = los1[0] * los3[0] + los1[1] * los3[1] + los1[2] * los3[2];
                                d4 = 1.0 - d2 * d2;
                                rng1 = (d1 - d3 * d2) / d4;
                                rng3 = (d1 * d2 - d3) / d4;
                                if (rng1 < 0.0)  // lobdep && 
                                    rng1 = minrng;
                                if (rng3 < 0.0)  // lobdep && 
                                    rng3 = minrng;
                                //write(2, 5006) itnum, rng1, rng3
                                //5006 format( 'revised starters after lambert fail (iteration',
                                //    &       i3, ')'/ ' (common perp. or zeros)', e21.13, ' & ', e21.13)
                                //goto 1;
                                break;
                            // last chance via zero (min range) starters
                            case 3:
                                rng1 = minrng;
                                rng3 = minrng;
                                //write(2, 5007) itnum, rng1, rng3
                                //5007 format( 'revised starters after lambert fail (iteration',
                                //    &       i3, ')'/ 'viz (forced zeros!)', e21.13, ' & ', e21.13)
                                // goto 1
                                break;
                            default:
                                errstr = errstr + "default return" + "\n";
                                return;
                        }  // switch

                        //goto 1 ----------------- put it here
                        pdincm = pdinc;    // 1

                        if (lminly)
                            crtold = 0.0;
                        lmincv = false;
                        crival = 100.0;

                        // convergence deemed for 'alternative goal' of minimization run, if/when true
                        nmod = 0;  // 1
                        ngm = 0;

                        if (ikn == 0 && rng1 == rng3) // lobdep &&
                        {
                            calcps(los1, los3, rs1eci, rs2eci, rs3eci, numhalfrev, tau12, tau13, rng1, rng3, ind,
                                out numsoltns, out magr1, out magr3, out r1, out r3, out rho2sez, out tmpstr);
                            errstr = errstr + tmpstr + "rhosez a " + rho2sez[0].ToString() + " " + rho2sez[1].ToString() + " "
                                + rho2sez[2].ToString() + "\n";
                            if (numsoltns > 0)
                            {
                                rng2 = MathTimeLibr.dot(los2, rho2sez);
                                if (rng2 < 0.0)
                                {
                                    dr = pdinc * (magr1 + magr3);
                                    calcps(los1, los3, rs1eci, rs2eci, rs3eci, numhalfrev, tau12, tau13,
                                        rng1 + dr, rng3 + dr, ind,
                                        out numsoltns, out magr1, out magr3, out r1, out r3, out rho2sez, out tmpstr);
                                    errstr = errstr + tmpstr + "rhosez b " + rho2sez[0].ToString() + " " + rho2sez[1].ToString() + " "
                                        + rho2sez[2].ToString() + "\n";
                                    rng1 = rng1 + dr * rng2 / (rng2 - los2[0] * rho2sez[0] - los2[1] * rho2sez[1] -
                                        los2[2] * rho2sez[2]);
                                    if (rng1 < 0.0)
                                        rng1 = minrng;
                                    rng3 = rng1;
                                    //if (lpr)
                                    //    write(2, 4000) rng1
                                    //4000 format('prelim starter(s) replaced by ', g23.15)
                                }
                            }
                            else
                            {
                                // do something???
                            }
                        }

                        // loop till finished or max iterations
                        itnum = 1;
                        critsq = crival + 1.0;
                    }
                }  // if numsltns == 0
                   // end of possible restart options
                else
                {
                    rng2 = MathTimeLibr.dot(los2, rho2sez);
                    // cr only used, non-critically, in denom of convergence test

                    //if (al != 0.0)
                    //    al = gravConst.mu / al;
                    //ecc = 1.0 - q * al / gm1;  // eccentricity (old los2[2])
                    //eideg = ei * radian;  // inclination

                    // ----- form the plane perpendicular to the los2 observed position ------
                    // plane vectors are pvec and qvec
                    MathTimeLibr.cross(los2, rho2sez, out qvec);
                    MathTimeLibr.cross(qvec, los2, out pvec);
                    magpvec = MathTimeLibr.mag(pvec);
                    // refind the qvec axis
                    MathTimeLibr.cross(los2, pvec, out qvecest);
                    magqvecest = MathTimeLibr.mag(qvecest);

                    // test for convergence
                    errstr = errstr + " difference " + magqvecest.ToString() + " " + itnum.ToString() + " " + nfail.ToString() + "\n";
                    if (Math.Abs(magqvecest) < small)
                    {
                        // converged!
                        crit = 0.0;
                        critsq = 0.0;
                        axrtio = 0.0;
                        // goto 7; done
                    }
                    else
                    {
                        // not converged yet
                        g = 0.0;
                        f = MathTimeLibr.dot(pvec, rho2sez) / magpvec;  // 99
                        fc = f;
                        g = MathTimeLibr.dot(qvec, rho2sez) / magqvecest;

                        // loop for 'known solutions'
                        for (i = 0; i < ikn; i++)     // 4
                        {
                            ro1kno[i] = rng1 - ro1kn[i];
                            ro3kno[i] = rng3 - ro3kn[i];
                            ro1sq[i] = ro1kno[i] * ro1kno[i];
                            ro3sq[i] = ro3kno[i] * ro3kno[i];
                            ro1qu[i] = ro1sq[i] + r1knsq[i];
                            ro3qu[i] = ro3sq[i] + r3knsq[i];
                            ro13sq[i] = ro1sq[i] + ro3sq[i];
                            ro13qu[i] = ro1qu[i] + ro3qu[i];
                            fc = fc * Math.Sqrt(ro13qu[i] / ro13sq[i]);
                        }  // for


                        lspec = itnum > 1;
                        // to avoid amstrad fault when itnum > 1 direct in next line
                        if (lspec && itnum > 1 && fc > 2.0 * fcold && nmods < 2 && !lminly && lmodfy)
                        {
                            fsum = fc + fcold;
                            rng1 = (fc * rng1old + fcold * rng1) / fsum;
                            rng3 = (fc * rng3old + fcold * rng3) / fsum;
                            //if (lpr)
                            //    write(2,5009) itnum - 1, itnum, fc, fcold, rng1, rng3;
                            //5009 format( 'mod it''n', i3, '/', i2, ', new & old fc being',
                            //    &   2g15.5/  '    so that rng1 & rng3 change to ', 2g18.6)
                            nmods = nmods + 1;
                            nmod = nmod + 1;
                            //goto 3;
                            calcps(los1, los3, rs1eci, rs2eci, rs3eci, numhalfrev, tau12, tau13, rng1, rng3, ind,
                                out numsoltns, out magr1, out magr3, out r1, out r3, out rho2sez, out tmpstr);
                            errstr = errstr + tmpstr + "rhosez c " + rho2sez[0].ToString() + " " + rho2sez[1].ToString() + " "
                                + rho2sez[2].ToString() + "\n";
                            r10 = magr1;
                            r30 = magr3;
                            //
                            //
                            // but still missing some more stuff beyond the goto......
                            //
                            //
                        }

                        // ------------------- now get partials for (in principle) 2d-halley
                        // increment using essentially arbitrary values
                        dro1 = pdincm * r10;
                        dro3 = pdincm * r30;
                        if (itnum >= 31)
                        {
                            d3d1 = Math.Max(d3, d1);
                            dro1 = Math.Min(dro1, d3d1);
                            dro3 = Math.Min(dro3, d3d1);
                        }
                        d2ro1 = 2.0 * dro1;
                        d2ro3 = 2.0 * dro3;
                        dro1sq = dro1 * dro1;
                        dro3sq = dro3 * dro3;
                        calcps(los1, los3, rs1eci, rs2eci, rs3eci, numhalfrev, tau12, tau13,
                            rng1 - dro1, rng3, ind,
                            out nldf[1], out magr1, out magr3, out r1, out r3, out rho2sez, out tmpstr);
                        errstr = errstr + tmpstr + "rhosez d1 " + rho2sez[0].ToString() + " " + rho2sez[1].ToString() + " " + rho2sez[2].ToString() + "\n";
                        fm1 = MathTimeLibr.dot(pvec, rho2sez) / magpvec - f;
                        gm1 = MathTimeLibr.dot(qvecest, rho2sez) / magqvecest - g;
                        calcps(los1, los3, rs1eci, rs2eci, rs3eci, numhalfrev, tau12, tau13,
                            rng1 + dro1, rng3, ind,
                            out nldf[2], out magr1, out magr3, out r1, out r3, out rho2sez, out tmpstr);
                        errstr = errstr + tmpstr + "rhosez d2 " + rho2sez[0].ToString() + " " + rho2sez[1].ToString() + " " + rho2sez[2].ToString() + "\n";
                        fp1 = MathTimeLibr.dot(pvec, rho2sez) / magpvec - f;
                        gp1 = MathTimeLibr.dot(qvecest, rho2sez) / magqvecest - g;
                        fd1 = (fp1 - fm1) / d2ro1;
                        fdd1 = (fp1 + fm1) / dro1sq;
                        gd1 = (gp1 - gm1) / d2ro1;
                        gdd1 = (gp1 + gm1) / dro1sq;
                        calcps(los1, los3, rs1eci, rs2eci, rs3eci, numhalfrev, tau12, tau13,
                            rng1, rng3 - dro3, ind,
                            out nldf[3], out magr1, out magr3, out r1, out r3, out rho2sez, out tmpstr);
                        errstr = errstr + tmpstr + "rhosez d3 " + rho2sez[0].ToString() + " " + rho2sez[1].ToString() + " " + rho2sez[2].ToString() + "\n";
                        fm3 = MathTimeLibr.dot(pvec, rho2sez) / magpvec - f;
                        gm3 = MathTimeLibr.dot(qvecest, rho2sez) / magqvecest - g;
                        calcps(los1, los3, rs1eci, rs2eci, rs3eci, numhalfrev, tau12, tau13,
                            rng1, rng3 + dro3, ind,
                            out nldf[4], out magr1, out magr3, out r1, out r3, out rho2sez, out tmpstr);
                        errstr = errstr + tmpstr + "rhosez d4 " + rho2sez[0].ToString() + " " + rho2sez[1].ToString() + " " + rho2sez[2].ToString() + "\n";
                        fp3 = MathTimeLibr.dot(pvec, rho2sez) / magpvec - f;
                        gp3 = MathTimeLibr.dot(qvecest, rho2sez) / magqvecest - g;
                        fd3 = (fp3 - fm3) / d2ro3;
                        fdd3 = (fp3 + fm3) / dro3sq;
                        gd3 = (gp3 - gm3) / d2ro3;
                        gdd3 = (gp3 + gm3) / dro3sq;
                        calcps(los1, los3, rs1eci, rs2eci, rs3eci, numhalfrev, tau12, tau13,
                            rng1 + dro1, rng3 + dro3, ind,
                            out nldf[5], out magr1, out magr3, out r1, out r3, out rho2sez, out tmpstr);
                        errstr = errstr + tmpstr + "rhosez d5 " + rho2sez[0].ToString() + " " + rho2sez[1].ToString() + " " + rho2sez[2].ToString() + "\n";
                        f13 = MathTimeLibr.dot(pvec, rho2sez) / magpvec - f;
                        g13 = MathTimeLibr.dot(qvecest, rho2sez) / magqvecest - g;
                        rofac = dro1 / dro3;
                        fd13 = f13 / (dro1 * dro3) -
                            (fd1 / dro3 + fd3 / dro1) - 0.5 * (fdd1 * rofac + fdd3 / rofac);
                        gd13 = g13 / (dro1 * dro3) -
                            (gd1 / dro3 + gd3 / dro1) - 0.5 * (gdd1 * rofac + gdd3 / rofac);

                        // check if there are enough roots in the various solution tests
                        for (i = 1; i <= 5; i++)  // GOTO 5 done
                        {
                            if (nldf[i] == 0)
                            {
                                npdf = npdf + 1;
                                if (npdf <= 3)
                                {
                                    pdincm = pdincm * 0.1;
                                    //if (lpr) write(2, 5010)
                                    //5010 format('reduce pdincm')
                                    //goto 2;
                                    nmods = 0;
                                    nflmod = 0;
                                    calcps(los1, los3, rs1eci, rs2eci, rs3eci, numhalfrev, tau12, tau13, rng1, rng3, ind,
                                        out numsoltns, out magr1, out magr3, out r1, out r3, out rho2sez, out tmpstr);
                                    errstr = errstr + tmpstr;
                                    r10 = magr1;
                                    r30 = magr3;
                                    //
                                    //
                                    // but still missing some more stuff beyond the goto......
                                    //
                                    //
                                }
                                else
                                {
                                    nfail = 1;
                                    errstr = errstr + "nfail 1 opt  return" + "\n";
                                    return;
                                }
                            }
                        }  // 5 continue

                        for (i = 0; i < ikn; i++)  // do 6
                        {
                            w[i] = 1.0 / ro13sq[i] - 1.0 / ro13qu[i];
                            uw[i] = w[i] - (2.0 / ro13sq[i] + 2.0 / ro13qu[i]);
                            w1[i] = w[i] * ro1kno[i];
                            w3[i] = w[i] * ro3kno[i];
                            fd1 = fd1 - f * w1[i];
                            fd3 = fd3 - f * w3[i];
                            fdd1 = fdd1 - (2.0 * fd1 * w1[i] + w[i] * f * (1.0 + ro1sq[i] * uw[i]));
                            gdd1 = gdd1 - 2.0 * w1[i] * gd1;
                            fdd3 = fdd3 - (2.0 * fd3 * w3[i] + w[i] * f * (1.0 + ro3sq[i] * uw[i]));
                            gdd3 = gdd3 - 2.0 * w3[i] * gd3;
                            fd13 = fd13 - (fd3 * w1[i] + fd1 * w3[i] + w[i] * f * ro1kno[i] * ro3kno[i] * uw[i]);
                            gd13 = gd13 - (w1[i] * gd3 + w3[i] * gd1);
                        }   // 6 continue

                        del = fd1 * gd3 - fd3 * gd1;

                        // this is some extra stuff...below... from original
                        fgdd11 = fd1 * fd1 + gd1 * gd1;
                        fgdd13 = fd1 * fd3 + gd1 * gd3;
                        fgdd33 = fd3 * fd3 + gd3 * gd3;

                        if (lminly)
                        {
                            ddel = fdd1 * fdd3 - fd13 * fd13;
                            delh = f * (f * ddel + fdd1 * fgdd33 - 2.0 * fd13 * fgdd13 + fdd3 * fgdd11)
                                   + del * del + g * (g * (gdd1 * gdd3 - gd13 * gd13) + gdd1 * fgdd33
                                   - 2.0 * gd13 * fgdd13 + gdd3 * fgdd11 + f *
                                   (fdd1 * gdd3 + fdd3 * gdd1 - 2.0 * fd13 * gd13));
                            gd3h = f * (fd1 * fdd3 - fd3 * fd13) + gd3 * del + g * (fd1 * gdd3 - fd3 * gd13);
                            gd1h = f * (fd1 * fd13 - fd3 * fdd1) + gd1 * del + g * (fd1 * gd13 - fd3 * gdd1);
                            fd3h = f * (gd3 * fd13 - gd1 * fdd3) + fd3 * del + g * (gd3 * gd13 - gd1 * gdd3);
                            fd1h = f * (gd3 * fdd1 - gd1 * fd13) + fd1 * del + g * (gd3 * gdd1 - gd1 * gd13);
                        }
                        else
                        {
                            d3nr = -gd3 * f / del;
                            d1nr = gd1 * f / del;
                            fd1h = fd1 + hn * (fdd1 * d3nr + fd13 * d1nr);
                            fd3h = fd3 + hn * (fd13 * d3nr + fdd3 * d1nr);
                            gd1h = gd1 + hn * (gdd1 * d3nr + gd13 * d1nr);
                            gd3h = gd3 + hn * (gd13 * d3nr + gdd3 * d1nr);
                            delh = fd1h * gd3h - fd3h * gd1h;
                        }

                        d3 = -gd3h * f / delh;
                        d1 = gd1h * f / delh;
                        // pre feb 95  del/(fd1* fd3 + gd1* gd3) as dillc(used now for dillch); new axrtio
                        fgxy = fd1 * fd1 + gd1 * gd1 + fd3 * fd3 + gd3 * gd3;

                        fgxy2 = Math.Sqrt(fgxy * fgxy - 4.0 * del * del);
                        axrtio = 2.0 * Math.Abs(del) / (fgxy + Math.Sqrt(fgxy * fgxy - 4.0 * del * del));

                        bearng = Math.Atan2(-2.0 * fgdd13, fgdd33 - fgdd11) * rad / 2.0;
                        dillc = delh / (fd1h * fd3h + gd1h * gd3h);
                        if (lgmean && (Math.Abs(axrtio) < 1.0e-3 || Math.Abs(dillc) < 1.0e-3) && !lminly)
                        {
                            d3 = Math.Sign(Math.Sqrt(Math.Abs(d3nr * d3))) * d3nr;
                            d1 = Math.Sign(Math.Sqrt(Math.Abs(d1nr * d1))) * d1nr;
                        }
                        ngm = ngm + 1;
                        //if (lpr)
                        //        write(2,30) itnum
                        //30     format('g. means on iteration ', i4);

                        rng1old = rng1;
                        rng3old = rng3;

                        rng1 = rng1 + d3;
                        rng3 = rng3 + d1;

                        den = Math.Max(rng2, magr2);
                        if (alpha > 0.0)
                            den = Math.Max(den, tau12 * Math.Sqrt(alpha * (2.0 * gravConst.mu / alpha - magr2) / magr2));
                        crit = f / den;

                        if (lminly)
                        {
                            ffggd1 = f * fd1 + g * gd1;
                            ffggd3 = f * fd3 + g * gd3;
                            fg11dd = f * fdd1 + g * gdd1 + fgdd11;
                            fg13dd = f * fd13 + g * gd13 + fgdd13;
                            fg33dd = f * fdd3 + g * gdd3 + fgdd33;
                            critsq = ffggd1 * ffggd1 + ffggd3 * ffggd3;
                            // (normally enough) critsq = f * f * (fd1 ^ 2 + fd3 ^ 2)
                            crit = Math.Sqrt(critsq);
                            //*...      g terms(which not needed anyway!) dropped now
                            fgxy = fgxy + f * (fdd1 + fdd3);
                            sqar = 4.0 * (f * (f * ddel + fdd1 * fgdd33 - 2.0 * fd13 * fgdd13 + fdd3 * fgdd11) + del * del);
                            // chk if ever negative?
                            axrtio = Math.Sqrt(sqar) / (fgxy + Math.Sqrt(fgxy * fgxy - sqar));

                            bearng = Math.Atan2(-2.0 * (f * fd13 + fgdd13), f * (fdd3 - fdd1) + fgdd33 - fgdd11) * rad / 2.0;

                            if (itnum > 1)
                                crtold = crit / crtold;
                            if (crtold <= 1.0 && crtold > 0.99)
                            {
                                lmincv = true;
                                crtold = crit;
                                crit = crit / den;
                                critsq = crit * crit;
                            }
                        }
                        else
                        {
                            //  i.e. for normal runs, not min
                            crit = Math.Sqrt(f * f + g * g) / den;
                            critsq = crit * crit;

                            //if (lpr) 
                            //  write(2, 5072) itnum, crit, fc, cr, rng1, rng3,...				 
                            //                     del, delh, axrtio, bearng, dillc, f
                            //5072 format('iteration', i4, g24.14, g24.14/ 3g24.14/ 1p, 4g12.4,
                            //    & ' deg ', 2g12.4)
                            //        if (lpr1 && lpr) write(2, 5073)
                            //5073 format(4g18.9)
                            //     & fd1,fd3,gd1,gd3,fd1h,fd3h,gd1h,gd3h
                            //&   , fdd1,fd13,fdd3, gdd1,gd13,gdd3
                            //if (lpr & lminly && itnum = 1)
                            //        &write(2, 5074) ffggd1, ffggd3, fg11dd, fg13dd, fg33dd
                            //5074 format(' ffggd1 & 3  ', 2g18.9, ' & partials are'/ 3g18.9)
                        }
                    }  // Math.Abs(magqvecest) > small

                    fcold = fc;
                    nfail = -1;

                    // convergence satisfied!!        
                    if (critsq > crival || lmincv)    // 7
                    {
                        // goto 9;
                        nfail = 0;
                        // unknown next line? int conversion?
                       //alpha = alpha;
                        errstr = errstr + "critsq return alpha " + alpha.ToString() + "\n";
                        //q = e;
                        //ei = eideg;
                        //return;
                    }

                    itnum = itnum + 1;
                }  // if numsltns > 0

                // end of big 'main' loop
            }  // while itnum && nfail && critsq > crival    8

            calcps(los1, los3, rs1eci, rs2eci, rs3eci, numhalfrev, tau12, tau13, rng1, rng3, ind,  //num
                out numsoltns, out magr1, out magr3, out r1, out r3, out rho2sez, out tmpstr);

            errstr = errstr + "try  rho2sez " + rho2sez[0].ToString() + " " + rho2sez[1].ToString()
                + " " + rho2sez[2].ToString() + "\n";

            // get a final 'best' vector
            rng2 = MathTimeLibr.dot(los2, rho2sez);

            r2[0] = rho2sez[0] + rs2eci[0];
            r2[1] = rho2sez[1] + rs2eci[1];
            r2[2] = rho2sez[2] + rs2eci[2];

            errstr = errstr + tmpstr + "end return r2 " + r2[0].ToString() + " " + r2[1].ToString() + " " + r2[2].ToString() + "\n";
        }  // obs3lsx


        /* ------------------------------------------------------------------------------
        *                                   calcps
        *                                   
        *  calculate slant range vector to compare with observed line of sight. Gooding IOD
        *    to derive the components of the range vector at time t2. the estimated range vectors 
        *    are 'topocentric', being relative to the observer.
        *    dav note: Gooding goes to a lot of trouble to find radial and transverse components
        *    of the velocity. with my lambert formulations, it's much easier to
        *    a. find the lambert solution (s) of t1/t3 and dt
        *    b. take initial t1 pos and velocity on lambert solution, and propagate to middle 
        *       time with kepler
        *    c. find slant range vector
        *    
        *  author        : david vallado           davallado@gmail.com   22 jan 2021
        *
        *  inputs        : description                                          range / units
        *    los1        - line of sight vector for t1                            
        *    los3        - line of sight vector for t3                            
        *    tau12       - t2 - t1                                                s  
        *    tau13       - t3 - t1                                                s
        *    rs1         - site position vector #1 eci                            km
        *    rs2         - site position vector #2 eci                            km
        *    rs3         - site position vector #3 eci                            km
        *    rho1        - estimated range at time t1                             km
        *    rho3        - estimated range at time t3                             km
        *    hrev        - number of half-revs (k) included in input angle p1-*-p3
        *    ind         - indicator for which of two lambert solutions to use
        *                    = 0       if numHalfRev  (k) = 0 or 1          
        *                    = 0 or 1  if numHalfRev  (k) >= 2            
        *    
        *  outputs       :
        *    numsoltns   - number of solutions found by lambert valamb    (0, unlikely 1, 2)
        *    r1          -  position vector #1                                    km
        *    r2          -  position vector #2                                    km
        *    r3          -  position vector #3                                    km
        *    magr1, magr3     = computed position magnitude from earth center at times t1, t3          
        *    rho2sez     - slant range vector for t2                              km
        *    
        *  locals        :
        *  
        *  
        *  
        * ------------------------------------------------------------------------------ */

        public void calcps
        (
            double[] los1, double[] los3, double[] rs1eci, double[] rs2eci, double[] rs3eci,
            int numHalfRev, double tau12, double tau13, double rho1, double rho3, int ind,
            out int numsoltns, out double magr1, out double magr3,
            out double[] r1, out double[] r3, out double[] rho2sez, out string errstr
        )
        {
            char hitearth, dm, de;
            int nrev;
            string detailSum, detailAll;
            rho2sez = new double[] { 0.0, 0.0, 0.0 };
            // hardwire this for now, could be an input
            double altpadc = 200.0 / gravConst.re;
            errstr = "";

            // internal variables 
            //int ktrm;
            r1 = new double[] { 0.0, 0.0, 0.0 };
            r3 = new double[] { 0.0, 0.0, 0.0 };
            double[] r2 = new double[3];
            double[] v2 = new double[3];
            double[] v1 = new double[] { 0.0, 0.0, 0.0 };
            double[] v1t = new double[3];
            double[] v2t = new double[3];

            // estimated position vector at time t1(r1) using slant range ests rho1, rho3
            r1[0] = rs1eci[0] + rho1 * los1[0];
            r1[1] = rs1eci[1] + rho1 * los1[1];
            r1[2] = rs1eci[2] + rho1 * los1[2];
            magr1 = MathTimeLibr.mag(r1);

            // estimated position vector at time t3(r3)
            r3[0] = rs3eci[0] + rho3 * los3[0];
            r3[1] = rs3eci[1] + rho3 * los3[1];
            r3[2] = rs3eci[2] + rho3 * los3[2];
            magr3 = MathTimeLibr.mag(r3);

            // compute(theta) angle between r1 and r3 in range(0, pi)
            //th = MathTimeLibr.angle(r1, r3);
            //// fails only if one or other point is at force center
            //// convert angle to range[k * pi, (k + 1) * pi]
            //// done already in angle????? mod function
            //// maybe not because it could be "more" than 1 rev
            //ktrm = numHalfRev % 2;
            //if (ktrm == 1)
            //    th = Math.PI - th;
            //th = th + numHalfRev * Math.PI;

            // not needed since lambert does it all
            // specify one direction and that's it you don't need "all" the options. 
            dm = 'S';
            //dm = 'L';  // unlikely to be going the long way
            de = 'L';
            //de = 'H';  // unlikely to be high energy if nrev = 0
            
            // this seems to help in the multi-rev cases
            if (tau12 >= 86400.0 && tau13 >= 86400.0)
                nrev = 1;
            else  
                nrev = 0;

            // lambert solution to get radial and transverse velocity components at time t1
            //valamb(r1, r3, th, tau13, numsoltns, vr1, vt1, vr3, vt3, alv, wr1, wt1, wr3, wt3, alw);
            errstr = errstr + "r1 " + r1[0].ToString() + " " + r1[1].ToString() + " " + r1[2].ToString();
            errstr = errstr + "r3 " + r3[0].ToString() + " " + r3[1].ToString() + " " + r3[2].ToString()
                + tau13.ToString() + "\n";

            lambertuniv(r1, r3, v1, dm, de, nrev, 0.0, tau13, 0.0, altpadc, 'y',
                out v1t, out v2t, out hitearth, out detailSum, out detailAll);
            lambertbattin(r1, r3, v1, dm, de, nrev, 0.0, tau13, altpadc, 'y',
                out v1t, out v2t, out hitearth, out detailSum, out detailAll);

            errstr = errstr + "Lambert " + hitearth + " " + detailSum + "\n";
            // hardwire for now, numsoltns of solutions
            numsoltns = 1;

            // proceed if at least one solution exists 
            // check hitearth...
            // if (numsoltns > 0)
            if (hitearth != 'y')
            {
                // extract solution of interest 
                // if only 1 root, overwrite W one
                //if (ind == 1)
                //{
                //    vr1 = wr1;
                //    vt1 = wt1;
                //    alv = alw;
                //}

                //// obtain full velocity vector estimate r1 at time t1 
                //MathTimeLibr.cross(r1, r3, out r13);
                //MathTimeLibr.cross(r13, r1, out rt);  // and now(r1^r3)^r1
                //magrt = MathTimeLibr.mag(rt);

                //// reverse if obtuse-angle
                //// is this high/low energy or long/short way?
                //if (ktrm == 1)
                //    magrt = -magrt;                              

                //if (magrt != 0.0)
                //    magrt = 1.0 / magrt;
                //v1[0] = vr1 * r1[0] / magr1 + vt1 * rt[0] * magrt;
                //v1[1] = vr1 * r1[1] / magr1 + vt1 * rt[1] * magrt;
                //v1[2] = vr1 * r1[2] / magr1 + vt1 * rt[2] * magrt;

                // convert r1 and r1 to universal keplerian elements 
                // he uses mu/a, rp, i, raan, argp, time since perigee
                //pv3els(r1, v1t, al, q, ei, bom, om, tau);
                //rv2coe(r1, v1t, out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, 
                //    out arglat, out truelon, out lonper);

                // propagate to get estimated position vector r at time t2  
                // note "alv" is from the valamb call   
                // note he uses tau, since perigee passage, as an orbital element
                // alv gives better accuracy than al for near-parabolic orbits
                //els3pv(alv, q, ei, bom, om, tau + tau12, out r2, out v2);
                kepler(r1, v1t, tau12, out r2, out v2);

                // only for the convergence criterion in the calling routine
                // slant range vector - eci
                rho2sez[0] = r2[0] - rs2eci[0];
                rho2sez[1] = r2[1] - rs2eci[1];
                rho2sez[2] = r2[2] - rs2eci[2];
            }
            else
            {
                // need to do something if no roots were found - perhaps delete guess?
            }

        } // calcps

        /* -------------------------- conversion techniques ------------------------- */

        /* ------------------------- three vector techniques ------------------------ */

        /* -----------------------------------------------------------------------------
        *
        *                           procedure gibbs
        *
        *  this procedure performs the gibbs method of orbit determination.
        *
        *  author        : david vallado           davallado@gmail.com   22 jun 2002
        *
        *  inputs          description                          range / units
        *    r1          -  position vector #1                        km
        *    r2          -  position vector #2                        km
        *    r3          -  position vector #3                        km
        *
        *  outputs       :
        *    v2          -  velocity vector for r2                    km / s
        *    theta       - angle between vectors                         rad
        *    error       - flag indicating success                       'ok',...
        *
        *  locals        :
        *    tover2      -
        *    l           -
        *    small       - tolerance for roundoff errors
        *    r1mr2       - MathTimeLibr.magnitude of r1 - r2
        *    r3mr1       - MathTimeLibr.magnitude of r3 - r1
        *    r2mr3       - MathTimeLibr.magnitude of r2 - r3
        *    p           - p vector     r2 x r3
        *    q           - q vector     r3 x r1
        *    w           - w vector     r1 x r2
        *    d           - d vector     p + q + w
        *    n           - n vector (r1)p + (r2)q + (r3)w
        *    s           - s vector
        *                    (r2-r3)r1+(r3-r1)r2+(r1-r2)r3
        *    b           - b vector     d x r2
        *    theta1      - temp angle between the vectors                rad
        *    pn          - p unit vector
        *    r1n         - r1 unit vector
        *    dn          - d unit vector
        *    nn          - n unit vector
        *
        *  coupling      :
        *    mag         - magnitude of a vector
        *    cross       - cross product of two vectors
        *    dot         - dot product of two vectors
        *    add3vec     - add three vectors
        *    norm        - creates a unit vector
        *    angle       - angle between two vectors
        *
        *  references    :
        *    vallado       2013, 460, alg 54, ex 7-3
        -----------------------------------------------------------------------------*/

        public void gibbs
    (
        double[] r1, double[] r2, double[] r3,
        out double[] v2, out double theta, out double theta1, out double copa, out string error
    )
        {
            double small = 0.000001;
            double tover2, l, r1mr2, r3mr1, r2mr3, magr1, magr2;
            double[] p, q, w, d, n, s, b, pn, r1n, dn, nn = new double[3];
            v2 = new double[] { 0.0, 0.0, 0.0 };

            /* --------------------  initialize values   -------------------- */
            error = "ok";
            magr1 = MathTimeLibr.mag(r1);
            magr2 = MathTimeLibr.mag(r2);

            theta = 0.0;
            theta1 = 0.0;
            copa = 0.0;

            /* ----------------------------------------------------------------
            *  determine if the vectors are coplanar.
            ----------------------------------------------------------------- */
            MathTimeLibr.cross(r2, r3, out p);
            MathTimeLibr.cross(r3, r1, out q);
            MathTimeLibr.cross(r1, r2, out w);
            pn = MathTimeLibr.norm(p);
            r1n = MathTimeLibr.norm(r1);
            copa = Math.Asin(MathTimeLibr.dot(pn, r1n));

            if (Math.Abs(copa) > 0.017452406)
            {
                error = "not coplanar";
            }

            /* ---------------- or don't continue processing ---------------- */
            MathTimeLibr.addvec3(1.0, p, 1.0, q, 1.0, w, out d);
            MathTimeLibr.addvec3(magr1, p, magr2, q, MathTimeLibr.mag(r3), w, out n);
            nn = MathTimeLibr.norm(n);
            dn = MathTimeLibr.norm(d);

            /* ----------------------------------------------------------------
            *  determine if the orbit is possible.  both d and n must be in
            *    the same direction, and non-zero.
            ----------------------------------------------------------------- */
            if ((Math.Abs(MathTimeLibr.mag(d)) < small) || (Math.Abs(MathTimeLibr.mag(n)) < small) ||
                (Math.Abs(MathTimeLibr.dot(nn, dn)) < small))
            {
                error = "impossible";
            }
            else
            {
                theta = MathTimeLibr.angle(r1, r2);
                theta1 = MathTimeLibr.angle(r2, r3);

                /* ------------ perform gibbs method to find v2 ----------- */
                r1mr2 = magr1 - magr2;
                r3mr1 = MathTimeLibr.mag(r3) - magr1;
                r2mr3 = magr2 - MathTimeLibr.mag(r3);
                MathTimeLibr.addvec3(r1mr2, r3, r3mr1, r2, r2mr3, r1, out s);
                MathTimeLibr.cross(d, r2, out b);
                l = Math.Sqrt(gravConst.mu / (MathTimeLibr.mag(d) * MathTimeLibr.mag(n)));
                tover2 = l / magr2;
                MathTimeLibr.addvec(tover2, b, l, s, out v2);
            }
            /*
            if (((show == 'y') || (show == 's')) && (strcmp(error, "ok") == 0))
            if (fileout != null)
            {
            fprintf(fileout, "%16s %9.3f %9.3f %9.3f\n",
            "p vector = ", p[1], p[2], p[3]);
            fprintf(fileout, "%16s %9.3f %9.3f %9.3f\n",
            "q vector = ", q[1], q[2], q[3]);
            fprintf(fileout, "%16s %9.3f %9.3f %9.3f\n",
            "w vector = ", w[1], w[2], w[3]);
            fprintf(fileout, "%16s %9.3f %9.3f %9.3f\n",
            "n vector = ", n[1], n[2], n[3]);
            fprintf(fileout, "%16s %9.3f %9.3f %9.3f\n",
            "d vector = ", d[1], d[2], d[3]);
            fprintf(fileout, "%16s %9.3f %9.3f %9.3f\n",
            "s vector = ", s[1], s[2], s[3]);
            fprintf(fileout, "%16s %9.3f %9.3f %9.3f\n",
            "b vector = ", b[1], b[2], b[3]);
            }
            */
        }  // gibbs


        /*------------------------------------------------------------------------------
        *
        *                           procedure herrgibbs
        *
        *  this procedure implements the herrick-gibbs approximation for orbit
        *    determination, and finds the middle velocity vector for the 3 given
        *    position vectors.
        *
        *  author        : david vallado           davallado@gmail.com   22 jun 2002
        *
        *  inputs          description                             range / units
        *    r1          -  position vector #1                         km
        *    r2          -  position vector #2                         km
        *    r3          -  position vector #3                         km
        *    jd1         - julian date of 1st sighting                days from 4713 bc
        *    jd2         - julian date of 2nd sighting                days from 4713 bc
        *    jd3         - julian date of 3rd sighting                days from 4713 bc
        *
        *  outputs       :
        *    v2          -  velocity vector for r2                     km / s
        *    theta       - angle between vectors                       rad
        *    error       - flag indicating success                     'ok',...
        *
        *  locals        :
        *    dt21        - time delta between r1 and r2                s
        *    dt31        - time delta between r3 and r1                s
        *    dt32        - time delta between r3 and r2                s
        *    p           - p vector    r2 x r3
        *    pn          - p unit vector
        *    r1n         - r1 unit vector
        *    theta1      - temporary angle between vec                 rad
        *    tolangle    - tolerance angle  (1 deg)                    rad
        *    term1       - 1st term for hgibbs expansion
        *    term2       - 2nd term for hgibbs expansion
        *    term3       - 3rd term for hgibbs expansion
        *    i           - index
        *
        *  coupling      :
        *    mag         - magnitude of a vector
        *    cross       - cross product of two vectors
        *    dot         - dot product of two vectors
        *    arcsin      - arc sine function
        *    norm        - creates a unit vector
        *    angle       - angle between two vectors
        *
        *  references    :
        *    vallado       2013, 466, alg 55, ex 7-4
        -----------------------------------------------------------------------------*/

        public void herrgibbs
        (
                double[] r1, double[] r2, double[] r3, double jd1, double jd2, double jd3,
                out double[] v2, out double theta, out double theta1, out double copa, out string error
        )
        {
            double[] p, pn, r1n = new double[3];
            double dt21, dt31, dt32, term1, term2, term3, tolangle, magr1, magr2;
            v2 = new double[] { 0.0, 0.0, 0.0 };

            /* --------------------  initialize values   -------------------- */
            tolangle = 0.017452406;  // (1.0 deg in rad)

            magr1 = MathTimeLibr.mag(r1);
            magr2 = MathTimeLibr.mag(r2);

            error = "ok";

            theta = 0.0;
            theta1 = 0.0;

            dt21 = (jd2 - jd1) * 86400.0;
            dt31 = (jd3 - jd1) * 86400.0;   // differences in times
            dt32 = (jd3 - jd2) * 86400.0;

            /* ----------------------------------------------------------------
            *  determine if the vectors are coplanar.
            ---------------------------------------------------------------- */
            MathTimeLibr.cross(r2, r3, out p);
            pn = MathTimeLibr.norm(p);
            r1n = MathTimeLibr.norm(r1);
            copa = Math.Asin(MathTimeLibr.dot(pn, r1n));
            if (Math.Abs(copa) > tolangle)
            {
                error = "not coplanar";
            }

            /* ----------------------------------------------------------------
            * check the size of the angles between the three position vectors.
            *   herrick gibbs only gives "reasonable" answers when the
            *   position vectors are reasonably close.  1.0 deg is only an estimate.
            ---------------------------------------------------------------- */
            theta = MathTimeLibr.angle(r1, r2);
            theta1 = MathTimeLibr.angle(r2, r3);
            if ((theta > tolangle) || (theta1 > tolangle))
            {
                error = "angle > 1 deg";
            }

            /* ------------ perform herrick-gibbs method to find v2 --------- */
            term1 = -dt32 *
                (1.0 / (dt21 * dt31) + gravConst.mu / (12.0 * magr1 * magr1 * magr1));
            term2 = (dt32 - dt21) *
                (1.0 / (dt21 * dt32) + gravConst.mu / (12.0 * magr2 * magr2 * magr2));
            term3 = dt21 *
                (1.0 / (dt32 * dt31) + gravConst.mu / (12.0 * MathTimeLibr.mag(r3) * MathTimeLibr.mag(r3) * MathTimeLibr.mag(r3)));
            MathTimeLibr.addvec3(term1, r1, term2, r2, term3, r3, out v2);
        }  // herrgibbs
                     

        /*------------------------------------------------------------------------------
        *
        *                           procedure target
        *
        *  this procedure accomplishes the targeting problem using kepler/pkepler and
        *    lambert.
        *
        *  author        : david vallado           davallado@gmail.com   22 jun 2002
        *
        *  inputs          description                             range / units
        *    rint        - initial position vector of int             er
        *    vint        - initial velocity vector of int             er/tu
        *    rtgt        - initial position vector of tgt             er
        *    vtgt        - initial velocity vector of tgt             er/tu
        *    dm          - direction of motion                        'L','S'
        *    de          - direction of energy                        'L', 'H'
        *    kind        - type of propagator                         'k','p'
        *    dtsec        - time of flight to the int                  tu
        *
        *  outputs       :
        *    v1t         - initial transfer velocity vec              er/tu
        *    v2t         - final transfer velocity vec                er/tu
        *    dv1         - initial change velocity vec                er/tu
        *    dv2         - final change velocity vec                  er/tu
        *    error       - error flag from gauss                      'ok', ...
        *
        *  locals        :
        *    transnormal - cross product of trans orbit                er
        *    intnormal   - cross product of int orbit                 er
        *    r1tgt       - position vector after dt, tgt              er
        *    v1tgt       - velocity vector after dt, tgt              er/tu
        *    rirt        - rint[4] * r1tgt[4]
        *    cosdeltanu  - cosine of deltanu                          rad
        *    sindeltanu  - sine of deltanu                            rad
        *    deltanu     - angle between position vectors             rad
        *
        *  coupling      :
        *    kepler      - find r2 and v2 at future time
        *    lambertuniv - find velocity vectors at each end of transfer
        *
        *  references    :
        *    vallado       2013, 503, alg 61
        -----------------------------------------------------------------------------*/

        public void target
        (
            double[] rint, double[] vint, double[] rtgt, double[] vtgt,
            char dm, char de, char kind, double dtsec,
            out double[] v1t, out double[] v2t, out double[] dv1, out double[] dv2, out string error
        )
        {
            double[] v1tgt = new double[3];
            v1t = new double[] { 0.0, 0.0, 0.0 };
            v2t = new double[] { 0.0, 0.0, 0.0 };
            dv1 = new double[] { 0.0, 0.0, 0.0 };
            dv2 = new double[] { 0.0, 0.0, 0.0 };
            error = "ok";

            //    int err = fopen_s(&outfile, "D:/Codes/LIBRARY/CPP/Libraries/ast2Body/test2body.out", "w");

            /* ----------- propagate target forward in time ----------------- */
            switch (kind)
            {
                case 'k':
                    //      kepler(rtgt, vtgt, dtsec,  r1tgt, v1tgt, error);
                    break;
                case 'p':
                    //      pkepler(rtgt, vtgt, dtsec,  r1tgt, v1tgt, error);
                    break;
                default:
                    //      kepler(rtgt, vtgt, dtsec,  r1tgt, v1tgt, error);
                    break;
            }

            /* ----------- calculate transfer orbit between r2's ------------- */
 //           if (error.Equals("ok"))
            {
                //        lambertuniv(rint, r1tgt, dm, de, nrev, dtsec, tbi, altpad, out v1t, out v2t, out hitearth, out error);
//                if (error.Equals("ok"))
                {
                    MathTimeLibr.addvec(1.0, v1t, -1.0, vint, out dv1);
                    MathTimeLibr.addvec(1.0, v1tgt, -1.0, v2t, out dv2);
                }
                //else
                {
                }
            }
        }  // target



        /* -----------------------------------------------------------------------------
        *
        *                           function initjplde
        *
        *  this function initializes the jpl planetary ephemeris data. the input
        *  data files are from processing the ascii files into a text file of sun
        *  and moon positions, generated by FOR code and the JPL DE files.
        *
        *  author        : david vallado           davallado@gmail.com   22 jan 2018
        *
        *  revisions
        *
        *  inputs          description                           range / units
        *    jplLoc      - location for jpl data file  
        *    infilename  - file name
        *
        *  outputs       :
        *    jpldearr    - array of jplde data records
        *    jdjpldestart- julian date of the start of the jpldearr data
        *    jdfjpldestart- julian date fraction of the start of the jpldearr data
        *
        *  locals        :
        *                -
        *
        *  coupling      :
        *
        *  references    :
        *
        *  -------------------------------------------------------------------------- */

        public void initjplde
            (
            ref jpldedataClass[] jpldearr,
            string jplLoc,
            string infilename,
            out double jdjpldestart, out double jdfjpldestart
            )
        {
            double jdtdb, jdtdbf;
            string pattern;
            Int32 i;
            jdjpldestart = 0.0;
            jdfjpldestart = 0.0;

            // read the whole file at once into lines of an array
            string[] fileData = File.ReadAllLines(jplLoc + infilename);

            pattern = @"^\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)";
            for (i = 0; i < fileData.Count(); i++)
            {
                // set new record as they are needed
                jpldearr[i] = new jpldedataClass();

                //line = Regex.Replace(fileData[i], @"\s+", "|");
                //string[] linedata = line.Split('|');
                string[] linedata = Regex.Split(fileData[i], pattern);

                jpldearr[i].year = Convert.ToInt32(linedata[1]);
                jpldearr[i].mon = Convert.ToInt32(linedata[2]);
                jpldearr[i].day = Convert.ToInt32(linedata[3]);
                jpldearr[i].hr = Convert.ToInt32(linedata[4]);
                jpldearr[i].rsun[0] = Convert.ToDouble(linedata[5]);
                jpldearr[i].rsun[1] = Convert.ToDouble(linedata[6]);
                jpldearr[i].rsun[2] = Convert.ToDouble(linedata[7]);
                jpldearr[i].rsmag = Convert.ToDouble(linedata[8]);
                jpldearr[i].rmoon[0] = Convert.ToDouble(linedata[10]);
                jpldearr[i].rmoon[1] = Convert.ToDouble(linedata[11]);
                jpldearr[i].rmoon[2] = Convert.ToDouble(linedata[12]);

                MathTimeLibr.jday(jpldearr[i].year, jpldearr[i].mon, jpldearr[i].day, jpldearr[i].hr, 0, 0.0, out jdtdb, out jdtdbf);
                jpldearr[i].mjd = jdtdb + jdtdbf - 2400000.5;

                // ---- find epoch date
                if (i == 0)
                {
                    MathTimeLibr.jday(jpldearr[i].year, jpldearr[i].mon, jpldearr[i].day, jpldearr[i].hr, 0, 0.0, out jdjpldestart, out jdfjpldestart);
                    jpldearr[i].mjd = jdjpldestart + jdfjpldestart;
                }
            }

            jpldesize = i;
        }   //  initjplde



        /* -----------------------------------------------------------------------------
        *
        *                           function findjpldeparam
        *
        *  this routine finds the jplde parameters for a given time. several types of
        *  interpolation are available. the cio and iau76 nutation parameters are also
        *  read for optimizing the speeed of calculations.
        *
        *  author        : david vallado               davallado@gmail.com   12 dec 2005
        *
        *  inputs          description                               range / units
        *    jdtdb         - epoch julian date                     days from 4713 BC
        *    jdtdbF        - epoch julian date fraction            day fraction from jdutc
        *    interp        - interpolation                        n-none, l-linear, s-spline
        *    jpldearr      - array of jplde data records
        *    jdjpldestart  - julian date of the start of the jpldearr data (set in initjplde)
        *
        *  outputs       :
        *    dut1        - delta ut1 (ut1-utc)                        sec
        *    dat         - number of leap seconds                     sec
        *    lod         - excess length of day                       sec
        *    xp          - x component of polar motion                rad
        *    yp          - y component of polar motion                rad
        *    ddpsi       - correction to delta psi (iau-76 theory)    rad
        *    ddeps       - correction to delta eps (iau-76 theory)    rad
        *    dx          - correction to x (cio theory)               rad
        *    dy          - correction to y (cio theory)               rad
        *    x           - x component of cio                         rad
        *    y           - y component of cio                         rad
        *    s           -                                            rad
        *    deltapsi    - nutation longitude angle                   rad
        *    deltaeps    - obliquity of the ecliptic correction       rad
        *
        *  locals        :
        *                -
        *
        *  coupling      :
        *    none        -
        *
        *  references    :
        *    vallado       2013,
        * --------------------------------------------------------------------------- */

        public void findjpldeparam
            (
            double jdtdb, double jdtdbF, char interp,
            jpldedataClass[] jpldearr,
            double jdjpldestart,
            out double[] rsun, out double rsmag,
            out double[] rmoon, out double rmmag
        )
        {
            Int32 recnum;
            Int32 off1, off2;
            double fixf, jdjpldestarto, mjd, jdb, mfme;
            jpldedataClass jplderec, nextjplderec;
            rsun = new double[3];
            rmoon = new double[3];

            // the ephemerides are centered on jdtdb, but it turns out to be 0.5, or 0000 hrs.
            // check if any whole days in jdF
            jdb = Math.Floor(jdtdb + jdtdbF) + 0.5;  // want jd at 0 hr
            mfme = (jdtdb + jdtdbF - jdb) * 1440.0;
            if (mfme < 0.0)
                mfme = 1440.0 + mfme;

            //printf("jdtdb %lf  %lf  %lf  %lf \n ", jdtdb, jdtdbF, jdb, mfme);x[recnum]

            // ---- read data for day of interest
            jdjpldestarto = Math.Floor(jdtdb + jdtdbF - jdjpldestart);
            //recnum = Convert.ToInt32(jdjpldestarto);  // for 1 day centers
            recnum = Convert.ToInt32(jdjpldestarto) * 2 - 1;

            // check for out of bound values
            if ((recnum >= 1) && (recnum < jpldesize - 2))
            {
                jplderec = jpldearr[recnum];

                // ---- set non-interpolated values
                rsun[0] = jplderec.rsun[0];
                rsun[1] = jplderec.rsun[1];
                rsun[2] = jplderec.rsun[2];
                rsmag = jplderec.rsmag;
                mjd = jplderec.mjd;
                rmoon[0] = jplderec.rmoon[0];
                rmoon[1] = jplderec.rmoon[1];
                rmoon[2] = jplderec.rmoon[2];
                rmmag = jplderec.rmmag;

                // ---- find nutation parameters for use in optimizing speed

                // ---- do linear interpolation
                if (interp == 'l')
                {
                    nextjplderec = jpldearr[recnum + 1];
                    fixf = mfme / 1440.0;

                    rsun[0] = jplderec.rsun[0] + (nextjplderec.rsun[0] - jplderec.rsun[0]) * fixf;
                    rsun[1] = jplderec.rsun[1] + (nextjplderec.rsun[1] - jplderec.rsun[1]) * fixf;
                    rsun[2] = jplderec.rsun[2] + (nextjplderec.rsun[2] - jplderec.rsun[2]) * fixf;
                    rsmag = MathTimeLibr.mag(rsun);
                    rmoon[0] = jplderec.rmoon[0] + (nextjplderec.rmoon[0] - jplderec.rmoon[0]) * fixf;
                    rmoon[1] = jplderec.rmoon[1] + (nextjplderec.rmoon[1] - jplderec.rmoon[1]) * fixf;
                    rmoon[2] = jplderec.rmoon[2] + (nextjplderec.rmoon[2] - jplderec.rmoon[2]) * fixf;
                    rmmag = MathTimeLibr.mag(rmoon);
                    //printf("sunm %i rsmag %lf fixf %lf n %lf nxt %lf \n", recnum, rsmag, fixf, jplderec.rsun[0], nextjplderec.rsun[0]);
                    //printf("recnum l %i fixf %lf %lf rsun %lf %lf %lf \n", recnum, fixf, jplderec.rsun[0], rsun[0], rsuny, rsunz);
                }

                // ---- do spline interpolations
                if (interp == 's')
                {
                    off1 = 1;     // every 1 days data 
                    off2 = 2;
                    fixf = mfme / 1440.0; // get back to days for this since each step is in days
                                          // setup so the interval is in between points 2 and 3
                    rsun[0] = MathTimeLibr.cubicinterp(jpldearr[recnum - off1].rsun[0], jpldearr[recnum].rsun[0], jpldearr[recnum + off1].rsun[0],
                        jpldearr[recnum + off2].rsun[0],
                        jpldearr[recnum - off1].mjd, jpldearr[recnum].mjd, jpldearr[recnum + off1].mjd, jpldearr[recnum + off2].mjd,
                        jpldearr[recnum].mjd + fixf);
                    rsun[1] = MathTimeLibr.cubicinterp(jpldearr[recnum - off1].rsun[1], jpldearr[recnum].rsun[1], jpldearr[recnum + off1].rsun[1],
                        jpldearr[recnum + off2].rsun[1],
                        jpldearr[recnum - off1].mjd, jpldearr[recnum].mjd, jpldearr[recnum + off1].mjd, jpldearr[recnum + off2].mjd,
                        jpldearr[recnum].mjd + fixf);
                    rsun[2] = MathTimeLibr.cubicinterp(jpldearr[recnum - off1].rsun[2], jpldearr[recnum].rsun[2], jpldearr[recnum + off1].rsun[2],
                        jpldearr[recnum + off2].rsun[2],
                        jpldearr[recnum - off1].mjd, jpldearr[recnum].mjd, jpldearr[recnum + off1].mjd, jpldearr[recnum + off2].mjd,
                        jpldearr[recnum].mjd + fixf);
                    rsmag = MathTimeLibr.mag(rsun);
                    rmoon[0] = MathTimeLibr.cubicinterp(jpldearr[recnum - off1].rmoon[0], jpldearr[recnum].rmoon[0], jpldearr[recnum + off1].rmoon[0],
                        jpldearr[recnum + off2].rmoon[0],
                        jpldearr[recnum - off1].mjd, jpldearr[recnum].mjd, jpldearr[recnum + off1].mjd, jpldearr[recnum + off2].mjd,
                        jpldearr[recnum].mjd + fixf);
                    rmoon[1] = MathTimeLibr.cubicinterp(jpldearr[recnum - off1].rmoon[1], jpldearr[recnum].rmoon[1], jpldearr[recnum + off1].rmoon[1],
                        jpldearr[recnum + off2].rmoon[1],
                        jpldearr[recnum - off1].mjd, jpldearr[recnum].mjd, jpldearr[recnum + off1].mjd, jpldearr[recnum + off2].mjd,
                        jpldearr[recnum].mjd + fixf);
                    rmoon[2] = MathTimeLibr.cubicinterp(jpldearr[recnum - off1].rmoon[2], jpldearr[recnum].rmoon[2], jpldearr[recnum + off1].rmoon[2],
                        jpldearr[recnum + off2].rmoon[2],
                        jpldearr[recnum - off1].mjd, jpldearr[recnum].mjd, jpldearr[recnum + off1].mjd, jpldearr[recnum + off2].mjd,
                        jpldearr[recnum].mjd + fixf);
                    rmmag = MathTimeLibr.mag(rmoon);
                    //printf("recnum s %i mfme %lf days rsun %lf %lf %lf \n", recnum, mfme, rsunx, rsuny, rsunz);
                    //printf(" %lf %lf %lf %lf \n", jpldearr[recnum - off2].mjd, jpldearr[recnum - off1.mjd, jpldearr[recnum].mjd, jpldearr[recnum + off1].mjd);
                }
            }
            // set default values
            else
            {
                rsun[0] = 0.0;
                rsun[1] = 0.0;
                rsun[2] = 0.0;
                rsmag = 0.0;
                rmoon[0] = 0.0;
                rmoon[1] = 0.0;
                rmoon[2] = 0.0;
                rmmag = 0.0;
            }
        }  //  findjpldeparam


        /* ------------------------------------------------------------------------------
        *
        *                           function sunmoonjpl
        *
        *  this function calculates the geocentric equatorial position vector
        *    the sun given the julian date. these are the jpl de ephemerides.
        *
        *  author        : david vallado           davallado@gmail.com   27 may 2002
        *
        *  revisions
        *
        *  inputs          description                    range / units
        *    jdtdb         - epoch julian date              days from 4713 BC
        *    jdtdbF        - epoch julian date fraction     day fraction from jdutc
        *    interp        - interpolation                        n-none, l-linear, s-spline
        *    jpldearr      - array of jplde data records
        *    jdjpldestart  - julian date of the start of the jpldearr data (set in initjplde)
        *
        *  outputs       :
        *    rsun        - ijk position vector of the sun au
        *    rtasc       - right ascension                rad
        *    decl        - declination                    rad
        *
        *  locals        :
        *    meanlong    - mean longitude
        *    meananomaly - mean anomaly
        *    eclplong    - ecliptic longitude
        *    obliquity   - mean obliquity of the ecliptic
        *    tut1        - julian centuries of ut1 from
        *                  jan 1, 2000 12h
        *    ttdb        - julian centuries of tdb from
        *                  jan 1, 2000 12h
        *    hr          - hours                          0 .. 24              10
        *    min         - minutes                        0 .. 59              15
        *    sec         - seconds                        0.0  .. 59.99          30.00
        *    temp        - temporary variable
        *    deg         - degrees
        *
        *  coupling      :
        *    none.
        *
        *  references    :
        *    vallado       2013, 279, alg 29, ex 5-1
        * --------------------------------------------------------------------------- */

        public void sunmoonjpl
            (
            double jdtdb, double jdtdbF,
            char interp,
            ref jpldedataClass[] jpldearr,
            double jdjpldestart,
            out double[] rsun, out double rtascs, out double decls,
            out double[] rmoon, out double rtascm, out double declm
            )
        {
            double rsmag, rmmag, temp;
            double small = 0.000001;

            // -------------------------  implementation   -----------------
            // -------------------  initialize values   --------------------
            findjpldeparam(jdtdb, jdtdbF, interp, jpldearr, jdjpldestart, out rsun, out rsmag, out rmoon, out rmmag);

            temp = Math.Sqrt(rsun[0] * rsun[0] + rsun[1] * rsun[1]);
            if (temp < small)
                // rtascs = atan2(v[1], v[0]);
                rtascs = 0.0;
            else
                rtascs = Math.Atan2(rsun[1], rsun[0]);
            decls = Math.Asin(rsun[2] / rsmag);

            temp = Math.Sqrt(rmoon[0] * rmoon[0] + rmoon[1] * rmoon[1]);
            if (temp < small)
                // rtascm = atan2(v[1], v[0]);
                rtascm = 0.0;
            else
                rtascm = Math.Atan2(rmoon[1], rmoon[0]);
            declm = Math.Asin(rmoon[2] / rmmag);
        }  // sunmoonjpl



        /* ------------------------------------------------------------------------------
        *
        *                           function sun
        *
        *  this function calculates the geocentric equatorial position vector
        *    the sun given the julian date. Sergey K (2022) has noted that improved results 
        *    are found assuming the oputput is in a precessing frame (TEME) and converting to ICRF. 
        *    this is the low precision formula and is valid for years from 1950 to 2050.  
        *    accuaracy of apparent coordinates is about 0.01 degrees.  notice many of 
        *    the calculations are performed in degrees, and are not changed until later.  
        *    this is due to the fact that the almanac uses degrees exclusively in their formulations.
        *
        *  author        : david vallado           davallado@gmail.com   27 may 2002
        *
        *  revisions
        *    vallado     - fix mean lon of sun                            7 may 2004
        *  
        *  inputs          description                         range / units
        *    jd          - julian date  (UTC)                       days from 4713 bc
        *
        *  outputs       :
        *    rsun        - inertial position vector of the sun      au
        *    rtasc       - right ascension                          rad
        *    decl        - declination                              rad
        *
        *  locals        :
        *    meanlong    - mean longitude
        *    meananomaly - mean anomaly
        *    eclplong    - ecliptic longitude
        *    obliquity   - mean obliquity of the ecliptic
        *    tut1        - julian centuries of ut1 from
        *                  jan 1, 2000 12h
        *    ttdb        - julian centuries of tdb from
        *                  jan 1, 2000 12h
        *    hr          - hours                                    0 .. 24              10
        *    min         - minutes                                  0 .. 59              15
        *    sec         - seconds                                  0.0  .. 59.99          30.00
        *    temp        - temporary variable
        *    deg         - degrees
        *
        *  coupling      :
        *    none.
        *
        *  references    :
        *    vallado       2013, 279, alg 29, ex 5-1
        * --------------------------------------------------------------------------- */

        public void sun
                    (
                    double jd,
                    out double[] rsun, out double rtasc, out double decl
                    )
        {
            double twopi, deg2rad;
            double tut1, meanlong, ttdb, meananomaly, eclplong, obliquity, magr;

            // needed since assignments arn't at root level in procedure
            rsun = new double[] { 0.0, 0.0, 0.0 };

            twopi = 2.0 * Math.PI;
            deg2rad = Math.PI / 180.0;

            // -------------------------  implementation   -----------------
            // -------------------  initialize values   --------------------
            tut1 = (jd - 2451545.0) / 36525.0;

            meanlong = 280.460 + 36000.77 * tut1;
            meanlong = (meanlong % 360.0);  //deg

            ttdb = tut1;
            meananomaly = 357.5277233 + 35999.05034 * ttdb;
            meananomaly = ((meananomaly * deg2rad) % twopi);  //rad
            if (meananomaly < 0.0)
            {
                meananomaly = twopi + meananomaly;
            }
            eclplong = meanlong + 1.914666471 * Math.Sin(meananomaly)
                        + 0.019994643 * Math.Sin(2.0 * meananomaly); //deg
            obliquity = 23.439291 - 0.0130042 * ttdb;  //deg
            meanlong = meanlong * deg2rad;
            if (meanlong < 0.0)
            {
                meanlong = twopi + meanlong;
            }
            eclplong = eclplong * deg2rad;
            obliquity = obliquity * deg2rad;

            // --------- find magnitude of sun vector, )   components ------
            magr = 1.000140612 - 0.016708617 * Math.Cos(meananomaly)
                               - 0.000139589 * Math.Cos(2.0 * meananomaly);    // in au's

            rsun[0] = magr * Math.Cos(eclplong);
            rsun[1] = magr * Math.Cos(obliquity) * Math.Sin(eclplong);
            rsun[2] = magr * Math.Sin(obliquity) * Math.Sin(eclplong);

            rtasc = Math.Atan(Math.Cos(obliquity) * Math.Tan(eclplong));

            // --- check that rtasc is in the same quadrant as eclplong ----
            if (eclplong < 0.0)
            {
                eclplong = eclplong + twopi;    // make sure it's in 0 to 2pi range
            }
            if (Math.Abs(eclplong - rtasc) > Math.PI * 0.5)
            {
                rtasc = rtasc + 0.5 * Math.PI * Math.Round((eclplong - rtasc) / (0.5 * Math.PI));
            }
            decl = Math.Asin(Math.Sin(obliquity) * Math.Sin(eclplong));
        }  // sun


        /* -----------------------------------------------------------------------------
        *
        *                           function moon
        *
        *  this function calculates the geocentric equatorial (ijk) position vector
        *    for the moon given the julian date.
        *
        *  author        : david vallado           davallado@gmail.com   27 may 2002
        *
        *  revisions
        *                -
        *
        *  inputs          description                             range / units
        *    jd          - julian date                              days from 4713 bc
        *
        *  outputs       :
        *    rmoon       - ijk position vector of moon              km
        *    rtasc       - right ascension                          rad
        *    decl        - declination                              rad
        *
        *  locals        :
        *    eclplong    - ecliptic longitude
        *    eclplat     - eclpitic latitude
        *    hzparal     - horizontal parallax
        *    l           - geocentric direction Math.Cosines
        *    m           -             "     "
        *    n           -             "     "
        *    ttdb        - julian centuries of tdb from
        *                  jan 1, 2000 12h
        *    hr          - hours                                    0 .. 24
        *    min         - minutes                                  0 .. 59
        *    sec         - seconds                                  0.0  .. 59.99
        *    deg         - degrees
        *
        *  coupling      :
        *    none.
        *
        *  references    :
        *    vallado       2013, 288, alg 31, ex 5-3
        * --------------------------------------------------------------------------- */

        public void moon
            (
            double jd,
            out double[] rmoon, out double rtasc, out double decl
            )
        {
            double twopi, deg2rad, magr;
            double ttdb, l, m, n, eclplong, eclplat, hzparal, obliquity;
            // needed since assignments arn't at root level in procedure
            rmoon = new double[] { 0.0, 0.0, 0.0 };

            twopi = 2.0 * Math.PI;
            deg2rad = Math.PI / 180.0;

            // -------------------------  implementation   -----------------
            ttdb = (jd - 2451545.0) / 36525.0;

            eclplong = 218.32 + 481267.8813 * ttdb
                        + 6.29 * Math.Sin((134.9 + 477198.85 * ttdb) * deg2rad)
                        - 1.27 * Math.Sin((259.2 - 413335.38 * ttdb) * deg2rad)
                        + 0.66 * Math.Sin((235.7 + 890534.23 * ttdb) * deg2rad)
                        + 0.21 * Math.Sin((269.9 + 954397.70 * ttdb) * deg2rad)
                        - 0.19 * Math.Sin((357.5 + 35999.05 * ttdb) * deg2rad)
                        - 0.11 * Math.Sin((186.6 + 966404.05 * ttdb) * deg2rad);      // deg

            eclplat = 5.13 * Math.Sin((93.3 + 483202.03 * ttdb) * deg2rad)
                        + 0.28 * Math.Sin((228.2 + 960400.87 * ttdb) * deg2rad)
                        - 0.28 * Math.Sin((318.3 + 6003.18 * ttdb) * deg2rad)
                        - 0.17 * Math.Sin((217.6 - 407332.20 * ttdb) * deg2rad);      // deg

            hzparal = 0.9508 + 0.0518 * Math.Cos((134.9 + 477198.85 * ttdb)
                       * deg2rad)
                      + 0.0095 * Math.Cos((259.2 - 413335.38 * ttdb) * deg2rad)
                      + 0.0078 * Math.Cos((235.7 + 890534.23 * ttdb) * deg2rad)
                      + 0.0028 * Math.Cos((269.9 + 954397.70 * ttdb) * deg2rad);    // deg

            eclplong = ((eclplong * deg2rad) % twopi);
            eclplat = ((eclplat * deg2rad) % twopi);
            hzparal = ((hzparal * deg2rad) % twopi);

            obliquity = 23.439291 - 0.0130042 * ttdb;  //deg
            obliquity = obliquity * deg2rad;

            // ------------ find the geocentric direction Math.Cosines ----------
            l = Math.Cos(eclplat) * Math.Cos(eclplong);
            m = Math.Cos(obliquity) * Math.Cos(eclplat) * Math.Sin(eclplong) - Math.Sin(obliquity) * Math.Sin(eclplat);
            n = Math.Sin(obliquity) * Math.Cos(eclplat) * Math.Sin(eclplong) + Math.Cos(obliquity) * Math.Sin(eclplat);

            // ------------- calculate moon position vector ----------------
            magr = 1.0 / Math.Sin(hzparal);
            rmoon[0] = magr * l;
            rmoon[1] = magr * m;
            rmoon[2] = magr * n;

            // -------------- find rt ascension and declination ------------
            rtasc = Math.Atan2(m, l);
            decl = Math.Asin(n);
        }  // moon



        /* ----------------------------------------------------------------------------
        *
        *                           function InitGravityField
        *
        *   this function reads and stores the gravity field coefficients. the routine can be 
        *      configured for either normalized or unnormalized values. note that in practice,
        *      the factorial can only return results to n = 170 due to precision limits. arrays 
        *      are dimensioned from 0 since L,m go from 0.
        *      
        *  author        : david vallado             davallado@gmail.com   9 oct 2019
        *
        *  inputs        description                                     range / units
        *    fname       - filename for gravity field            
        *    normal      - normalized coeff in file                        'y', 'n'
        *
        *  outputs       :
        *    order       - Size of gravity field                           1..2160..
        *    gravData.c  - gravitational coefficients (in gravityModelData)
        *    gravData.s  - gravitational coefficients (in gravityModelData)
        *    unnormArr   - normalization values for unnormalized formulations (precomputed)
        *                  uses factorials so it only is fgood to about 170
        *    normArr     - normalization values for normalized formulations (precomputed)
        *                  should be good to 1000, 2160, etc.
        *
        *  locals :
        *    L, m        - degree and order indices
        *    ktr         - starting line of data in each gravity file
        *    conv        - conversion to un-normalize
        *
        *  coupling      :
        *   none
        *
        *  references :
        *    vallado       2013, 597
        * ----------------------------------------------------------------------------*/

        public void initGravityField
            (
                string fname,
                Int32 startKtr,
                char normal,
                out Int32 order,
                out gravityConst gravData,
                out double[,] unnormArr,
                out double[,,] normArr
            )
        {
            int L, m, ktr, begincol;
            // 150 is arbitrary, but sufficiently large for satellite applications
            Int32 maxsizeGravField = 500;
            string line, line1;
            double conv;
            normArr = new double[maxsizeGravField + 2, maxsizeGravField + 2, 8];
            L = 0;

            unnormArr = new double[maxsizeGravField + 2, maxsizeGravField + 2];
            unnormArr[0, 0] = 1.0;
            unnormArr[1, 0] = Math.Sqrt(3);
            unnormArr[1, 1] = unnormArr[1, 0];

            gravData = new gravityConst();

            string[] fileData = File.ReadAllLines(fname);

            ktr = 0;
            begincol = 0;

            // start past the header info
            ktr = startKtr;
            if (startKtr == 0 || startKtr == 17)  // for large egm-08 and gem10b
                begincol = 1;
            else
                begincol = 0;

            while (ktr < fileData.Count() && !fileData[ktr].Contains("END"))
            {
                line = fileData[ktr];
                // remove additional spaces
                line1 = Regex.Replace(line, @"\s+", " ");
                line1 = line1.Replace('d', 'e');
                line1 = line1.Replace('D', 'e');
                string[] linesplt = line1.Split(' ');

                L = Convert.ToInt32(linesplt[begincol]);
                m = Convert.ToInt32(linesplt[begincol + 1]);
                if (normal == 'y')
                {
                    gravData.cNor[L, m] = Convert.ToDouble(linesplt[begincol + 2]);
                    gravData.sNor[L, m] = Convert.ToDouble(linesplt[begincol + 3]);
                }
                else
                {
                    gravData.c[L, m] = Convert.ToDouble(linesplt[begincol + 2]);
                    gravData.s[L, m] = Convert.ToDouble(linesplt[begincol + 3]);
                }
                // some models don't have sigmas
                try
                {
                    gravData.cSig[L, m] = Convert.ToDouble(linesplt[begincol + 4]);
                    gravData.sSig[L, m] = Convert.ToDouble(linesplt[begincol + 5]);
                }
                catch
                { }

                // find unnormalized normalization constant depending on which is already in file
                // note that above n = 170, the factorial will return 0, thus affecting the results!!!!
                if (L <= 170 && m <= 170)
                {
                    if (m == 0)
                        conv = Math.Sqrt(MathTimeLibr.factorial(L) * (2 * L + 1) / MathTimeLibr.factorial(L));
                    else
                        conv = Math.Sqrt(MathTimeLibr.factorial(L - m) * 2 * (2 * L + 1) / MathTimeLibr.factorial(L + m));

                    // ----- store these for later use
                    unnormArr[L, m] = conv;

                    if (normal == 'y')  // data in file was already normalized
                    {
                        gravData.c[L, m] = conv * gravData.cNor[L, m];
                        gravData.s[L, m] = conv * gravData.sNor[L, m];
                    }
                    else
                    {
                        double temp = 1.0 / conv;
                        gravData.cNor[L, m] = temp * gravData.c[L, m];
                        gravData.sNor[L, m] = temp * gravData.s[L, m];
                    }

                }

                ktr = ktr + 1;
            }

            // field size
            if (L > maxsizeGravField)
                order = maxsizeGravField - 1;
            else
                order = L;

            // note these normalization recursions should work for higher than 150 since factorials are "not" used. 
            // calculate normalization constants and store for later for Gottlieb approach
            // note that these normalizations are self sufficient - that is, index 5 does "not" depend on values from index 3, etc
            // constants for normalized diagonal (L,L) terms (index 4)
            for (L = 1; L <= order + 2; L++)
            {
                // constants for Gottlieb fully normalized (L,L) terms (index 4)
                normArr[L, L, 4] = Math.Sqrt((2.0 * L + 1) / (2.0 * L));
                // constants for fully normalized (L,0) terms (index 5, Gottlieb calls it delta)
                // normArr[L, L - 1, 5] = Math.Sqrt(2.0 * L + 1);
                if (L - 1 == 0)
                    normArr[L, L - 1, 5] = Math.Sqrt((L + 1) * L * 0.5);
                else
                    normArr[L, L - 1, 5] = Math.Sqrt(2.0 * L);

                normArr[L, L - 1, 7] = Math.Sqrt(2.0 * L + 1);  // old approach alt ver for Fukushima

                // constants for fully normalized (L,0) terms (index 2 and 3, Gottlieb calls them alpha and beta)
                // these do not need recursive values, 00 and 11 do not exist
                normArr[L, 0, 2] = Math.Sqrt((2.0 * L + 1) * (2.0 * L - 1)) / L;
                normArr[L, 0, 3] = (L - 1.0) / L * Math.Sqrt((2.0 * L + 1) / (2.0 * L - 3));
            }

            // constants for fully normalized general (L,m) terms (index 0 and 1, Gottlieb calls eta and zeta)
            normArr[0, 0, 0] = 1.0;
            normArr[1, 1, 0] = Math.Sqrt(3.0);  // no sine because we're not doing the latgc yet. just the coefficients
            for (L = 2; L <= order + 2; L++)
            {
                for (m = 0; m < L; m++)
                {
                    normArr[L, m, 0] = Math.Sqrt(((2.0 * L + 1) * (2.0 * L - 1)) / ((L + m) * (L - m)));
                    if (m > 0)
                    {
                        normArr[L, m, 1] = Math.Sqrt(((2.0 * L + 1) * (L + m - 1) * (L - m - 1)) / ((L + m) * (L - m) * (2.0 * L - 3)));
                        normArr[L, m, 6] = Math.Sqrt((L - m) * (L + m + 1));  // m >= 1
                        // simpler recursive approach for these (blm)
                        //  normArr[L, m, 1] = normArr[L, m, 0] / normArr[L, m - 1, 0];
                    }
                    else
                    {
                        normArr[L, 0, 1] = Math.Sqrt((2.0 * L + 1) * (L - 1) * (L - 1) / (L * L * (2.0 * L - 3)));
                        normArr[L, 0, 6] = Math.Sqrt(L * (L + 1) / 2.0);  // m = 0
                    }

                }
            }

        } // initGravityField


        /* ----------------------------------------------------------------------------
         *LEGFGL          93 / 03 / 23            0000.0    PGMR: FGL
         *
         * FUNCTION:  COMPUTES LEGENDRE AND ASSOCIATED LEGENDRE FUNCTIONS UP TO
         *DEGREE N AND ORDER M, N.GE.M
         *
         * I/O PARAMETERS:
         *NAME I/O A/S DESCRIPTION OF PARAMETERS
         * ------------------------------------------------------------
         * N   IS DEGREE OF SPHERICAL HARMONICS
         * M   IS    ORDER OF SPHERICAL HARMONICS
         * X   IS Math.Sin(LATITUDE)
         * POA    2-D LEGENDRE FUNCTIONS
         *
         * REFERENCES
         * JPL EM 312 / 87 - 153, 20 APRIL 1987
         ---------------------------------------------------------------------------- */
        public void geodynlegp
            (
               double latgc,
               Int32 degree,
               Int32 order,
               out double[,] LegArrOU,
               out double[,] LegArrON
            )
        {
            int i, j;
            double y, conv;
            double x = Math.Sin(latgc);
            LegArrOU = new double[degree + 2, order + 2];
            LegArrON = new double[degree + 2, order + 2];

            // zero all out
            for (i = 1; i <= degree + 1; i++)
            {
                for (j = 1; j <= order + 1; j++)
                    LegArrOU[i, j] = 0.0;
            }

            // recursions
            LegArrOU[1, 1] = 1.0;
            LegArrOU[2, 1] = x;

            // COMPUTE LEGENDRE FUNCTIONS UP TO DEGREE 
            for (i = 2; i <= degree; i++)
                LegArrOU[i + 1, 1] = (Convert.ToDouble(2 * i - 1) * x * LegArrOU[i, 1] - (i - 1) * LegArrOU[i - 1, 1]) / Convert.ToDouble(i);
            if (order == 0)
                return;  // just do zonals

            // COMPUTE ASSOCIATED LEGENDRE FUNCTIONS UP TO ORDER 
            y = Math.Sqrt(1.0 - x * x);
            LegArrOU[2, 2] = y;
            if (order != 1)
            {
                // THIS IS THE SECTORIAL PART OF THE ASSOCIATED FUNCTIONS
                for (i = 2; i <= order; i++)
                {
                    LegArrOU[i + 1, i + 1] = (2 * i - 1) * y * LegArrOU[i, i];
                }
            }
            // THIS THE TESSERAL PART OF THE ASSOCIATED FUNCTIONS
            for (i = 2; i <= degree; i++)
            {
                // int i1 = i - 1;
                for (j = 1; j <= i - 1; j++)
                {
                    if (j <= order)
                    {
                        LegArrOU[i + 1, j + 1] = Convert.ToDouble(2 * i - 1) * y * LegArrOU[i, j];
                        if (i - 2 >= j)
                            LegArrOU[i + 1, j + 1] = LegArrOU[i + 1, j + 1] + LegArrOU[i - 1, j + 1];
                    }
                }
            }

            // normalize after the polynomials are found because they are intertwined in the recursion
            // indicies are +1
            for (i = 3; i <= order; i++)
            {
                for (j = 1; j <= i; j++)
                {
                    // find normalized or unnormalized depending on which is already in file
                    // note that above n = 170, the factorial will return 0, thus affecting the results!!!!
                    if (j == 1)
                        conv = Math.Sqrt((MathTimeLibr.factorial(i - j) * (2 * i + 1)) / MathTimeLibr.factorial(i + j));
                    else
                        conv = Math.Sqrt((MathTimeLibr.factorial(i - j) * 2 * (2 * i + 1)) / MathTimeLibr.factorial(i + j));

                    LegArrON[i, j] = conv * LegArrOU[i, j];
                } // for m

            } // for L
        }  // geodynlegp



        /* ----------------------------------------------------------------------------
        *
        *                           function LegPolyG
        *
        *   this function finds the unnormalized Legendre polynomials for the gravity field.             
        *   the arrays are indexed from 0 to coincide with the usual nomenclature (eq 8-21 
        *   in my text). fortran and matlab implementations will have indicies of 1 greater  
        *   as they start at 1. note that some recursions at high degree tesseral terms 
        *   experience error for resonant orbits. these are valid for normalized and 
        *   unnormalized expressions, as long as the remaining equations are consistent. 
        *   for satellite operations, orders up to about 120 are valid. this routine uses 
        *   the GTDS normalization formulation.
        *      
        *  author        : david vallado             davallado@gmail.com  10 oct 2019
        *
        *  inputs        description                                   range / units
        *    latgc       - geocentric lat of satellite, not nadir point  pi to pi rad          
        *    order       - size of gravity field                         1..~170
        *    unnormArr   - normalization values for unnormalized formulations (precomputed)
        *    normArr     - normalization values for normalized formulations (precomputed)
        *
        *  outputs       :
        *    LegArrGU     - array of un normalized Legendre polynomials, gtds
        *    LegArrGN    - array of normalized Legendre polynomials, gtds
        *
        *  locals :
        *    L,m         - degree and order indices
        *
        *  coupling      :
        *   none
        *
        *  references :
        *    vallado       2013, 597, Eq 8-57
          ----------------------------------------------------------------------------*/

        public void LegPolyG
           (
               double latgc,
               Int32 order,
               char normalized,
               double[,] unnormArr,
               double[,,] normArr,  // needed only for Fukushima approach
               out double[,] legarrGU,
               out double[,] legarrGN
           )
        {
            Int32 L, m;
            double conv;
            legarrGU = new double[order + 2, order + 2];
            legarrGN = new double[order + 2, order + 2];

            legarrGU[0, 0] = 1.0;
            legarrGU[0, 1] = 0.0;
            legarrGU[1, 0] = Math.Sin(latgc);
            legarrGU[1, 1] = Math.Cos(latgc);

            // -------------------- perform recursions ---------------------- }
            for (L = 2; L <= order; L++)
            {
                for (m = 0; m <= L; m++)
                {
                    // Legendre functions, zonal gtds
                    if (m == 0)
                        legarrGU[L, 0] = ((2 * L - 1) * legarrGU[1, 0] * legarrGU[L - 1, 0] - (L - 1) * legarrGU[L - 2, 0]) / L;
                    else
                    {
                        // associated Legendre functions
                        if (m == L)
                            legarrGU[L, L] = (2 * L - 1) * legarrGU[1, 1] * legarrGU[L - 1, m - 1];  // sectoral part
                        else
                            legarrGU[L, m] = legarrGU[L - 2, m] + (2 * L - 1) * legarrGU[1, 1] * legarrGU[L - 1, m - 1];  // tesseral part
                    }
                }   // for m
            }   // for L

            // Legendre functions, zonal gtds
            // alt approach
            //for (L = 2; L <= order; L++)
            //{
            //    legarrGU[L, 0] = ((2 * L - 1) * legarrGU[1, 0] * legarrGU[L - 1, 0] - (L - 1) * legarrGU[L - 2, 0]) / L;
            //}
            // diagonal terms
            //for (L = 2; L <= order; L++)
            //{
            //    legarrGU[L, L] = (2 * L - 1) * legarrGU[1, 1] * legarrGU[L - 1, L - 1];  // sectoral part
            //}
            // tesseral terms
            //for (L = 2; L <= order; L++)
            //{
            //    for (m = 1; m <= L - 1; m++)
            //    {
            //        legarrGU[L, m] = (2 * L - 1) * legarrGU[1, 1] * legarrGU[L - 1, m - 1];
            //        if (L - 2 >= m)
            //            legarrGU[L, m] = legarrGU[L, m] + legarrGU[L - 2, m];  // tesseral part
            //    }
            //}

            // my simple approach to normalize
            // normalize after the polynomials are found because they are intertwined in the recursion
            Int32 orderlim = order;
            if (normalized == 'y')
            {
                if (order > 170)
                    orderlim = 170;
                else
                {
                    // can't do it...
                }
                legarrGN[0, 0] = 1.0;
                legarrGN[0, 1] = 0.0;
                for (L = 1; L <= orderlim; L++)
                {
                    for (m = 0; m <= L; m++)
                    {
                        // note that above n = 170, the factorial will return 0, thus affecting the results!!!!
                        conv = unnormArr[L, m];
                        legarrGN[L, m] = conv * legarrGU[L, m];
                    } // for m
                } // for L
            }

        } // LegPolyG


        /* ----------------------------------------------------------------------------
        *
        *                           function LegPolyGot
        *
        *   this function finds the Legendre polynomials for the gravity field, unnormalized 
        *   and normalized. Uses Gottlieb approach.          
        *   the arrays are indexed from 0 to coincide with the usual nomenclature (eq 8-21 
        *   in my text). fortran and matlab implementations will have indicies of 1 greater  
        *   as they start at 1. note that some recursions at high degree tesseral terms 
        *   experience error for resonant orbits. these are valid for normalized and 
        *   unnormalized expressions, as long as the remaining equations are consistent. 
        *   for satellite operations, orders up to about 120 are valid. 
        *      
        *  author        : david vallado             davallado@gmail.com  10 oct 2019
        *
        *  inputs        description                                   range / units
        *    latgc       - geocentric lat of satellite, not nadir point  pi to pi rad          
        *    order       - size of gravity field                         1..~170
        *    unnormArr   - normalization values for unnormalized formulations (precomputed)
        *    normArr     - normalization values for normalized formulations (precomputed)
        *
        *  outputs       :
        *    LegArrGU    - array of un normalized Legendre polynomials, gtds
        *    LegArrGN    - array of normalized Legendre polynomials, gtds
        *
        *  locals :
        *    L,m         - degree and order indices
        *
        *  coupling      :
        *   none
        *
        *  references :
        *    vallado       2013, 597, Eq 8-57
          ----------------------------------------------------------------------------*/

        public void LegPolyGot
           (
               double latgc,
               Int32 order,
               char normalized,
               double[,] unnormArr,
               double[,,] normArr,  // needed for constants
               out double[,] legarrGotU,
               out double[,] legarrGotN
           )
        {
            Int32 L, m;
            legarrGotU = new double[order + 2, order + 2];
            legarrGotN = new double[order + 2, order + 2];

            legarrGotU[0, 0] = 1.0;
            legarrGotU[0, 1] = 0.0;
            legarrGotU[1, 0] = Math.Sin(latgc);
            legarrGotU[1, 1] = Math.Cos(latgc);
            double slat = legarrGotU[1, 0];
            double clat = legarrGotU[1, 1];

            // -------------------- perform recursions ---------------------- }
            for (L = 2; L <= order; L++)
            {
                for (m = 0; m <= L; m++)
                {
                    //  process across each row 
                    if (m == 0)
                        legarrGotU[L, 0] = ((2 * L - 1) * slat * legarrGotU[L - 1, 0] - (L - 1) * legarrGotU[L - 2, 0]) / L;
                    else if (m == L - 1)
                        legarrGotU[L, L - 1] = (2 * L - 1) * slat * legarrGotU[L - 1, L - 1];   // inner diagonal
                    else if (m == L)
                        legarrGotU[L, L] = (2 * L - 1) * clat * legarrGotU[L - 1, L - 1];  // diagonal
                    else
                        legarrGotU[L, m] = ((2 * L - 1) * slat * legarrGotU[L - 1, m] - (L + m - 1) * legarrGotU[L - 2, m]) / (L - m);  // tesseral part
                }   // for m
            }   // for L

            // Gottlieb approach does normalization directly
            // note initial conditions are different!!
            legarrGotN[0, 0] = 1.0;
            legarrGotN[0, 1] = 0.0;
            legarrGotN[1, 0] = normArr[1, 1, 0] * Math.Sin(latgc);
            legarrGotN[1, 1] = normArr[1, 1, 0] * Math.Cos(latgc); // sqrt3
            //slat = Math.Sin(latgc);
            //clat = Math.Cos(latgc);

            for (L = 2; L <= order; L++)
            {
                for (m = 0; m <= L; m++)
                {
                    //  process across each row 
                    if (m == 0)
                        legarrGotN[L, 0] = normArr[L, 0, 2] * slat * legarrGotN[L - 1, 0] - normArr[L, 0, 3] * legarrGotN[L - 2, 0];
                    else if (m == L - 1)
                        legarrGotN[L, L - 1] = normArr[L, L - 1, 7] * slat * legarrGotN[L - 1, L - 1];   // inner diagonal
                    else if (m == L)
                        legarrGotN[L, L] = normArr[L, L, 4] * clat * legarrGotN[L - 1, L - 1];  // diagonal
                    else
                        legarrGotN[L, m] = normArr[L, m, 0] * slat * legarrGotN[L - 1, m] - normArr[L, m, 1] * legarrGotN[L - 2, m];  // tesseral part
                }   // for m

            }   // for L

        } // LegPolyGot



        /* ----------------------------------------------------------------------------
         *
         *                           function LegPolyM
         *
         *   this function finds the unnormalized and normalized Legendre polynomials for the 
         *   gravity field. the arrays are indexed from 0 to coincide with the usual nomenclature 
         *   (eq 8-21 in my text). fortran and matlab implementations will have indicies of 1 
         *   greater as they start at 1. note that some recursions at high degree tesseral terms 
         *   experience error for resonant orbits. these are valid for normalized and 
         *   unnormalized expressions. for satellite operations, orders up to about 120 are valid. 
         *   this is the montenbruck formulation. 
         *      
         *  author        : david vallado             davallado@gmail.com  10 oct 2019
         *
         *  inputs        description                                   range / units
         *    latgc       - geocentric lat of satellite, not nadir point  pi to pi rad          
         *    order       - size of gravity field                         1..~170
         *    unnormArr   - array of normalization values                  
         *
         *  outputs       :
         *    LegArrMU     - array of unnormalized Legendre polynomials, montenbruck
         *    LegArrMN     - array of normalized Legendre polynomials, montenbruck
         *
         *  locals :
         *    L,m         - degree and order indices
         *
         *  coupling      :
         *   none
         *
         *  references :
         *    vallado       2013, 597, Eq 8-57
           ----------------------------------------------------------------------------*/

        public void LegPolyM
           (
               double latgc,
               Int32 order,
               char normalized,
               double[,] unnormArr,
               out double[,] legarrMU,
               out double[,] legarrMN
           )
        {
            Int32 L, m;
            double conv;
            legarrMU = new double[order + 2, order + 2];
            legarrMN = new double[order + 2, order + 2];

            // --------------------  montenbruck approach
            legarrMU[0, 0] = 1.0;
            legarrMU[0, 1] = 0.0;
            legarrMU[1, 0] = Math.Sin(latgc);
            legarrMU[1, 1] = Math.Cos(latgc);
            double slat = legarrMU[1, 0];
            double clat = legarrMU[1, 1];

            // Legendre functions, zonal gtds
            for (L = 2; L <= order; L++)
            {
                legarrMU[L, L] = (2 * L - 1) * clat * legarrMU[L - 1, L - 1];
            }

            // associated Legendre functions
            for (L = 2; L <= order; L++)
            {
                for (m = 0; m < L; m++)
                {
                    if (L == m + 1)
                        legarrMU[L, m] = (2 * m + 1) * slat * legarrMU[m, m];
                    else
                        legarrMU[L, m] = 1.0 / (L - m) * ((2 * L - 1) * slat * legarrMU[L - 1, m] - (L + m - 1) * legarrMU[L - 2, m]);
                }
            }

            // my simple approach to normalization
            // normalize after the polynomials are found because they are intertwined in the recursion
            if (normalized == 'y')
            {
                legarrMN[0, 0] = 1.0;
                legarrMN[0, 1] = 0.0;
                for (L = 1; L <= order; L++)
                {
                    for (m = 0; m <= L; m++)
                    {
                        // note that above n = 170, the factorial will return 0, thus affecting the results!!!!
                        conv = unnormArr[L, m];
                        legarrMN[L, m] = conv * legarrMU[L, m];
                    } // for m
                } // for L
            }
        } // LegPolyM


        /* ----------------------------------------------------------------------------
        *                                   alfsx
        *
        *  return a one-dimensional array for the diagonal elements (Pl,l) (sectoral) of 
        *  the fully normalized ALFs for a given order in X-numbers. it is needed before 
        *  a call to alfmx. results in a 1d array for speed. 
        *  
        *    coslatgc - Math.Sin(latgc)
        *    mx       - order
        *    normArr  - precomputed alm, blm, dm conversions
        *               dm = sqrt((2L + 1) / (2L))
        *    ps       - Legendre function 1d array x, xnumber
        *    ips      - Legendre function 1d array ix, xnumber
        *    
        *    Fukushima 2012a table 9
        ------------------------------------------------------------------------------ */

        public void alfsx
            (
            double coslatgc,
            Int32 mx,
            double[,,] normArr,
            out double[] ps,
            out Int32[] ips
            )
        {
            ps = new double[mx + 2];
            ips = new Int32[mx + 2];

            Int32 IND = 960;
            double x, y;
            Int32 m, ix;
            double BIG = Math.Pow(2, IND);
            double BIGI = Math.Pow(2, -IND);
            double BIGS = Math.Pow(2, IND / 2);
            double BIGSI = Math.Pow(2, -IND / 2);
            double ROOT3 = 1.7320508075688773;

            // initialize
            ps[0] = 1.0;

            x = ROOT3 * coslatgc;
            ix = 0;
            ps[1] = x;
            ips[1] = ix;
            for (m = 2; m <= mx; m++)
            {
                x = (normArr[m, m, 4] * coslatgc) * x;
                y = Math.Abs(x);
                if (y >= BIGS)
                {
                    x = x * BIGI;
                    ix = ix + 1;
                }
                else if (y < BIGSI)
                {
                    x = x * BIG;
                    ix = ix - 1;
                }
                ps[m] = x;
                ips[m] = ix;
            }
        }  // alfsx



        /* ----------------------------------------------------------------------------
        *                                   alfmx
        *
        *  return a one-dimensional array (column) of the fully normalized ALFs for a 
        *  given order (m) using X-numbers. the psm ipsm are initial single diagonal (sectoral) 
        *  values. Returns a 1d array for speed. 
        *  
        *    t      - Math.Sin(latgc)
        *    m      - order, the col you want to find
        *    nx     - max order
        *    normArr  - precomputed alm, blm conversions
        *    psm    - Legendre function x, xnumber of order (col) m diagonal term
        *    ipsm   - Legendre function ix, xnumber of order (col) m diagonal term
        *    
        *    pm     - normalized Legendre function array
        *    
        *    Fukushima 2012a table 10
         ---------------------------------------------------------------------------- */

        public void alfmx
            (
            double sinlatgc,
            Int32 m,
            Int32 nx,
            double[,,] normArr,
            double psm,
            Int32 ipsm,
            //out double[] pm
            ref double[,] pm
            )
        {
            //pm = new double[NX];
            //pm = new double[NX+2, NX+2];
            Int32 IND = 960;
            double x, y, w, z;
            Int32 id, ix, iy, iz;
            double BIG = Math.Pow(2, IND);
            double BIGI = Math.Pow(2, -IND);
            double BIGS = Math.Pow(2, IND / 2);
            double BIGSI = Math.Pow(2, -IND / 2);

            // check to see if any variables are larger than the radix cutoff
            // input diagonal term from col of interest for ex 3, 3
            x = psm;
            ix = ipsm;
            if (ix == 0)
                pm[m, m] = x;
            else if (ix < 0)
                pm[m, m] = x * BIGI;
            else
                pm[m, m] = x * BIG;

            //if (m >= nx) return
            // now calculate the inner diagonal term for ex 4, 3
            y = x;
            iy = ix;
            x = (normArr[m + 1, m, 7] * sinlatgc) * y;
            ix = iy;
            w = Math.Abs(x);
            if (w >= BIGS)
            {
                x = x * BIGI;
                ix = ix + 1;
            }
            else if (w < BIGSI)
            {
                x = x * BIG;
                ix = ix - 1;
            }

            if (ix == 0)
                pm[m + 1, m] = x;
            else if (ix < 0)
                pm[m + 1, m] = x * BIGI;
            else
                pm[m + 1, m] = x * BIG;

            // now remaining terms for ex 5, 3 and 6, 3
            for (int L = m + 2; L <= nx; L++)
            {
                id = ix - iy;
                if (id == 0)
                {
                    z = (normArr[L, m, 0] * sinlatgc) * x - normArr[L, m, 1] * y;
                    iz = ix;
                }
                else if (id == 1)
                {
                    z = (normArr[L, m, 0] * sinlatgc) * x - normArr[L, m, 1] * (y * BIGI);
                    iz = ix;
                }
                else if (id == -1)
                {
                    z = (normArr[L, m, 0] * sinlatgc) * (x * BIGI) - normArr[L, m, 1] * y;
                    iz = iy;
                }
                else if (id > 1)
                {
                    z = (normArr[L, m, 0] * sinlatgc) * x;
                    iz = ix;
                }
                else
                {
                    z = -normArr[L, m, 1] * y;
                    iz = iy;
                }

                w = Math.Abs(z);
                if (w >= BIGS)
                {
                    z = z * BIGI;
                    iz = iz + 1;
                }
                else if (w < BIGSI)
                {
                    z = z * BIG;
                    iz = iz - 1;
                }

                if (iz == 0)
                    pm[L, m] = z;
                else if (iz < 0)
                    pm[L, m] = z * BIGI;
                else
                    pm[L, m] = z * BIG;

                y = x;
                iy = ix;
                x = z;
                ix = iz;
            }  // for 

        }  // alfmx


        /* ----------------------------------------------------------------------------
         *
         *                           function LegPolyF
         *
         *   this function finds the unnormalized and normalized Legendre polynomials for the 
         *   gravity field. the arrays are indexed from 0 to coincide with the usual nomenclature 
         *   (eq 8-21 in my text). fortran and matlab implementations will have indicies of 1 
         *   greater as they start at 1. note that some recursions at high degree tesseral terms 
         *   experience error for resonant orbits. these are valid for normalized and 
         *   unnormalized expressions. for satellite operations, orders up to about 120 are valid. 
         *   this is the fukushima formulation. 
         *      
         *  author        : david vallado             davallado@gmail.com  10 oct 2019
         *
         *  inputs        description                                   range / units
         *    latgc       - geocentric lat of satellite, not nadir point  pi to pi rad          
         *    order       - size of gravity field                         1..~170
         *    unnormArr   - array of normalization values                  
         *
         *  outputs       :
         *    LegArrFU     - array of unnormalized Legendre polynomials, fukushima
         *    LegArrFN     - array of normalized Legendre polynomials, fukushima
         *
         *  locals :
         *    L,m         - degree and order indices
         *
         *  coupling      :
         *   none
         *
         *  references :
         *    fukushima 2012a
           ----------------------------------------------------------------------------*/

        public void LegPolyF
           (
               double latgc,
               Int32 order,
               char normalized,
               double[,,] normArr,
               out double[,] legarrFN
           )
        {
            Int32 L, m;
            legarrFN = new double[order + 2, order + 2];
            double[] psm = new double[order + 2];
            Int32[] ipsm = new Int32[order + 2];

            // get diagonal terms
            for (L = 0; L <= order; L++)
                alfsx(Math.Cos(latgc), order, normArr, out psm, out ipsm);

            // get each col for rest of matrix
            for (m = 0; m <= order; m++)
                alfmx(Math.Sin(latgc), m, order, normArr, psm[m], ipsm[m], ref legarrFN);

        }  // LegPolyF



        /* ----------------------------------------------------------------------------
        *
        *                           function TrigPoly
        *
        *   this function finds the accumulated trigonmetric terms for satellite 
        *   operations, orders up to about 120 are valid. this is the gtds approach. 
        *   
        *  author        : david vallado             davallado@gmail.com  10 oct 2019
        *
        *  inputs        description                                   range / units
        *    lon         - longitude of satellite                        0 - 2pi rad
        *    order       - size of gravity field                         1..2160..
        *
        *  outputs       :
        *    trigArr     - array of trigonometric terms
        *
        *  locals :
        *    L,m         - degree and order indices
        *
        *  coupling      :
        *   none
        *
        *  references :
        *    vallado       2013, 597, Eq 8-57
        * ----------------------------------------------------------------------------*/

        public void TrigPoly
           (
               double lon,
               Int32 order,
               out double[,] trigArr
           )
        {
            trigArr = new double[order + 1, 3];
            double tlon, clon, slon;
            Int32 m;

            // initial values
            trigArr[0, 0] = 0.0;    // Math.Sin terms
            trigArr[0, 1] = 1.0;    // Math.Cos terms
            trigArr[0, 2] = 0.0;
            tlon = Math.Tan(lon);
            trigArr[1, 0] = slon = Math.Sin(lon);
            trigArr[1, 1] = clon = Math.Cos(lon);
            trigArr[1, 2] = tlon;

            for (m = 2; m <= order; m++)
            {
                trigArr[m, 0] = 2.0 * clon * trigArr[m - 1, 0] - trigArr[m - 2, 0];  // Math.Sin terms
                trigArr[m, 1] = 2.0 * clon * trigArr[m - 1, 1] - trigArr[m - 2, 1];  // Math.Cos terms
                trigArr[m, 2] = (m - 1) * tlon + tlon;  // m tan
            }
        } // TrigPoly



        /* ----------------------------------------------------------------------------
        *
        *                           function TrigPolyLeg
        *
        *   this function finds the accumulated legendre polynomials and trigonmetric
        *   terms for satellite operations, orders up to about 120 are valid. this is the 
        *   montenbruck approach. 
        *   
        *  author        : david vallado             davallado@gmail.com  10 oct 2019
        *
        *  inputs        description                                   range / units
        *    recef       - satellite position vector, earth fixed        km
        *    latgc       - geocentric lat of satellite, not nadir point  pi to pi rad          
        *    order       - size of gravity field                         1..2160..
        *    unnormArr     - array of normalization values                  
        *
        *  outputs       :
        *    VArr        - array of trig terms
        *    WArr        - array of trig terms
        *    VArrN       - array of trig terms normalized
        *    WArrN       - array of trig terms normalized
        *
        *  locals :
        *    L,m         - degree and order indices
        *
        *  coupling      :
        *   none
        *
        *  references :
        *    vallado       2013, 597, Eq 8-57
        * ----------------------------------------------------------------------------*/

        public void TrigPolyLeg
           (
               double[] recef,
               double latgc,
               Int32 order,
               char normalized,
               double[,] unnormArr,
               out double[,] VArr,
               out double[,] WArr,
               out double[,] VArrN,
               out double[,] WArrN
           )
        {
            double conv;
            double magr;
            Int32 L, m;

            // unnormalized
            VArr = new double[order + 2, order + 2];
            WArr = new double[order + 2, order + 2];
            // normalized
            VArrN = new double[order + 2, order + 2];
            WArrN = new double[order + 2, order + 2];

            magr = MathTimeLibr.mag(recef);

            // form first set of recursions for l = m on V and W
            double temp = gravConst.re / (magr * magr);

            // initial zonal values
            VArr[0, 0] = gravConst.re / magr;
            VArr[1, 0] = VArr[0, 0] * VArr[0, 0] * Math.Sin(latgc);
            WArr[0, 0] = 0.0;
            WArr[1, 0] = 0.0;

            // find zonal terms
            for (L = 2; L <= order + 1; L++)
            {
                VArr[L, 0] = (2.0 * L - 1) / L * recef[2] * temp * VArr[L - 1, 0] -
                     (L - 1.0) / L * temp * gravConst.re * VArr[L - 2, 0];
                WArr[L, 0] = 0.0;
            }

            // find tesseral and sectoral terms
            for (L = 1; L <= order + 1; L++)
            {
                m = L;
                VArr[L, m] = (2.0 * m - 1) * (recef[0] * temp * VArr[m - 1, m - 1] - recef[1] * temp * WArr[m - 1, m - 1]);
                WArr[L, m] = (2.0 * m - 1) * (recef[0] * temp * WArr[m - 1, m - 1] + recef[1] * temp * VArr[m - 1, m - 1]);

                for (int k = m + 1; k <= order + 1; k++)
                {
                    VArr[k, m] = ((2.0 * k - 1) / (k - m)) * recef[2] * temp * VArr[k - 1, m] -
                        (k + m - 1.0) / (k - m) * temp * gravConst.re * VArr[k - 2, m];
                    WArr[k, m] = ((2.0 * k - 1) / (k - m)) * recef[2] * temp * WArr[k - 1, m] -
                        (k + m - 1.0) / (k - m) * temp * gravConst.re * WArr[k - 2, m];
                }
            }

            // --------------- now normalize the variables
            // note this is only good for equally indexed functions [L, m], others have to be corrected later
            if (normalized == 'y')
            {
                VArrN[0, 0] = gravConst.re / magr;
                WArrN[0, 0] = 0.0;
                for (L = 1; L <= order + 1; L++)
                {
                    for (m = 0; m <= L; m++)
                    {
                        conv = unnormArr[L, m];
                        VArrN[L, m] = conv * VArr[L, m];
                        WArrN[L, m] = conv * WArr[L, m];
                    }
                }
            }

        } // TrigPolyLeg


        /* ----------------------------------------------------------------------------
        *
        *                           function FullGeopG
        *
        *   this function finds the acceleration for the gravity field. the acceleration is
        *   found in the body fixed frame. rotation back to inertial is done after this 
        *   routine. this is the gtds approach. 
        *      
        *  author        : david vallado             davallado@gmail.com  10 oct 2019
        *
        *  inputs        description                                   range / units
        *    recef       - position vector ECEF                          km   
        *    order       - size of gravity field                         1..360
        *    normalized  - normalized in file                            'y', 'n'
        *    unnormArr   - array of normalization values                  
        *    gravData.c  - gravitational coefficients (in gravData)
        *    gravData.s  - gravitational coefficients (in gravData)
        *
        *  outputs       :
        *    apertG      - efc perturbation acceleration                  km / s^2
        *
        *  locals :
        *    show        - show intermediate steps                        'y', 'n'
        *    straccum    - string containing the various intermediate steps
        *    conv        - conversion to normalize
        *    L, m        - degree and order indices
        *    trigArr     - array of trigonometric terms
        *    LegArr      - array of Legendre polynomials
        *    VArr        - array of trig terms
        *    WArr        - array of trig terms
        *
        *  coupling      :
        *   LegPolyG     - find the unnormalized Legendre polynomials through recursion
        *   TrigPoly     - find the trigonmetric terms through recursion
        *
        *  references :
        *    vallado       2013, 597, Eq 8-57
        * ----------------------------------------------------------------------------*/

        public void FullGeopG
        (
            double[] recef,
            Int32 order,
            char normalized,
            double[,] unnormArr,
            double[,,] normArr,
            gravityConst gravData,
            out double[] aPertG,
            char show,
            out string straccum
        )
        {
            Int32 L, m;
            double[,] legarrGU;  // unnormalized gtds
            double[,] legarrGN;  // normalized gtds
            double[,] trigArr;
            double oordelta, temp, oor, ror, muor, sumM1, sumM2, sumM3, dRdr,
                   dRdlat, dRdlon, RDelta, latgc, latgd, hellp, lon;
            double conv;
            aPertG = new double[3];
            sumM1 = 0.0;
            sumM2 = 0.0;
            sumM3 = 0.0;
            straccum = "";

            // --------------------find latgc and lon---------------------- }
            ecef2ll(recef, out latgc, out latgd, out lon, out hellp);

            // ---------------------Find Legendre polynomials -------------- }
            LegPolyG(latgc, order, normalized, unnormArr, normArr, out legarrGU, out legarrGN);

            TrigPoly(lon, order, out trigArr);

            if (show == 'y')
            {
                straccum = straccum + "legarrGU 4 0   " + legarrGU[4, 0].ToString() + "  4 1   " + legarrGU[4, 1].ToString() + "  4 4   " + legarrGU[4, 4].ToString() + "\n";
                straccum = straccum + "legarrGU 5 0   " + legarrGU[5, 0].ToString() + "  5 1   " + legarrGU[5, 1].ToString() + "  5 5   " + legarrGU[5, 5].ToString() + "\n";
                //straccum = straccum + "legarrGU 6 0   " + legarrGU[6, 0].ToString() + "  6 1   " + legarrGU[6, 1].ToString() + "  6 6   " + legarrGU[6, 6].ToString() + "\n";
                straccum = straccum + "trigarr 2 2 Math.Sin  " + trigArr[2, 0].ToString() + "  Math.Cos   " + trigArr[2, 1].ToString() + "  tan   " + trigArr[2, 2].ToString() + "\n";
            }

            // ----------Partial derivatives of disturbing potential ------- }
            double magr = MathTimeLibr.mag(recef);
            ror = gravConst.re / magr;
            dRdr = 0.0;
            dRdlat = 0.0;
            dRdlon = 0.0;
            temp = ror;

            // sum the Legendre polynomials for the given order
            for (L = 2; L <= order; L++)
            {
                // will do the power as each L is indexed }
                temp = temp * ror;
                sumM1 = 0.0;
                sumM2 = 0.0;
                sumM3 = 0.0;

                for (m = 0; m <= L; m++)
                {
                    // unnormalized should sum in reverse to preserve accuracy
                    if (normalized == 'n')  // unnormalized
                    {
                        double temparg = gravData.c[L, m] * trigArr[m, 1] + gravData.s[L, m] * trigArr[m, 0];
                        sumM1 = sumM1 + legarrGU[L, m] * temparg;
                        if (m + 1 <= L)
                            sumM2 = sumM2 + (legarrGU[L, m + 1] - trigArr[m, 2] * legarrGU[L, m]) * temparg;
                        else
                            sumM2 = sumM2 - trigArr[m, 2] * legarrGU[L, m] * temparg;
                        sumM3 = sumM3 + legarrGU[L, m] * m * (gravData.s[L, m] * trigArr[m, 1] - gravData.c[L, m] * trigArr[m, 0]);
                    }
                    else  // normalized
                    {
                        double temparg = gravData.cNor[L, m] * trigArr[m, 1] + gravData.sNor[L, m] * trigArr[m, 0];
                        sumM1 = sumM1 + legarrGN[L, m] * temparg;
                        if (m + 1 <= L)
                        {
                            // synchronize the normalization values
                            conv = unnormArr[L, m] / unnormArr[L, m + 1];
                            sumM2 = sumM2 + (conv * legarrGN[L, m + 1] - trigArr[m, 2] * legarrGN[L, m]) * temparg;
                        }
                        else
                            sumM2 = sumM2 - trigArr[m, 2] * legarrGN[L, m] * temparg;
                        sumM3 = sumM3 + legarrGN[L, m] * m * (gravData.sNor[L, m] * trigArr[m, 1] - gravData.cNor[L, m] * trigArr[m, 0]);
                    }
                }  // for m 

                dRdr = dRdr + temp * (L + 1) * sumM1;
                dRdlat = dRdlat + temp * sumM2;
                dRdlon = dRdlon + temp * sumM3;
            } // for L 

            muor = gravConst.mu / magr;
            dRdr = -muor / magr * dRdr;
            dRdlat = muor * dRdlat;
            dRdlon = muor * dRdlon;

            // ----------Non - spherical perturbative acceleration ------------ }
            oor = 1.0 / magr;
            RDelta = recef[0] * recef[0] + recef[1] * recef[1];
            double oordeltasqrt = 1.0 / Math.Sqrt(RDelta);
            oordelta = 1.0 / RDelta;
            temp = oor * dRdr - recef[2] * oor * oor * oordeltasqrt * dRdlat;
            //double tmp = gravConst.mu / (Math.Pow(MathTimeLibr.mag(recef), 3));

            aPertG[0] = temp * recef[0] - oordelta * dRdlon * recef[1]; // - tmp * recef[0];
            aPertG[1] = temp * recef[1] + oordelta * dRdlon * recef[0]; // - tmp * recef[1];
            aPertG[2] = oor * dRdr * recef[2] + oor * oor * Math.Sqrt(RDelta) * dRdlat; // - tmp * recef[2];

            if (show == 'y')
                straccum = straccum + "apertG  ef 4 4   " + aPertG[0].ToString() + "     " + aPertG[1].ToString() + "     " + aPertG[2].ToString() + "\n";
        }  // FullGeopG 


        /* ----------------------------------------------------------------------------
        *
        *                           function FullGeopM
        *
        *   this function finds the acceleration for the gravity field. the acceleration is
        *   found in the body fixed frame. rotation back to inertial is done after this 
        *   routine. this is the montenbruck approach. 
        *      
        *  author        : david vallado             davallado@gmail.com  10 oct 2019
        *
        *  inputs        description                                   range / units
        *    recef       - position vector ECEF                          km   
        *    order       - size of gravity field                         1..360
        *    normalized  - normalized in file                            'y', 'n'
        *    unnormArr   - array of normalization values                  
        *    gravData.c  - gravitational coefficients (in gravData)
        *    gravData.s  - gravitational coefficients (in gravData)
        *
        *  outputs       :
        *    apert       - efc perturbation acceleration                  km / s^2
        *
        *  locals :
        *    show        - show intermediate steps                        'y', 'n'
        *    straccum    - string containing the various intermediate steps
        *    conv        - conversion to normalize
        *    L, m        - degree and order indices
        *    LegArr      - array of Legendre polynomials
        *    VArr        - array of trig terms
        *    WArr        - array of trig terms
        *
        *  coupling      :
        *   LegPoly      - find the unnormalized Legendre polynomials through recursion
        *   TrigPoly     - find the trigonmetric terms through recursion
        *
        *  references :
        *    vallado       2013, 597, Eq 8-57
        * ----------------------------------------------------------------------------*/

        public void FullGeopM
        (
            double[] recef,
            Int32 order,
            char normalized,
            double[,] unnormArr,
            gravityConst gravData,
            out double[] aPertM,
            char show,
            out string straccum
        )
        {
            Int32 L, m;
            double[,] legarrMU;  // unnormalized montenbruck
            double[,] legarrMN;  // normalized montenbruck
            double[,] VArr;
            double[,] WArr;
            double[,] VArrN;
            double[,] WArrN;
            double latgc, latgd, hellp, lon;
            double conv, conv1, temp, temp1;
            aPertM = new double[3];
            straccum = "";

            // note, lat/lon could be done by Cartesian terms to erase any concenrs about proximity to 90 deg lat cases.

            // --------------------find latgc and lon---------------------- }
            ecef2ll(recef, out latgc, out latgd, out lon, out hellp);

            // ---------------------Find Legendre polynomials -------------- }
            LegPolyM(latgc, order, normalized, unnormArr, out legarrMU, out legarrMN);

            TrigPolyLeg(recef, latgc, order, normalized, unnormArr, out VArr, out WArr, out VArrN, out WArrN);

            if (show == 'y')
            {
                straccum = "legarrMU 4 0   " + legarrMU[4, 0].ToString() + "  4 1   " + legarrMU[4, 1].ToString() + "  4 4   " + legarrMU[4, 4].ToString() + "\n";
                straccum = straccum + "legarrMU 5 0   " + legarrMU[5, 0].ToString() + "  5 1   " + legarrMU[5, 1].ToString() + "  5 5   " + legarrMU[5, 5].ToString() + "\n";
                //straccum = straccum + "legarrMU 6 0   " + legarrMU[6, 0].ToString() + "  6 1   " + legarrMU[6, 1].ToString() + "  6 6   " + legarrMU[6, 6].ToString() + "\n";
                if (normalized == 'n')
                {
                    straccum = straccum + "VArr    1    " + VArr[1, 0].ToString() + "    " + VArr[1, 1].ToString() + "\n";
                    straccum = straccum + "WArr    1    " + WArr[1, 0].ToString() + "    " + WArr[1, 1].ToString() + "\n";
                    straccum = straccum + "VArr    2    " + VArr[2, 0].ToString() + "    " + VArr[2, 1].ToString() + "    " + VArr[2, 2].ToString() + "\n";
                    straccum = straccum + "WArr    2    " + WArr[2, 0].ToString() + "    " + WArr[2, 1].ToString() + "    " + WArr[2, 2].ToString() + "\n";
                    straccum = straccum + "VArr    3    " + VArr[3, 0].ToString() + "    " + VArr[3, 1].ToString() + "    " + VArr[3, 2].ToString() + "\n";
                    straccum = straccum + "WArr    3    " + WArr[3, 0].ToString() + "    " + WArr[3, 1].ToString() + "    " + WArr[3, 2].ToString() + "\n";
                    straccum = straccum + "VArr    4    " + VArr[4, 0].ToString() + "    " + VArr[4, 1].ToString() + "    " + VArr[4, 2].ToString() + "    " + VArr[4, 3].ToString() + "    " + VArr[4, 4].ToString() + "\n";
                    straccum = straccum + "WArr    4    " + WArr[4, 0].ToString() + "    " + WArr[4, 1].ToString() + "    " + WArr[4, 2].ToString() + "    " + WArr[4, 3].ToString() + "    " + WArr[4, 4].ToString() + "\n";
                }
                else
                {
                    straccum = straccum + "VArrN   1    " + VArrN[1, 0].ToString() + "    " + VArrN[1, 1].ToString() + "\n";
                    straccum = straccum + "WArrN   1    " + WArrN[1, 0].ToString() + "    " + WArrN[1, 1].ToString() + "\n";
                    straccum = straccum + "VArrN   2    " + VArrN[2, 0].ToString() + "    " + VArrN[2, 1].ToString() + "    " + VArrN[2, 2].ToString() + "\n";
                    straccum = straccum + "WArrN   2    " + WArrN[2, 0].ToString() + "    " + WArrN[2, 1].ToString() + "    " + WArrN[2, 2].ToString() + "\n";
                    straccum = straccum + "VArrN   3    " + VArrN[3, 0].ToString() + "    " + VArrN[3, 1].ToString() + "    " + VArrN[3, 2].ToString() + "\n";
                    straccum = straccum + "WArrN   3    " + WArrN[3, 0].ToString() + "    " + WArrN[3, 1].ToString() + "    " + WArrN[3, 2].ToString() + "\n";
                    straccum = straccum + "VArrN   4    " + VArrN[4, 0].ToString() + "    " + VArrN[4, 1].ToString() + "    " + VArrN[4, 2].ToString() + "    " + VArrN[4, 3].ToString() + "    " + VArrN[4, 4].ToString() + "\n";
                    straccum = straccum + "WArrN   4    " + WArrN[4, 0].ToString() + "    " + WArrN[4, 1].ToString() + "    " + WArrN[4, 2].ToString() + "    " + WArrN[4, 3].ToString() + "    " + WArrN[4, 4].ToString() + "\n";
                }
            }

            // correction term for method of formulating VArr and WArr
            temp = gravConst.mu / (gravConst.re * gravConst.re);
            temp = 1.0;
            // reverse order of summations to get it more accurate??
            for (m = 0; m <= order; m++)
            {
                for (L = m; L <= order; L++)
                {
                    if (normalized == 'n')
                    {
                        if (m == 0)
                        {
                            aPertM[0] = aPertM[0] + temp * (-gravData.c[L, 0] * VArr[L + 1, 1]);
                            aPertM[1] = aPertM[1] + temp * (-gravData.c[L, 0] * WArr[L + 1, 1]);
                            aPertM[2] = aPertM[2] + temp * (L + 1) * (-gravData.c[L, 0] * VArr[L + 1, 0]);
                        }
                        else
                        {
                            //  double temp1 = MathTimeLibr.factorial(L - m + 2) / MathTimeLibr.factorial(L - m);
                            // 1975 Giacaglia pg 6 shows the conversion, more stable
                            temp1 = temp * 0.5 * (L - m + 1) * (L - m + 2);
                            aPertM[0] = aPertM[0] + 0.5 * temp * (-gravData.c[L, m] * VArr[L + 1, m + 1] - gravData.s[L, m] * WArr[L + 1, m + 1]) +
                                temp1 * (gravData.c[L, m] * VArr[L + 1, m - 1] + gravData.s[L, m] * WArr[L + 1, m - 1]);
                            aPertM[1] = aPertM[1] + 0.5 * temp * (-gravData.c[L, m] * WArr[L + 1, m + 1] + gravData.s[L, m] * VArr[L + 1, m + 1]) +
                                temp1 * (-gravData.c[L, m] * WArr[L + 1, m - 1] + gravData.s[L, m] * VArr[L + 1, m - 1]);
                            aPertM[2] = aPertM[2] + temp * ((L - m + 1) * (-gravData.c[L, m] * VArr[L + 1, m] - gravData.s[L, m] * WArr[L + 1, m]));
                        }
                    }
                    else  // normalized
                    {
                        // note Varr and WArr not formulated for normalized yet because normalization is different for different terms
                        // this really maps out a gradient? of the normalization
                        // L,m vs L+1, m+1
                        // L,m vs L+1, m-1
                        // L,m vs L+1, m
                        if (m == 0)
                        {
                            conv = unnormArr[L, 0] / unnormArr[L + 1, 1];
                            aPertM[0] = aPertM[0] + temp * conv * (-gravData.cNor[L, 0] * VArrN[L + 1, 1]);
                            aPertM[1] = aPertM[1] + temp * conv * (-gravData.cNor[L, 0] * WArrN[L + 1, 1]);
                            conv = unnormArr[L, 0] / unnormArr[L + 1, 0];
                            aPertM[2] = aPertM[2] + temp * conv * (L + 1) * (-gravData.cNor[L, 0] * VArrN[L + 1, 0]);
                        }
                        else
                        {
                            //  double temp1 = MathTimeLibr.factorial(L - m + 2) / MathTimeLibr.factorial(L - m);
                            // 1975 Giacaglia pg 6 shows the conversion, more stable
                            temp1 = temp * 0.5 * (L - m + 1) * (L - m + 2);
                            conv = unnormArr[L, m] / unnormArr[L + 1, m + 1];
                            conv1 = unnormArr[L, m] / unnormArr[L + 1, m - 1];
                            aPertM[0] = aPertM[0] + 0.5 * temp * conv * (-gravData.cNor[L, m] * VArrN[L + 1, m + 1] - gravData.sNor[L, m] * WArrN[L + 1, m + 1]) +
                                temp1 * conv1 * (gravData.cNor[L, m] * VArrN[L + 1, m - 1] + gravData.sNor[L, m] * WArrN[L + 1, m - 1]);
                            aPertM[1] = aPertM[1] + 0.5 * temp * conv * (-gravData.cNor[L, m] * WArrN[L + 1, m + 1] + gravData.sNor[L, m] * VArrN[L + 1, m + 1]) +
                                temp1 * conv1 * (-gravData.cNor[L, m] * WArrN[L + 1, m - 1] + gravData.sNor[L, m] * VArrN[L + 1, m - 1]);
                            conv = unnormArr[L, m] / unnormArr[L + 1, m];
                            aPertM[2] = aPertM[2] + temp * conv * ((L - m + 1) * (-gravData.cNor[L, m] * VArrN[L + 1, m] - gravData.sNor[L, m] * WArrN[L + 1, m]));
                        }
                    }
                    if (show == 'y')
                    {
                        straccum = straccum + "Lm " + L.ToString() + " " + m.ToString() +
                               aPertM[0].ToString() + " " + aPertM[1].ToString() + " " + aPertM[2].ToString() + "\n";
                    }

                }  // for m 
            }  // for L

            temp = gravConst.mu / (gravConst.re * gravConst.re);
            aPertM[0] = temp * aPertM[0];
            aPertM[1] = temp * aPertM[1];
            aPertM[2] = temp * aPertM[2];
            if (show == 'y')
                straccum = straccum + "apertM  bf 4 4   " + aPertM[0].ToString() + "     " + aPertM[1].ToString() + "     " + aPertM[2].ToString() + "\n";

            // Inertial acceleration 
            //return Transp(E) * a_bf;

        }  // FullGeopM 


        /* ----------------------------------------------------------------------------
        *
        *                           function FullGeopMC
        *
        *   this function finds the acceleration for the gravity field. the acceleration is
        *   found in the body fixed frame. rotation back to inertial is done after this 
        *   routine. this is the montenbruck code approach. 
        *      
        *  author        : david vallado             davallado@gmail.com  10 oct 2019
        *
        *  inputs        description                                   range / units
        *    recef       - position vector ECEF                          km   
        *    order       - size of gravity field                         1..360
        *    normalized  - normalized in file                            'y', 'n'
        *    unnormArr   - array of normalization values                  
        *    gravData.c  - gravitational coefficients (in gravData)
        *    gravData.s  - gravitational coefficients (in gravData)
        *
        *  outputs       :
        *    apertMC     - efc perturbation acceleration                  km / s^2
        *
        *  locals :
        *    conv        - conversion to normalize
        *    L, m        - degree and order indices
        *    LegArr      - array of Legendre polynomials
        *    VArr        - array of trig terms
        *    WArr        - array of trig terms
        *
        *  coupling      :
        *   LegPoly      - find the unnormalized Legendre polynomials through recursion
        *
        *  references :
        *    vallado       2013, 597, Eq 8-57
        * ----------------------------------------------------------------------------*/

        public void FullGeopMC
        (
            double[] recef,
            Int32 order,
            char normalized,
            double[,] unnormArr,
            gravityConst gravData,
            out double[] aPertMC,
            char show,
            out string straccum
        )
        {
            Int32 L, m;
            double temp, latgc, latgd, hellp, lon, magr;
            double conv, conv1;
            aPertMC = new double[3];
            straccum = "";

            int n;                           // Loop counters
            double r_sqr, rho, Fac;               // Auxiliary quantities
            double x0, y0, z0;                      // Normalized coordinates
            double C, S;                           // Gravitational coefficients
            double[,] V = new double[order + 2, order + 2];
            double[,] W = new double[order + 2, order + 2];
            double[,] VArrN = new double[order + 2, order + 2];
            double[,] WArrN = new double[order + 2, order + 2];

            magr = MathTimeLibr.mag(recef);

            // --------------------find latgc and lon---------------------- }
            ecef2ll(recef, out latgc, out latgd, out lon, out hellp);

            // r_bf = E * r;
            // Auxiliary quantities
            r_sqr = MathTimeLibr.dot(recef, recef);               // Square of distance
            rho = gravConst.re * gravConst.re / r_sqr;
            x0 = gravConst.re * recef[0] / r_sqr;          // Normalized
            y0 = gravConst.re * recef[1] / r_sqr;          // coordinates
            z0 = gravConst.re * recef[2] / r_sqr;

            //
            // Evaluate harmonic functions 
            //   V_nm = (R_ref/r)^(n+1) * P_nm(sin(phi)) * Math.Cos(m*lambda)
            // and 
            //   W_nm = (R_ref/r)^(n+1) * P_nm(sin(phi)) * Math.Sin(m*lambda)
            // up to degree and order n_max+1
            // Calculate zonal terms V(n,0); set W(n,0)=0.0

            V[0, 0] = gravConst.re / Math.Sqrt(r_sqr);
            W[0, 0] = 0.0;
            V[1, 0] = z0 * V[0, 0];
            W[1, 0] = 0.0;
            for (n = 2; n <= order; n++)
            {
                V[n, 0] = ((2.0 * n - 1) * z0 * V[n - 1, 0] - (n - 1) * rho * V[n - 2, 0]) / n;
                W[n, 0] = 0.0;
            }

            // Calculate tesseral and sectorial terms 
            for (m = 1; m <= order + 1; m++)
            {
                // Calculate V(m,m) .. V(n_max+1,m)
                V[m, m] = (2.0 * m - 1) * (x0 * V[m - 1, m - 1] - y0 * W[m - 1, m - 1]);
                W[m, m] = (2.0 * m - 1) * (x0 * W[m - 1, m - 1] + y0 * V[m - 1, m - 1]);
                if (m <= order)
                {
                    V[m + 1, m] = (2.0 * m + 1) * z0 * V[m, m];
                    W[m + 1, m] = (2.0 * m + 1) * z0 * W[m, m];
                }
                for (n = m + 2; n <= order + 1; n++)
                {
                    V[n, m] = ((2.0 * n - 1) * z0 * V[n - 1, m] - (n + m - 1) * rho * V[n - 2, m]) / (n - m);
                    W[n, m] = ((2.0 * n - 1) * z0 * W[n - 1, m] - (n + m - 1) * rho * W[n - 2, m]) / (n - m);
                }
            }

            // --------------- now normalize the variables
            VArrN[0, 0] = gravConst.re / magr;
            WArrN[0, 0] = 0.0;
            for (L = 1; L <= order + 1; L++)
            {
                for (m = 0; m <= L; m++)
                {
                    conv = unnormArr[L, m];
                    VArrN[L, m] = conv * V[L, m];
                    WArrN[L, m] = conv * W[L, m];
                }
            }

            if (show == 'y')
            {
                if (normalized == 'n')
                {
                    straccum = straccum + "V    1    " + V[1, 0].ToString() + "    " + V[1, 1].ToString() + "\n";
                    straccum = straccum + "W    1    " + W[1, 0].ToString() + "    " + W[1, 1].ToString() + "\n";
                    straccum = straccum + "V    2    " + V[2, 0].ToString() + "    " + V[2, 1].ToString() + "    " + V[2, 2].ToString() + "\n";
                    straccum = straccum + "W    2    " + W[2, 0].ToString() + "    " + W[2, 1].ToString() + "    " + W[2, 2].ToString() + "\n";
                    straccum = straccum + "V    3    " + V[3, 0].ToString() + "    " + V[3, 1].ToString() + "    " + V[3, 2].ToString() + "\n";
                    straccum = straccum + "W    3    " + W[3, 0].ToString() + "    " + W[3, 1].ToString() + "    " + W[3, 2].ToString() + "\n";
                    straccum = straccum + "V    4    " + V[4, 0].ToString() + "    " + V[4, 1].ToString() + "    " + V[4, 2].ToString() + "    " + V[4, 3].ToString() + "    " + V[4, 4].ToString() + "\n";
                    straccum = straccum + "W    4    " + W[4, 0].ToString() + "    " + W[4, 1].ToString() + "    " + W[4, 2].ToString() + "    " + W[4, 3].ToString() + "    " + W[4, 4].ToString() + "\n";
                }
                else
                {
                    straccum = straccum + "VArrN   1    " + VArrN[1, 0].ToString() + "    " + VArrN[1, 1].ToString() + "\n";
                    straccum = straccum + "WArrN   1    " + WArrN[1, 0].ToString() + "    " + WArrN[1, 1].ToString() + "\n";
                    straccum = straccum + "VArrN   2    " + VArrN[2, 0].ToString() + "    " + VArrN[2, 1].ToString() + "    " + VArrN[2, 2].ToString() + "\n";
                    straccum = straccum + "WArrN   2    " + WArrN[2, 0].ToString() + "    " + WArrN[2, 1].ToString() + "    " + WArrN[2, 2].ToString() + "\n";
                    straccum = straccum + "VArrN   3    " + VArrN[3, 0].ToString() + "    " + VArrN[3, 1].ToString() + "    " + VArrN[3, 2].ToString() + "\n";
                    straccum = straccum + "WArrN   3    " + WArrN[3, 0].ToString() + "    " + WArrN[3, 1].ToString() + "    " + WArrN[3, 2].ToString() + "\n";
                    straccum = straccum + "VArrN   4    " + VArrN[4, 0].ToString() + "    " + VArrN[4, 1].ToString() + "    " + VArrN[4, 2].ToString() + "    " + VArrN[4, 3].ToString() + "    " + VArrN[4, 4].ToString() + "\n";
                    straccum = straccum + "WArrN   4    " + WArrN[4, 0].ToString() + "    " + WArrN[4, 1].ToString() + "    " + WArrN[4, 2].ToString() + "    " + WArrN[4, 3].ToString() + "    " + WArrN[4, 4].ToString() + "\n";
                }
            }

            // Calculate accelerations,note order switched here
            for (m = 0; m <= order; m++)
            {
                for (n = m; n <= order; n++)
                {
                    if (m == 0)
                    {
                        if (normalized == 'n')
                        {
                            C = gravData.c[n, 0];      // = C_n,0
                            aPertMC[0] = aPertMC[0] - C * V[n + 1, 1];
                            aPertMC[1] = aPertMC[1] - C * W[n + 1, 1];
                            aPertMC[2] = aPertMC[2] - (n + 1) * C * V[n + 1, 0];
                        }
                        else
                        {
                            C = gravData.cNor[n, 0];   // = C_n,0??
                            conv = unnormArr[n, 0] / unnormArr[n + 1, 1];
                            aPertMC[0] = aPertMC[0] - conv * C * VArrN[n + 1, 1];
                            aPertMC[1] = aPertMC[1] - conv * C * WArrN[n + 1, 1];
                            conv = unnormArr[n, 0] / unnormArr[n + 1, 0];
                            aPertMC[2] = aPertMC[2] - conv * (n + 1) * C * VArrN[n + 1, 0];
                        }
                    }
                    else
                    {
                        if (normalized == 'n')
                        {
                            C = gravData.c[n, m]; // = C_n,m 
                            S = gravData.s[n, m]; // = S_n,m   he stores CS differently m - 1, n
                            Fac = 0.5 * (n - m + 1) * (n - m + 2);
                            aPertMC[0] = aPertMC[0] + 0.5 * (-C * V[n + 1, m + 1] - S * W[n + 1, m + 1])
                                    + Fac * (+C * V[n + 1, m - 1] + S * W[n + 1, m - 1]);
                            aPertMC[1] = aPertMC[1] + 0.5 * (-C * W[n + 1, m + 1] + S * V[n + 1, m + 1])
                                    + Fac * (-C * W[n + 1, m - 1] + S * V[n + 1, m - 1]);
                            aPertMC[2] = aPertMC[2] + (n - m + 1) * (-C * V[n + 1, m] - S * W[n + 1, m]);
                        }
                        else
                        {
                            // note V and W not formulated for normalized yet??
                            C = gravData.cNor[n, m];  // = C_n,m 
                            S = gravData.sNor[n, m];  // = S_n,m   he stores CS differently m - 1, n
                            Fac = 0.5 * (n - m + 1) * (n - m + 2);
                            conv = unnormArr[n, m] / unnormArr[n + 1, m + 1];
                            conv1 = unnormArr[n, m] / unnormArr[n + 1, m - 1];
                            aPertMC[0] = aPertMC[0] + 0.5 * conv * (-C * VArrN[n + 1, m + 1] - S * WArrN[n + 1, m + 1])
                                    + Fac * conv1 * (+C * VArrN[n + 1, m - 1] + S * WArrN[n + 1, m - 1]);
                            aPertMC[1] = aPertMC[1] + 0.5 * conv * (-C * WArrN[n + 1, m + 1] + S * VArrN[n + 1, m + 1])
                                    + Fac * conv1 * (-C * WArrN[n + 1, m - 1] + S * VArrN[n + 1, m - 1]);
                            conv = unnormArr[n, m] / unnormArr[n + 1, m];
                            aPertMC[2] = aPertMC[2] + (n - m + 1) * conv * (-C * VArrN[n + 1, m] - S * WArrN[n + 1, m]);
                        }
                    }

                    straccum = straccum + "nm " + n.ToString() + " " + m.ToString() +
                        aPertMC[0].ToString() + " " + aPertMC[1].ToString() + " " + aPertMC[2].ToString() + "\n";
                }
            }
            // Body-fixed acceleration
            // now get correction for initial V, W formulation
            temp = gravConst.mu / (gravConst.re * gravConst.re);
            aPertMC[0] = temp * aPertMC[0];
            aPertMC[1] = temp * aPertMC[1];
            aPertMC[2] = temp * aPertMC[2];
            if (show == 'y')
                straccum = straccum + "apertMC unbf 4 4   " + aPertMC[0].ToString() + "     " + aPertMC[1].ToString() + "     " + aPertMC[2].ToString() + "\n";

            // Inertial acceleration 
            //return Transp(E) * a_bf;
        }  // FullGeopMC 





        /* ---------------------------------------------------------------------------- 
        *
        *                           function pathm
        *
        *  this function determines the end position for a given range and azimuth
        *    from a given point.
        *
        *  author        : david vallado           davallado@gmail.com   27 may 2002
        *
        *  inputs description                                        range / units
        *    llat        - start geocentric latitude              -pi/2 to pi/2 rad
        *    llon        - start longitude (west -)               0.0  to 2pi rad
        *    range       - range between points er
        *    az          - azimuth                                0.0  to 2pi rad
        *
        *  outputs       :
        *    tlat        - end geocentric latitude                -pi/2 to pi/2 rad
        *    tlon        - end longitude(west -)                 0.0  to 2pi rad
        *
        *  locals        :
        *    sindeltan   - sine of delta n                              rad
        *    cosdeltan   - cosine of delta n                            rad
        *    deltan      - angle between the two points                 rad
        *
        *  coupling      :
        *    none.
        *
        *  references    :
        *    vallado       2013, 774-776, eq 11-6, eq 11-7
        * ------------------------------------------------------------------------------ */

        public void pathm
            (
              double llat, double llon, double range, double az, out double tlat, out double tlon
            )
        {
            double twopi = 2.0 * Math.PI;
            double small, deltan, sindn, cosdn;

            // -------------------------  implementation   -----------------
            small = 0.00000001;
            deltan = 0.0;

            az = az % twopi;
            if (llon < 0.0)
                llon = twopi + llon;
            if (range > twopi)
                range = range % twopi;

            // ----------------- find geocentric latitude  -----------------
            tlat = Math.Asin(Math.Sin(llat) * Math.Cos(range) + Math.Cos(llat) * Math.Sin(range) * Math.Cos(az));

            // ---- find deltan, the angle between the points -------------
            if ((Math.Abs(Math.Cos(tlat)) > small) && (Math.Abs(Math.Cos(llat)) > small))
            {
                sindn = Math.Sin(az) * Math.Sin(range) / Math.Cos(tlat);
                cosdn = (Math.Cos(range) - Math.Sin(tlat) * Math.Sin(llat)) / (Math.Cos(tlat) * Math.Cos(llat));
                deltan = Math.Atan2(sindn, cosdn);
            }
            else
            {
                // ------ case where launch is within 3nm of a pole --------
                if (Math.Abs(Math.Cos(llat)) <= small)
                {
                    if ((range > Math.PI) && (range < twopi))
                        deltan = az + Math.PI;
                    else
                        deltan = az;
                }
                // ----- case where end point is within 3nm of a pole ------
                if (Math.Abs(Math.Cos(tlat)) <= small)
                    deltan = 0.0;
            }

            tlon = llon + deltan;
            if (Math.Abs(tlon) > twopi)
                tlon = tlon % twopi;
            if (tlon < 0.0)
                tlon = twopi + tlon;
        }


        /* ---------------------------------------------------------------------------- 
        *
        *                           function rngaz
        *
        *  this function calculates the range and azimuth between two specified
        *
        *
        *    ground points on a spherical earth.notice the range will always be
        *    within the range of values listed since you for not know the direction of
        *    firing, long or short.  the function will calculate rotating earth ranges
        *    if the tof is passed in other than 0.0 . range is calulated in rad and
        *    converted to er by s = ro, but the radius of the earth = 1 er, so it's
        *    s = o.
        *
        * author        : david vallado           davallado@gmail.com   27 may 2002
        *
        *  revisions
        *                - dav 31 may 06 add elliptical model
        *
        *  inputs description                                    range / units
        *    llat        - start geocentric latitude             -pi/2 to pi/2 rad
        *    llon        - start longitude (west -)               0.0  to 2pi rad
        *    tlat        - end geocentric latitude               -pi/2 to pi/2 rad
        *    tlon        - end longitude(west -)                  0.0  to 2pi rad
        *    tof         - time of flight if icbm, or             0.0 min
        *
        *  outputs       :
        *    range       - range between points km
        *    az          - azimuth                                0.0  to 2pi rad
        *
        *  locals        :
        *    none.
        *
        *  coupling      :
        *    site, rot3, binomial, cross, atan2, dot, unit
        *
        *  references    :
        *    vallado       2001, 774-775, eq 11-3, eq 11-4, eq 11-5
        * ------------------------------------------------------------------------------ */
        public void rngaz
            (
              double llat, double llon, double tlat, double tlon, double tof,
              out double range, out double az
            )
        {
            double twopi = 2.0 * Math.PI;
            double small = 0.00000001;
            double omegaearth = 0.05883359221938136;
            // fix units on tof and omegaearth

            // -------------------------  implementation   -------------------------
            range = Math.Acos(Math.Sin(llat) * Math.Sin(tlat) +
                  Math.Cos(llat) * Math.Cos(tlat) * Math.Cos(tlon - llon + omegaearth * tof));

            // ------ check if the range is 0 or half the earth  ---------
            if (Math.Abs(Math.Sin(range) * Math.Cos(llat)) < small)
            {
                if (Math.Abs(range - Math.PI) < small)
                    az = Math.PI;
                else
                    az = 0.0;
            }
            else
            {
                az = Math.Acos((Math.Sin(tlat) - Math.Cos(range) * Math.Sin(llat)) /
                          (Math.Sin(range) * Math.Cos(llat)));
            }

            // ------ check if the azimuth is grt than pi( 180deg ) -------
            if (Math.Sin(tlon - llon + omegaearth * tof) < 0.0)
                az = twopi - az;

            string strtemp = "spehrical range " + (range * 6378.1363).ToString() + " km az " + (az * 180 / Math.PI).ToString();


            // test ellipsoidal approach
            //double rad, alt, e, eps, phi, pih1, phi2, temp, funct1, funct2, delta,
            //    s1, f1, s2, f2;
            //Int32 m, r;
            //double[] rlch, vlch, rtgt, vtgt, rlu, rtu, w, v, vprime, uprime = new double[3];
            //rad = 180 / Math.PI;
            //alt = 0.0;
            //site(llat, llon, alt, out rlch, out vlch);
            //site(tlat, tlon, alt, out rtgt, out vtgt);
            //strtemp = "U rlch " + rlch[0].ToString() + " " + rlch[1].ToString() + " " + rlch[2].ToString();
            //strtemp = "  rtgt " + rtgt[0].ToString() + " " + rtgt[1].ToString() + " " + rtgt[2].ToString();


            //rlu = MathTimeLibr.norm(rlch); // his u vector
            //rtu = MathTimeLibr.norm(rtgt);

            //strtemp = "u rlu " + rlu[1].ToString() + " " + rlu[2].ToString() + " " + rlu[3].ToString();
            //strtemp = "  rtu " + rtu[0].ToString() + " " + rtu[1].ToString() + " " + rtu[2].ToString();

            //double rp = 6356.0; // polar radius
            //double re = 6378.137; //equatorial radius
            //delta = re * re / (rp * rp) - 1.0;

            //eps = delta * (r1u[3] r1u[3 + rtu[3]rtu[3]);

            //MathTimeLibr.cross(rlu, rtu, out w);
            //strtemp = " 'w UxV " + w[0].ToString() + " " + w[1].ToString() + " " + w[2].ToString();
            //if (w[2] < 0.0)
            //{
            //    MathTimeLibr.cross(rtu, rlu, out w); // switch order
            //    strtemp = " 'w UxV " + w[0].ToString() + " " + w[1].ToString() + " " + w[2].ToString();
            //}
            //MathTimeLibr.cross(rlu, w, out v);
            //strtemp = "v uxw " + v[0].ToString() + " " + v[1].ToString() + " " + v[2].ToString();

            //phi = Math.PI - 0.5 * Math.Atan2(-2 * v[2] * rlu[2], v[2] *v[2] - (rlu[2] * rlu[2])); // use to get angle from just xy
            //fprintf(1, 'phi %11.7f %11.7f \n', phi, phi * rad);

            ////        phi = 0.5*atan2(-2*.47024*.86603, .47024^2-.86603^2); 
            ////        fprintf(1,'phi %11.7f %11.7f \n', phi, phi* rad);

            //temp = [MathTimeLibr.dot(rlch, rlu) MathTimeLibr.dot(rlch, rtu) 0.0];
            //uprime = MathTimeLibr.rot3(temp, phi) / 6378.137; // he uses cannonical units
            //fprintf(1, "uprime %11.7f %11.7f  %11.7f \n", uprime[0], uprime[1], uprime[2]);
            //phi1 = twopi + Math.Atan2(uprime[1] * Math.Sqrt(1 + eps), uprime[0]);
            //fprintf(1, "phi1 %11.7f %11.7f \n", phi1, phi1 * rad);

            //temp = [MathTimeLibr.dot(rtgt, rlu) MathTimeLibr.dot(rtgt, rtu) 0.0];
            //vprime = MathTimeLibr.rot3(temp, phi) / 6378.137; // he uses cannonical units
            //strtemp = "vprime " + vprime[0].ToString() + " " + vprime[1].ToString() + " " + vprime[2].ToString();
            //phi2 = twopi + Math.Atan2(vprime[1] * Math.Sqrt(1 + eps), vprime[0]);
            //fprintf(1, 'phi1 %11.7f %11.7f \n', phi2, phi2 * rad);

            //e = 0.08181922;
            //// do each half of integral evaluation
            //phi = phi2;
            //m = 1;
            //r = 0;
            //s1 = (MathTimeLibr.factorial(2 * m) * (MathTimeLibr.factorial(r)) ^ 2) / (2 ^ (2 * m - 2 * r) * MathTimeLibr.factorial(2 * r + 1) * (MathTimeLibr.factorial(m)) ^ 2) * (Math.Cos(phi) ^ (2 * r + 1));
            //f1 = MathTimeLibr.binomial(2 * m, m) * phi / 2 ^ (2 * m) + Math.Sin(phi) * s1;
            //r = 0;
            //m = 2;
            //s1 = (MathTimeLibr.factorial(2 * m) * (MathTimeLibr.factorial(r)) ^ 2) / (2 ^ (2 * m - 2 * r) * MathTimeLibr.factorial(2 * r + 1) * (MathTimeLibr.factorial(m)) ^ 2) * (Math.Cos(phi) ^ (2 * r + 1));
            //r = 1;
            //s2 = (MathTimeLibr.factorial(2 * m) * (MathTimeLibr.factorial(r)) ^ 2) / (2 ^ (2 * m - 2 * r) * MathTimeLibr.factorial(2 * r + 1) * (MathTimeLibr.factorial(m)) ^ 2) * (Math.Cos(phi) ^ (2 * r + 1));
            //f2 = MathTimeLibr.binomial(2 * m, m) * phi / 2 ^ (2 * m) + Math.Sin(phi) * (s1 + s2);
            //funct2 = (1.0 - e ^ 2 / 2 * f1 - MathTimeLibr.binomial(2 * m - 3, m - 2) * (e ^ 2 * f2) / (m * 2 ^ (2 * m - 2)));

            //phi = phi1;
            //m = 1;
            //r = 0;
            //s1 = (MathTimeLibr.factorial(2 * m) * (MathTimeLibr.factorial(r)) ^ 2) / (2 ^ (2 * m - 2 * r) * MathTimeLibr.factorial(2 * r + 1) * (MathTimeLibr.factorial(m)) ^ 2) * (Math.Cos(phi) ^ (2 * r + 1));
            //f1 = MathTimeLibr.binomial(2 * m, m) * phi / 2 ^ (2 * m) + Math.Sin(phi) * s1;
            //r = 0;
            //m = 2;
            //s1 = (MathTimeLibr.factorial(2 * m) * (MathTimeLibr.factorial(r)) ^ 2) / (2 ^ (2 * m - 2 * r) * MathTimeLibr.factorial(2 * r + 1) * (MathTimeLibr.factorial(m)) ^ 2) * (Math.Cos(phi) ^ (2 * r + 1));
            //r = 1;
            //s2 = (MathTimeLibr.factorial(2 * m) * (MathTimeLibr.factorial(r)) ^ 2) / (2 ^ (2 * m - 2 * r) * MathTimeLibr.factorial(2 * r + 1) * (MathTimeLibr.factorial(m)) ^ 2) * (Math.Cos(phi) ^ (2 * r + 1));
            //f2 = MathTimeLibr.binomial(2 * m, m) * phi / 2 ^ (2 * m) + Math.Sin(phi) * (s1 + s2);
            //funct1 = (1.0 - e ^ 2 / 2 * f1 - MathTimeLibr.binomial(2 * m - 3, m - 2) * (e ^ 2 * f2) / (m * 2 ^ (2 * m - 2)));

            //range = (funct2 - funct1) * re;
        }



        // -----------------------------------------------------------------------------------------
        //                                       covariance functions
        // -----------------------------------------------------------------------------------------

        /* ---------------------------------------------------------------------------- 
        *
        *                           function posvelcov2pts
        *
        *  finds 12 sigma points given position and velocity and covariance information
        *  using cholesky matrix square root algorithm
        *  then progates covariance points
        *
        *  author        : sal alfano      719-573-2600   31 mar 2011
        *
        *  revisions
        *                  dave vallado   make single routine to simply find sigma
        *                  points
        *
        *  inputs          description 
        *    cov         - eci 6x6 covariance matrix      km or m
        *    reci        - eci 3x1 position vector        km or m
        *    veci        - eci 3x1 velocity vector        km or m
        *
        *  outputs       :
        *    sigmapts    - structure of sigma points (6 x 12)
        *
        *  locals        :
        *
        *  sigmapts = posvelcov2pts(reci, veci, cov);
         ---------------------------------------------------------------------------- - */

        public void posvelcov2pts
            (
              double[] reci, double[] veci, double[,] cov, out double[,] sigmapts
            )
        {
            Int32 i, j, jj;
            double[,] s = new double[6, 6];
            // -------------------------  implementation   -----------------

            // initialize data & pre-allocate new points
            sigmapts = new double[,] { { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } }; // array is 6 x 12
            // compute matrix square root of nP
            s = MathTimeLibr.cholesky(cov);

            for (i = 0; i < 6; i++)
                for (j = 0; j < 6; j++)
                    s[i, j] = Math.Sqrt(6.0) * s[i, j];

            // perturb states, propagate to toff
            for (i = 0; i < 6; i++)
            {
                // ---- find positive direction vectors
                jj = (i - 1) * 2 + 2;  // incr this by 2
                for (j = 0; j < 3; j++)
                {
                    sigmapts[j, jj] = reci[j] + s[j, i];  // transpose these 2????
                    sigmapts[j + 3, jj] = veci[j] + s[j + 3, i];

                    // ---- find negative direction vectors
                    sigmapts[j, jj + 1] = reci[j] - s[j, i];
                    sigmapts[j + 3, jj + 1] = veci[j] - s[j + 3, i];
                }
            } // for i
        }  // posvelcov2pts


        /* ---------------------------------------------------------------------------- 
        *
        *                           function poscov2pts
        *
        *  finds 12 sigma points given position and velocity and covariance information
        *  using cholesky matrix square root algorithm
        *  then progates covariance points
        *
        *  author        : sal alfano      719-573-2600   31 mar 2011
        *
        *  revisions
        *                  dave vallado   make single routine to simply find sigma
        *                  points
        *
        *  inputs          description 
        *    cov         - eci 3x3 covariance matrix      km or m
        *    reci        - eci 3x1 position vector        km or m
        *
        *  outputs       :
        *    sigmapts    - structure of sigma points (3 x 6)
        *
        *  locals        :
        *
        *  sigmapts = poscov2pts(reci, cov);
         ---------------------------------------------------------------------------- - */

        public void poscov2pts
            (
              double[] reci, double[,] cov, out double[,] sigmapts
            )
        {
            Int32 i, j, jj;
            double[,] s = new double[3, 3];
            // -------------------------  implementation   -----------------

            // initialize data & pre-allocate new points
            sigmapts = new double[,] { { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0 } }; // array is 3 x 6
            // compute matrix square root of nP
            s = MathTimeLibr.cholesky(cov);

            for (i = 0; i < 3; i++)
                for (j = 0; j < 3; j++)
                    s[i, j] = Math.Sqrt(3.0) * s[i, j];

            // perturb states, propagate to toff
            for (i = 0; i < 3; i++)
            {
                // ---- find positive direction vectors
                jj = (i - 1) * 2 + 2;  // incr this by 2
                for (j = 0; j < 3; j++)
                {
                    sigmapts[j, jj] = reci[j] + s[j, i];  // transpose these 2????

                    // ---- find negative direction vectors
                    sigmapts[j, jj + 1] = reci[j] - s[j, i];
                }
            } // for i
        }  // poscov2pts



        /* ---------------------------------------------------------------------------- 
        *
        *                           function remakecovpv
        *
        *  takes propagated perturbed points from square root algorithm
        *  and finds mean and covariance
        *
        *  author        : sal alfano      719-573-2600   7 apr 2010
        *
        *  revisions
        *                -
        *
        *  inputs          description 
        *    pts            Array of propagated points from square root algorithm
        *
        *  outputs       :
        *    cov             n_dim x n_dim covariance matrix (m)
        *    yu             1 x n_dim mean vector (m)
        *
        *  locals        :
        *    y              1 x n_dim mean shifted vector (m)
        *    n_dim          dimension of vector
        *    n_pts          total number of points
         ---------------------------------------------------------------------------- - */

        public void remakecovpv
            (
               double[,] sigmapts, out double[] yu, out double[,] cov
            )
        {
            Int32 i, j;
            double oo12;

            oo12 = 1.0 / 12.0;
            yu = new double[] { 0, 0, 0, 0, 0, 0 };

            // -------------------------  implementation   -----------------
            // initialize data & pre-allocate matrices
            double[,] y = new double[6, 12];
            double[,] tmp = new double[6, 6];

            // find mean
            for (i = 0; i < 12; i++)
                for (j = 0; j < 6; j++)
                    yu[j] = yu[j] + sigmapts[j, i];

            for (j = 0; j < 6; j++)
                yu[j] = yu[j] * oo12;

            // find covariance
            for (i = 0; i < 12; i++)
                for (j = 0; j < 6; j++)
                    y[j, i] = sigmapts[j, i] - yu[j];

            double[,] yt = MathTimeLibr.mattransx(y, 6, 12);
            tmp = MathTimeLibr.matmult(y, yt, 6, 12, 6);
            cov = MathTimeLibr.matscale(tmp, 6, 6, oo12);

            // cov = MathTimeLibr.matmult(y, tmp, 6, 6, 6);
            //cov = (tmp + tmp) * 0.5;  //tmp*tmp'? // ensures perfect symmetry
        } // remakecovpv   



        /* ---------------------------------------------------------------------------- 
        *
        *                           function remakecovp
        *
        *  takes propagated perturbed points from square root algorithm
        *  and finds mean and covariance
        *
        *  author        : sal alfano      719-573-2600   7 apr 2010
        *
        *  revisions
        *
        *  inputs          description 
        *    pts            Array of propagated points from square root algorithm
        *
        *  outputs       :
        *    cov             n_dim x n_dim covariance matrix (m)
        *    yu             1 x n_dim mean vector (m)
        *
        *  locals        :
        *    y              1 x n_dim mean shifted vector (m)
        *    n_dim          dimension of vector
        *    n_pts          total number of points
         ---------------------------------------------------------------------------- - */

        public void remakecovp
            (
               double[,] sigmapts, out double[] yu, out double[,] cov
            )
        {
            Int32 i, j;
            double oo6;

            oo6 = 1.0 / 6.0;
            yu = new double[] { 0, 0, 0 };

            // -------------------------  implementation   -----------------
            // initialize data & pre-allocate matrices
            double[,] y = new double[3, 6];
            double[,] tmp = new double[3, 3];

            // find mean
            for (i = 0; i < 6; i++)
                for (j = 0; j < 3; j++)
                    yu[j] = yu[j] + sigmapts[j, i];

            for (j = 0; j < 3; j++)
                yu[j] = yu[j] * oo6;

            // find covariance
            for (i = 0; i < 6; i++)
                for (j = 0; j < 3; j++)
                    y[j, i] = sigmapts[j, i] - yu[j];

            double[,] yt = MathTimeLibr.mattransx(y, 3, 6);
            tmp = MathTimeLibr.matmult(y, yt, 3, 6, 3);
            cov = MathTimeLibr.matscale(tmp, 3, 3, oo6);
        } // remakecovp  


        /* ----------------------------------------------------------------------------
        *
        *                           function covct2cl
        *
        *  this function transforms a six by six covariance matrix expressed in cartesian elements
        *    into one expressed in classical elements
        *
        *  author        : david vallado           davallado@gmail.com   21 jun 2002
        *
        *  revisions
        *    vallado     - major update                                  26 aug 2015
        *
        *  inputs          description                          range / units
        *    cartcov     - 6x6 cartesian covariance matrix         m, m/s
        *    cartstate   - 6x1 cartesian orbit state            (x y z vx vy vz)  m, m/s
        *    anomclass   - anomaly                              'meana', 'truea'
        *
        *  outputs       :
        *    classcov    - 6x6 classical covariance matrix
        *    tm          - transformation matrix
        *
        *  locals        :
        *    r           - matrix of partial derivatives
        *    rj2000      - position vector                      km
        *    x,y,z       - components of position vector        km
        *    vj2000      - velocity vector                      km/s
        *    vx,vy,vz    - components of position vector        km/s
        *    p           - semilatus rectum                     km
        *    a           - semimajor axis                       km
        *    ecc         - eccentricity
        *    incl        - inclination                          0.0  to pi rad
        *    oMathTimeLibr.maga       - longitude of ascending node    0.0  to 2pi rad
        *    argp        - argument of perigee                  0.0  to 2pi rad
        *    nu          - true anomaly                         0.0  to 2pi rad
        *    m           - mean anomaly                         0.0  to 2pi rad
        *    arglat      - argument of latitude      (ci)       0.0  to 2pi rad
        *    truelon     - true longitude            (ce)       0.0  to 2pi rad
        *    lonper      - longitude of periapsis    (ee)       0.0  to 2pi rad
        *    magr        - magnitude of position vector         km
        *    magv        - magnitude of velocity vector         km/s
        *
        *  coupling      :
        *    rv2coe      - position and velocity vectors to classical elements
        *
        *  references    :
        *    Vallado and Alfano 2015
        *
        * ---------------------------------------------------------------------------- */

        public void covct2cl(double[,] cartcov, double[] cartstate, string anomclass, out double[,] classcov, out double[,] tm)
        {
            double p, a, n, ecc, incl, raan, argp, nu, mean, arglat, truelon, lonper,
                magr, magv, magr3, sqrt1me2, r_dot_v, rx, ry, rz, vx, vy, vz,
                ecc_term, ecc_x, ecc_y, ecc_z, hx, hy, hz, h, h_squared, nx, ny, nz, node, n_squared, n_dot_e,
                sign_anode, cos_raan, sign_w, cos_w, w_scale, r_dot_e, cos_nu, sign_nu, nu_scale, p0, p1, p3, p4, p5, dMde, dMdnu, temp;
            double[] reci = new double[3];
            double[] veci = new double[3];
            double[] recim = new double[3];
            double[] vecim = new double[3];
            double[] node_vec = new double[3];
            double[] h_vec = new double[3];
            double[] ecc_vec = new double[3];

            tm = new double[,] { { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 } };
            // -------- define gravitational constant
            //double gravConst.mum = 3.986004418e14;  // m3/s2

            // -------- parse the input vectors into cartesian and classical components
            rx = cartstate[0] * 1000.0;  // m
            ry = cartstate[1] * 1000.0;
            rz = cartstate[2] * 1000.0;
            vx = cartstate[3] * 1000.0;  // m/s
            vy = cartstate[4] * 1000.0;
            vz = cartstate[5] * 1000.0;
            reci = new double[] { rx, ry, rz };  // m
            veci = new double[] { vx, vy, vz };

            for(int i = 0; i<3; i++)  // in km
            {
                recim[i] = reci[i] * 0.001;
                vecim[i] = veci[i] * 0.001;
            }

            // -------- convert to a classical orbit state for ease of computation
            rv2coe(recim, vecim, out p, out a, out ecc, out incl, out raan, out argp, out nu, out mean, out arglat, out truelon, out lonper);
            p = p * 1000.0;  // m
            a = a * 1000.0;
            n = Math.Sqrt(gravConst.mum / (a * a * a));

            // -------- calculate common quantities
            sqrt1me2 = Math.Sqrt(1.0 - ecc * ecc);
            magr = MathTimeLibr.mag(reci);  // m
            magr3 = Math.Pow(magr, 3);
            magv = MathTimeLibr.mag(veci);

            // ----------  form pqw position and velocity vectors ----------  
            r_dot_v = MathTimeLibr.dot(reci, veci);  // *1000.0 * 1000.0;

            ecc_term = magv * magv - gravConst.mum / magr;
            ecc_x = (ecc_term * rx - r_dot_v * vx) / gravConst.mum;
            ecc_y = (ecc_term * ry - r_dot_v * vy) / gravConst.mum;
            ecc_z = (ecc_term * rz - r_dot_v * vz) / gravConst.mum;
            ecc_vec = new double[] { ecc_x, ecc_y, ecc_z };

            hx = ry * vz - rz * vy;  // m
            hy = rz * vx - rx * vz;
            hz = rx * vy - ry * vx;
            h_vec = new double[] { hx, hy, hz };
            h = MathTimeLibr.mag(h_vec);
            h_squared = h * h;

            nx = -hy;
            ny = hx;
            nz = 0.0;
            node_vec = new double[] { nx, ny, nz };
            node = MathTimeLibr.mag(node_vec);
            n_squared = node * node;
            n_dot_e = MathTimeLibr.dot(node_vec, ecc_vec);

            sign_anode = Math.Sign(ny);
            cos_raan = nx / node;
            raan = sign_anode * Math.Acos(cos_raan);

            sign_w = Math.Sign((magv * magv - gravConst.mum / magr) * rz - r_dot_v * vz);
            cos_w = n_dot_e / (ecc * node);
            argp = sign_w * Math.Acos(cos_w);
            w_scale = -sign_w / Math.Sqrt(1.0 - cos_w * cos_w);

            r_dot_e = MathTimeLibr.dot(reci, ecc_vec);
            cos_nu = r_dot_e / (magr * ecc);
            sign_nu = Math.Sign(r_dot_v);
            nu = sign_nu * Math.Acos(cos_nu);
            nu_scale = -sign_nu / Math.Sqrt(1.0 - cos_nu * cos_nu);

            // ---------------- calculate matrix elements ------------------
            // ---- partials of a wrt (rx ry rz vx vy vz)
            p0 = 2.0 * a * a / Math.Pow(magr, 3);
            p1 = 2.0 / (n * n * a);
            //            p1 = -0.5 / a;  /// ????????????? no
            tm[0, 0] = p0 * rx;
            tm[0, 1] = p0 * ry;
            tm[0, 2] = p0 * rz;
            tm[0, 3] = p1 * vx;
            tm[0, 4] = p1 * vy;
            tm[0, 5] = p1 * vz;

            // ---- partials of ecc wrt (rx ry rz vx vy vz)
            p0 = 1.0 / (gravConst.mum * ecc);
            tm[1, 0] = -p0 * (((vx * vy - gravConst.mum * rx * ry / magr3) * ecc_y) + ((vx * vz - gravConst.mum * rx * rz / magr3) * ecc_z) -
                (vy * vy + vz * vz - gravConst.mum / magr + gravConst.mum * rx * rx / magr3) * ecc_x);
            tm[1, 1] = -p0 * (((vx * vy - gravConst.mum * rx * ry / magr3) * ecc_x) + ((vy * vz - gravConst.mum * ry * rz / magr3) * ecc_z) -
                (vx * vx + vz * vz - gravConst.mum / magr + gravConst.mum * ry * ry / magr3) * ecc_y);
            tm[1, 2] = -p0 * (((vx * vz - gravConst.mum * rx * rz / magr3) * ecc_x) + ((vy * vz - gravConst.mum * ry * rz / magr3) * ecc_y) -
                (vy * vy + vx * vx - gravConst.mum / magr + gravConst.mum * rz * rz / magr3) * ecc_z);
            tm[1, 3] = -p0 * ((rx * vy - 2.0 * ry * vx) * ecc_y + (ry * vy + rz * vz) * ecc_x + (rx * vz - 2.0 * rz * vx) * ecc_z);
            tm[1, 4] = -p0 * ((ry * vx - 2.0 * rx * vy) * ecc_x + (rx * vx + rz * vz) * ecc_y + (ry * vz - 2.0 * rz * vy) * ecc_z);
            tm[1, 5] = -p0 * ((rx * vx + ry * vy) * ecc_z + (rz * vx - 2.0 * rx * vz) * ecc_x + (rz * vy - 2.0 * ry * vz) * ecc_y);

            // ---- partials of incl wrt (rx ry rz vx vy vz)
            p3 = 1.0 / node;
            tm[2, 0] = -p3 * (vy - hz * (vy * hz - vz * hy) / h_squared);
            tm[2, 1] = p3 * (vx - hz * (vx * hz - vz * hx) / h_squared);
            tm[2, 2] = -p3 * (hz * (vy * hx - vx * hy) / h_squared);
            tm[2, 3] = p3 * (ry - hz * (ry * hz - rz * hy) / h_squared);
            tm[2, 4] = -p3 * (rx - hz * (rx * hz - rz * hx) / h_squared);
            tm[2, 5] = p3 * (hz * (ry * hx - rx * hy) / h_squared);

            // ---- partials of node wrt (rx ry rz vx vy vz)
            p4 = 1.0 / n_squared;
            tm[3, 0] = -p4 * vz * ny;
            tm[3, 1] = p4 * vz * nx;
            tm[3, 2] = p4 * (vx * ny - vy * nx);
            tm[3, 3] = p4 * rz * ny;
            tm[3, 4] = -p4 * rz * nx;
            tm[3, 5] = p4 * (ry * nx - rx * ny);

            // ---- partials of argp wrt (rx ry rz vx vy vz)
            p5 = 1.0 / (node * a * a);
            temp = -hy * (vy * vy + vz * vz - gravConst.mum / magr + gravConst.mum * rx * rx / magr3);
            temp = temp - hx * (vx * vy - gravConst.mum * rx * ry / magr3) + vz * gravConst.mum * ecc_x;
            temp = temp / (gravConst.mum * node * ecc) + vz * hy * n_dot_e / (node * node * node * ecc) - tm[1, 0] * n_dot_e / (node * ecc * ecc);
            tm[4, 0] = temp * w_scale;
            temp = hx * (vx * vx + vz * vz - gravConst.mum / magr + gravConst.mum * ry * ry / magr3);
            temp = temp + hy * (vx * vy - gravConst.mum * rx * ry / magr3) + vz * gravConst.mum * ecc_y;
            temp = temp / (gravConst.mum * node * ecc) - vz * hx * n_dot_e / (node * node * node * ecc) - tm[1, 1] * n_dot_e / (node * ecc * ecc);
            tm[4, 1] = temp * w_scale;
            temp = -hy * (vx * vz - gravConst.mum * rx * rz / magr3) + hx * (vy * vz - gravConst.mum * ry * rz / magr3) + vx * gravConst.mum * ecc_x + vy * gravConst.mum * ecc_y;
            temp = -temp / (gravConst.mum * node * ecc) + (vy * hx - vx * hy) * n_dot_e / (node * node * node * ecc) - tm[1, 2] * n_dot_e / (node * ecc * ecc);
            tm[4, 2] = temp * w_scale;
            temp = (rx * vy - 2.0 * ry * vx) * hx - hy * (ry * vy + rz * vz) + rz * gravConst.mum * ecc_x;
            temp = -temp / (gravConst.mum * node * ecc) - rz * hy * n_dot_e / (node * node * node * ecc) - tm[1, 3] * n_dot_e / (node * ecc * ecc);
            tm[4, 3] = temp * w_scale;
            temp = -(ry * vx - 2.0 * rx * vy) * hy + hx * (rx * vx + rz * vz) + rz * gravConst.mum * ecc_y;
            temp = -temp / (gravConst.mum * node * ecc) + rz * hx * n_dot_e / (node * node * node * ecc) - tm[1, 4] * n_dot_e / (node * ecc * ecc);
            tm[4, 4] = temp * w_scale;
            temp = -(rz * vx - 2.0 * rx * vz) * hy + hx * (rz * vy - 2.0 * ry * vz) - rx * gravConst.mum * ecc_x - ry * gravConst.mum * ecc_y;
            temp = -temp / (gravConst.mum * node * ecc) + (rx * hy - ry * hx) * n_dot_e / (node * node * node * ecc) - tm[1, 5] * n_dot_e / (node * ecc * ecc);
            tm[4, 5] = temp * w_scale;

            // ---- partials of true anomaly wrt (rx ry rz vx vy vz)
            temp = ry * (vx * vy - gravConst.mum * rx * ry / magr3) - rx * ecc_term + rz * (vx * vz - gravConst.mum * rx * rz / magr3);
            temp = temp - rx * (vy * vy + vz * vz - gravConst.mum / magr + gravConst.mum * rx * rx / magr3) + vx * r_dot_v;
            temp = -temp / (gravConst.mum * magr * ecc) - rx * r_dot_e / (magr3 * ecc) - tm[1, 0] * r_dot_e / (magr * ecc * ecc);
            tm[5, 0] = temp * nu_scale;
            temp = rx * (vx * vy - gravConst.mum * rx * ry / magr3) - ry * ecc_term + rz * (vy * vz - gravConst.mum * ry * rz / magr3);
            temp = temp - ry * (vx * vx + vz * vz - gravConst.mum / magr + gravConst.mum * ry * ry / magr3) + vy * r_dot_v;
            temp = -temp / (gravConst.mum * magr * ecc) - ry * r_dot_e / (magr3 * ecc) - tm[1, 1] * r_dot_e / (magr * ecc * ecc);
            tm[5, 1] = temp * nu_scale;
            temp = rx * (vx * vz - gravConst.mum * rx * rz / magr3) - rz * ecc_term + ry * (vy * vz - gravConst.mum * ry * rz / magr3);
            temp = temp - rz * (vx * vx + vy * vy - gravConst.mum / magr + gravConst.mum * rz * rz / magr3) + vz * r_dot_v;
            temp = -temp / (gravConst.mum * magr * ecc) - rz * r_dot_e / (magr3 * ecc) - tm[1, 2] * r_dot_e / (magr * ecc * ecc);
            tm[5, 2] = temp * nu_scale;
            temp = ry * (rx * vy - 2.0 * ry * vx) + rx * (ry * vy + rz * vz) + rz * (rx * vz - 2.0 * rz * vx);
            temp = -temp / (gravConst.mum * magr * ecc) - tm[1, 3] * r_dot_e / (magr * ecc * ecc);
            tm[5, 3] = temp * nu_scale;
            temp = rx * (ry * vx - 2.0 * rx * vy) + ry * (rx * vx + rz * vz) + rz * (ry * vz - 2.0 * rz * vy);
            temp = -temp / (gravConst.mum * magr * ecc) - tm[1, 4] * r_dot_e / (magr * ecc * ecc);
            tm[5, 4] = temp * nu_scale;
            temp = rz * (rx * vx + ry * vy) + rx * (rz * vx - 2.0 * rx * vz) + ry * (rz * vy - 2.0 * ry * vz);
            temp = -temp / (gravConst.mum * magr * ecc) - tm[1, 5] * r_dot_e / (magr * ecc * ecc);
            tm[5, 5] = temp * nu_scale;

            //       // same answers as above
            //       // ---- partials of true anomaly wrt (rx ry rz vx vy vz)
            //       p8 = magr*magr*magv*magv - gravConst.mum*magr - r_dot_v*r_dot_v;
            //       p9 = 1.0/(p8*p8 + r_dot_v*r_dot_v * h*h);
            //       tm[5,0] = p9 * ( p8 * (h*vx + r_dot_v*(vy*hz - vz*hy)/h) - r_dot_v*h*(2*rx*magv^2 - gravConst.mum*rx/magr - 2*r_dot_v*vx) );
            //       tm[5,1] = p9 * ( p8 * (h*vy + r_dot_v*(vz*hx - vx*hz)/h) - r_dot_v*h*(2*ry*magv^2 - gravConst.mum*ry/magr - 2*r_dot_v*vy) );
            //       tm[5,2] = p9 * ( p8 * (h*vz + r_dot_v*(vx*hy - vy*hx)/h) - r_dot_v*h*(2*rz*magv^2 - gravConst.mum*rz/magr - 2*r_dot_v*vz) );
            //       tm[5,3] = p9 * ( p8 * (h*rx + r_dot_v*(rz*hy - ry*hz)/h) - r_dot_v*h*(2*vx*magr^2 - 2*r_dot_v*rx) );
            //       tm[5,4] = p9 * ( p8 * (h*ry + r_dot_v*(rx*hz - rz*hx)/h) - r_dot_v*h*(2*vy*magr^2 - 2*r_dot_v*ry) );
            //       tm[5,5] = p9 * ( p8 * (h*rz + r_dot_v*(ry*hx - rx*hy)/h) - r_dot_v*h*(2*vz*magr^2 - 2*r_dot_v*rz) );

            if (anomclass.Contains("mean"))
            {
                // ---- partials of mean anomaly wrt (rx ry rz vx vy vz)
                // then update for mean anomaly
                ecc = MathTimeLibr.mag(ecc_vec);
                dMdnu = Math.Pow(1.0 - ecc * ecc, 1.5) / (Math.Pow(1.0 + ecc * cos_nu, 2));  // dm/dv
                dMde = -Math.Sin(nu) * ((ecc * cos_nu + 1.0) * (ecc + cos_nu) / Math.Sqrt(Math.Pow(ecc + cos_nu, 2)) +
                    1.0 - 2.0 * ecc * ecc - ecc * ecc * ecc * cos_nu) / (Math.Pow(ecc * cos_nu + 1.0, 2) * Math.Sqrt(1.0 - ecc * ecc));  // dm/de
                tm[5, 0] = tm[5, 0] * dMdnu + tm[1, 0] * dMde;
                tm[5, 1] = tm[5, 1] * dMdnu + tm[1, 1] * dMde;
                tm[5, 2] = tm[5, 2] * dMdnu + tm[1, 2] * dMde;
                tm[5, 3] = tm[5, 3] * dMdnu + tm[1, 3] * dMde;
                tm[5, 4] = tm[5, 4] * dMdnu + tm[1, 4] * dMde;
                tm[5, 5] = tm[5, 5] * dMdnu + tm[1, 5] * dMde;
            }

            // ---------- calculate the output covariance matrix -----------
            double[,] tmt = MathTimeLibr.mattrans(tm, 6);
            double[,] tempm = MathTimeLibr.matmult(cartcov, tmt, 6, 6, 6);
            classcov = MathTimeLibr.matmult(tm, tempm, 6, 6, 6);
        }  //  covct2cl


        /* ----------------------------------------------------------------------------
        *
        *                           function covcl2ct
        *
        *  this function transforms a six by six covariance matrix expressed in classical elements
        *    into one expressed in cartesian elements
        *
        *  author        : david vallado
        *
        *  revisions
        *    vallado     - simplify code using pqw-eci transformation    12 may 2017
        *    vallado     - major update                                  26 aug 2015
        *
        *  inputs          description                          range / units
        *    classcov    - 6x6 classical covariance matrix space delimited
        *    classstate  - 6x1 classical orbit state            (a e i O w nu/m)
        *    anomclass   - anomaly                              'meana', 'truea'
        *
        *  outputs       :
        *    cartcov     - 6x6 cartesian covariance matrix
        *    tm          - transformation matrix
        *
        *  locals        :
        *    r           - matrix of partial derivatives
        *    a           - semimajor axis                       m
        *    ecc         - eccentricity
        *    incl        - inclination                          0.0  to pi rad
        *    oMathTimeLibr.maga       - longitude of asc}ing node      0.0  to 2pi rad
        *    argp        - argument of perigee                  0.0  to 2pi rad
        *    nu          - true anomaly                         0.0  to 2pi rad
        *    m           - mean anomaly                         0.0  to 2pi rad
        *    p1,p2,p3,p4 - denominator terms for the partials
        *    eccanom          - eccentric anomaly                    0.0  to 2pi rad
        *    true1, true2- temp true anomaly                    0.0  to 2pi rad
        *
        *  coupling      :
        *    newtonm     - newton iteration for m and ecc to nu
        *    newtonnu    - newton iteration for nu and ecc to m
        *
        *  references    :
        *    Vallado and Alfano 2015
        *
        * ---------------------------------------------------------------------------- */

        public void covcl2ct
            (double[,] classcov, double[] classstate, string anomclass, out double[,] cartcov, out double[,] tm
            )
        {
            double p, a, n, ecc, incl, raan, argp, nu, mean, eccanom, e,
                rx, ry, rz, vx, vy, vz,
                sin_inc, cos_inc, sin_raan, cos_raan, sin_w, cos_w, sin_nu, cos_nu,
                p11, p12, p13, p21, p22, p23, p31, p32, p33, p0, p1, p2, p3, p4, p5, p6, dMdnu, dMde, temp;
            double[] r = new double[3];
            double[] reci = new double[3];
            double[] v = new double[3];
            double[] veci = new double[3];
            double[] node_vec = new double[3];
            double[] h_vec = new double[3];
            double[] ecc_vec = new double[3];
            tm = new double[,] { { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0 } };

            // -------- define gravitational constant
            //double mum = 3.986004415e14;  // m3/s2

            // --------- determine which set of variables is in use ---------
            // ---- parse the input vector into the classical elements -----
            a = classstate[0] * 1000.0;  // m
            n = Math.Sqrt(gravConst.mum / (a * a * a));
            ecc = classstate[1];
            incl = classstate[2];
            raan = classstate[3];
            argp = classstate[4];
            nu = 0.0; // set initially
            // -------- if mean anomaly is used, convert to true anomaly
            // -------- eccentric anomaly (e) is needed for both
            if (anomclass.Contains("mean"))
            {
                mean = classstate[5];
                newtonm(ecc, mean, out eccanom, out nu);
            }
            else
            {
                // note that mean is not used in the partials, but nu is! 
                nu = classstate[5];
                newtonnu(ecc, nu, out e, out mean);
            }

            p = a * (1.0 - ecc * ecc) * 0.001;  // needs to be in km
            coe2rv(p, ecc, incl, raan, argp, nu, 0.0, 0.0, 0.0, out r, out v);
            rx = r[0] * 1000.0;  // m
            ry = r[1] * 1000.0;
            rz = r[2] * 1000.0;
            vx = v[0] * 1000.0;
            vy = v[1] * 1000.0;
            vz = v[2] * 1000.0;

            // assign trig values for efficiency
            sin_inc = Math.Sin(incl);
            cos_inc = Math.Cos(incl);
            sin_raan = Math.Sin(raan);
            cos_raan = Math.Cos(raan);
            sin_w = Math.Sin(argp);
            cos_w = Math.Cos(argp);
            sin_nu = Math.Sin(nu);
            cos_nu = Math.Cos(nu);

            // assign elements of PQW to ECI transformation (pg 168)
            p11 = cos_raan * cos_w - sin_raan * sin_w * cos_inc;
            p12 = -cos_raan * sin_w - sin_raan * cos_w * cos_inc;
            p13 = sin_raan * sin_inc;
            p21 = sin_raan * cos_w + cos_raan * sin_w * cos_inc;
            p22 = -sin_raan * sin_w + cos_raan * cos_w * cos_inc;
            p23 = -cos_raan * sin_inc;
            p31 = sin_w * sin_inc;
            p32 = cos_w * sin_inc;
            p33 = cos_inc;

            // assign constants for efficiency
            temp = Math.Pow(1.0 + ecc * cos_nu, 2);
            p0 = Math.Sqrt(gravConst.mum / (a * (1.0 - ecc * ecc)));
            p1 = (1.0 - ecc * ecc) / (1.0 + ecc * cos_nu);
            p2 = 1.0 / (2.0 * a) * p0;
            p3 = (2.0 * a * ecc + a * cos_nu + a * cos_nu * ecc * ecc) / temp;
            p4 = ecc * gravConst.mum / (a * Math.Pow(1.0 - ecc * ecc, 2) * p0);
            p5 = a * p1;
            p6 = a * (1.0 - ecc * ecc) / (temp);

            dMdnu = Math.Pow(1.0 - ecc * ecc, 1.5) / (Math.Pow(1.0 + ecc * cos_nu, 2));  // dm/dv
            dMde = -sin_nu * ((ecc * cos_nu + 1.0) * (ecc + cos_nu) / Math.Sqrt(Math.Pow(ecc + cos_nu, 2)) + 1.0 - 2.0 * ecc * ecc - ecc * ecc * ecc * cos_nu) /
                (Math.Pow(ecc * cos_nu + 1.0, 2) * Math.Sqrt(1.0 - ecc * ecc));  // dm/de   

            // ---------------- calculate matrix elements ------------------
            // ---- partials of rx wrt (a e i O w m)
            tm[0, 0] = p1 * (p11 * cos_nu + p12 * sin_nu);
            //tm[0, 0] = rx / a; // alternate approach
            tm[0, 1] = -p3 * (p11 * cos_nu + p12 * sin_nu);
            tm[0, 2] = p5 * p13 * (sin_w * cos_nu + cos_w * sin_nu);
            tm[0, 3] = -p5 * (p21 * cos_nu + p22 * sin_nu);
            tm[0, 4] = p5 * (p12 * cos_nu - p11 * sin_nu);
            //p10 = a * (ecc * ecc - 1.0) / Math.Pow(ecc * cos_nu + 1.0, 2);
            //tm[0, 5] = p10 * (ecc * Math.Cos(raan) * sin_w + cos_raan * cos_w * sin_nu +
            //    cos_raan * sin_w * cos_nu + ecc * cos_inc * sin_raan * cos_w +
            //    cos_inc * sin_raan * cos_w * cos_nu - cos_inc * sin_raan * sin_w * sin_nu);
            tm[0, 5] = p6 * (-p11 * sin_nu + p12 * (ecc + cos_nu));
            if (anomclass.Contains("mean"))
            {
                tm[0, 5] = tm[0, 5] / dMdnu;
                tm[0, 1] = tm[0, 1] - tm[0, 5] * dMde;
            }

            // ---- partials of ry wrt (a e i O w nu/m)
            tm[1, 0] = p1 * (p21 * cos_nu + p22 * sin_nu);
            tm[1, 0] = ry / a;
            tm[1, 1] = -p3 * (p21 * cos_nu + p22 * sin_nu);
            tm[1, 2] = p5 * p23 * (sin_w * cos_nu + cos_w * sin_nu);
            tm[1, 3] = p5 * (p11 * cos_nu + p12 * sin_nu);
            tm[1, 4] = p5 * (p22 * cos_nu - p21 * sin_nu);
            //p10 = a * (ecc * ecc - 1.0) / Math.Pow(ecc * cos_nu + 1.0, 2);
            //tm[1, 5] = p10 * (ecc * sin_raan * sin_w + sin_raan * cos_w * sin_nu +
            //    sin_raan * sin_w * cos_nu - ecc * cos_inc * cos_raan * cos_w -
            //    cos_inc * cos_raan * cos_w * cos_nu + cos_inc * cos_raan * sin_w * sin_nu);
            tm[1, 5] = p6 * (-p21 * sin_nu + p22 * (ecc + cos_nu));
            if (anomclass.Contains("mean"))
            {
                tm[1, 5] = tm[1, 5] / dMdnu;
                tm[1, 1] = tm[1, 1] - tm[1, 5] * dMde;
            }

            // ---- partials of rz wrt (a e i O w nu/m)
            tm[2, 0] = p1 * (p31 * cos_nu + p32 * sin_nu);
            tm[2, 0] = rz / a;
            tm[2, 1] = -p3 * (p31 * cos_nu + p32 * sin_nu);   // old sin_inc * (cos_w * sin_nu + sin_w * cos_nu);
            tm[2, 2] = p5 * cos_inc * (cos_w * sin_nu + sin_w * cos_nu);
            tm[2, 3] = 0.0;
            tm[2, 4] = p5 * sin_inc * (cos_w * cos_nu - sin_w * sin_nu);
            //p10 = -a * (ecc * ecc - 1.0) / Math.Pow(ecc * cos_nu + 1.0, 2);
            //tm[2, 5] = p10 * Math.Sin(incl) * (Math.Cos(argp + nu) + ecc * cos_w);
            tm[2, 5] = p6 * (-p31 * sin_nu + p32 * (ecc + cos_nu));
            if (anomclass.Contains("mean"))
            {
                tm[2, 5] = tm[2, 5] / dMdnu;
                tm[2, 1] = tm[2, 1] - tm[2, 5] * dMde;
            }

            // ---- partials of vx wrt (a e i O w nu/m)
            tm[3, 0] = p2 * (p11 * sin_nu - p12 * (ecc + cos_nu));
            tm[3, 0] = -vx / (2.0 * a);
            tm[3, 1] = -p4 * (p11 * sin_nu - p12 * (ecc + cos_nu)) + p12 * p0;
            tm[3, 2] = -p0 * sin_raan * (p31 * sin_nu - p32 * (ecc + cos_nu));
            tm[3, 3] = p0 * (p21 * sin_nu - p22 * (ecc + cos_nu));
            tm[3, 4] = -p0 * (p12 * sin_nu + p11 * (ecc + cos_nu));
            //p10 = Math.Sqrt(-gravConst.mum / (a * (ecc * ecc - 1.0)));
            //tm[3, 5] = p10 * (Math.Cos(raan) * sin_w * sin_nu - Math.Cos(raan) * cos_w * cos_nu +
            //    cos_inc * sin_raan * cos_w * sin_nu + cos_inc * sin_raan * sin_w * cos_nu);
            tm[3, 5] = -p0 * (p11 * cos_nu + p12 * sin_nu);
            if (anomclass.Contains("mean"))
            {
                tm[3, 5] = tm[3, 5] / dMdnu;
                tm[3, 1] = tm[3, 1] - tm[3, 5] * dMde;
            }

            // ---- partials of vy wrt (a e i O w nu/m)
            tm[4, 0] = p2 * (p21 * sin_nu - p22 * (ecc + cos_nu));
            tm[4, 0] = -vy / (2.0 * a);
            tm[4, 1] = -p4 * (p21 * sin_nu - p22 * (ecc + cos_nu)) + p22 * p0;
            tm[4, 2] = p0 * cos_raan * (p31 * sin_nu - p32 * (ecc + cos_nu));
            tm[4, 3] = p0 * (-p11 * sin_nu + p12 * (ecc + cos_nu));
            tm[4, 4] = -p0 * (p22 * sin_nu + p21 * (ecc + cos_nu));
            //p10 = Math.Sqrt(-gravConst.mum / (a * (ecc * ecc - 1.0)));
            //tm[4, 5] = -p10 * (sin_raan * cos_w * cos_nu - sin_raan * sin_w * sin_nu +
            //    cos_inc * Math.Cos(raan) * cos_w * sin_nu + cos_inc * Math.Cos(raan) * sin_w * cos_nu);
            tm[4, 5] = -p0 * (p21 * cos_nu + p22 * sin_nu);
            if (anomclass.Contains("mean"))
            {
                tm[4, 5] = tm[4, 5] / dMdnu;
                tm[4, 1] = tm[4, 1] - tm[4, 5] * dMde;
            }

            // ---- partials of vz wrt (a e i O w nu/m)
            tm[5, 0] = p2 * (p31 * sin_nu - p32 * (ecc + cos_nu));
            tm[5, 0] = -vz / (2.0 * a);
            tm[5, 1] = -p4 * (p31 * sin_nu - p32 * (ecc + cos_nu)) + p32 * p0;
            tm[5, 2] = p0 * cos_inc * (cos_w * cos_nu - sin_w * sin_nu + ecc * cos_w);
            tm[5, 3] = 0.0;
            tm[5, 4] = -p0 * (p32 * sin_nu + p31 * (ecc + cos_nu));
            //p10 = Math.Sqrt(-gravConst.mum / (a * (ecc * ecc - 1.0)));
            //tm[5, 5] = p10 * (-Math.Sin(incl) * Math.Sin(argp + nu));
            tm[5, 5] = -p0 * (p31 * cos_nu + p32 * sin_nu);
            if (anomclass.Contains("mean"))
            {
                tm[5, 5] = tm[5, 5] / dMdnu;
                tm[5, 1] = tm[5, 1] - tm[5, 5] * dMde;
            }

            // ---------- calculate the output covariance matrix -----------
            double[,] tmt = MathTimeLibr.mattrans(tm, 6);
            double[,] tempm = MathTimeLibr.matmult(classcov, tmt, 6, 6, 6);
            cartcov = MathTimeLibr.matmult(tm, tempm, 6, 6, 6);
        }  //  covcl2ct


        /* ----------------------------------------------------------------------------
        *
        *                           function covct2eq
        *
        *  this function transforms a six by six covariance matrix expressed in
        *    cartesian vectors into one expressed in equinoctial elements.
        *
        *  author        : david vallado           davallado@gmail.com   14 jul 2002
        *
        *  revisions
        *    vallado     - major update                                  26 aug 2015
        *
        *  inputs          description                          range / units
        *    cartcov     - 6x6 cartesian covariance matrix       m, m/s
        *    cartstate   - 6x1 cartesian orbit state            (x y z vx vy vz)  m, m/s
        *    anomeq      - anomaly                              'meana', 'truea', 'meann', 'truen', 'meanp', 'truep'
        *    fr          - retrograde factor                     +1, -1
        *
        *  outputs       :
        *    eqcov       - 6x6 equinoctial covariance matrix
        *    tm          - transformation matrix
        *
        *  locals        :
        *    r           - matrix of partial derivatives
        *    a           - semimajor axis                       m
        *    ecc         - eccentricity
        *    incl        - inclination                          0.0  to pi rad
        *    oMathTimeLibr.maga       - longitude of ascending node    0.0  to 2pi rad
        *    argp        - argument of perigee                  0.0  to 2pi rad
        *    nu          - true anomaly                         0.0  to 2pi rad
        *    m           - mean anomaly                         0.0  to 2pi rad
        *    eccanom          - eccentric anomaly                    0.0  to 2pi rad
        *    tau         - time from perigee passage
        *    n           - mean motion                          rad
        *    af          - component of ecc vector
        *    ag          - component of ecc vector
        *    chi         - component of node vector in eqw
        *    psi         - component of node vector in eqw
        *    meanlon     - mean longitude                       rad
        *
        *  coupling      :
        *    constastro
        *
        *  references    :
        *    Vallado and Alfano 2015
        * ---------------------------------------------------------------------------- */

        public void covct2eq
            (
            double[,] cartcov, double[] cartstate, string anomeq, Int16 fr, out double[,] eqcov, out double[,] tm
            )
        {
            double p, a, n, af, ag, chi, psi, X, Y, b,
                magr, magv, r_dot_v, rx, ry, rz, vx, vy, vz,
                ecc_x, ecc_y, ecc_z, hx, hy, hz, p0, p1,
                fe, fq, fw, ge, gq, gw, we, wq, ww, tm34, tm35, tm36, tm24, tm25, tm26,
                cosF, sinF, A, B, C, F, XD, YD, partXDaf, partYDaf, partXDag, partYDag;
            double[] reci = new double[3];
            double[] veci = new double[3];
            double[] node_vec = new double[3];
            double[] h_vec = new double[3];
            double[] w_vec = new double[3];
            double[] f_vec = new double[3];
            double[] g_vec = new double[3];
            double[] ecc_vec = new double[3];
            tm = new double[,] { { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 } };

            // -------- define gravitational constant
            //double mum = 3.986004415e14;  // m3/s2
            double twopi = 2.0 * Math.PI;

            // -------- parse the input vectors into cartesian and classical components
            rx = cartstate[0] * 1000.0;  // m
            ry = cartstate[1] * 1000.0;
            rz = cartstate[2] * 1000.0;
            vx = cartstate[3] * 1000.0;  // m/s
            vy = cartstate[4] * 1000.0;
            vz = cartstate[5] * 1000.0;
            reci = new double[] { rx, ry, rz };
            veci = new double[] { vx, vy, vz };
            magr = MathTimeLibr.mag(reci); // m
            magv = MathTimeLibr.mag(veci); // m

            a = 1.0 / (2.0 / magr - magv * magv / gravConst.mum);  // m
            n = Math.Sqrt(gravConst.mum / (a * a * a));

            hx = ry * vz - rz * vy;
            hy = -rx * vz + rz * vx;
            hz = rx * vy - ry * vx;
            h_vec = new double[] { hx, hy, hz };
            //h_vec = MathTimeLibr.cross(reci, veci)  //same
            double magh = MathTimeLibr.mag(h_vec);
            w_vec[0] = h_vec[0] / magh;
            w_vec[1] = h_vec[1] / magh;
            w_vec[2] = h_vec[2] / magh;
            chi = w_vec[0] / (1.0 + fr * w_vec[2]);
            psi = -w_vec[1] / (1.0 + fr * w_vec[2]);

            // components of equinoctial system
            p0 = 1.0 / (1.0 + chi * chi + psi * psi);
            fe = p0 * (1.0 - chi * chi + psi * psi);
            fq = p0 * 2.0 * chi * psi;
            fw = p0 * -2.0 * fr * chi;
            ge = p0 * 2.0 * fr * chi * psi;
            gq = p0 * fr * (1.0 + chi * chi - psi * psi);
            gw = p0 * 2.0 * psi;
            f_vec = new double[] { fe, fq, fw };
            g_vec = new double[] { ge, gq, gw };
            we = w_vec[0];
            wq = w_vec[1];
            ww = w_vec[2];

            r_dot_v = MathTimeLibr.dot(reci, veci);
            p1 = magv * magv - gravConst.mum / magr;
            ecc_x = (p1 * rx - r_dot_v * vx) / gravConst.mum;
            ecc_y = (p1 * ry - r_dot_v * vy) / gravConst.mum;
            ecc_z = (p1 * rz - r_dot_v * vz) / gravConst.mum;
            ecc_vec = new double[] { ecc_x, ecc_y, ecc_z };

            af = MathTimeLibr.dot(ecc_vec, f_vec);
            ag = MathTimeLibr.dot(ecc_vec, g_vec);

            X = MathTimeLibr.dot(reci, f_vec);
            Y = MathTimeLibr.dot(reci, g_vec);

            b = 1.0 / (1.0 + Math.Sqrt(1.0 - af * af - ag * ag));
            p0 = 1.0 / (a * Math.Sqrt(1.0 - af * af - ag * ag));
            sinF = ag + p0 * ((1.0 - ag * ag * b) * Y - ag * af * b * X);
            cosF = af + p0 * ((1.0 - af * af * b) * X - ag * af * b * Y);
            F = Math.Atan2(sinF, cosF);
            if (F < 0.0)
                F = F + twopi;

            XD = n * a * a / magr * (af * ag * b * Math.Cos(F) - (1.0 - ag * ag * b) * Math.Sin(F));
            YD = n * a * a / magr * ((1.0 - af * af * b) * Math.Cos(F) - af * ag * b * Math.Sin(F));

            A = Math.Sqrt(gravConst.mum * a);
            B = Math.Sqrt(1.0 - ag * ag - af * af);
            C = 1.0 + chi * chi + psi * psi;

            partXDaf = a * XD * YD / (A * B) - A / (Math.Pow(magr, 3)) * (a * ag * X / (1.0 + B) + X * Y / B);
            partYDaf = -a * XD * XD / (A * B) - A / (Math.Pow(magr, 3)) * (a * ag * Y / (1.0 + B) - X * X / B);
            partXDag = a * YD * YD / (A * B) + A / (Math.Pow(magr, 3)) * (a * af * X / (1.0 + B) - Y * Y / B);
            partYDag = -a * XD * YD / (A * B) + A / (Math.Pow(magr, 3)) * (a * af * Y / (1.0 + B) + X * Y / B);

            // ---------------- calculate matrix elements ------------------
            // ---- partials of a wrt (rx ry rz vx vy vz)
            if (anomeq.Equals("truea") || anomeq.Equals("meana"))
            {
                p0 = 2.0 * a * a / Math.Pow(magr, 3);
                p1 = 2.0 / (n * n * a);
            }
            else
            {
                if (anomeq.Equals("truen") || anomeq.Equals("meann"))
                {
                    p0 = -3.0 * n * a / Math.Pow(magr, 3);
                    p1 = -3.0 / (n * a * a);
                }
                else
                    if (anomeq.Equals("truep") || anomeq.Equals("meanp"))
                {
                    double ecc = MathTimeLibr.mag(ecc_vec);
                    p = a * (1.0 - ecc * ecc);
                    p0 = 2.0 * p * p / Math.Pow(magr, 3);
                    p1 = 2.0 / (n * n * p);
                }
            }

            tm[0, 0] = p0 * rx;
            tm[0, 1] = p0 * ry;
            tm[0, 2] = p0 * rz;
            tm[0, 3] = p1 * vx;
            tm[0, 4] = p1 * vy;
            tm[0, 5] = p1 * vz;

            // ---- partials of v wrt ag
            tm34 = partXDag * fe + partYDag * ge;
            tm35 = partXDag * fq + partYDag * gq;
            tm36 = partXDag * fw + partYDag * gw;
            // ---- partials of af wrt (rx ry rz vx vy vz)
            p0 = 1.0 / gravConst.mum;
            tm[1, 0] = -a * b * af * B * rx / (Math.Pow(magr, 3)) - (ag * (chi * XD - psi * fr * YD) * we) / (A * B) + (B / A) * tm34;
            tm[1, 1] = -a * b * af * B * ry / (Math.Pow(magr, 3)) - (ag * (chi * XD - psi * fr * YD) * wq) / (A * B) + (B / A) * tm35;
            tm[1, 2] = -a * b * af * B * rz / (Math.Pow(magr, 3)) - (ag * (chi * XD - psi * fr * YD) * ww) / (A * B) + (B / A) * tm36;
            tm[1, 3] = p0 * ((2.0 * X * YD - XD * Y) * ge - Y * YD * fe) - (ag * (psi * fr * Y - chi * X) * we) / (A * B);
            tm[1, 4] = p0 * ((2.0 * X * YD - XD * Y) * gq - Y * YD * fq) - (ag * (psi * fr * Y - chi * X) * wq) / (A * B);
            tm[1, 5] = p0 * ((2.0 * X * YD - XD * Y) * gw - Y * YD * fw) - (ag * (psi * fr * Y - chi * X) * ww) / (A * B);

            // ---- partials of v wrt af
            tm24 = partXDaf * fe + partYDaf * ge;
            tm25 = partXDaf * fq + partYDaf * gq;
            tm26 = partXDaf * fw + partYDaf * gw;
            // ---- partials of ag wrt (rx ry rz vx vy vz)
            p0 = 1.0 / gravConst.mum;
            tm[2, 0] = -a * b * ag * B * rx / (Math.Pow(magr, 3)) + (af * (chi * XD - psi * fr * YD) * we) / (A * B) - (B / A) * tm24;
            tm[2, 1] = -a * b * ag * B * ry / (Math.Pow(magr, 3)) + (af * (chi * XD - psi * fr * YD) * wq) / (A * B) - (B / A) * tm25;
            tm[2, 2] = -a * b * ag * B * rz / (Math.Pow(magr, 3)) + (af * (chi * XD - psi * fr * YD) * ww) / (A * B) - (B / A) * tm26;
            tm[2, 3] = p0 * ((2.0 * XD * Y - X * YD) * fe - X * XD * ge) + (af * (psi * fr * Y - chi * X) * we) / (A * B);
            tm[2, 4] = p0 * ((2.0 * XD * Y - X * YD) * fq - X * XD * gq) + (af * (psi * fr * Y - chi * X) * wq) / (A * B);
            tm[2, 5] = p0 * ((2.0 * XD * Y - X * YD) * fw - X * XD * gw) + (af * (psi * fr * Y - chi * X) * ww) / (A * B);

            // ---- partials of chi wrt (rx ry rz vx vy vz)
            tm[3, 0] = -C * YD * we / (2.0 * A * B);
            tm[3, 1] = -C * YD * wq / (2.0 * A * B);
            tm[3, 2] = -C * YD * ww / (2.0 * A * B);
            tm[3, 3] = C * Y * we / (2.0 * A * B);
            tm[3, 4] = C * Y * wq / (2.0 * A * B);
            tm[3, 5] = C * Y * ww / (2.0 * A * B);

            // ---- partials of psi wrt (rx ry rz vx vy vz)
            tm[4, 0] = -fr * C * XD * we / (2.0 * A * B);
            tm[4, 1] = -fr * C * XD * wq / (2.0 * A * B);
            tm[4, 2] = -fr * C * XD * ww / (2.0 * A * B);
            tm[4, 3] = fr * C * X * we / (2.0 * A * B);
            tm[4, 4] = fr * C * X * wq / (2.0 * A * B);
            tm[4, 5] = fr * C * X * ww / (2.0 * A * B);

            if (anomeq.Equals("truea") || anomeq.Equals("truen") || anomeq.Equals("truep"))
            {
                // not ready yet
                //            p0 = -sign(argp)/Math.Sqrt(1-cos_w*cos_w);
                //            p1 = 1.0 / gravConst.mum;
                //             tm[5,0] = p0*(p1*()/(n*ecc) - ()/n*()/(n*n*ecc) - tm[ecc/ry*()) + fr*-vz*nodey/n*n +  
                //                      ;
                //             tm[5,1] = p0*();
                //             tm[5,2] = p0*();
                //             tm[5,3] = p0*();
                //             tm[5,4] = p0*();
                //             tm[5,5] = p0*();

                tm[5, 0] = 0.0;
                tm[5, 1] = 0.0;
                tm[5, 2] = 0.0;
                tm[5, 3] = 0.0;
                tm[5, 4] = 0.0;
                tm[5, 5] = 0.0;
            }
            else
            {
                if (anomeq.Equals("meana") || anomeq.Equals("meann") || anomeq.Equals("meanp"))
                {
                    // ---- partials of meanlon wrt (rx ry rz vx vy vz)
                    tm[5, 0] = -vx / A + (chi * XD - psi * fr * YD) * we / (A * B) - (b * B / A) * (ag * tm34 + af * tm24);
                    tm[5, 1] = -vy / A + (chi * XD - psi * fr * YD) * wq / (A * B) - (b * B / A) * (ag * tm35 + af * tm25);
                    tm[5, 2] = -vz / A + (chi * XD - psi * fr * YD) * ww / (A * B) - (b * B / A) * (ag * tm36 + af * tm26);
                    tm[5, 3] = -2.0 * rx / A + (af * tm[2, 3] - ag * tm[1, 3]) / (1.0 + B) + (fr * psi * Y - chi * X) * we / A;
                    tm[5, 4] = -2.0 * ry / A + (af * tm[2, 4] - ag * tm[1, 4]) / (1.0 + B) + (fr * psi * Y - chi * X) * wq / A;
                    tm[5, 5] = -2.0 * rz / A + (af * tm[2, 5] - ag * tm[1, 5]) / (1.0 + B) + (fr * psi * Y - chi * X) * ww / A;
                }
            }

            // ---------- calculate the output covariance matrix -----------
            double[,] tmt = MathTimeLibr.mattrans(tm, 6);
            double[,] tempm = MathTimeLibr.matmult(cartcov, tmt, 6, 6, 6);
            eqcov = MathTimeLibr.matmult(tm, tempm, 6, 6, 6);
        }  //  covct2eq


        /* ----------------------------------------------------------------------------
        *
        *                           function coveq2ct
        *
        *  this function transforms a six by six covariance matrix expressed in
        *    equinoctial elements into one expressed in cartesian elements.
        *
        *  author        : david vallado           davallado@gmail.com   24 jul 2003
        *
        *  revisions
        *    vallado     - major update                                  26 aug 2015
        *
        *  inputs          description                          range / units
        *    eqcov       - 6x6 equinoctial covariance matrix space delimited
        *    eqstate     - 6x1 equinoctial orbit state          (a/n af ag chi psi lm/ln)
        *    anomeq      - anomaly                              'meana', 'truea', 'meann', 'truen', 'meanp', 'truep'
        *    fr          - retrograde factor                     +1, -1
        *
        *  outputs       :
        *    cartcov     - 6x6 cartesian covariance matrix
        *    tm          - transformation matrix
        *
        *  locals        :
        *    n           - mean motion                          rad
        *    af          - component of ecc vector
        *    ag          - component of ecc vector
        *    chi         - component of node vector in eqw
        *    psi         - component of node vector in eqw
        *    meanlon     - mean longitude                       rad
        *    nu          - true anomaly                               0.0  to 2pi rad
        *    m           - mean anomaly                   0.0  to 2pi rad
        *    r           - matrix of partial derivatives
        *    eccanom          - eccentric anomaly                    0.0  to 2pi rad
        *
        *  coupling      :
        *    constastro
        *
        *  references    :
        *    Vallado and Alfano 2015
        * ---------------------------------------------------------------------------- */

        public void coveq2ct
            (
            double[,] eqcov, double[] eqstate, string anomeq, Int16 fr, out double[,] cartcov, out double[,] tm
            )
        {
            double p, a, n, ecc, raan, argp, nu, m, eccanom, af, ag, chi, psi, X, Y, b, meanlonM, meanlonNu,
                magr, magv, magr3, r_dot_v, rx, ry, rz, vx, vy, vz, F0, F1,
                ecc_term, ecc_x, ecc_y, ecc_z,
                r_dot_e, nu_scale, p0, p1, temp,
                fe, fq, fw, ge, gq, gw, we, wq, ww, partXaf, partXag, partYaf, partYag,
                A, B, C, F, G, XD, YD, partXDaf, partYDaf, partXDag, partYDag;
            Int32 ktr, numiter;
            double[] reci = new double[3];
            double[] veci = new double[3];
            tm = new double[,] { { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0 } };

            // -------- define the gravitational constant
            // double mum = 3.986004415e14;
            double small = 0.00000001;
            double twopi = 2.0 * Math.PI;
            // initialize
            meanlonM = 0.0;
            a = 0.0;
            n = 0.0;
            p = 0.0;
            nu = 0.0;
            ecc = 0.0;
            p1 = 0.0;

            // --------- determine which set of variables is in use ---------
            // -------- parse the orbit state
            if (anomeq.Equals("truea") || anomeq.Equals("meana"))
            {
                a = eqstate[0] * 1000.0;  // in m
                n = Math.Sqrt(gravConst.mum / (a * a * a));
            }
            else
            {
                if (anomeq.Equals("truen") || anomeq.Equals("meann"))
                {
                    n = eqstate[0];
                    a = Math.Pow(gravConst.mum / (n * n), 1.0 / 3.0);
                }
                else
                    if (anomeq.Equals("truep") || anomeq.Equals("meanp"))
                {
                    p = eqstate[0];
                    a = Math.Pow(gravConst.mum / (n * n), 1.0 / 3.0);
                    ecc = Math.Sqrt(1.0 - p / a);
                    n = Math.Sqrt(gravConst.mum / (a * a * a));
                }
            }
            af = eqstate[1];
            ag = eqstate[2];
            chi = eqstate[3];
            psi = eqstate[4];
            if (anomeq.Equals("meana") || anomeq.Equals("meann") || anomeq.Equals("meanp"))
                meanlonM = eqstate[5];  // in rad
            else
            {
                if (anomeq.Equals("truea") || anomeq.Equals("truen") || anomeq.Equals("meanp"))
                {
                    meanlonNu = eqstate[5];
                    raan = Math.Atan2(chi, psi);
                    argp = Math.Atan2(ag, af) - fr * Math.Atan2(chi, psi);
                    nu = meanlonNu - fr * raan - argp;
                    nu = (nu + twopi) % twopi;
                    ecc = Math.Sqrt(af * af + ag * ag);
                    newtonnu(ecc, nu, out eccanom, out m);
                    meanlonM = fr * raan + argp + m;
                    meanlonM = meanlonM % twopi;
                }
            }

            // needs to be mean longitude for eq2rv
            eq2rv(a * 0.001, af, ag, chi, psi, meanlonM, fr, out reci, out veci);
            rx = reci[0] * 1000.0;  // in km
            ry = reci[1] * 1000.0;
            rz = reci[2] * 1000.0;
            vx = veci[0] * 1000.0;
            vy = veci[1] * 1000.0;
            vz = veci[2] * 1000.0;

            magr = MathTimeLibr.mag(reci) * 1000.0; // m
            magv = MathTimeLibr.mag(veci) * 1000.0; // m/s

            A = n * a * a;
            B = Math.Sqrt(1.0 - ag * ag - af * af);
            C = 1.0 + chi * chi + psi * psi;
            b = 1.0 / (1.0 + B);

            G = n * a * a * Math.Sqrt(1.0 - af * af - ag * ag);  // = A*B

            // -----------  initial guess -------------
            F0 = meanlonM;
            numiter = 25;
            ktr = 1;
            F1 = F0 - (F0 + ag * Math.Cos(F0) - af * Math.Sin(F0) - meanlonM) / (1.0 - ag * Math.Sin(F0) - af * Math.Cos(F0));
            while ((Math.Abs(F1 - F0) > small) && (ktr <= numiter))
            {
                ktr = ktr + 1;
                F0 = F1;
                F1 = F0 - (F0 + ag * Math.Cos(F0) - af * Math.Sin(F0) - meanlonM) / (1.0 - ag * Math.Sin(F0) - af * Math.Cos(F0));
            }

            F = F1;

            X = a * ((1.0 - ag * ag * b) * Math.Cos(F) + af * ag * b * Math.Sin(F) - af);
            Y = a * ((1.0 - af * af * b) * Math.Sin(F) + af * ag * b * Math.Cos(F) - ag);

            XD = n * a * a / magr * (af * ag * b * Math.Cos(F) - (1.0 - ag * ag * b) * Math.Sin(F));
            YD = n * a * a / magr * ((1.0 - af * af * b) * Math.Cos(F) - af * ag * b * Math.Sin(F));

            // alt forms all are the same now 
            //sinL = ((1-af*af*b)*Math.Sin(F) + ag*af*b*Math.Cos(F) - ag) / (1 - ag*Math.Sin(F) - af*Math.Cos(F));
            //cosL = ((1-ag*ag*b)*Math.Cos(F) + ag*af*b*Math.Sin(F) - af) / (1 - ag*Math.Sin(F) - af*Math.Cos(F));
            //XD = -n*a*(ag + sinL) / (B);
            //YD =  n*a*(af + cosL) / (B);
            //r = a*(1-af*af-ag*ag) / (1 + ag*sinL + af*cosL);
            //r = a*(1.0 - ag*Math.Sin(F) - af*Math.Cos(F));
            //r = magr 
            //X = r*cosL;
            //Y = r*sinL;

            // components of equinoctial system
            p0 = 1.0 / (1.0 + chi * chi + psi * psi);
            fe = p0 * (1.0 - chi * chi + psi * psi);
            fq = p0 * 2.0 * chi * psi;
            fw = p0 * -2.0 * fr * chi;
            ge = p0 * 2.0 * fr * chi * psi;
            gq = p0 * fr * (1.0 + chi * chi - psi * psi);
            gw = p0 * 2.0 * psi;
            we = p0 * 2.0 * chi;
            wq = p0 * -2.0 * psi;
            ww = p0 * fr * (1.0 - chi * chi - psi * psi);

            partXaf = ag * b * XD / n + a / G * Y * XD - a;
            partXag = -af * b * XD / n + a / G * Y * YD;
            partYaf = ag * b * YD / n - a / G * X * XD;
            partYag = -af * b * YD / n - a / G * X * YD - a;

            partXDaf = a * XD * YD / (A * B) - A / (Math.Pow(magr, 3)) * (a * ag * X / (1.0 + B) + X * Y / B);
            partYDaf = -a * XD * XD / (A * B) - A / (Math.Pow(magr, 3)) * (a * ag * Y / (1.0 + B) - X * X / B);
            partXDag = a * YD * YD / (A * B) + A / (Math.Pow(magr, 3)) * (a * af * X / (1.0 + B) - Y * Y / B);
            partYDag = -a * XD * YD / (A * B) + A / (Math.Pow(magr, 3)) * (a * af * Y / (1.0 + B) + X * Y / B);

            // ---------------- calculate matrix elements ------------------
            // ---- partials of (rx ry rz vx vy vz) wrt a
            if (anomeq.Equals("truea") || anomeq.Equals("meana"))
            {
                p0 = 1.0 / a;
                p1 = -1.0 / (2.0 * a);
            }
            else
            {
                if (anomeq.Equals("truen") || anomeq.Equals("meann"))
                {
                    p0 = -2.0 / (3.0 * n);
                    p1 = 1.0 / (3.0 * n);
                }
                else
                    if (anomeq.Equals("truep") || anomeq.Equals("meanp"))
                {
                    p0 = 1.0 / p;
                    p1 = -1.0 / (2.0 * p);
                }
            }
            tm[0, 0] = p0 * rx;
            tm[1, 0] = p0 * ry;
            tm[2, 0] = p0 * rz;
            tm[3, 0] = p1 * vx;
            tm[4, 0] = p1 * vy;
            if (anomeq.Equals("meana") || anomeq.Equals("meann") || anomeq.Equals("meanp"))
                tm[5, 0] = p1 * vz;
            else
            {
                if (anomeq.Equals("truea") || anomeq.Equals("truen") || anomeq.Equals("truep"))
                    tm[5, 0] = 0.0;
            }

            // ---- partials of (rx ry rz vx vy vz) wrt af
            tm[0, 1] = partXaf * fe + partYaf * ge;
            tm[1, 1] = partXaf * fq + partYaf * gq;
            tm[2, 1] = partXaf * fw + partYaf * gw;
            tm[3, 1] = partXDaf * fe + partYDaf * ge;
            tm[4, 1] = partXDaf * fq + partYDaf * gq;
            if (anomeq.Equals("meana") || anomeq.Equals("meann") || anomeq.Equals("meanp"))
                tm[5, 1] = partXDaf * fw + partYDaf * gw;
            else
            {
                if (anomeq.Equals("truea") || anomeq.Equals("truen") || anomeq.Equals("truep"))
                {
                    tm[5, 1] = 0.0;
                }
            }

            // ---- partials of (rx ry rz vx vy vz) wrt ag
            tm[0, 2] = partXag * fe + partYag * ge;
            tm[1, 2] = partXag * fq + partYag * gq;
            tm[2, 2] = partXag * fw + partYag * gw;
            tm[3, 2] = partXDag * fe + partYDag * ge;
            tm[4, 2] = partXDag * fq + partYDag * gq;
            if (anomeq.Equals("meana") || anomeq.Equals("meann") || anomeq.Equals("meanp"))
                tm[5, 2] = partXDag * fw + partYDag * gw;
            else
            {
                if (anomeq.Equals("truea") || anomeq.Equals("truen") || anomeq.Equals("truep"))
                {
                    tm[5, 2] = 0.0;
                }
            }

            // ---- partials of (rx ry rz vx vy vz) wrt chi
            p0 = 2.0 / C;
            tm[0, 3] = p0 * fr * (psi * (Y * fe - X * ge) - X * we);  // switch to paper 2.0 * (fr *
            tm[1, 3] = p0 * fr * (psi * (Y * fq - X * gq) - X * wq);
            tm[2, 3] = p0 * fr * (psi * (Y * fw - X * gw) - X * ww);
            tm[3, 3] = p0 * fr * (psi * (YD * fe - XD * ge) - XD * we);
            tm[4, 3] = p0 * fr * (psi * (YD * fq - XD * gq) - XD * wq);
            if (anomeq.Equals("meana") || anomeq.Equals("meann") || anomeq.Equals("meanp"))
                tm[5, 3] = p0 * (fr * psi * (YD * fw - XD * gw) - XD * ww);
            else  // where is fr*?????????????????????above
            {
                if (anomeq.Equals("truea") || anomeq.Equals("truen") || anomeq.Equals("truep"))
                {
                    tm[5, 3] = 0.0;
                }
            }

            // ---- partials of (rx ry rz vx vy vz) wrt psi
            p0 = 2.0 / C;
            tm[0, 4] = p0 * fr * (chi * (X * ge - Y * fe) + Y * we);
            tm[1, 4] = p0 * fr * (chi * (X * gq - Y * fq) + Y * wq);
            tm[2, 4] = p0 * fr * (chi * (X * gw - Y * fw) + Y * ww);
            tm[3, 4] = p0 * fr * (chi * (XD * ge - YD * fe) + YD * we);
            tm[4, 4] = p0 * fr * (chi * (XD * gq - YD * fq) + YD * wq);
            if (anomeq.Equals("meana") || anomeq.Equals("meann") || anomeq.Equals("meanp"))
                tm[5, 4] = p0 * fr * (chi * (XD * gw - YD * fw) + YD * ww);
            else
            {
                if (anomeq.Equals("truea") || anomeq.Equals("truen") || anomeq.Equals("truep"))
                {
                    tm[5, 4] = 0.0;
                }
            }

            // ---- partials of (rx ry rz vx vy vz) wrt meanlon
            p0 = 1.0 / n;
            p1 = n * a * a * a / Math.Pow(magr, 3);
            tm[0, 5] = p0 * vx;
            tm[1, 5] = p0 * vy;
            tm[2, 5] = p0 * vz;
            tm[3, 5] = -p1 * rx;
            tm[4, 5] = -p1 * ry;
            if (anomeq.Equals("meana") || anomeq.Equals("meann") || anomeq.Equals("meanp"))
                tm[5, 5] = -p1 * rz;
            else
            {
                if (anomeq.Equals("truea") || anomeq.Equals("truen") || anomeq.Equals("truep"))
                {
                    tm[5, 5] = 0.0;
                    // similar to ct2cl true           
                    r_dot_v = Math.Sqrt(rx * vx + ry * vy + rz * vz);
                    ecc_term = magv * magv - gravConst.mum / magr;
                    ecc_x = (ecc_term * rx - r_dot_v * vx) / gravConst.mum;
                    ecc_y = (ecc_term * ry - r_dot_v * vy) / gravConst.mum;
                    ecc_z = (ecc_term * rz - r_dot_v * vz) / gravConst.mum;
                    r_dot_e = Math.Sqrt(rx * ecc_x + ry * ecc_y + rz * ecc_z);
                    nu_scale = -Math.Sign(r_dot_v) / Math.Sqrt(1 - Math.Cos(nu) * Math.Cos(nu));
                    magr3 = Math.Pow(magr, 3);
                    temp = ry * (vx * vy - gravConst.mum * rx * ry / magr3) - rx * ecc_term + rz * (vx * vz - gravConst.mum * rx * rz / magr3);
                    temp = temp - rx * (vy * vy + vz * vz - gravConst.mum / magr + gravConst.mum * rx * rx / magr3) + vx * r_dot_v;
                    temp = -temp / (gravConst.mum * magr * ecc) - rx * r_dot_e / (magr3 * ecc) - tm[1, 0] * r_dot_e / (magr * ecc * ecc);
                    tm[5, 0] = temp * nu_scale;
                    temp = rx * (vx * vy - gravConst.mum * rx * ry / magr3) - ry * ecc_term + rz * (vy * vz - gravConst.mum * ry * rz / magr3);
                    temp = temp - ry * (vx * vx + vz * vz - gravConst.mum / magr + gravConst.mum * ry * ry / magr3) + vy * r_dot_v;
                    temp = -temp / (gravConst.mum * magr * ecc) - ry * r_dot_e / (magr3 * ecc) - tm[1, 1] * r_dot_e / (magr * ecc * ecc);
                    tm[5, 1] = temp * nu_scale;
                    temp = rx * (vx * vz - gravConst.mum * rx * rz / magr3) - rz * ecc_term + ry * (vy * vz - gravConst.mum * ry * rz / magr3);
                    temp = temp - rz * (vx * vx + vy * vy - gravConst.mum / magr + gravConst.mum * rz * rz / magr3) + vz * r_dot_v;
                    temp = -temp / (gravConst.mum * magr * ecc) - rz * r_dot_e / (magr3 * ecc) - tm[1, 2] * r_dot_e / (magr * ecc * ecc);
                    tm[5, 2] = temp * nu_scale;
                    temp = ry * (rx * vy - 2.0 * ry * vx) + rx * (ry * vy + rz * vz) + rz * (rx * vz - 2 * rz * vx);
                    temp = -temp / (gravConst.mum * magr * ecc) - tm[1, 3] * r_dot_e / (magr * ecc * ecc);
                    tm[5, 3] = temp * nu_scale;
                    temp = rx * (ry * vx - 2.0 * rx * vy) + ry * (rx * vx + rz * vz) + rz * (ry * vz - 2 * rz * vy);
                    temp = -temp / (gravConst.mum * magr * ecc) - tm[1, 4] * r_dot_e / (magr * ecc * ecc);
                    tm[5, 4] = temp * nu_scale;
                    temp = rz * (rx * vx + ry * vy) + rx * (rz * vx - 2.0 * rx * vz) + ry * (rz * vy - 2 * ry * vz);
                    temp = -temp / (gravConst.mum * magr * ecc) - tm[1, 5] * r_dot_e / (magr * ecc * ecc);
                    tm[5, 5] = temp * nu_scale;
                }
            }

            // ---------- calculate the output covariance matrix -----------
            double[,] tmt = MathTimeLibr.mattrans(tm, 6);
            double[,] tempm = MathTimeLibr.matmult(eqcov, tmt, 6, 6, 6);
            cartcov = MathTimeLibr.matmult(tm, tempm, 6, 6, 6);
        }  //  coveq2ct


        /* ----------------------------------------------------------------------------
        *
        *                           function covcl2eq
        *
        *  this function transforms a six by six covariance matrix expressed in
        *    classical elements into one expressed in equinoctial elements.
        *
        *  author        : david vallado           davallado@gmail.com   14 jul 2002
        *
        *  revisions
        *    vallado     - major update                                  26 aug 2015
        *
        *  inputs          description                          range / units
        *    classcov    - 6x6 classical covariance matrix
        *    classstate  - 6x1 classical orbit state            (a e i O w nu/m)
        *    anomclass   - anomaly                              'meana', 'truea'
        *    anomeq      - anomaly                              'meana', 'truea', 'meann', 'truen', 'meanp', 'truep'
        *    fr          - retrograde factor                     +1, -1
        *
        *  outputs       :
        *    eqcov       - 6x6 equinoctial covariance matrix
        *    tm          - transformation matrix
        *
        *  locals        :
        *    a           - semimajor axis                       m
        *    ecc         - eccentricity
        *    incl        - inclination                          0.0  to pi rad
        *    oMathTimeLibr.maga       - longitude of ascending node    0.0  to 2pi rad
        *    argp        - argument of perigee                  0.0  to 2pi rad
        *    nu          - true anomaly                         0.0  to 2pi rad
        *    m           - mean anomaly                         0.0  to 2pi rad
        *    gravConst.mum, gravitational paramater   m^3/s^2 NOTE Meters!
        *
        *  coupling      :
        *
        *  references    :
        *    Vallado and Alfano 2015
        * ----------------------------------------------------------------------------*/

        public void covcl2eq
            (
            double[,] classcov, double[] classstate, string anomclass, string anomeq, Int16 fr, out double[,] eqcov, out double[,] tm
            )
        {
            double a, n, ecc, incl, raan, argp, nu, m;
            tm = new double[,] { { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0 } };

            // -------- define the gravitational constant
            // double mum = 3.986004418e14;

            // --------- determine which set of variables is in use ---------
            // -------- parse the orbit state
            a = classstate[0] * 1000.0;  // in m
            n = Math.Sqrt(gravConst.mum / (a * a * a));
            ecc = classstate[1];
            incl = classstate[2];
            raan = classstate[3];
            argp = classstate[4];
            if (anomclass.Equals("meana"))
                m = classstate[5];
            else
            {
                if (anomclass.Equals("truea"))
                {
                    nu = classstate[5];
                }
            }

            // ---- partials of a wrt (a e i O w nu/m/tau)
            if (anomeq.Equals("truea") || anomeq.Equals("meana"))
                tm[0, 0] = 1.0;
            else
            {
                if (anomeq.Equals("truen") || anomeq.Equals("meann"))
                {
                    tm[0, 0] = -(3.0 * Math.Sqrt(gravConst.mum / (a * a * a))) / (2.0 * a);  // if class = a, equin = n
                }
                else
                    if (anomeq.Equals("truep") || anomeq.Equals("meanp"))
                {
                    tm[0, 0] = 1.0 - ecc * ecc;
                }
            }

            // ---------------- calculate matrix elements ------------------
            tm[0, 1] = 0.0;
            if (anomeq.Equals("truep") || anomeq.Equals("meanp"))
                tm[0, 1] = -2.0 * a * ecc;

            tm[0, 2] = 0.0;
            tm[0, 3] = 0.0;
            tm[0, 4] = 0.0;
            tm[0, 5] = 0.0;

            // ---- partials of af wrt (a e i O w nu/m/tau)
            tm[1, 0] = 0.0;
            tm[1, 1] = Math.Cos(fr * raan + argp);
            tm[1, 2] = 0.0;
            tm[1, 3] = -ecc * fr * Math.Sin(fr * raan + argp);
            tm[1, 4] = -ecc * Math.Sin(fr * raan + argp);
            tm[1, 5] = 0.0;

            // ---- partials of ag wrt (a e i O w nu/m/tau)
            tm[2, 0] = 0.0;
            tm[2, 1] = Math.Sin(fr * raan + argp);
            tm[2, 2] = 0.0;
            tm[2, 3] = ecc * fr * Math.Cos(fr * raan + argp);
            tm[2, 4] = ecc * Math.Cos(fr * raan + argp);
            tm[2, 5] = 0.0;

            // ---- partials of chi wrt (a e i O w nu/m/tau)
            tm[3, 0] = 0.0;
            tm[3, 1] = 0.0;
            tm[3, 2] = Math.Sin(raan) * (0.5 * Math.Tan(incl * 0.5) * Math.Tan(incl * 0.5) + 0.5) * fr * Math.Pow(Math.Tan(incl * 0.5), fr - 1);
            tm[3, 3] = Math.Pow(Math.Tan(incl * 0.5), fr) * Math.Cos(raan);
            tm[3, 4] = 0.0;
            tm[3, 5] = 0.0;

            // ---- partials of psi wrt (a e i O w nu/m/tau)
            tm[4, 0] = 0.0;
            tm[4, 1] = 0.0;
            tm[4, 2] = Math.Cos(raan) * (0.5 * Math.Tan(incl * 0.5) * Math.Tan(incl * 0.5) + 0.5) * fr * Math.Pow(Math.Tan(incl * 0.5), fr - 1);
            tm[4, 3] = -Math.Pow(Math.Tan(incl * 0.5), fr) * Math.Sin(raan);
            tm[4, 4] = 0.0;
            tm[4, 5] = 0.0;

            // ---- partials of l wrt (a e i O w nu/m/tau)
            if (anomeq.Equals("truea") || anomeq.Equals("truen") || anomeq.Equals("truep"))
            {
                //[eccanom,nu] = newtonm ( ecc,anomaly );
                tm[5, 0] = 0.0;
                tm[5, 1] = 0.0;
                tm[5, 2] = 0.0;
                tm[5, 3] = fr;
                tm[5, 4] = 1.0;
                tm[5, 5] = 1.0;
            }
            else
            {
                if (anomeq.Equals("meana") || anomeq.Equals("meann") || anomeq.Equals("meanp"))
                {
                    tm[5, 0] = 0.0;
                    tm[5, 1] = 0.0;
                    tm[5, 2] = 0.0;
                    tm[5, 3] = fr;
                    tm[5, 4] = 1.0;
                    tm[5, 5] = 1.0;
                }
            }

            // ---------- calculate the output covariance matrix -----------
            double[,] tmt = MathTimeLibr.mattrans(tm, 6);
            double[,] tempm = MathTimeLibr.matmult(classcov, tmt, 6, 6, 6);
            eqcov = MathTimeLibr.matmult(tm, tempm, 6, 6, 6);
        }  //  covcl2eq



        /* ----------------------------------------------------------------------------
        *
        *                           function coveq2cl
        *
        *  this function transforms a six by six covariance matrix expressed in
        *    equinoctial elements into one expressed in classical orbital elements.
        *
        *  author        : david vallado           davallado@gmail.com   18 jun 2002
        *
        *  revisions
        *    vallado     - major update                                  26 aug 2015
        *
        *  inputs          description                                range / units
        *    eqcov       - 6x6 equinoctial covariance matrix
        *    eqstate     - 6x1 equinoctial orbit state                (a/n/p af ag chi psi lm/ln)
        *    anomclass   - anomaly                                    'meana', 'truea'
        *    anomeq      - anomaly                                    'meana', 'truea', 'meann', 'truen', 'meanp', 'truep'
        *    fr          - retrograde factor                           +1, -1
        *
        *  outputs       :
        *    classcov    - 6x6 classical covariance matrix
        *    tm          - transformation matrix
        *
        *  locals        :
        *    n           - mean motion                                rad
        *    af          - component of ecc vector
        *    ag          - component of ecc vector
        *    chi         - component of node vector in eqw
        *    psi         - component of node vector in eqw
        *    meanlon     - mean longitude                             rad
        *    gravConst.mum, gravitational paramater         m^3/s^2 NOTE Meters!
        *
        *  coupling      :
        *    constastro
        *
        *  references    :
        *    Vallado and Alfano 2015
        * ----------------------------------------------------------------------------*/

        public void coveq2cl
            (
            double[,] eqcov, double[] eqstate, string anomeq, string anomclass, Int16 fr, out double[,] classcov, out double[,] tm
            )
        {
            double p, a, ecc, n, af, ag, chi, psi, meanlonM, meanlonNu, p0, p1, p2, p3, p4;
            tm = new double[,] { { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 },
                    { 0, 0, 0, 0, 0, 0 } };

            // -------- define the gravitational constant
            //double mum = 3.986004418e14;

            // initialize
            a = 0.0;
            p = 0.0;
            n = 0.0;
            ecc = 0.0;

            // -------- parse the orbit state
            // --------- determine which set of variables is in use ---------
            if (anomeq.Equals("truea") || anomeq.Equals("meana"))
            {
                a = eqstate[0] * 1000.0;  // in m
                n = Math.Sqrt(gravConst.mum / (a * a * a));
            }
            else
            {
                if (anomeq.Equals("truen") || anomeq.Equals("meann"))
                {
                    n = eqstate[0];
                    a = Math.Pow(gravConst.mum / (n * n), 1.0 / 3.0);
                }
                else
                    if (anomeq.Equals("truep") || anomeq.Equals("meanp"))
                {
                    p = eqstate[0];
                    a = Math.Pow(gravConst.mum / (n * n), 1.0 / 3.0);
                    ecc = Math.Sqrt(1.0 - p / a);
                    n = Math.Sqrt(gravConst.mum / (a * a * a));
                }
            }
            af = eqstate[1];
            ag = eqstate[2];
            chi = eqstate[3];
            psi = eqstate[4];
            if (anomeq.Equals("meana") || anomeq.Equals("meann") || anomeq.Equals("meanp"))
                meanlonM = eqstate[5];
            else
            {
                if (anomeq.Equals("truea") || anomeq.Equals("truen") || anomeq.Equals("truep"))
                {
                    meanlonNu = eqstate[5];
                }
            }


            // ---------------- calculate matrix elements ------------------
            // ---- partials of a wrt (a af ag chi psi l)
            if (anomeq.Equals("truea") || anomeq.Equals("meana"))
                tm[0, 0] = 1.0;
            else
            {
                if (anomeq.Equals("truen") || anomeq.Equals("meann"))
                {
                    tm[0, 0] = -2.0 / (3.0 * n) * Math.Pow(gravConst.mum / (n * n), (1.0 / 3.0));
                }
                else
                    if (anomeq.Equals("truep") || anomeq.Equals("meanp"))
                {
                    tm[0, 0] = 1.0 / (1.0 - ecc * ecc);
                }
            }
            tm[0, 1] = 0.0;
            tm[0, 2] = 0.0;
            tm[0, 3] = 0.0;
            tm[0, 4] = 0.0;
            tm[0, 5] = 0.0;

            // ---- partials of ecc wrt (n af ag chi psi l)
            p0 = 1.0 / Math.Sqrt(af * af + ag * ag);
            tm[1, 0] = 0.0;
            if (anomeq.Equals("truep") || anomeq.Equals("meanp"))
                tm[1, 0] = -1.0 / (2.0 * a * Math.Sqrt(p));
            tm[1, 1] = p0 * af;
            tm[1, 2] = p0 * ag;
            tm[1, 3] = 0.0;
            tm[1, 4] = 0.0;
            tm[1, 5] = 0.0;

            // ---- partials of incl wrt (n af ag chi psi l)
            p1 = 2.0 * fr / ((1.0 + chi * chi + psi * psi) * Math.Sqrt(chi * chi + psi * psi));
            tm[2, 0] = 0.0;
            tm[2, 1] = 0.0;
            tm[2, 2] = 0.0;
            tm[2, 3] = p1 * chi;
            tm[2, 4] = p1 * psi;
            tm[2, 5] = 0.0;

            // ---- partials of raan wrt (n af ag chi psi l)
            p2 = 1.0 / (chi * chi + psi * psi);
            tm[3, 0] = 0.0;
            tm[3, 1] = 0.0;
            tm[3, 2] = 0.0;
            tm[3, 3] = p2 * psi;
            tm[3, 4] = -p2 * chi;
            tm[3, 5] = 0.0;

            // ---- partials of argp wrt (n af ag chi psi l)
            p3 = 1.0 / (af * af + ag * ag);
            tm[4, 0] = 0.0;
            tm[4, 1] = -p3 * ag;
            tm[4, 2] = p3 * af;
            tm[4, 3] = -fr * p2 * psi;
            tm[4, 4] = fr * p2 * chi;
            tm[4, 5] = 0.0;

            // ---- partials of anomaly wrt (n af ag chi psi l)
            p4 = 1.0 / (af * af + ag * ag);
            if (anomclass.Contains("true"))
            {
                tm[5, 0] = 0.0;
                tm[5, 1] = p4 * ag;  // p3 on these. same
                tm[5, 2] = -p4 * af;
                tm[5, 3] = 0.0;
                tm[5, 4] = 0.0;
                tm[5, 5] = 1.0;
            }
            else
            {
                if (anomclass.Contains("mean"))
                {
                    tm[5, 0] = 0.0;
                    tm[5, 1] = p3 * ag;  // switch signs to paper, not sure this is correct leave orig
                    tm[5, 2] = -p3 * af;
                    tm[5, 3] = 0.0;
                    tm[5, 4] = 0.0;
                    tm[5, 5] = 1.0;
                }
            }

            // ---------- calculate the output covariance matrix -----------
            double[,] tmt = MathTimeLibr.mattrans(tm, 6);
            double[,] tempm = MathTimeLibr.matmult(eqcov, tmt, 6, 6, 6);
            classcov = MathTimeLibr.matmult(tm, tempm, 6, 6, 6);
        }  //  coveq2cl


        /* ----------------------------------------------------------------------------
        *
        *                           function covct2fl
        *
        *  this function transforms a six by six covariance matrix expressed in cartesian elements
        *    into one expressed in flight parameters
        *
        *  author        : david vallado           davallado@gmail.com   21 jun 2002
        *
        *  revisions
        *    vallado     - major update                                  26 aug 2015
        *
        *  inputs          description                          range / units
        *    cartcov     - 6x6 cartesian covariance matrix
        *    cartstate   - 6x1 cartesian orbit state            (x y z vx vy vz)
        *    anom        - anomaly                              'latlon', 'radec'
        *    ttt         - julian centuries of tt               centuries
        *    jdut1       - julian date of ut1                   days from 4713 bc
        *    lod         - excess length of day                 sec
        *    xp          - polar motion coefficient             arc sec
        *    yp          - polar motion coefficient             arc sec
        *    terms       - number of terms for ast calculation 0,2
        *    opt         - method option                           e80, e96, e00a, e00cio
        *  outputs       :
        *    flcov       - 6x6 flight covariance matrix
        *    tm          - transformation matrix
        *
        *  locals        :
        *    r           - matrix of partial derivatives
        *    x,y,z       - components of position vector        km
        *    vx,vy,vz    - components of position vector        km/s
        *    magr        - eci position vector magnitude        km
        *    magv        - eci velocity vector magnitude        km/sec
        *    d           - r dot v
        *    h           - angular momentum vector
        *    hx,hy,hz    - components of angular momentum vector
        *    hcrossrx,y,z- components of h MathTimeLibr.cross r vector
        *    p1,p2       - denominator terms for the partials
        *
        * coupling      :
        *    ecef2eci    - convert eci vectors to ecef
        *
        * references    :
        *    Vallado and Alfano 2015
        ---------------------------------------------------------------------------- */

        public void covct2fl
            (
            double[,] cartcov, double[] cartstate, string anomflt, double jdtt, double jdftt, double jdut1, 
            double jdxysstart, double lod,
            double xp, double yp, Int16 terms, double ddpsi, double ddeps, double ddx, double ddy,
            EOPSPWLib.iau80Class iau80arr, EOPSPWLib.iau00Class iau00arr,
            EOpt opt,
            out double[,] flcov, out double[,] tm
            )
        {
            double rx, ry, rz, vx, vy, vz, rxf, ryf, rzf, magr, magv, rdotv, rdot, k1, k2, p0, p1, p2, h;
            double[] reci = new double[3];
            double[] veci = new double[3];
            double[] recef = new double[3];
            double[] vecef = new double[3];
            tm = new double[,] { { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0 } };
            // initialize
            rxf = 0.0;
            ryf = 0.0;
            rzf = 0.0;

            // -------- parse the input vectors into cartesian components
            rx = cartstate[0] * 1000.0;  // keep all in m, m/s
            ry = cartstate[1] * 1000.0;  // this is eci always
            rz = cartstate[2] * 1000.0;
            vx = cartstate[3] * 1000.0;
            vy = cartstate[4] * 1000.0;
            vz = cartstate[5] * 1000.0;

            if (anomflt.Equals("latlon"))
            {
                // -------- convert r to eci
                reci = new double[] { rx * 0.001, ry * 0.001, rz * 0.001 };
                veci = new double[] { vx * 0.001, vy * 0.001, vz * 0.001 };
                eci_ecef(ref reci, ref veci, MathTimeLib.Edirection.eto, ref recef, ref vecef,
                    EOpt.e80,
                    EOPSPWLibr.iau80arr, EOPSPWLibr.iau00arr,
                    jdtt, jdftt, jdut1, jdxysstart, lod, xp, yp, ddpsi, ddeps, ddx, ddy);

                recef[0] = recef[0] * 1000.0;  // in m
                recef[1] = recef[1] * 1000.0;
                recef[2] = recef[2] * 1000.0;
                vecef[0] = vecef[0] * 1000.0;  // in m/s
                vecef[1] = vecef[1] * 1000.0;
                vecef[2] = vecef[2] * 1000.0;
                rxf = recef[0];
                ryf = recef[1];
                rzf = recef[2];
            }
            else  // "radec"
            {
                // already eci is ok
            }

            // -------- calculate common quantities
            magr = Math.Sqrt(rx * rx + ry * ry + rz * rz);   // in m
            magv = Math.Sqrt(vx * vx + vy * vy + vz * vz);
            rdotv = rx * vx + ry * vy + rz * vz;

            h = Math.Sqrt(Math.Pow(rx * vy - ry * vx, 2) + Math.Pow(rz * vx - rx * vz, 2) + Math.Pow(ry * vz - rz * vy, 2));
            //          hx = ry*vz - rz*vy;  // 
            //          hy = rz*vx - rx*vz;
            //          hz = rx*vy - ry*vx;
            //          hcrossrx = (rz*hy - ry*hz);
            //          hcrossry = (rx*hz - rz*hx);
            //          hcrossrz = (ry*hx - rx*hy);

            // ---------------- calculate matrix elements ------------------
            if (anomflt.Equals("latlon"))
            {
                // partial of lon wrt (x y z vx vy vz)
                p0 = 1.0 / (rxf * rxf + ryf * ryf);
                tm[0, 0] = -p0 * ryf;
                tm[0, 1] = p0 * rxf;
                tm[0, 2] = 0.0;
                tm[0, 3] = 0.0;
                tm[0, 4] = 0.0;
                tm[0, 5] = 0.0;

                // partial of latgc wrt (x y z vx vy vz)
                p0 = 1.0 / (magr * magr * Math.Sqrt(rxf * rxf + ryf * ryf));
                tm[1, 0] = -p0 * (rxf * rzf);
                tm[1, 1] = -p0 * (ryf * rzf);
                tm[1, 2] = Math.Sqrt(rxf * rxf + ryf * ryf) / (magr * magr);
                tm[1, 3] = 0.0;
                tm[1, 4] = 0.0;
                tm[1, 5] = 0.0;
            }
            else  // radec
            {
                // partial of lon wrt (x y z vx vy vz)
                p0 = 1.0 / (rx * rx + ry * ry);
                tm[0, 0] = -p0 * ry;
                tm[0, 1] = p0 * rx;
                tm[0, 2] = 0.0;
                tm[0, 3] = 0.0;
                tm[0, 4] = 0.0;
                tm[0, 5] = 0.0;

                // partial of latgc wrt (x y z vx vy vz)
                p0 = 1.0 / (magr * magr * Math.Sqrt(rx * rx + ry * ry));
                tm[1, 0] = -p0 * (rx * rz);
                tm[1, 1] = -p0 * (ry * rz);
                tm[1, 2] = Math.Sqrt(rx * rx + ry * ry) / (magr * magr);
                tm[1, 3] = 0.0;
                tm[1, 4] = 0.0;
                tm[1, 5] = 0.0;
            }

            // partial of fpa wrt (x y z vx vy vz)
            rdot = rdotv / magr;  // (r dot v) / r
            p1 = -1.0 / (magr * Math.Sqrt(magv * magv - rdot * rdot));
            tm[2, 0] = p1 * (rdot * rx / magr - vx);
            tm[2, 1] = p1 * (rdot * ry / magr - vy);
            tm[2, 2] = p1 * (rdot * rz / magr - vz);
            tm[2, 3] = p1 * (rdot * magr * vx / (magv * magv) - rx);
            tm[2, 4] = p1 * (rdot * magr * vy / (magv * magv) - ry);
            tm[2, 5] = p1 * (rdot * magr * vz / (magv * magv) - rz);
            // Sal from mathcad matches previous with - sign on previous
            p0 = 1.0 / (magr * magr * h);
            p1 = 1.0 / (magv * magv * h);
            tm[2, 0] = p0 * (vx * (ry * ry + rz * rz) - rx * (ry * vy + rz * vz));
            tm[2, 1] = p0 * (vy * (rx * rx + rz * rz) - ry * (rx * vx + rz * vz));
            tm[2, 2] = p0 * (vz * (rx * rx + ry * ry) - rz * (rx * vx + ry * vy));
            tm[2, 3] = p1 * (rx * (vy * vy + vz * vz) - vx * (ry * vy + rz * vz));
            tm[2, 4] = p1 * (ry * (vx * vx + vz * vz) - vy * (rx * vx + rz * vz));
            tm[2, 5] = p1 * (rz * (vx * vx + vy * vy) - vz * (rx * vx + ry * vy));

            // partial of az wrt (x y z vx vy vz)
            //         p2 = 1.0 / ((magv^2 - rdot^2) * (rx^2 + ry^2));
            //         tm[3,0] = p2*( vy*(magr*vz - rz*rdot) - (rx*vy - ry*vx) * (rx*vz - rz*vx + rx*rz*rdot/magr) * (1.0 / magr) );
            //         tm[3,1] = p2*( -vx*(magr*vz - rz*rdot) + (rx*vy - ry*vx) * (ry*vz - rz*vy + ry*rz*rdot/magr) * (1.0 / magr) );                 
            //         p2 = 1.0 / (magr^2 * (magv^2 - rdot^2));        
            //         tm[3,2] = p2 * rdot * (rx*vy - ry*vx);
            //         p2 = 1.0 / (magr * (magv^2 - rdot^2));                 
            //         tm[3,3] = -p2 * (ry*vz - rz*vy);
            //         tm[3,4] =  p2 * (rx*vz - rz*vx);                  
            //         tm[3,5] = -p2 * (rx*vy - ry*vx); 
            // sal from mathcad
            p2 = 1.0 / ((magv * magv - rdot * rdotv) * (rx * rx + ry * ry));
            k1 = Math.Sqrt(rx * rx + ry * ry + rz * rz) * (rx * vy - ry * vx);
            k2 = ry * (ry * vz - rz * vy) + rx * (rx * vz - rz * vx);
            tm[3, 0] = p2 * (vy * (magr * vz - rz * rdot) - (rx * vy - ry * vx) / magr * (rx * vz - rz * vx + rx * ry * rdot / magr));
            p2 = 1.0 / (magr * (k1 * k1 + k2 * k2));
            tm[3, 0] = p2 * (k1 * magr * (rz * vx - 2.0 * rx * vz) + k2 * (-ry * vx * rx + vy * rx * rx + vy * magr * magr));
            tm[3, 1] = p2 * (k1 * magr * (rz * vy - 2.0 * ry * vz) + k2 * (rx * vy * ry - vx * ry * ry - vx * magr * magr));
            p2 = k1 / (magr * magr * (k1 * k1 + k2 * k2));
            tm[3, 2] = p2 * (k2 * rz + (rx * vx + ry * vy) * magr * magr);
            p2 = 1.0 / (k1 * k1 + k2 * k2);
            tm[3, 3] = p2 * (k1 * rx * rz - k2 * ry * magr);
            tm[3, 4] = p2 * (k1 * ry * rz + k2 * rx * magr);
            tm[3, 5] = -p2 * (k1 * (rx * rx + ry * ry));

            // partial of r wrt (x y z vx vy vz)
            p0 = 1.0 / magr;
            tm[4, 0] = p0 * rx;
            tm[4, 1] = p0 * ry;
            tm[4, 2] = p0 * rz;
            tm[4, 3] = 0.0;
            tm[4, 4] = 0.0;
            tm[4, 5] = 0.0;

            // partial of v wrt (x y z vx vy vz)
            p0 = 1.0 / magv;
            tm[5, 0] = 0.0;
            tm[5, 1] = 0.0;
            tm[5, 2] = 0.0;
            tm[5, 3] = p0 * vx;
            tm[5, 4] = p0 * vy;
            tm[5, 5] = p0 * vz;

            // ---------- calculate the output covariance matrix -----------
            double[,] tmt = MathTimeLibr.mattrans(tm, 6);
            double[,] tempm = MathTimeLibr.matmult(cartcov, tmt, 6, 6, 6);
            flcov = MathTimeLibr.matmult(tm, tempm, 6, 6, 6);
        }  //  covct2fl


        /* ----------------------------------------------------------------------------
        *
        * 
        *                        function covfl2ct
        *
        *  this function transforms a six by six covariance matrix expressed in
        *    flight elements into one expressed in cartesian elements.
        *
        *  author        : david vallado           davallado@gmail.com   27 may 2003
        *
        *  revisions
        *    vallado     - major update                                  26 aug 2015
        *
        *  inputs          description                          range / units
        *    flcov       - 6x6 flight covariance matrix
        *    flstate     - 6x1 flight orbit state               (r v latgc lon fpa az)
        *    anom        - anomaly                              'latlon', 'radec'
        *    ttt         - julian centuries of tt               centuries
        *    jdut1       - julian date of ut1                   days from 4713 bc
        *    lod         - excess length of day                 sec
        *    xp          - polar motion coefficient             arc sec
        *    yp          - polar motion coefficient             arc sec
        *    terms       - number of terms for ast calculation 0,2
        *    opt         - method option                           e80, e96, e00a, e00cio
        *
        *  outputs       :
        *    cartcov     - 6x6 cartesian covariance matrix
        *    tm          - transformation matrix
        *
        *  locals        :
        *    tm           - matrix of partial derivatives
        *    magr        - eci position vector magnitude        km
        *    magv        - eci velocity vector magnitude        km/sec
        *    latgc       - geocentric latitude                  rad
        *    lon         - longitude                            rad
        *    fpa         - sat flight path angle                rad
        *    az          - sat flight path az                   rad
        *    fpav        - sat flight path anglefrom vert       rad
        *    xe,ye,ze    - ecef position vector components      km
        *
        *  coupling      :
        *    ecef2eci    - convert eci vectors to ecef
        *
        *  references    :
        *    Vallado and Alfano 2015
        * ---------------------------------------------------------------------------- */

        public void covfl2ct
            (
            double[,] flcov, double[] flstate, string anomflt, double jdtt, double jdftt, double jdut1, 
            double jdxysstart, double lod,
            double xp, double yp, Int16 terms, double ddpsi, double ddeps, double ddx, double ddy, 
            EOPSPWLib.iau80Class iau80arr, EOPSPWLib.iau00Class iau00arr,
            EOpt opt,
            out double[,] cartcov, out double[,] tm
            )
        {
            double small, lon, latgc, fpa, cfpa, sfpa, az, decl, magr, magv, caz, saz, craf, sraf, cdf, sdf,
                cd, sd, cra, sra, temp, rtasc;
            double[] recef = new double[3];
            double[] vecef = new double[3];
            double[] reci = new double[3];
            double[] veci = new double[3];

            // initialize
            cd = 0.0;
            sd = 0.0;
            cdf = 0.0;
            sdf = 0.0;
            sra = 0.0;
            cra = 0.0;
            sraf = 0.0;
            craf = 0.0;

            tm = new double[,] { { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 } };

            small = 0.00000001;

            // -------- parse the input vectors into components
            lon = flstate[0]; // these will come in as either lon/lat or rtasc/decl dep}ing on anom1
            latgc = flstate[1];
            fpa = flstate[2];
            az = flstate[3];
            magr = flstate[4] * 1000.0;  // in m
            magv = flstate[5] * 1000.0;

            cfpa = Math.Cos(fpa);
            sfpa = Math.Sin(fpa);
            caz = Math.Cos(az);
            saz = Math.Sin(az);

            // --------- determine which set of variables is in use ---------
            // need to get eci vector
            if (anomflt.Equals("latlon"))
            {
                craf = Math.Cos(lon);  // earth fixed needed for the lon lat partials only
                sraf = Math.Sin(lon);
                cdf = Math.Cos(latgc);
                sdf = Math.Sin(latgc);
                recef[0] = magr * 0.001 * Math.Cos(latgc) * Math.Cos(lon);  // in km
                recef[1] = magr * 0.001 * Math.Cos(latgc) * Math.Sin(lon);
                recef[2] = magr * 0.001 * Math.Sin(latgc);
                // -------- convert r to eci
                // this vel is wrong but not needed except for special case ahead
                vecef[0] = magv * 0.001 * (-Math.Cos(lon) * Math.Sin(latgc) * caz * cfpa - Math.Sin(lon) * saz * cfpa + Math.Cos(lon) * Math.Cos(latgc) * sfpa); // m/s
                vecef[1] = magv * 0.001 * (-Math.Sin(lon) * Math.Sin(latgc) * caz * cfpa + Math.Cos(lon) * saz * cfpa + Math.Sin(lon) * Math.Cos(latgc) * sfpa);
                vecef[2] = magv * 0.001 * (Math.Sin(lon) * sfpa + Math.Cos(latgc) * caz * cfpa);
                double[] aecef = new double[] { 0, 0, 0 };
                eci_ecef(ref reci, ref veci, MathTimeLib.Edirection.efrom, ref recef, ref vecef,
                    EOpt.e80,
                    EOPSPWLibr.iau80arr, EOPSPWLibr.iau00arr,
                    jdtt, jdftt, jdut1, jdxysstart, lod, xp, yp, ddpsi, ddeps, ddx, ddy);

                reci[0] = reci[0] * 1000.0;  // in m
                reci[1] = reci[1] * 1000.0;
                reci[2] = reci[2] * 1000.0;
                veci[0] = veci[0] * 1000.0;  // in m/s
                veci[1] = veci[1] * 1000.0;
                veci[2] = veci[2] * 1000.0;

                temp = Math.Sqrt(reci[0] * reci[0] + reci[1] * reci[1]);
                if (temp < small)
                    rtasc = Math.Atan2(veci[1], veci[0]);
                else
                    rtasc = Math.Atan2(reci[1], reci[0]);

                //decl = atan2( reci[2] , Math.Sqrt(reci[0]^2 + reci[1]^2) )
                decl = Math.Asin(reci[2] / magr);
                cra = Math.Cos(rtasc);
                sra = Math.Sin(rtasc);
                cd = Math.Cos(decl);
                sd = Math.Sin(decl);
            }
            else  // radec
            {
                rtasc = lon;  // these come in as rtasc decl in this case
                decl = latgc;
                reci[0] = magr * Math.Cos(decl) * Math.Cos(rtasc);
                reci[1] = magr * Math.Cos(decl) * Math.Sin(rtasc);
                reci[2] = magr * Math.Sin(decl);
                veci[0] = magv * (-Math.Cos(rtasc) * Math.Sin(decl) * caz * cfpa - Math.Sin(rtasc) * saz * cfpa + Math.Cos(rtasc) * Math.Cos(decl) * sfpa); // m/s
                veci[1] = magv * (-Math.Sin(rtasc) * Math.Sin(decl) * caz * cfpa + Math.Cos(rtasc) * saz * cfpa + Math.Sin(rtasc) * Math.Cos(decl) * sfpa);
                veci[2] = magv * (Math.Sin(decl) * sfpa + Math.Cos(decl) * caz * cfpa);
                cra = Math.Cos(rtasc);
                sra = Math.Sin(rtasc);
                cd = Math.Cos(decl);
                sd = Math.Sin(decl);
            }

            // ---------------- calculate matrix elements ------------------
            // ---- partials of rx wrt (lon latgc fpa az r v)
            if (anomflt.Equals("radec"))
            {
                tm[0, 0] = -magr * cd * sra;
                tm[0, 1] = -magr * sd * cra;
            }
            else  // latlon
            {
                tm[0, 0] = -magr * cdf * sraf;
                tm[0, 1] = -magr * sdf * craf;
            }
            tm[0, 2] = 0.0;
            tm[0, 3] = 0.0;
            tm[0, 4] = cd * cra;
            tm[0, 5] = 0.0;

            // ---- partials of ry wrt (lon latgc fpa az r v)
            if (anomflt.Equals("radec"))
            {
                tm[1, 0] = magr * cd * cra;
                tm[1, 1] = -magr * sd * sra;
            }
            else  // latlon
            {
                tm[1, 0] = magr * cdf * craf;
                tm[1, 1] = -magr * sdf * sraf;
            }
            tm[1, 2] = 0.0;
            tm[1, 3] = 0.0;
            tm[1, 4] = cd * sra;
            tm[1, 5] = 0.0;

            // ---- partials of rz wrt (lon latgc fpa az r v)
            if (anomflt.Equals("radec"))
            {
                tm[2, 0] = 0.0;
                tm[2, 1] = magr * cd;
            }
            else  // latlon
            {
                tm[2, 0] = 0.0;
                tm[2, 1] = magr * cdf;
            }
            tm[2, 2] = 0.0;
            tm[2, 3] = 0.0;
            tm[2, 4] = sd;
            tm[2, 5] = 0.0;

            // ---- partials of vx wrt (lon latgc fpa az r v)
            if (anomflt.Equals("radec"))
            {
                tm[3, 0] = -magv * (-sra * caz * sd * cfpa + cra * saz * cfpa + cd * sra * sfpa);
                //  tm[3,0] = -vy;
                tm[3, 1] = -cra * magv * (sd * sfpa + cd * caz * cfpa);
                //  tm[3,1] = -vz*cra;
            }
            else  // latlon
            {
                tm[3, 0] = -magv * (-sraf * caz * sdf * cfpa + craf * saz * cfpa + cdf * sraf * sfpa);
                //  tm[3,0] = -vy;
                tm[3, 1] = -craf * magv * (sdf * sfpa + cdf * caz * cfpa);
                //  tm[3,1] = -vz*cra;
            }
            tm[3, 2] = magv * (cra * caz * sd * sfpa + sra * saz * sfpa + cd * cra * cfpa);
            tm[3, 3] = magv * (cra * saz * sd * cfpa - sra * caz * cfpa);
            tm[3, 4] = 0.0;
            tm[3, 5] = -cra * caz * sd * cfpa - sra * saz * cfpa + cd * cra * sfpa;

            // ---- partials of vy wrt (lon latgc fpa az r v)
            if (anomflt.Equals("radec"))
            {
                tm[4, 0] = magv * (-cra * caz * sd * cfpa - sra * saz * cfpa + cd * cra * sfpa);
                //  tm[4,0] = vx;
                tm[4, 1] = -sra * magv * (sd * sfpa + cd * caz * cfpa);
                //   tm[4,1] = -vz*sra;
            }
            else  // latlon
            {
                tm[4, 0] = magv * (-craf * caz * sdf * cfpa - sraf * saz * cfpa + cdf * craf * sfpa);
                //  tm[4,0] = vx;
                tm[4, 1] = -sraf * magv * (sdf * sfpa + cdf * caz * cfpa);
                //   tm[4,1] = -vz*sra;
            }
            tm[4, 2] = magv * (sra * caz * sd * sfpa - cra * saz * sfpa + cd * sra * cfpa);
            tm[4, 3] = magv * (sra * saz * sd * cfpa + cra * caz * cfpa);
            tm[4, 4] = 0.0;
            tm[4, 5] = -sra * caz * sd * cfpa + cra * saz * cfpa + cd * sra * sfpa;

            // ---- partials of vz wrt (lon latgc fpa az r v)
            if (anomflt.Equals("radec"))
            {
                tm[5, 0] = 0.0;
                tm[5, 1] = magv * (cd * sfpa - sd * caz * cfpa);
            }
            else  // latlon
            {
                tm[5, 0] = 0.0;
                tm[5, 1] = magv * (cdf * sfpa - sdf * caz * cfpa);
            }
            tm[5, 2] = magv * (sd * cfpa - cd * caz * sfpa);
            tm[5, 3] = -magv * cd * saz * cfpa;
            tm[5, 4] = 0.0;
            tm[5, 5] = sd * sfpa + cd * caz * cfpa;

            // ---------- calculate the output covariance matrix -----------
            double[,] tmt = MathTimeLibr.mattrans(tm, 6);
            double[,] tempm = MathTimeLibr.matmult(flcov, tmt, 6, 6, 6);
            cartcov = MathTimeLibr.matmult(tm, tempm, 6, 6, 6);
        }  //  covfl2ct



        /* ----------------------------------------------------------------------------
        *
        *                           function covct_rsw
        *
        *  this function transforms a six by six covariance matrix expressed in cartesian
        *    into one expressed in orbit plane, rsw frame
        *
        *  author        : david vallado           davallado@gmail.com   20 may 2003
        *
        *  inputs          description                          range / units
        *    cartcov     - 6x6 cartesian covariance matrix space delimited
        *    cartstate   - 6x1 cartesian orbit state            (x y z vx vy vz)
        *
        *  outputs       :
        *    covrsw      - 6x6 orbit plane rsw covariance matrix
        *
        *  locals        :
        *    r           - position vector                      m
        *    v           - velocity vector                      m/s
        *    tm          - transformation matrix
        *    temv        - temporary vector
        *
        *  coupling      :
        *    none
        *
        *  references    :
        *    Vallado and Alfano 2015
        * ---------------------------------------------------------------------------- */

        public void covct_rsw
            (
            ref double[,] cartcov, double[] cartstate, Enum direct, ref double[,] rswcov, out double[,] tm
            )
        {
            double x, y, z, vx, vy, vz;
            double[] r = new double[3];
            double[] v = new double[3];
            double[] sv = new double[3];
            double[] temv = new double[3];
            double[] wv = new double[3];
            double[] rv = new double[3];

            tm = new double[,] { { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 } };

            x = cartstate[0];  // m or km
            y = cartstate[1];
            z = cartstate[2];
            vx = cartstate[3];
            vy = cartstate[4];
            vz = cartstate[5];
            r = new double[] { x, y, z };
            v = new double[] { vx, vy, vz };

            rv = MathTimeLibr.norm(r);
            MathTimeLibr.cross(r, v, out temv);
            wv = MathTimeLibr.norm(temv);
            MathTimeLibr.cross(wv, rv, out sv);

            tm[0, 0] = rv[0];
            tm[0, 1] = rv[1];
            tm[0, 2] = rv[2];
            tm[1, 0] = sv[0];
            tm[1, 1] = sv[1];
            tm[1, 2] = sv[2];
            tm[2, 0] = wv[0];
            tm[2, 1] = wv[1];
            tm[2, 2] = wv[2];

            tm[3, 3] = rv[0];
            tm[3, 4] = rv[1];
            tm[3, 5] = rv[2];
            tm[4, 3] = sv[0];
            tm[4, 4] = sv[1];
            tm[4, 5] = sv[2];
            tm[5, 3] = wv[0];
            tm[5, 4] = wv[1];
            tm[5, 5] = wv[2];

            if (direct.Equals(MathTimeLib.Edirection.eto))
            {
                double[,] tmt = MathTimeLibr.mattrans(tm, 6);
                double[,] tempm = MathTimeLibr.matmult(cartcov, tmt, 6, 6, 6);
                rswcov = MathTimeLibr.matmult(tm, tempm, 6, 6, 6);
            }
            else
            {
                double[,] tmt = MathTimeLibr.mattrans(tm, 6);
                double[,] tempm = MathTimeLibr.matmult(rswcov, tm, 6, 6, 6);
                cartcov = MathTimeLibr.matmult(tmt, tempm, 6, 6, 6);
            }
        }  // covct_rsw


        /* ----------------------------------------------------------------------------
        *
        *                           function covct_ntw
        *
        *  this function transforms a six by six covariance matrix expressed in the
        *    cartesian frame to one expressed in the orbit plane (ntw)
        *
        *  author        : david vallado           davallado@gmail.com   17 jul 2003
        *
        *  revisions
        *    vallado     - s} out tm                                   25 jul 2003
        *
        *  inputs          description                          range / units
        *    covopntw    - 6x6 orbit plane ntw covariance matrix
        *    cartstate   - 6x1 cartesian orbit state            (x y z vx vy vz)
        *
        *  outputs       :
        *    cartcov     - 6x6 cartesian covariance matrix
        *
        *  locals        :
        *    r           - position vector                      m
        *    v           - velocity vector                      m/s
        *    tm          - transformation matrix
        *    temv        - temporary vector
        *
        *  coupling      :
        *    none
        *
        *  references    :
        *    Vallado and Alfano 2015
        * ---------------------------------------------------------------------------- */

        public void covct_ntw
            (
            ref double[,] cartcov, double[] cartstate, Enum direct, ref double[,] ntwcov, out double[,] tm
            )
        {
            double x, y, z, vx, vy, vz;
            double[] r = new double[3];
            double[] v = new double[3];
            double[] tv = new double[3];
            double[] temv = new double[3];
            double[] wv = new double[3];
            double[] nv = new double[3];

            tm = new double[,] { { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 } };

            x = cartstate[0];
            y = cartstate[1];
            z = cartstate[2];
            vx = cartstate[3];
            vy = cartstate[4];
            vz = cartstate[5];
            r = new double[] { x, y, z };
            v = new double[] { vx, vy, vz };

            tv = MathTimeLibr.norm(v);
            MathTimeLibr.cross(r, v, out temv);
            wv = MathTimeLibr.norm(temv);
            MathTimeLibr.cross(tv, wv, out nv);

            tm[0, 0] = nv[0];
            tm[0, 1] = nv[1];
            tm[0, 2] = nv[2];
            tm[1, 0] = tv[0];
            tm[1, 1] = tv[1];
            tm[1, 2] = tv[2];
            tm[2, 0] = wv[0];
            tm[2, 1] = wv[1];
            tm[2, 2] = wv[2];

            tm[3, 3] = nv[0];
            tm[3, 4] = nv[1];
            tm[3, 5] = nv[2];
            tm[4, 3] = tv[0];
            tm[4, 4] = tv[1];
            tm[4, 5] = tv[2];
            tm[5, 3] = wv[0];
            tm[5, 4] = wv[1];
            tm[5, 5] = wv[2];

            if (direct.Equals(MathTimeLib.Edirection.eto))
            {
                double[,] tmt = MathTimeLibr.mattrans(tm, 6);
                double[,] tempm = MathTimeLibr.matmult(cartcov, tmt, 6, 6, 6);
                ntwcov = MathTimeLibr.matmult(tm, tempm, 6, 6, 6);
            }
            else
            {
                double[,] tmt = MathTimeLibr.mattrans(tm, 6);
                double[,] tempm = MathTimeLibr.matmult(ntwcov, tm, 6, 6, 6);
                cartcov = MathTimeLibr.matmult(tmt, tempm, 6, 6, 6);
            }
        }  // covct_ntw

        /* ----------------------------------------------------------------------------
        *
        *                           function coveci_ecef
        *
        *  this function transforms a six by six covariance matrix expressed in eci coordinates
        *    into one expressed in the ecef frame. 
        *
        *  author        : david vallado           davallado@gmail.com   20 may 2003
        *
        *  inputs          description                          range / units
        *    ecicartcov  - 6x6 cartesian covariance matrix space delimited, in eci
        *    cartstate   - 6x1 cartesian orbit state            (x y z vx vy vz)
        *
        *  outputs       :
        *    ecefcartcov - 6x6 covariance matrix in ecef
        *
        *  locals        :
        *    r           - position vector                      m
        *    v           - velocity vector                      m/s
        *    tm          - transformation matrix
        *    temv        - temporary vector
        *
        *  coupling      :
        *    none
        *
        *  references    :
        *    Vallado and Alfano 2015
        * ---------------------------------------------------------------------------- */

        public void coveci_ecef
            (
            ref double[,] ecicartcov, double[] cartstate, Enum direct, ref double[,] ecefcartcov, out double[,] tm,
            EOPSPWLib.iau80Class iau80arr, 
            double ttt, double jdut1, double lod, double xp, double yp, int eqeterms, double ddpsi, double ddeps,
            EOpt opt
            )
        {
            double[] fArgs = new double[14];
            double psia, wa, epsa, chia;
            double meaneps, deltapsi, deltaeps, trueeps;
            double[] omegaearth = new double[3];
            double[] reci = new double[3];
            double[] veci = new double[3];
            double[] recef = new double[3];
            double[] vecef = new double[3];
            double[] rpef = new double[3];
            double[] vpef = new double[3];
            double[] crossr = new double[3];
            double[] tempvec1 = new double[3];
            double[,] prec = new double[3, 3];
            double[,] nut = new double[3, 3];
            double[,] st = new double[3, 3];
            double[,] pm = new double[3, 3];
            double[,] precp = new double[3, 3];
            double[,] nutp = new double[3, 3];
            double[,] stp = new double[3, 3];
            double[,] pmp = new double[3, 3];
            double[,] temp = new double[3, 3];
            double[,] trans = new double[3, 3];
            double[,] cov = new double[3, 3];
            double[,] tempcov = new double[3, 3];
            double[] cov1 = new double[3];
            double[,] transm = new double[6, 6];

            // zero out the transformation matrix
            tm = new double[,] { { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 } };

            deltapsi = 0.0;
            meaneps = 0.0;

            omegaearth[0] = 0.0;
            omegaearth[1] = 0.0;
            omegaearth[2] = gravConst.earthrot * (1.0 - lod / 86400.0);

            fundarg(ttt, opt, out fArgs);

            prec = precess(ttt, opt, out psia, out wa, out epsa, out chia);

            nut = nutation(ttt, ddpsi, ddeps, iau80arr, opt, fArgs, out deltapsi, out deltaeps, out trueeps, out meaneps);

            st = sidereal(jdut1, deltapsi, meaneps, fArgs, lod, eqeterms, opt);

            pm = polarm(xp, yp, ttt, opt);

            double x, y, z, vx, vy, vz;
            x = cartstate[0];  // m or km
            y = cartstate[1];
            z = cartstate[2];
            vx = cartstate[3];
            vy = cartstate[4];
            vz = cartstate[5];

            if (direct.Equals(MathTimeLib.Edirection.eto))
            {
                reci = new double[] { x, y, z };
                veci = new double[] { vx, vy, vz };

                // ---- perform transformations
                pmp = MathTimeLibr.mattrans(pm, 3);
                stp = MathTimeLibr.mattrans(st, 3);
                nutp = MathTimeLibr.mattrans(nut, 3);
                precp = MathTimeLibr.mattrans(prec, 3);

                // rpef needed for velocity transformation
                tm = MathTimeLibr.matmult(stp, nutp, 3, 3, 3);
                temp = MathTimeLibr.matmult(tm, precp, 3, 3, 3);
                rpef = MathTimeLibr.matvecmult(temp, reci, 3);

                // position covariance transformation
                tm = MathTimeLibr.matmult(pmp, stp, 3, 3, 3);
                trans = MathTimeLibr.matmult(tm, nutp, 3, 3, 3);
                tm = MathTimeLibr.matmult(trans, precp, 3, 3, 3);

                // form remaining transformation
                //MathTimeLibr.cross(omegaearth, rpef, out crossr);
                //// multiply by inverse of pm so it cancels out in final transformation
                //cov1 = MathTimeLibr.matvecmult(pm, crossr, 3);
                //transm = new double[,] { 
                //    { tm[0,0], tm[0,1], tm[0,2], 0, 0, 0 }, 
                //    { tm[1,0], tm[1,1], tm[1,2], 0, 0, 0 }, 
                //    { tm[2,0], tm[2,1], tm[2,2], 0, 0, 0 }, 
                //    { 0, 0, 0, cov1[0,0], cov1[0,1], cov1[0,2] },
                //    { 0, 0, 0, cov1[1,0], cov1[1,1], cov1[1,2] }, 
                //    { 0, 0, 0, cov1[2,0], cov1[2,1], cov1[2,2] } };
            }
            else
            {
                recef = new double[] { x, y, z };
                vecef = new double[] { vx, vy, vz };

                // ---- perform transformations
                rpef = MathTimeLibr.matvecmult(pm, recef, 3);

                tm = MathTimeLibr.matmult(prec, nut, 3, 3, 3);
                trans = MathTimeLibr.matmult(tm, st, 3, 3, 3);
                tm = MathTimeLibr.matmult(trans, pm, 3, 3, 3);

                // form remaining transformation
                //MathTimeLibr.cross(omegaearth, rpef, out crossr);
                //pmp = MathTimeLibr.mattrans(pm, 3);
                //cov1 = MathTimeLibr.matvecmult(pmp, crossr, 3);
                //transm = new double[,] { { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 },
                //                         { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 } };
            }

            if (direct.Equals(MathTimeLib.Edirection.eto))
            {
                double[,] tmt = MathTimeLibr.mattrans(tm, 6);
                double[,] tempm = MathTimeLibr.matmult(ecicartcov, tmt, 6, 6, 6);
                ecefcartcov = MathTimeLibr.matmult(tm, tempm, 6, 6, 6);
            }
            else
            {
                double[,] tmt = MathTimeLibr.mattrans(tm, 6);
                double[,] tempm = MathTimeLibr.matmult(ecefcartcov, tm, 6, 6, 6);
                ecicartcov = MathTimeLibr.matmult(tmt, tempm, 6, 6, 6);
            }
        }  // coveci_ecef


    }  // AstroLib

}  // namespace AstroMethods
