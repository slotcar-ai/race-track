namespace Scai.RaceTrack.Data
{
    public class DataPoint
    {
        public Car LeftCar { get; }

        public Car RightCar { get; }

        public DataPoint(int leftCarSpeed, int leftCarPosition, int rightCarSpeed, int rightCarPosition)
        {
            LeftCar = new Car(leftCarSpeed, leftCarPosition);
            RightCar = new Car(rightCarSpeed, rightCarPosition);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is DataPoint))
            {
                return false;
            }

            var dataPoint = (DataPoint)obj;
            return LeftCar.Equals(dataPoint.LeftCar) && RightCar.Equals(dataPoint.RightCar);
        }

        public override int GetHashCode()
        {
            return LeftCar.GetHashCode() ^ RightCar.GetHashCode();
        }
    }
}