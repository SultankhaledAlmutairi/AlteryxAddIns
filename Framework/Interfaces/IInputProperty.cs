﻿using System;
using OmniBus.Framework.EventHandlers;

namespace OmniBus.Framework.Interfaces
{
    /// <summary>
    ///     Incoming Connection Engine Interface
    /// </summary>
    public interface IInputProperty : AlteryxRecordInfoNet.IIncomingConnectionInterface
    {
        /// <summary>
        ///     Event When Alteryx Calls <see cref="AlteryxRecordInfoNet.IIncomingConnectionInterface.II_Init" />
        /// </summary>
        event Action<IInputProperty, SuccessEventArgs> InitCalled;

        /// <summary>
        ///     Event When Alteryx Pushes A Record On The Input
        /// </summary>
        event Action<IInputProperty, AlteryxRecordInfoNet.RecordData, SuccessEventArgs> RecordPushed;

        /// <summary>
        ///     Event when Alteryx updates progress on the input
        /// </summary>
        event Action<IInputProperty, double> ProgressUpdated;

        /// <summary>
        ///     Event when Alteryx closes the input
        /// </summary>
        event Action<IInputProperty> Closed;

        /// <summary>
        ///     Gets the current state.
        /// </summary>
        ConnectionState State { get; }

        /// <summary>
        ///     Gets the record information of incoming stream.
        /// </summary>
        AlteryxRecordInfoNet.RecordInfo RecordInfo { get; }

        /// <summary>
        ///     Gets the record copier for this property.
        /// </summary>
        IRecordCopier Copier { get; }
    }
}