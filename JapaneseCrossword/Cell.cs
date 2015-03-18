using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace JapaneseCrossword
{
    enum Sost
    {
        Empty,
        Fuzzy,
        Shaded
    }

    enum Check
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

        public Cell(Sost s, Check v)
        {
            sost = s;
            view = v;
        }
        public Cell() { }
        private Sost sost = Sost.Fuzzy;
        private Check view = Check.NotChecked;

        public char ToChar()
        {
            switch (sost)
            {
                case Sost.Empty:
                    return '.';
                case Sost.Fuzzy:
                    return '?';
                case Sost.Shaded:
                    return '*';
            }

            throw new Exception("SostToChar: incorrect sost");
        }

        public bool CanBe(Sost s)
        {
            switch (view)
            {
                case Check.NotChecked:
                    return true;
                case Check.Ckecked:
                    return true;
                case Check.Known:
                    return s == sost;
            }
            throw new Exception("CanBe: incorret view");
        }

        public bool TrySet(Sost s)
        {
            {
                switch (view)
                {
                    case Check.NotChecked:
                        sost = s;
                        view = Check.Ckecked;
                        return true;
                    case Check.Ckecked:
                        sost = s == sost ? s : Sost.Fuzzy;
                        return s == sost || sost == Sost.Fuzzy;
                    case Check.Known:
                        return s == sost;
                }
                throw new Exception("CanBe: incorret view");
            }
        }

        public void Known()
        {
            view = sost != Sost.Fuzzy ? Check.Known : Check.NotChecked;
        }

        public bool IsKnown()
        {
            return view == Check.Known;
        }
    }

    [TestFixture]
    public class CellTest
    {
        [Test]
        public static void CellToChar()
        {
            var cell = new Cell(Sost.Empty, Check.Ckecked);
            Assert.AreEqual(cell.ToChar(), '.');
            cell = new Cell(Sost.Fuzzy, Check.Ckecked);
            Assert.AreEqual(cell.ToChar(), '?');
            cell = new Cell(Sost.Shaded, Check.Ckecked);
            Assert.AreEqual(cell.ToChar(), '*');
        }

        [Test]
        public static void CellCanBe()
        {
            var cell = new Cell(Sost.Empty, Check.Known);
            Assert.AreEqual(cell.CanBe(Sost.Shaded), false);
            Assert.AreEqual(cell.CanBe(Sost.Empty), true);
            cell = new Cell(Sost.Shaded, Check.Known);
            Assert.AreEqual(cell.CanBe(Sost.Empty), false);
            Assert.AreEqual(cell.CanBe(Sost.Shaded), true);
        }
    }
}
