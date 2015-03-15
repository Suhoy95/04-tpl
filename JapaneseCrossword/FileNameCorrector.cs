using System;
using System.IO;
using System.Text.RegularExpressions;
using NUnit;
using NUnit.Framework;

namespace JapaneseCrossword
{
    public class FileNameCorrecter
    {
        public static bool IsCorrectedFileName(string filename)
        {
            return filename.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;

        }
    }

    [TestFixture]
    public class FileNameCorrecterTest
    {
        [Test]
        public static void CorrectFileName()
        {
            Assert.AreEqual(FileNameCorrecter.IsCorrectedFileName("test.txt"), true);
        }

        [Test]
        public static void InCorrectFileName()
        {
            Assert.AreEqual(FileNameCorrecter.IsCorrectedFileName("///.&*#"), false);
        }
    }
}
