using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awose
{
    enum ChangeType { Creating, Deleting, ChangingMass}
    class AwoseChange
    {
        public AwoseAgent Subject { get; set; }
        public ChangeType Type { get; set; }
        public double OldValue { get; set; }
        public double NewValue { get; set; }
        public AwoseChange(AwoseAgent subject, ChangeType type)
        {
            Subject = subject;
            Type = type;
        }

        public AwoseChange(AwoseAgent subject, ChangeType type, double oldValue, double newValue) : this(subject, type)
        {
            Subject = subject;
            Type = type;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public override string ToString()
        {
            switch (Type)
            {
                case ChangeType.Creating:
                    return Subject.Name + " creation";
                case ChangeType.Deleting:
                    return Subject.Name + " deletion";
                case ChangeType.ChangingMass:
                    return Subject.Name + ": mass " + OldValue.ToString() + " -> " + NewValue.ToString();
                default:
                    return "";
            }
        }
    }
}
