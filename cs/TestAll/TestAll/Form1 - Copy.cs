using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;


using MathTimeMethods;     // Edirection, globals
using EOPSPWMethods;       // EOPDataClass, SPWDataClass, iau80Class, iau00Class
using AstroLibMethods;     // EOpt, gravityConst, astroConst, xysdataClass, jpldedataClass
using AstroLambertkMethods;
using System.Data;

namespace TestAll
{
    public partial class Form1 : Form
    {
        public MathTimeLib MathTimeLibr = new MathTimeLib();

        public EOPSPWLib EOPSPWLibr = new EOPSPWLib();

        public AstroLib AstroLibr = new AstroLib();

        public AstroLambertkLib AstroLambertkLibr = new AstroLambertkLib();

        StringBuilder strbuild = new StringBuilder();

        // public enum MathTimeLib.Edirection { efrom, eto };

        //   public iau80Data iau80rec;
        // 000. gives leading 0's
        // ;+00.;-00. gives signs in front
        string fmt = "0.00000000000";
        string fmtE = "0.0000000000E0";
        string fmt1 = "0.000000000000";
        string fmt2 = "0.00000";

        public Form1()
        {
            InitializeComponent();
        }


        public void testvecouter()
        {
            double[] vec1 = new double[3];
            double[] vec2 = new double[3];
            double[,] mat1 = new double[3, 3];
            vec1 = new double[3] { 2.3, 4.7, -1.6 };
            vec2 = new double[3] { 0.3, -0.7, 6.0 };

            mat1 = MathTimeLibr.vecouter(vec1, vec2);

            strbuild.AppendLine("vecout = " + mat1[0, 0].ToString(fmt).PadLeft(4) + " " + mat1[0, 1].ToString(fmt).PadLeft(4) + " " + mat1[0, 2].ToString(fmt).PadLeft(4) + " ");
            strbuild.AppendLine("vecout = " + mat1[1, 0].ToString(fmt).PadLeft(4) + " " + mat1[1, 1].ToString(fmt).PadLeft(4) + " " + mat1[1, 2].ToString(fmt).PadLeft(4));
            strbuild.AppendLine("vecout = " + mat1[2, 0].ToString(fmt).PadLeft(4) + " " + mat1[2, 1].ToString(fmt).PadLeft(4) + " " + mat1[2, 2].ToString(fmt).PadLeft(4));
        }
        public void testmatadd()
        {
            double[,] mat1 = new double[3, 3];
            double[,] mat2 = new double[3, 3];
            double[,] mat3 = new double[3, 3];
            Int32 mat1r, mat1c;
            mat1 = new double[3, 3] {{ 1.0,   2.0,   3.0 },
                                     { -1.1,   0.5,   2.0 },
                                     { -2.00,  4.00,  7.0 }};
            mat2 = new double[3, 3]{ { 1.0,  1.4, 1.8 },
                                     { 0.0,  2.6, -0.6 },
                                     { 1.9,  0.1, 7.1 } };

            mat1r = 3;
            mat1c = 3;

            mat3 = MathTimeLibr.matadd(mat1, mat2, mat1r, mat1c);

            strbuild.AppendLine("matadd = " + mat3[0, 0].ToString(fmt).PadLeft(4) + " " + mat3[0, 1].ToString(fmt).PadLeft(4) + " " + mat3[0, 2].ToString(fmt).PadLeft(4) + " ");
            strbuild.AppendLine("matadd = " + mat3[1, 0].ToString(fmt).PadLeft(4) + " " + mat3[1, 1].ToString(fmt).PadLeft(4) + " " + mat3[1, 2].ToString(fmt).PadLeft(4) + " ");
            strbuild.AppendLine("matadd = " + mat3[2, 0].ToString(fmt).PadLeft(4) + " " + mat3[2, 1].ToString(fmt).PadLeft(4) + " " + mat3[2, 2].ToString(fmt).PadLeft(4) + " ");
        }
        public void testmatsub()
        {
            double[,] mat1 = new double[3, 3];
            double[,] mat2 = new double[3, 3];
            double[,] mat3 = new double[3, 3];
            Int32 mat1r, mat1c;
            mat1 = new double[3, 3] {{ 1.0,   2.0,   3.0 },
                                     { -1.1,   0.5,   2.0 },
                                     { -2.00,  4.00,  7.0 }};
            mat2 = new double[3, 3]{ { 1.0,  1.4, 1.8 },
                                     { 0.0,  2.6, -0.6 },
                                     { 1.9,  0.1, 7.1 } };

            mat1r = 3;
            mat1c = 3;

            mat3 = MathTimeLibr.matsub(mat1, mat2, mat1r, mat1c);

            strbuild.AppendLine("matsub = " + mat3[0, 0].ToString(fmt).PadLeft(4) + " " + mat3[0, 1].ToString(fmt).PadLeft(4) + " " + mat3[0, 2].ToString(fmt).PadLeft(4) + " ");
            strbuild.AppendLine("matsub = " + mat3[1, 0].ToString(fmt).PadLeft(4) + " " + mat3[1, 1].ToString(fmt).PadLeft(4) + " " + mat3[1, 2].ToString(fmt).PadLeft(4) + " ");
            strbuild.AppendLine("matsub = " + mat3[2, 0].ToString(fmt).PadLeft(4) + " " + mat3[2, 1].ToString(fmt).PadLeft(4) + " " + mat3[2, 2].ToString(fmt).PadLeft(4) + " ");
        }
        public void testmatmult()
        {
            double[,] mat1 = new double[3, 3];
            double[,] mat2 = new double[3, 3];
            double[,] mat3 = new double[3, 3];
            Int32 mat1r, mat1c, mat2c;
            mat1r = 3;
            mat1c = 3;
            mat2c = 2;

            mat1 = new double[3, 3] {{ 1.0,   2.0,   3.0 },
                                     { -1.1,   0.5,   2.0 },
                                     { -2.00,  4.00,  7.0 }};
            mat2 = new double[3, 2]{         { 1.0,  1.4 },
                                             { 0.0,  2.6 },
                                             { 1.9,  0.1 } };
            mat3 = MathTimeLibr.matmult(mat1, mat2, mat1r, mat1c, mat2c);

            strbuild.AppendLine("matmult = " + mat3[0, 0].ToString(fmt).PadLeft(4) + " " + mat3[0, 1].ToString(fmt).PadLeft(4));
            strbuild.AppendLine("matmult = " + mat3[1, 0].ToString(fmt).PadLeft(4) + " " + mat3[1, 1].ToString(fmt).PadLeft(4));
            strbuild.AppendLine("matmult = " + mat3[2, 0].ToString(fmt).PadLeft(4) + " " + mat3[2, 1].ToString(fmt).PadLeft(4));
        }

        public void testmattrans()
        {
            double[,] mat1 = new double[3, 3];
            double[,] mat3 = new double[3, 3];
            int matr;
            matr = 3;
            mat1 = new double[3, 3] {{ 1.0,   2.0,   3.0 },
                                     { -1.1,   0.5,   2.0 },
                                     { -2.00,  4.00,  7.0 }};
            mat3 = MathTimeLibr.mattrans(mat1, matr);

            strbuild.AppendLine("mattrans = " + mat3[0, 0].ToString(fmt).PadLeft(4) + " " + mat3[0, 1].ToString(fmt).PadLeft(4) + " " + mat3[0, 2].ToString(fmt).PadLeft(4) + " ");
            strbuild.AppendLine("mattrans = " + mat3[1, 0].ToString(fmt).PadLeft(4) + " " + mat3[1, 1].ToString(fmt).PadLeft(4) + " " + mat3[1, 2].ToString(fmt).PadLeft(4) + " ");
            strbuild.AppendLine("mattrans = " + mat3[2, 0].ToString(fmt).PadLeft(4) + " " + mat3[2, 1].ToString(fmt).PadLeft(4) + " " + mat3[2, 2].ToString(fmt).PadLeft(4) + " ");
        }

        public void testmattransx()
        {
            double[,] mat1 = new double[3, 3];
            double[,] mat3 = new double[3, 3];
            int matr, matc;
            matr = 3;
            matc = 3;
            mat1 = new double[3, 3] {{ 1.0,   2.0,   3.0 },
                                     { -1.1,   0.5,   2.0 },
                                     { -2.00,  4.00,  7.0 }};

            mat3 = MathTimeLibr.mattransx(mat1, matr, matc);
        }

        public void testmatinverse()
        {
            double[,] mat1 = new double[3, 3];
            double[,] matinv = new double[3, 3];

            // enter by COL!!!!!!!!!!!!!!
            mat1 = new double[,] { { 3, 5, 6 }, { 2, 0, 3 }, { 1, 2, 8 } };
            MathTimeLibr.matinverse(mat1, 3, out matinv);

            strbuild.AppendLine("matinv = " + matinv[0, 0].ToString(fmt).PadLeft(4) + " " + matinv[0, 1].ToString(fmt).PadLeft(4) + " " + matinv[0, 2].ToString(fmt).PadLeft(4) + " ");
            strbuild.AppendLine("matinv = " + matinv[1, 0].ToString(fmt).PadLeft(4) + " " + matinv[1, 1].ToString(fmt).PadLeft(4) + " " + matinv[1, 2].ToString(fmt).PadLeft(4) + " ");
            strbuild.AppendLine("matinv = " + matinv[2, 0].ToString(fmt).PadLeft(4) + " " + matinv[2, 1].ToString(fmt).PadLeft(4) + " " + matinv[2, 2].ToString(fmt).PadLeft(4) + " ");

            //Results: test before
            // 0.1016949    0.4745763 - 0.2542373
            // 0.2203390 - 0.3050847 - 0.0508475
            //- 0.0677966    0.0169492    0.1694915

            mat1 = new double[,] { { 1, 3, 3 }, { 1, 4, 3 }, { 1, 3, 4 } };
            MathTimeLibr.matinverse(mat1, 3, out matinv);

            strbuild.AppendLine("matinv = " + matinv[0, 0].ToString(fmt).PadLeft(4) + " " + matinv[0, 1].ToString(fmt).PadLeft(4) + " " + matinv[0, 2].ToString(fmt).PadLeft(4) + " ");
            strbuild.AppendLine("matinv = " + matinv[1, 0].ToString(fmt).PadLeft(4) + " " + matinv[1, 1].ToString(fmt).PadLeft(4) + " " + matinv[1, 2].ToString(fmt).PadLeft(4) + " ");
            strbuild.AppendLine("matinv = " + matinv[2, 0].ToString(fmt).PadLeft(4) + " " + matinv[2, 1].ToString(fmt).PadLeft(4) + " " + matinv[2, 2].ToString(fmt).PadLeft(4) + " ");


            double[,] ata = new double[,] {{264603537.493561, 206266447.729262, 274546062925.826, -282848493891885, 362835957483807, -4.3758299682612E+17 },
            { 206266447.729262, 160790924.64848, 214016946538.904, -220488942186083, 282841585443473, -3.41109159752805E+17 },
            { 274546062925.826, 214016946538.904, 284862180536440, -2.93476576836794E+17, 3.76469583735348E+17, -4.54025256502168E+20},
            { -282848493891885, -220488942186083, -2.93476576836794E+17, 3.02351477439543E+20, -3.87854240635812E+20, 4.67755241586584E+23},
            { 362835957483807, 282841585443473, 3.76469583735348E+17, -3.87854240635812E+20, 4.97536553328938E+20, -6.00032966815125E+23},
            { -4.3758299682612E+17, -3.41109159752805E+17, -4.54025256502168E+20, 4.67755241586584E+23, -6.00032966815125E+23, 7.23644441510866E+26} };

            MathTimeLibr.matinverse(ata, 6, out matinv);

            strbuild.AppendLine("matinv = " + matinv[0, 0].ToString(fmt1).PadLeft(4) + " " + matinv[0, 1].ToString(fmt1).PadLeft(4) + " " + matinv[0, 2].ToString(fmt1).PadLeft(4) + " " + matinv[0, 3].ToString(fmt1).PadLeft(4) + " " + matinv[0, 4].ToString(fmt1).PadLeft(4) + " " + matinv[0, 5].ToString(fmt1).PadLeft(4) + " ");
            strbuild.AppendLine("matinv = " + matinv[1, 0].ToString(fmt1).PadLeft(4) + " " + matinv[1, 1].ToString(fmt1).PadLeft(4) + " " + matinv[1, 2].ToString(fmt1).PadLeft(4) + " " + matinv[1, 3].ToString(fmt1).PadLeft(4) + " " + matinv[1, 4].ToString(fmt1).PadLeft(4) + " " + matinv[1, 5].ToString(fmt1).PadLeft(4) + " ");
            strbuild.AppendLine("matinv = " + matinv[2, 0].ToString(fmt1).PadLeft(4) + " " + matinv[2, 1].ToString(fmt1).PadLeft(4) + " " + matinv[2, 2].ToString(fmt1).PadLeft(4) + " " + matinv[2, 3].ToString(fmt1).PadLeft(4) + " " + matinv[2, 4].ToString(fmt1).PadLeft(4) + " " + matinv[2, 5].ToString(fmt1).PadLeft(4) + " ");
            strbuild.AppendLine("matinv = " + matinv[3, 0].ToString(fmt1).PadLeft(4) + " " + matinv[3, 1].ToString(fmt1).PadLeft(4) + " " + matinv[3, 2].ToString(fmt1).PadLeft(4) + " " + matinv[3, 3].ToString(fmt1).PadLeft(4) + " " + matinv[3, 4].ToString(fmt1).PadLeft(4) + " " + matinv[3, 5].ToString(fmt1).PadLeft(4) + " ");
            strbuild.AppendLine("matinv = " + matinv[4, 0].ToString(fmt1).PadLeft(4) + " " + matinv[4, 1].ToString(fmt1).PadLeft(4) + " " + matinv[4, 2].ToString(fmt1).PadLeft(4) + " " + matinv[4, 3].ToString(fmt1).PadLeft(4) + " " + matinv[4, 4].ToString(fmt1).PadLeft(4) + " " + matinv[4, 5].ToString(fmt1).PadLeft(4) + " ");
            strbuild.AppendLine("matinv = " + matinv[5, 0].ToString(fmt1).PadLeft(4) + " " + matinv[5, 1].ToString(fmt1).PadLeft(4) + " " + matinv[5, 2].ToString(fmt1).PadLeft(4) + " " + matinv[5, 3].ToString(fmt1).PadLeft(4) + " " + matinv[5, 4].ToString(fmt1).PadLeft(4) + " " + matinv[5, 5].ToString(fmt1).PadLeft(4) + " ");

            double[,] mat3 = MathTimeLibr.matmult(ata, matinv, 6, 6, 6);

            strbuild.AppendLine("mat3 = " + mat3[0, 0].ToString(fmt1).PadLeft(4) + " " + mat3[0, 1].ToString(fmt1).PadLeft(4) + " " + mat3[0, 2].ToString(fmt1).PadLeft(4) + " " + mat3[0, 3].ToString(fmt1).PadLeft(4) + " " + mat3[0, 4].ToString(fmt1).PadLeft(4) + " " + mat3[0, 5].ToString(fmt1).PadLeft(4) + " ");
            strbuild.AppendLine("mat3 = " + mat3[1, 0].ToString(fmt1).PadLeft(4) + " " + mat3[1, 1].ToString(fmt1).PadLeft(4) + " " + mat3[1, 2].ToString(fmt1).PadLeft(4) + " " + mat3[1, 3].ToString(fmt1).PadLeft(4) + " " + mat3[1, 4].ToString(fmt1).PadLeft(4) + " " + mat3[1, 5].ToString(fmt1).PadLeft(4) + " ");
            strbuild.AppendLine("mat3 = " + mat3[2, 0].ToString(fmt1).PadLeft(4) + " " + mat3[2, 1].ToString(fmt1).PadLeft(4) + " " + mat3[2, 2].ToString(fmt1).PadLeft(4) + " " + mat3[2, 3].ToString(fmt1).PadLeft(4) + " " + mat3[2, 4].ToString(fmt1).PadLeft(4) + " " + mat3[2, 5].ToString(fmt1).PadLeft(4) + " ");
            strbuild.AppendLine("mat3 = " + mat3[3, 0].ToString(fmt1).PadLeft(4) + " " + mat3[3, 1].ToString(fmt1).PadLeft(4) + " " + mat3[3, 2].ToString(fmt1).PadLeft(4) + " " + mat3[3, 3].ToString(fmt1).PadLeft(4) + " " + mat3[3, 4].ToString(fmt1).PadLeft(4) + " " + mat3[3, 5].ToString(fmt1).PadLeft(4) + " ");
            strbuild.AppendLine("mat3 = " + mat3[4, 0].ToString(fmt1).PadLeft(4) + " " + mat3[4, 1].ToString(fmt1).PadLeft(4) + " " + mat3[4, 2].ToString(fmt1).PadLeft(4) + " " + mat3[4, 3].ToString(fmt1).PadLeft(4) + " " + mat3[4, 4].ToString(fmt1).PadLeft(4) + " " + mat3[4, 5].ToString(fmt1).PadLeft(4) + " ");
            strbuild.AppendLine("mat3 = " + mat3[5, 0].ToString(fmt1).PadLeft(4) + " " + mat3[5, 1].ToString(fmt1).PadLeft(4) + " " + mat3[5, 2].ToString(fmt1).PadLeft(4) + " " + mat3[5, 3].ToString(fmt1).PadLeft(4) + " " + mat3[5, 4].ToString(fmt1).PadLeft(4) + " " + mat3[5, 5].ToString(fmt1).PadLeft(4) + " ");
        }

        public void testdeterminant()
        {
            double[,] mat1 = new double[3, 3];
            double det;
            int order;

            order = 3;

            mat1[0, 0] = 6.0;
            mat1[0, 1] = 1.0;
            mat1[0, 2] = 1.0;
            mat1[1, 0] = 4.0;
            mat1[1, 1] = -2.0;
            mat1[1, 2] = 5.0;
            mat1[2, 0] = 2.0;
            mat1[2, 1] = 8.0;
            mat1[2, 2] = 7.0;

            det = MathTimeLibr.determinant(mat1, order);

            strbuild.AppendLine("det = " + det.ToString(fmt).PadLeft(4) + " ans -306");
        }

        public void testcholesky()
        {
            double[,] mat1 = new double[3, 3];
            double[,] a = new double[3, 3];

            mat1 = MathTimeLibr.cholesky(a);

            strbuild.AppendLine("matcho = " + mat1[0, 0].ToString(fmt1).PadLeft(4) + " " + mat1[0, 1].ToString(fmt1).PadLeft(4) + " " + mat1[0, 2].ToString(fmt1).PadLeft(4));
            strbuild.AppendLine("matcho = " + mat1[1, 0].ToString(fmt1).PadLeft(4) + " " + mat1[1, 1].ToString(fmt1).PadLeft(4) + " " + mat1[1, 2].ToString(fmt1).PadLeft(4));
            strbuild.AppendLine("matcho = " + mat1[2, 0].ToString(fmt1).PadLeft(4) + " " + mat1[2, 1].ToString(fmt1).PadLeft(4) + " " + mat1[2, 2].ToString(fmt1).PadLeft(4));
        }

        public void testposvelcov2pts()
        {
            double[] reci = new double[3];
            double[] veci = new double[3];
            double[,] cov = new double[6, 6];
            double[,] sigmapts = new double[6, 12];

            AstroLibr.posvelcov2pts(reci, veci, cov, out sigmapts);

            strbuild.AppendLine("sigmapts = " + sigmapts[0, 0].ToString(fmt1).PadLeft(4) + " " + sigmapts[0, 1].ToString(fmt1).PadLeft(4) + " " + sigmapts[0, 2].ToString(fmt1).PadLeft(4) + " " + sigmapts[0, 3].ToString(fmt1).PadLeft(4) + " " + sigmapts[0, 4].ToString(fmt1).PadLeft(4) + " " + sigmapts[0, 5].ToString(fmt1).PadLeft(4) + " ");
            strbuild.AppendLine("sigmapts = " + sigmapts[1, 0].ToString(fmt1).PadLeft(4) + " " + sigmapts[1, 1].ToString(fmt1).PadLeft(4) + " " + sigmapts[1, 2].ToString(fmt1).PadLeft(4) + " " + sigmapts[1, 3].ToString(fmt1).PadLeft(4) + " " + sigmapts[1, 4].ToString(fmt1).PadLeft(4) + " " + sigmapts[1, 5].ToString(fmt1).PadLeft(4) + " ");
            strbuild.AppendLine("sigmapts = " + sigmapts[2, 0].ToString(fmt1).PadLeft(4) + " " + sigmapts[2, 1].ToString(fmt1).PadLeft(4) + " " + sigmapts[2, 2].ToString(fmt1).PadLeft(4) + " " + sigmapts[2, 3].ToString(fmt1).PadLeft(4) + " " + sigmapts[2, 4].ToString(fmt1).PadLeft(4) + " " + sigmapts[2, 5].ToString(fmt1).PadLeft(4) + " ");
        }

        public void testposcov2pts()
        {
            double[,] cov2 = new double[6, 6];
            double[,] cov = new double[6, 6];
            double[,] sigmapts = new double[6, 12];
            double[] yu = new double[6];
            double[,] covout = new double[6, 6];
            double[] r1 = new double[3];
            double[] v1 = new double[3];
            double[] reci = new double[3];
            double[,] statearr = new double[6, 6];

            r1[0] = statearr[0, 0];
            r1[1] = statearr[0, 1];
            r1[2] = statearr[0, 2];

            v1[0] = statearr[0, 3];
            v1[1] = statearr[0, 4];
            v1[2] = statearr[0, 5];

            cov2[0, 0] = 12559.93762571587;
            cov2[0, 1] = cov2[1, 0] = 12101.56371305036;
            cov2[0, 2] = cov2[2, 0] = -440.3145384949657;
            cov2[0, 3] = cov2[3, 0] = -0.8507401236198346;
            cov2[0, 4] = cov2[4, 0] = 0.9383675791981778;
            cov2[0, 5] = cov2[5, 0] = -0.0318596430999798;
            cov2[1, 1] = 12017.77368889201;
            cov2[1, 2] = cov2[2, 1] = 270.3798093532698;
            cov2[1, 3] = cov2[3, 1] = -0.8239662300032132;
            cov2[1, 4] = cov2[4, 1] = 0.9321640899868708;
            cov2[1, 5] = cov2[5, 1] = -0.001327326827629336;
            cov2[2, 2] = 4818.009967057008;
            cov2[2, 3] = cov2[3, 2] = 0.02033418761460195;
            cov2[2, 4] = cov2[4, 2] = 0.03077663516695039;
            cov2[2, 5] = cov2[5, 2] = 0.1977541628188323;
            cov2[3, 3] = 5.774758755889862e-005;
            cov2[3, 4] = cov2[4, 3] = -6.396031584925255e-005;
            cov2[3, 5] = cov2[5, 3] = 1.079960679599204e-006;
            cov2[4, 4] = 7.24599391355188e-005;
            cov2[4, 5] = cov2[5, 4] = 1.03146660433274e-006;
            cov2[5, 5] = 1.870413627417302e-005;

            // form sigmapts pos/vel
            AstroLibr.posvelcov2pts(r1, v1, cov2, out sigmapts);
            strbuild.AppendLine("sigmapts = " + sigmapts[0, 0].ToString(fmt1).PadLeft(4) + " " + sigmapts[0, 1].ToString(fmt1).PadLeft(4) + " " + sigmapts[0, 2].ToString(fmt1).PadLeft(4) + " " + sigmapts[0, 3].ToString(fmt1).PadLeft(4) + " " + sigmapts[0, 4].ToString(fmt1).PadLeft(4) + " " + sigmapts[0, 5].ToString(fmt1).PadLeft(4) + " ");
            strbuild.AppendLine("sigmapts = " + sigmapts[1, 0].ToString(fmt1).PadLeft(4) + " " + sigmapts[1, 1].ToString(fmt1).PadLeft(4) + " " + sigmapts[1, 2].ToString(fmt1).PadLeft(4) + " " + sigmapts[1, 3].ToString(fmt1).PadLeft(4) + " " + sigmapts[1, 4].ToString(fmt1).PadLeft(4) + " " + sigmapts[1, 5].ToString(fmt1).PadLeft(4) + " ");
            strbuild.AppendLine("sigmapts = " + sigmapts[2, 0].ToString(fmt1).PadLeft(4) + " " + sigmapts[2, 1].ToString(fmt1).PadLeft(4) + " " + sigmapts[2, 2].ToString(fmt1).PadLeft(4) + " " + sigmapts[2, 3].ToString(fmt1).PadLeft(4) + " " + sigmapts[2, 4].ToString(fmt1).PadLeft(4) + " " + sigmapts[2, 5].ToString(fmt1).PadLeft(4) + " ");

            // reassemble covariance at each step and write out
            AstroLibr.remakecovpv(sigmapts, out yu, out covout);
            AstroLibr.poscov2pts(reci, cov, out sigmapts);

            strbuild.AppendLine("sigmapts = " + sigmapts[0, 0].ToString(fmt1).PadLeft(4) + " " + sigmapts[0, 1].ToString(fmt1).PadLeft(4) + " " + sigmapts[0, 2].ToString(fmt1).PadLeft(4) + " " + sigmapts[0, 3].ToString(fmt1).PadLeft(4) + " " + sigmapts[0, 4].ToString(fmt1).PadLeft(4) + " " + sigmapts[0, 5].ToString(fmt1).PadLeft(4) + " ");
            strbuild.AppendLine("sigmapts = " + sigmapts[1, 0].ToString(fmt1).PadLeft(4) + " " + sigmapts[1, 1].ToString(fmt1).PadLeft(4) + " " + sigmapts[1, 2].ToString(fmt1).PadLeft(4) + " " + sigmapts[1, 3].ToString(fmt1).PadLeft(4) + " " + sigmapts[1, 4].ToString(fmt1).PadLeft(4) + " " + sigmapts[1, 5].ToString(fmt1).PadLeft(4) + " ");
            strbuild.AppendLine("sigmapts = " + sigmapts[2, 0].ToString(fmt1).PadLeft(4) + " " + sigmapts[2, 1].ToString(fmt1).PadLeft(4) + " " + sigmapts[2, 2].ToString(fmt1).PadLeft(4) + " " + sigmapts[2, 3].ToString(fmt1).PadLeft(4) + " " + sigmapts[2, 4].ToString(fmt1).PadLeft(4) + " " + sigmapts[2, 5].ToString(fmt1).PadLeft(4) + " ");
        }

        public void testremakecovpv()
        {
            double[,] sigmapts = new double[6, 12];
            double[] yu = new double[6];
            double[,] cov = new double[6, 6];
            double[,] cov2 = new double[6, 6];

            AstroLibr.remakecovpv(sigmapts, out yu, out cov);

            strbuild.AppendLine("cov = " + cov[0, 0].ToString(fmt1).PadLeft(4) + " " + cov[0, 1].ToString(fmt1).PadLeft(4) + " " + cov[0, 2].ToString(fmt1).PadLeft(4));
            strbuild.AppendLine("cov = " + cov[1, 0].ToString(fmt1).PadLeft(4) + " " + cov[1, 1].ToString(fmt1).PadLeft(4) + " " + cov[1, 2].ToString(fmt1).PadLeft(4));
            strbuild.AppendLine("cov = " + cov[2, 0].ToString(fmt1).PadLeft(4) + " " + cov[2, 1].ToString(fmt1).PadLeft(4) + " " + cov[2, 2].ToString(fmt1).PadLeft(4));
        }
        public void testremakecovp()
        {
            double[,] sigmapts = new double[6, 12];
            double[] yu = new double[6];
            double[,] cov = new double[6, 6];
            AstroLibr.remakecovp(sigmapts, out yu, out cov);

            strbuild.AppendLine("cov = " + cov[0, 0].ToString(fmt1).PadLeft(4) + " " + cov[0, 1].ToString(fmt1).PadLeft(4) + " " + cov[0, 2].ToString(fmt1).PadLeft(4));
            strbuild.AppendLine("cov = " + cov[1, 0].ToString(fmt1).PadLeft(4) + " " + cov[1, 1].ToString(fmt1).PadLeft(4) + " " + cov[1, 2].ToString(fmt1).PadLeft(4));
            strbuild.AppendLine("cov = " + cov[2, 0].ToString(fmt1).PadLeft(4) + " " + cov[2, 1].ToString(fmt1).PadLeft(4) + " " + cov[2, 2].ToString(fmt1).PadLeft(4));
        }
        public void testmatequal()
        {
            double[,] mat1 = new double[3, 3];
            double[,] mat3 = new double[3, 3];
            int matr;
            matr = 3;

            mat3 = MathTimeLibr.matequal(mat1, matr);

            strbuild.AppendLine("matequal = " + mat3[0, 0].ToString(fmt).PadLeft(4) + " " + mat3[0, 1].ToString(fmt).PadLeft(4) + " " + mat3[0, 2].ToString(fmt).PadLeft(4) + " ");
            strbuild.AppendLine("matequal = " + mat3[1, 0].ToString(fmt).PadLeft(4) + " " + mat3[1, 1].ToString(fmt).PadLeft(4) + " " + mat3[1, 2].ToString(fmt).PadLeft(4) + " ");
            strbuild.AppendLine("matequal = " + mat3[2, 0].ToString(fmt).PadLeft(4) + " " + mat3[2, 1].ToString(fmt).PadLeft(4) + " " + mat3[2, 2].ToString(fmt).PadLeft(4) + " ");
        }
        public void testmatscale()
        {
            double[,] mat1 = new double[3, 3];
            double[,] mat3 = new double[3, 3];
            int matr, matc;
            double scale;
            matr = 3;
            matc = 3;
            scale = 1.364;

            mat3 = MathTimeLibr.matscale(mat1, matr, matc, scale);

            strbuild.AppendLine("matscale = " + mat3[0, 0].ToString(fmt).PadLeft(4) + " " + mat3[0, 1].ToString(fmt).PadLeft(4) + " " + mat3[0, 2].ToString(fmt).PadLeft(4) + " ");
            strbuild.AppendLine("matscale = " + mat3[1, 0].ToString(fmt).PadLeft(4) + " " + mat3[1, 1].ToString(fmt).PadLeft(4) + " " + mat3[1, 2].ToString(fmt).PadLeft(4) + " ");
            strbuild.AppendLine("matscale = " + mat3[2, 0].ToString(fmt).PadLeft(4) + " " + mat3[2, 1].ToString(fmt).PadLeft(4) + " " + mat3[2, 2].ToString(fmt).PadLeft(4) + " ");
        }
        public void testnorm()
        {
            double[] vec1 = new double[3];
            double[] vec2 = new double[3];
            vec1 = new double[3] { 2.3, 4.7, -1.6 };

            vec2 = MathTimeLibr.norm(vec1);

            strbuild.AppendLine("norm = " + vec2[0].ToString(fmt).PadLeft(4) + " " + vec2[1].ToString(fmt).PadLeft(4) + " " + vec2[2].ToString(fmt).PadLeft(4) + " ");
        }
        public void testmag()
        {
            double[] x = new double[3];
            double magx;
            x = new double[3] { 1.0, 2.0, 5.0 };

            magx = MathTimeLibr.mag(x);

            strbuild.AppendLine("mag = " + magx.ToString(fmt).PadLeft(4));
        }
        public void testcross()
        {
            double[] vec1 = new double[3];
            double[] vec2 = new double[3];
            double[] outvec = new double[3];
            vec1 = new double[3] { 1.0, 2.0, 5.0 };
            vec2 = new double[3] { 2.3, 4.7, -1.6 };

            MathTimeLibr.cross(vec1, vec2, out outvec);

            strbuild.AppendLine("cross = " + outvec[0].ToString(fmt).PadLeft(4) + " " + outvec[1].ToString(fmt).PadLeft(4) + " " + outvec[2].ToString(fmt).PadLeft(4) + " ");
        }
        public void testdot()
        {
            double[] x = new double[3];
            double[] y = new double[3];
            double dotp;
            x = new double[3] { 1, 2, 5 };
            y = new double[3] { 2.3, 4.7, -1.6 };

            dotp = MathTimeLibr.dot(x, y);

            strbuild.AppendLine("x " + x[0].ToString(fmt).PadLeft(4) + " " + x[1].ToString(fmt).PadLeft(4) + " " + x[2].ToString(fmt).PadLeft(4));
            strbuild.AppendLine("y " + y[0].ToString(fmt).PadLeft(4) + " " + y[1].ToString(fmt).PadLeft(4) + " " + y[2].ToString(fmt).PadLeft(4));

            strbuild.AppendLine("dot = " + dotp.ToString(fmt).PadLeft(4));
        }

        public void testangle()
        {
            double[] vec1 = new double[3];
            double[] vec2 = new double[3];
            double ang;
            vec1 = new double[3] { 1, 2, 5 };
            vec2 = new double[3] { 2.3, 4.7, -1.6 };

            ang = MathTimeLibr.angle(vec1, vec2);

            strbuild.AppendLine("angle = " + ang.ToString(fmt).PadLeft(4) + " ");
        }
        public void testasinh()
        {
            double xval, ans;
            xval = 1.45;

            ans = MathTimeLibr.asinh(xval);

            strbuild.AppendLine("asinh = " + ans.ToString(fmt).PadLeft(4) + " ");
        }
        public void testcot()
        {
            double xval, ans;
            xval = 0.47238734;

            ans = MathTimeLibr.cot(xval);

            strbuild.AppendLine("cot = " + ans.ToString(fmt).PadLeft(4) + " ");
        }
        public void testacosh()
        {
            double xval, ans;
            xval = 1.43;

            ans = MathTimeLibr.acosh(xval);

            strbuild.AppendLine("acosh = " + ans.ToString(fmt).PadLeft(4) + " ");
        }
        public void testaddvec()
        {
            double a1, a2;
            double[] vec1 = new double[3];
            double[] vec2 = new double[3];
            double[] vec3 = new double[3];
            vec1 = new double[3] { 1, 2, 5 };
            vec2 = new double[3] { 2.3, 4.7, -5.6 };
            a1 = 1.0;
            a2 = 2.0;

            MathTimeLibr.addvec(a1, vec1, a2, vec2, out vec3);

            strbuild.AppendLine("vec1 " + vec1[0].ToString(fmt).PadLeft(4) + " " + vec1[1].ToString(fmt).PadLeft(4) + " " + vec1[2].ToString(fmt).PadLeft(4));
            strbuild.AppendLine("vec2 " + vec2[0].ToString(fmt).PadLeft(4) + " " + vec2[1].ToString(fmt).PadLeft(4) + " " + vec2[2].ToString(fmt).PadLeft(4));

            strbuild.AppendLine("addvec = " + vec3[0].ToString(fmt).PadLeft(4) + " " + vec3[1].ToString(fmt).PadLeft(4) + " " + vec3[2].ToString(fmt).PadLeft(4));
        }


        public void testPercentile()
        {
            double ans;
            double[] sequence = new double[15];
            double excelPercentile;
            Int32 arrSize;
            excelPercentile = 0.3;
            arrSize = 7;
            sequence[0] = 45.3;
            sequence[1] = 5.63;
            sequence[2] = 5.13;
            sequence[3] = 345.3;
            sequence[4] = 45.3;
            sequence[5] = 3445.3;
            sequence[6] = 0.03;

            ans = MathTimeLibr.Percentile(sequence, excelPercentile, arrSize);

            strbuild.AppendLine("percentile = " + ans.ToString(fmt).PadLeft(4) + " ");
        }
        public void testrot1()
        {
            double[] vec = new double[3];
            double[] vec3 = new double[3];
            double xval, rad;
            rad = 180.0 / Math.PI;
            vec[0] = 23.4;
            vec[1] = 6723.4;
            vec[2] = -2.4;
            xval = 225.0 / rad;

            vec3 = MathTimeLibr.rot1(vec, xval);

            strbuild.AppendLine("testrot1 = " + vec3[0].ToString(fmt).PadLeft(4) + " " + vec3[1].ToString(fmt).PadLeft(4) + " " + vec3[2].ToString(fmt).PadLeft(4));
        }
        public void testrot2()
        {
            double[] vec3 = new double[3];
            double[] vec = new double[3];
            double xval, rad;
            rad = 180.0 / Math.PI;
            vec[0] = 23.4;
            vec[1] = 6723.4;
            vec[2] = -2.4;
            xval = 23.4 / rad;
            vec3 = MathTimeLibr.rot2(vec, xval);

            strbuild.AppendLine("testrot2 = " + vec3[0].ToString(fmt).PadLeft(4) + " " + vec3[1].ToString(fmt).PadLeft(4) + " " + vec3[2].ToString(fmt).PadLeft(4));
        }
        public void testrot3()
        {
            double[] vec3 = new double[3];
            double[] vec = new double[3];
            double xval, rad;
            rad = 180.0 / Math.PI;
            vec[0] = 23.4;
            vec[1] = 6723.4;
            vec[2] = -2.4;
            xval = 323.4 / rad;
            vec3 = MathTimeLibr.rot3(vec, xval);

            strbuild.AppendLine("testrot3 = " + vec3[0].ToString(fmt).PadLeft(4) + " " + vec3[1].ToString(fmt).PadLeft(4) + " " + vec3[2].ToString(fmt).PadLeft(4));
        }
        public void testfactorial()
        {
            Int32 n;
            double ans;
            n = 4;

            ans = MathTimeLibr.factorial(n);

            strbuild.AppendLine("factorial = " + ans.ToString().PadLeft(4));
        }
        public void testcubicspl()
        {
            double p1, p2, p3, p4, acu0, acu1, acu2, acu3;
            p1 = 1.0;
            p2 = 3.5;
            p3 = 5.6;
            p4 = 32.0;

            MathTimeLibr.cubicspl(p1, p2, p3, p4, out acu0, out acu1, out acu2, out acu3);

            strbuild.AppendLine("cubicspl = " + acu0.ToString(fmt).PadLeft(7) + acu1.ToString(fmt).PadLeft(7)
                + acu2.ToString(fmt).PadLeft(7) + acu3.ToString(fmt).PadLeft(7));
        }
        public void testcubic()
        {
            double a3, b2, c1, d0;
            char opt;
            double r1r, r1i, r2r, r2i, r3r, r3i;
            a3 = 1.7;
            b2 = 3.5;
            c1 = 5.6;
            d0 = 32.0;
            opt = 'I';  // all roots, unique, real

            MathTimeLibr.cubic(a3, b2, c1, d0, opt, out r1r, out r1i, out r2r, out r2i, out r3r, out r3i);

            strbuild.AppendLine("cubic = " + r1r.ToString(fmt).PadLeft(7) + r1i.ToString(fmt).PadLeft(7)
                + r2r.ToString(fmt).PadLeft(7) + r2i.ToString(fmt).PadLeft(7)
                + r3r.ToString(fmt).PadLeft(7) + r3i.ToString(fmt).PadLeft(7));
        }
        public void testcubicinterp()
        {
            double p1a, p1b, p1c, p1d, p2a, p2b, p2c, p2d, valuein;
            double ans;
            p1a = 1.7;
            p1b = 3.5;
            p1c = 5.6;
            p1d = 11.7;
            p2a = 21.7;
            p2b = 35.5;
            p2c = 57.6;
            p2d = 181.7;
            valuein = 4.0;

            ans = MathTimeLibr.cubicinterp(p1a, p1b, p1c, p1d, p2a, p2b, p2c, p2d, valuein);

            strbuild.AppendLine("cubicint = " + ans.ToString(fmt).PadLeft(7));
        }

        public void testquadratic()
        {
            double a, b, c;
            char opt;
            double r1r, r1i, r2r, r2i;
            a = 1.7;
            b = 3.5;
            c = 5.6;
            opt = 'I';  // all roots, unique, real

            MathTimeLibr.quadratic(a, b, c, opt, out r1r, out r1i, out r2r, out r2i);

            strbuild.AppendLine("quad = " + r1r.ToString(fmt).PadLeft(7) + r1i.ToString(fmt).PadLeft(7)
                + r2r.ToString(fmt).PadLeft(7) + r2i.ToString(fmt).PadLeft(7));
        }

        public void testconvertMonth()
        {
            string monstr;
            monstr = "Jan";
            int mon;

            mon = MathTimeLibr.convertMonth(monstr);
        }

        public void testjday()
        {
            int year;
            double jd, jdFrac;
            int mon, day, hr, minute;
            double second;
            year = 2020;
            mon = 12;
            day = 15;
            hr = 16;
            minute = 58;
            second = 50.208;
            MathTimeLibr.jday(year, mon, day, hr, minute, second, out jd, out jdFrac);

            strbuild.AppendLine("jd " + jd.ToString(fmt).PadLeft(4) + " " + jdFrac.ToString(fmt).PadLeft(4));
        }
        public void testdays2mdhms()
        {
            int year;
            double days;
            int mon, day, hr, minute;
            double sec;
            year = 2020;
            days = 237.456982367;

            MathTimeLibr.days2mdhms(year, days, out mon, out day, out hr, out minute, out sec);
        }
        public void testinvjday()
        {
            int year;
            int mon, day, hr, minute;
            double jd, jdFrac, second;
            jd = 2449877.0;
            jdFrac = 0.3458762;

            MathTimeLibr.invjday(jd, jdFrac, out year, out mon, out day, out hr, out minute, out second);
        }

        // tests eop, spw, and fk5 iau80
        public void testiau80in()
        {
            Int32 year, mon, day, hr, minute, dat;
            double jd, jdFrac, jdut1, second, dut1, ttt, lod, xp, yp, ddx, ddy, ddpsi, ddeps;
            int i;
            double f107, f107bar, ap, avgap, kp, sumkp;
            double[] aparr = new double[8];
            double[] kparr = new double[8];
            Int32 mjdeopstart, mjdspwstart, ktrActObs;
            string EOPupdate;

            year = 2017;
            mon = 4;
            day = 6;
            hr = 0;
            minute = 0;
            second = 0.0;

            string nutLoc;
            nutLoc = @"D:\Codes\LIBRARY\DataLib\nut80.dat";
            EOPSPWLibr.iau80in(nutLoc, out EOPSPWLibr.iau80arr);

            string eopFileName = @"D:\Codes\LIBRARY\DataLib\EOP-All-v1.1_2018-01-04.txt";
            EOPSPWLibr.readeop(ref EOPSPWLibr.eopdata, eopFileName, out mjdeopstart, out ktrActObs, out EOPupdate);
            int y, m, d, h, mm;
            double ss;
            strbuild.AppendLine("EOP tests  mfme    dut1  dat    lod           xp                      yp               ddpsi                   ddeps               ddx                 ddy");
            for (i = 0; i < 90; i++)
            {
                MathTimeLibr.jday(year, mon, day, hr + i, minute, second, out jd, out jdFrac);
                EOPSPWLibr.findeopparam(jd, jdFrac, 's', EOPSPWLibr.eopdata, mjdeopstart + 2400000.5, out dut1, out dat,
                   out lod, out xp, out yp, out ddpsi, out ddeps, out ddx, out ddy);
                MathTimeLibr.invjday(jd, jdFrac, out y, out m, out d, out h, out mm, out ss);
                strbuild.AppendLine(y.ToString("0000") + " " + m.ToString("00") + " " + d.ToString("00") + " " + (h * 60 + mm).ToString("0000") + " " +
                    dut1.ToString(fmt).PadLeft(4) + " " + dat.ToString("00").PadLeft(4) + " " + lod.ToString(fmt).PadLeft(4) + " " + xp.ToString(fmtE).PadLeft(4) + " " + yp.ToString(fmtE).PadLeft(4) + " " +
                    ddpsi.ToString(fmtE).PadLeft(4) + " " + ddeps.ToString(fmtE).PadLeft(4) + " " + ddx.ToString(fmtE).PadLeft(4) + " " + ddy.ToString(fmtE).PadLeft(4) + " ");
            }

            string spwFileName = @"D:\Codes\LIBRARY\DataLib\SpaceWeather-All-v1.2_2018-01-04.txt";
            EOPSPWLibr.readspw(ref EOPSPWLibr.spwdata, spwFileName, out mjdspwstart, out ktrActObs);
            strbuild.AppendLine("SPW tests  mfme f107 f107bar ap apavg  kp sumkp aparr[]  ");
            for (i = 0; i < 90; i++)
            {
                MathTimeLibr.jday(year, mon, day, hr + i, minute, second, out jd, out jdFrac);
                // adj obs, last ctr, act con
                EOPSPWLibr.findspwparam(jd, jdFrac, 's', 'a', 'l', 'a', EOPSPWLibr.spwdata, mjdspwstart + 2400000.5, out f107, out f107bar,
                   out ap, out avgap, aparr, out kp, out sumkp, kparr);
                MathTimeLibr.invjday(jd, jdFrac, out y, out m, out d, out h, out mm, out ss);
                strbuild.AppendLine(y.ToString("0000") + " " + m.ToString("00") + " " + d.ToString("00") + " " + (h * 60 + mm).ToString("0000") + " " +
                   f107.ToString(fmt).PadLeft(4) + " " + f107bar.ToString(fmt).PadLeft(4) + " " + ap.ToString(fmt).PadLeft(4) + " " + avgap.ToString(fmt).PadLeft(4) + " " + kp.ToString(fmt).PadLeft(4) + " " +
                   sumkp.ToString(fmt).PadLeft(4) + " " + aparr[0].ToString(fmt).PadLeft(4) + " " + aparr[1].ToString(fmt).PadLeft(4) + " " + aparr[2].ToString(fmt).PadLeft(4) + " ");
            }
        }

        public void testfundarg()
        {
            double[] fArgs = new double[14];
            double ttt;
            AstroLib.EOpt opt = AstroLib.EOpt.e80;
            ttt = 0.042623631889;

            AstroLibr.fundarg(ttt, opt, out fArgs);
            strbuild.AppendLine("fundarg = " + fArgs[0].ToString(fmt).PadLeft(4) + " " + fArgs[1].ToString(fmt).PadLeft(4) + " " 
                + fArgs[2].ToString(fmt).PadLeft(4) + " " + fArgs[3].ToString(fmt).PadLeft(4) + " " + fArgs[4].ToString(fmt).PadLeft(4) + " "
                + fArgs[5].ToString(fmt).PadLeft(4) + " " + fArgs[6].ToString(fmt).PadLeft(4) + " " + fArgs[7].ToString(fmt).PadLeft(4) + " "
                + fArgs[8].ToString(fmt).PadLeft(4) + " " + fArgs[9].ToString(fmt).PadLeft(4) + " " + fArgs[10].ToString(fmt).PadLeft(4) + " " 
                + fArgs[11].ToString(fmt).PadLeft(4) + " " + fArgs[12].ToString(fmt).PadLeft(4) + " " + fArgs[13].ToString(fmt).PadLeft(4));

            // do in deg
            for (int i = 0; i < 14; i++)
                fArgs[i] = fArgs[i] * 180.0/Math.PI;

            strbuild.AppendLine("fundarg = " + fArgs[0].ToString(fmt).PadLeft(4) + " " + fArgs[1].ToString(fmt).PadLeft(4) + " "
                + fArgs[2].ToString(fmt).PadLeft(4) + " " + fArgs[3].ToString(fmt).PadLeft(4) + " " + fArgs[4].ToString(fmt).PadLeft(4) + " "
                + fArgs[5].ToString(fmt).PadLeft(4) + " " + fArgs[6].ToString(fmt).PadLeft(4) + " " + fArgs[7].ToString(fmt).PadLeft(4) + " "
                + fArgs[8].ToString(fmt).PadLeft(4) + " " + fArgs[9].ToString(fmt).PadLeft(4) + " " + fArgs[10].ToString(fmt).PadLeft(4) + " "
                + fArgs[11].ToString(fmt).PadLeft(4) + " " + fArgs[12].ToString(fmt).PadLeft(4) + " " + fArgs[13].ToString(fmt).PadLeft(4));


            AstroLibr.fundarg(ttt, AstroLib.EOpt.e06cio, out fArgs);
            strbuild.AppendLine("fundarg = " + fArgs[0].ToString(fmt).PadLeft(4) + " " + fArgs[1].ToString(fmt).PadLeft(4) + " "
                + fArgs[2].ToString(fmt).PadLeft(4) + " " + fArgs[3].ToString(fmt).PadLeft(4) + " " + fArgs[4].ToString(fmt).PadLeft(4) + " "
                + fArgs[5].ToString(fmt).PadLeft(4) + " " + fArgs[6].ToString(fmt).PadLeft(4) + " " + fArgs[7].ToString(fmt).PadLeft(4) + " "
                + fArgs[8].ToString(fmt).PadLeft(4) + " " + fArgs[9].ToString(fmt).PadLeft(4) + " " + fArgs[10].ToString(fmt).PadLeft(4) + " "
                + fArgs[11].ToString(fmt).PadLeft(4) + " " + fArgs[12].ToString(fmt).PadLeft(4) + " " + fArgs[13].ToString(fmt).PadLeft(4));

            // do in deg
            for (int i = 0; i < 14; i++)
                fArgs[i] = fArgs[i] * 180.0 / Math.PI;

            strbuild.AppendLine("fundarg = " + fArgs[0].ToString(fmt).PadLeft(4) + " " + fArgs[1].ToString(fmt).PadLeft(4) + " "
                + fArgs[2].ToString(fmt).PadLeft(4) + " " + fArgs[3].ToString(fmt).PadLeft(4) + " " + fArgs[4].ToString(fmt).PadLeft(4) + " "
                + fArgs[5].ToString(fmt).PadLeft(4) + " " + fArgs[6].ToString(fmt).PadLeft(4) + " " + fArgs[7].ToString(fmt).PadLeft(4) + " "
                + fArgs[8].ToString(fmt).PadLeft(4) + " " + fArgs[9].ToString(fmt).PadLeft(4) + " " + fArgs[10].ToString(fmt).PadLeft(4) + " "
                + fArgs[11].ToString(fmt).PadLeft(4) + " " + fArgs[12].ToString(fmt).PadLeft(4) + " " + fArgs[13].ToString(fmt).PadLeft(4));


        }
        public void testprecess()
        {
            double ttt, psia, wa, epsa, chia;
            double[,] prec = new double[3, 3];
            AstroLib.EOpt opt = AstroLib.EOpt.e80;

            ttt = 0.042623631889;
           // ttt = 0.04262362174880504;
            prec = AstroLibr.precess(ttt, opt, out psia, out wa, out epsa, out chia);

            strbuild.AppendLine("prec = " + prec[0, 0].ToString(fmt).PadLeft(4) + " " + prec[0, 1].ToString(fmt).PadLeft(4) + " " + prec[0, 2].ToString(fmt).PadLeft(4) + " ");
            strbuild.AppendLine("prec = " + prec[1, 0].ToString(fmt).PadLeft(4) + " " + prec[1, 1].ToString(fmt).PadLeft(4) + " " + prec[1, 2].ToString(fmt).PadLeft(4) + " ");
            strbuild.AppendLine("prec = " + prec[2, 0].ToString(fmt).PadLeft(4) + " " + prec[2, 1].ToString(fmt).PadLeft(4) + " " + prec[2, 2].ToString(fmt).PadLeft(4) + " ");

            prec = AstroLibr.precess(ttt, AstroLib.EOpt.e06eq, out psia, out wa, out epsa, out chia);

            strbuild.AppendLine("prec00 = " + prec[0, 0].ToString(fmt).PadLeft(4) + " " + prec[0, 1].ToString(fmt).PadLeft(4) + " " + prec[0, 2].ToString(fmt).PadLeft(4) + " ");
            strbuild.AppendLine("prec00 = " + prec[1, 0].ToString(fmt).PadLeft(4) + " " + prec[1, 1].ToString(fmt).PadLeft(4) + " " + prec[1, 2].ToString(fmt).PadLeft(4) + " ");
            strbuild.AppendLine("prec00 = " + prec[2, 0].ToString(fmt).PadLeft(4) + " " + prec[2, 1].ToString(fmt).PadLeft(4) + " " + prec[2, 2].ToString(fmt).PadLeft(4) + " ");
        }

        public void testnutation()
        {
            double[] fArgs = new double[14];
            double ttt, ddpsi, ddeps;
            double[,] nut = new double[3, 3];
            double[,] nut00 = new double[3, 3];
            AstroLib.EOpt opt = AstroLib.EOpt.e80;
            EOPSPWLib.iau80Class iau80arr;
            EOPSPWLib.iau00Class iau00arr;
            string nutLoc;
            nutLoc = @"D:\Codes\LIBRARY\DataLib\nut80.dat";
            EOPSPWLibr.iau80in(nutLoc, out iau80arr);
            nutLoc = @"D:\Codes\LIBRARY\DataLib\";
            EOPSPWLibr.iau00in(nutLoc, out iau00arr);

            double deltapsi, deltaeps, trueeps, meaneps;
            ttt = 0.042623631889;
            ddpsi = -0.052195;
            ddeps = -0.003875;

            AstroLibr.fundarg(ttt, opt, out fArgs);

            nut = AstroLibr.nutation(ttt, ddpsi, ddeps, EOPSPWLibr.iau80arr, opt, fArgs, out deltapsi, out deltaeps, out trueeps, out meaneps);

            strbuild.AppendLine("nut = " + nut[0, 0].ToString(fmt).PadLeft(4) + " " + nut[0, 1].ToString(fmt).PadLeft(4) + " " + nut[0, 2].ToString(fmt).PadLeft(4) + " ");
            strbuild.AppendLine("nut = " + nut[1, 0].ToString(fmt).PadLeft(4) + " " + nut[1, 1].ToString(fmt).PadLeft(4) + " " + nut[1, 2].ToString(fmt).PadLeft(4) + " ");
            strbuild.AppendLine("nut = " + nut[2, 0].ToString(fmt).PadLeft(4) + " " + nut[2, 1].ToString(fmt).PadLeft(4) + " " + nut[2, 2].ToString(fmt).PadLeft(4) + " ");

            AstroLibr.fundarg(ttt, AstroLib.EOpt.e06eq, out fArgs);
            nut00 = AstroLibr.precnutbias00a(ttt, ddpsi, ddeps, EOPSPWLibr.iau00arr, AstroLib.EOpt.e06eq, fArgs);
            strbuild.AppendLine("nut06 c= " + nut[0, 0].ToString(fmt).PadLeft(4) + " " + nut[0, 1].ToString(fmt).PadLeft(4) + " " + nut[0, 2].ToString(fmt).PadLeft(4) + " ");
            strbuild.AppendLine("nut06 c= " + nut[1, 0].ToString(fmt).PadLeft(4) + " " + nut[1, 1].ToString(fmt).PadLeft(4) + " " + nut[1, 2].ToString(fmt).PadLeft(4) + " ");
            strbuild.AppendLine("nut06 c= " + nut[2, 0].ToString(fmt).PadLeft(4) + " " + nut[2, 1].ToString(fmt).PadLeft(4) + " " + nut[2, 2].ToString(fmt).PadLeft(4) + " ");

            //AstroLibr.fundarg(ttt, AstroLib.EOpt.e00a, out fArgs);
            //nut00 = AstroLibr.nutation00a(ttt, ddpsi, ddeps, EOPSPWLibr.iau00arr, AstroLib.EOpt.e00a);
            //strbuild.AppendLine("nut06 a= " + nut[0, 0].ToString(fmt).PadLeft(4) + " " + nut[0, 1].ToString(fmt).PadLeft(4) + " " + nut[0, 2].ToString(fmt).PadLeft(4) + " ");
            //strbuild.AppendLine("nut06 a= " + nut[1, 0].ToString(fmt).PadLeft(4) + " " + nut[1, 1].ToString(fmt).PadLeft(4) + " " + nut[1, 2].ToString(fmt).PadLeft(4) + " ");
            //strbuild.AppendLine("nut06 a= " + nut[2, 0].ToString(fmt).PadLeft(4) + " " + nut[2, 1].ToString(fmt).PadLeft(4) + " " + nut[2, 2].ToString(fmt).PadLeft(4) + " ");
        }


        public void testnutationqmod()
        {
            double[] fArgs = new double[14];
            double[,] nutq = new double[3, 3];
            double ttt, deltapsi, deltaeps, meaneps;
            AstroLib.EOpt opt = AstroLib.EOpt.e80;
            string nutLoc;
            nutLoc = @"D:\Codes\LIBRARY\DataLib\nut80.dat";
            EOPSPWLibr.iau80in(nutLoc, out EOPSPWLibr.iau80arr);
            ttt = 0.042623631889;
           // ttt = 0.04262362174880504;

            AstroLibr.fundarg(ttt, opt, out fArgs);

            nutq = AstroLibr.nutationqmod(ttt, EOPSPWLibr.iau80arr, opt, fArgs, out deltapsi, out deltaeps, out meaneps);
        }
        public void testsidereal()
        {
            double[] fArgs = new double[14];
            double[,] nut = new double[3, 3];
            double[,] st = new double[3, 3];
            double ttt, jdut1, deltapsi, deltaeps, meaneps, trueeps, ddpsi, ddeps, lod;
            int eqeterms = 2;
            AstroLib.EOpt opt = AstroLib.EOpt.e80;
            EOPSPWLib.iau80Class iau80arr;
            EOPSPWLib.iau00Class iau00arr;
            jdut1 = 2453101.82740678310;
            ttt = 0.042623631889;
            //ttt = 0.04262362174880504;
            lod = 0.001556;
            string nutLoc;
            nutLoc = @"D:\Codes\LIBRARY\DataLib\nut80.dat";
            EOPSPWLibr.iau80in(nutLoc, out iau80arr);
            nutLoc = @"D:\Codes\LIBRARY\DataLib\";
            EOPSPWLibr.iau00in(nutLoc, out iau00arr);

            ddpsi = -0.052195;
            ddeps = -0.003875;

            AstroLibr.fundarg(ttt, opt, out fArgs);
            nut = AstroLibr.nutation(ttt, ddpsi, ddeps, EOPSPWLibr.iau80arr, opt, fArgs, 
                out deltapsi, out deltaeps, out trueeps, out meaneps);
            st = AstroLibr.sidereal(jdut1, deltapsi, meaneps, fArgs, lod, eqeterms, opt);
            strbuild.AppendLine("st = " + st[0, 0].ToString(fmt).PadLeft(4) + " " + st[0, 1].ToString(fmt).PadLeft(4) + " " + st[0, 2].ToString(fmt).PadLeft(4) + " ");
            strbuild.AppendLine("st = " + st[1, 0].ToString(fmt).PadLeft(4) + " " + st[1, 1].ToString(fmt).PadLeft(4) + " " + st[1, 2].ToString(fmt).PadLeft(4) + " ");
            strbuild.AppendLine("st = " + st[2, 0].ToString(fmt).PadLeft(4) + " " + st[2, 1].ToString(fmt).PadLeft(4) + " " + st[2, 2].ToString(fmt).PadLeft(4) + " ");


            AstroLibr.fundarg(ttt, AstroLib.EOpt.e06eq, out fArgs);
            nut = AstroLibr.nutation(ttt, ddpsi, ddeps, EOPSPWLibr.iau80arr, AstroLib.EOpt.e06eq, fArgs, 
                out deltapsi, out deltaeps, out trueeps, out meaneps);
            st = AstroLibr.sidereal(jdut1, deltapsi, meaneps, fArgs, lod, eqeterms, AstroLib.EOpt.e06eq);
            strbuild.AppendLine("st00 = " + st[0, 0].ToString(fmt).PadLeft(4) + " " + st[0, 1].ToString(fmt).PadLeft(4) + " " + st[0, 2].ToString(fmt).PadLeft(4) + " ");
            strbuild.AppendLine("st00 = " + st[1, 0].ToString(fmt).PadLeft(4) + " " + st[1, 1].ToString(fmt).PadLeft(4) + " " + st[1, 2].ToString(fmt).PadLeft(4) + " ");
            strbuild.AppendLine("st00 = " + st[2, 0].ToString(fmt).PadLeft(4) + " " + st[2, 1].ToString(fmt).PadLeft(4) + " " + st[2, 2].ToString(fmt).PadLeft(4) + " ");
        }
        public void testpolarm()
        {
            double[,] pm = new double[3, 3];
            double xp, yp, ttt;
            AstroLib.EOpt opt = AstroLib.EOpt.e80;

            ttt = 0.042623631889;
            //ttt = 0.04262363188899416;
            xp = 0.0;
            yp = 0.0;
            opt = AstroLib.EOpt.e80;

            pm = AstroLibr.polarm(xp, yp, ttt, opt);

            strbuild.AppendLine("pm = " + pm[0, 0].ToString(fmt).PadLeft(4) + " " + pm[0, 1].ToString(fmt).PadLeft(4) + " " + pm[0, 2].ToString(fmt).PadLeft(4) + " ");
            strbuild.AppendLine("pm = " + pm[1, 0].ToString(fmt).PadLeft(4) + " " + pm[1, 1].ToString(fmt).PadLeft(4) + " " + pm[1, 2].ToString(fmt).PadLeft(4) + " ");
            strbuild.AppendLine("pm = " + pm[2, 0].ToString(fmt).PadLeft(4) + " " + pm[2, 1].ToString(fmt).PadLeft(4) + " " + pm[2, 2].ToString(fmt).PadLeft(4) + " ");

            pm = AstroLibr.polarm(xp, yp, ttt, AstroLib.EOpt.e06eq);

            strbuild.AppendLine("pm06 = " + pm[0, 0].ToString(fmt).PadLeft(4) + " " + pm[0, 1].ToString(fmt).PadLeft(4) + " " + pm[0, 2].ToString(fmt).PadLeft(4) + " ");
            strbuild.AppendLine("pm06 = " + pm[1, 0].ToString(fmt).PadLeft(4) + " " + pm[1, 1].ToString(fmt).PadLeft(4) + " " + pm[1, 2].ToString(fmt).PadLeft(4) + " ");
            strbuild.AppendLine("pm06 = " + pm[2, 0].ToString(fmt).PadLeft(4) + " " + pm[2, 1].ToString(fmt).PadLeft(4) + " " + pm[2, 2].ToString(fmt).PadLeft(4) + " ");
        }
        public void testgstime()
        {
            double gst, jdut1;
            jdut1 = 2453101.82740678310;

            gst = AstroLibr.gstime(jdut1);

            strbuild.AppendLine("gst = " + gst.ToString(fmt).PadLeft(4) + " " + (gst * 180.0 / Math.PI).ToString(fmt).PadLeft(4));
        }

        public void testlstime()
        {
            double rad = 180.0 / Math.PI;
            double lon, jdut1, lst, gst;
            lon = -104.0 / rad;
            jdut1 = 2453101.82740678310;

            AstroLibr.lstime(lon, jdut1, out lst, out gst);

            strbuild.AppendLine("lst = " + lst.ToString(fmt).PadLeft(4) + " " + (lst * 180.0 / Math.PI).ToString(fmt).PadLeft(4));
        }

        public void testhms_sec()
        {
            int hr, min;
            double sec;
            double utsec;
            hr = 12;
            min = 34;
            sec = 56.233;
            utsec = 0.0;

            MathTimeLibr.hms_sec(ref hr, ref min, ref sec, MathTimeLib.Edirection.eto, ref utsec);

            strbuild.AppendLine("utsec = " + utsec.ToString(fmt).PadLeft(4));
        }

        public void testhms_ut()
        {
            int hr, min;
            double sec;
            double ut;
            hr = 13;
            min = 22;
            sec = 45.98;
            ut = 0.0;

            MathTimeLibr.hms_ut(ref hr, ref min, ref sec, MathTimeLib.Edirection.eto, ref ut);

            strbuild.AppendLine("ut = " + ut.ToString(fmt).PadLeft(4));
        }

        public void testhms_rad()
        {
            int hr, min;
            double sec;
            double hms;
            hr = 15;
            min = 15;
            sec = 53.63;
            hms = 0.0;

            MathTimeLibr.hms_rad(ref hr, ref min, ref sec, MathTimeLib.Edirection.eto, ref hms);

            strbuild.AppendLine("hms = " + hms.ToString(fmt).PadLeft(4));
        }

        public void testdms_rad()
        {
            int deg, min;
            double sec;
            double dms;
            deg = -35;
            min = -15;
            sec = -53.63;
            dms = 0.0;

            MathTimeLibr.dms_rad(ref deg, ref min, ref sec, MathTimeLib.Edirection.eto, ref dms);

            strbuild.AppendLine("dms = " + dms.ToString(fmt).PadLeft(4));
        }


        public void testeci_ecef()
        {
            double[] fArgs = new double[14];
            double[] reci = new double[3];
            double[] veci = new double[3];
            double[] recef = new double[3];
            double[] vecef = new double[3];
            double conv;
            Int32 year, mon, day, hr, minute, dat;
            double jd, jdFrac, jdut1, second, dut1, ttt, lod, xp, yp, ddx, ddy, ddpsi, ddeps,
                jdtt, jdftt;
            double x, y, s;
            
            conv = Math.PI / (180.0 * 3600.0);  // arcsec to rad

            recef = new double[] { -1033.4793830, 7901.2952754, 6380.3565958 };
            vecef = new double[] { -3.225636520, -2.872451450, 5.531924446 };
            year = 2004;
            mon = 4;
            day = 6;
            hr = 7;
            minute = 51;
            second = 28.386009;

            // sofa example -------------
            //year = 2007;
            //mon = 4;
            //day = 5;
            //hr = 12;
            //minute = 0;
            //second = 0.0;

            MathTimeLibr.jday(year, mon, day, hr, minute, second, out jd, out jdFrac);

            dut1 = -0.4399619;      // sec
            dat = 32;               // sec
            xp = -0.140682 * conv;  // " to rad
            yp = 0.333309 * conv;
            lod = 0.0015563;
            ddpsi = -0.052195 * conv;  // " to rad
            ddeps = -0.003875 * conv;
            ddx = -0.000205 * conv;    // " to rad
            ddy = -0.000136 * conv;

            // sofa example -------------
            //dut1 = -0.072073685;      // sec
            //dat = 33;                // sec
            //xp = 0.0349282 * conv;  // " to rad
            //yp = 0.4833163 * conv;
            //lod = 0.0;
            //ddpsi = -0.0550655 * conv;  // " to rad
            //ddeps = -0.006358 * conv;
            //ddx = 0.000175 * conv;    // " to rad
            //ddy = -0.0002259 * conv;

            jdtt = jd;
            jdftt = jdFrac + (dat + 32.184)/ 86400.0;
            ttt = (jdtt + jdftt - 2451545.0 ) / 36525.0;
            Console.WriteLine("ttt wo base (use this) " + ttt.ToString());
            jdut1 = jd + jdFrac + dut1 / 86400.0;

            strbuild.AppendLine("ITRF          IAU-76/FK5   " + recef[0].ToString(fmt).PadLeft(4) + " " + recef[1].ToString(fmt).PadLeft(4) + " " + recef[2].ToString(fmt).PadLeft(4) + " "
                + vecef[0].ToString(fmt).PadLeft(4) + " " + vecef[1].ToString(fmt).PadLeft(4) + " " + vecef[2].ToString(fmt).PadLeft(4));

            string nutLoc;
            EOPSPWLib.iau80Class iau80arr;
            EOPSPWLib.iau00Class iau00arr;
            nutLoc = @"D:\Codes\LIBRARY\DataLib\nut80.dat";
            EOPSPWLibr.iau80in(nutLoc, out iau80arr);

            nutLoc = @"D:\Codes\LIBRARY\DataLib\";
            EOPSPWLibr.iau00in(nutLoc, out iau00arr);

            // test creating xys file
            nutLoc = @"D:\Codes\LIBRARY\DataLib\";
            // done, works. :-)
            //AstroLibr.createXYS(nutLoc, iau00arr, fArgs);

            // now read it in
            double jdxysstart, jdfxysstart;
            AstroLib.xysdataClass[] xysarr = AstroLibr.xysarr;
            AstroLibr.initXYS(ref xysarr, nutLoc, "xysdata.dat", out jdxysstart, out jdfxysstart);

            // now test it for interpolation
            //jdtt = jd + jdFrac + (dat + 32.184) / 86400.0;
            AstroLibr.iau06xysSeries(ttt, iau00arr, fArgs, out x, out y, out s);
            strbuild.AppendLine("iau06xys     x   " + x.ToString() + " y " + y.ToString() + " s " + s.ToString());
            AstroLibr.findxysparam(jdtt + jdftt, 0.0, 'n', xysarr, jdxysstart, out x, out y, out s);
            strbuild.AppendLine("findxysparam n x " + x.ToString() + "   y " + y.ToString() + "   s " + s.ToString());
            AstroLibr.findxysparam(jdtt + jdftt, 0.0, 'l', xysarr, jdxysstart, out x, out y, out s);
            strbuild.AppendLine("findxysparam l x " + x.ToString() + " y " + y.ToString() + " s " + s.ToString());
            AstroLibr.findxysparam(jdtt + jdftt, 0.0, 's', xysarr, jdxysstart, out x, out y, out s);
            strbuild.AppendLine("findxysparam s x " + x.ToString() + " y " + y.ToString() + " s " + s.ToString());

            AstroLibr.eci_ecef(ref reci, ref veci, MathTimeLib.Edirection.efrom, ref recef, ref vecef,
                 AstroLib.EOpt.e80, iau80arr, iau00arr,
                 jdtt, jdftt, jdut1, jdxysstart, lod, xp, yp, ddpsi, ddeps, ddx, ddy);
            strbuild.AppendLine("GCRF          IAU-76/FK5   " + reci[0].ToString(fmt).PadLeft(4) + " " + reci[1].ToString(fmt).PadLeft(4) + " " + reci[2].ToString(fmt).PadLeft(4) + " "
                + veci[0].ToString(fmt).PadLeft(4) + " " + veci[1].ToString(fmt).PadLeft(4) + " " + veci[2].ToString(fmt).PadLeft(4));

            Console.WriteLine(" checking book test");
            AstroLibr.eci_ecef(ref reci, ref veci, MathTimeLib.Edirection.efrom, ref recef, ref vecef,
                 AstroLib.EOpt.e06cio, iau80arr, iau00arr,
                 jdtt, jdftt, jdut1, jdxysstart, lod, xp, yp, ddpsi, ddeps, ddx, ddy);
            strbuild.AppendLine("GCRF          IAU-2006 CIO " + reci[0].ToString(fmt).PadLeft(4) + " " + reci[1].ToString(fmt).PadLeft(4) + " " + reci[2].ToString(fmt).PadLeft(4) + " "
                + veci[0].ToString(fmt).PadLeft(4) + " " + veci[1].ToString(fmt).PadLeft(4) + " " + veci[2].ToString(fmt).PadLeft(4));
            Console.WriteLine("GCRF          IAU-2006 CIO " + reci[0].ToString(fmt).PadLeft(4) + " " + reci[1].ToString(fmt).PadLeft(4) + " " + reci[2].ToString(fmt).PadLeft(4) + " "
                + veci[0].ToString(fmt).PadLeft(4) + " " + veci[1].ToString(fmt).PadLeft(4) + " " + veci[2].ToString(fmt).PadLeft(4));


            // try backwards
            AstroLibr.eci_ecef(ref reci, ref veci, MathTimeLib.Edirection.eto, ref recef, ref vecef,
                AstroLib.EOpt.e06cio, iau80arr, iau00arr,
                jdtt, jdftt, jdut1, jdxysstart, lod, xp, yp, ddpsi, ddeps, ddx, ddy);
            strbuild.AppendLine("ITRF rev       IAU-76/FK5   " + recef[0].ToString(fmt).PadLeft(4) + " " + recef[1].ToString(fmt).PadLeft(4) + " " + recef[2].ToString(fmt).PadLeft(4) + " "
                + vecef[0].ToString(fmt).PadLeft(4) + " " + vecef[1].ToString(fmt).PadLeft(4) + " " + vecef[2].ToString(fmt).PadLeft(4));
            recef = new double[] { -1033.4793830, 7901.2952754, 6380.3565958 };
            vecef = new double[] { -3.225636520, -2.872451450, 5.531924446 };


            // these are not correct
            AstroLibr.eci_ecef(ref reci, ref veci, MathTimeLib.Edirection.efrom, ref recef, ref vecef,
                 AstroLib.EOpt.e06eq, iau80arr, iau00arr,
                 jdtt, jdftt, jdut1, jdxysstart, lod, xp, yp, ddpsi, ddeps, ddx, ddy);
            strbuild.AppendLine("GCRF          IAU-2006 06  " + reci[0].ToString(fmt).PadLeft(4) + " " + reci[1].ToString(fmt).PadLeft(4) + " " + reci[2].ToString(fmt).PadLeft(4) + " "
                + veci[0].ToString(fmt).PadLeft(4) + " " + veci[1].ToString(fmt).PadLeft(4) + " " + veci[2].ToString(fmt).PadLeft(4));
            Console.WriteLine("GCRF          IAU-2006 06  " + reci[0].ToString(fmt).PadLeft(4) + " " + reci[1].ToString(fmt).PadLeft(4) + " " + reci[2].ToString(fmt).PadLeft(4) + " "
                + veci[0].ToString(fmt).PadLeft(4) + " " + veci[1].ToString(fmt).PadLeft(4) + " " + veci[2].ToString(fmt).PadLeft(4));

            AstroLibr.eci_ecef(ref reci, ref veci, MathTimeLib.Edirection.efrom, ref recef, ref vecef,
                 AstroLib.EOpt.e00a, iau80arr, iau00arr,
                 jdtt, jdftt, jdut1, jdxysstart, lod, xp, yp, ddpsi, ddeps, ddx, ddy);
            strbuild.AppendLine("GCRF          IAU-2006 00a  " + reci[0].ToString(fmt).PadLeft(4) + " " + reci[1].ToString(fmt).PadLeft(4) + " " + reci[2].ToString(fmt).PadLeft(4) + " "
                + veci[0].ToString(fmt).PadLeft(4) + " " + veci[1].ToString(fmt).PadLeft(4) + " " + veci[2].ToString(fmt).PadLeft(4));
            Console.WriteLine("GCRF          IAU-2006 00a  " + reci[0].ToString(fmt).PadLeft(4) + " " + reci[1].ToString(fmt).PadLeft(4) + " " + reci[2].ToString(fmt).PadLeft(4) + " "
                + veci[0].ToString(fmt).PadLeft(4) + " " + veci[1].ToString(fmt).PadLeft(4) + " " + veci[2].ToString(fmt).PadLeft(4));
            strbuild.AppendLine("00a case is wrong");



            // writeout data for table interpolation
            Int32 i;
            year = 1980;
            mon = 1;
            day = 1;
            hr = 0;
            minute = 0;
            second = 0.0;
            Int32 mjdeopstart, ktrActObs;
            string EOPupdate;
            double jdeopstart;
            char interp = 'x';  // full series


            nutLoc = @"D:\Codes\LIBRARY\DataLib\";
            EOPSPWLibr.iau00in(nutLoc, out iau00arr);

            // read interpolated one
            //EOPSPWLibr.initEOPArrayP(ref EOPSPWLibr.eopdataP);

            // read existing data - this does not find x, y, s!
            //getCurrEOPFileName(this.EOPSPWLoc.Text, out eopFileName);
            string eopFileName = @"D:\Codes\LIBRARY\DataLib\EOP-All-v1.1_2018-01-04.txt";
            EOPSPWLibr.readeop(ref EOPSPWLibr.eopdata, eopFileName, out mjdeopstart, out ktrActObs, out EOPupdate);
            jdeopstart = mjdeopstart + 2400000.5;

            // now find table of CIO values
            double deltapsi, deltaeps, tempval;

            // rad to "
            double convrt = (180.0 * 3600.0) / Math.PI;
            strbuild.AppendLine("CIO tests      x                   y                     s          ddpsi            ddeps      mjd ");
            for (i = 0; i < 14; i++)   // 14500
            {
                MathTimeLibr.jday(year, mon, day + i, hr, minute, second, out jd, out jdFrac);
                //EOPSPWLibr.findeopparam(jd, jdFrac, 's', EOPSPWLibr.eopdata, mjdeopstart + 2400000.5, out dut1, out dat,
                //   out lod, out xp, out yp, out ddpsi, out ddeps, out ddx, out ddy);
                jdtt = jd;
                jdftt = jdFrac + (dat + 32.184) / 86400.0;
                ttt = (jdtt + jdftt - 2451545.0) / 36525.0;

                AstroLibr.fundarg(ttt, AstroLib.EOpt.e80, out fArgs);

                ddpsi = 0.0;
                ddeps = 0.0;
                deltapsi = 0.0;
                deltaeps = 0.0;
                int ii;
                for (ii = 105; ii >= 0; ii--)
                {
                    tempval = iau80arr.iar80[ii, 0] * fArgs[0] + iau80arr.iar80[ii, 1] * fArgs[1] + iau80arr.iar80[ii, 2] * fArgs[2] +
                             iau80arr.iar80[ii, 3] * fArgs[3] + iau80arr.iar80[ii, 4] * fArgs[4];
                    deltapsi = deltapsi + (iau80arr.rar80[ii, 0] + iau80arr.rar80[ii, 1] * ttt) * Math.Sin(tempval);
                    deltaeps = deltaeps + (iau80arr.rar80[ii, 2] + iau80arr.rar80[ii, 3] * ttt) * Math.Cos(tempval);
                }

                // --------------- find nutation parameters --------------------
                deltapsi = (deltapsi + ddpsi) % (2.0 * Math.PI);
                deltaeps = (deltaeps + ddeps) % (2.0 * Math.PI);

                // CIO parameters
                AstroLibr.fundarg(ttt, AstroLib.EOpt.e06cio, out fArgs);
                ddx = 0.0;
                ddy = 0.0;
                AstroLibr.iau06xys(jdtt, jdftt, ddx, ddy, interp, iau00arr, fArgs, jdxysstart, out x, out y, out s);
                x = x * convrt;
                y = y * convrt;
                s = s * convrt;
                deltapsi = deltapsi * convrt;
                deltaeps = deltaeps * convrt;

                strbuild.AppendLine(" " + x.ToString(fmt).PadLeft(4) + " " + y.ToString(fmt).PadLeft(4) + " " + s.ToString(fmt).PadLeft(4) + " " +
                    deltapsi.ToString(fmt).PadLeft(4) + " " + deltaeps.ToString(fmt).PadLeft(4) + " " + (jd + jdFrac - 2400000.5).ToString(fmt).PadLeft(4));
            }

        }
        public void testtod2ecef()
        {
            double[] fArgs = new double[14];
            double[] reci = new double[3];
            double[] veci = new double[3];
            double[] recii = new double[3];
            double[] vecii = new double[3];
            double[] rmod = new double[3];
            double[] vmod = new double[3];
            double[] rtod = new double[3];
            double[] vtod = new double[3];
            double[] rpef = new double[3];
            double[] vpef = new double[3];
            double[] recef = new double[3];
            double[] vecef = new double[3];
            double[] recefi = new double[3];
            double[] vecefi = new double[3];
            double[] rtemp = new double[3];
            double[] vtemp = new double[3];
            double conv;
            Int32 year, mon, day, hr, minute, dat;
            double jd, jdFrac, jdut1, second, dut1, ttt, lod, xp, yp, ddx, ddy, ddpsi, ddeps,
                jdtt, jdftt, jdxysstart;

            conv = Math.PI / (180.0 * 3600.0);

            recef = new double[] { -1033.4793830, 7901.2952754, 6380.3565958 };
            vecef = new double[] { -3.225636520, -2.872451450, 5.531924446 };
            year = 2004;
            mon = 4;
            day = 6;
            hr = 7;
            minute = 51;
            second = 28.386009;
            MathTimeLibr.jday(year, mon, day, hr, minute, second, out jd, out jdFrac);

            dut1 = -0.4399619;      // sec
            dat = 32;               // sec
            xp = -0.140682 * conv;  // " to rad
            yp = 0.333309 * conv;
            lod = 0.0015563;
            ddpsi = -0.052195 * conv;  // " to rad
            ddeps = -0.003875 * conv;
            ddx = -0.000205 * conv;    // " to rad
            ddy = -0.000136 * conv;

            reci = new double[] { 0.0, 0.0, 0.0 };
            veci = new double[] { 0.0, 0.0, 0.0 };

            string nutLoc;
            EOPSPWLib.iau80Class iau80arr;
            EOPSPWLib.iau00Class iau00arr;
            nutLoc = @"D:\Codes\LIBRARY\DataLib\nut80.dat";
            EOPSPWLibr.iau80in(nutLoc, out iau80arr);
            nutLoc = @"D:\Codes\LIBRARY\DataLib\";
            EOPSPWLibr.iau00in(nutLoc, out iau00arr);
            jdtt = jd;
            jdftt = jdFrac + (dat + 32.184) / 86400.0;
            ttt = (jdtt + jdftt - 2451545.0) / 36525.0;

            // now read it in
            double jdfxysstart;
            AstroLib.xysdataClass[] xysarr = AstroLibr.xysarr;
            AstroLibr.initXYS(ref xysarr, nutLoc, "xysdata.dat", out jdxysstart, out jdfxysstart);

            jdut1 = jd + jdFrac + dut1 / 86400.0;

            strbuild.AppendLine("ITRF start    IAU-76/FK5   " + recef[0].ToString(fmt).PadLeft(4) + " " + recef[1].ToString(fmt).PadLeft(4) + " " + recef[2].ToString(fmt).PadLeft(4) + " "
                + vecef[0].ToString(fmt).PadLeft(4) + " " + vecef[1].ToString(fmt).PadLeft(4) + " " + vecef[2].ToString(fmt).PadLeft(4));

            // PEF
            AstroLibr.ecef_pef(ref recef, ref vecef, MathTimeLib.Edirection.eto, ref rpef, ref vpef,
                AstroLib.EOpt.e80, ttt, xp, yp);
            strbuild.AppendLine("PEF           IAU-76/FK5   " + rpef[0].ToString(fmt).PadLeft(4) + " " +
                rpef[1].ToString(fmt).PadLeft(4) + " " + rpef[2].ToString(fmt).PadLeft(4) + " "
                + vpef[0].ToString(fmt).PadLeft(4) + " " + vpef[1].ToString(fmt).PadLeft(4) + " " + vpef[2].ToString(fmt).PadLeft(4));
            AstroLibr.ecef_pef(ref recii, ref vecii, MathTimeLib.Edirection.efrom, ref rpef, ref vpef,
                AstroLib.EOpt.e80, ttt, xp, yp);
            strbuild.AppendLine("ITRF  rev     IAU-76/FK5   " + recii[0].ToString(fmt).PadLeft(4) + " " + recii[1].ToString(fmt).PadLeft(4) + " " + recii[2].ToString(fmt).PadLeft(4) + " "
                + vecii[0].ToString(fmt).PadLeft(4) + " " + vecii[1].ToString(fmt).PadLeft(4) + " " + vecii[2].ToString(fmt).PadLeft(4));

            AstroLibr.ecef_pef(ref recef, ref vecef, MathTimeLib.Edirection.eto, ref rtemp, ref vtemp,
                AstroLib.EOpt.e06cio, ttt, xp, yp);
            strbuild.AppendLine("TIRS          IAU-2006 CIO " + rtemp[0].ToString(fmt).PadLeft(4) + " " +
                rtemp[1].ToString(fmt).PadLeft(4) + " " + rtemp[2].ToString(fmt).PadLeft(4) + " "
                + vtemp[0].ToString(fmt).PadLeft(4) + " " + vtemp[1].ToString(fmt).PadLeft(4) + " " + vtemp[2].ToString(fmt).PadLeft(4));
            AstroLibr.ecef_pef(ref recefi, ref vecefi, MathTimeLib.Edirection.efrom, ref rtemp, ref vtemp,
                AstroLib.EOpt.e06cio, ttt, xp, yp);
            strbuild.AppendLine("ITRF rev      IAU-2006 CIO " + recefi[0].ToString(fmt).PadLeft(4) + " " + recefi[1].ToString(fmt).PadLeft(4) + " " + recefi[2].ToString(fmt).PadLeft(4) + " "
                + vecefi[0].ToString(fmt).PadLeft(4) + " " + vecefi[1].ToString(fmt).PadLeft(4) + " " + vecefi[2].ToString(fmt).PadLeft(4));

            // TOD
            AstroLibr.ecef_tod(ref recef, ref vecef, MathTimeLib.Edirection.eto, ref rtod, ref vtod,
                AstroLib.EOpt.e80, iau80arr, iau00arr,
                ttt, jdut1, lod, xp, yp, 0.0, 0.0, ddx, ddy);
            strbuild.AppendLine("TOD wo corr   IAU-76/FK5   " + rtod[0].ToString(fmt).PadLeft(4) + " " + rtod[1].ToString(fmt).PadLeft(4) + " " + rtod[2].ToString(fmt).PadLeft(4) + " "
                + vtod[0].ToString(fmt).PadLeft(4) + " " + vtod[1].ToString(fmt).PadLeft(4) + " " + vtod[2].ToString(fmt).PadLeft(4));
            AstroLibr.ecef_tod(ref recef, ref vecef, MathTimeLib.Edirection.eto, ref rtod, ref vtod,
                AstroLib.EOpt.e80, iau80arr, iau00arr,
                ttt, jdut1, lod, xp, yp, ddpsi, ddeps, ddx, ddy);
            strbuild.AppendLine("TOD w corr    IAU-76/FK5   " + rtod[0].ToString(fmt).PadLeft(4) + " " + rtod[1].ToString(fmt).PadLeft(4) + " " + rtod[2].ToString(fmt).PadLeft(4) + " "
                + vtod[0].ToString(fmt).PadLeft(4) + " " + vtod[1].ToString(fmt).PadLeft(4) + " " + vtod[2].ToString(fmt).PadLeft(4));
            AstroLibr.ecef_tod(ref recefi, ref vecefi, MathTimeLib.Edirection.efrom, ref rtod, ref vtod,
                AstroLib.EOpt.e80, iau80arr, iau00arr,
                ttt, jdut1, lod, xp, yp, ddpsi, ddeps, ddx, ddy);
            strbuild.AppendLine("ITRFi         IAU-76/FK5   " + recefi[0].ToString(fmt).PadLeft(4) + " " + recefi[1].ToString(fmt).PadLeft(4) + " " + recefi[2].ToString(fmt).PadLeft(4) + " "
                + vecefi[0].ToString(fmt).PadLeft(4) + " " + vecefi[1].ToString(fmt).PadLeft(4) + " " + vecefi[2].ToString(fmt).PadLeft(4));

            AstroLibr.ecef_tod(ref recef, ref vecef, MathTimeLib.Edirection.eto, ref rtemp, ref vtemp,
                AstroLib.EOpt.e06cio, iau80arr, iau00arr,
                ttt, jdut1, lod, xp, yp, ddpsi, ddeps, ddx, ddy);
            strbuild.AppendLine("CIRS          IAU-2006 CIO " + rtemp[0].ToString(fmt).PadLeft(4) + " " +
                rtemp[1].ToString(fmt).PadLeft(4) + " " + rtemp[2].ToString(fmt).PadLeft(4) + " "
                + vtemp[0].ToString(fmt).PadLeft(4) + " " + vtemp[1].ToString(fmt).PadLeft(4) + " " + vtemp[2].ToString(fmt).PadLeft(4));
            AstroLibr.ecef_tod(ref recefi, ref vecefi, MathTimeLib.Edirection.efrom, ref rtemp, ref vtemp,
               AstroLib.EOpt.e06cio, iau80arr, iau00arr,
               ttt, jdut1, lod, xp, yp, ddpsi, ddeps, ddx, ddy);
            strbuild.AppendLine("ITRF rev      IAU-2006 CIO " + recefi[0].ToString(fmt).PadLeft(4) + " " + recefi[1].ToString(fmt).PadLeft(4) + " " + recefi[2].ToString(fmt).PadLeft(4) + " "
                + vecefi[0].ToString(fmt).PadLeft(4) + " " + vecefi[1].ToString(fmt).PadLeft(4) + " " + vecefi[2].ToString(fmt).PadLeft(4));

            // MOD
            AstroLibr.ecef_mod(ref recef, ref vecef, MathTimeLib.Edirection.eto, ref rmod, ref vmod,
                AstroLib.EOpt.e80, iau80arr, iau00arr,
                jdtt, jdftt, jdut1, jdxysstart, lod, xp, yp, 0.0, 0.0, ddx, ddy);
            strbuild.AppendLine("MOD wo corr   IAU-76/FK5   " + rmod[0].ToString(fmt).PadLeft(4) + " " +
                rmod[1].ToString(fmt).PadLeft(4) + " " + rmod[2].ToString(fmt).PadLeft(4) + " "
                + vmod[0].ToString(fmt).PadLeft(4) + " " + vmod[1].ToString(fmt).PadLeft(4) + " " + vmod[2].ToString(fmt).PadLeft(4));
            AstroLibr.ecef_mod(ref recef, ref vecef, MathTimeLib.Edirection.eto, ref rmod, ref vmod,
              AstroLib.EOpt.e80, iau80arr, iau00arr,
              jdtt, jdftt, jdut1, jdxysstart, lod, xp, yp, ddpsi, ddeps, ddx, ddy);
            strbuild.AppendLine("MOD  w corr   IAU-76/FK5   " + rmod[0].ToString(fmt).PadLeft(4) + " " +
                rmod[1].ToString(fmt).PadLeft(4) + " " + rmod[2].ToString(fmt).PadLeft(4) + " "
                + vmod[0].ToString(fmt).PadLeft(4) + " " + vmod[1].ToString(fmt).PadLeft(4) + " " + vmod[2].ToString(fmt).PadLeft(4));
            AstroLibr.ecef_mod(ref recefi, ref vecefi, MathTimeLib.Edirection.efrom, ref rmod, ref vmod,
                AstroLib.EOpt.e80, iau80arr, iau00arr,
                jdtt, jdftt, jdut1, jdxysstart, lod, xp, yp, ddpsi, ddeps, ddx, ddy);
            strbuild.AppendLine("ITRF  rev     IAU-76/FK5   " + recefi[0].ToString(fmt).PadLeft(4) + " " + recefi[1].ToString(fmt).PadLeft(4) + " " + recefi[2].ToString(fmt).PadLeft(4) + " "
                + vecefi[0].ToString(fmt).PadLeft(4) + " " + vecefi[1].ToString(fmt).PadLeft(4) + " " + vecefi[2].ToString(fmt).PadLeft(4));


            // J2000
            AstroLibr.eci_ecef(ref recii, ref vecii, MathTimeLib.Edirection.efrom, ref recef, ref vecef,
                 AstroLib.EOpt.e80, iau80arr, iau00arr,
                 jdtt, jdftt, jdut1, jdxysstart, lod, xp, yp, 0.0, 0.0, ddx, ddy);
            strbuild.AppendLine("J2000 wo corr IAU-76/FK5   " + recii[0].ToString(fmt).PadLeft(4) + " " + recii[1].ToString(fmt).PadLeft(4) + " " + recii[2].ToString(fmt).PadLeft(4) + " "
                + vecii[0].ToString(fmt).PadLeft(4) + " " + vecii[1].ToString(fmt).PadLeft(4) + " " + vecii[2].ToString(fmt).PadLeft(4));

            // GCRF
            AstroLibr.eci_ecef(ref reci, ref veci, MathTimeLib.Edirection.efrom, ref recef, ref vecef,
                 AstroLib.EOpt.e80, iau80arr, iau00arr,
                 jdtt, jdftt, jdut1, jdxysstart, lod, xp, yp, ddpsi, ddeps, ddx, ddy);
            strbuild.AppendLine("GCRF w corr   IAU-76/FK5   " + reci[0].ToString(fmt).PadLeft(4) + " " + reci[1].ToString(fmt).PadLeft(4) + " " + reci[2].ToString(fmt).PadLeft(4) + " "
                + veci[0].ToString(fmt).PadLeft(4) + " " + veci[1].ToString(fmt).PadLeft(4) + " " + veci[2].ToString(fmt).PadLeft(4));
            AstroLibr.eci_ecef(ref reci, ref veci, MathTimeLib.Edirection.eto, ref recefi, ref vecefi,
                 AstroLib.EOpt.e80, iau80arr, iau00arr,
                 jdtt, jdftt, jdut1, jdxysstart, lod, xp, yp, ddpsi, ddeps, ddx, ddy);
            strbuild.AppendLine("ITRF rev      IAU-76/FK5   " + recefi[0].ToString(fmt).PadLeft(4) + " " + recefi[1].ToString(fmt).PadLeft(4) + " " + recefi[2].ToString(fmt).PadLeft(4) + " "
                + vecef[0].ToString(fmt).PadLeft(4) + " " + vecefi[1].ToString(fmt).PadLeft(4) + " " + vecefi[2].ToString(fmt).PadLeft(4));

            AstroLibr.eci_ecef(ref recii, ref vecii, MathTimeLib.Edirection.efrom, ref recef, ref vecef,
                 AstroLib.EOpt.e06cio, iau80arr, iau00arr,
                 jdtt, jdftt, jdut1, jdxysstart, lod, xp, yp, ddpsi, ddeps, ddx, ddy);
            strbuild.AppendLine("GCRF          IAU-2006 CIO " + recii[0].ToString(fmt).PadLeft(4) + " " + recii[1].ToString(fmt).PadLeft(4) + " " + recii[2].ToString(fmt).PadLeft(4) + " "
                + vecii[0].ToString(fmt).PadLeft(4) + " " + vecii[1].ToString(fmt).PadLeft(4) + " " + vecii[2].ToString(fmt).PadLeft(4));
            AstroLibr.eci_ecef(ref recii, ref vecii, MathTimeLib.Edirection.eto, ref recefi, ref vecefi,
                 AstroLib.EOpt.e06cio, iau80arr, iau00arr,
                 jdtt, jdftt, jdut1, jdxysstart, lod, xp, yp, ddpsi, ddeps, ddx, ddy);
            strbuild.AppendLine("ITRF rev      IAU-2006 CIO " + recefi[0].ToString(fmt).PadLeft(4) + " " + recefi[1].ToString(fmt).PadLeft(4) + " " + recefi[2].ToString(fmt).PadLeft(4) + " "
                + vecefi[0].ToString(fmt).PadLeft(4) + " " + vecefi[1].ToString(fmt).PadLeft(4) + " " + vecefi[2].ToString(fmt).PadLeft(4));

            // sofa
            strbuild.AppendLine("SOFA ECI CIO  5102.508959486507   6123.011392959787   6378.136934384333");
            strbuild.AppendLine("SOFA ECI 06a  5102.508965811828   6123.011397147659   6378.136925303720");
            strbuild.AppendLine("SOFA ECI 00a  5102.508965732931   6123.011397847143   6378.136924695331");


            // now reverses from eci
            strbuild.AppendLine("GCRF wco STARTIAU-76/FK5   " + reci[0].ToString(fmt).PadLeft(4) + " " + reci[1].ToString(fmt).PadLeft(4) + " " + reci[2].ToString(fmt).PadLeft(4) + " "
                + veci[0].ToString(fmt).PadLeft(4) + " " + veci[1].ToString(fmt).PadLeft(4) + " " + veci[2].ToString(fmt).PadLeft(4));

           // PEF
           AstroLibr.eci_pef(ref reci, ref veci, MathTimeLib.Edirection.eto, ref rpef, ref vpef,
                AstroLib.EOpt.e80, iau80arr, iau00arr,
                jdtt, jdftt, jdut1, jdxysstart, lod, ddpsi, ddeps, ddx, ddy);
            strbuild.AppendLine("PEF           IAU-76/FK5   " + rpef[0].ToString(fmt).PadLeft(4) + " " +
                rpef[1].ToString(fmt).PadLeft(4) + " " + rpef[2].ToString(fmt).PadLeft(4) + " "
                + vpef[0].ToString(fmt).PadLeft(4) + " " + vpef[1].ToString(fmt).PadLeft(4) + " " + vpef[2].ToString(fmt).PadLeft(4));
            AstroLibr.eci_pef(ref recii, ref vecii, MathTimeLib.Edirection.efrom, ref rpef, ref vpef,
                AstroLib.EOpt.e80, iau80arr, iau00arr,
                jdtt, jdftt, jdut1, jdxysstart, lod, ddpsi, ddeps, ddx, ddy);
            strbuild.AppendLine("ECI rev       IAU-76/FK5   " + recii[0].ToString(fmt).PadLeft(4) + " " + recii[1].ToString(fmt).PadLeft(4) + " " + recii[2].ToString(fmt).PadLeft(4) + " "
                + vecii[0].ToString(fmt).PadLeft(4) + " " + vecii[1].ToString(fmt).PadLeft(4) + " " + vecii[2].ToString(fmt).PadLeft(4));

            AstroLibr.eci_pef(ref reci, ref veci, MathTimeLib.Edirection.eto, ref rpef, ref vpef,
                AstroLib.EOpt.e06cio, iau80arr, iau00arr,
                jdtt, jdftt, jdut1, jdxysstart, lod, ddpsi, ddeps, ddx, ddy);
            strbuild.AppendLine("TIRS          IAU-2006 CIO  " + rpef[0].ToString(fmt).PadLeft(4) + " " +
                rpef[1].ToString(fmt).PadLeft(4) + " " + rpef[2].ToString(fmt).PadLeft(4) + " "
                + vpef[0].ToString(fmt).PadLeft(4) + " " + vpef[1].ToString(fmt).PadLeft(4) + " " + vpef[2].ToString(fmt).PadLeft(4));
            AstroLibr.eci_pef(ref recii, ref vecii, MathTimeLib.Edirection.efrom, ref rpef, ref vpef,
                AstroLib.EOpt.e06cio, iau80arr, iau00arr,
                jdtt, jdftt, jdut1, jdxysstart, lod, ddpsi, ddeps, ddx, ddy);
            strbuild.AppendLine("ECI rev       IAU-2006 CIO " + recii[0].ToString(fmt).PadLeft(4) + " " + recii[1].ToString(fmt).PadLeft(4) + " " + recii[2].ToString(fmt).PadLeft(4) + " "
                + vecii[0].ToString(fmt).PadLeft(4) + " " + vecii[1].ToString(fmt).PadLeft(4) + " " + vecii[2].ToString(fmt).PadLeft(4));

            // TOD
            AstroLibr.eci_tod(ref reci, ref veci, MathTimeLib.Edirection.eto, ref rtod, ref vtod,
                AstroLib.EOpt.e80, iau80arr, iau00arr,
                jdtt, jdftt, jdut1, jdxysstart, lod, ddpsi, ddeps, ddx, ddy);
            strbuild.AppendLine("TOD           IAU-76/FK5   " + rtod[0].ToString(fmt).PadLeft(4) + " " + rtod[1].ToString(fmt).PadLeft(4) + " " + rtod[2].ToString(fmt).PadLeft(4) + " "
                + vtod[0].ToString(fmt).PadLeft(4) + " " + vtod[1].ToString(fmt).PadLeft(4) + " " + vtod[2].ToString(fmt).PadLeft(4));
            AstroLibr.eci_tod(ref recii, ref vecii, MathTimeLib.Edirection.efrom, ref rtod, ref vtod,
                AstroLib.EOpt.e80, iau80arr, iau00arr,
                jdtt, jdftt, jdut1, jdxysstart, lod, ddpsi, ddeps, ddx, ddy);
            strbuild.AppendLine("ECI rev       IAU-76/FK5   " + recii[0].ToString(fmt).PadLeft(4) + " " + recii[1].ToString(fmt).PadLeft(4) + " " + recii[2].ToString(fmt).PadLeft(4) + " "
                + vecii[0].ToString(fmt).PadLeft(4) + " " + vecii[1].ToString(fmt).PadLeft(4) + " " + vecii[2].ToString(fmt).PadLeft(4));

            AstroLibr.eci_tod(ref reci, ref veci, MathTimeLib.Edirection.eto, ref rtod, ref vtod,
                AstroLib.EOpt.e06cio, iau80arr, iau00arr,
                jdtt, jdftt, jdut1, jdxysstart, lod, ddpsi, ddeps, ddx, ddy);
            strbuild.AppendLine("CIRS          IAU-2006 CIO " + rtod[0].ToString(fmt).PadLeft(4) + " " + rtod[1].ToString(fmt).PadLeft(4) + " " + rtod[2].ToString(fmt).PadLeft(4) + " "
                + vtod[0].ToString(fmt).PadLeft(4) + " " + vtod[1].ToString(fmt).PadLeft(4) + " " + vtod[2].ToString(fmt).PadLeft(4));
            AstroLibr.eci_tod(ref recii, ref vecii, MathTimeLib.Edirection.efrom, ref rtod, ref vtod, 
                AstroLib.EOpt.e06cio, iau80arr, iau00arr,
                jdtt, jdftt, jdut1, jdxysstart, lod, ddpsi, ddeps, ddx, ddy);
            strbuild.AppendLine("ECI rev       IAU-2006 CIO " + recii[0].ToString(fmt).PadLeft(4) + " " + recii[1].ToString(fmt).PadLeft(4) + " " + recii[2].ToString(fmt).PadLeft(4) + " "
                + vecii[0].ToString(fmt).PadLeft(4) + " " + vecii[1].ToString(fmt).PadLeft(4) + " " + vecii[2].ToString(fmt).PadLeft(4));

            // MOD
            AstroLibr.eci_mod(ref reci, ref veci, MathTimeLib.Edirection.eto, ref rmod, ref vmod,
                AstroLib.EOpt.e80, iau80arr, iau00arr, ttt);
            strbuild.AppendLine("MOD           IAU-76/FK5   " + rmod[0].ToString(fmt).PadLeft(4) + " " +
                rmod[1].ToString(fmt).PadLeft(4) + " " + rmod[2].ToString(fmt).PadLeft(4) + " "
                + vmod[0].ToString(fmt).PadLeft(4) + " " + vmod[1].ToString(fmt).PadLeft(4) + " " + vmod[2].ToString(fmt).PadLeft(4));
            AstroLibr.eci_mod(ref recii, ref vecii, MathTimeLib.Edirection.efrom, ref rmod, ref vmod,
                AstroLib.EOpt.e80, iau80arr, iau00arr, ttt);
            strbuild.AppendLine("ECI rev       IAU-76/FK5   " + recii[0].ToString(fmt).PadLeft(4) + " " + recii[1].ToString(fmt).PadLeft(4) + " " + recii[2].ToString(fmt).PadLeft(4) + " "
                + vecii[0].ToString(fmt).PadLeft(4) + " " + vecii[1].ToString(fmt).PadLeft(4) + " " + vecii[2].ToString(fmt).PadLeft(4));
        }
        public void testteme_ecef()
        {
            double[] fArgs = new double[14];
            double[] rteme = new double[3];
            double[] vteme = new double[3];
            int eqeterms = 2;
            double[] recef = new double[3];
            double[] vecef = new double[3];
            double conv;
            Int32 year, mon, day, hr, minute, dat;
            double jd, jdFrac, jdut1, second, dut1, ttt, lod, xp, yp, ddx, ddy, ddpsi, ddeps;
            EOPSPWLib.iau80Class iau80arr;

            conv = Math.PI / (180.0 * 3600.0);

            recef = new double[] { -1033.4793830, 7901.2952754, 6380.3565958 };
            vecef = new double[] { -3.225636520, -2.872451450, 5.531924446 };
            year = 2004;
            mon = 4;
            day = 6;
            hr = 7;
            minute = 51;
            second = 28.386009;
            MathTimeLibr.jday(year, mon, day, hr, minute, second, out jd, out jdFrac);

            dut1 = -0.4399619;      // sec
            dat = 32;               // sec
            xp = -0.140682 * conv;  // " to rad
            yp = 0.333309 * conv;
            lod = 0.0015563;
            ddpsi = -0.052195 * conv;  // " to rad
            ddeps = -0.003875 * conv;
            ddx = -0.000205 * conv;    // " to rad
            ddy = -0.000136 * conv;

            string nutLoc = @"D:\Codes\LIBRARY\DataLib\nut80.dat";
            EOPSPWLibr.iau80in(nutLoc, out iau80arr);

            // note you have to use tdb for time of ineterst AND j2000 (when dat = 32)
            ttt = (jd + jdFrac + (dat + 32.184) / 86400.0 - 2451545.0 - (32 + 32.184) / 86400.0) / 36525.0;
            jdut1 = jd + jdFrac + dut1 / 86400.0;

            strbuild.AppendLine("ITRF          IAU-76/FK5   " + recef[0].ToString(fmt).PadLeft(4) + " " + recef[1].ToString(fmt).PadLeft(4) + " " + recef[2].ToString(fmt).PadLeft(4) + " "
                + vecef[0].ToString(fmt).PadLeft(4) + " " + vecef[1].ToString(fmt).PadLeft(4) + " " + vecef[2].ToString(fmt).PadLeft(4));

            AstroLibr.teme_ecef(ref rteme, ref vteme, MathTimeLib.Edirection.efrom, ttt, jdut1, lod, xp, yp,
                eqeterms, AstroLib.EOpt.e80, ref recef, ref vecef);
            strbuild.AppendLine("TEME          IAU-76/FK5   " + rteme[0].ToString(fmt).PadLeft(4) + " " + rteme[1].ToString(fmt).PadLeft(4) + " " + rteme[2].ToString(fmt).PadLeft(4) + " "
                + vteme[0].ToString(fmt).PadLeft(4) + " " + vteme[1].ToString(fmt).PadLeft(4) + " " + vteme[2].ToString(fmt).PadLeft(4));

            recef = new double[] { 0.0, 0.0, 0.0 };
            vecef = new double[] { 0.0, 0.0, 0.0 };
            AstroLibr.teme_ecef(ref rteme, ref vteme, MathTimeLib.Edirection.eto, ttt, jdut1, lod, xp, yp,
                eqeterms, AstroLib.EOpt.e80, ref recef, ref vecef);
            strbuild.AppendLine("ITRF          IAU-76/FK5   " + recef[0].ToString(fmt).PadLeft(4) + " " + recef[1].ToString(fmt).PadLeft(4) + " " + recef[2].ToString(fmt).PadLeft(4) + " "
                + vecef[0].ToString(fmt).PadLeft(4) + " " + vecef[1].ToString(fmt).PadLeft(4) + " " + vecef[2].ToString(fmt).PadLeft(4));

        }
        public void testteme_eci()
        {
            double[] fArgs = new double[14];
            double[] rteme = new double[3];
            double[] vteme = new double[3];
            double[] reci = new double[3];
            double[] veci = new double[3];
            double conv;
            Int32 year, mon, day, hr, minute, dat;
            double jd, jdFrac, jdut1, second, dut1, ttt, xp, yp, ddx, ddy, ddpsi, ddeps;
            EOPSPWLib.iau80Class iau80arr;

            conv = Math.PI / (180.0 * 3600.0);

            reci = new double[] { 5102.5089579, 6123.0114007, 6378.1369282 };
            veci = new double[] { -4.743220157, 0.790536497, 5.533755727 };
            year = 2004;
            mon = 4;
            day = 6;
            hr = 7;
            minute = 51;
            second = 28.386009;
            MathTimeLibr.jday(year, mon, day, hr, minute, second, out jd, out jdFrac);

            dut1 = -0.4399619;      // sec
            dat = 32;               // sec
            xp = -0.140682 * conv;  // " to rad
            yp = 0.333309 * conv;
            ddpsi = -0.052195 * conv;  // " to rad
            ddeps = -0.003875 * conv;
            ddx = -0.000205 * conv;    // " to rad
            ddy = -0.000136 * conv;

            string nutLoc = @"D:\Codes\LIBRARY\DataLib\nut80.dat";
            EOPSPWLibr.iau80in(nutLoc, out iau80arr);

            // note you have to use tdb for time of ineterst AND j2000 (when dat = 32)
            ttt = (jd + jdFrac + (dat + 32.184) / 86400.0 - 2451545.0 - (32 + 32.184) / 86400.0) / 36525.0;
            jdut1 = jd + jdFrac + dut1 / 86400.0;

            strbuild.AppendLine("GCRF          IAU-76/FK5   " + reci[0].ToString(fmt).PadLeft(4) + " " + reci[1].ToString(fmt).PadLeft(4) + " " + reci[2].ToString(fmt).PadLeft(4) + " "
                + veci[0].ToString(fmt).PadLeft(4) + " " + veci[1].ToString(fmt).PadLeft(4) + " " + veci[2].ToString(fmt).PadLeft(4));

            AstroLibr.teme_eci(ref rteme, ref vteme, iau80arr, MathTimeLib.Edirection.efrom, ttt, ddpsi, ddeps, AstroLib.EOpt.e80, ref reci, ref veci);
            strbuild.AppendLine("TEME          IAU-76/FK5   " + rteme[0].ToString(fmt).PadLeft(4) + " " + rteme[1].ToString(fmt).PadLeft(4) + " " + rteme[2].ToString(fmt).PadLeft(4) + " "
                + vteme[0].ToString(fmt).PadLeft(4) + " " + vteme[1].ToString(fmt).PadLeft(4) + " " + vteme[2].ToString(fmt).PadLeft(4));

            AstroLibr.teme_eci(ref rteme, ref vteme, iau80arr, MathTimeLib.Edirection.eto, ttt, ddpsi, ddeps, AstroLib.EOpt.e80, ref reci, ref veci);
            strbuild.AppendLine("GCRF          IAU-76/FK5   " + reci[0].ToString(fmt).PadLeft(4) + " " + reci[1].ToString(fmt).PadLeft(4) + " " + reci[2].ToString(fmt).PadLeft(4) + " "
                + veci[0].ToString(fmt).PadLeft(4) + " " + veci[1].ToString(fmt).PadLeft(4) + " " + veci[2].ToString(fmt).PadLeft(4));
        }
        public void testqmod2ecef()
        {
            double[] fArgs = new double[14];
            double[] rqmod = new double[3];
            double[] vqmod = new double[3];
            double ttt, jdutc;
            AstroLib.EOpt opt = AstroLib.EOpt.e80;
            double[] recef = new double[3];
            double[] vecef = new double[3];
            EOPSPWLib.iau80Class iau80arr;

            ttt = 0.042623631889;
            jdutc = 2453101.82740678310;

            AstroLibr.fundarg(ttt, opt, out fArgs);

            string nutLoc = @"D:\Codes\LIBRARY\DataLib\nut80.dat";
            EOPSPWLibr.iau80in(nutLoc, out iau80arr);

            AstroLibr.qmod2ecef(rqmod, vqmod, ttt, jdutc, iau80arr, opt, out recef, out vecef);
        }
        public void testcsm2efg()
        {
            double[] fArgs = new double[14];
            double[] r1pef = new double[3];
            double[] v1pef = new double[3];
            double[] r1ecef = new double[3];
            double[] v1ecef = new double[3];
            double[] r2ecef = new double[3];
            double[] v2ecef = new double[3];
            double[] r2ric = new double[3];
            double[] v2ric = new double[3];
            double ttt, lod, xp, yp, jdut1, ddpsi, ddeps;
            int eqeterms;
            xp = 0.0;
            yp = 0.0;
            lod = 0.0;
            jdut1 = 2453101.82740678310;
            ttt = 0.042623631889;
            ddpsi = -0.052195;
            ddeps = -0.003875;
            eqeterms = 2;

            AstroLibr.csm2efg(r1pef, v1pef, r2ric, v2ric, ttt, jdut1, lod, xp, yp, eqeterms, ddpsi, ddeps, AstroLib.EOpt.e80,
                out r1ecef, out v1ecef, out r2ecef, out v2ecef);
        }

        public void testrv_elatlon()
        {
            double rr, ecllat, ecllon, drr, decllat, decllon, rad;
            double[] rijk = new double[3];
            double[] vijk = new double[3];
            rad = 180.0 / Math.PI;
            rr = 12756.00;
            ecllat = 60.04570;
            ecllon = 256.004567345;
            drr = 0.045670;
            decllat = 6.798614;
            decllon = 0.00768;

            AstroLibr.rv_elatlon(ref rijk, ref vijk, MathTimeLib.Edirection.efrom, ref rr, ref ecllat, ref ecllon, ref drr, ref decllat, ref decllon);
            strbuild.AppendLine("rv ecllat " + rijk[0].ToString(fmt).PadLeft(4) + " " + rijk[1].ToString(fmt).PadLeft(4) + " " + rijk[2].ToString(fmt).PadLeft(4) + " " +
                                " " + vijk[0].ToString(fmt).PadLeft(4) + " " + vijk[1].ToString(fmt).PadLeft(4) + " " + vijk[2].ToString(fmt).PadLeft(4));

            AstroLibr.rv_elatlon(ref rijk, ref vijk, MathTimeLib.Edirection.eto, ref rr, ref ecllat, ref ecllon, ref drr, ref decllat, ref decllon);

            strbuild.AppendLine("ecllat  " + rr.ToString(fmt).PadLeft(4) + " " + (ecllat * rad).ToString(fmt).PadLeft(4) + " " + (ecllon * rad).ToString(fmt).PadLeft(4) + " " +
                              " " + drr.ToString(fmt).PadLeft(4) + " " + decllat.ToString(fmt).PadLeft(4) + " " + decllon.ToString(fmt).PadLeft(4));
        }

        public void testrv2radec()
        {
            double[] r = new double[3];
            double[] v = new double[3];
            double rr, rtasc, decl, drr, drtasc, ddecl;
            r = new double[] { -605.79221660, -5870.22951108, 3493.05319896 };
            v = new double[] { -1.56825429, -3.70234891, -6.47948395 };
            rr = rtasc = decl = drr = drtasc = ddecl = 0.0;

            AstroLibr.rv_radec(ref r, ref v, MathTimeLib.Edirection.eto, ref rr, ref rtasc, ref decl, ref drr, ref drtasc, ref ddecl);
            strbuild.AppendLine("rv radec " + r[0].ToString(fmt).PadLeft(4) + " " + r[1].ToString(fmt).PadLeft(4) + " " + r[2].ToString(fmt).PadLeft(4) + " " +
                                " " + v[0].ToString(fmt).PadLeft(4) + " " + v[1].ToString(fmt).PadLeft(4) + " " + v[2].ToString(fmt).PadLeft(4));
            strbuild.AppendLine("radec " + rr.ToString(fmt).PadLeft(4) + " " + rtasc.ToString(fmt).PadLeft(4) + " " + decl.ToString(fmt).PadLeft(4) + " " +
                                "  " + drr.ToString(fmt).PadLeft(4) + " " + drtasc.ToString(fmt).PadLeft(4) + " " + ddecl.ToString(fmt).PadLeft(4));

            AstroLibr.rv_radec(ref r, ref v, MathTimeLib.Edirection.efrom, ref rr, ref rtasc, ref decl, ref drr, ref drtasc, ref ddecl);

            strbuild.AppendLine("rv radec  " + r[0].ToString(fmt).PadLeft(4) + " " + r[1].ToString(fmt).PadLeft(4) + " " + r[2].ToString(fmt).PadLeft(4) + " " +
                                "  " + v[0].ToString(fmt).PadLeft(4) + " " + v[1].ToString(fmt).PadLeft(4) + " " + v[2].ToString(fmt).PadLeft(4));
        }
        public void testrv_razel()
        {
            double rad = 180.0 / Math.PI;
            double[] recef = new double[3];
            double[] vecef = new double[3];
            double[] rsecef = new double[3];
            double rho, az, el, drho, daz, del, latgd, lon, alt;
            recef = new double[] { -605.79221660, -5870.22951108, 3493.05319896 };
            vecef = new double[] { -1.56825429, -3.70234891, -6.47948395 };
            //rsecef = new double[] { -1605.79221660, -570.22951108, 193.05319896 };
            lon = -104.883 / rad;
            latgd = 39.883 / rad;
            alt = 2.102;
            rho = 0.0186569;
            az = -0.3501725;
            el = -0.5839385;
            drho = 0.6811410;
            daz = -0.4806057;
            del = 0.6284403;

            AstroLibr.rv_razel(ref recef, ref vecef, latgd, lon, alt, MathTimeLib.Edirection.eto, ref rho, ref az, ref el, ref drho, ref daz, ref del);
            strbuild.AppendLine("rv_razel  " + recef[0].ToString(fmt).PadLeft(4) + " " + recef[1].ToString(fmt).PadLeft(4) + " " + recef[2].ToString(fmt).PadLeft(4) + " " +
                                "  " + vecef[0].ToString(fmt).PadLeft(4) + " " + vecef[1].ToString(fmt).PadLeft(4) + " " + vecef[2].ToString(fmt).PadLeft(4));
            strbuild.AppendLine("razel " + rho.ToString(fmt).PadLeft(4) + " " + az.ToString(fmt).PadLeft(4) + " " + el.ToString(fmt).PadLeft(4) + " " +
                                "  " + drho.ToString(fmt).PadLeft(4) + " " + daz.ToString(fmt).PadLeft(4) + " " + del.ToString(fmt).PadLeft(4));

            AstroLibr.rv_razel(ref recef, ref vecef, latgd, lon, alt, MathTimeLib.Edirection.efrom, ref rho, ref az, ref el, ref drho, ref daz, ref del);
            strbuild.AppendLine("rv_razel  " + recef[0].ToString(fmt).PadLeft(4) + " " + recef[1].ToString(fmt).PadLeft(4) + " " + recef[2].ToString(fmt).PadLeft(4) + " " +
                                "  " + vecef[0].ToString(fmt).PadLeft(4) + " " + vecef[1].ToString(fmt).PadLeft(4) + " " + vecef[2].ToString(fmt).PadLeft(4));
        }

        public void testrv_tradec()
        {
            double[] rijk = new double[3];
            double[] vijk = new double[3];
            double[] rsijk = new double[3];
            double rho, trtasc, tdecl, drho, dtrtasc, dtdecl;
            rijk = new double[] { 4066.716, -2847.545, 3994.302 };
            vijk = new double[] { -1.56825429, -3.70234891, -6.47948395 };
            rsijk = new double[] { -1605.79221660, -570.22951108, 193.05319896 };
            rho = 0.2634728;
            trtasc = -0.1492353;
            tdecl = 0.0519525;
            drho = 0.3072265;
            dtrtasc = 0.2045751;
            dtdecl = -0.7510033;

            AstroLibr.rv_tradec(ref rijk, ref vijk, rsijk, MathTimeLib.Edirection.eto, ref rho, ref trtasc, ref tdecl, ref drho, ref dtrtasc, ref dtdecl);
            strbuild.AppendLine("rv tradec  " + rijk[0].ToString(fmt).PadLeft(4) + " " + rijk[1].ToString(fmt).PadLeft(4) + " " + rijk[2].ToString(fmt).PadLeft(4) + " " +
                                "  " + vijk[0].ToString(fmt).PadLeft(4) + " " + vijk[1].ToString(fmt).PadLeft(4) + " " + vijk[2].ToString(fmt).PadLeft(4));
            strbuild.AppendLine("tradec " + rho.ToString(fmt).PadLeft(4) + " " + trtasc.ToString(fmt).PadLeft(4) + " " + tdecl.ToString(fmt).PadLeft(4) + " " +
                                "  " + drho.ToString(fmt).PadLeft(4) + " " + dtrtasc.ToString(fmt).PadLeft(4) + " " + dtdecl.ToString(fmt).PadLeft(4));

            AstroLibr.rv_tradec(ref rijk, ref vijk, rsijk, MathTimeLib.Edirection.efrom, ref rho, ref trtasc, ref tdecl, ref drho, ref dtrtasc, ref dtdecl);
            strbuild.AppendLine("rv tradec  " + rijk[0].ToString(fmt).PadLeft(4) + " " + rijk[1].ToString(fmt).PadLeft(4) + " " + rijk[2].ToString(fmt).PadLeft(4) + " " +
                                "  " + vijk[0].ToString(fmt).PadLeft(4) + " " + vijk[1].ToString(fmt).PadLeft(4) + " " + vijk[2].ToString(fmt).PadLeft(4));
        }
        public void testrvsez_razel()
        {
            double[] rhosez = new double[3];
            double[] drhosez = new double[3];
            double rho, az, el, drho, daz, del;
            rhosez = new double[] { -605.79221660, -5870.22951108, 3493.05319896 };
            drhosez = new double[] { -1.56825429, -3.70234891, -6.47948395 };
            rho = 0.0186569;
            az = -0.3501725;
            el = -0.5839385;
            drho = 0.6811410;
            daz = -0.4806057;
            del = 0.6284403;

            AstroLibr.rvsez_razel(ref rhosez, ref drhosez, MathTimeLib.Edirection.eto, ref rho, ref az, ref el, ref drho, ref daz, ref del);
            strbuild.AppendLine("rv rhosez  " + rhosez[0].ToString(fmt).PadLeft(4) + " " + rhosez[1].ToString(fmt).PadLeft(4) + " " + rhosez[2].ToString(fmt).PadLeft(4) + " " +
                             "  " + drhosez[0].ToString(fmt).PadLeft(4) + " " + drhosez[1].ToString(fmt).PadLeft(4) + " " + drhosez[2].ToString(fmt).PadLeft(4));
            strbuild.AppendLine("rhosez " + rho.ToString(fmt).PadLeft(4) + " " + az.ToString(fmt).PadLeft(4) + " " + el.ToString(fmt).PadLeft(4) + " " +
                                "  " + drho.ToString(fmt).PadLeft(4) + " " + daz.ToString(fmt).PadLeft(4) + " " + del.ToString(fmt).PadLeft(4));

            AstroLibr.rvsez_razel(ref rhosez, ref drhosez, MathTimeLib.Edirection.efrom, ref rho, ref az, ref el, ref drho, ref daz, ref del);

            strbuild.AppendLine("rv rhosez  " + rhosez[0].ToString(fmt).PadLeft(4) + " " + rhosez[1].ToString(fmt).PadLeft(4) + " " + rhosez[2].ToString(fmt).PadLeft(4) + " " +
                             "  " + drhosez[0].ToString(fmt).PadLeft(4) + " " + drhosez[1].ToString(fmt).PadLeft(4) + " " + drhosez[2].ToString(fmt).PadLeft(4));
        }
        public void testrv2rsw()
        {
            double[] r = new double[3];
            double[] v = new double[3];
            double[] rrsw = new double[3];
            double[] vrsw = new double[3];
            double[,] tm = new double[3, 3];
            r = new double[] { -605.79221660, -5870.22951108, 3493.05319896 };
            v = new double[] { -1.56825429, -3.70234891, -6.47948395 };

            tm = AstroLibr.rv2rsw(r, v, out rrsw, out vrsw);

            strbuild.AppendLine("rv2rsw " + rrsw[0].ToString(fmt) + " " + rrsw[1].ToString(fmt) + " " + rrsw[2].ToString(fmt) + " " +
                      vrsw[0].ToString(fmt) + " " + vrsw[1].ToString(fmt) + " " + vrsw[2].ToString(fmt));
        }

        public void testrv2pqw()
        {
            double[] r = new double[3];
            double[] v = new double[3];
            double[] rpqw = new double[3];
            double[] vpqw = new double[3];
            double[,] tm = new double[3, 3];
            r = new double[] { -605.79221660, -5870.22951108, 3493.05319896 };
            v = new double[] { -1.56825429, -3.70234891, -6.47948395 };

            AstroLibr.rv2pqw(r, v, out rpqw, out vpqw);

            strbuild.AppendLine("rv2pqw " + rpqw[0].ToString(fmt) + " " + rpqw[1].ToString(fmt) + " " + rpqw[2].ToString(fmt) + " " +
                  vpqw[0].ToString(fmt) + " " + vpqw[1].ToString(fmt) + " " + vpqw[2].ToString(fmt));
        }

        public void testrv2coe()
        {
            Int32 i;
            double[] r = new double[3];
            double[] v = new double[3];
            double[] r1 = new double[3];
            double[] v1 = new double[3];
            double rad = 180.0 / Math.PI;
            double p, a, ecc, incl, raan, argp, nu, m, arglat, truelon, lonper;

            for (i = 1; i <= 20; i++)
            {
                if (i == 1)
                {
                    r = new double[] { -605.79221660, -5870.22951108, 3493.05319896 };
                    v = new double[] { -1.56825429, -3.70234891, -6.47948395 };
                }
                if (i == 2)
                {
                    strbuild.AppendLine(" coe test ----------------------------");
                    r = new double[] { 6524.834, 6862.875, 6448.296 };
                    v = new double[] { 4.901327, 5.533756, -1.976341 };
                }

                // ------- elliptical orbit tests -------------------
                if (i == 3)
                {
                    strbuild.AppendLine(" coe test elliptical ----------------------------");
                    r = new double[] { 1.1372844 * AstroLibr.gravConst.re, -1.0534274 * AstroLibr.gravConst.re, -0.8550194 * AstroLibr.gravConst.re };
                    v = new double[] { 0.6510489 * AstroLibr.gravConst.velkmps, 0.4521008 * AstroLibr.gravConst.velkmps, 0.0381088 * AstroLibr.gravConst.velkmps };
                }
                if (i == 4)
                {
                    strbuild.AppendLine(" coe test elliptical ----------------------------");
                    r = new double[] { 1.056194 * AstroLibr.gravConst.re, -0.8950922 * AstroLibr.gravConst.re, -0.0823703 * AstroLibr.gravConst.re };
                    v = new double[] { -0.5981066 * AstroLibr.gravConst.velkmps, -0.6293575 * AstroLibr.gravConst.velkmps, 0.1468194 * AstroLibr.gravConst.velkmps };
                }

                // ------- circular inclined orbit tests -------------------
                if (i == 5)
                {
                    strbuild.AppendLine(" coe test near circular inclined ----------------------------");
                    r = new double[] { -0.422277 * AstroLibr.gravConst.re, 1.0078857 * AstroLibr.gravConst.re, 0.7041832 * AstroLibr.gravConst.re };
                    v = new double[] { -0.5002738 * AstroLibr.gravConst.velkmps, -0.5415267 * AstroLibr.gravConst.velkmps, 0.4750788 * AstroLibr.gravConst.velkmps };
                }
                if (i == 6)
                {
                    strbuild.AppendLine(" coe test near circular inclined ----------------------------");
                    r = new double[] { -0.7309361 * AstroLibr.gravConst.re, -0.6794646 * AstroLibr.gravConst.re, -0.8331183 * AstroLibr.gravConst.re };
                    v = new double[] { -0.6724131 * AstroLibr.gravConst.velkmps, 0.0341802 * AstroLibr.gravConst.velkmps, 0.5620652 * AstroLibr.gravConst.velkmps };
                }

                if (i == 7) // -- CI u = 45 deg
                {
                    strbuild.AppendLine(" coe test circular inclined ----------------------------");
                    r = new double[] { -2693.34555010128, 6428.43425355863, 4491.37782050409 };
                    v = new double[] { -3.95484712246016, -4.28096585381370, 3.75567104538731 };
                }
                if (i == 8) // -- CI u = 315 deg
                {
                    strbuild.AppendLine(" coe test circular inclined ----------------------------");
                    r = new double[] { -7079.68834483379, 3167.87718823353, -2931.53867301568 };
                    v = new double[] { 1.77608080328182, 6.23770933190509, 2.45134017949138 };
                }

                // ------- elliptical equatorial orbit tests -------------------
                if (i == 9)
                {
                    strbuild.AppendLine(" coe test elliptical near equatorial ----------------------------");
                    r = new double[] { 21648.6109280739, -14058.7723188698, -0.0003598029 };
                    v = new double[] { 2.16378060719980, 3.32694348486311, 0.00000004164788 };
                }
                if (i == 10)
                {
                    strbuild.AppendLine(" coe test elliptical near equatorial ----------------------------");
                    r = new double[] { 7546.9914487222, 24685.1032834356, -0.0003598029 };
                    v = new double[] { 3.79607016047138, -1.15773520476223, 0.00000004164788 };
                }

                if (i == 11) // -- EE w = 20 deg
                {
                    strbuild.AppendLine(" coe test elliptical equatorial ----------------------------");
                    r = new double[] { -22739.1086596208, -22739.1086596208, 0.0 };
                    v = new double[] { 2.48514004188565, -2.02004112073465, 0.0 };
                }
                if (i == 12) // -- EE w = 240 deg
                {
                    strbuild.AppendLine(" coe test elliptical equatorial ----------------------------");
                    r = new double[] { 28242.3662822040, 2470.8868808397, 0.0 };
                    v = new double[] { 0.66575215057746, -3.62533022188304, 0.0 };
                }

                // ------- circular equatorial orbit tests -------------------
                if (i == 13)
                {
                    strbuild.AppendLine(" coe test circular near equatorial ----------------------------");
                    r = new double[] { -2547.3697454933, 14446.8517254604, 0.000 };
                    v = new double[] { -5.13345156333487, -0.90516601477599, 0.00000090977789 };
                }
                if (i == 14)
                {
                    strbuild.AppendLine(" coe test circular near equatorial ----------------------------");
                    r = new double[] { 7334.858850000, -12704.3481945462, 0.000 };
                    v = new double[] { -4.51428154312046, -2.60632166411836, 0.00000090977789 };
                }

                if (i == 15) // -- CE l = 65 deg
                {
                    strbuild.AppendLine(" coe test circular equatorial ----------------------------");
                    r = new double[] { 6199.6905946008, 13295.2793851394, 0.0 };
                    v = new double[] { -4.72425923942564, 2.20295826245369, 0.0 };
                }
                if (i == 16) // -- CE l = 65 deg i = 180 deg
                {
                    strbuild.AppendLine(" coe test circular equatorial ----------------------------");
                    r = new double[] { 6199.6905946008, -13295.2793851394, 0.0 };
                    v = new double[] { -4.72425923942564, -2.20295826245369, 0.0 };
                }

                // ------- parabolic orbit tests -------------------
                if (i == 17)
                {
                    strbuild.AppendLine(" coe test parabolic ----------------------------");
                    r = new double[] { 0.5916109 * AstroLibr.gravConst.re, -1.2889359 * AstroLibr.gravConst.re, -0.3738343 * AstroLibr.gravConst.re };
                    v = new double[] { 1.1486347 * AstroLibr.gravConst.velkmps, -0.0808249 * AstroLibr.gravConst.velkmps, -0.1942733 * AstroLibr.gravConst.velkmps };
                }

                if (i == 18)
                {
                    strbuild.AppendLine(" coe test parabolic ----------------------------");
                    r = new double[] { -1.0343646 * AstroLibr.gravConst.re, -0.4814891 * AstroLibr.gravConst.re, 0.1735524 * AstroLibr.gravConst.re };
                    v = new double[] { 0.1322278 * AstroLibr.gravConst.velkmps, 0.7785322 * AstroLibr.gravConst.velkmps, 1.0532856 * AstroLibr.gravConst.velkmps };
                }

                if (i == 19)
                {
                    strbuild.AppendLine(" coe test hyperbolic ---------------------------");
                    r = new double[] { 0.9163903 * AstroLibr.gravConst.re, 0.7005747 * AstroLibr.gravConst.re, -1.3909623 * AstroLibr.gravConst.re };
                    v = new double[] { 0.1712704 * AstroLibr.gravConst.velkmps, 1.1036199 * AstroLibr.gravConst.velkmps, -0.3810377 * AstroLibr.gravConst.velkmps };
                }

                if (i == 20)
                {
                    strbuild.AppendLine(" coe test hyperbolic ---------------------------");
                    r = new double[] { 12.3160223 * AstroLibr.gravConst.re, -7.0604653 * AstroLibr.gravConst.re, -3.7883759 * AstroLibr.gravConst.re };
                    v = new double[] { -0.5902725 * AstroLibr.gravConst.velkmps, 0.2165037 * AstroLibr.gravConst.velkmps, 0.1628339 * AstroLibr.gravConst.velkmps };
                }

                strbuild.AppendLine(" start r " + r[0].ToString(fmt) + " " + r[1].ToString(fmt) + " " + r[2].ToString(fmt) + " " +
                "v " + v[0].ToString(fmt) + " " + v[1].ToString(fmt) + " " + v[2].ToString(fmt));
                // --------  coe2rv       - classical elements to posisiotn and velocity
                // --------  rv2coe       - position and velocity vectors to classical elements
                AstroLibr.rv2coe(r, v, out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out arglat, out truelon, out lonper);
                strbuild.AppendLine("           p km       a km      ecc      incl deg     raan deg     argp deg      nu deg      m deg      arglat   truelon    lonper");
                strbuild.AppendLine("ans coes " + a.ToString(fmt).PadLeft(17) + " " + ecc.ToString("0.000000000") + " " + (incl * rad).ToString(fmt) + " " +
                          (raan * rad).ToString(fmt) + " " + (argp * rad).ToString(fmt) + " " + (nu * rad).ToString(fmt) + " " + (m * rad).ToString(fmt) + " " +
                          (arglat * rad).ToString(fmt) + " " + (truelon * rad).ToString(fmt) + " " + (lonper * rad).ToString(fmt));

                AstroLibr.coe2rv(p, ecc, incl, raan, argp, nu, arglat, truelon, lonper, out r1, out v1);
                strbuild.AppendLine(" end  r " + r1[0].ToString(fmt) + " " + r1[1].ToString(fmt) + " " + r1[2].ToString(fmt) + " " +
                "v " + v1[0].ToString(fmt) + " " + v1[1].ToString(fmt) + " " + v1[2].ToString(fmt));
            }  // through for
        }

        public void testfindc2c3()
        {
            double znew, c2new, c3new;

            // --------  findc2c3     - find c2 c3 parameters for f and g battins method
            znew = -39.47842;
            AstroLibr.findc2c3(znew, out c2new, out c3new);
            strbuild.AppendLine("findc2c3 z " + znew.ToString(fmt) + " " + c2new.ToString(fmt) + " " + c3new.ToString(fmt));

            znew = 0.0;
            AstroLibr.findc2c3(znew, out c2new, out c3new);
            strbuild.AppendLine("findc2c3 z " + znew.ToString(fmt) + " " + c2new.ToString(fmt) + " " + c3new.ToString(fmt));

            znew = 0.57483;
            AstroLibr.findc2c3(znew, out c2new, out c3new);
            strbuild.AppendLine("findc2c3 z " + znew.ToString(fmt) + " " + c2new.ToString(fmt) + " " + c3new.ToString(fmt));

            znew = 39.47842;
            AstroLibr.findc2c3(znew, out c2new, out c3new);
            strbuild.AppendLine("findc2c3 z " + znew.ToString(fmt) + " " + c2new.ToString(fmt) + " " + c3new.ToString(fmt));
        }


        public void testcoe2rv()
        {
            double[] r = new double[3];
            double[] v = new double[3];
            double p, a, ecc, incl, raan, argp, nu, m, arglat, truelon, lonper;

            r = new double[] { -605.79221660, -5870.22951108, 3493.05319896 };
            v = new double[] { -1.56825429, -3.70234891, -6.47948395 };
            AstroLibr.rv2coe(r, v, out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out arglat, out truelon, out lonper);

            AstroLibr.coe2rv(p, ecc, incl, raan, argp, nu, arglat, truelon, lonper, out r, out v);

            strbuild.AppendLine("coe2rv r " + r[0].ToString(fmt) + " " + r[1].ToString(fmt) + " " + r[2].ToString(fmt) + " " +
                "v " + v[0].ToString(fmt) + " " + v[1].ToString(fmt) + " " + v[2].ToString(fmt));
        }

        public void testlon2nu()
        {
            double rad = 180.0 / Math.PI;
            double jdut1, lon, incl, raan, argp;
            string strtext;
            jdut1 = 2449470.5;
            incl = 35.324598 / rad;
            lon = -121.3487 / rad;
            raan = 45.0 / rad;
            argp = 34.456798 / rad;

            lon = AstroLibr.lon2nu(jdut1, lon, incl, raan, argp, out strtext);

        }

        // faster version?
        public void testnewtonmx()
        {
            double rad = 180.0 / Math.PI;
            double ecc, eccanom, m, nu;
            ecc = 0.4;
            m = 334.566986 / rad;

            AstroLibr.newtonmx(ecc, m, out eccanom, out nu);

            strbuild.AppendLine(" newtonmx " + ecc.ToString() + " m " + (m * rad).ToString() +
                " eccanom " + (eccanom * rad).ToString() + " nu " + (nu * rad).ToString());
        }

        // --------  newtonm      - find eccentric and true anomaly given ecc and mean anomaly
        public void testnewtonm()
        {
            double rad = 180.0 / Math.PI;
            double ecc, eccanom, m, nu;
            ecc = 0.4;
            eccanom = 334.566986 / rad;
            AstroLibr.newtone(ecc, eccanom, out m, out nu);

            strbuild.AppendLine(" newtone ecc " + ecc.ToString(fmt) + " eccanom " + (eccanom * rad).ToString(fmt) +
                " m " + (m * rad).ToString(fmt) + " nu " + (nu * rad).ToString(fmt));

            ecc = 0.34;
            m = 235.4 / rad;
            AstroLibr.newtonm(ecc, m, out eccanom, out nu);
            strbuild.AppendLine(" newtonm ecc " + ecc.ToString(fmt) + " m " + (m * rad).ToString(fmt) +
                " eccanom " + (eccanom * rad).ToString(fmt) + " nu " + (nu * rad).ToString(fmt));
        }


        // --------  newtone      - find true and mean anomaly given ecc and eccentric anomaly
        public void testnewtone()
        {
            double rad = 180.0 / Math.PI;
            double ecc, eccanom, m, nu;
            ecc = 0.34;
            eccanom = 334.566986 / rad;
            AstroLibr.newtone(ecc, eccanom, out m, out nu);

            strbuild.AppendLine(" newtone ecc " + ecc.ToString(fmt) + " eccanom " + (eccanom * rad).ToString(fmt) +
                " m " + (m * rad).ToString(fmt) + " nu " + (nu * rad).ToString(fmt));
        }

        // --------  newtonnu     - find eccentric and mean anomaly given ecc and true anomaly
        public void testnewtonnu()
        {
            double rad = 180.0 / Math.PI;
            double ecc, eccanom, m, nu;
            ecc = 0.34;
            nu = 134.567001 / rad;

            AstroLibr.newtonnu(ecc, nu, out eccanom, out m);

            strbuild.AppendLine(" newtonnu ecc " + ecc.ToString(fmt) + " nu " + (nu * rad).ToString(fmt) +
                " eccanom " + (eccanom * rad).ToString(fmt) + " m " + (m * rad).ToString(fmt));
        }


        public void keplerc2c3
        (
            double[] r1, double[] v1, double dtseco, out double[] r2, out double[] v2,
            out double c2new, out double c3new, out double xnew, out double znew
        )
        {
            // -------------------------  implementation   -----------------
            int ktr, i, numiter, mulrev;
            double[] h = new double[3];
            double[] rx = new double[3];
            double[] vx = new double[3];
            double f, g, fdot, gdot, rval, xold, xoldsqrd,
                  xnewsqrd, p, dtnew, rdotv, a, dtsec,
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
            v2 = vx;
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
                //            printf(" r1 %16.8f %16.8f %16.8f ER \n",r1[0]/AstroLibr.gravConst.re,r1[1]/AstroLibr.gravConst.re,r1[2]/AstroLibr.gravConst. );
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
                magvo = MathTimeLibr.mag(v1);
                rdotv = MathTimeLibr.dot(r1, v1);

                // -------------  find sme, alpha, and a  ------------------
                sme = ((magvo * magvo) * 0.5) - (AstroLibr.gravConst.mu / magro);
                alpha = -sme * 2.0 / AstroLibr.gravConst.mu;

                if (Math.Abs(sme) > small)
                    a = -AstroLibr.gravConst.mu / (2.0 * sme);
                else
                    a = 999999.9;
                if (Math.Abs(alpha) < small)   // parabola
                    alpha = 0.0;

                if (show == 'y')
                {
                    //           printf(" sme %16.8f  a %16.8f alp  %16.8f ER \n",sme/(AstroLibr.gravConst.mu/AstroLibr.gravConst.), a/AstroLibr.gravConst.re, alpha * AstroLibr.gravConst. );
                    //           printf(" sme %16.8f  a %16.8f alp  %16.8f km \n",sme, a, alpha );
                    //           printf(" ktr      xn        psi           r2          xn+1        dtn \n" );
                }

                // ------------   setup initial guess for x  ---------------
                // -----------------  circle and ellipse -------------------
                if (alpha >= small)
                {
                    period = twopi * Math.Sqrt(Math.Abs(a * a * a) / AstroLibr.gravConst.mu);
                    // ------- next if needed for 2body multi-rev ----------
                    if (Math.Abs(dtseco) > Math.Abs(period))
                        // including the truncation will produce vertical lines that are parallel
                        // (plotting chi vs time)
                        //                    dtsec = rem( dtseco,period );
                        mulrev = Convert.ToInt16(dtseco / period);
                    if (Math.Abs(alpha - 1.0) > small)
                        xold = Math.Sqrt(AstroLibr.gravConst.mu) * dtsec * alpha;
                    else
                        // - first guess can't be too close. ie a circle, r2=a
                        xold = Math.Sqrt(AstroLibr.gravConst.mu) * dtsec * alpha * 0.97;
                }
                else
                {
                    // --------------------  parabola  ---------------------
                    if (Math.Abs(alpha) < small)
                    {
                        MathTimeLibr.cross(r1, v1, out h);
                        magh = MathTimeLibr.mag(h);
                        p = magh * magh / AstroLibr.gravConst.mu;
                        s = 0.5 * (halfpi - Math.Atan(3.0 * Math.Sqrt(AstroLibr.gravConst.mu / (p * p * p)) * dtsec));
                        w = Math.Atan(Math.Pow(Math.Tan(s), (1.0 / 3.0)));
                        xold = Math.Sqrt(p) * (2.0 * MathTimeLibr.cot(2.0 * w));
                        alpha = 0.0;
                    }
                    else
                    {
                        // ------------------  hyperbola  ------------------
                        temp = -2.0 * AstroLibr.gravConst.mu * dtsec /
                              (a * (rdotv + Math.Sign(dtsec) * Math.Sqrt(-AstroLibr.gravConst.mu * a) *
                              (1.0 - magro * alpha)));
                        xold = Math.Sign(dtsec) * Math.Sqrt(-a) * Math.Log(temp);
                    }
                } // if alpha

                ktr = 1;
                dtnew = -10.0;
                double tmp = 1.0 / Math.Sqrt(AstroLibr.gravConst.mu);
                while ((Math.Abs(dtnew * tmp - dtsec) >= small) && (ktr < numiter))
                {
                    xoldsqrd = xold * xold;
                    znew = xoldsqrd * alpha;

                    // ------------- find c2 and c3 functions --------------
                    AstroLibr.findc2c3(znew, out c2new, out c3new);

                    // ------- use a newton iteration for new values -------
                    rval = xoldsqrd * c2new + rdotv * tmp * xold * (1.0 - znew * c3new) +
                             magro * (1.0 - znew * c2new);
                    dtnew = xoldsqrd * xold * c3new + rdotv * tmp * xoldsqrd * c2new +
                             magro * xold * (1.0 - znew * c3new);

                    // ------------- calculate new value for x -------------
                    xnew = xold + (dtsec * Math.Sqrt(AstroLibr.gravConst.mu) - dtnew) / rval;

                    // ----- check if the univ param goes negative. if so, use bissection
                    if (xnew < 0.0)
                        xnew = xold * 0.5;

                    if (show == 'y')
                    {
                        //  printf("%3i %11.7f %11.7f %11.7f %11.7f %11.7f \n", ktr,xold,znew,rval,xnew,dtnew);
                        //  printf("%3i %11.7f %11.7f %11.7f %11.7f %11.7f \n", ktr,xold/sqrt(AstroLibr.gravConst.),znew,rval/AstroLibr.gravConst.re,xnew/sqrt(AstroLibr.gravConst.),dtnew/sqrt(mu));
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
                        v2[i] = 0.0;
                        r2[i] = v2[i];
                    }
                }
                else
                {
                    // --- find position and velocity vectors at new time --
                    xnewsqrd = xnew * xnew;
                    f = 1.0 - (xnewsqrd * c2new / magro);
                    g = dtsec - xnewsqrd * xnew * c3new / Math.Sqrt(AstroLibr.gravConst.mu);

                    for (i = 0; i < 3; i++)
                        r2[i] = f * r1[i] + g * v1[i];
                    magr = MathTimeLibr.mag(r2);
                    gdot = 1.0 - (xnewsqrd * c2new / magr);
                    fdot = (Math.Sqrt(AstroLibr.gravConst.mu) * xnew / (magro * magr)) * (znew * c3new - 1.0);
                    for (i = 0; i < 3; i++)
                        v2[i] = fdot * r1[i] + gdot * v1[i];
                    magv = MathTimeLibr.mag(v2);
                    temp = f * gdot - fdot * g;
                    //if (Math.Abs(temp - 1.0) > 0.00001)
                    //    errork = "fandg";

                    if (show == 'y')
                    {
                        //           printf("f %16.8f g %16.8f fdot %16.8f gdot %16.8f \n",f, g, fdot, gdot );
                        //           printf("f %16.8f g %16.8f fdot %16.8f gdot %16.8f \n",f, g, fdot, gdot );
                        //           printf("r1 %16.8f %16.8f %16.8f ER \n",r2[0]/AstroLibr.gravConst.re,r2[1]/AstroLibr.gravConst.re,r2[2]/AstroLibr.gravConst. );
                        //           printf("v1 %16.8f %16.8f %16.8f ER/TU \n",v[0]/velkmps, v[1]/velkmps, v[2]/velkmps );
                    }
                }
            } // if fabs
            else
                // ----------- set vectors to incoming since 0 time --------
                for (i = 0; i < 3; i++)
                {
                    r2[i] = r1[i];
                    v2[i] = v1[i];
                }

            //       fprintf( fid,"%11.5f  %11.5f %11.5f  %5i %3i ",znew, dtseco/60.0, xold/(rad), ktr, mulrev );
        }  // keplerc2c3


        public void testfindfandg()
        {
            double[] r1;
            double[] v1;
            double[] r2;
            double[] v2;
            double dtsec, x, c2, c3, z, f, g, fdot, gdot;
            string opt = "pqw";  //  pqw, series, c2c3

            r1 = new double[] { 4938.49830042171, -1922.24810472241, 4384.68293292613 };
            v1 = new double[] { 0.738204644165659, 7.20989453238397, 2.32877392066299 };
            r2 = new double[] { -1105.78023519582, 2373.16130661458, 6713.89444816503 };
            v2 = new double[] { 5.4720951867079, -4.39299050886976, 2.45681739563752 };
            dtsec = 6372.69272563561; // 1ld
            dtsec = 60.0; // must be small step sizes!!

            //// dan hyperbolic test
            //r1 = new double[] { 4070.5942270000000, 3786.8271570000002, 4697.0576309999997 };
            ////v1 = new double[] { -32553.559100851671, -37563.543526937596, -37563.543526937596 };
            //// exact opposite from r velocity
            //v1 = new double[] { -34845.69531184976, -32416.55098811211, -40208.43885307875 };
            //r2 = new double[] { -1105.78023519582, 2373.16130661458, 6713.89444816503 };
            //v2 = new double[] { 5.4720951867079, -4.39299050886976, 2.45681739563752 };
            //dtsec = 0.25; // must be small step sizes!!

            strbuild.AppendLine(" r1 " + r1[0].ToString(fmt) + " " + r1[1].ToString(fmt) + " " + r1[2].ToString(fmt) + " " +
               "v1 " + v1[0].ToString(fmt) + " " + v1[1].ToString(fmt) + " " + v1[2].ToString(fmt));

            for (int i = 0; i <= 5; i++)
            {
                if (i == 0)
                    dtsec = 60.0;
                if (i == 1)
                    dtsec = 0.1;
                if (i == 2)
                    dtsec = 1.0;
                if (i == 3)
                    dtsec = 10.0;
                if (i == 4)
                    dtsec = 100.0;
                if (i == 5)
                    dtsec = 500.0;

                keplerc2c3(r1, v1, dtsec, out r2, out v2, out c2, out c3, out x, out z);
                strbuild.AppendLine(" r2 " + r2[0].ToString(fmt) + " " + r2[1].ToString(fmt) + " " + r2[2].ToString(fmt) + " " +
                    "v2 " + v2[0].ToString(fmt) + " " + v2[1].ToString(fmt) + " " + v2[2].ToString(fmt));

                strbuild.AppendLine("c2 " + c2.ToString(fmt) + " c3 " + c3.ToString(fmt) + " x " +
                    x.ToString(fmt) + " z " + z.ToString(fmt) + " dtsec " + dtsec.ToString(fmt));

                opt = "pqw";
                AstroLibr.findfandg(r1, v1, r2, v2, dtsec, x, c2, c3, z, opt, out f, out g, out fdot, out gdot);
                double ans = f * gdot - g * fdot;
                strbuild.AppendLine("f and g pqw    " + f.ToString(fmt) + " " + g.ToString(fmt) + " " +
                    fdot.ToString(fmt) + " " + gdot.ToString(fmt) + " " + ans.ToString(fmt));

                opt = "series";  //  pqw, series, c2c3
                AstroLibr.findfandg(r1, v1, r2, v2, dtsec, x, c2, c3, z, opt, out f, out g, out fdot, out gdot);
                ans = f * gdot - g * fdot;
                strbuild.AppendLine("f and g series " + f.ToString(fmt) + " " + g.ToString(fmt) + " " +
                    fdot.ToString(fmt) + " " + gdot.ToString(fmt) + " " + ans.ToString(fmt));

                opt = "c2c3";  //  pqw, series, c2c3
                AstroLibr.findfandg(r1, v1, r2, v2, dtsec, x, c2, c3, z, opt, out f, out g, out fdot, out gdot);
                ans = f * gdot - g * fdot;

                strbuild.AppendLine("f and g c2c3   " + f.ToString(fmt) + " " + g.ToString(fmt) + " " +
                    fdot.ToString(fmt) + " " + gdot.ToString(fmt) + " " + ans.ToString(fmt) + "\n");
            }

        }

        public void testcheckhitearth()
        {
            string hitearthstr = "";
            double[] r1 = new double[3];
            double[] v1t = new double[3];
            double[] r2 = new double[3];
            double[] v2t = new double[3];
            double ang, magr1, magr2, cosdeltanu, altpad, rp, a;
            Int32 nrev;
            char hitearth;

            nrev = 0;
            r1 = new double[] { 2.500000 * AstroLibr.gravConst.re, 0.000000, 0.000000 };
            r2 = new double[] { 1.9151111 * AstroLibr.gravConst.re, 1.6069690 * AstroLibr.gravConst.re, 0.000000 };
            // assume circular initial orbit for vel calcs
            v1t = new double[] { 0.0, Math.Sqrt(AstroLibr.gravConst.mu / r1[0]), 0.0 };
            ang = Math.Atan(r2[1] / r2[0]);
            v2t = new double[] { -Math.Sqrt(AstroLibr.gravConst.mu / r2[1]) * Math.Cos(ang), Math.Sqrt(AstroLibr.gravConst.mu / r2[0]) * Math.Sin(ang), 0.0 };
            altpad = 100.0; // km

            magr1 = MathTimeLibr.mag(r1);
            magr2 = MathTimeLibr.mag(r2);
            cosdeltanu = MathTimeLibr.dot(r1, r2) / (magr1 * magr2);

            AstroLibr.checkhitearth(altpad, r1, v1t, r2, v2t, nrev, out hitearth, out hitearthstr, out rp, out a);

            strbuild.AppendLine("hitearth? " + hitearthstr + " " + (Math.Acos(cosdeltanu) * 180.0 / Math.PI).ToString(fmt) + " ");
        }

        public void testcheckhitearthc()
        {
            string hitearthstr = "";
            double[] r1c = new double[3];
            double[] v1tc = new double[3];
            double[] r2c = new double[3];
            double[] v2tc = new double[3];
            double ang, magr1c, magr2c, cosdeltanu, altpadc, rp, a;
            Int32 nrev;
            char hitearth;

            nrev = 0;
            r1c = new double[] { 2.500000, 0.000000, 0.000000 };
            r2c = new double[] { 1.9151111, 1.6069690, 0.000000 };
            // assume circular initial orbit for vel calcs
            v1tc = new double[] { 0.0, Math.Sqrt(1.0 / r1c[0]), 0.0 };
            ang = Math.Atan(r2c[1] / r2c[0]);
            v2tc = new double[] { -Math.Sqrt(1.0 / r2c[1]) * Math.Cos(ang), Math.Sqrt(1.0 / r2c[0]) * Math.Sin(ang), 0.0 };
            altpadc = 100.0 / AstroLibr.gravConst.re; // er

            magr1c = MathTimeLibr.mag(r1c);
            magr2c = MathTimeLibr.mag(r2c);
            cosdeltanu = MathTimeLibr.dot(r1c, r2c) / (magr1c * magr2c);
            AstroLibr.checkhitearthc(altpadc, r1c, v1tc, r2c, v2tc, nrev, out hitearth, out hitearthstr, out rp, out a);

            strbuild.AppendLine("hitearth? " + hitearthstr + " " + (Math.Acos(cosdeltanu) * 180.0 / Math.PI).ToString(fmt) + " ");
        }


        public void testgibbs()
        {
            double[] r1 = new double[3];
            double[] r2 = new double[3];
            double[] r3 = new double[3];
            double[] v2 = new double[3];
            double copa, theta, theta1, rad;
            string errorstr;

            rad = 180.0 / Math.PI;

            r1 = new double[] { 0.0000000, 0.000000, AstroLibr.gravConst.re };
            r2 = new double[] { 0.0000000, -4464.696, -5102.509 };
            r3 = new double[] { 0.0000000, 5740.323, 3189.068 };

            AstroLibr.gibbs(r1, r2, r3, out v2, out theta, out theta1, out copa, out errorstr);

            strbuild.AppendLine("testgibbs " + v2[0].ToString(fmt) + " " + v2[1].ToString(fmt) + " " + v2[2].ToString(fmt));
            strbuild.AppendLine("testgibbs " + (theta * rad).ToString(fmt) + " " + (theta1 * rad).ToString(fmt) + " " + (copa * rad).ToString(fmt));
        }


        public void testhgibbs()
        {
            double[] r1 = new double[3];
            double[] r2 = new double[3];
            double[] r3 = new double[3];
            double[] v2 = new double[3];
            double copa, theta, theta1, rad, jd1, jd2, jd3;
            string errorstr;

            rad = 180.0 / Math.PI;

            r1 = new double[] { 0.0000000, 0.000000, AstroLibr.gravConst.re };
            r2 = new double[] { 0.0000000, -4464.696, -5102.509 };
            r3 = new double[] { 0.0000000, 5740.323, 3189.068 };
            jd1 = 2451849.5;
            jd2 = jd1 + 1.0 / 1440.0 + 16.48 / 86400.0;
            jd3 = jd1 + 2.0 / 1440.0 + 33.04 / 86400.0;
            AstroLibr.herrgibbs(r1, r2, r3, jd1, jd2, jd3, out v2, out theta, out theta1, out copa, out errorstr);

            strbuild.AppendLine("testherrgibbs " + v2[0].ToString(fmt) + " " + v2[1].ToString(fmt) + " " + v2[2].ToString(fmt));
            strbuild.AppendLine("testherrgibbs " + (theta * rad).ToString(fmt) + " " + (theta1 * rad).ToString(fmt) + " " + (copa * rad).ToString(fmt));
        }


        // ---------------------------------------------------------------------------------
        // get root from Gauss to use as a seed for double-r and Gooding
        public void getGaussRoot
            (
            double tdecl1, double tdecl2, double tdecl3,
            double trtasc1, double trtasc2, double trtasc3,
            double jd1, double jdf1, double jd2, double jdf2, double jd3, double jdf3,
            double[] rs1, double[] rs2, double[] rs3, out double bigr2
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
            double deriv, derivn1, derivn2, bigr2n; 

            // ---- set middle to 0, deltas to other times ---- 
            // need to separate into jd and jdf portions for accuracy since it's often only seconds or minutes
            tau12 = (jd1 - jd2) * 86400.0 + (jdf1 - jdf2) * 86400.0; // days to sec
            tau13 = (jd1 - jd3) * 86400.0 + (jdf1 - jdf3) * 86400.0;
            tau32 = (jd3 - jd2) * 86400.0 + (jdf3 - jdf2) * 86400.0;

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
                rsmat[i, 0] = rs1[i];  // km
                rsmat[i, 1] = rs2[i];
                rsmat[i, 2] = rs3[i];
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
            l2dotrs = MathTimeLibr.dot(los2, rs2);
            poly[1] = 1.0;  // r2
            poly[2] = 0.0;
            poly[3] = -(d1 * d1 + 2.0 * d1 * l2dotrs + MathTimeLibr.mag(rs2) * MathTimeLibr.mag(rs2));
            poly[4] = 0.0;
            poly[5] = 0.0;
            poly[6] = -2.0 * AstroLibr.gravConst.mu * d2 * (l2dotrs + d1);
            poly[7] = 0.0;
            poly[8] = 0.0;
            poly[9] = -AstroLibr.gravConst.mu * AstroLibr.gravConst.mu * d2 * d2;

            // simply iterate to find the correct root
            bigr2 = 100.0;
            // can do Newton iteration Curtis p 289 simple derivative of the poly
            // makes sense since the values are so huge in the polynomial
            // do as Halley iteration since derivatives are possible. tests at LEO, GPS, GEO,
            // all seem to converge to the proper answer
            int kk = 0;
            bigr2n = 20000.0; // guess ~GPS altitude
            while (Math.Abs(bigr2 - bigr2n) > 0.5 && kk < 15)
            {
                bigr2 = bigr2n;
                deriv = Math.Pow(bigr2, 8) + poly[3] * Math.Pow(bigr2, 6)
                    + poly[6] * Math.Pow(bigr2, 3) + poly[9];
                derivn1 = 8.0 * Math.Pow(bigr2, 7) + 6.0 * poly[3] * Math.Pow(bigr2, 5)
                    + 3.0 * poly[6] * Math.Pow(bigr2, 2);
                derivn2 = 56.0 * Math.Pow(bigr2, 6) + 30.0 * poly[3] * Math.Pow(bigr2, 4)
                    + 6.0 * poly[6] * bigr2;
                // Newton iteration
                //bigr2n = bigr2 - deriv / derivn1;
                // Halley iteration
                bigr2n = bigr2 - (2.0 * deriv * derivn1) / (2.0 * derivn1 * derivn1 - deriv * derivn2);
                kk = kk + 1;
            }
        }  // getGaussRoot




        // test angles-only routines
        // output these results separately to the testall directory
        public void testangles()
        {
            double[] rseci1 = new double[3];
            double[] vseci1 = new double[3];
            double[] rseci2 = new double[3];
            double[] vseci2 = new double[3];
            double[] rseci3 = new double[3];
            double[] vseci3 = new double[3];
            double[] rsecef1 = new double[3];
            double[] rsecef2 = new double[3];
            double[] rsecef3 = new double[3];
            double[] vsecef1 = new double[3];
            double[] vsecef2 = new double[3];
            double[] vsecef3 = new double[3];
            double[] r2 = new double[3];
            double[] v2 = new double[3];
            double rad, jd1, jd2, jd3, jdf1, jdf2, jdf3, lod;
            double[] latgd = new double[15];
            double[] lon = new double[15];
            double[] alt = new double[15];
            double[] trtasc = new double[15];
            double[] tdecl = new double[15];
            double[] initguess = new double[30];
            string errstr;
            Int32[] year = new int[15];
            Int32[] mon = new int[15];
            Int32[] day = new int[15];
            Int32[] hr = new int[15];
            Int32[] minute = new int[15];
            double[] second = new double[15];
            int dat;
            double[] jd = new double[100];
            double[] jdf = new double[100];
            double jdut1, dut1, jdtt, jdftt, ttt, xp, yp, conv, ddx, ddy, ddpsi, ddeps;
            double rng1, rng2, rng3;
            Int32 iyear, imon, iday, ihr, iminute;
            double isecond, bigr2x;
            Int32 numhalfrev;
            conv = Math.PI / (180.0 * 3600.0);
            rad = 180.0 / Math.PI;
            errstr = "";
            char diffsites = 'n';
            StringBuilder strbuild = new StringBuilder();
            StringBuilder strbuildall = new StringBuilder();

            this.opsStatus.Text = "Test Angles ";
            Refresh();

            string nutLoc;
            string ans;
            Int32 ktrActObs;
            string EOPupdate;
            Int32 mjdeopstart;
            EOPSPWLib.iau80Class iau80arr;
            EOPSPWLib.iau00Class iau00arr;
            nutLoc = @"D:\Codes\LIBRARY\DataLib\nut80.dat";
            EOPSPWLibr.iau80in(nutLoc, out iau80arr);
            nutLoc = @"D:\Codes\LIBRARY\DataLib\";
            EOPSPWLibr.iau00in(nutLoc, out iau00arr);

            string eopFileName = @"D:\Codes\LIBRARY\DataLib\EOP-All-v1.1_2020-02-12.txt";
            EOPSPWLibr.readeop(ref EOPSPWLibr.eopdata, eopFileName, out mjdeopstart, out ktrActObs, out EOPupdate);

            // now read it in
            double jdxysstart, jdfxysstart;
            AstroLib.xysdataClass[] xysarr = AstroLibr.xysarr;
            AstroLibr.initXYS(ref xysarr, nutLoc, "xysdata.dat", out jdxysstart, out jdfxysstart);

            // gooding tests cases from Gooding paper (1997 CMDA)
            double[] los1;
            double[] los2;
            double[] los3;
            jd1 = 0.0;
            jd2 = 0.0;
            jd3 = 0.0;

            // read input data
            // note the input data has a # line between each case
            string infilename = @"D:\Codes\LIBRARY\DataLib\anglestest.dat";
            string[] fileData = File.ReadAllLines(infilename);

            // --- read obs data in
            //int caseopt = 0;  // set this for whichever case to run

            // find mins
            // orbits only need to be close
            double rtol = 500.0; // km
            double vtol = 0.1;   // km/s
            double atol = 500.0; // km
            double ptol = 500.0; // km
            double etol = 0.1;  // 
            double itol = 5.0 / rad;   // rad

            for (int caseopt = 0; caseopt <= 23; caseopt++)  // 0  23
            {
                strbuild.AppendLine("caseopt " + caseopt.ToString());
                int ktr = 1;     // skip header, go to next # comment line

                string line = fileData[ktr];
                line.Replace(@"\s+", " ");
                string[] linesplt = line.Split(' ');
                int tmpcase = Convert.ToInt32(linesplt[1]);
                while (tmpcase != caseopt)
                {
                    line = fileData[ktr];
                    line.Replace(@"\s+", " ");
                    linesplt = line.Split(' ');
                    if (line[0].Equals('#'))
                        tmpcase = Convert.ToInt32(linesplt[1]);

                    ktr = ktr + 1;
                }

                // get all the data for caseopt
                int obsktr = 0;
                // set the first case only
                if (caseopt == 0)
                {
                    ans = fileData[ktr];
                    ktr = 2;
                }
                else
                    ans = fileData[ktr - 1];
                while (ktr < fileData.Count() && !fileData[ktr][0].Equals('#'))
                {
                    line = fileData[ktr];
                    linesplt = line.Split(',');
                    mon[obsktr] = Convert.ToInt32(linesplt[1]);
                    day[obsktr] = Convert.ToInt32(linesplt[0]);
                    year[obsktr] = Convert.ToInt32(linesplt[2]);
                    hr[obsktr] = Convert.ToInt32(linesplt[3]);
                    minute[obsktr] = Convert.ToInt32(linesplt[4]);
                    second[obsktr] = Convert.ToDouble(linesplt[5]);
                    MathTimeLibr.jday(year[obsktr], mon[obsktr], day[obsktr], hr[obsktr], minute[obsktr], second[obsktr],
                        out jd[obsktr], out jdf[obsktr]);

                    latgd[obsktr] = Convert.ToDouble(linesplt[6]) / rad;
                    lon[obsktr] = Convert.ToDouble(linesplt[7]) / rad;
                    alt[obsktr] = Convert.ToDouble(linesplt[8]) / rad;

                    trtasc[obsktr] = Convert.ToDouble(linesplt[9]) / rad;
                    tdecl[obsktr] = Convert.ToDouble(linesplt[10]) / rad;
                    if (obsktr == 0)
                        initguess[tmpcase] = Convert.ToDouble(linesplt[11]);  // initial guess in km

                    obsktr = obsktr + 1;
                    ktr = ktr + 1;
                }

                int idx1, idx2, idx3;
                idx1 = 0;
                idx2 = 1;
                idx3 = 2;
                strbuildall.AppendLine("/n/n ================================ case number " + caseopt.ToString() + " ================================");
                strbuild.AppendLine("/n/n ================================ case number " + caseopt.ToString() + " ================================");
                switch (caseopt)
                {
                    case 0:
                        idx1 = 2;
                        idx2 = 4;
                        idx3 = 5;

                        break;
                    case 1:
                        // book example
                        //dut1 = -0.609641;      // sec
                        //dat = 35;              // sec
                        //lod = 0.0;
                        //xp = 0.137495 * conv;  // " to rad
                        //yp = 0.342416 * conv;
                        //ddpsi = 0.0;  // " to rad
                        //ddeps = 0.0;
                        //ddx = 0.0;    // " to rad
                        //ddy = 0.0;
                        //latgd = 40.0 / rad;
                        //lon = -110.0 / rad;
                        //alt = 2.0;  // km
                        // ---- select points to use
                        idx1 = 2;
                        idx2 = 4;
                        idx3 = 5;

                        idx1 = 5;
                        idx2 = 9;
                        idx3 = 13;

                        break;
                    case int n when (n >= 2 && n <= 12):
                        idx1 = 0;
                        idx2 = 1;
                        idx3 = 2;
                        break;
                }  // end switch

                //    // herrick interplantetary Herrick pp 384-5 & 418 (gibbs)
                //    // units of days in 1910 (?) and au
                //    // says mu = 1.0
                //    // k = 0, rho = 5.9 and 5.9 au?
                //    // some initi rho of 1000000 still works
                //    // days 7.8205, 26.7480, 48.6262 (in 1910 Nov)
                //    //rtasc = 3-50-24.3, decl = 25-11-10.5
                //    //rtasc = 3-13-3.0  decl = 22-29-31.3
                //    //rtasc = 4-54-19.5  decl = 20-14-51.9
                //    double tau12 = 0.325593;
                //    double tau13 = 0.701944;
                //    rseci1 = new double[] { 0.7000687 * AstroLibr.astroConsts.au,
                //        0.6429399 * AstroLibr.astroConsts.au, 0.2789211 * AstroLibr.astroConsts.au };
                //    rseci2 = new double[] { 0.4306907, 0.8143496, 0.3532745 };
                //    rseci3 = new double[] { 0.0628371, 0.9007098, 0.3907417 };
                //    los1 = new double[] { 0.9028975, 0.0606048, 0.4255621 };
                //    los2 = new double[] { 0.9224764, 0.0518570, 0.3825549 };
                //    los3 = new double[] { 0.9347684, 0.0802269, 0.3460802 };
                //    //(light-corrected times not used but aze 0.325578 and 0.701903)
                //    break;
                //case 2:
                //    // extreme case Lane ex 2
                //    tau12 = 1.570796327;
                //    tau13 = 3.141592654;
                //    rseci1 = new double[] { 0.0, -1.0, 0.0 };
                //    rseci2 = new double[] { 1.0, 0.0, 0.0 };
                //    rseci3 = new double[] { 0.0, 1.0, 0.0 };
                //    los1 = new double[] { 0.0, 1.0, 0.0 };
                //    los2 = new double[] { 0.0, 0.0, 1.0 };
                //    los3 = new double[] { 0.0, -1.0, 0.0 };

                //    break;
                //case 3:
                //    // rectilinear case
                //    tau12 = 1.047197552;
                //    tau13 = 2.960420507;
                //    rseci1 = new double[] { 1.0, 0.0, 0.0 };
                //    rseci2 = new double[] { 0.0, 1.0, 0.0 };
                //    rseci3 = new double[] { 0.0, 0.0, 0.0 };
                //    los1 = new double[] { -1.0, 0.0, 0.5 };
                //    los2 = new double[] { 0.0, -1.0, 1.5 };
                //    los3 = new double[] { 0.0, 0.0, 2.0 };
                //    break;
                //case 5:
                //    // revised escobal example 
                //    tau12 = 0.0381533;
                //    tau13 = 0.0399364;
                //    rseci1 = new double[] { 0.16606957, 0.84119785, -0.51291356 };
                //    rseci2 = new double[] { -0.73815134, -0.41528280, 0.53035336 };
                //    rseci3 = new double[] { -0.73343987, -0.42352540, 0.53037164 };
                //    los1 = new double[] { -0.92475472, -0.37382824, -0.07128226 };
                //    los2 = new double[] { 0.80904274, -0.55953385, 0.17992142 };
                //    los3 = new double[] { 0.85044131, -0.49106628, 0.18868888 };
                //    break;
                //case 7:
                //    // example from Lane
                //    tau12 = 2.2;
                //    tau13 = 0.35225232;
                //    rseci2 = new double[] { 0.89263524, 0.28086002, 0.35277012 };
                //    rseci3 = new double[] { -0.02703285, 0.93585748, 0.35152067 };
                //    los1 = new double[] { 0.76526944, 0.12314580, 0.63182102 };
                //    los2 = new double[] { -0.21266402, -0.54295751, 0.81238609 };
                //    los3 = new double[] { 0.52029946, -0.39083440, 0.75930030 };
                //    break;


                // state already exists use that period
                // TLE exists use that period
                // otherwise options, 95 min, 108 min, 150 min, 250 min, 7.2 hr, 12 hr, and 24 hr 

//                for (int z = 0; z <= -10; z++)
                //{
                //    switch (z)
                //    {
                //        case 0:
                //            idx1 = 2;
                //            idx2 = 4;
                //            idx3 = 5;
                //            break;
                //        case 1:
                //            idx1 = 0;
                //            idx2 = 2;
                //            idx3 = 3;
                //            break;
                //        case 2:
                //            idx1 = 0;
                //            idx2 = 2;
                //            idx3 = 9;
                //            break;
                //        case 3:
                //            idx1 = 0;
                //            idx2 = 2;
                //            idx3 = 6;
                //            break;
                //        case 4:
                //            idx1 = 0;
                //            idx2 = 2;
                //            idx3 = 7;
                //            break;
                //        case 5:
                //            idx1 = 0;
                //            idx2 = 2;
                //            idx3 = 12;
                //            break;
                //        case 6:
                //            idx1 = 2;
                //            idx2 = 4;
                //            idx3 = 14;
                //            break;
                //        case 7:
                //            idx1 = 2;
                //            idx2 = 4;
                //            idx3 = 7;
                //            break;
                //        case 8:
                //            idx1 = 0;
                //            idx2 = 13;
                //            idx3 = 14;
                //            break;
                //        case 9:
                //            idx1 = 2;
                //            idx2 = 4;
                //            idx3 = 10;
                //            break;
                //        case 10:
                //            idx1 = 10;
                //            idx2 = 11;
                //            idx3 = 12;
                //            break;
                //    }
                //    strbuild.AppendLine("\nz " + z.ToString());

                    //jd1 = jd[idx1] + jdf[idx1];
                    //jd2 = jd[idx2] + jdf[idx2];
                    //jd3 = jd[idx3] + jdf[idx3];
                    AstroLibr.site(latgd[idx1], lon[idx1], alt[idx1], out rsecef1, out vsecef1);
                    EOPSPWLibr.findeopparam(jd[idx1], jdf[idx1], 's', EOPSPWLibr.eopdata, mjdeopstart + 2400000.5,
                        out dut1, out dat, out lod, out xp, out yp, out ddpsi, out ddeps, out ddx, out ddy);
                    //MathTimeLibr.convtime(year[idx1], mon[idx1], day[idx1], hr[idx1], minute[idx1], second[idx1], 0, dut1, dat,
                    //    out ut1, out tut1, out jdut1, out jdut1frac, out utc, out tai,
                    //    out tt, out ttt, out jdtt, out jdttfrac, out tdb, out ttdb, out jdtdb, out jdtdbfrac);
                    jdtt = jd[idx1];
                    jdftt = jdf[idx1] + (dat + 32.184) / 86400.0;
                    ttt = (jdtt + jdftt - 2451545.0) / 36525.0;
                    // note you have to use tdb for time of ineterst AND j2000 (when dat = 32)
                    //  ttt = (jd + jdFrac + (dat + 32.184) / 86400.0 - 2451545.0 - (32 + 32.184) / 86400.0) / 36525.0;
                    jdut1 = jd[idx1] + jdf[idx1] + dut1 / 86400.0;
                    AstroLibr.eci_ecef(ref rseci1, ref vseci1, MathTimeLib.Edirection.efrom, ref rsecef1, ref vsecef1,
                         AstroLib.EOpt.e80, iau80arr, iau00arr,
                         jdtt, jdftt, jdut1, jdxysstart, lod, xp, yp, ddpsi, ddeps, ddx, ddy);

                    AstroLibr.site(latgd[idx2], lon[idx2], alt[idx2], out rsecef2, out vsecef2);
                    EOPSPWLibr.findeopparam(jd[idx2], jdf[idx2], 's', EOPSPWLibr.eopdata, mjdeopstart + 2400000.5,
                        out dut1, out dat, out lod, out xp, out yp, out ddpsi, out ddeps, out ddx, out ddy);
                    //MathTimeLibr.convtime(year[idx2], mon[idx2], day[idx2], hr[idx2], minute[idx2], second[idx2], 0, dut1, dat,
                    //    out ut1, out tut1, out jdut1, out jdut1frac, out utc, out tai,
                    //    out tt, out ttt, out jdtt, out jdttfrac, out tdb, out ttdb, out jdtdb, out jdtdbfrac);
                    jdtt = jd[idx2];
                    jdftt = jdf[idx2] + (dat + 32.184) / 86400.0;
                    ttt = (jdtt + jdftt - 2451545.0) / 36525.0;
                    jdut1 = jd[idx2] + jdf[idx2] + dut1 / 86400.0;
                    AstroLibr.eci_ecef(ref rseci2, ref vseci2, MathTimeLib.Edirection.efrom, ref rsecef2, ref vsecef2,
                         AstroLib.EOpt.e80, iau80arr, iau00arr,
                         jdtt, jdftt, jdut1, jdxysstart, lod, xp, yp, ddpsi, ddeps, ddx, ddy);
                    double gst, lst;
                    AstroLibr.lstime(lon[idx2], jdut1, out lst, out gst);
                    strbuild.AppendLine("\nlst " + lst.ToString() + " " + (lst * rad).ToString());


                    AstroLibr.site(latgd[idx3], lon[idx3], alt[idx3], out rsecef3, out vsecef3);
                    EOPSPWLibr.findeopparam(jd[idx3], jdf[idx3], 's', EOPSPWLibr.eopdata, mjdeopstart + 2400000.5,
                        out dut1, out dat, out lod, out xp, out yp, out ddpsi, out ddeps, out ddx, out ddy);
                    jdtt = jd[idx3];
                    jdftt = jdf[idx3] + (dat + 32.184) / 86400.0;
                    ttt = (jdtt + jdftt - 2451545.0) / 36525.0;
                    jdut1 = jd[idx3] + jdf[idx3] + dut1 / 86400.0;
                    AstroLibr.eci_ecef(ref rseci3, ref vseci3, MathTimeLib.Edirection.efrom, ref rsecef3, ref vsecef3,
                         AstroLib.EOpt.e80, iau80arr, iau00arr,
                         jdtt, jdftt, jdut1, jdxysstart, lod, xp, yp, ddpsi, ddeps, ddx, ddy);

                    if (Math.Abs(latgd[idx1] - latgd[idx2]) < 0.001 && Math.Abs(latgd[idx1] - latgd[idx3]) < 0.001
                        && Math.Abs(lon[idx1] - lon[idx2]) < 0.001 && Math.Abs(lon[idx1] - lon[idx3]) < 0.001)
                        diffsites = 'n';
                    else
                        diffsites = 'y';


                    // write output
                    strbuild.AppendLine("rseci1 " + rseci1[0].ToString("0.000000") + " " +
                    rseci1[1].ToString("0.000000") + " " + rseci1[2].ToString("0.000000"));
                    strbuild.AppendLine("rseci2 " + rseci2[0].ToString("0.000000") + " " +
                        rseci2[1].ToString("0.000000") + " " + rseci2[2].ToString("0.000000"));
                    strbuild.AppendLine("rseci3 " + rseci3[0].ToString("0.000000") + " " +
                        rseci3[1].ToString("0.000000") + " " + rseci3[2].ToString("0.000000"));

                    los1 = new double[3];
                    los2 = new double[3];
                    los3 = new double[3];
                    los1[0] = Math.Cos(tdecl[idx1]) * Math.Cos(trtasc[idx1]);
                    los1[1] = Math.Cos(tdecl[idx1]) * Math.Sin(trtasc[idx1]);
                    los1[2] = Math.Sin(tdecl[idx1]);

                    los2[0] = Math.Cos(tdecl[idx2]) * Math.Cos(trtasc[idx2]);
                    los2[1] = Math.Cos(tdecl[idx2]) * Math.Sin(trtasc[idx2]);
                    los2[2] = Math.Sin(tdecl[idx2]);

                    los3[0] = Math.Cos(tdecl[idx3]) * Math.Cos(trtasc[idx3]);
                    los3[1] = Math.Cos(tdecl[idx3]) * Math.Sin(trtasc[idx3]);
                    los3[2] = Math.Sin(tdecl[idx3]);

                    strbuild.AppendLine("los1 " + los1[0].ToString("0.00000000") + " " +
                        los1[1].ToString("0.00000000") + " " + los1[2].ToString("0.00000000") +
                        " " + MathTimeLibr.mag(los1).ToString("0.00000000"));
                    strbuild.AppendLine("los2 " + los2[0].ToString("0.00000000") + " " +
                        los2[1].ToString("0.00000000") + " " + los2[2].ToString("0.00000000") +
                        " " + MathTimeLibr.mag(los2).ToString("0.00000000"));
                    strbuild.AppendLine("los3 " + los3[0].ToString("0.00000000") + " " +
                        los3[1].ToString("0.00000000") + " " + los3[2].ToString("0.000000") +
                        " " + MathTimeLibr.mag(los3).ToString("0.00000000"));

                    double p, a, ecc, incl, raan, argp, nu, m, arglat, truelon, lonper;

                    // to get initial guess, take measurments (1/2 and 2/3), assume circular orbit
                    // find velocity and compare - just distinguish between LEO, GPS and GEO for now
                    double dtrtasc1, dtdecl1, dtrtasc2, dtdecl2, dt1, dt2;
                    dt1 = (jd[idx2] - jd[idx1]) * 86400.0 + (jdf[idx2] - jdf[idx1]) * 86400.0;
                    dt2 = (jd[idx3] - jd[idx2]) * 86400.0 + (jdf[idx3] - jdf[idx2]) * 86400.0;
                    dtrtasc1 = (trtasc[idx2] - trtasc[idx1]) / dt1;
                    dtrtasc2 = (trtasc[idx3] - trtasc[idx2]) / dt2;
                    dtdecl1 = (tdecl[idx2] - tdecl[idx1]) / dt1;
                    dtdecl2 = (tdecl[idx3] - tdecl[idx2]) / dt2;

                    strbuild.AppendLine("rtasc " + (trtasc[idx1] * rad).ToString() + " " + (trtasc[idx2] * rad).ToString()
                        + " " + (trtasc[idx3] * rad).ToString());
                    strbuild.AppendLine("decl " + (tdecl[idx1] * rad).ToString() + " " + (tdecl[idx2] * rad).ToString()
                        + " " + (tdecl[idx3] * rad).ToString());


                    strbuild.AppendLine("");
                    strbuildall.AppendLine("Laplace -----------------------------------");
                    strbuild.AppendLine("Laplace -----------------------------------");

                    strbuild.AppendLine("\n\ninputs: \n");
                    strbuild.AppendLine("Site obs1 "
                        + rseci1[0].ToString() + " " + rseci1[1].ToString() + " " + rseci1[2].ToString()
                        + " km  lat " + (latgd[idx1] * rad).ToString() + " lon " + (lon[idx1] * rad).ToString() + " "
                        + alt[idx1].ToString());
                    strbuild.AppendLine("Site obs2 "
                        + rseci2[0].ToString() + " " + rseci2[1].ToString() + " " + rseci2[2].ToString()
                        + " km  lat " + (latgd[idx2] * rad).ToString() + " lon " + (lon[idx2] * rad).ToString() + " "
                        + alt[idx2].ToString());
                    strbuild.AppendLine("Site obs3 "
                        + rseci3[0].ToString() + " " + rseci3[1].ToString() + " " + rseci3[2].ToString()
                        + " km  lat " + (latgd[idx3] * rad).ToString() + " lon " + (lon[idx3] * rad).ToString() + " "
                        + alt[idx3].ToString());
                    MathTimeLibr.invjday(jd[idx1], jdf[idx1], out iyear, out imon, out iday, out ihr, out iminute, out isecond);
                    strbuild.AppendLine("obs#1 " + iyear.ToString() + " " + imon.ToString() + " " + iday.ToString()
                        + " " + ihr.ToString("00") + " " + iminute.ToString("00") + " " + isecond.ToString("0.000")
                        + " " + (trtasc[idx1] * rad).ToString() + " " + (tdecl[idx1] * rad).ToString().ToString());
                    MathTimeLibr.invjday(jd[idx2], jdf[idx2], out iyear, out imon, out iday, out ihr, out iminute, out isecond);
                    strbuild.AppendLine("obs#2 " + iyear.ToString() + " " + imon.ToString() + " " + iday.ToString()
                        + " " + ihr.ToString("00") + " " + iminute.ToString("00") + " " + isecond.ToString("0.000")
                        + " " + (trtasc[idx2] * rad).ToString() + " " + (tdecl[idx2] * rad).ToString().ToString());
                    MathTimeLibr.invjday(jd[idx3], jdf[idx3], out iyear, out imon, out iday, out ihr, out iminute, out isecond);
                    strbuild.AppendLine("obs#3 " + iyear.ToString() + " " + imon.ToString() + " " + iday.ToString()
                        + " " + ihr.ToString("00") + " " + iminute.ToString("00") + " " + isecond.ToString("0.000")
                        + " " + (trtasc[idx3] * rad).ToString() + " " + (tdecl[idx3] * rad).ToString().ToString());
                    //if (caseopt == 2)
                    //    diffsites = 'y';
                    //else 
                    //diffsites = 'n';

                    AstroLibr.angleslaplace(tdecl[idx1], tdecl[idx2], tdecl[idx3], trtasc[idx1], trtasc[idx2], trtasc[idx3],
                                jd[idx1], jdf[idx1], jd[idx2], jdf[idx2], jd[idx3], jdf[idx3],
                                diffsites, rseci1, rseci2, rseci3, out r2, out v2, out errstr);
                    strbuild.AppendLine(errstr);
                    strbuild.AppendLine("r2 " + r2[0].ToString("0.000000") + " " +
                        r2[1].ToString("0.000000") + " " + r2[2].ToString("0.000000")
                        + "v2 " + v2[0].ToString("0.000000") + " " +
                        v2[1].ToString("0.000000") + " " + v2[2].ToString("0.000000"));
                    AstroLibr.rv2coe(r2, v2, out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out arglat, out truelon, out lonper);
                    strbuild.AppendLine("\nlaplace coes a= " + a.ToString("0.0000") + " e= " + ecc.ToString("0.000000000") + " i= " + (incl * rad).ToString("0.0000") + " " +
                        (raan * rad).ToString("0.0000") + " " + (argp * rad).ToString("0.0000") + " " + (nu * rad).ToString("0.0000") + " " + (m * rad).ToString("0.0000") + " " +
                        (arglat * rad).ToString("0.0000")); // + " " + (truelon * rad).ToString("0.0000") + " " + (lonper * rad).ToString("0.0000"));
                    strbuild.AppendLine(ans);
                    strbuildall.AppendLine("laplace coes a= " + a.ToString("0.0000") + " e= " + ecc.ToString("0.000000000") + " i= " + (incl * rad).ToString("0.0000") + " " +
                        (raan * rad).ToString("0.0000") + " " + (argp * rad).ToString("0.0000") + " " + (nu * rad).ToString("0.0000") + " " + (m * rad).ToString("0.0000") + " " +
                        (arglat * rad).ToString("0.0000")); // + " " + (truelon * rad).ToString("0.0000") + " " + (lonper * rad).ToString("0.0000"));
                    strbuildall.AppendLine(ans);

                    strbuildall.AppendLine("Gauss  -----------------------------------");
                    strbuild.AppendLine("Gauss  -----------------------------------");
                    if (caseopt == 23)
                    {  // curtis example -many mistakes!
                        rseci1 = new double[] { 3489.8, 3430.2, 4078.5 };
                        rseci2 = new double[] { 3460.1, 3460.1, 4078.5 };
                        rseci3 = new double[] { 3429.9, 3490.1, 4078.5 };
                    }
                    AstroLibr.anglesgauss(tdecl[idx1], tdecl[idx2], tdecl[idx3], trtasc[idx1], trtasc[idx2], trtasc[idx3],
                         jd[idx1], jdf[idx1], jd[idx2], jdf[idx2], jd[idx3], jdf[idx3],
                         rseci1, rseci2, rseci3, out r2, out v2, out errstr);
                    strbuild.AppendLine(errstr);
                    strbuild.AppendLine("r2 " + r2[0].ToString("0.000000") + " " +
                         r2[1].ToString("0.000000") + " " + r2[2].ToString("0.000000")
                         + "v2 " + v2[0].ToString("0.000000") + " " +
                         v2[1].ToString("0.000000") + " " + v2[2].ToString("0.000000"));
                    AstroLibr.rv2coe(r2, v2, out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out arglat, out truelon, out lonper);
                    strbuild.AppendLine("gauss coes a= " + a.ToString("0.0000") + " e= " + ecc.ToString("0.000000000") + " i= " + (incl * rad).ToString("0.0000") + " " +
                         (raan * rad).ToString("0.0000") + " " + (argp * rad).ToString("0.0000") + " " + (nu * rad).ToString("0.0000") + " " + (m * rad).ToString("0.0000") + " " +
                         (arglat * rad).ToString("0.0000")); // + " " + (truelon * rad).ToString("0.0000") + " " + (lonper * rad).ToString("0.0000"));
                    strbuild.AppendLine(ans);
                    strbuildall.AppendLine("gauss coes a= " + a.ToString("0.0000") + " e= " + ecc.ToString("0.000000000") + " i= " + (incl * rad).ToString("0.0000") + " " +
                         (raan * rad).ToString("0.0000") + " " + (argp * rad).ToString("0.0000") + " " + (nu * rad).ToString("0.0000") + " " + (m * rad).ToString("0.0000") + " " +
                         (arglat * rad).ToString("0.0000")); // + " " + (truelon * rad).ToString("0.0000") + " " + (lonper * rad).ToString("0.0000"));
                    strbuildall.AppendLine(ans);

                    strbuildall.AppendLine("Double-r -----------------------------------");
                    strbuild.AppendLine("Double-r -----------------------------------");
                    // initial guesses needed for double-r and Gooding
                    // use result from Gauss as it's usually pretty good
                    // this seems to really help Gooding!!
                    getGaussRoot(tdecl[idx1], tdecl[idx2], tdecl[idx3], trtasc[idx1], trtasc[idx2], trtasc[idx3],
                         jd[idx1], jdf[idx1], jd[idx2], jdf[idx2], jd[idx3], jdf[idx3],
                         rseci1, rseci2, rseci3, out bigr2x);
                    initguess[caseopt] = bigr2x;

                    rng1 = initguess[caseopt];  // old 12500 needs to be in km!! seems to do better when all the same? if too far off (*2) NAN
                    rng2 = initguess[caseopt] * 1.02;  // 1.02 might be better? make the initial guess a bit different
                    rng3 = initguess[caseopt] * 1.08;
                    AstroLibr.anglesdoubler(tdecl[idx1], tdecl[idx2], tdecl[idx3], trtasc[idx1], trtasc[idx2], trtasc[idx3],
                         jd[idx1], jdf[idx1], jd[idx2], jdf[idx2], jd[idx3], jdf[idx3],
                         rseci1, rseci2, rseci3, rng1, rng2, out r2, out v2, out errstr);
                    strbuild.AppendLine(errstr);
                    AstroLibr.rv2coe(r2, v2, out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out arglat, out truelon, out lonper);
                    strbuild.AppendLine("doubler coes a= " + a.ToString("0.0000") + " e= " + ecc.ToString("0.000000000") + " i= " + (incl * rad).ToString("0.0000") + " " +
                        (raan * rad).ToString("0.0000") + " " + (argp * rad).ToString("0.0000") + " " + (nu * rad).ToString("0.0000") + " " + (m * rad).ToString("0.0000") + " " +
                        (arglat * rad).ToString("0.0000")); // + " " + (truelon * rad).ToString("0.0000") + " " + (lonper * rad).ToString("0.0000"));
                    strbuild.AppendLine(ans);
                    strbuildall.AppendLine("doubler coes a= " + a.ToString("0.0000") + " e= " + ecc.ToString("0.000000000") + " i= " + (incl * rad).ToString("0.0000") + " " +
                        (raan * rad).ToString("0.0000") + " " + (argp * rad).ToString("0.0000") + " " + (nu * rad).ToString("0.0000") + " " + (m * rad).ToString("0.0000") + " " +
                        (arglat * rad).ToString("0.0000")); // + " " + (truelon * rad).ToString("0.0000") + " " + (lonper * rad).ToString("0.0000"));
                    strbuildall.AppendLine(ans);

                    strbuildall.AppendLine("Gooding -----------------------------------");
                    strbuild.AppendLine("Gooding -----------------------------------");
                    numhalfrev = 0;
                    AstroLibr.anglesgooding(tdecl[idx1], tdecl[idx2], tdecl[idx3], trtasc[idx1], trtasc[idx2], trtasc[idx3],
                        jd[idx1], jdf[idx1], jd[idx2], jdf[idx2], jd[idx3], jdf[idx3],
                        rseci1, rseci2, rseci3, numhalfrev, rng1, rng2, rng3, out r2, out v2, out errstr);
                    strbuild.AppendLine(errstr);
                    AstroLibr.rv2coe(r2, v2, out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out arglat, out truelon, out lonper);
                    strbuild.AppendLine("gooding coes  a= " + a.ToString("0.0000") + " e= " + ecc.ToString("0.000000000") + " i= " + (incl * rad).ToString("0.0000") + " " +
                        (raan * rad).ToString("0.0000") + " " + (argp * rad).ToString("0.0000") + " " + (nu * rad).ToString("0.0000") + " " + (m * rad).ToString("0.0000") + " " +
                        (arglat * rad).ToString("0.0000")); // + " " + (truelon * rad).ToString("0.0000") + " " + (lonper * rad).ToString("0.0000"));
                    strbuild.AppendLine(ans);
                    strbuildall.AppendLine("gooding coes  a= " + a.ToString("0.0000") + " e= " + ecc.ToString("0.000000000") + " i= " + (incl * rad).ToString("0.0000") + " " +
                        (raan * rad).ToString("0.0000") + " " + (argp * rad).ToString("0.0000") + " " + (nu * rad).ToString("0.0000") + " " + (m * rad).ToString("0.0000") + " " +
                        (arglat * rad).ToString("0.0000")); // + " " + (truelon * rad).ToString("0.0000") + " " + (lonper * rad).ToString("0.0000"));
                    strbuildall.AppendLine(ans);

//                }  // loop through cases of caseopt = 0

            } // caseopt

            string directory = @"D:\Codes\LIBRARY\cs\TestAll\";
            File.WriteAllText(directory + "testallAngles.out", strbuild.ToString());
            File.WriteAllText(directory + "testallAnglessum.out", strbuildall.ToString());

            this.opsStatus.Text = "Test Angles - Done";
            Refresh();

        }   // testangles



        // test uct
        // store all the obs
        public class obsClass
        {
     //       public Int32 numtracks;
            public double jd;
            public double jdf;
            public double mjd;
            public string datestr;
            public Int32 sennum;
            public Int32 sscnum;
            public double rtasc; 
            public double decl; 
            public double lat; 
            public double lon; 
            public double alt;
            public double passlen;
            public double[] rseci = new double[3];
        };

        // store all the tracks
        public class trackClass
        {
            public Int32 numobstk;
            public obsClass[] tracks = new obsClass[50000];
        };


        private static void ShowTable(DataTable table, out string strout)
        {
            StringBuilder strbuild = new StringBuilder();

            foreach (DataColumn col in table.Columns)
            {
                strbuild.Append(String.Format("{0,-14}", col.ColumnName));
            }
            strbuild.AppendLine();

            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn col in table.Columns)
                {
                    if (col.DataType.Equals(typeof(DateTime)))
                        strbuild.Append(String.Format("{0,-14:d} ", row[col]));
                    else if (col.DataType.Equals(typeof(Decimal)))
                        strbuild.Append(String.Format("{0,-14:C} ", row[col]));
                    else
                        strbuild.Append(String.Format("{0,-14} ", row[col]));
                }
                strbuild.AppendLine();
            }

            strout = strbuild.ToString();
        }

        public void MHTangles
            (
            )
        {
            StringBuilder strbuild = new StringBuilder();
            StringBuilder strbuildObs = new StringBuilder();
            string strout;

            //double mjd;
            double[] rseci = new double[3];
            double[] rseci1 = new double[3];
            double[] rseci2 = new double[3];
            double[] rseci3 = new double[3];
            double[] vseci = new double[3];
            double[] rsecef = new double[3];
            double[] vsecef = new double[3];
            double[] ro = new double[3];
            double[] vo = new double[3];
            double[] r1 = new double[3];
            double[] v1 = new double[3];
            double[] r2l = new double[3];
            double[] v2l = new double[3];
            double[] r2g = new double[3];
            double[] v2g = new double[3];
            double[] r2d = new double[3];
            double[] v2d = new double[3];
            double[] r2o = new double[3];
            double[] v2o = new double[3];
            double[] r3 = new double[3];
            double[] v3 = new double[3];
            double[] r4 = new double[3];
            double[] v4 = new double[3];
            double[] dr1 = new double[3];
            double[] dv1 = new double[3];
            double[] dr2 = new double[3];
            double[] dv2 = new double[3];
            double obsvalue, second, rad, bigr2x;
            double pl, al, eccl, incll, raanl, argpl, nul, ml, arglatl, truelonl, lonperl;
            double pg, ag, eccg, inclg, raang, argpg, nug, mg, arglatg, truelong, lonperg;
            double pd, ad, eccd, incld, raand, argpd, nud, md, arglatd, truelond, lonperd;
            double po, ao, ecco, inclo, raano, argpo, nuo, mo, arglato, truelono, lonpero;
            double rtasc1, decl1, rtasc2, decl2, rtasc3, decl3;
            double lat, lon, alt;
            int year; int month; int day; int hr; int minute;
            string monstr;
            string errstr;
            string infilename, obstype;
            Int32 numobs, aa, bb, cc;
            // collect all the permutations. this will vary with each problem
            List<string> possibilities = new List<string>();
            List<string> strArr = new List<string>();
            string[] monthStr = new string[13] { "", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

            rad = 180.0 / Math.PI;
            Int32 i = 0;  // keep track of all entries
            numobs = 0;

            // setup structure to hold all the multiple obs 
            DataTable obsTable = new DataTable("obsTable");
            DataRow orow;
            // Create first column and add to the DataTable.
            DataColumn[] ocols =
            {
                new DataColumn("jd", typeof(double)),
                new DataColumn("jdf", typeof(double)),
                new DataColumn("datestr", typeof(string)),
                new DataColumn("sennum", typeof(Int32)),
                new DataColumn("sscnum", typeof(Int32)),
                new DataColumn("rtasc", typeof(double)),
                new DataColumn("decl", typeof(double)),
                new DataColumn("lat", typeof(double)),
                new DataColumn("lon", typeof(double)),
                new DataColumn("alt", typeof(double)),
                new DataColumn("passlen", typeof(double)),
                new DataColumn("rseci[0]",typeof(double)),
                new DataColumn("rseci[1]",typeof(double)),
                new DataColumn("rseci[2]",typeof(double)),
            };
            obsTable.Columns.AddRange(ocols);

            // setup structure to hold all the data, sort, etc. 
            DataTable satTable = new DataTable("satTable");
            DataRow row;
            // Create first column and add to the DataTable.
            DataColumn[] cols =
            {
                // association - leave blank for now
                //new DataColumn("sscnum",typeof(Int32))
                new DataColumn("poss", typeof(string)),
                new DataColumn("al", typeof(double)),
                new DataColumn("ag", typeof(double)),
                new DataColumn("ad", typeof(double)),
                new DataColumn("ao", typeof(double)),
                new DataColumn("eccl", typeof(double)),
                new DataColumn("eccg", typeof(double)),
                new DataColumn("eccd", typeof(double)),
                new DataColumn("ecco", typeof(double)),
                new DataColumn("incll", typeof(double)),
                new DataColumn("inclg", typeof(double)),
                new DataColumn("incld", typeof(double)),
                new DataColumn("inclo", typeof(double)),
                new DataColumn("r2l[0]",typeof(double)),
                new DataColumn("r2l[1]",typeof(double)),
                new DataColumn("r2l[2]",typeof(double)),
                new DataColumn("r2g[0]",typeof(double)),
                new DataColumn("r2g[1]",typeof(double)),
                new DataColumn("r2g[2]",typeof(double)),
                new DataColumn("r2d[0]",typeof(double)),
                new DataColumn("r2d[1]",typeof(double)),
                new DataColumn("r2d[2]",typeof(double)),
                new DataColumn("r2o[0]", typeof(double)),
                new DataColumn("r2o[1]", typeof(double)),
                new DataColumn("r2o[2]", typeof(double)),
                new DataColumn("v2l[0]",typeof(double)),
                new DataColumn("v2l[1]",typeof(double)),
                new DataColumn("v2l[2]",typeof(double)),
                new DataColumn("v2g[0]",typeof(double)),
                new DataColumn("v2g[1]",typeof(double)),
                new DataColumn("v2g[2]",typeof(double)),
                new DataColumn("v2d[0]",typeof(double)),
                new DataColumn("v2d[1]",typeof(double)),
                new DataColumn("v2d[2]",typeof(double)),
                new DataColumn("v2o[0]", typeof(double)),
                new DataColumn("v2o[1]", typeof(double)),
                new DataColumn("v2o[2]", typeof(double))
            };
            satTable.Columns.AddRange(cols);

            this.opsStatus.Text = "Test Test MHTAngles (UCT) ";
            Refresh();

            // -------------------------------- read EOP data in ----------------------------------
            int dat;
            double dut1, lod, xp, yp, ddpsi, ddeps, ddx, ddy;
            double ttt, jdut1;

            string nutLoc;
            Int32 ktrActObs;
            string EOPupdate;
            Int32 mjdeopstart;
            EOPSPWLib.iau80Class iau80arr;
            EOPSPWLib.iau00Class iau00arr;
            nutLoc = @"D:\Codes\LIBRARY\DataLib\nut80.dat";
            EOPSPWLibr.iau80in(nutLoc, out iau80arr);
            nutLoc = @"D:\Codes\LIBRARY\DataLib\";
            EOPSPWLibr.iau00in(nutLoc, out iau00arr);

            string eopFileName = @"D:\Codes\LIBRARY\DataLib\EOP-All-v1.1_2021-07-28.txt";
            EOPSPWLibr.readeop(ref EOPSPWLibr.eopdata, eopFileName, out mjdeopstart, out ktrActObs, out EOPupdate);

            double jdxysstart, jdfxysstart;
            AstroLib.xysdataClass[] xysarr = AstroLibr.xysarr;
            AstroLibr.initXYS(ref xysarr, nutLoc, "xysdata.dat", out jdxysstart, out jdfxysstart);


            // ---------------------------------- read all obs in ----------------------------------
            // two formats exist
            // 1. text format, each value is on a separate line. can be 2, 3, 4 etc that are the same time
            // 15 Jul 2021 00:36:11.099000  6217        25509         30.236400 -5.608580  0.674 324.6182556  deg Right Ascension
            // 15 Jul 2021 00:36:11.099000  6217        25509         30.236400 -5.608580  0.674 324.6422729  deg Right Ascension
            // 15 Jul 2021 00:36:11.099000  6217        25509         30.236400 -5.608580  0.674  -4.9919863  deg Declination
            // 15 Jul 2021 00:36:11.099000  6217        25509         30.236400 -5.608580  0.674  -4.9705443  deg Declination
            // 15 Jul 2021 00:35:28.493000  6217        29945         30.236400 -5.608580  0.674 324.4261169  deg Right Ascension
            // 15 Jul 2021 00:35:28.493000  6217        29945         30.236400 -5.608580  0.674  -5.0073156  deg Declination

            // 2. comma delimited, all values for a time are on 1 line. can be 3, 4 etc that are the same time
            // 3,2021-07-25T12:29:25.312000,JTMA-013,99999,0,1,198.9839478,5.1738567,12.92403
            // 3,2021-07-25T12:29:25.562000,JTMA-013,99999,0,1,199.2442474,4.5760307,11.824894
            // 3,2021-07-25T12:29:25.562000,JTMA-013,99999,0,1,198.9852905,5.1737432,12.966325
            // 3,2021-07-25T12:29:25.562000,JTMA-013,99999,0,1,199.0706329,5.0698462,11.361035

            this.opsStatus.Text = "Reading in obs ";
            Refresh();
            // make sure site vectors are converted to ECI
            // assign number to each obs to differentiate at the end
            
            // 1. this is supposed to be the astra cluster (GEO)
            // this has lots of duplicate times/obs
            infilename = @"D:\datafile\CSSI\COMSPOC\UCT\allobs.dat";
            
            // 2. astra cluster JTMA original file, no mutliple obs at same time same object, but only 3 tracks dies on diff rates
            //infilename = @"D:\datafile\CSSI\COMSPOC\UCT\JTMA-S013-20210725T122924.jtm";
            
            // 3. some random ucts from the system
            // this has lots of duplicate times/obs in jtm format
            infilename = @"D:\datafile\CSSI\COMSPOC\UCT\GSAT_18.jtm";
            
            // 4. new object that mfast failed on? lots of tracks, some duplicate times
            //infilename = @"D:\datafile\CSSI\COMSPOC\UCT\MT_Sat_SSN_49115_1.jtm";

            string[] fileData = File.ReadAllLines(infilename);

            obsClass[] obsClassArr = new obsClass[50000];

            string pattern = @"^\s+(\S+)\s+(\S+)\s+(\S+)\s+(\d{2}):(\d{2}):(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)";
            string[] currrec = new string[10];  // 10 obs at a single time
            string[] linedata = new string[20];

            // 1 is gobs format
            // 2 is jtm format
            char obsformat;
            if (Path.GetExtension(infilename).Equals(".jtm"))
                obsformat = '2';
            else
                obsformat = '1';

            char multiples = 'n';
            int nummultiples = 0;
            if (obsformat == '1')
            {
                // type 1 file (see above)
                // skip header
                for (i = 1; i < fileData.Count(); i++)
                {
                    // find how many obs at the same time
                    int j = 0;
                    do
                    {
                        j = j + 1;
                        linedata = Regex.Split(fileData[i + j], pattern);
                        currrec[j] = linedata[14];
                    } while (currrec[j] != "Declination");

                    // determine how many times are the same
                    nummultiples = j;// / 2 + 1;

                    // assign all the obs at this time
                    for (int k = 0; k < nummultiples; k++)
                    {
                        obsClassArr[numobs] = new obsClass();

                        linedata = Regex.Split(fileData[i], pattern);

                        // set new record as they are needed
                        day = Convert.ToInt32(linedata[1]);
                        monstr = linedata[2];
                        year = Convert.ToInt32(linedata[3]);
                        hr = Convert.ToInt32(linedata[4]);
                        minute = Convert.ToInt32(linedata[5]);
                        second = Convert.ToDouble(linedata[6]);
                        obsClassArr[numobs].datestr = linedata[1] + ' ' + linedata[2] + ' ' + linedata[3]
                            + ' ' + linedata[4] + ' ' + linedata[5] + ' ' + linedata[6];
                        MathTimeLibr.jday(year, MathTimeLibr.convertMonth(monstr), day, hr, minute, second,
                            out obsClassArr[numobs].jd, out obsClassArr[numobs].jdf);
                        obsClassArr[numobs].mjd = obsClassArr[numobs].jd + obsClassArr[numobs].jdf - 2400000.5;
                        obsClassArr[numobs].sennum = Convert.ToInt32(linedata[7]);

                        //sscnum
                        // adjust this number so it can be keyed on (> 1e5) later for multiple satellites
                        obsClassArr[numobs].sscnum = Convert.ToInt32(linedata[8]);
                        if (nummultiples > 1)
                        {
                            obsClassArr[numobs].sscnum = obsClassArr[numobs].sscnum * 10 + k;
                            multiples = 'y';
                        }

                        obsClassArr[numobs].lat = Convert.ToDouble(linedata[9]) / rad;
                        obsClassArr[numobs].lon = Convert.ToDouble(linedata[10]) / rad;
                        obsClassArr[numobs].alt = Convert.ToDouble(linedata[11]);

                        AstroLibr.site(obsClassArr[numobs].lat, obsClassArr[numobs].lon, obsClassArr[numobs].alt, out rsecef, out vsecef);
                        EOPSPWLibr.findeopparam(obsClassArr[numobs].jd, obsClassArr[numobs].jdf, 's', EOPSPWLibr.eopdata, mjdeopstart + 2400000.5,
                            out dut1, out dat, out lod, out xp, out yp, out ddpsi, out ddeps, out ddx, out ddy);
                        double jdtt = obsClassArr[numobs].jd;
                        double jdftt = obsClassArr[numobs].jdf + (dat + 32.184) / 86400.0;
                        ttt = (jdtt + jdftt - 2451545.0) / 36525.0;
                        jdut1 = obsClassArr[numobs].jd + obsClassArr[numobs].jdf + dut1 / 86400.0;
                        AstroLibr.eci_ecef(ref rseci, ref vseci, MathTimeLib.Edirection.efrom, ref rsecef, ref vsecef,
                             AstroLib.EOpt.e80, iau80arr, iau00arr,
                             jdtt, jdftt, jdut1, jdxysstart, lod, xp, yp, ddpsi, ddeps, ddx, ddy);
                        obsClassArr[numobs].rseci[0] = rseci[0];
                        obsClassArr[numobs].rseci[1] = rseci[1];
                        obsClassArr[numobs].rseci[2] = rseci[2];

                        obsvalue = (Convert.ToDouble(linedata[12])) / rad;
                        obstype = linedata[13];
                        obsClassArr[numobs].rtasc = obsvalue;

                        linedata = Regex.Split(fileData[i + nummultiples], pattern);
                        obsvalue = (Convert.ToDouble(linedata[12])) / rad;
                        obsClassArr[numobs].decl = obsvalue;

                        // if multiple obs at same time, store to obsTable instead of obsarray for subsequent sorting and processing
                        if (nummultiples > 1)
                        {
                            Object[] rows = new Object[] { obsClassArr[numobs].jd, obsClassArr[numobs].jdf, obsClassArr[numobs].datestr,
                                obsClassArr[numobs].sennum, obsClassArr[numobs].sscnum,
                                obsClassArr[numobs].rtasc, obsClassArr[numobs].decl, obsClassArr[numobs].lat, obsClassArr[numobs].lon, obsClassArr[numobs].alt,
                                obsClassArr[numobs].passlen, obsClassArr[numobs].rseci[0], obsClassArr[numobs].rseci[1], obsClassArr[numobs].rseci[2] };

                            obsTable.Rows.Add(rows);
                        }
                        else
                            numobs = numobs + 1;

                        i = i + 1;
                    } // for k getting the obs at a single time

                    i = i + nummultiples - 1;
                }  // for i reading all obs in

           }
            else
            {
                // type 2 file (jtm csv format see above) --------------------------------------------------------
                for (i = 0; i < fileData.Count(); i++)
                {
                    // find how many obs at the same time
                    string newtime, oldtime;
                    int j = 0;
                    linedata = fileData[i + j].Split(',');
                    oldtime = linedata[1];
                    do
                    {
                        j = j + 1;
                        linedata = fileData[i + j].Split(',');
                        newtime = linedata[1];
                    } while (newtime == oldtime && i + j + 2 <= fileData.Count());

                    // determine how many times are the same
                    nummultiples = j;

                    // assign all the obs at this time
                    for (int k = 0; k < nummultiples; k++)
                    {
                        // set new record as they are needed
                        obsClassArr[numobs] = new obsClass();

                        linedata = fileData[i].Split(',');

                        string[] linedata1 = new string[3];
                        string[] linedata2 = new string[3];
                        linedata1 = linedata[1].Split('T');
                        linedata2 = linedata1[0].Split('-');
                        day = Convert.ToInt32(linedata2[2]);
                        month = Convert.ToInt32(linedata2[1]);
                        year = Convert.ToInt32(linedata2[0]);

                        linedata2 = linedata1[1].Split(':');
                        hr = Convert.ToInt32(linedata2[0]);
                        minute = Convert.ToInt32(linedata2[1]);
                        second = Convert.ToDouble(linedata2[2]);
                        obsClassArr[numobs].datestr = day.ToString() + ' ' + month.ToString() + ' ' + year.ToString()
                            + ' ' + linedata2[0] + ' ' + linedata2[1] + ' ' + linedata2[2];
                        MathTimeLibr.jday(year, month, day, hr, minute, second,
                            out obsClassArr[numobs].jd, out obsClassArr[numobs].jdf);
                        string senname = linedata[2];
                        lat = 0.0;
                        lon = 0.0;
                        alt = 0.0;
                        switch (senname)
                        {
                            case "JTMA-001":
                                lat = 35.305774 / rad;
                                lon = -106.457277 / rad;
                                alt = 1.7882;
                                obsClassArr[numobs].sennum = 6201;
                                obsClassArr[numobs].sscnum = 99999;
                                break;
                            case "JTMA-003":
                                lat = 35.305782 / rad;
                                lon = -106.457083 / rad;
                                alt = 1.7882;
                                obsClassArr[numobs].sennum = 6203;
                                obsClassArr[numobs].sscnum = 99999;
                                break;
                            case "JTMA-004":
                                lat = 35.305715 / rad;
                                lon = -106.457170 / rad;
                                alt = 1.7971;
                                obsClassArr[numobs].sennum = 6204;
                                obsClassArr[numobs].sscnum = 99999;
                                break;
                            case "JTMA-005":
                                lat = 40.037885 / rad;
                                lon = -75.596769 / rad;
                                alt = 0.078;
                                obsClassArr[numobs].sennum = 6205;
                                obsClassArr[numobs].sscnum = 99999;
                                break;
                            case "JTMA-006":
                                lat = -32.007550 / rad;
                                lon = 116.136740 / rad;
                                alt = 0.374;
                                obsClassArr[numobs].sennum = 6206;
                                obsClassArr[numobs].sscnum = 99999;
                                break;
                            case "JTMA-007":
                                lat = -32.294010 / rad;
                                lon = 148.585270 / rad;
                                alt = 0.280;
                                obsClassArr[numobs].sennum = 6207;
                                obsClassArr[numobs].sscnum = 99999;
                                break;
                            case "JTMA-008":
                                lat = 38.165700 / rad;
                                lon = -2.326862 / rad;
                                alt = 1.654;
                                obsClassArr[numobs].sennum = 6208;
                                obsClassArr[numobs].sscnum = 99999;
                                break;
                            case "JTMA-009":
                                lat = -25.743746 / rad;
                                lon = 29.298861 / rad;
                                alt = 1.480;
                                obsClassArr[numobs].sennum = 6209;
                                obsClassArr[numobs].sscnum = 99999;
                                break;
                            case "JTMA-010":
                                lat = 30.597332 / rad;
                                lon = 34.763020 / rad;
                                alt = 0.872;
                                obsClassArr[numobs].sennum = 6210;
                                obsClassArr[numobs].sscnum = 99999;
                                break;
                            case "JTMA-011":
                                lat = 20.707100 / rad;
                                lon = -156.257467 / rad;
                                alt = 3.0703;
                                obsClassArr[numobs].sennum = 6211;
                                obsClassArr[numobs].sscnum = 99999;
                                break;
                            case "JTMA-012":
                                lat = -22.953317 / rad;
                                lon = -68.179997 / rad;
                                alt = 2.660;
                                obsClassArr[numobs].sennum = 6212;
                                obsClassArr[numobs].sscnum = 99999;
                                break;
                            case "JTMA-013":
                                lat = -31.820550 / rad;
                                lon = 117.281500 / rad;
                                alt = 0.331933;
                                obsClassArr[numobs].sennum = 6213;
                                obsClassArr[numobs].sscnum = 99999;
                                break;
                            case "JTMA-014":
                                lat = 34.291194 / rad;
                                lon = -115.382611 / rad;
                                alt = 0.884987;
                                obsClassArr[numobs].sennum = 6214;
                                obsClassArr[numobs].sscnum = 99999;
                                break;
                            case "JTMA-015":
                                lat = -23.771000 / rad;
                                lon = 133.919000 / rad;
                                alt = 0.884987;
                                obsClassArr[numobs].sennum = 6215;
                                obsClassArr[numobs].sscnum = 99999;
                                break;
                            case "JTMA-016":
                                lat = 20.900587 / rad;
                                lon = -156.504102 / rad;
                                alt = 0.227;
                                obsClassArr[numobs].sennum = 6216;
                                obsClassArr[numobs].sscnum = 99999;
                                break;
                            case "JTMA-017":
                                lat = 30.236 / rad;
                                lon = -5.609 / rad;
                                alt = 0.674;
                                obsClassArr[numobs].sennum = 6217;
                                obsClassArr[numobs].sscnum = 99999;
                                break;
                        }

                        // obsClassArr[numobs].sennum = Convert.ToInt32(linedata[7]);
                        // obsClassArr[numobs].sscnum = Convert.ToInt32(linedata[8]);
                        //sscnum;
                        if (nummultiples > 1)
                        {
                            obsClassArr[numobs].sscnum = obsClassArr[numobs].sscnum * 10 + k;
                            multiples = 'y';
                        }

                        obsClassArr[numobs].lat = lat;
                        obsClassArr[numobs].lon = lon;
                        obsClassArr[numobs].alt = alt;

                        AstroLibr.site(obsClassArr[numobs].lat, obsClassArr[numobs].lon, obsClassArr[numobs].alt, out rsecef, out vsecef);
                        EOPSPWLibr.findeopparam(obsClassArr[numobs].jd, obsClassArr[numobs].jdf, 's', EOPSPWLibr.eopdata, mjdeopstart + 2400000.5,
                            out dut1, out dat, out lod, out xp, out yp, out ddpsi, out ddeps, out ddx, out ddy);
                        double jdtt = obsClassArr[numobs].jd;
                        double jdftt = obsClassArr[numobs].jdf + (dat + 32.184) / 86400.0;
                        ttt = (jdtt + jdftt - 2451545.0) / 36525.0;
                        jdut1 = obsClassArr[numobs].jd + obsClassArr[numobs].jdf + dut1 / 86400.0;
                        AstroLibr.eci_ecef(ref rseci, ref vseci, MathTimeLib.Edirection.efrom, ref rsecef, ref vsecef,
                             AstroLib.EOpt.e80, iau80arr, iau00arr,
                             jdtt, jdftt, jdut1, jdxysstart, lod, xp, yp, ddpsi, ddeps, ddx, ddy);
                        obsClassArr[numobs].rseci[0] = rseci[0];
                        obsClassArr[numobs].rseci[1] = rseci[1];
                        obsClassArr[numobs].rseci[2] = rseci[2];

                        obsvalue = (Convert.ToDouble(linedata[6])) / rad;
                        obsClassArr[numobs].rtasc = obsvalue;
                        obsvalue = (Convert.ToDouble(linedata[7])) / rad;
                        obsClassArr[numobs].decl = obsvalue;

                        // if multiple obs at same time, store to obsTable instead of obsarray for subsequent sorting and processing
                        if (nummultiples > 1)
                        {
                            Object[] rows = new Object[] { obsClassArr[numobs].jd, obsClassArr[numobs].jdf, obsClassArr[numobs].datestr,
                                obsClassArr[numobs].sennum, obsClassArr[numobs].sscnum,
                                obsClassArr[numobs].rtasc, obsClassArr[numobs].decl, obsClassArr[numobs].lat, obsClassArr[numobs].lon, obsClassArr[numobs].alt,
                                obsClassArr[numobs].passlen, obsClassArr[numobs].rseci[0], obsClassArr[numobs].rseci[1], obsClassArr[numobs].rseci[2] };

                            obsTable.Rows.Add(rows);
                        }
                        else
                            numobs = numobs + 1;
                        i = i + 1;
                    }  // for k getting the obs at a single time

                    // get ready for next line, after duplicates
                    //i = i - 1;
                }  // for i reading all obs in

            }  // if type 2


            // add any duplicate time obs into the mix at the end of the other obs
            numobs = numobs - 1;
            strbuildObs.AppendLine("Reading in obs " + numobs.ToString());
            this.opsStatus.Text = "Reading in obs " + numobs.ToString();
            Refresh();

            //Int32 z = 0;
            if (multiples == 'y')
            {
                this.opsStatus.Text = "Reading in obs - process duplicates  ";
                Refresh();
                // now do sort on col for changed sscnum
                DataView doo = obsTable.DefaultView;
                doo.Sort = "sscnum";
                DataTable sortedobsDT = doo.ToTable();

                ShowTable(sortedobsDT, out strout);
                pattern = @"^s?(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\d{2})\s+(\d{2})\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)";

                string[] matches = Regex.Split(strout, @"\r\n");

                this.opsStatus.Text = "Reading in obs " + numobs.ToString() + " " + matches.Count().ToString();
                Refresh();
                strbuildObs.AppendLine("Reading in obs " + numobs.ToString() + " " + matches.Count().ToString());

                for (int zz = 0; zz < matches.Count() - 2; zz++)
                {
                    linedata = Regex.Split(matches[zz + 1], pattern);

                    // be sure to update total number of obs!
                    numobs = numobs + 1;
                    obsClassArr[numobs] = new obsClass();

                    obsClassArr[numobs].jd = Convert.ToDouble(linedata[1]);
                    obsClassArr[numobs].jdf = Convert.ToDouble(linedata[2]);
                    obsClassArr[numobs].datestr = linedata[3] + " " + linedata[4] + " " + linedata[5] + " "
                        + linedata[6] + ":" + linedata[7] + ":" + linedata[8];
                    obsClassArr[numobs].sennum = Convert.ToInt32(linedata[9]);
                    obsClassArr[numobs].sscnum = Convert.ToInt32(linedata[10]);
                    obsClassArr[numobs].rtasc = Convert.ToDouble(linedata[11]);
                    obsClassArr[numobs].decl = Convert.ToDouble(linedata[12]);
                    obsClassArr[numobs].lat = Convert.ToDouble(linedata[13]);
                    obsClassArr[numobs].lon = Convert.ToDouble(linedata[14]);
                    obsClassArr[numobs].alt = Convert.ToDouble(linedata[15]);
                    obsClassArr[numobs].passlen = Convert.ToInt32(linedata[16]);
                    obsClassArr[numobs].rseci[0] = Convert.ToDouble(linedata[17]);
                    obsClassArr[numobs].rseci[1] = Convert.ToDouble(linedata[18]);
                    obsClassArr[numobs].rseci[2] = Convert.ToDouble(linedata[19]);

                    //strbuildObs.AppendLine(matches[zz]);
                    //z = z + 1;
                }

            }  // if multiples


            // ---------------------------------------- find tracks --------------------------------------------
            // first have to check if obs at same time but different rtacs/decl
            // separate these by the sscnum that was set larger than 1e5 when read in
            //DataView doobs = obsTable.DefaultView;
            //            dv.Sort = "ad";
            //            dv.Sort = "ag";
            //DataTable sortedDT = doobs.ToTable();

            //ShowTable(sortedDT, out strout);
            // File.WriteAllText(@"D:\Codes\LIBRARY\cs\ConjunctionSieve\test.out", strout);

            this.opsStatus.Text = "Finding tracks";
            Refresh();

            // determine initial processing of tracks - do by time since when obs time changes from previous deltas, it's a new track
            // - could also do by plotting via rtasc-decl space
            trackClass[] trackClassArr = new trackClass[50000];
            double drtasc, ddecl, dtime, olddtime;
            Int32 obsktr, totalTracks, trknumktr;
            Int32 obsintrk;
            string tempstr, oldtempstr;
            tempstr = "";
            olddtime = 0.0;
            dtime = 0.0;
            totalTracks = 0;
            obsintrk = 0;
            trackClassArr[totalTracks] = new trackClass();
            for (obsktr = 0; obsktr < numobs - 1; obsktr++)
            {
                olddtime = dtime;
                oldtempstr = tempstr;
                tempstr = (obsClassArr[obsktr].jd - 2400000.5 + obsClassArr[obsktr].jdf).ToString()
                    + " " + (obsClassArr[obsktr].rtasc * rad).ToString() + " " + (obsClassArr[obsktr].decl * rad).ToString()
                    + " " + obsClassArr[obsktr].sennum.ToString() + " " + obsClassArr[obsktr].sscnum.ToString()
                    + " " + obsClassArr[obsktr].datestr;
                
                // test existing where ssc number differs
                // this is a test only if sscs are already available
                //                if (obsktr > 1)
                //                    if (obsClassArr[obsktr].sscnum - obsClassArr[obsktr - 1].sscnum != 0)
                //                    {
                //                        dtime = obsClassArr[obsktr].jd - obsClassArr[obsktr - 1].jd + obsClassArr[obsktr].jdf - obsClassArr[obsktr - 1].jdf;
                ////                        strbuildObs.AppendLine(oldtempstr);
                ////                        strbuildObs.AppendLine(tempstr + " " + dtime.ToString());
                //                    }

                // test with delta time changing
                // JTM passes seem to be about 3-4 secs in length. Don't bother doing IOD on them within a track
                if (obsktr > 0)
                {
                    // find time delta from last obs
                    dtime = obsClassArr[obsktr].jd - obsClassArr[obsktr - 1].jd + obsClassArr[obsktr].jdf - obsClassArr[obsktr - 1].jdf;
                    // chk if curr obs is before last obs
                    // dtime seems to find when the inter-track spacing changes (more than about 8.64 sec)
                    if (obsClassArr[obsktr - 1].jd == obsClassArr[obsktr].jd && obsClassArr[obsktr - 1].jdf - obsClassArr[obsktr].jdf > 0.0
                        || dtime - olddtime > 0.0001)
                    //    || dtime > 0.001)
                    {
                        obsClassArr[obsktr].passlen = obsClassArr[obsktr-1].jd - obsClassArr[obsktr- obsintrk].jd 
                            + obsClassArr[obsktr-1].jdf - obsClassArr[obsktr- obsintrk].jdf;
                        obsClassArr[obsktr].passlen = obsClassArr[obsktr].passlen * 86400.0;
                        //strbuildObs.AppendLine("Pass length = " + obsClassArr[obsktr].passlen.ToString() + " sec, obs in pass " 
                        //    + obsintrk.ToString() + "\n");
                        strbuildObs.AppendLine("\n");
                        trackClassArr[totalTracks].numobstk = obsintrk;
                        obsintrk = 0;
                        totalTracks = totalTracks + 1;
                        trackClassArr[totalTracks] = new trackClass();
                    }
                }

                strbuildObs.AppendLine(totalTracks.ToString() + " " + tempstr); // + " " + olddtime.ToString() + " " + dtime.ToString());

                // assign new track obs info
                trackClassArr[totalTracks].tracks[obsintrk] = obsClassArr[obsktr];
                obsintrk = obsintrk + 1;
            } // through all the obs

            // now check for obs that are 1 or 2 long with additional tracks
            // these will be at the start so you don't have to check from 0 tracks each time
            // only for gobs format after through SSS
            for (trknumktr = 0; trknumktr < totalTracks; trknumktr++)
            {
                // tracks goes from 0, and to 1 less
                if (trackClassArr[trknumktr].numobstk <= 2)
                {
                    // search the other tracks to see if it goes with them
                    for (int z = trknumktr + 1; z < totalTracks; z++)
                    {
                        // check if close in time
                        if (trackClassArr[trknumktr].tracks[0].mjd - trackClassArr[z].tracks[0].mjd < 0.001)
                        {
                            if (trackClassArr[trknumktr].tracks[0].rtasc - trackClassArr[z].tracks[0].rtasc < 0.001 
                                && trackClassArr[trknumktr].tracks[0].decl - trackClassArr[z].tracks[0].decl < 0.001)
                            {
                                // insert this obs into the larger track


















                            }
                        }
                    }
                }
            }
            //File.WriteAllText(@"D:\datafile\CSSI\COMSPOC\UCT\testxobs.out", strbuildObs.ToString());


            // -------------------------- test tracks rtasc/decl differential rates -------------------------------------
            this.opsStatus.Text = "Test track consistency ";
            Refresh();
            // now examine each track and determine if all the obs belong to it
            // since the obs will be very closely spaced, look at the rtasc/decl rates of change and compare
            double pctrtasc, pctdecl, drtasco, ddeclo;
            drtasco = 0.0;
            ddeclo = 0.0;
            drtasc = 0.0;
            ddecl = 0.0;
            pctrtasc = 0.0;
            pctdecl = 0.0;
            for (trknumktr = 0; trknumktr < totalTracks - 1; trknumktr++)
            {
                // tracks goes from 0, and to 1 less
                for (Int32 ktrtk = 0; ktrtk < trackClassArr[trknumktr].numobstk-1; ktrtk++)
                {
                    // test as a % change to next value
                    //drtasc = (trackClassArr[trknumktr].tracks[ktrtk].rtasc - trackClassArr[trknumktr].tracks[ktrtk + 1].rtasc)
                    //    / trackClassArr[trknumktr].tracks[ktrtk].rtasc;
                    //ddecl = (trackClassArr[trknumktr].tracks[ktrtk].decl - trackClassArr[trknumktr].tracks[ktrtk + 1].decl)
                    //    / trackClassArr[trknumktr].tracks[ktrtk].decl;

                    // form angular rates with next obs
                    double dt = trackClassArr[trknumktr].tracks[ktrtk].jd + trackClassArr[trknumktr].tracks[ktrtk].jdf
                           - trackClassArr[trknumktr].tracks[ktrtk + 1].jd - trackClassArr[trknumktr].tracks[ktrtk + 1].jdf;
                    dt = dt * 86400.0;  // sec, not days!

                    drtasco = drtasc;
                    ddeclo = ddecl;

                    drtasc = (trackClassArr[trknumktr].tracks[ktrtk].rtasc - trackClassArr[trknumktr].tracks[ktrtk + 1].rtasc) / dt;
                    ddecl = (trackClassArr[trknumktr].tracks[ktrtk].decl - trackClassArr[trknumktr].tracks[ktrtk + 1].decl) / dt;

                    // store previous values
                    //if (trknumktr > 0)
                    //{
                        pctrtasc = (drtasc - drtasco) / drtasco;
                        pctdecl = (ddecl - ddeclo) / ddeclo;
                    //}
                    //else
                    //{
                    //}


                    // test angular rates out with assumed range - from Gauss?
                    //AstroLibr.rv_radec(ref reci, ref veci, MathTimeLib.Edirection.efrom, ref rr, ref rtasc, ref decl, ref drr, ref drtasc, ref ddecl);

                    tempstr = trknumktr.ToString() + " "
                        + (trackClassArr[trknumktr].tracks[ktrtk].jd - 2400000.0 + trackClassArr[trknumktr].tracks[ktrtk].jdf).ToString()
                        + " " + (drtasc * rad).ToString() + " " + (ddecl * rad).ToString()
                        + " " + trackClassArr[trknumktr].tracks[ktrtk].sennum.ToString() + " " + trackClassArr[trknumktr].tracks[ktrtk].sscnum.ToString()
                        + " " + trackClassArr[trknumktr].tracks[ktrtk].datestr + " % " + pctrtasc.ToString("0.####") + " " + pctdecl.ToString("0.####");
                    strbuildObs.AppendLine(tempstr);
                }

                strbuildObs.AppendLine("\n");
            } // for through all the tracks testing rtasc/decl rates
            //File.WriteAllText(@"D:\datafile\CSSI\COMSPOC\UCT\testxobs.out", strbuildObs.ToString());


            // ---------------------------------------- test each track ---------------------------------------------
            // do this within the tracks established before if they're long enough
            // make sure each track is longer than about 1 minute though!!
            this.opsStatus.Text = "Create triplets within each track ";
            Refresh();
            //string whichApp = "seq"; 
            string whichApp = "all"; 
            string numCode;
            Int32 numtrials = 0;

            // ---- find just sequential sets
            // test each track beginining, middle, end and 1/3 in ... if enough obs
            // 0 skip for now, totalTracks - 2 xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            // move to test track middle points as they are farther apart
            for (trknumktr = totalTracks; trknumktr < totalTracks - 2; trknumktr++)  
            {
                if (trackClassArr[trknumktr].numobstk > 3)
                {
                    //aa = (int)Math.Floor(trackClassArr[trknumktr].numobstk * 0.5);
                    bb = (int)Math.Floor(trackClassArr[trknumktr].numobstk * 0.5);
                    cc = trackClassArr[trknumktr].numobstk - 1;
                    if (trknumktr == 0)
                        numCode = trknumktr + " 2 " + bb + " " + cc;
                    else
                        numCode = trknumktr + " 1 " + bb + " " + cc;


                    if (!possibilities.Exists(element => element.Equals(numCode)))
                        possibilities.Add(numCode);
                    numtrials = numtrials + 1;

                    // try a second test if there are enough obs
                    if (trackClassArr[trknumktr].numobstk > 8)
                    {
                        bb = (int)Math.Floor(trackClassArr[trknumktr].numobstk * 0.5);
                        cc = trackClassArr[trknumktr].numobstk - 3;
                        numCode = trknumktr + " 3 " + bb + " " + cc;

                        if (!possibilities.Exists(element => element.Equals(numCode)))
                            possibilities.Add(numCode);
                        numtrials = numtrials + 1;
                    }

                }

                strbuildObs.AppendLine("\n");
            }  // for testing through each track




            // ------------------------------ create triplets with tracks, ntoobs within a track -------------------
            // do this within the tracks established before!!
            this.opsStatus.Text = "Create triplets from " + totalTracks.ToString() + " tracks";
            Refresh();
            Int32 obs2skip;
            if (totalTracks < 6)
                obs2skip = 1;
            else
                obs2skip = 2;

            char processtracks = 'y';

            // assign strings for each combination
            // 1-3-5, 1-4-18, 3-6-7, ...
            // ----  find all permutations - use this with processtracks = 'y'
            Int32 bbktr, ccktr;
            if (whichApp.Equals("all"))
            {
                processtracks = 'y';
                for (trknumktr = 0; trknumktr <= totalTracks - 2 - obs2skip * 2; trknumktr += obs2skip)
                {
                    aa = trknumktr;
                    for (bbktr = trknumktr + 1 * (obs2skip + 1); bbktr <= totalTracks - 1 - obs2skip; bbktr += obs2skip)
                    {
                        bb = bbktr;
                        for (ccktr = bbktr + 1 * (obs2skip + 1); ccktr <= totalTracks - 1; ccktr += obs2skip+1)
                        {
                            cc = ccktr;
                            numCode = aa + " " + bb + " " + cc;
                            if (!possibilities.Exists(element => element.Equals(numCode)))
                                possibilities.Add(numCode);
                            numtrials = numtrials + 1;

                        }  // cc
                    }  // bb
                } // aa
            } // whichapp    

            // ---- find just sequential sets
            // set each track number (not obsktr), and then use the middle obsintrk value 
            // we've already made "sure" each track is consistent wihtin itself above
            // code will be tracknumktr, tracknumktr + obs2skip, tracknumktr + 2*obs2skip
            // the code will have the first tracknumktr, and then the 3 middle obs of each of the tracks identified above
            if (whichApp.Equals("seq"))
            {
//                for (obsktr = 0; obsktr <= numobs - 3 - obs2skip * 2; obsktr++)
                for (trknumktr = 0; trknumktr < totalTracks - 2*obs2skip; trknumktr++)
                    {
                    // make sure each track has enough obs
                    if (trackClassArr[trknumktr].numobstk > 3)
                    {
                        aa = (int)Math.Floor(trackClassArr[trknumktr].numobstk * 0.5);
                        bb = (int)Math.Floor(trackClassArr[trknumktr + obs2skip].numobstk * 0.5);
                        cc = (int)Math.Floor(trackClassArr[trknumktr + 2 * obs2skip].numobstk * 0.5);

                        numCode = trknumktr + " " + aa + " " + bb + " " + cc;
                        numtrials = numtrials + 1;

                        if (!possibilities.Exists(element => element.Equals(numCode)))
                            possibilities.Add(numCode);
                    }
                }  // cc
            } // whichapp    

            this.opsStatus.Text = "Form IOD orbits ";
            Refresh();

            // --------------------------------------------- form iod orbits ---------------------------------------
            // loop through obs and use combinations to form all solutions
            string[] poss = possibilities.ToArray();

            Int32 ob1, ob2, ob3, sen1, sen2, sen3;
            double jd1, jd2, jd3, jd1f, jd2f, jd3f;
            char diffsites;
            for (obsktr = 0; obsktr <= numtrials - 1; obsktr++)
            {
                Int32 currTrack1, currTrack2, currTrack3;
                currTrack1 = 0;
                currTrack2 = 0;
                currTrack3 = 0;
                string[] pos = poss[obsktr].Split(' ');
                if (processtracks == 'n')
                {
                    currTrack1 = Convert.ToInt32(pos[0]);
                    currTrack2 = currTrack1 + obs2skip;
                    currTrack3 = currTrack2 + obs2skip;
                    ob1 = Convert.ToInt32(pos[1]);
                    ob2 = Convert.ToInt32(pos[2]);
                    ob3 = Convert.ToInt32(pos[3]);
                }
                else
                {
                    currTrack1 = Convert.ToInt32(pos[0]);
                    ob1 = (int)Math.Floor(trackClassArr[currTrack1].numobstk * 0.5);
                    currTrack2 = Convert.ToInt32(pos[1]);
                    ob2 = (int)Math.Floor(trackClassArr[currTrack2].numobstk * 0.5);
                    currTrack3 = Convert.ToInt32(pos[2]);
                    ob3 = (int)Math.Floor(trackClassArr[currTrack3].numobstk * 0.5);
                }

                jd1 = trackClassArr[currTrack1].tracks[ob1].jd;
                jd1f = trackClassArr[currTrack1].tracks[ob1].jdf;
                jd2 = trackClassArr[currTrack2].tracks[ob2].jd;
                jd2f = trackClassArr[currTrack2].tracks[ob2].jdf;
                jd3 = trackClassArr[currTrack3].tracks[ob3].jd;
                jd3f = trackClassArr[currTrack3].tracks[ob3].jdf;

                // don't process obs at same time
                if (Math.Abs(jd1 + jd1f - jd2 - jd2f) > 0.000001 && Math.Abs(jd2 + jd2f - jd3 - jd3f) > 0.000001)
                {
                    sen1 = trackClassArr[currTrack1].tracks[ob1].sennum;
                    sen2 = trackClassArr[currTrack2].tracks[ob2].sennum;
                    sen3 = trackClassArr[currTrack3].tracks[ob3].sennum;
                    diffsites = 'y';
                    if (sen1 == sen2 && sen2 == sen3)
                        diffsites = 'n';

                    rtasc1 = trackClassArr[currTrack1].tracks[ob1].rtasc;
                    decl1  = trackClassArr[currTrack1].tracks[ob1].decl;
                    rtasc2 = trackClassArr[currTrack2].tracks[ob2].rtasc;
                    decl2  = trackClassArr[currTrack2].tracks[ob2].decl;
                    rtasc3 = trackClassArr[currTrack3].tracks[ob3].rtasc;
                    decl3  = trackClassArr[currTrack3].tracks[ob3].decl;

                    rseci1 = trackClassArr[currTrack1].tracks[ob1].rseci;
                    rseci2 = trackClassArr[currTrack2].tracks[ob2].rseci;
                    rseci3 = trackClassArr[currTrack3].tracks[ob3].rseci;

                    AstroLibr.angleslaplace(
                        decl1, decl2, decl3, rtasc1, rtasc2, rtasc3,
                        jd1, jd1f, jd2, jd2f, jd3, jd3f,
                        diffsites, rseci1, rseci2, rseci3, out r2l, out v2l, out errstr);
                    strbuildObs.AppendLine(" " + errstr);
                    AstroLibr.rv2coe(r2l, v2l, out pl, out al, out eccl, out incll, out raanl, out argpl, out nul, out ml, out arglatl, out truelonl, out lonperl);

                    strbuildObs.AppendLine(poss[obsktr] + "laplace coes a= " + al.ToString("0.0000") + " e= " + eccl.ToString("0.000000000") + " i= " + (incll * rad).ToString("0.0000") + " " +
                       (raanl * rad).ToString("0.0000") + " " + (argpl * rad).ToString("0.0000") + " " + (nul * rad).ToString("0.0000") + " " + (ml * rad).ToString("0.0000") + " " +
                       (arglatl * rad).ToString("0.0000")); // + " " + (truelon * rad).ToString("0.0000") + " " + (lonper * rad).ToString("0.0000"));

                    AstroLibr.anglesgauss(decl1, decl2, decl3, rtasc1, rtasc2, rtasc3,
                        jd1, jd1f, jd2, jd2f, jd3, jd3f,
                         rseci1, rseci2, rseci3, out r2g, out v2g, out errstr);
                    strbuildObs.AppendLine(" " + errstr);
                    AstroLibr.rv2coe(r2g, v2g, out pg, out ag, out eccg, out inclg, out raang, out argpg, out nug, out mg, out arglatg, out truelong, out lonperg);
                    strbuildObs.AppendLine(poss[obsktr] + "gauss   coes a= " + ag.ToString("0.0000") + " e= " + eccg.ToString("0.000000000") + " i= " + (inclg * rad).ToString("0.0000") + " " +
                       (raang * rad).ToString("0.0000") + " " + (argpg * rad).ToString("0.0000") + " " + (nug * rad).ToString("0.0000") + " " + (mg * rad).ToString("0.0000") + " " +
                       (arglatg * rad).ToString("0.0000")); // + " " + (truelon * rad).ToString("0.0000") + " " + (lonper * rad).ToString("0.0000"));


                    // initial guesses needed for double-r and Gooding
                    // use result from Gauss as it's usually pretty good
                    // this seems to really help Gooding!!
                    getGaussRoot(decl1, decl2, decl3, rtasc1, rtasc2, rtasc3, jd1, jd1f, jd2, jd2f, jd3, jd3f,
                         rseci1, rseci2, rseci3, out bigr2x);

                    double rng1 = bigr2x;  // old 12500 needs to be in km!! seems to do better when all the same? if too far off (*2) NAN
                    double rng2 = bigr2x * 1.02;  // 1.02 might be better? make the initial guess a bit different
                    double rng3 = bigr2x * 1.08;
                    AstroLibr.anglesdoubler(decl1, decl2, decl3, rtasc1, rtasc2, rtasc3,
                        jd1, jd1f, jd2, jd2f, jd3, jd3f,
                         rseci1, rseci2, rseci3, rng1, rng2, out r2d, out v2d, out errstr);
                    AstroLibr.rv2coe(r2d, v2d, out pd, out ad, out eccd, out incld, out raand, out argpd, out nud, out md, out arglatd, out truelond, out lonperd);
                    strbuildObs.AppendLine(poss[obsktr] + "doubler coes a= " + ad.ToString("0.0000") + " e= " + eccd.ToString("0.000000000") + " i= " + (incld * rad).ToString("0.0000") + " " +
                       (raand * rad).ToString("0.0000") + " " + (argpd * rad).ToString("0.0000") + " " + (nud * rad).ToString("0.0000") + " " + (md * rad).ToString("0.0000") + " " +
                       (arglatd * rad).ToString("0.0000")); // + " " + (truelon * rad).ToString("0.0000") + " " + (lonper * rad).ToString("0.0000"));

                    int numhalfrev = 0;
                    AstroLibr.anglesgooding(decl1, decl2, decl3, rtasc1, rtasc2, rtasc3,
                        jd1, jd1f, jd2, jd2f, jd3, jd3f,
                        rseci1, rseci2, rseci3, numhalfrev, rng1, rng2, rng3, out r2o, out v2o, out errstr);
                    AstroLibr.rv2coe(r2o, v2o, out po, out ao, out ecco, out inclo, out raano, out argpo, out nuo, out mo, out arglato, out truelono, out lonpero);
                    strbuildObs.AppendLine(poss[obsktr] + "gooding coes a= " + ao.ToString("0.0000") + " e= " + ecco.ToString("0.000000000") + " i= " + (inclo * rad).ToString("0.0000") + " " +
                        (raano * rad).ToString("0.0000") + " " + (argpo * rad).ToString("0.0000") + " " + (nuo * rad).ToString("0.0000") + " " + (mo * rad).ToString("0.0000") + " " +
                        (arglato * rad).ToString("0.0000") + "\n\n"); // + " " + (truelon * rad).ToString("0.0000") + " " + (lonper * rad).ToString("0.0000"));

                    // determine which is correct...
                    // use a, e, inc, r2, v2?



                    // store results for sorting and processing
                    // this will be row 0
                    Object[] rows = new Object[] {poss[obsktr], al, ag, ad, ao, eccl, eccg, eccd, ecco,
                    incll, inclg, incld, inclo,
                    r2l[0],   r2l[1],    r2l[2],
                    r2g[0],   r2g[1],    r2g[2],
                    r2d[0],   r2d[1],    r2d[2],
                    r2o[0],   r2o[1],    r2o[2],
                    v2l[0],   v2l[1],    v2l[2],
                    v2g[0],   v2g[1],    v2g[2],
                    v2o[0],   v2o[1],    v2o[2],
                    v2d[0],   v2d[1],    v2d[2] };

                    satTable.Rows.Add(rows);
                } // if not at same time
                else
                {
                    strbuildObs.AppendLine("obs at the same time "
                        + jd1f.ToString() + " " + jd2f.ToString() + " " + jd3f.ToString());
                }

            }

            File.WriteAllText(@"D:\datafile\CSSI\COMSPOC\UCT\testxobs.out", strbuildObs.ToString());


            // null hypothesis is to consider the obs all releated to a single satellite








            //// ---------------------------------- sort on parameter selected (dr, dv, p, a, e, i)
            this.opsStatus.Text = "Sorting ... ";
            Refresh();

            // now do sort on col
            DataView dv = satTable.DefaultView;
            dv.Sort = "ad";
            //            dv.Sort = "ag";
            DataTable sortedsatDT = dv.ToTable();

            //         ShowTable(sortedDT, out strout);
            //         File.WriteAllText(@"D:\Codes\LIBRARY\cs\ConjunctionSieve\test.out", strout);

            // now see which ones are "close"
            // setup another datatable with same structure, but no data yet
            //DataTable sortedDtx = new DataTable();
            //sortedDtx = sortedDT.Clone();

            ShowTable(sortedsatDT, out strout);
            File.WriteAllText(@"D:\datafile\CSSI\COMSPOC\UCT\testx.out", strout);

            // ---------------------------------- add only ones that are "close"
            // this may give the original satellite too??????????
            //for (int ktrt = 1; ktrt < sortedDT.Rows.Count; ktrt++)
            //{
            //    if (Math.Abs(compvalue - sortedDT.Rows[ktrt].Field<double>(casei + 1)) <
            //        Convert.ToDouble(this.ipnutThresh.Text))  // 1 is r1[0], 2 is r1[1], etc
            //    {
            //        sortedDtx.ImportRow(sortedDT.Rows[ktrt]);
            //    }
            //}

            //// ---- sort on the range (1st item in string list)   
            //strArr.Sort();

            //while (casei < 3 && numclose > 0)
            //{
            //    // store target sat compare value
            //    double compvalue = r1[casei];
            //    DataTable newtable = table.Copy();

            //    // now do sort on col
            //    DataView dv = table.DefaultView;
            //    dv.Sort = "r1[" + casei.ToString() + "] asc";
            //    DataTable sortedDT = dv.ToTable();

            //    //         ShowTable(sortedDT, out strout);
            //    //         File.WriteAllText(@"D:\Codes\LIBRARY\cs\ConjunctionSieve\test.out", strout);

            //    // now see which ones are "close"
            //    // setup another datatable with same structure, but no data yet
            //    DataTable sortedDtx = new DataTable();
            //    sortedDtx = sortedDT.Clone();

            //    // ---------------------------------- add only ones that are "close"
            //    // this may give the original satellite too??????????
            //    for (int ktrt = 1; ktrt < sortedDT.Rows.Count; ktrt++)
            //    {
            //        if (Math.Abs(compvalue - sortedDT.Rows[ktrt].Field<double>(casei + 1)) <
            //            Convert.ToDouble(this.ipnutThresh.Text))  // 1 is r1[0], 2 is r1[1], etc
            //        {
            //            sortedDtx.ImportRow(sortedDT.Rows[ktrt]);
            //        }
            //    }


            //    // copy just the sorted results
            //    table = sortedDtx.Copy();

            //    // --------------- setup for next comparison
            //    numclose = sortedDtx.Rows.Count;
            //    Int32 numclose1 = table.Rows.Count;
            //    casei = casei + 1;
            //}  // while through the position comparisons


            //for (int j = 1; j < numclose; j++)
            //{
            //    //  is there a faster way to do this?
            //    mjd = Convert.ToDouble(table.Rows[j].Field<double>(0));
            //    r3[0] = table.Rows[j].Field<double>(1);
            //    r3[1] = table.Rows[j].Field<double>(2);
            //    r3[2] = table.Rows[j].Field<double>(3);
            //    v3[0] = table.Rows[j].Field<double>(4);
            //    v3[1] = table.Rows[j].Field<double>(5);
            //    v3[2] = table.Rows[j].Field<double>(6);
            //    r4[0] = table.Rows[j].Field<double>(7);
            //    r4[1] = table.Rows[j].Field<double>(8);
            //    r4[2] = table.Rows[j].Field<double>(9);
            //    v4[0] = table.Rows[j].Field<double>(10);
            //    v4[1] = table.Rows[j].Field<double>(11);
            //    v4[2] = table.Rows[j].Field<double>(12);
            //    Int32 sscnum = table.Rows[j].Field<Int32>(13);

            //    dr1[0] = r1[0] - r3[0];
            //    dr1[1] = r1[1] - r3[1];
            //    dr1[2] = r1[2] - r3[2];
            //    dv1[0] = v1[0] - v3[0];
            //    dv1[1] = v1[1] - v3[1];
            //    dv1[2] = v1[2] - v3[2];
            //    dr2[0] = r2[0] - r4[0];
            //    dr2[1] = r2[1] - r4[1];
            //    dr2[2] = r2[2] - r4[2];
            //    dv2[0] = v2[0] - v4[0];
            //    dv2[1] = v2[1] - v4[1];
            //    dv2[2] = v2[2] - v4[2];

            //}


            // ---------------------------------- write out associated observations
            string directory = @"D:\Codes\LIBRARY\cs\TestAll\";
            File.WriteAllText(directory + "testUCT.out", strbuild.ToString());
        }



        // create topology --------------------------------------------------
        public void testanglestopology
        ()
        {
            double[] rs1ecef = new double[3];
            double[] vs1ecef = new double[3];
            double[] rs1eci = new double[3];
            double[] vs1eci = new double[3];
            double[] rs2eci = new double[3];
            double[] vs2eci = new double[3];
            double[] rs3eci = new double[3];
            double[] vs3eci = new double[3];
            double[] r1 = new double[3];
            double[] v1 = new double[3];
            double[] r2 = new double[3];
            double[] v2 = new double[3];
            double[] r2l = new double[3];
            double[] v2l = new double[3];
            double[] r2g = new double[3];
            double[] v2g = new double[3];
            double[] r2d = new double[3];
            double[] v2d = new double[3];
            double[] r2o = new double[3];
            double[] v2o = new double[3];
            double[] r2ans = new double[3];
            double[] v2ans = new double[3];
            double[] r3 = new double[3];
            double[] v3 = new double[3];
            double[] drl = new double[3];
            double[] dvl = new double[3];
            double[] drg = new double[3];
            double[] dvg = new double[3];
            double[] drd = new double[3];
            double[] dvd = new double[3];
            double[] dro = new double[3];
            double[] dvo = new double[3];
            double rho1, drho1, trtasc1, tdecl1, dtrtasc1, dtdecl1;
            double rho2, drho2, trtasc2, tdecl2, dtrtasc2, dtdecl2;
            double rho3, drho3, trtasc3, tdecl3, dtrtasc3, dtdecl3;
            double latgd, lon, alt, dt1, dt2, rng1, rng2, rng3, initguess, bigr2x;
            double rad, jd1, jdf1, jd2, jdf2, jd3, jdf3, jdut1, dut1, jdtt, jdftt, ttt, xp, yp, lod, conv, ddx, ddy, ddpsi, ddeps;
            double p, a, ecc, incl, raan, argp, nu, m, arglat, lonper, truelon;
            double pans, aans, eccans, inclans, raanans, argpans, nuans, mans, arglatans, lonperans, truelonans;
            double drmin, dvmin, damin, dpmin, deccmin, dinclmin;
            double da1, decc1, dincl1, da2, decc2, dincl2, da3, decc3, dincl3, da4, decc4, dincl4, dp1, dp2, dp3, dp4;
            Int32 ktrRl, ktrVl, ktrAl, ktrPl, ktrEl, ktrIl, ktrRg, ktrVg, ktrAg, ktrPg, ktrEg, ktrIg,
                  ktrRd, ktrVd, ktrAd, ktrPd, ktrEd, ktrId, ktrRo, ktrVo, ktrAo, ktrPo, ktrEo, ktrIo;
            string mintech;
            char diffsites;
            Int32 dat;
            StringBuilder strbuild = new StringBuilder();
            string errstr = "";

            string nutLoc;
            string ans;
            Int32 ktrActObs;
            string EOPupdate;
            Int32 mjdeopstart;
            EOPSPWLib.iau80Class iau80arr;
            EOPSPWLib.iau00Class iau00arr;
            nutLoc = @"D:\Codes\LIBRARY\DataLib\nut80.dat";
            EOPSPWLibr.iau80in(nutLoc, out iau80arr);
            nutLoc = @"D:\Codes\LIBRARY\DataLib\";
            EOPSPWLibr.iau00in(nutLoc, out iau00arr);
            string eopFileName = @"D:\Codes\LIBRARY\DataLib\EOP-All-v1.1_2020-02-12.txt";
            EOPSPWLibr.readeop(ref EOPSPWLibr.eopdata, eopFileName, out mjdeopstart, out ktrActObs, out EOPupdate);

            double jdxysstart, jdfxysstart;
            AstroLib.xysdataClass[] xysarr = AstroLibr.xysarr;
            AstroLibr.initXYS(ref xysarr, nutLoc, "xysdata.dat", out jdxysstart, out jdfxysstart);

            ktrRl = 0; ktrVl = 0; ktrAl = 0; ktrPl = 0; ktrEl = 0; ktrIl = 0;
            ktrRg = 0; ktrVg = 0; ktrAg = 0; ktrPg = 0; ktrEg = 0; ktrIg = 0;
            ktrRd = 0; ktrVd = 0; ktrAd = 0; ktrPd = 0; ktrEd = 0; ktrId = 0;
            ktrRo = 0; ktrVo = 0; ktrAo = 0; ktrPo = 0; ktrEo = 0; ktrIo = 0;
            mintech = "";
            rad = 180.0 / Math.PI;

            rho1 = rho2 = rho3 = 0.0;
            drho1 = drho2 = drho3 = 0.0;
            trtasc1 = trtasc2 = trtasc3 = 0.0;
            dtrtasc1 = dtrtasc2 = dtrtasc3 = 0.0;
            tdecl1 = tdecl2 = tdecl3 = 0.0;
            dtdecl1 = dtdecl2 = dtdecl3 = 0.0;

            latgd = 38.887100 / rad;
            // lon should be made so visible to GEO sats, but "not" the same...
            lon = -104.201600 / rad;
            alt = 1.9419;  // km

            AstroLibr.site(latgd, lon, alt, out rs1ecef, out vs1ecef);
            // do each of these separately


            // loop through spacing
            for (int i = 0; i <= 500; i++)
            {
                dt1 = 20.0 + 20.0 * i;  // sec spacing of obs 1-2
                dt2 = dt1;              // sec spacing of obs 2-3

                // loop through semimajor axes 7378 - 57378
                for (int j = 1; j <= 50; j++)
                {
                    // reset each time since each solution finds coes
                    //a = 7138.0;
                    a = 42164.0;
                    ecc = 0.01;
                    incl = 0.1 / rad;
                    raan = 0.0 / rad;
                    argp = 0.0 / rad;
                    nu = 0.0 / rad;
                    arglat = 0.0 / rad;
                    lonper = 0.0 / rad;
                    truelon = 0.0 / rad;
                    a = AstroLibr.gravConst.re + 1000.0 * j;

                    // loop through eccentricity 0.0 to 0.98
                    //for (int j = 0; j <= 49; j++)
                    //{
                    //    //reset each time since each solution finds coes
                    //    //a = 7138.0;  // careful as higher e will give subearth trajectory
                    //    a = 42164.0;
                    //    ecc = 0.01;
                    //    incl = 0.1 / rad;
                    //    raan = 0.0 / rad;
                    //    argp = 0.0 / rad;
                    //    nu = 0.0 / rad;
                    //    arglat = 0.0 / rad;
                    //    lonper = 0.0 / rad;
                    //    truelon = 0.0 / rad;
                    //    ecc = 0.02 * j;

                    // loop through inclination 0 - 180
                    //for (int j = 0; j <= 18; j++)
                    //{
                    //    a = 7138.0;
                    //    //a = 42164.0;
                    //    ecc = 0.01;
                    //    incl = 0.1 / rad;
                    //    raan = 0.0 / rad;
                    //    argp = 0.0 / rad;
                    //    nu = 0.0 / rad;
                    //    arglat = 0.0 / rad;
                    //    lonper = 0.0 / rad;
                    //    truelon = 0.0 / rad;
                    //    incl = (10.0 * j) / rad;

                    // propagate to the three times and get obs
                    p = a * (1.0 - ecc * ecc);
                    AstroLibr.coe2rv(p, ecc, incl, raan, argp, nu, arglat, truelon, lonper, out r1, out v1);

                    // --------------------- create obs1
                    MathTimeLibr.jday(2021, 6, 8, 3, 51, 42.7657, out jd1, out jdf1);
                    EOPSPWLibr.findeopparam(jd1, jdf1, 's', EOPSPWLibr.eopdata, mjdeopstart + 2400000.5,
                        out dut1, out dat, out lod, out xp, out yp, out ddpsi, out ddeps, out ddx, out ddy);
                    jdtt = jd1;
                    jdftt = jdf1 + (dat + 32.184) / 86400.0;
                    ttt = (jdtt + jdftt - 2451545.0) / 36525.0;
                    jdut1 = jd1 + jdf1 + dut1 / 86400.0;
                    AstroLibr.eci_ecef(ref rs1eci, ref vs1eci, MathTimeLib.Edirection.efrom, ref rs1ecef, ref vs1ecef,
                         AstroLib.EOpt.e80, iau80arr, iau00arr,
                         jdtt, jdftt, jdut1, jdxysstart, lod, xp, yp, ddpsi, ddeps, ddx, ddy);

                    AstroLibr.rv_tradec(ref r1, ref v1, rs1eci, MathTimeLib.Edirection.eto,
                        ref rho1, ref trtasc1, ref tdecl1, ref drho1, ref dtrtasc1, ref dtdecl1);

                    // --------------------- create obs2
                    AstroLibr.kepler(r1, v1, dt1, out r2, out v2);
                    for (int k = 0; k <= 2; k++)
                    {
                        r2ans[k] = r2[k];
                        v2ans[k] = v2[k];
                    }
                    AstroLibr.rv2coe(r2ans, v2ans, out pans, out aans, out eccans, out inclans, out raanans,
                        out argpans, out nuans, out mans, out arglatans, out truelonans, out lonperans);
                    MathTimeLibr.jday(2021, 6, 8, 3, 51, 42.7657 + dt1, out jd2, out jdf2);
                    EOPSPWLibr.findeopparam(jd2, jdf2, 's', EOPSPWLibr.eopdata, mjdeopstart + 2400000.5,
                        out dut1, out dat, out lod, out xp, out yp, out ddpsi, out ddeps, out ddx, out ddy);
                    jdtt = jd2;
                    jdftt = jdf2 + (dat + 32.184) / 86400.0;
                    ttt = (jdtt + jdftt - 2451545.0) / 36525.0;
                    jdut1 = jd2 + jdf2 + dut1 / 86400.0;
                    AstroLibr.eci_ecef(ref rs2eci, ref vs2eci, MathTimeLib.Edirection.efrom, ref rs1ecef, ref vs1ecef,
                         AstroLib.EOpt.e80, iau80arr, iau00arr,
                         jdtt, jdftt, jdut1, jdxysstart, lod, xp, yp, ddpsi, ddeps, ddx, ddy);

                    AstroLibr.rv_tradec(ref r2, ref v2, rs2eci, MathTimeLib.Edirection.eto,
                        ref rho2, ref trtasc2, ref tdecl2, ref drho2, ref dtrtasc2, ref dtdecl2);

                    // --------------------- create obs3
                    AstroLibr.kepler(r1, v1, dt1 + dt2, out r3, out v3);
                    MathTimeLibr.jday(2021, 6, 8, 3, 51, 42.7657 + dt1 + dt2, out jd3, out jdf3);
                    EOPSPWLibr.findeopparam(jd3, jdf3, 's', EOPSPWLibr.eopdata, mjdeopstart + 2400000.5,
                        out dut1, out dat, out lod, out xp, out yp, out ddpsi, out ddeps, out ddx, out ddy);
                    jdtt = jd3;
                    jdftt = jdf3 + (dat + 32.184) / 86400.0;
                    ttt = (jdtt + jdftt - 2451545.0) / 36525.0;
                    jdut1 = jd3 + jdf3 + dut1 / 86400.0;
                    AstroLibr.eci_ecef(ref rs3eci, ref vs3eci, MathTimeLib.Edirection.efrom, ref rs1ecef, ref vs1ecef,
                         AstroLib.EOpt.e80, iau80arr, iau00arr,
                         jdtt, jdftt, jdut1, jdxysstart, lod, xp, yp, ddpsi, ddeps, ddx, ddy);

                    AstroLibr.rv_tradec(ref r3, ref v3, rs3eci, MathTimeLib.Edirection.eto,
                        ref rho3, ref trtasc3, ref tdecl3, ref drho3, ref dtrtasc3, ref dtdecl3);

                    strbuild.Append(dt1.ToString() + " " + dt2.ToString() + " " + a.ToString() + " "
                        + ecc.ToString("0.000") + " " + (incl * rad).ToString("0.000") + " ");


                    // --------------------- now test each method ---------------------
                    AstroLibr.angleslaplace(tdecl1, tdecl2, tdecl3, trtasc1, trtasc2, trtasc3,
                        jd1, jdf1, jd2, jdf2, jd3, jdf3,
                        'n', rs1eci, rs2eci, rs3eci, out r2, out v2, out errstr);
                    for (int k = 0; k <= 2; k++)
                    {
                        drl[k] = r2ans[k] - r2[k];
                        dvl[k] = v2ans[k] - v2[k];
                        r2l[k] = r2[k];
                        v2l[k] = v2[k];
                    }
                    if (!Double.IsNaN(MathTimeLibr.mag(r2)) && !Double.IsNaN(MathTimeLibr.mag(v2)))
                    {
                        AstroLibr.rv2coe(r2, v2, out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out arglat, out truelon, out lonper);
                        //if (Math.Abs(dt1 - 480.0) < 0.01 && Math.Abs(eccans - 0.640) < 0.001)
                        //{
                        //    da1 = a;
                        //}
                        da1 = Math.Abs(aans - a);
                        dp1 = Math.Abs(pans - p);
                        decc1 = Math.Abs(eccans - ecc);
                        dincl1 = Math.Abs(inclans - incl);
                    }
                    else
                    {
                        da1 = 99999.9;
                        dp1 = 99999.9;
                        decc1 = 99999.9;
                        dincl1 = 99999.9;
                    }
                    strbuild.Append(" L " + MathTimeLibr.mag(drl).ToString(fmt2) + " " + MathTimeLibr.mag(dvl).ToString(fmt2) + " "
                        + da1.ToString(fmt2) + " " + decc1.ToString(fmt2) + " " + (dincl1 * rad).ToString(fmt2) + " ");


                    AstroLibr.anglesgauss(tdecl1, tdecl2, tdecl3, trtasc1, trtasc2, trtasc3,
                        jd1, jdf1, jd2, jdf2, jd3, jdf3,
                        rs1eci, rs2eci, rs3eci, out r2, out v2, out errstr);
                    for (int k = 0; k <= 2; k++)
                    {
                        drg[k] = r2ans[k] - r2[k];
                        dvg[k] = v2ans[k] - v2[k];
                        r2g[k] = r2[k];
                        v2g[k] = v2[k];
                    }
                    if (!Double.IsNaN(MathTimeLibr.mag(r2)) && !Double.IsNaN(MathTimeLibr.mag(v2)))
                    {
                        AstroLibr.rv2coe(r2, v2, out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out arglat, out truelon, out lonper);
                        da2 = Math.Abs(aans - a);
                        dp2 = Math.Abs(pans - p);
                        decc2 = Math.Abs(eccans - ecc);
                        dincl2 = Math.Abs(inclans - incl);
                    }
                    else
                    {
                        da2 = 99999.9;
                        dp2 = 99999.9;
                        decc2 = 99999.9;
                        dincl2 = 99999.9;
                    }

                    strbuild.Append(" G " + MathTimeLibr.mag(drg).ToString(fmt2) + " " + MathTimeLibr.mag(dvg).ToString(fmt2) + " "
                        + da2.ToString(fmt2) + " " + decc2.ToString(fmt2) + " " + (dincl2 * rad).ToString(fmt2) + " ");


                    getGaussRoot(tdecl1, tdecl2, tdecl3, trtasc1, trtasc2, trtasc3,
                        jd1, jdf1, jd2, jdf2, jd3, jdf3,
                        rs1eci, rs2eci, rs3eci, out bigr2x);
                    initguess = bigr2x;
                    strbuild.Append(" bigr2x " + bigr2x.ToString() + " ");
                    rng1 = initguess;  // old 12500 needs to be in km!! seems to do better when all the same? if too far off (*2) NAN
                    rng2 = initguess * 1.02;  // 1.02 might be better? make the initial guess a bit different
                    rng3 = initguess * 1.08;
                    AstroLibr.anglesdoubler(tdecl1, tdecl2, tdecl3, trtasc1, trtasc2, trtasc3,
                        jd1, jdf1, jd2, jdf2, jd3, jdf3,
                         rs1eci, rs2eci, rs3eci, rng1, rng2, out r2, out v2, out errstr);
                    for (int k = 0; k <= 2; k++)
                    {
                        drd[k] = r2ans[k] - r2[k];
                        dvd[k] = v2ans[k] - v2[k];
                        r2d[k] = r2[k];
                        v2d[k] = v2[k];
                    }
                    if (!Double.IsNaN(MathTimeLibr.mag(r2)) && !Double.IsNaN(MathTimeLibr.mag(v2)))
                    {
                        AstroLibr.rv2coe(r2, v2, out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out arglat, out truelon, out lonper);
                        da3 = Math.Abs(aans - a);
                        dp3 = Math.Abs(pans - p);
                        decc3 = Math.Abs(eccans - ecc);
                        dincl3 = Math.Abs(inclans - incl);
                    }
                    else
                    {
                        da3 = 99999.9;
                        dp3 = 99999.9;
                        decc3 = 99999.9;
                        dincl3 = 99999.9;
                    }

                    strbuild.Append(" D " + MathTimeLibr.mag(drd).ToString(fmt2) + " " + MathTimeLibr.mag(dvd).ToString(fmt2) + " "
                        + da3.ToString(fmt2) + " " + decc3.ToString(fmt2) + " " + (dincl3 * rad).ToString(fmt2) + " ");


                    int numhalfrev = 0;
                    AstroLibr.anglesgooding(tdecl1, tdecl2, tdecl3, trtasc1, trtasc2, trtasc3,
                        jd1, jdf1, jd2, jdf2, jd3, jdf3,
                         rs1eci, rs2eci, rs3eci, numhalfrev, rng1, rng2, rng3, out r2, out v2, out errstr);
                    for (int k = 0; k <= 2; k++)
                    {
                        dro[k] = r2ans[k] - r2[k];
                        dvo[k] = v2ans[k] - v2[k];
                        r2o[k] = r2[k];
                        v2o[k] = v2[k];
                    }
                    if (!Double.IsNaN(MathTimeLibr.mag(r2)) && !Double.IsNaN(MathTimeLibr.mag(v2)))
                    {
                        AstroLibr.rv2coe(r2, v2, out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out arglat, out truelon, out lonper);
                        da4 = Math.Abs(aans - a);
                        dp4 = Math.Abs(pans - p);
                        decc4 = Math.Abs(eccans - ecc);
                        dincl4 = Math.Abs(inclans - incl);
                    }
                    else
                    {
                        da4 = 99999.9;
                        dp4 = 99999.9;
                        decc4 = 99999.9;
                        dincl4 = 99999.9;
                    }

                    strbuild.Append(" O " + MathTimeLibr.mag(dro).ToString(fmt2) + " " + MathTimeLibr.mag(dvo).ToString(fmt2) + " "
                       + da4.ToString(fmt2) + " " + decc4.ToString(fmt2) + " " + (dincl4 * rad).ToString(fmt2) + " ");

                    // --------------------- determine mins ---------------------
                    // orbits only need to be close
                    double rtol = 500.0; // km
                    double vtol = 0.1;   // km/s
                    double atol = 500.0; // km
                    double ptol = 500.0; // km
                    double etol = 0.1;  // 
                    double itol = 5.0/rad;   // rad

                    drmin = Math.Min(rtol, MathTimeLibr.mag(drl));
                    drmin = Math.Min(drmin, MathTimeLibr.mag(drg));
                    drmin = Math.Min(drmin, MathTimeLibr.mag(drd));
                    drmin = Math.Min(drmin, MathTimeLibr.mag(dro));

                    dvmin = Math.Min(vtol, MathTimeLibr.mag(drl));
                    dvmin = Math.Min(dvmin, MathTimeLibr.mag(dvg));
                    dvmin = Math.Min(dvmin, MathTimeLibr.mag(dvd));
                    dvmin = Math.Min(dvmin, MathTimeLibr.mag(dvo));

                    damin = Math.Min(atol, da1);
                    damin = Math.Min(damin, da2);
                    damin = Math.Min(damin, da3);
                    damin = Math.Min(damin, da4);

                    dpmin = Math.Min(ptol, dp1);
                    dpmin = Math.Min(dpmin, dp2);
                    dpmin = Math.Min(dpmin, dp3);
                    dpmin = Math.Min(dpmin, dp4);

                    deccmin = Math.Min(etol, decc1);
                    deccmin = Math.Min(deccmin, decc2);
                    deccmin = Math.Min(deccmin, decc3);
                    deccmin = Math.Min(deccmin, decc4);

                    dinclmin = Math.Min(itol, dincl1);
                    dinclmin = Math.Min(dinclmin, dincl2);
                    dinclmin = Math.Min(dinclmin, dincl3);
                    dinclmin = Math.Min(dinclmin, dincl4);

                    // --------------------- R
                    if (Math.Abs(MathTimeLibr.mag(drl) - drmin) <= rtol)
                    {
                        mintech = mintech + "Lr";
                        ktrRl = ktrRl + 1;
                    }
                    if (Math.Abs(MathTimeLibr.mag(drg) - drmin) <= rtol)
                    {
                        mintech = mintech + "Gr";
                        ktrRg = ktrRg + 1;
                    }
                    if (Math.Abs(MathTimeLibr.mag(drd) - drmin) <= rtol)
                    {
                        mintech = mintech + "Dr";
                        ktrRd = ktrRd + 1;
                    }
                    if (Math.Abs(MathTimeLibr.mag(dro) - drmin) <= rtol)
                    {
                        mintech = mintech + "Or";
                        ktrRo = ktrRo + 1;
                    }

                    // --------------------- V
                    if (Math.Abs(MathTimeLibr.mag(dvl) - dvmin) <= vtol)
                    {
                        mintech = mintech + "Lv";
                        ktrVl = ktrVl + 1;
                    }
                    if (Math.Abs(MathTimeLibr.mag(dvg) - dvmin) <= vtol)
                    {
                        mintech = mintech + "Gv";
                        ktrVg = ktrVg + 1;
                    }
                    if (Math.Abs(MathTimeLibr.mag(dvd) - dvmin) <= vtol)
                    {
                        mintech = mintech + "Dv";
                        ktrVd = ktrVd + 1;
                    }
                    if (Math.Abs(MathTimeLibr.mag(dvo) - dvmin) <= vtol)
                    {
                        mintech = mintech + "Ov";
                        ktrVo = ktrVo + 1;
                    }

                    // --------------------- A
                    if (Math.Abs(da1 - damin) <= atol)
                    { 
                        mintech = mintech + "La";
                        ktrAl = ktrAl + 1;
                    }
                    if (Math.Abs(da2 - damin) <= atol)
                    { 
                        mintech = mintech + "Ga";
                        ktrAg = ktrAg + 1;
                    }
                    if (Math.Abs(da3 - damin) <= atol)
                    {
                        mintech = mintech + "Da";
                        ktrAd = ktrAd + 1;
                    }
                    if (Math.Abs(da4 - damin) <= atol)
                    {
                        mintech = mintech + "Oa";
                        ktrAo = ktrAo + 1;
                    }

                    // --------------------- P
                    if (Math.Abs(dp1 - dpmin) <= ptol)
                    { 
                        mintech = mintech + "Lp";
                        ktrPl = ktrPl + 1;
                    }
                    if (Math.Abs(dp2 - dpmin) <= ptol)
                    {
                        mintech = mintech + "Gp";
                        ktrPg = ktrPg + 1;
                    }
                    if (Math.Abs(dp3 - dpmin) <= ptol)
                    {
                        mintech = mintech + "Dp";
                        ktrPd = ktrPd + 1;
                    }
                    if (Math.Abs(dp4 - dpmin) <= ptol)
                    {
                        mintech = mintech + "Op";
                        ktrPo = ktrPo + 1;
                    }

                    // --------------------- E
                    if (Math.Abs(decc1 - deccmin) <= etol)
                    {
                        mintech = mintech + "Le";
                        ktrEl = ktrEl + 1;
                    }
                    if (Math.Abs(decc2 - deccmin) <= etol)
                    {
                        mintech = mintech + "Ge";
                        ktrEg = ktrEg + 1;
                    }
                    if (Math.Abs(decc3 - deccmin) <= etol)
                    {
                        mintech = mintech + "De";
                        ktrEd = ktrEd + 1;
                    }
                    if (Math.Abs(decc4 - deccmin) <= etol)
                    {
                        mintech = mintech + "Oe";
                        ktrEo = ktrEo + 1;
                    }

                    // --------------------- I
                    if (Math.Abs(dincl1 - dinclmin) <= itol)
                    {
                        mintech = mintech + "Li";
                        ktrIl = ktrIl + 1;
                    }
                    if (Math.Abs(dincl2 - dinclmin) <= itol)
                    {
                        mintech = mintech + "Gi";
                        ktrIg = ktrIg + 1;
                    }
                    if (Math.Abs(dincl3 - dinclmin) <= itol)
                    {
                        mintech = mintech + "Di";
                        ktrId = ktrId + 1;
                    }
                    if (Math.Abs(dincl4 - dinclmin) <= itol)
                    {
                        mintech = mintech + "Oi";
                        ktrIo = ktrIo + 1;
                    }

                    strbuild.AppendLine(" mins "
                        + drmin.ToString(fmt2) + " " + dvmin.ToString(fmt2) + " "
                        + damin.ToString(fmt2) + " " + deccmin.ToString(fmt2) + " " + (dinclmin * rad).ToString(fmt2)
                        + " " + mintech + " ");
                    mintech = "";
                }  // a/ecc loop

            }  // dt loop

            Console.WriteLine("   R      V      A      P      E      I");
            Console.WriteLine(ktrRl.ToString() + " " + ktrVl.ToString() + " "
                + ktrAl.ToString() + " " + ktrPl.ToString() + " " + ktrEl.ToString() + " " + ktrIl.ToString() + " ");
            Console.WriteLine(ktrRg.ToString() + " " + ktrVg.ToString() + " "
                + ktrAg.ToString() + " " + ktrPg.ToString() + " " + ktrEg.ToString() + " " + ktrIg.ToString() + " ");
            Console.WriteLine(ktrRd.ToString() + " " + ktrVd.ToString() + " "
                + ktrAd.ToString() + " " + ktrPd.ToString() + " " + ktrEd.ToString() + " " + ktrId.ToString() + " ");
            Console.WriteLine(ktrRo.ToString() + " " + ktrVo.ToString() + " "
                + ktrAo.ToString() + " " + ktrPo.ToString() + " " + ktrEo.ToString() + " " + ktrIo.ToString() + " " );

            string directory = @"D:\Codes\LIBRARY\cs\TestAll\";
            File.WriteAllText(directory + "testalltoposum.out", strbuild.ToString());

        }  // test angles topology


        public void testlambertumins()
        {
            double[,] tbidu = new double[10, 3];
            double[,] tbiru = new double[10, 3];
            double[] r1 = new double[3];
            double[] r2 = new double[3];
            double tmin, tminp, tminenergy;
            int i;

            r1 = new double[] { 2.500000 * AstroLibr.gravConst.re, 0.000000, 0.000000 };
            r2 = new double[] { 1.9151111 * AstroLibr.gravConst.re, 1.6069690 * AstroLibr.gravConst.re, 0.000000 };
            char dm = 'S';
            char de = 'L';
            Int32 nrev = 0;

            // timing of routines
            var watch = System.Diagnostics.Stopwatch.StartNew();
            for (i = 0; i < 1000; i++)
            {
                for (int j = 1; j < 5; j++)
                    AstroLibr.lambertminT(r1, r2, dm, 'L', nrev, out tmin, out tminp, out tminenergy);

                for (int j = 1; j < 5; j++)
                    AstroLibr.lambertminT(r1, r2, dm, 'H', nrev, out tmin, out tminp, out tminenergy);
            }
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            strbuild.AppendLine("time for Lambert minT " + watch.ElapsedMilliseconds);

            double kbi, tof;

            // timing of routines
            watch = System.Diagnostics.Stopwatch.StartNew();
            for (i = 0; i < 1000; i++)
            {
                AstroLibr.lambertumins(r1, r2, 1, 'S', out kbi, out tof);
                tbidu[1, 1] = kbi;
                tbidu[1, 2] = tof;
                AstroLibr.lambertumins(r1, r2, 2, 'S', out kbi, out tof);
                tbidu[2, 1] = kbi;
                tbidu[2, 2] = tof;
                AstroLibr.lambertumins(r1, r2, 3, 'S', out kbi, out tof);
                tbidu[3, 1] = kbi;
                tbidu[3, 2] = tof;
                AstroLibr.lambertumins(r1, r2, 4, 'S', out kbi, out tof);
                tbidu[4, 1] = kbi;
                tbidu[4, 2] = tof;
                AstroLibr.lambertumins(r1, r2, 5, 'S', out kbi, out tof);
                tbidu[5, 1] = kbi;
                tbidu[5, 2] = tof;

                AstroLibr.lambertumins(r1, r2, 1, 'L', out kbi, out tof);
                tbiru[1, 1] = kbi;
                tbiru[1, 2] = tof;
                AstroLibr.lambertumins(r1, r2, 2, 'L', out kbi, out tof);
                tbiru[2, 1] = kbi;
                tbiru[2, 2] = tof;
                AstroLibr.lambertumins(r1, r2, 3, 'L', out kbi, out tof);
                tbiru[3, 1] = kbi;
                tbiru[3, 2] = tof;
                AstroLibr.lambertumins(r1, r2, 4, 'L', out kbi, out tof);
                tbiru[4, 1] = kbi;
                tbiru[4, 2] = tof;
                AstroLibr.lambertumins(r1, r2, 5, 'L', out kbi, out tof);
                tbiru[5, 1] = kbi;
                tbiru[5, 2] = tof;
            }
            watch.Stop();
            elapsedMs = watch.ElapsedMilliseconds;
            strbuild.AppendLine("time for Lambert umin " + watch.ElapsedMilliseconds);

            double[,] tbidk = new double[10, 3];
            double[,] tbirk = new double[10, 3];
            double tusec = 806.8111238242922;
            double ootusec = 1.0 / tusec;

            // timing of routines
            watch = System.Diagnostics.Stopwatch.StartNew();

            double s, tau;
            AstroLambertkLibr.lambertkmins1st(r1, r2, out s, out tau);

            for (i = 0; i < 1000; i++)
            {
                AstroLambertkLibr.lambertkmins(s, tau, 1, 'L', out kbi, out tof);
                tbidk[1, 1] = kbi;
                tbidk[1, 2] = tof * ootusec;
                AstroLambertkLibr.lambertkmins(s, tau, 2, 'L', out kbi, out tof);
                tbidk[2, 1] = kbi;
                tbidk[2, 2] = tof * ootusec;
                AstroLambertkLibr.lambertkmins(s, tau, 3, 'L', out kbi, out tof);
                tbidk[3, 1] = kbi;
                tbidk[3, 2] = tof * ootusec;
                AstroLambertkLibr.lambertkmins(s, tau, 4, 'L', out kbi, out tof);
                tbidk[4, 1] = kbi;
                tbidk[4, 2] = tof * ootusec;
                AstroLambertkLibr.lambertkmins(s, tau, 5, 'L', out kbi, out tof);
                tbidk[5, 1] = kbi;
                tbidk[5, 2] = tof * ootusec;

                AstroLambertkLibr.lambertkmins(s, tau, 1, 'H', out kbi, out tof);
                tbirk[1, 1] = kbi;
                tbirk[1, 2] = tof * ootusec;
                AstroLambertkLibr.lambertkmins(s, tau, 2, 'H', out kbi, out tof);
                tbirk[2, 1] = kbi;
                tbirk[2, 2] = tof * ootusec;
                AstroLambertkLibr.lambertkmins(s, tau, 3, 'H', out kbi, out tof);
                tbirk[3, 1] = kbi;
                tbirk[3, 2] = tof * ootusec;
                AstroLambertkLibr.lambertkmins(s, tau, 4, 'H', out kbi, out tof);
                tbirk[4, 1] = kbi;
                tbirk[4, 2] = tof * ootusec;
                AstroLambertkLibr.lambertkmins(s, tau, 5, 'H', out kbi, out tof);
                tbirk[5, 1] = kbi;
                tbirk[5, 2] = tof * ootusec;
            }

            watch.Stop();
            elapsedMs = watch.ElapsedMilliseconds;
            strbuild.AppendLine("time for Lambert kmin " + watch.ElapsedMilliseconds);
        }

        public void testlambertminT()
        {
            double[] r1 = new double[3];
            double[] r2 = new double[3];
            char dm, de;
            Int32 nrev;
            double tmin, tminp, tminenergy;
            r1 = new double[] { 2.500000 * AstroLibr.gravConst.re, 0.000000, 0.000000 };
            r2 = new double[] { 1.9151111 * AstroLibr.gravConst.re, 1.6069690 * AstroLibr.gravConst.re, 0.000000 };
            dm = 'S';
            de = 'L';
            nrev = 0;

            AstroLibr.lambertminT(r1, r2, dm, de, nrev, out tmin, out tminp, out tminenergy);
            strbuild.AppendLine("lambertminT tmin  s " + tmin.ToString(fmt) + " minp " + tminp.ToString(fmt) + 
                " minener " + tminenergy.ToString(fmt));

            AstroLibr.lambertminT(r1, r2, dm, 'H', nrev, out tmin, out tminp, out tminenergy);
            strbuild.AppendLine("lambertminT tmin  s " + tmin.ToString(fmt) + " minp " + tminp.ToString(fmt) +
                " minener " + tminenergy.ToString(fmt));
        }

        public void testlambhodograph()
        {
            double rad = 180.0 / Math.PI;
            double[] r1 = new double[3];
            double[] v1 = new double[3];
            double[] r2 = new double[3];
            double p, ecc, dnu, dtsec;
            double[] v1t;
            double[] v2t;
            r1 = new double[] { 2.500000 * AstroLibr.gravConst.re, 0.000000, 0.000000 };
            r2 = new double[] { 1.9151111 * AstroLibr.gravConst.re, 1.6069690 * AstroLibr.gravConst.re, 0.000000 };
            // assume circular initial orbit for vel calcs
            v1 = new double[] { 0.0, Math.Sqrt(AstroLibr.gravConst.mu / r1[0]), 0.0 };
            p = 12345.235;  // km
            ecc = 0.023487;
            dnu = 34.349128 / rad;
            dtsec = 92854.234;


            AstroLibr.lambhodograph(r1, r2, v1, p, ecc, dnu, dtsec, out v1t, out v2t);

            strbuild.AppendLine("lamb hod " + v1t[0].ToString(fmt) + " " + v1t[1].ToString(fmt) + " " + v1t[2].ToString(fmt) + " \nlamb hod" +
                   v2t[0].ToString(fmt) + " " + v2t[1].ToString(fmt) + " " + v2t[2].ToString(fmt));
        }

        public void testlambertbattin()
        {
            double[] r1 = new double[3];
            double[] v1 = new double[3];
            double[] r2 = new double[3];
            double[] v1t = new double[3];
            double[] v2t = new double[3];
            double dtsec, altpadc, dtwait;
            string errorsum = "";
            string errorout = "";
            char hitearth, dm, de;
            Int32 nrev;

            r1 = new double[] { 2.500000 * AstroLibr.gravConst.re, 0.000000, 0.000000 };
            r2 = new double[] { 1.9151111 * AstroLibr.gravConst.re, 1.6069690 * AstroLibr.gravConst.re, 0.000000 };
            dm = 'S';
            de = 'L';
            nrev = 0;
            dtsec = 76.0 * 60.0;
            altpadc = 100.0 / AstroLibr.gravConst.re;  //er
            dtwait = 0.0;

            AstroLibr.lambertbattin(r1, r2, v1, dm, de, nrev, dtwait, dtsec, altpadc, 'y', out v1t, out v2t, out hitearth, out errorsum, out errorout);

            strbuild.AppendLine("lambertbattin " + v1t[0].ToString(fmt) + " " + v1t[1].ToString(fmt) + " " + v1t[2].ToString(fmt) + " \nlambertbattin " +
                v2t[0].ToString(fmt) + " " + v2t[1].ToString(fmt) + " " + v2t[2].ToString(fmt));
        }

        public void testeq2rv()
        {
            double[] r = new double[3];
            double[] v = new double[3];
            double a, af, ag, chi, psi, meanlon;
            Int16 fr;
            a = 7236.346;
            af = 0.23457;
            ag = 0.47285;
            chi = 0.23475;
            psi = 0.28374;
            meanlon = 2.230482378;
            fr = 1;

            AstroLibr.eq2rv(a, af, ag, chi, psi, meanlon, fr, out r, out v);

            strbuild.AppendLine("eq2rv " + r[0].ToString(fmt) + " " + r[1].ToString(fmt) + " " + r[2].ToString(fmt) + " " +
                 v[0].ToString(fmt) + " " + v[1].ToString(fmt) + " " + v[2].ToString(fmt));
        }

        public void testrv2eq()
        {
            double[] r, v;
            double a, n, af, ag, chi, psi, meanlonM, meanlonNu;
            Int16 fr;

            r = new double[] { 2.500000 * AstroLibr.gravConst.re, 0.000000, 0.000000 };
            // assume circular initial orbit for vel calcs
            v = new double[] { 0.0, Math.Sqrt(AstroLibr.gravConst.mu / r[0]), 0.0 };

            AstroLibr.rv2eq(r, v, out a, out n, out af, out ag, out chi, out psi, out meanlonM, out meanlonNu, out fr);

            strbuild.AppendLine("rv2eq   a " + a.ToString(fmt) + " n " + n.ToString(fmt) + " af " + af.ToString(fmt) + " ag " 
                + ag.ToString(fmt) + " chi " + chi.ToString(fmt) + " psi " + psi.ToString(fmt) + " mm " + 
                meanlonM.ToString(fmt) + " mnu " + meanlonNu);
        }



        // test building the lambert envelope
        private void testAll()
        {
            double mu, tusec, dtwait, dtsec, dtseco;
            string detailSum, detailAll, errorout;
            char dm, de, hitearth, whichcase;
            Int32 ktr, ktr1, ktr2, ktr3, ktr4, i, iktr, nrev, numiter;
            double f, g, gdot;
            double tofu1, tofu2, kbiu1, kbiu2, tof, kbi;
            double[] v1t = new double[3];
            double[] v2t = new double[3];
            double[] r1 = new double[3];
            double[] r2 = new double[3];
            double[] v1 = new double[3];
            double[] v2 = new double[3];
            double s, tau;
            string outstr;
            StringBuilder strbuild = new StringBuilder();
            detailSum = "";
            detailAll = "";
            char show = 'n';     // for test180, show = n, show180 = y
            char show180 = 'n';  // for testlamb known show = y, show180 = n, n/n for envelope

            whichcase = 'k';  // k
            // whichcase = 'u'; //  universal
            // whichcase = 'b';  // battin

            dm = 's';
            de = 'r';
            double altpadc = 200.0 / 6378.137;  // set 200 km for altitude you set as the over limit. 
            ktr1 = 0;
            ktr2 = 0;
            ktr3 = 0;
            ktr4 = 0;
            tofu1 = 0.0;
            tofu2 = 0.0;
            kbiu1 = 0.0;
            kbiu2 = 0.0;

            mu = 3.986004415e5;
            tusec = 806.8111238242922;
            numiter = 17;
            dtwait = 0.0;
            tof = 0.0;
            kbi = 0.0;

            this.opsStatus.Text = "working on all cases ";
            Refresh();

            // book fig
            r1 = new double[] { 2.500000 * 6378.137, 0.000000, 0.000000 };
            r2 = new double[] { 1.9151111 * 6378.137, 1.6069690 * 6378.137, 0.000000 };
            // assume circular initial orbit for vel calcs
            v1 = new double[] { 0.0, Math.Sqrt(mu / r1[0]), 0.0 };
            double ang = Math.Atan(r2[1] / r2[0]);
            v2 = new double[] { -Math.Sqrt(mu / r2[1]) * Math.Cos(ang), Math.Sqrt(mu / r2[0]) * Math.Sin(ang), 0.0 };

            //// test case
            //double[] r2 = new double[] { -1105.78023519582, 2373.16130661458, 6713.89444816503 };
            //double[] v2 = new double[] { 5.4720951867079, -4.39299050886976, 2.45681739563752 };
            //double[] r1 = new double[] { 4938.49830042171, -1922.24810472241, 4384.68293292613 };
            //double[] v1 = new double[] { 0.738204644165659, 7.20989453238397, 2.32877392066299 };

            // more normal figure
            //double[] r2 = new double[] { -10000.0, 3750.0, 0.00 };
            //double[] v2 = new double[] { 5.4720951867079, -4.39299050886976, 2.45681739563752 };
            //double[] r1 = new double[] {7278.0,  0.00, 0.00};
            //double[] v1 = new double[] { 0.738204644165659, 7.20989453238397, 2.32877392066299 };

            // case 71 ld1 this is a 360/0 deg case
            //nrev = 1;
            //r1 = new double[] { -1467.02165038667, 1586.15766686882, 6812.89290230288 };
            //v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
            //r2 = new double[] { -1467.02165038667, 1586.15766686882, 6812.89290230288 };
            //v2 = new double[] { -0.631836782875836, 1.40386453042887, 2.14318960051298 };  // xx
            //dtsec = 6325.0;

            //double[] r2 = new double[] { 12214.84096602,  10249.46843675,      0.00000000};
            //double[] v2 = new double[] {-4.77718638,      3.67191377,      0.00000000 };
            //double[] r1 = new double[] { 15945.34250000,      0.00000000,      0.00000000 };
            //double[] v1 = new double[] {  0.00000000,      4.99979228,      0.00000000 };

            //// 179.9999972 test
            //r1 = new double[] { 5690.21923, 3309.62377, 1311.30504 };
            //v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
            //r2 = new double[] { -5691.9147514, -3310.6095425, -1311.695711 };
            //v2 = new double[] { -2.65211231086955, 1.55584941166386, -5.96175008776875E-07 };

            // near 0 180 deg test
            //r1 = new double[] { -1467.0216503866718, 1586.1576668688224, 6812.892902302875 };
            //v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
            //r2 = new double[] { 1482.87003576258, -1557.63131212973, -6827.43044367934 };
            //v2 = new double[] { -2.65211231086955, 1.55584941166386, -5.96175008776875E-07 };


            //r1 = new double[] { -1567.0216503866718, 1686.1576668688224, 6812.892902302875 };
            //v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
            //r2 = new double[] { 1482.87003576258, -1557.63131212973, -6827.43044367934 };
            //v2 = new double[] { -2.65211231086955, 1.55584941166386, -5.96175008776875E-07 };

            // case 25
            //nrev = 1;
            //r1 = new double[] { -1467.0216503866718, 1586.1576668688224, 6812.892902302875 };
            //v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
            //r2 = new double[] { -1271.900616, 2703.476434, 6306.177922 };
            //v2 = new double[] { 2.65211231086955, -1.55584941166386, 5.96175008776875E-07 };
            //dtsec = 5000.0; // 1sr

            // case 78 Nathaniel test case
            nrev = 1;
            r1 = new double[] { 7117.5156243154161, -4257.1942597683246, -583.88210887807986 };
            v1 = new double[] { -7.0417929290503141, -2.8681303717265290, 1.2224374606557487 };
            r2 = new double[] { 7172.3074180808417, -4123.2685183007470, -560.47505356742181 };
            v2 = new double[] { 5.9109725501309471, 2.0990367313315055, -1.1164901518784618 };  // xx


            // ----------------------------- put min values etc points on plot ---------------------------------
            double ktemp = -Math.Sqrt(2);
            hitearth = '-';

            // ----------------------------- put min values etc points on plot ---------------------------------
            double tmin, tminp, tminenergy;
            AstroLibr.lambertminT(r1, r2, 'S', 'L', 1, out tmin, out tminp, out tminenergy);
            detailSum = "S   L   1  0.000 " + tmin.ToString("0.#######").PadLeft(15) + " mint " + ktemp.ToString("0.#######").PadLeft(15) + " - 0";
            strbuild.AppendLine(detailSum);
            detailSum = "S   L   1  0.000 " + tmin.ToString("0.#######").PadLeft(15) + " mint " + (-ktemp).ToString("0.#######").PadLeft(15) + " - 0\n";
            strbuild.AppendLine(detailSum);
            AstroLibr.lambertminT(r1, r2, 'S', 'L', 2, out tmin, out tminp, out tminenergy);
            detailSum = "S   L   2  0.000 " + tmin.ToString("0.#######").PadLeft(15) + " mint " + ktemp.ToString("0.#######").PadLeft(15) + " - 0";
            strbuild.AppendLine(detailSum);
            detailSum = "S   L   2  0.000 " + tmin.ToString("0.#######").PadLeft(15) + " mint " + (-ktemp).ToString("0.#######").PadLeft(15) + " - 0\n";
            strbuild.AppendLine(detailSum);
            AstroLibr.lambertminT(r1, r2, 'S', 'L', 3, out tmin, out tminp, out tminenergy);
            detailSum = "S   L   3  0.000 " + tmin.ToString("0.#######").PadLeft(15) + " mint " + ktemp.ToString("0.#######").PadLeft(15) + " - 0";
            strbuild.AppendLine(detailSum);
            detailSum = "S   L   3  0.000 " + tmin.ToString("0.#######").PadLeft(15) + " mint " + (-ktemp).ToString("0.#######").PadLeft(15) + " - 0\n";
            strbuild.AppendLine(detailSum);

            AstroLibr.lambertminT(r1, r2, 'S', 'H', 1, out tmin, out tminp, out tminenergy);
            detailSum = "S   H   1  0.000 " + tmin.ToString("0.#######").PadLeft(15) + " mint " + ktemp.ToString("0.#######").PadLeft(15) + " - 0";
            strbuild.AppendLine(detailSum);
            detailSum = "S   H   1  0.000 " + tmin.ToString("0.#######").PadLeft(15) + " mint " + (-ktemp).ToString("0.#######").PadLeft(15) + " - 0\n";
            strbuild.AppendLine(detailSum);
            AstroLibr.lambertminT(r1, r2, 'S', 'H', 2, out tmin, out tminp, out tminenergy);
            detailSum = "S   H   2  0.000 " + tmin.ToString("0.#######").PadLeft(15) + " mint " + ktemp.ToString("0.#######").PadLeft(15) + " - 0";
            strbuild.AppendLine(detailSum);
            detailSum = "S   H   2  0.000 " + tmin.ToString("0.#######").PadLeft(15) + " mint " + (-ktemp).ToString("0.#######").PadLeft(15) + " - 0\n";
            strbuild.AppendLine(detailSum);
            AstroLibr.lambertminT(r1, r2, 'S', 'H', 3, out tmin, out tminp, out tminenergy);
            detailSum = "S   H   3  0.000 " + tmin.ToString("0.#######").PadLeft(15) + " mint " + ktemp.ToString("0.#######").PadLeft(15) + " - 0";
            strbuild.AppendLine(detailSum);
            detailSum = "S   H   3  0.000 " + tmin.ToString("0.#######").PadLeft(15) + " mint " + (-ktemp).ToString("0.#######").PadLeft(15) + " - 0\n";
            strbuild.AppendLine(detailSum);




            // do 0 rev cases first
            nrev = 0;
            ktr = 0;  // overall ktr
            for (iktr = 1; iktr <= 2; iktr++)
            {
                if (iktr == 1)
                {
                    dm = 'S';
                    de = 'L';   // or 'H'
                }
                if (iktr == 2)
                {
                    dm = 'L';
                    de = 'H';    // or 'L'
                }

                dtseco = 0.0;
                // calc the actual lambert values
                for (i = 1; i <= 500; i++)
                {
                    if (i < 60)
                        dtsec = dtseco + i * 1.0;
                    else
                    {
                        if (i < 200)
                            dtsec = dtseco + (i - 59) * 60.0;
                        else
                            dtsec = dtseco + (i - 185) * 600.0;
                    }
                    if (de == 'L')
                    {
                        if (whichcase == 'k')
                        {
                            AstroLambertkLibr.lambertK(r1, v1, r2, dm, de, nrev, dtwait, dtsec, 0.0, 0.0, numiter, altpadc, 'n', 'y',
                                 out v1t, out v2t, out f, out g, out gdot, out hitearth, out errorout, out detailSum, out detailAll);
                            if (detailAll.Contains("gnot"))
                            {
                                AstroLambertkLibr.lambertK(r1, v1, r2, dm, de, nrev, dtwait, dtsec, 0.0, 0.0, numiter, altpadc, 'n', 'y',
                                     out v1t, out v2t, out f, out g, out gdot, out hitearth, out errorout, out detailSum, out detailAll);
                            }
                        }
                        if (whichcase == 'b')
                            AstroLibr.lambertbattin(r1, r2, v1, dm, de, nrev, 0.0, dtsec, altpadc, 'n', out v1t, out v2t, out hitearth, out detailSum, out detailAll);
                        if (whichcase == 'u')
                            AstroLibr.lambertuniv(r1, r2, v1, dm, de, nrev, 0.0, dtsec, 0.0, altpadc, 'n', out v1t, out v2t, out hitearth, out detailSum, out detailAll);
                    }
                    else
                    {
                        if (whichcase == 'k')
                        {
                            AstroLambertkLibr.lambertK(r1, v1, r2, dm, de, nrev, dtwait, dtsec, 0.0, 0.0, numiter, altpadc, 'n', 'y',
                                 out v1t, out v2t, out f, out g, out gdot, out hitearth, out errorout, out detailSum, out detailAll);
                            if (detailAll.Contains("gnot"))
                            {
                                AstroLambertkLibr.lambertK(r1, v1, r2, dm, de, nrev, dtwait, dtsec, 0.0, 0.0, numiter, altpadc, 'n', 'y',
                                     out v1t, out v2t, out f, out g, out gdot, out hitearth, out errorout, out detailSum, out detailAll);
                            }
                        }
                        if (whichcase == 'b')
                            AstroLibr.lambertbattin(r1, r2, v1, dm, de, nrev, 0.0, dtsec, altpadc, 'n', out v1t, out v2t, out hitearth, out detailSum, out detailAll);
                        if (whichcase == 'u')
                            AstroLibr.lambertuniv(r1, r2, v1, dm, de, nrev, 0.0, dtsec, 0.0, altpadc, 'n', out v1t, out v2t, out hitearth, out detailSum, out detailAll);
                    }
                    ktr = ktr + 1;
                    if (whichcase == 'k' || whichcase == 'u')
                        strbuild.AppendLine(detailSum);
                    // if (detailAll.Contains("All"))
                    if (whichcase == 'b')
                        strbuild.AppendLine(detailSum);  // for battin tests
                }  // for i through all the times

                strbuild.AppendLine(" ");
                if (iktr == 1)
                    ktr1 = ktr;
                if (iktr == 2)
                    ktr2 = ktr;
            } // for iktr through cases

            strbuild.AppendLine(" ");

            AstroLambertkLibr.lambertkmins1st(r1, r2, out s, out tau);

            for (iktr = 1; iktr <= 4; iktr++)
            {
                if (iktr == 1)
                {
                    dm = 'S';
                    de = 'L';
                }
                if (iktr == 2)
                {
                    dm = 'S';
                    de = 'H';
                }
                if (iktr == 3)
                {
                    dm = 'L';
                    de = 'L';
                }
                if (iktr == 4)
                {
                    dm = 'L';
                    de = 'H';
                }

                for (nrev = 1; nrev <= 4; nrev++)
                {
                    dtseco = 0.0;
                    if (nrev > 0)
                    {
                        // secs
                        getmins(1, 'u', nrev, r1, r2, 0.0, 0.0, dm, de, out tofu1, out kbiu1, out tofu2, out kbiu2, out outstr);
                        // secs fix SH and LL cases
                        if (dm == 'S' && de == 'H')
                            AstroLambertkLibr.lambertkmins(s, tau, nrev, 'L', out kbi, out tof);
                        else
                        if (dm == 'L' && de == 'L')
                            AstroLambertkLibr.lambertkmins(s, tau, nrev, 'H', out kbi, out tof);
                        else
                            AstroLambertkLibr.lambertkmins(s, tau, nrev, de, out kbi, out tof);


                        //getmins(1, 'k', nrev, r1, r2, s, tau, dm, de, out tofk1, out kbik1, out tofk2, out kbik2, out outstr);

                        if (de == 'H')
                        {
                            if (whichcase == 'k')
                                dtseco = tof;  // in sec
                            if (whichcase == 'u')
                                dtseco = tofu2;
                            if (whichcase == 'b')
                                dtseco = tofu2;  // use the univ var value
                        }
                        else
                        {
                            if (whichcase == 'k')
                                dtseco = tof;
                            if (whichcase == 'u')
                                dtseco = tofu1;
                            if (whichcase == 'b')
                                dtseco = tofu1;  // use the univ var value
                        }
                    }

                    // calc the actual lambert values
                    for (i = 1; i <= 500; i++)
                    {
                        if (i < 60)
                            dtsec = dtseco + i * 1.0;
                        else
                        {
                            if (i < 200)
                                dtsec = dtseco + (i - 59) * 60.0;
                            else
                                dtsec = dtseco + (i - 185) * 600.0;
                        }
                        if (de == 'L')
                        {
                            if (whichcase == 'k')
                            {
                                AstroLambertkLibr.lambertK(r1, v1, r2, dm, de, nrev, dtwait, dtsec, tof, kbi, numiter, altpadc, 'n', 'y',
                                     out v1t, out v2t, out f, out g, out gdot, out hitearth, out errorout, out detailSum, out detailAll);
                                //if (detailAll.Contains("gnot"))
                                //{
                                //    AstroLambertkLibr.lambertK(r1, v1, r2, dm, de, nrev, dtwait, dtsec, tbi, kbi, numiter, altpadc, 'n', 'y',
                                //         out v1t, out v2t, out f, out g, out gdot, out hitearth, out errorout, out detailSum, out detailAll, 'y', show180);
                                //}
                            }
                            if (whichcase == 'b')
                                AstroLibr.lambertbattin(r1, r2, v1, dm, de, nrev, 0.0, dtsec, altpadc, 'n', out v1t, out v2t, out hitearth, out detailSum, out detailAll);
                            if (whichcase == 'u')
                                AstroLibr.lambertuniv(r1, r2, v1, dm, de, nrev, 0.0, dtsec, tofu1, altpadc, 'n', out v1t, out v2t, out hitearth, out detailSum, out detailAll);
                        }
                        else
                        {
                            if (whichcase == 'k')
                            {
                                AstroLambertkLibr.lambertK(r1, v1, r2, dm, de, nrev, dtwait, dtsec, tof, kbi, numiter, altpadc, 'n', 'y',
                                     out v1t, out v2t, out f, out g, out gdot, out hitearth, out errorout, out detailSum, out detailAll);
                                //if (detailAll.Contains("gnot"))
                                //{
                                //    AstroLambertkLibr.lambertK(r1, v1, r2, dm, de, nrev, dtwait, dtsec, tbi, kbi, numiter, altpadc, 'n', 'y',
                                //         out v1t, out v2t, out f, out g, out gdot, out hitearth, out errorout, out detailSum, out detailAll, 'y', show180);
                                //}
                            }
                            if (whichcase == 'b')
                                AstroLibr.lambertbattin(r1, r2, v1, dm, de, nrev, 0.0, dtsec, altpadc, 'n', out v1t, out v2t, out hitearth, out detailSum, out detailAll);
                            if (whichcase == 'u')
                                AstroLibr.lambertuniv(r1, r2, v1, dm, de, nrev, 0.0, dtsec, tofu2, altpadc, 'n', out v1t, out v2t, out hitearth, out detailSum, out detailAll);
                        }
                        ktr = ktr + 1;
                        if (whichcase == 'k' || whichcase == 'u')
                            strbuild.AppendLine(detailSum);
                        // if (detailAll.Contains("All"))
                        if (whichcase == 'b')
                            strbuild.AppendLine(detailSum);  // for battin tests
                    }  // for i through all the times

                    strbuild.AppendLine(" ");
                }  // if nrev > 0

                strbuild.AppendLine(" ");
                if (iktr == 2)
                    ktr3 = ktr;
                if (iktr == 4)
                    ktr4 = ktr;
            } // for iktr through cases

            strbuild.AppendLine("ktrs " + ktr1.ToString() + " " + ktr2.ToString() + " " + ktr3.ToString() + " " + ktr4.ToString() + " ");

            string directory = @"d:\codes\library\matlab\";
            File.WriteAllText(directory + "tlambertAll.out", strbuild.ToString());

            this.opsStatus.Text = "Done ";
            Refresh();
        }  // testAll


        /* ------------------------------------------------------------------------------
   *                                 testAllMoving 
   *                                    
   * calc the values needed to graph the whole envelope response. this is the most 
   * generic construction for the lambert solutions.
   *  you need to entere which lambert technique to use
   *      output these results separately to the matlab directory
   * 
   *  author        : david vallado             davallado@gmail.com  10 oct 2019
   *
   *  inputs        description                                   range / units
   *
   *  outputs       :
   *    
   *  locals        :
   *    r1          - ijk position vector 1                km
   *    r2          - ijk position vector 2                km
   *    v1          - ijk velocity vector 1 if avail       km/s
   *    dm          - direction of motion                  'L', 'S'
   *    de          - orbital energy                       'L', 'H'
   *    dtsec       - time between r1 and r2               sec
   *    dtwait      - time to wait before starting         sec
   *    nrev        - number of revs to complete           0, 1, 2, 3,  
   *    tbi         - array of times for nrev              [,] 
   *    altpad      - altitude pad for hitearth calc       km
   *    v1t         - ijk transfer velocity vector         km/s
   *    v2t         - ijk transfer velocity vector         km/s
   *    hitearth    - flag if hit or not                   'y', 'n'
   *    error       - error flag                           1, 2, 3,   use numbers since c++ is so horrible at strings
   *  
     ------------------------------------------------------------------------------*/

        public void testAllMoving()
        {
            double dtwait, dtsec, dtseco;
            string detailSum, detailAll, errorout;
            char dm, de, hitearth, whichcase;
            Int32 ktr, ktr1, ktr2, ktr3, ktr4, i, iktr, nrev, numiter;
            double f, g, gdot;
            double[] v1t = new double[3];
            double[] v2t = new double[3];
            double tofsh, kbish, toflg, kbilg, toflo, kbilo, tofhi, kbihi;
            string outstr;
            double[] r1 = new double[3];
            double[] r2 = new double[3];
            double[] v1 = new double[3];
            double[] v2 = new double[3];
            double[] r1o = new double[3];
            double[] r2o = new double[3];
            double[] v1o = new double[3];
            double[] v2o = new double[3];
            double[] dv1 = new double[3];
            double[] dv2 = new double[3];
            StringBuilder strbuild = new StringBuilder();
            StringBuilder strbuildDV = new StringBuilder();

            // initialize variables
            detailSum = "";
            detailAll = "";
            outstr = "";
            char show = 'y';     // for test180, show = n, show180 = y
            //char show180 = 'n';  // for testlamb known show = y, show180 = n, n/n for envelope
            tofsh = 0.0;
            kbish = 0.0;
            toflg = 0.0;
            kbilg = 0.0;
            toflo = 0.0;
            kbilo = 0.0;
            tofhi = 0.0;
            kbihi = 0.0;

            whichcase = 'k';
        //    whichcase = 'u';
        //    whichcase = 'b';

            dm = 'S';
            de = 'H';
            double altpadc = 200.0 / 6378.137;  // set 200 km for altitude you set as the over limit. 
            ktr1 = 0;
            ktr2 = 0;
            ktr3 = 0;
            ktr4 = 0;

            //mu = 3.986004415e5;
            //tusec = 806.8111238242922;
            numiter = 17;
            dtwait = 0.0;

            this.opsStatus.Text = "working on all cases ";
            Refresh();

            // book fig
            //   r1o = new double[] { 2.500000 * 6378.137, 0.000000, 0.000000 };
            //   r2o = new double[] { 1.9151111 * 6378.137, 1.6069690 * 6378.137, 0.000000 };

            // do reverse case to check retro direct!
            //r2 = new double[] { 2.500000 * 6378.137, 0.000000, 0.000000 };
            //r1 = new double[] { 1.9151111 * 6378.137, 1.6069690 * 6378.137, 0.000000 };

            // assume circular initial orbit for vel calcs
            //    v1o = new double[] { 0.0, Math.Sqrt(mu / r1o[0]), 0.0 };
            //    double ang = Math.Atan(r2o[1] / r2o[0]);
            //    v2o = new double[] { -Math.Sqrt(mu / r2o[1]) * Math.Cos(ang), Math.Sqrt(mu / r2o[0]) * Math.Sin(ang), 0.0 };

            // book fig 7-17 case tgt/int fixed
            r1o = new double[] { -6518.1083, -2403.8479, -22.1722 };
            v1o = new double[] { 2.604057, -7.105717, -0.263218 };
            r2o = new double[] { 6697.4756, 1794.5832, 0.000 };
            v2o = new double[] { -1.962373, 7.323674, 0.000 };

            // book fig 7-18 case tgt/int moving
            r1o = new double[] { -6175.1034, 2757.0706, 1626.6556 };
            v1o = new double[] { 2.376641, 1.139677, 7.078097 };
            r2o = new double[] { -1078.007289, 8796.641859, 1890.7135 };
            v2o = new double[] { 2.654700, 1.018600, 7.015400 };


            // nathaniel test case (1 rev)
            r1o = new double[] { 7117.5156243154161, -4257.1942597683246, -583.88210887807986 };
            v1o = new double[] { -7.0417929290503141, -2.8681303717265290, 1.2224374606557487 };
            r2o = new double[] { 7172.3074180808417, -4123.2685183007470, -560.47505356742181 };
            v2o = new double[] { 5.9109725501309471, 2.0990367313315055, -1.1164901518784618 };  // xx

            //double tmin, tminp, tminenergy;
            //AstroLibr.lambertminT(r1o, r2o, 'S', 'L', 0, out tmin, out tminp, out tminenergy);
            //AstroLibr.lambertminT(r1o, r2o, 'S', 'L', 1, out tmin, out tminp, out tminenergy);

            //double tmaxrp;
            //AstroLibr.lambertTmaxrp(r1o, r2o, 'S', 0, out tmaxrp, out v1t);
            //AstroLibr.lambertTmaxrp(r1o, r2o, 'S', 2, out tmaxrp, out v1t);

            //char opt = 'f';  // fixed target through dtsec
            char opt = 'm';  // moving target through dtsec

            // be sure to set the correct S/L, L/H below as needed

            // dtwait
            int lktr1 = 250;  // 250
            double step1 = 120.0;  // 120
            // dtsec
            int lktr2 = 100;  // 100
            double step2 = 120.0;  // 120

            // do a loop for dtwait
            for (int loopktr = 0; loopktr < lktr1; loopktr++)  // 0
            {
                dtwait = loopktr * step1;   // secs

                this.opsStatus.Text = "working dtwait = " + dtwait.ToString();
                Refresh();

                // propagate through dtwait
                AstroLibr.kepler(r1o, v1o, dtwait, out r1, out v1);

                hitearth = '-';

                // -------------- do 0 rev cases first
                nrev = 0;
                ktr = 0;  // overall ktr
                for (iktr = 1; iktr <= 1; iktr++)   // 1    2
                {
                    if (iktr == 1)
                    {
                        dm = 'S';
                        de = 'L';   // or 'H'
                    }
                    if (iktr == 2)
                    {
                        dm = 'L';
                        de = 'H';    // or 'L'
                    }

                    dtseco = 0.0;
                    // calc the actual lambert values
                    for (i = 1; i <= lktr2; i++)
                    {
                        dtsec = dtseco + i * step2;

                        // propagate through dtsec and dtwait
                        if (opt.Equals('m'))
                            AstroLibr.kepler(r2o, v2o, dtwait + dtsec, out r2, out v2);
                        else
                            AstroLibr.kepler(r2o, v2o, dtwait, out r2, out v2);

                        if (nrev > 0)
                            getmins(1, 'u', nrev, r1, r2, 0.0, 0.0, dm, de, out tofsh, out kbish, out toflg, out kbilg, out outstr);
                        // strbuild.AppendLine(outstr);
                        hitearth = '-';

                        if (de == 'L')
                        {
                            if (whichcase == 'k')
                            {
                                AstroLambertkLibr.lambertK(r1, v1, r2, dm, de, nrev, dtwait, dtsec, toflo, kbilo, numiter, altpadc, 'n', show,
                                     out v1t, out v2t, out f, out g, out gdot, out hitearth, out errorout, out detailSum, out detailAll);
                                // writeout details if there's a problem...probably not needed
                                //if (detailAll.Contains("gnot"))
                                //{
                                //    AstroLambertkLibr.lambertK(r1, v1, r2, dm, de, nrev, dtwait, dtsec, tbilk, numiter, altpadc,
                                //         out v1t, out v2t, out f, out g, out gdot, out hitearth, out errorout, out detailSum, out detailAll, 'y', show180);
                                //}
                            }
                            if (whichcase == 'b')
                                AstroLibr.lambertbattin(r1, r2, v1, dm, de, nrev, dtsec, dtwait, altpadc, 'y', out v1t, out v2t, out hitearth, out detailSum, out detailAll);
                            if (whichcase == 'u')
                            {
                                if (dm == 'S')
                                    AstroLibr.lambertuniv(r1, r2, v1, dm, de, nrev, dtwait, dtsec, kbish, altpadc, 'y', out v1t, out v2t, out hitearth, out detailSum, out detailAll);
                                else
                                    AstroLibr.lambertuniv(r1, r2, v1, dm, de, nrev, dtwait, dtsec, kbilg, altpadc, 'y', out v1t, out v2t, out hitearth, out detailSum, out detailAll);
                            }
                        }
                        else
                        {
                            if (whichcase == 'k')
                            {
                                AstroLambertkLibr.lambertK(r1, v1, r2, dm, de, nrev, dtwait, dtsec, tofhi, kbihi, numiter, altpadc, 'n', show,
                                     out v1t, out v2t, out f, out g, out gdot, out hitearth, out errorout, out detailSum, out detailAll);
                                //if (detailAll.Contains("gnot"))
                                //{
                                //    AstroLambertkLibr.lambertK(r1, v1, r2, dm, de, nrev, dtwait, dtsec, tbihk, numiter, altpadc,
                                //         out v1t, out v2t, out f, out g, out gdot, out hitearth, out errorout, out detailSum, out detailAll, 'y', show180);
                                //}
                            }
                            if (whichcase == 'b')
                                AstroLibr.lambertbattin(r1, r2, v1, dm, de, nrev, dtsec, dtwait, altpadc, 'y', out v1t, out v2t, out hitearth, out detailSum, out detailAll);
                            if (whichcase == 'u')
                            {
                                if (dm == 'S')
                                    AstroLibr.lambertuniv(r1, r2, v1, dm, de, nrev, dtwait, dtsec, kbish, altpadc, 'y', out v1t, out v2t, out hitearth, out detailSum, out detailAll);
                                else
                                    AstroLibr.lambertuniv(r1, r2, v1, dm, de, nrev, dtwait, dtsec, kbilg, altpadc, 'y', out v1t, out v2t, out hitearth, out detailSum, out detailAll);
                            }
                        }
                        ktr = ktr + 1;
                        //strbuild.AppendLine(detailSum);
                        dv1 = new double[] { 0.0, 0.0, 0.0 };
                        dv2 = new double[] { 0.0, 0.0, 0.0 };
                        if (MathTimeLibr.mag(v1t) > 0.0000001)
                        {
                            for (int ii = 0; ii < 3; ii++)
                            {
                                dv1[ii] = v1t[ii] - v1[ii];
                                dv2[ii] = v2t[ii] - v2[ii];
                            }
                        }
                        strbuild.AppendLine(detailSum + " " + MathTimeLibr.mag(dv1).ToString() + " " + MathTimeLibr.mag(dv2).ToString());
                        double magdv1 = MathTimeLibr.mag(dv1);
                        double magdv2 = MathTimeLibr.mag(dv2);
                        strbuildDV.AppendLine(dtwait.ToString("0.0000000").PadLeft(12) + " " +
                              dtsec.ToString("0.0000000").PadLeft(15) + " " +
                              magdv1.ToString() + " " + magdv2.ToString());  // + " " + dm + "  " + de);
                                                                             //   strbuild.AppendLine(detailAll);
                    }  // for i through all the times

                    strbuild.AppendLine(" ");
                    strbuildDV.AppendLine(" ");
                    if (iktr == 1)
                        ktr1 = ktr;
                    if (iktr == 2)
                        ktr2 = ktr;
                } // for iktr through cases

                //strbuild.AppendLine(" ");
                this.opsStatus.Text = "working dtwait = " + dtwait.ToString() + " now the 4 cases";
                Refresh();

                // set this for doing just the nrev cases
                getmins(1, 'u', nrev, r1, r2, 0.0, 0.0, dm, de, out tofsh, out kbish, out toflg, out kbilg, out outstr);

                for (iktr = 5; iktr <= 1; iktr++)  //4
                {
                    if (iktr == 1)
                    {
                        dm = 'S';
                        de = 'L';
                    }
                    if (iktr == 2)
                    {
                        dm = 'S';
                        de = 'H';
                    }
                    if (iktr == 3)
                    {
                        dm = 'L';
                        de = 'L';
                    }
                    if (iktr == 4)
                    {
                        dm = 'L';
                        de = 'H';
                    }

                    for (nrev = 1; nrev <= 1; nrev++)   // 1  4
                    {
                        dtseco = 0.0;
                        if (nrev > 0)
                        {
                            if (whichcase == 'k')
                            {
                                if (de == 'H')
                                    dtseco = toflo;// * tusec; // use univ for test
                                else
                                    dtseco = tofhi;// * tusec;  // use univ for test
                            }
                            else
                            {
                                // use univ for all these cases (univ and battin)
                                if (dm == 'L')
                                    dtseco = toflg;
                                else
                                    dtseco = tofsh;
                            }
                        }

                        // calc the actual lambert values
                        for (i = 1; i <= lktr2; i++)
                        {
                            dtsec = dtseco + i * step2;

                            // propagate through dtsec and dtwait
                            if (opt.Equals('m'))
                                AstroLibr.kepler(r2o, v2o, dtwait + dtsec, out r2, out v2);
                            else
                                AstroLibr.kepler(r2o, v2o, dtwait, out r2, out v2);

                            getmins(1, 'u', nrev, r1, r2, 0.0, 0.0, dm, de, out tofsh, out kbish, out toflg, out kbilg, out outstr);
                            // strbuild.AppendLine(outstr);
                            hitearth = '-';

                            if (de == 'L')
                            {
                                if (whichcase == 'k')
                                {
                                    AstroLambertkLibr.lambertK(r1, v1, r2, dm, de, nrev, dtwait, dtsec, toflo, kbilo, numiter, altpadc, 'n', show,
                                         out v1t, out v2t, out f, out g, out gdot, out hitearth, out errorout, out detailSum, out detailAll);
                                    //if (detailAll.Contains("gnot"))
                                    //{
                                    //    AstroLambertkLibr.lambertK(r1, v1, r2, dm, de, nrev, dtwait, dtsec, tbilk, numiter, altpadc,
                                    //         out v1t, out v2t, out f, out g, out gdot, out hitearth, out errorout, out detailSum, out detailAll, 'y', show180);
                                    //}
                                }
                                if (whichcase == 'b')
                                    AstroLibr.lambertbattin(r1, r2, v1, dm, de, nrev, dtsec, dtwait, altpadc, 'y', out v1t, out v2t, out hitearth, out detailSum, out detailAll);
                                if (whichcase == 'u')
                                {
                                    if (dm == 'S')
                                        AstroLibr.lambertuniv(r1, r2, v1, dm, de, nrev, dtwait, dtsec, kbish, altpadc, 'y', out v1t, out v2t, out hitearth, out detailSum, out detailAll);
                                    else
                                        AstroLibr.lambertuniv(r1, r2, v1, dm, de, nrev, dtwait, dtsec, kbilg, altpadc, 'y', out v1t, out v2t, out hitearth, out detailSum, out detailAll);
                                }
                            }
                            else
                            {
                                if (whichcase == 'k')
                                {
                                    AstroLambertkLibr.lambertK(r1, v1, r2, dm, de, nrev, dtwait, dtsec, tofhi, kbihi, numiter, altpadc, 'n', show,
                                         out v1t, out v2t, out f, out g, out gdot, out hitearth, out errorout, out detailSum, out detailAll);
                                    //if (detailAll.Contains("gnot"))
                                    //{
                                    //    AstroLambertkLibr.lambertK(r1, v1, r2, dm, de, nrev, dtwait, dtsec, tbihk, numiter, altpadc,
                                    //         out v1t, out v2t, out f, out g, out gdot, out hitearth, out errorout, out detailSum, out detailAll, 'y', show180);
                                    //}
                                }
                                if (whichcase == 'b')
                                    AstroLibr.lambertbattin(r1, r2, v1, dm, de, nrev, dtsec, dtwait, altpadc, 'y', out v1t, out v2t, out hitearth, out detailSum, out detailAll);
                                if (whichcase == 'u')
                                {
                                    if (dm == 'S')
                                        AstroLibr.lambertuniv(r1, r2, v1, dm, de, nrev, dtwait, dtsec, kbish, altpadc, 'y', out v1t, out v2t, out hitearth, out detailSum, out detailAll);
                                    else
                                        AstroLibr.lambertuniv(r1, r2, v1, dm, de, nrev, dtwait, dtsec, kbilg, altpadc, 'y', out v1t, out v2t, out hitearth, out detailSum, out detailAll);
                                }
                            }
                            ktr = ktr + 1;
                            //strbuild.AppendLine(detailSum);
                            dv1 = new double[] { 0.0, 0.0, 0.0 };
                            dv2 = new double[] { 0.0, 0.0, 0.0 };
                            if (MathTimeLibr.mag(v1t) > 0.0000001)
                            {
                                for (int ii = 0; ii < 3; ii++)
                                {
                                    dv1[ii] = v1t[ii] - v1[ii];
                                    dv2[ii] = v2t[ii] - v2[ii];
                                }
                            }
                            strbuild.AppendLine(detailSum + " " + MathTimeLibr.mag(dv1).ToString() + " " + MathTimeLibr.mag(dv2).ToString());
                            double magdv1 = MathTimeLibr.mag(dv1);
                            double magdv2 = MathTimeLibr.mag(dv2);
                            strbuildDV.AppendLine(dtwait.ToString("0.0000000").PadLeft(12) + " " +
                                  dtsec.ToString("0.0000000").PadLeft(15) + " " +
                                  magdv1.ToString() + " " + magdv2.ToString());  // + " " + dm + "  " + de);
                        }  // for i through all the times

                        strbuild.AppendLine(" ");
                        strbuildDV.AppendLine(" ");
                    }  // if nrev > 0

                    //                    strbuild.AppendLine(" ");
                    if (iktr == 2)
                        ktr3 = ktr;
                    if (iktr == 4)
                        ktr4 = ktr;
                } // for iktr through cases


            } // loop through dtwait

            strbuild.AppendLine("ktrs " + ktr1.ToString() + " " + ktr2.ToString() + " " + ktr3.ToString() + " " + ktr4.ToString() + " ");

            string directory = @"d:\codes\library\matlab\";
            File.WriteAllText(directory + "tlambertAllx.out", strbuild.ToString());
            File.WriteAllText(directory + "tlamb3dx.out", strbuildDV.ToString());

            this.opsStatus.Text = "Done ";
            Refresh();
        }  // testAllMoving


        /* ------------------------------------------------------------------------------
        *                                    getmins 
        *                                    
        * find tbi mins for k, lambert, etc. does either universal and k approach.
        *   universal seems to find them better though... could store in an array, but 
        *   faster to do 1 at a time. also note that the k lambert needs SH - LL, and LL - SH 
        *   reversal and it is done here. 
        *                                    
        *  author        : david vallado             davallado@gmail.com  10 oct 2019
        *
        *  inputs        description                                   range / units
        *    loopktr     - ktr for whether or not to write output           0 (writes)   
        *    app         - which approach to use                           'u', 'k'
        *    nrev        - number of revolultions 
        *    r1          - first position vector                            km
        *    r2          - second position vector                           km
        *    s           - parameter for k only, not needed for univ
        *    tau         - parameter for k only, not needed for univ
        *    dm          - parameter for k only, not needed for univ        'S', 'L'
        *    de          - parameter for k only, not needed for univ        'L', 'H'
        *    
        *  outputs       :
        *    tof         - min tof for the specified nrev                    s
        *    kbi         - min psi/k/etc for the given nrev 
        *    outstr      - output string if case 0
        *
        *  locals :
        *
        *  coupling      :
        *
         ------------------------------------------------------------------------------*/

        private void getmins
            (
                int loopktr, char app, int nrev, double[] r1, double[] r2, double s, double tau, char dm, char de,
                out double tof1, out double kbi1, out double tof2, out double kbi2, out string outstr
            )
        {
            StringBuilder strbuild = new StringBuilder();
            string detailSum;
            double tusec, tofsh, toflg, kbish, kbilg;
            double[] v1t = new double[3];
            double temp;
            tof1 = 0.0;
            kbi1 = 0.0;
            tof2 = 0.0;
            kbi2 = 0.0;

            //tusec = 806.8111238242922;
            if (nrev > 0)
            {

                if (app == 'u')
                {
                    // universal variable approach 
                    AstroLibr.lambertumins(r1, r2, nrev, 'S', out kbi1, out tof1);
                    AstroLibr.lambertumins(r1, r2, nrev, 'L', out kbi2, out tof2);
                }
                else
                {
                    // -----------do these calcs one time to save time
                    // call this outside getmins
                    // AstroLambertkLibr.lambertkmins1st(r1, r2, out s, out tau);

                    // k value approaches
                    AstroLambertkLibr.lambertkmins(s, tau, nrev, 'L', out kbi1, out tof1);
                    //tof1 = tof1 / tusec;  // in tu, tof is in secs
                    AstroLambertkLibr.lambertkmins(s, tau, nrev, 'H', out kbi2, out tof2);
                    //tof2 = tof2 / tusec;
                    // switch these here so it's not needed elsewhere
                    if ((dm == 'S' && de == 'H') || ((dm == 'L' && de == 'L')))
                    {
                        temp = kbi1;
                        kbi1 = kbi2;
                        kbi2 = temp;
                        temp = tof1;
                        tof1 = tof2;
                        tof2 = temp;
                    }
                }

                // -----------------------------put min values etc points on plot ---------------------------------
                // writeout just one time
                if (loopktr == 0)
                {
                    strbuild.AppendLine("Lambertumin");
                    detailSum = "S   L   1  0.000 " + tof1.ToString("0.#######").PadLeft(15) + " psimin " + kbi1.ToString("0.#######").PadLeft(15) + " -0\n";
                    strbuild.AppendLine(detailSum);

                    strbuild.AppendLine("Lambertkmin");
                    detailSum = "S   L   1  0.000 " + (tof1).ToString("0.#######").PadLeft(15) + " kmin " + kbi1.ToString("0.#######").PadLeft(15) + " -0\n";
                    strbuild.AppendLine(detailSum);

                    // -----------------------------put min values etc points on plot ---------------------------------
                    double tmin, tminp, tminenergy;
                    strbuild.AppendLine("Lamberttmin");
                    AstroLibr.lambertminT(r1, r2, 'S', 'L', 1, out tmin, out tminp, out tminenergy);
                    detailSum = "S   L   tminp  0.000 " + tminp.ToString("0.#######").PadLeft(15) + "5.0000000".PadLeft(15) + " -0";
                    strbuild.AppendLine(detailSum);
                    detailSum = "S   L   tminp  0.000 " + tminp.ToString("0.#######").PadLeft(15) + "-5.0000000".PadLeft(15) + " -0\n";
                    strbuild.AppendLine(detailSum);

                    detailSum = "S   L   tminenergy  0.000 " + tminenergy.ToString("0.#######").PadLeft(15) + "5.0000000".PadLeft(15) + " -0";
                    strbuild.AppendLine(detailSum);
                    detailSum = "S   L   tminenergy  0.000 " + tminenergy.ToString("0.#######").PadLeft(15) + "-5.0000000".PadLeft(15) + " -0\n";
                    strbuild.AppendLine(detailSum);

                    AstroLibr.lambertumins(r1, r2, 1, 'S', out kbish, out tofsh);
                    detailSum = "S   L   1  0.000 " + tmin.ToString("0.#######").PadLeft(15) + (tofsh - 5).ToString("0.#######").PadLeft(15) + " -0";
                    strbuild.AppendLine(detailSum);
                    detailSum = "S   L   1  0.000 " + tmin.ToString("0.#######").PadLeft(15) + (tofsh + 5).ToString("0.#######").PadLeft(15) + " -0\n";
                    strbuild.AppendLine(detailSum);
                    AstroLibr.lambertumins(r1, r2, 2, 'S', out kbish, out tofsh);
                    AstroLibr.lambertminT(r1, r2, 'S', 'L', 2, out tmin, out tminp, out tminenergy);
                    detailSum = "S   L   2  0.000 " + tmin.ToString("0.#######").PadLeft(15) + (tofsh - 5).ToString("0.#######").PadLeft(15) + " -0";
                    strbuild.AppendLine(detailSum);
                    detailSum = "S   L   2  0.000 " + tmin.ToString("0.#######").PadLeft(15) + (tofsh + 5).ToString("0.#######").PadLeft(15) + " -0\n";
                    strbuild.AppendLine(detailSum);
                    AstroLibr.lambertumins(r1, r2, 3, 'S', out kbish, out tofsh);
                    AstroLibr.lambertminT(r1, r2, 'S', 'L', 3, out tmin, out tminp, out tminenergy);
                    detailSum = "S   L   3  0.000 " + tmin.ToString("0.#######").PadLeft(15) + (tofsh - 5).ToString("0.#######").PadLeft(15) + " -0";
                    strbuild.AppendLine(detailSum);
                    detailSum = "S   L   3  0.000 " + tmin.ToString("0.#######").PadLeft(15) + (tofsh + 5).ToString("0.#######").PadLeft(15) + " -0\n";
                    strbuild.AppendLine(detailSum);

                    AstroLibr.lambertminT(r1, r2, 'L', 'H', 1, out tmin, out tminp, out tminenergy);
                    detailSum = "L   H   tminp  0.000 " + tminp.ToString("0.#######").PadLeft(15) + "5.0000000".PadLeft(15) + " -0";
                    strbuild.AppendLine(detailSum);
                    detailSum = "L   H   tminp  0.000 " + tminp.ToString("0.#######").PadLeft(15) + "-5.0000000".PadLeft(15) + " -0\n";
                    strbuild.AppendLine(detailSum);

                    detailSum = "L   H   tminenergy  0.000 " + tminenergy.ToString("0.#######").PadLeft(15) + "5.0000000".PadLeft(15) + " -0";
                    strbuild.AppendLine(detailSum);
                    detailSum = "L   H   tminenergy  0.000 " + tminenergy.ToString("0.#######").PadLeft(15) + "-5.0000000".PadLeft(15) + " -0\n";
                    strbuild.AppendLine(detailSum);

                    AstroLibr.lambertumins(r1, r2, 1, 'L', out kbilg, out toflg);
                    detailSum = "L   H   1  0.000 " + tmin.ToString("0.#######").PadLeft(15) + (toflg - 5).ToString("0.#######").PadLeft(15) + " -0";
                    strbuild.AppendLine(detailSum);
                    detailSum = "L   H   1  0.000 " + tmin.ToString("0.#######").PadLeft(15) + (toflg + 5).ToString("0.#######").PadLeft(15) + " -0\n";
                    strbuild.AppendLine(detailSum);
                    AstroLibr.lambertumins(r1, r2, 2, 'L', out kbilg, out toflg);
                    AstroLibr.lambertminT(r1, r2, 'S', 'H', 2, out tmin, out tminp, out tminenergy);
                    detailSum = "L   H   2  0.000 " + tmin.ToString("0.#######").PadLeft(15) + (toflg - 5).ToString("0.#######").PadLeft(15) + " -0";
                    strbuild.AppendLine(detailSum);
                    detailSum = "L   H   2  0.000 " + tmin.ToString("0.#######").PadLeft(15) + (toflg + 5).ToString("0.#######").PadLeft(15) + " -0\n";
                    strbuild.AppendLine(detailSum);
                    AstroLibr.lambertumins(r1, r2, 3, 'L', out kbilg, out toflg);
                    AstroLibr.lambertminT(r1, r2, 'S', 'H', 3, out tmin, out tminp, out tminenergy);
                    detailSum = "L   H   3  0.000 " + tmin.ToString("0.#######").PadLeft(15) + (toflg - 5).ToString("0.#######").PadLeft(15) + " -0";
                    strbuild.AppendLine(detailSum);
                    detailSum = "L   H   3  0.000 " + tmin.ToString("0.#######").PadLeft(15) + (toflg + 5).ToString("0.#######").PadLeft(15) + " -0\n";
                    strbuild.AppendLine(detailSum);

                    // find max rp values for each nrev
                    double tmaxrp;
                    AstroLibr.lambertTmaxrp(r1, r2, 'S', 0, out tmaxrp, out v1t);
                    strbuild.AppendLine("x   x   0  0.000 " + tmaxrp.ToString() + " " +
                        v1t[0].ToString("0.0000000").PadLeft(15) + v1t[1].ToString("0.0000000").PadLeft(15) + v1t[2].ToString("0.0000000").PadLeft(15));
                    strbuild.AppendLine("x   x   0  0.000 " + tmaxrp.ToString() + " " +
                        v1t[0].ToString("0.0000000").PadLeft(15) + v1t[1].ToString("0.0000000").PadLeft(15) + v1t[2].ToString("0.0000000").PadLeft(15) + "\n");
                    AstroLibr.lambertTmaxrp(r1, r2, 'S', 1, out tmaxrp, out v1t);
                    strbuild.AppendLine("x   x   1  0.000 " + tmaxrp.ToString() + " " +
                        v1t[0].ToString("0.0000000").PadLeft(15) + v1t[1].ToString("0.0000000").PadLeft(15) + v1t[2].ToString("0.0000000").PadLeft(15));
                    strbuild.AppendLine("x   x   1  0.000 " + tmaxrp.ToString() + " " +
                        v1t[0].ToString("0.0000000").PadLeft(15) + v1t[1].ToString("0.0000000").PadLeft(15) + v1t[2].ToString("0.0000000").PadLeft(15) + "\n");
                    AstroLibr.lambertTmaxrp(r1, r2, 'S', 2, out tmaxrp, out v1t);
                    strbuild.AppendLine("x   x   2  0.000 " + tmaxrp.ToString() + " " +
                        v1t[0].ToString("0.0000000").PadLeft(15) + v1t[1].ToString("0.0000000").PadLeft(15) + v1t[2].ToString("0.0000000").PadLeft(15));
                    strbuild.AppendLine("x   x   2  0.000 " + tmaxrp.ToString() + " " +
                        v1t[0].ToString("0.0000000").PadLeft(15) + v1t[1].ToString("0.0000000").PadLeft(15) + v1t[2].ToString("0.0000000").PadLeft(15) + "\n");
                }
            }  // if nrev > 0

            outstr = strbuild.ToString();
        }   // getmins


        /* ------------------------------------------------------------------------------
        *                                      makesurf 
        *                                    
        *  make a surface from a fixdat result with numbers takes a text file of number of points, 
        *  then all the points, and cross-hatches it to get a surface. you run fixdat first 
        *  in most cases.
        *                                    
        *  author        : david vallado             davallado@gmail.com  10 oct 2019
        *
        *  inputs        description                                   range / units
        *    infilename  - in filename  
        *    outfilename - out filename  
        *    
        *  outputs       :
        *
        *  locals :
        *
        *  coupling      :
        *
          ------------------------------------------------------------------------------*/

        private void makesurf
            (
        string infilename,
        string outfilename
            )
        {
            Int32 ktr, numPerLine, NumLines, i, j;
            string line, line1, Restoflgine;
            string[] linesplt;
            StringBuilder strbuild = new StringBuilder();
            Restoflgine = "";

            string[] fileData = File.ReadAllLines(infilename);

            // process all the x lines 
            numPerLine = 0;
            ktr = 0;     // reset the file
            NumLines = 0;
            while (ktr < fileData.Count() - 1)  // not eof
            {
                line = fileData[ktr];
                linesplt = line.Split(' ');
                numPerLine = Convert.ToInt32(linesplt[0]);
                // matlab uses Inf or Nan to start a new line
                // needs to be in each col as well
                strbuild.AppendLine(" Nan Nan NaN NaN NaN NaN");  // numPerLine.ToString() +
                ktr = ktr + 1;
                NumLines = NumLines + 1;

                for (i = 1; i <= numPerLine; i++)
                {
                    line = fileData[ktr];
                    line1 = Regex.Replace(line, @"\s+", " ");
                    linesplt = line1.Split(' ');
                    //int posrest = line1.IndexOf(linesplt[2], 15); // start at position 3
                    //Restoflgine = line1.Substring(posrest -1, line1.Length -posrest);
                    Restoflgine = linesplt[2].ToString() + " " + linesplt[3].ToString() + " " +
                                 linesplt[4].ToString() + " " + linesplt[5].ToString();
                    strbuild.AppendLine(linesplt[0].ToString() + " " + linesplt[1].ToString() + " " + Restoflgine);
                    ktr = ktr + 1;
                }
            }

            // ------process the y lines-------
            // 'process y lines ');
            // the number of lines needs to be constant!! 
            //numPerLine = 0;
            int numinrow = 0;  // position of each y line
            // go through each point in initial line
            while (numinrow < numPerLine)
            {
                this.opsStatus.Text = "Done with line " + numinrow.ToString();
                Refresh();

                // ---get the nth point from the first row---
                ktr = 0;  // reset the file
                //line = fileData[ktr];
                //linesplt = line.Split(' ');
                //k = Convert.ToInt32(linesplt[0]);
                strbuild.AppendLine(" Nan Nan NaN NaN NaN NaN");  // k.ToString() +
                ktr = ktr + 1 + numinrow;  // get to first point of data
                line = fileData[ktr];
                line1 = Regex.Replace(line, @"\s+", " ");
                linesplt = line1.Split(' ');
                Restoflgine = linesplt[2].ToString() + " " + linesplt[3].ToString() + " " +
                             linesplt[4].ToString() + " " + linesplt[5].ToString();
                strbuild.AppendLine(linesplt[0].ToString() + " " + linesplt[1].ToString() + " " + Restoflgine);

                // ---get nth number from each other segment---
                // since they are all evenly spaced, simply add the delta until the end of file
                Int32 ktr0 = ktr + 1;
                for (j = 1; j < NumLines; j++)
                {
                    ktr = ktr0 + j * numPerLine;
                    line = fileData[ktr];
                    line1 = Regex.Replace(line, @"\s+", " ");
                    linesplt = line1.Split(' ');
                    Restoflgine = linesplt[2].ToString() + " " + linesplt[3].ToString() + " " +
                                 linesplt[4].ToString() + " " + linesplt[5].ToString();
                    strbuild.AppendLine(linesplt[0].ToString() + " " + linesplt[1].ToString() + " " + Restoflgine);
                    ktr0 = ktr0 + 1;
                }

                numinrow = numinrow + 1;
            }  // while

            string directory = @"d:\codes\library\matlab\";
            File.WriteAllText(directory + "surf.out", strbuild.ToString());
        }  // makesurf



        /* ------------------------------------------------------------------------------
        *                                      fixdat 
        *                                    
        *  fix the blank lines in a datafile. let 4 values be taken depending on the 
        *  intindex values inserts the number of points for each segment, then it can be 
        *  used in makesurf.
        *                                    
        *  author        : david vallado             davallado@gmail.com  10 oct 2019
        *
        *  inputs        description                                   range / units
        *    infilename  - in filename  
        *    outfilename - out filename  
        *    intindxx    - which indices to use, 1.2.3.4.5, 2.5.7.8 etc
        *    
        *  outputs       :
        *
        *  locals :
        *
          ------------------------------------------------------------------------------*/

        private void fixdat
            (
            string infilename,
            string outfilename,
            int intindx1, int intindx2, int intindx3, int intindx4, int intindx5, int intindx6
            )
        {
            Int32 ktr, i, j;
            string LongString;
            string[] DatArray = new string[2001];
            StringBuilder strbuild = new StringBuilder();
            LongString = "";

            string[] fileData = File.ReadAllLines(infilename);

            i = 0;
            ktr = 0;
            while (ktr < fileData.Count() - 1)  // not eof
            {
                LongString = fileData[ktr];

                if ((LongString.Contains("xx")) || LongString.Length < 10 || (i == 2000))
                {
                    this.opsStatus.Text = "Break " + ktr.ToString();
                    Refresh();

                    // ----Put a mandatory break at 2000----
                    if ((i == 2000) && (!LongString.Contains("xx")))
                        strbuild.AppendLine((i + 1).ToString() + " xx broken");
                    else
                    {
                        if (i > 0)
                            strbuild.AppendLine(i.ToString() + " xx ");
                    }

                    // ----Write out all the data to this point----
                    for (j = 1; j <= i; j++)
                        strbuild.AppendLine(DatArray[j].ToString());

                    // ----Write out crossover point for the mandatory break ---
                    if ((i == 2000) && ((!LongString.Contains('x')) || LongString.Length > 10))
                    {
                        strbuild.AppendLine(LongString);
                        // write out last one in loop ! 
                        i = 1;
                        DatArray[i] = LongString;
                    }
                    else
                    {
                        i = 0;
                        ktr = ktr + 1;   // file counter
                    }
                }
                else
                // ----Add new data ----
                {
                    i = i + 1;
                    string line1 = Regex.Replace(LongString, @"\s+", " ");
                    string[] linesplt = line1.Split(' ');

                    DatArray[i] = linesplt[intindx1] + " " + linesplt[intindx2] + " " + linesplt[intindx3] + " " + linesplt[intindx4]
                        + " " + linesplt[intindx5] + " " + linesplt[intindx6];
                    ktr = ktr + 1;   // file counter
                }

            }  // while through file

            // ----Write out the last section----
            if (i > 1)
            {
                if ((i == 2000) && (!LongString.Contains('x')) && LongString.Length > 10)
                    strbuild.AppendLine((i + 1).ToString() + " xx broken");
                else
                {
                    if (i > 0)
                        strbuild.AppendLine(i.ToString() + " xx ");
                }
                for (j = 1; j <= i; j++)
                    strbuild.AppendLine(DatArray[j]);
            }

            string directory = @"d:\codes\library\matlab\";
            File.WriteAllText(directory + "t.out", strbuild.ToString());
        }  // fixdat


        public void testlambertuniv()
        {
            double[] v1t = new double[3];
            double[] v2t = new double[3];
            double[] v1t1 = new double[3];
            double[] v2t1 = new double[3];
            double[] v1t2 = new double[3];
            double[] v2t2 = new double[3];
            double[] v1t3 = new double[3];
            double[] v2t3 = new double[3];
            double[] v1t4 = new double[3];
            double[] v2t4 = new double[3];
            double[,] tbidu = new double[10, 3];
            double[,] tbiru = new double[10, 3];
            double[] r1 = new double[3];
            double[] r2 = new double[3];
            double[] r3 = new double[3];
            double[] r4 = new double[3];
            double[] v1 = new double[3];
            double[] dv1 = new double[3];
            double[] dv11 = new double[3];
            double[] dv12 = new double[3];
            double[] dv13 = new double[3];
            double[] dv14 = new double[3];
            double[] v2 = new double[3];
            double[] v3 = new double[3];
            double[] v4 = new double[3];
            double[] dv2 = new double[3];
            double[] dv21 = new double[3];
            double[] dv22 = new double[3];
            double[] dv23 = new double[3];
            double[] dv24 = new double[3];
            double kbi, tof, dtwait, altpad, ang, dtsec;
            Int32 nrev, i, j, k;
            string errorsum = "";
            string errorout = "";
            char show = 'n';     // for test180, show = n, show180 = y
            //char show180 = 'n';  // for testlamb known show = y, show180 = n, n/n for envelope
            char hitearth, dm, de;
            nrev = 0;
            r1 = new double[] { 2.500000 * AstroLibr.gravConst.re, 0.000000, 0.000000 };
            r2 = new double[] { 1.9151111 * AstroLibr.gravConst.re, 1.6069690 * AstroLibr.gravConst.re, 0.000000 };
            // assume circular initial orbit for vel calcs
            v1 = new double[] { 0.0, Math.Sqrt(AstroLibr.gravConst.mu / r1[0]), 0.0 };
            ang = Math.Atan(r2[1] / r2[0]);
            v2 = new double[] { -Math.Sqrt(AstroLibr.gravConst.mu / r2[1]) * Math.Cos(ang), Math.Sqrt(AstroLibr.gravConst.mu / r2[0]) * Math.Sin(ang), 0.0 };
            dtsec = 99900.3;
            dtsec = 76.0 * 60.0;
            altpad = 100.0;  // 100 km


            AstroLibr.lambertumins(r1, r2, 1, 'S', out kbi, out tof);
            tbidu[1, 1] = kbi;
            tbidu[1, 2] = tof;
            AstroLibr.lambertumins(r1, r2, 2, 'S', out kbi, out tof);
            tbidu[2, 1] = kbi;
            tbidu[2, 2] = tof;
            AstroLibr.lambertumins(r1, r2, 3, 'S', out kbi, out tof);
            tbidu[3, 1] = kbi;
            tbidu[3, 2] = tof;
            AstroLibr.lambertumins(r1, r2, 4, 'S', out kbi, out tof);
            tbidu[4, 1] = kbi;
            tbidu[4, 2] = tof;
            AstroLibr.lambertumins(r1, r2, 5, 'S', out kbi, out tof);
            tbidu[5, 1] = kbi;
            tbidu[5, 2] = tof;

            AstroLibr.lambertumins(r1, r2, 1, 'L', out kbi, out tof);
            tbiru[1, 1] = kbi;
            tbiru[1, 2] = tof;
            AstroLibr.lambertumins(r1, r2, 2, 'L', out kbi, out tof);
            tbiru[2, 1] = kbi;
            tbiru[2, 2] = tof;
            AstroLibr.lambertumins(r1, r2, 3, 'L', out kbi, out tof);
            tbiru[3, 1] = kbi;
            tbiru[3, 2] = tof;
            AstroLibr.lambertumins(r1, r2, 4, 'L', out kbi, out tof);
            tbiru[4, 1] = kbi;
            tbiru[4, 2] = tof;
            AstroLibr.lambertumins(r1, r2, 5, 'L', out kbi, out tof);
            tbiru[5, 1] = kbi;
            tbiru[5, 2] = tof;


            if (show == 'y')
                strbuild.AppendLine(" TEST ------------------ s/l  d  0 rev ------------------");
            hitearth = ' ';
            dm = 'S';
            de = 'L';
            nrev = 0;
            dtwait = 0.0;


            AstroLibr.lambertuniv(r1, r2, v1, dm, de, nrev, dtwait, dtsec, kbi,
                          altpad, 'y', out v1t, out v2t, out hitearth, out errorsum, out errorout);




            // test chap 7 fig 7-18 runs
            if (show == 'y')
                strbuild.AppendLine(" TEST ------------------ s/l  d  0 rev ------------------");
            hitearth = ' ';
            dm = 'S';
            de = 'L';
            nrev = 0;
            dtwait = 0.0;

            // fig 7-18 fixed tgt and int
            r1 = new double[] { -6518.1083, -2403.8479, -22.1722 };
            v1 = new double[] { 2.604057, -7.105717, -0.263218 };
            r2 = new double[] { 6697.4756, 1794.5832, 0.0 };
            v2 = new double[] { -1.962373, 7.323674, 0.0 };
            strbuild.AppendLine("dtwait  dtsec       dv1       dv2 ");
            this.opsStatus.Text = "Status:  on case 80a";
            Refresh();
 //           for (i = 0; i < 250; i++)
            {
                i = 0;
                dtsec = i * 60.0;
                AstroLibr.lambertuniv(r1, r2, v1, 'S', 'L', nrev, dtwait, dtsec, kbi,
                              altpad, 'y', out v1t1, out v2t1, out hitearth, out errorsum, out errorout);
                AstroLibr.lambertuniv(r1, r2, v1, 'S', 'H', nrev, dtwait, dtsec, kbi,
                              altpad, 'y', out v1t2, out v2t2, out hitearth, out errorsum, out errorout);
                AstroLibr.lambertuniv(r1, r2, v1, 'L', 'L', nrev, dtwait, dtsec, kbi,
                              altpad, 'y', out v1t3, out v2t3, out hitearth, out errorsum, out errorout);
                AstroLibr.lambertuniv(r1, r2, v1, 'L', 'H', nrev, dtwait, dtsec, kbi,
                              altpad, 'y', out v1t4, out v2t4, out hitearth, out errorsum, out errorout);

                if (errorout.Contains("ok"))
                {
                    MathTimeLibr.addvec(1.0, v1t1, -1.0, v1, out dv11);
                    MathTimeLibr.addvec(1.0, v2t1, -1.0, v2, out dv21);
                    MathTimeLibr.addvec(1.0, v1t2, -1.0, v1, out dv12);
                    MathTimeLibr.addvec(1.0, v2t2, -1.0, v2, out dv22);
                    MathTimeLibr.addvec(1.0, v1t3, -1.0, v1, out dv13);
                    MathTimeLibr.addvec(1.0, v2t3, -1.0, v2, out dv23);
                    MathTimeLibr.addvec(1.0, v1t4, -1.0, v1, out dv14);
                    MathTimeLibr.addvec(1.0, v2t4, -1.0, v2, out dv24);
                    strbuild.AppendLine(dtwait.ToString() + " " + dtsec.ToString() +
                        "  " + MathTimeLibr.mag(dv11).ToString() + "  " + MathTimeLibr.mag(dv21).ToString() +
                        "  " + MathTimeLibr.mag(dv12).ToString() + "  " + MathTimeLibr.mag(dv22).ToString() +
                        "  " + MathTimeLibr.mag(dv13).ToString() + "  " + MathTimeLibr.mag(dv23).ToString() +
                        "  " + MathTimeLibr.mag(dv14).ToString() + "  " + MathTimeLibr.mag(dv24).ToString());
                }
                else
                    strbuild.AppendLine(errorsum + " " + errorout);
            }


            // fig 7-19 moving tgt
            r1 = new double[] { 5328.7862, 4436.1273, 101.4720 };
            v1 = new double[] { -4.864779, 5.816486, 0.240163 };
            r2 = new double[] { 6697.4756, 1794.5831, 0.0 };
            v2 = new double[] { -1.962372, 7.323674, 0.0 };
            strbuild.AppendLine("dtwait  dtsec       dv1       dv2 ");
            // diff vectors, needs new umins

            this.opsStatus.Text = "Status:  on case 80b";
            Refresh();
 //           for (i = 0; i < 250; i++)
            {
                i = 0;
                dtsec = i * 60.0;
                AstroLibr.kepler(r2, v2, dtsec, out r3, out v3);
//                for (j = 0; j < 25; j++)
                {
                    j = 0;
                    dtwait = j * 10.0;
                    dtwait = 0.0;  // set to 0 for now

                    AstroLibr.lambertuniv(r1, r3, v1, 'S', 'L', nrev, dtwait, dtsec, kbi,
                                  altpad, 'y', out v1t1, out v2t1, out hitearth, out errorsum, out errorout);
                    AstroLibr.lambertuniv(r1, r3, v1, 'S', 'H', nrev, dtwait, dtsec, kbi,
                                  altpad, 'y', out v1t2, out v2t2, out hitearth, out errorsum, out errorout);
                    AstroLibr.lambertuniv(r1, r3, v1, 'L', 'L', nrev, dtwait, dtsec, kbi,
                                  altpad, 'y', out v1t3, out v2t3, out hitearth, out errorsum, out errorout);
                    AstroLibr.lambertuniv(r1, r3, v1, 'L', 'H', nrev, dtwait, dtsec, kbi,
                                  altpad, 'y', out v1t4, out v2t4, out hitearth, out errorsum, out errorout);
                    if (errorout.Contains("ok"))
                    {
                        MathTimeLibr.addvec(1.0, v1t1, -1.0, v1, out dv11);
                        MathTimeLibr.addvec(1.0, v2t1, -1.0, v3, out dv21);
                        MathTimeLibr.addvec(1.0, v1t2, -1.0, v1, out dv12);
                        MathTimeLibr.addvec(1.0, v2t2, -1.0, v3, out dv22);
                        MathTimeLibr.addvec(1.0, v1t3, -1.0, v1, out dv13);
                        MathTimeLibr.addvec(1.0, v2t3, -1.0, v3, out dv23);
                        MathTimeLibr.addvec(1.0, v1t4, -1.0, v1, out dv14);
                        MathTimeLibr.addvec(1.0, v2t4, -1.0, v3, out dv24);
                        strbuild.AppendLine(dtwait.ToString() + " " + dtsec.ToString() +
                            "  " + MathTimeLibr.mag(dv11).ToString() + "  " + MathTimeLibr.mag(dv21).ToString() +
                            "  " + MathTimeLibr.mag(dv12).ToString() + "  " + MathTimeLibr.mag(dv22).ToString() +
                            "  " + MathTimeLibr.mag(dv13).ToString() + "  " + MathTimeLibr.mag(dv23).ToString() +
                            "  " + MathTimeLibr.mag(dv14).ToString() + "  " + MathTimeLibr.mag(dv24).ToString());
                    }
                    else
                        strbuild.AppendLine(errorsum + " " + errorout);
                }
            }


            // fig 7-21
            StringBuilder strbuildFig = new StringBuilder();
            r1 = new double[] { -6175.1034, 2757.0706, 1626.6556 };
            v1 = new double[] { 2.376641, 1.139677, 7.078097 };
            r2 = new double[] { -6078.007289, 2796.641859, 1890.7135 };
            v2 = new double[] { 2.654700, 1.018600, 7.015400 };

            strbuildFig.AppendLine("dtwait  dtsec       dv1       dv2 ");
            this.opsStatus.Text = "Status:  on case 80c";
            Refresh();
            int totaldts = 15000;
            int totaldtw = 30000;
            int step1 = 60;   // 60 orig
            int step2 = 600;  // 600 orig
            int stop1 = (int) (totaldts/step1);
            int stop2 = (int) (totaldtw/step2);
            for (i = 0; i < stop1; i++)  // orig 250, 15000 s total 
            {
                dtsec = i * step1;    // orig 60
                AstroLibr.kepler(r1, v1, dtsec, out r4, out v4);
                for (j = 0; j < stop2; j++)  // orig 25 600*25 = 15000 s total
                {
                    dtwait = j * step2;   // orig 600
                    AstroLibr.kepler(r2, v2, dtsec + dtwait, out r3, out v3);

                    AstroLibr.lambertuniv(r4, r3, v4, 's', 'd', nrev, dtwait, dtsec, kbi,
                                  altpad, 'y', out v1t1, out v2t1, out hitearth, out errorsum, out errorout);
                    AstroLibr.lambertuniv(r4, r3, v4, 's', 'r', nrev, dtwait, dtsec, kbi,
                                  altpad, 'y', out v1t2, out v2t2, out hitearth, out errorsum, out errorout);
                    AstroLibr.lambertuniv(r4, r3, v4, 'l', 'd', nrev, dtwait, dtsec, kbi,
                                  altpad, 'y', out v1t3, out v2t3, out hitearth, out errorsum, out errorout);
                    AstroLibr.lambertuniv(r4, r3, v4, 'l', 'r', nrev, dtwait, dtsec, kbi,
                                  altpad, 'y', out v1t4, out v2t4, out hitearth, out errorsum, out errorout);
                    if (errorout.Contains("ok"))
                    {
                        MathTimeLibr.addvec(1.0, v1t1, -1.0, v4, out dv11);
                        MathTimeLibr.addvec(1.0, v2t1, -1.0, v3, out dv21);
                        MathTimeLibr.addvec(1.0, v1t2, -1.0, v4, out dv12);
                        MathTimeLibr.addvec(1.0, v2t2, -1.0, v3, out dv22);
                        MathTimeLibr.addvec(1.0, v1t3, -1.0, v4, out dv13);
                        MathTimeLibr.addvec(1.0, v2t3, -1.0, v3, out dv23);
                        MathTimeLibr.addvec(1.0, v1t4, -1.0, v4, out dv14);
                        MathTimeLibr.addvec(1.0, v2t4, -1.0, v3, out dv24);
                        strbuildFig.AppendLine(dtwait.ToString() + " " + dtsec.ToString() +
                            "  " + MathTimeLibr.mag(dv11).ToString() + "  " + MathTimeLibr.mag(dv21).ToString() +
                            "  " + MathTimeLibr.mag(dv12).ToString() + "  " + MathTimeLibr.mag(dv22).ToString() +
                            "  " + MathTimeLibr.mag(dv13).ToString() + "  " + MathTimeLibr.mag(dv23).ToString() +
                            "  " + MathTimeLibr.mag(dv14).ToString() + "  " + MathTimeLibr.mag(dv24).ToString());
                    }
                    else
                        strbuildFig.AppendLine(errorsum + " " + errorout);
                }
            }

            // write data out
            string directory = @"D:\Codes\LIBRARY\Matlab\";
            File.WriteAllText(directory + "surfMovingSalltest.out", strbuildFig.ToString());
        }

        // test all the known problem cases for lambert
        // output these results separately to the testall directory
        private void testknowncases()
        {
            //  this file runs all the known problem cases. 
            double tusec = 806.8111238242922;
            Int16 numiter = 16;
            Int32 caseopt, nrev;
            double dtwait, dtsec;
            double[] r1 = new double[3];
            double[] r2 = new double[3];
            double[] v1 = new double[3];
            double[] v2 = new double[3];
            double[] v1tk = new double[3];
            double[] v2tk = new double[3];
            double[] v1tu = new double[3];
            double[] v2tu = new double[3];
            double[] v1tb = new double[3];
            double[] v2tb = new double[3];
            double[] v1tt = new double[3];
            double[] v2tt = new double[3];
            string detailSum, detailAll, errorout;
            double[] dv1 = new double[3];
            double[] dv2 = new double[3];
            double[] dv1t = new double[3];
            double[] dv2t = new double[3];
            double[] r3h = new double[3];
            double[] v3h = new double[3];
            double[] dr = new double[3];
            double magv1t, magv2t, ang, f, g, gdot, s, tau;
            double tmin, tminp, tminenergy;
            StringBuilder strbuild = new StringBuilder();
            detailSum = "";
            detailAll = "";
            int i;
            char dm, de, hitearth;
            // for test180, show = n, show180 = y
            // for testlamb, show = y, show180 = n known cases
            // for envelope, show = n, show180 = n 

            double altpadc = 200.0 / AstroLibr.gravConst.re;  // set 200 km for altitude you set as the over limit. 

            dtsec = 0.0;
            nrev = 0;
            for (caseopt = 0; caseopt <= 79; caseopt++) // 74
            {
                this.opsStatus.Text = "working on lambert case " + caseopt.ToString();
                Refresh();

                dtwait = 0.0;
                // Problem cases during evaluation
                switch (caseopt)
                {
                    case 0:
                        strbuild.AppendLine("\n-------- lambert test book pg 497 short way \n");
                        nrev = 0;
                        r1 = new double[] { 2.500000 * AstroLibr.gravConst.re, 0.000000, 0.000000 };
                        r2 = new double[] { 1.9151111 * AstroLibr.gravConst.re, 1.6069690 * AstroLibr.gravConst.re, 0.000000 };
                        // assume circular initial orbit for vel calcs
                        v1 = new double[] { 0.0, Math.Sqrt(AstroLibr.gravConst.mu / r1[0]), 0.0 };
                        ang = Math.Atan(r2[1] / r2[0]);
                        v2 = new double[] { -Math.Sqrt(AstroLibr.gravConst.mu / r2[1]) * Math.Cos(ang), Math.Sqrt(AstroLibr.gravConst.mu / r2[0]) * Math.Sin(ang), 0.0 };
                        dtsec = 99900.3;
                        dtsec = 76.0 * 60.0;
                        dtsec = 21000.0;

                        strbuild.AppendLine("r1 " + " " + r1[0].ToString("0.00000000000") + " " + r1[1].ToString("0.00000000000") + " " + r1[2].ToString("0.00000000000"));
                        strbuild.AppendLine("r2 " + " " + r2[0].ToString("0.00000000000") + " " + r2[1].ToString("0.00000000000") + " " + r2[2].ToString("0.00000000000"));
                        strbuild.AppendLine("v1 " + " " + v1[0].ToString("0.00000000000") + " " + v1[1].ToString("0.00000000000") + " " + v1[2].ToString("0.00000000000"));
                        strbuild.AppendLine("v2 " + " " + v2[0].ToString("0.00000000000") + " " + v2[1].ToString("0.00000000000") + " " + v2[2].ToString("0.00000000000"));
                        break;
                    case 1:
                        nrev = 1;
                        r2 = new double[] { -1105.78023519582, 2373.16130661458, 6713.89444816503 };
                        v2 = new double[] { 5.4720951867079, -4.39299050886976, 2.45681739563752 };
                        r1 = new double[] { 4938.49830042171, -1922.24810472241, 4384.68293292613 };
                        v1 = new double[] { 0.738204644165659, 7.20989453238397, 2.32877392066299 };
                        dtsec = 6372.69272563561; // 1ld
                        break;
                    case 2:
                        // cases where it keeps going into -sqrt2
                        nrev = 1;
                        r2 = new double[] { 5389.93493556163, -4804.52588241862, 25.1582783497964 };
                        v2 = new double[] { -0.727069514249192, -0.859982846655316, -7.342663871411 };
                        r1 = new double[] { -1522.75287570047, -6087.9994451461, -2815.10297689319 };
                        v1 = new double[] { 5.24505988961516, -3.34468129213862, 4.39250899043762 };
                        dtsec = 10221.5467480492; // 1ld
                        break;
                    case 3:
                        // cases where it keeps going into -sqrt2
                        nrev = 1;
                        r2 = new double[] { 1116.1162667853, -2378.25911910064, -6722.28853781123 };
                        v2 = new double[] { -5.47209340870903, 4.37730656506257, -2.45025316846857 };
                        r1 = new double[] { 1554.48430223507, -6700.43642118198, 57.4227160854669 };
                        v1 = new double[] { 5.23346411132679, 1.25993062455167, 5.38814398663452 };
                        dtsec = 10852.5064238547; // 1ld
                        break;
                    case 4:
                        // this case suggested chaging the order in mutli rev p long
                        nrev = 1;
                        r2 = new double[] { -4822.158616898, 3455.91484677543, -4098.68288033907 };
                        v2 = new double[] { -2.57576029510129, 3.53163203241951, 6.01488885299844 };
                        r1 = new double[] { -4669.11761281366, 3212.49596830797, -3898.12963175177 };
                        v1 = new double[] { -1.87628239850534, -6.63882509070526, -3.21890799080453 };
                        dtsec = 2776.22257354422; // 1ld                   
                        break;
                    case 5:
                        // this one requires an additional check, not needeed now?
                        nrev = 1;
                        r2 = new double[] { -2641.12487002351, 3531.78400296964, 5698.93227237491 };
                        v2 = new double[] { 4.88892723175287, -3.4599464651134, 4.40951762424652 };
                        r1 = new double[] { 4938.49830042171, -1922.24810472241, 4384.68293292613 };
                        v1 = new double[] { 0.738204644165659, 7.20989453238397, 2.32877392066299 };
                        dtsec = 12177.5217430462; // 1ld
                        break;
                    case 6:
                        // set to 0.001 with case E4
                        nrev = 0;
                        r2 = new double[] { -5057.4585090362, 3809.11827469679, -3450.0477818739 };
                        r1 = new double[] { -4407.00934446058, 4033.85497867959, -3409.86172213079 };
                        v2 = new double[] { -2.02465066311744, 3.13212960292945, 6.4339466703018 };
                        v1 = new double[] { -2.56559926580898, -6.05303910025674, -3.83938365285278 };
                        dtsec = 8265.57175305213; // sr0
                        break;
                    case 7:
                        //  add check M3plong
                        nrev = 1;
                        r2 = new double[] { -1331.61712455282, -269.887149406199, -7085.53489160526 };
                        v2 = new double[] { -5.42091526234769, 5.00552178527806, 0.834085025435344 };
                        r1 = new double[] { -1885.20749244741, 6606.0376125118, -379.861779252094 };
                        v1 = new double[] { -5.10094853369525, -1.76531996825405, -5.37072372683008 };
                        dtsec = 14070.4007704627; // 1ld
                        break;
                    case 8:
                        // this case showed the need to have real k values...
                        nrev = 1;
                        r2 = new double[] { -758.422432200603, 2091.16846174067, 6854.59658477816 };
                        v2 = new double[] { 5.53448307124667, -4.54239531565604, 2.00157190055228 };
                        r1 = new double[] { 4938.49830042171, -1922.24810472241, 4384.68293292613 };
                        v1 = new double[] { 0.738204644165659, 7.20989453238397, 2.32877392066299 };
                        dtsec = 6435.78869321616; // 1ld
                        break;
                    case 9:
                        // check for cases where k is still less than -sqrt2
                        // note as the nrevs go up, the result moves less negative from -sqrt2
                        nrev = 0;  // slope -14, ans -1.407629, 9 iter with 0.001
                        r2 = new double[] { 4976.15252113442, -3689.5625418661, 3702.49326716728 };
                        r1 = new double[] { 4554.0066232623, -3634.43274023062, 3655.51495588221 };
                        v2 = new double[] { 2.23862305685032, -3.29331102465481, -6.27247192272167 };
                        v1 = new double[] { 2.22128773227645, 6.35746126414215, 3.55208099082447 };
                        dtsec = 2271.45483289982; // 0sr
                        break;
                    case 10:
                        // another similar to last case, 1/dtdk didn't work for this one
                        nrev = 0;  // slope -3100
                        r2 = new double[] { -4919.10396163648, 3603.57554611554, -3848.40171187302 };
                        v2 = new double[] { -2.35929722680958, 3.38077576872956, 6.18851117355277 };
                        r1 = new double[] { -4558.02316256308, 3642.41900628984, -3643.58590236029 };
                        v1 = new double[] { -2.21927554566458, -6.34957798602613, -3.56614347300579 };
                        dtsec = 8139.37981789103; // 0sr
                        break;
                    case 11:
                        // cases where it keeps going into -sqrt2
                        nrev = 1;
                        r2 = new double[] { -4822.15861689805, 3455.9148467755, -4098.68288033895 };
                        v2 = new double[] { -2.57576029510118, 3.53163203241943, 6.01488885299853 };
                        r1 = new double[] { -4677.91372255611, 3222.96969164161, -3878.88763811588 };
                        v1 = new double[] { -1.86920520521555, -6.62702216153142, -3.24731757926936 };
                        dtsec = 8454.8596557937; // 1sr
                        break;
                    case 12:
                        // if within the tol of sqrt2, use bissection to avoid overshooting
                        nrev = 1;
                        r2 = new double[] { 2446.18922274494, -797.812934707937, 6735.54747599021 };
                        v2 = new double[] { 4.99682827935479, -4.95260674643056, -2.39173249168695 };
                        r1 = new double[] { 1846.58055111604, 5858.69785273178, 3097.77066306967 };
                        v1 = new double[] { -5.11856419928947, 3.81079538113064, -4.15161560857017 };
                        dtsec = 11925.1378727242; // 1ld           
                        break;
                    case 13:
                        // case requires imaginary part of xs too M3Plong
                        nrev = 1;
                        r2 = new double[] { -3510.55150355902, 4115.53040858099, 4761.12955817936 };
                        v2 = new double[] { 4.26806116234577, -2.68813916207364, 5.46786810528871 };
                        r1 = new double[] { -3597.14246007294, 5385.47909322872, -2320.9741205119 };
                        v1 = new double[] { -3.81051604568839, -4.58797507705978, -4.73322184068286 };
                        dtsec = 9716.77900740478; // 1ld
                        break;
                    case 14:
                        nrev = 0;
                        r2 = new double[] { -2223.24342336674, 3241.02235559901, 6039.79724630885 };
                        v2 = new double[] { 5.09225965957541, -3.7673082496703, 3.89651751594842 };
                        r1 = new double[] { 4938.49830042171, -1922.24810472241, 4384.68293292613 };
                        v1 = new double[] { 0.738204644165659, 7.20989453238397, 2.32877392066299 };
                        dtsec = 63.095967580550479; // 0ld
                        break;
                    case 15:
                        nrev = 1;
                        r2 = new double[] { -4974.3696377813, 3671.28381637024, -3710.74798832271 };
                        v2 = new double[] { -2.24725132040761, 3.29263408780505, 6.2774291914344 };
                        r1 = new double[] { -4407.00934446058, 4033.85497867959, -3409.86172213079 };
                        v1 = new double[] { -2.56559926580898, -6.05303910025674, -3.83938365285278 };
                        dtsec = 14322.7846407849; // 0ld
                        break;
                    case 16:// hyperbolic
                        nrev = 0;
                        r2 = new double[] { 2955.13131737849, -3763.49652390861, -5406.10786702601 };
                        v2 = new double[] { -4.68036851378556, 3.21003519509918, -4.78619261107496 };
                        r1 = new double[] { -1559.02303183983, 6701.22258760379, -40.3381891429746 };
                        v1 = new double[] { -5.23416635407774, -1.25061571408999, -5.38702212157039 };
                        dtsec = 1135.72741644991; // 0ld
                        break;
                    case 17:
                        nrev = 1;
                        r2 = new double[] { -4974.36963778144, 3671.28381637045, -3710.74798832232 };
                        v2 = new double[] { -2.24725132040728, 3.29263408780481, 6.27742919143465 };
                        r1 = new double[] { -4677.91372255611, 3222.96969164161, -3878.88763811588 };
                        v1 = new double[] { -1.86920520521555, -6.62702216153142, -3.24731757926936 };
                        dtsec = 8517.95562337433; // 0ld
                        break;
                    case 18:
                        nrev = 1;
                        r2 = new double[] { -21337.5237890659, -36365.1683964209, 2.43003186524166 };
                        v2 = new double[] { 2.65198086085399, -1.55608104392457, -2.28006813941804E-05 };
                        r1 = new double[] { -20702.6892300265, -36730.2874492455, 3.07140781569511 };
                        v1 = new double[] { 2.67862420772792, -1.50975376683806, -0.000107174977829257 };
                        dtsec = 83292.715003933; // 1ld
                        break;
                    case 19:
                        nrev = 2;
                        r2 = new double[] { -21335.6734907851, -36366.3155269607, 1.8168603510595 };
                        v2 = new double[] { 2.65205952295591, -1.55594317639927, -7.58319122309682E-06 };
                        r1 = new double[] { -20702.6892300265, -36730.2874492455, 3.07140781569511 };
                        v1 = new double[] { 2.67862420772792, -1.50975376683806, -0.000107174977829257 };
                        dtsec = 169457.592594209; // 2ld
                        break;
                    case 20:   // case requries 16-17 iterations...
                        nrev = 0;
                        r2 = new double[] { -21334.4279584025, -36367.1076344499, 1.43164460418315 };
                        v2 = new double[] { 2.65211231086955, -1.55584941166386, 5.96175008776875E-07 };
                        r1 = new double[] { -20702.6892300265, -36730.2874492455, 3.07140781569511 };
                        v1 = new double[] { 2.67862420772792, -1.50975376683806, -0.000107174977829257 };
                        dtsec = 255622.470184485; // 0sr
                        break;
                    case 21:
                        nrev = 3;
                        r2 = new double[] { -21334.4279584025, -36367.1076344499, 1.43164460418315 };
                        v2 = new double[] { 2.65211231086955, -1.55584941166386, 5.96175008776875E-07 };
                        r1 = new double[] { -20702.6892300265, -36730.2874492455, 3.07140781569511 };
                        v1 = new double[] { 2.67862420772792, -1.50975376683806, -0.000107174977829257 };
                        dtsec = 255622.470184485; // 3ld
                        break;
                    case 22:  // shows need for a little margin on the tbir values
                        nrev = 1;
                        r2 = new double[] { -10000.0, 3750.0, 0.0 };
                        v2 = new double[] { 2.65211231086955, -1.55584941166386, 5.96175008776875E-07 };
                        r1 = new double[] { 7278.1363, 0.0, 0.0 };
                        v1 = new double[] { 2.67862420772792, -1.50975376683806, -0.000107174977829257 };
                        dtsec = 86400.0; // 1ld
                        break;
                    case 23:
                        nrev = 0;
                        r1 = new double[] { -1467.0216503866718, 1586.1576668688224, 6812.892902302875 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { 0.0, 6021.0229606764833, -3721.196836741588 };
                        v2 = new double[] { 2.67862420772792, -1.50975376683806, -0.000107174977829257 };
                        dtsec = 60.0; // 0ld
                        break;
                    case 24:  // shows need for complex
                        nrev = 0;
                        r1 = new double[] { -1467.0216503866718, 1586.1576668688224, 6812.892902302875 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { -1674.818065, 1349.399286, 6496.355133 };
                        v2 = new double[] { 2.67862420772792, -1.50975376683806, -0.000107174977829257 };
                        dtsec = 60.0; // 0sd
                        break;
                    case 25:
                        nrev = 1;
                        r1 = new double[] { -1467.0216503866718, 1586.1576668688224, 6812.892902302875 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { -1271.900616, 2703.476434, 6306.177922 };
                        v2 = new double[] { 2.65211231086955, -1.55584941166386, 5.96175008776875E-07 };
                        dtsec = 5000.0; // 1sr
                        break;
                    case 26:
                        nrev = 0;
                        r1 = new double[] { -1467.0216503866718, 1586.1576668688224, 6812.892902302875 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { -1552.07576, 1558.848765, 6832.766801 };
                        v2 = new double[] { 2.65211231086955, -1.55584941166386, 5.96175008776875E-07 };
                        dtsec = 10.0; // 1ld
                        break;
                    case 27:
                        nrev = 1;
                        r1 = new double[] { -1467.0216503866718, 1586.1576668688224, 6812.892902302875 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { -1530.453496, 1537.132145, 6737.578219 };
                        v2 = new double[] { 2.65211231086955, -1.55584941166386, 5.96175008776875E-07 };
                        dtsec = 4000.0; // 1ld
                        break;
                    case 28:
                        nrev = 1;
                        r1 = new double[] { -1467.0216503866718, 1586.1576668688224, 6812.892902302875 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { 233.4851636, 6213.368002, 3167.639301 };
                        v2 = new double[] { 2.65211231086955, -1.55584941166386, 5.96175008776875E-07 };
                        dtsec = 5000.0; // 1ld
                        break;
                    case 29:
                        nrev = 1;
                        r1 = new double[] { -1467.0216503866718, 1586.1576668688224, 6812.892902302875 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { -5252.90710654156, -2309.78595136001, 3970.67397046322 };
                        v2 = new double[] { 2.65211231086955, -1.55584941166386, 5.96175008776875E-07 };
                        dtsec = 5000.0; // 1ld
                        break;
                    case 30:
                        nrev = 1;
                        r1 = new double[] { -1467.0216503866718, 1586.1576668688224, 6812.892902302875 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { -1720.86781033105, 2455.79570291113, 6300.95845373533 };
                        v2 = new double[] { 2.65211231086955, -1.55584941166386, 5.96175008776875E-07 };
                        dtsec = 5000.0; // 1sr
                        break;
                    case 31:   // needs 16 iterations?
                        nrev = 1;
                        r1 = new double[] { -1467.0216503866718, 1586.1576668688224, 6812.892902302875 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { -1501.58876366474, 1598.6698362693, 6729.74808816849 };
                        v2 = new double[] { 2.65211231086955, -1.55584941166386, 5.96175008776875E-07 };
                        dtsec = 4000.0; // 1ld
                        break;
                    case 32:
                        nrev = 1;
                        r1 = new double[] { -1467.0216503866718, 1586.1576668688224, 6812.892902302875 };
                        v1 = new double[] { 2.65211231086955, -1.55584941166386, 5.96175008776875E-07 };
                        r2 = new double[] { -1596.16447011803, 2572.48740761792, 6287.20633610422 };
                        v2 = new double[] { 2.65211231086955, -1.55584941166386, 5.96175008776875E-07 };
                        dtsec = 5000.0; // 1sr
                        break;
                    case 33:
                        nrev = 1;
                        r1 = new double[] { -1467.0216503866718, 1586.1576668688224, 6812.892902302875 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { -1530.76975907306, 2578.18070794673, 6301.12086637351 };
                        v2 = new double[] { 2.65211231086955, -1.55584941166386, 5.96175008776875E-07 };
                        dtsec = 5000.0; // 1sr
                        break;
                    case 34:
                        nrev = 0;
                        r1 = new double[] { -1467.0216503866718, 1586.1576668688224, 6812.892902302875 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { -1453.48010524337, 1619.93081291069, 6819.24812980238 };
                        v2 = new double[] { 2.65211231086955, -1.55584941166386, 5.96175008776875E-07 };
                        dtsec = 10.0; // 0ld
                        break;
                    case 35:
                        nrev = 0;
                        r1 = new double[] { -1467.0216503866718, 1586.1576668688224, 6812.892902302875 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { -1482.87003576258, 1557.63131212973, 6827.43044367934 };
                        v2 = new double[] { 2.65211231086955, -1.55584941166386, 5.96175008776875E-07 };
                        dtsec = 10.0; // 0sd
                        break;
                    case 36:
                        nrev = 0;
                        r1 = new double[] { -1467.0216503866718, 1586.1576668688224, 6812.892902302875 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { -1608.99706453697, 2084.23508424173, 6656.27657708845 };
                        v2 = new double[] { 2.65211231086955, -1.55584941166386, 5.96175008776875E-07 };
                        dtsec = 2400.0; // 0lr
                        break;
                    case 37:
                        nrev = 0;
                        r1 = new double[] { -1467.0216503866718, 1586.1576668688224, 6812.892902302875 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { -1121.44922682935, 1510.6959195005, 6906.45167804839 };
                        v2 = new double[] { 2.65211231086955, -1.55584941166386, 5.96175008776875E-07 };
                        dtsec = 2400.0; // 0sr
                        break;

                    case 38:
                        nrev = 0;
                        r1 = new double[] { -1467.0216503866718, 1586.1576668688224, 6812.892902302875 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { 1469.25087675358, -1588.56792747551, -6823.24549695535 };
                        v2 = new double[] { 2.65211231086955, -1.55584941166386, 5.96175008776875E-07 };
                        dtsec = 25.0; // 0sr
                        break;
                    case 39:
                        nrev = 2;
                        r1 = new double[] { -1467.0216503866718, 1586.1576668688224, 6812.892902302875 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { 940.2742457112, 2002.55768150576, 6807.6848036512 };
                        v2 = new double[] { 2.65211231086955, -1.55584941166386, 5.96175008776875E-07 };
                        dtsec = 7200.0; // 0lr
                        break;
                    case 40:  // ld2 may be a 0 deg case?
                        nrev = 2;
                        r1 = new double[] { -1467.0216503866718, 1586.1576668688224, 6812.892902302875 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { -1482.87003576258, 1557.63131212973, 6827.43044367934 };
                        v2 = new double[] { 2.65211231086955, -1.55584941166386, 5.96175008776875E-07 };
                        dtsec = 7300.0; // 0sr
                        break;
                    case 41:  // ld2 may be a 180 deg case? 
                        nrev = 2;
                        r1 = new double[] { -1467.0216503866718, 1586.1576668688224, 6812.892902302875 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { 1482.87003576258, -1557.63131212973, -6827.43044367934 };
                        v2 = new double[] { -2.65211231086955, 1.55584941166386, -5.96175008776875E-07 };
                        dtsec = 7300.0; // 0sr
                        break;
                    case 42:  // 180 deg case from JGCD 2013: pg 1925
                        nrev = 0;
                        r1 = new double[] { 5690.21923, 3309.62377, 1311.30504 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { -5691.9147514, -3310.6095425, -1311.695711 };
                        v2 = new double[] { -2.65211231086955, 1.55584941166386, -5.96175008776875E-07 };
                        dtsec = 2750.0; // 0sr
                        break;
                    case 43:  // 
                        nrev = 0;
                        r1 = new double[] { -1467.0216503866718, 1586.1576668688224, 6812.892902302875 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { -1442.6617806489, -3284.98662949686, 5868.25003058415 };
                        v2 = new double[] { 2.65211231086955, -1.55584941166386, 5.96175008776875E-07 };
                        dtsec = 7800.0; // 0ld
                        break;
                    case 44:  //
                        nrev = 0;
                        r1 = new double[] { -1467.0216503866718, 1586.1576668688224, 6812.892902302875 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { -1442.6617806489, -3284.98662949686, 5868.25003058415 };
                        v2 = new double[] { 2.65211231086955, -1.55584941166386, 5.96175008776875E-07 };
                        dtsec = 7800.0; // 0ld
                        break;
                    case 45:  // 
                        nrev = 0;
                        r1 = new double[] { -1467.0216503866718, 1586.1576668688224, 6812.892902302875 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { 1479.51367890978, -1599.66416611606, -6870.90626048191 };
                        v2 = new double[] { 2.65211231086955, -1.55584941166386, 5.96175008776875E-07 };
                        dtsec = 500.0; // 0lr
                        break;
                    case 46:  // 
                        nrev = 0;
                        r1 = new double[] { -1467.0216503866718, 1586.1576668688224, 6812.892902302875 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { 138.1064291783988, 1622.8249169416792, 6970.3867334561619 };
                        v2 = new double[] { 2.65211231086955, -1.55584941166386, 5.96175008776875E-07 };
                        dtsec = 6000.0; // 0lr
                        break;
                    case 47:  // 
                        nrev = 0;
                        r1 = new double[] { -1467.0216503866718, 1586.1576668688224, 6812.892902302875 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { 1990.42456035076, -2152.06583753701, -9243.5918421576 };
                        v2 = new double[] { 2.65211231086955, -1.55584941166386, 5.96175008776875E-07 };
                        dtsec = 960.0; // ld0
                        break;
                    case 48:  // 
                        nrev = 0;
                        r1 = new double[] { -1467.0216503866718, 1586.1576668688224, 6812.892902302875 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { 1836.48252800838, -1985.62225792871, -8528.68038925911 };
                        v2 = new double[] { 2.65211231086955, -1.55584941166386, 5.96175008776875E-07 };
                        dtsec = 960.0; // lr0
                        break;
                    case 49:  // 
                        nrev = 0;
                        r1 = new double[] { -1467.0216503866718, 1586.1576668688224, 6812.892902302875 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { 1374.65643098123, -1486.2915191038, -6383.9460305636 };
                        v2 = new double[] { 2.65211231086955, -1.55584941166386, 5.96175008776875E-07 };
                        dtsec = 1080.0; // lr0
                        break;
                    case 50:  // exact same start/stop
                        nrev = 1;
                        r1 = new double[] { -1467.0216503866718, 1586.1576668688224, 6812.892902302875 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { -1467.12165038667, 1586.15766686882, 6812.89290230288 };
                        v2 = new double[] { 2.65211231086955, -1.55584941166386, 5.96175008776875E-07 };
                        dtsec = 1260.0; // sd1
                        break;
                    case 51:  // 
                        nrev = 0;
                        r1 = new double[] { -1467.0216503866718, 1586.1576668688224, 6812.892902302875 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { 1384.91923313739, -1497.38775774435, -6431.60679409017 };
                        v2 = new double[] { 2.65211231086955, -1.55584941166386, 5.96175008776875E-07 };
                        dtsec = 1380.0; // ld0
                        break;
                    case 52:  // GEO explosion case
                        nrev = 0;
                        r1 = new double[] { 482.5243001466, -42162.5367629581, -4.55720848611 };
                        v1 = new double[] { 3.07452652818, 0.03517993278, -0.00157950955 };
                        r2 = new double[] { -345.20067445796, 30163.3225995325, 3.26025330242 };
                        v2 = new double[] { 2.65211231086955, -1.55584941166386, 5.96175008776875E-07 };
                        dtsec = 5000.0; // lr0
                        break;
                    case 53:  // 
                        nrev = 0;
                        r1 = new double[] { -1467.02165038667, 1586.15766686882, 6812.89290230288 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { 4125.08740883182, -4460.08347477213, -19157.0306556835 };
                        v2 = new double[] { 2.65211231086955, -1.55584941166386, 5.96175008776875E-07 };
                        dtsec = 8340.0; // ld0
                        break;
                    case 54:  // 
                        nrev = 0; r1 = new double[] { -1467.02165038667, 1586.15766686882, 6812.89290230288 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { 1467.02165038667, -1586.15766686882, -6812.89290230288 };
                        v2 = new double[] { 6.99554331377, 2.45471059071, 0.93275076625 };
                        dtsec = 8340.0; // ld0
                        break;
                    case 55: // 360 deg
                        nrev = 0;
                        r1 = new double[] { -1467.02165038667, 1586.15766686882, 6812.89290230288 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { -1467.02165038667, 1586.15766686882, 6812.89290230288 };
                        v2 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        dtsec = 480.0; // ld0
                        break;
                    case 56: // blair gnc paper tests case 1/2 
                        nrev = 0;
                        r1 = new double[] { 3189.06815, 3826.88178, 4464.69541 };
                        v1 = new double[] { -2.8590, 6.0850, -4.0020 };
                        r2 = new double[] { -0.0110, AstroLibr.gravConst.re, -0.0160 };
                        v2 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        dtsec = 780.0; // ld0
                        break;
                    case 57: // blair gnc paper tests case 3/4
                        nrev = 3;
                        r1 = new double[] { 3189.06815, 3826.88178, 4464.69541 };
                        v1 = new double[] { -2.8590, 6.0850, -4.0020 };
                        r2 = new double[] { -0.0110, AstroLibr.gravConst.re, -0.0160 };
                        v2 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        dtsec = 12600.0; // ld0
                        break;
                    case 58: // 180 deg blair gnc paper case 5
                        nrev = 0;
                        r1 = new double[] { -21027.028559886, -38422.464945077, -21040.739334720 };
                        v1 = new double[] { 3.325465893, 2.974269616, 1.543325496 };
                        r2 = new double[] { 3608.25094727094, 6593.31839594208, 3610.60369026747 };
                        v2 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        dtsec = 9091.575023; // ld0
                        break;
                    case 59: // 180 deg blair gnc paper case 6
                        nrev = 8;
                        r1 = new double[] { 6495.311253785, 1581.249033056, 582.620453154 };
                        v1 = new double[] { -1.915647898, 6.463641713, 3.813978950 };
                        r2 = new double[] { -6724.292992018, -1636.993312801, -603.159736589 };
                        v2 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        dtsec = 47761.161352; // ld0
                        break;
                    case 60: // 180 deg blair gnc paper case 7
                        nrev = 6;
                        r1 = new double[] { 45285.8020, -88813.9820, 2399.1110 };
                        v1 = new double[] { 0.8882, 1.3803, -0.3455 };
                        r2 = new double[] { -63464.2660, -67537.1770, 20780.2140 };
                        v2 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        dtsec = 1380794.0; // ld0
                        break;
                    case 61: //  ted new combo
                        nrev = 0;
                        r1 = new double[] { 4542.91131514247, 1821.67923522038, 4832.74476981704 };
                        v1 = new double[] { -2.26483797514683, 7.24188730176539, -.599013408289993 };
                        r2 = new double[] { 4761.27919527887, -3399.35772120015, 4224.48134696029 };
                        v2 = new double[] { 2.67662728229248, -3.61820668585223, -5.91158244795794 };
                        dtsec = 1009.2273957218166; // ld0
                        break;
                    case 62: //  ted hit earth
                        nrev = 0;
                        r1 = new double[] { 20291.6011015625, 8524.842359375, -2.20945309966803 };
                        v1 = new double[] { -.622422892570496, 1.48184535074234, -0.00000870507268700749 };
                        r2 = new double[] { 57349.23609375, -19192.008359375, -3.79552479560673 };
                        v2 = new double[] { 1.39519703006744, 4.18299925518036, -0.00022377042460721 };  //xx
                        dtsec = 5744.5489431519109; // ld0
                        break;
                    case 63: //  ted notch case
                        nrev = 0;
                        r1 = new double[] { 1213.240569289341, -6764.54813179293, -289.971597590799 };
                        v1 = new double[] { 5.342982219845, 0.727123585546384, 5.37891609348336 };
                        r2 = new double[] { 20517.0512490881, 7556.0384007962, 29118.4938038349 };
                        v2 = new double[] { 1395.19703006744, 4182.99925518036, -0.22377042460721 };  //xx
                        dtsec = 5613.8273887025871; // ld0
                        break;
                    case 64: //  ted notch case
                        nrev = 0;
                        r1 = new double[] { 1213.240569289341, -6764.54813179293, -289.971597590799 };
                        v1 = new double[] { 5.342982219845, 0.727123585546384, 5.37891609348336 };
                        r2 = new double[] { 20557.2340311722, 7467.6271535836, 2.14318960051298 };
                        v2 = new double[] { -0.631836782875836, 1.40386453042887, 2.14318960051298 };
                        dtsec = 5550.7506764699738; // ld0
                        break;
                    case 65: // sd0 dan dread exact same start/stop
                        nrev = 0;
                        r1 = new double[] { -1467.02165038667, 1586.15766686882, 6812.89290230288 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { -1467.12165038667, 1586.15766686882, 6812.89290230288 };
                        v2 = new double[] { -0.631836782875836, 1.40386453042887, 2.14318960051298 };
                        dtsec = 225;
                        break;
                    case 66: // ld1 dan dread exact same start/stop
                        nrev = 1;
                        r1 = new double[] { -1467.02165038667, 1586.15766686882, 6812.89290230288 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { -1467.12165038667, 1586.15766686882, 6812.89290230288 };
                        v2 = new double[] { -0.631836782875836, 1.40386453042887, 2.14318960051298 };
                        dtsec = 4000;
                        break;
                    case 67: // lr1 dan dread
                        nrev = 1;
                        r1 = new double[] { -1467.02165038667, 1586.15766686882, 6812.89290230288 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { 1876.72223784062, -1540.28396484846, -6615.85535337256 };
                        v2 = new double[] { -0.631836782875836, 1.40386453042887, 2.14318960051298 };
                        dtsec = 8675;
                        break;
                    case 68: // lr1 dan dread
                        nrev = 1;
                        r1 = new double[] { -1467.02165038667, 1586.15766686882, 6812.89290230288 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { 2010.78554376248, -1708.25120993044, -7337.31128158262 };
                        v2 = new double[] { -0.631836782875836, 1.40386453042887, 2.14318960051298 };
                        dtsec = 9400;
                        break;
                    case 69: // 0sd ted latest
                        nrev = 0;
                        r1 = new double[] { 38552.6973353558, 17070.7437708141, 73.9065325070225 };
                        v1 = new double[] { -1.24482521753443, 2.81155032626659, -0.002979028376179 };
                        r2 = new double[] { 39676.8516951764, 14293.6391122781, 11.439978324149 };
                        v2 = new double[] { -1.0427035181671, 2.89206500777264, -0.00086707753542956 };  //xx
                        dtsec = 2872.31657022148;
                        break;
                    case 70: // 0sd ted latest
                        nrev = 0;
                        r1 = new double[] { -15719.4372207424, 39127.4313653065, -38.2565972410531 };
                        v1 = new double[] { -2.85296391088266, -1.14595651345875, -0.00549014511352564 };
                        r2 = new double[] { -12901.5977809522, 40136.901044369, -10.1027040757486 };
                        v2 = new double[] { -2.92766954080673, -0.941319460393697, -0.00099716931263049 };
                        dtsec = 2872.31657022148;
                        break;
                    case 71: // ld1 dan dread w new < 0.0, not <-1.0, and NAN for knew
                        nrev = 1;
                        r1 = new double[] { -1467.02165038667, 1586.15766686882, 6812.89290230288 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { -1467.02165038667, 1586.15766686882, 6812.89290230288 };
                        v2 = new double[] { -0.631836782875836, 1.40386453042887, 2.14318960051298 };  // xx
                        dtsec = 6325.0;
                        break;
                    case 72: // ld0 dan dread w new < 0.0, not <-1.0
                        nrev = 0;
                        r1 = new double[] { -1467.02165038667, 1586.15766686882, 6812.89290230288 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { -1467.02165038667, 1586.15766686882, 6812.89290230288 };
                        v2 = new double[] { -0.631836782875836, 1.40386453042887, 2.14318960051298 };  // xx
                        dtsec = 50.0;
                        break;
                    case 73: // ld2 dan dread w new < 0.0, not <-1.0
                        nrev = 2;
                        r1 = new double[] { -1467.02165038667, 1586.15766686882, 6812.89290230288 };
                        v1 = new double[] { -6.99554331377, -2.45471059071, -0.93275076625 };
                        r2 = new double[] { 3643.54668127958, 1361.21401630224, 5846.71089380907 };
                        v2 = new double[] { -0.631836782875836, 1.40386453042887, 2.14318960051298 };  // xx
                        dtsec = 8050.0;
                        break;
                    case 74: // ld2 dan dread w new < 0.0, not <-1.0
                        nrev = 2;
                        r1 = new double[] { -5383.180477, -889.26752, 4749.772766 };
                        v1 = new double[] { -4.952471, 0.680931, -5.484262 };
                        r2 = new double[] { -1577.53668163127, -1674.29862046126, 8942.79596495134 };
                        v2 = new double[] { -0.631836782875836, 1.40386453042887, 2.14318960051298 };  // xx
                        dtsec = 9840.0;
                        break;
                    case 75: // Nathan Zero Rev(2 degree transfer, short path)-
                        nrev = 0;
                        r1 = new double[] { -2447.5108649997796, 6425.7721331143939, 2598.9244187603417 };
                        v1 = new double[] { -7.0417929290503141, -2.8681303717265290, 1.2224374606557487 };
                        r2 = new double[] {-2681.1652972600514, 6326.4598751843578, 2638.2723306223224 };
                        v2 = new double[] { -6.9560477233116353, -3.0813393768501216, 1.1348757995584626};  // xx
                        dtsec = 33.381379026791095;
                        break;
                    case 76: // Nathan Zero Rev(2 degree transfer, short path)-
                        nrev = 0;
                        r1 = new double[] { -2447.5108649997796, 6425.7721331143939, 2598.9244187603417 };
                        v1 = new double[] { -7.0417929290503141, -2.8681303717265290, 1.2224374606557487 };
                        r2 = new double[] { -2681.1652972600514, 6326.4598751843578, 2638.2723306223224 };
                        v2 = new double[] { -6.9560477233116353, -3.0813393768501216, 1.1348757995584626 };  // xx
                        dtsec = 133.381379026791095;
                        break;
                    case 77: // Nathan Multi Rev (1 Rev, 173.5 degree transfer after rev)
                        nrev = 1;
                        r1 = new double[] { -2447.5108649997796, 6425.7721331143939, 2598.9244187603417 };
                        v1 = new double[] { -7.0417929290503141, -2.8681303717265290, 1.2224374606557487 };
                        r2 = new double[] { 2030.5293385373081, -8121.6045151470798, -2973.4911622306390};
                        v2 = new double[] { 5.9109725501309471, 2.0990367313315055, -1.1164901518784618 };  // xx
                        dtsec = 10925.110937168842;
                        break;
                    case 78: // Nathan Multi Rev (1 Rev, converges hyperbolic/parabolic?)
                        nrev = 1;
                        r1 = new double[] { 7117.5156243154161, -4257.1942597683246, -583.88210887807986 };
                        v1 = new double[] { -7.0417929290503141, -2.8681303717265290, 1.2224374606557487 };
                        r2 = new double[] { 7172.3074180808417, -4123.2685183007470, -560.47505356742181 };
                        v2 = new double[] { 5.9109725501309471, 2.0990367313315055, -1.1164901518784618 };  // xx
                        dtsec = 9864.8956080266707;
                        break;
                    case 79: // Nathan Multi Rev (1 Rev, converges hyperbolic/parabolic?)
                        nrev = 1;
                        r1 = new double[] { 15945.3425000,      0.0000000,      0.0000000 };
                        v1 = new double[] { -7.0417929290503141, -2.8681303717265290, 1.2224374606557487 };
                        r2 = new double[] { 12214.8409660,  10249.4684368,      0.0000000 };
                        v2 = new double[] { 5.9109725501309471, 2.0990367313315055, -1.1164901518784618 };  // xx
                        dtsec = 29573.972651;
                        break;
                          
                }  // switch

                //      dtsec0 = 300.0;
                //      r1 = new double[] { 6.6100000    0.000000    0.000000]*AstroLibr.gravConst.re;
                //      r2  = [ 32328.6   27126.9   0.000000];
                strbuild.AppendLine("===== Lambert Case " + caseopt.ToString() + " === ");

                // assume circular initial orbit for vel calcs
                //    v1 = new double[] {0.0; sqrt(mu/r1(1)); 0.0];
                ang = Math.Atan(r2[1] / r2[0]);
                //    v2 = new double[] {-sqrt(mu/r2(2))*cos(ang);sqrt(mu/r2(1))*sin(ang);0.0];

                double magr1 = MathTimeLibr.mag(r1);
                double magr2 = MathTimeLibr.mag(r2);

                // this value stays constant in all calcs, vara changes with df
                double cosdeltanu = MathTimeLibr.dot(r1, r2) / (magr1 * magr2);

                //strbuild.AppendLine("now do findtbi calcs");
                //strbuild.AppendLine("iter       y         dtnew          psiold      psinew   psinew-psiold   dtdpsi      dtdpsi2    lower    upper     ");

                AstroLambertkLibr.lambertkmins1st(r1, r2, out s, out tau);
                strbuild.AppendLine(" s " + s.ToString(fmt) + " tau " + tau.ToString(fmt) );

                double kbi, tof;
                double[,] tbidk = new double[10, 3];
                AstroLambertkLibr.lambertkmins(s, tau, 1, 'L', out kbi, out tof);
                tbidk[1, 1] = kbi;
                tbidk[1, 2] = tof / tusec;
                AstroLambertkLibr.lambertkmins(s, tau, 2, 'L', out kbi, out tof);
                tbidk[2, 1] = kbi;
                tbidk[2, 2] = tof / tusec;
                AstroLambertkLibr.lambertkmins(s, tau, 3, 'L', out kbi, out tof);
                tbidk[3, 1] = kbi;
                tbidk[3, 2] = tof / tusec;
                AstroLambertkLibr.lambertkmins(s, tau, 4, 'L', out kbi, out tof);
                tbidk[4, 1] = kbi;
                tbidk[4, 2] = tof / tusec;
                AstroLambertkLibr.lambertkmins(s, tau, 5, 'L', out kbi, out tof);
                tbidk[5, 1] = kbi;
                tbidk[5, 2] = tof / tusec;
                AstroLambertkLibr.lambertkmins(s, tau, 6, 'L', out kbi, out tof);
                tbidk[6, 1] = kbi;
                tbidk[6, 2] = tof / tusec;

                double[,] tbirk = new double[10, 3];
                AstroLambertkLibr.lambertkmins(s, tau, 1, 'H', out kbi, out tof);
                tbirk[1, 1] = kbi;
                tbirk[1, 2] = tof / tusec;
                AstroLambertkLibr.lambertkmins(s, tau, 2, 'H', out kbi, out tof);
                tbirk[2, 1] = kbi;
                tbirk[2, 2] = tof / tusec;
                AstroLambertkLibr.lambertkmins(s, tau, 3, 'H', out kbi, out tof);
                tbirk[3, 1] = kbi;
                tbirk[3, 2] = tof / tusec;
                AstroLambertkLibr.lambertkmins(s, tau, 4, 'H', out kbi, out tof);
                tbirk[4, 1] = kbi;
                tbirk[4, 2] = tof / tusec;
                AstroLambertkLibr.lambertkmins(s, tau, 5, 'H', out kbi, out tof);
                tbirk[5, 1] = kbi;
                tbirk[5, 2] = tof / tusec;
                AstroLambertkLibr.lambertkmins(s, tau, 6, 'H', out kbi, out tof);
                tbirk[6, 1] = kbi;
                tbirk[6, 2] = tof / tusec;

                strbuild.AppendLine("From k variables ");
                strbuild.AppendLine(" " + tbidk[1, 1].ToString("#0.00000000000") + "  " + (tbidk[1, 2] * tusec).ToString("0.00000000000") + " s " + (tbidk[1, 2]).ToString("0.00000000000") + " tu ");
                strbuild.AppendLine(" " + tbidk[2, 1].ToString("#0.00000000000") + "  " + (tbidk[2, 2] * tusec).ToString("0.00000000000") + " s " + (tbidk[2, 2]).ToString("0.00000000000") + " tu ");
                strbuild.AppendLine(" " + tbidk[3, 1].ToString("#0.00000000000") + "  " + (tbidk[3, 2] * tusec).ToString("0.00000000000") + " s " + (tbidk[3, 2]).ToString("0.00000000000") + " tu ");
                strbuild.AppendLine(" " + tbidk[4, 1].ToString("#0.00000000000") + "  " + (tbidk[4, 2] * tusec).ToString("0.00000000000") + " s " + (tbidk[4, 2]).ToString("0.00000000000") + " tu ");
                strbuild.AppendLine(" " + tbidk[5, 1].ToString("#0.00000000000") + "  " + (tbidk[5, 2] * tusec).ToString("0.00000000000") + " s " + (tbidk[5, 2]).ToString("0.00000000000") + " tu ");
                strbuild.AppendLine("");
                strbuild.AppendLine(" " + tbirk[1, 1].ToString("#0.00000000000") + "  " + (tbirk[1, 2] * tusec).ToString("0.00000000000") + " s " + (tbirk[1, 2]).ToString("0.00000000000") + " tu ");
                strbuild.AppendLine(" " + tbirk[2, 1].ToString("#0.00000000000") + "  " + (tbirk[2, 2] * tusec).ToString("0.00000000000") + " s " + (tbirk[2, 2]).ToString("0.00000000000") + " tu ");
                strbuild.AppendLine(" " + tbirk[3, 1].ToString("#0.00000000000") + "  " + (tbirk[3, 2] * tusec).ToString("0.00000000000") + " s " + (tbirk[3, 2]).ToString("0.00000000000") + " tu ");
                strbuild.AppendLine(" " + tbirk[4, 1].ToString("#0.00000000000") + "  " + (tbirk[4, 2] * tusec).ToString("0.00000000000") + " s " + (tbirk[4, 2]).ToString("0.00000000000") + " tu ");
                strbuild.AppendLine(" " + tbirk[5, 1].ToString("#0.00000000000") + "  " + (tbirk[5, 2] * tusec).ToString("0.00000000000") + " s " + (tbirk[5, 2]).ToString("0.00000000000") + " tu ");


                //strbuild.AppendLine("lambertTest" + caseopt.ToString() + " " + r1[0].ToString("0.00000000000") + " " + r1[1].ToString("0.00000000000") + " " + r1[2].ToString("0.00000000000") +
                //    " " + v1[0].ToString("0.00000000000") + " " + v1[1].ToString("0.00000000000") + " " + v1[2].ToString("0.00000000000") +
                //    " " + r2[0].ToString("0.00000000000") + " " + r2[1].ToString("0.00000000000") + " " + r2[2].ToString("0.00000000000") +
                //    " " + v2[0].ToString("0.00000000000") + " " + v2[1].ToString("0.00000000000") + " " + v2[2].ToString("0.00000000000") + " " + dtsec.ToString());

                AstroLibr.lambertminT(r1, r2, 'S', 'L', 1, out tmin, out tminp, out tminenergy);
                strbuild.AppendLine("mint S " + tmin.ToString("0.0000") + " minp " + tminp.ToString("0.0000") + " minener " + tminenergy.ToString("0.0000"));
                AstroLibr.lambertminT(r1, r2, 'S', 'L', 2, out tmin, out tminp, out tminenergy);
                strbuild.AppendLine("mint S " + tmin.ToString("0.0000") + " minp " + tminp.ToString("0.0000") + " minener " + tminenergy.ToString("0.0000"));
                AstroLibr.lambertminT(r1, r2, 'S', 'L', 3, out tmin, out tminp, out tminenergy);
                strbuild.AppendLine("mint S " + tmin.ToString("0.0000") + " minp " + tminp.ToString("0.0000") + " minener " + tminenergy.ToString("0.0000"));

                AstroLibr.lambertminT(r1, r2, 'L', 'H', 1, out tmin, out tminp, out tminenergy);
                strbuild.AppendLine("mint L " + tmin.ToString("0.0000") + " minp " + tminp.ToString("0.0000") + " minener " + tminenergy.ToString("0.0000"));
                AstroLibr.lambertminT(r1, r2, 'L', 'H', 2, out tmin, out tminp, out tminenergy);
                strbuild.AppendLine("mint L " + tmin.ToString("0.0000") + " minp " + tminp.ToString("0.0000") + " minener " + tminenergy.ToString("0.0000"));
                AstroLibr.lambertminT(r1, r2, 'L', 'H', 3, out tmin, out tminp, out tminenergy);
                strbuild.AppendLine("mint L " + tmin.ToString("0.0000") + " minp " + tminp.ToString("0.0000") + " minener " + tminenergy.ToString("0.0000"));


                strbuild.AppendLine(" TEST ------------------ s/l  H  0 rev ------------------");
                hitearth = ' ';
                dm = 'S';
                de = 'H';
                AstroLambertkLibr.lambertK(r1, v1, r2, dm, de, nrev, dtwait, dtsec, 0.0, 0.0, numiter, altpadc, 'n', 'n',
                    out v1tk, out v2tk, out f, out g, out gdot, out hitearth, out errorout, out detailSum, out detailAll);
                strbuild.AppendLine(detailAll);
                AstroLibr.kepler(r1, v1tk, dtsec, out r3h, out v3h);
                strbuild.AppendLine("r3h " + " " + r3h[0].ToString("0.00000000000") + " " + r3h[1].ToString("0.00000000000") + " " + r3h[2].ToString("0.00000000000"));
                for (int j = 0; j < 3; j++)
                    dr[j] = r2[j] - r3h[j];
                if (MathTimeLibr.mag(dr) > 0.01)
                    strbuild.AppendLine("velk does not get to r2 position (km) " + MathTimeLibr.mag(dr).ToString() + "\n");

                //strbuild.AppendLine("k#" + caseopt + " " + detailSum + " diffs " + MathTimeLib::mag(dr).ToString("0.00000000000"));
                strbuild.AppendLine("lamk v1t " + " " + v1tk[0].ToString("0.00000000000") + " " + v1tk[1].ToString("0.00000000000") + " " + v1tk[2].ToString("0.00000000000"));
                strbuild.AppendLine("lamk v2t " + " " + v2tk[0].ToString("0.00000000000") + " " + v2tk[1].ToString("0.00000000000") + " " + v2tk[2].ToString("0.00000000000"));
                //strbuild.AppendLine(magv1t.ToString("0.0000000").PadLeft(12) + " " + magv2t.ToString("0.0000000").PadLeft(12));

                AstroLibr.lambertuniv(r1, r2, v1, dm, de, nrev, 0.0, dtsec, 0.0, altpadc * AstroLibr.gravConst.re, 'n', out v1tu, out v2tu, out hitearth, out detailSum, out detailAll);
                //strbuild.AppendLine(detailSum);
                strbuild.AppendLine("univ v1t " + " " + v1tu[0].ToString("0.00000000000") + " " + v1tu[1].ToString("0.00000000000") + " " + v1tu[2].ToString("0.00000000000"));
                strbuild.AppendLine("univ v2t " + " " + v2tu[0].ToString("0.00000000000") + " " + v2tu[1].ToString("0.00000000000") + " " + v2tu[2].ToString("0.00000000000"));
                AstroLibr.kepler(r1, v1tu, dtsec, out r3h, out v3h);
                for (int j = 0; j < 3; j++)
                    dr[j] = r2[j] - r3h[j];
                if (MathTimeLibr.mag(dr) > 0.01)
                    strbuild.AppendLine("velu does not get to r2 position (km) " + MathTimeLibr.mag(dr).ToString() + "\n");

                for (int j = 0; j < 3; j++)
                {
                    dv1[j] = v1tk[j] - v1tu[j];
                    dv2[j] = v2tk[j] - v2tu[j];
                }
                if (MathTimeLibr.mag(dv1) > 0.01 || MathTimeLibr.mag(dv2) > 0.01)
                    strbuild.AppendLine("velk does not match velu \n");

                AstroLibr.lambertbattin(r1, r2, v1, dm, de, nrev, 0.0, dtsec, altpadc * AstroLibr.gravConst.re, 'n', out v1tb, out v2tb, out hitearth, out detailSum, out detailAll);
                //strbuild.AppendLine(detailSum);
                strbuild.AppendLine("batt v1t " + " " + v1tb[0].ToString("0.00000000000") + " " + v1tb[1].ToString("0.00000000000") + " " + v1tb[2].ToString("0.00000000000"));
                strbuild.AppendLine("batt v2t " + " " + v2tb[0].ToString("0.00000000000") + " " + v2tb[1].ToString("0.00000000000") + " " + v2tb[2].ToString("0.00000000000"));
                AstroLibr.kepler(r1, v1tb, dtsec, out r3h, out v3h);
                for (int j = 0; j < 3; j++)
                    dr[j] = r2[j] - r3h[j];
                if (MathTimeLibr.mag(dr) > 0.01)
                    strbuild.AppendLine("velb does not get to r2 position (km) " + MathTimeLibr.mag(dr).ToString() + "\n");
                //strbuild.AppendLine("diffs " + MathTimeLibr.mag(dr).ToString("0.00000000000"));

                for (int j = 0; j < 3; j++)
                {
                    dv1[j] = v1tk[j] - v1tb[j];
                    dv2[j] = v2tk[j] - v2tb[j];
                }
                if (MathTimeLibr.mag(dv1) > 0.01 || MathTimeLibr.mag(dv2) > 0.01)
                    strbuild.AppendLine("velk does not match velb \n");
                //strbuild.AppendLine("diffs " + MathTimeLib::mag(dr).ToString("0.00000000000"));

                //// teds approach
                //double[] r2ted = new double[] { r2[0], r2[1], r2[2] };
                //double[] v2ted = new double[] { v2[0], v2[1], v2[2] };
                //double[] r1ted = new double[] { r1[0], r1[1], r1[2] };
                //double[] v1ted = new double[] { v1[0], v1[1], v1[2] };
                //Cartesian r1com = new Cartesian(r1ted[0], r1ted[1], r1ted[2]);
                //Cartesian v1com = new Cartesian(v1ted[0], v1ted[1], v1ted[2]);
                //Cartesian r2com = new Cartesian(r2ted[0], r2ted[1], r2ted[2]);
                //Cartesian v2com = new Cartesian(v2ted[0], v2ted[1], v2ted[2]);
                //var result = LambertDeltaV.FindMinimumDeltaV(r2com, v2com, r1com, v1com, dtsec, Lambert.EngagementType.Prox, 0);  // .Intercept
                //double[] v1tr = { result.Velocities.Item1.X, result.Velocities.Item1.Y, result.Velocities.Item1.Z };  // LambertKMin/s
                //double[] v2tr = { result.Velocities.Item2.X, result.Velocities.Item2.Y, result.Velocities.Item2.Z };  // LambertKMin/s
                //for (int i = 0; i < 3; i++)
                //{
                //    dv1t[i] = v1[i] - v1tr[i];
                //    dv2t[i] = v2[i] - v2tr[i];
                //}
                //magv1tt = MathTimeLibr.mag(dv1);
                //magv2tt = MathTimeLibr.mag(dv2);
                ////strbuild.AppendLine(detailAll);  // dont do again
                //double knew = 1.1;
                //detailAll = ("T" + detailSum.PadLeft(2) + result.LambertCalculations.ToString().PadLeft(4) + 0.ToString().PadLeft(3) + "   " + dm + "  " + df + dtwait.ToString("0.00000000000").PadLeft(15) +
                //           dtsec.ToString("0.00000000000").PadLeft(15) + knew.ToString("0.00000000000").PadLeft(15) +
                //           v1tr[0].ToString("0.00000000000").PadLeft(15) + v1tr[1].ToString("0.00000000000").PadLeft(15) + v1tr[2].ToString("0.00000000000").PadLeft(15) +
                //           v2tr[0].ToString("0.00000000000").PadLeft(15) + v2tr[1].ToString("0.00000000000").PadLeft(15) + v2tr[2].ToString("0.00000000000").PadLeft(15) +
                //           (Math.Acos(cosdeltanu) * 180 / Math.PI).ToString("0.00000000000").PadLeft(15) + caseopt + hitearth);
                ////                strbuild.AppendLine(detailAll);

                ////                strbuild.AppendLine(magv1tt.ToString("0.00000000000") + "  " + magv2tt.ToString("0.00000000000") + " \n");
                //if ((Math.Abs(magv1t - magv1tt) > 0.01) || (Math.Abs(magv2t - magv2tt) > 0.01))
                //    strbuild.AppendLine("Error between the approaches");

                // timing of routines
                //var watch = System.Diagnostics.Stopwatch.StartNew();
                //int l = 0;
                //for (l = 1; l < 500; l++)
                {
                    strbuild.AppendLine(" TEST ------------------ s/l L 0 rev ------------------");
                    dm = 'L';
                    de = 'L';
                    // k near 180 is about 53017 while battin is 30324!
                    AstroLambertkLibr.lambertK(r1, v1, r2, dm, de, nrev, dtwait, dtsec, 0.0, 0.0, numiter, altpadc, 'n', 'n',
                        out v1tk, out v2tk, out f, out g, out gdot, out hitearth, out errorout, out detailSum, out detailAll);
                    strbuild.AppendLine(detailAll);
                    AstroLibr.kepler(r1, v1tk, dtsec, out r3h, out v3h);
                    for (int j = 0; j < 3; j++)
                        dr[j] = r2[j] - r3h[j];
                    if (MathTimeLibr.mag(dr) > 0.01)
                        strbuild.AppendLine("velk does not get to r2 (km) position " + MathTimeLibr.mag(dr).ToString() + "\n");

                    //strbuild.AppendLine("k#" + caseopt + " " + detailSum + " diffs " + MathTimeLibr.mag(dr).ToString("0.00000000000"));
                    strbuild.AppendLine("lamk v1t " + " " + v1tk[0].ToString("0.00000000000") + " " + v1tk[1].ToString("0.00000000000") + " " + v1tk[2].ToString("0.00000000000"));
                    strbuild.AppendLine("lamk v2t " + " " + v2tk[0].ToString("0.00000000000") + " " + v2tk[1].ToString("0.00000000000") + " " + v2tk[2].ToString("0.00000000000"));
                    //strbuild.AppendLine(magv1t.ToString("0.0000000").PadLeft(12) + " " + magv2t.ToString("0.0000000").PadLeft(12));

                    AstroLibr.lambertuniv(r1, r2, v1, dm, de, nrev, 0.0, dtsec, 0.0, altpadc * AstroLibr.gravConst.re, 'n', out v1tu, out v2tu, out hitearth, out detailSum, out detailAll);
                    //strbuild.AppendLine(detailSum);
                    strbuild.AppendLine("univ v1t " + " " + v1tu[0].ToString("0.00000000000") + " " + v1tu[1].ToString("0.00000000000") + " " + v1tu[2].ToString("0.00000000000"));
                    strbuild.AppendLine("univ v2t " + " " + v2tu[0].ToString("0.00000000000") + " " + v2tu[1].ToString("0.00000000000") + " " + v2tu[2].ToString("0.00000000000"));
                    AstroLibr.kepler(r1, v1tu, dtsec, out r3h, out v3h);
                    for (int j = 0; j < 3; j++)
                        dr[j] = r2[j] - r3h[j];
                    if (MathTimeLibr.mag(dr) > 0.01)
                        strbuild.AppendLine("velu does not get to r2 (km) position " + MathTimeLibr.mag(dr).ToString() + "\n");

                    for (int j = 0; j < 3; j++)
                    {
                        dv1[j] = v1tk[j] - v1tu[j];
                        dv2[j] = v2tk[j] - v2tu[j];
                    }
                    if (MathTimeLibr.mag(dv1) > 0.01 || MathTimeLibr.mag(dv2) > 0.01)
                        strbuild.AppendLine("velk does not match velu \n");

                    AstroLibr.lambertbattin(r1, r2, v1, dm, de, nrev, 0.0, dtsec, altpadc * AstroLibr.gravConst.re, 'n', out v1tb, out v2tb, out hitearth, out detailSum, out detailAll);
                    //strbuild.AppendLine(detailSum);
                    strbuild.AppendLine("batt v1t " + " " + v1tb[0].ToString("0.00000000000") + " " + v1tb[1].ToString("0.00000000000") + " " + v1tb[2].ToString("0.00000000000"));
                    strbuild.AppendLine("batt v2t " + " " + v2tb[0].ToString("0.00000000000") + " " + v2tb[1].ToString("0.00000000000") + " " + v2tb[2].ToString("0.00000000000"));
                    AstroLibr.kepler(r1, v1tb, dtsec, out r3h, out v3h);
                    for (int j = 0; j < 3; j++)
                        dr[j] = r2[j] - r3h[j];
                    if (MathTimeLibr.mag(dr) > 0.01)
                        strbuild.AppendLine("velb does not get to r2 (km) position " + MathTimeLibr.mag(dr).ToString() + "\n");
                    //strbuild.AppendLine("diffs " + MathTimeLibr.mag(dr).ToString("0.00000000000"));

                    for (int j = 0; j < 3; j++)
                    {
                        dv1[j] = v1tk[j] - v1tb[j];
                        dv2[j] = v2tk[j] - v2tb[j];
                    }
                    if (MathTimeLibr.mag(dv1) > 0.01 || MathTimeLibr.mag(dv2) > 0.01)
                        strbuild.AppendLine("velk does not match velb \n");
                    //strbuild.AppendLine("diffs " + MathTimeLib::mag(dr).ToString("0.00000000000"));

                    for (int j = 0; j < 3; j++)
                    {
                        dv1[j] = v1tk[j] - v1tb[j];
                        dv2[j] = v2tk[j] - v2tb[j];
                    }
                    if (MathTimeLibr.mag(dv1) > 0.01 || MathTimeLibr.mag(dv2) > 0.01)
                        strbuild.AppendLine("velk does not match velb \n");
                    //strbuild.AppendLine("diffs " + MathTimeLib::mag(dr).ToString("0.00000000000"));
                }
                //watch.Stop();
                //var elapsedMs = watch.ElapsedMilliseconds;
                //Console.WriteLine(watch.ElapsedMilliseconds); 

                // use random nrevs, but check if nrev = 0 and set to 1
                // but then you have to check that there is enough time for 1 rev
                int nnrev = nrev;
                if (nnrev == 0)
                    nnrev = 1;

                AstroLibr.lambertminT(r1, r2, 'S', 'L', nnrev, out tmin, out tminp, out tminenergy);
                strbuild.AppendLine("mint S " + tmin.ToString("0.0000") + " minp " + tminp.ToString("0.0000") + " minener " + tminenergy.ToString("0.0000"));
                AstroLibr.lambertminT(r1, r2, 'L', 'L', nnrev, out tmin, out tminp, out tminenergy);
                strbuild.AppendLine("mint L " + tmin.ToString("0.0000") + " minp " + tminp.ToString("0.0000") + " minener " + tminenergy.ToString("0.0000"));

                strbuild.AppendLine(" TEST ------------------ S  L " + nnrev.ToString() + " rev ------------------");
                if (dtsec / tusec >= tbidk[nnrev, 2])
                {
                    dm = 'S';
                    de = 'L';
                    AstroLambertkLibr.lambertkmins(s, tau, nnrev, de, out kbi, out tof);
                    AstroLambertkLibr.lambertK(r1, v1, r2, dm, de, nnrev, dtwait, dtsec, tof, kbi, numiter, altpadc, 'n', 'n',
                        out v1tk, out v2tk, out f, out g, out gdot, out hitearth, out errorout, out detailSum, out detailAll);
                    strbuild.AppendLine(detailAll);
                    AstroLibr.kepler(r1, v1tk, dtsec, out r3h, out v3h);
                    for (int j = 0; j < 3; j++)
                        dr[j] = r2[j] - r3h[j];
                    if (MathTimeLibr.mag(dr) > 0.01)
                        strbuild.AppendLine("velk does not get to r2 (km) position " + MathTimeLibr.mag(dr).ToString() + "\n");

                    //strbuild.AppendLine("k#" + caseopt + " " + detailSum + " diffs " + MathTimeLibr.mag(dr).ToString("0.00000000000"));
                    strbuild.AppendLine("lamk v1t " + " " + v1tk[0].ToString("0.00000000000") + " " + v1tk[1].ToString("0.00000000000") + " " + v1tk[2].ToString("0.00000000000"));
                    strbuild.AppendLine("lamk v2t " + " " + v2tk[0].ToString("0.00000000000") + " " + v2tk[1].ToString("0.00000000000") + " " + v2tk[2].ToString("0.00000000000"));
                    //strbuild.AppendLine(magv1t.ToString("0.0000000").PadLeft(12) + " " + magv2t.ToString("0.0000000").PadLeft(12));

                    AstroLibr.lambertumins(r1, r2, nnrev, dm, out kbi, out tof);
                    AstroLibr.lambertuniv(r1, r2, v1, dm, de, nnrev, 0.0, dtsec, kbi, altpadc * AstroLibr.gravConst.re, 'n', out v1tu, out v2tu, out hitearth, out detailSum, out detailAll);
                    //strbuild.AppendLine(detailSum);
                    strbuild.AppendLine("univ v1t " + " " + v1tu[0].ToString("0.00000000000") + " " + v1tu[1].ToString("0.00000000000") + " " + v1tu[2].ToString("0.00000000000"));
                    strbuild.AppendLine("univ v2t " + " " + v2tu[0].ToString("0.00000000000") + " " + v2tu[1].ToString("0.00000000000") + " " + v2tu[2].ToString("0.00000000000"));
                    AstroLibr.kepler(r1, v1tu, dtsec, out r3h, out v3h);
                    for (int j = 0; j < 3; j++)
                        dr[j] = r2[j] - r3h[j];
                    if (MathTimeLibr.mag(dr) > 0.01)
                        strbuild.AppendLine("velu does not get to r2 (km) position " + MathTimeLibr.mag(dr).ToString() + "\n");

                    for (int j = 0; j < 3; j++)
                    {
                        dv1[j] = v1tk[j] - v1tu[j];
                        dv2[j] = v2tk[j] - v2tu[j];
                    }
                    if (MathTimeLibr.mag(dv1) > 0.01 || MathTimeLibr.mag(dv2) > 0.01)
                        strbuild.AppendLine("velk does not match velu \n");

                    AstroLibr.lambertbattin(r1, r2, v1, dm, de, nnrev, 0.0, dtsec, altpadc * AstroLibr.gravConst.re, 'n', out v1tb, out v2tb, out hitearth, out detailSum, out detailAll);
                    //strbuild.AppendLine(detailSum);
                    strbuild.AppendLine("batt v1t " + " " + v1tb[0].ToString("0.00000000000") + " " + v1tb[1].ToString("0.00000000000") + " " + v1tb[2].ToString("0.00000000000"));
                    strbuild.AppendLine("batt v2t " + " " + v2tb[0].ToString("0.00000000000") + " " + v2tb[1].ToString("0.00000000000") + " " + v2tb[2].ToString("0.00000000000"));
                    AstroLibr.kepler(r1, v1tb, dtsec, out r3h, out v3h);
                    for (int j = 0; j < 3; j++)
                        dr[j] = r2[j] - r3h[j];
                    if (MathTimeLibr.mag(dr) > 0.01)
                        strbuild.AppendLine("velb does not get to r2 (km) position " + MathTimeLibr.mag(dr).ToString() + "\n");
                    //strbuild.AppendLine("diffs " + MathTimeLibr.mag(dr).ToString("0.00000000000"));

                    for (int j = 0; j < 3; j++)
                    {
                        dv1[j] = v1tk[j] - v1tb[j];
                        dv2[j] = v2tk[j] - v2tb[j];
                    }
                    if (MathTimeLibr.mag(dv1) > 0.01 || MathTimeLibr.mag(dv2) > 0.01)
                        strbuild.AppendLine("velk does not match velb \n");
                    //strbuild.AppendLine("diffs " + MathTimeLib::mag(dr).ToString("0.00000000000"));
                }
                else
                    strbuild.AppendLine(" ------------------------- not enough time for 1 revs ");

                strbuild.AppendLine(" TEST ------------------ L  L " + nnrev.ToString() + " rev ------------------");
                if (dtsec / tusec >= tbidk[nnrev, 2])
                {
                    dm = 'L';
                    de = 'L';
                    // switch tdi!!  tdidk to tdirk
                    AstroLambertkLibr.lambertkmins(s, tau, nnrev, 'H', out kbi, out tof);  // 'H'

                    //double tofk1, kbik2, tofk2, kbik1;
                    //string outstr;
                    //getmins(1, 'k', nrev, r1, r2, s, tau, dm, de, out tofk1, out kbik1, out tofk2, out kbik2, out outstr);

                    AstroLambertkLibr.lambertK(r1, v1, r2, dm, de, nnrev, dtwait, dtsec, tof, kbi, numiter, altpadc, 'n', 'n',
                        out v1tk, out v2tk, out f, out g, out gdot, out hitearth, out errorout, out detailSum, out detailAll);
                    strbuild.AppendLine(detailAll);
                    AstroLibr.kepler(r1, v1tk, dtsec, out r3h, out v3h);
                    for (int j = 0; j < 3; j++)
                        dr[j] = r2[j] - r3h[j];
                    if (MathTimeLibr.mag(dr) > 0.01)
                        strbuild.AppendLine("velk does not get to r2 (km) position " + MathTimeLibr.mag(dr).ToString() + "\n");

                    //strbuild.AppendLine("k#" + caseopt + " " + detailSum + " diffs " + MathTimeLibr.mag(dr).ToString("0.00000000000"));
                    strbuild.AppendLine("lamk v1t " + " " + v1tk[0].ToString("0.00000000000") + " " + v1tk[1].ToString("0.00000000000") + " " + v1tk[2].ToString("0.00000000000"));
                    strbuild.AppendLine("lamk v2t " + " " + v2tk[0].ToString("0.00000000000") + " " + v2tk[1].ToString("0.00000000000") + " " + v2tk[2].ToString("0.00000000000"));
                    //strbuild.AppendLine(magv1t.ToString("0.0000000").PadLeft(12) + " " + magv2t.ToString("0.0000000").PadLeft(12));

                    AstroLibr.lambertumins(r1, r2, nnrev, dm, out kbi, out tof);
                    AstroLibr.lambertuniv(r1, r2, v1, dm, de, nnrev, 0.0, dtsec, kbi, altpadc * AstroLibr.gravConst.re, 'n', out v1tu, out v2tu, out hitearth, out detailSum, out detailAll);
                    //strbuild.AppendLine(detailSum);
                    strbuild.AppendLine("univ v1t " + " " + v1tu[0].ToString("0.00000000000") + " " + v1tu[1].ToString("0.00000000000") + " " + v1tu[2].ToString("0.00000000000"));
                    strbuild.AppendLine("univ v2t " + " " + v2tu[0].ToString("0.00000000000") + " " + v2tu[1].ToString("0.00000000000") + " " + v2tu[2].ToString("0.00000000000"));
                    AstroLibr.kepler(r1, v1tu, dtsec, out r3h, out v3h);
                    for (int j = 0; j < 3; j++)
                        dr[j] = r2[j] - r3h[j];
                    if (MathTimeLibr.mag(dr) > 0.01)
                        strbuild.AppendLine("velu does not get to r2 (km) position " + MathTimeLibr.mag(dr).ToString() + "\n");

                    for (int j = 0; j < 3; j++)
                    {
                        dv1[j] = v1tk[j] - v1tu[j];
                        dv2[j] = v2tk[j] - v2tu[j];
                    }
                    if (MathTimeLibr.mag(dv1) > 0.01 || MathTimeLibr.mag(dv2) > 0.01)
                        strbuild.AppendLine("velk does not match velu \n");

                    AstroLibr.lambertbattin(r1, r2, v1, dm, de, nnrev, 0.0, dtsec, altpadc * AstroLibr.gravConst.re, 'n', out v1tb, out v2tb, out hitearth, out detailSum, out detailAll);
                    //strbuild.AppendLine(detailSum);
                    strbuild.AppendLine("batt v1t " + " " + v1tb[0].ToString("0.00000000000") + " " + v1tb[1].ToString("0.00000000000") + " " + v1tb[2].ToString("0.00000000000"));
                    strbuild.AppendLine("batt v2t " + " " + v2tb[0].ToString("0.00000000000") + " " + v2tb[1].ToString("0.00000000000") + " " + v2tb[2].ToString("0.00000000000"));
                    AstroLibr.kepler(r1, v1tb, dtsec, out r3h, out v3h);
                    for (int j = 0; j < 3; j++)
                        dr[j] = r2[j] - r3h[j];
                    if (MathTimeLibr.mag(dr) > 0.01)
                        strbuild.AppendLine("velb does not get to r2 (km) position " + MathTimeLibr.mag(dr).ToString() + "\n");
                    //strbuild.AppendLine("diffs " + MathTimeLibr.mag(dr).ToString("0.00000000000"));

                    for (int j = 0; j < 3; j++)
                    {
                        dv1[j] = v1tk[j] - v1tb[j];
                        dv2[j] = v2tk[j] - v2tb[j];
                    }
                    if (MathTimeLibr.mag(dv1) > 0.01 || MathTimeLibr.mag(dv2) > 0.01)
                        strbuild.AppendLine("velk does not match velb \n");
                    //strbuild.AppendLine("diffs " + MathTimeLib::mag(dr).ToString("0.00000000000"));
                }
                else
                    strbuild.AppendLine(" ------------------------- not enough time for 1 revs ");

                strbuild.AppendLine(" TEST ------------------ S  H " + nnrev.ToString() + " rev ------------------");
                if (dtsec / tusec >= tbirk[nnrev, 2])
                {
                    dm = 'S';
                    de = 'H';
                    // switch tdi!!  tdirk to tdidk
                    AstroLambertkLibr.lambertkmins(s, tau, nnrev, 'L', out kbi, out tof);  // 'L'
                    AstroLambertkLibr.lambertK(r1, v1, r2, dm, de, nnrev, dtwait, dtsec, tof, kbi, numiter, altpadc, 'n', 'n',
                        out v1tk, out v2tk, out f, out g, out gdot, out hitearth, out errorout, out detailSum, out detailAll);
                    strbuild.AppendLine(detailAll);
                    AstroLibr.kepler(r1, v1tk, dtsec, out r3h, out v3h);
                    for (int j = 0; j < 3; j++)
                        dr[j] = r2[j] - r3h[j];
                    if (MathTimeLibr.mag(dr) > 0.01)
                        strbuild.AppendLine("velk does not get to r2 (km) position " + MathTimeLibr.mag(dr).ToString() + "\n");

                    //strbuild.AppendLine("k#" + caseopt + " " + detailSum + " diffs " + MathTimeLibr.mag(dr).ToString("0.00000000000"));
                    strbuild.AppendLine("lamk v1t " + " " + v1tk[0].ToString("0.00000000000") + " " + v1tk[1].ToString("0.00000000000") + " " + v1tk[2].ToString("0.00000000000"));
                    strbuild.AppendLine("lamk v2t " + " " + v2tk[0].ToString("0.00000000000") + " " + v2tk[1].ToString("0.00000000000") + " " + v2tk[2].ToString("0.00000000000"));
                    //strbuild.AppendLine(magv1t.ToString("0.0000000").PadLeft(12) + " " + magv2t.ToString("0.0000000").PadLeft(12));

                    AstroLibr.lambertumins(r1, r2, nnrev, dm, out kbi, out tof);
                    AstroLibr.lambertuniv(r1, r2, v1, dm, de, nnrev, 0.0, dtsec, kbi, altpadc * AstroLibr.gravConst.re, 'n', out v1tu, out v2tu, out hitearth, out detailSum, out detailAll);
                    //strbuild.AppendLine(detailSum);
                    strbuild.AppendLine("univ v1t " + " " + v1tu[0].ToString("0.00000000000") + " " + v1tu[1].ToString("0.00000000000") + " " + v1tu[2].ToString("0.00000000000"));
                    strbuild.AppendLine("univ v2t " + " " + v2tu[0].ToString("0.00000000000") + " " + v2tu[1].ToString("0.00000000000") + " " + v2tu[2].ToString("0.00000000000"));
                    AstroLibr.kepler(r1, v1tu, dtsec, out r3h, out v3h);
                    for (int j = 0; j < 3; j++)
                        dr[j] = r2[j] - r3h[j];
                    if (MathTimeLibr.mag(dr) > 0.01)
                        strbuild.AppendLine("velu does not get to r2 (km) position " + MathTimeLibr.mag(dr).ToString() + "\n");

                    for (int j = 0; j < 3; j++)
                    {
                        dv1[j] = v1tk[j] - v1tu[j];
                        dv2[j] = v2tk[j] - v2tu[j];
                    }
                    if (MathTimeLibr.mag(dv1) > 0.01 || MathTimeLibr.mag(dv2) > 0.01)
                        strbuild.AppendLine("velk does not match velu \n");

                    AstroLibr.lambertbattin(r1, r2, v1, dm, de, nnrev, 0.0, dtsec, altpadc * AstroLibr.gravConst.re, 'n', out v1tb, out v2tb, out hitearth, out detailSum, out detailAll);
                    //strbuild.AppendLine(detailSum);
                    strbuild.AppendLine("batt v1t " + " " + v1tb[0].ToString("0.00000000000") + " " + v1tb[1].ToString("0.00000000000") + " " + v1tb[2].ToString("0.00000000000"));
                    strbuild.AppendLine("batt v2t " + " " + v2tb[0].ToString("0.00000000000") + " " + v2tb[1].ToString("0.00000000000") + " " + v2tb[2].ToString("0.00000000000"));
                    AstroLibr.kepler(r1, v1tb, dtsec, out r3h, out v3h);
                    for (int j = 0; j < 3; j++)
                        dr[j] = r2[j] - r3h[j];
                    if (MathTimeLibr.mag(dr) > 0.01)
                        strbuild.AppendLine("velb does not get to r2 (km) position " + MathTimeLibr.mag(dr).ToString() + "\n");
                    //strbuild.AppendLine("diffs " + MathTimeLibr.mag(dr).ToString("0.00000000000"));

                    for (int j = 0; j < 3; j++)
                    {
                        dv1[j] = v1tk[j] - v1tb[j];
                        dv2[j] = v2tk[j] - v2tb[j];
                    }
                    if (MathTimeLibr.mag(dv1) > 0.01 || MathTimeLibr.mag(dv2) > 0.01)
                        strbuild.AppendLine("velk does not match velb \n");
                    //strbuild.AppendLine("diffs " + MathTimeLib::mag(dr).ToString("0.00000000000"));
                }
                else
                    strbuild.AppendLine(" ------------------------- not enough time for 1 revs ");

                strbuild.AppendLine(" TEST ------------------ L  H " + nnrev.ToString() + " rev ------------------");
                if (dtsec / tusec >= tbirk[nnrev, 2])
                {
                    dm = 'L';
                    de = 'H';
                    AstroLambertkLibr.lambertkmins(s, tau, nnrev, de, out kbi, out tof);
                    AstroLambertkLibr.lambertK(r1, v1, r2, dm, de, nnrev, dtwait, dtsec, tof, kbi, numiter, altpadc, 'n', 'n',
                        out v1tk, out v2tk, out f, out g, out gdot, out hitearth, out errorout, out detailSum, out detailAll);
                    strbuild.AppendLine(detailAll);
                    AstroLibr.kepler(r1, v1tk, dtsec, out r3h, out v3h);
                    for (int j = 0; j < 3; j++)
                        dr[j] = r2[j] - r3h[j];
                    if (MathTimeLibr.mag(dr) > 0.01)
                        strbuild.AppendLine("velk does not get to r2 (km) position " + MathTimeLibr.mag(dr).ToString() + "\n");

                    //strbuild.AppendLine("k#" + caseopt + " " + detailSum + " diffs " + MathTimeLibr.mag(dr).ToString("0.00000000000"));
                    strbuild.AppendLine("lamk v1t " + " " + v1tk[0].ToString("0.00000000000") + " " + v1tk[1].ToString("0.00000000000") + " " + v1tk[2].ToString("0.00000000000"));
                    strbuild.AppendLine("lamk v2t " + " " + v2tk[0].ToString("0.00000000000") + " " + v2tk[1].ToString("0.00000000000") + " " + v2tk[2].ToString("0.00000000000"));
                    //strbuild.AppendLine(magv1t.ToString("0.0000000").PadLeft(12) + " " + magv2t.ToString("0.0000000").PadLeft(12));

                    AstroLibr.lambertumins(r1, r2, nnrev, dm, out kbi, out tof);
                    AstroLibr.lambertuniv(r1, r2, v1, dm, de, nnrev, 0.0, dtsec, kbi, altpadc * AstroLibr.gravConst.re, 'n', out v1tu, out v2tu, out hitearth, out detailSum, out detailAll);
                    //strbuild.AppendLine(detailSum);
                    strbuild.AppendLine("univ v1t " + " " + v1tu[0].ToString("0.00000000000") + " " + v1tu[1].ToString("0.00000000000") + " " + v1tu[2].ToString("0.00000000000"));
                    strbuild.AppendLine("univ v2t " + " " + v2tu[0].ToString("0.00000000000") + " " + v2tu[1].ToString("0.00000000000") + " " + v2tu[2].ToString("0.00000000000"));
                    AstroLibr.kepler(r1, v1tu, dtsec, out r3h, out v3h);
                    for (int j = 0; j < 3; j++)
                        dr[j] = r2[j] - r3h[j];
                    if (MathTimeLibr.mag(dr) > 0.01)
                        strbuild.AppendLine("velu does not get to r2 (km) position " + MathTimeLibr.mag(dr).ToString() + "\n");

                    for (int j = 0; j < 3; j++)
                    {
                        dv1[j] = v1tk[j] - v1tu[j];
                        dv2[j] = v2tk[j] - v2tu[j];
                    }
                    if (MathTimeLibr.mag(dv1) > 0.01 || MathTimeLibr.mag(dv2) > 0.01)
                        strbuild.AppendLine("velk does not match velu \n");

                    AstroLibr.lambertbattin(r1, r2, v1, dm, de, nnrev, 0.0, dtsec, altpadc * AstroLibr.gravConst.re, 'n', out v1tb, out v2tb, out hitearth, out detailSum, out detailAll);
                    //strbuild.AppendLine(detailSum);
                    strbuild.AppendLine("batt v1t " + " " + v1tb[0].ToString("0.00000000000") + " " + v1tb[1].ToString("0.00000000000") + " " + v1tb[2].ToString("0.00000000000"));
                    strbuild.AppendLine("batt v2t " + " " + v2tb[0].ToString("0.00000000000") + " " + v2tb[1].ToString("0.00000000000") + " " + v2tb[2].ToString("0.00000000000"));
                    AstroLibr.kepler(r1, v1tb, dtsec, out r3h, out v3h);
                    for (int j = 0; j < 3; j++)
                        dr[j] = r2[j] - r3h[j];
                    if (MathTimeLibr.mag(dr) > 0.01)
                        strbuild.AppendLine("velb does not get to r2 (km) position " + MathTimeLibr.mag(dr).ToString() + "\n");
                    //strbuild.AppendLine("diffs " + MathTimeLibr.mag(dr).ToString("0.00000000000"));

                    for (int j = 0; j < 3; j++)
                    {
                        dv1[j] = v1tk[j] - v1tb[j];
                        dv2[j] = v2tk[j] - v2tb[j];
                    }
                    if (MathTimeLibr.mag(dv1) > 0.01 || MathTimeLibr.mag(dv2) > 0.01)
                        strbuild.AppendLine("velk does not match velb \n");
                    //strbuild.AppendLine("diffs " + MathTimeLib::mag(dr).ToString("0.00000000000"));
                }
                else
                    strbuild.AppendLine(" ------------------------- not enough time for 1 revs ");

                strbuild.AppendLine(" --------------------------------end case " + caseopt + "------------------------------------------------ ");
                string resultStr = strbuild.ToString();
            }

            string directory = @"D:\Codes\LIBRARY\cs\TestAll\";
            File.WriteAllText(directory + "tlambertknown.out", strbuild.ToString());

            this.opsStatus.Text = "Done ";
            Refresh();
        }  // testknowncases


        public void testradecgeo2azel()
        {
            double rad = 180.0 / Math.PI;
            double rtasc, decl, rr, latgd, lon, alt, az, el;
            double ttt, jdut1, lod, xp, yp, ddpsi, ddeps;

            rtasc = 294.98914583 / rad;
            decl = -20.8234944 / rad;
            xp = 0.0;
            yp = 0.0;
            lod = 0.0;
            jdut1 = 2453101.82740678310;
            ttt = 0.042623631889;
            ddpsi = -0.052195;
            ddeps = -0.003875;
            rr = 12373.3546098;  // km
            latgd = 39.007 / rad;
            lon = -104.883 / rad;
            alt = 0.3253;

            AstroLibr.radecgeo2azel(rtasc, decl, rr, latgd, lon, alt, ttt, jdut1, lod, xp, yp, ddpsi, ddeps, AstroLib.EOpt.e80, out az, out el);
        }

        public void testijk2ll()
        {
            double[] r = new double[3];
            double latgc, latgd, lon, hellp, rad;
            rad = 180.0 / Math.PI;

            r = new double[] { 1.023 * AstroLibr.gravConst.re, 1.076 * AstroLibr.gravConst.re, 1.011 * AstroLibr.gravConst.re };

            AstroLibr.ecef2ll(r, out latgc, out latgd, out lon, out hellp);

            strbuild.AppendLine("ecef2ll " + (latgd*rad).ToString(fmt) + " " + 
                (lon * rad).ToString(fmt) + " "+ hellp.ToString(fmt) + "\n");
        }

        public void testgd2gc()
        {
            double rad = 180.0 / Math.PI;
            double latgd, ans;
            latgd = 34.173429 / rad;

            ans = AstroLibr.gd2gc(latgd);

            strbuild.AppendLine("gd2gc " + ans.ToString(fmt) + "\n");
        }

        public void testsite()
        {
            double rad = 180.0 / Math.PI;
            double latgd, lon, alt;
            double[] rsecef;
            double[] vsecef;
            latgd = 39.007 / rad;
            lon = -104.883 / rad;
            alt = 0.3253;

            AstroLibr.site(latgd, lon, alt, out rsecef, out vsecef);

            strbuild.AppendLine("site " + rsecef[0].ToString(fmt) + " " + rsecef[1].ToString(fmt) + " " + rsecef[2].ToString(fmt) + " " +
                        vsecef[0].ToString(fmt) + " " + vsecef[1].ToString(fmt) + " " + vsecef[2].ToString(fmt));
        }


        // --------  sun          - analytical sun ephemeris
        public void testsun()
        {
            double jd, rtasc, decl;
            double[] rsun;
            jd = 2449444.5;
            AstroLibr.sun(jd, out rsun, out rtasc, out decl);

            strbuild.AppendLine("sun " + rsun[0].ToString(fmt) + " " + rsun[1].ToString(fmt) + " " + rsun[2].ToString(fmt));
        }

        // --------  moon         - analytical moon ephemeris
        public void testmoon()
        {
            double jd, rtasc, decl;
            double[] rmoon;

            jd = 2449470.5;
            AstroLibr.moon(jd, out rmoon, out rtasc, out decl);

            strbuild.AppendLine("moon " + rmoon[0].ToString(fmt) + " " + rmoon[1].ToString(fmt) + " " + rmoon[2].ToString(fmt));
        }

        public void testkepler()
        {
            double[] r1;
            double[] v1;
            double[] r2;
            double[] v2;
            double dtsec;
            dtsec = 42397.344;  // s

            r1 = new double[] { 2.500000 * AstroLibr.gravConst.re, 0.000000, 0.000000 };
            // assume circular initial orbit for vel calcs
            v1 = new double[] { 0.0, Math.Sqrt(AstroLibr.gravConst.mu / r1[0]), 0.0 };

            AstroLibr.kepler(r1, v1, dtsec, out r2, out v2);

            strbuild.AppendLine("kepler " + r2[0].ToString(fmt) + " " + r2[1].ToString(fmt) + " " + r2[2].ToString(fmt) + " " + 
                                v2[0].ToString(fmt) + " " + v2[1].ToString(fmt) + " " + v2[2].ToString(fmt));
        }



        // test in geoloc.sln
        //public void testcovct2cl()
        //{
        //    double[,] cartcov = new double[6, 6];
        //    double[] cartstate = new double[6];
        //    string anomclass;
        //    double[,] classcov = new double[6, 6];
        //    double[,] tm = new double[6, 6];

        //    MathTimeLibr.covct2cl(cartcov, cartstate, anomclass, out classcov, out tm);

        //}
        //public void testcovcl2ct()
        //{
        //    MathTimeLibr.covcl2ct
        //    (double[,] classcov, double[] classstate, string anomclass, out double[,] cartcov, out double[,] tm
        //            );
        //}
        //public void testcovct2eq()
        //{
        //    double[] classState = new double[6];
        //    double[] cartState = new double[6];
        //    double[] eqState = new double[6];
        //    double[] flState = new double[6];
        //    double[,] cartCov = new double[6, 6];
        //    double[,] classCov = new double[6, 6];
        //    double[,] eqCov = new double[6, 6];
        //    double[,] flCov = new double[6, 6];
        //    double[,] rswCov = new double[6, 6];
        //    double[,] ntwCov = new double[6, 6];
        //    double[,] tm = new double[6, 6];

        //    cartCov = new double[,] { { 1, 0, 0, 0, 0, 0 }, { 0, 1, 0, 0, 0, 0 }, { 0, 0, 1, 0, 0, 0 },
        //                         { 0, 0, 0, 1, 0, 0 }, { 0, 0, 0, 0, 1, 0 }, { 0, 0, 0, 0, 0, 1 } };


        //    MathTimeLibr.covct2eq
        //    (     double[,] cartcov, double[] cartstate, string anomeq, Int16 fr, out double[,] eqcov, out  tm                );
        //}
        //public void testcoveq2ct()
        //{
        //    MathTimeLibr.coveq2ct
        //     (                double[,] eqcov, double[] eqstate, string anomeq, Int16 fr, out double[,] cartcov, out  tm                );
        //}
        //public void testcovcl2eq()
        //{
        //    MathTimeLibr.covcl2eq
        //    (
        //            double[,] classcov, double[] classstate, string anomclass, string anomeq, Int16 fr, out double[,] eqcov, out  tm                );
        //}
        //public void testcoveq2cl()
        //{
        //    MathTimeLibr.coveq2cl(double[,] eqcov, double[] eqstate, string anomeq, string anomclass, Int16 fr, out double[,] classcov, out  tm);
        //}
        //public void testcovct2fl()
        //{
        //    MathTimeLibr.covct2fl
        //      (
        //            double[,] cartcov, double[] cartstate, string anomflt, double ttt, double jdut1, double lod,
        //            double xp, double yp, Int16 terms, double ddpsi, double ddeps, out double[,] flcov, out  tm
        //            );
        //}
        //public void testcovfl2ct()
        //{
        //    MathTimeLibr.covfl2ct(double[,] flcov, double[] flstate, string anomflt, double ttt, double jdut1, double lod,
        //            double xp, double yp, Int16 terms, double ddpsi, double ddeps, out double[,] cartcov, out  tm);

        //}
        //public void testcovct_rsw()
        //{
        //    MathTimeLibr.covct_rsw(ref double[,] cartcov, double[] cartstate, MathTimeLib.Edirection direct, ref double[,] rswcov, out  tm);
        //        direct = MathTimeLib.Edirection.eto;
        //        MathTimelibr.covct_ntw(ref cartCovo, cartState, direct, ref ntwCov, out tm);

        //    }
        //    public void testcovct_ntw()
        //{
        //        direct = MathTimeLib.Edirection.eto;
        //        MathTimelibr.covct_ntw(ref cartCovo, cartState, direct, ref ntwCov, out tm);

        //        MathTimeLibr.covct_ntw(ref double[,] cartcov, double[] cartstate, MathTimeLib.Edirection direct, ref double[,] ntwcov, out  tm);
        //}

        public void testsunmoonjpl()
        {
            AstroLib.jpldedataClass[] jpldearr = AstroLibr.jpldearr;
            double[] rsun = new double[3];
            double[] rmoon = new double[3];
            double rtascs, decls, rtascm, declm, rsmag, rmmag, jdjpldestart, jdjpldestartFrac, jd, jdF;

            MathTimeLibr.jday(2017, 5, 11, 3, 51, 42.7657, out jd, out jdF);

            strbuild.AppendLine(" =============================   test sun and moon ephemerides =============================\n");

            // read in jpl sun moon files
            AstroLibr.initjplde(ref jpldearr, "D:/Codes/LIBRARY/DataLib/", "sunmooneph_430t.txt", out jdjpldestart, out jdjpldestartFrac);

            AstroLibr.findjpldeparam(jd, 0.0, 'l', jpldearr, jdjpldestart, out rsun, out rsmag, out rmoon, out rmmag);
            strbuild.AppendLine("findjpldeephem 0000 hrs l\n " + jd.ToString() + " 0.00000 " + rsun[0].ToString() + " " +
                rsun[1].ToString() + " " + rsun[2].ToString() + " " +
                rmoon[0].ToString() + " " + rmoon[1].ToString() + " " + rmoon[2].ToString());

            AstroLibr.findjpldeparam(jd, jdF, 's', jpldearr, jdjpldestart, out rsun, out rsmag, out rmoon, out rmmag);
            strbuild.AppendLine("findjpldeephem 0000 hrs s\n " + jd.ToString() + " " + jdF.ToString() + " " + rsun[0].ToString() + " " +
                rsun[1].ToString() + " " + rsun[2].ToString() + " " +
                rmoon[0].ToString() + " " + rmoon[1].ToString() + " " + rmoon[2].ToString());

            AstroLibr.sunmoonjpl(jd, jdF, 's', ref jpldearr, jdjpldestart, out rsun, out rtascs, out decls, out rmoon, out rtascm, out declm);
            strbuild.AppendLine("sunmoon 0000 hrs s\n " + jd.ToString() + " " + jdF.ToString() + " " + rsun[0].ToString() + " " +
                rsun[1].ToString() + " " + rsun[2].ToString() + " " +
                rmoon[0].ToString() + " " + rmoon[1].ToString() + " " + rmoon[2].ToString());


            AstroLibr.findjpldeparam(jd, jdF, 'l', jpldearr, jdjpldestart, out rsun, out rsmag, out rmoon, out rmmag);
            strbuild.AppendLine("findjpldeephem 0000 hrs l\n " + jd.ToString() + " " + jdF.ToString() + " " + rsun[0].ToString() + " " +
                rsun[1].ToString() + " " + rsun[2].ToString() + " " +
                rmoon[0].ToString() + " " + rmoon[1].ToString() + " " + rmoon[2].ToString());

            AstroLibr.sunmoonjpl(jd, jdF, 'l', ref jpldearr, jdjpldestart, out rsun, out rtascs, out decls, out rmoon, out rtascm, out declm);
            strbuild.AppendLine("sunmoon 0000 hrs l\n " + jd.ToString() + " " + jdF.ToString() + " " + rsun[0].ToString() + " " +
                rsun[1].ToString() + " " + rsun[2].ToString() + " " +
                rmoon[0].ToString() + " " + rmoon[1].ToString() + " " + rmoon[2].ToString());

            AstroLibr.findjpldeparam(jd, 1.0, 'l', jpldearr, jdjpldestart, out rsun, out rsmag, out rmoon, out rmmag);
            strbuild.AppendLine("findjpldeephem 2400 hrs s\n " + jd.ToString() + " " + jdF.ToString() + " " + rsun[0].ToString() + " " +
                rsun[1].ToString() + " " + rsun[2].ToString() + " " +
                rmoon[0].ToString() + " " + rmoon[1].ToString() + " " + rmoon[2].ToString());


            // ex 8.5 test
            MathTimeLibr.jday(2020, 2, 18, 15, 8, 47.23847, out jd, out jdF);
            AstroLibr.findjpldeparam(jd, jdF, 's', jpldearr, jdjpldestart, out rsun, out rsmag, out rmoon, out rmmag);
            strbuild.AppendLine("ex findjpldeephem 0000 hrs s\n " + jd.ToString() + " " + jdF.ToString() + " " + rsun[0].ToString() + " " +
                rsun[1].ToString() + " " + rsun[2].ToString() + " " +
                rmoon[0].ToString() + " " + rmoon[1].ToString() + " " + rmoon[2].ToString());

            // test interpolation of vectors
            // shows spline is MUCH better - 3 km sun variation in mid day linear, 60m diff with spline. 
            strbuild.AppendLine("findjplde  mfme     rsun x             y                 z             rmoon x             y                z      (km)");
            int ii;
            for (ii = 0; ii < 90; ii++)
            {
                AstroLibr.findjpldeparam(jd, (ii * 1.0) / 24.0, 's', jpldearr, jdjpldestart, out rsun, out rsmag, out rmoon, out rmmag);
                strbuild.AppendLine(" " + jd.ToString() + " " + (ii * 60.0).ToString("0000") + " " +
                    rsun[0].ToString() + " " + rsun[1].ToString() + " " + rsun[2].ToString() + " " +
                    rmoon[0].ToString() + " " + rmoon[1].ToString() + " " + rmoon[2].ToString());
            }
        }

        public void testkp2ap()
        {
            int i;
            double kp, ap;

            for (i = 1; i <= 27; i++)
            {
                kp = 10.0 * i / 3.0;
                ap = EOPSPWLibr.kp2ap(kp);
                // get spacing correct, leading 0, front spaces
                strbuild.AppendLine(i.ToString("##") + (0.1 * kp).ToString("  0.######") + ap.ToString("  0.######"));
            }
        }


        public void testazel2radec()
        {
            double rad = 180.0 / Math.PI;
            double[] reci = new double[3];
            double[] veci = new double[3];
            double[] recef = new double[3];
            double[] vecef = new double[3];
            double[] rsecef = new double[3];
            double[] vsecef = new double[3];
            double[] rseci = new double[3];
            double[] vseci = new double[3];
            double rho, az, el, drho, daz, del, alt, latgd, lon;
            double rr, rtasc, decl, drr, drtasc, ddecl;
            double trtasc, tdecl, dtrtasc, dtdecl;
            double ttt, xp, yp, lod, jdut1, ddpsi, ddeps, ddx, ddy, dut1, lst, gst, jdtt, jdftt;
            int year, mon, day, hr, minute, dat;
            double second, jd, jdFrac;

            rr = trtasc = tdecl = rtasc = decl = drr = dtrtasc = dtdecl = drtasc = ddecl = 0.0;
            rho = az = el = drho = daz = del = 0.0;

            EOPSPWLib.iau80Class iau80arr;
            EOPSPWLib.iau00Class iau00arr;
            string nutLoc;
            nutLoc = @"D:\Codes\LIBRARY\DataLib\nut80.dat";
            EOPSPWLibr.iau80in(nutLoc, out iau80arr);
            nutLoc = @"D:\Codes\LIBRARY\DataLib\";
            EOPSPWLibr.iau00in(nutLoc, out iau00arr);

            // now read it in
            double jdxysstart, jdfxysstart;
            AstroLib.xysdataClass[] xysarr = AstroLibr.xysarr;
            AstroLibr.initXYS(ref xysarr, nutLoc, "xysdata.dat", out jdxysstart, out jdfxysstart);

            xp = 0.0;
            yp = 0.0;
            lod = 0.0;
            ddpsi = -0.052195;
            ddeps = -0.003875;
            ddx = 0.0;  // fixxxxxx
            ddy = 0.0;
            dut1 = -0.37816;

            year = 2015;
            mon = 12;
            day = 15;
            hr = 16;
            dat = 36;
            minute = 58;
            second = 50.208;
            MathTimeLibr.jday(year, mon, day, hr, minute, second, out jd, out jdFrac);

            // note you have to use tdb for time of ineterst AND j2000 (when dat = 32)
            jdtt = jd;
            jdftt = jdFrac + (dat + 32.184) / 86400.0;
            ttt = (jdtt + jdftt - 2451545.0) / 36525.0;
            jdut1 = jd + jdFrac + dut1 / 86400.0;

            recef = new double[] { -605.79221660, -5870.22951108, 3493.05319896 };
            recef = new double[] { -100605.79221660, -1005870.22951108, 1003493.05319896 };
            vecef = new double[] { -1.56825429, -3.70234891, -6.47948395 };
            AstroLibr.eci_ecef(ref reci, ref veci, MathTimeLib.Edirection.efrom, ref recef, ref vecef,
                AstroLib.EOpt.e80, iau80arr, iau00arr,
                jdtt, jdftt, jdut1, jdxysstart, lod, xp, yp, ddpsi, ddeps, ddx, ddy);

            lon = -104.883 / rad;
            latgd = 39.007 / rad;
            alt = 2.102;
            AstroLibr.site(latgd, lon, alt, out rsecef, out vsecef);

            AstroLibr.eci_ecef(ref rseci, ref vseci, MathTimeLib.Edirection.efrom, ref rsecef, ref vsecef,
                AstroLib.EOpt.e80, iau80arr, iau00arr,
                jdtt, jdftt, jdut1, jdxysstart, lod, xp, yp, ddpsi, ddeps, ddx, ddy);

            AstroLibr.lstime(lon, jdut1, out lst, out gst);

            // print out initial conditions
            strbuild.AppendLine("recef  " + recef[0].ToString(fmt).PadLeft(4) + " " + recef[1].ToString(fmt).PadLeft(4) + " " + recef[2].ToString(fmt).PadLeft(4) + " " +
                                "v  " + vecef[0].ToString(fmt).PadLeft(4) + " " + vecef[1].ToString(fmt).PadLeft(4) + " " + vecef[2].ToString(fmt).PadLeft(4));
            strbuild.AppendLine("rs ecef  " + rsecef[0].ToString(fmt).PadLeft(4) + " " + rsecef[1].ToString(fmt).PadLeft(4) + " " + rsecef[2].ToString(fmt).PadLeft(4));
            strbuild.AppendLine("reci  " + reci[0].ToString(fmt).PadLeft(4) + " " + reci[1].ToString(fmt).PadLeft(4) + " " + reci[2].ToString(fmt).PadLeft(4) + " " +
                                "v  " + veci[0].ToString(fmt).PadLeft(4) + " " + veci[1].ToString(fmt).PadLeft(4) + " " + veci[2].ToString(fmt).PadLeft(4));
            strbuild.AppendLine("rs eci  " + rseci[0].ToString(fmt).PadLeft(4) + " " + rseci[1].ToString(fmt).PadLeft(4) + " " + rseci[2].ToString(fmt).PadLeft(4));


            AstroLibr.rv_razel(ref recef, ref vecef, latgd, lon, alt, MathTimeLib.Edirection.eto, ref rho, ref az, ref el, ref drho, ref daz, ref del);

            AstroLibr.rv_radec(ref reci, ref veci, MathTimeLib.Edirection.eto, ref rr, ref rtasc, ref decl, ref drr, ref drtasc, ref ddecl);

            AstroLibr.rv_tradec(ref reci, ref veci, rseci, MathTimeLib.Edirection.eto, ref rho, ref trtasc, ref tdecl, ref drho, ref dtrtasc, ref dtdecl);

            // print out results
            strbuild.AppendLine("razel " + rho.ToString(fmt).PadLeft(4) + " " + az.ToString(fmt).PadLeft(4) + " " + el.ToString(fmt).PadLeft(4) + " " +
                                "  " + drho.ToString(fmt).PadLeft(4) + " " + daz.ToString(fmt).PadLeft(4) + " " + del.ToString(fmt).PadLeft(4));
            strbuild.AppendLine("radec " + rr.ToString(fmt).PadLeft(4) + " " + rtasc.ToString(fmt).PadLeft(4) + " " + decl.ToString(fmt).PadLeft(4) + " " +
                                "  " + drr.ToString(fmt).PadLeft(4) + " " + drtasc.ToString(fmt).PadLeft(4) + " " + ddecl.ToString(fmt).PadLeft(4));
            strbuild.AppendLine("tradec " + rho.ToString(fmt).PadLeft(4) + " " + trtasc.ToString(fmt).PadLeft(4) + " " + tdecl.ToString(fmt).PadLeft(4) + " " +
                                drho.ToString(fmt).PadLeft(4) + " " + dtrtasc.ToString(fmt).PadLeft(4) + " " + dtdecl.ToString(fmt).PadLeft(4));

            double rtasc1;
            AstroLibr.azel_radec(az, el, lst, latgd, out rtasc, out decl, out rtasc1);
            strbuild.AppendLine("radec " + rtasc.ToString(fmt).PadLeft(4) + " rtasc1 " + rtasc1.ToString(fmt).PadLeft(4) + " " + decl.ToString(fmt).PadLeft(4));
            strbuild.AppendLine("radec " + (Math.PI * 2 - rtasc).ToString(fmt).PadLeft(4) + " rtasc1 " + (Math.PI * 2 - rtasc1).ToString(fmt).PadLeft(4) + " " + decl.ToString(fmt).PadLeft(4));
        }


        /* ------------------------------------------------------------------------------
         *
         *                           function LegPolyEx
         *
         *   this function finds the exact (from equations) Legendre polynomials for the gravity field. 
         *   note that the arrays are indexed from 0 to coincide with the usual nomenclature (eq 8-21 
         *   in my text). fortran implementations will have indicies of 1 greater as they often 
         *   start at 1. these are exact expressions derived from mathematica. 
         *      
         *  author        : david vallado             davallado@gmail.com  16 dec 2019
         *
         *  inputs        description                                   range / units
         *    latgc       - Geocentric lat of satellite                   pi to pi rad          
         *    order       - size of gravity field                         1..2160..
         *
         *  outputs       :
         *    LegArr      - [,] array of Legendre polynomials
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
          ------------------------------------------------------------------------------*/

        public void LegPolyEx
           (
           double latgc,
           Int32 order,
           out double[,] LegArrEx
           )
        {
            LegArrEx = new double[order + 2, order + 2];

            double s = LegArrEx[1, 0] = Math.Sin(latgc);
            double c = LegArrEx[1, 1] = Math.Cos(latgc);

            // -------------------- exact epxressions ---------------------- }
            LegArrEx[2, 0] = 0.5 * (3 * s * s - 1.0);
            LegArrEx[2, 1] = 3.0 * s * c;
            LegArrEx[2, 2] = 3.0 * c * c;

            // include (-1)^m for all the terms
            LegArrEx[3, 0] = -0.5 * s * (3 - 5 * s * s);
            LegArrEx[3, 1] = (3.0 / 2) * c * (-1 + 5 * s * s); // 15s^2 - 3
            LegArrEx[3, 2] = 15 * s * c * c;
            LegArrEx[3, 3] = 15 * (c * c * c);

            LegArrEx[4, 0] = 1.0 / 8.0 * (35.0 * Math.Pow(s, 4) - 30.0 * Math.Pow(s, 2) + 3.0);
            LegArrEx[4, 1] = 2.5 * c * (-3 * s + 7 * Math.Pow(s, 3));
            LegArrEx[4, 2] = 7.5 * Math.Pow(c, 2) * (-1 + 7 * Math.Pow(s, 2));
            LegArrEx[4, 3] = 105.0 * Math.Pow(c, 3) * s;
            LegArrEx[4, 4] = 105.0 * Math.Pow(c, 4);

            LegArrEx[5, 0] = (1.0 / 8) * s * (15 - 70 * Math.Pow(s, 2) + 63 * Math.Pow(s, 4));
            LegArrEx[5, 1] = (15.0 / 8) * c * (1 - 14 * Math.Pow(s, 2) + 21 * Math.Pow(s, 4));
            LegArrEx[5, 2] = (105.0 / 2) * c * c * (-s + 3 * Math.Pow(s, 3));
            LegArrEx[5, 3] = (105.0 / 2) * Math.Pow(c, 3) * (-1 + 9 * Math.Pow(s, 2));
            LegArrEx[5, 4] = 945.0 * s * Math.Pow(c, 4);
            LegArrEx[5, 5] = 945.0 * Math.Pow(c, 5);

            LegArrEx[6, 0] = 1.0 / 16 * (-5 + 105 * Math.Pow(s, 2) - 315 * Math.Pow(s, 4) + 231 * Math.Pow(s, 6));
            LegArrEx[6, 1] = (21.0 / 8) * c * (5 * s - 30 * Math.Pow(s, 3) + 33 * Math.Pow(s, 5));
            LegArrEx[6, 2] = (105.0 / 8) * c * c * (1 - 18 * Math.Pow(s, 2) + 33 * Math.Pow(s, 4));
            LegArrEx[6, 3] = (315.0 / 2) * Math.Pow(c, 3) * (-3 * s + 11 * Math.Pow(s, 3));
            LegArrEx[6, 4] = 945.0 / 2 * Math.Pow(c * c, 2) * (-1 + 11 * Math.Pow(s, 2));
            LegArrEx[6, 5] = 10395.0 * s * Math.Pow(c, 5);
            LegArrEx[6, 6] = 10395.0 * Math.Pow(c * c, 3);

            LegArrEx[7, 0] = 1.0 / 16 * (-35 * s + 315 * Math.Pow(s, 3) - 693 * Math.Pow(s, 5) + 429 * Math.Pow(s, 7));
            LegArrEx[7, 1] = (7.0 / 16) * c * (-5 + 135 * Math.Pow(s, 2) - 495 * Math.Pow(s, 4) + 429 * Math.Pow(s, 6));
            LegArrEx[7, 2] = (63.0 / 8) * c * c * (15 * s - 110 * Math.Pow(s, 3) + 143 * Math.Pow(s, 5));
            LegArrEx[7, 3] = (315.0 / 8) * Math.Pow(c, 3) * (3 - 66 * Math.Pow(s, 2) + 143 * Math.Pow(s, 4));
            LegArrEx[7, 4] = 3465.0 / 2 * Math.Pow(c * c, 2) * (-3 * s + 13 * Math.Pow(s, 3));
            LegArrEx[7, 5] = (10395.0 / 2) * Math.Pow(c, 5) * (-1 + 13 * Math.Pow(s, 2));
            LegArrEx[7, 6] = 135135.0 * s * Math.Pow(c * c, 3);
            LegArrEx[7, 7] = 135135.0 * Math.Pow(c, 7);

            LegArrEx[8, 0] = 1.0 / 128 * (35 - 1260 * Math.Pow(s, 2) + 6930 * Math.Pow(s, 4) - 12012 * Math.Pow(s, 6) + 6435 * Math.Pow(s, 8));
            LegArrEx[8, 1] = (9.0 / 16) * c * (-35 * s + 385 * Math.Pow(s, 3) - 1001 * Math.Pow(s, 5) + 715 * Math.Pow(s, 7));
            LegArrEx[8, 2] = (315.0 / 16) * c * c * (-1 + 33 * Math.Pow(s, 2) - 143 * Math.Pow(s, 4) + 143 * Math.Pow(s, 6));
            LegArrEx[8, 3] = (3465.0 / 8) * Math.Pow(c, 3) * (3 * s - 26 * Math.Pow(s, 3) + 39 * Math.Pow(s, 5));
            LegArrEx[8, 4] = 10395.0 / 8 * Math.Pow(c * c, 2) * (1 - 26 * Math.Pow(s, 2) + 65 * Math.Pow(s, 4));
            LegArrEx[8, 5] = (135135.0 / 2) * Math.Pow(c, 5) * (-s + 5 * Math.Pow(s, 3));
            LegArrEx[8, 6] = (135135.0 / 2) * Math.Pow(c * c, 3) * (-1 + 15 * Math.Pow(s, 2));
            LegArrEx[8, 7] = 2027025.0 * s * Math.Pow(c, 7);
            LegArrEx[8, 8] = 2027025.0 * Math.Pow(c * c, 4);

            LegArrEx[9, 0] = 1.0 / 128 * (315 * s - 4620 * Math.Pow(s, 3) + 18018 * Math.Pow(s, 5) - 25740 * Math.Pow(s, 7) + 12155 * Math.Pow(s, 9));
            LegArrEx[9, 1] = (45.0 / 128) * c * (7 - 308 * Math.Pow(s, 2) + 2002 * Math.Pow(s, 4) - 4004 * Math.Pow(s, 6) + 2431 * Math.Pow(s, 8));
            LegArrEx[9, 2] = (495.0 / 16) * c * c * (-7 * s + 91 * Math.Pow(s, 3) - 273 * Math.Pow(s, 5) + 221 * Math.Pow(s, 7));
            LegArrEx[9, 3] = (3465.0 / 16) * Math.Pow(c, 3) * (-1 + 39 * Math.Pow(s, 2) - 195 * Math.Pow(s, 4) + 221 * Math.Pow(s, 6));
            LegArrEx[9, 4] = 135135.0 / 8 * Math.Pow(c * c, 2) * (s - 10 * Math.Pow(s, 3) + 17 * Math.Pow(s, 5));
            LegArrEx[9, 5] = (135135.0 / 8) * Math.Pow(c, 5) * (1 - 30 * Math.Pow(s, 2) + 85 * Math.Pow(s, 4));
            LegArrEx[9, 6] = (675675.0 / 2) * Math.Pow(c * c, 3) * (-3 * s + 17 * Math.Pow(s, 3));
            LegArrEx[9, 7] = (2027025.0 / 2) * Math.Pow(c, 7) * (-1 + 17 * Math.Pow(s, 2));
            LegArrEx[9, 8] = 34459425.0 * s * Math.Pow(c * c, 4);
            LegArrEx[9, 9] = 34459425.0 * Math.Pow(c, 9);

            LegArrEx[10, 0] = 1.0 / 256 * (-63 + 3465 * Math.Pow(s, 2) - 30030 * Math.Pow(s, 4) + 90090 * Math.Pow(s, 6) - 109395 * Math.Pow(s, 8) + 46189 * Math.Pow(s, 10));
            LegArrEx[10, 1] = (55.0 / 128) * c * (63 * s - 1092 * Math.Pow(s, 3) + 4914 * Math.Pow(s, 5) - 7956 * Math.Pow(s, 7) + 4199 * Math.Pow(s, 9));
            LegArrEx[10, 2] = (495.0 / 128) * c * c * (7 - 364 * Math.Pow(s, 2) + 2730 * Math.Pow(s, 4) - 6188 * Math.Pow(s, 6) + 4199 * Math.Pow(s, 8));
            LegArrEx[10, 3] = (6435.0 / 16) * Math.Pow(c, 3) * (-7 * s + 105 * Math.Pow(s, 3) - 357 * Math.Pow(s, 5) + 323 * Math.Pow(s, 7));
            LegArrEx[10, 4] = 45045.0 / 16 * Math.Pow(c * c, 2) * (-1 + 45 * Math.Pow(s, 2) - 255 * Math.Pow(s, 4) + 323 * Math.Pow(s, 6));
            LegArrEx[10, 5] = (135135.0 / 8) * Math.Pow(c, 5) * (15 * s - 170 * Math.Pow(s, 3) + 323 * Math.Pow(s, 5));
            LegArrEx[10, 6] = (675675.0 / 8) * Math.Pow(c * c, 3) * (3 - 102 * Math.Pow(s, 2) + 323 * Math.Pow(s, 4));
            LegArrEx[10, 7] = (11486475.0 / 2) * Math.Pow(c, 7) * (-3 * s + 19 * Math.Pow(s, 3));
            LegArrEx[10, 8] = 34459425.0 / 2 * Math.Pow(c * c, 4) * (-1 + 19 * Math.Pow(s, 2));
            LegArrEx[10, 9] = 654729075.0 * s * Math.Pow(c, 9);
            LegArrEx[10, 10] = 654729075.0 * Math.Pow(c * c, 5);

            LegArrEx[11, 0] = 1.0 / 256 * (-693 * s + 15015 * Math.Pow(s, 3) - 90090 * Math.Pow(s, 5) + 218790 * Math.Pow(s, 7) - 230945 * Math.Pow(s, 9) + 88179 * Math.Pow(s, 11));
            LegArrEx[11, 1] = (33.0 / 256) * c * (-21 + 1365 * Math.Pow(s, 2) - 13650 * Math.Pow(s, 4) + 46410 * Math.Pow(s, 6) - 62985 * Math.Pow(s, 8) + 29393 * Math.Pow(s, 10));
            LegArrEx[11, 2] = (2145.0 / 128) * c * c * (21 * s - 420 * Math.Pow(s, 3) + 2142 * Math.Pow(s, 5) - 3876 * Math.Pow(s, 7) + 2261 * Math.Pow(s, 9));
            LegArrEx[11, 3] = (45045.0 / 128) * Math.Pow(c, 3) * (1 - 60 * Math.Pow(s, 2) + 510 * Math.Pow(s, 4) - 1292 * Math.Pow(s, 6) + 969 * Math.Pow(s, 8));
            LegArrEx[11, 4] = 135135.0 / 16 * Math.Pow(c * c, 2) * (-5 * s + 85 * Math.Pow(s, 3) - 323 * Math.Pow(s, 5) + 323 * Math.Pow(s, 7));
            LegArrEx[11, 5] = (135135.0 / 16) * Math.Pow(c, 5) * (-5 + 255 * Math.Pow(s, 2) - 1615 * Math.Pow(s, 4) + 2261 * Math.Pow(s, 6));
            LegArrEx[11, 6] = (2297295.0 / 8) * Math.Pow(c * c, 3) * (15 * s - 190 * Math.Pow(s, 3) + 399 * Math.Pow(s, 5));
            LegArrEx[11, 7] = (34459425.0 / 8) * Math.Pow(c, 7) * (1 - 38 * Math.Pow(s, 2) + 133 * Math.Pow(s, 4));
            LegArrEx[11, 8] = 654729075.0 / 2 * Math.Pow(c * c, 4) * (-s + 7 * Math.Pow(s, 3));
            LegArrEx[11, 9] = (654729075.0 / 2) * Math.Pow(c, 9) * (-1 + 21 * Math.Pow(s, 2));
            LegArrEx[11, 10] = 13749310575.0 * s * Math.Pow(c * c, 5);
            LegArrEx[11, 11] = 13749310575.0 * Math.Pow(c, 11);

            LegArrEx[12, 0] = (231.0 - 18018 * Math.Pow(s, 2) + 225225 * Math.Pow(s, 4) - 1021020 * Math.Pow(s, 6) + 2078505 * Math.Pow(s, 8) - 1939938 * Math.Pow(s, 10) + 676039 * Math.Pow(s, 12)) / 1024;
            LegArrEx[12, 1] = (39.0 / 256) * c * (-231 * s + 5775 * Math.Pow(s, 3) - 39270 * Math.Pow(s, 5) + 106590 * Math.Pow(s, 7) - 124355 * Math.Pow(s, 9) + 52003 * Math.Pow(s, 11));
            LegArrEx[12, 2] = (3003.0 / 256) * c * c * (-3 + 225 * Math.Pow(s, 2) - 2550 * Math.Pow(s, 4) + 9690 * Math.Pow(s, 6) - 14535 * Math.Pow(s, 8) + 7429 * Math.Pow(s, 10));
            LegArrEx[12, 3] = (15015.0 / 128) * Math.Pow(c, 3) * (45 * s - 1020 * Math.Pow(s, 3) + 5814 * Math.Pow(s, 5) - 11628 * Math.Pow(s, 7) + 7429 * Math.Pow(s, 9));
            LegArrEx[12, 4] = 135135.0 / 128 * Math.Pow(c * c, 2) * (5 - 340 * Math.Pow(s, 2) + 3230 * Math.Pow(s, 4) - 9044 * Math.Pow(s, 6) + 7429 * Math.Pow(s, 8));
            LegArrEx[12, 5] = (2297295.0 / 16) * Math.Pow(c, 5) * (-5 * s + 95 * Math.Pow(s, 3) - 399 * Math.Pow(s, 5) + 437 * Math.Pow(s, 7));
            LegArrEx[12, 6] = (2297295.0 / 16) * Math.Pow(c * c, 3) * (-5 + 285 * Math.Pow(s, 2) - 1995 * Math.Pow(s, 4) + 3059 * Math.Pow(s, 6));
            LegArrEx[12, 7] = (130945815.0 / 8) * Math.Pow(c, 7) * (5 * s - 70 * Math.Pow(s, 3) + 161 * Math.Pow(s, 5));
            LegArrEx[12, 8] = 654729075.0 / 8 * Math.Pow(c * c, 4) * (1 - 42 * Math.Pow(s, 2) + 161 * Math.Pow(s, 4));
            LegArrEx[12, 9] = (4583103525.0 / 2) * Math.Pow(c, 9) * (-3 * s + 23 * Math.Pow(s, 3));
            LegArrEx[12, 10] = (13749310575.0 / 2) * Math.Pow(c * c, 5) * (-1 + 23 * Math.Pow(s, 2));
            LegArrEx[12, 11] = 316234143225.0 * s * Math.Pow(c, 11);
            LegArrEx[12, 12] = 316234143225.0 * Math.Pow(c * c, 6);

            LegArrEx[13, 0] = (3003.0 * s - 90090 * Math.Pow(s, 3) + 765765 * Math.Pow(s, 5) - 2771340 * Math.Pow(s, 7) + 4849845 * Math.Pow(s, 9) - 4056234 * Math.Pow(s, 11) + 1300075 * Math.Pow(s, 13)) / 1024;
            LegArrEx[13, 1] = ((91.0 * c * (33 - 2970 * Math.Pow(s, 2) + 42075 * Math.Pow(s, 4) - 213180 * Math.Pow(s, 6) + 479655 * Math.Pow(s, 8) - 490314 * Math.Pow(s, 10) + 185725 * Math.Pow(s, 12)) / 1024));
            LegArrEx[13, 2] = (1365.0 / 256) * c * c * (-99 * s + 2805 * Math.Pow(s, 3) - 21318 * Math.Pow(s, 5) + 63954 * Math.Pow(s, 7) - 81719 * Math.Pow(s, 9) + 37145 * Math.Pow(s, 11));
            LegArrEx[13, 3] = (15015.0 / 256) * Math.Pow(c, 3) * (-9 + 765 * Math.Pow(s, 2) - 9690 * Math.Pow(s, 4) + 40698 * Math.Pow(s, 6) - 66861 * Math.Pow(s, 8) + 37145 * Math.Pow(s, 10));
            LegArrEx[13, 4] = 255255.0 / 128 * Math.Pow(c * c, 2) * (45 * s - 1140 * Math.Pow(s, 3) + 7182 * Math.Pow(s, 5) - 15732 * Math.Pow(s, 7) + 10925 * Math.Pow(s, 9));
            LegArrEx[13, 5] = (2297295.0 / 128) * Math.Pow(c, 5) * (5 - 380 * Math.Pow(s, 2) + 3990 * Math.Pow(s, 4) - 12236 * Math.Pow(s, 6) + 10925 * Math.Pow(s, 8));
            LegArrEx[13, 6] = (43648605.0 / 16) * Math.Pow(c * c, 3) * (-5 * s + 105 * Math.Pow(s, 3) - 483 * Math.Pow(s, 5) + 575 * Math.Pow(s, 7));
            LegArrEx[13, 7] = (218243025.0 / 16) * Math.Pow(c, 7) * (-1 + 63 * Math.Pow(s, 2) - 483 * Math.Pow(s, 4) + 805 * Math.Pow(s, 6));
            LegArrEx[13, 8] = 4583103525.0 / 8 * Math.Pow(c * c, 4) * (3 * s - 46 * Math.Pow(s, 3) + 115 * Math.Pow(s, 5));
            LegArrEx[13, 9] = (4583103525.0 / 8) * Math.Pow(c, 9) * (3 - 138 * Math.Pow(s, 2) + 575 * Math.Pow(s, 4));
            LegArrEx[13, 10] = (105411381075.0 / 2) * Math.Pow(c * c, 5) * (-3 * s + 25 * Math.Pow(s, 3));
            LegArrEx[13, 11] = (316234143225.0 / 2) * Math.Pow(c, 11) * (-1 + 25 * Math.Pow(s, 2));
            LegArrEx[13, 12] = 7905853580625.0 * s * Math.Pow(c * c, 6);
            LegArrEx[13, 13] = 7905853580625.0 * Math.Pow(c, 13);

            LegArrEx[14, 0] = (-429.0 + 45045 * Math.Pow(s, 2) - 765765 * Math.Pow(s, 4) + 4849845 * Math.Pow(s, 6) - 14549535 * Math.Pow(s, 8) + 22309287 * Math.Pow(s, 10) - 16900975 * Math.Pow(s, 12) + 5014575 * Math.Pow(s, 14)) / 2048;
            LegArrEx[14, 1] = ((105.0 * c * (429 * s - 14586 * Math.Pow(s, 3) + 138567 * Math.Pow(s, 5) - 554268 * Math.Pow(s, 7) + 1062347 * Math.Pow(s, 9) - 965770 * Math.Pow(s, 11) + 334305 * Math.Pow(s, 13)) / 1024));
            LegArrEx[14, 2] = ((1365.0 * c * c * (33 - 3366 * Math.Pow(s, 2) + 53295 * Math.Pow(s, 4) - 298452 * Math.Pow(s, 6) + 735471 * Math.Pow(s, 8) - 817190 * Math.Pow(s, 10) + 334305 * Math.Pow(s, 12)) / 1024));
            LegArrEx[14, 3] = (23205.0 / 256) * Math.Pow(c, 3) * (-99 * s + 3135 * Math.Pow(s, 3) - 26334 * Math.Pow(s, 5) + 86526 * Math.Pow(s, 7) - 120175 * Math.Pow(s, 9) + 58995 * Math.Pow(s, 11));
            LegArrEx[14, 4] = 2297295.0 / 256 * Math.Pow(c * c, 2) * (-1 + 95 * Math.Pow(s, 2) - 1330 * Math.Pow(s, 4) + 6118 * Math.Pow(s, 6) - 10925 * Math.Pow(s, 8) + 6555 * Math.Pow(s, 10));
            LegArrEx[14, 5] = (43648605.0 / 128) * Math.Pow(c, 5) * (5 * s - 140 * Math.Pow(s, 3) + 966 * Math.Pow(s, 5) - 2300 * Math.Pow(s, 7) + 1725 * Math.Pow(s, 9));
            LegArrEx[14, 6] = (218243025.0 / 128) * Math.Pow(c * c, 3) * (1 - 84 * Math.Pow(s, 2) + 966 * Math.Pow(s, 4) - 3220 * Math.Pow(s, 6) + 3105 * Math.Pow(s, 8));
            LegArrEx[14, 7] = (654729075.0 / 16) * Math.Pow(c, 7) * (-7 * s + 161 * Math.Pow(s, 3) - 805 * Math.Pow(s, 5) + 1035 * Math.Pow(s, 7));
            LegArrEx[14, 8] = 4583103525.0 / 16 * Math.Pow(c * c, 4) * (-1 + 69 * Math.Pow(s, 2) - 575 * Math.Pow(s, 4) + 1035 * Math.Pow(s, 6));
            LegArrEx[14, 9] = (105411381075.0 / 8) * Math.Pow(c, 9) * (3 * s - 50 * Math.Pow(s, 3) + 135 * Math.Pow(s, 5));
            LegArrEx[14, 10] = (316234143225.0 / 8) * Math.Pow(c * c, 5) * (1 - 50 * Math.Pow(s, 2) + 225 * Math.Pow(s, 4));
            LegArrEx[14, 11] = (7905853580625.0 / 2) * Math.Pow(c, 11) * (-s + 9 * Math.Pow(s, 3));
            LegArrEx[14, 12] = 7905853580625.0 / 2 * Math.Pow(c * c, 6) * (-1 + 27 * Math.Pow(s, 2));
            LegArrEx[14, 13] = 213458046676875.0 * s * Math.Pow(c, 13);
            LegArrEx[14, 14] = 213458046676875.0 * Math.Pow(c * c, 7);

            LegArrEx[15, 0] = (-6435.0 * s + 255255 * Math.Pow(s, 3) - 2909907 * Math.Pow(s, 5) + 14549535 * Math.Pow(s, 7) - 37182145 * Math.Pow(s, 9) + 50702925 * Math.Pow(s, 11) - 35102025 * Math.Pow(s, 13) + 9694845 * Math.Pow(s, 15)) / 2048;
            LegArrEx[15, 1] = ((15.0 * c * (-429 + 51051 * Math.Pow(s, 2) - 969969 * Math.Pow(s, 4) + 6789783 * Math.Pow(s, 6) - 22309287 * Math.Pow(s, 8) + 37182145 * Math.Pow(s, 10) - 30421755 * Math.Pow(s, 12) + 9694845 * Math.Pow(s, 14)) / 2048));
            LegArrEx[15, 2] = ((1785.0 * c * c * (429 * s - 16302 * Math.Pow(s, 3) + 171171 * Math.Pow(s, 5) - 749892 * Math.Pow(s, 7) + 1562275 * Math.Pow(s, 9) - 1533870 * Math.Pow(s, 11) + 570285 * Math.Pow(s, 13)) / 1024));
            LegArrEx[15, 3] = ((69615.0 * Math.Pow(c, 3) * (11 - 1254 * Math.Pow(s, 2) + 21945 * Math.Pow(s, 4) - 134596 * Math.Pow(s, 6) + 360525 * Math.Pow(s, 8) - 432630 * Math.Pow(s, 10) + 190095 * Math.Pow(s, 12)) / 1024));
            LegArrEx[15, 4] = 3968055.0 / 256 * Math.Pow(c * c, 2) * (-11 * s + 385 * Math.Pow(s, 3) - 3542 * Math.Pow(s, 5) + 12650 * Math.Pow(s, 7) - 18975 * Math.Pow(s, 9) + 10005 * Math.Pow(s, 11));
            LegArrEx[15, 5] = (43648605.0 / 256) * Math.Pow(c, 5) * (-1 + 105 * Math.Pow(s, 2) - 1610 * Math.Pow(s, 4) + 8050 * Math.Pow(s, 6) - 15525 * Math.Pow(s, 8) + 10005 * Math.Pow(s, 10));
            LegArrEx[15, 6] = (218243025.0 / 128) * Math.Pow(c * c, 3) * (21 * s - 644 * Math.Pow(s, 3) + 4830 * Math.Pow(s, 5) - 12420 * Math.Pow(s, 7) + 10005 * Math.Pow(s, 9));
            LegArrEx[15, 7] = (654729075.0 / 128) * Math.Pow(c, 7) * (7 - 644 * Math.Pow(s, 2) + 8050 * Math.Pow(s, 4) - 28980 * Math.Pow(s, 6) + 30015 * Math.Pow(s, 8));
            LegArrEx[15, 8] = 15058768725.0 / 16 * Math.Pow(c * c, 4) * (-7 * s + 175 * Math.Pow(s, 3) - 945 * Math.Pow(s, 5) + 1305 * Math.Pow(s, 7));
            LegArrEx[15, 9] = (105411381075.0 / 16) * Math.Pow(c, 9) * (-1 + 75 * Math.Pow(s, 2) - 675 * Math.Pow(s, 4) + 1305 * Math.Pow(s, 6));
            LegArrEx[15, 10] = (1581170716125.0 / 8) * Math.Pow(c * c, 5) * (5 * s - 90 * Math.Pow(s, 3) + 261 * Math.Pow(s, 5));
            LegArrEx[15, 11] = (7905853580625.0 / 8) * Math.Pow(c, 11) * (1 - 54 * Math.Pow(s, 2) + 261 * Math.Pow(s, 4));
            LegArrEx[15, 12] = 71152682225625.0 / 2 * Math.Pow(c * c, 6) * (-3 * s + 29 * Math.Pow(s, 3));
            LegArrEx[15, 13] = (213458046676875.0 / 2) * Math.Pow(c, 13) * (-1 + 29 * Math.Pow(s, 2));
            LegArrEx[15, 14] = 6190283353629375.0 * s * Math.Pow(c * c, 7);
            LegArrEx[15, 15] = 6190283353629375.0 * Math.Pow(c, 15);

            LegArrEx[16, 0] = (6435.0 - 875160 * Math.Pow(s, 2) + 19399380 * Math.Pow(s, 4) - 162954792 * Math.Pow(s, 6) + 669278610 * Math.Pow(s, 8) - 1487285800 * Math.Pow(s, 10) + 1825305300 * Math.Pow(s, 12) - 1163381400 * Math.Pow(s, 14) + 300540195 * Math.Pow(s, 16)) / 32768;
            LegArrEx[16, 1] = ((17.0 * c * (-6435 * s + 285285 * Math.Pow(s, 3) - 3594591 * Math.Pow(s, 5) + 19684665 * Math.Pow(s, 7) - 54679625 * Math.Pow(s, 9) + 80528175 * Math.Pow(s, 11) - 59879925 * Math.Pow(s, 13) + 17678835 * Math.Pow(s, 15)) / 2048));
            LegArrEx[16, 2] = ((765.0 * c * c * (-143 + 19019 * Math.Pow(s, 2) - 399399 * Math.Pow(s, 4) + 3062059 * Math.Pow(s, 6) - 10935925 * Math.Pow(s, 8) + 19684665 * Math.Pow(s, 10) - 17298645 * Math.Pow(s, 12) + 5892945 * Math.Pow(s, 14)) / 2048));
            LegArrEx[16, 3] = ((101745.0 * Math.Pow(c, 3) * (143 * s - 6006 * Math.Pow(s, 3) + 69069 * Math.Pow(s, 5) - 328900 * Math.Pow(s, 7) + 740025 * Math.Pow(s, 9) - 780390 * Math.Pow(s, 11) + 310155 * Math.Pow(s, 13)) / 1024));
            LegArrEx[16, 4] = (1322685.0 * Math.Pow(c * c, 2) * (11 - 1386 * Math.Pow(s, 2) + 26565 * Math.Pow(s, 4) - 177100 * Math.Pow(s, 6) + 512325 * Math.Pow(s, 8) - 660330 * Math.Pow(s, 10) + 310155 * Math.Pow(s, 12)) / 1024);
            LegArrEx[16, 5] = (3968055.0 / 256) * Math.Pow(c, 5) * (-231 * s + 8855 * Math.Pow(s, 3) - 88550 * Math.Pow(s, 5) + 341550 * Math.Pow(s, 7) - 550275 * Math.Pow(s, 9) + 310155 * Math.Pow(s, 11));
            LegArrEx[16, 6] = (43648605.0 / 256) * Math.Pow(c * c, 3) * (-21 + 2415 * Math.Pow(s, 2) - 40250 * Math.Pow(s, 4) + 217350 * Math.Pow(s, 6) - 450225 * Math.Pow(s, 8) + 310155 * Math.Pow(s, 10));
            LegArrEx[16, 7] = (5019589575.0 / 128) * Math.Pow(c, 7) * (21 * s - 700 * Math.Pow(s, 3) + 5670 * Math.Pow(s, 5) - 15660 * Math.Pow(s, 7) + 13485 * Math.Pow(s, 9));
            LegArrEx[16, 8] = 15058768725.0 / 128 * Math.Pow(c * c, 4) * (7 - 700 * Math.Pow(s, 2) + 9450 * Math.Pow(s, 4) - 36540 * Math.Pow(s, 6) + 40455 * Math.Pow(s, 8));
            LegArrEx[16, 9] = (75293843625.0 / 16) * Math.Pow(c, 9) * (-35 * s + 945 * Math.Pow(s, 3) - 5481 * Math.Pow(s, 5) + 8091 * Math.Pow(s, 7));
            LegArrEx[16, 10] = (527056905375.0 / 16) * Math.Pow(c * c, 5) * (-5 + 405 * Math.Pow(s, 2) - 3915 * Math.Pow(s, 4) + 8091 * Math.Pow(s, 6));
            LegArrEx[16, 11] = (14230536445125.0 / 8) * Math.Pow(c, 11) * (15 * s - 290 * Math.Pow(s, 3) + 899 * Math.Pow(s, 5));
            LegArrEx[16, 12] = 71152682225625.0 / 8 * Math.Pow(c * c, 6) * (3 - 174 * Math.Pow(s, 2) + 899 * Math.Pow(s, 4));
            LegArrEx[16, 13] = (2063427784543125.0 / 2) * Math.Pow(c, 13) * (-3 * s + 31 * Math.Pow(s, 3));
            LegArrEx[16, 14] = (6190283353629375.0 / 2) * Math.Pow(c * c, 7) * (-1 + 31 * Math.Pow(s, 2));
            LegArrEx[16, 15] = 191898783962510625.0 * s * Math.Pow(c, 15);
            LegArrEx[16, 16] = 191898783962510625.0 * Math.Pow(c * c, 8);

            LegArrEx[17, 0] = (109395.0 * s - 5542680 * Math.Pow(s, 3) + 81477396 * Math.Pow(s, 5) - 535422888 * Math.Pow(s, 7) + 1859107250 * Math.Pow(s, 9) - 3650610600 * Math.Pow(s, 11) + 4071834900 * Math.Pow(s, 13) - 2404321560 * Math.Pow(s, 15) + 583401555 * Math.Pow(s, 17)) / 32768;
            LegArrEx[17, 1] = ((153.0 * c * (715 - 108680 * Math.Pow(s, 2) + 2662660 * Math.Pow(s, 4) - 24496472 * Math.Pow(s, 6) + 109359250 * Math.Pow(s, 8) - 262462200 * Math.Pow(s, 10) + 345972900 * Math.Pow(s, 12) - 235717800 * Math.Pow(s, 14) + 64822395 * Math.Pow(s, 16)) / 32768));
            LegArrEx[17, 2] = ((2907.0 * c * c * (-715 * s + 35035 * Math.Pow(s, 3) - 483483 * Math.Pow(s, 5) + 2877875 * Math.Pow(s, 7) - 8633625 * Math.Pow(s, 9) + 13656825 * Math.Pow(s, 11) - 10855425 * Math.Pow(s, 13) + 3411705 * Math.Pow(s, 15)) / 2048));
            LegArrEx[17, 3] = ((14535.0 * Math.Pow(c, 3) * (-143 + 21021 * Math.Pow(s, 2) - 483483 * Math.Pow(s, 4) + 4029025 * Math.Pow(s, 6) - 15540525 * Math.Pow(s, 8) + 30045015 * Math.Pow(s, 10) - 28224105 * Math.Pow(s, 12) + 10235115 * Math.Pow(s, 14)) / 2048));
            LegArrEx[17, 4] = (305235.0 * Math.Pow(c * c, 2) * (1001 * s - 46046 * Math.Pow(s, 3) + 575575 * Math.Pow(s, 5) - 2960100 * Math.Pow(s, 7) + 7153575 * Math.Pow(s, 9) - 8064030 * Math.Pow(s, 11) + 3411705 * Math.Pow(s, 13)) / 1024);
            LegArrEx[17, 5] = ((43648605.0 * Math.Pow(c, 5) * (7 - 966 * Math.Pow(s, 2) + 20125 * Math.Pow(s, 4) - 144900 * Math.Pow(s, 6) + 450225 * Math.Pow(s, 8) - 620310 * Math.Pow(s, 10) + 310155 * Math.Pow(s, 12)) / 1024));
            LegArrEx[17, 6] = (1003917915.0 / 256) * Math.Pow(c * c, 3) * (-21 * s + 875 * Math.Pow(s, 3) - 9450 * Math.Pow(s, 5) + 39150 * Math.Pow(s, 7) - 67425 * Math.Pow(s, 9) + 40455 * Math.Pow(s, 11));
            LegArrEx[17, 7] = (3011753745.0 / 256) * Math.Pow(c, 7) * (-7 + 875 * Math.Pow(s, 2) - 15750 * Math.Pow(s, 4) + 91350 * Math.Pow(s, 6) - 202275 * Math.Pow(s, 8) + 148335 * Math.Pow(s, 10));
            LegArrEx[17, 8] = 75293843625.0 / 128 * Math.Pow(c * c, 4) * (35 * s - 1260 * Math.Pow(s, 3) + 10962 * Math.Pow(s, 5) - 32364 * Math.Pow(s, 7) + 29667 * Math.Pow(s, 9));
            LegArrEx[17, 9] = (75293843625.0 / 128) * Math.Pow(c, 9) * (35 - 3780 * Math.Pow(s, 2) + 54810 * Math.Pow(s, 4) - 226548 * Math.Pow(s, 6) + 267003 * Math.Pow(s, 8));
            LegArrEx[17, 10] = (2032933777875.0 / 16) * Math.Pow(c * c, 5) * (-35 * s + 1015 * Math.Pow(s, 3) - 6293 * Math.Pow(s, 5) + 9889 * Math.Pow(s, 7));
            LegArrEx[17, 11] = (14230536445125.0 / 16) * Math.Pow(c, 11) * (-5 + 435 * Math.Pow(s, 2) - 4495 * Math.Pow(s, 4) + 9889 * Math.Pow(s, 6));
            LegArrEx[17, 12] = 412685556908625.0 / 8 * Math.Pow(c * c, 6) * (15 * s - 310 * Math.Pow(s, 3) + 1023 * Math.Pow(s, 5));
            LegArrEx[17, 13] = (6190283353629375.0 / 8) * Math.Pow(c, 13) * (1 - 62 * Math.Pow(s, 2) + 341 * Math.Pow(s, 4));
            LegArrEx[17, 14] = (191898783962510625.0 / 2) * Math.Pow(c * c, 7) * (-s + 11 * Math.Pow(s, 3));
            LegArrEx[17, 15] = (191898783962510625.0 / 2) * Math.Pow(c, 15) * (-1 + 33 * Math.Pow(s, 2));
            LegArrEx[17, 16] = 6332659870762850625.0 * s * Math.Pow(c * c, 8);
            LegArrEx[17, 17] = 6332659870762850625.0 * Math.Pow(c, 17);

            LegArrEx[18, 0] = (-12155.0 + 2078505 * Math.Pow(s, 2) - 58198140 * Math.Pow(s, 4) + 624660036 * Math.Pow(s, 6) - 3346393050 * Math.Pow(s, 8) + 10039179150 * Math.Pow(s, 10) - 17644617900 * Math.Pow(s, 12) + 18032411700 * Math.Pow(s, 14) - 9917826435 * Math.Pow(s, 16) + 2268783825 * Math.Pow(s, 18)) / 65536;
            LegArrEx[18, 1] = ((171.0 * c * (12155 * s - 680680 * Math.Pow(s, 3) + 10958948 * Math.Pow(s, 5) - 78278200 * Math.Pow(s, 7) + 293543250 * Math.Pow(s, 9) - 619109400 * Math.Pow(s, 11) + 738168900 * Math.Pow(s, 13) - 463991880 * Math.Pow(s, 15) + 119409675 * Math.Pow(s, 17)) / 32768));
            LegArrEx[18, 2] = ((14535.0 * c * c * (143 - 24024 * Math.Pow(s, 2) + 644644 * Math.Pow(s, 4) - 6446440 * Math.Pow(s, 6) + 31081050 * Math.Pow(s, 8) - 80120040 * Math.Pow(s, 10) + 112896420 * Math.Pow(s, 12) - 81880920 * Math.Pow(s, 14) + 23881935 * Math.Pow(s, 16)) / 32768));
            LegArrEx[18, 3] = ((101745.0 * Math.Pow(c, 3) * (-429 * s + 23023 * Math.Pow(s, 3) - 345345 * Math.Pow(s, 5) + 2220075 * Math.Pow(s, 7) - 7153575 * Math.Pow(s, 9) + 12096045 * Math.Pow(s, 11) - 10235115 * Math.Pow(s, 13) + 3411705 * Math.Pow(s, 15)) / 2048));
            LegArrEx[18, 4] = (3357585.0 * Math.Pow(c * c, 2) * (-13 + 2093 * Math.Pow(s, 2) - 52325 * Math.Pow(s, 4) + 470925 * Math.Pow(s, 6) - 1950975 * Math.Pow(s, 8) + 4032015 * Math.Pow(s, 10) - 4032015 * Math.Pow(s, 12) + 1550775 * Math.Pow(s, 14)) / 2048);
            LegArrEx[18, 5] = ((77224455.0 * Math.Pow(c, 5) * (91 * s - 4550 * Math.Pow(s, 3) + 61425 * Math.Pow(s, 5) - 339300 * Math.Pow(s, 7) + 876525 * Math.Pow(s, 9) - 1051830 * Math.Pow(s, 11) + 471975 * Math.Pow(s, 13)) / 1024));
            LegArrEx[18, 6] = ((1003917915.0 * Math.Pow(c * c, 3) * (7 - 1050 * Math.Pow(s, 2) + 23625 * Math.Pow(s, 4) - 182700 * Math.Pow(s, 6) + 606825 * Math.Pow(s, 8) - 890010 * Math.Pow(s, 10) + 471975 * Math.Pow(s, 12)) / 1024));
            LegArrEx[18, 7] = (75293843625.0 / 256) * Math.Pow(c, 7) * (-7 * s + 315 * Math.Pow(s, 3) - 3654 * Math.Pow(s, 5) + 16182 * Math.Pow(s, 7) - 29667 * Math.Pow(s, 9) + 18879 * Math.Pow(s, 11));
            LegArrEx[18, 8] = 75293843625.0 / 256 * Math.Pow(c * c, 4) * (-7 + 945 * Math.Pow(s, 2) - 18270 * Math.Pow(s, 4) + 113274 * Math.Pow(s, 6) - 267003 * Math.Pow(s, 8) + 207669 * Math.Pow(s, 10));
            LegArrEx[18, 9] = (225881530875.0 / 128) * Math.Pow(c, 9) * (315 * s - 12180 * Math.Pow(s, 3) + 113274 * Math.Pow(s, 5) - 356004 * Math.Pow(s, 7) + 346115 * Math.Pow(s, 9));
            LegArrEx[18, 10] = (14230536445125.0 / 128) * Math.Pow(c * c, 5) * (5 - 580 * Math.Pow(s, 2) + 8990 * Math.Pow(s, 4) - 39556 * Math.Pow(s, 6) + 49445 * Math.Pow(s, 8));
            LegArrEx[18, 11] = (412685556908625.0 / 16) * Math.Pow(c, 11) * (-5 * s + 155 * Math.Pow(s, 3) - 1023 * Math.Pow(s, 5) + 1705 * Math.Pow(s, 7));
            LegArrEx[18, 12] = 2063427784543125.0 / 16 * Math.Pow(c * c, 6) * (-1 + 93 * Math.Pow(s, 2) - 1023 * Math.Pow(s, 4) + 2387 * Math.Pow(s, 6));
            LegArrEx[18, 13] = (191898783962510625.0 / 8) * Math.Pow(c, 13) * (s - 22 * Math.Pow(s, 3) + 77 * Math.Pow(s, 5));
            LegArrEx[18, 14] = (191898783962510625.0 / 8) * Math.Pow(c * c, 7) * (1 - 66 * Math.Pow(s, 2) + 385 * Math.Pow(s, 4));
            LegArrEx[18, 15] = (2110886623587616875.0 / 2) * Math.Pow(c, 15) * (-3 * s + 35 * Math.Pow(s, 3));
            LegArrEx[18, 16] = 6332659870762850625.0 / 2 * Math.Pow(c * c, 8) * (-1 + 35 * Math.Pow(s, 2));
            LegArrEx[18, 17] = 221643095476699771875.0 * s * Math.Pow(c, 17);
            LegArrEx[18, 18] = 221643095476699771875.0 * Math.Pow(c * c, 9);

            LegArrEx[19, 0] = (-230945.0 * s + 14549535 * Math.Pow(s, 3) - 267711444 * Math.Pow(s, 5) + 2230928700 * Math.Pow(s, 7) - 10039179150 * Math.Pow(s, 9) + 26466926850 * Math.Pow(s, 11) - 42075627300 * Math.Pow(s, 13) + 39671305740 * Math.Pow(s, 15) - 20419054425 * Math.Pow(s, 17) + 4418157975 * Math.Pow(s, 19)) / 65536;
            LegArrEx[19, 1] = ((95.0 * c * (-2431 + 459459 * Math.Pow(s, 2) - 14090076 * Math.Pow(s, 4) + 164384220 * Math.Pow(s, 6) - 951080130 * Math.Pow(s, 8) + 3064591530 * Math.Pow(s, 10) - 5757717420 * Math.Pow(s, 12) + 6263890380 * Math.Pow(s, 14) - 3653936055 * Math.Pow(s, 16) + 883631595 * Math.Pow(s, 18)) / 65536));
            LegArrEx[19, 2] = ((5985.0 * c * c * (7293 * s - 447304 * Math.Pow(s, 3) + 7827820 * Math.Pow(s, 5) - 60386040 * Math.Pow(s, 7) + 243221550 * Math.Pow(s, 9) - 548354040 * Math.Pow(s, 11) + 695987820 * Math.Pow(s, 13) - 463991880 * Math.Pow(s, 15) + 126233085 * Math.Pow(s, 17)) / 32768));
            LegArrEx[19, 3] = ((1119195.0 * Math.Pow(c, 3) * (39 - 7176 * Math.Pow(s, 2) + 209300 * Math.Pow(s, 4) - 2260440 * Math.Pow(s, 6) + 11705850 * Math.Pow(s, 8) - 32256120 * Math.Pow(s, 10) + 48384180 * Math.Pow(s, 12) - 37218600 * Math.Pow(s, 14) + 11475735 * Math.Pow(s, 16)) / 32768));
            LegArrEx[19, 4] = (25741485.0 * Math.Pow(c * c, 2) * (-39 * s + 2275 * Math.Pow(s, 3) - 36855 * Math.Pow(s, 5) + 254475 * Math.Pow(s, 7) - 876525 * Math.Pow(s, 9) + 1577745 * Math.Pow(s, 11) - 1415925 * Math.Pow(s, 13) + 498945 * Math.Pow(s, 15)) / 2048);
            LegArrEx[19, 5] = ((77224455.0 * Math.Pow(c, 5) * (-13 + 2275 * Math.Pow(s, 2) - 61425 * Math.Pow(s, 4) + 593775 * Math.Pow(s, 6) - 2629575 * Math.Pow(s, 8) + 5785065 * Math.Pow(s, 10) - 6135675 * Math.Pow(s, 12) + 2494725 * Math.Pow(s, 14)) / 2048));
            LegArrEx[19, 6] = ((1930611375.0 * Math.Pow(c * c, 3) * (91 * s - 4914 * Math.Pow(s, 3) + 71253 * Math.Pow(s, 5) - 420732 * Math.Pow(s, 7) + 1157013 * Math.Pow(s, 9) - 1472562 * Math.Pow(s, 11) + 698523 * Math.Pow(s, 13)) / 1024));
            LegArrEx[19, 7] = ((25097947875.0 * Math.Pow(c, 7) * (7 - 1134 * Math.Pow(s, 2) + 27405 * Math.Pow(s, 4) - 226548 * Math.Pow(s, 6) + 801009 * Math.Pow(s, 8) - 1246014 * Math.Pow(s, 10) + 698523 * Math.Pow(s, 12)) / 1024));
            LegArrEx[19, 8] = 225881530875.0 / 256 * Math.Pow(c * c, 4) * (-63 * s + 3045 * Math.Pow(s, 3) - 37758 * Math.Pow(s, 5) + 178002 * Math.Pow(s, 7) - 346115 * Math.Pow(s, 9) + 232841 * Math.Pow(s, 11));
            LegArrEx[19, 9] = (1581170716125.0 / 256) * Math.Pow(c, 9) * (-9 + 1305 * Math.Pow(s, 2) - 26970 * Math.Pow(s, 4) + 178002 * Math.Pow(s, 6) - 445005 * Math.Pow(s, 8) + 365893 * Math.Pow(s, 10));
            LegArrEx[19, 10] = (45853950767625.0 / 128) * Math.Pow(c * c, 5) * (45 * s - 1860 * Math.Pow(s, 3) + 18414 * Math.Pow(s, 5) - 61380 * Math.Pow(s, 7) + 63085 * Math.Pow(s, 9));
            LegArrEx[19, 11] = (2063427784543125.0 / 128) * Math.Pow(c, 11) * (1 - 124 * Math.Pow(s, 2) + 2046 * Math.Pow(s, 4) - 9548 * Math.Pow(s, 6) + 12617 * Math.Pow(s, 8));
            LegArrEx[19, 12] = 63966261320836875.0 / 16 * Math.Pow(c * c, 6) * (-s + 33 * Math.Pow(s, 3) - 231 * Math.Pow(s, 5) + 407 * Math.Pow(s, 7));
            LegArrEx[19, 13] = (63966261320836875.0 / 16) * Math.Pow(c, 13) * (-1 + 99 * Math.Pow(s, 2) - 1155 * Math.Pow(s, 4) + 2849 * Math.Pow(s, 6));
            LegArrEx[19, 14] = (2110886623587616875.0 / 8) * Math.Pow(c * c, 7) * (3 * s - 70 * Math.Pow(s, 3) + 259 * Math.Pow(s, 5));
            LegArrEx[19, 15] = (2110886623587616875.0 / 8) * Math.Pow(c, 15) * (3 - 210 * Math.Pow(s, 2) + 1295 * Math.Pow(s, 4));
            LegArrEx[19, 16] = 73881031825566590625.0 / 2 * Math.Pow(c * c, 8) * (-3 * s + 37 * Math.Pow(s, 3));
            LegArrEx[19, 17] = (221643095476699771875.0 / 2) * Math.Pow(c, 17) * (-1 + 37 * Math.Pow(s, 2));
            LegArrEx[19, 18] = 8200794532637891559375.0 * s * Math.Pow(c * c, 9);
            LegArrEx[19, 19] = 8200794532637891559375.0 * Math.Pow(c, 19);

            LegArrEx[20, 0] = (1.0 / 262144) * (46189 - 9699690 * Math.Pow(s, 2) + 334639305 * Math.Pow(s, 4) - 4461857400 * Math.Pow(s, 6) + 30117537450 * Math.Pow(s, 8) - 116454478140 * Math.Pow(s, 10) + 273491577450 * Math.Pow(s, 12) - 396713057400 * Math.Pow(s, 14) + 347123925225 * Math.Pow(s, 16) - 167890003050 * Math.Pow(s, 18) + 34461632205 * Math.Pow(s, 20));
            LegArrEx[20, 1] = ((105.0 * c * (-46189 * s + 3187041 * Math.Pow(s, 3) - 63740820 * Math.Pow(s, 5) + 573667380 * Math.Pow(s, 7) - 2772725670 * Math.Pow(s, 9) + 7814045070 * Math.Pow(s, 11) - 13223768580 * Math.Pow(s, 13) + 13223768580 * Math.Pow(s, 15) - 7195285845 * Math.Pow(s, 17) + 1641030105 * Math.Pow(s, 19)) / 65536));
            LegArrEx[20, 2] = ((21945.0 * c * c * (-221 + 45747 * Math.Pow(s, 2) - 1524900 * Math.Pow(s, 4) + 19213740 * Math.Pow(s, 6) - 119399670 * Math.Pow(s, 8) + 411265530 * Math.Pow(s, 10) - 822531060 * Math.Pow(s, 12) + 949074300 * Math.Pow(s, 14) - 585262485 * Math.Pow(s, 16) + 149184555 * Math.Pow(s, 18)) / 65536));
            LegArrEx[20, 3] = ((1514205.0 * Math.Pow(c, 3) * (663 * s - 44200 * Math.Pow(s, 3) + 835380 * Math.Pow(s, 5) - 6921720 * Math.Pow(s, 7) + 29801850 * Math.Pow(s, 9) - 71524440 * Math.Pow(s, 11) + 96282900 * Math.Pow(s, 13) - 67856520 * Math.Pow(s, 15) + 19458855 * Math.Pow(s, 17)) / 32768));
            LegArrEx[20, 4] = (77224455.0 * Math.Pow(c * c, 2) * (13 - 2600 * Math.Pow(s, 2) + 81900 * Math.Pow(s, 4) - 950040 * Math.Pow(s, 6) + 5259150 * Math.Pow(s, 8) - 15426840 * Math.Pow(s, 10) + 24542700 * Math.Pow(s, 12) - 19957800 * Math.Pow(s, 14) + 6486285 * Math.Pow(s, 16)) / 32768);
            LegArrEx[20, 5] = ((386122275.0 * Math.Pow(c, 5) * (-65 * s + 4095 * Math.Pow(s, 3) - 71253 * Math.Pow(s, 5) + 525915 * Math.Pow(s, 7) - 1928355 * Math.Pow(s, 9) + 3681405 * Math.Pow(s, 11) - 3492615 * Math.Pow(s, 13) + 1297257 * Math.Pow(s, 15)) / 2048));
            LegArrEx[20, 6] = ((25097947875.0 * Math.Pow(c * c, 3) * (-1 + 189 * Math.Pow(s, 2) - 5481 * Math.Pow(s, 4) + 56637 * Math.Pow(s, 6) - 267003 * Math.Pow(s, 8) + 623007 * Math.Pow(s, 10) - 698523 * Math.Pow(s, 12) + 299367 * Math.Pow(s, 14)) / 2048));
            LegArrEx[20, 7] = ((225881530875.0 * Math.Pow(c, 7) * (21 * s - 1218 * Math.Pow(s, 3) + 18879 * Math.Pow(s, 5) - 118668 * Math.Pow(s, 7) + 346115 * Math.Pow(s, 9) - 465682 * Math.Pow(s, 11) + 232841 * Math.Pow(s, 13)) / 1024));
            LegArrEx[20, 8] = (1581170716125.0 * Math.Pow(c * c, 4) * (3 - 522 * Math.Pow(s, 2) + 13485 * Math.Pow(s, 4) - 118668 * Math.Pow(s, 6) + 445005 * Math.Pow(s, 8) - 731786 * Math.Pow(s, 10) + 432419 * Math.Pow(s, 12)) / 1024);
            LegArrEx[20, 9] = (45853950767625.0 / 256) * Math.Pow(c, 9) * (-9 * s + 465 * Math.Pow(s, 3) - 6138 * Math.Pow(s, 5) + 30690 * Math.Pow(s, 7) - 63085 * Math.Pow(s, 9) + 44733 * Math.Pow(s, 11));
            LegArrEx[20, 10] = (137561852302875.0 / 256) * Math.Pow(c * c, 5) * (-3 + 465 * Math.Pow(s, 2) - 10230 * Math.Pow(s, 4) + 71610 * Math.Pow(s, 6) - 189255 * Math.Pow(s, 8) + 164021 * Math.Pow(s, 10));
            LegArrEx[20, 11] = (21322087106945625.0 / 128) * Math.Pow(c, 11) * (3 * s - 132 * Math.Pow(s, 3) + 1386 * Math.Pow(s, 5) - 4884 * Math.Pow(s, 7) + 5291 * Math.Pow(s, 9));
            LegArrEx[20, 12] = 63966261320836875.0 / 128 * Math.Pow(c * c, 6) * (1 - 132 * Math.Pow(s, 2) + 2310 * Math.Pow(s, 4) - 11396 * Math.Pow(s, 6) + 15873 * Math.Pow(s, 8));
            LegArrEx[20, 13] = (2110886623587616875.0 / 16) * Math.Pow(c, 13) * (-s + 35 * Math.Pow(s, 3) - 259 * Math.Pow(s, 5) + 481 * Math.Pow(s, 7));
            LegArrEx[20, 14] = (2110886623587616875.0 / 16) * Math.Pow(c * c, 7) * (-1 + 105 * Math.Pow(s, 2) - 1295 * Math.Pow(s, 4) + 3367 * Math.Pow(s, 6));
            LegArrEx[20, 15] = (14776206365113318125.0 / 8) * Math.Pow(c, 15) * (15 * s - 370 * Math.Pow(s, 3) + 1443 * Math.Pow(s, 5));
            LegArrEx[20, 16] = 221643095476699771875.0 / 8 * Math.Pow(c * c, 8) * (1 - 74 * Math.Pow(s, 2) + 481 * Math.Pow(s, 4));
            LegArrEx[20, 17] = (8200794532637891559375.0 / 2) * Math.Pow(c, 17) * (-s + 13 * Math.Pow(s, 3));
            LegArrEx[20, 18] = (8200794532637891559375.0 / 2) * Math.Pow(c * c, 9) * (-1 + 39 * Math.Pow(s, 2));
            LegArrEx[20, 19] = 319830986772877770815625.0 * s * Math.Pow(c, 19);
            LegArrEx[20, 20] = 319830986772877770815625.0 * Math.Pow(c * c, 10);

            LegArrEx[21, 0] = (1.0 / 262144) * (969969 * s - 74364290 * Math.Pow(s, 3) + 1673196525 * Math.Pow(s, 5) - 17210021400 * Math.Pow(s, 7) + 97045398450 * Math.Pow(s, 9) - 328189892940 * Math.Pow(s, 11) + 694247850450 * Math.Pow(s, 13) - 925663800600 * Math.Pow(s, 15) + 755505013725 * Math.Pow(s, 17) - 344616322050 * Math.Pow(s, 19) + 67282234305 * Math.Pow(s, 21));
            LegArrEx[21, 1] = (1.0 / 262144) * 231 * c * (4199 - 965770 * Math.Pow(s, 2) + 36216375 * Math.Pow(s, 4) - 521515800 * Math.Pow(s, 6) + 3780989550 * Math.Pow(s, 8) - 15628090140 * Math.Pow(s, 10) + 39070225350 * Math.Pow(s, 12) - 60108039000 * Math.Pow(s, 14) + 55599936075 * Math.Pow(s, 16) - 28345065450 * Math.Pow(s, 18) + 6116566755 * Math.Pow(s, 20));
            LegArrEx[21, 2] = ((26565.0 * c * c * (-4199 * s + 314925 * Math.Pow(s, 3) - 6802380 * Math.Pow(s, 5) + 65756340 * Math.Pow(s, 7) - 339741090 * Math.Pow(s, 9) + 1019223270 * Math.Pow(s, 11) - 1829375100 * Math.Pow(s, 13) + 1933910820 * Math.Pow(s, 15) - 1109154735 * Math.Pow(s, 17) + 265937685 * Math.Pow(s, 19)) / 65536));
            LegArrEx[21, 3] = ((504735.0 * Math.Pow(c, 3) * (-221 + 49725 * Math.Pow(s, 2) - 1790100 * Math.Pow(s, 4) + 24226020 * Math.Pow(s, 6) - 160929990 * Math.Pow(s, 8) + 590076630 * Math.Pow(s, 10) - 1251677700 * Math.Pow(s, 12) + 1526771700 * Math.Pow(s, 14) - 992401605 * Math.Pow(s, 16) + 265937685 * Math.Pow(s, 18)) / 65536));
            LegArrEx[21, 4] = (22713075.0 * Math.Pow(c * c, 2) * (1105 * s - 79560 * Math.Pow(s, 3) + 1615068 * Math.Pow(s, 5) - 14304888 * Math.Pow(s, 7) + 65564070 * Math.Pow(s, 9) - 166890360 * Math.Pow(s, 11) + 237497820 * Math.Pow(s, 13) - 176426952 * Math.Pow(s, 15) + 53187537 * Math.Pow(s, 17)) / 32768);
            LegArrEx[21, 5] = ((5019589575.0 * Math.Pow(c, 5) * (5 - 1080 * Math.Pow(s, 2) + 36540 * Math.Pow(s, 4) - 453096 * Math.Pow(s, 6) + 2670030 * Math.Pow(s, 8) - 8306760 * Math.Pow(s, 10) + 13970460 * Math.Pow(s, 12) - 11974680 * Math.Pow(s, 14) + 4091349 * Math.Pow(s, 16)) / 32768));
            LegArrEx[21, 6] = ((15058768725.0 * Math.Pow(c * c, 3) * (-45 * s + 3045 * Math.Pow(s, 3) - 56637 * Math.Pow(s, 5) + 445005 * Math.Pow(s, 7) - 1730575 * Math.Pow(s, 9) + 3492615 * Math.Pow(s, 11) - 3492615 * Math.Pow(s, 13) + 1363783 * Math.Pow(s, 15)) / 2048));
            LegArrEx[21, 7] = ((225881530875.0 * Math.Pow(c, 7) * (-3 + 609 * Math.Pow(s, 2) - 18879 * Math.Pow(s, 4) + 207669 * Math.Pow(s, 6) - 1038345 * Math.Pow(s, 8) + 2561251 * Math.Pow(s, 10) - 3026933 * Math.Pow(s, 12) + 1363783 * Math.Pow(s, 14)) / 2048));
            LegArrEx[21, 8] = (45853950767625.0 * Math.Pow(c * c, 4) * (3 * s - 186 * Math.Pow(s, 3) + 3069 * Math.Pow(s, 5) - 20460 * Math.Pow(s, 7) + 63085 * Math.Pow(s, 9) - 89466 * Math.Pow(s, 11) + 47027 * Math.Pow(s, 13)) / 1024);
            LegArrEx[21, 9] = ((45853950767625.0 * Math.Pow(c, 9) * (3 - 558 * Math.Pow(s, 2) + 15345 * Math.Pow(s, 4) - 143220 * Math.Pow(s, 6) + 567765 * Math.Pow(s, 8) - 984126 * Math.Pow(s, 10) + 611351 * Math.Pow(s, 12)) / 1024));
            LegArrEx[21, 10] = (4264417421389125.0 / 256) * Math.Pow(c * c, 5) * (-3 * s + 165 * Math.Pow(s, 3) - 2310 * Math.Pow(s, 5) + 12210 * Math.Pow(s, 7) - 26455 * Math.Pow(s, 9) + 19721 * Math.Pow(s, 11));
            LegArrEx[21, 11] = (4264417421389125.0 / 256) * Math.Pow(c, 11) * (-3 + 495 * Math.Pow(s, 2) - 11550 * Math.Pow(s, 4) + 85470 * Math.Pow(s, 6) - 238095 * Math.Pow(s, 8) + 216931 * Math.Pow(s, 10));
            LegArrEx[21, 12] = 234542958176401875.0 / 128 * Math.Pow(c * c, 6) * (9 * s - 420 * Math.Pow(s, 3) + 4662 * Math.Pow(s, 5) - 17316 * Math.Pow(s, 7) + 19721 * Math.Pow(s, 9));
            LegArrEx[21, 13] = (2110886623587616875.0 / 128) * Math.Pow(c, 13) * (1 - 140 * Math.Pow(s, 2) + 2590 * Math.Pow(s, 4) - 13468 * Math.Pow(s, 6) + 19721 * Math.Pow(s, 8));
            LegArrEx[21, 14] = (2110886623587616875.0 / 16) * Math.Pow(c * c, 7) * (-35 * s + 1295 * Math.Pow(s, 3) - 10101 * Math.Pow(s, 5) + 19721 * Math.Pow(s, 7));
            LegArrEx[21, 15] = (14776206365113318125.0 / 16) * Math.Pow(c, 15) * (-5 + 555 * Math.Pow(s, 2) - 7215 * Math.Pow(s, 4) + 19721 * Math.Pow(s, 6));
            LegArrEx[21, 16] = 1640158906527578311875.0 / 8 * Math.Pow(c * c, 8) * (5 * s - 130 * Math.Pow(s, 3) + 533 * Math.Pow(s, 5));
            LegArrEx[21, 17] = (8200794532637891559375.0 / 8) * Math.Pow(c, 17) * (1 - 78 * Math.Pow(s, 2) + 533 * Math.Pow(s, 4));
            LegArrEx[21, 18] = (106610328924292590271875.0 / 2) * Math.Pow(c * c, 9) * (-3 * s + 41 * Math.Pow(s, 3));
            LegArrEx[21, 19] = (319830986772877770815625.0 / 2) * Math.Pow(c, 19) * (-1 + 41 * Math.Pow(s, 2));
            LegArrEx[21, 20] = 13113070457687988603440625.0 * s * Math.Pow(c * c, 10);
            LegArrEx[21, 21] = 13113070457687988603440625.0 * Math.Pow(c, 21);

            if (order > 21)
            {
                LegArrEx[22, 0] = (-88179.0 + 22309287 * Math.Pow(s, 2) - 929553625 * Math.Pow(s, 4) + 15058768725 * Math.Pow(s, 6) - 124772655150.0 * Math.Pow(s, 8) + 601681470390.0 * Math.Pow(s, 10) - 1805044411170.0 * Math.Pow(s, 12) + 3471239252250.0 * Math.Pow(s, 14) - 4281195077775.0 * Math.Pow(s, 16) + 3273855059475.0 * Math.Pow(s, 18) - 1412926920405.0 * Math.Pow(s, 20) + 263012370465.0 * Math.Pow(s, 22)) / 524288;
                LegArrEx[22, 1] = (1.0 / 262144) * 253 * c * (88179 * s - 7348250 * Math.Pow(s, 3) + 178562475 * Math.Pow(s, 5) - 1972690200.0 * Math.Pow(s, 7) + 11890938150.0 * Math.Pow(s, 9) - 42807377340.0 * Math.Pow(s, 11) + 96042192750.0 * Math.Pow(s, 13) - 135373757400 * Math.Pow(s, 15) + 116461247175.0 * Math.Pow(s, 17) - 55846913850.0 * Math.Pow(s, 19) + 11435320455.0 * Math.Pow(s, 21));
                LegArrEx[22, 2] = (1.0 / 262144) * 5313 * c * c * (4199 - 1049750 * Math.Pow(s, 2) + 42514875 * Math.Pow(s, 4) - 657563400.0 * Math.Pow(s, 6) + 5096116350.0 * Math.Pow(s, 8) - 22422911940.0 * Math.Pow(s, 10) + 59454690750.0 * Math.Pow(s, 12) - 96695541000.0 * Math.Pow(s, 14) + 94278152475.0 * Math.Pow(s, 16) - 50528160150 * Math.Pow(s, 18) + 11435320455 * Math.Pow(s, 20));
                LegArrEx[22, 3] = (1.0 / 65536) * 132825 * Math.Pow(c, 3) * (-20995 * s + 1700595 * Math.Pow(s, 3) - 39453804 * Math.Pow(s, 5) + 407689308 * Math.Pow(s, 7) - 2242291194.0 * Math.Pow(s, 9) + 7134562890 * Math.Pow(s, 11) - 13537375740.0 * Math.Pow(s, 13) + 15084504396 * Math.Pow(s, 15) - 9095068827.0 * Math.Pow(s, 17) + 2287064091 * Math.Pow(s, 19));
                LegArrEx[22, 4] = (32807775.0 * Math.Pow(c, 4) * (-85 + 20655 * Math.Pow(s, 2) - 798660 * Math.Pow(s, 4) + 11553948 * Math.Pow(s, 6) - 81702918 * Math.Pow(s, 8) + 317733570 * Math.Pow(s, 10) - 712493460.0 * Math.Pow(s, 12) + 916063020.0 * Math.Pow(s, 14) - 625976397 * Math.Pow(s, 16) + 175928007 * Math.Pow(s, 18))) / 65536;
                LegArrEx[22, 5] = ((885809925.0 * Math.Pow(c, 5) * (765 * s - 59160 * Math.Pow(s, 3) + 1283772 * Math.Pow(s, 5) - 12104136 * Math.Pow(s, 7) + 58839550 * Math.Pow(s, 9) - 158331880 * Math.Pow(s, 11) + 237497820.0 * Math.Pow(s, 13) - 185474488 * Math.Pow(s, 15) + 58642669 * Math.Pow(s, 17))) / 32768);
                LegArrEx[22, 6] = ((15058768725.0 * Math.Pow(c, 6) * (45 - 10440 * Math.Pow(s, 2) + 377580 * Math.Pow(s, 4) - 4984056 * Math.Pow(s, 6) + 31150350 * Math.Pow(s, 8) - 102450040 * Math.Pow(s, 10) + 181615980 * Math.Pow(s, 12) - 163653960 * Math.Pow(s, 14) + 58642669 * Math.Pow(s, 16))) / 32768);
                LegArrEx[22, 7] = ((436704293025.0 * Math.Pow(c, 7) * (-45 * s + 3255 * Math.Pow(s, 3) - 64449 * Math.Pow(s, 5) + 537075 * Math.Pow(s, 7) - 2207975 * Math.Pow(s, 9) + 4696965 * Math.Pow(s, 11) - 4937835 * Math.Pow(s, 13) + 2022161 * Math.Pow(s, 15))) / 2048);
                LegArrEx[22, 8] = (6550564395375.0 * Math.Pow(c, 8) * (-3 + 651 * Math.Pow(s, 2) - 21483 * Math.Pow(s, 4) + 250635 * Math.Pow(s, 6) - 1324785 * Math.Pow(s, 8) + 3444441 * Math.Pow(s, 10) - 4279457 * Math.Pow(s, 12) + 2022161 * Math.Pow(s, 14))) / 2048;
                LegArrEx[22, 9] = ((1421472473796375.0 * Math.Pow(c, 9) * (3 * s - 198 * Math.Pow(s, 3) + 3465 * Math.Pow(s, 5) - 24420 * Math.Pow(s, 7) + 79365 * Math.Pow(s, 9) - 118326 * Math.Pow(s, 11) + 65231 * Math.Pow(s, 13))) / 1024);
                LegArrEx[22, 10] = ((1421472473796375.0 * Math.Pow(c, 10) * (3 - 594 * Math.Pow(s, 2) + 17325 * Math.Pow(s, 4) - 170940 * Math.Pow(s, 6) + 714285 * Math.Pow(s, 8) - 1301586 * Math.Pow(s, 10) + 848003 * Math.Pow(s, 12))) / 1024);
                LegArrEx[22, 11] = (4264417421389125.0 / 256) * Math.Pow(c, 11) * (-99 * s + 5775 * Math.Pow(s, 3) - 85470 * Math.Pow(s, 5) + 476190 * Math.Pow(s, 7) - 1084655 * Math.Pow(s, 9) + 848003 * Math.Pow(s, 11));
                LegArrEx[22, 12] = 46908591635280375.0 / 256 * Math.Pow(c, 12) * (-9 + 1575 * Math.Pow(s, 2) - 38850 * Math.Pow(s, 4) + 303030 * Math.Pow(s, 6) - 887445 * Math.Pow(s, 8) + 848003 * Math.Pow(s, 10));
                LegArrEx[22, 13] = (234542958176401875.0 / 128) * Math.Pow(c, 13) * (315 * s - 15540 * Math.Pow(s, 3) + 181818 * Math.Pow(s, 5) - 709956 * Math.Pow(s, 7) + 848003 * Math.Pow(s, 9));
                LegArrEx[22, 14] = (2110886623587616875.0 / 128) * Math.Pow(c, 14) * (35 - 5180 * Math.Pow(s, 2) + 101010 * Math.Pow(s, 4) - 552188 * Math.Pow(s, 6) + 848003 * Math.Pow(s, 8));
                LegArrEx[22, 15] = (78102805072741824375.0 / 16) * Math.Pow(c, 15) * (-35 * s + 1365 * Math.Pow(s, 3) - 11193 * Math.Pow(s, 5) + 22919 * Math.Pow(s, 7));
                LegArrEx[22, 16] = 546719635509192770625.0 / 16 * Math.Pow(c, 16) * (-5 + 585 * Math.Pow(s, 2) - 7995 * Math.Pow(s, 4) + 22919 * Math.Pow(s, 6));
                LegArrEx[22, 17] = (21322065784858518054375.0 / 8) * Math.Pow(c, 17) * (15 * s - 410 * Math.Pow(s, 3) + 1763 * Math.Pow(s, 5));
                LegArrEx[22, 18] = (106610328924292590271875.0 / 8) * Math.Pow(c, 18) * (3 - 246 * Math.Pow(s, 2) + 1763 * Math.Pow(s, 4));
                LegArrEx[22, 19] = (4371023485895996201146875.0 / 2) * Math.Pow(c, 19) * (-3 * s + 43 * Math.Pow(s, 3));
                LegArrEx[22, 20] = 13113070457687988603440625.0 / 2 * Math.Pow(c, 20) * (-1 + 43 * Math.Pow(s, 2));
                LegArrEx[22, 21] = 563862029680583509947946875.0 * s * Math.Pow(c, 21);
                LegArrEx[22, 22] = 563862029680583509947946875.0 * Math.Pow(c, 22);

                LegArrEx[23, 0] = (-2028117.0 * s + 185910725 * Math.Pow(s, 3) - 5019589575.0 * Math.Pow(s, 5) + 62386327575.0 * Math.Pow(s, 7) - 429772478850.0 * Math.Pow(s, 9) + 1805044411170.0 * Math.Pow(s, 11) - 4859734953150.0 * Math.Pow(s, 13) + 8562390155550 * Math.Pow(s, 15) - 9821565178425.0 * Math.Pow(s, 17) + 7064634602025.0 * Math.Pow(s, 19) - 2893136075115.0 * Math.Pow(s, 21) + 514589420475.0 * Math.Pow(s, 23)) / 524288;
                LegArrEx[23, 1] = (1.0 / 524288) * 69 * c * (-29393 + 8083075 * Math.Pow(s, 2) - 363738375 * Math.Pow(s, 4) + 6329047725.0 * Math.Pow(s, 6) - 56057279850.0 * Math.Pow(s, 8) + 287760703230.0 * Math.Pow(s, 10) - 915602237550.0 * Math.Pow(s, 12) + 1861389164250.0 * Math.Pow(s, 14) - 2419805913525.0 * Math.Pow(s, 16) + 1945334165775.0 * Math.Pow(s, 18) - 880519675035.0 * Math.Pow(s, 20) + 171529806825.0 * Math.Pow(s, 22));
                LegArrEx[23, 2] = (1.0 / 262144) * 18975 * c * c * (29393 * s - 2645370 * Math.Pow(s, 3) + 69044157 * Math.Pow(s, 5) - 815378616 * Math.Pow(s, 7) + 5232012786 * Math.Pow(s, 9) - 19976776092 * Math.Pow(s, 11) + 47380815090.0 * Math.Pow(s, 13) - 70394353848.0 * Math.Pow(s, 15) + 63665481789.0 * Math.Pow(s, 17) - 32018897274.0 * Math.Pow(s, 19) + 6861192273.0 * Math.Pow(s, 21));
                LegArrEx[23, 3] = (1.0 / 262144) * 1726725 * Math.Pow(c, 3) * (323 - 87210 * Math.Pow(s, 2) + 3793635 * Math.Pow(s, 4) - 62721432 * Math.Pow(s, 6) + 517451814 * Math.Pow(s, 8) - 2414775132 * Math.Pow(s, 10) + 6768687870 * Math.Pow(s, 12) - 11603464920.0 * Math.Pow(s, 14) + 11893551543.0 * Math.Pow(s, 16) - 6685264266.0 * Math.Pow(s, 18) + 1583352063.0 * Math.Pow(s, 20));
                LegArrEx[23, 4] = (46621575 * Math.Pow(c, 4) * (-1615 * s + 140505 * Math.Pow(s, 3) - 3484524 * Math.Pow(s, 5) + 38329764 * Math.Pow(s, 7) - 223590290 * Math.Pow(s, 9) + 752076430 * Math.Pow(s, 11) - 1504152860 * Math.Pow(s, 13) + 1762007636 * Math.Pow(s, 15) - 1114210711.0 * Math.Pow(s, 17) + 293213345 * Math.Pow(s, 19))) / 65536;
                LegArrEx[23, 5] = ((885809925 * Math.Pow(c, 5) * (-85 + 22185 * Math.Pow(s, 2) - 916980 * Math.Pow(s, 4) + 14121492 * Math.Pow(s, 6) - 105911190 * Math.Pow(s, 8) + 435412670 * Math.Pow(s, 10) - 1029157220 * Math.Pow(s, 12) + 1391058660 * Math.Pow(s, 14) - 996925373.0 * Math.Pow(s, 16) + 293213345 * Math.Pow(s, 18))) / 65536);
                LegArrEx[23, 6] = ((25688487825.0 * Math.Pow(c, 6) * (765 * s - 63240 * Math.Pow(s, 3) + 1460844 * Math.Pow(s, 5) - 14608440 * Math.Pow(s, 7) + 75071150 * Math.Pow(s, 9) - 212929080 * Math.Pow(s, 11) + 335772780 * Math.Pow(s, 13) - 275013896 * Math.Pow(s, 15) + 90997245 * Math.Pow(s, 17))) / 32768);
                LegArrEx[23, 7] = ((6550564395375.0 * Math.Pow(c, 7) * (3 - 744 * Math.Pow(s, 2) + 28644 * Math.Pow(s, 4) - 401016 * Math.Pow(s, 6) + 2649570 * Math.Pow(s, 8) - 9185176 * Math.Pow(s, 10) + 17117828 * Math.Pow(s, 12) - 16177288 * Math.Pow(s, 14) + 6066483 * Math.Pow(s, 16))) / 32768);
                LegArrEx[23, 8] = (203067496256625.0 * Math.Pow(c, 8) * (-3 * s + 231 * Math.Pow(s, 3) - 4851 * Math.Pow(s, 5) + 42735 * Math.Pow(s, 7) - 185185 * Math.Pow(s, 9) + 414141 * Math.Pow(s, 11) - 456617 * Math.Pow(s, 13) + 195693 * Math.Pow(s, 15))) / 2048;
                LegArrEx[23, 9] = ((203067496256625.0 * Math.Pow(c, 9) * (-3 + 693 * Math.Pow(s, 2) - 24255 * Math.Pow(s, 4) + 299145 * Math.Pow(s, 6) - 1666665 * Math.Pow(s, 8) + 4555551 * Math.Pow(s, 10) - 5936021 * Math.Pow(s, 12) + 2935395 * Math.Pow(s, 14))) / 2048);
                LegArrEx[23, 10] = ((4264417421389125.0 * Math.Pow(c, 10) * (33 * s - 2310 * Math.Pow(s, 3) + 42735 * Math.Pow(s, 5) - 317460 * Math.Pow(s, 7) + 1084655 * Math.Pow(s, 9) - 1696006 * Math.Pow(s, 11) + 978465 * Math.Pow(s, 13))) / 1024);
                LegArrEx[23, 11] = ((4264417421389125.0 * Math.Pow(c, 11) * (33 - 6930 * Math.Pow(s, 2) + 213675 * Math.Pow(s, 4) - 2222220 * Math.Pow(s, 6) + 9761895 * Math.Pow(s, 8) - 18656066 * Math.Pow(s, 10) + 12720045 * Math.Pow(s, 12))) / 1024);
                LegArrEx[23, 12] = 21322087106945625.0 / 256 * Math.Pow(c, 12) * (-693 * s + 42735 * Math.Pow(s, 3) - 666666 * Math.Pow(s, 5) + 3904758 * Math.Pow(s, 7) - 9328033 * Math.Pow(s, 9) + 7632027 * Math.Pow(s, 11));
                LegArrEx[23, 13] = (2110886623587616875.0 / 256) * Math.Pow(c, 13) * (-7 + 1295 * Math.Pow(s, 2) - 33670 * Math.Pow(s, 4) + 276094 * Math.Pow(s, 6) - 848003 * Math.Pow(s, 8) + 848003 * Math.Pow(s, 10));
                LegArrEx[23, 14] = (78102805072741824375.0 / 128) * Math.Pow(c, 14) * (35 * s - 1820 * Math.Pow(s, 3) + 22386 * Math.Pow(s, 5) - 91676 * Math.Pow(s, 7) + 114595 * Math.Pow(s, 9));
                LegArrEx[23, 15] = (78102805072741824375.0 / 128) * Math.Pow(c, 15) * (35 - 5460 * Math.Pow(s, 2) + 111930 * Math.Pow(s, 4) - 641732 * Math.Pow(s, 6) + 1031355 * Math.Pow(s, 8));
                LegArrEx[23, 16] = 3046009397836931150625.0 / 16 * Math.Pow(c, 16) * (-35 * s + 1435 * Math.Pow(s, 3) - 12341 * Math.Pow(s, 5) + 26445 * Math.Pow(s, 7));
                LegArrEx[23, 17] = (106610328924292590271875.0 / 16) * Math.Pow(c, 17) * (-1 + 123 * Math.Pow(s, 2) - 1763 * Math.Pow(s, 4) + 5289 * Math.Pow(s, 6));
                LegArrEx[23, 18] = (4371023485895996201146875.0 / 8) * Math.Pow(c, 18) * (3 * s - 86 * Math.Pow(s, 3) + 387 * Math.Pow(s, 5));
                LegArrEx[23, 19] = (13113070457687988603440625.0 / 8) * Math.Pow(c, 19) * (1 - 86 * Math.Pow(s, 2) + 645 * Math.Pow(s, 4));
                LegArrEx[23, 20] = 563862029680583509947946875.0 / 2 * Math.Pow(c, 20) * (-s + 15 * Math.Pow(s, 3));
                LegArrEx[23, 21] = (563862029680583509947946875.0 / 2) * Math.Pow(c, 21) * (-1 + 45 * Math.Pow(s, 2));
                LegArrEx[23, 22] = 25373791335626257947657609375.0 * s * Math.Pow(c, 22);
                LegArrEx[23, 23] = 25373791335626257947657609375.0 * Math.Pow(c, 23);

                LegArrEx[24, 0] = (676039 - 202811700 * Math.Pow(s, 2) + 10039179150 * Math.Pow(s, 4) - 194090796900 * Math.Pow(s, 6) + 1933976154825 * Math.Pow(s, 8) - 11345993441640 * Math.Pow(s, 10) + 42117702927300 * Math.Pow(s, 12) - 102748681866600 * Math.Pow(s, 14) + 166966608033225 * Math.Pow(s, 16) - 178970743251300 * Math.Pow(s, 18) + 121511715154830 * Math.Pow(s, 20) - 47342226683700 * Math.Pow(s, 22) + 8061900920775 * Math.Pow(s, 24)) / 4194304;
                LegArrEx[24, 1] = (1.0 / 524288) * 75 * c * (-676039 * s + 66927861 * Math.Pow(s, 3) - 1940907969 * Math.Pow(s, 5) + 25786348731 * Math.Pow(s, 7) - 189099890694 * Math.Pow(s, 9) + 842354058546 * Math.Pow(s, 11) - 2397469243554 * Math.Pow(s, 13) + 4452442880886 * Math.Pow(s, 15) - 5369122297539 * Math.Pow(s, 17) + 4050390505161 * Math.Pow(s, 19) - 1735881645069 * Math.Pow(s, 21) + 322476036831 * Math.Pow(s, 23));
                LegArrEx[24, 2] = (1.0 / 524288) * 22425 * c * c * (-2261 + 671517 * Math.Pow(s, 2) - 32456655 * Math.Pow(s, 4) + 603693783 * Math.Pow(s, 6) - 5691969954 * Math.Pow(s, 8) + 30989614194 * Math.Pow(s, 10) - 104237793198 * Math.Pow(s, 12) + 223366699710 * Math.Pow(s, 14) - 305267822937 * Math.Pow(s, 16) + 257382674241 * Math.Pow(s, 18) - 121918108851 * Math.Pow(s, 20) + 24805848987 * Math.Pow(s, 22));
                LegArrEx[24, 3] = (1.0 / 262144) * 2220075 * Math.Pow(c, 3) * (6783 * s - 655690 * Math.Pow(s, 3) + 18293751 * Math.Pow(s, 5) - 229978584 * Math.Pow(s, 7) + 1565132030 * Math.Pow(s, 9) - 6317442012 * Math.Pow(s, 11) + 15793605030 * Math.Pow(s, 13) - 24668106904 * Math.Pow(s, 15) + 23398424931 * Math.Pow(s, 17) - 12314960490 * Math.Pow(s, 19) + 2756205443 * Math.Pow(s, 21));
                LegArrEx[24, 4] = (1.0 / 262144) * 46621575 * Math.Pow(c, 4) * (323 - 93670 * Math.Pow(s, 2) + 4355655 * Math.Pow(s, 4) - 76659528 * Math.Pow(s, 6) + 670770870 * Math.Pow(s, 8) - 3309136292 * Math.Pow(s, 10) + 9776993590 * Math.Pow(s, 12) - 17620076360 * Math.Pow(s, 14) + 18941582087 * Math.Pow(s, 16) - 11142107110 * Math.Pow(s, 18) + 2756205443 * Math.Pow(s, 20));
                LegArrEx[24, 5] = ((1352025675.0 * Math.Pow(c, 5) * (-1615 * s + 150195 * Math.Pow(s, 3) - 3965148 * Math.Pow(s, 5) + 46260060 * Math.Pow(s, 7) - 285270370 * Math.Pow(s, 9) + 1011413130 * Math.Pow(s, 11) - 2126560940 * Math.Pow(s, 13) + 2612632012 * Math.Pow(s, 15) - 1728947655 * Math.Pow(s, 17) + 475207835 * Math.Pow(s, 19))) / 65536);
                LegArrEx[24, 6] = ((128442439125.0 * Math.Pow(c, 6) * (-17 + 4743 * Math.Pow(s, 2) - 208692 * Math.Pow(s, 4) + 3408636 * Math.Pow(s, 6) - 27025614 * Math.Pow(s, 8) + 117110994 * Math.Pow(s, 10) - 291003076 * Math.Pow(s, 12) + 412520844 * Math.Pow(s, 14) - 309390633 * Math.Pow(s, 16) + 95041567 * Math.Pow(s, 18))) / 65536);
                LegArrEx[24, 7] = ((11945146838625.0 * Math.Pow(c, 7) * (51 * s - 4488 * Math.Pow(s, 3) + 109956 * Math.Pow(s, 5) - 1162392 * Math.Pow(s, 7) + 6296290 * Math.Pow(s, 9) - 18774392 * Math.Pow(s, 11) + 31049956 * Math.Pow(s, 13) - 26614248 * Math.Pow(s, 15) + 9197571 * Math.Pow(s, 17))) / 32768);
                LegArrEx[24, 8] = (203067496256625.0 * Math.Pow(c, 8) * (3 - 792 * Math.Pow(s, 2) + 32340 * Math.Pow(s, 4) - 478632 * Math.Pow(s, 6) + 3333330 * Math.Pow(s, 8) - 12148136 * Math.Pow(s, 10) + 23744084 * Math.Pow(s, 12) - 23483160 * Math.Pow(s, 14) + 9197571 * Math.Pow(s, 16))) / 32768;
                LegArrEx[24, 9] = ((203067496256625.0 * Math.Pow(c, 9) * (-99 * s + 8085 * Math.Pow(s, 3) - 179487 * Math.Pow(s, 5) + 1666665 * Math.Pow(s, 7) - 7592585 * Math.Pow(s, 9) + 17808063 * Math.Pow(s, 11) - 20547765 * Math.Pow(s, 13) + 9197571 * Math.Pow(s, 15))) / 2048);
                LegArrEx[24, 10] = ((609202488769875.0 * Math.Pow(c, 10) * (-33 + 8085 * Math.Pow(s, 2) - 299145 * Math.Pow(s, 4) + 3888885 * Math.Pow(s, 6) - 22777755 * Math.Pow(s, 8) + 65296231 * Math.Pow(s, 10) - 89040315 * Math.Pow(s, 12) + 45987855 * Math.Pow(s, 14))) / 2048);
                LegArrEx[24, 11] = ((21322087106945625.0 * Math.Pow(c, 11) * (231 * s - 17094 * Math.Pow(s, 3) + 333333 * Math.Pow(s, 5) - 2603172 * Math.Pow(s, 7) + 9328033 * Math.Pow(s, 9) - 15264054 * Math.Pow(s, 11) + 9197571 * Math.Pow(s, 13))) / 1024);
                LegArrEx[24, 12] = (63966261320836875.0 * Math.Pow(c, 12) * (77 - 17094 * Math.Pow(s, 2) + 555555 * Math.Pow(s, 4) - 6074068 * Math.Pow(s, 6) + 27984099 * Math.Pow(s, 8) - 55968198 * Math.Pow(s, 10) + 39856141 * Math.Pow(s, 12))) / 1024;
                LegArrEx[24, 13] = (7100255006612893125.0 / 256) * Math.Pow(c, 13) * (-77 * s + 5005 * Math.Pow(s, 3) - 82082 * Math.Pow(s, 5) + 504218 * Math.Pow(s, 7) - 1260545 * Math.Pow(s, 9) + 1077193 * Math.Pow(s, 11));
                LegArrEx[24, 14] = (78102805072741824375.0 / 256) * Math.Pow(c, 14) * (-7 + 1365 * Math.Pow(s, 2) - 37310 * Math.Pow(s, 4) + 320866 * Math.Pow(s, 6) - 1031355 * Math.Pow(s, 8) + 1077193 * Math.Pow(s, 10));
                LegArrEx[24, 15] = (1015336465945643716875.0 / 128) * Math.Pow(c, 15) * (105 * s - 5740 * Math.Pow(s, 3) + 74046 * Math.Pow(s, 5) - 317340 * Math.Pow(s, 7) + 414305 * Math.Pow(s, 9));
                LegArrEx[24, 16] = 15230046989184655753125.0 / 128 * Math.Pow(c, 16) * (7 - 1148 * Math.Pow(s, 2) + 24682 * Math.Pow(s, 4) - 148092 * Math.Pow(s, 6) + 248583 * Math.Pow(s, 8));
                LegArrEx[24, 17] = (624431926556570885878125.0 / 16) * Math.Pow(c, 17) * (-7 * s + 301 * Math.Pow(s, 3) - 2709 * Math.Pow(s, 5) + 6063 * Math.Pow(s, 7));
                LegArrEx[24, 18] = (4371023485895996201146875.0 / 16) * Math.Pow(c, 18) * (-1 + 129 * Math.Pow(s, 2) - 1935 * Math.Pow(s, 4) + 6063 * Math.Pow(s, 6));
                LegArrEx[24, 19] = (563862029680583509947946875.0 / 8) * Math.Pow(c, 19) * (s - 30 * Math.Pow(s, 3) + 141 * Math.Pow(s, 5));
                LegArrEx[24, 20] = 563862029680583509947946875.0 / 8 * Math.Pow(c, 20) * (1 - 90 * Math.Pow(s, 2) + 705 * Math.Pow(s, 4));
                LegArrEx[24, 21] = (8457930445208752649219203125.0 / 2) * Math.Pow(c, 21) * (-3 * s + 47 * Math.Pow(s, 3));
                LegArrEx[24, 22] = (25373791335626257947657609375.0 / 2) * Math.Pow(c, 22) * (-1 + 47 * Math.Pow(s, 2));
                LegArrEx[24, 23] = 1192568192774434123539907640625.0 * s * Math.Pow(c, 23);
                LegArrEx[24, 24] = 1192568192774434123539907640625.0 * Math.Pow(c, 24);

                LegArrEx[25, 0] = (16900975 * s - 1825305300.0 * Math.Pow(s, 3) + 58227239070.0 * Math.Pow(s, 5) - 859544957700.0 * Math.Pow(s, 7) + 7091245901025.0 * Math.Pow(s, 9) - 36100888223400.0 * Math.Pow(s, 11) + 119873462177700.0 * Math.Pow(s, 13) - 267146572853160.0 * Math.Pow(s, 15) + 402684172315425.0 * Math.Pow(s, 17) - 405039050516100.0 * Math.Pow(s, 19) + 260382246760350.0 * Math.Pow(s, 21) - 96742811049300.0 * Math.Pow(s, 23) + 15801325804719.0 * Math.Pow(s, 25)) / 4194304;
                LegArrEx[25, 1] = (1.0 / 4194304) * 325 * c * (52003 - 16848972 * Math.Pow(s, 2) + 895803678 * Math.Pow(s, 4) - 18513276012 * Math.Pow(s, 6) + 196372963413.0 * Math.Pow(s, 8) - 1221876216792.0 * Math.Pow(s, 10) + 4794938487108.0 * Math.Pow(s, 12) - 12329841823992.0 * Math.Pow(s, 14) + 21063479782653.0 * Math.Pow(s, 16) - 23679206030172.0 * Math.Pow(s, 18) + 16824699021438.0 * Math.Pow(s, 20) - 6846414320412.0 * Math.Pow(s, 22) + 1215486600363.0 * Math.Pow(s, 24));
                LegArrEx[25, 2] = (1.0 / 524288) * 8775 * c * c * (-156009 * s + 16588957 * Math.Pow(s, 3) - 514257667 * Math.Pow(s, 5) + 7273072719.0 * Math.Pow(s, 7) - 56568343370 * Math.Pow(s, 9) + 266385471506.0 * Math.Pow(s, 11) - 799156414518.0 * Math.Pow(s, 13) + 1560257761678.0 * Math.Pow(s, 15) - 1973267169181.0 * Math.Pow(s, 17) + 1557842501985.0 * Math.Pow(s, 19) - 697319977079.0 * Math.Pow(s, 21) + 135054066707.0 * Math.Pow(s, 23));
                LegArrEx[25, 3] = (1.0 / 524288) * 1412775 * Math.Pow(c, 3) * (-969 + 309111 * Math.Pow(s, 2) - 15970735 * Math.Pow(s, 4) + 316220553 * Math.Pow(s, 6) - 3162205530 * Math.Pow(s, 8) + 18200249606.0 * Math.Pow(s, 10) - 64528157694 * Math.Pow(s, 12) + 145365629970.0 * Math.Pow(s, 14) - 208357402957 * Math.Pow(s, 16) + 183844767315.0 * Math.Pow(s, 18) - 90954779619.0 * Math.Pow(s, 20) + 19293438101.0 * Math.Pow(s, 22));
                LegArrEx[25, 4] = (1.0 / 262144) * 450675225 * Math.Pow(c, 4) * (969 * s - 100130 * Math.Pow(s, 3) + 2973861 * Math.Pow(s, 5) - 39651480 * Math.Pow(s, 7) + 285270370 * Math.Pow(s, 9) - 1213695756 * Math.Pow(s, 11) + 3189841410 * Math.Pow(s, 13) - 5225264024 * Math.Pow(s, 15) + 5186842965 * Math.Pow(s, 17) - 2851247010 * Math.Pow(s, 19) + 665290969 * Math.Pow(s, 21));
                LegArrEx[25, 5] = (1.0 / 262144) * 1352025675 * Math.Pow(c, 5) * (323 - 100130 * Math.Pow(s, 2) + 4956435 * Math.Pow(s, 4) - 92520120 * Math.Pow(s, 6) + 855811110 * Math.Pow(s, 8) - 4450217772 * Math.Pow(s, 10) + 13822646110 * Math.Pow(s, 12) - 26126320120.0 * Math.Pow(s, 14) + 29392110135 * Math.Pow(s, 16) - 18057897730 * Math.Pow(s, 18) + 4657036783 * Math.Pow(s, 20));
                LegArrEx[25, 6] = ((209563979625 * Math.Pow(c, 6) * (-323 * s + 31977 * Math.Pow(s, 3) - 895356 * Math.Pow(s, 5) + 11042724 * Math.Pow(s, 7) - 71777706 * Math.Pow(s, 9) + 267535086 * Math.Pow(s, 11) - 589949164 * Math.Pow(s, 13) + 758506068 * Math.Pow(s, 15) - 524261547 * Math.Pow(s, 17) + 150226993 * Math.Pow(s, 19))) / 65536);
                LegArrEx[25, 7] = ((3981715612875 * Math.Pow(c, 7) * (-17 + 5049 * Math.Pow(s, 2) - 235620 * Math.Pow(s, 4) + 4068372 * Math.Pow(s, 6) - 33999966 * Math.Pow(s, 8) + 154888734 * Math.Pow(s, 10) - 403649428 * Math.Pow(s, 12) + 598820580 * Math.Pow(s, 14) - 469076121 * Math.Pow(s, 16) + 150226993 * Math.Pow(s, 18))) / 65536);
                LegArrEx[25, 8] = (11945146838625.0 * Math.Pow(c, 8) * (1683 * s - 157080 * Math.Pow(s, 3) + 4068372 * Math.Pow(s, 5) - 45333288 * Math.Pow(s, 7) + 258147890 * Math.Pow(s, 9) - 807298856 * Math.Pow(s, 11) + 1397248020 * Math.Pow(s, 13) - 1250869656 * Math.Pow(s, 15) + 450680979 * Math.Pow(s, 17))) / 32768;
                LegArrEx[25, 9] = ((203067496256625.0 * Math.Pow(c, 9) * (99 - 27720 * Math.Pow(s, 2) + 1196580 * Math.Pow(s, 4) - 18666648 * Math.Pow(s, 6) + 136666530 * Math.Pow(s, 8) - 522369848 * Math.Pow(s, 10) + 1068483780 * Math.Pow(s, 12) - 1103708520 * Math.Pow(s, 14) + 450680979 * Math.Pow(s, 16))) / 32768);
                LegArrEx[25, 10] = ((1421472473796375.0 * Math.Pow(c, 10) * (-495 * s + 42735 * Math.Pow(s, 3) - 999999 * Math.Pow(s, 5) + 9761895 * Math.Pow(s, 7) - 46640165 * Math.Pow(s, 9) + 114480405 * Math.Pow(s, 11) - 137963565 * Math.Pow(s, 13) + 64382997 * Math.Pow(s, 15))) / 2048);
                LegArrEx[25, 11] = ((63966261320836875.0 * Math.Pow(c, 11) * (-11 + 2849 * Math.Pow(s, 2) - 111111 * Math.Pow(s, 4) + 1518517 * Math.Pow(s, 6) - 9328033 * Math.Pow(s, 8) + 27984099 * Math.Pow(s, 10) - 39856141 * Math.Pow(s, 12) + 21460999 * Math.Pow(s, 14))) / 2048);
                LegArrEx[25, 12] = (2366751668870964375.0 * Math.Pow(c, 12) * (77 * s - 6006 * Math.Pow(s, 3) + 123123 * Math.Pow(s, 5) - 1008436 * Math.Pow(s, 7) + 3781635 * Math.Pow(s, 9) - 6463158 * Math.Pow(s, 11) + 4060189 * Math.Pow(s, 13))) / 1024;
                LegArrEx[25, 13] = ((2366751668870964375.0 * Math.Pow(c, 13) * (77 - 18018 * Math.Pow(s, 2) + 615615 * Math.Pow(s, 4) - 7059052 * Math.Pow(s, 6) + 34034715 * Math.Pow(s, 8) - 71094738 * Math.Pow(s, 10) + 52782457 * Math.Pow(s, 12))) / 1024);
                LegArrEx[25, 14] = (92303315085967610625.0 / 256) * Math.Pow(c, 14) * (-231 * s + 15785 * Math.Pow(s, 3) - 271502 * Math.Pow(s, 5) + 1745370 * Math.Pow(s, 7) - 4557355 * Math.Pow(s, 9) + 4060189 * Math.Pow(s, 11));
                LegArrEx[25, 15] = (1015336465945643716875.0 / 256) * Math.Pow(c, 15) * (-21 + 4305 * Math.Pow(s, 2) - 123410 * Math.Pow(s, 4) + 1110690 * Math.Pow(s, 6) - 3728745 * Math.Pow(s, 8) + 4060189 * Math.Pow(s, 10));
                LegArrEx[25, 16] = 208143975518856961959375.0 / 128 * Math.Pow(c, 16) * (21 * s - 1204 * Math.Pow(s, 3) + 16254 * Math.Pow(s, 5) - 72756 * Math.Pow(s, 7) + 99029 * Math.Pow(s, 9));
                LegArrEx[25, 17] = (4371023485895996201146875.0 / 128) * Math.Pow(c, 17) * (1 - 172 * Math.Pow(s, 2) + 3870 * Math.Pow(s, 4) - 24252 * Math.Pow(s, 6) + 42441 * Math.Pow(s, 8));
                LegArrEx[25, 18] = (187954009893527836649315625.0 / 16) * Math.Pow(c, 18) * (-s + 45 * Math.Pow(s, 3) - 423 * Math.Pow(s, 5) + 987 * Math.Pow(s, 7));
                LegArrEx[25, 19] = (187954009893527836649315625.0 / 16) * Math.Pow(c, 19) * (-1 + 135 * Math.Pow(s, 2) - 2115 * Math.Pow(s, 4) + 6909 * Math.Pow(s, 6));
                LegArrEx[25, 20] = 1691586089041750529843840625.0 / 8 * Math.Pow(c, 20) * (15 * s - 470 * Math.Pow(s, 3) + 2303 * Math.Pow(s, 5));
                LegArrEx[25, 21] = (8457930445208752649219203125.0 / 8) * Math.Pow(c, 21) * (3 - 282 * Math.Pow(s, 2) + 2303 * Math.Pow(s, 4));
                LegArrEx[25, 22] = (397522730924811374513302546875.0 / 2) * Math.Pow(c, 22) * (-3 * s + 49 * Math.Pow(s, 3));
                LegArrEx[25, 23] = (1192568192774434123539907640625.0 / 2) * Math.Pow(c, 23) * (-1 + 49 * Math.Pow(s, 2));
                LegArrEx[25, 24] = 58435841445947272053455474390625.0 * s * Math.Pow(c, 24);
                LegArrEx[25, 25] = 58435841445947272053455474390625.0 * Math.Pow(c, 25);

                LegArrEx[26, 0] = (-1300075 + 456326325.0 * Math.Pow(s, 2) - 26466926850.0 * Math.Pow(s, 4) + 601681470390.0 * Math.Pow(s, 6) - 7091245901025.0 * Math.Pow(s, 8) + 49638721307175.0 * Math.Pow(s, 10) - 222622144044300.0 * Math.Pow(s, 12) + 667866432132900.0 * Math.Pow(s, 14) - 1369126185872445.0 * Math.Pow(s, 16) + 1923935489951475.0 * Math.Pow(s, 18) - 1822675727322450.0 * Math.Pow(s, 20) + 1112542327066950.0 * Math.Pow(s, 22) - 395033145117975.0 * Math.Pow(s, 24) + 61989816618513.0 * Math.Pow(s, 26)) / 8388608;
                LegArrEx[26, 1] = (1.0 / 4194304) * 351 * c * (1300075 * s - 150808700.0 * Math.Pow(s, 3) + 5142576670.0 * Math.Pow(s, 5) - 80811919100 * Math.Pow(s, 7) + 707104292125.0 * Math.Pow(s, 9) - 3805506735800.0 * Math.Pow(s, 11) + 13319273575300.0 * Math.Pow(s, 13) - 31205155233560.0 * Math.Pow(s, 15) + 49331679229525.0 * Math.Pow(s, 17) - 51928083399500.0 * Math.Pow(s, 19) + 34865998853950.0 * Math.Pow(s, 21) - 13505406670700.0 * Math.Pow(s, 23) + 2295919134019.0 * Math.Pow(s, 25));
                LegArrEx[26, 2] = (1.0 / 4194304) * 61425 * c * c * (7429 - 2585292 * Math.Pow(s, 2) + 146930762 * Math.Pow(s, 4) - 3232476764 * Math.Pow(s, 6) + 36365363595.0 * Math.Pow(s, 8) - 239203280536.0 * Math.Pow(s, 10) + 989431751308.0 * Math.Pow(s, 12) - 2674727591448.0 * Math.Pow(s, 14) + 4792220268011.0 * Math.Pow(s, 16) - 5637906197660.0 * Math.Pow(s, 18) + 4183919862474.0 * Math.Pow(s, 20) - 1774996305292.0 * Math.Pow(s, 22) + 327988447717.0 * Math.Pow(s, 24));
                LegArrEx[26, 3] = (1.0 / 524288) * 1781325 * Math.Pow(c, 3) * (-22287 * s + 2533289 * Math.Pow(s, 3) - 83598537 * Math.Pow(s, 5) + 1253978055 * Math.Pow(s, 7) - 10310486230.0 * Math.Pow(s, 9) + 51177504378 * Math.Pow(s, 11) - 161405975346.0 * Math.Pow(s, 13) + 330497949518.0 * Math.Pow(s, 15) - 437423756715.0 * Math.Pow(s, 17) + 360682746765.0 * Math.Pow(s, 19) - 168318615157.0 * Math.Pow(s, 21) + 33929839419.0 * Math.Pow(s, 23));
                LegArrEx[26, 4] = (1.0 / 524288) * 122911425 * Math.Pow(c, 4) * (-323 + 110143 * Math.Pow(s, 2) - 6057865 * Math.Pow(s, 4) + 127215165 * Math.Pow(s, 6) - 1344846030 * Math.Pow(s, 8) + 8158732582 * Math.Pow(s, 10) - 30409821442.0 * Math.Pow(s, 12) + 71847380330.0 * Math.Pow(s, 14) - 107771070495.0 * Math.Pow(s, 16) + 99318437515.0 * Math.Pow(s, 18) - 51227404613 * Math.Pow(s, 20) + 11309946473 * Math.Pow(s, 22));
                LegArrEx[26, 5] = (1.0 / 262144) * 41912795925.0 * Math.Pow(c, 5) * (323 * s - 35530 * Math.Pow(s, 3) + 1119195 * Math.Pow(s, 5) - 15775320 * Math.Pow(s, 7) + 119629510 * Math.Pow(s, 9) - 535070172 * Math.Pow(s, 11) + 1474872910 * Math.Pow(s, 13) - 2528353560 * Math.Pow(s, 15) + 2621307735 * Math.Pow(s, 17) - 1502269930 * Math.Pow(s, 19) + 364836983 * Math.Pow(s, 21));
                LegArrEx[26, 6] = (1.0 / 262144) * 41912795925.0 * Math.Pow(c, 6) * (323 - 106590 * Math.Pow(s, 2) + 5595975 * Math.Pow(s, 4) - 110427240 * Math.Pow(s, 6) + 1076665590 * Math.Pow(s, 8) - 5885771892 * Math.Pow(s, 10) + 19173347830 * Math.Pow(s, 12) - 37925303400 * Math.Pow(s, 14) + 44562231495 * Math.Pow(s, 16) - 28543128670 * Math.Pow(s, 18) + 7661576643 * Math.Pow(s, 20));
                LegArrEx[26, 7] = (1.0 / 65536) * 628691938875.0 * Math.Pow(c, 7) * (-3553 * s + 373065 * Math.Pow(s, 3) - 11042724 * Math.Pow(s, 5) + 143555412 * Math.Pow(s, 7) - 980961982 * Math.Pow(s, 9) + 3834669566 * Math.Pow(s, 11) - 8849237460 * Math.Pow(s, 13) + 11883261732 * Math.Pow(s, 15) - 8562938601 * Math.Pow(s, 17) + 2553858881 * Math.Pow(s, 19));
                LegArrEx[26, 8] = (203067496256625.0 * Math.Pow(c, 8) * (-11 + 3465 * Math.Pow(s, 2) - 170940 * Math.Pow(s, 4) + 3111108 * Math.Pow(s, 6) - 27333306 * Math.Pow(s, 8) + 130592462 * Math.Pow(s, 10) - 356161260 * Math.Pow(s, 12) + 551854260 * Math.Pow(s, 14) - 450680979 * Math.Pow(s, 16) + 150226993 * Math.Pow(s, 18))) / 65536;
                LegArrEx[26, 9] = ((1421472473796375.0 * Math.Pow(c, 9) * (495 * s - 48840 * Math.Pow(s, 3) + 1333332 * Math.Pow(s, 5) - 15619032 * Math.Pow(s, 7) + 93280330 * Math.Pow(s, 9) - 305281080 * Math.Pow(s, 11) + 551854260 * Math.Pow(s, 13) - 515063976 * Math.Pow(s, 15) + 193148991 * Math.Pow(s, 17))) / 32768);
                LegArrEx[26, 10] = ((12793252264167375.0 * Math.Pow(c, 10) * (55 - 16280 * Math.Pow(s, 2) + 740740 * Math.Pow(s, 4) - 12148136 * Math.Pow(s, 6) + 93280330 * Math.Pow(s, 8) - 373121320 * Math.Pow(s, 10) + 797122820 * Math.Pow(s, 12) - 858439960 * Math.Pow(s, 14) + 364836983 * Math.Pow(s, 16))) / 32768);
                LegArrEx[26, 11] = ((473350333774192875.0 * Math.Pow(c, 11) * (-55 * s + 5005 * Math.Pow(s, 3) - 123123 * Math.Pow(s, 5) + 1260545 * Math.Pow(s, 7) - 6302725 * Math.Pow(s, 9) + 16157895 * Math.Pow(s, 11) - 20300945 * Math.Pow(s, 13) + 9860459 * Math.Pow(s, 15))) / 2048);
                LegArrEx[26, 12] = (2366751668870964375.0 * Math.Pow(c, 12) * (-11 + 3003 * Math.Pow(s, 2) - 123123 * Math.Pow(s, 4) + 1764763 * Math.Pow(s, 6) - 11344905 * Math.Pow(s, 8) + 35547369 * Math.Pow(s, 10) - 52782457 * Math.Pow(s, 12) + 29581377 * Math.Pow(s, 14))) / 2048;
                LegArrEx[26, 13] = ((7100255006612893125.0 * Math.Pow(c, 13) * (1001 * s - 82082 * Math.Pow(s, 3) + 1764763 * Math.Pow(s, 5) - 15126540 * Math.Pow(s, 7) + 59245615 * Math.Pow(s, 9) - 105564914 * Math.Pow(s, 11) + 69023213 * Math.Pow(s, 13))) / 1024);
                LegArrEx[26, 14] = ((92303315085967610625.0 * Math.Pow(c, 14) * (77 - 18942 * Math.Pow(s, 2) + 678755 * Math.Pow(s, 4) - 8145060 * Math.Pow(s, 6) + 41016195 * Math.Pow(s, 8) - 89324158 * Math.Pow(s, 10) + 69023213 * Math.Pow(s, 12))) / 1024);
                LegArrEx[26, 15] = (3784435918524672035625.0 / 256) * Math.Pow(c, 15) * (-231 * s + 16555 * Math.Pow(s, 3) - 297990 * Math.Pow(s, 5) + 2000790 * Math.Pow(s, 7) - 5446595 * Math.Pow(s, 9) + 5050479 * Math.Pow(s, 11));
                LegArrEx[26, 16] = 874204697179199240229375.0 / 256 * Math.Pow(c, 16) * (-1 + 215 * Math.Pow(s, 2) - 6450 * Math.Pow(s, 4) + 60630 * Math.Pow(s, 6) - 212205 * Math.Pow(s, 8) + 240499 * Math.Pow(s, 10));
                LegArrEx[26, 17] = (187954009893527836649315625.0 / 128) * Math.Pow(c, 17) * (s - 60 * Math.Pow(s, 3) + 846 * Math.Pow(s, 5) - 3948 * Math.Pow(s, 7) + 5593 * Math.Pow(s, 9));
                LegArrEx[26, 18] = (187954009893527836649315625.0 / 128) * Math.Pow(c, 18) * (1 - 180 * Math.Pow(s, 2) + 4230 * Math.Pow(s, 4) - 27636 * Math.Pow(s, 6) + 50337 * Math.Pow(s, 8));
                LegArrEx[26, 19] = (1691586089041750529843840625.0 / 16) * Math.Pow(c, 19) * (-5 * s + 235 * Math.Pow(s, 3) - 2303 * Math.Pow(s, 5) + 5593 * Math.Pow(s, 7));
                LegArrEx[26, 20] = 1691586089041750529843840625.0 / 16 * Math.Pow(c, 20) * (-5 + 705 * Math.Pow(s, 2) - 11515 * Math.Pow(s, 4) + 39151 * Math.Pow(s, 6));
                LegArrEx[26, 21] = (79504546184962274902660509375.0 / 8) * Math.Pow(c, 21) * (15 * s - 490 * Math.Pow(s, 3) + 2499 * Math.Pow(s, 5));
                LegArrEx[26, 22] = (1192568192774434123539907640625.0 / 8) * Math.Pow(c, 22) * (1 - 98 * Math.Pow(s, 2) + 833 * Math.Pow(s, 4));
                LegArrEx[26, 23] = (58435841445947272053455474390625.0 / 2) * Math.Pow(c, 23) * (-s + 17 * Math.Pow(s, 3));
                LegArrEx[26, 24] = 58435841445947272053455474390625.0 / 2 * Math.Pow(c, 24) * (-1 + 51 * Math.Pow(s, 2));
                LegArrEx[26, 25] = 2980227913743310874726229193921875.0 * s * Math.Pow(c, 25);
                LegArrEx[26, 26] = 2980227913743310874726229193921875.0 * Math.Pow(c, 26);

                LegArrEx[27, 0] = (-35102025.0 * s + 4411154475 * Math.Pow(s, 3) - 164094946470.0 * Math.Pow(s, 5) + 2836498360410.0 * Math.Pow(s, 7) - 27577067392875 * Math.Pow(s, 9) + 166966608033225 * Math.Pow(s, 11) - 667866432132900 * Math.Pow(s, 13) + 1825501581163260 * Math.Pow(s, 15) - 3463083881912655 * Math.Pow(s, 17) + 4556689318306125.0 * Math.Pow(s, 19) - 4079321865912150.0 * Math.Pow(s, 21) + 2370198870707850.0 * Math.Pow(s, 23) - 805867616040669.0 * Math.Pow(s, 25) + 121683714103007.0 * Math.Pow(s, 27)) / 8388608;
                LegArrEx[27, 1] = (1.0 / 8388608) * 189 * c * (-185725 + 70018325 * Math.Pow(s, 2) - 4341136150 * Math.Pow(s, 4) + 105055494830 * Math.Pow(s, 6) - 1313193685375 * Math.Pow(s, 8) + 9717633271775 * Math.Pow(s, 10) - 45937902739300 * Math.Pow(s, 12) + 144881077870100 * Math.Pow(s, 14) - 311494317420715 * Math.Pow(s, 16) + 458079878559875.0 * Math.Pow(s, 18) - 453257985101350 * Math.Pow(s, 20) + 288436899609950.0 * Math.Pow(s, 22) - 106596245508025.0 * Math.Pow(s, 24) + 17383387729001.0 * Math.Pow(s, 26));
                LegArrEx[27, 2] = (1.0 / 4194304) * 71253 * c * c * (185725 * s - 23029900 * Math.Pow(s, 3) + 835985370 * Math.Pow(s, 5) - 13933089500.0 * Math.Pow(s, 7) + 128881077875 * Math.Pow(s, 9) - 731107205400 * Math.Pow(s, 11) + 2690099589100 * Math.Pow(s, 13) - 6609958990360 * Math.Pow(s, 15) + 10935593917875 * Math.Pow(s, 17) - 12022758225500 * Math.Pow(s, 19) + 8415930757850.0 * Math.Pow(s, 21) - 3392983941900.0 * Math.Pow(s, 23) + 599427163069.0 * Math.Pow(s, 25));
                LegArrEx[27, 3] = (1.0 / 4194304) * 1781325 * Math.Pow(c, 3) * (7429 - 2763588 * Math.Pow(s, 2) + 167197074 * Math.Pow(s, 4) - 3901265060 * Math.Pow(s, 6) + 46397188035 * Math.Pow(s, 8) - 321687170376 * Math.Pow(s, 10) + 1398851786332 * Math.Pow(s, 12) - 3965975394216 * Math.Pow(s, 14) + 7436203864155 * Math.Pow(s, 16) - 9137296251380 * Math.Pow(s, 18) + 7069381836594.0 * Math.Pow(s, 20) - 3121545226548 * Math.Pow(s, 22) + 599427163069.0 * Math.Pow(s, 24));
                LegArrEx[27, 4] = (1.0 / 524288) * 165663225 * Math.Pow(c, 4) * (-7429 * s + 898909 * Math.Pow(s, 3) - 31461815 * Math.Pow(s, 5) + 498894495 * Math.Pow(s, 7) - 4323752290 * Math.Pow(s, 9) + 22562125586 * Math.Pow(s, 11) - 74628569246 * Math.Pow(s, 13) + 159918362670 * Math.Pow(s, 15) - 221063618985 * Math.Pow(s, 17) + 190037146145 * Math.Pow(s, 19) - 92303756699.0 * Math.Pow(s, 21) + 19336360099 * Math.Pow(s, 23));
                LegArrEx[27, 5] = (1.0 / 524288) * 3810254175 * Math.Pow(c, 5) * (-323 + 117249 * Math.Pow(s, 2) - 6839525 * Math.Pow(s, 4) + 151837455 * Math.Pow(s, 6) - 1691903070 * Math.Pow(s, 8) + 10790581802 * Math.Pow(s, 10) - 42181365226 * Math.Pow(s, 12) + 104294584350 * Math.Pow(s, 14) - 163394848815 * Math.Pow(s, 16) + 156987207685 * Math.Pow(s, 18) - 84277343073.0 * Math.Pow(s, 20) + 19336360099 * Math.Pow(s, 22));
                LegArrEx[27, 6] = (1.0 / 262144) * 41912795925 * Math.Pow(c, 6) * (10659 * s - 1243550 * Math.Pow(s, 3) + 41410215 * Math.Pow(s, 5) - 615237480 * Math.Pow(s, 7) + 4904809910 * Math.Pow(s, 9) - 23008017396 * Math.Pow(s, 11) + 66369280950 * Math.Pow(s, 13) - 118832617320 * Math.Pow(s, 15) + 128444079015 * Math.Pow(s, 17) - 76615766430 * Math.Pow(s, 19) + 19336360099 * Math.Pow(s, 21));
                LegArrEx[27, 7] = (1.0 / 262144) * 2137552592175.0 * Math.Pow(c, 7) * (209 - 73150 * Math.Pow(s, 2) + 4059825 * Math.Pow(s, 4) - 84444360 * Math.Pow(s, 6) + 865554690 * Math.Pow(s, 8) - 4962513556 * Math.Pow(s, 10) + 16917659850 * Math.Pow(s, 12) - 34950769800 * Math.Pow(s, 14) + 42814693005 * Math.Pow(s, 16) - 28543128670 * Math.Pow(s, 18) + 7962030629 * Math.Pow(s, 20));
                LegArrEx[27, 8] = (1.0 / 65536) * 74814340726125.0 * Math.Pow(c, 8) * (-1045 * s + 115995 * Math.Pow(s, 3) - 3619044 * Math.Pow(s, 5) + 49460268 * Math.Pow(s, 7) - 354465254 * Math.Pow(s, 9) + 1450085130 * Math.Pow(s, 11) - 3495076980 * Math.Pow(s, 13) + 4893107772 * Math.Pow(s, 15) - 3669830829 * Math.Pow(s, 17) + 1137432947 * Math.Pow(s, 19));
                LegArrEx[27, 9] = ((1421472473796375.0 * Math.Pow(c, 9) * (-55 + 18315 * Math.Pow(s, 2) - 952380 * Math.Pow(s, 4) + 18222204 * Math.Pow(s, 6) - 167904594 * Math.Pow(s, 8) + 839522970 * Math.Pow(s, 10) - 2391368460 * Math.Pow(s, 12) + 3862979820 * Math.Pow(s, 14) - 3283532847 * Math.Pow(s, 16) + 1137432947 * Math.Pow(s, 18))) / 65536);
                LegArrEx[27, 10] = ((473350333774192875.0 * Math.Pow(c, 10) * (55 * s - 5720 * Math.Pow(s, 3) + 164164 * Math.Pow(s, 5) - 2016872 * Math.Pow(s, 7) + 12605450 * Math.Pow(s, 9) - 43087720 * Math.Pow(s, 11) + 81203780 * Math.Pow(s, 13) - 78883672 * Math.Pow(s, 15) + 30741431 * Math.Pow(s, 17))) / 32768);
                LegArrEx[27, 11] = ((473350333774192875.0 * Math.Pow(c, 11) * (55 - 17160 * Math.Pow(s, 2) + 820820 * Math.Pow(s, 4) - 14118104 * Math.Pow(s, 6) + 113449050 * Math.Pow(s, 8) - 473964920 * Math.Pow(s, 10) + 1055649140 * Math.Pow(s, 12) - 1183255080 * Math.Pow(s, 14) + 522604327 * Math.Pow(s, 16))) / 32768);
                LegArrEx[27, 12] = (473350333774192875.0 * Math.Pow(c, 12) * (-2145 * s + 205205 * Math.Pow(s, 3) - 5294289 * Math.Pow(s, 5) + 56724525 * Math.Pow(s, 7) - 296228075 * Math.Pow(s, 9) + 791736855 * Math.Pow(s, 11) - 1035348195 * Math.Pow(s, 13) + 522604327 * Math.Pow(s, 15))) / 2048;
                LegArrEx[27, 13] = ((7100255006612893125.0 * Math.Pow(c, 13) * (-143 + 41041 * Math.Pow(s, 2) - 1764763 * Math.Pow(s, 4) + 26471445 * Math.Pow(s, 6) - 177736845 * Math.Pow(s, 8) + 580607027 * Math.Pow(s, 10) - 897301769 * Math.Pow(s, 12) + 522604327 * Math.Pow(s, 14))) / 2048);
                LegArrEx[27, 14] = ((291110455271128618125.0 * Math.Pow(c, 14) * (1001 * s - 86086 * Math.Pow(s, 3) + 1936935 * Math.Pow(s, 5) - 17340180 * Math.Pow(s, 7) + 70805735 * Math.Pow(s, 9) - 131312454 * Math.Pow(s, 11) + 89225129 * Math.Pow(s, 13))) / 1024);
                LegArrEx[27, 15] = ((26491051429672704249375.0 * Math.Pow(c, 15) * (11 - 2838 * Math.Pow(s, 2) + 106425 * Math.Pow(s, 4) - 1333860 * Math.Pow(s, 6) + 7002765 * Math.Pow(s, 8) - 15872934 * Math.Pow(s, 10) + 12746447 * Math.Pow(s, 12))) / 1024);
                LegArrEx[27, 16] = 3417345634427778848169375.0 / 256 * Math.Pow(c, 16) * (-11 * s + 825 * Math.Pow(s, 3) - 15510 * Math.Pow(s, 5) + 108570 * Math.Pow(s, 7) - 307615 * Math.Pow(s, 9) + 296429 * Math.Pow(s, 11));
                LegArrEx[27, 17] = (37590801978705567329863125.0 / 256) * Math.Pow(c, 17) * (-1 + 225 * Math.Pow(s, 2) - 7050 * Math.Pow(s, 4) + 69090 * Math.Pow(s, 6) - 251685 * Math.Pow(s, 8) + 296429 * Math.Pow(s, 10));
                LegArrEx[27, 18] = (187954009893527836649315625.0 / 128) * Math.Pow(c, 18) * (45 * s - 2820 * Math.Pow(s, 3) + 41454 * Math.Pow(s, 5) - 201348 * Math.Pow(s, 7) + 296429 * Math.Pow(s, 9));
                LegArrEx[27, 19] = (1691586089041750529843840625.0 / 128) * Math.Pow(c, 19) * (5 - 940 * Math.Pow(s, 2) + 23030 * Math.Pow(s, 4) - 156604 * Math.Pow(s, 6) + 296429 * Math.Pow(s, 8));
                LegArrEx[27, 20] = 79504546184962274902660509375.0 / 16 * Math.Pow(c, 20) * (-5 * s + 245 * Math.Pow(s, 3) - 2499 * Math.Pow(s, 5) + 6307 * Math.Pow(s, 7));
                LegArrEx[27, 21] = (79504546184962274902660509375.0 / 16) * Math.Pow(c, 21) * (-5 + 735 * Math.Pow(s, 2) - 12495 * Math.Pow(s, 4) + 44149 * Math.Pow(s, 6));
                LegArrEx[27, 22] = (11687168289189454410691094878125.0 / 8) * Math.Pow(c, 22) * (5 * s - 170 * Math.Pow(s, 3) + 901 * Math.Pow(s, 5));
                LegArrEx[27, 23] = (58435841445947272053455474390625.0 / 8) * Math.Pow(c, 23) * (1 - 102 * Math.Pow(s, 2) + 901 * Math.Pow(s, 4));
                LegArrEx[27, 24] = 993409304581103624908743064640625.0 / 2 * Math.Pow(c, 24) * (-3 * s + 53 * Math.Pow(s, 3));
                LegArrEx[27, 25] = (2980227913743310874726229193921875.0 / 2) * Math.Pow(c, 25) * (-1 + 53 * Math.Pow(s, 2));
                LegArrEx[27, 26] = 157952079428395476360490147277859375.0 * s * Math.Pow(c, 26);
                LegArrEx[27, 27] = 157952079428395476360490147277859375.0 * Math.Pow(c, 27);

                LegArrEx[28, 0] = (5014575 - 2035917450.0 * Math.Pow(s, 2) + 136745788725.0 * Math.Pow(s, 4) - 3610088822340.0 * Math.Pow(s, 6) + 49638721307175.0 * Math.Pow(s, 8) - 408140597414550.0 * Math.Pow(s, 10) + 2170565904431925.0 * Math.Pow(s, 12) - 7823578204985400.0 * Math.Pow(s, 14) + 19624141997505045.0 * Math.Pow(s, 16) - 34630838819126550.0 * Math.Pow(s, 18) + 42832879592077575.0 * Math.Pow(s, 20) - 36343049350853700.0 * Math.Pow(s, 22) + 20146690401016725.0 * Math.Pow(s, 24) - 6570920561562378.0 * Math.Pow(s, 26) + 956086325095055.0 * Math.Pow(s, 28)) / 33554432;
                LegArrEx[28, 1] = (1.0 / 8388608) * 203 * c * (-5014575 * s + 673624575 * Math.Pow(s, 3) - 26675533170.0 * Math.Pow(s, 5) + 489051441450.0 * Math.Pow(s, 7) - 5026362037125.0 * Math.Pow(s, 9) + 32077328636925.0 * Math.Pow(s, 11) - 134889279396300.0 * Math.Pow(s, 13) + 386682600936060.0 * Math.Pow(s, 15) - 767678693034825 * Math.Pow(s, 17) + 1054997034287625 * Math.Pow(s, 19) - 984663898668450.0 * Math.Pow(s, 21) + 595468681803450.0 * Math.Pow(s, 23) - 210398934237219.0 * Math.Pow(s, 25) + 32968493968795.0 * Math.Pow(s, 27));
                LegArrEx[28, 2] = (1.0 / 8388608) * 27405 * c * c * (-37145 + 14969435 * Math.Pow(s, 2) - 987982710 * Math.Pow(s, 4) + 25358222890.0 * Math.Pow(s, 6) - 335090802475 * Math.Pow(s, 8) + 2613708259305 * Math.Pow(s, 10) - 12989338015940.0 * Math.Pow(s, 12) + 42964733437340.0 * Math.Pow(s, 14) - 96670650234015 * Math.Pow(s, 16) + 148481064084925.0 * Math.Pow(s, 18) - 153169939792870.0 * Math.Pow(s, 20) + 101450219862810.0 * Math.Pow(s, 22) - 38962765599485.0 * Math.Pow(s, 24) + 6593698793759.0 * Math.Pow(s, 26));
                LegArrEx[28, 3] = (1.0 / 4194304) * 11044215 * Math.Pow(c, 3) * (37145 * s - 4903140 * Math.Pow(s, 3) + 188770890 * Math.Pow(s, 5) - 3325963300 * Math.Pow(s, 7) + 32428142175 * Math.Pow(s, 9) - 193389647880 * Math.Pow(s, 11) + 746285692460.0 * Math.Pow(s, 13) - 1919020352040 * Math.Pow(s, 15) + 3315954284775 * Math.Pow(s, 17) - 3800742922900.0 * Math.Pow(s, 19) + 2769112700970 * Math.Pow(s, 21) - 1160181605940 * Math.Pow(s, 23) + 212699961089.0 * Math.Pow(s, 25));
                LegArrEx[28, 4] = (1.0 / 4194304) * 55221075 * Math.Pow(c, 4) * (7429 - 2941884 * Math.Pow(s, 2) + 188770890 * Math.Pow(s, 4) - 4656348620 * Math.Pow(s, 6) + 58370655915.0 * Math.Pow(s, 8) - 425457225336 * Math.Pow(s, 10) + 1940342800396.0 * Math.Pow(s, 12) - 5757061056120 * Math.Pow(s, 14) + 11274244568235 * Math.Pow(s, 16) - 14442823107020 * Math.Pow(s, 18) + 11630273344074 * Math.Pow(s, 20) - 5336835387324 * Math.Pow(s, 22) + 1063499805445.0 * Math.Pow(s, 24));
                LegArrEx[28, 5] = (1.0 / 524288) * 1822295475 * Math.Pow(c, 5) * (-22287 * s + 2860165 * Math.Pow(s, 3) - 105826105 * Math.Pow(s, 5) + 1768807755 * Math.Pow(s, 7) - 16115803990 * Math.Pow(s, 9) + 88197400018 * Math.Pow(s, 11) - 305298692370 * Math.Pow(s, 13) + 683287549590 * Math.Pow(s, 15) - 984737939115 * Math.Pow(s, 17) + 881081313945 * Math.Pow(s, 19) - 444736282277 * Math.Pow(s, 21) + 96681800495 * Math.Pow(s, 23));
                LegArrEx[28, 6] = (1.0 / 524288) * 712517530725 * Math.Pow(c, 6) * (-57 + 21945 * Math.Pow(s, 2) - 1353275 * Math.Pow(s, 4) + 31666635 * Math.Pow(s, 6) - 370952010 * Math.Pow(s, 8) + 2481256778 * Math.Pow(s, 10) - 10150595910 * Math.Pow(s, 12) + 26213077350 * Math.Pow(s, 14) - 42814693005 * Math.Pow(s, 16) + 42814693005 * Math.Pow(s, 18) - 23886091887 * Math.Pow(s, 20) + 5687164735 * Math.Pow(s, 22));
                LegArrEx[28, 7] = (1.0 / 262144) * 3562587653625 * Math.Pow(c, 7) * (4389 * s - 541310 * Math.Pow(s, 3) + 18999981 * Math.Pow(s, 5) - 296761608 * Math.Pow(s, 7) + 2481256778 * Math.Pow(s, 9) - 12180715092 * Math.Pow(s, 11) + 36698308290 * Math.Pow(s, 13) - 68503508808 * Math.Pow(s, 15) + 77066447409 * Math.Pow(s, 17) - 47772183774 * Math.Pow(s, 19) + 12511762417 * Math.Pow(s, 21));
                LegArrEx[28, 8] = (1.0 / 262144) * 74814340726125 * Math.Pow(c, 8) * (209 - 77330 * Math.Pow(s, 2) + 4523805 * Math.Pow(s, 4) - 98920536 * Math.Pow(s, 6) + 1063395762 * Math.Pow(s, 8) - 6380374572 * Math.Pow(s, 10) + 22718000370 * Math.Pow(s, 12) - 48931077720 * Math.Pow(s, 14) + 62387124093 * Math.Pow(s, 16) - 43222451986 * Math.Pow(s, 18) + 12511762417 * Math.Pow(s, 20));
                LegArrEx[28, 9] = (1.0 / 65536) * 2768130606866625 * Math.Pow(c, 9) * (-1045 * s + 122265 * Math.Pow(s, 3) - 4010292 * Math.Pow(s, 5) + 57480852 * Math.Pow(s, 7) - 431106390 * Math.Pow(s, 9) + 1842000030 * Math.Pow(s, 11) - 4628615460 * Math.Pow(s, 13) + 6744553956 * Math.Pow(s, 15) - 5256784701 * Math.Pow(s, 17) + 1690778705 * Math.Pow(s, 19));
                LegArrEx[28, 10] = (1.0 / 65536) * 52594481530465875 * Math.Pow(c, 10) * (-55 + 19305 * Math.Pow(s, 2) - 1055340 * Math.Pow(s, 4) + 21177156 * Math.Pow(s, 6) - 204208290 * Math.Pow(s, 8) + 1066421070 * Math.Pow(s, 10) - 3166947420 * Math.Pow(s, 12) + 5324647860 * Math.Pow(s, 14) - 4703438943 * Math.Pow(s, 16) + 1690778705 * Math.Pow(s, 18));
                LegArrEx[28, 11] = ((473350333774192875.0 * Math.Pow(c, 11) * (2145 * s - 234520 * Math.Pow(s, 3) + 7059052 * Math.Pow(s, 5) - 90759240 * Math.Pow(s, 7) + 592456150 * Math.Pow(s, 9) - 2111298280 * Math.Pow(s, 11) + 4141392780 * Math.Pow(s, 13) - 4180834616 * Math.Pow(s, 15) + 1690778705 * Math.Pow(s, 17))) / 32768);
                LegArrEx[28, 12] = (2366751668870964375.0 * Math.Pow(c, 12) * (429 - 140712 * Math.Pow(s, 2) + 7059052 * Math.Pow(s, 4) - 127062936 * Math.Pow(s, 6) + 1066421070 * Math.Pow(s, 8) - 4644856216 * Math.Pow(s, 10) + 10767621228 * Math.Pow(s, 12) - 12542503848 * Math.Pow(s, 14) + 5748647597 * Math.Pow(s, 16))) / 32768;
                LegArrEx[28, 13] = ((97036818423709539375.0 * Math.Pow(c, 13) * (-429 * s + 43043 * Math.Pow(s, 3) - 1162161 * Math.Pow(s, 5) + 13005135 * Math.Pow(s, 7) - 70805735 * Math.Pow(s, 9) + 196968681 * Math.Pow(s, 11) - 267675387 * Math.Pow(s, 13) + 140210917 * Math.Pow(s, 15))) / 2048);
                LegArrEx[28, 14] = ((291110455271128618125.0 * Math.Pow(c, 14) * (-143 + 43043 * Math.Pow(s, 2) - 1936935 * Math.Pow(s, 4) + 30345315 * Math.Pow(s, 6) - 212417205 * Math.Pow(s, 8) + 722218497 * Math.Pow(s, 10) - 1159926677 * Math.Pow(s, 12) + 701054585 * Math.Pow(s, 14))) / 2048);
                LegArrEx[28, 15] = ((87624247036609714055625.0 * Math.Pow(c, 15) * (143 * s - 12870 * Math.Pow(s, 3) + 302445 * Math.Pow(s, 5) - 2822820 * Math.Pow(s, 7) + 11996985 * Math.Pow(s, 9) - 23121462 * Math.Pow(s, 11) + 16303595 * Math.Pow(s, 13))) / 1024);
                LegArrEx[28, 16] = (12530267326235189109954375.0 * Math.Pow(c, 16) * (1 - 270 * Math.Pow(s, 2) + 10575 * Math.Pow(s, 4) - 138180 * Math.Pow(s, 6) + 755055 * Math.Pow(s, 8) - 1778574 * Math.Pow(s, 10) + 1482145 * Math.Pow(s, 12))) / 1024;
                LegArrEx[28, 17] = (187954009893527836649315625.0 / 256) * Math.Pow(c, 17) * (-9 * s + 705 * Math.Pow(s, 3) - 13818 * Math.Pow(s, 5) + 100674 * Math.Pow(s, 7) - 296429 * Math.Pow(s, 9) + 296429 * Math.Pow(s, 11));
                LegArrEx[28, 18] = (187954009893527836649315625.0 / 256) * Math.Pow(c, 18) * (-9 + 2115 * Math.Pow(s, 2) - 69090 * Math.Pow(s, 4) + 704718 * Math.Pow(s, 6) - 2667861 * Math.Pow(s, 8) + 3260719 * Math.Pow(s, 10));
                LegArrEx[28, 19] = (8833838464995808322517834375.0 / 128) * Math.Pow(c, 19) * (45 * s - 2940 * Math.Pow(s, 3) + 44982 * Math.Pow(s, 5) - 227052 * Math.Pow(s, 7) + 346885 * Math.Pow(s, 9));
                LegArrEx[28, 20] = 79504546184962274902660509375.0 / 128 * Math.Pow(c, 20) * (5 - 980 * Math.Pow(s, 2) + 24990 * Math.Pow(s, 4) - 176596 * Math.Pow(s, 6) + 346885 * Math.Pow(s, 8));
                LegArrEx[28, 21] = (556531823294735924318623565625.0 / 16) * Math.Pow(c, 21) * (-35 * s + 1785 * Math.Pow(s, 3) - 18921 * Math.Pow(s, 5) + 49555 * Math.Pow(s, 7));
                LegArrEx[28, 22] = (19478613815315757351151824796875.0 / 16) * Math.Pow(c, 22) * (-1 + 153 * Math.Pow(s, 2) - 2703 * Math.Pow(s, 4) + 9911 * Math.Pow(s, 6));
                LegArrEx[28, 23] = (993409304581103624908743064640625.0 / 8) * Math.Pow(c, 23) * (3 * s - 106 * Math.Pow(s, 3) + 583 * Math.Pow(s, 5));
                LegArrEx[28, 24] = 993409304581103624908743064640625.0 / 8 * Math.Pow(c, 24) * (3 - 318 * Math.Pow(s, 2) + 2915 * Math.Pow(s, 4));
                LegArrEx[28, 25] = (52650693142798492120163382425953125.0 / 2) * Math.Pow(c, 25) * (-3 * s + 55 * Math.Pow(s, 3));
                LegArrEx[28, 26] = (157952079428395476360490147277859375.0 / 2) * Math.Pow(c, 26) * (-1 + 55 * Math.Pow(s, 2));
                LegArrEx[28, 27] = 8687364368561751199826958100282265625.0 * s * Math.Pow(c, 27);
                LegArrEx[28, 28] = 8687364368561751199826958100282265625.0 * Math.Pow(c, 28);

                LegArrEx[29, 0] = (145422675.0 * s - 21037813650.0 * Math.Pow(s, 3) + 902522205585.0 * Math.Pow(s, 5) - 18050444111700.0 * Math.Pow(s, 7) + 204070298707275 * Math.Pow(s, 9) - 1447043936287950 * Math.Pow(s, 11) + 6845630929362225 * Math.Pow(s, 13) - 22427590854291480.0 * Math.Pow(s, 15) + 51946258228689825.0 * Math.Pow(s, 17) - 85665759184155150.0 * Math.Pow(s, 19) + 99943385714847675.0 * Math.Pow(s, 21) - 80586761604066900.0 * Math.Pow(s, 23) + 42710983650155457.0 * Math.Pow(s, 25) - 13385208551330770.0 * Math.Pow(s, 27) + 1879204156221315.0 * Math.Pow(s, 29)) / 33554432.0;
                LegArrEx[29, 1] = (1.0 / 33554432) * 435 * c * (334305 - 145088370 * Math.Pow(s, 2) + 10373818455 * Math.Pow(s, 4) - 290466916740 * Math.Pow(s, 6) + 4222144111185 * Math.Pow(s, 8) - 36591915630270 * Math.Pow(s, 10) + 204582073751055.0 * Math.Pow(s, 12) - 773365201872120.0 * Math.Pow(s, 14) + 2030083654914315.0 * Math.Pow(s, 16) - 3741722814940110.0 * Math.Pow(s, 18) + 4824853103475405.0 * Math.Pow(s, 20) - 4260909234238020.0 * Math.Pow(s, 22) + 2454654232767555.0 * Math.Pow(s, 24) - 830806048013634.0 * Math.Pow(s, 26) + 125280277081421.0 * Math.Pow(s, 28));
                LegArrEx[29, 2] = (1.0 / 8388608) * 94395 * c * c * (-334305 * s + 47805615 * Math.Pow(s, 3) - 2007835830 * Math.Pow(s, 5) + 38913770610 * Math.Pow(s, 7) - 421565848275 * Math.Pow(s, 9) + 2828323600245 * Math.Pow(s, 11) - 12473632288260 * Math.Pow(s, 13) + 37420896864780.0 * Math.Pow(s, 15) - 77593330263735.0 * Math.Pow(s, 17) + 111171730494825.0 * Math.Pow(s, 19) - 107995395337830.0 * Math.Pow(s, 21) + 67870623947490 * Math.Pow(s, 23) - 24885895447413.0 * Math.Pow(s, 25) + 4041299260691.0 * Math.Pow(s, 27));
                LegArrEx[29, 3] = (1.0 / 8388608) * 849555.0 * Math.Pow(c, 3) * (-37145 + 15935205 * Math.Pow(s, 2) - 1115464350 * Math.Pow(s, 4) + 30266266030 * Math.Pow(s, 6) - 421565848275 * Math.Pow(s, 8) + 3456839955855 * Math.Pow(s, 10) - 18017468860820.0 * Math.Pow(s, 12) + 62368161441300 * Math.Pow(s, 14) - 146565179387055.0 * Math.Pow(s, 16) + 234695875489075.0 * Math.Pow(s, 18) - 251989255788270 * Math.Pow(s, 20) + 173447150088030 * Math.Pow(s, 22) - 69127487353925.0 * Math.Pow(s, 24) + 12123897782073 * Math.Pow(s, 26));
                LegArrEx[29, 4] = (1.0 / 4194304) * 364459095.0 * Math.Pow(c, 4) * (37145 * s - 5200300 * Math.Pow(s, 3) + 211652210 * Math.Pow(s, 5) - 3930683900 * Math.Pow(s, 7) + 40289509975 * Math.Pow(s, 9) - 251992571480 * Math.Pow(s, 11) + 1017662307900 * Math.Pow(s, 13) - 2733150198360 * Math.Pow(s, 15) + 4923689695575.0 * Math.Pow(s, 17) - 5873875426300.0 * Math.Pow(s, 19) + 4447362822770 * Math.Pow(s, 21) - 1933636009900 * Math.Pow(s, 23) + 367390841881 * Math.Pow(s, 25));
                LegArrEx[29, 5] = (1.0 / 4194304) * 30979023075.0 * Math.Pow(c, 5) * (437 - 183540 * Math.Pow(s, 2) + 12450130 * Math.Pow(s, 4) - 323703380 * Math.Pow(s, 6) + 4265948115 * Math.Pow(s, 8) - 32610803368 * Math.Pow(s, 10) + 155642470620 * Math.Pow(s, 12) - 482320623240 * Math.Pow(s, 14) + 984737939115 * Math.Pow(s, 16) - 1312983918820.0 * Math.Pow(s, 18) + 1098760226802 * Math.Pow(s, 20) - 523219155620 * Math.Pow(s, 22) + 108056129965 * Math.Pow(s, 24));
                LegArrEx[29, 6] = (1.0 / 524288) * 154895115375.0 * Math.Pow(c, 6) * (-9177 * s + 1245013 * Math.Pow(s, 3) - 48555507 * Math.Pow(s, 5) + 853189623 * Math.Pow(s, 7) - 8152700842 * Math.Pow(s, 9) + 46692741186 * Math.Pow(s, 11) - 168812218134 * Math.Pow(s, 13) + 393895175646 * Math.Pow(s, 15) - 590842763469 * Math.Pow(s, 17) + 549380113401 * Math.Pow(s, 19) - 287770535591 * Math.Pow(s, 21) + 64833677979 * Math.Pow(s, 23));
                LegArrEx[29, 7] = (1.0 / 524288) * 10687762960875.0 * Math.Pow(c, 7) * (-133 + 54131 * Math.Pow(s, 2) - 3518515 * Math.Pow(s, 4) + 86555469 * Math.Pow(s, 6) - 1063395762 * Math.Pow(s, 8) + 7443770334 * Math.Pow(s, 10) - 31805200518 * Math.Pow(s, 12) + 85629386010 * Math.Pow(s, 14) - 145569956217 * Math.Pow(s, 16) + 151278581951 * Math.Pow(s, 18) - 87582336919 * Math.Pow(s, 20) + 21611225993 * Math.Pow(s, 22));
                LegArrEx[29, 8] = (1.0 / 262144) * 395447229552375.0 * Math.Pow(c, 8) * (1463 * s - 190190 * Math.Pow(s, 3) + 7018011 * Math.Pow(s, 5) - 114961704 * Math.Pow(s, 7) + 1005914910 * Math.Pow(s, 9) - 5157600084 * Math.Pow(s, 11) + 16200154110 * Math.Pow(s, 13) - 31474585128 * Math.Pow(s, 15) + 36797492907 * Math.Pow(s, 17) - 23670901870 * Math.Pow(s, 19) + 6424959079 * Math.Pow(s, 21));
                LegArrEx[29, 9] = (1.0 / 262144) * 52594481530465875.0 * Math.Pow(c, 9) * (11 - 4290 * Math.Pow(s, 2) + 263835 * Math.Pow(s, 4) - 6050616 * Math.Pow(s, 6) + 68069430 * Math.Pow(s, 8) - 426568428 * Math.Pow(s, 10) + 1583473710 * Math.Pow(s, 12) - 3549765240 * Math.Pow(s, 14) + 4703438943 * Math.Pow(s, 16) - 3381557410 * Math.Pow(s, 18) + 1014467223 * Math.Pow(s, 20));
                LegArrEx[29, 10] = (1.0 / 65536) * 157783444591397625.0 * Math.Pow(c, 10) * (-715 * s + 87945 * Math.Pow(s, 3) - 3025308 * Math.Pow(s, 5) + 45379620 * Math.Pow(s, 7) - 355473690 * Math.Pow(s, 9) + 1583473710 * Math.Pow(s, 11) - 4141392780 * Math.Pow(s, 13) + 6271251924 * Math.Pow(s, 15) - 5072336115 * Math.Pow(s, 17) + 1690778705 * Math.Pow(s, 19));
                LegArrEx[29, 11] = (1.0 / 65536) * 788917222956988125.0 * Math.Pow(c, 11) * (-143 + 52767 * Math.Pow(s, 2) - 3025308 * Math.Pow(s, 4) + 63531468 * Math.Pow(s, 6) - 639852642 * Math.Pow(s, 8) + 3483642162 * Math.Pow(s, 10) - 10767621228 * Math.Pow(s, 12) + 18813755772 * Math.Pow(s, 14) - 17245942791 * Math.Pow(s, 16) + 6424959079 * Math.Pow(s, 18));
                LegArrEx[29, 12] = (97036818423709539375.0 * Math.Pow(c, 12) * (429 * s - 49192 * Math.Pow(s, 3) + 1549548 * Math.Pow(s, 5) - 20808216 * Math.Pow(s, 7) + 141611470 * Math.Pow(s, 9) - 525249816 * Math.Pow(s, 11) + 1070701548 * Math.Pow(s, 13) - 1121687336 * Math.Pow(s, 15) + 470118957 * Math.Pow(s, 17))) / 32768;
                LegArrEx[29, 13] = ((291110455271128618125.0 * Math.Pow(c, 13) * (143 - 49192 * Math.Pow(s, 2) + 2582580 * Math.Pow(s, 4) - 48552504 * Math.Pow(s, 6) + 424834410 * Math.Pow(s, 8) - 1925915992 * Math.Pow(s, 10) + 4639706708 * Math.Pow(s, 12) - 5608436680 * Math.Pow(s, 14) + 2664007423 * Math.Pow(s, 16))) / 32768);
                LegArrEx[29, 14] = ((12517749576658530579375.0 * Math.Pow(c, 14) * (-143 * s + 15015 * Math.Pow(s, 3) - 423423 * Math.Pow(s, 5) + 4939935 * Math.Pow(s, 7) - 27992965 * Math.Pow(s, 9) + 80925117 * Math.Pow(s, 11) - 114125165 * Math.Pow(s, 13) + 61953661 * Math.Pow(s, 15))) / 2048);
                LegArrEx[29, 15] = ((137695245343243836373125.0 * Math.Pow(c, 15) * (-13 + 4095 * Math.Pow(s, 2) - 192465 * Math.Pow(s, 4) + 3143595 * Math.Pow(s, 6) - 22903335 * Math.Pow(s, 8) + 80925117 * Math.Pow(s, 10) - 134875195 * Math.Pow(s, 12) + 84482265 * Math.Pow(s, 14))) / 2048);
                LegArrEx[29, 16] = (14458000761040602819178125.0 * Math.Pow(c, 16) * (39 * s - 3666 * Math.Pow(s, 3) + 89817 * Math.Pow(s, 5) - 872508 * Math.Pow(s, 7) + 3853577 * Math.Pow(s, 9) - 7707154 * Math.Pow(s, 11) + 5632151 * Math.Pow(s, 13))) / 1024;
                LegArrEx[29, 17] = ((187954009893527836649315625.0 * Math.Pow(c, 17) * (3 - 846 * Math.Pow(s, 2) + 34545 * Math.Pow(s, 4) - 469812 * Math.Pow(s, 6) + 2667861 * Math.Pow(s, 8) - 6521438 * Math.Pow(s, 10) + 5632151 * Math.Pow(s, 12))) / 1024);
                LegArrEx[29, 18] = (8833838464995808322517834375.0 / 256) * Math.Pow(c, 18) * (-9 * s + 735 * Math.Pow(s, 3) - 14994 * Math.Pow(s, 5) + 113526 * Math.Pow(s, 7) - 346885 * Math.Pow(s, 9) + 359499 * Math.Pow(s, 11));
                LegArrEx[29, 19] = (26501515394987424967553503125.0 / 256) * Math.Pow(c, 19) * (-3 + 735 * Math.Pow(s, 2) - 24990 * Math.Pow(s, 4) + 264894 * Math.Pow(s, 6) - 1040655 * Math.Pow(s, 8) + 1318163 * Math.Pow(s, 10));
                LegArrEx[29, 20] = 185510607764911974772874521875.0 / 128 * Math.Pow(c, 20) * (105 * s - 7140 * Math.Pow(s, 3) + 113526 * Math.Pow(s, 5) - 594660 * Math.Pow(s, 7) + 941545 * Math.Pow(s, 9));
                LegArrEx[29, 21] = (2782659116473679621593117828125.0 / 128) * Math.Pow(c, 21) * (7 - 1428 * Math.Pow(s, 2) + 37842 * Math.Pow(s, 4) - 277508 * Math.Pow(s, 6) + 564927 * Math.Pow(s, 8));
                LegArrEx[29, 22] = (141915614940157660701249009234375.0 / 16) * Math.Pow(c, 22) * (-7 * s + 371 * Math.Pow(s, 3) - 4081 * Math.Pow(s, 5) + 11077 * Math.Pow(s, 7));
                LegArrEx[29, 23] = (993409304581103624908743064640625.0 / 16) * Math.Pow(c, 23) * (-1 + 159 * Math.Pow(s, 2) - 2915 * Math.Pow(s, 4) + 11077 * Math.Pow(s, 6));
                LegArrEx[29, 24] = 52650693142798492120163382425953125.0 / 8 * Math.Pow(c, 24) * (3 * s - 110 * Math.Pow(s, 3) + 627 * Math.Pow(s, 5));
                LegArrEx[29, 25] = (157952079428395476360490147277859375.0 / 8) * Math.Pow(c, 25) * (1 - 110 * Math.Pow(s, 2) + 1045 * Math.Pow(s, 4));
                LegArrEx[29, 26] = (8687364368561751199826958100282265625.0 / 2) * Math.Pow(c, 26) * (-s + 19 * Math.Pow(s, 3));
                LegArrEx[29, 27] = (8687364368561751199826958100282265625.0 / 2) * Math.Pow(c, 27) * (-1 + 57 * Math.Pow(s, 2));
                LegArrEx[29, 28] = 495179769008019818390136611716089140625.0 * s * Math.Pow(c, 28);
                LegArrEx[29, 29] = 495179769008019818390136611716089140625.0 * Math.Pow(c, 29);

                LegArrEx[30, 0] = (-9694845 + 4508102925.0 * Math.Pow(s, 2) - 347123925225.0 * Math.Pow(s, 4) + 10529425731825.0 * Math.Pow(s, 6) - 166966608033225.0 * Math.Pow(s, 8) + 1591748329916745 * Math.Pow(s, 10) - 9888133564634325.0 * Math.Pow(s, 12) + 42051732851796525.0 * Math.Pow(s, 14) - 126155198555389575.0 * Math.Pow(s, 16) + 271274904083157975.0 * Math.Pow(s, 18) - 419762220002360235.0 * Math.Pow(s, 20) + 463373879223384675.0 * Math.Pow(s, 22) - 355924863751295475.0 * Math.Pow(s, 24) + 180700315442965395.0 * Math.Pow(s, 26) - 54496920530418135.0 * Math.Pow(s, 28) + 7391536347803839.0 * Math.Pow(s, 30)) / 67108864;
                LegArrEx[30, 1] = (1.0 / 33554432) * 465 * c * (9694845 * s - 1493006130 * Math.Pow(s, 3) + 67931778915 * Math.Pow(s, 5) - 1436271897060.0 * Math.Pow(s, 7) + 17115573439965 * Math.Pow(s, 9) - 127588820188830.0 * Math.Pow(s, 11) + 633036838629195.0 * Math.Pow(s, 13) - 2170412018157240.0 * Math.Pow(s, 15) + 5250482014512735 * Math.Pow(s, 17) - 9027144516179790.0 * Math.Pow(s, 19) + 10961532626789745.0 * Math.Pow(s, 21) - 9185157774226980.0 * Math.Pow(s, 23) + 5051836775824839.0 * Math.Pow(s, 25) - 1640767499840546.0 * Math.Pow(s, 27) + 238436656380769.0 * Math.Pow(s, 29));
                LegArrEx[30, 2] = (1.0 / 33554432.0) * 13485 * c * c * (334305 - 154448910 * Math.Pow(s, 2) + 11712375675 * Math.Pow(s, 4) - 346686319980 * Math.Pow(s, 6) + 5311729688265.0 * Math.Pow(s, 8) - 48395759381970.0 * Math.Pow(s, 10) + 283775134557915.0 * Math.Pow(s, 12) - 1122626905943400.0 * Math.Pow(s, 14) + 3077868767128155.0 * Math.Pow(s, 16) - 5914336062324690.0 * Math.Pow(s, 18) + 7937661557330505 * Math.Pow(s, 20) - 7284780303697260.0 * Math.Pow(s, 22) + 4355031703297275 * Math.Pow(s, 24) - 1527611120541198.0 * Math.Pow(s, 26) + 238436656380769.0 * Math.Pow(s, 28));
                LegArrEx[30, 3] = (1.0 / 8388608) * 1038345 * Math.Pow(c, 3) * (-1002915 * s + 152108775 * Math.Pow(s, 3) - 6753629610 * Math.Pow(s, 5) + 137967004890 * Math.Pow(s, 7) - 1571290889025 * Math.Pow(s, 9) + 11056174073685 * Math.Pow(s, 11) - 51028495724700.0 * Math.Pow(s, 13) + 159889286604060 * Math.Pow(s, 15) - 345643016629365 * Math.Pow(s, 17) + 515432568657825.0 * Math.Pow(s, 19) - 520341450264090.0 * Math.Pow(s, 21) + 339353119737450.0 * Math.Pow(s, 23) - 128954185500231.0 * Math.Pow(s, 25) + 21676059670979.0 * Math.Pow(s, 27));
                LegArrEx[30, 4] = (1.0 / 8388608) * 476600355 * Math.Pow(c, 4) * (-2185 + 994175 * Math.Pow(s, 2) - 73568950 * Math.Pow(s, 4) + 2104071970 * Math.Pow(s, 6) - 30809625275 * Math.Pow(s, 8) + 264962777365 * Math.Pow(s, 10) - 1445251512900 * Math.Pow(s, 12) + 5225140085100 * Math.Pow(s, 14) - 12801593208495 * Math.Pow(s, 16) + 21335988680825.0 * Math.Pow(s, 18) - 23806471580710.0 * Math.Pow(s, 20) + 17004622557650 * Math.Pow(s, 22) - 7023648447725.0 * Math.Pow(s, 24) + 1275062333587 * Math.Pow(s, 26));
                LegArrEx[30, 5] = (1.0 / 4194304) * 6195804615 * Math.Pow(c, 5) * (76475 * s - 11318300 * Math.Pow(s, 3) + 485555070 * Math.Pow(s, 5) - 9479884700 * Math.Pow(s, 7) + 101908760525 * Math.Pow(s, 9) - 667039159800 * Math.Pow(s, 11) + 2813536968900 * Math.Pow(s, 13) - 7877903512920 * Math.Pow(s, 15) + 14771069086725 * Math.Pow(s, 17) - 18312670446700 * Math.Pow(s, 19) + 14388526779550 * Math.Pow(s, 21) - 6483367797900 * Math.Pow(s, 23) + 1275062333587 * Math.Pow(s, 25));
                LegArrEx[30, 6] = (1.0 / 4194304) * 154895115375 * Math.Pow(c, 6) * (3059 - 1358196 * Math.Pow(s, 2) + 97111014 * Math.Pow(s, 4) - 2654367716 * Math.Pow(s, 6) + 36687153789 * Math.Pow(s, 8) - 293497230312 * Math.Pow(s, 10) + 1463039223828 * Math.Pow(s, 12) - 4726742107752 * Math.Pow(s, 14) + 10044326978973 * Math.Pow(s, 16) - 13917629539492 * Math.Pow(s, 18) + 12086362494822 * Math.Pow(s, 20) - 5964698374068 * Math.Pow(s, 22) + 1275062333587 * Math.Pow(s, 24));
                LegArrEx[30, 7] = (1.0 / 524288) * 17193357806625.0 * Math.Pow(c, 7) * (-3059 * s + 437437 * Math.Pow(s, 3) - 17934917 * Math.Pow(s, 5) + 330514899 * Math.Pow(s, 7) - 3305148990 * Math.Pow(s, 9) + 19770800322 * Math.Pow(s, 11) - 74520708906 * Math.Pow(s, 13) + 180978864486 * Math.Pow(s, 15) - 282114112287 * Math.Pow(s, 17) + 272215371505 * Math.Pow(s, 19) - 147774058817 * Math.Pow(s, 21) + 34461144151 * Math.Pow(s, 23));
                LegArrEx[30, 8] = (1.0 / 524288) * 7513497361495125.0 * Math.Pow(c, 8) * (-7 + 3003 * Math.Pow(s, 2) - 205205 * Math.Pow(s, 4) + 5294289 * Math.Pow(s, 6) - 68069430 * Math.Pow(s, 8) + 497663166 * Math.Pow(s, 10) - 2216863194 * Math.Pow(s, 12) + 6212089170 * Math.Pow(s, 14) - 10974690867 * Math.Pow(s, 16) + 11835450935 * Math.Pow(s, 18) - 7101270561 * Math.Pow(s, 20) + 1813744429 * Math.Pow(s, 22));
                LegArrEx[30, 9] = (1.0 / 262144) * 7513497361495125.0 * Math.Pow(c, 9) * (3003 * s - 410410 * Math.Pow(s, 3) + 15882867 * Math.Pow(s, 5) - 272277720 * Math.Pow(s, 7) + 2488315830 * Math.Pow(s, 9) - 13301179164 * Math.Pow(s, 11) + 43484624190 * Math.Pow(s, 13) - 87797526936 * Math.Pow(s, 15) + 106519058415 * Math.Pow(s, 17) - 71012705610 * Math.Pow(s, 19) + 19951188719 * Math.Pow(s, 21));
                LegArrEx[30, 10] = (1.0 / 262144) * 157783444591397625.0 * Math.Pow(c, 10) * (143 - 58630 * Math.Pow(s, 2) + 3781635 * Math.Pow(s, 4) - 90759240 * Math.Pow(s, 6) + 1066421070 * Math.Pow(s, 8) - 6967284324 * Math.Pow(s, 10) + 26919053070 * Math.Pow(s, 12) - 62712519240 * Math.Pow(s, 14) + 86229713955 * Math.Pow(s, 16) - 64249590790 * Math.Pow(s, 18) + 19951188719 * Math.Pow(s, 20));
                LegArrEx[30, 11] = (1.0 / 65536) * 32345606141236513125.0 * Math.Pow(c, 11) * (-143 * s + 18447 * Math.Pow(s, 3) - 664092 * Math.Pow(s, 5) + 10404108 * Math.Pow(s, 7) - 84966882 * Math.Pow(s, 9) + 393937362 * Math.Pow(s, 11) - 1070701548 * Math.Pow(s, 13) + 1682531004 * Math.Pow(s, 15) - 1410356871 * Math.Pow(s, 17) + 486614359 * Math.Pow(s, 19));
                LegArrEx[30, 12] = (1.0 / 65536) * 32345606141236513125.0 * Math.Pow(c, 12) * (-143 + 55341 * Math.Pow(s, 2) - 3320460 * Math.Pow(s, 4) + 72828756 * Math.Pow(s, 6) - 764701938 * Math.Pow(s, 8) + 4333310982 * Math.Pow(s, 10) - 13919120124 * Math.Pow(s, 12) + 25237965060 * Math.Pow(s, 14) - 23976066807 * Math.Pow(s, 16) + 9245672821 * Math.Pow(s, 18));
                LegArrEx[30, 13] = ((12517749576658530579375.0 * Math.Pow(c, 13) * (143 * s - 17160 * Math.Pow(s, 3) + 564564 * Math.Pow(s, 5) - 7903896 * Math.Pow(s, 7) + 55985930 * Math.Pow(s, 9) - 215800312 * Math.Pow(s, 11) + 456500660 * Math.Pow(s, 13) - 495629288 * Math.Pow(s, 15) + 215015647 * Math.Pow(s, 17))) / 32768);
                LegArrEx[30, 14] = ((137695245343243836373125.0 * Math.Pow(c, 14) * (13 - 4680 * Math.Pow(s, 2) + 256620 * Math.Pow(s, 4) - 5029752 * Math.Pow(s, 6) + 45806670 * Math.Pow(s, 8) - 215800312 * Math.Pow(s, 10) + 539500780 * Math.Pow(s, 12) - 675858120 * Math.Pow(s, 14) + 332296909 * Math.Pow(s, 16))) / 32768);
                LegArrEx[30, 15] = ((137695245343243836373125.0 * Math.Pow(c, 15) * (-585 * s + 64155 * Math.Pow(s, 3) - 1886157 * Math.Pow(s, 5) + 22903335 * Math.Pow(s, 7) - 134875195 * Math.Pow(s, 9) + 404625585 * Math.Pow(s, 11) - 591375855 * Math.Pow(s, 13) + 332296909 * Math.Pow(s, 15))) / 2048);
                LegArrEx[30, 16] = (2065428680148657545596875.0 * Math.Pow(c, 16) * (-39 + 12831 * Math.Pow(s, 2) - 628719 * Math.Pow(s, 4) + 10688223 * Math.Pow(s, 6) - 80925117 * Math.Pow(s, 8) + 296725429 * Math.Pow(s, 10) - 512525741 * Math.Pow(s, 12) + 332296909 * Math.Pow(s, 14))) / 2048;
                LegArrEx[30, 17] = ((679526035768908332501371875.0 * Math.Pow(c, 17) * (39 * s - 3822 * Math.Pow(s, 3) + 97461 * Math.Pow(s, 5) - 983892 * Math.Pow(s, 7) + 4509505 * Math.Pow(s, 9) - 9346974 * Math.Pow(s, 11) + 7070147 * Math.Pow(s, 13))) / 1024);
                LegArrEx[30, 18] = ((8833838464995808322517834375.0 * Math.Pow(c, 18) * (3 - 882 * Math.Pow(s, 2) + 37485 * Math.Pow(s, 4) - 529788 * Math.Pow(s, 6) + 3121965 * Math.Pow(s, 8) - 7908978 * Math.Pow(s, 10) + 7070147 * Math.Pow(s, 12))) / 1024);
                LegArrEx[30, 19] = (185510607764911974772874521875.0 / 256) * Math.Pow(c, 19) * (-21 * s + 1785 * Math.Pow(s, 3) - 37842 * Math.Pow(s, 5) + 297330 * Math.Pow(s, 7) - 941545 * Math.Pow(s, 9) + 1010021 * Math.Pow(s, 11));
                LegArrEx[30, 20] = 185510607764911974772874521875.0 / 256 * Math.Pow(c, 20) * (-21 + 5355 * Math.Pow(s, 2) - 189210 * Math.Pow(s, 4) + 2081310 * Math.Pow(s, 6) - 8473905 * Math.Pow(s, 8) + 11110231 * Math.Pow(s, 10));
                LegArrEx[30, 21] = (15768401660017517855694334359375.0 / 128) * Math.Pow(c, 21) * (63 * s - 4452 * Math.Pow(s, 3) + 73458 * Math.Pow(s, 5) - 398772 * Math.Pow(s, 7) + 653543 * Math.Pow(s, 9));
                LegArrEx[30, 22] = (141915614940157660701249009234375.0 / 128) * Math.Pow(c, 22) * (7 - 1484 * Math.Pow(s, 2) + 40810 * Math.Pow(s, 4) - 310156 * Math.Pow(s, 6) + 653543 * Math.Pow(s, 8));
                LegArrEx[30, 23] = (7521527591828356017166197489421875.0 / 16) * Math.Pow(c, 23) * (-7 * s + 385 * Math.Pow(s, 3) - 4389 * Math.Pow(s, 5) + 12331 * Math.Pow(s, 7));
                LegArrEx[30, 24] = 52650693142798492120163382425953125.0 / 16 * Math.Pow(c, 24) * (-1 + 165 * Math.Pow(s, 2) - 3135 * Math.Pow(s, 4) + 12331 * Math.Pow(s, 6));
                LegArrEx[30, 25] = (1737472873712350239965391620056453125.0 / 8) * Math.Pow(c, 25) * (5 * s - 190 * Math.Pow(s, 3) + 1121 * Math.Pow(s, 5));
                LegArrEx[30, 26] = (8687364368561751199826958100282265625.0 / 8) * Math.Pow(c, 26) * (1 - 114 * Math.Pow(s, 2) + 1121 * Math.Pow(s, 4));
                LegArrEx[30, 27] = (165059923002673272796712203905363046875.0 / 2) * Math.Pow(c, 27) * (-3 * s + 59 * Math.Pow(s, 3));
                LegArrEx[30, 28] = 495179769008019818390136611716089140625.0 / 2 * Math.Pow(c, 28) * (-1 + 59 * Math.Pow(s, 2));
                LegArrEx[30, 29] = 29215606371473169285018060091249259296875.0 * s * Math.Pow(c, 29);
                LegArrEx[30, 30] = 29215606371473169285018060091249259296875.0 * Math.Pow(c, 30);

                LegArrEx[31, 0] = (-300540195 * s + 49589132175.0 * Math.Pow(s, 3) - 2429867476575.0 * Math.Pow(s, 5) + 55655536011075.0 * Math.Pow(s, 7) - 723521968143975.0 * Math.Pow(s, 9) + 5932880138780595.0 * Math.Pow(s, 11) - 32706903329175075.0 * Math.Pow(s, 13) + 126155198555389575.0 * Math.Pow(s, 15) - 348782019535488825.0 * Math.Pow(s, 17) + 699603700003933725 * Math.Pow(s, 19) - 1019422534291446285.0 * Math.Pow(s, 21) + 1067774591253886425.0 * Math.Pow(s, 23) - 783034700252850045.0 * Math.Pow(s, 25) + 381478443712926945.0 * Math.Pow(s, 27) - 110873045217057585.0 * Math.Pow(s, 29) + 14544636039226909.0 * Math.Pow(s, 31)) / 67108864;
                LegArrEx[31, 1] = (1.0 / 67108864) * 31 * c * (-9694845 + 4798948275 * Math.Pow(s, 2) - 391914109125.0 * Math.Pow(s, 4) + 12567379099275.0 * Math.Pow(s, 6) - 210054764945025.0 * Math.Pow(s, 8) + 2105215533115695.0 * Math.Pow(s, 10) - 13715798170299225.0 * Math.Pow(s, 12) + 61042838010672375.0 * Math.Pow(s, 14) - 191267559100106775.0 * Math.Pow(s, 16) + 428789364518540025.0 * Math.Pow(s, 18) - 690576555487753935.0 * Math.Pow(s, 20) + 792219858027077025.0 * Math.Pow(s, 22) - 631479596978104875.0 * Math.Pow(s, 24) + 332255418717710565.0 * Math.Pow(s, 26) - 103719945525634515.0 * Math.Pow(s, 28) + 14544636039226909.0 * Math.Pow(s, 30));
                LegArrEx[31, 2] = (1.0 / 33554432) * 5115 * c * c * (29084535 * s - 4750474050 * Math.Pow(s, 3) + 228497801805 * Math.Pow(s, 5) - 5092236725940.0 * Math.Pow(s, 7) + 63794410094415.0 * Math.Pow(s, 9) - 498756297101790.0 * Math.Pow(s, 11) + 2589696158028525.0 * Math.Pow(s, 13) - 9273578623035480.0 * Math.Pow(s, 15) + 23388510791920365.0 * Math.Pow(s, 17) - 41853124575015390.0 * Math.Pow(s, 19) + 52814657201805135.0 * Math.Pow(s, 21) - 45925788871134900.0 * Math.Pow(s, 23) + 26177699656546893.0 * Math.Pow(s, 25) - 8800480226417474.0 * Math.Pow(s, 27) + 1322239639929719.0 * Math.Pow(s, 29));
                LegArrEx[31, 3] = (1.0 / 33554432) * 2521695 * Math.Pow(c, 3) * (58995 - 28907550 * Math.Pow(s, 2) + 2317421925 * Math.Pow(s, 4) - 72303564060 * Math.Pow(s, 6) + 1164603835395.0 * Math.Pow(s, 8) - 11128436649330.0 * Math.Pow(s, 10) + 68288133984525.0 * Math.Pow(s, 12) - 282157564595400 * Math.Pow(s, 14) + 806500372135185.0 * Math.Pow(s, 16) - 1613000744270370 * Math.Pow(s, 18) + 2249711564377095 * Math.Pow(s, 20) - 2142582442263900.0 * Math.Pow(s, 22) + 1327469556620025.0 * Math.Pow(s, 24) - 481973562095886.0 * Math.Pow(s, 26) + 77778802348807 * Math.Pow(s, 28));
                LegArrEx[31, 4] = (1.0 / 8388608) * 17651865 * Math.Pow(c, 4) * (-2064825 * s + 331060275 * Math.Pow(s, 3) - 15493620870 * Math.Pow(s, 5) + 332743952970.0 * Math.Pow(s, 7) - 3974441660475.0 * Math.Pow(s, 9) + 29266343136225.0 * Math.Pow(s, 11) - 141078782297700.0 * Math.Pow(s, 13) + 460857355505820.0 * Math.Pow(s, 15) - 1036929049888095.0 * Math.Pow(s, 17) + 1606936831697925.0 * Math.Pow(s, 19) - 1683457633207350.0 * Math.Pow(s, 21) + 1137831048531450.0 * Math.Pow(s, 23) - 447546879089037.0 * Math.Pow(s, 25) + 77778802348807.0 * Math.Pow(s, 27));
                LegArrEx[31, 5] = (1.0 / 8388608) * 476600355 * Math.Pow(c, 5) * (-76475 + 36784475 * Math.Pow(s, 2) - 2869189050 * Math.Pow(s, 4) + 86266950770 * Math.Pow(s, 6) - 1324813886825.0 * Math.Pow(s, 8) + 11923324981425.0 * Math.Pow(s, 10) - 67926821106300.0 * Math.Pow(s, 12) + 256031864169900.0 * Math.Pow(s, 14) - 652881253633245.0 * Math.Pow(s, 16) + 1130807400083725.0 * Math.Pow(s, 18) - 1309355936939050.0 * Math.Pow(s, 20) + 969263485786050.0 * Math.Pow(s, 22) - 414395258415775 * Math.Pow(s, 24) + 77778802348807.0 * Math.Pow(s, 26));
                LegArrEx[31, 6] = (1.0 / 4194304) * 229244770755.0 * Math.Pow(c, 6) * (76475 * s - 11930100 * Math.Pow(s, 3) + 538047510 * Math.Pow(s, 5) - 11017163300 * Math.Pow(s, 7) + 123943087125.0 * Math.Pow(s, 9) - 847320013800.0 * Math.Pow(s, 11) + 3726035445300.0 * Math.Pow(s, 13) - 10858731869160.0 * Math.Pow(s, 15) + 21158558421525 * Math.Pow(s, 17) - 27221537150500.0 * Math.Pow(s, 19) + 22166108822550 * Math.Pow(s, 21) - 10338343245300 * Math.Pow(s, 23) + 2102129793211 * Math.Pow(s, 25));
                LegArrEx[31, 7] = (1.0 / 4194304) * 108891266108625.0 * Math.Pow(c, 7) * (161 - 75348 * Math.Pow(s, 2) + 5663658 * Math.Pow(s, 4) - 162358196 * Math.Pow(s, 6) + 2348395335 * Math.Pow(s, 8) - 19622147688 * Math.Pow(s, 10) + 101975706924.0 * Math.Pow(s, 12) - 342907322184 * Math.Pow(s, 14) + 757253669823 * Math.Pow(s, 16) - 1088861486020 * Math.Pow(s, 18) + 979975337418 * Math.Pow(s, 20) - 500593462404 * Math.Pow(s, 22) + 110638410169 * Math.Pow(s, 24));
                LegArrEx[31, 8] = (1.0 / 524288) * 326673798325875.0 * Math.Pow(c, 8) * (-6279 * s + 943943 * Math.Pow(s, 3) - 40589549 * Math.Pow(s, 5) + 782798445 * Math.Pow(s, 7) - 8175894870 * Math.Pow(s, 9) + 50987853462 * Math.Pow(s, 11) - 200029271274.0 * Math.Pow(s, 13) + 504835779882.0 * Math.Pow(s, 15) - 816646114515 * Math.Pow(s, 17) + 816646114515 * Math.Pow(s, 19) - 458877340537 * Math.Pow(s, 21) + 110638410169 * Math.Pow(s, 23));
                LegArrEx[31, 9] = (1.0 / 524288) * 7513497361495125.0 * Math.Pow(c, 9) * (-273 + 123123 * Math.Pow(s, 2) - 8823815 * Math.Pow(s, 4) + 238243005 * Math.Pow(s, 6) - 3199263210 * Math.Pow(s, 8) + 24385495134 * Math.Pow(s, 10) - 113060022894 * Math.Pow(s, 12) + 329240726010 * Math.Pow(s, 14) - 603607997685 * Math.Pow(s, 16) + 674620703295 * Math.Pow(s, 18) - 418974963099 * Math.Pow(s, 20) + 110638410169 * Math.Pow(s, 22));
                LegArrEx[31, 10] = (1.0 / 262144) * 308053391821300125.0 * Math.Pow(c, 10) * (3003 * s - 430430 * Math.Pow(s, 3) + 17432415 * Math.Pow(s, 5) - 312123240 * Math.Pow(s, 7) + 2973840870 * Math.Pow(s, 9) - 16545369204 * Math.Pow(s, 11) + 56211831270 * Math.Pow(s, 13) - 117777170280 * Math.Pow(s, 15) + 148087471455 * Math.Pow(s, 17) - 102189015390 * Math.Pow(s, 19) + 29683475899 * Math.Pow(s, 21));
                LegArrEx[31, 11] = (1.0 / 262144) * 6469121228247302625.0 * Math.Pow(c, 11) * (143 - 61490 * Math.Pow(s, 2) + 4150575 * Math.Pow(s, 4) - 104041080 * Math.Pow(s, 6) + 1274503230 * Math.Pow(s, 8) - 8666621964 * Math.Pow(s, 10) + 34797800310 * Math.Pow(s, 12) - 84126550200 * Math.Pow(s, 14) + 119880334035 * Math.Pow(s, 16) - 92456728210 * Math.Pow(s, 18) + 29683475899 * Math.Pow(s, 20));
                LegArrEx[31, 12] = (1.0 / 65536) * 1390861064073170064375.0 * Math.Pow(c, 12) * (-143 * s + 19305 * Math.Pow(s, 3) - 725868 * Math.Pow(s, 5) + 11855844 * Math.Pow(s, 7) - 100774674 * Math.Pow(s, 9) + 485550702 * Math.Pow(s, 11) - 1369501980 * Math.Pow(s, 13) + 2230331796 * Math.Pow(s, 15) - 1935140823 * Math.Pow(s, 17) + 690313393 * Math.Pow(s, 19));
                LegArrEx[31, 13] = (1.0 / 65536) * 15299471704804870708125.0 * Math.Pow(c, 13) * (-13 + 5265 * Math.Pow(s, 2) - 329940 * Math.Pow(s, 4) + 7544628 * Math.Pow(s, 6) - 82452006 * Math.Pow(s, 8) + 485550702 * Math.Pow(s, 10) - 1618502340 * Math.Pow(s, 12) + 3041361540 * Math.Pow(s, 14) - 2990672181 * Math.Pow(s, 16) + 1192359497 * Math.Pow(s, 18));
                LegArrEx[31, 14] = ((137695245343243836373125.0 * Math.Pow(c, 14) * (585 * s - 73320 * Math.Pow(s, 3) + 2514876 * Math.Pow(s, 5) - 36645336 * Math.Pow(s, 7) + 269750390 * Math.Pow(s, 9) - 1079001560 * Math.Pow(s, 11) + 2365503420 * Math.Pow(s, 13) - 2658375272 * Math.Pow(s, 15) + 1192359497 * Math.Pow(s, 17))) / 32768);
                LegArrEx[31, 15] = (1.0 / 32768) * 137695245343243836373125.0 * Math.Pow(c, 15) * (585 - 219960 * Math.Pow(s, 2) + 12574380 * Math.Pow(s, 4) - 256517352 * Math.Pow(s, 6) + 2427753510 * Math.Pow(s, 8) - 11869017160 * Math.Pow(s, 10) + 30751544460 * Math.Pow(s, 12) - 39875629080 * Math.Pow(s, 14) + 20270111449 * Math.Pow(s, 16));
                LegArrEx[31, 16] = (6471676531132460309536875.0 * Math.Pow(c, 16) * (-585 * s + 66885 * Math.Pow(s, 3) - 2046681 * Math.Pow(s, 5) + 25827165 * Math.Pow(s, 7) - 157832675 * Math.Pow(s, 9) + 490716135 * Math.Pow(s, 11) - 742365435 * Math.Pow(s, 13) + 431278967 * Math.Pow(s, 15))) / 2048;
                LegArrEx[31, 17] = ((97075147966986904643053125.0 * Math.Pow(c, 17) * (-39 + 13377 * Math.Pow(s, 2) - 682227 * Math.Pow(s, 4) + 12052677 * Math.Pow(s, 6) - 94699605 * Math.Pow(s, 8) + 359858499 * Math.Pow(s, 10) - 643383377 * Math.Pow(s, 12) + 431278967 * Math.Pow(s, 14))) / 2048);
                LegArrEx[31, 18] = ((4756682250382358327509603125.0 * Math.Pow(c, 18) * (273 * s - 27846 * Math.Pow(s, 3) + 737919 * Math.Pow(s, 5) - 7730580 * Math.Pow(s, 7) + 36720255 * Math.Pow(s, 9) - 78781638 * Math.Pow(s, 11) + 61611281 * Math.Pow(s, 13))) / 1024);
                LegArrEx[31, 19] = ((61836869254970658257624840625.0 * Math.Pow(c, 19) * (21 - 6426 * Math.Pow(s, 2) + 283815 * Math.Pow(s, 4) - 4162620 * Math.Pow(s, 6) + 25421715 * Math.Pow(s, 8) - 66661386 * Math.Pow(s, 10) + 61611281 * Math.Pow(s, 12))) / 1024);
                LegArrEx[31, 20] = 3153680332003503571138866871875.0 / 256 * Math.Pow(c, 20) * (-63 * s + 5565 * Math.Pow(s, 3) - 122430 * Math.Pow(s, 5) + 996930 * Math.Pow(s, 7) - 3267715 * Math.Pow(s, 9) + 3624193 * Math.Pow(s, 11));
                LegArrEx[31, 21] = (3153680332003503571138866871875.0 / 256) * Math.Pow(c, 21) * (-63 + 16695 * Math.Pow(s, 2) - 612150 * Math.Pow(s, 4) + 6978510 * Math.Pow(s, 6) - 29409435 * Math.Pow(s, 8) + 39866123 * Math.Pow(s, 10));
                LegArrEx[31, 22] = (835725287980928446351799721046875.0 / 128) * Math.Pow(c, 22) * (63 * s - 4620 * Math.Pow(s, 3) + 79002 * Math.Pow(s, 5) - 443916 * Math.Pow(s, 7) + 752191 * Math.Pow(s, 9));
                LegArrEx[31, 23] = (7521527591828356017166197489421875.0 / 128) * Math.Pow(c, 23) * (7 - 1540 * Math.Pow(s, 2) + 43890 * Math.Pow(s, 4) - 345268 * Math.Pow(s, 6) + 752191 * Math.Pow(s, 8));
                LegArrEx[31, 24] = 82736803510111916188828172383640625.0 / 16 * Math.Pow(c, 24) * (-35 * s + 1995 * Math.Pow(s, 3) - 23541 * Math.Pow(s, 5) + 68381 * Math.Pow(s, 7));
                LegArrEx[31, 25] = (579157624570783413321797206685484375.0 / 16) * Math.Pow(c, 25) * (-5 + 855 * Math.Pow(s, 2) - 16815 * Math.Pow(s, 4) + 68381 * Math.Pow(s, 6));
                LegArrEx[31, 26] = (33011984600534654559342440781072609375.0 / 8) * Math.Pow(c, 26) * (15 * s - 590 * Math.Pow(s, 3) + 3599 * Math.Pow(s, 5));
                LegArrEx[31, 27] = (165059923002673272796712203905363046875.0 / 8) * Math.Pow(c, 27) * (3 - 354 * Math.Pow(s, 2) + 3599 * Math.Pow(s, 4));
                LegArrEx[31, 28] = 9738535457157723095006020030416419765625.0 / 2 * Math.Pow(c, 28) * (-3 * s + 61 * Math.Pow(s, 3));
                LegArrEx[31, 29] = (29215606371473169285018060091249259296875.0 / 2) * Math.Pow(c, 29) * (-1 + 61 * Math.Pow(s, 2));
                LegArrEx[31, 30] = 1782151988659863326386101665566204817109375.0 * s * Math.Pow(c, 30);
                LegArrEx[31, 31] = 1782151988659863326386101665566204817109375.0 * Math.Pow(c, 31);

                LegArrEx[32, 0] = (300540195 - 158685222960 * Math.Pow(s, 2) + 13884957009000 * Math.Pow(s, 4) - 479493848710800 * Math.Pow(s, 6) + 8682263617727700 * Math.Pow(s, 8) - 94926082220489520 * Math.Pow(s, 10) + 680303589246841560 * Math.Pow(s, 12) - 3364138628143722000 * Math.Pow(s, 14) + 11858588664206620050.0 * Math.Pow(s, 16) - 30382789257313693200.0 * Math.Pow(s, 18) + 57087661920320991960.0 * Math.Pow(s, 20) - 78588209916286040880.0 * Math.Pow(s, 22) + 78303470025285004500.0 * Math.Pow(s, 24) - 54932895894661480080.0 * Math.Pow(s, 26) + 25722546490357359720.0 * Math.Pow(s, 28) - 7214139475456546864.0 * Math.Pow(s, 30) + 916312070471295267.0 * Math.Pow(s, 32)) / 2147483648;
                LegArrEx[32, 1] = (1.0 / 67108864) * 33 * c * (-300540195 * s + 52594534125 * Math.Pow(s, 3) - 2724396867675 * Math.Pow(s, 5) + 65774724376725 * Math.Pow(s, 7) - 898921233148575 * Math.Pow(s, 9) + 7730722605077745 * Math.Pow(s, 11) - 44600322721602375 * Math.Pow(s, 13) + 179675585821312425.0 * Math.Pow(s, 15) - 517888453249665225.0 * Math.Pow(s, 17) + 1081205718187897575.0 * Math.Pow(s, 19) - 1637254373255959185.0 * Math.Pow(s, 21) + 1779624318756477375.0 * Math.Pow(s, 23) - 1352514482254922805.0 * Math.Pow(s, 25) + 682037217547354235.0 * Math.Pow(s, 27) - 204947144189106445.0 * Math.Pow(s, 29) + 27767032438524099.0 * Math.Pow(s, 31));
                LegArrEx[32, 2] = (1.0 / 67108864) * 17391 * c * c * (-570285 + 299399625 * Math.Pow(s, 2) - 25848167625 * Math.Pow(s, 4) + 873668065725 * Math.Pow(s, 6) - 15351596012025 * Math.Pow(s, 8) + 161362331415285 * Math.Pow(s, 10) - 1100197714195125 * Math.Pow(s, 12) + 5114105858291625 * Math.Pow(s, 14) - 16706079137085975 * Math.Pow(s, 16) + 38980851319867275.0 * Math.Pow(s, 18) - 65241635366935755 * Math.Pow(s, 20) + 77668613532066375 * Math.Pow(s, 22) - 64161028569967875.0 * Math.Pow(s, 24) + 34943083251951735 * Math.Pow(s, 26) - 11277926340577015 * Math.Pow(s, 28) + 1633354849324947.0 * Math.Pow(s, 30));
                LegArrEx[32, 3] = (1.0 / 33554432) * 608685 * Math.Pow(c, 3) * (8554275 * s - 1477038150 * Math.Pow(s, 3) + 74885834205 * Math.Pow(s, 5) - 1754468115660 * Math.Pow(s, 7) + 23051761630755 * Math.Pow(s, 9) - 188605322433450 * Math.Pow(s, 11) + 1022821171658325 * Math.Pow(s, 13) - 3818532374191080 * Math.Pow(s, 15) + 10023647482251585 * Math.Pow(s, 17) - 18640467247695930 * Math.Pow(s, 19) + 24410135681506575 * Math.Pow(s, 21) - 21998066938274700 * Math.Pow(s, 23) + 12978859493582073 * Math.Pow(s, 25) - 4511170536230806 * Math.Pow(s, 27) + 700009221139263 * Math.Pow(s, 29));
                LegArrEx[32, 4] = (1.0 / 33554432) * 158866785 * Math.Pow(c, 4) * (32775 - 16977450 * Math.Pow(s, 2) + 1434594525 * Math.Pow(s, 4) - 47054700420 * Math.Pow(s, 6) + 794888332095 * Math.Pow(s, 8) - 7948883320950 * Math.Pow(s, 10) + 50945115829725 * Math.Pow(s, 12) - 219455883574200 * Math.Pow(s, 14) + 652881253633245 * Math.Pow(s, 16) - 1356968880100470 * Math.Pow(s, 18) + 1964033905408575 * Math.Pow(s, 20) - 1938526971572100 * Math.Pow(s, 22) + 1243185775247325 * Math.Pow(s, 24) - 466672814092842 * Math.Pow(s, 26) + 77778802348807 * Math.Pow(s, 28));
                LegArrEx[32, 5] = (1.0 / 8388608) * 5878071045 * Math.Pow(c, 5) * (-229425 * s + 38772825 * Math.Pow(s, 3) - 1907622990 * Math.Pow(s, 5) + 42966936870 * Math.Pow(s, 7) - 537086710875 * Math.Pow(s, 9) + 4130685067275 * Math.Pow(s, 11) - 20759340338100 * Math.Pow(s, 13) + 70581757149540 * Math.Pow(s, 15) - 165036755687895 * Math.Pow(s, 17) + 265409987217375 * Math.Pow(s, 19) - 288159414693150 * Math.Pow(s, 21) + 201597693283350 * Math.Pow(s, 23) - 81983061935229 * Math.Pow(s, 25) + 14714908552477 * Math.Pow(s, 27));
                LegArrEx[32, 6] = (1.0 / 8388608) * 335050049565 * Math.Pow(c, 6) * (-4025 + 2040675 * Math.Pow(s, 2) - 167335350 * Math.Pow(s, 4) + 5276641370 * Math.Pow(s, 6) - 84803164875 * Math.Pow(s, 8) + 797149749825 * Math.Pow(s, 10) - 4734586392900 * Math.Pow(s, 12) + 18574146618300 * Math.Pow(s, 14) - 49221488538495 * Math.Pow(s, 16) + 88469995739125 * Math.Pow(s, 18) - 106163994886950 * Math.Pow(s, 20) + 81346437640650 * Math.Pow(s, 22) - 35957483304925 * Math.Pow(s, 24) + 6970219840647 * Math.Pow(s, 26));
                LegArrEx[32, 7] = (1.0 / 4194304) * 13066951933035 * Math.Pow(c, 7) * (52325 * s - 8581300 * Math.Pow(s, 3) + 405895490 * Math.Pow(s, 5) - 8697760500 * Math.Pow(s, 7) + 102198685875 * Math.Pow(s, 9) - 728397906600 * Math.Pow(s, 11) + 3333821187900 * Math.Pow(s, 13) - 10096715597640 * Math.Pow(s, 15) + 20416152862875 * Math.Pow(s, 17) - 27221537150500 * Math.Pow(s, 19) + 22943867026850 * Math.Pow(s, 21) - 11063841016900 * Math.Pow(s, 23) + 2323406613549 * Math.Pow(s, 25));
                LegArrEx[32, 8] = (1.0 / 4194304) * 326673798325875 * Math.Pow(c, 8) * (2093 - 1029756 * Math.Pow(s, 2) + 81179098 * Math.Pow(s, 4) - 2435372940 * Math.Pow(s, 6) + 36791526915 * Math.Pow(s, 8) - 320495078904 * Math.Pow(s, 10) + 1733587017708 * Math.Pow(s, 12) - 6058029358584 * Math.Pow(s, 14) + 13882983946755 * Math.Pow(s, 16) - 20688368234380 * Math.Pow(s, 18) + 19272848302554 * Math.Pow(s, 20) - 10178733735548 * Math.Pow(s, 22) + 2323406613549 * Math.Pow(s, 24));
                LegArrEx[32, 9] = (1.0 / 524288) * 13393625731360875.0 * Math.Pow(c, 9) * (-6279 * s + 989989 * Math.Pow(s, 3) - 44549505 * Math.Pow(s, 5) + 897354315 * Math.Pow(s, 7) - 9771191430 * Math.Pow(s, 9) + 63423915282 * Math.Pow(s, 11) - 258574423842 * Math.Pow(s, 13) + 677218729110 * Math.Pow(s, 15) - 1135337281155 * Math.Pow(s, 17) + 1175173676985 * Math.Pow(s, 19) - 682719945677 * Math.Pow(s, 21) + 170005361967 * Math.Pow(s, 23));
                LegArrEx[32, 10] = (1.0 / 524288) * 6469121228247302625.0 * Math.Pow(c, 10) * (-13 + 6149 * Math.Pow(s, 2) - 461175 * Math.Pow(s, 4) + 13005135 * Math.Pow(s, 6) - 182071890 * Math.Pow(s, 8) + 1444436994 * Math.Pow(s, 10) - 6959560062 * Math.Pow(s, 12) + 21031637550 * Math.Pow(s, 14) - 39960111345 * Math.Pow(s, 16) + 46228364105 * Math.Pow(s, 18) - 29683475899 * Math.Pow(s, 20) + 8095493427 * Math.Pow(s, 22));
                LegArrEx[32, 11] = (1.0 / 262144) * 278172212814634012875.0 * Math.Pow(c, 11) * (143 * s - 21450 * Math.Pow(s, 3) + 907335 * Math.Pow(s, 5) - 16936920 * Math.Pow(s, 7) + 167957790 * Math.Pow(s, 9) - 971101404 * Math.Pow(s, 11) + 3423754950 * Math.Pow(s, 13) - 7434439320 * Math.Pow(s, 15) + 9675704115 * Math.Pow(s, 17) - 6903133930 * Math.Pow(s, 19) + 2070940179 * Math.Pow(s, 21));
                LegArrEx[32, 12] = (1.0 / 262144) * 3059894340960974141625.0 * Math.Pow(c, 12) * (13 - 5850 * Math.Pow(s, 2) + 412425 * Math.Pow(s, 4) - 10778040 * Math.Pow(s, 6) + 137420010 * Math.Pow(s, 8) - 971101404 * Math.Pow(s, 10) + 4046255850 * Math.Pow(s, 12) - 10137871800 * Math.Pow(s, 14) + 14953360905 * Math.Pow(s, 16) - 11923594970 * Math.Pow(s, 18) + 3953613069 * Math.Pow(s, 20));
                LegArrEx[32, 13] = (1.0 / 65536) * 137695245343243836373125.0 * Math.Pow(c, 13) * (-65 * s + 9165 * Math.Pow(s, 3) - 359268 * Math.Pow(s, 5) + 6107556 * Math.Pow(s, 7) - 53950078 * Math.Pow(s, 9) + 269750390 * Math.Pow(s, 11) - 788501140 * Math.Pow(s, 13) + 1329187636 * Math.Pow(s, 15) - 1192359497 * Math.Pow(s, 17) + 439290341 * Math.Pow(s, 19));
                LegArrEx[32, 14] = (1.0 / 65536) * 137695245343243836373125.0 * Math.Pow(c, 14) * (-65 + 27495 * Math.Pow(s, 2) - 1796340 * Math.Pow(s, 4) + 42752892 * Math.Pow(s, 6) - 485550702 * Math.Pow(s, 8) + 2967254290 * Math.Pow(s, 10) - 10250514820 * Math.Pow(s, 12) + 19937814540 * Math.Pow(s, 14) - 20270111449 * Math.Pow(s, 16) + 8346516479 * Math.Pow(s, 18));
                LegArrEx[32, 15] = ((6471676531132460309536875.0 * Math.Pow(c, 15) * (585 * s - 76440 * Math.Pow(s, 3) + 2728908 * Math.Pow(s, 5) - 41323464 * Math.Pow(s, 7) + 315665350 * Math.Pow(s, 9) - 1308576360 * Math.Pow(s, 11) + 2969461740 * Math.Pow(s, 13) - 3450231736 * Math.Pow(s, 15) + 1598269113 * Math.Pow(s, 17))) / 32768);
                LegArrEx[32, 16] = (19415029593397380928610625.0 * Math.Pow(c, 16) * (195 - 76440 * Math.Pow(s, 2) + 4548180 * Math.Pow(s, 4) - 96421416 * Math.Pow(s, 6) + 946996050 * Math.Pow(s, 8) - 4798113320 * Math.Pow(s, 10) + 12867667540 * Math.Pow(s, 12) - 17251158680 * Math.Pow(s, 14) + 9056858307 * Math.Pow(s, 16))) / 32768;
                LegArrEx[32, 17] = ((951336450076471665501920625.0 * Math.Pow(c, 17) * (-195 * s + 23205 * Math.Pow(s, 3) - 737919 * Math.Pow(s, 5) + 9663225 * Math.Pow(s, 7) - 61200425 * Math.Pow(s, 9) + 196954095 * Math.Pow(s, 11) - 308056405 * Math.Pow(s, 13) + 184833843 * Math.Pow(s, 15))) / 2048);
                LegArrEx[32, 18] = ((4756682250382358327509603125.0 * Math.Pow(c, 18) * (-39 + 13923 * Math.Pow(s, 2) - 737919 * Math.Pow(s, 4) + 13528515 * Math.Pow(s, 6) - 110160765 * Math.Pow(s, 8) + 433299009 * Math.Pow(s, 10) - 800946653 * Math.Pow(s, 12) + 554501529 * Math.Pow(s, 14))) / 2048);
                LegArrEx[32, 19] = ((242590794769500274702989759375.0 * Math.Pow(c, 19) * (273 * s - 28938 * Math.Pow(s, 3) + 795795 * Math.Pow(s, 5) - 8640060 * Math.Pow(s, 7) + 42480295 * Math.Pow(s, 9) - 94229018 * Math.Pow(s, 11) + 76108053 * Math.Pow(s, 13))) / 1024);
                LegArrEx[32, 20] = (3153680332003503571138866871875.0 * Math.Pow(c, 20) * (21 - 6678 * Math.Pow(s, 2) + 306075 * Math.Pow(s, 4) - 4652340 * Math.Pow(s, 6) + 29409435 * Math.Pow(s, 8) - 79732246 * Math.Pow(s, 10) + 76108053 * Math.Pow(s, 12))) / 1024;
                LegArrEx[32, 21] = (167145057596185689270359944209375.0 / 256) * Math.Pow(c, 21) * (-63 * s + 5775 * Math.Pow(s, 3) - 131670 * Math.Pow(s, 5) + 1109790 * Math.Pow(s, 7) - 3760955 * Math.Pow(s, 9) + 4308003 * Math.Pow(s, 11));
                LegArrEx[32, 22] = (1504305518365671203433239497884375.0 / 256) * Math.Pow(c, 22) * (-7 + 1925 * Math.Pow(s, 2) - 73150 * Math.Pow(s, 4) + 863170 * Math.Pow(s, 6) - 3760955 * Math.Pow(s, 8) + 5265337 * Math.Pow(s, 10));
                LegArrEx[32, 23] = (82736803510111916188828172383640625.0 / 128) * Math.Pow(c, 23) * (35 * s - 2660 * Math.Pow(s, 3) + 47082 * Math.Pow(s, 5) - 273524 * Math.Pow(s, 7) + 478667 * Math.Pow(s, 9));
                LegArrEx[32, 24] = 579157624570783413321797206685484375.0 / 128 * Math.Pow(c, 24) * (5 - 1140 * Math.Pow(s, 2) + 33630 * Math.Pow(s, 4) - 273524 * Math.Pow(s, 6) + 615429 * Math.Pow(s, 8));
                LegArrEx[32, 25] = (33011984600534654559342440781072609375.0 / 16) * Math.Pow(c, 25) * (-5 * s + 295 * Math.Pow(s, 3) - 3599 * Math.Pow(s, 5) + 10797 * Math.Pow(s, 7));
                LegArrEx[32, 26] = (33011984600534654559342440781072609375.0 / 16) * Math.Pow(c, 26) * (-5 + 885 * Math.Pow(s, 2) - 17995 * Math.Pow(s, 4) + 75579 * Math.Pow(s, 6));
                LegArrEx[32, 27] = (1947707091431544619001204006083283953125.0 / 8) * Math.Pow(c, 27) * (15 * s - 610 * Math.Pow(s, 3) + 3843 * Math.Pow(s, 5));
                LegArrEx[32, 28] = 29215606371473169285018060091249259296875.0 / 8 * Math.Pow(c, 28) * (1 - 122 * Math.Pow(s, 2) + 1281 * Math.Pow(s, 4));
                LegArrEx[32, 29] = (1782151988659863326386101665566204817109375.0 / 2) * Math.Pow(c, 29) * (-s + 21 * Math.Pow(s, 3));
                LegArrEx[32, 30] = (1782151988659863326386101665566204817109375.0 / 2) * Math.Pow(c, 30) * (-1 + 63 * Math.Pow(s, 2));
                LegArrEx[32, 31] = 112275575285571389562324404930670903477890625.0 * s * Math.Pow(c, 31);
                LegArrEx[32, 32] = 112275575285571389562324404930670903477890625.0 * Math.Pow(c, 32);

                LegArrEx[33, 0] = (9917826435.0 * s - 1851327601200 * Math.Pow(s, 3) + 102748681866600.0 * Math.Pow(s, 5) - 2671465728531600.0 * Math.Pow(s, 7) + 39552534258537300 * Math.Pow(s, 9) - 371074685043731760.0 * Math.Pow(s, 11) + 2354897039700605400.0 * Math.Pow(s, 13) - 10540967701516995600.0 * Math.Pow(s, 15) + 34180637914477904850.0 * Math.Pow(s, 17) - 81553802743315702800.0 * Math.Pow(s, 19) + 144078384846524408280.0 * Math.Pow(s, 21) - 187928328060684010800.0 * Math.Pow(s, 23) + 178531911657649810260.0 * Math.Pow(s, 25) - 120038550288334345360.0 * Math.Pow(s, 27) + 54106046065924101480.0 * Math.Pow(s, 29) - 14660993127540724272.0 * Math.Pow(s, 31) + 1804857108504066435.0 * Math.Pow(s, 33)) / 2147483648;
                LegArrEx[33, 1] = (1.0 / 2147483648) * 561 * c * (17678835 - 9900147600 * Math.Pow(s, 2) + 915763653000 * Math.Pow(s, 4) - 33333796969200 * Math.Pow(s, 6) + 634532635163700 * Math.Pow(s, 8) - 7275974216543760 * Math.Pow(s, 10) + 54569806624078200 * Math.Pow(s, 12) - 281844056190294000 * Math.Pow(s, 14) + 1035776906499330450.0 * Math.Pow(s, 16) - 2762071750664881200.0 * Math.Pow(s, 18) + 5393308523666689080.0 * Math.Pow(s, 20) - 7704726462380984400.0 * Math.Pow(s, 22) + 7955967542676016500.0 * Math.Pow(s, 24) - 5777256430989353520 * Math.Pow(s, 26) + 2796925732463099720.0 * Math.Pow(s, 28) - 810144005265173712.0 * Math.Pow(s, 30) + 106168065206121555.0 * Math.Pow(s, 32));
                LegArrEx[33, 2] = (1.0 / 67108864) * 19635 * c * c * (-17678835 * s + 3270584475 * Math.Pow(s, 3) - 178573912335 * Math.Pow(s, 5) + 4532375965455 * Math.Pow(s, 7) - 64964055504855 * Math.Pow(s, 9) + 584676499543695 * Math.Pow(s, 11) - 3523050702378675 * Math.Pow(s, 13) + 14796812949990435 * Math.Pow(s, 15) - 44390438849971305 * Math.Pow(s, 17) + 96309080779762305 * Math.Pow(s, 19) - 151342841225340765 * Math.Pow(s, 21) + 170485018771628925 * Math.Pow(s, 23) - 134114881433681421 * Math.Pow(s, 25) + 69923143311577493 * Math.Pow(s, 27) - 21700285855317153 * Math.Pow(s, 29) + 3033373291603473.0 * Math.Pow(s, 31));
                LegArrEx[33, 3] = (1.0 / 67108864) * 1826055 * Math.Pow(c, 3) * (-190095 + 105502725 * Math.Pow(s, 2) - 9600747975 * Math.Pow(s, 4) + 341146578045 * Math.Pow(s, 6) - 6286844081115 * Math.Pow(s, 8) + 69155284892265 * Math.Pow(s, 10) - 492469453020675 * Math.Pow(s, 12) + 2386582733869425 * Math.Pow(s, 14) - 8114381295156045 * Math.Pow(s, 16) + 19676048761456815 * Math.Pow(s, 18) - 34174189954109205 * Math.Pow(s, 20) + 42162961631693175 * Math.Pow(s, 22) - 36052387482172425 * Math.Pow(s, 24) + 20300267413038627 * Math.Pow(s, 26) - 6766755804346209 * Math.Pow(s, 28) + 1011124430534491 * Math.Pow(s, 30));
                LegArrEx[33, 4] = (1.0 / 33554432) * 202692105.0 * Math.Pow(c, 4) * (950475 * s - 172986450 * Math.Pow(s, 3) + 9220177785 * Math.Pow(s, 5) - 226552939860 * Math.Pow(s, 7) + 3115102923075 * Math.Pow(s, 9) - 26619970433550 * Math.Pow(s, 11) + 150505217451225 * Math.Pow(s, 13) - 584820273524760 * Math.Pow(s, 15) + 1595355304982985 * Math.Pow(s, 17) - 3078755851721550 * Math.Pow(s, 19) + 4178311513050675 * Math.Pow(s, 21) - 3897555403478100 * Math.Pow(s, 23) + 2377508796121641 * Math.Pow(s, 25) - 853464696043666 * Math.Pow(s, 27) + 136638436558715 * Math.Pow(s, 29));
                LegArrEx[33, 5] = (1.0 / 33554432) * 111683349855.0 * Math.Pow(c, 5) * (1725 - 941850 * Math.Pow(s, 2) + 83667675 * Math.Pow(s, 4) - 2878168020 * Math.Pow(s, 6) + 50881898925 * Math.Pow(s, 8) - 531433166550 * Math.Pow(s, 10) + 3550939794675 * Math.Pow(s, 12) - 15920697101400 * Math.Pow(s, 14) + 49221488538495 * Math.Pow(s, 16) - 106163994886950 * Math.Pow(s, 18) + 159245992330425 * Math.Pow(s, 20) - 162692875281300 * Math.Pow(s, 22) + 107872449914775 * Math.Pow(s, 24) - 41821319043882 * Math.Pow(s, 26) + 7191496660985 * Math.Pow(s, 28));
                LegArrEx[33, 6] = (1.0 / 8388608) * 1451883548115.0 * Math.Pow(c, 6) * (-36225 * s + 6435975 * Math.Pow(s, 3) - 332096310 * Math.Pow(s, 5) + 7827984450 * Math.Pow(s, 7) - 102198685875 * Math.Pow(s, 9) + 819447644925 * Math.Pow(s, 11) - 4286341527300 * Math.Pow(s, 13) + 15145073396460 * Math.Pow(s, 15) - 36749075153175 * Math.Pow(s, 17) + 61248458588625 * Math.Pow(s, 19) - 68831601080550 * Math.Pow(s, 21) + 49787284576050 * Math.Pow(s, 23) - 20910659521941 * Math.Pow(s, 25) + 3872344355915 * Math.Pow(s, 27));
                LegArrEx[33, 7] = (1.0 / 8388608) * 65334759665175.0 * Math.Pow(c, 7) * (-805 + 429065 * Math.Pow(s, 2) - 36899590 * Math.Pow(s, 4) + 1217686470 * Math.Pow(s, 6) - 20439737175 * Math.Pow(s, 8) + 200309424315 * Math.Pow(s, 10) - 1238276441220 * Math.Pow(s, 12) + 5048357798820 * Math.Pow(s, 14) - 13882983946755 * Math.Pow(s, 16) + 25860460292975 * Math.Pow(s, 18) - 32121413837590 * Math.Pow(s, 20) + 25446834338870 * Math.Pow(s, 22) - 11617033067745 * Math.Pow(s, 24) + 2323406613549 * Math.Pow(s, 26));
                LegArrEx[33, 8] = (1.0 / 4194304) * 2678725146272175.0 * Math.Pow(c, 8) * (10465 * s - 1799980 * Math.Pow(s, 3) + 89099010 * Math.Pow(s, 5) - 1994120700 * Math.Pow(s, 7) + 24427978575 * Math.Pow(s, 9) - 181211186520 * Math.Pow(s, 11) + 861914746140 * Math.Pow(s, 13) - 2708874916440 * Math.Pow(s, 15) + 5676686405775 * Math.Pow(s, 17) - 7834491179900 * Math.Pow(s, 19) + 6827199456770 * Math.Pow(s, 21) - 3400107239340 * Math.Pow(s, 23) + 736689901857 * Math.Pow(s, 25));
                LegArrEx[33, 9] = (1.0 / 4194304) * 93755380119526125.0 * Math.Pow(c, 9) * (299 - 154284 * Math.Pow(s, 2) + 12728430 * Math.Pow(s, 4) - 398824140 * Math.Pow(s, 6) + 6281480205 * Math.Pow(s, 8) - 56952087192 * Math.Pow(s, 10) + 320139762852 * Math.Pow(s, 12) - 1160946392760 * Math.Pow(s, 14) + 2757247682805 * Math.Pow(s, 16) - 4253009497660 * Math.Pow(s, 18) + 4096319674062 * Math.Pow(s, 20) - 2234356185852 * Math.Pow(s, 22) + 526207072755 * Math.Pow(s, 24));
                LegArrEx[33, 10] = (1.0 / 524288) * 12094444035418870125.0 * Math.Pow(c, 10) * (-299 * s + 49335 * Math.Pow(s, 3) - 2318745 * Math.Pow(s, 5) + 48693645 * Math.Pow(s, 7) - 551861310 * Math.Pow(s, 9) + 3722555382 * Math.Pow(s, 11) - 15749272770 * Math.Pow(s, 13) + 42748026090 * Math.Pow(s, 15) - 74180398215 * Math.Pow(s, 17) + 79386040195 * Math.Pow(s, 19) - 47631624117 * Math.Pow(s, 21) + 12237373785 * Math.Pow(s, 23));
                LegArrEx[33, 11] = (1.0 / 524288) * 278172212814634012875.0 * Math.Pow(c, 11) * (-13 + 6435 * Math.Pow(s, 2) - 504075 * Math.Pow(s, 4) + 14819805 * Math.Pow(s, 6) - 215945730 * Math.Pow(s, 8) + 1780352574 * Math.Pow(s, 10) - 8901762870 * Math.Pow(s, 12) + 27879147450 * Math.Pow(s, 14) - 54828989985 * Math.Pow(s, 16) + 65579772335 * Math.Pow(s, 18) - 43489743759 * Math.Pow(s, 20) + 12237373785 * Math.Pow(s, 22));
                LegArrEx[33, 12] = (1.0 / 262144) * 45898415114414612124375.0 * Math.Pow(c, 12) * (39 * s - 6110 * Math.Pow(s, 3) + 269451 * Math.Pow(s, 5) - 5235048 * Math.Pow(s, 7) + 53950078 * Math.Pow(s, 9) - 323700468 * Math.Pow(s, 11) + 1182751710 * Math.Pow(s, 13) - 2658375272 * Math.Pow(s, 15) + 3577078491 * Math.Pow(s, 17) - 2635742046 * Math.Pow(s, 19) + 815824919 * Math.Pow(s, 21));
                LegArrEx[33, 13] = (1.0 / 262144) * 137695245343243836373125.0 * Math.Pow(c, 13) * (13 - 6110 * Math.Pow(s, 2) + 449085 * Math.Pow(s, 4) - 12215112 * Math.Pow(s, 6) + 161850234 * Math.Pow(s, 8) - 1186901716 * Math.Pow(s, 10) + 5125257410 * Math.Pow(s, 12) - 13291876360 * Math.Pow(s, 14) + 20270111449 * Math.Pow(s, 16) - 16693032958 * Math.Pow(s, 18) + 5710774433 * Math.Pow(s, 20));
                LegArrEx[33, 14] = (1.0 / 65536) * 6471676531132460309536875.0 * Math.Pow(c, 14) * (-65 * s + 9555 * Math.Pow(s, 3) - 389844 * Math.Pow(s, 5) + 6887244 * Math.Pow(s, 7) - 63133070 * Math.Pow(s, 9) + 327144090 * Math.Pow(s, 11) - 989820580 * Math.Pow(s, 13) + 1725115868 * Math.Pow(s, 15) - 1598269113 * Math.Pow(s, 17) + 607529195 * Math.Pow(s, 19));
                LegArrEx[33, 15] = (1.0 / 65536) * 6471676531132460309536875.0 * Math.Pow(c, 15) * (-65 + 28665 * Math.Pow(s, 2) - 1949220 * Math.Pow(s, 4) + 48210708 * Math.Pow(s, 6) - 568197630 * Math.Pow(s, 8) + 3598584990 * Math.Pow(s, 10) - 12867667540 * Math.Pow(s, 12) + 25876738020 * Math.Pow(s, 14) - 27170574921 * Math.Pow(s, 16) + 11543054705 * Math.Pow(s, 18));
                LegArrEx[33, 16] = (951336450076471665501920625.0 * Math.Pow(c, 16) * (195 * s - 26520 * Math.Pow(s, 3) + 983892 * Math.Pow(s, 5) - 15461160 * Math.Pow(s, 7) + 122400850 * Math.Pow(s, 9) - 525210920 * Math.Pow(s, 11) + 1232225620 * Math.Pow(s, 13) - 1478670744 * Math.Pow(s, 15) + 706717635 * Math.Pow(s, 17))) / 32768;
                LegArrEx[33, 17] = ((4756682250382358327509603125.0 * Math.Pow(c, 17) * (39 - 15912 * Math.Pow(s, 2) + 983892 * Math.Pow(s, 4) - 21645624 * Math.Pow(s, 6) + 220321530 * Math.Pow(s, 8) - 1155464024 * Math.Pow(s, 10) + 3203786612 * Math.Pow(s, 12) - 4436012232 * Math.Pow(s, 14) + 2402839959 * Math.Pow(s, 16))) / 32768);
                LegArrEx[33, 18] = ((80863598256500091567663253125.0 * Math.Pow(c, 18) * (-117 * s + 14469 * Math.Pow(s, 3) - 477477 * Math.Pow(s, 5) + 6480045 * Math.Pow(s, 7) - 42480295 * Math.Pow(s, 9) + 141343527 * Math.Pow(s, 11) - 228324159 * Math.Pow(s, 13) + 141343527 * Math.Pow(s, 15))) / 2048);
                LegArrEx[33, 19] = ((3153680332003503571138866871875.0 * Math.Pow(c, 19) * (-3 + 1113 * Math.Pow(s, 2) - 61215 * Math.Pow(s, 4) + 1163085 * Math.Pow(s, 6) - 9803145 * Math.Pow(s, 8) + 39866123 * Math.Pow(s, 10) - 76108053 * Math.Pow(s, 12) + 54362895 * Math.Pow(s, 14))) / 2048);
                LegArrEx[33, 20] = (167145057596185689270359944209375.0 * Math.Pow(c, 20) * (21 * s - 2310 * Math.Pow(s, 3) + 65835 * Math.Pow(s, 5) - 739860 * Math.Pow(s, 7) + 3760955 * Math.Pow(s, 9) - 8616006 * Math.Pow(s, 11) + 7180005 * Math.Pow(s, 13))) / 1024;
                LegArrEx[33, 21] = ((501435172788557067811079832628125.0 * Math.Pow(c, 21) * (7 - 2310 * Math.Pow(s, 2) + 109725 * Math.Pow(s, 4) - 1726340 * Math.Pow(s, 6) + 11282865 * Math.Pow(s, 8) - 31592022 * Math.Pow(s, 10) + 31113355 * Math.Pow(s, 12))) / 1024);
                LegArrEx[33, 22] = (7521527591828356017166197489421875.0 / 256) * Math.Pow(c, 22) * (-77 * s + 7315 * Math.Pow(s, 3) - 172634 * Math.Pow(s, 5) + 1504382 * Math.Pow(s, 7) - 5265337 * Math.Pow(s, 9) + 6222671 * Math.Pow(s, 11));
                LegArrEx[33, 23] = (579157624570783413321797206685484375.0 / 256) * Math.Pow(c, 23) * (-1 + 285 * Math.Pow(s, 2) - 11210 * Math.Pow(s, 4) + 136762 * Math.Pow(s, 6) - 615429 * Math.Pow(s, 8) + 888953 * Math.Pow(s, 10));
                LegArrEx[33, 24] = 11003994866844884853114146927024203125.0 / 128 * Math.Pow(c, 24) * (15 * s - 1180 * Math.Pow(s, 3) + 21594 * Math.Pow(s, 5) - 129564 * Math.Pow(s, 7) + 233935 * Math.Pow(s, 9));
                LegArrEx[33, 25] = (33011984600534654559342440781072609375.0 / 128) * Math.Pow(c, 25) * (5 - 1180 * Math.Pow(s, 2) + 35990 * Math.Pow(s, 4) - 302316 * Math.Pow(s, 6) + 701805 * Math.Pow(s, 8));
                LegArrEx[33, 26] = (1947707091431544619001204006083283953125.0 / 16) * Math.Pow(c, 26) * (-5 * s + 305 * Math.Pow(s, 3) - 3843 * Math.Pow(s, 5) + 11895 * Math.Pow(s, 7));
                LegArrEx[33, 27] = (9738535457157723095006020030416419765625.0 / 16) * Math.Pow(c, 27) * (-1 + 183 * Math.Pow(s, 2) - 3843 * Math.Pow(s, 4) + 16653 * Math.Pow(s, 6));
                LegArrEx[33, 28] = 1782151988659863326386101665566204817109375.0 / 8 * Math.Pow(c, 28) * (s - 42 * Math.Pow(s, 3) + 273 * Math.Pow(s, 5));
                LegArrEx[33, 29] = (1782151988659863326386101665566204817109375.0 / 8) * Math.Pow(c, 29) * (1 - 126 * Math.Pow(s, 2) + 1365 * Math.Pow(s, 4));
                LegArrEx[33, 30] = (37425191761857129854108134976890301159296875.0 / 2) * Math.Pow(c, 30) * (-3 * s + 65 * Math.Pow(s, 3));
                LegArrEx[33, 31] = (112275575285571389562324404930670903477890625.0 / 2) * Math.Pow(c, 31) * (-1 + 65 * Math.Pow(s, 2));
                LegArrEx[33, 32] = 7297912393562140321551086320493608726062890625.0 * s * Math.Pow(c, 32);
                LegArrEx[33, 33] = 7297912393562140321551086320493608726062890625.0 * Math.Pow(c, 33);

                LegArrEx[34, 0] = (-583401555.0 + 347123925225.0 * Math.Pow(s, 2) - 34249560622200.0 * Math.Pow(s, 4) + 1335732864265800.0 * Math.Pow(s, 6) - 27382523717448900.0 * Math.Pow(s, 8) + 340151794623420780.0 * Math.Pow(s, 10) - 2783060137827988200.0 * Math.Pow(s, 12) + 15811451552275493400 * Math.Pow(s, 14) - 64563427171791598050.0 * Math.Pow(s, 16) + 193690281515374794150.0 * Math.Pow(s, 18) - 432235154539573224840.0 * Math.Pow(s, 20) + 720391924232622041400.0 * Math.Pow(s, 22) - 892659558288249051300.0 * Math.Pow(s, 24) + 810260214446256831180.0 * Math.Pow(s, 26) - 523025111970599647640.0 * Math.Pow(s, 28) + 227245393476881226216.0 * Math.Pow(s, 30) - 59560284580634192355.0 * Math.Pow(s, 32) + 7113260368810144185.0 * Math.Pow(s, 34)) / 4294967296;
                LegArrEx[34, 1] = (1.0 / 2147483648) * 595 * c * (583401555 * s - 115124573520 * Math.Pow(s, 3) + 6734787550920.0 * Math.Pow(s, 5) - 184084193058480.0 * Math.Pow(s, 7) + 2858418442213620.0 * Math.Pow(s, 9) - 28064471978097360.0 * Math.Pow(s, 11) + 186017077085594040.0 * Math.Pow(s, 13) - 868079693066105520.0 * Math.Pow(s, 15) + 2929768964098106130.0 * Math.Pow(s, 17) - 7264456378816356720.0 * Math.Pow(s, 19) + 13318170027829987320.0 * Math.Pow(s, 21) - 18003217982284014480.0 * Math.Pow(s, 23) + 17703164349245947572 * Math.Pow(s, 25) - 12306473222837638768.0 * Math.Pow(s, 27) + 5728875465803728392.0 * Math.Pow(s, 29) - 1601621097966633744.0 * Math.Pow(s, 31) + 203236010537432691.0 * Math.Pow(s, 33));
                LegArrEx[34, 2] = (1.0 / 2147483648) * 58905 * c * c * (5892945 - 3488623440 * Math.Pow(s, 2) + 340140785400 * Math.Pow(s, 4) - 13016054054640 * Math.Pow(s, 6) + 259856222019420 * Math.Pow(s, 8) - 3118274664233040.0 * Math.Pow(s, 10) + 24426484869825480.0 * Math.Pow(s, 12) - 131527226222137200.0 * Math.Pow(s, 14) + 503091640299674790 * Math.Pow(s, 16) - 1394188597954654320.0 * Math.Pow(s, 18) + 2825066369539694280.0 * Math.Pow(s, 20) - 4182565793863962960.0 * Math.Pow(s, 22) + 4470496047789380700.0 * Math.Pow(s, 24) - 3356310878955719664.0 * Math.Pow(s, 26) + 1678155439477859832.0 * Math.Pow(s, 28) - 501517717545107536.0 * Math.Pow(s, 30) + 67745336845810897.0 * Math.Pow(s, 32));
                LegArrEx[34, 3] = (1.0 / 67108864) * 2179485 * Math.Pow(c, 3) * (-5892945 * s + 1149124275 * Math.Pow(s, 3) - 65959733385 * Math.Pow(s, 5) + 1755785283915 * Math.Pow(s, 7) - 26336779258725 * Math.Pow(s, 9) + 247565725032015.0 * Math.Pow(s, 11) - 1555220580329325.0 * Math.Pow(s, 13) + 6798535679725335.0 * Math.Pow(s, 15) - 21195434766202515 * Math.Pow(s, 17) + 47720715701684025.0 * Math.Pow(s, 19) - 77716594142742555 * Math.Pow(s, 21) + 90618163130865825 * Math.Pow(s, 23) - 73702772679770871.0 * Math.Pow(s, 25) + 39686108366030469.0 * Math.Pow(s, 27) - 12707374599960495 * Math.Pow(s, 29) + 1830955049886781 * Math.Pow(s, 31));
                LegArrEx[34, 4] = (1.0 / 67108864) * 1283716665.0 * Math.Pow(c, 4) * (-10005 + 5852925 * Math.Pow(s, 2) - 559929825 * Math.Pow(s, 4) + 20866718145 * Math.Pow(s, 6) - 402429564225 * Math.Pow(s, 8) + 4623468548985 * Math.Pow(s, 10) - 34325751348525 * Math.Pow(s, 12) + 173137580977725 * Math.Pow(s, 14) - 611752786121295 * Math.Pow(s, 16) + 1539377925860775 * Math.Pow(s, 18) - 2770880266549395.0 * Math.Pow(s, 20) + 3538570037368275 * Math.Pow(s, 22) - 3128301047528475 * Math.Pow(s, 24) + 1819227378408867.0 * Math.Pow(s, 26) - 625660209505695.0 * Math.Pow(s, 28) + 96366055257199 * Math.Pow(s, 30));
                LegArrEx[34, 5] = (1.0 / 33554432) * 50064949935.0 * Math.Pow(c, 5) * (150075 * s - 28714350 * Math.Pow(s, 3) + 1605132165 * Math.Pow(s, 5) - 41274827100 * Math.Pow(s, 7) + 592752378075 * Math.Pow(s, 9) - 5280884822850 * Math.Pow(s, 11) + 31075976072925 * Math.Pow(s, 13) - 125487750999240 * Math.Pow(s, 15) + 355241059814025 * Math.Pow(s, 17) - 710482119628050 * Math.Pow(s, 19) + 998058215667975 * Math.Pow(s, 21) - 962554168470300 * Math.Pow(s, 23) + 606409126136289 * Math.Pow(s, 25) - 224595972643070.0 * Math.Pow(s, 27) + 37063867406615.0 * Math.Pow(s, 29));
                LegArrEx[34, 6] = (1.0 / 33554432) * 7259417740575.0 * Math.Pow(c, 6) * (1035 - 594090 * Math.Pow(s, 2) + 55349385 * Math.Pow(s, 4) - 1992577860 * Math.Pow(s, 6) + 36791526915 * Math.Pow(s, 8) - 400618848630 * Math.Pow(s, 10) + 2786121992745 * Math.Pow(s, 12) - 12981491482680 * Math.Pow(s, 14) + 41648951840265 * Math.Pow(s, 16) - 93097657054710 * Math.Pow(s, 18) + 144546362269155 * Math.Pow(s, 20) - 152681006033220 * Math.Pow(s, 22) + 104553297609705 * Math.Pow(s, 24) - 41821319043882 * Math.Pow(s, 26) + 7412773481323 * Math.Pow(s, 28));
                LegArrEx[34, 7] = (1.0 / 8388608) * 297636127363575.0 * Math.Pow(c, 7) * (-7245 * s + 1349985 * Math.Pow(s, 3) - 72899190 * Math.Pow(s, 5) + 1794708630 * Math.Pow(s, 7) - 24427978575 * Math.Pow(s, 9) + 203862584835 * Math.Pow(s, 11) - 1108176102180 * Math.Pow(s, 13) + 4063312374660 * Math.Pow(s, 15) - 10218035530395 * Math.Pow(s, 17) + 17627605154775 * Math.Pow(s, 19) - 20481598370310 * Math.Pow(s, 21) + 15300482577030 * Math.Pow(s, 23) - 6630209116713 * Math.Pow(s, 25) + 1265595472421 * Math.Pow(s, 27));
                LegArrEx[34, 8] = (1.0 / 8388608) * 18751076023905225.0 * Math.Pow(c, 8) * (-115 + 64285 * Math.Pow(s, 2) - 5785650 * Math.Pow(s, 4) + 199412070 * Math.Pow(s, 6) - 3489711225 * Math.Pow(s, 8) + 35595054495 * Math.Pow(s, 10) - 228671259180 * Math.Pow(s, 12) + 967455327300 * Math.Pow(s, 14) - 2757247682805 * Math.Pow(s, 16) + 5316261872075 * Math.Pow(s, 18) - 6827199456770 * Math.Pow(s, 20) + 5585890464630 * Math.Pow(s, 22) - 2631035363775 * Math.Pow(s, 24) + 542398059609 * Math.Pow(s, 26));
                LegArrEx[34, 9] = (1.0 / 4194304) * 806296269027924675.0 * Math.Pow(c, 9) * (1495 * s - 269100 * Math.Pow(s, 3) + 13912470 * Math.Pow(s, 5) - 324624300 * Math.Pow(s, 7) + 4138959825 * Math.Pow(s, 9) - 31907617560 * Math.Pow(s, 11) + 157492727700 * Math.Pow(s, 13) - 512976313080 * Math.Pow(s, 15) + 1112705973225 * Math.Pow(s, 17) - 1587720803900 * Math.Pow(s, 19) + 1428948723510 * Math.Pow(s, 21) - 734242427100 * Math.Pow(s, 23) + 163980808719 * Math.Pow(s, 25));
                LegArrEx[34, 10] = (1.0 / 4194304) * 4031481345139623375.0 * Math.Pow(c, 10) * (299 - 161460 * Math.Pow(s, 2) + 13912470 * Math.Pow(s, 4) - 454474020 * Math.Pow(s, 6) + 7450127685 * Math.Pow(s, 8) - 70196758632 * Math.Pow(s, 10) + 409481092020 * Math.Pow(s, 12) - 1538928939240 * Math.Pow(s, 14) + 3783200308965 * Math.Pow(s, 16) - 6033339054820 * Math.Pow(s, 18) + 6001584638742 * Math.Pow(s, 20) - 3377515164660 * Math.Pow(s, 22) + 819904043595 * Math.Pow(s, 24));
                LegArrEx[34, 11] = (1.0 / 524288) * 181416660531283051875.0 * Math.Pow(c, 11) * (-897 * s + 154583 * Math.Pow(s, 3) - 7574567 * Math.Pow(s, 5) + 165558393 * Math.Pow(s, 7) - 1949909962 * Math.Pow(s, 9) + 13649369734 * Math.Pow(s, 11) - 59847236526 * Math.Pow(s, 13) + 168142235954 * Math.Pow(s, 15) - 301666952741 * Math.Pow(s, 17) + 333421368819 * Math.Pow(s, 19) - 206403704507 * Math.Pow(s, 21) + 54660269573 * Math.Pow(s, 23));
                LegArrEx[34, 12] = (1.0 / 524288) * 4172583192219510193125.0 * Math.Pow(c, 12) * (-39 + 20163 * Math.Pow(s, 2) - 1646645 * Math.Pow(s, 4) + 50387337 * Math.Pow(s, 6) - 763008246 * Math.Pow(s, 8) + 6527959438 * Math.Pow(s, 10) - 33826698906 * Math.Pow(s, 12) + 109657979970 * Math.Pow(s, 14) - 222971225939 * Math.Pow(s, 16) + 275435043807 * Math.Pow(s, 18) - 188455556289 * Math.Pow(s, 20) + 54660269573 * Math.Pow(s, 22));
                LegArrEx[34, 13] = (1.0 / 262144) * 2157225510377486769845625.0 * Math.Pow(c, 13) * (39 * s - 6370 * Math.Pow(s, 3) + 292383 * Math.Pow(s, 5) - 5903352 * Math.Pow(s, 7) + 63133070 * Math.Pow(s, 9) - 392572908 * Math.Pow(s, 11) + 1484730870 * Math.Pow(s, 13) - 3450231736 * Math.Pow(s, 15) + 4794807339 * Math.Pow(s, 17) - 3645175170 * Math.Pow(s, 19) + 1162984459 * Math.Pow(s, 21));
                LegArrEx[34, 14] = (1.0 / 262144) * 6471676531132460309536875.0 * Math.Pow(c, 14) * (13 - 6370 * Math.Pow(s, 2) + 487305 * Math.Pow(s, 4) - 13774488 * Math.Pow(s, 6) + 189399210 * Math.Pow(s, 8) - 1439433996 * Math.Pow(s, 10) + 6433833770 * Math.Pow(s, 12) - 17251158680 * Math.Pow(s, 14) + 27170574921 * Math.Pow(s, 16) - 23086109410 * Math.Pow(s, 18) + 8140891213 * Math.Pow(s, 20));
                LegArrEx[34, 15] = (1.0 / 65536) * 317112150025490555167306875.0 * Math.Pow(c, 15) * (-65 * s + 9945 * Math.Pow(s, 3) - 421668 * Math.Pow(s, 5) + 7730580 * Math.Pow(s, 7) - 73440510 * Math.Pow(s, 9) + 393908190 * Math.Pow(s, 11) - 1232225620 * Math.Pow(s, 13) + 2218006116 * Math.Pow(s, 15) - 2120152905 * Math.Pow(s, 17) + 830703185 * Math.Pow(s, 19));
                LegArrEx[34, 16] = (1.0 / 65536) * 1585560750127452775836534375.0 * Math.Pow(c, 16) * (-13 + 5967 * Math.Pow(s, 2) - 421668 * Math.Pow(s, 4) + 10822812 * Math.Pow(s, 6) - 132192918 * Math.Pow(s, 8) + 866598018 * Math.Pow(s, 10) - 3203786612 * Math.Pow(s, 12) + 6654018348 * Math.Pow(s, 14) - 7208519877 * Math.Pow(s, 16) + 3156672103 * Math.Pow(s, 18));
                LegArrEx[34, 17] = (1.0 / 32768) * 4756682250382358327509603125.0 * Math.Pow(c, 17) * (1989 * s - 281112 * Math.Pow(s, 3) + 10822812 * Math.Pow(s, 5) - 176257224 * Math.Pow(s, 7) + 1444330030 * Math.Pow(s, 9) - 6407573224 * Math.Pow(s, 11) + 15526042812 * Math.Pow(s, 13) - 19222719672 * Math.Pow(s, 15) + 9470016309 * Math.Pow(s, 17));
                LegArrEx[34, 18] = ((1051226777334501190379622290625.0 * Math.Pow(c, 18) * (9 - 3816 * Math.Pow(s, 2) + 244860 * Math.Pow(s, 4) - 5582808 * Math.Pow(s, 6) + 58818870 * Math.Pow(s, 8) - 318928984 * Math.Pow(s, 10) + 913296636 * Math.Pow(s, 12) - 1304709480 * Math.Pow(s, 14) + 728462793 * Math.Pow(s, 16))) / 32768);
                LegArrEx[34, 19] = ((55715019198728563090119981403125.0 * Math.Pow(c, 19) * (-9 * s + 1155 * Math.Pow(s, 3) - 39501 * Math.Pow(s, 5) + 554895 * Math.Pow(s, 7) - 3760955 * Math.Pow(s, 9) + 12924009 * Math.Pow(s, 11) - 21540015 * Math.Pow(s, 13) + 13744581 * Math.Pow(s, 15))) / 2048);
                LegArrEx[34, 20] = (501435172788557067811079832628125.0 * Math.Pow(c, 20) * (-1 + 385 * Math.Pow(s, 2) - 21945 * Math.Pow(s, 4) + 431585 * Math.Pow(s, 6) - 3760955 * Math.Pow(s, 8) + 15796011 * Math.Pow(s, 10) - 31113355 * Math.Pow(s, 12) + 22907635 * Math.Pow(s, 14))) / 2048;
                LegArrEx[34, 21] = ((2507175863942785339055399163140625.0 * Math.Pow(c, 21) * (77 * s - 8778 * Math.Pow(s, 3) + 258951 * Math.Pow(s, 5) - 3008764 * Math.Pow(s, 7) + 15796011 * Math.Pow(s, 9) - 37336026 * Math.Pow(s, 11) + 32070689 * Math.Pow(s, 13))) / 1024);
                LegArrEx[34, 22] = ((17550231047599497373387794141984375.0 * Math.Pow(c, 22) * (11 - 3762 * Math.Pow(s, 2) + 184965 * Math.Pow(s, 4) - 3008764 * Math.Pow(s, 6) + 20309157 * Math.Pow(s, 8) - 58670898 * Math.Pow(s, 10) + 59559851 * Math.Pow(s, 12))) / 1024);
                LegArrEx[34, 23] = (1000363169713171350283104266093109375.0 / 256) * Math.Pow(c, 23) * (-33 * s + 3245 * Math.Pow(s, 3) - 79178 * Math.Pow(s, 5) + 712602 * Math.Pow(s, 7) - 2573285 * Math.Pow(s, 9) + 3134729 * Math.Pow(s, 11));
                LegArrEx[34, 24] = 11003994866844884853114146927024203125.0 / 256 * Math.Pow(c, 24) * (-3 + 885 * Math.Pow(s, 2) - 35990 * Math.Pow(s, 4) + 453474 * Math.Pow(s, 6) - 2105415 * Math.Pow(s, 8) + 3134729 * Math.Pow(s, 10));
                LegArrEx[34, 25] = (649235697143848206333734668694427984375.0 / 128) * Math.Pow(c, 25) * (15 * s - 1220 * Math.Pow(s, 3) + 23058 * Math.Pow(s, 5) - 142740 * Math.Pow(s, 7) + 265655 * Math.Pow(s, 9));
                LegArrEx[34, 26] = (9738535457157723095006020030416419765625.0 / 128) * Math.Pow(c, 26) * (1 - 244 * Math.Pow(s, 2) + 7686 * Math.Pow(s, 4) - 66612 * Math.Pow(s, 6) + 159393 * Math.Pow(s, 8));
                LegArrEx[34, 27] = (594050662886621108795367221855401605703125.0 / 16) * Math.Pow(c, 27) * (-s + 63 * Math.Pow(s, 3) - 819 * Math.Pow(s, 5) + 2613 * Math.Pow(s, 7));
                LegArrEx[34, 28] = 594050662886621108795367221855401605703125.0 / 16 * Math.Pow(c, 28) * (-1 + 189 * Math.Pow(s, 2) - 4095 * Math.Pow(s, 4) + 18291 * Math.Pow(s, 6));
                LegArrEx[34, 29] = (37425191761857129854108134976890301159296875.0 / 8) * Math.Pow(c, 29) * (3 * s - 130 * Math.Pow(s, 3) + 871 * Math.Pow(s, 5));
                LegArrEx[34, 30] = (37425191761857129854108134976890301159296875.0 / 8) * Math.Pow(c, 30) * (3 - 390 * Math.Pow(s, 2) + 4355 * Math.Pow(s, 4));
                LegArrEx[34, 31] = (2432637464520713440517028773497869575354296875.0 / 2) * Math.Pow(c, 31) * (-3 * s + 67 * Math.Pow(s, 3));
                LegArrEx[34, 32] = 7297912393562140321551086320493608726062890625.0 / 2 * Math.Pow(c, 32) * (-1 + 67 * Math.Pow(s, 2));
                LegArrEx[34, 33] = 488960130368663401543922783473071784646213671875.0 * s * Math.Pow(c, 33);
                LegArrEx[34, 34] = 488960130368663401543922783473071784646213671875.0 * Math.Pow(c, 34);

                LegArrEx[35, 0] = (-20419054425 * s + 4281195077775.0 * Math.Pow(s, 3) - 267146572853160.0 * Math.Pow(s, 5) + 7823578204985400.0 * Math.Pow(s, 7) - 130827613316700300.0 * Math.Pow(s, 9) + 1391530068913994100.0 * Math.Pow(s, 11) - 10061832805993495800.0 * Math.Pow(s, 13) + 51650741737433278440.0 * Math.Pow(s, 15) - 193690281515374794150.0 * Math.Pow(s, 17) + 540293943174466531050.0 * Math.Pow(s, 19) - 1132044452365548922200.0 * Math.Pow(s, 21) + 1785319116576498102600.0 * Math.Pow(s, 23) - 2106676557560267761068.0 * Math.Pow(s, 25) + 1830587891897098766740.0 * Math.Pow(s, 27) - 1136226967384406131080.0 * Math.Pow(s, 29) + 476482276645073538840.0 * Math.Pow(s, 31) - 120925426269772451145.0 * Math.Pow(s, 33) + 14023284727082855679.0 * Math.Pow(s, 35)) / 4294967296;
                LegArrEx[35, 1] = (1.0 / 4294967296) * 315 * c * (-64822395 + 40773286455 * Math.Pow(s, 2) - 4240421791320 * Math.Pow(s, 4) + 173857293444120.0 * Math.Pow(s, 6) - 3737931809048580.0 * Math.Pow(s, 8) + 48593113517631540 * Math.Pow(s, 10) - 415250242787033160.0 * Math.Pow(s, 12) + 2459559130353965640 * Math.Pow(s, 14) - 10453126304004353970.0 * Math.Pow(s, 16) + 32589158477190044730.0 * Math.Pow(s, 18) - 75469630157703261480.0 * Math.Pow(s, 20) + 130356633908760178920.0 * Math.Pow(s, 22) - 167196552187322838180.0 * Math.Pow(s, 24) + 156907533591179894292.0 * Math.Pow(s, 26) - 104605022394119929528.0 * Math.Pow(s, 28) + 46891906590467554616.0 * Math.Pow(s, 30) - 12668377990166637739.0 * Math.Pow(s, 32) + 1558142747453650631.0 * Math.Pow(s, 34));
                LegArrEx[35, 2] = (1.0 / 2147483648) * 198135.0 * c * c * (64822395 * s - 13483058160 * Math.Pow(s, 3) + 829208076840 * Math.Pow(s, 5) - 23770631536080 * Math.Pow(s, 7) + 386272762461300 * Math.Pow(s, 9) - 3961051600512240.0 * Math.Pow(s, 11) + 27371882213796120.0 * Math.Pow(s, 13) - 132949142181295440 * Math.Pow(s, 15) + 466299564856455330 * Math.Pow(s, 17) - 1199835137642341200 * Math.Pow(s, 19) + 2279686761520448280.0 * Math.Pow(s, 21) - 3189759342206477040.0 * Math.Pow(s, 23) + 3242921997909918324.0 * Math.Pow(s, 25) - 2328251690807120848.0 * Math.Pow(s, 27) + 1118248964796523560.0 * Math.Pow(s, 29) - 322248088780073456.0 * Math.Pow(s, 31) + 42111966147395963.0 * Math.Pow(s, 33));
                LegArrEx[35, 3] = (1.0 / 2147483648) * 41410215.0 * Math.Pow(c, 3) * (310155 - 193536720 * Math.Pow(s, 2) + 19837513800 * Math.Pow(s, 4) - 796145553840 * Math.Pow(s, 6) + 16633755321300 * Math.Pow(s, 8) - 208476400026960 * Math.Pow(s, 10) + 1702557266886840 * Math.Pow(s, 12) - 9541804462772400 * Math.Pow(s, 14) + 37928672739520290 * Math.Pow(s, 16) - 109075921603849200 * Math.Pow(s, 18) + 229059435368083320 * Math.Pow(s, 20) - 351026147706932880 * Math.Pow(s, 22) + 387909329893530900 * Math.Pow(s, 24) - 300778926563599344 * Math.Pow(s, 26) + 155163731957412360 * Math.Pow(s, 28) - 47797563407570704 * Math.Pow(s, 30) + 6649257812746731.0 * Math.Pow(s, 32));
                LegArrEx[35, 4] = (1.0 / 67108864) * 1614998385.0 * Math.Pow(c, 4) * (-310155 * s + 63581775 * Math.Pow(s, 3) - 3827622855 * Math.Pow(s, 5) + 106626636675 * Math.Pow(s, 7) - 1670483974575 * Math.Pow(s, 9) + 16370742950835 * Math.Pow(s, 11) - 107039473140075 * Math.Pow(s, 13) + 486265035122055 * Math.Pow(s, 15) - 1573210407747825 * Math.Pow(s, 17) + 3670824284744925 * Math.Pow(s, 19) - 6187960937141445 * Math.Pow(s, 21) + 7459794805644825 * Math.Pow(s, 23) - 6266227636741653 * Math.Pow(s, 25) + 3481237575967585 * Math.Pow(s, 27) - 1148979889605065 * Math.Pow(s, 29) + 170493790070429 * Math.Pow(s, 31));
                LegArrEx[35, 5] = (1.0 / 67108864) * 50064949935.0 * Math.Pow(c, 5) * (-10005 + 6153075 * Math.Pow(s, 2) - 617358525 * Math.Pow(s, 4) + 24076982475 * Math.Pow(s, 6) - 484979218425 * Math.Pow(s, 8) + 5808973305135 * Math.Pow(s, 10) - 44887520994225 * Math.Pow(s, 12) + 235289533123575 * Math.Pow(s, 14) - 862728288119775 * Math.Pow(s, 16) + 2249860045488825 * Math.Pow(s, 18) - 4191844505805495 * Math.Pow(s, 20) + 5534686468704225 * Math.Pow(s, 22) - 5053409384469075 * Math.Pow(s, 24) + 3032045630681445 * Math.Pow(s, 26) - 1074852154791835 * Math.Pow(s, 28) + 170493790070429 * Math.Pow(s, 30));
                LegArrEx[35, 6] = (1.0 / 33554432) * 10263314736675.0 * Math.Pow(c, 6) * (30015 * s - 6023010 * Math.Pow(s, 3) + 352346085 * Math.Pow(s, 5) - 9463009140 * Math.Pow(s, 7) + 141682275735 * Math.Pow(s, 9) - 1313781102270 * Math.Pow(s, 11) + 8034276740805 * Math.Pow(s, 13) - 33667445390040 * Math.Pow(s, 15) + 98774343460485 * Math.Pow(s, 17) - 204480219795390 * Math.Pow(s, 19) + 296983176369495 * Math.Pow(s, 21) - 295809329822580 * Math.Pow(s, 23) + 192276064384677 * Math.Pow(s, 25) - 73404537400418 * Math.Pow(s, 27) + 12475155371007 * Math.Pow(s, 29));
                LegArrEx[35, 7] = (1.0 / 33554432) * 892908382090725.0 * Math.Pow(c, 7) * (345 - 207690 * Math.Pow(s, 2) + 20249775 * Math.Pow(s, 4) - 761391540 * Math.Pow(s, 6) + 14656787145 * Math.Pow(s, 8) - 166110254310 * Math.Pow(s, 10) + 1200524110695 * Math.Pow(s, 12) - 5804731963800 * Math.Pow(s, 14) + 19300733779635 * Math.Pow(s, 16) - 44656599725430 * Math.Pow(s, 18) + 71685594296085 * Math.Pow(s, 20) - 78202466504820 * Math.Pow(s, 22) + 55251742639275 * Math.Pow(s, 24) - 22780718503578 * Math.Pow(s, 26) + 4158385123669 * Math.Pow(s, 28));
                LegArrEx[35, 8] = (1.0 / 8388608) * 268765423009308225.0 * Math.Pow(c, 8) * (-345 * s + 67275 * Math.Pow(s, 3) - 3794310 * Math.Pow(s, 5) + 97387290 * Math.Pow(s, 7) - 1379653275 * Math.Pow(s, 9) + 11965356585 * Math.Pow(s, 11) - 67496883300 * Math.Pow(s, 13) + 256488156540 * Math.Pow(s, 15) - 667623583935 * Math.Pow(s, 17) + 1190790602925 * Math.Pow(s, 19) - 1428948723510 * Math.Pow(s, 21) + 1101363640650 * Math.Pow(s, 23) - 491942426157 * Math.Pow(s, 25) + 96706630783 * Math.Pow(s, 27));
                LegArrEx[35, 9] = (1.0 / 8388608) * 806296269027924675.0 * Math.Pow(c, 9) * (-115 + 67275 * Math.Pow(s, 2) - 6323850 * Math.Pow(s, 4) + 227237010 * Math.Pow(s, 6) - 4138959825 * Math.Pow(s, 8) + 43872974145 * Math.Pow(s, 10) - 292486494300 * Math.Pow(s, 12) + 1282440782700 * Math.Pow(s, 14) - 3783200308965 * Math.Pow(s, 16) + 7541673818525 * Math.Pow(s, 18) - 10002641064570 * Math.Pow(s, 20) + 8443787911650 * Math.Pow(s, 22) - 4099520217975 * Math.Pow(s, 24) + 870359677047 * Math.Pow(s, 26));
                LegArrEx[35, 10] = (1.0 / 4194304) * 7256666421251322075.0 * Math.Pow(c, 10) * (7475 * s - 1405300 * Math.Pow(s, 3) + 75745670 * Math.Pow(s, 5) - 1839537700 * Math.Pow(s, 7) + 24373874525 * Math.Pow(s, 9) - 194990996200 * Math.Pow(s, 11) + 997453942100 * Math.Pow(s, 13) - 3362844719080 * Math.Pow(s, 15) + 7541673818525 * Math.Pow(s, 17) - 11114045627300 * Math.Pow(s, 19) + 10320185225350 * Math.Pow(s, 21) - 5466026957300 * Math.Pow(s, 23) + 1257186200179 * Math.Pow(s, 25));
                LegArrEx[35, 11] = (1.0 / 4194304) * 4172583192219510193125.0 * Math.Pow(c, 11) * (13 - 7332 * Math.Pow(s, 2) + 658658 * Math.Pow(s, 4) - 22394372 * Math.Pow(s, 6) + 381504123 * Math.Pow(s, 8) - 3730262536 * Math.Pow(s, 10) + 22551132604 * Math.Pow(s, 12) - 87726383976 * Math.Pow(s, 14) + 222971225939 * Math.Pow(s, 16) - 367246725076 * Math.Pow(s, 18) + 376911112578 * Math.Pow(s, 20) - 218641078292 * Math.Pow(s, 22) + 54660269573 * Math.Pow(s, 24));
                LegArrEx[35, 12] = (1.0 / 524288) * 196111410034316979076875.0 * Math.Pow(c, 12) * (-39 * s + 7007 * Math.Pow(s, 3) - 357357 * Math.Pow(s, 5) + 8117109 * Math.Pow(s, 7) - 99209110 * Math.Pow(s, 9) + 719716998 * Math.Pow(s, 11) - 3266407914 * Math.Pow(s, 13) + 9488137274 * Math.Pow(s, 15) - 17580960243 * Math.Pow(s, 17) + 20048463435 * Math.Pow(s, 19) - 12792829049 * Math.Pow(s, 21) + 3488953377 * Math.Pow(s, 23));
                LegArrEx[35, 13] = (1.0 / 524288) * 588334230102950937230625.0 * Math.Pow(c, 13) * (-13 + 7007 * Math.Pow(s, 2) - 595595 * Math.Pow(s, 4) + 18939921 * Math.Pow(s, 6) - 297627330 * Math.Pow(s, 8) + 2638962326 * Math.Pow(s, 10) - 14154434294 * Math.Pow(s, 12) + 47440686370 * Math.Pow(s, 14) - 99625441377 * Math.Pow(s, 16) + 126973601755 * Math.Pow(s, 18) - 89549803343 * Math.Pow(s, 20) + 26748642557 * Math.Pow(s, 22));
                LegArrEx[35, 14] = (1.0 / 262144) * 45301735717927222166758125.0 * Math.Pow(c, 14) * (91 * s - 15470 * Math.Pow(s, 3) + 737919 * Math.Pow(s, 5) - 15461160 * Math.Pow(s, 7) + 171361190 * Math.Pow(s, 9) - 1102942932 * Math.Pow(s, 11) + 4312789670 * Math.Pow(s, 13) - 10350695208 * Math.Pow(s, 15) + 14841070335 * Math.Pow(s, 17) - 11629844590 * Math.Pow(s, 19) + 3821234651 * Math.Pow(s, 21));
                LegArrEx[35, 15] = (1.0 / 262144) * 317112150025490555167306875.0 * Math.Pow(c, 15) * (13 - 6630 * Math.Pow(s, 2) + 527085 * Math.Pow(s, 4) - 15461160 * Math.Pow(s, 6) + 220321530 * Math.Pow(s, 8) - 1733196036 * Math.Pow(s, 10) + 8009466530 * Math.Pow(s, 12) - 22180061160 * Math.Pow(s, 14) + 36042599385 * Math.Pow(s, 16) - 31566721030 * Math.Pow(s, 18) + 11463703953 * Math.Pow(s, 20));
                LegArrEx[35, 16] = (1.0 / 65536) * 4756682250382358327509603125.0 * Math.Pow(c, 16) * (-221 * s + 35139 * Math.Pow(s, 3) - 1546116 * Math.Pow(s, 5) + 29376204 * Math.Pow(s, 7) - 288866006 * Math.Pow(s, 9) + 1601893306 * Math.Pow(s, 11) - 5175347604 * Math.Pow(s, 13) + 9611359836 * Math.Pow(s, 15) - 9470016309 * Math.Pow(s, 17) + 3821234651 * Math.Pow(s, 19));
                LegArrEx[35, 17] = (1.0 / 65536) * 61836869254970658257624840625.0 * Math.Pow(c, 17) * (-17 + 8109 * Math.Pow(s, 2) - 594660 * Math.Pow(s, 4) + 15817956 * Math.Pow(s, 6) - 199984158 * Math.Pow(s, 8) + 1355448182 * Math.Pow(s, 10) - 5175347604 * Math.Pow(s, 12) + 11090030580 * Math.Pow(s, 14) - 12383867481 * Math.Pow(s, 16) + 5584881413 * Math.Pow(s, 18));
                LegArrEx[35, 18] = ((3277354070513444887654116553125.0 * Math.Pow(c, 18) * (153 * s - 22440 * Math.Pow(s, 3) + 895356 * Math.Pow(s, 5) - 15093144 * Math.Pow(s, 7) + 127872470 * Math.Pow(s, 9) - 585888408 * Math.Pow(s, 11) + 1464721020 * Math.Pow(s, 13) - 1869263016 * Math.Pow(s, 15) + 948376089 * Math.Pow(s, 17))) / 32768);
                LegArrEx[35, 19] = (501435172788557067811079832628125.0 * Math.Pow(c, 19) * (1 - 440 * Math.Pow(s, 2) + 29260 * Math.Pow(s, 4) - 690536 * Math.Pow(s, 6) + 7521910 * Math.Pow(s, 8) - 42122696 * Math.Pow(s, 10) + 124453420 * Math.Pow(s, 12) - 183261080 * Math.Pow(s, 14) + 105375121 * Math.Pow(s, 16)) / 32768);
                LegArrEx[35, 20] = (501435172788557067811079832628125.0 * Math.Pow(c, 20) * (-55 * s + 7315 * Math.Pow(s, 3) - 258951 * Math.Pow(s, 5) + 3760955 * Math.Pow(s, 7) - 26326685 * Math.Pow(s, 9) + 93340065 * Math.Pow(s, 11) - 160353445 * Math.Pow(s, 13) + 105375121 * Math.Pow(s, 15))) / 2048;
                LegArrEx[35, 21] = ((2507175863942785339055399163140625.0 * Math.Pow(c, 21) * (-11 + 4389 * Math.Pow(s, 2) - 258951 * Math.Pow(s, 4) + 5265337 * Math.Pow(s, 6) - 47388033 * Math.Pow(s, 8) + 205348143 * Math.Pow(s, 10) - 416918957 * Math.Pow(s, 12) + 316125363 * Math.Pow(s, 14))) / 2048);
                LegArrEx[35, 22] = ((1000363169713171350283104266093109375.0 * Math.Pow(c, 22) * (11 * s - 1298 * Math.Pow(s, 3) + 39589 * Math.Pow(s, 5) - 475068 * Math.Pow(s, 7) + 2573285 * Math.Pow(s, 9) - 6269458 * Math.Pow(s, 11) + 5546059 * Math.Pow(s, 13))) / 1024);
                LegArrEx[35, 23] = ((1000363169713171350283104266093109375.0 * Math.Pow(c, 23) * (11 - 3894 * Math.Pow(s, 2) + 197945 * Math.Pow(s, 4) - 3325476 * Math.Pow(s, 6) + 23159565 * Math.Pow(s, 8) - 68964038 * Math.Pow(s, 10) + 72098767 * Math.Pow(s, 12))) / 1024);
                LegArrEx[35, 24] = 59021427013077109666703151699493453125.0 / 256 * Math.Pow(c, 24) * (-33 * s + 3355 * Math.Pow(s, 3) - 84546 * Math.Pow(s, 5) + 785070 * Math.Pow(s, 7) - 2922205 * Math.Pow(s, 9) + 3666039 * Math.Pow(s, 11));
                LegArrEx[35, 25] = (1947707091431544619001204006083283953125.0 / 256) * Math.Pow(c, 25) * (-1 + 305 * Math.Pow(s, 2) - 12810 * Math.Pow(s, 4) + 166530 * Math.Pow(s, 6) - 796965 * Math.Pow(s, 8) + 1222013 * Math.Pow(s, 10));
                LegArrEx[35, 26] = (594050662886621108795367221855401605703125.0 / 128) * Math.Pow(c, 26) * (s - 84 * Math.Pow(s, 3) + 1638 * Math.Pow(s, 5) - 10452 * Math.Pow(s, 7) + 20033 * Math.Pow(s, 9));
                LegArrEx[35, 27] = (594050662886621108795367221855401605703125.0 / 128) * Math.Pow(c, 27) * (1 - 252 * Math.Pow(s, 2) + 8190 * Math.Pow(s, 4) - 73164 * Math.Pow(s, 6) + 180297 * Math.Pow(s, 8));
                LegArrEx[35, 28] = 5346455965979589979158304996698614451328125.0 / 16 * Math.Pow(c, 28) * (-7 * s + 455 * Math.Pow(s, 3) - 6097 * Math.Pow(s, 5) + 20033 * Math.Pow(s, 7));
                LegArrEx[35, 29] = (37425191761857129854108134976890301159296875.0 / 16) * Math.Pow(c, 29) * (-1 + 195 * Math.Pow(s, 2) - 4355 * Math.Pow(s, 4) + 20033 * Math.Pow(s, 6));
                LegArrEx[35, 30] = (486527492904142688103405754699573915070859375.0 / 8) * Math.Pow(c, 30) * (15 * s - 670 * Math.Pow(s, 3) + 4623 * Math.Pow(s, 5));
                LegArrEx[35, 31] = (7297912393562140321551086320493608726062890625.0 / 8) * Math.Pow(c, 31) * (1 - 134 * Math.Pow(s, 2) + 1541 * Math.Pow(s, 4));
                LegArrEx[35, 32] = 488960130368663401543922783473071784646213671875.0 / 2 * Math.Pow(c, 32) * (-s + 23 * Math.Pow(s, 3));
                LegArrEx[35, 33] = (488960130368663401543922783473071784646213671875.0 / 2) * Math.Pow(c, 33) * (-1 + 69 * Math.Pow(s, 2));
                LegArrEx[35, 34] = 33738248995437774706530672059641953140588743359375.0 * s * Math.Pow(c, 34);
                LegArrEx[35, 35] = 33738248995437774706530672059641953140588743359375.0 * Math.Pow(c, 35);

                LegArrEx[36, 0] = (2268783825 - 1511010027450 * Math.Pow(s, 2) + 166966608033225.0 * Math.Pow(s, 4) - 7302006324653040.0 * Math.Pow(s, 6) + 168206931407186100.0 * Math.Pow(s, 8) - 2354897039700605400.0 * Math.Pow(s, 10) + 21800637746319240900.0 * Math.Pow(s, 12) - 140865659283908941200.0 * Math.Pow(s, 14) + 658546957152274300110.0 * Math.Pow(s, 16) - 2281241093403303131100.0 * Math.Pow(s, 18) + 5943233374919131841550.0 * Math.Pow(s, 20) - 11732097051788416102800.0 * Math.Pow(s, 22) + 17555637979668898008900.0 * Math.Pow(s, 24) - 19770349232488666680792.0 * Math.Pow(s, 26) + 16475291027073888900660.0 * Math.Pow(s, 28) - 9847300383998186469360.0 * Math.Pow(s, 30) + 3990539066902490887785.0 * Math.Pow(s, 32) - 981629930895799897530.0 * Math.Pow(s, 34) + 110628135069209194801.0 * Math.Pow(s, 36)) / 17179869184;
                LegArrEx[36, 1] = (1.0 / 4294967296) * 333 * c * (-2268783825 * s + 501401225325.0 * Math.Pow(s, 3) - 32891920381320.0 * Math.Pow(s, 5) + 1010251840283400.0 * Math.Pow(s, 7) - 17679407204959500.0 * Math.Pow(s, 9) + 196402141858731900.0 * Math.Pow(s, 11) - 1480569992473517400.0 * Math.Pow(s, 13) + 7910473959787078680.0 * Math.Pow(s, 15) - 30827582343287880150.0 * Math.Pow(s, 17) + 89237738362149126750.0 * Math.Pow(s, 19) - 193773374729238103800.0 * Math.Pow(s, 21) + 316317801435475639800.0 * Math.Pow(s, 23) - 385907717751280280556.0 * Math.Pow(s, 25) + 346327439007559226140.0 * Math.Pow(s, 27) - 221786044684643839400.0 * Math.Pow(s, 29) + 95868806412071853160.0 * Math.Pow(s, 31) - 25056619857700597985.0 * Math.Pow(s, 33) + 2989949596465113373.0 * Math.Pow(s, 35));
                LegArrEx[36, 2] = (1.0 / 4294967296) * 221445.0 * c * c * (-3411705 + 2261960415 * Math.Pow(s, 2) - 247307672040 * Math.Pow(s, 4) + 10634229897720 * Math.Pow(s, 6) - 239270172698700 * Math.Pow(s, 8) + 3248757233753460 * Math.Pow(s, 10) - 28943473537076280 * Math.Pow(s, 12) + 178431743453843880.0 * Math.Pow(s, 14) - 788073533587810470.0 * Math.Pow(s, 16) + 2549649667489975050.0 * Math.Pow(s, 18) - 6119159201975940120 * Math.Pow(s, 20) + 10940314936866074760.0 * Math.Pow(s, 22) - 14507808938018055660 * Math.Pow(s, 24) + 14061414816848269332.0 * Math.Pow(s, 26) - 9671872625345370440 * Math.Pow(s, 28) + 4469072178607860824 * Math.Pow(s, 30) - 1243411210983638697.0 * Math.Pow(s, 32) + 157365768235005967.0 * Math.Pow(s, 34));
                LegArrEx[36, 3] = (1.0 / 2147483648) * 48939345.0 * Math.Pow(c, 3) * (10235115 * s - 2238078480 * Math.Pow(s, 3) + 144356061960 * Math.Pow(s, 5) - 4330681858800 * Math.Pow(s, 7) + 73501294881300 * Math.Pow(s, 9) - 785795661640080 * Math.Pow(s, 11) + 5651684181795960.0 * Math.Pow(s, 13) - 28527548727160560 * Math.Pow(s, 15) + 103831886911356450 * Math.Pow(s, 17) - 276885031763617200 * Math.Pow(s, 19) + 544540562468447160.0 * Math.Pow(s, 21) - 787754331476093520 * Math.Pow(s, 23) + 827142048049898196 * Math.Pow(s, 25) - 612697813370294960 * Math.Pow(s, 27) + 303330690855737160 * Math.Pow(s, 29) - 90020721157186512 * Math.Pow(s, 31) + 12105059095000459 * Math.Pow(s, 33));
                LegArrEx[36, 4] = (1.0 / 2147483648) * 1614998385.0 * Math.Pow(c, 4) * (310155 - 203461680 * Math.Pow(s, 2) + 21872130600 * Math.Pow(s, 4) - 918629485200 * Math.Pow(s, 6) + 20045807694900 * Math.Pow(s, 8) - 261931887213360 * Math.Pow(s, 10) + 2226421041313560 * Math.Pow(s, 12) - 12967067603254800 * Math.Pow(s, 14) + 53489153863426050 * Math.Pow(s, 16) - 159418654651779600 * Math.Pow(s, 18) + 346525812479920920 * Math.Pow(s, 20) - 549040897695459120 * Math.Pow(s, 22) + 626622763674165300 * Math.Pow(s, 24) - 501298210939332240 * Math.Pow(s, 26) + 266563334388375080 * Math.Pow(s, 28) - 84564919874932784.0 * Math.Pow(s, 30) + 12105059095000459 * Math.Pow(s, 32));
                LegArrEx[36, 5] = (1.0 / 67108864) * 66214933785.0 * Math.Pow(c, 5) * (-310155 * s + 66683325 * Math.Pow(s, 3) - 4201049475 * Math.Pow(s, 5) + 122230534725 * Math.Pow(s, 7) - 1996432067175 * Math.Pow(s, 9) + 20363607085185 * Math.Pow(s, 11) - 138368099424975 * Math.Pow(s, 13) + 652306754432025 * Math.Pow(s, 15) - 2187146176625025 * Math.Pow(s, 17) + 5282405678047575 * Math.Pow(s, 19) - 9206478467454345 * Math.Pow(s, 21) + 11462611530624975 * Math.Pow(s, 23) - 9934263326541645 * Math.Pow(s, 25) + 5688851648532395 * Math.Pow(s, 27) - 1933649082506085 * Math.Pow(s, 29) + 295245343780499 * Math.Pow(s, 31));
                LegArrEx[36, 6] = (1.0 / 67108864) * 2052662947335.0 * Math.Pow(c, 6) * (-10005 + 6453225 * Math.Pow(s, 2) - 677588625 * Math.Pow(s, 4) + 27600443325 * Math.Pow(s, 6) - 579609309825 * Math.Pow(s, 8) + 7225796062485 * Math.Pow(s, 10) - 58025332016925 * Math.Pow(s, 12) + 315632300531625 * Math.Pow(s, 14) - 1199402742020175 * Math.Pow(s, 16) + 3237603480093675 * Math.Pow(s, 18) - 6236646703759395 * Math.Pow(s, 20) + 8504518232399175 * Math.Pow(s, 22) - 8011502682694875 * Math.Pow(s, 24) + 4954806274528215 * Math.Pow(s, 26) - 1808897528796015 * Math.Pow(s, 28) + 295245343780499 * Math.Pow(s, 30));
                LegArrEx[36, 7] = (1.0 / 33554432) * 1323967601031075.0 * Math.Pow(c, 7) * (10005 * s - 2101050 * Math.Pow(s, 3) + 128374155 * Math.Pow(s, 5) - 3594476340 * Math.Pow(s, 7) + 56013922965 * Math.Pow(s, 9) - 539770530390 * Math.Pow(s, 11) + 3425466827475 * Math.Pow(s, 13) - 14876313079320 * Math.Pow(s, 15) + 45175862512935 * Math.Pow(s, 17) - 96692196957510 * Math.Pow(s, 19) + 145038295436265 * Math.Pow(s, 21) - 149051212701300 * Math.Pow(s, 23) + 99864312509871 * Math.Pow(s, 25) - 39262892097898 * Math.Pow(s, 27) + 6866170785593 * Math.Pow(s, 29));
                LegArrEx[36, 8] = (1.0 / 33554432) * 38395060429901175.0 * Math.Pow(c, 8) * (345 - 217350 * Math.Pow(s, 2) + 22133475 * Math.Pow(s, 4) - 867632220 * Math.Pow(s, 6) + 17383631265 * Math.Pow(s, 8) - 204740546010 * Math.Pow(s, 10) + 1535554095075 * Math.Pow(s, 12) - 7694644696200 * Math.Pow(s, 14) + 26482402162755 * Math.Pow(s, 16) - 63350060075610 * Math.Pow(s, 18) + 105027731177985 * Math.Pow(s, 20) - 118213030763100 * Math.Pow(s, 22) + 86089924577475 * Math.Pow(s, 24) - 36555106435974 * Math.Pow(s, 26) + 6866170785593 * Math.Pow(s, 28));
                LegArrEx[36, 9] = (1.0 / 8388608) * 268765423009308225.0 * Math.Pow(c, 9) * (-15525 * s + 3161925 * Math.Pow(s, 3) - 185921190 * Math.Pow(s, 5) + 4966751790 * Math.Pow(s, 7) - 73121623575 * Math.Pow(s, 9) + 658094612175 * Math.Pow(s, 11) - 3847322348100 * Math.Pow(s, 13) + 15132801235860 * Math.Pow(s, 15) - 40725038620035 * Math.Pow(s, 17) + 75019807984275 * Math.Pow(s, 19) - 92881667028150 * Math.Pow(s, 21) + 73791363923550 * Math.Pow(s, 23) - 33944027404833 * Math.Pow(s, 25) + 6866170785593 * Math.Pow(s, 27));
                LegArrEx[36, 10] = (1.0 / 8388608) * 166903327688780407725.0 * Math.Pow(c, 10) * (-25 + 15275 * Math.Pow(s, 2) - 1496950 * Math.Pow(s, 4) + 55985930 * Math.Pow(s, 6) - 1059733675 * Math.Pow(s, 8) + 11657070425 * Math.Pow(s, 10) - 80539759300 * Math.Pow(s, 12) + 365526599900 * Math.Pow(s, 14) - 1114856129695 * Math.Pow(s, 16) + 2295292031725 * Math.Pow(s, 18) - 3140925938150 * Math.Pow(s, 20) + 2733013478650 * Math.Pow(s, 22) - 1366506739325 * Math.Pow(s, 24) + 298529164591 * Math.Pow(s, 26));
                LegArrEx[36, 11] = (1.0 / 4194304) * 7844456401372679163075.0 * Math.Pow(c, 11) * (325 * s - 63700 * Math.Pow(s, 3) + 3573570 * Math.Pow(s, 5) - 90190100 * Math.Pow(s, 7) + 1240113875 * Math.Pow(s, 9) - 10281671400 * Math.Pow(s, 11) + 54440131900 * Math.Pow(s, 13) - 189762745480 * Math.Pow(s, 15) + 439524006075 * Math.Pow(s, 17) - 668282114500 * Math.Pow(s, 19) + 639641452450 * Math.Pow(s, 21) - 348895337700 * Math.Pow(s, 23) + 82571896589 * Math.Pow(s, 25));
                LegArrEx[36, 12] = (1.0 / 4194304) * 196111410034316979076875.0 * Math.Pow(c, 12) * (13 - 7644 * Math.Pow(s, 2) + 714714 * Math.Pow(s, 4) - 25253228 * Math.Pow(s, 6) + 446440995 * Math.Pow(s, 8) - 4523935416 * Math.Pow(s, 10) + 28308868588 * Math.Pow(s, 12) - 113857647288 * Math.Pow(s, 14) + 298876324131 * Math.Pow(s, 16) - 507894407020 * Math.Pow(s, 18) + 537298820058 * Math.Pow(s, 20) - 320983710684 * Math.Pow(s, 22) + 82571896589 * Math.Pow(s, 24));
                LegArrEx[36, 13] = (1.0 / 524288) * 4118339610720656560614375.0 * Math.Pow(c, 13) * (-91 * s + 17017 * Math.Pow(s, 3) - 901901 * Math.Pow(s, 5) + 21259095 * Math.Pow(s, 7) - 269281870 * Math.Pow(s, 9) + 2022062042 * Math.Pow(s, 11) - 9488137274 * Math.Pow(s, 13) + 28464411822 * Math.Pow(s, 15) - 54417257895 * Math.Pow(s, 17) + 63964145245 * Math.Pow(s, 19) - 42033581161 * Math.Pow(s, 21) + 11795985227 * Math.Pow(s, 23));
                LegArrEx[36, 14] = (1.0 / 524288) * 4118339610720656560614375.0 * Math.Pow(c, 14) * (-91 + 51051 * Math.Pow(s, 2) - 4509505 * Math.Pow(s, 4) + 148813665 * Math.Pow(s, 6) - 2423536830 * Math.Pow(s, 8) + 22242682462 * Math.Pow(s, 10) - 123345784562 * Math.Pow(s, 12) + 426966177330 * Math.Pow(s, 14) - 925093384215 * Math.Pow(s, 16) + 1215318759655 * Math.Pow(s, 18) - 882705204381 * Math.Pow(s, 20) + 271307660221 * Math.Pow(s, 22));
                LegArrEx[36, 15] = (1.0 / 262144) * 45301735717927222166758125.0 * Math.Pow(c, 15) * (4641 * s - 819910 * Math.Pow(s, 3) + 40585545 * Math.Pow(s, 5) - 881286120 * Math.Pow(s, 7) + 10110310210 * Math.Pow(s, 9) - 67279518852 * Math.Pow(s, 11) + 271705749210 * Math.Pow(s, 13) - 672795188520 * Math.Pow(s, 15) + 994351712445 * Math.Pow(s, 17) - 802459276710 * Math.Pow(s, 19) + 271307660221 * Math.Pow(s, 21));
                LegArrEx[36, 16] = (1.0 / 262144) * 12367373850994131651524968125.0 * Math.Pow(c, 16) * (17 - 9010 * Math.Pow(s, 2) + 743325 * Math.Pow(s, 4) - 22597080 * Math.Pow(s, 6) + 333306930 * Math.Pow(s, 8) - 2710896364 * Math.Pow(s, 10) + 12938369010 * Math.Pow(s, 12) - 36966768600 * Math.Pow(s, 14) + 61919337405 * Math.Pow(s, 16) - 55848814130 * Math.Pow(s, 18) + 20869820017 * Math.Pow(s, 20));
                LegArrEx[36, 17] = (1.0 / 65536) * 3277354070513444887654116553125.0 * Math.Pow(c, 17) * (-17 * s + 2805 * Math.Pow(s, 3) - 127908 * Math.Pow(s, 5) + 2515524 * Math.Pow(s, 7) - 25574494 * Math.Pow(s, 9) + 146472102 * Math.Pow(s, 11) - 488240340 * Math.Pow(s, 13) + 934631508 * Math.Pow(s, 15) - 948376089 * Math.Pow(s, 17) + 393770189 * Math.Pow(s, 19));
                LegArrEx[36, 18] = (1.0 / 65536) * 3277354070513444887654116553125.0 * Math.Pow(c, 18) * (-17 + 8415 * Math.Pow(s, 2) - 639540 * Math.Pow(s, 4) + 17608668 * Math.Pow(s, 6) - 230170446 * Math.Pow(s, 8) + 1611193122 * Math.Pow(s, 10) - 6347124420 * Math.Pow(s, 12) + 14019472620 * Math.Pow(s, 14) - 16122393513 * Math.Pow(s, 16) + 7481633591 * Math.Pow(s, 18));
                LegArrEx[36, 19] = (1.0 / 32768) * 29496186634621003988887048978125.0 * Math.Pow(c, 19) * (935 * s - 142120 * Math.Pow(s, 3) + 5869556 * Math.Pow(s, 5) - 102297976 * Math.Pow(s, 7) + 895107290 * Math.Pow(s, 9) - 4231416280 * Math.Pow(s, 11) + 10904034260 * Math.Pow(s, 13) - 14331016456 * Math.Pow(s, 15) + 7481633591 * Math.Pow(s, 17));
                LegArrEx[36, 20] = (1.0 / 32768) * 501435172788557067811079832628125.0 * Math.Pow(c, 20) * (55 - 25080 * Math.Pow(s, 2) + 1726340 * Math.Pow(s, 4) - 42122696 * Math.Pow(s, 6) + 473880330 * Math.Pow(s, 8) - 2737975240 * Math.Pow(s, 10) + 8338379140 * Math.Pow(s, 12) - 12645014520 * Math.Pow(s, 14) + 7481633591 * Math.Pow(s, 16));
                LegArrEx[36, 21] = ((9527268282982584288410516819934375.0 * Math.Pow(c, 21) * (-165 * s + 22715 * Math.Pow(s, 3) - 831369 * Math.Pow(s, 5) + 12470535 * Math.Pow(s, 7) - 90064975 * Math.Pow(s, 9) + 329146545 * Math.Pow(s, 11) - 582336195 * Math.Pow(s, 13) + 393770189 * Math.Pow(s, 15))) / 2048);
                LegArrEx[36, 22] = ((142909024244738764326157752299015625.0 * Math.Pow(c, 22) * (-11 + 4543 * Math.Pow(s, 2) - 277123 * Math.Pow(s, 4) + 5819583 * Math.Pow(s, 6) - 54038985 * Math.Pow(s, 8) + 241374133 * Math.Pow(s, 10) - 504691369 * Math.Pow(s, 12) + 393770189 * Math.Pow(s, 14))) / 2048);
                LegArrEx[36, 23] = ((59021427013077109666703151699493453125.0 * Math.Pow(c, 23) * (11 * s - 1342 * Math.Pow(s, 3) + 42273 * Math.Pow(s, 5) - 523380 * Math.Pow(s, 7) + 2922205 * Math.Pow(s, 9) - 7332078 * Math.Pow(s, 11) + 6674071 * Math.Pow(s, 13))) / 1024);
                LegArrEx[36, 24] = (59021427013077109666703151699493453125.0 * Math.Pow(c, 24) * (11 - 4026 * Math.Pow(s, 2) + 211365 * Math.Pow(s, 4) - 3663660 * Math.Pow(s, 6) + 26299845 * Math.Pow(s, 8) - 80652858 * Math.Pow(s, 10) + 86762923 * Math.Pow(s, 12))) / 1024;
                LegArrEx[36, 25] = (10800921143393111069006676761007301921875.0 / 256) * Math.Pow(c, 25) * (-11 * s + 1155 * Math.Pow(s, 3) - 30030 * Math.Pow(s, 5) + 287430 * Math.Pow(s, 7) - 1101815 * Math.Pow(s, 9) + 1422343 * Math.Pow(s, 11));
                LegArrEx[36, 26] = (118810132577324221759073444371080321140625.0 / 256) * Math.Pow(c, 26) * (-1 + 315 * Math.Pow(s, 2) - 13650 * Math.Pow(s, 4) + 182910 * Math.Pow(s, 6) - 901485 * Math.Pow(s, 8) + 1422343 * Math.Pow(s, 10));
                LegArrEx[36, 27] = (594050662886621108795367221855401605703125.0 / 128) * Math.Pow(c, 27) * (63 * s - 5460 * Math.Pow(s, 3) + 109746 * Math.Pow(s, 5) - 721188 * Math.Pow(s, 7) + 1422343 * Math.Pow(s, 9));
                LegArrEx[36, 28] = 5346455965979589979158304996698614451328125.0 / 128 * Math.Pow(c, 28) * (7 - 1820 * Math.Pow(s, 2) + 60970 * Math.Pow(s, 4) - 560924 * Math.Pow(s, 6) + 1422343 * Math.Pow(s, 8));
                LegArrEx[36, 29] = (69503927557734669729057964957081987867265625.0 / 16) * Math.Pow(c, 29) * (-35 * s + 2345 * Math.Pow(s, 3) - 32361 * Math.Pow(s, 5) + 109411 * Math.Pow(s, 7));
                LegArrEx[36, 30] = (486527492904142688103405754699573915070859375.0 / 16) * Math.Pow(c, 30) * (-5 + 1005 * Math.Pow(s, 2) - 23115 * Math.Pow(s, 4) + 109411 * Math.Pow(s, 6));
                LegArrEx[36, 31] = (97792026073732680308784556694614356929242734375.0 / 8) * Math.Pow(c, 31) * (5 * s - 230 * Math.Pow(s, 3) + 1633 * Math.Pow(s, 5));
                LegArrEx[36, 32] = 488960130368663401543922783473071784646213671875.0 / 8 * Math.Pow(c, 32) * (1 - 138 * Math.Pow(s, 2) + 1633 * Math.Pow(s, 4));
                LegArrEx[36, 33] = (11246082998479258235510224019880651046862914453125.0 / 2) * Math.Pow(c, 33) * (-3 * s + 71 * Math.Pow(s, 3));
                LegArrEx[36, 34] = (33738248995437774706530672059641953140588743359375.0 / 2) * Math.Pow(c, 34) * (-1 + 71 * Math.Pow(s, 2));
                LegArrEx[36, 35] = 2395415678676082004163677716234578672981800778515625.0 * s * Math.Pow(c, 35);
                LegArrEx[36, 36] = 2395415678676082004163677716234578672981800778515625.0 * Math.Pow(c, 36);

                if (order > 40)
                {
                    LegArrEx[50, 0] = 1.0 / 140737488355328.0 * (-15801325804719.0 + 20146690401016725.0 * Math.Pow(s, 2)
                        - 4271098365015545700.0 * Math.Pow(s, 4) + 360195962116311020700.0 * Math.Pow(s, 6)
                        - 16131633446209072141350.0 * Math.Pow(s, 8) + 444157640885623119625170.0 * Math.Pow(s, 10)
                        - 8210186695158487968828900.0 * Math.Pow(s, 12) + 107995532682469341743826300.0 * Math.Pow(s, 14)
                        - 1052956443654076082002306425.0 * Math.Pow(s, 16) + 7838675747202566388239392275.0 * Math.Pow(s, 18)
                        - 45546831710061227855875205640.0 * Math.Pow(s, 20) + 209988639702230336218645428600.0 * Math.Pow(s, 22)
                        - 777566629622026824693679811700.0 * Math.Pow(s, 24) + 2332699888866080474081039435100.0 * Math.Pow(s, 26)
                        - 5702155283894863381086985285800.0 * Math.Pow(s, 28) + 11391202164838244317619747616920.0 * Math.Pow(s, 30)
                        - 18602568051449552212241926551825.0 * Math.Pow(s, 32) + 24770264410753681822717859419275.0 * Math.Pow(s, 34)
                        - 26736158411607148634044673658900.0 * Math.Pow(s, 36) + 23161195551449151519392896526700.0 * Math.Pow(s, 38)
                        - 15856510800607496040199752237510.0 * Math.Pow(s, 40) + 8379456927150302785471413784050.0 * Math.Pow(s, 42)
                        - 3295092998837116951580725082100.0 * Math.Pow(s, 44) + 907344448955148146087446037100.0 * Math.Pow(s, 46)
                        - 156050375086257748529223875175.0 * Math.Pow(s, 48) + 12611418068195524166851562157.0 * Math.Pow(s, 50));

                    double ex5001 = (-15801325804719.0 + 20146690401016725.0 * Math.Pow(s, 2) -
                              4271098365015545700.0 * Math.Pow(s, 4) + 360195962116311020700.0 * Math.Pow(s, 6) -
                               16131633446209072141350.0 * Math.Pow(s, 8) + 444157640885623119625170.0 * Math.Pow(s, 10) -
                                 8210186695158487968828900.0 * Math.Pow(s, 12) +
                                 107995532682469341743826300.0 * Math.Pow(s, 14) -
                                 1052956443654076082002306425.0 * Math.Pow(s, 16) +
                                 7838675747202566388239392275.0 * Math.Pow(s, 18) -
                                 45546831710061227855875205640.0 * Math.Pow(s, 20) +
                                 209988639702230336218645428600.0 * Math.Pow(s, 22) -
                                 777566629622026824693679811700.0 * Math.Pow(s, 24) +
                                 2332699888866080474081039435100.0 * Math.Pow(s, 26) -
                                 5702155283894863381086985285800.0 * Math.Pow(s, 28) +
                                 1391202164838244317619747616920.0 * Math.Pow(s, 30) -
                                 18602568051449552212241926551825.0 * Math.Pow(s, 32) +
                                 24770264410753681822717859419275.0 * Math.Pow(s, 34) -
                                 26736158411607148634044673658900.0 * Math.Pow(s, 36) +
                                 23161195551449151519392896526700.0 * Math.Pow(s, 38) -
                                 15856510800607496040199752237510.0 * Math.Pow(s, 40) +
                                 8379456927150302785471413784050.0 * Math.Pow(s, 42) -
                                 3295092998837116951580725082100.0 * Math.Pow(s, 44) +
                                 907344448955148146087446037100.0 * Math.Pow(s, 46) -
                                 156050375086257748529223875175.0 * Math.Pow(s, 48) +
                                 12611418068195524166851562157.0 * Math.Pow(s, 50)) / 140737488355328.0;

                    LegArrEx[50, 21] = 1.0 / 33554432 * 14307987156536070552844545071598923765625.0 * Math.Pow(c, 21) * (3933 * s - 1339842 * Math.Pow(s, 3)
                        + 130634595 * Math.Pow(s, 5) - 5747922180 * Math.Pow(s, 7) + 138748454845 * Math.Pow(s, 9) - 2043386334990 * Math.Pow(s, 11) + 19569353746635 * Math.Pow(s, 13)
                        - 126734862359160 * Math.Pow(s, 15) + 567511258652415 * Math.Pow(s, 17) - 1772228141054910.0 * Math.Pow(s, 19) + 3839827638952305.0 * Math.Pow(s, 21)
                        - 5645912575850820.0 * Math.Pow(s, 23) + 5363616947058279 * Math.Pow(s, 25) - 2964506232847026 * Math.Pow(s, 27) + 722872209487329 * Math.Pow(s, 29));
                }
            }
        } // LegPolyEx





        // ----------------------------------------------------------------------------
        // fukushima method (JG 2018)
        //   for very large spherical harmonic expansions and calcs of normalized associated 
        //   Legendre polynomials
        //   
        //   Plm are converted to X-numbers
        //   Clm, Slm treated as F-numbers
        // -----------------------------------------------------------------------------

        // initialize legendre function values
        public void pinit
            (
            Int32 n,
            Int32 m,
            ref double[] p
            )
        {
            p = new double[360];

            if (n == 0)
                p[0] = 1.0;
            else if (n == 1)
                p[0] = 1.7320508075688773;
            else if (n == 2)
            {
                if (m == 0)
                {
                    p[0] = 0.5590169943749474;
                    p[1] = 1.6770509831248423;
                }
                else if (m == 1)
                {
                    p[0] = 0.0;
                    p[1] = 1.9364916731037084;
                }
                else if (m == 2)
                {
                    p[0] = 0.9682458365518542;
                    p[1] = -0.9682458365518542;
                }
                else if (n == 3)
                {
                    if (m == 0)
                    {
                        p[0] = 0.9921567416492215;
                        p[1] = 1.6535945694153691;
                    }
                    else if (m == 1)
                    {
                        p[0] = 0.4050462936504913;
                        p[1] = 2.0252314682524563;
                    }
                    else if (m == 2)
                    {
                        p[0] = 1.2808688457449498;
                        p[1] = -1.2808688457449498;
                    }
                    else if (m == 3)
                    {
                        p[0] = 1.5687375497513917;
                        p[1] = -0.5229125165837972;
                    }
                }
                else if (n == 4)
                {
                    if (m == 0)
                    {
                        p[0] = 0.421875;
                        p[1] = 0.9375;
                        p[2] = 1.640625;
                    }
                    else if (m == 1)
                    {
                        p[0] = 0.0;
                        p[1] = 0.5929270612815711;
                        p[2] = 2.0752447144854989;
                    }
                    else if (m == 2)
                    {
                        p[0] = 0.6288941186718159;
                        p[1] = 0.8385254915624211;
                        p[2] = -1.4674196102342370;
                    }
                    else if (m == 3)
                    {
                        p[0] = 0.0;
                        p[1] = 1.5687375497513917;
                        p[2] = -0.7843687748756958;
                    }
                    else if (m == 4)
                    {
                        p[0] = 0.8319487194983835;
                        p[1] = -1.1092649593311780;
                        p[2] = 0.2773162398327945;
                    }
                }
            }
        }  // pinit


        /* ----------------------------------------------------------------------------
        *      x2f
        *      
        *      convert x to f number
        *      
           ------------------------------------------------------------------------------*/
        public double x2f
            (
              double x,
              Int32 ix
            )
        {
            double x2fv;
            Int32 IND;
            double BIG, BIGI;

            IND = 960;
            BIG = Math.Pow(2.0, IND);
            BIGI = Math.Pow(2.0, -IND);

            if (ix == 0)
                x2fv = x;
            else if (ix == -1)
                x2fv = x * BIGI;
            else if (ix == 1)
                x2fv = x * BIG;
            else if (ix < 0)
                x2fv = 0.0;
            else if (x < 0)
                x2fv = -BIG;
            else
                x2fv = BIG;

            return x2fv;
        }


        /* ----------------------------------------------------------------------------
         *                                  xnorm
         *                                  
         * uses the "x" factor approach - value and exponent
         * 
          ------------------------------------------------------------------------------*/

        public void xnorm
                (
                ref double x,
                ref Int32 ix
                )
        {
            Int32 IND = 960;
            double w;

            double BIG = Math.Pow(2, IND);
            double BIGI = Math.Pow(2, -IND);
            double BIGS = Math.Pow(2.0, 480);  // IND / 2
            double BIGSI = Math.Pow(2.0, -480);  // IND / 2

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
        }  // xnorm


        /* ----------------------------------------------------------------------------
         *                                       xl2sum
         *                                       
         * routine to compute the two-term linear sum of X-numbers
         * with F-number coefficients
         * 
          ---------------------------------------------------------------------------- */
        public void xlsum2
                (
                   double f,
                   double g,
                   double x,
                   double y,
                   out double z,
                   Int32 ix,
                   Int32 iy,
                   out Int32 iz)
        {
            Int32 id;
            Int32 IND = 960;
            double BIGI = Math.Pow(2, -IND);

            id = ix - iy;
            if (id == 0)
            {
                z = f * x + g * y;
                iz = ix;
            }
            else if (id == 1)
            {
                z = f * x + g * (y * BIGI);
                iz = ix;
            }
            else if (id == -1)
            {
                z = g * y + f * (x * BIGI);
                iz = iy;
            }
            else if (id > 1)
            {
                z = f * x;
                iz = ix;
            }
            else
            {
                z = g * y;
                iz = iy;
            }

            xnorm(ref z, ref iz);
        }  // xlsum2


        /* ----------------------------------------------------------------------------
         *                                  dpeven
         *                                  
         * find Pnnj and Pn,n-1,j when degree n is even and n >= 6. the returned values are 
         * (xp[j], ip[j]) and (xp1[j], ip1[j]), double X-number vectors representing
         * Pnnj and Pn,n-1,j, respectively. required initial values for Pn-2,n-2,j as 
         * (xpold[j], ipold[j]) are needed. 
         * 
         *  inputs          description                                  range / units
         *    n           - degree 
         *
         *
         *
         *  outputs       :
         *    xp1         - value
         *    ip1         - exponent
         *
         *  locals        :
         *                -
         *
         *  coupling      :
         *    xnorm 
         *    xlsum2  
         *
         *  references    :
         * Fukushima (2012a)
           ---------------------------------------------------------------------------- */

        public void dpeven
            (
            Int32 n,
            double[] xpold,
            out double[] xp,
            out double[] xp1,
            Int32[] ipold,
            out Int32[] ip,
            out Int32[] ip1
            )
        {
            xpold = new double[360];
            xp = new double[360];
            xp1 = new double[360];
            ipold = new Int32[360];
            ip = new Int32[360];
            ip1 = new Int32[360];

            Int32 jx, jxm2, jxm1, n2, j, itemp, jm1, jp1;
            double gamma, gamma2, xtemp, alpha2;

            jx = n / 2;
            jxm2 = jx - 2;
            jxm1 = jx - 1;
            n2 = n * 2;
            gamma = Math.Sqrt(Convert.ToDouble(n2 + 1) * Convert.ToDouble(n2 - 1) / (Convert.ToDouble(n) * Convert.ToDouble(n - 1))) * 0.125;
            gamma2 = gamma * 2.0;
            xlsum2(gamma2, xpold[0], -gamma, xpold[1], out xp[0], ipold[0], ipold[1], out ip[0]);
            xlsum2(-gamma2, xpold[0], gamma2, xpold[1], out xtemp, ipold[0], ipold[1], out itemp);
            xlsum2(1.0, xtemp, -gamma, xpold[2], out xp[1], itemp, ipold[2], out ip[1]);
            j = 2;
            while (j <= jxm2)
            {
                jm1 = j - 1;
                jp1 = j + 1;
                xlsum2(-gamma, xpold[jm1], gamma2, xpold[j], out xtemp, ipold[jm1], ipold[j], out itemp);
                xlsum2(1.0, xtemp, -gamma, xpold[jp1], out xp[j], itemp, ipold[jp1], out ip[j]);
                j = j + 1;
            }
            xlsum2(-gamma, xpold[jxm2], gamma2, xpold[jxm1], out xp[jxm1], ipold[jxm2], ipold[jxm1], out ip[jxm1]);
            xp[jx] = -gamma * xpold[jxm1];
            ip[jx] = ipold[jxm1];
            xnorm(ref xp[jx], ref ip[jx]);
            alpha2 = Math.Sqrt(2.0 / Convert.ToDouble(n)) * 2.0;
            xp1[0] = 0.0;
            ip1[0] = 0;
            j = 1;
            while (j <= jx)
            {
                xp1[j] = -Convert.ToDouble(j) * alpha2 * xp[j];
                ip1[j] = ip[j];
                xnorm(ref xp1[j], ref ip1[j]);
                j = j + 1;
            }
        }   // dpeven


        /* ----------------------------------------------------------------------------
         *                                 dpodd
         *                                 
         *  routine to return Pnnj and Pn,n-1,j when n is odd and n >= 5. Same as Table 4 
         *  but when n is odd and n >= 5.
         ---------------------------------------------------------------------------- */
        public void dpodd
            (
            Int32 n,
            double[] xpold,
            out double[] xp,
            out double[] xp1,
            Int32[] ipold,
            out Int32[] ip,
            out Int32[] ip1
     )
        {
            xpold = new double[360];
            xp = new double[360];
            xp1 = new double[360];
            ipold = new Int32[360];
            ip = new Int32[360];
            ip1 = new Int32[360];

            Int32 jx, jxm2, jxm1, n2, j, itemp, jm1, jp1;
            double gamma, gamma2, xtemp, alpha;

            jx = (n - 1) / 2;
            jxm2 = jx - 2;
            jxm1 = jx - 1;
            n2 = n * 2;
            gamma = Math.Sqrt(Convert.ToDouble(n2 + 1) * Convert.ToDouble(n2 - 1) / (Convert.ToDouble(n) * Convert.ToDouble(n - 1))) * 0.125;
            gamma2 = gamma * 2.0;
            xlsum2(gamma * 3.0, xpold[0], -gamma, xpold[1], out xp[0], ipold[0], ipold[1], out ip[0]);
            j = 1;
            while (j <= jxm2)
            {
                jm1 = j - 1;
                jp1 = j + 1;
                xlsum2(-gamma, xpold[jm1], gamma2, xpold[j], out xtemp, ipold[jm1], ipold[j], out itemp);
                xlsum2(1.0, xtemp, -gamma, xpold[jp1], out xp[j], itemp, ipold[jp1], out ip[j]);
                j = j + 1;
            }
            xlsum2(-gamma, xpold[jxm2], gamma2, xpold[jxm1], out xp[jxm1], ipold[jxm2], ipold[jxm1], out ip[jxm1]);
            xp[jx] = -gamma * xpold[jxm1];
            ip[jx] = ipold[jxm1];
            xnorm(ref xp[jx], ref ip[jx]);
            alpha = Math.Sqrt(2.0 / Convert.ToDouble(n));
            j = 0;
            while (j <= jx)
            {
                xp1[j] = Convert.ToDouble(2 * j + 1) * alpha * xp[j];
                ip1[j] = ip[j];
                xnorm(ref xp1[j], ref ip1[j]);
                j = j + 1;
            }
        }   // dpodd


        /* ----------------------------------------------------------------------------
         *                             gpeven
         *                             
         * routine to return Pnmj when n is even. The returned values are (xp0[j], ip0[j]), 
         * a double X-number vector representing Pnmj. We assume that Pn,m+1,j and Pn,m+2,j are
         * externally provided as (xp1[j], ip1[j]) and (xp2[j], ip2[j]), respectively.
         * The routine internally calls xnorm and xlsum2 provided in Tables 7 and 8 of 
         * Fukushima (2012a).
          ---------------------------------------------------------------------------- */

        public void gpeven
            (
            Int32 jmax,
            Int32 n,
            Int32 m,
            double[] xp2,
            double[] xp1,
            out double[] xp0,
            Int32[] ip2,
            Int32[] ip1,
            out Int32[] ip0
            )
        {
            xp2 = new double[360];
            xp1 = new double[360];
            xp0 = new double[360];
            ip2 = new Int32[360];
            ip1 = new Int32[360];
            ip0 = new Int32[360];

            Int32 j, m1, m2, modd;
            double u, alpha2, beta;

            m1 = m + 1;
            m2 = m + 2;
            modd = m - Convert.ToInt32(m * 0.5) * 2;
            if (m == 0)
                u = Math.Sqrt(0.5 / (Convert.ToDouble(n) * Convert.ToDouble(n + 1)));
            else
                u = Math.Sqrt(1.0 / (Convert.ToDouble(n - m) * Convert.ToDouble(n + m1)));

            alpha2 = 4.0 * u;
            beta = Math.Sqrt(Convert.ToDouble(n - m1) * Convert.ToDouble(n + m2)) * u;
            xp0[0] = beta * xp2[0];
            ip0[0] = ip2[0];
            xnorm(ref xp0[0], ref ip0[0]);
            if (modd == 0)
            {
                j = 1;
                while (j <= jmax)
                {
                    xlsum2(Convert.ToDouble(j) * alpha2, xp1[j], beta, xp2[j], out xp0[j], ip1[j], ip2[j], out ip0[j]);
                    j = j + 1;
                }
            }
            else
            {
                j = 1;
                while (j <= jmax)
                {
                    xlsum2(-Convert.ToDouble(j) * alpha2, xp1[j], beta, xp2[j], out xp0[j], ip1[j], ip2[j], out ip0[j]);
                    j = j + 1;
                }
            }
        }  // gpeven


        /* ----------------------------------------------------------------------------
        *                                 gpodd
        *                                 
        * Table 7: Fortran routine to return Pnmj when n is odd. Same as
        * Table 6 but when n is odd.
         ---------------------------------------------------------------------------- */
        public void gpodd
            (
            Int32 jmax,
            Int32 n,
            Int32 m,
            double[] xp2,
            double[] xp1,
            out double[] xp0,
            Int32[] ip2,
            Int32[] ip1,
            out Int32[] ip0
            )
        {
            xp2 = new double[360];
            xp1 = new double[360];
            xp0 = new double[360];
            ip2 = new Int32[360];
            ip1 = new Int32[360];
            ip0 = new Int32[360];

            Int32 j, m1, m2, modd;
            double u, alpha, beta;

            m1 = m + 1;
            m2 = m + 2;
            modd = m - Convert.ToInt32(m * 0.5) * 2;
            if (m == 0)
                u = Math.Sqrt(0.50 / (Convert.ToDouble(n) * Convert.ToDouble(n + 1)));
            else
                u = Math.Sqrt(1.0 / (Convert.ToDouble(n - m) * Convert.ToDouble(n + m1)));

            alpha = 2.0 * u;
            beta = Math.Sqrt(Convert.ToDouble(n - m1) * Convert.ToDouble(n + m2)) * u;
            if (modd == 0)
            {
                j = 0;
                while (j <= jmax)
                {
                    xlsum2(Convert.ToDouble(2 * j + 1) * alpha, xp1[j], beta, xp2[j], out xp0[j], ip1[j], ip2[j], out ip0[j]);
                    j = j + 1;
                }
            }
            else
            {
                j = 0;
                while (j <= jmax)
                {
                    xlsum2(-Convert.ToDouble(2 * j + 1) * alpha, xp1[j], beta, xp2[j], out xp0[j], ip1[j], ip2[j], out ip0[j]);
                    j = j + 1;
                }
            }
        }  // gpodd



        // Fukushima combined approach to find matrix of normalized Legendre polynomials
        //
        //

        public void LegPolyFF
            (
                double[] recef,
                double latgc,
                Int32 order,
                char normalized,
                double[,,] normArr,
                AstroLib.gravityConst gravData,
                out double[,] ALFArr
            )
        {
            Int32 L, m, ix, iy, iz;
            double x, y, z, f, g;

            ALFArr = new double[order + 2, order + 2];
            double magr = MathTimeLibr.mag(recef);

            // initial values
            ALFArr[0, 0] = 1.0;
            ALFArr[0, 1] = 0.0;
            ALFArr[1, 0] = Math.Sin(latgc);
            ALFArr[1, 1] = Math.Cos(latgc);
            m = 2;
            L = m + 1;
            x = ALFArr[1, 0];
            y = ALFArr[1, 1];

            // find zonal terms
            for (L = 2; L <= order + 1; L++)
            {
                // find tesseral and sectoral terms
                //               for (m = 0; m <= order + 1; m++)
                {
                    f = normArr[L, m, 0] * Math.Sin(latgc);
                    g = -normArr[L, m, 1];
                    z = f * x + g * y;
                    ALFArr[L, m] = z;
                    y = x;
                    x = z;
                }
            }

        }  // LegPolyFF



        /* ----------------------------------------------------------------------------
        *                                   xfsh2f
        *
        *  transfrom the Cnm, Snm, the 4 fully normalized spherical harmonic coefficients 
        *  of a given function depend on the spherical surface, to (Akm, Bkm), the 
        *  corresponding Fourier series coefficients of the function. in the program, 
        *  (i) x2f and xnorm are the Fortran function/routine to handle X-numbers
        *      (Fukushima, 2012a, tables 6 and 7), and 
        *  (ii) pinit, dpeven, dpodd, gpeven, and gpodd are the Fortran routines listed 
        *  in Tables 3–7, respectively.
        *  
        *    
        *  
        *    Fukushima 2018 table xx
          ---------------------------------------------------------------------------- */

        public void xfsh2f
            (
            Int32 nmax,
            AstroLib.gravityConst gravData,
            out double[,] a,
            out double[,] b
            )
        {
            Int32 NX = 100;  // 2200;
            Int32 j, m, k, L, jmax, n1;
            a = new double[NX, NX];
            b = new double[NX, NX];
            //     double[,] pja = new double[NX, NX];  // test to see the values
            jmax = 0;

            Int32[] ipold, ip, ip0;
            Int32[] ip1, ip2;
            double[] xpold, xp, xp0;
            double[] xp1, xp2;
            double pj;
            xpold = new double[NX];
            xp = new double[NX];
            xp0 = new double[NX];
            xp1 = new double[NX];
            xp2 = new double[NX];
            ipold = new Int32[NX];
            ip = new Int32[NX];
            ip0 = new Int32[NX];
            ip1 = new Int32[NX];
            ip2 = new Int32[NX];

            // initialize all to 0
            for (m = 0; m <= nmax; m++)
            {
                for (k = 0; k <= nmax; k++)
                {
                    a[k, m] = 0.0;
                    b[k, m] = 0.0;
                }
            }

            // initialize the first 4x4 values
            for (L = 0; L <= 4; L += 2)
            {
                jmax = Convert.ToInt32(L * 0.5);

                for (m = 0; m <= L; m++)
                {
                    pinit(L, m, ref xp);

                    for (j = 0; j <= jmax; j++)
                    {
                        k = 2 * j;
                        a[k, m] = a[k, m] + xp[j] * gravData.c[L, m];
                        b[k, m] = b[k, m] + xp[j] * gravData.s[L, m];
                    }
                }
            }

            // even terms
            for (j = 0; j <= jmax; j++)
            {
                ip[j] = 0;
            }

            for (L = 6; L <= nmax; L += 2)
            {

                for (j = 0; j <= jmax; j++)
                {
                    xpold[j] = xp[j];
                    ipold[j] = ip[j];
                }
                jmax = Convert.ToInt32(L * 0.5);
                n1 = L - 1;
                dpeven(L, xpold, out xp, out xp1, ipold, out ip, out ip1);

                for (j = 0; j <= jmax; j++)
                {
                    k = 2 * j;
                    pj = x2f(xp[j], ip[j]);
                    //       pja[k, n1] = pj;
                    a[k, L] = a[k, L] + pj * gravData.c[L, L];
                    b[k, L] = b[k, L] + pj * gravData.s[L, L];
                    pj = x2f(xp1[j], ip1[j]);
                    a[k, n1] = a[k, n1] + pj * gravData.c[L, n1];
                    b[k, n1] = b[k, n1] + pj * gravData.s[L, n1];
                    xp2[j] = xp[j];
                    ip2[j] = ip[j];
                }

                for (m = L - 2; m >= 0; m -= 1)
                {
                    gpeven(jmax, L, m, xp2, xp1, out xp0, ip2, ip1, out ip0);

                    for (j = 0; j <= jmax; j++)
                    {
                        k = 2 * j;
                        pj = x2f(xp0[j], ip0[j]);
                        //         pja[k, m] = pj;
                        a[k, m] = a[k, m] + pj * gravData.c[L, m];
                        b[k, m] = b[k, m] + pj * gravData.s[L, m];
                        xp2[j] = xp1[j];
                        ip2[j] = ip1[j];
                        xp1[j] = xp0[j];
                        ip1[j] = ip0[j];
                    }
                }
            }

            for (L = 1; L <= 3; L += 2)
            {
                jmax = Convert.ToInt32((L - 1) * 0.5);

                for (m = 0; m <= L; m++)
                {
                    pinit(L, m, ref xp);

                    for (j = 0; j <= jmax; j++)
                    {
                        k = 2 * j + 1;
                        a[k, m] = a[k, m] + xp[j] * gravData.c[L, m];
                        b[k, m] = b[k, m] + xp[j] * gravData.s[L, m];
                    }
                }
            }

            // odd terms
            for (j = 0; j <= jmax; j++)
            {
                ip[j] = 0;
            }

            for (L = 5; L <= nmax; L += 2)
            {

                for (j = 0; j <= jmax; j++)
                {
                    xpold[j] = xp[j];
                    ipold[j] = ip[j];
                }
                jmax = Convert.ToInt32((L - 1) * 0.5);
                n1 = L - 1;
                dpodd(L, xpold, out xp, out xp1, ipold, out ip, out ip1);

                for (j = 0; j <= jmax; j++)
                {
                    k = 2 * j + 1;
                    pj = x2f(xp[j], ip[j]);
                    a[k, L] = a[k, L] + pj * gravData.c[L, L];
                    b[k, L] = b[k, L] + pj * gravData.s[L, L];
                    //   pja[k, n] = pj;
                    pj = x2f(xp1[j], ip1[j]);
                    a[k, n1] = a[k, n1] + pj * gravData.c[L, n1];
                    b[k, n1] = b[k, n1] + pj * gravData.s[L, n1];
                    xp2[j] = xp[j];
                    ip2[j] = ip[j];
                }

                for (m = L - 2; m >= 0; m -= 1)
                {
                    gpodd(jmax, L, m, xp2, xp1, out xp0, ip2, ip1, out ip0);

                    for (j = 0; j <= jmax; j++)
                    {
                        k = 2 * j + 1;
                        pj = x2f(xp0[j], ip0[j]);
                        //     pja[k, m] = pj;
                        a[k, m] = a[k, m] + pj * gravData.c[L, m];
                        b[k, m] = b[k, m] + pj * gravData.s[L, m];
                        xp2[j] = xp1[j];
                        ip2[j] = ip1[j];
                        xp1[j] = xp0[j];
                        ip1[j] = ip0[j];
                    }
                }
            }
        }  // xfsh2f

        // from 2017 fukishima paper, ALFs
        //        j n = 2j Pn(1=2) d Pn(1=2)
        //1 2 􀀀2.7950849718747371205114670859140954E􀀀01 +4.46E􀀀16
        //2 4 􀀀8.6718750000000000000000000000000019E􀀀01 +1.28E􀀀16
        //3 8 􀀀3.0362102888840987874508856147660683E􀀀01 +4.89E􀀀16
        //4 16 􀀀8.6085221363787000197086857609730105E􀀀01 􀀀2.47E􀀀16
        //5 32 􀀀3.1119962497760147366174972709413071E􀀀01 􀀀1.14E􀀀15
        //6 64 􀀀8.5832418550243444685033693028444350E􀀀01 􀀀1.97E􀀀16
        //7 128 􀀀3.1316449107909472026965626279232704E􀀀01 􀀀2.13E􀀀15
        //8 256 􀀀8.5762286823362136598509949562358587E􀀀01 +4.73E􀀀16
        //9 512 􀀀3.1365884210353617421696699741828314E􀀀01 +1.94E􀀀15
        //10 1024 􀀀8.5744308441362078316451963835453876E􀀀01 􀀀1.12E􀀀15
        //11 2048 􀀀3.1378260213136415436393598399168296E􀀀01 +1.01E􀀀14


        // GMAT Pines approach
        //------------------------------------------------------------------------------
        public void CalculateField
            (
            double jday,
            double[] pos,
            double latgc,
            Int32 nn, Int32 mm,
            AstroLib.gravityConst gravData,
            out double[] acc
            //out double[,] gradient
            )
        {
            acc = new double[3];

            // Int32 XS = fillgradient ? 2 : 1;
            // calculate vector components ----------------------------------
            double magr = Math.Sqrt(pos[0] * pos[0] + pos[1] * pos[1] + pos[2] * pos[2]);    // Naming scheme from ref [3]
            double s = pos[0] / magr;
            double t = pos[1] / magr;
            double u = pos[2] / magr; // sin(phi), phi = geocentric latitude

            // Calculate values for A -----------------------------------------
            double[,] A = new double[50, 50];
            double[,] N1 = new double[50, 50];
            double[,] N2 = new double[50, 50];
            double[,] V = new double[50, 50];
            double[,] VR01 = new double[50, 50];
            double[,] VR11 = new double[50, 50];
            double[,] VR02 = new double[50, 50];
            double[,] VR12 = new double[50, 50];
            double[,] VR22 = new double[50, 50];
            double[] Re = new double[50];
            double[] Im = new double[50];
            double sqrt2 = Math.Sqrt(2.0);
            Int32 XS = 2;
            u = Math.Sin(latgc);

            // get leg poly normalization numbers (do once)
            // all 0
            for (Int32 m = 0; m <= mm + 2; ++m)
            {
                for (Int32 n = m + 2; n <= nn + 2; ++n)
                {
                    N1[n, m] = Math.Sqrt(((2.0 * n + 1) * (2 * n - 1)) / ((n - m) * (n + m)));  // double in denom??
                    N2[n, m] = Math.Sqrt(((2.0 * n + 1) * (n - m - 1) * (n + m - 1)) / ((2.0 * n - 3) * (n + m) * (n - m)));
                }
            }

            // NANs
            for (Int32 n = 0; n <= nn + 2; ++n)
            {
                V[n, 0] = Math.Sqrt((2.0 * (2 * n + 1)));   // Temporary, to make following loop work
                for (Int32 m = 1; m <= n + 2 && m <= mm + 2; ++m)
                {
                    V[n, m] = V[n, m - 1] / Math.Sqrt(((n + m) * (n - m + 1)));  // need real on n-m?
                }
                V[n, 0] = Math.Sqrt((2.0 * n + 1));       // Now set true value
            }

            for (Int32 n = 0; n <= nn; ++n)
                for (Int32 m = 0; m <= n && m <= mm; ++m)
                {
                    //double nn = n;
                    VR01[n, m] = Math.Sqrt(((nn - m) * (nn + m + 1)));  // need real on n-m?
                    VR11[n, m] = Math.Sqrt(((2.0 * nn + 1) * (nn + m + 2) * (nn + m + 1)) / ((2.0 * nn + 3)));
                    VR02[n, m] = Math.Sqrt(((nn - m) * (nn - m - 1) * (nn + m + 1) * (nn + m + 2)));  // need real on n-m?
                    VR12[n, m] = Math.Sqrt((2.0 * nn + 1) / (2.0 * nn + 3) * ((nn - m) * (nn + m + 1) * (nn + m + 2) * (nn + m + 3)));  // need real on n-m?
                    VR22[n, m] = Math.Sqrt((2.0 * nn + 1) / (2.0 * nn + 5) * ((nn + m + 1.0) * (nn + m + 2) * (nn + m + 3) * (nn + m + 4)));
                    if (m == 0)
                    {
                        VR01[n, m] /= sqrt2;
                        VR11[n, m] /= sqrt2;
                        VR02[n, m] /= sqrt2;
                        VR12[n, m] /= sqrt2;
                        VR22[n, m] /= sqrt2;
                    }
                }

            // generate legendre polynomials - the off-diagonal elements
            A[1, 0] = u * Math.Sqrt(3.0);
            for (Int32 n = 1; n <= nn + XS; ++n)
                A[n + 1, n] = u * Math.Sqrt(2.0 * n + 3) * A[n, n];

            // apply column-fill recursion formula (Table 2, Row I, Ref.[1])
            for (Int32 m = 0; m <= mm + XS; ++m)
            {
                for (Int32 n = m + 2; n <= nn + XS; ++n)
                    A[n, m] = u * N1[n, m] * A[n - 1, m] - N2[n, m] * A[n - 2, m];  // uses anm bnm from fukushima eq 6, 7
                // Ref.[3], Eq.(24)
                if (m == 0)
                    Re[m] = 1;
                else
                    Re[m] = s * Re[m - 1] - t * Im[m - 1]; // real part of (s + i*t)^m
                if (m == 0)
                    Im[m] = 0;
                else
                    Im[m] = s * Im[m - 1] + t * Re[m - 1]; // imaginary part of (s + i*t)^m
            }

            // Now do summation ------------------------------------------------
            // initialize recursion
            double FieldRadius = AstroLibr.gravConst.re;
            double rho = FieldRadius / magr;
            double Factor = AstroLibr.gravConst.mu;
            double rho_np1 = -Factor / magr * rho;   // rho(0) ,Ref[3], Eq 26 , factor = mu for gravity
            double rho_np2 = rho_np1 * rho;
            double a1 = 0;
            double a2 = 0;
            double a3 = 0;
            double a4 = 0;
            double a11 = 0;
            double a12 = 0;
            double a13 = 0;
            double a14 = 0;
            double a23 = 0;
            double a24 = 0;
            double a33 = 0;
            double a34 = 0;
            double a44 = 0;
            for (Int32 n = 1; n <= nn; ++n)
            {
                rho_np1 *= rho;
                rho_np2 *= rho;
                double sum1 = 0;
                double sum2 = 0;
                double sum3 = 0;
                double sum4 = 0;
                double sum11 = 0;
                double sum12 = 0;
                double sum13 = 0;
                double sum14 = 0;
                double sum23 = 0;
                double sum24 = 0;
                double sum33 = 0;
                double sum34 = 0;
                double sum44 = 0;

                for (Int32 m = 0; m <= n && m <= mm; ++m) // wcs - removed "m<=n"
                {
                    double Cval = gravData.c[n, m]; // Cnm(jday, n, m);
                    double Sval = gravData.s[n, m]; // Snm(jday, n, m);
                    // Pines Equation 27 (Part of)
                    double D = (Cval * Re[m] + Sval * Im[m]) * sqrt2;
                    double E, F;
                    if (m == 0)
                        E = 0;
                    else
                        E = (Cval * Re[m - 1] + Sval * Im[m - 1]) * sqrt2;
                    if (m == 0)
                        F = 0;
                    else
                        F = (Sval * Re[m - 1] - Cval * Im[m - 1]) * sqrt2;

                    // Correct for normalization
                    double Avv00 = A[n, m];
                    double Avv01 = VR01[n, m] * A[n, m + 1];
                    double Avv11 = VR11[n, m] * A[n + 1, m + 1];
                    // Pines Equation 30 and 30b (Part of)
                    sum1 += m * Avv00 * E;
                    sum2 += m * Avv00 * F;
                    sum3 += Avv01 * D;
                    sum4 += Avv11 * D;

                    // Truncate the gradient at GRADIENT_MAX x GRADIENT_MAX
                    //if (fillgradient)
                    //{
                    //    if ((m <= gradientlimit) && (n <= gradientlimit))
                    //    {
                    //        // Pines Equation 27 (Part of)
                    //        // 2015.09.18 GMT-5295 m<=2  -> m<=1
                    //        double G = m <= 1 ? 0 : (Cval * AstroLibr.gravConst.[m - 2] + Sval * Im[m - 2]) * sqrt2;
                    //        double H = m <= 1 ? 0 : (Sval * AstroLibr.gravConst.[m - 2] - Cval * Im[m - 2]) * sqrt2;
                    //        // Correct for normalization
                    //        double Avv02 = VR02[n][m] * A[n][m + 2];
                    //        double Avv12 = VR12[n][m] * A[n + 1][m + 2];
                    //        double Avv22 = VR22[n][m] * A[n + 2][m + 2];
                    //        if (GmatMathUtil::IsNaN(Avv02) || GmatMathUtil::IsInf(Avv02))
                    //            Avv02 = 0.0;  // ************** wcs added ****

                    //        // Pines Equation 36 (Part of)
                    //        sum11 += m * (m - 1) * Avv00 * G;
                    //        sum12 += m * (m - 1) * Avv00 * H;
                    //        sum13 += m * Avv01 * E;
                    //        sum14 += m * Avv11 * E;
                    //        sum23 += m * Avv01 * F;
                    //        sum24 += m * Avv11 * F;
                    //        sum33 += Avv02 * D;
                    //        sum34 += Avv12 * D;
                    //        sum44 += Avv22 * D;
                    //    }
                    //    else
                    //    {
                    //        if (matrixTruncationWasPosted == false)
                    //        {
                    //            MessageInterface::ShowMessage("*** WARNING *** Gradient data "

                    //                  "for the state transition matrix and A-matrix "

                    //                  "computations are truncated at degree and order "

                    //                  "<= %d.\n", gradientlimit);
                    //            matrixTruncationWasPosted = true;
                    //        }
                    //    }
                    //}
                }
                // Pines Equation 30 and 30b (Part of)
                double rr = rho_np1 / FieldRadius;
                a1 += rr * sum1;
                a2 += rr * sum2;
                a3 += rr * sum3;
                a4 -= rr * sum4;
                //if (fillgradient)
                //{
                //    // Pines Equation 36 (Part of)
                //    a11 += rho_np2 / FieldRadius / FieldRadius * sum11;
                //    a12 += rho_np2 / FieldRadius / FieldRadius * sum12;
                //    a13 += rho_np2 / FieldRadius / FieldRadius * sum13;
                //    a14 -= rho_np2 / FieldRadius / FieldRadius * sum14;
                //    a23 += rho_np2 / FieldRadius / FieldRadius * sum23;
                //    a24 -= rho_np2 / FieldRadius / FieldRadius * sum24;
                //    a33 += rho_np2 / FieldRadius / FieldRadius * sum33;
                //    a34 -= rho_np2 / FieldRadius / FieldRadius * sum34;
                //    a44 += rho_np2 / FieldRadius / FieldRadius * sum44;
                //}
            }

            // Pines Equation 31 
            acc[0] = a1 + a4 * s;
            acc[1] = a2 + a4 * t;
            acc[2] = a3 + a4 * u;
            //if (fillgradient)
            //{
            //    // Pines Equation 37
            //    gradient[0, 0] = a11 + s * s * a44 + a4 / magr + 2 * s * a14;
            //    gradient[1, 1] = -a11 + t * t * a44 + a4 / magr + 2 * t * a24;
            //    gradient[2, 2] = a33 + u * u * a44 + a4 / magr + 2 * u * a34;
            //    gradient[0, 1] = gradient[1, 0] = a12 + s * t * a44 + s * a24 + t * a14;
            //    gradient[0, 2] = gradient[2, 0] = a13 + s * u * a44 + s * a34 + u * a14;
            //    gradient[1, 2] = gradient[2, 1] = a23 + t * u * a44 + u * a24 + t * a34;
            //}
        }


        // -----------------------------------------------------------------------------------------------\
        // Gottlieb approach for acceleration
        // gotpot in his nomenclature
        //
        // -----------------------------------------------------------------------------------------------\

        public void FullGeopF
    (
        AstroLib.gravityConst gravData,
        double[] recef,
        double[,,] normArr,
        int order,
        out double[,] legPoly,
        out double[] G
    )
        {
            legPoly = new double[order + 2, order + 2];
            G = new double[3];

            //normArr = new double[order + 2, order + 2, 7];
            double[] zeta, eta, xi;
            zeta = new double[order + 1];
            eta = new double[order + 1];
            xi = new double[order + 1];
            double[] ctrigArr = new double[order + 1];
            double[] strigArr = new double[order + 1];

            double Ri, Xovr, Yovr, Zovr, sinlat, magr;
            double muor, muor2, Reor, Reorn;
            double Sumh, Sumgam, Sumj, Sumk, Lambda;
            double Sumh_N, Sumgam_N, Sumj_N, Sumk_N, Mxpnm;
            double BnmVal, pnm, snm, cnm, ctmm1, stmm1;
            Int32 mm1, mp1, nm1, nm2, npmp1, Lim, Sum_Init;
            double cn, sn, zn;

            magr = MathTimeLibr.mag(recef);
            Ri = 1.0 / magr;
            Xovr = recef[0] * Ri;
            Yovr = recef[1] * Ri;
            Zovr = recef[2] * Ri;
            sinlat = Zovr;
            double coslat = Math.Cos(Math.Asin(sinlat));
            Reor = AstroLibr.gravConst.re * Ri;
            Reorn = Reor;
            muor = AstroLibr.gravConst.mu * Ri;
            muor2 = muor * Ri;

            // include two-body or not
            //if (Want_Central_force == true)
            //    Sum_Init = 1;
            //else
            // note, 1 makes the two body pretty close, except for the 1st component
            Sum_Init = 0;

            // initial values
            // ctrigArr[0] = 1.0;    
            ctrigArr[1] = Xovr;
            //  strigArr[0] = 0.0;
            strigArr[1] = Yovr;
            Sumh = 0.0;
            Sumj = 0.0;
            Sumk = 0.0;
            Sumgam = Sum_Init;

            // normArr(L, m, 0) xi Gottlieb eta
            // normArr(L, m, 1) eta Gottlieb zeta
            // normArr(L, m, 2) alpha Gottlieb alpha
            // normArr(L, m, 3) beta Gottlieb beta
            // normArr(L, m, 5) delta Gottlieb zn
            legPoly[0, 0] = 1.0;
            legPoly[0, 1] = 0.0;
            legPoly[1, 0] = Math.Sqrt(3) * sinlat;
            legPoly[1, 1] = Math.Sqrt(3); // * coslat;

            for (int n = 2; n <= order; n++)
            {
                // get the power for each n
                Reorn = Reorn * Reor;
                //pn = legPoly[n, 0];
                cn = gravData.cNor[n, 0];
                //sn = gravData.sNor[n, 0];

                nm1 = n - 1;
                nm2 = n - 2;

                // eq 3-17, eq 7-14  alpha(n) beta(n)
                legPoly[n, 0] = sinlat * normArr[n, 0, 2] * legPoly[nm1, 0] - normArr[n, 0, 3] * legPoly[nm2, 0];
                // inner diagonal eq 7-16
                legPoly[n, nm1] = normArr[n - 1, nm2, 6] * sinlat * legPoly[n, n];  // n-1,n-2, 6, not 5, no nm1
                                                                                    //      legPoly[n, nm1] = normArr[n, nm1, 7] * sinlat * legPoly[n, n];
                                                                                    // diagonal eq 7-8
                legPoly[n, n] = normArr[n, n, 4] * coslat * legPoly[nm1, nm1];

                Sumh_N = normArr[1, 0, 6] * legPoly[n, 0] * cn;  // 0 by 2016 paper
                Sumgam_N = legPoly[n, 0] * cn * (n + 1);  // double

                if (order > 0)
                {
                    for (int m = 1; m <= nm2; m++)
                    {
                        // eq 3-18, eq 7-12   xin(m) eta(m)
                        legPoly[n, m] = normArr[n, m, 0] * sinlat * legPoly[nm1, m] - normArr[n, m, 1] * legPoly[nm2, m];
                    }
                    // got all the Legendre functions now

                    Sumj_N = 0.0;
                    Sumk_N = 0.0;
                    ctrigArr[n] = ctrigArr[1] * ctrigArr[nm1] - strigArr[1] * strigArr[nm1]; // mm1????
                    strigArr[n] = strigArr[1] * ctrigArr[nm1] + ctrigArr[1] * strigArr[nm1];

                    if (n < order)
                        Lim = n;
                    else
                        Lim = order;

                    for (int m = 1; m <= Lim; m++)
                    {
                        mm1 = m - 1;
                        mp1 = m + 1;
                        npmp1 = (n + mp1);  // double
                        pnm = legPoly[n, m];
                        cnm = gravData.cNor[n, m];
                        snm = gravData.sNor[n, m];
                        //ctmm1 = gravData.cNor[n, mm1];
                        //stmm1 = gravData.sNor[n, mm1];

                        Mxpnm = m * pnm;  // double
                        BnmVal = cnm * ctrigArr[m] + snm * strigArr[m];
                        Sumh_N = Sumh_N + legPoly[n, mp1] * BnmVal * normArr[n, m, 6];  // zn(m)
                        Sumgam_N = Sumgam_N + npmp1 * pnm * BnmVal;
                        Sumj_N = Sumj_N + Mxpnm * (cnm * ctrigArr[m] + snm * strigArr[m]);
                        Sumk_N = Sumk_N - Mxpnm * (cnm * strigArr[m] - snm * ctrigArr[m]);
                    }   // for through m

                    Sumj = Sumj + Reorn * Sumj_N;
                    Sumk = Sumk + Reorn * Sumk_N;
                }  // if order > 0

                // ---- SUMS BELOW HERE HAVE VALUES WHEN m = 0
                Sumh = Sumh + Reorn * Sumh_N;
                Sumgam = Sumgam + Reorn * Sumgam_N;
            }  // loop

            Lambda = Sumgam + sinlat * Sumh;
            G[0] = -muor2 * (Lambda * Xovr - Sumj);
            G[1] = -muor2 * (Lambda * Yovr - Sumk);
            G[2] = -muor2 * (Lambda * Zovr - Sumh);
        }  // gotpot;



        public void testproporbit()
        {
            double[] fArgs = new double[14];
            double[] reci = new double[3];
            double[] veci = new double[3];
            double[] aeci = new double[3];
            double[] aeci2 = new double[3];
            double[] recef = new double[3];
            double[] vecef = new double[3];
            double[] aecef = new double[3];
            double[] rsecef = new double[3];
            double[] vsecef = new double[3];
            double[] rseci = new double[3];
            double[] vseci = new double[3];
            double psia, wa, epsa, chia;
            double meaneps, deltapsi, deltaeps, trueeps;
            double[] omegaearth = new double[3];
            double[] rpef = new double[3];
            double[] vpef = new double[3];
            double[] apef = new double[3];
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
            double[,] temp1 = new double[3, 3];
            double[,] transeci2ecef = new double[3, 3];
            double[,] transecef2eci = new double[3, 3];
            double[,] convArr = new double[152, 152];

            double[] adrag = new double[3];
            double[] vrel = new double[3];

            double[] rsun = new double[3];
            double[] rsatsun = new double[3];
            double[] rmoon = new double[3];
            double[] rsat3 = new double[3];
            double[] rearth3 = new double[3];
            double[] a3body = new double[3];
            double[] athirdbody = new double[3];
            double[] athirdbody1 = new double[3];
            double[] athirdbody2 = new double[3];
            double[] aPertG = new double[3];
            double[] aPertM = new double[3];
            double[] aPertM1 = new double[3];

            double[] asrp = new double[3];

            double ttt, ttdb, xp, yp, lod, jdut1, ddpsi, ddeps, ddx, ddy, dut1, jdtt, jdftt;
            int year, mon, day, hr, minute, dat, eqeterms;
            double second, jdF, jdutc, jdFutc, jdtdb, jdFtdb, jdtdbjplstart, jdFtdbjplstart;

            double hellp, latgd, lon;
            double cd, cr, area, mass, q, tmptdb;

            double latgc;
            Int32 degree, order;
            double[,] LegArrMU;  // montenbruck
            double[,] LegArrMN;
            double[,] LegArrGU;  // gtds
            double[,] LegArrGN;
            double[,] LegArrOU;  // geodyn
            double[,] LegArrON;
            double[,] LegArrGott; // Gottlieb
            double[,] LegArrGotU; // Gottlieb
            double[,] LegArrGotN; // Gottlieb
            double[,] LegArrEx;  // exact 
            double[,] LegArrF;   // Fukushima 
            // 152 is arbitrary
            Int32 orderSize = 500;
            double[,,] normArr = new double[orderSize, orderSize, 7];

            LegArrF = new double[orderSize, orderSize];

            AstroLib.gravityConst gravData;

            double rad = 180.0 / Math.PI;              // deg to rad
            double conv = Math.PI / (180.0 * 3600.0);  // " to rad

            //StringBuilder strbuild = new StringBuilder();
            strbuild.Clear();
            StringBuilder strbuildplot = new StringBuilder();
            strbuildplot.Clear();

            // ------------------------------- initial state -------------------------------
            reci = new double[] { -605.79079600, -5870.23042200, 3493.05191600 };
            veci = new double[] { -1.568251000, -3.702348000, -6.479485000 };
            // print out initial conditions
            strbuild.AppendLine("reci  " + reci[0].ToString(fmt).PadLeft(4) + " " + reci[1].ToString(fmt).PadLeft(4) + " " + reci[2].ToString(fmt).PadLeft(4) + " " +
                                "v  " + veci[0].ToString(fmt).PadLeft(4) + " " + veci[1].ToString(fmt).PadLeft(4) + " " + veci[2].ToString(fmt).PadLeft(4));
            cd = 2.2;
            cr = 1.2;
            area = 40.0;     // m^2 
            mass = 1000.0;   // kg

            // ------------------------------- establish time parameters -------------------------------
            EOPSPWLib.iau80Class iau80arr;
            EOPSPWLib.iau00Class iau00arr;
            string nutLoc;
            nutLoc = @"D:\Codes\LIBRARY\DataLib\nut80.dat";
            EOPSPWLibr.iau80in(nutLoc, out iau80arr);
            nutLoc = @"D:\Codes\LIBRARY\DataLib\";
            EOPSPWLibr.iau00in(nutLoc, out iau00arr);
            // now read it in
            double jdxysstart, jdfxysstart;
            AstroLib.xysdataClass[] xysarr = AstroLibr.xysarr;
            AstroLibr.initXYS(ref xysarr, nutLoc, "xysdata.dat", out jdxysstart, out jdfxysstart);

            year = 2020;
            mon = 2;
            day = 18;
            hr = 15;
            minute = 8;
            second = 47.23847;
            MathTimeLibr.jday(year, mon, day, hr, minute, second, out jdutc, out jdFutc);  // utc

            // these vaues are not consistent wih the EOP files - are they interpolated already????
            // no. change to use actual at 0000 values, rerun probelm. 
            //2020 02 18 58897  0.030655  0.336009 -0.1990725  0.0000639 -0.108041 -0.007459  0.000315 -0.000055  37
            //2020 02 19 58898  0.030313  0.337617 - 0.1991016  0.0000235 - 0.107939 - 0.007476  0.000324 - 0.000036  37

            xp = 0.030655 * conv;
            yp = 0.336009 * conv;
            lod = 0.0000639;
            ddpsi = -0.108041 * conv;  // " to rad
            ddeps = -0.007459 * conv;
            ddx = 0.000315 * conv;     // " to rad
            ddy = 0.000055 * conv;
            dut1 = -0.1990725;   // sec
            dat = 37;            // sec
            AstroLib.EOpt opt = AstroLib.EOpt.e80;
            eqeterms = 2;
            jdtt = jdutc;
            jdftt = jdFutc + (dat + 32.184)/86400.0;

            // method to do calculations in
            char normalized = 'y';
            strbuild.AppendLine("normalized = " + normalized.ToString());

            strbuild.AppendLine(year.ToString("0000") + " " + mon.ToString("00") + " " + day.ToString("00") + " " + hr.ToString("00") + ":" +
                minute.ToString("00") + ":" + second.ToString());
            strbuild.AppendLine("dat " + dat.ToString() + " lod " + lod.ToString());
            strbuild.AppendLine("jdutc " + (jdutc + jdFutc).ToString());
            strbuild.AppendLine("xp yp " + (xp / conv).ToString() + " " + (yp / conv).ToString() + " arcsec");
            strbuild.AppendLine("dpsi deps " + (ddpsi / conv).ToString() + " " + (ddeps / conv).ToString() + " arcsec");
            strbuild.AppendLine("dx dy " + (ddx / conv).ToString() + " " + (ddy / conv).ToString() + " arcsec \n");

            jdut1 = jdutc + jdFutc + dut1 / 86400.0;
            strbuild.AppendLine("jdut1 " + jdut1.ToString());

            // watch if getting tdb that j2000 is also tdb
            ttt = (jdutc + jdFutc + (dat + 32.184) / 86400.0 - 2451545.0) / 36525.0;
            strbuild.AppendLine("jdttt " + (jdutc + jdFutc + (dat + 32.184) / 86400.0).ToString() + " ttt " + ttt.ToString() + "\n");

            AstroLibr.fundarg(ttt, opt, out fArgs);

            tmptdb = (dat + 32.184 + 0.001657 * Math.Sin(628.3076 * ttt + 6.2401)
                 + 0.000022 * Math.Sin(575.3385 * ttt + 4.2970)
                 + 0.000014 * Math.Sin(1256.6152 * ttt + 6.1969)
                 + 0.000005 * Math.Sin(606.9777 * ttt + 4.0212)
                 + 0.000005 * Math.Sin(52.9691 * ttt + 0.4444)
                 + 0.000002 * Math.Sin(21.3299 * ttt + 5.5431)
                 + 0.000010 * ttt * Math.Sin(628.3076 * ttt + 4.2490)) / 86400.0;  // USNO circ(14)
            MathTimeLibr.jday(year, mon, day, hr, minute, second + tmptdb, out jdtdb, out jdFtdb);
            ttdb = (jdtdb + jdFtdb - 2451545.0) / 36525.0;
            strbuild.AppendLine("jdttb " + (jdtdb + jdFtdb).ToString() + " ttdb " + ttdb.ToString());

            // get reduction matrices
            deltapsi = 0.0;
            meaneps = 0.0;

            omegaearth[0] = 0.0;
            omegaearth[1] = 0.0;
            omegaearth[2] = AstroLibr.gravConst.earthrot * (1.0 - lod / 86400.0);

            prec = AstroLibr.precess(ttt, opt, out psia, out wa, out epsa, out chia);
            nut = AstroLibr.nutation(ttt, ddpsi, ddeps, iau80arr, opt, fArgs, out deltapsi, out deltaeps, out trueeps, out meaneps);
            st = AstroLibr.sidereal(jdut1, deltapsi, meaneps, fArgs, lod, eqeterms, opt);
            pm = AstroLibr.polarm(xp, yp, ttt, opt);

            //// ---- perform transformations eci to ecef
            pmp = MathTimeLibr.mattrans(pm, 3);
            stp = MathTimeLibr.mattrans(st, 3);
            nutp = MathTimeLibr.mattrans(nut, 3);
            precp = MathTimeLibr.mattrans(prec, 3);
            temp = MathTimeLibr.matmult(pmp, stp, 3, 3, 3);
            temp1 = MathTimeLibr.matmult(temp, nutp, 3, 3, 3);
            transeci2ecef = MathTimeLibr.matmult(temp1, precp, 3, 3, 3);
            recef = MathTimeLibr.matvecmult(transeci2ecef, reci, 3);
            strbuild.AppendLine("recef  " + recef[0].ToString(fmt).PadLeft(4) + " " + recef[1].ToString(fmt).PadLeft(4) + " " + recef[2].ToString(fmt).PadLeft(4) + " " +
                                "v  " + vecef[0].ToString(fmt).PadLeft(4) + " " + vecef[1].ToString(fmt).PadLeft(4) + " " + vecef[2].ToString(fmt).PadLeft(4));

            //----perform transformations ecef to eci
            temp = MathTimeLibr.matmult(prec, nut, 3, 3, 3);
            temp1 = MathTimeLibr.matmult(temp, st, 3, 3, 3);
            transecef2eci = MathTimeLibr.matmult(temp1, pm, 3, 3, 3);
            reci = MathTimeLibr.matvecmult(transecef2eci, recef, 3);
            strbuild.AppendLine("reci  " + reci[0].ToString(fmt).PadLeft(4) + " " + reci[1].ToString(fmt).PadLeft(4) + " " + reci[2].ToString(fmt).PadLeft(4) + " " +
                                "v  " + veci[0].ToString(fmt).PadLeft(4) + " " + veci[1].ToString(fmt).PadLeft(4) + " " + veci[2].ToString(fmt).PadLeft(4));

            AstroLibr.eci_ecef(ref reci, ref veci, MathTimeLib.Edirection.eto, ref recef, ref vecef,
                 AstroLib.EOpt.e80, iau80arr, iau00arr,
                 jdtt, jdftt, jdut1, jdxysstart, lod, xp, yp, ddpsi, ddeps, ddx, ddy);

            // print out initial conditions
            strbuild.AppendLine("reci  " + reci[0].ToString(fmt).PadLeft(4) + " " + reci[1].ToString(fmt).PadLeft(4) + " " + reci[2].ToString(fmt).PadLeft(4) + " " +
                                "v  " + veci[0].ToString(fmt).PadLeft(4) + " " + veci[1].ToString(fmt).PadLeft(4) + " " + veci[2].ToString(fmt).PadLeft(4));
            strbuild.AppendLine("recef  " + recef[0].ToString(fmt).PadLeft(4) + " " + recef[1].ToString(fmt).PadLeft(4) + " " + recef[2].ToString(fmt).PadLeft(4) + " " +
                                "v  " + vecef[0].ToString(fmt).PadLeft(4) + " " + vecef[1].ToString(fmt).PadLeft(4) + " " + vecef[2].ToString(fmt).PadLeft(4));


            this.opsStatus.Text = "Status: Reading gravity field EGM-08 test";
            Refresh();
            // get past text in each file
            //if (fname.Contains("GEM"))    // GEM10bunnorm36.grv, GEMT3norm50.grv
            //    startKtr = 17;
            //if (fname.Contains("EGM-96")) // EGM-96norm70.grv
            //    startKtr = 73;
            //if (fname.Contains("EGM-08")) // EGM-08norm100.grv
            //    startKtr = 83;  // or 21 for the larger file... which has gfc in the first col too
            //string fname = "D:/Dataorig/Gravity/EGM-08norm100.grv";  // 83
            string fname = "D:/Dataorig/Gravity/EGM2008_to2190_TideFree.txt";  // 0 large one
            char normal = 'y';  // file has normalized coefficients

            AstroLibr.initGravityField(fname, 0, normal, out order, out gravData, out convArr, out normArr);
            strbuild.AppendLine("\nread in gravity field " + fname + " " + order.ToString() + " --------------- ");
            strbuild.AppendLine("\ncoefficents --------------- ");
            strbuild.AppendLine("c  2  0  " + gravData.c[2, 0].ToString() + " s " + gravData.s[2, 0].ToString());
            strbuild.AppendLine("c  4  0  " + gravData.c[4, 0].ToString() + " s " + gravData.s[4, 0].ToString());
            strbuild.AppendLine("c  4  4  " + gravData.c[4, 4].ToString() + " s " + gravData.s[4, 4].ToString());
            strbuild.AppendLine("c 21  1 " + gravData.c[21, 1].ToString() + " s " + gravData.s[21, 1].ToString());
            strbuild.AppendLine("\nnormalized coefficents --------------- ");
            strbuild.AppendLine("c  2  0  " + gravData.cNor[2, 0].ToString() + " s " + gravData.sNor[2, 0].ToString());
            strbuild.AppendLine("c  4  0  " + gravData.cNor[4, 0].ToString() + " s " + gravData.sNor[4, 0].ToString());
            strbuild.AppendLine("c  4  4  " + gravData.cNor[4, 4].ToString() + " s " + gravData.sNor[4, 4].ToString());
            strbuild.AppendLine("c 21  1 " + gravData.cNor[21, 1].ToString() + " s " + gravData.sNor[21, 1].ToString());

            AstroLibr.ecef2ll(recef, out latgc, out latgd, out lon, out hellp);
            degree = 500;
            order = 500;
            // GTDS version
            AstroLibr.LegPolyG(latgc, order, normalized, convArr, normArr, out LegArrGU, out LegArrGN);
            // Gottlieb version
            AstroLibr.LegPolyGot(latgc, order, normalized, convArr, normArr, out LegArrGotU, out LegArrGotN);
            // Montenbruck version
            AstroLibr.LegPolyM(latgc, order, normalized, convArr, out LegArrMU, out LegArrMN);
            // Geodyn version
            AstroLibr.geodynlegp(latgc, degree, order, out LegArrOU, out LegArrON);
            // Exact values
            LegPolyEx(latgc, order, out LegArrEx);

            // fukushima approach do as 1-d arrays for now
            //double[] pmm = new double[8];
            //double[] psm = new double[8];
            //Int32[] ipsm = new Int32[8];
            // get the values in X-numbers
            //alfsx(Math.Cos(latgc), 6, normArr, out psm, out ipsm);
            //alfmx(Math.Sin(latgc), 3, 6, normArr, psm[3], ipsm[3], out pmm);
            AstroLibr.LegPolyF(latgc, order, 'y', normArr, out LegArrF);


            string errstr = " ";
            double dr1, dr2, dr3, sumdr1, sumdr2, sumdr3;
            sumdr1 = 0.0;
            sumdr2 = 0.0;
            sumdr3 = 0.0;
            strbuild.AppendLine("\nnormalized Legendre polynomials --------------- ");

            for (int L = 0; L <= 130; L++)  // order xxxxxxxxxxxxxxxxxx
            {
                string tempstr1 = "MN  ";  // montenbruck
                string tempstr2 = "GN  ";  // gtds
                string tempstr3 = "MU  ";
                string tempstr3a = "LU  ";  // exact
                string tempstr4 = "OU  ";  // geodyn\
                string tempstr5 = "GtN ";  // gottlieb\
                string tempstr6 = "GtU ";  // gottlieb
                string tempstr7 = "FN  ";  // Fukushima, test ones
                int stopL = L;
                for (int m = 0; m <= stopL; m++)
                {
                    tempstr1 = tempstr1 + " " + L.ToString() + "  " + m.ToString() + "   " + LegArrMN[L, m].ToString();
                    tempstr2 = tempstr2 + " " + L.ToString() + "  " + m.ToString() + "   " + LegArrGN[L, m].ToString();
                    tempstr5 = tempstr5 + " " + L.ToString() + "  " + m.ToString() + "   " + LegArrGotN[L, m].ToString();
                    tempstr7 = tempstr7 + " " + L.ToString() + "  " + m.ToString() + "   " + LegArrF[L, m].ToString();
                    tempstr3 = tempstr3 + " " + L.ToString() + "  " + m.ToString() + "   " + LegArrMU[L, m].ToString();
                    tempstr3a = tempstr3a + " " + L.ToString() + "  " + m.ToString() + "   " + LegArrEx[L, m].ToString();
                    tempstr6 = tempstr6 + " " + L.ToString() + "  " + m.ToString() + "   " + LegArrGotU[L, m].ToString();
                    tempstr4 = tempstr4 + " " + L.ToString() + "  " + m.ToString() + "   " + LegArrOU[L + 1, m + 1].ToString();
                    // check error values
                    dr1 = 100.0 * (LegArrF[L, m] - LegArrGotN[L, m]) / LegArrF[L, m];
                    dr2 = 100.0 * (LegArrF[L, m] - LegArrGN[L, m]) / LegArrF[L, m];
                    dr3 = 100.0 * (LegArrF[L, m] - LegArrMN[L, m]) / LegArrF[L, m];
                    sumdr1 = sumdr1 + dr1;
                    sumdr2 = sumdr2 + dr2;
                    sumdr3 = sumdr3 + dr3;
                    errstr = errstr + "\n" + L.ToString() + "  " + m.ToString() + "   " + dr1.ToString()
                        + " " + dr2.ToString() + " " + dr3.ToString();
                }
                strbuild.AppendLine(tempstr2);
                strbuild.AppendLine(tempstr1);
                strbuild.AppendLine(tempstr5);
                strbuild.AppendLine(tempstr7 + "\n");
                //strbuild.AppendLine(tempstr3);
                //strbuild.AppendLine(tempstr3a);
                //strbuild.AppendLine(tempstr6);
                //strbuild.AppendLine(tempstr4 + "\n");
            }
            strbuildplot.AppendLine(errstr);

            strbuild.AppendLine("\naccelerations --------------- ");
            string straccum = "";

            order = 4;
            // GTDS acceleration
            AstroLibr.FullGeopG(recef, order, normalized, convArr, normArr, gravData, out aPertG, 'y', out straccum);
            // Montenbruck acceleration
            AstroLibr.FullGeopM(recef, order, normalized, convArr, gravData, out aPertM, 'y', out straccum);
            // Montenbruck code acceleration
            AstroLibr.FullGeopMC(recef, order, normalized, convArr, gravData, out aPertM1, 'y', out straccum);
            // Gottlieb acceleration
            double[] G = new double[3];
            double[] aPertGt = new double[3];
            FullGeopF(gravData, recef, normArr, order, out LegArrGott, out G);

            // Fukushima acceleration
            //LegPolyFF(recef, latgc, 4, 'y', normArr, gravData, out LegArrF);



            double[,] a = new double[360, 360];
            double[,] b = new double[360, 360];

            xfsh2f(80, gravData, out a, out b);
            strbuild.AppendLine(a[2, 0].ToString());

            strbuild.AppendLine("a  2  0  " + a[2, 0].ToString() + " b " + b[2, 0].ToString());
            strbuild.AppendLine("a  2  1  " + a[2, 1].ToString() + " b " + b[2, 1].ToString());
            strbuild.AppendLine("a  4  0  " + a[4, 0].ToString() + " b " + b[4, 0].ToString());
            strbuild.AppendLine("a  4  1  " + a[4, 0].ToString() + " b " + b[4, 1].ToString());
            strbuild.AppendLine("a  4  4  " + a[4, 4].ToString() + " b " + b[4, 4].ToString());
            strbuild.AppendLine("a 21  1 " + a[21, 1].ToString() + " b " + b[21, 1].ToString());

            CalculateField(jdutc, recef, latgc, order, order, gravData, out aeci);

            strbuild.AppendLine(straccum);
            strbuild.AppendLine(" body fixed frame ");
            strbuild.AppendLine("apertG bf 4 4   " + aPertG[0].ToString() + "     " + aPertG[1].ToString() + "     " + aPertG[2].ToString());
            strbuild.AppendLine("apertM bf 4 4   " + aPertM[0].ToString() + "     " + aPertM[1].ToString() + "     " + aPertM[2].ToString());

// bfc for gravity in example problem
            double[] temm = new double[3];
            temm[0] = aPertM[0];
            temm[1] = aPertM[1];
            temm[2] = aPertM[2];

            strbuild.AppendLine("apertMC bf 4 4  " + aPertM1[0].ToString() + "     " + aPertM1[1].ToString() + "     " + aPertM1[2].ToString());
            strbuild.AppendLine("apertGt bf 4 4  " + G[0].ToString() + "     " + G[1].ToString() + "     " + G[2].ToString());
            aPertG = MathTimeLibr.matvecmult(transecef2eci, aPertG, 3);
            aPertM = MathTimeLibr.matvecmult(transecef2eci, aPertM, 3);
            aPertM1 = MathTimeLibr.matvecmult(transecef2eci, aPertM1, 3);
            aPertGt = MathTimeLibr.matvecmult(transecef2eci, G, 3);
            strbuild.AppendLine("apertG ec 4 4   " + aPertG[0].ToString() + "     " + aPertG[1].ToString() + "     " + aPertG[2].ToString());
            strbuild.AppendLine("apertM ec 4 4   " + aPertM[0].ToString() + "     " + aPertM[1].ToString() + "     " + aPertM[2].ToString());
            strbuild.AppendLine("apertMC ec 4 4  " + aPertM1[0].ToString() + "     " + aPertM1[1].ToString() + "     " + aPertM1[2].ToString());
            strbuild.AppendLine("apertGt ec 4 4  " + aPertGt[0].ToString() + "     " + aPertGt[1].ToString() + "     " + aPertGt[2].ToString());
            strbuild.AppendLine("ans 4x4         -0.0000003723020	-0.0000031362090   	-0.0000102647170\n");  // no 2-body

            // add in two body term since full geop is only disturbing part
            AstroLibr.eci_ecef(ref reci, ref veci, MathTimeLib.Edirection.efrom, ref recef, ref vecef,
                 AstroLib.EOpt.e80, iau80arr, iau00arr,
                 jdtt, jdftt, jdut1, jdxysstart, lod, xp, yp, ddpsi, ddeps, ddx, ddy);

            aeci2[0] = -AstroLibr.gravConst.mu * reci[0] / (Math.Pow(MathTimeLibr.mag(reci), 3));
            aeci2[1] = -AstroLibr.gravConst.mu * reci[1] / (Math.Pow(MathTimeLibr.mag(reci), 3));
            aeci2[2] = -AstroLibr.gravConst.mu * reci[2] / (Math.Pow(MathTimeLibr.mag(reci), 3));
            strbuild.AppendLine("a2body      " + aeci2[0].ToString() + "     " + aeci2[1].ToString() + "     " + aeci2[2].ToString());

            aPertG[0] = aPertG[0] + aeci2[0];
            aPertG[1] = aPertG[1] + aeci2[1];
            aPertG[2] = aPertG[2] + aeci2[2];

            aPertM[0] = aPertM[0] + aeci2[0];
            aPertM[1] = aPertM[1] + aeci2[1];
            aPertM[2] = aPertM[2] + aeci2[2];

            aPertM1[0] = aPertM1[0] + aeci2[0];
            aPertM1[1] = aPertM1[1] + aeci2[1];
            aPertM1[2] = aPertM1[2] + aeci2[2];

            strbuild.AppendLine(" now with two body included");
            strbuild.AppendLine("apertG 4 4   " + aPertG[0].ToString() + "     " + aPertG[1].ToString() + "     " + aPertG[2].ToString());
            strbuild.AppendLine("apertM 4 4   " + aPertM[0].ToString() + "     " + aPertM[1].ToString() + "     " + aPertM[2].ToString());
            strbuild.AppendLine("apertMC 4 4  " + aPertM1[0].ToString() + "     " + aPertM1[1].ToString() + "     " + aPertM1[2].ToString());
            strbuild.AppendLine("ans 4x4 w2   0.0007483593980          0.0072522125910         -0.0043275195170\n");  // no 2-body
            //                    4x4 j2000   0.00074835849281         0.00725221243453        -0.00432751993509
            //                    4x4 icrf    0.00074835939828         0.00725221259059        -0.00432751951698
            //                    all j2000   0.00074845403274         0.00725223127396        -0.00432750265312
            //                    all icrf    0.00074845493821         0.00725223143002        -0.00432750223499


            order = 5;
            // normalized calcs, show
            AstroLibr.FullGeopG(recef, order, normalized, convArr, normArr, gravData, out aPertG, 'y', out straccum);

            AstroLibr.FullGeopM(recef, order, normalized, convArr, gravData, out aPertM, 'y', out straccum);

            AstroLibr.FullGeopMC(recef, order, normalized, convArr, gravData, out aPertM1, 'y', out straccum);

            strbuild.AppendLine(straccum);
            strbuild.AppendLine(" body fixed frame ");
            strbuild.AppendLine("apertG bf 5 5   " + aPertG[0].ToString() + "     " + aPertG[1].ToString() + "     " + aPertG[2].ToString());
            strbuild.AppendLine("apertM bf 5 5   " + aPertM[0].ToString() + "     " + aPertM[1].ToString() + "     " + aPertM[2].ToString());
            strbuild.AppendLine("apertMC bf 5 5  " + aPertM1[0].ToString() + "     " + aPertM1[1].ToString() + "     " + aPertM1[2].ToString());
            aPertG = MathTimeLibr.matvecmult(transecef2eci, aPertG, 3);
            aPertM = MathTimeLibr.matvecmult(transecef2eci, aPertM, 3);
            aPertM1 = MathTimeLibr.matvecmult(transecef2eci, aPertM1, 3);
            strbuild.AppendLine("apertG ec 5 5   " + aPertG[0].ToString() + "     " + aPertG[1].ToString() + "     " + aPertG[2].ToString());
            strbuild.AppendLine("apertM ec 5 5   " + aPertM[0].ToString() + "     " + aPertM[1].ToString() + "     " + aPertM[2].ToString());
            strbuild.AppendLine("apertMC ec 5 5  " + aPertM1[0].ToString() + "     " + aPertM1[1].ToString() + "     " + aPertM1[2].ToString());
            strbuild.AppendLine("ans 5x5      -0.0000003688720	    -0.0000031905410    	-0.0000102894900\n");

            // add in two body term since full geop is only disturbing part
            AstroLibr.eci_ecef(ref reci, ref veci, MathTimeLib.Edirection.efrom, ref recef, ref vecef,
                 AstroLib.EOpt.e80, iau80arr, iau00arr,
                 jdtt, jdftt, jdut1, jdxysstart, lod, xp, yp, ddpsi, ddeps, ddx, ddy);

            aeci2[0] = -AstroLibr.gravConst.mu * reci[0] / (Math.Pow(MathTimeLibr.mag(reci), 3));
            aeci2[1] = -AstroLibr.gravConst.mu * reci[1] / (Math.Pow(MathTimeLibr.mag(reci), 3));
            aeci2[2] = -AstroLibr.gravConst.mu * reci[2] / (Math.Pow(MathTimeLibr.mag(reci), 3));
            strbuild.AppendLine("a2body      " + aeci2[0].ToString() + "     " + aeci2[1].ToString() + "     " + aeci2[2].ToString());

            aPertG[0] = aPertG[0] + aeci2[0];
            aPertG[1] = aPertG[1] + aeci2[1];
            aPertG[2] = aPertG[2] + aeci2[2];

            aPertM[0] = aPertM[0] + aeci2[0];
            aPertM[1] = aPertM[1] + aeci2[1];
            aPertM[2] = aPertM[2] + aeci2[2];

            aPertM1[0] = aPertM1[0] + aeci2[0];
            aPertM1[1] = aPertM1[1] + aeci2[1];
            aPertM1[2] = aPertM1[2] + aeci2[2];

            strbuild.AppendLine(" now with two body included");
            strbuild.AppendLine("apertG 5 5   " + aPertG[0].ToString() + "     " + aPertG[1].ToString() + "     " + aPertG[2].ToString());
            strbuild.AppendLine("apertM 5 5   " + aPertM[0].ToString() + "     " + aPertM[1].ToString() + "     " + aPertM[2].ToString());
            strbuild.AppendLine("apertMC 5 5  " + aPertM1[0].ToString() + "     " + aPertM1[1].ToString() + "     " + aPertM1[2].ToString());


            strbuild.AppendLine("------------------ find drag acceleration");
            double density = 1.5e-12;  // kg / m3
            double magv = MathTimeLibr.mag(vecef);
            vrel[0] = vecef[0]; // vecef normal is veci to tod, then - wxr
            vrel[1] = vecef[1];
            vrel[2] = vecef[2];
            strbuild.AppendLine(" vrel " + vrel[0].ToString() + " " + vrel[1].ToString() + " " + vrel[2].ToString());
            //                 kg / m3        m2  /  kg     km / s  km / s
            adrag[0] = -0.5 * density * cd * area / mass * magv * vrel[0] * 1000.0;  // simplify vel, get units to km/s2
            adrag[1] = -0.5 * density * cd * area / mass * magv * vrel[1] * 1000.0;  // simplify vel, get units to km/s2
            adrag[2] = -0.5 * density * cd * area / mass * magv * vrel[2] * 1000.0;  // simplify vel, get units to km/s2

            strbuild.AppendLine(" adrag efc" + adrag[0].ToString() + " " + adrag[1].ToString() + " " + adrag[2].ToString());

            double[] temmm = new double[3];

            strbuild.AppendLine(" agrav + drag efc" +(temm[0]+ adrag[0]).ToString() + " " +
                (temm[1] + adrag[1]).ToString() + " " + (temm[2] + adrag[2]).ToString());


            transecef2eci = MathTimeLibr.matmult(temp1, pm, 3, 3, 3);
            aeci = MathTimeLibr.matvecmult(transecef2eci, adrag, 3);
            strbuild.AppendLine(" adrag eci " + aeci[0].ToString() + " " + aeci[1].ToString() + " " + aeci[2].ToString());
            strbuild.AppendLine("ans drag JR spline      0.0000000001040	0.0000000002090	0.0000000003550\n");
            strbuild.AppendLine("ans drag JR daily       0.0000000000840	0.0000000001720	0.0000000002900\n");
            strbuild.AppendLine("ans drag MSIS daily     0.0000000000730	0.0000000001510	0.0000000002530\n");

            temmm[0] = temm[0] + adrag[0];
            temmm[1] = temm[1] + adrag[1];
            temmm[2] = temm[2] + adrag[2];
            aeci = MathTimeLibr.matvecmult(transecef2eci, temmm, 3);
            strbuild.AppendLine(" agrav+drag eci " + aeci[0].ToString() + " " + aeci[1].ToString() + " " + aeci[2].ToString());

            strbuild.AppendLine(" ------------------ find third body acceleration");
            AstroLib.jpldedataClass[] jpldearr = AstroLibr.jpldearr;
            double musun, mumoon, rsmag, rmmag;
            musun = 1.32712428e11;    // km3 / s2
            mumoon = 4902.799;        // km3 / s2
            AstroLibr.initjplde(ref jpldearr, @"D:\Codes\LIBRARY\DataLib\", "sunmooneph_430t.txt", out jdtdbjplstart, out jdFtdbjplstart);

            // sun
            AstroLibr.findjpldeparam(jdtdb, jdFtdb, 's', jpldearr, jdtdbjplstart, out rsun, out rsmag, out rmoon, out rmmag);
            // stk value (chk that tdb is argument)
            double[] rsuns = new double[] { 126916355.384390, -69567131.339884, -30163629.424510 };
            // JPL ans  2020  2 18  M          0.6306    
            double[] rmoonj = new double[] { 14462.2967, -357096.9762, -151599.3021 };
            //JPL ans  2020  2 18 15:08:47.23847 S       0.6306  
            double[] rsunj = new double[] { 126921698.4134, -69564121.8695, -30156263.9220 };

            MathTimeLibr.addvec(1.0, rsuns, -1.0, rsun, out tempvec1);
            strbuild.AppendLine(" diff rsun stk-mine " + tempvec1[0].ToString() + " " + tempvec1[1].ToString() + " " +
                tempvec1[2].ToString() + " " + MathTimeLibr.mag(tempvec1).ToString());
            MathTimeLibr.addvec(1.0, rsunj, -1.0, rsun, out tempvec1);
            strbuild.AppendLine(" diff rsun jpl-mine " + tempvec1[0].ToString() + " " + tempvec1[1].ToString() + " " +
                tempvec1[2].ToString() + " " + MathTimeLibr.mag(tempvec1).ToString());
            MathTimeLibr.addvec(1.0, rsuns, -1.0, rsunj, out tempvec1);
            strbuild.AppendLine(" diff rsun stk-jpl  " + tempvec1[0].ToString() + " " + tempvec1[1].ToString() + " " +
                tempvec1[2].ToString() + " " + MathTimeLibr.mag(tempvec1).ToString());
            MathTimeLibr.addvec(1.0, rmoonj, -1.0, rmoon, out tempvec1);
            strbuild.AppendLine(" diff rmoon jpl-mine " + tempvec1[0].ToString() + " " + tempvec1[1].ToString() + " " +
                tempvec1[2].ToString() + " " + MathTimeLibr.mag(tempvec1).ToString());
            strbuild.AppendLine(" rsun  " + rsun[0].ToString() + " " + rsun[1].ToString() + " " + rsun[2].ToString());
            strbuild.AppendLine(" rmoon " + rmoon[0].ToString() + " " + rmoon[1].ToString() + " " + rmoon[2].ToString());

            double mu3 = musun;
            rsat3[0] = rsun[0] - reci[0];
            rsat3[1] = rsun[1] - reci[1];
            rsat3[2] = rsun[2] - reci[2];
            double magrsat3 = MathTimeLibr.mag(rsat3);
            rearth3[0] = rsun[0];
            rearth3[1] = rsun[1];
            rearth3[2] = rsun[2];
            double magrearth3 = MathTimeLibr.mag(rearth3);
            athirdbody[0] = mu3 * (rsat3[0] / Math.Pow(magrsat3, 3) - rearth3[0] / Math.Pow(magrearth3, 3));
            athirdbody[1] = mu3 * (rsat3[1] / Math.Pow(magrsat3, 3) - rearth3[1] / Math.Pow(magrearth3, 3));
            athirdbody[2] = mu3 * (rsat3[2] / Math.Pow(magrsat3, 3) - rearth3[2] / Math.Pow(magrearth3, 3));
            strbuild.AppendLine(" a3bodyS  eci " + athirdbody[0].ToString() + " " + athirdbody[1].ToString() + " " + athirdbody[2].ToString());
            athirdbody2[0] = -mu3 / Math.Pow(magrearth3, 3) * (rearth3[0] - 3.0 * rearth3[0] * (MathTimeLibr.dot(reci, rearth3) / Math.Pow(magrearth3, 2))
                - 7.5 * rearth3[0] * Math.Pow(((MathTimeLibr.dot(reci, rearth3) / Math.Pow(magrearth3, 2))), 2));
            athirdbody2[1] = -mu3 / Math.Pow(magrearth3, 3) * (rearth3[1] - 3.0 * rearth3[1] * (MathTimeLibr.dot(reci, rearth3) / Math.Pow(magrearth3, 2))
                - 7.5 * rearth3[1] * Math.Pow(((MathTimeLibr.dot(reci, rearth3) / Math.Pow(magrearth3, 2))), 2));
            athirdbody2[2] = -mu3 / Math.Pow(magrearth3, 3) * (rearth3[2] - 3.0 * rearth3[2] * (MathTimeLibr.dot(reci, rearth3) / Math.Pow(magrearth3, 2))
                - 7.5 * rearth3[2] * Math.Pow(((MathTimeLibr.dot(reci, rearth3) / Math.Pow(magrearth3, 2))), 2));
            strbuild.AppendLine(" a3bodyS2 eci" + athirdbody2[0].ToString() + " " + athirdbody2[1].ToString() + " " + athirdbody2[2].ToString());
            q = (Math.Pow(MathTimeLibr.mag(reci), 2) + 2.0 * MathTimeLibr.dot(reci, rsat3)) *
                (Math.Pow(magrearth3, 2) + magrearth3 * magrsat3 + Math.Pow(magrsat3, 2)) /
                (Math.Pow(magrearth3, 3) * Math.Pow(magrsat3, 3) * (magrearth3 + magrsat3));
            athirdbody1[0] = mu3 * (rsat3[0] * q - reci[0] / Math.Pow(magrearth3, 3));
            athirdbody1[1] = mu3 * (rsat3[1] * q - reci[1] / Math.Pow(magrearth3, 3));
            athirdbody1[2] = mu3 * (rsat3[2] * q - reci[2] / Math.Pow(magrearth3, 3));
            strbuild.AppendLine(" a3bodyS1 eci" + athirdbody1[0].ToString() + " " + athirdbody1[1].ToString() + " " + athirdbody1[2].ToString());
            strbuild.AppendLine("ans sun        0.0000000001820	0.0000000001620	-0.0000000001800\n");
            a3body[0] = athirdbody1[0];
            a3body[1] = athirdbody1[1];
            a3body[2] = athirdbody1[2];

            // moon
            mu3 = mumoon;
            rsat3[0] = rmoon[0] - reci[0];
            rsat3[1] = rmoon[1] - reci[1];
            rsat3[2] = rmoon[2] - reci[2];
            magrsat3 = MathTimeLibr.mag(rsat3);
            rearth3[0] = rmoon[0];
            rearth3[1] = rmoon[1];
            rearth3[2] = rmoon[2];
            magrearth3 = MathTimeLibr.mag(rearth3);
            athirdbody[0] = mu3 * (rsat3[0] / Math.Pow(magrsat3, 3) - rearth3[0] / Math.Pow(magrearth3, 3));
            athirdbody[1] = mu3 * (rsat3[1] / Math.Pow(magrsat3, 3) - rearth3[1] / Math.Pow(magrearth3, 3));
            athirdbody[2] = mu3 * (rsat3[2] / Math.Pow(magrsat3, 3) - rearth3[2] / Math.Pow(magrearth3, 3));
            strbuild.AppendLine(" a3bodyM  eci " + athirdbody[0].ToString() + " " + athirdbody[1].ToString() + " " + athirdbody[2].ToString());
            athirdbody2[0] = -mu3 / Math.Pow(magrearth3, 3) * (rearth3[0] - 3.0 * rearth3[0] * (MathTimeLibr.dot(reci, rearth3) / Math.Pow(magrearth3, 2))
                - 7.5 * rearth3[0] * Math.Pow(((MathTimeLibr.dot(reci, rearth3) / Math.Pow(magrearth3, 2))), 2));
            athirdbody2[1] = -mu3 / Math.Pow(magrearth3, 3) * (rearth3[1] - 3.0 * rearth3[1] * (MathTimeLibr.dot(reci, rearth3) / Math.Pow(magrearth3, 2))
                - 7.5 * rearth3[1] * Math.Pow(((MathTimeLibr.dot(reci, rearth3) / Math.Pow(magrearth3, 2))), 2));
            athirdbody2[2] = -mu3 / Math.Pow(magrearth3, 3) * (rearth3[2] - 3.0 * rearth3[2] * (MathTimeLibr.dot(reci, rearth3) / Math.Pow(magrearth3, 2))
                - 7.5 * rearth3[2] * Math.Pow(((MathTimeLibr.dot(reci, rearth3) / Math.Pow(magrearth3, 2))), 2));
            strbuild.AppendLine(" a3bodyM2 eci" + athirdbody2[0].ToString() + " " + athirdbody2[1].ToString() + " " + athirdbody2[2].ToString());
            q = (Math.Pow(MathTimeLibr.mag(reci), 2) + 2.0 * MathTimeLibr.dot(reci, rsat3)) *
                (Math.Pow(magrearth3, 2) + magrearth3 * magrsat3 + Math.Pow(magrsat3, 2)) /
                (Math.Pow(magrearth3, 3) * Math.Pow(magrsat3, 3) * (magrearth3 + magrsat3));
            athirdbody1[0] = mu3 * (rsat3[0] * q - reci[0] / Math.Pow(magrearth3, 3));
            athirdbody1[1] = mu3 * (rsat3[1] * q - reci[1] / Math.Pow(magrearth3, 3));
            athirdbody1[2] = mu3 * (rsat3[2] * q - reci[2] / Math.Pow(magrearth3, 3));
            strbuild.AppendLine(" a3bodyM1 eci" + athirdbody1[0].ToString() + " " + athirdbody1[1].ToString() + " " + athirdbody1[2].ToString());
            strbuild.AppendLine("ans moon        0.0000000000860	-0.0000000004210	-0.0000000006980\n");
            a3body[0] = a3body[0] + athirdbody1[0];
            a3body[1] = a3body[1] + athirdbody1[1];
            a3body[2] = a3body[2] + athirdbody1[2];
            strbuild.AppendLine("ans sun/moon    0.0000000002730	-0.0000000002680	-0.0000000008800\n");


            strbuild.AppendLine(" ------------------ find srp acceleration\n");
            double psrp = 4.56e-6;  // N/m2 = kgm/s2 / m2 = kg/ms2
            rsatsun[0] = rsun[0] - reci[0];
            rsatsun[1] = rsun[1] - reci[1];
            rsatsun[2] = rsun[2] - reci[2];
            double magrsatsun = MathTimeLibr.mag(rsatsun);
            //           kg/ms2      m2      kg      km            km
            asrp[0] = -(psrp * cr * area / mass * rsatsun[0] / magrsatsun) / 1000.0;  // result in km/s
            asrp[1] = -(psrp * cr * area / mass * rsatsun[1] / magrsatsun) / 1000.0;
            asrp[2] = -(psrp * cr * area / mass * rsatsun[2] / magrsatsun) / 1000.0;
            strbuild.AppendLine(" asrp eci " + asrp[0].ToString() + " " + asrp[1].ToString() + " " + asrp[2].ToString());
            strbuild.AppendLine("ans srp        -0.0000000001970	0.0000000001150	0.0000000000480\n");

            strbuild.AppendLine(" ------------------ add perturbing accelerations\n");
            aecef[0] = adrag[0];  // plus gravity xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            aecef[1] = adrag[1];
            aecef[2] = adrag[2];
            strbuild.AppendLine(" aecef " + aecef[0].ToString() + " " + aecef[1].ToString() + " " + aecef[2].ToString());

            // ---- move acceleration from earth fixed coordinates to eci
            // there are no cross products here as normal
            aeci = MathTimeLibr.matvecmult(transecef2eci, aecef, 3);
            strbuild.AppendLine(" aeci " + aeci[0].ToString() + " " + aeci[1].ToString() + " " + aeci[2].ToString());

            // find two body component of eci acceleration
            aeci2[0] = -AstroLibr.gravConst.mu * reci[0] / (Math.Pow(MathTimeLibr.mag(reci), 3));
            aeci2[1] = -AstroLibr.gravConst.mu * reci[1] / (Math.Pow(MathTimeLibr.mag(reci), 3));
            aeci2[2] = -AstroLibr.gravConst.mu * reci[2] / (Math.Pow(MathTimeLibr.mag(reci), 3));
            strbuild.AppendLine(" aeci2body " + aeci2[0].ToString() + " " + aeci2[1].ToString() + " " + aeci2[2].ToString());

            // totla acceleration
            aeci[0] = aeci2[0] + a3body[0] + asrp[0] + aeci[0];
            aeci[1] = aeci2[1] + a3body[1] + asrp[1] + aeci[1];
            aeci[2] = aeci2[2] + a3body[2] + asrp[2] + aeci[2];
            strbuild.AppendLine("total aeci " + aeci[0].ToString() + " " + aeci[1].ToString() + " " + aeci[2].ToString());




            // ------------------------------------------- timing comparisons
            strbuild.AppendLine("\n ===================================== Timing Comparisons =====================================");
            // timing of routines
            var watch = System.Diagnostics.Stopwatch.StartNew();

            for (int i = 0; i < 500; i++)
            {
                straccum = "";
                order = 50;
                // normalized calcs, show
                AstroLibr.FullGeopM(recef, order, 'y', convArr, gravData, out aPertM, 'n', out straccum);
            }
            //  stop timer
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            strbuild.AppendLine("Done with Montenbruck calcs " + (watch.ElapsedMilliseconds * 0.001).ToString() + " sec  ");

            watch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < 500; i++)
            {
                straccum = "";
                order = 50;
                // normalized calcs, show
                AstroLibr.FullGeopG(recef, order, 'y', convArr, normArr, gravData, out aPertG, 'n', out straccum);
            }
            //  stop timer
            watch.Stop();
            elapsedMs = watch.ElapsedMilliseconds;
            strbuild.AppendLine("Done with GTDS calcs " + (watch.ElapsedMilliseconds * 0.001).ToString() + " sec  ");


            watch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < 500; i++)
            {
                straccum = "";
                order = 100;
                // normalized calcs, show
                // GTDS version
                AstroLibr.LegPolyG(latgc, order, 'y', convArr, normArr, out LegArrGU, out LegArrGN);
            }
            //  stop timer
            watch.Stop();
            elapsedMs = watch.ElapsedMilliseconds;
            strbuild.AppendLine("Done with GTDS ALF calcs " + (watch.ElapsedMilliseconds * 0.001).ToString() + " sec  ");


            watch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < 500; i++)
            {
                straccum = "";
                order = 100;
                // normalized calcs, show
                // Gottlieb version
                AstroLibr.LegPolyGot(latgc, order, 'y', convArr, normArr, out LegArrGotU, out LegArrGotN);
            }
            //  stop timer
            watch.Stop();
            elapsedMs = watch.ElapsedMilliseconds;
            strbuild.AppendLine("Done with Gott ALF calcs " + (watch.ElapsedMilliseconds * 0.001).ToString() + " sec  ");

            watch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < 500; i++)
            {
                straccum = "";
                order = 100;
                // normalized calcs, show
                // Montenbruck version
                AstroLibr.LegPolyM(latgc, order, 'y', convArr, out LegArrMU, out LegArrMN);
            }
            //  stop timer
            watch.Stop();
            elapsedMs = watch.ElapsedMilliseconds;
            strbuild.AppendLine("Done with Mont ALF calcs " + (watch.ElapsedMilliseconds * 0.001).ToString() + " sec  ");


            watch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < 500; i++)
            {
                straccum = "";
                order = 100;
                // normalized calcs, show
                // Fukushima version
                AstroLibr.LegPolyF(latgc, order, 'y', normArr, out LegArrF);
            }
            //  stop timer
            watch.Stop();
            elapsedMs = watch.ElapsedMilliseconds;
            strbuild.AppendLine("Done with Fukushima ALF calcs " + (watch.ElapsedMilliseconds * 0.001).ToString() + " sec  ");



            // ------------------------------------------- pole test case comparisons
            strbuild.AppendLine("\n ===================================== Pole Test Comparisons =====================================");

            rad = 180.0 / Math.PI;
            for (int i = 0; i < 500; i++)
            {
                lon = 154.0 / rad;
                latgc = (89.9 + (i / 1000.0)) / rad;
                double magr = 7378.382745;

                recef[0] = (magr * Math.Cos(latgc) * Math.Cos(lon));
                recef[1] = (magr * Math.Cos(latgc) * Math.Sin(lon));
                recef[2] = (magr * Math.Sin(latgc));

                straccum = "";
                order = 50;
                // normalized calcs, show
                AstroLibr.FullGeopG(recef, order, 'y', convArr, normArr, gravData, out aPertG, 'n', out straccum);

                strbuild.AppendLine("test pole " + (latgc * rad).ToString() + " " + (lon * rad).ToString() + " " + aPertM[0].ToString() + "     " + aPertM[1].ToString() + "     " + aPertM[2].ToString());
            }


            // available files:
            // GEM10Bunnorm36.grv
            // GEMT1norm36.grv
            // GEMT2norm36.grv
            // GEMT3norm36.grv
            // GEMT3norm50.grv
            // EGM-96norm70.grv
            // EGM-96norm254.grv
            // EGM-08norm100.grv
            // EGM-96norm70.grv
            // EGM-96norm70.grv
            // EGM-96norm70.grv
            // GGM01Cnorm90.grv
            // GGM02Cnorm90.grv
            // GGM03Cnorm100.grv
            // JGM2norm70.grv
            // JGM3norm70.grv
            // WGS-84_EGM96norm70.grv
            // WGS-84Enorm180.grv
            // WGS-84norm70.grv
            // 
            //string fname = "D:\Dataorig\Gravity\EGM96A.TXT";       // norm
            //string fname = "D:\Dataorig\Gravity\egm2008_gfc.txt";  // norm

            // --------------------gottlieb 1993 test
            strbuild.AppendLine("===================================== Gottlieb 1993 test case ===================================== ");
            strbuild.AppendLine("GEM-10B unnormalized 36x36 ");
            // get past text in each file
            //if (fname.Contains("GEM"))    // GEM10bunnorm36.grv, GEMT3norm50.grv
            //    startKtr = 17;
            //if (fname.Contains("EGM-96")) // EGM-96norm70.grv
            //    startKtr = 73;
            //if (fname.Contains("EGM-08")) // EGM-08norm100.grv
            //    startKtr = 83;  // or 21 for the larger file... which has gfc in the first col too
            fname = "D:/Dataorig/Gravity/GEM10Bunnorm36.grv";
            normal = 'n';
            //double latgc;
            //Int32 degree, order;
            //double[,] LegArr;  // montenbruck
            //double[,] LegArrN;
            //double[,] LegArrG;  // gtds
            //double[,] LegArrGN;
            ////  double[,] LegArrEx;
            //double[,] LegArr1;  // geodyn

            //AstroLib.gravityModelData gravData;

            recef = new double[] { 5489.1500, 802.2220, 3140.9160 };  // km
            strbuild.AppendLine("recef = " + recef[0].ToString() + " " + recef[1].ToString() + " " + recef[2].ToString());
            // these are from the vector
            latgc = Math.Asin(recef[2] / MathTimeLibr.mag(recef));
            double templ = Math.Sqrt(recef[0] * recef[0] + recef[1] * recef[1]);
            double rtasc;
            if (Math.Abs(templ) < 0.0000001)
                rtasc = Math.Sign(recef[2]) * Math.PI * 0.5;
            else
                rtasc = Math.Atan2(recef[1], recef[0]);
            lon = rtasc;
            strbuild.AppendLine("latgc lon " + (latgc * rad).ToString() + " " + (lon * rad).ToString());

            this.opsStatus.Text = "Status: Reading gravity field Gottlieb test";
            Refresh();

            AstroLibr.initGravityField(fname, 17, normal, out order, out gravData, out convArr, out normArr);
            strbuild.AppendLine("\ncoefficents --------------- ");
            strbuild.AppendLine("c  2  0  " + gravData.c[2, 0].ToString() + " s " + gravData.s[2, 0].ToString());
            strbuild.AppendLine("c  4  0  " + gravData.c[4, 0].ToString() + " s " + gravData.s[4, 0].ToString());
            strbuild.AppendLine("c  4  4  " + gravData.c[4, 4].ToString() + " s " + gravData.s[4, 4].ToString());
            strbuild.AppendLine("c 21  1 " + gravData.c[21, 1].ToString() + " s " + gravData.s[21, 1].ToString());
            strbuild.AppendLine("\nnormalized coefficents --------------- ");
            strbuild.AppendLine("c  2  0  " + gravData.cNor[2, 0].ToString() + " s " + gravData.sNor[2, 0].ToString());
            strbuild.AppendLine("c  4  0  " + gravData.cNor[4, 0].ToString() + " s " + gravData.sNor[4, 0].ToString());
            strbuild.AppendLine("c  4  4  " + gravData.cNor[4, 4].ToString() + " s " + gravData.sNor[4, 4].ToString());
            strbuild.AppendLine("c 21  1 " + gravData.cNor[21, 1].ToString() + " s " + gravData.sNor[21, 1].ToString());

            this.opsStatus.Text = "Status: Gottlieb test legpoly calcs";
            Refresh();

            degree = 36;
            order = 36;
            AstroLibr.LegPolyG(latgc, order, normalized, convArr, normArr, out LegArrGU, out LegArrGN);
            AstroLibr.LegPolyM(latgc, order, normalized, convArr, out LegArrMU, out LegArrMN);
            // get geodyn version
            AstroLibr.geodynlegp(latgc, degree, order, out LegArrOU, out LegArrON);
            // get exact values
            // LegPolyEx(latgc, order, out LegArrEx);

            errstr = " ";
            sumdr1 = 0.0;
            sumdr2 = 0.0;
            strbuild.AppendLine("\nLegendre polynomials --------------- ");
            for (int L = 1; L <= 6; L++)  // order xxxxxxxxxxxxxxxxxx
            {
                string tempstr1 = "MN ";  // montenbruck
                string tempstr2 = "GN ";  // gtds
                string tempstr3 = "MU ";
                string tempstr4 = "OU ";  // geodyn
                for (int m = 0; m <= L; m++)
                {
                    tempstr1 = tempstr1 + " " + L.ToString() + "  " + m.ToString() + "   " + LegArrMN[L, m].ToString();
                    tempstr2 = tempstr2 + " " + L.ToString() + "  " + m.ToString() + "   " + LegArrGN[L, m].ToString();
                    tempstr3 = tempstr3 + " " + L.ToString() + "  " + m.ToString() + "   " + LegArrMU[L, m].ToString();
                    tempstr4 = tempstr4 + " " + L.ToString() + "  " + m.ToString() + "   " + LegArrOU[L + 1, m + 1].ToString();
                    //dr1 = 100.0 * (LegArr[L, m] - LegArrEx[L, m]) / LegArrEx[L, m];
                    //dr2 = 100.0 * (LegArr1[L, m] - LegArrEx[L, m]) / LegArrEx[L, m];
                    //sumdr1 = sumdr1 + dr1;
                    //sumdr2 = sumdr2 + dr2;
                    //errstr = errstr + "\n" + L.ToString() + "  " + m.ToString() + "   " + dr1.ToString()
                    //    + " " + dr2.ToString();
                }
                strbuild.AppendLine(tempstr1);
                strbuild.AppendLine(tempstr2);
                //  strbuild.AppendLine(tempstr3);
                strbuild.AppendLine(tempstr4 + "\n");
            }
            strbuild.AppendLine("totals gtds " + sumdr1.ToString() + " montenbruck " + sumdr2.ToString());
            strbuildplot.AppendLine(errstr);

            strbuild.AppendLine("\naccelerations --------------- ");
            jdutc = 2451573.0;
            jdF = 0.1;
            straccum = "";
            order = 4;
            // normalized calcs, show
            AstroLibr.FullGeopM(recef, order, 'n', convArr, gravData, out aPertM, 'y', out straccum);
            // add in two body term since full geop is only disturbing part
            jdut1 = jdutc + jdF;
            //AstroLibr.eci_ecef(ref reci, ref veci, iau80arr, MathTimeLib.Edirection.efrom, ttt, jdut1, lod, xp, yp, eqeterms, ddpsi, ddeps, AstroLib.EOpt.e80, ref recef, ref vecef);
            // time is not given, so let ecef and eci be =
            reci[0] = recef[0];
            reci[1] = recef[1];
            reci[2] = recef[2];

            aeci2[0] = -398600.47 * reci[0] / (Math.Pow(MathTimeLibr.mag(reci), 3));
            aeci2[1] = -398600.47 * reci[1] / (Math.Pow(MathTimeLibr.mag(reci), 3));
            aeci2[2] = -398600.47 * reci[2] / (Math.Pow(MathTimeLibr.mag(reci), 3));
            //aPertG[0] = aPertG[0] + aeci2[0];
            //aPertG[1] = aPertG[1] + aeci2[1];
            //aPertG[2] = aPertG[2] + aeci2[2];
            aPertM[0] = aPertM[0] + aeci2[0];
            aPertM[1] = aPertM[1] + aeci2[1];
            aPertM[2] = aPertM[2] + aeci2[2];


            strbuild.AppendLine(straccum);
            //   strbuild.AppendLine("apertG 4 4   " + aPertG[0].ToString() + "     " + aPertG[1].ToString() + "     " + aPertG[2].ToString());
            strbuild.AppendLine("apertM 4 4   " + aPertM[0].ToString() + "     " + aPertM[1].ToString() + "     " + aPertM[2].ToString());
            strbuild.AppendLine("ans          -0.00844269212018857E+00 -0.00123393633785485E+00 -0.00484659352346614E+00  km/s2  \n");

            straccum = "";
            order = 5;
            // normalized calcs, show
            AstroLibr.FullGeopG(recef, order, 'n', convArr, normArr, gravData, out aPertG, 'y', out straccum);
            strbuild.AppendLine(straccum);
            strbuild.AppendLine("apertG 5 5   " + aPertG[0].ToString() + "     " + aPertG[1].ToString() + "     " + aPertG[2].ToString());
            // strbuild.AppendLine("apertM 5 5   " + aPertM[0].ToString() + "     " + aPertM[1].ToString() + "     " + aPertM[2].ToString());
            strbuild.AppendLine("ans          -0.00844260633555472E+00 -0.00123393243051834E+00 -0.00484652486332608E+00  km/s2  \n");



            // --------------------fonte 1993 test
            // strbuild.AppendLine("\n ===================================== Fonte 1993 test case =====================================");
            // strbuild.AppendLine("GEM-10B unnormalized 36x36 ");
            // fname = "D:/Dataorig/Gravity/GEM10Bunnorm36.grv";
            // normal = 'n';
            // recef = new double[] { 180.295260378399, -1145.13224944286, -6990.09446227757 }; // km
            // strbuild.AppendLine("recef = " + recef[0].ToString() + " " + recef[1].ToString() + " " + recef[2].ToString());
            // latgc = -1.40645188850273;
            // lon = -4.09449590512370;
            // strbuild.AppendLine("latgc lon " + (latgc * rad).ToString() + " " + (lon * rad).ToString());

            // this.opsStatus.Text = "Status: Reading gravity field Fonte test";
            // Refresh();

            // // Un-normalized Polynomial Validation GEM10B
            // // GTDS vs Lundberg Truth GTDS (21x21 GEM10B)
            // strbuild.AppendLine("\ncoefficients --------------- ");
            // AstroLibr.initGravityField(fname, normal, out gravData);
            // strbuild.AppendLine("c  4  0    " + gravData.c[4, 0].ToString() + " s " + gravData.s[4, 0].ToString());
            // strbuild.AppendLine("c 21  0   " + gravData.c[21, 0].ToString() + " s " + gravData.s[21, 0].ToString());
            // strbuild.AppendLine("c 21  5    " + gravData.c[21, 5].ToString() + " s " + gravData.s[21, 5].ToString());
            // strbuild.AppendLine("c 21 20   " + gravData.c[21, 20].ToString() + " s " + gravData.s[21, 20].ToString());
            // strbuild.AppendLine("c 21 21    " + gravData.c[21, 21].ToString() + " s " + gravData.s[21, 21].ToString());

            // // GTDS Emulation vs Lundberg Truth (21x21 GEM10B)
            // degree = 21;
            // order = 21;
            // AstroLibr.LegPoly(latgc, order, out LegArr, out LegArrG, out LegArrN, out LegArrGN);
            // // get geodyn version
            // AstroLibr.geodynlegp(latgc, degree, order, out LegArr1);
            // // get exact values
            // //   LegPolyEx(latgc, order, out LegArrEx);

            // dr1 = 0.0;
            // dr2 = 0.0;
            // sumdr1 = 0.0;
            // sumdr2 = 0.0;
            // strbuild.AppendLine("\nLegendre polynomials --------------- ");
            // for (int L = 1; L <= 6; L++)  // order
            // {
            //     string tempstr1 = "M ";
            //     string tempstr2 = "G ";
            //     string tempstr3 = "E ";
            //     string tempstr4 = "O ";
            //     for (int m = 0; m <= L; m++)
            //     {
            //         tempstr1 = tempstr1 + " " + L.ToString() + "  " + m.ToString() + "   " + LegArrN[L, m].ToString();
            //         tempstr2 = tempstr2 + " " + L.ToString() + "  " + m.ToString() + "   " + LegArrGN[L, m].ToString();
            //         // tempstr3 = tempstr3 + " " + L.ToString() + "  " + m.ToString() + "   " + LegArrEx[L, m].ToString();
            //         tempstr4 = tempstr4 + " " + L.ToString() + "  " + m.ToString() + "   " + LegArr1[L + 1, m + 1].ToString();
            //         //    dr1 = 100.0 * (LegArr[L, m] - LegArrEx[L, m]) / LegArrEx[L, m];
            //         //    dr2 = 100.0 * (LegArr1[L, m] - LegArrEx[L, m]) / LegArrEx[L, m];
            //         //sumdr1 = sumdr1 + dr1;
            //         //sumdr2 = sumdr2 + dr2;
            //         //errstr = errstr + "\n" + L.ToString() + "  " + m.ToString() + "   " + dr1.ToString()
            //         //    + " " + dr2.ToString();
            //     }
            //     strbuild.AppendLine(tempstr1);
            //     strbuild.AppendLine(tempstr2);
            //    // strbuild.AppendLine(tempstr3);
            //     strbuild.AppendLine(tempstr4 + "\n");
            // }
            //// strbuild.AppendLine("totals gtds " + sumdr1.ToString() + " montenbruck " + sumdr2.ToString());
            // strbuild.AppendLine("ans 21  0 0.385389365005720                                                                      21  5   354542.107743601  354542.1077435970657340");
            // strbuild.AppendLine("ans 21 20         -2442182686.11423  -2442182686.11409981594");
            // strbuild.AppendLine("ans 21 21          405012060.632803  405012060.6327805324689" + "\n");

            // strbuild.AppendLine("\naccelerations --------------- ");
            // AstroLibr.FullGeop(recef, jd, jdF, order, gravData, out aPert, out aPert1);

            // strbuild.AppendLine("apertG 21 21   " + aPert[0].ToString() + " " + aPert[1].ToString() + " " + aPert[2].ToString());
            // strbuild.AppendLine("apertM 21 21   " + aPert1[0].ToString() + "     " + aPert1[1].ToString() + "     " + aPert1[2].ToString());
            // strbuild.AppendLine("ans             8.653210294968294E-7  -6.515584998975128E-6  -1.931032474628621E-5 ");
            // strbuild.AppendLine("ans             8.653210294968E-7     -6.5155849989750E-6    -1.931032474628616E-5");

            // // --------------------fonte 1993 test
            // strbuild.AppendLine("\n===================================== Fonte 1993 test case =====================================");
            // strbuild.AppendLine("GEM-T3 normalized 50x50 ");
            // fname = "D:/Dataorig/Gravity/GEMT3norm50.grv";          // norm only released as 36x36 though...
            // normal = 'y';

            // this.opsStatus.Text = "Status: Reading gravity field fonte 93 test";
            // Refresh();

            // AstroLibr.initGravityField(fname, normal, out gravData);
            // strbuild.AppendLine("\ncoefficients --------------- ");
            // strbuild.AppendLine("c  4  0   " + gravData.c[4, 0].ToString() + " " + gravData.s[4, 0].ToString());
            // strbuild.AppendLine("c 21 20   " + gravData.c[21, 20].ToString() + " " + gravData.s[21, 20].ToString());
            // strbuild.AppendLine("c 50  0   " + gravData.c[50, 0].ToString() + " " + gravData.s[50, 0].ToString());
            // strbuild.AppendLine("c 50 50   " + gravData.c[50, 50].ToString() + " " + gravData.s[50, 50].ToString());
            // strbuild.AppendLine("c 50  5   " + gravData.c[50, 5].ToString() + " " + gravData.s[50, 5].ToString());

            // strbuild.AppendLine("\nLegendre polynomials --------------- ");
            // // GTDS Emulation vs Lundberg Truth (21x21 GEM10B)
            // degree = 50;
            // order = 50;

            // AstroLibr.LegPoly(latgc, order, out LegArr, out LegArrG, out LegArrN, out LegArrGN);
            // // get geodyn version
            // AstroLibr.geodynlegp(latgc, degree, order, out LegArr1);
            // // get exact
            // //  LegPolyEx(latgc, order, out LegArrEx);

            // strbuild.AppendLine("legarr4    0   " + LegArrN[4, 0].ToString() + " " + LegArrN[4, 1].ToString());

            // strbuild.AppendLine("50  0          " + LegArrN[50, 0].ToString());
            // strbuild.AppendLine("50  0 alt      " + LegArrGN[50, 0].ToString());
            // strbuild.AppendLine("ans 50  0      0.09634780379822722     9.634780379823085162E-02");
            // strbuild.AppendLine("50  0 geody    " + LegArr1[50, 0].ToString() + "\n");
            // //   strbuild.AppendLine("50  0 exact    " + LegArrEx[50, 0].ToString() + "\n");
            // //    strbuild.AppendLine("50  0 exact    " + LegArrEx[50, 0].ToString() + "\n");

            // strbuild.AppendLine("50 21       " + LegArrN[50, 21].ToString());
            // strbuild.AppendLine("50 21 alt   " + LegArrGN[50, 21].ToString());
            // strbuild.AppendLine("ans 50  21  -1.443200082785759E+28  -14432000827857661203015450149.6553");
            // strbuild.AppendLine("50 21 geody " + LegArr1[50, 21].ToString() + "\n");
            // //   strbuild.AppendLine("50 21 exact  " + LegArrEx[50, 21].ToString() + "\n");

            // strbuild.AppendLine("50 49       " + LegArrN[50, 49].ToString());
            // strbuild.AppendLine("50 49 alt   " + LegArrGN[50, 49].ToString());
            // strbuild.AppendLine("ans 50  49  -8.047341511222794E+39  -8.047341511222872818E+39");
            // strbuild.AppendLine("50 49 geody " + LegArr1[50, 49].ToString() + "\n");
            // // strbuild.AppendLine("50 49 exact " + ex5049.ToString() + "\n");

            // strbuild.AppendLine("50 50       " + LegArrN[50, 50].ToString());
            // strbuild.AppendLine("50 50 alt   " + LegArrGN[50, 50].ToString());
            // strbuild.AppendLine("ans 50 50      1.334572710963763E+39   1.334572710963775698E+39" + "\n");
            // strbuild.AppendLine("50 50 geody " + LegArr1[50, 50].ToString() + "\n");
            // //strbuild.AppendLine("50 50 exact " + ex550.ToString() + "\n");

            // strbuild.AppendLine("\naccelerations --------------- ");
            // normalized calcs, show
            // AstroLibr.FullGeop(recef, order, normalized, gravData, out aPert, out aPert1);
            // strbuild.AppendLine("apert 50 50   " + aPert[0].ToString() + " " + aPert[1].ToString() + " " + aPert[2].ToString());
            // strbuild.AppendLine("ans           8.683465146150188E-007    -6.519678538340073E-006   -1.931876804829165E-005");
            // strbuild.AppendLine("ans           8.68346514615019361E-07   -6.51967853834008023E-06  -1.93187680482916393E-05");

            // recef = new double[] { 487.0696937, -5330.5022406, 4505.7372146 };  // m
            // vecef = new double[] { -2.101083975, 4.624581986, 5.688300377 };

            // write out results
            string directory = @"d:\codes\library\matlab\";
            File.WriteAllText(directory + "legpoly.txt", strbuild.ToString());

            File.WriteAllText(directory + "legendreAcc.txt", strbuildplot.ToString());

        }


        /* ----------------------------------------------------------------------------
*
*                                 function printcov
*
* this function prints a covariance matrix
*
* author        : david vallado                  719 - 573 - 2600   23 may 2003
*
* revisions
*
* inputs description range / units
*   covin     - 6x6 input covariance matrix
*   covtype   - type of covariance             'cl','ct','fl','sp','eq',
*   cu        - covariance units(deg or rad)  't' or 'm'
*   anom      - anomaly                        'mean' or 'true' or 'tau'
*
* outputs       :
*
* locals        :
*
* references    :
* none
*
* printcov(covin, covtype, cu, anom)
* ----------------------------------------------------------------------------*/

        public void printcov(double[,] covin, string covtype, char cu, string anom, out string strout)
        {
            int i;
            string semi = "";
            strout = "";

            if (anom.Equals("truea") || anom.Equals("meana"))
                semi = "a m  ";
            else
            {
                if (anom.Equals("truen") || anom.Equals("meann"))
                    semi = "n rad";
            }

            if (covtype.Equals("ct"))
            {
                strout = "cartesian covariance \n";
                strout = strout + "        x  m            y m             z  m           xdot  m/s       ydot  m/s       zdot  m/s  \n";
            }

            if (covtype.Equals("cl"))
            {
                strout = strout + "classical covariance \n";
                if (cu == 'm')
                {
                    strout = strout + "          " + semi + "          ecc           incl rad      raan rad         argp rad        ";
                    if (anom.Contains("mean")) // 'meana' 'meann'
                        strout = strout + "m rad \n";
                    else     // 'truea' 'truen'
                        strout = strout + " nu rad \n";
                }
                else
                {
                    strout = strout + "          " + semi + "           ecc           incl deg      raan deg         argp deg        ";
                    if (anom.Contains("mean")) // 'meana' 'meann'
                        strout = strout + " m deg \n";
                    else     // 'truea' 'truen'
                        strout = strout + " nu deg \n";
                }
            }

            if (covtype.Equals("eq"))
            {
                strout = strout + "equinoctial covariance \n";
                //            if (cu == 'm')
                if (anom.Contains("mean")) // 'meana' 'meann'
                    strout = strout + "         " + semi + "           af              ag           chi             psi         meanlonM rad\n";
                else     // 'truea' 'truen'
                    strout = strout + "         " + semi + "           af              ag           chi             psi         meanlonNu rad\n";
            }

            if (covtype.Equals("fl"))
            {
                strout = strout + "flight covariance \n";
                strout = strout + "       lon  rad      latgc rad        fpa rad         az rad           r  m           v  m/s  \n";
            }

            if (covtype.Equals("sp"))
            {
                strout = strout + "spherical covariance \n";
                strout = strout + "      rtasc deg       decl deg        fpa deg         az deg           r  m           v  m/s  \n";
            }

            // format strings to show signs "and" to not round off if trailing 0!!
            string fmt = "+#.#########0E+00;-#.#########0E+00";
            for (i = 0; i < 6; i++)
                strout = strout + covin[i, 0].ToString(fmt) + " " + covin[i, 1].ToString(fmt) + " " + covin[i, 2].ToString(fmt) + " " +
                 covin[i, 3].ToString(fmt) + " " + covin[i, 4].ToString(fmt) + " " + covin[i, 5].ToString(fmt) + "\n";
        }  // printcov


        /* ----------------------------------------------------------------------------
        *
        *                                  function printdiff
        *
        * this function prints a covariance matrix difference
        *
        * author        : david vallado                  719 - 573 - 2600   23 may 2003
        *
        * revisions
        *
        * inputs description range / units
        *   strin    - title
        *   mat1     - 6x6 input matrix
        *   mat2     - 6x6 input matrix
        *
        * outputs       :
        *
        * locals        :
        *
        * ----------------------------------------------------------------------------*/

        public void printdiff(string strin, double[,] mat1, double[,] mat2, out string strout)
        {
            double small = 1e-18;
            double[,] dr = new double[6, 6];
            double[,] diffmm = new double[6, 6];
            int i, j;

            // format strings to show signs "and" to not round off if trailing 0!!
            string fmt = "+#.#########0E+00;-#.#########0E+00";

            strout = "diff " + strin + "\n";
            for (i = 0; i < 6; i++)
            {
                for (j = 0; j < 6; j++)
                    dr[i, j] = mat1[i, j] - mat2[i, j];
                strout = strout + dr[i, 0].ToString(fmt) + " " + dr[i, 1].ToString(fmt) + " " + dr[i, 2].ToString(fmt) + " " +
                    dr[i, 3].ToString(fmt) + " " + dr[i, 4].ToString(fmt) + " " + dr[i, 5].ToString(fmt) + "\n";
            }

            strout = strout + "pctdiff % " + strin + " pct over 1e-18  \n";
            // fprintf(1, '%14.4f%14.4f%14.4f%14.4f%14.4f%14.4f \n', 100.0 * ((mat1' - mat2') / mat1'));
            // fprintf(1, 'Check consistency of both approaches tmct2cl-inv(tmcl2ct) diff pct over 1e-18 \n');
            // fprintf(1, '-------- accuracy of tm comparing ct2cl and cl2ct --------- \n');
            //tm1 = mat1';
            //tm2 = mat2';
            fmt = "+0.###0;-0.###0";
            for (i = 0; i < 6; i++)
            {
                for (j = 0; j < 6; j++)
                {
                    if (Math.Abs(dr[i, j]) < small || Math.Abs(mat1[i, j]) < small)
                        diffmm[i, j] = 0.0;
                    else
                        diffmm[i, j] = 100.0 * (dr[i, j] / mat1[i, j]);
                }
                strout = strout + diffmm[i, 0].ToString(fmt) + " " + diffmm[i, 1].ToString(fmt) + " " + diffmm[i, 2].ToString(fmt) + " " +
                     diffmm[i, 3].ToString(fmt) + " " + diffmm[i, 4].ToString(fmt) + " " + diffmm[i, 5].ToString(fmt) + "\n";
            }

        }  // printdiff



        public void testcovct2rsw()
        {
        }

        public void testcovct2ntw()
        {

        }


        // test eci_ecef too
        public void testcovct2clmean()
        {
            int year, mon, day, hr, minute, timezone, dat, terms;
            double sec, dut1, ut1, tut1, jdut1, jdut1frac, utc, tai, tt, ttt, jdtt, jdttfrac, tdb, ttdb, jdtdb, jdtdbfrac;
            double p, a, n, ecc, incl, raan, argp, nu, m, arglat, truelon, lonper;
            double af, ag, chi, psi, meanlonNu, meanlonM;
            double latgc, lon, rtasc, decl, fpa, lod, xp, yp, ddpsi, ddeps, ddx, ddy, az, magr, magv;
            Int16 fr;
            double[,] tm = new double[,] { { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 } };
            string anom = "meana";  // truea/n, meana/n
            string anomflt = "latlon"; // latlon  radec
            double[] cartstate = new double[6];
            double[] classstate = new double[6];
            double[] eqstate = new double[6];
            double[] fltstate = new double[6];
            double[] recef = new double[3];
            double[] vecef = new double[3];
            double[] avec = new double[3];

            double[,] classcovmeana = new double[6, 6];
            double[,] cartcovmeanarev = new double[6, 6];
            double[,] tmct2cl = new double[6, 6];
            double[,] tmcl2ct = new double[6, 6];
            string strout;

            double[] reci = new double[3] { -605.79221660, -5870.22951108, 3493.05319896 };
            double[] veci = new double[3] { -1.56825429, -3.70234891, -6.47948395 };
            double[] aeci = new double[3] { 0.001, 0.002, 0.003 };

            // StringBuilder strbuild = new StringBuilder();
            // strbuild.Clear();

            EOPSPWLib.iau80Class iau80arr;
            EOPSPWLib.iau00Class iau00arr;
            string nutLoc;
            nutLoc = @"D:\Codes\LIBRARY\DataLib\nut80.dat";
            EOPSPWLibr.iau80in(nutLoc, out iau80arr);
            nutLoc = @"D:\Codes\LIBRARY\DataLib\";
            EOPSPWLibr.iau00in(nutLoc, out iau00arr);
            // now read it in
            double jdxysstart, jdfxysstart;
            AstroLib.xysdataClass[] xysarr = AstroLibr.xysarr;
            AstroLibr.initXYS(ref xysarr, nutLoc, "xysdata.dat", out jdxysstart, out jdfxysstart);

            year = 2000;
            mon = 12;
            day = 15;
            hr = 16;
            minute = 58;
            sec = 50.208;
            dut1 = 0.10597;
            dat = 32;
            timezone = 0;
            xp = 0.0;
            yp = 0.0;
            lod = 0.0;
            terms = 2;
            timezone = 0;
            ddpsi = 0.0;
            ddeps = 0.0;
            ddx = 0.0;
            ddy = 0.0;

            MathTimeLibr.convtime(year, mon, day, hr, minute, sec, timezone, dut1, dat,
                out ut1, out tut1, out jdut1, out jdut1frac, out utc, out tai,
                out tt, out ttt, out jdtt, out jdttfrac,
                out tdb, out ttdb, out jdtdb, out jdtdbfrac);

            // ---convert the eci state into the various other state formats(classical, equinoctial, etc)
            double[,] cartcov = new double[,] {
            { 100.0, 1.0e-2, 1.0e-2, 1.0e-4, 1.0e-4, 1.0e-4 },
            {1.0e-2, 100.0,  1.0e-2, 1.0e-4,   1.0e-4,   1.0e-4},
            {1.0e-2, 1.0e-2, 100.0,  1.0e-4,   1.0e-4,   1.0e-4},
            {1.0e-4, 1.0e-4, 1.0e-4, 0.0001,   1.0e-6,   1.0e-6},
            {1.0e-4, 1.0e-4, 1.0e-4, 1.0e-6,   0.0001,   1.0e-6},
            {1.0e-4, 1.0e-4, 1.0e-4, 1.0e-6,   1.0e-6,   0.0001} };
            cartstate = new double[] { reci[0], reci[1], reci[2], veci[0], veci[1], veci[2] };  // in km

            // --------convert to a classical orbit state
            AstroLibr.rv2coe(reci, veci, 
                out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out arglat, out truelon, out lonper);
            classstate[0] = a;   // km
            classstate[1] = ecc;
            classstate[2] = incl;
            classstate[3] = raan;
            classstate[4] = argp;
            if (anom.Contains("mean")) // meann or meana
                classstate[5] = m;
            else  // truea or truen
                classstate[5] = nu;

            // -------- convert to an equinoctial orbit state
            AstroLibr.rv2eq(reci, veci, out a, out n, out af, out ag, out chi, out psi, out meanlonM, out meanlonNu, out fr);
            if (anom.Equals("meana") || anom.Equals("truea"))
                eqstate[0] = a;  // km
            else // meann or truen
                eqstate[0] = n;
            eqstate[1] = af;
            eqstate[2] = ag;
            eqstate[3] = chi;
            eqstate[4] = psi;
            if (anom.Contains("mean")) //  meana or meann
                eqstate[5] = meanlonM;
            else // truea or truen
                eqstate[5] = meanlonNu;

            // --------convert to a flight orbit state
            AstroLibr.rv2flt(reci, veci, jdtt, jdttfrac, jdut1, jdxysstart, lod, xp, yp, terms, ddpsi, ddeps, ddx, ddy, 
                iau80arr, iau00arr, 
                 out lon, out latgc, out rtasc, out decl, out fpa, out az, out magr, out magv);
            if (anomflt.Equals("radec"))
            {
                fltstate[0] = rtasc;
                fltstate[1] = decl;
            }
            else
            if (anomflt.Equals("latlon"))
            {
                fltstate[0] = lon;
                fltstate[1] = latgc;
            }
            fltstate[2] = fpa;
            fltstate[3] = az;
            fltstate[4] = magr;  // km
            fltstate[5] = magv;

            // test position and velocity going back
            avec = new double[] { 0.0, 0.0, 0.0 };

            AstroLibr.eci_ecef(ref reci, ref veci, MathTimeLib.Edirection.eto, ref recef, ref vecef,
                 AstroLib.EOpt.e80, iau80arr, iau00arr,
                 jdtt, jdttfrac, jdut1, jdxysstart, lod, xp, yp, ddpsi, ddeps, ddx, ddy);
            //vx = magv* ( -cos(lon)*sin(latgc)*cos(az)*cos(fpa) - sin(lon)*sin(az)*cos(fpa) + cos(lon)*cos(latgc)*sin(fpa) ); 
            //vy = magv* ( -sin(lon)*sin(latgc)*cos(az)*cos(fpa) + cos(lon)*sin(az)*cos(fpa) + sin(lon)*cos(latgc)*sin(fpa) );  
            //vz = magv* (sin(latgc) * sin(fpa) + cos(latgc)*cos(az)*cos(fpa) );
            //// correct:
            //ve1 = magv* ( -cos(rtasc)*sin(decl)*cos(az)*cos(fpa) - sin(rtasc)*sin(az)*cos(fpa) + cos(rtasc)*cos(decl)*sin(fpa) ); // m/s
            // ve2 = magv* (-sin(rtasc) * sin(decl) * cos(az) * cos(fpa) + cos(rtasc) * sin(az) * cos(fpa) + sin(rtasc) * cos(decl) * sin(fpa));
            //ve3 = magv* (sin(decl) * sin(fpa) + cos(decl)*cos(az)*cos(fpa) );

            strbuild.AppendLine("==================== do the sensitivity tests \n");

            strbuild.AppendLine("1.  Cartesian Covariance \n");
            printcov(cartcov, "ct", 'm', anom, out strout);
            strbuild.AppendLine(strout);

            strbuild.AppendLine("2.  Classical Covariance from Cartesian #1 above (" + anom + ") ------------------- \n");

            AstroLibr.covct2cl(cartcov, cartstate, anom, out classcovmeana, out tmct2cl);
            printcov(classcovmeana, "cl", 'm', anom, out strout);
            strbuild.AppendLine(strout);

            strbuild.AppendLine("  Cartesian Covariance from Classical #2 above \n");
            AstroLibr.covcl2ct(classcovmeana, classstate, anom, out cartcovmeanarev, out tmcl2ct);
            printcov(cartcovmeanarev, "ct", 'm', anom, out strout);
            strbuild.AppendLine(strout);
            strbuild.AppendLine("\n");

            printdiff(" cartcov - cartcovmeanarev \n", cartcov, cartcovmeanarev, out strout);
            strbuild.AppendLine(strout);

            double[,] ecefcartcov = new double[6, 6];

            //AstroLibr.coveci_ecef(ref cartcov, cartstate, MathTimeLib.Edirection.eto,  ref ecefcartcov, out tm, iau80arr,
            //            ttt, jdut1, lod, xp, yp, 2, ddpsi, ddeps, AstroLib.EOpt.e80);
            //printcov(cartcovmeanarev, "ct", 'm', anom, out strout);
            //strbuild.AppendLine(strout);
            //strbuild.AppendLine("\n");

        }  // testcovct2clmean


        public void testcovct2cltrue()
        {
            int year, mon, day, hr, minute, timezone, dat, terms;
            double sec, dut1, ut1, tut1, jdut1, jdut1frac, utc, tai, tt, ttt, jdtt, jdttfrac, tdb, ttdb, jdtdb, jdtdbfrac;
            double p, a, n, ecc, incl, raan, argp, nu, m, arglat, truelon, lonper;
            double af, ag, chi, psi, meanlonNu, meanlonM;
            double latgc, lon, rtasc, decl, fpa, lod, xp, yp, ddpsi, ddeps, ddx, ddy, az, magr, magv;
            Int16 fr;
            double[,] tm = new double[,] { { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 } };
            string anom = "truea";  // truea/n, meana/n
            string anomflt = "latlon"; // latlon  radec
            double[] cartstate = new double[6];
            double[] classstate = new double[6];
            double[] eqstate = new double[6];
            double[] fltstate = new double[6];
            double[] recef = new double[3];
            double[] vecef = new double[3];
            double[] avec = new double[3];

            double[,] classcovtruea = new double[6, 6];
            double[,] cartcovtruearev = new double[6, 6];
            double[,] tmct2cl = new double[6, 6];
            double[,] tmcl2ct = new double[6, 6];
            string strout;

            double[] reci = new double[3] { -605.79221660, -5870.22951108, 3493.05319896 };
            double[] veci = new double[3] { -1.56825429, -3.70234891, -6.47948395 };
            double[] aeci = new double[3] { 0.001, 0.002, 0.003 };

            //StringBuilder strbuild = new StringBuilder();
            //strbuild.Clear();

            EOPSPWLib.iau80Class iau80arr;
            EOPSPWLib.iau00Class iau00arr;
            string nutLoc;
            nutLoc = @"D:\Codes\LIBRARY\DataLib\nut80.dat";
            EOPSPWLibr.iau80in(nutLoc, out iau80arr);
            nutLoc = @"D:\Codes\LIBRARY\DataLib\";
            EOPSPWLibr.iau00in(nutLoc, out iau00arr);
            // now read it in
            double jdxysstart, jdfxysstart;
            AstroLib.xysdataClass[] xysarr = AstroLibr.xysarr;
            AstroLibr.initXYS(ref xysarr, nutLoc, "xysdata.dat", out jdxysstart, out jdfxysstart);

            year = 2000;
            mon = 12;
            day = 15;
            hr = 16;
            minute = 58;
            sec = 50.208;
            dut1 = 0.10597;
            dat = 32;
            timezone = 0;
            xp = 0.0;
            yp = 0.0;
            lod = 0.0;
            terms = 2;
            timezone = 0;
            ddpsi = 0.0;
            ddeps = 0.0;
            ddx = 0.0;
            ddy = 0.0;

            MathTimeLibr.convtime(year, mon, day, hr, minute, sec, timezone, dut1, dat,
                out ut1, out tut1, out jdut1, out jdut1frac, out utc, out tai,
                out tt, out ttt, out jdtt, out jdttfrac,
                out tdb, out ttdb, out jdtdb, out jdtdbfrac);

            // ---convert the eci state into the various other state formats(classical, equinoctial, etc)
            double[,] cartcov = new double[,] {
            { 100.0, 1.0e-2, 1.0e-2, 1.0e-4, 1.0e-4, 1.0e-4 },
            {1.0e-2, 100.0,  1.0e-2, 1.0e-4,   1.0e-4,   1.0e-4},
            {1.0e-2, 1.0e-2, 100.0,  1.0e-4,   1.0e-4,   1.0e-4},
            {1.0e-4, 1.0e-4, 1.0e-4, 0.0001,   1.0e-6,   1.0e-6},
            {1.0e-4, 1.0e-4, 1.0e-4, 1.0e-6,   0.0001,   1.0e-6},
            {1.0e-4, 1.0e-4, 1.0e-4, 1.0e-6,   1.0e-6,   0.0001} };
            cartstate = new double[] { reci[0], reci[1], reci[2], veci[0], veci[1], veci[2] };  // in km

            // --------convert to a classical orbit state
            AstroLibr.rv2coe(reci, veci, 
                out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out arglat, out truelon, out lonper);
            classstate[0] = a;  // in km
            classstate[1] = ecc;
            classstate[2] = incl;
            classstate[3] = raan;
            classstate[4] = argp;
            if (anom.Contains("mean")) // meann or meana
                classstate[5] = m;
            else  // truea or truen
                classstate[5] = nu;

            // -------- convert to an equinoctial orbit state
            AstroLibr.rv2eq(reci, veci, out a, out n, out af, out ag, out chi, out psi, out meanlonM, out meanlonNu, out fr);
            if (anom.Equals("meana") || anom.Equals("truea"))
                eqstate[0] = a;  // km
            else // meann or truen
                eqstate[0] = n;
            eqstate[1] = af;
            eqstate[2] = ag;
            eqstate[3] = chi;
            eqstate[4] = psi;
            if (anom.Contains("mean")) //  meana or meann
                eqstate[5] = meanlonM;
            else // truea or truen
                eqstate[5] = meanlonNu;

            // --------convert to a flight orbit state
            AstroLibr.rv2flt(reci, veci, jdtt, jdttfrac, jdut1, jdxysstart, lod, xp, yp, terms, ddpsi, ddeps, ddx, ddy, 
                iau80arr, iau00arr, 
                 out lon, out latgc, out rtasc, out decl, out fpa, out az, out magr, out magv);
            if (anomflt.Equals("radec"))
            {
                fltstate[0] = rtasc;
                fltstate[1] = decl;
            }
            else
            if (anomflt.Equals("latlon"))
            {
                fltstate[0] = lon;
                fltstate[1] = latgc;
            }
            fltstate[2] = fpa;
            fltstate[3] = az;
            fltstate[4] = magr;  // in km
            fltstate[5] = magv;

            // test position and velocity going back
            avec = new double[] { 0.0, 0.0, 0.0 };

            AstroLibr.eci_ecef(ref reci, ref veci, MathTimeLib.Edirection.eto, ref recef, ref vecef,
                 AstroLib.EOpt.e80, iau80arr, iau00arr,
                 jdtt, jdttfrac, jdut1, jdxysstart, lod, xp, yp, ddpsi, ddeps, ddx, ddy);
            //vx = magv* ( -cos(lon)*sin(latgc)*cos(az)*cos(fpa) - sin(lon)*sin(az)*cos(fpa) + cos(lon)*cos(latgc)*sin(fpa) ); 
            //vy = magv* ( -sin(lon)*sin(latgc)*cos(az)*cos(fpa) + cos(lon)*sin(az)*cos(fpa) + sin(lon)*cos(latgc)*sin(fpa) );  
            //vz = magv* (sin(latgc) * sin(fpa) + cos(latgc)*cos(az)*cos(fpa) );
            //// correct:
            //ve1 = magv* ( -cos(rtasc)*sin(decl)*cos(az)*cos(fpa) - sin(rtasc)*sin(az)*cos(fpa) + cos(rtasc)*cos(decl)*sin(fpa) ); // m/s
            // ve2 = magv* (-sin(rtasc) * sin(decl) * cos(az) * cos(fpa) + cos(rtasc) * sin(az) * cos(fpa) + sin(rtasc) * cos(decl) * sin(fpa));
            //ve3 = magv* (sin(decl) * sin(fpa) + cos(decl)*cos(az)*cos(fpa) );

            strbuild.AppendLine("==================== do the sensitivity tests \n");

            strbuild.AppendLine("1.  Cartesian Covariance \n");
            printcov(cartcov, "ct", 'm', anom, out strout);
            strbuild.AppendLine(strout);

            strbuild.AppendLine("2.  Classical Covariance from Cartesian #1 above (" + anom + ") ------------------- \n");

            AstroLibr.covct2cl(cartcov, cartstate, anom, out classcovtruea, out tmct2cl);
            printcov(classcovtruea, "cl", 'm', anom, out strout);
            strbuild.AppendLine(strout);

            strbuild.AppendLine("  Cartesian Covariance from Classical #2 above \n");
            AstroLibr.covcl2ct(classcovtruea, classstate, anom, out cartcovtruearev, out tmcl2ct);
            printcov(cartcovtruearev, "ct", 'm', anom, out strout);
            strbuild.AppendLine(strout);
            strbuild.AppendLine("\n");

            printdiff(" cartcov - cartcovtruearev \n", cartcov, cartcovtruearev, out strout);
            strbuild.AppendLine(strout);
        }  // testcovct2cltrue

        public void testhill()
        {
            double[] r, v, rh, vh, rint, vint;
            double alt, dts;
            r = new double[3];
            v = new double[3];
            rh = new double[3];
            vh = new double[3];
            // StringBuilder strbuild = new StringBuilder();

            dts = 1400.0; // sec

            // circular orbit
            alt = 590.0;
            r[0]= AstroLibr.gravConst.re + alt;
            r[1]= 0.0;
            r[2]= 0.0;
            v[0]= 0.0;
            v[1]= Math.Sqrt(AstroLibr.gravConst.mu / MathTimeLibr.mag(r));
            v[2]= 0.0;

            rh[0] = 0.0;
            rh[1] = 0.0;
            rh[2] = 0.0;
            vh[0] = -0.1;
            vh[1] = -0.04;
            vh[2] = -0.02;

            for (int i = 1; i <= 50; i++)
            {
                dts = i * 60.0;  // sec
                AstroLibr.hillsr(rh, vh, alt, dts, out rint, out vint);
                strbuild.AppendLine(dts.ToString() + " " + rint[0].ToString() + " " + rint[1].ToString() + " " + rint[2].ToString() +
                    " " + vint[0].ToString() + " " + vint[1].ToString() + " " + vint[2].ToString() );
            }


            AstroLibr.hillsv(r, alt, dts, out vint);



        }  // test hill

        public void testcovcl2eq(string anom)
        {
            int year, mon, day, hr, minute, timezone, dat, terms;
            double sec, dut1, ut1, tut1, jdut1, jdut1frac, utc, tai, tt, ttt, jdtt, jdttfrac, tdb, ttdb, jdtdb, jdtdbfrac;
            double p, a, n, ecc, incl, raan, argp, nu, m, arglat, truelon, lonper;
            double af, ag, chi, psi, meanlonNu, meanlonM;
            double latgc, lon, rtasc, decl, fpa, lod, xp, yp, ddpsi, ddeps, ddx, ddy, az, magr, magv;
            Int16 fr;
            double[,] tm = new double[,] { { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 } };
            string anomflt = "latlon"; // latlon  radec
            double[] cartstate = new double[6];
            double[] classstate = new double[6];
            double[] eqstate = new double[6];
            double[] fltstate = new double[6];
            double[] recef = new double[3];
            double[] vecef = new double[3];
            double[] avec = new double[3];

            double[,] classcovmeana = new double[6, 6];
            double[,] cartcovmeanarev = new double[6, 6];
            double[,] eqcovmeana = new double[6, 6]; 
            double[,] tmct2cl = new double[6, 6];
            double[,] tmcl2ct = new double[6, 6];
            double[,] tmcl2eq = new double[6, 6];
            double[,] tmeq2cl = new double[6, 6];
            string strout;

            double[] reci = new double[3] { -605.79221660, -5870.22951108, 3493.05319896 };
            double[] veci = new double[3] { -1.56825429, -3.70234891, -6.47948395 };
            double[] aeci = new double[3] { 0.001, 0.002, 0.003 };

            // StringBuilder strbuild = new StringBuilder();
            // strbuild.Clear();

            EOPSPWLib.iau80Class iau80arr;
            EOPSPWLib.iau00Class iau00arr;
            string nutLoc;
            nutLoc = @"D:\Codes\LIBRARY\DataLib\nut80.dat";
            EOPSPWLibr.iau80in(nutLoc, out iau80arr);
            nutLoc = @"D:\Codes\LIBRARY\DataLib\";
            EOPSPWLibr.iau00in(nutLoc, out iau00arr);
            // now read it in
            double jdxysstart, jdfxysstart;
            AstroLib.xysdataClass[] xysarr = AstroLibr.xysarr;
            AstroLibr.initXYS(ref xysarr, nutLoc, "xysdata.dat", out jdxysstart, out jdfxysstart);

            year = 2000;
            mon = 12;
            day = 15;
            hr = 16;
            minute = 58;
            sec = 50.208;
            dut1 = 0.10597;
            dat = 32;
            timezone = 0;
            xp = 0.0;
            yp = 0.0;
            lod = 0.0;
            terms = 2;
            timezone = 0;
            ddpsi = 0.0;
            ddeps = 0.0;
            ddx = 0.0;
            ddy = 0.0;

            MathTimeLibr.convtime(year, mon, day, hr, minute, sec, timezone, dut1, dat,
                out ut1, out tut1, out jdut1, out jdut1frac, out utc, out tai,
                out tt, out ttt, out jdtt, out jdttfrac,
                out tdb, out ttdb, out jdtdb, out jdtdbfrac);

            // ---convert the eci state into the various other state formats(classical, equinoctial, etc)
            double[,] cartcov = new double[,] {
            { 100.0, 1.0e-2, 1.0e-2, 1.0e-4, 1.0e-4, 1.0e-4 },
            {1.0e-2, 100.0,  1.0e-2, 1.0e-4,   1.0e-4,   1.0e-4},
            {1.0e-2, 1.0e-2, 100.0,  1.0e-4,   1.0e-4,   1.0e-4},
            {1.0e-4, 1.0e-4, 1.0e-4, 0.0001,   1.0e-6,   1.0e-6},
            {1.0e-4, 1.0e-4, 1.0e-4, 1.0e-6,   0.0001,   1.0e-6},
            {1.0e-4, 1.0e-4, 1.0e-4, 1.0e-6,   1.0e-6,   0.0001} };
            cartstate = new double[] { reci[0], reci[1], reci[2], veci[0], veci[1], veci[2] };  // in km

            // --------convert to a classical orbit state
            AstroLibr.rv2coe(reci, veci, 
                out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out arglat, out truelon, out lonper);
            classstate[0] = a;   // km
            classstate[1] = ecc;
            classstate[2] = incl;
            classstate[3] = raan;
            classstate[4] = argp;
            if (anom.Contains("mean")) // meann or meana
                classstate[5] = m;
            else  // truea or truen
                classstate[5] = nu;

            // -------- convert to an equinoctial orbit state
            AstroLibr.rv2eq(reci, veci, out a, out n, out af, out ag, out chi, out psi, out meanlonM, out meanlonNu, out fr);
            if (anom.Equals("meana") || anom.Equals("truea"))
                eqstate[0] = a;  // km
            else // meann or truen
                eqstate[0] = n;
            eqstate[1] = af;
            eqstate[2] = ag;
            eqstate[3] = chi;
            eqstate[4] = psi;
            if (anom.Contains("mean")) //  meana or meann
                eqstate[5] = meanlonM;
            else // truea or truen
                eqstate[5] = meanlonNu;

            // --------convert to a flight orbit state
            AstroLibr.rv2flt(reci, veci, jdtt, jdttfrac, jdut1, jdxysstart, lod, xp, yp, terms, ddpsi, ddeps, ddx, ddy, 
                iau80arr, iau00arr,
                out lon, out latgc, out rtasc, out decl, out fpa, out az, out magr, out magv);
            if (anomflt.Equals("radec"))
            {
                fltstate[0] = rtasc;
                fltstate[1] = decl;
            }
            else
            if (anomflt.Equals("latlon"))
            {
                fltstate[0] = lon;
                fltstate[1] = latgc;
            }
            fltstate[2] = fpa;
            fltstate[3] = az;
            fltstate[4] = magr;  // km
            fltstate[5] = magv;

            // test position and velocity going back
            avec = new double[] { 0.0, 0.0, 0.0 };

            AstroLibr.eci_ecef(ref reci, ref veci, MathTimeLib.Edirection.eto, ref recef, ref vecef,
                 AstroLib.EOpt.e80, iau80arr, iau00arr,
                 jdtt, jdttfrac, jdut1, jdxysstart, lod, xp, yp, ddpsi, ddeps, ddx, ddy);
            //vx = magv* ( -cos(lon)*sin(latgc)*cos(az)*cos(fpa) - sin(lon)*sin(az)*cos(fpa) + cos(lon)*cos(latgc)*sin(fpa) ); 
            //vy = magv* ( -sin(lon)*sin(latgc)*cos(az)*cos(fpa) + cos(lon)*sin(az)*cos(fpa) + sin(lon)*cos(latgc)*sin(fpa) );  
            //vz = magv* (sin(latgc) * sin(fpa) + cos(latgc)*cos(az)*cos(fpa) );
            //// correct:
            //ve1 = magv* ( -cos(rtasc)*sin(decl)*cos(az)*cos(fpa) - sin(rtasc)*sin(az)*cos(fpa) + cos(rtasc)*cos(decl)*sin(fpa) ); // m/s
            // ve2 = magv* (-sin(rtasc) * sin(decl) * cos(az) * cos(fpa) + cos(rtasc) * sin(az) * cos(fpa) + sin(rtasc) * cos(decl) * sin(fpa));
            //ve3 = magv* (sin(decl) * sin(fpa) + cos(decl)*cos(az)*cos(fpa) );

            strbuild.AppendLine("==================== do the sensitivity tests \n");

            strbuild.AppendLine("1.  Cartesian Covariance \n");
            printcov(cartcov, "ct", 'm', anom, out strout);
            strbuild.AppendLine(strout);

            strbuild.AppendLine("3.  Equinoctial Covariance from Classical (Cartesian) #1 above (" + anom + ") ------------------- \n");
            AstroLibr.covct2cl(cartcov, cartstate, anom, out classcovmeana, out tmct2cl);
            AstroLibr.covcl2eq(classcovmeana, classstate, anom, anom, fr, out eqcovmeana, out tmcl2eq);

            printcov(eqcovmeana, "eq", 'm', anom, out strout);
            strbuild.AppendLine(strout);

            strbuild.AppendLine("  Cartesian Covariance from Classical #3 above \n");
            AstroLibr.coveq2cl(eqcovmeana, eqstate, anom, anom, fr, out classcovmeana, out tmeq2cl);
            printcov(classcovmeana, "cl", 'm', anom, out strout);
            strbuild.AppendLine(strout);

            AstroLibr.covcl2ct(classcovmeana, classstate, anom, out cartcovmeanarev, out tmcl2ct);
            printcov(cartcovmeanarev, "ct", 'm', anom, out strout);
            strbuild.AppendLine(strout);
            strbuild.AppendLine("\n");

            printdiff(" cartcov - cartcov" + anom + "rev \n", cartcov, cartcovmeanarev, out strout);
            strbuild.AppendLine(strout);
        }  // testcovcl2eq

        public void testcovct2eq(string anom)
        {
            int year, mon, day, hr, minute, timezone, dat, terms;
            double sec, dut1, ut1, tut1, jdut1, jdut1frac, utc, tai, tt, ttt, jdtt, jdttfrac, tdb, ttdb, jdtdb, jdtdbfrac;
            double p, a, n, ecc, incl, raan, argp, nu, m, arglat, truelon, lonper;
            double af, ag, chi, psi, meanlonNu, meanlonM;
            double latgc, lon, rtasc, decl, fpa, lod, xp, yp, ddpsi, ddeps, ddx, ddy, az, magr, magv;
            Int16 fr;
            double[,] tm = new double[,] { { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 } };
            string anomflt = "latlon"; // latlon  radec
            double[] cartstate = new double[6];
            double[] classstate = new double[6];
            double[] eqstate = new double[6];
            double[] fltstate = new double[6];
            double[] recef = new double[3];
            double[] vecef = new double[3];
            double[] avec = new double[3];

            double[,] classcovmeana = new double[6, 6];
            double[,] cartcovmeanarev = new double[6, 6];
            double[,] eqcovmeana = new double[6, 6];
            double[,] tmct2cl = new double[6, 6];
            double[,] tmcl2ct = new double[6, 6];
            double[,] tmct2eq = new double[6, 6];
            double[,] tmeq2ct = new double[6, 6];
            string strout;

            double[] reci = new double[3] { -605.79221660, -5870.22951108, 3493.05319896 };
            double[] veci = new double[3] { -1.56825429, -3.70234891, -6.47948395 };
            double[] aeci = new double[3] { 0.001, 0.002, 0.003 };

            // StringBuilder strbuild = new StringBuilder();
            // strbuild.Clear();

            EOPSPWLib.iau80Class iau80arr;
            EOPSPWLib.iau00Class iau00arr;
            string nutLoc;
            nutLoc = @"D:\Codes\LIBRARY\DataLib\nut80.dat";
            EOPSPWLibr.iau80in(nutLoc, out iau80arr);
            nutLoc = @"D:\Codes\LIBRARY\DataLib\";
            EOPSPWLibr.iau00in(nutLoc, out iau00arr);
            // now read it in
            double jdxysstart, jdfxysstart;
            AstroLib.xysdataClass[] xysarr = AstroLibr.xysarr;
            AstroLibr.initXYS(ref xysarr, nutLoc, "xysdata.dat", out jdxysstart, out jdfxysstart);

            year = 2000;
            mon = 12;
            day = 15;
            hr = 16;
            minute = 58;
            sec = 50.208;
            dut1 = 0.10597;
            dat = 32;
            timezone = 0;
            xp = 0.0;
            yp = 0.0;
            lod = 0.0;
            terms = 2;
            timezone = 0;
            ddpsi = 0.0;
            ddeps = 0.0;
            ddx = 0.0;
            ddy = 0.0;

            MathTimeLibr.convtime(year, mon, day, hr, minute, sec, timezone, dut1, dat,
                out ut1, out tut1, out jdut1, out jdut1frac, out utc, out tai,
                out tt, out ttt, out jdtt, out jdttfrac,
                out tdb, out ttdb, out jdtdb, out jdtdbfrac);

            // ---convert the eci state into the various other state formats(classical, equinoctial, etc)
            double[,] cartcov = new double[,] {
            { 100.0, 1.0e-2, 1.0e-2, 1.0e-4, 1.0e-4, 1.0e-4 },
            {1.0e-2, 100.0,  1.0e-2, 1.0e-4,   1.0e-4,   1.0e-4},
            {1.0e-2, 1.0e-2, 100.0,  1.0e-4,   1.0e-4,   1.0e-4},
            {1.0e-4, 1.0e-4, 1.0e-4, 0.0001,   1.0e-6,   1.0e-6},
            {1.0e-4, 1.0e-4, 1.0e-4, 1.0e-6,   0.0001,   1.0e-6},
            {1.0e-4, 1.0e-4, 1.0e-4, 1.0e-6,   1.0e-6,   0.0001} };
            cartstate = new double[] { reci[0], reci[1], reci[2], veci[0], veci[1], veci[2] };  // in km

            // --------convert to a classical orbit state
            AstroLibr.rv2coe(reci, veci, 
                out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out arglat, out truelon, out lonper);
            classstate[0] = a;   // km
            classstate[1] = ecc;
            classstate[2] = incl;
            classstate[3] = raan;
            classstate[4] = argp;
            if (anom.Contains("mean")) // meann or meana
                classstate[5] = m;
            else  // truea or truen
                classstate[5] = nu;

            // -------- convert to an equinoctial orbit state
            AstroLibr.rv2eq(reci, veci, out a, out n, out af, out ag, out chi, out psi, out meanlonM, out meanlonNu, out fr);
            if (anom.Equals("meana") || anom.Equals("truea"))
                eqstate[0] = a;  // km
            else // meann or truen
                eqstate[0] = n;
            eqstate[1] = af;
            eqstate[2] = ag;
            eqstate[3] = chi;
            eqstate[4] = psi;
            if (anom.Contains("mean")) //  meana or meann
                eqstate[5] = meanlonM;
            else // truea or truen
                eqstate[5] = meanlonNu;

            // --------convert to a flight orbit state
            AstroLibr.rv2flt(reci, veci, jdtt, jdttfrac, jdut1, jdxysstart, lod, xp, yp, terms, ddpsi, ddeps, ddx, ddy,
                iau80arr, iau00arr,
                out lon, out latgc, out rtasc, out decl, out fpa, out az, out magr, out magv);
            if (anomflt.Equals("radec"))
            {
                fltstate[0] = rtasc;
                fltstate[1] = decl;
            }
            else
            if (anomflt.Equals("latlon"))
            {
                fltstate[0] = lon;
                fltstate[1] = latgc;
            }
            fltstate[2] = fpa;
            fltstate[3] = az;
            fltstate[4] = magr;  // km
            fltstate[5] = magv;

            // test position and velocity going back
            avec = new double[] { 0.0, 0.0, 0.0 };

            AstroLibr.eci_ecef(ref reci, ref veci, MathTimeLib.Edirection.eto, ref recef, ref vecef,
                 AstroLib.EOpt.e80, iau80arr, iau00arr,
                 jdtt, jdttfrac, jdut1, jdxysstart, lod, xp, yp, ddpsi, ddeps, ddx, ddy);
            //vx = magv* ( -cos(lon)*sin(latgc)*cos(az)*cos(fpa) - sin(lon)*sin(az)*cos(fpa) + cos(lon)*cos(latgc)*sin(fpa) ); 
            //vy = magv* ( -sin(lon)*sin(latgc)*cos(az)*cos(fpa) + cos(lon)*sin(az)*cos(fpa) + sin(lon)*cos(latgc)*sin(fpa) );  
            //vz = magv* (sin(latgc) * sin(fpa) + cos(latgc)*cos(az)*cos(fpa) );
            //// correct:
            //ve1 = magv* ( -cos(rtasc)*sin(decl)*cos(az)*cos(fpa) - sin(rtasc)*sin(az)*cos(fpa) + cos(rtasc)*cos(decl)*sin(fpa) ); // m/s
            // ve2 = magv* (-sin(rtasc) * sin(decl) * cos(az) * cos(fpa) + cos(rtasc) * sin(az) * cos(fpa) + sin(rtasc) * cos(decl) * sin(fpa));
            //ve3 = magv* (sin(decl) * sin(fpa) + cos(decl)*cos(az)*cos(fpa) );

            strbuild.AppendLine("==================== do the sensitivity tests \n");

            strbuild.AppendLine("1.  Cartesian Covariance \n");
            printcov(cartcov, "ct", 'm', anom, out strout);
            strbuild.AppendLine(strout);

            strbuild.AppendLine("3.  Equinoctial Covariance from Cartesian #1 above (" + anom + ") ------------------- \n");
            AstroLibr.covct2eq(cartcov, cartstate, anom, fr, out eqcovmeana, out tmct2eq);

            printcov(eqcovmeana, "eq", 'm', anom, out strout);
            strbuild.AppendLine(strout);

            strbuild.AppendLine("  Cartesian Covariance from Classical #3 above \n");
            AstroLibr.coveq2ct(eqcovmeana, eqstate, anom, fr, out cartcovmeanarev, out tmeq2ct);

            printcov(cartcovmeanarev, "ct", 'm', anom, out strout);
            strbuild.AppendLine(strout);
            strbuild.AppendLine("\n");

            printdiff(" cartcov - cartcov" + anom + "rev \n", cartcov, cartcovmeanarev, out strout);
            strbuild.AppendLine(strout);
        }  // testcoveq2clmeann

     
        public void testcovct2fl(string anomflt)
        {
            int year, mon, day, hr, minute, timezone, dat, terms;
            double sec, dut1, ut1, tut1, jdut1, jdut1frac, utc, tai, tt, ttt, jdtt, jdttfrac, tdb, ttdb, jdtdb, jdtdbfrac;
            double p, a, n, ecc, incl, raan, argp, nu, m, arglat, truelon, lonper;
            double af, ag, chi, psi, meanlonNu, meanlonM;
            double latgc, lon, rtasc, decl, fpa, lod, xp, yp, ddpsi, ddeps, ddx, ddy, az, magr, magv;
            Int16 fr;
            double[,] tm = new double[,] { { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 } };
            string anom = "meann"; 
            double[] cartstate = new double[6];
            double[] classstate = new double[6];
            double[] eqstate = new double[6];
            double[] fltstate = new double[6];
            double[] recef = new double[3];
            double[] vecef = new double[3];
            double[] avec = new double[3];

            double[,] classcovmeana = new double[6, 6];
            double[,] cartcovmeanarev = new double[6, 6];
            double[,] fltcovmeana = new double[6, 6];
            double[,] tmct2cl = new double[6, 6];
            double[,] tmcl2ct = new double[6, 6];
            double[,] tmct2fl = new double[6, 6];
            double[,] tmfl2ct = new double[6, 6];
            string strout;

            double[] reci = new double[3] { -605.79221660, -5870.22951108, 3493.05319896 };
            double[] veci = new double[3] { -1.56825429, -3.70234891, -6.47948395 };
            double[] aeci = new double[3] { 0.001, 0.002, 0.003 };

            // StringBuilder strbuild = new StringBuilder();
            // strbuild.Clear();

            EOPSPWLib.iau80Class iau80arr;
            EOPSPWLib.iau00Class iau00arr;
            string nutLoc;
            nutLoc = @"D:\Codes\LIBRARY\DataLib\nut80.dat";
            EOPSPWLibr.iau80in(nutLoc, out iau80arr);
            nutLoc = @"D:\Codes\LIBRARY\DataLib\";
            EOPSPWLibr.iau00in(nutLoc, out iau00arr);
            // now read it in
            double jdxysstart, jdfxysstart;
            AstroLib.xysdataClass[] xysarr = AstroLibr.xysarr;
            AstroLibr.initXYS(ref xysarr, nutLoc, "xysdata.dat", out jdxysstart, out jdfxysstart);

            year = 2000;
            mon = 12;
            day = 15;
            hr = 16;
            minute = 58;
            sec = 50.208;
            dut1 = 0.10597;
            dat = 32;
            timezone = 0;
            xp = 0.0;
            yp = 0.0;
            lod = 0.0;
            terms = 2;
            timezone = 0;
            ddpsi = 0.0;
            ddeps = 0.0;
            ddx = 0.0;
            ddy = 0.0;

            MathTimeLibr.convtime(year, mon, day, hr, minute, sec, timezone, dut1, dat,
                out ut1, out tut1, out jdut1, out jdut1frac, out utc, out tai,
                out tt, out ttt, out jdtt, out jdttfrac,
                out tdb, out ttdb, out jdtdb, out jdtdbfrac);

            // ---convert the eci state into the various other state formats(classical, equinoctial, etc)
            double[,] cartcov = new double[,] {
            { 100.0, 1.0e-2, 1.0e-2, 1.0e-4, 1.0e-4, 1.0e-4 },
            {1.0e-2, 100.0,  1.0e-2, 1.0e-4,   1.0e-4,   1.0e-4},
            {1.0e-2, 1.0e-2, 100.0,  1.0e-4,   1.0e-4,   1.0e-4},
            {1.0e-4, 1.0e-4, 1.0e-4, 0.0001,   1.0e-6,   1.0e-6},
            {1.0e-4, 1.0e-4, 1.0e-4, 1.0e-6,   0.0001,   1.0e-6},
            {1.0e-4, 1.0e-4, 1.0e-4, 1.0e-6,   1.0e-6,   0.0001} };
            cartstate = new double[] { reci[0], reci[1], reci[2], veci[0], veci[1], veci[2] };  // in km

            // --------convert to a classical orbit state
            AstroLibr.rv2coe(reci, veci, 
                out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out arglat, out truelon, out lonper);
            classstate[0] = a;   // km
            classstate[1] = ecc;
            classstate[2] = incl;
            classstate[3] = raan;
            classstate[4] = argp;
            if (anom.Contains("mean")) // meann or meana
                classstate[5] = m;
            else  // truea or truen
                classstate[5] = nu;

            // -------- convert to an equinoctial orbit state
            AstroLibr.rv2eq(reci, veci, out a, out n, out af, out ag, out chi, out psi, out meanlonM, out meanlonNu, out fr);
            if (anom.Equals("meana") || anom.Equals("truea"))
                eqstate[0] = a;  // km
            else // meann or truen
                eqstate[0] = n;
            eqstate[1] = af;
            eqstate[2] = ag;
            eqstate[3] = chi;
            eqstate[4] = psi;
            if (anom.Contains("mean")) //  meana or meann
                eqstate[5] = meanlonM;
            else // truea or truen
                eqstate[5] = meanlonNu;

            // --------convert to a flight orbit state
            AstroLibr.rv2flt(reci, veci, jdtt, jdttfrac, jdut1, jdxysstart, lod, xp, yp, terms, ddpsi, ddeps, ddx, ddy,
                iau80arr, iau00arr,
                 out lon, out latgc, out rtasc, out decl, out fpa, out az, out magr, out magv);
            if (anomflt.Equals("radec"))
            {
                fltstate[0] = rtasc;
                fltstate[1] = decl;
            }
            else
            if (anomflt.Equals("latlon"))
            {
                fltstate[0] = lon;
                fltstate[1] = latgc;
            }
            fltstate[2] = fpa;
            fltstate[3] = az;
            fltstate[4] = magr;  // km
            fltstate[5] = magv;

            // test position and velocity going back
            avec = new double[] { 0.0, 0.0, 0.0 };

            AstroLibr.eci_ecef(ref reci, ref veci, MathTimeLib.Edirection.eto, ref recef, ref vecef,
                 AstroLib.EOpt.e80, iau80arr, iau00arr,
                 jdtt, jdttfrac, jdut1, jdxysstart, lod, xp, yp, ddpsi, ddeps, ddx, ddy);
            //vx = magv* ( -cos(lon)*sin(latgc)*cos(az)*cos(fpa) - sin(lon)*sin(az)*cos(fpa) + cos(lon)*cos(latgc)*sin(fpa) ); 
            //vy = magv* ( -sin(lon)*sin(latgc)*cos(az)*cos(fpa) + cos(lon)*sin(az)*cos(fpa) + sin(lon)*cos(latgc)*sin(fpa) );  
            //vz = magv* (sin(latgc) * sin(fpa) + cos(latgc)*cos(az)*cos(fpa) );
            //// correct:
            //ve1 = magv* ( -cos(rtasc)*sin(decl)*cos(az)*cos(fpa) - sin(rtasc)*sin(az)*cos(fpa) + cos(rtasc)*cos(decl)*sin(fpa) ); // m/s
            // ve2 = magv* (-sin(rtasc) * sin(decl) * cos(az) * cos(fpa) + cos(rtasc) * sin(az) * cos(fpa) + sin(rtasc) * cos(decl) * sin(fpa));
            //ve3 = magv* (sin(decl) * sin(fpa) + cos(decl)*cos(az)*cos(fpa) );

            strbuild.AppendLine("==================== do the sensitivity tests \n");

            strbuild.AppendLine("1.  Cartesian Covariance \n");
            printcov(cartcov, "ct", 'm', anomflt, out strout);
            strbuild.AppendLine(strout);

            strbuild.AppendLine("7.  Flight Covariance from Cartesian #1 above (" + anomflt + ") ------------------- \n");
            AstroLibr.covct2fl(cartcov, cartstate, anomflt, jdtt, jdttfrac, jdut1, jdxysstart,
                lod, xp, yp, 2, ddpsi, ddeps, ddx, ddy, 
                iau80arr, iau00arr, AstroLib.EOpt.e80, out fltcovmeana, out tmct2fl);

            if (anomflt.Equals("latlon"))
                printcov(fltcovmeana, "fl", 'm', anomflt, out strout);
            else
                printcov(fltcovmeana, "sp", 'm', anomflt, out strout);

            strbuild.AppendLine(strout);

            strbuild.AppendLine("  Cartesian Covariance from Flight #7 above \n");
            AstroLibr.covfl2ct(fltcovmeana, fltstate, anomflt, jdtt, jdttfrac, jdut1, jdxysstart,
                lod, xp, yp, 2, ddpsi, ddeps, ddx, ddy,
                iau80arr, iau00arr, AstroLib.EOpt.e80, out cartcovmeanarev, out tmfl2ct);

            printcov(cartcovmeanarev, "ct", 'm', anomflt, out strout);
            strbuild.AppendLine(strout);
            strbuild.AppendLine("\n");

            printdiff(" cartcov - cartcov" + anomflt + "rev \n", cartcov, cartcovmeanarev, out strout);
            strbuild.AppendLine(strout);
        }  // testcovct2fl





        // -----------------------------------------------------------------
        //
        // this file tests the various functions.
        //
        // companion code for
        // fundamentals of astrodyanmics and applications
        // 2019
        // by david vallado
        //
        // (w)719-573-2600, email dvallado@agi.com, davallado@gmail.com
        //
        // *****************************************************************
        //
        // current :
        // 24 jan 19  david vallado
        // original baseline
        //
        // *****************************************************************

        private void button4_Click_1(object sender, EventArgs e)
        {
            double rad = 180.0 / Math.PI;
            double rad2 = rad * rad;

            Int32 opt;

            this.opsStatus.Text = "Status: Find Test Solutions ";
            Refresh();

            //rj2000 = [-605.79221660; -5870.22951108; 3493.05319896;];
            //vj2000 = [-1.56825429; -3.70234891; -6.47948395; ];
            //aj2000 = [0.001; 0.002; 0.003];

            //end

            // -----------------------------------------------------------------
            // 74 angles-only tests
            // 80 lambert tests
            // 101 semianalytical (not built yet)
            //
            for (opt = 74; opt <= 74; opt++)  //101
            {
                strbuild.AppendLine("\n\n=================================== Case" + opt.ToString() + " =======================================");
                this.opsStatus.Text = "Status:  on case " + opt.ToString();
                Refresh();
                switch (opt)
                {
                    case 1:
                        testvecouter();
                        break;
                    case 2:
                        testmatadd();
                        break;
                    case 3:
                        testmatsub();
                        break;
                    case 4:
                        testmatmult();
                        break;
                    case 5:
                        testmattrans();
                        break;
                    case 6:
                        testmattransx();
                        break;
                    case 7:
                        testmatinverse();
                        break;
                    case 8:
                        testdeterminant();
                        break;
                    case 9:
                        testcholesky();
                        break;
                    case 10:
                        testposvelcov2pts();
                        break;
                    case 11:
                        testposcov2pts();
                        break;
                    case 12:
                        testremakecovpv();
                        break;
                    case 13:
                        testremakecovp();
                        break;
                    case 14:
                        testmatequal();
                        break;
                    case 15:
                        testmatscale();
                        break;
                    case 16:
                        testnorm();
                        break;
                    case 17:
                        testmag();
                        break;
                    case 18:
                        testcross();
                        break;
                    case 19:
                        testdot();
                        break;
                    case 20:
                        testangle();
                        break;
                    case 21:
                        testasinh();
                        break;
                    case 22:
                        testcot();
                        break;
                    case 23:
                        testacosh();
                        break;
                    case 24:
                        testaddvec();
                        break;
                    case 25:
                        testPercentile();
                        break;
                    case 26:
                        testrot1();
                        break;
                    case 27:
                        testrot2();
                        break;
                    case 28:
                        testrot3();
                        break;
                    case 29:
                        testfactorial();
                        break;
                    case 30:
                        testcubicspl();
                        break;
                    case 31:
                        testcubic();
                        break;
                    case 32:
                        testcubicinterp();
                        break;
                    case 33:
                        testquadratic();
                        break;
                    case 34:
                        testconvertMonth();
                        break;
                    case 35:
                        testjday();
                        break;
                    case 36:
                        testdays2mdhms();
                        break;
                    case 37:
                        testinvjday();
                        break;
                    case 38:
                        // tests eop, spw, and fk5 iau80
                        testiau80in();
                        break;
                    case 39:
                        testfundarg();
                        break;
                    case 40:
                        testprecess();
                        break;
                    case 41:
                        testnutation();
                        break;
                    case 42:
                        testnutationqmod();
                        break;
                    case 43:
                        testsidereal();
                        break;
                    case 44:
                        testpolarm();
                        break;
                    case 45:
                        testgstime();
                        break;
                    case 46:
                        testlstime();
                        break;
                    case 47:
                        testhms_sec();
                        break;
                    case 48:
                        testhms_ut();
                        break;
                    case 49:
                        testhms_rad();
                        break;
                    case 50:
                        testdms_rad();
                        break;
                    case 51:
                        testeci_ecef();
                        break;
                    case 52:
                        testtod2ecef();
                        break;
                    case 53:
                        testteme_ecef();
                        break;
                    case 54:
                        testteme_eci();
                        break;
                    case 55:
                        testqmod2ecef();
                        break;
                    case 56:
                        testcsm2efg();
                        break;
                    case 57:
                        testrv_elatlon();
                        break;
                    case 58:
                        testrv2radec();
                        break;
                    case 59:
                        testrv_razel();
                        break;
                    case 60:
                        testrv_tradec();
                        break;
                    case 61:
                        testrvsez_razel();
                        break;
                    case 62:
                        testrv2rsw();
                        break;
                    case 63:
                        testrv2pqw();
                        break;
                    case 64:
                        testrv2coe();
                        break;
                    case 65:
                        testcoe2rv();
                        break;
                    case 66:
                        testlon2nu();
                        break;
                    case 67:
                        testnewtonmx();
                        break;
                    case 68:
                        testnewtonm();
                        break;
                    case 69:
                        testnewtonnu();
                        break;
                    case 70:
                        testfindc2c3();
                        break;
                    case 71:
                        testfindfandg();
                        break;
                    case 72:
                        testcheckhitearth();
                        testcheckhitearthc();
                        break;
                    case 73:
                        testgibbs();
                        testhgibbs();
                        break;
                    case 74:
                        testangles();
                        //testanglestopology();
                        MHTangles();
                        break;
                    case 75:
                        testlambertumins();
                        testlambertminT();
                        break;
                    case 76:
                        testlambhodograph();
                        break;
                    case 77:
                        testlambertbattin();
                        break;
                    case 78:
                        testeq2rv();
                        break;
                    case 79:
                        testrv2eq();
                        break;
                    case 80:
                        // book example, simple
                        testlambertuniv();

                        // old approach? yes...
                        testAllMoving();

                        // envelope testing
                        testAll();

                        // known problem cases
                        testknowncases();
                        break;
                    case 81:
                        testradecgeo2azel();
                        break;
                    case 82:
                        testijk2ll();
                        break;
                    case 83:
                        testgd2gc();
                        break;
                    case 84:
                        testsite();
                        break;
                    case 85:
                        testsun();
                        break;
                    case 86:
                        testmoon();
                        break;
                    case 87:
                        testkepler();
                        break;
                    case 88:
                        // mean
                        testcovct2clmean();
                        break;
                    case 89:
                        // true
                        testcovct2cltrue();
                        break;
                    case 90:
                        testhill();
                        break;
                    case 91:
                        break;
                    case 92:
                        testcovcl2eq("truea");
                        testcovcl2eq("truen");
                        testcovcl2eq("meana");
                        testcovcl2eq("meann");
                        break;
                    case 93:
                        testcovct2eq("truea");
                        testcovct2eq("truen");
                        testcovct2eq("meana");
                        testcovct2eq("meann");
                        break;
                    case 94:
                        testcovct2fl("latlon");
                        testcovct2fl("radec");
                        break;
                    case 95:
                        break;
                    case 96:
                        testcovct2rsw();
                        break;
                    case 97:
                        testcovct2ntw();
                        break;
                    case 98:
                        testsunmoonjpl();
                        break;
                    case 99:
                        testkp2ap();
                        break;
                    case 100:
                        testproporbit();
                        break;
                    case 101:
                        //testsemianaly();
                        break;
                    case 102:
                        testazel2radec();
                        break;
                }
                
            } // for

            // write data out
            string directory = @"D:\Codes\LIBRARY\cs\TestAll\";
            File.WriteAllText(directory + "testall.out", strbuild.ToString());


            this.opsStatus.Text = "Done";
            Refresh();
            int pauseTime = 1000;
            System.Threading.Thread.Sleep(pauseTime);
            this.opsStatus.Text = "Status: ";

        }


    }
}
