using System;
using System.IO;
using System.Text.RegularExpressions;
using NUnit;
using NUnit.Framework;

namespace JapaneseCrossword
{
    public class FileNameCorrecter
    {
        private static Regex nameMask = new Regex(@"[^/&#*?<>|]+\.[^/&#*?<>|]+");
        private static Regex nameWithPath = new Regex(@"[^/&#*?<>|]+\\.+\.[^/&#*?<>|]+");
        public static bool IsCorrectedFileName(string filename)
        {
            return filename.IndexOfAny(Path.GetInvalidPathChars()) < 0 && 
                (nameMask.IsMatch(filename) || nameWithPath.IsMatch(filename));
        }
    }

    [TestFixture]
    public class FileNameCorrecterTest
    {
        [Test]
        public static void CorrectFileName()
        {
            Assert.AreEqual(true, FileNameCorrecter.IsCorrectedFileName("test.txt"));
        }

        [Test]
        public static void InCorrectFileName()
        {
            Assert.AreEqual(false, FileNameCorrecter.IsCorrectedFileName("///.&*#"));
        }
    }
}
