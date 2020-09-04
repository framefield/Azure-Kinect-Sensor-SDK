using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.Kinect.Sensor
{
    public class Playback : IDisposable
    {
        // The native handle to the device.
        private NativeMethods.k4a_playback_t handle;

        // To detect redundant calls to Dispose
        private bool disposedValue = false;

        public bool _isAtEndOfFile { get; private set; } = false;

        public static Playback Open(string filename)
        {
            NativeMethods.k4a_playback_t handle = default;
            AzureKinectOpenDeviceException.ThrowIfNotSuccess(() => NativeMethods.k4a_playback_open(filename, out handle));
            return new Playback(handle);
        }

        private Playback(NativeMethods.k4a_playback_t handle)
        {
            // Hook the native allocator and register this object.
            // .Dispose() will be called on this object when the allocator is shut down.
            Allocator.Singleton.RegisterForDisposal(this);

            this.handle = handle;
        }

        public Calibration GetCalibration()
        {
            lock (this)
            {
                if (this.disposedValue)
                    throw new ObjectDisposedException(nameof(Device));
                Calibration calibration = new Calibration();
                AzureKinectException.ThrowIfNotSuccess<NativeMethods.k4a_result_t>((Func<NativeMethods.k4a_result_t>)(() => NativeMethods.k4a_playback_get_calibration(this.handle, out calibration)));
                return calibration;
            }
        }

        public enum StreamResult
        {
            Success = 0,
            Failed,
            EndOfFile,
        }

        public (Capture, StreamResult) GetCapture()
        {
            lock (this)
            {
                if (this.disposedValue)
                    throw new ObjectDisposedException(nameof(Device));
                using (LoggingTracer tracer = new LoggingTracer())
                {
                    NativeMethods.k4a_capture_t capture_handle;
                    NativeMethods.k4a_stream_result_t capture = NativeMethods.k4a_playback_get_next_capture(this.handle, out capture_handle);
                    //                    if (capture == NativeMethods.k4a_stream_result_t.K4A_STREAM_RESULT_FAILED)
                    //                        throw new TimeoutException("Timed out waiting for capture");
                    //                    AzureKinectException.ThrowIfNotSuccess<NativeMethods.k4a_stream_result_t>(tracer, capture);
                    //                    return !capture_handle.IsInvalid ? (new Capture(capture_handle) : throw new AzureKinectException("k4a_playback_get_next_capture did not return a valid capture handle");
                    return (new Capture(capture_handle), (StreamResult)capture);
                }
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(disposing) below.
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>True</c> to release both managed and unmanaged resources; <c>False</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    Allocator.Singleton.UnregisterForDisposal(this);

                    this.handle.Close();
                    this.handle = null;

                    this.disposedValue = true;
                }
            }
        }
    }
}
