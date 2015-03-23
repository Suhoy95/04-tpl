using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace JapaneseCrossword
{
    class Line
    {
        public static List<int> IsDifferentCells(Cell[] prevCells, Cell[] newCells)
        {
            if (prevCells.Length != newCells.Length)
                throw new Exception("IsDifferentLine: lines have a different length");

            var luckyLines = new List<int>();
            for (var i = 0; i < prevCells.Length; i++)
            {
                newCells[i].Known();
                if (!prevCells[i].IsKnown() && newCells[i].IsKnown())
                    luckyLines.Add(i);
            }

            return luckyLines;
        }
        
        public Cell[] cells;
        public int[] blocks;

        public Line TryBlock()
        {
            for(var i = 0; i < GetLimit(0); i++)
                TryBlock(blocks[0], i, 1);
            return this;
        }

        private void TryBlock(int width, int position, int nextBlock)
        {
            var limit = GetLimit(nextBlock);
            for (var i = position; i < limit; i++)
                if (BlockCanBe(position, i, width, limit))
                {
                    if(!BlockTrySet(position, i, width, limit))
                        throw new Exception("Try Block: can not to set block");

                    if (nextBlock < blocks.Length)
                        TryBlock(blocks[nextBlock], i + width, nextBlock + 1);
                }
        }

        
        
        public int GetLimit(int nextBlock)
        {
            if (blocks.Length == 1)
                return cells.Length - blocks[0] + 1;

            return cells.Length
                    - blocks.Skip(nextBlock).Sum()
                    - blocks.Skip(nextBlock).Count();
        }

        public bool BlockCanBe(int position, int positionOfBlock, int width, int limit)
        {
            var amountEmptyCells = limit == cells.Length ? limit - (positionOfBlock + width) : 1;
            amountEmptyCells = positionOfBlock + width == cells.Length ? 0 : amountEmptyCells;
            return CellsCanBe(position, positionOfBlock - position, StateOfCell.Empty)
                   && CellsCanBe(positionOfBlock, width, StateOfCell.Shaded)
                   && CellsCanBe(positionOfBlock + width, amountEmptyCells, StateOfCell.Empty);
        }

        public bool CellsCanBe(int position, int width, StateOfCell state)
        {
            return Enumerable.Range(position, width).All(i => cells[i].CanBe(state));
        }

        public bool BlockTrySet(int position, int positionOfBlock, int width, int limit)
        {
            var amountEmptyCells = limit == cells.Length ? limit - (positionOfBlock + width) : 1;
            amountEmptyCells = positionOfBlock + width == cells.Length ? 0 : amountEmptyCells;
            return TrySetCells(position, positionOfBlock - position, StateOfCell.Empty)
                   && TrySetCells(positionOfBlock, width, StateOfCell.Shaded)
                   && TrySetCells(positionOfBlock + width, amountEmptyCells, StateOfCell.Empty);
        }

        public bool TrySetCells(int position, int width, StateOfCell state)
        {
            return Enumerable.Range(position, width).All(i => cells[i].TrySet(state));
        }
    }

    [TestFixture]
    class LineTest
    {
        private Line line = new Line();

        [Test]
        public void CalculateOfLimit()
        {
            line.cells = new Cell[10];
            line.blocks = new[]{1, 2, 3};
            Assert.AreEqual(1, line.GetLimit(0) );
            Assert.AreEqual(3, line.GetLimit(1) );
            Assert.AreEqual(6, line.GetLimit(2) );
            Assert.AreEqual(10, line.GetLimit(3) );
        }

        [Test]
        public void CalculateOfLimitForOneBlock()
        {
            line.cells = new Cell[5];
            line.blocks = new[] {4};
            Assert.AreEqual(2, line.GetLimit(0));
        }

        [Test]
        public void SimpleTryLines()
        {
            line.cells = Enumerable.Range(0, 4).Select(i => new Cell()).ToArray();
            line.blocks = new[] {4};
            var result = Enumerable.Range(0, 4)
                                   .Select(i => new Cell(StateOfCell.Shaded, CheckState.Ckecked))
                                   .ToArray();
            line.TryBlock();
            Assert.AreEqual(result, line.cells);
        }
    }
}