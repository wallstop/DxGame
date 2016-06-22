using System;
using System.Diagnostics;

namespace DxCore.Core.Utils.Validate
{
    public static class Validate
    {
        internal sealed class CheckValidator : Validator
        {
            public static readonly CheckValidator CheckInstance = new CheckValidator();

            private CheckValidator() {}

            protected override void TestFailure(Func<string> messageProducer)
            {
                // No-op
            }
        }

        internal sealed class AssertValidator : Validator
        {
            public static readonly AssertValidator AssertInstance = new AssertValidator();

            private AssertValidator() {}

            protected override void TestFailure(Func<string> messageProducer)
            {
                Debug.Assert(false, messageProducer.Invoke());
            }
        }

        internal sealed class FailValidator : Validator
        {
            public static readonly FailValidator FailInstance = new FailValidator();

            private FailValidator() {}

            protected override void TestFailure(Func<string> messageProducer)
            {
                throw new ArgumentException(messageProducer.Invoke());
            }
        }

        public static Validator Hard => FailValidator.FailInstance;
        public static Validator Check => CheckValidator.CheckInstance;
        public static Validator Assert => AssertValidator.AssertInstance;
    }
}