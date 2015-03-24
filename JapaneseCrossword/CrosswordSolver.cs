using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JapaneseCrossword
{
    public class CrosswordSolver : ICrosswordSolver
    {
        private CrosswordField field;

        public override string ToString()
        {
            return field != null ? field.ToString() : "";
        }
        
        private SolutionStatus Init(string inputFilePath, string outputFilePath)
        {
            if (!File.Exists(inputFilePath)) return SolutionStatus.BadInputFilePath;
            if (!FileNameCorrecter.IsCorrectedFileName(inputFilePath) ||
                !FileNameCorrecter.IsCorrectedFileName(outputFilePath))
                return SolutionStatus.BadOutputFilePath;

            field = FieldBuilder.CreateFieldFromFile(inputFilePath);

            if (!field.IsCorrect())
                return SolutionStatus.IncorrectCrossword;

            return SolutionStatus.PartiallySolved;
        }
        
        public SolutionStatus Solve(string inputFilePath, string outputFilePath)
        {
            var resultInit = Init(inputFilePath, outputFilePath);
            if (resultInit != SolutionStatus.PartiallySolved) return resultInit;

            var luckyLines = new Queue<int>();
            for(var i = 0; i < field.width + field.height; i++)
            {
                TryLine(i, luckyLines);
                while (luckyLines.Count > 0)
                {
                    i = -1;
                    TryLine(luckyLines.Dequeue(), luckyLines);
                }
            }

            File.WriteAllText(outputFilePath, field.ToString());

            return field.HaveFuzzy() ? SolutionStatus.PartiallySolved : SolutionStatus.Solved;
        }

        private void TryLine(int number, Queue<int> luckyLines)
        {
            var prevLine = GetLine(number);
            var newLine = GetLine(number).TryBlock();
            var lucky = Line.FindDifferentCells(prevLine.cells, newLine.cells);
            
            if (lucky.Count == 0) return;
            
            SetLine(number, newLine);
            foreach (var i in lucky)
                luckyLines.Enqueue(number < field.width ? field.width + i : i);
        }

        private Line GetLine(int number)
        {
            if (number >= field.width + field.height) 
                throw new Exception("GetLine: number incorrect");
            
            if (number < field.width)
                return field.GetColumn(number);
            return field.GetRow(number - field.width);
        }

        private void SetLine(int number, Line newLine)
        {
            if (number >= field.width + field.height)
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