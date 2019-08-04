namespace Scai.Driver.Data
{
    public class Car
    {
        public int Speed { get; }

        public int Position { get; }

        public Car(int speed, int position)
        {
            Speed = speed;
            Position = position;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Car))
            {
                return false;
            }

            var car = (Car)obj;
            return (Speed == car.Speed) && (Position == car.Position);
        }

        public override int GetHashCode()
        {
            return Speed ^ Position;
        }
    }
}