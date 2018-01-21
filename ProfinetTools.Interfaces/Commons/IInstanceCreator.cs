using System;
using System.Linq;
using Ninject.Parameters;

namespace ProfinetTools.Interfaces.Commons
{
    public interface IInstanceCreator
    {
        T CreateInstance<T>(ConstructorArgument[] arguments);
    }
}