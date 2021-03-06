﻿namespace ShaderPlayground.Core
{
    public interface IShaderCompiler
    {
        string Name { get; }
        string DisplayName { get; }
        string Description { get; }

        string[] InputLanguages { get; }
        
        ShaderCompilerParameter[] Parameters { get; }

        ShaderCompilerResult Compile(ShaderCode shaderCode, ShaderCompilerArguments arguments);
    }
}
