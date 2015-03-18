using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    return s == sost || sost == Sost.Fuzzy;
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

        internal bool IsKnown()
        {
            return view == Check.Known;
        }
    }
}
