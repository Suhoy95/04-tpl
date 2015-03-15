using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JapaneseCrossword
{
    enum Cell
    {
        Empty, 
        Fuzzy,
        Shaded
    }

    class CrosswordField
    {
        public Cell[][] field;
        public int[][] vBlocks;
        public int[][] gBlocks;
        public int width;
        public int height;

        public CrosswordField(string filename)
        {
            var lines = File.ReadAllLines(filename);
            height =int.Parse(lines[0].Split(':')[1]);

            gBlocks = new int[height][];
            for (var i = 1; i < height + 1; i++)
                gBlocks[i - 1] = lines[i].Split(' ').Select(int.Parse).ToArray();
 
            width = int.Parse(lines[height+1].Split(':')[1]);

            vBlocks = new int[width][];
            for (var i = height + 2; i < lines.Length; i++)
                vBlocks[i - height - 2] = lines[i].Split(' ').Select(int.Parse).ToArray();

            field = Enumerable.Range(0, height)
                        .Select(y => Enumerable.Range(0, width)
                                               .Select(x => Cell.Fuzzy)
                                               .ToArray()
                                ).ToArray();
        }

        public bool IsCorrect()
        {
            var cellsSum = vBlocks.Sum(block => block.Sum()) + gBlocks.Sum(block => block.Sum());
            return vBlocks.All(block => block.Sum() <= height + 1 - block.Length) &&
                   gBlocks.All(block => block.Sum() <= width + 1 - block.Length) &&
                   cellsSum <= width * height;
        }
    }
}
