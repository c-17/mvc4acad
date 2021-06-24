using System;
using System.Collections.Generic;

namespace MVC4ACAD.Core{
    internal static class Extensiones{
        public static IEnumerable<String> Split(this String String, Int32 Length){
            for(Int32 index = 0; index < String.Length; index += Length)
                yield return String.Substring(index, Math.Min(Length, String.Length - index));
            }
        }
    }
