﻿using System;
using System.IO;
using ShaderPlayground.Core.Util;

namespace ShaderPlayground.Core.Compilers.XShaderCompiler
{
    internal sealed class XscCompiler : IShaderCompiler
    {
        public string Name { get; } = CompilerNames.XShaderCompiler;
        public string DisplayName { get; } = "XShaderCompiler";
        public string Description { get; } = "Lukas Hermanns' XShaderCompiler";

        public string[] InputLanguages { get; } = { LanguageNames.Hlsl };

        public ShaderCompilerParameter[] Parameters { get; } =
        {
            CommonParameters.GlslShaderStage,
            CommonParameters.HlslEntryPoint,
            CommonParameters.CreateOutputParameter(new[] { LanguageNames.Glsl }),
            new ShaderCompilerParameter("GlslVersion", "GLSL Version", ShaderCompilerParameterType.ComboBox, GlslVersionOptions, "GLSL"),
        };

        private static readonly string[] GlslVersionOptions =
        {
            "GLSL",
            "GLSL130",
            "GLSL140",
            "GLSL150",
            "GLSL330",
            "GLSL400",
            "GLSL410",
            "GLSL420",
            "GLSL430",
            "GLSL440",
            "GLSL450",
            "GLSL460",
        };

        public ShaderCompilerResult Compile(ShaderCode shaderCode, ShaderCompilerArguments arguments)
        {
            var stage = arguments.GetString("ShaderStage");
            var entryPoint = arguments.GetString("EntryPoint");
            var glslVersion = arguments.GetString("GlslVersion");

            using (var tempFile = TempFile.FromShaderCode(shaderCode))
            {
                var outputPath = $"{tempFile.FilePath}.out";

                ProcessHelper.Run(
                    Path.Combine(AppContext.BaseDirectory, "Binaries", "XShaderCompiler", "xsc.exe"),
                    $"-T {stage} -E {entryPoint} -Vout {glslVersion} -o \"{outputPath}\" \"{tempFile.FilePath}\"",
                    out var stdOutput,
                    out var _);

                var textOutput = FileHelper.ReadAllTextIfExists(outputPath);

                var hasCompilationErrors = string.IsNullOrWhiteSpace(textOutput);

                FileHelper.DeleteIfExists(outputPath);

                return new ShaderCompilerResult(
                    new ShaderCode(LanguageNames.Glsl, textOutput),
                    hasCompilationErrors ? (int?) 1 : null,
                    new ShaderCompilerOutput("Output", LanguageNames.Glsl, textOutput),
                    new ShaderCompilerOutput("Build output", null, stdOutput));
            }
        }
    }
}
