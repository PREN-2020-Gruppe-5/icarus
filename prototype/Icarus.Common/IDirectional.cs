namespace Icarus.Common
{
    public interface IDirectional<out T>
    {
        T Left { get; }
        T Right { get; }
    }
}