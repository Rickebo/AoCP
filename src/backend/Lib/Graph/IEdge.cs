namespace Lib;

public interface IEdge<TNode> where TNode : notnull
{
    TNode From { get; }
    TNode To { get; }
}