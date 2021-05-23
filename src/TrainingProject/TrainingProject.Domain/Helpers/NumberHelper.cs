namespace TrainingProject.Domain.Helpers
{
    public static class NumberHelper
    {
        public static int TrueModulo(int a, int b)
        {
            return a > 0 ? a % b : a % b + b;
        }
    }
}