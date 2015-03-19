using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JapaneseCrossword
{
    public class CrosswordSolver : ICrosswordSolver
    {
        private CrosswordField field;
        private Stack<int> luckyLines = new Stack<int>();
        private SolutionStatus Init(string inputFilePath, string outputFilePath)
        {
            if (!File.Exists(inputFilePath)) return SolutionStatus.BadInputFilePath;
            if (!FileNameCorrecter.IsCorrectedFileName(inputFilePath) ||
                !FileNameCorrecter.IsCorrectedFileName(outputFilePath))
                return SolutionStatus.BadOutputFilePath;

            field = new CrosswordField(inputFilePath);

            if (!field.IsCorrect())
                return SolutionStatus.IncorrectCrossword;

            return SolutionStatus.PartiallySolved;
        }

        public override string ToString()
        {
            return field != null ? field.ToString() : "";
        }

        public SolutionStatus Solve(string inputFilePath, string outputFilePath)
        {
            var resultInit = Init(inputFilePath, outputFilePath);
            if (resultInit != SolutionStatus.PartiallySolved) return resultInit;

            for(var i = 0; i < field.width + field.height; i++)
            {
                TryLine(i);
                while (luckyLines.Count > 0)
                {
                    i = -1;
                    TryLine(luckyLines.Pop());
                }
            }

            File.WriteAllText(outputFilePath, field.ToString());

            return field.HaveFuzzy() ? SolutionStatus.PartiallySolved : SolutionStatus.Solved;
        }

        private void TryLine(int number)
        {
            var prevLine = GetLine(number);
            var newLine = TryBlock(0, 0, GetLine(number), GetBlocks(number), 0);
            if (IsDifferentLine(prevLine, newLine, number < field.width))
                SetLine(number, newLine);
        }

        private Cell[] TryBlock(int width, int pos, Cell[] line, int[] blocks, int nextBlock)
        {
            var limit = line.Length - (blocks.Skip(nextBlock).Sum() + blocks.Skip(nextBlock).Count() + (width == 0 ? -1 : 0)) + 1;
            for (var i = pos; i < limit; i++)
            {
                var can = Enumerable.Range(0, width).All(j => i+j < line.Length && line[i + j].CanBe(Sost.Shaded));
                can = can && Enumerable.Range(pos, i - pos).All(j => line[j].CanBe(Sost.Empty));
                can = can &&
                      (nextBlock < blocks.Length ||
                       Enumerable.Range(i + width, line.Length - i - width).All(j => line[j].CanBe(Sost.Empty)));
                if (can)
                {
                    Enumerable.Range(0, width).All(j => line[i + j].TrySet(Sost.Shaded));
                    Enumerable.Range(pos, i - pos).All(j => width == 0 || line[j].TrySet(Sost.Empty));
                    if(nextBlock >= blocks.Length)
                        Enumerable.Range(i + width, line.Length - i - width).All(j => line[j].TrySet(Sost.Empty));
                }
                if (can && nextBlock < blocks.Length)
                    line = TryBlock(blocks[nextBlock], i + width, line, blocks, nextBlock + 1);
            }

            return line;
        }

        private bool IsDifferentLine(Cell[] prevLine, Cell[] newLine, bool isColumn)
        {
            if(prevLine.Length != newLine.Length) throw new Exception("IsDifferentLine: lines have a different length");
            var isDifferentLine = false;
            for (var i = 0; i < prevLine.Length; i++)
            {
                newLine[i].Known();
                if (!prevLine[i].IsKnown() && newLine[i].IsKnown())
                {
                    isDifferentLine = true;
                    luckyLines.Push(isColumn ? field.width + i : i);
                }
            }
               
            return isDifferentLine;
        }

        private int[] GetBlocks(int number)
        {
            if (number >= field.width + field.height)
                throw new Exception("GetBlocks: number incorrect");

            if (number < field.width)
                return field.vBlocks[number];
            return field.gBlocks[number - field.width];
        }

        private Cell[] GetLine(int number)
        {
            if (number >= field.width + field.height) 
                throw new Exception("GetLine: number incorrect");
            
            if (number < field.width)
                return field.GetColumn(number);
            return field.GetRow(number - field.width);
        }

        private void SetLine(int number, Cell[] newLine)
        {
            if (number >= field.width + field.height - 1)
                throw new Exception("SetLine: number incorrect");

            if (number < field.width)
            {
                field.SetColumn(number, newLine);
                return;
            }
            field.SetRow(number - field.width, newLine);
        }
    }
}