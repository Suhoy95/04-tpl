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
        private SolutionStatus result;

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

        public SolutionStatus Solve(string inputFilePath, string outputFilePath)
        {
            var resultInit = Init(inputFilePath, outputFilePath);
            if (resultInit != SolutionStatus.PartiallySolved) return resultInit;

            for(var i = 0; i < field.width + field.height - 1; i++)
            {
                TryLine(i);
                while (luckyLines.Count > 0)
                    TryLine(luckyLines.Pop());
            }

            File.WriteAllText(outputFilePath, field.ToString());

            return SolutionStatus.Solved;
        }

        private void TryLine(int number)
        {
            var prevLine = GetLine(number);
            var newLine = RobinBlock(GetLine(number), GetBlocks(number));
            if (IsDifferentLine(prevLine, newLine, number < field.width))
                SetLine(number, newLine);
        }

        private Cell[] RobinBlock(Cell[] line, int[] blocks)
        {
            return TryBlock(0, -1, line, blocks, 0);
        }

        private Cell[] TryBlock(int width, int pos, Cell[] line, int[] blocks, int nextBlock)
        {
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