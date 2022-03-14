using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awose
{
    enum ChangeType { Creating, Deleting, ChangingMass, 
        ChangingCharge, ChangingName, ChangingX, ChangingY}
    class AwoseChange
    {
        public AwoseAgent Subject { get; set; }
        public ChangeType Type { get; set; }
        public double OldValue { get; set; }
        public double NewValue { get; set; }
        public string OldStringValue { get; set; }
        public string NewStringValue { get; set; }
        public AwoseChange(AwoseAgent subject, ChangeType type)
        {
            Subject = subject;
            Type = type;
        }

        public AwoseChange(AwoseAgent subject, ChangeType type, double oldValue, double newValue)
        {
            Subject = subject;
            Type = type;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public AwoseChange(AwoseAgent subject, ChangeType type, string oldValue, string newValue)
        {
            Subject = subject;
            Type = type;
            OldStringValue = oldValue;
            NewStringValue = newValue;
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
                case ChangeType.ChangingCharge:
                    return Subject.Name + ": charge " + OldValue.ToString() + " -> " + NewValue.ToString();
                case ChangeType.ChangingName:
                    return "renaming " + OldStringValue + " to " + NewStringValue;
                case ChangeType.ChangingX:
                    return "X-axis movement of " + OldValue.ToString() + " -> " + NewValue.ToString();
                case ChangeType.ChangingY:
                    return "Y-axis movement of " + OldValue.ToString() + " -> " + NewValue.ToString();
                default:
                    return "";
            }
        }
    }
}
