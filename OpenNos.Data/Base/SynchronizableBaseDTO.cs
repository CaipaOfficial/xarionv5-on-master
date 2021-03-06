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

namespace OpenNos.Data.Base
{
    [Serializable]
    public abstract class SynchronizableBaseDTO
    {
        #region Instantiation

        protected SynchronizableBaseDTO() => Id = Guid.NewGuid();

        #endregion

        // make unique

        #region Properties

        public Guid Id { get; set; }

        #endregion

        #region Methods

        public override bool Equals(object obj) => obj != null && ((SynchronizableBaseDTO) obj).Id == Id;

        // ReSharper disable once NonReadonlyMemberInGetHashCode
        public override int GetHashCode() => Id.GetHashCode();

        #endregion
    }
}