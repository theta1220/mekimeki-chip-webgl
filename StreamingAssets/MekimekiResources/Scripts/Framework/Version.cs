using System;
using Cysharp.Threading.Tasks;
using NClone;

public class Version<T> where T : class
{
    private const int UNDO_MAX = 100;
    private ReferenceValue<T> _instance = null;
    private QueueStack<T> _undoStack = new QueueStack<T>();
    private QueueStack<T> _redoStack = new QueueStack<T>();
    public T Instance => _instance.Value;

    ~Version()
    {
        Clear();
    }

    public void Undo()
    {
        if (_undoStack.Count == 0)
        {
            return;
        }

        _redoStack.PushBack(Clone.ObjectGraph(_instance.Value));
        _instance.Value = _undoStack.PopBack();
    }

    public void Redo()
    {
        if (_redoStack.Count == 0)
        {
            return;
        }

        _undoStack.PushBack(Clone.ObjectGraph(_instance.Value));
        _instance.Value = _redoStack.PopBack();
    }

    public void Commit()
    {
        var obj = Clone.ObjectGraph(_instance.Value);
        _undoStack.PushBack(obj);
        if (_undoStack.Count == UNDO_MAX)
        {
            // 破棄
            _undoStack.PopFront();
        }

        _redoStack.Clear();
    }

    public void SetInstance(ReferenceValue<T> obj)
    {
        if (obj == null) return;
        Clear();
        _instance = obj;
    }

    public void Clear()
    {
        _undoStack.Clear();
        _redoStack.Clear();
        _instance = null;
    }
}