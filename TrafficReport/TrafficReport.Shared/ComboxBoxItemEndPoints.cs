using System;
using System.Collections.Generic;
using System.Text;

namespace TrafficReport
{
    class ComboxBoxItemEndPoints
    {
        public string Id { get; set; }
        public string Value { get; set; }

        public ComboxBoxItemEndPoints(string id, string value)
        {
            Id = id;
            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
