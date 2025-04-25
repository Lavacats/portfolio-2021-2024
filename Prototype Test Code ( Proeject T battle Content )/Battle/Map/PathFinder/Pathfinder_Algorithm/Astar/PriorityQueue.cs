using System;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<T> where T : IComparable<T>
{
    public List<T> _heap = new List<T>();
    public int Count => _heap.Count;


    public void Add(T item) // 우선순위 큐 데이터 추가
    {
        Push(item);
    }

    public void Clear() // 우선순위 큐 클리어
    {
        _heap.Clear();
    }

    public bool Contains(T item)    // 큐 내부 데이터 있는지 확인
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

    private bool Compare(int _compareIndexA, int _compareIndexB)    // 인덱스 입력에 따른 비교
    {
        // < 0 일 경우 오름차순 정렬
        return _heap[_compareIndexA].CompareTo(_heap[_compareIndexB]) < 0;
    }

    private void Swap(int _indexA, int _indexB) // 데이터 교체
    {
        T temp = _heap[_indexB];
        _heap[_indexB] = _heap[_indexA];
        _heap[_indexA] = temp;
    }

    public string PrintArray()  // 인덱스배열 출력
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

    public void Push(T _data)   // 큐 데이터 추가하면서 정렬후 추가
    {
        _heap.Add(_data);

        int now = _heap.Count - 1;  // 가장 뒤에있는 데이터
        while(now > 0)
        {
            int next = (now - 1) / 2;           // 중앙값
            bool bBreak = Compare(now, next);   // 가장 뒤와 중앙값비교

            if (bBreak)
                break;

            Swap(next, now);                    // 오름차순 정렬
            now = next;
        }
    }

    public T Pop()  // 큐에서 데이터 하나 꺼내기
    {
        if(_heap.Count<=0)
        {
            Debug.LogError("Queue is Empty");
            return default(T);
        }

        T ret = _heap[0];

        int lastIndex = _heap.Count - 1;
        _heap[0] = _heap[lastIndex];        // POP 형태이기 떄문에 가장 앞에 있는 데이터를 꺼낸다
        _heap.RemoveAt(lastIndex);

        if (_heap.Count == 1)
            return ret;

        lastIndex--;

        int now = 0;
        while(true)
        {
            // 트리의 left 와 right 노드를 구하는 공식
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