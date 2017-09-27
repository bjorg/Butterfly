using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading;

namespace Win32 {
    internal sealed class HiPerfTimer {

        //--- Class Fields ---
        private static long Frequency = 0;

        //--- Class Methods ---
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        internal static void StopAndStart(HiPerfTimer stop_timer, HiPerfTimer start_timer) {
            stop_timer.Stop();
            start_timer.start_counter = stop_timer.stop_counter;
        }

        //--- Fields ----
        private long start_counter;
        private long stop_counter;
        private int count;
        private double total_time;

        //--- Constructors ---
        internal HiPerfTimer() {

            // check if frequency has been set
            if(Frequency == 0) {
                if(QueryPerformanceFrequency(out Frequency) == false) {
                    throw new Win32Exception();
                }
            }
        }

        //--- Properties ---
        internal double TotalDuration {
            get {
                return total_time;
            }
        }

        internal double AverageDuration {
            get {
                return (count > 0) ? total_time / count  : 0;
            }
        }

        internal int Count {
            get {
                return count;
            }
        }

        //--- Methods ---
        internal void Clear() {
            start_counter = 0;
            stop_counter = 0;
            count = 0;
            total_time = 0.0;
        }

        internal void Start() {

            // lets do the waiting threads there work
            Thread.Sleep(0);

            QueryPerformanceCounter(out start_counter);
        }

        internal void Start(HiPerfTimer other) {
            start_counter = other.start_counter;
        }

        internal void Stop() {
            QueryPerformanceCounter(out stop_counter);
            ++count;
            total_time += (double)(stop_counter - start_counter) / (double)Frequency;
        }

        internal void Stop(HiPerfTimer other) {
            stop_counter = other.stop_counter;
            ++count;
            total_time += (double)(stop_counter - start_counter) / (double)Frequency;
        }
    }
}