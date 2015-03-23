using System;
using NUnit.Framework;

namespace JapaneseCrossword
{
    enum StateOfCell
    {
        Empty,
        Fuzzy,
        Shaded
    }

    enum CheckState
    {
        Ckecked,
        NotChecked,
        Known
    }

    class Cell
    {
        public Cell(Cell cell)
        {
            sost = cell.sost;
            view = cell.view;
        }

        public Cell(StateOfCell s, CheckState v)
        {
            sost = s;
            view = v;
        }
        public Cell() { }
        private StateOfCell sost = StateOfCell.Fuzzy;
        private CheckState view = CheckState.NotChecked;

        public char ToChar()
        {
            switch (sost)
            {
                case StateOfCell.Empty:
                    return '.';
                case StateOfCell.Fuzzy:
                    return '?';
                case StateOfCell.Shaded:
                    return '*';
            }

            throw new Exception("SostToChar: incorrect sost");
        }

        public bool CanBe(StateOfCell s)
        {
            switch (view)
            {
                case CheckState.NotChecked:
                    return true;
                case CheckState.Ckecked:
                    return true;
                case CheckState.Known:
                    return s == sost;
            }
            throw new Exception("CanBe: incorret view");
        }

        public bool TrySet(StateOfCell s)
        {
            {
                switch (view)
                {
                    case CheckState.NotChecked:
                        sost = s;
                        view = CheckState.Ckecked;
                        return true;
                    case CheckState.Ckecked:
                        sost = s == sost ? s : StateOfCell.Fuzzy;
                        return s == sost || sost == StateOfCell.Fuzzy;
                    case CheckState.Known:
                        return s == sost;
                }
                throw new Exception("CanBe: incorret view");
            }
        }

        public void Known()
        {
            view = sost != StateOfCell.Fuzzy ? CheckState.Known : CheckState.NotChecked;
        }

        public bool IsKnown()
        {
            return view == CheckState.Known;
        }
    }

    [TestFixture]
    public class CellTest
    {
        [Test]
        public static void CellToChar()
        {
            var cell = new Cell(StateOfCell.Empty, CheckState.Ckecked);
            Assert.AreEqual(cell.ToChar(), '.');
            cell = new Cell(StateOfCell.Fuzzy, CheckState.Ckecked);
            Assert.AreEqual(cell.ToChar(), '?');
            cell = new Cell(StateOfCell.Shaded, CheckState.Ckecked);
            Assert.AreEqual(cell.ToChar(), '*');
        }

        [Test]
        public static void CellCanBe()
        {
            var cell = new Cell(StateOfCell.Empty, CheckState.Known);
            Assert.AreEqual(cell.CanBe(StateOfCell.Shaded), false);
            Assert.AreEqual(cell.CanBe(StateOfCell.Empty), true);
            cell = new Cell(StateOfCell.Shaded, CheckState.Known);
            Assert.AreEqual(cell.CanBe(StateOfCell.Empty), false);
            Assert.AreEqual(cell.CanBe(StateOfCell.Shaded), true);
        }
    }
}
