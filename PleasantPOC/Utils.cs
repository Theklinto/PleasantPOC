using PleasantPOC.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PleasantPOC
{
    public static partial class Utils
    {
        public const string TemplateFilePathKey = "TemplateFile";
        public const string FilePathKey = "FilePlacement";
        [GeneratedRegex("^#.*#$")]
        public static partial Regex IsValidKeyRegex();
        
    }

}
