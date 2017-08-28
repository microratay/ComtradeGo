using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ComtradeGo
{
    static class Universal
    {
        public static string AbsolutePath(string path)
        {
            string root = Application.StartupPath;
            return Path.Combine(root, path);
        }

        public static byte[] TrimZero(byte[] buffer)
        {
            List<byte> result = new List<byte>();
            foreach (byte item in buffer)
            {
                if (item == 0)
                {
                    return result.ToArray();
                }
                result.Add(item);
            }
            return buffer;
        }

        public static string[][] Read_CSV(string path)
        {
            Encoding code = Encoding.Default;
            string[] input = File.ReadAllLines(path, code);
            List<string[]> result = new List<string[]>();
            foreach (string line in input)
            {
                if (line != string.Empty)
                {
                    result.Add(line.Split(','));
                }
            }
            return result.ToArray();
        }

        public static double StampToValue(DateTime stamp)
        {
            string format = "HHmmss.ffffff";
            string text = stamp.ToString(format);
            return double.Parse(text);
        }

        public static string[] GetPortList()
        {
            return SerialPort.GetPortNames();
        }

        public static string[] GetFileList(string path)
        {
            DirectoryInfo list = new DirectoryInfo(path);
            List<string> result = new List<string>();
            foreach (FileInfo item in list.GetFiles())
            {
                result.Add(ExtractName(item.Name));
            }
            return result.ToArray();
        }

        public static string[] GetFolderList(string path)
        {
            DirectoryInfo list = new DirectoryInfo(path);
            List<string> result = new List<string>();
            foreach (DirectoryInfo item in list.GetDirectories())
            {
                result.Add(item.Name);
            }
            return result.ToArray();
        }

        public static bool YesOrNo(string text, string head)
        {
            DialogResult yes = DialogResult.Yes;
            MessageBoxButtons type = MessageBoxButtons.YesNo;
            return (MessageBox.Show(text, head, type) == yes);
        }

        public static bool YesOrNo(string text)
        {
            return YesOrNo(text, "提示");
        }

        public static bool ValueEqual(object value1, object value2)
        {
            if ((value1 == null) || (value2 == null))
            {
                return (value1 == value2);
            }
            return value1.Equals(value2);
        }

        public static Font LoadFont(string path, float size)
        {
            PrivateFontCollection font = new PrivateFontCollection();
            font.AddFontFile(path);
            return new Font(font.Families[0], size);
        }

        public static string RemoveText(string text, string target)
        {
            return text.Replace(target, string.Empty);
        }

        public static DateTime ParseStamp(string text, string format)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;
            return DateTime.ParseExact(text, format, culture);
        }

        public static T ParseEnum<T>(string value)
        {
            return (T)(Enum.Parse(typeof(T), value));
        }

        public static string MakeEnum(dynamic value)
        {
            return Enum.GetName(value.GetType(), value);
        }

        public static string ExtractName(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        public static bool TestBit(byte value, int offset)
        {
            byte result = (byte)(1 << offset);
            return ((result & value) == result);
        }

        public static byte MaskBit(bool value, int offset)
        {
            byte result = (byte)(1 << offset);
            return (byte)(value ? result : 0x00);
        }

        public static int FileLength(string path)
        {
            return File.ReadAllBytes(path).Length;
        }

        public static Control FindItem(Form form, string name)
        {
            return form.Controls.Find(name, true)[0];
        }

        public static ListViewItem MakeItem(params string[] text)
        {
            return new ListViewItem(text);
        }

        public static T[] RepeatArray<T>(T value, int size)
        {
            T[] result = new T[size];
            for (int i = 0; i < size; i++)
            {
                result[i] = value;
            }
            return result;
        }

        public static bool CompareArray<T>(T[] buffer1, T[] buffer2)
        {
            if (buffer1.Length == buffer2.Length)
            {
                for (int i = 0; i < buffer1.Length; i++)
                {
                    if (buffer1[i].Equals(buffer2[i]) == false)
                    {
                        return false;
                    }
                }
                return true;
            }
            else return false;
        }

        public static T[] CutArray<T>(T[] buffer, int offset, int size)
        {
            T[] result = new T[size];
            for (int i = 0; i < size; i++)
            {
                result[i] = buffer[i + offset];
            }
            return result;
        }

        public static T[] InsertArray<T>(T[] buffer, int offset, int size)
        {
            T[] result = new T[buffer.Length + size];
            for (int i = 0; i < offset; i++)
            {
                result[i] = buffer[i];
            }
            for (int i = offset; i < buffer.Length; i++)
            {
                result[i + size] = buffer[i];
            }
            return result;
        }

        public static T[] DeleteArray<T>(T[] buffer, int offset, int size)
        {
            T[] result = new T[buffer.Length - size];
            for (int i = 0; i < offset; i++)
            {
                result[i] = buffer[i];
            }
            for (int i = offset + size; i < buffer.Length; i++)
            {
                result[i - size] = buffer[i];
            }
            return result;
        }

        public static byte CheckSum(byte[] buffer, int offset, int size)
        {
            byte result = 0;
            for (int i = 0; i < size; i++)
            {
                result += buffer[i + offset];
            }
            return result;
        }

        public static byte CheckSum(byte[] buffer, int size)
        {
            return CheckSum(buffer, 0, size);
        }

        public static byte CheckSum(byte[] buffer)
        {
            return CheckSum(buffer, 0, buffer.Length);
        }

        public static byte[] TextToArray(string text)
        {
            text = RemoveText(text, " ");
            if (text.Length > 0)
            {
                List<byte> result = new List<byte>();
                for (int i = 0; i < text.Length; i += 2)
                {
                    string item = text.Substring(i, 2);
                    result.Add(Convert.ToByte(item, 16));
                }
                return result.ToArray();
            }
            else throw null;
        }

        public static string ArrayToText(byte[] buffer, int offset, int size)
        {
            string result = string.Empty;
            for (int i = 0; i < size; i++)
            {
                result += (buffer[i + offset].ToString("X2") + " ");
            }
            return result.TrimEnd();
        }

        public static string ArrayToText(byte[] buffer, int size)
        {
            return ArrayToText(buffer, 0, size);
        }

        public static string ArrayToText(byte[] buffer)
        {
            return ArrayToText(buffer, 0, buffer.Length);
        }

        public static UInt16 ByteToU16(byte[] buffer, int offset)
        {
            return BitConverter.ToUInt16(buffer, offset);
        }

        public static UInt32 ByteToU32(byte[] buffer, int offset)
        {
            return BitConverter.ToUInt32(buffer, offset);
        }

        public static Int16 ByteToS16(byte[] buffer, int offset)
        {
            return BitConverter.ToInt16(buffer, offset);
        }

        public static Int32 ByteToS32(byte[] buffer, int offset)
        {
            return BitConverter.ToInt32(buffer, offset);
        }

        public static float ByteToF32(byte[] buffer, int offset)
        {
            return BitConverter.ToSingle(buffer, offset);
        }

        public static double ByteToF64(byte[] buffer, int offset)
        {
            return BitConverter.ToDouble(buffer, offset);
        }

        public static void ByteFromU16(UInt16 value, byte[] buffer, int offset)
        {
            byte[] result = BitConverter.GetBytes(value);
            Array.Copy(result, 0, buffer, offset, result.Length);
        }

        public static void ByteFromU32(UInt32 value, byte[] buffer, int offset)
        {
            byte[] result = BitConverter.GetBytes(value);
            Array.Copy(result, 0, buffer, offset, result.Length);
        }

        public static void ByteFromS16(Int16 value, byte[] buffer, int offset)
        {
            byte[] result = BitConverter.GetBytes(value);
            Array.Copy(result, 0, buffer, offset, result.Length);
        }

        public static void ByteFromS32(Int32 value, byte[] buffer, int offset)
        {
            byte[] result = BitConverter.GetBytes(value);
            Array.Copy(result, 0, buffer, offset, result.Length);
        }

        public static void ByteFromF32(float value, byte[] buffer, int offset)
        {
            byte[] result = BitConverter.GetBytes(value);
            Array.Copy(result, 0, buffer, offset, result.Length);
        }

        public static void ByteFromF64(double value, byte[] buffer, int offset)
        {
            byte[] result = BitConverter.GetBytes(value);
            Array.Copy(result, 0, buffer, offset, result.Length);
        }

        public static UInt16 ParseHex16(string text)
        {
            return Convert.ToUInt16(text, 16);
        }

        public static UInt32 ParseHex32(string text)
        {
            return Convert.ToUInt32(text, 16);
        }

        public static string MakeHex16(UInt16 value)
        {
            return value.ToString("X4");
        }

        public static string MakeHex32(UInt32 value)
        {
            return value.ToString("X8");
        }

        public static dynamic RawToValue(Type type, byte[] buffer)
        {
            switch (type.Name)
            {
                case nameof(Byte):
                    return buffer[0];

                case nameof(SByte):
                    return (sbyte)buffer[0];

                case nameof(UInt16):
                    return BitConverter.ToUInt16(buffer, 0);

                case nameof(Int16):
                    return BitConverter.ToInt16(buffer, 0);

                case nameof(UInt32):
                    return BitConverter.ToUInt32(buffer, 0);

                case nameof(Int32):
                    return BitConverter.ToInt32(buffer, 0);

                case nameof(UInt64):
                    return BitConverter.ToUInt64(buffer, 0);

                case nameof(Int64):
                    return BitConverter.ToInt64(buffer, 0);

                case nameof(Single):
                    return BitConverter.ToSingle(buffer, 0);

                case nameof(Double):
                    return BitConverter.ToDouble(buffer, 0);

                case nameof(Boolean):
                    return BitConverter.ToBoolean(buffer, 0);

                case nameof(String):
                    return Encoding.ASCII.GetString(buffer);

                case nameof(Array):
                    return ArrayToText(buffer);

                default:
                    return null;
            }
        }

        public static byte[] ValueToRaw(Type type, dynamic value)
        {
            switch (type.Name)
            {
                case nameof(Byte):
                case nameof(SByte):
                    return new byte[] { (byte)value };

                case nameof(UInt16):
                case nameof(Int16):
                case nameof(UInt32):
                case nameof(Int32):
                case nameof(UInt64):
                case nameof(Int64):
                case nameof(Single):
                case nameof(Double):
                case nameof(Boolean):
                    return BitConverter.GetBytes(value);

                case nameof(String):
                    return Encoding.ASCII.GetBytes(value);

                case nameof(Array):
                    return TextToArray(value);

                default:
                    return null;
            }
        }

        public static T RawToValue<T>(byte[] buffer)
        {
            return RawToValue(typeof(T), buffer);
        }

        public static byte[] ValueToRaw<T>(T value)
        {
            return ValueToRaw(typeof(T), value);
        }

        public static bool Contain<T>(T value, params T[] set)
        {
            foreach (T item in set)
            {
                if (item.Equals(value) == true)
                {
                    return true;
                }
            }
            return false;
        }

        public static string CallerMethod()
        {
            StackFrame method = new StackFrame(2);
            return method.GetMethod().Name;
        }

        public static DateTime CompileTime()
        {
            string path = Application.ExecutablePath;
            return File.GetLastWriteTime(path);
        }

        public static double GetRandom()
        {
            int seed = new Object().GetHashCode();
            return new Random(seed).NextDouble();
        }

        public static void TryCatch(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "异常");
            }
        }

        public static bool Execute(Action action)
        {
            try
            {
                action();
                return true;
            }
            catch { }
            return false;
        }

        public static bool Execute<T>(dynamic table, T key)
        {
            if (table.ContainsKey(key) == true)
            {
                table[key]();
                return true;
            }
            else return false;
        }
    }

    static class Extend
    {
        public static T Cast<T>(this string text)
        {
            return (T)(typeof(T).Cast(text));
        }

        public static dynamic Cast(this Type type, string text)
        {
            return Convert.ChangeType(text, type);
        }

        public static bool Check<T>(this string text)
        {
            return Universal.Execute(() => text.Cast<T>());
        }

        public static bool Check(this Type type, string text)
        {
            return Universal.Execute(() => type.Cast(text));
        }

        public static bool IsMatch(this string text, string regex)
        {
            return Regex.IsMatch(text, regex);
        }

        public static Color FindColor(this ListView item)
        {
            Color color1 = Color.FromArgb(255, 255, 255);
            Color color2 = Color.FromArgb(240, 240, 240);
            int count = item.Items.Count;
            bool even = ((count % 2) == 0);
            return (even ? color1 : color2);
        }

        public static void CopyText(this ListView item)
        {
            int index = item.SelectedIndices[0];
            string text1 = item.Items[index].SubItems[0].Text;
            string text2 = item.Items[index].SubItems[1].Text;
            Clipboard.SetText($"{text1} {text2}");
        }

        public static string ExportDemo(this ListView item)
        {
            string result = string.Empty;
            string line = Environment.NewLine;
            foreach (ListViewItem row in item.Items)
            {
                result += (row.SubItems[0].Text + " 【");
                result += (row.SubItems[1].Text + "】 ");
                result += (row.SubItems[2].Text + line);
            }
            bool check = (item.Items.Count > 0);
            return (check ? (result + line) : string.Empty);
        }

        public static string ExportChild(this ListView item)
        {
            string result = string.Empty;
            string line = Environment.NewLine;
            result += (item.Columns[0].Text + ",");
            result += (item.Columns[1].Text + ",");
            result += (item.Columns[2].Text + ",");
            result += (item.Columns[3].Text + line);
            foreach (ListViewItem row in item.Items)
            {
                result += (row.SubItems[0].Text + ",");
                result += (row.SubItems[1].Text + ",");
                result += (row.SubItems[2].Text + ",");
                result += (row.SubItems[3].Text + line);
            }
            bool check = (item.Items.Count > 0);
            return (check ? (result + line) : string.Empty);
        }

        public static void AvoidEmpty(this NumericUpDown item)
        {
            item.Leave += (sender, e) =>
            {
                decimal value = item.Value;
                item.Value = item.Maximum;
                item.Value = item.Minimum;
                item.Value = value;
            };
        }

        public static void AutoUpper(this TextBox item)
        {
            item.Leave += (sender, e) =>
            {
                string text = item.Text;
                item.Text = text.ToUpper();
            };
        }

        public static void AutoLower(this TextBox item)
        {
            item.Leave += (sender, e) =>
            {
                string text = item.Text;
                item.Text = text.ToLower();
            };
        }

        public static void StopBlink(this Control item)
        {
            Type type = item.GetType();
            string name = "DoubleBuffered";
            BindingFlags flag = BindingFlags.Instance;
            flag |= BindingFlags.NonPublic;
            PropertyInfo field = type.GetProperty(name, flag);
            field.SetValue(item, true, null);
        }

        public static void EnableDrag(this Control item)
        {
            item.Cursor = Cursors.SizeAll;
            item.MouseDown += (sender, e) =>
            {
                item.Tag = e.Location;
            };
            item.MouseMove += (sender, e) =>
            {
                if (item.Tag != null)
                {
                    Point point = (Point)(item.Tag);
                    item.Left += (e.X - point.X);
                    item.Top += (e.Y - point.Y);
                }
            };
            item.MouseUp += (sender, e) =>
            {
                item.Tag = null;
            };
        }

        public static Bitmap GetBitmap(this Control item)
        {
            Bitmap result = new Bitmap(item.Width, item.Height);
            item.DrawToBitmap(result, item.ClientRectangle);
            return result;
        }

        public static string DropHead(this string text, int size)
        {
            return text.Remove(0, size);
        }

        public static string DropTail(this string text, int size)
        {
            return text.Remove(text.Length - size);
        }

        public static string GetHead(this string text, int size)
        {
            return text.Remove(size);
        }

        public static string GetTail(this string text, int size)
        {
            return text.Remove(0, text.Length - size);
        }

        public static T[] AddHead<T>(this T[] buffer1, T[] buffer2)
        {
            T[] result = new T[buffer1.Length + buffer2.Length];
            buffer2.CopyTo(result, 0);
            buffer1.CopyTo(result, buffer2.Length);
            return result;
        }

        public static T[] AddHead<T>(this T[] buffer, T value)
        {
            return buffer.AddHead(new T[] { value });
        }

        public static T[] AddTail<T>(this T[] buffer1, T[] buffer2)
        {
            return buffer2.AddHead(buffer1);
        }

        public static T[] AddTail<T>(this T[] buffer, T value)
        {
            return buffer.AddTail(new T[] { value });
        }

        public static T[] EmptyHead<T>(this T[] buffer, int size)
        {
            return buffer.AddHead(new T[size]);
        }

        public static T[] EmptyTail<T>(this T[] buffer, int size)
        {
            return buffer.AddTail(new T[size]);
        }

        public static T[] DropHead<T>(this T[] buffer, int size)
        {
            return buffer.Split(size)[1];
        }

        public static T[] DropTail<T>(this T[] buffer, int size)
        {
            return buffer.Split(buffer.Length - size)[0];
        }

        public static T[] GetHead<T>(this T[] buffer, int size)
        {
            return buffer.Split(size)[0];
        }

        public static T[] GetTail<T>(this T[] buffer, int size)
        {
            return buffer.Split(size)[1];
        }

        public static T[][] Split<T>(this T[] buffer, int index)
        {
            int size = buffer.Length - index;
            T[] result1 = Universal.CutArray(buffer, 0, index);
            T[] result2 = Universal.CutArray(buffer, index, size);
            return new T[][] { result1, result2 };
        }

        public static T[] Merge<T>(this T[][] buffer)
        {
            T[] result = new T[0];
            foreach (T[] item in buffer)
            {
                result = result.AddTail(item);
            }
            return result;
        }

        public static T[] Merge<T>(this List<T[]> list)
        {
            return Merge(list.ToArray());
        }

        public static void Save(this byte[] buffer, string path)
        {
            File.WriteAllBytes(path, buffer);
        }

        public static string TextFull(this DateTime stamp)
        {
            return stamp.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        public static string TextLong(this DateTime stamp)
        {
            return stamp.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string TextMini(this DateTime stamp)
        {
            return stamp.ToString("HH:mm:ss.fff");
        }

        public static string TextShort(this DateTime stamp)
        {
            return stamp.ToString("yyMMdd_HHmmss");
        }
    }

    class ObjectHash<T>
    {
        Dictionary<object, T> mHash;

        public ObjectHash()
        {
            mHash = new Dictionary<object, T>();
        }

        public T this[object key]
        {
            set { mHash[key] = value; }
            get { return mHash[key]; }
        }

        public void Add(object key, T value)
        {
            mHash.Add(key, value);
        }

        public void Remove(object key)
        {
            mHash.Remove(key);
        }

        public void Clear()
        {
            mHash.Clear();
        }

        public bool Contain(object key)
        {
            return mHash.ContainsKey(key);
        }
    }
}
