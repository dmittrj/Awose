using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Awose
{
    public partial class Awose : Form
    {
        List<AwoseAgent> agents = new();
        public Awose()
        {
            InitializeComponent();
        }

        private void SaveModel_MSItem_Click(object sender, EventArgs e)
        {
            if (SaveModel_SFD.ShowDialog() == DialogResult.OK)
            {

            }
        }
    }
}
