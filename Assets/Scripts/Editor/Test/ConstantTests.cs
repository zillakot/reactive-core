using System;
using System.Reflection;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using UniRx;

namespace UnityTest
{
    [TestFixture]
    [Category("Constant Tests")]
    public class ConstantTests
    {
        [Test]
        public void LayersTest(){
            FieldInfo[] fields = typeof(Layers).GetFields();
            //fields.ToList();
            Assert.Pass();
        }
    }
}