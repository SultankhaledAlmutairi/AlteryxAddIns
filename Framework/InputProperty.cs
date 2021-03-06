﻿using System;
using System.Xml;

using AlteryxRecordInfoNet;

using OmniBus.Framework.EventHandlers;
using OmniBus.Framework.Interfaces;

namespace OmniBus.Framework
{
    /// <summary>
    ///     Handle the connection to Alteryx.
    ///     Responds to the interface
    ///     Has a state property
    /// </summary>
    /// <seealso cref="IInputProperty" />
    public class InputProperty : IInputProperty
    {
        private readonly Lazy<IRecordCopier> _lazyCopier;

        private readonly Func<bool> _showDebugMessagesFunc;

        private readonly ProgressUpdatedEventArgs _progressUpdatedEventArgs;

        /// <summary>
        ///     Initializes a new instance of the <see cref="InputProperty" /> class.
        /// </summary>
        /// <param name="copierFactory">Factory for creating <see cref="IRecordCopier" /> objects.</param>
        /// <param name="showDebugMessagesFunc">Call back function to determine whether to show debug messages.</param>
        internal InputProperty(IRecordCopierFactory copierFactory = null, Func<bool> showDebugMessagesFunc = null)
        {
            this._lazyCopier =
                new Lazy<IRecordCopier>(() => copierFactory?.CreateCopier(this.RecordInfo, this.RecordInfo));

            this._showDebugMessagesFunc = showDebugMessagesFunc ?? (() => false);

            this._progressUpdatedEventArgs = new ProgressUpdatedEventArgs(double.NaN);
        }

        /// <summary>
        ///     Event When Alteryx Calls <see cref="IIncomingConnectionInterface.II_Init" />
        /// </summary>
        public event SuccessEventHandler InitCalled;

        /// <summary>
        ///     Event When A Record Is Pushed
        /// </summary>
        public event RecordPushedEventHandler RecordPushed;

        /// <summary>
        ///     Event to update progress
        /// </summary>
        public event ProgressUpdatedEventHandler ProgressUpdated;

        /// <summary>
        ///     Event when Alteryx Closes the Input
        /// </summary>
        public event ClosedEventHandler Closed;

        /// <summary>
        ///     Gets the current state.
        /// </summary>
        public ConnectionState State { get; private set; }

        /// <summary>
        ///     Gets the record information of incoming stream.
        /// </summary>
        public RecordInfo RecordInfo { get; private set; }

        /// <summary>
        ///     Gets the record copier for this property.
        /// </summary>
        public IRecordCopier Copier => this._lazyCopier.Value;

        /// <summary>
        ///     Called by Alteryx to determine if the incoming data should be sorted.
        /// </summary>
        /// <param name="pXmlProperties">The XML COnfiguration Properties</param>
        /// <returns>Null To  Do Nothing, Xml To Sort or Filter Columns</returns>
        public XmlElement II_GetPresortXml(XmlElement pXmlProperties)
        {
            this.State = ConnectionState.Added;

            // ToDo: Render Xml Output
            return null;
        }

        /// <summary>
        ///     Called by Alteryx to initialize the incoming connection.
        /// </summary>
        /// <param name="recordInfo">The record information.</param>
        /// <returns>True if OK</returns>
        public bool II_Init(RecordInfo recordInfo)
        {
            this.State = ConnectionState.InitCalled;
            this.RecordInfo = recordInfo;

            var args = new SuccessEventArgs();
            this.InitCalled?.Invoke(this, args);
            return args.Success;
        }

        /// <summary>
        ///     Called by Alteryx to send each data record to the tool.
        /// </summary>
        /// <param name="pRecord">The new record</param>
        /// <returns>True if Ok</returns>
        public bool II_PushRecord(RecordData pRecord)
        {
            var args = new RecordPushedEventArgs(pRecord);
            this.RecordPushed?.Invoke(this, args);
            return args.Success;
        }

        /// <summary>
        ///     Called by Alteryx when it wants the tool to update its progress.
        /// </summary>
        /// <param name="dPercent">The new progress percentage.</param>
        public void II_UpdateProgress(double dPercent)
        {
            this._progressUpdatedEventArgs.Progress = dPercent;
            this.ProgressUpdated?.Invoke(this, this._progressUpdatedEventArgs);
        }

        /// <summary>
        ///     Called by Alteryx when the connection is finished sending data.
        /// </summary>
        public void II_Close()
        {
            this.State = ConnectionState.Closed;
            this.Closed?.Invoke(this);
        }

        /// <summary>
        ///     Called by Alteryx to determine whether or not to display debug level messages.
        /// </summary>
        /// <returns>A value which indicates whether or not to show debug messages.</returns>
        public bool ShowDebugMessages() => this._showDebugMessagesFunc();
    }
}