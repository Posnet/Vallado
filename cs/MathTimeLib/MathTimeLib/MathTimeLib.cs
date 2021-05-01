//
//                          MathTimeMethods
//
// this library contains various math and basic time routines.
//
//    current :
//              13 mar 18  david vallado
//                           split up to be more functional
//    changes :
//              29 nov 17  david vallado
//                           fixes to teme, new routines, etc
//              19 mar 14  david vallado
//                           original baseline 
//
//    (w) 719-573-2600, email dvallado@agi.com, davallado@gmail.com
//

using System;
using System.Linq;


namespace MathTimeMethods
{

    // -----------------------------------------------------------------------------
    public class MathTimeLib
    {
        public string MathTimeLibVersion = "MathTimeLib Version 2019-10-23";

        // ------------------------------- constants -----------------------------------
        public static class globals
        {
            public static double infinite = 999999.9;
            public static double undefined = 999999.1;
            public static double mum = 3.986004415e14; // m^3/s^2 stk uses .4415
            public static double mu = 398600.4415;     // km^3/s^2 stk uses .4415
            public static double re = 6378.1363;       // km  stk uses .1363
            public static double velkmps = 7.9053657160394282;
            public static double earthrot = 7.29211514670698e-05;  // older rad/s
            public static double c = 2.99792458e8;  // speed of light m/s
          //  public static double earthrot = 7.292115e-05;  // rad/s
        }

        public enum Edirection { efrom, eto };

        // -----------------------------------------------------------------------------
        public double radians
            (
            double degrees
            )
        {
            return degrees * Math.PI / 180.0;
        } // radians


        // -----------------------------------------------------------------------------
        public double degrees
            (
            double radians
            )
        {
            return radians / Math.PI * 180.0;
        } // degrees


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

        public double acosh
            (
            double xval
            )
        {
            return Math.Log(xval + Math.Sqrt(xval * xval - 1.0));
        }  // end acosh

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

        public double asinh
            (
            double xval
            )
        {
            return Math.Log(xval + Math.Sqrt(xval * xval + 1.0));
        }  //  asinh


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

        public double cot
            (
            double xval
            )
        {
            double temp;

            temp = Math.Tan(xval);
            if (Math.Abs(temp) < 0.00000001)
                return 999999.9;
            else
                return 1.0 / temp;
        }  //  cot


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

        public double dot
            (
            double[] x, double[] y
            )
        {
            return (x[0] * y[0] + x[1] * y[1] + x[2] * y[2]);
        }  //  dot


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

        public double mag
            (
            double[] x
            )
        {
            return Math.Sqrt(x[0] * x[0] + x[1] * x[1] + x[2] * x[2]);
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

        public void cross
            (
            double[] vec1, double[] vec2, out double[] outvec
            )
        {
            double[] tempvec = new double[3];
            tempvec[0] = vec1[1] * vec2[2] - vec1[2] * vec2[1];
            tempvec[1] = vec1[2] * vec2[0] - vec1[0] * vec2[2];
            tempvec[2] = vec1[0] * vec2[1] - vec1[1] * vec2[0];
            outvec = tempvec;
        }  //  cross


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

        public double[] norm
            (
            double[] vec1
            )
        {
            double[] norm = new double[3];
            double magr;

            magr = mag(vec1);
            norm[0] = vec1[0] / magr;
            norm[1] = vec1[1] / magr;
            norm[2] = vec1[2] / magr;
            return norm;
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
        *    theta       - angle between the two vectors  -Math.PI to Math.PI
        *    magv1       - magnitude of vec1
        *    magv2       - magnitude of vec2
        *    small       - value defining a small value
        *    undefined   - large number to use in place of a not defined number 
        *
        *  locals        :
        *    temp        - temporary real variable
        *
        *  coupling      :
        *    dot           dot product of two vectors
        * --------------------------------------------------------------------------- */

        public double angle
            (
            double[] vec1,
            double[] vec2
            )
        {
            double small, undefined, magv1, magv2, temp;
            small = 0.00000001;
            undefined = 999999.1;

            magv1 = mag(vec1);
            magv2 = mag(vec2);

            if (magv1 * magv2 > small * small)
            {
                temp = dot(vec1, vec2) / (magv1 * magv2);
                if (Math.Abs(temp) > 1.0)
                    temp = Math.Sign(temp) * 1.0;
                return Math.Acos(temp);
            }
            else
                return undefined;
        }  //  angle


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
*
* --------------------------------------------------------------------------- */

        public double[] rot1
            (
            double[] vec,
            double xval
            )
        {
            double c, s, temp;
            double[] outvec = new double[3];

            temp = vec[2];
            c = Math.Cos(xval);
            s = Math.Sin(xval);

            outvec[2] = c * vec[2] - s * vec[1];
            outvec[1] = c * vec[1] + s * temp;
            outvec[0] = vec[0];

            return outvec;
        }  // rot1  


        public double[] rot2
            (
            double[] vec,
            double xval
            )
        {
            double c, s, temp;
            double[] outvec = new double[3];

            temp = vec[2];
            c = Math.Cos(xval);
            s = Math.Sin(xval);

            outvec[2] = c * vec[2] + s * vec[0];
            outvec[0] = c * vec[0] - s * temp;
            outvec[1] = vec[1];

            return outvec;
        }   // rot2  



        public double[] rot3
            (
            double[] vec,
            double xval
            )
        {
            double c, s, temp;
            double[] outvec = new double[3];

            temp = vec[1];
            c = Math.Cos(xval);
            s = Math.Sin(xval);

            outvec[1] = c * vec[1] - s * vec[0];
            outvec[0] = c * vec[0] + s * temp;
            outvec[2] = vec[2];

            return outvec;
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

        public double[,] rot1mat
            (
            double xval
            )
        {
            double c, s;
            double[,] outmat = new double[3, 3];

            c = Math.Cos(xval);
            s = Math.Sin(xval);

            outmat[0, 0] = 1.0;
            outmat[0, 1] = 0.0;
            outmat[0, 2] = 0.0;

            outmat[1, 0] = 0.0;
            outmat[1, 1] = c;
            outmat[1, 2] = s;

            outmat[2, 0] = 0.0;
            outmat[2, 1] = -s;
            outmat[2, 2] = c;
            return outmat;
        }  //  rot1mat


        public double[,] rot2mat
            (
            double xval
            )
        {
            double c, s;
            double[,] outmat = new double[3, 3];

            c = Math.Cos(xval);
            s = Math.Sin(xval);

            outmat[0, 0] = c;
            outmat[0, 1] = 0.0;
            outmat[0, 2] = -s;

            outmat[1, 0] = 0.0;
            outmat[1, 1] = 1.0;
            outmat[1, 2] = 0.0;

            outmat[2, 0] = s;
            outmat[2, 1] = 0.0;
            outmat[2, 2] = c;
            return outmat;
        }  // rot2mat

        public double[,] rot3mat
            (
            double xval
            )
        {
            double c, s;
            double[,] outmat = new double[3, 3];

            c = Math.Cos(xval);
            s = Math.Sin(xval);

            outmat[0, 0] = c;
            outmat[0, 1] = s;
            outmat[0, 2] = 0.0;

            outmat[1, 0] = -s;
            outmat[1, 1] = c;
            outmat[1, 2] = 0.0;

            outmat[2, 0] = 0.0;
            outmat[2, 1] = 0.0;
            outmat[2, 2] = 1.0;
            return outmat;
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

        public void addvec
            (
            double a1, double[] vec1,
            double a2, double[] vec2,
            out double[] vec3
            )
        {
            vec3 = new double[] { 0.0, 0.0, 0.0 };
            int row;
            double[] tempvec = new double[3];

            for (row = 0; row <= 2; row++)
            {
                vec3[row] = 0.0;
                vec3[row] = a1 * vec1[row] + a2 * vec2[row];
            }
        } // addvec


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
        *    order       - square size of the mat and the size of the vector which must be the same
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

        public double[] matvecmult
            (
            double[,] mat, double[] vec, int order
            )
        {
            int row, ktr;
            double[] vecout = new double[order];
            for (row = 0; row < order; row++)
            {
                vecout[row] = 0.0;
                for (ktr = 0; ktr < order; ktr++)
                    vecout[row] = vecout[row] + mat[row, ktr] * vec[ktr];
            }
            return vecout;
        } // matvecmult  



        // -----------------------------------------------------------------------------
        // perform vector outer product
        // makes a 3x3 matrix from multiplying 2 [3] dimensioned vectors
        public double[,] vecouter
            (
            double[] vec1, double[] vec2
            )
        {
            double[,] mat = new double[3, 3];
            int row, col;
            for (row = 0; row <= 2; row++)
            {
                for (col = 0; col <= 2; col++)
                {
                    mat[row, col] = vec1[row] * vec2[col];
                }
            }
            return mat;
        } // vecouter  


        // -----------------------------------------------------------------------------
        // matrixes must be the same size
        public double[,] matadd
            (
            double[,] mat1, double[,] mat2, int mat1r, int mat1c
            )
        {
            int row, col;
            double[,] mat3 = new double[mat1r, mat1c];
            for (row = 0; row < mat1r; row++)
            {
                for (col = 0; col < mat1c; col++)
                    mat3[row, col] = mat1[row, col] + mat2[row, col];
            }
            return mat3;
        } // matadd  


        // -----------------------------------------------------------------------------
        // matrixes must be the same size
        public double[,] matsub
            (
            double[,] mat1, double[,] mat2, int mat1r, int mat1c
            )
        {
            int row, col;
            double[,] mat3 = new double[mat1r, mat1c];
            for (row = 0; row < mat1r; row++)
            {
                for (col = 0; col < mat1c; col++)
                    mat3[row, col] = mat1[row, col] - mat2[row, col];
            }
            return mat3;
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

        public double[,] matmult
            (
            double[,] mat1, double[,] mat2, int mat1r, int mat1c, int mat2c
            )
        {
            int row, col, ktr;
            double[,] mat3 = new double[mat1r, mat2c];
            for (row = 0; row < mat1r; row++)
            {
                for (col = 0; col < mat2c; col++)
                {
                    mat3[row, col] = 0.0;
                    for (ktr = 0; ktr < mat1c; ktr++)
                        mat3[row, col] = mat3[row, col] + mat1[row, ktr] * mat2[ktr, col];
                }
            }
            return mat3;
        } // matmult  


        // -----------------------------------------------------------------------------
        // form the transponse of a square matrix
        public double[,] mattrans
            (
            double[,] mat1, int matr
            )
        {
            int row, col;
            double[,] mat2 = new double[matr, matr];
            for (row = 0; row < matr; row++)
            {
                for (col = 0; col < matr; col++)
                {
                    mat2[row, col] = mat1[col, row];
                }
            }
            return mat2;
        }  // mattrans 



        // -----------------------------------------------------------------------------
        // form the transponse of a non-square matrix
        public double[,] mattransx
            (
            double[,] mat1, int matr, int matc
            )
        {
            int row, col;
            double[,] mat2 = new double[matc, matr];
            for (row = 0; row < matr; row++)
            {
                for (col = 0; col < matc; col++)
                {
                    mat2[col, row] = mat1[row, col];
                }
            }
            return mat2;
        }  // mattransx



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

        private void ludecomp
            (
            ref double[,] lu,
            out int[] indexx,
            int order
            )
        {
            const double small = 0.000001;
            int i, j, k, imax;
            double[] scale = new double[order];
            double sum, amax, dum;

            indexx = new int[] { 0, 0, 0, 0, 0, 0 };

            imax = 0;
            for (i = 0; i < order; i++)
            {
                amax = 0.0;
                for (j = 0; j < order; j++)
                    if (Math.Abs(lu[i, j]) > amax)
                        amax = Math.Abs(lu[i, j]);
                if (Math.Abs(amax) < small)
                {
                    // " singular matrix amax "
                }
                scale[i] = 1.0 / amax;
            }
            for (j = 0; j < order; j++)
            {
                for (i = 0; i <= j - 1; i++)
                {
                    sum = lu[i, j];
                    for (k = 0; k <= i - 1; k++)
                        sum = sum - lu[i, k] * lu[k, j];
                    lu[i, j] = sum;
                }
                amax = 0.0;
                for (i = j; i < order; i++)
                {
                    sum = lu[i, j];
                    for (k = 0; k <= j - 1; k++)
                        sum = sum - lu[i, k] * lu[k, j];
                    lu[i, j] = sum;
                    dum = scale[i] * Math.Abs(sum);
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
                        dum = lu[imax, k];
                        lu[imax, k] = lu[j, k];
                        lu[j, k] = dum;
                    }
                    scale[imax] = scale[j];
                }
                indexx[j] = imax;
                if (Math.Abs(lu[j, j]) < small)
                {
                    // " matrix is singular lu "
                }
                if (j != order - 1)
                {
                    dum = 1.0 / lu[j, j];
                    for (i = j + 1; i < order; i++)
                        lu[i, j] = dum * lu[i, j];
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

        private void lubksub
        (
        double[,] lu,
        int[] indexx,
        int order,
        ref double[] b
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
                        sum = sum - lu[i, j] * b[j];
                else
                {
                    if (sum != 0.0)
                        ii = i;
                }
                b[i] = sum;
            }

            //b[order - 1] = b[order - 1] / lu[order - 1, order - 1];

            for (i = order - 1; i >= 0; i--)
            {
                sum = b[i];
                for (j = i + 1; j < order; j++)
                    sum = sum - lu[i, j] * b[j];
                b[i] = sum / lu[i, i];
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

        public void matinverse
        (
        double[,] mat,
        int order,
        out double[,] matinv
        )
        {
            int i, j;
            int[] indexx = new int[order];
            double[,] lu = new double[order, order];
            double[] b = new double[order];
            matinv = new double[,] { { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 },
               { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 } };

            for (i = 0; i < order; i++)
            {
                indexx[i] = i;
                for (j = 0; j < order; j++)
                    lu[i, j] = mat[i, j];
            }

            ludecomp(ref lu, out indexx, order);

            for (j = 0; j < order; j++)
            {
                for (i = 0; i < order; i++)
                {
                    if (i == j)
                        b[i] = 1.0;
                    else
                        b[i] = 0.0;
                }

                lubksub(lu, indexx, order, ref b);

                for (i = 0; i < order; i++)
                    matinv[i, j] = b[i];
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

        public double determinant
        (
        double[,] mat1,
        int order
        )
        {
            double small = 0.00000001;
            int i, j, k;
            double temp, d, sum;
            double[,] l = new double[order, order];
            double[,] u = new double[order, order];

            sum = 0.0;
            // ----------- Switch a non zero row to the first row---------- 
            if (Math.Abs(mat1[0, 0]) < small)
            {
                j = 0;
                while (j < order)
                {
                    if (Math.Abs(mat1[j, 0]) > small)
                    {
                        for (k = 0; k < order; k++)
                        {
                            temp = mat1[0, k];
                            mat1[0, k] = mat1[j, k];
                            mat1[j, k] = temp;
                        }
                        j = order + 1;
                    }
                    j = j + 1;
                } // if abs
            }

            for (i = 0; i < order; i++)
                l[i, 0] = mat1[i, 0];
            for (j = 0; j < order; j++)
                u[0, j] = mat1[0, j] / l[0, 0];
            for (j = 1; j < order; j++)
            {
                for (i = j; i < order; i++)
                {
                    sum = 0.0;
                    for (k = 0; k <= j - 1; k++)
                        sum = sum + l[i, k] * u[k, j];
                    l[i, j] = mat1[i, j] - sum;
                } // for i 
                u[j, j] = 1.0;
                if (j != order)
                {
                    for (i = j + 1; i < order; i++)
                    {
                        sum = 0.0;
                        for (k = 0; k <= j - 1; k++)
                            sum = sum + l[j, k] * u[k, i];
                        u[j, i] = (mat1[j, i] - sum) / l[j, j];
                    } // for i 
                } // if j 
            } //  for j 
            d = 1.0;
            for (i = 0; i < order; i++)
                d = d * l[i, i];
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
        public double[,] cholesky
            (
            double[,] a
            )
        {
            int n = (int)Math.Sqrt(a.Length);

            double[,] ret = new double[n, n];
            for (int r = 0; r < n; r++)
                for (int c = 0; c <= r; c++)
                {
                    if (c == r)
                    {
                        double sum = 0;
                        for (int j = 0; j < c; j++)
                        {
                            sum = sum + ret[c, j] * ret[c, j];
                        }
                        ret[c, c] = Math.Sqrt(a[c, c] - sum);
                    }
                    else
                    {
                        double sum = 0;
                        for (int j = 0; j < c; j++)
                            sum = sum + ret[r, j] * ret[c, j];
                        ret[r, c] = 1.0 / ret[c, c] * (a[r, c] - sum);
                    }
                }

            return ret;
        }   // cholesky


        // -----------------------------------------------------------------------------
        // form a copy of a square matrix
        public double[,] matequal
            (
            double[,] mat1, int matr
            )
        {
            int row, col;
            double[,] mat2 = new double[matr, matr];
            for (row = 0; row < matr; row++)
            {
                for (col = 0; col < matr; col++)
                {
                    mat2[row, col] = mat1[row, col];
                }
            }
            return mat2;
        }  // mattrans 


        // -----------------------------------------------------------------------------
        // multiply a matrix * a scalar
        public double[,] matscale
            (
            double[,] mat1, int matr, int matc, double scale
            )
        {
            int row, col;
            double[,] mat2 = new double[matr, matr];
            for (row = 0; row < matr; row++)
            {
                for (col = 0; col < matc; col++)
                {
                    mat2[row, col] = mat1[row, col] * scale;
                }
            }
            return mat2;
        }  // matscale 







        // percentile function
        // excelpercentile is a fraction from 0.0 to 1.0
        // arrSize     - size of array. It could be 4 or 5, etc.
        // some adjustments to be sure it works for variable sized arrays and fractional % values
        // -----------------------------------------------------------------------------
        public double Percentile
            (
            double[] sequence, double excelPercentile, Int32 arrSize
            )
        {
            // limit the percentile to 2 decimal places
            double[] b = new double[100];  
            // get just the valid members of the sequence
            Array.Copy(sequence, 0, b, 0, arrSize);
            Array.Sort(b, 0, arrSize);  // needs sort for fractional % calcs
            if (arrSize > 0)
            {
                if (arrSize == 1)
                    return b[0]; // b.Min();
                else
                {
                    int N = arrSize;  //  sequence.Length;
                    double n = (N - 1) * excelPercentile + 1;
                    if (n == N)
                        return b[N];  // b.Max();
                    else
                    {
                        int k = (int)n;
                        double d = n - k;
                        return b[k - 1] + d * (b[k] - b[k - 1]);
                    }
                }
            }
            else
                return 0.0;
        }  // percentile



        /* -----------------------------------------------------------------------------
        *
        *                           function factorial
        *
        *  this function performs a factorial. note the use of double in the return as the 
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

        public double factorial(int n)
        {
            if (n == 0)
                return 1.0;
            if (n >= 1)
                return n * factorial(n - 1);
            else
                return 0.0;
        }  // factorial 


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

        public void cubicspl
            (
            double p1, double p2, double p3, double p4,
            out double acu0, out double acu1, out double acu2, out double acu3
            )
        {
            acu0 = p2;
            acu1 = -p1 / 3.0 - 0.5 * p2 + p3 - p4 / 6.0;
            acu2 = 0.5 * p1 - p2 + 0.5 * p3;
            acu3 = -p1 / 6.0 + 0.5 * p2 - 0.5 * p3 + p4 / 6.0;
        }  // cubicspl


        /* -----------------------------------------------------------------------------
        *
        *                           function cubic
        *
        *  this function solves for the three roots of a cubic equation.  there are
        *    no restrictions on the coefficients, and imaginary results are passed
        *    out as separate values.  the general form is y = a3x3 + b2x2 + c1x + d0.  note
        *    that r1i will always be zero Math.Since there is always at least one real root.
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

        public void cubic
            (
            double a3, double b2, double c1, double d0, char opt,
            out double r1r, out double r1i, out double r2r, out double r2i, out double r3r, out double r3i
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

            if (Math.Abs(a3) > small)
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
                    temp1 = (-b2 * 0.5) + Math.Sqrt(delta);
                    temp2 = (-b2 * 0.5) - Math.Sqrt(delta);
                    temp1 = Math.Sign(temp1) * Math.Pow(Math.Abs(temp1), onethird);
                    temp2 = Math.Sign(temp2) * Math.Pow(Math.Abs(temp2), onethird);
                    r1r = temp1 + temp2 - p * onethird;

                    if (opt == 'I')
                    {
                        r2r = -0.5 * (temp1 + temp2) - p * onethird;
                        r2i = -0.5 * Math.Sqrt(3.0) * (temp1 - temp2);
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
                    if (Math.Abs(delta) < small)
                    {
                        r1r = -2.0 * Math.Sign(b2) * Math.Pow(Math.Abs(b2 * 0.5), onethird) - p * onethird;
                        r2r = Math.Sign(b2) * Math.Pow(Math.Abs(b2 * 0.5), onethird) - p * onethird;
                        if (opt == 'U')
                            r3r = 99999.9;
                        else
                            r3r = r2r;
                    }
                    else
                    {
                        // --------------- use trigonometric identities ----------------
                        e0 = 2.0 * Math.Sqrt(-a3 * onethird);
                        cosphi = (-b2 / (2.0 * Math.Sqrt(-a3 * a3 * a3 / 27.0)));
                        sinphi = Math.Sqrt(1.0 - cosphi * cosphi);
                        phi = Math.Atan2(sinphi, cosphi);
                        if (phi < 0.0)
                            phi = phi + 2.0 * Math.PI;
                        r1r = e0 * Math.Cos(phi * onethird) - p * onethird;
                        r2r = e0 * Math.Cos(phi * onethird + 120.0 / rad) - p * onethird;
                        r3r = e0 * Math.Cos(phi * onethird + 240.0 / rad) - p * onethird;
                    } // if fabs(delta) 
                }  // if delta > small
            }  // if fabs > small
            else
            {
                quadratic(b2, c1, d0, opt, out r1r, out r1i, out r2r, out r2i);
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
        *    vallado       2013, 1027
        * --------------------------------------------------------------------------- */

        public double cubicinterp
            (
            double p1a, double p1b, double p1c, double p1d, double p2a, double p2b,
            double p2c, double p2d, double valuein
            )
        {
            double kc0, kc1, kc2, kc3, ac0, ac1, ac2, ac3,
                   r1r, r1i, r2r, r2i, r3r, r3i, value;

            // -------- assign function points ---------
            cubicspl(p1a, p1b, p1c, p1d, out ac0, out ac1, out ac2, out ac3);
            cubicspl(p2a, p2b, p2c, p2d, out kc0, out kc1, out kc2, out kc3);

            // recover the original function values
            // use the normalized time first, but at an arbitrary interval
            cubic(kc3, kc2, kc1, kc0 - valuein, 'R', out r1r, out r1i, out r2r, out r2i, out r3r, out r3i);

            if ((r1r >= -0.000001) && (r1r <= 1.001))
            {
                value = r1r;
            }
            else
            {
                if ((r2r >= -0.000001) && (r2r <= 1.001))
                {
                    value = r2r;
                }
                else
                {
                    if ((r3r >= -0.000001) && (r3r <= 1.001))
                    {
                        value = r3r;
                    }
                    else
                    {
                        value = 0.0;
                        Console.Write("error in cubicinterp root {0} {1} {2} {3} \n", valuein, r1r, r2r, r3r);
                    }
                }
            }

            return (ac3 * Math.Pow(value, 3) + ac2 * value * value + ac1 * value + ac0);
        } // cubicinterp   


        /* -----------------------------------------------------------------------------
        *
        *                           function quadratic
        *
        *  this function solves for the two roots of a quadratic equation.  there are
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

        public void quadratic
            (
            double a, double b, double c, char opt,
            out double r1r, out double r1i, out double r2r, out double r2i
            )
        {
            const double small = 0.0000001;
            double discrim;
            // --------------------  implementation   ----------------------
            r1r = 0.0;
            r1i = 0.0;
            r2r = 0.0;
            r2i = 0.0;

            discrim = b * b - 4.0 * a * c;

            // ---------------------  real roots  --------------------------
            if (Math.Abs(discrim) < small)
            {
                r1r = -b / (2.0 * a);
                r2r = r1r;
                if (opt == 'U')
                    r2r = 99999.9;
            }
            else
            {
                if (Math.Abs(a) < small)
                    r1r = -c / b;
                else
                {
                    if (discrim > 0.0)
                    {
                        r1r = (-b + Math.Sqrt(discrim)) / (2.0 * a);
                        r2r = (-b - Math.Sqrt(discrim)) / (2.0 * a);
                    }
                    else
                    {
                        // ------------------ complex roots --------------------
                        if (opt == 'I')
                        {
                            r1r = -b / (2.0 * a);
                            r2r = r1r;
                            r1i = Math.Sqrt(-discrim) / (2.0 * a);
                            r2i = -Math.Sqrt(-discrim) / (2.0 * a);
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



        // -----------------------------------------------------------------------------------------
        //                                       time functions
        // -----------------------------------------------------------------------------------------

        // -----------------------------------------------------------------------------------------
        // convert a string month to integer
        public int convertMonth
            (
            string monthstr
            )
        {
            int strNumber, strIndex;
            string[] monstr = new string[13] { "", "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };

            strIndex = 0;
            for (strNumber = 0; strNumber < monstr.Length; strNumber++)
            {
                strIndex = monstr[strNumber].IndexOf(monthstr.ToUpper());
                if (strIndex >= 0)
                    break;
            }
            return strNumber;
        } // convertMonth


        /* -----------------------------------------------------------------------------
        *
        *                           procedure jday
        *
        *  this procedure finds the julian date given the year, month, day, and time.
        *    the julian date is defined by each elapsed day since noon, jan 1, 4713 bc.
        *    two values are passed back for improved accuracy
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
        *    vallado       2013, 183, alg 14, ex 3-14
        * --------------------------------------------------------------------------- */

        public void jday
            (
            int year, int mon, int day, int hr, int minute, double sec,
            out double jd, out double jdFrac
            )
        {
            jd = 367.0 * year -
                 Math.Floor((7 * (year + Math.Floor((mon + 9) / 12.0))) * 0.25) +
                 Math.Floor(275 * mon / 9.0) +
                 day + 1721013.5;  // use - 678987.0 to go to mjd directly
            jdFrac = (sec + minute * 60.0 + hr * 3600.0) / 86400.0;

            // check that the day and fractional day are correct
            if (Math.Abs(jdFrac) >= 1.0)
            {
                double dtt = Math.Floor(jdFrac);
                jd = jd + dtt;
                jdFrac = jdFrac - dtt;
            }

            // - 0.5*Math.Sign(100.0*year + mon - 190002.5) + 0.5;
        }  //  jday


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

        public void days2mdhms
            (
            int year, double days,
            out int mon, out int day, out int hr, out int minute, out double sec
            )
        {
            int i, inttemp, dayofyr;
            double temp;
            int[] lmonth = new int[13] { 0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

            dayofyr = (int)Math.Floor(days);
            /* ----------------- find month and day of month ---------------- */
            if ((year % 4) == 0)
                lmonth[2] = 29;

            i = 1;
            inttemp = 0;
            while ((dayofyr > inttemp + lmonth[i]) && (i < 12))
            {
                inttemp = inttemp + lmonth[i];
                i = i + 1;
            }
            mon = i;
            day = dayofyr - inttemp;

            /* ----------------- find hours minutes and seconds ------------- */
            temp = (days - dayofyr) * 24.0;
            hr = Convert.ToInt16(Math.Floor(temp));
            temp = (temp - hr) * 60.0;
            minute = Convert.ToInt16(Math.Floor(temp));
            sec = (temp - minute) * 60.0;
        }  //  days2mdhms

        /* -----------------------------------------------------------------------------
        *
        *                           procedure invjday
        *
        *  this procedure finds the year, month, day, hour, minute and second
        *  given the julian date. tu can be ut1, tdt, tdb, etc. jd is input
        *  with two arguments for additional accuracy
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
        *    vallado       2013, 202, alg 22, ex 3-13
        * --------------------------------------------------------------------------- */

        public void invjday
            (
            double jd, double jdFrac,
            out int year, out int mon, out int day,
            out int hr, out int minute, out double second
            )
        {
            int leapyrs;
            double dt, days, tu, temp;

            // check jdfrac for multiple days
            if (Math.Abs(jdFrac) >= 1.0)
            {
                jd = jd + Math.Floor(jdFrac);
                jdFrac = jdFrac - Math.Floor(jdFrac);
            }

            // check for fraction of a day included in the jd
            dt = jd - Math.Floor(jd) - 0.5;
            if (Math.Abs(dt) > 0.00000001)
            {
                jd = jd - dt;
                jdFrac = jdFrac + dt;
            }

            /* --------------- find year and days of the year --------------- */
            temp = jd - 2415019.5;
            tu = temp / 365.25;
            year = 1900 + Convert.ToInt16(Math.Floor(tu));
            leapyrs = Convert.ToInt16(Math.Floor((year - 1901) * 0.25));

            days = Math.Floor(temp - ((year - 1900) * 365.0 + leapyrs));

            /* ------------ check for case of beginning of a year ----------- */
            if (days + jdFrac < 1.0)
            {
                year = year - 1;
                leapyrs = Convert.ToInt16(Math.Floor((year - 1901) * 0.25));
                days = Math.Floor(temp - ((year - 1900) * 365.0 + leapyrs));
            }

            /* ----------------- find remaining data  ----------------------- */
            // now add the daily time in to preserve accuracy
            days2mdhms(year, days + jdFrac, out mon, out day, out hr, out minute, out second);
        }  // invjday



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

        public void hms_sec
            (
            ref int hr, ref int min, ref double sec, Enum direct, ref double utsec
            )
        {
            double temp;

            // ------------------------  implementation   ------------------
            if (direct.Equals(Edirection.eto))
                utsec = hr * 3600.0 + min * 60.0 + sec;
            else
            {
                temp = utsec / 3600.0;
                hr = (int)Math.Floor(temp);
                min = (int)Math.Floor((temp - hr) * 60.0);
                sec = (temp - hr - min / 60.0) * 3600.0;
            }
        }  // hms_sec


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

        public void hms_ut
            (
            ref int hr, ref int min, ref double sec, Enum direct, ref double ut
            )
        {
            // ------------------------  implementation   ------------------
            if (direct.Equals(Edirection.eto))
                ut = hr * 100.0 + min + sec * 0.01;
            else
            {
                hr = (int)Math.Floor(ut * 0.01);
                min = (int)Math.Floor(ut - hr * 100.0);
                sec = (ut - hr * 100.0 - min) * 100.0;
            }
        }  // hms_ut


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

        public void hms_rad
            (
            ref int hr, ref int min, ref double sec, Enum direct, ref double hms
            )
        {
            const double rad2deg = 57.29577951308230;
            double temp;

            // ------------------------  implementation   ------------------
            temp = 15.0 / rad2deg;
            if (direct.Equals(Edirection.eto))
                hms = hr + min / 60.0 + sec / 3600.0;
            else
            {
                temp = hms / temp;
                hr = Convert.ToInt32(temp);
                min = Convert.ToInt32((temp - hr) * 60.0);
                sec = (temp - hr - min / 60.0) * 3600.0;
            }
        }  // hms_rad


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

        public void dms_rad
            (
            ref int deg, ref int min, ref double sec, Enum direct, ref double dms
            )
        {
            const double rad2deg = 57.29577951308230;
            double temp;

            // ------------------------  implementation   ------------------
            if (direct.Equals(Edirection.eto))
                dms = (deg + min / 60.0 + sec / 3600.0) / rad2deg;
            else
            {
                temp = dms * rad2deg;
                deg = (int)Math.Floor(temp);
                min = (int)Math.Floor((temp - deg) * 60.0);
                sec = (temp - deg - min / 60.0) * 3600.0;
            }
        }  // dms_rad


        /* ------------------------------------------------------------------------------
*
*                           function convtime
*
*  this function finds the time parameters and julian century values for inputs
*    of utc or ut1.numerous outputs are found as shown in the local variables.
*    because calucations are in utc, you must include timezone if (you enter a
*    local time, otherwise it should be zero.
*
*  author        : david vallado                  719-573-2600    4 jun 2002
*
*  revisions
*    vallado     - add tcg, tcb, etc                              6 oct 2005
*    vallado     - fix documentation for dut1                     8 oct 2002
*
*  inputs description                                          range / units
*    year        - year                                         1900 .. 2100
*    mon         - month                                        1 .. 12
*    day         - day                                          1 .. 28,29,30,31
*    hr          - universal time hour                          0 .. 23
*    min         - universal time min                           0 .. 59
*    sec         - universal time sec (utc)                     0.0  .. 59.999
*    timezone    - offset to utc from local site                0 .. 23 hr
*    dut1        - delta of ut1 - utc sec
*    dat         - delta of tai - utc sec
*
*  outputs       :
*    ut1         - universal time                               sec
*    tut1        - julian centuries of ut1
*    jdut1       - julian date (days only) days from 4713 bc
*    jdut1Frac   - julian date (fraction of a day)              days from 0 hr of the day
*    utc         - coordinated universal time                   sec
*    tai         - atomic time                                  sec
*    tdt         - terrestrial dynamical time                   sec
*    ttdt        - julian centuries of tdt
*    jdtt        - julian date(days only)                       days from 4713 bc
*    jdttFrac    - julian date(fraction of a day)               days from 0 hr of the day
*    tdb         - terrestrial barycentric time                 sec
*    ttdb        - julian centuries of tdb
*    jdtdb       - julian date of tdb                           days from 4713 bc
*    tcb         - celestial barycentric time                   sec
*    tcg         - celestial geocentric time                    sec
*    jdtdb       - julian date(days only)                       days from 4713 bc
*    jdtdbFrac   - julian date(fraction of a day)               days from 0 hr of the day
*
*  locals        :
*    hrtemp      - temporary hours                              hr
*    mintemp     - temporary minutes                            min
*    sectemp     - temporary seconds                            sec
*    localhr     - difference to local time                     hr
*    jd          - julian date of request                       days from 4713 bc
*    me          - mean anomaly of the earth                    rad
*
*  coupling      :
*    hms_2_sec   - conversion between hr-min-sec.and.seconds
*    jday        - find the julian date
*
*  references    :
*    vallado       2013, 201, alg 16, ex 3-7
* ------------------------------------------------------------------------------*/

        public void convtime
           (
            int year, int mon, int day, int hr, int minute, double sec, int timezone, double dut1, int dat,
            out double ut1, out double tut1, out double jdut1, out double jdut1frac, out double utc, out double tai,
            out double tt, out double ttt, out double jdtt, out double jdttfrac,
            out double tdb, out double ttdb, out double jdtdb, out double jdtdbfrac
            )
        {
            const double rad2deg = 57.29577951308230;
            double mjd, mfme, jd, jdfrac, jdtai, jdtaifrac, sectemp, me, tt2, tdb2, ttdb2, jdtdb2, jdtdb2frac, dlje;
                       //tcbmtdb, tcb, ttcb, jdtcb, jdtcbfrac;
            int localhr, hrtemp, mintemp;

            // initialize
            utc = 0.0;
            hrtemp = 0;
            mintemp = 0;
            sectemp = 0.0;

            // ------------------------  implementation   ------------------
            jday(year, mon, day, hr + timezone, minute, sec, out jd, out jdfrac);
            mjd = jd + jdfrac - 2400000.5;
            mfme = hr * 60.0 + minute + sec / 60.0;

            // ------------------ start if (ut1 is known ------------------
            localhr = timezone + hr;
            hms_sec(ref localhr, ref minute, ref sec, Edirection.eto, ref utc);

            ut1 = utc + dut1;
            hms_sec(ref localhr, ref mintemp, ref sectemp, Edirection.efrom, ref ut1);
            jday(year, mon, day, localhr, mintemp, sectemp, out jdut1, out jdut1frac);
            tut1 = (jdut1 + jdut1frac - 2451545.0) / 36525.0;

            tai = utc + dat;
            hms_sec(ref localhr, ref mintemp, ref sectemp, Edirection.efrom, ref tai);
            jday(year, mon, day, localhr, mintemp, sectemp, out jdtai, out jdtaifrac);

            tt = tai + 32.184;   // sec
            hms_sec(ref localhr, ref mintemp, ref sectemp, Edirection.efrom, ref tt);
            jday(year, mon, day, localhr, mintemp, sectemp, out jdtt, out jdttfrac);
            ttt = (jdtt + jdttfrac - 2451545.0) / 36525.0;

            // tdb
            // std approach(digits)
            //         me= 357.53  + 0.9856003 * (jdtt - 2451545.0);   
            //         me= mod(me,360.0  );
            //         me= me* deg2rad;
            //         tdb1= tt + 0.001658  * sin(me) + 0.000014 *sin(2.0 *me);
            //         [hrtemp, mintemp, sectemp] = sec2hms(tdb1 );
            //         [jdtdb1, jdtdb1frac] = jday(year, mon, day, hrtemp, mintemp, sectemp );
            //         ttdb1= (jdtdb1 + jdtdb1frac - 2451545.0  )/ 36525.0;
            //         fprintf(1,'std  tdb %8.6f ttdb  %16.12f jdtdb  %18.11f %18.11f \n', tdb1, ttdb1, jdtdb1, jdtdb1frac );
            // ast alm approach(2012) bradley email
            me = 357.53 + 0.98560028 * (jdtt - 2451545.0);
            me = me % 360.0;
            me = me / rad2deg;
            dlje = 246.11 + 0.90251792 * (jdtt - 2451545.0);
            tdb2 = tt + 0.001657 * Math.Sin(me) + 0.000022 * Math.Sin(dlje);
            hms_sec(ref localhr, ref mintemp, ref sectemp, Edirection.efrom, ref tdb2);
            jday(year, mon, day, localhr, mintemp, sectemp, out jdtdb2, out jdtdb2frac);
            ttdb2 = (jdtdb2 + jdtdb2frac - 2451545.0) / 36525.0;
            //fprintf(1,'asta tdb %8.6f ttdb  %16.12f jdtdb  %18.11f %18.11f \n', tdb2, ttdb2, jdtdb2, jdtdb2frac );
            // usno circular approach
            tdb = tt + 0.001657 * Math.Sin(628.3076 * ttt + 6.2401)
                   + 0.000022 * Math.Sin(575.3385 * ttt + 4.2970)
                   + 0.000014 * Math.Sin(1256.6152 * ttt + 6.1969)
                   + 0.000005 * Math.Sin(606.9777 * ttt + 4.0212)
                   + 0.000005 * Math.Sin(52.9691 * ttt + 0.4444)
                   + 0.000002 * Math.Sin(21.3299 * ttt + 5.5431)
                   + 0.000010 * ttt * Math.Sin(628.3076 * ttt + 4.2490);  // USNO circ(14)
            hms_sec(ref localhr, ref mintemp, ref sectemp, Edirection.efrom, ref tdb);
            jday(year, mon, day, localhr, mintemp, sectemp, out jdtdb, out jdtdbfrac);
            ttdb = (jdtdb + jdtdbfrac - 2451545.0) / 36525.0;

            //fprintf(1,'usno tdb %8.6f ttdb  %16.12f jdtdb  %18.11f %18.11f \n', tdb, ttdb, jdtdb, jdtdbfrac );
            //hms_sec(ref localhr, ref minute, ref sec, Edirection.efrom, ref tdb);
            //        fprintf(1,'hms %3i %3i %8.6f \n', h, m, s);


            // tcg
            // approx with tai
            //tcg = tt + 6.969290134e-10 * (jdtai - 2443144.5003725) * 86400.0;  // AAS 05-352 (10) and IERS TN(104)
            //hms_sec(ref localhr, ref minute, ref sec, Edirection.efrom, ref tcg);
            //jday(year, mon, day, hrtemp, mintemp, sectemp, out jdtcg, out jdtcgfrac);
            //tt2 = tcg - 6.969290134e-10 * (jdtcg + jdtcgfrac - 2443144.5003725) * 86400.0;

            //fprintf(1,'tcg %8.6f jdtcg  %18.11f ', tcg, jdtcg );
            //hms_sec(ref localhr, ref minute, ref sec, Edirection.efrom, ref tcg);
            //        fprintf(1,'hms %3i %3i %8.6f \n', h, m, s);        

            // binomial approach with days
            //        lg=6.969290134e-10*86400.0;
            //        tcg1 = tt + (jdtt - 2443144.5003725)*(lg + lg* lg + lg* lg*lg);
            // days from 77
            //        jdttx = jday(year, mon, day, 0, 0, 0.0); 
            //        ttx = tt/86400.0 + jdttx-2443144.5003725  % days from the 1977 epoch
            //        tcg2 = (jdttx - 6.969290134e-10*2443144.5003725) / (1.0 - 6.969290134e-10) % days
            //        tcg2 = (tcg2 - jdttx)*86400*86400;
            // sec from 77
            //        ttx = tt + (jdttx-2443144.5003725)*86400.0;  % s from the 1977 epoch
            //        tcg3 = ttx / (1.0 - 6.969290134e-10); % s
            //        tcg3 = tcg3 -(jdttx-2443144.5003725)*86400.0;
            // check with tcg
            //        tcg4 = tt + 6.969290134e-10*(jdtcg - 2443144.5003725)*86400.0;  % AAS 05-352 (10) and IERS TN(104)
            //        [hrtemp, mintemp, sectemp] = sec2hms(tcg4 );
            //        jdtcg4 = jday(year, mon, day, hrtemp, mintemp, sectemp );
            //        tt2 = tcg4-6.969290134e-10*(jdtcg4-2443144.5003725)*86400.0;
            //        difchk = tt2-tt


            //tcbmtdb = -1.55051976772e-8 * (jdtai + jdtaifrac - 2443144.5003725) * 86400.0 - 6.55e-5;  // sec, value for de405 AAS 05-352 (10) and IERS TN(104)?
            //tcb = tdb + tcbmtdb;
            //hms_sec(ref localhr, ref minute, ref sec, Edirection.efrom, ref tcb);
            //jday(year, mon, day, hrtemp, mintemp, sectemp, out jdtcb, out jdtcbfrac);
            //ttcb = (jdtcb + jdtcbfrac - 2451545.0) / 36525.0;
            //fprintf(1,'     tcb %8.6f ttcb  %16.12f jdtcb  %18.11f %18.11f \n', tcb, ttcb, jdtcb, jdtcbfrac );
        }  // convtime




    }  //  class MathTimeLib

}  //  MathTimeMethods
