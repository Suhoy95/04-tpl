using System;
using System.IO;
using System.Text.RegularExpressions;

namespace JapaneseCrossword
{
    public class CrosswordSolver : ICrosswordSolver
    {
        public SolutionStatus Solve(string inputFilePath, string outputFilePath)
        {
            if (!File.Exists(inputFilePath)) return SolutionStatus.BadInputFilePath;
            if (!FileNameCorrecter.IsCorrectedFileName(inputFilePath) &&
                !FileNameCorrecter.IsCorrectedFileName(outputFilePath))
                return SolutionStatus.BadOutputFilePath;

            var field = new CrosswordField(inputFilePath);

            if(!field.IsCorrect())
                return SolutionStatus.IncorrectCrossword;

            return SolutionStatus.Solved;
        }
    }

   
}