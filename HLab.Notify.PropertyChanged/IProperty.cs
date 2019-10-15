﻿using System.Runtime.CompilerServices;

namespace HLab.Notify.PropertyChanged
{
    public interface IProperty<T> : IProperty, IChildObject
    {
        bool Set(T value);


#if DEBUG
        T Get([CallerMemberName]string name=null);
#else
        T Get();
#endif
    }

    public interface IProperty
    {
    }
}