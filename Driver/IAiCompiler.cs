using Scai.Driver.Data;

namespace Scai.Driver
{
    public interface IAiCompiler
    {
        CompilationResult Compile(string code);
    }
}
