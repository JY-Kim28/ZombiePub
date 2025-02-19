using System;
using UnityEngine;
using System.Collections.Generic;

namespace DopaminTable
{
    [Serializable]
    public struct Table_MachineLevel200Data
    {
        public uint id {  get; set; }
        public ushort machineType {  get; set; }
        public ushort level {  get; set; }
        public List<uint> buffList {  get; set; }
        public List<float> buffValue {  get; set; }
        public List<uint> specialBuffList {  get; set; }
        public List<float> specialBuffValue {  get; set; }
    }

    public class Table_MachineLevel200 : Table<Table_MachineLevel200Data>
    {
        public ushort Version { get; private set; } = 1;

        public Table_MachineLevel200Data GetData(uint id)
        {
             return Data.ContainsKey(id) ? Data[id] : default;
        }

        public static uint CreateCode(ushort middle, ushort last)
        {
            if (middle > 999) middle = 999;
            if (last > 999) last = 999;

            return (uint)(200000000 + (middle * 1000) + last);
        }
    }
}
