using System;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<T> where T : IComparable<T>
{
    public List<T> _heap = new List<T>();
    public int Count => _heap.Count;


    public void Add(T item) // �켱���� ť ������ �߰�
    {
        Push(item);
    }

    public void Clear() // �켱���� ť Ŭ����
    {
        _heap.Clear();
    }

    public bool Contains(T item)    // ť ���� ������ �ִ��� Ȯ��
    {
        foreach(T data in _heap)
        {
            if(item.CompareTo(data)==0)
            {
                return true;
            }
        }
        return false;
    }

    private bool Compare(int _compareIndexA, int _compareIndexB)    // �ε��� �Է¿� ���� ��
    {
        // < 0 �� ��� �������� ����
        return _heap[_compareIndexA].CompareTo(_heap[_compareIndexB]) < 0;
    }

    private void Swap(int _indexA, int _indexB) // ������ ��ü
    {
        T temp = _heap[_indexB];
        _heap[_indexB] = _heap[_indexA];
        _heap[_indexA] = temp;
    }

    public string PrintArray()  // �ε����迭 ���
    {
        int i = 0;
        string printStr = string.Empty;
        while (i < Count)
        {
            printStr += _heap[i].ToString() + "";
            i++;
        }
        return printStr;
    }

    public void Push(T _data)   // ť ������ �߰��ϸ鼭 ������ �߰�
    {
        _heap.Add(_data);

        int now = _heap.Count - 1;  // ���� �ڿ��ִ� ������
        while(now > 0)
        {
            int next = (now - 1) / 2;           // �߾Ӱ�
            bool bBreak = Compare(now, next);   // ���� �ڿ� �߾Ӱ���

            if (bBreak)
                break;

            Swap(next, now);                    // �������� ����
            now = next;
        }
    }

    public T Pop()  // ť���� ������ �ϳ� ������
    {
        if(_heap.Count<=0)
        {
            Debug.LogError("Queue is Empty");
            return default(T);
        }

        T ret = _heap[0];

        int lastIndex = _heap.Count - 1;
        _heap[0] = _heap[lastIndex];        // POP �����̱� ������ ���� �տ� �ִ� �����͸� ������
        _heap.RemoveAt(lastIndex);

        if (_heap.Count == 1)
            return ret;

        lastIndex--;

        int now = 0;
        while(true)
        {
            // Ʈ���� left �� right ��带 ���ϴ� ����
            int left = 2 * now + 1;
            int right = 2 * now + 2;

            int next = now;

            // left > now ? && next < left ?
            if (left <= lastIndex && _heap[next].CompareTo(_heap[left]) < 0)
                next = left;

            // right > new ? && next < left ?
            if (right <= lastIndex && _heap[next].CompareTo(_heap[right]) < 0)
                next = right;

            // left & right < now
            if (next == now)
                break;

            Swap(next, now);
            now = next;
        }
        return ret;
    }
}