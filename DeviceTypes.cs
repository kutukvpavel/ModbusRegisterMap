using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Linq;

namespace ModbusRegisterMap
{
    public abstract class DevTypeBase : IDeviceType
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public abstract ushort Size { get; }

        public abstract object Get();
        public abstract ushort[] GetWords();
        public abstract void Set(byte[] data, int startIndex = 0);
        public abstract void Set(string data);

        protected void OnPropertyChanged(string? name = null)
        {
            Task.Run(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)));
        }
    }
    public abstract class ComplexDevTypeBase : DevTypeBase
    {
        protected IDeviceType[]? Fields;

        public override ushort[] GetWords()
        {
            if (Fields == null) 
                throw new Exception($"This ComplexDevType is not defined correctly: {Get().GetType().Name}");

            ushort[] buf = new ushort[Size];
            int i = 0;
            foreach (var item in Fields)
            {
                item.GetWords().CopyTo(buf, i);
                i += item.Size;
            }
            return buf;
        }
        public override void Set(byte[] data, int startIndex = 0)
        {
            if (Fields == null)
                throw new Exception($"This ComplexDevType is not defined correctly: {Get().GetType().Name}");

            foreach (var item in Fields)
            {
                item.Set(data, startIndex);
                startIndex += item.Size * sizeof(ushort);
            }
            OnPropertyChanged();
        }
        public override void Set(string data)
        {
            throw new NotImplementedException();
        }
    }

    public class DevFloat : DevTypeBase
    {
        public float Value { get; set; }

        public override ushort Size => 2;

        public override object Get()
        {
            return this;
        }
        public override ushort[] GetWords()
        {
            return IDeviceType.BytesToWords(BitConverter.GetBytes(Value), Size);
        }
        public override void Set(byte[] data, int startIndex = 0)
        {
            Value = BitConverter.ToSingle(data, startIndex);
            OnPropertyChanged();
        }
        public override void Set(string data)
        {
            Value = float.Parse(data, System.Globalization.NumberStyles.Float);
            OnPropertyChanged();
        }
        public override string ToString()
        {
            return Value.ToString(MathF.Abs(Value) < 0.001 ? "E6" : "F6");
        }

        public static explicit operator float(DevFloat v) => v.Value;
        public static explicit operator DevFloat(float v) => new DevFloat() { Value = v };
    }

    public class DevUShort : DevTypeBase
    {
        public ushort Value { get; set; }

        public override ushort Size => 1;

        public override object Get()
        {
            return this;
        }
        public override ushort[] GetWords()
        {
            return IDeviceType.BytesToWords(BitConverter.GetBytes(Value), Size);
        }
        public override void Set(byte[] data, int startIndex = 0)
        {
            Value = BitConverter.ToUInt16(data, startIndex);
            OnPropertyChanged();
        }
        public override void Set(string data)
        {
            Value = ushort.Parse(data);
            OnPropertyChanged();
        }
        public override string ToString()
        {
            return Value.ToString();
        }

        public static explicit operator ushort(DevUShort v) => v.Value;
        public static explicit operator DevUShort(ushort v) => new DevUShort() { Value = v };
    }

    public class DevULong : DevTypeBase
    {
        public uint Value { get; set; }

        public override ushort Size => 2;
        public override object Get()
        {
            return this;
        }
        public override ushort[] GetWords()
        {
            return IDeviceType.BytesToWords(BitConverter.GetBytes(Value), Size);
        }
        public override void Set(byte[] data, int startIndex = 0)
        {
            Value = BitConverter.ToUInt32(data, startIndex);
            OnPropertyChanged();
        }
        public override void Set(string data)
        {
            Value = uint.Parse(data);
            OnPropertyChanged();
        }
        public override string ToString()
        {
            return Value.ToString();
        }

        public static explicit operator uint(DevULong v) => v.Value;
        public static explicit operator DevULong(uint v) => new DevULong() { Value = v };
    }

    public class AioCal : ComplexDevTypeBase
    {
        public AioCal()
        {
            Fields = new IDeviceType[] {
                K,
                B
            };
        }

        public DevFloat K { get; set; } = new DevFloat();
        public DevFloat B { get; set; } = new DevFloat();

        public override ushort Size => 2 * 2;
        public override object Get()
        {
            return this;
        }
    }
}
