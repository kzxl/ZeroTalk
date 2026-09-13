using System;
using System.Collections.Generic;

namespace ZeroTalk.Tests
{
    public static class Assert
    {
        public static void True(bool condition, string message = null)
        {
            if (!condition)
                throw new Exception(message ?? "Assert.True failed: condition was false.");
        }

        public static void False(bool condition, string message = null)
        {
            if (condition)
                throw new Exception(message ?? "Assert.False failed: condition was true.");
        }

        public static void Equal<T>(T expected, T actual, string message = null)
        {
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
                throw new Exception(message ?? $"Assert.Equal failed: expected '{expected}', actual '{actual}'.");
        }

        public static void NotNull(object obj, string message = null)
        {
            if (obj == null)
                throw new Exception(message ?? "Assert.NotNull failed: object was null.");
        }
    }
}
