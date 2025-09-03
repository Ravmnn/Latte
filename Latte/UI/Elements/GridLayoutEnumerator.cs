using System;
using System.Collections;
using System.Collections.Generic;


namespace Latte.UI.Elements;


public class GridLayoutEnumerator : IEnumerator<Element?>
{
    private readonly GridLayoutElement _gridLayout;
    private uint _row, _col;

    public Element? Current => _gridLayout[_row, _col];
    object? IEnumerator.Current => Current;

    private bool _disposed;


    public GridLayoutEnumerator(GridLayoutElement gridLayout)
    {
        _gridLayout = gridLayout;
        _row = _col = 0;

        _disposed = false;
    }


    public bool MoveNext()
    {
        _col++;

        if (_col < _gridLayout.Columns)
            return true;

        _col = 0;
        _row++;

        return _row < _gridLayout.Rows;
    }

    public void Reset()
    {
        _row = _col = 0;
    }


    // an IEnumerator should implement the dispose pattern

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        // nothing to dispose...

        if (_disposed)
            return;

        if (disposing) {}

        _disposed = true;
    }


    ~GridLayoutEnumerator()
    {
        Dispose(false);
    }
}
