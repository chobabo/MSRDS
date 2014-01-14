namespace MazeExplorer
{
    partial class MazeForm
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
            this.components = new System.ComponentModel.Container();
            this.btnDrawMode = new System.Windows.Forms.Button();
            this.btnEditMode = new System.Windows.Forms.Button();
            this.btnClearMap = new System.Windows.Forms.Button();
            this.btnSaveMap = new System.Windows.Forms.Button();
            this.saveDlg = new System.Windows.Forms.SaveFileDialog();
            this.openDlg = new System.Windows.Forms.OpenFileDialog();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnLoadMap = new System.Windows.Forms.Button();
            this.btnSubmitMap = new System.Windows.Forms.Button();
            this.btnSetDrive = new System.Windows.Forms.Button();
            this.btnSetRobot = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnRobot = new System.Windows.Forms.Button();
            this.btnTarget = new System.Windows.Forms.Button();
            this.labelTimer = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Pos3 = new System.Windows.Forms.Button();
            this.Pos2 = new System.Windows.Forms.Button();
            this.Pos1 = new System.Windows.Forms.Button();
            this.YPos3 = new System.Windows.Forms.TextBox();
            this.XPos3 = new System.Windows.Forms.TextBox();
            this.YPos2 = new System.Windows.Forms.TextBox();
            this.XPos2 = new System.Windows.Forms.TextBox();
            this.YPos1 = new System.Windows.Forms.TextBox();
            this.XPos1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnRobot3 = new System.Windows.Forms.Button();
            this.btnRobot2 = new System.Windows.Forms.Button();
            this.r3PoseZ = new System.Windows.Forms.Label();
            this.r3PoseX = new System.Windows.Forms.Label();
            this.r2PoseZ = new System.Windows.Forms.Label();
            this.r2PoseX = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.r1PoseZ = new System.Windows.Forms.Label();
            this.r1PoseX = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.filterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.thresholdToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cannyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sobelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.laplaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.interestPointExtractionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sURFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trackerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dATABASEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnDrawMode
            // 
            this.btnDrawMode.Location = new System.Drawing.Point(6, 18);
            this.btnDrawMode.Name = "btnDrawMode";
            this.btnDrawMode.Size = new System.Drawing.Size(75, 23);
            this.btnDrawMode.TabIndex = 0;
            this.btnDrawMode.Text = "Draw Mode";
            this.btnDrawMode.UseVisualStyleBackColor = true;
            this.btnDrawMode.Click += new System.EventHandler(this.btnDrawMode_Click);
            // 
            // btnEditMode
            // 
            this.btnEditMode.Location = new System.Drawing.Point(96, 18);
            this.btnEditMode.Name = "btnEditMode";
            this.btnEditMode.Size = new System.Drawing.Size(75, 23);
            this.btnEditMode.TabIndex = 1;
            this.btnEditMode.Text = "Edit Mode";
            this.btnEditMode.UseVisualStyleBackColor = true;
            this.btnEditMode.Click += new System.EventHandler(this.btnEditMode_Click);
            // 
            // btnClearMap
            // 
            this.btnClearMap.Location = new System.Drawing.Point(96, 47);
            this.btnClearMap.Name = "btnClearMap";
            this.btnClearMap.Size = new System.Drawing.Size(75, 23);
            this.btnClearMap.TabIndex = 2;
            this.btnClearMap.Text = "Clear Map";
            this.btnClearMap.UseVisualStyleBackColor = true;
            this.btnClearMap.Click += new System.EventHandler(this.btnClearMap_Click);
            // 
            // btnSaveMap
            // 
            this.btnSaveMap.Location = new System.Drawing.Point(6, 76);
            this.btnSaveMap.Name = "btnSaveMap";
            this.btnSaveMap.Size = new System.Drawing.Size(75, 23);
            this.btnSaveMap.TabIndex = 3;
            this.btnSaveMap.Text = "Save Map";
            this.btnSaveMap.UseVisualStyleBackColor = true;
            this.btnSaveMap.Click += new System.EventHandler(this.btnSaveMap_Click);
            // 
            // openDlg
            // 
            this.openDlg.FileName = "openFileDialog1";
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnLoadMap
            // 
            this.btnLoadMap.Location = new System.Drawing.Point(96, 76);
            this.btnLoadMap.Name = "btnLoadMap";
            this.btnLoadMap.Size = new System.Drawing.Size(75, 23);
            this.btnLoadMap.TabIndex = 4;
            this.btnLoadMap.Text = "Load Map";
            this.btnLoadMap.UseVisualStyleBackColor = true;
            this.btnLoadMap.Click += new System.EventHandler(this.btnLoadMap_Click);
            // 
            // btnSubmitMap
            // 
            this.btnSubmitMap.Location = new System.Drawing.Point(6, 47);
            this.btnSubmitMap.Name = "btnSubmitMap";
            this.btnSubmitMap.Size = new System.Drawing.Size(75, 23);
            this.btnSubmitMap.TabIndex = 5;
            this.btnSubmitMap.Text = "Build Map";
            this.btnSubmitMap.UseVisualStyleBackColor = true;
            this.btnSubmitMap.Click += new System.EventHandler(this.btnSubmitMap_Click);
            // 
            // btnSetDrive
            // 
            this.btnSetDrive.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnSetDrive.ForeColor = System.Drawing.Color.MidnightBlue;
            this.btnSetDrive.Location = new System.Drawing.Point(537, 563);
            this.btnSetDrive.Name = "btnSetDrive";
            this.btnSetDrive.Size = new System.Drawing.Size(255, 23);
            this.btnSetDrive.TabIndex = 6;
            this.btnSetDrive.Text = "4. START";
            this.btnSetDrive.UseVisualStyleBackColor = true;
            this.btnSetDrive.Click += new System.EventHandler(this.btnSetDrive_Click);
            // 
            // btnSetRobot
            // 
            this.btnSetRobot.Location = new System.Drawing.Point(6, 144);
            this.btnSetRobot.Name = "btnSetRobot";
            this.btnSetRobot.Size = new System.Drawing.Size(75, 23);
            this.btnSetRobot.TabIndex = 7;
            this.btnSetRobot.Text = "Reset";
            this.btnSetRobot.UseVisualStyleBackColor = true;
            this.btnSetRobot.Click += new System.EventHandler(this.btnSetRobot_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(96, 144);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 8;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnRobot
            // 
            this.btnRobot.Location = new System.Drawing.Point(6, 28);
            this.btnRobot.Name = "btnRobot";
            this.btnRobot.Size = new System.Drawing.Size(75, 23);
            this.btnRobot.TabIndex = 9;
            this.btnRobot.Text = "Mos Pose 1";
            this.btnRobot.UseVisualStyleBackColor = true;
            this.btnRobot.Click += new System.EventHandler(this.btnRobot_Click);
            // 
            // btnTarget
            // 
            this.btnTarget.Location = new System.Drawing.Point(6, 115);
            this.btnTarget.Name = "btnTarget";
            this.btnTarget.Size = new System.Drawing.Size(75, 23);
            this.btnTarget.TabIndex = 10;
            this.btnTarget.Text = "Target Pose";
            this.btnTarget.UseVisualStyleBackColor = true;
            this.btnTarget.Click += new System.EventHandler(this.btnTarget_Click);
            // 
            // labelTimer
            // 
            this.labelTimer.AutoSize = true;
            this.labelTimer.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelTimer.ForeColor = System.Drawing.Color.Red;
            this.labelTimer.Location = new System.Drawing.Point(6, 17);
            this.labelTimer.Name = "labelTimer";
            this.labelTimer.Size = new System.Drawing.Size(79, 16);
            this.labelTimer.TabIndex = 13;
            this.labelTimer.Text = "00:00.000";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnDrawMode);
            this.groupBox1.Controls.Add(this.btnEditMode);
            this.groupBox1.Controls.Add(this.btnClearMap);
            this.groupBox1.Controls.Add(this.btnSubmitMap);
            this.groupBox1.Controls.Add(this.btnSaveMap);
            this.groupBox1.Controls.Add(this.btnLoadMap);
            this.groupBox1.Location = new System.Drawing.Point(537, 211);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(255, 116);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "2. Map Drawing";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.Pos3);
            this.groupBox2.Controls.Add(this.Pos2);
            this.groupBox2.Controls.Add(this.Pos1);
            this.groupBox2.Controls.Add(this.YPos3);
            this.groupBox2.Controls.Add(this.XPos3);
            this.groupBox2.Controls.Add(this.YPos2);
            this.groupBox2.Controls.Add(this.XPos2);
            this.groupBox2.Controls.Add(this.YPos1);
            this.groupBox2.Controls.Add(this.XPos1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.btnRobot3);
            this.groupBox2.Controls.Add(this.btnRobot2);
            this.groupBox2.Controls.Add(this.btnRobot);
            this.groupBox2.Controls.Add(this.btnTarget);
            this.groupBox2.Controls.Add(this.btnSetRobot);
            this.groupBox2.Controls.Add(this.btnStop);
            this.groupBox2.Location = new System.Drawing.Point(537, 29);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(255, 176);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "1. Initial Position Setting";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(94, 120);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(144, 12);
            this.label3.TabIndex = 41;
            this.label3.Text = "Pos(m) = (value*0.02f)-5.0f";
            // 
            // Pos3
            // 
            this.Pos3.Location = new System.Drawing.Point(208, 84);
            this.Pos3.Name = "Pos3";
            this.Pos3.Size = new System.Drawing.Size(41, 23);
            this.Pos3.TabIndex = 40;
            this.Pos3.Text = "SET";
            this.Pos3.UseVisualStyleBackColor = true;
            this.Pos3.Click += new System.EventHandler(this.Pos3_Click);
            // 
            // Pos2
            // 
            this.Pos2.Location = new System.Drawing.Point(208, 57);
            this.Pos2.Name = "Pos2";
            this.Pos2.Size = new System.Drawing.Size(41, 23);
            this.Pos2.TabIndex = 39;
            this.Pos2.Text = "SET";
            this.Pos2.UseVisualStyleBackColor = true;
            this.Pos2.Click += new System.EventHandler(this.Pos2_Click);
            // 
            // Pos1
            // 
            this.Pos1.Location = new System.Drawing.Point(207, 28);
            this.Pos1.Name = "Pos1";
            this.Pos1.Size = new System.Drawing.Size(42, 23);
            this.Pos1.TabIndex = 23;
            this.Pos1.Text = "SET";
            this.Pos1.UseVisualStyleBackColor = true;
            this.Pos1.Click += new System.EventHandler(this.Pos1_Click);
            // 
            // YPos3
            // 
            this.YPos3.Location = new System.Drawing.Point(152, 88);
            this.YPos3.Name = "YPos3";
            this.YPos3.Size = new System.Drawing.Size(50, 19);
            this.YPos3.TabIndex = 38;
            this.YPos3.Text = "300";
            // 
            // XPos3
            // 
            this.XPos3.Location = new System.Drawing.Point(96, 88);
            this.XPos3.Name = "XPos3";
            this.XPos3.Size = new System.Drawing.Size(50, 19);
            this.XPos3.TabIndex = 37;
            this.XPos3.Text = "400";
            // 
            // YPos2
            // 
            this.YPos2.Location = new System.Drawing.Point(152, 59);
            this.YPos2.Name = "YPos2";
            this.YPos2.Size = new System.Drawing.Size(50, 19);
            this.YPos2.TabIndex = 36;
            this.YPos2.Text = "300";
            // 
            // XPos2
            // 
            this.XPos2.Location = new System.Drawing.Point(96, 59);
            this.XPos2.Name = "XPos2";
            this.XPos2.Size = new System.Drawing.Size(50, 19);
            this.XPos2.TabIndex = 35;
            this.XPos2.Text = "350";
            // 
            // YPos1
            // 
            this.YPos1.Location = new System.Drawing.Point(152, 30);
            this.YPos1.Name = "YPos1";
            this.YPos1.Size = new System.Drawing.Size(50, 19);
            this.YPos1.TabIndex = 34;
            this.YPos1.Text = "300";
            // 
            // XPos1
            // 
            this.XPos1.Location = new System.Drawing.Point(96, 30);
            this.XPos1.Name = "XPos1";
            this.XPos1.Size = new System.Drawing.Size(50, 19);
            this.XPos1.TabIndex = 33;
            this.XPos1.Text = "300";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(150, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 12);
            this.label2.TabIndex = 32;
            this.label2.Text = "ZPos";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(94, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 12);
            this.label1.TabIndex = 31;
            this.label1.Text = "XPos";
            // 
            // btnRobot3
            // 
            this.btnRobot3.Location = new System.Drawing.Point(6, 86);
            this.btnRobot3.Name = "btnRobot3";
            this.btnRobot3.Size = new System.Drawing.Size(75, 23);
            this.btnRobot3.TabIndex = 30;
            this.btnRobot3.Text = "Mos Pose 3";
            this.btnRobot3.UseVisualStyleBackColor = true;
            this.btnRobot3.Click += new System.EventHandler(this.btnRobot3_Click);
            // 
            // btnRobot2
            // 
            this.btnRobot2.Location = new System.Drawing.Point(6, 57);
            this.btnRobot2.Name = "btnRobot2";
            this.btnRobot2.Size = new System.Drawing.Size(75, 23);
            this.btnRobot2.TabIndex = 29;
            this.btnRobot2.Text = "Mos Pose 2";
            this.btnRobot2.UseVisualStyleBackColor = true;
            this.btnRobot2.Click += new System.EventHandler(this.btnRobot2_Click);
            // 
            // r3PoseZ
            // 
            this.r3PoseZ.AutoSize = true;
            this.r3PoseZ.Location = new System.Drawing.Point(32, 149);
            this.r3PoseZ.Name = "r3PoseZ";
            this.r3PoseZ.Size = new System.Drawing.Size(36, 12);
            this.r3PoseZ.TabIndex = 28;
            this.r3PoseZ.Text = "Zpose";
            // 
            // r3PoseX
            // 
            this.r3PoseX.AutoSize = true;
            this.r3PoseX.Location = new System.Drawing.Point(32, 128);
            this.r3PoseX.Name = "r3PoseX";
            this.r3PoseX.Size = new System.Drawing.Size(36, 12);
            this.r3PoseX.TabIndex = 27;
            this.r3PoseX.Text = "Xpose";
            // 
            // r2PoseZ
            // 
            this.r2PoseZ.AutoSize = true;
            this.r2PoseZ.Location = new System.Drawing.Point(32, 101);
            this.r2PoseZ.Name = "r2PoseZ";
            this.r2PoseZ.Size = new System.Drawing.Size(36, 12);
            this.r2PoseZ.TabIndex = 26;
            this.r2PoseZ.Text = "Zpose";
            // 
            // r2PoseX
            // 
            this.r2PoseX.AutoSize = true;
            this.r2PoseX.Location = new System.Drawing.Point(32, 80);
            this.r2PoseX.Name = "r2PoseX";
            this.r2PoseX.Size = new System.Drawing.Size(36, 12);
            this.r2PoseX.TabIndex = 25;
            this.r2PoseX.Text = "Xpose";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 128);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(19, 12);
            this.label7.TabIndex = 24;
            this.label7.Text = "R3";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 80);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(19, 12);
            this.label6.TabIndex = 23;
            this.label6.Text = "R2";
            // 
            // r1PoseZ
            // 
            this.r1PoseZ.AutoSize = true;
            this.r1PoseZ.Location = new System.Drawing.Point(32, 59);
            this.r1PoseZ.Name = "r1PoseZ";
            this.r1PoseZ.Size = new System.Drawing.Size(36, 12);
            this.r1PoseZ.TabIndex = 22;
            this.r1PoseZ.Text = "Zpose";
            // 
            // r1PoseX
            // 
            this.r1PoseX.AutoSize = true;
            this.r1PoseX.Location = new System.Drawing.Point(32, 38);
            this.r1PoseX.Name = "r1PoseX";
            this.r1PoseX.Size = new System.Drawing.Size(36, 12);
            this.r1PoseX.TabIndex = 21;
            this.r1PoseX.Text = "Xpose";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 38);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(19, 12);
            this.label5.TabIndex = 20;
            this.label5.Text = "R1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(121, 12);
            this.label4.TabIndex = 19;
            this.label4.Text = "World Coordinate Pose";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.r3PoseZ);
            this.groupBox4.Controls.Add(this.r1PoseX);
            this.groupBox4.Controls.Add(this.r3PoseX);
            this.groupBox4.Controls.Add(this.r1PoseZ);
            this.groupBox4.Controls.Add(this.r2PoseZ);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.r2PoseX);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Location = new System.Drawing.Point(537, 380);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(255, 170);
            this.groupBox4.TabIndex = 19;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Coordinate Position";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.labelTimer);
            this.groupBox3.Location = new System.Drawing.Point(537, 333);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(255, 41);
            this.groupBox3.TabIndex = 21;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Elapsed Time";
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.YellowGreen;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filterToolStripMenuItem,
            this.interestPointExtractionToolStripMenuItem,
            this.trackerToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(806, 26);
            this.menuStrip1.TabIndex = 22;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // filterToolStripMenuItem
            // 
            this.filterToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.grayToolStripMenuItem,
            this.thresholdToolStripMenuItem,
            this.cannyToolStripMenuItem,
            this.sobelToolStripMenuItem,
            this.laplaceToolStripMenuItem});
            this.filterToolStripMenuItem.Name = "filterToolStripMenuItem";
            this.filterToolStripMenuItem.Size = new System.Drawing.Size(65, 22);
            this.filterToolStripMenuItem.Text = "3. Filter";
            // 
            // grayToolStripMenuItem
            // 
            this.grayToolStripMenuItem.Name = "grayToolStripMenuItem";
            this.grayToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.grayToolStripMenuItem.Text = "Gray";
            this.grayToolStripMenuItem.Click += new System.EventHandler(this.grayToolStripMenuItem_Click);
            // 
            // thresholdToolStripMenuItem
            // 
            this.thresholdToolStripMenuItem.Name = "thresholdToolStripMenuItem";
            this.thresholdToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.thresholdToolStripMenuItem.Text = "Threshold";
            this.thresholdToolStripMenuItem.Click += new System.EventHandler(this.thresholdToolStripMenuItem_Click);
            // 
            // cannyToolStripMenuItem
            // 
            this.cannyToolStripMenuItem.Name = "cannyToolStripMenuItem";
            this.cannyToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.cannyToolStripMenuItem.Text = "Canny";
            this.cannyToolStripMenuItem.Click += new System.EventHandler(this.cannyToolStripMenuItem_Click);
            // 
            // sobelToolStripMenuItem
            // 
            this.sobelToolStripMenuItem.Name = "sobelToolStripMenuItem";
            this.sobelToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.sobelToolStripMenuItem.Text = "Sobel";
            this.sobelToolStripMenuItem.Click += new System.EventHandler(this.sobelToolStripMenuItem_Click);
            // 
            // laplaceToolStripMenuItem
            // 
            this.laplaceToolStripMenuItem.Name = "laplaceToolStripMenuItem";
            this.laplaceToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.laplaceToolStripMenuItem.Text = "Laplace";
            this.laplaceToolStripMenuItem.Click += new System.EventHandler(this.laplaceToolStripMenuItem_Click);
            // 
            // interestPointExtractionToolStripMenuItem
            // 
            this.interestPointExtractionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sURFToolStripMenuItem,
            this.dATABASEToolStripMenuItem});
            this.interestPointExtractionToolStripMenuItem.Name = "interestPointExtractionToolStripMenuItem";
            this.interestPointExtractionToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.interestPointExtractionToolStripMenuItem.Text = "3. Interest Point Extraction";
            // 
            // sURFToolStripMenuItem
            // 
            this.sURFToolStripMenuItem.Name = "sURFToolStripMenuItem";
            this.sURFToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.sURFToolStripMenuItem.Text = "SURF";
            this.sURFToolStripMenuItem.Click += new System.EventHandler(this.sURFToolStripMenuItem_Click);
            // 
            // trackerToolStripMenuItem
            // 
            this.trackerToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.trackerToolStripMenuItem.Name = "trackerToolStripMenuItem";
            this.trackerToolStripMenuItem.Size = new System.Drawing.Size(79, 22);
            this.trackerToolStripMenuItem.Text = "5. Tracker";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // dATABASEToolStripMenuItem
            // 
            this.dATABASEToolStripMenuItem.Name = "dATABASEToolStripMenuItem";
            this.dATABASEToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.dATABASEToolStripMenuItem.Text = "DATABASE";
            this.dATABASEToolStripMenuItem.Click += new System.EventHandler(this.dATABASEToolStripMenuItem_Click);
            // 
            // MazeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(806, 598);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnSetDrive);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MazeForm";
            this.Text = "Interface of Setting of the Map and Robot";
            this.Load += new System.EventHandler(this.MazeForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDrawMode;
        private System.Windows.Forms.Button btnEditMode;
        private System.Windows.Forms.Button btnClearMap;
        private System.Windows.Forms.Button btnSaveMap;
        private System.Windows.Forms.SaveFileDialog saveDlg;
        private System.Windows.Forms.OpenFileDialog openDlg;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnLoadMap;
        private System.Windows.Forms.Button btnSubmitMap;
        private System.Windows.Forms.Button btnSetDrive;
        private System.Windows.Forms.Button btnSetRobot;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnRobot;
        private System.Windows.Forms.Button btnTarget;
        private System.Windows.Forms.Label labelTimer;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label r1PoseZ;
        private System.Windows.Forms.Label r1PoseX;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label r2PoseX;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label r3PoseZ;
        private System.Windows.Forms.Label r3PoseX;
        private System.Windows.Forms.Label r2PoseZ;
        private System.Windows.Forms.Button btnRobot3;
        private System.Windows.Forms.Button btnRobot2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem trackerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem grayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem thresholdToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cannyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sobelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem laplaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem interestPointExtractionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sURFToolStripMenuItem;
        private System.Windows.Forms.Button Pos2;
        private System.Windows.Forms.Button Pos1;
        private System.Windows.Forms.TextBox YPos3;
        private System.Windows.Forms.TextBox XPos3;
        private System.Windows.Forms.TextBox YPos2;
        private System.Windows.Forms.TextBox XPos2;
        private System.Windows.Forms.TextBox YPos1;
        private System.Windows.Forms.TextBox XPos1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Pos3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStripMenuItem dATABASEToolStripMenuItem;
    }
}