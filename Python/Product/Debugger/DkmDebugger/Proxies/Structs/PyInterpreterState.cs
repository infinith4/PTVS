// Python Tools for Visual Studio
// Copyright(c) Microsoft Corporation
// All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the License); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS
// OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY
// IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABLITY OR NON-INFRINGEMENT.
//
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

using System.Collections.Generic;
using Microsoft.VisualStudio.Debugger;

namespace Microsoft.PythonTools.DkmDebugger.Proxies.Structs {
    internal class PyInterpreterState : StructProxy {
        private class Fields {
            public StructField<PointerProxy<PyInterpreterState>> next;
            public StructField<PointerProxy<PyThreadState>> tstate_head;
            public StructField<PointerProxy<PyDictObject>> modules;
        }

        private readonly Fields _fields;

        public PyInterpreterState(DkmProcess process, ulong address)
            : base(process, address) {
            InitializeStruct(this, out _fields);
        }

        public PointerProxy<PyInterpreterState> next {
            get { return GetFieldProxy(_fields.next); }
        }

        public PointerProxy<PyThreadState> tstate_head {
            get { return GetFieldProxy(_fields.tstate_head); }
        }

        public PointerProxy<PyDictObject> modules {
            get { return GetFieldProxy(_fields.modules); }
        }

        private class InterpHeadHolder : DkmDataItem {
            public readonly PointerProxy<PyInterpreterState> Proxy;

            public InterpHeadHolder(DkmProcess process) {
                Proxy = process.GetPythonRuntimeInfo().DLLs.Python.GetStaticVariable<PointerProxy<PyInterpreterState>>("interp_head");
            }
        }

        public static PointerProxy<PyInterpreterState> interp_head(DkmProcess process) {
            return process.GetOrCreateDataItem(() => new InterpHeadHolder(process)).Proxy;
        }

        public static IEnumerable<PyInterpreterState> GetInterpreterStates(DkmProcess process) {
            for (var interp = interp_head(process).TryRead(); interp != null; interp = interp.next.TryRead()) {
                yield return interp;
            }
        }

        public IEnumerable<PyThreadState> GetThreadStates() {
            for (var tstate = tstate_head.TryRead(); tstate != null; tstate = tstate.next.TryRead()) {
                yield return tstate;
            }
        }
    }
}
