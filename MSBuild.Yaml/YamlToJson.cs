//-----------------------------------------------------------------------
// <copyright file="YamlToJson.cs" company="DiliTrust">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace PosInformatique.MSBuild.Yaml
{
    using System;
    using System.IO;
    using Microsoft.Build.Framework;
    using Newtonsoft.Json;
    using YamlDotNet.Serialization;

    /// <summary>
    /// MSBuild task which convert a YAML file into a JSON file.
    /// </summary>
    public sealed class YamlToJson : Microsoft.Build.Utilities.Task
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YamlToJson"/> class.
        /// </summary>
        public YamlToJson()
        {
            this.WithIndentation = true;
        }

        /// <summary>
        /// Gets or sets the path of the input YAML file to convert to JSON.
        /// </summary>
        [Required]
        public string YamlInputFile
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the path of the output JSON file to generate.
        /// </summary>
        [Required]
        public string JsonOutputFile
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating if the output <see cref="JsonOutputFile"/> must be indented.
        /// By default, the <see cref="WithIndentation"/> is defined to <see langword="true"/>.
        /// </summary>
        public bool WithIndentation
        {
            get;
            set;
        }

        /// <inheritdoc />
        public override bool Execute()
        {
            if (!File.Exists(this.YamlInputFile))
            {
                this.Log.LogError(MsBuildYamlResources.NoYamlInputFile, this.YamlInputFile);

                return false;
            }

            if (this.WithIndentation)
            {
                this.Log.LogMessage(MessageImportance.Normal, MsBuildYamlResources.LogConvertingYamlFile, this.YamlInputFile, this.JsonOutputFile, MsBuildYamlResources.WithIndentation);
            }
            else
            {
                this.Log.LogMessage(MessageImportance.Normal, MsBuildYamlResources.LogConvertingYamlFile, this.YamlInputFile, this.JsonOutputFile, MsBuildYamlResources.WithoutIndentation);
            }

            var yamlDeserializer = new Deserializer();

            using (var inputReader = new StreamReader(this.YamlInputFile))
            {
                var yamlContent = yamlDeserializer.Deserialize(inputReader);

                var jsonSerializer = new JsonSerializer();

                if (this.WithIndentation)
                {
                    jsonSerializer.Formatting = Formatting.Indented;
                }

                using (var outputWriter = new StreamWriter(this.JsonOutputFile))
                {
                    jsonSerializer.Serialize(outputWriter, yamlContent);
                }
            }

            return true;
        }
    }
}
