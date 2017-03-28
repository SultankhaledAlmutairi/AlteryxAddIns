﻿using OmniBus;
using OmniBus.Framework;

namespace JDunkerley.AlteryxAddIns
{
    /// <summary>
    /// Replicate the Tableau HexBin functions
    /// </summary>
    public class HexBin : BaseTool<HexBinConfig, HexBinEngine>, AlteryxGuiToolkit.Plugins.IPlugin
    {
        /// <summary>
        /// Place Holder for Old Entry Point
        /// </summary>
        public class Engine : HexBinEngine
        {
        }
    }
}