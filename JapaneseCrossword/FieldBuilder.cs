using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JapaneseCrossword
{
    class FieldBuilder
    {
        public static CrosswordField CreateFieldFromFile(string filename)
        {
            var cField = new CrosswordField();
            var lines = File.ReadAllLines(filename);

            var height = cField.height = int.Parse(lines[0].Split(':')[1]);
            var width = cField.width = int.Parse(lines[cField.height + 1].Split(':')[1]);

            cField.hBlocks = new int[height][];
            cField.vBlocks = new int[width][];
            for (var i = 1; i < height + 1; i++)
                cField.hBlocks[i - 1] = lines[i].Split(' ').Select(int.Parse).ToArray();

            for (var i = height + 2; i < lines.Length; i++)
                cField.vBlocks[i - height - 2] = lines[i].Split(' ').Select(int.Parse).ToArray();

            cField.field = new Cell[cField.width][];
            for (var i = 0; i < width; i++)
                cField.field[i] = Enumerable.Range(0, height).Select(cell => new Cell()).ToArray(); ;

            return cField;
        }
    }
}
