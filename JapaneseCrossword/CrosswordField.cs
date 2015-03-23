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
        public Cell[][] field;
        public int[][] vBlocks;
        public int[][] hBlocks;
        public int width;
        public int height;

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
            var rawSum = hBlocks.Sum(block => block.Sum());
            return vBlocks.All(block => block.Sum() <= height - (block.Length - 1)) &&
                   hBlocks.All(block => block.Sum() <= width - (block.Length - 1)) &&
                   columnSum == rawSum && rawSum <= width * height;
        }

        public bool HaveFuzzy()
        {
            return field.Any(column => column.Any(cell => cell.ToChar() == '?'));
        }

        public Line GetRow(int number)
        {
            if (0 <= number && number < height)
            {
                var line = new Line();
                line.cells = field.Select(column => column[number]).ToArray();
                line.blocks = hBlocks[number];
                return line;
            }
            throw new Exception("GetRow: number incorrect");
        }

        public Line GetColumn(int number)
        {
            if (0 <= number && number < width)
            {
                var line = new Line();
                line.cells = Enumerable.Range(0, height)
                                       .Select(rawIndex => new Cell(field[number][rawIndex]))
                                       .ToArray();
                line.blocks = vBlocks[number];
                return line;
            }
            throw new Exception("GetColumn: number incorrect");
        }

        public void SetRow(int number, Line newLine)
        {
            if (0 <= number && number < height && newLine.cells.Length == width)
            {
                for (int i = 0; i < width; i++)
                    field[i][number] = newLine.cells[i];
                return;
            }
            throw new Exception("SetRow: Invalid parametr");
        }

        public void SetColumn(int number, Line newLine)
        {
            if (0 <= number && number < width && newLine.cells.Length == height)
            {
                for (int i = 0; i < height; i++)
                    field[number][i] = newLine.cells[i];
                return;
            }
            throw new Exception("SetColumn: Invalid parametr");
        }
    }
}
