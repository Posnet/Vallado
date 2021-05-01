/*     ----------------------------------------------------------------
*
*                                MathTimeLib.cpp
*
*    this file contains miscellaneous math functions.
*
*                          Companion code for
*             Fundamentals of Astrodynamics and Applications
*                                  2013
*                            by David Vallado
*
*       (w) 719-573-2600, email dvallado@agi.com, davallado@gmail.com
*
*    current :
*              11 jan 18  david vallado
*                           misc cleanup
*    changes :
*              30 sep 15  david vallado
*                           fix jd, jdfrac
*               3 nov 14  david vallado
*                           update to msvs2013 c++
*               7 may 08  david vallado
*                           misc updates, fix sgn, show both options for matrix
*                           multiplications
*              22 jan 08  david vallado
*                           fix some minor errors, fixes to matmult
*              19 oct 07  david vallado
*                           fix sgn baseline comparison
*              30 may 07  david vallado
*                           3rd edition baseline
*              21 jul 05  david vallado
*                           2nd printing baseline
*              14 may 01  david vallado
*                           2nd edition baseline
*              23 nov 87  david vallado
*                           original baseline
*       ----------------------------------------------------------------      */

#include "MathTimeLib.h"

namespace MathTimeLib
{

	// -----------------------------------------------------------------------------
	double sgn
	(
		double x
	)
	{
		if (x < 0.0)
		{
			return -1.0;
		}
		else
		{
			return 1.0;
		}
	} // sgn


	// -----------------------------------------------------------------------------
	// round a number to the nearest integer
	double round
	(
		double x
	)
	{
		double temp;
		temp = floor(x + 0.5);
		return int(temp);
	}  // round

/* -----------------------------------------------------------------------------
*
*                           function acosh
*
*  this function evaluates the inverse hyperbolic cosine function.
*
*  author        : david vallado                  719-573-2600    1 mar 2001
*
*  inputs          description                    range / units
*    xval        - angle value                                  1.0 to infinity
*
*  outputs       :
*    acosh       - result                                       any real
*
*  locals        :
*    temp        - temporary value
*
*  coupling      :
*    none.
* --------------------------------------------------------------------------- */

	double  acosh
	(
		double xval
	)
	{
		double temp;
		if (xval*xval - 1.0 < 0.0)
		{
			temp = undefined;
			printf("error in arccosh function \n");
		}
		else
			temp = log(xval + sqrt(xval*xval - 1.0));

		return temp;
	}  // acosh


		/* -----------------------------------------------------------------------------
		*
		*                           function asinh
		*
		*  this function evaluates the inverse hyperbolic sine function.
		*
		*  author        : david vallado                  719-573-2600    1 mar 2001
		*
		*  inputs          description                               range / units
		*    xval        - angle value                                  any real
		*
		*  outputs       :
		*    asinh       - result                                       any real
		*
		*  locals        :
		*    none.
		*
		*  coupling      :
		*    none.
		* --------------------------------------------------------------------------- */

	double  asinh
	(
		double xval
	)
	{
		return log(xval + sqrt(xval*xval + 1.0));
	}  // asinh


/* ------------------------------------------------------------------------------
*
*                           function cot
*
*  this function finds the cotangent of an input in radians.
*
*  author        : david vallado                  719-573-2600    1 Mar 2001
*
*  inputs          description                    range / units
*    xval        - input to take cotangent of        rad
*
*  outputs       :
*    cot         - result
*
*  locals        :
*    temp        - temporary real variable
 ---------------------------------------------------------------------------- */

	double cot
	(
		double xval
	)
	{
		double temp;

		temp = tan(xval);
		if (fabs(temp) < 0.00000001)
			return infinite;
		else
			return 1.0 / temp;
	}  // cot


	   /* -----------------------------------------------------------------------------
		*
		*                           function dot
		*
		*  this function finds the dot product of two vectors.
		*
		*  author        : david vallado                  719-573-2600    1 mar 2001
		*
		*  inputs          description                    range / units
		*    vec1        - vector number 1
		*    vec2        - vector number 2
		*
		*  outputs       :
		*    dot         - result
		*
		*  locals        :
		*    none.
		* --------------------------------------------------------------------------- */

	double  dot
	(
		double x[3], double y[3]
	)
	{
		return (x[0] * y[0] + x[1] * y[1] + x[2] * y[2]);
	}  // dot

		/* -----------------------------------------------------------------------------
		*
		*                           function mag
		*
		*  this procedure finds the magnitude of a vector.
		*
		*  author        : david vallado                  719-573-2600    1 mar 2001
		*
		*  inputs          description                    range / units
		*    vec         - vector
		*
		*  outputs       :
		*    vec         - answer stored in fourth component
		*
		*  locals        :
		*    none.
		*
		*  coupling      :
		*    none.
		* --------------------------------------------------------------------------- */

	double  mag
	(
		double x[3]
	)
	{
		return sqrt(x[0] * x[0] + x[1] * x[1] + x[2] * x[2]);
	}  // mag

/* -----------------------------------------------------------------------------
*
*                           procedure cross
*
*  this procedure crosses two vectors.
*
*  author        : david vallado                  719-573-2600    1 mar 2001
*
*  inputs          description                    range / units
*    vec1        - vector number 1
*    vec2        - vector number 2
*
*  outputs       :
*    outvec      - vector result of a x b
*
*  locals        :
*    none.
*
*  coupling      :
*    none
 ---------------------------------------------------------------------------- */

	void    cross
	(
		double vec1[3], double vec2[3], double outvec[3]
	)
	{
		outvec[0] = vec1[1] * vec2[2] - vec1[2] * vec2[1];
		outvec[1] = vec1[2] * vec2[0] - vec1[0] * vec2[2];
		outvec[2] = vec1[0] * vec2[1] - vec1[1] * vec2[0];
	}  // cross

		/* -----------------------------------------------------------------------------
		*
		*                           procedure norm
		*
		*  this procedure calculates a unit vector given the original vector.  if a
		*  zero vector is input, the vector is set to zero.
		*
		* author        : david vallado                  719-573-2600    1 mar 2001
		*
		*  inputs          description                    range / units
		*    vec        - vector
		*
		* outputs       :
		*    outvec     - unit vector
		*
		*  locals       :
		*    i          - index
		*    small      - value defining a small value
		*    magv       - magnitude of the vector
		*
		*  coupling     :
		*    mag        magnitude of a vector
		 -----------------------------------------------------------------------------*/

	void    norm
	(
		double vec[3],
		double outvec[3]
	)
	{
		const double small = 0.000001;
		double magv;
		int i;

		magv = mag(vec);
		if (magv > small)
		{
			for (i = 0; i <= 2; i++)
				outvec[i] = vec[i] / magv;
		}
		else
			for (i = 0; i <= 2; i++)
				outvec[i] = 0.0;
	}  // norm


	/* -----------------------------------------------------------------------------
*
*                           procedure angle
*
*  this procedure calculates the angle between two vectors.  the output is
*    set to 999999.1 to indicate an undefined value.  be sure to check for
*    this at the output phase.
*
*  author        : david vallado                  719-573-2600    1 mar 2001
*
*  inputs          description                    range / units
*    vec1        - vector number 1
*    vec2        - vector number 2
*
*  outputs       :
*    theta       - angle between the two vectors  -Pi to Pi
*
*  locals        :
*    temp        - temporary real variable
*    magv1       - magnitude of vec1
*    magv2       - magnitude of vec2
*    small       - value defining a small value
*    undefined   - large number to use in place of a not defined number
*
*  coupling      :
*    dot           dot product of two vectors
*    acos          arc cosine function
*    mag           magnitude of a vector
* --------------------------------------------------------------------------- */

	double  angle
	(
		double vec1[3],
		double vec2[3]
	)
	{
		double small, magv1, magv2, temp;
		small = 0.00000001;

		magv1 = mag(vec1);
		magv2 = mag(vec2);

		if (magv1*magv2 > small*small)
		{
			temp = dot(vec1, vec2) / (magv1*magv2);
			if (fabs(temp) > 1.0)
				temp = sgn(temp) * 1.0;
			return acos(temp);
		}
		else
			return undefined;
	}  // angle


/* -----------------------------------------------------------------------------
*
*                           procedure roti
*
*  this procedure performs a rotation about the ith axis. i is specified
*    for each operation.
*
*  author        : david vallado                  719-573-2600    1 mar 2001
*
*  inputs          description                    range / units
*    vec         - input vector
*    xval        - angle of rotation              rad
*
*  outputs       :
*    outvec      - vector result
*
*  locals        :
*    c           - cosine of the angle xval
*    s           - sine of the angle xval
*    temp        - temporary extended value
*
*  coupling      :
*    none.
* --------------------------------------------------------------------------- */

	void    rot1
	(
		double vec[3],
		double xval,
		double outvec[3]
	)
	{
		double c, s, temp;

		temp = vec[2];
		c = cos(xval);
		s = sin(xval);

		outvec[2] = c * vec[2] - s * vec[1];
		outvec[1] = c * vec[1] + s * temp;
		outvec[0] = vec[0];
	}  //  rot1

	void    rot2
	(
		double vec[3],
		double xval,
		double outvec[3]
	)
	{
		double c, s, temp;

		temp = vec[2];
		c = cos(xval);
		s = sin(xval);

		outvec[2] = c * vec[2] + s * vec[0];
		outvec[0] = c * vec[0] - s * temp;
		outvec[1] = vec[1];
	}  // rot2

	void    rot3
	(
		double vec[3],
		double xval,
		double outvec[3]
	)
	{
		double c, s, temp;

		temp = vec[1];
		c = cos(xval);
		s = sin(xval);

		outvec[1] = c * vec[1] - s * vec[0];
		outvec[0] = c * vec[0] + s * temp;
		outvec[2] = vec[2];
	}  // rot3

/* -----------------------------------------------------------------------------
*
*                                  rotimat
*
*  this function sets up a rotation matrix for an input angle about the first
*    axis.
*
*  author        : david vallado                  719-573-2600   10 jan 2003
*
*  revisions
*                -
*
*  inputs          description                    range / units
*    xval        - angle of rotation              rad
*
*  outputs       :
*    outmat      - matrix result
*
*  locals        :
*    c           - cosine of the angle xval
*    s           - sine of the angle xval
*
*  coupling      :
*    none.
* --------------------------------------------------------------------------- */

	void    rot1mat
	(
		double xval,
		std::vector< std::vector<double> > &outmat
	)
	{
		outmat.resize(3);  // rows
		for (std::vector< std::vector<double> >::iterator it = outmat.begin(); it != outmat.end(); ++it)
			it->resize(3);
		double c, s;
		c = cos(xval);
		s = sin(xval);

		outmat[0][0] = 1.0;
		outmat[0][1] = 0.0;
		outmat[0][2] = 0.0;

		outmat[1][0] = 0.0;
		outmat[1][1] = c;
		outmat[1][2] = s;

		outmat[2][0] = 0.0;
		outmat[2][1] = -s;
		outmat[2][2] = c;
	}  //  rot1mat


	void    rot2mat
	(
		double xval,
		std::vector< std::vector<double> > &outmat
	)
	{
		outmat.resize(3);  // rows
		for (std::vector< std::vector<double> >::iterator it = outmat.begin(); it != outmat.end(); ++it)
			it->resize(3);
		double c, s;
		c = cos(xval);
		s = sin(xval);

		outmat[0][0] = c;
		outmat[0][1] = 0.0;
		outmat[0][2] = -s;

		outmat[1][0] = 0.0;
		outmat[1][1] = 1.0;
		outmat[1][2] = 0.0;

		outmat[2][0] = s;
		outmat[2][1] = 0.0;
		outmat[2][2] = c;
	}  // rot2mat

	void    rot3mat
	(
		double xval,
		std::vector< std::vector<double> > &outmat
	)
	{
		outmat.resize(3);  // rows
		for (std::vector< std::vector<double> >::iterator it = outmat.begin(); it != outmat.end(); ++it)
			it->resize(3);
		double c, s;
		c = cos(xval);
		s = sin(xval);

		outmat[0][0] = c;
		outmat[0][1] = s;
		outmat[0][2] = 0.0;

		outmat[1][0] = -s;
		outmat[1][1] = c;
		outmat[1][2] = 0.0;

		outmat[2][0] = 0.0;
		outmat[2][1] = 0.0;
		outmat[2][2] = 1.0;
	}  // rot3mat


/* -----------------------------------------------------------------------------
*
*                           procedure addvec
*
*  this procedure adds two vectors possibly multiplied by a constant.
*
*  author        : david vallado                  719-573-2600    1 mar 2001
*
*  inputs          description                    range / units
*    a1          - constant multiplier
*    a2          - constant multiplier
*    vec1        - vector number 1
*    vec2        - vector number 2
*
*  outputs       :
*    outvec      - vector result of a + b
*
*  locals        :
*    row         - index
*
*  coupling      :
*     none
* --------------------------------------------------------------------------- */

	void    addvec
	(
		double a1, double vec1[3],
		double a2, double vec2[3],
		double vec3[3]
	)
	{
		int row;

		for (row = 0; row <= 2; row++)
		{
			vec3[row] = 0.0;
			vec3[row] = a1 * vec1[row] + a2 * vec2[row];
		}
	}  // addvec

/* -----------------------------------------------------------------------------
*
*                           procedure addvec3
*
*  this procedure adds three vectors possibly multiplied by a constant.
*
*  author        : david vallado                  719-573-2600    1 mar 2001
*
*  inputs          description                    range / units
*    a1          - constant multiplier
*    a2          - constant multiplier
*    a3          - constant multiplier
*    vec1        - vector number 1
*    vec2        - vector number 2
*    vec3        - vector number 3
*
*  outputs       :
*    outvec      - vector result of a + b + c
*
*  locals        :
*    row         - index
*
*  coupling      :
*     none
* --------------------------------------------------------------------------- */

	void    addvec3
	(
		double a1, double vec1[3],
		double a2, double vec2[3],
		double a3, double vec3[3],
		double vec4[3]
	)
	{
		int row;

		for (row = 0; row <= 2; row++)
		{
			vec4[row] = 0.0;
			vec4[row] = a1 * vec1[row] + a2 * vec2[row] + a3 * vec3[row];
		}
	}  // addvec3



// this writes a vector out to the screen
	void    writevec
	(
		char title[10],
		double r[3], double v[3], double a[3]
	)
	{
		printf("%10s  %15.8f%15.8f%15.8f", title, r[0], r[1], r[2]);
		printf(" v %15.9f%15.9f%15.9f", v[0], v[1], v[2]);
		printf(" a %14.9f%14.9f%14.9f\n", a[0], a[1], a[2]);
	}  //  writevec


/* -----------------------------------------------------------------------------
*
*                           procedure matvecmult
*
*  this procedure multiplies a 3x3 matrix and a 3x1 vector together.
*
*  author        : david vallado                  719-573-2600    1 mar 2001
*
*  inputs          description                    range / units
*    mat         - 3 x 3 matrix
*    vec         - vector
*
*  outputs       :
*    vecout      - vector result of mat * vec
*
*  locals        :
*    row         - row index
*    col         - column index
*    ktr         - index
*
*  coupling      :
* --------------------------------------------------------------------------- */

	void    matvecmult
	(
		std::vector< std::vector<double> > mat,
		//          double mat[3][3],
		double vec[3],
		double vecout[3]
	)
	{
		int row, ktr;

		for (row = 0; row <= 2; row++)
		{
			vecout[row] = 0.0;
			for (ktr = 0; ktr <= 2; ktr++)
				vecout[row] = vecout[row] + mat[row][ktr] * vec[ktr];
		}
	}  // matvecmult


		// -----------------------------------------------------------------------------
		// perform vector outer product
		// makes a 3x3 matrix from multiplying 2 [3] dimensioned vectors
	void vecouter
	(
		double vec1[3], double vec2[3],
		std::vector< std::vector<double> > &mat
	)
	{
		mat.resize(3);  // rows
		for (std::vector< std::vector<double> >::iterator it = mat.begin(); it != mat.end(); ++it)
			it->resize(3);
		int row, col;
		for (row = 0; row <= 2; row++)
		{
			for (col = 0; col <= 2; col++)
			{
				mat[row][col] = vec1[row] * vec2[col];
			}
		}
	} // vecouter  


			// -----------------------------------------------------------------------------
		// matrixes must be the same size
	void matadd
	(
		std::vector< std::vector<double> > mat1,
		std::vector< std::vector<double> > mat2,
		int mat1r, int mat1c,
		std::vector< std::vector<double> > &mat3
	)
	{
		int row, col;
		mat3.resize(mat1r);  // rows
		for (std::vector< std::vector<double> >::iterator it = mat3.begin(); it != mat3.end(); ++it)
			it->resize(mat1c);
		for (row = 0; row < mat1r; row++)
		{
			for (col = 0; col < mat1c; col++)
				mat3[row][col] = mat1[row][col] + mat2[row][col];
		}
	} // matadd  


	// -----------------------------------------------------------------------------
	// matrixes must be the same size
	void matsub
	(
		std::vector< std::vector<double> > mat1,
		std::vector< std::vector<double> > mat2,
		int mat1r, int mat1c,
		std::vector< std::vector<double> > &mat3
	)
	{
		int row, col;
		mat3.resize(mat1r);  // rows
		for (std::vector< std::vector<double> >::iterator it = mat3.begin(); it != mat3.end(); ++it)
			it->resize(mat1c);
		for (row = 0; row < mat1r; row++)
		{
			for (col = 0; col < mat1c; col++)
				mat3[row][col] = mat1[row][col] - mat2[row][col];
		}
	} // matsub  


		/* -----------------------------------------------------------------------------
*
*                           procedure matmult
*
*  this procedure multiplies two matricies up to 10x10 together.
*
*  author        : david vallado                  719-573-2600    7 dec 2007
*
*  inputs          description                    range / units
*    mat1        - matrix number 1
*    mat2        - matrix number 2
*    mat1r       - matrix number 1 rows
*    mat1c       - matrix number 1 columns
*    mat2c       - matrix number 2 columns
*
*  outputs       :
*    mat3        - matrix result of mat1 * mat2 of size mat1r x mat2c
*
*  locals        :
*    row         - row index
*    col         - column index
*    ktr         - index
*
*  coupling      :
* --------------------------------------------------------------------------- */

	void    matmult
	(
		std::vector< std::vector<double> > mat1,
		std::vector< std::vector<double> > mat2,
		std::vector< std::vector<double> > &mat3,
		//          double mat1[3][3],
		//          double mat2[3][3],
		//          double mat3[3][3],
		int mat1r, int mat1c, int mat2c
	)
	{
		int row, col, ktr;
		// specify the actual sizes
		mat3.resize(mat1r);  // rows
		for (std::vector< std::vector<double> >::iterator it = mat3.begin(); it != mat3.end(); ++it)
			it->resize(mat2c);

		for (row = 0; row < mat1r; row++)
		{
			for (col = 0; col < mat2c; col++)
			{
				mat3[row][col] = 0.0;
				for (ktr = 0; ktr < mat1c; ktr++)
					mat3[row][col] = mat3[row][col] + mat1[row][ktr] * mat2[ktr][col];
			}
		}
	}  // matmult


/* -----------------------------------------------------------------------------
*
*                           procedure mattrans
*
*  this procedure finds the transpose of a matrix.
*
*  author        : david vallado                  719-573-2600    1 mar 2001
*
*  inputs          description                    range / units
*    mat1        - matrix number 1
*    mat1r       - matrix number 1 rows
*    mat1c       - matrix number 1 columns
*
*  outputs       :
*    mat2        - matrix result of transpose mat2
*
*  locals        :
*    row         - row index
*    col         - column index
*
*  coupling      :
* --------------------------------------------------------------------------- */

	void    mattrans
	(
		std::vector< std::vector<double> > mat1,
		std::vector< std::vector<double> > &mat2,
		//          double mat1[3][3],
		//          double mat2[3][3],
		int mat1r, int mat1c
	)
	{
		int row, col;

		mat2.resize(mat1c);  // rows
		for (std::vector< std::vector<double> >::iterator it = mat2.begin(); it != mat2.end(); ++it)
			it->resize(mat1r);

		for (row = 0; row < mat1r; row++)
		{
			for (col = 0; col < mat1c; col++)
				mat2[col][row] = mat1[row][col];
		}
	}  // mattrans



/* ------------------------------------------------------------------------------
*
*                           procedure ludecomp
*
*  this procedure decomposes a matrix into an lu form.
*
*  author        : david vallado                  719-573-2600    1 mar 2001
*
*  inputs          description                    range / units
*    order       - order of matrix
*
*  outputs       :
*    lu          - lu decomposition matrix
*    index       - index vector for pivoting
*
*  locals        :
*    i           - index
*    j           - index
*    k           - index
*    imax        - pivot row pointer
*    scale       - scale factor vector
*    sum         - temporary variables
*    amax        - temporary variables
*    dum         - temporary variables
*
*  coupling      :
*    none
*
*  references    :
*    numerical recipes - flannery
 ----------------------------------------------------------------------------- */

	void ludecomp
	(
		std::vector< std::vector<double> > &lu,
		std::vector< int > &indexx,
		//          double lu[3][3],
		//          double indexx[3],
		int order
	)
	{
		const double small = 0.000001;
		int i, j, k, imax;
		//     std::vector< double > scale(order);
		std::vector< double > scale = std::vector< double >(order);

		double sum, amax, dum;

		imax = 0;
		for (i = 0; i < order; i++)
		{
			amax = 0.0;
			for (j = 0; j < order; j++)
				if (fabs(lu[i][j]) > amax)
					amax = fabs(lu[i][j]);
			if (fabs(amax) < small)
			{
				printf(" singular matrix amax ");
			}
			scale[i] = 1.0 / amax;
		}
		for (j = 0; j < order; j++)
		{
			for (i = 0; i <= j - 1; i++)
			{
				sum = lu[i][j];
				for (k = 0; k <= i - 1; k++)
					sum = sum - lu[i][k] * lu[k][j];
				lu[i][j] = sum;
			}
			amax = 0.0;
			for (i = j; i < order; i++)
			{
				sum = lu[i][j];
				for (k = 0; k <= j - 1; k++)
					sum = sum - lu[i][k] * lu[k][j];
				lu[i][j] = sum;
				dum = scale[i] * fabs(sum);
				if (dum >= amax)
				{
					imax = i;
					amax = dum;
				}
			}
			if (j != imax)
			{
				for (k = 0; k < order; k++)
				{
					dum = lu[imax][k];
					lu[imax][k] = lu[j][k];
					lu[j][k] = dum;
				}
				scale[imax] = scale[j];
			}
			indexx[j] = imax;
			if (fabs(lu[j][j]) < small)
			{
				printf(" matrix is singular lu ");
			}
			if (j != order - 1)
			{
				dum = 1.0 / lu[j][j];
				for (i = j + 1; i < order; i++)
					lu[i][j] = dum * lu[i][j];
			}
		}
	} // ludecmp

/* ------------------------------------------------------------------------------
*
*                           procedure lubksub
*
*  this procedure finds the inverse of a matrix using lu decomposition.
*
*  author        : david vallado                  719-573-2600    1 mar 2001
*
*  inputs          description                    range / units
*    order       - order of matrix
*    lu          - lu decomposition matrix
*    index       - index vector for pivoting
*
*  outputs       :
*    b           - solution vector
*
*  locals        :
*    i           - index
*    j           - index
*    i0          - pointer to non-zero element
*    iptr        - pivot rwo pointer
*    sum         - temporary variables
*
*  coupling      :
*    none
*
*  references    :
*    numerical recipes - flannery
 ----------------------------------------------------------------------------- */

	void lubksub
	(
		std::vector< std::vector<double> > lu,
		std::vector< int > indexx,
		//          double lu[3][3],
		//          double indexx[3],
		int order,
		std::vector< double > &b
		//          double b[3]
	)
	{
		int i, j, iptr, ii;
		double sum;

		ii = -1;
		for (i = 0; i < order; i++)
		{
			iptr = indexx[i];
			sum = b[iptr];
			b[iptr] = b[i];
			if (ii != -1)
				for (j = ii; j <= i - 1; j++)
					sum = sum - lu[i][j] * b[j];
			else
				if (sum != 0.0)
					ii = i;
			b[i] = sum;
		}

		b[order - 1] = b[order - 1] / lu[order - 1][order - 1];

		for (i = order - 2; i >= 0; i--)
		{
			sum = b[i];
			for (j = i + 1; j < order; j++)
				sum = sum - lu[i][j] * b[j];
			b[i] = sum / lu[i][i];
		}
	}  // lubksub

/* ------------------------------------------------------------------------------
*
*                           procedure matinverse
*
*  this procedure finds the inverse of a matrix using lu decomposition.
*
*  author        : david vallado                  719-573-2600    1 mar 2001
*
*  inputs          description                    range / units
*    mat         - matrix to invert, 0 array starts
*    order       - order of matrix
*
*  outputs       :
*    matinv      - inverted matrix, 0 array starts
*
*  locals        :
*    i           - index
*    j           - index
*    index       - index vector for pivoting
*    lu          - lu decomposition matrix
*    b           - operational vector to form matinv
*
*  coupling      :
*    ludecomp    -
*    lubksub     -
*
*  references    :
*    numerical recipes - flannery
 ----------------------------------------------------------------------------- */

	void matinverse
	(
		std::vector< std::vector<double> > mat,
		//          double mat[3][3],
		int  order,
		std::vector< std::vector<double> > &matinv
		//          double matinv[3][3]
	)
	{
		int i, j;
		std::vector< int > indexx(order);
		//std::vector< std::vector<double> > lu(order,order);
		//std::vector< std::vector<double> >  b(order,2);
		std::vector< std::vector<double> > lu = std::vector< std::vector<double> >(order, std::vector<double>(order, 0.0));
		std::vector< double >  b = std::vector< double >(order);

		matinv.resize(order);  // rows
		for (std::vector< std::vector<double> >::iterator it = matinv.begin(); it != matinv.end(); ++it)
			it->resize(order);

		for (i = 0; i < order; i++)
		{
			indexx[i] = i;
			for (j = 0; j < order; j++)
				lu[i][j] = mat[i][j];
		}
		ludecomp(lu, indexx, order);
		for (j = 0; j < order; j++)
		{
			for (i = 0; i < order; i++)
			{
				if (i == j)
					b[i] = 1.0;
				else
					b[i] = 0.0;
			}

			lubksub(lu, indexx, order, b);
			for (i = 0; i < order; i++)
				matinv[i][j] = b[i];
		}
	} // matinverse


/* ----------------------------------------------------------------------------
*
*                           procedure determinant
*
*  This function calculates the determinant value using L - U decompisition.
*    The formula must have a non-zero number in the 1, 1 position. if the
*    function senses a non-zero number in row 1, it exchanges row1 for a row
*    with a non-zero number.
*
*  author        : david vallado                  719-573-2600    1 mar 2001
*
*  inputs          description                    range / units
*    mat1        - matrix to find determinant of
*    order       - order of matrix
*
*  outputs       :
*    determinant - determinant of mat1
*
*  locals        :
*    i, j, k, n  - index
*    index       - index vector for pivoting
*    lu          - lu decomposition matrix
*    b           - operational vector to form matinv
*
*  coupling      :
*    ludecomp    -
*    lubksub     -
*
*  references    :
*    Marion        pg. 168 - 172, 126 - 127
 ---------------------------------------------------------------------------- - */

	double determinant
	(
		std::vector< std::vector<double> > mat1,
		int  order
	)
	{
		double small = 0.00000001;
		int i, j, k;
		double temp, d, sum;
		std::vector< std::vector<double> > l = std::vector< std::vector<double> >(order, std::vector<double>(order, 0.0));
		std::vector< std::vector<double> > u = std::vector< std::vector<double> >(order, std::vector<double>(order, 0.0));

		sum = 0.0;
		// ----------- Switch a non zero row to the first row---------- 
		if (abs(mat1[0][0]) < small)
		{
			j = 0;
			while (j < order)
			{
				if (abs(mat1[j][0]) > small)
				{
					for (k = 0; k < order; k++)
					{
						temp = mat1[0][k];
						mat1[0][k] = mat1[j][k];
						mat1[j][k] = temp;
					}
					j = order + 1;
				}
				j = j + 1;
			} // if abs
		}

		for (i = 0; i < order; i++)
			l[i][0] = mat1[i][0];
		for (j = 0; j < order; j++)
			u[0][j] = mat1[0][j] / l[0][0];
		for (j = 1; j < order; j++)
		{
			for (i = j; i < order; i++)
			{
				sum = 0.0;
				for (k = 0; k <= j - 1; k++)
					sum = sum + l[i][k] * u[k][j];
				l[i][j] = mat1[i][j] - sum;
			} // for i 
			u[j][j] = 1.0;
			if (j != order)
			{
				for (i = j + 1; i < order; i++)
				{
					sum = 0.0;
					for (k = 0; k <= j - 1; k++)
						sum = sum + l[j][k] * u[k][i];
					u[j][i] = (mat1[j][i] - sum) / l[j][j];
				} // for i 
			} // if j 
		} //  for j 
		d = 1.0;
		for (i = 0; i < order; i++)
			d = d * l[i][i];
		return d;
	} // determinant


		   // https://rosettacode.org/wiki/Cholesky_decomposition#C.23
		// Content is available under GNU Free Documentation License 1.2 unless otherwise noted.
		// Returns the lower Cholesky Factor, L, of input matrix A. 
		// Satisfies the equation: L*L^T = A.
		// </summary>
		// <param name="a">Input matrix must be square, symmetric, 
		// and positive definite. This method does not check for these properties,
		// and may produce unexpected results of those properties are not met.</param>
		// <returns></returns>
	void cholesky
	(
		std::vector< std::vector<double> > a,
		int n,
		std::vector< std::vector<double> > &ret
	)
	{
		ret.resize(n);  // rows
		for (std::vector< std::vector<double> >::iterator it = ret.begin(); it != ret.end(); ++it)
			it->resize(n);

		for (int r = 0; r < n; r++)
			for (int c = 0; c <= r; c++)
			{
				if (c == r)
				{
					double sum = 0;
					for (int j = 0; j < c; j++)
					{
						sum = sum + ret[c][j] * ret[c][j];
					}
					ret[c][c] = sqrt(a[c][c] - sum);
				}
				else
				{
					double sum = 0;
					for (int j = 0; j < c; j++)
						sum = sum + ret[r][j] * ret[c][j];
					ret[r][c] = 1.0 / ret[c][c] * (a[r][c] - sum);
				}
			}

	}   // cholesky


	void writemat
	(
		char matname[30],
		std::vector< std::vector<double> > mat,
		//          double mat[3][3],
		int row, int col
	)
	{
		int r, c;

		printf("matrix %15s \n", matname);
		for (r = 0; r < row; r++)
		{
			for (c = 0; c < col; c++)
				printf("%16.11f ", mat[r][c]);
			printf(" \n");
		}
	}  // writemat

	void writeexpmat
	(
		char matname[30],
		std::vector< std::vector<double> > mat,
		//          double mat[3][3],
		int row, int col
	)
	{
		int r, c;

		printf("matrix %15s \n", matname);
		for (r = 0; r < row; r++)
		{
			for (c = 0; c < col; c++)
				printf("%14g ", mat[r][c]);
			printf(" \n");
		}
	}  // writeexpmat

/* -----------------------------------------------------------------------------
*
*                           function cubicspl
*
*  this function performs cubic splining of an input zero crossing
*  function in order to find function values.
*
*  author        : david vallado                  719-573-2600     7 aug 2005
*
*  revisions
*                -
*  inputs          description                    range / units
*    p0,p1,p2,p3 - function values used for splining
*    t0,t1,t2,t3 - time values used for splining
*
*  outputs       :
*    acu0..acu3  - splined polynomial coefficients. acu3 t^3, etc
*
*  locals        : none
*
*  coupling      :
*    none
*
*  references    :
*    vallado       2013, 1034
* --------------------------------------------------------------------------- */

	void cubicspl
	(
		double p1, double p2, double p3, double p4,
		double& acu0, double& acu1, double& acu2, double& acu3
	)
	{
		acu0 = p2;
		acu1 = -p1 / 3.0 - 0.5 * p2 + p3 - p4 / 6.0;
		acu2 = 0.5 * p1 - p2 + 0.5 * p3;
		acu3 = -p1 / 6.0 + 0.5 * p2 - 0.5 * p3 + p4 / 6.0;
	}  // cubicspl

/* -----------------------------------------------------------------------------
*
*                           function quadric
*
*  this function solves for the two roots of a quadric equation.  there are
*    no restrictions on the coefficients, and imaginary results are passed
*    out as separate values.  the general form is y = ax2 + bx + c.
*
*  author        : david vallado                  719-573-2600    1 mar 2001
*
*  revisions
*    vallado     - convert to matlab              719-573-2600    3 dec 2002
*
*  inputs          description                    range / units
*    a           - coefficient of x squared term
*    b           - coefficient of x term
*    c           - constant
*    opt         - option for output              I all roots including imaginary
*                                                 R only real roots
*                                                 U only unique real roots (no repeated)
*
*  outputs       :
*    r1r         - real portion of root 1
*    r1i         - imaginary portion of root 1
*    r2r         - real portion of root 2
*    r2i         - imaginary portion of root 2
*
*  locals        :
*    discrim     - discriminate b2 - 4ac
*
*  coupling      :
*    none.
*
*  references    :
*    vallado       2013, 1027
* ----------------------------------------------------------------------------*/

	void quadratic
	(
		double a, double b, double c, char opt,
		double& r1r, double& r1i, double& r2r, double& r2i
	)
	{
		const double small = 0.0000001;
		double discrim;
		// --------------------  implementation   ----------------------
		r1r = 0.0;
		r1i = 0.0;
		r2r = 0.0;
		r2i = 0.0;

		discrim = b * b - 4.0 *a*c;

		// ---------------------  real roots  --------------------------
		if (fabs(discrim) < small)
		{
			r1r = -b / (2.0 *a);
			r2r = r1r;
			if (opt == 'U')
				r2r = 99999.9;
		}
		else
		{
			if (fabs(a) < small)
				r1r = -c / b;
			else
			{
				if (discrim > 0.0)
				{
					r1r = (-b + sqrt(discrim)) / (2.0 *a);
					r2r = (-b - sqrt(discrim)) / (2.0 *a);
				}
				else
				{
					// ------------------ complex roots --------------------
					if (opt == 'I')
					{
						r1r = -b / (2.0 *a);
						r2r = r1r;
						r1i = sqrt(-discrim) / (2.0 *a);
						r2i = -sqrt(-discrim) / (2.0 *a);
					}
					else
					{
						r1r = 99999.9;
						r2r = 99999.9;
					}
				}
			}
		}
	}  // quadratic


/* -----------------------------------------------------------------------------
*
*                           function cubic
*
*  this function solves for the three roots of a cubic equation.  there are
*    no restrictions on the coefficients, and imaginary results are passed
*    out as separate values.  the general form is y = a3x3 + b2x2 + c1x + d0.  note
*    that r1i will always be zero since there is always at least one real root.
*
*  author        : david vallado                  719-573-2600    1 mar 2001
*
*  revisions
*    vallado     - convert to matlab              719-573-2600   18 dec 2002
*
*  inputs          description                    range / units
*    a3          - coefficient of x cubed term
*    b2          - coefficient of x squared term
*    c1          - coefficient of x term
*    d0          - constant
*    opt         - option for output              I all roots including imaginary
*                                                 R only real roots
*                                                 U only unique real roots (no repeated)
*
*  outputs       :
*    r1r         - real portion of root 1
*    r1i         - imaginary portion of root 1
*    r2r         - real portion of root 2
*    r2i         - imaginary portion of root 2
*    r3r         - real portion of root 3
*    r3i         - imaginary portion of root 3
*
*  locals        :
*    temp1       - temporary value
*    temp2       - temporary value
*    p           - coefficient of x squared term where x cubed term is 1.0
*    q           - coefficient of x term where x cubed term is 1.0
*    r           - coefficient of constant term where x cubed term is 1.0
*    delta       - discriminator for use with cardans formula
*    e0          - angle holder for trigonometric solution
*    phi         - angle used in trigonometric solution
*    cosphi      - cosine of phi
*    sinphi      - sine of phi
*
*  coupling      :
*    quadric     - quadratic roots
*
*  references    :
*    vallado       2013, 1027
* --------------------------------------------------------------------------- */

	void cubic
	(
		double a3, double b2, double c1, double d0, char opt,
		double& r1r, double& r1i, double& r2r, double& r2i, double& r3r, double& r3i
	)
	{
		const double rad = 57.29577951308230;
		const double onethird = 1.0 / 3.0;
		const double small = 0.00000001;
		double temp1, temp2, p, q, r, delta, e0, cosphi, sinphi, phi;
		// ------------------------  implementation   --------------------------
		r1r = 0.0;
		r1i = 0.0;
		r2r = 0.0;
		r2i = 0.0;
		r3r = 0.0;
		r3i = 0.0;

		if (fabs(a3) > small)
		{
			// ------------- force coefficients into std form -------------------
			p = b2 / a3;
			q = c1 / a3;
			r = d0 / a3;

			a3 = onethird * (3.0 * q - p * p);
			b2 = (1.0 / 27.0) * (2.0 * p * p * p - 9.0 * p * q + 27.0 * r);

			delta = (a3 * a3 * a3 / 27.0) + (b2 * b2 * 0.25);

			// -------------------- use cardans formula ------------------------
			if (delta > small)
			{
				temp1 = (-b2 * 0.5) + sqrt(delta);
				temp2 = (-b2 * 0.5) - sqrt(delta);
				temp1 = sgn(temp1) * pow(fabs(temp1), onethird);
				temp2 = sgn(temp2) * pow(fabs(temp2), onethird);
				r1r = temp1 + temp2 - p * onethird;

				if (opt == 'I')
				{
					r2r = -0.5 * (temp1 + temp2) - p * onethird;
					r2i = -0.5 * sqrt(3.0) * (temp1 - temp2);
					r3r = -0.5 * (temp1 + temp2) - p * onethird;
					r3i = -r2i;
				}
				else
				{
					r2r = 99999.9;
					r3r = 99999.9;
				}
			}
			else
			{
				// -------------------- evaluate zero point ------------------------
				if (fabs(delta) < small)
				{
					r1r = -2.0 * sgn(b2) * pow(fabs(b2 * 0.5), onethird) - p * onethird;
					r2r = sgn(b2) * pow(fabs(b2 * 0.5), onethird) - p * onethird;
					if (opt == 'U')
						r3r = 99999.9;
					else
						r3r = r2r;
				}
				else
				{
					// --------------- use trigonometric identities ----------------
					e0 = 2.0 * sqrt(-a3 * onethird);
					cosphi = (-b2 / (2.0 *sqrt(-a3 * a3 * a3 / 27.0)));
					sinphi = sqrt(1.0 - cosphi * cosphi);
					phi = atan2(sinphi, cosphi);
					if (phi < 0.0)
						phi = phi + 2.0 * pi;
					r1r = e0 * cos(phi * onethird) - p * onethird;
					r2r = e0 * cos(phi * onethird + 120.0 / rad) - p * onethird;
					r3r = e0 * cos(phi * onethird + 240.0 / rad) - p * onethird;
				} // if fabs(delta)...
			}  // if delta > small
		}  // if fabs > small
		else
		{
			quadratic(b2, c1, d0, opt, r1r, r1i, r2r, r2i);
			r3r = 99999.9;
			r3i = 99999.9;
		}
	}  // cubic

/* -----------------------------------------------------------------------------
*
*                           function cubicinterp
*
*  this function performs a cubic spline. four points are needed.
*
*  author        : david vallado                  719-573-2600   1 dec  2005
*
*  revisions
*
*  inputs          description                    range / units
*    valuein     - kp
*
*  outputs       :
*    out         - ap
*
*  locals        :
*                -
*
*  coupling      :
*    cubicspl
*
*  references    :
*    vallado       2013, 1034
* --------------------------------------------------------------------------- */

	double  cubicinterp
	(
		double p1a, double p1b, double p1c, double p1d, double p2a, double p2b,
		double p2c, double p2d, double valuein
	)
	{
		double kc0, kc1, kc2, kc3, ac0, ac1, ac2, ac3,
			r1r, r1i, r2r, r2i, r3r, r3i, value;

		//p2b = (p2b - p2a) / (p2d - p2a);
		//p2c = (p2c - p2a) / (p2d - p2a);
		//p2a = 0.0;
		//p2d = 1.0;

		// -------- assign function points ---------
		cubicspl(p1a, p1b, p1c, p1d, ac0, ac1, ac2, ac3);
		cubicspl(p2a, p2b, p2c, p2d, kc0, kc1, kc2, kc3);

		// recover the original function values
		// use the normalized time first, but at an arbitrary interval
		cubic(kc3, kc2, kc1, kc0 - valuein, 'R', r1r, r1i, r2r, r2i, r3r, r3i);

		//if ((r1r >= -0.000001) &&(r1r <= 1.001))
		if (fabs(r1i) < 0.000001)
		{
			value = r1r;
		}
		else
		{
			//if ((r2r >= -0.000001) && (r2r <= 1.001))
			if (fabs(r2i) < 0.000001)
			{
				value = r2r;
			}
			else
			{
				//if ((r3r >= -0.000001) && (r3r <= 1.001))
				if (fabs(r3i) <= 0.000001)
				{
					value = r3r;
				}
				else
				{
					value = 0.0;
					printf("\nerror in cubicinterp root %17.14f %11.7f %11.7f %11.7f \n",
						valuein, r1r, r2r, r3r);
					printf("valuein %lf value(pos root) %lf \n", valuein, value);
				}
			}
		}

		//printf("valuein %lf value(pos root) %lf %lf\n", valuein, value, ac3 * pow(value, 3) + ac2 * value * value + ac1 * value + ac0);
		//printf("in p1a %lf  %lf  %lf  %lf cubspl  %lf %lf  %lf  %lf  %lf \n", p1a, p1b, p1c, p1d, ac0, ac1, ac2, ac3);
		//printf("in p2a %lf  %lf  %lf  %lf cubspl  %lf %lf  %lf  %lf  %lf \n", p2a, p2b, p2c, p2d, kc0, kc1, kc2, kc3);
		//printf("cubic %lf  %lf  %lf  %lf cubspl  %lf %lf r2 %lf  %lf r3 %lf  %lf \n", kc3, kc2, kc1, kc0 - valuein, r1r, r1i, r2r, r2i, r3r, r3i);
		return (ac3 * pow(value, 3) + ac2 * value * value + ac1 * value + ac0);
	} // cubicinterp


/* -----------------------------------------------------------------------------
*
*                           function factorial
*
* this function performs a factorial. note the use of double in the return as the
*  numbers get huge quickly. this is good to about n = 170.
*
*  author        : david vallado                  719-573-2600   11 feb 2016
*
*  revisions
*
*  inputs          description                    range / units
*    n           - order in
*
*  outputs       :
*    factorial   - result
*
*  locals        :
*                -
*
*  coupling      :
*    none
* --------------------------------------------------------------------------- */

	double factorial(long n)
	{
		if (n == 0)
			return 1;
		return n * factorial(n - 1);
	}  // factorial 



	   // -----------------------------------------------------------------------------------------
	   //                                       time functions
	   // -----------------------------------------------------------------------------------------

	int getmon
	(
		char instr[3]
	)
	{
		int ktr;
		typedef char str3[4];
		str3 monstr[13];

		// ------------------------  implementation   --------------------------
		strcpy_s(monstr[1], "Jan");
		strcpy_s(monstr[2], "Feb");
		strcpy_s(monstr[3], "Mar");
		strcpy_s(monstr[4], "Apr");
		strcpy_s(monstr[5], "May");
		strcpy_s(monstr[6], "Jun");
		strcpy_s(monstr[7], "Jul");
		strcpy_s(monstr[8], "Aug");
		strcpy_s(monstr[9], "Sep");
		strcpy_s(monstr[10], "Oct");
		strcpy_s(monstr[11], "Nov");
		strcpy_s(monstr[12], "Dec");

		ktr = 1;
		while ((strcmp(instr, monstr[ktr]) != 0) && (ktr <= 12))
			ktr = ktr + 1;

		return (ktr);
	} // getmon


	/*       ----------------------------------------------------------------      */
	int getday
	(
		char instr[3]
	)
	{
		int ktr;
		typedef char str3[4];
		str3 monstr[8];

		// ------------------------  implementation   --------------------------
		strcpy_s(monstr[1], "Sun");
		strcpy_s(monstr[2], "Mon");
		strcpy_s(monstr[3], "Tue");
		strcpy_s(monstr[4], "Wed");
		strcpy_s(monstr[5], "Thr");
		strcpy_s(monstr[6], "Fri");
		strcpy_s(monstr[7], "Sat");

		ktr = 1;
		while ((strcmp(instr, monstr[ktr]) != 0) && (ktr <= 7))
			ktr = ktr + 1;

		return (ktr);
	} // getday


	/* -----------------------------------------------------------------------------
	*
	*                           function dayofweek
	*
	*  this function finds the day of the week. integers are used for the days,
	*    1 = 'Sun', 2 = 'Mon', ... 7 = 'Sat'.
	*
	*  Author        : David Vallado                  719-573-2600    1 Mar 2001
	*
	*  Inputs          Description                    Range / Units
	*    JD          - Julian date of interest        days from 4713 BC
	*
	*  OutPuts       :
	*    dayofweek   - answer                         1 to 7
	*
	*  Locals        :
	*    None.
	*
	*  Coupling      :
	*    None.
	*
	*  References    :
	* --------------------------------------------------------------------------- */

	int dayofweek
	(
		double jd
	)
	{
		int temp;
		// ----- Be sure jd is at 0.0 h on the day of interest -----
		jd = floor(jd + 0.5);

		temp = int(floor(jd - 7 * floor((jd + 1) / 7) + 2));
		return temp;
	}  // function dayofweek


	/* -----------------------------------------------------------------------------
	*
	*                           procedure jday
	*
	*  this procedure finds the julian date given the year, month, day, and time.
	*    the julian date is defined by each elapsed day since noon, jan 1, 4713 bc.
	*
	*  algorithm     : calculate the answer in one step for efficiency
	*
	*  author        : david vallado                  719-573-2600    1 mar 2001
	*
	*  inputs          description                    range / units
	*    year        - year                           1900 .. 2100
	*    mon         - month                          1 .. 12
	*    day         - day                            1 .. 28,29,30,31
	*    hr          - universal time hour            0 .. 23
	*    min         - universal time min             0 .. 59
	*    sec         - universal time sec             0.0 .. 59.999
	*
	*  outputs       :
	*    jd          - julian date (days only)           days from 4713 bc
	*    jdFrac      - julian date (fraction of a day)   days from 0 hr of the day
	*
	*  locals        :
	*    none.
	*
	*  coupling      :
	*    none.
	*
	*  references    :
	*    vallado       2013, 183, alg 14, ex 3-4
	*
	* --------------------------------------------------------------------------- */

	void jday
	(
		int year, int mon, int day, int hr, int minute, double sec,
		double& jd, double& jdFrac
	)
	{
		jd = 367.0 * year -
			floor((7 * (year + floor((mon + 9) / 12.0))) * 0.25) +
			floor(275 * mon / 9.0) +
			day + 1721013.5;  // use - 678987.0 to go to mjd directly
		jdFrac = (sec + minute * 60.0 + hr * 3600.0) / 86400.0;

		// check that the day and fractional day are correct
		if (fabs(jdFrac) >= 1.0)
		{
			double dtt = floor(jdFrac);
			jd = jd + dtt;
			jdFrac = jdFrac - dtt;
		}

		// - 0.5*sgn(100.0*year + mon - 190002.5) + 0.5;
	}

	/* -----------------------------------------------------------------------------
	*
	*                           procedure jdayall
	*
	*  this procedure finds the julian date given the year, month, day, and time.
	*    the julian date is defined by each elapsed day since noon, jan 1, 4713 bc.
	*
	*  algorithm     : calculate the answer in one step for efficiency
	*
	*  author        : david vallado                  719-573-2600    1 mar 2001
	*
	*  inputs          description                    range / units
	*    year        - year                           all, 1900 .. 2100
	*    mon         - month                          1 .. 12
	*    day         - day                            1 .. 28,29,30,31
	*    hr          - universal time hour            0 .. 23
	*    min         - universal time min             0 .. 59
	*    sec         - universal time sec             0.0 .. 59.999
	*    whichtype   - julian or gregorian calender   'j' or 'g'
	*
	*  outputs       :
	*    jd          - julian date                    days from 4713 bc
	*
	*  locals        :
	*    b           - var to aid gregorian dates
	*
	*  coupling      :
	*    none.
	*
	*  references    :
	*    vallado       2013, 183, alg 14, ex 3-4
	* --------------------------------------------------------------------------- */

	void    jdayall
	(
		int year, int mon, int day, int hr, int minute, double sec,
		char whichtype, double& jd
	)
	{
		double b;

		if (mon <= 2)
		{
			year = year - 1;
			mon = mon + 12;
		}
		/* --------- use for julian calender, every 4 years --------- */
		if (whichtype == 'j')
			b = 0.0;
		else
		{
			/* ---------------------- use for gregorian ----------------- */
			b = 2 - floor(year * 0.01) + floor(floor(year * 0.01) * 0.25);
			jd = floor(365.25 * (year + 4716)) +
				floor(30.6001 * (mon + 1)) +
				day + b - 1524.5 +
				((sec / 60.0 + minute) / 60.0 + hr) / 24.0;  // ut in days
		}
	}

	/* -----------------------------------------------------------------------------
	*
	*                           procedure days2mdhms
	*
	*  this procedure converts the day of the year, days, to the equivalent month
	*    day, hour, minute and second.
	*
	*  algorithm     : set up array for the number of days per month
	*                  find leap year - use 1900 because 2000 is a leap year
	*                  loop through a temp value while the value is < the days
	*                  perform int conversions to the correct day and month
	*                  convert remainder into h m s using type conversions
	*
	*  author        : david vallado                  719-573-2600    1 mar 2001
	*
	*  inputs          description                    range / units
	*    year        - year                           1900 .. 2100
	*    days        - julian day of the year         0.0  .. 366.0
	*
	*  outputs       :
	*    mon         - month                          1 .. 12
	*    day         - day                            1 .. 28,29,30,31
	*    hr          - hour                           0 .. 23
	*    min         - minute                         0 .. 59
	*    sec         - second                         0.0 .. 59.999
	*
	*  locals        :
	*    dayofyr     - day of year
	*    temp        - temporary extended values
	*    inttemp     - temporary int value
	*    i           - index
	*    lmonth[12]  - int array containing the number of days per month
	*
	*  coupling      :
	*    none.
	* --------------------------------------------------------------------------- */

	void    days2mdhms
	(
		int year, double days,
		int& mon, int& day, int& hr, int& minute, double& sec
	)
	{
		int i, inttemp, dayofyr;
		double    temp;
		// start indicies at 1
		int lmonth[] = { 0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

		dayofyr = (int)floor(days);
		/* ----------------- find month and day of month ---------------- */
		if ((year % 4) == 0)
			lmonth[2] = 29;

		i = 1;
		inttemp = 0;
		while ((dayofyr > inttemp + lmonth[i]) && (i < 12))
		{
			inttemp = inttemp + lmonth[i];
			i++;
		}
		mon = i;
		day = dayofyr - inttemp;

		/* ----------------- find hours minutes and seconds ------------- */
		temp = (days - dayofyr) * 24.0;
		hr = floor(temp);
		temp = (temp - hr) * 60.0;
		minute = floor(temp);
		sec = (temp - minute) * 60.0;
	}

	/* -----------------------------------------------------------------------------
	*
	*                           procedure invjday
	*
	*  this procedure finds the year, month, day, hour, minute and second
	*  given the julian date. tu can be ut1, tdt, tdb, etc.
	*
	*  algorithm     : set up starting values
	*                  find leap year - use 1900 because 2000 is a leap year
	*                  find the elapsed days through the year in a loop
	*                  call routine to find each individual value
	*
	*  author        : david vallado                  719-573-2600    1 mar 2001
	*
	*  inputs          description                    range / units
	*    jd          - julian date (days only)           days from 4713 bc
	*    jdFrac      - julian date (fraction of a day)   days from 0 hr of the day
	*
	*  outputs       :
	*    year        - year                           1900 .. 2100
	*    mon         - month                          1 .. 12
	*    day         - day                            1 .. 28,29,30,31
	*    hr          - hour                           0 .. 23
	*    min         - minute                         0 .. 59
	*    sec         - second                         0.0 .. 59.999
	*
	*  locals        :
	*    days        - day of year plus fractional
	*                  portion of a day               days
	*    tu          - julian centuries from 0 h
	*                  jan 0, 1900
	*    temp        - temporary double values
	*    leapyrs     - number of leap years from 1900
	*
	*  coupling      :
	*    days2mdhms  - finds month, day, hour, minute and second given days and year
	*
	*  references    :
	*    vallado       2013, 203, alg 22, ex 3-13
	* --------------------------------------------------------------------------- */

	void    invjday
	(
		double jd, double jdfrac,
		int& year, int& mon, int& day,
		int& hr, int& minute, double& sec
	)
	{
		int leapyrs;
		double dt, days, tu, temp;

		// check jdfrac for multiple days
		if (fabs(jdfrac) >= 1.0)
		{
			jd = jd + floor(jdfrac);
			jdfrac = jdfrac - floor(jdfrac);
		}

		// check for fraction of a day included in the jd
		dt = jd - floor(jd) - 0.5;
		if (fabs(dt) > 0.00000001)
		{
			jd = jd - dt;
			jdfrac = jdfrac + dt;
		}

		/* --------------- find year and days of the year --------------- */
		temp = jd - 2415019.5;
		tu = temp / 365.25;
		year = 1900 + (int)floor(tu);
		leapyrs = (int)floor((year - 1901) * 0.25);

		days = floor(temp - ((year - 1900) * 365.0 + leapyrs));

		/* ------------ check for case of beginning of a year ----------- */
		if (days + jdfrac < 1.0)
		{
			year = year - 1;
			leapyrs = (int)floor((year - 1901) * 0.25);
			days = floor(temp - ((year - 1900) * 365.0 + leapyrs));
		}

		/* ----------------- find remaing data  ------------------------- */
		// now add the daily time in to preserve accuracy
		days2mdhms(year, days + jdfrac, mon, day, hr, minute, sec);
	}

	/* -----------------------------------------------------------------------------
	*
	*                           procedure finddays
	*
	*  this procedure finds the fractional days through a year given the year,
	*    month, day, hour, minute and second.
	*
	*  algorithm     : set up array for the number of days per month
	*                  find leap year - use 1900 because 2000 is a leap year
	*                  check for a leap year
	*                  loop to find the elapsed days in the year
	*
	*  author        : david vallado                  719-573-2600
	*
	*  inputs          description                    range / units
	*    year        - year                           1900 .. 2100
	*    mon         - month                          1 .. 12
	*    day         - day                            1 .. 28,29,30,31
	*    hr          - hour                           0 .. 23
	*    min         - minute                         0 .. 59
	*    sec         - second                         0.0 .. 59.999
	*
	*  outputs       :
	*    days        - day of year plus fraction of a
	*                    day                          days
	*
	*  locals        :
	*    lmonth      - length of months of year
	*    i           - index
	*
	*  coupling      :
	*    none.
	*
	*  references    :
	*    vallado       2013, 201, ex 3-12
	* --------------------------------------------------------------------------- */

	void    finddays
	(
		int year, int month, int day, int hr, int minute,
		double sec, double& days
	)
	{
		// shift index values to be 1-12
		int lmonth[] = { 0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
		int i;

		if (((year - 1900) % 4) == 0)
			lmonth[2] = 29;

		i = 1;
		days = 0.0;
		while ((i < month) && (i < 12))
		{
			days = days + lmonth[i];
			i = i + 1;
		}
		days = days + day + hr / 24.0 + minute / 1440.0 + sec / 86400.0;
	}


	/* -----------------------------------------------------------------------------
	*
	*                           procedure hms_sec
	*
	*  this procedure converts hours, minutes and seconds to seconds from the
	*  beginning of the day.
	*
	*  author        : david vallado                  719-573-2600   25 jun 2002
	*
	*  revisions
	*                -
	*
	*  inputs          description                    range / units
	*    utsec       - seconds                        0.0 .. 86400.0
	*
	*  outputs       :
	*    hr          - hours                          0 .. 24
	*    min         - minutes                        0 .. 59
	*    sec         - seconds                        0.0 .. 59.99
	*
	*  locals        :
	*    temp        - temporary variable
	*
	*  coupling      :
	*    none.
	* --------------------------------------------------------------------------- */

	void    hms_sec
	(
		int& hr, int& min, double& sec, edirection direct, double& utsec
	)
	{
		double temp;

		// ------------------------  implementation   ------------------
		if (direct == eTo)
			utsec = hr * 3600.0 + min * 60.0 + sec;
		else
		{
			temp = utsec / 3600.0;
			hr = (int)floor(temp);
			min = (int)floor((temp - hr)* 60.0);
			sec = (temp - hr - min / 60.0) * 3600.0;
		}
	}

	/* -----------------------------------------------------------------------------
	*
	*                           procedure hms_ut
	*
	*  this procedure converts hours, minutes and seconds into universal time.
	*
	*  author        : david vallado                  719-573-2600    1 mar 2001
	*
	*  inputs          description                    range / units
	*    hr          - hours                          0 .. 24
	*    min         - minutes                        0 .. 59
	*    sec         - seconds                        0.0 .. 59.99
	*    direction   - which set of vars to output    from  too
	*
	*  outputs       :
	*    ut          - universal time                 hrmin.sec
	*
	*  locals        :
	*    none.
	*
	*  coupling      :
	*    none.
	*
	*  references    :
	*    vallado       2013, 199, alg 21, ex 3-10
	* --------------------------------------------------------------------------- */

	void    hms_ut
	(
		int& hr, int& min, double& sec, edirection direct, double& ut
	)
	{
		// ------------------------  implementation   ------------------
		if (direct == eTo)
			ut = hr * 100.0 + min + sec * 0.01;
		else
		{
			hr = (int)floor(ut * 0.01);
			min = (int)floor(ut - hr * 100.0);
			sec = (ut - hr * 100.0 - min) * 100.0;
		}
	}

	/* -----------------------------------------------------------------------------
	*
	*                           procedure hms_rad
	*
	*  this procedure converts hours, minutes and seconds into radians.
	*
	*  author        : david vallado                  719-573-2600    1 mar 2001
	*
	*  inputs          description                    range / units
	*    hr          - hours                          0 .. 24
	*    min         - minutes                        0 .. 59
	*    sec         - seconds                        0.0 .. 59.99
	*    direction   - which set of vars to output    from  too
	*
	*  outputs       :
	*    hms         - result                         rad
	*
	*  locals        :
	*    temp        - conversion from hours to rad   0.261799
	*
	*  coupling      :
	*    none.
	*
	*  references    :
	*    vallado       2013, 198, alg 19 alg 20, ex 3-9
	* --------------------------------------------------------------------------- */

	void    hms_rad
	(
		int& hr, int& min, double& sec, edirection direct, double& hms
	)
	{
		const double rad2deg = 57.29577951308230;
		double temp;

		// ------------------------  implementation   ------------------
		temp = 15.0 / rad2deg;
		if (direct == eTo)
			hms = hr + min / 60.0 + sec / 3600.0;
		else
		{
			temp = hms / temp;
			hr = int(temp);
			min = int((temp - hr)* 60.0);
			sec = (temp - hr - min / 60.0) * 3600.0;
		}
	}

	/* -----------------------------------------------------------------------------
	*
	*                           procedure dms_rad
	*
	*  this procedure converts degrees, minutes and seconds into radians.
	*
	*  author        : david vallado                  719-573-2600    1 mar 2001
	*
	*  inputs          description                    range / units
	*    deg         - degrees                        0 .. 360
	*    min         - minutes                        0 .. 59
	*    sec         - seconds                        0.0 .. 59.99
	*    direction   - which set of vars to output    from  too
	*
	*  outputs       :
	*    dms         - result                         rad
	*
	*  locals        :
	*    temp        - temporary variable
	*
	*  coupling      :
	*    none.
	*
	*  references    :
	*    vallado       2013, 197, alg 17 alg 18, ex 3-8
	* --------------------------------------------------------------------------- */

	void    dms_rad
	(
		int& deg, int& min, double& sec, edirection direct, double& dms
	)
	{
		const double rad2deg = 57.29577951308230;
		double temp;

		// ------------------------  implementation   ------------------
		if (direct == eTo)
			dms = (deg + min / 60.0 + sec / 3600.0) / rad2deg;
		else
		{
			temp = dms * rad2deg;
			deg = (int)floor(temp);
			min = (int)floor((temp - deg)* 60.0);
			sec = (temp - deg - min / 60.0) * 3600.0;
		}
	}

	/* -----------------------------------------------------------------------------
	*
	*                           function jd2sse
	*
	*  this function finds the seconds since epoch (1 Jan 2000) given the julian date
	*
	*  author        : david vallado                  719-573-2600   12 dec 2002
	*
	*  revisions
	*                -
	*
	*  inputs          description                    range / units
	*    jd          - julian date                    days from 4713 bc
	*
	*  outputs       :
	*    sse         - seconds since epoch 1 jan 2000
	*
	*  locals        :
	*    none.
	*
	*  coupling      :
	*    none.
	*
	*  references    :
	*
	* sse = jd2sse( jd );
	* ----------------------------------------------------------------------------- */

	double jd2sse
	(
		double jd
	)
	{
		double temp;
		// ------------------------  implementation   ------------------
		temp = (jd - 2451544.5) * 86400.0;
		return temp;
	}  // function jd2sse


	/* -----------------------------------------------------------------------------
	*
	*                           procedure convtime
	*
	*  this procedure finds the time parameters and julian century values for inputs
	*    of utc or ut1. numerous outputs are found as shown in the local variables.
	*    because calucations are in utc, you must include timezone if ( you enter a
	*    local time, otherwise it should be zero.
	*
	*  author        : david vallado                  719-573-2600    4 jun 2002
	*
	*  revisions
	*    vallado     - fix documentation for dut1                     8 oct 2002
	*
	*  inputs          description                    range / units
	*    year        - year                           1900 .. 2100
	*    mon         - month                          1 .. 12
	*    day         - day                            1 .. 28,29,30,31
	*    hr          - universal time hour            0 .. 23
	*    min         - universal time min             0 .. 59
	*    sec         - universal time sec (utc)            0.0  .. 59.999
	*    timezone    - offset to utc from local site  0 .. 23 hr
	*    dut1        - delta of ut1 - utc             sec
	*    dat         - delta of utc - tai             sec
	*
	*  outputs       :
	*    ut1         - universal time                 sec
	*    tut1        - julian centuries of ut1
	*    jdut1       - julian date (days only)           days from 4713 bc
	*    jdut1Frac   - julian date (fraction of a day)   days from 0 hr of the day
	*    utc         - coordinated universal time     sec
	*    tai         - atomic time                    sec
	*    tdt         - terrestrial dynamical time     sec
	*    ttdt        - julian centuries of tdt
	*    jdtt        - julian date (days only)           days from 4713 bc
	*    jdttFrac    - julian date (fraction of a day)   days from 0 hr of the day
	*    tcg         - geocentric coordinate time     sec
	*    tdb         - terrestrial barycentric time   sec
	*    ttdb        - julian centuries of tdb
	*    jdtdb       - julian date (days only)           days from 4713 bc
	*    jdtdbFrac   - julian date (fraction of a day)   days from 0 hr of the day
	*
	*  locals        :
	*    hrtemp      - temporary hours                hr
	*    mintemp     - temporary minutes              min
	*    sectemp     - temporary seconds              sec
	*    localhr     - difference to local time       hr
	*    jd          - julian date of request         days from 4713 bc
	*    me          - mean anomaly of the earth      rad
	*
	*  coupling      :
	*    hms_2_sec   - conversion between hr-min-sec .and. seconds
	*    jday        - find the julian date
	*
	*  references    :
	*    vallado       2013, 195, alg 16, ex 3-7
	* --------------------------------------------------------------------------- */

	void    convtime
	(
		int year, int mon, int day, int hr, int min, double sec, int timezone,
		double dut1, int dat,
		double& ut1, double& tut1, double& jdut1, double& jdut1Frac, double& utc, double& tai,
		double& tt, double& ttt, double& jdtt, double& jdttFrac, double& tcg, double& tdb,
		double& ttdb, double& jdtdb, double& jdtdbFrac, double& tcb
	)
	{
		double deg2rad, jd, jdFrac, sectemp, me;
		int localhr, hrtemp, mintemp;

		deg2rad = pi / 180.0;

		// ------------------------  implementation   ------------------
		jday(year, mon, day, 0, 0, 0.0, jd, jdFrac);
		//     mjd  = jd - 2400000.5;
		//     mfme = hr*60.0 + min + sec/60.0;

		// ------------------ start if ( ut1 is known ------------------
		localhr = timezone + hr;

		hms_sec(localhr, min, sec, eTo, utc);
		ut1 = utc + dut1;
		hms_sec(hrtemp, mintemp, sectemp, eFrom, ut1);
		jday(year, mon, day, hrtemp, mintemp, sectemp, jdut1, jdut1Frac);
		tut1 = (jdut1 + jdut1Frac - 2451545.0) / 36525.0;

		tai = utc + dat;

		tt = tai + 32.184;   // sec
		hms_sec(hrtemp, mintemp, sectemp, eFrom, tt);
		jday(year, mon, day, hrtemp, mintemp, sectemp, jdtt, jdttFrac);
		ttt = (jdtt + jdttFrac - 2451545.0) / 36525.0;

		tcg = tt + 6.969290134e-10*(jdut1 - 2443144.5)*86400.0; // sec

		me = 357.5277233 + 35999.05034 *ttt;
		me = fmod(me, 360.0);
		me = me * deg2rad;
		tdb = tt + 0.001657  * sin(me) + 0.00001385 *sin(2.0 *me);
		hms_sec(hrtemp, mintemp, sectemp, eFrom, tdb);
		jday(year, mon, day, hrtemp, mintemp, sectemp, jdtdb, jdtdbFrac);
		ttdb = (jdtdb + jdtdbFrac - 2451545.0) / 36525.0;

		tcb = tdb + 1.55051976772e-8*(jdtt - 2443144.5)*86400.0; // sec

		//     fprintf( 'time %14.6f%14.6f%9.6f%18.5f\n',utc,ut1,ttt,jdut1 );
	}



}  // namespace

