public class Utils
{
    public static T NegGet<T>(T[] arr, int index)
    {
        return arr[(index + arr.Length) % arr.Length];
    }

}