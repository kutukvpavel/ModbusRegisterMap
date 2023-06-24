using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ModbusRegisterMap
{
    public class Map
    {
        public Map()
        {
            
        }

        public OrderedDictionary HoldingRegisters { get; } = new OrderedDictionary();
        public OrderedDictionary InputRegisters { get; } = new OrderedDictionary();
        public List<string> ConfigRegisters { get; } = new List<string>();
        public List<string> PollInputRegisters { get; } = new List<string>();
        public List<string> PollHoldingRegisters { get; } = new List<string>();

        public void Clear()
        {
            HoldingRegisters.Clear();
            InputRegisters.Clear();
            ConfigRegisters.Clear();
            PollInputRegisters.Clear();
            PollHoldingRegisters.Clear();
        }
        public IRegister GetHolding(string name, int index)
        {
            if (HoldingRegisters[name + index.ToString()] is not IRegister r)
                throw new ArgumentException("Specified holding register does not exit in this map");
            return r;
        }
        public IRegister GetInput(string name, int index)
        {
            if (InputRegisters[name + index.ToString()] is not IRegister r)
                throw new ArgumentException("Specified input register does not exit in this map");
            return r;
        }
        public float GetInputFloat(string name, int index = -1)
        {
            if (index >= 0) name += index.ToString();
            if (InputRegisters[name] is not Register<DevFloat> res)
                throw new ArgumentException("Specified input register does not exit or is not of type 'DevFloat'"); 
            return res.TypedValue.Value;
        }
        public float GetHoldingFloat(string name, int index = -1)
        {
            if (index >= 0) name += index.ToString();
            if (HoldingRegisters[name] is not Register<DevFloat> res)
                throw new ArgumentException("Specified holding register does not exit or is not of type 'DevFloat'"); 
            return res.TypedValue.Value;
        }
        public void AddHolding<T>(string name, int num, bool poll = false) where T : IDeviceType, new()
        {
            Add<T>(HoldingRegisters, name, num, poll: poll, pollList: PollHoldingRegisters);
        }
        public void AddInput<T>(string name, int num, bool cfg = false, bool poll = false) where T : IDeviceType, new()
        {
            Add<T>(InputRegisters, name, num, cfg, poll, PollInputRegisters);
        }

        private void Add<T>(OrderedDictionary to, string name, int num, bool cfg = false, bool poll = false, 
            List<string>? pollList = null) where T : IDeviceType, new()
        {
            int addr = 0;
            if (to.Count > 0)
            {
                //Do not use ^1 with OrderedDictionary index-based access!! Will return NULL.
#pragma warning disable IDE0056
                if (to[to.Count - 1] is not IRegister last) throw new Exception("This cannot happen under normal circumstances!");
#pragma warning restore IDE0056
                addr = last.Address + last.Length;
            }
            string n = name;
            for (int i = 0; i < num; i++)
            {
                if (num > 1) n = name + i.ToString();
                var item = new Register<T>((ushort)addr, n);
                to.Add(n, item);
                if (cfg) ConfigRegisters.Add(n);
                if (poll) pollList?.Add(n);
                addr += item.Length;
            }
        }
    }
}
