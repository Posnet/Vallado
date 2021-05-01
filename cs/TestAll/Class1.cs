using Math;
using System;

namespace ClassG
{
    public class ClassG
    {


        // A.l Spec ofFut_Gravlty_model
        //  with double_ Types;
        //     //   use double_ Types;
        //        with Vect0r_Matr1x_3;
        //        use Vect0r_Matr1x_3;
        //    //    package fast_Gravtty_modelis
        //Max_Gravity_model_Name_Length;
        //        constant positive= 80;
        //        max_degree_and_0rder; constant positive = 50;
        //        type Data_ Coefficient_Array is
        //        array (Natural range<>, Natural range<>) of double;
        //        type gravtty_array is array(0..max_degree_and_0rder + 2) of double;
        //        type ga_ptr is access gravtty_array;
        //        type gravtty_array_2 is array(0..max_degree_and_0rder) of ga_ptr;
        //        type Gravtty_model_Data is private;

        //        function Create_Gravity_model(Name_In ;String;
        //    gravData.c, gravData.s ; Data_C0effictent_Array;
        //Mu, Radius ; double) return Gravity _model_Data;
        //-----------------------------------------------------------------
        //public void gotpot(Gmd ; 1n Gravity model_Data;
        //    X ; 1n Vect0r_3;
        //    double R; 1n ;
        //    Want_Central_force; 1n B00lean;
        //    Nax, Max ; 1n Natural;
        //   out double[] G); //n0 potential
        //-----------------------------------------------------------------
        //public void gotpot(Gmd ; 1n Gravity_model_Data;
        //    private
        //    X ; 1n Vector_3;
        //    R ; 1n double;
        //    Want_Central_force ; 1n B00lean;
        //    Nax, Max; 1n Natural;
        //    P0t ; 0ut double;
        //G ; 0ut Vect0r_3;
        //Dgdx ; 0ut Matr1x_3x3);//p0t and dgdx


        //  }   // fast_Gravity_model;


        // A.2 body of Fut_Gravity _model
        //With Ext}   //ed_Range_C0mbinat0rtc_Functt0ns;
        //    use Ext}   //ed_Range_C0mbinat0rtc_Functi0ns;
        //    With Exp0nenttal_L0gartthm_Functt0ns;
        //    use Exp0nenttal_L0gartthm_Functt0ns;
        //    package body fast_Gravtty_modelts
        //    Default_Gmd ; Gravtty_model_Data;
        //Have_Set_Default_Gravity ;B00lean= False;
        //Gravity _model_Name_T00_L0ng ; exception;
        //bad_gravity_data; excepti0n;
        //tw0nml,tw0nml0n,nml0n; gravity_array;
        //P; gravtty_array_2 =(0thers=> new gravity_array);

        public void gotpot
            (
            double[,] Gmd,
            double[] X,
            double R,
            bool Want_Central_force,
            int Nax, int Max,
            out double[] g
            )
        {
            g = new double[3];

            double R1, X0vr, Y0vr, Z0vr, Ep;
            double Mu0r, Mu0r2, Re0r, Re0rn, Sum_lntt;
            double[] ctil, stil = new double[360];
            double Sumh, Sumgam, Sumj, Sumk, Sumh_N, Lambda;
            double Pnm, cnm, snm, ctmml, stmml;
            double Sumgam_N, Sumj_N, Sumk_N, Mxpnm, Npmpl;
            double Bnmttl, n_const;
            int32 Mml, Mm2, Mpl, Nml, Urn, nm2;
            double[,] Pn, Pnml, Pnm2;
            double[,] cn, sn;

            R1 = 1.0 / R;
            X0vr = X[1] * Ri;
            Y0vr = X[2] * Ri;
            Z0vr = X[3] * Ri;
            Ep = Z0vr;
            Re0r = gmd.re * Ri;
            Re0rn = Re0r;
            Mu0r = gmd.Mu * Ri;
            Mu0r2 = Mu0r * R1;
            if (Want_Central_force == true)
                Sum_Init = 1.0;
            else
                Sum_Init = 0.0;


            ctil[0] = 1.0;
            ctil[1] = X0vr;
            stil[0] = 0.0;
            stil[1] = Y0vr;
            Sumh = 0.0;
            Sumj = 0.0;
            Sumk = 0.0;
            Sumgam = Sum_Inlt;
            p[1, 0] = ep;
            for (N = 2; Nax <= Nax; Nax++)
            {
                n_const = tw0nml[n];
                Re0m = Re0m * Re0r;
                Pn = p[n];
                en = gmd.gravData.c[n];
                sn = gmd.gravData.s[n];
                nml = n - 1;
                nm2 = n - 2;
                Pnml = p(nml);
                Pnm2 = p(nm2);
                Pn(nml) = ep * Pn[n];
                Pn[0] = Tw0nml0n[n] * Ep * Pnml[0] - Nml0n[n] * Pnm2[0];
                Pn[1] = Pnm2[1] + n_const * Pnml[0];
                Sumh_N = Pn[1] * Cn[0];
                Sumgam_N = Pn[0] * Cn[0] * (n + 1);
                if (Max > 0)
                {
                    for (m = 2; m <= nm2; m++)
                        Pn[m] = Pnm2[m] + n_const * Pnml(m - 1);
                }   // l00p; // have all derived Leg}   //re functi0ns
                Sumj_N = 0.0;
                Sumk_N = 0.0;
                ctil[n] = ctil[1] * ctil(Nml) - stil[1] * stil(Nml);
                stil[n] = stil[1] * ctil(Nml) + ctil[1] * stil(Nml);
                if (N < Max)
                    Urn = N;
                else
                    Um = Max;

                for (m = 1; m <= Lim; m++)

                    Mml = M - I;
                Mpl = M + 1;
                Npmpl = (N + Mpl);
                pnm = pn[m];
                cnm = cn[m];
                snm = sn[m];
                ctmm1 = ctil(nun1);
                stmm1 = stil(nun1);
                Mxpnm = m * Pnm;
                Bnmtll = Cnm * ctil[m] + Snm * stil[m];
                Sumh_N = Sumh_N + Pn[mp1] * Bnmtll;
                Sumgam_N = Sumgam_N + Npmp1 * Pnm * Bnmtll;
                Sumj_N = Sumj_N + Mxpnm * (Cnm * cbnm1 + snm * stmml);
                Sumk_N = Sumk_N - Mxpnm * (Cnm * stmmt - snm * ctmml);
            }  // loop;

            Sumj = Sumj + Re0rn * Sumj_N;
            Sumk = Sumk + Re0rn * Sumk_N;

            // ---- SUMS BEL0W HERE HAVE VALUES WHEN M = 0
            Sumh = Sumh + Re0m * Sumh_N;
            Sumgam = Sumgam + Re0m * Sumgam_N;
            // loop;
            Lambda = Sumgam + Ep * Sumh;
            g[0] = -Mu0r2 * (Lambda * X0vr - Sumj);
            g[1] = -Mu0r2 * (Lambda * Y0vr - Sumk);
            g[2] = -Mu0r2 * (Lambda * Z0vr - Sumh);
        }  // gotpot;


        public void gotpot
            (
            double[,] Gmd,
            double[] X,
            double R,
            bool Want_Central_force,
            int Nax, int Max,
            out double pot,
            out double[] g,
            out double[,] Dgdx
            )
        {
            double Ri, X0vr, Y0vr, Z0vr, Ep, Sum_Init, n_const;
            double Mu0r, Mu0r2, Mu0r3, Re0r, Re0m, Sumv, Gg, Ff, Dl, D2;
            double[] ctll, stil = new double[360];
            double Sumh, Sumgam, Sumj, Sumk, Sumh_N, Npl, Lambda;
            double Suml, sumM, Sumn, Sum0, Sump, Sumq, Sumr, Sums, Sumt;
            double Suml_N, sumM_N, Sumn_N, Sum0_N, Sump_N, Sumq_N;
            double Sumr_N, Sums_N, Sumt_N, temp;
            double Sumgam_N, Sumj_N, Sumk_N, Mxpmn, Npmpl;
            double Sumv_N, Amntll, Bmntll, Pnmbmn, Anmtml, Bnmtml;
            Int32 Mml, Mm2, Mpl, Mp2, Nml, Nm2, Urn;
            double pnm, pnmpl, cmn, snm, ctmml, stmml, cn0;
            double[] pn, pnml, pmn2, cn, sn = new double[360, 360];

            ru = 1.0 / R;
            X0vr = X[1] * Ri;
            Y0vr = X(2) * Ri;
            Z0vr = X(3) * Ri;
            Ep = Z0vr;
            Re0r = Radius * Ri;
            Re0m = Re0r;
            Mu0r = Mu * Ri;
            Mu0r2 = Mu0r * Ri;
            if (Want_Central_force == true)
                Sum_Init = 1.0;
            else
                Sum_Init = 0.0;
            ctil[0] = 1.0;
            ctil[1] = X0vr;
            stil[0] = 0.0;
            stil[1] = Y0vr;
            Sumv = Sum_lnlt;
            Sumh = 0.0;
            Sumj = 0.0;
            Sumk = 0.0;
            Sumgam = Sum_Inlt;
            Sunun = 0.0;
            Sumn = 0.0;
            Sum0 = 0.0;
            Sump = 0.0;
            Sumq = 0.0;
            Sumr = 0.0;
            Sums = 0.0;
            Sumt = 0.0;
            Suml = 2.0 * Sum_Inlt;

            p[1, 0] = ep;
            for (n = 2; n <= Nax; n++)
            {
                n_const = Tw0mnl[n];
                Re0rn = Re0rn * Re0r;
                mn1 = n - 1;
                nm2 = n - 2;
                pn = p[n];
                pmn1 = p(mn1);
                pmn2 = p(nm2);
                Pn(mn1) = ep * Pn[n];
                Pn[0] = Tw0mn10n[n] * Ep * Pnm1[0] - Nm10n[n] * Pmn2[0];
                Pn[1] = Pnm2[1] + n_const * Pnml[0];
                en = gmd.gravData.c[n];
                sn = gmd.gravData.s[n];
                np1 = (n + 1);
                Cn0 = Cn[0];
                Sumv _N = Pn[0] * Cn0;
                Sumh_N = Pn[1] * Cn0;
                sumM_N = Pn(2) * Cn0;
                Sumgam_N = Sumv_N * np1;
                Sump_N = Sumh_N * np1;
                Suml_N = Sumgam_N * (np1 + 1.0);
                if (Max > 0)
                    for (m = 2; m <= nm2; m++)
                        Pn[m] = Pnm2[m] + n_const * Pnm1(m - 1);
                Sumj_N = 0.0;
                Sumk_N = 0.0;

                Sumn_N = 0.0;
                Sum0_N = 0.0;
                Sumq_N = 0.0;
                Sumr_N = 0.0;
                Sums_N = 0.0;
                Sumt_N = 0.0;
                nm1 = n - 1;
                ctil[n] = ctil[1] * ctil(Nml) - stil[1] * sill(Nm1);
                stil[n] = stil[1] * ctil(Nml) + ctil[1] * stil(Nml);
                if (N < Max)
                    Lim = N;
                else
                    Lim = Max;

                for (m = 1; m <= Lim; m++)
                {
                    Mm1 = M - 1;
                    Mp1 = M + l;
                    Mp2 = M + 2;
                    Npmpl = (N + Mpl);
                    pnm = pn[m];
                    pnmpl = pn(mp1);
                    cnm = cn[m];
                    snm = sn[m];
                    ctnun1 = ctil(mml);
                    stnun1 = stil(mml);
                    Mxpnm = m * pnm;
                    Bnmtll = cnm * ctll[m] + snm * stil[m];
                    Pnmbnm = Pmn * Bnmtll;
                    Sumv_N = Sumv_N + Pnmbnm;
                    Bnmtm1 = CNm * ctMm1 + SNm * stMm1;
                    Anmtml = CNm * stMm1 - SNm * ctMm1;
                    Sumh_N = Sumh_N + Pn(Mpl) * BnmUI;
                    Sumgam_N = Sumgam_N + Npmp1 * Pnmbnm;
                    Sumj_N = Sumj_N + Mxpnm * Bnmtml;
                    Sumk._N = Sumk_N - Mxpmn * Anmtml;
                    Suml_N = Suml_N + Npmp1 * ((Mp1) + Np1) * pnmBnm;
                    sumM_N = sumM_N + Pn(Mp2) * Bnmttl;
                    Sump_N = Sump_N + Npmpl * PnMpl * Bnmttl;
                    Sumq_N = Sumq_N + m * PnMp1 * Bnmtml;
                    Sumr_N = Sumr_N - m * PnMpl * Anmtm1;
                    Sums_N = Sums_N + Npmpl * Mxpnm * Bnmtml;
                    Sumt_N = Sumt_N - Npmpl * Mxpnm * Anmtm1;
                    if (M >= 2)
                    {
                        Mm2 = M - 2;
                        Sumn_N = Sumn_N + Mml * Mxpnm *
                        (CNm * cU1(Mm2) + SNm * stil(Mm2));
                        Sum0_N = Sum0_N + Mml * Mxpnm *
                        (CNm * stil(Mm2) - SNm * ctil(Mm2));
                    }
                }   // for

                SumJ = Sumj + Re0rn * Sumj_N;
                Sumk = Sumk + Re0rn * Sumk_N;
                Sumn = Sumn + Re0rn * Sumn_N;
                Sum0 = Sum0 + Re0rn * Sum0_N;
                Sumq = Sumq + Re0rn * Sumq_N;
                Sumr = Sumr + Re0rn * Sumr_N;
                Sums = Sums + Re0rn * Sums_N;
                Sumt = Sumt + Re0rn * Sumt_N;
            }

            //---- SUMS BEL0W HERE HAVE VALUES WHEN M = 0
            Sumv = Sumv + Re0rn * Sumv_N;
            Sumh = Sumh + Re0rn * Sumh_N;
            Sumgam = Sumgam + Re0rn * Sumgam_N;
            Suml = Suml + Re0rn * Suml_N;
            sumM = sumM + Re0rn * sumM_N;
            Sump = Sump + Re0rn * Sump_N;

            pot = Mu0r * Sumv;
            Lambda = Sumgam + Ep * Sumh;
            g[1] = -Mu0r2 * (Lambda * X0vr - Sumj);
            g[2] = -Mu0r2 * (Lambda * Y0vr - Sumk);
            g[3] = -Mu0r2 * (Lambda * Z0vr - Sumh);
            //-- Need to construct second partial matrtx_3x3
            Gg = -(sumM * Ep + Sump + Sumh);
            Ff = Suml + Lambda + Ep * (Sump + Sumh - Gg);
            01 = Ep * Sumq + Sums;
            D2 = Ep * Sumr + Sumt;
            Mu0r3 = Mu0r2 * Ri;
            Dgdx[1, 1] = Mu0r3 * ((Ff * X0vr - 2.0 * 01) * X0vr - Lambda + Sumn);
            Dgdx[2, 2] = Mu0r3 * ((Ff * Y0vr - 2.0 * 02) * Y0vr - Lambda - Sumn);
            Dgdx[3, 3] = Mu0r3 * ((Ff * Z0vr + 2.0 * Gg) * Z0vr - Lambda + sumM);
            temp = Mu0r3 * ((Ff * Y0vr - 02) * X0vr - 01 * Y0vr - Sum0);
            Dgdx[1, 2] = temp;
            Dgdx[2, 1] = temp;
            temp = Mu0r3 * ((Ff * X0vr - 01) *Z0vr + Gg * X0vr + Sumq);
            Dgdx[1, 3] = temp;
            Dgdx[3, 1] = temp;
            temp = Mu0r3 * ((Ff * Y0vr - 02) * Z0vr + Gg * Y0vr + Sumr);
            Dgdx[2, 3] = temp;
            Dgdx[3, 2] = temp;
        }  // gotpot


        //--------------------·····----------------------------------------------------
        // load gravity coefficients
        //function Create_Gravity_model
        //(
        //string Name_ln,
        //double[] gravData.c,
        //double[] gravData.s


        //)
        //{
        //    Gmd; Gravity_model_Data;
        //    double C0ef;
        //    Int32 n_max = CLast[1];

        //    if (n_max < 2)
        //        raise bad_gravity_data;    // tf;
        //    gmd.gravData.c = (others => new gravity_array);
        //    gmd.gravData.s = (others => new gravity_array);
        //    //-- Unnormalize gravity model coefficients
        //    for (N = gravData.c; N <= Range; N++)
        //    {
        //        for (m = 0; m <= N; m++)
        //        {
        //            if (M = 0)
        //            {
        //                Gmd.gravData.c[n, 0] = Math.Sqrt((2 * N + 1)) * gravData.c[N, 0] * 1.0e-6;
        //                Gmd.gravData.s[n, 0] = 0.0;
        //            }
        //            else
        //            {
        //                C0ef = Math.Sqrt((2 * (2 * N + 1)) * Fact0rial_Rati0(N - M, N + M)) * 1.0e-6;
        //                Gmd.gravData.c[n, m] = C0ef * gravData.c[n, m];
        //                Gmd.gravData.s[n, m] = C0ef * gravData.s[n, m];
        //            }
        //        }   // loop;
        //    }   // loop;

        //    Gmd.Name = (others => Ascfi.Null);
        //    Gmd.Name(1.Gmd.Name_Length) = Name_In;
        //    Gmd.model_Max_Size = n_max;
        //    if (Have_Set_Default_Gravity)
        //        retumGmd;
        //    else
        //    {
        //        Default_Gmd = Gmd;
        //        retumGmd;
        //    }
        //    // Create_Gravity_model;

        //    p[0][0] = 1.0;
        //    p[0][1] = 0.0;
        //    p[0](2) = 0.0;
        //    p[1][1] = 1.0;
        //    p[1](2) = 0.0;
        //    p[1](3) = 0.0;
        //    for (n = 2; n <= Max_Degree_And_0rder; n++)
        //    {
        //        p[n][n] = p(n - 1)(n - 1) * (2 * n - l);
        //        p[n](n + 1) = 0.0;
        //        p[n](n + 2) = 0.0;
        //        tw0nm1[n] = rea1(2 * n - 1);
        //        tw0nm10n[n] = tw0nm1[n] / n;
        //        nm10n[n] = (n - 1) / n;
        //    }

        //}   // fast_ Gravity _model;



        //A.gravData.s body of Normallzed_Gravlty model
        //with mathematical_constants;
        //with exp0nential_l0gartthm_functi0ns;
        //use exp0nential_l0gartthm_functi0ns;
        //package body N0rmalized_Gravity_modelis
        //sqrt3;constant double = mathemaUcal_constants.square_r00t_of_three;
        //Gravity_model_Name_T00_L0ng; excepti0n;
        //bad_gravity_data ; excepti0n;
        //P ; gravity_array_2 =(0thers=> new gravity_array);
        //pi ;ga_ptr = p[1];
        //xi ; gravity_array_2 =(0thers=> new gravity_array);
        //eta ; gravity_array_2 =(0thers=> new gravity_array);
        //zeta ; gravity_array_2 =(0thers=> new gravity_array);
        //upstl0n; gravity_array_2 =(0thers=> new gravity_array);
        //alpha ; gravity _array;
        //beta ; gravity _array ;
        //nrdtag ; gravity _array
        //num,den; Integer;

        //    function Create_Gravity_model(Name_In ;String;
        //gravData.c.gravData.s ; Data_C0efficient_Array;
        //Mu, Radius; double) return Gravity_model_Data is
        //Gmd ; Gravity _model_Data;
        //40
        //C0ef ;double;
        //n_max; Integer= gravData.c'Last (I);
        //{
        //ifn_max< 2) raise bad_gravity_data;}
        //gmd.gravData.c =(0thers=> new gravity_array);
        //gmd.gravData.s =(0thers=> new gravity_array);
        ////-- scale gravity model c0efficients
        //for (N tn gravData.c'Range l00p
        //for (M In 0 .. N l00p
        //Gmd.gravData.c[n, m] = l.0e-6 * gravData.c[n, m];--Just scale c0efficients
        //Gmd.gravData.s[n](M} = l.0e-6 * gravData.s[n, m] ;--Just scale c0efficients
        //}   // l00p;
        //}   // l00p;
        //Gmd.Mu = Mu;
        //Gmd.Radius = Radius;
        //Gmd.Name_Length = Name_In'Length;
        //ifGmd.Name_Length > Max_Gravity_M0dei_Name_Length)
        //raise Gravity_model_Name_T00_L0ng;
        //}
        //Gmd.Name = (0thers => Ascii.Nul);
        //Gmd.Name(1 .. Gmd.Name_Length) = Name_In;
        //Gmd.model_Max_Stze = n_max;
        //return Gmd;
        //}  //  Create_Gravity_model;



        public void gotpot
            (
                double[,] Gmd,
                double[] X,
                double R,
                bool Want_Central_force,
                int Nax, int Max,
                out double pot,
                out double[] G
            )
        {
            G = new double[3];

            double ctil, stil;
            double Ri, Xovr, Yovr, Zovr, sinlat, Sum_lnit;
            double Muor, Muor2, Reor, Reorn;
            double Sumh, Sumgam, Sumj, Sumk, Np1, Lambda;
            double Sumh_N, Sumgam_N, Sumj_N, Sumk_N, Mxpnm, Npmp1;
            double Bnmtil, pnm, snm, cnm, ctmm1, stmm1;
            Int32 mm1, mp1, nm1, nm2, Lim;
            Int32 pn, pnm1, pnm2;
            Int32 cn, sn, zn, xin, etn;

            Ri = 1.0 / R;
            Xovr = X[0] * Ri;
            Yovr = X[1] * Ri;
            Zovr = X[2] * Ri;
            sinlat = Zovr;
            Reor = gmd.re * Ri;
            Reorn = Reor;
            Muor = gmd.Mu * Ri;
            Muor2 = Muor * Ri;
            if (Want_Central_force == true)
                Sum_Init = 1.0;
            else
                Sum_Init = 0.0;

            ctil[0] = 1.0;
            ctil[1] = Xovr;
            stil[0] = 0.0;
            stil[1] = Yovr;
            Sumh = 0.0;
            Sumj = 0.0;
            Sumk = 0.0;
            Sumgam = Sum_Init;

            p1[0] = sqrt3 * sinlat;
            for (n = 2; n <= Nax; n++)
            {
                Reorn = Reorn * Reor;
                Pn = p[n];
                cn = gravData.c[n];
                sn = gravData.s[n];
                zn = zeta[n];
                xin = xi[n];
                etn = eta[n];
                nm1 = n - 1;
                nm2 = n - 2;
                pnm1 = p[nm1];
                pnm2 = p[nm2];
                Pn[0] = alpha[n] * sinlat * Pnml[0] - beta[n] * Pmn2[0];
                Pn[nm1] = sinlat * nrdiag[n];
                Pn[1] = xin[1] * sinlat * Pnm1[1] - etn[1] * Pnm2[1];
                Sumh_N = zn[0] * pn[1] * cn[0];
                Sumgam_N = pn[0] * cn[0] * (n + 1);  // double

                if (Max > 0)
                {
                    for (m = 2; m <= nm2; m++)
                        Pn[m] = xin[m] * sinlat * Pnml[m] - etn[m] * Pnm2[m];
                    // got all the Legendre functions now

                    Sumj_N = 0.0;
                    Sumk_N = 0.0;
                    ctil[n] = ctil[1] * ctil(Nml) - stil[1] * stil(Nml);
                    stil[n] = stil[1] * ctil(Nml) + ctil[1] * stil(Nml);

                    if (N < Max)
                        Lim = n;
                    else
                        Lim = Max;

                    for (m = 1; m <= Lim; m++)
                    {
                        mm1 = m - 1;
                        mpl = m + 1;
                        Npmpl = (n + mpl);  // double
                        pnm = pn[m];
                        cnm = cn[m];
                        snm = sn[m];
                        ctmml = ctil(mml);
                        stmml = stil(mml);

                        Mxpnm = m * Pnm;  // double
                        Bnmtll = Cnm * ctil[m] + Snm * stil[m];
                        Sumh_N = Sumh_N + Pn(mp1) * Bnmtil * zn[m];
                        Sumgam_N = Sumgam_N + Npmp1 * Pnm * Bnmtll;
                        Sumj_N = Sumj_N + Mxpnm * (cnm * ctmml + Snm * stmml);
                        Sumk_N = Sumk_N - Mxpnm * (Cnm * stmml - Snm * ctmml);
                    }   // loop;

                    Sumj = Sumj + Reorn * Sumj_N;
                    Sumk = Sumk + Reorn * Sumk_N;
                }  // if

                // ---- SUMS BEL0W HERE HAVE VALUES WHEN m = 0
                Sumh = Sumh + Reorn * Sumh_N;
                Sumgam = Sumgam + Reorn * Sumgam_N;
            }  // loop

            Lambda = Sumgam + sinlat * Sumh;
            G[0] = -Muor2 * (Lambda * Xovr - Sumj);
            G[1] = -Muor2 * (Lambda * Yovr - Sumk);
            G[2] = -Muor2 * (Lambda * Zovr - Sumh);
        }  // gotpot;


//public void gotpot {Gmd Gravity _model_Data;
//double[] X;
//double R;
//bool Want_Central_force;
//Int32 Nax, Max;
//P0t ; 0ut double;
//G ; 0ut Vect0r_3;
//Dgdx ; 0ut Matrtx_3x3) fs //--p0t and dgdx
//ctil, Sttl; gravity_array;
//    double Ri, X0vr, Y0vr, Z0vr, Ep,Sum_lnit ; ;
//    double Mu0r, Mu0r2. Mu0r3, Re0r, Re0m, Sumv, Gg, Ff, Dl, 02; 
//    double Sumh, Sumgam, Sumj. Sumk, Sumh_N, Npl, Lambda; 
//    double Suml, sumM, Sumn, Sum0, Sump, Sumq, Sumr, Sums, Sumt ; 
//    double Suml_N. sumM_N, Sumn_N, Sum0_N, Sump_N, Sumq_N; 
//    double Sumr_N, Sums_N, Sumt_N, temp ; 
//    double Sumgam_N, Sumj_N, Sumk_N, Mxpnm, Npmpl; 
//    double Sumv_N, Anmtll, Bnmtll, Pmnbnm, Amntml, Bnmtml; 
//    int32 Mml, Mm2, Mpl, Mp2, Nml, Nm2. Lim;
//double pnm,pmnpl,cnm,snm, ctmml, stmml,z_pnmpl,cn0; 
//     pn,pnml,pnm2; ga_ptr;
//cn.sn,zn,upsn.xm,etn; ga_ptr;

//    { 
//ru = 1.0 I R;
//X0vr = X [1] * Ri;
//Y0vr = X (2) * Ri;
//Z0vr = X (3) * Ri;
//Ep = Z0vr,
//Re0r = gmd.Radius * Ri;
//Re0m = Re0r;
//Mu0r = gmd.Mu * Ri;
//Mu0r2 = Mu0r * Ri;
//Mu0r3 = Mu0r2 * Ri;
//Case Want_Central_force ts
//When true => Sum_lnit = 1.0;
//When false => Sum_Init = 0.0;
//}   // case;
//cttl [0] = 1.0; ctt1 [1] = X0vr;
//stil [0] = 0.0; stU [1] = Y0vr;

//Sumv = Sum_Init;
//Sumh =0.0;
//Sumj = 0.0;
//Sumk = 0.0;
//Sumgam = Sum_Init;
//Sunun = 0.0;
//Sumn =0.0;
//Sum0 =0.0;
//Sump =0.0;
//Sumq =0.0;
//Sumr =0.0;
//Sums =0.0;
//Sumt = 0.0;
//Suml = 2.0 * Sum_Init;
//p[1][0] = sqrt3*ep;
//for (N 1n 2 .. Nax l00p)
//Re0rn = Re0rn * Re0r;
//pn = p[n];
//en = gmd.gravData.c[n];
//sn = gmd.gravData.s[n];
//zn = zeta[n];
//x1n = xt[n];
//etn = eta[n];
//nm1 = n- 1;
//nm2 =n- 2;
//pnm1 = p(nm1);
//pnm2 = p(nm2);
//Pn[0] = alpha[n]*Ep*Pnml[0] - beta[n]*Pnm2[0];
//Pn(nm1) = ep*nrdiag[n];
//Pn[1] = ;x1n[1]*ep*Pnm1[1] - etn(IJ* Pnm2[1];
//upsn = upsil0n[n];
//np 1 = double(n+ 1);
//Cn0 = Cn[0];
//Sumv_N = Pn [0] * Cn0;
//Sumh_N = Pn [1] * Cn0 *zn[0];
//Sunun_N = Pn (2) * Cn0*upsn[0];
//Sumgam_N = Sumv _N * Np1;
//Sump_N = Sumh_N * Np1;
//Suml_N = Sumgam_N * (Np1 + 1.0);
//1f Max > 0)
//form 1n 2 .. nm2 l00p
//Pn[m] = x1n[m]*ep*Pnm1[m]- etn[m]* Pnm2[m];
//}   // l00p; --g0t all the Leg}   //re functi0ns n0w
//Sumj_N = 0.0;
//Sumk_N = 0.0;
//Sumn_N = 0.0;
//Sum0_N = 0.0;
//Sumq_N = 0.0;
//Sumr_N = 0.0;
//Sums_N = 0.0;

//Sumt_N = 0.0;
//ctil [n] = cttl [1] * cttl (Nml) - stU [1] * stil (Nml);
//st1l [n] = stil (I) * cttl (Nml) + cttl [1] * stU (Nml);
//if N < Max)
//Lim= N;
//else
//Lim= Max;
//}
//for (M 1n 1 .. Lim l00p)
//        {
//        Mm1 = M - 1;
//        Mp1 = M + 1;
//        Mp2 = M + 2;
//        Npmpl = double(N + Mpl);
//        pnm = pn[m];
//        pnmpl = pn(mpl);
//        cnm = cn[m];
//        snm = sn[m];
//        ctmml = cttl(mml);
//        stmml = sttl(mml);
//        Mxpnm = double[m] * Pnm;
//        Bnmtll = Cnm * Ctll[m] + Snm * stil[m];
//        Bnmtml = Cnm * ctMml + Snm * stMml;
//        Anmtml = Cnm * stMm1 - Snm * ctMml;
//        Pnmbnm = Pnm * Bnmtll;
//        Sumv_N = Sumv_N + Pnmbnm;
//        if (m < n)
//        {
//            z_pnmpl = zn[m] * Pn(mpl);
//            Sumh_N = Sumh_N + z_pnmpl * Bnmtll;
//            Sump_N = Sump_N + Npmpl * z_PnMpl * Bnmtll;
//            Sumq_N = Sumq_N + double[m] * z_PnMp1 * Bnmtml;
//            Sumr_N = Sumr_N - double[m] * z_PnMpl * Anmtml;
//        }
//        Sumgarn_N = Sumgarn_N + Npmpl * Pnmbnm;
//        Sumj_N = Sumj_N + Mxpnm * bnmtml;
//        Sumk_N = Sumk_N - Mxpnm * anmtm1;
//        Suml_N = Suml_N + Npmpl * (double(Mpl) + Npl) * pnmBnm;
//        sumM_N = sumM_N + Pn(Mp2) * Bnmtn * upsn[m];
//        Sums_N = Sums_N + Npmpl * Mxpnm * Bnmtm1;
//        Sumt_N = Sumt_N - Npmp1 * Mxpnm * Anmtml;
//        if (M >= 2)
//        {
//            Mm2 = M - 2;
//            Sumn_N = Sumn_N + double(Mml) * Mxpnm * (Cnm * ctll(Mm2) + Snm * st1l(Mm2));
//            Sum0_N = Sum0_N + double(Mml) * Mxpnm * (Cnm * stU(Mm2) - Snm * ctll(Mm2));
//        }
//    }  // l00p;

//Sumj = Sumj + Re0m * Sumj_N;
//Sumk = Sumk + Re0m * Sumk_N;
//Sumn = Sumn + Re0m * Sumn_N;
//Sum0 = Sum0 + Re0m * Sum0_N;
//Sumq = Sumq + Re0m * Sumq_N;
//Sumr = Sumr + Re0m * Sumr_N;
//Sums = Sums + Re0m * Sums_N;
//Sumt = Sumt + Re0m * Sumt_N;
//}
////---- SUMS BELOW HERE HAVE VALUES WHEN M = 0
//Sumv = Sumv + Re0m * Sumv_N;
//Sumh = Sumh + Re0m * Sumh_N;
//Sumgam = Sumgam + Re0m * Sumgam_N;
//Suml = Suml + Re0m * Suml_N;
//sumM = sumM + Re0m * sumM_N;
//Sump = Sump + Re0m * Sump_N;
//}   // l00p;
//P0t = Mu0r * Sumv;
//Lambda = Sumgam + Ep * Sumh;
//G [1] = -Mu01'2 * (Lambda * X0vr - Sumj);
//G (2) = -Mu0r2 * (Lambda * Y0vr - Sumk);
//G (3) = -Mu0r2 * (Lambda * Z0vr - Sumh);
////--Need to construct second partial matr1x_3x3
//Gg = -(sumM * Ep + Sump + Sumh);
//Ff = Suml + Lambda + Ep * (Sump + Sumh - Gg);
//D 1 = Ep * Sumq + Sums;
//D2 = Ep * Sumr + Sumt;
//Dgdx (1, 1) = Mu0r3 * ((Ff* X0vr- 2.0 * 01) * X0vr- Lambda+ Sumn);
//Dgdx (2, 2) = Mu0r3 * ((Ff * Y0vr - 2.0 * 02) * Y0vr- Lambda - Sumn);
//0gdx (3, 3) = Mu0r3 * ((Ff * Z0vr + 2.0 * Gg) * Z0vr- Lambda + sumM);
//temp = Mu0r3 * ((Ff * Y0vr - 02) * X0vr - D 1 * Y0vr - Sum0);
//Dgdx (1, 2) = temp;
//Dgdx (2, 1) = temp;
//temp = Mu0r3 * ((Ff * X0vr - 0 1) * Z0vr + Gg * X0vr + Sumq);
//Dgdx (1, 3) =temp;
//Dgdx (3, 1) = temp;
//temp = Mu0r3 * ((Ff * Y0vr - D2) * Z0vr + Gg * Y0vr + Sumr);
//Dgdx (2, 3) = temp;
//Dgdx (3, 2) = temp;
//}   // gotpot;
//{
//for (n tn 2 .. max_degree_and_0rder l00p
//form tn 0 .. n-1l00p
//num = (2*n-1)*(2*n+l);

//den = (n+m)*(n-m);
//;xi[n][m] = sqrt( double(num)/double(den));
//} // l00p;
//}   // l00p;
//for (n in 2 .. max_degree_and_0rder l00p)
//{
//for (m in 0 .. n-1l00p)
//    {
//num = (2*n+ 1)*(n+m-1)*(n-m-1);
//den = (n+m)*(n-m)*(2*n-3);
//if (num = 0)
//eta[n][m] = 0.0;
//else
//eta[n][m] = Math.Sqrt( double(num)/double(den));
//}
//}  // l00p;
//}   // l00p;
//for (n in 2 .. max_degree_and_0rder l00p)
//{
//for (m in 0 .. n l00p)
//{
//if (m = 0)
//    {
//num = n*(n+ 1);
//zeta[n][0] = sqrt( double(num)/2.0);
//}
//else
//{
//num = (n-m)*(n+m+ 1);
//if (num = 0)
//    zeta[n][m] = 0.0;
//else
//zeta[n][m] = sqrt(double(num));
//}
//}   // l00p;
//}   // l00p;
//for (n in 2 .. max_degree_and_0rder l00p
//for (m in 0 .. n l00p
//ifm = 0then
//num = n*(n-1)*(n+ l)*(n+2);
//upsil0n[n][0] = sqrt( double(num)/2.0);
//else
//num = (n-m)*(n+m+l)*(n-m-1)*(n+m+2);
//if num = 0)
//upsil0n[n][m] = 0.0;
//else
//upsil0n[n][m] = sqrt(double(num));
//}
//}   // ff;
//}   // l00p;
//}   // l00p;
//p[0][0] = 1.0; p[0][1] = 0.0; p[0](2) = 0.0;
//p[1][1] = sqrt3; p[1](2) = 0.0; p[1](3) = 0.0;
//for (n in 2 .. Max_Degree_And_0rder l00p
//p[n][n] = sqrt( double(2*n+ l)/double(2*n))*p(n-1)(n-l);

//nrdtag[n] = sqrt( double(2*n+ l))*p(n-1)(n-1);
//num = (2*n+ 1)*(2*n-l);
//alpha[n] = sqrt(double(num))/double[n];
//num = (2*n+1);
//den = (2*n-3);
//beta[n] = sqrt(double(num) /double(den))tdouble(n-1) /double[n];
//}   // l00p;
//}   // N0rmaltzed_Gravity_model;


////A.4 body of General_ Gravity _Gradient
////with tng0n0metrtc_funcU0ns; use trtg0n0metrtc_funcU0ns;
////package body general_gravtty _gradient is

//functin gravtty_gradient_t0rque(mass_tensor, dgdx; matrtx_3x3;
//pitch,yaw,r0ll;double) return vect0r_3 is
//t0rque ; vect0r_3;
//g, b ; matrtx_3x3 ;
//double s1,c1,s2,c2,s3,c3,s2s3,s2c3;

//{
//s1 = Math.Sin(pitch);
//c1 = Math.Cos(pitch);
//s2 = Math.Sin(yaw);
//c2 = Math.Cos(yaw);
//s3 = Math.Sin(r0ll);
//c3 = Math.Cos(r0ll);
//82s3 = 82 * 83;
//82c3 = 82 * C3;
//b = (( C1 * C2, -C1 * 82c3 + 81 * 83, C1 * 82s3 + 81 * C3),
//( 82 , C2 * C3 . -C2 * 83 ) *
//(-81 * C2, S1 * S2c3 + C1 * 83,-81 * S2s3 + C1 * C3));
//g = b·~ * dgdx * b ; //--N0te; **tr results in transpose
//t0rque( I)= g[2,3)*(mass_tensor(3,3)- mass_tensor(2,2))
//- g[l,3]* mass_tensor(1,2)
//+ g[1,2]* mass_tensor(1,3)
//- mass_tensor(2,3)*(g[3,3) - g[2,2)) ;
//t0rque(2) ; = g{l,3)*(mass_tensor( 1,1) - mass_tensor(3,3))
//+ g[2.3]* mass_tensor(1,2)
//- g[l,2]* mass_tensor(2,3)
//- mass_tensor(1,3)*(g[l.1)- g[3,3));
//t0rque(3) = g[l,2)*(mass_tensor(2,2)- mass_tensor(1,1))
//- g[2,3]* mass_ tens0r( 1,3)
//+ g[1,3]* mass_tensor(2,3)
//- mass_tensor(1,2)*(g[2,2)- g[l,l));
//return t0rque ;
//}  // gravity _gradient_torque;
//}   // general_gravtty _gradient ;

//// A.5 Spec of Fast_Magnetlc_model
//// with double_ Types;
//// use double_ Types;
//with Vect0r_Matrlx_3;
//use Vect0r_Matrix_3;
//package fast_Magnetlc_model 1s
//Max_Magnettc_model_Name_Length ; constant positive = 80;
//max_degree_and_0rder ; constant positive = 20;
//type Data_ C0efficient_ Array is
//array (Natural range <>, Natural range <>) of double;
//type magnettc_array is array(0 .. max_degree_and_0rder+2) of double;
//type mag_ptr is access magnettc_array;
//type magnetlc_array_2 is array(0 .. max_degree_and_0rder) of
//mag_ptr;
//type Magnetlc_model_Data is private;
//functi0n Create_Magnetlc_model (Name_In ; String;
//G, H ; Data_C0efficient_Array;
//Radius ; double) return Magnetlc_model_Data;
//public void Magg0t (Mmd Magnetlc_model_Data;
//double[] X;
//double R;
//Nax. Max Natural;
//B ; 0ut Vect0r_3);
//private
//type Magnetlc_model_Data is // defaulted t0 p0int mass gem_9
//rec0rd
//Name ; String (1 *. Max_magnetlc_model_Name_Length);
//Name_Length ; Integer;
//G ; magnettc_array_2;
//H ; magnetlc_array_2;
//Radius ; double = 6_371_200.0; // planet mean radius [m]
//model_Max_Size ; Natural;-- max size current gravity model data
//}   // rec0rd;
//} // fast_Magnettc_model;


////A.6 body of Fut_;ua,netlc_model
////with Ext}   //ed_Range_C0mbinat0rtc_Functi0ns;
////use Ext}   //ed_Range_C0mbtnat0rtc_Functt0ns;
////with Exp0nenttal_L0gartthm_Functt0ns;
////use Exp0nenttal_L0gartthm_Functi0ns;
////package body Fast_Magnettc_modelis
//Magnettc_model_Name_T00_L0ng; excepti0n;
//bad_Magnetic_data; excepti0n;
//tw0nml,tw0nml0n,nml0n; Magnetic_array;
//P; Magnetlc_array_2 =(0thers=> new Magnettc_array);
//public void Magg0t (Mmd Magnetlc_model_Data;
//double[] X;
//double R;
//Nax, MaxNatural;
//B ; 0ut Vect0r_3) 1s
//Ri, X0vr, Y0vr, Z0vr, Ep ; double;
//Mu0r, ae0r2, Re0r, Re0m ;double;
//ctil, stil; Magnetic_array;
//Sumh, Sumgam, Sumj, Sumk. Sumh_N, Lambda ; double;
//pnm,cnm,snm,ctnunl,stnunl; double;
//Sumgam_N, Sumj_N, Sumk_N, Mxpnm, Npmpl; double;
//Bnmtll,n_const; double;
//Mml, Mm2, Mpl, Mp2, Nml, Lim ,nm2; Integer;
//pn,pnml,pnm2 ; mag_ptr;
//cn.sn; mag_ptr;
//{
//R1 = 1.0 I R;
//X0vr =X (I) * Ri;
//Y0vr = x (2) * ru;
//Z0vr = X (3) * Ri;
//Ep = Z0vr;
//Re0r = Mmd.Radius * Ri;
//Re0m = Re0r;
//ae0r2 = re0I .. re0r;
//ctn [0] = 1.0; ctJl ( 1) = X0vr;
//sUI [0] = 0.0; stil ( 1) = Y0vr;
//if (Nax < 1)
//                               {
//Sumj = 0.0;
//Surnlt = 0.0;
//Sumh = 0.0;
//Sumgam = 0.0;
//    }
//elseif Max > 0)
//{
//Sumj = re0r*mmd.g[1][1];
//Surnlt = re0r*mmd.h[1][1];

//Sumh = re0r*mmd.g[1][0];
//Sumgam = 3.0*(sumj*x0vr + sumkty0vr) + 2.0*(sumh*z0vr);
//    }
//else //--Max= 0
//    {
//Sumj = 0.0;
//Sumk =0.0;
//Sumh = re0r*mmd.g[1][0];
//Sumgam = 2.0*sumh*z0vr; --n0te; ep and z0vr are the same thing
//}
//p[1][0] = ep;
//for (N in 2 .. Nax l00p)
//{
//n_const = tw0nml[n];
//nml = n- 1;
//nm2 = n- 2;
//pn = p[n];
//pnml = p(nml);
//pnm2 = p(nm2);
//Pn(nml) = ep*Pn[n];
//Pn[0] = Tw0nml0n[n]*Ep*Pnmi[0]- Nml0n[n]*Pnm2[0];
//Pn[1] = Pnm2[1] + n_const * Pnml[0];
//Re0m = Re0m * Re0r;
//en = mmd.g[n];
//sn = nund.h[n];
//Sumh_N = Pn [1] * Cn[0];
//Sumgam_N = Pn [0] * Cn[0] * double(n + 1);
//if Max > 0)
//form in 2 .. nm2 l00p
//Pn[m] = Pnm2[m] + n_const * Pnml(m-1);
//}  // l00p;
//Sumj_N = 0.0;
//Sumk_N = 0.0;
//nml = n- 1;
//ctJl [n] = ctll [1] * ctil (Nml) - stil [1] * stil (Nml);
//stil [n] = stil [1] * ctil (Nml) + ctll [1] * stil (Nml);
//if (N < Max)
//Um=N;
//else
//Um=Max;

//for (M in 1 .. Ltm l00p)
//{
//Mml =M-1;
//Mpl = M + 1;
//Npmpl =double (N + Mpl);
//pnm = pn[m];
//cnm = cn[m];
//snm ;;. sn[m];
//ctnunl = ctil(mml);
//stnunt' = stil(mml);
//Mxpnm = double [m] * Pnm;

//BnmUI = Cnm * ctil [m] + Snm * sUI [m];
//Sumh_N = Sumh_N + Pn(mpl) * Bnmtil;
//Sumgam_N = Sumgam_N + Npmpl * Pnm * Bnmttl;
//Sumj_N = Sumj_N + Mxpnm * (Cnm*ctnunl + Snm*stnunl);
//Sumk_N = Sumk_N- Mxpnm * (Cnm*stnunl - Snm*ctnunl);
//}  // l00p;
//Sumj = Sumj + Re0m * Sumj_N;
//Sumk = Sumk + Re0m * Sumk_N;
//}
////---- SUMS BEL0W HERE HAVE VALUES WHEN M = 0
//Sumh = Sumh + Re0m * Sumh_N;
//Sumgam = Sumgam + Re0m * Sumgam_N;
//}  // l00p;
//Lambda = Sumgam + Ep * Sumh;
//B [1] = ae0r2 * (Lambda * X0vr- Sumj);
//B (2) = ae0r2 * (Lambda * Y0vr- Sumk);
//B (3) = ae0r2 * (Lambda * Z0vr - Sumh);
//} // Maggot;


//function Create_Magnetlc_model (Name_In ; String;
//g. h ; Data_ C0effictent_Array;
//Radius ; double) return Magnetlc_model_Data is
//Gmd ; Magnetlc_model_Data;
//C0ef ;double;
//n_max; ; Integer = g'Last [1];
//{
//if (n_max; < 2) raise bad_magnetlc_data; }
//gmd.g = (0thers => new Magnetlc_anay);
//gmd.h = (0thers => new Magnetlc_array);
//-- Unn0rmaltze gravity model c0efficients
//for (N in g'Range[1] l00p)
//    {
//for (M in 0 .. N l00p)
//{
//tfM = 0)
//Gmd.G [n][0] = G (N, 0}*l.0e-9;
//Gmd.H [n][0] = 0.0;
//else
//C0ef = Sqrt(2.0*Fact0rtal_Ratl0(N - M,N + M))*l.0e-9;
//Gmd.G [n]( M) = C0ef * G [n, m];
//Gmd.H [n]( M) = C0ef* H [n, m];
//}
//}   // l00p;
//}   // l00p;
//Gmd.Radius = Radius;
//Gmd.Name_Length = Name_In'Length;
//if Gmd.Name_Length > Max_Magnetlc_model_Name_Length)
//raiSe Magnetlc_model_Name_T00_L0ng;
//}

//Gmd.Name = (0thers => Ascii.Nul);
//Gmd.Name (I .. Gmd.Name_Length) = Name_In;
//Gmd.model_Max_Size = n_max;
//return Gmd;
//}   // Create_MagneUc_model;
//{ 
////--Initialtze constant values
////--------------------------------------~~-~~~------------------------
//p[0][0] = 1.0; p[0](I) = 0.0; p[0](2) = 0.0;
//p(I)(I) = 1.0; p[1](2) = 0.0; p[1](3) = 0.0;
//for (n in 2 .. Max_Degree_And_0rder l00p
//p[n][n] = p(n-l)(n-I)*reai(2*n-l);
//p[n](n+l) = 0.0;
//p[n](n+2) = 0.0;
//tw0mnl[n] = rea1(2*n- 1);
//tw0mnl0n[n] = tw0mnl[n]/double[n];
//mnl0n[n] = double(n- 1)/double[n];
//}   // l00p;
//}   // Fast_Magnetlc_model;
//54


	}
}
