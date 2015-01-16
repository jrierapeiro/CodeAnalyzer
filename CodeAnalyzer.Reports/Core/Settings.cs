using System;
using System.IO;

namespace CodeAnalyzer.Reports.Core
{
    public static class Settings
    {
        public static readonly string TemplateFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");
    }
}