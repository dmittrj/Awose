using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awose
{
    enum ChangeType { Creating, Deleting}
    class AwoseChange
    {
        AwoseAgent Subject { get; set; }
        ChangeType Type { get; set; }
        public AwoseChange(AwoseAgent subject, ChangeType type)
        {
            Subject = subject;
            Type = type;
        }
        public override string ToString()
        {
            switch (Type)
            {
                case ChangeType.Creating:
                    return Subject.Name + " creation";
                case ChangeType.Deleting:
                    return Subject.Name + " deletion";
                default:
                    return "";
            }
        }
    }
}
