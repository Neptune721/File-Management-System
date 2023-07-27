using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem
{
    class BitMap
    {
        public const int MEMORY_SIZE = 1024;
        public int Length
        {
            get { return m_length; }
        }
        private int[] m_array;
        private int m_length;

        //分配空间以容纳长度位值, 位数组中的所有值都设置为defaultValue。
        public BitMap(int length, bool defaultValue)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length", "长度值不能小于0");
            }

            int arrayLength = length > 0 ? (((length - 1) / 32) + 1) : 0;
            m_array = new int[arrayLength];
            m_length = length;

            int fillValue = defaultValue ? unchecked(((int)0xffffffff)) : 0;
            for (int i = 0; i < m_array.Length; i++)
            {
                m_array[i] = fillValue;
            }
        }

        //返回位置索引处的位值。
        public bool Get(int index)
        {
            if (index < 0 || index >= Length)
            {
                throw new ArgumentOutOfRangeException("index", "索引值超出范围");
            }
            return (m_array[index / 32] & (1 << (index % 32))) != 0;
        }

        //将位置索引处的位值设置为value。
        public void Set(int index, bool value)
        {
            if (index < 0 || index >= Length)
            {
                throw new ArgumentOutOfRangeException("index", "索引值超出范围");
            }

            if (value)
            {
                m_array[index / 32] |= (1 << (index % 32));
            }
            else
            {
                m_array[index / 32] &= ~(1 << (index % 32));
            }
        }
        public int search_Address()
        {
            for (int i = 0; i < MEMORY_SIZE; i++)
            {
                if (!Get(i))
                    return i * MEMORY_SIZE;
            }
            return -1;
        }
    }
}
