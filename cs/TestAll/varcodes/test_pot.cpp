#include "stdafx.h"
      
// Sample Program for computing geopotential
      

void main(void)
{
	int nmax, m;
	double x[3], v;

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

    // Routine to compute geopotential
    v = leg_forcol_pot (m, x);

    // Result shoud read: 9  45425152.301864
    printf ("%2i %16.6f\n", m, v);

	return;

}

