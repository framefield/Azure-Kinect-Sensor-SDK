﻿//------------------------------------------------------------------------------
// <copyright file="AzureKinectException.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Kinect.Sensor
{
    /// <summary>
    /// Represents errors that occur when interacting with the Azure Kinect Sensor SDK.
    /// </summary>
    [Serializable]
    public class AzureKinectException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureKinectException"/> class.
        /// </summary>
        public AzureKinectException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureKinectException"/> class with a
        /// specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public AzureKinectException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureKinectException"/> class with a
        /// specified error message and a reference to the inner exception that is the cause of
        /// this exception.
        /// </summary>
        /// <param name="message">
        /// The error message that explains the reason for the exception.
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.
        /// </param>
        public AzureKinectException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureKinectException"/> class with
        /// serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the
        /// exception being thrown.</param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/>System.Runtime.Serialization.StreamingContext that
        /// contains contextual information about the source or destination.
        /// </param>
        protected AzureKinectException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureKinectException"/> class with a
        /// specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="logMessages">
        /// The log messages that happened during the function call that generated this error.
        /// </param>
        protected AzureKinectException(string message, ICollection<LogMessage> logMessages)
            : base(message)
        {
            this.LogMessages = logMessages;
        }

        /// <summary>
        /// Gets the log messages that happened during the function call that generated this
        /// error.
        /// </summary>
        public ICollection<LogMessage> LogMessages { get; }

        /// <summary>
        /// Throws an <see cref="AzureKinectException"/> if the result of the function is not
        /// a success.
        /// </summary>
        /// <param name="function">The native function to call.</param>
        /// <typeparam name="T">The type of result to expect from the function call.</typeparam>
        internal static void ThrowIfNotSuccess<T>(Func<T> function)
            where T : System.Enum
        {
            using (LoggingTracer tracer = new LoggingTracer())
            {
                T result = function();
                if (!AzureKinectException.IsSuccess(result))
                {
                    throw new AzureKinectException($"result = {result}", tracer.LogMessages);
                }
            }
        }

        /// <summary>
        /// Throws an <see cref="AzureKinectException"/> if the result of the function is not
        /// a success.
        /// </summary>
        /// <param name="tracer">The tracer is that is capturing logging messages.</param>
        /// <param name="result">The result native function to call.</param>
        /// <typeparam name="T">The type of result to expect from the function call.</typeparam>
        internal static void ThrowIfNotSuccess<T>(LoggingTracer tracer, T result)
            where T : System.Enum
        {
            if (!AzureKinectException.IsSuccess(result))
            {
                throw new AzureKinectException($"result = {result}", tracer.LogMessages);
            }
        }

        /// <summary>
        /// Determines if the result is a success result.
        /// </summary>
        /// <typeparam name="T">The type of result.</typeparam>
        /// <param name="result">The result to check if it is a success.</param>
        /// <returns><c>True</c> if the result is a success;otherwise <c>false</c>.</returns>
        internal static bool IsSuccess<T>(T result)
            where T : Enum
        {
            switch (result)
            {
                case NativeMethods.k4a_result_t k4a_result:
                    return IsSuccess(k4a_result);

                case NativeMethods.k4a_wait_result_t k4a_result:
                    return IsSuccess(k4a_result);

                case NativeMethods.k4a_buffer_result_t k4a_result:
                    return IsSuccess(k4a_result);

                case NativeMethods.k4a_stream_result_t k4a_result:
                    return IsSuccess(k4a_result);

                default:
                    throw new ArgumentException("Result is not of a recognized result type.", nameof(result));
            }
        }

        /// <summary>
        /// Determines if the <see cref="NativeMethods.k4a_result_t"/> is a success.
        /// </summary>
        /// <param name="result">The result to check if it is a success.</param>
        /// <returns><c>True</c> if the result is a success;otherwise <c>false</c>.</returns>
        internal static bool IsSuccess(NativeMethods.k4a_result_t result)
        {
            return result == NativeMethods.k4a_result_t.K4A_RESULT_SUCCEEDED;
        }

        /// <summary>
        /// Determines if the <see cref="NativeMethods.k4a_wait_result_t"/> is a success.
        /// </summary>
        /// <param name="result">The result to check if it is a success.</param>
        /// <returns><c>True</c> if the result is a success;otherwise <c>false</c>.</returns>
        internal static bool IsSuccess(NativeMethods.k4a_wait_result_t result)
        {
            return result == NativeMethods.k4a_wait_result_t.K4A_WAIT_RESULT_SUCCEEDED;
        }

        /// <summary>
        /// Determines if the <see cref="NativeMethods.k4a_buffer_result_t"/> is a success.
        /// </summary>
        /// <param name="result">The result to check if it is a success.</param>
        /// <returns><c>True</c> if the result is a success;otherwise <c>false</c>.</returns>
        internal static bool IsSuccess(NativeMethods.k4a_buffer_result_t result)
        {
            return result == NativeMethods.k4a_buffer_result_t.K4A_BUFFER_RESULT_SUCCEEDED;
        }

        internal static bool IsSuccess(NativeMethods.k4a_stream_result_t result)
        {
            return result == NativeMethods.k4a_stream_result_t.K4A_STREAM_RESULT_SUCCEEDED;
        }

    }
}
