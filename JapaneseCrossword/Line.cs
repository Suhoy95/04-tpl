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
            return TryBlock(blocks[0], 0, 1);
        }

        private Line TryBlock(int width, int position, int nextBlock)
        {
            var limit = GetLimit(nextBlock - 1);
            for (var i = position; i < limit; i++)
                if (BlockCanBe(position, i, width, limit))
                {
                    if(!BlockTrySet(position, i, width, limit))
                        throw new Exception("Try Block: can not to set block");

                    if (nextBlock < blocks.Length)
                        TryBlock(blocks[nextBlock], i + width + 1, nextBlock + 1);
                }
            return this;
        }
        
        public int GetLimit(int indexBlock)
        {
            return cells.Length
                    - blocks.Skip(indexBlock + 1).Sum()
                    - blocks.Skip(indexBlock + 1).Count()
                    - blocks[indexBlock] + 1;
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

        private void CreateLine(int amount, int[] blocks)
        {
            line.cells = Enumerable.Range(0, amount).Select(i => new Cell()).ToArray();
            line.blocks = blocks;
        }

        [Test]
        public void CalculateOfLimit()
        {
            CreateLine(10, new []{1, 2, 3});
            Assert.AreEqual(3, line.GetLimit(0) );
            Assert.AreEqual(5, line.GetLimit(1) );
            Assert.AreEqual(8, line.GetLimit(2) );
        }
        
        [Test]
        public void SimpleTryLines()
        {
            CreateLine(4, new []{4});
            var result = Enumerable.Range(0, 4)
                                   .Select(i => new Cell(StateOfCell.Shaded, CheckState.Ckecked))
                                   .ToArray();
            line.TryBlock();
            Assert.AreEqual(result, line.cells);
        }

        [Test]
        public void TryLines_FindShadedPoint()
        {
            CreateLine(5, new []{4});
            var fuzzyIndex = new[] {0, 4};
            var result = Enumerable.Range(0, 5)
                .Select(i => new Cell(fuzzyIndex.Contains(i) ? StateOfCell.Fuzzy: StateOfCell.Shaded,CheckState.Ckecked))
                .ToArray();
            Assert.AreEqual(result, line.TryBlock().cells);
        }

        [Test]
        public void TryLines_FindShadedPoint2()
        {
            CreateLine(6, new []{4});
            var shadedIndex = new[] {2, 3};
            var result = Enumerable.Range(0, 6)
                .Select(i => new Cell(shadedIndex.Contains(i) ? StateOfCell.Shaded : StateOfCell.Fuzzy, CheckState.Ckecked))
                .ToArray();
            Assert.AreEqual(result, line.TryBlock().cells);
        }

        [Test]
        public void TryLines_TwoBlockSimple()
        {
            CreateLine(6, new []{2, 3});
            var emptyIndex = new[] {2};
            var result = Enumerable.Range(0, 6)
                .Select(i => new Cell(emptyIndex.Contains(i) ? StateOfCell.Empty : StateOfCell.Shaded, CheckState.Ckecked))
                .ToArray();
            Assert.AreEqual(result, line.TryBlock().cells);
        }

        [Test]
        public void TryLines_TwoBlock_FindShaded()
        {
            CreateLine(7, new[] { 2, 3 });
            var shadedIndex = new[] {1, 4, 5};
            var result = Enumerable.Range(0, 7)
                .Select(i => new Cell(shadedIndex.Contains(i) ? StateOfCell.Shaded : StateOfCell.Fuzzy, CheckState.Ckecked))
                .ToArray();
            Assert.AreEqual(result, line.TryBlock().cells);
        }
    }
}