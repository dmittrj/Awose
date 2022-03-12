﻿
namespace Awose
{
    partial class Awose
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Main_MStr = new System.Windows.Forms.MenuStrip();
            this.Simulation_MSItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LaunchSimulation_MSItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pauseSimulationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopSimulationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.openModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveModel_MSItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.Undo_MSItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Control_Panel = new System.Windows.Forms.Panel();
            this.SaveModel_SFD = new System.Windows.Forms.SaveFileDialog();
            this.ModelBoard_PB = new System.Windows.Forms.PictureBox();
            this.Space_CMStr = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Mistake_CMItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SepMistake_CMSepar = new System.Windows.Forms.ToolStripSeparator();
            this.CreateObject_CMItem = new System.Windows.Forms.ToolStripMenuItem();
            this.presetsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.starToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.smallPlanetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bigPlanetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.blackHoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chargesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.positiveChargeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.negativeChargeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Instrumental_Panel = new System.Windows.Forms.Panel();
            this.Mistake_Panel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.MistakeIcon_PB = new System.Windows.Forms.PictureBox();
            this.Main_MStr.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ModelBoard_PB)).BeginInit();
            this.Space_CMStr.SuspendLayout();
            this.Mistake_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MistakeIcon_PB)).BeginInit();
            this.SuspendLayout();
            // 
            // Main_MStr
            // 
            this.Main_MStr.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(11)))), ((int)(((byte)(11)))));
            this.Main_MStr.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Simulation_MSItem,
            this.hToolStripMenuItem});
            this.Main_MStr.Location = new System.Drawing.Point(0, 0);
            this.Main_MStr.Name = "Main_MStr";
            this.Main_MStr.Size = new System.Drawing.Size(1241, 28);
            this.Main_MStr.TabIndex = 0;
            this.Main_MStr.Text = "menuStrip1";
            // 
            // Simulation_MSItem
            // 
            this.Simulation_MSItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LaunchSimulation_MSItem,
            this.pauseSimulationToolStripMenuItem,
            this.stopSimulationToolStripMenuItem,
            this.resetToolStripMenuItem,
            this.toolStripMenuItem1,
            this.openModelToolStripMenuItem,
            this.SaveModel_MSItem,
            this.toolStripMenuItem2,
            this.Undo_MSItem,
            this.redoToolStripMenuItem});
            this.Simulation_MSItem.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Simulation_MSItem.ForeColor = System.Drawing.Color.White;
            this.Simulation_MSItem.Name = "Simulation_MSItem";
            this.Simulation_MSItem.Size = new System.Drawing.Size(92, 24);
            this.Simulation_MSItem.Text = "Simulation";
            // 
            // LaunchSimulation_MSItem
            // 
            this.LaunchSimulation_MSItem.BackColor = System.Drawing.Color.White;
            this.LaunchSimulation_MSItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.LaunchSimulation_MSItem.ForeColor = System.Drawing.Color.Black;
            this.LaunchSimulation_MSItem.Name = "LaunchSimulation_MSItem";
            this.LaunchSimulation_MSItem.Padding = new System.Windows.Forms.Padding(0);
            this.LaunchSimulation_MSItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.LaunchSimulation_MSItem.Size = new System.Drawing.Size(246, 22);
            this.LaunchSimulation_MSItem.Text = "Launch simulation";
            this.LaunchSimulation_MSItem.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            // 
            // pauseSimulationToolStripMenuItem
            // 
            this.pauseSimulationToolStripMenuItem.BackColor = System.Drawing.Color.White;
            this.pauseSimulationToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.pauseSimulationToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.pauseSimulationToolStripMenuItem.Name = "pauseSimulationToolStripMenuItem";
            this.pauseSimulationToolStripMenuItem.Size = new System.Drawing.Size(246, 24);
            this.pauseSimulationToolStripMenuItem.Text = "Pause simulation";
            this.pauseSimulationToolStripMenuItem.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            // 
            // stopSimulationToolStripMenuItem
            // 
            this.stopSimulationToolStripMenuItem.Name = "stopSimulationToolStripMenuItem";
            this.stopSimulationToolStripMenuItem.Size = new System.Drawing.Size(246, 24);
            this.stopSimulationToolStripMenuItem.Text = "Stop simulation";
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(246, 24);
            this.resetToolStripMenuItem.Text = "Reset";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(243, 6);
            // 
            // openModelToolStripMenuItem
            // 
            this.openModelToolStripMenuItem.Name = "openModelToolStripMenuItem";
            this.openModelToolStripMenuItem.Size = new System.Drawing.Size(246, 24);
            this.openModelToolStripMenuItem.Text = "Open modeling...";
            // 
            // SaveModel_MSItem
            // 
            this.SaveModel_MSItem.Name = "SaveModel_MSItem";
            this.SaveModel_MSItem.Size = new System.Drawing.Size(246, 24);
            this.SaveModel_MSItem.Text = "Save modeling...";
            this.SaveModel_MSItem.Click += new System.EventHandler(this.SaveModel_MSItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(243, 6);
            // 
            // Undo_MSItem
            // 
            this.Undo_MSItem.Name = "Undo_MSItem";
            this.Undo_MSItem.Size = new System.Drawing.Size(246, 24);
            this.Undo_MSItem.Text = "Undo";
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(246, 24);
            this.redoToolStripMenuItem.Text = "Redo";
            // 
            // hToolStripMenuItem
            // 
            this.hToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.hToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.hToolStripMenuItem.Name = "hToolStripMenuItem";
            this.hToolStripMenuItem.Size = new System.Drawing.Size(71, 24);
            this.hToolStripMenuItem.Text = "Objects";
            // 
            // Control_Panel
            // 
            this.Control_Panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Control_Panel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.Control_Panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Control_Panel.Location = new System.Drawing.Point(0, 28);
            this.Control_Panel.Name = "Control_Panel";
            this.Control_Panel.Size = new System.Drawing.Size(248, 686);
            this.Control_Panel.TabIndex = 1;
            // 
            // SaveModel_SFD
            // 
            this.SaveModel_SFD.Filter = "Awose File|*.awose";
            this.SaveModel_SFD.Title = "Save modeling as...";
            // 
            // ModelBoard_PB
            // 
            this.ModelBoard_PB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ModelBoard_PB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.ModelBoard_PB.ContextMenuStrip = this.Space_CMStr;
            this.ModelBoard_PB.Location = new System.Drawing.Point(248, 28);
            this.ModelBoard_PB.Name = "ModelBoard_PB";
            this.ModelBoard_PB.Size = new System.Drawing.Size(993, 686);
            this.ModelBoard_PB.TabIndex = 2;
            this.ModelBoard_PB.TabStop = false;
            this.ModelBoard_PB.Click += new System.EventHandler(this.ModelBoard_PB_Click);
            this.ModelBoard_PB.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.ModelBoard_PB_MouseWheel);
            // 
            // Space_CMStr
            // 
            this.Space_CMStr.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Mistake_CMItem,
            this.SepMistake_CMSepar,
            this.CreateObject_CMItem,
            this.presetsToolStripMenuItem});
            this.Space_CMStr.Name = "Space_CMStr";
            this.Space_CMStr.Size = new System.Drawing.Size(158, 82);
            this.Space_CMStr.Opening += new System.ComponentModel.CancelEventHandler(this.Space_CMStr_Opening);
            // 
            // Mistake_CMItem
            // 
            this.Mistake_CMItem.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.Mistake_CMItem.Name = "Mistake_CMItem";
            this.Mistake_CMItem.Size = new System.Drawing.Size(157, 24);
            this.Mistake_CMItem.Text = "Warning";
            this.Mistake_CMItem.Visible = false;
            // 
            // SepMistake_CMSepar
            // 
            this.SepMistake_CMSepar.Name = "SepMistake_CMSepar";
            this.SepMistake_CMSepar.Size = new System.Drawing.Size(154, 6);
            this.SepMistake_CMSepar.Visible = false;
            // 
            // CreateObject_CMItem
            // 
            this.CreateObject_CMItem.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.CreateObject_CMItem.Name = "CreateObject_CMItem";
            this.CreateObject_CMItem.Size = new System.Drawing.Size(157, 24);
            this.CreateObject_CMItem.Text = "Create object";
            this.CreateObject_CMItem.Click += new System.EventHandler(this.CreateObject_CMItem_Click);
            // 
            // presetsToolStripMenuItem
            // 
            this.presetsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.spaceToolStripMenuItem,
            this.chargesToolStripMenuItem});
            this.presetsToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.presetsToolStripMenuItem.Name = "presetsToolStripMenuItem";
            this.presetsToolStripMenuItem.Size = new System.Drawing.Size(157, 24);
            this.presetsToolStripMenuItem.Text = "Presets";
            // 
            // spaceToolStripMenuItem
            // 
            this.spaceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.starToolStripMenuItem,
            this.smallPlanetToolStripMenuItem,
            this.bigPlanetToolStripMenuItem,
            this.blackHoleToolStripMenuItem});
            this.spaceToolStripMenuItem.Name = "spaceToolStripMenuItem";
            this.spaceToolStripMenuItem.Size = new System.Drawing.Size(128, 24);
            this.spaceToolStripMenuItem.Text = "Space";
            // 
            // starToolStripMenuItem
            // 
            this.starToolStripMenuItem.Name = "starToolStripMenuItem";
            this.starToolStripMenuItem.Size = new System.Drawing.Size(152, 24);
            this.starToolStripMenuItem.Text = "Star";
            // 
            // smallPlanetToolStripMenuItem
            // 
            this.smallPlanetToolStripMenuItem.Name = "smallPlanetToolStripMenuItem";
            this.smallPlanetToolStripMenuItem.Size = new System.Drawing.Size(152, 24);
            this.smallPlanetToolStripMenuItem.Text = "Small planet";
            // 
            // bigPlanetToolStripMenuItem
            // 
            this.bigPlanetToolStripMenuItem.Name = "bigPlanetToolStripMenuItem";
            this.bigPlanetToolStripMenuItem.Size = new System.Drawing.Size(152, 24);
            this.bigPlanetToolStripMenuItem.Text = "Big planet";
            // 
            // blackHoleToolStripMenuItem
            // 
            this.blackHoleToolStripMenuItem.Name = "blackHoleToolStripMenuItem";
            this.blackHoleToolStripMenuItem.Size = new System.Drawing.Size(152, 24);
            this.blackHoleToolStripMenuItem.Text = "Black hole";
            // 
            // chargesToolStripMenuItem
            // 
            this.chargesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.positiveChargeToolStripMenuItem,
            this.negativeChargeToolStripMenuItem});
            this.chargesToolStripMenuItem.Name = "chargesToolStripMenuItem";
            this.chargesToolStripMenuItem.Size = new System.Drawing.Size(128, 24);
            this.chargesToolStripMenuItem.Text = "Charges";
            // 
            // positiveChargeToolStripMenuItem
            // 
            this.positiveChargeToolStripMenuItem.Name = "positiveChargeToolStripMenuItem";
            this.positiveChargeToolStripMenuItem.Size = new System.Drawing.Size(177, 24);
            this.positiveChargeToolStripMenuItem.Text = "Positive charge";
            // 
            // negativeChargeToolStripMenuItem
            // 
            this.negativeChargeToolStripMenuItem.Name = "negativeChargeToolStripMenuItem";
            this.negativeChargeToolStripMenuItem.Size = new System.Drawing.Size(177, 24);
            this.negativeChargeToolStripMenuItem.Text = "Negative charge";
            // 
            // Instrumental_Panel
            // 
            this.Instrumental_Panel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.Instrumental_Panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Instrumental_Panel.Location = new System.Drawing.Point(254, 51);
            this.Instrumental_Panel.Name = "Instrumental_Panel";
            this.Instrumental_Panel.Size = new System.Drawing.Size(29, 142);
            this.Instrumental_Panel.TabIndex = 3;
            this.Instrumental_Panel.Visible = false;
            // 
            // Mistake_Panel
            // 
            this.Mistake_Panel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.Mistake_Panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Mistake_Panel.Controls.Add(this.label1);
            this.Mistake_Panel.Controls.Add(this.MistakeIcon_PB);
            this.Mistake_Panel.Location = new System.Drawing.Point(606, 284);
            this.Mistake_Panel.Name = "Mistake_Panel";
            this.Mistake_Panel.Size = new System.Drawing.Size(261, 69);
            this.Mistake_Panel.TabIndex = 4;
            this.Mistake_Panel.Visible = false;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(44, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(204, 55);
            this.label1.TabIndex = 1;
            this.label1.Text = "Useless object";
            // 
            // MistakeIcon_PB
            // 
            this.MistakeIcon_PB.Location = new System.Drawing.Point(8, 8);
            this.MistakeIcon_PB.Name = "MistakeIcon_PB";
            this.MistakeIcon_PB.Size = new System.Drawing.Size(32, 32);
            this.MistakeIcon_PB.TabIndex = 0;
            this.MistakeIcon_PB.TabStop = false;
            // 
            // Awose
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.ClientSize = new System.Drawing.Size(1241, 711);
            this.Controls.Add(this.Mistake_Panel);
            this.Controls.Add(this.Instrumental_Panel);
            this.Controls.Add(this.ModelBoard_PB);
            this.Controls.Add(this.Control_Panel);
            this.Controls.Add(this.Main_MStr);
            this.MainMenuStrip = this.Main_MStr;
            this.Name = "Awose";
            this.Text = "Awose";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Awose_Load);
            this.Main_MStr.ResumeLayout(false);
            this.Main_MStr.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ModelBoard_PB)).EndInit();
            this.Space_CMStr.ResumeLayout(false);
            this.Mistake_Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MistakeIcon_PB)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip Main_MStr;
        private System.Windows.Forms.ToolStripMenuItem Simulation_MSItem;
        private System.Windows.Forms.ToolStripMenuItem hToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LaunchSimulation_MSItem;
        private System.Windows.Forms.ToolStripMenuItem pauseSimulationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopSimulationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem openModelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveModel_MSItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem Undo_MSItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.Panel Control_Panel;
        private System.Windows.Forms.SaveFileDialog SaveModel_SFD;
        private System.Windows.Forms.PictureBox ModelBoard_PB;
        private System.Windows.Forms.ContextMenuStrip Space_CMStr;
        private System.Windows.Forms.ToolStripMenuItem CreateObject_CMItem;
        private System.Windows.Forms.ToolStripMenuItem presetsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem spaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chargesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem starToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem smallPlanetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bigPlanetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem blackHoleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem positiveChargeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem negativeChargeToolStripMenuItem;
        private System.Windows.Forms.Panel Instrumental_Panel;
        private System.Windows.Forms.Panel Mistake_Panel;
        private System.Windows.Forms.PictureBox MistakeIcon_PB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem Mistake_CMItem;
        private System.Windows.Forms.ToolStripSeparator SepMistake_CMSepar;
    }
}

