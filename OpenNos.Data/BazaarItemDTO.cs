﻿// This file is part of the OpenNos NosTale Emulator Project.
// 
// This program is licensed under a deviated version of the Fair Source License,
// granting you a non-exclusive, non-transferable, royalty-free and fully-paid-up
// license, under all of the Licensor's copyright and patent rights, to use, copy, prepare
// derivative works of, publicly perform and display the Software, subject to the
// conditions found in the LICENSE file.
// 
// THIS FILE IS PROVIDED "AS IS", WITHOUT WARRANTY OR
// CONDITION, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. THE AUTHORS HEREBY DISCLAIM ALL LIABILITY, WHETHER IN
// AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE.
using System;

namespace OpenNos.Data
{
    [Serializable]
    public class BazaarItemDTO
    {
        #region Properties

        public byte Amount { get; set; }

        public long BazaarItemId { get; set; }

        public DateTime DateStart { get; set; }

        public short Duration { get; set; }

        public bool IsPackage { get; set; }

        public Guid ItemInstanceId { get; set; }

        public bool MedalUsed { get; set; }

        public long Price { get; set; }

        public long SellerId { get; set; }

        #endregion
    }
}