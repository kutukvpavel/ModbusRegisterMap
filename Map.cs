#define MODBUS_COMMON_MEMORY_SPACE

using Avalonia.PropertyGrid.Model.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace ModbusRegisterMap
{
    public class Map
    {
#if MODBUS_COMMON_MEMORY_SPACE
        protected int RegisterAddAddr = 0;
#endif

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
#if MODBUS_COMMON_MEMORY_SPACE
            RegisterAddAddr = 0;
#endif
            HoldingRegisters.Clear();
            InputRegisters.Clear();
            ConfigRegisters.Clear();
            PollInputRegisters.Clear();
            PollHoldingRegisters.Clear();
        }
        public IRegister? Find(string name)
        {
            IRegister? res = InputRegisters[name] as IRegister;
            if (res != null) return res;
            res = HoldingRegisters[name] as IRegister;
            return res;
        }
        public IEnumerable<IRegister> GetPollRegs()
        {
            return PollHoldingRegisters.Select(x => HoldingRegisters[x] as IRegister)
                .Concat(PollInputRegisters.Select(x => InputRegisters[x] as IRegister))
                .Where(x => x != null).Cast<IRegister>();
        }
        public IRegister GetHolding(string name, int index)
        {
            name += index.ToString();
            if (HoldingRegisters[name] is not IRegister r)
                throw new ArgumentException($"Specified holding register does not exit in this map: '{name}'");
            return r;
        }
        public IRegister GetInput(string name, int index)
        {
            name += index.ToString();
            if (InputRegisters[name] is not IRegister r)
                throw new ArgumentException($"Specified input register does not exit in this map: '{name}'");
            return r;
        }
        public ushort GetInputWord(string name)
        {
            if (InputRegisters[name] is not Register<DevUShort> res)
                throw new ArgumentException($"Specified input register does not exit or is not of type 'DevUshort': '{name}'"); 
            return res.TypedValue.Value;
        }
        public ushort GetHoldingWord(string name)
        {
            if (HoldingRegisters[name] is not Register<DevUShort> res)
                throw new ArgumentException($"Specified holding register does not exit or is not of type 'DevUshort': '{name}'"); 
            return res.TypedValue.Value;
        }
        public float GetInputFloat(string name, int index = -1)
        {
            if (index >= 0) name += index.ToString();
            if (InputRegisters[name] is not Register<DevFloat> res)
                throw new ArgumentException($"Specified input register does not exit or is not of type 'DevFloat': '{name}'"); 
            return res.TypedValue.Value;
        }
        public float GetHoldingFloat(string name, int index = -1)
        {
            if (index >= 0) name += index.ToString();
            if (HoldingRegisters[name] is not Register<DevFloat> res)
                throw new ArgumentException($"Specified holding register does not exit or is not of type 'DevFloat': '{name}'"); 
            return res.TypedValue.Value;
        }
        public void AddHolding<T>(string name, int num, bool poll = false) where T : IDeviceType, new()
        {
            Add<T>(HoldingRegisters, name, num, poll: poll, pollList: PollHoldingRegisters);
        }
        public void AddInput<T>(string name, int num, bool cfg = false, bool poll = false) where T : IDeviceType, new()
        {
            Add<T>(InputRegisters, name, num, cfg, true, poll, PollInputRegisters);
        }

        private void Add<T>(OrderedDictionary to, string name, int num, bool cfg = false, bool ro = false, bool poll = false, 
            List<string>? pollList = null) where T : IDeviceType, new()
        {
#if !MODBUS_COMMON_MEMORY_SPACE
            int RegisterAddAddr = 0;
            if (to.Count > 0)
            {
                //Do not use ^1 with OrderedDictionary index-based access!! Will return NULL.
#pragma warning disable IDE0056
                if (to[to.Count - 1] is not IRegister last) throw new Exception("This cannot happen under normal circumstances!");
#pragma warning restore IDE0056
                RegisterAddAddr = last.Address + last.Length;
            }
#endif
            string n = name;
            for (int i = 0; i < num; i++)
            {
                if (num > 1) n = name + i.ToString();
                var item = new Register<T>((ushort)RegisterAddAddr, n, ro);
                to.Add(n, item);
                if (cfg) ConfigRegisters.Add(n);
                if (poll) pollList?.Add(n);
                RegisterAddAddr += item.Length;
            }
        }
    }
}
