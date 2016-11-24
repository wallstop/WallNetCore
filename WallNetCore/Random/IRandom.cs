namespace WallNetCore.Random
{
    public interface IRandom
    {
        int Next();
        int Next(int max);
        int Next(int min, int max);

        bool NextBool();

        double NextDouble();
        double NextDouble(double max);
        double NextDouble(double min, double max);

        float NextFloat();
        float NextFloat(float max);
        float NextFloat(float min, float max);

        // TODO: Longs
    }
}