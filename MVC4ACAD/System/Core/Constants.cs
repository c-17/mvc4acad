using System;
using System.Reflection;

namespace MVC4ACAD.Core{
    internal static class Constantes{
        internal static Int32 AutoCADVersion{
            get{
                Int32[] Versions = {2013, 2014, 2015, 2016, 2017, 2018, 2019, 2020, 2021, 2020, 2021, 2022};

                foreach(Int32 Version in Versions){
                    if(Assembly.GetExecutingAssembly().Location.Contains(Version.ToString()))
                        return Version;
                    }

                return Versions[Versions.Length-1];
                }
            }
        }
    }
