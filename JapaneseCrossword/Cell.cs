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
        public StateOfCell sost = StateOfCell.Fuzzy;
        public CheckState view = CheckState.NotChecked;

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != this.GetType()) return false;
            var cell = (Cell) obj;
            return cell.sost == sost && cell.view == view;
        }

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

        public Cell Known()
        {
            view = sost != StateOfCell.Fuzzy ? CheckState.Known : CheckState.NotChecked;
            return this;
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
            Assert.AreEqual('.', cell.ToChar());
            cell = new Cell(StateOfCell.Fuzzy, CheckState.Ckecked);
            Assert.AreEqual('?', cell.ToChar());
            cell = new Cell(StateOfCell.Shaded, CheckState.Ckecked);
            Assert.AreEqual('*', cell.ToChar());
        }

        [Test]
        public static void CellCanBe()
        {
            var cell = new Cell(StateOfCell.Empty, CheckState.Known);
            Assert.AreEqual(false, cell.CanBe(StateOfCell.Shaded));
            Assert.AreEqual(true, cell.CanBe(StateOfCell.Empty));
            cell = new Cell(StateOfCell.Shaded, CheckState.Known);
            Assert.AreEqual(false, cell.CanBe(StateOfCell.Empty));
            Assert.AreEqual(true, cell.CanBe(StateOfCell.Shaded));
        }
    }
}
