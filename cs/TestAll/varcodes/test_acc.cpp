#include "stdafx.h"
      
// Sample Program for computing geopotential acceleration
      

void main(void)
{
	int nmax, m;
	double x[3], ac[3];

//	nmax = 2190;  // Load full model up to n,m <= 2190
	nmax = 10;    // We wanna use only up to 10

	// Load GM, Ae and harmonic coefficients C and S
	leg_initialize("EGM2008_ZeroTide_Truncated.dat", nmax);

    // Sample ECEF position vector [m]
	x[0] = 6000000.;
	x[1] = -4000000.;
	x[2] = 5000000.;

	// set order/degree M <= Nmax
    m = 9;

    // Routine to compute geopotential acceleration
    leg_forcol_ac (m, x, ac);

    // Result shoud read: 9  -3.5377058876337  2.3585194144421  -2.9531441870790
    printf ("%2i %17.13f %17.13f %17.13f\n", m, ac[0], ac[1], ac[2]);

	return;

}

