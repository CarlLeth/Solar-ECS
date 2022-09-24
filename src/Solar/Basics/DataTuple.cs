using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs
{
    /// <summary>
    /// Contains helper methods for constructing DataTuples.
    /// </summary>
    public static class DataTuple
    {
        public static ZeroTuple Zero => new ZeroTuple();
        public static DataTuple<T1> From<T1>(T1 item1) => new DataTuple<T1>(item1);
        public static DataTuple<T1, T2> From<T1, T2>(T1 item1, T2 item2) => new DataTuple<T1, T2>(item1, item2);
        public static DataTuple<T1, T2, T3> From<T1, T2, T3>(T1 item1, T2 item2, T3 item3) => new DataTuple<T1, T2, T3>(item1, item2, item3);
        public static DataTuple<T1, T2, T3, T4> From<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4) => new DataTuple<T1, T2, T3, T4>(item1, item2, item3, item4);
        public static DataTuple<T1, T2, T3, T4, T5> From<T1, T2, T3, T4, T5>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5) => new DataTuple<T1, T2, T3, T4, T5>(item1, item2, item3, item4, item5);
        public static DataTuple<T1, T2, T3, T4, T5, T6> From<T1, T2, T3, T4, T5, T6>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6) => new DataTuple<T1, T2, T3, T4, T5, T6>(item1, item2, item3, item4, item5, item6);
        public static DataTuple<T1, T2, T3, T4, T5, T6, T7> From<T1, T2, T3, T4, T5, T6, T7>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7) => new DataTuple<T1, T2, T3, T4, T5, T6, T7>(item1, item2, item3, item4, item5, item6, item7);
        public static DataTuple<T1, T2, T3, T4, T5, T6, T7, T8> From<T1, T2, T3, T4, T5, T6, T7, T8>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8) => new DataTuple<T1, T2, T3, T4, T5, T6, T7, T8>(item1, item2, item3, item4, item5, item6, item7, item8);
        public static DataTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9> From<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8, T9 item9) => new DataTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9>(item1, item2, item3, item4, item5, item6, item7, item8, item9);
    }

    public class ZeroTuple
    {

    }

    /// <summary>
    /// Query-safe version of System.Tuple
    /// </summary>
    public class DataTuple<T1>
    {
        public T1 Item1 { get; private set; }

        public DataTuple(T1 item1)
        {
            this.Item1 = item1;
        }

        private DataTuple() { }
    }

    /// <summary>
    /// Query-safe version of System.Tuple
    /// </summary>
    public class DataTuple<T1, T2>
    {
        public T1 Item1 { get; private set; }
        public T2 Item2 { get; private set; }

        public DataTuple(T1 item1, T2 item2)
        {
            this.Item1 = item1;
            this.Item2 = item2;
        }

        private DataTuple() { }
    }

    /// <summary>
    /// Query-safe version of System.Tuple
    /// </summary>
    public class DataTuple<T1, T2, T3>
    {
        public T1 Item1 { get; private set; }
        public T2 Item2 { get; private set; }
        public T3 Item3 { get; private set; }

        public DataTuple(T1 item1, T2 item2, T3 item3)
        {
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
        }

        private DataTuple() { }
    }

    /// <summary>
    /// Query-safe version of System.Tuple
    /// </summary>
    public class DataTuple<T1, T2, T3, T4>
    {
        public T1 Item1 { get; private set; }
        public T2 Item2 { get; private set; }
        public T3 Item3 { get; private set; }
        public T4 Item4 { get; private set; }

        public DataTuple(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
            this.Item4 = item4;
        }

        private DataTuple() { }
    }

    /// <summary>
    /// Query-safe version of System.Tuple
    /// </summary>
    public class DataTuple<T1, T2, T3, T4, T5>
    {
        public T1 Item1 { get; private set; }
        public T2 Item2 { get; private set; }
        public T3 Item3 { get; private set; }
        public T4 Item4 { get; private set; }
        public T5 Item5 { get; private set; }

        public DataTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
        {
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
            this.Item4 = item4;
            this.Item5 = item5;
        }

        private DataTuple() { }
    }

    /// <summary>
    /// Query-safe version of System.Tuple
    /// </summary>
    public class DataTuple<T1, T2, T3, T4, T5, T6>
    {
        public T1 Item1 { get; private set; }
        public T2 Item2 { get; private set; }
        public T3 Item3 { get; private set; }
        public T4 Item4 { get; private set; }
        public T5 Item5 { get; private set; }
        public T6 Item6 { get; private set; }

        public DataTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
        {
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
            this.Item4 = item4;
            this.Item5 = item5;
            this.Item6 = item6;
        }

        private DataTuple() { }
    }

    /// <summary>
    /// Query-safe version of System.Tuple
    /// </summary>
    public class DataTuple<T1, T2, T3, T4, T5, T6, T7>
    {
        public T1 Item1 { get; private set; }
        public T2 Item2 { get; private set; }
        public T3 Item3 { get; private set; }
        public T4 Item4 { get; private set; }
        public T5 Item5 { get; private set; }
        public T6 Item6 { get; private set; }
        public T7 Item7 { get; private set; }

        public DataTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
        {
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
            this.Item4 = item4;
            this.Item5 = item5;
            this.Item6 = item6;
            this.Item7 = item7;
        }

        private DataTuple() { }
    }

    /// <summary>
    /// Query-safe version of System.Tuple
    /// </summary>
    public class DataTuple<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        public T1 Item1 { get; private set; }
        public T2 Item2 { get; private set; }
        public T3 Item3 { get; private set; }
        public T4 Item4 { get; private set; }
        public T5 Item5 { get; private set; }
        public T6 Item6 { get; private set; }
        public T7 Item7 { get; private set; }
        public T8 Item8 { get; private set; }

        public DataTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8)
        {
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
            this.Item4 = item4;
            this.Item5 = item5;
            this.Item6 = item6;
            this.Item7 = item7;
            this.Item8 = item8;
        }

        private DataTuple() { }
    }

    /// <summary>
    /// Query-safe version of System.Tuple
    /// </summary>
    public class DataTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9>
    {
        public T1 Item1 { get; private set; }
        public T2 Item2 { get; private set; }
        public T3 Item3 { get; private set; }
        public T4 Item4 { get; private set; }
        public T5 Item5 { get; private set; }
        public T6 Item6 { get; private set; }
        public T7 Item7 { get; private set; }
        public T8 Item8 { get; private set; }
        public T9 Item9 { get; private set; }

        public DataTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8, T9 item9)
        {
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
            this.Item4 = item4;
            this.Item5 = item5;
            this.Item6 = item6;
            this.Item7 = item7;
            this.Item8 = item8;
            this.Item9 = item9;
        }

        private DataTuple() { }
    }
}
