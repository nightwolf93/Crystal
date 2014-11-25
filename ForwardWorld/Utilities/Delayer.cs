using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Reflection;
using System.Text;

namespace Crystal.WorldServer.Utilities
{
    public class Delayer<T> : IDisposable
    {
        private MethodInfo _method = null;
        private object[] _parameters = null;
        private int _offset;
        private Timer _delayer;
        private T _obj;

        public Delayer(MethodInfo method, object[] parameters, T obj, int offset, bool start = false)
        {
            this._method = method;
            this._parameters = parameters;
            this._offset = offset;
            this._obj = obj;
            this._delayer = new Timer(this._offset);
            this._delayer.Elapsed += new ElapsedEventHandler(delayedEventElapsed);
            if (start)
            {
                this.Start();
            }
        }

        private void delayedEventElapsed(object sender, ElapsedEventArgs e)
        {
            _delayer.Close();
            _delayer.Enabled = false;
            this._method.Invoke(this._obj, this._parameters);
        }

        public void Start()
        {
            this._delayer.Enabled = true;
            this._delayer.Start();
        }

        public void Dispose()
        {
            this._method = null;
            this._parameters = null;
        }
    }
}
