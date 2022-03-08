
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
            this.SaveModel_MSItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Control_Panel = new System.Windows.Forms.Panel();
            this.SaveModel_SFD = new System.Windows.Forms.SaveFileDialog();
            this.ModelBoard_PB = new System.Windows.Forms.PictureBox();
            this.Main_MStr.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ModelBoard_PB)).BeginInit();
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
            this.ModelBoard_PB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.ModelBoard_PB.Location = new System.Drawing.Point(248, 28);
            this.ModelBoard_PB.Name = "ModelBoard_PB";
            this.ModelBoard_PB.Size = new System.Drawing.Size(993, 686);
            this.ModelBoard_PB.TabIndex = 2;
            this.ModelBoard_PB.TabStop = false;
            // 
            // Awose
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.ClientSize = new System.Drawing.Size(1241, 711);
            this.Controls.Add(this.ModelBoard_PB);
            this.Controls.Add(this.Control_Panel);
            this.Controls.Add(this.Main_MStr);
            this.MainMenuStrip = this.Main_MStr;
            this.Name = "Awose";
            this.Text = "Awose";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Main_MStr.ResumeLayout(false);
            this.Main_MStr.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ModelBoard_PB)).EndInit();
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
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.Panel Control_Panel;
        private System.Windows.Forms.SaveFileDialog SaveModel_SFD;
        private System.Windows.Forms.PictureBox ModelBoard_PB;
    }
}

