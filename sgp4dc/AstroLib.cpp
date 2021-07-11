/*     -------------------------------------------------------------------------
*
*                                AstroLib.cpp
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
*              15 jan 19  david vallado
*                           combine with astiod etc
*    changes :
*              11 jan 18  david vallado
*                           misc cleanup
*              30 sep 15  david vallado
*                           fix jd, jdfrac
*               3 nov 14  david vallado
*                           update to msvs2013 c++
*               4 may 09  david vallado
*                           misc updates
*              23 feb 07  david vallado
*                           3rd edition baseline
*              21 jul 05  david vallado
*                           2nd printing baseline
*              14 may 01  david vallado
*                           2nd edition baseline
*              23 nov 87  david vallado
*                           original baseline
*       ----------------------------------------------------------------      */

#include "AstroLib.h"

namespace AstroLib
{
	// simple function to store help levels that can be set on/off during execution
	void sethelp
	(
		char& iauhelp, char iauopt
	)
	{
		static char iaustore;

		if (iauopt != ' ')
		{
			iauhelp = iauopt;
			iaustore = iauopt;
		}
		else
			iauhelp = iaustore;
	}



	// -----------------------------------------------------------------------------------------
	//                                       coordinate functions
	// -----------------------------------------------------------------------------------------

	/* ----------------------------------------------------------------------------
	*
	*                           function ddpsiddeps_dxdy
	*
	*  this function transforms the offset corrections between the ddpsi/ddeps and
	*  the dx/dy.
	*
	*  author        : david vallado                  719-573-2600   11 nov 2005
	*
	*  revisions
	*
	*  inputs        : description                    range / units
	*    ttt         - julian centuries of tt         centuries
	*    ddpsi, ddeps- offsets to iau2000a            rad
	*    direct      - direction of transfer          eFrom, eTo
	*
	*  outputs       :
	*    dx, dy      - offsets to iau2000a            rad
	*
	*  locals        :
	*    psia        - cannonical precession angle    rad
	*    wa          - cannonical precession angle    rad
	*    epsa        - cannonical precession angle    rad
	*    chia        - cannonical precession angle    rad
	*
	*  coupling      :
	*    none
	*
	*  references    :
	*    vallado       2007, 217
	*    lieske et al. 1977, A&A 58, pp. 1-16
	*    IERS Conventions 2000, chap. 5, herring et al. 2002, JGR 107, B4
	* --------------------------------------------------------------------------- */

	void ddpsiddeps_dxdy
	(
		double ttt, double& ddpsi, double& ddeps,
		MathTimeLib::edirection direct,
		double& dx, double& dy
	)
	{
		double psia, chia, epsa, epsa1;
		const double a2r = 4.84813681109535993e-6;

		//     current values of precession
		psia = ((((-0.0000000951 * ttt + 0.000132851) * ttt - 0.00114045) * ttt - 1.0790069) * ttt + 5038.481507) * ttt; // "

		// value from website code - intermediate value
		epsa1 = 84381.448 - 46.8402 * ttt - 0.00059 * ttt * ttt + 0.001813 * ttt * ttt * ttt;
		epsa = ((((-0.0000000434 * ttt - 0.000000576) * ttt + 0.00200340) * ttt - 0.0001831) * ttt - 46.836769) * ttt + 84381.406;
		// use new value of eo or old one?

		chia = ((((-0.0000000560 * ttt + 0.000170663) * ttt - 0.00121197) * ttt - 2.3814292) * ttt + 10.556403) * ttt;

		psia = psia * a2r;
		chia = chia * a2r;
		epsa = epsa * a2r;

		if (direct == MathTimeLib::eTo)
		{
			//	Chapter 5, equation (23)
			dx = ddpsi * sin(epsa) + ddeps * (psia * cos(epsa) - chia);
			dy = ddeps - ddpsi * sin(epsa) * (psia * cos(epsa) - chia);
		}
		else
		{
			//	Chapter 5, equation (23) reversed
			ddpsi = dx / sin(epsa) - dy * (psia * cos(epsa) - chia) / sin(epsa);
			ddeps = dy + dx * (psia  *  cos(epsa) - chia);
		}
	}   // procedure ddpsiddeps_dxdy



/* -----------------------------------------------------------------------------
*
*                           function iau80in
*
*  this function initializes the nutation matricies needed for reduction
*    calculations. the routine needs the filename of the files as input.
*
*  author        : david vallado                  719-573-2600   27 may 2002
*
*  revisions
*    vallado     - conversion to c++                             21 feb 2005
*
*  inputs          description                    range / units
*    EopLoc      - location of data input file
*  outputs       :
*    iau80arr    - record containing the iau80 constants rad
*
*  locals        :
*    convrt      - conversion factor milli arcsec to radians
*    i,j         - index
*
*  coupling      :
*    none        -
*
*  references    :
* --------------------------------------------------------------------------- */

	void iau80in
	(
		std::string EopLoc,
		iau80data& iau80arr
	)
	{
		FILE *infile;
		double convrt;
		int i, j, ret;

		// ------------------------  implementation   -------------------
		convrt = 0.0001 * pi / (180 * 3600.0); 	// 0.0001" to rad

#ifdef _MSC_VER
		infile = fopen(EopLoc.c_str(), "r");
#else
		infile = fopen(EopLoc, "r");
#endif

		for (i = 0; i < 106; i++)
		{
#ifdef _MSC_VER
			ret = fscanf_s(infile, "%d %d %d %d %d %lf %lf %lf %lf %d \n ",
				&iau80arr.iar80[i][0], &iau80arr.iar80[i][1], &iau80arr.iar80[i][2],
				&iau80arr.iar80[i][3], &iau80arr.iar80[i][4],
				&iau80arr.rar80[i][0], &iau80arr.rar80[i][1], &iau80arr.rar80[i][2],
				&iau80arr.rar80[i][3], &j);
#else
			ret = fscanf(infile, "%d %d %d %d %d %lf %lf %lf %lf %d \n ",
				&iau80arr.iar80[i][0], &iau80arr.iar80[i][1], &iau80arr.iar80[i][2],
				&iau80arr.iar80[i][3], &iau80arr.iar80[i][4],
				&iau80arr.rar80[i][0], &iau80arr.rar80[i][1], &iau80arr.rar80[i][2],
				&iau80arr.rar80[i][3], &j);
#endif
			if (ret == EOF)
			{
				break;      /* get out of loop reading lines � found end of file prematurely */
			}

			for (j = 0; j < 4; j++)
				iau80arr.rar80[i][j] = iau80arr.rar80[i][j] * convrt;
		}
		fclose(infile);
	}  // procedure iau80in


		/* ----------------------------------------------------------------------------
		*
		*                           function iau00in
		*
		*  this function initializes the matricies needed for iau 2000 reduction
		*    calculations. the routine uses the files listed as inputs, but they are
		*    are not input to the routine as they are static files. 
		*
		*  author        : david vallado                  719-573-2600   16 jul 2004
		*
		*  revisions
		*    dav 14 apr 11  update for iau2010 conventions
		*
		*  inputs description                                      range / units
		*    none
		*    iau00x.dat  - file for x coefficient
		*    iau00y.dat  - file for y coefficient
		*    iau00s.dat  - file for s coefficient
		*    iau00n.dat  - file for nutation coefficients
		*    iau00pl.dat notused - file for planetary nutation coefficients
		*    iau00gs.dat - file for gmst coefficients
		*
		*  outputs       :
		*    axs0        - real coefficients for x                     rad
		*    a0xi        - integer coefficients for x
		*    ays0        - real coefficients for y                     rad
		*    a0yi        - integer coefficients for y
		*    ass0        - real coefficients for s                     rad
		*    a0si        - integer coefficients for s
		*    apn         - real coefficients for nutation              rad
		*    apni        - integer coefficients for nutation
		*    ape         - real coefficients for obliquity             rad
		*    apei        - integer coefficients for obliquity
		*    agst        - real coefficients for gst                   rad
		*    agsti       - integer coefficients for gst
		*
		*  locals        :
		*    convrt      - conversion factor to radians
		*    i           - index
		*
		*  coupling      :
		*    none        -
		*
		*  references    :
		*    vallado     2013, pg 205-219, 910-912
		* ----------------------------------------------------------------------------- */

	void iau00in
	(
		std::string EopLoc,
		iau00data& iau00arr
	)
	{
		FILE *infile;
		// string line;
		int i, j, ret, tmpint;
		double tmpdbl;
		// int k, numsegs, ktr;

		// ------------------------  implementation   -------------------
		// " to rad
		double convrtu = (0.000001 * pi) / (180.0 * 3600.0);  // if micro arcsecond
		double convrtm = (0.001 * pi) / (180.0 * 3600.0);     // if milli arcsecond

		// ------------------------------
		//  note that since all these coefficients have only a single decimal place, one could store them as integers, and then simply
		//  divide by one additional power of ten. it would make memeory storage much smaller and potentially faster.

		// xys values
		// tab5.2a.txt in IERS
		// find first data point, and number for the segment
		//ktr = 0;  // ktr going through the file
		//k = 0;    // ktr through array of values
		//while (ktr < fileData.Count())
		//{
		//    while (!(fileData[ktr][0] == 'j'))
		//        ktr = ktr + 1;
		//    numsegs = Convert.ToInt32(fileData[ktr].Split('=')[2]);
		//    ktr = ktr + 2;
		//    for (i = 0; i < numsegs; i++)
		//    {
		//        //line = Regex.Replace(fileData[i], @"\s+", "|");
		//        //string[] linedata = line.Split('|');
		//        // reals
		//        string[] linedata = Regex.Split(fileData[ktr], pattern);
		//        iau00arr.axs0[k, 0] = Convert.ToDouble(linedata[2]) * convrtu;  // rad
		//        iau00arr.axs0[k, 1] = Convert.ToDouble(linedata[3]) * convrtu;  // rad
		//                                                                        // integers
		//        for (j = 0; j < 13; j++)
		//            iau00arr.a0xi[k, j] = Convert.ToInt32(linedata[j + 4]);
		//        k = k + 1;
		//        ktr = ktr + 1;
		//    }
		//}

			// iau00x.txt in IERS
#ifdef _MSC_VER
		infile = fopen((EopLoc + "iau00x.dat").c_str(), "r");
#else
		infile = fopen(EopLoc, "r");
#endif
			for (i = 0; i < 1600; i++)
			{
#ifdef _MSC_VER
				ret = fscanf_s(infile, "%d %lf %lf %d %d %d %d %d %d %d %d %d %d %d %d %d %d \n ",
					&tmpint, &iau00arr.axs0[i][0], &iau00arr.axs0[i][1],
					&iau00arr.a0xi[i][0], &iau00arr.a0xi[i][1], &iau00arr.a0xi[i][2], &iau00arr.a0xi[i][3],
					&iau00arr.a0xi[i][4], &iau00arr.a0xi[i][5], &iau00arr.a0xi[i][6], &iau00arr.a0xi[i][7],
					&iau00arr.a0xi[i][8], &iau00arr.a0xi[i][9], &iau00arr.a0xi[i][10], &iau00arr.a0xi[i][11], 
					&iau00arr.a0xi[i][12], &iau00arr.a0xi[i][13]);
#else
				ret = fscanf(infile, "%d %lf %lf %d %d %d %d %d %d %d %d %d %d %d %d %d %d \n ",
					&tmpint, &iau00arr.axs0[i][0], &iau00arr.axs0[i][1],
					&iau00arr.a0xi[i][0], &iau00arr.a0xi[i][1], &iau00arr.a0xi[i][2], &iau00arr.a0xi[i][3],
					&iau00arr.a0xi[i][4], &iau00arr.a0xi[i][5], &iau00arr.a0xi[i][6], &iau00arr.a0xi[i][7],
					&iau00arr.a0xi[i][8], &iau00arr.a0xi[i][9], &iau00arr.a0xi[i][10], &iau00arr.a0xi[i][11],
					&iau00arr.a0xi[i][12], &iau00arr.a0xi[i][13]);
#endif

				for (j = 0; j <= 1; j++)
					iau00arr.axs0[i][j] = iau00arr.axs0[i][j] * convrtu;   // rad
			}

		// tab5.2b.txt in IERS
#ifdef _MSC_VER
			infile = fopen((EopLoc + "iau00y.dat").c_str(), "r");
#else
			infile = fopen(EopLoc, "r");
#endif
			for (i = 0; i < 1275; i++)
			{
#ifdef _MSC_VER
				ret = fscanf_s(infile, "%d %lf %lf %d %d %d %d %d %d %d %d %d %d %d %d %d %d \n ",
					&tmpint, &iau00arr.ays0[i][0], &iau00arr.ays0[i][1],
					&iau00arr.a0yi[i][0], &iau00arr.a0yi[i][1], &iau00arr.a0yi[i][2], &iau00arr.a0yi[i][3],
					&iau00arr.a0yi[i][4], &iau00arr.a0yi[i][5], &iau00arr.a0yi[i][6], &iau00arr.a0yi[i][7],
					&iau00arr.a0yi[i][8], &iau00arr.a0yi[i][9], &iau00arr.a0yi[i][10], &iau00arr.a0yi[i][11], 
					&iau00arr.a0yi[i][12], &iau00arr.a0yi[i][13]);
#else
				ret = fscanf(infile, "%d %lf %lf %d %d %d %d %d %d %d %d %d %d %d %d %d %d \n ",
					&tmpint, &iau00arr.ays0[i][0], &iau00arr.ays0[i][1],
					&iau00arr.a0yi[i][0], &iau00arr.a0yi[i][1], &iau00arr.a0yi[i][2], &iau00arr.a0yi[i][3],
					&iau00arr.a0yi[i][4], &iau00arr.a0yi[i][5], &iau00arr.a0yi[i][6], &iau00arr.a0yi[i][7],
					&iau00arr.a0yi[i][8], &iau00arr.a0yi[i][9], &iau00arr.a0yi[i][10], &iau00arr.a0yi[i][11],
					&iau00arr.a0yi[i][12], &iau00arr.a0yi[i][13]);
#endif

				for (j = 0; j <= 1; j++)
					iau00arr.ays0[i][j] = iau00arr.ays0[i][j] * convrtu;   // rad
			}

		// tab5.2d.txt in IERS
#ifdef _MSC_VER
			infile = fopen((EopLoc + "iau00s.dat").c_str(), "r");
#else
			infile = fopen(EopLoc, "r");
#endif
			for (i = 0; i < 66; i++)
			{
#ifdef _MSC_VER
				ret = fscanf_s(infile, "%d %lf %lf %d %d %d %d %d %d %d %d %d %d %d %d %d %d \n ",
					&tmpint, &iau00arr.ass0[i][0], &iau00arr.ass0[i][1],
					&iau00arr.a0si[i][0], &iau00arr.a0si[i][1], &iau00arr.a0si[i][2], &iau00arr.a0si[i][3],
					&iau00arr.a0si[i][4], &iau00arr.a0si[i][5], &iau00arr.a0si[i][6], &iau00arr.a0si[i][7],
					&iau00arr.a0si[i][8], &iau00arr.a0si[i][9], &iau00arr.a0si[i][10], &iau00arr.a0si[i][11],
					&iau00arr.a0si[i][12], &iau00arr.a0si[i][13]);
#else
				ret = fscanf(infile, "%d %lf %lf %d %d %d %d %d %d %d %d %d %d %d %d %d %d \n ",
					&tmpint, &iau00arr.ass0[i][0], &iau00arr.ass0[i][1],
					&iau00arr.a0si[i][0], &iau00arr.a0si[i][1], &iau00arr.a0si[i][2], &iau00arr.a0si[i][3],
					&iau00arr.a0si[i][4], &iau00arr.a0si[i][5], &iau00arr.a0si[i][6], &iau00arr.a0si[i][7],
					&iau00arr.a0si[i][8], &iau00arr.a0si[i][9], &iau00arr.a0si[i][10], &iau00arr.a0si[i][11],
					&iau00arr.a0si[i][12], &iau00arr.a0si[i][13]);
#endif

				for (j = 0; j <= 1; j++)
					iau00arr.ass0[i][j] = iau00arr.ass0[i][j] * convrtu;   // rad
			}

			//     // nutation values old approach iau2000a
#ifdef _MSC_VER
			infile = fopen((EopLoc + "iau00an.dat").c_str(), "r");
#else
			infile = fopen(EopLoc, "r");
#endif
			for (i = 0; i < 678; i++)
			{
#ifdef _MSC_VER
				ret = fscanf_s(infile, "%d %d %d %d %d %lf %lf %lf %lf %lf %lf %lf %lf %lf \n",
					&iau00arr.appni[i][0], &iau00arr.appni[i][1], &iau00arr.appni[i][2], &iau00arr.appni[i][3], &iau00arr.appni[i][4],
                    &tmpdbl, 
					&iau00arr.appn[i][0], &iau00arr.appn[i][1], &iau00arr.appn[i][2], &iau00arr.appn[i][3],
					&iau00arr.appn[i][4], &iau00arr.appn[i][5], &iau00arr.appn[i][6], &iau00arr.appn[i][7] );
#else
				ret = fscanf(infile, "%d %d %d %d %d %lf %lf %lf %lf %lf %lf %lf %lf %lf \n",
					&iau00arr.appni[i][0], &iau00arr.appni[i][1], &iau00arr.appni[i][2], &iau00arr.appni[i][3], &iau00arr.appni[i][4],
					&tmpdbl,
					&iau00arr.appn[i][0], &iau00arr.appn[i][1], &iau00arr.appn[i][2], &iau00arr.appn[i][3],
					&iau00arr.appn[i][4], &iau00arr.appn[i][5], &iau00arr.appn[i][6], &iau00arr.appn[i][7] );
#endif

				for (j = 0; j < 8; j++)
					iau00arr.appn[i][j] = iau00arr.appn[i][j] * convrtm;   // rad
			}			

			//     // planetary nutation values
#ifdef _MSC_VER
			infile = fopen((EopLoc + "iau00apl.dat").c_str(), "r");
#else
			infile = fopen(EopLoc, "r");
#endif
			for (i = 0; i < 687; i++)
			{
#ifdef _MSC_VER
				ret = fscanf_s(infile, "%d %d %d %d %d %d %d %d %d %d %d %d %d %d %d %lf %lf %lf %lf %lf %lf \n",
					&tmpint, 
					&iau00arr.aplni[i][0], &iau00arr.aplni[i][1], &iau00arr.aplni[i][2], &iau00arr.aplni[i][3], &iau00arr.aplni[i][4],
					&iau00arr.aplni[i][5], &iau00arr.aplni[i][6], &iau00arr.aplni[i][7], &iau00arr.aplni[i][8], &iau00arr.aplni[i][9],
					&iau00arr.aplni[i][10], &iau00arr.aplni[i][11], &iau00arr.aplni[i][12], &iau00arr.aplni[i][13],
					&tmpdbl, & iau00arr.apln[i][0], &iau00arr.apln[i][1],
					&iau00arr.apln[i][2], &iau00arr.apln[i][3], &iau00arr.apln[i][4] );
#else
				ret = fscanf(infile, "%d %d %d %d %d %d %d %d %d %d %d %d %d %d %d %lf %lf %lf %lf %lf %lf \n",
					&tmpint,
					&iau00arr.aplni[i][0], &iau00arr.aplni[i][1], &iau00arr.aplni[i][2], &iau00arr.aplni[i][3], &iau00arr.aplni[i][4],
					&iau00arr.aplni[i][5], &iau00arr.aplni[i][6], &iau00arr.aplni[i][7], &iau00arr.aplni[i][8], &iau00arr.aplni[i][9],
					&iau00arr.aplni[i][10], &iau00arr.aplni[i][11], &iau00arr.aplni[i][12], &iau00arr.aplni[i][13],
					&tmpdbl, &iau00arr.apln[i][0], &iau00arr.apln[i][1],
					&iau00arr.apln[i][2], &iau00arr.apln[i][3], &iau00arr.apln[i][4]);
#endif
				for (j = 0; j < 5; j++)
					iau00arr.apln[i][j] = iau00arr.apln[i][j] * convrtm;   // rad
			}


		// tab5.3a.txt in IERS
		// nutation values planetary now included new iau2006
		// nutation in longitude
#ifdef _MSC_VER
			infile = fopen((EopLoc + "iau00nlon.dat").c_str(), "r");
#else
			infile = fopen(EopLoc, "r");
#endif
			for (i = 0; i < 1358; i++)
			{
#ifdef _MSC_VER
				ret = fscanf_s(infile, "%d %lf %lf %d %d %d %d %d %d %d %d %d %d %d %d %d %d \n",
					&tmpint,
					&iau00arr.apn[i][0], &iau00arr.apn[i][1],
					&iau00arr.apni[i][0], &iau00arr.apni[i][1], &iau00arr.apni[i][2], &iau00arr.apni[i][3], &iau00arr.apni[i][4],
					&iau00arr.apni[i][5], &iau00arr.apni[i][6], &iau00arr.apni[i][7], &iau00arr.apni[i][8], &iau00arr.apni[i][9],
					&iau00arr.apni[i][10], &iau00arr.apni[i][11], &iau00arr.apni[i][12], &iau00arr.apni[i][13]);
#else
				ret = fscanf(infile, "%d %lf %lf %d %d %d %d %d %d %d %d %d %d %d %d %d %d \n",
					&tmpint,
					&iau00arr.apn[i][0], &iau00arr.apn[i][1],
					&iau00arr.apni[i][0], &iau00arr.apni[i][1], &iau00arr.apni[i][2], &iau00arr.apni[i][3], &iau00arr.apni[i][4],
					&iau00arr.apni[i][5], &iau00arr.apni[i][6], &iau00arr.apni[i][7], &iau00arr.apni[i][8], &iau00arr.apni[i][9],
					&iau00arr.apni[i][10], &iau00arr.apni[i][11], &iau00arr.apni[i][12], &iau00arr.apni[i][13]);
#endif
				for (j = 0; j < 1; j++)
					iau00arr.apn[i][j] = iau00arr.apn[i][j] * convrtu;   // rad
			}


			// tab5.3b.txt in IERS
		// nutation in obliquity
#ifdef _MSC_VER
			infile = fopen((EopLoc + "iau00nobl.dat").c_str(), "r");
#else
			infile = fopen(EopLoc, "r");
#endif
			for (i = 0; i < 1056; i++)
			{
#ifdef _MSC_VER
				ret = fscanf_s(infile, "%d %lf %lf %d %d %d %d %d %d %d %d %d %d %d %d %d %d \n",
					&tmpint,
					&iau00arr.ape[i][0], &iau00arr.ape[i][1],
					&iau00arr.apei[i][0], &iau00arr.apei[i][1], &iau00arr.apei[i][2], &iau00arr.apei[i][3], &iau00arr.apei[i][4],
					&iau00arr.apei[i][5], &iau00arr.apei[i][6], &iau00arr.apei[i][7], &iau00arr.apei[i][8], &iau00arr.apei[i][9],
					&iau00arr.apei[i][10], &iau00arr.apei[i][11], &iau00arr.apei[i][12], &iau00arr.apei[i][13]);
#else
				ret = fscanf(infile, "%d %lf %lf %d %d %d %d %d %d %d %d %d %d %d %d %d %d \n",
					&tmpint,
					&iau00arr.ape[i][0], &iau00arr.ape[i][1],
					&iau00arr.apei[i][0], &iau00arr.apei[i][1], &iau00arr.apei[i][2], &iau00arr.apei[i][3], &iau00arr.apei[i][4],
					&iau00arr.apei[i][5], &iau00arr.apei[i][6], &iau00arr.apei[i][7], &iau00arr.apei[i][8], &iau00arr.apei[i][9],
					&iau00arr.apei[i][10], &iau00arr.apei[i][11], &iau00arr.apei[i][12], &iau00arr.apei[i][13]);
#endif
				for (j = 0; j < 1; j++)
					iau00arr.ape[i][j] = iau00arr.ape[i][j] * convrtu;   // rad
			}


		// tab5.2e.txt in IERS
		// gmst values
		// note - these are very similar to the first 34 elements of iau00s.dat,
		// but they are not the same.
#ifdef _MSC_VER
			infile = fopen((EopLoc + "iau00gst.dat").c_str(), "r");
#else
			infile = fopen(EopLoc, "r");
#endif
			for (i = 0; i < 34; i++)
			{
#ifdef _MSC_VER
				ret = fscanf_s(infile, "%d %lf %lf %d %d %d %d %d %d %d %d %d %d %d %d %d %d \n",
					&tmpint,
					&iau00arr.agst[i][0], &iau00arr.agst[i][1],
					&iau00arr.agsti[i][0], &iau00arr.agsti[i][1], &iau00arr.agsti[i][2], &iau00arr.agsti[i][3], &iau00arr.agsti[i][4],
					&iau00arr.agsti[i][5], &iau00arr.agsti[i][6], &iau00arr.agsti[i][7], &iau00arr.agsti[i][8], &iau00arr.agsti[i][9],
					&iau00arr.agsti[i][10], &iau00arr.agsti[i][11], &iau00arr.agsti[i][12], &iau00arr.agsti[i][13]);
#else
				ret = fscanf(infile, "%d %lf %lf %d %d %d %d %d %d %d %d %d %d %d %d %d %d \n",
					&tmpint,
					&iau00arr.agst[i][0], &iau00arr.agst[i][1],
					&iau00arr.agsti[i][0], &iau00arr.agsti[i][1], &iau00arr.agsti[i][2], &iau00arr.agsti[i][3], &iau00arr.agsti[i][4],
					&iau00arr.agsti[i][5], &iau00arr.agsti[i][6], &iau00arr.agsti[i][7], &iau00arr.agsti[i][8], &iau00arr.agsti[i][9],
					&iau00arr.agsti[i][10], &iau00arr.agsti[i][11], &iau00arr.agsti[i][12], &iau00arr.agsti[i][13]);
#endif
				for (j = 0; j <= 1; j++)
					iau00arr.agst[i][j] = iau00arr.agst[i][j] * convrtu;   // rad
			}
			
	}    // iau00in


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
         *    planetary longitudes                                          rad
         *
         *  locals        :
         *
         *  coupling      :
         *    none        -
         *
         *  references    :
         *    vallado       2013, 210-211, 225
         * --------------------------------------------------------------------------- */

	void fundarg
	(
		double ttt, eOpt opt,
		double& l, double& l1, double& f, double& d, double& omega,
		double& lonmer, double& lonven, double& lonear, double& lonmar,
		double& lonjup, double& lonsat, double& lonurn, double& lonnep,
		double& precrate
	)
	{
		double deg2rad, oo3600;
		char iauhelp;

		sethelp(iauhelp, ' ');
		deg2rad = pi / 180.0;
		oo3600 = 1.0 / 3600.0;
                l = l1 = f = d = omega = lonmer = lonven = lonear = lonmar = lonjup = lonsat = lonurn = lonnep = precrate = 0.0;

            // ---- determine coefficients for various iers nutation theories ----
		// ----  iau-2010 cio theory and iau-2000a theory
		if (opt == e00cio || opt == e00a)
		{
                // ------ form the delaunay fundamental arguments in ", converted to rad
			l = ((((-0.00024470 * ttt + 0.051635) * ttt + 31.8792) * ttt + 1717915923.2178) * ttt + 485868.249036) * oo3600;
			l1 = ((((-0.00001149 * ttt + 0.000136) * ttt - 0.5532) * ttt + 129596581.0481) * ttt + 1287104.793048) * oo3600;
			f = ((((+0.00000417 * ttt - 0.001037) * ttt - 12.7512) * ttt + 1739527262.8478) * ttt + 335779.526232) * oo3600;
			d = ((((-0.00003169 * ttt + 0.006593) * ttt - 6.3706) * ttt + 1602961601.2090) * ttt + 1072260.703692) * oo3600;
			omega = ((((-0.00005939 * ttt + 0.007702) * ttt + 7.4722) * ttt - 6962890.5431) * ttt + 450160.398036) * oo3600;

                // ------ form the planetary arguments in ", converted to rad
			lonmer = (908103.259872 + 538101628.688982  * ttt) * oo3600;
			lonven = (655127.283060 + 210664136.433548  * ttt) * oo3600;
			lonear = (361679.244588 + 129597742.283429  * ttt) * oo3600;
			lonmar = (1279558.798488 + 68905077.493988  * ttt) * oo3600;
			lonjup = (123665.467464 + 10925660.377991  * ttt) * oo3600;
			lonsat = (180278.799480 + 4399609.855732  * ttt) * oo3600;
			lonurn = (1130598.018396 + 1542481.193933  * ttt) * oo3600;
			lonnep = (1095655.195728 + 786550.320744  * ttt) * oo3600;
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
		if (opt == e00b)
		{
			// ------ form the delaunay fundamental arguments in deg
			l = (1717915923.2178  * ttt + 485868.249036) * oo3600;
			l1 = (129596581.0481  * ttt + 1287104.79305) * oo3600;
			f = (1739527262.8478  * ttt + 335779.526232) * oo3600;
			d = (1602961601.2090  * ttt + 1072260.70369) * oo3600;
			omega = (-6962890.5431  * ttt + 450160.398036) * oo3600;

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
		if (opt == e96)
		{
			// ------ form the delaunay fundamental arguments in deg
			l = ((((-0.00024470 * ttt + 0.051635) * ttt + 31.8792) * ttt + 1717915923.2178) * ttt) * oo3600 + 134.96340251;
			l1 = ((((-0.00001149 * ttt - 0.000136) * ttt - 0.5532) * ttt + 129596581.0481) * ttt) * oo3600 + 357.52910918;
			f = ((((+0.00000417 * ttt + 0.001037) * ttt - 12.7512) * ttt + 1739527262.8478) * ttt) * oo3600 + 93.27209062;
			d = ((((-0.00003169 * ttt + 0.006593) * ttt - 6.3706) * ttt + 1602961601.2090) * ttt) * oo3600 + 297.85019547;
			omega = ((((-0.00005939 * ttt + 0.007702) * ttt + 7.4722) * ttt - 6962890.2665) * ttt) * oo3600 + 125.04455501;
			// ------ form the planetary arguments in deg
			lonmer = 0.0;
			lonven = 181.979800853 + 58517.8156748   * ttt;
			lonear = 100.466448494 + 35999.3728521   * ttt;
			lonmar = 355.433274605 + 19140.299314    * ttt;
			lonjup = 34.351483900 + 3034.90567464  * ttt;
			lonsat = 50.0774713998 + 1222.11379404  * ttt;
			lonurn = 0.0;
			lonnep = 0.0;
			precrate = (0.0003086 * ttt + 1.39697137214) * ttt;
		}

		// ---- iau-1980 theory
		if (opt == e80)
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
			// circ 163 shows no planetary
			// ???????
			lonmer = 252.3 + 149472.0  * ttt;
			lonven = 179.9 + 58517.8  * ttt;
			lonear = 98.4 + 35999.4  * ttt;
			lonmar = 353.3 + 19140.3  * ttt;
			lonjup = 32.3 + 3034.9  * ttt;
			lonsat = 48.0 + 1222.1  * ttt;
			lonurn = 0.0;
			lonnep = 0.0;
			precrate = 0.0;
		}

		// ---- convert units from deg to rad
		l = fmod(l, 360.0)      *  deg2rad;
		l1 = fmod(l1, 360.0)     *  deg2rad;
		f = fmod(f, 360.0)      *  deg2rad;
		d = fmod(d, 360.0)      *  deg2rad;
		omega = fmod(omega, 360.0)  *  deg2rad;

		lonmer = fmod(lonmer, 360.0) * deg2rad;
		lonven = fmod(lonven, 360.0) * deg2rad;
		lonear = fmod(lonear, 360.0) * deg2rad;
		lonmar = fmod(lonmar, 360.0) * deg2rad;
		lonjup = fmod(lonjup, 360.0) * deg2rad;
		lonsat = fmod(lonsat, 360.0) * deg2rad;
		lonurn = fmod(lonurn, 360.0) * deg2rad;
		lonnep = fmod(lonnep, 360.0) * deg2rad;
		precrate = fmod(precrate, 360.0) * deg2rad;

		if (iauhelp == 'y')
		{
			printf("fa %11.7f  %11.7f  %11.7f  %11.7f  %11.7f deg \n", l * 180 / pi, l1 * 180 / pi, f * 180 / pi, d * 180 / pi, omega * 180 / pi);
			printf("fa %11.7f  %11.7f  %11.7f  %11.7f deg \n", lonmer * 180 / pi, lonven * 180 / pi, lonear * 180 / pi, lonmar * 180 / pi);
			printf("fa %11.7f  %11.7f  %11.7f  %11.7f deg \n", lonjup * 180 / pi, lonsat * 180 / pi, lonurn * 180 / pi, lonnep * 180 / pi);
			printf("fa %11.7f  \n", precrate * 180 / pi);
		}
	}  // procedure fundarg


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

	void iau00xys
	(
		double ttt, double ddx, double ddy, eOpt opt,
		const iau00data &iau00arr,
		double& x, double& y, double& s,
		std::vector< std::vector<double> > &nut
	)
	{
		nut.resize(3);  // rows
		for (std::vector< std::vector<double> >::iterator it = nut.begin(); it != nut.end(); ++it)
			it->resize(3);
		double ttt2, ttt3, ttt4, ttt5;
		int i, j;
		std::vector< std::vector<double> > nut1 = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > nut2 = std::vector< std::vector<double> >(3, std::vector<double>(3));
		double a, sum0, sum1, sum2, sum3, sum4;
		double l, l1, f, d, omega, lonmer, lonven, lonear, lonmar, lonjup, lonsat, lonurn, lonnep, precrate, tempval;

		// " to rad
		double convrt = pi / (180.0 * 3600.0);

		ttt2 = ttt * ttt;
		ttt3 = ttt2 * ttt;
		ttt4 = ttt2 * ttt2;
		ttt5 = ttt3 * ttt2;

		//iau00in(nutLoc, iau00arr);

		fundarg(ttt, opt, l, l1, f, d, omega, lonmer, lonven, lonear, lonmar, lonjup, lonsat, lonurn, lonnep, precrate);

		// ---------------- find x
		// the iers code puts the constants in here, however
		// don't sum constants in here because they're larger than the last few terms
		sum0 = 0.0;
		for (i = 1305; i >= 0; i--)
		{
			tempval = iau00arr.a0xi[i][0] * l + iau00arr.a0xi[i][1] * l1 + iau00arr.a0xi[i][2] * f + iau00arr.a0xi[i][3] * d + iau00arr.a0xi[i][4] * omega +
				iau00arr.a0xi[i][5] * lonmer + iau00arr.a0xi[i][6] * lonven + iau00arr.a0xi[i][7] * lonear + iau00arr.a0xi[i][8] * lonmar +
				iau00arr.a0xi[i][9] * lonjup + iau00arr.a0xi[i][10] * lonsat + iau00arr.a0xi[i][11] * lonurn + iau00arr.a0xi[i][12] * lonnep +
				iau00arr.a0xi[i][13] * precrate;
			sum0 = sum0 + iau00arr.axs0[i][0] * sin(tempval) + iau00arr.axs0[i][1] * cos(tempval);
		}
		sum1 = 0.0;
		// note that the index changes here to j. this is because the a0xi etc
		// indicies go from 1 to 1600, but there are 5 groups.the i index counts through each
		// calculation, and j takes care of the individual summations. note that
		// this same process is used for y and s.
		for (j = 252; j >= 0; j--)
		{
			i = 1306 + j;
			tempval = iau00arr.a0xi[i][0] * l + iau00arr.a0xi[i][1] * l1 + iau00arr.a0xi[i][2] * f + iau00arr.a0xi[i][3] * d + iau00arr.a0xi[i][4] * omega +
				iau00arr.a0xi[i][5] * lonmer + iau00arr.a0xi[i][6] * lonven + iau00arr.a0xi[i][7] * lonear + iau00arr.a0xi[i][8] * lonmar +
				iau00arr.a0xi[i][9] * lonjup + iau00arr.a0xi[i][10] * lonsat + iau00arr.a0xi[i][11] * lonurn + iau00arr.a0xi[i][12] *
				lonnep + iau00arr.a0xi[i][13] * precrate;
			sum1 = sum1 + iau00arr.axs0[i][0] * sin(tempval) + iau00arr.axs0[i][1] * cos(tempval);
		}
		sum2 = 0.0;
		for (j = 35; j >= 0; j--)
		{
			i = 1306 + 253 + j;
			tempval = iau00arr.a0xi[i][0] * l + iau00arr.a0xi[i][1] * l1 + iau00arr.a0xi[i][2] * f + iau00arr.a0xi[i][3] * d + iau00arr.a0xi[i][4] * omega +
				iau00arr.a0xi[i][5] * lonmer + iau00arr.a0xi[i][6] * lonven + iau00arr.a0xi[i][7] * lonear + iau00arr.a0xi[i][8] * lonmar +
				iau00arr.a0xi[i][9] * lonjup + iau00arr.a0xi[i][10] * lonsat + iau00arr.a0xi[i][11] * lonurn + iau00arr.a0xi[i][12] *
				lonnep + iau00arr.a0xi[i][13] * precrate;
			sum2 = sum2 + iau00arr.axs0[i][0] * sin(tempval) + iau00arr.axs0[i][1] * cos(tempval);
		}
		sum3 = 0.0;
		for (j = 3; j >= 0; j--)
		{
			i = 1306 + 253 + 36 + j;
			tempval = iau00arr.a0xi[i][0] * l + iau00arr.a0xi[i][1] * l1 + iau00arr.a0xi[i][2] * f + iau00arr.a0xi[i][3] * d + iau00arr.a0xi[i][4] * omega +
				iau00arr.a0xi[i][5] * lonmer + iau00arr.a0xi[i][6] * lonven + iau00arr.a0xi[i][7] * lonear + iau00arr.a0xi[i][8] * lonmar +
				iau00arr.a0xi[i][9] * lonjup + iau00arr.a0xi[i][10] * lonsat + iau00arr.a0xi[i][11] * lonurn + iau00arr.a0xi[i][12] *
				lonnep + iau00arr.a0xi[i][13] * precrate;
			sum3 = sum3 + iau00arr.axs0[i][0] * sin(tempval) + iau00arr.axs0[i][1] * cos(tempval);
		}
		sum4 = 0.0;
		for (j = 0; j >= 0; j--)
		{
			i = 1306 + 253 + 36 + 4 + j;
			tempval = iau00arr.a0xi[i][0] * l + iau00arr.a0xi[i][1] * l1 + iau00arr.a0xi[i][2] * f + iau00arr.a0xi[i][3] * d + iau00arr.a0xi[i][4] * omega +
				iau00arr.a0xi[i][5] * lonmer + iau00arr.a0xi[i][6] * lonven + iau00arr.a0xi[i][7] * lonear + iau00arr.a0xi[i][8] * lonmar +
				iau00arr.a0xi[i][9] * lonjup + iau00arr.a0xi[i][10] * lonsat + iau00arr.a0xi[i][11] * lonurn + iau00arr.a0xi[i][12] *
				lonnep + iau00arr.a0xi[i][13] * precrate;
			sum4 = sum4 + iau00arr.axs0[i][0] * sin(tempval) + iau00arr.axs0[i][1] * cos(tempval);
		}

		x = -0.016617 + 2004.191898 * ttt - 0.4297829 * ttt2
			- 0.19861834 * ttt3 - 0.000007578 * ttt4 + 0.0000059285 * ttt5; // "
		x = x * convrt + sum0 + sum1 * ttt + sum2 * ttt2 + sum3 * ttt3 + sum4 * ttt4;  // rad

		// ---------------- now find y
		sum0 = 0.0;
		for (i = 961; i >= 0; i--)
		{
			tempval = iau00arr.a0yi[i][0] * l + iau00arr.a0yi[i][1] * l1 + iau00arr.a0yi[i][2] * f + iau00arr.a0yi[i][3] * d + iau00arr.a0yi[i][4] * omega +
				iau00arr.a0yi[i][5] * lonmer + iau00arr.a0yi[i][6] * lonven + iau00arr.a0yi[i][7] * lonear + iau00arr.a0yi[i][8] * lonmar +
				iau00arr.a0yi[i][9] * lonjup + iau00arr.a0yi[i][10] * lonsat + iau00arr.a0yi[i][11] * lonurn + iau00arr.a0yi[i][12] *
				lonnep + iau00arr.a0yi[i][13] * precrate;
			sum0 = sum0 + iau00arr.ays0[i][0] * sin(tempval) + iau00arr.ays0[i][1] * cos(tempval);
		}

		sum1 = 0.0;
		for (j = 276; j >= 0; j--)
		{
			i = 962 + j;
			tempval = iau00arr.a0yi[i][0] * l + iau00arr.a0yi[i][1] * l1 + iau00arr.a0yi[i][2] * f + iau00arr.a0yi[i][3] * d + iau00arr.a0yi[i][4] * omega +
				iau00arr.a0yi[i][5] * lonmer + iau00arr.a0yi[i][6] * lonven + iau00arr.a0yi[i][7] * lonear + iau00arr.a0yi[i][8] * lonmar +
				iau00arr.a0yi[i][9] * lonjup + iau00arr.a0yi[i][10] * lonsat + iau00arr.a0yi[i][11] * lonurn + iau00arr.a0yi[i][12] *
				lonnep + iau00arr.a0yi[i][13] * precrate;
			sum1 = sum1 + iau00arr.ays0[i][0] * sin(tempval) + iau00arr.ays0[i][1] * cos(tempval);
		}
		sum2 = 0.0;
		for (j = 29; j >= 0; j--)
		{
			i = 962 + 277 + j;
			tempval = iau00arr.a0yi[i][0] * l + iau00arr.a0yi[i][1] * l1 + iau00arr.a0yi[i][2] * f + iau00arr.a0yi[i][3] * d + iau00arr.a0yi[i][4] * omega +
				iau00arr.a0yi[i][5] * lonmer + iau00arr.a0yi[i][6] * lonven + iau00arr.a0yi[i][7] * lonear + iau00arr.a0yi[i][8] * lonmar +
				iau00arr.a0yi[i][9] * lonjup + iau00arr.a0yi[i][10] * lonsat + iau00arr.a0yi[i][11] * lonurn + iau00arr.a0yi[i][12] *
				lonnep + iau00arr.a0yi[i][13] * precrate;
			sum2 = sum2 + iau00arr.ays0[i][0] * sin(tempval) + iau00arr.ays0[i][1] * cos(tempval);
		}
		sum3 = 0.0;
		for (j = 4; j >= 0; j--)
		{
			i = 962 + 277 + 30 + j;
			tempval = iau00arr.a0yi[i][0] * l + iau00arr.a0yi[i][1] * l1 + iau00arr.a0yi[i][2] * f + iau00arr.a0yi[i][3] * d + iau00arr.a0yi[i][4] * omega +
				iau00arr.a0yi[i][5] * lonmer + iau00arr.a0yi[i][6] * lonven + iau00arr.a0yi[i][7] * lonear + iau00arr.a0yi[i][8] * lonmar +
				iau00arr.a0yi[i][9] * lonjup + iau00arr.a0yi[i][10] * lonsat + iau00arr.a0yi[i][11] * lonurn + iau00arr.a0yi[i][12] *
				lonnep + iau00arr.a0yi[i][13] * precrate;
			sum3 = sum3 + iau00arr.ays0[i][0] * sin(tempval) + iau00arr.ays0[i][1] * cos(tempval);
		}
		sum4 = 0.0;
		for (j = 0; j >= 0; j--)
		{
			i = 962 + 277 + 30 + 5 + j;
			tempval = iau00arr.a0yi[i][0] * l + iau00arr.a0yi[i][1] * l1 + iau00arr.a0yi[i][2] * f + iau00arr.a0yi[i][3] * d + iau00arr.a0yi[i][4] * omega +
				iau00arr.a0yi[i][5] * lonmer + iau00arr.a0yi[i][6] * lonven + iau00arr.a0yi[i][7] * lonear + iau00arr.a0yi[i][8] * lonmar +
				iau00arr.a0yi[i][9] * lonjup + iau00arr.a0yi[i][10] * lonsat + iau00arr.a0yi[i][11] * lonurn + iau00arr.a0yi[i][12] *
				lonnep + iau00arr.a0yi[i][13] * precrate;
			sum4 = sum4 + iau00arr.ays0[i][0] * sin(tempval) + iau00arr.ays0[i][1] * cos(tempval);
		}

		y = -0.006951 - 0.025896 * ttt - 22.4072747 * ttt2
			+ 0.00190059 * ttt3 + 0.001112526 * ttt4 + 0.0000001358 * ttt5;  // "
		y = y * convrt + sum0 + sum1 * ttt + sum2 * ttt2 + sum3 * ttt3 + sum4 * ttt4;  // rad


		// ---------------- now find s
		sum0 = 0.0;
		for (i = 32; i >= 0; i--)
		{
			tempval = iau00arr.a0si[i][0] * l + iau00arr.a0si[i][1] * l1 + iau00arr.a0si[i][2] * f + iau00arr.a0si[i][3] * d + iau00arr.a0si[i][4] * omega +
				iau00arr.a0si[i][5] * lonmer + iau00arr.a0si[i][6] * lonven + iau00arr.a0si[i][7] * lonear + iau00arr.a0si[i][8] * lonmar +
				iau00arr.a0si[i][9] * lonjup + iau00arr.a0si[i][10] * lonsat + iau00arr.a0si[i][11] * lonurn + iau00arr.a0si[i][12] *
				lonnep + iau00arr.a0si[i][13] * precrate;
			sum0 = sum0 + iau00arr.ass0[i][0] * sin(tempval) + iau00arr.ass0[i][1] * cos(tempval);
		}
		sum1 = 0.0;
		for (j = 2; j >= 0; j--)
		{
			i = 33 + j;
			tempval = iau00arr.a0si[i][0] * l + iau00arr.a0si[i][1] * l1 + iau00arr.a0si[i][2] * f + iau00arr.a0si[i][3] * d + iau00arr.a0si[i][4] * omega +
				iau00arr.a0si[i][5] * lonmer + iau00arr.a0si[i][6] * lonven + iau00arr.a0si[i][7] * lonear + iau00arr.a0si[i][8] * lonmar +
				iau00arr.a0si[i][9] * lonjup + iau00arr.a0si[i][10] * lonsat + iau00arr.a0si[i][11] * lonurn + iau00arr.a0si[i][12] *
				lonnep + iau00arr.a0si[i][13] * precrate;
			sum1 = sum1 + iau00arr.ass0[i][0] * sin(tempval) + iau00arr.ass0[i][1] * cos(tempval);
		}
		sum2 = 0.0;
		for (j = 24; j >= 0; j--)
		{
			i = 33 + 3 + j;
			tempval = iau00arr.a0si[i][0] * l + iau00arr.a0si[i][1] * l1 + iau00arr.a0si[i][2] * f + iau00arr.a0si[i][3] * d + iau00arr.a0si[i][4] * omega +
				iau00arr.a0si[i][5] * lonmer + iau00arr.a0si[i][6] * lonven + iau00arr.a0si[i][7] * lonear + iau00arr.a0si[i][8] * lonmar +
				iau00arr.a0si[i][9] * lonjup + iau00arr.a0si[i][10] * lonsat + iau00arr.a0si[i][11] * lonurn + iau00arr.a0si[i][12] *
				lonnep + iau00arr.a0si[i][13] * precrate;
			sum2 = sum2 + iau00arr.ass0[i][0] * sin(tempval) + iau00arr.ass0[i][1] * cos(tempval);
		}
		sum3 = 0.0;
		for (j = 3; j >= 0; j--)
		{
			i = 33 + 3 + 25 + j;
			tempval = iau00arr.a0si[i][0] * l + iau00arr.a0si[i][1] * l1 + iau00arr.a0si[i][2] * f + iau00arr.a0si[i][3] * d + iau00arr.a0si[i][4] * omega +
				iau00arr.a0si[i][5] * lonmer + iau00arr.a0si[i][6] * lonven + iau00arr.a0si[i][7] * lonear + iau00arr.a0si[i][8] * lonmar +
				iau00arr.a0si[i][9] * lonjup + iau00arr.a0si[i][10] * lonsat + iau00arr.a0si[i][11] * lonurn + iau00arr.a0si[i][12] *
				lonnep + iau00arr.a0si[i][13] * precrate;
			sum3 = sum3 + iau00arr.ass0[i][0] * sin(tempval) + iau00arr.ass0[i][1] * cos(tempval);
		}
		sum4 = 0.0;
		for (j = 0; j >= 0; j--)
		{
			i = 33 + 3 + 25 + 4 + j;
			tempval = iau00arr.a0si[i][0] * l + iau00arr.a0si[i][1] * l1 + iau00arr.a0si[i][2] * f + iau00arr.a0si[i][3] * d + iau00arr.a0si[i][4] * omega +
				iau00arr.a0si[i][5] * lonmer + iau00arr.a0si[i][6] * lonven + iau00arr.a0si[i][7] * lonear + iau00arr.a0si[i][8] * lonmar +
				iau00arr.a0si[i][9] * lonjup + iau00arr.a0si[i][10] * lonsat + iau00arr.a0si[i][11] * lonurn + iau00arr.a0si[i][12] *
				lonnep + iau00arr.a0si[i][13] * precrate;
			sum4 = sum4 + iau00arr.ass0[i][0] * sin(tempval) + iau00arr.ass0[i][1] * cos(tempval);
		}

		s = 0.000094 + 0.00380865 * ttt - 0.00012268 * ttt2
			- 0.07257411 * ttt3 + 0.00002798 * ttt4 + 0.00001562 * ttt5;   // "
		//            + 0.00000171*ttt* sin(omega) + 0.00000357*ttt* cos(2.0*omega)  
		//            + 0.00074353*ttt2* sin(omega) + 0.00005691*ttt2* sin(2.0*(f-d+omega))  
		//            + 0.00000984*ttt2* sin(2.0*(f+omega)) - 0.00000885*ttt2* sin(2.0*omega);
		s = -x * y * 0.5 + s * convrt + sum0 + sum1 * ttt + sum2 * ttt2 + sum3 * ttt3 + sum4 * ttt4;  // rad


		// add corrections if available
		x = x + ddx;
		y = y + ddy;

		// ---------------- now find a
		a = 0.5 + 0.125 * (x * x + y * y); // units take on whatever x and y are

		// ----------------- find nutation matrix ----------------------
		nut1[0][0] = 1.0 - a * x * x;
		nut1[0][1] = -a * x * y;
		nut1[0][2] = x;
		nut1[1][0] = -a * x * y;
		nut1[1][1] = 1.0 - a * y * y;
		nut1[1][2] = y;
		nut1[2][0] = -x;
		nut1[2][1] = -y;
		nut1[2][2] = 1.0 - a * (x * x + y * y);

		nut2[2][2] = 1.0;
		nut2[0][0] = cos(s);
		nut2[1][1] = cos(s);
		nut2[0][1] = sin(s);
		nut2[1][0] = -sin(s);

		MathTimeLib::matmult(nut1, nut2, nut, 3, 3, 3);

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

	void iau00pn
	(
		double ttt, double ddx, double ddy, eOpt opt,
		iau00data& iau00arr,
		double x, double y, double s,
		std::vector< std::vector<double> > &nut
	)
	{
		nut.resize(3);  // rows
		for (std::vector< std::vector<double> >::iterator it = nut.begin(); it != nut.end(); ++it)
			it->resize(3);
		double ttt2, ttt3, ttt4, ttt5;
		std::vector< std::vector<double> > nut1 = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > nut2 = std::vector< std::vector<double> >(3, std::vector<double>(3));
		double a;
		double l, l1, f, d, omega, lonmer, lonven, lonear, lonmar, lonjup, lonsat, lonurn, lonnep, precrate;

		ttt2 = ttt * ttt;
		ttt3 = ttt2 * ttt;
		ttt4 = ttt2 * ttt2;
		ttt5 = ttt3 * ttt2;

		char nutLoc[100] = "D:/Codes/LIBRARY/cpp/TestAll/";
		iau00in(nutLoc, iau00arr);

		fundarg(ttt, opt, l, l1, f, d, omega, lonmer, lonven, lonear, lonmar, lonjup, lonsat, lonurn, lonnep, precrate);

		// ---------------- find x

		// add corrections if available
		x = x + ddx;
		y = y + ddy;

		// ---------------- now find a
		a = 0.5 + 0.125 * (x * x + y * y); // units take on whatever x and y are

		// ----------------- find nutation matrix ----------------------
		nut1[0][0] = 1.0 - a * x * x;
		nut1[0][1] = -a * x * y;
		nut1[0][2] = x;
		nut1[1][0] = -a * x * y;
		nut1[1][1] = 1.0 - a * y * y;
		nut1[1][2] = y;
		nut1[2][0] = -x;
		nut1[2][1] = -y;
		nut1[2][2] = 1.0 - a * (x * x + y * y);

		nut2[2][2] = 1.0;
		nut2[0][0] = cos(s);
		nut2[1][1] = cos(s);
		nut2[0][1] = sin(s);
		nut2[1][0] = -sin(s);

		MathTimeLib::matmult(nut1, nut2, nut, 3, 3, 3);

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

	void gstime00
	(
		double jdut1, double deltapsi, double ttt, const iau00data &iau00arr, eOpt opt, double& gst,
		std::vector< std::vector<double> > &st
	)
	{
		st.resize(3);  // rows
		for (std::vector< std::vector<double> >::iterator it = st.begin(); it != st.end(); ++it)
			it->resize(3);
		const double deg2rad = pi / 180.0;
		double convrt, ttt2, ttt3, ttt4, ttt5, epsa, tempval, gstsum0, gstsum1;
		double l, l1, f, d, lonmer, lonven, lonear, lonmar, lonjup, lonsat, lonurn, lonnep, precrate, raan,
			eect2000, ee2000, tut1d, era, gmst2000;
		int i, j;

		// " to rad
		convrt = pi / (180.0 * 3600.0);

		ttt2 = ttt * ttt;
		ttt3 = ttt2 * ttt;
		ttt4 = ttt2 * ttt2;
		ttt5 = ttt3 * ttt2;

		// mean obliquity of the ecliptic
		// see sofa code obl06.f (no iau_ in front)
		epsa = 84381.406 - 46.836769 * ttt - 0.0001831 * ttt2 + 0.00200340 * ttt3 - 0.000000576 * ttt4 - 0.0000000434 * ttt5; // "
		epsa = fmod(epsa / 3600.0, 360.0);  // deg

		epsa = epsa * deg2rad; // rad

		fundarg(ttt, opt, l, l1, f, d, raan, lonmer, lonven, lonear, lonmar, lonjup, lonsat, lonurn, lonnep, precrate);

		//  evaluate the ee complementary terms
		gstsum0 = 0.0;
		// data file is not reversed
		for (i = 32; i >= 0; i--)
		{
			tempval = iau00arr.agsti[i][0] * l + iau00arr.agsti[i][1] * l1 + iau00arr.agsti[i][2] * f + iau00arr.agsti[i][3] * d + iau00arr.agsti[i][4] * raan +
				iau00arr.agsti[i][5] * lonmer + iau00arr.agsti[i][6] * lonven + iau00arr.agsti[i][7] * lonear + iau00arr.agsti[i][8] * lonmar +
				iau00arr.agsti[i][9] * lonjup + iau00arr.agsti[i][10] * lonsat + iau00arr.agsti[i][11] * lonurn + iau00arr.agsti[i][12] * lonnep + iau00arr.agsti[i][13] * precrate;
			gstsum0 = gstsum0 + iau00arr.agst[i][0] * sin(tempval) + iau00arr.agst[i][1] * cos(tempval);
		}

		gstsum1 = 0.0;
		// data file is not reversed
		for (j = 0; j >= 0; j--)
		{
			i = 32 + j;
			tempval = iau00arr.agsti[i][0] * l + iau00arr.agsti[i][1] * l1 + iau00arr.agsti[i][2] * f + iau00arr.agsti[i][3] * d + iau00arr.agsti[i][4] * raan +
				iau00arr.agsti[i][5] * lonmer + iau00arr.agsti[i][6] * lonven + iau00arr.agsti[i][7] * lonear + iau00arr.agsti[i][8] * lonmar +
				iau00arr.agsti[i][9] * lonjup + iau00arr.agsti[i][10] * lonsat + iau00arr.agsti[i][11] * lonurn + iau00arr.agsti[i][12] * lonnep + iau00arr.agsti[i][13] * precrate;
			gstsum1 = gstsum1 + (iau00arr.agst[i][0] * sin(tempval) + iau00arr.agst[i][1] * cos(tempval)) * ttt;
		}

		eect2000 = gstsum0 + gstsum1 * ttt;  // rad

		// equation of the equinoxes
		ee2000 = deltapsi * cos(epsa) + eect2000;  // rad

		//  earth rotation angle
		tut1d = jdut1 - 2451545.0;
		era = twopi * (0.7790572732640 + 1.00273781191135448 * tut1d);
		era = fmod(era, 2.0 * pi);

		//  greenwich mean sidereal time, iau 2000.
		gmst2000 = era + (0.014506 + 4612.156534 * ttt + 1.3915817 * ttt2
			- 0.00000044 * ttt3 + 0.000029956 * ttt4 + 0.0000000368 * ttt5) * convrt; // " to rad

		gst = gmst2000 + ee2000; // rad

		st[0][0] = cos(gst);
		st[0][1] = -sin(gst);
		st[0][2] = 0.0;
		st[1][0] = sin(gst);
		st[1][1] = cos(gst);
		st[1][2] = 0.0;
		st[2][0] = 0.0;
		st[2][1] = 0.0;
		st[2][2] = 1.0;
	}  // gstime00 

		/* -----------------------------------------------------------------------------
	*
	*                           function gstime
	*
	*  this function finds the greenwich sidereal time (iau-82).
	*
	*  author        : david vallado                  719-573-2600    1 mar 2001
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
	*    vallado       2013, 187, eq 3-45
	* --------------------------------------------------------------------------- */

	double  gstime
	(
		double jdut1
	)
	{
		const double deg2rad = pi / 180.0;
		double       temp, tut1;

		tut1 = (jdut1 - 2451545.0) / 36525.0;
		temp = -6.2e-6* tut1 * tut1 * tut1 + 0.093104 * tut1 * tut1 +
			(876600.0 * 3600 + 8640184.812866) * tut1 + 67310.54841;  // sec
		temp = fmod(temp * deg2rad / 240.0, twopi); //360/86400 = 1/240, to deg, to rad

		// ------------------------ check quadrants ---------------------
		if (temp < 0.0)
			temp = temp + twopi;

		return temp;
	}




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
	*    vallado       2013, 188, alg 15, ex 3-5
	* --------------------------------------------------------------------------- */

	void    lstime
	(
		double lon, double jdut1, double& lst, double& gst
	)
	{
		gst = gstime(jdut1);
		lst = lon + gst;

		/* ------------------------ check quadrants --------------------- */
		lst = fmod(lst, twopi);
		if (lst < 0.0)
			lst = lst + twopi;
	}


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
*
*  inputs          description                    range / units
*    ttt         - julian centuries of tt
*    opt         - method option                  e00a, e00b, e96, e80
*
*  outputs       :
*    prec        - transformation matrix for mod - j2000 (80 only)
*    psia        - cannonical precession angle    rad    (00 only)
*    wa          - cannonical precession angle    rad    (00 only)
*    epsa        - cannonical precession angle    rad    (00 only)
*    chia        - cannonical precession angle    rad    (00 only)
*    prec        - matrix converting from "mod" to gcrf
*
*  locals        :
*    convrt      - conversion factor arcsec to radians
*    zeta        - precession angle               rad
*    z           - precession angle               rad
*    theta       - precession angle               rad
*    oblo        - obliquity value at j2000 epoch "//
*
*  coupling      :
*    none        -
*
*  references    :
*    vallado       2013, 213, 226
* --------------------------------------------------------------------------- */

	void precess
	(
		double ttt, eOpt opt,
		double& psia, double& wa, double& epsa, double& chia,
		std::vector< std::vector<double> > &prec
	)
	{
		prec.resize(3);  // rows
		for (std::vector< std::vector<double> >::iterator it = prec.begin(); it != prec.end(); ++it)
			it->resize(3);
		std::vector< std::vector<double> > p1, p2, p3, p4, tr1, tr2;
		double convrt, zeta, theta, z, coszeta, sinzeta, costheta, sintheta,
			cosz, sinz, oblo;
		char iauhelp;
		sethelp(iauhelp, ' ');

		convrt = pi / (180.0 * 3600.0);

		// ------------------- iau 77 precession angles --------------------
		if ((opt == e80) | (opt == e96))
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
		// ------------------ iau 00 precession angles -------------------
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

		if ((opt == e80) | (opt == e96))
		{
			coszeta = cos(zeta);
			sinzeta = sin(zeta);
			costheta = cos(theta);
			sintheta = sin(theta);
			cosz = cos(z);
			sinz = sin(z);

			// ----------------- form matrix  mod to gcrf ------------------
			prec[0][0] = coszeta * costheta * cosz - sinzeta * sinz;
			prec[0][1] = coszeta * costheta * sinz + sinzeta * cosz;
			prec[0][2] = coszeta * sintheta;
			prec[1][0] = -sinzeta * costheta * cosz - coszeta * sinz;
			prec[1][1] = -sinzeta * costheta * sinz + coszeta * cosz;
			prec[1][2] = -sinzeta * sintheta;
			prec[2][0] = -sintheta * cosz;
			prec[2][1] = -sintheta * sinz;
			prec[2][2] = costheta;

			// alternate approach
			//MathTimeLib::rot3mat(z, p1);
			//MathTimeLib::rot2mat(-theta, p2);
			//MathTimeLib::rot3mat(zeta, p3);
			//MathTimeLib::matmult(p2, p1, tr1, 3, 3, 3);
			//MathTimeLib::matmult(p3, tr1, prec, 3, 3, 3);
		}
		else
		{
			MathTimeLib::rot3mat(-chia, p1);
			MathTimeLib::rot1mat(wa, p2);
			MathTimeLib::rot3mat(psia, p3);
			MathTimeLib::rot1mat(-oblo, p4);
			MathTimeLib::matmult(p4, p3, tr1, 3, 3, 3);
			MathTimeLib::matmult(tr1, p2, tr2, 3, 3, 3);
			MathTimeLib::matmult(tr2, p1, prec, 3, 3, 3);
		}

		if (iauhelp == 'y')
		{
			printf("pr %11.7f  %11.7f  %11.7f %11.7fdeg \n", psia * 180 / pi, wa * 180 / pi, epsa * 180 / pi, chia * 180 / pi);
			printf("pr %11.7f  %11.7f  %11.7fdeg \n", zeta * 180 / pi, theta * 180 / pi, z * 180 / pi);
		}
	} // procedure precess

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
        *    opt         - method option                               e00cio, e00a, e96, e80
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

	void nutation
	(
		double ttt, double ddpsi, double ddeps,
		const iau80data &iau80arr, eOpt opt,
		double& deltapsi, double& deltaeps, double& trueeps, double& meaneps, double& omega,
		std::vector< std::vector<double> > &nut
	)
	{
		nut.resize(3);  // rows
		for (std::vector< std::vector<double> >::iterator it = nut.begin(); it != nut.end(); ++it)
			it->resize(3);
		// locals
		double deg2rad, l, l1, f, d,
			lonmer, lonven, lonear, lonmar, lonjup, lonsat, lonurn, lonnep, precrate,
			cospsi, sinpsi, coseps, sineps, costrueeps, sintrueeps;
		int  i;
		double tempval;

		char iauhelp;
		sethelp(iauhelp, ' ');

		deg2rad = pi / 180.0;

		// ---- determine coefficients for iau 1980 nutation theory ----
		meaneps = ((0.001813  * ttt - 0.00059) * ttt - 46.8150) * ttt + 84381.448;
		meaneps = fmod(meaneps / 3600.0, 360.0);
		meaneps = meaneps * deg2rad;

		fundarg(ttt, opt, l, l1, f, d, omega,
			lonmer, lonven, lonear, lonmar, lonjup, lonsat, lonurn, lonnep, precrate);

		deltapsi = 0.0;
		deltaeps = 0.0;
            for (i = 105; i >= 0; i--)
		{
			tempval = iau80arr.iar80[i][0] * l + iau80arr.iar80[i][1] * l1 + iau80arr.iar80[i][2] * f +
				iau80arr.iar80[i][3] * d + iau80arr.iar80[i][4] * omega;
			deltapsi = deltapsi + (iau80arr.rar80[i][0] + iau80arr.rar80[i][1] * ttt)  *  sin(tempval);
			deltaeps = deltaeps + (iau80arr.rar80[i][2] + iau80arr.rar80[i][3] * ttt) * cos(tempval);
		}

		// --------------- find nutation parameters --------------------
		deltapsi = fmod(deltapsi + ddpsi, twopi);
		deltaeps = fmod(deltaeps + ddeps, twopi);

		trueeps = meaneps + deltaeps;

		cospsi = cos(deltapsi);
		sinpsi = sin(deltapsi);
		coseps = cos(meaneps);
		sineps = sin(meaneps);
		costrueeps = cos(trueeps);
		sintrueeps = sin(trueeps);

		nut[0][0] = cospsi;
		nut[0][1] = costrueeps * sinpsi;
		nut[0][2] = sintrueeps * sinpsi;
		nut[1][0] = -coseps * sinpsi;
		nut[1][1] = costrueeps * coseps * cospsi + sintrueeps * sineps;
		nut[1][2] = sintrueeps * coseps * cospsi - sineps * costrueeps;
		nut[2][0] = -sineps * sinpsi;
		nut[2][1] = costrueeps * sineps * cospsi - sintrueeps * coseps;
		nut[2][2] = sintrueeps * sineps * cospsi + costrueeps * coseps;

		// alternate approach
		//MathTimeLib::rot1mat(trueeps, n1);
		//MathTimeLib::rot3mat(deltapsi, n2);
		//MathTimeLib::rot1mat(-meaneps, n3);
		//MathTimeLib::matmult(n2, n1, tr1, 3, 3, 3);
		//MathTimeLib::matmult(n3, tr1, nut, 3, 3, 3);

		if (iauhelp == 'y')
			printf("meaneps %11.7f dp  %11.7f de  %11.7f te  %11.7f  \n", meaneps * 180 / pi, deltapsi * 180 / pi,
				deltaeps * 180 / pi, trueeps * 180 / pi);
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

	void nutation00a
	(
		double ttt, double ddpsi, double ddeps,
		const iau00data &iau00arr, eOpt opt,
		std::vector< std::vector<double> > &tm
	)
	{
		tm.resize(3);  // rows
		for (std::vector< std::vector<double> >::iterator it = tm.begin(); it != tm.end(); ++it)
			it->resize(3);
		std::vector< std::vector<double> > prec = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > nut = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > a1 = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > a2 = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > a3 = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > a4 = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > a5 = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > a6 = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > a7 = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > a8 = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > a9 = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > a10 = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > tr1 = std::vector< std::vector<double> >(3, std::vector<double>(3));
		double l, l1, f, d, psia, wa, epsa, chia, deltapsi, deltaeps,
			lonmer, lonven, lonear, lonmar, lonjup, lonsat, lonurn, lonnep, precrate;
		int i;
		double tempval, convrt, ttt2, ttt3, ttt4, ttt5, j2d, raan;
		double pnsum, ensum, pplnsum, eplnsum, oblo;

		deltapsi = deltaeps = 0.0;

		// " to rad
		convrt = pi / (180.0 * 3600.0);

		ttt2 = ttt * ttt;
		ttt3 = ttt2 * ttt;
		ttt4 = ttt2 * ttt2;
		ttt5 = ttt3 * ttt2;

		fundarg(ttt, opt,  l,  l1,  f,  d,  raan,  lonmer,  lonven,  lonear,  lonmar,
			 lonjup,  lonsat,  lonurn,  lonnep,  precrate);

		// ---- obtain data coefficients
		// iau2006 approach - does not seem to be correct, close though
		if (opt == e00cio)
		{
			// looks like they still use the iau2000a method and adjust
			pnsum = 0.0;
			// data file is not not reveresed
			for (i = 1357; i >= 0; i--)
			{
				tempval = iau00arr.apni[i][0] * l + iau00arr.apni[i][1] * l1 + iau00arr.apni[i][2] * f + iau00arr.apni[i][3] * d + iau00arr.apni[i][4] * raan +
					iau00arr.apni[i][5] * lonmer + iau00arr.apni[i][6] * lonven + iau00arr.apni[i][7] * lonear + iau00arr.apni[i][8] * lonmar +
					iau00arr.apni[i][9] * lonjup + iau00arr.apni[i][10] * lonsat + iau00arr.apni[i][11] * lonurn + iau00arr.apni[i][12] * lonnep + iau00arr.apni[i][13] * precrate;
				if (i > 1319)
					pnsum = pnsum + (iau00arr.apn[i][0] * sin(tempval) + iau00arr.apn[i][1] * cos(tempval)) * ttt;  //note that sin and cos are reveresed between n and e
				else
					pnsum = pnsum + iau00arr.apn[i][0] * sin(tempval) + iau00arr.apn[i][1] * cos(tempval);
			}

			ensum = 0.0;
			// data file is not reveresed
			for (i = 1055; i >= 0; i--)
			{
				tempval = iau00arr.apei[i][0] * l + iau00arr.apei[i][1] * l1 + iau00arr.apei[i][2] * f + iau00arr.apei[i][3] * d + iau00arr.apei[i][4] * raan +
					iau00arr.apei[i][5] * lonmer + iau00arr.apei[i][6] * lonven + iau00arr.apei[i][7] * lonear + iau00arr.apei[i][8] * lonmar +
					iau00arr.apei[i][9] * lonjup + iau00arr.apei[i][10] * lonsat + iau00arr.apei[i][11] * lonurn + iau00arr.apei[i][12] * lonnep + iau00arr.apei[i][13] * precrate;
				if (i > 1036)
					ensum = ensum + (iau00arr.ape[i][0] * cos(tempval) + iau00arr.ape[i][1] * sin(tempval)) * ttt;
				else
					ensum = ensum + iau00arr.ape[i][0] * cos(tempval) + iau00arr.ape[i][1] * sin(tempval);
			}
			//  add planetary and luni-solar components.
			deltapsi = pnsum;  // rad
			deltaeps = ensum;

			// iau2006 corrections to the iau2000a
			j2d = -2.7774e-6 * ttt * convrt;  // rad
			deltapsi = deltapsi + deltapsi * (0.4697e-6 + j2d);  // rad
			deltaeps = deltaeps + deltaeps * j2d;
		}

		if (opt == e00a)
		{
			pnsum = 0.0;
			ensum = 0.0;
			for (i = 677; i >= 0; i--)
			{
				tempval = iau00arr.appni[i][0] * l + iau00arr.appni[i][1] * l1 + iau00arr.appni[i][2] * f + iau00arr.appni[i][3] * d + iau00arr.appni[i][4] * raan;
				tempval = fmod(tempval, 2.0 * pi);  // rad
													  //            pnsum = pnsum + (apn[i,1) + apn[i,2)*ttt) * sin(tempval) 
													  //                          + (apn[i,5) + apn[i,6)*ttt) * cos(tempval);
													  //            ensum = ensum + (apn[i,3) + apn[i,4)*ttt) * cos(tempval) 
													  //                          + (apn[i,7) + apn[i,8)*ttt) * sin(tempval);
													  // iers doesn't include the last few terms
				pnsum = pnsum + (iau00arr.appn[i][0] + iau00arr.appn[i][1] * ttt) * sin(tempval)
					+ (iau00arr.appn[i][4]) * cos(tempval);
				ensum = ensum + (iau00arr.appn[i][2] + iau00arr.appn[i][3] * ttt) * cos(tempval)
					+ (iau00arr.appn[i][6]) * sin(tempval);
			}

			pplnsum = 0.0;
			eplnsum = 0.0;
			// data file is already reveresed
			for (i = 686; i >= 0; i--)
			{
				tempval = iau00arr.aplni[i][0] * l + iau00arr.aplni[i][1] * l1 + iau00arr.aplni[i][2] * f + iau00arr.aplni[i][3] * d + iau00arr.aplni[i][4] * raan +
					iau00arr.aplni[i][5] * lonmer + iau00arr.aplni[i][6] * lonven + iau00arr.aplni[i][7] * lonear + iau00arr.aplni[i][8] * lonmar +
					iau00arr.aplni[i][9] * lonjup + iau00arr.aplni[i][10] * lonsat + iau00arr.aplni[i][11] * lonurn + iau00arr.aplni[i][12] * lonnep + iau00arr.aplni[i][13] * precrate;
				pplnsum = pplnsum + iau00arr.apln[i][0] * sin(tempval) + iau00arr.apln[i][1] * cos(tempval);
				eplnsum = eplnsum + iau00arr.apln[i][2] * sin(tempval) + iau00arr.apln[i][3] * cos(tempval);
			}

			//  add planetary and luni-solar components.
			deltapsi = pnsum + pplnsum;  // rad
			deltaeps = ensum + eplnsum;
		}

		// 2000b has 77 terms
		if (opt == e00b)
		{

		}

		precess(ttt, opt,  psia,  wa,  epsa,  chia, prec);

		oblo = 84381.406 * convrt; // " to rad or 448 - 406 for iau2006????

		// ----------------- find nutation matrix ----------------------
		// mean to true
		MathTimeLib::rot1mat(epsa + deltaeps, a1);
		MathTimeLib::rot3mat(deltapsi, a2);
		MathTimeLib::rot1mat(-epsa, a3);

		// j2000 to date(precession)
		MathTimeLib::rot3mat(-chia, a4);
		MathTimeLib::rot1mat(wa, a5);
		MathTimeLib::rot3mat(psia, a6);
		MathTimeLib::rot1mat(-oblo, a7);

		// icrs to j2000
		MathTimeLib::rot1mat(-0.0068192 * convrt, a8);
		MathTimeLib::rot2mat(0.0417750 * sin(oblo) * convrt, a9);
		//      a9  = rot2mat(0.0166170*convrt);
		MathTimeLib::rot3mat(0.0146 * convrt, a10);

		MathTimeLib::matmult(a5, a4, tr1, 3, 3, 3);
		MathTimeLib::matmult(tr1, a6, prec, 3, 3, 3);
		MathTimeLib::matmult(a7, prec, tr1, 3, 3, 3);
		MathTimeLib::matmult(tr1, a8, prec, 3, 3, 3);
		MathTimeLib::matmult(a9, prec,tr1,  3, 3, 3);
		MathTimeLib::matmult(tr1, a10, prec, 3, 3, 3);

		MathTimeLib::matmult(a2, a1, tr1, 3, 3, 3);
		MathTimeLib::matmult(tr1, a3, nut, 3, 3, 3);

		MathTimeLib::matmult(prec, nut, tm, 3, 3, 3);
	}   // nutation00a


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

	void sidereal
	(
		double jdut1, double deltapsi, double meaneps, double omega,
		double lod, int eqeterms, eOpt opt,
		std::vector< std::vector<double> > &st,
		std::vector< std::vector<double> > &stdot
	)
	{
		st.resize(3);  // rows
		for (std::vector< std::vector<double> >::iterator it = st.begin(); it != st.end(); ++it)
			it->resize(3);
		stdot.resize(3);  // rows
		for (std::vector< std::vector<double> >::iterator it = stdot.begin(); it != stdot.end(); ++it)
			it->resize(3);
		// locals
		double gmst, ast, thetasa, omegaearth, sinast, cosast, era, tut1d;
		char iauhelp;

		sethelp(iauhelp, ' ');

		if (opt == e80 || opt == e96)
		{
			// ------------------------ find gmst --------------------------
			gmst = AstroLib::gstime(jdut1);

			// ------------------------ find mean ast ----------------------
			if ((jdut1 > 2450449.5) && (eqeterms > 0))
			{
				ast = gmst + deltapsi * cos(meaneps) + 0.00264 * pi / (3600 * 180) * sin(omega)
					+ 0.000063 * pi / (3600 * 180) * sin(2.0  * omega);
			}
			else
				ast = gmst + deltapsi * cos(meaneps);

			ast = fmod(ast, 2.0 * pi);
		}
		else  // IAU 2010 approach
		{
			// julian centuries of ut1
			tut1d = jdut1 - 2451545.0;

			era = pi * 2.0 * (0.7790572732640 + 1.00273781191135448 * tut1d);
			era = fmod(era, (2.0 * pi));

			ast = era;  // set this for the matrix calcs below
		}


		thetasa = 7.29211514670698e-05 * (1.0 - lod / 86400.0);
		omegaearth = thetasa;

		sinast = sin(ast);
		cosast = cos(ast);

		st[0][0] = cosast;
		st[0][1] = -sinast;
		st[0][2] = 0.0;
		st[1][0] = sinast;
		st[1][1] = cosast;
		st[1][2] = 0.0;
		st[2][0] = 0.0;
		st[2][1] = 0.0;
		st[2][2] = 1.0;

		// compute sidereal time rate matrix
		stdot[0][0] = -omegaearth * sinast;
		stdot[0][1] = -omegaearth * cosast;
		stdot[0][2] = 0.0;
		stdot[1][0] = omegaearth * cosast;
		stdot[1][1] = -omegaearth * sinast;
		stdot[1][2] = 0.0;
		stdot[2][0] = 0.0;
		stdot[2][1] = 0.0;
		stdot[2][2] = 0.0;

		if (iauhelp == 'y')
			printf("st gmst %11.7f ast %11.7f ome  %11.7f \n", gmst * 180 / pi, ast * 180 / pi, omegaearth * 180 / pi);

	}  // procedure sidereal

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

	void polarm
	(
		double xp, double yp, double ttt, eOpt opt, std::vector< std::vector<double> > &pm
	)
	{
		pm.resize(3);  // rows
		for (std::vector< std::vector<double> >::iterator it = pm.begin(); it != pm.end(); ++it)
			it->resize(3);
		double cosxp, cosyp, sinxp, sinyp, sp, cossp, sinsp;

		cosxp = cos(xp);
		sinxp = sin(xp);
		cosyp = cos(yp);
		sinyp = sin(yp);

		if ((opt == e80) | (opt == e96))
		{
			pm[0][0] = cosxp;
			pm[0][1] = 0.0;
			pm[0][2] = -sinxp;
			pm[1][0] = sinxp * sinyp;
			pm[1][1] = cosyp;
			pm[1][2] = cosxp * sinyp;
			pm[2][0] = sinxp * cosyp;
			pm[2][1] = -sinyp;
			pm[2][2] = cosxp * cosyp;

			// alternate approach
			//MathTimeLib::rot2mat(xp, a1);
			//MathTimeLib::rot1mat(yp, a2);
			//MathTimeLib::matmult(a2, a1, pm, 3, 3, 3);
		}
		else
		{
			// approximate sp value in rad
			sp = -47.0e-6 * ttt * pi / (180.0 * 3600.0);
			cossp = cos(sp);
			sinsp = sin(sp);

			// form the matrix
			pm[0][0] = cosxp * cossp;
			pm[0][1] = -cosyp * sinsp + sinyp * sinxp * cossp;
			pm[0][2] = -sinyp * sinsp - cosyp * sinxp * cossp;
			pm[1][0] = cosxp * sinsp;
			pm[1][1] = cosyp * cossp + sinyp * sinxp * sinsp;
			pm[1][2] = sinyp * cossp - cosyp * sinxp * sinsp;
			pm[2][0] = sinxp;
			pm[2][1] = -sinyp * cosxp;
			pm[2][2] = cosyp * cosxp;

			// alternate approach
			//MathTimeLib::rot1mat(yp, a1);
			//MathTimeLib::rot2mat(xp, a2);
			//MathTimeLib::rot3mat(-sp, a3);
			//MathTimeLib::matmult(a2, a1, tr1, 3, 3, 3);
			//MathTimeLib::matmult(a3, tr1, pm, 3, 3, 3);
		}
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

	void framebias
	(
		char opt,
		double& term1, double& term2, double& term3, std::vector< std::vector<double> > &fb
	)
	{
		fb.resize(3);  // rows
		for (std::vector< std::vector<double> >::iterator it = fb.begin(); it != fb.end(); ++it)
			it->resize(3);
		double convrt;
		convrt = pi / (3600.0 * 180.0);

		// j2000 version referred to iau76/80 theory
		if (opt == 'j')
		{
			term1 = -0.0146 * convrt;
			term2 = -0.016617 * convrt;
			term3 = -0.0068192 * convrt;
			fb[0][0] = 0.99999999999999;
			fb[0][1] = 0.00000007078279;
			fb[0][2] = -0.00000008056149;
			fb[1][0] = -0.00000007078280;
			fb[1][1] = 1.0;
			fb[1][2] = -0.00000003306041;
			fb[2][0] = 0.00000008056149;
			fb[2][1] = 0.00000003306041;
			fb[2][2] = 1.0;
		}

		// fk5 version - catalog origin
		if (opt == 'f')
		{
			term1 = -0.0229 * convrt;
			term2 = 0.0091 * convrt;
			term3 = -0.0199 * convrt;
			fb[0][0] = 0.99999999999999;
			fb[0][1] = 0.00000011102234;
			fb[0][2] = 0.00000004411803;
			fb[1][0] = -0.00000011102233;
			fb[1][1] = 0.99999999999999;
			fb[1][2] = -0.00000009647793;
			fb[2][0] = -0.00000004411804;
			fb[2][1] = 0.00000009647792;
			fb[2][2] = 0.99999999999999;
		}

	}  // procedure framebias




/* ----------------------------------------------------------------------------
*
*                           function itrf_gcrf
*
*  this function transforms a vector between the earth fixed (itrf) frame, and
*    the gcrf mean equator mean equinox. this is the preferrred method to
*    accomplish the new iau 2000 resolutions and uses the eop corrections
*
*  author        : david vallado                  719-573-2600   23 nov 2005
*
*  revisions
*
*  inputs          description                    range / units
*    ritrf       - position vector earth fixed    km
*    vitrf       - velocity vector earth fixed    km/s
*    aitrf       - acceleration vector earth fixedkm/s2
*    direct      - direction of transfer          eFrom, eTo
*    iau80arr    - record containing the iau80 constants rad
*    ttt         - julian centuries of tt         centuries
*    jdut1       - julian date of ut1             days from 4713 bc
*    lod         - excess length of day           sec
*    xp          - polar motion coefficient       rad
*    yp          - polar motion coefficient       rad
*    eqeterms    - terms for ast calculation      0,2
*    ddpsi       - delta psi correction to gcrf   rad
*    ddeps       - delta eps correction to gcrf   rad
*    deltapsi    - nutation angle                 rad
*    deltaeps    - nutation angle                 rad
		*    opt         - method option                           e80, e96, e00a, e00b
*
*  outputs       :
*    rgcrf       - position vector gcrf            km
*    vgcrf       - velocity vector gcrf            km/s
*    agcrf       - acceleration vector gcrf        km/s2
*    trans       - matrix for pef - gcrf
*
*  locals        :
*    trueeps     - true obliquity of the ecliptic rad
*    meaneps     - mean obliquity of the ecliptic rad
*    omega       -                                rad
*    prec        - matrix for mod - gcrf
*    nut         - matrix for tod - mod
*    st          - matrix for pef - tod
*    stdot       - matrix for pef - tod rate
*    pm          - matrix for itrf - pef
*
*  coupling      :
*   precess      - rotation for precession
*   nutation     - rotation for nutation
*   sidereal     - rotation for sidereal time
*   polarm       - rotation for polar motion
*
*  references    :
*    vallado       2013, 228, Alg 24
* --------------------------------------------------------------------------- */

	void itrf_gcrf
	(
		double ritrf[3], double vitrf[3], double aitrf[3],
		MathTimeLib::edirection direct,
		double rgcrf[3], double vgcrf[3], double agcrf[3],
		const iau80data &iau80arr, eOpt opt,
		double ttt, double jdut1, double lod, double xp,
		double yp, int eqeterms, double ddpsi, double ddeps,
		std::vector< std::vector<double> > &trans
	)
	{
		trans.resize(3);  // rows
		for (std::vector< std::vector<double> >::iterator it = trans.begin(); it != trans.end(); ++it)
			it->resize(3);
		// locals
		double psia, wa, epsa, chia, deltapsi, deltaeps,
			trueeps, meaneps, omega, thetasa, omegaearth[3], omgxr[3], omgxomgxr[3],
			rpef[3], vpef[3], apef[3], omgxv[3], tempvec1[3], tempvec[3];
		double  deg2rad;
		//std::vector< std::vector<double> > prec, st, stdot, pm, pmp, stp, nutp, precp, temp;
		std::vector< std::vector<double> > prec = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > nut = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > st = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > stdot = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > pm = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > precp = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > nutp = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > stp = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > pmp = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > temp = std::vector< std::vector<double> >(3, std::vector<double>(3));

		deg2rad = pi / 180.0;

		// ---- find matrices
		precess(ttt, opt, psia, wa, epsa, chia, prec);
		nutation(ttt, ddpsi, ddeps, iau80arr, opt, deltapsi, deltaeps, trueeps, meaneps, omega, nut);
		sidereal(jdut1, deltapsi, meaneps, omega, lod, eqeterms, opt, st, stdot);
		polarm(xp, yp, ttt, opt, pm);

		// ---- perform transformations
		thetasa = 7.29211514670698e-05 * (1.0 - lod / 86400.0);
		omegaearth[0] = 0.0;
		omegaearth[1] = 0.0;
		omegaearth[2] = thetasa;

		if (direct == MathTimeLib::eTo)
		{
			MathTimeLib::matvecmult(pm, ritrf, rpef);
			MathTimeLib::matmult(prec, nut, temp, 3, 3, 3);
			MathTimeLib::matmult(temp, st, trans, 3, 3, 3);
			MathTimeLib::matvecmult(trans, rpef, rgcrf);

			MathTimeLib::matvecmult(pm, vitrf, vpef);
			MathTimeLib::cross(omegaearth, rpef, omgxr);
			MathTimeLib::addvec(1.0, vpef, 1.0, omgxr, tempvec1);
			MathTimeLib::matvecmult(trans, tempvec1, vgcrf);

			MathTimeLib::matvecmult(pm, aitrf, apef);
			MathTimeLib::cross(omegaearth, omgxr, omgxomgxr);
			MathTimeLib::cross(omegaearth, vpef, omgxv);
			MathTimeLib::addvec(1.0, apef, 1.0, omgxomgxr, tempvec);
			MathTimeLib::addvec(1.0, tempvec, 2.0, omgxv, tempvec1);
			MathTimeLib::matvecmult(trans, tempvec1, agcrf);
		}
		else
		{
			MathTimeLib::mattrans(pm, pmp, 3, 3);
			MathTimeLib::mattrans(st, stp, 3, 3);
			MathTimeLib::mattrans(nut, nutp, 3, 3);
			MathTimeLib::mattrans(prec, precp, 3, 3);

			MathTimeLib::matmult(stp, nutp, temp, 3, 3, 3);
			MathTimeLib::matmult(temp, precp, trans, 3, 3, 3);
			MathTimeLib::matvecmult(trans, rgcrf, rpef);
			MathTimeLib::matvecmult(pmp, rpef, ritrf);

			MathTimeLib::cross(omegaearth, rpef, omgxr);
			MathTimeLib::matvecmult(trans, vgcrf, tempvec1);
			MathTimeLib::addvec(1.0, tempvec1, -1.0, omgxr, vpef);
			MathTimeLib::matvecmult(pmp, vpef, vitrf);

			MathTimeLib::addvec(1.0, tempvec1, -1.0, omgxr, vpef);
			MathTimeLib::cross(omegaearth, vpef, omgxv);
			MathTimeLib::cross(omegaearth, omgxr, omgxomgxr);
			MathTimeLib::matvecmult(trans, agcrf, tempvec1);
			MathTimeLib::addvec(1.0, tempvec1, -1.0, omgxomgxr, tempvec);
			MathTimeLib::addvec(1.0, tempvec, -2.0, omgxv, apef);
			MathTimeLib::matvecmult(pmp, apef, aitrf);
		}
	}  // procedure itrf_gcrf


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

	void eci_ecef00
	(
		double reci[3], double veci[3], 
		MathTimeLib::edirection direct,
		double recef[3], double vecef[3],
		const iau00data &iau00arr, eOpt opt,
		double ttt, double jdut1, double lod, double xp, double yp, int eqeterms, double ddx, double ddy
	)
	{
		double psia, wa, epsa, chia;
		double x, y, s;
		double meaneps, raan, deltapsi, gst;
		double omegaearth[3];
		double rpef[3];
		double vpef[3];
		double crossr[3];
		double tempvec1[3];
		std::vector< std::vector<double> > prec = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > nut = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > st = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > stdot = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > pm = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > precp = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > nutp = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > stp = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > pmp = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > temp = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > trans = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > tm = std::vector< std::vector<double> >(3, std::vector<double>(3));

		deltapsi = 0.0;
		meaneps = 0.0;
		raan = 0.0;

		// IAU-2010 CIO approach
		if (opt == e00cio)
		{
			prec[0][0] = 1.0;
			prec[1][1] = 1.0;
			prec[2][2] = 1.0;
			iau00xys(ttt, ddx, ddy, opt, iau00arr, x, y, s, nut);
			sidereal(jdut1, deltapsi, meaneps, raan, lod, eqeterms, opt, st, stdot);
		}
		else  // IAU-2010 pna approach
		{
			precess(ttt, opt, psia, wa, epsa, chia, prec);
			nutation00a(ttt, ddx, ddy, iau00arr, opt, nut);
			gstime00(jdut1, deltapsi, ttt, iau00arr, opt, gst, st);
		}

		omegaearth[0] = 0.0;
		omegaearth[1] = 0.0;
		omegaearth[2] = earthrot * (1.0 - lod / 86400.0);

		polarm(xp, yp, ttt, opt, pm);

		if (direct == MathTimeLib::eTo)
		{
			// ---- perform transformations
			MathTimeLib::mattrans(pm, pmp, 3, 3);
			MathTimeLib::mattrans(st, stp, 3, 3);
			MathTimeLib::mattrans(nut, nutp, 3, 3);
			MathTimeLib::mattrans(prec, precp, 3, 3);

			MathTimeLib::matmult(stp, nutp, temp, 3, 3, 3);
			MathTimeLib::matmult(temp, precp, trans, 3, 3, 3);
			MathTimeLib::matvecmult(trans, reci, rpef);
			MathTimeLib::matvecmult(pmp, rpef, recef);

			MathTimeLib::matvecmult(trans, veci, tempvec1);
			MathTimeLib::cross(omegaearth, rpef, crossr);
			vpef[0] = tempvec1[0] - crossr[0];
			vpef[1] = tempvec1[1] - crossr[1];
			vpef[2] = tempvec1[2] - crossr[2];
			MathTimeLib::matvecmult(pmp, vpef, vecef);
		}
		else
		{
			// ---- perform transformations
			MathTimeLib::matvecmult(pm, recef, rpef);
			printf("pm = %11.7f  %11.7f %11.7f \n", pm[0][0], pm[0][1], pm[0][2]);
			MathTimeLib::matmult(prec, nut, temp, 3, 3, 3);
			printf("nut = %11.7f  %11.7f %11.7f \n", nut[0][0], nut[0][1], nut[0][2]);
			MathTimeLib::matmult(temp, st, trans, 3, 3, 3);
			printf("st = %11.7f  %11.7f %11.7f \n", st[0][0], st[0][1], st[0][2]);
			MathTimeLib::matvecmult(trans, rpef, reci);
			printf("trans = %11.7f  %11.7f %11.7f \n", trans[0][0], trans[0][1], trans[0][2]);

			MathTimeLib::matvecmult(pm, vecef, vpef);
			MathTimeLib::cross(omegaearth, rpef, crossr);
			tempvec1[0] = vpef[0] + crossr[0];
			tempvec1[1] = vpef[1] + crossr[1];
			tempvec1[2] = vpef[2] + crossr[2];
			MathTimeLib::matvecmult(trans, tempvec1, veci);
		}

	}  //  eci2ecef00 



/* ----------------------------------------------------------------------------
*
*                           function itrf_j2k
*
*  this function transforms a vector between the earth fixed (itrf) frame, and
*    the j2k mean equator mean equinox (iau-76, fk5). note that the delta
*    corrections are not included for this approach.
*
*  author        : david vallado                  719-573-2600   30 nov 2005
*
*  revisions
*
*  inputs          description                    range / units
*    ritrf       - position vector earth fixed    km
*    vitrf       - velocity vector earth fixed    km/s
*    aitrf       - acceleration vector earth fixedkm/s2
*    direct      - direction of transfer          eFrom, eTo
*    iau80arr    - record containing the iau80 constants rad
*    ttt         - julian centuries of tt         centuries
*    jdut1       - julian date of ut1             days from 4713 bc
*    lod         - excess length of day           sec
*    xp          - polar motion coefficient       rad
*    yp          - polar motion coefficient       rad
*    eqeterms    - terms for ast calculation      0,2
		*    opt         - method option                           e80, e96, e00a, e00b
*
*  outputs       :
*    rj2k        - position vector j2k            km
*    vj2k        - velocity vector j2k            km/s
*    aj2k        - acceleration vector j2k        km/s2
*
*  locals        :
*    deltapsi    - nutation angle                 rad
*    trueeps     - true obliquity of the ecliptic rad
*    meaneps     - mean obliquity of the ecliptic rad
*    omega       -                                rad
*    prec        - matrix for mod - j2k
*    nut         - matrix for tod - mod
*    st          - matrix for pef - tod
*    stdot       - matrix for pef - tod rate
*    pm          - matrix for itrf - pef
*
*  coupling      :
*   precess      - rotation for precession
*   nutation     - rotation for nutation
*   sidereal     - rotation for sidereal time
*   polarm       - rotation for polar motion
*
*  references    :
*    vallado       2013, 228, Alg 24
* --------------------------------------------------------------------------- */

	void itrf_j2k
	(
		double ritrf[3], double vitrf[3], double aitrf[3],
		MathTimeLib::edirection direct,
		double rj2k[3], double vj2k[3], double aj2k[3],
		const iau80data &iau80arr, eOpt opt,
		double ttt, double jdut1, double lod,
		double xp, double yp, int eqeterms,
		std::vector< std::vector<double> > &trans
	)
	{
		trans.resize(3);  // rows
		for (std::vector< std::vector<double> >::iterator it = trans.begin(); it != trans.end(); ++it)
			it->resize(3);
		// locals
		//std::vector< std::vector<double> > prec, nut, st, stdot, pm, temp,pmp, stp, nutp, precp;
		std::vector< std::vector<double> > prec = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > nut = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > st = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > stdot = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > pm = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > precp = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > nutp = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > stp = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > pmp = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > temp = std::vector< std::vector<double> >(3, std::vector<double>(3));
		double psia, wa, epsa, chia, deltapsi, deltaeps, trueeps, meaneps,
			omega, thetasa, omegaearth[3], rpef[3], vpef[3], apef[3], omgxr[3], omgxomgxr[3],
			omgxv[3], tempvec1[3], tempvec[3];

		// ---- find matrices
		precess(ttt, opt, psia, wa, epsa, chia, prec);
		nutation(ttt, 0.0, 0.0, iau80arr, opt, deltapsi, deltaeps, trueeps, meaneps, omega, nut);
		sidereal(jdut1, deltapsi, meaneps, omega, lod, eqeterms, opt, st, stdot);
		polarm(xp, yp, ttt, opt, pm);

		// ---- perform transformations
		thetasa = 7.29211514670698e-05 * (1.0 - lod / 86400.0);
		omegaearth[0] = 0.0;
		omegaearth[1] = 0.0;
		omegaearth[2] = thetasa;

		if (direct == MathTimeLib::eTo)
		{
			MathTimeLib::matvecmult(pm, ritrf, rpef);
			MathTimeLib::matmult(prec, nut, temp, 3, 3, 3);
			MathTimeLib::matmult(temp, st, trans, 3, 3, 3);
			MathTimeLib::matvecmult(trans, rpef, rj2k);

			MathTimeLib::matvecmult(pm, vitrf, vpef);
			MathTimeLib::cross(omegaearth, rpef, omgxr);
			MathTimeLib::addvec(1.0, vpef, 1.0, omgxr, tempvec1);
			MathTimeLib::matvecmult(trans, tempvec1, vj2k);

			MathTimeLib::matvecmult(pm, aitrf, apef);
			MathTimeLib::cross(omegaearth, omgxr, omgxomgxr);
			MathTimeLib::cross(omegaearth, vpef, omgxv);
			MathTimeLib::addvec(1.0, apef, 1.0, omgxomgxr, tempvec);
			MathTimeLib::addvec(1.0, tempvec, 2.0, omgxv, tempvec1);
			MathTimeLib::matvecmult(trans, tempvec1, aj2k);
		}
		else
		{
			MathTimeLib::mattrans(pm, pmp, 3, 3);
			MathTimeLib::mattrans(st, stp, 3, 3);
			MathTimeLib::mattrans(nut, nutp, 3, 3);
			MathTimeLib::mattrans(prec, precp, 3, 3);

			MathTimeLib::matmult(stp, nutp, temp, 3, 3, 3);
			MathTimeLib::matmult(temp, precp, trans, 3, 3, 3);
			MathTimeLib::matvecmult(trans, rj2k, rpef);
			MathTimeLib::matvecmult(pmp, rpef, ritrf);

			MathTimeLib::cross(omegaearth, rpef, omgxr);
			MathTimeLib::matvecmult(trans, vj2k, tempvec1);
			MathTimeLib::addvec(1.0, tempvec1, -1.0, omgxr, vpef);
			MathTimeLib::matvecmult(pmp, vpef, vitrf);

			MathTimeLib::addvec(1.0, tempvec1, -1.0, omgxr, vpef);
			MathTimeLib::cross(omegaearth, vpef, omgxv);
			MathTimeLib::cross(omegaearth, omgxr, omgxomgxr);
			MathTimeLib::matvecmult(trans, aj2k, tempvec1);
			MathTimeLib::addvec(1.0, tempvec1, -1.0, omgxomgxr, tempvec);
			MathTimeLib::addvec(1.0, tempvec, -2.0, omgxv, apef);
			MathTimeLib::matvecmult(pmp, apef, aitrf);
		}
	} // procedure itrf_j2k

/* ----------------------------------------------------------------------------
*
*                           function itrf_mod
*
*  this function transforms a vector between the earth fixed (itrf) frame, and
*    the gcrf mean equator mean equinox (j2000).
*
*  author        : david vallado                  719-573-2600   22 nov 2005
*
*  revisions
*
*  inputs          description                    range / units
*    ritrf       - position vector earth fixed    km
*    vitrf       - velocity vector earth fixed    km/s
*    aitrf       - acceleration vector earth fixedkm/s2
*    direct      - direction of transfer          eFrom, eTo
*    iau80arr    - record containing the iau80 constants rad
*    ttt         - julian centuries of tt         centuries
*    jdut1       - julian date of ut1             days from 4713 bc
*    lod         - excess length of day           sec
*    xp          - polar motion coefficient       rad
*    yp          - polar motion coefficient       rad
*    eqeterms    - terms for ast calculation      0,2
*    ddpsi       - delta psi correction to gcrf   rad
*    ddeps       - delta eps correction to gcrf   rad
		*    opt         - method option                           e80, e96, e00a, e00b
*
*  outputs       :
*    rmod        - position vector mod            km
*    vmod        - velocity vector mod            km/s
*    amod        - acceleration vector mod        km/s2
*
*  locals        :
*    deltapsi    - nutation angle                 rad
*    trueeps     - true obliquity of the ecliptic rad
*    meaneps     - mean obliquity of the ecliptic rad
*    omega       -                                rad
*    nut         - matrix for tod - mod
*    st          - matrix for pef - tod
*    stdot       - matrix for pef - tod rate
*    pm          - matrix for itrf - pef
*
*  coupling      :
*   nutation     - rotation for nutation
*   sidereal     - rotation for sidereal time
*   polarm       - rotation for polar motion
*
*  references    :
*    vallado       2013, 228, Alg 24
* --------------------------------------------------------------------------- */

	void itrf_mod
	(
		double ritrf[3], double vitrf[3], double aitrf[3],
		MathTimeLib::edirection direct,
		double rmod[3], double vmod[3], double amod[3],
		const iau80data &iau80arr, eOpt opt,
		double ttt, double jdut1, double lod, double xp,
		double yp, int eqeterms, double ddpsi, double ddeps,
		std::vector< std::vector<double> > &trans
	)
	{
		trans.resize(3);  // rows
		for (std::vector< std::vector<double> >::iterator it = trans.begin(); it != trans.end(); ++it)
			it->resize(3);
		//std::vector< std::vector<double> > nut, st, stdot, pm, temp[3][3], tempmat,	pmp, stp, nutp, precp;
		std::vector< std::vector<double> > nut = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > st = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > stdot = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > pm = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > precp = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > nutp = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > stp = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > pmp = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > temp = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > tempmat = std::vector< std::vector<double> >(3, std::vector<double>(3));
		double deltapsi, deltaeps, trueeps, meaneps, omega, thetasa, omegaearth[3],
			rpef[3], vpef[3], apef[3], omgxr[3], omgxomgxr[3],
			omgxv[3], tempvec1[3], tempvec[3];

		// ---- find matrices
		nutation(ttt, ddpsi, ddeps, iau80arr, opt, deltapsi, deltaeps, trueeps, meaneps, omega, nut);
		sidereal(jdut1, deltapsi, meaneps, omega, lod, eqeterms, opt, st, stdot);
		polarm(xp, yp, ttt, opt, pm);

		// ---- perform transformations
		thetasa = 7.29211514670698e-05 * (1.0 - lod / 86400.0);
		omegaearth[0] = 0.0;
		omegaearth[1] = 0.0;
		omegaearth[2] = thetasa;

		if (direct == MathTimeLib::eTo)
		{
			MathTimeLib::matvecmult(pm, ritrf, rpef);
			MathTimeLib::matmult(nut, st, tempmat, 3, 3, 3);
			MathTimeLib::matvecmult(tempmat, rpef, rmod);

			MathTimeLib::matvecmult(pm, vitrf, vpef);
			MathTimeLib::cross(omegaearth, rpef, omgxr);
			MathTimeLib::addvec(1.0, vpef, 1.0, omgxr, tempvec1);
			MathTimeLib::matvecmult(tempmat, tempvec1, vmod);

			MathTimeLib::matvecmult(pm, aitrf, apef);
			MathTimeLib::cross(omegaearth, omgxr, omgxomgxr);
			MathTimeLib::cross(omegaearth, vpef, omgxv);
			MathTimeLib::addvec(1.0, apef, 1.0, omgxomgxr, tempvec);
			MathTimeLib::addvec(1.0, tempvec, 2.0, omgxv, tempvec1);
			MathTimeLib::matvecmult(tempmat, tempvec1, amod);
		}
		else
		{
			MathTimeLib::mattrans(pm, pmp, 3, 3);
			MathTimeLib::mattrans(st, stp, 3, 3);
			MathTimeLib::mattrans(nut, nutp, 3, 3);

			MathTimeLib::matmult(stp, nutp, tempmat, 3, 3, 3);
			MathTimeLib::matvecmult(tempmat, rmod, rpef);
			MathTimeLib::matvecmult(pmp, rpef, ritrf);

			MathTimeLib::cross(omegaearth, rpef, omgxr);
			MathTimeLib::matvecmult(tempmat, vmod, tempvec1);
			MathTimeLib::addvec(1.0, tempvec1, -1.0, omgxr, vpef);
			MathTimeLib::matvecmult(pmp, vpef, vitrf);
			MathTimeLib::addvec(1.0, tempvec1, -1.0, omgxr, vpef);
			MathTimeLib::cross(omegaearth, vpef, omgxv);
			MathTimeLib::cross(omegaearth, omgxr, omgxomgxr);
			MathTimeLib::matvecmult(tempmat, amod, tempvec1);
			MathTimeLib::addvec(1.0, tempvec1, -1.0, omgxomgxr, tempvec);
			MathTimeLib::addvec(1.0, tempvec, -2.0, omgxv, apef);
			MathTimeLib::matvecmult(pmp, apef, aitrf);
		}
	}  // procedure itrf_mod



/* ----------------------------------------------------------------------------
*
*                           function itrf_tod
*
*  this function transforms a vector between the earth fixed (itrf) frame, and
*    the true of date (tod).
*
*  author        : david vallado                  719-573-2600   22 nov 2005
*
*  revisions
*
*  inputs          description                    range / units
*    ritrf       - position vector earth fixed    km
*    vitrf       - velocity vector earth fixed    km/s
*    aitrf       - acceleration vector earth fixedkm/s2
*    direct      - direction of transfer          eFrom, eTo
*    iau80arr    - record containing the iau80 constants rad
*    ttt         - julian centuries of tt         centuries
*    jdut1       - julian date of ut1             days from 4713 bc
*    lod         - excess length of day           sec
*    xp          - polar motion coefficient       rad
*    yp          - polar motion coefficient       rad
*    eqeterms    - terms for ast calculation      0,2
*    ddpsi       - delta psi correction to gcrf   rad
*    ddeps       - delta eps correction to gcrf   rad
		*    opt         - method option                           e80, e96, e00a, e00b
*
*  outputs       :
*    rtod        - position vector tod            km
*    vtod        - velocity vector tod            km/s
*    atod        - acceleration vector tod        km/s2
*
*  locals        :
*    deltapsi    - nutation angle                 rad
*    trueeps     - true obliquity of the ecliptic rad
*    meaneps     - mean obliquity of the ecliptic rad
*    omega       -                                rad
*    nut         - matrix for tod - mod
*    st          - matrix for pef - tod
*    stdot       - matrix for pef - tod rate
*    pm          - matrix for itrf - pef
*
*  coupling      :
*   nutation     - rotation for nutation
*   sidereal     - rotation for sidereal time
*   polarm       - rotation for polar motion
*
*  references    :
*    vallado       2013, 228, Alg 24
* --------------------------------------------------------------------------- */

	void itrf_tod
	(
		double ritrf[3], double vitrf[3], double aitrf[3],
		MathTimeLib::edirection direct,
		double rtod[3], double vtod[3], double atod[3],
		const iau80data &iau80arr, eOpt opt,
		double ttt, double jdut1, double lod, double xp,
		double yp, int eqeterms, double ddpsi, double ddeps,
		std::vector< std::vector<double> > &trans
	)
	{
		trans.resize(3);  // rows
		for (std::vector< std::vector<double> >::iterator it = trans.begin(); it != trans.end(); ++it)
			it->resize(3);
		// locals
		//std::vector< std::vector<double> > nut, st, stdot, pm, temp[3][3], tempmat,	pmp, stp, nutp, precp;
		std::vector< std::vector<double> > nut = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > st = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > stdot = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > pm = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > precp = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > nutp = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > stp = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > pmp = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > temp = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > tempmat = std::vector< std::vector<double> >(3, std::vector<double>(3));
		double deltapsi, deltaeps, trueeps, meaneps, omega, thetasa, omegaearth[3],
			rpef[3], vpef[3], apef[3], omgxr[3], omgxomgxr[3],
			omgxv[3], tempvec1[3], tempvec[3];

		// ---- find matrices
		nutation(ttt, ddpsi, ddeps, iau80arr, opt, deltapsi, deltaeps, trueeps, meaneps, omega, nut);
		sidereal(jdut1, deltapsi, meaneps, omega, lod, eqeterms, opt, st, stdot);
		polarm(xp, yp, ttt, opt, pm);

		// ---- perform transformations
		thetasa = 7.29211514670698e-05 * (1.0 - lod / 86400.0);
		omegaearth[0] = 0.0;
		omegaearth[1] = 0.0;
		omegaearth[2] = thetasa;

		if (direct == MathTimeLib::eTo)
		{
			MathTimeLib::matvecmult(pm, ritrf, rpef);
			MathTimeLib::matvecmult(st, rpef, rtod);

			MathTimeLib::matvecmult(pm, vitrf, vpef);
			MathTimeLib::cross(omegaearth, rpef, omgxr);
			MathTimeLib::addvec(1.0, vpef, 1.0, omgxr, tempvec1);
			MathTimeLib::matvecmult(st, tempvec1, vtod);

			MathTimeLib::matvecmult(pm, aitrf, apef);
			MathTimeLib::cross(omegaearth, omgxr, omgxomgxr);
			MathTimeLib::cross(omegaearth, vpef, omgxv);
			MathTimeLib::addvec(1.0, apef, 1.0, omgxomgxr, tempvec);
			MathTimeLib::addvec(1.0, tempvec, 2.0, omgxv, tempvec1);
			MathTimeLib::matvecmult(st, tempvec1, atod);
		}
		else
		{
			MathTimeLib::mattrans(pm, pmp, 3, 3);
			MathTimeLib::mattrans(st, stp, 3, 3);

			MathTimeLib::matvecmult(stp, rtod, rpef);
			MathTimeLib::matvecmult(pmp, rpef, ritrf);

			MathTimeLib::cross(omegaearth, rpef, omgxr);
			MathTimeLib::matvecmult(stp, vtod, tempvec1);
			MathTimeLib::addvec(1.0, tempvec1, -1.0, omgxr, vpef);
			MathTimeLib::matvecmult(pmp, vpef, vitrf);
			MathTimeLib::addvec(1.0, tempvec1, -1.0, omgxr, vpef);
			MathTimeLib::cross(omegaearth, vpef, omgxv);
			MathTimeLib::cross(omegaearth, omgxr, omgxomgxr);
			MathTimeLib::matvecmult(stp, atod, tempvec1);
			MathTimeLib::addvec(1.0, tempvec1, -1.0, omgxomgxr, tempvec);
			MathTimeLib::addvec(1.0, tempvec, -2.0, omgxv, apef);
			MathTimeLib::matvecmult(pmp, apef, aitrf);
		}
	}  // procedure itrf_tod


/* ----------------------------------------------------------------------------
*
*                           function itrf_pef
*
*  this function transforms a vector between the earth fixed (itrf) frame, and
*    the psuedo earth fixed (pef).
*
*  author        : david vallado                  719-573-2600   22 nov 2005
*
*  revisions
*
*  inputs          description                    range / units
*    ritrf       - position vector earth fixed    km
*    vitrf       - velocity vector earth fixed    km/s
*    aitrf       - acceleration vector earth fixedkm/s2
*    direct      - direction of transfer          eFrom, eTo
*    ttt         - julian centuries of tt         centuries
*    xp          - polar motion coefficient       rad
*    yp          - polar motion coefficient       rad
*
*  outputs       :
*    rpef        - position vector pef            km
*    vpef        - velocity vector pef            km/s
*    apef        - acceleration vector pef        km/s2
*
*  locals        :
*    pm          - matrix for itrf - pef
*
*  coupling      :
*   polarm       - rotation for polar motion
*
*  references    :
*    vallado       2013, 228, Alg 24
* --------------------------------------------------------------------------- */

	void itrf_pef
	(
		double ritrf[3], double vitrf[3], double aitrf[3],
		MathTimeLib::edirection direct,
		double rpef[3], double vpef[3], double apef[3],
		double ttt, double xp, double yp,
		std::vector< std::vector<double> > &trans
	)
	{
		trans.resize(3);  // rows
		for (std::vector< std::vector<double> >::iterator it = trans.begin(); it != trans.end(); ++it)
			it->resize(3);
		// locals
		//std::vector< std::vector<double> > pm, temp[3][3], pmp;
		std::vector< std::vector<double> > pm = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > pmp = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > temp = std::vector< std::vector<double> >(3, std::vector<double>(3));

		// ---- find matrices
		polarm(xp, yp, ttt, e80, pm);

		// ---- perform transformations
		if (direct == MathTimeLib::eTo)
		{
			MathTimeLib::matvecmult(pm, ritrf, rpef);

			MathTimeLib::matvecmult(pm, vitrf, vpef);

			MathTimeLib::matvecmult(pm, aitrf, apef);
		}
		else
		{
			MathTimeLib::mattrans(pm, pmp, 3, 3);

			MathTimeLib::matvecmult(pmp, rpef, ritrf);

			MathTimeLib::matvecmult(pmp, vpef, vitrf);

			MathTimeLib::matvecmult(pmp, apef, aitrf);
		}
	} // procedure itrf_pef


/* ----------------------------------------------------------------------------
*
*                           function pef_gcrf
*
*  this function transforms a vector between the pseudo earth fixed frame (pef),
*    and the mean equator mean equinox (j2000) frame.
*
*  author        : david vallado                  719-573-2600   25 jun 2002
*
*  revisions
*    vallado     - add terms for ast calculation                 30 sep 2002
*    vallado     - consolidate with iau 2000                     14 feb 2005
*    vallado     - conversion to c++                             21 feb 2005
*
*  inputs          description                    range / units
*    rpef        - position pseudo earth fixed    km
*    vpef        - velocity pseudo earth fixed    km/s
*    apef        - acceleration pseudo earth fixedkm/s2
*    direct      - direction of transfer          eFrom, 'TOO '
*    iau80arr    - record containing the iau80 constants rad
*    ttt         - julian centuries of tt         centuries
*    jdut1       - julian date of ut1             days from 4713 bc
*    lod         - excess length of day           sec
*    terms       - number of terms for ast calculation 0,2
		*    opt         - method option                           e80, e96, e00a, e00b
*
*  outputs       :
*    rgcrf        - position vector gcrf            km
*    vgcrf        - velocity vector gcrf            km/s
*    agcrf        - acceleration vector gcrf        km/s2
*
*  locals        :
*    prec        - matrix for gcrf - mod
*    deltapsi    - nutation angle                 rad
*    trueeps     - true obliquity of the ecliptic rad
*    meaneps     - mean obliquity of the ecliptic rad
*    omega       -                                rad
*    nut         - matrix for mod - tod
*    st          - matrix for tod - pef
*    stdot       - matrix for tod - pef rate
*
*  coupling      :
*   precess      - rotation for precession        mod - gcrf
*   nutation     - rotation for nutation          tod - mod
*   sidereal     - rotation for sidereal time     pef - tod
*
*  references    :
*    vallado       2013, 228, Alg 24
* --------------------------------------------------------------------------- */

	void pef_gcrf
	(
		double rpef[3], double vpef[3], double apef[3],
		MathTimeLib::edirection direct,
		double rgcrf[3], double vgcrf[3], double agcrf[3],
		const iau80data &iau80arr, eOpt opt,
		double ttt, double jdut1, double lod, int eqeterms,
		double ddpsi, double ddeps
	)
	{
		std::vector< std::vector<double> > prec, nut, st, stdot, temp, tempmat, stp, nutp, precp =
			std::vector< std::vector<double> >(3, std::vector<double>(3));
		double psia, wa, epsa, chia, deltapsi, deltaeps, trueeps, meaneps,
			omega, thetasa, omegaearth[3], omgxr[3], omgxomgxr[3],
			omgxv[3], tempvec1[3], tempvec[3];

		precess(ttt, opt, psia, wa, epsa, chia, prec);
		nutation(ttt, ddpsi, ddeps, iau80arr, opt, deltapsi, deltaeps, trueeps, meaneps, omega, nut);
		sidereal(jdut1, deltapsi, meaneps, omega, lod, eqeterms, opt, st, stdot);

		thetasa = 7.29211514670698e-05 * (1.0 - lod / 86400.0);
		omegaearth[0] = 0.0;
		omegaearth[1] = 0.0;
		omegaearth[2] = thetasa;

		if (direct == MathTimeLib::eTo)
		{
			MathTimeLib::matmult(prec, nut, temp, 3, 3, 3);
			MathTimeLib::matmult(temp, st, tempmat, 3, 3, 3);
			MathTimeLib::matvecmult(tempmat, rpef, rgcrf);

			MathTimeLib::cross(omegaearth, rpef, omgxr);
			MathTimeLib::addvec(1.0, vpef, 1.0, omgxr, tempvec1);
			MathTimeLib::matvecmult(tempmat, tempvec1, vgcrf);

			MathTimeLib::cross(omegaearth, omgxr, omgxomgxr);
			MathTimeLib::cross(omegaearth, vpef, omgxv);
			MathTimeLib::addvec(1.0, apef, 1.0, omgxomgxr, tempvec);
			MathTimeLib::addvec(1.0, tempvec, 2.0, omgxv, tempvec1);
			MathTimeLib::matvecmult(tempmat, tempvec1, agcrf);
		}
		else
		{
			MathTimeLib::mattrans(st, stp, 3, 3);
			MathTimeLib::mattrans(nut, nutp, 3, 3);
			MathTimeLib::mattrans(prec, precp, 3, 3);

			MathTimeLib::matmult(stp, nutp, temp, 3, 3, 3);
			MathTimeLib::matmult(temp, precp, tempmat, 3, 3, 3);
			MathTimeLib::matvecmult(tempmat, rgcrf, rpef);

			MathTimeLib::cross(omegaearth, rpef, omgxr);
			MathTimeLib::matvecmult(tempmat, vgcrf, tempvec1);
			MathTimeLib::addvec(1.0, tempvec1, -1.0, omgxr, vpef);

			MathTimeLib::addvec(1.0, tempvec1, -1.0, omgxr, vpef);
			MathTimeLib::cross(omegaearth, vpef, omgxv);
			MathTimeLib::cross(omegaearth, omgxr, omgxomgxr);
			MathTimeLib::matvecmult(tempmat, agcrf, tempvec1);
			MathTimeLib::addvec(1.0, tempvec1, -1.0, omgxomgxr, tempvec);
			MathTimeLib::addvec(1.0, tempvec, -2.0, omgxv, apef);
		}
	}  // procedure pef_gcrf

/* -----------------------------------------------------------------------------
*
*                           function tod_gcrf
*
*  this function transforms a vector between the true equator true equinox frame
*    of date (tod), and the mean equator mean equinox (j2000) frame.
*
*  author        : david vallado                  719-573-2600   25 jun 2002
*
*  revisions
*    vallado     - consolidate with iau 2000                     14 feb 2005
*    vallado     - conversion to c++                             21 feb 2005
*
*  inputs          description                    range / units
*    rtod        - position vector of date
*                    true equator, true equinox   km
*    vtod        - velocity vector of date
*                    true equator, true equinox   km/s
*    atod        - acceleration vector of date
*                    true equator, true equinox   km/s2
*    direct      - direction of transfer          eFrom, 'TOO '
*    iau80arr    - record containing the iau80 constants rad
*    ttt         - julian centuries of tt         centuries
		*    opt         - method option                           e80, e96, e00a, e00b
*
*  outputs       :
*    rgcrf        - position vector gcrf            km
*    vgcrf        - velocity vector gcrf            km/s
*    agcrf        - acceleration vector gcrf        km/s2
*
*  locals        :
*    deltapsi    - nutation angle                 rad
*    trueeps     - true obliquity of the ecliptic rad
*    meaneps     - mean obliquity of the ecliptic rad
*    omega       -                                rad
*    nut         - matrix for mod - tod
*
*  coupling      :
*   precess      - rotation for precession        mod - gcrf
*   nutation     - rotation for nutation          tod - mod
*
*  references    :
*    vallado       2013, 228, Alg 24
* ----------------------------------------------------------------------------*/

	void tod_gcrf
	(
		double rtod[3], double vtod[3], double atod[3],
		MathTimeLib::edirection direct,
		double rgcrf[3], double vgcrf[3], double agcrf[3],
		const iau80data &iau80arr, eOpt opt,
		double ttt, double ddpsi, double ddeps
	)
	{
		std::vector< std::vector<double> > prec, nut, nutp, precp = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > tempmat = std::vector< std::vector<double> >(3, std::vector<double>(3));
		double psia, wa, epsa, chia, deltapsi, deltaeps, trueeps, meaneps, omega;

		precess(ttt, opt, psia, wa, epsa, chia, prec);
		nutation(ttt, ddpsi, ddeps, iau80arr, opt, deltapsi, deltaeps, trueeps, meaneps, omega, nut);

		if (direct == MathTimeLib::eTo)
		{
			MathTimeLib::matmult(prec, nut, tempmat, 3, 3, 3);
			MathTimeLib::matvecmult(tempmat, rtod, rgcrf);
			MathTimeLib::matvecmult(tempmat, vtod, vgcrf);
			MathTimeLib::matvecmult(tempmat, atod, agcrf);
		}
		else
		{
			MathTimeLib::mattrans(nut, nutp, 3, 3);
			MathTimeLib::mattrans(prec, precp, 3, 3);
			MathTimeLib::matmult(nutp, precp, tempmat, 3, 3, 3);

			MathTimeLib::matvecmult(tempmat, rgcrf, rtod);
			MathTimeLib::matvecmult(tempmat, vgcrf, vtod);
			MathTimeLib::matvecmult(tempmat, agcrf, atod);
		}
	}  // procedure tod_gcrf

/* -----------------------------------------------------------------------------
*
*                           function mod_gcrf
*
*  this function transforms a vector between the mean equator mean equinox of
*    date (mod) and the mean equator mean equinox (j2000) frame.
*
*  author        : david vallado                  719-573-2600   25 jun 2002
*
*  revisions
*    vallado     - consolidate with iau 2000                     14 feb 2005
*    vallado     - conversion to c++                             21 feb 2005
*
*  inputs          description                    range / units
*    rmod        - position vector of date
*                    mean equator, mean equinox   km
*    vmod        - velocity vector of date
*                    mean equator, mean equinox   km/s
*    amod        - acceleration vector of date
*                    mean equator, mean equinox   km/s2
*    direct      - direction of transfer          eFrom, 'TOO '
*    ttt         - julian centuries of tt         centuries
*
*  outputs       :
*    rgcrf        - position vector gcrf            km
*    vgcrf        - velocity vector gcrf            km/s
*    agcrf        - acceleration vector gcrf        km/s2
*
*  locals        :
*    none.
*
*  coupling      :
*   precess      - rotation for precession        mod - gcrf
*
*  references    :
*    vallado       2013, 228, Alg 24
* --------------------------------------------------------------------------- */

	void mod_gcrf
	(
		double rmod[3], double vmod[3], double amod[3],
		MathTimeLib::edirection direct,
		double rgcrf[3], double vgcrf[3], double agcrf[3],
		double ttt
	)
	{
		std::vector< std::vector<double> > prec, precp;
		double psia, wa, epsa, chia;

		precess(ttt, e80, psia, wa, epsa, chia, prec);

		if (direct == MathTimeLib::eTo)
		{
			MathTimeLib::matvecmult(prec, rmod, rgcrf);
			MathTimeLib::matvecmult(prec, vmod, vgcrf);
			MathTimeLib::matvecmult(prec, amod, agcrf);
		}
		else
		{
			MathTimeLib::mattrans(prec, precp, 3, 3);

			MathTimeLib::matvecmult(precp, rgcrf, rmod);
			MathTimeLib::matvecmult(precp, vgcrf, vmod);
			MathTimeLib::matvecmult(precp, agcrf, amod);
		}
	}  // procedure mod_gcrf


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
*  inputs          description                    range / units
*    rteme        - position vector teme            km
*    vteme        - velocity vector teme            km / s
*    ateme        - acceleration vector teme        km / s2
*    direct       - direction of transfer           eFrom, 'TOO '
*    ttt          - julian centuries of tt          centuries
*    jdut1        - julian date of ut1              days from 4713 bc
*    lod          - excess length of day            sec
*    xp           - polar motion coefficient        rad
*    yp           - polar motion coefficient        rad
*    eqeterms     - use extra two terms(kinematic) after 1997  0, 2
*
*  outputs       :
*    recef        - position vector earth fixed     km
*    vecef        - velocity vector earth fixed     km / s
*    aecef        - acceleration vector earth fixed km / s2
*
*  locals :
*    st           - matrix for pef - tod
*    pm           - matrix for ecef - pef
*
*  coupling :
*   gstime        - greenwich mean sidereal time    rad
*   polarm        - rotation for polar motion       pef - ecef
*
*  references :
*    vallado       2013, 231 - 233
* ----------------------------------------------------------------------------*/

	void teme_ecef
	(
		double rteme[3], double vteme[3], double ateme[3],
		MathTimeLib::edirection direct,
		double recef[3], double vecef[3], double aecef[3],
		double ttt, double jdut1, double lod, double xp, double yp, int eqeterms
	)
	{
		double deg2rad, omega, gmstg, thetasa;
		//trans.resize(3);  // rows
		//for (std::vector< std::vector<double> >::iterator it = trans.begin(); it != trans.end(); ++it)
		//	it->resize(3);
		std::vector< std::vector<double> > st = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > stdot = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > temp = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > tempmat = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > pm, pmp, stp = std::vector< std::vector<double> >(3, std::vector<double>(3));
		double omegaearth[3], rpef[3], vpef[3], apef[3], omgxr[3], omgxomgxr[3],
			omgxv[3], tempvec1[3], tempvec[3], gmst;

		deg2rad = pi / 180.0;

		// find omeage from nutation theory
		omega = 125.04452222 + (-6962890.5390 *ttt + 7.455 *ttt*ttt + 0.008 *ttt*ttt*ttt) / 3600.0;
		omega = fmod(omega, 360.0) * deg2rad;

		// ------------------------find gmst--------------------------
		gmst = AstroLib::gstime(jdut1);

		// teme does not include the geometric terms here
		// after 1997, kinematic terms apply
		if ((jdut1 > 2450449.5) && (eqeterms > 0))
		{
			gmstg = gmst
				+ 0.00264*pi / (3600.0 * 180.0)*sin(omega)
				+ 0.000063*pi / (3600.0 * 180.0)*sin(2.0 *omega);
		}
		else
			gmstg = gmst;
		gmstg = fmod(gmstg, 2.0*pi);

		thetasa = 7.29211514670698e-05 * (1.0 - lod / 86400.0);
		omegaearth[0] = 0.0;
		omegaearth[1] = 0.0;
		omegaearth[2] = thetasa;

		st[0][0] = cos(gmstg);
		st[0][1] = -sin(gmstg);
		st[0][2] = 0.0;
		st[1][0] = sin(gmstg);
		st[1][1] = cos(gmstg);
		st[1][2] = 0.0;
		st[2][0] = 0.0;
		st[2][1] = 0.0;
		st[2][2] = 1.0;

		// compute sidereal time rate matrix
		stdot[0][0] = -omegaearth[2] * sin(gmstg);
		stdot[0][1] = -omegaearth[2] * cos(gmstg);
		stdot[0][2] = 0.0;
		stdot[1][0] = omegaearth[2] * cos(gmstg);
		stdot[1][1] = -omegaearth[2] * sin(gmstg);
		stdot[1][2] = 0.0;
		stdot[2][0] = 0.0;
		stdot[2][1] = 0.0;
		stdot[2][2] = 0.0;

		polarm(xp, yp, ttt, e80, pm);

		if (direct == MathTimeLib::eTo)
		{
			MathTimeLib::mattrans(pm, pmp, 3, 3);
			MathTimeLib::mattrans(st, stp, 3, 3);

			MathTimeLib::matvecmult(stp, rteme, rpef);
			MathTimeLib::matvecmult(pmp, rpef, recef);

			MathTimeLib::cross(omegaearth, rpef, omgxr);
			MathTimeLib::matvecmult(stp, vteme, tempvec1);
			MathTimeLib::addvec(1.0, tempvec1, -1.0, omgxr, vpef);
			MathTimeLib::matvecmult(pmp, vpef, vecef);

			MathTimeLib::addvec(1.0, tempvec1, -1.0, omgxr, vpef);
			MathTimeLib::cross(omegaearth, vpef, omgxv);
			MathTimeLib::cross(omegaearth, omgxr, omgxomgxr);
			MathTimeLib::matvecmult(stp, ateme, tempvec1);
			MathTimeLib::addvec(1.0, tempvec1, -1.0, omgxomgxr, tempvec);
			MathTimeLib::addvec(1.0, tempvec, -2.0, omgxv, apef);
			MathTimeLib::matvecmult(pmp, apef, aecef);
		}
		else
		{
			MathTimeLib::matvecmult(pm, recef, rpef);
			MathTimeLib::matvecmult(st, rpef, rteme);

			MathTimeLib::matvecmult(pm, vecef, vpef);
			MathTimeLib::cross(omegaearth, rpef, omgxr);
			MathTimeLib::addvec(1.0, vpef, 1.0, omgxr, tempvec1);
			MathTimeLib::matvecmult(st, tempvec1, vteme);

			MathTimeLib::matvecmult(pm, aecef, apef);
			MathTimeLib::cross(omegaearth, omgxr, omgxomgxr);
			MathTimeLib::cross(omegaearth, vpef, omgxv);
			MathTimeLib::addvec(1.0, apef, 1.0, omgxomgxr, tempvec);
			MathTimeLib::addvec(1.0, tempvec, 2.0, omgxv, tempvec1);
			MathTimeLib::matvecmult(st, tempvec1, ateme);
		}

		//fprintf(1, 'st gmst %11.8f ast %11.8f ome  %11.8f \n', gmst * 180 / pi, ast * 180 / pi, omegaearth * 180 / pi);

	}  // procedure teme_ecef




/* ----------------------------------------------------------------------------
*
*                           function teme_eci
*
*  this function transforms a vector from the true equator mean equinox system,
*  (teme) to the mean equator mean equinox (j2000) system.
*
*  author        : david vallado                  719 - 573 - 2600   30 oct 2017
*
*  inputs        description                    range / units
*    rteme       - position vector of date
*                  true equator, mean equinox      km
*    vteme       - velocity vector of date
*                  true equator, mean equinox      km / s
*    ateme       - acceleration vector of date
*                  true equator, mean equinox      km / s2
*    ttt         - julian centuries of tt          centuries
*    ddpsi       - delta psi correction to gcrf    rad
*    ddeps       - delta eps correction to gcrf    rad
		*    opt         - method option                           e80, e96, e00a, e00b
*
*  outputs       :
*    reci        - position vector eci             km
*    veci        - velocity vector eci             km / s
*    aeci        - acceleration vector eci         km / s2
*
*  locals :
*    prec        - matrix for eci - mod
*    nutteme     - matrix for mod - teme - an approximation for nutation
*    eqeg        - rotation for equation of equinoxes(geometric terms only)
*    tm          - combined matrix for teme2eci
*
*  coupling      :
*   precess      - rotation for precession         eci - mod
*   nutation     - rotation for nutation           eci - tod
*
*  references :
*    vallado       2013, 231 - 233
* ----------------------------------------------------------------------------*/

	void teme_eci
	(
		double rteme[3], double vteme[3], double ateme[3],
		MathTimeLib::edirection direct,
		double rgcrf[3], double vgcrf[3], double agcrf[3],
		const iau80data &iau80arr, eOpt opt,
		double ttt, double ddpsi, double ddeps
	)
	{
		std::vector< std::vector<double> > prec, nut, temp, tempmat, nutp, precp, eqep;
		std::vector< std::vector<double> > eqe = std::vector< std::vector<double> >(3, std::vector<double>(3));
		double psia, wa, epsa, chia, deltapsi, deltaeps, trueeps, meaneps, omega, eqeg;

		precess(ttt, e80, psia, wa, epsa, chia, prec);
		nutation(ttt, ddpsi, ddeps, iau80arr, opt, deltapsi, deltaeps, trueeps, meaneps, omega, nut);

		// ------------------------find eqeg----------------------
		// rotate teme through just geometric terms
		eqeg = deltapsi * cos(meaneps);
		eqeg = fmod(eqeg, twopi);

		eqe[0][0] = cos(eqeg);
		eqe[0][1] = sin(eqeg);
		eqe[0][2] = 0.0;
		eqe[1][0] = -sin(eqeg);
		eqe[1][1] = cos(eqeg);
		eqe[1][2] = 0.0;
		eqe[2][0] = 0.0;
		eqe[2][1] = 0.0;
		eqe[2][2] = 1.0;

		if (direct == MathTimeLib::eTo)
		{
			MathTimeLib::mattrans(eqe, eqep, 3, 3);

			MathTimeLib::matmult(nut, eqep, temp, 3, 3, 3);
			MathTimeLib::matmult(prec, temp, tempmat, 3, 3, 3);

			MathTimeLib::matvecmult(tempmat, rteme, rgcrf);
			MathTimeLib::matvecmult(tempmat, vteme, vgcrf);
			MathTimeLib::matvecmult(tempmat, ateme, agcrf);
		}
		else
		{
			MathTimeLib::mattrans(nut, nutp, 3, 3);
			MathTimeLib::mattrans(prec, precp, 3, 3);

			MathTimeLib::matmult(nutp, precp, temp, 3, 3, 3);
			MathTimeLib::matmult(eqe, temp, tempmat, 3, 3, 3);

			MathTimeLib::matvecmult(tempmat, rgcrf, rteme);
			MathTimeLib::matvecmult(tempmat, vgcrf, vteme);
			MathTimeLib::matvecmult(tempmat, agcrf, ateme);
		}

	}  //  teme_eci



	// -----------------------------------------------------------------------------------------
	//                                       2-body functions
	// -----------------------------------------------------------------------------------------


	/* -----------------------------------------------------------------------------
	*
	*                           function rv2coe
	*
	*  this function finds the classical orbital elements given the geocentric
	*    equatorial position and velocity vectors.
	*
	*  author        : david vallado                  719-573-2600   21 jun 2002
	*
	*  revisions
	*    vallado     - fix special cases                              5 sep 2002
	*    vallado     - delete extra check in inclination code        16 oct 2002
	*    vallado     - add constant file use                         29 jun 2003
	*
	*  inputs          description                              range / units
	*    r           - position vector                               km
	*    v           - velocity vector                            km / s
	*
	*  outputs       :
	*    p           - semilatus rectum                          km
	*    a           - semimajor axis                            km
	*    ecc         - eccentricity
	*    incl        - inclination                               0.0  to pi rad
	*    raan        - rtasc of ascending node                   0.0  to 2pi rad
	*    argp        - argument of perigee                       0.0  to 2pi rad
	*    nu          - true anomaly                              0.0  to 2pi rad
	*    m           - mean anomaly                              0.0  to 2pi rad
	*    eccanom     - eccentric, parabolic,
	*                  hyperbolic anomaly                        rad
	*    arglat      - argument of latitude      (ci)            0.0  to 2pi rad
	*    truelon     - true longitude            (ce)            0.0  to 2pi rad
	*    lonper      - longitude of periapsis    (ee)            0.0  to 2pi rad
	*
	*  locals        :
	*    hbar        - angular momentum h vector                 km2 / s
	*    ebar        - eccentricity     e vector
	*    nbar        - line of nodes    n vector
	*    c1          - v**2 - u/r
	*    rdotv       - r dot v
	*    hk          - hk unit vector
	*    sme         - specfic mechanical energy                 km2 / s2
	*    i           - index
	*    temp        - temporary variable
	*    typeorbit   - type of orbit                             ee, ei, ce, ci
	*
	*  coupling      :
	*    mag         - magnitude of a vector
	*    cross       - cross product of two vectors
	*    angle       - find the angle between two vectors
	*    newtonnu    - find the mean anomaly
	*
	*  references    :
	*    vallado       2013, 113, alg 9, ex 2-5
	* --------------------------------------------------------------------------- */

	void rv2coe
	(
		double r[3], double v[3],
		double& p, double& a, double& ecc, double& incl, double& raan, double& argp,
		double& nu, double& m, double& eccanom, double& arglat, double& truelon, double& lonper
	)
	{
		double small, hbar[3], nbar[3], magr, magv, magn, ebar[3], sme, rdotv, temp, c1, hk, magh, halfpi;

		int i;
		// switch this to an integer msvs seems to have problems with this and strncpy_s
		//char typeorbit[2];
		int typeorbit;
		// here 
		// typeorbit = 1 = 'ei'
		// typeorbit = 2 = 'ce'
		// typeorbit = 3 = 'ci'
		// typeorbit = 4 = 'ee'

		halfpi = 0.5 * pi;
		small = 0.00000001;
		eccanom = 0.0;

		// -------------------------  implementation   -----------------
		magr = MathTimeLib::mag(r);
		magv = MathTimeLib::mag(v);

		// ------------------  find h n and e vectors   ----------------
		MathTimeLib::cross(r, v, hbar);
		magh = MathTimeLib::mag(hbar);
		if (magh >= 0.0)
		{
			nbar[0] = -hbar[1];
			nbar[1] = hbar[0];
			nbar[2] = 0.0;
			magn = MathTimeLib::mag(nbar);
			c1 = magv * magv - mu / magr;
			rdotv = MathTimeLib::dot(r, v);
			temp = 1.0 / mu;
			for (i = 0; i <= 2; i++)
				ebar[i] = (c1 * r[i] - rdotv * v[i]) * temp;
			ecc = MathTimeLib::mag(ebar);

			// ------------  find a e and semi-latus rectum   ----------
			sme = (magv * magv * 0.5) - (mu / magr);
			if (fabs(sme) > small)
				a = -mu / (2.0 * sme);
			else
				a = infinite;
			p = magh * magh * temp;

			// -----------------  find inclination   -------------------
			hk = hbar[2] / magh;
			incl = acos(hk);

			// --------  determine type of orbit for later use  --------
			// ------ elliptical, parabolic, hyperbolic inclined -------
			//#ifdef _MSC_VER  // chk if compiling under MSVS
			//		   strcpy_s(typeorbit, 2 * sizeof(char), "ei");
			//#else
			//		   strcpy(typeorbit, "ei");
			//#endif
			typeorbit = 1;

			if (ecc < small)
			{
				// ----------------  circular equatorial ---------------
				if ((incl < small) | (fabs(incl - pi) < small))
				{
					//#ifdef _MSC_VER
					//				   strcpy_s(typeorbit, sizeof(typeorbit), "ce");
					//#else
					//				   strcpy(typeorbit, "ce");
					//#endif
					typeorbit = 2;
				}
				else
				{
					// --------------  circular inclined ---------------
					//#ifdef _MSC_VER
					//				   strcpy_s(typeorbit, sizeof(typeorbit), "ci");
					//#else
					//				   strcpy(typeorbit, "ci");
					//#endif
					typeorbit = 3;
				}
			}
			else
			{
				// - elliptical, parabolic, hyperbolic equatorial --
				if ((incl < small) | (fabs(incl - pi) < small))
				{
					//#ifdef _MSC_VER
					//				   strcpy_s(typeorbit, sizeof(typeorbit), "ee");
					//#else
					//				   strcpy(typeorbit, "ee");
					//#endif
					typeorbit = 4;
				}
			}

			// ----------  find right ascension of the ascending node ------------
			if (magn > small)
			{
				temp = nbar[0] / magn;
				if (fabs(temp) > 1.0)
					temp = MathTimeLib::sgn(temp);
				raan = acos(temp);
				if (nbar[1] < 0.0)
					raan = twopi - raan;
			}
			else
				raan = undefined;

			// ---------------- find argument of perigee ---------------
			//if (strcmp(typeorbit, "ei") == 0)
			if (typeorbit == 1)
			{
				argp = MathTimeLib::angle(nbar, ebar);
				if (ebar[2] < 0.0)
					argp = twopi - argp;
			}
			else
				argp = undefined;

			// ------------  find true anomaly at epoch    -------------
			//if (typeorbit[0] == 'e')
			if ((typeorbit == 1) || (typeorbit == 4))
			{
				nu = MathTimeLib::angle(ebar, r);
				if (rdotv < 0.0)
					nu = twopi - nu;
			}
			else
				nu = undefined;

			// ----  find argument of latitude - circular inclined -----
			//if (strcmp(typeorbit, "ci") == 0)
			if (typeorbit == 3)
			{
				arglat = MathTimeLib::angle(nbar, r);
				if (r[2] < 0.0)
					arglat = twopi - arglat;
				m = arglat;
			}
			else
				arglat = undefined;

			// -- find longitude of perigee - elliptical equatorial ----
			//if ((ecc>small) && (strcmp(typeorbit, "ee") == 0))
			if ((ecc > small) && (typeorbit == 4))
			{
				temp = ebar[0] / ecc;
				if (fabs(temp) > 1.0)
					temp = MathTimeLib::sgn(temp);
				lonper = acos(temp);
				if (ebar[1] < 0.0)
					lonper = twopi - lonper;
				if (incl > halfpi)
					lonper = twopi - lonper;
			}
			else
				lonper = undefined;

			// -------- find true longitude - circular equatorial ------
			//if ((magr>small) && (strcmp(typeorbit, "ce") == 0))
			if ((magr > small) && (typeorbit == 2))
			{
				temp = r[0] / magr;
				if (fabs(temp) > 1.0)
					temp = MathTimeLib::sgn(temp);
				truelon = acos(temp);
				if (r[1] < 0.0)
					truelon = twopi - truelon;
				if (incl > halfpi)
					truelon = twopi - truelon;
				m = truelon;
			}
			else
				truelon = undefined;

			// ------------ find mean anomaly for all orbits -----------
			//if (typeorbit[0] == 'e')
			if ((typeorbit == 1) || (typeorbit == 4))
				newtonnu(ecc, nu, eccanom, m);
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
	*    equatorial (ijk) system given the classical orbit elements.
	*
	*  author        : david vallado                  719-573-2600    1 mar 2001
	*
	*  inputs          description                          range / units
	*    p           - semilatus rectum                          km
	*    ecc         - eccentricity
	*    incl        - inclination                               0.0 to pi rad
	*    raan        - rtasc of ascending node                   0.0 to 2pi rad
	*    argp        - argument of perigee                       0.0 to 2pi rad
	*    nu          - true anomaly                              0.0 to 2pi rad
	*    arglat      - argument of latitude      (ci)            0.0 to 2pi rad
	*    lamtrue     - true longitude            (ce)            0.0 to 2pi rad
	*    lonper      - longitude of periapsis    (ee)            0.0 to 2pi rad
	*
	*  outputs       :
	*    r           -  position vector                           km
	*    v           -  velocity vector                           km / s
	*
	*  locals        :
	*    temp        - temporary real*8 value
	*    rpqw        - pqw position vector                        km
	*    vpqw        - pqw velocity vector                        km / s
	*    sinnu       - sine of nu
	*    cosnu       - cosine of nu
	*    tempvec     - pqw velocity vector
	*
	*  coupling      :
	*    rot3        - rotation about the 3rd axis
	*    rot1        - rotation about the 1st axis
	*
	*  references    :
	*    vallado       2013, 118, alg 10, ex 2-5
	* --------------------------------------------------------------------------- */

	void coe2rv
	(
		double p, double ecc, double incl, double raan, double argp, double nu,
		double arglat, double truelon, double lonper,
		double r[3], double v[3]
	)
	{
		double rpqw[3], vpqw[3], tempvec[3], temp, sinnu, cosnu, small;

		small = 0.0000001;

		// --------------------  implementation   ----------------------
		//       determine what type of orbit is involved and set up the
		//       set up angles for the special cases.
		// -------------------------------------------------------------
		if (ecc < small)
		{
			// ----------------  circular equatorial  ------------------
			if ((incl < small) | (fabs(incl - pi) < small))
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
			if ((incl < small) | (fabs(incl - pi) < small))
			{
				argp = lonper;
				raan = 0.0;
			}
		}

		// ----------  form pqw position and velocity vectors ----------
		cosnu = cos(nu);
		sinnu = sin(nu);
		temp = p / (1.0 + ecc * cosnu);
		rpqw[0] = temp * cosnu;
		rpqw[1] = temp * sinnu;
		rpqw[2] = 0.0;
		if (fabs(p) < 0.00000001)
			p = 0.00000001;

		vpqw[0] = -sinnu * sqrt(mu / p);
		vpqw[1] = (ecc + cosnu) * sqrt(mu / p);
		vpqw[2] = 0.0;

		// ----------------  perform transformation to ijk  ------------
		MathTimeLib::rot3(rpqw, -argp, tempvec);
		MathTimeLib::rot1(tempvec, -incl, tempvec);
		MathTimeLib::rot3(tempvec, -raan, r);

		MathTimeLib::rot3(vpqw, -argp, tempvec);
		MathTimeLib::rot1(tempvec, -incl, tempvec);
		MathTimeLib::rot3(tempvec, -raan, v);
	}  // coe2rv


	/* ----------------------------------------------------------------------------
	*
	*                           function rv2eq.m
	*
	*  this function transforms a position and velocity vector into the flight
	*    elements - latgc, lon, fpa, az, position and velocity magnitude.
	*
	*  author        : david vallado                  719 - 573 - 2600    7 jun 2002
	*
	*  inputs          description                         range / units
	*    r           - eci position vector                      km
	*    v           - eci velocity vector                      km / s
	*
	*  outputs       :
	*    n           - mean motion                               rad
	*    a           - semi major axis                           km
	*    af          - component of ecc vector
	*    ag          - component of ecc vector
	*    chi         - component of node vector in eqw
	*    psi         - component of node vector in eqw
	*    meanlon     - mean longitude                            rad
	*    truelon     - true longitude                            rad
	*
	*  locals        :
	*    none -
	*
	*  coupling :
	*    none -
	*
	*  references :
	*    vallado       2013, 108
	*    chobotov            30
	---------------------------------------------------------------------------- */

	void rv2eq
	(
		double r[3], double v[3],
		double& a, double& n, double& af, double& ag, double& chi, double& psi, double& meanlonM,
		double& meanlonNu, int& fr
	)
	{
		double p, ecc, incl, raan, argp, nu, m, eccanom, arglat, truelon, lonper, small;
		small = 0.00000001;

		// --------convert to classical elements----------------------
		rv2coe(r, v, p, a, ecc, incl, raan, argp, nu, m, eccanom, arglat, truelon, lonper);

		// --------setup retrograde factor----------------------------
		fr = 1;
		// ----------set this so it for orbits over 90 deg !!-------- -
		if (incl > pi * 0.5)
			fr = -1;

		if (ecc < small)
		{
			// ----------------circular equatorial------------------
			if ((incl < small) || (abs(incl - pi) < small))
			{
				argp = 0.0;
				raan = 0.0;
				// nu = truelon;
			}
			else
			{
				// --------------circular inclined------------------
				argp = 0.0;
			}
			//  nu = arglat;
		}
		else
		{
			// -------------- - elliptical equatorial---------------- -
			if ((incl < small) || (abs(incl - pi) < small))
			{
				argp = lonper;
				raan = 0.0;
			}
		}

		af = ecc * cos(fr*raan + argp);
		ag = ecc * sin(fr*raan + argp);

		if (fr > 0)
		{
			chi = tan(incl*0.5) * sin(raan);
			psi = tan(incl*0.5) * cos(raan);
		}
		else
		{
			chi = MathTimeLib::cot(incl*0.5) * sin(raan);
			psi = MathTimeLib::cot(incl*0.5) * cos(raan);
		}

		n = sqrt(mu / (a * a * a));

		meanlonM = fr * raan + argp + m;
		meanlonM = fmod(meanlonM, twopi);

		meanlonNu = fr * raan + argp + nu;
		meanlonNu = fmod(meanlonNu, twopi);
	} // rv2eq


	/* ------------------------------------------------------------------------------
	*
	*                           function eq2rv
	*
	*  this function finds the classical orbital elements given the equinoctial
	*   elements.
	*
	*  author        : david vallado                  719 - 573 - 2600    9 jun 2002
	*
	*  revisions
	*    vallado - fix elliptical equatorial orbits case         19 oct 2002
	* vallado - add constant file use                         29 jun 2003
	*
	*  inputs          description                           range / units
	*    n           - mean motion                               rad
	*    a           - semi major axis                           km
	*    af          - component of ecc vector
	*    ag          - component of ecc vector
	*    chi         - component of node vector in eqw
	*    psi         - component of node vector in eqw
	*    meanlon     - mean longitude                            rad
	*    truelon     - true longitude                            rad
	*
	*  outputs       :
	*    r           - position vector                           km
	*    v           - velocity vector                           km / s
	*
	*  locals :
	*    temp        - temporary variable
	*    p           - semilatus rectum                          km
	*    ecc         - eccentricity
	*    incl        - inclination                               0.0  to pi rad
	*    raan        - rtasc of ascending node                   0.0  to 2pi rad
	*    argp        - argument of perigee                       0.0  to 2pi rad
	*    nu          - true anomaly                              0.0  to 2pi rad
	*    m           - mean anomaly                              0.0  to 2pi rad
	*    arglat      - argument of latitude(ci)                  0.0  to 2pi rad
	*    truelon     - true longitude(ce)                        0.0  to 2pi rad
	*    lonper      - longitude of periapsis(ee)                0.0  to 2pi rad
	*
	*  coupling      :
	*
	*  references :
	*    vallado 2013 : 108
	------------------------------------------------------------------------------ */

	void eq2rv
	(
		double a, double af, double ag, double chi, double psi, double meanlon, int fr,
		double r[3], double v[3]
	)
	{
		double p, ecc, incl, raan, argp, nu, m, arglat, truelon, lonper, small, e0;
		small = 0.00000001;

		// ------------------------ - implementation---------------- 
		arglat = 999999.1;
		lonper = 999999.1;
		truelon = 999999.1;

		// ---- if n is input----
		//a = (mu / n ^ 2) ^ (1.0 / 3.0);

		ecc = sqrt(af * af + ag * ag);
		p = a * (1.0 - ecc * ecc);
		incl = pi * ((1.0 - fr) * 0.5) + 2.0 * fr * atan(sqrt(chi * chi + psi * psi));
		raan = atan2(chi, psi);
		argp = atan2(ag, af) - fr * atan2(chi, psi);

		if (ecc < small)
		{
			// ----------------circular equatorial------------------
			if ((incl < small) || (abs(incl - pi) < small))
			{
				argp = 0.0;
				raan = 0.0;
				// truelon = nu;
			}
			else
			{
				// --------------circular inclined------------------
				argp = 0.0;
				// arglat = nu;
			}
		}
		else
		{
			// -------------- - elliptical equatorial---------------- 
			if ((incl < small) || (abs(incl - pi) < small))
			{
				// argp = lonper;
				raan = 0.0;
			}
		}

		m = meanlon - fr * raan - argp;
		m = fmod(m + twopi, twopi);

		newtonm(ecc, m, e0, nu);

		// ----------fix for elliptical equatorial orbits------------
		if (ecc < small)
		{
			// ----------------circular equatorial------------------
			if ((incl < small) || (abs(incl - pi) < small))
			{
				argp = undefined;
				raan = undefined;
				truelon = nu;
			}
			else
			{
				// --------------circular inclined------------------
				argp = undefined;
				arglat = nu;
			}
			nu = undefined;
		}
		else
		{
			// -------------- - elliptical equatorial---------------- -
			if ((incl < small) || (abs(incl - pi) < small))
			{
				lonper = argp;
				argp = undefined;
				raan = undefined;
			}
		}

		// --------now convert back to position and velocity vectors
		coe2rv(p, ecc, incl, raan, argp, nu, arglat, truelon, lonper, r, v);
	}  // eq2rv


	/* ------------------------------------------------------------------------------
	*
	*                           function eq2coe
	*
	*  this function finds the classical orbital elements given the equinoctial
	*   elements.
	*
	*  author        : david vallado                  719 - 573 - 2600    9 jun 2002
	*
	*  revisions
	*    vallado - fix elliptical equatorial orbits case         19 oct 2002
	* vallado - add constant file use                         29 jun 2003
	*
	*  inputs          description                          range / units
	*    n           - mean motion                               rad
	*    a           - semi major axis                           km
	*    af          - component of ecc vector
	*    ag          - component of ecc vector
	*    chi         - component of node vector in eqw
	*    psi         - component of node vector in eqw
	*    meanlon     - mean longitude                            rad
	*    truelon     - true longitude                            rad
	*
	*  outputs       :
	*    p           - semilatus rectum                          km
	*    ecc         - eccentricity
	*    incl        - inclination                               0.0  to pi rad
	*    raan        - rtasc of ascending node                   0.0  to 2pi rad
	*    argp        - argument of perigee                       0.0  to 2pi rad
	*    nu          - true anomaly                              0.0  to 2pi rad
	*    m           - mean anomaly                              0.0  to 2pi rad
	*    arglat      - argument of latitude(ci)                  0.0  to 2pi rad
	*    truelon     - true longitude(ce)                        0.0  to 2pi rad
	*    lonper      - longitude of periapsis(ee)                0.0  to 2pi rad
	*
	*  locals :
	*    temp - temporary variable
	*
	*  coupling      :
	*
	*  references :
	*    vallado 2013 : 108
	------------------------------------------------------------------------------ */

	void eq2coe
	(
		double x1in, double af, double ag, double chi, double psi, double x2in, int fr, equintype typeorbit,
		double& p, double& ecc, double& incl, double& raan, double& argp, double& nu, double& m,
		double& eccanom, double& arglat, double& truelon, double& lonper
	)
	{
		double a;
		double small;
		small = 0.00000001;

		// ------------------------ - implementation---------------- 
		arglat = 999999.1;
		lonper = 999999.1;
		truelon = 999999.1;

		ecc = sqrt(af * af + ag * ag);

		// use int for typeorbit, c++ can't do strings!
		// 1 a_m
		// 2 a_nu
		// 3 n_m
		// 4 n_nu
		// 5 p_m
		// 6 p_nu
		if (typeorbit == ea_m || typeorbit == ea_nu)  // a is input
		{
			a = x1in;
			p = a * (1.0 - ecc * ecc);
		}
		if (typeorbit == en_m || typeorbit == en_nu)  // n is input
		{
			a = pow(mu / pow(x1in, 2), 1.0 / 3.0);
			p = a * (1.0 - ecc * ecc);
		}
		if (typeorbit == ep_m || typeorbit == ep_nu)  // p is input
			p = x1in;

		incl = pi * ((1.0 - fr) * 0.5) + 2.0 * fr * atan(sqrt(chi * chi + psi * psi));
		raan = atan2(chi, psi);
		argp = atan2(ag, af) - fr * atan2(chi, psi);


		if (typeorbit == ea_m || typeorbit == en_m || typeorbit == ep_m)  // m is input
		{
			m = x2in;
			m = m - fr * raan - argp;
			m = fmod(m + twopi, twopi);
			newtonm(ecc, m, eccanom, nu);
		}

		if (typeorbit == ea_nu || typeorbit == en_nu || typeorbit == ep_nu)  // nu is input
		{
			nu = x2in;
			newtonnu(ecc, nu, eccanom, m);
		}

		// ----------fix for elliptical equatorial orbits------------
		if (ecc < small)
		{
			// ----------------circular equatorial------------------
			if ((incl < small) || (abs(incl - pi) < small))
			{
				argp = undefined;
				raan = undefined;
				truelon = nu;
			}
			else
			{
				// --------------circular inclined------------------
				argp = undefined;
				arglat = nu;
			}
			nu = undefined;
		}
		else
		{
			// -------------- - elliptical equatorial---------------- -
			if ((incl < small) || (abs(incl - pi) < small))
			{
				lonper = argp;
				argp = undefined;
				raan = undefined;
			}
		}
	}  // eq2coe


		/*------------------------------------------------------------------------------
	*
	*                           procedure radec_azel
	*
	* this procedure converts right ascension declination values with
	*   azimuth, and elevation.  notice the range is not defined because
	*   right ascension declination only allows a unit vector to be formed.
	*
	*  author        : david vallado                  719-573-2600   22 jun 2002
	*
	*  inputs          description                             range / units
	*    rtasc       - right ascension                          0.0 to 2pi rad
	*    decl        - declination                              -pi/2 to pi/2 rad
	*    lst         - local sidereal time                     -2pi to 2pi rad
	*    latgd       - geodetic latitude                       -pi/2 to pi/2 rad
	*    direct      -  direction to convert                     eFrom  eTo
	*
	*  outputs       :
	*    az          - azimuth                                   0.0 to 2pi rad
	*    el          - elevation                                -pi/2 to pi/2 rad
	*
	*  locals        :
	*    lha         - local hour angle                          -2pi to 2pi rad
	*    sinv        - sine value
	*    cosv        - cosine value
	*
	*  coupling      :
	*    arcsin      - arc sine function
	*    atan2       - arc tangent function that resolves quadrant ambiguites
	*
	*  references    :
	*    vallado       2013, 265, alg 27
	-----------------------------------------------------------------------------*/

	void radec_azel
	(
		double& rtasc, double& decl, double& lst, double& latgd,
		MathTimeLib::edirection direct,
		double& az, double& el
	)
	{
		double sinv, cosv, lha;
		if (direct == MathTimeLib::eFrom)
		{
			decl = asin(sin(el) * sin(latgd) + cos(el) * cos(latgd) * cos(az));

			sinv = -(sin(az) * cos(el) * cos(latgd)) / (cos(latgd) * cos(decl));
			cosv = (sin(el) - sin(latgd) * sin(decl)) / (cos(latgd) * cos(decl));
			lha = atan2(sinv, cosv);
			rtasc = lst - lha;
		}
		else
		{
			lha = lst - rtasc;
			el = asin(sin(decl) * sin(latgd) + cos(decl) * cos(latgd) * cos(lha));
			sinv = -sin(lha) * cos(decl) * cos(latgd) / (cos(el) * cos(latgd));
			cosv = (sin(decl) - sin(el) * sin(latgd)) / (cos(el) * cos(latgd));
			az = atan2(sinv, cosv);
		}

		//  if (show == 'y')
		//    if (fileout != null)
		//      fprintf(fileout, "%f\n", lha * 180.0 / pi);
	} // procedure radec_azel


	/*------------------------------------------------------------------------------
	*
	*                           procedure radec_elatlon
	*
	*  this procedure converts right-ascension declination values with ecliptic
	*    latitude and longitude values.
	*
	*  author        : david vallado                  719-573-2600   22 jun 2002
	*
	*  inputs          description                               range / units
	*    rtasc       - right ascension                               rad
	*    decl        - declination                                   rad
	*    direct      -  direction to convert                         eFrom  eTo
	*
	*  outputs       :
	*    ecllat      - ecliptic latitude                             -pi/2 to pi/2 rad
	*    ecllon      - ecliptic longitude                            -pi/2 to pi/2 rad
	*
	*  locals        :
	*    obliquity   - obliquity of the ecliptic                     rad
	*    sinv        -
	*    cosv        -
	*
	*  coupling      :
	*    arcsin      - arc sine function
	*    atan2       - arc tangent function that resolves quadrant ambiguites
	*
	*  references    :
	*    vallado       2013, 270, eq 4-19, eq 4-20
	-----------------------------------------------------------------------------*/

	void radec_elatlon
	(
		double& rtasc, double& decl,
		MathTimeLib::edirection direct,
		double& ecllat, double& ecllon
	)
	{
		double sinv, cosv, obliquity;

		obliquity = 0.40909280; // 23.439291/rad
		if (direct == MathTimeLib::eFrom)
		{
			decl = asin(sin(ecllat) * cos(obliquity) +
				cos(ecllat) * sin(obliquity) * sin(ecllon));
			sinv = (-sin(ecllat) * sin(obliquity) +
				cos(ecllat) * cos(obliquity) * sin(ecllon)) / cos(decl);
			cosv = cos(ecllat) * cos(ecllon) / cos(decl);
			rtasc = atan2(sinv, cosv);
		}
		else
		{
			ecllat = asin(-cos(decl) * sin(rtasc) * sin(obliquity) +
				sin(decl) * cos(obliquity));
			sinv = (cos(decl) * sin(rtasc) * cos(obliquity) +
				sin(decl) * sin(obliquity)) / cos(ecllat);
			cosv = cos(decl) * cos(rtasc) / cos(ecllat);
			ecllon = atan2(sinv, cosv);
		}
	}  // radec_elatlon


	/*------------------------------------------------------------------------------
	*
	*                           procedure rv_elatlon
	*
	*  this procedure converts ecliptic latitude and longitude with position and
	*    velocity vectors. uses velocity vector to find the solution of singular
	*    cases.
	*
	*  author        : david vallado                  719-573-2600   22 jun 2002
	*
	*  inputs          description                             range / units
	*    r        -  position vector                           er
	*    v        -  velocity vector                           er/tu
	*    direction   - which set of vars to output                   from  too
	*
	*  outputs       :
	*    rr          - radius of the sat                             er
	*    ecllat      - ecliptic latitude                            -pi/2 to pi/2 rad
	*    ecllon      - ecliptic longitude                            -pi/2 to pi/2 rad
	*    drr         - radius of the sat rate                        er/tu
	*    decllat     - ecliptic latitude rate                        -pi/2 to pi/2 rad
	*    eecllon     - ecliptic longitude rate                       -pi/2 to pi/2 rad
	*
	*  locals        :
	*    obliquity   - obliquity of the ecliptic                     rad
	*    temp        -
	*    temp1       -
	*    recl          - position vec in eclitpic frame
	*    vecl          - velocity vec in ecliptic frame
	*
	*  coupling      :
	*    mag         - magnitude of a vector
	*    rot1        - rotation about 1st axis
	*    dot         - dot product
	*    arcsin      - arc sine function
	*    atan2       - arc tangent function that resolves quadrant ambiguites
	*
	*  references    :
	*    vallado       2013, 268, eq 4-15
	-----------------------------------------------------------------------------*/

	void rv_elatlon
	(
		double r[3], double v[3],
		MathTimeLib::edirection direct,
		double& rr, double& ecllat, double& ecllon,
		double& drr, double& decllat, double& decllon
	)
	{
		const double small = 0.00000001;
		double recl[3], vecl[3];
		double   obliquity, temp, temp1;

		obliquity = 0.40909280; // 23.439291/rad
		if (direct == MathTimeLib::eFrom)
		{
			recl[0] = (rr * cos(ecllat) * cos(ecllon));
			recl[1] = (rr * cos(ecllat) * sin(ecllon));
			recl[2] = (rr               * sin(ecllon));
			vecl[0] = (drr * cos(ecllat) * cos(ecllon) -
				rr * sin(ecllat) * cos(ecllon) * decllat -
				rr * cos(ecllat) * sin(ecllon) * decllon);
			vecl[1] = (drr * cos(ecllat) * sin(ecllon) -
				rr * sin(ecllat) * cos(ecllon) * decllat +
				rr * cos(ecllat) * cos(ecllon) * decllon);
			vecl[2] = (drr * sin(ecllat) + rr * cos(ecllat) * decllat);

			MathTimeLib::rot1(recl, -obliquity, r);
			MathTimeLib::rot1(vecl, -obliquity, v);
		}
		else
		{
			/* -------------- calculate angles and rates ---------------- */
			rr = MathTimeLib::mag(recl);
			temp = sqrt(recl[0] * recl[0] + recl[1] * recl[1]);
			if (temp < small)
			{
				temp1 = sqrt(vecl[0] * vecl[0] + vecl[1] * vecl[1]);
				if (fabs(temp1) > small)
					ecllon = atan2(vecl[1] / temp1, vecl[0] / temp1);
				else
					ecllon = 0.0;
			}
			else
				ecllon = atan2(recl[1] / temp, recl[0] / temp);
			ecllat = asin(recl[2] / MathTimeLib::mag(recl));

			temp1 = -recl[1] * recl[1] - recl[0] * recl[0]; // different now
			drr = MathTimeLib::dot(recl, vecl) / rr;
			if (fabs(temp1) > small)
				decllon = (vecl[0] * recl[1] - vecl[1] * recl[0]) / temp1;
			else
				decllon = 0.0;
			if (fabs(temp) > small)
				decllat = (vecl[2] - drr * sin(ecllat)) / temp;
			else
				decllat = 0.0;
		}
	} // procedure rv_elatlon


		/* ------------------------------------------------------------------------------
		*
		*                            function rv2radec
		*
		*  this function converts the right ascension and declination values with
		*    position and velocity vectors of a satellite. uses velocity vector to
		*    find the solution of singular cases.
		*
		*  author        : david vallado                  719-573-2600   22 jun 2002
		*
		*  inputs          description                              range / units
		*    r           - position vector eci                          km, er
		*    v           - velocity vector eci                          km/s, er/tu
		*    direct      - direction to convert                         eFrom  eTo
		*
		*  outputs       :
		*    rr          - radius of the satellite                      km
		*    rtasc       - right ascension                              rad
		*    decl        - declination                                  rad
		*    drr         - radius of the satellite rate                 km/s
		*    drtasc      - right ascension rate                         rad/s
		*    ddecl       - declination rate                             rad/s
		*
		*  locals        :
		*    temp        - temporary position vector
		*    temp1       - temporary variable
		*
		*  coupling      :
		*     none
		*
		*  references    :
		*    vallado       2013, 259, alg 25
		-----------------------------------------------------------------------------*/

	void rv_radec
	(
		double r[3], double v[3],
		MathTimeLib::edirection direct,
		double& rr, double& rtasc, double& decl, double& drr, double& drtasc, double& ddecl
	)
	{
		const double small = 0.00000001;
		double temp, temp1;

		if (direct == MathTimeLib::eFrom)
		{
			r[0] = (rr * cos(decl) * cos(rtasc));
			r[1] = (rr * cos(decl) * sin(rtasc));
			r[2] = (rr * sin(decl));
			v[0] = (drr * cos(decl) * cos(rtasc) -
				rr * sin(decl) * cos(rtasc) * ddecl -
				rr * cos(decl) * sin(rtasc) * drtasc);
			v[1] = (drr * cos(decl) * sin(rtasc) -
				rr * sin(decl) * sin(rtasc) * ddecl +
				rr * cos(decl) * cos(rtasc) * drtasc);
			v[2] = (drr * sin(decl) + rr * cos(decl) * ddecl);
		}
		else
		{
			// -------------- calculate angles and rates ---------------- 
			rr = MathTimeLib::mag(r);
			temp = sqrt(r[0] * r[0] + r[1] * r[1]);
			if (temp < small)
			{
				temp1 = sqrt(v[0] * v[0] + v[1] * v[1]);
				if (fabs(temp1) > small)
					rtasc = atan2(v[1] / temp1, v[0] / temp1);
				else
					rtasc = 0.0;
			}
			else
				rtasc = atan2(r[1] / temp, r[0] / temp);

			decl = asin(r[2] / rr);

			temp1 = -r[1] * r[1] - r[0] * r[0];
			drr = MathTimeLib::dot(r, v) / rr;
			if (fabs(temp1) > small)
				drtasc = (v[0] * r[1] - v[1] * r[0]) / temp1;
			else
				drtasc = 0.0;
			if (fabs(temp) > small)
				ddecl = (v[2] - drr * sin(decl)) / temp;
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
	*  inputs          description                              range / units
	*    recef       - ecef position vector                          km
	*    vecef       - ecef velocity vector                          km/s
	*    rsecef      - ecef site position vector                     km
	*    latgd       - geodetic latitude                             -pi/2 to pi/2 rad
	*    lon         - geodetic longitude                            -2pi to pi rad
	*    direct      -  direction to convert                         eFrom  eTo
	*
	*  outputs       :
	*    rho         - satellite range from site                     km
	*    az          - azimuth                                       0.0 to 2pi rad
	*    el          - elevation                                     -pi/2 to pi/2 rad
	*    drho        - range rate                                    km/s
	*    daz         - azimuth rate                                  rad/s
	*    del         - elevation rate                                rad/s
	*
	*  locals        :
	*    rhovecef    - ecef range vector from site                   km
	*    drhovecef   - ecef velocity vector from site                km/s
	*    rhosez      - sez range vector from site                    km
	*    drhosez     - sez velocity vector from site                 km
	*    tempvec     - temporary vector
	*    temp        - temporary extended value
	*    temp1       - temporary extended value
	*    i           - index
	*
	*  coupling      :
	*    mag         - MathTimeLib::magnitude of a vector
	*    addvec      - add two vectors
	*    rot3        - rotation about the 3rd axis
	*    rot2        - rotation about the 2nd axis
	*    atan2       - arc tangent function which also resloves quadrants
	*    dot         - dot product of two vectors
	*    rvsez_razel - find r2 and v2 from site in topocentric horizon (sez) system
	*    lncom2      - combine two vectors and constants
	*    arcsin      - arc sine function
	*    sgn         - returns the MathTimeLib::sgn of a variable
	*
	*  references    :
	*    vallado       2013, 265, alg 27
	-----------------------------------------------------------------------------*/

	void rv_razel
	(
		double recef[3], double vecef[3], double rsecef[3], double latgd, double lon,
		MathTimeLib::edirection direct,
		double& rho, double& az, double& el, double& drho, double& daz, double& del
	)
	{
		const double halfpi = pi / 2.0;
		const double small = 0.0000001;

		double temp, temp1;
		double rhoecef[3], drhoecef[3], rhosez[3], drhosez[3], tempvec[3];

		if (direct == MathTimeLib::eFrom)
		{
			/* ---------  find sez range and velocity vectors ----------- */
			rvsez_razel(rhosez, drhosez, direct, rho, az, el, drho, daz, del);

			/* ----------  perform sez to ecef transformation ------------ */
			MathTimeLib::rot2(rhosez, latgd - halfpi, tempvec);
			MathTimeLib::rot3(tempvec, -lon, rhoecef);
			MathTimeLib::rot2(drhosez, latgd - halfpi, tempvec);
			MathTimeLib::rot3(tempvec, -lon, drhoecef);

			/* ---------  find ecef range and velocity vectors -----------*/
			MathTimeLib::addvec(1.0, rhoecef, 1.0, rsecef, recef);
			vecef[0] = drhoecef[0];
			vecef[1] = drhoecef[1];
			vecef[2] = drhoecef[2];
		}
		else
		{
			/* ------- find ecef range vector from site to satellite ----- */
			MathTimeLib::addvec(1.0, recef, -1.0, rsecef, rhoecef);
			drhoecef[0] = vecef[0];
			drhoecef[1] = vecef[1];
			drhoecef[2] = vecef[2];
			rho = MathTimeLib::mag(rhoecef);

			/* ------------ convert to sez for calculations ------------- */
			MathTimeLib::rot3(rhoecef, lon, tempvec);
			MathTimeLib::rot2(tempvec, halfpi - latgd, rhosez);
			MathTimeLib::rot3(drhoecef, lon, tempvec);
			MathTimeLib::rot2(tempvec, halfpi - latgd, drhosez);

			/* ------------ calculate azimuth and elevation ------------- */
			temp = sqrt(rhosez[0] * rhosez[0] + rhosez[1] * rhosez[1]);
			if (fabs(rhosez[1]) < small)
				if (temp < small)
				{
					temp1 = sqrt(drhosez[0] * drhosez[0] + drhosez[1] * drhosez[1]);
					az = atan2(drhosez[1] / temp1, -drhosez[0] / temp1);
				}
				else
					if (rhosez[0] > 0.0)
						az = pi;
					else
						az = 0.0;
			else
				az = atan2(rhosez[1] / temp, -rhosez[0] / temp);

			if (temp < small)  // directly over the north pole
				el = MathTimeLib::sgn(rhosez[2]) * halfpi; // +- 90
			else
				el = asin(rhosez[2] / MathTimeLib::mag(rhosez));

			/* ----- calculate range, azimuth and elevation rates ------- */
			drho = MathTimeLib::dot(rhosez, drhosez) / rho;
			if (fabs(temp * temp) > small)
				daz = (drhosez[0] * rhosez[1] - drhosez[1] * rhosez[0]) / (temp * temp);
			else
				daz = 0.0;

			if (fabs(temp) > 0.00000001)
				del = (drhosez[2] - drho * sin(el)) / temp;
			else
				del = 0.0;
		}
	}  // rv_razel


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
	*  inputs          description                           range / units
	*    r           -  position vector                           er
	*    v           -  velocity vector                           er/tu
	*    rs          -  site position vector                      er
	*    direct      -  direction to convert                         eFrom  eTo
	*
	*  outputs       :
	*    rho         - top radius of the sat                         er
	*    trtasc      - top right ascension                           rad
	*    tdecl       - top declination                               rad
	*    drho        - top radius of the sat rate                    er/tu
	*    tdrtasc     - top right ascension rate                      rad/tu
	*    tddecl      - top declination rate                          rad/tu
	*
	*  locals        :
	*    rhov        -  range vector from site                    er
	*    drhov       -  velocity vector from site                 er / tu
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

	void rv_tradec
	(
		double r[3], double v[3], double rs[3],
		MathTimeLib::edirection direct,
		double& rho, double& trtasc, double& tdecl,
		double& drho, double& dtrtasc, double& dtdecl
	)
	{
		const double small = 0.00000001;
		const double raanearth = 0.05883359221938136;  // earth rot rad/tu

		double earthrate[3], rhov[3], drhov[3], vs[3];
		double   latgc, temp, temp1;

		latgc = asin(rs[2] / MathTimeLib::mag(rs));
		earthrate[0] = 0.0;
		earthrate[1] = 0.0;
		earthrate[2] = raanearth;
		MathTimeLib::cross(earthrate, rs, vs);

		if (direct == MathTimeLib::eFrom)
		{
			/* --------  calculate topocentric vectors ------------------ */
			rhov[0] = (rho * cos(tdecl) * cos(trtasc));
			rhov[1] = (rho * cos(tdecl) * sin(trtasc));
			rhov[2] = (rho * sin(tdecl));

			drhov[0] = (drho * cos(tdecl) * cos(trtasc) -
				rho * sin(tdecl) * cos(trtasc) * dtdecl -
				rho * cos(tdecl) * sin(trtasc) * dtrtasc);
			drhov[1] = (drho * cos(tdecl) * sin(trtasc) -
				rho * sin(tdecl) * sin(trtasc) * dtdecl +
				rho * cos(tdecl) * cos(trtasc) * dtrtasc);
			drhov[2] = (drho * sin(tdecl) + rho * cos(tdecl) * dtdecl);

			/* ------ find ijk range vector from site to satellite ------ */
			MathTimeLib::addvec(1.0, rhov, 1.0, rs, r);
			MathTimeLib::addvec(1.0, drhov, cos(latgc), vs, v);
			/*
			if (show == 'y')
			if (fileout != null)
			{
			fprintf(fileout, "rtb %18.7f %18.7f %18.7f %18.7f er\n",
			rhov[1], rhov[2], rhov[3], MathTimeLib::mag(rhov));
			fprintf(fileout, "vtb %18.7f %18.7f %18.7f %18.7f\n",
			drhov[1], drhov[2], drhov[3], MathTimeLib::mag(drhov));
			}
			*/
		}
		else
		{
			/* ------ find ijk range vector from site to satellite ------ */
			MathTimeLib::addvec(1.0, r, -1.0, rs, rhov);
			MathTimeLib::addvec(1.0, v, -cos(latgc), vs, drhov);

			/* -------- calculate topocentric angle and rate values ----- */
			rho = MathTimeLib::mag(rhov);
			temp = sqrt(rhov[0] * rhov[0] + rhov[1] * rhov[1]);
			if (temp < small)
			{
				temp1 = sqrt(drhov[0] * drhov[0] + drhov[1] * drhov[1]);
				trtasc = atan2(drhov[1] / temp1, drhov[0] / temp1);
			}
			else
				trtasc = atan2(rhov[1] / temp, rhov[0] / temp);

			tdecl = asin(rhov[2] / MathTimeLib::mag(rhov));

			temp1 = -rhov[1] * rhov[1] - rhov[0] * rhov[0];
			drho = MathTimeLib::dot(rhov, drhov) / rho;
			if (fabs(temp1) > small)
				dtrtasc = (drhov[0] * rhov[1] - drhov[1] * rhov[0]) / temp1;
			else
				dtrtasc = 0.0;
			if (fabs(temp) > small)
				dtdecl = (drhov[2] - drho * sin(tdecl)) / temp;
			else
				dtdecl = 0.0;
			/*
			if (show == 'y')
			if (fileout != null)
			{
			fprintf(fileout, "rta %18.7f %18.7f %18.7f %18.7f er\n",
			rhov[1], rhov[3], rhov[3], MathTimeLib::mag(rhov));
			fprintf(fileout, "vta %18.7f %18.7f %18.7f %18.7f er\n",
			drhov[1], drhov[3], drhov[3], MathTimeLib::mag(drhov));
			}
			*/
		}
	} // rv_tradec


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
	*  inputs          description                               range / units
	*    rhovec      - sez satellite range vector                    km
	*    drhovec     - sez satellite velocity vector                 km/s
	*    direct      -  direction to convert                         eFrom  eTo
	*
	*  outputs       :
	*    rho         - satellite range from site                     km
	*    az          - azimuth                                       0.0 to 2pi rad
	*    el          - elevation                                     -pi/2 to pi/2 rad
	*    drho        - range rate                                    km/s
	*    daz         - azimuth rate                                  rad/s
	*    del         - elevation rate                                rad/s
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
	*    sgn         - returns the MathTimeLib::sgn of a variable
	*    dot         - dot product
	*    arcsin      - arc sine function
	*    atan2       - arc tangent function that resolves quadrant ambiguites
	*
	*  references    :
	*    vallado       2013, 261, eq 4-4, eq 4-5
	-----------------------------------------------------------------------------*/

	void rvsez_razel
	(
		double rhosez[3], double drhosez[3],
		MathTimeLib::edirection direct,
		double& rho, double& az, double& el, double& drho, double& daz, double& del
	)
	{
		const double small = 0.00000001;
		const double halfpi = pi / 2.0;

		double temp1, temp, sinel, cosel, sinaz, cosaz;

		if (direct == MathTimeLib::eFrom)
		{
			sinel = sin(el);
			cosel = cos(el);
			sinaz = sin(az);
			cosaz = cos(az);

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
			temp = sqrt(rhosez[0] * rhosez[0] + rhosez[1] * rhosez[1]);
			if (fabs(rhosez[1]) < small)
				if (temp < small)
				{
					temp1 = sqrt(drhosez[0] * drhosez[0] + drhosez[1] * drhosez[1]);
					az = atan2(drhosez[1] / temp1, drhosez[0] / temp1);
				}
				else
					if (drhosez[0] > 0.0)
						az = pi;
					else
						az = 0.0;
			else
				az = atan2(rhosez[1] / temp, rhosez[0] / temp);

			if (temp < small)   // directly over the north pole
				el = MathTimeLib::sgn(rhosez[2]) * halfpi;  // +- 90
			else
				el = asin(rhosez[2] / MathTimeLib::mag(rhosez));

			/* ------  calculate range, azimuth and elevation rates ----- */
			drho = MathTimeLib::dot(rhosez, drhosez) / rho;
			if (fabs(temp * temp) > small)
				daz = (drhosez[0] * rhosez[1] - drhosez[1] * rhosez[0]) / (temp * temp);
			else
				daz = 0.0;

			if (fabs(temp) > small)
				del = (drhosez[2] - drho * sin(el)) / temp;
			else
				del = 0.0;
		}
	}   // rvsez_razel


	/* -----------------------------------------------------------------------------
	*
	*                           function rv2rsw
	*
	*  this function converts position and velocity vectors into radial, along-
	*    track, and cross-track coordinates. note that sometimes the middle vector
	*    is called in-track.
	*
	*  author        : david vallado                  719-573-2600    9 jun 2002
	*
	*  revisions
	*                -
	*
	*  inputs          description                           range / units
	*    r           - position vector                             km
	*    v           - velocity vector                             km/s
	*
	*  outputs       :
	*    rrsw        - position vector                             km
	*    vrsw        - velocity vector                             km/s
	*    transmat    - transformation matrix
	*
	*  locals        :
	*    tempvec     - temporary vector
	*    rvec,svec,wvec - direction cosines
	*
	*  coupling      :
	*
	*  references    :
	*    vallado       2013, 164
	* --------------------------------------------------------------------------- */

	void rv2rsw
	(
		double r[3], double v[3],
		double rrsw[3], double vrsw[3], std::vector< std::vector<double> > &transmat
	)
	{
		double rvec[3], svec[3], wvec[3], tempvec[3];

		// --------------------  Implementation   ----------------------
		// in order to work correctly each of the components must be
		// unit vectors
		// radial component
		MathTimeLib::norm(r, rvec);

		// ncross-track component
		MathTimeLib::cross(r, v, tempvec);
		MathTimeLib::norm(tempvec, wvec);

		// along-track component
		MathTimeLib::cross(wvec, rvec, tempvec);
		MathTimeLib::norm(tempvec, svec);

		// assemble transformation matrix from to rsw frame (individual
		//  components arranged in row vectors)
		transmat[0][0] = rvec[0];
		transmat[0][1] = rvec[1];
		transmat[0][2] = rvec[2];
		transmat[1][0] = svec[0];
		transmat[1][1] = svec[1];
		transmat[1][2] = svec[2];
		transmat[2][0] = wvec[0];
		transmat[2][1] = wvec[1];
		transmat[2][2] = wvec[2];

		MathTimeLib::matvecmult(transmat, r, rrsw);
		MathTimeLib::matvecmult(transmat, v, vrsw);
		/*
		*   alt approach
		*       rrsw[0] = mag(r)
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
		*    incl        - inclination                    0.0  to pi rad
		*    nu          - true anomaly                   0.0  to 2pi rad
		*    arglat      - argument of latitude      (ci) 0.0  to 2pi rad
		*    truelon     - true longitude            (ce) 0.0  to 2pi rad
		*    lonper      - longitude of periapsis    (ee) 0.0  to 2pi rad
		*    temp        - temporary variable
		*    typeorbit   - type of orbit                  ee, ei, ce, ci
		*
		*  coupling      :
		*    mag         - magnitude of a vector
		*    MathTimeLib::cross       - MathTimeLib::cross product of two vectors
		*    angle       - find the angle between two vectors
		*
		*  references    :
		*    vallado       2007, 126, alg 9, ex 2-5
		* --------------------------------------------------------------------------- */

	void rv2pqw
	(
		double r[3], double v[3],
		double rpqw[3], double vpqw[3]
	)
	{
		double small, magr, magv, magn, rdotv, temp, c1, hk, magh, halfpi;
		double p, ecc, incl, nu, arglat, truelon;
		double sin_nu, cos_nu;
		double hbar[3];
		double ebar[3];
		double nbar[3];
		int i;
		// switch this to an integer msvs seems to have problems with this and strncpy_s
		//char typeorbit[2];
		int typeorbit;
		// here 
		// typeorbit = 1 = 'ei'
		// typeorbit = 2 = 'ce'
		// typeorbit = 3 = 'ci'
		// typeorbit = 4 = 'ee'
		// define these here since setting individually

		halfpi = 0.5 * pi;
		small = 0.00000001;

		// -------------------------  implementation   -----------------
		magr = MathTimeLib::mag(r);
		magv = MathTimeLib::mag(v);

		// ------------------  find h n and e vectors   ----------------
		MathTimeLib::cross(r, v, hbar);
		magh = MathTimeLib::mag(hbar);
		if (magh > small)
		{
			nbar[0] = -hbar[1];
			nbar[1] = hbar[0];
			nbar[2] = 0.0;
			magn = MathTimeLib::mag(nbar);
			c1 = magv * magv - mu / magr;
			rdotv = MathTimeLib::dot(r, v);
			for (i = 0; i <= 2; i++)
				ebar[i] = (c1 * r[i] - rdotv * v[i]) / mu;
			ecc = MathTimeLib::mag(ebar);

			// ------------  find a e and semi-latus rectum   ----------
			p = magh * magh / mu;

			// -----------------  find inclination   -------------------
			hk = hbar[2] / magh;
			incl = acos(hk);

			// --------  determine type of orbit for later use  --------
			// ------ elliptical, parabolic, hyperbolic inclined -------
			//#ifdef _MSC_VER  // chk if compiling under MSVS
			//		   strcpy_s(typeorbit, 2 * sizeof(char), "ei");
			//#else
			//		   strcpy(typeorbit, "ei");
			//#endif
			typeorbit = 1;

			if (ecc < small)
			{
				// ----------------  circular equatorial ---------------
				if ((incl < small) | (fabs(incl - pi) < small))
				{
					//#ifdef _MSC_VER
					//				   strcpy_s(typeorbit, sizeof(typeorbit), "ce");
					//#else
					//				   strcpy(typeorbit, "ce");
					//#endif
					typeorbit = 2;
				}
				else
				{
					// --------------  circular inclined ---------------
					//#ifdef _MSC_VER
					//				   strcpy_s(typeorbit, sizeof(typeorbit), "ci");
					//#else
					//				   strcpy(typeorbit, "ci");
					//#endif
					typeorbit = 3;
				}
			}
			else
			{
				// - elliptical, parabolic, hyperbolic equatorial --
				if ((incl < small) | (fabs(incl - pi) < small))
				{
					//#ifdef _MSC_VER
					//				   strcpy_s(typeorbit, sizeof(typeorbit), "ee");
					//#else
					//				   strcpy(typeorbit, "ee");
					//#endif
					typeorbit = 4;
				}
			}


			// ------------  find true anomaly at epoch    -------------
			//if (typeorbit[0] == 'e')
			if ((typeorbit == 1) || (typeorbit == 4))
			{
				nu = MathTimeLib::angle(ebar, r);
				if (rdotv < 0.0)
					nu = twopi - nu;
			}
			else
				nu = undefined;

			// ----  find argument of latitude - circular inclined -----
			//if (strcmp(typeorbit, "ci") == 0)
			if (typeorbit == 3)
			{
				arglat = MathTimeLib::angle(nbar, r);
				if (r[2] < 0.0)
					arglat = twopi - arglat;
			}
			else
				arglat = undefined;

			// -------- find true longitude - circular equatorial ------
			//if ((magr>small) && (strcmp(typeorbit, "ce") == 0))
			if ((magr > small) && (typeorbit == 2))
			{
				temp = r[0] / magr;
				if (fabs(temp) > 1.0)
					temp = MathTimeLib::sgn(temp);
				truelon = acos(temp);
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
			//if (strcmp(typeorbit, "ce") == 0)
				if (typeorbit == 2)
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
			sin_nu = sin(nu);
			cos_nu = cos(nu);
			temp = p / (1.0 + ecc * cos_nu);
			rpqw[0] = temp * cos_nu;
			rpqw[1] = temp * sin_nu;
			rpqw[2] = 0.0;
			if (abs(p) < 0.00000001)
				p = 0.00000001;

			vpqw[0] = -sin_nu * sqrt(mu / p);
			vpqw[1] = (ecc + cos_nu) *sqrt(mu / p);
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



	/* -----------------------------------------------------------------------------
	*
	*                           function rv2ntw
	*
	*  this function converts position and velocity vectors into normal,
	*    velocity, and cross-track coordinates. this is the ntw system in vallado.
	*
	*  author        : david vallado                  719-573-2600    5 jul 2002
	*
	*  revisions
	*                -
	*
	*  inputs          description                            range / units
	*    r           - position vector                             km
	*    v           - velocity vector                             km/s
	*
	*  outputs       :
	*    rntw        - position vector                             km
	*    vntw        - velocity vector                             km/s
	*    transmat    - transformation matrix
	*
	*  locals        :
	*    tempvec     - temporary vector
	*    nvec,tvec,wvec - direction cosines
	*
	*  coupling      :
	*
	*  references    :
	*    vallado       2013, 164
	* --------------------------------------------------------------------------- */

	void  rv2ntw
	(
		double r[3], double v[3],
		double rntw[3], double vntw[3], std::vector< std::vector<double> > &transmat
	)
	{
		double tvec[3], nvec[3], wvec[3], tempvec[3];

		// --------------------  Implementation   ----------------------
		// in order to work correctly each of the components must be
		// unit vectors
		// in-velocity component
		MathTimeLib::norm(v, tvec);

		// cross-track component
		MathTimeLib::cross(r, v, tempvec);
		MathTimeLib::norm(tempvec, wvec);

		// along-radial component
		MathTimeLib::cross(tvec, wvec, tempvec);
		MathTimeLib::norm(tempvec, nvec);

		// assemble transformation matrix from to ntw frame (individual
		//  components arranged in row vectors)
		transmat[0][0] = nvec[0];
		transmat[0][1] = nvec[1];
		transmat[0][2] = nvec[2];
		transmat[1][0] = tvec[0];
		transmat[1][1] = tvec[1];
		transmat[1][2] = tvec[2];
		transmat[2][0] = wvec[0];
		transmat[2][1] = wvec[1];
		transmat[2][2] = wvec[2];

		MathTimeLib::matvecmult(transmat, r, rntw);
		MathTimeLib::matvecmult(transmat, v, vntw);
	}  // rv2ntw


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

	void newtone
	(
		double ecc, double e0, double& m, double& nu
	)
	{
		double small, sinv, cosv;

		// -------------------------  implementation   -----------------
		small = 0.00000001;

		// ------------------------- circular --------------------------
		if (abs(ecc) < small)
		{
			m = e0;
			nu = e0;
		}
		else
		{
			// ----------------------- elliptical ----------------------
			if (ecc < 0.999)
			{
				m = e0 - ecc * sin(e0);
				sinv = (sqrt(1.0 - ecc * ecc) * sin(e0)) / (1.0 - ecc * cos(e0));
				cosv = (cos(e0) - ecc) / (1.0 - ecc * cos(e0));
				nu = atan2(sinv, cosv);
			}
			else
			{

				// ---------------------- hyperbolic  ------------------
				if (ecc > 1.0001)
				{
					m = ecc * sinh(e0) - e0;
					sinv = (sqrt(ecc * ecc - 1.0) * sinh(e0)) / (1.0 - ecc * cosh(e0));
					cosv = (cosh(e0) - ecc) / (1.0 - ecc * cosh(e0));
					nu = atan2(sinv, cosv);
				}
				else
				{

					// -------------------- parabolic ------------------
					m = e0 + (1.0 / 3.0) * e0 * e0 * e0;
					nu = 2.0 * atan(e0);
				}
			}
		}
	}  // newtone


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
	*  inputs          description                         range / units
	*    ecc         - eccentricity                            0.0 to
	*    m           - mean anomaly                        -2pi to 2pi rad
	*
	*  outputs       :
	*    e0          - eccentric anomaly                    0.0 to 2pi rad
	*    nu          - true anomaly                        0.0 to 2pi rad
	*
	*  locals        :
	*    e1          - eccentric anomaly, next value               rad
	*    sinv        - sine of nu
	*    cosv        - cosine of nu
	*    ktr         - index
	*    r1r         - cubic roots - 1 to 3
	*    r1i         - imaginary component
	*    r2r         -
	*    r2i         -
	*    r3r         -
	*    r3i         -
	*    s           - variables for parabolic solution
	*    w           - variables for parabolic solution
	*
	*  coupling      :
	*    atan2       - arc tangent function which also resloves quadrants
	*    cubic       - solves a cubic polynomial
	*    power       - raises a base number to an arbitrary power
	*    sinh        - hyperbolic sine
	*    cosh        - hyperbolic cosine
	*    sgn         - returns the MathTimeLib::sgn of an argument
	*
	*  references    :
	*    vallado       2013, 65, alg 2, ex 2-1
	* --------------------------------------------------------------------------- */

	void newtonm
	(
		double ecc, double m, double& e0, double& nu
	)
	{
		const int numiter = 50;
		const double small = 0.00000001;       // small value for tolerances

		double e1, sinv, cosv, cose1, coshe1, temp, r1r = 0.0;
		int ktr;

		// -------------------------- hyperbolic  ----------------------- 
		if ((ecc - 1.0) > small)
		{
			// ------------  initial guess ------------- 
			if (ecc < 1.6)
				if (((m < 0.0) && (m > -pi)) || (m > pi))
					e0 = m - ecc;
				else
					e0 = m + ecc;
			else
				if ((ecc < 3.6) && (fabs(m) > pi)) // just edges)
					e0 = m - MathTimeLib::sgn(m) * ecc;
				else
					e0 = m / (ecc - 1.0); // best over 1.8 in middle
			ktr = 1;
			e1 = e0 + ((m - ecc * sinh(e0) + e0) / (ecc * cosh(e0) - 1.0));
			while ((fabs(e1 - e0) > small) && (ktr <= numiter))
			{
				e0 = e1;
				e1 = e0 + ((m - ecc * sinh(e0) + e0) / (ecc * cosh(e0) - 1.0));
				ktr++;
			}
			// ---------  find true anomaly  ----------- 
			coshe1 = cosh(e1);
			sinv = -(sqrt(ecc * ecc - 1.0) * sinh(e1)) / (1.0 - ecc * coshe1);
			cosv = (coshe1 - ecc) / (1.0 - ecc * coshe1);
			nu = atan2(sinv, cosv);
		}
		else
		{
			// ---------------------- parabolic ------------------------- 
			if (fabs(ecc - 1.0) < small)
			{
				//kbn      cubic(1.0 / 3.0, 0.0, 1.0, -m, r1r, r1i, r2r, r2i, r3r, r3i);
				e0 = r1r;
				//kbn      if (fileout != null)
				//        fprintf(fileout, "roots %11.7f %11.7f %11.7f %11.7f %11.7f %11.7f\n",
				//                          r1r, r1i, r2r, r2i, r3r, r3i);
				/*
					 s  = 0.5 * (halfpi - atan(1.5 * m));
					 w  = atan(power(tan(s), 1.0 / 3.0));
					 e0 = 2.0 * cot(2.0* w );
				*/
				ktr = 1;
				nu = 2.0 * atan(e0);
			}
			else
			{
				// --------------------- elliptical --------------------- 
				if (ecc > small)
				{
					// ------------  initial guess ------------- 
					if (((m < 0.0) && (m > -pi)) || (m > pi))
						e0 = m - ecc;
					else
						e0 = m + ecc;
					ktr = 1;
					e1 = e0 + (m - e0 + ecc * sin(e0)) / (1.0 - ecc * cos(e0));
					while ((fabs(e1 - e0) > small) && (ktr <= numiter))
					{
						ktr++;
						e0 = e1;
						e1 = e0 + (m - e0 + ecc * sin(e0)) / (1.0 - ecc * cos(e0));
					}
					// ---------  find true anomaly  ----------- 
					cose1 = cos(e1);
					temp = 1.0 / (1.0 - ecc * cose1);
					sinv = (sqrt(1.0 - ecc * ecc) * sin(e1)) * temp;
					cosv = (cose1 - ecc) * temp;
					nu = atan2(sinv, cosv);
				}
				else
				{
					// --------------------- circular --------------------- 
					ktr = 0;
					nu = m;
					e0 = m;
				}
			}
		}
		if (ktr > numiter)
			printf("newtonrhapson not converged in %3d iterations\n", numiter);
	}    // procedure newtonm



	/* -----------------------------------------------------------------------------
	*
	*                           function newtonnu
	*
	*  this function solves keplers equation when the true anomaly is known.
	*    the mean and eccentric, parabolic, or hyperbolic anomaly is also found.
	*    the parabolic limit at 168� is arbitrary. the hyperbolic anomaly is also
	*    limited. the hyperbolic sine is used because it's not double valued.
	*
	*  author        : david vallado                  719-573-2600   27 may 2002
	*
	*  revisions
	*    vallado     - fix small                                     24 sep 2002
	*
	*  inputs          description                             range / units
	*    ecc         - eccentricity                                0.0  to
	*    nu          - true anomaly                                -2pi to 2pi rad
	*
	*  outputs       :
	*    e0          - eccentric anomaly                           0.0  to 2pi rad       153.02 deg
	*    m           - mean anomaly                                0.0  to 2pi rad       151.7425 deg
	*
	*  locals        :
	*    e1          - eccentric anomaly, next value               rad
	*    sine        - sine of e
	*    cose        - cosine of e
	*    ktr         - index
	*
	*  coupling      :
	*    arcsinh     - arc hyperbolic sine
	*    sinh        - hyperbolic sine
	*
	*  references    :
	*    vallado       2013, 77, alg 5
	* --------------------------------------------------------------------------- */

	void newtonnu
	(
		double ecc, double nu,
		double& e0, double& m
	)
	{
		double small, sine, cose, cosnu, temp;

		// ---------------------  implementation   ---------------------
		e0 = 999999.9;
		m = 999999.9;
		small = 0.00000001;

		// --------------------------- circular ------------------------
		if (fabs(ecc) < small)
		{
			m = nu;
			e0 = nu;
		}
		else
			// ---------------------- elliptical -----------------------
			if (ecc < 1.0 - small)
			{
				cosnu = cos(nu);
				temp = 1.0 / (1.0 + ecc * cosnu);
				sine = (sqrt(1.0 - ecc * ecc) * sin(nu)) * temp;
				cose = (ecc + cosnu) * temp;
				e0 = atan2(sine, cose);
				m = e0 - ecc * sin(e0);
			}
			else
				// -------------------- hyperbolic  --------------------
				if (ecc > 1.0 + small)
				{
					if ((ecc > 1.0) && (fabs(nu) + 0.00001 < pi - acos(1.0 / ecc)))
					{
						sine = (sqrt(ecc * ecc - 1.0) * sin(nu)) / (1.0 + ecc * cos(nu));
						e0 = MathTimeLib::asinh(sine);
						m = ecc * sinh(e0) - e0;
					}
				}
				else
					// ----------------- parabolic ---------------------
					if (fabs(nu) < 168.0 * pi / 180.0)
					{
						e0 = tan(nu * 0.5);
						m = e0 + (e0 * e0 * e0) / 3.0;
					}

		if (ecc < 1.0)
		{
			m = fmod(m, 2.0 * pi);
			if (m < 0.0)
				m = m + 2.0 * pi;
			e0 = fmod(e0, 2.0 *pi);
		}
	}  // newtonnu


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
	*  inputs          description                           range / units
	*    znew        - z variable                                rad2
	*
	*  outputs       :
	*    c2new       - c2 function value
	*    c3new       - c3 function value
	*
	*  locals        :
	*    sqrtz       - square root of znew
	*
	*  coupling      :
	*    sinh        - hyperbolic sine
	*    cosh        - hyperbolic cosine
	*
	*  references    :
	*    vallado       2013, 63, alg 1
	* --------------------------------------------------------------------------- */

	void findc2c3
	(
		double znew,
		double& c2new, double& c3new
	)
	{
		double small, sqrtz;
		small = 0.00000001;

		// -------------------------  implementation   -----------------
		if (znew > small)
		{
			sqrtz = sqrt(znew);
			c2new = (1.0 - cos(sqrtz)) / znew;
			c3new = (sqrtz - sin(sqrtz)) / (sqrtz*sqrtz*sqrtz);
		}
		else
		{
			if (znew < -small)
			{
				sqrtz = sqrt(-znew);
				c2new = (1.0 - cosh(sqrtz)) / znew;
				c3new = (sinh(sqrtz) - sqrtz) / (sqrtz*sqrtz*sqrtz);
			}
			else
			{
				c2new = 0.5;
				c3new = 1.0 / 6.0;
			}
		}
	}  // findc2c3


			/* -----------------------------------------------------------------------------
		*
		*                           function findfandg
		*
		*  this function calculates the f and g functions for use in various applications.
		*  several methods are available. the values are in normal (not canonical) units.
		*  note that not all the input parameters are needed for each case.
		*
		*  author        : david vallado                  719-573-2600   23 jan 2019
		*
		*  inputs          description                    range / units
		*    r1          - position vector                     km
		*    v1          - velocity vector                     km/s
		*    r2          - position vector                     km
		*    v2          - velocity vector                     km/s
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
	void findfandg
	(
		double r1[3], double v1[3], double r2[3], double v2[3], double dtsec,
		double x, double c2, double c3, double z,
		int opt,
		double& f, double& g, double& fdot, double& gdot
	)
	{
		double magr1, magv1;
		f = 0.0;
		g = 0.0;
		gdot = 0.0;

		magr1 = MathTimeLib::mag(r1);
		magv1 = MathTimeLib::mag(v1);

		// -------------------------  implementation   -----------------
		// use int instead of a string since c++ can't do strings
		// 0 pqw
		// 1 series
		// 2 c2c3
		f = 0.0;
		g = 0.0;
		fdot = 0.0;
		gdot = 0.0;
		switch (opt)
		{
		case 0:  // "pqw"
		{
			double h;
			double hbar[3];
			double rpqw1[3];
			double rpqw2[3];
			double vpqw1[3];
			double vpqw2[3];
			MathTimeLib::cross(r1, v1, hbar);
			h = MathTimeLib::mag(hbar);
			// find vectors in PQW frame
			rv2pqw(r1, v1, rpqw1, vpqw1);
			// find vectors in PQW frame
			rv2pqw(r2, v2, rpqw2, vpqw2);

			// normal units
			f = (rpqw2[0] * vpqw1[1] - vpqw2[0] * rpqw1[1]) / h;
			g = (rpqw1[0] * rpqw2[1] - rpqw2[0] * rpqw1[1]) / h;
			gdot = (rpqw1[0] * vpqw2[1] - vpqw2[0] * rpqw1[1]) / h;
			// fdot = (v2t[0] * v1t[1] - v2t[1] * v1t[0]) / h;
		}
			break;
		case 1:  //"series":
		{
			double u, p, q;
			u = mu / (magr1 * magr1 * magr1);
			p = MathTimeLib::dot(r1, v1) / (magr1 * magr1);
			q = (magv1 * magv1 - magr1 * magr1 * u) / (magr1 * magr1);
			double p2 = p * p;
			double p3 = p2 * p;
			double p4 = p3 * p;
			double p5 = p4 * p;
			double p6 = p5 * p;
			double u2 = u * u;
			double u3 = u2 * u;
			double u4 = u3 * u;
			double q2 = q * q;
			double q3 = q2 * q;
			double dt2 = dtsec * dtsec;
			double dt3 = dt2 * dtsec;
			double dt4 = dt3 * dtsec;
			double dt5 = dt4 * dtsec;
			double dt6 = dt5 * dtsec;
			double dt7 = dt6 * dtsec;
			double dt8 = dt7 * dtsec;
			f = 1.0 - 0.5 * u * dt2 + 0.5 * u * p * dt3 + 1.0 / 24.0 * (3 * u * q - 15 * u * p2 + u2) * dt4 +
				1.0 / 8.0 * (7 * u * p3 - 3 * u * p * q - u2 * p) * dt5 +
				1.0 / 720.0 * (630 * u * p2 * q - 24 * u2 * q - u3 - 45 * u * q2 - 945 * u * p4 + 210 * u2 * p2) * dt6 +
				1.0 / 5040 * (882 * u2 * p * q - 3150 * u2 * p3 - 9450 * u * p3 * q + 1575 * u * p * q2 + 63 * u3 * p + 10395 * u * p5) * dt7 +
				1.0 / 40320 * (1107 * u2 * q2 - 24570 * u2 * p2 * q - 2205 * u3 * p2 + 51975 * u2 * p4 - 42525 * u * p2 * q2 + 155925 * u * p4 * q + 1575 * u * q3 + 117 * u3 * q - 135135 * u * p6 + u4) * dt8;

			g = dtsec - 1 / 6 * u * dt3 + 0.25 * u * p * dt4 + 1 / 120 * (9 * u * q - 45 * u * p2 + u2) * dt5 +
				1 / 360 * (210 * u * p3 - 90 * u * p * q - 15 * u2 * p) * dt6 +
				1 / 5040 * (3150 * u * p2 * q - 54 * u2 * q - 225 * u * q2 - 4725 * u * p4 + 630 * u2 * p2 - u3) * dt7 +
				1 / 40320 * (3024 * u2 * p * q - 12600 * u2 * p3 - 56700 * u * p3 * q + 9450 * u * p * q2 + 62370 * u * p5 + 126 * u3 * p) * dt8;

			fdot = -u * dtsec + 1.5 * u * p * dt2 + 1.0 / 6.0 * (3 * u * q - 15 * u * p2 + u2) * dt3 +
				5.0 / 8.0 * (7.0 * u * p3 - 3 * u * p * q - u2 * p) * dt4 +
				6.0 / 720.0 * (630 * u * p2 * q - 24 * u2 * q - u3 - 45 * u * q2 - 945 * u * p4 + 210 * u2 * p2) * dt5 +
				7.0 / 5040 * (882 * u2 * p * q - 3150 * u2 * p3 - 9450 * u * p3 * q + 1575 * u * p * q2 + 63 * u3 * p + 10395 * u * p5) * dt6 +
				8.0 / 40320 * (1107 * u2 * q2 - 24570 * u2 * p2 * q - 2205 * u3 * p2 + 51975 * u2 * p4 - 42525 * u * p2 * q2 + 155925 * u * p4 * q + 1575 * u * q3 + 117 * u3 * q - 135135 * u * p6 + u4) * dt7;

			gdot = 1.0 - 0.5 * u * dt2 + u * p * dt3 + 5 / 120 * (9 * u * q - 45 * u * p2 + u2) * dt4 +
				1.0 / 60 * (210 * u * p3 - 90 * u * p * q - 15 * u2 * p) * dt5 +
				7.0 / 5040 * (3150 * u * p2 * q - 54 * u2 * q - 225 * u * q2 - 4725 * u * p4 + 630 * u2 * p2 - u3) * dt6 +
				8.0 / 40320 * (3024 * u2 * p * q - 12600 * u2 * p3 - 56700 * u * p3 * q + 9450 * u * p * q2 + 62370 * u * p5 + 126 * u3 * p) * dt7;
		}
			break;
		case 2:  //"c2c3":
		{
			double xsqrd = x * x;
			double magr2 = MathTimeLib::mag(r2);
			f = 1.0 - (xsqrd * c2 / magr1);
			g = dtsec - xsqrd * x * c3 / sqrt(mu);
			gdot = 1.0 - (xsqrd * c2 / magr2);
			fdot = (sqrt(mu) * x / (magr1 * magr2)) * (z * c3 - 1.0);
		}
			break;
		}
		// g = g * tusec;  / to canonical if needed, fdot/tusec too
		fdot = (f * gdot - 1.0) / g;

	}  //  findfandg


		/* -----------------------------------------------------------------------------
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
	*  inputs          description                             range / units
	*    ro          -  position vector - initial               km
	*    vo          -  velocity vector - initial               km / s
	*    dtsec       - length of time to propagate                 s
	*
	*  outputs       :
	*    r           -  position vector                         km
	*    v           -  velocity vector                         km / s
	*    error       - error flag                                  'ok', ...
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
	*    dtsec       - change in time                              s
	*    timenew     - new time                                    s
	*    rdotv       - result of ro dot vo
	*    a           - semi or axis                                km
	*    alpha       - reciprocol  1/a
	*    sme         - specific mech energy                       km2 / s2
	*    period      - time period for satellite                   s
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
	*    vallado       2013, 93, alg 8, ex 2-4
	---------------------------------------------------------------------------- */

	void kepler
	(
		double ro[3], double vo[3], double dtseco, double r[3], double v[3]
	)
	{
		int ktr, i, numiter, mulrev;
		double h[3], f, g, fdot, gdot, rval, xold, xoldsqrd, xnew,
			xnewsqrd, znew, p, c2new, c3new, dtnew, rdotv, a, dtsec,
			alpha, sme, period, s, w, temp,
			magro, magvo, magh, magr, magv;
		char show, errork[10];
		show = 'n';
		double small, halfpi;

		small = 0.00000001;
		halfpi = pi * 0.5;

		// -------------------------  implementation   -----------------
		// set constants and intermediate printouts
		numiter = 50;

		if (show == 'y')
		{
			printf(" ro %16.8f %16.8f %16.8f ER \n", ro[0] / re, ro[1] / re, ro[2] / re);
			printf(" vo %16.8f %16.8f %16.8f ER/TU \n", vo[0] / velkmps, vo[1] / velkmps, vo[2] / velkmps);
		}

		// --------------------  initialize values   -------------------
		ktr = 0;
		xold = 0.0;
		znew = 0.0;
		strcpy_s(errork, "      ok");
		dtsec = dtseco;
		mulrev = 0;

		if (fabs(dtseco) > small)
		{
			magro = MathTimeLib::mag(ro);
			magvo = MathTimeLib::mag(vo);
			rdotv = MathTimeLib::dot(ro, vo);

			// -------------  find sme, alpha, and a  ------------------
			sme = ((magvo * magvo) * 0.5) - (mu / magro);
			alpha = -sme * 2.0 / mu;

			if (fabs(sme) > small)
				a = -mu / (2.0 * sme);
			else
				a = infinite;
			if (fabs(alpha) < small)   // parabola
				alpha = 0.0;

			if (show == 'y')
			{
				printf(" sme %16.8f  a %16.8f alp  %16.8f ER \n", sme / (mu / re), a / re, alpha * re);
				printf(" sme %16.8f  a %16.8f alp  %16.8f km \n", sme, a, alpha);
				printf(" ktr      xn        psi           r          xn+1        dtn \n");
			}

			// ------------   setup initial guess for x  ---------------
			// -----------------  circle and ellipse -------------------
			if (alpha >= small)
			{
				period = twopi * sqrt(fabs(a * a * a) / mu);
				// ------- next if needed for 2body multi-rev ----------
				if (fabs(dtseco) > fabs(period))
					// including the truncation will produce vertical lines that are parallel
					// (plotting chi vs time)
					//                    dtsec = rem( dtseco,period );
					mulrev = int(dtseco / period);
				if (fabs(alpha - 1.0) > small)
					xold = sqrt(mu) * dtsec * alpha;
				else
					// - first guess can't be too close. ie a circle, r=a
					xold = sqrt(mu) * dtsec * alpha * 0.97;
			}
			else
			{
				// --------------------  parabola  ---------------------
				if (fabs(alpha) < small)
				{
					MathTimeLib::cross(ro, vo, h);
					magh = MathTimeLib::mag(h);
					p = magh * magh / mu;
					s = 0.5  * (halfpi - atan(3.0 * sqrt(mu / (p * p * p)) * dtsec));
					w = atan(pow(tan(s), (1.0 / 3.0)));
					xold = sqrt(p) * (2.0 * MathTimeLib::cot(2.0 * w));
					alpha = 0.0;
				}
				else
				{
					// ------------------  hyperbola  ------------------
					temp = -2.0 * mu * dtsec /
						(a * (rdotv + MathTimeLib::sgn(dtsec) * sqrt(-mu * a) *	(1.0 - magro * alpha)));
					xold = MathTimeLib::sgn(dtsec) * sqrt(-a) * log(temp);
				}
			} // if alpha

			ktr = 1;
			dtnew = -10.0;
			// conv for dtsec to x units
			double tmp = 1.0 / sqrt(mu);

			while ((fabs(dtnew * tmp - dtsec) >= small) && (ktr < numiter))
			{
				xoldsqrd = xold * xold;
				znew = xoldsqrd * alpha;

				// ------------- find c2 and c3 functions --------------
				findc2c3(znew, c2new, c3new);

				// ------- use a newton iteration for new values -------
				rval = xoldsqrd * c2new + rdotv * tmp * xold * (1.0 - znew * c3new) +
					magro * (1.0 - znew * c2new);
				dtnew = xoldsqrd * xold * c3new + rdotv * tmp * xoldsqrd * c2new +
					magro * xold * (1.0 - znew * c3new);

				// ------------- calculate new value for x -------------
				xnew = xold + (dtsec * sqrt(mu) - dtnew) / rval;

				// ----- check if the univ param goes negative. if so, use bissection
				if (xnew < 0.0)
					xnew = xold * 0.5;

				if (show == 'y')
				{
					printf("%3i %11.7f %11.7f %11.7f %11.7f %11.7f \n", ktr, xold, znew, rval, xnew, dtnew);
					printf("%3i %11.7f %11.7f %11.7f %11.7f %11.7f \n", ktr, xold / sqrt(re), znew, rval / re,
						xnew / sqrt(re), dtnew / sqrt(mu));
				}

				ktr = ktr + 1;
				xold = xnew;
			}  // while

			if (ktr >= numiter)
			{
				strcpy_s(errork, "knotconv");
				printf("not converged in %2i iterations \n", numiter);
				for (i = 0; i < 3; i++)
				{
					v[i] = 0.0;
					r[i] = v[i];
				}
			}
			else
			{
				// --- find position and velocity vectors at new time --
				xnewsqrd = xnew * xnew;
				f = 1.0 - (xnewsqrd * c2new / magro);
				g = dtsec - xnewsqrd * xnew * c3new / sqrt(mu);

				for (i = 0; i < 3; i++)
					r[i] = f * ro[i] + g * vo[i];
				magr = MathTimeLib::mag(r);
				gdot = 1.0 - (xnewsqrd * c2new / magr);
				fdot = (sqrt(mu) * xnew / (magro * magr)) * (znew * c3new - 1.0);
				for (i = 0; i < 3; i++)
					v[i] = fdot * ro[i] + gdot * vo[i];
				magv = MathTimeLib::mag(v);
				temp = f * gdot - fdot * g;
				if (fabs(temp - 1.0) > 0.00001)
					strcpy_s(errork, "fandg");

				if (show == 'y')
				{
					printf("f %16.8f g %16.8f fdot %16.8f gdot %16.8f \n", f, g, fdot, gdot);
					printf("f %16.8f g %16.8f fdot %16.8f gdot %16.8f \n", f, g, fdot, gdot);
					printf("r1 %16.8f %16.8f %16.8f ER \n", r[0] / re, r[1] / re, r[2] / re);
					printf("v1 %16.8f %16.8f %16.8f ER/TU \n", v[0] / velkmps, v[1] / velkmps, v[2] / velkmps);
				}
			}
		} // if fabs
		else
			// ----------- set vectors to incoming since 0 time --------
			for (i = 0; i < 3; i++)
			{
				r[i] = ro[i];
				v[i] = vo[i];
			}

		//       fprintf( fid,"%11.5f  %11.5f %11.5f  %5i %3i ",znew, dtseco/60.0, xold/(rad), ktr, mulrev );
	}   // kepler


	// actually for astPert, but leave here for now
	/* ----------------------------------------------------------------------------
	*
	*                           function pkepler
	*
	*  this function propagates a satellite's position and velocity vector over
	*    a given time period accounting for perturbations caused by j2.
	*
	*  author        : david vallado                  719-573-2600    1 mar 2001
	*
	*  inputs          description                          range / units
	*    ro          - original position vector                    km
	*    vo          - original velocity vector                    km/sec
	*    ndot        - time rate of change of n                    rad/sec
	*    nddot       - time accel of change of n                   rad/sec2
	*    dtsec       - change in time                              sec
	*
	*  outputs       :
	*    r           - updated position vector                     km
	*    v           - updated velocity vector                     km/sec
	*
	*  locals        :
	*    p           - semi-paramter                               km
	*    a           - semior axis                                 km
	*    ecc         - eccentricity
	*    incl        - inclination                                 rad
	*    argp        - argument of periapsis                       rad
	*    argpdot     - change in argument of perigee               rad/sec
	*    raan       - longitude of the asc node                    rad
	*    raandot    - change in raan                               rad
	*    e0          - eccentric anomaly                           rad
	*    e1          - eccentric anomaly                           rad
	*    m           - mean anomaly                                rad/sec
	*    mdot        - change in mean anomaly                      rad/sec
	*    arglat      - argument of latitude                        rad
	*    arglatdot   - change in argument of latitude              rad/sec
	*    truelon     - true longitude of vehicle                  rad
	*    truelondot  - change in the true longitude                rad/sec
	*    lonper     - longitude of periapsis                      rad
	*    lonperodot  - longitude of periapsis change               rad/sec
	*    n           - mean angular motion                         rad/sec
	*    nuo         - true anomaly                                rad
	*    j2op2       - j2 over p sqyared
	*    sinv,cosv   - sine and cosine of nu
	*
	*  coupling:
	*    rv2coe      - orbit elements from position and velocity vectors
	*    coe2rv      - position and velocity vectors from orbit elements
	*    newtonm     - newton rhapson to find nu and eccentric anomaly
	*
	*  references    :
	*    vallado       2013, 691, alg 65
	* -------------------------------------------------------------------------- - */

	void pkepler
	(
		double r1[3], double v1[3], double &dtsec, double &ndot, double &nddot, double r2[3], double v2[3]
	)
	{
		double  truelondot, arglatdot, lonperdot, e0;
		double p, a, ecc, incl, raan, argp, nu, m, eccanom, arglat, truelon, lonper;
		double sqrtbeta, nbar, mdot, raandot, argpdot, sini, cosi;

		double small = 0.00000001;
		double halfpi = pi * 0.5;
		double j2 = 0.00108262617;
		double j3 = -2.53241052e-06;
		double j4 = -1.6198976e-06;
		double j6 = -5.40666576e-07;

		rv2coe(r1, v1, p, a, ecc, incl, raan, argp, nu, m, eccanom, arglat, truelon, lonper);

		//  fprintf(1,'          p km       a km      ecc      incl deg     raan deg     argp deg      nu deg      m deg      arglat   truelon    lonper\n');
		//  fprintf(1,'coes %11.4f%11.4f%13.9f%13.7f%11.5f%11.5f%11.5f%11.5f%11.5f%11.5f%11.5f\n',...
		//          p,a,ecc,incl*rad,raan*rad,argp*rad,nu*rad,m*rad, ...
		//          arglat*rad,truelon*rad,lonper*rad );

		double n1 = (mu / (a * a * a));
		double n = sqrt(n1);

		sini = sin(incl);
		cosi = cos(incl);

		// ------------- find the value of j2 perturbations -------------
		double j2op2 = (n * 1.5 * re * re * j2) / (p * p);
		//     nbar    = n*( 1.0 + j2op2*sqrt(1.0-ecc*ecc)* (1.0 - 1.5*sin(incl)*sin(incl)) );
		raandot = -j2op2 * cos(incl);
		argpdot = j2op2 * (2.0 - 2.5 * sin(incl) * sin(incl));
		mdot = n;

		a = a - 2.0 * ndot * dtsec * a / (3.0 * n);
		ecc = ecc - 2.0 * (1.0 - ecc) * ndot * dtsec / (3.0 * n);
		p = a * (1.0 - ecc * ecc);

		// ------------- find the value of j2, j2^2, j4 from escobal pg 371 perturbations -------------
		sqrtbeta = sqrt(1.0 - ecc * ecc);
		nbar = n * (1.5*j2*pow(re / p, 2) * sqrtbeta*((1.0 - 1.5*sini * sini))
			+ 3.0 / 128.0*j2 *j2 * pow(re / p, 4) * sqrtbeta*(16.0*sqrtbeta + 25.0*(1.0 - ecc * ecc)
				- 15.0 + (30.0 - 96.0*sqrtbeta - 90.0*(1.0 - ecc * ecc))*cosi*cosi
				+ (105.0 + 144.0*sqrtbeta + 25.0*(1.0 - ecc * ecc))*pow(cosi, 4))
			- 45.0 / 128.0*j4*ecc*ecc*pow(re / p, 4) * sqrtbeta*(3.0 - 30.0*cosi*cosi + 35.0*pow(cosi, 4)));
		mdot = n + nbar;

		raandot = -1.5*mdot*j2*pow(re / p, 2) * cosi*
			(1.0 + 1.5*j2*pow(re / p, 2) * (1.5 + ecc * ecc / 6.0 - 2.0*sqrtbeta
				- (5.0 / 3.0 - 5.0 / 24.0*ecc*ecc - 3.0*sqrtbeta)*sini*sini))
			- 35.0 / 8.0*n*j4*pow(re / p, 4) * cosi*(1.0 + 1.5*ecc*ecc)*((12.0 - 21.0*sini *sini) / 14.0);
		//	alt approach - less fractions in equations same
		double raandote1 = -1.5*j2*pow(re / p, 2) * mdot*cosi
			- 9.0 / 96.0*j2 * j2 * pow(re / p, 4) * mdot*cosi*(36.0 + 4.0*ecc * ecc - 48.0*sqrtbeta
				- (40.0 - 5.0*ecc*ecc - 72.0*sqrtbeta)*sini*sini)
			- 35.0 / 112.0*j4*pow(re / p, 4) * n*cosi*(1.0 + 1.5*ecc*ecc)*(12.0 - 21.0*sini *sini);


		argpdot = 1.5*mdot*j2*pow(re / p, 2) * (2.0 - 2.5*sini *sini)*
			(1.0 + 1.5*j2*pow(re / p, 2) * (2.0 + ecc * ecc / 2.0 - 2.0*sqrtbeta
				- (43.0 / 24.0 - ecc * ecc / 48.0 - 3.0*sqrtbeta)*sini*sini))
			- 45.0 / 36.0*j2*j2*mdot*pow(re / p, 4) * ecc*ecc*pow(cosi, 4)
			- 35.0 / 8.0*n*j4*pow(re / p, 4) * (12.0 / 7.0 - 93.0 / 14.0*sini * sini
				+ 21.0 / 4.0*pow(sini, 4) + ecc * ecc*(27.0 / 14.0 - 189.0 / 28.0*sini*sini + 81.0 / 16.0*pow(sini, 4)));
		// same
		double argpdote1 = 0.75*j2*pow(re / p, 2) * mdot*(4.0 - 5.0*sini *sini)
			+ 9.0 / 192.0*j2 * j2 * pow(re / p, 4) * mdot*(2.0 - 2.5*sini *sini)*(96.0 + 24.0*ecc*ecc
				- 96.0*sqrtbeta - (86.0 - ecc * ecc - 144.0*sqrtbeta)*sini*sini)
			- 45.0 / 36.0*j2 * j2 * pow(re / p, 4) * ecc*ecc*n*pow(cosi, 4)
			- 35.0 / 896.0*j4*pow(re / p, 4) * n*(192.0 - 744.0*sini *sini
				+ 588 * pow(sini, 4) + ecc * ecc*(216.0 - 756.0*sini*sini + 567.0*pow(sini, 4)));


		// ----- update the orbital elements for each orbit type --------
		if (ecc < small)
		{
			//  -------------  circular equatorial  ----------------
			if ((incl < small) | (fabs(incl - pi) < small))
			{
				truelondot = raandot + argpdot + mdot;
				truelon = truelon + truelondot * dtsec;
				truelon = fmod(truelon, twopi);
			}
			else
			{
				//  -------------  circular inclined    --------------
				raan = raan + raandot * dtsec;
				raan = fmod(raan, twopi);
				arglatdot = argpdot + mdot;
				arglat = arglat + arglatdot * dtsec;
				arglat = fmod(arglat, twopi);
			}
		}
		else
		{
			//  ---- elliptical, parabolic, hyperbolic equatorial ---
			if ((incl < small) | (fabs(incl - pi) < small))
			{
				lonperdot = raandot + argpdot;
				lonper = lonper + lonperdot * dtsec;
				lonper = fmod(lonper, twopi);
				m = m + mdot * dtsec + ndot * dtsec * dtsec + nddot * pow(dtsec, 3);
				m = fmod(m, twopi);
				newtonm(ecc, m, e0, nu);
			}

			else
			{
				//  ---- elliptical, parabolic, hyperbolic inclined --
				raan = raan + raandot * dtsec;
				raan = fmod(raan, twopi);
				argp = argp + argpdot * dtsec;
				argp = fmod(argp, twopi);
				m = m + mdot * dtsec + ndot * dtsec * dtsec + nddot * dtsec * dtsec * dtsec;
				m = fmod(m, twopi);
				newtonm(ecc, m, e0, nu);
			}
		}

		// ------------- use coe2rv to find new vectors ---------------

		coe2rv(p, ecc, incl, raan, argp, nu, arglat, truelon, lonper, r2, v2);

		//     fprintf(1,'r    %15.9f%15.9f%15.9f',r );
		//     fprintf(1,' v %15.10f%15.10f%15.10f\n',v );
	}   // pkepler


	/* -----------------------------------------------------------------------------
	*
	*                           function ecef2ll
	*
	*  these subroutines convert a geocentric equatorial position vector into
	*    latitude and longitude.  geodetic and geocentric latitude are found. the
	*    inputs must be ecef.
	*
	*  author        : david vallado                  719-573-2600    6 dec 2005
	*
	*  revisions
	*
	*  inputs          description                         range / units
	*    recef       - ecef position vector                     km
	*
	*  outputs       :
	*    latgc       - geocentric latitude                   -pi to pi rad
	*    latgd       - geodetic latitude                     -pi to pi rad
	*    lon         - longitude (west -)                     -2pi to 2pi rad
	*    hellp       - height above the ellipsoid                  km
	*
	*  locals        :
	*    temp        - diff between geocentric/
	*                  geodetic lat                                rad
	*    sintemp     - sine of temp                                rad
	*    olddelta    - previous value of deltalat                  rad
	*    rtasc       - right ascension                             rad
	*    decl        - declination                                 rad
	*    i           - index
	*
	*  coupling      :
	*    mag         - magnitude of a vector
	*    gcgd        - converts between geocentric and geodetic latitude
	*
	*  references    :
	*    vallado       2013, 173, alg 12 and alg 13, ex 3-3
	* --------------------------------------------------------------------------- */

	void ecef2ll
	(
		double recef[3],
		double& latgc, double& latgd, double& lon, double& hellp
	)
	{
		const double small = 0.00000001;         // small value for tolerances
		const double eesqrd = 0.006694385000;     // eccentricity of earth sqrd
		double magr, decl, rtasc, olddelta, temp, sintemp, s, c = 0.0;
		int i;

		// ---------------------------  implementation   -----------------------
		magr = MathTimeLib::mag(recef);

		// ---------------------- find longitude value  ------------------------
		temp = sqrt(recef[0] * recef[0] + recef[1] * recef[1]);
		if (fabs(temp) < small)
			rtasc = MathTimeLib::sgn(recef[2]) * pi * 0.5;
		else
			rtasc = atan2(recef[1], recef[0]);

		lon = rtasc;
		if (fabs(lon) >= pi)   // mod it ?
		{
			if (lon < 0.0)
				lon = twopi + lon;
			else
				lon = lon - twopi;

		}
		decl = asin(recef[2] / magr);
		latgd = decl;

		// ----------------- iterate to find geodetic latitude -----------------
		i = 1;
		olddelta = latgd + 10.0;

		while ((fabs(olddelta - latgd) >= small) && (i < 10))
		{
			olddelta = latgd;
			sintemp = sin(latgd);
			c = re / (sqrt(1.0 - eesqrd * sintemp * sintemp));
			latgd = atan((recef[2] + c * eesqrd*sintemp) / temp);
			i = i + 1;
		}

		if ((pi * 0.5 - fabs(latgd)) > pi / 180.0)  // 1 deg
			hellp = (temp / cos(latgd)) - c;
		else
		{
			s = c * (1.0 - eesqrd);
			hellp = recef[2] / sin(latgd) - s;
		}

		latgc = asin(recef[2] / magr);  // any location
		//gc_gd(latgc, MathTimeLib::eFrom, latgd);  // surface of the Earth location
	}   // ecef2ll




	/* -----------------------------------------------------------------------------
	*
	*                           function gc_gd
	*
	*  this function converts from geodetic to geocentric latitude for positions
	*    on the surface of the earth.  notice that (1-f) squared = 1-esqrd.
	*
	*  author        : david vallado                  719-573-2600    6 dec 2005
	*
	*  revisions
	*
	*  inputs          description                          range / units
	*    latgd       - geodetic latitude                     -pi to pi rad
	*
	*  outputs       :
	*    latgc       - geocentric latitude                    -pi to pi rad
	*
	*  locals        :
	*    none.
	*
	*  coupling      :
	*    none.
	*
	*  references    :
	*    vallado       2013, 140, eq 3-11
	* --------------------------------------------------------------------------- */

	void gc_gd
	(
		double&    latgc,
		MathTimeLib::edirection direct,
		double&    latgd
	)
	{
		const double eesqrd = 0.006694385000;     // eccentricity of earth sqrd

		if (direct == MathTimeLib::eTo)
			latgd = atan(tan(latgc) / (1.0 - eesqrd));
		else
			latgc = atan((1.0 - eesqrd) * tan(latgd));
	}   // gc_gd



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
	//    MathTimeLib::cross       - MathTimeLib::cross product of vectors
	//
	//  references    :
	//    vallado       2013, 503, alg 60
	// ------------------------------------------------------------------------------*/

	void checkhitearth
	(
		double altPad, double r1[3], double v1t[3], double r2[3], double v2t[3], int nrev, char& hitearth
	)
	{
		double radiuspad = re + altPad; // radius of Earth with pad, km
		double rp, magh, magv1, v12, a, ainv, ecc, ecosea1, esinea1, ecosea2;
		double hbar[3];
		rp = 0.0;

		double magr1 = MathTimeLib::mag(r1);
		double magr2 = MathTimeLib::mag(r2);

		hitearth = 'n';
		//hitearthstr = "no";

		// check whether Lambert transfer trajectory hits the Earth
		if (magr1 < radiuspad || magr2 < radiuspad)
		{
			// hitting earth already at start or stop point
			hitearth = 'y';
			//hitearthstr = hitearth + "_radii";
		}
		else
		{
			double rdotv1 = MathTimeLib::dot(r1, v1t);
			double rdotv2 = MathTimeLib::dot(r2, v2t);

			// Solve for a 
			magv1 = MathTimeLib::mag(v1t);
			v12 = magv1 * magv1;
			ainv = 2.0 / magr1 - v12 / mu;

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
					esinea1 = rdotv1 / sqrt(mu * a);
					ecc = sqrt(ecosea1 * ecosea1 + esinea1 * esinea1);
				}
				// hyperbolic orbit
				else
				{
					esinea1 = rdotv1 / sqrt(mu * fabs(-a));
					ecc = sqrt(ecosea1 * ecosea1 - esinea1 * esinea1);
				}
				rp = a * (1.0 - ecc);
				if (rp < radiuspad)
				{
					hitearth = 'y';
					//hitearthstr = hitearth + "Sub_Earth_nrrp";
				}
			}
			// nrev = 0, 3 cases:
			// heading to perigee and ending after perigee
			// both headed away from perigee, but end is closer to perigee
			// both headed toward perigee, but start is closer to perigee
			else
			{
				if ((rdotv1 < 0.0 && rdotv2 > 0.0) || (rdotv1 > 0.0 && rdotv2 > 0.0 && ecosea1 < ecosea2) ||
					(rdotv1 < 0.0 && rdotv2 < 0.0 && ecosea1 > ecosea2))
				{
					// parabolic orbit
					if (fabs(ainv) <= 1.0e-10)
					{
						MathTimeLib::cross(r1, v1t, hbar);
						magh = MathTimeLib::mag(hbar); // find h magnitude
						rp = magh * magh * 0.5 / mu;
						if (rp < radiuspad)
						{
							hitearth = 'y';
							//hitearthstr = hitearth + "Sub_Earth_para";
						}
					}
					else
					{
						a = 1.0 / ainv;
						// elliptical orbit
						if (a > 0.0)
						{
							esinea1 = rdotv1 / sqrt(mu * a);
							ecc = sqrt(ecosea1 * ecosea1 + esinea1 * esinea1);
						}
						// hyperbolic orbit
						else
						{
							esinea1 = rdotv1 / sqrt(mu * fabs(-a));
							ecc = sqrt(ecosea1 * ecosea1 - esinea1 * esinea1);
						}
						if (ecc < 1.0)
						{
							rp = a * (1.0 - ecc);
							if (rp < radiuspad)
							{
								hitearth = 'y';
								//hitearthstr = hitearth + "Sub_Earth_ell";
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
									//hitearthstr = hitearth + "Sub_Earth_hyp";
								}
							}
						}

					} // ell and hyp checks
				} // end nrev = 0 cases
			}  // nrev = 0 cases
		}  // check if starting positions ok
	} // checkhitearth

	
	/* ----------------------- lambert techniques -------------------- */
			/* ------------------------------------------------------------------------------
		//                           function lambertumins
		//
		//  find the minimum psi values for the universal variable lambert problem
		//
		//  author        : david vallado                  719-573-2600    8 jun 2016
		//
		//  inputs          description                    range / units
		//    r1          - ijk position vector 1          km
		//    r2          - ijk position vector 2          km
		//    nrev        - multiple revolutions           0, 1,
		//    df          - dir of flight (direct, retrograde) 'd','r'
		//                  this is the inclination discriminator
		//
		//  outputs       :
		//    kbi         - k values for min tof for each nrev
		//    tof         - min time of flight for each nrev   tu
		//
		//  references    :
		//    Arora and Russell AAS 10-198
		// ------------------------------------------------------------------------------*/

	void lambertumins
	(
		double r1[3], double r2[3], int nrev, char df,
		double& kbi, double& tof
	)
	{
		double small, oomu, magr1, magr2, vara, cosdeltanu, sqrtmu, sqrty, dtdpsi;
		double psinew, x, y, q, s1, s2, s3, s4, x3, x5, dtnew;
		double psiold2, psiold3, psiold4;
		double c2, c3, c2dot, c3dot, c2ddot, c3ddot, upper, lower, psiold, dtdpsi2;
		double oox;
		int numiter, loops;
		x = 0.0;
		sqrty = 0.0;
		psinew = 0.0;
		numiter = 20; // arbitrary limit here - doens't seem to break it. 

		small = 0.00000001;

		oomu = 1.0 / sqrt(mu);  // for speed
		sqrtmu = sqrt(mu);

		// ---- find parameters that are constant for the intiial geometry
		magr1 = MathTimeLib::mag(r1);
		magr2 = MathTimeLib::mag(r2);

		cosdeltanu = MathTimeLib::dot(r1, r2) / (magr1 * magr2);
		if (df == 'r')
			vara = -sqrt(magr1 * magr2 * (1.0 + cosdeltanu));
		else
			vara = sqrt(magr1 * magr2 * (1.0 + cosdeltanu));

		// ------------ find the minimum time for a nrev transfer --------------
		//   nrev = 0;
		//   for zz = 0: 4
		//       nrev = nrev + 1;
		// ---- get outer bounds for each nrev case
		lower = 4.0 * nrev * nrev * pi * pi;
		upper = 4.0 * (nrev + 1.0) * (nrev + 1.0) * pi * pi;

		// ---- streamline since we know it's near the center
		upper = lower + (upper - lower) * 0.6;
		lower = lower + (upper - lower) * 0.3;

		// ---- initial estimate, just put in center 
		psiold = (upper + lower) * 0.5;
		findc2c3(psiold, c2, c3);

		loops = 0;
		dtdpsi = 200.0;
		while ((fabs(dtdpsi) >= 0.1) && (loops < numiter))
		{
			if (fabs(c2) > small)
				y = magr1 + magr2 - (vara * (1.0 - psiold * c3) / sqrt(c2));
			else
				y = magr1 + magr2;
			if (fabs(c2) > small)
			{
				x = sqrt(y / c2);
				oox = 1.0 / x;
			}
			else
			{
				x = 0.0;
				oox = 0.0;
			}
			sqrty = sqrt(y);
			if (fabs(psiold) > 1e-5)
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
				c2dot = -2.0 / MathTimeLib::factorial(4) + 2.0 * psiold / MathTimeLib::factorial(6) - 3.0 * psiold2 / MathTimeLib::factorial(8) +
					4.0 * psiold3 / MathTimeLib::factorial(10) - 5.0 * psiold4 / MathTimeLib::factorial(12);
				c3dot = -1.0 / MathTimeLib::factorial(5) + 2.0 * psiold / MathTimeLib::factorial(7) - 3.0 * psiold2 / MathTimeLib::factorial(9) +
					4.0 * psiold3 / MathTimeLib::factorial(11) - 5.0 * psiold4 / MathTimeLib::factorial(13);
				c2ddot = 0.0;
				c3ddot = 0.0;
			}
			// now solve this for dt = 0.0
			//            dtdpsi = x^3*(c3dot - 3.0*c3*c2dot/(2.0*c2))* oomu + 0.125*vara/sqrt(mu) * (3.0*c3*sqrty/c2 + vara/x);
			dtdpsi = x * x * x * (c3dot - 3.0 * c3 * c2dot / (2.0 * c2)) * oomu + 0.125 * vara * (3.0 * c3 * sqrty / c2 + vara * oox) * oomu;

			q = 0.25 * vara * sqrt(c2) - x * x * c2dot;
			x3 = x * x * x;
			x5 = x3 * x * x;
			s1 = -24.0 * q * x3 * c2 * sqrty * c3dot;
			s2 = 36.0 * q * x3 * sqrty * c3 * c2dot - 16.0 * x5 * sqrty * c3ddot * c2 * c2;
			s3 = 24.0 * x5 * sqrty * (c3dot * c2dot * c2 + c3 * c2ddot * c2 - c3 * c2dot * c2dot) - 6.0 * vara * c3dot * y * c2 * x * x;
			s4 = -0.75 * vara * vara * c3 * pow(c2, 1.5) * x * x + 6.0 * vara * c3 * y * c2dot * x * x +
				(vara * vara * c2 * (0.25 * vara * sqrt(c2) - x * x * c2)) * sqrty * oox; // C(z)??

			dtdpsi2 = -(s1 + s2 + s3 + s4) / (16.0 * sqrtmu * (c2 * c2 * sqrty * x * x));
			// NR update
			psinew = psiold - dtdpsi / dtdpsi2;

			//fprintf(" %3i %12.4f %12.4f %12.4f %12.4f %11.4f %12.4f %12.4f %11.4f %11.4f \n", 
			//    loops, y, dtdpsi, psiold, psinew, psinew - psiold, dtdpsi, dtdpsi2, lower, upper);
			psiold = psinew;
			findc2c3(psiold, c2, c3);
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
	*  inputs          description                    range / units
	*    r1          - ijk position vector 1          km
	*    r2          - ijk position vector 2          km
	*    dm          - direction of motion            'L','S'
	*    de          - direction of energy            'L', 'H'
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
	*    vallado       2013, 494, Alg 59, ex 7-5
	*    prussing      JAS 2000
	-----------------------------------------------------------------------------*/

	void lambertminT
	(
		double r1[3], double r2[3], char dm, char de, int nrev,
		double& tmin, double& tminp, double& tminenergy
	)
	{
		double a, s, chord, magr1, magr2, cosdeltanu;
		double alpha, alp, beta, fa, fadot, xi, eta, del, an;
		int i;

		magr1 = MathTimeLib::mag(r1);
		magr2 = MathTimeLib::mag(r2);
		cosdeltanu = MathTimeLib::dot(r1, r2) / (magr1 * magr2);
		// make sure it's not more than 1.0
		if (fabs(cosdeltanu) > 1.0)
			cosdeltanu = 1.0 * MathTimeLib::sgn(cosdeltanu);

		// these are the same
		chord = sqrt(magr1 * magr1 + magr2 * magr2 - 2.0 * magr1 * magr2 * cosdeltanu);
		//chord = MathTimeLib::mag(r2 - r1);

		s = (magr1 + magr2 + chord) * 0.5;

		xi = 0.0;
		eta = 0.0;

		// could do this just for nrev cases, but you c an alos find these for any nrev if (nrev > 0)
		// ------------- calc tmin parabolic tof to see if the orbit is possible
		if (dm == 'S')
			tminp = (1.0 / 3.0) * sqrt(2.0 / mu) * (pow(s, 1.5) - pow(s - chord, 1.5));
		else
			tminp = (1.0 / 3.0) * sqrt(2.0 / mu) * (pow(s, 1.5) + pow(s - chord, 1.5));

		// ------------- this is the min energy ellipse tof
		double amin = 0.5 * s;
		alpha = pi;
		beta = 2.0 * asin(sqrt((s - chord) / s));
		if (dm == 'S')
			tminenergy = pow(amin, 1.5) * ((2.0 * nrev + 1.0) * pi - beta + sin(beta)) / sqrt(mu);
		else
			tminenergy = pow(amin, 1.5) * ((2.0 * nrev + 1.0) * pi + beta - sin(beta)) / sqrt(mu);

		// -------------- calc min tof ellipse (prussing 1992 aas, 2000 jas)
		an = 1.001 * amin;
		fa = 10.0;
		i = 1;
		double rad = 180.0 / pi;
		//string[] tempstr = new string[25];
		while (fabs(fa) > 0.00001 && i <= 20)
		{
			a = an;
			alp = 1.0 / a;
			double temp = sqrt(0.5 * s * alp);
			if (fabs(temp) > 1.0)
				temp = MathTimeLib::sgn(temp) * 1.0;
			alpha = 2.0 * asin(temp);
			// now account for direct or retrograde
			temp = sqrt(0.5 * (s - chord) * alp);
			if (fabs(temp) > 1.0)
				temp = MathTimeLib::sgn(temp) * 1.0;
			if (de == 'L')
				beta = 2.0 * asin(temp);
			else
				beta = -2.0 * asin(temp);  // fix quadrant
			xi = alpha - beta;
			eta = sin(alpha) - sin(beta);
			fa = (6.0 * nrev * pi + 3.0 * xi - eta) * (sin(xi) + eta) - 8.0 * (1.0 - cos(xi));

			fadot = ((6.0 * nrev * pi + 3.0 * xi - eta) * (cos(xi) + cos(alpha)) +
				(3.0 - cos(alpha)) * (sin(xi) + eta) - 8.0 * sin(xi)) * (-alp * tan(0.5 * alpha))
				+ ((6.0 * nrev * pi + 3.0 * xi - eta) * (-cos(xi) - cos(beta)) +
				(-3.0 + cos(beta)) * (sin(xi) + eta) + 8.0 * sin(xi)) * (-alp * tan(0.5 * beta));
			del = fa / fadot;
			an = a - del;
			//tempstr[i] = (alpha * rad).ToString("0.00000000") + " " + (beta * rad).ToString("0.00000000") + " " +
			//           (xi).ToString("0.00000000") + " " + eta.ToString("0.00000000") + " " + fadot.ToString("0.00000000") + " " + fa.ToString("0.00000000") + " " +
			//           an.ToString("0.00000000");
			i = i + 1;
		}
		// could update beta one last time with alpha too????
		if (dm == 'S')
			tmin = pow(an, 1.5) * (2.0 * pi * nrev + xi - eta) / sqrt(mu);
		else
			tmin = pow(an, 1.5) * (2.0 * pi * nrev + xi + eta) / sqrt(mu);
	}  // lambertminT



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
	*  author        : david vallado                  719-573-2600   22 jun 2002
	*
	*  inputs          description                            range / units
	*    r1          -  position vector 1                          km
	*    r2          -  position vector 2                          km
	*    dm          - direction of motion                        'L','S'
	*    de          - direction of energy                        'L', 'H'
	*    dtsec       - time between r1 and r2                     sec
	*
	*  outputs       :
	*    v1          -  velocity vector                          km/s
	*    v2          -  velocity vector                          km/s
	*    error       - error flag                                 1, 2, 3, ... use numbers since c++ is so horrible at strings
	*        error = 1;   // g not converged
	*        error = 2;   // y negative
	*        error = 3;   // impossible 180 transfer
	*
	*  locals        :
	*    vara        - variable of the iteration,
	*                  not the semi or axis!
	*    y           - area between position vectors
	*    upper       - upper bound for z
	*    lower       - lower bound for z
	*    cosdeltanu  - cosine of true anomaly change              rad
	*    f           - f expression
	*    g           - g expression
	*    gdot        - g dot expression
	*    xold        - old universal variable x
	*    xoldcubed   - xold cubed
	*    zold        - old value of z
	*    znew        - new value of z
	*    c2new       - c2(z) function
	*    c3new       - c3(z) function
	*    timenew     - new time                                   s
	*    small       - tolerance for roundoff errors
	*    i, j        - index
	*
	*  coupling
	*    MathTimeLib::mag         - MathTimeLib::magnitude of a vector
	*    dot         - dot product of two vectors
	*    findc2c3    - find c2 and c3 functions
	*
	*  references    :
	*    vallado       2013, 492, alg 58, ex 7-5
	-----------------------------------------------------------------------------*/

	void lambertuniv
	(
		double r1[3], double r2[3], char dm, char de, int nrev, double dtsec, double tbi[5][5], double altpad,
		double v1t[3], double v2t[3], char& hitearth, int& error, FILE *outfile
	)
	{
		const double small = 0.0000001;
		const int numiter = 40;

		int loops, ynegktr;
		double rp, vara, y, upper, lower, cosdeltanu, f, g, gdot, xold, xoldcubed, magr1, magr2,
			psiold, psinew, psilast, c2new, c3new, dtnew, dtold, c2dot, c3dot, dtdpsi, psiold2, a, tmp;

		/* --------------------  initialize values   -------------------- */
		error = 0;
		psinew = 0.0;

		magr1 = MathTimeLib::mag(r1);
		magr2 = MathTimeLib::mag(r2);

		cosdeltanu = MathTimeLib::dot(r1, r2) / (magr1 * magr2);
		if (dm == 'L')
			vara = -sqrt(magr1 * magr2 * (1.0 + cosdeltanu));
		else
			vara = sqrt(magr1 * magr2 * (1.0 + cosdeltanu));

		/* -------- set up initial bounds for the bissection ------------ */
		if (nrev == 0)
		{
			upper = 4.0 * pi * pi;  // could be negative infinity for all cases
			lower = -4.0 * pi * pi; // allow hyperbolic and parabolic solutions
		}
		else
		{
			lower = 4.0 * nrev * nrev * pi * pi;
			upper = 4.0 * (nrev + 1.0) * (nrev + 1.0) * pi * pi;
			if (de == 'H')   // high way is always the 1st half
				upper = tbi[nrev][1];
			else
				lower = tbi[nrev][1];
		}

		/* ----------------  form initial guesses   --------------------- */
		psinew = 0.0;
		xold = 0.0;
		if (nrev == 0)
		{
			// use log to get initial guess
			// empirical relation here from 10000 random draws
			// 10000 cases up to 85000 dtsec  0.11604050x + 9.69546575
			psiold = (log(dtsec) - 9.61202327) / 0.10918231;
			if (psiold > upper)
				psiold = upper - pi;
		}
		else
		{
			psiold = lower + (upper - lower)*0.5;
		}
		findc2c3(psiold, c2new, c3new);

		double oosqrtmu = 1.0 / sqrt(mu);

		// find initial dtold from psiold
		if (fabs(c2new) > small)
			y = magr1 + magr2 - (vara * (1.0 - psiold * c3new) / sqrt(c2new));
		else
			y = magr1 + magr2;
		if (fabs(c2new) > small)
			xold = sqrt(y / c2new);
		else
			xold = 0.0;
		xoldcubed = xold * xold * xold;
		dtold = (xoldcubed * c3new + vara * sqrt(y)) * oosqrtmu;

		// --------  determine if the orbit is possible at all ---------- 
		if (fabs(vara) > small)  // 0.2??
		{
			loops = 0;
			ynegktr = 1; // y neg ktr
			dtnew = -10.0;
			while ((fabs(dtnew - dtsec) >= small) && (loops < numiter) && (ynegktr <= 10))
			{
				if (fabs(c2new) > small)
					y = magr1 + magr2 - (vara * (1.0 - psiold * c3new) / sqrt(c2new));
				else
					y = magr1 + magr2;

				// ------- check for negative values of y ------- 
				if ((vara > 0.0) && (y < 0.0))
				{
					ynegktr = 1;
					while ((y < 0.0) && (ynegktr < 10))
					{
						psinew = 0.8 * (1.0 / c3new) * (1.0 - (magr1 + magr2) * sqrt(c2new) / vara);

						/* ------ find c2 and c3 functions ------ */
						findc2c3(psinew, c2new, c3new);
						psiold = psinew;
						lower = psiold;
						if (fabs(c2new) > small)
							y = magr1 + magr2 - vara * (1.0 - psiold * c3new) / sqrt(c2new);
						else
							y = magr1 + magr2;
						//          if (show == 'y')
						//            if (fileout != null)
						//              fprintf(fileout, "%3d %10.5f %10.5f %10.5f %7.3f %9.5f %9.5f\n",
						//                      loops, psiold, y, xold, dtnew, vara, upper, lower);

						ynegktr++;
					}
				}

				if (ynegktr < 10)
				{
					if (fabs(c2new) > small)
						xold = sqrt(y / c2new);
					else
						xold = 0.0;
					xoldcubed = xold * xold * xold;
					dtnew = (xoldcubed * c3new + vara * sqrt(y)) * oosqrtmu;

					// try newton rhapson iteration to update psi
					if (fabs(psiold) > 1e-5)
					{
						c2dot = 0.5 / psiold * (1.0 - psiold * c3new - 2.0 * c2new);
						c3dot = 0.5 / psiold * (c2new - 3.0 * c3new);
					}
					else
					{
						psiold2 = psiold * psiold;
						c2dot = -1.0 / MathTimeLib::factorial(4) + 2.0 * psiold / MathTimeLib::factorial(6)
							- 3.0 * psiold2 / MathTimeLib::factorial(8) +
							4.0 * psiold2 * psiold / MathTimeLib::factorial(10)
							- 5.0 * psiold2*psiold2 / MathTimeLib::factorial(12);
						c3dot = -1.0 / MathTimeLib::factorial(5) + 2.0 * psiold / MathTimeLib::factorial(7)
							- 3.0 * psiold2 / MathTimeLib::factorial(9) +
							4.0 * psiold2 * psiold / MathTimeLib::factorial(11)
							- 5.0 * psiold2*psiold2 / MathTimeLib::factorial(13);
					}
					dtdpsi = (xoldcubed * (c3dot - 3.0 * c3new * c2dot / (2.0 * c2new))
						+ vara / 8.0 * (3.0 * c3new * sqrt(y) / c2new + vara / xold)) / sqrt(mu);
					psinew = psiold - (dtnew - dtsec) / dtdpsi;
					double psitmp = psinew;

					// check if newton guess for psi is outside bounds(too steep a slope)
					if (abs(psinew) > upper || psinew < lower)
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
					}


					// -------------- find c2 and c3 functions ---------- 
					findc2c3(psinew, c2new, c3new);
					psilast = psiold;  // keep previous iteration
					psiold = psinew;
					dtold = dtnew;
					loops = loops + 1;

					// ---- make sure the first guess isn't too close --- 
					if ((fabs(dtnew - dtsec) < small) && (loops == 1))
						dtnew = dtsec - 1.0;
				}

			}

			if ((loops >= numiter) || (ynegktr >= 10))
			{
				error = 1; // g not converged

				if (ynegktr >= 10)
				{
					error = 2;  // y negative
				}
			}
			else
			{
				// ---- use f and g series to find velocity vectors ----- 
				f = 1.0 - y / magr1;
				gdot = 1.0 - y / magr2;
				g = 1.0 / (vara * sqrt(y / mu)); // 1 over g
				//	fdot = sqrt(y) * (-magr2 - magr1 + y) / (magr2 * magr1 * vara);
				for (int i = 0; i < 3; i++)
				{
					v1t[i] = ((r2[i] - f * r1[i]) * g);
					v2t[i] = ((gdot * r2[i] - r1[i]) * g);
				}
				checkhitearth(altpad, r1, v1t, r2, v2t, nrev, hitearth);
			}
		}
		else
			error = 3;   // impossible 180 transfer

	}  // lambertuniv



	// utility functions for lambertbattin, etc 
	/* ------------------------------------------------------------------------------
	*                           function lambhodograph
	*
	* this function accomplishes 180 deg transfer(and 360 deg) for lambert problem.
	*
	*  author        : david vallado                  719 - 573 - 2600   22 may 2017
	*
	*  inputs          description                          range / units
	*    r1 -  position vector 1                               km
	*    r2 -  position vector 2                               km
	*    dtsec - time between r1 and r2                         s
	*    dnu - true anomaly change                            rad
	*
	*  outputs       :
	*    v1t -  transfer velocity vector                   km / s
	*    v2t -  transfer velocity vector                   km / s
	*
	*  references :
	*    Thompson JGCD 2013 v34 n6 1925
	*    Thompson AAS GNC 2018
	* ------------------------------------------------------------------------------ */

	void lambhodograph
	(
		double r1[3], double v1[3], double r2[3], double p, double ecc, double dnu, double dtsec,
		double v1t[3], double v2t[3]
	)
	{
		double eps, magr1, magr2, a, b, x1, x2, nvec[3], y2a, y2b, ptx, oomagr1, oomagr2, oop, rcrv[3], rcrr[3];
		int i;
            oop = 1.0 / p;

            magr1 = MathTimeLib::mag(r1);
            oomagr1 = 1.0 / magr1;
            magr2 = MathTimeLib::mag(r2);
            oomagr2 = 1.0 / magr2;
            eps = 0.001 / magr2;  // 1e-14

		magr1 = MathTimeLib::mag(r1);
		magr2 = MathTimeLib::mag(r2);

		a = mu * (1.0 * oomagr1 - 1.0 * oop);  // not the semi - major axis
		b = pow(mu*ecc * oop, 2) - a * a;
		if (b <= 0.0)
			x1 = 0.0;
		else
			x1 = -sqrt(b);

		// 180 deg, and multiple 180 deg transfers
		if (fabs(sin(dnu)) < eps)
		{
			MathTimeLib::cross(r1, v1, rcrv);
			for (i = 0; i < 3; i++)
				nvec[i] = rcrv[i] / MathTimeLib::mag(rcrv);
			if (ecc < 1.0)
			{
				ptx = twopi * sqrt(p * p * p / pow(mu * (1.0 - ecc * ecc), 3));
				if (fmod(dtsec, ptx) > ptx * 0.5)
					x1 = -x1;
			}
		}
		else
		{
			y2a = mu * oop - x1 * sin(dnu) + a * cos(dnu);
			y2b = mu * oop + x1 * sin(dnu) + a * cos(dnu);
			if (fabs(mu *oomagr2 - y2b) < fabs(mu *oomagr2 - y2a))
				x1 = -x1;

			// depending on the cross product, this will be normal or in plane,
			// or could even be a fan
			MathTimeLib::cross(r1, r2, rcrr);
			for (i = 0; i < 3; i++)
				nvec[i] = rcrr[i] / MathTimeLib::mag(rcrr); // if this is r1, v1, the transfer is coplanar!
			if (fmod(dnu, twopi) > pi)
			{
				for (i = 0; i < 3; i++)
					nvec[i] = -nvec[i];
			}
		}

		MathTimeLib::cross(nvec, r1, rcrv);
		MathTimeLib::cross(nvec, r2, rcrr);
		x2 = x1 * cos(dnu) + a * sin(dnu);
		for (i = 0; i < 3; i++)
		{
			v1t[i] = (sqrt(mu * p) / magr1) * ((x1 / mu)*r1[i] + rcrv[i]) * oomagr1;
			v2t[i] = (sqrt(mu * p) / magr2) * ((x2 / mu)*r2[i] + rcrr[i]) * oomagr2;
		}
	}  // lambhodograph




	static double kbattin
	(
		double v
	)
	{
		double sum1, delold, termold, del, term;
		double d[21] =
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
		//double del, delold, term, termold, sum1;
		int i;

		/* ---- process forwards ---- */
		sum1 = d[0];
		delold = 1.0;
		termold = d[0];
		sum1 = termold;
		i = 1;
		while ((i <= 20) && (fabs(termold) > 0.00000001))
		{
			del = 1.0 / (1.0 - d[i] * v * delold);
			term = termold * (del - 1.0);
			sum1 = sum1 + term;

			i = i + 1;
			delold = del;
			termold = term;
		}
		//return sum1;

		int ktr = 20;
		double sum2 = 0.0;
		double term2 = 1.0 + d[ktr] * v;
		for (i = 1; i <= ktr - 1; i++)
		{
			sum2 = d[ktr - i] * v / term2;
			term2 = 1.0 + sum2;
		}

		return (d[0] / term2);
	}  // kbattin


	/* -------------------------------------------------------------------------- */

	static double seebattin(double v2)
	{
		double c[21]
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
		// first term is diff, indices are offset too
		double c1[20]
		{
			9.0 / 7.0, 16.0 / 63.0,
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

		double term, termold, del, delold, sum1, eta, sqrtopv;
		int i;

		sqrtopv = sqrt(1.0 + v2);
		eta = v2 / pow(1.0 + sqrtopv, 2);

		// ---- process forwards ---- 
		delold = 1.0;
		termold = c[0];  // * eta
		sum1 = termold;
		i = 1;
		while ((i <= 20) && (fabs(termold) > 0.000001))
		{
			del = 1.0 / (1.0 + c[i] * eta * delold);
			term = termold * (del - 1.0);
			sum1 = sum1 + term;

                i = i + 1;
			delold = del;
			termold = term;
		}

		//   return ((1.0 / (8.0 * (1.0 + sqrtopv))) * (3.0 + sum1 / (1.0 + eta * sum1)));
		double seebatt = 1.0 / ((1.0 / (8.0*(1.0 + sqrtopv))) * (3.0 + sum1 / (1.0 + eta * sum1)));

		int ktr = 19;
		double sum2 = 0.0;
		double term2 = 1.0 + c1[ktr] * eta;
		for (i = 0; i <= ktr - 1; i++)
		{
			sum2 = c1[ktr - i] * eta / term2;
			term2 = 1.0 + sum2;
		}

		return (8.0*(1.0 + sqrtopv) /
			(3.0 +
			(1.0 /
				(5.0 + eta + ((9.0 / 7.0)*eta / term2)))));
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
	*  inputs          description                               range / units
        *    r1          - ijk position vector 1                      km
        *    r2          - ijk position vector 2                      km
        *    v1          - ijk velocity vector 1 if avail             km/s
        *    dm          - direction of motion                       'L', 'S'
        *    de          - orbital energy                            'L', 'H'
	*    dtsec       - time between r1 and r2                      sec
	*
	*  outputs       :
	*    v1          -  velocity vector                              km/s
	*    v2          -  velocity vector                              km/s
	*    error       - error flag                                 1, 2, 3, ... use numbers since c++ is so horrible at strings
	*        error = 1;   // a = 0.0
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
	*    arcsin      - arc sine function
	*    arccos      - arc cosine function
	*    MathTimeLib::mag         - MathTimeLib::magnitude of a vector
	*    arcsinh     - inverse hyperbolic sine
	*    arccosh     - inverse hyperbolic cosine
	*    sinh        - hyperbolic sine
	*    power       - raise a base to a power
	*    atan2       - arc tangent function that resolves quadrants
	*
	*  references    :
	*    vallado       2013, 494, Alg 59, ex 7-5
        *    thompson      AAS GNC 2018
	-----------------------------------------------------------------------------*/

	void lambertbattin
	(
		double r1[3], double r2[3], double v1[3], char dm, char de, int nrev, double dtsec, double tbi[5][5], double altpad,
		double v1t[3], double v2t[3], char& hitearth, int& error, FILE *outfile
	)
	{
		const double small = 0.0000000001;
		double rcrossr[3];
		int   i, loops;
		double   u, b, x, xn, y, L, m, cosdeltanu, sindeltanu, dnu, a,
			ror, h1, h2, tempx, eps, denom, chord, k2, s,
			p, ecc, f, A, y1, bigt;
		double magr1, magr2, magrcrossr, lam, temp, temp1, temp2, v1dvl[3], v2dvl[3], v2[3];

		f = 0.0;
		error = 0;
		magr1 = MathTimeLib::mag(r1);
		magr2 = MathTimeLib::mag(r2);

		cosdeltanu = MathTimeLib::dot(r1, r2) / (magr1 * magr2);
		// make sure it's not more than 1.0
		if (abs(cosdeltanu) > 1.0)
			cosdeltanu = 1.0 * MathTimeLib::sgn(cosdeltanu);

		MathTimeLib::cross(r1, r2, rcrossr);
		magrcrossr = MathTimeLib::mag(rcrossr);
		if (dm == 'S')
			sindeltanu = magrcrossr / (magr1*magr2);
		else
			sindeltanu = -magrcrossr / (magr1*magr2);

		dnu = atan2(sindeltanu, cosdeltanu);
		// the angle needs to be positive to work for the long way
		if (dnu < 0.0)
			dnu = 2.0*pi + dnu;

		// these are the same
		chord = sqrt(magr1*magr1 + magr2 * magr2 - 2.0*magr1*magr2*cosdeltanu);
		//chord = mag(r2 - r1);

		s = (magr1 + magr2 + chord)*0.5;
		ror = magr2 / magr1;
		eps = ror - 1.0;

		lam = 1.0 / s * sqrt(magr1*magr2) * cos(dnu*0.5);
		L = pow((1.0 - lam) / (1.0 + lam), 2);
		m = 8.0*mu*dtsec*dtsec / (s * s * s * pow(1.0 + lam, 6));

		// initial guess
		if (nrev > 0)
			xn = 1.0 + 4.0*L;
		else
			xn = L;   //l    // 0.0 for par and hyp, l for ell

		// alt approach for high energy(long way, retro multi - rev) case
		if ((de == 'H') && (nrev > 0))
		{
			xn = 1e-20;  // be sure to reset this here!!
			x = 10.0;  // starting value
			loops = 1;
			while ((abs(xn - x) >= small) && (loops <= 20))
			{
				x = xn;
				temp = 1.0 / (2.0*(L - x * x));
				temp1 = sqrt(x);
				temp2 = (nrev*pi*0.5 + atan(temp1)) / temp1;
				h1 = temp * (L + x) * (1.0 + 2.0*x + L);
				h2 = temp * m * temp1 * ((L - x * x) * temp2 - (L + x));

				b = 0.25*27.0*h2 / (pow(temp1 * (1.0 + h1), 3));
				if (b < -1.0) // reset the initial condition
					f = 2.0 * cos(1.0 / 3.0 * acos(sqrt(b + 1.0)));
				else
				{
					A = pow(sqrt(b) + sqrt(b + 1.0), (1.0 / 3.0));
					f = A + 1.0 / A;
				}

				y = 2.0 / 3.0 * temp1 * (1.0 + h1) *(sqrt(b + 1.0) / f + 1.0);
				xn = 0.5 * ((m / (y*y) - (1.0 + L)) - sqrt(pow(m / (y*y) - (1.0 + L), 2) - 4.0*L));
				fprintf(outfile, " %3i yh %11.6f x %11.6f h1 %11.6f h2 %11.6f b %11.6f f %11.7f \n", loops, y, x, h1, h2, b, f);
				loops = loops + 1;
			}  // while
			x = xn;
			a = s * pow(1.0 + lam, 2) * (1.0 + x)*(L + x) / (8.0 * x);
			p = (2.0 * magr1 * magr2 * (1.0 + x) * pow(sin(dnu * 0.5), 2)) / (s * pow(1.0 + lam, 2) * (L + x));  // thompson
			ecc = sqrt(1.0 - p / a);
			lambhodograph(r1, v1, r2, p, ecc, dnu, dtsec, v1t, v2t);
			fprintf(outfile, "high v1t %16.8f %16.8f %16.8f \n", v1t[0], v1t[1], v1t[2]);
		}
		else
		{
			// standard processing low energy
			// note that the dr nrev = 0 case is not represented
			loops = 1;
			y1 = 0.0;
			x = 10.0;  // starting value
			while ((abs(xn - x) >= small) && (loops <= 30))
			{
				if (nrev > 0)
				{
					x = xn;
					temp = 1.0 / ((1.0 + 2.0*x + L) * (4.0 * x * x));
					temp1 = (nrev*pi*0.5 + atan(sqrt(x))) / sqrt(x);
					h1 = temp * pow(L + x, 2) * (3.0 * pow(1.0 + x, 2) * temp1 - (3.0 + 5.0*x));
					h2 = temp * m * ((x*x - x * (1.0 + L) - 3.0*L) * temp1 + (3.0*L + x));
				}
				else
				{
					x = xn;
					tempx = seebattin(x);
					denom = 1.0 / ((1.0 + 2.0*x + L) * (4.0*x + tempx * (3.0 + x)));
					h1 = pow(L + x, 2) * (1.0 + 3.0*x + tempx)*denom;
					h2 = m * (x - L + tempx)*denom;
				}

				// ---------------------- - evaluate cubic------------------
				b = 0.25*27.0*h2 / (pow(1.0 + h1, 3));

				u = 0.5*b / (1.0 + sqrt(1.0 + b));
				k2 = kbattin(u);
				y = ((1.0 + h1) / 3.0)*(2.0 + sqrt(1.0 + b) / (1.0 + 2.0*u*k2*k2));
				xn = sqrt(pow((1.0 - L)*0.5, 2) + m / (y*y)) - (1.0 + L)*0.5;

				y1 = sqrt(m / ((L + x)*(1.0 + x)));
				loops = loops + 1;
				//        fprintf(1, ' %3i yb %11.6f x %11.6f k2 %11.6f b %11.6f u %11.6f y1 %11.7f \n', loops, y, x, k2, b, u, y1);
			}  // while

		}

		if (loops < 30)
		{
			// blair approach use y from solution
			//       lam = 1.0 / s * sqrt(magr1*magr2) * cos(dnu*0.5);
			//       m = 8.0*mu*dtsec*dtsec / (s ^ 3 * (1.0 + lam) ^ 6);
			//       L = ((1.0 - lam) / (1.0 + lam)) ^ 2;
			//a = s*(1.0 + lam) ^ 2 * (1.0 + x)*(lam + x) / (8.0*x);
			// p = (2.0*magr1*magr2*(1.0 + x)*sin(dnu*0.5) ^ 2) ^ 2 / (s*(1 + lam) ^ 2 * (lam + x));  % loechler, not right ?
			p = (2.0 * magr1 * magr2 * y * y * pow(1.0 + x, 2) * pow(sin(dnu*0.5), 2)) / (m*s*pow(1.0 + lam, 2));  // thompson
			ecc = sqrt((eps * eps + 4.0*magr2 / magr1 * pow(sin(dnu * 0.5), 2) * pow((L - x) / (L + x), 2)) / (eps * eps + 4.0*magr2 / magr1 * pow(sin(dnu * 0.5), 2)));
			lambhodograph(r1, v1, r2, p, ecc, dnu, dtsec, v1t, v2t);
			//            fprintf(1, 'oldb v1t %16.8f %16.8f %16.8f %16.8f\n', v1dv, mag(v1dv));

			// Battin solution to orbital parameters(and velocities)
			// thompson 2011, loechler 1988
			if (dnu > pi)
				lam = -sqrt((s - chord) / s);
			else
				lam = sqrt((s - chord) / s);

			// loechler pg 21 seems correct!
			for (i = 0; i < 3; i++)
			{
				v1dvl[i] = 1.0 / (lam*(1.0 + lam))*sqrt(mu*(1.0 + x) / (2.0* s * s * s * (L + x)))*((r2[i] - r1[i]) + s * pow(1.0 + lam, 2) * (L + x) / (magr1*(1.0 + x))*r1[i]);
				// added v2
				v2dvl[i] = 1.0 / (lam*(1.0 + lam))*sqrt(mu*(1.0 + x) / (2.0* s *s *s * (L + x)))*((r2[i] - r1[i]) - s * pow(1.0 + lam, 2) * (L + x) / (magr2*(1.0 + x))*r2[i]);
			}
			//fprintf(1, 'loe v1t %16.8f %16.8f %16.8f %16.8f\n', v1dvl, mag(v1dvl));
			//fprintf(1, 'loe v2t %16.8f %16.8f %16.8f %16.8f\n', v2dvl, mag(v2dvl));
		}  // if loops converged < 30

		//  if (fileout != null)
		//    fprintf(fileout, "%8.5f %3d\n", testamt, loops);
		if (dtsec < 0.001)
			fprintf(outfile, " \n");
		else
			fprintf(outfile, "x %3i %c %5i %12.5f %12.5f %16.8f %16.8f %16.8f %16.8f %12.9f %12.9f %12.9f %12.9f \n",
				nrev, dm, loops, dtsec, dtsec, y, f, v1[0], v1[1], v1[2], v2[0], v2[1], v2[2]);

		bigt = sqrt(8.0 / (s * s * s)) * dtsec;
	}  // lambertbattin




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
	*  inputs          description                         range / units
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

	void initjplde
	(
		std::vector<jpldedata> &jpldearr,
		char infilename[140],
		double& jdjpldestart, double& jdjpldestartFrac
	)
	{
		jpldearr.resize(jpldesize);

		FILE *infile;
		double rs2, jdtdb, jdtdbf;
		char longstr[170];
		long i;

		// ---- open files select compatible files!!
#ifdef _MSC_VER
		errno_t jpldeInFileErrors = fopen_s(&infile, infilename, "r");
#else
		infile = fopen(infilename, "r");
#endif		

		// ---- start finding of data
		// ---- find epoch date
		fgets(longstr, 170, infile);

		i = 0;
		// ---- first record is in the string above
#ifdef _MSC_VER
		sscanf_s(longstr, "%i %i %i %lf %lf %lf %lf %lf %lf %lf %lf ",
			&jpldearr[i].year, &jpldearr[i].mon, &jpldearr[i].day, &jpldearr[i].rsun[0],
			&jpldearr[i].rsun[1], &jpldearr[i].rsun[2], &jpldearr[i].rsmag, &rs2,
			&jpldearr[i].rmoon[0], &jpldearr[i].rmoon[1], &jpldearr[i].rmoon[2]);
#else
		sscanf_s(longstr, "%i %i %i %lf %lf %lf %lf %lf %lf %lf %lf ",
			&jpldearr[i].year, &jpldearr[i].mon, &jpldearr[i].day, &jpldearr[i].rsun[0],
			&jpldearr[i].rsun[1], &jpldearr[i].rsun[2], &jpldearr[i].rsmag, &rs2,
			&jpldearr[i].rmoon[0], &jpldearr[i].rmoon[1], &jpldearr[i].rmoon[2]);
#endif
		MathTimeLib::jday(jpldearr[i].year, jpldearr[i].mon, jpldearr[i].day, 0, 0, 0.0, jdjpldestart, jdjpldestartFrac);
		jpldearr[i].mjd = jdjpldestart + jdjpldestartFrac;

		// ---- process observed records
		for (i = 1; i <= 51830 - 1; i++)  // number of days 1 jan 1957 - 2100
		{
			// use d format for integers with leading 0's
#ifdef _MSC_VER
			fscanf_s(infile, "%i %i %i %lf %lf %lf %lf %lf %lf %lf %lf ",
				&jpldearr[i].year, &jpldearr[i].mon, &jpldearr[i].day, &jpldearr[i].rsun[0],
				&jpldearr[i].rsun[1], &jpldearr[i].rsun[2], &jpldearr[i].rsmag, &rs2,
				&jpldearr[i].rmoon[0], &jpldearr[i].rmoon[1], &jpldearr[i].rmoon[2]);
#else
			fscanf(infile, "%i %i %i %lf %lf %lf %lf %lf %lf %lf %lf ",
				&jpldearr[i].year, &jpldearr[i].mon, &jpldearr[i].day, &jpldearr[i].rsun[0],
				&jpldearr[i].rsun[1], &jpldearr[i].rsun[2], &jpldearr[i].rsmag, &rs2,
				&jpldearr[i].rmoon[0], &jpldearr[i].rmoon[1], &jpldearr[i].rmoon[2]);
#endif
			MathTimeLib::jday(jpldearr[i].year, jpldearr[i].mon, jpldearr[i].day, 0, 0, 0.0, jdtdb, jdtdbf);
			jpldearr[i].mjd = jdtdb + jdtdbf - 2400000.5;
		}

		fclose(infile);
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
	*  inputs          description                                  range / units
	*    jdtdb         - epoch julian date                        days from 4713 BC
	*    jdtdbF        - epoch julian date fraction               day fraction from jdutc
	*    interp        - interpolation                             n-none, l-linear, s-spline
	*    jpldearr      - array of jplde data records
	*    jdjpldestart  - julian date of the start of the jpldearr data (set in initjplde)
	*
	*  outputs       :
	*    dut1        - delta ut1 (ut1-utc)                               sec
	*    dat         - number of leap seconds                            sec
	*    lod         - excess length of day                              sec
	*    xp          - x component of polar motion                       rad
	*    yp          - y component of polar motion                       rad
	*    ddpsi       - correction to delta psi (iau-76 theory)           rad
	*    ddeps       - correction to delta eps (iau-76 theory)           rad
	*    dx          - correction to x (cio theory)                      rad
	*    dy          - correction to y (cio theory)                      rad
	*    x           - x component of cio                                rad
	*    y           - y component of cio                                rad
	*    s           -                                                   rad
	*    deltapsi    - nutation longitude angle                          rad
	*    deltaeps    - obliquity of the ecliptic correction              rad
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

	void findjpldeparam
	(
		double  jdtdb, double jdtdbF, char interp,
		const std::vector<jpldedata> &jpldearr,  // pass by reference, not modify
		double jdjpldestart,
		double rsun[3], double& rsmag,
		double rmoon[3], double& rmmag
	)
	{
		long recnum;
		int  off1, off2;
		jpldedata jplderec, nextjplderec;
		double  fixf, jdjpldestarto, mjd, jdb, mfme;

		// the ephemerides are centered on jdtdb, but it turns out to be 0.5, or 0000 hrs.
		// check if any whole days in jdF
		jdb = floor(jdtdb + jdtdbF) + 0.5;  // want jd at 0 hr
		mfme = (jdtdb + jdtdbF - jdb) * 1440.0;
		if (mfme < 0.0)
			mfme = 1440.0 + mfme;

		//printf("jdtdb %lf  %lf  %lf  %lf \n ", jdtdb, jdtdbF, jdb, mfme);

		// ---- read data for day of interest
		jdjpldestarto = floor(jdtdb + jdtdbF - jdjpldestart);
		recnum = int(jdjpldestarto);

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
				rsmag = MathTimeLib::mag(rsun);
				rmoon[0] = jplderec.rmoon[0] + (nextjplderec.rmoon[0] - jplderec.rmoon[0]) * fixf;
				rmoon[1] = jplderec.rmoon[1] + (nextjplderec.rmoon[1] - jplderec.rmoon[1]) * fixf;
				rmoon[2] = jplderec.rmoon[2] + (nextjplderec.rmoon[2] - jplderec.rmoon[2]) * fixf;
				rmmag = MathTimeLib::mag(rmoon);
				//printf("sunm %i rsmag %lf fixf %lf n %lf nxt %lf \n", recnum, rsmag, fixf, jplderec.rsun[0], nextjplderec.rsun[0]);
				//printf("recnum l %i fixf %lf %lf rsun %lf %lf %lf \n", recnum, fixf, jplderec.rsun[0], rsun[0], rsun[1], rsun[2]);
			}

			// ---- do spline interpolations
			if (interp == 's')
			{
				off2 = 2;   // every 5 days data...
				off1 = 1;
				mfme = mfme / 1440.0; // get back to days for this since each step is in days
				// setup so the interval is in between points 2 and 3
				rsun[0] = MathTimeLib::cubicinterp(jpldearr[recnum - off1].rsun[0], jpldearr[recnum].rsun[0],
					jpldearr[recnum + off1].rsun[0], jpldearr[recnum + off2].rsun[0],
					jpldearr[recnum - off1].mjd, jpldearr[recnum].mjd, jpldearr[recnum + off1].mjd,
					jpldearr[recnum + off2].mjd, jpldearr[recnum].mjd + mfme);
				rsun[1] = MathTimeLib::cubicinterp(jpldearr[recnum - off1].rsun[1], jpldearr[recnum].rsun[1],
					jpldearr[recnum + off1].rsun[1], jpldearr[recnum + off2].rsun[1],
					jpldearr[recnum - off1].mjd, jpldearr[recnum].mjd, jpldearr[recnum + off1].mjd,
					jpldearr[recnum + off2].mjd, jpldearr[recnum].mjd + mfme);
				rsun[2] = MathTimeLib::cubicinterp(jpldearr[recnum - off1].rsun[2], jpldearr[recnum].rsun[2],
					jpldearr[recnum + off1].rsun[2], jpldearr[recnum + off2].rsun[2],
					jpldearr[recnum - off1].mjd, jpldearr[recnum].mjd, jpldearr[recnum + off1].mjd,
					jpldearr[recnum + off2].mjd, jpldearr[recnum].mjd + mfme);
				rsmag = MathTimeLib::mag(rsun);
				rmoon[0] = MathTimeLib::cubicinterp(jpldearr[recnum - off1].rmoon[0], jpldearr[recnum].rmoon[0],
					jpldearr[recnum + off1].rmoon[0], jpldearr[recnum + off2].rmoon[0],
					jpldearr[recnum - off1].mjd, jpldearr[recnum].mjd, jpldearr[recnum + off1].mjd,
					jpldearr[recnum + off2].mjd, jpldearr[recnum].mjd + mfme);
				rmoon[1] = MathTimeLib::cubicinterp(jpldearr[recnum - off1].rmoon[1], jpldearr[recnum].rmoon[1],
					jpldearr[recnum + off1].rmoon[1], jpldearr[recnum + off2].rmoon[1],
					jpldearr[recnum - off1].mjd, jpldearr[recnum].mjd, jpldearr[recnum + off1].mjd,
					jpldearr[recnum + off2].mjd, jpldearr[recnum].mjd + mfme);
				rmoon[2] = MathTimeLib::cubicinterp(jpldearr[recnum - off1].rmoon[2], jpldearr[recnum].rmoon[2],
					jpldearr[recnum + off1].rmoon[2], jpldearr[recnum + off2].rmoon[2],
					jpldearr[recnum - off1].mjd, jpldearr[recnum].mjd, jpldearr[recnum + off1].mjd, jpldearr[recnum + off2].mjd,
					jpldearr[recnum].mjd + mfme);
				rmmag = MathTimeLib::mag(rmoon);
				//printf("recnum s %i mfme %lf days rsun %lf %lf %lf \n", recnum, mfme, rsun[0], rsun[1], rsun[2]);
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
	*  inputs          description                                 range / units
	*    jdtdb         - epoch julian date                       days from 4713 BC
	*    jdtdbF        - epoch julian date fraction              day fraction from jdutc
	*    interp        - interpolation                            n-none, l-linear, s-spline
	*    jpldearr      - array of jplde data records
	*    jdjpldestart  - julian date of the start of the jpldearr data (set in initjplde)
	*
	*  outputs       :
	*    rsun        -  position vector of the sun au
	*    rtasc       - right ascension                             rad
	*    decl        - declination                                 rad
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
	*    hr          - hours                                       0 .. 24              10
	*    min         - minutes                                     0 .. 59              15
	*    sec         - seconds                                     0.0  .. 59.99          30.00
	*    temp        - temporary variable
	*    deg         - degrees
	*
	*  coupling      :
	*    none.
	*
	*  references    :
	*    vallado       2013, 279, alg 29, ex 5-1
	* --------------------------------------------------------------------------- */

	void sunmoonjpl
	(
		double jdtdb, double jdtdbF,
		char interp,
		const std::vector<jpldedata> &jpldearr,
		double jdjpldestart,
		double rsun[3], double& rtascs, double& decls,
		double rmoon[3], double& rtascm, double& declm
	)
	{
		double deg2rad;
		double rsmag, rmmag, temp;
		double small = 0.000001;

		deg2rad = pi / 180.0;

		// -------------------------  implementation   -----------------
		// -------------------  initialize values   --------------------
		findjpldeparam(jdtdb, jdtdbF, interp, jpldearr, jdjpldestart, rsun, rsmag, rmoon, rmmag);

		temp = sqrt(rsun[0] * rsun[0] + rsun[1] * rsun[1]);
		if (temp < small)
			// rtascs = atan2(v[1], v[0]);
			rtascs = 0.0;
		else
			rtascs = atan2(rsun[1], rsun[0]);
		decls = asin(rsun[2] / rsmag);

		temp = sqrt(rmoon[0] * rmoon[0] + rmoon[1] * rmoon[1]);
		if (temp < small)
			// rtascm = atan2(v[1], v[0]);
			rtascm = 0.0;
		else
			rtascm = atan2(rmoon[1], rmoon[0]);
		declm = asin(rmoon[2] / rmmag);


		//// evaluate DE series
		//const char *ephfile_name = "D:/Dataorig/Planet/DE200/jpleph";  // 1940-2100 or so
		//      #define JPL_MAX_N_CONSTANTS 1018
		//char nams[JPL_MAX_N_CONSTANTS][6], buff[102];
		//double vals[JPL_MAX_N_CONSTANTS];
		//void *ephem = jpl_init_ephemeris(ephfile_name, nams, vals);
		//double jdarr;
		//jdarr = 2453238.5;
		//// jdarr = 0.349572634765;
		//int calcopt[13];
		//int ntarg = 3; // earth
		//int ncent = 11;  // sun 10 = moon
		////   jpl_state(jdarr, calcopt, ntarg, ncent, rrd[], calc_velocity);
		//int calc_velocity = 1;  // true, 0 if not to calc vel
		//double rrd[6];
		//double au = 149597870.0;  // au 2 km
		//int err_code = jpl_pleph(ephem, jdarr, ntarg, ncent, rrd, calc_velocity);
		//printf("%i sun - earth %d %d %d %d %d %d \n", err_code, rrd[0] * au, rrd[1] * au, rrd[2] * au, rrd[3], rrd[4], rrd[5]);

		//err_code = jpl_pleph(ephem, jdarr, 5, 12, rrd, calc_velocity);
		//printf("%i sol sys bary - jup %d %d %d %d %d %d \n", err_code, rrd[0] * au, rrd[1] * au, rrd[2] * au, rrd[3], rrd[4], rrd[5]);

		//err_code = jpl_pleph(ephem, jdarr, 13, 11, rrd, calc_velocity);
		//printf("%i sun - earthm bary %d %d %d %d %d %d \n", err_code, rrd[0] * au, rrd[1] * au, rrd[2] * au, rrd[3], rrd[4], rrd[5]);

		//err_code = jpl_pleph(ephem, jdarr, 10, 3, rrd, calc_velocity);
		//printf("%i earth - moon %d %d %d %d %d %d \n", err_code, rrd[0] * au, rrd[1] * au, rrd[2] * au, rrd[3], rrd[4], rrd[5]);
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
	*  inputs          description                             range / units
	*    jdtdb         - epoch julian date                    days from 4713 BC
	*    jdtdbF        - epoch julian date fraction           day fraction from jdutc
	*
	*  outputs       :
	*    rsun        -  position vector of the sun au
	*    rtasc       - right ascension                             rad
	*    decl        - declination                                 rad
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
	*    hr          - hours                                       0 .. 24              10
	*    min         - minutes                                     0 .. 59              15
	*    sec         - seconds                                     0.0  .. 59.99          30.00
	*    temp        - temporary variable
	*    deg         - degrees
	*
	*  coupling      :
	*    none.
	*
	*  references    :
	*    vallado       2013, 279, alg 29, ex 5-1
	* --------------------------------------------------------------------------- */

	void sun
	(
		double jdtdb, double jdtdbF,
		double rsun[3], double& rtasc, double& decl
	)
	{
		double deg2rad;
		double tut1, meanlong, ttdb, meananomaly, eclplong, obliquity, magr;

		deg2rad = pi / 180.0;

		// -------------------------  implementation   -----------------
		// -------------------  initialize values   --------------------
		tut1 = (jdtdb + jdtdbF - 2451545.0) / 36525.0;

		meanlong = 280.460 + 36000.77 * tut1;
		meanlong = fmod(meanlong, 360.0);  //deg

		ttdb = tut1;
		meananomaly = 357.5277233 + 35999.05034 * ttdb;
		meananomaly = fmod(meananomaly * deg2rad, twopi);  //rad
		if (meananomaly < 0.0)
		{
			meananomaly = twopi + meananomaly;
		}
		eclplong = meanlong + 1.914666471 * sin(meananomaly)
			+ 0.019994643 * sin(2.0 * meananomaly); //deg
		obliquity = 23.439291 - 0.0130042 * ttdb;  //deg
		meanlong = meanlong * deg2rad;
		if (meanlong < 0.0)
		{
			meanlong = twopi + meanlong;
		}
		eclplong = eclplong * deg2rad;
		obliquity = obliquity * deg2rad;

		// --------- find magnitude of sun vector, ) components ------
		magr = 1.000140612 - 0.016708617 * cos(meananomaly)
			- 0.000139589 * cos(2.0 *meananomaly);    // in au's

		rsun[0] = magr * cos(eclplong);
		rsun[1] = magr * cos(obliquity) * sin(eclplong);
		rsun[2] = magr * sin(obliquity) * sin(eclplong);

		rtasc = atan(cos(obliquity) * tan(eclplong));

		// --- check that rtasc is in the same quadrant as eclplong ----
		if (eclplong < 0.0)
		{
			eclplong = eclplong + twopi;    // make sure it's in 0 to 2pi range
		}
		if (fabs(eclplong - rtasc) > pi * 0.5)
		{
			rtasc = rtasc + 0.5 * pi * MathTimeLib::round((eclplong - rtasc) / (0.5 * pi));
		}
		decl = asin(sin(obliquity) * sin(eclplong));

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
	*  inputs          description                            range / units
	*    jdtdb         - epoch julian date                    days from 4713 BC
	*    jdtdbF        - epoch julian date fraction            day fraction from jdutc
	*
	*  outputs       :
	*    rmoon       -  position vector of moon                    km
	*    rtasc       - right ascension                             rad
	*    decl        - declination                                 rad
	*
	*  locals        :
	*    eclplong    - ecliptic longitude
	*    eclplat     - eclpitic latitude
	*    hzparal     - horizontal parallax
	*    l           - geocentric direction cosines
	*    m           -             "     "
	*    n           -             "     "
	*    ttdb        - julian centuries of tdb from
	*                  jan 1, 2000 12h
	*    hr          - hours                                       0 .. 24
	*    min         - minutes                                     0 .. 59
	*    sec         - seconds                                     0.0  .. 59.99
	*    deg         - degrees
	*
	*  coupling      :
	*    none.
	*
	*  references    :
	*    vallado       2013, 288, alg 31, ex 5-3
	* --------------------------------------------------------------------------- */

	void moon
	(
		double jdtdb, double jdtdbF,
		double rmoon[3], double& rtasc, double& decl
	)
	{
		double deg2rad, magr;
		double ttdb, l, m, n, eclplong, eclplat, hzparal, obliquity;

		deg2rad = pi / 180.0;

		// -------------------------  implementation   -----------------
		ttdb = (jdtdb + jdtdbF - 2451545.0) / 36525.0;

		eclplong = 218.32 + 481267.8813 * ttdb
			+ 6.29 * sin((134.9 + 477198.85 * ttdb) * deg2rad)
			- 1.27 * sin((259.2 - 413335.38 * ttdb) * deg2rad)
			+ 0.66 * sin((235.7 + 890534.23 * ttdb) * deg2rad)
			+ 0.21 * sin((269.9 + 954397.70 * ttdb) * deg2rad)
			- 0.19 * sin((357.5 + 35999.05 * ttdb) * deg2rad)
			- 0.11 * sin((186.6 + 966404.05 * ttdb) * deg2rad);      // deg

		eclplat = 5.13 * sin((93.3 + 483202.03 * ttdb) * deg2rad)
			+ 0.28 * sin((228.2 + 960400.87 * ttdb) * deg2rad)
			- 0.28 * sin((318.3 + 6003.18 * ttdb) * deg2rad)
			- 0.17 * sin((217.6 - 407332.20 * ttdb) * deg2rad);      // deg

		hzparal = 0.9508 + 0.0518 * cos((134.9 + 477198.85 * ttdb)
			* deg2rad)
			+ 0.0095 * cos((259.2 - 413335.38 * ttdb) * deg2rad)
			+ 0.0078 * cos((235.7 + 890534.23 * ttdb) * deg2rad)
			+ 0.0028 * cos((269.9 + 954397.70 * ttdb) * deg2rad);    // deg

		eclplong = fmod(eclplong * deg2rad, twopi);
		eclplat = fmod(eclplat * deg2rad, twopi);
		hzparal = fmod(hzparal * deg2rad, twopi);

		obliquity = 23.439291 - 0.0130042 * ttdb;  //deg
		obliquity = obliquity * deg2rad;

		// ------------ find the geocentric direction cosines ----------
		l = cos(eclplat) * cos(eclplong);
		m = cos(obliquity) * cos(eclplat) * sin(eclplong) - sin(obliquity) * sin(eclplat);
		n = sin(obliquity) * cos(eclplat) * sin(eclplong) + cos(obliquity) * sin(eclplat);

		// ------------- calculate moon position vector ----------------
		magr = re / sin(hzparal);  // km
		rmoon[0] = magr * l;
		rmoon[1] = magr * m;
		rmoon[2] = magr * n;

		// -------------- find rt ascension and declination ------------
		rtasc = atan2(m, l);
		decl = asin(n);
	}  // moon


	// IOD type routines -----------------------------------------------------
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
	*  inputs          description                               range / units
	*    latgd       - geodetic latitude                        -pi/2 to pi/2 rad
	*    lon         - longitude of site                        -2pi to 2pi rad
	*    alt         - altitude                                     km
	*
	*  outputs       :
	*    rsecef      - ecef site position vector                     km
	*    vsecef      - ecef site velocity vector                     km/s
	*
	*  locals        :
	*    sinlat      - variable containing  sin(lat)                 rad
	*    temp        - temporary real value
	*    rdel        - rdel component of site vector                 km
	*    rk          - rk component of site vector                   km
	*    cearth      -
	*
	*  coupling      :
	*    none
	*
	*  references    :
	*    vallado       2013, 430, alg 51, ex 7-1
	----------------------------------------------------------------------------*/

	void site
	(
		double latgd, double lon, double alt,
		double rsecef[3], double vsecef[3]
	)
	{
		const double rearth = 6378.137;  // km
		const double eesqrd = 0.00669437999013;
		double   sinlat, cearth, rdel, rk;

		// ---------------------  initialize values   ------------------- 
		sinlat = sin(latgd);

		// -------  find rdel and rk components of site vector  --------- 
		cearth = rearth / sqrt(1.0 - (eesqrd * sinlat * sinlat));
		rdel = (cearth + alt) * cos(latgd);
		rk = ((1.0 - eesqrd) * cearth + alt) * sinlat;

		// ----------------  find site position vector  ----------------- 
		rsecef[0] = rdel * cos(lon);
		rsecef[1] = rdel * sin(lon);
		rsecef[2] = rk;

		// ----------------  find site velocity vector  ----------------- 
		vsecef[0] = 0.0;
		vsecef[1] = 0.0;
		vsecef[2] = 0.0;
	}  // site


	// -------------------------- angles-only techniques ------------------------ 


	/*------------------------------------------------------------------------------
	*
	*                           procedure anglesgauss
	*
	*  this procedure solves the problem of orbit determination using three
	*    optical sightings.  the solution procedure uses the gaussian technique.
	*
	*  author        : david vallado                  719-573-2600   22 jun 2002
	*
	*  inputs          description                              range / units
	*    trtasc1      - right ascension #1                           rad
	*    trtasc2      - right ascension #2                           rad
	*    trtasc3      - right ascension #3                           rad
	*    tdecl1       - declination #1                               rad
	*    tdecl2       - declination #2                               rad
	*    tdecl3       - declination #3                               rad
	*    jd1          - julian date of 1st sighting            days from 4713 bc
	*    jd2          - julian date of 2nd sighting            days from 4713 bc
	*    jd3          - julian date of 3rd sighting            days from 4713 bc
	*    rs           -  site position vector                        er
	*
	*  outputs        :
	*    r2            -  position vector at t2                    er
	*    v2            -  velocity vector at t2                    er / tu
	*
	*  locals         :
	*    l1           - line of sight vector for 1st
	*    l2           - line of sight vector for 2nd
	*    l3           - line of sight vector for 3rd
	*    tau          - taylor expansion series about
	*                   tau ( t - to )
	*    tausqr       - tau squared
	*    t21t23       - (t2-t1) * (t2-t3)
	*    t31t32       - (t3-t1) * (t3-t2)
	*    i            - index
	*    d            -
	*    rho          - range from site to sat at t2                 er
	*    rhodot       -
	*    dmat         -
	*    rs1          - site vectors
	*    rs2          -
	*    rs3          -
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
	*    MathTimeLib::mag          - MathTimeLib::magnitude of a vector
	*    detrminant   - evaluate the determinant of a matrix
	*    factor       - find roots of a polynomial
	*    matmult      - multiply two matrices together
	*    gibbs        - gibbs method of orbit determination
	*    hgibbs       - herrick gibbs method of orbit determination
	*    angle        - angle between two vectors
	*
	*  references     :
	*    vallado       2013, 442, alg 52, ex 7-2
	-----------------------------------------------------------------------------*/

	void anglesgauss
	(
		double tdecl1, double tdecl2, double tdecl3,
		double trtasc1, double trtasc2, double trtasc3,
		double jd1, double jd2, double jd3,
		double rs1[3], double rs2[3], double rs3[3], double r2[3], double v2[3]
	)
	{
		const double tuday = 0.00933809017716;
		const double muc = 1.0;
		const double small = 0.0000001;
		const double rad = 180.0 / pi;
		int i, j, ll;
		char error[12];
		double poly[16];
		double roots[15][2];
		double r1[3], r3[3], l1[3], l2[3], l3[3];
		//	std::vector< std::vector<double> > lmatii(3, 3), cmat(3, 3), rhomat(3, 3),
		//		lmati(3, 3), rsmat(3, 3), lir(3, 3);
		std::vector< std::vector<double> > lmatii = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > cmat = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > rhomat = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > lmati = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > rsmat = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > lir = std::vector< std::vector<double> >(3, std::vector<double>(3));

		double rdot, tau1, tau3, u, udot, p, f1, g1, f3, g3, a, ecc, incl, raan, argp,
			nu, m, eccanom, l, argper, bigr2, a1, a1u, a3, a3u, d, d1, d2, c1, c3, l2dotrs, magr1, magr2,
			rhoold1, rhoold2, rhoold3, temproot, theta, theta1, copa, tausqr;

		/* ----------------------   initialize   ------------------------ */
		jd1 = jd1 / tuday;   // convert days to tu
		jd2 = jd2 / tuday;
		jd3 = jd3 / tuday;
		/* ---- set middle to 0, deltas to other times ---- */
		tau1 = jd1 - jd2;
		tau3 = jd3 - jd2;

		/* ----------------  find line of sight vectors  ---------------- */
		l1[0] = cos(tdecl1) * cos(trtasc1);
		l1[1] = cos(tdecl1) * sin(trtasc1);
		l1[2] = sin(tdecl1);

		l2[0] = cos(tdecl2) * cos(trtasc2);
		l2[1] = cos(tdecl2) * sin(trtasc2);
		l2[2] = sin(tdecl2);

		l3[0] = cos(tdecl3) * cos(trtasc3);
		l3[1] = cos(tdecl3) * sin(trtasc3);
		l3[2] = sin(tdecl3);

		/* -------------- find l matrix and determinant ----------------- */
		/* -------- called lmati since it is only used for determ ------- */
		for (i = 0; i < 3; i++)
		{
			l1[0] = lmatii[i][0];
			l2[1] = lmatii[i][1];
			l3[2] = lmatii[i][2];
			rs1[0] = rsmat[i][0];
			rs2[1] = rsmat[i][1];
			rs3[2] = rsmat[i][2];
		}

		d = MathTimeLib::determinant(lmatii, 3);

		/* ------------------- now asMathTimeLib::sgn the inverse ------------------- */
		lmati[0][0] = (l2[1] * l3[2] - l2[2] * l3[1]) / d;
		lmati[1][1] = (-l1[1] * l3[2] + l1[2] * l3[1]) / d;
		lmati[2][2] = (l1[1] * l2[2] - l1[2] * l2[1]) / d;
		lmati[0][0] = (-l2[0] * l3[2] + l2[2] * l3[0]) / d;
		lmati[1][1] = (l1[0] * l3[2] - l1[2] * l3[0]) / d;
		lmati[2][2] = (-l1[0] * l2[2] + l1[2] * l2[0]) / d;
		lmati[0][0] = (l2[0] * l3[1] - l2[1] * l3[0]) / d;
		lmati[1][1] = (-l1[0] * l3[1] + l1[1] * l3[0]) / d;
		lmati[2][2] = (l1[0] * l2[1] - l1[1] * l2[0]) / d;

		MathTimeLib::matmult(lmati, rsmat, lir, 3, 3, 3);

		/* ------------- find f and g series at 1st and 3rd obs --------- */
		/* speed by assuming circ sat vel for udot here ??                */
		/* some similartities in 1/6t3t1 ...                              */
		/* ---- keep separated this time ----                             */
		a1 = tau3 / (tau3 - tau1);
		a1u = (tau3 * ((tau3 - tau1) * (tau3 - tau1) - tau3 * tau3)) /
			(6.0 * (tau3 - tau1));
		a3 = -tau1 / (tau3 - tau1);
		a3u = -(tau1 * ((tau3 - tau1) * (tau3 - tau1) - tau1 * tau1)) /
			(6.0 * (tau3 - tau1));

		/* ---- form initial guess of r2 ---- */
		d1 = lir[1][0] * a1 - lir[1][1] + lir[1][2] * a3;
		d2 = lir[1][0] * a1u + lir[1][2] * a3u;

		/* -------- solve eighth order poly not same as laplace --------- */
		l2dotrs = MathTimeLib::dot(l2, rs2);
		poly[0] = 1.0;  // r2
		poly[1] = 0.0;
		poly[2] = -(d1 * d1 + 2.0 * d1 * l2dotrs + MathTimeLib::mag(rs2) * MathTimeLib::mag(rs2));
		poly[3] = 0.0;
		poly[4] = 0.0;
		poly[5] = -2.0* muc * (l2dotrs * d2 + d1 * d2);
		poly[6] = 0.0;
		poly[7] = 0.0;
		poly[8] = -muc * muc * d2 * d2;
		poly[9] = 0.0;
		poly[10] = 0.0;
		poly[11] = 0.0;
		poly[12] = 0.0;
		poly[13] = 0.0;
		poly[14] = 0.0;
		poly[15] = 0.0;
		//// fixxxxxxxxxxxxxxxxxxxxxxxxxx
		///  factor(poly, 8, (double **) roots);

		/* ------------------- select the correct root ------------------ */
		bigr2 = 0.0;
		for (j = 0; j < 8; j++)
		{
			if (fabs(roots[j][1]) < small)
			{
				temproot = roots[j][0] * roots[j][0];
				temproot = temproot * temproot * temproot * temproot +
					poly[2] * temproot * temproot * temproot +
					poly[5] * roots[j][0] * temproot + poly[8];
				//      if (fileout != null)
				//      {
				//        fprintf(fileout, "root %d %0.7f + %0.7f j  value = %0.7f\n",
				//                          j, roots[j][0], roots[j][1], temproot);
				//        fprintf(fileout, "root %d %0.7f + %0.7f j  value = %0.7f\n",
				//                          j, roots[j][0], roots[j][1], temproot);
				//      }
				if (roots[j][0] > bigr2)
					bigr2 = roots[j][0];
			}
		}
		printf("input r2 ");
		scanf_s("%lf\n", &bigr2);

		/* ------------- solve matrix with u2 better known -------------- */
		u = muc / (bigr2 * bigr2 * bigr2);

		c1 = a1 + a1u * u;
		c3 = a3 + a3u * u;
		cmat[0][0] = -c1;
		cmat[1][0] = 1.0;
		cmat[2][0] = -c3;
		/// fixxxxxxx
		///  matmult(lir, cmat, rhomat);

		rhoold1 = rhomat[0][0] / c1;
		rhoold2 = -rhomat[1][0];
		rhoold3 = rhomat[2][0] / c3;

		//  if (fileout != null)
		//    fprintf(fileout, " now start refining the guess\n");

		/* --------- loop through the refining process ------------ */
		for (ll = 0; ll < 3; ll++)
		{
			//    if (fileout != null)
			//      fprintf(fileout, " iteration # %2d\n", ll);
			/* ---- now form the three position vectors ----- */
			for (i = 0; i < 3; i++)
			{
				r1[0] = rhomat[0][0] * l1[i] / c1 + rs1[i];
				r2[1] = -rhomat[1][0] * l2[i] + rs2[i];
				r3[2] = rhomat[2][0] * l3[i] / c3 + rs3[i];
			}

			gibbs(r1, r2, r3, v2, theta, theta1, copa, error);

			if ((strcmp(error, "ok") == 0) && (copa < 1.0 / rad))
			{
				/* hgibbs to get middle vector ---- */
				jd1 = jd1 * tuday;   // convert tu to days
				jd2 = jd2 * tuday;
				jd3 = jd3 * tuday;

				herrgibbs(r1, r2, r3, jd1, jd2, jd3, v2, theta, theta1, copa, error);

				//      if (fileout != null)
				//        fprintf(fileout, "hgibbs\n");
			}

			rv2coe(r2, v2, p, a, ecc, incl, raan, argp, nu, m, eccanom, u, l, argper);

			//    if (fileout != null)
			//    {
			//      fprintf(fileout, "t123 %18.7f %18.7f %18.7f tu\n", jd1, jd2, jd3);
			//      fprintf(fileout, "t123 %18.7f %18.7f %18.7f days\n",
			//                        jd1 * tuday, jd2 * tuday, jd3 * tuday);
			//      fprintf(fileout, "los 1    %12.6f %12.6f %12.6f\n",
			//                        l1[1), l1[2], l1[3]);
			//      fprintf(fileout, "los 2    %12.6f %12.6f %12.6f\n",
			//                        l2[1), l2[2], l2[3]);
			//      fprintf(fileout, "los 3    %12.6f %12.6f %12.6f\n",
			//                        l3[1), l3[2], l3[3]);
			//      fileprint(rsmat, " rsmat ", 3, fileout);
			//    }

			/// fixxxxxxxxxxxxxx
			///    lmatii = lmati.inverse();

			/*
			lir    = lmati * lmatii;
			lmati.display(" lmati matrix\n", 3);
			lir.display(" i matrix\n", 6);
			printf("%14.7f\n", d);
			if (fileout != null)
			fileprint(lmati,  " lmat matinv " ,3, fileout);
			lir = lmati * lmatii;
			lir.display(" should be i matrix ", 6);
			lir.display(" lir matrix ", 6);
			*/
			/*    if (fileout != null)
			{
			fprintf(fileout, "tau  %11.7f %11.7f tu %14.7f\n",tau1, tau3, u);
			fprintf(fileout, "a13, u %11.7f %11.7f %11.7f%11.7f\n", a1, a3, a1u, a3u);
			fprintf(fileout, "d1, d2 %11.7f %11.7f ldotr %11.7f\n", d1, d2, l2dotrs);
			fprintf(fileout, "coeff %11.7f %11.7f %11.7f %11.7f\n",
			poly[0], poly[2], poly[5], poly[8]);
			fileprint(cmat, " c matrix ", 3, fileout);
			fileprint(rhomat, " rho matrix ", 3, fileout);
			fprintf(fileout, "r1 %11.7f %11.7f %11.7f %11.7f\n",
			r1[1), r1[2], r1[3], r1.MathTimeLib::mag());
			fprintf(fileout, "r2 %11.7f %11.7f %11.7f %11.7f\n",
			r2[1), r2[2], r2[3], r2.MathTimeLib::mag());
			fprintf(fileout, "r3 %11.7f %11.7f %11.7f %11.7f\n",
			r3[1), r3[2], r3[3], r3.MathTimeLib::mag());
			fprintf(fileout, "hggau r2 %12.6 %12.6 %12.6 %s %11.7f ",
			r2[1), r2[2], r2[3], r2.MathTimeLib::mag(), error, copa * rad);
			fprintf(fileout, "%12.6 %12.6 %12.6\n", v2[1), v2[2], v2[3]);
			fprintf(fileout, "p=%11.7f  a%11.7f  e %11.7f i %11.7f omeg %10.6f \
			argp %10.6f\n",
			p, a, ecc, incl * rad, raan * rad, argp * rad);
			printf("p=%11.7f  a%11.7f  e %11.7f i %11.7f w%10.6f w%10.6f\n",
			p, a, ecc, incl * rad, raan * rad, argp * rad);
			}
			*/
			magr1 = MathTimeLib::mag(r1);
			magr2 = MathTimeLib::mag(r2);

			if (ll <= 2)
			{
				/* ---- now get an improved estimate of the f and g series ---- */
				/* or can the analytic functions be found now??                 */
				u = muc / (magr2 * magr2 * magr2);
				rdot = MathTimeLib::dot(r2, v2) / magr2;
				udot = (-3.0 * muc * rdot) / (pow(magr2, 4));

				tausqr = tau1 * tau1;
				f1 = 1.0 - 0.5 * u * tausqr -
					(1.0 / 6.0) * udot * tausqr * tau1 +
					(1.0 / 24.0) * u * u * tausqr * tausqr +
					(1.0 / 30.0) * u * udot * tausqr * tausqr * tau1;
				g1 = tau1 - (1.0 / 6.0) * u * tau1 * tausqr -
					(1.0 / 12.0) * udot * tausqr * tausqr +
					(1.0 / 120.0) * u * u * tausqr * tausqr * tau1 +
					(1.0 / 120.0) * u * udot * tausqr * tausqr * tausqr;
				tausqr = tau3 * tau3;
				f3 = 1.0 - 0.5 * u * tausqr -
					(1.0 / 6.0) * udot* tausqr * tau3 +
					(1.0 / 24.0) * u * u * tausqr * tausqr +
					(1.0 / 30.0) * u * udot * tausqr * tausqr * tau3;
				g3 = tau3 - (1.0 / 6.0) * u * tau3 * tausqr -
					(1.0 / 12.0) * udot * tausqr * tausqr +
					(1.0 / 120.0) * u * u * tausqr * tausqr * tau3 +
					(1.0 / 120.0) * u * udot * tausqr * tausqr * tausqr;
				//      if (fileout != null)
				//        fprintf(fileout, "tau1 %11.7f tau3 %11.7f u %14.7f %14.7f\n",
				//                          tau1, tau3, u, udot);
			}
			else
			{
				/* --- now use exact method to find f and g --- */
				theta = MathTimeLib::angle(r1, r2);
				theta1 = MathTimeLib::angle(r2, r3);

				f1 = 1.0 - ((magr1 * (1.0 - cos(theta)) / p));
				g1 = (magr1*magr2 * sin(-theta)) / sqrt(p); // - angle because backwards!!
				f3 = 1.0 - ((MathTimeLib::mag(r3) * cos(1.0 - cos(theta1)) / p));
				g3 = (MathTimeLib::mag(r3)*magr2 * sin(theta1)) / sqrt(p);
				//    if (fileout != null)
				//      fprintf(fileout, "f1n%11.7f %11.7f f3 %11.7f %11.7f\n", f1, g1, f3, g3);
				c1 = g3 / (f1 * g3 - f3 * g1);
				c3 = -g1 / (f1 * g3 - f3 * g1);
			}
			/* ---- solve for all three ranges via matrix equation ---- */
			cmat[0][0] = -c1;
			cmat[1][0] = 1.0;
			cmat[2][0] = -c3;
			/// fixxxxxxx
			///  matmult(lir, cmat, rhomat);

			//    if (fileout != null)
			//    {
			//      fprintf(fileout, "c1, c3 %11.7f %11.7f\n", c1, c3);
			//      fileprint(rhomat, " rho matrix ", 3, fileout);
			//    }
			/* ---- check for convergence ---- */
		}
		/* ---- find all three vectors ri ---- */
		for (i = 0; i < 3; i++)
		{
			r1[0] = (rhomat[0][0] * l1[i] / c1 + rs1[i]);
			r2[1] = (-rhomat[1][0] * l2[i] + rs2[i]);
			r3[2] = (rhomat[2][0] * l3[i] / c3 + rs3[i]);
		}
		//  if (fileout != null)
		//  {
		//    fprintf(fileout, "r1 %11.7f %11.7f %11.7f %11.7f",
		//                      r1[1), r1[2], r1[3], r1.MathTimeLib::mag());
		//    fprintf(fileout, "r2 %11.7f %11.7f %11.7f %11.7f",
		//                      r2[1), r2[2], r2[3], r2.MathTimeLib::mag());
		//    fprintf(fileout, "r3 %11.7f %11.7f %11.7f %11.7f",
		//                      r3[1), r3[2], r3[3], r3.MathTimeLib::mag());
		//  }
	}    // anglsegauss


	/*------------------------------------------------------------------------------
	*
	*                           procedure angleslaplace
	*
	*  this procedure solves the problem of orbit determination using three
	*    optical sightings and the method of laplace.
	*
	*  author        : david vallado                  719-573-2600   22 jun 2002
	*
	*  inputs          description                               range / units
	*    trtasc1      - right ascension #1                           rad
	*    trtasc2      - right ascension #2                           rad
	*    trtasc3      - right ascension #3                           rad
	*    tdecl1       - declination #1                               rad
	*    tdecl2       - declination #2                               rad
	*    tdecl3       - declination #3                               rad
	*    jd1          - julian date of 1st sighting                  days from 4713 bc
	*    jd2          - julian date of 2nd sighting                  days from 4713 bc
	*    jd3          - julian date of 3rd sighting                  days from 4713 bc
	*    rs1          -  site position vector #1                  er
	*    rs2          -  site position vector #2                  er
	*    rs3          -  site position vector #3                  er
	*
	*  outputs        :
	*    r2            -  position vector                         er
	*    v2            -  velocity vector                          er / tu
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
	*    MathTimeLib::mag          - MathTimeLib::magnitude of a vector
	*    determinant  - evaluate the determinant of a matrix
	*    cross        - cross product of two vectors
	*    norm         - normlize a matrix
	*    asMathTimeLib::sgnval    - asMathTimeLib::sgn a value to a matrix
	*    getval       - get a value from a matrix
	*    initmatrix   - initialize a matrix and fil with 0.0's

	*    delmatrix    - delete a matrix
	*    factor       - find the roots of a polynomial
	*
	*  references     :
	*    vallado       2013, 435
	-----------------------------------------------------------------------------*/
	/*
	void angleslaplace
	(
	double tdecl1, double tdecl2, double tdecl3,
	double trtasc1, double trtasc2, double trtasc3,
	double jd1, double jd2, double jd3,
	vector rs1, vector rs2, vector rs3, vector& r2, vector& v2
	)
	{
	const double raanearth = 0.05883359980154919;
	const double tuday      = 0.00933809017716;   // tuday:= 58.132440906;
	const double mu         = 1.0;
	const double small      = 0.0000001;

	double poly[16];
	double roots[15][2];
	matrix dmat(3, 3), dmat1(3, 3), dmat2(3, 3), dmat3(3, 3), dmat4(3, 3);
	vector l1(3), l2(3), l3(3), ldot(3), lddot(3), rs2dot(3), rs2ddot(3),
	earthrate(3), temp(3), temp1(3);
	double   d, d1, d2, d3, d4, rho, rhodot, t1t13, t1t3, t31t3, temproot,
	tau1, tau3, bigr2, l2dotrs;
	char   tc, chg;
	byte   schi;

	// ----------------------   initialize   ------------------------
	earthrate[0] = 0.0;
	earthrate[1] = 0.0;
	earthrate[2] = raanearth;

	jd1 = jd1 / tuday;   // days to tu
	jd2 = jd2 / tuday;   // days to tu
	jd3 = jd3 / tuday;   // days to tu

	// ---- set middle to 0, deltas to other times ----
	tau1 = jd1 - jd2;
	tau3 = jd3 - jd2;

	// --------------- find line of sight vectors -------------------
	l1[] = (cos(tdecl1) * cos(trtasc1), 1);
	l2[] = (cos(tdecl1) * sin(trtasc1), 2);
	l2[] = (sin(tdecl1), 3);
	l1[] = (cos(tdecl2) * cos(trtasc2), 1);
	l2[] = (cos(tdecl2) * sin(trtasc2), 2);
	l2[] = (sin(tdecl2), 3);
	l1[] = (cos(tdecl3) * cos(trtasc3), 1);
	l2[] = (cos(tdecl3) * sin(trtasc3), 2);
	l2[] = (sin(tdecl3), 3);

	// --------------------------------------------------------------
	// using lagrange interpolation formula to derive an expression
	// for l(t), substitute t=t2 and differentiate to obtain the
	// derivatives of l.
	// ---------------------------------------------------------------
	t1t13 = 1.0 / (tau1 * (tau1 - tau3));
	t1t3  = 1.0 / (tau1 * tau3);
	t31t3 = 1.0 / ((tau3 - tau1) * tau3);
	for (uint i = 1; i <= 3; i++)
	{
	ldot[] = ((-tau3 * t1t13) * l1[i) +
	((-tau1 - tau3) * t1t3) * l2[i) +
	(-tau1 * t31t3) * l3[i), i);
	lddot[] = ((2.0 * t1t13) * l1[i) +
	(2.0 * t1t3)  * l2[i) +
	(2.0 * t31t3) * l3[i), i);
	}

	// -------------------- find 2nd derivative of rs ---------------
	temp  = rs1.cross(rs2);
	temp1 = rs2.cross(rs3);

	// needs a different test xxxx!!
	if ((fabs(temp.MathTimeLib::mag()) > small) && (fabs(temp1.MathTimeLib::mag()) > small))
	{
	// ------- all sightings from one site ---------
	// fix this testhere
	for (uint i = 1; i <= 3; i++)
	{
	// esc pg 268  doesn't seem to work!!! xx
	rs2dot[] = ((-tau3 * t1t13) * rs1[i) +
	((-tau1 - tau3) * t1t3) * rs2[i) +
	(-tau1 * t31t3) * rs3[i), i);
	rs2ddot[] = ((2.0 * t1t13) * rs1[i) +
	(2.0 * t1t3 ) * rs2[i) +
	(2.0 * t31t3) * rs3[i), i);
	}

	rs2dot = earthrate.cross(rs2);
	rs2ddot = earthrate.cross(rs2dot);
	}
	else
	{
	// ---- each sighting from a different site ----
	for (uint i = 1; i <= 3; i++)
	{
	rs2dot[] = ((-tau3 * t1t13) * rs1[i) +
	((-tau1 - tau3) * t1t3) * rs2[i) +
	(-tau1 * t31t3) * rs3[i), i);
	rs2ddot[] = ((2.0 * t1t13) * rs1[i) +
	(2.0 * t1t3 ) * rs2[i) +
	(2.0 * t31t3) * rs3[i), i);
	}
	}
	for (uint i = 1; i <= 3; i++)
	{
	dmat[] = (2.0 * l2[i), i, 1);
	dmat[] = (2.0 * ldot[i), i, 2);
	dmat[] = (2.0 * ldot[i), i, 3);

	// ----  position determinants ----
	dmat1[] = (l2[i), i, 1);
	dmat1[] = (ldot[i), i, 2);
	dmat1[] = (rs2ddot[i), i, 3);
	dmat2[] = (l2[i), i, 1);
	dmat2[] = (ldot[i), i, 2);
	dmat2[] = (rs2[i), i, 3);

	// ----  velocity determinants ----
	dmat3[] = (l2[i), i, 1);
	dmat3[] = (rs2ddot[i), i, 2);
	dmat3[] = (lddot[i), i, 3);
	dmat4[] = (l2[i), i, 1);
	dmat4[] = (rs2[i), i, 2);
	dmat4[] = (lddot[i), i, 3);
	}

	d  = dmat.determinant();
	d1 = dmat1.determinant();
	d2 = dmat2.determinant();
	d3 = dmat3.determinant();
	d4 = dmat4.determinant();

	// --------------  iterate to find rho MathTimeLib::magnitude ---------------
	/*
	*     r2[4]:= 1.5;  { first guess }
	*     writeln( 'input initial guess for r2[4] ' );
	*     readln( r2[4] );
	*     i:= 1;
	*     repeat
	*         oldr:= r2[4];
	*         rho:= -2.0*d1/d - 2.0*d2/(r2[4]*r2[4]*r2[4]*d);
	*         r2[4]:= sqrt( rho*rho + 2.0*rho*l2dotrs + rs2[4]*rs2[4] );
	*         inc(i);
	*         r2[4]:= (oldr - r2[4] ) / 2.0;            // simple bissection }
	*         writeln( fileout,'rho guesses ',i:2,'rho ',rho:14:7,' r2[4] ',r2[4]:14:7,oldr:14:7 );
	*  // seems to converge, but wrong numbers
	*         inc(i);
	*     until ( abs( oldr-r2[4] ) < small ) or ( i >= 30 );


	if (fabs(d) > 0.000001)
	{
	// ---------------- solve eighth order poly -----------------
	l2dotrs  = l2.dot(rs2);
	poly[ 0] =  1.0; // r2^8th variable!!!!!!!!!!!!!!
	poly[ 1] =  0.0;
	poly[ 2] =  (l2dotrs * 4.0 * d1 / d - 4.0 * d1 * d1 / (d * d) -
	rs2.MathTimeLib::mag() * rs2.MathTimeLib::mag());
	poly[ 3] =  0.0;
	poly[ 4] =  0.0;
	poly[ 5] =  mu * (l2dotrs * 4.0 * d2 / d - 8.0 * d1 * d2 / (d * d));
	poly[ 6] =  0.0;
	poly[ 7] =  0.0;
	poly[ 8] = -4.0 * mu * d2 * d2 / (d * d);
	poly[ 9] =  0.0;
	poly[10] =  0.0;
	poly[11] =  0.0;
	poly[12] =  0.0;
	poly[13] =  0.0;
	poly[14] =  0.0;
	poly[15] =  0.0;
	factor(poly, 8, (double **)roots);

	// ---- find correct (xx) root ----
	bigr2 = 0.0;
	for (uint j = 0; j < 8; j++)
	if (fabs(roots[j][2]) < small)
	{
	printf("root %d %f + %f\n", j+1, roots[j][1], roots[j][2]);
	temproot = roots[j][0] * roots[j][0];
	temproot = temproot * temproot * temproot * temproot +
	poly[2]  * temproot * temproot * temproot +
	poly[5]  * roots[j][0] * temproot + poly[8];
	if (fileout != null)
	fprintf(fileout, "root %d %f + %f j  value = %f\n",
	j, roots[j][0], roots[j][1], temproot);
	if (roots[j][0] > bigr2)
	bigr2 = roots[j][0];
	}
	printf("bigr2 %14.7f\n", bigr2);
	if (fileout != null)
	fprintf(fileout, "bigr2 %14.7f\n", bigr2);
	printf("keep this root ? ");
	scanf("%f\n", &bigr2);

	rho = -2.0 * d1 / d - 2.0 * mu * d2 / (bigr2 * bigr2 * bigr2 * d);

	// ---- find the middle position vector ----
	for (uint k = 1; k <= 3; k++)
	r2[] = (rho * l2[k) + rs2[k), k);

	// -------- find rhodot MathTimeLib::magnitude --------
	rhodot = -d3 / d - mu * d4 / (r2.MathTimeLib::mag() * r2.MathTimeLib::mag() * r2.MathTimeLib::mag() * d);
	if (fileout != null)
	{
	fprintf(fileout, "rho %14.7f\n", rho);
	fprintf(fileout, "rhodot %14.7f\n", rhodot);
	}

	// ----- find middle velocity vector -----
	for (uint i = 1; i <= 3; i++)
	v2[] = (rhodot * l2[i) + rho * ldot[i) + rs2dot[i), i);
	}
	else
	printf("determinant value was zero %f\n", d);

	//  if (fileout != null)
	//  {
	fprintf(fileout, "t123 %18.7f %18.7f %18.7f tu\n", jd1, jd2, jd3);
	fprintf(fileout, "t123 %18.7f %18.7f %18.7f days\n",
	jd1 * tuday, jd2 * tuday, jd3 * tuday);
	fprintf(fileout, "tau  %11.7f %11.7f tu\n", tau1, tau3);
	fprintf(fileout, "tau  %11.7f %11.7f min\n",
	tau1 * 13.446849, tau3 * 13.446849);
	fprintf(fileout, "delta123 %12.6f %12.6f %12.6f\n",
	tdecl1 * 57.2957, tdecl2 * 57.2957, tdecl3 * 57.2957);
	fprintf(fileout, "rtasc123 %12.6f %12.6f %12.6f\n",
	trtasc1 * 57.2957, trtasc2 * 57.2957, trtasc3 * 57.2957);
	fprintf(fileout, "rtasc1   %12.6f %12.6f %12.6f\n",
	rs1[1], rs1[2], rs1[3]);
	fprintf(fileout, "rtasc 2  %12.6f %12.6f %12.6f\n",
	rs2[1], rs2[2], rs2[3]);
	fprintf(fileout, "rtasc  3 %12.6f %12.6f %12.6f\n",
	rs3[1], rs3[2], rs3[3]);
	fprintf(fileout, "los 1    %12.6f %12.6f %12.6f\n",
	l1[1], l1[2], l1[3]);
	fprintf(fileout, "los 2    %12.6f %12.6f %12.6f\n",
	l2[1], l2[2], l2[3]);
	fprintf(fileout, "los 3    %12.6f %12.6f %12.6f\n",
	l3[1], l3[2], l3[3]);
	fprintf(fileout, "ldot     %12.6f %12.6f %12.6f\n",
	ldot[1], ldot[2], ldot[3]);
	fprintf(fileout, "lddot    %12.6f %12.6f %12.6f\n",
	lddot[1], lddot[2], lddot[3]);
	fprintf(fileout, "rs2     %12.6f %12.6f %12.6f\n",
	rs2[1], rs2[2], rs2[3]);
	fprintf(fileout, "rs2dot  %12.6f %12.6f %12.6f\n",
	rs2dot[1], rs2dot[2], rs2dot[3]);
	fprintf(fileout, "rs2ddot %12.6f %12.6f %12.6f\n",
	rs2ddot[1], rs2ddot[2], rs2ddot[3]);
	fprintf(fileout, "d 01234 = %12.6f %12.6f %12.6f %12.6f %12.6f\n",
	d, d1, d2, d3, d4);
	//  }
	dmat.display(" d matrix ", 6);
	dmat1.display(" d1 matrix ", 6);
	dmat2.display(" d2 matrix ", 6);
	dmat3.display(" d3 matrix ", 6);
	dmat4.display(" d4 matrix ", 6);
	}

	/* -------------------------- conversion techniques ------------------------- */



	/* ------------------------- three vector techniques ------------------------ */

	/* -----------------------------------------------------------------------------
	*
	*                           procedure gibbs
	*
	*  this procedure performs the gibbs method of orbit determination.
	*
	*  author        : david vallado                  719-573-2600   22 jun 2002
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
	*    r1mr2       - MathTimeLib::magnitude of r1 - r2
	*    r3mr1       - MathTimeLib::magnitude of r3 - r1
	*    r2mr3       - MathTimeLib::magnitude of r2 - r3
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
	*    lncom2      - multiply two vectors by two constants
	*    lncom3      - add three vectors each multiplied by a constant
	*    norm        - creates a unit vector
	*    angle       - angle between two vectors
	*
	*  references    :
	*    vallado       2013, 460, alg 54, ex 7-3
	-----------------------------------------------------------------------------*/

	void gibbs
	(
		double r1[3], double r2[3], double r3[3],
		double v2[3], double& theta, double& theta1, double& copa, char error[14]
	)
	{
		const double small = 0.000001;
		double tover2, l, r1mr2, r3mr1, r2mr3, magr1, magr2;
		double p[3], q[3], w[3], d[3], n[3], s[3], b[3], pn[3], r1n[3], dn[3], nn[3];

		/* --------------------  initialize values   -------------------- */
#ifdef _MSC_VER
		strcpy_s(error, sizeof(error), "ok");
#else
		strcpy(error, "ok");
#endif
		magr1 = MathTimeLib::mag(r1);
		magr2 = MathTimeLib::mag(r2);

		theta = 0.0;
		theta1 = 0.0;
		copa = 0.0;

		/* ----------------------------------------------------------------
		*  determine if the vectors are coplanar.
		----------------------------------------------------------------- */
		MathTimeLib::cross(r2, r3, p);
		MathTimeLib::cross(r3, r1, q);
		MathTimeLib::cross(r1, r2, w);
		MathTimeLib::norm(p, pn);
		MathTimeLib::norm(r1, r1n);
		copa = asin(MathTimeLib::dot(pn, r1n));

		if (fabs(MathTimeLib::dot(r1n, pn)) > 0.017452406)
		{
#ifdef _MSC_VER
			strcpy_s(error, sizeof(error), "not coplanar");
#else
			strcpy(error, "not coplanar");
#endif
		}

		/* ---------------- or don't continue processing ---------------- */
		MathTimeLib::addvec3(1.0, p, 1.0, q, 1.0, w, d);
		MathTimeLib::addvec3(magr1, p, magr2, q, MathTimeLib::mag(r3), w, n);
		MathTimeLib::norm(n, nn);
		MathTimeLib::norm(d, dn);

		/* ----------------------------------------------------------------
		*  determine if the orbit is possible.  both d and n must be in
		*    the same direction, and non-zero.
		----------------------------------------------------------------- */
		if ((fabs(MathTimeLib::mag(d)) < small) || (fabs(MathTimeLib::mag(n)) < small) ||
			(fabs(MathTimeLib::dot(nn, dn)) < small))
		{
#ifdef _MSC_VER
			strcpy_s(error, sizeof(error), "impossible");
#else
			strcpy(error, "impossible");
#endif
		}
		else
		{
			theta = MathTimeLib::angle(r1, r2);
			theta1 = MathTimeLib::angle(r2, r3);

			/* ------------ perform gibbs method to find v2 ----------- */
			r1mr2 = magr1 - magr2;
			r3mr1 = MathTimeLib::mag(r3) - magr1;
			r2mr3 = magr2 - MathTimeLib::mag(r3);
			MathTimeLib::addvec3(r1mr2, r3, r3mr1, r2, r2mr3, r1, s);
			MathTimeLib::cross(d, r2, b);
			l = sqrt(mu / (MathTimeLib::mag(d) * MathTimeLib::mag(n)));
			tover2 = l / magr2;
			MathTimeLib::addvec(tover2, b, l, s, v2);
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
	*  author        : david vallado                  719-573-2600   22 jun 2002
	*
	*  inputs          description                             range / units
	*    r1          -  position vector #1                     km
	*    r2          -  position vector #2                     km
	*    r3          -  position vector #3                     km
	*    jd1         - julian date of 1st sighting                days from 4713 bc
	*    jd2         - julian date of 2nd sighting                days from 4713 bc
	*    jd3         - julian date of 3rd sighting                days from 4713 bc
	*
	*  outputs       :
	*    v2          -  velocity vector for r2                 km / s
	*    theta       - angle between vectors                      rad
	*    error       - flag indicating success                    'ok',...
	*
	*  locals        :
	*    dt21        - time delta between r1 and r2               s
	*    dt31        - time delta between r3 and r1               s
	*    dt32        - time delta between r3 and r2               s
	*    p           - p vector    r2 x r3
	*    pn          - p unit vector
	*    r1n         - r1 unit vector
	*    theta1      - temporary angle between vec                rad
	*    tolangle    - tolerance angle  (1 deg)                   rad
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
	*    lncom3      - combination of three scalars and three vectors
	*    angle       - angle between two vectors
	*
	*  references    :
	*    vallado       2013, 466, alg 55, ex 7-4
	-----------------------------------------------------------------------------*/

	void herrgibbs
	(
		double r1[3], double r2[3], double r3[3], double jd1, double jd2, double jd3,
		double v2[3], double& theta, double& theta1, double& copa, char error[14]
	)
	{
		double p[3], pn[3], r1n[3];
		double   dt21, dt31, dt32, term1, term2, term3, tolangle, magr1, magr2;

		/* --------------------  initialize values   -------------------- */
		tolangle = 0.017452406;  // (1.0 deg in rad)

		magr1 = MathTimeLib::mag(r1);
		magr2 = MathTimeLib::mag(r2);

#ifdef _MSC_VER
		strcpy_s(error, sizeof(error), "ok");
#else
		strcpy(error, "ok");
#endif

		theta = 0.0;
		theta1 = 0.0;

		dt21 = (jd2 - jd1) * 86400.0;
		dt31 = (jd3 - jd1) * 86400.0;   // differences in times
		dt32 = (jd3 - jd2) * 86400.0;

		/* ----------------------------------------------------------------
		*  determine if the vectors are coplanar.
		---------------------------------------------------------------- */
		MathTimeLib::cross(r2, r3, p);
		MathTimeLib::norm(p, pn);
		MathTimeLib::norm(r1, r1n);
		copa = asin(MathTimeLib::dot(pn, r1n));
		if (fabs(MathTimeLib::dot(pn, r1n)) > tolangle)
		{
#ifdef _MSC_VER
			strcpy_s(error, sizeof(error), "not coplanar");
#else
			strcpy(error, "not coplanar");
#endif
		}

		/* ----------------------------------------------------------------
		* check the size of the angles between the three position vectors.
		*   herrick gibbs only gives "reasonable" answers when the
		*   position vectors are reasonably close.  1.0 deg is only an estimate.
		---------------------------------------------------------------- */
		theta = MathTimeLib::angle(r1, r2);
		theta1 = MathTimeLib::angle(r2, r3);
		if ((theta > tolangle) || (theta1 > tolangle))
		{
#ifdef _MSC_VER
			strcpy_s(error, sizeof(error), "angle > 1�");
#else
			strcpy(error, "angle > 1�");
#endif
		}

		/* ------------ perform herrick-gibbs method to find v2 --------- */
		term1 = -dt32 *
			(1.0 / (dt21 * dt31) + mu / (12.0 * magr1 * magr1 * magr1));
		term2 = (dt32 - dt21) *
			(1.0 / (dt21 * dt32) + mu / (12.0 * magr2 * magr2 * magr2));
		term3 = dt21 *
			(1.0 / (dt32 * dt31) + mu / (12.0 * MathTimeLib::mag(r3) * MathTimeLib::mag(r3) * MathTimeLib::mag(r3)));
		MathTimeLib::addvec3(term1, r1, term2, r2, term3, r3, v2);
	}  // herrgibbs





	/*------------------------------------------------------------------------------
	*
	*                           procedure target
	*
	*  this procedure accomplishes the targeting problem using kepler/pkepler and
	*    lambert.
	*
	*  author        : david vallado                  719-573-2600   22 jun 2002
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
	*    lncom2      - linear combination of two vectors and constants
	*
	*  references    :
	*    vallado       2013, 503, alg 61
	-----------------------------------------------------------------------------*/

	void target
	(
		double rint[3], double vint[3], double rtgt[3], double vtgt[3],
		char dm, char de, char kind, double dtsec,
		double v1t[3], double v2t[3], double dv1[3], double dv2[3], int error
	)
	{
		double  r1tgt[3], v1tgt[3], altpad;
		double tbi[5][5];
		int nrev;
		char hitearth;
		FILE *outfile;

		altpad = 100.0;  // km
		nrev = 0;

		int err = fopen_s(&outfile, "D:/Codes/LIBRARY/CPP/Libraries/ast2Body/test2body.out", "w");

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
		if (error == 0)
		{
			lambertuniv(rint, r1tgt, dm, de, nrev, dtsec, tbi, altpad, v1t, v2t, hitearth, error, outfile);

			if (error == 0)
			{
				MathTimeLib::addvec(1.0, v1t, -1.0, vint, dv1);
				MathTimeLib::addvec(1.0, v1tgt, -1.0, v2t, dv2);
			}
			else
			{
			}
		}
	}  // target



	// -----------------------------------------------------------------------------------------
	//                                       covariance functions
	// -----------------------------------------------------------------------------------------



	// -----------------------------------------------------------------------------------------
	//                                       perturbed functions
	// -----------------------------------------------------------------------------------------




} // namespace
