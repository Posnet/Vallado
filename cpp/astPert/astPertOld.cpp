/*      ----------------------------------------------------------------
 * 
 *                                 astpert.cpp
 * 
 *    This file contains fundamental Astrodynamic procedures and functions
 *    relating to the propagation and perturbation functions. These routines
 *    are discussed in Ch 8 and Ch 9.
 * 
 *                           Companion code for
 *              Fundamentals of Astrodynamics and Applications
 *                                   2007
 *                             by David Vallado
 * 
 *        (w) 719-573-2600, email dvallado@agi.com
 * 
 *     current :
 *               11 jan 18  david vallado
 *                            misc cleanup
 *     changes :
 *               30 sep 15  david vallado
 *                            fix jdutc, jdfrac
 *                4 may 09  david vallado
 *                            original baseline
 *        ----------------------------------------------------------------       */

// stdafx.h must be the first in cpp files
//#include "stdafx.h"

#include "astPert.h"

namespace astPert
{
	/*  ------------------------------------------------------------------------------
	 * 
	 *                                 procedure itermeanoscZonal
	 * 
	 *  This function iterates on the secular/periodic equations to convert between osculating
	 *   and mean elements. it only considers zonal harmonics, and the periodic terms.
	 * 
	 *   author        : david vallado                  719-573-2600   11 jan 2018
	 * 
	 *   inputs          description                    range / units
	 *     jdutc       - epoch julian date              days from 4713 BC
	 *     jdutcF      - epoch julian date fraction     day fraction from jdutc
	 *     dat         - number of leap seconds         s
	 *     dut1        - delta between utc and ut1      s
	 *     rosc        - osculating position vector     km
	 *     vosc        - osculating velocity vector     km/s
	 * 
	 *   outputs       :
	 *     rmean       - mean position vector           km
	 *     vmean       - mean velocity vector           km/s
	 * 
	 *   locals        :
	 *     r3          - Cube of r
	 * 
	 *   coupling      :
	 *     astTime:: jday
	 *     ast2Body:: rv2eq, eq2rv, rv2coe, sunmoonjpl
	 * 
	 *   references    :
	 *     chao, hoots
	-----------------------------------------------------------------------------  */

	void itermeanoscZonal
		(
		double jdutc,
		double jdutcF,
		double dat, double dut1,
		double rosc[3], double vosc[3],
		double rmean[3], double vmean[3]
		)
	{
		double dtsec;
		double jdut1, jdut1f, tut1, jdtt, jdttf, ttt, aosc, nosc, afosc, agosc, chiosc, psiosc, mlonmosc, mlonnuosc, 
			posc, eccosc, inclosc, raanosc, argposc, nuosc, mosc, eccanomosc, arglatosc, truelonosc, lonperosc;
		double temp;
		double amean, afmean, agmean, chimean, psimean, mlonmmean, pmean, eccmean, inclmean, raanmean, argpmean, numean, mmean, eccanommean, arglatmean, truelonmean, lonpermean;
		double aosco, afosco, agosco, chiosco, psiosco, mlonmosco;
		double dndtp, dedtp, didtp, draandtp, dargpdtp, dmdtp, dela, delaf, delag, delchi, delpsi, delmlonm;
		double p1, a1, ecc1, incl1, raan1, argp1, nu1, m1, eccanom1, arglat1, truelon1, lonper1;
		double sini, cosi, cosargp, sinargp, cosraan, sinraan, beta, n1, beta2, magr;
		double rcurr[3], vcurr[3];
		int fr;
		double rmean1[3], vmean1[3], dr[3], magdr;

		double rad = 180.0 / pi;
		double dayconv = 1.0 / 86400.0;
		double jyrconv = 1.0 / 36525.0;

		// evaluate at epoch time only
		dtsec = 0.0;

		// either find these, or fix so the same as calling
		jdut1 = jdutc;
		jdut1f = jdutcF + dut1*dayconv;
		tut1 = (jdut1 + jdut1f - 2451545.0) * jyrconv;
		jdtt = jdutc;
		jdttf = jdutcF + (dat + 32.184)*dayconv;
		ttt = (jdtt + jdttf - 2451545.0) * jyrconv;

		printf("osculating   %11.6f %11.6f %11.6f %11.6f %11.6f %11.6f \n", rosc[0], rosc[1], rosc[2], vosc[0], vosc[1], vosc[2]);

		ast2Body::rv2eq(rosc, vosc, aosc, nosc, afosc, agosc, chiosc, psiosc, mlonmosc, mlonnuosc, fr);
		mlonmosc = fmod(mlonmosc, twopi);
		printf("eq osc  %12.9f %12.9f %12.9f %12.9f %12.9f %12.9f \n", aosc, afosc, agosc, chiosc, psiosc, mlonmosc * rad);

		// find osc coes
		ast2Body::rv2coe(rosc, vosc, mu, posc, aosc, eccosc, inclosc, raanosc, argposc, nuosc, mosc, eccanomosc, arglatosc, truelonosc, lonperosc);
		printf("coes osc   %11.4f %11.4f %13.9f %13.7f %11.5f %11.5f %11.5f %11.5f %11.5f \n",
			posc, aosc, eccosc, inclosc * rad, raanosc * rad, argposc * rad, nuosc * rad, mosc * rad, arglatosc * rad);

		// set initial guess for mean elements
		// use as initial guess for mean
		amean = aosc;
		afmean = afosc;
		agmean = agosc;
		chimean = chiosc;
		psimean = psiosc;
		mlonmmean = mlonmosc;

		// store originals for residual calcs later
		aosco = aosc;
		afosco = afosc;
		agosco = agosc;
		chiosco = chiosc;
		psiosco = psiosc;
		mlonmosco = mlonmosc;

		// --------------------------iterate to find mean elements----------------------- 
		// set initial to get into loop
		magdr = 1000.0;
		int i = 0;
		printf("begin iterating\n");
		while (i <= 20 && magdr > 0.1)
		{
			ast2Body::eq2rv(amean, afmean, agmean, chimean, psimean, mlonmmean, fr, rcurr, vcurr);
			ast2Body::rv2coe(rcurr, vcurr, mu, p1, a1, ecc1, incl1, raan1, argp1, nu1, m1, eccanom1, arglat1, truelon1, lonper1);
			cosargp = cos(argp1);
			sinargp = sin(argp1);
			cosi = cos(incl1);
			sini = sin(incl1);
			cosraan = cos(raan1);
			sinraan = sin(raan1);
			magr = astMath::mag(rcurr);
			beta = sqrt(1.0 - ecc1 * ecc1);
			beta2 = (1.0 - ecc1 * ecc1);
			n1 = sqrt(mu / pow(a1, 3));

			// ----------------------------------periodic equations----------------------------
			dndtp = -1.5 * n1 * j2 * pow(re / a1, 2) * (pow(a1 / magr, 3) * ((1.0 - 1.5 * sini * sini) + 1.5 * sini * sini * cos(2.0 * nu1 + 2.0 * argp1))
				- (1.0 - 1.5 * sini * sini) * pow(beta2, -1.5));
			dedtp = 0.5 * j2 * pow(re / p1, 2) * (1.0 - 1.5 * sini * sini) * (2.5 * ecc1 + (ecc1 - pow(ecc1, 3)) / (1.0 + beta) + 3.0 * (1.0 - 0.25 * ecc1 * ecc1) * cos(nu1)
				+ 1.5 * ecc1 * cos(2.0 * nu1) + 0.25 * ecc1 * ecc1 * cos(3.0 * nu1))
				+ 3.0 / 8.0 * j2 * pow(re / p1, 2) * sini * sini  * 
				((1.0 + 11.0 / 4.0 * ecc1 * ecc1) * cos(nu1 + 2.0 * argp1)
				+ 0.25 * ecc1 * ecc1 * cos(2.0 * argp1 - nu1) + 5.0 * ecc1 * cos(2.0 * argp1 + 2.0 * nu1)
				+ 1.0 / 3.0 * (7.0 + 17.0 / 4.0 * ecc1 * ecc1) * cos(3.0 * nu1 + 2.0 * argp1)
				+ 1.5 * ecc1 * cos(4.0 * nu1 + 2.0 * argp1) + 0.25 * ecc1 * ecc1 * cos(5.0 * nu1 + 2.0 * argp1) + 1.5 * ecc1 * cos(2.0 * argp1));
			didtp = 3.0 / 8.0 * j2 * pow(re / p1, 2) * sin(2.0 * incl1) * (ecc1 * cos(nu1 + 2.0 * argp1) + cos(2.0 * nu1 + 2.0 * argp1)
				+ ecc1 / 3.0 * cos(3.0 * nu1 + 2.0 * argp1));
			draandtp = -1.5 * j2 * pow(re / p1, 2) * cosi * (nu1 - m1 + ecc1 * sin(nu1) - 0.5 * ecc1 * sin(nu1 + 2.0 * argp1)
				- 0.5 * sin(2.0 * nu1 + 2.0 * argp1) - ecc1 / 6.0 * sin(3.0 * nu1 + 2.0 * argp1));
			dargpdtp = 0.75 * j2 * pow(re / p1, 2) * (4.0 - 5.0 * sini * sini) * (nu1 - m1 + ecc1 * sin(nu1))
				+ 1.5 * j2 * pow(re / p1, 2) * (1.0 - 1.5 * sini * sini) * (1.0 / ecc1 * (1.0 - 0.25 * ecc1 * ecc1) * sin(nu1) + 0.5 * sin(2.0 * nu1) + 1.0 / 12.0 * ecc1 * sin(3.0 * nu1))
				- 1.5 * j2 * pow(re / p1, 2) * (1.0 / ecc1 * (0.25 * sini * sini + 0.5 * ecc1 * ecc1 * (1.0 - 15.0 / 8.0 * sini * sini)) * sin(nu1 + 2.0 * argp1))
				+ ecc1 / 16.0 * sini * sini * sin(2.0 * argp1 - nu1) + 0.5 * (1.0 - 2.5 * sini * sini) * sin(2.0 * nu1 + 2.0 * argp1)
				- 1.0 / ecc1 * (7.0 / 12.0 * sini * sini - ecc1 * ecc1 / 6.0 * (1.0 - 19.0 / 8.0 * sini * sini)) * sin(3.0 * nu1 + 2.0 * argp1) - 3.0 * 8.0 * sini * sini * sin(4.0 * nu1 + 2.0 * argp1)
				- 1.0 / 16.0 * ecc1 * sini * sini * sin(5.0 * nu1 + 2.0 * argp1)
				- 9.0 / 16.0 * j2 * pow(re / p1, 2) * sini * sini * sin(2.0 * argp1);
			dmdtp = -1.5 * j2 * pow(re / p1, 2) * beta / ecc1 * ((1.0 - 1.5 * sini * sini) * ((1.0 - 0.25 * ecc1 * ecc1) * sin(nu1) + 0.5 * ecc1 * sin(2.0 * nu1) + ecc1 * ecc1 / 12.0 * sin(3.0 * nu1))
				+ 0.5 * sini * sini * (-0.5 * (1.0 + 1.25 * ecc1 * ecc1) * sin(nu1 + 2.0 * argp1) - ecc1 * ecc1 / 8.0 * sin(2.0 * argp1 - nu1)
				+ 7.0 / 6.0 * (1.0 - ecc1 * ecc1 / 28.0) * sin(3.0 * nu1 + 2.0 * argp1) + 0.75 * ecc1 * sin(4.0 * nu1 + 2.0 * argp1) + ecc1 * ecc1 / 8.0 * sin(5.0 * nu1 + 2.0 * argp1)))
				+ 9.0 / 16.0 * j2 * pow(re / p1, 2) * beta * sini * sini * sin(argp1);

			// ------------ - periodic equations in equinoctial space----------------
			// find the osculating contribution to add to the mean sec / lp values in equinoctial space
			temp = 1.0 / (1.0 + cosi);
			dela = -2.0 / 3.0 * a1 * dndtp / n1;
			delaf = dedtp * cos(argp1 + raan1) - ecc1 * (dargpdtp + draandtp) * sin(argp1 + raan1);
			delag = dedtp * sin(argp1 + raan1) + ecc1 * (dargpdtp + draandtp) * cos(argp1 + raan1);
			delchi = didtp * sin(raan1) * temp + sini * draandtp * cos(raan1) * temp;
			delpsi = didtp * cos(raan1) * temp - sini * draandtp * sin(raan1) * temp;
			delmlonm = dmdtp + dargpdtp + draandtp;

			// -----------  updated osculating equinoctial elements--------------------
			temp = 1.0;
			aosc = amean + temp * dela;
			afosc = afmean + temp * delaf;
			agosc = agmean + temp * delag;
			chiosc = chimean + temp * delchi;
			psiosc = psimean + temp * delpsi;
			mlonmosc = mlonmmean + temp * delmlonm;
			mlonmosc = fmod(mlonmosc, twopi);

			temp = -1.0;  // use lambda 0.3 per Cain paper ?
			amean = amean + temp * (aosc - aosco);
			afmean = afmean + temp * (afosc - afosco);
			agmean = agmean + temp * (agosc - agosco);
			chimean = chimean + temp * (chiosc - chiosco);
			psimean = psimean + temp * (psiosc - psiosco);
			mlonmmean = mlonmmean + temp * (mlonmosc - mlonmosco);
			mlonmmean = fmod(mlonmmean, twopi);

			ast2Body::eq2rv(amean, afmean, agmean, chimean, psimean, mlonmmean, fr, rmean1, vmean1);
			for (int j = 0; j < 3; j++)
				dr[j] = rcurr[j] - rmean1[j];
			magdr = astMath::mag(dr);
			printf("%5i %11.7f %11.7f %11.7f %11.7f %11.7f %11.7f %11.7f \n",
				i, aosc - aosco, afosc - afosco, agosc - agosco, chiosc - chiosco, psiosc - psiosco, mlonmosc - mlonmosco, magdr);

			i = i + 1;
		}   // while

		ast2Body::eq2rv(amean, afmean, agmean, chimean, psimean, mlonmmean, fr, rmean, vmean);
		printf("vectors mean  %14.8f %14.8f %14.8f %14.9f %14.9f %14.9f \n", rmean[0], rmean[1], rmean[2], vmean[0], vmean[1], vmean[2]);
		printf("rmean = [ %14.8f, %14.8f, %14.8f]; \nv = [ %14.9f, %14.9f, %14.9f]; \n", rmean[0], rmean[1], rmean[2], vmean[0], vmean[1], vmean[2]);
		ast2Body::rv2coe(rmean, vmean, mu, pmean, amean, eccmean, inclmean, raanmean, argpmean, numean, mmean, eccanommean, arglatmean, truelonmean, lonpermean);
		printf("coes mean  %11.4f %11.4f %13.9f %13.7f %11.5f %11.5f %11.5f %11.5f %11.5f \n",
			pmean, amean, eccmean, inclmean * rad, raanmean * rad, argpmean * rad, numean * rad, mmean * rad, arglatmean * rad);
	}  // itermeanoscZonal


	/*  ------------------------------------------------------------------------------
	 * 
	 *                                 procedure itermeanosc3body
	 * 
	 *  This function iterates on the secular/periodic equations to convert between osculating
	 *   and mean elements. it only considers 3body perturbations and periodic terms.
	 * 
	 *   author        : david vallado                  719-573-2600   11 jan 2018
	 * 
	 *   inputs          description                    range / units
	 *     jdutc       - epoch julian date              days from 4713 BC
	 *     jdutcF      - epoch julian date fraction     day fraction from jdutc
	 *     dat         - number of leap seconds         s
	 *     dut1        - delta between utc and ut1      s
	 *     rosc        - osculating position vector     km
	 *     vosc        - osculating velocity vector     km/s
	 *     interp      - interpolationg for sun/moon    'l', 's'
	 *     jpldearr    - array of Sun and Moon position vectors from de430
	 *     jdjpldestart- jd of start of array of de vectors days frm 4713 BC
	 * 
	 *   outputs       :
	 *     rmean       - mean position vector           km
	 *     vmean       - mean velocity vector           km/s
	 * 
	 *   locals        :
	 *     r3          - Cube of r
	 * 
	 *   coupling      :
	 *     astTime:: jday
	 *     ast2Body:: rv2eq, eq2rv, rv2coe, sunmoonjpl
	 * 
	 *   references    :
	 *     chao, hoots
	-----------------------------------------------------------------------------  */

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
		)
	{
		double dtsec;
		double jdut1, jdut1f, tut1, jdtt, jdttf, ttt, aosc, nosc, afosc, agosc, chiosc, psiosc, mlonmosc, mlonnuosc, 
			posc, eccosc, inclosc, raanosc, argposc, nuosc, mosc, eccanomosc, arglatosc, truelonosc, lonperosc;
		double amean, afmean, agmean, chimean, psimean, mlonmmean, pmean, eccmean, inclmean, raanmean, argpmean, numean, mmean, eccanommean, arglatmean, truelonmean, lonpermean;
		double aosco, afosco, agosco, chiosco, psiosco, mlonmosco;
		double x3, x4, dndtp, dedtp, didtp, draandtp, dargpdtp, temp, dmdtp, delaf, delag, temp1, delchi, delpsi, delmlonm;
		double rcurr[3], vcurr[3];
		double rsun[3], rtascs, decls, rmoon[3], rtascm, declm;
		int fr, ithird;
		double eccanom1;
		double p1, a1, n1, ecc1, incl1, raan1, argp1, nu1, m1, arglat1, truelon1, lonper1;
		double deln, del1, del2, e11, e21, e31, cosargp1, sinargp1, cosraan1, sinraan1;
		double q11, q12, q13, q21, q22, q23, q31, q32, q33;
		double u3vec[3], mu3, mass3, magr3, mrat3, mrat4, dr[3];
		double dndt3p, dedt3p, didt3p, draandt3p, dargpdt3p, dmdt3p;
		double dndtps, dedtps, didtps, draandtps, dargpdtps, dmdtps, dndtpm, dedtpm, didtpm, draandtpm, dargpdtpm, dmdtpm;
		double rmean1[3], vmean1[3], magdr;

		double rad = 180.0 / pi;
		double dayconv = 1.0 / 86400.0;
		double jyrconv = 1.0 / 36525.0;
		double m3s = 1.9891e30;     // sun kg
		double massE = 5.9742e24;     // earth kg
		double m3m = 7.3483e22;     // moon kg

		// evaluate at epoch time only
		dtsec = 0.0;

		// either find these, or fix so the same as calling
		jdut1 = jdutc;
		jdut1f = jdutcF + dut1 * dayconv;
		tut1 = (jdut1 + jdut1f - 2451545.0) * jyrconv;
		jdtt = jdutc;
		jdttf = jdutcF + (dat + 32.184) * dayconv;
		ttt = (jdtt + jdttf - 2451545.0) * jyrconv;

		printf("osculating   %11.6f %11.6f %11.6f %11.6f %11.6f %11.6f \n", rosc[0], rosc[1], rosc[2], vosc[0], vosc[1], vosc[2]);

		ast2Body::rv2eq(rosc, vosc, aosc, nosc, afosc, agosc, chiosc, psiosc, mlonmosc, mlonnuosc, fr);
		mlonmosc = fmod(mlonmosc, twopi);
		printf("eq osc  %12.9f %12.9f %12.9f %12.9f %12.9f %12.9f \n", aosc, afosc, agosc, chiosc, psiosc, mlonmosc * rad);

		// hardwire jpl and linear for now
		// if (opt == 'j')
		{
			//jpl ephemerides - just do once for sun and moon
			// let tt approx tdb
			ast2Body::sunmoonjpl(jdtt, jdttf, interp, jpldearr, jdjpldestart, rsun, rtascs, decls, rmoon, rtascm, declm);
			//			r3mag = astMath::mag(r3);
			//printf("sun i\n%lf %lf  %lf %lf %lf   %lf %lf %lf \n", jdtt, jdttf, rsun[0], rsun[1], rsun[2], rmoon[0], rmoon[1], rmoon[2]);
		}

		// find osc coes
		ast2Body::rv2coe(rosc, vosc, mu, posc, aosc, eccosc, inclosc, raanosc, argposc, nuosc, mosc, eccanomosc, arglatosc, truelonosc, lonperosc);
		printf("coes osc   %11.4f %11.4f %13.9f %13.7f %11.5f %11.5f %11.5f %11.5f %11.5f \n",
			posc, aosc, eccosc, inclosc * rad, raanosc * rad, argposc * rad, nuosc * rad, mosc * rad, arglatosc * rad);

		// set initial guess for mean elements
		// use as initial guess for mean
		amean = aosc;
		afmean = afosc;
		agmean = agosc;
		chimean = chiosc;
		psimean = psiosc;
		mlonmmean = mlonmosc;

		// store originals for residual calcs later
		aosco = aosc;
		afosco = afosc;
		agosco = agosc;
		chiosco = chiosc;
		psiosco = psiosc;
		mlonmosco = mlonmosc;

		// --------------------------iterate to find mean elements----------------------- 
		// set initial to get into loop
		magdr = 1000.0;
		int ii = 0;

		printf("begin iterating\n");
		double cosi1, sini1, beta1;
		while (ii <= 20 && magdr > 0.01)  //0.025
		{
			ast2Body::eq2rv(amean, afmean, agmean, chimean, psimean, mlonmmean, fr, rcurr, vcurr);
			ast2Body::rv2coe(rcurr, vcurr, mu, p1, a1, ecc1, incl1, raan1, argp1, nu1, m1, eccanom1, arglat1, truelon1, lonper1);
			cosargp1 = cos(argp1);
			sinargp1 = sin(argp1);
			cosi1 = cos(incl1);
			sini1 = sin(incl1);
			cosraan1 = cos(raan1);
			sinraan1 = sin(raan1);
			beta1 = sqrt(1.0 - ecc1 * ecc1);
			n1 = sqrt(mu / (a1 * a1 * a1));

			q11 = cosargp1 * cosraan1 - sinargp1 * sinraan1 * cosi1;
			q12 = cosargp1 * sinraan1 + sinargp1 * cosraan1 * cosi1;
			q13 = sinargp1 * sini1;
			q21 = -sinargp1 * cosraan1 - cosargp1 * sinraan1 * cosi1;
			q22 = -sinargp1 * sinraan1 + cosargp1 * cosraan1 * cosi1;
			q23 = cosargp1 * sini1;
			q31 = sinraan1 * sini1;
			q32 = -cosraan1 * sini1;
			q33 = cosi1;

			// calculate for both sun and moon
			for (ithird = 1; ithird <= 2; ithird++)
			{
				if (ithird == 1)
				{
					astMath::norm(rsun, u3vec);
					mu3 = muSun;  // km3 / s2
					mass3 = m3s;  // kg
					magr3 = astMath::mag(rsun);
				}
				else
				{
					astMath::norm(rmoon, u3vec);
					mu3 = muMoon;
					mass3 = m3m;
					magr3 = astMath::mag(rmoon);
				}
				mrat3 = (mass3 / massE) * pow(a1 / magr3, 3);  // kg / kg km / km->rad
				mrat4 = (mass3 / massE) * pow(a1 / magr3, 4);

				e11 = q11 * u3vec[0] + q12 * u3vec[1] + q13 * u3vec[2];
				e21 = q21 * u3vec[0] + q22 * u3vec[1] + q23 * u3vec[2];
				e31 = q31 * u3vec[0] + q32 * u3vec[1] + q33 * u3vec[2];

				// ----------------------------------periodic equations----------------------------
				dndtp = -3.0 * n1 * mrat3 * (-3.0 * ecc1 * beta1 * e11 * e21 * sin(eccanom1) * sin(2.0 * eccanom1) + 1.5 * e11 * e21 * beta1 * sin(2.0 * eccanom1) -
					ecc1 * (3.0 * e11 * e11 - 1.0) * cos(eccanom1) + 0.25 * (3.0 * (e11 * e11 - e21 * e21) + (3.0 * e21 * e21 - 1.0) * ecc1 * ecc1) * cos(2.0 * eccanom1)) -
					9.0 / 8.0 * n1 / beta1 * mrat4 * (e21 * (-4.0 + 3.0 * ecc1 * ecc1 + pow(ecc1, 4) + 5.0 * e11 * e11 + 15.0 * ecc1 * ecc1 * e11 * e11 + 5.0 * e21 * e21 - 10.0 * ecc1 * ecc1 * e21 * e21 + 5.0 * pow(ecc1, 4) * e21 * e21 - 20.0 * pow(ecc1, 4) * e11 * e11) * sin(eccanom1) +
					4.0 * ecc1 * e21 * (1.0 - ecc1 * ecc1) * (1.0 - 5.0 * e11 * e11) * sin(2.0 * eccanom1) +
					1.0 / 3.0 * e21 * beta1 * beta1 * (5.0 * (3.0 * e11 * e11 - e21 * e21) + ecc1 * ecc1 * (5.0 * e11 * e11 - 3.0)) * sin(3.0 * eccanom1) +
					beta1 * e11 * ((5.0 * e21 * e21 - 1.0) * (4.0 + 11.0 * ecc1 * ecc1) + 5.0 * (e11 * e11 - 3.0 * e21 * e21) * (1.0 + 4.0 * ecc1 * ecc1)) * cos(eccanom1) -
					2.0 * beta1 * ecc1 * e11 * ((5.0 * e21 * e21 - 1.0) * (2.0 + ecc1 * ecc1) + 5.0 * (e11 * e11 - 3.0 * e21 * e21)) * cos(2.0 * eccanom1) +
					1.0 / 3.0 * beta1 * e11 * (5.0 * (e11 * e11 - 3.0 * e21 * e21) + 3.0 * ecc1 * ecc1 * (5.0 * ecc1 * ecc1 - 1.0)) * cos(3.0 * eccanom1));
				// all the rest are unitless
				dedtp = -7.5 * ecc1  * beta1 * mrat3 * ecc1  * beta1  * e11  * e21  * (eccanom1 - m1) +
					2.25 * mrat3 * beta1  * e11  * e21  * ((2.0 + 3.0 * ecc1 * ecc1) * sin(eccanom1) - ecc1  * sin(2.0 * eccanom1) + 1.0 / 9.0 * (2.0 - ecc1 * ecc1) * sin(3.0 * eccanom1)) +
					0.25 * mrat3 * beta1 * beta1 * (3.0 * cos(eccanom1) * (e11 * e11 - 5.0 * e21 * e21 + 4.0 / 3.0) + ecc1  * (6.0 * e21 * e21 - 3.0 * e11 * e11 - 1.0) * cos(2.0 * eccanom1) - (e21 * e21 - e11 * e11) * cos(3.0 * eccanom1)) -
					1.5 * mrat4 * (beta1  * ecc1  * e11  * (2.5 * (e11 * e11 - 3.0 * e21 * e21) * 1.0 - 4.0 * ecc1 * ecc1) - 2.0 * (5.0 * e21 * e21 - 1.0) -
					1.0 / 8.0 * beta1  * e11  * cos(2.0 * eccanom1) * (5.0 * (e11 * e11 - 3.0 * e21 * e21) * (1.0 + 2.0 * ecc1 * ecc1) - (5.0 * e21 * e21 - 1.0) * (2.0 + 3.0 * ecc1 * ecc1)) +
					1.0 / 6.0 * beta1  * ecc1  * e11  * cos(3.0 * eccanom1) * (5.0 * (e11 * e11 - 3.0 * e21 * e21)) -
					1.0 / 32.0 * beta1  * e11  * cos(4.0 * eccanom1) * (5.0 * (e11 * e11 - 3.0 * e21 * e21) + (5.0 * e21 * e21 - 1.0) * ecc1 * ecc1) +
					5.0 / 8.0 * e21  * (eccanom1 - m1) * ((1.0 + 10.0 * e11 * e11 - 5.0 * e21 * e21) * (4.0 + 3.0 * ecc1 * ecc1) - 5.0 * (3.0 * e11 * e11 - e21 * e21) * (3.0 - 4.0 * ecc1 * ecc1)) +
					0.25 * ecc1  * e21  * sin(eccanom1) * (-24.0 - 4.0 * ecc1 * ecc1 + 90.0 * e11 * e11 + 50.0 * ecc1 * ecc1 * e11 * e11 + 10.0 * e21 * e21 - 10.0 * ecc1 * ecc1 * e21 * e21) +
					0.25 * e21  * sin(2.0 * eccanom1) * (-1.0 + 8.0 * ecc1 * ecc1 - 10.0 * e11 * e11 + 5.0 * e21 * e21 - 25.0 * ecc1 * ecc1 * e11 * e11 - 5.0 * ecc1 * ecc1 * e21 * e21 + 15.0 * ecc1 * ecc1 * e21 * e21) +
					1.0 / 12.0 * ecc1  * e21  * sin(3.0 * eccanom1) * ((-3.0 + ecc1 * ecc1) * (1.0 + 10.0 * e11 * e11 - 5.0 * e21 * e21) + 15.0 * (3.0 * e11 * e11 - e21 * e21) + 3.0 -
					3.0 * ecc1 * ecc1 + 15.0 * e11 * e11 - 10.0 * e21 * e21 + 5.0 * ecc1 * ecc1 * e21 * e21) +
					1.0 / 32.0 * e21  * sin(4.0 * eccanom1) * (ecc1 * ecc1 * (1.0 + 10.0 * e11 * e11 - 5.0 * e21 * e21) - 5.0 * (3.0 * e11 * e11 - e21 * e21)));

				del1 = -5.0 / 8.0 * (5.0 * e21 * e21 - 1.0) * ecc1  * (4.0 + 3.0 * ecc1 * ecc1) * (eccanom1 - m1) - 25.0 / 8.0 * (e11 * e11 - e21 * e21) * ecc1  * (3.0 + 4.0 * ecc1 * ecc1) * (eccanom1 - m1) +
					(5.0 * e21 * e21 - 1.0) * (1.0 + 21.0 / 4.0 * ecc1 * ecc1 + 0.75 * pow(ecc1, 4)) * sin(eccanom1) + 5.0 * (e11 * e11 - e21 * e21) * (0.75 + 21.0 / 4.0 * ecc1 * ecc1 - pow(ecc1, 4)) * sin(eccanom1) -
					0.25 * (5.0 * e21 * e21 - 1.0) * ecc1  * (3.0 + 4.0 * ecc1 * ecc1) * sin(2.0 * eccanom1) - 5.0 / 4.0 * (e11 * e11 - e21 * e21) * ecc1  * (4.0 + 3.0 * ecc1 * ecc1) * sin(2.0 * eccanom1) +
					1.0 / 12.0 * sin(3.0 * eccanom1) * ((5.0 * e21 * e21 - 1.0) * (3.0 * ecc1 * ecc1 + pow(ecc1, 4)) + 5.0 * (e11 * e11 - e21 * e21) * (1.0 - 3.0 * ecc1 * ecc1)) -
					1.0 / 32.0 * sin(4.0 * eccanom1) * ((1.0 + 2.0 * ecc1 * ecc1) * ecc1 * ecc1 + 5.0 * (e11 * e11 - e21 * e21)) +
					2.5 * beta1  * e11  * e21  * (-(1.0 + 6.0 * ecc1 * ecc1) * cos(eccanom1) + ecc1  * (2.5 + ecc1 * ecc1) * cos(2.0 * eccanom1) - 1.0 / 3.0 * (1.0 + 2.0 * ecc1 * ecc1) * cos(3.0 * eccanom1) + 1.0 / 8.0 * ecc1  * cos(4.0 * eccanom1));

				del2 = -0.25 * beta1  * ((5.0 * e21 * e21 - 1.0) * (4.0 + 3.0 * ecc1 * ecc1) + 5.0 * (e11 * e11 - e21 * e21) * (1.0 + 2.0 * ecc1 * ecc1)) * cos(eccanom1) +
					1.0 / 8.0 * ecc1  * beta1  * ((5.0 * e21 * e21 - 1.0) * (6.0 + ecc1 * ecc1) + 5.0 * (e11 * e11 - e21 * e21) * (3.0 + 2.0 * ecc1 * ecc1)) * cos(2.0 * eccanom1) -
					1.0 / 12.0 * beta1  * ((5.0 * e21 * e21 - 1.0) * 3.0 * ecc1 * ecc1 + 5.0 * (e11 * e11 - e21 * e21) * (1.0 + 2.0 * ecc1 * ecc1)) * cos(3.0 * eccanom1) +
					1.0 / 32.0 * ecc1  * beta1  * ((5.0 * e21 * e21 - 1.0) * ecc1 * ecc1 + 5.0 * (e11 * e11 - e21 * e21)) * cos(4.0 * eccanom1) +
					10.0 * e11  * e21  * (1.0 - ecc1 * ecc1) * (-5.0 / 8.0 * (eccanom1 - m1) + 0.25 * (1.0 + ecc1 * ecc1) * sin(eccanom1) +
					0.25 * ecc1  * sin(2.0 * eccanom1) - 1.0 / 12.0 * (1.0 + ecc1 * ecc1) * sin(3.0 * eccanom1) + 1.0 / 32.0 * ecc1  * sin(4.0 * eccanom1));

				didtp = 1.5 / beta1  * mrat3 * e31  * (e11  * (1.0 + 4.0 * ecc1 * ecc1) * cosargp1 - (1.0 - ecc1 * ecc1) * e21  * sinargp1) * (eccanom1 - m1) +
					1.0 / beta1  * mrat3 * e31  * (-3.0 * ecc1  * sin(eccanom1) * ((11.0 / 4.0 + ecc1 * ecc1) * e11  * cosargp1 - 0.25 * (1.0 - ecc1 * ecc1) * e21  * sinargp1) +
					0.75 * sin(2.0 * eccanom1) * ((1.0 + 2.0 * ecc1 * ecc1) * e11  * cosargp1 + (1.0 - ecc1 * ecc1) * e21  * sinargp1) -
					0.25 * ecc1  * sin(3.0 * eccanom1) * (e11  * cosargp1 + (1.0 - ecc1 * ecc1) * e21  * sinargp1) +
					0.75 * beta1  * (e21  * cosargp1 - e11  * sinargp1) * (5.0 * ecc1  * cos(eccanom1) - (1.0 + ecc1 * ecc1) * cos(2.0 * eccanom1) + 1.0 / 3.0 * ecc1  * cos(3.0 * eccanom1))) +
					1.5 / beta1  * mrat4 * e31  * (del1 * cosargp1 - del2 * sinargp1);
				draandtp = 1.5 * 1.90 / (beta1  * sini1) * mrat3 * e31  * (e21  * (1.0 - ecc1 * ecc1) * cosargp1 + (1.0 + 4.0 * ecc1 * ecc1) * e11  * sinargp1) * (eccanom1 - m1) +
					1.0 / (beta1  * sini1) * mrat3 * e31  * ((-3.0 * ecc1  * (0.25 * e21  * (1.0 - ecc1 * ecc1) * cosargp1 + (11.0 / 4.0 + ecc1 * ecc1) * e11  * sinargp1) * e11  * sinargp1) * sin(eccanom1) +
					0.75 * (-e21  * (1.0 - ecc1 * ecc1) * cosargp1 + (1.0 + 2.0 * ecc1 * ecc1) * e11  * sinargp1) * sin(2.0 * eccanom1) -
					0.25 * ecc1  * (-e21  * (1.0 - ecc1 * ecc1) * cosargp1 + e11  * sinargp1) * sin(3.0 * eccanom1) +
					0.75 * beta1  * (e11  * cosargp1 + e21  * sinargp1) * (5.0 * ecc1  * cos(eccanom1) - (1.0 - ecc1 * ecc1) * cos(2.0 * eccanom1) + 1.0 / 3.0 * ecc1  * cos(eccanom1))) +
					1.5 / (beta1  * sini1) * mrat4 * e31  * (del2 * cosargp1 + del1 * sinargp1);
				x3 = 1.5 * mrat3 * ecc1  * beta1  * (4.0 * e11 * e11 - e21 * e21 - 1.0) * (eccanom1 - m1) +
					mrat3 * beta1  * (((-15.0 / 4.0 * e11 * e11 + 0.75 * e21 * e21 + 1.0) - ecc1 * ecc1 * (3.0 * e11 * e11 - 1.0)) * sin(eccanom1) +
					0.25 * ecc1  * (3.0 * e21 * e21 - 1.0) * sin(2.0 * eccanom1) - 0.25 * (e21 * e21 - e11 * e11) * sin(3.0 * eccanom1) +
					0.75 / beta1  * e11  * e21  * (6.0 - 11.0 * ecc1 * ecc1) * cos(eccanom1) + ecc1  * (1.0 + ecc1 * ecc1) * cos(2.0 * eccanom1) - 1.0 / 3.0 * (2.0 - ecc1 * ecc1) * cos(3.0 * eccanom1)) +
					1.5 * beta1  * mrat4 * e11  * (1.0 / 8.0 * (20.0 + 45.0 * ecc1 * ecc1 - 25.0 * e11 * e11 - 100.0 * ecc1 * ecc1 * e11 * e11 - 25.0 * e21 * e21 + 75.0 * ecc1 * ecc1 * e21 * e21) * (eccanom1 - m1) +
					0.25 * ecc1  * (-36.0 - 11.0 * ecc1 * ecc1 + 65.0 * e11 * e11 + 20.0 * ecc1 * ecc1 * e11 * e11 - 15.0 * e21 * e21 - 5.0 * ecc1 * ecc1 * e21 * e21) * sin(eccanom1) +
					0.25 * (1.0 + 7.0 * ecc1 * ecc1 - 5.0 * e11 * e11 - 5.0 * ecc1 * ecc1 * e11 * e11 + 10.0 * e21 * e21 - 20.0 * ecc1 * ecc1 * e21 * e21) * sin(2.0 * eccanom1) +
					1.0 / 12.0 * ecc1  * (-ecc1 * ecc1 - 5.0 * e11 * e11 + 15.0 * e21 * e21 + 5.0 * ecc1 * ecc1 * e21 * e21) * sin(3.0 * eccanom1) +
					1.0 / 32.0 * (-ecc1 * ecc1 + 5.0 * e11 * e11 - 15.0 * e21 * e21 + 5.0 * ecc1 * ecc1 * e21 * e21) * sin(4.0 * eccanom1)) +
					1.5 * mrat4 * e21  * (-0.25 * ecc1  * (-4.0 + 11.0 * ecc1 * ecc1 - 11.0 * ecc1 * ecc1 + 65.0 * e11 * e11 - 100.0 * ecc1 * ecc1 * e11 * e11 - 15.0 * e21 * e21 + 15.0 * ecc1 * ecc1 * e21 * e21) * cos(eccanom1) -
					1.0 / 8.0 * (2.0 - 5.0 * ecc1 * ecc1 - 4.0 * ecc1 * ecc1 - 25.0 * e11 * e11 + 40.0 * ecc1 * ecc1 * e11 * e11 + 20.0 * pow(e21, 4) * e11 * e11 + 5.0 * e21 * e21 - 5.0 * ecc1 * ecc1 * e21 * e21) * cos(2.0 * eccanom1) -
					1.0 / 12.0 * ecc1  * (3.0 * ecc1 * ecc1 - 15.0 * e11 * e11 + 5.0 * ecc1 * ecc1 * e21 * e21) * cos(3.0 * eccanom1) -
					1.0 / 32.0 * (-ecc1 * ecc1 + 15.0 * e11 * e11 - 10.0 * ecc1 * ecc1 * e11 * e11 - 5.0 * e21 * e21 + 5.0 * ecc1 * ecc1 * e21 * e21) * cos(4.0 * eccanom1));
				dargpdtp = -draandtp * cosi1 + 1.0 / ecc1  * x3;
				x4 = -21.0 / 4.0 * mrat3 * (e11 * e11 * (1.0 + 4.0 * ecc1 * ecc1) + e21 * e21 * (1.0 - ecc1 * ecc1) - (2.0 / 3.0 + ecc1 * ecc1)) * (eccanom1 - m1) -
					7.0 * mrat3 * (-1.5 * ecc1  * (e11 * e11 * (11.0 / 4.0 + ecc1 * ecc1) + 0.25 * e21 * e21 * (1.0 - ecc1 * ecc1) - (1.0 + 0.25 * ecc1 * ecc1)) * sin(eccanom1) +
					3.0 / 8.0 * (e11 * e11 * (1.0 + 2.0 * ecc1 * ecc1) - e21 * e21 * (1.0 - ecc1 * ecc1) - ecc1 * ecc1) * sin(2.0 * eccanom1) -
					1.0 / 24.0 * ecc1  * (3.0 * e11 * e11 - 3.0 * e21 * e21 * (1.0 - ecc1 * ecc1) - ecc1 * ecc1) * sin(3.0 * eccanom1) +
					0.75 * beta1  * e11  * e21  * (5.0 * ecc1  * cos(eccanom1) - (1.0 + ecc1 * ecc1) * cos(2.0 * eccanom1) + 1.0 / 3.0 * ecc1  * cos(eccanom1))) -
					3.0 * mrat4 *  (-5.0 / 8.0 * ecc1  * e11  * (3.0 * (5.0 * e21 * e21 - 1.0) * (4.0 + 3.0 * ecc1 * ecc1) + 5.0 * (e11 * e11 - 3.0 * e21 * e21) * (3.0 + 4.0 * ecc1 * ecc1)) * (eccanom1 - m1) +
					0.25 * e11  * (3.0 * (5.0 * e21 * e21 - 1.0) * (4.0 + 21.0 * ecc1 * ecc1 + 3.0 * pow(ecc1, 4)) + 5.0 * (e11 * e11 - 3.0 * e21 * e21) * (3.0 + 21.0 * ecc1 * ecc1 + 4.0 * ecc1 * ecc1)) * sin(eccanom1) -
					0.25 * ecc1  * e11  * (3.0 * (5.0 * e21 * e21 - 1.0) * (3.0 + 4.0 * ecc1 * ecc1) + 5.0 * (e11 * e11 - 3.0 * e21 * e21) * (4.0 + 3.0 * ecc1 * ecc1)) * sin(2.0 * eccanom1) +
					1.0 / 12.0 * e11  * (3.0 * (5.0 * e21 * e21 - 1.0) * (3.0 * ecc1 * ecc1 + pow(ecc1, 4)) + 5.0 * (e11 * e11 - 3.0 * e21 * e21) * (1.0 + 3.0 * ecc1 * ecc1)) * sin(3.0 * eccanom1) -
					1.0 / 32.0 * ecc1  * e11  * (3.0 * (5.0 * e21 * e21 - 1.0) * ecc1 * ecc1 + 5.0 * (e11 * e11 - 3.0 * e21 * e21)) * sin(4.0 * eccanom1) -
					0.25 * beta1  * e21  * (5.0 * (3.0 * e11 * e11 - e21 * e21) * (1.0 + 6.0 * ecc1 * ecc1) + (5.0 * e21 * e21 - 3.0) * (4.0 + 3.0 * ecc1 * ecc1)) * cos(eccanom1) +
					1.0 / 8.0 * beta1  * ecc1  * e21  * (5.0 * (3.0 * e11 * e11 - e21 * e21) * (5.0 + 2.0 * ecc1 * ecc1) + (5.0 * e21 * e21 - 3.0) * (6.0 + ecc1 * ecc1)) * cos(2.0 * eccanom1) -
					1.0 / 12.0 * beta1  * e21  * (5.0 * (3.0 * e11 * e11 - e21 * e21) * (1.0 + 2.0 * ecc1 * ecc1) + (5.0 * e21 * e21 - 3.0) * 3.0 * ecc1 * ecc1) * cos(3.0 * eccanom1) +
					1.0 / 32.0 * beta1  * ecc1  * e21  * (5.0 * (3.0 * e11 * e11 - e21 * e21) + (5.0 * e21 * e21 - 3.0) * ecc1 * ecc1) * cos(4.0 * eccanom1));
				dmdtp = -beta1  * (dargpdtp + draandtp * cosi1) + x4;

				// sum periodic terms
				if (ithird == 1)
				{
					dndtps = dndtp;  // sun periodic
					dedtps = dedtp;
					didtps = didtp;
					draandtps = draandtp;
					dargpdtps = dargpdtp;
					dmdtps = dmdtp;
				}
				else
				{
					dndtpm = dndtp;  // moon periodic
					dedtpm = dedtp;
					didtpm = didtp;
					draandtpm = draandtp;
					dargpdtpm = dargpdtp;
					dmdtpm = dmdtp;
				}

			}  // for ithird through periodic terms

			//    printf('changes_from_3bdy_(deg/day)_dedtdidtsec %12.9f %12.9f %12.9f %12.9f dedtdidtper %11.8f  %11.8f %11.8f  %11.8f   \n',  
			//               dedtss * 86400.0, didtss * rad * 86400.0, dedtsm * 86400.0, didtsm * rad * 86400.0,  
			//               dedtps * 86400.0, didtps * rad * 86400.0, dedtpm * 86400.0, didtpm * rad * 86400.0);

			// sum the third body periodic effects
			dndt3p = dndtps + dndtpm;
			//dadt3p = -2.0 / 3.0 * a * dndt3p / n;
			dedt3p = dedtps + dedtpm;
			didt3p = didtps + didtpm;
			draandt3p = draandtps + draandtpm;
			dargpdt3p = dargpdtps + dargpdtpm;
			dmdt3p = dmdtps + dmdtpm;

//			if (show == 'y')
//				printf("3eq_peri %11.7f %11.7f %11.7f %11.7f %11.7f %11.7f \n", dndt3p, dedt3p, didt3p, draandt3p, dargpdt3p, dmdt3p);

			// find the osculating contribution to add to the sec / lp values
			temp = 1.0 / (1.0 + cosi1);
			//dela = -2.0 / 3.0 * a * dndtp / n;  // km / s s
			deln = dndt3p;  // -1.5 * n / a * dadt3p; so just dndt3p
			// rest are unitless
			// account for small ecc, incl
			temp1 = x3 + ecc1  * sini1 * draandt3p * sini1 * temp;
			//delaf = dedt3p * cos(argp + raan) - ecc1  * (dargpdt3p + draandt3p) * sin(argp + raan);
			delaf = dedt3p * cos(argp1 + raan1) - temp1 * sin(argp1 + raan1);
			//delag = dedt3p * sin(argp + raan) + ecc1  * (dargpdt3p + draandt3p) * cos(argp + raan);
			delag = dedt3p * sin(argp1 + raan1) + temp1 * cos(argp1 + raan1);
			delchi = didt3p * sinraan1 * temp + sini1  * draandt3p * cosraan1 * temp;
			delpsi = didt3p * cosraan1 * temp - sini1  * draandt3p * sinraan1 * temp;
			//delmlonm = dmdt3p + dargpdt3p + draandt3p;
			delmlonm = x4 + x3 * ecc1 / (1.0 + beta1) + sini1 * draandt3p * sini1 * temp;

			// ---------- - updated osculating equinoctial elements---------------- -
			temp = 1.0;
			nosc = n1 + temp * deln;
			aosc = pow(mu / (nosc * nosc), (1.0 / 3.0));
			afosc = afmean + temp * delaf;
			agosc = agmean + temp * delag;
			chiosc = chimean + temp * delchi;
			psiosc = psimean + temp * delpsi;
			mlonmosc = mlonmmean + temp * delmlonm; // +n1 * dtsec;

	        // adjust mean estimates based on osculating values
			temp = -1.0;  // use lambda 0.3 per Cain paper ?
			amean = amean + temp * (aosc - aosco);
			afmean = afmean + temp * (afosc - afosco);
			agmean = agmean + temp * (agosc - agosco);
			chimean = chimean + temp * (chiosc - chiosco);
			psimean = psimean + temp * (psiosc - psiosco);
			mlonmmean = mlonmmean + temp * (mlonmosc - mlonmosco);
			mlonmmean = fmod(mlonmmean, twopi);

			ast2Body::eq2rv(amean, afmean, agmean, chimean, psimean, mlonmmean, fr, rmean1, vmean1);
			for (int j = 0; j < 3; j++)
				dr[j] = rcurr[j] - rmean1[j];
			magdr = astMath::mag(dr);
			printf("%5i %11.7f %11.7f %11.7f %11.7f %11.7f %11.7f %11.7f \n",
				ii, aosc - aosco, afosc - afosco, agosc - agosco, chiosc - chiosco, psiosc - psiosco, mlonmosc - mlonmosco, magdr);

			ii = ii + 1;
		}   // while

		ast2Body::eq2rv(amean, afmean, agmean, chimean, psimean, mlonmmean, fr, rmean, vmean);
		printf("vectors mean  %14.8f %14.8f %14.8f %14.9f %14.9f %14.9f \n", rmean[0], rmean[1], rmean[2], vmean[0], vmean[1], vmean[2]);
		printf("rmean = [ %14.8f, %14.8f, %14.8f]; \nv = [ %14.9f, %14.9f, %14.9f]; \n", rmean[0], rmean[1], rmean[2], vmean[0], vmean[1], vmean[2]);
		ast2Body::rv2coe(rmean, vmean, mu, pmean, amean, eccmean, inclmean, raanmean, argpmean, numean, mmean, eccanommean, arglatmean, truelonmean, lonpermean);
		printf("coes mean  %11.4f %11.4f %13.9f %13.7f %11.5f %11.5f %11.5f %11.5f %11.5f \n",
			pmean, amean, eccmean, inclmean * rad, raanmean * rad, argpmean * rad, numean * rad, mmean * rad, arglatmean * rad);
	}  // itermeanosc3body


	/*  ------------------------------------------------------------------------------
	 * 
	 *                                 procedure itermeanoscSRP
	 * 
	 *  This function iterates on the secular/periodic equations to convert between osculating 
	 *   and mean elements. it only considers solar radiation pressure.  
	 * 
	 *   author        : david vallado                  719-573-2600   11 jan 2018
	 * 
	 *   inputs          description                    range / units
	 *     jdutc       - epoch julian date              days from 4713 BC
	 *     jdutcF      - epoch julian date fraction     day fraction from jdutc
	 *     dat         - number of leap seconds         s
	 *     dut1        - delta between utc and ut1      s
	 *     rosc        - osculating position vector     km
	 *     vosc        - osculating velocity vector     km/s
     *     cram        - srp coeff cr * a / m           m^2/kg
	 *     interp      - interpolationg for sun/moon    'l', 's'
	 *     jpldearr    - array of Sun and Moon position vectors from de430
	 *     jdjpldestart- jd of start of array of de vectors days frm 4713 BC
	 * 
	 *   outputs       :
	 *     rmean       - mean position vector           km
	 *     vmean       - mean velocity vector           km/s
	 * 
	 *   locals        :
	 *     r3          - Cube of r
	 * 
	 *   coupling      :
	 *     astTime:: jday
	 *     ast2Body:: rv2eq, eq2rv, rv2coe, sunmoonjpl
	 * 
	 *   references    :
	 *     chao, hoots
	-----------------------------------------------------------------------------  */

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
		)
	{
		double dtsec, psrp, betas, fsrp, rsunkm, asun;
		double jdut1, jdut1f, tut1, jdtt, jdttf, ttt, aosc, nosc, afosc, agosc, chiosc, psiosc, mlonmosc, mlonnuosc, ttdb, 
			posc, eccosc, inclosc, raanosc, argposc, nuosc, mosc, eccanomosc, arglatosc, truelonosc, lonperosc, 
			pcurr, acurr, ecccurr, inclcurr, raancurr, argpcurr, nucurr, mcurr, eccanomcurr, arglatcurr, trueloncurr, lonpercurr;
		double cosa51, sina51, cosa61, sina61, cosoblh1, sinoblh1, c11, c21, c31, c41, c51, c61, c71, c81, d11, d21, d31, d41;
		double amean, afmean, agmean, chimean, psimean, mlonmmean, pmean, eccmean, inclmean, raanmean, argpmean, numean, mmean, 
			eccanommean, arglatmean, truelonmean, lonpermean;
		double aosco, afosco, agosco, chiosco, psiosco, mlonmosco;
		double dndtp, dedtp, didtp, draandtp, x7, dargpdtp, x8, temp, dmdtp, dela, delaf, delag, temp1, delchi, delpsi, delmlonm;
		int fr;
		double rcurr[3], vcurr[3];
		double rsun[3], rtascs, decls, rmoon[3], rtascm, declm;
		double rmean1[3], vmean1[3], dr[3], magdr;

		double rad = 180.0 / pi;
		double dayconv = 1.0 / 86400.0;
		double jyrconv = 1.0 / 36525.0;

		// evaluate at epoch time only
		dtsec = 0.0;

		rsunkm = 149597870.7; // km
		asun = 149598023.0; // km
		psrp = 4.56e-9;  // kgm / s2m2
		betas = 1.0;  // 0.0 to 1.0
		fsrp = cram * psrp * pow(asun / rsunkm, 2.0);  // m2 / kg kgm / s2m2->km / s2

        // either find these, or fix so the same as calling
		jdut1 = jdutc;
		jdut1f = jdutcF + dut1 * dayconv;
		tut1 = (jdut1 + jdut1f - 2451545.0) * jyrconv;
		jdtt = jdutc;
		jdttf = jdutcF + (dat + 32.184) * dayconv;
		ttt = (jdtt + jdttf - 2451545.0) * jyrconv;
		printf("osculating   %11.6f %11.6f %11.6f %11.6f %11.6f %11.6f \n", rosc[0], rosc[1], rosc[2], vosc[0], vosc[1], vosc[2]);

		ast2Body::rv2eq(rosc, vosc, aosc, nosc, afosc, agosc, chiosc, psiosc, mlonmosc, mlonnuosc, fr);
		mlonmosc = fmod(mlonmosc, twopi);
		printf("eq osc  %12.9f %12.9f %12.9f %12.9f %12.9f %12.9f \n", aosc, afosc, agosc, chiosc, psiosc, mlonmosc * rad);

//		if (opt == 'j')
		{
			//jpl ephemerides - just do once for sun and moon
			// let tt approx tdb
			ast2Body::sunmoonjpl(jdtt, jdttf, interp, jpldearr, jdjpldestart, rsun, rtascs, decls, rmoon, rtascm, declm);
//			r3mag = astMath::mag(r3);
			//printf("rsunmoon  //lf //lf //lf //lf //lf //lf \n", r3[0], r3[1], r3[2], rmoon[0], rmoon[1], rmoon[2]);
		}

		// find osc coes
		ast2Body::rv2coe(rosc, vosc, mu, posc, aosc, eccosc, inclosc, raanosc, argposc, nuosc, mosc, eccanomosc, arglatosc, truelonosc, lonperosc);
		printf("coes osc   %11.4f %11.4f %13.9f %13.7f %11.5f %11.5f %11.5f %11.5f %11.5f \n", 
			posc, aosc, eccosc, inclosc * rad, raanosc * rad, argposc * rad, nuosc * rad, mosc * rad, arglatosc * rad);

		// set initial guess for mean elements
		// use as initial guess for mean
		amean = aosc;
		afmean = afosc;
		agmean = agosc;
		chimean = chiosc;
		psimean = psiosc;
		mlonmmean = mlonmosc;

		// store originals for residual calcs later
		aosco = aosc;
		afosco = afosc;
		agosco = agosc;
		chiosco = chiosc;
		psiosco = psiosc;
		mlonmosco = mlonmosc;

		// --------------------------iterate to find mean elements----------------------- 
		// set initial to get into loop
		magdr = 1000.0;
		int i = 0;
		printf("begin iterating\n");
		while (i <= 40 && magdr > 0.010)
		{
			ast2Body::eq2rv(amean, afmean, agmean, chimean, psimean, mlonmmean, fr, rcurr, vcurr);
			ast2Body::rv2coe(rcurr, vcurr, mu, pcurr, acurr, ecccurr, inclcurr, raancurr, argpcurr, nucurr, mcurr, eccanomcurr, arglatcurr, trueloncurr, lonpercurr);
			double cosi1, sini1, beta1, ncurr, obliquity1, meanlonsun1, meanlong1, meananomaly1, eclplong1, cosa11, sina11, cosa21, sina21, cosa31;
			double sina31, cosa41, sina41;
			cosi1 = cos(inclcurr);
			sini1 = sin(inclcurr);
			beta1 = sqrt(1.0 - ecccurr * ecccurr);
			ncurr = sqrt(mu / pow(acurr, 3));

			//tut1 = (jdut1 - 2451545.0) *jyrconv;
			tut1 = (tut1 * 36525.0 + dtsec * dayconv) * jyrconv;
			//tut1 = tut1 + dtsec /
			obliquity1 = (23.439291 - 0.0130042 * tut1) / rad;
			meanlonsun1 = (280.4606184 + 36000.77005361 * tut1) / rad;  // rad
			// mean longitude seems to be better, but TOR says ecliptic longiude is the term

			meanlong1 = 280.460 + 36000.77 * tut1;
			meanlong1 = fmod(meanlong1, 360.0);  //deg, rem ? ?
			ttdb = tut1;
			meananomaly1 = 357.5277233 + 35999.05034 * ttdb;
			meananomaly1 = fmod(meananomaly1 / rad, twopi);  //rad
			if (meananomaly1 < 0.0)
				meananomaly1 = twopi + meananomaly1;

			eclplong1 = meanlong1 + 1.914666471 * sin(meananomaly1) + 0.019994643 * sin(2.0 * meananomaly1); //deg
			eclplong1 = fmod(eclplong1, 360.0) / rad;  //rad

			//   these angular values could be enhanced by using JPL DE ephemerides??


			cosa11 = cos(eclplong1 - argpcurr - raancurr);
			sina11 = sin(eclplong1 - argpcurr - raancurr);
			cosa21 = cos(eclplong1 - argpcurr + raancurr);
			sina21 = sin(eclplong1 - argpcurr + raancurr);
			cosa31 = cos(eclplong1 - argpcurr);
			sina31 = sin(eclplong1 - argpcurr);
			cosa41 = cos(eclplong1 + argpcurr);
			sina41 = sin(eclplong1 + argpcurr);
			cosa51 = cos(eclplong1 + argpcurr - raancurr);
			sina51 = sin(eclplong1 + argpcurr - raancurr);
			cosa61 = cos(eclplong1 + argpcurr + raancurr);
			sina61 = sin(eclplong1 + argpcurr + raancurr);

			cosoblh1 = cos(obliquity1 * 0.5);
			sinoblh1 = sin(obliquity1 *  0.5);
			c11 = cos(inclcurr * 0.5) * cos(inclcurr * 0.5) * cosoblh1 * cosoblh1;
			c21 = sin(inclcurr * 0.5) * sin(inclcurr * 0.5) * sinoblh1 * sinoblh1;
			c31 = 0.5 * sini1 * sin(obliquity1);
			c41 = -sin(inclcurr * 0.5) * sin(inclcurr * 0.5) * cosoblh1 * cosoblh1;
			c51 = -cos(inclcurr * 0.5) * cos(inclcurr * 0.5) * sinoblh1 * sinoblh1;
			c61 = sini1 * cosoblh1 * sini1 * cosoblh1;
			c71 = sini1 * sinoblh1 * sini1 * sinoblh1;
			c81 = cosi1 * sin(obliquity1);

			d11 = c11 * cosa11 + c21 * cosa21 + c31 * cosa31 - c31 * cosa41 - c41 * cosa51 - c51 * cosa61;
			d21 = c11 * sina11 + c21 * sina21 + c31 * sina31 + c31 * sina41 + c41 * sina51 + c51 * sina61;
			d31 = 0.5 * (c61 * cosa11 - c61 * cosa51 + c71 * cosa61 - c71 * cosa21 + c81 * cosa41 - c81 * cosa31);
			d41 = 0.5 * (c61 * sina11 + c61 * sina51 - c71 * sina61 - c71 * sina21 - c81 * sina41 - c81 * cosa31);

			// ----------------------------------periodic equations----------------------------
			// calc classical periodic values
			dndtp = 3.0 / (ncurr * acurr) * fsrp * (d11 * cos(eccanomcurr) + d21 * beta1 * sin(eccanomcurr));   // s / km km / s2 -> / s
			//fprintf(1, ' eccanom %11.7f dela %11.7f m %11.7f m1 %11.7f \n', eccanom * rad, -2.0 / 3.0 * a * dndtp / n, m * rad, m1 * rad);
			// rest are unitless
			dedtp = -beta1 / (ncurr * ncurr * acurr) * fsrp * (d21 * (0.25 * sin(2.0 * eccanomcurr)) - 0.5 * d21 * ecccurr * sin(eccanomcurr) + 0.25 * d11 * cos(2.0 * eccanomcurr));
			didtp = 1.0 / (ncurr * ncurr * acurr * beta1 * sini1) * fsrp * (d41 * ((1.0 - 0.5 * ecccurr * ecccurr) * sin(eccanomcurr) - 0.25 * ecccurr * sin(2.0 * eccanomcurr)) - 
				d31 * (-cos(eccanomcurr) + 0.25 * ecccurr * cos(2.0 * eccanomcurr)));
			draandtp = 1.0 / (ncurr * ncurr * acurr * beta1 * sini1) * fsrp * (d41 * beta1 * (-cos(eccanomcurr) + 0.25 * ecccurr * cos(2.0 * eccanomcurr)) + 
				d31 * ((1.0 - 0.5 * ecccurr * ecccurr) * sin(eccanomcurr) - 0.25 * ecccurr * sin(2.0 * eccanomcurr)));
			x7 = beta1 / (ncurr * ncurr * acurr) * fsrp * (d11 * (0.5 * ecccurr * sin(eccanomcurr) - 0.25 * sin(2.0 * eccanomcurr)) + 
				1.0 / beta1 * d21 * (0.25 * cos(2.0 * eccanomcurr) - ecccurr * cos(eccanomcurr)));
			dargpdtp = -draandtp * cosi1 + 1.0 / ecccurr * x7;
			x8 = 5.0 / (ncurr * ncurr * acurr) * fsrp * d11 * ((1.0 - 0.5 * ecccurr * ecccurr) * sin(eccanomcurr) - 0.25 * ecccurr * sin(2.0 * eccanomcurr)) +
				5.0 / (ncurr * ncurr * acurr) * fsrp * d21 * beta1 * (-cos(eccanomcurr) + 0.25 * ecccurr * cos(2.0 * eccanomcurr));
			dmdtp = x8 - beta1 * (dargpdtp + draandtp * cosi1);
			//   fprintf(1, 'eq_peri %12.9f %12.9f %12.9f %12.9f %12.9f %12.9f \n', dndtp, dedtp, didtp, draandtp, dargpdtp, dmdtp);

			// -------------  periodic equations in equinoctial space----------------
			// find the osculating contribution to add to the mean sec / lp values in equinoctial space
			temp = 1.0 / (1.0 + cosi1);
			dela = -2.0 / 3.0 * acurr * dndtp / ncurr;  // km / s s
			// rest are unitless
			// account for small ecc, incl
			temp1 = x7 + ecccurr * sini1 * draandtp * sini1 * temp;
			//delaf = dedtp * cos(argp + raan) - ecc1 * (dargpdtp + draandtp) * sin(argp + raan);
			delaf = dedtp * cos(argpcurr + raancurr) - temp1 * sin(argpcurr + raancurr);
			//delag = dedtp * sin(argp + raan) + ecc1 * (dargpdtp + draandtp) * cos(argp + raan);
			delag = dedtp * sin(argpcurr + raancurr) + temp1 * cos(argpcurr + raancurr);
			delchi = didtp * sin(raancurr) * temp + sini1 * draandtp * cos(raancurr) * temp;
			delpsi = didtp * cos(raancurr) * temp - sini1 * draandtp * sin(raancurr) * temp;
			//delmlonm = dmdtp + dargpdtp + draandtp;
			delmlonm = x8 + x7 * ecccurr / (1.0 + beta1) + sini1 * draandtp * sini1 * temp;
//			printf("del eq %12.9f %12.9f %12.9f %12.9f %12.9f %12.9f \n", dela, delaf, delag, delchi, delpsi, delmlonm);

			// -----------  updated osculating equinoctial elements--------------------
			temp = 1.0;
			aosc = amean + temp * dela;
			afosc = afmean + temp * delaf;
			agosc = agmean + temp * delag;
			chiosc = chimean + temp * delchi;
			psiosc = psimean + temp * delpsi;
			mlonmosc = mlonmmean + temp * delmlonm;
			mlonmosc = fmod(mlonmosc, twopi);

			// adjust mean values based on osculating guesses
			temp = -1.0;  // use lambda 0.3 per Cain paper ?
			amean = amean + temp * (aosc - aosco);
			afmean = afmean + temp * (afosc - afosco);
			agmean = agmean + temp * (agosc - agosco);
			chimean = chimean + temp * (chiosc - chiosco);
			psimean = psimean + temp * (psiosc - psiosco);
			mlonmmean = mlonmmean + temp * (mlonmosc - mlonmosco);
			mlonmmean = fmod(mlonmmean, twopi);

			ast2Body::eq2rv(amean, afmean, agmean, chimean, psimean, mlonmmean, fr, rmean1, vmean1);
			for (int j = 0; j < 3; j++)
				dr[j] = rcurr[j] - rmean1[j];
			magdr = astMath::mag(dr);
			printf("%5i %11.7f %11.7f %11.7f %11.7f %11.7f %11.7f %11.7f \n", 
				i, aosc - aosco, afosc - afosco, agosc - agosco, chiosc - chiosco, psiosc - psiosco, mlonmosc - mlonmosco, magdr);

			i = i + 1;
		}   // while

		ast2Body::eq2rv(amean, afmean, agmean, chimean, psimean, mlonmmean, fr, rmean, vmean);
		printf("vectors mean  %14.8f %14.8f %14.8f %14.9f %14.9f %14.9f \n", rmean[0], rmean[1], rmean[2], vmean[0], vmean[1], vmean[2]);
		printf("rmean = [ %14.8f, %14.8f, %14.8f]; \nv = [ %14.9f, %14.9f, %14.9f]; \n", rmean[0], rmean[1], rmean[2], vmean[0], vmean[1], vmean[2]);
		ast2Body::rv2coe(rmean, vmean, mu, pmean, amean, eccmean, inclmean, raanmean, argpmean, numean, mmean, eccanommean, arglatmean, truelonmean, lonpermean);
		printf("coes mean  %11.4f %11.4f %13.9f %13.7f %11.5f %11.5f %11.5f %11.5f %11.5f \n",
			pmean, amean, eccmean, inclmean * rad, raanmean * rad, argpmean * rad, numean * rad, mmean * rad, arglatmean * rad);

		//// ----------------check out how close it is
		//year = 2017;
		//mon = 5;
		//day = 11;
		//hr = 3;
		//minute = 51;
		//second = 42.7657;
		//dat = 37;
		//dut1 = 0.4072908;  // note that c# will interpolate this, so values will differ
		//astTime::jday(year, mon, day, hr, minute, second + dut1, jdut1, jdut1);
		//tut1 = (jdut1 - 2451545.0) * jyrconv;

		//jdepoch = jdut1 + jdut1f;
		//dtsec = 0.0;
		//ast2Body::kepler(rosc, vosc, dtsec, r1, v1);
		//// sun
		//ast2Body::sun(jdut1, jdut1f, rsun, rtascs, decls);  // in au
		//for (i = 0; i < 3; i++)
		//    rsun[i] = rsun[i] * rsunkm;  // in km

		//printf("\n\n2body %5i  %11.6f %11.6f %11.6f %11.6f %11.6f %11.6f \n", day, r1[0], r1[1], r1[2], v1[0], v1[1], v1[2]);

		//astPert::srpperts(jdut1, jdut1f, rmean, vmean, 'l', 'j', 'y', dtsec, jpldearr, jdjpldestart, tut1, cram, r11, v11);  // fullsun 'y'

		//printf("srp  %5i     %11.6f %11.6f %11.6f %11.6f %11.6f %11.6f \n", day, r11[0], r11[1], r11[2], v11[0], v11[1], v11[2]);
		//for (i = 0; i < 3; i++)
		//	dr[i] = rmean[i] - r11[i];
		//printf("srp dr %5i     %11.6f %11.6f %11.6f    %11.6f \n", day, dr[0], dr[1], dr[2], astMath::mag(dr));
	}  // itermeanoscSRP



	/*  ------------------------------------------------------------------------------
	 * 
	 *                            procedure deriv
	 * 
	 *  This function calculates the derivative of the two-body state vector for
	 *  use with the Runge-Kutta algorithm.
	 * 
	 *   author        : david vallado                  719-573-2600   11 jan 2018
	 * 
	 *   inputs          description                    range / units
	 *     Xeci        - epoch eci state vector         km, km/s
	 * 
	 *   outputs       :
	 *     Xdoteci     - epoch eci state vector derivative km/s, km/s2
	 * 
	 *   locals        :
	 *     r3          - Cube of r
	 * 
	 *   coupling      :
	 *     none
	 * 
	 *   references    :
	 *     vallado
	-----------------------------------------------------------------------------  */

	void deriv
		(
		std::vector< std::vector<double> > Xeci,
		std::vector< std::vector<double> > &Xdoteci
		)
	{
		double rmag3, rmag;
		Xeci.resize(7);  // rows
		for (std::vector< std::vector<double> >::iterator it = Xeci.begin(); it != Xeci.end(); ++it)
			it->resize(7);
		Xdoteci.resize(7);  // rows
		for (std::vector< std::vector<double> >::iterator it = Xdoteci.begin(); it != Xdoteci.end(); ++it)
			it->resize(7);
		double temp;

		rmag = sqrt(Xeci[0][0] * Xeci[0][0] + Xeci[1][0] * Xeci[1][0] + Xeci[2][0] * Xeci[2][0]);
		rmag3 = rmag * rmag * rmag;

		// --------------------Velocity Terms------------------------ 
		Xdoteci[0][0] = Xeci[3][0];
		Xdoteci[1][0] = Xeci[4][0];
		Xdoteci[2][0] = Xeci[5][0];

		// ------------------Acceleration Terms---------------------- 
		temp = mu / rmag3;
		Xdoteci[3][0] = -Xeci[0][0] * temp;
		Xdoteci[4][0] = -Xeci[1][0] * temp;
		Xdoteci[5][0] = -Xeci[2][0] * temp;
	}   // deriv


	/*  ------------------------------------------------------------------------------
	 * 
	 *                            procedure rk4
	 * 
	 *  This function is a fourth order Runge-Kutta integrator for a 7 dimension
	 *  1st order differential equation. note that the eci-ecef conversion can be
	 *  done in pderiv each tie, or once per rk4 call, or some other approach. currently, 
	 *  it's set up to perform the conversion once per rk4 call, and then pass the 
	 *  transformation matrices to pderiv. 
	 * 
	 *   author        : david vallado                  719-573-2600   11 jan 2018
	 * 
	 *   inputs          description                    range / units
	 *     jdutc       - epoch julian date              days from 4713 BC
	 *     jdutcF      - epoch julian date fraction     day fraction from jdutc
	 *     dtsec       - Step size                      sec
	 *     Xeci        - epoch eci state vector         km, km/s
	 *  use an integer because msvs c++ cannot do strings reliably as formal parameters
	 *     derivType = 1 = 2-body
	 *     derivType = 2 = J2
	 *     derivType = 3 = J3
	 *     derivType = 4 = J4
	 *     derivType = 5 = Jx
	 *     derivType = 6 = drag
	 *     derivType = 7 = 3-body
	 *     derivType = 8 = srp
	 *     derivType = 10 = all
	 *     cdam        - cd * a / mass                    m2/kg
	 *     cram        - cr * a / mass                    m2/kg
	 *     interp      - interpolation for sun/moon vectors 'l', 's'
	 *     opt         - method of calc sun/moon        'a', 'j'
	 *     jpldearr    - array of Sun and Moon position vectors from de430
	 *     jdjpldestart- jd of start of array of de vectors days frm 4713 BC
	 *     iau80rec    - array of iau coefficients
	 *     jdeopstart  - start jd for eop data
	 *     eopearr     - array of EOP data
	 *
	 *   outputs       :
	 *     Xfeci          - final eci state vector           km, km/s
	 * 
	 *   locals        :
	 *     eop params - ttt, jdut1, jdut1f, lod, xp, yp, ddpsi, ddeps, deltapsi, deltaeps,
	 *     Xdoteci     - derivative of State Vector
	 *     rsun        - sun vector ecef                  km
	 *     rmoon       - moon vector ecef                 km
	 *     r3          - Cube of r
	 *     K           - Storage for values of state vector at different times
	 *     Temp        - Storage for state vector
	 *     TempTime    - Temporary time storage halfway between dtsec             sec
	 *     DtTU        - Step size                      s
	 * 
	 *   coupling      :
	 *     deriv       - derivatives of EOM
	 *     pderiv      - perturbed derivatives of EOM
	 * 
	 *   references    :
	 *     vallado       2013, 526
	-----------------------------------------------------------------------------  */

	void rk4
		(
		double jdutc,
		double jdutcF,
		double dtsec,
		std::vector< std::vector<double> > Xeci,
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
		)
	{
		Xeci.resize(7);  // rows
		for (std::vector< std::vector<double> >::iterator it = Xeci.begin(); it != Xeci.end(); ++it)
			it->resize(1);
		Xfeci.resize(7);  // rows
		for (std::vector< std::vector<double> >::iterator it = Xfeci.begin(); it != Xfeci.end(); ++it)
			it->resize(1);
        // locals
		int i, j, dat;
		double dut1, lod, xp, yp, ddpsi, ddeps, dx, dy, icrsx, y, s, deltapsi, deltaeps;
		double tut1, jdut1, jdut1f, ttt, jdtt, jdttf, jdtdb, jdtdbF;
		double tempjd, tempjdF;
		double rsun[3], rsmag, rtascs, decls, rmoon[3], rtascm, declm;
		double reci[3], veci[3], aeci[3], aecef[3];
		std::vector< std::vector<double> > Xdoteci = std::vector< std::vector<double> >(7, std::vector<double>(1, 0.0));
		std::vector< std::vector<double> > K = std::vector< std::vector<double> >(7, std::vector<double>(4, 0.0));
		std::vector< std::vector<double> > Temp = std::vector< std::vector<double> >(7, std::vector<double>(1, 0.0));
		std::vector< std::vector<double> > Xecef = std::vector< std::vector<double> >(7, std::vector<double>(1, 0.0));
		std::vector< std::vector<double> > Xfecef = std::vector< std::vector<double> >(7, std::vector<double>(1, 0.0));
		std::vector< std::vector<double> > Xdotecef = std::vector< std::vector<double> >(7, std::vector<double>(1, 0.0));

		// ------- convert from eci to ecef before each pderiv step --------
		int timezone = 0;
		double deg2rad = pi / 180.0;

		double dayconv = 1.0 / 86400.0;
		double jdyconv = 1.0 / 36525.0;

		// base jd stays the same
		jdut1 = jdutc;
		jdtt = jdutc;
		
		// initialize and set the accelerations
		for (i = 0; i < 3; i++)
		{
			reci[i] = Xeci[i][0];
			veci[i] = Xeci[i + 3][0];
			aeci[i] = 0.0;
			aecef[i] = 0.0;
		}

		// check for zero propagate time
		if (fabs(dtsec) < 0.000001)
		{
			for (j = 0; j < 6; j++)
				Xfeci[j][0] = Xeci[j][0];
		}
		else
		{
//int start_s = clock();

			// calculate eop once per rk4 call (current), or before each pderiv call, or less often otuside rk4?
			EopSpw::findeopparam(jdutc, jdutcF, 'l', 'f', eoparr, jdeopstart, dut1, dat, lod, xp, yp, ddpsi, ddeps, dx, dy, icrsx, y, s, deltapsi, deltaeps);
			jdut1 = jdutc;
			jdut1f = jdutcF + dut1 * dayconv;
			tut1 = (jdut1 + jdut1f - 2451545.0) * jdyconv;
			jdtt = jdutc;
			jdttf = jdutcF + (dat + 32.184) * dayconv;   // sec
			ttt = (jdtt + jdttf - 2451545.0) * jdyconv;
			jdtdb = jdtt;   // set approx equal
			jdtdbF = jdttf;
			//int stop_s = clock();
//printf("timeeop rk4: %11.6f \n", (stop_s - start_s) / double(CLOCKS_PER_SEC) * 1000);
//start_s = clock();

			// perform one time (per step) eci to ecef - save matrices and use inside pderiv
			double psia, wa, epsa, chia, trueeps, meaneps, omega, thetasa, omegaearth[3];
			std::vector< std::vector<double> > prec = std::vector< std::vector<double> >(3, std::vector<double>(3));
			std::vector< std::vector<double> > nut = std::vector< std::vector<double> >(3, std::vector<double>(3));
			std::vector< std::vector<double> > st = std::vector< std::vector<double> >(3, std::vector<double>(3));
			std::vector< std::vector<double> > stdot = std::vector< std::vector<double> >(3, std::vector<double>(3));
			std::vector< std::vector<double> > pm = std::vector< std::vector<double> >(3, std::vector<double>(3));
			std::vector< std::vector<double> > pmp = std::vector< std::vector<double> >(3, std::vector<double>(3));
			std::vector< std::vector<double> > temp = std::vector< std::vector<double> >(3, std::vector<double>(3));

			std::vector< std::vector<double> > trans = std::vector< std::vector<double> >(3, std::vector<double>(3));
			std::vector< std::vector<double> > transp = std::vector< std::vector<double> >(3, std::vector<double>(3));
			std::vector< std::vector<double> > transf = std::vector< std::vector<double> >(3, std::vector<double>(3));

			// ---- find matrices once per call
			coordFK5::precess(ttt, e80, psia, wa, epsa, chia, prec);
			coordFK5::nutation(ttt, ddpsi, ddeps, iau80rec, deltapsi, deltaeps, trueeps, meaneps, omega, nut);
			coordFK5::sidereal(jdut1, deltapsi, meaneps, omega, lod, 2, st, stdot);
			coordFK5::polarm(xp, yp, ttt, e80, pm);

			// ---- perform transformations
			thetasa = 7.29211514670698e-05 * (1.0 - lod * dayconv);
			omegaearth[0] = 0.0;
			omegaearth[1] = 0.0;
			omegaearth[2] = thetasa;

			astMath::matmult(prec, nut, temp, 3, 3, 3);
			astMath::matmult(temp, st, trans, 3, 3, 3);
			astMath::mattrans(trans, transp, 3, 3);
    		astMath::mattrans(pm, pmp, 3, 3);
//stop_s = clock();
//printf("timecoord rk4: %11.6f \n", (stop_s - start_s) / double(CLOCKS_PER_SEC) * 1000);
//start_s = clock();

			// find sun and moon vectors if needed one time per step
			// note that these results are eci!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			if (derivType == 7 || derivType == 8 || derivType == 10)
			{
				if (opt == 'j')
				{
					ast2Body::findjpldeparam(jdtdb, jdtdbF, interp, jpldearr, jdjpldestart, rsun, rsmag, rmoon);
					//printf("findjpldeephem\n%lf %lf  %lf %lf %lf   %lf %lf %lf \n", jdut1, jdut1f, rsun[0], rsun[1], rsun[2], rmoon[0], rmoon[1], rmoon[2]);
				}
				else
				{
					ast2Body::sun(jdtdb, jdtdbF, rsun, rtascs, decls);  // eci au
					ast2Body::moon(jdtdb, jdtdbF, rmoon, rtascm, declm);  // eci km
					for (int i = 0; i < 3; i++)
						rsun[i] = rsun[i] * magr3s;  // convert to km
				}
				//printf("sun eci    %11.9f %11.9f %11.9f \n", rsun[0], rsun[1], rsun[2]);

				//// make vectors ecef for pderiv
				//double rpef[3];
				//astMath::matvecmult(transp, rsun, rpef);
				//astMath::matvecmult(pmp, rpef, rsunecef);

				//astMath::matmult(pmp, transp, transf, 3, 3, 3);
				////astMath::matvecmult(transf, rsun, rsunecef);
				//astMath::matvecmult(transf, rmoon, rmoonecef);
				//printf("3bdsunecef %11.9f %11.9f %11.9f \n", rsunecef[0], rsunecef[1], rsunecef[2]);
			}

//stop_s = clock();
//printf("timejpl rk4: %11.6f \n", (stop_s - start_s) / double(CLOCKS_PER_SEC) * 1000);

//start_s = clock();
			// ----------------------Initialize Xdoteci ---------------------- 
			// --------------Evaluate 1st Taylor Series Term -------------- 
			if (derivType == 1)
			    deriv(Xeci, Xdoteci);
			else
				pderiv(jdutc, jdutcF, Xeci, rsun, rmoon, derivType, cdam, cram, trans, pm, omegaearth, Xdoteci);
			//printf("1st %11.6f %11.6f %11.6f %11.6f %11.6f %11.6f \n", Xeci[0][0], Xeci[1][0], Xeci[2][0], Xeci[3][0], Xeci[4][0], Xeci[5][0]);
//			printf("1xdt %11.6f %11.6f %11.6f %11.6f %11.6f %11.6f \n", Xdoteci[0][0], Xdoteci[1][0], Xdoteci[2][0], Xdoteci[3][0], Xdoteci[4][0], Xdoteci[5][0]);

			// --------------Evaluate 2nd Taylor Series Term -------------- 
			tempjd = jdutc;
			tempjdF = jdutcF + dtsec * 0.5 * dayconv;
			for (j = 0; j < 6; j++)
			{
				K[j][0] = dtsec *  Xdoteci[j][0];          // s km/s -> km
				Temp[j][0] = Xeci[j][0] + 0.5 *  K[j][0];  // km
			}
			if (derivType == 1)
				deriv(Temp, Xdoteci);
			else
				pderiv(tempjd, tempjdF, Temp, rsun, rmoon, derivType, cdam, cram, trans, pm, omegaearth, Xdoteci);
			//printf("2nd %11.6f %11.6f %11.6f %11.6f %11.6f %11.6f \n", Temp[0][0], Temp[1][0], Temp[2][0], Temp[3][0], Temp[4][0], Temp[5][0]);
			//printf("2xdt %11.6f %11.6f %11.6f %11.6f %11.6f %11.6f \n", Xdoteci[0][0], Xdoteci[1][0], Xdoteci[2][0], Xdoteci[3][0], Xdoteci[4][0], Xdoteci[5][0]);

			// --------------Evaluate 3rd Taylor Series Term --------------  
			for (j = 0; j < 6; j++)
			{
				K[j][1] = dtsec *  Xdoteci[j][0];
				Temp[j][0] = Xeci[j][0] + 0.5 *  K[j][1];
			}
			if (derivType == 1)
				deriv(Temp, Xdoteci);
			else
				pderiv(tempjd, tempjdF, Temp, rsun, rmoon, derivType, cdam, cram, trans, pm, omegaearth, Xdoteci);
			//printf("3rd %11.6f %11.6f %11.6f %11.6f %11.6f %11.6f \n", Temp[0][0], Temp[1][0], Temp[2][0], Temp[3][0], Temp[4][0], Temp[5][0]);
			//printf("3xdt %11.6f %11.6f %11.6f %11.6f %11.6f %11.6f \n", Xdoteci[0][0], Xdoteci[1][0], Xdoteci[2][0], Xdoteci[3][0], Xdoteci[4][0], Xdoteci[5][0]);

			// --------------Evaluate 4th Taylor Series Term --------------  
			for (j = 0; j < 6; j++)
			{
				K[j][2] = dtsec *  Xdoteci[j][0];
				Temp[j][0] = Xeci[j][0] + K[j][2];
			}
			if (derivType == 1)
				deriv(Temp, Xdoteci);
			else
				pderiv(jdutc, jdutcF + dtsec * dayconv, Temp, rsun, rmoon, derivType, cdam, cram, trans, pm, omegaearth, Xdoteci);
			//printf("4th %11.6f %11.6f %11.6f %11.6f %11.6f %11.6f \n", Temp[0][0], Temp[1][0], Temp[2][0], Temp[3][0], Temp[4][0], Temp[5][0]);
			//printf("4xdt %11.6f %11.6f %11.6f %11.6f %11.6f %11.6f \n", Xdoteci[0][0], Xdoteci[1][0], Xdoteci[2][0], Xdoteci[3][0], Xdoteci[4][0], Xdoteci[5][0]);

			// -------  Update the State vector, perform integration---------  
			// ------- all eci
			for (j = 0; j < 6; j++)
				Xfeci[j][0] = Xeci[j][0] + (K[j][0] + 2.0 * (K[j][1] + K[j][2]) + dtsec *  Xdoteci[j][0]) / 6.0;

//stop_s = clock();
//printf("time2nd rk4: %11.6f \n", (stop_s - start_s) / double(CLOCKS_PER_SEC) * 1000);
	}
	}  //  rk4 


	/*  ------------------------------------------------------------------------------
	 * 
	 *                            procedure rk45
	 * 
	 *  This function is a fourth order Runge-Kutta-Fehlberg integrator for a 7 dimension
	 *  1st order differential equation.
	 * 
	 *   author        : david vallado                  719-573-2600   11 jan 2018
	 * 
	 *   inputs          description                    range / units
	 *     jdutc       - epoch julian date              days from 4713 BC
	 *     jdutcF      - epoch julian date fraction     day fraction from jdutc
	 *     dtsec       - Step size                      sec
	 *     Xeci           - epoch eci state vector         km, km/s
	 *  use an integer because msvs c++ cannot do strings reliably as formal parameters
	 *     derivType = 1 = 2-body
	 *     derivType = 2 = J2
	 *     derivType = 3 = J3
	 *     derivType = 4 = J4
	 *     derivType = 5 = Jx
	 *     derivType = 6 = drag
	 *     derivType = 7 = 3-body
	 *     derivType = 8 = srp
	 *     derivType = 10 = all
	 *     cdam        - cd * a / mass                    m2/kg
	 *     cram        - cr * a / mass                    m2/kg
	 *     interp      - interpolation for sun/moon vectors 'l', 's'
	 *     opt         - method of calc sun/moon        'a', 'j'
	 *     jpldearr    - array of Sun and Moon position vectors from de430
	 *     jdjpldestart- jd of start of array of de vectors days frm 4713 BC
	 *     iau80rec    - array of iau coefficients
	 *     jdeopstart  - start jd for eop data
	 *     eoparr      - array of eop data
	 * 
	 *   outputs       :
	 *     Xfeci          - final eci state vector         km, km/s
	 * 
	 *   locals        :
	 *     Xdoteci        - derivative of State Vector
	 *     r3          - Cube of r
	 *     K           - Storage for values of state vector at different times
	 *     Temp        - Storage for state vector
	 *     TempTime    - Temporary time storage halfway between dtsec             sec
	 *     DtTU        - Step size                      s
	 * 
	 *   coupling      :
	 *     astTime:: injjday, convtime
	 *     astPert:: deriv, pderiv 
	 *     EopSpw::  findeopparam
	 * 
	 *   references    :
	 *     vallado       2013, 526
	 -----------------------------------------------------------------------------  */

	void rkF45
		(
		double jdutc,
		double jdutcF,
		double dtsec,
		std::vector< std::vector<double> > Xeci,
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
		)
	{
		Xeci.resize(7);  // rows
		for (std::vector< std::vector<double> >::iterator it = Xeci.begin(); it != Xeci.end(); ++it)
			it->resize(1);
		Xfeci.resize(7);  // rows
		for (std::vector< std::vector<double> >::iterator it = Xfeci.begin(); it != Xfeci.end(); ++it)
			it->resize(1);
		// locals
		int i, j, dat;
		double dut1, lod, xp, yp, ddpsi, ddeps, dx, dy, icrsx, y, s, deltapsi, deltaeps;
		double tut1, jdut1, jdut1f, ttt, jdtt, jdttf, jdtdb, jdtdbF;
		double tempjd, tempjdF, dayconv, jdyconv;
		double rsun[3], rsmag, rtascs, decls, rmoon[3], rtascm, declm, rsunecef[3], rmoonecef[3];
		double reci[3], veci[3], aeci[3], aecef[3];
		double  deg2rad;
		std::vector< std::vector<double> > Xdoteci = std::vector< std::vector<double> >(7, std::vector<double>(1, 0.0));
		std::vector< std::vector<double> > K = std::vector< std::vector<double> >(7, std::vector<double>(4, 0.0));
		std::vector< std::vector<double> > Temp = std::vector< std::vector<double> >(7, std::vector<double>(1, 0.0));
		std::vector< std::vector<double> > Xecef = std::vector< std::vector<double> >(7, std::vector<double>(1, 0.0));
		std::vector< std::vector<double> > Xfecef = std::vector< std::vector<double> >(7, std::vector<double>(1, 0.0));
		std::vector< std::vector<double> > Xdotecef = std::vector< std::vector<double> >(7, std::vector<double>(1, 0.0));

		double psia, wa, epsa, chia, trueeps, meaneps, omega, thetasa, omegaearth[3];
		std::vector< std::vector<double> > prec = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > nut = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > st = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > stdot = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > pm = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > pmp = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > temp = std::vector< std::vector<double> >(3, std::vector<double>(3));

		std::vector< std::vector<double> > trans = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > transp = std::vector< std::vector<double> >(3, std::vector<double>(3));
		std::vector< std::vector<double> > transf = std::vector< std::vector<double> >(3, std::vector<double>(3));

		// ------- convert from eci to ecef before each pderiv step --------
		int timezone = 0;
		deg2rad = pi / 180.0;
		double small = 0.000001;
		double ss;
		dayconv = 1.0 / 86400.0;  // pretty sensitive
		jdyconv = 1.0 / 36525.0;

		// base jd stays the same
		jdut1 = jdutc;
		jdtt = jdutc;

		// initialize and set the accelerations
		for (i = 0; i < 3; i++)
		{
			reci[i] = Xeci[i][0];
			veci[i] = Xeci[i + 3][0];
			aeci[i] = 0.0;
			aecef[i] = 0.0;
		}

		// -----------------------  Initialize Xeci DOT---------------------  
		double HMin, HMax, Time, TStop;
		HMin = dtsec / 64.0;
		HMax = dtsec * 64.0;
		Time = jdutc + jdutcF;
		TStop = jdutc + jdutcF + dtsec * dayconv;
		/* 
			CA[1]= 0.2;        CA[5]= 0.2;  CA[2]= 0.3;   CA[6]= 0.075;
			CA[10]= 0.0225;   CA[3]= 0.6;   CA[7]= 0.3;  CA[11]= -0.9;
			CA[8]= -11.0 / 54.0; CA[12]= 2.5; CA[4]= 0.875; CA[9]= 1631.0 / 55296.0;
			CA[13]= 175.0 / 512.0;
			CA[14]= 1.2;         CA[15]= -70.0 / 27.0;       CA[16]= 575.0 / 13824.0;
			CA[17]= 35.0 / 27.0;   CA[18]= 44275.0 / 110592.0; CA[19]= 253.0 / 4096.0;
			C[1]= 37.0 / 379.0;    Cst[1]= 2825.0 / 27648.0;  C[2]= 250.0 / 621.0;   Cst[2]= 18575.0 / 48384.0;
			C[3]= 125.0 / 594.0;   Cst[3]= 13525.0 / 55296.0; C[4]= 0.0;           Cst[4]= 277.0 / 14336.0;
			C[5]= 512.0 / 1771.0;  Cst[5]= 0.25;
    	 */
		int ktr = 1;
		while (Time < TStop)
		{
			// calculate eop once per rk4 call (current), or before each pderiv call, or less often otuside rk4?
			EopSpw::findeopparam(jdutc, jdutcF, 'l', 'f', eoparr, jdeopstart, dut1, dat, lod, xp, yp, ddpsi, ddeps, dx, dy, icrsx, y, s, deltapsi, deltaeps);
			jdut1 = jdutc;
			jdut1f = jdutcF + dut1 * dayconv;
			tut1 = (jdut1 + jdut1f - 2451545.0) * jdyconv;
			jdtt = jdutc;
			jdttf = jdutcF + (dat + 32.184) * dayconv;   // sec
			ttt = (jdtt + jdttf - 2451545.0) * jdyconv;
			jdtdb = jdtt;   // set approx equal
			jdtdbF = jdttf;

			// perform one time (per step) eci to ecef - save matrices and use inside pderiv

			// ---- find matrices once per call
			coordFK5::precess(ttt, e80, psia, wa, epsa, chia, prec);
			coordFK5::nutation(ttt, ddpsi, ddeps, iau80rec, deltapsi, deltaeps, trueeps, meaneps, omega, nut);
			coordFK5::sidereal(jdut1, deltapsi, meaneps, omega, lod, 2, st, stdot);
			coordFK5::polarm(xp, yp, ttt, e80, pm);

			// ---- perform transformations
			thetasa = 7.29211514670698e-05 * (1.0 - lod * dayconv);
			omegaearth[0] = 0.0;
			omegaearth[1] = 0.0;
			omegaearth[2] = thetasa;

			astMath::matmult(prec, nut, temp, 3, 3, 3);
			astMath::matmult(temp, st, trans, 3, 3, 3);
			astMath::mattrans(trans, transp, 3, 3);
			astMath::mattrans(pm, pmp, 3, 3);

			// find sun and moon vectors if needed one time per step
			if (derivType == 7 || derivType == 8 || derivType == 10)
			{
				if (opt == 'j')
				{
					ast2Body::findjpldeparam(jdtdb, jdtdbF, interp, jpldearr, jdjpldestart, rsun, rsmag, rmoon);
					//printf("findjpldeephem\n%lf %lf  %lf %lf %lf   %lf %lf %lf \n", jdut1, jdut1f, rsun[0], rsun[1], rsun[2], rmoon[0], rmoon[1], rmoon[2]);
				}
				else
				{
					ast2Body::sun(jdtdb, jdtdbF, rsun, rtascs, decls);  // au
					ast2Body::moon(jdtdb, jdtdbF, rmoon, rtascm, declm);  // km
					for (int i = 0; i < 3; i++)
						rsun[i] = rsun[i] * magr3s;  // convert to km
				}

				// make vectors ecef for pderiv
				astMath::matmult(transp, pmp, transf, 3, 3, 3);
				astMath::matvecmult(transf, rsun, rsunecef);
				astMath::matvecmult(transf, rmoon, rmoonecef);
			}

			if (Time + dtsec > TStop) // Make sure you END exactly on the step 
				dtsec = TStop - Time;
			
			// --------------Evaluate 1st Taylor Series Term-----------  
			if (derivType == 1)
				deriv(Xeci, Xdoteci);
			else
				pderiv(jdutc, jdutcF, Xeci, rsunecef, rmoonecef, derivType, cdam, cram, trans, pm, omegaearth, Xdoteci);

			// --------------Evaluate 2nd Taylor Series Term-----------  
			tempjd = jdutc;
			tempjdF = jdutcF + dtsec * 0.25 * dayconv;
			for (j = 0; j < 6; j++)
			{
				K[j][0] = dtsec *  Xdoteci[j][0];  
				Temp[j][0] = Xeci[j][0] + 0.25 *  K[j][0];  
			}
			if (derivType == 1)
				deriv(Temp, Xdoteci);
			else
				pderiv(tempjd, tempjdF, Temp, rsunecef, rmoonecef, derivType, cdam, cram, trans, pm, omegaearth, Xdoteci);

			// --------------Evaluate 3rd Taylor Series Term-----------  
			tempjd = jdutc;
			tempjdF = jdutcF + dtsec * 0.375 * dayconv;
			for (j = 0; j < 6; j++)
			{
				K[j][1] = dtsec *  Xdoteci[j][0];
				Temp[j][0] = Xeci[j][0] + 0.09375 *  K[j][0] + 0.28125 *  K[j][1];
			}
			if (derivType == 1)
				deriv(Temp, Xdoteci);
			else
				pderiv(tempjd, tempjdF, Temp, rsunecef, rmoonecef, derivType, cdam, cram, trans, pm, omegaearth, Xdoteci);

			// --------------Evaluate 4th Taylor Series Term-----------  
			tempjd = jdutc;
			tempjdF = jdutcF + dtsec * (12.0 / 13.0) * dayconv;
			for (j = 0; j < 6; j++)
			{
				K[j][2] = dtsec *  Xdoteci[j][0];
				Temp[j][0] = Xeci[j][0] + K[j][0] * 1932.0 / 2197.0
					- K[j][1] * 7200.0 / 2197.0 + K[j][2] * 7296.0 / 2197.0;
			}
			if (derivType == 1)
				deriv(Temp, Xdoteci);
			else
				pderiv(tempjd, tempjdF, Temp, rsunecef, rmoonecef, derivType, cdam, cram, trans, pm, omegaearth, Xdoteci);

			// --------------Evaluate 5th Taylor Series Term-----------  
			for (j = 0; j < 6; j++)
			{
				K[j][3] = dtsec *  Xdoteci[j][0];
				Temp[j][0] = Xeci[j][0] + K[j][0] * 439.0 / 216.0
					- K[j][1] * 8.0 + K[j][2] * 3680.0 / 513.0 - K[j][3] * 845.0 / 4104.0;
			}
			if (derivType == 1)
				deriv(Temp, Xdoteci);
			else
				pderiv(jdutc, jdutcF + dtsec * dayconv, Temp, rsunecef, rmoonecef, derivType, cdam, cram, trans, pm, omegaearth, Xdoteci);

			// --------------Evaluate 6th Taylor Series Term-----------  
			tempjd = jdutc;
			tempjdF = jdutcF + dtsec * 0.5 * dayconv;
			for (j = 0; j < 6; j++)
			{
				K[j][4] = dtsec *  Xdoteci[j][0];
				Temp[j][0] = Xeci[j][0] - K[j][0] * 8.0 / 27.0
					+ K[j][1] * 2.0 - K[j][2] * 3544.0 / 2565.0
					+ K[j][3] * 1859.0 / 4104.0 - K[j][4] * 0.275;
			}
			if (derivType == 1)
				deriv(Temp, Xdoteci);
			else
				pderiv(tempjd, tempjdF, Temp, rsunecef, rmoonecef, derivType, cdam, cram, trans, pm, omegaearth, Xdoteci);

			for (j = 0; j < 6; j++)
				K[j][5] = dtsec *  Xdoteci[j][0];

			// --------------------Check for convergence---------------  
			double Err = 0.0;
			for (j = 0; j < 6; j++)
				Err = fabs(K[j][0] * 1.0 / 360.0
				- K[j][2] * 128.0 / 4275.0 - K[j][3] * 2197.0 / 75240.0
				+ K[j][4] * 0.02 + K[j][5] * 2.0 / 55.0);

			// ------Update the State vector, perform integration------ 
			if ((Err < small) || (dtsec <= 2.0 * HMin + small))
			{
				for (j = 0; j < 6; j++)
					Xeci[j][0] = Xeci[j][0] + K[j][0] * 25.0 / 216.0 + K[j][2] * 1408.0 / 2565.0
					+ K[j][3] * 2197.0 / 4104.0 - K[j][4] * 0.2;
				Time = Time + dtsec * dayconv;
				ss = 0.0;
				ktr = 1;
			}
			else
			{
				ss = 0.84 * pow(small * dtsec / Err, 0.25);
				if ((ss < 0.75) && (dtsec > 2.0 * HMin)) // Reduce  Step  Size 
					dtsec = dtsec * 0.5;
				if ((ss > 1.5) && (2.0 * dtsec < HMax)) // Increase Step Size 
					dtsec = dtsec * 2.0;
				ktr = ktr + 1;
				//	printf("itime ", itime:18 : 11, ktr : 3, "dtsec", dtsec : 18 : 15, " err", err : 10 : 7, " s", s : 10 : 7, "kj6", k, 1, 6) : 10 : 6);
				//	printf(FileOut, "itime ", itime:18 : 11, ktr : 3, "dtsec", dtsec : 8 : 4, " err", err : 10 : 7, " s", s : 10 : 7, "kj6", k, 1, 6) : 10 : 6);
			}

		}  // while

	}  // rkF45 


	

	//
	// Sixth Order Runge-Kutta Integrator (Rung-Kutta-Fehlberg)
	// This yeilds the same results as the 4th order rk4 integrator, but
	// improves the processing speed by having variable step sizes.
	// 
	// Fundamentals of Astrodynamics and Applications, Second Edition, Chapter 8
	//
	//  * yn is a 6 element array defining the position (km) and velocity (km/s) 
	// vectors of the satellite at time t (Fundementals equation 8-7)
	// yn[0] = Xeci,    yn[1] = y,    yn[2] = z     (km)
	// yn[3] = Xdoteci, yn[4] = ydot, yn[5] = zdot  (km/s)
	//
	// BC is the ballistic Coefficient BC=(mass/AccDragCoefficient * Area)  (kg/m2)
	// SC is the solar coefficient     SC=(Reflectivity * Area/mass)     (m2/kg)
	//
	//  * yOut is a 6 element array defining the position (km) and velocity (km/s) 
	// vectors of the satellite at time t+deltaT
	//
	// Note: t and deltaT are in Julian Days
	//
	//void rkF
	//	(
	//	double yn[7],
	//	double jdutc, double jdutcF, double BC, double SC,
	//	double& step, double yOut[7]
	//	)
	//{
	//	int i;
	//	double h;
	//	double k1[7], k2[7], k3[7], k4[7], k5[7], k6[7];
	//	double yNext[7], ydot[7];
	//	double deltaT;
	//	double delta45[7], s, MagDelta45;
	//	double startStepSize;

	//	double t = jdutc + jdutcF;

	//	deltaT = step;

	//	// the deltaT variable needs to be in seconds
	//	// to maintain consistency with km and km/s
	//	h = deltaT * 24.0 * 3600.0;

	//	// k1 = h * f(xn,yn)
	//	// where f(xn,yn) is the derivative function
	//	pderiv(t, BC, SC, yn, ydot);
	//	for (i = 0; i < 6; i++)
	//	{
	//		k1[i] = h * ydot[i];
	//		yNext[i] = yn[i] + k1[i] / 4.0;
	//	}

	//	// k2 = h * f(xn + h/4,yn + k1/4)
	//	pderiv(t + deltaT / 4.0, BC, SC, yNext, ydot);
	//	for (i = 0; i < 6; i++)
	//	{
	//		k2[i] = h * ydot[i];

	//		yNext[i] = yn[i] + 3.0 * k1[i] / 32.0 + 9.0 * k2[i] / 32.0;
	//	}

	//	// k3 = h * f(xn + 3h/8,yn + 3k1/32 + 9k2/32)
	//	pderiv(t + 3.0 * deltaT / 8.0, BC, SC, yNext, ydot);

	//	for (i = 0; i < 6; i++)
	//	{
	//		k3[i] = h * ydot[i];

	//		yNext[i] = yn[i] + 1932.0 * k1[i] / 2197.0 - 7200.0 * k2[i] / 2197.0 + 7296.0 * k3[i] / 2197.0;
	//	}

	//	// k4 = h * f(xn + 12h/13,yn + 1932k1/2197 - 7200k2/2197 + 7296k3/2197)
	//	pderiv(t + 12.0 * deltaT / 13.0, BC, SC, yNext, ydot);
	//	for (i = 0; i < 6; i++)
	//	{
	//		k4[i] = h * ydot[i];

	//		yNext[i] = yn[i] + 439.0 * k1[i] / 216.0 - 8.0 * k2[i] + 3680.0 * k3[i] / 513.0 - 845.0 * k4[i] / 4104.0;
	//	}

	//	// k5 = h * f(xn + h,yn + 439/216 - 8k28 + 3680k3/513 - 854k4/4104)
	//	pderiv(t + deltaT, BC, SC, yNext, ydot);
	//	for (i = 0; i < 6; i++)
	//	{
	//		k5[i] = h * ydot[i];

	//		yNext[i] = yn[i] - 8.0 * k1[i] / 27.0 + 2.0 * k2[i] - 3544.0 * k3[i] / 2565.0 + 1859.0 * k4[i] / 4104.0 - 11.0 * k5[i] / 40.0;
	//	}

	//	// k6 = h * f(xn + h/2.0,yn + 439/216 - 8k28 + 3680k3/513 - 854k4/4104)
	//	pderiv(t + deltaT / 2.0, BC, SC, yNext, ydot);
	//	for (i = 0; i < 6; i++)
	//	{
	//		k6[i] = h * ydot[i];

	//		yOut[i] = yn[i] + 25.0 * k1[i] / 216.0 + 1408.0 * k3[i] / 2565.0 + 2197.0 * k4[i] / 4104.0 - k5[i] / 5.0;

	//		delta45[i] = k1[i] / 360.0 - 128.0 * k3[i] / 4275.0 - 2197.0 * k4[i] / 75240.0 + k5[i] / 50.0 + 2.0 * k6[i] / 55.0;
	//	}

	//	// compute variable step size parameters and equations
	//	MagDelta45 = sqrt(delta45[0] * delta45[0] + delta45[1] * delta45[1] + delta45[2] * delta45[2]);

	//	// Compute S avoid divide by zero.
	//	s = 1000.0;
	//	if (MagDelta45 > 1e-20)
	//		s = 0.8408 * sqrt(sqrt(1.0e-8 * h / MagDelta45));

	//	startStepSize = elSetrk4.stepSize;

	//	// if step size is too large,  reset step size and recompute position
	//	// based on smaller step size
	//	// NOTE RECURSION!
	//	if ((s < 0.75) && (fabs(elSetrk4.stepSize - elSetrk4.minStep) > 1e-6))
	//	{
	//		// Half the step size and see if you have it right
	//		elSetrk4.stepSize = elSetrk4.stepSize / 2.0;
	//		if (elSetrk4.stepSize < elSetrk4.minStep)
	//			elSetrk4.stepSize = elSetrk4.minStep;
	//		step = deltaT / fabs(deltaT) * elSetrk4.stepSize;  //keep the sign of the step!
	//		rkF(elSetrk4.yn, elSetrk4.jdutc, elSetrk4.jdutcF, elSetrk4.BC, elSetrk4.SC, step, yOut);
	//	}

	//	// step size can get bigger next step
	//	else if (s > 1.5)
	//	{
	//		// if the current step is smaller than the current stepSize, don't adjust larger
	//		// because may be stepping in small increments due to some required period of update
	//		// say once per second 
	//		if (fabs(step) < elSetrk4.stepSize) return;
	//		// Double current step Size, limit to a maximum step size
	//		elSetrk4.stepSize = elSetrk4.stepSize * 2.0;
	//		if (elSetrk4.stepSize > elSetrk4.maxStep)
	//			elSetrk4.stepSize = elSetrk4.maxStep;
	//	}
	//}


	/* ------------------------- test numerical propagator --------------------------
	* use Xfeci for the final state in case you want to restart from a certain point
	* kepler will start from the original epoch
	* srpperts will start from the previous step
	*  use an integer because msvs c++ cannot do strings reliably as formal parameters
    *     derivType = 1 = 2-body
    *     derivType = 2 = J2
    *     derivType = 3 = J3
    *     derivType = 4 = J4
    *     derivType = 5 = Jx
    *     derivType = 6 = drag
    *     derivType = 7 = 3-body
    *     derivType = 8 = srp
    *     derivType = 10 = all
    *     derivtype = 11 - srp semianalytical
	*     derivtype = 12 - 3body semianalytical
    *     derivtype = 13 - zonal semianalytical
	*     interp      - interpolation for sun/moon vectors 'l', 's'
	*     opt         - method of calc sun/moon        'a', 'j'
	-----------------------------------------------------------------------------  */
    void testPropOrbit
		(
		double jdutc, double jdutcF, 
		double cdam, double cram,
		int derivType, char interp, char opt,
		double rosc[3], double vosc[3],
		double rmean[3], double vmean[3]
		)
	{
		double jde, jdeF, jds, jdsF, tut1, rmean1[3], vmean1[3], rosc1[3], vosc1[3];
		double dut1, lod, xp, yp, ddpsi, ddeps, iaudx, dy, icrsx, y, s, deltapsi, deltaeps, jdut1, jdut1f,
			ttt, jdtt, jdttf;
		double dtsec, dtseco, jdepoch, jdtdb, jdtdbF;
		int i, ii, dat, timezone;
		std::vector< std::vector<double> > Xeci = std::vector< std::vector<double> >(7, std::vector<double>(1, 0.0));
		std::vector< std::vector<double> > Xfeci = std::vector< std::vector<double> >(7, std::vector<double>(1, 0.0));
		double dayconv = 1.0 / 86400.0;
		double jdyconv = 1.0 / 36525.0;

		jde = jdutc;  // assumed to be utc
		jdeF = jdutcF;
		// set base values
		jdtt = jdutc;
		jdut1 = jdutc;


		// semianalytic uses 1-day steps
		// numerical cannot use this!
		if (derivType >= 11)
		{
			dtseco = 86400.0;  // sec
			jds = jde + 31;    // 91, 31, 10 days
		}
		else
		{
			dtseco = 60.0;   // sec
			jds = jde + 31;  // 3 days xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
		}
		dtsec = dtseco; // initial
		timezone = 0;

		// stopping condition
		jdsF = jdeF;

		// for semianalytic, incoming is osculating vectors
		// we'll assume eci for now
		for (i = 0; i < 3; i++)
		{
			Xeci[i][0] = rmean[i];
			Xeci[i + 3][0] = vmean[i];
		}
    	printf("%lf  %lf %lf %lf %lf %lf %lf \n", 0.0, Xeci[0][0], Xeci[1][0], Xeci[2][0], Xeci[3][0], Xeci[4][0], Xeci[5][0]);

		// ----------------- initialize all the EOP/jpl/coord data once at the beginning ---------------------
		double jdeopstart, jdeopstartf;
		char EopLoc[85] = "../nut80.dat";  //D:/Codes/LIBRARY/CPP/TestSGP4DC/
		iau80data iau80rec;
		coordFK5::iau80in(iau80rec, EopLoc);
		std::vector<eopdata> eoparr;
		EopSpw::initeop(eoparr, "../EOP-All-v1.1_2018-01-04.txt", jdeopstart, jdeopstartf);

        // ------------------------------ initialize the JPL DE ephemerides ----------------------------------
		double jdjpldestart, jdjpldestartFrac;
		//double rsun[3], rmoon[3], rsmag;
		std::vector<jpldedata> jpldearr;
		jpldearr.resize(jpldesize);
		ast2Body::initjplde(jpldearr, "D:/Codes/LIBRARY/CPP/TestSGP4DC/sunmooneph_430t.txt", jdjpldestart, jdjpldestartFrac);
		
        // for numerical integration, Xeci, Xfeci are osculating
		if (derivType <= 10)
		{
			for (ii = 0; ii < 3; ii++)
			{
				Xeci[ii][0] = rosc[ii];
				Xeci[ii + 3][0] = vosc[ii];
			}
		}

		long ktr = 0;
		// set back so first step is epoch time
		jdutcF = jdutcF - dtseco * dayconv;
		while (jdutc + jdutcF < jds + jdsF)
		{
			if (derivType >= 11)
			{
				// Xeci and Xfeci are always mean here
				EopSpw::findeopparam(jdutc, jdutcF + dtseco * dayconv, 'l', 'f', eoparr, jdeopstart, dut1, dat, lod, xp, yp, ddpsi, ddeps, iaudx, dy, icrsx, y, s, deltapsi, deltaeps);
				jdut1 = jdutc;
				jdut1f = jdutcF + (dtseco + dut1) * dayconv;
				tut1 = (jdut1 + jdut1f - 2451545.0) * jdyconv;
				jdtt = jdutc; 
				jdttf = jdutcF + (dtseco + dat + 32.184) * dayconv;   // sec
				ttt = (jdtt + jdttf - 2451545.0) * jdyconv;
				jdtdb = jdtt;   // set approx equal
				jdtdbF = jdttf;

				if (ktr == 0)
				{
					jdepoch = jdut1 + jdut1f;
					ast2Body::kepler(rosc, vosc, 0.0, rosc1, vosc1);
				}
				else
					ast2Body::kepler(rosc, vosc, dtseco, rosc1, vosc1);

				if (show == 'y')
					printf("2bodyk %5i  %11.6f %11.6f %11.6f %11.6f %11.6f %11.6f %12.3f \n", ktr + 11, rosc1[0], rosc1[1], rosc1[2], vosc1[0], vosc1[1], vosc1[2], dtsec);

				char fullsun = 'y';
				for (ii = 0; ii < 3; ii++)
				{
					rmean[ii] = Xeci[ii][0];
					vmean[ii] = Xeci[ii + 3][0];
				}
				//dtsec = (jdut1 + jdut1f - jdepoch) * 86400.0;
				// these are all mean elements
				// or use dtseco if redoing the mean state each time
				if (derivType == 11)
				{
					if (ktr == 0)
						astPert::srpperts(jdtdb, jdtdbF, rmean, vmean, interp, opt, fullsun, 0.0, jpldearr, jdjpldestart, tut1, cram, rosc1, vosc1, rmean1, vmean1);
					else
						astPert::srpperts(jdtdb, jdtdbF, rmean, vmean, interp, opt, fullsun, dtseco, jpldearr, jdjpldestart, tut1, cram, rosc1, vosc1, rmean1, vmean1);
				}

				if (derivType == 12)
				{
					if (ktr == 0)
						astPert::thirdbodyperts(jdtdb, jdtdbF, rmean, vmean, interp, jpldearr, jdjpldestart, 0.0, tut1, rosc1, vosc1, rmean1, vmean1);
					else
						astPert::thirdbodyperts(jdtdb, jdtdbF, rmean, vmean, interp, jpldearr, jdjpldestart, dtseco, tut1, rosc1, vosc1, rmean1, vmean1);
				}

				if (derivType == 13)
				{
					if (ktr == 0)
					    astPert::zonalperts(jdutc, jdutcF, rmean, vmean, 0.0, tut1, rosc1, vosc1, rmean1, vmean1);
					else
						astPert::zonalperts(jdutc, jdutcF, rmean, vmean, dtseco, tut1, rosc1, vosc1, rmean1, vmean1);
				}

				// take large step sizes here
				for (ii = 0; ii < 3; ii++)
				{
					// update starting mean vectors for next step
					Xeci[ii][0] = rmean1[ii];
					Xeci[ii + 3][0] = vmean1[ii];
				}
			}
			else
			{
				// Move the orbit ahead one step and copy the output to the current position
				// Xeci and Xfeci are always osculating (assuming eci too) here
				astPert::rk4(jdutc, jdutcF, dtsec, Xeci, derivType, cdam, cram, interp, opt, jpldearr, jdjpldestart, iau80rec, jdeopstart, eoparr, Xfeci);
				for (ii = 0; ii < 3; ii++)
				{
					rosc1[ii] = Xfeci[ii][0];
					vosc1[ii] = Xfeci[ii + 3][0];
					// transfer for next step
					Xeci[ii][0] = Xfeci[ii][0];
					Xeci[ii+3][0] = Xfeci[ii+3][0];
				}
			}

			if (ktr <= 92 ||  fmod(ktr+1, 1440) == 0 )  //  ktr == 1439 || ktr == 2879 || ktr == 4319 || ktr == 5759)
			{
				if (derivType == 1)
					printf("2bodyn %i %lf %lf %lf %lf %lf %lf  %lf \n", ktr + 11, rosc1[0], rosc1[1], rosc1[2], vosc1[0], vosc1[1], vosc1[2], dtseco);
				if (derivType == 11)
					printf("srp %i %lf %lf %lf %lf %lf %lf  %lf \n", ktr + 11, rosc1[0], rosc1[1], rosc1[2], vosc1[0], vosc1[1], vosc1[2], dtseco);
				if (derivType == 12)
					printf("3body %i %lf %lf %lf %lf %lf %lf  %lf \n", ktr + 11, rosc1[0], rosc1[1], rosc1[2], vosc1[0], vosc1[1], vosc1[2], dtseco);
				if (derivType == 13)
					printf("zonal %i %lf %lf %lf %lf %lf %lf  %lf \n", ktr + 11, rosc1[0], rosc1[1], rosc1[2], vosc1[0], vosc1[1], vosc1[2], dtseco);
				if (derivType > 1 && derivType <= 10)
					printf("    %i %lf %lf %lf %lf %lf %lf  %lf \n", ktr + 11, rosc1[0], rosc1[1], rosc1[2], vosc1[0], vosc1[1], vosc1[2], dtseco);
			}

			if (show == 'y')
				printf(" ------------------------- \n");
			// both jd, jdf work...either way
			if (dtseco == 86400.0)
				jdutc = jdutc + dtseco * dayconv;
			else
				jdutcF = jdutcF + dtseco * dayconv;
			ktr = ktr + 1;
		} // while
	}  // testPropOrbit


	
	/*  ------------ ------------------------------------------------------------------
	 * 
	 *                            procedure pderiv
	 * 
	 *  This function calculates the derivative of the two-body state vector for
	 *  use with the Runge-Kutta algorithm including perturbations. Note that the vectors
	 *  are in earth fixed.
	 * 
	 *   author        : david vallado                  719-573-2600   11 jan 2018
	 * 
	 *   inputs          description                    range / units
	 *     jdutc       - epoch julian date              days from 4713 BC
	 *     jdutcF      - epoch julian date fraction     day fraction from jdutc
	 *     Xeci           - eci state vector at initial time   km, km/s
	 *     derivType   - String of which perts to incl  'Y' and 'N'
	 *  switch this to an integer msvs c++ cannot do strings reliably as formal parameters
	 *     derivType = 1 = 2-body
	 *     derivType = 2 = J2
	 *     derivType = 3 = J3
	 *     derivType = 4 = J4
	 *     derivType = 5 = Jx
	 *     derivType = 6 = drag
	 *     derivType = 7 = 3-body
	 *     derivType = 8 = srp
	 *     derivType = 10 = all
	 *     derivType = 11 = semi-analytical
	 *     cdam        - cd * a / mass                    kg / m2
	 *     cram        - cr * a / mass                    kg / m2
	 *     rsun        - sun vector ecef                  km
	 *     rmoon       - moon vector ecef                 km
	 * 
	 *   outputs       :
	 *     Xdoteci        - eci derivative of state vector
	 * 
	 *   locals        :
	 *     r3          - Cube of r
	 *     apert       - Perturbing acceleration        km / s2
	 * 
	 *   coupling      :
	 *     coordFK5::itrf_gcrf
	 *     AccTwoBody
	 *     AccNonSph
	 *     AccDrag
	 *     Acc3Body
	 *     AccSRP
	 * 
	 *   references    :
	 *     vallado       2013, 526
	 * 
	-----------------------------------------------------------------------------  */

	void pderiv
		(
		double jdutc, double jdutcF,
		std::vector< std::vector<double> > Xeci,
		double rsun[3], double rmoon[3],
		int derivtype,
		double cdam, double cram,
		std::vector< std::vector<double> > trans,
		std::vector< std::vector<double> > pm,
		double omegaearth[3],
		std::vector< std::vector<double> > &Xdoteci
		)
	{
		Xeci.resize(7);  // rows
		for (std::vector< std::vector<double> >::iterator it = Xeci.begin(); it != Xeci.end(); ++it)
			it->resize(7);
		Xdoteci.resize(7);  // rows
		for (std::vector< std::vector<double> >::iterator it = Xdoteci.begin(); it != Xdoteci.end(); ++it)
			it->resize(7);
		trans.resize(3);  // rows
		for (std::vector< std::vector<double> >::iterator it = trans.begin(); it != trans.end(); ++it)
			it->resize(3);
		pm.resize(3);  // rows
		for (std::vector< std::vector<double> >::iterator it = pm.begin(); it != pm.end(); ++it)
			it->resize(3);
		// locals
		double apert[3], reci[3], veci[3], aeci[3], aeci1[3], recef[3], vecef[3], aecef[3], omgxr[3], omgxomgxr[3],
			rpef[3], vpef[3], apef[3], omgxv[3], tempvec1[3], tempvec[3];
		int i;
		std::vector< std::vector<double> > transp = std::vector< std::vector<double> >(3, std::vector<double>(3, 0.0));
		std::vector< std::vector<double> > pmp = std::vector< std::vector<double> >(3, std::vector<double>(3, 0.0));

		for (i = 0; i < 3; i++)
		{
			reci[i] = Xeci[i][0];
			veci[i] = Xeci[i + 3][0];
			aeci[i] = 0.0;
			aeci1[i] = 0.0;
			aecef[i] = 0.0;
			apert[i] = 0.0;
		}

		if ((derivtype > 1 && derivtype <= 6) || (derivtype == 10))
		{
			// convert eci to ecef if needed (non-spherical grav, drag)
			astMath::mattrans(trans, transp, 3, 3);
			astMath::mattrans(pm, pmp, 3, 3);

			astMath::matvecmult(transp, reci, rpef);
			astMath::matvecmult(pmp, rpef, recef);

			astMath::cross(omegaearth, rpef, omgxr);
			astMath::matvecmult(transp, veci, tempvec1);
			astMath::addvec(1.0, tempvec1, -1.0, omgxr, vpef);
			astMath::matvecmult(pmp, vpef, vecef);
		}

		// ----------------------------------------------------------------
		// ---- need 2-body acceleration with all the others! ----
		AccTwoBody(reci, aeci);

		// ---- j2, j3, j4 for now
		if ((derivtype > 1 && derivtype < 6) || (derivtype == 10))
		{
			AccNonSph(recef, apert);
			for (i = 0; i < 3; i++)
				aecef[i] = aecef[i] + apert[i];
		}

		// ---- exponential atmosphere model for now
		if (derivtype == 6 || derivtype == 10)
		{
			AccDrag(recef, vecef, omegaearth[2], cdam, apert);
			for (i = 0; i < 3; i++)
				aecef[i] = aecef[i] + apert[i];
		}

		// ---- 3-body sun and moon
		if (derivtype == 7 || derivtype == 10)
		{
			Acc3Body(jdutc, jdutcF, reci, rsun, rmoon, apert);
			for (i = 0; i < 3; i++)
				aeci[i] = aeci[i] + apert[i];
		}

		// ---- solar radiation pressure 
		if (derivtype == 8 || derivtype == 10)
		{
			AccSRP(jdutc, jdutcF, reci, rsun, cram, apert);
			for (i = 0; i < 3; i++)
				aeci[i] = aeci[i] + apert[i];
		}

		// ---- change ecef back to eci for the cases where it is used
		if ((derivtype > 1 && derivtype <= 6) || (derivtype == 10))
		{
			// change ecef to eci for derivative vector
			astMath::matvecmult(pm, recef, rpef);
			astMath::matvecmult(trans, rpef, reci);

			astMath::matvecmult(pm, vecef, vpef);
			astMath::cross(omegaearth, rpef, omgxr);
			astMath::addvec(1.0, vpef, 1.0, omgxr, tempvec1);
			astMath::matvecmult(trans, tempvec1, veci);

			astMath::matvecmult(pm, aecef, apef);
			astMath::cross(omegaearth, omgxr, omgxomgxr);
			astMath::cross(omegaearth, vpef, omgxv);
			astMath::addvec(1.0, apef, 1.0, omgxomgxr, tempvec);
			astMath::addvec(1.0, tempvec, 2.0, omgxv, tempvec1);
			astMath::matvecmult(trans, tempvec1, aeci1);
		}

		// find the derivative state vector
		// be sure to add all accelerations
		for (i = 0; i < 3; i++)
		{
			Xdoteci[i][0] = veci[i];     // km/s
			Xdoteci[i + 3][0] = aeci[i] + aeci1[i];  // km/s2
		}
	}  // pderiv



	/*  ------------------------------------------------------------------------------
	 * 
	 *                            procedure AccTwoBody
	 * 
	 *  This function calculates the 2-body acceleration. Note that the operation 
	 *  takes place in the inertial coordinate system.
	 * 
	 *   author        : david vallado                  719-573-2600   11 jan 2018
	 * 
	 *   inputs          description                    range / units
	 *     reci        - eci position vector           km
	 * 
	 *   outputs       :
	 *     accel       - perturbing acceleration        km / s2
	 * 
	 *   locals        :
	 * 
	 *   coupling      :
	 *     astMath:: mag
	 * 
	 *   references    :
	 *     vallado       2013, 526
	-----------------------------------------------------------------------------  */

	void AccTwoBody
		(
		double reci[3], double accel[3]
		)
	{
		double rmag, temp;
		int i;

		rmag = astMath::mag(reci);  //km
		// derivative of velocity is acceleration
		temp = -mu / (rmag * rmag * rmag);  // (1/s2)
		for (i = 0; i < 3; i++)
			accel[i] = accel[i] + (reci[i] * temp);    // (km/s2)
	}  // AccTwoBody


	/*  ------------------------------------------------------------------------------
	 * 
	 *                            procedure AccNonSph
	 * 
	 *  This function calculates the non-spherical acceleration. this operation takes place
	 *  in the earth fixed frame. this is especially imporatnt when considering full 
	 *  geopotential and satellite latitude longitude points. 
	 * 
	 *   author        : david vallado                  719-573-2600   11 jan 2018
	 * 
	 *   inputs          description                    range / units
	 *     recef       - ecef position vector           km
	 * 
	 *   outputs       :
	 *     accel       - perturbing acceleration        km / s2
	 * 
	 *   locals        :
	 * 
	 *   coupling      :
	 *     astMath:: mag
	 * 
	 *   references    :
	 *     vallado       2013, 526
	-----------------------------------------------------------------------------  */

	void AccNonSph
		(
		double recef[3], double accel[3]
		)
	{
		double rmag, rmag2, temp, temp1, temp2;
		double a2[3], a3[3], a4[3];
		int i;

		rmag = astMath::mag(recef);  // km
		rmag2 = rmag * rmag;

		// J2 component
		temp = -1.5 * j2 * mu  * re * re / (rmag2 * rmag2 * rmag);
		temp1 = 1.0 - 5.0 * recef[2] * recef[2] / rmag2;
		a2[0] = temp * recef[0] * temp1;   // (km/s2)
		a2[1] = temp * recef[1] * temp1;
		a2[2] = temp * recef[2] * (3.0 - 5.0 * recef[2] * recef[2] / rmag2);
		//printf("a2 //lf //lf //lf \n", a2[0], a2[1], a2[2]);

		// J3 component
		temp1 = -2.5 * j3  * mu  * pow(re, 3) / (rmag2 * rmag2 * rmag2 * rmag);
		temp2 = 3.0 * recef[2] - 7.0 * pow(recef[2], 3) / rmag2;
		a3[0] = temp1 * recef[0] * temp2;
		a3[1] = temp1 * recef[1] * temp2;
		a3[2] = temp1 * recef[2] * (6.0 * recef[2] * recef[2] - (7.0 * pow(recef[2], 4) / rmag2) - (3.0 * rmag2 / (5.0 * rmag2)));
		//printf("a3 //lf //lf //lf \n", a3[0], a3[1], a3[2]);

		// J4 component
		temp1 = -1.875 * j4 * mu  * pow(re, 4) / (rmag2 * rmag2 * rmag2 * rmag);
		temp2 = 1.0 - (14.0 * pow(recef[2], 2) / rmag2) + (21.0 * pow(recef[2], 4) / (rmag2 * rmag2));
		a4[0] = temp1 * recef[0] * temp2;
		a4[1] = temp1 * recef[1] * temp2;
		a4[2] = temp1 * recef[2] * (5.0 - (70.0 * pow(recef[2], 2) / (3.0 * rmag2)) + (21.0 * pow(recef[2], 4) / (rmag2 * rmag2)));
		//printf("a4 //lf //lf //lf \n", a4[0], a4[1], a4[2]);

		for (i = 0; i < 3; i++)
			accel[i] = a2[i] + a3[i] + a4[i];   // (km/s2)
	}  // AccNonSph



	/*  ------------------------------------------------------------------------------
	 * 
	 *                            procedure Acc3Body
	 * 
	 *  This function calculates the 3-body acceleration from sun and moon.
	 * 
	 *   author        : david vallado                  719-573-2600   11 jan 2018
	 * 
	 *   inputs          description                    range / units
	 *     jdutc       - epoch julian date              days from 4713 BC
	 *     jdutcF      - epoch julian date fraction     day fraction from jdutc
	 *     reci        - eci position vector             km
	 *     rsuneci     - sun vector eci                   km
	 *     rmooneci    - moon vector eci                  km
	 *
	 *   outputs       :
	 *     accel       - perturbing acceleration        km / s2
	 * 
	 *   locals        :
	 * 
	 *   coupling      :
	 *     astMath:: mag, dot
	 *     ast2Body::sunmoonjpl, sun, moon
	 * 
	 *   references    :
	 *     vallado       2013, 526
	-----------------------------------------------------------------------------  */
	
	void Acc3Body
		(
		double jdutc, double jdutcF, double reci[3], double rsuneci[3], double rmooneci[3],
		double accel[3]
		)
	{
		double q, temp, mu3, rmag, temp1;
		double r3mag, rsat3[3], rsat3mag;
		double a1[3], a2[3];
		int i;

		double mu3s = 1.32712428e11; // sun  km3 / s2
		double mu3m = 4902.799;      // moon  km3 / s
		double rad = 180.0 / pi;

        // ------------------ sun calcs
		r3mag = astMath::mag(rsuneci);
		rmag = astMath::mag(reci);
		mu3 = mu3s;

		//// alt approach Taylor series
		////temp = -mu / (pow(rmag, 3));
		//temp1 = -mu3s / (pow(r3mag, 3));
		//temp2 = astMath::dot(reci, rsuneci) / (r3mag*r3mag);
		//for (i = 0; i < 3; i++)
		//	a1[i] = temp1 * (rsuneci[i] - 3.0 * rsuneci[i] * temp2 - 7.5 * temp2*temp2 * rsuneci[i]);  // don't include 2-body part  temp * reci[i]
		//printf("3bdacc1b %11.9f %11.9f %11.9f \n", a1[0], a1[1], a1[2]);

		// find sat to 3rd body vector
		for (i = 0; i < 3; i++)
			rsat3[i] = rsuneci[i] - reci[i];
		rsat3mag = astMath::mag(rsat3);
		temp = astMath::dot(reci, rsat3);
		q = ((rmag * rmag + 2.0 * temp) * (r3mag * r3mag + r3mag * rsat3mag + rsat3mag * rsat3mag)) / (pow(r3mag, 3) * pow(rsat3mag, 3) * (r3mag + rsat3mag));
		//temp = -mu / (pow(rmag, 3));
		temp1 = 1.0 / (pow(r3mag, 3));
		for (i = 0; i < 3; i++)
			a1[i] =  mu3 * (rsat3[i] * q - reci[i] * temp1);  // don't include 2-body part temp * reci[i] +
		//printf("3bdacc1a %11.9f %11.9f %11.9f \n", a1[0], a1[1], a1[2]);



		// ------------------ moon calcs
		mu3 = mu3m;
		r3mag = astMath::mag(rmooneci);

		//// alt approach Taylor series
		////temp = -mu / (pow(rmag, 3));
		//temp1 = -mu3m / (pow(r3mag, 3));
		//temp2 = astMath::dot(reci, rmooneci) / (r3mag*r3mag);
		//for (i = 0; i < 3; i++)
		//	a2[i] = temp1 * (rmooneci[i] - 3.0 * rmooneci[i] * temp2 - 7.5 * temp2*temp2 * rmooneci[i]);// don't include 2-body part 
		//printf("3bdacc2b %11.9f %11.9f %11.9f \n", a2[0], a2[1], a2[2]);


		//// to avoid numerical problems, use various vectors
		for (i = 0; i < 3; i++)
			rsat3[i] = rmooneci[i] - reci[i];
		rsat3mag = astMath::mag(rsat3);

		temp = astMath::dot(reci, rsat3);
		q = ((rmag * rmag + 2.0 * temp) * (r3mag * r3mag + r3mag * rsat3mag + rsat3mag * rsat3mag)) / (pow(r3mag, 3) * pow(rsat3mag, 3) * (r3mag + rsat3mag));
		//temp = -mu / (pow(rmag, 3));
		temp1 = 1.0 / (pow(r3mag, 3));
		for (i = 0; i < 3; i++)
			a2[i] =  mu3 * (rsat3[i] * q - reci[i] * temp1);  // don't include 2-body part temp * reci[i] +
		//printf("3bdacc2a %11.9f %11.9f %11.9f \n", a2[0], a2[1], a2[2]);


		// Add the accelerations
		for (i = 0; i < 3; i++)
			accel[i] = a1[i] + a2[i];
//		printf("3bdaccl %11.9f %11.9f %11.9f \n", accel[0], accel[1], accel[2]);
	}  // Acc3Body



	/*  ------------------------------------------------------------------------------
	 * 
	 *                            procedure AccDrag
	 * 
	 *  This function calculates the atmospheric drag acceleration. this operation takes 
	 * place in the earth fixed coordinate system. 
	 * 
	 *   author        : david vallado                  719-573-2600   11 jan 2018
	 * 
	 *   inputs          description                    range / units
	 *     Xecef       - epoch ecef state vector        km, km/s
	 *     cdam        - cd * a / mass                    kg / m2
	 * 
	 *   outputs       :
	 *     accel       - perturbing acceleration        km / s2
	 * 
	 *   locals        :
	 * 
	 *   coupling      :
	 *     astMath:: mag
	 * 
	 *   references    :
	 *     vallado       2013, 526
	-----------------------------------------------------------------------------  */
	
	void AccDrag
		(
		double recef[3], double vecef[3],
		double omegaearth, double cdam, 
		double accel[3]
		)
	{
		double rmag, vmag, vRel[3], aDrag[3], rho, temp;
		int i;
		double dayconv = 1.0 / 86400.0;

		rmag = astMath::mag(recef);  // (km)

		// Compute Atmospheric Density (Exponential Model)
		// rho = atmosphericDensity(recef);   // (kg/mass3)
		rho = 1e-12;

		// Find the relative veloctity between the earths rotation and the satellite
		vRel[0] = vecef[0] + recef[1] * omegaearth * dayconv;  // (km/s)
		vRel[1] = vecef[1] - recef[0] * omegaearth * dayconv;
		vRel[2] = vecef[2];
		vmag = astMath::mag(vRel);

		temp = 0.0;
		if (cdam > 1.0e-15)
			temp = -0.5 * cdam * rho * vmag * vmag;  // (1/s)

		for (i = 0; i < 3; i++)
			aDrag[i] = temp * vRel[i];     // (km/s2)

		// Add in the accelerations
		for (i = 0; i < 3; i++)
			accel[i] = aDrag[i];      // (km/s2)
	}  // AccDrag


	/*  ------------------------------------------------------------------------------
	 * 
	 *                            procedure AccSRP
	 * 
	 *  This function calculates the solar radiation pressure acceleration.
	 * 
	 *   author        : david vallado                  719-573-2600   11 jan 2018
	 * 
	 *   inputs          description                    range / units
	 *     jdutc       - epoch julian date              days from 4713 BC
	 *     jdutcF      - epoch julian date fraction     day fraction from jdutc
	 *     reci        - eci position vector              km
	 *     rsun        - sun vector eci                   km
	 *     cram        - cr * a / mass                    kg / m2
	 * 
	 *   outputs       :
	 *     accel       - perturbing acceleration        km / s2
	 * 
	 *   locals        :
	 * 
	 *   coupling      :
	 *     astMath:: mag
	 *     ast2Body::sunmoonjpl, sun
	 * 
	 *   references    :
	 *     vallado       2013, 526
	-----------------------------------------------------------------------------  */
	 
	void AccSRP
		(
		double jdutc, double jdutcF, double reci[3], double rsuneci[3],
        double cram, double accel[3]
		)
	{
		double temp, r3satmag;
		double r3sat[3];
		int i;

		// find sat to 3rd body vector
		for (i = 0; i < 3; i++)
			r3sat[i] = rsuneci[i] - reci[i]; // km
		r3satmag = astMath::mag(r3sat);            // km

		temp = -pSR * cram / r3satmag;   // km/km s2
		for (i = 0; i < 3; i++)
			accel[i] = temp * r3sat[i];  // km/s2
	}  // AccSRP



	/*  ------------------------------------------------------------------------------
	 * 
	 *                            procedure zonalperts
	 * 
	 *   this procedure calculates the secular / lp avg rates and the osculating classical
	 *     elements for a satellite from zonal perturbations.
	 * 
	 *   author        : david vallado                  719 - 573 - 2600     3 dec 2017
	 * 
	 *   inputs          description                    range / units
	 *     jdutc       - epoch julian date              days from 4713 BC
	 *     jdutcF      - epoch julian date fraction     day fraction from jdutc
	 *     rmean       - init mean position vector eci  km
	 *     vmean       - init mean velocity vector eci  km / s
	 *     dtsec       - desired time from 0.0 epoch    sec
	 *     tut1        - julian centuries of ut1
	 *     cram        - cr * a / mass                    kg / m2
	 * 
	 *   outputs       :
	 *     rosc        - final osc position vector eci  km
	 *     vosc        - final osc velocity vector eci  km / s
	 *     rmean1      - final mean position vector eci  km
	 *     vmean1      - final mean velocity vector eci  km / s
	 * 
	 *   locals :
	 *     rsun        - vector from earth to sun       km
	 * 
	 *   coupling :
	 *     ast2Body::sunmoonjpl, sun, rv2coe, rv2eq, eq2rv
	 * 
	 *   references :
	 *     vallado       2013, ch 9
	 *  chao/hoots       2018 (tbd)
	----------------------------------------------------------------------------   */

	void zonalperts
		(
		double jdutc, double jdutcF, double rmean[3], double vmean[3],
		double dtsec,
		double tut1, double rosc[3], double vosc[3], double rmean1[3], double vmean1[3]
		)
	{
		double temp, magr;
		double p, a, ecc, incl, raan, argp, nu, m, arglat, truelon, lonper, mlonm, mlonnu, n;
		double beta, eccanom;
		double beta2, eccanom1;
		double p1, a1, ecc1, incl1, raan1, argp1, nu1, m1, arglat1, truelon1, lonper1;
		double cosi, sini, cosraan, sinraan, sinargp, cosargp;
		double af, ag, chi, psi, mlon, afnew, agnew, chinew, psinew, mlonmnew, anew;
		double dndts, dedts, didts, draandts, dargpdts, dmdts;
		double dadt, dndt, dafdt, dagdt, dchidt, dpsidt, dmlonmdt, dt;
		double dela, delaf, delag, delchi, delpsi, delmlonm;
		double dndtp, dedtp, didtp, draandtp, dargpdtp, dmdtp;
		double elemt0, elemratet0, t0, f1;
		double t2, x2, f2, t3, x3, f3, t4, x4, f4, elemnew;
		int i, fr;
		double rad = 180.0 / pi;

		// -------------------- - find mean coes at epoch---------------------- -
		ast2Body::rv2coe(rmean, vmean, mu, p, a, ecc, incl, raan, argp, nu, m, eccanom, arglat, truelon, lonper);
		if (show == 'y')
			printf("coes_init %11.4f %11.4f %13.9f %13.7f %11.5f %11.5f %11.5f %11.5f %11.5f \n",
			p, a, ecc, incl * rad, raan * rad, argp * rad, nu * rad, m * rad, arglat * rad);
		magr = astMath::mag(rmean);
		cosargp = cos(argp);
		sinargp = sin(argp);
		cosi = cos(incl);
		sini = sin(incl);
		cosraan = cos(raan);
		sinraan = sin(raan);
		beta2 = (1.0 - ecc * ecc);
		beta = sqrt(beta2);
		double j22 = j2 * j2;

		// ----------------find mean equinoctials at epoch--------------------
		ast2Body::rv2eq(rmean, vmean, a, n, af, ag, chi, psi, mlonm, mlonnu, fr);
		if (show == 'y')
			printf("eq_init  %12.9f %12.9f %12.9f %12.9f %12.9f %12.9f n %12.9f \n", a, af, ag, chi, psi, mlonm * rad, n);

		// -------------- - 2nd order secular and lp equations------------------
		// ----in terms of kepler elements
		dndts = 0.0;
		dedts = -3.0 / 32.0 * n * j22 * pow(re / p, 4) * sini * sini * (14.0 - 15.0 * sini * sini) * ecc * beta2 * sin(2.0 * argp)
			- 3.0 / 8.0 * n * j3 * pow(re / p, 3) * sini * (4.0 - 5.0 * sini * sini) * beta2 * cos(argp)
			- 15.0 / 32.0 * n * j4 * pow(re / p, 4) * sini * sini * (6.0 - 7.0 * sini * sini) * ecc * beta2 * sin(2.0 * argp);
		didts = 3.0 / 64.0 * n * j22 * pow(re / p, 4) * sin(2.0 * incl) * (14.0 - 15.0 * sini * sini) * ecc * ecc * sin(2.0 * argp)
			+ 3.0 / 8.0 * n * j3 * pow(re / p, 3) * cosi * (4.0 - 5.0 * sini * sini) * ecc * cos(argp)
			+ 15.0 / 64.0 * n * j4 * pow(re / p, 4) * sin(2.0 * incl) * (6.0 - 7.0 * sini * sini) * ecc * ecc * sin(2.0 * argp);
		draandts = -1.5 * n * j2 * pow(re / p, 2) * cosi
			- 1.5 * n * j22 * pow(re / p, 4) * cosi * (2.25 + 1.5 * beta - sini * sini * (2.5 + 2.25 * beta) + ecc * 0.25 * (1.0 + 1.25 * sini * sini) + pow(ecc, 2) / 8.0 * (7.0 - 15.0 * sini * sini) * cos(2.0 * argp))
			- 3.0 / 8.0 * n * j3 * pow(re / p, 3) * (15.0 * sini * sini - 4.0) * ecc * cosi / sini * sin(argp)
			+ 15.0 / 64.0 * n * j4 * pow(re / p, 4) * cosi * ((4.0 - 7.0 * sini * sini) * (1.0 + 1.5 * sini * sini) - (3.0 - 7.0 * sini * sini) * ecc * ecc * cos(2.0 * argp));
		dargpdts = 0.75 * n * j2 * pow(re / p, 2) * (4.0 - 5.0 * sini * sini)
			+ 3.0 / 16.0 * n * j22 * pow(re / p, 4) * (48.0 - 103.0 * sini * sini + 215.0 * 0.25 * pow(sini, 4) + (7.0 - 4.5 * sini * sini - 45.0 / 8.0 * pow(sini, 4)) * ecc * ecc
			+ 6.0 * (1.0 - 1.5 * sini * sini) * (4.0 - 5.0 * sini) * beta - 0.25 * (2.0 * (14.0 - 15.0 * sini * sini) * sini * sini - (28.0 - 158.0 * sini * sini + 135.0 * pow(sini, 4)) * ecc * ecc) * cos(2.0 * argp))
			+ 3.0 / 8.0 * n * j3 * pow(re / p, 3) * ((4.0 - 5.0 * sini * sini) * ((sini * sini - ecc * ecc * cosi * cosi) / (ecc * sini)) + 2.0 * sini * (13.0 - 15.0 * sini * sini) * ecc) * sin(argp)
			- 15.0 / 32.0 * n * j4 * pow(re / p, 4) * (16.0 - 62.0 * sini * sini + 49.0 * pow(sini, 4) + 0.75 * (24.0 - 84.0 * sini * sini + 63.0 * pow(sini, 4)) * ecc * ecc
			+ (sini * sini * (6.0 - 7.0 * sini * sini) - 0.5 * (12.0 - 70.0 * sini * sini + 63.0 * pow(sini, 4)) * ecc * ecc) * cos(2.0 * argp));
		// yes n in here
		dmdts = n * (1.0 + 1.5 * j2 * pow(re / p, 2) * (1.0 - 1.5 * sini * sini) * beta)
			+ 1.5 * n * j22 * pow(re / p, 4) * (pow(1.0 - 1.5 * sini * sini, 2) * beta2 + (1.25 * (1.0 - 2.5 * sini * sini + 13.0 / 8.0 * pow(sini, 4))
			+ 5.0 / 8.0 * (1.0 - sini * sini - 5.0 / 8.0 * pow(sini, 4)) * ecc * ecc + 1.0 / 16.0 * sini * sini * (14.0 - 15.0 * sini * sini) * (1.0 - 2.5 * ecc * ecc) * cos(2.0 * argp)) * beta)
			+ 3.0 / 8.0 * n * j22 * pow(re / p, 4) * 1.0 / beta * (3.0 * (3.0 - 7.5 * sini * sini + 47.0 / 8.0 * pow(sini, 4) + (1.5 - 5.0 * sini * sini + 117.0 / 6.0 * pow(sini, 4)) * ecc * ecc
			- 1.0 / 8.0 * (1.0 + 5.0 * sini * sini - 101.0 / 8.0 * pow(sini, 4)) * pow(ecc, 4))) + ecc * ecc / 8.0 * sini * sini * (70.0 - 123.0 * sini * sini
			+ (56.0 - 66.0 * sini * sini) * ecc * ecc) * cos(2.0 * argp) + 27.0 / 128.0 * pow(ecc, 4) * pow(sini, 4) * cos(4.0 * argp)
			- 3.0 / 8.0 * n * j3 * pow(re / p, 3) * sini * (4.0 - 5.0 * sini * sini) * (1.0 - 4.0 * ecc * ecc) / ecc * beta * sin(argp)
			+ 15.0 / 64.0 * n * j4 * pow(re / p, 4) * sini * sini * (6.0 - 7.0 * sini * sini) * (2.0 - 5.0 * ecc * ecc) * beta * cos(2.0 * argp)
			- 45.0 / 128.0 * n * j4 * pow(re / p, 4) * (8.0 - 40.0 * sini * sini + 35.0 * pow(sini, 4)) * ecc * ecc * beta;
		if (show == 'y')
			printf("Zonal secular %11.8g %11.8g %11.8g %11.8g %11.8g %11.8g\n", dndts, dedts, didts, draandts, dargpdts, dmdts);

		// -------------------------------------------------------------------- -
		// find avg rate of change in the equinoctial elements. use the 2nd order
		// secular and lp equations in kepler elements.these equations are the
		// rate in the numerical integration in terms of equinoctial elements.
		// -------------------------------------------------------------------- -
		temp = 1.0 / (1.0 + cosi);
		dadt = 0.0;
		dndt = 0.0;  // -1.5 * n / a * dadt3s;
		dafdt = dedts * cos(argp + raan) - ecc * (dargpdts + draandts) * sin(argp + raan);
		dagdt = dedts * sin(argp + raan) + ecc * (dargpdts + draandts) * cos(argp + raan);
		dchidt = didts * sinraan * temp + sini * draandts * cosraan * temp;
		dpsidt = didts * cosraan * temp - sini * draandts * sinraan * temp;
		dmlonmdt = dmdts + dargpdts + draandts;

		if (show == 'y')
		{
			//printf("zon_rate %11.7g %11.7g %11.7g %11.7g %11.7g %11.7g \n", dndt, dedts, didts, dafdt, dagdt, dchidt, dpsidt, dmlonmdt);
			printf("zon_rate (rad/min) %11.7g %11.7g %11.7g %11.7g %11.7g %11.7g \n", dndt * 60, dedts * 60, didts * 60, ecc*(draandts + dargpdts) * 60, sini*draandts * 60, (dmlonmdt - n) * 60);
		}
		printf("zon_rate (rad/min) %11.7g %11.7g %11.7g %11.7g %11.7g %11.7g \n", dndt * 60, dedts * 60, didts * 60, ecc*(draandts + dargpdts) * 60, sini*draandts * 60, (dmlonmdt - n) * 60);

		// ------------------ - numerically integrate 2nd ord sec / lp to current time-------------------------- -
		// Hoots uses an optimized RK4 - note the different coefficients
		dt = 86400.0 * floor(dtsec / 86400);  // sec, 1 day step size
		anew = a;
		for (i = 1; i <= 5; i++)
		{
			// process each of the mean orbital elements and update to current time
			switch (i)
			{
				// dtmin is the minutes from epoch (1st obs in .e file) to currobs time
			case 1:
				elemratet0 = dafdt;
				elemt0 = af;
				break;
			case 2:
				elemratet0 = dagdt;
				elemt0 = ag;
				break;
			case 3:
				elemratet0 = dchidt;
				elemt0 = chi;
				break;
			case 4:
				elemratet0 = dpsidt;
				elemt0 = psi;
				break;
			case 5:
				// n must go here!!  no
				elemratet0 = dmlonmdt;
				elemt0 = mlonm;
				break;
			case 6:
				// elemratet0 = dndt;
				elemratet0 = dadt;  // but this is 0
				elemt0 = a;
				break;
			}

			f1 = elemratet0;
			t0 = 0.0;  // assume starts at epoch

			// Could also use an optimized RK4 - Enright - note the different coefficients
			// assume that the rates are the same for each evaluation???
			//         t2 = t0 + 3.0 / 8.0 * dt;
			//         x2 = elemt0 + 3.0 / 8.0 * f1 * dt;
			//         f2 = elemratet2;
			//         t3 = t0 + 9.0 / 16.0 * dt;
			//         x3 = elemt0 + 9.0 / 16.0 * f2 * dt;
			//         f3 = elemratet3;
			//         t4 = t0 + 25.0 / 32.0 * dt;
			//         x4 = elemt0 + (-125.0 / 672.0 * f1 + 325.0 / 336.0 * f2) * dt;
			//         f4 = elemratet4;
			//         elemnew = elemt0 + (37.0 / 225.0 * f1 + 44.0 / 117.0 * f2 + 448.0 / 975.0 * f4) * dt;

			t2 = t0 + 0.5 * dt;
			x2 = elemt0 + 0.5 * f1 * dt;
			f2 = elemratet0;
			t3 = t0 + 0.5 * dt;
			x3 = elemt0 + 0.5 * f2 * dt;
			f3 = elemratet0;
			t4 = t0 + dt;
			x4 = elemt0 + f3 * dt;
			f4 = elemratet0;
			elemnew = elemt0 + 0.166666666666667 * (f1 + 2.0 * f2 + 2.0 * f3 + f4) * dt;

			// -----------------------  interpolate elems to exact time--------------------------- 
			//tneed = fmod(dtsec, 86400);
			//dd0 = pow(tneed - 1.0, 2) * (2.0 * tneed + 1.0);
			//dd1 = tneed * pow(tneed - 1.0, 2);
			//dd2 = 1.0 - dd0;
			//dd3 = pow(tneed, 2) * (tneed - 1.0);
			//elemnew = dd0 * elemt0 + dd1 * elemratet0 * dt + dd2 * elemnew + dd3 * elemratet0 * dt;  // or ratenew ? ?
			// does the fractional time come "after" the elemnew, or before it? elemt0 + or elemnew + ??
			//elemnew = elemt0 + elemratet0 * dtsec;

			// find mean equinoctial elements at ti
			switch (i)
			{
			case 1:
				afnew = elemnew;
				break;
			case 2:
				agnew = elemnew;
				break;
			case 3:
				chinew = elemnew;
				break;
			case 4:
				psinew = elemnew;
				break;
			case 5:
				mlonmnew = elemnew;
				break;
			case 6:
				anew = elemnew;
				break;
			}
		}  // for i through 6 orbital elements

		mlonmnew = fmod(mlonmnew, twopi);
		//	printf("eq_new  %12.9f %12.9f %12.9f %12.9f %12.9f %12.9f \n", anew, afnew, agnew, chinew, psinew, mlonmnew * rad);

		// chg updated mean equinoctial to mean classical at new time for periodic evaluation
		ast2Body::eq2rv(anew, afnew, agnew, chinew, psinew, mlonmnew, fr, rmean1, vmean1);
		if (show == 'y')
			printf("zon updated pos/vel  %11.6f %11.6f %11.6f %11.6f %11.6f %11.6f %12.3f \n", rmean1[0], rmean1[1], rmean1[2], vmean1[0], vmean1[1], vmean1[2], dtsec);

		ast2Body::rv2coe(rmean1, vmean1, mu, p1, a1, ecc1, incl1, raan1, argp1, nu1, m1, eccanom1, arglat1, truelon1, lonper1);
		if (show == 'y')
		{
			printf("Zon_eqmean updated  %12.9f %12.9f %12.9f %12.9f %12.9f %12.9f \n", anew, afnew, agnew, chinew, psinew, mlonmnew * 57.2957);
			printf("Zoncoes_new  %11.4f %11.4f %13.9f %13.7f %11.5f %11.5f %11.5f %11.5f %11.5f \n",
				p1, a1, ecc1, incl1 * 57.2957, raan1 * 57.2957, argp1 * 57.2957, nu1 * 57.2957, m1 * 57.2957, arglat1 * 57.2957);
		}
		
		// ----------------------------------periodic equations----------------------------
		dndtp = -1.5 * n * j2 * pow(re / a, 2) * (pow(a / magr, 3) * ((1.0 - 1.5 * sini * sini) + 1.5 * sini * sini * cos(2.0 * nu + 2.0 * argp))
			- (1.0 - 1.5 * sini * sini) * pow(beta2, -1.5));
		dedtp = 0.5 * j2 * pow(re / p, 2) * (1.0 - 1.5 * sini * sini) * (2.5 * ecc + (ecc - pow(ecc, 3)) / (1.0 + beta) + 3.0 * (1.0 - 0.25 * ecc * ecc) * cos(nu)
			+ 1.5 * ecc * cos(2.0 * nu) + 0.25 * ecc * ecc * cos(3.0 * nu))
			+ 3.0 / 8.0 * j2 * pow(re / p, 2) * sini * sini  *
			((1.0 + 11.0 / 4.0 * ecc * ecc) * cos(nu + 2.0 * argp)
			+ 0.25 * ecc * ecc * cos(2.0 * argp - nu) + 5.0 * ecc * cos(2.0 * argp + 2.0 * nu)
			+ 1.0 / 3.0 * (7.0 + 17.0 / 4.0 * ecc * ecc) * cos(3.0 * nu + 2.0 * argp)
			+ 1.5 * ecc * cos(4.0 * nu + 2.0 * argp) + 0.25 * ecc * ecc * cos(5.0 * nu + 2.0 * argp) + 1.5 * ecc * cos(2.0 * argp));
		didtp = 3.0 / 8.0 * j2 * pow(re / p, 2) * sin(2.0 * incl) * (ecc * cos(nu + 2.0 * argp) + cos(2.0 * nu + 2.0 * argp)
			+ ecc / 3.0 * cos(3.0 * nu + 2.0 * argp));
		draandtp = -1.5 * j2 * pow(re / p, 2) * cosi * (nu - m + ecc * sin(nu) - 0.5 * ecc * sin(nu + 2.0 * argp)
			- 0.5 * sin(2.0 * nu + 2.0 * argp) - ecc / 6.0 * sin(3.0 * nu + 2.0 * argp));
		dargpdtp = 0.75 * j2 * pow(re / p, 2) * (4.0 - 5.0 * sini * sini) * (nu - m + ecc * sin(nu))
			+ 1.5 * j2 * pow(re / p, 2) * (1.0 - 1.5 * sini * sini) * (1.0 / ecc * (1.0 - 0.25 * ecc * ecc) * sin(nu) + 0.5 * sin(2.0 * nu) + 1.0 / 12.0 * ecc * sin(3.0 * nu))
			- 1.5 * j2 * pow(re / p, 2) * (1.0 / ecc * (0.25 * sini * sini + 0.5 * ecc * ecc * (1.0 - 15.0 / 8.0 * sini * sini)) * sin(nu + 2.0 * argp))
			+ ecc / 16.0 * sini * sini * sin(2.0 * argp - nu) + 0.5 * (1.0 - 2.5 * sini * sini) * sin(2.0 * nu + 2.0 * argp)
			- 1.0 / ecc * (7.0 / 12.0 * sini * sini - ecc * ecc / 6.0 * (1.0 - 19.0 / 8.0 * sini * sini)) * sin(3.0 * nu + 2.0 * argp) - 3.0 * 8.0 * sini * sini * sin(4.0 * nu + 2.0 * argp)
			- 1.0 / 16.0 * ecc * sini * sini * sin(5.0 * nu + 2.0 * argp)
			- 9.0 / 16.0 * j2 * pow(re / p, 2) * sini * sini * sin(2.0 * argp);
		dmdtp = -1.5 * j2 * pow(re / p, 2) * beta / ecc * ((1.0 - 1.5 * sini * sini) * ((1.0 - 0.25 * ecc * ecc) * sin(nu) + 0.5 * ecc * sin(2.0 * nu) + ecc * ecc / 12.0 * sin(3.0 * nu))
			+ 0.5 * sini * sini * (-0.5 * (1.0 + 1.25 * ecc * ecc) * sin(nu + 2.0 * argp) - ecc * ecc / 8.0 * sin(2.0 * argp - nu)
			+ 7.0 / 6.0 * (1.0 - ecc * ecc / 28.0) * sin(3.0 * nu + 2.0 * argp) + 0.75 * ecc * sin(4.0 * nu + 2.0 * argp) + ecc * ecc / 8.0 * sin(5.0 * nu + 2.0 * argp)))
			+ 9.0 / 16.0 * j2 * pow(re / p, 2) * beta * sini * sini * sin(argp);

		if (show == 'y')
			printf("Zonal %11.8g %11.8g %11.8g %11.8g %11.8g %11.8g\n", dndtp, dedtp, didtp, draandtp, dargpdtp, dmdtp);

		// ------------ - periodic equations in equinoctial space----------------
		// find the osculating contribution to add to the mean sec / lp values in equinoctial space
		temp = 1.0 / (1.0 + cosi);
		dela = -2.0 / 3.0 * a * dndtp / n;
		delaf = dedtp * cos(argp + raan) - ecc * (dargpdtp + draandtp) * sin(argp + raan);
		delag = dedtp * sin(argp + raan) + ecc * (dargpdtp + draandtp) * cos(argp + raan);
		delchi = didtp * sin(raan) * temp + sini * draandtp * cos(raan) * temp;
		delpsi = didtp * cos(raan) * temp - sini * draandtp * sin(raan) * temp;
		delmlonm = dmdtp + dargpdtp + draandtp;

		// ---------- - updated osculating equinoctial elements--------------------
		a = anew + dela;
		af = afnew + delaf;
		ag = agnew + delag;
		chi = chinew + delchi;
		psi = psinew + delpsi;
		mlon = mlonmnew + delmlonm;

		ast2Body::eq2rv(a, af, ag, chi, psi, mlon, fr, rosc, vosc);
	}  // zonalperts


	/*  ------------------------------------------------------------------------------
	*
	*                            procedure dragperts
	*
	*   this procedure calculates the secular / lp avg rates and the osculating classical
	*     elements for a satellite from drag perturbations.
	*
	*   author        : david vallado                  719 - 573 - 2600     3 dec 2017
	*
	*   inputs          description                    range / units
	*     jdutc       - epoch julian date              days from 4713 BC
	*     jdutcF      - epoch julian date fraction     day fraction from jdutc
	*     rmean       - init mean position vector eci  km
	*     vmean       - init mean velocity vector eci  km / s
	*     dtsec       - desired time from 0.0 epoch    sec
	*     tut1        - julian centuries of ut1
	*     cdam        - cd * a / mass                    kg / m2
	*
	*   outputs       :
	*     rosc        - final osc position vector eci  km
	*     vosc        - final osc velocity vector eci  km / s
	*     rmean1      - final mean position vector eci  km
	*     vmean1      - final mean velocity vector eci  km / s
	*
	*   locals :
	*     rsun        - vector from earth to sun       km
	*
	*   coupling :
	*
	*   references :
	*     vallado       2013, ch 9
	*  chao/hoots       2018 (tbd)
	----------------------------------------------------------------------------   */

	void dragperts
		(
		double jdtdb, double jdtdbF, double rmean[3], double vmean[3],
		double dtsec,
		double tut1, double cdam, double rosc[3], double vosc[3], double rmean1[3], double vmean1[3]
		)
	{
		double temp, magr;
		double p, a, ecc, incl, raan, argp, nu, m, arglat, truelon, lonper, mlonm, mlonnu, n;
		double beta, eccanom;
		double beta2, eccanom1;
		double p1, a1, ecc1, incl1, raan1, argp1, nu1, m1, arglat1, truelon1, lonper1;
		double cosi, sini, cosraan, sinraan, sinargp, cosargp;
		double af, ag, chi, psi, mlon, afnew, agnew, chinew, psinew, mlonmnew, anew;
		double dndts, dedts, didts, draandts, dargpdts, dmdts;
		double dadt, dndt, dafdt, dagdt, dchidt, dpsidt, dmlonmdt, dt;
		double dela, delaf, delag, delchi, delpsi, delmlonm;
		double dndtp, dedtp, didtp, draandtp, dargpdtp, dmdtp;
		double elemt0, elemratet0, t0, f1;
		double t2, x2, f2, t3, x3, f3, t4, x4, f4, elemnew;
		double temp1, temp2, temp3, temp4, temp5, temp6, tempint, tempint1, vtemp, omegaearth;
		int i, fr;

		// -------------------- - find mean coes at epoch---------------------- -
		ast2Body::rv2coe(rmean, vmean, mu, p, a, ecc, incl, raan, argp, nu, m, eccanom, arglat, truelon, lonper);
		magr = astMath::mag(rmean);
		cosargp = cos(argp);
		sinargp = sin(argp);
		cosi = cos(incl);
		sini = sin(incl);
		cosraan = cos(raan);
		sinraan = sin(raan);
		beta2 = (1.0 - ecc * ecc);
		beta = sqrt(beta2); 

		// ----------------find mean equinoctials at epoch--------------------
		ast2Body::rv2eq(rmean, vmean, a, n, af, ag, chi, psi, mlonm, mlonnu, fr);

		// -------------- - 2nd order secular and lp equations------------------
		// ----in terms of kepler elements
		double rho = 0.0; // fix ---------------------------------------------------------
		double lod = 0.0;  // fix ---------------------------------------------------------
		omegaearth = 7.29211514670698e-05 * (1.0 - lod / 86400.0);

		temp1 = 1.0 / (1.0 + ecc * cos(nu));
		temp2 = temp1 * temp1;
		temp3 = (1.0 + 2.0 * ecc * cos(nu) + ecc * ecc);
		vtemp = sqrt(temp3) * (1.0 - (pow(beta2, 1.5) * omegaearth*cos(incl) / n) / temp3 );
		temp4 = rho * vtemp * temp2;
		temp5 = cdam / twopi;

		tempint = temp4 * (1.0 + ecc * cos(nu) + ecc * (cos(nu) + ecc) - omegaearth * pow(beta2, 1.5) / n * cos(incl));
		// integrate with Gaussian Quadrature



		dndts = 1.5 * n * n * a * temp5 * tempint;

		tempint = temp4 * (-2.0 * (ecc + cos(nu)) + omegaearth * pow(beta2, 1.5) / n * cos(incl) * (2.0 * cos(nu) + ecc + ecc * cos(nu) * cos(nu)) * temp2 );
		// integrate with Gaussian Quadrature



		dedts = 0.5 * n * p * temp5 * tempint;

		tempint = temp4 * temp2 * cos(arglat) * cos(arglat);
		// integrate with Gaussian Quadrature



		didts = -0.5 * omegaearth * a * pow(beta2, 2.5) / n * sin(incl) * temp5 * tempint;

		tempint = temp4 * temp2 * sin(arglat) * cos(arglat);
		// integrate with Gaussian Quadrature



		draandts = -0.5 * omegaearth * a * pow(beta2, 2.5) / n * temp5 * tempint;

		tempint1 = temp4 * sin(nu) * (1.0 - temp2 * omegaearth * pow(beta, 1.5) / n) * cos(incl);
		// integrate with Gaussian Quadrature



		temp6 = -temp5 * n * p / ecc * tempint1;
		tempint = temp4 * temp2 * (sin(nu) * cos(nu) + sin(arglat) * cos(arglat));
		// integrate with Gaussian Quadrature



		dargpdts = 0.5 * omegaearth * a * pow(beta2, 2.5) * temp5 * cos(incl) * tempint + temp6;
		
		tempint = temp4 * temp1 * (n * a * pow(beta2, 2.5) * ecc * sin(nu) - 
			0.5 * omegaearth * a * pow(beta2, 3) * cdam * sin(nu) * cos(nu) * temp2 * cos(incl) * temp4 );
		// integrate with Gaussian Quadrature



		// no n here
		dmdts = temp5 * tempint - beta * temp6;

		// -------------------------------------------------------------------- -
		// find avg rate of change in the equinoctial elements. use the 2nd order
		// secular and lp equations in kepler elements.these equations are the
		// rate in the numerical integration in terms of equinoctial elements.
		// -------------------------------------------------------------------- -
		temp = 1.0 / (1.0 + cosi);
		dadt = 0.0;
		dndt = 0.0;  // -1.5 * n / a * dadt3s;
		dafdt = dedts * cos(argp + raan) - ecc * (dargpdts + draandts) * sin(argp + raan);
		dagdt = dedts * sin(argp + raan) + ecc * (dargpdts + draandts) * cos(argp + raan);
		dchidt = didts * sinraan * temp + sini * draandts * cosraan * temp;
		dpsidt = didts * cosraan * temp - sini * draandts * sinraan * temp;
		dmlonmdt = dmdts + dargpdts + draandts;

		// ------------------ - numerically integrate 2nd ord sec / lp to current time-------------------------- -
		// Hoots uses an optimized RK4 - note the different coefficients
		dt = 86400.0 * floor(dtsec / 86400);  // sec, 1 day step size
		anew = a;
		for (i = 1; i <= 6; i++)
		{
			// process each of the mean orbital elements and update to current time
			switch (i)
			{
				// dtmin is the minutes from epoch (1st obs in .e file) to currobs time
			case 1:
				elemratet0 = dafdt;
				elemt0 = af;
				break;
			case 2:
				elemratet0 = dagdt;
				elemt0 = ag;
				break;
			case 3:
				elemratet0 = dchidt;
				elemt0 = chi;
				break;
			case 4:
				elemratet0 = dpsidt;
				elemt0 = psi;
				break;
			case 5:
				// elemratet0 = dndt;
				elemratet0 = dadt;  // but this is 0
				elemt0 = a;
				break;
			case 6:
				elemratet0 = n + dmlonmdt;
				elemt0 = mlonm;
				break;
			}

			f1 = elemratet0;
			t0 = 0.0;  // assume starts at epoch

			// Could also use an optimized RK4 - note the different coefficients
			// assume that the rates are the same for each evaluation???
			//         t2 = t0 + 3.0 / 8.0 * dt;
			//         x2 = elemt0 + 3.0 / 8.0 * f1 * dt;
			//         f2 = elemratet2;
			//         t3 = t0 + 9.0 / 16.0 * dt;
			//         x3 = elemt0 + 9.0 / 16.0 * f2 * dt;
			//         f3 = elemratet3;
			//         t4 = t0 + 25.0 / 32.0 * dt;
			//         x4 = elemt0 + (-125.0 / 672.0 * f1 + 325.0 / 336.0 * f2) * dt;
			//         f4 = elemratet4;
			//         elemnew = elemt0 + (37.0 / 225.0 * f1 + 44.0 / 117.0 * f2 + 448.0 / 975.0 * f4) * dt;

			t2 = t0 + 0.5 * dt;
			x2 = elemt0 + 0.5 * f1 * dt;
			f2 = elemratet0;
			t3 = t0 + 0.5 * dt;
			x3 = elemt0 + 0.5 * f2 * dt;
			f3 = elemratet0;
			t4 = t0 + dt;
			x4 = elemt0 + f3 * dt;
			f4 = elemratet0;
			elemnew = elemt0 + 0.166666666666667 * (f1 + 2.0 * f2 + 2.0 * f3 + f4) * dt;

			// -----------------------  interpolate elems to exact time--------------------------- 
			//tneed = fmod(dtsec, 86400);
			//dd0 = pow(tneed - 1.0, 2) * (2.0 * tneed + 1.0);
			//dd1 = tneed * pow(tneed - 1.0, 2);
			//dd2 = 1.0 - dd0;
			//dd3 = pow(tneed, 2) * (tneed - 1.0);
			//elemnew = dd0 * elemt0 + dd1 * elemratet0 * dt + dd2 * elemnew + dd3 * elemratet0 * dt;  // or ratenew ? ?
			// does the fractional time come "after" the elemnew, or before it? elemt0 + or elemnew + ??
			//elemnew = elemt0 + elemratet0 * dtsec;

			// find mean equinoctial elements at ti
			switch (i)
			{
			case 1:
				afnew = elemnew;
				break;
			case 2:
				agnew = elemnew;
				break;
			case 3:
				chinew = elemnew;
				break;
			case 4:
				psinew = elemnew;
				break;
			case 5:
				anew = elemnew;
				break;
			case 6:
				mlonmnew = elemnew;
				break;
			}
		}  // for i through 6 orbital elements

		mlonmnew = fmod(mlonmnew, twopi);
		//	printf("eq_new  %12.9f %12.9f %12.9f %12.9f %12.9f %12.9f \n", anew, afnew, agnew, chinew, psinew, mlonmnew * rad);

		// chg updated mean equinoctial to mean classical at new time for periodic evaluation
		ast2Body::eq2rv(anew, afnew, agnew, chinew, psinew, mlonmnew, fr, rmean1, vmean1);
		ast2Body::rv2coe(rmean1, vmean1, mu, p1, a1, ecc1, incl1, raan1, argp1, nu1, m1, eccanom1, arglat1, truelon1, lonper1);
		if (show == 'y')
		{
			printf("deq_mean  %12.9f %12.9f %12.9f %12.9f %12.9f %12.9f \n", anew, afnew, agnew, chinew, psinew, mlonmnew * 57.2957);
			printf("dcoes_new  %11.4f %11.4f %13.9f %13.7f %11.5f %11.5f %11.5f %11.5f %11.5f \n",
				p1, a1, ecc1, incl1 * 57.2957, raan1 * 57.2957, argp1 * 57.2957, nu1 * 57.2957, m1 * 57.2957, arglat1 * 57.2957);
		}

		// ----------------------------------periodic equations----------------------------
		dndtp = -1.5 * n * j2;
		dedtp = 0.5 * j2 * j2;
		didtp = 3.0 / 8.0 * j2;
		draandtp = -1.5 * j2;
		dargpdtp = 0.75 * j2;
		dmdtp = -1.5 * j2;

		//    fprintf(1, 'changes from drag (deg/day) dedtdidtsec %12.9f %12.9f dedtdidtper %11.8f  %11.8f \n', dedts * 86400.0, didts * rad * 86400.0, dedtp * 86400.0, didtp * rad * 86400.0);

		// ------------ - periodic equations in equinoctial space----------------
		// find the osculating contribution to add to the mean sec / lp values in equinoctial space
		temp = 1.0 / (1.0 + cosi);
		dela = -2.0 / 3.0 * a * dndtp / n;
		delaf = dedtp * cos(argp + raan) - ecc * (dargpdtp + draandtp) * sin(argp + raan);
		delag = dedtp * sin(argp + raan) + ecc * (dargpdtp + draandtp) * cos(argp + raan);
		delchi = didtp * sin(raan) * temp + sini * draandtp * cos(raan) * temp;
		delpsi = didtp * cos(raan) * temp - sini * draandtp * sin(raan) * temp;
		delmlonm = dmdtp + dargpdtp + draandtp;

		// ---------- - updated osculating equinoctial elements--------------------
		a = anew + dela;
		af = afnew + delaf;
		ag = agnew + delag;
		chi = chinew + delchi;
		psi = psinew + delpsi;
		mlon = mlonmnew + delmlonm;

		ast2Body::eq2rv(a, af, ag, chi, psi, mlon, fr, rosc, vosc);
	}  // dragperts


	/*  ------------------------------------------------------------------------------
	 * 
	 *                            procedure thirdbodyperts
	 * 
	 *   this procedure calculates the secular / lp avg rates and the osculating classical
	 *     elements for a satellite from srp perturbations.
	 * 
	 *   author        : david vallado                  719 - 573 - 2600     3 dec 2017
	 * 
	 *   inputs          description                    range / units
	 *     jdtdb       - epoch julian date              days from 4713 BC
	 *     jdtdbF      - epoch julian date fraction     day fraction from jdtdb
	 *     rmean       - init mean position vector eci  km
	 *     vmean       - init mean velocity vector eci  km / s
	 *     rsun        - vector from earth to sun eci   km
	 *     rmoon       - vector from earth to moon eci  km
	 *     dtsec       - desired time from 0.0 epoch    sec
	 *     tut1        - julian centuries of ut1
	 * 
	 *   outputs       :
	 *     rosc        - final osc position vector eci  km
	 *     vosc        - final osc velocity vector eci  km / s
	 *     rmean1      - final mean position vector eci  km
	 *     vmean1      - final mean velocity vector eci  km / s
	 * 
	 *   locals :
	 * 
	 *   coupling :
	 *     ast2Body::sunmoonjpl, sun, rv2coe, rv2eq, eq2rv
	 * 
	 *   references :
	 *     vallado       2013, ch 9
	 *  chao/hoots       2018 (tbd)
	----------------------------------------------------------------------------   */

	void thirdbodyperts
		(
		double jdtdb, double jdtdbF, double rmean[3], double vmean[3],
		char interp,
		const std::vector<jpldedata> &jpldearr,
		double jdjpldestart,
		double dtsec,
		double tut1, double rosc[3], double vosc[3], double rmean1[3], double vmean1[3]
		)
	{
		double temp, magr, rsun[3], rmoon[3], rsmag;
		double p, a, ecc, incl, raan, argp, nu, m, arglat, truelon, lonper, mlonm, mlonnu, n;
		double beta, eccanom;
		double beta1, eccanom1;
		double p1, a1, n1, ecc1, incl1, raan1, argp1, nu1, m1, arglat1, truelon1, lonper1;
		double cosi, sini, cosraan, sinraan, cosargp, sinargp;
		double af, ag, chi, psi, afnew, agnew, chinew, psinew, mlonmnew, anew, nnew, chi1, psi1, mlon1, af1, ag1;
		double cosi1, sini1;
		double dndts, dedts, didts, draandts, dargpdts, dmdts, dadt3s;
		double dadt, dndt, dafdt, dagdt, dchidt, dpsidt, dmlonmdt, dt, x1, dedtss, didtss, draandtss, dargpdtss, dmdtss;
		double dedtsm, didtsm, draandtsm, dargpdtsm, dmdtsm;
		double deln, delaf, delag, delchi, delpsi, delmlonm, del1, del2, e11, e21, e31, cosargp1, sinargp1, cosraan1, sinraan1;
		double dndtp, dedtp, didtp, draandtp, dargpdtp, dmdtp, dndtss, dndtsm, dndt3s, dedt3s, didt3s, draandt3s, dargpdt3s, dmdt3s;
		double q11, q12, q13, q21, q22, q23, q31, q32, q33;
		double u3vec[3], mu3, mass3, magr3, e1, e2, e3, mrat3, mrat4;
		double elemt0, elemratet0, t0, f1, temp1, dndt3p, dedt3p, didt3p, draandt3p, dargpdt3p, dmdt3p;
		double dndtps, dedtps, didtps, draandtps, dargpdtps, dmdtps, dndtpm, dedtpm, didtpm, draandtpm, dargpdtpm, dmdtpm;
		double t2, x2, f2, t3, x3, f3, t4, x4, f4, elemnew;
		int i, fr, ithird;

		double rad = 180.0 / pi;
		double dayconv = 1.0 / 86400.0;
		double m3s = 1.9891e30;     // sun kg
		double massE = 5.9742e24;     // earth kg
		double m3m = 7.3483e22;     // moon kg

		// -------------------- - find mean coes at epoch---------------------- -
		ast2Body::rv2coe(rmean, vmean, mu, p, a, ecc, incl, raan, argp, nu, m, eccanom, arglat, truelon, lonper);
		if (show == 'y')
			printf("coes_init %11.4f %11.4f %13.9f %13.7f %11.5f %11.5f %11.5f %11.5f %11.5f \n",
			p, a, ecc, incl * rad, raan * rad, argp * rad, nu * rad, m * rad, arglat * rad);
		magr = astMath::mag(rmean);
		beta = sqrt(1.0 - ecc * ecc);

		cosargp = cos(argp);
		sinargp = sin(argp);
		cosi = cos(incl);
		sini = sin(incl);
		cosraan = cos(raan);
		sinraan = sin(raan);

		// transform coordinates eci to pqw
		q11 = cosargp * cosraan - sinargp * sinraan * cosi;
		q12 = cosargp * sinraan + sinargp * cosraan * cosi;
		q13 = sinargp * sini;
		q21 = -sinargp * cosraan - cosargp * sinraan * cosi;
		q22 = -sinargp * sinraan + cosargp * cosraan * cosi;
		q23 = cosargp * sini;
		q31 = sinraan * sini;
		q32 = -cosraan * sini;
		q33 = cosi;

		// ----------------find mean equinoctials at epoch--------------------
		ast2Body::rv2eq(rmean, vmean, a, n, af, ag, chi, psi, mlonm, mlonnu, fr);
		if (show == 'y')
			printf("eq_init  %12.9f %12.9f %12.9f %12.9f %12.9f %12.9f n %12.9f \n", a, af, ag, chi, psi, mlonm * rad, n);

		// ------------------ initial sun and moon position vectors ------------------
		ast2Body::findjpldeparam(jdtdb, jdtdbF, interp, jpldearr, jdjpldestart, rsun, rsmag, rmoon);
		//printf("sun l\n%lf %lf  %lf %lf %lf   %lf %lf %lf \n", jdtdb, jdtdbF, rsun[0], rsun[1], rsun[2], rmoon[0], rmoon[1], rmoon[2]);
		for (ithird = 1; ithird <= 2; ithird++)
		{
			// calculate for both sun and moon
			if (ithird == 1)
			{
				astMath::norm(rsun, u3vec);
				mu3 = muSun;  // km3 / s2
				mass3 = m3s;  // kg
				magr3 = astMath::mag(rsun);
			}
			else
			{
				astMath::norm(rmoon, u3vec);
				mu3 = muMoon;
				mass3 = m3m;
				magr3 = astMath::mag(rmoon);
			}

			mrat3 = (mass3 / massE) * pow(a / magr3, 3);  // kg / kg km / km->rad
			mrat4 = (mass3 / massE) * pow(a / magr3, 4);

			e1 = q11 * u3vec[0] + q12 * u3vec[1] + q13 * u3vec[2];
			e2 = q21 * u3vec[0] + q22 * u3vec[1] + q23 * u3vec[2];
			e3 = q31 * u3vec[0] + q32 * u3vec[1] + q33 * u3vec[2];
			//fprintf(1, 'e123 %11.7f %11.7f %11.7f \n', e1, e2, e3);

			// second order terms
			//         fr = mu3 / (magr3 ^ 2) * (magr / magr3) * (3.0 * (e2 * e2 + (e1 * e1 - e2 * e2) * cos(nu) ^ 2 + 2.0 * e1 * e2 * sin(nu) * cos(nu)) - 1.0) +  
			//              1.5 * mu3 / (magr3 ^ 2) * (magr / magr3) ^ 2 * (3.0 * e1 * (5.0 * e2 * e2 - 1.0) * cos(nu) + 5.0 * e1 * (e1 * e1 - 3.0 * e2 * e2) * cos(nu) ^ 2 +  
			//              5.0 * e2 * (3.0 * e1 * e1 - e2 * e2) * sin(nu) * cos(nu) ^ 2 + e2 * (5.0 * e2 * e2 - 3.0) * sin(nu));
			//         ft = 3.0 * mu3 / (magr3 ^ 2) * (magr / magr3) * ((e1 * e1 - e2 * e2) * sin(nu) * cos(nu) + 2.0 * e1 * e2 * cos(nu) ^ 2 - e1 * e2) +  
			//              1.5 * mu3 / (magr3 ^ 2) * (magr / magr3) ^ 2 * (-e2 * (1.0 + 10.0 * e1 * e1 - 5.0 * e2 * e2) * cos(nu) + 5.0 * e2 * (3.0 * e1 * e1 - e2 * e2) * cos(nu) ^ 3 -  
			//              5.0 * e1 * (e1 * e1 - 3.0 * e2 * e2) * sin(nu) * cos(nu) ^ 2 - e1 * (5.0 * e2 * e2 - 1.0) * sin(nu));
			//         fn = 3.0 * mu3 / (magr3 ^ 2) * (magr / magr3) * e3  * (e1 * cos(nu) + e2 * sin(nu)) +  
			//              1.5 * mu3 / (magr3 ^ 2) * (magr / magr3) ^ 2 * e3 * ((5.0 * e2 * e2 - 1.0) + 5.0 * (e1 * e1 - e2 * e2) * cos(nu) ^ 2 + 10.0 * e1 * e2 * sin(nu) * cos(nu));

			// --------------lpeom equations of variation 3rd body-------------------------------- -
			//         // ----note these are not used in the semianalytical approach because
			//         // ----they are in the 2nd order secular and lp equations
			//         dndts = -3.0 / (a * beta) * (ecc * sin(nu) * fr + p / magr * ft);
			//         dadts = 2.0 / (n * beta) * (ecc * sin(nu) * fr + (a * beta * beta / magr) * ft);
			//         dedts = beta / (n * a) * (fr * sin(nu) + ((magr + a * beta * beta) * cos(nu) + ecc * magr) / (a * beta * beta) * ft);
			//         didts = magr / (n * a ^ 2 * beta) * fn * cos(nu + argp);
			//         draandts = magr / (n * a ^ 2 * beta * sini) * fn * sin(nu + argp);
			//         dargpdts = -draandt * cosi + beta / (n * a * ecc) * (-fr * cos(nu) + ft * sin(nu) * (1.0 + magr / (a * beta * beta)));
			//         dmdts = n - 2.0 * magr / (n * a ^ 2) * fr - beta * (dargpdt + draandt * cosi);

			// -------------- 2nd order secular and lp equations in keplerian elements-------------------- -
			dndts = 0.0;
			dedts = -7.5 * ecc * beta * n * mrat3 * e1 * e2
				- 15.0 / 16.0 * beta * n * mrat4 * e2 *
				((4.0 + 3.0 * ecc * ecc) - 5.0 * (1.0 + 6.0 * ecc * ecc) * e1 * e1 - 5.0 * (1.0 - ecc * ecc) * e2 * e2);  // / s
			didts = 1.5 * n / beta * mrat3 * e3 * (e1 * (1.0 + 4.0 * ecc * ecc) * cosargp - e2 * (1.0 - ecc * ecc) * sinargp) +
				15.0 / 16.0 * ecc * n / beta * mrat4 * e3 * (10.0 * e1 * e2 * (1.0 - ecc * ecc) * sinargp - 5.0 * e2 * e2 * (1.0 - ecc * ecc) * cosargp +
				(4.0 + 3.0 * ecc * ecc) * cosargp - 5.0 * e1 * e1 * (3.0 + 4.0 * ecc * ecc) * cosargp);  // / s
			draandts = 1.5 * n / (beta * sini) * mrat3 * e3 * (e2 * (1.0 - ecc * ecc) * cosargp + e1 * (1.0 + 4.0 * ecc * ecc) * sinargp) 
				- 15.0 / 16.0 * ecc * n / (beta * sini) * mrat4 * e3 * (10.0 * e1 * e2 * beta * beta * cosargp + 5.0 * e2 * e2 * beta * beta * sinargp
				- (4.0 + 3.0 * ecc * ecc) * sinargp + 5.0 * e1 * e1 * (3.0 + 4.0 * ecc * ecc) * sinargp);  // / s
			x1 = 1.5 * ecc * beta * n * mrat3 * (4.0 * e1 * e1 - e2 * e2 - 1.0) 
				- 15.0 / 16.0 * beta * n * mrat4 * e1 * (5.0 * e2 * e2 * (1.0 - 3.0 * ecc * ecc) - (4.0 + 9.0 * ecc * ecc) + 5.0 * e1 * e1 * (1.0 + 4.0 * ecc * ecc));  // / s
			dargpdts = -draandts * cosi + 1.0 / ecc * x1;  // / s
			// no n here, no need it 
			x2 = n - 3.0 * n * mrat3 * (e1 * e1 * (1.0 + 4.0 * ecc * ecc) + e2 * e2 * (1.0 - ecc * ecc) - (2.0 / 3.0 + ecc * ecc)) 
				+ 15.0 / 8.0 * ecc * n * mrat4 * e1 * (15.0 * e2 * e2 * (1.0 - ecc * ecc) - 3.0 * (4.0 + 3.0 * ecc * ecc) + 5.0 * e1 * e1 * (3.0 + 4.0 * ecc * ecc)); // / s
			dmdts = -beta * (dargpdts + draandts * cosi) + x2; // / s  add n here no, do in element update? ? ? , and not in periodic!

			if (ithird == 1)
			{
				dndtss = 0.0;  // dndts;    // sun sec and lp
				dedtss = dedts;
				didtss = didts;
				draandtss = draandts;
				dargpdtss = dargpdts;
				dmdtss = dmdts;
				if (show == 'y')
					printf("3bdy1st %1i %11.8g %11.8g %11.8g %11.8g %11.8g %11.8g\n", ithird, dndts, dedts, didts, draandts, dargpdts, dmdts);
			}
			else
			{
				dndtsm = 0.0;  // dndts;    // moon sec and lp
				dedtsm = dedts;
				didtsm = didts;
				draandtsm = draandts;
				dargpdtsm = dargpdts;
				dmdtsm = dmdts;
				if (show == 'y')
					printf("3bdy2nd %1i %11.8g %11.8g %11.8g %11.8g %11.8g %11.8g\n", ithird, dndts, dedts, didts, draandts, dargpdts, dmdts);
			}
		}  // for ithird secular through both sun and moon

		// ----sum the third body secular effects from sun and moon
		dndt3s = 0.0;  // dndtss + dndtsm;
		dadt3s = 0.0;
		dedt3s = dedtss + dedtsm;
		didt3s = didtss + didtsm;
		draandt3s = draandtss + draandtsm;
		dargpdt3s = dargpdtss + dargpdtsm;
		dmdt3s = dmdtss + dmdtsm;

		// now integrate the elements into the current time
		// -------------------------------------------------------------------- -
		// find avg rate of change in the equinoctial elements.use the 2nd order
		// secular and lp equations in kepler elements.these equations are the
		// rate in the numerical integration in terms of equinoctial elements.
		// -------------------------------------------------------------------- -
		temp = 1.0 / (1.0 + cosi);
		//dadt = -2.0 / 3.0 * a * dndts / n;
		//dndt = -1.5 * n * dadts / a;
		dadt = 0.0;
		dndt = 0.0;
		//dafdt = dedt3s * cos(argp + raan) - ecc * (dargpdt3s + draandt3s) * sin(argp + raan);
		// account for small ecc, incl
		temp1 = x1 + ecc * sini * draandt3s * sini * temp;
		dafdt = dedt3s * cos(argp + raan) - temp1 * sin(argp + raan);
		//dagdt = dedt3s * sin(argp + raan) + ecc * (dargpdt3s + draandt3s) * cos(argp + raan);
		dagdt = dedt3s * sin(argp + raan) + temp1 * cos(argp + raan);
		dchidt = didt3s * sinraan * temp + draandt3s * sini * cosraan * temp;
		dpsidt = didt3s * cosraan * temp - draandt3s * sini * sinraan * temp;
		//dmlonmdt = dmdt3s + dargpdt3s + draandt3s;
		dmlonmdt = x2 + x1 * ecc / (1.0 + beta) + sin(incl) * draandt3s * sin(incl) * temp;

		if (show == 'y')
			printf("3coe_rate (rad/min) %11.7g %11.7g %11.7g %11.7g %11.7g %11.7g \n", dndt3s * 60, dedt3s * 60, didt3s * 60, ecc*(draandt3s + dargpdt3s) * 60, sini*draandt3s * 60, (dmlonmdt - n) * 60);
		printf("3coe_rate (rad/min) %11.7g %11.7g %11.7g %11.7g %11.7g %11.7g \n", dndt3s * 60, dedt3s * 60, didt3s * 60, ecc*(draandt3s + dargpdt3s) * 60, sini*draandt3s * 60, (dmlonmdt-n) * 60);

		if (show == 'y')
			printf("3eq_rate %11.7g %11.7g %11.7g %11.7g %11.7g %11.7g \n", dndt, dafdt, dagdt, dchidt, dpsidt, dmlonmdt);

		// ------------------ - numerically integrate 2nd ord sec / lp to current time-------------------------- -
		// use an optimized RK4 - note the different coefficients
		dt = 86400.0 * floor(dtsec * dayconv);  // sec, 1 day step size
		// note that n stays constant, but be sure to assign it
		nnew = n;
		anew = a;
		for (i = 1; i <= 5; i++)
		{
			// process each of the mean orbital elements and update to current time
			switch (i)
			{
				// dtmin is the minutes from epoch (1st obs in .e file) to currobs time
			case 1:
				elemratet0 = dafdt;
				elemt0 = af;
				break;
			case 2:
				elemratet0 = dagdt;
				elemt0 = ag;
				break;
			case 3:
				elemratet0 = dchidt;
				elemt0 = chi;
				break;
			case 4:
				elemratet0 = dpsidt;
				elemt0 = psi;
				break;
			case 5:
				// put n in here for 2-body motion, no
				elemratet0 = dmlonmdt;
				elemt0 = mlonm;
				break;
			case 6:
				// elemratet0 = dndt;
				elemratet0 = dadt;  // but this is 0
				elemt0 = a;
				break;
			}

			f1 = elemratet0;
			t0 = 0.0;  // assume starts at epoch

			// Could also use an optimized RK4 - note the different coefficients
			// assume that the rates are the same for each evaluation???
			//         t2 = t0 + 3.0 / 8.0 * dt;
			//         x2 = elemt0 + 3.0 / 8.0 * f1 * dt;
			//         f2 = elemratet2;
			//         t3 = t0 + 9.0 / 16.0 * dt;
			//         x3 = elemt0 + 9.0 / 16.0 * f2 * dt;
			//         f3 = elemratet3;
			//         t4 = t0 + 25.0 / 32.0 * dt;
			//         x4 = elemt0 + (-125.0 / 672.0 * f1 + 325.0 / 336.0 * f2) * dt;
			//         f4 = elemratet4;
			//         elemnew = elemt0 + (37.0 / 225.0 * f1 + 44.0 / 117.0 * f2 + 448.0 / 975.0 * f4) * dt;

			t2 = t0 + 0.5 * dt;
			x2 = elemt0 + 0.5 * f1 * dt;
			f2 = elemratet0;
			t3 = t0 + 0.5 * dt;
			x3 = elemt0 + 0.5 * f2 * dt;
			f3 = elemratet0;
			t4 = t0 + dt;
			x4 = elemt0 + f3 * dt;
			f4 = elemratet0;
			elemnew = elemt0 + 0.166666666666667 * (f1 + 2.0 * f2 + 2.0 * f3 + f4) * dt;

			// -----------------------  interpolate elems to exact time--------------------------- 
			//tneed = fmod(dtsec, 86400);
			//dd0 = pow(tneed - 1.0, 2) * (2.0 * tneed + 1.0);
			//dd1 = tneed * pow(tneed - 1.0, 2);
			//dd2 = 1.0 - dd0;
			//dd3 = pow(tneed, 2) * (tneed - 1.0);
			//elemnew = dd0 * elemt0 + dd1 * elemratet0 * dt + dd2 * elemnew + dd3 * elemratet0 * dt;  // or ratenew ? ?
			// does the fractional time come "after" the elemnew, or before it? elemt0 + or elemnew + ??
			//elemnew = elemt0 + elemratet0 * dtsec;

			// find mean equinoctial elements at ti
			switch (i)
			{
			case 1:
				afnew = elemnew;
				break;
			case 2:
				agnew = elemnew;
				break;
			case 3:
				chinew = elemnew;
				break;
			case 4:
				psinew = elemnew;
				break;
			case 5:
				mlonmnew = elemnew;
				break;
			case 6:
				anew = elemnew;
				break;
			}
		}  // for i through 5 orbital elements

		mlonmnew = fmod(mlonmnew, twopi);
		//	printf("eq_new  %12.9f %12.9f %12.9f %12.9f %12.9f %12.9f \n", anew, afnew, agnew, chinew, psinew, mlonmnew * rad);

		// chg updated mean equinoctial to mean classical at new time for periodic evaluation
		ast2Body::eq2rv(anew, afnew, agnew, chinew, psinew, mlonmnew, fr, rmean1, vmean1);
		if (show == 'y')
			printf("3bdy updated pos/vel  %11.6f %11.6f %11.6f %11.6f %11.6f %11.6f %12.3f \n", rmean1[0], rmean1[1], rmean1[2], vmean1[0], vmean1[1], vmean1[2], dtsec);

		ast2Body::rv2coe(rmean1, vmean1, mu, p1, a1, ecc1, incl1, raan1, argp1, nu1, m1, eccanom1, arglat1, truelon1, lonper1);
		if (show == 'y')
		{
			printf("3eq_mean updated  %12.9f %12.9f %12.9f %12.9f %12.9f %12.9f \n", anew, afnew, agnew, chinew, psinew, mlonmnew * rad);
			printf("3coes_new  %11.4f %11.4f %13.9f %13.7f %11.5f %11.5f %11.5f %11.5f %11.5f \n",
				p1, a1, ecc1, incl1 * rad, raan1 * rad, argp1 * rad, nu1 * rad, m1 * rad, arglat1 * rad);
		}

		beta1 = sqrt(1.0 - ecc1 * ecc1);
		n1 = sqrt(mu / (a1 * a1 * a1));

		cosargp1 = cos(argp1);
		sinargp1 = sin(argp1);
		cosi1 = cos(incl1);
		sini1 = sin(incl1);
		cosraan1 = cos(raan1);
		sinraan1 = sin(raan1);

		q11 = cosargp1 * cosraan1 - sinargp1 * sinraan1 * cosi1;
		q12 = cosargp1 * sinraan1 + sinargp1 * cosraan1 * cosi1;
		q13 = sinargp1 * sini1;
		q21 = -sinargp1 * cosraan1 - cosargp1 * sinraan1 * cosi1;
		q22 = -sinargp1 * sinraan1 + cosargp1 * cosraan1 * cosi1;
		q23 = cosargp1 * sini1;
		q31 = sinraan1 * sini1;
		q32 = -cosraan1 * sini1;
		q33 = cosi1;

		// ----------------- need updated sun and moon position vectors!! ------------------------
		ast2Body::findjpldeparam(jdtdb + dt * dayconv, jdtdbF, interp, jpldearr, jdjpldestart, rsun, rsmag, rmoon);
		//		printf("sun l\n%lf %lf  %lf %lf %lf   %lf %lf %lf \n", jdtdb, jdtdbF, rsun[0], rsun[1], rsun[2]);

		// now do periodic terms for both sun and moon
		for (ithird = 1; ithird <= 2; ithird++)
		{
			if (ithird == 1)
			{
				astMath::norm(rsun, u3vec);
				mu3 = muSun;  // km3 / s2
				mass3 = m3s;  // kg
				magr3 = astMath::mag(rsun);  
			}
			else
			{
				astMath::norm(rmoon, u3vec);
				mu3 = muMoon;
				mass3 = m3m;
				magr3 = astMath::mag(rmoon);  
			}
			mrat3 = (mass3 / massE) * pow(a1 / magr3, 3);  // kg / kg km / km->rad
			mrat4 = (mass3 / massE) * pow(a1 / magr3, 4);

			e11 = q11 * u3vec[0] + q12 * u3vec[1] + q13 * u3vec[2];
			e21 = q21 * u3vec[0] + q22 * u3vec[1] + q23 * u3vec[2];
			e31 = q31 * u3vec[0] + q32 * u3vec[1] + q33 * u3vec[2];

			// ----------------------------------periodic equations----------------------------
			dndtp = -3.0 * n1 * mrat3 * (-3.0 * ecc1 * beta1 * e11 * e21 * sin(eccanom1) * sin(2.0 * eccanom1) + 1.5 * e11 * e21 * beta1 * sin(2.0 * eccanom1) -
				ecc1 * (3.0 * e11 * e11 - 1.0) * cos(eccanom1) + 0.25 * (3.0 * (e11 * e11 - e21 * e21) + (3.0 * e21 * e21 - 1.0) * ecc1 * ecc1) * cos(2.0 * eccanom1)) -
				9.0 / 8.0 * n1 / beta1 * mrat4 * (e21 * (-4.0 + 3.0 * ecc1 * ecc1 + pow(ecc1, 4) + 5.0 * e11 * e11 + 15.0 * ecc1 * ecc1 * e11 * e11 + 5.0 * e21 * e21 - 10.0 * ecc1 * ecc1 * e21 * e21 + 5.0 * pow(ecc1, 4) * e21 * e21 - 20.0 * pow(ecc1, 4) * e11 * e11) * sin(eccanom1) +
				4.0 * ecc1 * e21 * (1.0 - ecc1 * ecc1) * (1.0 - 5.0 * e11 * e11) * sin(2.0 * eccanom1) +
				1.0 / 3.0 * e21 * beta1 * beta1 * (5.0 * (3.0 * e11 * e11 - e21 * e21) + ecc1 * ecc1 * (5.0 * e11 * e11 - 3.0)) * sin(3.0 * eccanom1) +
				beta1 * e11 * ((5.0 * e21 * e21 - 1.0) * (4.0 + 11.0 * ecc1 * ecc1) + 5.0 * (e11 * e11 - 3.0 * e21 * e21) * (1.0 + 4.0 * ecc1 * ecc1)) * cos(eccanom1) -
				2.0 * beta1 * ecc1 * e11 * ((5.0 * e21 * e21 - 1.0) * (2.0 + ecc1 * ecc1) + 5.0 * (e11 * e11 - 3.0 * e21 * e21)) * cos(2.0 * eccanom1) +
				1.0 / 3.0 * beta1 * e11 * (5.0 * (e11 * e11 - 3.0 * e21 * e21) + 3.0 * ecc1 * ecc1 * (5.0 * ecc1 * ecc1 - 1.0)) * cos(3.0 * eccanom1));
			// all the rest are unitless
			dedtp = -7.5 * ecc1  * beta1 * mrat3 * ecc1  * beta1  * e11  * e21  * (eccanom1 - m1) +
				2.25 * mrat3 * beta1  * e11  * e21  * ((2.0 + 3.0 * ecc1 * ecc1) * sin(eccanom1) - ecc1  * sin(2.0 * eccanom1) + 1.0 / 9.0 * (2.0 - ecc1 * ecc1) * sin(3.0 * eccanom1)) +
				0.25 * mrat3 * beta1 * beta1 * (3.0 * cos(eccanom1) * (e11 * e11 - 5.0 * e21 * e21 + 4.0 / 3.0) + ecc1  * (6.0 * e21 * e21 - 3.0 * e11 * e11 - 1.0) * cos(2.0 * eccanom1) - (e21 * e21 - e11 * e11) * cos(3.0 * eccanom1)) -
				1.5 * mrat4 * (beta1  * ecc1  * e11  * (2.5 * (e11 * e11 - 3.0 * e21 * e21) * 1.0 - 4.0 * ecc1 * ecc1) - 2.0 * (5.0 * e21 * e21 - 1.0) -
				1.0 / 8.0 * beta1  * e11  * cos(2.0 * eccanom1) * (5.0 * (e11 * e11 - 3.0 * e21 * e21) * (1.0 + 2.0 * ecc1 * ecc1) - (5.0 * e21 * e21 - 1.0) * (2.0 + 3.0 * ecc1 * ecc1)) +
				1.0 / 6.0 * beta1  * ecc1  * e11  * cos(3.0 * eccanom1) * (5.0 * (e11 * e11 - 3.0 * e21 * e21)) -
				1.0 / 32.0 * beta1  * e11  * cos(4.0 * eccanom1) * (5.0 * (e11 * e11 - 3.0 * e21 * e21) + (5.0 * e21 * e21 - 1.0) * ecc1 * ecc1) +
				5.0 / 8.0 * e21  * (eccanom1 - m1) * ((1.0 + 10.0 * e11 * e11 - 5.0 * e21 * e21) * (4.0 + 3.0 * ecc1 * ecc1) - 5.0 * (3.0 * e11 * e11 - e21 * e21) * (3.0 - 4.0 * ecc1 * ecc1)) +
				0.25 * ecc1  * e21  * sin(eccanom1) * (-24.0 - 4.0 * ecc1 * ecc1 + 90.0 * e11 * e11 + 50.0 * ecc1 * ecc1 * e11 * e11 + 10.0 * e21 * e21 - 10.0 * ecc1 * ecc1 * e21 * e21) +
				0.25 * e21  * sin(2.0 * eccanom1) * (-1.0 + 8.0 * ecc1 * ecc1 - 10.0 * e11 * e11 + 5.0 * e21 * e21 - 25.0 * ecc1 * ecc1 * e11 * e11 - 5.0 * ecc1 * ecc1 * e21 * e21 + 15.0 * ecc1 * ecc1 * e21 * e21) +
				1.0 / 12.0 * ecc1  * e21  * sin(3.0 * eccanom1) * ((-3.0 + ecc1 * ecc1) * (1.0 + 10.0 * e11 * e11 - 5.0 * e21 * e21) + 15.0 * (3.0 * e11 * e11 - e21 * e21) + 3.0 -
				3.0 * ecc1 * ecc1 + 15.0 * e11 * e11 - 10.0 * e21 * e21 + 5.0 * ecc1 * ecc1 * e21 * e21) +
				1.0 / 32.0 * e21  * sin(4.0 * eccanom1) * (ecc1 * ecc1 * (1.0 + 10.0 * e11 * e11 - 5.0 * e21 * e21) - 5.0 * (3.0 * e11 * e11 - e21 * e21)));

			del1 = -5.0 / 8.0 * (5.0 * e21 * e21 - 1.0) * ecc1  * (4.0 + 3.0 * ecc1 * ecc1) * (eccanom1 - m1) - 25.0 / 8.0 * (e11 * e11 - e21 * e21) * ecc1  * (3.0 + 4.0 * ecc1 * ecc1) * (eccanom1 - m1) +
				(5.0 * e21 * e21 - 1.0) * (1.0 + 21.0 / 4.0 * ecc1 * ecc1 + 0.75 * pow(ecc1, 4)) * sin(eccanom1) + 5.0 * (e11 * e11 - e21 * e21) * (0.75 + 21.0 / 4.0 * ecc1 * ecc1 - pow(ecc1, 4)) * sin(eccanom1) -
				0.25 * (5.0 * e21 * e21 - 1.0) * ecc1  * (3.0 + 4.0 * ecc1 * ecc1) * sin(2.0 * eccanom1) - 5.0 / 4.0 * (e11 * e11 - e21 * e21) * ecc1  * (4.0 + 3.0 * ecc1 * ecc1) * sin(2.0 * eccanom1) +
				1.0 / 12.0 * sin(3.0 * eccanom1) * ((5.0 * e21 * e21 - 1.0) * (3.0 * ecc1 * ecc1 + pow(ecc1, 4)) + 5.0 * (e11 * e11 - e21 * e21) * (1.0 - 3.0 * ecc1 * ecc1)) -
				1.0 / 32.0 * sin(4.0 * eccanom1) * ((1.0 + 2.0 * ecc1 * ecc1) * ecc1 * ecc1 + 5.0 * (e11 * e11 - e21 * e21)) +
				2.5 * beta1  * e11  * e21  * (-(1.0 + 6.0 * ecc1 * ecc1) * cos(eccanom1) + ecc1  * (2.5 + ecc1 * ecc1) * cos(2.0 * eccanom1) - 1.0 / 3.0 * (1.0 + 2.0 * ecc1 * ecc1) * cos(3.0 * eccanom1) + 1.0 / 8.0 * ecc1  * cos(4.0 * eccanom1));

			del2 = -0.25 * beta1  * ((5.0 * e21 * e21 - 1.0) * (4.0 + 3.0 * ecc1 * ecc1) + 5.0 * (e11 * e11 - e21 * e21) * (1.0 + 2.0 * ecc1 * ecc1)) * cos(eccanom1) +
				1.0 / 8.0 * ecc1  * beta1  * ((5.0 * e21 * e21 - 1.0) * (6.0 + ecc1 * ecc1) + 5.0 * (e11 * e11 - e21 * e21) * (3.0 + 2.0 * ecc1 * ecc1)) * cos(2.0 * eccanom1) -
				1.0 / 12.0 * beta1  * ((5.0 * e21 * e21 - 1.0) * 3.0 * ecc1 * ecc1 + 5.0 * (e11 * e11 - e21 * e21) * (1.0 + 2.0 * ecc1 * ecc1)) * cos(3.0 * eccanom1) +
				1.0 / 32.0 * ecc1  * beta1  * ((5.0 * e21 * e21 - 1.0) * ecc1 * ecc1 + 5.0 * (e11 * e11 - e21 * e21)) * cos(4.0 * eccanom1) +
				10.0 * e11  * e21  * (1.0 - ecc1 * ecc1) * (-5.0 / 8.0 * (eccanom1 - m1) + 0.25 * (1.0 + ecc1 * ecc1) * sin(eccanom1) +
				0.25 * ecc1  * sin(2.0 * eccanom1) - 1.0 / 12.0 * (1.0 + ecc1 * ecc1) * sin(3.0 * eccanom1) + 1.0 / 32.0 * ecc1  * sin(4.0 * eccanom1));

			didtp = 1.5 / beta1  * mrat3 * e31  * (e11  * (1.0 + 4.0 * ecc1 * ecc1) * cosargp1 - (1.0 - ecc1 * ecc1) * e21  * sinargp1) * (eccanom1 - m1) +
				1.0 / beta1  * mrat3 * e31  * (-3.0 * ecc1  * sin(eccanom1) * ((11.0 / 4.0 + ecc1 * ecc1) * e11  * cosargp1 - 0.25 * (1.0 - ecc1 * ecc1) * e21  * sinargp1) +
				0.75 * sin(2.0 * eccanom1) * ((1.0 + 2.0 * ecc1 * ecc1) * e11  * cosargp1 + (1.0 - ecc1 * ecc1) * e21  * sinargp1) -
				0.25 * ecc1  * sin(3.0 * eccanom1) * (e11  * cosargp1 + (1.0 - ecc1 * ecc1) * e21  * sinargp1) +
				0.75 * beta1  * (e21  * cosargp1 - e11  * sinargp1) * (5.0 * ecc1  * cos(eccanom1) - (1.0 + ecc1 * ecc1) * cos(2.0 * eccanom1) + 1.0 / 3.0 * ecc1  * cos(3.0 * eccanom1))) +
				1.5 / beta1  * mrat4 * e31  * (del1 * cosargp1 - del2 * sinargp1);
			draandtp = 1.5 * 1.90 / (beta1  * sini1) * mrat3 * e31  * (e21  * (1.0 - ecc1 * ecc1) * cosargp1 + (1.0 + 4.0 * ecc1 * ecc1) * e11  * sinargp1) * (eccanom1 - m1) +
				1.0 / (beta1  * sini1) * mrat3 * e31  * ((-3.0 * ecc1  * (0.25 * e21  * (1.0 - ecc1 * ecc1) * cosargp1 + (11.0 / 4.0 + ecc1 * ecc1) * e11  * sinargp1) * e11  * sinargp1) * sin(eccanom1) +
				0.75 * (-e21  * (1.0 - ecc1 * ecc1) * cosargp1 + (1.0 + 2.0 * ecc1 * ecc1) * e11  * sinargp1) * sin(2.0 * eccanom1) -
				0.25 * ecc1  * (-e21  * (1.0 - ecc1 * ecc1) * cosargp1 + e11  * sinargp1) * sin(3.0 * eccanom1) +
				0.75 * beta1  * (e11  * cosargp1 + e21  * sinargp1) * (5.0 * ecc1  * cos(eccanom1) - (1.0 - ecc1 * ecc1) * cos(2.0 * eccanom1) + 1.0 / 3.0 * ecc1  * cos(eccanom1))) +
				1.5 / (beta1  * sini1) * mrat4 * e31  * (del2 * cosargp1 + del1 * sinargp1);
			x3 = 1.5 * mrat3 * ecc1  * beta1  * (4.0 * e11 * e11 - e21 * e21 - 1.0) * (eccanom1 - m1) +
				mrat3 * beta1  * (((-15.0 / 4.0 * e11 * e11 + 0.75 * e21 * e21 + 1.0) - ecc1 * ecc1 * (3.0 * e11 * e11 - 1.0)) * sin(eccanom1) +
				0.25 * ecc1  * (3.0 * e21 * e21 - 1.0) * sin(2.0 * eccanom1) - 0.25 * (e21 * e21 - e11 * e11) * sin(3.0 * eccanom1) +
				0.75 / beta1  * e11  * e21  * (6.0 - 11.0 * ecc1 * ecc1) * cos(eccanom1) + ecc1  * (1.0 + ecc1 * ecc1) * cos(2.0 * eccanom1) - 1.0 / 3.0 * (2.0 - ecc1 * ecc1) * cos(3.0 * eccanom1)) +
				1.5 * beta1  * mrat4 * e11  * (1.0 / 8.0 * (20.0 + 45.0 * ecc1 * ecc1 - 25.0 * e11 * e11 - 100.0 * ecc1 * ecc1 * e11 * e11 - 25.0 * e21 * e21 + 75.0 * ecc1 * ecc1 * e21 * e21) * (eccanom1 - m1) +
				0.25 * ecc1  * (-36.0 - 11.0 * ecc1 * ecc1 + 65.0 * e11 * e11 + 20.0 * ecc1 * ecc1 * e11 * e11 - 15.0 * e21 * e21 - 5.0 * ecc1 * ecc1 * e21 * e21) * sin(eccanom1) +
				0.25 * (1.0 + 7.0 * ecc1 * ecc1 - 5.0 * e11 * e11 - 5.0 * ecc1 * ecc1 * e11 * e11 + 10.0 * e21 * e21 - 20.0 * ecc1 * ecc1 * e21 * e21) * sin(2.0 * eccanom1) +
				1.0 / 12.0 * ecc1  * (-ecc1 * ecc1 - 5.0 * e11 * e11 + 15.0 * e21 * e21 + 5.0 * ecc1 * ecc1 * e21 * e21) * sin(3.0 * eccanom1) +
				1.0 / 32.0 * (-ecc1 * ecc1 + 5.0 * e11 * e11 - 15.0 * e21 * e21 + 5.0 * ecc1 * ecc1 * e21 * e21) * sin(4.0 * eccanom1)) +
				1.5 * mrat4 * e21  * (-0.25 * ecc1  * (-4.0 + 11.0 * ecc1 * ecc1 - 11.0 * ecc1 * ecc1 + 65.0 * e11 * e11 - 100.0 * ecc1 * ecc1 * e11 * e11 - 15.0 * e21 * e21 + 15.0 * ecc1 * ecc1 * e21 * e21) * cos(eccanom1) -
				1.0 / 8.0 * (2.0 - 5.0 * ecc1 * ecc1 - 4.0 * ecc1 * ecc1 - 25.0 * e11 * e11 + 40.0 * ecc1 * ecc1 * e11 * e11 + 20.0 * pow(e21, 4) * e11 * e11 + 5.0 * e21 * e21 - 5.0 * ecc1 * ecc1 * e21 * e21) * cos(2.0 * eccanom1) -
				1.0 / 12.0 * ecc1  * (3.0 * ecc1 * ecc1 - 15.0 * e11 * e11 + 5.0 * ecc1 * ecc1 * e21 * e21) * cos(3.0 * eccanom1) -
				1.0 / 32.0 * (-ecc1 * ecc1 + 15.0 * e11 * e11 - 10.0 * ecc1 * ecc1 * e11 * e11 - 5.0 * e21 * e21 + 5.0 * ecc1 * ecc1 * e21 * e21) * cos(4.0 * eccanom1));
			dargpdtp = -draandtp * cosi1 + 1.0 / ecc1  * x3;
			x4 = -21.0 / 4.0 * mrat3 * (e11 * e11 * (1.0 + 4.0 * ecc1 * ecc1) + e21 * e21 * (1.0 - ecc1 * ecc1) - (2.0 / 3.0 + ecc1 * ecc1)) * (eccanom1 - m1) -
				7.0 * mrat3 * (-1.5 * ecc1  * (e11 * e11 * (11.0 / 4.0 + ecc1 * ecc1) + 0.25 * e21 * e21 * (1.0 - ecc1 * ecc1) - (1.0 + 0.25 * ecc1 * ecc1)) * sin(eccanom1) +
				3.0 / 8.0 * (e11 * e11 * (1.0 + 2.0 * ecc1 * ecc1) - e21 * e21 * (1.0 - ecc1 * ecc1) - ecc1 * ecc1) * sin(2.0 * eccanom1) -
				1.0 / 24.0 * ecc1  * (3.0 * e11 * e11 - 3.0 * e21 * e21 * (1.0 - ecc1 * ecc1) - ecc1 * ecc1) * sin(3.0 * eccanom1) +
				0.75 * beta1  * e11  * e21  * (5.0 * ecc1  * cos(eccanom1) - (1.0 + ecc1 * ecc1) * cos(2.0 * eccanom1) + 1.0 / 3.0 * ecc1  * cos(eccanom1))) -
				3.0 * mrat4 *  (-5.0 / 8.0 * ecc1  * e11  * (3.0 * (5.0 * e21 * e21 - 1.0) * (4.0 + 3.0 * ecc1 * ecc1) + 5.0 * (e11 * e11 - 3.0 * e21 * e21) * (3.0 + 4.0 * ecc1 * ecc1)) * (eccanom1 - m1) +
				0.25 * e11  * (3.0 * (5.0 * e21 * e21 - 1.0) * (4.0 + 21.0 * ecc1 * ecc1 + 3.0 * pow(ecc1, 4)) + 5.0 * (e11 * e11 - 3.0 * e21 * e21) * (3.0 + 21.0 * ecc1 * ecc1 + 4.0 * ecc1 * ecc1)) * sin(eccanom1) -
				0.25 * ecc1  * e11  * (3.0 * (5.0 * e21 * e21 - 1.0) * (3.0 + 4.0 * ecc1 * ecc1) + 5.0 * (e11 * e11 - 3.0 * e21 * e21) * (4.0 + 3.0 * ecc1 * ecc1)) * sin(2.0 * eccanom1) +
				1.0 / 12.0 * e11  * (3.0 * (5.0 * e21 * e21 - 1.0) * (3.0 * ecc1 * ecc1 + pow(ecc1, 4)) + 5.0 * (e11 * e11 - 3.0 * e21 * e21) * (1.0 + 3.0 * ecc1 * ecc1)) * sin(3.0 * eccanom1) -
				1.0 / 32.0 * ecc1  * e11  * (3.0 * (5.0 * e21 * e21 - 1.0) * ecc1 * ecc1 + 5.0 * (e11 * e11 - 3.0 * e21 * e21)) * sin(4.0 * eccanom1) -
				0.25 * beta1  * e21  * (5.0 * (3.0 * e11 * e11 - e21 * e21) * (1.0 + 6.0 * ecc1 * ecc1) + (5.0 * e21 * e21 - 3.0) * (4.0 + 3.0 * ecc1 * ecc1)) * cos(eccanom1) +
				1.0 / 8.0 * beta1  * ecc1  * e21  * (5.0 * (3.0 * e11 * e11 - e21 * e21) * (5.0 + 2.0 * ecc1 * ecc1) + (5.0 * e21 * e21 - 3.0) * (6.0 + ecc1 * ecc1)) * cos(2.0 * eccanom1) -
				1.0 / 12.0 * beta1  * e21  * (5.0 * (3.0 * e11 * e11 - e21 * e21) * (1.0 + 2.0 * ecc1 * ecc1) + (5.0 * e21 * e21 - 3.0) * 3.0 * ecc1 * ecc1) * cos(3.0 * eccanom1) +
				1.0 / 32.0 * beta1  * ecc1  * e21  * (5.0 * (3.0 * e11 * e11 - e21 * e21) + (5.0 * e21 * e21 - 3.0) * ecc1 * ecc1) * cos(4.0 * eccanom1));
			dmdtp = -beta1  * (dargpdtp + draandtp * cosi1) + x4;

			// sum periodic terms
			if (ithird == 1)
			{
				dndtps = dndtp;  // sun periodic
				dedtps = dedtp;
				didtps = didtp;
				draandtps = draandtp;
				dargpdtps = dargpdtp;
				dmdtps = dmdtp;
				if (show == 'y')
					printf("3bdy1st %1i %11.8g %11.8g %11.8g %11.8g %11.8g %11.8g\n", ithird, dndtp, dedtp, didtp, draandtp, dargpdtp, dmdtp);
			}
			else
			{
				dndtpm = dndtp;  // moon periodic
				dedtpm = dedtp;
				didtpm = didtp;
				draandtpm = draandtp;
				dargpdtpm = dargpdtp;
				dmdtpm = dmdtp;
				if (show == 'y')
					printf("3bdy2nd %1i %11.8g %11.8g %11.8g %11.8g %11.8g %11.8g\n", ithird, dndtp, dedtp, didtp, draandtp, dargpdtp, dmdtp);
			}

		}  // for ithird through periodic terms

		//    printf('changes_from_3bdy_(deg/day)_dedtdidtsec %12.9f %12.9f %12.9f %12.9f dedtdidtper %11.8f  %11.8f %11.8f  %11.8f   \n',  
		//               dedtss * 86400.0, didtss * rad * 86400.0, dedtsm * 86400.0, didtsm * rad * 86400.0,  
		//               dedtps * 86400.0, didtps * rad * 86400.0, dedtpm * 86400.0, didtpm * rad * 86400.0);

		// sum the third body periodic effects
		dndt3p = dndtps + dndtpm;
		//dadt3p = -2.0 / 3.0 * a * dndt3p / n;
		dedt3p = dedtps + dedtpm;
		didt3p = didtps + didtpm;
		draandt3p = draandtps + draandtpm;
		dargpdt3p = dargpdtps + dargpdtpm;
		dmdt3p = dmdtps + dmdtpm;

		if (show == 'y')
			printf("3eq_peri %11.7f %11.7f %11.7f %11.7f %11.7f %11.7f \n", dndt3p, dedt3p, didt3p, draandt3p, dargpdt3p, dmdt3p);

		// find the osculating contribution to add to the sec / lp values
		temp = 1.0 / (1.0 + cosi1);
		//dela = -2.0 / 3.0 * a * dndtp / n;  // km / s s
		deln = dndt3p;  // -1.5 * n / a * dadt3p; so just dndt3p
		// rest are unitless
		// account for small ecc, incl
		temp1 = x3 + ecc1  * sini1 * draandt3p * sini1 * temp;
		//delaf = dedt3p * cos(argp1 + raan1) - ecc1  * (dargpdt3p + draandt3p) * sin(argp1 + raan1);
		delaf = dedt3p * cos(argp1 + raan1) - temp1 * sin(argp1 + raan1);
		//delag = dedt3p * sin(argp1 + raan1) + ecc1  * (dargpdt3p + draandt3p) * cos(argp1 + raan1);
		delag = dedt3p * sin(argp1 + raan1) + temp1 * cos(argp1 + raan1);
		delchi = didt3p * sinraan1 * temp + sini1  * draandt3p * cosraan1 * temp;
		delpsi = didt3p * cosraan1 * temp - sini1  * draandt3p * sinraan1 * temp;
		//delmlonm = dmdt3p + dargpdt3p + draandt3p;
		delmlonm = x4 + x3 * ecc1 / (1.0 + beta1) + sini1 * draandt3p * sini1 * temp;

		// ---------- - updated osculating equinoctial elements---------------- -
		temp = 1.0;
		n1 = n1 + temp * deln;
		// a1 already calculated 
//		a1 = pow(mu / (n1 * n1), (1.0 / 3.0));
		af1 = afnew + temp * delaf;
		ag1 = agnew + temp * delag;
		chi1 = chinew + temp * delchi;
		psi1 = psinew + temp * delpsi;
		mlon1 = mlonmnew + temp * delmlonm; // +n1 * dtsec;  ?????

		if (show == 'y')
			printf("3eq_final %11.7f %11.7f %11.7f %11.7f %11.7f %11.7f \n", a1, af1, ag1, chi1, psi1, mlon1 * rad);

		ast2Body::eq2rv(a1, af1, ag1, chi1, psi1, mlon1, fr, rosc, vosc);
	}  // thirdbodyperts



	/*  ------------------------------------------------------------------------------
	 * 
	 *                            procedure srpperts
	 * 
	 *   this procedure calculates the secular / lp avg rates and the osculating classical
	 *     elements for a satellite from srp perturbations.
	 * 
	 *   author        : david vallado                  719 - 573 - 2600     3 dec 2017
	 * 
	 *   inputs          description                    range / units
	 *     jdtdb       - epoch julian date              days from 4713 BC
	 *     jdtdbF      - epoch julian date fraction     day fraction from jdtdb
	 *     rmean       - init mean position vector eci  km
	 *     vmean       - init mean velocity vector eci  km / s
	 *     interp      - interpolation for jpl sun      l, s
	 *     opt         - which method to find sun (jpl) j, Xeci
	 *     fullsun     - option to use full sun or not  y, n
	 *     dtsec       - desired time from 0.0 epoch    sec
	 *     jpldearr    - array of Sun and Moon position vectors from de430
	 *     jdjpldestart- jd of start of array of de vectors days frm 4713 BC
	 *     tut1        - julian centuries of ut1
	 *     cram        - cr * a / mass                    kg / m2
	 * 
	 *   outputs       :
	 *     rosc        - final osc position vector eci  km
	 *     vosc        - final osc velocity vector eci  km / s
	 *     rmean1      - final mean position vector eci  km
	 *     vmean1      - final mean velocity vector eci  km / s
	 * 
	 *   locals :
	 *     rsun        - vector from earth to sun       km
	 * 
	 *   coupling :
	 *     ast2Body::sunmoonjpl, sun, rv2coe, rv2eq, eq2rv
	 * 
	 *   references :
	 *     vallado       2013, ch 9
	 *  chao/hoots       2018 (tbd)
	  ----------------------------------------------------------------------------   */

	void srpperts
		(
		double jdtdb, double jdtdbF, double rmean[3], double vmean[3],
		char interp, char opt, char fullsun,
		double dtsec,
		const std::vector<jpldedata> &jpldearr,
		double jdjpldestart,
		double tut1, double cram, double rosc[3], double vosc[3], double rmean1[3], double vmean1[3]
		)
	{
		double temp, rtasc, decl;
		double p, a, ecc, incl, raan, argp, nu, m, arglat, truelon, lonper, mlonm, mlonnu, n;
		double rsun[3], rmoon[3], rtascs, decls, rtascm, declm;
		double betas, fsrp, obliquity, meanlonsun, meanlong, ttdb, meananomaly, eclplong, beta, eccanom;
		double beta1, obliquity1, meanlonsun1, meanlong1, meananomaly1, eclplong1, eccanom1;
		double p1, a1, n1, ecc1, incl1, raan1, argp1, nu1, m1, arglat1, truelon1, lonper1;
		double cosi, sini, cosraan, sinraan;
		double af, ag, chi, psi, mlon, afnew, agnew, chinew, psinew, mlonmnew, anew;
		double cosa1, sina1, cosa2, sina2, cosa3, sina3, cosa4, sina4, cosa5, sina5, cosa6, sina6, cosoblh, sinoblh;
		double c1, c2, c3, c4, c5, c6, c7, c8, d1, d2, d3, d4;
		double cosa11, sina11, cosa21, sina21, cosa31, sina31, cosa41, sina41, cosa51, sina51, cosa61, sina61, cosoblh1, sinoblh1;
		double c11, c21, c31, c41, c51, c61, c71, c81, d11, d21, d31, d41, cosi1, sini1;
		double een, eex, cos2emcos2x, cosemcosx, sin2emsin2x, sinemsinx, x5, x6, dndts, dedts, didts, draandts, dargpdts, dmdts;
		double dadt, dafdt, dagdt, dchidt, dpsidt, dmlonmdt, dt;
		double dela, delaf, delag, delchi, delpsi, delmlonm;
		double dndtp, dedtp, didtp, draandtp, dargpdtp, dmdtp, x7, x8;
		double elemt0, elemratet0, t0, f1, temp1;
		double t2, x2, f2, t3, x3, f3, t4, x4, f4, elemnew;
		int i, fr;

		if (opt == 'j')
		{
			//jpl ephemerides - just do once for sun. and moon
			ast2Body::sunmoonjpl(jdtdb, jdtdbF, interp, jpldearr, jdjpldestart, rsun, rtascs, decls, rmoon, rtascm, declm);
		}
		else
			ast2Body::sun(jdtdb, jdtdbF, rsun, rtasc, decl);   // au

		// --------------------initialize values-------------------  
		double rad = 180.0 / pi;
		double rsunkm = 149597870.66; // km
		double asun = 149598023.0; // km
		double dayconv = 1.0 / 86400.0;
		double jyrconv = 1.0 / 36525.0;

		betas = 1.0;  // 0.0 to 1.0
		fsrp = cram * pSR * pow(asun / rsunkm, 2);  // m2 / kg kg km / s2m2->km / s2

		obliquity = (23.439291 - 0.0130042 * tut1) / rad;
		meanlonsun = (280.4606184 + 36000.77005361 * tut1) / rad;  // rad
		// mean longitude seems to be better, but report says ecliptic longiude is the term

		meanlong = 280.460 + 36000.77 * tut1;
		meanlong = fmod(meanlong, 360.0);  //deg
		ttdb = tut1;
		meananomaly = 357.5277233 + 35999.05034 * ttdb;
		meananomaly = fmod(meananomaly / rad, twopi);  //rad
		if (meananomaly < 0.0)
			meananomaly = twopi + meananomaly;

		eclplong = meanlong + 1.914666471 * sin(meananomaly) + 0.019994643 * sin(2.0 * meananomaly); //deg
		eclplong = fmod(eclplong, 360.0) / rad;  //rad

		//same
		//d = tut1 * 36525.0 + 2451545.5 - 2400000.5 - 15019.5;
		//eclplong2 = mod(279.7 + 0.98564734 * d + 1.92 * sin((358.48 + 0.98560027 * d) / rad), 360.0) / rad;

		// ---------------------  find mean coes at epoch----------------------- 
		ast2Body::rv2coe(rmean, vmean, mu, p, a, ecc, incl, raan, argp, nu, m, eccanom, arglat, truelon, lonper);
		if (show == 'y')
			printf("coes_init %11.4f %11.4f %13.9f %13.7f %11.5f %11.5f %11.5f %11.5f %11.5f \n",
			p, a, ecc, incl * rad, raan * rad, argp * rad, nu * rad, m * rad, arglat * rad);
		cosi = cos(incl);
		sini = sin(incl);
		cosraan = cos(raan);
		sinraan = sin(raan);
		beta = sqrt(1.0 - ecc * ecc);

		// ----------------find mean equinoctials at epoch--------------------
		ast2Body::rv2eq(rmean, vmean, a, n, af, ag, chi, psi, mlonm, mlonnu, fr);
		mlonm = fmod(mlonm, twopi);
		if (show == 'y')
			printf("eq_init  %12.9f %12.9f %12.9f %12.9f %12.9f %12.9f n %12.9f \n", a, af, ag, chi, psi, mlonm * rad, n);

		//     u3vec = unit(rsun);
		//     // transform to sat coordinate rsw system
		//     q11 = cosargp * cosraan - sinargp * sinraan * cosi;
		//     q12 = cosargp * sinraan + sinargp * cosraan * cosi;
		//     q13 = sinargp * sini;
		//     q21 = -sinargp * cosraan - cosargp * sinraan * cosi;
		//     q22 = -sinargp * sinraan + cosargp * cosraan * cosi;
		//     q23 = cosargp * sini;
		//     q31 = sinraan * sini;
		//     q32 = -cosraan * sini;
		//     q33 = cosi;
		//     e1 = q11 * u3vec(1) + q12 * u3vec(2) + q13 * u3vec(3);
		//     e2 = q21 * u3vec(1) + q22 * u3vec(2) + q23 * u3vec(3);
		//     e3 = q31 * u3vec(1) + q32 * u3vec(2) + q33 * u3vec(3);

		// ----------------second order terms
		// ----srp disturbing force components
		// ----not needed in this formulation
		//     cos2eps = cosoblh ^ 2;
		//     sin2eps = sinoblh ^ 2;
		//     cos2incl = cos(incl * 0.5) ^ 2;
		//     sin2incl = sin(incl * 0.5) ^ 2;
		//     u = argp + nu;
		//     rp = -fsrp * (cos(meanlonsun) * (cos(u) * cos(raan) - sin(u) * sin(raan) * cos(incl)) +  
		//                  sin(meanlonsun) * (cos(obliquity) * cos(u) * sin(raan) + cos(obliquity) * sin(u) * cos(raan) * cos(incl) + sin(obliquity) * sini * sin(u)));
		//     sp = fsrp * (cos(meanlonsun) * (sin(u) * cos(raan) + cos(u) * sin(raan) * cos(incl)) -  
		//                 sin(meanlonsun) * (-cos(obliquity) * sin(u) * sin(raan) + cos(obliquity) * cos(u) * cos(raan) * cos(incl) + sin(obliquity) * sini * cos(u)));
		//     wp = fsrp * (-cos(meanlonsun) * sini * sin(raan) + sin(meanlonsun) * cos(raan) * sini * cos(u) * cos(obliquity) - sin(menalonsun) * cos(incl) * sin(obliquity));

		// --------------lpeom equations of variation srp--------------------------------- 
		// ----note these are not used in the semianalytical approach because
		// ----they are in the 2nd order secular and lp equations
		//     dndt = -3.0 / (a * beta) * (ecc * sin(nu) * fr + p / magr * ft);
		//     dadt = 2.0 / (n * beta) * (ecc * sin(nu) * fr + (a * beta * beta / magr) * ft);
		//     dedt = beta / (n * a) * (fr * sin(nu) + ((magr + a * beta * beta) * cos(nu) + ecc * magr) / (a * beta * beta) * ft);
		//     didt = magr / (n * a ^ 2 * beta) * fn * cos(nu + argp);
		//     draandt = magr / (n * a ^ 2 * beta * sini) * fn * sin(nu + argp);
		//     dargpdt = -draandt * cos(incl) + beta / (n * a * ecc) * (-fr * cos(nu) + ft * sin(nu) * (1.0 + magr / (a * beta * beta)));
		//     dmdt = n - 2.0 * magr / (n * a ^ 2) * fr - beta * (dargpdt + draandt * cos(incl));

		// ---------------  2nd order secular and lp equations------------------
		// ------------------------intermediate terms------------------------- 
		cosa1 = cos(eclplong - argp - raan);
		sina1 = sin(eclplong - argp - raan);
		cosa2 = cos(eclplong - argp + raan);
		sina2 = sin(eclplong - argp + raan);
		cosa3 = cos(eclplong - argp);
		sina3 = sin(eclplong - argp);
		cosa4 = cos(eclplong + argp);
		sina4 = sin(eclplong + argp);
		cosa5 = cos(eclplong + argp - raan);
		sina5 = sin(eclplong + argp - raan);
		cosa6 = cos(eclplong + argp + raan);
		sina6 = sin(eclplong + argp + raan);

		cosoblh = cos(obliquity * 0.5);
		sinoblh = sin(obliquity * 0.5);
		c1 = cos(incl * 0.5) * cos(incl * 0.5) * cosoblh * cosoblh;
		c2 = sin(incl * 0.5) * sin(incl * 0.5) * sinoblh * sinoblh;
		c3 = 0.5 * sini * sin(obliquity);
		c4 = -sin(incl * 0.5) * sin(incl * 0.5) * cosoblh * cosoblh;
		c5 = -cos(incl * 0.5) * cos(incl * 0.5) * sinoblh * sinoblh;
		c6 = sini * cosoblh * cosoblh;
		c7 = sini * sinoblh * sinoblh;
		c8 = cosi * sin(obliquity);

		d1 = c1 * cosa1 + c2 * cosa2 + c3 * cosa3 - c3 * cosa4 - c4 * cosa5 - c5 * cosa6;
		d2 = c1 * sina1 + c2 * sina2 + c3 * sina3 + c3 * sina4 + c4 * sina5 + c5 * sina6;
		d3 = 0.5 * (c6 * cosa1 - c6 * cosa5 + c7 * cosa6 - c7 * cosa2 + c8 * cosa4 - c8 * cosa3);
		d4 = 0.5 * (c6 * sina1 + c6 * sina5 - c7 * sina6 - c7 * sina2 - c8 * sina4 - c8 * sina3);

		if (fullsun == 'n')
		{
			// -----------------  use entry exit from eclipse------------------- 
			// calc "all" the entry exits, then add up "all" the values ?
			een = 1.0;  // fixxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
			eex = 1.0;

			cos2emcos2x = cos(2.0 * een) - cos(2.0 * eex);
			cosemcosx = cos(een) - cos(eex);
			sin2emsin2x = sin(2.0 * een) - sin(2.0 * eex);
			sinemsinx = sin(een) - sin(eex);
			temp = fsrp / (2.0 * pi * n * a);  //  km / s2 s / km -> / s
			// x5 and x6 corrected for singularities
			x5 = beta * temp * d1 * (1.5 * (een - eex) - ecc * sinemsinx - 0.25 * sin2emsin2x) +  // / ecc
				1.0 * temp * d2 * (0.25 * cos2emcos2x - ecc * cosemcosx);
			x6 = 1.0 * temp * d1 * (-4.5 * ecc * (een - eex) - 5.0 / 4.0 * ecc * sin2emsin2x + (5.0 + 2.0 * ecc * ecc) * sinemsinx) -
				5.0 * beta * temp * d2 * (cosemcosx - 0.25 * ecc * cos2emcos2x);

			// ----in terms of kepler elements
			dndts = 3.0 / (2.0 * pi * a) * fsrp * (d1 * cosemcosx + d2 * beta * sinemsinx);  // / km  km / s2 -> / s2
			dedts = -beta * temp * d2 * (1.5 * (een - eex) + 0.25 * sin2emsin2x - 2.0 * ecc * sinemsinx) -
				beta / (4.0) * temp * d1 * cos2emcos2x;  // / (/ s km) km / s ^ 2  -> / s
			didts = 1.0 / beta * temp * d4 * (-1.5 * ecc * (een - eex) + (1.0 + ecc * ecc) * sinemsinx - 0.25 * ecc * sin2emsin2x) -
				1.0 * temp * d3 * (-cosemcosx + 0.25 * ecc * cos2emcos2x);  // / s
			draandts = 1.0 / sini * temp * d4 * (-cosemcosx +
				0.25 * ecc * (2.0 * cos(een) - cos(2.0 * eex))) +
				1.0 / (beta * sini) * temp * d3 * (-1.5 * ecc * (een - eex) + (1.0 + ecc * ecc) * sinemsinx - 0.25 * ecc * sin2emsin2x);   // / s
			dargpdts = -draandts * cosi + 1.0 / ecc * x5;   // / s
			dmdts = -beta / ecc * x5 + x6; // corrected equation
		}
		else
		{
			// ------------------------use full sun--------------------------- 
			// ----in terms of kepler elements, all units(s / km km / s2)-> / s
			dndts = 0.0;  // units should be / s2 if not 0
			dedts = -1.5 * beta / (n * a) * fsrp * d2;     // / s
			didts = -1.5 * ecc / (n * a * beta) * fsrp * d4;  // / s
			draandts = -1.5 * ecc / (n * a * beta * sini) * fsrp * d3;  // / s
			// x5 and x6 corrected for singularities
			x5 = 1.5 * beta / (n * a) * fsrp * d1;
			dargpdts = -draandts * cosi + 1.0 / ecc * x5;  // / s
			// which is correct ? ? ? ? ? ? ? ? ? ?
			//x6 = n - 3.0 * ecc / (n * a) * fsrp * d1;
			x6 = n - 4.5 * ecc / (n * a) * fsrp * d1;  // no n in here? no. put in update to elements
			dmdts = x6 - beta * (dargpdts + draandts * cosi);  // / s
		}
		if (show == 'y')
		{
			printf(" eccanom %11.7f fsrp %11.7g dela %11.7f %11.7f %11.7f  \n", eccanom * rad, fsrp, -2.0 / 3.0 * a * dndts / n, fmod(meanlonsun, twopi) * rad, eclplong * rad);  //, eclplong2 * rad
			printf("srpcoe_rate %11.7g %11.7g %11.7g %11.7g %11.7g %11.7g \n", dndts, dedts, didts, draandts, dargpdts, dmdts);
		}

		// --------------------------------------------------------------------- 
		// find avg rate of change in the equinoctial elements.use the 2nd order
		// secular and lp equations in kepler elements.these equations are the
		// rate in the numerical integration in terms of equinoctial elements.
		// --------------------------------------------------------------------- 
		double cosargpraan = cos(argp + raan);
		double sinargpraan = sin(argp + raan);
		temp = 1.0 / (1.0 + cosi);
		// use one of the following 
		dadt = -0.666666666666667 * a * dndts / n;  // km / s2 s->km / s
		//dndt = -1.5 * n * dadts / a;  // from third body tor 
		// account for small ecc, incl
		temp1 = x5 + ecc * sini * draandts * sini * temp;
		//dafdt = dedts * cos(argp + raan) - ecc * (dargpdts + draandts) * sin(argp + raan);  // / s
		dafdt = dedts * cosargpraan - temp1 * sinargpraan;  // / s
		//dagdt = dedts * sin(argp + raan) + ecc * (dargpdts + draandts) * cos(argp + raan);  // / s
		dagdt = dedts * sinargpraan + temp1 * cosargpraan;  // / s
		dchidt = didts * sinraan * temp + sini * draandts * cosraan * temp;  // / s
		dpsidt = didts * cosraan * temp - sini * draandts * sinraan * temp;  // / s
		//dmlonmdt = dmdts + dargpdts + draandts;  // / s
		dmlonmdt = x6 + x5 * ecc / (1.0 + beta) + sini * draandts * sini * temp;

		if (show == 'y')
		{
			//printf("eq_rate %11.7g %11.7g %11.7g %11.7g %11.7g %11.7g \n", dadt, dafdt, dagdt, dchidt, dpsidt, dmlonmdt);
			printf("eq_rate (rad/min) %11.7g %11.7g %11.7g %11.7g %11.7g %11.7g %11.7g \n", (dmlonmdt - n) * 60, dadt * 60, dchidt * 60, dpsidt * 60, dafdt * 60, dagdt * 60, dedts * 60);
		}
		printf("eq_rate (rad/min) %11.7g %11.7g %11.7g %11.7g %11.7g %11.7g %11.7g \n", (dmlonmdt - n) * 60, dadt * 60, dchidt * 60, dpsidt * 60, dafdt * 60, dagdt * 60, dedts * 60);

		// -------------------  numerically integrate 2nd ord sec / lp to current time--------------------------- 
		dt = 86400.0 * floor(dtsec * dayconv);  // sec, 1 day step size
		for (i = 1; i <= 6; i++)
		{
			// process each of the mean orbital elements and update to current time
			switch (i)
			{
				// dtmin is the minutes from epoch (1st obs in .e file) to currobs time
			case 1:
				elemratet0 = dafdt;
				elemt0 = af;
				break;
			case 2:
				elemratet0 = dagdt;
				elemt0 = ag;
				break;
			case 3:
				elemratet0 = dchidt;
				elemt0 = chi;
				break;
			case 4:
				elemratet0 = dpsidt;
				elemt0 = psi;
				break;
			case 5:
				elemratet0 = dmlonmdt;  // put n in here!!!  no
				elemt0 = mlonm;
				break;
			case 6:
				// elemratet0 = dndt;
				elemratet0 = dadt;  // this is 0 if full sun
				elemt0 = a;
				break;
			}

			f1 = elemratet0;
			t0 = 0.0;  // assume starts at epoch

			// Could also use an optimized RK4 - note the different coefficients
			// assume that the rates are the same for each evaluation???
			//         t2 = t0 + 3.0 / 8.0 * dt;
			//         x2 = elemt0 + 3.0 / 8.0 * f1 * dt;
			//         f2 = elemratet2;
			//         t3 = t0 + 9.0 / 16.0 * dt;
			//         x3 = elemt0 + 9.0 / 16.0 * f2 * dt;
			//         f3 = elemratet3;
			//         t4 = t0 + 25.0 / 32.0 * dt;
			//         x4 = elemt0 + (-125.0 / 672.0 * f1 + 325.0 / 336.0 * f2) * dt;
			//         f4 = elemratet4;
			//         elemnew = elemt0 + (37.0 / 225.0 * f1 + 44.0 / 117.0 * f2 + 448.0 / 975.0 * f4) * dt;

			t2 = t0 + 0.5 * dt;
			x2 = elemt0 + 0.5 * f1 * dt;
			f2 = elemratet0;
			t3 = t0 + 0.5 * dt;
			x3 = elemt0 + 0.5 * f2 * dt;
			f3 = elemratet0;
			t4 = t0 + dt;
			x4 = elemt0 + f3 * dt;
			f4 = elemratet0;
			elemnew = elemt0 + 0.166666666666667 * (f1 + 2.0 * f2 + 2.0 * f3 + f4) * dt;

			// -----------------------  interpolate elems to exact time--------------------------- 
			//tneed = fmod(dtsec, 86400);
			//dd0 = pow(tneed - 1.0, 2) * (2.0 * tneed + 1.0);
			//dd1 = tneed * pow(tneed - 1.0, 2);
			//dd2 = 1.0 - dd0;
			//dd3 = pow(tneed, 2) * (tneed - 1.0);
			//elemnew = dd0 * elemt0 + dd1 * elemratet0 * dt + dd2 * elemnew + dd3 * elemratet0 * dt;  // or ratenew ? ?
            // does the fractional time come "after" the elemnew, or before it? elemt0 + or elemnew + ??
			//elemnew = elemt0 + elemratet0 * dtsec;

			// find mean equinoctial elements at ti
			switch (i)
			{
			case 1:
				afnew = elemnew;
				break;
			case 2:
				agnew = elemnew;
				break;
			case 3:
				chinew = elemnew;
				break;
			case 4:
				psinew = elemnew;
				break;
			case 5:
				mlonmnew = elemnew;
				break;
			case 6:
				anew = elemnew;
				break;
			}
		}  // for i through 6 orbital elements

		mlonmnew = fmod(mlonmnew, twopi);
		//	printf("eq_new  %12.9f %12.9f %12.9f %12.9f %12.9f %12.9f \n", anew, afnew, agnew, chinew, psinew, mlonmnew * rad);

		// chg updated mean equinoctial to mean classical at new time for periodic evaluation
		ast2Body::eq2rv(anew, afnew, agnew, chinew, psinew, mlonmnew, fr, rmean1, vmean1);
		if (show == 'y')
			printf("srp updated pos/vel  %11.6f %11.6f %11.6f %11.6f %11.6f %11.6f %12.3f \n", rmean1[0], rmean1[1], rmean1[2], vmean1[0], vmean1[1], vmean1[2], dtsec);

		ast2Body::rv2coe(rmean1, vmean1, mu, p1, a1, ecc1, incl1, raan1, argp1, nu1, m1, eccanom1, arglat1, truelon1, lonper1);
		if (show == 'y')
		{
			printf("srp eq_mean updated %12.9f %12.9f %12.9f %12.9f %12.9f %12.9f \n", anew, afnew, agnew, chinew, psinew, mlonmnew * rad);
			printf("coes_new  %11.4f %11.4f %13.9f %13.7f %11.5f %11.5f %11.5f %11.5f %11.5f \n",
				p1, a1, ecc1, incl1 * rad, raan1 * rad, argp1 * rad, nu1 * rad, m1 * rad, arglat1 * rad);
		}
		cosi1 = cos(incl1);
		sini1 = sin(incl1);
		beta1 = sqrt(1.0 - ecc1 * ecc1);
		n1 = sqrt(mu / (a1 * a1 * a1));
		//   above seems to give slightly better results

		//tut1 = (jdut1 - 2451545.0) * jyrconv;
		tut1 = (tut1 * 36525.0 + dtsec * dayconv) * jyrconv;
		//tut1 = tut1 + dtsec /
		obliquity1 = (23.439291 - 0.0130042 * tut1) / rad;
		meanlonsun1 = (280.4606184 + 36000.77005361 * tut1) / rad;  // rad
		// mean longitude seems to be better, but report says ecliptic longiude is the term

		meanlong1 = 280.460 + 36000.77 * tut1;
		meanlong1 = fmod(meanlong1, 360.0);  //deg
		ttdb = tut1;
		meananomaly1 = 357.5277233 + 35999.05034  * ttdb;
		meananomaly1 = fmod(meananomaly1 / rad, 2.0 * pi);  //rad
		if (meananomaly1 < 0.0)
			meananomaly1 = 2.0 * pi + meananomaly1;

		eclplong1 = meanlong1 + 1.914666471 * sin(meananomaly1) + 0.019994643 * sin(2.0 * meananomaly1); //deg
		eclplong1 = fmod(eclplong1, 360.0) / rad;  //rad

		cosa11 = cos(eclplong1 - argp1 - raan1);
		sina11 = sin(eclplong1 - argp1 - raan1);
		cosa21 = cos(eclplong1 - argp1 + raan1);
		sina21 = sin(eclplong1 - argp1 + raan1);
		cosa31 = cos(eclplong1 - argp1);
		sina31 = sin(eclplong1 - argp1);
		cosa41 = cos(eclplong1 + argp1);
		sina41 = sin(eclplong1 + argp1);
		cosa51 = cos(eclplong1 + argp1 - raan1);
		sina51 = sin(eclplong1 + argp1 - raan1);
		cosa61 = cos(eclplong1 + argp1 + raan1);
		sina61 = sin(eclplong1 + argp1 + raan1);

		cosoblh1 = cos(obliquity1 * 0.5);
		sinoblh1 = sin(obliquity1 * 0.5);
		c11 = pow(cos(incl1 * 0.5), 2) * pow(cosoblh1, 2);
		c21 = pow(sin(incl1 * 0.5), 2) * pow(sinoblh1, 2);
		c31 = 0.5 * sini1 * sin(obliquity1);
		c41 = -pow(sin(incl1 * 0.5), 2) * pow(cosoblh1, 2);
		c51 = -pow(cos(incl1 * 0.5), 2) * pow(sinoblh1, 2);
		c61 = sini1 * pow(cosoblh1, 2);
		c71 = sini1 * pow(sinoblh1, 2);
		c81 = cosi1 * sin(obliquity1);

		d11 = c11 * cosa11 + c21 * cosa21 + c31 * cosa31 - c31 * cosa41 - c41 * cosa51 - c51 * cosa61;
		d21 = c11 * sina11 + c21 * sina21 + c31 * sina31 + c31 * sina41 + c41 * sina51 + c51 * sina61;
		d31 = 0.5 * (c61 * cosa11 - c61 * cosa51 + c71 * cosa61 - c71 * cosa21 + c81 * cosa41 - c81 * cosa31);
		d41 = 0.5 * (c61 * sina11 + c61 * sina51 - c71 * sina61 - c71 * sina21 - c81 * sina41 - c81 * cosa31);

		// ----------------------------------periodic equations----------------------------
		// calc classical periodic values
		double sineccanom1 = sin(eccanom1);
		double sin2eccanom1 = sin(2.0 * eccanom1);
		double coseccanom1 = cos(eccanom1);
		double cos2eccanom1 = cos(2.0 * eccanom1);
		dndtp = 3.0 / (n1 * a1) * fsrp * (d11 * coseccanom1 + d21 * beta1 * sineccanom1);   // s / km km / s2 -> / s
		//fprintf(1, ' eccanom %11.7f dela %11.7f m %11.7f m1 %11.7f \n', eccanom * rad, -2.0 / 3.0 * a * dndtp / n, m * rad, m1 * rad);
		// rest are unitless
		dedtp = -beta1 / (n1 * n1 * a1) * fsrp * (d21 * (0.25 * sin2eccanom1) - 0.5 * d21 * ecc1 * sineccanom1 + 0.25 * d11 * cos2eccanom1);
		didtp = 1.0 / (n1 * n1 * a1 * beta1 * sini1) * fsrp * (d41 * ((1.0 - 0.5 * ecc1 * ecc1) * sineccanom1 - 0.25 * ecc1 * sin2eccanom1) -
			d31 * (-coseccanom1 + 0.25 * ecc1 * cos2eccanom1));
		draandtp = 1.0 / (n1 * n1 *  a1 * beta1 * sini1) * fsrp * (d41 * beta1 * (-coseccanom1 + 0.25 * ecc1 * cos2eccanom1) +
			d31 * ((1.0 - 0.5 * ecc1 * ecc1) * sineccanom1 - 0.25 * ecc1 * sin(2.0 * eccanom1)));
		x7 = beta1 / (n1 * n1 * a1) * fsrp * (d11 * (0.5 * ecc1 * sineccanom1 - 0.25 * sin2eccanom1) +
			1.0 / beta1 * d21 * (0.25 * cos2eccanom1 - ecc1 * coseccanom1));
		dargpdtp = -draandtp * cosi1 + 1.0 / ecc1 * x7;
		x8 = 5.0 / (n1 * n1 * a1) * fsrp * d11 * ((1.0 - 0.5 * ecc1 * ecc1) * sineccanom1 - 0.25 * ecc1 * sin2eccanom1) +
			5.0 / (n1 * n1 * a1) * fsrp * d21 * beta1 * (-coseccanom1 + 0.25 * ecc1 * cos2eccanom1);
		dmdtp = x8 - beta1 * (dargpdtp + draandtp * cosi1);
		if (show == 'y')
			printf("srpeq_peri %12.9f %12.9f %12.9f %12.9f %12.9f %12.9f \n", dndtp, dedtp, didtp, draandtp, dargpdtp, dmdtp);

		// -------------  periodic equations in equinoctial space----------------
		// find the osculating contribution to add to the mean sec / lp values in equinoctial space
		temp = 1.0 / (1.0 + cosi1);
		dela = -2.0 / 3.0 * a1 * dndtp / n1;  // km / s s
		double sinraan1 = sin(raan1);
		double sinargpraan1 = sin(argp1 + raan1);
		double cosraan1 = cos(raan1);
		double cosargpraan1 = cos(argp1 + raan1);
		// rest are unitless
		// account for small ecc, incl
		temp1 = x7 + ecc1 * sini1 * draandtp * sini1 * temp;
		//delaf = dedtp * cos(argp + raan) - ecc1 * (dargpdtp + draandtp) * sin(argp + raan);
		delaf = dedtp * cosargpraan1 - temp1 * sinargpraan1;
		//delag = dedtp * sin(argp + raan) + ecc1 * (dargpdtp + draandtp) * cos(argp + raan);
		delag = dedtp * sinargpraan1 + temp1 * cosargpraan1;
		delchi = didtp * sinraan1 * temp + sini1 * draandtp * cosraan1 * temp;
		delpsi = didtp * cosraan1 * temp - sini1 * draandtp * sinraan1 * temp;
		//delmlonm = dmdtp + dargpdtp + draandtp;
		delmlonm = x8 + x7 * ecc1 / (1.0 + beta1) + sini1 * draandtp * sini1 * temp;

		// -----------  updated osculating equinoctial elements--------------------
		//temp = 1.0;
		temp = 1.0; //0.05 * sin(m1 - m);
		a = anew + temp * dela;
		//n = nnew + deln; //kkkkkkkkkkkkkk
		af = afnew + temp * delaf;
		ag = agnew + temp * delag;
		chi = chinew + temp * delchi;
		psi = psinew + temp * delpsi;
		mlon = mlonmnew + temp * delmlonm;  // +n * dtsec;
		mlon = fmod(mlon, twopi);
		if (show == 'y')
		{
			printf("%11.7f  %11.7f  %11.7f\n", temp, m1 * rad, m * rad);
			printf("eq_f osc %12.9f %12.9f %12.9f %12.9f %12.9f %12.9f \n", a, af, ag, chi, psi, mlon * rad);
		}

		ast2Body::eq2rv(a, af, ag, chi, psi, mlon, fr, rosc, vosc);
	}  // srpperts



}  // namespace


