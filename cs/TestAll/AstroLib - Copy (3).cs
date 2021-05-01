/*     -------------------------------------------------------------------------
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
*       (w) 719-573-2600, email dvallado@agi.com, davallado@gmail.com
*
*    current :
*              13 feb 20  david vallado
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
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

using MathTimeMethods;  // MathTimeLib.globals, Edirection
using EOPSPWMethods;    // EOPDataClass, SPWDataClass, iau80Class, iau00Class


namespace AstroLibMethods
{
    public class AstroLib
    {
        public string AstroLibVersion = "AstroLib Version 2019-11-05";

        public enum EOpt
        {
            e80,  // FK5/IAU76
            e96,  // 1996
            e00a, // IAU2010a approx
            e00b, // IAU2010b approx, not used
            e00cio  // IAU2010 cio full
        };

        // class for gravity field data
        public class gravityModelData
        {
            public string name;
            public double[,] c = new double[2200, 2200];
            public double[,] s = new double[2200, 2200];
            public double[,] cNor = new double[2200, 2200];
            public double[,] sNor = new double[2200, 2200];
            public double[,] cSig = new double[2200, 2200];
            public double[,] sSig = new double[2200, 2200];
            public double mu = 398600.4415e9;  // default value, set with each model
            public double re = 6378136.3;      // default equatorial radius[m], set with each model
            public int maxsize = 2160;         // default max size gravity model data
        };
        public gravityModelData gravData = new gravityModelData();


        // setup the class so methods can be called
        public MathTimeLib MathTimeLibr = new MathTimeLib();

        public string EOPFileLoc;
        public string SPWFileLoc;
        public EOPSPWLib EOPSPWLibr = new EOPSPWLib();

        // setup the class so methods can be called
        // public SGP4Lib SGP4Libr = new SGP4Lib();

        // ------------------- jpl planetary ephemerides -------------------
        public const long jplsize = 60000;
        public Int32 jpldesize;
        public class jpldedataClass
        {
            public double[] rsun = new double[3];
            public double[] rmoon = new double[3];
            public Int32 year, mon, day;
            public double rsmag, rmmag, mjd;
        };
        // use a class for all the eop data so it can be processed
        public jpldedataClass[] jpldearr = new jpldedataClass[jplsize];



        // -----------------------------------------------------------------------------------------
        //                              transformation functions
        // -----------------------------------------------------------------------------------------


        /* -----------------------------------------------------------------------------
         *
         *                           function fundarg
         *
         *  this function calulates the delauany variables and planetary values for
         *  several theories.
         *
         *  author        : david vallado                  719-573-2600   16 jul 2004
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
         *    l           - mean anomaly of the moon                          rad
         *    l1          - mean anomaly of the Sun                           rad
         *    f           - mean longitude of the Moon minus that of asc node rad
         *    d           - mean elongation of the Moon from the Sun          rad
         *    omega       - mean longitude of the ascending node of the Moon  rad
         *    planetary longitudes                                            rad
         *
         *  locals        :
         *
         *  coupling      :
         *    none        -
         *
         *  references    :
         *    vallado       2013, 210-211, 225
         * --------------------------------------------------------------------------- */

        public void fundarg
            (
            double ttt, EOpt opt,
            out double l, out double l1, out double f, out double d, out double omega,
            out double lonmer, out double lonven, out double lonear, out double lonmar,
            out double lonjup, out double lonsat, out double lonurn, out double lonnep,
            out double precrate
            )
        {
            double deg2rad, oo3600;

            deg2rad = Math.PI / 180.0;
            // arcsec to deg
            oo3600 = 1.0 / 3600.0;
            l = l1 = f = d = omega = lonmer = lonven = lonear = lonmar = lonjup = lonsat = lonurn = lonnep = precrate = 0.0;

            // ---- determine coefficients for various iers nutation theories ----
            // ----  iau-2010 cio theory and iau-2000a theory
            if (opt.Equals(EOpt.e00cio) || opt.Equals(EOpt.e00a))
            {
                // ------ form the delaunay fundamental arguments in ", converted to rad
                l = ((((-0.00024470 * ttt + 0.051635) * ttt + 31.8792) * ttt + 1717915923.2178) * ttt + 485868.249036) * oo3600;
                l1 = ((((-0.00001149 * ttt + 0.000136) * ttt - 0.5532) * ttt + 129596581.0481) * ttt + 1287104.793048) * oo3600;
                f = ((((+0.00000417 * ttt - 0.001037) * ttt - 12.7512) * ttt + 1739527262.8478) * ttt + 335779.526232) * oo3600;
                d = ((((-0.00003169 * ttt + 0.006593) * ttt - 6.3706) * ttt + 1602961601.2090) * ttt + 1072260.703692) * oo3600;
                omega = ((((-0.00005939 * ttt + 0.007702) * ttt + 7.4722) * ttt - 6962890.5431) * ttt + 450160.398036) * oo3600;

                // ------ form the planetary arguments in ", converted to rad
                lonmer = (908103.259872 + 538101628.688982 * ttt) * oo3600;
                lonven = (655127.283060 + 210664136.433548 * ttt) * oo3600;
                lonear = (361679.244588 + 129597742.283429 * ttt) * oo3600;
                lonmar = (1279558.798488 + 68905077.493988 * ttt) * oo3600;
                lonjup = (123665.467464 + 10925660.377991 * ttt) * oo3600;
                lonsat = (180278.799480 + 4399609.855732 * ttt) * oo3600;
                lonurn = (1130598.018396 + 1542481.193933 * ttt) * oo3600;
                lonnep = (1095655.195728 + 786550.320744 * ttt) * oo3600;
                precrate = ((1.112022 * ttt + 5028.8200) * ttt) * oo3600;
                // these are close (all in rad) - usually 1e-10, but some are as high as 1e-06
                //lonmer = (4.402608842 + 2608.7903141574 * ttt) % twopi;
                //lonven = (3.176146697 + 1021.3285546211 * ttt) % twopi;
                //lonear = (1.753470314 + 628.3075849991 * ttt) % twopi;
                //lonmar = (6.203480913 + 334.0612426700 * ttt) % twopi;
                //lonjup = (0.599546497 + 52.9690962641 * ttt) % twopi;
                //lonsat = (0.874016757 + 21.3299104960 * ttt) % twopi;
                //lonurn = (5.481293872 + 7.4781598567 * ttt) % twopi;
                //lonnep = (5.311886287 + 3.8133035638 * ttt) % twopi;
                //precrate = (0.024381750 + 0.00000538691 * ttt ) *ttt;
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

            // ---- iau-1980 theory
            if (opt.Equals(EOpt.e80))
            {
                // ------ form the delaunay fundamental arguments in deg
                l = ((((0.064) * ttt + 31.310) * ttt + 1717915922.6330) * ttt) * oo3600 + 134.96298139;
                l1 = ((((-0.012) * ttt - 0.577) * ttt + 129596581.2240) * ttt) * oo3600 + 357.52772333;
                f = ((((0.011) * ttt - 13.257) * ttt + 1739527263.1370) * ttt) * oo3600 + 93.27191028;
                d = ((((0.019) * ttt - 6.891) * ttt + 1602961601.3280) * ttt) * oo3600 + 297.85036306;
                omega = ((((0.008) * ttt + 7.455) * ttt - 6962890.5390) * ttt) * oo3600 + 125.04452222;
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

            // ---- convert units from deg to rad except e00a which is already in rad
            l = (l % 360.0) * deg2rad;
            l1 = (l1 % 360.0) * deg2rad;
            f = (f % 360.0) * deg2rad;
            d = (d % 360.0) * deg2rad;
            omega = (omega % 360.0) * deg2rad;

            lonmer = (lonmer % 360.0) * deg2rad;
            lonven = (lonven % 360.0) * deg2rad;
            lonear = (lonear % 360.0) * deg2rad;
            lonmar = (lonmar % 360.0) * deg2rad;
            lonjup = (lonjup % 360.0) * deg2rad;
            lonsat = (lonsat % 360.0) * deg2rad;
            lonurn = (lonurn % 360.0) * deg2rad;
            lonnep = (lonnep % 360.0) * deg2rad;
            precrate = (precrate % 360.0) * deg2rad;
        }  //  fundarg 



        /* ----------------------------------------------------------------------------
        *
        *                           function iau00xys
        *
        *  this function calulates the transformation matrix that accounts for the
        *    effects of precession-nutation in the iau2010 theory.
        *
        *  author        : david vallado                  719-573-2600   16 jul 2004
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
        *    a0xi        - integer coefficients for x
        *    ays0        - real coefficients for y                          rad
        *    a0yi        - integer coefficients for y
        *    ass0        - real coefficients for s                          rad
        *    a0si        - integer coefficients for s
        *    apn         - real coefficients for nutation                   rad
        *    apni        - integer coefficients for nutation
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
        *    fundarg     - find the fundamental arguments
        *
        *  references    : 
        *    vallado       2013, 212-214
        * ---------------------------------------------------------------------------- */

        public double[,] iau00xys
            (
            double ttt, double ddx, double ddy, EOpt opt,
            EOPSPWLib.iau00Class iau00arr,
            out double x, out double y, out double s
            )
        {
            double ttt2, ttt3, ttt4, ttt5;
            int i, j;
            double[,] nut1 = new double[3, 3];
            double[,] nut2 = new double[3, 3];
            double a, sum0, sum1, sum2, sum3, sum4;
            double l, l1, f, d, omega, lonmer, lonven, lonear, lonmar, lonjup, lonsat, lonurn, lonnep, precrate, tempval;

            // " to rad
            double convrt = Math.PI / (180.0 * 3600.0);

            ttt2 = ttt * ttt;
            ttt3 = ttt2 * ttt;
            ttt4 = ttt2 * ttt2;
            ttt5 = ttt3 * ttt2;

            // iau00in(nutLoc, iau00arr);

            fundarg(ttt, opt, out l, out l1, out f, out d, out omega, out lonmer, out lonven, out lonear, out lonmar, out lonjup, out lonsat,
                out lonurn, out lonnep, out precrate);

            // ---------------- find x
            // the iers code puts the constants in here, however
            // don't sum constants in here because they're larger than the last few terms
            sum0 = 0.0;
            for (i = 1305; i >= 0; i--)
            {
                tempval = iau00arr.a0xi[i, 0] * l + iau00arr.a0xi[i, 1] * l1 + iau00arr.a0xi[i, 2] * f + iau00arr.a0xi[i, 3] * d + iau00arr.a0xi[i, 4] * omega +
                           iau00arr.a0xi[i, 5] * lonmer + iau00arr.a0xi[i, 6] * lonven + iau00arr.a0xi[i, 7] * lonear + iau00arr.a0xi[i, 8] * lonmar +
                           iau00arr.a0xi[i, 9] * lonjup + iau00arr.a0xi[i, 10] * lonsat + iau00arr.a0xi[i, 11] * lonurn + iau00arr.a0xi[i, 12] * lonnep +
                           iau00arr.a0xi[i, 13] * precrate;
                sum0 = sum0 + iau00arr.axs0[i, 0] * Math.Sin(tempval) + iau00arr.axs0[i, 1] * Math.Cos(tempval);
            }
            sum1 = 0.0;
            // note that the index changes here to j. this is because the a0xi etc
            // indicies go from 1 to 1600, but there are 5 groups.the i index counts through each
            // calculation, and j takes care of the individual summations. note that
            // this same process is used for y and s.
            for (j = 252; j >= 0; j--)
            {
                i = 1306 + j;
                tempval = iau00arr.a0xi[i, 0] * l + iau00arr.a0xi[i, 1] * l1 + iau00arr.a0xi[i, 2] * f + iau00arr.a0xi[i, 3] * d + iau00arr.a0xi[i, 4] * omega +
                          iau00arr.a0xi[i, 5] * lonmer + iau00arr.a0xi[i, 6] * lonven + iau00arr.a0xi[i, 7] * lonear + iau00arr.a0xi[i, 8] * lonmar +
                          iau00arr.a0xi[i, 9] * lonjup + iau00arr.a0xi[i, 10] * lonsat + iau00arr.a0xi[i, 11] * lonurn + iau00arr.a0xi[i, 12] *
                          lonnep + iau00arr.a0xi[i, 13] * precrate;
                sum1 = sum1 + iau00arr.axs0[i, 0] * Math.Sin(tempval) + iau00arr.axs0[i, 1] * Math.Cos(tempval);
            }
            sum2 = 0.0;
            for (j = 35; j >= 0; j--)
            {
                i = 1306 + 253 + j;
                tempval = iau00arr.a0xi[i, 0] * l + iau00arr.a0xi[i, 1] * l1 + iau00arr.a0xi[i, 2] * f + iau00arr.a0xi[i, 3] * d + iau00arr.a0xi[i, 4] * omega +
                          iau00arr.a0xi[i, 5] * lonmer + iau00arr.a0xi[i, 6] * lonven + iau00arr.a0xi[i, 7] * lonear + iau00arr.a0xi[i, 8] * lonmar +
                          iau00arr.a0xi[i, 9] * lonjup + iau00arr.a0xi[i, 10] * lonsat + iau00arr.a0xi[i, 11] * lonurn + iau00arr.a0xi[i, 12] *
                          lonnep + iau00arr.a0xi[i, 13] * precrate;
                sum2 = sum2 + iau00arr.axs0[i, 0] * Math.Sin(tempval) + iau00arr.axs0[i, 1] * Math.Cos(tempval);
            }
            sum3 = 0.0;
            for (j = 3; j >= 0; j--)
            {
                i = 1306 + 253 + 36 + j;
                tempval = iau00arr.a0xi[i, 0] * l + iau00arr.a0xi[i, 1] * l1 + iau00arr.a0xi[i, 2] * f + iau00arr.a0xi[i, 3] * d + iau00arr.a0xi[i, 4] * omega +
                          iau00arr.a0xi[i, 5] * lonmer + iau00arr.a0xi[i, 6] * lonven + iau00arr.a0xi[i, 7] * lonear + iau00arr.a0xi[i, 8] * lonmar +
                          iau00arr.a0xi[i, 9] * lonjup + iau00arr.a0xi[i, 10] * lonsat + iau00arr.a0xi[i, 11] * lonurn + iau00arr.a0xi[i, 12] *
                          lonnep + iau00arr.a0xi[i, 13] * precrate;
                sum3 = sum3 + iau00arr.axs0[i, 0] * Math.Sin(tempval) + iau00arr.axs0[i, 1] * Math.Cos(tempval);
            }
            sum4 = 0.0;
            for (j = 0; j >= 0; j--)
            {
                i = 1306 + 253 + 36 + 4 + j;
                tempval = iau00arr.a0xi[i, 0] * l + iau00arr.a0xi[i, 1] * l1 + iau00arr.a0xi[i, 2] * f + iau00arr.a0xi[i, 3] * d + iau00arr.a0xi[i, 4] * omega +
                          iau00arr.a0xi[i, 5] * lonmer + iau00arr.a0xi[i, 6] * lonven + iau00arr.a0xi[i, 7] * lonear + iau00arr.a0xi[i, 8] * lonmar +
                          iau00arr.a0xi[i, 9] * lonjup + iau00arr.a0xi[i, 10] * lonsat + iau00arr.a0xi[i, 11] * lonurn + iau00arr.a0xi[i, 12] *
                          lonnep + iau00arr.a0xi[i, 13] * precrate;
                sum4 = sum4 + iau00arr.axs0[i, 0] * Math.Sin(tempval) + iau00arr.axs0[i, 1] * Math.Cos(tempval);
            }

            x = -0.016617 + 2004.191898 * ttt - 0.4297829 * ttt2
                - 0.19861834 * ttt3 - 0.000007578 * ttt4 + 0.0000059285 * ttt5; // "
            x = x * convrt + sum0 + sum1 * ttt + sum2 * ttt2 + sum3 * ttt3 + sum4 * ttt4;  // rad

            // ---------------- now find y
            sum0 = 0.0;
            for (i = 961; i >= 0; i--)
            {
                tempval = iau00arr.a0yi[i, 0] * l + iau00arr.a0yi[i, 1] * l1 + iau00arr.a0yi[i, 2] * f + iau00arr.a0yi[i, 3] * d + iau00arr.a0yi[i, 4] * omega +
                          iau00arr.a0yi[i, 5] * lonmer + iau00arr.a0yi[i, 6] * lonven + iau00arr.a0yi[i, 7] * lonear + iau00arr.a0yi[i, 8] * lonmar +
                          iau00arr.a0yi[i, 9] * lonjup + iau00arr.a0yi[i, 10] * lonsat + iau00arr.a0yi[i, 11] * lonurn + iau00arr.a0yi[i, 12] *
                          lonnep + iau00arr.a0yi[i, 13] * precrate;
                sum0 = sum0 + iau00arr.ays0[i, 0] * Math.Sin(tempval) + iau00arr.ays0[i, 1] * Math.Cos(tempval);
            }

            sum1 = 0.0;
            for (j = 276; j >= 0; j--)
            {
                i = 962 + j;
                tempval = iau00arr.a0yi[i, 0] * l + iau00arr.a0yi[i, 1] * l1 + iau00arr.a0yi[i, 2] * f + iau00arr.a0yi[i, 3] * d + iau00arr.a0yi[i, 4] * omega +
                          iau00arr.a0yi[i, 5] * lonmer + iau00arr.a0yi[i, 6] * lonven + iau00arr.a0yi[i, 7] * lonear + iau00arr.a0yi[i, 8] * lonmar +
                          iau00arr.a0yi[i, 9] * lonjup + iau00arr.a0yi[i, 10] * lonsat + iau00arr.a0yi[i, 11] * lonurn + iau00arr.a0yi[i, 12] *
                          lonnep + iau00arr.a0yi[i, 13] * precrate;
                sum1 = sum1 + iau00arr.ays0[i, 0] * Math.Sin(tempval) + iau00arr.ays0[i, 1] * Math.Cos(tempval);
            }
            sum2 = 0.0;
            for (j = 29; j >= 0; j--)
            {
                i = 962 + 277 + j;
                tempval = iau00arr.a0yi[i, 0] * l + iau00arr.a0yi[i, 1] * l1 + iau00arr.a0yi[i, 2] * f + iau00arr.a0yi[i, 3] * d + iau00arr.a0yi[i, 4] * omega +
                          iau00arr.a0yi[i, 5] * lonmer + iau00arr.a0yi[i, 6] * lonven + iau00arr.a0yi[i, 7] * lonear + iau00arr.a0yi[i, 8] * lonmar +
                          iau00arr.a0yi[i, 9] * lonjup + iau00arr.a0yi[i, 10] * lonsat + iau00arr.a0yi[i, 11] * lonurn + iau00arr.a0yi[i, 12] *
                          lonnep + iau00arr.a0yi[i, 13] * precrate;
                sum2 = sum2 + iau00arr.ays0[i, 0] * Math.Sin(tempval) + iau00arr.ays0[i, 1] * Math.Cos(tempval);
            }
            sum3 = 0.0;
            for (j = 4; j >= 0; j--)
            {
                i = 962 + 277 + 30 + j;
                tempval = iau00arr.a0yi[i, 0] * l + iau00arr.a0yi[i, 1] * l1 + iau00arr.a0yi[i, 2] * f + iau00arr.a0yi[i, 3] * d + iau00arr.a0yi[i, 4] * omega +
                          iau00arr.a0yi[i, 5] * lonmer + iau00arr.a0yi[i, 6] * lonven + iau00arr.a0yi[i, 7] * lonear + iau00arr.a0yi[i, 8] * lonmar +
                          iau00arr.a0yi[i, 9] * lonjup + iau00arr.a0yi[i, 10] * lonsat + iau00arr.a0yi[i, 11] * lonurn + iau00arr.a0yi[i, 12] *
                          lonnep + iau00arr.a0yi[i, 13] * precrate;
                sum3 = sum3 + iau00arr.ays0[i, 0] * Math.Sin(tempval) + iau00arr.ays0[i, 1] * Math.Cos(tempval);
            }
            sum4 = 0.0;
            for (j = 0; j >= 0; j--)
            {
                i = 962 + 277 + 30 + 5 + j;
                tempval = iau00arr.a0yi[i, 0] * l + iau00arr.a0yi[i, 1] * l1 + iau00arr.a0yi[i, 2] * f + iau00arr.a0yi[i, 3] * d + iau00arr.a0yi[i, 4] * omega +
                          iau00arr.a0yi[i, 5] * lonmer + iau00arr.a0yi[i, 6] * lonven + iau00arr.a0yi[i, 7] * lonear + iau00arr.a0yi[i, 8] * lonmar +
                          iau00arr.a0yi[i, 9] * lonjup + iau00arr.a0yi[i, 10] * lonsat + iau00arr.a0yi[i, 11] * lonurn + iau00arr.a0yi[i, 12] *
                          lonnep + iau00arr.a0yi[i, 13] * precrate;
                sum4 = sum4 + iau00arr.ays0[i, 0] * Math.Sin(tempval) + iau00arr.ays0[i, 1] * Math.Cos(tempval);
            }

            y = -0.006951 - 0.025896 * ttt - 22.4072747 * ttt2
                + 0.00190059 * ttt3 + 0.001112526 * ttt4 + 0.0000001358 * ttt5;  // "
            y = y * convrt + sum0 + sum1 * ttt + sum2 * ttt2 + sum3 * ttt3 + sum4 * ttt4;  // rad


            // ---------------- now find s
            sum0 = 0.0;
            for (i = 32; i >= 0; i--)
            {
                tempval = iau00arr.a0si[i, 0] * l + iau00arr.a0si[i, 1] * l1 + iau00arr.a0si[i, 2] * f + iau00arr.a0si[i, 3] * d + iau00arr.a0si[i, 4] * omega +
                      iau00arr.a0si[i, 5] * lonmer + iau00arr.a0si[i, 6] * lonven + iau00arr.a0si[i, 7] * lonear + iau00arr.a0si[i, 8] * lonmar +
                      iau00arr.a0si[i, 9] * lonjup + iau00arr.a0si[i, 10] * lonsat + iau00arr.a0si[i, 11] * lonurn + iau00arr.a0si[i, 12] *
                      lonnep + iau00arr.a0si[i, 13] * precrate;
                sum0 = sum0 + iau00arr.ass0[i, 0] * Math.Sin(tempval) + iau00arr.ass0[i, 1] * Math.Cos(tempval);
            }
            sum1 = 0.0;
            for (j = 2; j >= 0; j--)
            {
                i = 33 + j;
                tempval = iau00arr.a0si[i, 0] * l + iau00arr.a0si[i, 1] * l1 + iau00arr.a0si[i, 2] * f + iau00arr.a0si[i, 3] * d + iau00arr.a0si[i, 4] * omega +
                          iau00arr.a0si[i, 5] * lonmer + iau00arr.a0si[i, 6] * lonven + iau00arr.a0si[i, 7] * lonear + iau00arr.a0si[i, 8] * lonmar +
                          iau00arr.a0si[i, 9] * lonjup + iau00arr.a0si[i, 10] * lonsat + iau00arr.a0si[i, 11] * lonurn + iau00arr.a0si[i, 12] *
                          lonnep + iau00arr.a0si[i, 13] * precrate;
                sum1 = sum1 + iau00arr.ass0[i, 0] * Math.Sin(tempval) + iau00arr.ass0[i, 1] * Math.Cos(tempval);
            }
            sum2 = 0.0;
            for (j = 24; j >= 0; j--)
            {
                i = 33 + 3 + j;
                tempval = iau00arr.a0si[i, 0] * l + iau00arr.a0si[i, 1] * l1 + iau00arr.a0si[i, 2] * f + iau00arr.a0si[i, 3] * d + iau00arr.a0si[i, 4] * omega +
                          iau00arr.a0si[i, 5] * lonmer + iau00arr.a0si[i, 6] * lonven + iau00arr.a0si[i, 7] * lonear + iau00arr.a0si[i, 8] * lonmar +
                          iau00arr.a0si[i, 9] * lonjup + iau00arr.a0si[i, 10] * lonsat + iau00arr.a0si[i, 11] * lonurn + iau00arr.a0si[i, 12] *
                          lonnep + iau00arr.a0si[i, 13] * precrate;
                sum2 = sum2 + iau00arr.ass0[i, 0] * Math.Sin(tempval) + iau00arr.ass0[i, 1] * Math.Cos(tempval);
            }
            sum3 = 0.0;
            for (j = 3; j >= 0; j--)
            {
                i = 33 + 3 + 25 + j;
                tempval = iau00arr.a0si[i, 0] * l + iau00arr.a0si[i, 1] * l1 + iau00arr.a0si[i, 2] * f + iau00arr.a0si[i, 3] * d + iau00arr.a0si[i, 4] * omega +
                          iau00arr.a0si[i, 5] * lonmer + iau00arr.a0si[i, 6] * lonven + iau00arr.a0si[i, 7] * lonear + iau00arr.a0si[i, 8] * lonmar +
                          iau00arr.a0si[i, 9] * lonjup + iau00arr.a0si[i, 10] * lonsat + iau00arr.a0si[i, 11] * lonurn + iau00arr.a0si[i, 12] *
                          lonnep + iau00arr.a0si[i, 13] * precrate;
                sum3 = sum3 + iau00arr.ass0[i, 0] * Math.Sin(tempval) + iau00arr.ass0[i, 1] * Math.Cos(tempval);
            }
            sum4 = 0.0;
            for (j = 0; j >= 0; j--)
            {
                i = 33 + 3 + 25 + 4 + j;
                tempval = iau00arr.a0si[i, 0] * l + iau00arr.a0si[i, 1] * l1 + iau00arr.a0si[i, 2] * f + iau00arr.a0si[i, 3] * d + iau00arr.a0si[i, 4] * omega +
                          iau00arr.a0si[i, 5] * lonmer + iau00arr.a0si[i, 6] * lonven + iau00arr.a0si[i, 7] * lonear + iau00arr.a0si[i, 8] * lonmar +
                          iau00arr.a0si[i, 9] * lonjup + iau00arr.a0si[i, 10] * lonsat + iau00arr.a0si[i, 11] * lonurn + iau00arr.a0si[i, 12] *
                          lonnep + iau00arr.a0si[i, 13] * precrate;
                sum4 = sum4 + iau00arr.ass0[i, 0] * Math.Sin(tempval) + iau00arr.ass0[i, 1] * Math.Cos(tempval);
            }

            s = 0.000094 + 0.00380865 * ttt - 0.00012268 * ttt2
                - 0.07257411 * ttt3 + 0.00002798 * ttt4 + 0.00001562 * ttt5;   // "
            //            + 0.00000171*ttt* Math.Sin(omega) + 0.00000357*ttt* Math.Cos(2.0*omega)  
            //            + 0.00074353*ttt2* Math.Sin(omega) + 0.00005691*ttt2* Math.Sin(2.0*(f-d+omega))  
            //            + 0.00000984*ttt2* Math.Sin(2.0*(f+omega)) - 0.00000885*ttt2* Math.Sin(2.0*omega);
            s = -x * y * 0.5 + s * convrt + sum0 + sum1 * ttt + sum2 * ttt2 + sum3 * ttt3 + sum4 * ttt4;  // rad


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
            //  d = atan(sqrt((x^2 + y^2) / (1.0-x^2-y^2)) );
            //  nut1 = rot3mat(-e)*rot2mat(-d)*rot3mat(e+s)
        }  // iau00xys



        /* ----------------------------------------------------------------------------
 *
 *                           function iau00pn
 *
 *  this function calulates the transformation matrix that accounts for the
 *    effects of precession-nutation in the iau2010 theory.
 *
 *  author        : david vallado                  719-573-2600   16 jul 2004
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
 *    a0xi        - integer coefficients for x
 *    ays0        - real coefficients for y                          rad
 *    a0yi        - integer coefficients for y
 *    ass0        - real coefficients for s                          rad
 *    a0si        - integer coefficients for s
 *    apn         - real coefficients for nutation                   rad
 *    apni        - integer coefficients for nutation
 *    appl        - real coefficients for planetary nutation         rad
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
 *    fundarg     - find the fundamental arguments
 *
 *  references    : 
 *    vallado       2013, 212-214
 * ---------------------------------------------------------------------------- */

        public double[,] iau00pn
            (
            double ttt, double ddx, double ddy, EOpt opt,
            EOPSPWLib.iau00Class iau00arr,
            double x, double y, double s
            )
        {
            double ttt2, ttt3, ttt4, ttt5;
            double[,] nut1 = new double[3, 3];
            double[,] nut2 = new double[3, 3];
            double a;
            double l, l1, f, d, omega, lonmer, lonven, lonear, lonmar, lonjup, lonsat, lonurn, lonnep, precrate;

            ttt2 = ttt * ttt;
            ttt3 = ttt2 * ttt;
            ttt4 = ttt2 * ttt2;
            ttt5 = ttt3 * ttt2;

            // iau00in(nutLoc, iau00arr);

            fundarg(ttt, opt, out l, out l1, out f, out d, out omega, out lonmer, out lonven, out lonear, out lonmar, out lonjup, out lonsat,
                out lonurn, out lonnep, out precrate);

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
            //  d = atan(sqrt((x^2 + y^2) / (1.0-x^2-y^2)) );
            //  nut1 = rot3mat(-e)*rot2mat(-d)*rot3mat(e+s)
        }  // iau00pn


        /* -----------------------------------------------------------------------------
*
*                           function gstime00
*
*  this function finds the greenwich sidereal time (iau-2010).
*
*  author        : david vallado                  719-573-2600    1 mar 2001
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
            double jdut1, double deltapsi, double ttt, EOPSPWLib.iau00Class iau00arr, EOpt opt, out double gst
            )
        {
            double[,] st = new double[3, 3];
            const double twopi = 2.0 * Math.PI;
            const double deg2rad = Math.PI / 180.0;
            double convrt, ttt2, ttt3, ttt4, ttt5, epsa, tempval, gstsum0, gstsum1;
            double l, l1, f, d, lonmer, lonven, lonear, lonmar, lonjup, lonsat, lonurn, lonnep, precrate, raan,
                   eect2000, ee2000, tut1d, era, gmst2000;
            Int32 i, j;

            // " to rad
            convrt = Math.PI / (180.0 * 3600.0);

            ttt2 = ttt * ttt;
            ttt3 = ttt2 * ttt;
            ttt4 = ttt2 * ttt2;
            ttt5 = ttt3 * ttt2;

            // mean obliquity of the ecliptic
            // see sofa code obl06.f (no iau_ in front)
            epsa = 84381.406 - 46.836769 * ttt - 0.0001831 * ttt2 + 0.00200340 * ttt3 - 0.000000576 * ttt4 - 0.0000000434 * ttt5; // "
            epsa = (epsa / 3600.0 % 360.0);  // deg

            epsa = epsa * deg2rad; // rad

            fundarg(ttt, opt, out l, out l1, out f, out d, out raan, out lonmer, out lonven, out lonear, out lonmar,
                     out lonjup, out lonsat, out lonurn, out lonnep, out precrate);

            //  evaluate the ee complementary terms
            gstsum0 = 0.0;
            // data file is not reversed
            for (i = 32; i >= 0; i--)
            {
                tempval = iau00arr.agsti[i, 0] * l + iau00arr.agsti[i, 1] * l1 + iau00arr.agsti[i, 2] * f + iau00arr.agsti[i, 3] * d + iau00arr.agsti[i, 4] * raan +
                          iau00arr.agsti[i, 5] * lonmer + iau00arr.agsti[i, 6] * lonven + iau00arr.agsti[i, 7] * lonear + iau00arr.agsti[i, 8] * lonmar +
                          iau00arr.agsti[i, 9] * lonjup + iau00arr.agsti[i, 10] * lonsat + iau00arr.agsti[i, 11] * lonurn + iau00arr.agsti[i, 12] * lonnep + iau00arr.agsti[i, 13] * precrate;
                gstsum0 = gstsum0 + iau00arr.agst[i, 0] * Math.Sin(tempval) + iau00arr.agst[i, 1] * Math.Cos(tempval);
            }

            gstsum1 = 0.0;
            // data file is not reversed
            for (j = 0; j >= 0; j--)
            {
                i = 32 + j;
                tempval = iau00arr.agsti[i, 0] * l + iau00arr.agsti[i, 1] * l1 + iau00arr.agsti[i, 2] * f + iau00arr.agsti[i, 3] * d + iau00arr.agsti[i, 4] * raan +
                          iau00arr.agsti[i, 5] * lonmer + iau00arr.agsti[i, 6] * lonven + iau00arr.agsti[i, 7] * lonear + iau00arr.agsti[i, 8] * lonmar +
                          iau00arr.agsti[i, 9] * lonjup + iau00arr.agsti[i, 10] * lonsat + iau00arr.agsti[i, 11] * lonurn + iau00arr.agsti[i, 12] * lonnep + iau00arr.agsti[i, 13] * precrate;
                gstsum1 = gstsum1 + (iau00arr.agst[i, 0] * Math.Sin(tempval) + iau00arr.agst[i, 1] * Math.Cos(tempval)) * ttt;
            }

            eect2000 = gstsum0 + gstsum1 * ttt;  // rad

            // equation of the equinoxes
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
          *  author        : david vallado                  719-573-2600    1 mar 2001
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
            temp = -6.2e-6 * tut1 * tut1 * tut1 + 0.093104 * tut1 * tut1 +
                    (876600.0 * 3600 + 8640184.812866) * tut1 + 67310.54841;  // sec
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
	    *  author        : david vallado                  719-573-2600    1 mar 2001
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
        *                           function precess
        *
        *  this function calulates the transformation matrix that accounts for the effects
        *    of precession. both the 1980 and 2000 theories are handled. note that the
        *    required parameters differ a little.
        *
        *  author        : david vallado                  719-573-2600   25 jun 2002
        *
        *  revisions
        *    vallado     - conversion to c++                             21 feb 2005
        *    vallado     - misc updates, nomenclature, etc               23 nov 2005
        *    vallado     - conversion to c#                              16 Nov 2011
        *
        *  inputs          description                                 range / units
        *    ttt         - julian centuries of tt
        *    opt         - method option                           e80, e96, e00a, e00cio
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

            return prec;
        }  //  precess 


        /* -----------------------------------------------------------------------------
        *
        *                           function nutation
        *
        *  this function calulates the transformation matrix that accounts for the
        *    effects of nutation.
        *
        *  author        : david vallado                  719-573-2600   27 jun 2002
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
        *    raan        -                                                   rad
        *    nut         - transform matrix for tod - mod
        *
        *  locals        :
        *    iar80       - integers for fk5 1980
        *    rar80       - reals for fk5 1980                                rad
        *    l           -                                                   rad
        *    ll          -                                                   rad
        *    f           -                                                   rad
        *    d           -                                                   rad
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
            EOPSPWLib.iau80Class iau80arr, EOpt opt,
            out double deltapsi, out double deltaeps, out double trueeps, out double meaneps, out double raan
            )
        {
            double[,] nut = new double[3, 3];
            double deg2rad, l, l1, f, d,
                   lonmer, lonven, lonear, lonmar, lonjup, lonsat, lonurn, lonnep, precrate,
                   cospsi, sinpsi, coseps, sineps, costrueeps, sintrueeps;
            int i;
            double tempval;

            deg2rad = Math.PI / 180.0;

            // ---- determine coefficients for iau 1980 nutation theory ----
            meaneps = ((0.001813 * ttt - 0.00059) * ttt - 46.8150) * ttt + 84381.448;
            meaneps = (meaneps / 3600.0 % 360.0);
            meaneps = meaneps * deg2rad;

            fundarg(ttt, opt, out l, out l1, out f, out d, out raan,
                     out lonmer, out lonven, out lonear, out lonmar,
                     out lonjup, out lonsat, out lonurn, out lonnep, out precrate);

            deltapsi = 0.0;
            deltaeps = 0.0;
            for (i = 105; i >= 0; i--)
            {
                tempval = iau80arr.iar80[i, 0] * l + iau80arr.iar80[i, 1] * l1 + iau80arr.iar80[i, 2] * f +
                         iau80arr.iar80[i, 3] * d + iau80arr.iar80[i, 4] * raan;
                deltapsi = deltapsi + (iau80arr.rar80[i, 0] + iau80arr.rar80[i, 1] * ttt) * Math.Sin(tempval);
                deltaeps = deltaeps + (iau80arr.rar80[i, 2] + iau80arr.rar80[i, 3] * ttt) * Math.Cos(tempval);
            }

            // --------------- find nutation parameters --------------------
            deltapsi = (deltapsi + ddpsi) % (2.0 * Math.PI);
            deltaeps = (deltaeps + ddeps) % (2.0 * Math.PI);

            //      Console.WriteLine(String.Format("meaneps {0}  deltapsi {1}  deltaeps {2} ttt {3} ", meaneps * 180 / Math.PI, deltapsi * 180 / Math.PI, deltaeps * 180 / Math.PI, ttt));

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
            //astMath::MathTimeLibr.rot1mat(trueeps, n1);
            //astMath::rot3mat(deltapsi, n2);
            //astMath::MathTimeLibr.rot1mat(-meaneps, n3);
            //astMath::MathTimeLibr.matmult(n2, n1, tr1, 3, 3, 3);
            //astMath::MathTimeLibr.matmult(n3, tr1, nut, 3, 3, 3);

            return nut;
        }  //  nutation 


        /* ----------------------------------------------------------------------------
        %
        %                           function nutation00a
        %
        *  this function calulates the transformation matrix that accounts for the
        *    effects of precession-nutation in the iau2000a theory.
        *
        *  author        : david vallado                  719-573-2600   16 jul 2004
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
        *    l           - delaunay element               rad
        *    ll          - delaunay element               rad
        *    f           - delaunay element               rad
        *    d           - delaunay element               rad
        *    omega       - delaunay element               rad
        *    many others for planetary values             rad
        *
        *  locals        :
        *    x           - coordinate rad
        *    y           - coordinate rad
        *    s           - coordinate rad
        *    axs0        - real coefficients for x rad
        *    a0xi        - integer coefficients for x
        *    ays0        - real coefficients for y rad
        *    a0yi        - integer coefficients for y
        *    ass0        - real coefficients for s rad
        *    a0si        - integer coefficients for s
        *    apn         - real coefficients for nutation rad
        *    apni        - integer coefficients for nutation
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
        *
        * [deltapsi, pnb, nut, l, l1, f, d, omega, ...
        *   lonmer, lonven, lonear, lonmar, lonjup, lonsat, lonurn, lonnep, precrate...
        * ] = iau00pna(ttt);
        * ---------------------------------------------------------------------------- */

        public double[,] nutation00a
            (
            double ttt, double ddpsi, double ddeps,
            EOPSPWLib.iau00Class iau00arr, EOpt opt
            )
        {
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
            double l, l1, f, d, psia, wa, epsa, chia, deltapsi, deltaeps,
                    lonmer, lonven, lonear, lonmar, lonjup, lonsat, lonurn, lonnep, precrate;
            int i;
            double tempval, convrt, ttt2, ttt3, ttt4, ttt5, j2d, raan;
            double pnsum, ensum, pplnsum, eplnsum, oblo;

            deltapsi = deltaeps = 0.0;

            // " to rad
            convrt = Math.PI / (180.0 * 3600.0);

            ttt2 = ttt * ttt;
            ttt3 = ttt2 * ttt;
            ttt4 = ttt2 * ttt2;
            ttt5 = ttt3 * ttt2;

            fundarg(ttt, opt, out l, out l1, out f, out d, out raan, out lonmer, out lonven, out lonear, out lonmar,
                     out lonjup, out lonsat, out lonurn, out lonnep, out precrate);

            // ---- obtain data coefficients
            // iau2006 approach - does not seem to be correct, close though
            if (opt.Equals(EOpt.e00cio))
            {
                // looks like they still use the iau2000a method and adjust
                pnsum = 0.0;
                // data file is not not reveresed
                for (i = 1357; i >= 0; i--)
                {
                    tempval = iau00arr.apni[i, 0] * l + iau00arr.apni[i, 1] * l1 + iau00arr.apni[i, 2] * f + iau00arr.apni[i, 3] * d + iau00arr.apni[i, 4] * raan +
                              iau00arr.apni[i, 5] * lonmer + iau00arr.apni[i, 6] * lonven + iau00arr.apni[i, 7] * lonear + iau00arr.apni[i, 8] * lonmar +
                              iau00arr.apni[i, 9] * lonjup + iau00arr.apni[i, 10] * lonsat + iau00arr.apni[i, 11] * lonurn + iau00arr.apni[i, 12] * lonnep + iau00arr.apni[i, 13] * precrate;
                    if (i > 1319)
                        pnsum = pnsum + (iau00arr.apn[i, 0] * Math.Sin(tempval) + iau00arr.apn[i, 1] * Math.Cos(tempval)) * ttt;  //note that sin and cos are reveresed between n and e
                    else
                        pnsum = pnsum + iau00arr.apn[i, 0] * Math.Sin(tempval) + iau00arr.apn[i, 1] * Math.Cos(tempval);
                }

                ensum = 0.0;
                // data file is not reveresed
                for (i = 1055; i >= 0; i--)
                {
                    tempval = iau00arr.apei[i, 0] * l + iau00arr.apei[i, 1] * l1 + iau00arr.apei[i, 2] * f + iau00arr.apei[i, 3] * d + iau00arr.apei[i, 4] * raan +
                              iau00arr.apei[i, 5] * lonmer + iau00arr.apei[i, 6] * lonven + iau00arr.apei[i, 7] * lonear + iau00arr.apei[i, 8] * lonmar +
                              iau00arr.apei[i, 9] * lonjup + iau00arr.apei[i, 10] * lonsat + iau00arr.apei[i, 11] * lonurn + iau00arr.apei[i, 12] * lonnep + iau00arr.apei[i, 13] * precrate;
                    if (i > 1036)
                        ensum = ensum + (iau00arr.ape[i, 0] * Math.Cos(tempval) + iau00arr.ape[i, 1] * Math.Sin(tempval)) * ttt;
                    else
                        ensum = ensum + iau00arr.ape[i, 0] * Math.Cos(tempval) + iau00arr.ape[i, 1] * Math.Sin(tempval);
                }
                //  add planetary and luni-solar components.
                deltapsi = pnsum;  // rad
                deltaeps = ensum;

                // iau2006 corrections to the iau2000a
                j2d = -2.7774e-6 * ttt * convrt;  // rad
                deltapsi = deltapsi + deltapsi * (0.4697e-6 + j2d);  // rad
                deltaeps = deltaeps + deltaeps * j2d;
            }

            if (opt.Equals(EOpt.e00a))
            {
                pnsum = 0.0;
                ensum = 0.0;
                for (i = 677; i >= 0; i--)
                {
                    tempval = iau00arr.appni[i, 0] * l + iau00arr.appni[i, 1] * l1 + iau00arr.appni[i, 2] * f + iau00arr.appni[i, 3] * d + iau00arr.appni[i, 4] * raan;
                    tempval = tempval % (2.0 * Math.PI);  // rad
                                                          //            pnsum = pnsum + (apn[i,1) + apn[i,2)*ttt) * sin(tempval) 
                                                          //                          + (apn[i,5) + apn[i,6)*ttt) * cos(tempval);
                                                          //            ensum = ensum + (apn[i,3) + apn[i,4)*ttt) * cos(tempval) 
                                                          //                          + (apn[i,7) + apn[i,8)*ttt) * sin(tempval);
                                                          // iers doesn't include the last few terms
                    pnsum = pnsum + (iau00arr.appn[i, 0] + iau00arr.appn[i, 1] * ttt) * Math.Sin(tempval)
                                      + (iau00arr.appn[i, 4]) * Math.Cos(tempval);
                    ensum = ensum + (iau00arr.appn[i, 2] + iau00arr.appn[i, 3] * ttt) * Math.Cos(tempval)
                                      + (iau00arr.appn[i, 6]) * Math.Sin(tempval);
                }

                pplnsum = 0.0;
                eplnsum = 0.0;
                // data file is already reveresed
                for (i = 686; i >= 0; i--)
                {
                    tempval = iau00arr.aplni[i, 0] * l + iau00arr.aplni[i, 1] * l1 + iau00arr.aplni[i, 2] * f + iau00arr.aplni[i, 3] * d + iau00arr.aplni[i, 4] * raan +
                              iau00arr.aplni[i, 5] * lonmer + iau00arr.aplni[i, 6] * lonven + iau00arr.aplni[i, 7] * lonear + iau00arr.aplni[i, 8] * lonmar +
                              iau00arr.aplni[i, 9] * lonjup + iau00arr.aplni[i, 10] * lonsat + iau00arr.aplni[i, 11] * lonurn + iau00arr.aplni[i, 12] * lonnep + iau00arr.aplni[i, 13] * precrate;
                    pplnsum = pplnsum + iau00arr.apln[i, 0] * Math.Sin(tempval) + iau00arr.apln[i, 1] * Math.Cos(tempval);
                    eplnsum = eplnsum + iau00arr.apln[i, 2] * Math.Sin(tempval) + iau00arr.apln[i, 3] * Math.Cos(tempval);
                }

                //  add planetary and luni-solar components.
                deltapsi = pnsum + pplnsum;  // rad
                deltaeps = ensum + eplnsum;
            }

            // 2000b has 77 terms
            if (opt.Equals(EOpt.e00b))
            {

            }

            prec = precess(ttt, opt, out psia, out wa, out epsa, out chia);

            oblo = 84381.406 * convrt; // " to rad or 448 - 406 for iau2006????

            // ----------------- find nutation matrix ----------------------
            // mean to true
            a1 = MathTimeLibr.rot1mat(epsa + deltaeps);
            a2 = MathTimeLibr.rot3mat(deltapsi);
            a3 = MathTimeLibr.rot1mat(-epsa);

            // j2000 to date(precession)
            a4 = MathTimeLibr.rot3mat(-chia);
            a5 = MathTimeLibr.rot1mat(wa);
            a6 = MathTimeLibr.rot3mat(psia);
            a7 = MathTimeLibr.rot1mat(-oblo);

            // icrs to j2000
            a8 = MathTimeLibr.rot1mat(-0.0068192 * convrt);
            a9 = MathTimeLibr.rot2mat(0.0417750 * Math.Sin(oblo) * convrt);
            //      a9  = rot2mat(0.0166170*convrt);
            a10 = MathTimeLibr.rot3mat(0.0146 * convrt);

            tr1 = MathTimeLibr.matmult(a5, a4, 3, 3, 3);
            prec = MathTimeLibr.matmult(tr1, a6, 3, 3, 3);
            tr1 = MathTimeLibr.matmult(a7, prec, 3, 3, 3);
            prec = MathTimeLibr.matmult(tr1, a8, 3, 3, 3);
            tr1 = MathTimeLibr.matmult(a9, prec, 3, 3, 3);
            prec = MathTimeLibr.matmult(tr1, a10, 3, 3, 3);

            tr1 = MathTimeLibr.matmult(a2, a1, 3, 3, 3);
            nut = MathTimeLibr.matmult(tr1, a3, 3, 3, 3);

            return MathTimeLibr.matmult(prec, nut, 3, 3, 3);
        }   // nutation00a



        /* -----------------------------------------------------------------------------
        *
        *                           function nutationqmod
        *
        *  this function calulates the transformation matrix that accounts for the
        *    effects of nutation within the qmod paradigm. There are several assumptions. 
        *
        *  author        : david vallado                  719-573-2600   27 jun 2002
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
        *    raan        -                                                rad
        *    nut         - transform matrix for tod - mod
        *
        *  locals        :
        *    iar80       - integers for fk5 1980
        *    rar80       - reals for fk5 1980                             rad
        *    l           -                                                rad
        *    ll          -                                                rad
        *    f           -                                                rad
        *    d           -                                                rad
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
            EOPSPWLib.iau80Class iau80arr, EOpt opt,
            out double deltapsi, out double deltaeps, out double meaneps, out double raan
            )
        {
            double[,] nut = new double[3, 3];
            double[,] n1 = new double[3, 3];
            double[,] n2 = new double[3, 3];
            double deg2rad, l, l1, f, d, sineps,
                   lonmer, lonven, lonear, lonmar, lonjup, lonsat, lonurn, lonnep, precrate;
            int i;
            double tempval;

            deg2rad = Math.PI / 180.0;

            // ---- determine coefficients for iau 1980 nutation theory ----
            //  assumption for mean eps
            meaneps = 84381.448;
            meaneps = ((meaneps / 3600.0) % 360.0);
            meaneps = meaneps * deg2rad;  // rad

            //  assumption that planetary terms are not used later on
            fundarg(ttt, opt, out l, out l1, out f, out d, out raan,
                     out lonmer, out lonven, out lonear, out lonmar,
                     out lonjup, out lonsat, out lonurn, out lonnep, out precrate);

            deltapsi = 0.0;
            deltaeps = 0.0;
            //  assumption that only largest 9 terms are used
            for (i = 8; i >= 0; i--)
            {
                tempval = EOPSPWLibr.iau80arr.iar80[i, 0] * l + EOPSPWLibr.iau80arr.iar80[i, 1] * l1 + EOPSPWLibr.iau80arr.iar80[i, 2] * f +
                         EOPSPWLibr.iau80arr.iar80[i, 3] * d + EOPSPWLibr.iau80arr.iar80[i, 4] * raan;
                deltapsi = deltapsi + (EOPSPWLibr.iau80arr.rar80[i, 0] + EOPSPWLibr.iau80arr.rar80[i, 1] * ttt) * Math.Sin(tempval);
                deltaeps = deltaeps + (EOPSPWLibr.iau80arr.rar80[i, 2] + EOPSPWLibr.iau80arr.rar80[i, 3] * ttt) * Math.Cos(tempval);
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
        *  this function calulates the transformation matrix that accounts for the
        *    effects of sidereal time. Notice that deltaspi should not be moded to a
        *    positive number because it is multiplied rather than used in a
        *    trigonometric argument.
        *
        *  author        : david vallado                  719-573-2600   25 jun 2002
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
        *    raan       - long of asc node of moon                           rad
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
            double jdut1, double deltapsi, double meaneps, double raan,
            double lod, int eqeterms, EOpt opt
            )
        {
            double gmst, ast, thetasa, omegaearth, sinast, cosast;
            double era, tut1d;
            double[,] stdot = new double[3, 3];
            double[,] st = new double[3, 3];
            ast = era = 0.0;

            // FK5 approach
            if ((opt.Equals(EOpt.e80)) | (opt.Equals(EOpt.e96)))
            {
                // ------------------------ find gmst --------------------------
                gmst = gstime(jdut1);

                // ------------------------ find mean ast ----------------------
                if ((jdut1 > 2450449.5) && (eqeterms > 0))
                {
                    ast = gmst + deltapsi * Math.Cos(meaneps) + 0.00264 * Math.PI / (3600.0 * 180.0) * Math.Sin(raan)
                                + 0.000063 * Math.PI / (3600.0 * 180.0) * Math.Sin(2.0 * raan);
                }
                else
                    ast = gmst + deltapsi * Math.Cos(meaneps);

                ast = (ast % (2.0 * Math.PI));
            }
		else  // IAU 2010 approach
            {
                // julian centuries of ut1
                tut1d = jdut1 - 2451545.0;

                era = Math.PI * 2.0 * (0.7790572732640 + 1.00273781191135448 * tut1d);
                era = (era % (2.0 * Math.PI));

                ast = era;  // set this for the matrix calcs below
            }

            thetasa = MathTimeLib.globals.earthrot * (1.0 - lod / 86400.0);
            omegaearth = thetasa;

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
            stdot[0, 0] = -omegaearth * sinast;
            stdot[0, 1] = -omegaearth * cosast;
            stdot[0, 2] = 0.0;
            stdot[1, 0] = omegaearth * cosast;
            stdot[1, 1] = -omegaearth * sinast;
            stdot[1, 2] = 0.0;
            stdot[2, 0] = 0.0;
            stdot[2, 1] = 0.0;
            stdot[2, 2] = 0.0;

            return st;
        }  //  sidereal 


        /* -----------------------------------------------------------------------------
        *
        *                           function polarm
        *
        *  this function calulates the transformation matrix that accounts for polar
        *    motion within the fk5 and iau2010 systems. 
        *
        *  author        : david vallado                  719-573-2600   25 jun 2002
        *
        *  revisions
        *    vallado     - conversion to c++                             23 nov 2005
        *    vallado     - conversion to c#                              16 Nov 2011
        *
        *  inputs          description                                 range / units
        *    xp          - polar motion coefficient                         rad
        *    yp          - polar motion coefficient                         rad
        *    ttt         - julian centuries of tt (00 theory only)
        *    opt         - method option                           e80, e96, e00a, e00cio
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

            // FK5 approach
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
                //astMath::rot2mat(xp, a1);
                //astMath::rot1mat(yp, a2);
                //astMath::MathTimeLibr.matmult(a2, a1, pm, 3, 3, 3);
            }
            else
            // IAU-2010 approach
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
                //astMath::rot1mat(yp, a1);
                //astMath::rot2mat(xp, a2);
                //astMath::rot3mat(-sp, a3);
                //astMath::MathTimeLibr.matmult(a2, a1, tr1, 3, 3, 3);
                //astMath::MathTimeLibr.matmult(a3, tr1, pm, 3, 3, 3);
            }

            return pm;
        }  //  polarm 


        /* -----------------------------------------------------------------------------
        *
        *                           function framebias
        *
        *  this function calulates the transformation matrix that accounts for frame
        *    bias.
        *
        *  author        : david vallado                  719-573-2600   19 sep 05
        *
        *  revisions
        *
        *  inputs          description                                 range / units
        *    opt         - frame bias method option                 'j' j2000, 'f' fk5
        *
        *  outputs       :
        *    term1       - alpha delta o                                rad
        *    term2       - psi deltab sin(eps deltao)                   rad
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
        *    the eci mean equator mean equinox (j2000).
        *
        *  author        : david vallado                  719-573-2600    4 jun 2002
        *
        *  revisions
        *    vallado     - add terms for ast calculation                 30 sep 2002
        *    vallado     - consolidate with iau 2000                     14 feb 2005
        *
        *  inputs          description                                 range / units
        *    recef       - position vector earth fixed                 km
        *    vecef       - velocity vector earth fixed                 km/s
        *    ttt         - julian centuries of tt                      centuries
        *    jdut1       - julian date of ut1                          days from 4713 bc
        *    lod         - excess length of day                        sec
        *    xp          - polar motion coefficient                    rad
        *    yp          - polar motion coefficient                    rad
        *    eqeterms    - terms for ast calculation                   0,2
        *    ddpsi       - delta psi correction to gcrf                rad
        *    ddeps       - delta eps correction to gcrf                rad
        *    opt         - method option                           e80, e96, e00a, e00cio
        *
        *  outputs       :
        *    reci        - position vector eci                         km
        *    veci        - velocity vector eci                         km/s
        *
        *  locals        :
        *    deltapsi    - nutation angle                              rad
        *    trueeps     - true obliquity of the ecliptic              rad
        *    meaneps     - mean obliquity of the ecliptic              rad
        *    raan       -                                              rad
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
            ref double[] reci, ref double[] veci, EOPSPWLib.iau80Class iau80arr, Enum direct,
            double ttt, double jdut1, double lod, double xp, double yp, int eqeterms, double ddpsi, double ddeps,
            EOpt opt,
            ref double[] recef, ref double[] vecef
            )
        {
            double psia, wa, epsa, chia;
            double meaneps, raan, deltapsi, deltaeps, trueeps;
            double[] omegaearth = new double[3];
            double[] rpef = new double[3];
            double[] vpef = new double[3];
            double[] crossr = new double[3];
            double[] tempvec1 = new double[3];
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

            deltapsi = 0.0;
            meaneps = 0.0;
            raan = 0.0;

            omegaearth[0] = 0.0;
            omegaearth[1] = 0.0;
            omegaearth[2] = MathTimeLib.globals.earthrot * (1.0 - lod / 86400.0);

            prec = precess(ttt, opt, out psia, out wa, out epsa, out chia);

            nut = nutation(ttt, ddpsi, ddeps, iau80arr, opt, out deltapsi, out deltaeps, out trueeps, out meaneps, out raan);

            st = sidereal(jdut1, deltapsi, meaneps, raan, lod, eqeterms, opt);

            pm = polarm(xp, yp, ttt, opt);

            if (direct.Equals(MathTimeLib.Edirection.eto))
            {
                // ---- perform transformations
                pmp = MathTimeLibr.mattrans(pm, 3);
                stp = MathTimeLibr.mattrans(st, 3);
                nutp = MathTimeLibr.mattrans(nut, 3);
                precp = MathTimeLibr.mattrans(prec, 3);

                temp = MathTimeLibr.matmult(stp, nutp, 3, 3, 3);
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
                trans = MathTimeLibr.matmult(temp, st, 3, 3, 3);
                reci = MathTimeLibr.matvecmult(trans, rpef, 3);

                vpef = MathTimeLibr.matvecmult(pm, vecef, 3);
                MathTimeLibr.cross(omegaearth, rpef, out crossr);
                tempvec1[0] = vpef[0] + crossr[0];
                tempvec1[1] = vpef[1] + crossr[1];
                tempvec1[2] = vpef[2] + crossr[2];
                veci = MathTimeLibr.matvecmult(trans, tempvec1, 3);
            }
        }  //  eci2ecef 



        /* ----------------------------------------------------------------------------
        *
        *                           function eci_ecef00
        *
        *  this function transforms between the earth fixed (itrf) frame, and
        *    the eci mean equator mean equinox (j2000).
        *
        *  author        : david vallado                  719-573-2600    4 jun 2002
        *
        *  revisions
        *    vallado     - add terms for ast calculation                 30 sep 2002
        *    vallado     - consolidate with iau 2000                     14 feb 2005
        *
        *  inputs          description                                   range / units
        *    recef       - position vector earth fixed                   km
        *    vecef       - velocity vector earth fixed                   km/s
        *    ttt         - julian centuries of tt                        centuries
        *    jdut1       - julian date of ut1                            days from 4713 bc
        *    lod         - excess length of day                          sec
        *    xp          - polar motion coefficient                      rad
        *    yp          - polar motion coefficient                      rad
        *    eqeterms    - terms for ast calculation                     0,2
        *    ddpsi       - delta psi correction to gcrf                  rad
        *    ddeps       - delta eps correction to gcrf                  rad
        *    opt         - method option                           e80, e96, e00a, e00cio
        *
        *  outputs       :
        *    reci        - position vector eci                           km
        *    veci        - velocity vector eci                           km/s
        *
        *  locals        :
        *    deltapsi    - nutation angle                                rad
        *    trueeps     - true obliquity of the ecliptic                rad
        *    meaneps     - mean obliquity of the ecliptic                rad
        *    raan       -                                                rad
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

        public void eci_ecef00
            (
            ref double[] reci, ref double[] veci, EOPSPWLib.iau00Class iau00arr, Enum direct,
            double ttt, double jdut1, double lod, double xp, double yp, int eqeterms, double ddx, double ddy,
            EOpt opt,
            ref double[] recef, ref double[] vecef
            )
        {
            double psia, wa, epsa, chia;
            double x, y, s;
            double meaneps, raan, deltapsi, gst;
            double[] omegaearth = new double[3];
            double[] rpef = new double[3];
            double[] vpef = new double[3];
            double[] crossr = new double[3];
            double[] tempvec1 = new double[3];
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

            deltapsi = 0.0;
            meaneps = 0.0;
            raan = 0.0;

            // IAU-2010 CIO approach
            if (opt.Equals(EOpt.e00cio))
            {
                prec[0, 0] = 1.0;
                prec[1, 1] = 1.0;
                prec[2, 2] = 1.0;
                nut = iau00xys(ttt, ddx, ddy, opt, iau00arr, out x, out y, out s);
                st = sidereal(jdut1, deltapsi, meaneps, raan, lod, eqeterms, opt);
            }
            else
            // IAU-2010 pna approach
            {
                prec = precess(ttt, opt, out psia, out wa, out epsa, out chia);
                nut = nutation00a(ttt, ddx, ddy, iau00arr, opt);
                st = gstime00(jdut1, deltapsi, ttt, iau00arr, opt, out gst);
            }

            omegaearth[0] = 0.0;
            omegaearth[1] = 0.0;
            omegaearth[2] = MathTimeLib.globals.earthrot * (1.0 - lod / 86400.0);

            pm = polarm(xp, yp, ttt, opt);

            if (direct.Equals(MathTimeLib.Edirection.eto))
            {
                // ---- perform transformations
                pmp = MathTimeLibr.mattrans(pm, 3);
                stp = MathTimeLibr.mattrans(st, 3);
                nutp = MathTimeLibr.mattrans(nut, 3);
                precp = MathTimeLibr.mattrans(prec, 3);

                temp = MathTimeLibr.matmult(stp, nutp, 3, 3, 3);
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
                trans = MathTimeLibr.matmult(temp, st, 3, 3, 3);
                reci = MathTimeLibr.matvecmult(trans, rpef, 3);

                vpef = MathTimeLibr.matvecmult(pm, vecef, 3);
                MathTimeLibr.cross(omegaearth, rpef, out crossr);
                tempvec1[0] = vpef[0] + crossr[0];
                tempvec1[1] = vpef[1] + crossr[1];
                tempvec1[2] = vpef[2] + crossr[2];
                veci = MathTimeLibr.matvecmult(trans, tempvec1, 3);
            }

        }  //  eci2ecef00 



        /* ----------------------------------------------------------------------------
        *
        *                           function tod_ecef
        *
        *  this function transforms a vector from true of date (tod) to 
        *  earth fixed (itrf) frame.
        *
        *  author        : david vallado                  719-573-2600    4 jun 2002
        *
        *  revisions
        *    vallado     - add terms for ast calculation                 30 sep 2002
        *    vallado     - consolidate with iau 2000                     14 feb 2005
        *
        *  inputs          description                                   range / units
        *    rtod        - position vector eci                           km
        *    vtod        - velocity vector eci                           km/s
        *    ttt         - julian centuries of tt                        centuries
        *    jdut1       - julian date of ut1                            days from 4713 bc
        *    lod         - excess length of day                          sec
        *    xp          - polar motion coefficient                      rad
        *    yp          - polar motion coefficient                      rad
        *    eqeterms    - terms for ast calculation                     0,2
        *    ddpsi       - delta psi correction to gcrf                  rad
        *    ddeps       - delta eps correction to gcrf                  rad
        *    opt         - method option                           e80, e96, e00a, e00cio
        *
        *  outputs       :
        *    recef       - position vector earth fixed                   km
        *    vecef       - velocity vector earth fixed                   km/s
        *
        *  locals        :
        *    deltapsi    - nutation angle                                rad
        *    trueeps     - true obliquity of the ecliptic                rad
        *    meaneps     - mean obliquity of the ecliptic                rad
        *    raan       -                                                rad
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
        *    vallado       2013, 228-236
        * ---------------------------------------------------------------------------- */

        public void tod_ecef
            (
            ref double[] rtod, ref double[] vtod, Enum direct,
            double ttt, double jdut1, double lod, double xp, double yp, int eqeterms, double ddpsi, double ddeps,
            EOPSPWLib.iau80Class iau80arr, EOpt opt,
            ref double[] recef, ref double[] vecef
            )
        {
            double meaneps, raan, deltapsi, deltaeps, trueeps;
            double[] omegaearth = new double[3];
            double[] rpef = new double[3];
            double[] vpef = new double[3];
            double[] crossr = new double[3];
            double[] tempvec1 = new double[3];
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
            deltapsi = 0.0;
            meaneps = 0.0;
            raan = 0.0;

            omegaearth[0] = 0.0;
            omegaearth[1] = 0.0;
            omegaearth[2] = MathTimeLib.globals.earthrot * (1.0 - lod / 86400.0);

            nut = nutation(ttt, ddpsi, ddeps, iau80arr, opt, out deltapsi, out deltaeps, out trueeps, out meaneps, out raan);

            st = sidereal(jdut1, deltapsi, meaneps, raan, lod, eqeterms, opt);

            pm = polarm(xp, yp, ttt, opt);

            if (direct.Equals(MathTimeLib.Edirection.eto))
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
        }  //  tod2ecef 



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
        *    opt         - method option                           e80, e96, e00a, e00cio
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
             ref double[] rteme, ref double[] vteme, Enum direct, double ttt, double jdut1, double lod, double xp, double yp, Int32 eqeterms,
             EOpt opt,
             ref double[] recef, ref double[] vecef
            )
        {
            double deg2rad, raan, gmstg, thetasa;
            double[] omegaearth = new double[3];
            double[,] st = new double[3, 3];
            double[,] stdot = new double[3, 3];
            double[,] pm = new double[3, 3];
            double[] rpef = new double[3];
            double[] vpef = new double[3];
            double[] crossr = new double[3];
            double[,] stp = new double[3, 3];
            double[,] pmp = new double[3, 3];
            double gmst;

            raan = 0.0;

            deg2rad = Math.PI / 180.0;

            // ------------------------find gmst--------------------------
            gmst = gstime(jdut1);

            // teme does not include the geometric terms here
            // after 1997, kinematic terms apply
            if ((jdut1 > 2450449.5) && (eqeterms > 0))
            {
                gmstg = gmst
                    + 0.00264 * Math.PI / (3600.0 * 180.0) * Math.Sin(raan)
                    + 0.000063 * Math.PI / (3600.0 * 180.0) * Math.Sin(2.0 * raan);
            }
            else
                gmstg = gmst;
            gmstg = gmstg % (2.0 * Math.PI);

            // find omega from nutation theory
            raan = 125.04452222 + (-6962890.5390 * ttt +
                7.455 * ttt * ttt + 0.008 * ttt * ttt * ttt) / 3600.0;
            raan = raan % (360.0 * deg2rad);

            thetasa = MathTimeLib.globals.earthrot * (1.0 - lod / 86400.0);
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
        *    opt         - method option                           e80, e96, e00a, e00cio
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
            ref double[] rteme, ref double[] vteme, EOPSPWLib.iau80Class iau80arr, Enum direct, double ttt, double ddpsi, double ddeps,
            EOpt opt,
            ref double[] rgcrf, ref double[] vgcrf
            )
        {
            double[,] prec = new double[3, 3];
            double[,] nut = new double[3, 3];
            double[,] precp = new double[3, 3];
            double[,] nutp = new double[3, 3];
            double[,] eqe = new double[3, 3];
            double[,] eqep = new double[3, 3];
            double[,] temp = new double[3, 3];
            double[,] tempmat = new double[3, 3];
            double psia, wa, epsa, chia, deltapsi, deltaeps, trueeps, meaneps, raan, eqeg;

            prec = precess(ttt, EOpt.e80, out psia, out wa, out epsa, out chia);
            nut = nutation(ttt, ddpsi, ddeps, iau80arr, opt, out deltapsi, out deltaeps, out trueeps, out meaneps, out raan);

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
        *  author        : david vallado                  719-573-2600   27 jun 2002
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
        *    raan       -                                                rad
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
            double[] rqmod, double[] vqmod, double ttt, double jdutc, EOPSPWLib.iau80Class iau80arr, EOpt opt,
            out double[] recef, out double[] vecef
            )
        {
            double gmst, ed, deg2rad;
            double meaneps, raan, deltapsi, deltaeps;
            double[,] nut = new double[3, 3];
            double[,] st = new double[3, 3];
            double[,] nutp = new double[3, 3];
            double[,] stp = new double[3, 3];
            double[,] tm = new double[3, 3];
            double[] crossr = new double[3];
            double[] omegaearth = new double[3];
            omegaearth[2] = MathTimeLib.globals.earthrot;

            nut = nutationqmod(ttt, iau80arr, opt, out deltapsi, out deltaeps, out meaneps, out raan);

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
        *  author        : david vallado                  719-573-2600   18 nov 2011
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
        *    opt         - method option                           e80, e96, e00a, e00cio
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
        *    raan       -                                  rad
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
            double psia, wa, epsa, chia;
            double meaneps, raan, deltapsi, deltaeps, trueeps;
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
            omegaearth[2] = MathTimeLib.globals.earthrot * (1.0 - lod / 86400.0);

            prec = precess(ttt, opt, out psia, out wa, out epsa, out chia);

            nut = nutation(ttt, ddpsi, ddeps, EOPSPWLibr.iau80arr, opt, out deltapsi, out deltaeps, out trueeps, out meaneps, out raan);

            st = sidereal(jdut1, deltapsi, meaneps, raan, lod, eqeterms, opt);

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
            Console.WriteLine(String.Format("r2eci = {0}  {1}  {2}  {3}  {4}  {5}", r2eci[0], r2eci[1], r2eci[2], v2eci[0], v2eci[1], v2eci[2]));
            Console.WriteLine(String.Format("r2ric = {0}  {1}  {2}  {3}  {4}  {5}", r2ric[0], r2ric[1], r2ric[2], v2ric[0], v2ric[1], v2ric[2]));

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
         *  author        : david vallado                  719-573-2600   21 jun 2002
         *
         *  revisions
         *    vallado     - fix special cases                              5 sep 2002
         *    vallado     - delete extra check in inclination code        16 oct 2002
         *    vallado     - add constant file use                         29 jun 2003
         *    vallado     - add mu                                         2 apr 2007
         *
         *  inputs          description                    range / units
         *    r           - ijk position vector            km or m
         *    v           - ijk velocity vector            km/s or m/s
         *    mu          - gravitational parameter        km3/s2 or m3/s2
         *
         *  outputs       :
         *    p           - semilatus rectum               km
         *    a           - semimajor axis                 km
         *    ecc         - eccentricity
         *    incl        - inclination                    0.0  to Math.PI rad
         *    raan       - right ascension of ascending node    0.0  to 2pi rad
         *    argp        - argument of perigee            0.0  to 2pi rad
         *    nu          - true anomaly                   0.0  to 2pi rad
         *    m           - mean anomaly                   0.0  to 2pi rad
         *    arglat      - argument of latitude      (ci) 0.0  to 2pi rad
         *    truelon     - true longitude            (ce) 0.0  to 2pi rad
         *    lonper      - longitude of periapsis    (ee) 0.0  to 2pi rad
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
         *    e           - eccentric, parabolic,
         *                  hyperbolic anomaly             rad
         *    temp        - temporary variable
         *    typeorbit   - type of orbit                  ee, ei, ce, ci
         *
         *  coupling      :
         *    mag         - magnitude of a vector
         *    MathTimeLibr.cross       - MathTimeLibr.cross product of two vectors
         *    angle       - find the angle between two vectors
         *    newtonnu    - find the mean anomaly
         *
         *  references    :
         *    vallado       2007, 126, alg 9, ex 2-5
         * --------------------------------------------------------------------------- */

        public void rv2coe
             (
               double[] r, double[] v, double mu,
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
                c1 = magv * magv - mu / magr;
                rdotv = MathTimeLibr.dot(r, v);
                for (i = 0; i <= 2; i++)
                    ebar[i] = (c1 * r[i] - rdotv * v[i]) / mu;
                ecc = MathTimeLibr.mag(ebar);

                // ------------  find a e and semi-latus rectum   ----------
                sme = (magv * magv * 0.5) - (mu / magr);
                if (Math.Abs(sme) > small)
                    a = -mu / (2.0 * sme);
                else
                    a = infinite;
                p = magh * magh / mu;

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
        *  author        : david vallado                  719-573-2600    1 mar 2001
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

            vpqw[0] = -sin_nu * Math.Sqrt(MathTimeLib.globals.mu / p);
            vpqw[1] = (ecc + cos_nu) * Math.Sqrt(MathTimeLib.globals.mu / p);
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
        //                           function rv2eq.m
        //
        // this function transforms a position and velocity vector into the flight
        //    elements - latgc, lon, fpa, az, position and velocity magnitude.
        //
        //  author        : david vallado                  719-573-2600    7 jun 2002
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
              double[] r, double[] v, out double a, out double n, out double af, out double ag, out double chi, out double psi, out double meanlonM,
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
            rv2coe(r, v, MathTimeLib.globals.mu, out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out arglat, out truelon, out lonper);

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

            n = Math.Sqrt(MathTimeLib.globals.mu / (a * a * a));

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
        //  author        : david vallado                  719-573-2600    9 jun 2002
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
            double p, ecc, incl, raan, argp, nu, m, e0, arglat, lonper, truelon;
            double small = 0.0000001;
            double undefined = 999999.1;
            double twopi = 2.0 * Math.PI;
            arglat = 999999.1;
            lonper = 999999.1;
            truelon = 999999.1;

            // ---- if n is input ----
            //a = (MathTimeLib.globals.mu/n^2)^(1.0/3.0);

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

            newtonm(ecc, m, out e0, out nu);

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


        /*------------------------------------------------------------------------------
        *
        *                           procedure rv_elatlon
        *
        *  this procedure converts ecliptic latitude and longitude with position and
        *    velocity vectors. uses velocity vector to find the solution of Math.Singular
        *    cases.
        *
        *  author        : david vallado                  719-573-2600   22 jun 2002
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
                rr = MathTimeLibr.mag(re);
                temp = Math.Sqrt(re[0] * re[0] + re[1] * re[1]);
                if (temp < small)
                {
                    temp1 = Math.Sqrt(ve[0] * ve[0] + ve[1] * ve[1]);
                    if (Math.Abs(temp1) > small)
                        ecllon = Math.Atan2(ve[1] / temp1, ve[0] / temp1);
                    else
                        ecllon = 0.0;
                }
                else
                    ecllon = Math.Atan2(re[1] / temp, re[0] / temp);
                ecllat = Math.Asin(re[2] / MathTimeLibr.mag(re));

                temp1 = -re[1] * re[1] - re[0] * re[0]; // different now
                drr = MathTimeLibr.dot(re, ve) / rr;
                if (Math.Abs(temp1) > small)
                    decllon = (ve[0] * re[1] - ve[1] * re[0]) / temp1;
                else
                    decllon = 0.0;
                if (Math.Abs(temp) > small)
                    decllat = (ve[2] - drr * Math.Sin(ecllat)) / temp;
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
    	*  author        : david vallado                  719-573-2600   22 jun 2002
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
        *  author        : david vallado                  719-573-2600   22 jun 2002
        *
        *  inputs          description                            range / units
        *    recef       - ecef position vector                       km
        *    vecef       - ecef velocity vector                       km/s
        *    rsecef      - ecef site position vector                  km
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
        *    lncom2      - combine two vectors and constants
        *    arcsin      - arc sine function
        *    sign        - returns the sign of a variable
        *
        *  references    :
        *    vallado       2013, 265, alg 27
        -----------------------------------------------------------------------------*/

        public void rv_razel
            (
            ref double[] recef, ref double[] vecef, ref double[] rsecef, double latgd, double lon,
            Enum direct,
            ref double rho, ref double az, ref double el, ref double drho, ref double daz, ref double del
            )
        {
            const double halfpi = Math.PI * 0.5;
            const double small = 0.0000001;

            double temp, temp1;
            double[] rhoecef = new double[3];
            double[] drhoecef = new double[3];
            double[] rhosez = new double[3];
            double[] drhosez = new double[3];
            double[] tempvec = new double[3];

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
        *    position and velocity vectors. uses velocity vector to find the
        *    solution of singular cases.
        *
        *  author        : david vallado                  719-573-2600   22 jun 2002
        *
        *  inputs          description                    range / units
        *    reci        - eci position vector            er
        *    veci        - eci velocity vector            er/tu
        *    rseci       - eci site position vector       er
        *    direct      -  direction to convert          eFrom  eTo
        *
        *  outputs       :
        *    rho         - top radius of the sat          er
        *    trtasc      - top right ascension            rad
        *    tdecl       - top declination                rad
        *    drho        - top radius of the sat rate     er/tu
        *    tdrtasc     - top right ascension rate       rad/tu
        *    tddecl      - top declination rate           rad/tu
        *
        *  locals        :
        *    rhov        - eci range vector from site     er
        *    drhov       - eci velocity vector from site  er / tu
        *    temp        - temporary extended value
        *    temp1       - temporary extended value
        *    i           - index
        *
        *  coupling      :
        *    mag         - magnitude of a vector
        *    atan2       - arc tangent function that resolves the quadrant ambiguities
        *    arcsin      - arc sine function
        *    lncom2      - linear combination of 2 vectors
        *    addvec      - add two vectors
        *    dot         - dot product of two vectors
        *
        *  references    :
        *    vallado       2013, 260, alg 26
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
            const double omegaearth = 0.05883359221938136;  // earth rot rad/tu

            double[] earthrate = new double[3];
            double[] rhov = new double[3];
            double[] drhov = new double[3];
            double[] vseci = new double[3];
            double latgc, temp, temp1;

            latgc = Math.Asin(rseci[2] / MathTimeLibr.mag(rseci));
            earthrate[0] = 0.0;
            earthrate[1] = 0.0;
            earthrate[2] = omegaearth;
            MathTimeLibr.cross(earthrate, rseci, out vseci);

            if (direct.Equals(MathTimeLib.Edirection.efrom))
            {
                /* --------  calculate topocentric vectors ------------------ */
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

                /* ------ find eci range vector from site to satellite ------ */
                MathTimeLibr.addvec(1.0, rhov, 1.0, rseci, out reci);
                MathTimeLibr.addvec(1.0, drhov, Math.Cos(latgc), vseci, out veci);
                /*
                if (show == 'y')
                if (fileout != null)
                {
                fprintf(fileout, "rtb %18.7f %18.7f %18.7f %18.7f er\n",
                rhov[1], rhov[2], rhov[3], MathTimeLibr.mag(rhov));
                fprintf(fileout, "vtb %18.7f %18.7f %18.7f %18.7f\n",
                drhov[1], drhov[2], drhov[3], MathTimeLibr.mag(drhov));
                }
                */
            }
            else
            {
                /* ------ find eci range vector from site to satellite ------ */
                MathTimeLibr.addvec(1.0, reci, -1.0, rseci, out rhov);
                MathTimeLibr.addvec(1.0, veci, -Math.Cos(latgc), vseci, out drhov);

                /* -------- calculate topocentric angle and rate values ----- */
                rho = MathTimeLibr.mag(rhov);
                temp = Math.Sqrt(rhov[0] * rhov[0] + rhov[1] * rhov[1]);
                if (temp < small)
                {
                    temp1 = Math.Sqrt(drhov[0] * drhov[0] + drhov[1] * drhov[1]);
                    trtasc = Math.Atan2(drhov[1] / temp1, drhov[0] / temp1);
                }
                else
                    trtasc = Math.Atan2(rhov[1] / temp, rhov[0] / temp);

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
                /*
                if (show == 'y')
                if (fileout != null)
                {
                fprintf(fileout, "rta %18.7f %18.7f %18.7f %18.7f er\n",
                rhov[1], rhov[3], rhov[3], MathTimeLibr.mag(rhov));
                fprintf(fileout, "vta %18.7f %18.7f %18.7f %18.7f er\n",
                drhov[1], drhov[3], drhov[3], MathTimeLibr.mag(drhov));
                }
                */
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
        *  author        : david vallado                  719-573-2600   22 jun 2002
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
        *    sinel       - variable for sin( el )
        *    cosel       - variable for cos( el )
        *    sinaz       - variable for sin( az )
        *    cosaz       - variable for cos( az )
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
            double psia, wa, ea, xa, deltapsi, deltaeps, trueeps, raan, meaneps, magrhosez;
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
            // a = [0;0;0];
            // [recef,vecef,aecef] = eci2ecef(reci,veci,a,ttt,jdut1,lod,xp,yp,terms,ddpsi,ddeps);
            prec = precess(ttt, opt, out psia, out wa, out ea, out xa);
            nut = nutation(ttt, ddpsi, ddeps, EOPSPWLibr.iau80arr, opt, out deltapsi, out deltaeps, out trueeps, out meaneps, out raan);
            st = sidereal(jdut1, deltapsi, meaneps, raan, lod, 2, opt);  // stdot calc, not returned, use terms = 2 since well past 1997
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
        *  author        : david vallado                  719-573-2600    9 jun 2002
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
        *                           function rv2pqw
        *
        *  this function finds the pqw vectors given the geocentric equatorial 
        *  position and velocity vectors.  mu is needed if km and m are
        *    both used with the same routine
        *
        *  author        : david vallado                  719-573-2600   21 jun 2002
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
               double[] r, double[] v, double mu,
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
                c1 = magv * magv - mu / magr;
                rdotv = MathTimeLibr.dot(r, v);
                for (i = 0; i <= 2; i++)
                    ebar[i] = (c1 * r[i] - rdotv * v[i]) / mu;
                ecc = MathTimeLibr.mag(ebar);

                // ------------  find a e and semi-latus rectum   ----------
                p = magh * magh / mu;

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

                vpqw[0] = -sin_nu * Math.Sqrt(MathTimeLib.globals.mu / p);
                vpqw[1] = (ecc + cos_nu) * Math.Sqrt(MathTimeLib.globals.mu / p);
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
          *  this procedure performs the newton rhapson iteration to find the
          *    mean and true anomaly given the eccentreic anomaly.  
          *
          *  author        : david vallado                  719-573-2600   14 feb 2020
          *
          *  inputs          description                    range / units
          *    ecc         - eccentricity                   0.0 to
          *    e0          - eccentric anomaly              0.0 to 2pi rad
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
          *    vallado       2007, 73, alg 2, ex 2-1
          * ----------------------------------------------------------------- */

        public void newtone
           (
           double ecc, double e0, out double m, out double nu
           )
        {
            double small, sinv, cosv;

            // -------------------------  implementation   -----------------
            small = 0.00000001;

            // ------------------------- circular --------------------------
            if (Math.Abs(ecc) < small)
            {
                m = e0;
                nu = e0;
            }
            else
            {
                // ----------------------- elliptical ----------------------
                if (ecc < 0.999)
                {
                    m = e0 - ecc * Math.Sin(e0);
                    sinv = (Math.Sqrt(1.0 - ecc * ecc) * Math.Sin(e0)) / (1.0 - ecc * Math.Cos(e0));
                    cosv = (Math.Cos(e0) - ecc) / (1.0 - ecc * Math.Cos(e0));
                    nu = Math.Atan2(sinv, cosv);
                }
                else
                {

                    // ---------------------- hyperbolic  ------------------
                    if (ecc > 1.0001)
                    {
                        m = ecc * Math.Sinh(e0) - e0;
                        sinv = (Math.Sqrt(ecc * ecc - 1.0) * Math.Sinh(e0)) / (1.0 - ecc * Math.Cosh(e0));
                        cosv = (Math.Cosh(e0) - ecc) / (1.0 - ecc * Math.Cosh(e0));
                        nu = Math.Atan2(sinv, cosv);
                    }
                    else
                    {

                        // -------------------- parabolic ------------------
                        m = e0 + (1.0 / 3.0) * e0 * e0 * e0;
                        nu = 2.0 * Math.Atan(e0);
                    }
                }
            }
        }  // newtone


        /* ----------------------------------------------------------------------------
         *
         *                           procedure newtonmx
         *
         *  this procedure performs the newton rhapson iteration to find the
         *    eccentric anomaly given the mean anomaly.  the true anomaly is also
         *    calculated. This is an optimized version from Oltrogge JAS 2015. 
         *
         *  author        : david vallado                  719-573-2600    1 mar 2001
         *
         *  inputs          description                    range / units
         *    ecc         - eccentricity                   0.0 to
         *    m           - mean anomaly                   -2pi to 2pi rad
         *
         *  outputs       :
         *    e0          - eccentric anomaly              0.0 to 2pi rad
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
         *    vallado       2007, 73, alg 2, ex 2-1
         *    oltrogge      JAS 2015
         * ----------------------------------------------------------------- */

        public void newtonmx
            (
            double ecc, double m, out double e0, out double nu
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
                        e0 = m - ecc;
                    else
                        e0 = m + ecc;
                }
                else
                {
                    if ((ecc < 3.6) && (Math.Abs(m) > Math.PI))
                        e0 = m - Math.Sign(m) * ecc;
                    else
                        e0 = m / (ecc - 1.0);
                }

                ktr = 1;
                e1 = e0 + ((m - ecc * Math.Sinh(e0) + e0) / (ecc * Math.Cosh(e0) - 1.0));
                while ((Math.Abs(e1 - e0) > small) && (ktr <= numiter))
                {
                    e0 = e1;
                    e1 = e0 + ((m - ecc * Math.Sinh(e0) + e0) / (ecc * Math.Cosh(e0) - 1.0));
                    ktr = ktr + 1;
                }

                e0 = e1;

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
                    //                e0= r1r 
                    s = 0.5 * (halfpi - Math.Atan(1.5 * m));
                    w = Math.Atan(Math.Pow(Math.Tan(s), 1.0 / 3.0));
                    e0 = 2.0 * Math.Tan(halfpi - 2.0 * w);  // really cot()
                    ktr = 1;
                    nu = 2.0 * Math.Atan(e0);
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
                        e0 = Math.Atan2(sinE, cosE);

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
                        e0 = m;
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
        *  author        : david vallado                  719-573-2600    1 mar 2001
        *
        *  inputs          description                    range / units
        *    ecc         - eccentricity                   0.0 to
        *    m           - mean anomaly                   -2pi to 2pi rad
        *
        *  outputs       :
        *    e0          - eccentric anomaly              0.0 to 2pi rad
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
        *    vallado       2007, 73, alg 2, ex 2-1
        * ----------------------------------------------------------------- */

        public void newtonm
            (
            double ecc, double m, out double e0, out double nu
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
                        e0 = m - ecc;
                    else
                        e0 = m + ecc;
                }
                else
                {
                    if ((ecc < 3.6) && (Math.Abs(m) > Math.PI))
                        e0 = m - Math.Sign(m) * ecc;
                    else
                        e0 = m / (ecc - 1.0);
                }

                ktr = 1;
                e1 = e0 + ((m - ecc * Math.Sinh(e0) + e0) / (ecc * Math.Cosh(e0) - 1.0));
                while ((Math.Abs(e1 - e0) > small) && (ktr <= numiter))
                {
                    e0 = e1;
                    e1 = e0 + ((m - ecc * Math.Sinh(e0) + e0) / (ecc * Math.Cosh(e0) - 1.0));
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
                    //                e0= r1r 
                    s = 0.5 * (halfpi - Math.Atan(1.5 * m));
                    w = Math.Atan(Math.Pow(Math.Tan(s), 1.0 / 3.0));
                    e0 = 2.0 * Math.Tan(halfpi - 2.0 * w);  // really cot()
                    ktr = 1;
                    nu = 2.0 * Math.Atan(e0);
                }
                else
                {
                    // -------------------- elliptical ----------------------
                    if (ecc > small)
                    {
                        // -----------  initial guess -------------
                        if (((m < 0.0) && (m > -Math.PI)) || (m > Math.PI))
                            e0 = m - ecc;
                        else
                            e0 = m + ecc;
                        ktr = 1;
                        e1 = e0 + (m - e0 + ecc * Math.Sin(e0)) / (1.0 - ecc * Math.Cos(e0));

                        while ((Math.Abs(e1 - e0) > small) && (ktr <= numiter))
                        {
                            ktr = ktr + 1;
                            e0 = e1;
                            e1 = e0 + (m - e0 + ecc * Math.Sin(e0)) / (1.0 - ecc * Math.Cos(e0));
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
                        e0 = m;
                    } // if ecc > small
                }  // if abs()
            }
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
        *  author        : david vallado                  719-573-2600   27 may 2002
        *
        *  revisions
        *    vallado     - fix small                                     24 sep 2002
        *
        *  inputs          description                    range / units
        *    ecc         - eccentricity                   0.0  to
        *    nu          - true anomaly                   -2pi to 2pi rad
        *
        *  outputs       :
        *    e0          - eccentric anomaly              0.0  to 2pi rad       153.02 ø
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
            double ecc, double nu, out double e0, out double m
            )
        {
            double small, sine, cose;

            // ---------------------  implementation   ---------------------
            e0 = 999999.9;
            m = 999999.9;
            small = 0.00000001;

            // --------------------------- circular ------------------------
            if (Math.Abs(ecc) < small)
            {
                m = nu;
                e0 = nu;
            }
            else
            {
                // ---------------------- elliptical -----------------------
                if (ecc < 1.0 - small)
                {
                    sine = (Math.Sqrt(1.0 - ecc * ecc) * Math.Sin(nu)) / (1.0 + ecc * Math.Cos(nu));
                    cose = (ecc + Math.Cos(nu)) / (1.0 + ecc * Math.Cos(nu));
                    e0 = Math.Atan2(sine, cose);
                    m = e0 - ecc * Math.Sin(e0);
                }
                else
                {
                    // -------------------- hyperbolic  --------------------
                    if (ecc > 1.0 + small)
                    {
                        if ((ecc > 1.0) && (Math.Abs(nu) + 0.00001 < Math.PI - Math.Acos(1.0 / ecc)))
                        {
                            sine = (Math.Sqrt(ecc * ecc - 1.0) * Math.Sin(nu)) / (1.0 + ecc * Math.Cos(nu));
                            e0 = MathTimeLibr.asinh(sine);
                            m = ecc * Math.Sinh(e0) - e0;
                        }
                    }
                    else
                    {
                        // ----------------- parabolic ---------------------
                        if (Math.Abs(nu) < 168.0 * Math.PI / 180.0)
                        {
                            e0 = Math.Tan(nu * 0.5);
                            m = e0 + (e0 * e0 * e0) / 3.0;
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
                e0 = e0 - Math.Floor(e0 / (2.0 * Math.PI)) * (2.0 * Math.PI);
            }
        } // newtonnu


        /* -----------------------------------------------------------------------------
        *
        *                           function lon2nu
        *
        *  this function finds the true anomaly from coes and longitude.
        *
        *  author        : david vallado                  719-573-2600    1 mar 2011
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

            // Console.WriteLine(String.Format("gmst  lu  nu  arglat = {0}  {1}  {2}  {3}  ", gmst * rad, lambdau * rad, temp * rad, arglat * rad));

        }  // lon2nu  


 

        /* -----------------------------------------------------------------------------
        *
        *                           function findc2c3
        *
        *  this function calculates the c2 and c3 functions for use in the universal
        *    variable calculation of z.
        *
        *  author        : david vallado                  719-573-2600   27 may 2002
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
        *  note that not all the input parameters are needed for each case.also, the step
        *  size dtsec should be small, perhaps on the order of 60-120 secs!
        *
        *  author        : david vallado                  719-573-2600   27 jan 2020
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
                    rv2pqw(r1, v1, MathTimeLib.globals.mu, out rpqw1, out vpqw1);
                    // find vectors in PQW frame
                    rv2pqw(r2, v2, MathTimeLib.globals.mu, out rpqw2, out vpqw2);

                    // normal units
                    f = (rpqw2[0] * vpqw1[1] - vpqw2[0] * rpqw1[1]) / h;
                    g = (rpqw1[0] * rpqw2[1] - rpqw2[0] * rpqw1[1]) / h;
                    gdot = (rpqw1[0] * vpqw2[1] - vpqw2[0] * rpqw1[1]) / h;
                    fdot = (vpqw2[0] * vpqw1[1] - vpqw2[1] * vpqw1[0]) / h;
                    break;

                case "series":
                    double u, p, q;
                    u = MathTimeLib.globals.mu / (magr1 * magr1 * magr1);
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
                    f = 1.0 - 0.5 * u * dt2 + 0.5 * u * p * dt3 + 
                        u / 24.0 * (-15 * p2 + 3.0 * q  + u) * dt4 +
                        p * u / 8.0 * (7.0 * p2 - 3 * q - u) * dt5 +
                        u / 720.0 * (-945.0 * p4 + 630.0 * p2 * q + 210 * u * p2 - 45 * q2 - 24 * u * q - u2) * dt6 +
                        p*u / 80.0 * (165 * p4 - 150 * p2 * q - 50 * u * p2 + 25 * q2 + 14.0 * u * q + u2) * dt7 +
                        u / 40320.0 * (-135135.0 * p6 + 155925 * p4 * q + 51975 * u * p4 - 42525 * p2 * q2 -
                            24570 * u * p2 * q - 2205 * u2 * p2 + 1575 * q3 + 1107.0 * u * q2 + 117.0 * u2 * q  + u3) * dt8;

                    g = dtsec - 1.0 / 6.0 * u * dt3 + 0.25 * u * p * dt4 + 
                        u / 120.0 * (-45 * p2 + 9.0 * q  + u) * dt5 +
                        p*u / 24.0 * (14.0 * p2 - 6 * q - u) * dt6 +
                        u / 5040.0 * (-4725 * p4 + 3150 * p2 * q + 630 * u * p2 - 225 * q2 - 54 * u * q   - u2) * dt7 +
                        p*u / 320.0 * (495 * p4 - 450 * p2 * q - 100 * u * p2 + 75 * q2 + 24.0 * u * q   + u2) * dt8;

                    fdot = -u * dtsec + 1.5 * u * p * dt2 + 
                        u / 6.0 * (-15 * p2 + 3 * q  + u) * dt3 +
                        5.0*p*u / 8.0 * (7.0 * p2 - 3 * q - u) * dt4 +
                        u / 120.0 * (-945 * p4 + 630 * p2 * q + 210 * u * p2 - 45 * q2 - 24 * u * q - u2) * dt5 +
                        7.0*p*u / 80.0 * (165 * p4 - 150 * p2 * q - 50 * u * p2 + 25 * q2 + 14 * u * q  + u2) * dt6 +
                        u / 5040.0 * (-135135 * p6 + 155925 * p4 * q + 51975 * u * p4 - 42525 * p2 * q2 - 24570 * u * p2 * q
                         - 2205 * u2 * p2 + 1575 * q3 + 1107 * u * q2 + 117 * u2 * q  + u3) * dt7;

                    gdot = 1.0 - 0.5 * u * dt2 + u * p * dt3 + 
                        u / 24 * (-45 * p2  + 9 * q + u) * dt4 +
                        p*u / 4 * (14 * p2 - 6 * q - u) * dt5 +
                        u / 720 * (-4725 * p4 + 3150 * p2 * q + 630 * u * p2 - 225 * q2 - 54 * u * q  - u2) * dt6 +
                        p*u / 40 * (495 * p4 - 450 * p2 * q - 100 * u * p2 + 75 * q2 + 24 * u * q   + u2) * dt7;

                    //f = dt8* u* (-135135 * p6 + 155925 * p4* q + 51975 * p4* u - 42525 * p2 * q2 - 24570 * p2 * q * u - 2205 * p2 * u2 + 1575 * q3 + 1107 * q2 * u + 117 * q * u2 + u3) / 40320 + dt7 * p * u * (165 * p4 - 150 * p2 * q - 50 * p2 * u + 25 * q2 + 14 * q * u + u2) / 80 + dt6* u * (-945 * p4 + 630 * p2 * q + 210 * p2 * u - 45 * q2 - 24 * q * u - u2) / 720 + dt5 * p * u * (7 * p2 - 3 * q - u) / 8 + dt4 * u * (-15 * p2 + 3 * q + u) / 24 + dt3 * p * u / 2 - dt2 * u / 2 + 1;
                    //g = dt8 * p * u * (495 * p4 - 450 * p2 * q - 100 * p2 * u + 75 * q2 + 24 * q * u + u2) / 320 + dt7 * u * (-4725 * p4 + 3150 * p2 * q + 630 * p2 * u - 225 * q2 - 54 * q * u - u2) / 5040 + dt6 * p * u * (14 * p2 - 6 * q - u) / 24 + dt5 * u * (-45 * p2 + 9 * q + u) / 120 + dt4 * p * u / 4 - dt3 * u / 6 + dtsec;
                    //fdot = dt7 * u * (-135135 * p6 + 155925 * p4 * q + 51975 * p4 * u - 42525 * p2 * q2 - 24570 * p2 * q * u - 2205 * p2 * u2 + 1575 * q3 + 1107 * q2 * u + 117 * q * u2 + u3) / 5040 + 7 * dt6 * p * u * (165 * p4 - 150 * p2 * q - 50 * p2 * u + 25 * q2 + 14 * q * u + u2) / 80 + dt5 * u * (-945 * p4 + 630 * p2 * q + 210 * p2 * u - 45 * q2 - 24 * q * u - u2) / 120 + 5 * dt4 * p * u * (7 * p2 - 3 * q - u) / 8 + dt3 * u * (-15 * p2 + 3 * q + u) / 6 + 3 * dt2 * p * u / 2 - dtsec * u;
                    //gdot = dt7 * p * u * (495 * p4 - 450 * p2 * q - 100 * p2 * u + 75 * q2 + 24 * q * u + u2) / 40 + dt6 * u * (-4725 * p4 + 3150 * p2 * q + 630 * p2 * u - 225 * q2 - 54 * q * u - u2) / 720 + dt5 * p * u * (14 * p2 - 6 * q - u) / 4 + dt4 * u * (-45 * p2 + 9 * q + u) / 24 + dt3 * p * u - dt2 * u / 2 + 1;

                    break;
                case "c2c3":
                    double xsqrd = x * x;
                    double magr2 = MathTimeLibr.mag(r2);
                    f = 1.0 - (xsqrd * c2 / magr1);
                    g = dtsec - xsqrd * x * c3 / Math.Sqrt(MathTimeLib.globals.mu);
                    gdot = 1.0 - (xsqrd * c2 / magr2);
                    fdot = (Math.Sqrt(MathTimeLib.globals.mu) * x / (magr1 * magr2)) * (z * c3 - 1.0);
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



        /* ------------------------------------------------------------------------------
        *
        *                           function kepler
        *
        *  this function solves keplers problem for orbit determination and returns a
        *    future geocentric equatorial (ijk) position and velocity vector.  the
        *    solution uses universal variables.
        *
        *  author        : david vallado                  719-573-2600   22 jun 2002
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
            double[] r1, double[] vo, double dtseco, out double[] r2, out double[] v
            )
        {
            // -------------------------  implementation   -----------------
            int ktr, i, numiter, mulrev;
            double[] h = new double[3];
            double[] rx = new double[3];
            double[] vx = new double[3];
            double f, g, fdot, gdot, rval, xold, xoldsqrd, xnew,
                  xnewsqrd, znew, p, c2new, c3new, dtnew, rdotv, a, dtsec,
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
            //errork = "      ok";
            dtsec = dtseco;
            mulrev = 0;

            if (Math.Abs(dtseco) > small)
            {
                magro = MathTimeLibr.mag(r1);
                magvo = MathTimeLibr.mag(vo);
                rdotv = MathTimeLibr.dot(r1, vo);

                // -------------  find sme, alpha, and a  ------------------
                sme = ((magvo * magvo) * 0.5) - (MathTimeLib.globals.mu / magro);
                alpha = -sme * 2.0 / MathTimeLib.globals.mu;

                if (Math.Abs(sme) > small)
                    a = -MathTimeLib.globals.mu / (2.0 * sme);
                else
                    a = 999999.9;
                if (Math.Abs(alpha) < small)   // parabola
                    alpha = 0.0;

                if (show == 'y')
                {
                    //           printf(" sme %16.8f  a %16.8f alp  %16.8f ER \n",sme/(MathTimeLib.globals.mu/re), a/re, alpha * re );
                    //           printf(" sme %16.8f  a %16.8f alp  %16.8f km \n",sme, a, alpha );
                    //           printf(" ktr      xn        psi           r2          xn+1        dtn \n" );
                }

                // ------------   setup initial guess for x  ---------------
                // -----------------  circle and ellipse -------------------
                if (alpha >= small)
                {
                    period = twopi * Math.Sqrt(Math.Abs(a * a * a) / MathTimeLib.globals.mu);
                    // ------- next if needed for 2body multi-rev ----------
                    if (Math.Abs(dtseco) > Math.Abs(period))
                        // including the truncation will produce vertical lines that are parallel
                        // (plotting chi vs time)
                        //                    dtsec = rem( dtseco,period );
                        mulrev = Convert.ToInt16(dtseco / period);
                    if (Math.Abs(alpha - 1.0) > small)
                        xold = Math.Sqrt(MathTimeLib.globals.mu) * dtsec * alpha;
                    else
                        // - first guess can't be too close. ie a circle, r2=a
                        xold = Math.Sqrt(MathTimeLib.globals.mu) * dtsec * alpha * 0.97;
                }
                else
                {
                    // --------------------  parabola  ---------------------
                    if (Math.Abs(alpha) < small)
                    {
                        MathTimeLibr.cross(r1, vo, out h);
                        magh = MathTimeLibr.mag(h);
                        p = magh * magh / MathTimeLib.globals.mu;
                        s = 0.5 * (halfpi - Math.Atan(3.0 * Math.Sqrt(MathTimeLib.globals.mu / (p * p * p)) * dtsec));
                        w = Math.Atan(Math.Pow(Math.Tan(s), (1.0 / 3.0)));
                        xold = Math.Sqrt(p) * (2.0 * MathTimeLibr.cot(2.0 * w));
                        alpha = 0.0;
                    }
                    else
                    {
                        // ------------------  hyperbola  ------------------
                        temp = -2.0 * MathTimeLib.globals.mu * dtsec /
                              (a * (rdotv + Math.Sign(dtsec) * Math.Sqrt(-MathTimeLib.globals.mu * a) *
                              (1.0 - magro * alpha)));
                        xold = Math.Sign(dtsec) * Math.Sqrt(-a) * Math.Log(temp);
                    }
                } // if alpha

                ktr = 1;
                dtnew = -10.0;
                double tmp = 1.0 / Math.Sqrt(MathTimeLib.globals.mu);
                while ((Math.Abs(dtnew * tmp - dtsec) >= small) && (ktr < numiter))
                {
                    xoldsqrd = xold * xold;
                    znew = xoldsqrd * alpha;

                    // ------------- find c2 and c3 functions --------------
                    findc2c3(znew, out c2new, out c3new);

                    // ------- use a newton iteration for new values -------
                    rval = xoldsqrd * c2new + rdotv * tmp * xold * (1.0 - znew * c3new) +
                             magro * (1.0 - znew * c2new);
                    dtnew = xoldsqrd * xold * c3new + rdotv * tmp * xoldsqrd * c2new +
                             magro * xold * (1.0 - znew * c3new);

                    // ------------- calculate new value for x -------------
                    xnew = xold + (dtsec * Math.Sqrt(MathTimeLib.globals.mu) - dtnew) / rval;

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
                    g = dtsec - xnewsqrd * xnew * c3new / Math.Sqrt(MathTimeLib.globals.mu);

                    for (i = 0; i < 3; i++)
                        r2[i] = f * r1[i] + g * vo[i];
                    magr = MathTimeLibr.mag(r2);
                    gdot = 1.0 - (xnewsqrd * c2new / magr);
                    fdot = (Math.Sqrt(MathTimeLib.globals.mu) * xnew / (magro * magr)) * (znew * c3new - 1.0);
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
            } // if fabs
            else
                // ----------- set vectors to incoming since 0 time --------
                for (i = 0; i < 3; i++)
                {
                    r2[i] = r1[i];
                    v[i] = vo[i];
                }

            //       fprintf( fid,"%11.5f  %11.5f %11.5f  %5i %3i ",znew, dtseco/60.0, xold/(rad), ktr, mulrev );
        }  // kepler



        /* ----------------------------------------------------------------------------
  *
  *
  *                           function ijk2ll
  *
  *  these subroutines convert a geocentric equatorial position vector into
  *    latitude and longitude.  geodetic and geocentric latitude are found. the
  *    inputs must be ecef.
  *
  *  author        : david vallado                  719-573-2600   27 may 2002
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

        public void ijk2ll
            (
            double[] r, out double latgc, out double latgd, out double lon, out double hellp
            )
        {
            double twopi = 2.0 * Math.PI;
            double small = 0.00000001;         // small value for tolerances
            double eesqrd = 0.006694385000;     // eccentricity of earth sqrd

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
                c = MathTimeLib.globals.re / (Math.Sqrt(1.0 - eesqrd * sintemp * sintemp));
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

            latgc = gd2gc(latgd);
        } //  ijk2ll


        /*---------------------------------------------------------------------------
        *
        *                           function gd2gc
        *
        *  this function converts from geodetic to geocentric latitude for positions
        *    on the surface of the earth.  notice that (1-f) squared = 1-esqrd.
        *
        *  author        : david vallado                  719-573-2600   30 may 2002
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
            double eesqrd = 0.006694385000;     // eccentricity of earth sqrd

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
        //  author        : david vallado                  719-573-2600   14 aug 2017
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
            double radiuspad = MathTimeLib.globals.re + altPad; // radius of Earth with pad, km
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
                ainv = 2.0 / magr1 - v12 / MathTimeLib.globals.mu;

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
                        esinea1 = rdotv1 / Math.Sqrt(MathTimeLib.globals.mu * a);
                        ecc = Math.Sqrt(ecosea1 * ecosea1 + esinea1 * esinea1);
                    }
                    // hyperbolic orbit
                    else
                    {
                        esinea1 = rdotv1 / Math.Sqrt(MathTimeLib.globals.mu * Math.Abs(-a));
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
                    double c1 = magv1 * magv1 - MathTimeLib.globals.mu / magr1;
                    double rdotv = MathTimeLibr.dot(r1, v1t);
                    for (int i = 0; i <= 2; i++)
                        ebar[i] = (c1 * r1[i] - rdotv * v1t[i]) / MathTimeLib.globals.mu;
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
                            rp = magh * magh * 0.5 / MathTimeLib.globals.mu;
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
                                esinea1 = rdotv1 / Math.Sqrt(MathTimeLib.globals.mu * a);
                                ecc = Math.Sqrt(ecosea1 * ecosea1 + esinea1 * esinea1);
                            }
                            // hyperbolic orbit
                            else
                            {
                                esinea1 = rdotv1 / Math.Sqrt(MathTimeLib.globals.mu * Math.Abs(-a));
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
        //  author        : david vallado                  719-573-2600   14 aug 2017
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
        //  author        : david vallado                  719-573-2600    8 jun 2016
        //
        //  inputs          description                          range / units
        //    r1          - ijk position vector 1                 km
        //    r2          - ijk position vector 2                 km
        //    nrev        - multiple revolutions                  0, 1,  
        //    de          - orbital energy                       'L', 'H'
        //                  this is the inclination discriminator
        //
        //  outputs       :
        //    kbi         - k values for min tof for each nrev
        //    tof         - min time of flight for each nrev   tu
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

            oomu = 1.0 / Math.Sqrt(MathTimeLib.globals.mu);  // for speed
            sqrtmu = Math.Sqrt(MathTimeLib.globals.mu);

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
                    c2dot = -2.0 / MathTimeLibr.factorial(4) + 2.0 * psiold / MathTimeLibr.factorial(6) - 3.0 * psiold2 / MathTimeLibr.factorial(8) +
                        4.0 * psiold3 / MathTimeLibr.factorial(10) - 5.0 * psiold4 / MathTimeLibr.factorial(12);
                    c3dot = -1.0 / MathTimeLibr.factorial(5) + 2.0 * psiold / MathTimeLibr.factorial(7) - 3.0 * psiold2 / MathTimeLibr.factorial(9) +
                        4.0 * psiold3 / MathTimeLibr.factorial(11) - 5.0 * psiold4 / MathTimeLibr.factorial(13);
                    c2ddot = 0.0;
                    c3ddot = 0.0;
                }
                // now solve this for dt = 0.0
                //            dtdpsi = x^3*(c3dot - 3.0*c3*c2dot/(2.0*c2))* oomu + 0.125*vara/sqrt(MathTimeLib.globals.mu) * (3.0*c3*sqrty/c2 + vara/x);
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
        *  author        : david vallado                  719-573-2600   22 mar 2018
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
                tminp = (1.0 / 3.0) * Math.Sqrt(2.0 / MathTimeLib.globals.mu) * (Math.Pow(s, 1.5) - Math.Pow(s - chord, 1.5));
            else
                tminp = (1.0 / 3.0) * Math.Sqrt(2.0 / MathTimeLib.globals.mu) * (Math.Pow(s, 1.5) + Math.Pow(s - chord, 1.5));

            // ------------- this is the min energy ellipse tof
            double amin = 0.5 * s;
            alpha = Math.PI;
            beta = 2.0 * Math.Asin(Math.Sqrt((s - chord) / s));
            if (dm == 'S')
                tminenergy = Math.Pow(amin, 1.5) * ((2.0 * nrev + 1.0) * Math.PI - beta + Math.Sin(beta)) / Math.Sqrt(MathTimeLib.globals.mu);
            else
                tminenergy = Math.Pow(amin, 1.5) * ((2.0 * nrev + 1.0) * Math.PI + beta - Math.Sin(beta)) / Math.Sqrt(MathTimeLib.globals.mu);

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
                tmin = Math.Pow(an, 1.5) * (2.0 * Math.PI * nrev + xi - eta) / Math.Sqrt(MathTimeLib.globals.mu);
            else
                tmin = Math.Pow(an, 1.5) * (2.0 * Math.PI * nrev + xi + eta) / Math.Sqrt(MathTimeLib.globals.mu);
        }  // lambertminT


        /*------------------------------------------------------------------------------
*
*                           procedure lambertTmaxrp
*
*  this procedure solves lambert's problem and finds the TOF for maximum rp
*
*  author        : david vallado                  719-573-2600   26 aug 2019
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

            y1 = MathTimeLib.globals.mu / magr1;
            y2 = MathTimeLib.globals.mu / magr2;

            // ------------- nearly circular endpoints
            MathTimeLibr.addvec(1.0, r1, -1.0, r2, out tempvec);
            // circular orbit  to within 1 meter
            if (MathTimeLibr.mag(tempvec) < 0.001)  
            {
                c = Math.Sqrt(y1);
                x1 = x2 = r = 0.0;
                tmaxrp = (2.0 * nrev * Math.PI + Math.Atan2(sindeltanu, cosdeltanu)) * (MathTimeLib.globals.mu / Math.Pow(c, 3));
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

                tmaxrp = MathTimeLib.globals.mu * (
                    (2.0 * nrev * Math.PI + k) / Math.Pow(Math.Abs(c * c - r * r), 1.5) +
                    (c * sindeltanu) / (y1 * y2) );
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
        *  author        : david vallado                        719-573-2600   22 jun 2002
        *
        *  inputs          description                          range / units
        *    r1          - ijk position vector 1                km
        *    r2          - ijk position vector 2                km
        *    v1          - ijk velocity vector 1 if avail       km/s
        *    dm          - direction of motion                  'L', 'S'
        *    de          - orbital energy                       'L', 'H'
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
        *    hitearth    - flag if hti or not                   'y', 'n'
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

            double oosqrtmu = 1.0 / Math.Sqrt(MathTimeLib.globals.mu);

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
                                errorout = errorout + "\r\nyneg" + loops.ToString().PadLeft(3) + "   " + psiold.ToString("0.0000000").PadLeft(12) + "  " +
                                    y.ToString("0.0000000").PadLeft(15) + " " + xold.ToString("0.00000").PadLeft(13) + " " +
                                    dtnew.ToString("0.#######").PadLeft(15) + " " + vara.ToString("0.#######").PadLeft(15) + " " +
                                    upper.ToString("0.#######").PadLeft(15) + " " + lower.ToString("0.#######").PadLeft(15) + " " + ynegktr.ToString();

                            ynegktr++;
                        }
                    }

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
                        estr = ynegktr.ToString() + " newt ";

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
                        loops = loops + 1;

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
                    g = 1.0 / (vara * Math.Sqrt(y / MathTimeLib.globals.mu)); // 1 over g
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
                errorstr = "impossible180";

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
        *  author        : david vallado                  719-573-2600  22 may 2017
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

            a = MathTimeLib.globals.mu * (1.0 * oomagr1 - 1.0 * oop);  // not the semi - major axis
            b = Math.Pow(MathTimeLib.globals.mu * ecc * oop, 2) - a * a;
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
                    ptx = (2.0 * Math.PI) * Math.Sqrt(p * p * p / (MathTimeLib.globals.mu * Math.Pow(1.0 - ecc * ecc, 3)));
                    if (dtsec % ptx > ptx * 0.5)  // mod
                        x1 = -x1;
                }
            }
            else
            {
                // this appears to be the more common option
                y2a = MathTimeLib.globals.mu * oop - x1 * Math.Sin(dnu) + a * Math.Cos(dnu);
                y2b = MathTimeLib.globals.mu * oop + x1 * Math.Sin(dnu) + a * Math.Cos(dnu);
                if (Math.Abs(MathTimeLib.globals.mu * oomagr2 - y2b) < Math.Abs(MathTimeLib.globals.mu * oomagr2 - y2a))
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
                v1t[i] = (Math.Sqrt(MathTimeLib.globals.mu * p) * oomagr1) * ((x1 / MathTimeLib.globals.mu) * r1[i] + rcrv[i] * oomagr1);
                v2t[i] = (Math.Sqrt(MathTimeLib.globals.mu * p) * oomagr2) * ((x2 / MathTimeLib.globals.mu) * r2[i] + rcrr[i] * oomagr2);
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
        *  author        : david vallado                  719-573-2600   22 jun 2002
        *
        *  inputs          description                            range / units
        *    r1          - ijk position vector 1                      km
        *    r2          - ijk position vector 2                      km
        *    v1          - ijk velocity vector 1 if avail             km/s
        *    dm          - direction of motion                       'L', 'S'
        *    de          - orbital energy                            'L', 'H'
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
               double[] r1, double[] r2, double[] v1, char dm, char de, Int32 nrev, double dtsec, double dtwait,
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
            v1t = new double[] { 1002.0, 1002.0, 1002.0 };
            v2t = new double[] { 1002.0, 1002.0, 1002.0 };

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
            m = 8.0 * MathTimeLib.globals.mu * dtsec * dtsec / (s * s * s * Math.Pow(1.0 + lam, 6));

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
                            " " + dnu.ToString("0.0000000").PadLeft(15) +  " " + hitearth;
                }  // if loops converged < 30
            }  // if nrev and r

        }  //  lambertbattin




  
        /*---------------------------------------------------------------------------
        *
        *                           procedure site
        *
        *  this function finds the position and velocity vectors for a site.  the
        *    answer is returned in the geocentric equatorial (ecef) coordinate system.
        *    note that the velocity is zero because the coordinate system is fixed to
        *    the earth.
        *
        *  author        : david vallado                  719-573-2600   25 jun 2002
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
            cearth = MathTimeLib.globals.re / Math.Sqrt(1.0 - (eesqrd * sinlat * sinlat));
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



        /* -----------------------------------------------------------------------------
        *
        *                           function initjplde
        *
        *  this function initializes the jpl planetary ephemeris data. the input
        *  data files are from processing the ascii files into a text file of sun
        *  and moon positions.
        *
        *  author        : david vallado                  719-573-2600   22 jan 2018
        *
        *  revisions
        *
        *  inputs          description                    range / units
        *
        *
        *
        *
        *  outputs       :
        *    jpldearr    - array of jplde data records
        *    jdjpldestart- julian date of the start of the jpldearr data
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
            out double jdjpldestart, out double jdjpldestartFrac
            )
        {
            double jdtdb, jdtdbf;
            string pattern;
            Int32 i;
            jdjpldestart = 0.0;
            jdjpldestartFrac = 0.0;

            // read the whole file at once into lines of an array
            string[] fileData = File.ReadAllLines(jplLoc + infilename);

            pattern = @"^\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)";
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
                jpldearr[i].rsun[0] = Convert.ToDouble(linedata[4]);
                jpldearr[i].rsun[1] = Convert.ToDouble(linedata[5]);
                jpldearr[i].rsun[2] = Convert.ToDouble(linedata[6]);
                jpldearr[i].rsmag = Convert.ToDouble(linedata[7]);
                jpldearr[i].rmoon[0] = Convert.ToDouble(linedata[9]);
                jpldearr[i].rmoon[1] = Convert.ToDouble(linedata[10]);
                jpldearr[i].rmoon[2] = Convert.ToDouble(linedata[11]);

                MathTimeLibr.jday(jpldearr[i].year, jpldearr[i].mon, jpldearr[i].day, 0, 0, 0.0, out jdtdb, out jdtdbf);
                jpldearr[i].mjd = jdtdb + jdtdbf - 2400000.5;

                // ---- find epoch date
                if (i == 0)
                {
                    MathTimeLibr.jday(jpldearr[i].year, jpldearr[i].mon, jpldearr[i].day, 0, 0, 0.0, out jdjpldestart, out jdjpldestartFrac);
                    jpldearr[i].mjd = jdjpldestart + jdjpldestartFrac;
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
        *  author        : david vallado                      719-573-2600   12 dec 2005
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
            recnum = Convert.ToInt32(jdjpldestarto);

            // check for out of bound values
            if ((recnum >= 1) && (recnum <= jpldesize))
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
        *  author        : david vallado                  719-573-2600   27 may 2002
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
        *    the sun given the julian date.  this is the low precision formula and
        *    is valid for years from 1950 to 2050.  accuaracy of apparent coordinates
        *    is 0.01  degrees.  notice many of the calculations are performed in
        *    degrees, and are not changed until later.  this is due to the fact that
        *    the almanac uses degrees exclusively in their formulations.
        *
        *  author        : david vallado                  719-573-2600   27 may 2002
        *
        *  revisions
        *    vallado     - fix mean lon of sun                            7 mat 2004
        *
        *  inputs          description                    range / units
        *    jd          - julian date                    days from 4713 bc
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
        *  author        : david vallado                  719-573-2600   27 may 2002
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
        *      the factorial can only return results to n = 170 due to precision limits. 
        *      
        *  author        : david vallado                    719-573-2600   9 oct 2019
        *
        *  inputs        description                                     range / units
        *    fname       - filename for gravity field            
        *    order       - Size of gravity field                           1..2160..
        *    normal      - normalized in file                              'y', 'n'
        *
        *  outputs       :
        *    gravData.c  - gravitational coefficients (in gravityModelData)
        *    gravData.s  - gravitational coefficients (in gravityModelData)
        *
        *  locals :
        *    L, m        - degree and order indices
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
                char normal,
                out gravityModelData gravData
            )
        {
            int L, m, ktr;
            double conv;
            string line, line1;

            gravData = new gravityModelData();

            string[] fileData = File.ReadAllLines(fname);

            ktr = 0;
            ktr = 17;  // GEM 10b
            //ktr = 1;   // EGM96a
            //ktr = 23;  // egm08
            while (ktr < fileData.Count() && !fileData[ktr].Contains("END"))
            {
                line = fileData[ktr];
                // remove additional spaces
                line1 = Regex.Replace(line, @"\s+", " ");
                string[] linesplt = line1.Split(' ');

                L = Convert.ToInt32(linesplt[1]);
                m = Convert.ToInt32(linesplt[2]);
                if (normal == 'y')
                {
                    gravData.cNor[L, m] = Convert.ToDouble(linesplt[3]);
                    gravData.sNor[L, m] = Convert.ToDouble(linesplt[4]);
                }
                else
                {
                    gravData.c[L, m] = Convert.ToDouble(linesplt[3]);
                    gravData.s[L, m] = Convert.ToDouble(linesplt[4]);
                }
                // some models don't have sigmas
                try
                {
                    gravData.cSig[L, m] = Convert.ToDouble(linesplt[5]);
                    gravData.sSig[L, m] = Convert.ToDouble(linesplt[6]);
                }
                catch
                { }

                // find normalized or unnormalized depending on which is already in file
                // note that above n = 170, the factorial will return 0, thus affecting the results!!!!
                if (m == 0)
                    conv = Math.Sqrt(MathTimeLibr.factorial(L + m) / (MathTimeLibr.factorial(L - m) * (2 * L + 1)));
                else
                    conv = Math.Sqrt(MathTimeLibr.factorial(L + m) / (MathTimeLibr.factorial(L - m) * 2 * (2 * L + 1)));
                if (normal == 'y')
                {
                    double temp = 1.0 / conv;
                    gravData.c[L, m] = temp * gravData.cNor[L, m];
                    gravData.s[L, m] = temp * gravData.sNor[L, m];
                }
                else
                {
                    gravData.cNor[L, m] = conv * gravData.c[L, m];
                    gravData.sNor[L, m] = conv * gravData.s[L, m];
                }
                ktr = ktr + 1;
            }
        } // initGravityField


        /*  *********1 * ********2 * ********3 * ********4 * ********5 * ********6 * ********7 * *
 *LEGFGL          93 / 03 / 23            0000.0    PGMR: FGL
 *
 * FUNCTION:  COMPUTES LEGENDRE AND ASSOCIATED LEGENDRE FUNCTIONS UP TO
 *DEGREE N AND ORDER M, N.GE.M
 *
 * I/O PARAMETERS:
 *
 *NAME I/O A/S DESCRIPTION OF PARAMETERS
 * ------------------------------------------------------------
 * N   IS DEGREE OF SPHERICAL HARMONICS
 * M   IS    ORDER OF SPHERICAL HARMONICS
 * X   IS SIN(LATITUDE)
 * POA    2-D LEGENDRE FUNCTIONS
 *
 * REFERENCES
 * JPL EM 312 / 87 - 153, 20 APRIL 1987
 *********1 * ********2 * ********3 * ********4 * ********5 * ********6 * ********7 * */
        public void geodynlegp
              (
               double latgc,
              Int32 degree,
               Int32 order,
                 out double[,] LegArr
           )
        {
            int i, j;
            double y;
            double x = Math.Sin(latgc);
            Int32 maxdeg = 50;
            Int32 maxord = 50;
            LegArr = new double[maxdeg + 2, maxord + 2];  // max deg and ord = 50

            // zero all out
            for (i = 1; i <= maxdeg + 1; i++)
            {
                for (j = 1; j <= maxord + 1; j++)
                    LegArr[i, j] = 0.0;
            }

            // recursions
            LegArr[1, 1] = 1.0;
            LegArr[2, 1] = x;

            // COMPUTE LEGENDRE FUNCTIONS UP TO DEGREE 
            for (i = 2; i <= degree; i++)
                LegArr[i + 1, 1] = (Convert.ToDouble(2 * i - 1) * x * LegArr[i, 1] - (i - 1) * LegArr[i - 1, 1]) / Convert.ToDouble(i);
            if (order == 0)
                return;  // just do zonals

            // COMPUTE ASSOCIATED LEGENDRE FUNCTIONS UP TO ORDER 
            y = Math.Sqrt(1.0 - x * x);
            LegArr[2, 2] = y;
            if (order != 1)
            {
                // THIS IS THE SECTORIAL PART OF THE ASSOCIATED FUNCTIONS
                for (i = 2; i <= order; i++)
                {
                    LegArr[i + 1, i + 1] = (2 * i - 1) * y * LegArr[i, i];
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
                        LegArr[i + 1, j + 1] = Convert.ToDouble(2 * i - 1) * y * LegArr[i, j];
                        if (i - 2 >= j)
                            LegArr[i + 1, j + 1] = LegArr[i + 1, j + 1] + LegArr[i - 1, j + 1];
                    }
                }
            }

        }  // geodynlegp



        /* ----------------------------------------------------------------------------
         *
         *                           function LegPoly
         *
         *   this function finds the Legendre polynomials for the gravity field. note that            
         *   the arrays are indexed from 0 to coincide with the usual nomenclature (eq 8-21 
         *   in my text). fortran implementations will have indicies of 1 greater as they 
         *   start at 1. note that high degree tesseral terms experience error for resonant 
         *   orbits. these are normalized expressions. 
         *      
         *  author        : david vallado                    719-573-2600  10 oct 2019
         *
         *  inputs        description                                   range / units
         *    latgc       - Geocentric lat of satellite                   pi to pi rad          
         *    lon         - longuitude of satellite                       rad
         *    order       - size of gravity field                         1..2160..
         *
         *  outputs       :
         *    LegArr      - array of Legendre polynomials
         *    trigArr     - array of trigonometric terms
         *
         *  locals :
         *    L,m         - degree and order indices
         *    conv        - conversion to un-normalize
         *
         *  coupling      :
         *   none
         *
         *  references :
         *    vallado       2013, 597, Eq 8-57
           ----------------------------------------------------------------------------*/

        public void LegPoly
           (
               double latgc,
               Int32 order,
               out double[,] LegArr,
               out double[,] LegArr1
           )
        {
            Int32 L, m;
            LegArr = new double[order + 2, order + 2];

            // try out montenbruck approach
            LegArr1 = new double[order + 2, order + 2];

            double[,] VArr = new double[order + 2, order + 2];
            double[,] WArr = new double[order + 2, order + 2];

            // initial values
            LegArr[0, 0] = 1.0;
            LegArr[0, 1] = 0.0;
            LegArr[1, 0] = Math.Sin(latgc);
            LegArr[1, 1] = Math.Cos(latgc);

            // -------------------- perform recursions ---------------------- }
            // -------------------- gtds approach
            for (L = 2; L <= order; L++)
            {
                for (m = 0; m <= L + 1; m++)
                    LegArr[L, m] = 0.0;

                for (m = 0; m <= L + 1; m++)
                {
                    // Legendre functions, zonal gtds
                    if (m == 0)
                        LegArr[L, 0] = ((2 * L - 1) * LegArr[1, 0] * LegArr[L - 1, 0] - (L - 1) * LegArr[L - 2, 0]) / L;
                    else
                    {
                        // associated Legendre functions
                        if (m == L)
                            LegArr[L, m] = (2 * L - 1) * LegArr[1, 1] * LegArr[L - 1, m - 1];  // sectoral part
                        else
                            LegArr[L, m] = LegArr[L - 2, m] + (2 * L - 1) * LegArr[1, 1] * LegArr[L - 1, m - 1];  // tesseral part
                    }
                }   // for m
            }   // for L

            // -------------------- alt approach montenbruck
            LegArr1[0, 0] = 1.0;
            LegArr1[0, 1] = 0.0;
            LegArr1[1, 0] = Math.Sin(latgc);
            LegArr1[1, 1] = Math.Cos(latgc);
            // Legendre functions, zonal gtds
            for (L = 2; L <= order; L++)
                LegArr1[L, L] = (2 * L - 1) * LegArr1[1, 1] * LegArr1[L - 1, L - 1];
            // associated Legendre functions
            for (L = 2; L <= order; L++)
            {
                for (m = 0; m < L; m++)  // L + 1??
                {
                    if (L == m + 1)
                        LegArr1[L, m] = (2 * m + 1) * LegArr[1, 0] * LegArr1[m, m];
                    else
                        LegArr1[L, m] = 1.0 / (L - m) * ((2 * L - 1) * LegArr1[1, 0] * LegArr1[L - 1, m] - (L + m - 1) * LegArr1[L - 2, m]);
                }
            }
        } // LegPoly



        /* ----------------------------------------------------------------------------
        *
        *                           function TrigPoly
        *
        *   this function finds the acumlated legendre polynomials and trigonmetric terms
        *   
        *  author        : david vallado                    719-573-2600  10 oct 2019
        *
        *  inputs        description                                   range / units
        *    latgc       - Geocentric lat of satellite                   pi to pi rad          
        *    lon         - longuitude of satellite                       rad
        *    order       - size of gravity field                         1..2160..
        *
        *  outputs       :
        *    LegArr      - array of Legendre polynomials
        *    trigArr     - array of trigonometric terms
        *
        *  locals :
        *    L,m         - degree and order indices
        *    conv        - conversion to un-normalize
        *
        *  coupling      :
        *   none
        *
        *  references :
        *    vallado       2013, 597, Eq 8-57
        * ----------------------------------------------------------------------------*/

        public void TrigPoly
           (
               double[] r,
               double lon,
               Int32 order,
               out double[,] trigArr,
               out double[,] VArr,
               out double[,] WArr
           )
        {
            // try out montenbruck approach
            trigArr = new double[order + 1, 3];
            VArr = new double[order + 2, order + 2];
            WArr = new double[order + 2, order + 2];

            double tlon, clon, slon, magr;
            Int32 L, m;

            magr = MathTimeLibr.mag(r);

            // -------------------- gtds approach
            trigArr[0, 0] = 0.0;    // sin terms
            trigArr[0, 1] = 1.0;    // cos terms
            tlon = Math.Tan(lon);
            trigArr[1, 0] = slon = Math.Sin(lon);  // initial value
            trigArr[1, 1] = clon = Math.Cos(lon);

            for (m = 2; m <= order; m++)
            {
                trigArr[m, 0] = 2.0 * clon * trigArr[m - 1, 0] - trigArr[m - 2, 0];  // sin terms
                trigArr[m, 1] = 2.0 * clon * trigArr[m - 1, 1] - trigArr[m - 2, 1];  // cos terms
                trigArr[m, 2] = (m - 1) * tlon + tlon;  // m tan
            }

            // -------------------- montenbruck approach
            // now form first set of recursions for l=m on V and W
            // initial zonal values
            double temp = MathTimeLib.globals.re / (magr * magr);
            VArr[0, 0] = MathTimeLib.globals.re / (magr * magr);
            WArr[0, 0] = 0.0;
            for (L = 1; L <= order; L++)
            {
                m = 0;
                if (L == 1)
                    VArr[L, m] = ((2 * L - 1) / L) * r[2] * temp * VArr[L - 1, 0];
                else
                    VArr[L, m] = ((2 * L - 1) / L) * r[2] * temp * VArr[L - 1, 0] - ((L - 1) / L) * temp * MathTimeLib.globals.re * VArr[L - 2, 0];
                WArr[L, m] = 0.0;
            }

            // now remaining values
            for (L = 1; L < order; L++)
            {
                m = L;
                VArr[L, m] = (2 * m - 1) * r[0] * temp * VArr[L - 1, m - 1] - r[1] * temp * WArr[L - 1, m - 1];
                WArr[L, m] = (2 * m - 1) * r[0] * temp * WArr[L - 1, m - 1] - r[1] * temp * VArr[L - 1, m - 1];

                for (m = L + 1; m <= order; m++)
                {
                    if (L == 1)
                    {
                        VArr[L, m] = ((2 * L - 1) / (L - m)) * r[2] * temp * VArr[L - 1, m];
                        WArr[L, m] = ((2 * L - 1) / (L - m)) * r[2] * temp * WArr[L - 1, m];
                    }
                    else
                    {
                        VArr[L, m] = ((2 * L - 1) / (L - m)) * r[2] * temp * VArr[L - 1, m] - 
                            ((L + m - 1) / (L - m)) * temp * MathTimeLib.globals.re * VArr[L - 2, m];
                        WArr[L, m] = ((2 * L - 1) / (L - m)) * r[2] * temp * WArr[L - 1, m] - 
                            ((L + m - 1) / (L - m)) * temp * MathTimeLib.globals.re * WArr[L - 2, m];
                    }
                }
            }

        } // TrigPoly


        /* ----------------------------------------------------------------------------
        *
        *                           function FullGeop
        *
        *   this function finds the Legendre polynomial value for the gravity field.            
        *      
        *  author        : david vallado                    719-573-2600  10 oct 2019
        *
        *  inputs        description                                   range / units
        *    r           - position vector ECEF                          km   
        *    order       - Size of gravity field                         1..360
        *    normal      - normalized in file                            'y', 'n'
        *    gravData.c  - gravitational coefficients (in gravityModelData)
        *    gravData.s  - gravitational coefficients (in gravityModelData)
        *
        *  outputs       :
        *    LegArr      - Array of Legendre Polynomials
        *
        *  locals :
        *    L, m        - degree and order indices
        *    conv        - conversion to un-normalize
        *
        *  coupling      :
        *   none
        *
        *  references :
        *    vallado       2013, 597, Eq 8-57
        * ----------------------------------------------------------------------------*/

        public void FullGeop
            (
                double[] r,
                double jd, double jdF,
                Int32 order,
                gravityModelData gravData,
                out double[] aPert,
                out double[,,] aPert1
            )
        {
            Int32 L, m;
            double[,] LegArr;
            double[,] trigArr;
            double[,] VArr;
            double[,] WArr;
            double[,] LegArr1;
            double oordelta, temp, oor, sumM1, sumM2, sumM3, distPartR,
                distPartPhi, distPartLon, RDelta, latgc, latgd, hellp, lon;
            aPert = new double[3];
            sumM1 = 0.0;
            sumM2 = 0.0;
            sumM3 = 0.0;
            aPert1 = new double[order + 2, order + 2, 3];

            // --------------------find latgc and lon---------------------- }
            ijk2ll(r, out latgc, out latgd, out lon, out hellp);

            // ---------------------Find Legendre polynomials -------------- }
            LegPoly(latgc, order, out LegArr, out LegArr1);

            TrigPoly(r, lon, order, out trigArr, out VArr, out WArr);

            // ----------Partial derivatives of disturbing potential ------- }
            oor = MathTimeLib.globals.re / MathTimeLibr.mag(r);
            distPartR = 0.0;
            distPartPhi = 0.0;
            distPartLon = 0.0;
            temp = oor;

            for (L = 2; L <= order; L++)
            {
                // will do the power as each L is indexed }
                temp = temp * oor;
                sumM1 = 0.0;
                sumM2 = 0.0;
                sumM3 = 0.0;

                for (m = 0; m <= L; m++)
                {
                    sumM1 = sumM1 + LegArr[L, m] * (gravData.cNor[L, m] * trigArr[m, 1] + gravData.sNor[L, m] * trigArr[m, 0]);
                    sumM2 = sumM2 + (LegArr[L, m + 1] - trigArr[m, 2] * LegArr[L, m]) *
                                    (gravData.cNor[L, m] * trigArr[m, 1] + gravData.sNor[L, m] * trigArr[m, 0]);
                    sumM3 = sumM3 + m * LegArr[L, m] * (gravData.sNor[L, m] * trigArr[m, 1] - gravData.cNor[L, m] * trigArr[m, 0]);
                } // for m 

                distPartR = distPartR + temp * (L + 1) * sumM1;
                distPartPhi = distPartPhi + temp * sumM2;
                distPartLon = distPartLon + temp * sumM3;
            } // for L 

            distPartR = -oor * oor * sumM1;
            distPartPhi = oor * sumM2;
            distPartLon = oor * sumM3;

            // ----------Non - spherical perturbative acceleration ------------ }
            RDelta = Math.Sqrt(r[0] * r[0] + r[1] * r[1]);
            oordelta = 1.0 / RDelta;
            temp = oor * distPartR - r[2] * oor * oor * oordelta * distPartPhi;
            double tmp = MathTimeLib.globals.mum / (Math.Pow(MathTimeLibr.mag(r), 3));

            aPert[0] = temp * r[1] - oordelta * distPartLon * r[1] - tmp * r[0];
            aPert[1] = temp * r[2] + oordelta * distPartLon * r[0] - tmp * r[1];
            aPert[2] = oor * distPartR * r[2] + oor * oor * RDelta * distPartPhi - tmp * r[2];

            // montenbruck approach
            for (L = 2; L <= order; L++)
            {
                // will do the power as each L is indexed }
                temp = MathTimeLib.globals.mu / (MathTimeLibr.mag(r) * MathTimeLibr.mag(r));

                for (m = 0; m <= L; m++)
                {
                    double temp1 = MathTimeLibr.factorial(L - m + 2) / MathTimeLibr.factorial(L - m);

                    if (m == 0)
                    {
                        aPert1[L, m, 0] = temp * (-gravData.cNor[L, m] * VArr[L + 1, 1]);
                        aPert1[L, m, 1] = temp * (-gravData.cNor[L, m] * WArr[L + 1, 1]);
                    }
                    else
                    {
                        aPert1[L, m, 0] = 0.5 * temp * ((-gravData.cNor[L, m] * VArr[L + 1, m + 1] - gravData.sNor[L, m] * WArr[L + 1, m + 1]) +
                            temp1 * (gravData.cNor[L, m] * VArr[L + 1, m - 1] + gravData.sNor[L, m] * WArr[L + 1, m - 1]));
                        aPert1[L, m, 1] = 0.5 * temp * ((-gravData.cNor[L, m] * WArr[L + 1, m + 1] + gravData.sNor[L, m] * VArr[L + 1, m + 1]) +
                            temp1 * (-gravData.cNor[L, m] * WArr[L + 1, m - 1] + gravData.sNor[L, m] * VArr[L + 1, m - 1]));
                    }
                    aPert1[L,m,2] = temp * ((L - m + 1) * (-gravData.cNor[L, m] * VArr[L + 1, m] - gravData.sNor[L, m] * WArr[L + 1, m]));
                } // for m 

            }

        }  // FullGeop 



            /* ---------------------------------------------------------------------------- 
            *
            *                           function pathm
            *
            *  this function determines the end position for a given range and azimuth
            *    from a given point.
            *
            *  author        : david vallado                  719-573-2600   27 may 2002
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
        * author        : david vallado                  719-573-2600   27 may 2002
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
        *  author        : david vallado                  719-573-2600   21 jun 2002
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
            double[] node_vec = new double[3];
            double[] h_vec = new double[3];
            double[] ecc_vec = new double[3];

            tm = new double[,] { { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 } };
            // -------- define gravitational constant
            //double MathTimeLib.globals.mum = 3.986004418e14;  // m3/s2

            // -------- parse the input vectors into cartesian and classical components
            rx = cartstate[0] * 1000.0;  // m
            ry = cartstate[1] * 1000.0;
            rz = cartstate[2] * 1000.0;
            vx = cartstate[3] * 1000.0;  // m/s
            vy = cartstate[4] * 1000.0;
            vz = cartstate[5] * 1000.0;
            reci = new double[] { rx, ry, rz };  // m
            veci = new double[] { vx, vy, vz };

            // -------- convert to a classical orbit state for ease of computation
            rv2coe(reci, veci, MathTimeLib.globals.mum, out p, out a, out ecc, out incl, out raan, out argp, out nu, out mean, out arglat, out truelon, out lonper);
            //  p = p * 1000.0;  // m
            //  a = a * 1000.0;
            n = Math.Sqrt(MathTimeLib.globals.mum / (a * a * a));

            // -------- calculate common quantities
            sqrt1me2 = Math.Sqrt(1.0 - ecc * ecc);
            magr = MathTimeLibr.mag(reci);  // m
            magr3 = Math.Pow(magr, 3);
            magv = MathTimeLibr.mag(veci);

            // ----------  form pqw position and velocity vectors ----------  
            r_dot_v = MathTimeLibr.dot(reci, veci);  // *1000.0 * 1000.0;

            ecc_term = magv * magv - MathTimeLib.globals.mum / magr;
            ecc_x = (ecc_term * rx - r_dot_v * vx) / MathTimeLib.globals.mum;
            ecc_y = (ecc_term * ry - r_dot_v * vy) / MathTimeLib.globals.mum;
            ecc_z = (ecc_term * rz - r_dot_v * vz) / MathTimeLib.globals.mum;
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

            sign_w = Math.Sign((magv * magv - MathTimeLib.globals.mum / magr) * rz - r_dot_v * vz);
            cos_w = n_dot_e / (ecc * node);
            argp = sign_w * Math.Acos(cos_w);
            w_scale = -sign_w / Math.Sqrt(1.0 - cos_w * cos_w);

            r_dot_e = MathTimeLibr.dot(reci, ecc_vec);
            cos_nu = r_dot_e / (magr * ecc);
            sign_nu = Math.Sign(r_dot_v);
            nu = sign_nu * Math.Acos(cos_nu);
            nu_scale = -sign_nu / Math.Sqrt(1.0 - cos_nu * cos_nu);

            // ---------------- calculate matrix elements ------------------
            // ---- partials of a wrt (x y z vx vy vz)
            p0 = 2.0 * a * a / Math.Pow(magr, 3);
            p1 = 2.0 / (n * n * a);
            //            p1 = -0.5 / a;  /// ????????????? no
            tm[0, 0] = p0 * rx;
            tm[0, 1] = p0 * ry;
            tm[0, 2] = p0 * rz;
            tm[0, 3] = p1 * vx;
            tm[0, 4] = p1 * vy;
            tm[0, 5] = p1 * vz;

            // ---- partials of ecc wrt (x y z vx vy vz)
            p0 = 1.0 / (MathTimeLib.globals.mum * ecc);
            tm[1, 0] = -p0 * (((vx * vy - MathTimeLib.globals.mum * rx * ry / magr3) * ecc_y) + ((vx * vz - MathTimeLib.globals.mum * rx * rz / magr3) * ecc_z) -
                (vy * vy + vz * vz - MathTimeLib.globals.mum / magr + MathTimeLib.globals.mum * rx * rx / magr3) * ecc_x);
            tm[1, 1] = -p0 * (((vx * vy - MathTimeLib.globals.mum * rx * ry / magr3) * ecc_x) + ((vy * vz - MathTimeLib.globals.mum * ry * rz / magr3) * ecc_z) -
                (vx * vx + vz * vz - MathTimeLib.globals.mum / magr + MathTimeLib.globals.mum * ry * ry / magr3) * ecc_y);
            tm[1, 2] = -p0 * (((vx * vz - MathTimeLib.globals.mum * rx * rz / magr3) * ecc_x) + ((vy * vz - MathTimeLib.globals.mum * ry * rz / magr3) * ecc_y) -
                (vy * vy + vx * vx - MathTimeLib.globals.mum / magr + MathTimeLib.globals.mum * rz * rz / magr3) * ecc_z);
            tm[1, 3] = -p0 * ((rx * vy - 2.0 * ry * vx) * ecc_y + (ry * vy + rz * vz) * ecc_x + (rx * vz - 2.0 * rz * vx) * ecc_z);
            tm[1, 4] = -p0 * ((ry * vx - 2.0 * rx * vy) * ecc_x + (rx * vx + rz * vz) * ecc_y + (ry * vz - 2.0 * rz * vy) * ecc_z);
            tm[1, 5] = -p0 * ((rx * vx + ry * vy) * ecc_z + (rz * vx - 2.0 * rx * vz) * ecc_x + (rz * vy - 2.0 * ry * vz) * ecc_y);

            // ---- partials of incl wrt (x y z vx vy vz)
            p3 = 1.0 / node;
            tm[2, 0] = -p3 * (vy - hz * (vy * hz - vz * hy) / h_squared);
            tm[2, 1] = p3 * (vx - hz * (vx * hz - vz * hx) / h_squared);
            tm[2, 2] = -p3 * (hz * (vy * hx - vx * hy) / h_squared);
            tm[2, 3] = p3 * (ry - hz * (ry * hz - rz * hy) / h_squared);
            tm[2, 4] = -p3 * (rx - hz * (rx * hz - rz * hx) / h_squared);
            tm[2, 5] = p3 * (hz * (ry * hx - rx * hy) / h_squared);

            // ---- partials of node wrt (x y z vx vy vz)
            p4 = 1.0 / n_squared;
            tm[3, 0] = -p4 * vz * ny;
            tm[3, 1] = p4 * vz * nx;
            tm[3, 2] = p4 * (vx * ny - vy * nx);
            tm[3, 3] = p4 * rz * ny;
            tm[3, 4] = -p4 * rz * nx;
            tm[3, 5] = p4 * (ry * nx - rx * ny);

            // ---- partials of argp wrt (x y z vx vy vz)
            p5 = 1.0 / (node * a * a);
            temp = -hy * (vy * vy + vz * vz - MathTimeLib.globals.mum / magr + MathTimeLib.globals.mum * rx * rx / magr3);
            temp = temp - hx * (vx * vy - MathTimeLib.globals.mum * rx * ry / magr3) + vz * MathTimeLib.globals.mum * ecc_x;
            temp = temp / (MathTimeLib.globals.mum * node * ecc) + vz * hy * n_dot_e / (node * node * node * ecc) - tm[1, 0] * n_dot_e / (node * ecc * ecc);
            tm[4, 0] = temp * w_scale;
            temp = hx * (vx * vx + vz * vz - MathTimeLib.globals.mum / magr + MathTimeLib.globals.mum * ry * ry / magr3);
            temp = temp + hy * (vx * vy - MathTimeLib.globals.mum * rx * ry / magr3) + vz * MathTimeLib.globals.mum * ecc_y;
            temp = temp / (MathTimeLib.globals.mum * node * ecc) - vz * hx * n_dot_e / (node * node * node * ecc) - tm[1, 1] * n_dot_e / (node * ecc * ecc);
            tm[4, 1] = temp * w_scale;
            temp = -hy * (vx * vz - MathTimeLib.globals.mum * rx * rz / magr3) + hx * (vy * vz - MathTimeLib.globals.mum * ry * rz / magr3) + vx * MathTimeLib.globals.mum * ecc_x + vy * MathTimeLib.globals.mum * ecc_y;
            temp = -temp / (MathTimeLib.globals.mum * node * ecc) + (vy * hx - vx * hy) * n_dot_e / (node * node * node * ecc) - tm[1, 2] * n_dot_e / (node * ecc * ecc);
            tm[4, 2] = temp * w_scale;
            temp = (rx * vy - 2.0 * ry * vx) * hx - hy * (ry * vy + rz * vz) + rz * MathTimeLib.globals.mum * ecc_x;
            temp = -temp / (MathTimeLib.globals.mum * node * ecc) - rz * hy * n_dot_e / (node * node * node * ecc) - tm[1, 3] * n_dot_e / (node * ecc * ecc);
            tm[4, 3] = temp * w_scale;
            temp = -(ry * vx - 2.0 * rx * vy) * hy + hx * (rx * vx + rz * vz) + rz * MathTimeLib.globals.mum * ecc_y;
            temp = -temp / (MathTimeLib.globals.mum * node * ecc) + rz * hx * n_dot_e / (node * node * node * ecc) - tm[1, 4] * n_dot_e / (node * ecc * ecc);
            tm[4, 4] = temp * w_scale;
            temp = -(rz * vx - 2.0 * rx * vz) * hy + hx * (rz * vy - 2.0 * ry * vz) - rx * MathTimeLib.globals.mum * ecc_x - ry * MathTimeLib.globals.mum * ecc_y;
            temp = -temp / (MathTimeLib.globals.mum * node * ecc) + (rx * hy - ry * hx) * n_dot_e / (node * node * node * ecc) - tm[1, 5] * n_dot_e / (node * ecc * ecc);
            tm[4, 5] = temp * w_scale;

            // ---- partials of true anomaly wrt (x y z vx vy vz)
            temp = ry * (vx * vy - MathTimeLib.globals.mum * rx * ry / magr3) - rx * ecc_term + rz * (vx * vz - MathTimeLib.globals.mum * rx * rz / magr3);
            temp = temp - rx * (vy * vy + vz * vz - MathTimeLib.globals.mum / magr + MathTimeLib.globals.mum * rx * rx / magr3) + vx * r_dot_v;
            temp = -temp / (MathTimeLib.globals.mum * magr * ecc) - rx * r_dot_e / (magr3 * ecc) - tm[1, 0] * r_dot_e / (magr * ecc * ecc);
            tm[5, 0] = temp * nu_scale;
            temp = rx * (vx * vy - MathTimeLib.globals.mum * rx * ry / magr3) - ry * ecc_term + rz * (vy * vz - MathTimeLib.globals.mum * ry * rz / magr3);
            temp = temp - ry * (vx * vx + vz * vz - MathTimeLib.globals.mum / magr + MathTimeLib.globals.mum * ry * ry / magr3) + vy * r_dot_v;
            temp = -temp / (MathTimeLib.globals.mum * magr * ecc) - ry * r_dot_e / (magr3 * ecc) - tm[1, 1] * r_dot_e / (magr * ecc * ecc);
            tm[5, 1] = temp * nu_scale;
            temp = rx * (vx * vz - MathTimeLib.globals.mum * rx * rz / magr3) - rz * ecc_term + ry * (vy * vz - MathTimeLib.globals.mum * ry * rz / magr3);
            temp = temp - rz * (vx * vx + vy * vy - MathTimeLib.globals.mum / magr + MathTimeLib.globals.mum * rz * rz / magr3) + vz * r_dot_v;
            temp = -temp / (MathTimeLib.globals.mum * magr * ecc) - rz * r_dot_e / (magr3 * ecc) - tm[1, 2] * r_dot_e / (magr * ecc * ecc);
            tm[5, 2] = temp * nu_scale;
            temp = ry * (rx * vy - 2.0 * ry * vx) + rx * (ry * vy + rz * vz) + rz * (rx * vz - 2.0 * rz * vx);
            temp = -temp / (MathTimeLib.globals.mum * magr * ecc) - tm[1, 3] * r_dot_e / (magr * ecc * ecc);
            tm[5, 3] = temp * nu_scale;
            temp = rx * (ry * vx - 2.0 * rx * vy) + ry * (rx * vx + rz * vz) + rz * (ry * vz - 2.0 * rz * vy);
            temp = -temp / (MathTimeLib.globals.mum * magr * ecc) - tm[1, 4] * r_dot_e / (magr * ecc * ecc);
            tm[5, 4] = temp * nu_scale;
            temp = rz * (rx * vx + ry * vy) + rx * (rz * vx - 2.0 * rx * vz) + ry * (rz * vy - 2.0 * ry * vz);
            temp = -temp / (MathTimeLib.globals.mum * magr * ecc) - tm[1, 5] * r_dot_e / (magr * ecc * ecc);
            tm[5, 5] = temp * nu_scale;

            //       // same answers as above
            //       // ---- partials of true anomaly wrt (x y z vx vy vz)
            //       p8 = magr*magr*magv*magv - MathTimeLib.globals.mum*magr - r_dot_v*r_dot_v;
            //       p9 = 1.0/(p8*p8 + r_dot_v*r_dot_v * h*h);
            //       tm[5,0] = p9 * ( p8 * (h*vx + r_dot_v*(vy*hz - vz*hy)/h) - r_dot_v*h*(2*rx*magv^2 - MathTimeLib.globals.mum*rx/magr - 2*r_dot_v*vx) );
            //       tm[5,1] = p9 * ( p8 * (h*vy + r_dot_v*(vz*hx - vx*hz)/h) - r_dot_v*h*(2*ry*magv^2 - MathTimeLib.globals.mum*ry/magr - 2*r_dot_v*vy) );
            //       tm[5,2] = p9 * ( p8 * (h*vz + r_dot_v*(vx*hy - vy*hx)/h) - r_dot_v*h*(2*rz*magv^2 - MathTimeLib.globals.mum*rz/magr - 2*r_dot_v*vz) );
            //       tm[5,3] = p9 * ( p8 * (h*rx + r_dot_v*(rz*hy - ry*hz)/h) - r_dot_v*h*(2*vx*magr^2 - 2*r_dot_v*rx) );
            //       tm[5,4] = p9 * ( p8 * (h*ry + r_dot_v*(rx*hz - rz*hx)/h) - r_dot_v*h*(2*vy*magr^2 - 2*r_dot_v*ry) );
            //       tm[5,5] = p9 * ( p8 * (h*rz + r_dot_v*(ry*hx - rx*hy)/h) - r_dot_v*h*(2*vz*magr^2 - 2*r_dot_v*rz) );

            if (anomclass.Equals("meana"))
            {
                // ---- partials of mean anomaly wrt (x y z vx vy vz)
                // then update for mean anomaly
                ecc = MathTimeLibr.mag(ecc_vec);
                dMdnu = Math.Pow(1.0 - ecc * ecc, 1.5) / (Math.Pow(1.0 + ecc * cos_nu, 2));  // dm/dv
                dMde = -Math.Sin(nu) * ((ecc * cos_nu + 1.0) * (ecc + cos_nu) / Math.Sqrt(Math.Pow(ecc + cos_nu, 2)) +
                    1.0 - 2.0 * ecc * ecc - ecc * ecc * ecc * cos_nu) / (Math.Pow(ecc * cos_nu + 1.0, 2) * Math.Sqrt(1.0 - ecc * ecc));  // dm/de
                tm[5, 0] = tm[5, 0] * dMde + tm[1, 0] * dMdnu;
                tm[5, 1] = tm[5, 1] * dMde + tm[1, 1] * dMdnu;
                tm[5, 2] = tm[5, 2] * dMde + tm[1, 2] * dMdnu;
                tm[5, 3] = tm[5, 3] * dMde + tm[1, 3] * dMdnu;
                tm[5, 4] = tm[5, 4] * dMde + tm[1, 4] * dMdnu;
                tm[5, 5] = tm[5, 5] * dMde + tm[1, 5] * dMdnu;
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
        *    e0          - eccentric anomaly                    0.0  to 2pi rad
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
            double p, a, ecc, incl, raan, argp, nu, mean, e0, e,
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
            a = classstate[0];  // m
                                // n = Math.Sqrt(MathTimeLib.globals.mum / (a * a * a));
            ecc = classstate[1];
            incl = classstate[2];
            raan = classstate[3];
            argp = classstate[4];
            nu = 0.0; // set initially
            // -------- if mean anomaly is used, convert to true anomaly
            // -------- eccentric anomaly (e) is needed for both
            if (anomclass.Equals("meana"))
            {
                mean = classstate[5];
                newtonm(ecc, mean, out e0, out nu);
            }
            else
            {
                if (anomclass.Equals("truea"))
                {
                    // note that mean is not used in the partials, but nu is! 
                    nu = classstate[5];
                    newtonnu(ecc, nu, out e, out mean);
                }
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
            p0 = Math.Sqrt(MathTimeLib.globals.mum / (a * (1.0 - ecc * ecc)));
            p1 = (1.0 - ecc * ecc) / (1.0 + ecc * cos_nu);
            p2 = 1.0 / (2.0 * a) * p0;
            p3 = (2.0 * a * ecc + a * cos_nu + a * cos_nu * ecc * ecc) / temp;
            p4 = ecc * MathTimeLib.globals.mum / (a * Math.Pow(1.0 - ecc * ecc, 2) * p0);
            p5 = a * p1;
            p6 = a * (1.0 - ecc * ecc) / (temp);

            dMdnu = Math.Pow(1.0 - ecc * ecc, 1.5) / (Math.Pow(1.0 + ecc * cos_nu, 2));  // dm/dv
            dMde = -sin_nu * ((ecc * cos_nu + 1.0) * (ecc + cos_nu) / Math.Sqrt(Math.Pow(ecc + cos_nu, 2)) + 1.0 - 2.0 * ecc * ecc - ecc * ecc * ecc * cos_nu) /
                (Math.Pow(ecc * cos_nu + 1.0, 2) * Math.Sqrt(1.0 - ecc * ecc));  // dm/de   
            
            // ---------------- calculate matrix elements ------------------
            // ---- partials of rx wrt (a e i O w m)
            tm[0, 0] = p1 * (p11 * cos_nu + p12 * sin_nu);
            tm[0, 0] = rx / a; // alternate approach
            tm[0, 1] = -p3 * (p11 * cos_nu + p12 * sin_nu);
            tm[0, 2] = p5 * p13 * (sin_w * cos_nu + cos_w * sin_nu);
            tm[0, 3] = -p5 * (p21 * cos_nu + p22 * sin_nu);
            tm[0, 4] = p5 * (p12 * cos_nu - p11 * sin_nu);
            //p10 = a * (ecc * ecc - 1.0) / Math.Pow(ecc * cos_nu + 1.0, 2);
            //tm[0, 5] = p10 * (ecc * Math.Cos(raan) * sin_w + cos_raan * cos_w * sin_nu +
            //    cos_raan * sin_w * cos_nu + ecc * cos_inc * sin_raan * cos_w +
            //    cos_inc * sin_raan * cos_w * cos_nu - cos_inc * sin_raan * sin_w * sin_nu);
            tm[0, 5] = p6 * (-p11 * sin_nu + p12 * (ecc + cos_nu));
            if (anomclass.Equals("meana"))
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
            if (anomclass.Equals("meana"))
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
            if (anomclass.Equals("meana"))
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
            //p10 = Math.Sqrt(-MathTimeLib.globals.mum / (a * (ecc * ecc - 1.0)));
            //tm[3, 5] = p10 * (Math.Cos(raan) * sin_w * sin_nu - Math.Cos(raan) * cos_w * cos_nu +
            //    cos_inc * sin_raan * cos_w * sin_nu + cos_inc * sin_raan * sin_w * cos_nu);
            tm[3, 5] = -p0 * (p11 * cos_nu + p12 * sin_nu);
            if (anomclass.Equals("meana"))
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
            //p10 = Math.Sqrt(-MathTimeLib.globals.mum / (a * (ecc * ecc - 1.0)));
            //tm[4, 5] = -p10 * (sin_raan * cos_w * cos_nu - sin_raan * sin_w * sin_nu +
            //    cos_inc * Math.Cos(raan) * cos_w * sin_nu + cos_inc * Math.Cos(raan) * sin_w * cos_nu);
            tm[4, 5] = -p0 * (p21 * cos_nu + p22 * sin_nu);
            if (anomclass.Equals("meana"))
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
            //p10 = Math.Sqrt(-MathTimeLib.globals.mum / (a * (ecc * ecc - 1.0)));
            //tm[5, 5] = p10 * (-Math.Sin(incl) * Math.Sin(argp + nu));
            tm[5, 5] = -p0 * (p31 * cos_nu + p32 * sin_nu);
            if (anomclass.Equals("meana"))
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
        *  author        : david vallado                  719-573-2600   14 jul 2002
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
        *    e0          - eccentric anomaly                    0.0  to 2pi rad
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

            a = 1.0 / (2.0 / magr - magv * magv / MathTimeLib.globals.mum);
            n = Math.Sqrt(MathTimeLib.globals.mum / (a * a * a));

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
            p1 = magv * magv - MathTimeLib.globals.mum / magr;
            ecc_x = (p1 * rx - r_dot_v * vx) / MathTimeLib.globals.mum;
            ecc_y = (p1 * ry - r_dot_v * vy) / MathTimeLib.globals.mum;
            ecc_z = (p1 * rz - r_dot_v * vz) / MathTimeLib.globals.mum;
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

            A = Math.Sqrt(MathTimeLib.globals.mum * a);
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
            p0 = 1.0 / MathTimeLib.globals.mum;
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
            p0 = 1.0 / MathTimeLib.globals.mum;
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
                //            p1 = 1.0 / MathTimeLib.globals.mum;
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
        *  author        : david vallado                  719-573-2600   24 jul 2003
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
        *    e0          - eccentric anomaly                    0.0  to 2pi rad
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
            double p, a, n, ecc, raan, argp, nu, m, e0, af, ag, chi, psi, X, Y, b, meanlonM, meanlonNu,
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
                a = eqstate[0];  // in m
                n = Math.Sqrt(MathTimeLib.globals.mum / (a * a * a));
            }
            else
            {
                if (anomeq.Equals("truen") || anomeq.Equals("meann"))
                {
                    n = eqstate[0];
                    a = Math.Pow(MathTimeLib.globals.mum / (n * n), 1.0 / 3.0);
                }
                else
                    if (anomeq.Equals("truep") || anomeq.Equals("meanp"))
                {
                    p = eqstate[0];
                    a = Math.Pow(MathTimeLib.globals.mum / (n * n), 1.0 / 3.0);
                    ecc = Math.Sqrt(1.0 - p / a);
                    n = Math.Sqrt(MathTimeLib.globals.mum / (a * a * a));
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
                    newtonnu(ecc, nu, out e0, out m);
                    meanlonM = fr * raan + argp + m;
                    meanlonM = meanlonM % twopi;
                }
            }

            // needs to be mean longitude for eq2rv
            eq2rv(a * 0.001, af, ag, chi, psi, meanlonM, fr, out reci, out veci);
            rx = reci[0] * 1000.0;
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
            // ---- partials of rx wrt (a af ag chi psi meanlon)
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

            // ---- partials of ry wrt (a af ag chi psi meanlon)
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

            // ---- partials of rz wrt (a af ag chi psi meanlon)
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

            // ---- partials of vx wrt (a af ag chi psi meanlon)
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

            // ---- partials of vy wrt (a af ag chi psi meanlon)
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

            // ---- partials of vz wrt (a af ag chi psi meanlon)
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
                    ecc_term = magv * magv - MathTimeLib.globals.mum / magr;
                    ecc_x = (ecc_term * rx - r_dot_v * vx) / MathTimeLib.globals.mum;
                    ecc_y = (ecc_term * ry - r_dot_v * vy) / MathTimeLib.globals.mum;
                    ecc_z = (ecc_term * rz - r_dot_v * vz) / MathTimeLib.globals.mum;
                    r_dot_e = Math.Sqrt(rx * ecc_x + ry * ecc_y + rz * ecc_z);
                    nu_scale = -Math.Sign(r_dot_v) / Math.Sqrt(1 - Math.Cos(nu) * Math.Cos(nu));
                    magr3 = Math.Pow(magr, 3);
                    temp = ry * (vx * vy - MathTimeLib.globals.mum * rx * ry / magr3) - rx * ecc_term + rz * (vx * vz - MathTimeLib.globals.mum * rx * rz / magr3);
                    temp = temp - rx * (vy * vy + vz * vz - MathTimeLib.globals.mum / magr + MathTimeLib.globals.mum * rx * rx / magr3) + vx * r_dot_v;
                    temp = -temp / (MathTimeLib.globals.mum * magr * ecc) - rx * r_dot_e / (magr3 * ecc) - tm[1, 0] * r_dot_e / (magr * ecc * ecc);
                    tm[5, 0] = temp * nu_scale;
                    temp = rx * (vx * vy - MathTimeLib.globals.mum * rx * ry / magr3) - ry * ecc_term + rz * (vy * vz - MathTimeLib.globals.mum * ry * rz / magr3);
                    temp = temp - ry * (vx * vx + vz * vz - MathTimeLib.globals.mum / magr + MathTimeLib.globals.mum * ry * ry / magr3) + vy * r_dot_v;
                    temp = -temp / (MathTimeLib.globals.mum * magr * ecc) - ry * r_dot_e / (magr3 * ecc) - tm[1, 1] * r_dot_e / (magr * ecc * ecc);
                    tm[5, 1] = temp * nu_scale;
                    temp = rx * (vx * vz - MathTimeLib.globals.mum * rx * rz / magr3) - rz * ecc_term + ry * (vy * vz - MathTimeLib.globals.mum * ry * rz / magr3);
                    temp = temp - rz * (vx * vx + vy * vy - MathTimeLib.globals.mum / magr + MathTimeLib.globals.mum * rz * rz / magr3) + vz * r_dot_v;
                    temp = -temp / (MathTimeLib.globals.mum * magr * ecc) - rz * r_dot_e / (magr3 * ecc) - tm[1, 2] * r_dot_e / (magr * ecc * ecc);
                    tm[5, 2] = temp * nu_scale;
                    temp = ry * (rx * vy - 2.0 * ry * vx) + rx * (ry * vy + rz * vz) + rz * (rx * vz - 2 * rz * vx);
                    temp = -temp / (MathTimeLib.globals.mum * magr * ecc) - tm[1, 3] * r_dot_e / (magr * ecc * ecc);
                    tm[5, 3] = temp * nu_scale;
                    temp = rx * (ry * vx - 2.0 * rx * vy) + ry * (rx * vx + rz * vz) + rz * (ry * vz - 2 * rz * vy);
                    temp = -temp / (MathTimeLib.globals.mum * magr * ecc) - tm[1, 4] * r_dot_e / (magr * ecc * ecc);
                    tm[5, 4] = temp * nu_scale;
                    temp = rz * (rx * vx + ry * vy) + rx * (rz * vx - 2.0 * rx * vz) + ry * (rz * vy - 2 * ry * vz);
                    temp = -temp / (MathTimeLib.globals.mum * magr * ecc) - tm[1, 5] * r_dot_e / (magr * ecc * ecc);
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
        *  author        : david vallado                  719-573-2600   14 jul 2002
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
        *    MathTimeLib.globals.mum, gravitational paramater   m^3/s^2 NOTE Meters!
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
            a = classstate[0];  // in m
            n = Math.Sqrt(MathTimeLib.globals.mum / (a * a * a));
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
                    tm[0, 0] = -(3.0 * Math.Sqrt(MathTimeLib.globals.mum / (a * a * a))) / (2.0 * a);  // if class = a, equin = n
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
                //[e0,nu] = newtonm ( ecc,anomaly );
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
        *  author        : david vallado                  719-573-2600   18 jun 2002
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
        *    MathTimeLib.globals.mum, gravitational paramater         m^3/s^2 NOTE Meters!
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
                a = eqstate[0];  // in m
                n = Math.Sqrt(MathTimeLib.globals.mum / (a * a * a));
            }
            else
            {
                if (anomeq.Equals("truen") || anomeq.Equals("meann"))
                {
                    n = eqstate[0];
                    a = Math.Pow(MathTimeLib.globals.mum / (n * n), 1.0 / 3.0);
                }
                else
                    if (anomeq.Equals("truep") || anomeq.Equals("meanp"))
                {
                    p = eqstate[0];
                    a = Math.Pow(MathTimeLib.globals.mum / (n * n), 1.0 / 3.0);
                    ecc = Math.Sqrt(1.0 - p / a);
                    n = Math.Sqrt(MathTimeLib.globals.mum / (a * a * a));
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
                    tm[0, 0] = -2.0 / (3.0 * n) * Math.Pow(MathTimeLib.globals.mum / (n * n), (1.0 / 3.0));
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
            if (anomclass.Equals("truea"))
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
                if (anomclass.Equals("meana"))
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
        *  author        : david vallado                  719-573-2600   21 jun 2002
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
            double[,] cartcov, double[] cartstate, string anomflt, double ttt, double jdut1, double lod,
            double xp, double yp, Int16 terms, double ddpsi, double ddeps, EOPSPWLib.iau80Class iau80arr,
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
                eci_ecef(ref reci, ref veci, iau80arr, MathTimeLib.Edirection.eto, ttt, jdut1, lod, xp, yp, terms, ddpsi, ddeps, opt, ref recef, ref vecef);
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
            else
            {
                if (anomflt.Equals("radec"))
                {
                }
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
                tm[1, 2] = Math.Sqrt(rxf * rxf + ryf * ryf) / magr * magr;
                tm[1, 3] = 0.0;
                tm[1, 4] = 0.0;
                tm[1, 5] = 0.0;
            }
            else
            {
                if (anomflt.Equals("radec"))
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
                    tm[1, 2] = Math.Sqrt(rx * rx + ry * ry) / magr * magr;
                    tm[1, 3] = 0.0;
                    tm[1, 4] = 0.0;
                    tm[1, 5] = 0.0;
                }
            }

            // partial of fpa wrt (x y z vx vy vz)
            rdot = rdotv / magr;  // (r dot v) / r
            p1 = -1.0 / (magr * Math.Sqrt(magv * magv - rdot * rdot));
            tm[2, 0] = p1 * (rdot * rx / magr - vx);
            tm[2, 1] = p1 * (rdot * ry / magr - vy);
            tm[2, 2] = p1 * (rdot * rz / magr - vz);
            tm[2, 3] = p1 * (rdot * magr * vx / magv * magv - rx);
            tm[2, 4] = p1 * (rdot * magr * vy / magv * magv - ry);
            tm[2, 5] = p1 * (rdot * magr * vz / magv * magv - rz);
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
        *  author        : david vallado                  719-573-2600   27 may 2003
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
            double[,] flcov, double[] flstate, string anomflt, double ttt, double jdut1, double lod,
            double xp, double yp, Int16 terms, double ddpsi, double ddeps, EOPSPWLib.iau80Class iau80arr,
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

            // initiialize
            cd = 0.0;
            sd = 0.0;
            cdf = 0.0;
            sdf = 0.0;
            sra = 0.0;
            cra = 0.0;
            sraf = 0.0;
            craf = 0.0;

            tm = new double[,] { { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0 } };

            small = 0.00000001;

            // -------- parse the input vectors into components
            lon = flstate[0]; // these will come in as either lon/lat or rtasc/decl dep}ing on anom1
            latgc = flstate[1];
            fpa = flstate[2];
            az = flstate[3];
            magr = flstate[4];  // already converted to m in setcov
            magv = flstate[5];

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
                eci_ecef(ref recef, ref vecef, iau80arr, MathTimeLib.Edirection.efrom, ttt, jdut1, lod, xp, yp, terms, ddpsi, ddeps, opt, ref reci, ref veci);
                reci[0] = reci[0] * 1000.0;  // in m
                reci[1] = reci[1] * 1000.0;
                reci[2] = reci[2] * 1000.0;
                veci[0] = veci[0] * 1000.0;  // in m/s
                veci[1] = veci[1] * 1000.0;
                veci[2] = veci[2] * 1000.0;

                temp = Math.Sqrt(reci[0] * reci[0] + reci[1] * reci[1]);
                if (temp < small)
                {
                    rtasc = Math.Atan2(veci[1], veci[0]);
                }
                else
                {
                    rtasc = Math.Atan2(reci[1], reci[0]);
                }
                //decl = atan2( reci[2] , Math.Sqrt(reci[0]^2 + reci[1]^2) )
                decl = Math.Asin(reci[2] / magr);
                cra = Math.Cos(rtasc);
                sra = Math.Sin(rtasc);
                cd = Math.Cos(decl);
                sd = Math.Sin(decl);
            }
            else
            {
                if (anomflt.Equals("radec"))
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
            }

            // ---------------- calculate matrix elements ------------------
            // ---- partials of rx wrt (lon latgc fpa az r v)
            if (anomflt.Equals("radec"))
            {
                tm[0, 0] = -magr * cd * sra;
                tm[0, 1] = -magr * sd * cra;
            }
            else
            {
                if (anomflt.Equals("latlon"))
                {
                    tm[0, 0] = -magr * cdf * sraf;
                    tm[0, 1] = -magr * sdf * craf;
                }
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
            else
            {
                if (anomflt.Equals("latlon"))
                {
                    tm[1, 0] = magr * cdf * craf;
                    tm[1, 1] = -magr * sdf * sraf;
                }
            }
            tm[0, 2] = 0.0;
            tm[0, 3] = 0.0;
            tm[0, 4] = cd * sra;
            tm[0, 5] = 0.0;

            // ---- partials of rz wrt (lon latgc fpa az r v)
            if (anomflt.Equals("radec"))
            {
                tm[2, 0] = 0.0;
                tm[2, 1] = magr * cd;
            }
            else
            {
                if (anomflt.Equals("latlon"))
                {
                    tm[2, 0] = 0.0;
                    tm[2, 1] = magr * cdf;
                }
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
            else
            {
                if (anomflt.Equals("latlon"))
                {
                    tm[3, 0] = -magv * (-sraf * caz * sdf * cfpa + craf * saz * cfpa + cdf * sraf * sfpa);
                    //  tm[3,0] = -vy;
                    tm[3, 1] = -craf * magv * (sdf * sfpa + cdf * caz * cfpa);
                    //  tm[3,1] = -vz*cra;
                }
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
            else
            {
                if (anomflt.Equals("latlon"))
                {
                    tm[4, 0] = magv * (-craf * caz * sdf * cfpa - sraf * saz * cfpa + cdf * craf * sfpa);
                    //  tm[4,0] = vx;
                    tm[4, 1] = -sraf * magv * (sdf * sfpa + cdf * caz * cfpa);
                    //   tm[4,1] = -vz*sra;
                }
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
            else
            {
                if (anomflt.Equals("latlon"))
                {
                    tm[5, 0] = 0.0;
                    tm[5, 1] = magv * (cdf * sfpa - sdf * caz * cfpa);
                }
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
        *  author        : david vallado                  719-573-2600   20 may 2003
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
            ref double[,] cartcov, double[] cartstate, MathTimeLib.Edirection direct, ref double[,] rswcov, out double[,] tm
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
        *  author        : david vallado                  719-573-2600   17 jul 2003
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
            ref double[,] cartcov, double[] cartstate, MathTimeLib.Edirection direct, ref double[,] ntwcov, out double[,] tm
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


    }  // AstroLib

}  // namespace AstroMethods
