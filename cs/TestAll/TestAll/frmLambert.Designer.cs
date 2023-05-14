namespace TestAllTool
{
    partial class frmLambert
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSearch = new System.Windows.Forms.Button();
            this.lblEpoch = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.r1 = new System.Windows.Forms.TextBox();
            this.r2 = new System.Windows.Forms.TextBox();
            this.v1tS = new System.Windows.Forms.TextBox();
            this.v2tS = new System.Windows.Forms.TextBox();
            this.dtsec = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.v1 = new System.Windows.Forms.TextBox();
            this.v2 = new System.Windows.Forms.TextBox();
            this.nrev = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.v2tL = new System.Windows.Forms.TextBox();
            this.v1tL = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.v2tSL = new System.Windows.Forms.TextBox();
            this.v1tSL = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.v2tSH = new System.Windows.Forms.TextBox();
            this.v1tSH = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.v2tLL = new System.Windows.Forms.TextBox();
            this.v1tLL = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.v2tLH = new System.Windows.Forms.TextBox();
            this.v1tLH = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.cbLambertList = new System.Windows.Forms.ComboBox();
            this.hitearthS = new System.Windows.Forms.TextBox();
            this.hitearthL = new System.Windows.Forms.TextBox();
            this.hitearthSL = new System.Windows.Forms.TextBox();
            this.hitearthSH = new System.Windows.Forms.TextBox();
            this.hitearthLL = new System.Windows.Forms.TextBox();
            this.hitearthLH = new System.Windows.Forms.TextBox();
            this.dtwait = new System.Windows.Forms.TextBox();
            this.aeiS = new System.Windows.Forms.TextBox();
            this.aeiL = new System.Windows.Forms.TextBox();
            this.aeiSL = new System.Windows.Forms.TextBox();
            this.aeiSH = new System.Windows.Forms.TextBox();
            this.aeiLL = new System.Windows.Forms.TextBox();
            this.aeiLH = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.storedCase = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.btnSearch.Location = new System.Drawing.Point(550, 905);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(6);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(315, 55);
            this.btnSearch.TabIndex = 250;
            this.btnSearch.Text = "Calculate";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // lblEpoch
            // 
            this.lblEpoch.AutoSize = true;
            this.lblEpoch.Location = new System.Drawing.Point(67, 439);
            this.lblEpoch.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblEpoch.Name = "lblEpoch";
            this.lblEpoch.Size = new System.Drawing.Size(81, 25);
            this.lblEpoch.TabIndex = 249;
            this.lblEpoch.Text = "Dt (sec)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(65, 144);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(240, 25);
            this.label1.TabIndex = 248;
            this.label1.Text = "Satellite State at start (km)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(726, 42);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(254, 32);
            this.label2.TabIndex = 251;
            this.label2.Text = "Lambert Calculator";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(65, 285);
            this.label5.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(191, 25);
            this.label5.TabIndex = 254;
            this.label5.Text = "Satellite State at end";
            // 
            // r1
            // 
            this.r1.Location = new System.Drawing.Point(114, 176);
            this.r1.Name = "r1";
            this.r1.Size = new System.Drawing.Size(542, 29);
            this.r1.TabIndex = 256;
            this.r1.Text = "15945.34075000000 0.00000000000 0.00000000000";
            // 
            // r2
            // 
            this.r2.Location = new System.Drawing.Point(114, 332);
            this.r2.Name = "r2";
            this.r2.Size = new System.Drawing.Size(542, 29);
            this.r2.TabIndex = 257;
            this.r2.Text = "12214.83962544290 10249.46731187470 0.00000000000";
            // 
            // v1tS
            // 
            this.v1tS.Location = new System.Drawing.Point(884, 176);
            this.v1tS.Name = "v1tS";
            this.v1tS.Size = new System.Drawing.Size(540, 29);
            this.v1tS.TabIndex = 259;
            this.v1tS.Text = "5.07434678875 1.60765512787 0.00000000000";
            // 
            // v2tS
            // 
            this.v2tS.Location = new System.Drawing.Point(884, 227);
            this.v2tS.Name = "v2tS";
            this.v2tS.Size = new System.Drawing.Size(540, 29);
            this.v2tS.TabIndex = 260;
            this.v2tS.Text = "-4.92055600588 -2.03019195311 0.00000000000";
            // 
            // dtsec
            // 
            this.dtsec.Location = new System.Drawing.Point(116, 478);
            this.dtsec.Name = "dtsec";
            this.dtsec.Size = new System.Drawing.Size(140, 29);
            this.dtsec.TabIndex = 261;
            this.dtsec.Text = "21000.00";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1029, 133);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(166, 25);
            this.label3.TabIndex = 262;
            this.label3.Text = "0 Revolution case";
            // 
            // v1
            // 
            this.v1.Location = new System.Drawing.Point(114, 226);
            this.v1.Name = "v1";
            this.v1.Size = new System.Drawing.Size(542, 29);
            this.v1.TabIndex = 263;
            this.v1.Text = "0.00000000000 4.99979255422 0.00000000000";
            // 
            // v2
            // 
            this.v2.Location = new System.Drawing.Point(114, 385);
            this.v2.Name = "v2";
            this.v2.Size = new System.Drawing.Size(542, 29);
            this.v2.TabIndex = 264;
            this.v2.Text = "-4.77718663779 3.67191396603 0.00000000000";
            // 
            // nrev
            // 
            this.nrev.Location = new System.Drawing.Point(297, 478);
            this.nrev.Name = "nrev";
            this.nrev.Size = new System.Drawing.Size(49, 29);
            this.nrev.TabIndex = 265;
            this.nrev.Text = "1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(292, 439);
            this.label4.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 25);
            this.label4.TabIndex = 266;
            this.label4.Text = "nrev";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(764, 302);
            this.label6.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 25);
            this.label6.TabIndex = 269;
            this.label6.Text = "Long";
            // 
            // v2tL
            // 
            this.v2tL.Location = new System.Drawing.Point(893, 353);
            this.v2tL.Name = "v2tL";
            this.v2tL.Size = new System.Drawing.Size(540, 29);
            this.v2tL.TabIndex = 268;
            this.v2tL.Text = "-4.92055600588 -2.03019195311 0.00000000000";
            // 
            // v1tL
            // 
            this.v1tL.Location = new System.Drawing.Point(893, 302);
            this.v1tL.Name = "v1tL";
            this.v1tL.Size = new System.Drawing.Size(540, 29);
            this.v1tL.TabIndex = 267;
            this.v1tL.Text = "5.07434678875 1.60765512787 0.00000000000";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(1012, 414);
            this.label7.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(222, 25);
            this.label7.TabIndex = 272;
            this.label7.Text = "Mulitple Revolution case";
            // 
            // v2tSL
            // 
            this.v2tSL.Location = new System.Drawing.Point(893, 508);
            this.v2tSL.Name = "v2tSL";
            this.v2tSL.Size = new System.Drawing.Size(540, 29);
            this.v2tSL.TabIndex = 271;
            this.v2tSL.Text = "-4.92055600588 -2.03019195311 0.00000000000";
            // 
            // v1tSL
            // 
            this.v1tSL.Location = new System.Drawing.Point(893, 457);
            this.v1tSL.Name = "v1tSL";
            this.v1tSL.Size = new System.Drawing.Size(540, 29);
            this.v1tSL.TabIndex = 270;
            this.v1tSL.Text = "5.07434678875 1.60765512787 0.00000000000";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(761, 191);
            this.label8.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 25);
            this.label8.TabIndex = 273;
            this.label8.Text = "Short";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(761, 461);
            this.label9.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(100, 25);
            this.label9.TabIndex = 274;
            this.label9.Text = "Short Low";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(761, 585);
            this.label10.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(104, 25);
            this.label10.TabIndex = 277;
            this.label10.Text = "Short High";
            // 
            // v2tSH
            // 
            this.v2tSH.Location = new System.Drawing.Point(893, 632);
            this.v2tSH.Name = "v2tSH";
            this.v2tSH.Size = new System.Drawing.Size(540, 29);
            this.v2tSH.TabIndex = 276;
            this.v2tSH.Text = "-4.92055600588 -2.03019195311 0.00000000000";
            // 
            // v1tSH
            // 
            this.v1tSH.Location = new System.Drawing.Point(893, 581);
            this.v1tSH.Name = "v1tSH";
            this.v1tSH.Size = new System.Drawing.Size(540, 29);
            this.v1tSH.TabIndex = 275;
            this.v1tSH.Text = "5.07434678875 1.60765512787 0.00000000000";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(764, 704);
            this.label11.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(97, 25);
            this.label11.TabIndex = 280;
            this.label11.Text = "Long Low";
            // 
            // v2tLL
            // 
            this.v2tLL.Location = new System.Drawing.Point(893, 755);
            this.v2tLL.Name = "v2tLL";
            this.v2tLL.Size = new System.Drawing.Size(540, 29);
            this.v2tLL.TabIndex = 279;
            this.v2tLL.Text = "-4.92055600588 -2.03019195311 0.00000000000";
            // 
            // v1tLL
            // 
            this.v1tLL.Location = new System.Drawing.Point(893, 704);
            this.v1tLL.Name = "v1tLL";
            this.v1tLL.Size = new System.Drawing.Size(540, 29);
            this.v1tLL.TabIndex = 278;
            this.v1tLL.Text = "5.07434678875 1.60765512787 0.00000000000";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(764, 831);
            this.label12.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(101, 25);
            this.label12.TabIndex = 283;
            this.label12.Text = "Long High";
            // 
            // v2tLH
            // 
            this.v2tLH.Location = new System.Drawing.Point(893, 882);
            this.v2tLH.Name = "v2tLH";
            this.v2tLH.Size = new System.Drawing.Size(540, 29);
            this.v2tLH.TabIndex = 282;
            this.v2tLH.Text = "-4.92055600588 -2.03019195311 0.00000000000";
            // 
            // v1tLH
            // 
            this.v1tLH.Location = new System.Drawing.Point(893, 831);
            this.v1tLH.Name = "v1tLH";
            this.v1tLH.Size = new System.Drawing.Size(540, 29);
            this.v1tLH.TabIndex = 281;
            this.v1tLH.Text = "5.07434678875 1.60765512787 0.00000000000";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(65, 596);
            this.label13.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(78, 25);
            this.label13.TabIndex = 285;
            this.label13.Text = "Method";
            // 
            // cbLambertList
            // 
            this.cbLambertList.FormattingEnabled = true;
            this.cbLambertList.Items.AddRange(new object[] {
            "lambertu",
            "lambertb",
            "lambertk"});
            this.cbLambertList.Location = new System.Drawing.Point(116, 639);
            this.cbLambertList.Margin = new System.Windows.Forms.Padding(6);
            this.cbLambertList.Name = "cbLambertList";
            this.cbLambertList.Size = new System.Drawing.Size(312, 32);
            this.cbLambertList.TabIndex = 284;
            this.cbLambertList.Text = "Select...";
            // 
            // hitearthS
            // 
            this.hitearthS.Location = new System.Drawing.Point(1452, 176);
            this.hitearthS.Name = "hitearthS";
            this.hitearthS.Size = new System.Drawing.Size(49, 29);
            this.hitearthS.TabIndex = 286;
            this.hitearthS.Text = "1";
            // 
            // hitearthL
            // 
            this.hitearthL.Location = new System.Drawing.Point(1452, 298);
            this.hitearthL.Name = "hitearthL";
            this.hitearthL.Size = new System.Drawing.Size(49, 29);
            this.hitearthL.TabIndex = 287;
            this.hitearthL.Text = "1";
            // 
            // hitearthSL
            // 
            this.hitearthSL.Location = new System.Drawing.Point(1452, 457);
            this.hitearthSL.Name = "hitearthSL";
            this.hitearthSL.Size = new System.Drawing.Size(49, 29);
            this.hitearthSL.TabIndex = 288;
            this.hitearthSL.Text = "1";
            // 
            // hitearthSH
            // 
            this.hitearthSH.Location = new System.Drawing.Point(1452, 581);
            this.hitearthSH.Name = "hitearthSH";
            this.hitearthSH.Size = new System.Drawing.Size(49, 29);
            this.hitearthSH.TabIndex = 289;
            this.hitearthSH.Text = "1";
            // 
            // hitearthLL
            // 
            this.hitearthLL.Location = new System.Drawing.Point(1452, 701);
            this.hitearthLL.Name = "hitearthLL";
            this.hitearthLL.Size = new System.Drawing.Size(49, 29);
            this.hitearthLL.TabIndex = 290;
            this.hitearthLL.Text = "1";
            // 
            // hitearthLH
            // 
            this.hitearthLH.Location = new System.Drawing.Point(1452, 827);
            this.hitearthLH.Name = "hitearthLH";
            this.hitearthLH.Size = new System.Drawing.Size(49, 29);
            this.hitearthLH.TabIndex = 291;
            this.hitearthLH.Text = "1";
            // 
            // dtwait
            // 
            this.dtwait.Location = new System.Drawing.Point(116, 529);
            this.dtwait.Name = "dtwait";
            this.dtwait.Size = new System.Drawing.Size(140, 29);
            this.dtwait.TabIndex = 292;
            this.dtwait.Text = "0.00";
            // 
            // aeiS
            // 
            this.aeiS.Location = new System.Drawing.Point(1243, 262);
            this.aeiS.Name = "aeiS";
            this.aeiS.Size = new System.Drawing.Size(439, 29);
            this.aeiS.TabIndex = 293;
            this.aeiS.Text = "-4.92055600588 -2.03019195311 0.00000000000";
            // 
            // aeiL
            // 
            this.aeiL.Location = new System.Drawing.Point(1243, 388);
            this.aeiL.Name = "aeiL";
            this.aeiL.Size = new System.Drawing.Size(439, 29);
            this.aeiL.TabIndex = 294;
            this.aeiL.Text = "-4.92055600588 -2.03019195311 0.00000000000";
            // 
            // aeiSL
            // 
            this.aeiSL.Location = new System.Drawing.Point(1243, 543);
            this.aeiSL.Name = "aeiSL";
            this.aeiSL.Size = new System.Drawing.Size(439, 29);
            this.aeiSL.TabIndex = 295;
            this.aeiSL.Text = "-4.92055600588 -2.03019195311 0.00000000000";
            // 
            // aeiSH
            // 
            this.aeiSH.Location = new System.Drawing.Point(1243, 669);
            this.aeiSH.Name = "aeiSH";
            this.aeiSH.Size = new System.Drawing.Size(439, 29);
            this.aeiSH.TabIndex = 296;
            this.aeiSH.Text = "-4.92055600588 -2.03019195311 0.00000000000";
            // 
            // aeiLL
            // 
            this.aeiLL.Location = new System.Drawing.Point(1243, 790);
            this.aeiLL.Name = "aeiLL";
            this.aeiLL.Size = new System.Drawing.Size(439, 29);
            this.aeiLL.TabIndex = 297;
            this.aeiLL.Text = "-4.92055600588 -2.03019195311 0.00000000000";
            // 
            // aeiLH
            // 
            this.aeiLH.Location = new System.Drawing.Point(1243, 917);
            this.aeiLH.Name = "aeiLH";
            this.aeiLH.Size = new System.Drawing.Size(439, 29);
            this.aeiLH.TabIndex = 298;
            this.aeiLH.Text = "-4.92055600588 -2.03019195311 0.00000000000";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(232, 755);
            this.label14.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(205, 25);
            this.label14.TabIndex = 300;
            this.label14.Text = "Use Case (0) for none";
            // 
            // storedCase
            // 
            this.storedCase.Location = new System.Drawing.Point(297, 794);
            this.storedCase.Name = "storedCase";
            this.storedCase.Size = new System.Drawing.Size(49, 29);
            this.storedCase.TabIndex = 299;
            this.storedCase.Text = "1";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.button1.Location = new System.Drawing.Point(613, 972);
            this.button1.Margin = new System.Windows.Forms.Padding(6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(185, 38);
            this.button1.TabIndex = 423;
            this.button1.Text = "Write out input";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // frmLambert
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1738, 1076);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.storedCase);
            this.Controls.Add(this.aeiLH);
            this.Controls.Add(this.aeiLL);
            this.Controls.Add(this.aeiSH);
            this.Controls.Add(this.aeiSL);
            this.Controls.Add(this.aeiL);
            this.Controls.Add(this.aeiS);
            this.Controls.Add(this.dtwait);
            this.Controls.Add(this.hitearthLH);
            this.Controls.Add(this.hitearthLL);
            this.Controls.Add(this.hitearthSH);
            this.Controls.Add(this.hitearthSL);
            this.Controls.Add(this.hitearthL);
            this.Controls.Add(this.hitearthS);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.cbLambertList);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.v2tLH);
            this.Controls.Add(this.v1tLH);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.v2tLL);
            this.Controls.Add(this.v1tLL);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.v2tSH);
            this.Controls.Add(this.v1tSH);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.v2tSL);
            this.Controls.Add(this.v1tSL);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.v2tL);
            this.Controls.Add(this.v1tL);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.nrev);
            this.Controls.Add(this.v2);
            this.Controls.Add(this.v1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dtsec);
            this.Controls.Add(this.v2tS);
            this.Controls.Add(this.v1tS);
            this.Controls.Add(this.r2);
            this.Controls.Add(this.r1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.lblEpoch);
            this.Controls.Add(this.label1);
            this.Name = "frmLambert";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmLambert";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label lblEpoch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox r1;
        private System.Windows.Forms.TextBox r2;
        private System.Windows.Forms.TextBox v1tS;
        private System.Windows.Forms.TextBox v2tS;
        private System.Windows.Forms.TextBox dtsec;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox v1;
        private System.Windows.Forms.TextBox v2;
        private System.Windows.Forms.TextBox nrev;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox v2tL;
        private System.Windows.Forms.TextBox v1tL;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox v2tSL;
        private System.Windows.Forms.TextBox v1tSL;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox v2tSH;
        private System.Windows.Forms.TextBox v1tSH;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox v2tLL;
        private System.Windows.Forms.TextBox v1tLL;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox v2tLH;
        private System.Windows.Forms.TextBox v1tLH;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox cbLambertList;
        private System.Windows.Forms.TextBox hitearthS;
        private System.Windows.Forms.TextBox hitearthL;
        private System.Windows.Forms.TextBox hitearthSL;
        private System.Windows.Forms.TextBox hitearthSH;
        private System.Windows.Forms.TextBox hitearthLL;
        private System.Windows.Forms.TextBox hitearthLH;
        private System.Windows.Forms.TextBox dtwait;
        private System.Windows.Forms.TextBox aeiS;
        private System.Windows.Forms.TextBox aeiL;
        private System.Windows.Forms.TextBox aeiSL;
        private System.Windows.Forms.TextBox aeiSH;
        private System.Windows.Forms.TextBox aeiLL;
        private System.Windows.Forms.TextBox aeiLH;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox storedCase;
        private System.Windows.Forms.Button button1;
    }
}