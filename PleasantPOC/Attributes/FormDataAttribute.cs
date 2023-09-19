using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PleasantPOC.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FormDataAttribute : Attribute
    {
        public string Name { get; }
        public FormDataAttribute(string name)
        {
            Name = name;
        }
    }
}
