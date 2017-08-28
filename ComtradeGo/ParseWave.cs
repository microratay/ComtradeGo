using System;
using System.Collections.Generic;
using System.IO;
using static ComtradeGo.Universal;

namespace ComtradeGo
{
    using Analog = List<AnalogData>;
    using AnalogRaw = List<double>;
    using Digital = List<DigitalData>;
    using DigitalRaw = List<bool>;
    using Stamp = List<DateTime>;

    struct AnalogData
    {
        public AnalogRaw Data;
        public string Name;
    }

    struct DigitalData
    {
        public DigitalRaw Data;
        public string Name;
    }

    class ParseWave
    {
        public string Device;
        public DateTime Fault;
        public Stamp Stamp;
        public Analog Analog;
        public Digital Digital;
        public string FilePath;

        byte[] mData;
        string[][] mConfig;
        int mCountA;
        int mCountD;

        public ParseWave(string path)
        {
            FilePath = path;
            LoadFile(FilePath);
            Device = mConfig[0][0];
            Fault = ReadStamp(6);
            CreateStamp();
            CreateAnalog();
            CreateDigital();
        }

        void LoadFile(string path)
        {
            string dat = $"{path}.dat";
            string cfg = $"{path}.cfg";
            mData = File.ReadAllBytes(dat);
            mConfig = Read_CSV(cfg);
            string textA = mConfig[1][1];
            textA = RemoveText(textA, "A");
            string textD = mConfig[1][2];
            textD = RemoveText(textD, "D");
            mCountA = int.Parse(textA);
            mCountD = int.Parse(textD);
        }

        void CreateStamp()
        {
            Stamp = new Stamp();
            int sum = (mCountA + mCountD);
            string[] row = mConfig[sum + 4];
            double rate = double.Parse(row[0]);
            int total = int.Parse(row[1]);
            DateTime stamp = ReadStamp(5);
            if (stamp.Second == 59)
            {
                stamp = stamp.AddSeconds(1);
                Fault = Fault.AddSeconds(1);
            }
            int tick = (int)(10000000 / rate);
            for (int i = 0; i < total; i++)
            {
                Stamp.Add(stamp);
                stamp = stamp.AddTicks(tick);
            }
        }

        void CreateAnalog()
        {
            Analog = new Analog();
            for (int i = 0; i < mCountA; i++)
            {
                string[] row = mConfig[i + 2];
                string name = row[1];
                string unit = row[4];
                double k = double.Parse(row[5]);
                double b = double.Parse(row[6]);
                AnalogRaw data = ReadAnalog(i);
                for (int m = 0; m < data.Count; m++)
                {
                    data[m] = (data[m] * k + b);
                }
                Analog.Add(new AnalogData()
                {
                    Data = data,
                    Name = $"{name}（{unit}）"
                });
            }
        }

        void CreateDigital()
        {
            Digital = new Digital();
            for (int i = 0; i < mCountD; i++)
            {
                int index = (mCountA + i + 2);
                Digital.Add(new DigitalData()
                {
                    Data = ReadDigital(i),
                    Name = mConfig[index][1]
                });
            }
        }

        DateTime ReadStamp(int offset)
        {
            int sum = (mCountA + mCountD);
            int index = (sum + offset);
            string text1 = mConfig[index][0];
            string text2 = mConfig[index][1];
            string text = $"{text1},{text2}";
            string[] dateStr = text1.Split('/');
            string mask1 = (dateStr[2].Length == 4) ? "dd/MM/yyyy" : "dd/MM/yy";
            string mask2 = "HH:mm:ss.ffffff";
            string mask = $"{mask1},{mask2}";
            return ParseStamp(text, $"{mask1},{mask2}");
        }

        AnalogRaw ReadAnalog(int index)
        {
            int offset = (index * 2 + 8);
            AnalogRaw raw = new AnalogRaw();
            for (int i = 0; i < Stamp.Count; i++)
            {
                raw.Add(ByteToS16(mData, offset));
                offset += (mCountA * 2 + 8);
                offset += ((mCountD > 0) ? 2 : 0);
            }
            return raw;
        }

        DigitalRaw ReadDigital(int index)
        {
            int offset = 0;
            DigitalRaw raw = new DigitalRaw();
            for (int i = 0; i < Stamp.Count; i++)
            {
                offset += (mCountA * 2 + 10);
                byte value = mData[offset - 2];
                raw.Add(TestBit(value, index));
            }
            return raw;
        }
    }
}
