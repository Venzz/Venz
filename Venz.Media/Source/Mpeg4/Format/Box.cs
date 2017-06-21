using System;
using System.Collections.Generic;

namespace Venz.Media.Mpeg4
{
    public class Box
    {
        internal UInt32 Offset { get; }
        internal UInt32 Length { get; }
        internal ICollection<Box> Boxes { get; }

        public String Name { get; }
        public Boolean IsContainer => Boxes.Count > 0;



        public Box(UInt32 offset, UInt32 length, String name, IEnumerable<Box> subBoxes)
        {
            Offset = offset;
            Length = length;
            Name = name;
            Boxes = new List<Box>(subBoxes);
        }

        public String ToStringView(String indent = "")
        {
            var view = indent + ToString() + "\n";
            foreach (var box in Boxes)
                view += indent + box.ToStringView(indent + "    ");
            return view;
        }

        public override String ToString() => $"{Name}, {Length} bytes, {(IsContainer ? "CONTAINER" : "DATA")}";
    }
}
