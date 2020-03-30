using System;
using System.IO;
using FluentAssertions;
using Microsoft.Build.Framework;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace PosInformatique.MSBuild.Yaml.Tests
{
    public class YamlToJsonTest
    {
        [Fact]
        public void Constructor()
        {
            var task = new YamlToJson();

            task.WithIndentation.Should().BeTrue();
        }

        [Fact]
        public void YamlInputFile_ValueChanged()
        {
            var task = new YamlToJson();

            task.YamlInputFile = "The yaml file";
            task.YamlInputFile.Should().Be("The yaml file");
        }

        [Fact]
        public void JsonOutputFile_ValueChanged()
        {
            var task = new YamlToJson();

            task.JsonOutputFile = "The JSON file";
            task.JsonOutputFile.Should().Be("The JSON file");
        }

        [Fact]
        public void Execute_WithIndentation()
        {
            if (Directory.Exists(@"sub\folder\"))
            {
                Directory.Delete(@"sub\folder\", true);
            }

            Directory.CreateDirectory(@"sub\folder\");

            var yaml = @"
object:
- value: 'v1'
- innerObject:
  innerValue: 1234
";

            File.WriteAllText(@"sub\folder\yaml.yaml", yaml);

            var buildEngine = new Mock<IBuildEngine>(MockBehavior.Strict);
            buildEngine.Setup(b => b.ProjectFileOfTaskNode)
                .Returns("The project file");
            buildEngine.Setup(b => b.LogMessageEvent(It.IsAny<BuildMessageEventArgs>()))
                .Callback((BuildMessageEventArgs args) =>
                {
                    args.Message.Should().Be("Converting the YAML file \"sub\\folder\\yaml.yaml\" to the \"sub\\folder\\json.json\" JSON file.");
                });

            var task = new YamlToJson()
            {
                BuildEngine = buildEngine.Object,
                YamlInputFile = @"sub\folder\yaml.yaml",
                JsonOutputFile = @"sub\folder\json.json",
                WithIndentation = true
            };

            var result = task.Execute();

            result.Should().BeTrue();

            var json = File.ReadAllText(@"sub\folder\json.json");

            json.Should().Be("{\r\n  \"object\": [\r\n    {\r\n      \"value\": \"v1\"\r\n    },\r\n    {\r\n      \"innerObject\": null,\r\n      \"innerValue\": \"1234\"\r\n    }\r\n  ]\r\n}");

            buildEngine.Verify(b => b.LogMessageEvent(It.IsAny<BuildMessageEventArgs>()));
        }

        [Fact]
        public void Execute_WithNoIndentation()
        {
            if (Directory.Exists(@"sub\folder\"))
            {
                Directory.Delete(@"sub\folder\", true);
            }

            Directory.CreateDirectory(@"sub\folder\");

            var yaml = @"
object:
- value: 'v1'
- innerObject:
  innerValue: 1234
";

            File.WriteAllText(@"sub\folder\yaml.yaml", yaml);

            var buildEngine = new Mock<IBuildEngine>(MockBehavior.Strict);
            buildEngine.Setup(b => b.ProjectFileOfTaskNode)
                .Returns("The project file");
            buildEngine.Setup(b => b.LogMessageEvent(It.IsAny<BuildMessageEventArgs>()))
                .Callback((BuildMessageEventArgs args) =>
                {
                    args.Message.Should().Be("Converting the YAML file \"sub\\folder\\yaml.yaml\" to the \"sub\\folder\\json.json\" JSON file.");
                });

            var task = new YamlToJson()
            {
                BuildEngine = buildEngine.Object,
                YamlInputFile = @"sub\folder\yaml.yaml",
                JsonOutputFile = @"sub\folder\json.json",
                WithIndentation = false
            };

            var result = task.Execute();

            result.Should().BeTrue();

            var json = File.ReadAllText(@"sub\folder\json.json");

            json.Should().Be("{\"object\":[{\"value\":\"v1\"},{\"innerObject\":null,\"innerValue\":\"1234\"}]}");
           
            buildEngine.Verify(b => b.LogMessageEvent(It.IsAny<BuildMessageEventArgs>()));
        }

        [Fact]
        public void Execute_WithNotExistingFile()
        {
            var buildEngine = new Mock<IBuildEngine>(MockBehavior.Strict);
            buildEngine.Setup(b => b.ProjectFileOfTaskNode)
                .Returns("The project file");
            buildEngine.Setup(b => b.LineNumberOfTaskNode)
                .Returns(1234);
            buildEngine.Setup(b => b.ColumnNumberOfTaskNode)
                .Returns(5678);
            buildEngine.Setup(b => b.LogErrorEvent(It.IsAny<BuildErrorEventArgs>()))
                .Callback((BuildErrorEventArgs args) =>
                {
                    args.Message.Should().Be("The \"not_existing_file.yaml\" YAML input file does not exist.");
                });

            var task = new YamlToJson()
            {
                BuildEngine = buildEngine.Object,
                YamlInputFile = @"not_existing_file.yaml"
            };

            var result = task.Execute();

            result.Should().BeFalse();

            buildEngine.Verify(b => b.LogErrorEvent(It.IsAny<BuildErrorEventArgs>()));
        }
    }
}
