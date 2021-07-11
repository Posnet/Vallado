/* ---------------------------------------------------------------------
*
*                              EopSpw.cpp
*
*  this file contains routines to read the eop and space weather data
*  from the cssi files on celestrak. it also includes routines to 
*  interpolate and convert between the values.
*
*                          companion code for
*             fundamentals of astrodynamics and applications
*                                  2013
*                            by david vallado
*
*     (w) 719-573-2600, email dvallado@agi.com
*     *****************************************************************
*    current :
*              30 sep 15  david vallado
*                           fix jdutc, jdfrac
*    changes :
*               3 nov 14  david vallado
*                           update to msvs2013 c++
*               8 aug 14 alek lidtke
*                           started using fopenf_s in line 71 in line 83 with MSVS compiler, disabled SDL checks to supress other errors
*              14 dec 05  david vallado
*                           misc fixes
*              21 oct 05  david vallado
*                           original version
*       ----------------------------------------------------------------      */

#include "EopSpw.h"

namespace EopSpw 
{

	/* -----------------------------------------------------------------------------
	*
	*                           function readspw
	*
	*  this function initializes the space weather data from the cssi files.
	*
	*  author        : david vallado                  719-573-2600   2 nov 2005
	*
	*  revisions
	*
	*  inputs          description                    range / units
	*
	*  outputs       :
	*    spwarr      - array of spw data records
	*    jdspwstart  - julian date of the start of the spwarr data
	*
	*  locals        :
	*                -
	*
	*  coupling      :
	*    jday        - julian date
	*
	*  references    :
	*
	*  -------------------------------------------------------------------------- */

	void readspw
		(
		std::vector<spwdata> &spwarr,
		std::string infilename,
		double& jdspwstart, double& jdspwstartFrac
		)
	{
		FILE *infile;
		char longstr[170];
		char str[9], blk[27];
		long int numrecsobs, numrecspred, i, year, mon, day;
		int filelen = 170;
		double jd, jdFrac;

		spwarr.resize(spwsize);

		// ---- open files
#ifdef _MSC_VER
		infile = fopen(infilename.c_str(), "r");
#else
		infile = fopen(infilename, "r");
#endif
		// find beginning of data
		i = 0;
		do
		{
			fgets(longstr, filelen, infile);
#ifdef _MSC_VER
			strncpy_s(str, &longstr[13], 6);
#else
			strncpy(str, &longstr[13]);
#endif
			str[6] = '\0';
			i = i + 1;
		} while ((strncmp(str, "POINTS", 6) != 0) && (feof(infile) == 0));

		// ---- read number of data points
#ifdef _MSC_VER
		sscanf_s(longstr, "%19c %5i ", blk, 19 * sizeof(char), &numrecsobs);
#else
		sscanf(longstr, "%19c %5i ", blk, &numrecsobs);
#endif

		// ---- find epoch date
		fgets(longstr, filelen, infile);
		fgets(longstr, filelen, infile);
#ifdef _MSC_VER
		sscanf_s(longstr, "%4i %3i %3i ", &year, &mon, &day);
#else
		sscanf(longstr, "%4i %3i %3i ", &year, &mon, &day);
#endif
		MathTimeLib::jday(year, mon, day, 0, 0, 0.0, jdspwstart, jdspwstartFrac);
		jdspwstart = jdspwstart;
	
		i = 0;
		// ---- first record is in the string above
#ifdef _MSC_VER
		sscanf_s(longstr, "%4i %3d %3d %5i %3i %3i %3i %3i %3i %3i %3i %3i %3i %4i %4i %4i %4i %4i %4i %4i %4i %4i %4i %4lf %2i %4i %6lf %2i %7lf %6lf %6lf %6lf %6lf \n",
			&spwarr[i].year, &spwarr[i].mon, &spwarr[i].day, &spwarr[i].bsrn, &spwarr[i].nd,
			&spwarr[i].kparr[0], &spwarr[i].kparr[1], &spwarr[i].kparr[2], &spwarr[i].kparr[3],
			&spwarr[i].kparr[4], &spwarr[i].kparr[5], &spwarr[i].kparr[6], &spwarr[i].kparr[7], &spwarr[i].sumkp,
			&spwarr[i].aparr[0], &spwarr[i].aparr[1], &spwarr[i].aparr[2], &spwarr[i].aparr[3],
			&spwarr[i].aparr[4], &spwarr[i].aparr[5], &spwarr[i].aparr[6], &spwarr[i].aparr[7], &spwarr[i].avgap,
			&spwarr[i].cp, &spwarr[i].c9, &spwarr[i].isn, &spwarr[i].adjf10, &spwarr[i].q,
			&spwarr[i].adjctrf81, &spwarr[i].adjlstf81, &spwarr[i].obsf10, &spwarr[i].obsctrf81, &spwarr[i].obslstf81);
#else
		sscanf(longstr, "%4i %3d %3d %5i %3i %3i %3i %3i %3i %3i %3i %3i %3i %4i %4i %4i %4i %4i %4i %4i %4i %4i %4i %4lf %2i %4i %6lf %2i %7lf %6lf %6lf %6lf %6lf \n",
			&spwarr[i].year, &spwarr[i].mon, &spwarr[i].day, &spwarr[i].bsrn, &spwarr[i].nd,
			&spwarr[i].kparr[0], &spwarr[i].kparr[1], &spwarr[i].kparr[2], &spwarr[i].kparr[3],
			&spwarr[i].kparr[4], &spwarr[i].kparr[5], &spwarr[i].kparr[6], &spwarr[i].kparr[7], &spwarr[i].sumkp,
			&spwarr[i].aparr[0], &spwarr[i].aparr[1], &spwarr[i].aparr[2], &spwarr[i].aparr[3],
			&spwarr[i].aparr[4], &spwarr[i].aparr[5], &spwarr[i].aparr[6], &spwarr[i].aparr[7], &spwarr[i].avgap,
			&spwarr[i].cp, &spwarr[i].c9, &spwarr[i].isn, &spwarr[i].adjf10, &spwarr[i].q,
			&spwarr[i].adjctrf81, &spwarr[i].adjlstf81, &spwarr[i].obsf10, &spwarr[i].obsctrf81, &spwarr[i].obslstf81);
#endif
		MathTimeLib::jday(spwarr[i].year, spwarr[i].mon, spwarr[i].day, 0, 0, 0.0, jd, jdFrac);
		spwarr[i].mjd = int(jd + jdFrac - 2400000.5);

		// ---- process observed records
		for (i = 1; i <= numrecsobs - 1; i++)
		{
			fgets(longstr, filelen, infile);
			// use d format for integers with leading 0's
			// use the scanf here in case extra fields are in spw data
#ifdef _MSC_VER
			sscanf_s(longstr, "%4i %3d %3d %5i %3i %3i %3i %3i %3i %3i %3i %3i %3i %4i %4i %4i %4i %4i %4i %4i %4i %4i %4i %4lf %2i %4i %6lf %2i %7lf %6lf %6lf %6lf %6lf \n",
				&spwarr[i].year, &spwarr[i].mon, &spwarr[i].day, &spwarr[i].bsrn, &spwarr[i].nd,
				&spwarr[i].kparr[0], &spwarr[i].kparr[1], &spwarr[i].kparr[2], &spwarr[i].kparr[3],
				&spwarr[i].kparr[4], &spwarr[i].kparr[5], &spwarr[i].kparr[6], &spwarr[i].kparr[7], &spwarr[i].sumkp,
				&spwarr[i].aparr[0], &spwarr[i].aparr[1], &spwarr[i].aparr[2], &spwarr[i].aparr[3],
				&spwarr[i].aparr[4], &spwarr[i].aparr[5], &spwarr[i].aparr[6], &spwarr[i].aparr[7], &spwarr[i].avgap,
				&spwarr[i].cp, &spwarr[i].c9, &spwarr[i].isn, &spwarr[i].adjf10, &spwarr[i].q,
				&spwarr[i].adjctrf81, &spwarr[i].adjlstf81, &spwarr[i].obsf10, &spwarr[i].obsctrf81, &spwarr[i].obslstf81);
#else
			sscanf(longstr, "%4i %3d %3d %5i %3i %3i %3i %3i %3i %3i %3i %3i %3i %4i %4i %4i %4i %4i %4i %4i %4i %4i %4i %4lf %2i %4i %6lf %2i %7lf %6lf %6lf %6lf %6lf \n",
				&spwarr[i].year, &spwarr[i].mon, &spwarr[i].day, &spwarr[i].bsrn, &spwarr[i].nd,
				&spwarr[i].kparr[0], &spwarr[i].kparr[1], &spwarr[i].kparr[2], &spwarr[i].kparr[3],
				&spwarr[i].kparr[4], &spwarr[i].kparr[5], &spwarr[i].kparr[6], &spwarr[i].kparr[7], &spwarr[i].sumkp,
				&spwarr[i].aparr[0], &spwarr[i].aparr[1], &spwarr[i].aparr[2], &spwarr[i].aparr[3],
				&spwarr[i].aparr[4], &spwarr[i].aparr[5], &spwarr[i].aparr[6], &spwarr[i].aparr[7], &spwarr[i].avgap,
				&spwarr[i].cp, &spwarr[i].c9, &spwarr[i].isn, &spwarr[i].adjf10, &spwarr[i].q,
				&spwarr[i].adjctrf81, &spwarr[i].adjlstf81, &spwarr[i].obsf10, &spwarr[i].obsctrf81, &spwarr[i].obslstf81);
#endif
			MathTimeLib::jday(spwarr[i].year, spwarr[i].mon, spwarr[i].day, 0, 0, 0.0, jd, jdFrac);
			spwarr[i].mjd = int(jd + jdFrac - 2400000.5);
		}

		fgets(longstr, filelen, infile);
		fgets(longstr, filelen, infile);
		fgets(longstr, filelen, infile);
#ifdef _MSC_VER
		sscanf_s(longstr, "%26c %5i ", blk, 26 * sizeof(char), &numrecspred);
#else
		sscanf(longstr, "%26c %5i ", blk, &numrecspred);
#endif
		fgets(longstr, filelen, infile);
//		fgets(longstr, filelen, infile);

		// ---- process predicted records
		for (i = numrecsobs; i <= numrecsobs + numrecspred-1; i++)
		{
			fgets(longstr, filelen, infile);
			// use d format for integers with leading 0's
#ifdef _MSC_VER
			sscanf_s(longstr, "%4i %3d %3d %5i %3i %3i %3i %3i %3i %3i %3i %3i %3i %4i %4i %4i %4i %4i %4i %4i %4i %4i %4i %4lf %2i %4i %6lf %2i %7lf %6lf %6lf %6lf %6lf \n",
				&spwarr[i].year, &spwarr[i].mon, &spwarr[i].day, &spwarr[i].bsrn, &spwarr[i].nd,
				&spwarr[i].kparr[0], &spwarr[i].kparr[1], &spwarr[i].kparr[2], &spwarr[i].kparr[3],
				&spwarr[i].kparr[4], &spwarr[i].kparr[5], &spwarr[i].kparr[6], &spwarr[i].kparr[7], &spwarr[i].sumkp,
				&spwarr[i].aparr[0], &spwarr[i].aparr[1], &spwarr[i].aparr[2], &spwarr[i].aparr[3],
				&spwarr[i].aparr[4], &spwarr[i].aparr[5], &spwarr[i].aparr[6], &spwarr[i].aparr[7], &spwarr[i].avgap,
				&spwarr[i].cp, &spwarr[i].c9, &spwarr[i].isn, &spwarr[i].adjf10, &spwarr[i].q,
				&spwarr[i].adjctrf81, &spwarr[i].adjlstf81, &spwarr[i].obsf10, &spwarr[i].obsctrf81, &spwarr[i].obslstf81);
#else
			sscanf(longstr, "%4i %3d %3d %5i %3i %3i %3i %3i %3i %3i %3i %3i %3i %4i %4i %4i %4i %4i %4i %4i %4i %4i %4i %4lf %2i %4i %6lf %2i %7lf %6lf %6lf %6lf %6lf \n",
				&spwarr[i].year, &spwarr[i].mon, &spwarr[i].day, &spwarr[i].bsrn, &spwarr[i].nd,
				&spwarr[i].kparr[0], &spwarr[i].kparr[1], &spwarr[i].kparr[2], &spwarr[i].kparr[3],
				&spwarr[i].kparr[4], &spwarr[i].kparr[5], &spwarr[i].kparr[6], &spwarr[i].kparr[7], &spwarr[i].sumkp,
				&spwarr[i].aparr[0], &spwarr[i].aparr[1], &spwarr[i].aparr[2], &spwarr[i].aparr[3],
				&spwarr[i].aparr[4], &spwarr[i].aparr[5], &spwarr[i].aparr[6], &spwarr[i].aparr[7], &spwarr[i].avgap,
				&spwarr[i].cp, &spwarr[i].c9, &spwarr[i].isn, &spwarr[i].adjf10, &spwarr[i].q,
				&spwarr[i].adjctrf81, &spwarr[i].adjlstf81, &spwarr[i].obsf10, &spwarr[i].obsctrf81, &spwarr[i].obslstf81);
#endif
			MathTimeLib::jday(spwarr[i].year, spwarr[i].mon, spwarr[i].day, 0, 0, 0.0, jd, jdFrac);
			spwarr[i].mjd = int(jd + jdFrac - 2400000.5);
		}
		fclose(infile);
	}  // readspw


	/* -----------------------------------------------------------------------------
	*
	*                           function readeop
	*
	*  this function initializes the earth orientation parameter data. the input
	*  data files are from celestrak and the eoppn.txt file contains the nutation
	*  daily values used for optimizing the speed of operation. note these nutation
	*  values do not have corrections applied (dx/dy/ddpsi/ddeps) so a single
	*  routine can process past and future data with these corrections. the
	*  corrections are not known in the future. the files could be combined, but it
	*  may make more sense to keep them separate because the values can be calculated
	*  long into the future.
	*
	*  author        : david vallado                  719-573-2600   2 nov 2005
	*
	*  revisions
	*
	*  inputs          description                    range / units
	*
	*  outputs       :
	*    eoparr      - array of eop data records
	*    jdeopstart  - julian date of the start of the eoparr data
	*
	*  locals        :
	*    convrt      - conversion factor arcsec to radians
	*                -
	*
	*  coupling      :
	*
	*  references    :
	*
	*  -------------------------------------------------------------------------- */

	void readeop
		(
		std::vector<eopdata> &eoparr,  // pass by ref
    	std::string infilename,
		double& jdeopstart, double& jdeopstartFrac
		)
	{
		FILE *infile; // , *infile1;
		char longstr[140];
		char str[9], blk[20];
		int numrecsobs, numrecspred, year, mon, day;
		double convrt;
		long i;

		// arcsec to rad
		convrt = pi / (180.0 * 3600.0);
		
		eoparr.resize(eopsize);

		// ---- open files select compatible files!!
#ifdef _MSC_VER
		infile = fopen(infilename.c_str(), "r");
#else
		infile = fopen(infilename, "r");
#endif		// comment these, then run testcoord option 7 to create file. then
		// uncomment these to read the two files and put in the record.
		//       infile1 = fopen( "eoppn1.txt", "r");
		//       infile1 = fopen( "eoppn00arc.txt", "r");
		//       infile1 = fopen( "eoppn00rad.txt", "r");
		//       infile1 = fopen( "eoppn62arc.txt", "r");
		//       infile1 = fopen( "eoppn62rad.txt", "r");

		// ---- find beginning of data
		i = 0;
		do
		{
			fgets(longstr, 140, infile);
#ifdef _MSC_VER
			strncpy_s(str, &longstr[13], 6);
#else
			strncpy(str, &longstr[13]);
#endif
			str[6] = '\0';
			i = i + 1;
		} while ((strncmp(str, "POINTS", 6) != 0) && (feof(infile) == 0));

		// ---- read number of data points
#ifdef _MSC_VER
		sscanf_s(longstr, "%19c %5i ", blk, 19 * sizeof(char), &numrecsobs);
#else
		sscanf(longstr, "%19c %5i ", blk, &numrecsobs);
#endif

		// ---- find epoch date
		fgets(longstr, 140, infile);
		fgets(longstr, 140, infile);
#ifdef _MSC_VER
		sscanf_s(longstr, "%4i %3i %3i ", &year, &mon, &day);
#else
		sscanf(longstr, "%4i %3i %3i ", &year, &mon, &day);
#endif
		MathTimeLib::jday(year, mon, day, 0, 0, 0.0, jdeopstart, jdeopstartFrac);

		i = 0;
		// ---- first record is in the string above
#ifdef _MSC_VER
		sscanf_s(longstr, "%4i %3d %3d %6i %10lf %10lf %11lf %11lf %10lf %10lf %10lf %10lf %4i ",
			&eoparr[i].year, &eoparr[i].mon, &eoparr[i].day, &eoparr[i].mjd,
			&eoparr[i].xp, &eoparr[i].yp, &eoparr[i].dut1, &eoparr[i].lod,
			&eoparr[i].ddpsi, &eoparr[i].ddeps, &eoparr[i].dx, &eoparr[i].dy, &eoparr[i].dat);
#else
		sscanf(longstr, "%4i %3d %3d %6i %10lf %10lf %11lf %11lf %10lf %10lf %10lf %10lf %4i ",
			&eoparr[i].year, &eoparr[i].mon, &eoparr[i].day, &eoparr[i].mjd,
			&eoparr[i].xp, &eoparr[i].yp, &eoparr[i].dut1, &eoparr[i].lod,
			&eoparr[i].ddpsi, &eoparr[i].ddeps, &eoparr[i].dx, &eoparr[i].dy, &eoparr[i].dat);
#endif
		// uncomment these to read the xys parameters also
		//       fgets( longstr,140,infile1);
		//       sscanf_s(longstr," %lf  %lf  %lf  %lf %lf  %lf \n", &eoparr[i].x, &eoparr[i].y,
		//                  &eoparr[i].s, &eoparr[i].deltapsi, &eoparr[i].deltaeps, &mjd);

		// --------------- fix units ---------
		eoparr[i].xp = eoparr[i].xp * convrt;
		eoparr[i].yp = eoparr[i].yp * convrt;
		eoparr[i].ddpsi = eoparr[i].ddpsi * convrt;
		eoparr[i].ddeps = eoparr[i].ddeps * convrt;
		eoparr[i].dx = eoparr[i].dx * convrt;
		eoparr[i].dy = eoparr[i].dy * convrt;

		// ---- process observed records
		for (i = 1; i <= numrecsobs - 1; i++)
		{
			// use d format for integers with leading 0's
#ifdef _MSC_VER
			fscanf_s(infile, "%4i %3d %3d %6i %10lf %10lf %11lf %11lf %10lf %10lf %10lf %10lf %4i \n",
				&eoparr[i].year, &eoparr[i].mon, &eoparr[i].day, &eoparr[i].mjd,
				&eoparr[i].xp, &eoparr[i].yp, &eoparr[i].dut1, &eoparr[i].lod,
				&eoparr[i].ddpsi, &eoparr[i].ddeps, &eoparr[i].dx, &eoparr[i].dy, &eoparr[i].dat);
#else
			fscanf(infile, "%4i %3d %3d %6i %10lf %10lf %11lf %11lf %10lf %10lf %10lf %10lf %4i \n",
				&eoparr[i].year, &eoparr[i].mon, &eoparr[i].day, &eoparr[i].mjd,
				&eoparr[i].xp, &eoparr[i].yp, &eoparr[i].dut1, &eoparr[i].lod,
				&eoparr[i].ddpsi, &eoparr[i].ddeps, &eoparr[i].dx, &eoparr[i].dy, &eoparr[i].dat);
#endif

			// uncomment these to read the xys parameters also
			//           fscanf_s(infile1," %21lf  %20lf  %20lf  %16lf %16lf  %11lf \n", &eoparr[i].x, &eoparr[i].y,
			//                 &eoparr[i].s, &eoparr[i].deltapsi, &eoparr[i].deltaeps, &mjd );

			// --------------- fix units ---------
			eoparr[i].xp = eoparr[i].xp * convrt;
			eoparr[i].yp = eoparr[i].yp * convrt;
			eoparr[i].ddpsi = eoparr[i].ddpsi * convrt;
			eoparr[i].ddeps = eoparr[i].ddeps * convrt;
			eoparr[i].dx = eoparr[i].dx * convrt;
			eoparr[i].dy = eoparr[i].dy * convrt;
		}

		fgets(longstr, 140, infile);
		fgets(longstr, 140, infile);
		fgets(longstr, 140, infile);
#ifdef _MSC_VER
		sscanf_s(longstr, "%20c %5i ", blk, 20 * sizeof(char), &numrecspred);
#else
		sscanf(longstr, "%20c %5i ", blk, &numrecspred);
#endif
		fgets(longstr, 140, infile);

		// ---- process predicted records
		for (i = numrecsobs; i <= numrecsobs + numrecspred; i++)
		{
			// use d format for integers with leading 0's
#ifdef _MSC_VER
			fscanf_s(infile, "%4i %3d %3d %6i %10lf %10lf %11lf %11lf %10lf %10lf %10lf %10lf %4i \n",
				&eoparr[i].year, &eoparr[i].mon, &eoparr[i].day, &eoparr[i].mjd,
				&eoparr[i].xp, &eoparr[i].yp, &eoparr[i].dut1, &eoparr[i].lod,
				&eoparr[i].ddpsi, &eoparr[i].ddeps, &eoparr[i].dx, &eoparr[i].dy, &eoparr[i].dat);
#else
			fscanf(infile, "%4i %3d %3d %6i %10lf %10lf %11lf %11lf %10lf %10lf %10lf %10lf %4i \n",
				&eoparr[i].year, &eoparr[i].mon, &eoparr[i].day, &eoparr[i].mjd,
				&eoparr[i].xp, &eoparr[i].yp, &eoparr[i].dut1, &eoparr[i].lod,
				&eoparr[i].ddpsi, &eoparr[i].ddeps, &eoparr[i].dx, &eoparr[i].dy, &eoparr[i].dat);
#endif
			// uncomment these to read the xys parameters also
			//           fscanf_s(infile1," %21lf  %20lf  %20lf   %16lf %16lf  %11lf \n", &eoparr[i].x, &eoparr[i].y,
			//                  &eoparr[i].s, &eoparr[i].deltapsi, &eoparr[i].deltaeps, &mjd );

			// --------------- fix units ---------
			eoparr[i].xp = eoparr[i].xp * convrt;
			eoparr[i].yp = eoparr[i].yp * convrt;
			eoparr[i].ddpsi = eoparr[i].ddpsi * convrt;
			eoparr[i].ddeps = eoparr[i].ddeps * convrt;
			eoparr[i].dx = eoparr[i].dx * convrt;
			eoparr[i].dy = eoparr[i].dy * convrt;
		}
		fclose(infile);
	}   //  readeop


	/* -----------------------------------------------------------------------------
	*
	*                           function findeopparam
	*
	*  this routine finds the eop parameters for a given time. several types of
	*  interpolation are available. the cio and iau76 nutation parameters are also
	*  read for optimizing the speeed of calculations.
	*
	*  author        : david vallado                      719-573-2600   12 dec 2005
	*
	*  inputs          description                          range / units
	*    jdutc          - julian date of epoch (0 hrs utc)
	*    jdutcF         - fraction of a day from jdutc
	*    interp      - interpolation                        n-none, l-linear, s-spline
	*    whichm      - which method fk5 or iau2010, or both 'f', 'i', 'b'
	*    eoparr      - array of eop data records
	*    jdeopstart  - julian date of the start of the eoparr data (set in initeop)
	*
	*  outputs       :
	*    dut1        - delta ut1 (ut1-utc)                  sec
	*    dat         - number of leap seconds               sec
	*    lod         - excess length of day                 sec
	*    xp          - x component of polar motion          rad
	*    yp          - y component of polar motion          rad
	*    ddpsi       - correction to delta psi (iau-76 theory) rad
	*    ddeps       - correction to delta eps (iau-76 theory) rad
	*    x           - x component of cio                   rad
	*    y           - y component of cio                   rad
	*    s           -                                      rad
	*    dx          - correction to x (cio theory)         rad
	*    dy          - correction to y (cio theory)         rad
	*
	*  locals        :
	*                -
	*
	*  coupling      :
	*    none        -
	*
	*  references    :
	*    vallado       2004,
	* --------------------------------------------------------------------------- */

	void findeopparam
		(
		double  jdutc, double jdutcF, char interp, 
		const std::vector<eopdata> eoparr,
		double jdeopstart,
		double& dut1, int& dat,
		double& lod, double& xp, double& yp,
		double& ddpsi, double& ddeps, double& dx, double& dy,
		double& x, double& y, double& s
		)
	{
		long recnum;
		int  off1, off2;
		eopdata eoprec, nexteoprec;
		double  jd1, fixf, jdeopstarto, mfme;
		double convrt = pi / (3600.0 * 180.0);  // " to rad

		// set default values
		dut1 = 0.0;
		dat = 33;
		lod = 0.0;
		xp = 0.0;
		yp = 0.0;
		ddpsi = 0.0;
		ddeps = 0.0;
		x = 0.0;
		y = 0.0;
		s = 0.0;
		dx = 0.0;
		dy = 0.0;

		// check if any whole days in jdF
		jd1 = floor(jdutc + jdutcF) + 0.5;  // want jd at 0 hr
		mfme = (jdutc + jdutcF - jd1) * 1440.0;
		if (mfme < 0.0)
			mfme = 1440.0 + mfme;

		// ---- read data for day of interest
		jdutc = jdutc + jdutcF;
		jdeopstarto = floor(jdutc - jdeopstart);
		recnum = int(jdeopstarto);

		// check for out of bound values
		if ((recnum >= 1) && (recnum <= eopsize))
		{
			eoprec = eoparr[recnum];

			// ---- set non-interpolated values
			dut1 = eoprec.dut1;
			dat = eoprec.dat;
			lod = eoprec.lod;
			xp = eoprec.xp;
			yp = eoprec.yp;
			ddpsi = eoprec.ddpsi;
			ddeps = eoprec.ddeps;
			x = eoprec.x;
			y = eoprec.y;
			s = eoprec.s;
			dx = eoprec.dx;
			dy = eoprec.dy;

			// ---- do linear interpolation
			if (interp == 'l')
			{
				nexteoprec = eoparr[recnum + 1];
				fixf = mfme / 1440.0;

				dut1 = eoprec.dut1 + (nexteoprec.dut1 - eoprec.dut1) * fixf;
				dat = eoprec.dat + int((nexteoprec.dat - eoprec.dat) * fixf);
				lod = eoprec.lod + (nexteoprec.lod - eoprec.lod) * fixf;
				xp = eoprec.xp + (nexteoprec.xp - eoprec.xp) * fixf;
				yp = eoprec.yp + (nexteoprec.yp - eoprec.yp) * fixf;
				ddpsi = eoprec.ddpsi + (nexteoprec.ddpsi - eoprec.ddpsi) * fixf;
				ddeps = eoprec.ddeps + (nexteoprec.ddeps - eoprec.ddeps) * fixf;
				x = eoprec.x + (nexteoprec.x - eoprec.x) * fixf;
				y = eoprec.y + (nexteoprec.y - eoprec.y) * fixf;
				s = eoprec.s + (nexteoprec.s - eoprec.s) * fixf;
				dx = eoprec.dx + (nexteoprec.dx - eoprec.dx) * fixf;
				dy = eoprec.dy + (nexteoprec.dy - eoprec.dy) * fixf;
			}

			// ---- do spline interpolations
			if (interp == 's')
			{
				fixf = mfme / 1440.0;
				off1 = 1;   // every 5 days data...
				off2 = 2;
				dut1 = MathTimeLib::cubicinterp(eoparr[recnum - off1].dut1, eoparr[recnum].dut1, eoparr[recnum + off1].dut1, eoparr[recnum + off2].dut1,
					eoparr[recnum - off1].mjd, eoparr[recnum].mjd, eoparr[recnum + off1].mjd, eoparr[recnum + off2].mjd,
					eoparr[recnum].mjd + jdutcF);
				dat = int(MathTimeLib::cubicinterp(eoparr[recnum - off1].dat, eoparr[recnum].dat, eoparr[recnum + off1].dat, eoparr[recnum + off2].dat,
					eoparr[recnum - off1].mjd, eoparr[recnum].mjd, eoparr[recnum + off1].mjd, eoparr[recnum + off2].mjd,
					eoparr[recnum].mjd + jdutcF));
				lod = MathTimeLib::cubicinterp(eoparr[recnum - off1].lod, eoparr[recnum].lod, eoparr[recnum + off1].lod, eoparr[recnum + off2].lod,
					eoparr[recnum - off1].mjd, eoparr[recnum].mjd, eoparr[recnum + off1].mjd, eoparr[recnum + off2].mjd,
					eoparr[recnum].mjd + jdutcF);
				xp = MathTimeLib::cubicinterp(eoparr[recnum - off1].xp, eoparr[recnum].xp, eoparr[recnum + off1].xp, eoparr[recnum + off2].xp,
					eoparr[recnum - off1].mjd, eoparr[recnum].mjd, eoparr[recnum + off1].mjd, eoparr[recnum + off2].mjd,
					eoparr[recnum].mjd + jdutcF);
				yp = MathTimeLib::cubicinterp(eoparr[recnum - off1].yp, eoparr[recnum].yp, eoparr[recnum + off1].yp, eoparr[recnum + off2].yp,
					eoparr[recnum - off1].mjd, eoparr[recnum].mjd, eoparr[recnum + off1].mjd, eoparr[recnum + off2].mjd,
					eoparr[recnum].mjd + jdutcF);
				ddpsi = MathTimeLib::cubicinterp(eoparr[recnum - off1].ddpsi, eoparr[recnum].ddpsi, eoparr[recnum + off1].ddpsi, eoparr[recnum + off2].ddpsi,
					eoparr[recnum - off1].mjd, eoparr[recnum].mjd, eoparr[recnum + off1].mjd, eoparr[recnum + off2].mjd,
					eoparr[recnum].mjd + jdutcF);
				ddeps = MathTimeLib::cubicinterp(eoparr[recnum - off1].ddeps, eoparr[recnum].ddeps, eoparr[recnum + off1].ddeps, eoparr[recnum + off2].ddeps,
					eoparr[recnum - off1].mjd, eoparr[recnum].mjd, eoparr[recnum + off1].mjd, eoparr[recnum + off2].mjd,
					eoparr[recnum].mjd + jdutcF);
				x = MathTimeLib::cubicinterp(eoparr[recnum - off1].x, eoparr[recnum].x, eoparr[recnum + off1].x, eoparr[recnum + off2].x,
					eoparr[recnum - off1].mjd, eoparr[recnum].mjd, eoparr[recnum + off1].mjd, eoparr[recnum + off2].mjd,
					eoparr[recnum].mjd + jdutcF);
				y = MathTimeLib::cubicinterp(eoparr[recnum - off1].y, eoparr[recnum].y, eoparr[recnum + off1].y, eoparr[recnum + off2].y,
					eoparr[recnum - off1].mjd, eoparr[recnum].mjd, eoparr[recnum + off1].mjd, eoparr[recnum + off2].mjd,
					eoparr[recnum].mjd + jdutcF);
				s = MathTimeLib::cubicinterp(eoparr[recnum - off1].s, eoparr[recnum].s, eoparr[recnum + off1].s, eoparr[recnum + off2].s,
					eoparr[recnum - off1].mjd, eoparr[recnum].mjd, eoparr[recnum + off1].mjd, eoparr[recnum + off2].mjd,
					eoparr[recnum].mjd + jdutcF);
				dx = MathTimeLib::cubicinterp(eoparr[recnum - off1].dx, eoparr[recnum].dx, eoparr[recnum + off1].dx, eoparr[recnum + off2].dx,
					eoparr[recnum - off1].mjd, eoparr[recnum].mjd, eoparr[recnum + off1].mjd, eoparr[recnum + off2].mjd,
					eoparr[recnum].mjd + jdutcF);
				dy = MathTimeLib::cubicinterp(eoparr[recnum - off1].dy, eoparr[recnum].dy, eoparr[recnum + off1].dy, eoparr[recnum + off2].dy,
					eoparr[recnum - off1].mjd, eoparr[recnum].mjd, eoparr[recnum + off1].mjd, eoparr[recnum + off2].mjd,
					eoparr[recnum].mjd + jdutcF);
			}
		}

		// now convert units for use in operations already done
		//xp = xp * convrt;  // " to rad
		//yp = yp * convrt;
		//ddpsi = ddpsi * convrt;  // " to rad
		//ddeps = ddeps * convrt;
		//dx = dx * convrt;  // " to rad
		//dy = dy * convrt;
	}  //  findeopparam



		/* -----------------------------------------------------------------------------
		*
		*                           function findspwparam
		*
		*  this routine finds the atmospheric parameters for a given time.
		*    ap/kp 3 hourly data is valid at 0000, 0300 hrs, etc
		*    apavg and kpsum are valid at 1200 hrs
		*    f107 and f107bar values are valid at 1700/2000 hrs depending on the date
		*    ap arrays go 0-7, but msisarr goes 1-8 to match msis code and fortran source
		*
		*  author        : david vallado                      719-573-2600    2 dec 2005
		*
		*  inputs          description                                     range / units
		*    jd          - julian date of epoch (0 hrs utc)              days from 4713 bc
		*    mfme        - minutes from midnight epoch                       mins
		*    interp      - interpolation                                 l - linear, s - spline
		*    fluxtype    - flux type                                     a-adjusted, o-observed
		*    f81type     - flux 81-day avg type                          l-last, c-centered
		*    inputtype   - input type                                    a-actual, c - constant
		*    spwarr      - array of space weather data
		*    jdspwstart  - julian date of the start of the spwarr data (set in readspw)
		*
		*  outputs       :
		*    f107        - f10.7 value (current day)
		*    f107bar     - f10.7 81-day avg value
		*    ap          - planetary effect array
		*    avgap       - daily average ap planetary value
		*    aparr       - last 8 values of 3-hourly ap
		*    kp          - planetary effect array
		*    sumkp       - daily kp sum of planetary values
		*    kparr       - last 8 values of 3-hourly kp
		*
		*  locals        :
		*    fluxtime    - minutes from midnight where f107 is valid (1020 or 1200)
		*
		*  coupling      :
		*    none        -
		*  -------------------------------------------------------------------------- */

	void findspwparam
		(
		double jdutc, double jdutcF, char interp, char fluxtype, char f81type, char inputtype,
		const std::vector<spwdata> spwarr,  // pass by ref, not modify
		double jdspwstart,
		double& f107, double& f107bar,
		double& ap, double& avgap, double aparr[8],
		double& kp, double& sumkp, double kparr[8]
		)
	{
		int     recnum, idx, i, j, off1, off2;
		double  jd1, mfme, jdspwstarto, fluxtime, fixf, tf107, tf107bar;
		spwdata spwrec, lastspwrec, nextspwrec, tempspwrec;

		// --------------------  implementation   ----------------------
		// check if any whole days in jdF
		jd1 = floor(jdutc + jdutcF) + 0.5;  // want jd at 0 hr
		mfme = (jdutc + jdutcF - jd1) * 1440.0;
		if (mfme < 0.0)
			mfme = 1440.0 + mfme;

		// ---- set flux time based on when measurments were taken
		// ---- before may 31, 1991, use 1700 hrs (1020 minutes)
		if (jdutc > 2448407.5)
			fluxtime = 1200.0;  // min
		else
			fluxtime = 1020.0;

		// ---- read data for day of interest
		jdspwstarto = floor(jdutc + jdutcF - jdspwstart);
		recnum = int(jdspwstarto);

		// check for out of bound values
		if ((recnum >= 1) && (recnum <= eopsize))
		{
			spwrec = spwarr[recnum];
			lastspwrec = spwarr[recnum - 1];
			nextspwrec = spwarr[recnum + 1];

			// ---------------------- set non-interpolated values -----------------------------
			if (fluxtype == 'a')
			{
				f107 = spwrec.adjf10;
				if (f81type == 'l')
					f107bar = spwrec.adjlstf81;
				else
					f107bar = spwrec.adjctrf81;
			}
			else  //  observed
			{
				f107 = spwrec.obsf10;
				if (f81type == 'l')
					f107bar = spwrec.obslstf81;
				else
					f107bar = spwrec.obsctrf81;
			}
			avgap = spwrec.avgap;
			sumkp = spwrec.sumkp;

			// ---- get last ap/kp array value from the current time value
			idx = int(jdutcF * 1440.0 / 180.0); // values change at 0, 3, 6, ... hrs
			if (idx < 0) idx = 0;
			if (idx > 7) idx = 7;

			j = idx;
			for (i = 1; i <= 8; i++)
			{
				if (j >= 0)
				{
					aparr[8 - i] = spwrec.aparr[j];
					kparr[8 - i] = spwrec.kparr[j];
				}
				else
				{
					aparr[8 - i] = lastspwrec.aparr[8 + j];
					kparr[8 - i] = lastspwrec.kparr[8 + j];
				}
				j = j - 1;
			}
			ap = spwrec.aparr[idx];
			kp = spwrec.kparr[idx] * 0.1;

			// ------------------------ do interpolations ------------------------
				if (interp == 'l')
				{
					if (jdutcF * 1440.0 > fluxtime - 720.0) // go 12 hrs before...
					{
						if (jdutcF * 1440.0 > fluxtime)
							tempspwrec = nextspwrec;
						else
							tempspwrec = lastspwrec;
						fixf = (fluxtime - jdutcF * 1440.0) / 1440.0;
					}
					else
					{
						tempspwrec = lastspwrec;
						fixf = (jdutcF * 1440.0 + (1440 - fluxtime)) / 1440.0;
					}
					if (fluxtype == 'a') // adjusted or observed values
					{
						tf107 = tempspwrec.adjf10;
						if (f81type == 'l')
							tf107bar = tempspwrec.adjlstf81;
						else
							tf107bar = tempspwrec.adjctrf81;
					}
					else
					{
						tf107 = tempspwrec.obsf10;
						if (f81type == 'l')
							tf107bar = tempspwrec.obslstf81;
						else
							tf107bar = tempspwrec.obsctrf81;
					}
					// ---- perform simple linear interpolation
					if (jdutcF * 1440.0 <= fluxtime)
					{
						if (jdutcF * 1440.0 > fluxtime - 720.0)
						{
							f107 = f107 + (tf107 - f107) * fixf;
							f107bar = f107bar + (tf107bar - f107bar) * fixf;
						}
						else
						{
							f107 = tf107 + (f107 - tf107) * fixf;
							f107bar = tf107bar + (f107bar - tf107bar) * fixf;
						}
					}
					else
					{
						f107 = f107 + (tf107 - f107) * fixf;
						f107bar = f107bar + (tf107bar - f107bar) * fixf;
					}

					// ---- do spline interpolations
					if (interp == 's')
					{
						off1 = 1;
						off2 = 2;
						fixf = mfme / 1440.0;  // days for mjd
						// setup so the interval is in between points 2 and 3
						if (fluxtype == 'a') // adjusted 
						{
							f107 = MathTimeLib::cubicinterp(
								lastspwrec.adjf10, spwrec.adjf10, nextspwrec.adjf10, spwarr[recnum + off2].adjf10,
								lastspwrec.mjd, spwrec.mjd, nextspwrec.mjd, spwarr[recnum + off2].mjd,
								spwrec.mjd + fixf);
							if (f81type == 'l')
								f107bar = MathTimeLib::cubicinterp(
									lastspwrec.adjlstf81, spwrec.adjlstf81, nextspwrec.adjlstf81, spwarr[recnum + off2].adjlstf81,
									lastspwrec.mjd, spwrec.mjd, nextspwrec.mjd, spwarr[recnum + off2].mjd,
									spwrec.mjd + fixf);
							else
								f107bar = MathTimeLib::cubicinterp(
									lastspwrec.adjctrf81, spwrec.adjctrf81, nextspwrec.adjctrf81, spwarr[recnum + off2].adjctrf81,
									lastspwrec.mjd, spwrec.mjd, nextspwrec.mjd, spwarr[recnum + off2].mjd,
									spwrec.mjd + fixf);
						}
						else  // observed values
						{
							f107 = MathTimeLib::cubicinterp(
								lastspwrec.obsf10, spwrec.obsf10, nextspwrec.obsf10, spwarr[recnum + off2].obsf10,
								lastspwrec.mjd, spwrec.mjd, nextspwrec.mjd, spwarr[recnum + off2].mjd,
								spwrec.mjd + fixf);
							if (f81type == 'l')
								f107bar = MathTimeLib::cubicinterp(
									lastspwrec.obslstf81, spwrec.obslstf81, nextspwrec.obslstf81, spwarr[recnum + off2].obslstf81,
									lastspwrec.mjd, spwrec.mjd, nextspwrec.mjd, spwarr[recnum + off2].mjd,
									spwrec.mjd + fixf);
							else
								f107bar = MathTimeLib::cubicinterp(
									lastspwrec.obsctrf81, spwrec.obsctrf81, nextspwrec.obslstf81, spwarr[recnum + off2].obslstf81,
									lastspwrec.mjd, spwrec.mjd, nextspwrec.mjd, spwarr[recnum + off2].mjd,
									spwrec.mjd + fixf);
						}

					}  // spline interpolation

				} // if interp = f
		}
		else
			// ---- user input data
		if (inputtype == 'u')
		{
			// this is for data that may be simulated, or otherwise different from
			// the current noaa data

			// there could also be the interpolation stuff from above

		}
		else
			// ---- constant data
		if (inputtype == 'c')
		{
			// this data is the same all the time
			// leave the same as when it enters
		}

	}   // findatmosparam


	/* -----------------------------------------------------------------------------
	*
	*                           function kp2ap
	*
	*  this function converts kp to ap using cubic splines. notice the arrays go
	*  beyond the range of values to permit endpoint evaluations without additional
	*  logic. the arrays have an extra element so they will start at 1.
	*
	*  author        : david vallado                  719-573-2600   7 aug  2005
	*
	*  revisions
	*
	*  inputs          description                    range / units
	*    kpin        - kp
	*
	*  outputs       :
	*    kp2ap       - ap
	*
	*  locals        :
	*    idx         - index of function value above the input value so the input
	*                  value is between the 2nd and 3rd point
	*
	*  coupling      :
	*    cubicspl    - perform the splining operation given 4 points
	*
	*  references    :
	*    vallado       2004, 899-901
	* --------------------------------------------------------------------------- */

	double kp2ap
		(
		double kpin
		)
	{
		static double bapt[32] =
		{ 0, -0.00001, -0.001,
		0, 2, 3, 4, 5, 6, 7, 9, 12, 15, 18, 22, 27, 32,
		39, 48, 56, 67, 80, 94, 111, 132, 154, 179, 207, 236, 300, 400, 900
		};
		static double bkpt[32] =
		{ 0, -0.66666667, -0.33333,
		0, 0.33333, 0.66667, 1, 1.33333, 1.66667, 2, 2.33333,
		2.66667, 3, 3.33333, 3.66667, 4, 4.33333,
		4.66667, 5, 5.33333, 5.66667, 6.0, 6.33333, 6.66667, 7,
		7.33333, 7.66667, 8, 8.33333, 8.666667, 9, 9.33333
		};

		double bap[32], bkp[32];
		int idx;

		memcpy(bap, bapt, 32 * sizeof(double));
		memcpy(bkp, bkpt, 32 * sizeof(double));

		idx = 1;
		while ((idx < 33) && (kpin > bkp[idx]))
		{
			idx = idx + 1;
		}

		if (idx > 2)
		{
			return MathTimeLib::cubicinterp(bap[idx - 1], bap[idx], bap[idx + 1], bap[idx + 2],
				bkp[idx - 1], bkp[idx], bkp[idx + 1], bkp[idx + 2],
				kpin);
		} // if idx > 3
		else
			return 0.0;
	}  // kp2ap

	/* -----------------------------------------------------------------------------
	*
	*                           function ap2kp
	*
	*  this function converts ap to kp using cubic splines. notice the values go
	*  beyond the range of values to permit endpoint evaluations without additional
	*  logic. the arrays have an extra element so they will start at 1.
	*
	*  author        : david vallado                  719-573-2600   7 aug  2005
	*
	*  revisions
	*
	*  inputs          description                    range / units
	*    kpin        - kp
	*
	*  outputs       :
	*    ap2kp       - ap
	*
	*  locals        :
	*    idx         - index of function value above the input value so the input
	*                  value is between the 2nd and 3rd point
	*
	*  coupling      :
	*
	*  references    :
	*    vallado       2004, 899-901
	* --------------------------------------------------------------------------- */

	double ap2kp
		(
		double apin
		)
	{
		static double bapt[32] =
		{ 0, -0.00001, -0.001,
		0, 2, 3, 4, 5, 6, 7, 9, 12, 15, 18, 22, 27, 32,
		39, 48, 56, 67, 80, 94, 111, 132, 154, 179, 207, 236, 300, 400, 900
		};
		static double bkpt[32] =
		{ 0, -0.66666667, -0.33333,
		0, 0.33333, 0.66667, 1, 1.33333, 1.66667, 2, 2.33333,
		2.66667, 3, 3.33333, 3.66667, 4, 4.33333,
		4.66667, 5, 5.33333, 5.66667, 6.0, 6.33333, 6.66667, 7,
		7.33333, 7.66667, 8, 8.33333, 8.666667, 9, 9.33333
		};

		double bap[32], bkp[32];
		int  idx;

		memcpy(bap, bapt, 32 * sizeof(double));
		memcpy(bkp, bkpt, 32 * sizeof(double));

		idx = 1;
		while ((idx < 33) && (apin > bap[idx]))
		{
			idx = idx + 1;
		}

		if (idx > 1)
		{
			// desired point needs to be between 2 and 3
			return MathTimeLib::cubicinterp(bkp[idx - 1], bkp[idx], bkp[idx + 1], bkp[idx + 2],
				bap[idx - 1], bap[idx], bap[idx + 1], bap[idx + 2],
				apin);
		} // if idxs > 3
		else
			return 0.0;
	}   // ap2kp


} //  namespace 

