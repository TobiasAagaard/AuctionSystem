using System.Reflection;

namespace Auctionly.Modules.Auctions.Application;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}