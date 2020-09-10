namespace testPr.Graphs
{
    internal interface IGraph<TKey, TVal>
    {
        TVal GetY(TKey valX);
    }
}
