﻿#region License
/* 
 * Copyright (c) 2016, Roland Harrison
 * roland.c.harrison@gmail.com
 *
 * Copyright (c) 2010, Michael Kelly
 * michael.e.kelly@gmail.com
 * http://michael-kelly.com/ 
 *  
 * (c) 2015 kuzkok
 * kuzkok@gmail.com
 * 
 * All rights reserved.
 * Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
 * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
 * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
 * Neither the name of the organization nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 */
#endregion License
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace J2534DotNet
{
    public class J2534Extended : J2534, IJ2534Extended
    {
        unsafe public J2534Err GetConfig(int channelId, ref List<SConfig> config)
        {
            SConfigList sConfigList = new SConfigList();
            sConfigList.ListPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SConfig)) * config.Count);

            for (int i = 0; i < config.Count; i++) { 
                Marshal.StructureToPtr(config[i], new IntPtr(sConfigList.ListPtr.ToInt64() + Marshal.SizeOf(typeof(SConfig))*i), false);
            }

            IntPtr output = IntPtr.Zero;
            IntPtr input = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SConfigList)));

            Marshal.StructureToPtr(sConfigList, input, false);

            var err = (J2534Err)_wrapper.Ioctl(channelId, (int)Ioctl.GET_CONFIG, input, output);

            var configList = input.AsStruct<SConfigList>().GetList();


            config = new List<SConfig>();
            for (int i = 0; i < configList.Count; i++)
            {
                config.Add(configList[i]);
            }

            return err;

        }


        public J2534Err SetConfig(int channelId, ref List<SConfig> config)
        {
            IntPtr input = IntPtr.Zero;
            IntPtr output = IntPtr.Zero;

            return (J2534Err)_wrapper.Ioctl(channelId, (int)Ioctl.SET_CONFIG, input, output);
        }

        public J2534Err ReadBatteryVoltage(int deviceId, ref int voltage)
        {
            IntPtr input = IntPtr.Zero;
            IntPtr output = Marshal.AllocHGlobal(8);

            J2534Err returnValue = (J2534Err)_wrapper.Ioctl(deviceId, (int)Ioctl.READ_VBATT, input, output);
            if (returnValue == J2534Err.STATUS_NOERROR)
            {
                voltage = Marshal.ReadInt32(output);
            }

            Marshal.FreeHGlobal(output);

            return returnValue;
        }

        public J2534Err ReadProgrammingVoltage(int deviceId, ref int voltage)
        {
            IntPtr input = IntPtr.Zero;
            IntPtr output = Marshal.AllocHGlobal(8);

            voltage = (int)_wrapper.Ioctl(deviceId, (int)Ioctl.READ_PROG_VOLTAGE, input, output);
            //if (returnValue == J2534Err.STATUS_NOERROR)
            //{
            //}

            Marshal.FreeHGlobal(output);

            return J2534Err.STATUS_NOERROR;
        }

        public J2534Err FiveBaudInit(int channelId, byte targetAddress, ref byte keyword1, ref byte keyword2)
        {
            J2534Err returnValue;
            IntPtr input = IntPtr.Zero;
            IntPtr output = IntPtr.Zero;

            SByteArray inputArray = new SByteArray();
            SByteArray outputArray = new SByteArray();
            inputArray.NumOfBytes = 1;
            unsafe
            {
                //inputArray.BytePtr[0] = targetAddress;
                outputArray.NumOfBytes = 2;

                Marshal.StructureToPtr(inputArray, input, true);
                Marshal.StructureToPtr(outputArray, output, true);

                returnValue = (J2534Err)_wrapper.Ioctl(channelId, (int)Ioctl.FIVE_BAUD_INIT, input, output);

                Marshal.PtrToStructure(output, outputArray);
            }
            return returnValue;
        }

        public J2534Err FastInit(int channelId, PassThruMsg txMsg, ref PassThruMsg rxMsg)
        {
            IntPtr input = IntPtr.Zero;
            IntPtr output = IntPtr.Zero;
            PassThruMsg uRxMsg = new PassThruMsg();

            Marshal.StructureToPtr(txMsg, input, true);
            Marshal.StructureToPtr(uRxMsg, output, true);

            J2534Err returnValue = (J2534Err)_wrapper.Ioctl(channelId, (int)Ioctl.FAST_INIT, input, output);
            if (returnValue == J2534Err.STATUS_NOERROR)
            {
                Marshal.PtrToStructure(output, rxMsg);
            }

            return returnValue;
        }

        public J2534Err ClearTxBuffer(int channelId)
        {
            IntPtr input = IntPtr.Zero;
            IntPtr output = IntPtr.Zero;

            return (J2534Err)_wrapper.Ioctl(channelId, (int)Ioctl.CLEAR_TX_BUFFER, input, output);
        }

        public J2534Err ClearRxBuffer(int channelId)
        {
            IntPtr input = IntPtr.Zero;
            IntPtr output = IntPtr.Zero;

            return (J2534Err)_wrapper.Ioctl(channelId, (int)Ioctl.CLEAR_RX_BUFFER, input, output);
        }

        public J2534Err ClearPeriodicMsgs(int channelId)
        {
            IntPtr input = IntPtr.Zero;
            IntPtr output = IntPtr.Zero;

            return (J2534Err)_wrapper.Ioctl(channelId, (int)Ioctl.CLEAR_PERIODIC_MSGS, input, output);
        }

        public J2534Err ClearMsgFilters(int channelId)
        {
            IntPtr input = IntPtr.Zero;
            IntPtr output = IntPtr.Zero;

            return (J2534Err)_wrapper.Ioctl(channelId, (int)Ioctl.CLEAR_MSG_FILTERS, input, output);
        }

        public J2534Err ClearFunctMsgLookupTable(int channelId)
        {
            IntPtr input = IntPtr.Zero;
            IntPtr output = IntPtr.Zero;

            return (J2534Err)_wrapper.Ioctl(channelId, (int)Ioctl.CLEAR_FUNCT_MSG_LOOKUP_TABLE, input, output);
        }

        public J2534Err AddToFunctMsgLookupTable(int channelId)
        {
            IntPtr input = IntPtr.Zero;
            IntPtr output = IntPtr.Zero;
            // TODO: fix this
            return (J2534Err)_wrapper.Ioctl(channelId, (int)Ioctl.ADD_TO_FUNCT_MSG_LOOKUP_TABLE, input, output);
        }

        public J2534Err DeleteFromFunctMsgLookupTable(int channelId)
        {
            IntPtr input = IntPtr.Zero;
            IntPtr output = IntPtr.Zero;
            // TODO: fix this
            return (J2534Err)_wrapper.Ioctl(channelId, (int)Ioctl.DELETE_FROM_FUNCT_MSG_LOOKUP_TABLE, input, output);
        }

        /// <summary>
        /// Poll for messages until we get a timeout
        /// </summary>
        /// <param name="numMsgs"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public J2534Err ReadAllMessages(int channelId, int numMsgs, int timeout, out List<PassThruMsg> messages, bool readUntilTimeout = true)
        {
            messages = new List<PassThruMsg>();

            IntPtr rxMsgs = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(PassThruMsg)) * numMsgs);
            var m_status = J2534Err.STATUS_NOERROR;
            var m_status2 = J2534Err.STATUS_NOERROR;

            //Read the first block of messages
            m_status = PassThruReadMsgs(channelId, rxMsgs, ref numMsgs, timeout);
            if (m_status == J2534Err.STATUS_NOERROR)
            {
                var msgs = rxMsgs.AsMsgList(numMsgs);
                if (msgs.Count > 0) messages.AddRange(msgs);

                //If we are only reading this block then return now
                if (!readUntilTimeout) return m_status;
            }
            else
            {
                var msgs = rxMsgs.AsMsgList(numMsgs);
                if (msgs.Count > 0) messages.AddRange(msgs);
                //If we failed on the first read give up now
                return m_status;
            }

            //We successfully read one block, now keep going
            while (J2534Err.STATUS_NOERROR == m_status2)
            {
                m_status2 = PassThruReadMsgs(channelId, rxMsgs, ref numMsgs, timeout);
                if (m_status2 == J2534Err.STATUS_NOERROR)
                {
                    var msgs = rxMsgs.AsMsgList(numMsgs);
                    foreach (var msg in msgs) messages.Add(msg);
                }
                else
                {
                    break;
                }
            }

            return J2534Err.STATUS_NOERROR;
        }

    }

    public class J2534Exception : Exception
    {
        J2534Err _error;
        public J2534Err Error
        {
            get { return _error; }
        }
        public J2534Exception(J2534Err error) : base(error.ToString())
        {
            _error = error;
        }
    }
}
