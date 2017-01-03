using System.Runtime.InteropServices;
using System.Text;
using System.Text.Formatting;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnostics.Windows;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace StringFormatterBench
{
    public class Config : ManualConfig
    {
        public Config() {
            Add(Job.LegacyJitX64, Job.RyuJitX64);
            Add(StatisticColumn.Min);
            Add(StatisticColumn.Max);
            Add(new MemoryDiagnoser());
        }
    }

    [Config(typeof(Config))]
    public class StringFormatBenchmark {
        StringBuilder _sb1;
        StringBuffer _sb2;
        StringByteBuffer _sb3;
        static readonly string formatTest = "Foo {0,13:e12} and bar!! {1,-15:P}bah";
        const double v1 = 13.934939;
        const double v2 = 0;

        [Setup]
        public void Setup() {
            _sb1 = new StringBuilder();
            _sb2 = new StringBuffer();
            _sb3 = new StringByteBuffer();
        }

        [Benchmark(Baseline = true)]
        public string Baseline() {
            _sb1.AppendFormat(formatTest, v1, v2);
            var s = _sb1.ToString();
            _sb1.Clear();
            return s;
        }

        [Benchmark]
        public string StringBuffer() {
            _sb2.AppendFormat(formatTest, v1, v2);
            var s = _sb2.ToString();
            _sb2.Clear();
            return s;
        }

        [Benchmark]
        public string StringByteBuffer() {
            _sb3.AppendFormat(formatTest, v1, v2);
            var s = _sb3.ToString();
            _sb3.Clear();
            return s;
        }
    }

    [Config(typeof(Config))]
    public unsafe class NoAllocationBenchmark {
        StringBuilder _sb1;
        StringBuffer _sb2;
        StringByteBuffer _sb3;
        public char* DestNative;
        public char[] DestManaged;
        static readonly string formatTest = "Foo {0,13:e12} and bar!! {1,-15:P}bah";

        const double v1 = 13.934939;
        const double v2 = 0;

        [Setup]
        public unsafe void Setup() {
            DestNative = (char*)Marshal.AllocHGlobal(128);
            DestManaged = new char[128];
            _sb1 = new StringBuilder();
            _sb2 = new StringBuffer();
            _sb3 = new StringByteBuffer();
        }

        [Benchmark(Baseline = true)]
        public void Baseline() {
            _sb1.AppendFormat(formatTest, v1, v2);
            _sb1.CopyTo(0, DestManaged, 0, _sb1.Length);
            _sb1.Clear();
        }

        [Benchmark]
        public void NoAllocation() {
            _sb2.AppendFormat(formatTest, v1, v2);
            _sb2.CopyTo(DestNative, 0, _sb2.Count);
            _sb2.Clear();
        }

        /*[Benchmark]
        public void NoAllocationByte() {
            _sb3.AppendFormat(formatTest, v1, v2);
            _sb3.CopyTo(DestNative, 0, _sb3.Count);
            _sb3.Clear();
        }*/
    }

    [Config(typeof(Config))]
    public unsafe class BytesFormatBenchmark {
        StringBuilder _sb1;
        StringBuffer _sb2;
        StringByteBuffer _sb3;
        static readonly string formatTest = "Foo {0,13:e12} and bar!! {1,-15:P}bah";
        const double v1 = 13.934939;
        const double v2 = 0;
        public byte* DestNative;
        public byte[] DestManaged;

        [Setup]
        public void Setup() {
            DestNative = (byte*)Marshal.AllocHGlobal(256);
            DestManaged = new byte[256];
            _sb1 = new StringBuilder();
            _sb2 = new StringBuffer();
            _sb3 = new StringByteBuffer();
        }

        [Benchmark(Baseline = true)]
        public void Baseline() {
            _sb1.AppendFormat(formatTest, v1, v2);
            var s = _sb1.ToString();
            Encoding.Default.GetBytes(s, 0, s.Length, DestManaged, 0);
            _sb1.Clear();
        }

        [Benchmark]
        public void StringBuffer() {
            _sb2.AppendFormat(formatTest, v1, v2);
            _sb2.CopyTo(DestNative, 0, _sb2.Count, 256, Encoding.Default);
            _sb2.Clear();
        }

        [Benchmark]
        public void StringByteBuffer() {
            _sb3.AppendFormat(formatTest, v1, v2);
            _sb3.CopyTo(DestNative, 0, _sb3.Count, 256, Encoding.Default);
            _sb3.Clear();
        }
    }

    unsafe class Program {
        static unsafe void Main(string[] args) {
            var competition = new BenchmarkSwitcher(new[] {
                typeof(StringFormatBenchmark),
                typeof(NoAllocationBenchmark),
                typeof(BytesFormatBenchmark),
            });

            competition.Run(args);
        }
    }
}
