namespace TestAllTool
{
    partial class Form1
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
            this.button4 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.OFLoc = new System.Windows.Forms.TextBox();
            this.opsStatus = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.testNum = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.time2 = new System.Windows.Forms.TextBox();
            this.doy = new System.Windows.Forms.TextBox();
            this.decl = new System.Windows.Forms.TextBox();
            this.dms = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.hms = new System.Windows.Forms.TextBox();
            this.rtasc = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.button4.Location = new System.Drawing.Point(497, 252);
            this.button4.Margin = new System.Windows.Forms.Padding(6);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(281, 42);
            this.button4.TabIndex = 289;
            this.button4.Text = "Run Tests";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click_1);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(576, 54);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(241, 33);
            this.label2.TabIndex = 288;
            this.label2.Text = "Test All Summary";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(174, 102);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(219, 17);
            this.label1.TabIndex = 287;
            this.label1.Text = "Program to generate debris fields";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(193, 150);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(172, 25);
            this.label12.TabIndex = 286;
            this.label12.Text = "Output file location";
            // 
            // OFLoc
            // 
            this.OFLoc.Location = new System.Drawing.Point(198, 179);
            this.OFLoc.Margin = new System.Windows.Forms.Padding(4);
            this.OFLoc.Name = "OFLoc";
            this.OFLoc.Size = new System.Drawing.Size(1036, 29);
            this.OFLoc.TabIndex = 285;
            this.OFLoc.Text = "D:\\POE\\TestSLR\\Automation\\CurrentScenarioLEOGrace\\";
            // 
            // opsStatus
            // 
            this.opsStatus.AutoSize = true;
            this.opsStatus.Location = new System.Drawing.Point(182, 663);
            this.opsStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.opsStatus.Name = "opsStatus";
            this.opsStatus.Size = new System.Drawing.Size(79, 25);
            this.opsStatus.TabIndex = 290;
            this.opsStatus.Text = "Status: ";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.button1.Location = new System.Drawing.Point(497, 318);
            this.button1.Margin = new System.Windows.Forms.Padding(6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(281, 42);
            this.button1.TabIndex = 291;
            this.button1.Text = "Run Lambert Form";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.button2.Location = new System.Drawing.Point(669, 597);
            this.button2.Margin = new System.Windows.Forms.Padding(6);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(281, 42);
            this.button2.TabIndex = 292;
            this.button2.Text = "Run Test #";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // testNum
            // 
            this.testNum.Location = new System.Drawing.Point(993, 603);
            this.testNum.Margin = new System.Windows.Forms.Padding(4);
            this.testNum.Name = "testNum";
            this.testNum.Size = new System.Drawing.Size(197, 29);
            this.testNum.TabIndex = 355;
            this.testNum.Text = "94";
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.button3.Location = new System.Drawing.Point(497, 386);
            this.button3.Margin = new System.Windows.Forms.Padding(6);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(281, 42);
            this.button3.TabIndex = 356;
            this.button3.Text = "Run Gauss Form";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // time2
            // 
            this.time2.Location = new System.Drawing.Point(216, 437);
            this.time2.Name = "time2";
            this.time2.Size = new System.Drawing.Size(324, 29);
            this.time2.TabIndex = 361;
            this.time2.Text = "16 Sep 2020 14:7:1.625";
            this.time2.TextChanged += new System.EventHandler(this.time2_TextChanged);
            // 
            // doy
            // 
            this.doy.Location = new System.Drawing.Point(560, 437);
            this.doy.Name = "doy";
            this.doy.Size = new System.Drawing.Size(55, 29);
            this.doy.TabIndex = 362;
            this.doy.Text = "260";
            // 
            // decl
            // 
            this.decl.Location = new System.Drawing.Point(205, 524);
            this.decl.Name = "decl";
            this.decl.Size = new System.Drawing.Size(167, 29);
            this.decl.TabIndex = 363;
            this.decl.Text = "97.23485765";
            this.decl.TextChanged += new System.EventHandler(this.decl_TextChanged);
            // 
            // dms
            // 
            this.dms.Location = new System.Drawing.Point(443, 524);
            this.dms.Name = "dms";
            this.dms.Size = new System.Drawing.Size(201, 29);
            this.dms.TabIndex = 366;
            this.dms.Text = "97 14 25.43";
            this.dms.TextChanged += new System.EventHandler(this.dms_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(379, 524);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 25);
            this.label3.TabIndex = 367;
            this.label3.Text = "deg";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(664, 524);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 25);
            this.label4.TabIndex = 368;
            this.label4.Text = "DMS";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(664, 479);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 25);
            this.label5.TabIndex = 372;
            this.label5.Text = "HMS";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(379, 479);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(45, 25);
            this.label6.TabIndex = 371;
            this.label6.Text = "deg";
            // 
            // hms
            // 
            this.hms.Location = new System.Drawing.Point(443, 479);
            this.hms.Name = "hms";
            this.hms.Size = new System.Drawing.Size(201, 29);
            this.hms.TabIndex = 370;
            this.hms.Text = "15 15 53.630";
            this.hms.TextChanged += new System.EventHandler(this.hms_TextChanged);
            // 
            // rtasc
            // 
            this.rtasc.Location = new System.Drawing.Point(205, 479);
            this.rtasc.Name = "rtasc";
            this.rtasc.Size = new System.Drawing.Size(167, 29);
            this.rtasc.TabIndex = 369;
            this.rtasc.Text = "228.9735";
            this.rtasc.TextChanged += new System.EventHandler(this.rtasc_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(139, 524);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(48, 25);
            this.label7.TabIndex = 373;
            this.label7.Text = "decl";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(139, 483);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(54, 25);
            this.label8.TabIndex = 374;
            this.label8.Text = "rtasc";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1467, 831);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.hms);
            this.Controls.Add(this.rtasc);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dms);
            this.Controls.Add(this.decl);
            this.Controls.Add(this.doy);
            this.Controls.Add(this.time2);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.testNum);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.opsStatus);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.OFLoc);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox OFLoc;
        private System.Windows.Forms.Label opsStatus;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox testNum;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox time2;
        private System.Windows.Forms.TextBox doy;
        private System.Windows.Forms.TextBox decl;
        private System.Windows.Forms.TextBox dms;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox hms;
        private System.Windows.Forms.TextBox rtasc;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
    }
}

