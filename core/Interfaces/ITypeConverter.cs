namespace algo_vis.core.Interfaces;

public interface ITypeConverter<in TFrom, out TTo>
{
    TTo Convert(TFrom source);
}