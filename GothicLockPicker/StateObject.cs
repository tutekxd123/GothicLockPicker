using System;
using System.Collections.Generic;
using System.Text;

namespace GothicLockPicker
{
    public class ConnectionsLock
    {
        Lock? LockNext;
        bool IsRight;
        public ConnectionsLock(Lock lockNext, bool isReverse)
        {
            LockNext = lockNext;
            IsRight = isReverse;
        }
    }
    public class Lock
    {
        private int _value = 0;
        public int Value
        {
            get { return _value; }
            set
            {
                if(value<0 || value > 7)
                {
                    return;
                }
                this._value = value;
            }
        }
        protected List<ConnectionsLock> Connections = new();
        public Lock(int value)
        {
            this._value = value;
        }
        public void changeConnections(List<ConnectionsLock> connections)
        {
            this.Connections = connections;
        }
        public Lock(int value, List<ConnectionsLock> connections)
        {
            this._value = value;
            Connections = connections;
        }
    }
}
