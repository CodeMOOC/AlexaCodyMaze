using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot {

    public static class IntentTypes {

        public const string ReachCell = nameof(ReachCell);
        public const string GiveDirection = nameof(GiveDirection);

        public const string Cancel = "AMAZON.CancelIntent";
        public const string Help = "AMAZON.HelpIntent";
        public const string Stop = "AMAZON.StopIntent";

    }

}
