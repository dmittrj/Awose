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
            this.Main_MStr = new System.Windows.Forms.MenuStrip();
            this.Simulation_MSItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LaunchSimulation_MSItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pauseSimulationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopSimulationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.openModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveModelingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Main_MStr.SuspendLayout();
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
            this.Main_MStr.Size = new System.Drawing.Size(1055, 28);
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
            this.saveModelingToolStripMenuItem,
            this.toolStripMenuItem2,
            this.undoToolStripMenuItem,
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
            // saveModelingToolStripMenuItem
            // 
            this.saveModelingToolStripMenuItem.Name = "saveModelingToolStripMenuItem";
            this.saveModelingToolStripMenuItem.Size = new System.Drawing.Size(246, 24);
            this.saveModelingToolStripMenuItem.Text = "Save modeling...";
            // 
            // hToolStripMenuItem
            // 
            this.hToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.hToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.hToolStripMenuItem.Name = "hToolStripMenuItem";
            this.hToolStripMenuItem.Size = new System.Drawing.Size(71, 24);
            this.hToolStripMenuItem.Text = "Objects";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(243, 6);
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(246, 24);
            this.undoToolStripMenuItem.Text = "Undo";
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(246, 24);
            this.redoToolStripMenuItem.Text = "Redo";
            // 
            // Awose
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.ClientSize = new System.Drawing.Size(1055, 658);
            this.Controls.Add(this.Main_MStr);
            this.MainMenuStrip = this.Main_MStr;
            this.Name = "Awose";
            this.Text = "Awose";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Main_MStr.ResumeLayout(false);
            this.Main_MStr.PerformLayout();
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
        private System.Windows.Forms.ToolStripMenuItem saveModelingToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
    }
}

