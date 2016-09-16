﻿using J2534DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OBD
{
    /// <summary>
    /// J2534 Specific OBD implementation
    /// </summary>
    public interface OBDInterface
    {
        bool IsConnected();
        bool DetectProtocol();
        bool Disconnect();

        void ReadObdPid(OBDcmd.Mode mode, out byte[] payload, byte pid = 0);

        void GetAvailableObdPidsAt(byte start, ref List<byte> availablePids);

        bool GetAvailableObdPids(ref List<byte> availablePids);

        bool ClearFaults();

        string GetVin();

        bool GetBatteryVoltage(ref double voltage);

        bool SendMessage(byte [] txMsgBytes);

    }

}

        

