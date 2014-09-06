using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wixosssimulator.components.card
{
    public class Cost
    {
        private Dictionary<Color, byte> value;

        public Dictionary<Color, byte> Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
    }
}