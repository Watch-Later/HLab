﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HLab.Notify.PropertyChanged.UTest
{
    class TestChild : NotifierBase
    {
        public TestChild()
        {
            H<TestChild>.Initialize(this);
        }
        public int Value
        {
            get => _value.Get();
            set => _value.Set(value);
        }
        private readonly IProperty<int> _value = H<TestChild>.Property<int>();
        public TestChild Child
        {
            get => _child.Get();
            set => _child.Set(value);
        }
        private readonly IProperty<TestChild> _child = H<TestChild>.Property<TestChild>();
    }

    class TestParent : NotifierBase
    {
        public TestParent(TestChild a)
        {
            H<TestParent>.Initialize(this);
            Child = a;
        }
        public TestChild Child
        {
            get => _child.Get();
            set => _child.Set(value);
        }
        private readonly IProperty<TestChild> _child = H<TestParent>.Property<TestChild>();

        public int Value => _value.Get();
        private readonly IProperty<int> _value = H<TestParent>.Property<int>(c=>c
            .Set(e=>e.Child.Child.Value)
            .On(e => e.Child.Child.Value)
            .Update()
        );
    }

    public class UnitTest_ReadonlyProperty
    {
        [Fact]
        public void TestReadonlyProperty()
        {
            var obj = new TestParent(new TestChild{Child=new TestChild{Value=-1}});

            Assert.Equal(-1,obj.Value);

            obj.Child.Child.Value = 1;

            Assert.Equal(1,obj.Value);

            obj.Child.Child.Value = 2;

            Assert.Equal(2,obj.Value);
        }
    }
}
