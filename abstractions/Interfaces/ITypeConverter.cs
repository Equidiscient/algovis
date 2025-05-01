namespace algo_vis.abstractions.Interfaces;

public interface ITypeConverter<in TFrom, out TTo>
{
    TTo Convert(TFrom source);
}