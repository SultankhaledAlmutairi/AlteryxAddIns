using System;
using System.Collections.Generic;
using System.Linq;

using AlteryxRecordInfoNet;

using OmniBus.Framework;
using OmniBus.Framework.Attributes;
using OmniBus.Framework.Factories;
using OmniBus.Framework.Interfaces;
using OmniBus.Framework.Serialisation;
using OmniBus.Framework.TypeConverters;

namespace OmniBus
{
    /// <summary>
    ///     Alteryx Engine to Sort Data With A Culture
    /// </summary>
    public class SortWithCultureEngine : BaseEngine<SortWithCultureConfig>
    {
        private IRecordCopier _copier;

        private List<Tuple<string, Record>> _data;
        private FieldBase _inputFieldBase;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SortWithCultureEngine" /> class.
        ///     Constructor For Alteryx
        /// </summary>
        public SortWithCultureEngine()
            : this(new RecordCopierFactory(), new InputPropertyFactory(), new OutputHelperFactory())
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SortWithCultureEngine" /> class.
        ///     Create An Engine for unit testing.
        /// </summary>
        /// <param name="recordCopierFactory">Factory to create copiers</param>
        /// <param name="inputPropertyFactory">Factory to create input properties</param>
        /// <param name="outputHelperFactory">Factory to create output helpers</param>
        internal SortWithCultureEngine(
            IRecordCopierFactory recordCopierFactory,
            IInputPropertyFactory inputPropertyFactory,
            IOutputHelperFactory outputHelperFactory)
            : base(recordCopierFactory, outputHelperFactory)
        {
            this.Input = inputPropertyFactory.Build(recordCopierFactory, this.ShowDebugMessages);
            this.Input.InitCalled += (sender, args) => args.Success = this.InitFunc(this.Input.RecordInfo);
            this.Input.RecordPushed += (sender, args) => args.Success = this.PushFunc(args.RecordData);
            this.Input.Closed += sender => this.ClosedAction();
        }

        /// <summary>
        ///     Gets the input stream.
        /// </summary>
        [CharLabel('I')]
        public IInputProperty Input { get; }

        /// <summary>
        ///     Gets or sets the output stream.
        /// </summary>
        [CharLabel('O')]
        public IOutputHelper Output { get; set; }

        /// <summary>Create a Serialiser</summary>
        /// <returns><see cref="T:OmniBus.Framework.Serialisation.ISerialiser`1" /> to de-serialise object</returns>
        protected override ISerialiser<SortWithCultureConfig> Serialiser() => new Serialiser<SortWithCultureConfig>();

        private bool InitFunc(RecordInfo info)
        {
            this._inputFieldBase = info.GetFieldByName(this.ConfigObject.SortField, false);
            if (this._inputFieldBase == null)
            {
                return false;
            }

            this.Output?.Init(FieldDescription.CreateRecordInfo(info));

            // Create the Copier
            this._copier = this.RecordCopierFactory.CreateCopier(info, this.Output?.RecordInfo);

            this._data = new List<Tuple<string, Record>>();

            return true;
        }

        private bool PushFunc(RecordData r)
        {
            var record = this.Output.Record;
            this._copier.Copy(record, r);

            var input = this._inputFieldBase.GetAsString(r);
            this._data.Add(Tuple.Create(input, record));
            return true;
        }

        private void ClosedAction()
        {
            var culture = CultureTypeConverter.GetCulture(this.ConfigObject.Culture);
            var comparer = StringComparer.Create(culture, this.ConfigObject.IgnoreCase);

            var count = 0;
            foreach (var record in this._data.OrderBy(t => t.Item1, comparer).Select(t => t.Item2))
            {
                var d = count++ / (double)this._data.Count;
                this.Output?.UpdateProgress(d, true);
                this.Output?.Push(record);
            }

            this.Output?.Close(true);
        }
    }
}