namespace Business.Models
{
    public class Period
    {
        public Period(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        public Period(DateTime start):this(start, start)
        {}

        public DateTime Start { get; private set; }

        public DateTime End { get; private set; }

        public bool IsMoment => Start == End;

        public void Setup(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        public bool OverlapsWith(Period period)
        {
            return Start < period.End && period.Start < End;
        }
    }
}
