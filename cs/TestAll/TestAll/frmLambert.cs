using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using MathTimeMethods;     // Edirection, globals
using EOPSPWMethods;       // EOPDataClass, SPWDataClass, iau80Class, iau00Class
using AstroLibMethods;     // EOpt, gravityConst, astroConst, xysdataClass, jpldedataClass
using AstroLambertkMethods;
using System.IO;

namespace TestAllTool
{
    public partial class frmLambert : Form
    {
        public MathTimeLib MathTimeLibr = new MathTimeLib();

        public EOPSPWLib EOPSPWLibr = new EOPSPWLib();

        public AstroLib AstroLibr = new AstroLib();

        public AstroLambertkLib AstroLambertkLibr = new AstroLambertkLib();

        public StringBuilder strbuild = new StringBuilder();

        // 000. gives leading 0's
        // ;+00.;-00. gives signs in front
        string fmt = "0.00000000000";
        string fmtE = "0.0000000000E0";
        string fmt1 = "0.000000000000";
        string fmt2 = "0.00000";

        public frmLambert()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
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
            double ang, f, g, gdot, s, tau;
            double tmin, tminp, tminenergy;
            detailSum = "";
            detailAll = "";
            //int i;
            char dm, de, hitearth;
            // for test180, show = n, show180 = y
            // for testlamb, show = y, show180 = n known cases
            // for envelope, show = n, show180 = n 

            double altpadc = 200.0 / AstroLibr.gravConst.re;  // set 200 km for altitude you set as the over limit. 

            if (this.cbLambertList.SelectedItem == null)
                this.cbLambertList.SelectedIndex = 1;

            // input stored test cases if needed
            switch (Convert.ToInt32(this.storedCase.Text))
            {
                case 1:
                    this.r1.Text = "15945.34075000000 0.00000000000 0.00000000000";
                    this.v1.Text = "0.00000000000 4.99979255422 0.00000000000";
                    this.r2.Text = "12214.83962544290 10249.46731187470 0.00000000000";
                    this.v2.Text = "-4.77718663779 3.67191396603 0.00000000000";
                    this.nrev.Text = "1";
                    this.dtsec.Text = "21000.00";
                    this.dtwait.Text = "0.0";
                    break;

                case 2:
                    this.r1.Text = "2434.30789431563 -6302.76434599776 2433.46804666018";
                    this.v1.Text = "0.00000000000 4.99979255422 0.00000000000";
                    this.r2.Text = "-3244.01178958993 5561.5015207476 3181.63137126354";
                    this.v2.Text = "-4.77718663779 3.67191396603 0.00000000000";
                    this.nrev.Text = "22";
                    this.dtsec.Text = "135479.99997139";
                    this.dtwait.Text = "0.0";
                    break;
            }

            nrev = Convert.ToInt32(this.nrev.Text);
            dtsec = Convert.ToDouble(this.dtsec.Text);
            dtwait = Convert.ToDouble(this.dtwait.Text);

            Regex data = new Regex(@"^\s?(\S+)\s+(\S+)\s+(\S+)\s?");
            // remove additional spaces
            //string line1 = Regex.Replace(this.r1.Text.TrimStart(' '), @"\s+", " ");
            Match match = data.Match(this.r1.Text);
            r1[0] = Convert.ToDouble(match.Groups[1].Value);   // km
            r1[1] = Convert.ToDouble(match.Groups[2].Value);
            r1[2] = Convert.ToDouble(match.Groups[3].Value);

            string line1 = Regex.Replace(this.v1.Text.TrimStart(' '), @"\s+", " ");
            match = data.Match(line1);
            v1[0] = Convert.ToDouble(match.Groups[1].Value);   // km/s
            v1[1] = Convert.ToDouble(match.Groups[2].Value);
            v1[2] = Convert.ToDouble(match.Groups[3].Value);

            line1 = Regex.Replace(this.r2.Text.TrimStart(' '), @"\s+", " ");
            match = data.Match(line1);
            r2[0] = Convert.ToDouble(match.Groups[1].Value);   // km
            r2[1] = Convert.ToDouble(match.Groups[2].Value);
            r2[2] = Convert.ToDouble(match.Groups[3].Value);

            line1 = Regex.Replace(this.v2.Text.TrimStart(' '), @"\s+", " ");
            match = data.Match(line1);
            v2[0] = Convert.ToDouble(match.Groups[1].Value);   // km/s
            v2[1] = Convert.ToDouble(match.Groups[2].Value);
            v2[2] = Convert.ToDouble(match.Groups[3].Value);

            string methodType = this.cbLambertList.SelectedItem.ToString();

            double magr1 = MathTimeLibr.mag(r1);
            double magr2 = MathTimeLibr.mag(r2);

            // this value stays constant in all calcs, vara changes with df
            double cosdeltanu = MathTimeLibr.dot(r1, r2) / (magr1 * magr2);

            //strbuild.AppendLine("now do findtbi calcs");
            //strbuild.AppendLine("iter       y         dtnew          psiold      psinew   psinew-psiold   dtdpsi      dtdpsi2    lower    upper     ");

            AstroLambertkLibr.lambertkmins1st(r1, r2, out s, out tau);
            strbuild.AppendLine(" s " + s.ToString(fmt) + " tau " + tau.ToString(fmt));

            double kbi, tof;
            double[,] tbidk = new double[10, 3];
            AstroLambertkLibr.lambertkmins(s, tau, nrev, 'x', 'L', out kbi, out tof);
            tbidk[1, 1] = kbi;
            tbidk[1, 2] = tof / tusec;

            double[,] tbirk = new double[10, 3];
            AstroLambertkLibr.lambertkmins(s, tau, nrev, 'x', 'H', out kbi, out tof);
            tbirk[1, 1] = kbi;
            tbirk[1, 2] = tof / tusec;

            strbuild.AppendLine("From k variables ");
            strbuild.AppendLine(" " + tbidk[1, 1].ToString("#0.00000000000") + "  " + (tbidk[1, 2] * tusec).ToString("0.00000000000") + " s " + (tbidk[1, 2]).ToString("0.00000000000") + " tu ");
            strbuild.AppendLine("");
            strbuild.AppendLine(" " + tbirk[1, 1].ToString("#0.00000000000") + "  " + (tbirk[1, 2] * tusec).ToString("0.00000000000") + " s " + (tbirk[1, 2]).ToString("0.00000000000") + " tu ");


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

            char modecon = 'n';  // 'c' to shortcut bad cases (hitearth) at iter 3 or 'n'
            double p, a, ecc, incl, argp, raan, nu, m, arglat, truelon, lonper, rad;
            rad = 180.0 / Math.PI;

            strbuild.AppendLine(" TEST ------------------ s/l  H  0 rev ------------------");
            hitearth = ' ';
            dm = 'S';
            de = 'L';
            if (methodType == "lambertk")
            {
                AstroLambertkLibr.lambertK(r1, v1, r2, dm, de, 0, dtwait, dtsec, 0.0, 0.0, numiter, altpadc, modecon, 'n',
                    out v1tk, out v2tk, out f, out g, out gdot, out hitearth, out errorout, out detailSum, out detailAll);
                strbuild.AppendLine(detailAll);
                //strbuild.AppendLine("k#" + caseopt + " " + detailSum + " diffs " + MathTimeLib::mag(dr).ToString("0.00000000000"));
                strbuild.AppendLine("lamk v1t " + " " + v1tk[0].ToString("0.00000000000") + " " + v1tk[1].ToString("0.00000000000") + " " + v1tk[2].ToString("0.00000000000"));
                strbuild.AppendLine("lamk v2t " + " " + v2tk[0].ToString("0.00000000000") + " " + v2tk[1].ToString("0.00000000000") + " " + v2tk[2].ToString("0.00000000000"));
                //strbuild.AppendLine(magv1t.ToString("0.0000000").PadLeft(12) + " " + magv2t.ToString("0.0000000").PadLeft(12));

                AstroLibr.kepler(r1, v1tk, dtsec, out r3h, out v3h);
                strbuild.AppendLine("r3h " + " " + r3h[0].ToString("0.00000000000") + " " + r3h[1].ToString("0.00000000000") + " " + r3h[2].ToString("0.00000000000"));
                for (int j = 0; j < 3; j++)
                    dr[j] = r2[j] - r3h[j];
                if (MathTimeLibr.mag(dr) > 0.01)
                    strbuild.AppendLine("velk does not get to r2 position (km) " + MathTimeLibr.mag(dr).ToString() + "\n");
                this.v1tS.Text = v1tk[0] + " " + v1tk[1] + " " + v1tk[2];
                this.v2tS.Text = v2tk[0] + " " + v2tk[1] + " " + v2tk[2];
            
                AstroLibr.rv2coe(r1, v1tk, out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out arglat, out truelon, out lonper);
                this.aeiS.Text = "aei " + a + " " + ecc + " " + incl*rad;
            }

            if (methodType == "lambertu")
            {
                AstroLibr.lambertuniv(r1, r2, v1, dm, de, 0, 0.0, dtsec, 0.0, altpadc * AstroLibr.gravConst.re, 'n', out v1tu, out v2tu, out hitearth, out detailSum, out detailAll);
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

                this.v1tS.Text = v1tu[0] + " " + v1tu[1] + " " + v1tu[2];
                this.v2tS.Text = v2tu[0] + " " + v2tu[1] + " " + v2tu[2];

                AstroLibr.rv2coe(r1, v1tu, out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out arglat, out truelon, out lonper);
                this.aeiS.Text = "aei " + a + " " + ecc + " " + incl * rad;
            }

            if (methodType == "lambertb")
            {

                AstroLibr.lambertbattin(r1, r2, v1, dm, de, 0, 0.0, dtsec, altpadc * AstroLibr.gravConst.re, 'n', out v1tb, out v2tb, out hitearth, out detailSum, out detailAll);
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
                this.v1tS.Text = v1tb[0] + " " + v1tb[1] + " " + v1tb[2];
                this.v2tS.Text = v2tb[0] + " " + v2tb[1] + " " + v2tb[2];

                AstroLibr.rv2coe(r1, v1tb, out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out arglat, out truelon, out lonper);
                this.aeiS.Text = "aei " + a + " " + ecc + " " + incl * rad;
            }
            this.hitearthS.Text = Convert.ToString(hitearth);

            strbuild.AppendLine(" TEST ------------------ s/l L 0 rev ------------------");
            dm = 'L';
            de = 'H';
            if (methodType == "lambertk")
            {
                // k near 180 is about 53017 while battin is 30324!
                AstroLambertkLibr.lambertK(r1, v1, r2, dm, de, 0, dtwait, dtsec, 0.0, 0.0, numiter, altpadc, modecon, 'n',
                    out v1tk, out v2tk, out f, out g, out gdot, out hitearth, out errorout, out detailSum, out detailAll);
                strbuild.AppendLine(detailAll);
                //strbuild.AppendLine("k#" + caseopt + " " + detailSum + " diffs " + MathTimeLibr.mag(dr).ToString("0.00000000000"));
                strbuild.AppendLine("lamk v1t " + " " + v1tk[0].ToString("0.00000000000") + " " + v1tk[1].ToString("0.00000000000") + " " + v1tk[2].ToString("0.00000000000"));
                strbuild.AppendLine("lamk v2t " + " " + v2tk[0].ToString("0.00000000000") + " " + v2tk[1].ToString("0.00000000000") + " " + v2tk[2].ToString("0.00000000000"));
                //strbuild.AppendLine(magv1t.ToString("0.0000000").PadLeft(12) + " " + magv2t.ToString("0.0000000").PadLeft(12));

                AstroLibr.kepler(r1, v1tk, dtsec, out r3h, out v3h);
                for (int j = 0; j < 3; j++)
                    dr[j] = r2[j] - r3h[j];
                if (MathTimeLibr.mag(dr) > 0.01)
                    strbuild.AppendLine("velk does not get to r2 (km) position " + MathTimeLibr.mag(dr).ToString() + "\n");
                this.v1tL.Text = v1tk[0] + " " + v1tk[1] + " " + v1tk[2];
                this.v2tL.Text = v2tk[0] + " " + v2tk[1] + " " + v2tk[2];

                AstroLibr.rv2coe(r1, v1tk, out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out arglat, out truelon, out lonper);
                this.aeiL.Text = "aei " + a + " " + ecc + " " + incl * rad;
            }
            if (methodType == "lambertu")
            {
                AstroLibr.lambertuniv(r1, r2, v1, dm, de, 0, 0.0, dtsec, 0.0, altpadc * AstroLibr.gravConst.re, 'n', out v1tu, out v2tu, out hitearth, out detailSum, out detailAll);
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

                this.v1tL.Text = v1tu[0] + " " + v1tu[1] + " " + v1tu[2];
                this.v2tL.Text = v2tu[0] + " " + v2tu[1] + " " + v2tu[2];

                AstroLibr.rv2coe(r1, v1tu, out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out arglat, out truelon, out lonper);
                this.aeiL.Text = "aei " + a + " " + ecc + " " + incl * rad;
            }
            if (methodType == "lambertb")
            {

                AstroLibr.lambertbattin(r1, r2, v1, dm, de, 0, 0.0, dtsec, altpadc * AstroLibr.gravConst.re, 'n', out v1tb, out v2tb, out hitearth, out detailSum, out detailAll);
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
                this.v1tL.Text = v1tb[0] + " " + v1tb[1] + " " + v1tb[2];
                this.v2tL.Text = v2tb[0] + " " + v2tb[1] + " " + v2tb[2];

                AstroLibr.rv2coe(r1, v1tb, out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out arglat, out truelon, out lonper);
                this.aeiL.Text = "aei " + a + " " + ecc + " " + incl * rad;
            }
            this.hitearthL.Text = Convert.ToString(hitearth);

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
            //if (dtsec / tusec >= tbidk[nnrev, 2])
            // do inside lambertk now
            {
                dm = 'S';
                de = 'L';
                if (methodType == "lambertk")
                {
                    AstroLambertkLibr.lambertkmins(s, tau, nnrev, dm, de, out kbi, out tof);
                    AstroLambertkLibr.lambertK(r1, v1, r2, dm, de, nnrev, dtwait, dtsec, tof, kbi, numiter, altpadc, modecon, 'n',
                        out v1tk, out v2tk, out f, out g, out gdot, out hitearth, out errorout, out detailSum, out detailAll);
                    strbuild.AppendLine(detailAll);
                    //strbuild.AppendLine("k#" + caseopt + " " + detailSum + " diffs " + MathTimeLibr.mag(dr).ToString("0.00000000000"));
                    strbuild.AppendLine("lamk v1t " + " " + v1tk[0].ToString("0.00000000000") + " " + v1tk[1].ToString("0.00000000000") + " " + v1tk[2].ToString("0.00000000000"));
                    strbuild.AppendLine("lamk v2t " + " " + v2tk[0].ToString("0.00000000000") + " " + v2tk[1].ToString("0.00000000000") + " " + v2tk[2].ToString("0.00000000000"));
                    //strbuild.AppendLine(magv1t.ToString("0.0000000").PadLeft(12) + " " + magv2t.ToString("0.0000000").PadLeft(12));

                    AstroLibr.kepler(r1, v1tk, dtsec, out r3h, out v3h);
                    for (int j = 0; j < 3; j++)
                        dr[j] = r2[j] - r3h[j];
                    if (MathTimeLibr.mag(dr) > 0.01)
                        strbuild.AppendLine("velk does not get to r2 (km) position " + MathTimeLibr.mag(dr).ToString() + "\n");

                    this.v1tSL.Text = v1tk[0] + " " + v1tk[1] + " " + v1tk[2];
                    this.v2tSL.Text = v2tk[0] + " " + v2tk[1] + " " + v2tk[2];

                    AstroLibr.rv2coe(r1, v1tk, out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out arglat, out truelon, out lonper);
                    this.aeiSL.Text = "aei " + a + " " + ecc + " " + incl * rad;
                }
                if (methodType == "lambertu")
                {

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

                    this.v1tSL.Text = v1tu[0] + " " + v1tu[1] + " " + v1tu[2];
                    this.v2tSL.Text = v2tu[0] + " " + v2tu[1] + " " + v2tu[2];

                    AstroLibr.rv2coe(r1, v1tu, out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out arglat, out truelon, out lonper);
                    this.aeiSL.Text = "aei " + a + " " + ecc + " " + incl * rad;
                }
                if (methodType == "lambertb")
                {

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
                    this.v1tSH.Text = v1tb[0] + " " + v1tb[1] + " " + v1tb[2];
                    this.v2tSH.Text = v2tb[0] + " " + v2tb[1] + " " + v2tb[2];

                    AstroLibr.rv2coe(r1, v1tb, out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out arglat, out truelon, out lonper);
                    this.aeiSL.Text = "aei " + a + " " + ecc + " " + incl * rad;
                }
            }
            //else
            //    strbuild.AppendLine(" ------------------------- not enough time for 1 revs ");
            this.hitearthSL.Text = Convert.ToString(hitearth);

            strbuild.AppendLine(" TEST ------------------ L  L " + nnrev.ToString() + " rev ------------------");
            //if (dtsec / tusec >= tbidk[nnrev, 2])
            // do inside lambertk now
            {
                dm = 'L';
                de = 'L';
                // switch tdi!!  tdidk to tdirk 'H'
                if (methodType == "lambertk")
                {
                    AstroLambertkLibr.lambertkmins(s, tau, nnrev, dm, de, out kbi, out tof);  // 'H'

                    //double tofk1, kbik2, tofk2, kbik1;
                    //string outstr;
                    //getmins(1, 'k', nrev, r1, r2, s, tau, dm, de, out tofk1, out kbik1, out tofk2, out kbik2, out outstr);

                    AstroLambertkLibr.lambertK(r1, v1, r2, dm, de, nnrev, dtwait, dtsec, tof, kbi, numiter, altpadc, modecon, 'n',
                        out v1tk, out v2tk, out f, out g, out gdot, out hitearth, out errorout, out detailSum, out detailAll);
                    strbuild.AppendLine(detailAll);
                    //strbuild.AppendLine("k#" + caseopt + " " + detailSum + " diffs " + MathTimeLibr.mag(dr).ToString("0.00000000000"));
                    strbuild.AppendLine("lamk v1t " + " " + v1tk[0].ToString("0.00000000000") + " " + v1tk[1].ToString("0.00000000000") + " " + v1tk[2].ToString("0.00000000000"));
                    strbuild.AppendLine("lamk v2t " + " " + v2tk[0].ToString("0.00000000000") + " " + v2tk[1].ToString("0.00000000000") + " " + v2tk[2].ToString("0.00000000000"));
                    //strbuild.AppendLine(magv1t.ToString("0.0000000").PadLeft(12) + " " + magv2t.ToString("0.0000000").PadLeft(12));

                    AstroLibr.kepler(r1, v1tk, dtsec, out r3h, out v3h);
                    for (int j = 0; j < 3; j++)
                        dr[j] = r2[j] - r3h[j];
                    if (MathTimeLibr.mag(dr) > 0.01)
                        strbuild.AppendLine("velk does not get to r2 (km) position " + MathTimeLibr.mag(dr).ToString() + "\n");

                    this.v1tLL.Text = v1tk[0] + " " + v1tk[1] + " " + v1tk[2];
                    this.v2tLL.Text = v2tk[0] + " " + v2tk[1] + " " + v2tk[2];

                    AstroLibr.rv2coe(r1, v1tk, out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out arglat, out truelon, out lonper);
                    this.aeiLL.Text = "aei " + a + " " + ecc + " " + incl * rad;
                }
                if (methodType == "lambertu")
                {

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

                    this.v1tLL.Text = v1tu[0] + " " + v1tu[1] + " " + v1tu[2];
                    this.v2tLL.Text = v2tu[0] + " " + v2tu[1] + " " + v2tu[2];

                    AstroLibr.rv2coe(r1, v1tu, out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out arglat, out truelon, out lonper);
                    this.aeiLL.Text = "aei " + a + " " + ecc + " " + incl * rad;
                }
                if (methodType == "lambertb")
                {

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
                    this.v1tLL.Text = v1tb[0] + " " + v1tb[1] + " " + v1tb[2];
                    this.v2tLL.Text = v2tb[0] + " " + v2tb[1] + " " + v2tb[2];

                    AstroLibr.rv2coe(r1, v1tb, out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out arglat, out truelon, out lonper);
                    this.aeiLL.Text = "aei " + a + " " + ecc + " " + incl * rad;
                }
            }
            //else
            //    strbuild.AppendLine(" ------------------------- not enough time for 1 revs ");
            this.hitearthLL.Text = Convert.ToString(hitearth);

            strbuild.AppendLine(" TEST ------------------ S  H " + nnrev.ToString() + " rev ------------------");
            //if (dtsec / tusec >= tbirk[nnrev, 2])
            // do inside lambertk now
            {
                dm = 'S';
                de = 'H';
                // switch tdi!!  tdirk to tdidk  'L'
                if (methodType == "lambertk")
                {
                    AstroLambertkLibr.lambertkmins(s, tau, nnrev, dm, de, out kbi, out tof);  // 'L'
                    AstroLambertkLibr.lambertK(r1, v1, r2, dm, de, nnrev, dtwait, dtsec, tof, kbi, numiter, altpadc, modecon, 'n',
                        out v1tk, out v2tk, out f, out g, out gdot, out hitearth, out errorout, out detailSum, out detailAll);
                    strbuild.AppendLine(detailAll);
                    //strbuild.AppendLine("k#" + caseopt + " " + detailSum + " diffs " + MathTimeLibr.mag(dr).ToString("0.00000000000"));
                    strbuild.AppendLine("lamk v1t " + " " + v1tk[0].ToString("0.00000000000") + " " + v1tk[1].ToString("0.00000000000") + " " + v1tk[2].ToString("0.00000000000"));
                    strbuild.AppendLine("lamk v2t " + " " + v2tk[0].ToString("0.00000000000") + " " + v2tk[1].ToString("0.00000000000") + " " + v2tk[2].ToString("0.00000000000"));
                    //strbuild.AppendLine(magv1t.ToString("0.0000000").PadLeft(12) + " " + magv2t.ToString("0.0000000").PadLeft(12));

                    AstroLibr.kepler(r1, v1tk, dtsec, out r3h, out v3h);
                    for (int j = 0; j < 3; j++)
                        dr[j] = r2[j] - r3h[j];
                    if (MathTimeLibr.mag(dr) > 0.01)
                        strbuild.AppendLine("velk does not get to r2 (km) position " + MathTimeLibr.mag(dr).ToString() + "\n");

                    this.v1tSH.Text = v1tk[0] + " " + v1tk[1] + " " + v1tk[2];
                    this.v2tSH.Text = v2tk[0] + " " + v2tk[1] + " " + v2tk[2];

                    AstroLibr.rv2coe(r1, v1tk, out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out arglat, out truelon, out lonper);
                    this.aeiSH.Text = "aei " + a + " " + ecc + " " + incl * rad;
                }
                if (methodType == "lambertu")
                {

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

                    this.v1tSH.Text = v1tu[0] + " " + v1tu[1] + " " + v1tu[2];
                    this.v2tSH.Text = v2tu[0] + " " + v2tu[1] + " " + v2tu[2];

                    AstroLibr.rv2coe(r1, v1tu, out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out arglat, out truelon, out lonper);
                    this.aeiSH.Text = "aei " + a + " " + ecc + " " + incl * rad;
                }
                if (methodType == "lambertb")
                {

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
                    this.v1tSH.Text = v1tb[0] + " " + v1tb[1] + " " + v1tb[2];
                    this.v2tSH.Text = v2tb[0] + " " + v2tb[1] + " " + v2tb[2];

                    AstroLibr.rv2coe(r1, v1tb, out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out arglat, out truelon, out lonper);
                    this.aeiSH.Text = "aei " + a + " " + ecc + " " + incl * rad;
                }
            }
            //else
            //    strbuild.AppendLine(" ------------------------- not enough time for 1 revs ");
            this.hitearthSH.Text = Convert.ToString(hitearth);

            strbuild.AppendLine(" TEST ------------------ L  H " + nnrev.ToString() + " rev ------------------");
            //if (dtsec / tusec >= tbirk[nnrev, 2])
            // do inside lambertk now
            {
                dm = 'L';
                de = 'H';
                if (methodType == "lambertk")
                {
                    AstroLambertkLibr.lambertkmins(s, tau, nnrev, dm, de, out kbi, out tof);
                    AstroLambertkLibr.lambertK(r1, v1, r2, dm, de, nnrev, dtwait, dtsec, tof, kbi, numiter, altpadc, modecon, 'n',
                        out v1tk, out v2tk, out f, out g, out gdot, out hitearth, out errorout, out detailSum, out detailAll);
                    strbuild.AppendLine(detailAll);
                    //strbuild.AppendLine("k#" + caseopt + " " + detailSum + " diffs " + MathTimeLibr.mag(dr).ToString("0.00000000000"));
                    strbuild.AppendLine("lamk v1t " + " " + v1tk[0].ToString("0.00000000000") + " " + v1tk[1].ToString("0.00000000000") + " " + v1tk[2].ToString("0.00000000000"));
                    strbuild.AppendLine("lamk v2t " + " " + v2tk[0].ToString("0.00000000000") + " " + v2tk[1].ToString("0.00000000000") + " " + v2tk[2].ToString("0.00000000000"));
                    //strbuild.AppendLine(magv1t.ToString("0.0000000").PadLeft(12) + " " + magv2t.ToString("0.0000000").PadLeft(12));

                    AstroLibr.kepler(r1, v1tk, dtsec, out r3h, out v3h);
                    for (int j = 0; j < 3; j++)
                        dr[j] = r2[j] - r3h[j];
                    if (MathTimeLibr.mag(dr) > 0.01)
                        strbuild.AppendLine("velk does not get to r2 (km) position " + MathTimeLibr.mag(dr).ToString() + "\n");

                    this.v1tLH.Text = v1tk[0] + " " + v1tk[1] + " " + v1tk[2];
                    this.v2tLH.Text = v2tk[0] + " " + v2tk[1] + " " + v2tk[2];

                    AstroLibr.rv2coe(r1, v1tk, out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out arglat, out truelon, out lonper);
                    this.aeiLH.Text = "aei " + a + " " + ecc + " " + incl * rad;
                }
                if (methodType == "lambertu")
                {

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

                    this.v1tLH.Text = v1tu[0] + " " + v1tu[1] + " " + v1tu[2];
                    this.v2tLH.Text = v2tu[0] + " " + v2tu[1] + " " + v2tu[2];

                    AstroLibr.rv2coe(r1, v1tu, out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out arglat, out truelon, out lonper);
                    this.aeiLH.Text = "aei " + a + " " + ecc + " " + incl * rad;
                }
                if (methodType == "lambertb")
                {

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
                    this.v1tLH.Text = v1tb[0] + " " + v1tb[1] + " " + v1tb[2];
                    this.v2tLH.Text = v2tb[0] + " " + v2tb[1] + " " + v2tb[2];

                    AstroLibr.rv2coe(r1, v1tb, out p, out a, out ecc, out incl, out raan, out argp, out nu, out m, out arglat, out truelon, out lonper);
                    this.aeiLH.Text = "aei " + a + " " + ecc + " " + incl * rad;
                }
            }
            //else
            //    strbuild.AppendLine(" ------------------------- not enough time for 1 revs ");
            this.hitearthLH.Text = Convert.ToString(hitearth);

            string directory = @"D:\Codes\LIBRARY\cs\TestAll\";
            File.WriteAllText(directory + "testall-Lambert.out", strbuild.ToString());

            Refresh();
        }


        // write out so it can be run later if input from gui
        private void button1_Click_1(object sender, EventArgs e)
        {
            strbuild.AppendLine("# 30 new test case ");
            strbuild.AppendLine(this.r1.Text + "," + this.v1.Text + ",");
            strbuild.AppendLine(this.r2.Text + "," + this.v2.Text + ",");
            strbuild.AppendLine(this.dtsec.Text + "," + this.nrev.Text + "," + this.dtwait.Text + ",");
        }


    }  // class

}  // namespace
