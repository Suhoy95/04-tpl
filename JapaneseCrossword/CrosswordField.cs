using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JapaneseCrossword
{
    class CrosswordField
    {
        private Cell[][] field;
        public int[][] vBlocks { get; private set; }
        public int[][] gBlocks { get; private set; }
        public int width { get; private set; }
        public int height { get; private set; }

        public CrosswordField(string filename)
        {
            var lines = File.ReadAllLines(filename);
            height = int.Parse(lines[0].Split(':')[1]);

            gBlocks = new int[height][];
            for (var i = 1; i < height + 1; i++)
                gBlocks[i - 1] = lines[i].Split(' ').Select(int.Parse).ToArray();

            width = int.Parse(lines[height + 1].Split(':')[1]);

            vBlocks = new int[width][];
            for (var i = height + 2; i < lines.Length; i++)
                vBlocks[i - height - 2] = lines[i].Split(' ').Select(int.Parse).ToArray();

            field = new Cell[width][];
            for (var i = 0; i < field.Length; i++)
                field[i] = Enumerable.Range(0, height).Select(cell => new Cell()).ToArray();;
        }

        public override string ToString()
        {
            var ans = new StringBuilder((width + 2)*height);

            for (var y = 0; y < height; y++) 
            {
                for (var x = 0; x < width; x++)
                    ans.Append(field[x][y].ToChar());
                ans.Append("\r\n");
            }

            return ans.ToString();
        }

        public bool IsCorrect()
        {
            var columnSum = vBlocks.Sum(block => block.Sum());
            var rawSum = gBlocks.Sum(block => block.Sum());
            return vBlocks.All(block => block.Sum() <= height - (block.Length - 1)) &&
                   gBlocks.All(block => block.Sum() <= width - (block.Length - 1)) &&
                   columnSum == rawSum && rawSum <= width * height;
        }

        public Cell[] GetRow(int number)
        {
            if (0 <= number && number < height)
                return field.Select(column => column[number]).ToArray();
            throw new Exception("GetRow: number incorrect");
        }

        public Cell[] GetColumn(int number)
        {
            if (0 <= number && number < width)
                return Enumerable.Range(0, height).Select(x => new Cell(field[number][x])).ToArray();
            throw new Exception("GetColumn: number incorrect");
        }

        public void SetRow(int number, Cell[] newLine)
        {
            if (0 <= number && number < height && newLine.Length == width)
            {
                for (int i = 0; i < width; i++)
                    field[i][number] = newLine[i];
                return;
            }
            throw new Exception("SetRow: Invalid parametr");
        }

        public void SetColumn(int number, Cell[] newLine)
        {
            if (0 <= number && number < width && newLine.Length == height)
            {
                for (int i = 0; i < height; i++)
                    field[number][i] = newLine[i];
                return;
            }
            throw new Exception("SetColumn: Invalid parametr");
        }

        public bool HaveFuzzy()
        {
            return field.Any(column => column.Any(cell => cell.ToChar() == '?'));
        }
    }
}
