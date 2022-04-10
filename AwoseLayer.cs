using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awose
{
    class AwoseLayer
    {
        List<AwoseAgent> agents { get; set; }
        public AwoseLayer()
        {
            agents = new List<AwoseAgent>();
        }
    }
}
