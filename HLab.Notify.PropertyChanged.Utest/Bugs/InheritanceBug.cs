﻿using System;
using Xunit;

namespace HLab.Notify.PropertyChanged.UTest.Bugs
{
    class DataLocker<T> : NotifierBase
    {
        public DataLocker() => H<DataLocker<T>>.Initialize(this);

        public bool IsActive
        {
            get => _isActive.Get();
            set => _isActive.Set(value);
        }
        private readonly IProperty<bool> _isActive = H<DataLocker<T>>.Property<bool>();
        public T Model
        {
            get => _model.Get();
            set => _model.Set(value);
        }
        private readonly IProperty<T> _model = H<DataLocker<T>>.Property<T>();
    }

    abstract class ViewModel<T> : NotifierBase
    {
        protected ViewModel() => H<ViewModel<T>>.Initialize(this);
        public T Model
        {
            get => _model.Get();
            set => _model.Set(value);
        }
        private readonly IProperty<T> _model = H<ViewModel<T>>.Property<T>();

    }



    class BaseClass<T> : ViewModel<T>
    {

        private Func<T, DataLocker<T>> _getLocker;
        public void InitBase(Func<T,DataLocker<T>> getLocker)
        {
            _getLocker = getLocker;
            H<BaseClass<T>>.Initialize(this);
        }


        public DataLocker<T> Locker => _locker.Get();
        private readonly IProperty<DataLocker<T>> _locker = H<BaseClass<T>>.Property<DataLocker<T>>(c => c
            .NotNull(e => e.Model)
            .Set(e => e.GetLocker())
            .On(e => e.Model)
            .Update()
        );

        public int Count = 0;

        private DataLocker<T> GetLocker()
        {
            Count++;
            var locker = _getLocker(Model);
            //locker.PropertyChanged += Locker_PropertyChanged;
            return locker;
        }

    }
    class FrontClass : BaseClass<object>
    {
        public void  Init() => H<FrontClass>.Initialize(this);

        public bool Test => _test.Get();
        private readonly IProperty<bool> _test = H<FrontClass>.Property<bool>(c => c
            .Set(e => e.Locker.IsActive)
            .On(e => e.Locker.IsActive)
            .Update()
        );
    }

    public class InheritanceBug
    {
        [Fact]
        public void Test()
        {
            var f = new FrontClass();

            f.InitBase(o => new DataLocker<object> {Model = o});
            f.Init();

            f.Model = new object();

            f.Locker.IsActive = true;

            Assert.True(f.Test);
        }
    }
}
