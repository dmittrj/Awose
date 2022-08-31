using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awose
{
    enum GridColors { Blue, Pink }
    public enum StreamMode { None, Gravity, Electric}
    public class AwoseLayer
    {
        public string Name { get; set; }
        public List<AwoseAgent> Agents { get; set; }
        public int LayerNumber { get; set; }
        public SolidBrush GridColorMain { get; set; }
        public SolidBrush GridColorSub { get; set; }
        public AwoseLayer MotherLayer { get; set; }
        public float ELaw { get; set; }
        public float GLaw { get; set; }
        public int Selected { get; set; }

        public StreamMode StrMode { get; set; }

        public List<AwoseParticle> Sources = new();
        public int StreamFreq { get; set; }

        public AwoseLayer(string name, int layerNumber)
        {
            Name = name;
            Agents = new List<AwoseAgent>();
            LayerNumber = layerNumber;
            ELaw = -2;
            GLaw = -2;
            StrMode = StreamMode.None;
            StreamFreq = 200;
            switch ((layerNumber - 1) % 2)
            {
                case 0:
                    GridColorMain = new SolidBrush(Color.FromArgb(21, 47, 53));
                    GridColorSub = new SolidBrush(Color.FromArgb(19, 22, 23));
                    break;
                case 2:
                    GridColorMain = new SolidBrush(Color.FromArgb(53, 47, 21));
                    GridColorSub = new SolidBrush(Color.FromArgb(23, 22, 19));
                    break;
                default:
                    break;
            }
        }
    
        public bool IsThereSelections()
        {
            int x = 0;
            foreach (AwoseAgent item in Agents)
            {
                if (item.IsSelected) {
                    Selected = x;
                    return true; 
                }
                x++;
            }
            Selected = -1;
            return false;
        }
    }
}
